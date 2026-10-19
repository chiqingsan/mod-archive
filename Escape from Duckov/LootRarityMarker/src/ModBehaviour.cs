using System;
using System.Collections.Generic;
using System.Text;
using ItemStatsSystem;
using UnityEngine;

namespace LootRarityMarker
{
    /// <summary>
    /// 战利品品质标记：按周围容器里**最高品质物品**的颜色，给容器的交互点染色
    /// （白 / 绿 / 蓝 / 紫 / 橙 / 红，颜色取自游戏自己的品质颜色表）。
    ///
    /// ============================ 性能设计（这是本 mod 的第一原则）============================
    /// 1) 每个容器有「脏标记」，扫过一遍就不再重算。
    ///    UpdateEntry 里 `if (!entry.Dirty) return;` 在遍历物品之前就返回 ——
    ///    一个已经算过的容器，之后每次被球查询命中，代价只有一次字典查找 + 一次布尔判断。
    ///    脏标记由这些信号置位（都是「内容真的可能变了」的时刻）：
    ///      * Inventory.onContentChanged（游戏自己每次增删物品都会发）
    ///      * 容器刚被创建、刚换关卡、设置被改动
    ///      * Inventory.Loading 还是 true（内容是异步装填的）
    ///      * InteractableLootbox.OnStartLoot / OnStopLoot
    ///    另外每 FullRescanIntervalSeconds 强制全量重算一次，作为「万一漏了某个事件」的兜底 ——
    ///    也就是说最坏情况下颜色最多旧 10 秒，而不是永远错下去（自愈）。
    ///
    ///    为什么这比「无状态重扫」好：一个关卡最多可以有 758 个战利品容器（实测，见 README §4.4），
    ///    无状态重扫会在每次扫描时把它们全部重新遍历一遍物品；脏标记只在你真的动过东西时才重算。
    ///
    /// 2) 站着不动 = 完全不扫描（scanOnlyWhenMoving，默认开）。
    ///    这是最省的一条：玩家站在原地看背包时，本 mod 每帧的全部开销就是
    ///    `if (!_scanRequested && Time.unscaledTime < _nextScanTime) return;` —— 一次 float 比较。
    ///    移动、开关战利品界面、进出关卡都会把 _scanRequested 置位，强制补扫一次，所以「不扫描」
    ///    不会漏掉任何真实的画面变化。
    ///    （代价：这个模式下周期性的兜底全量重算也一并被压住了 —— 站着不动时真的什么都不做。
    ///      这是刻意的取舍，因为站着不动时容器内容只可能被你自己的开箱动作改变，而那是会强制补扫的。）
    ///
    /// 3) 扫描本身比游戏自己便宜得多。
    ///    游戏每帧都在做 Physics.OverlapSphereNonAlloc(pos, 0.3f, ...) 找可交互物（CA_Interact）。
    ///    本 mod 用同样的 API，但半径 25 米、每 500 毫秒最多一次、站着不动时一次都不做 ——
    ///    稳态下不到游戏自身开销的百分之一。
    ///
    /// 4) 零 GC。
    ///    Collider[] 缓冲区、MaterialPropertyBlock、字典、列表全部复用；
    ///    遍历物品用索引 for 而不是 foreach（Inventory 的 foreach 会 new 一个迭代器）；
    ///    不 new Material、不 new Texture、不 Shader.Find。
    ///
    /// 5) 不写共享材质。
    ///    染色只用 per-renderer 的 MaterialPropertyBlock（见 MarkerTint），不碰材质资源本身，
    ///    不破坏 SRP Batcher 的材质分组，也不会影响别的容器 / 别的 mod / 游戏本体。
    ///
    /// ============================ 兼容性设计 ============================
    /// * 零 Harmony 补丁、零反射改私有字段（唯一的例外是最多碰一次那个本来就该被碰的公开 getter，
    ///   见 UpdateEntry 里的说明）。
    /// * 路线 A（真描边）全走反射调用游戏自带的 EPOOutline，编译期不引用它的 dll：
    ///   游戏更新把 EPO 删了也只是描边不生效，染色功能不受影响。
    /// * 设置面板对接靠 ModConfigBridge 反射，没装 ModConfig 也能用 config.json 跑。
    /// </summary>
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        /// <summary>物理扫描结果缓冲区。满了只影响一次扫描能覆盖的容器数，不会崩。</summary>
        private const int HitBufferSize = 256;

        /// <summary>关卡刚加载完的一段时间内，即使站着不动也照常扫描（容器的内容是异步装填的）。</summary>
        private const float LevelSettleSeconds = 6f;

        private const int MaskNotResolved = -2;

        private const int ModConfigMaxAttempts = 15;
        private const float ModConfigRetryInterval = 2f;

        /// <summary>兜底：每隔这么久强制全量重算一次，防止漏掉某个事件导致颜色永远停在旧值。</summary>
        private const float FullRescanIntervalSeconds = 10f;

        /// <summary>
        /// 路线 A 专用：单个渲染器的世界包围盒最长边超过这个尺寸（米）就不描边。
        /// 存在的理由很实际：AddAllChildRenderersToRenderingList 会把容器**整个子树**的
        /// MeshRenderer 都收进去，而关卡里的容器常常是整组场景道具（衣柜、货架、带圆形贴花的机器），
        /// 于是一个箱子能描出半屏大的红圈。4 米足够容纳任何正常箱子，又能挡住这类巨型物体。
        /// </summary>
        private const float MaxOutlineSizeMeters = 4f;

        /// <summary>路线 A 专用：一个容器最多描几个渲染器（按加入顺序取前 N 个，多出来的剔除）。</summary>
        private const int MaxOutlineTargets = 6;

        /// <summary>一个被追踪的容器。</summary>
        private sealed class Entry
        {
            public InteractableLootbox Box;
            public Inventory Inventory;
            public MarkerTint Tint;
            public Component Outlinable;

            /// <summary>上次应用的判定结果；三者全等就跳过，一个字节都不写。</summary>
            public int AppliedRank = int.MinValue;
            public int AppliedLevel = -1;
            /// <summary>上次着色时容器内所有物品的价值之和。发光强度靠它，所以它变了也要重刷。</summary>
            public int AppliedBestValue = -1;
            public bool AppliedSuppressed;

            /// <summary>这个容器没有交互点，已经提醒过了（避免每次扫描都刷日志）。</summary>
            public bool NoMarkerLogged;

            // ---- 脏标记：内容没变就不重算 ----

            /// <summary>
            /// true = 这个容器的判定需要重算。已经算过、内容又没变的容器会在
            /// UpdateEntry 里遍历物品**之前**就返回，这是本 mod 在容器很多时的主要省法。
            /// </summary>
            public bool Dirty = true;

            /// <summary>当前挂在哪个 Inventory 的 onContentChanged 上（换对象时要先摘旧的）。</summary>
            public Inventory BoundInventory;

