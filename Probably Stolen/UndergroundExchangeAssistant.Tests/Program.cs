using System;
using System.Collections.Generic;
using System.Linq;
using UndergroundExchangeAssistant;

internal static class World
{
    internal static void Reset()
    {
        EmporiumEntry.Instance = new EmporiumEntry();
        PlayerStore.Instance = new PlayerStore();
        StoreReputation.Blocked = false;
        StoreService.Service = new StoreService { isUsing = true };
        ItemMouseDragHandler.current = null;
    }

    internal static GameItem AddItem(string id, int count = 1, int durability = 0)
    {
        var item = new GameItem { identifier = id, unitCount = count, durability = durability };
        EmporiumEntry.Instance.invElement.UncheckedAccept(item);
        return item;
    }

    internal static GameItem TrashItem(string id, int count = 1)
    {
        var item = new GameItem { identifier = id, unitCount = count };
        EmporiumEntry.Instance.trashcanInvElement.UncheckedAccept(item);
        return item;
    }

    internal static ExchangeBarter Barter(string id, int priority, params string[] conditionIds)
    {
        var barter = new ExchangeBarter { identifier = id, priority = priority };
        foreach (var conditionId in conditionIds) barter.conditions.Add(new BarterCondition { Id = conditionId });
        return barter;
    }

    internal static void Register(params ExchangeBarter[] bars)
        => PlayerStore.Instance.exchangeManager.activeBarter.AddRange(bars);

    internal static void RegisterPromotional(params ExchangeBarter[] bars)
    {
        PlayerStore.Instance.exchangeManager.promotionalBarter.AddRange(bars);
        foreach (var bar in bars) PlayerStore.Instance.exchangeManager.activePromotionalBarterIds.Add(bar.identifier);
    }

    internal static List<GameMaterials.Offer> Offers()
        => GameMaterials.ActiveOffers(PlayerStore.Instance.exchangeManager);

    internal static List<GameMaterials.TrashEntry> Trash()
        => GameMaterials.TrashEntries(EmporiumEntry.Instance.trashcan);

    internal static MaterialPlan Plan(string id)
    {
        var offers = Offers();
        var target = offers.First(offer => offer.Id == id);
        return MaterialPlan.Create(offers, target, Trash(), GameMaterials.Candidates(EmporiumEntry.Instance));
    }
}

internal static class Program
{
    private static int failed;

    private static void Check(bool condition, string name)
    {
        if (condition) Console.WriteLine("  PASS " + name);
        else { Console.WriteLine("  FAIL " + name); failed++; }
    }

    private static void SimulationTests()
    {
        Console.WriteLine("ExchangeSimulation");
        World.Reset();
        World.Register(World.Barter("two_seeds", 1, "seed", "seed"));
        World.TrashItem("seed");
        Check(!ExchangeSimulation.Run(World.Offers(), World.Trash()).Accepted.ContainsKey("two_seeds"),
            "one item cannot satisfy two conditions");

        World.Reset();
        World.Register(World.Barter("two_seeds", 1, "seed", "seed"));
        World.TrashItem("seed");
        World.TrashItem("seed");
        Check(ExchangeSimulation.Run(World.Offers(), World.Trash()).Accepted.ContainsKey("two_seeds"),
            "two items satisfy two conditions");

        World.Reset();
        World.Register(World.Barter("low", 1, "dream_dust"), World.Barter("high", 4, "dream_dust"));
        World.TrashItem("dream_dust");
        var settlement = ExchangeSimulation.Run(World.Offers(), World.Trash());
        Check(settlement.Accepted.ContainsKey("high"), "higher priority consumes contested item");
        Check(!settlement.Accepted.ContainsKey("low"), "lower priority loses contested item");

        World.Reset();
        World.Register(World.Barter("bay", 10, "smuggler_bay"), World.Barter("injector", 1, "pink_injector"));
        var bay = World.TrashItem("smuggler_bay");
        bay.contents = new GameInventory { owner = bay };
        bay.contents.UncheckedAccept(new GameItem { identifier = "pink_injector" });
        settlement = ExchangeSimulation.Run(World.Offers(), World.Trash());
        Check(settlement.Accepted.ContainsKey("bay"), "container traded while holding contents");
        Check(!settlement.Accepted.ContainsKey("injector"), "contents consumed with the container");
    }

