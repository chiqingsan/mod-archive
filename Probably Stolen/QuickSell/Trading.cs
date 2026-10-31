using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QuickSell;

public sealed partial class QuickSellMod
{
    private void StartDiscovery()
    {
        if (IsRestocking) { if (page == 0) pageStatus.text = "上货中，稍后刷新"; return; }
        ReplaceJob(0, Discover());
        if (page == 0) pageStatus.text = "正在扫描…";
    }

    private IEnumerator Discover()
    {
        var store = PlayerStore.Instance;
        var entry = EmporiumEntry.Instance;
        if (store == null || entry == null) yield break;
        knownItems.Clear(); containers.Clear();
        var queue = new Queue<(GameInventory Inventory, string Path)>();
        queue.Enqueue((entry.invElement, "库存"));
        queue.Enqueue((entry.backInvinvElement, "台面"));
        queue.Enqueue((entry.frontInvinvElement, "前台"));
        queue.Enqueue((entry.showcaseElement, "展示柜"));
        queue.Enqueue((entry.hiddenElement, "暗格"));
        var visited = new HashSet<GameInventory>();
        while (queue.Count > 0)
        {
            if (store != PlayerStore.Instance || entry != EmporiumEntry.Instance) yield break;
            var source = queue.Dequeue();
            if (source.Inventory == null || !visited.Add(source.Inventory)) continue;
            var items = source.Inventory.childItems;
            yield return null;
            foreach (var item in items)
            {
                if (store != PlayerStore.Instance || entry != EmporiumEntry.Instance) yield break;
                if (item.parentInventory != source.Inventory || !GeneralHelper.IsItemOwned(item)) { yield return null; continue; }
                int id = item.GetUniqueID(); knownItems[id] = item;
                if (item.contentWindow != null && !item.IsGameItemType("MACHINE") && !item.IsGameItemType("MODULE") && !item.IsGameItemType("NODE") && !item.IsTag("ADV_GUN_TAG"))
                {
                    var inventories = GraphUtils.FindChildrenType<GameInventory>(item.contentWindow);
                    string position = source.Inventory is GameGridInventory grid
                        ? $"[第 {item.modifiedShape.minY - grid.inventoryShape.minY + 1} 行、第 {item.modifiedShape.minX - grid.inventoryShape.minX + 1} 列]" : "";
                    string path = source.Path + position + " → " + item.GetDisplayName(true);
                    if (inventories.Any(value => value is GameGridInventory)) containers.Add((item, path));
                    foreach (var inventory in inventories) queue.Enqueue((inventory, path));
                }
                yield return null;
            }
        }
        if (settings != null && settings.IsAlive && (page == 0 || page == 1))
        {
            RefreshPage();
            if (page == 0) pageStatus.text = $"找到 {containers.Count} 个自有容器";
        }
    }

    private void BeginRestock()
    {
        if (IsRestocking) { ReplaceJob(1, null); SetStatus("已取消；已摆上的商品保留。", true); return; }
        if (!SelectionRules.HasSource(mainInventory.Value, zoneIds.Count)) { SetStatus("请启用主物品栏上货或加入供货区。", true); OpenWindow(); return; }
        ReplaceJob(0, null);
        SetStatus("正在检索供货区…");
        ReplaceJob(1, Restock());
    }

    private bool Allowed(GameItem item, StoreClient client)
    {
        if (item.parentInventory == null || item.parentInventory.IsRemoveLocked() || !GeneralHelper.IsItemOwned(item) || !item.MayRemove()) return false;
        if (IsProtected(item)) return false;
        if (GraphUtils.FindParentsType<GameItem>(item).Any(IsProtected)) return false;
        if (!client.IsClientBuyingThisItem(item) || client.CanClientExposeAnyFeatureDeep(item)) return false;
        if ((client.isSecurity || client.IsFaction(StoreClient.FACTION_SECURITY)) && StolenHelper.IsStolenItem(item)) return false;
        if (client.isRequireVariety && client.IsItemInVarietyList(item)) return false;
        return !HasProtectedContent(item);
    }

