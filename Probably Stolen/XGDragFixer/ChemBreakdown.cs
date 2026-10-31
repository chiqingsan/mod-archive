using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using HarmonyLib;

namespace XGDragFixer;

/// <summary>
/// Diagnostic-only sub-step timing for the nightly chemical simulation, which is
/// the single most expensive vanilla settlement stage. The measured methods are
/// hot per simulated second, so they are only timed while a settlement runs and
/// diagnostics are enabled; prefixes/postfixes never change any result.
/// </summary>
internal static class ChemBreakdown
{
    private sealed class Seg
    {
        public long Ticks;
        public int Calls;
    }

    private static readonly Dictionary<MethodBase, string> Labels = new Dictionary<MethodBase, string>();
    private static readonly Dictionary<string, Seg> Segments = new Dictionary<string, Seg>();
    private static readonly List<MethodBase> Targets = new List<MethodBase>();
    private static bool _installed;

    internal static void Install(HarmonyLib.Harmony harmony)
    {
        if (_installed)
        {
            return;
        }

        _installed = true;

        Add(typeof(ChemicalHelper), "HandleAmbientTempDrop", "环境降温", new[] { typeof(GameItem) });
        Add(typeof(ChemicalHelper), "HandleTemperatureReaction", "温度反应", new[] { typeof(GameItem) });
        Add(typeof(MachineFridge), "CoolZoneContainers", "冷库降温",
            new[] { typeof(GameGridInventory), typeof(double), typeof(double) });

        var pre = new HarmonyMethod(AccessTools.Method(typeof(ChemBreakdown), nameof(Prefix)));
        var post = new HarmonyMethod(AccessTools.Method(typeof(ChemBreakdown), nameof(Postfix)));
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
                Core.Log?.Warning($"化学细分计时补丁失败 {target.Name}: {ex.Message}");
            }
        }

        Core.Log?.Log($"[结算诊断] 已挂接 {patched}/{Targets.Count} 个化学模拟子计时点");
    }

    private static void Add(Type type, string name, string label, Type[]? arguments = null)
    {
        try
        {
            var method = arguments == null
                ? AccessTools.Method(type, name)
                : AccessTools.Method(type, name, arguments);
            if (method == null || Labels.ContainsKey(method))
            {
                return;
            }

            Labels[method] = label;
            Targets.Add(method);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"化学细分目标 {type.Name}.{name} 解析失败: {ex.Message}");
        }
    }

    internal static void Reset()
    {
        Segments.Clear();
    }

    private static void Prefix(MethodBase __originalMethod, out long __state)
    {
        __state = NightBreakdown.Active && __originalMethod != null ? Stopwatch.GetTimestamp() : 0L;
    }

    private static void Postfix(MethodBase __originalMethod, long __state)
    {
        if (__state == 0L || __originalMethod == null || !NightBreakdown.Active)
        {
            return;
        }

        try
        {
            if (!Labels.TryGetValue(__originalMethod, out var label))
            {
                return;
            }

            if (!Segments.TryGetValue(label, out var seg))
            {
                seg = new Seg();
                Segments[label] = seg;
            }

            seg.Ticks += Stopwatch.GetTimestamp() - __state;
            seg.Calls++;
        }
        catch
        {
            // Diagnostics must never break settlement.
        }
    }

    internal static string Summary()
    {
        if (Segments.Count == 0)
        {
            return "无";
        }

        var list = new List<KeyValuePair<string, Seg>>(Segments);
        list.Sort((a, b) => b.Value.Ticks.CompareTo(a.Value.Ticks));
        var text = new StringBuilder(128);
        for (var i = 0; i < list.Count; i++)
        {
            if (i > 0)
            {
                text.Append(", ");
            }

            var seg = list[i].Value;
            text.Append(list[i].Key).Append('=')
                .Append((seg.Ticks * 1000.0 / Stopwatch.Frequency).ToString("0.0")).Append("ms/")
                .Append(seg.Calls).Append('次');
        }

        return text.ToString();
    }
}