    private static void PlanTests()
    {
        Console.WriteLine("MaterialPlan");
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        var stack = World.AddItem("water_filter", 6);
        var plan = World.Plan("filter");
        Check(plan.Status == PlanStatus.Ready, "six filters plan a ready filter barter");
        Check(plan.Materials.Count == 3, "only three units are taken from a stack of six");
        Check(plan.Materials.All(item => ReferenceEquals(item, stack)), "all units come from the same stack");
        Check(plan.Placements.Count == 3, "one slot per unit");

        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        World.AddItem("water_filter");
        World.AddItem("water_filter");
        Check(World.Plan("filter").Status == PlanStatus.MissingMaterials, "two filters are not enough");

        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        World.AddItem("water_filter", 6);
        for (int i = 0; i < 3; i++) World.TrashItem("water_filter");
        Check(World.Plan("filter").Status == PlanStatus.AlreadyPrepared, "prepared material is recognised");

        World.Reset();
        World.Register(World.Barter("dream", 2, "dream_dust", "dream_dust"));
        World.RegisterPromotional(World.Barter("improved", 4, "dream_dust"));
        World.AddItem("dream_dust");
        World.AddItem("dream_dust");
        World.AddItem("dream_dust");
        plan = World.Plan("dream");
        Check(plan.Status == PlanStatus.Ready, "promotional contention is accounted for");
        Check(plan.Materials.Count == 3, "one extra dust covers the higher priority barter");

        World.Reset();
        World.Register(World.Barter("dream", 2, "dream_dust", "dream_dust"));
        World.RegisterPromotional(World.Barter("improved", 4, "dream_dust"));
        World.AddItem("dream_dust");
        World.AddItem("dream_dust");
        Check(World.Plan("dream").Status == PlanStatus.MissingMaterials,
            "promotional barter steals one of two dusts");

        World.Reset();
        World.Register(World.Barter("bay", 10, "smuggler_bay"));
        var bay = World.AddItem("smuggler_bay");
        bay.contents = new GameInventory { owner = bay };
        bay.contents.UncheckedAccept(new GameItem { identifier = "valuable" });
        plan = World.Plan("bay");
        Check(plan.Status == PlanStatus.Ready, "bay with contents is planned");
        Check(plan.LostBayContents == 1, "contents lost by trading the bay are counted");
    }