    // 顾客临时特征（零售加价、多样化、游客税等）会在物品上台后抬高价值；
    // 规划与拆堆报价用同样的口径估算，避免上台后才超预算、整次上货没有进度。
    private static long GetStagedQuote(GameItem item, StoreClient client)
    {
        const long clientMarketBonus = 15;
        if (item == null || client == null) return 0;
        if (client.clientIntent != StoreClient.ClientIntent.BUY && client.clientIntent != StoreClient.ClientIntent.SELLNBUY)
            return item.GetNegociatedValue();
        long extraMarketMod = 0;
        if (client.isRequireVariety) extraMarketMod += clientMarketBonus;
        if (client.clientFaction == "FACTION_TOURIST") extraMarketMod += clientMarketBonus;
        bool markup = PlayerStore.Instance != null && PlayerStore.Instance.IsCurrentClientAcceptMarkup();
        return item.GetCurrentValue(useRetailMarkup: markup, forceMarkup: markup, extraMarketMod: extraMarketMod);
    }

    // 物品上台瞬间原版会触发顾客对话；物品被回滚、台面没有可售物品时，
    // 这条对话会残留，看起来像上货成功了。只清理“放置物品”类对话。
    private static void ClearStalePlacementDialog(GameGridInventory table, StoreClient client)
    {
        if (table == null || client == null) return;
        if (table.childItems.Any(value => GeneralHelper.IsItemOwned(value) && client.IsClientBuyingThisItem(value))) return;
        var dialogs = DialogUIManager.Instance;
        if (dialogs == null) return;
        Dialogue? current = dialogs.GetCurrentDialog();
        if (current != null && current.priority == 4 && current.choices.Count == 0) dialogs.InterruptDialog();
    }

    private sealed class RestockPassResult
    {
        internal int Moved;
        internal bool Completed, NewlyEmpty;
        internal string Summary = "";
    }

    private sealed class CounterSlotSearch
    {
        internal SlotMarker? Slot;
    }

    // The native search prefers merging into an existing stack or an inner
    // container. Search actual counter cells when that is its first result.
    private static IEnumerator FindCounterSlot(GameGridInventory table, GameItem item, int quantity, CounterSlotSearch result)
    {
        var first = table.TryFindOneValidInventorySlot(item);
        if (first == null) yield break;
        if (first.inventory == table && first.targetItem == null && first.numTransfer >= quantity)
        { result.Slot = first; yield break; }
        var shape = new GridShapeBuilder(item.modifiedShape);
        for (int flip = 0; flip < 2; flip++)
        for (int rotation = 0; rotation < 4; rotation++)
        for (int y = 0; y < table.inventoryShape.height; y++)
        for (int x = 0; x < table.inventoryShape.width; x++)
        {
            shape.SetTransform(x, y, flip != 0, 4 - rotation);
            var slot = table.TryInventorySlot(item, quantity, shape);
            if (slot != null && slot.inventory == table && slot.targetItem == null && slot.numTransfer >= quantity)
            { result.Slot = slot; yield break; }
            yield return null;
        }
    }

