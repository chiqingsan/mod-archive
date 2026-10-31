using System;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// Port of the original mod's tooltip cache: the basic and advanced tooltips of
/// an item element are rebuilt at most once per few frames while the pointer
/// hovers the same element.
/// </summary>
internal static class TooltipCache
{
    private sealed class CacheSlot
    {
        internal GameItemElement? Instance;
        internal int UniqueId;
        internal int Frame;
        internal RichText? Text;

        /// <summary>
        /// True when the last prefix call let the vanilla build run. A cache hit
        /// (prefix returned false) must not refresh <see cref="Frame"/> from the
        /// postfix, or the entry would never expire and the tooltip would stay
        /// frozen while the pointer keeps hovering the same item.
        /// </summary>
        internal bool StorePending = true;
    }

    private static readonly CacheSlot BasicSlot = new();
    private static readonly CacheSlot AdvancedSlot = new();

    internal static bool GetTooltipPrefix(GameItemElement __instance, ref RichText? __result)
    {
        return TryGet(__instance, BasicSlot, ref __result);
    }

    internal static void GetTooltipPostfix(GameItemElement __instance, RichText? __result)
    {
        if (!BasicSlot.StorePending)
        {
            return;
        }

        Store(__instance, BasicSlot, __result);
    }

    internal static bool GetAdvancedTooltipPrefix(GameItemElement __instance, ref RichText? __result)
    {
        return TryGet(__instance, AdvancedSlot, ref __result);
    }

    internal static void GetAdvancedTooltipPostfix(GameItemElement __instance, RichText? __result)
    {
        if (!AdvancedSlot.StorePending)
        {
            return;
        }

        Store(__instance, AdvancedSlot, __result);
    }

    internal static void Clear()
    {
        BasicSlot.Instance = null;
        BasicSlot.Text = null;
        BasicSlot.StorePending = true;
        AdvancedSlot.Instance = null;
        AdvancedSlot.Text = null;
        AdvancedSlot.StorePending = true;
    }

    private static bool TryGet(GameItemElement instance, CacheSlot slot, ref RichText? result)
    {
        // Default to "vanilla runs": every early return below lets the build run
        // and the postfix may store it. Only the cache-hit return clears it.
        slot.StorePending = true;

        if (Core.TooltipCacheEnabled?.Value != true || instance == null)
        {
            return true;
        }

        var text = slot.Text;
        if (text == null || slot.Instance == null || !ReferenceEquals(slot.Instance, instance))
        {
            return true;
        }

        var frames = Core.TooltipCacheFrames?.Value ?? 0;
        if (frames <= 0)
        {
            return true;
        }

        if (slot.UniqueId != instance.uniqueId)
        {
            return true;
        }

        var age = Time.frameCount - slot.Frame;
        if (age < 0 || age >= frames)
        {
            return true;
        }

        result = text;
        slot.StorePending = false;
        return false;
    }

    private static void Store(GameItemElement instance, CacheSlot slot, RichText? result)
    {
        if (Core.TooltipCacheEnabled?.Value != true || instance == null || result == null)
        {
            return;
        }

        slot.Instance = instance;
        slot.UniqueId = instance.uniqueId;
        slot.Frame = Time.frameCount;
        slot.Text = result;
    }
}
