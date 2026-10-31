using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// Optional diagnostic mode: per-patch call/skip counters, frame-time percentiles,
/// managed-allocation rate and GC counts, written to the game log once per report
/// interval (no files are written; the mod only declares reflection access).
/// Also provides the A/B switch: a hotkey and/or an automatic timer that turns all
/// performance patches off and on again, tagging every report line with the phase,
/// so a single normal play session yields comparable "on" and "off" samples.
/// Disabled by default; while disabled every entry point returns after a single
/// bool check.
/// </summary>
internal static class PerfDiagnostics
{
    internal enum PerfId
    {
        DragFix,
        FastSlot,
        FastSlotScroll,
        ContainerCold,
        LayoutSuppress,
        MaskSuppress,
        ValidateSkip,
        PeekBlock,
        TooltipCache,
        TooltipIdle,
        RaycastCache,
        ContextMenu,
        EndNight,
        AgingBatch,
        MachineBatch,
        ModuleEffect,
        SpriteGuard,
        SlotIndex,
        StoreHover,
        TextFieldFocus,
        SizeFitter
    }

    private const int IdCount = (int)PerfId.SizeFitter + 1;

    private static readonly string[] Names =
    {
        "DragFix", "FastSlot", "FastSlotScroll", "ContainerCold", "LayoutSuppress", "MaskSuppress",
        "ValidateSkip", "PeekBlock", "TooltipCache", "TooltipIdle", "RaycastCache", "ContextMenu",
        "EndNight", "AgingBatch", "MachineBatch", "ModuleEffect", "SpriteGuard", "SlotIndex",
        "StoreHover", "TextFieldFocus", "SizeFitter"
    };

    internal static bool Enabled { get; private set; }

    private static readonly long[] Calls = new long[IdCount];
    private static readonly long[] Saves = new long[IdCount];
    private static readonly long[] Ticks = new long[IdCount];

    private static float _reportSeconds = 10f;
    private static float _nextReport;
    private static int _intervalIndex;
    private static readonly List<float> FrameMs = new List<float>(2048);
    private static long _allocAtStart;
    private static int _gcAtStart;

    // A/B switch state.
    private static bool _hotkeyEnabled;
    private static bool _autoEnabled;
    private static ABMode _abMode = ABMode.Hotkey;
    private static KeyCode _abKey = KeyCode.F8;
    private static string _abKeyName = "F8";
    private static int _autoABSeconds = 300;
    private static float _nextToggleAt;
    private static bool _abPhase = true;
    private static bool _switchLogged;

    internal static void Enable(float reportSeconds, ABMode mode, int autoABSeconds, string keyName)
    {
        if (Enabled)
        {
            return;
        }

        Enabled = true;
        _reportSeconds = Math.Max(1f, reportSeconds);
        _hotkeyEnabled = mode is ABMode.Hotkey or ABMode.Both;
        _autoEnabled = mode is ABMode.Auto or ABMode.Both;
        _autoABSeconds = Math.Max(0, Math.Min(3600, autoABSeconds));
        _abMode = mode;
        _abKey = ParseKey(keyName);
        _abKeyName = keyName;
        _abPhase = true;
        PerfGate.Active = true;
        _switchLogged = false;

        _boundary = Time.realtimeSinceStartup;
        _nextReport = _boundary + _reportSeconds;
        _nextToggleAt = _boundary + (_autoEnabled && _autoABSeconds > 0 ? _autoABSeconds : 0f);
        _allocAtStart = GC.GetTotalMemory(false);
        _gcAtStart = GC.CollectionCount(0);
        Core.Log?.Log($"性能诊断已开启：每 {_reportSeconds:0} 秒在游戏日志输出一行统计");
        if (mode == ABMode.Off)
        {
            Core.Log?.Log("  A/B 切换：未启用（只记录当前状态）");
        }
        else if (_autoEnabled && _autoABSeconds > 0)
        {
            Core.Log?.Log(_hotkeyEnabled
                ? $"  A/B 切换：每 {_autoABSeconds} 秒自动切换，也可按 {_abKey} 手动切换（日志里 ab=全开/全关 标记阶段）"
                : $"  A/B 切换：每 {_autoABSeconds} 秒自动切换（日志里 ab=全开/全关 标记阶段）");
        }
        else
        {
            Core.Log?.Log(_hotkeyEnabled
                ? $"  A/B 切换：按 {_abKey} 在补丁全开/全关之间切换"
                : "  A/B 切换：未启用（只记录当前状态）");
        }
    }

