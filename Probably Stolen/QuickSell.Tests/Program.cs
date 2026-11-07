using QuickSell;

int passed = 0;
void Check(bool condition, string label)
{
    if (!condition) throw new InvalidOperationException(label);
    Console.WriteLine("PASS " + label); passed++;
}

List<List<SelectionPlanner.Option>> Groups(params (long Price, int Space)[] values)
    => values.Select((value, index) => new List<SelectionPlanner.Option> { new(index, 1, value.Price, value.Space) }).ToList();

SelectionPlanner Plan(List<List<SelectionPlanner.Option>> values, long budget, bool space = false, bool single = false, bool variety = false, string[]? types = null)
{
    var planner = new SelectionPlanner();
    foreach (int step in planner.Build(values, budget, space, single, variety, types ?? Enumerable.Range(0, values.Count).Select(index => index.ToString()).ToArray())) { }
    return planner;
}

var combo = Plan(Groups((70, 1), (60, 1), (40, 1)), 100);
NativeContracts.CheckParenting();
Check(true, "native AddParent/RemoveParent detached-object contract (console runtime)");
Check(combo.Result.Sum(option => option.Price) == 100, "70/60/40 chooses 60+40 for budget 100");
Check(Plan(Groups((5, 1)), 0).Result.Count == 0, "zero remaining budget");
Check(Plan(Groups((105, 1)), 100).Result.Count == 0 && Plan(Groups((105, 1)), 105).Result.Count == 1, "over-budget allowance is a strict bound");
Check(Plan(Groups((20, 1), (80, 1), (60, 1)), 100, single: true).Result.Single().Price == 80, "single item limit");
var varietyPlan = Plan(Groups((30, 1), (30, 1), (20, 1)), 100, variety: true, types: new[] { "A", "A", "B" });
Check(varietyPlan.Result.Select(option => new[] { "A", "A", "B" }[option.Item]).Distinct().Count() == varietyPlan.Result.Count, "variety cannot repeat a type");
var samePriceVariety = Plan(Groups((30, 1), (30, 1), (20, 1)), 50, variety: true, types: new[] { "A", "B", "A" });
Check(samePriceVariety.Result.Sum(option => option.Price) == 50 && samePriceVariety.Result.Select(option => new[] { "A", "B", "A" }[option.Item]).Distinct().Count() == 2,
    "same-price different types remain available for variety selection");
Check(SelectionRules.RiskRank(true, false, true, "正常选货", "正常选货") == -1
    && SelectionRules.RiskRank(true, true, false, "正常选货", "不自动上货") == -1
    && SelectionRules.RiskRank(true, true, false, "正常选货", "低优先级") == 1
    && SelectionRules.RiskRank(false, false, true, "不自动上货", "不自动上货") == 0,
    "security and combined risk rules use the strictest applicable policy");
Check(SelectionRules.Available(0, true) == 0 && SelectionRules.Available(1, true) == 0
    && SelectionRules.Available(10, true) == 9 && SelectionRules.Available(10, false) == 10,
    "reserve-one quantity boundaries");
Check(SelectionRules.HasSource(true, 0) && SelectionRules.HasSource(false, 1) && !SelectionRules.HasSource(false, 0),
    "main inventory alone permits restocking; disabling it requires a supply container");
Check(SelectionRules.IsSourceCandidate(true, false) && !SelectionRules.IsSourceCandidate(true, true)
    && SelectionRules.IsSourceCandidate(false, true) && SelectionRules.IsSourceCandidate(false, false),
    "main inventory excludes items with content windows while selected supply containers retain their rules");
Check(SelectionRules.IsSourceCandidate(true, true, true) && !SelectionRules.IsSourceCandidate(true, true, false)
    && SelectionRules.IsSourceCandidate(false, true, false) && SelectionRules.IsSourceCandidate(true, false, false),
    "packaging containers are scanned from the main inventory while ordinary containers are still skipped");
Check(SelectionRules.RetryAfterEmptyContainer(1, true, 0, 8)
    && !SelectionRules.RetryAfterEmptyContainer(0, true, 0, 8)
    && !SelectionRules.RetryAfterEmptyContainer(1, false, 0, 8)
    && !SelectionRules.RetryAfterEmptyContainer(1, true, 7, 8),
    "newly emptied containers trigger a bounded retry only after progress");
