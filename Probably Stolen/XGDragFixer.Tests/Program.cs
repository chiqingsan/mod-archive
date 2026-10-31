using XGDragFixer;

namespace XGDragFixer.Tests;

/// <summary>
/// Offline checks for the fast placement pre-filter.
///
/// The reference implementation below is transcribed from the game's
/// GridShape.ForwardTransform / GridShape.InverseTransform and from the
/// conditions TryInventorySlot applies:
/// - FullyIntersects(inventoryShape, (b, bo) => b != 1 || bo == 0)
/// - !Intersects(existing items, (b, bo) => b > 0 && b == bo)
/// SlotFit must agree with it for every transform/origin combination.
/// </summary>
internal static class Program
{
    private static int _failures;

    private static int Main(string[] args)
    {
        if (Array.IndexOf(args, "--bench") >= 0)
        {
            Benchmark.Run();
            return 0;
        }

        TestForwardInverseRoundTrip();
        TestDifferential();
        TestHandPickedCases();

        if (_failures == 0)
        {
            Console.WriteLine("ALL TESTS PASSED");
            return 0;
        }

        Console.WriteLine($"FAILURES: {_failures}");
        return 1;
    }

    private static void TestForwardInverseRoundTrip()
    {
        var rng = new Random(20251031);
        for (var iteration = 0; iteration < 20000; iteration++)
        {
            var width = rng.Next(1, 6);
            var height = rng.Next(1, 6);
            var flipped = rng.Next(2) == 1;
            var orientation = rng.Next(-8, 9);
            var localX = rng.Next(0, width);
            var localY = rng.Next(0, height);

            var (gx, gy) = Forward(width, height, flipped, orientation, localX, localY);
            var (rx, ry) = Inverse(width, height, flipped, orientation, gx, gy);
            if (rx != localX || ry != localY)
            {
                Fail($"round-trip mismatch w={width} h={height} f={flipped} o={orientation} local=({localX},{localY}) forward=({gx},{gy}) inverse=({rx},{ry})");
            }
        }
    }

    private static void TestDifferential()
    {
        var rng = new Random(424242);
        for (var iteration = 0; iteration < 50000; iteration++)
        {
            var maskWidth = rng.Next(1, 5);
            var maskHeight = rng.Next(1, 5);
            var mask = new byte[maskWidth * maskHeight];
            var hasSpecialValues = rng.Next(4) == 0;
            for (var i = 0; i < mask.Length; i++)
            {
                var roll = rng.Next(100);
                if (roll < 55)
                {
                    mask[i] = 0;
                }
                else if (hasSpecialValues && roll < 70)
                {
                    mask[i] = (byte)(rng.Next(3) + 2);
                }
                else
                {
                    mask[i] = 1;
                }
            }

            var inventoryWidth = rng.Next(2, 11);
            var inventoryHeight = rng.Next(2, 11);
            var blocked = new bool[inventoryWidth * inventoryHeight];
            for (var i = 0; i < blocked.Length; i++)
            {
                blocked[i] = rng.Next(100) < 12;
            }

            var occupied = new byte[inventoryWidth * inventoryHeight];
            var itemCells = new List<Cell>();
            FillRandomItems(rng, inventoryWidth, inventoryHeight, blocked, occupied, itemCells);

            var flipped = rng.Next(2) == 1;
            var orientation = rng.Next(-8, 9);
            var originX = rng.Next(-2, inventoryWidth + 2);
            var originY = rng.Next(-2, inventoryHeight + 2);

            var fast = SlotFit.Fits(mask, maskWidth, maskHeight, flipped, orientation, originX, originY,
                occupied, blocked, inventoryWidth, inventoryHeight, requireInsideForOnes: false);
            var reference = ReferenceFits(mask, maskWidth, maskHeight, flipped, orientation, originX, originY,
                blocked, inventoryWidth, inventoryHeight, itemCells, requireInsideForOnes: false);

            // Scrollable inventories have no area shape: FullyIntersectsRect makes every
            // value-1 cell that leaves the rectangle invalid.
            var fastScroll = SlotFit.Fits(mask, maskWidth, maskHeight, flipped, orientation, originX, originY,
                occupied, null, inventoryWidth, inventoryHeight, requireInsideForOnes: true);
            var referenceScroll = ReferenceFits(mask, maskWidth, maskHeight, flipped, orientation, originX,
                originY, null, inventoryWidth, inventoryHeight, itemCells, requireInsideForOnes: true);

            if (fast != reference || fastScroll != referenceScroll)
            {
                Fail($"differential mismatch iteration={iteration} fast={fast}/{fastScroll} " +
                     $"reference={reference}/{referenceScroll} " +
                     $"mask={maskWidth}x{maskHeight} flipped={flipped} orientation={orientation} " +
                     $"origin=({originX},{originY}) inv={inventoryWidth}x{inventoryHeight}");
            }
        }
    }

