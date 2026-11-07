using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UndergroundExchangeAssistant;

internal static class GameMaterials
{
    internal static bool IsExchangeVisible(PlayerStore? store)
        => store?.exchangeManager?.isExchangeUnlocked == true;

    internal sealed class Offer
    {
        internal readonly ExchangeBarter Native;
        internal readonly string Id;
        internal readonly int Priority;
        internal readonly Func<GameItem, bool>[] Conditions;

        internal Offer(ExchangeBarter native)
        {
            Native = native;
            Id = native.identifier;
            Priority = native.priority;
            Conditions = native.conditions
                .Select<BarterCondition, Func<GameItem, bool>>(condition => condition.IsItemValid)
                .ToArray();
        }
    }

    // One GameItem together with the item that stores it (if any). Mirrors the
    // breadth-first order of ExchangeBarter.GetRequiredItems.
    internal sealed class TrashEntry
    {
        internal readonly GameItem Item;
        internal readonly TrashEntry? Parent;

        internal TrashEntry(GameItem item, TrashEntry? parent = null)
        {
            Item = item;
            Parent = parent;
        }

        internal bool IsSelfOrDescendantOf(TrashEntry? ancestor)
        {
            for (TrashEntry? current = this; current != null; current = current.Parent)
                if (ReferenceEquals(current, ancestor)) return true;
            return false;
        }
    }

    internal static List<Offer> ActiveOffers(ExchangeManager manager)
    {
        var promos = manager.activePromotionalBarterIds;
        return manager.activeBarter
            .Concat(promos == null
                ? Enumerable.Empty<ExchangeBarter>()
                : manager.promotionalBarter.Where(offer => promos.Contains(offer.identifier)))
            .Select(offer => new Offer(offer))
            .ToList();
    }

    // The grid inside the trashcan item itself. That is the inventory the
    // native exchange (ExchangeBarter.GetRequiredItems) and the night service
    // see; on loaded saves EmporiumEntry.trashcanInvElement can point at a
    // different, orphaned grid instance.
    internal static GameGridInventory? TrashGrid()
    {
        try
        {
            var item = EmporiumEntry.Instance?.trashcan;
            if (item?.contentWindow?.child is GameGridInventory viaWindow) return viaWindow;
        }
        catch (Exception error)
        {
            LogOnce("垃圾桶网格读取失败", error);
        }
        try { return EmporiumEntry.Instance?.trashcanInvElement; }
        catch { return null; }
    }

    internal static bool TrashGridMismatch()
    {
        try
        {
            var entry = EmporiumEntry.Instance;
            if (entry?.trashcanInvElement == null) return false;
            var viaWindow = entry.trashcan?.contentWindow?.child as GameGridInventory;
            return viaWindow != null && !ReferenceEquals(viaWindow, entry.trashcanInvElement);
        }
        catch { return false; }
    }

    // ExchangeBarter.GetRequiredItems walks the trashcan breadth first: direct
    // items first, then the contents of those items.
    internal static List<TrashEntry> TrashEntries(GraphNodeStorage trashcan)
    {
        var result = new List<TrashEntry>();
        Collect(trashcan, null, result);
        return result;
    }

    // The entries a single item contributes when it is moved into the trashcan.
    internal static List<TrashEntry> EntriesFor(GameItem item)
    {
        var root = new TrashEntry(item);
        var result = new List<TrashEntry> { root };
        Collect(item, root, result);
        return result;
    }

