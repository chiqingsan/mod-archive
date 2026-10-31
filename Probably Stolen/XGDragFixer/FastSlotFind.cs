using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// Reimplementation of TryFindOneValidInventorySlot(GameItem, bool) for the drag
/// path, for both the fixed grid inventory and the scrollable one. Vanilla probes
/// every rotation/flip/position and calls the abstract TryInventorySlot for each
/// candidate; for a nearly full container that means thousands of fully generic
/// checks per mouse move. We keep the exact same search order and validity rules,
/// but pre-filter candidate shapes against a per-inventory occupancy bitmap before
/// invoking TryInventorySlot.
/// </summary>
internal static class FastSlotFind
{
    private sealed class ReferenceComparer : IEqualityComparer<GameInventory>
    {
        internal static readonly ReferenceComparer Instance = new();

        public bool Equals(GameInventory x, GameInventory y) => ReferenceEquals(x, y);

        public int GetHashCode(GameInventory obj) => RuntimeHelpers.GetHashCode(obj);
    }

    private sealed class CacheEntry
    {
        internal long Key;
        internal byte[] Bitmap = Array.Empty<byte>();
    }

    private const int MaxCacheEntries = 8;

    private static readonly FieldInfo? GridItemsField =
        AccessTools.Field(typeof(GameGridInventory), "items");

    private static readonly FieldInfo? ScrollItemsField =
        AccessTools.Field(typeof(GameGridScrollableInventory), "items");

    private static readonly Comparison<GameItem> PositionOrder = ComparePosition;

    private static readonly Dictionary<GameInventory, CacheEntry> BitmapCache =
        new(ReferenceComparer.Instance);

    internal static void Reset()
    {
        BitmapCache.Clear();
    }

    private static int ComparePosition(GameItem x, GameItem y)
    {
        var xs = x.modifiedShape;
        var ys = y.modifiedShape;
        if (xs == null || ys == null)
        {
            return 0;
        }

        if (xs.minX == ys.minX && xs.minY == ys.minY)
        {
            return 0;
        }

        return (xs.minX > ys.minX && xs.minY == ys.minY) || xs.minY > ys.minY ? 1 : -1;
    }

    /// <summary>Prefix for GameGridInventory.TryFindOneValidInventorySlot(GameItem, bool).</summary>
    internal static bool Prefix(GameGridInventory __instance, GameItem item, bool keepOrientation,
        ref SlotMarker __result)
    {
        if (__instance == null || item == null || Core.FastSlotFind?.Value != true)
        {
            return true;
        }

        try
        {
            var items = GridItemsField?.GetValue(__instance) as List<GameItem>;
            var areaShape = __instance.inventoryShape;
            if (items == null || areaShape == null)
            {
                return true;
            }

            var invW = areaShape.width;
            var invH = areaShape.height;
            if (invW <= 0 || invH <= 0)
            {
                return true;
            }

            return Search(__instance, items, invW, invH, areaShape, item, keepOrientation, ref __result);
        }
        catch (Exception ex)
        {
            LogOnce(ex);
            return true;
        }
    }

    /// <summary>Prefix for GameGridScrollableInventory.TryFindOneValidInventorySlot(GameItem, bool).</summary>
    internal static bool PrefixScroll(GameGridScrollableInventory __instance, GameItem item,
        bool keepOrientation, ref SlotMarker __result)
    {
        if (__instance == null || item == null || Core.FastSlotFind?.Value != true)
        {
            return true;
        }

        try
        {
            var items = ScrollItemsField?.GetValue(__instance) as List<GameItem>;
            if (items == null)
            {
                return true;
            }

            var invW = __instance.width;
            var invH = __instance.height;
            if (invW <= 0 || invH <= 0)
            {
                return true;
            }

            return Search(__instance, items, invW, invH, null, item, keepOrientation, ref __result);
        }
        catch (Exception ex)
        {
            LogOnce(ex);
            return true;
        }
    }

