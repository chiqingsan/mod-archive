using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace XGDragFixer;

/// <summary>
/// Harmony entry points. 只保留已验证过的补丁入口（对齐游戏 0.46D）
/// actually did: skip when the condition holds, otherwise run vanilla.
/// </summary>
internal static class Hooks
{
    private static MethodInfo? _onEventReset;
    private static bool _onEventResetMissing;

    internal static bool StartDragPrefix()
    {
        if (Core.FixGhostDrag?.Value != true)
        {
            return true;
        }

        bool pressed;
        try
        {
            pressed = Input.GetMouseButton(0);
        }
        catch
        {
            return true;
        }

        if (pressed)
        {
            return true;
        }

        // A drag was started without the left button being held down (usually after
        // a click/double click reset the input state). Reset it like the vanilla
        // handler would on mouse-up instead of entering a stuck drag.
        try
        {
            var handler = ItemMouseDragHandler.current;
            if (handler == null)
            {
                return false;
            }

            if (_onEventReset == null && !_onEventResetMissing)
            {
                _onEventReset = AccessTools.Method(typeof(ItemMouseDragHandler), "OnEventReset", Type.EmptyTypes);
                if (_onEventReset == null)
                {
                    _onEventResetMissing = true;
                    Core.Log?.Warning("[兼容] 未找到 ItemMouseDragHandler.OnEventReset，拖拽沾手修复退化为放行原版");
                }
            }

            if (_onEventReset == null)
            {
                return true;
            }

            _onEventReset.Invoke(handler, null);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"拖拽状态重置失败: {ex.GetType().Name} {ex.Message}");
        }

        return false;
    }

    internal static void ColdStartPrefix(PixelWindow __instance, out (int Depth, long Started) __state)
    {
        __state = (LayoutSuppressor.Depth, Diagnostics.Begin());
        ContainerShowFix.ColdStartPrefix(__instance);
    }

    internal static Exception? ShowFinalizer(Exception? __exception, (int Depth, long Started) __state)
    {
        while (LayoutSuppressor.Depth > __state.Depth)
        {
            LayoutSuppressor.End();
        }
        Diagnostics.End(Diagnostics.Probe.WindowShow, __state.Started, __exception);
        return __exception;
    }

    internal static bool MarkLayoutPrefix(RectTransform rect)
    {
        return LayoutSuppressor.MarkLayoutPrefix(rect);
    }

    [HarmonyPriority(Priority.First)]
    internal static bool BlockTooltipPrefix()
    {
        return HoverPeek.BlockTooltipPrefix();
    }

    internal static bool GetTooltipPrefix(GameItemElement __instance, ref RichText? __result)
    {
        return TooltipCache.GetTooltipPrefix(__instance, ref __result);
    }

    internal static void GetTooltipPostfix(GameItemElement __instance, RichText? __result)
    {
        TooltipCache.GetTooltipPostfix(__instance, __result);
    }

    internal static bool GetAdvancedTooltipPrefix(GameItemElement __instance, ref RichText? __result)
    {
        return TooltipCache.GetAdvancedTooltipPrefix(__instance, ref __result);
    }

    internal static void GetAdvancedTooltipPostfix(GameItemElement __instance, RichText? __result)
    {
        TooltipCache.GetAdvancedTooltipPostfix(__instance, __result);
    }

    internal static bool Fix4Prefix(GameGridInventory __instance, GameItem item, bool keepOrientation,
        ref SlotMarker? __result, out long __state)
    {
        __state = Diagnostics.Begin();
        return Fix4.TryFindOneValidInventorySlotPrefix(__instance, item, keepOrientation, ref __result, false);
    }

    internal static bool Fix4ScrollablePrefix(GameGridScrollableInventory __instance, GameItem item,
        bool keepOrientation, ref SlotMarker? __result, out long __state)
    {
        __state = Diagnostics.Begin();
        return Fix4.TryFindOneValidInventorySlotPrefix(__instance, item, keepOrientation, ref __result, true);
    }

    internal static Exception? FindSlotFinalizer(Exception? __exception, long __state)
    {
        Diagnostics.End(Diagnostics.Probe.FindSlot, __state, __exception);
        return __exception;
    }

}
