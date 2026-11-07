using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace PurifierTower;

/// <summary>
/// 临时诊断（仅 debug.txt 存在时启用）：
/// Item Manager（创意工坊 mod asteroid_item_manager）在「生成 -> ObserveItem -> SetAmount ->
/// AddDirectToWeightedTable」链路里会抛 NRE，但它只打印 ex.Message，看不到栈。
/// 这里在这些游戏方法与它的 ObserveItem 上挂 finalizer，把完整异常栈写进日志。
/// 发布版不安装任何补丁（本类不再带 [HarmonyPatch] 特性，避免被加载器无条件 PatchAll）。
/// </summary>
internal static class ProbePatches
{
    private static Harmony? harmony;

    public static void Install()
    {
        if (!DebugTools.Enabled)
        {
            return;
        }

        if (harmony != null)
        {
            return;
        }

        try
        {
            harmony = new Harmony("chiqingsan.PurifierTower.trace");
            TryPatch("ItemManager.ObserveItem", FindObserveItem(), nameof(ObserveItemTrace));
            TryPatch(nameof(PlayerStore.AddDirectToWeightedTable),
                typeof(PlayerStore).GetMethod(nameof(PlayerStore.AddDirectToWeightedTable)), nameof(AddToWeightedTableTrace));
            TryPatch(nameof(GameItem.SetUnitCount),
                typeof(GameItem).GetMethod(nameof(GameItem.SetUnitCount)), nameof(SetUnitCountTrace));
            TryPatch(nameof(GameItem.ValidateShapeState),
                typeof(GameItem).GetMethod(nameof(GameItem.ValidateShapeState)), nameof(ValidateShapeStateTrace));
            TryPatch(nameof(GraphUtils.TryAcceptAllOrDestroy),
                typeof(GraphUtils).GetMethod(nameof(GraphUtils.TryAcceptAllOrDestroy)), nameof(AcceptOrDestroyTrace));

            // 紫外线灯插入卡顿（m03076②）计时：定位耗时落在「插入校验」还是「窗口刷新」。
            TryTimingPatch("GameSlotInventory.UncheckedAccept",
                typeof(GameSlotInventory).GetMethod(nameof(GameSlotInventory.UncheckedAccept), new[] { typeof(GameItem) }),
                nameof(OnAcceptPrefix), nameof(OnAcceptPostfix));
            TryTimingPatch("OverlayHandler.UpdateInventoryStateUIImmediately",
                typeof(OverlayHandler).GetMethod(nameof(OverlayHandler.UpdateInventoryStateUIImmediately)),
                nameof(OnRefreshPrefix), nameof(OnRefreshPostfix));
            TryTimingPatch("GameSlotInventory.Validate",
                typeof(GameSlotInventory).GetMethod(nameof(GameSlotInventory.Validate)),
                nameof(OnValidatePrefix), nameof(OnValidatePostfix));
        }
        catch (Exception error)
        {
            Debug.LogError("[PurifierTower][trace] install failed:\n" + error);
        }
    }

    public static void Uninstall()
    {
        try
        {
            harmony?.UnpatchAll(harmony.Id);
        }
        catch (Exception error)
        {
            Debug.LogWarning("[PurifierTower][trace] uninstall failed: " + error.Message);
        }
        finally
        {
            harmony = null;
        }
    }

    private static void TryPatch(string label, MethodInfo? target, string finalizerName)
    {
        if (target == null)
        {
            Debug.Log($"[PurifierTower][trace] {label} not found, skip");
            return;
        }

        MethodInfo finalizer = typeof(ProbePatches).GetMethod(finalizerName, BindingFlags.Static | BindingFlags.NonPublic)
                               ?? throw new MissingMethodException(nameof(ProbePatches), finalizerName);
        harmony!.Patch(target, finalizer: new HarmonyMethod(finalizer));
        Debug.Log($"[PurifierTower][trace] patched {target.DeclaringType?.FullName}.{target.Name}");
    }