    private static void TestHandPickedCases()
    {
        // 1x1 item into empty 2x2 inventory.
        var emptyBlocked = new bool[4];
        var emptyOccupied = new byte[4];
        var oneByOne = new byte[] { 1 };
        for (var originY = -1; originY <= 2; originY++)
        {
            for (var originX = -1; originX <= 2; originX++)
            {
                var inside = originX >= 0 && originY >= 0 && originX < 2 && originY < 2;
                if (SlotFit.Fits(oneByOne, 1, 1, false, 0, originX, originY, emptyOccupied, emptyBlocked,
                        2, 2, requireInsideForOnes: true) != inside)
                {
                    Fail($"hand case 1x1(scrollable) origin=({originX},{originY}) expected {inside}");
                }

                if (!SlotFit.Fits(oneByOne, 1, 1, false, 0, originX, originY, emptyOccupied, emptyBlocked,
                        2, 2, requireInsideForOnes: false))
                {
                    Fail($"hand case 1x1(fixed grid) origin=({originX},{originY}) expected true");
                }
            }
        }

        // Overhanging value-1 cells: the fixed grid inventory only checks cells inside
        // its own shape bounds, so a 2x1 item at the last column is not rejected by the
        // pre-filter; the scrollable inventory rejects it.
        var wide = new byte[] { 1, 1 };
        if (!SlotFit.Fits(wide, 2, 1, false, 0, 1, 0, emptyOccupied, emptyBlocked, 2, 2,
                requireInsideForOnes: false))
        {
            Fail("hand case fixed grid overhang should pass the pre-filter");
        }

        if (SlotFit.Fits(wide, 2, 1, false, 0, 1, 0, emptyOccupied, emptyBlocked, 2, 2,
                requireInsideForOnes: true))
        {
            Fail("hand case scrollable overhang was not rejected");
        }

        // Occupied cell blocks the 1x1 item.
        var occupied = new byte[4];
        occupied[0] |= 2;
        if (SlotFit.Fits(oneByOne, 1, 1, false, 0, 0, 0, occupied, emptyBlocked, 2, 2, false))
        {
            Fail("hand case occupied cell was not rejected");
        }

        // Blocked cell is not placeable.
        var blockedCell = new bool[4];
        blockedCell[3] = true;
        if (SlotFit.Fits(oneByOne, 1, 1, false, 0, 1, 1, emptyOccupied, blockedCell, 2, 2, false))
        {
            Fail("hand case blocked cell was not rejected");
        }

        // 1x2 item rotated: only valid when the inventory is tall enough.
        var tall = new byte[] { 1, 1 };
        var blockedTall = new bool[4];
        var occupiedTall = new byte[4];
        if (!SlotFit.Fits(tall, 1, 2, false, 1, 0, 0, occupiedTall, blockedTall, 2, 2, false))
        {
            Fail("hand case rotated 1x2 should fit into 2x2");
        }

        // Value above 1 ignores blocked inventory cells but still collides with the
        // same value in an existing item.
        var special = new byte[] { 2 };
        var specialBlocked = new bool[4];
        specialBlocked[0] = true;
        if (!SlotFit.Fits(special, 1, 1, false, 0, 0, 0, emptyOccupied, specialBlocked, 2, 2, false))
        {
            Fail("hand case value 2 should ignore blocked cells");
        }

        var specialOccupied = new byte[4];
        specialOccupied[0] |= 1 << 2;
        if (SlotFit.Fits(special, 1, 1, false, 0, 0, 0, specialOccupied, emptyBlocked, 2, 2, false))
        {
            Fail("hand case value 2 should collide with value 2");
        }

        var otherOccupied = new byte[4];
        otherOccupied[0] |= 1 << 1;
        if (!SlotFit.Fits(special, 1, 1, false, 0, 0, 0, otherOccupied, emptyBlocked, 2, 2, false))
        {
            Fail("hand case value 2 should not collide with value 1");
        }
    }

    private readonly record struct Cell(int X, int Y, byte Value);