Check(Plan(Groups((100, 1), (40, 8), (40, 8)), 100, space: true).Result.Sum(option => option.Space) == 16, "space priority");
var choices = new List<List<SelectionPlanner.Option>>
{
    new() { new(0, 1, 30, 1), new(0, 3, 90, 4) }, new() { new(1, 1, 10, 1) }
};
var partial = Plan(choices, 100);
Check(partial.Result.Select(option => option.Item).Distinct().Count() == partial.Result.Count && partial.Result.Sum(option => option.Price) == 100, "one quantity choice per physical stack");
var random = new Random(1729);
bool exactSmall = true;
for (int attempt = 0; attempt < 100; attempt++)
{
    var prices = Enumerable.Range(0, 8).Select(_ => random.Next(1, 90)).ToArray();
    int limit = random.Next(20, 180), best = 0;
    for (int mask = 0; mask < 1 << prices.Length; mask++)
    {
        int sum = 0;
        for (int i = 0; i < prices.Length; i++) if ((mask & 1 << i) != 0) sum += prices[i];
        if (sum <= limit) best = Math.Max(best, sum);
    }
    var result = Plan(Groups(prices.Select(value => ((long)value, 1)).ToArray()), limit);
    exactSmall &= result.Result.Sum(option => option.Price) == best;
}
Check(exactSmall, "100 small samples match exhaustive budget solutions");
foreach (int size in new[] { 100, 1000, 10000 })
{
    var groups = Groups(Enumerable.Range(0, size).Select(index => ((long)(index % 97 + 1), index % 12 + 1)).ToArray());
    long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
    var timer = System.Diagnostics.Stopwatch.StartNew();
    var result = Plan(groups, 900);
    timer.Stop();
    long allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
    Check(result.Transitions <= SelectionPlanner.TransitionLimit + 1 && result.Result.Sum(option => option.Price) <= 900 && allocated < 8 * 1024 * 1024,
        $"{size} candidates respect computation, money and 8 MiB allocation limits ({timer.ElapsedMilliseconds}ms, {allocated / 1024} KiB allocated; console total, not a native frame measurement)");
}
bool orientationBounds = true;
foreach (int orientation in new[] { 0, 1, 2, 3 })
foreach (bool flipped in new[] { false, true })
{
    // Use the actual game's shape type: sprite dimensions stay 1x4 while
    // the rotated inventory footprint must become 4x1 at quarter turns.
    var shape = new GridShapeBuilder(1, 4).SetTransform(3, 5, flipped, orientation);
    int expectedWidth = orientation % 2 == 0 ? 1 : 4;
    int expectedHeight = orientation % 2 == 0 ? 4 : 1;
    orientationBounds &= shape.globalWidth == expectedWidth && shape.globalHeight == expectedHeight
        && shape.minX == 3 && shape.minY == 5 && shape.maxX - shape.minX + 1 == expectedWidth && shape.maxY - shape.minY + 1 == expectedHeight;
}
Check(orientationBounds, "native item footprints: four rotations, both flips, nonzero inventory offset");
bool dragBounds = true;
var storedShape = new GridShapeBuilder(1, 4).SetTransform(3, 5, false, 0).Build();
foreach (int orientation in new[] { 0, 1, 2, 3 })
foreach (bool flipped in new[] { false, true })
{
    var previewShape = new GridShapeBuilder(storedShape).SetTransform(3, 5, flipped, orientation).Build();
    var preview = HintBounds.Calculate(previewShape, 16, true);
    var stored = HintBounds.Calculate(storedShape, 16, false);
    dragBounds &= preview.X == 0 && preview.Y == 0
        && preview.Width == (orientation % 2 == 0 ? 16 : 64) && preview.Height == (orientation % 2 == 0 ? 64 : 16)
        && stored == (48, -80, 16, 64);
}
Check(dragBounds, "drag preview ignores inventory offset, uses live rotation/flip, and restores stored bounds");
Check(ShortcutKeys.TryParse("LeftControl+L", out var panelKeys), "panel shortcut parses Ctrl+L");
bool ShortcutPressed(UnityEngine.KeyCode[] held, params UnityEngine.KeyCode[] down)
    => ShortcutKeys.Pressed(panelKeys, key => held.Contains(key), key => down.Contains(key));