            /// <summary>挂在事件上的委托实例；摘除时必须用同一个实例，所以不能临时 new。</summary>
            public Action<Inventory, int> ContentHandler;

            // ---- 任务队列 ----

            /// <summary>已经排进待办队列了，避免同一个容器被重复入队。</summary>
            public bool Queued;

            /// <summary>已被清理（容器销毁）。队列里遇到它直接跳过，不再复活。</summary>
            public bool Dead;
        }

        // ===== 设置 =====
        private static ModSettings _settings = new ModSettings();
        private static string[] _colorProperties = MarkerTint.DefaultColorProperties;

        /// <summary>ModConfig 的 UI 项每个进程只注册一次，避免 mod 被重新启用时出现重复条目。</summary>
        private static bool _modConfigEntriesAdded;
        private static bool _modConfigHandlerAdded;

        /// <summary>设置版本号。ModConfig 里改了东西就 +1，Update 发现版本变了才重建染色配方。</summary>
        private static int _settingsVersion = 1;

        private bool _modConfigReady;
        private bool _modConfigGaveUp;
        private int _modConfigAttempts;
        private float _nextModConfigAttempt;

        // ===== 运行时状态 =====
        private readonly Dictionary<int, Entry> _entries = new Dictionary<int, Entry>(128);
        private readonly List<int> _deadKeys = new List<int>(32);
        private readonly Collider[] _hits = new Collider[HitBufferSize];

        /// <summary>
        /// 待办队列（FIFO）。扫描只负责「把需要重算的容器排进来」，真正的重活由
        /// DrainWorkQueue 每帧按预算放行几个 —— 这样换关卡 / 改设置时的几百次重建
        /// 会被摊到很多帧里，不会集中在一帧造成掉帧。
        ///
        /// 用「List + 游标」而不是 Queue&lt;T&gt;：出队不产生任何分配，也不用复制数组。
        /// </summary>
        private readonly List<Entry> _workQueue = new List<Entry>(128);

        /// <summary>
        /// 待办队列里「还没处理」的条目上限。纯粹是保险丝：正常情况下队列长度
        /// 不会超过当前关卡里的容器数（去重保证同一个容器最多排一个），这里再放大 4 倍余量。
        ///
        /// 为什么必须有：队列是靠「处理速度（maxWorkPerFrame/帧）」消化入队洪水的。
        /// 如果入队速度一时超过处理速度（换关卡瞬间几百个容器同时变脏、尸体容器刷一堆），
        /// 没有上限就意味着内存无界增长。有了上限，最坏结果是本轮少刷新几个容器，
        /// 下一轮扫描会把它们重新排进来 —— 功能无损，只是晚一点。
        /// </summary>
        private const int MaxQueueLength = 4096;

        /// <summary>_workQueue 里下一次该处理的下标；追上 Count 时整表清空。</summary>
        private int _queueHead;

        /// <summary>队列打满的警告只报一次，避免每帧刷日志。</summary>
        private bool _queueFullLogged;

        /// <summary>限流日志的上次输出时间（秒，unscaled）。</summary>
        private float _lastThrottledLogTime = -999f;

        /// <summary>正在 DrainWorkQueue 里跑。用来禁止在出队过程中整理队列（见 Enqueue）。</summary>
        private bool _draining;

        private int _interactableMask = MaskNotResolved;
        private int _appliedSettingsVersion;
        private float _nextScanTime;
        private float _nextFullRescan;
        private float _levelSettleUntil;
        private Vector3 _lastScanPos;
        private bool _scanRequested = true;
        private bool _subscribed;
        private bool _active;
        private bool _startupLogged;
        private bool _tintWarningLogged;
        private bool _saturationWarned;
        private bool _dumpedFirstMarker;

        /// <summary>
        /// 单帧耗时超过这个毫秒数就写一条日志（同类每分钟最多一条）。
        /// 这是「自证清白」用的：用户的核心诉求是「别瞬间抢占性能导致掉帧」，
        /// 那么卡顿到底是不是本 mod 造成的，就应该能从日志里直接读出来，而不是靠猜。
        /// </summary>
        private const float SpikeWarnMs = 6f;

        /// <summary>
        /// 工作队列的单帧时间预算。maxWorkPerFrame 仍保留为数量上限，
        /// 但不能只按容器数量限流：一个大背包的成本可能远高于一个普通箱子。
        /// </summary>
        private const double WorkBudgetMilliseconds = 2.0;

        /// <summary>
        /// 同时挂着的描边组件上限。
        /// 为什么必须有：`Outlinable` 是往容器本体上 AddComponent，EPO 每帧要为每个
        /// Outlinable 生成一遍描边（额外的 draw call + 模板缓冲读写）。一个关卡里有 758 个
        /// 容器，如果范围内一次刷出几百个描边，那就是纯粹的「瞬间抢占性能」——正是要避免的。
        /// 16：EPO 的每个描边目标都要额外一次 draw call + 模板缓冲读写，
        /// 实测一个容器本体最多留 6 个目标，16 个容器就是最多 96 个目标。
        /// 压到 16（最多 96 个目标）之后，「眼前这一堆」照样全有描边，
        /// 但不会因为一次换关卡就吃掉整帧预算。
        /// </summary>
        private const int MaxOutlinedContainers = 16;

        /// <summary>当前挂着描边的容器数，每次扫描时重算一遍（O(容器数)，每 500 毫秒一次）。</summary>
        private int _outlinedCount;

        /// <summary>路线 A 因为超过上限而被跳过的提示只报一次。</summary>
        private bool _outlineCapLogged;

        /// <summary>本 mod 单帧干活耗时（毫秒）的历史最大值，写进启动日志方便事后对照。</summary>
        private float _worstFrameMs;


        // ======================= 生命周期 =======================

        protected override void OnAfterSetup()
        {
            _settings = ModSettings.LoadFromModFolder();
            _colorProperties = ParseColorProperties(_settings.colorProperties);
            _settingsVersion++;
            _appliedSettingsVersion = _settingsVersion;

            RarityRules.Init(); // 反射拿 ItemAssetsCollection.dynamicDic，失败只会退化成兜底规则

            _modConfigGaveUp = false;
            TryRegisterModConfig(); // ModConfig 可能还没加载完（按文件夹名顺序激活），失败时在 Update 里重试

            Subscribe();
            _scanRequested = true;
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

            Unsubscribe();
            ReleaseAll();
            _entries.Clear();
            _active = false;
        }

        private void OnDestroy()
        {
            // 幂等：即使没经过 NotifyBeforeDeactivate，也不留下悬挂的静态委托和染过色的交互点
            OnBeforeDeactivate();
        }

