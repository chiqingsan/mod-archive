using System;
using System.Collections.Generic;
using System.Reflection;
using ItemStatsSystem;
using UnityEngine;

namespace LootRarityMarker
{
    /// <summary>
    /// 稀有度判定：**逐字复刻**工坊 mod「物品价值稀有度与搜索音效」
    /// （`ItemLevelAndSearchSoundMod` / workshop 3588386576）的
    /// `Util.GetItemValueLevel(Item)`。
    ///
    /// 为什么直接用别人的标准而不是自己定：
    ///   玩家眼里的「这件东西是什么颜色」是由那个 mod 决定的，它给物品名上色。
    ///   我们如果另起一套，交互点的颜色就会和名字的颜色对不上 —— 这正是之前反复被报的 bug。
    ///
    /// 复刻要点（容易漏的地方都标出来了）：
    ///   1. 它先把 value 除以 2（`num = item.Value / 2f`）再查阈值表。
    ///      所以阈值 200/600/1200/2500/5000/10000 对应的真实 value 是
    ///      400/1200/2400/5000/10000/20000。
    ///   2. 子弹（Tag 含 Bullet）不是用 num，而是 `num * 30`，并且最多只算到橙。
    ///   3. 装备（Equipment）、饰品（Accessory）、特殊装备（Equipment+Special）
    ///      主要看 `item.Quality`，不看 value。
    ///   4. 动态物品（在 ItemAssetsCollection.dynamicDic 里的 TypeID）只认
    ///      DisplayQuality / Quality，不走价值表。
    ///   5. 兜底分支是 max(按价值, 按 DisplayQuality)。
    ///
    /// 返回值就是 `ItemValueLevel` 的整数值：0=白 1=绿 2=蓝 3=紫 4=橙 5=浅红 6=深红。
    /// </summary>
    internal static class RarityRules
    {
        public const int White = 0;
        public const int Green = 1;
        public const int Blue = 2;
        public const int Purple = 3;
        public const int Orange = 4;
        public const int LightRed = 5;
        public const int Red = 6;

        /// <summary>档位总数（白 … 深红）。</summary>
        public const int LevelCount = 7;

        // ── Tag 缓存 ───────────────────────────────────────────────────────
        // `TagCollection.Contains(string)` 内部是 `list.Any(t => t.name == tagName)`：
        // 每次调用都会分配一个闭包 + 委托，而且每个元素都要取一次 `t.name`
        // （UnityEngine.Object.name 是原生调用，每次返回新的 string → 分配）。
        // 扫描时要对每个物品问 3~4 个 Tag，758 个容器 × 4 件就是上万次分配。
        //
        // 而 Tag 是物品**资产**的属性，同一个 TypeID 的 Tag 永远一样，
        // 所以按 TypeID 缓存成位标志，每个 TypeID 一辈子只查一次。
        private const byte TagBullet = 1;
        private const byte TagEquipment = 2;
        private const byte TagAccessory = 4;
        private const byte TagSpecial = 8;
        private const byte TagStormProtection = 16;

        private static readonly Dictionary<int, byte> _tagFlags = new Dictionary<int, byte>(512);

        /// <summary>
        /// 取这个 TypeID 的 Tag 位标志；第一次遇到时算好并缓存。
        /// 注意：Tag 属于物品资产，与具体实例无关，所以按 TypeID 缓存是安全的。
        /// </summary>
        private static byte FlagsOf(Item item)
        {
            int typeId = item.TypeID;
            byte flags;
            if (_tagFlags.TryGetValue(typeId, out flags))
            {
                return flags;
            }

            flags = 0;
            var tags = item.Tags;
            if (tags != null)
            {
                if (tags.Contains("Bullet"))
                {
                    flags |= TagBullet;
                }
                if (tags.Contains("Equipment"))
                {
                    flags |= TagEquipment;
                }
                if (tags.Contains("Accessory"))
                {
                    flags |= TagAccessory;
                }
                if (tags.Contains("Special"))
                {
                    flags |= TagSpecial;
                }
            }
            if ((flags & TagSpecial) != 0)
            {
                // 只在「特殊装备」这一小撮物品上才去读 name（原生调用 + 字符串分配）
                string itemName = item.name;
                if (itemName != null && itemName.Contains("StormProtection"))
                {
                    flags |= TagStormProtection;
                }
            }
            _tagFlags[typeId] = flags;
            return flags;
        }

        /// <summary>
        /// 物品定义表里的「动态物品」集合：`ItemAssetsCollection.dynamicDic`。
        /// 它是 private static，只能反射拿。拿不到就退化成「没有动态物品」，
        /// 后果只是那一小撮物品会走兜底分支，不会崩。
        /// </summary>
        private static Dictionary<int, ItemAssetsCollection.DynamicEntry> _dynamicMap;

        private static bool _initDone;

        /// <summary>反射是否成功（只用于启动日志）。</summary>
        public static bool DynamicMapReady { get; private set; }