    private IEnumerator Restock()
    {
        const int maxPasses = 8;
        var customer = PlayerStore.Instance?.currentClientInstance;
        bool initialMainInventory = mainInventory.Value, initialNested = nested.Value;
        var initialZones = zoneIds.ToArray();
        int totalMoved = 0;
        for (int pass = 0; pass < maxPasses; pass++)
        {
            if (customer == null || PlayerStore.Instance?.currentClientInstance != customer) yield break;
            var result = new RestockPassResult();
            var work = RestockPass(result, initialMainInventory, initialNested, initialZones);
            try { while (work.MoveNext()) yield return null; }
            finally { (work as IDisposable)?.Dispose(); }
            if (!result.Completed) yield break;
            totalMoved += result.Moved;
            if (!SelectionRules.RetryAfterEmptyContainer(result.Moved, result.NewlyEmpty, pass, maxPasses))
            {
                string suffix = result.NewlyEmpty && result.Moved > 0 && pass == maxPasses - 1 ? " 还有新空出的容器，请再次上货。" : "";
                string status = (totalMoved == 0 ? "没有符合条件及剩余预算的商品。" : $"已上货 {totalMoved} 件，请检查后手动出售。") + result.Summary + suffix;
                var client = customer.GetClientBlueprint();
                var table = EmporiumEntry.Instance?.backInvinvElement;
                if (table != null && client.useClientBudget)
                {
                    long tableLimit = client.GetBudget() + (long)AllowanceAmount;
                    long tableTotal = table.childItems.Where(value => GeneralHelper.IsItemOwned(value) && client.IsClientBuyingThisItem(value)).Sum(value => value.GetNegociatedValue());
                    // 台面价值可能在个别物品上台后因顾客临时特征继续上涨，
                    // 最后再核对一次总额，超预算时明确提示，避免悄悄卖出。
                    if (tableTotal > tableLimit) status = (totalMoved > 0 ? $"已上货 {totalMoved} 件，但台面总额" : "台面总额") + $" {tableTotal} 超出预算 {tableLimit}，请检查后再出售。";
                }
                SetStatus(status, true);
                yield break;
            }
            SetStatus($"已上货 {totalMoved} 件，正在检查新空出的容器…");
            yield return null;
        }
    }

    private bool SourcesChanged(bool initialMainInventory, bool initialNested, int[] initialZones)
        => mainInventory.Value != initialMainInventory || nested.Value != initialNested || !zoneIds.SetEquals(initialZones);