    private static void Collect(GraphNodeStorage node, TrashEntry? owner, List<TrashEntry> result)
    {
        var queue = new Queue<(GraphNodeStorage Node, TrashEntry? Owner)>();
        try
        {
            foreach (var child in ChildrenOf(node)) queue.Enqueue((child, owner));
        }
        catch (Exception error)
        {
            LogOnce("垃圾桶内容读取失败", error);
        }
        var seen = new HashSet<GraphNodeStorage>();
        while (queue.Count > 0)
        {
            var (current, parent) = queue.Dequeue();
            if (!seen.Add(current)) continue;
            if (current is GameItem item)
            {
                var entry = new TrashEntry(item, parent);
                result.Add(entry);
                foreach (var child in SafeChildren(item)) queue.Enqueue((child, entry));
            }
            else
            {
                foreach (var child in ChildrenOf(current)) queue.Enqueue((child, parent));
            }
        }
    }

    // Every player-side store root that can hold goods. The same roots the
    // shop-assistant ("帮我上货") scans, plus the document, after-hours, hireling
    // and bazaar storages. The trash and sold-goods inventories are never roots.
    // The showcase is deliberately left out: those items are on display for
    // customers, so the exchange must not pull them as barter material.
    internal static IEnumerable<GameInventory> InventoryRoots(EmporiumEntry entry)
    {
        var roots = new GameInventory?[] { entry.invElement, entry.backInvinvElement,
            entry.backInvinvElementCounter, entry.frontInvinvElement,
            entry.hiddenElement, entry.docInvElement, entry.afterhourInventory,
            entry.hirelingInv, entry.bazarLeftinvElement };
        return roots.Where(root => root != null).Cast<GameInventory>().Distinct();
    }

    internal static List<GameItem> Candidates(EmporiumEntry entry)
    {
        // Walked the same way "帮我上货" discovers goods: queue the player's
        // inventories, then every container window found on a kept item, so
        // material stashed in nested boxes is seen as well.
        var roots = InventoryRoots(entry).ToList();
        var direct = new HashSet<GameItem>();
        foreach (var root in roots)
        {
            var items = SafeChildItems(root);
            foreach (var item in items) direct.Add(item);
        }
        var visited = new HashSet<GameInventory>();
        var queue = new Queue<GameInventory>();
        foreach (var root in roots) if (visited.Add(root)) queue.Enqueue(root);
        var kept = new List<(GameItem Item, int Rank, float Durability, int Container, int Direct)>();
        var seenItems = new HashSet<GameItem>();
        int stateSkipped = 0, ownedSkipped = 0, machineSkipped = 0, installedSkipped = 0, keepSkipped = 0;
        while (queue.Count > 0)
        {
            var inventory = queue.Dequeue();
            var items = SafeChildItems(inventory);
            foreach (var item in items)
            {
                if (item == null || !seenItems.Add(item)) continue;
                try
                {
                    // Deserialization can expose an item before its state is built.
                    if (item.state?.dict == null || item.modifiedState?.dict == null)
                    {
                        stateSkipped++;
                        continue;
                    }
                    // Customer containers and installed machines are not the
                    // player's goods, and their internals must not be scanned.
                    // Note: item containers (backpacks, storage bays, the
                    // smuggler's compartment) also use the machinery window
                    // style, so the style cannot be used to detect machines.
                    if (!OwnedByPlayer(item)) { ownedSkipped++; continue; }
                    if (item.IsGameItemType("MACHINE") || item.IsTag(MachineHelper.STANDARD_MACHINE_TAG))
                    {
                        machineSkipped++;
                        continue;
                    }
                    // A filter slotted inside a filter container (water pitcher)
                    // is installed equipment; pulling it out would break the
                    // container setup, so it is never offered as loose stock.
                    if (IsInstalledFilter(item))
                    {
                        installedSkipped++;
                        continue;
                    }
                    if (SafeKeep(item))
                    {
                        float durability = 0;
                        int container = 0;
                        if (item.identifier == "water_filter"
                            && item.modifiedState.dict.TryGetValue("DURABILITY_TAG", out var tag) && tag != null)
                            durability = tag.GetInt();
                        try { container = item.FindAllChildItems().Count > 0 ? 1 : 0; } catch { }
                        kept.Add((item, item.identifier == "system_module_ruined" ? 0 : 1, durability, container,
                            direct.Contains(item) ? 0 : 1));
                    }
                    else keepSkipped++;
                    // Recurse into nested containers exactly like the shop
                    // assistant does. Reading the inventory below the item's
                    // window keeps this independent of the window being open.
                    foreach (var nested in GraphUtils.FindChildrenType<GameInventory>(item))
                        if (nested != null && visited.Add(nested)) queue.Enqueue(nested);
                }
                catch (Exception error)
                {
                    LogOnce("物品状态异常，已跳过：", error);
                }
            }
        }
        int nestedCount = kept.Count(candidate => !direct.Contains(candidate.Item));
        string summary = "材料扫描：库存根 " + roots.Count + "，候选 " + kept.Count + "，其中嵌套容器 "
            + nestedCount;
        if (kept.Count == 0)
            summary += "（状态异常 " + stateSkipped + "，非玩家所有 " + ownedSkipped + "，机器 "
                + machineSkipped + "，已安装过滤 " + installedSkipped + "，其它过滤 " + keepSkipped
                + "）；明细 " + RootSummary(roots);
        LogMessageOnce(summary);
        return kept
            .OrderBy(candidate => candidate.Rank)
            .ThenBy(candidate => candidate.Item.identifier == "water_filter" ? candidate.Durability : 0)
            .ThenBy(candidate => candidate.Container)
            .ThenBy(candidate => candidate.Direct)
            .Select(candidate => candidate.Item)
            .ToList();
    }