        public static void Init()
        {
            if (_initDone)
            {
                return;
            }
            _initDone = true;
            try
            {
                FieldInfo field = typeof(ItemAssetsCollection).GetField(
                    "dynamicDic", BindingFlags.Static | BindingFlags.NonPublic);
                if (field != null)
                {
                    _dynamicMap = field.GetValue(null) as Dictionary<int, ItemAssetsCollection.DynamicEntry>;
                    DynamicMapReady = _dynamicMap != null;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[LootRarityMarker] 读取动态物品表失败，部分物品改用兜底规则：" + e.Message);
            }
        }

        /// <summary>
        /// 一件物品的档位（0…6）。null 一律白。
        /// 与那个 mod 的 `GetItemValueLevel` 分支顺序完全一致。
        /// </summary>
        public static int LevelOf(Item item)
        {
            if (item == null)
            {
                return White;
            }

            float num = item.Value / 2f;

            // ── 1. 动态物品 ──────────────────────────────────────────────
            if (_dynamicMap != null && _dynamicMap.ContainsKey(item.TypeID))
            {
                if (item.DisplayQuality != DisplayQuality.None)
                {
                    return ParseDisplayQuality(item);
                }
                return ParseQuality(item.Quality);
            }

            byte flags = FlagsOf(item);

            // ── 2. 子弹 ──────────────────────────────────────────────────
            if ((flags & TagBullet) != 0)
            {
                if (item.DisplayQuality != DisplayQuality.None)
                {
                    // 子弹的橙品质被特意抬成「浅红」
                    if (item.DisplayQuality == DisplayQuality.Orange)
                    {
                        return LightRed;
                    }
                    return ParseDisplayQuality(item);
                }
                if (item.Quality == 1)
                {
                    return White;
                }
                if (item.Quality == 2)
                {
                    return Green;
                }
                // 子弹按「num × 30」算价值，且封顶在橙
                int byBulletValue = CalculateItemValueLevel((int)(num * 30f));
                return (byBulletValue > Orange) ? Orange : byBulletValue;
            }

            // ── 3. 装备 ──────────────────────────────────────────────────
            if ((flags & TagEquipment) != 0)
            {
                if ((flags & TagSpecial) != 0)
                {
                    if ((flags & TagStormProtection) != 0)
                    {
                        return ClampLevel(item.Quality - 1);
                    }
                    // 特殊装备：Quality - 2 起算
                    int shifted = item.Quality - 2;
                    if (shifted > Red)
                    {
                        return Red;
                    }
                    if (shifted < 0)
                    {
                        return White;
                    }
                    return shifted;
                }
                if (item.Quality <= 7)
                {
                    return ClampLevel(item.Quality - 1);
                }
                return CalculateItemValueLevel((int)num);
            }

            // ── 4. 饰品 ──────────────────────────────────────────────────
            if ((flags & TagAccessory) != 0)
            {
                if (item.Quality <= 7)
                {
                    return ClampLevel(item.Quality - 1);
                }
                return ParseDisplayQuality(item);
            }

            // ── 5. 两个写死的特例 ────────────────────────────────────────
            if (item.TypeID == 862 || item.TypeID == 1238)
            {
                return Orange;
            }

            // ── 6. 兜底：按价值与按品质取高 ──────────────────────────────
            int byValue = CalculateItemValueLevel((int)num);
            int byDisplayQuality = ParseDisplayQuality(item);
            return (byDisplayQuality > byValue) ? byDisplayQuality : byValue;
        }

        /// <summary>
        /// 价值档位表。注意传进来的已经是「除以 2 之后」的值。
        /// `>=` 边界与原实现一致。
        /// </summary>
        public static int CalculateItemValueLevel(int value)
        {
            if (value >= 10000)
            {
                return Red;
            }
            if (value >= 5000)
            {
                return LightRed;
            }
            if (value >= 2500)
            {
                return Orange;
            }
            if (value >= 1200)
            {
                return Purple;
            }
            if (value >= 600)
            {
                return Blue;
            }
            if (value >= 200)
            {
                return Green;
            }
            return White;
        }

        /// <summary>把 `Item.DisplayQuality` 映射成档位（红分成浅红/深红）。</summary>
        private static int ParseDisplayQuality(Item item)
        {
            switch (item.DisplayQuality)
            {
                case DisplayQuality.None:
                case DisplayQuality.White:
                    return White;
                case DisplayQuality.Green:
                    return Green;
                case DisplayQuality.Blue:
                    return Blue;
                case DisplayQuality.Purple:
                    return Purple;
                case DisplayQuality.Orange:
                    return Orange;
                case DisplayQuality.Red:
                    // 品质正好是 6（红）的算「浅红」，更高的才算「深红」
                    return (item.Quality == 6) ? LightRed : Red;
                case DisplayQuality.Q7:
                case DisplayQuality.Q8:
                    return Red;
                default:
                    return White;
            }
        }

        /// <summary>`ParseQuality`：Quality 1 → 白，2 → 绿 …，夹在 0…6。</summary>
        private static int ParseQuality(int quality)
        {
            return ClampLevel(quality - 1);
        }

        private static int ClampLevel(int level)
        {
            if (level < 0)
            {
                return 0;
            }
            if (level > Red)
            {
                return Red;
            }
            return level;
        }

        /// <summary>档位在 config.json 里的英文名，只给日志/README 用。</summary>
        public static string LevelName(int level)
        {
            switch (level)
            {
                case White: return "White";
                case Green: return "Green";
                case Blue: return "Blue";
                case Purple: return "Purple";
                case Orange: return "Orange";
                case LightRed: return "LightRed";
                case Red: return "Red";
                default: return "?";
            }
        }
    }
}
