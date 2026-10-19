using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LootRarityMarker
{
    /// <summary>
    /// 路线 A（默认关）：给容器本体挂上游戏自带的大名鼎鼎的 EPOOutline 描边组件，按品质上色。
    ///
    /// 为什么是 EPOOutline 而不是自己写描边：
    ///   * 《逃离鸭科夫》**本体就带了完整的 EPOOutline 运行时**（Managed 下有 EPO.dll / EPOURP.dll /
    ///     EPOUtilities.dll / EPODemo.dll / EPOHDRP.dll），而且它的 URP ScriptableRendererFeature
    ///     （EPOOutline.OutlineEffect）已经**接进了本体的渲染管线** —— globalgamemanagers.assets 里
    ///     能同时搜到 Outliner / OutlineTarget / OutlineParameters / OutlineEffect。
    ///   * 本体自己在用：`HalfObsticle.cs` 里有 `using EPOOutline;` 和 `public Outlinable outline;`，
    ///     关卡里大量半透明障碍物就是靠它描边的。所以这条路是「复用游戏已有设施」，
    ///     不是「往别人管线里塞自己的 RendererFeature」。
    ///
    /// 为什么全走反射、编译期不引用 EPO.dll：
    ///   EPO 是游戏程序集，写死引用会让 mod 在游戏更新/换版本时直接 `Assembly.LoadFrom` 失败
    ///   （整份 mod 都加载不起来，而不是只坏一个功能）。反射的意义是：EPO 不在、改名、签名变了，
    ///   最坏结果只是描边不生效 + 一行警告，染色功能完全不受影响。
    ///
    /// 已知不确定性（所以默认关，见 README）：
    ///   * 运行时 AddComponent 出来的 Outlinable 能不能被 OutlineEffect 选中，取决于
    ///     OutlineEffect 的 OutlineLayerMask 是否包含容器所在的层 —— 那是场景里序列化好的值，
    ///     我们改不了也不该改。
    ///
    /// 【已修】为什么必须筛选渲染器：
    ///   AddAllChildRenderersToRenderingList 会把容器**整个子树**的 MeshRenderer 都收进描边列表，
    ///   而关卡里的「一个容器」往往是一整组场景道具（衣柜、货架、带圆形贴花的机器……）。
    ///   实测后果：一个箱子能描出横跨半个屏幕的大红圈（用户截图复现）。
    ///   所以挂载后必须做三件事：剔除交互点自带的标记方块、剔除超大渲染器、限制总数。
    ///   剔除用 EPO 自己的 RemoveTarget（公开 API），不需要构造委托或表达式树。
    /// </summary>
    internal static class OutlineTint
    {
        private static bool _initDone;
        private static bool _available;
        private static Type _outlinableType;
        private static PropertyInfo _parametersProperty;
        private static PropertyInfo _colorProperty;
        private static PropertyInfo _enabledProperty;
        private static MethodInfo _addAllChildRenderers;
        private static object _meshRendererMode;
        private static PropertyInfo _targetsProperty;
        private static MethodInfo _removeTarget;
        private static PropertyInfo _targetRendererProperty;

        /// <summary>诊断用：路线 A 不可用的原因。</summary>
        public static string UnavailableReason { get; private set; }

        /// <summary>挂载时要剔除的描边目标对象（避免每次挂载都新建 List）。</summary>
        private static readonly List<object> DropScratch = new List<object>(64);

        /// <summary>反射调 RemoveTarget 时复用的参数数组（避免每次删目标都新建数组）。</summary>
        private static readonly object[] RemoveArgs = new object[1];

        /// <summary>查找并校验 EPOOutline 的 API。只做一次；之后直接返回缓存结果。</summary>
        public static bool TryInit()
        {
            if (_initDone)
            {
                return _available;
            }
            _initDone = true;
            _available = TryInitCore();
            if (!_available)
            {
                // 必须报出来。第一版这里不报，结果「描边没生效」在日志里完全查不到原因。
                Debug.Log("[LootRarityMarker] 路线 A 不可用，描边已停用（只影响描边，染色照常）：" +
                          UnavailableReason);
            }
            return _available;
        }

        private static bool TryInitCore()
        {
            try
            {
                _outlinableType = FindType("EPOOutline.Outlinable");
                if (_outlinableType == null)
                {
                    UnavailableReason = "找不到类型 EPOOutline.Outlinable";
                    return false;
                }
                if (!typeof(MonoBehaviour).IsAssignableFrom(_outlinableType))
                {
                    UnavailableReason = "EPOOutline.Outlinable 不是 MonoBehaviour";
                    return false;
                }

                _parametersProperty = _outlinableType.GetProperty("OutlineParameters");
                Type parametersType = (_parametersProperty != null) ? _parametersProperty.PropertyType : null;
                if (parametersType == null)
                {
                    UnavailableReason = "找不到 Outlinable.OutlineParameters";
                    return false;
                }

                _colorProperty = parametersType.GetProperty("Color");
                _enabledProperty = parametersType.GetProperty("Enabled");
                if (_colorProperty == null || _enabledProperty == null ||
                    _colorProperty.PropertyType != typeof(Color) ||
                    _enabledProperty.PropertyType != typeof(bool))
                {
                    UnavailableReason = "OutlineParameters 上没有可写的 Color / Enabled";
                    return false;
                }

                _addAllChildRenderers = _outlinableType.GetMethod("AddAllChildRenderersToRenderingList");
                if (_addAllChildRenderers != null)
                {
                    ParameterInfo[] ps = _addAllChildRenderers.GetParameters();
                    if (ps.Length != 1 || !ps[0].ParameterType.IsEnum)
                    {
                        _addAllChildRenderers = null;
                    }
                    else
                    {
                        // 交互点是 MeshRenderer（不是 SpriteRenderer），用 MeshRenderer 模式即可
                        try
                        {
                            _meshRendererMode = Enum.Parse(ps[0].ParameterType, "MeshRenderer");
                        }
                        catch (Exception)
                        {
                            _meshRendererMode = Enum.ToObject(ps[0].ParameterType, 0);
                        }
                    }
                }
                if (_addAllChildRenderers == null)
                {
                    UnavailableReason = "找不到 AddAllChildRenderersToRenderingList";
                    return false;
                }

                // 筛选渲染器所需的 API。这几个是**必需**的：拿不到筛选能力就宁可整条路线停用，
                // 也绝不能退化成「不筛选」—— 那正是会把整个场景道具组描成大圈的已知坏行为。
                //
                // 【踩过的两个坑，别再犯】
                //   坑 1：OutlineTargets 的声明类型是 IReadOnlyList<OutlineTarget>，它 **不实现**
                //         非泛型的 System.Collections.IList（只有 List<T> 才两个都实现）。用
                //         IList.IsAssignableFrom 判断永远为 false。
                //   坑 2：反射 Count 时不能拿「声明类型」去找 —— IReadOnlyList<T> 自己并没有声明
                //         Count，Count 是上一层 IReadOnlyCollection<T> 声明的；而 Type.GetProperty
                //         **不会**返回从别的接口继承来的成员。于是 Count 拿不到，路线 A 又一次被静默停用。
                //
                // 现在改成完全不碰 Count / 索引器：IReadOnlyList<T> 一定实现 IEnumerable<T>、
                // 也就一定实现非泛型的 System.Collections.IEnumerable，直接枚举即可，
                // 删除也按「对象」删（RemoveTarget），不依赖任何下标，天然免疫错位问题。
                _targetsProperty = _outlinableType.GetProperty("OutlineTargets");
                if (_targetsProperty == null)
                {
                    UnavailableReason = "找不到 Outlinable.OutlineTargets（无法筛选渲染器，停用路线 A）";
                    return false;
                }
                Type targetsType = _targetsProperty.PropertyType;
                if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(targetsType))
                {
                    UnavailableReason = "OutlineTargets（" + targetsType.Name +
                                        "）不可枚举（无法筛选渲染器，停用路线 A）";
                    return false;
                }

                _removeTarget = _outlinableType.GetMethod("RemoveTarget");
                if (_removeTarget == null)
                {
                    UnavailableReason = "找不到 Outlinable.RemoveTarget（无法筛选渲染器，停用路线 A）";
                    return false;
                }
                ParameterInfo[] rps = _removeTarget.GetParameters();
                if (rps.Length != 1)
                {
                    UnavailableReason = "Outlinable.RemoveTarget 签名不符（无法筛选渲染器，停用路线 A）";
                    return false;
                }
                _targetRendererProperty = rps[0].ParameterType.GetProperty("Renderer");
                if (_targetRendererProperty == null ||
                    !typeof(Renderer).IsAssignableFrom(_targetRendererProperty.PropertyType))
                {
                    UnavailableReason = "OutlineTarget 上没有 Renderer（无法筛选渲染器，停用路线 A）";
                    return false;
                }

                _available = true;
                Debug.Log("[LootRarityMarker] 路线 A 可用：已找到游戏自带的 EPOOutline（" +
                          _outlinableType.Assembly.GetName().Name + "）。");
                return true;
            }
            catch (Exception e)
            {
                UnavailableReason = e.Message;
                Debug.LogWarning("[LootRarityMarker] 路线 A 初始化失败（只影响描边）：" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 给容器挂一个 Outlinable，收进渲染器后立刻做筛选。失败返回 null。
        ///
        /// maxSize   —— 单个渲染器包围盒最长边超过这个米数就剔除（挡住巨型场景道具），&lt;=0 表示不限；
        /// maxTargets—— 最多保留几个渲染器（按加入顺序取前 N 个），&lt;=0 表示不限；
        /// wantReport—— 为 true 时把筛选结果写进 report，供 diagnostics 输出。
        /// </summary>
        public static Component Attach(GameObject target, float maxSize, int maxTargets,
                                       bool wantReport, out string report)
        {
            report = null;
            if (!_available || target == null)
            {
                return null;
            }
            try
            {
                Component outlinable = target.AddComponent(_outlinableType);
                if (outlinable == null)
                {
                    return null;
                }

                // 先把子树里的 MeshRenderer 全收进来（这一步就是「大圈」的来源）
                _addAllChildRenderers.Invoke(outlinable, new object[] { _meshRendererMode });

                // 再按我们的规则剔除。不做这一步路线 A 就是坏的。
                if (!PruneTargets(outlinable, maxSize, maxTargets, wantReport, out report))
                {
                    // 筛选失败时宁可不描边，也不能把整个容器子树交给 EPO。
                    UnityEngine.Object.Destroy(outlinable);
                    return null;
                }
                return outlinable;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[LootRarityMarker] 路线 A：给 " + target.name + " 挂 Outlinable 失败：" + e.Message);
                return null;
            }
        }

        /// <summary>
        /// 剔除不该描边的渲染器。三条规则：
        ///   1) 属于交互点标记（InteractMarker）的渲染器 —— 那不是容器本体，描它没有意义；
        ///   2) 包围盒最长边 &gt; maxSize 的渲染器 —— 关卡里超大贴花/圆环/整组道具，就是它们画出大圈；
        ///   3) 超过 maxTargets 的尾部渲染器 —— 防止一个容器挂几十个描边目标。
        /// 删除走 EPO 自己的 RemoveTarget；倒序删保证索引不错位。
        /// </summary>
        private static bool PruneTargets(Component outlinable, float maxSize, int maxTargets,
                                         bool wantReport, out string report)
        {
            report = null;
            DropScratch.Clear();
            object list;
            try
            {
                list = _targetsProperty.GetValue(outlinable, null);
            }
            catch (Exception e)
            {
                report = "读取 OutlineTargets 失败：" + e.Message;
                return false;
            }
            if (list == null)
            {
                report = "OutlineTargets 为 null，筛选失败";
                return false;
            }

            System.Collections.IEnumerable enumerable = list as System.Collections.IEnumerable;
            if (enumerable == null)
            {
                report = "OutlineTargets（" + list.GetType().Name + "）不可枚举，跳过筛选";
                return false;
            }

            int total = 0;
            int marker = 0;
            int oversized = 0;
            int broken = 0;
            int kept = 0;

            DropScratch.Clear();
            try
            {
                // 只枚举、不修改：先把要剔除的对象收集起来，枚举结束后再统一删。
                foreach (object item in enumerable)
                {
                    total++;

                    Renderer renderer = null;
                    if (item != null)
                    {
                        try
                        {
                            renderer = _targetRendererProperty.GetValue(item, null) as Renderer;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (renderer == null)
                    {
                        broken++;
                        DropScratch.Add(item);
                        continue;
                    }

                    // 规则 1：交互点标记本身不描边（那是我们自己染色的方块，不是容器本体）
                    if (renderer.GetComponentInParent<InteractMarker>() != null)
                    {
                        marker++;
                        DropScratch.Add(item);
                        continue;
                    }

                    // 规则 2：超大渲染器不描边（用户截图里那个横跨半屏的红圈就是它）
                    if (maxSize > 0f)
                    {
                        Vector3 size = renderer.bounds.size;
                        float longest = Mathf.Max(size.x, Mathf.Max(size.y, size.z));
                        if (longest > maxSize)
                        {
                            oversized++;
                            DropScratch.Add(item);
                            continue;
                        }
                    }

                    // 规则 3：只保留前 maxTargets 个（按加入顺序）
                    if (maxTargets > 0 && kept >= maxTargets)
                    {
                        DropScratch.Add(item);
                        continue;
                    }

                    kept++;
                }
            }
            catch (Exception e)
            {
                report = "枚举 OutlineTargets 失败：" + e.Message;
                DropScratch.Clear();
                return false;
            }

            // 按「对象」删，不用下标：枚举期间收集的引用就是最终要删的引用，不存在错位问题。
            bool removeFailed = false;
            for (int i = 0; i < DropScratch.Count; i++)
            {
                object victim = DropScratch[i];
                if (victim == null)
                {
                    continue;
                }
                RemoveArgs[0] = victim;
                try
                {
                    _removeTarget.Invoke(outlinable, RemoveArgs);
                }
                catch (Exception)
                {
                    removeFailed = true;
                }
            }

            if (removeFailed)
            {
                report = "删除无效描边目标失败，筛选未完成";
                DropScratch.Clear();
                return false;
            }

            if (wantReport)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("描边目标 ").Append(total).Append(" 个 → 保留 ").Append(total - DropScratch.Count).Append(" 个");
                if (marker > 0 || oversized > 0 || broken > 0)
                {
                    sb.Append("（剔除：标记 ").Append(marker)
                      .Append("、超尺寸 ").Append(oversized);
                    if (broken > 0)
                    {
                        sb.Append("、无效 ").Append(broken);
                    }
                    sb.Append('）');
                }
                report = sb.ToString();
            }
            DropScratch.Clear();
            return true;
        }

        /// <summary>设置描边颜色并启用。</summary>
        public static void SetColor(Component outlinable, Color color)
        {
            if (!_available || outlinable == null)
            {
                return;
            }
            try
            {
                object parameters = _parametersProperty.GetValue(outlinable);
                if (parameters == null)
                {
                    return;
                }
                _colorProperty.SetValue(parameters, color, null);
                _enabledProperty.SetValue(parameters, true, null);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[LootRarityMarker] 路线 A：设置描边颜色失败：" + e.Message);
                UnityEngine.Object.Destroy(outlinable);
            }
        }

        /// <summary>关掉描边（不销毁组件）。</summary>
        public static void SetEnabled(Component outlinable, bool enabled)
        {
            if (!_available || outlinable == null)
            {
                return;
            }
            try
            {
                object parameters = _parametersProperty.GetValue(outlinable);
                if (parameters != null)
                {
                    _enabledProperty.SetValue(parameters, enabled, null);
                }
            }
            catch (Exception)
            {
                // 关闭失败时销毁组件，避免描边继续占用 EPO 的渲染成本。
                UnityEngine.Object.Destroy(outlinable);
            }
        }

        /// <summary>在所有已加载程序集里按全名找类型。</summary>
        private static Type FindType(string fullName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    Type type = assemblies[i].GetType(fullName, false);
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch (Exception)
                {
                    // 个别程序集反射会抛，跳过
                }
            }
            return null;
        }
    }
}