        private void Update()
        {
            // ---- ModConfig 重试（只在启动阶段，最多 15 次 × 2 秒）----
            if (!_modConfigReady && !_modConfigGaveUp)
            {
                float retryNow = Time.unscaledTime;
                if (retryNow >= _nextModConfigAttempt)
                {
                    _nextModConfigAttempt = retryNow + ModConfigRetryInterval;
                    _modConfigAttempts++;
                    if (!TryRegisterModConfig() && _modConfigAttempts >= ModConfigMaxAttempts)
                    {
                        _modConfigGaveUp = true;
                        Debug.Log("[LootRarityMarker] 未检测到可用的 ModConfig，停用设置菜单对接；" +
                                  "数值改由 mod 目录下 config.json / 内置默认值决定。");
                    }
                }
            }

            // ---- 总开关：关掉时把已经染上的颜色全部还原成游戏原样 ----
            if (!_settings.enabled)
            {
                if (_active)
                {
                    DeactivateTints();
                    _active = false;
                }
                return;
            }
            _active = true;

            // ---- 设置变过（ModConfig 面板里改的）：重建染色配方 ----
            if (_appliedSettingsVersion != _settingsVersion)
            {
                _appliedSettingsVersion = _settingsVersion;
                try
                {
                    RebuildTints();
                }
                catch (Exception e)
                {
                    LogThrottled("应用新设置时出错：" + e.Message);
                }
            }

            if (!LevelManager.LevelInited)
            {
                return;
            }

            // ---- 从这里开始测量本 mod 自己这一帧干了多少活（看门狗）----
            double tickStart = Time.realtimeSinceStartupAsDouble;

            // ---- 每帧按预算消化待办队列（队列空时只有一次比较）----
            // 必须放在稳态快路径之前，否则「站着不动」时队列永远没机会被消化。
            try
            {
                DrainWorkQueue();
            }
            catch (Exception e)
            {
                LogThrottled("消化待办队列时出错（已清空队列自保）：" + e.Message);
                ClearWorkQueue();
            }

            // ===== 稳态快路径：这一行就是「玩家站着不动」时的全部每帧开销 =====
            float now = Time.unscaledTime;
            if (!_scanRequested && now < _nextScanTime)
            {
                Watchdog(tickStart);
                return;
            }
            // 无论这次扫不扫，都把下一次到点时间往后推：站着不动时最多每 scanInterval 做一次距离判断
            _nextScanTime = now + _settings.ScanIntervalSeconds;

            CharacterMainControl main = CharacterMainControl.Main;
            if (main == null)
            {
                Watchdog(tickStart);
                return;
            }
            Vector3 playerPos = main.transform.position;

            // 站着没动 → 跳过。距离是相对「上次真正扫描的位置」累计的，
            // 所以慢慢挪动也终究会触发一次扫描。
            if (!_scanRequested && _settings.scanOnlyWhenMoving && now > _levelSettleUntil &&
                (playerPos - _lastScanPos).sqrMagnitude < _settings.ScanMoveDistanceSquared)
            {
                Watchdog(tickStart);
                return;
            }

            _scanRequested = false;
            _lastScanPos = playerPos;

            // 球查询 + 遍历结果本身不该抛，但它会碰 collider.transform 和 GetComponentInParent，
            // 而这些东西可能在场景切换半途被销毁。兜一层，别让一次异常卡死整个 mod 的后续帧。
            try
            {
                Scan(playerPos);
            }
            catch (Exception e)
            {
                LogThrottled("扫描时出错（已跳过本次扫描）：" + e.Message);
            }

            Watchdog(tickStart);
        }

        /// <summary>
        /// 看门狗：记下本帧耗时，超阈值就报一条（限流）。
        /// 目的不是自我节流（节流已经由 maxWorkPerFrame 保证了），
        /// 而是让「卡顿是不是本 mod 造成的」这个问题在日志里有确定的答案。
        /// </summary>
        private void Watchdog(double tickStart)
        {
            float spentMs = (float)((Time.realtimeSinceStartupAsDouble - tickStart) * 1000.0);
            if (spentMs > _worstFrameMs)
            {
                _worstFrameMs = spentMs;
            }
            if (spentMs >= SpikeWarnMs)
            {
                LogThrottled("本帧耗时 " + spentMs.ToString("F1") + " 毫秒（待办队列剩余 " +
                             (_workQueue.Count - _queueHead) + " 个，已跟踪容器 " + _entries.Count +
                             " 个）。若这行反复出现，把 maxWorkPerFrame 调小或暂时关闭 outline。");
            }
        }

        // ======================= 扫描 =======================

        /// <summary>
        /// 扫一次玩家周围的容器，逐个重算判定。这是本 mod 唯一的重活，调用频率由 Update 控制。
        /// </summary>
        private void Scan(Vector3 playerPos)
        {
            int mask = ResolveInteractableMask();
            if (mask == 0)
            {
                return;
            }

            // 重算当前挂着描边的容器数，供路线 A 的上限闸门使用。
            // 每次扫描重算而不是增量维护：销毁路径太多（容器被销毁、关卡切换、设置重建、
            // 玩家手动关掉总开关），增量维护迟早记错；这里 O(容器数) 且每 500 毫秒才一次，很便宜。
            RecountOutlines();

            if (!_startupLogged)
            {
                _startupLogged = true;
                Debug.Log("[LootRarityMarker] 开始工作：半径 " + _settings.scanRadius + " 米，间隔 " +
                          _settings.scanIntervalMs + " 毫秒" +
                          (_settings.scanOnlyWhenMoving ? "（站着不动时不扫描）" : "") +
                          "，排序依据=" + (_settings.rankByValue ? "价值" : "品质") +
                          "，发光=" + (GlowSummary()) +
                          "，判定=" + (RarityRules.DynamicMapReady
                              ? "物品价值稀有度 mod 同款规则（7 档，动态物品表已取到）"
                              : "物品价值稀有度 mod 同款规则（7 档，动态物品表未取到，走兜底）") +
                          "，容器本体描边=" + (_settings.outline ? "开" : "关") +
                          "，每帧最多处理=" + _settings.maxWorkPerFrame + " 个容器，工作预算=" +
                          WorkBudgetMilliseconds.ToString("F1") + " 毫秒" +
                          "。");            }

            // 和游戏 CA_Interact.SearchInteractableAround 完全一样的查法：
            // 同样的 "Interactable" 层、同样的 QueryTriggerInteraction.Collide，
            // 只是半径大得多、频率低得多。NonAlloc 版本不产生 GC。
            int count = Physics.OverlapSphereNonAlloc(
                playerPos + Vector3.up * 0.5f,
                _settings.ScanRadiusMeters,
                _hits,
                mask,
                QueryTriggerInteraction.Collide);

            if (count >= _hits.Length && !_saturationWarned)
            {
                _saturationWarned = true;
                Debug.LogWarning("[LootRarityMarker] 一次扫描内的可交互物达到缓冲区上限 " + _hits.Length +
                                 "，可能有容器没被扫到。把 scanRadius 调小一点即可。");
            }

            // 兜底：每 10 秒无视脏标记，把所有容器的判定重算一遍。
            // 这样即使某个内容变化事件被漏掉，颜色最多旧 10 秒，不会永远错下去。
            // 注意它被外层挡着：scanOnlyWhenMoving 打开时，站着不动连兜底扫描也不会发生。
            float now = Time.unscaledTime;
            bool fullRescan = now >= _nextFullRescan;
            if (fullRescan)
            {
                _nextFullRescan = now + FullRescanIntervalSeconds;
            }

            // 扫描本身只做「发现 + 入队」，不做任何重活 —— 重活交给每帧按预算放行的队列去消化。
            // 所以扫描那一帧的耗时几乎与容器数量无关，场上容器再多也不会在扫描帧掉帧。
            for (int i = 0; i < count; i++)
            {
                Collider collider = _hits[i];
                if (collider == null)
                {
                    continue;
                }
                // 容器上可能不止一个碰撞体，同一个容器会被多个碰撞体命中；
                // 下面的字典查找 + Queued 标记天然去重，所以这里不用额外判重。
                InteractableLootbox box = collider.GetComponentInParent<InteractableLootbox>();
                if (box == null)
                {
                    continue; // 是别的可交互物（拾取物、门、NPC…），不关我们的事
                }

                Entry entry = GetOrCreateEntry(box);
                if (fullRescan)
                {
                    entry.Dirty = true; // 兜底全量重算
                }
                // 只有真的有活要干才入队：已经算过、内容又没变的容器连队列都不进。
                if (entry.Dirty || entry.Tint == null)
                {
                    Enqueue(entry);
                }
            }

            PruneDeadEntries();
        }