Check(ShortcutPressed(new[] { UnityEngine.KeyCode.LeftControl, UnityEngine.KeyCode.L }, UnityEngine.KeyCode.L)
    && ShortcutPressed(new[] { UnityEngine.KeyCode.RightControl, UnityEngine.KeyCode.L }, UnityEngine.KeyCode.L)
    && ShortcutPressed(new[] { UnityEngine.KeyCode.LeftControl, UnityEngine.KeyCode.L }, UnityEngine.KeyCode.LeftControl),
    "panel shortcut accepts either Ctrl and either chord press order");
Check(!ShortcutPressed(new[] { UnityEngine.KeyCode.LeftControl, UnityEngine.KeyCode.L })
    && !ShortcutPressed(new[] { UnityEngine.KeyCode.L }, UnityEngine.KeyCode.L)
    && !ShortcutPressed(new[] { UnityEngine.KeyCode.LeftControl }, UnityEngine.KeyCode.LeftControl),
    "panel shortcut does not repeat while held or fire for an incomplete chord");
Check(ShortcutKeys.TryParse("N", out var highlightKeys)
    && ShortcutKeys.Held(highlightKeys, key => key == UnityEngine.KeyCode.N)
    && !ShortcutKeys.Held(highlightKeys, _ => false)
    && ShortcutKeys.TryParse("", out var disabledKeys) && !ShortcutKeys.Pressed(disabledKeys, _ => true, _ => true)
    && !ShortcutKeys.TryParse("Mouse0", out _) && !ShortcutKeys.TryParse("NotAKey", out _),
    "hold highlight remains independent; cleared and invalid shortcuts do not trigger");
var fallbackGroups = Groups((10, 1), (30, 2), (20, 3));
var fallbackSelected = new List<SelectionPlanner.Option> { fallbackGroups[0][0] };
SelectionPlanner.AppendAlternatives(fallbackGroups, fallbackSelected, new HashSet<int> { fallbackGroups[0][0].Item }, 0, false);
Check(fallbackSelected.Select(option => option.Price).SequenceEqual(new long[] { 10, 30, 20 }),
    "fallback candidates continue from remaining items after a rolled-back move, highest price first");
var spaceFallback = new List<SelectionPlanner.Option>();
SelectionPlanner.AppendAlternatives(fallbackGroups, spaceFallback, new HashSet<int>(), 0, true);
Check(spaceFallback.Select(option => option.Space).SequenceEqual(new[] { 3, 2, 1 }), "space priority fallback orders remaining candidates by footprint");
var cappedFallback = new List<SelectionPlanner.Option>();
SelectionPlanner.AppendAlternatives(Groups(Enumerable.Range(0, 40).Select(index => ((long)(80 - index), 1)).ToArray()), cappedFallback, new HashSet<int>(), 0, false);
Check(cappedFallback.Count == 33, "fallback candidate batch stays bounded");
MenuCellKind NativeCell(int x, int y) => x == 0 && y < 7 ? MenuCellKind.Text : MenuCellKind.Empty;
Check(MenuStructure.IsNativeRow(1, 7, NativeCell), "native 1-column 7-row context menu is recognized");
Check(!MenuStructure.IsNativeRow(7, 1, NativeCell) && !MenuStructure.IsNativeRow(1, 6, NativeCell),
    "context menu with a different size is not mistaken for the native layout");
Check(!MenuStructure.IsNativeRow(1, 7, (x, y) => y == 3 ? MenuCellKind.Other : MenuCellKind.Text),
    "native layout detection requires text buttons only");
Check(MenuStructure.TryLocateWrappedRow(1, 2,
        (x, y) => y == 0 ? MenuCellKind.Grid : MenuCellKind.Text,
        (x, y) => y == 0,
        out int wrappedX, out int wrappedY) && wrappedX == 0 && wrappedY == 0,
    "nested native menu wrapped by another mod is located by its container cell");
Check(!MenuStructure.TryLocateWrappedRow(1, 2, (x, y) => MenuCellKind.Text, (x, y) => false, out _, out _),
    "unknown context menu structure is rejected instead of being taken over");
Console.WriteLine($"{passed} checks passed.");