    private static void ArtworkTests()
    {
        Console.WriteLine("PaperArtwork");
        var source = new PaperArtwork.Pixel[25];
        for (int y = 1; y < 4; y++) for (int x = 1; x < 4; x++)
            source[y * 5 + x] = new PaperArtwork.Pixel(190, 159, 158);
        var art = PaperArtwork.Build(source, 5, 5);
        Check(art.Normal.Length == art.Outline.Length && art.Normal.Length == art.Width * art.Height,
            "normal and hover use identical canvases");
        Check(art.Normal[0].A == 0 && art.Outline[0].A == 0, "canvas corners remain transparent");
        Check(!art.Normal.Where((pixel, i) => pixel.A != 0 && art.Outline[i].A != 0).Any(),
            "outline never recolours opaque paper pixels");
        var hollow = new PaperArtwork.Pixel[49];
        for (int y = 2; y <= 4; y++) for (int x = 2; x <= 4; x++)
            if (x != 3 || y != 3) hollow[y * 7 + x] = new PaperArtwork.Pixel(190, 159, 158);
        var ring = PaperArtwork.Outline(hollow, 7, 7);
        Check(ring[3 * 7 + 3].A == 0, "interior transparent holes are not outlined");
        Check(ring[1 * 7 + 2].A == 255 && ring[1 * 7 + 1].A == 255, "exterior and diagonal edges receive a one-pixel outline");
        Check(PaperArtwork.RelativeWidth > 0 && PaperArtwork.RelativeWidth < 1, "paper keeps a relative width");

        Console.WriteLine("NativeOfferRows");
        Check(NativeOfferRows.PermanentIds.Length == 7, "seven permanent rows");
        Check(NativeOfferRows.PermanentIds[0] == "filter_barter"
            && NativeOfferRows.PermanentIds[6] == "smuggler_barter", "row order matches ExchangeManager.Init");

        Console.WriteLine("PaperPointerBlocker");
        var blocker = new PaperPointerBlocker();
        var data = new UnityEngine.EventSystems.PointerEventData();
        blocker.OnBeginDrag(data);
        Check(data.Used, "paper swallows drag");

        Console.WriteLine("PaperHover");
        var highlight = new UnityEngine.GameObject();
        var hover = new PaperHover(); hover.Init(highlight);
        Check(!highlight.activeSelf, "outline starts hidden");
        hover.OnPointerEnter(data); Check(highlight.activeSelf, "mouse entry shows outline");
        hover.OnPointerExit(data); Check(!highlight.activeSelf, "mouse exit hides outline");
        hover.OnPointerEnter(data);
        typeof(PaperHover).GetMethod("OnDisable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(hover, null);
        Check(!highlight.activeSelf, "disabling the paper clears outline");
    }

    private static void RefreshOnce(ExchangeTransfer.ExchangeContext context)
    {
        foreach (var offer in context.Offers)
        {
            using var plan = new ExchangeTransfer(offer.Id, context);
            _ = plan.Reason;
        }
    }

    // Local cost model for the runtime refresh path: one inventory sweep plus one
    // material plan per trade row, which is what Update() does every 0.5-1s while
    // the window is open. Purely a dev tool, not shipped.
    private static void BenchTests(int totalItems)
    {
        Console.WriteLine("Bench");
        World.Reset();
        World.Register(
            World.Barter("filter_barter", 1, "water_filter", "water_filter", "water_filter"),
            World.Barter("module_barter", 1, "system_module_ruined", "system_module_ruined"),
            World.Barter("dream_dust_barter", 2, "dream_dust", "dream_dust"),
            World.Barter("commandKeycardBarter", 1, "cmd_keycard"),
            World.Barter("poison_barter", 1, "poison"),
            World.Barter("bullet_barter", 1, "ammunition"),
            World.Barter("smuggler_barter", 10, "smuggler_bay"));
        World.RegisterPromotional(
            World.Barter("improved_dream_dust_barter", 4, "dream_dust"),
            World.Barter("seed_barter", 2, "seed"),
            World.Barter("immunivax_barter", 1, "pink_injector"));

        string[] pool = { "water_filter", "system_module_ruined", "dream_dust", "seed", "cmd_keycard", "pink_injector", "poison", "ammunition", "junk" };
        const int topLevel = 24;
        for (int i = 0; i < topLevel; i++) World.AddItem(pool[i % pool.Length]);
        const int boxes = 10;
        int perBox = Math.Max(1, (totalItems - topLevel) / boxes);
        for (int b = 0; b < boxes; b++)
        {
            var box = new GameItem { identifier = "backpack", contents = new GameInventory() };
            EmporiumEntry.Instance.invElement.UncheckedAccept(box);
            box.contents!.owner = box;
            for (int i = 0; i < perBox; i++)
                box.contents.UncheckedAccept(new GameItem
                {
                    identifier = pool[(b + i) % pool.Length],
                    durability = i % 400,
                });
        }

        var context = ExchangeTransfer.CreateContext();
        Console.WriteLine($"  inventory total={totalItems + boxes} candidates={context.Candidates.Count} offers={context.Offers.Count} trash={context.Trash.Count}");

        for (int i = 0; i < 20; i++) RefreshOnce(context);
        var contextWatch = System.Diagnostics.Stopwatch.StartNew();
        const int contextRuns = 100;
        for (int i = 0; i < contextRuns; i++) ExchangeTransfer.CreateContext();
        contextWatch.Stop();

        var refreshWatch = System.Diagnostics.Stopwatch.StartNew();
        const int refreshRuns = 200;
        for (int i = 0; i < refreshRuns; i++) RefreshOnce(context);
        refreshWatch.Stop();

        var candidatesWatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < contextRuns; i++) GameMaterials.Candidates(EmporiumEntry.Instance);
        candidatesWatch.Stop();

        Console.WriteLine($"  CreateContext (inventory sweep) = {contextWatch.Elapsed.TotalMilliseconds / contextRuns:F3} ms");
        Console.WriteLine($"  Candidates only               = {candidatesWatch.Elapsed.TotalMilliseconds / contextRuns:F3} ms");
        Console.WriteLine($"  refresh 10 rows (plans)       = {refreshWatch.Elapsed.TotalMilliseconds / refreshRuns:F3} ms");
        Console.WriteLine($"  => open window at 1 Hz uses ~{(contextWatch.Elapsed.TotalMilliseconds / contextRuns + refreshWatch.Elapsed.TotalMilliseconds / refreshRuns):F3} ms/s");
    }

    private static int Main(string[] args)
    {
        if (args.Length >= 1 && args[0] == "--bench")
        {
            BenchTests(args.Length >= 2 ? int.Parse(args[1]) : 760);
            return 0;
        }
        if (args.Length == 5 && args[0] == "--export-art")
        {
            var bytes = System.IO.File.ReadAllBytes(args[1]);
            var input = new PaperArtwork.Pixel[bytes.Length / 4];
            for (int i = 0; i < input.Length; i++) input[i] = new PaperArtwork.Pixel(bytes[i * 4], bytes[i * 4 + 1], bytes[i * 4 + 2], bytes[i * 4 + 3]);
            var art = PaperArtwork.Build(input, int.Parse(args[2]), int.Parse(args[3]));
            System.IO.File.WriteAllBytes(args[4] + ".normal.rgba", art.Normal.SelectMany(p => new[] { p.R, p.G, p.B, p.A }).ToArray());
            System.IO.File.WriteAllBytes(args[4] + ".hover.rgba", art.Normal.SelectMany((p, i) => { var q = p.A != 0 ? p : art.Outline[i]; return new[] { q.R, q.G, q.B, q.A }; }).ToArray());
            System.IO.File.WriteAllText(args[4] + ".size.txt", art.Width + " " + art.Height);
            return 0;
        }
        SimulationTests();
        PlanTests();
        TransferTests.Run(Check);
        ArtworkTests();
        Console.WriteLine(failed == 0 ? "ALL TESTS PASSED" : failed + " TEST(S) FAILED");
        return failed == 0 ? 0 : 1;
    }
}
