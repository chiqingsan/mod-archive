using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UndergroundExchangeAssistant;

// Plans one night's exchange for a single barter row and moves the missing
// material into the trashcan when the player confirms.
internal sealed class ExchangeTransfer : IDisposable
{
    internal string Reason { get; private set; } = "";
    // One entry per unit to move; a stack appears once per split-off unit.
    internal IReadOnlyList<GameItem> Materials { get; private set; } = Array.Empty<GameItem>();
    // One slot per unit, in the same order as Materials.
    internal IReadOnlyList<SlotMarker> Placements { get; private set; } = Array.Empty<SlotMarker>();
    // How many items inside a smuggler bay would be lost to this exchange.
    internal int LostBayContents { get; private set; }

    private readonly string offerId = "";
    private readonly List<Move> moves = new();

    private sealed class Move
    {
        internal GameItem Placed = null!;
        internal GameItem Material = null!;
        internal GameInventory? Source;
    }

    internal ExchangeTransfer(string id) : this(id, BuildContext()) { }

    internal ExchangeTransfer(string id, ExchangeContext context)
    {
        offerId = id;
        var entry = EmporiumEntry.Instance;
        if (entry == null || entry.trashcan == null || entry.trashcanInvElement == null)
        {
            Reason = "垃圾桶尚未就绪";
            return;
        }
        var manager = PlayerStore.Instance?.exchangeManager;
        if (manager == null)
        {
            Reason = "交易尚未就绪";
            return;
        }
        if (!manager.isExchangeUnlocked)
        {
            Reason = "地下交易尚未解锁";
            return;
        }
        if (StoreReputation.IsPerkUnlocked("BM_DISTRUSTED"))
        {
            Reason = "地下交易当前不可用";
            return;
        }
        var janitor = StoreService.GetJanitorialService();
        if (janitor == null || !janitor.isUsing)
        {
            Reason = "请先启用清洁服务";
            return;
        }
        if (ItemMouseDragHandler.current != null && ItemMouseDragHandler.current.IsDraggingItem)
        {
            Reason = "请先放下手中物品";
            return;
        }
        var target = context.Offers.FirstOrDefault(offer => offer.Id == id);
        if (target == null)
        {
            Reason = "交易已刷新";
            return;
        }
        var plan = MaterialPlan.Create(context.Offers, target, context.Trash, context.Candidates);
        Materials = plan.Materials;
        Placements = plan.Placements;
        LostBayContents = plan.LostBayContents;
        Reason = plan.Status switch
        {
            PlanStatus.Ready => "",
            PlanStatus.AlreadyPrepared => "垃圾桶已备齐",
            PlanStatus.MissingMaterials => "库存材料不足",
            PlanStatus.NoSpace => "垃圾桶空间不足",
            _ => "材料与其他交易冲突",
        };
    }

    // The exchange window asks for every row's state on each refresh. Walking
    // the player's inventories once and reusing the offers/trash/candidates
    // keeps that refresh linear instead of ten full scans per second.
    internal sealed class ExchangeContext
    {
        internal List<GameMaterials.Offer> Offers = null!;
        internal List<GameMaterials.TrashEntry> Trash = null!;
        internal List<GameItem> Candidates = null!;
    }

    private static ExchangeContext BuildContext() => CreateContext();

    internal static ExchangeContext CreateContext()
    {
        var manager = PlayerStore.Instance?.exchangeManager;
        var entry = EmporiumEntry.Instance;
        return new ExchangeContext
        {
            Offers = manager == null ? new List<GameMaterials.Offer>() : GameMaterials.ActiveOffers(manager),
            Trash = entry?.trashcan == null
                ? new List<GameMaterials.TrashEntry>()
                : GameMaterials.TrashEntries(entry.trashcan),
            Candidates = entry == null ? new List<GameItem>() : GameMaterials.Candidates(entry),
        };
    }

