using System.Reflection;
using System.Runtime.Serialization;

namespace XGDragFixer.Tests;

// Exercise the built mod's managed logic without starting Unity or constructing UI.
internal static class RuntimeRegressionTests
{
    private const BindingFlags Static = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

    internal static void Run()
    {
        TestTooltipCache();
        TestOccupancy();
        TestSearchAfterMove();
        TestSearchRules();
        TestSearchPaddedEdges();
        TestShowFailure();
        TestModuleFailure();
        TestVanillaTempStatReset();
        TestQueriesDisabled();
        TestDiagnostics();
        Console.WriteLine("Runtime regressions passed (tooltip, occupancy, moved-item search, admission, stacking order/priority, passthrough, padded edges, exception cleanup, disabled queries, optional diagnostics).");
    }

    private static Type ModType(string name) => typeof(Core).Assembly.GetType("XGDragFixer." + name, true)!;

    private static object? Call(string type, string method, params object?[] args) =>
        ModType(type).GetMethod(method, Static)!.Invoke(null, args);

    private static void Set(string type, string field, object value) =>
        ModType(type).GetField(field, Static)!.SetValue(null, value);

    private static T Empty<T>() => (T)FormatterServices.GetUninitializedObject(typeof(T));

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static void Config(string key, bool value)
    {
        var entry = Activator.CreateInstance(typeof(ConfigEntry<bool>), BindingFlags.Instance | BindingFlags.NonPublic,
            null, new object[] { value }, null)!;
        Set("Core", "<" + key + ">k__BackingField", entry);
    }

    private static void ConfigInt(string key, int value)
    {
        var entry = Activator.CreateInstance(typeof(ConfigEntry<int>), BindingFlags.Instance | BindingFlags.NonPublic,
            null, new object[] { value }, null)!;
        Set("Core", "<" + key + ">k__BackingField", entry);
    }

    private static void SetClock(int frame)
    {
        var field = ModType("TooltipCache").GetField("Clock", Static)!;
        field.SetValue(null, (Func<int>)(() => frame));
    }

    private static void TestTooltipCache()
    {
        Config("TooltipCacheEnabled", true);
        ConfigInt("TooltipCacheFrames", 30);
        SetClock(100);

        var element = Empty<GameItemElement>();
        var other = Empty<GameItemElement>();
        var text = Empty<RichText>();

        // First call: cache miss, vanilla builds, the postfix stores frame 100.
        Check((bool)Call("Hooks", "GetTooltipPrefix", element, null)!, "First call must let vanilla build the tooltip.");
        Call("Hooks", "GetTooltipPostfix", element, text);

        // Inside the window the cached text is reused and the postfix must not
        // refresh the stored frame (that would freeze the tooltip forever).
        SetClock(129);
        Check(!(bool)Call("Hooks", "GetTooltipPrefix", element, null)!, "Cache hit expected inside the frame window.");
        Call("Hooks", "GetTooltipPostfix", element, text);

        // Age reaches the configured lifetime: the entry must expire.
        SetClock(130);
        Check((bool)Call("Hooks", "GetTooltipPrefix", element, null)!, "Cache must expire after TooltipCacheFrames.");

        // Another element never reuses the cached text.
        SetClock(101);
        Check((bool)Call("Hooks", "GetTooltipPrefix", other, null)!, "Another element must not use the cached tooltip.");
    }

    private static GameItemElement Item(GridShape shape)
    {
        var item = Empty<GameItemElement>();
        item.modifiedShape = shape;
        item.unitCount = 1;
        return item;
    }