    /// <summary>
    /// Returns true when the vanilla implementation must run: either something the
    /// bitmap cannot model was found, or the inventory state changed while scanning.
    /// </summary>
    private static bool Search(GameInventory inventory, List<GameItem> items, int invW, int invH,
        GridShape? areaShape, GameItem item, bool keepOrientation, ref SlotMarker __result)
    {
        var num = item.MaxNumRemove();
        if (num < 1)
        {
            __result = null;
            return false;
        }

        // Vanilla sorts by grid position first; keep the mutation for parity.
        items.Sort(PositionOrder);

        var marker = new SlotMarker
        {
            item = item,
            inventory = inventory,
            itemGridShape = item.modifiedShape
        };

        // Let special items (e.g. wallets) delegate the search to child items.
        for (var i = 0; i < items.Count; i++)
        {
            var other = items[i];
            if (other == null || other == item)
            {
                continue;
            }

            if (!PassesThrough(other, item, inventory, marker))
            {
                continue;
            }

            var delegated = other.TryFindOneValidInventorySlot(item);
            if (delegated != null)
            {
                __result = delegated;
                return false;
            }
        }

        if (!inventory.MayHaveValidInventorySlot(item))
        {
            __result = null;
            return false;
        }

        for (var i = 0; i < items.Count; i++)
        {
            var other = items[i];
            if (other == null || other == item || !item.MayStack(other))
            {
                continue;
            }

            var transfer = Math.Min(num, other.MaxNumAdd());
            if (transfer < 1)
            {
                continue;
            }

            marker.targetItem = other;
            marker.numTransfer = transfer;
            marker.priority = Math.Max(
                inventory.slotPriorityFunc?.Invoke(item, inventory, marker) ?? 0,
                item.slotPriorityFunc?.Invoke(item, inventory, marker) ?? 0);
            __result = marker;
            return false;
        }

        var shape = item.modifiedShape ?? item.shape;
        if (shape == null || shape.GetOutsideBounds() != 0)
        {
            return true;
        }

        var width = shape.width;
        var height = shape.height;
        if (width <= 0 || height <= 0)
        {
            return true;
        }

        var mask = new byte[width * height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var value = shape.GetLocal(x, y);
                if (value >= 32)
                {
                    // A shape value the 8-bit occupancy map cannot represent.
                    return true;
                }

                mask[y * width + x] = value;
            }
        }

        bool[]? blocked = null;
        if (areaShape != null)
        {
            blocked = new bool[invW * invH];
            for (var y = 0; y < invH; y++)
            {
                for (var x = 0; x < invW; x++)
                {
                    // Vanilla rejects a value-1 cell only where inventoryShape == 1;
                    // other non-zero values of the area shape do not block.
                    blocked[y * invW + x] = areaShape.Get(x, y) == 1;
                }
            }
        }

        var bitmap = GetOccupancyBitmap(inventory, item, items, invW, invH);
        if (bitmap == null)
        {
            return true;
        }

        var builder = new GridShapeBuilder(shape);
        var flips = keepOrientation ? 1 : 2;
        var rotations = keepOrientation ? 1 : 4;

        // The scrollable inventory has no area shape: its FullyIntersectsRect check
        // requires every value-1 cell to stay inside the rectangle. The fixed grid
        // iterates its own shape bounds instead, so a value-1 cell beyond the last
        // row/column is never looked at by the vanilla check.
        var requireInsideForOnes = areaShape == null;

        // Same order as vanilla: flip outer loop, rotation inner loop, row, column.
        for (var flipIndex = 0; flipIndex < flips; flipIndex++)
        {
            var flipped = flipIndex != 0;
            for (var index = 0; index < rotations; index++)
            {
                var orientation = 4 - index;
                for (var row = 0; row < invH; row++)
                {
                    for (var col = 0; col < invW; col++)
                    {
                        if (!SlotFit.Fits(mask, width, height, flipped, orientation, col, row,
                                bitmap, blocked, invW, invH, requireInsideForOnes))
                        {
                            continue;
                        }

                        builder.SetTransform(col, row, flipped, orientation);
                        var slot = inventory.TryInventorySlot(item, num, builder);
                        if (slot != null && slot.numTransfer > 0)
                        {
                            __result = slot;
                            return false;
                        }
                    }
                }
            }
        }