    /// <summary>
    /// Called once the game world is ready (fonts/canvas exist) so the startup
    /// notice is actually visible; Enable() itself runs too early for UI.
    /// </summary>
    internal static void ShowStartupNotice()
    {
        if (!Enabled)
        {
            return;
        }

        Toast.Show($"性能诊断已开启（{ModeText(_abMode)}）· {HelpText()}", 6f);
    }

    private static string ModeText(ABMode mode)
    {
        return mode switch
        {
            ABMode.Hotkey => "热键",
            ABMode.Auto => "自动",
            ABMode.Both => "热键+自动",
            _ => "关闭"
        };
    }

    private static string HelpText()
    {
        if (_abMode == ABMode.Off)
        {
            return "仅记录，不切换";
        }

        if (_autoEnabled && _autoABSeconds > 0)
        {
            return _hotkeyEnabled
                ? $"每 {_autoABSeconds:0} 秒自动切换，或按 {_abKey} 手动切换"
                : $"每 {_autoABSeconds:0} 秒自动切换";
        }

        return $"按 {_abKey} 切换补丁全开/全关";
    }

    private static float _boundary;

    internal static void Shutdown()
    {
        if (!Enabled)
        {
            return;
        }

        Report(force: true);
        Enabled = false;
        PerfGate.Active = true;
        Array.Clear(Calls, 0, Calls.Length);
        Array.Clear(Saves, 0, Saves.Length);
        Array.Clear(Ticks, 0, Ticks.Length);
        FrameMs.Clear();
        Core.Log?.Log("性能诊断已停止");
    }

    internal static long Begin()
    {
        return Enabled ? Stopwatch.GetTimestamp() : 0L;
    }

    /// <summary>Records one hook invocation that let the vanilla code run.</summary>
    internal static void Hit(PerfId id)
    {
        if (!Enabled)
        {
            return;
        }

        Calls[(int)id]++;
    }

    /// <summary>Records one hook invocation that replaced (skipped) the vanilla work.</summary>
    internal static void Skip(PerfId id)
    {
        if (!Enabled)
        {
            return;
        }

        Calls[(int)id]++;
        Saves[(int)id]++;
    }

    /// <summary>Records one finished hook invocation; <paramref name="replacedVanilla"/> means the vanilla work was skipped.</summary>
    internal static void End(PerfId id, long begin, bool replacedVanilla)
    {
        if (!Enabled)
        {
            return;
        }

        var index = (int)id;
        Calls[index]++;
        if (replacedVanilla)
        {
            Saves[index]++;
        }

        if (begin != 0L)
        {
            Ticks[index] += Stopwatch.GetTimestamp() - begin;
        }
    }

    private static float _disabledHintAt = -10f;

    /// <summary>
    /// When diagnostics are off the hotkey has no effect; show a short hint so
    /// pressing it never feels broken.
    /// </summary>
    private static void NotifyDisabledHotkey()
    {
        if (!Input.anyKeyDown)
        {
            return;
        }

        var keyName = Core.PerfABKey?.Value;
        if (string.IsNullOrWhiteSpace(keyName))
        {
            return;
        }

        var key = ParseKey(keyName);
        if (key == KeyCode.None || !Input.GetKeyDown(key))
        {
            return;
        }

        if (Time.realtimeSinceStartup - _disabledHintAt < 1f)
        {
            return;
        }

        _disabledHintAt = Time.realtimeSinceStartup;
        Toast.Show("性能诊断未开启：设置 → 模组设置 → 深空当铺 · 更好的性能优化 → 诊断调试 → 开启 PerfDiagnostics 并重启", 5f);
    }

