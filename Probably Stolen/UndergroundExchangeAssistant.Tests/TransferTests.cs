using System;
using System.Collections.Generic;
using System.Linq;
using UndergroundExchangeAssistant;

internal static class TransferTests
{
    internal static void Run(Action<bool, string> check)
    {
        Console.WriteLine("ExchangeTransfer");
        PreparedStack(check);
        PartialTopUp(check);
        RollbackOnReceivingFailure(check);
        RollbackOfSeparateItems(check);
        NoSpace(check);
        DurabilityOrder(check);
        MachineModules(check);
        BayConfirmation(check);
        Unavailable(check);
        MissingMaterials(check);
        AllInventories(check);
        InstalledFilterShielded(check);
        AuditRegressions(check);
    }

    private static void AuditRegressions(Action<bool, string> check)
    {
        World.Reset();
        var plain = World.AddItem("plain");
        plain.modifiedState.dict.Clear();
        var loading = World.AddItem("loading_item");
        loading.modifiedState = null!;
        var candidates = GameMaterials.Candidates(EmporiumEntry.Instance);
        check(candidates.Contains(plain) && !candidates.Contains(loading), "partially deserialized items are excluded");
        check(plain.modifiedState.dict.Count == 0, "inventory scanning does not initialise durability tags on ordinary items");

        World.Reset();
        World.Register(World.Barter("bay", 10, "smuggler_bay", "injector"));
        var bay = World.AddItem("smuggler_bay");
        bay.contents = new GameInventory { owner = bay };
        var valuable = new GameItem { identifier = "valuable" };
        bay.contents.UncheckedAccept(valuable);
        World.AddItem("injector");
        EmporiumEntry.Instance.trashcanInvElement.FailAt = 2;
        bool threw = false;
        try { using var transfer = new ExchangeTransfer("bay"); transfer.Commit(); }
        catch (InvalidOperationException) { threw = true; }
        check(threw, "receiving failure after bay transfer is reported");
        check(bay.parentInventory == EmporiumEntry.Instance.invElement && valuable.parentInventory == bay.contents,
            "rollback returns the bay intact without extracting its contents");
        check(EmporiumEntry.Instance.trashcanInvElement.items.Count == 0, "failed bay transfer leaves no material in trash");

        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter"));
        World.AddItem("water_filter");
        using (var transfer = new ExchangeTransfer("filter"))
        {
            check(transfer.Reason == "", "a free trash slot plans a ready transfer");
            // Fill the trash after planning: commit must go through the live
            // native placement path and refuse instead of silently picking
            // another slot that no longer exists.
            EmporiumEntry.Instance.trashcanInvElement.inventoryShape = new GridShape { width = 1, height = 1 };
            var blocker = new GameItem { identifier = "blocker", modifiedShape = new GridShape { width = 1, height = 1 } };
            EmporiumEntry.Instance.trashcanInvElement.UncheckedAccept(blocker);
            threw = false;
            try { transfer.Commit(); } catch (InvalidOperationException) { threw = true; }
            check(threw, "commit refuses when the trash has no room left");
            check(EmporiumEntry.Instance.invElement.items.Single().identifier == "water_filter",
                "refused commit leaves the source material in the inventory");
            check(EmporiumEntry.Instance.trashcanInvElement.items.Count == 1,
                "refused commit leaves only the blocker in the trash");
        }
    }

    private static void InstalledFilterShielded(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        var entry = EmporiumEntry.Instance;
        var pitcher = new GameItem { identifier = "water_pitcher" };
        pitcher.EnableTag("FILTER_CONTAINER_TAG");
        pitcher.contents = new GameInventory { owner = pitcher };
        entry.invElement.UncheckedAccept(pitcher);
        var installed = new GameItem { identifier = "water_filter" };
        installed.EnableTag("WATER_FILTER");
        pitcher.contents.UncheckedAccept(installed);
        check(!GameMaterials.Candidates(entry).Any(item => ReferenceEquals(item, installed)),
            "an installed filter inside a water container is shielded");
        using var transfer = new ExchangeTransfer("filter");
        check(transfer.Reason.StartsWith("库存材料不足"),
            "the installed filter is not offered as barter material");
    }

