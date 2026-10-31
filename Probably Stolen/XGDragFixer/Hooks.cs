using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XGDragFixer;

namespace XGDragFixer;

/// <summary>
/// Harmony entry points. Every method here is referenced from Core.InstallPatches
/// by name; keep the names and signatures stable.
/// </summary>
internal static class Hooks
{
    private static MethodInfo? _onEventReset;
    private static long _endNightStart;
    private static long _machineStart;

    internal static bool StartDragPrefix()
    {
        if (Core.FixGhostDrag?.Value != true)
        {
            PerfDiagnostics.Hit(PerfDiagnostics.PerfId.DragFix);
            return true;
        }

        bool pressed;
        try
        {
            pressed = Input.GetMouseButton(0);
        }
        catch
        {
            PerfDiagnostics.Hit(PerfDiagnostics.PerfId.DragFix);
            return true;
        }

        if (pressed)
        {
            PerfDiagnostics.Hit(PerfDiagnostics.PerfId.DragFix);
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
                PerfDiagnostics.Skip(PerfDiagnostics.PerfId.DragFix);
                return false;
            }

            _onEventReset ??= AccessTools.Method(typeof(ItemMouseDragHandler), "OnEventReset",
                Type.EmptyTypes);
            _onEventReset?.Invoke(handler, null);
        }
        catch (System.Exception ex)
        {
            Core.Log?.Warning($"拖拽状态重置失败: {ex.GetType().Name} {ex.Message}");
        }