    private static void FillRandomItems(Random rng, int inventoryWidth, int inventoryHeight, bool[] blocked,
        byte[] occupied, List<Cell> itemCells)
    {
        var attempts = rng.Next(0, 10);
        for (var attempt = 0; attempt < attempts; attempt++)
        {
            var width = rng.Next(1, 4);
            var height = rng.Next(1, 4);
            var originX = rng.Next(0, inventoryWidth);
            var originY = rng.Next(0, inventoryHeight);
            var value = (byte)(rng.Next(6) == 0 ? 2 : 1);
            var cells = new List<Cell>();
            var fits = true;
            for (var y = 0; y < height && fits; y++)
            {
                for (var x = 0; x < width && fits; x++)
                {
                    var gx = originX + x;
                    var gy = originY + y;
                    if (gx >= inventoryWidth || gy >= inventoryHeight || blocked[gy * inventoryWidth + gx] ||
                        occupied[gy * inventoryWidth + gx] != 0)
                    {
                        fits = false;
                        break;
                    }

                    cells.Add(new Cell(gx, gy, value));
                }
            }

            if (!fits)
            {
                continue;
            }

            foreach (var cell in cells)
            {
                occupied[cell.Y * inventoryWidth + cell.X] |= (byte)(1 << cell.Value);
                itemCells.Add(cell);
            }
        }
    }

    private static bool ReferenceFits(byte[] mask, int maskWidth, int maskHeight, bool flipped, int orientation,
        int originX, int originY, bool[]? blocked, int inventoryWidth, int inventoryHeight, List<Cell> itemCells,
        bool requireInsideForOnes)
    {
        var normalized = Normalize(orientation);
        var globalWidth = normalized % 2 != 0 ? maskHeight : maskWidth;
        var globalHeight = normalized % 2 != 0 ? maskWidth : maskHeight;

        // FullyIntersects(inventoryShape, (b, bo) => b != 1 || bo == 0)
        for (var dy = 0; dy < globalHeight; dy++)
        {
            for (var dx = 0; dx < globalWidth; dx++)
            {
                if (CandidateValue(mask, maskWidth, maskHeight, flipped, normalized, dx, dy) != 1)
                {
                    continue;
                }

                var gx = originX + dx;
                var gy = originY + dy;
                if (gx < 0 || gy < 0 || gx >= inventoryWidth || gy >= inventoryHeight)
                {
                    if (requireInsideForOnes)
                    {
                        return false;
                    }

                    continue;
                }

                if (blocked != null && blocked[gy * inventoryWidth + gx])
                {
                    return false;
                }
            }
        }

        // !Intersects(existingItems, (b, bo) => b > 0 && b == bo)
        foreach (var cell in itemCells)
        {
            var dx = cell.X - originX;
            var dy = cell.Y - originY;
            if (dx < 0 || dy < 0 || dx >= globalWidth || dy >= globalHeight)
            {
                continue;
            }

            var value = CandidateValue(mask, maskWidth, maskHeight, flipped, normalized, dx, dy);
            if (value > 0 && value == cell.Value)
            {
                return false;
            }
        }

        return true;
    }

    private static byte CandidateValue(byte[] mask, int width, int height, bool flipped, int orientation, int dx,
        int dy)
    {
        var (localX, localY) = Inverse(width, height, flipped, orientation, dx, dy);
        if (localX < 0 || localY < 0 || localX >= width || localY >= height)
        {
            return 0;
        }

        return mask[localY * width + localX];
    }

    // Transcription of GridShape.ForwardTransform (offsets only, no minX/minY).
    private static (int X, int Y) Forward(int width, int height, bool flipped, int orientation, int localX,
        int localY)
    {
        if (flipped)
        {
            localX = width - 1 - localX;
        }

        var x = localX;
        var y = localY;
        switch (Normalize(orientation))
        {
            case 1:
                (x, y) = (y, width - 1 - x);
                break;
            case 2:
                (x, y) = (width - 1 - x, height - 1 - y);
                break;
            case 3:
                (x, y) = (height - 1 - y, x);
                break;
        }

        return (x, y);
    }

    // Transcription of GridShape.InverseTransform (offsets only, no minX/minY).
    private static (int X, int Y) Inverse(int width, int height, bool flipped, int orientation, int x, int y)
    {
        var normalized = Normalize(orientation);
        int rotationWidth;
        int rotationHeight;
        if (normalized % 2 != 0)
        {
            rotationWidth = height;
            rotationHeight = width;
        }
        else
        {
            rotationWidth = width;
            rotationHeight = height;
        }

        switch (normalized)
        {
            case 1:
                (x, y) = (rotationHeight - 1 - y, x);
                break;
            case 2:
                (x, y) = (rotationWidth - 1 - x, rotationHeight - 1 - y);
                break;
            case 3:
                (x, y) = (y, rotationWidth - 1 - x);
                break;
        }

        if (flipped)
        {
            x = width - 1 - x;
        }

        return (x, y);
    }

    private static int Normalize(int orientation)
    {
        return ((orientation % 4) + 4) % 4;
    }

    private static void Fail(string message)
    {
        _failures++;
        if (_failures <= 20)
        {
            Console.WriteLine("FAIL: " + message);
        }
    }
}
