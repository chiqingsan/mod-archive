using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace XGDragFixer;

/// <summary>
/// GameGridInventory / GameGridScrollableInventory expose childItems as a fresh
/// copy of the private items list, then GetIncompleteItemInventorySlot /
/// GetItemInventorySlot call childItems.IndexOf(item) — allocating and copying a
/// list every time a slot marker is built (slot events, drag validation, overhauls).
/// Scan the backing list directly instead.
/// </summary>
internal static class SlotIndexDirect
{
    private static readonly FieldInfo? GridItemsField =
        AccessTools.Field(typeof(GameGridInventory), "items");

    private static readonly FieldInfo? ScrollItemsField =
        AccessTools.Field(typeof(GameGridScrollableInventory), "items");

    internal static bool Enabled => Core.SlotIndexDirect?.Value == true;

    internal static bool PrefixIncomplete(GameGridInventory __instance, GameItem item, ref SlotMarker __result)
    {
        if (!Enabled || __instance == null || item == null)
        {
            return true;
        }

        try
        {
            __result = new SlotMarker
            {
                item = item,
                inventory = __instance,
                itemGridShape = item.modifiedShape,
                index = IndexOf(__instance, item)
            };
            return false;
        }
        catch
        {
            return true;
        }
    }

    internal static bool PrefixSlot(GameGridInventory __instance, GameItem item, ref SlotMarker __result)
    {
        if (!Enabled || __instance == null || item == null)
        {
            return true;
        }

        try
        {
            SlotMarker marker = __instance.TryInventorySlot(item, item.modifiedShape);
            if (marker == null)
            {
                return true;
            }

            marker.index = IndexOf(__instance, item);
            __result = marker;
            return false;
        }
        catch
        {
            return true;
        }
    }

    internal static bool PrefixIncompleteScroll(GameGridScrollableInventory __instance, GameItem item, ref SlotMarker __result)
    {
        if (!Enabled || __instance == null || item == null)
        {
            return true;
        }

        try
        {
            __result = new SlotMarker
            {
                item = item,
                inventory = __instance,
                itemGridShape = item.modifiedShape,
                index = IndexOf(__instance, item)
            };
            return false;
        }
        catch
        {
            return true;
        }
    }

    internal static bool PrefixSlotScroll(GameGridScrollableInventory __instance, GameItem item, ref SlotMarker __result)
    {
        if (!Enabled || __instance == null || item == null)
        {
            return true;
        }

        try
        {
            SlotMarker marker = __instance.TryInventorySlot(item, item.modifiedShape);
            if (marker == null)
            {
                return true;
            }

            marker.index = IndexOf(__instance, item);
            __result = marker;
            return false;
        }
        catch
        {
            return true;
        }
    }

    private static int IndexOf(GameInventory inventory, GameItem item)
    {
        var field = inventory is GameGridScrollableInventory ? ScrollItemsField : GridItemsField;
        if (field?.GetValue(inventory) is not List<GameItem> items)
        {
            return -1;
        }

        var comparer = EqualityComparer<GameItem>.Default;
        for (var i = 0; i < items.Count; i++)
        {
            if (comparer.Equals(items[i], item))
            {
                return i;
            }
        }

        return -1;
    }
}
