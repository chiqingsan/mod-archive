using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// Load-time safety net.
///
/// When a save is loaded while the game window is in the background, some purely
/// visual objects can be a frame behind the state that owns them and the vanilla
/// per-frame update paths start throwing NullReferenceExceptions. The game logs every
/// exception and aborts the rest of that frame's initialisation, so a transient race
/// turns into a container that never opens / a store that never finishes building.
///
/// This adds a finalizer to the handful of noisy update methods: a reference-check
/// exception raised by them is counted, logged once and swallowed so the update chain
/// can finish. Real logic errors (anything that is not a null/ missing reference) are
/// still thrown as usual, and the whole guard can be turned off in the settings.
/// </summary>
internal static class StartupFaultGuard
{
    private static readonly (Type Type, string Name, Type[] Args)[] Targets =
    {
        (typeof(GameGridInventory), nameof(GameGridInventory.Validate), Type.EmptyTypes),
        (typeof(GameGridScrollableInventory), nameof(GameGridScrollableInventory.Validate), Type.EmptyTypes),
        (typeof(GameSlotInventory), nameof(GameSlotInventory.Validate), Type.EmptyTypes),
        (typeof(PixelWindow), nameof(PixelWindow.Validate), Type.EmptyTypes),
        (typeof(EmporiumEntry), nameof(EmporiumEntry.Validate), new[] { typeof(bool) }),
        (typeof(GameItem), nameof(GameItem.EnableTag), new[] { typeof(string), typeof(bool) }),
        (typeof(GameItem), nameof(GameItem.ValidateShapeState), Type.EmptyTypes),
        (typeof(GameItem), nameof(GameItem.SetGameItemType), new[] { typeof(string) }),
        (typeof(WaterHelper), "UpdateContainerGoodType", new[] { typeof(GameItem) }),
    };

    private static readonly Dictionary<string, int> Counts = new();
    private static readonly HashSet<string> Reported = new();
    private static float _nextSummary;

    internal static bool Installed { get; private set; }

    internal static bool Active => Core.StartupFaultGuard?.Value == true;

    internal static int Swallowed { get; private set; }

    internal static void Install(Harmony harmony)
    {
        var ok = 0;
        var missing = 0;

        foreach (var target in Targets)
        {
            var original = AccessTools.Method(target.Type, target.Name, target.Args);
            if (original == null)
            {
                missing++;
                continue;
            }

            try
            {
                harmony.Patch(original,
                    finalizer: new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(Hooks.FaultGuardFinalizer))));
                ok++;
            }
            catch (Exception ex)
            {
                missing++;
                Core.Log?.Warning($"界面未就绪兜底未能挂接 {target.Type.Name}.{target.Name}: {ex.Message}");
            }
        }

        Installed = ok > 0;
        Core.Log?.Log(missing == 0
            ? $"[稳定性] 界面未就绪异常兜底已挂接 {ok} 个方法"
            : $"[稳定性] 界面未就绪异常兜底已挂接 {ok} 个方法，缺失 {missing} 个");
    }

    /// <summary>Called from the Harmony finalizer of every guarded method.</summary>
    internal static Exception? Catch(Exception? exception, MethodBase? method)
    {
        if (exception == null || !Active)
        {
            return exception;
        }

        if (exception is not NullReferenceException && exception is not MissingReferenceException)
        {
            return exception;
        }

        try
        {
            var key = method == null ? "unknown" : $"{method.DeclaringType?.Name}.{method.Name}";
            Swallowed++;
            Counts.TryGetValue(key, out var count);
            Counts[key] = count + 1;
            if (Reported.Add(key))
            {
                Core.Log?.Warning($"[稳定性] 已忽略一次读档期界面未就绪异常：{key}");
            }
        }
        catch
        {
            // The guard must never throw itself.
        }

        return null;
    }

    internal static void Tick()
    {
        if (Swallowed == 0 || !Active)
        {
            return;
        }

        var now = Time.realtimeSinceStartup;
        if (now < _nextSummary)
        {
            return;
        }

        _nextSummary = now + 30f;
        var builder = new StringBuilder();
        foreach (var pair in Counts)
        {
            builder.Append(pair.Key).Append('=').Append(pair.Value).Append(' ');
        }

        Core.Log?.Log($"[稳定性] 已忽略读档期界面未就绪异常 {Swallowed} 次：{builder}");
    }

    internal static void Reset()
    {
        Counts.Clear();
        Reported.Clear();
        Swallowed = 0;
        _nextSummary = 0f;
    }
}
