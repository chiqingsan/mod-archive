using System.Diagnostics;

namespace XGDragFixer.Tests;

/// <summary>
/// Offline workload benchmark for the placement pre-filter (no game assembly required).
///
/// Vanilla work is simulated from the decompiled conditions: for every candidate
/// orientation/position the game walks the existing items and does a bounding-box reject,
/// then compares overlapping cells one by one; only then it calls TryInventorySlot.
/// The fast path replaces that with one occupancy bitmap (built once per inventory)
/// plus a few bit tests per candidate, and only calls the full placement routine for
/// candidates that survive the pre-filter. The search stops at the first accepted
/// candidate, exactly like the game.
///
/// The final section converts measured per-search costs into per-frame estimates using
/// the call counts visible in the decompiled code (drag hover: 1 search per frame while
/// hovering a container; drop: EndDrag tries up to 40 inventories).
/// </summary>
internal static class Benchmark
{
    private const int ItemMaskWidth = 3;
    private const int ItemMaskHeight = 2;
    private const int Repeats = 200;
    private const double FrameBudgetMs = 1000.0 / 60.0;

    internal static void Run()
    {
        Console.WriteLine("Placement pre-filter workload benchmark (simulated vanilla checks, no game assembly)");
        Console.WriteLine($"item {ItemMaskWidth}x{ItemMaskHeight}, {Repeats} searches per scenario");
        Console.WriteLine();

        var scenarios = new[]
        {
            new Scenario("fixed 20x20, 95% full (no free space)", 20, 20, 0.95, RequireInsideForOnes: false),
            new Scenario("fixed 20x30, 95% full (no free space)", 20, 30, 0.95, RequireInsideForOnes: false),
            new Scenario("fixed 12x12, 80% full", 12, 12, 0.80, RequireInsideForOnes: false),
            new Scenario("fixed 20x20, 50% full (free space exists)", 20, 20, 0.50, RequireInsideForOnes: false),
            new Scenario("scrollable 8x60, 95% full", 8, 60, 0.95, RequireInsideForOnes: true),
        };

        var results = new List<Result>();
        foreach (var scenario in scenarios)
        {
            results.Add(RunScenario(scenario));
        }

        Console.WriteLine();
        Console.WriteLine("== Estimated gameplay impact (60 fps = 16.7 ms frame) ==");
        Console.WriteLine("| scenario | per search | 1 hover search / frame | 40 searches at drop |");
        Console.WriteLine("| --- | --- | --- | --- |");
        foreach (var result in results)
        {
            var savedPerSearch = result.VanillaMs - result.FastMs;
            var savedPerFrame = savedPerSearch;
            var savedPerDrop = savedPerSearch * 40;
            Console.WriteLine(
                $"| {result.Scenario.Name} | {savedPerSearch:0.000} ms | {savedPerFrame:0.00} ms/frame ({savedPerFrame / FrameBudgetMs * 100:0.0}% of frame) | {savedPerDrop:0.0} ms once |");
        }

        Console.WriteLine();
        Console.WriteLine("Notes:");
        Console.WriteLine("- The vanilla column is a cost model transcribed from the decompiled");
        Console.WriteLine("  TryInventorySlot conditions (bbox reject + overlapping-cell compare).");
        Console.WriteLine("  It does not include Unity-side allocations/layout, so this compares search");
        Console.WriteLine("  CPU work only, not real frame time.");
        Console.WriteLine("- The fast column does not include the occupancy bitmap build cost: in the game");
        Console.WriteLine("  the bitmap is rebuilt once per inventory change and reused by every search.");
        Console.WriteLine("- 1 search/frame = ItemMouseDragHandler.Drag hovering a container; 40 searches = ");
        Console.WriteLine("  the upper bound EndDrag performs when dropping an item near containers.");
    }

    private readonly record struct Scenario(string Name, int Width, int Height, double FillRatio, bool RequireInsideForOnes);

    private readonly record struct Item(int MinX, int MaxX, int MinY, int MaxY);

    private readonly record struct Found(int OriginX, int OriginY, bool Flipped, int Orientation);

    private sealed class SearchResult
    {
        internal long Candidates;
        internal long BboxChecks;
        internal long CellChecks;
        internal long FullChecks;
        internal long Searches;
        internal Found? Found;
        internal double Ms;
    }

    private sealed record Result(Scenario Scenario, double VanillaMs, double FastMs, bool FoundMatch);