        // ======================= 任务队列 =======================

        /// <summary>按 instanceID 取容器账本，没有就新建一个。纯字典操作，不碰任何 Unity 对象。</summary>
        private Entry GetOrCreateEntry(InteractableLootbox box)
        {
            int key = box.GetInstanceID();
            Entry entry;
            if (!_entries.TryGetValue(key, out entry) || !ReferenceEquals(entry.Box, box))
            {
                entry = new Entry { Box = box };
                _entries[key] = entry;
            }
            return entry;
        }

        /// <summary>把一个容器排进待办队列。已经排过 / 已失效的容器直接忽略。</summary>
        private void Enqueue(Entry entry)
        {
            if (entry == null || entry.Queued || entry.Dead)
            {
                return;
            }

            // 保险丝 1：未处理条目不得越过上限。
            if (_workQueue.Count - _queueHead >= MaxQueueLength)
            {
                if (!_queueFullLogged)
                {
                    _queueFullLogged = true;
                    Debug.LogWarning("[LootRarityMarker] 待办队列已达上限 " + MaxQueueLength +
                                     " 个，本轮跳过部分容器的刷新（不影响已上色结果，下一轮扫描会补上）。" +
                                     "出现这行说明入队速度长期超过处理能力，可以把 maxWorkPerFrame 调大，或减小 scanRadius / 增大 scanIntervalMs。");
                }
                return;
            }

            // 保险丝 2：队列头部已经被消化掉的那一段及时丢掉，避免 List 只要不清空就无界增长。
            // 只在「已消化 >= 未消化」时才整理，均摊到每次入队是 O(1) 的。
            //
            // 但**正在出队时不许整理**：Enqueue 可能在 DrainWorkQueue 内部被间接调用
            // （UpdateEntry → box.Inventory → GetOrCreateInventory 的同步段 → AddItem →
            //  onContentChanged → ContentHandler → Enqueue）。那一刻 RemoveRange 会把
            // 列表前移、把 _queueHead 归零，正在跑的那个循环就会从头再走一遍。
            // 加个 _draining 闸门，把这唯一一种重入彻底掐掉，代价是一次 bool 判断。
            if (!_draining && _queueHead > 0 && _queueHead >= _workQueue.Count - _queueHead)
            {
                _workQueue.RemoveRange(0, _queueHead);
                _queueHead = 0;
            }

            entry.Queued = true;
            _workQueue.Add(entry);
        }

        /// <summary>
        /// 每帧消化一点待办队列，最多 maxWorkPerFrame 个，并受固定时间预算限制。
        /// 停下来时剩下的留在队列里，下一帧接着做，避免多个大容器集中占用同一帧。
        ///
        /// 单个容器的工作量并不固定：大型背包可能有数百件物品，描边还会遍历整个子树。
        /// 因此数量上限之外再加一层时间闸门，避免多个大容器恰好集中在同一帧。
        ///
        /// 队列为空时这里只有一次 int 比较，可以放心每帧调用。
        /// </summary>
        private void DrainWorkQueue()
        {
            if (_queueHead >= _workQueue.Count)
            {
                if (_workQueue.Count != 0)
                {
                    _workQueue.Clear();
                    _queueHead = 0;
                }
                _queueFullLogged = false;
                return;
            }

            int maxCount = _settings.maxWorkPerFrame;
            int done = 0;
            int scanned = 0;
            double workStart = Time.realtimeSinceStartupAsDouble;

            // _draining 用 try/finally 保证一定复位：中途抛异常也不会把闸门永久焊死。
            _draining = true;
            try
            {
                // scanned 这道闸门只防一种情况：队列里堆了一大片已销毁的容器，
                // 它们走 continue 不计入 done，若不加限制就会在同一帧里被全部扫一遍。
                while (_queueHead < _workQueue.Count && done < maxCount && scanned < 512)
                {
                    if (done > 0 &&
                        (Time.realtimeSinceStartupAsDouble - workStart) * 1000.0 >= WorkBudgetMilliseconds)
                    {
                        break;
                    }

                    Entry entry = _workQueue[_queueHead];
                    _queueHead++;
                    scanned++;
                    entry.Queued = false;

                    if (entry.Dead || entry.Box == null)
                    {
                        continue; // 容器已经没了，不计入预算
                    }

                    // force = true：入队的都是「有活要干」的，进来就干完（UpdateEntry 自己会清脏标记）
                    //
                    // try/catch 兜底：UpdateEntry 会碰游戏对象（容器 / 交互点 / 材质 / Inventory），
                    // 任何一个在场景切换半途被销毁都可能抛 NRE。如果不在这里兜住，异常会一路上抛到
                    // Unity 的 Update，每帧抛一次 → 控制台刷屏 + 队列永远推不动 → 看上去就是「卡死」。
                    // 兜住之后最坏结果是这一个容器这一帧没刷新，其余照常。
                    try
                    {
                        UpdateEntry(entry.Box, true);
                    }
                    catch (Exception e)
                    {
                        entry.Dirty = false;
                        LogThrottled("处理容器 " + SafeName(entry.Box) + " 时出错（已跳过）：" + e.Message);
                    }
                    done++;
                }
            }
            finally
            {
                _draining = false;
            }

            if (_queueHead >= _workQueue.Count)
            {
                _workQueue.Clear();
                _queueHead = 0;
                _queueFullLogged = false;
            }
        }

