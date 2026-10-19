using System;
using System.Collections.Generic;
using System.Reflection;
using ItemStatsSystem;
using UnityEngine;

namespace HeadshotBulletRefund
{
    /// <summary>
    /// 爆头击杀返还 1 颗子弹到枪械弹匣。
    ///
    /// 可配置：数值来自 ModConfig 框架的游戏内设置菜单（主菜单 → 设置 → Mod Settings），
    /// 没装 ModConfig 时读同目录 config.json，都没有则用内置默认值（详见 ModSettings）。
    ///
    /// 性能设计：
    ///  * 只订阅 Health.OnDead 静态事件（游戏自身的任务系统也是这么做的），
    ///    不挂 Harmony 补丁、不轮询、不用协程 —— 没有死亡发生时开销为 0。
    ///  * 事件回调第一行是总开关、第二行按 crit（爆头）短路，绝大多数死亡在此直接返回。
    ///  * 只有真的返还子弹时才写物品、才用一次反射（PropertyInfo 只解析一次）。
    ///  * 常态下唯一的每帧开销是 Update 里两个 bool 判断（设置项就绪后、没有挂起返还时立即返回）。
    ///  * 设置项只在启动阶段注册一次；连不上 ModConfig 时最多重试 15 次（每 2 秒一次）后彻底放弃。
    /// </summary>
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        /// <summary>换弹中挂起的返还最多等多久（秒），超时丢弃，避免长期挂着一个引用。</summary>
        private const float PendingTimeout = 10f;

        // ===== 可配置数值（默认值 = 原行为）=====
        private static ModSettings _settings = new ModSettings();

        /// <summary>ModConfig 的 UI 项每个进程只注册一次，避免 mod 被重新启用时出现重复条目。</summary>
        private static bool _modConfigEntriesAdded;
        private static bool _modConfigHandlerAdded;

        private const int ModConfigMaxAttempts = 15;
        private const float ModConfigRetryInterval = 2f;

        private bool _modConfigReady;
        private bool _modConfigGaveUp;
        private int _modConfigAttempts;
        private float _nextModConfigAttempt;

        // ===== 挂起的返还：换弹中不写入（换弹按开头算好的 needCount 一次性装填，此刻插入会让弹匣溢出）=====
        private static ItemSetting_Gun _pendingSetting;
        private static int _pendingCount;
        private static float _pendingSince;

        // ===== 反射缓存：同步 ItemSetting_Gun 的私有弹药计数缓存（只在返还时用）=====
        private static PropertyInfo _bulletCountProperty;
        private static FieldInfo _bulletCountField;
        private static bool _cacheSyncUnavailable;

        private static bool _prefabMissingWarned;
        private static bool _hooked;

        // ======================= 生命周期 =======================

        protected override void OnAfterSetup()
        {
            _settings = ModSettings.LoadFromModFolder();
            _modConfigGaveUp = false;
            TryRegisterModConfig(); // ModConfig 可能还没加载完（按文件夹名顺序激活），失败时在 Update 里重试

            if (_hooked)
            {
                return;
            }
            Health.OnDead += OnAnyHealthDead;
            _hooked = true;
        }

        protected override void OnBeforeDeactivate()
        {
            if (_modConfigHandlerAdded)
            {
                ModConfigBridge.RemoveChangedHandler(OnModConfigChanged);
                _modConfigHandlerAdded = false;
            }
            _modConfigReady = false;
            _modConfigGaveUp = true;
            if (_hooked)
            {
                Health.OnDead -= OnAnyHealthDead;
                _hooked = false;
            }
            ClearPending();
        }

        private void OnDestroy()
        {
            // 幂等：即使没经过 NotifyBeforeDeactivate，也不留下悬挂的静态委托
            OnBeforeDeactivate();
        }

        private void Update()
        {
            if (!_modConfigReady && !_modConfigGaveUp)
            {
                float now = Time.unscaledTime;
                if (now >= _nextModConfigAttempt)
                {
                    _nextModConfigAttempt = now + ModConfigRetryInterval;
                    _modConfigAttempts++;
                    if (!TryRegisterModConfig() && _modConfigAttempts >= ModConfigMaxAttempts)
                    {
                        _modConfigGaveUp = true;
                        Debug.Log("[HeadshotBulletRefund] 未检测到可用的 ModConfig，停用设置菜单对接；" +
                                  "数值改由 mod 目录下 config.json / 内置默认值决定。");
                    }
                }
            }

            if (_pendingCount <= 0)
            {
                return; // 常态下唯一开销
            }
            TryFlushPending(Time.unscaledTime);
        }