    internal static void Frame()
    {
        if (!Enabled)
        {
            NotifyDisabledHotkey();
            return;
        }

        SyncSettings();
        FrameMs.Add(Time.unscaledDeltaTime * 1000f);
        var now = Time.realtimeSinceStartup;

        // A/B switch: hotkey first (works in Hotkey/Both), then the automatic timer.
        var hotkeyDown = Input.GetKeyDown(_abKey);
        if (_hotkeyEnabled && hotkeyDown)
        {
            Toggle("热键");
        }
        else if (_autoEnabled && _autoABSeconds > 0 && now >= _nextToggleAt)
        {
            Toggle($"自动 {_autoABSeconds}s");
        }
        else if (!_hotkeyEnabled && hotkeyDown)
        {
            Toast.Show($"当前为「{ModeText(_abMode)}」模式，热键不生效（设置 → 诊断调试 → PerfABMode）", 4f);
        }

        if (Time.realtimeSinceStartup >= _nextReport)
        {
            Report(force: false);
        }
    }

    /// <summary>
    /// Applies report interval / A/B mode / hotkey changes from the mod settings
    /// without a restart, so the player can rebind or switch mode mid-session.
    /// </summary>
    private static void SyncSettings()
    {
        var report = Math.Max(1f, Math.Min(120f, Core.PerfReportSeconds?.Value ?? 10));
        if (Math.Abs(report - _reportSeconds) > 0.01f)
        {
            _reportSeconds = report;
            _nextReport = Time.realtimeSinceStartup + report;
            Core.Log?.Log($"[性能诊断] 报告间隔改为 {report:0} 秒");
        }

        var keyName = Core.PerfABKey?.Value ?? "F8";
        if (!string.Equals(keyName, _abKeyName, StringComparison.OrdinalIgnoreCase))
        {
            _abKeyName = keyName;
            _abKey = ParseKey(keyName);
            Core.Log?.Log($"[性能诊断] A/B 热键改为 {_abKey}");
        }

        var mode = Core.PerfABMode?.Value ?? ABMode.Off;
        var auto = Math.Max(0, Math.Min(3600, Core.PerfAutoABSeconds?.Value ?? 300));
        if (mode != _abMode || auto != _autoABSeconds)
        {
            _abMode = mode;
            _hotkeyEnabled = mode is ABMode.Hotkey or ABMode.Both;
            _autoEnabled = mode is ABMode.Auto or ABMode.Both;
            _autoABSeconds = auto;
            _nextToggleAt = Time.realtimeSinceStartup +
                           (_autoEnabled && _autoABSeconds > 0 ? _autoABSeconds : 0f);
            var text = $"A/B 模式：{ModeText(mode)} · {HelpText()}";
            Core.Log?.Log($"[性能诊断] {text}");
            Toast.Show(text, 3f);
        }
    }

    private static void Toggle(string reason)
    {
        // Seal the current interval first so the old phase keeps clean samples.
        Report(force: false);

        _abPhase = !_abPhase;
        PerfGate.Active = _abPhase;
        ResetSwitchCaches();

        _nextToggleAt = Time.realtimeSinceStartup + (_autoEnabled && _autoABSeconds > 0 ? _autoABSeconds : 0f);
        if (!_switchLogged)
        {
            _switchLogged = true;
            Core.Log?.Log("[性能诊断] A/B 已开始，日志里 ab=全开/全关 标记每段统计属于哪个阶段");
        }

        Core.Log?.Log($"[性能诊断] A/B：补丁全{(_abPhase ? "开" : "关")}（{reason}）");
        Toast.Show($"A/B 已切换：性能补丁{(_abPhase ? "全开" : "全关")}（{reason}）", 2.5f);
    }

