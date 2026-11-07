using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// Fix4 空位搜索（行为与游戏内实现对齐）
/// (XG_Mod/Patch.cs: TryFindOneValidInventorySlot_Prefix + MarkOccupied + FitsMaskAt).
///
/// It replaces the vanilla empty-slot search of <see cref="GameGridInventory"/> by rebuilding the current occupancy: the occupied cells of every item are baked into a
/// bitmap once per call, the candidate positions of the dragged item are pre-filtered
/// against that bitmap and only the surviving candidates are passed to the vanilla
/// <c>TryInventorySlot</c> (which still performs the authoritative checks).
///
/// Scrollable inventories are intentionally left to the vanilla search, exactly like
/// the reference mod does.
/// </summary>
internal static class Fix4
{
    private static FieldInfo? _itemsField;
    private static bool _itemsFieldResolved;

    internal static bool TryFindOneValidInventorySlotPrefix(
        GameInventory __instance,
        GameItem item,
        bool keepOrientation,
        ref SlotMarker? __result,
        bool scrollable)
    {
        try
        {
            if (__instance == null || item == null)
            {
                __result = null;
                return false;
            }

            if (Core.Fix4?.Value != true)
            {
                return true;
            }

            // The reference mod only accelerates grid inventories; scrollable ones
            // keep using the vanilla search.
            if (scrollable)
            {
                return true;
            }

            var inventory = __instance as GameGridInventory;
            if (inventory == null)
            {
                return true;
            }

            var num = item.MaxNumRemove();
            if (num < 1)
            {
                __result = null;
                return false;
            }

            var inventoryShape = inventory.inventoryShape;
            var inventoryWidth = inventoryShape?.width ?? 0;
            var inventoryHeight = inventoryShape?.height ?? 0;

            var items = GetItems(__instance);
            if (items == null)
            {
                return true;
            }

            // Keep vanilla's spatial order and delegate nested-container searches
            // to vanilla, including its callback order and target selection.
            if (inventory.slotSearchPassthroughForItemFunc != null
                || items.Exists(other => other.slotSearchPassthroughForItemFunc != null))
            {
                Diagnostics.Count("找位穿透回原版");
                return true;
            }

            items.Sort((x, y) =>
            {
                if (x.modifiedShape.minX == y.modifiedShape.minX && x.modifiedShape.minY == y.modifiedShape.minY)
                {
                    return 0;
                }
                return ((x.modifiedShape.minX > y.modifiedShape.minX && x.modifiedShape.minY == y.modifiedShape.minY)
                    || x.modifiedShape.minY > y.modifiedShape.minY) ? 1 : -1;
            });

            if (!inventory.MayHaveValidInventorySlot(item))
            {
                Diagnostics.Count("找位准入拒绝");
                __result = null;
                return false;
            }

            // 1) stack onto an existing stack after the vanilla admission checks.
            var childItems = items;
            if (childItems != null)
            {
                for (var i = 0; i < childItems.Count; i++)
                {
                    var other = childItems[i];
                    if (other == null || ReferenceEquals(other, item) || !item.MayStack(other))
                    {
                        continue;
                    }

                    var transfer = Math.Min(num, other.MaxNumAdd());
                    if (transfer >= 1)
                    {
                        __result = new SlotMarker
                        {
                            item = item,
                            inventory = __instance,
                            itemGridShape = item.modifiedShape,
                            targetItem = other,
                            numTransfer = transfer
                        };
                        __result.priority = Math.Max(
                            inventory.slotPriorityFunc?.Invoke(item, inventory, __result) ?? 0,
                            item.slotPriorityFunc?.Invoke(item, inventory, __result) ?? 0);
                        return false;
                    }
                }
            }

            // 2) empty-slot search, pre-filtered by an occupancy bitmap.
            var shape = item.modifiedShape ?? item.shape;
            if (shape == null)
            {
                __result = null;
                return false;
            }

            var width = shape.width;
            var height = shape.height;
            if (width <= 0 || height <= 0)
            {
                __result = null;
                return false;
            }

            var mask = new byte[width * height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    mask[y * width + x] = shape.GetLocal(x, y);
                }
            }

            var bitmap = inventoryWidth > 0 && inventoryHeight > 0
                ? new byte[inventoryWidth * inventoryHeight]
                : null;

            if (bitmap != null && items != null)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    var current = items[i];
                    if (current == null || ReferenceEquals(current, item))
                    {
                        continue;
                    }

                    var modifiedShape = current.modifiedShape;
                    if (modifiedShape != null)
                    {
                        MarkOccupied(bitmap, modifiedShape, inventoryWidth, inventoryHeight);
                    }
                }
            }

            var flipCount = keepOrientation ? 1 : 2;
            var orientationCount = keepOrientation ? 1 : 4;
            var builder = new GridShapeBuilder(shape);
            for (var flipIndex = 0; flipIndex < flipCount; flipIndex++)
            {
                var flipped = flipIndex != 0;
                for (var orientationIndex = 0; orientationIndex < orientationCount; orientationIndex++)
                {
                    var orientation = 4 - orientationIndex;
                    // Vanilla tests every origin inside the grid. Blank shape
                    // padding may extend outside; only occupied cells constrain fit.
                    for (var row = 0; row < inventoryHeight; row++)
                    {
                        for (var column = 0; column < inventoryWidth; column++)
                        {
                            if (bitmap != null
                                && !FitsMaskAt(mask, width, height, flipped, orientation, column, row, bitmap,
                                    inventoryWidth, inventoryHeight))
                            {
                                continue;
                            }

                            var candidateShape = builder.SetTransform(column, row, flipped, orientation).Build();
                            if (candidateShape == null)
                            {
                                continue;
                            }

                            var marker = inventory.TryInventorySlot(item, num, new Vector2(float.NaN, 0f), candidateShape,
                                null);
                            if (marker != null && marker.numTransfer > 0)
                            {
                                __result = marker;
                                return false;
                            }
                        }
                    }
                }
            }

            __result = null;
            return false;
        }
        catch (Exception ex)
        {
            Diagnostics.Count("找位异常回退");
            // The reference mod lets the vanilla search run when its shortcut fails.
            Core.Log?.Warning($"[Fix4] 找位加速异常，已回退原版: {ex.GetType().Name} {ex.Message}");
            return true;
        }
    }

    private static List<GameItem>? GetItems(GameInventory inventory)
    {
        if (!_itemsFieldResolved)
        {
            _itemsFieldResolved = true;
            _itemsField = AccessTools.Field(typeof(GameGridInventory), "items");
            if (_itemsField == null)
            {
                Core.Log?.Warning("[Fix4] 未找到 GameGridInventory.items，找位加速放弃");
            }
        }

        try
        {
            return _itemsField?.GetValue(inventory) as List<GameItem>;
        }
        catch
        {
            return null;
        }
    }

    private static void MarkOccupied(byte[] bitmap, GridShape shape, int inventoryWidth, int inventoryHeight)
    {
        var minX = shape.minX;
        var minY = shape.minY;
        var maxX = shape.maxX;
        var maxY = shape.maxY;
        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                if (x < 0 || x >= inventoryWidth || y < 0 || y >= inventoryHeight)
                {
                    continue;
                }

                var value = shape.Get(x, y);
                if (value != 0)
                {
                    bitmap[x * inventoryHeight + y] = (byte)(bitmap[x * inventoryHeight + y] | (1 << (value & 0x1F)));
                }
            }
        }
    }

    private static bool FitsMaskAt(
        byte[] mask,
        int maskWidth,
        int maskHeight,
        bool flipped,
        int orientation,
        int column,
        int row,
        byte[] bitmap,
        int inventoryWidth,
        int inventoryHeight)
    {
        var normalized = (orientation % 4 + 4) % 4;
        for (var y = 0; y < maskHeight; y++)
        {
            for (var x = 0; x < maskWidth; x++)
            {
                var value = mask[y * maskWidth + x];
                if (value == 0)
                {
                    continue;
                }

                var localX = flipped ? maskWidth - x - 1 : x;
                var localY = y;
                int transformedX;
                int transformedY;
                switch (normalized)
                {
                    case 1:
                        transformedX = localY;
                        transformedY = maskWidth - localX - 1;
                        break;
                    case 2:
                        transformedX = maskWidth - localX - 1;
                        transformedY = maskHeight - localY - 1;
                        break;
                    case 3:
                        transformedX = maskHeight - localY - 1;
                        transformedY = localX;
                        break;
                    default:
                        transformedX = localX;
                        transformedY = localY;
                        break;
                }

                var targetX = column + transformedX;
                var targetY = row + transformedY;
                if (targetX < 0 || targetY < 0 || targetX >= inventoryWidth || targetY >= inventoryHeight)
                {
                    return false;
                }

                if ((bitmap[targetX * inventoryHeight + targetY] & (1 << value)) != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