        // ======================= 设置项 =======================

        /// <summary>
        /// 把本 mod 的数值注册进 ModConfig 的设置菜单，并读回当前值。
        /// 成功返回 true；ModConfig 未加载/版本不符/缺方法时返回 false（此时用 config.json 的值）。
        /// </summary>
        private bool TryRegisterModConfig()
        {
            if (_modConfigReady)
            {
                return true;
            }
            if (!ModConfigBridge.TryInit())
            {
                return false;
            }

            if (!_modConfigEntriesAdded)
            {
                ModConfigBridge.AddBoolDropdown("Enabled", "总开关 / Master switch: on = refund", _settings.enabled);
                ModConfigBridge.AddBoolDropdown("HeadshotOnly", "仅爆头击杀时返还 / Only on headshot kills", _settings.headshotOnly);
                ModConfigBridge.AddIntSlider("RefundCount", "每次击杀返还的子弹数 / Bullets per kill",
                    _settings.refundCount, 1, ModSettings.MaxRefundCount);
                ModConfigBridge.AddBoolDropdown("RespectMagazineCapacity",
                    "绝不超过弹匣容量 / Never exceed magazine capacity", _settings.respectMagazineCapacity);
                _modConfigEntriesAdded = true;
            }
            if (!_modConfigHandlerAdded)
            {
                _modConfigHandlerAdded = ModConfigBridge.AddChangedHandler(OnModConfigChanged);
            }
            ReloadFromModConfig();
            _modConfigReady = true;
            return true;
        }

        /// <summary>ModConfig 每次落盘都会回调，参数是它的完整键（HeadshotBulletRefund_xxx）。</summary>
        private static void OnModConfigChanged(string key)
        {
            if (key == null || !key.StartsWith(ModConfigBridge.ChangedKeyPrefix, StringComparison.Ordinal))
            {
                return;
            }
            ReloadFromModConfig();
        }

        /// <summary>把 ModConfig 里的值读回内存（只在注册时和玩家改设置后调用，不参与每帧）。</summary>
        private static void ReloadFromModConfig()
        {
            ModSettings s = _settings;
            s.enabled = ModConfigBridge.Load("Enabled", s.enabled);
            s.headshotOnly = ModConfigBridge.Load("HeadshotOnly", s.headshotOnly);
            s.refundCount = ModConfigBridge.Load("RefundCount", s.refundCount);
            s.respectMagazineCapacity = ModConfigBridge.Load("RespectMagazineCapacity", s.respectMagazineCapacity);
            s.Normalize();
        }

        // ======================= 击杀回调 =======================

        private static void OnAnyHealthDead(Health health, DamageInfo info)
        {
            // 0) 总开关（设置项，默认开）
            if (!_settings.enabled)
            {
                return;
            }
            // 1) 只处理爆头。本作玩家开枪时 damageInfo.critRate = 准星是否对着敌人头部 ? 1 : 0，
            //    再由 Health.Hurt 掷骰写入 crit（0/1），crit > 0 就是游戏自己的“爆头”定义
            //    （爆头音效、取头盔而非防弹衣、任务 requireHeadShot 都以此判定）。
            //    设置里可以关掉这条，变成任意击杀都返还。
            if (_settings.headshotOnly && info.crit <= 0)
            {
                return;
            }
            // 2) 死的是敌人（我方、自己、队友都不算）
            if (health == null || health.team == Teams.player)
            {
                return;
            }
            // 3) 击杀者是主角
            CharacterMainControl killer = info.fromCharacter;
            if (killer == null || !killer.IsMainCharacter)
            {
                return;
            }
            // 4) 靶场（开枪本就不消耗子弹）不返还
            LevelManager level = LevelManager.Instance;
            if (level == null || level.IsBaseLevel)
            {
                return;
            }
            // 5) 主角手上必须是枪
            ItemAgent_Gun gun = killer.CurrentHoldItemAgent as ItemAgent_Gun;
            if (gun == null)
            {
                return;
            }
            ItemSetting_Gun setting = gun.GunItemSetting;
            Item gunItem = gun.Item;
            if (setting == null || gunItem == null)
            {
                return;
            }
            Inventory magazine = gunItem.Inventory;
            if (magazine == null)
            {
                return;
            }
            // 6) 必须就是用这把枪打死的（排除手雷/爆炸/别的武器）
            if (info.fromWeaponItemID > 0 && info.fromWeaponItemID != gunItem.TypeID)
            {
                return;
            }

            if (setting.LoadingBullets)
            {
                // 换弹中：此刻写入会被换弹逻辑覆盖或导致多装一发，先挂起，等换弹结束后补
                EnqueuePending(setting);
                return;
            }
            TryRefund(magazine, setting);
        }

