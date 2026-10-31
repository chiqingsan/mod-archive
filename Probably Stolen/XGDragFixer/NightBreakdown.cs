using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using HarmonyLib;

namespace XGDragFixer;

/// <summary>
/// Diagnostic-only timing breakdown of the vanilla night settlement
/// (PlayerStore.EndNight). Every measured stage is a pure time accounting layer:
/// prefixes/postfixes never alter the result, they are installed once and only
/// cost anything while a settlement is running with diagnostics enabled.
/// Results are written to the game log as a single "过夜结算细分" line per night.
/// </summary>
internal static class NightBreakdown
{
    private struct Frame
    {
        public MethodBase Method;
        public bool Outer;
        public long Start;
    }

    private sealed class Seg
    {
        public long Ticks;
        public int Calls;
    }

    private static readonly Dictionary<string, Seg> Segments = new Dictionary<string, Seg>(48);
    private static readonly Dictionary<MethodBase, string> Labels = new Dictionary<MethodBase, string>();
    private static readonly List<MethodBase> Targets = new List<MethodBase>(64);
    private static readonly Stack<Frame> Stack = new Stack<Frame>();
    private static bool _installed;

    internal static bool Active { get; private set; }

    private static long _overallStart;
    private static long _memStart;
    private static int _itemScans;
    private static int _itemCount;

    internal static void Install(HarmonyLib.Harmony harmony)
    {
        if (_installed)
        {
            return;
        }

        _installed = true;

        Add(typeof(PlayerStore), "FindAllItem", "物品扫描", new[] { typeof(bool) });
        Add(typeof(PlayerStore), "GetTotalEstimatedValue", "总价值计算");
        Add(typeof(PlayerStore), "UpdatePowerState", "电力更新");
        Add(typeof(RunStats), "OnDayEnded", "日结统计");
        Add(typeof(ChemicalHelper), "RunNightlyChemicalSimulation", "化学模拟");
        Add(typeof(MachineFridge), "ClearColdMarkers", "冷库标记清理");
        Add(typeof(MachineFridge), "HandleColdFeatureExpiry", "冷库过期处理");
        Add(typeof(HopperHelper), "RunWindow", "漏斗扫描", new[] { typeof(int) });
        Add(typeof(StorePhoneClient), "OnEndDay", "手机日结");
        Add(typeof(EmporiumEntry), "ClearFrontInv", "清空前台");
        Add(typeof(EmporiumEntry), "ClearFrontInvNonOwned", "清空前台(非己)");
        Add(typeof(ModHook), "FireOnGoingSleepEarly", "ModHook 事件");
        Add(typeof(ModHook), "FireOnHandlingNightlyServicesEarly", "ModHook 事件");
        Add(typeof(ModHook), "FireOnHandlingNightlyServicesLate", "ModHook 事件");
        Add(typeof(ModHook), "FireOnGoingSleepLate", "ModHook 事件");
        Add(typeof(ReactorHelper), "OnNight", "反应堆夜结");
        Add(typeof(DebrisHelper), "OnNight", "杂物夜结");
        Add(typeof(UseCountHelper), "OnNight", "使用次数夜结");
        Add(typeof(LouBombHelper), "OnNight", "炸弹夜结");
        Add(typeof(ExpeditionData), "OnDayEnd", "探险日结");
        Add(typeof(HealthData), "OnSleep", "健康睡眠");
        Add(typeof(ExchangeManager), "ResetPromotional", "促销重置");
        Add(typeof(StartingPerkContent), "HandleAgoraphobiaReady", "特质处理");
        Add(typeof(StolenHelper), "UpdateStolenItem", "赃物更新", new[] { typeof(GameItem) });
        Add(typeof(GameItem), "ValidateShapeState", "物品刷新");
        Add(typeof(GameItem), "GetIncompleteSlotInInventory", "槽位查询");
        Add(typeof(GameItem), "GetNegociatedValue", "物品估值");
        Add(typeof(StoreUIManager), "ClearArrestState", "清逮捕状态");

        // All PlayerStore.HandleXxx() settlement steps share one aggregate label.
        try
        {
            foreach (var method in typeof(PlayerStore).GetMethods(BindingFlags.Instance |
                                                                 BindingFlags.Public | BindingFlags.NonPublic |
                                                                 BindingFlags.DeclaredOnly))
            {
                if (method.Name.StartsWith("Handle", StringComparison.Ordinal) &&
                    method.GetParameters().Length == 0)
                {
                    Add(method, "结算Handler");
                }
            }
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"结算诊断目标扫描失败: {ex.Message}");
        }

        var pre = new HarmonyMethod(AccessTools.Method(typeof(NightBreakdown), nameof(TimerPrefix)));
        var post = new HarmonyMethod(AccessTools.Method(typeof(NightBreakdown), nameof(TimerPostfix)));
        var patched = 0;
        foreach (var target in Targets)
        {
            try
            {
                harmony.Patch(target, pre, post);
                patched++;
            }
            catch (Exception ex)
            {
                Core.Log?.Warning($"结算诊断补丁失败 {target.Name}: {ex.Message}");
            }
        }

