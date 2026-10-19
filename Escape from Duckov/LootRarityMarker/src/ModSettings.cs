using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace LootRarityMarker
{
    /// <summary>
    /// 本 mod 的可配置数值。默认值 = 最“开箱即用”的行为：
    /// 移动时每 0.5 秒扫一次 25 米内的容器，按最高品质给交互点染色，白/绿/蓝/紫/橙/红。
    ///
    /// 取值优先级（低 → 高）：
    ///   1. 这里的字段默认值
    ///   2. 与 dll 同目录的 config.json（没有 ModConfig 框架时也能配置）
    ///   3. ModConfig 框架的存档值（游戏内 Mod Settings 标签页里改过的值优先，见 ModConfigBridge）
    ///
    /// 关于单位：能进 ModConfig 菜单的数值必须是 int（框架的滑条只支持 int），
    /// 所以半径用「米」、间隔用「毫秒」、距离用「厘米」；scanMoveDistance 不上面板，直接用米。
    /// </summary>
    [Serializable]
    public class ModSettings
    {
        /// <summary>总开关。关掉后立刻把已经染上的颜色还原成游戏原样。</summary>
        public bool enabled = true;

        /// <summary>扫描半径（米）。只影响发现容器的范围，不影响已经染好的颜色。</summary>
        public int scanRadius = 25;

        /// <summary>两次扫描之间至少间隔多久（毫秒）。站着不动时不会扫描，这个间隔只在移动中生效。</summary>
        public int scanIntervalMs = 500;

        /// <summary>
        /// 站着不动时完全不扫描（默认开）。这是本 mod 最大的一条性能优化：
        /// 常态下每帧只剩一次 float 比较。任何战利品交互、关卡加载都会强制补扫。
        /// </summary>
        public bool scanOnlyWhenMoving = true;

        /// <summary>距上次扫描位置移动超过多少米才算“动了”（米）。只在 scanOnlyWhenMoving 打开时有意义。</summary>
        public float scanMoveDistance = 1.0f;

        /// <summary>
        /// 排序依据。false（默认）= 按最高品质选“最有价值的那件”，颜色 = 那件的品质；
        /// true = 按最高 Value 选，颜色仍然 = 那件的品质（所以可能看到很高的价值配白/绿颜色）。
        /// </summary>
        public bool rankByValue = false;

        /// <summary>
        /// 交互点按「容器里最贵的那一件」发光（默认关）。
        ///
        /// 为什么是「单件最高价值」而不是「总价值」：
        ///   稀有度阈值（见 RarityRules）本来就是按**单件**标定的 ——
        ///   先把 Value 除以 2，再和 200/600/1200/2500/5000/10000 比。
        ///   把 4 件物品的价值加起来喂进去，是量纲错配：实测从发行资源里抽样模拟，
        ///   4 件容器里有 57% 的总价值直接顶到 5000 以上，也就是**一大半箱子会顶格发光**，
        ///   这正好是「全都亮等于都不亮」。
        ///   换成单件最高价值后，一个装满垃圾、只塞了一件好东西的箱子会亮，
        ///   一个装满中等货的箱子不会 —— 这才是玩家真正想知道的信息。
        ///
        /// 亮度曲线（锚点就是 RarityRules 里那套阈值的「除以 2 之后」的刻度）：
        ///   价值/2 ≤ 600（白/绿/蓝） → 1.00× 完全不发光
        ///   = 1200（紫）             → 1.15×
        ///   = 2500（橙）             → 1.50×
        ///   = 5000（浅红）           → 2.00×
        ///   ≥ 10000（深红）          → 2.60×
        /// 中间线性插值，所以「越贵越亮」仍然成立，但低档位是**完全无感**的，
        /// 不会出现一屏箱子都在发光。
        ///
        /// 原理：交互点着色器 BillBoardOverlay_Markers 的 _Tint / _Tint2 是 HDR 颜色，
        /// 原版值就超过 1（光点最高 3.0、圆环 3.44），那不是笔误，是发光强度。
        /// 游戏的 URP Asset 开了 HDR（m_SupportsHDR = 1），所以超过 1 的亮度真的会被送到屏幕上。
        /// </summary>
        public bool glowByValue = false;

        /// <summary>发光倍率的上限（深红档）。</summary>
        public const float MaxValueGlow = 2.6f;

        /// <summary>
        /// 路线 A：给容器本体加游戏自带的 EPOOutline 描边。
        /// 默认关；打开后由固定的描边数量和单帧工作预算控制成本。
        /// </summary>
        public bool outline = false;

        /// <summary>诊断日志。打开后会把第一次遇到的容器上所有材质属性、每次判定的结果打到 Player.log。</summary>
        public bool diagnostics = false;

        /// <summary>
        /// 覆盖染色用的材质属性名（英文逗号分隔）。留空 = 用内置的 "_Tint,_Tint2"。
        /// 内置值是直接从游戏发行资源里挖出来的、真正承载交互点颜色的两个属性；
        /// 只有在游戏更新换了属性名时才需要动它（配合 diagnostics=true 看日志）。
        /// </summary>
        public string colorProperties = "";

        /// <summary>
        /// 每帧最多处理几个容器（默认 8）—— 任务队列的数量上限。
        /// 队列同时受代码内固定的单帧时间预算限制。
        /// 换关卡 / 改设置会在同一帧里给几百个容器标脏，不分批处理就是一次实打实的掉帧。
        /// 队列每帧只放行这么多，剩下的留到下一帧（改设置时颜色会像波纹一样逐个亮起来）。
        /// </summary>
        public int maxWorkPerFrame = 8;

        public const int MaxScanRadius = 200;
        public const int MinScanIntervalMs = 50;
        public const int MaxScanIntervalMs = 10000;
        public const int MinMaxWorkPerFrame = 1;
        public const int MaxMaxWorkPerFrame = 64;

        public void Normalize()
        {
            if (scanRadius < 1)
            {
                scanRadius = 1;
            }
            else if (scanRadius > MaxScanRadius)
            {
                scanRadius = MaxScanRadius;
            }

            if (scanIntervalMs < MinScanIntervalMs)
            {
                scanIntervalMs = MinScanIntervalMs;
            }
            else if (scanIntervalMs > MaxScanIntervalMs)
            {
                scanIntervalMs = MaxScanIntervalMs;
            }

            if (scanMoveDistance < 0f)
            {
                scanMoveDistance = 0f;
            }
            else if (scanMoveDistance > 100f)
            {
                scanMoveDistance = 100f;
            }

            if (maxWorkPerFrame < MinMaxWorkPerFrame)
            {
                maxWorkPerFrame = MinMaxWorkPerFrame;
            }
            else if (maxWorkPerFrame > MaxMaxWorkPerFrame)
            {
                maxWorkPerFrame = MaxMaxWorkPerFrame;
            }
        }

        /// <summary>扫描半径（米 → float）。</summary>
        public float ScanRadiusMeters
        {
            get { return scanRadius; }
        }

        /// <summary>扫描间隔（毫秒 → 秒）。</summary>
        public float ScanIntervalSeconds
        {
            get { return scanIntervalMs * 0.001f; }
        }

        /// <summary>移动判定阈值，预先平方，避免每帧开方。</summary>
        public float ScanMoveDistanceSquared
        {
            get { return scanMoveDistance * scanMoveDistance; }
        }

        /// <summary>
        /// 用 mod 目录下（与 dll 同级）的 config.json 覆盖默认值。
        /// 文件不存在或格式错误时静默使用代码里的默认值，不影响 mod 工作。
        /// </summary>
        public static ModSettings LoadFromModFolder()
        {
            ModSettings settings = new ModSettings();
            try
            {
                string path = Path.Combine(ModFolder, "config.json");
                if (File.Exists(path))
                {
                    // JsonUtility 由 UnityEngine.JSONSerializeModule 提供，游戏自带，无第三方依赖
                    JsonUtility.FromJsonOverwrite(File.ReadAllText(path, Encoding.UTF8), settings);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[LootRarityMarker] 读取 config.json 失败，改用内置默认值：" + e.Message);
            }
            settings.Normalize();
            return settings;
        }

        private static string _modFolder;

        /// <summary>mod 自己的目录（dll 所在目录）。</summary>
        internal static string ModFolder
        {
            get
            {
                if (_modFolder == null)
                {
                    try
                    {
                        _modFolder = Path.GetDirectoryName(typeof(ModBehaviour).Assembly.Location);
                    }
                    catch (Exception)
                    {
                        _modFolder = null;
                    }
                    if (string.IsNullOrEmpty(_modFolder))
                    {
                        _modFolder = ".";
                    }
                }
                return _modFolder;
            }
        }
    }
}