        // ======================= 返还 =======================

        private static bool TryRefund(Inventory magazine, ItemSetting_Gun setting)
        {
            int want = _settings.refundCount;
            if (want < 1)
            {
                want = 1;
            }

            if (_settings.respectMagazineCapacity)
            {
                int capacity = setting.Capacity;
                if (capacity <= 0)
                {
                    return false;
                }
                // 用真实统计值判满，不依赖可能尚未刷新的缓存
                int room = capacity - setting.GetBulletCount();
                if (room <= 0)
                {
                    return false; // 弹匣已满，装不下
                }
                if (want > room)
                {
                    want = room; // 装得下几颗算几颗
                }
            }

            int typeID = ResolveBulletTypeID(setting);
            if (typeID <= 0)
            {
                return false;
            }

            bool added = false;
            for (int i = 0; i < want; i++)
            {
                if (!AddOneBullet(magazine, setting, typeID))
                {
                    break;
                }
                added = true;
            }
            if (added)
            {
                // 全部装完再同步一次缓存，避免逐颗写入
                SyncBulletCountCache(setting);
            }
            return added;
        }

        /// <summary>确定“击杀时使用的那颗子弹”的弹种。</summary>
        private static int ResolveBulletTypeID(ItemSetting_Gun setting)
        {
            // 弹匣里还有同类弹：直接用那一堆的弹种（最精确）
            Item loaded = setting.GetCurrentLoadedBullet();
            if (loaded != null)
            {
                return loaded.TypeID;
            }

            // 弹匣正好被击杀那一枪打空：用枪械记录的当前弹种
            // （targetBulletID 不参与序列化，射击/换弹后一直保留到下次设置）
            int typeID = setting.TargetBulletID;
            if (typeID > 0)
            {
                return typeID;
            }

            // 读档后空弹匣（targetBulletID 初始为 -1）：按口径从主角背包里认一次，
            // 沿用游戏自身 ItemSetting_Gun.Start 的做法
            CharacterMainControl main = CharacterMainControl.Main;
            Item characterItem = (main != null) ? main.CharacterItem : null;
            if (characterItem != null && characterItem.Inventory != null &&
                setting.AutoSetTypeInInventory(characterItem.Inventory))
            {
                typeID = setting.TargetBulletID;
                if (typeID > 0)
                {
                    return typeID;
                }
            }
            return -1;
        }

        /// <summary>
        /// 把一颗子弹放进弹匣。只管物品本身，不刷新弹药计数缓存
        /// （由 TryRefund 在整批装完后统一同步一次）。
        /// </summary>
        private static bool AddOneBullet(Inventory magazine, ItemSetting_Gun setting, int typeID)
        {
            // 情况 A（绝大多数）：弹匣里已有同类弹堆 —— 直接 +1，不产生任何新对象、不触发 GC。
            // 用 List<Item> 索引遍历，避免 foreach 走 IEnumerator&lt;Item&gt; 接口带来的枚举器分配。
            List<Item> content = magazine.Content;
            if (content != null)
            {
                for (int i = 0; i < content.Count; i++)
                {
                    Item stack = content[i];
                    if (stack == null || stack.TypeID != typeID)
                    {
                        continue;
                    }
                    if (!stack.Stackable || stack.StackCount >= stack.MaxStackCount)
                    {
                        continue;
                    }
                    // StackCount setter 内部会 clamp 到 MaxStackCount、写 Item.Variables["Count"]、
                    // 通知子物体与所在 Inventory，所以这里是唯一需要的写操作
                    stack.StackCount++;
                    return true;
                }
            }

            // 情况 B：弹匣被打空（或同类弹堆已满）—— 现场生成 1 颗。
            // 与游戏自身 ItemSetting_Gun.LoadBulletsFromInventory 的做法完全一致：
            // InstantiateSync -> StackCount -> Inspected = true -> Inventory.AddAndMerge
            if (ItemAssetsCollection.GetPrefab(typeID) == null)
            {
                // 没有配置该弹种的 prefab。不生成 FallbackItem（裸 Item 无法正确堆叠/显示），
                // 直接放弃这一颗，只提醒一次。
                if (!_prefabMissingWarned)
                {
                    _prefabMissingWarned = true;
                    Debug.LogWarning("[HeadshotBulletRefund] 找不到弹种 ID " + typeID +
                                     " 的 Item prefab，本次不返还子弹。");
                }
                return false;
            }

            Item bullet = ItemAssetsCollection.InstantiateSync(typeID);
            if (bullet == null)
            {
                return false;
            }
            bullet.StackCount = 1;
            bullet.Inspected = true;
            if (magazine.AddAndMerge(bullet))
            {
                return true;
            }
            // 放不进去（无空位）就不要留下孤儿对象
            bullet.DestroyTree();
            return false;
        }