    private IEnumerator RestockPass(RestockPassResult result, bool initialMainInventory, bool initialNested, int[] initialZones)
    {
        var store = PlayerStore.Instance;
        var entry = EmporiumEntry.Instance;
        if (store == null || entry == null || store.currentClientInstance == null) yield break;
        var customer = store.currentClientInstance;
        var client = customer.GetClientBlueprint();
        if (!client.IsClientBuying() || !client.useClientBudget || client.isBusinessComplete || NegociationUIManager.IsTradeLocked())
        { SetStatus("当前顾客不适用自动上货。", true); yield break; }
        if (initialZones.Any(id => !knownItems.TryGetValue(id, out var item) || item.parentInventory == null || !GeneralHelper.IsItemOwned(item)))
        {
            var discovery = Discover();
            try { while (discovery.MoveNext()) yield return null; }
            finally { (discovery as IDisposable)?.Dispose(); }
        }
        if (store != PlayerStore.Instance || customer != store.currentClientInstance) yield break;
        var queue = new Queue<(GameInventory Inventory, bool Main)>();
        var sourceContainers = new HashSet<GameItem>();
        foreach (int id in initialZones)
        {
            if (knownItems.TryGetValue(id, out var container) && container.contentWindow != null && !IsProtected(container))
            {
                sourceContainers.Add(container);
                foreach (var inventory in GraphUtils.FindChildrenType<GameInventory>(container.contentWindow)) queue.Enqueue((inventory, false));
            }
            yield return null;
        }
        if (initialMainInventory) queue.Enqueue((entry.invElement, true));
        var protectedContainers = new HashSet<GameItem>(sourceContainers);
        foreach (var container in sourceContainers)
            foreach (var parent in GraphUtils.FindParentsType<GameItem>(container)) protectedContainers.Add(parent);
        var candidates = new List<GameItem>();
        var candidateHeat = new List<int>();
        var seen = new HashSet<GameItem>();
        var groups = new List<List<SelectionPlanner.Option>>();
        var types = new List<string>();
        var keepers = new Dictionary<string, GameItem>();
        var available = new SortedDictionary<(int Rank, int Heat), List<GameItem>>();
        var previouslyNonEmpty = new List<GameItem>();
        bool orderedSelection = false;
        string initialStolenRule = stolenRule.Value, initialContrabandRule = contrabandRule.Value;
        bool initialKeepOne = keepOne.Value;
        int protectedCount = 0, riskCount = 0, spaceCount = 0, budgetCount = 0, reserveCount = 0;
        var visited = new HashSet<GameInventory>();
        var table = entry.backInvinvElement;
        var existing = table.childItems.Where(item => GeneralHelper.IsItemOwned(item) && client.IsClientBuyingThisItem(item)).ToList();
        long budget = Math.Max(0, client.GetBudget() + (long)AllowanceAmount - existing.Sum(item => item.GetNegociatedValue()));
        if (client.isMultiBuyDisabled && existing.Count > 0) { SetStatus("顾客仅接受一件，台面已有商品。", true); yield break; }
        while (queue.Count > 0)
        {
            if (store != PlayerStore.Instance || customer != store.currentClientInstance || NegociationUIManager.IsTradeLocked()) yield break;
            if (SourcesChanged(initialMainInventory, initialNested, initialZones)) { SetStatus("货源设置已变化，请重新上货。", true); yield break; }
            var source = queue.Dequeue();
            var inventory = source.Inventory;
            if (!visited.Add(inventory) || inventory.IsRemoveLocked()) continue;
            var items = inventory.childItems;
            yield return null;
            foreach (var item in items)
            {
                if (store != PlayerStore.Instance || customer != store.currentClientInstance) yield break;
                if (item.parentInventory != inventory || !GeneralHelper.IsItemOwned(item) || !seen.Add(item)) { yield return null; continue; }
                if (!SelectionRules.IsSourceCandidate(source.Main, item.contentWindow != null)) { yield return null; continue; }
                if (keepOne.Value && (!keepers.TryGetValue(item.identifier, out var keeper)
                    || (!IsProtected(keeper) && IsProtected(item))))
                    keepers[item.identifier] = item;
                bool container = item.contentWindow != null && !item.IsTag("ADV_GUN_TAG");
                bool machineContainer = container && (item.IsGameItemType("MACHINE") || item.IsGameItemType("MODULE") || item.IsGameItemType("NODE"));
                if (IsProtected(item))
                {
                    // Count only items that could otherwise be candidates for this customer.
                    // Do not traverse protected containers to count their contents.
                    if (!protectedContainers.Contains(item) && (!container || (!machineContainer && item.FindAllChildItems().Count == 0))
                        && client.IsClientBuyingThisItem(item) && !client.CanClientExposeAnyFeatureDeep(item)
                        && (!client.isRequireVariety || !existing.Any(value => value.identifier == item.identifier))
                        && SelectionRules.RiskRank(StolenHelper.IsStolenItem(item), ContrabandHelper.IsContraband(item),
                            client.isSecurity || client.IsFaction(StoreClient.FACTION_SECURITY), stolenRule.Value, contrabandRule.Value) >= 0)
                        protectedCount++;
                    yield return null;
                    continue;
                }
                if (container)
                {
                    if (!source.Main && initialNested && !machineContainer)
                        foreach (var child in GraphUtils.FindChildrenType<GameInventory>(item.contentWindow)) queue.Enqueue((child, false));
                }
                bool emptyContainer = false;
                if (container && !machineContainer && !protectedContainers.Contains(item))
                {
                    emptyContainer = item.FindAllChildItems().Count == 0;
                    if (!emptyContainer) previouslyNonEmpty.Add(item);
                }
                if (!protectedContainers.Contains(item) && (!container || emptyContainer) && Allowed(item, client)
                    && (!client.isRequireVariety || !existing.Any(value => value.identifier == item.identifier)))
                {
                    bool isStolen = StolenHelper.IsStolenItem(item), isContraband = ContrabandHelper.IsContraband(item);
                    int rank = SelectionRules.RiskRank(isStolen, isContraband, client.isSecurity || client.IsFaction(StoreClient.FACTION_SECURITY), stolenRule.Value, contrabandRule.Value);
                    if (rank < 0) { riskCount++; yield return null; continue; }
                    int heat = isStolen ? StolenHelper.GetFoundChance(item) : -1;
                    var key = (rank, heat);
                    if (!available.TryGetValue(key, out var ordered)) available[key] = ordered = new List<GameItem>();
                    ordered.Add(item);
                    orderedSelection |= rank != 0 || isStolen;
                }
                yield return null;
            }
        }
        if (SourcesChanged(initialMainInventory, initialNested, initialZones)) { SetStatus("货源设置已变化，请重新上货。", true); yield break; }
        foreach (var bucket in available)
        foreach (var item in bucket.Value)
        {
            int maxQuantity = (int)SelectionRules.Available(item.unitCount, keepOne.Value && keepers[item.identifier] == item);
            if (maxQuantity <= 0) { reserveCount++; yield return null; continue; }
            long price = GetStagedQuote(item, client);
            (int Quantity, long Price)? partial = null;
            if (maxQuantity < item.unitCount || price > budget)
                partial = split.Value ? CheckedSplitQuote(item, client, budget, maxQuantity) : null;
            int quantity = partial?.Quantity ?? ((maxQuantity == item.unitCount && price <= budget) ? item.unitCount : 0);
            long quote = partial?.Price ?? price;
            if (quantity > 0 && quote > 0 && quote <= budget)
            {
                if (quantity == item.unitCount)
                {
                    var search = new CounterSlotSearch();
                    var scan = FindCounterSlot(table, item, quantity, search);
                    try { while (scan.MoveNext()) yield return null; }
                    finally { (scan as IDisposable)?.Dispose(); }
                    if (search.Slot == null)
                    { spaceCount++; yield return null; continue; }
                }
                int index = candidates.Count;
                candidates.Add(item); candidateHeat.Add(bucket.Key.Heat); types.Add(item.identifier);
                groups.Add(new List<SelectionPlanner.Option> { new(index, quantity, quote, quantity == item.unitCount ? item.modifiedShape.width * item.modifiedShape.height : 0) });
            }
            else if (maxQuantity < item.unitCount) reserveCount++;
            else if (price > budget) budgetCount++;
            yield return null;
        }
        SetStatus("正在计算选货组合…");
        var planner = new SelectionPlanner();
        var selected = new List<SelectionPlanner.Option>();
        if (orderedSelection)
        {
            long planned = 0; var plannedTypes = new HashSet<string>();
            foreach (var group in groups)
            {
                var choice = group[0];
                if (choice.Price <= budget - planned && (!client.isMultiBuyDisabled || selected.Count == 0)
                    && (!client.isRequireVariety || plannedTypes.Add(types[choice.Item]))) { selected.Add(choice); planned += choice.Price; }
                yield return null;
            }
        }
        else
        {
            foreach (var step in planner.Build(groups, budget, priority.Value == "优先腾空间", client.isMultiBuyDisabled, client.isRequireVariety, types)) yield return null;
            selected = planner.Result;
        }
        int moved = 0, lastMoveFrame = -1;
        var selectedIds = new HashSet<int>(selected.Select(value => value.Item));
        bool alternativesAdded = false;
        for (int selectedIndex = 0; selectedIndex < selected.Count; selectedIndex++)
        {
            var choice = selected[selectedIndex];
            while (lastMoveFrame == Time.frameCount) yield return null;
            if (store != PlayerStore.Instance || customer != store.currentClientInstance || client.isBusinessComplete || NegociationUIManager.IsTradeLocked() || table.IsInsertLocked()) break;
            if (SourcesChanged(initialMainInventory, initialNested, initialZones)) { SetStatus("货源设置已变化，请重新上货。", true); yield break; }
            if (keepOne.Value != initialKeepOne || stolenRule.Value != initialStolenRule || contrabandRule.Value != initialContrabandRule) { SetStatus("选货规则已变化，请重新上货。", true); yield break; }
            var item = candidates[choice.Item];
            if (item.parentInventory == null || !visited.Contains(item.parentInventory) || !Allowed(item, client)) continue;
            if (item.parentInventory == entry.invElement && !SelectionRules.IsSourceCandidate(true, item.contentWindow != null)) continue;
            // A container selected while empty may have received contents while
            // the cooperative task was yielding. Never move it in that case.
            if (item.contentWindow != null && !item.IsTag("ADV_GUN_TAG") && item.FindAllChildItems().Count != 0) continue;
            int currentHeat = StolenHelper.IsStolenItem(item) ? StolenHelper.GetFoundChance(item) : -1;
            if (currentHeat != candidateHeat[choice.Item]) { SetStatus("物品赃物状态或热度已变化，请重新上货。", true); yield break; }
            if (keepOne.Value)
            {
                var keeper = keepers[item.identifier];
                if (keeper.parentInventory == null || !visited.Contains(keeper.parentInventory) || !GeneralHelper.IsItemOwned(keeper)
                    || keeper.unitCount < 1 || (keeper == item && item.unitCount - choice.Quantity < 1)) { reserveCount++; continue; }
            }
            if (SelectionRules.RiskRank(StolenHelper.IsStolenItem(item), ContrabandHelper.IsContraband(item), client.isSecurity || client.IsFaction(StoreClient.FACTION_SECURITY), stolenRule.Value, contrabandRule.Value) < 0) { riskCount++; continue; }
            var onTable = table.childItems.Where(value => GeneralHelper.IsItemOwned(value) && client.IsClientBuyingThisItem(value)).ToList();
            if (client.isMultiBuyDisabled && onTable.Count > 0) break;
            if (client.isRequireVariety && onTable.Any(value => value.identifier == item.identifier)) continue;
            long remaining = client.GetBudget() + (long)AllowanceAmount - onTable.Sum(value => value.GetNegociatedValue());
            var search = new CounterSlotSearch();
            var scan = FindCounterSlot(table, item, choice.Quantity, search);
            try { while (scan.MoveNext()) yield return null; }
            finally { (scan as IDisposable)?.Dispose(); }
            if (store != PlayerStore.Instance || customer != store.currentClientInstance || NegociationUIManager.IsTradeLocked()
                || item.parentInventory == null || !visited.Contains(item.parentInventory) || !Allowed(item, client)) yield break;
            if (item.parentInventory == entry.invElement && !SelectionRules.IsSourceCandidate(true, item.contentWindow != null)) continue;
            if (choice.Quantity > item.unitCount) continue;
            if (item.contentWindow != null && !item.IsTag("ADV_GUN_TAG") && item.FindAllChildItems().Count != 0) continue;
            currentHeat = StolenHelper.IsStolenItem(item) ? StolenHelper.GetFoundChance(item) : -1;
            if (currentHeat != candidateHeat[choice.Item]) { SetStatus("物品赃物状态或热度已变化，请重新上货。", true); yield break; }
            if (keepOne.Value != initialKeepOne || stolenRule.Value != initialStolenRule || contrabandRule.Value != initialContrabandRule)
            { SetStatus("选货规则已变化，请重新上货。", true); yield break; }
            if (SourcesChanged(initialMainInventory, initialNested, initialZones)) { SetStatus("货源设置已变化，请重新上货。", true); yield break; }
            if (SelectionRules.RiskRank(StolenHelper.IsStolenItem(item), ContrabandHelper.IsContraband(item), client.isSecurity || client.IsFaction(StoreClient.FACTION_SECURITY), stolenRule.Value, contrabandRule.Value) < 0) { riskCount++; continue; }
            if (keepOne.Value)
            {
                var keeper = keepers[item.identifier];
                if (keeper.parentInventory == null || !visited.Contains(keeper.parentInventory) || !GeneralHelper.IsItemOwned(keeper)
                    || keeper.unitCount < 1 || (keeper == item && item.unitCount - choice.Quantity < 1)) { reserveCount++; continue; }
            }
            onTable = table.childItems.Where(value => GeneralHelper.IsItemOwned(value) && client.IsClientBuyingThisItem(value)).ToList();
            if (client.isMultiBuyDisabled && onTable.Count > 0) break;
            if (client.isRequireVariety && onTable.Any(value => value.identifier == item.identifier)) continue;
            remaining = client.GetBudget() + (long)AllowanceAmount - onTable.Sum(value => value.GetNegociatedValue());
            GameItem moving = item;
            bool partial = choice.Quantity < item.unitCount;
            if (partial)
            {
                if (CheckedSplitQuote(item, client, remaining, choice.Quantity)?.Quantity != choice.Quantity) continue;
                moving = SplitCandidate(item, choice.Quantity);
            }
            var slot = search.Slot == null ? null : table.TryInventorySlot(moving, moving.unitCount, search.Slot.itemGridShape);
            bool overBudget = GetStagedQuote(moving, client) > remaining;
            if (slot == null || slot.targetItem != null || slot.numTransfer < moving.unitCount || overBudget)
            {
                if (partial) moving.Destroy();
                if (overBudget) budgetCount++; else spaceCount++;
                if (!alternativesAdded) { alternativesAdded = true; SelectionPlanner.AppendAlternatives(groups, selected, selectedIds, selectedIndex, priority.Value == "优先腾空间"); }
                continue;
            }
            var source = item.parentInventory;
            int originalCount = item.unitCount;
            try
            {
                if (partial) ChangeStackAmount(item, -choice.Quantity);
                slot.TryAcceptOnce(moving.unitCount);
            }
            catch (Exception error)
            {
                string name = moving.GetDisplayName();
                int id = moving.uniqueId;
                // Only recover when the split copy is still detached and the
                // source stack is exactly in the state created by this attempt.
                if (partial && moving.parentInventory == null && !source.childItems.Contains(moving)
                    && !table.childItems.Contains(moving) && item.parentInventory == source)
                {
                    if (item.unitCount == originalCount - choice.Quantity) ChangeStackAmount(item, choice.Quantity);
                    if (item.unitCount == originalCount) moving.Destroy();
                }
                throw new InvalidOperationException($"上货转移异常：物品 {name} #{id}，来源 {source.identifier}，来源仍含物品 {source.childItems.Contains(moving)}，台面含物品 {table.childItems.Contains(moving)}，有父库存 {moving.parentInventory != null}。已停止上货。", error);
            }
            lastMoveFrame = Time.frameCount;
            if (moving.parentInventory != table)
            {
                if (table.childItems.Contains(moving))
                    throw new InvalidOperationException("原版接收后物品父库存与台面列表不一致，已停止上货，请重新读档。");
                if (partial && moving.parentInventory == null) { ChangeStackAmount(item, moving.unitCount); moving.Destroy(); }
                ClearStalePlacementDialog(table, client);
                continue;
            }
            bool budgetExceeded = moving.GetNegociatedValue() > remaining;
            bool tableExceeded = false;
            if (!budgetExceeded)
            {
                long tableLimit = client.GetBudget() + (long)AllowanceAmount;
                tableExceeded = table.childItems.Where(value => GeneralHelper.IsItemOwned(value) && client.IsClientBuyingThisItem(value))
                    .Sum(value => value.GetNegociatedValue()) > tableLimit;
            }
            if (budgetExceeded || tableExceeded || !store.CanSellThisItem(moving, ignoreBudget: true))
            {
                var restore = source.TryFindOneValidInventorySlot(moving);
                if (restore != null && restore.targetItem == null && restore.numTransfer >= moving.unitCount) restore.TryAcceptOnce(moving.unitCount);
                if (moving.parentInventory == table) { SetStatus("商品无法放回原容器，已停止，请检查台面。", true); yield break; }
                if (budgetExceeded || tableExceeded) budgetCount++;
                ClearStalePlacementDialog(table, client);
                // 顾客临时特征（多样化、零售加价等）会在上台后抬高价值，规划价可能偏低；
                // 回滚这一件后回填其它候选，避免整次上货没有进度。
                if (!alternativesAdded) { alternativesAdded = true; SelectionPlanner.AppendAlternatives(groups, selected, selectedIds, selectedIndex, priority.Value == "优先腾空间"); }
                continue;
            }
            moved++; SetStatus($"已上货 {moved} 件，继续选货中…");
            NegociationUIManager.Instance.RefreshUI();
            yield return null;
        }
        result.Moved = moved;
        result.Summary = (spaceCount > 0 ? $" 无法放置 {spaceCount}。" : "") + (budgetCount > 0 ? $" 超出预算 {budgetCount}（按上台价）。" : "") + (reserveCount > 0 ? $" 保留库存 {reserveCount}。" : "")
            + (riskCount > 0 ? $" 风险规则排除 {riskCount}。" : "") + (protectedCount > 0 ? $" 顾客需求物品中受保护 {protectedCount} 项。" : "");
        if (moved > 0 && !client.isMultiBuyDisabled)
        {
            foreach (var item in previouslyNonEmpty)
            {
                if (item.parentInventory != null && visited.Contains(item.parentInventory) && item.FindAllChildItems().Count == 0
                    && Allowed(item, client)) { result.NewlyEmpty = true; break; }
                yield return null;
            }
        }
        result.Completed = true;
    }