        PerfDiagnostics.Skip(PerfDiagnostics.PerfId.DragFix);
        return false;
    }

    internal static bool TryFindSlotPrefix(GameGridInventory __instance, GameItem item,
        bool keepOrientation, ref SlotMarker __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var start = PerfDiagnostics.Begin();
        var result = FastSlotFind.Prefix(__instance, item, keepOrientation, ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.FastSlot, start, !result);
        return result;
    }

    internal static bool ScrollSlotFindPrefix(GameGridScrollableInventory __instance, GameItem item,
        bool keepOrientation, ref SlotMarker __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var start = PerfDiagnostics.Begin();
        var result = FastSlotFind.PrefixScroll(__instance, item, keepOrientation, ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.FastSlotScroll, start, !result);
        return result;
    }

    internal static void ColdStartPrefix(PixelWindow __instance)
    {
        if (!PerfGate.Active)
        {
            return;
        }

        PerfDiagnostics.Hit(PerfDiagnostics.PerfId.ContainerCold);
        ContainerShowFix.ColdStartPrefix(__instance);
    }

    internal static void ShowPostfix()
    {
        LayoutSuppressor.End();
    }

    internal static void SetTitlePrefix()
    {
        if (!PerfGate.Active)
        {
            return;
        }

        ContainerShowFix.SetTitlePrefix();
    }

    internal static void SetTitlePostfix()
    {
        ContainerShowFix.SetTitlePostfix();
    }

    internal static bool ValidatePrefix()
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = ContainerShowFix.ValidatePrefix();
        PerfDiagnostics.End(PerfDiagnostics.PerfId.ValidateSkip, 0L, !result);
        return result;
    }

    internal static bool MarkLayoutPrefix(RectTransform rect)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = LayoutSuppressor.MarkLayoutPrefix(rect);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.LayoutSuppress, 0L, !result);
        return result;
    }

    internal static bool MaskNotifyPrefix(Component mask)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = LayoutSuppressor.MaskNotifyPrefix(mask);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.MaskSuppress, 0L, !result);
        return result;
    }

    [HarmonyPriority(Priority.First)]
    internal static bool BlockTooltipPrefix()
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = HoverPeek.BlockTooltipPrefix();
        PerfDiagnostics.End(PerfDiagnostics.PerfId.PeekBlock, 0L, !result);
        return result;
    }

    internal static bool GetTooltipPrefix(GameItemElement __instance, ref RichText? __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = TooltipCache.GetTooltipPrefix(__instance, ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.TooltipCache, 0L, !result);
        return result;
    }

    internal static void GetTooltipPostfix(GameItemElement __instance, RichText? __result)
    {
        TooltipCache.GetTooltipPostfix(__instance, __result);
    }

    internal static bool GetAdvancedTooltipPrefix(GameItemElement __instance, ref RichText? __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = TooltipCache.GetAdvancedTooltipPrefix(__instance, ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.TooltipCache, 0L, !result);
        return result;
    }

    internal static void GetAdvancedTooltipPostfix(GameItemElement __instance, RichText? __result)
    {
        TooltipCache.GetAdvancedTooltipPostfix(__instance, __result);
    }

    // ---- v1.1.0 performance hooks ----

    internal static bool RaycastPrefix(PointerEventData eventData, List<RaycastResult> raycastResults,
        ref bool __state)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var start = PerfDiagnostics.Begin();
        var result = RaycastFrameCache.Prefix(eventData, raycastResults, ref __state);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.RaycastCache, start, !result);
        return result;
    }

    internal static void RaycastPostfix(PointerEventData eventData, List<RaycastResult> raycastResults,
        bool __state)
    {
        RaycastFrameCache.Postfix(eventData, raycastResults, __state);
    }

    [HarmonyPriority(Priority.LowerThanNormal)]
    internal static bool TooltipIdlePrefix(Vector2 position, Vector2 localTranslation, RichText? richText,
        int maxCharWidth)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = TooltipIdleSkip.Prefix(position, localTranslation, richText, maxCharWidth);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.TooltipIdle, 0L, !result);
        return result;
    }

    internal static void TooltipIdlePostfix(Vector2 position, Vector2 localTranslation, RichText? richText,
        int maxCharWidth)
    {
        TooltipIdleSkip.Postfix(position, localTranslation, richText, maxCharWidth);
    }

    internal static void TooltipHidePostfix()
    {
        TooltipIdleSkip.HidePrefix();
    }

    internal static bool ContextMenuPrefix(ItemContextHandler __instance, ref bool __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = ContextMenuSkip.Prefix(__instance, ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.ContextMenu, 0L, !result);
        return result;
    }

    internal static void ContextMenuPostfix()
    {
        ContextMenuSkip.Postfix();
    }

    private static bool _endNightOptimized;

    internal static void EndNightPrefix()
    {
        // The container tag cache is valid for the whole settlement (it is keyed
        // per item and dropped whenever the game rebuilds modifiedState), and it
        // must run independently of the diagnostic switch.
        ChemTagCache.Begin();

        // The stage breakdown is pure measurement and also runs in the A/B "off"
        // phase, so a settlement that happens while patches are off still yields
        // a cost breakdown.
        NightBreakdown.Begin();

        if (!PerfGate.Active)
        {
            NightValidateBatch.Reset();
            _endNightOptimized = false;
            return;
        }

        _endNightOptimized = true;
        _endNightStart = PerfDiagnostics.Begin();
        NightValidateBatch.Begin();
        AgingBatch.Begin();
    }

    internal static void EndNightPostfix()
    {
        if (_endNightOptimized)
        {
            NightValidateBatch.End();
            AgingBatch.End();
            PerfDiagnostics.End(PerfDiagnostics.PerfId.EndNight, _endNightStart, false);
            _endNightStart = 0L;
            _endNightOptimized = false;
        }

        NightBreakdown.End();
        ChemTagCache.End();
    }

    internal static void NightSimPrefix()
    {
        if (!PerfGate.Active)
        {
            return;
        }

        NightSimQuiet.Begin();
    }

    internal static void NightSimPostfix()
    {
        NightSimQuiet.End();
    }

    internal static bool OnAgePrefix(GameItem item)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = AgingBatch.Prefix(item);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.AgingBatch, 0L, !result);
        return result;
    }

    internal static void MachineActionedPrefix()
    {
        if (!PerfGate.Active)
        {
            return;
        }

        _machineStart = PerfDiagnostics.Begin();
        MachineBatch.Begin();
    }

    internal static void MachineActionedPostfix()
    {
        MachineBatch.End();
        PerfDiagnostics.End(PerfDiagnostics.PerfId.MachineBatch, _machineStart, false);
        _machineStart = 0L;
    }

    internal static bool ModuleEffectPrefix(GameItem item)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = MachineBatch.Prefix(item);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.ModuleEffect, 0L, !result);
        return result;
    }

    internal static bool UpdateModSpritePrefix(GameItemElement __instance, int i, Sprite modSprite,
        Vector2 modPosition, int modOrientation, bool modFlip)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = SpriteGuards.Prefix(__instance, i, modSprite, modPosition, modOrientation, modFlip);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.SpriteGuard, 0L, !result);
        return result;
    }

    internal static bool SlotIndexPrefix(GameGridInventory __instance, GameItem item, ref SlotMarker __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = SlotIndexDirect.PrefixIncomplete(__instance, item, ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.SlotIndex, 0L, !result);
        return result;
    }

    internal static bool ItemSlotIndexPrefix(GameGridInventory __instance, GameItem item, ref SlotMarker __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = SlotIndexDirect.PrefixSlot(__instance, item, ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.SlotIndex, 0L, !result);
        return result;
    }

    internal static bool ScrollSlotIndexPrefix(GameGridScrollableInventory __instance, GameItem item,
        ref SlotMarker __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = SlotIndexDirect.PrefixIncompleteScroll(__instance, item, ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.SlotIndex, 0L, !result);
        return result;
    }

    internal static bool ScrollItemSlotIndexPrefix(GameGridScrollableInventory __instance, GameItem item,
        ref SlotMarker __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = SlotIndexDirect.PrefixSlotScroll(__instance, item, ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.SlotIndex, 0L, !result);
        return result;
    }

    // ---- v1.2.0 store UI / per-frame hooks ----

    internal static bool StoreHoverPrefix()
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var start = PerfDiagnostics.Begin();
        var result = StoreHoverCache.Prefix();
        PerfDiagnostics.End(PerfDiagnostics.PerfId.StoreHover, start, !result);
        return result;
    }

    internal static void StoreHoverPostfix()
    {
        StoreHoverCache.Postfix();
    }

    internal static bool TextFieldFocusPrefix(ref bool __result)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = TextFieldFocusCache.Prefix(ref __result);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.TextFieldFocus, 0L, !result);
        return result;
    }

    internal static void TextFieldFocusPostfix(bool __result)
    {
        TextFieldFocusCache.Postfix(__result);
    }

    internal static bool SizeFitterPrefix(AdvancedContentSizeFitter __instance)
    {
        if (!PerfGate.Active)
        {
            return true;
        }

        var result = SizeFitterGuard.Prefix(__instance);
        PerfDiagnostics.End(PerfDiagnostics.PerfId.SizeFitter, 0L, !result);
        return result;
    }

    // ---- v1.9.1 load-time safety net ----

    internal static Exception? FaultGuardFinalizer(Exception? __exception, MethodBase __originalMethod)
    {
        return StartupFaultGuard.Catch(__exception, __originalMethod);
    }
}