        /// <summary>重算当前挂着描边的容器数（路线 A 的上限闸门用）。</summary>
        private void RecountOutlines()
        {
            int count = 0;
            foreach (Entry entry in _entries.Values)
            {
                if (entry.Outlinable != null)
                {
                    count++;
                }
            }
            _outlinedCount = count;
            if (count < MaxOutlinedContainers)
            {
                // 掉到上限以下就重新允许报一次「到上限了」的提示
                _outlineCapLogged = false;
            }
        }

        /// <summary>取名字给日志用；对象已被销毁时返回 &lt;已销毁&gt;，避免 Unity 伪空引用炸掉日志本身。</summary>
        private static string SafeName(InteractableLootbox box)
        {
            try
            {
                if (box == null)
                {
                    return "<已销毁>";
                }
                string name = box.name;
                return string.IsNullOrEmpty(name) ? box.GetInstanceID().ToString() : name;
            }
            catch (Exception)
            {
                return "<已销毁>";
            }
        }

        /// <summary>
        /// 限流日志：同类消息每分钟最多一条。
        /// 理由：这条路径上的日志都在「本来就已经出错」的分支里，一旦每帧刷就会雪上加霜
        /// （Unity 每次 Debug.Log 都要抓堆栈、写文件）。
        /// </summary>
        private void LogThrottled(string message)
        {
            float now = Time.unscaledTime;
            if (now - _lastThrottledLogTime < 60f)
            {
                return;
            }
            _lastThrottledLogTime = now;
            Debug.LogWarning("[LootRarityMarker] " + message);
        }

        /// <summary>清空待办队列（场景切换 / 还原 / 停用时调用，避免留下指向旧关卡的引用）。</summary>
        private void ClearWorkQueue()
        {
            // 只清「还没处理」的那一段：已经处理过的下标从 0 到 _queueHead 的条目，
            // 它们的 Queued 早已在出队时被清零，不需要再碰。
            for (int i = _queueHead; i < _workQueue.Count; i++)
            {
                _workQueue[i].Queued = false;
            }
            _workQueue.Clear();
            _queueHead = 0;
            _queueFullLogged = false;
        }

        private void UpdateEntry(InteractableLootbox box, bool force)
        {
            Entry entry = GetOrCreateEntry(box);

            // ---- 1) 确保交互点与染色配方就位 ----
            if (entry.Tint == null || !entry.Tint.IsAlive)
            {
                InteractMarker marker = box.GetComponentInChildren<InteractMarker>(true);
                if (marker == null)
                {
                    if (_settings.diagnostics && !entry.NoMarkerLogged)
                    {
                        entry.NoMarkerLogged = true;
                        Debug.Log("[LootRarityMarker][诊断] " + box.name +
                                  " 身上没有 InteractMarker 子物体（该容器的 interactMarkerVisible 大概是 false），跳过。");
                    }
                    // 清掉脏标记：否则 Scan 里的 `entry.Dirty || entry.Tint == null` 每次都会
                    // 把这个容器重新塞进队列，等于每轮扫描白跑一遍。真正的重试留给 10 秒兜底全量重扫。
                    entry.Dirty = false;
                    return;
                }

                entry.Tint = new MarkerTint(marker, _colorProperties);
                entry.AppliedRank = int.MinValue;
                entry.AppliedLevel = -1;
                entry.AppliedBestValue = -1;
                entry.AppliedSuppressed = false;
                entry.Dirty = true; // 配方是新的，判定必须重算一次

                if (!entry.Tint.HasAnySlot && !_tintWarningLogged)
                {
                    _tintWarningLogged = true;
                    Debug.LogWarning("[LootRarityMarker] 交互点材质上没找到可染的属性（候选：" +
                                     string.Join(",", _colorProperties) + "），染色不会有任何视觉效果。" +
                                     "请把 config.json 的 diagnostics 改成 true，重启游戏后把 Player.log 里的" +
                                     "[LootRarityMarker][诊断] 那段发出来。");
                }
                if (_settings.diagnostics && !_dumpedFirstMarker)
                {
                    _dumpedFirstMarker = true;
                    Debug.Log("[LootRarityMarker][诊断] 首个交互点的材质清单：\n" +
                              entry.Tint.Describe(_colorProperties));
                }
            }

            // ---- 2) 拿到容器内容 ----
            // 说明：box.Inventory 这个 getter 不是纯读的（见反编译 InteractableLootbox.cs:59-95），
            // 但它的副作用全是幂等的、而且游戏自己在 Start() 里已经走过一遍了：
            //   * inventoryReference 非空时直接返回，不会新建任何东西；
            //   * hasBeenInspectedInLootBox 为真时调一次 SetMarkerUsed()（MarkAsUsed 内部有
            //     markedAsUsed 守卫，重复调用无副作用）；
            //   * 写一次 DisplayNameKey（游戏自己也在写）。
            // 所以这里只在缓存失效时取一次，之后每轮扫描只读 Inventory.Content（纯字段）。
            Inventory inventory = entry.Inventory;
            if (inventory == null)
            {
                inventory = box.Inventory;
                entry.Inventory = inventory;
            }
            if (inventory == null)
            {
                // box.Inventory 失败时游戏自己会 Debug.LogError（每次带堆栈，很贵），
                // 而且 GetOrCreateInventory 会真的 new 一个 GameObject。
                // 所以这里清掉脏标记，别让同一个坏容器每轮扫描都重试一遍；
                // 10 秒兜底全量重扫会再给它一次机会。
                entry.Dirty = false;
                return;
            }

            // ---- 2b) 挂上「内容变了」的通知 ----
            // Inventory.onContentChanged 是实例事件（Inventory.cs:130），所以每个容器各挂一次。
            // 回调只做一件事：把这个 entry 标脏。真正的重算留到下一次扫描，
            // 这样连续搬十几件物品也只会重算一次，不会每件都刷一遍材质。
            if (!ReferenceEquals(entry.BoundInventory, inventory))
            {
                if (entry.BoundInventory != null && entry.ContentHandler != null)
                {
                    entry.BoundInventory.onContentChanged -= entry.ContentHandler;
                }
                Entry captured = entry;
                entry.ContentHandler = delegate (Inventory changed, int position)
                {
                    // 回调里只做两件极廉价的事，重算本身留给队列按预算做：
                    // 连续搬十几件物品 → 脏标记被置十几次，但只会重算一次。
                    captured.Dirty = true;
                    Enqueue(captured);
                };
                inventory.onContentChanged += entry.ContentHandler;
                entry.BoundInventory = inventory;
                entry.Dirty = true;
            }

            if (inventory.Loading)
            {
                // 内容是异步装填的（LootBoxLoader.Setup），装完之前判定不算数。
                //
                // 【重要】这里**绝对不能**再 Enqueue(entry)。
                // 本方法就是被 DrainWorkQueue 调用的，如果在里面又把自己塞回队列：
                //   出队 → 入队 → 出队 → 入队 …… 队头永远追不上队尾，
                //   每帧固定净增 maxWorkPerFrame 个元素，队列再无清空之日（内存无界增长 + 每帧空转）。
                // 实际后果就是用户遇到的「死亡瞬间卡死」：死亡会一次性刷出一堆尸体容器，
                // 它们全是 Loading 状态，于是在几秒内把这个循环点火。
                //
                // 正确做法：只留脏标记。下一次扫描（≤ scanIntervalMs）会重新入队重试；
                // 而装填过程中每 AddItem 一次都会触发 onContentChanged → ContentHandler → 入队，
                // 所以「装完的瞬间就上色」依然成立，不需要靠这里的自旋。
                entry.Dirty = true;
                return;
            }

            // ---- 2c) 脏标记闸门：这个容器扫过一遍、内容又没变，直接走人 ----
            // 这一行是「容器很多」时的主要省法：它挡在遍历物品之前。
            if (!entry.Dirty && !force)
            {
                return;
            }
            entry.Dirty = false;

            // ---- 3) 算判定；和上次一样就立刻返回（绝大多数扫描都走到这里）----
            int rank;
            int level;
            int bestValue;
            bool suppressed;
            ComputeVerdict(inventory, out rank, out level, out bestValue, out suppressed);

            if (rank == entry.AppliedRank && level == entry.AppliedLevel &&
                bestValue == entry.AppliedBestValue && suppressed == entry.AppliedSuppressed)
            {
                return;
            }
            entry.AppliedRank = rank;
            entry.AppliedLevel = level;
            entry.AppliedBestValue = bestValue;
            entry.AppliedSuppressed = suppressed;

            if (_settings.diagnostics)
            {
                List<Item> content = inventory.Content;
                Debug.Log("[LootRarityMarker][诊断] " + box.name + "：物品数=" +
                          (content != null ? content.Count : 0) + "，档位=" + RarityRules.LevelName(level) +
                          "，最贵单件=" + bestValue + "，排序键=" + rank + "，不染色=" + suppressed);
            }

            // ---- 4) 应用 ----
            if (suppressed)
            {
                entry.Tint.Clear();
                OutlineTint.SetEnabled(entry.Outlinable, false);
                return;
            }

            Color color = QualityPalette.Get(level);
            float intensity = GlowIntensityFor(bestValue);
            entry.Tint.Apply(color, intensity);

            if (_settings.outline && OutlineTint.TryInit())
            {
                if (entry.Outlinable == null)
                {
                    // 上限闸门：范围内已经挂了 16 个描边就不再新增。
                    // 这是「别瞬间抢占性能」这条总原则在路线 A 上的落点。
                    if (_outlinedCount >= MaxOutlinedContainers)
                    {
                        if (!_outlineCapLogged)
                        {
                            _outlineCapLogged = true;
                            Debug.Log("[LootRarityMarker] 描边数量已达上限 " + MaxOutlinedContainers +
                                      " 个，范围内的其余容器只用交互点染色表达。" +
                                      "（想让更多容器有描边，就调小扫描半径或关掉路线 A）");
                        }
                    }
                    else
                    {
                        string report;
                        entry.Outlinable = OutlineTint.Attach(
                            box.gameObject, MaxOutlineSizeMeters, MaxOutlineTargets,
                            _settings.diagnostics, out report);
                        if (entry.Outlinable != null)
                        {
                            _outlinedCount++;
                        }
                        if (_settings.diagnostics && entry.Outlinable != null)
                        {
                            Debug.Log("[LootRarityMarker][诊断] 路线A 给 " + box.name + " 挂描边：" + report);
                        }
                    }
                }
                OutlineTint.SetColor(entry.Outlinable, ScaleRgb(color, intensity));
            }
        }