    private static (int Quantity, long Price)? CheckedSplitQuote(GameItem item, StoreClient client, long budget, int maxQuantity = int.MaxValue)
    {
        if (item.contentWindow != null || item.examineWindow != null || item.itemFeatures.Count != 0 || item.IsTag("BOUGHT_PRICE_TAG")) return null;
        long total = GetStagedQuote(item, client);
        int quantity = total > 0 ? (int)Math.Min(maxQuantity, Math.Min(item.unitCount - 1L, budget * item.unitCount / total)) : 0;
        if (quantity <= 0) return null;
        var copy = SplitCandidate(item, quantity);
        try
        {
            copy.unitCount = quantity;
            long quote = GetStagedQuote(copy, client);
            return quote > 0 && quote <= budget ? (quantity, quote) : null;
        }
        finally { copy.Destroy(); }
    }

    private static GameItem SplitCandidate(GameItem item, int quantity)
    {
        // Limited to items with no feature/window/cost metadata by CheckedSplitQuote.
        // The native tag clone preserves ownership and risk tags instead of reinitialising them.
        var copy = DirectoryMaster.Item(item.identifier);
        copy.state = item.state.Clone(); copy.modifiedState = item.modifiedState.Clone();
        copy.unitValue = item.unitValue; copy.unitBaseValue = item.unitBaseValue;
        copy.lateUnitValue = item.lateUnitValue; copy.backupUnitValue = item.backupUnitValue;
        copy.name = item.name; copy.customText = item.customText;
        copy.shape = item.shape.Clone(); copy.modifiedShape = item.modifiedShape.Clone();
        copy.unitCount = quantity;
        copy.onChangeAmountItemFunc?.Invoke(copy);
        return copy;
    }

    private static void ChangeStackAmount(GameItem item, int delta)
    {
        item.unitCount += delta;
        item.Validate();
        item.onChangeAmountItemFunc?.Invoke(item);
        if (item.parentInventory != null)
        {
            var source = item.parentInventory;
            var slot = source.GetIncompleteItemInventorySlot(item);
            item.onSlotChangeAmountItemFunc?.Invoke(item, source, slot);
            source.onSlotChangeAmountItemFunc?.Invoke(item, source, slot);
        }
        OverlayHandler.current.UpdateInventoryStateUI();
    }
}