    // Items inside a player container often carry no IS_OWNED_TAG of their own;
    // the ownership of the containing item is what makes them the player's.
    private static bool OwnedByPlayer(GameItem item)
    {
        try
        {
            return GeneralHelper.IsItemOwned(item) || GeneralHelper.IsItemEffectivelyOwned(item);
        }
        catch
        {
            return false;
        }
    }

    private static bool SafeKeep(GameItem item)
    {
        try
        {
            return OwnedByPlayer(item) && item.MayRemove() && item.MaxNumRemove() > 0
                && !ItemPinHelper.IsPinned(item) && !GeneralHelper.IsItemLocked(item)
                && !InMachine(item) && MayMove(item);
        }
        catch (Exception error)
        {
            // A damaged save can leave an item whose slot queries throw. Skip it
            // instead of taking the whole material list down with it.
            LogOnce("物品状态异常，已跳过：", error);
            return false;
        }
    }

    // A container that still holds items may only be offered when the trade is
    // the smuggler bay itself; everything else would silently lose contents.
    private static bool MayMove(GameItem item)
    {
        if (item.identifier == "smuggler_bay") return true;
        try { return item.FindAllChildItems().Count == 0; }
        catch { return false; }
    }

    private static bool InMachine(GameItem item)
    {
        try
        {
            return GraphUtils.FindParentsType<GameItem>(item.parentInventory)
                .Any(parent => parent.IsTag(MachineHelper.STANDARD_MACHINE_TAG)
                    || parent.IsGameItemType("MACHINE"));
        }
        catch { return true; }
    }

    // Filters installed in a filter container (water pitcher) are equipment,
    // not loose materials: the parent item carries FILTER_CONTAINER_TAG.
    private static bool IsInstalledFilter(GameItem item)
    {
        try
        {
            if (!item.IsTag(AmenitiesItemDirectory.WATER_FILTER)) return false;
            GameItem? owner = GraphUtils.FindParentsType<GameItem>(item.parentInventory).FirstOrDefault();
            return owner != null && owner.IsTag(WaterHelper.FILTER_CONTAINER_TAG);
        }
        catch
        {
            return true;
        }
    }