        __result = null;
        return false;
    }

    private static bool PassesThrough(GameItem other, GameItem item, GameInventory inventory,
        SlotMarker marker)
    {
        var func = other.slotSearchPassthroughForItemFunc;
        if (func != null && func(item, inventory, marker, other))
        {
            return true;
        }

        var invFunc = inventory.slotSearchPassthroughForItemFunc;
        return invFunc != null && invFunc(item, inventory, marker, other);
    }

    private static byte[]? GetOccupancyBitmap(GameInventory inventory, GameItem item,
        List<GameItem> items, int invW, int invH)
    {
        var key = ComputeCacheKey(items, item, invW, invH);
        if (BitmapCache.TryGetValue(inventory, out var entry) && entry.Key == key &&
            entry.Bitmap.Length == invW * invH)
        {
            return entry.Bitmap;
        }

        var bitmap = new byte[invW * invH];
        for (var i = 0; i < items.Count; i++)
        {
            var other = items[i];
            if (other == null || other == item)
            {
                continue;
            }

            var otherShape = other.modifiedShape;
            if (otherShape == null)
            {
                continue;
            }

            if (!Mark(bitmap, otherShape, invW, invH))
            {
                // A shape value outside the 0/1 range cannot be represented in the
                // bitmap; fall back to the vanilla method for this inventory.
                return null;
            }
        }

        if (BitmapCache.Count >= MaxCacheEntries)
        {
            BitmapCache.Clear();
        }

        BitmapCache[inventory] = new CacheEntry { Key = key, Bitmap = bitmap };
        return bitmap;
    }

    private static long ComputeCacheKey(List<GameItem> items, GameItem item, int invW, int invH)
    {
        // FNV-1a over everything that can change the occupancy of the inventory —
        // including grid positions, since stability alone is not enough: moving an
        // item must invalidate the cached bitmap. O(items) is negligible next to the
        // placement scan it guards.
        var hash = 1469598103934665603UL;
        hash = (hash ^ (ulong)(uint)invW) * 1099511628211UL;
        hash = (hash ^ (ulong)(uint)invH) * 1099511628211UL;
        hash = (hash ^ (ulong)(uint)item.uniqueId) * 1099511628211UL;
        hash = (hash ^ (ulong)(uint)items.Count) * 1099511628211UL;
        for (var i = 0; i < items.Count; i++)
        {
            var other = items[i];
            if (other == null)
            {
                continue;
            }

            hash = (hash ^ (ulong)(uint)other.uniqueId) * 1099511628211UL;
            var otherShape = other.modifiedShape;
            if (otherShape != null)
            {
                hash = (hash ^ (ulong)(uint)otherShape.minX) * 1099511628211UL;
                hash = (hash ^ (ulong)(uint)otherShape.minY) * 1099511628211UL;
                hash = (hash ^ (ulong)(uint)otherShape.width) * 1099511628211UL;
                hash = (hash ^ (ulong)(uint)otherShape.height) * 1099511628211UL;
            }
        }

        return (long)hash;
    }

    private static bool Mark(byte[] bitmap, GridShape shape, int invW, int invH)
    {
        for (var y = shape.minY; y <= shape.maxY; y++)
        {
            for (var x = shape.minX; x <= shape.maxX; x++)
            {
                if (x < 0 || x >= invW || y < 0 || y >= invH)
                {
                    continue;
                }

                var value = shape.Get(x, y);
                if (value == 0)
                {
                    continue;
                }

                if (value >= 32)
                {
                    return false;
                }

                bitmap[y * invW + x] |= (byte)(1 << value);
            }
        }

        return true;
    }

    private static bool _logged;

    private static void LogOnce(Exception ex)
    {
        if (_logged)
        {
            return;
        }

        _logged = true;
        Core.Log?.Warning($"空位搜索快速路径异常，已回退到原版实现: {ex}");
    }
}