    private static void AllInventories(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        var entry = EmporiumEntry.Instance;
        var front = new GameItem { identifier = "water_filter" };
        entry.frontInvinvElement.UncheckedAccept(front);
        var display = new GameItem { identifier = "water_filter" };
        entry.showcaseElement.UncheckedAccept(display);
        var back = new GameItem { identifier = "water_filter" };
        entry.backInvinvElement.UncheckedAccept(back);
        check(!GameMaterials.Candidates(entry).Any(item => ReferenceEquals(item, display)),
            "items on display in the showcase are shielded");
        var box = new GameItem { identifier = "box" };
        box.contents = new GameInventory { owner = box };
        entry.hiddenElement.UncheckedAccept(box);
        var nested = new GameItem { identifier = "water_filter" };
        box.contents.UncheckedAccept(nested);
        using (var transfer = new ExchangeTransfer("filter"))
        {
            check(transfer.Reason == "", "materials across front, back and nested hidden storage are found");
            transfer.Commit();
        }
        check(entry.trashcanInvElement.items.Count == 3, "cross-inventory transfer moves exactly one set");

        World.Reset(); entry = EmporiumEntry.Instance;
        var customer = new GameItem { identifier = "customer_box", owned = false };
        customer.contents = new GameInventory { owner = customer };
        customer.contents.UncheckedAccept(new GameItem { identifier = "water_filter", owned = true });
        entry.backInvinvElement.UncheckedAccept(customer);
        var trash = World.TrashItem("water_filter");
        check(GameMaterials.Candidates(entry).Count == 0, "customer container contents and trash are excluded");
        var material = new GameItem { identifier = "water_filter" };
        entry.docInvElement.UncheckedAccept(material);
        entry.frontInvinvElement = entry.docInvElement;
        check(GameMaterials.Candidates(entry).Count(item => ReferenceEquals(item, material)) == 1, "shared roots do not duplicate materials");

        World.Reset(); entry = EmporiumEntry.Instance;
        var ownedBox = new GameItem { identifier = "owned_box" };
        ownedBox.contents = new GameInventory { owner = ownedBox };
        entry.hiddenElement.UncheckedAccept(ownedBox);
        var stashed = new GameItem { identifier = "water_filter", owned = false };
        ownedBox.contents.UncheckedAccept(stashed);
        check(GameMaterials.Candidates(entry).Any(item => ReferenceEquals(item, stashed)),
            "materials inside a player container are found without their own owned tag");

        World.Reset(); entry = EmporiumEntry.Instance;
        var bay = new GameItem { identifier = "smuggler_bay_mod" };
        bay.contentWindow = new PixelWindow { style = WindowStyle.Machinery };
        bay.contents = new GameInventory { owner = bay };
        entry.invElement.UncheckedAccept(bay);
        var hidden = new GameItem { identifier = "water_filter" };
        bay.contents.UncheckedAccept(hidden);
        check(GameMaterials.Candidates(entry).Any(item => ReferenceEquals(item, hidden)),
            "item containers use the machinery window style but are still scanned");

        check(GameMaterials.IsExchangeVisible(PlayerStore.Instance), "unlocked save shows paper");
        PlayerStore.Instance.exchangeManager.isExchangeUnlocked = false;
        check(!GameMaterials.IsExchangeVisible(PlayerStore.Instance), "locked save hides paper");
        check(!GameMaterials.IsExchangeVisible(null), "no active save hides paper");
    }

    private static void PreparedStack(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        var stack = World.AddItem("water_filter", 6);
        using (var transfer = new ExchangeTransfer("filter"))
        {
            check(transfer.Reason == "", "stack transfer is ready");
            check(transfer.Materials.Count == 3, "stack transfer takes three units");
            transfer.Commit();
        }
        check(stack.unitCount == 3, "stack keeps its remaining units");
        var trash = EmporiumEntry.Instance.trashcanInvElement.items;
        check(trash.Count == 3 && trash.All(item => item.unitCount == 1), "trash holds three single filters");
        using var again = new ExchangeTransfer("filter");
        check(again.Reason == "垃圾桶已备齐", "second attempt is blocked");
    }

    private static void PartialTopUp(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        World.TrashItem("water_filter");
        World.AddItem("water_filter");
        World.AddItem("water_filter");
        var plan = World.Plan("filter");
        check(plan.Status == PlanStatus.Ready, "partial trash is topped up");
        check(plan.Materials.Count == 2, "only the missing units are planned");
    }