    // Moves the planned units with the same call the game itself uses to put
    // items into the trashcan (`GraphUtils.TryAcceptAllOrDestroy` ->
    // `TryAcceptAll` -> `SlotMarker.TryAcceptOnce`), without the destroy half.
    // If anything fails, every unit that already made it in is handed back.
    internal void Commit()
    {
        if (Reason.Length != 0) throw new InvalidOperationException(Reason);
        var entry = EmporiumEntry.Instance;
        if (entry == null || entry.trashcan == null) throw new InvalidOperationException("垃圾桶尚未就绪");
        var trashcan = entry.trashcan;
        var manager = PlayerStore.Instance?.exchangeManager;
        if (manager == null) throw new InvalidOperationException("交易尚未就绪");
        var offers = GameMaterials.ActiveOffers(manager);
        var target = offers.FirstOrDefault(offer => offer.Id == offerId)?.Native;
        if (target == null) throw new InvalidOperationException("交易已刷新");
        var before = new HashSet<GameItem>(GameMaterials.TrashEntries(trashcan).Select(entry2 => entry2.Item));
        ReportTrashGridOnce(entry, trashcan, before.Count);
        try
        {
            if (target.GetRequiredItems().Count == target.conditions.Count)
                throw new InvalidOperationException("垃圾桶已备齐");
            for (int index = 0; index < Materials.Count; index++)
            {
                var material = Materials[index];
                var source = material.parentInventory;
                if (source == null) throw new InvalidOperationException("材料已不在库存中");
                int left;
                try
                {
                    // Native placement path: walks the trashcan item's own
                    // window, so the materials land in the same grid that the
                    // game's own exchange scan reads.
                    left = GraphUtils.TryAcceptAll(trashcan, material, 1);
                }
                catch (Exception)
                {
                    // The call can throw after the unit already left the source
                    // inventory; remember anything that did make it in so the
                    // catch below can hand it back.
                    RecordNew(trashcan, before, material, source);
                    throw;
                }
                RecordNew(trashcan, before, material, source);
                if (left != 0) throw new InvalidOperationException("垃圾桶空间不足");
            }
            if (target.GetRequiredItems().Count != target.conditions.Count)
            {
                var trash = GameMaterials.TrashEntries(trashcan);
                Debug.LogWarning($"[UndergroundExchange] 投放后校验失败 id={offerId} " +
                    $"native={target.GetRequiredItems().Count}/{target.conditions.Count} " +
                    $"moves=[{string.Join(",", moves.Select(move => Describe(move.Placed)))}] " +
                    $"trash=[{string.Join(",", trash.Select(entry2 => Describe(entry2.Item)))}]");
                throw new InvalidOperationException("投放后交易匹配发生变化");
            }
        }
        catch
        {
            ReturnMoves(trashcan);
            throw;
        }
        moves.Clear();
    }

    private void RecordNew(GameItem trashcan, HashSet<GameItem> before, GameItem material, GameInventory? source)
    {
        foreach (var entry in GameMaterials.TrashEntries(trashcan))
        {
            if (!before.Add(entry.Item)) continue;
            if (entry.Parent != null) continue;
            moves.Add(new Move { Placed = entry.Item, Material = material, Source = source });
        }
    }

    // Diagnostic: on loaded saves EmporiumEntry.trashcanInvElement and the grid
    // under the trashcan item's window can be different instances. Commit now
    // uses the window grid, so log the discrepancy once instead of guessing.
    private static bool reportedTrashGrid;

    private static void ReportTrashGridOnce(EmporiumEntry entry, GameItem trashcan, int scanned)
    {
        if (reportedTrashGrid) return;
        reportedTrashGrid = true;
        try
        {
            var windowGrid = trashcan.contentWindow?.child as GameGridInventory;
            bool mismatch = windowGrid != null && !ReferenceEquals(windowGrid, entry.trashcanInvElement);
            if (!mismatch) return;
            Debug.LogWarning("[UndergroundExchange] 垃圾桶网格不一致：以 item.contentWindow.child 为准 " +
                $"invCount={entry.trashcanInvElement?.childItems?.Count ?? -1} " +
                $"windowCount={windowGrid?.childItems?.Count ?? -1} scan={scanned}");
        }
        catch (Exception exception)
        {
            Debug.LogWarning("[UndergroundExchange] 垃圾桶网格核对失败：" + exception.Message);
        }
    }

    private static string Describe(GameItem? item)
    {
        if (item == null) return "null";
        string parent;
        try { parent = item.parentInventory == null ? "none" : item.parentInventory.GetType().Name; }
        catch { parent = "?"; }
        return item.identifier + "@" + parent;
    }

    private void ReturnMoves(GameItem trashcan)
    {
        // Hand back only the units this commit put in. Never destroy anything:
        // if no inventory has room the item is logged instead of deleted.
        var inTrash = new HashSet<GameItem>(GameMaterials.TrashEntries(trashcan).Select(entry2 => entry2.Item));
        for (int i = moves.Count - 1; i >= 0; i--)
        {
            var move = moves[i];
            var placed = move.Placed;
            try
            {
                if (placed == null) continue;
                if (placed.parentInventory != null && inTrash.Contains(placed))
                {
                    try { placed.parentInventory.Expel(placed); }
                    catch (Exception exception)
                    {
                        // The trashcan's sprite refresh can throw after the item
                        // already left the grid; the removal itself still ran.
                        Debug.LogWarning("[UndergroundExchange] 取出材料时游戏刷新异常（物品已取出）：" + exception.Message);
                    }
                }
                if (placed.parentInventory != null) continue;
                if (move.Source != null && GraphUtils.TryAcceptAll(move.Source, placed, 1) == 0) continue;
                if (ReturnToPlayer(placed)) continue;
                Debug.LogError("[UndergroundExchange] 无法归还材料，已放弃移动：" + placed.identifier);
            }
            catch (Exception exception)
            {
                Debug.LogError("[UndergroundExchange] 归还材料失败：" + exception);
            }
        }
        moves.Clear();
    }

    private static bool ReturnToPlayer(GameItem item)
    {
        var entry = EmporiumEntry.Instance;
        if (entry == null) return false;
        var nodes = new GraphNodeStorage?[]
        {
            entry.invElement, entry.backInvinvElement, entry.frontInvinvElement,
            entry.bazarLeftinvElement, entry.showcaseElement,
        };
        foreach (var node in nodes)
        {
            if (node == null) continue;
            if (GraphUtils.CanAccept(node, item) && GraphUtils.TryAcceptAll(node, item, 1) == 0) return true;
        }
        return false;
    }

    public void Dispose() => moves.Clear();
}