        /// <summary>把当前的发光配置拼成一句人话，给启动日志用（只调用一次）。</summary>
        private static string GlowSummary()
        {
            if (!_settings.glowByValue)
            {
                return "关（游戏原样亮度）";
            }
            return "开，随容器内最贵的一件（紫 " + GlowScales[1].ToString("F2") + " 倍 → 深红 " +
                   ModSettings.MaxValueGlow.ToString("F2") + " 倍）";
        }

        /// <summary>
        /// 发光曲线的锚点。单位是「物品价值 ÷ 2」—— 和 RarityRules 的阈值同一个刻度，
        /// 所以锚点正好落在档位边界上：600=蓝 1200=紫 2500=橙 5000=浅红 10000=深红。
        /// 低于 600（白/绿/蓝）一律 1.00×，也就是**完全不发光**。
        /// </summary>
        private static readonly int[] GlowUnits = { 600, 1200, 2500, 5000, 10000 };
        private static readonly float[] GlowScales = { 1.00f, 1.15f, 1.50f, 2.00f, 2.60f };

        /// <summary>
        /// 本次要用的发光强度倍率。基准固定是 1.0（游戏原本的亮度，一点都不改）。
        ///
        /// 驱动量是**容器内单件最高价值**，不是所有物品的总和 ——
        /// 总和会让一大半箱子直接顶格（实测 4 件容器 57% 超过 5000），
        /// 那就变成「全都亮 = 都不亮」。详见 ModSettings.glowByValue 的注释。
        ///
        /// 纯算术：最多 5 次比较，不分配、不查表。
        /// </summary>
        private static float GlowIntensityFor(int bestItemValue)
        {
            if (!_settings.glowByValue)
            {
                return 1f;
            }
            float units = bestItemValue * 0.5f;
            if (units <= GlowUnits[0])
            {
                return GlowScales[0];
            }
            for (int i = 1; i < GlowUnits.Length; i++)
            {
                if (units < GlowUnits[i])
                {
                    float t = (units - GlowUnits[i - 1]) / (float)(GlowUnits[i] - GlowUnits[i - 1]);
                    return GlowScales[i - 1] + (GlowScales[i] - GlowScales[i - 1]) * t;
                }
            }
            return ModSettings.MaxValueGlow;
        }