    private static void RollbackOnReceivingFailure(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        var stack = World.AddItem("water_filter", 6);
        EmporiumEntry.Instance.trashcanInvElement.FailAt = 2;
        bool threw = false;
        try
        {
            using var transfer = new ExchangeTransfer("filter");
            transfer.Commit();
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }
        check(threw, "commit failure is reported");
        var inventory = EmporiumEntry.Instance.invElement.items;
        check(inventory.Sum(item => item.unitCount) == 6, "failed commit conserves every material unit");
        check(inventory.All(item => ReferenceEquals(item.parentInventory, EmporiumEntry.Instance.invElement)),
            "conserved units sit back in the inventory");
        check(EmporiumEntry.Instance.trashcanInvElement.items.Count == 0, "failed commit leaves the trash empty");
    }

    private static void RollbackOfSeparateItems(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        var items = new List<GameItem>
        {
            World.AddItem("water_filter"),
            World.AddItem("water_filter"),
            World.AddItem("water_filter"),
        };
        EmporiumEntry.Instance.trashcanInvElement.FailAt = 2;
        bool threw = false;
        try
        {
            using var transfer = new ExchangeTransfer("filter");
            transfer.Commit();
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }
        check(threw, "separate item commit failure is reported");
        check(EmporiumEntry.Instance.invElement.items.Count == 3, "all separate items are back in the inventory");
        check(items.All(item => ReferenceEquals(item.parentInventory, EmporiumEntry.Instance.invElement)),
            "restored items sit in the inventory again");
        check(EmporiumEntry.Instance.trashcanInvElement.items.Count == 0, "trash is empty after rollback");
    }

    private static void NoSpace(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        World.AddItem("water_filter", 6);
        EmporiumEntry.Instance.trashcanInvElement.inventoryShape = new GridShape { width = 1, height = 1 };
        using var transfer = new ExchangeTransfer("filter");
        check(transfer.Reason == "垃圾桶空间不足", "full trashcan is reported");
    }

    private static void DurabilityOrder(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        World.AddItem("water_filter", 1, 100);
        World.AddItem("water_filter", 1, 0);
        World.AddItem("water_filter", 1, 20);
        World.AddItem("water_filter", 1, 30);
        var plan = World.Plan("filter");
        check(plan.Status == PlanStatus.Ready, "four filters cover the barter");
        check(plan.Materials[0].durability == 0, "drained filter is taken first");
        check(plan.Materials.All(item => item.durability != 100), "healthy filter is kept");
    }

    private static void MachineModules(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("module", 1, "system_module_ruined", "system_module_ruined"));
        var machine = World.AddItem("machine");
        machine.machine = true;
        machine.contents = new GameInventory { owner = machine };
        machine.contents.UncheckedAccept(new GameItem { identifier = "system_module_ruined" });
        machine.contents.UncheckedAccept(new GameItem { identifier = "system_module_ruined" });
        using var transfer = new ExchangeTransfer("module");
        check(transfer.Reason == "库存材料不足", "modules inside machines are not taken");
    }

    private static void BayConfirmation(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("bay", 10, "smuggler_bay"));
        var bay = World.AddItem("smuggler_bay");
        bay.contents = new GameInventory { owner = bay };
        bay.contents.UncheckedAccept(new GameItem { identifier = "valuable" });
        using var transfer = new ExchangeTransfer("bay");
        check(transfer.Reason == "", "bay trade is ready");
        check(transfer.LostBayContents == 1, "bay contents loss is counted");
        transfer.Commit();
        check(EmporiumEntry.Instance.trashcanInvElement.items.Any(item => ReferenceEquals(item, bay)),
            "bay moved into the trash");
        check(bay.contents.items.Count == 1, "bay contents are untouched before the night exchange");
    }

    private static void Unavailable(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        World.AddItem("water_filter", 3);
        PlayerStore.Instance.exchangeManager.isExchangeUnlocked = false;
        using (var transfer = new ExchangeTransfer("filter"))
            check(transfer.Reason == "地下交易尚未解锁", "locked exchange is reported");

        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        World.AddItem("water_filter", 3);
        StoreService.Service.isUsing = false;
        using (var transfer = new ExchangeTransfer("filter"))
            check(transfer.Reason == "请先启用清洁服务", "inactive janitor is reported");

        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        World.AddItem("water_filter", 3);
        StoreReputation.Blocked = true;
        using (var transfer = new ExchangeTransfer("filter"))
            check(transfer.Reason == "地下交易当前不可用", "blocked reputation is reported");
    }

    private static void MissingMaterials(Action<bool, string> check)
    {
        World.Reset();
        World.Register(World.Barter("filter", 1, "water_filter", "water_filter", "water_filter"));
        World.AddItem("water_filter");
        using var transfer = new ExchangeTransfer("filter");
        check(transfer.Reason == "库存材料不足", "missing materials are reported");
    }
}