    private static Result RunScenario(Scenario scenario)
    {
        var random = new Random(12345);
        var items = BuildItems(random, scenario, out var occupied);
        var blocked = scenario.RequireInsideForOnes ? null : new bool[scenario.Width * scenario.Height];
        var mask = new byte[ItemMaskWidth * ItemMaskHeight];
        Array.Fill(mask, (byte)1);

        // Warm up the JIT.
        SearchVanilla(items, occupied, mask, scenario, 1);
        SearchFast(items, occupied, blocked, mask, scenario, 1);

        var vanilla = SearchVanilla(items, occupied, mask, scenario, Repeats);
        var fast = SearchFast(items, occupied, blocked, mask, scenario, Repeats);

        Console.WriteLine($"== {scenario.Name} ==");
        Console.WriteLine($"items: {items.Count}");
        Console.WriteLine(
            $"candidates evaluated/search: {vanilla.Candidates / Math.Max(1, vanilla.Searches)} vs {fast.Candidates / Math.Max(1, fast.Searches)}");
        Console.WriteLine(
            $"vanilla: {vanilla.Ms / Repeats,7:0.000} ms/search  ({vanilla.BboxChecks / Math.Max(1, vanilla.Searches),7} bbox tests + {vanilla.CellChecks / Math.Max(1, vanilla.Searches),7} cell compares)");
        Console.WriteLine(
            $"fast   : {fast.Ms / Repeats,7:0.000} ms/search  ({fast.CellChecks / Math.Max(1, fast.Searches),7} mask-cell bit tests, {fast.FullChecks,5} full placements)");
        if (fast.Ms > 0)
        {
            Console.WriteLine($"speedup: {vanilla.Ms / fast.Ms:0.0}x");
        }

        var match = Equals(vanilla.Found, fast.Found);
        Console.WriteLine($"first slot: vanilla {Format(vanilla.Found)}, fast {Format(fast.Found)} (match: {match})");
        Console.WriteLine();
        return new Result(scenario, vanilla.Ms / Repeats, fast.Ms / Repeats, match);
    }

    private static string Format(Found? found)
    {
        return found == null
            ? "none"
            : $"({found.Value.OriginX},{found.Value.OriginY}) flip={found.Value.Flipped} rot={found.Value.Orientation}";
    }

    private static List<Item> BuildItems(Random random, Scenario scenario, out byte[] occupied)
    {
        var width = scenario.Width;
        var height = scenario.Height;
        occupied = new byte[width * height];
        var items = new List<Item>();
        var target = (int)(width * height * scenario.FillRatio);
        var filled = 0;

        while (filled < target)
        {
            var w = random.Next(1, 4);
            var h = random.Next(1, 4);
            if (w > width || h > height)
            {
                continue;
            }

            var x = random.Next(0, width - w + 1);
            var y = random.Next(0, height - h + 1);
            var fits = true;
            for (var dy = 0; dy < h && fits; dy++)
            {
                for (var dx = 0; dx < w && fits; dx++)
                {
                    if (occupied[(y + dy) * width + x + dx] != 0)
                    {
                        fits = false;
                        break;
                    }
                }
            }

            if (!fits)
            {
                continue;
            }

            for (var dy = 0; dy < h; dy++)
            {
                for (var dx = 0; dx < w; dx++)
                {
                    occupied[(y + dy) * width + x + dx] = 2;
                }
            }

            items.Add(new Item(x, x + w - 1, y, y + h - 1));
            filled += w * h;
        }

        return items;
    }