    /// <summary>给方法挂 Prefix/Postfix 计时（只记录超过阈值的调用，避免刷屏）。</summary>
    private static void TryTimingPatch(string label, MethodInfo? target, string? prefixName, string? postfixName)
    {
        if (target == null)
        {
            Debug.Log($"[PurifierTower][trace] {label} not found, skip");
            return;
        }

        HarmonyMethod? prefix = prefixName == null ? null : HarmonyMethodOf(prefixName);
        HarmonyMethod? postfix = postfixName == null ? null : HarmonyMethodOf(postfixName);
        harmony!.Patch(target, prefix: prefix, postfix: postfix);
        Debug.Log($"[PurifierTower][trace] timing patched {label}");
    }

    private static HarmonyMethod HarmonyMethodOf(string name)
    {
        MethodInfo method = typeof(ProbePatches).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)
                            ?? throw new MissingMethodException(nameof(ProbePatches), name);
        return new HarmonyMethod(method);
    }

    // ================= 计时探针（仅 -pt-debug） =================

    private const int SlowMs = 15;

    private static readonly System.Diagnostics.Stopwatch AcceptWatch = new();
    private static readonly System.Diagnostics.Stopwatch RefreshWatch = new();
    private static readonly System.Diagnostics.Stopwatch ValidateWatch = new();
    private static string acceptLabel = string.Empty;
    private static int acceptDepth;

    private static void OnAcceptPrefix(GameItem item)
    {
        if (acceptDepth++ > 0)
        {
            return;
        }

        acceptLabel = item?.identifier ?? "?";
        AcceptWatch.Restart();
    }

    private static void OnAcceptPostfix()
    {
        if (acceptDepth <= 0 || --acceptDepth > 0)
        {
            return;
        }

        AcceptWatch.Stop();
        if (AcceptWatch.ElapsedMilliseconds >= SlowMs)
        {
            Debug.Log($"[PurifierTower][trace] UncheckedAccept({acceptLabel}) took {AcceptWatch.ElapsedMilliseconds} ms");
        }
    }

    private static void OnRefreshPrefix() => RefreshWatch.Restart();

    private static void OnRefreshPostfix()
    {
        RefreshWatch.Stop();
        if (RefreshWatch.ElapsedMilliseconds >= SlowMs)
        {
            Debug.Log($"[PurifierTower][trace] UpdateInventoryStateUIImmediately took {RefreshWatch.ElapsedMilliseconds} ms"
                      + $" (visible windows={WindowsHandler.current?.visibleWindows?.Count ?? 0})");
        }
    }

    private static void OnValidatePrefix() => ValidateWatch.Restart();

    private static void OnValidatePostfix()
    {
        ValidateWatch.Stop();
        if (ValidateWatch.ElapsedMilliseconds >= SlowMs)
        {
            Debug.Log($"[PurifierTower][trace] GameSlotInventory.Validate took {ValidateWatch.ElapsedMilliseconds} ms");
        }
    }

    private static MethodInfo? FindObserveItem()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            string name = assembly.GetName().Name ?? string.Empty;
            if (name.IndexOf("ItemManager", StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch
            {
                continue;
            }

            foreach (Type type in types)
            {
                MethodInfo? method = type.GetMethod("ObserveItem",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (method != null)
                {
                    return method;
                }
            }
        }

        return null;
    }

    private static Exception? Trace(string label, Exception? exception)
    {
        if (exception != null)
        {
            Debug.LogError($"[PurifierTower][trace] {label} threw:\n{exception}");
        }

        return exception;
    }

    // Harmony finalizer 必须是 "Exception Finalizer(Exception __exception)" 形式。
    private static Exception? ObserveItemTrace(Exception? __exception) => Trace("ItemManager.ObserveItem", __exception);

    private static Exception? AddToWeightedTableTrace(Exception? __exception) =>
        Trace(nameof(PlayerStore.AddDirectToWeightedTable), __exception);

    private static Exception? SetUnitCountTrace(Exception? __exception) => Trace(nameof(GameItem.SetUnitCount), __exception);

    private static Exception? ValidateShapeStateTrace(Exception? __exception) =>
        Trace(nameof(GameItem.ValidateShapeState), __exception);

    private static Exception? AcceptOrDestroyTrace(Exception? __exception) =>
        Trace(nameof(GraphUtils.TryAcceptAllOrDestroy), __exception);
}