    private static T Inventory<T>(GridShape shape, params GameItem[] items) where T : GameGridInventory
    {
        var inventory = Empty<T>();
        typeof(GameGridInventory).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(inventory, items.ToList());
        typeof(GameGridInventory).GetField("<inventoryShape>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(inventory, shape);
        return inventory;
    }

    private static void TestOccupancy()
    {
        Config("GridOccupancyShortcut", true);
        for (var width = 1; width <= 3; width++)
        for (var height = 1; height <= 3; height++)
        {
            var cells = width * height;
            for (var mask = 0; mask < (1 << cells); mask++)
            {
                var items = new List<GameItem>();
                for (var cell = 0; cell < cells; cell++)
                    if ((mask & (1 << cell)) != 0)
                        items.Add(Item(new GridShapeBuilder(1, 1).SetDataFill(1).SetPosition(cell % width, cell / width)));
                var inventory = Inventory<GameGridInventory>(new GridShapeBuilder(width, height), items.ToArray());
                object?[] args = { inventory, false, null };
                Check(!(bool)Call("RefQueries", "HasFreeCellPrefix", args)!, "Valid grid should use the occupancy shortcut.");
                Check((bool)args[1]! == inventory.HasFreeCell(), "Occupancy must agree with vanilla, including edge cells.");
            }
        }
        var rotated = Item(new GridShapeBuilder(1, 2).SetDataFill(1).SetOrientation(1));
        var full = Inventory<GameGridInventory>(new GridShapeBuilder(2, 1), rotated);
        object?[] rotatedArgs = { full, true, null };
        Check(!(bool)Call("RefQueries", "HasFreeCellPrefix", rotatedArgs)! && !(bool)rotatedArgs[1]!,
            "Rotated items must cover their last row and column.");
    }

    // Keep the real search and bitmap logic; substitute only Unity's slot/UI boundary.
    private sealed class SearchInventory : GameGridInventory
    {
        private SearchInventory() : base(1, 1) { }
        internal bool Reject;
        public override bool MayHaveValidInventorySlot(GameItem item) => !Reject;

        public override SlotMarker TryInventorySlot(GameItem item, int amount, UnityEngine.Vector2 position,
            GridShape shape, TagSystem state)
        {
            return new SlotMarker { item = item, inventory = this, itemGridShape = shape,
                numTransfer = !shape.FullyIntersects(inventoryShape, (b, other) => b != 1 || other == 0)
                    || childItems.Any(other => shape.Intersects(other.modifiedShape)) ? 0 : 1 };
        }
    }

    private static void TestSearchAfterMove()
    {
        Config("Fix4", true);
        var blocker = Item(new GridShapeBuilder(1, 1).SetDataFill(1));
        var inventory = Inventory<SearchInventory>(new GridShapeBuilder(3, 1), blocker);
        var incoming = Item(new GridShapeBuilder(2, 1).SetDataFill(1));
        object?[] first = { inventory, incoming, true, null, false };
        Check(!(bool)Call("Fix4", "TryFindOneValidInventorySlotPrefix", first)! && first[3] is SlotMarker,
            "First search must find the right-hand space.");
        blocker.modifiedShape = new GridShapeBuilder(1, 1).SetDataFill(1).SetPosition(2, 0);
        object?[] moved = { inventory, incoming, true, null, false };
        Check(!(bool)Call("Fix4", "TryFindOneValidInventorySlotPrefix", moved)! && moved[3] is SlotMarker marker
            && marker.itemGridShape.minX == 0, "Moving a blocker must expose the newly free left-hand space.");
    }

    private static void TestSearchRules()
    {
        Config("Fix4", true);
        var incoming = Item(new GridShapeBuilder(1, 1).SetDataFill(1));
        var right = Item(new GridShapeBuilder(1, 1).SetDataFill(1).SetPosition(1, 0));
        var left = Item(new GridShapeBuilder(1, 1).SetDataFill(1));
        incoming.mayStackItemFunc = (_, _) => true;
        right.mayStackItemFunc = (_, _) => true;
        left.mayStackItemFunc = (_, _) => true;
        var inventory = Inventory<SearchInventory>(new GridShapeBuilder(2, 1), right, left);
        inventory.slotPriorityFunc = (_, _, _) => 4;
        incoming.slotPriorityFunc = (_, _, _) => 9;

        inventory.Reject = true;
        object?[] denied = { inventory, incoming, true, null, false };
        Check(!(bool)Call("Fix4", "TryFindOneValidInventorySlotPrefix", denied)! && denied[3] == null,
            "Rejected inventories must not return a stacking slot.");
        Check(inventory.TryFindOneValidInventorySlot(incoming, true) == null,
            "Vanilla also rejects the same stacking request.");

        inventory.Reject = false;
        // Reverse the stored order again: both searches must choose the left stack.
        typeof(GameGridInventory).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(inventory, new List<GameItem> { right, left });
        object?[] allowed = { inventory, incoming, true, null, false };
        Check(!(bool)Call("Fix4", "TryFindOneValidInventorySlotPrefix", allowed)!, "Stacking shortcut should run.");
        var marker = (SlotMarker)allowed[3]!;
        var vanilla = inventory.TryFindOneValidInventorySlot(incoming, true);
        Check(ReferenceEquals(marker.targetItem, left) && ReferenceEquals(marker.targetItem, vanilla.targetItem)
            && marker.priority == 9 && marker.priority == vanilla.priority && marker.numTransfer == vanilla.numTransfer,
            "Stack target, priority and amount must match vanilla.");

        // Either passthrough callback must leave the whole search to vanilla,
        // before admission rejects the outer inventory.
        inventory.Reject = true;
        for (var source = 0; source < 2; source++)
        {
            var calls = 0;
            Func<GameItem, GameInventory, SlotMarker, GameItem, bool> callback = (_, _, _, _) => { calls++; return false; };
            inventory.slotSearchPassthroughForItemFunc = source == 0 ? callback : null;
            left.slotSearchPassthroughForItemFunc = source == 1 ? callback : null;
            object?[] passthrough = { inventory, incoming, true, null, false };
            Check((bool)Call("Fix4", "TryFindOneValidInventorySlotPrefix", passthrough)! && calls == 0,
                "Passthrough inventories must run the original search without evaluating callbacks twice.");
            inventory.TryFindOneValidInventorySlot(incoming, true);
            Check(calls > 0, "Vanilla must evaluate passthrough before checking outer admission.");
        }
    }

    private static void TestSearchPaddedEdges()
    {
        Config("Fix4", true);
        foreach (var vertical in new[] { false, true })
        foreach (var padded in new[] { false, true })
        {
            var width = vertical ? 1 : 2;
            var height = vertical ? 2 : 1;
            var blocker = Item(new GridShapeBuilder(1, 1).SetDataFill(1));
            var inventory = Inventory<SearchInventory>(
                new GridShapeBuilder(width, height).SetDataOutside(1), blocker);
            var incoming = Item(new GridShapeBuilder().SetData(padded ? "10" : "11", width));
            object?[] args = { inventory, incoming, true, null, false };
            Check(!(bool)Call("Fix4", "TryFindOneValidInventorySlotPrefix", args)!,
                "Edge searches must exercise the optimized path.");
            var actual = args[3] as SlotMarker;
            var vanilla = inventory.TryFindOneValidInventorySlot(incoming, true);
            if (padded)
            {
                Check(actual != null && vanilla != null && actual.itemGridShape.minX == vanilla.itemGridShape.minX
                    && actual.itemGridShape.minY == vanilla.itemGridShape.minY && actual.numTransfer == vanilla.numTransfer,
                    "Blank padding may extend past the right or bottom edge, as in vanilla.");
            }
            else
            {
                Check(actual == null && vanilla == null, "Occupied cells must not extend outside the inventory.");
            }
        }
    }

    private static void TestShowFailure()
    {
        Set("LayoutSuppressor", "Clock", (Func<int>)(() => 100));
        Config("SkipLayoutMark", true);
        ConfigInt("StickyLayoutFrames", 0);
        Set("LayoutSuppressor", "_depth", 2);
        Set("LayoutSuppressor", "_active", true);
        var failure = new InvalidOperationException("Show failed");
        Check(ReferenceEquals(Call("Hooks", "ShowFinalizer", failure, (1, 0L)), failure), "Keep the Show failure.");
        Check((int)ModType("LayoutSuppressor").GetProperty("Depth", Static)!.GetValue(null)! == 1,
            "An inner Show must preserve the outer suppression scope.");
        Call("Hooks", "ShowFinalizer", failure, (0, 0L));
        Check(!(bool)ModType("LayoutSuppressor").GetField("_active", Static)!.GetValue(null)!,
            "A failed outer Show must release layout suppression.");
    }

    private static void TestModuleFailure()
    {
        Config("ModuleRecomputeDedup", true);
        var brokenModule = Empty<GameItemElement>();
        // Broken module fails during activation; empty bay fails later when writing
        // effects to the deliberately uninitialized machine. Both must propagate.
        foreach (var items in new[] { new GameItem[] { brokenModule }, Array.Empty<GameItem>() })
        {
            var inventory = Inventory<GameGridInventory>(new GridShapeBuilder(1, 1), items);
            try
            {
                Call("RefVisual", "ModuleActionedPrefix", Empty<GameItemElement>(), inventory, null);
                throw new Exception("Module failure must propagate instead of retrying vanilla or being swallowed.");
            }
            catch (TargetInvocationException ex) when (ex.InnerException is NullReferenceException) { }
            Check(!(bool)ModType("RefVisual").GetField("_suppress", Static)!.GetValue(null)!,
                "Module suppression must be released on failure.");
        }
    }

    private static void TestQueriesDisabled()
    {
        Config("GridOccupancyShortcut", false);
        var harmony = new HarmonyLib.Harmony("BetterPerformance.Tests.DisabledQueries");
        Call("RefQueries", "Install", harmony);
        Check(!harmony.GetPatchedMethods().Any(), "Disabled queries must not install any detours.");
    }

    // Substitute only the Unity visual refresh; retain the game's tag writes and
    // shape/state validation so all three intermediate validations are observable.
    private sealed class StatItem : GameItemElement
    {
        private StatItem() : base("test", "", "") { }
        internal int Validations;
        public override void Validate() => Validations++;
    }

    private static void TestVanillaTempStatReset()
    {
        var module = Empty<StatItem>();
        module.shape = new GridShapeBuilder(1, 1).SetDataFill(1);
        module.modifiedShape = module.shape;
        module.state = new TagSystem();
        var stats = new[] { "PERFORMANCE", "EFFICIENCY", "QUALITY" };
        for (var i = 0; i < stats.Length; i++)
        {
            module.state.GetTag($"BONUS_PERCENTAGE_{stats[i]}_INT").SetInt(11 + i);
            module.state.GetTag($"TEMP_PERCENTAGE_{stats[i]}_INT").SetInt(99);
        }
        module.modifiedState = module.state.Clone();
        ModuleEffectHelper.ResetAllTempStat(module);
        Check(module.Validations == 3, "Vanilla reset must validate all three tag writes.");
        for (var i = 0; i < stats.Length; i++)
            Check(module.GetTagReadonly($"TEMP_PERCENTAGE_{stats[i]}_INT").GetInt() == 11 + i,
                "Temporary stats must retain the permanent bonus after resetting.");
    }

    private static void TestDiagnostics()
    {
        var messages = new List<string>();
        var clock = ModType("Diagnostics").GetField("Clock", Static)!;
        var writer = ModType("Diagnostics").GetField("Write", Static)!;
        var originalClock = clock.GetValue(null);
        var originalWriter = writer.GetValue(null);
        try
        {
            Call("Diagnostics", "Reset");
            Config("DiagnosticsEnabled", false);
            Set("Diagnostics", "Clock", (Func<long>)(() => throw new Exception("Disabled diagnostics must not read the clock.")));
            Set("Diagnostics", "Write", (Action<string>)messages.Add);
            Check((long)Call("Diagnostics", "Begin")! == 0, "Disabled diagnostics must return an inactive measurement.");
            Call("Diagnostics", "Count", "disabled");
            Call("Diagnostics", "Tick");
            Call("Diagnostics", "ReportTagPatchOwners");
            Call("Diagnostics", "Flush");
            Check(messages.Count == 0, "Disabled diagnostics must not write logs.");

            var now = 100L * System.Diagnostics.Stopwatch.Frequency;
            Set("Diagnostics", "Clock", (Func<long>)(() => now));
            Config("DiagnosticsEnabled", true);
            Call("Diagnostics", "Tick");
            Check(messages.Count == 1 && messages[0].Contains("已开启"), "Enabling must announce the diagnostic session once.");
            var patchedBefore = HarmonyLib.Harmony.GetAllPatchedMethods().ToArray();
            Call("Diagnostics", "ReportTagPatchOwners");
            Check(messages.Count == 3 && messages.Any(line => line.Contains("GameItem.ModifyTag: none"))
                && messages.Any(line => line.Contains("ModuleEffectHelper.ResetAllTempStat: none")),
                "Enabled diagnostics must report both target owners after installation.");
            Call("Diagnostics", "ReportTagPatchOwners");
            Check(messages.Count == 3 && patchedBefore.SequenceEqual(HarmonyLib.Harmony.GetAllPatchedMethods()),
                "Owner reporting must run once and must not install detours.");
            var started = (long)Call("Diagnostics", "Begin")!;
            now += System.Diagnostics.Stopwatch.Frequency / 100;
            var failure = new InvalidOperationException("diagnostic failure");
            Check(ReferenceEquals(Call("Hooks", "ShowFinalizer", failure, (0, started)), failure),
                "Diagnostic finalizers must preserve the original exception.");
            started = (long)Call("Diagnostics", "Begin")!;
            now += System.Diagnostics.Stopwatch.Frequency / 50;
            Call("Hooks", "ShowFinalizer", null, (0, started));
            Call("Diagnostics", "Count", "fallback");
            Call("Diagnostics", "Count", "fallback");
            Call("Diagnostics", "Tick");
            Check(messages.Count == 3, "Diagnostics must not log each invocation.");
            now += 10L * System.Diagnostics.Stopwatch.Frequency;
            Call("Diagnostics", "Tick");
            Check(messages.Any(line => line.Contains("WindowShow") && line.Contains("次数=2") && line.Contains("异常=1")
                && line.Contains($"均值={15.0:F3}ms") && line.Contains($"最大={20.0:F3}ms")),
                "Summary must contain the call and exception counts.");
            Check(messages.Any(line => line.Contains("diagnostic failure")) && messages.Any(line => line.Contains("fallback: 2")),
                "Summary must retain the first exception and fallback counts.");
            var count = messages.Count;
            Call("Diagnostics", "Flush");
            Check(messages.Count == count, "Flushed samples must not be reported twice.");
            Config("DiagnosticsEnabled", false);
            Call("Diagnostics", "Tick");
            Check(messages.Last().Contains("已关闭") && (long)Call("Diagnostics", "Begin")! == 0,
                "Disabling must take effect without restarting.");
            Config("DiagnosticsEnabled", true);
            Call("Diagnostics", "Tick");
            count = messages.Count;
            Call("Diagnostics", "ReportTagPatchOwners");
            Check(messages.Count == count + 2, "Re-enabling diagnostics must refresh the owner snapshot.");
        }
        finally
        {
            Config("DiagnosticsEnabled", false);
            Call("Diagnostics", "Reset");
            clock.SetValue(null, originalClock);
            writer.SetValue(null, originalWriter);
        }
    }

}
