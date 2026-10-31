namespace XGDragFixer;

/// <summary>
/// Pure pre-filter for a candidate placement. Mirrors the vanilla rules:
/// - GameGridInventory: FullyIntersects(inventoryShape, (b, bo) => b != 1 || bo == 0)
///   only visits cells inside the inventory shape's own bounds, so a value-1 cell
///   beyond the last row/column is not rejected there.
/// - GameGridScrollableInventory: FullyIntersectsRect(width, height, ...) visits the
///   candidate's own bounds, so every value-1 cell must stay inside the grid.
/// - !Intersects(existingItemShape): only equal, non-zero values collide.
/// A candidate rejected here would also be rejected by the vanilla TryInventorySlot
/// before doing any of its generic checks.
/// </summary>
internal static class SlotFit
{
    internal static bool Fits(byte[] mask, int maskWidth, int maskHeight, bool flipped, int orientation,
        int originX, int originY, byte[] occupied, bool[]? blocked, int inventoryWidth, int inventoryHeight,
        bool requireInsideForOnes)
    {
        var rotation = ((orientation % 4) + 4) % 4;

        for (var localY = 0; localY < maskHeight; localY++)
        {
            for (var localX = 0; localX < maskWidth; localX++)
            {
                var value = mask[localY * maskWidth + localX];
                if (value == 0)
                {
                    continue;
                }

                var x = flipped ? maskWidth - 1 - localX : localX;
                var y = localY;

                int transformedX;
                int transformedY;
                switch (rotation)
                {
                    case 1:
                        transformedX = y;
                        transformedY = maskWidth - 1 - x;
                        break;
                    case 2:
                        transformedX = maskWidth - 1 - x;
                        transformedY = maskHeight - 1 - y;
                        break;
                    case 3:
                        transformedX = maskHeight - 1 - y;
                        transformedY = x;
                        break;
                    default:
                        transformedX = x;
                        transformedY = y;
                        break;
                }

                var worldX = originX + transformedX;
                var worldY = originY + transformedY;
                var inside = worldX >= 0 && worldY >= 0 && worldX < inventoryWidth && worldY < inventoryHeight;

                if (value == 1)
                {
                    if (!inside)
                    {
                        if (requireInsideForOnes)
                        {
                            return false;
                        }

                        // Fixed grid inventories never look at cells outside their
                        // own shape bounds, so such a cell cannot reject a candidate.
                        continue;
                    }

                    var index = worldY * inventoryWidth + worldX;
                    if ((blocked != null && blocked[index]) || (occupied[index] & 2) != 0)
                    {
                        return false;
                    }
                }
                else
                {
                    // Values above 1 never collide with the 0/1 shapes the game actually
                    // builds; the outside flag does not block them either.
                    if (!inside)
                    {
                        continue;
                    }

                    if ((occupied[worldY * inventoryWidth + worldX] & (1 << value)) != 0)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}