    // Number of items inside a consumed smuggler bay that the night exchange
    // will not use and therefore orphan when the bay is destroyed.
    internal static int LostBayContents(IReadOnlyList<TrashEntry> consumed)
    {
        var consumedItems = new HashSet<GameItem>(consumed.Select(entry => entry.Item));
        int lost = 0;
        foreach (var entry in consumed)
        {
            if (entry.Item.identifier != "smuggler_bay") continue;
            foreach (var nested in SafeChildrenAll(entry.Item))
                if (nested is GameItem item && !consumedItems.Contains(item)) lost++;
        }
        return lost;
    }

    // GameGridInventory.get_childItems and the window lookups inside it can throw
    // when a save carries an item whose inventory was torn down. Reading through
    // GraphNodeStorage.children is the same traversal the game uses, but guarded.
    private static List<GraphNodeStorage> SafeChildren(GraphNodeStorage node)
    {
        try
        {
            var children = node.children;
            if (children == null) return new List<GraphNodeStorage>();
            return children.Where(child => child != null).ToList();
        }
        catch (Exception error)
        {
            LogOnce("容器内容读取失败", error);
            return new List<GraphNodeStorage>();
        }
    }

    private static List<GraphNodeStorage> SafeChildrenAll(GraphNodeStorage node)
    {
        var result = new List<GraphNodeStorage>();
        var queue = new Queue<GraphNodeStorage>();
        queue.Enqueue(node);
        var seen = new HashSet<GraphNodeStorage>();
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!seen.Add(current)) continue;
            foreach (var child in SafeChildren(current))
            {
                result.Add(child);
                queue.Enqueue(child);
            }
        }
        return result;
    }

    // A save whose load was interrupted can leave an inventory whose item list is
    // unreachable through its virtual getters. childItems (plus the reflection
    // fallback) is tried first; when that yields nothing the generic traversal is
    // still used so windows and machines below the node stay reachable.
    private static List<GraphNodeStorage> ChildrenOf(GraphNodeStorage node)
    {
        if (node is GameInventory inventory)
        {
            var items = SafeChildItems(inventory);
            if (items.Count > 0) return items.Cast<GraphNodeStorage>().ToList();
        }
        return SafeChildren(node);
    }

    private static List<GameItem> SafeChildItems(GameInventory inventory)
    {
        try
        {
            return inventory.childItems.Where(item => item != null).ToList();
        }
        catch (Exception)
        {
            // Some saves contain an inventory whose backing list is gone; the
            // private field can still hold the item references.
            return BackingItems(inventory);
        }
    }

    private static List<GameItem> BackingItems(GameInventory inventory)
    {
        try
        {
            for (var type = inventory.GetType(); type != null && type != typeof(object); type = type.BaseType)
            {
                var field = type.GetField("items",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (field?.GetValue(inventory) is IEnumerable list)
                {
                    var result = new List<GameItem>();
                    foreach (var entry in list)
                        if (entry is GameItem item) result.Add(item);
                    return result;
                }
            }
        }
        catch
        {
            // Ignore: report an empty inventory rather than failing the window.
        }
        return new List<GameItem>();
    }

    // Used when a session somehow reports zero candidates: shows which player
    // inventory roots actually returned items so the cause is visible in the log.
    private static string RootSummary(IEnumerable<GameInventory> roots)
    {
        var parts = new List<string>();
        foreach (var root in roots)
        {
            if (root == null) { parts.Add("null=0"); continue; }
            parts.Add(root.GetType().Name + "=" + SafeChildItems(root).Count);
        }
        return string.Join(", ", parts);
    }

    private static readonly HashSet<string> Logged = new HashSet<string>();

    private static void LogOnce(string context, Exception error)
    {
        if (!Logged.Add(context + "|" + error.GetType().Name)) return;
        UnityEngine.Debug.LogWarning("[UndergroundExchange] " + context + " " + error.GetType().Name + ": " + error.Message);
    }

    private static void LogMessageOnce(string message)
    {
        if (!Logged.Add(message)) return;
        UnityEngine.Debug.LogWarning("[UndergroundExchange] " + message);
    }
}