        // The item scans also feed the "扫描 N 次/M 件" totals of the report.
        try
        {
            var findAll = AccessTools.Method(typeof(PlayerStore), "FindAllItem", new[] { typeof(bool) });
            if (findAll != null)
            {
                harmony.Patch(findAll, null,
                    new HarmonyMethod(AccessTools.Method(typeof(NightBreakdown), nameof(FindAllItemPostfix))));
            }
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"结算扫描计数补丁失败: {ex.Message}");
        }

        if (Core.Log != null)
        {
            Core.Log.Log($"[结算诊断] 已挂接 {patched}/{Targets.Count} 个结算阶段计时点");
        }
    }

    private static void Add(Type type, string methodName, string label, Type[]? argumentTypes = null)
    {
        try
        {
            var method = argumentTypes == null
                ? AccessTools.Method(type, methodName)
                : AccessTools.Method(type, methodName, argumentTypes);
            Add(method, label);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"结算诊断目标 {type.Name}.{methodName} 解析失败: {ex.Message}");
        }
    }

    private static void Add(MethodBase? method, string label)
    {
        if (method == null || Labels.ContainsKey(method))
        {
            return;
        }

        Labels[method] = label;
        Targets.Add(method);
    }

    /// <summary>Called from the EndNight prefix, after the A/B gate approved the settlement.</summary>
    internal static void Begin()
    {
        if (!PerfDiagnostics.Enabled)
        {
            return;
        }

        Segments.Clear();
        Stack.Clear();
        ChemBreakdown.Reset();
        NightSimQuiet.Reset();
        ChemTagCache.ResetCounters();
        _itemScans = 0;
        _itemCount = 0;
        _overallStart = Stopwatch.GetTimestamp();
        _memStart = GC.GetTotalMemory(false);
        Active = true;
    }

    /// <summary>Called from the EndNight postfix, after the deferred flushes ran.</summary>
    internal static void End()
    {
        if (!Active)
        {
            return;
        }

        Active = false;
        try
        {
            var totalMs = (Stopwatch.GetTimestamp() - _overallStart) * 1000.0 / Stopwatch.Frequency;
            var allocMb = (GC.GetTotalMemory(false) - _memStart) / (1024.0 * 1024.0);
            var text = new StringBuilder(256);
            text.Append("[结算诊断] 过夜结算细分：总=").Append(totalMs.ToString("0.0")).Append("ms")
                .Append(" 分配=").Append(allocMb.ToString("0.0")).Append("MB")
                .Append(" 扫描=").Append(_itemScans).Append("次/").Append(_itemCount).Append("件");
            text.AppendLine();
            text.Append("  阶段：");
            var list = new List<KeyValuePair<string, Seg>>(Segments);
            list.Sort((a, b) => b.Value.Ticks.CompareTo(a.Value.Ticks));
            for (var i = 0; i < list.Count; i++)
            {
                if (i == 12)
                {
                    break;
                }

                var seg = list[i].Value;
                if (i > 0)
                {
                    text.Append(", ");
                }

                text.Append(list[i].Key).Append('=')
                    .Append((seg.Ticks * 1000.0 / Stopwatch.Frequency).ToString("0.0")).Append("ms/")
                    .Append(seg.Calls).Append('次');
            }

            text.Append("；库存刷新：抑制 ").Append(NightValidateBatch.SuppressedCalls)
                .Append("次，实际刷新 ").Append(NightValidateBatch.FlushedInventories)
                .Append("个(").Append(NightValidateBatch.FlushMs.ToString("0.0")).Append("ms)");
            text.AppendLine();
            text.Append("  化学细分：").Append(ChemBreakdown.Summary())
                .Append("；界面静默：")
                .Append(NightSimQuiet.Installed ? NightSimQuiet.Summary() : "未启用")
                .Append("；")
                .Append(ChemTagCache.Installed ? ChemTagCache.Summary() : "化学标签缓存未启用");
            Core.Log?.Log(text.ToString());
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"结算诊断汇总失败: {ex.GetType().Name} {ex.Message}");
        }
        finally
        {
            Segments.Clear();
            Stack.Clear();
        }
    }

    internal static void Reset()
    {
        Active = false;
        Segments.Clear();
        Stack.Clear();
        ChemBreakdown.Reset();
        NightSimQuiet.Reset();
        ChemTagCache.ResetCounters();
    }

    private static void TimerPrefix(MethodBase __originalMethod)
    {
        if (!Active || __originalMethod == null)
        {
            return;
        }

        try
        {
            Stack.Push(new Frame
            {
                Method = __originalMethod,
                Outer = Stack.Count == 0,
                Start = Stopwatch.GetTimestamp()
            });
        }
        catch
        {
            // Diagnostics must never break settlement.
        }
    }

    private static void TimerPostfix()
    {
        if (!Active || Stack.Count == 0)
        {
            return;
        }

        try
        {
            var frame = Stack.Pop();
            if (!frame.Outer)
            {
                return;
            }

            var elapsed = Stopwatch.GetTimestamp() - frame.Start;
            if (!Labels.TryGetValue(frame.Method, out var label))
            {
                return;
            }

            if (!Segments.TryGetValue(label, out var seg))
            {
                seg = new Seg();
                Segments[label] = seg;
            }

            seg.Ticks += elapsed;
            seg.Calls++;
        }
        catch
        {
            // Diagnostics must never break settlement.
        }
    }

    private static void FindAllItemPostfix(ref List<GameItem> __result)
    {
        if (!Active)
        {
            return;
        }

        _itemScans++;
        _itemCount += __result?.Count ?? 0;
    }
}