    private static void ResetSwitchCaches()
    {
        // Drop per-frame caches so a phase change never mixes samples from the other phase.
        try
        {
            RaycastFrameCache.Reset();
            TooltipIdleSkip.Reset();
            ContextMenuSkip.Reset();
            StoreHoverCache.Reset();
            TextFieldFocusCache.Reset();
            SizeFitterGuard.Reset();
            FastSlotFind.Reset();
            TooltipCache.Clear();
            MachineBatch.Reset();
            AgingBatch.Reset();
            LayoutSuppressor.Reset();
            HoverPeek.ResetState();
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"A/B 切换时重置缓存失败: {ex.GetType().Name} {ex.Message}");
        }
    }

    private static KeyCode ParseKey(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return KeyCode.F8;
        }

        return Enum.TryParse<KeyCode>(name.Trim(), true, out var key) ? key : KeyCode.F8;
    }

    private static void Report(bool force)
    {
        var now = Time.realtimeSinceStartup;
        var elapsed = now - _boundary;
        if (elapsed <= 0.001f || (FrameMs.Count == 0 && !force))
        {
            return;
        }

        var interval = ++_intervalIndex;
        var frames = FrameMs.Count;
        var fps = frames / elapsed;
        var p50 = Percentile(0.50f);
        var p95 = Percentile(0.95f);
        var p99 = Percentile(0.99f);
        var over33 = 0;
        foreach (var ms in FrameMs)
        {
            if (ms > 33.3f)
            {
                over33++;
            }
        }

        var alloc = GC.GetTotalMemory(false) - _allocAtStart;
        var allocKbPerSec = alloc / 1024.0 / elapsed;
        var gc0 = GC.CollectionCount(0) - _gcAtStart;
        var patchMs = 0.0;
        for (var i = 0; i < IdCount; i++)
        {
            patchMs += Ticks[i] * 1000.0 / Stopwatch.Frequency;
        }

        var top = new List<(string Name, double SkipPct, double MsPerSec)>();
        var detail = new StringBuilder();
        for (var i = 0; i < IdCount; i++)
        {
            var calls = Calls[i];
            var saves = Saves[i];
            var skipPct = calls == 0 ? 0.0 : saves * 100.0 / calls;
            var ms = Ticks[i] * 1000.0 / Stopwatch.Frequency;
            if (calls > 0)
            {
                top.Add((Names[i], skipPct, ms / elapsed));
                if (detail.Length > 0)
                {
                    detail.Append("； ");
                }

                detail.Append(Names[i]).Append('=').Append(calls).Append("次/跳过")
                    .Append(skipPct.ToString("0", CultureInfo.InvariantCulture)).Append("%/")
                    .Append(ms.ToString("0.00", CultureInfo.InvariantCulture)).Append("ms");
            }
        }

        top.Sort((a, b) => b.MsPerSec.CompareTo(a.MsPerSec));
        var summary = new StringBuilder();
        summary.Append("[性能诊断] #").Append(interval)
            .Append(" ab=").Append(_abPhase ? "全开" : "全关")
            .Append(" fps=").Append(fps.ToString("0.0", CultureInfo.InvariantCulture))
            .Append(" p50=").Append(p50.ToString("0.0", CultureInfo.InvariantCulture))
            .Append("ms p95=").Append(p95.ToString("0.0", CultureInfo.InvariantCulture))
            .Append("ms p99=").Append(p99.ToString("0.0", CultureInfo.InvariantCulture))
            .Append("ms >33ms=").Append(over33)
            .Append(" 分配=").Append(allocKbPerSec.ToString("0", CultureInfo.InvariantCulture)).Append("KB/s")
            .Append(" GC0=").Append(gc0)
            .Append(" 补丁耗时=").Append(patchMs.ToString("0.0", CultureInfo.InvariantCulture)).Append("ms");
        for (var i = 0; i < Math.Min(3, top.Count); i++)
        {
            summary.Append(" | ").Append(top[i].Name)
                .Append(" 跳过=").Append(top[i].SkipPct.ToString("0", CultureInfo.InvariantCulture)).Append('%')
                .Append(" ").Append(top[i].MsPerSec.ToString("0.00", CultureInfo.InvariantCulture)).Append("ms/s");
        }

        Core.Log?.Log(summary.ToString());
        if (detail.Length > 0)
        {
            Core.Log?.Log("[性能诊断] 明细 #" + interval + "：" + detail);
        }

        _intervalIndex = interval;
        _boundary = now;
        _nextReport = now + _reportSeconds;
        FrameMs.Clear();
        _allocAtStart = GC.GetTotalMemory(false);
        _gcAtStart = GC.CollectionCount(0);
        Array.Clear(Calls, 0, Calls.Length);
        Array.Clear(Saves, 0, Saves.Length);
        Array.Clear(Ticks, 0, Ticks.Length);

        if (force)
        {
            Core.Log?.Log("[性能诊断] 已结束");
        }
    }

    private static float Percentile(float fraction)
    {
        if (FrameMs.Count == 0)
        {
            return 0f;
        }

        var sorted = FrameMs.ToArray();
        Array.Sort(sorted);
        var index = (int)((sorted.Length - 1) * fraction);
        return sorted[index];
    }
}