        /// <summary>
        /// 把弹匣真实余弹同步进 ItemSetting_Gun 的私有缓存。
        /// ItemSetting_Gun.BulletCount 只在缓存 &lt; 0 时才用 GetBulletCount() 重算，
        /// 直接改弹匣物品不会刷新它（弹匣 HUD 也会因此显示旧值），所以这里显式写入。
        /// 只在“真的返还了一颗”时调用；若游戏更新改了私有成员名，则退化为让游戏自行重算。
        /// </summary>
        private static void SyncBulletCountCache(ItemSetting_Gun setting)
        {
            if (_cacheSyncUnavailable)
            {
                return;
            }
            try
            {
                if (_bulletCountProperty == null && _bulletCountField == null)
                {
                    Type type = typeof(ItemSetting_Gun);
                    PropertyInfo property = type.GetProperty("bulletCount",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (property != null && property.CanWrite)
                    {
                        _bulletCountProperty = property;
                    }
                    else
                    {
                        _bulletCountField = type.GetField("_bulletCountCache",
                            BindingFlags.Instance | BindingFlags.NonPublic);
                    }
                }

                if (_bulletCountProperty != null)
                {
                    // private setter 会同时写缓存与 Item.Variables["BulletCount"]
                    _bulletCountProperty.SetValue(setting, setting.GetBulletCount(), null);
                    return;
                }
                if (_bulletCountField != null)
                {
                    // 兜底：置 -1，让游戏在下一次读取 BulletCount 时自行重算
                    _bulletCountField.SetValue(setting, -1);
                    return;
                }

                _cacheSyncUnavailable = true;
                Debug.LogWarning("[HeadshotBulletRefund] 未找到 ItemSetting_Gun 的弹药计数缓存成员，" +
                                 "改由游戏在下一次读取时重算（不影响返还本身）。");
            }
            catch (Exception e)
            {
                _cacheSyncUnavailable = true;
                Debug.LogWarning("[HeadshotBulletRefund] 同步弹药计数缓存失败：" + e.Message);
            }
        }

        // ======================= 换弹中挂起的返还 =======================

        private static void EnqueuePending(ItemSetting_Gun setting)
        {
            _pendingSetting = setting;
            _pendingCount++;
            _pendingSince = Time.unscaledTime;
        }

        private static void ClearPending()
        {
            _pendingSetting = null;
            _pendingCount = 0;
        }

        private static void TryFlushPending(float now)
        {
            ItemSetting_Gun setting = _pendingSetting;
            if (setting == null)
            {
                ClearPending();
                return;
            }
            if (setting.LoadingBullets)
            {
                if (now - _pendingSince > PendingTimeout)
                {
                    ClearPending();
                }
                return;
            }

            Item gunItem = setting.Item;
            Inventory magazine = (gunItem != null) ? gunItem.Inventory : null;
            if (magazine != null && _pendingCount > 0)
            {
                // 换弹已结束：逐颗补回（弹匣满或弹种未知时自然放弃）
                while (_pendingCount > 0)
                {
                    if (!TryRefund(magazine, setting))
                    {
                        break;
                    }
                    _pendingCount--;
                }
            }
            ClearPending();
        }
    }
}