        /// <summary>路线 A 的描边颜色按同一个倍率提亮（alpha 保持不变）。</summary>
        private static Color ScaleRgb(Color color, float intensity)
        {
            return new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a);
        }

        /// <summary>
        /// 算出这个容器的判定结果。
        ///   rank      —— 排序键（品质模式 = 最高档位；价值模式 = 最高价值）。用来判断「和上次一样吗」。
        ///   level     —— 决定颜色的档位（0=白 … 6=深红），见 RarityRules。
        ///   bestValue —— 容器内**单件**最高价值，只用来算发光强度。
        ///   suppressed—— true = 不染色，保持游戏原样（空容器）。
        ///
        /// 注意这里是**故意**去读未鉴定物品的：本 mod 的目的就是让玩家在打开之前知道里面有没有好东西，
        /// Item.DisplayQuality 是普通字段（不随 Inspected 变化），所以雾里的物品照样能算出颜色。
        ///
        /// 档位判定完全交给 RarityRules（复刻「物品价值稀有度与搜索音效」mod 的规则），
        /// 所以交互点的颜色和那个 mod 给物品名上的色用的是同一套标准。
        ///
        /// 价值用的是 Item.Value（单个物品的基础价值，纯字段读取）。
        /// 不用 GetTotalRawValue()：它会按耐久折算、乘堆叠数、还会递归进子容器，
        /// 每个容器每个物品都调一次太贵，而且它不是单调的。
        /// </summary>
        /// <param name="bestValue">容器内单件最高价值（不乘堆叠数），只用来算发光强度。</param>
        private static void ComputeVerdict(Inventory inventory, out int rank, out int level,
                                          out int bestValue, out bool suppressed)
        {
            List<Item> content = inventory.Content;

            int bestMetric = int.MinValue;
            int bestLevel = -1;
            int maxItemValue = 0;

            if (content != null)
            {
                bool byValue = _settings.rankByValue;
                // 索引遍历：Inventory 实现了 IEnumerable<Item>，foreach 会 new 一个迭代器
                for (int i = 0; i < content.Count; i++)
                {
                    Item item = content[i];
                    if (item == null)
                    {
                        continue; // Inventory 的槽位可能是空的
                    }
                    int lv = RarityRules.LevelOf(item);
                    int value = item.Value;
                    if (value > maxItemValue)
                    {
                        maxItemValue = value;
                    }
                    int metric = byValue ? value : lv;
                    if (metric > bestMetric || (metric == bestMetric && lv > bestLevel))
                    {
                        bestMetric = metric;
                        bestLevel = lv;
                    }
                }
            }

            bestValue = maxItemValue;

            if (bestLevel < 0)
            {
                // 空容器（或全空槽）：不该有颜色。已染过的话会被还原。
                rank = int.MinValue;
                level = -1;
                suppressed = true;
                return;
            }

            rank = bestMetric;
            level = bestLevel;
            // 门槛项（最低品质 / 最低价值）已经按用户要求删掉，所以非空容器一律染色。
            suppressed = false;
        }

        private void PruneDeadEntries()
        {
            if (_entries.Count == 0)
            {
                return;
            }
            _deadKeys.Clear();
            foreach (KeyValuePair<int, Entry> pair in _entries)
            {
                Entry entry = pair.Value;
                // 容器被销毁（尸体的容器、换关卡、任务刷掉的箱子），或者它的交互点被重建了
                if (entry.Box == null || (entry.Tint != null && !entry.Tint.IsAlive))
                {
                    _deadKeys.Add(pair.Key);
                }
            }
            for (int i = 0; i < _deadKeys.Count; i++)
            {
                int key = _deadKeys[i];
                Entry dying;
                if (_entries.TryGetValue(key, out dying))
                {
                    UnbindContentHandler(dying);
                    // 已被排进队列的，靠 Dead 标记让 DrainWorkQueue 跳过它（不占预算、不碰已销毁对象）
                    dying.Dead = true;
                    dying.Queued = false;
                }
                _entries.Remove(key);
            }
        }

        /// <summary>把某个容器挂着的 onContentChanged 回调摘掉，避免留下悬空委托。</summary>
        private static void UnbindContentHandler(Entry entry)
        {
            // entry.BoundInventory != null 用的是 Unity 的重载：对象已被销毁时这里是 false，
            // 此时直接跳过 —— 对象都没了，委托也跟着没了，不存在泄漏。
            if (entry.BoundInventory != null && entry.ContentHandler != null)
            {
                entry.BoundInventory.onContentChanged -= entry.ContentHandler;
            }
            entry.BoundInventory = null;
            entry.ContentHandler = null;
        }

        private int ResolveInteractableMask()
        {
            if (_interactableMask == MaskNotResolved)
            {
                // 和游戏 CA_Interact.Awake 一样的取法：1 << LayerMask.NameToLayer("Interactable")
                _interactableMask = LayerMask.GetMask("Interactable");
                if (_interactableMask == 0)
                {
                    Debug.LogError("[LootRarityMarker] 找不到 \"Interactable\" 层，无法扫描容器。");
                }
            }
            return _interactableMask;
        }

        // ======================= 还原 =======================

        /// <summary>把已经染上的颜色全部还原（总开关被关掉时用，之后打开会重新染回来）。</summary>
        private void DeactivateTints()
        {
            ClearWorkQueue(); // 关掉总开关时，队列里没做完的活也不用做了
            foreach (KeyValuePair<int, Entry> pair in _entries)
            {
                Entry entry = pair.Value;
                if (entry.Tint != null && entry.Tint.IsAlive)
                {
                    entry.Tint.Clear();
                }
                if (entry.Outlinable != null)
                {
                    OutlineTint.SetEnabled(entry.Outlinable, false);
                    UnityEngine.Object.Destroy(entry.Outlinable);
                    entry.Outlinable = null;
                }
                entry.AppliedRank = int.MinValue;
                entry.AppliedLevel = -1;
                entry.AppliedBestValue = -1;
                entry.AppliedSuppressed = false;
                entry.Dirty = true;
            }
            _scanRequested = true;
        }

        /// <summary>mod 被停用 / 对象被销毁时：还原颜色、拿掉描边组件、断开所有静态事件。</summary>
        private void ReleaseAll()
        {
            ClearWorkQueue();
            foreach (KeyValuePair<int, Entry> pair in _entries)
            {
                Entry entry = pair.Value;
                if (entry.Tint != null && entry.Tint.IsAlive)
                {
                    entry.Tint.Clear();
                }
                if (entry.Outlinable != null)
                {
                    OutlineTint.SetEnabled(entry.Outlinable, false);
                    UnityEngine.Object.Destroy(entry.Outlinable);
                    entry.Outlinable = null;
                }
                UnbindContentHandler(entry);
            }
        }

        /// <summary>设置变了：丢掉所有染色配方，下一轮扫描按新配置重建。</summary>
        private void RebuildTints()
        {
            // 队列里的待办是按旧配方排的，全部作废；下面把每个容器重新标脏，
            // 下一次扫描会把它们重新排队 —— 于是「改设置」这个尖峰就变成了每帧几个的细水长流。
            ClearWorkQueue();
            _colorProperties = ParseColorProperties(_settings.colorProperties);
            foreach (KeyValuePair<int, Entry> pair in _entries)
            {
                Entry entry = pair.Value;
                if (entry.Tint != null && entry.Tint.IsAlive)
                {
                    entry.Tint.Clear();
                }
                if (!_settings.outline && entry.Outlinable != null)
                {
                    OutlineTint.SetEnabled(entry.Outlinable, false);
                    UnityEngine.Object.Destroy(entry.Outlinable);
                    entry.Outlinable = null;
                }
                entry.Tint = null;
                entry.AppliedRank = int.MinValue;
                entry.AppliedLevel = -1;
                entry.AppliedBestValue = -1;
                entry.AppliedSuppressed = false;
                entry.Dirty = true;
            }
            _tintWarningLogged = false;
            _dumpedFirstMarker = false;
            _scanRequested = true;
            _nextFullRescan = Time.unscaledTime + FullRescanIntervalSeconds;
        }

        // ======================= 事件 =======================

        private void Subscribe()
        {
            if (_subscribed)
            {
                return;
            }
            _subscribed = true;
            LevelManager.OnLevelBeginInitializing += OnLevelBeginInitializing;
            LevelManager.OnLevelInitialized += RequestScan;
            LevelManager.OnAfterLevelInitialized += RequestScan;
            InteractableLootbox.OnStartLoot += OnLootEvent;
            InteractableLootbox.OnStopLoot += OnLootEvent;
        }

        private void Unsubscribe()
        {
            if (!_subscribed)
            {
                return;
            }
            _subscribed = false;
            LevelManager.OnLevelBeginInitializing -= OnLevelBeginInitializing;
            LevelManager.OnLevelInitialized -= RequestScan;
            LevelManager.OnAfterLevelInitialized -= RequestScan;
            InteractableLootbox.OnStartLoot -= OnLootEvent;
            InteractableLootbox.OnStopLoot -= OnLootEvent;
        }

        /// <summary>
        /// 换关卡：旧关卡的容器连同交互点马上会被销毁，颜色跟着对象一起没了，不需要逐个还原；
        /// 这里只清账本（避免留下悬空引用），并在一小段时间内无视「站着不动不扫描」，
        /// 因为容器内容是这个阶段异步装填的。
        /// </summary>
        private void OnLevelBeginInitializing()
        {
            ClearWorkQueue(); // 队列里全是旧关卡的容器，必须先作废
            foreach (KeyValuePair<int, Entry> pair in _entries)
            {
                UnbindContentHandler(pair.Value);
            }
            _entries.Clear();
            _lastScanPos = new Vector3(0f, -99999f, 0f); // 保证第一帧一定算「动过」
            _levelSettleUntil = Time.unscaledTime + LevelSettleSeconds;
            _nextFullRescan = 0f;
            _scanRequested = true;
            _startupLogged = false;
            _saturationWarned = false;
            _dumpedFirstMarker = false;
        }

        /// <summary>关卡事件 / 战利品交互 → 强制补扫一次（无视间隔与「站着不动不扫描」）。</summary>
        private void RequestScan()
        {
            _scanRequested = true;
        }

        private void OnLootEvent(InteractableLootbox box)
        {
            // 关掉战利品界面之后容器内容肯定变了，把那个容器标脏（就算 onContentChanged 因为
            // 什么原因没发到，这里也兜住了）。
            if (box != null)
            {
                Entry entry;
                if (_entries.TryGetValue(box.GetInstanceID(), out entry))
                {
                    entry.Dirty = true;
                    Enqueue(entry); // 精确只重算这一个容器，不用等下一次扫描
                }
            }
            _scanRequested = true;
        }

        // ======================= 设置项 =======================

        /// <summary>
        /// 把本 mod 的数值注册进 ModConfig 的设置菜单，并读回当前值。
        /// 成功返回 true；ModConfig 未加载 / 版本不符 / 缺方法时返回 false（此时用 config.json 的值）。
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
                // 说明一律只用中文：ModConfig 的设置面板是中文界面，
                // 中英混排（"扫描半径（米）/ Scan radius"）在窄面板里会被截断成半截英文，反而更难读。
                ModConfigBridge.AddBoolDropdown("Enabled", "总开关：按品质给容器交互点染色", _settings.enabled);
                ModConfigBridge.AddIntSlider("ScanRadius", "扫描半径（米）",
                    _settings.scanRadius, 1, ModSettings.MaxScanRadius);
                ModConfigBridge.AddIntSlider("ScanIntervalMs", "扫描间隔（毫秒）",
                    _settings.scanIntervalMs, 100, 5000);
                ModConfigBridge.AddBoolDropdown("ScanOnlyWhenMoving", "站着不动时不扫描（省性能）",
                    _settings.scanOnlyWhenMoving);
                ModConfigBridge.AddBoolDropdown("RankByValue", "按价值（而不是品质）挑最高的那件",
                    _settings.rankByValue);
                ModConfigBridge.AddBoolDropdown("GlowByValue", "交互点随价值发光（越贵越亮）",
                    _settings.glowByValue);
                ModConfigBridge.AddBoolDropdown("Outline", "容器本体描边",
                    _settings.outline);
                ModConfigBridge.AddBoolDropdown("Diagnostics", "输出诊断日志（排查问题时才开）",
                    _settings.diagnostics);
                ModConfigBridge.AddIntSlider("MaxWorkPerFrame", "每帧最多处理几个容器（任务队列上限）",
                    _settings.maxWorkPerFrame, ModSettings.MinMaxWorkPerFrame, ModSettings.MaxMaxWorkPerFrame);
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

        /// <summary>ModConfig 每次落盘都会回调，参数是它的完整键（LootRarityMarker_xxx）。</summary>
        private static void OnModConfigChanged(string key)
        {
            if (key == null || !key.StartsWith(ModConfigBridge.ChangedKeyPrefix, StringComparison.Ordinal))
            {
                return;
            }
            ReloadFromModConfig();
        }

        /// <summary>
        /// 把 ModConfig 里的值读回内存（只在注册时和玩家改设置后调用，不参与每帧）。
        /// 版本号 +1 通知 Update 重建染色配方。
        /// </summary>
        private static void ReloadFromModConfig()
        {
            ModSettings s = _settings;
            s.enabled = ModConfigBridge.Load("Enabled", s.enabled);
            s.scanRadius = ModConfigBridge.Load("ScanRadius", s.scanRadius);
            s.scanIntervalMs = ModConfigBridge.Load("ScanIntervalMs", s.scanIntervalMs);
            s.scanOnlyWhenMoving = ModConfigBridge.Load("ScanOnlyWhenMoving", s.scanOnlyWhenMoving);
            s.rankByValue = ModConfigBridge.Load("RankByValue", s.rankByValue);
            s.glowByValue = ModConfigBridge.Load("GlowByValue", s.glowByValue);
            s.outline = ModConfigBridge.Load("Outline", s.outline);
            s.diagnostics = ModConfigBridge.Load("Diagnostics", s.diagnostics);
            s.maxWorkPerFrame = ModConfigBridge.Load("MaxWorkPerFrame", s.maxWorkPerFrame);
            s.Normalize();
            _settingsVersion++;
        }

        // ======================= 小工具 =======================

        private static string[] ParseColorProperties(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return MarkerTint.DefaultColorProperties;
            }
            string[] parts = raw.Split(',');
            List<string> list = new List<string>(parts.Length);
            for (int i = 0; i < parts.Length; i++)
            {
                string s = parts[i].Trim();
                if (s.Length > 0)
                {
                    list.Add(s);
                }
            }
            return (list.Count > 0) ? list.ToArray() : MarkerTint.DefaultColorProperties;
        }

    }
}