    private static SearchResult SearchVanilla(List<Item> items, byte[] occupied, byte[] mask, Scenario scenario,
        int repeats = 1)
    {
        var stopwatch = repeats > 0 ? Stopwatch.StartNew() : null;
        var result = new SearchResult();
        for (var repeat = 0; repeat < repeats && result.Found == null; repeat++)
        {
            result.Searches++;
            for (var flip = 0; flip < 2 && result.Found == null; flip++)
            {
                var flipped = flip != 0;
                for (var rotation = 0; rotation < 4 && result.Found == null; rotation++)
                {
                    var orientation = 4 - rotation;
                    var globalWidth = orientation % 2 != 0 ? ItemMaskHeight : ItemMaskWidth;
                    var globalHeight = orientation % 2 != 0 ? ItemMaskWidth : ItemMaskHeight;

                    for (var originY = 0; originY <= scenario.Height - globalHeight && result.Found == null; originY++)
                    {
                        for (var originX = 0;
                             originX <= scenario.Width - globalWidth && result.Found == null;
                             originX++)
                        {
                            result.Candidates++;
                            var fits = true;
                            foreach (var other in items)
                            {
                                result.BboxChecks++;
                                if (other.MaxX < originX || other.MinX > originX + globalWidth - 1 ||
                                    other.MaxY < originY || other.MinY > originY + globalHeight - 1)
                                {
                                    continue;
                                }

                                for (var gy = other.MinY; gy <= other.MaxY && fits; gy++)
                                {
                                    var dy = gy - originY;
                                    if (dy < 0 || dy >= globalHeight)
                                    {
                                        continue;
                                    }

                                    for (var gx = other.MinX; gx <= other.MaxX; gx++)
                                    {
                                        var dx = gx - originX;
                                        if (dx < 0 || dx >= globalWidth)
                                        {
                                            continue;
                                        }

                                        result.CellChecks++;
                                        var value = CandidateValue(mask, flipped, orientation, dx, dy);
                                        var outside = scenario.RequireInsideForOnes &&
                                                      (originX + dx < 0 || originX + dx >= scenario.Width ||
                                                       originY + dy < 0 || originY + dy >= scenario.Height);
                                        if (value > 0 && scenario.RequireInsideForOnes && value == 1 && outside)
                                        {
                                            fits = false;
                                            break;
                                        }

                                        if (value > 0 &&
                                            !outside &&
                                            (occupied[gy * scenario.Width + gx] & (1 << value)) != 0)
                                        {
                                            fits = false;
                                            break;
                                        }
                                    }
                                }

                                if (!fits)
                                {
                                    break;
                                }
                            }

                            if (fits)
                            {
                                result.Found = new Found(originX, originY, flipped, orientation);
                            }
                        }
                    }
                }
            }
        }

        stopwatch?.Stop();
        result.Ms = stopwatch?.Elapsed.TotalMilliseconds ?? 0;
        return result;
    }

    private static SearchResult SearchFast(List<Item> items, byte[] occupied, bool[]? blocked, byte[] mask,
        Scenario scenario, int repeats = 1)
    {
        var stopwatch = repeats > 0 ? Stopwatch.StartNew() : null;
        var result = new SearchResult();
        for (var repeat = 0; repeat < repeats && result.Found == null; repeat++)
        {
            result.Searches++;
            for (var flip = 0; flip < 2 && result.Found == null; flip++)
            {
                var flipped = flip != 0;
                for (var rotation = 0; rotation < 4 && result.Found == null; rotation++)
                {
                    var orientation = 4 - rotation;
                    var globalWidth = orientation % 2 != 0 ? ItemMaskHeight : ItemMaskWidth;
                    var globalHeight = orientation % 2 != 0 ? ItemMaskWidth : ItemMaskHeight;

                    for (var originY = 0; originY <= scenario.Height - globalHeight && result.Found == null; originY++)
                    {
                        for (var originX = 0;
                             originX <= scenario.Width - globalWidth && result.Found == null;
                             originX++)
                        {
                            result.Candidates++;
                            result.CellChecks += ItemMaskWidth * ItemMaskHeight;
                            if (!SlotFit.Fits(mask, ItemMaskWidth, ItemMaskHeight, flipped, orientation, originX,
                                    originY, occupied, blocked, scenario.Width, scenario.Height,
                                    requireInsideForOnes: scenario.RequireInsideForOnes))
                            {
                                continue;
                            }

                            // Surviving candidates still go through the real TryInventorySlot; this
                            // model treats a passed pre-filter as the accepted first slot.
                            result.FullChecks++;
                            result.Found = new Found(originX, originY, flipped, orientation);
                        }
                    }
                }
            }
        }

        stopwatch?.Stop();
        result.Ms = stopwatch?.Elapsed.TotalMilliseconds ?? 0;
        return result;
    }

    private static byte CandidateValue(byte[] mask, bool flipped, int orientation, int dx, int dy)
    {
        var normalized = ((orientation % 4) + 4) % 4;
        var (x, y) = Inverse(ItemMaskWidth, ItemMaskHeight, flipped, normalized, dx, dy);
        if (x < 0 || y < 0 || x >= ItemMaskWidth || y >= ItemMaskHeight)
        {
            return 0;
        }

        return mask[y * ItemMaskWidth + x];
    }

    private static (int X, int Y) Inverse(int width, int height, bool flipped, int orientation, int x, int y)
    {
        int rotationWidth;
        int rotationHeight;
        if (orientation % 2 != 0)
        {
            rotationWidth = height;
            rotationHeight = width;
        }
        else
        {
            rotationWidth = width;
            rotationHeight = height;
        }

        switch (orientation)
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
}
