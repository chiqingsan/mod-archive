using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;

namespace XGDragFixer;

internal static class Diagnostics
{
    internal enum Probe { FindSlot, WindowShow, Aging, Module, Occupancy, Blend }

    private struct Sample
    {
        internal long Count, Total, Max, Errors;
        internal string? FirstError;
    }

    private static readonly Sample[] Samples = new Sample[6];
    private static readonly Dictionary<string, long> Events = new();
    private static bool _active;
    private static bool _tagOwnersReported;
    private static long _lastReport;
    internal static Func<long> Clock = Stopwatch.GetTimestamp;
    internal static Action<string> Write = message => Core.Log?.Log(message);

    private static bool Enabled => Core.DiagnosticsEnabled?.Value == true;

    internal static long Begin() => Enabled ? Clock() + 1 : 0;

    internal static void End(Probe probe, long started, Exception? error)
    {
        if (started == 0 || !Enabled) return;
        var elapsed = Clock() - (started - 1);
        ref var sample = ref Samples[(int)probe];
        sample.Count++;
        sample.Total += elapsed;
        sample.Max = Math.Max(sample.Max, elapsed);
        if (error != null)
        {
            sample.Errors++;
            sample.FirstError ??= error.ToString();
        }
    }

    internal static void Count(string reason)
    {
        if (!Enabled) return;
        Events.TryGetValue(reason, out var count);
        Events[reason] = count + 1;
    }

    internal static void Tick()
    {
        if (!Enabled)
        {
            if (_active)
            {
                Flush();
                Reset();
                Write("[诊断] 已关闭");
            }
            return;
        }

        if (!_active)
        {
            _active = true;
            _lastReport = Clock();
            Write($"[诊断] 已开启 v{typeof(Core).Assembly.GetName().Version}，每 10 秒汇总；" +
                "耗时包含原版方法及其他模组补丁，嵌套调用不可相加；异常仅记录，不拦截。" +
                $" 游戏程序集 MVID={typeof(GameItem).Module.ModuleVersionId}");
        }
        if (Clock() - _lastReport >= 10 * Stopwatch.Frequency)
        {
            Flush();
            _lastReport = Clock();
        }
    }

    // Called only after the game's patch installation attempt. Read metadata without
    // patching or invoking either target, including when diagnostics is enabled later.
    internal static void ReportTagPatchOwners()
    {
        if (!Enabled || _tagOwnersReported) return;
        foreach (var method in new[]
        {
            AccessTools.Method(typeof(GameItem), nameof(GameItem.ModifyTag),
                new[] { typeof(string), typeof(Action<TagState>), typeof(bool) }),
            AccessTools.Method(typeof(ModuleEffectHelper), nameof(ModuleEffectHelper.ResetAllTempStat),
                new[] { typeof(GameItem) })
        })
        {
            if (method == null)
            {
                Write("[诊断] 标签方法缺失，无法读取 Harmony owners");
                continue;
            }
            var info = Harmony.GetPatchInfo(method);
            var owners = info == null || !info.Owners.Any() ? "none" : string.Join(", ", info.Owners.OrderBy(owner => owner));
            Write($"[诊断] Harmony owners {method.DeclaringType.Name}.{method.Name}: {owners}");
        }
        _tagOwnersReported = true;
    }

    internal static void Flush()
    {
        for (var index = 0; index < Samples.Length; index++)
        {
            var sample = Samples[index];
            if (sample.Count == 0) continue;
            var scale = 1000.0 / Stopwatch.Frequency;
            Write($"[诊断] {(Probe)index}: 次数={sample.Count}, " +
                $"均值={sample.Total * scale / sample.Count:F3}ms, 最大={sample.Max * scale:F3}ms, 异常={sample.Errors}");
            if (sample.FirstError != null) Write($"[诊断] {(Probe)index} 本周期首次异常: {sample.FirstError}");
        }
        foreach (var entry in Events) Write($"[诊断] {entry.Key}: {entry.Value}");
        Array.Clear(Samples, 0, Samples.Length);
        Events.Clear();
    }

    internal static void Reset()
    {
        Array.Clear(Samples, 0, Samples.Length);
        Events.Clear();
        _active = false;
        _tagOwnersReported = false;
        _lastReport = 0;
    }
}
