using System;
using System.Collections.Generic;
using System.Linq;
using Entry = UndergroundExchangeAssistant.GameMaterials.TrashEntry;
using Offer = UndergroundExchangeAssistant.GameMaterials.Offer;

namespace UndergroundExchangeAssistant;

internal enum PlanStatus { Ready, AlreadyPrepared, MissingMaterials, ConflictingOffer, NoSpace }

internal sealed class MaterialPlan
{
    internal readonly PlanStatus Status;
    // One entry per unit taken; the same stack appears once per split-off unit.
    internal readonly IReadOnlyList<GameItem> Materials;
    // One slot per unit, in the same order as Materials.
    internal readonly IReadOnlyList<SlotMarker> Placements;
    internal readonly int LostBayContents;

    private MaterialPlan(PlanStatus status, IReadOnlyList<GameItem>? materials = null,
        IReadOnlyList<SlotMarker>? placements = null, int lostBayContents = 0)
    {
        Status = status;
        Materials = materials ?? Array.Empty<GameItem>();
        Placements = placements ?? Array.Empty<SlotMarker>();
        LostBayContents = lostBayContents;
    }

    // Candidates arrive in consumption preference order. Materials that are
    // already in the trashcan are reused; only the missing units are added.
    internal static MaterialPlan Create(IReadOnlyList<Offer> offers, Offer target,
        IReadOnlyList<Entry> trash, IReadOnlyList<GameItem> candidates)
    {
        var before = ExchangeSimulation.Run(offers, trash);
        var initial = ExchangeSimulation.Run(offers, trash, target.Id);
        if (initial.Complete)
            return new MaterialPlan(PlanStatus.AlreadyPrepared,
                lostBayContents: GameMaterials.LostBayContents(initial.Matched));
        var additions = new List<GameItem>();
        for (int guard = 0; guard <= 64; guard++)
        {
            var entries = WithAdditions(trash, additions);
            var atTarget = ExchangeSimulation.Run(offers, entries, target.Id);
            if (atTarget.Complete)
            {
                if (additions.Count == 0)
                    return new MaterialPlan(PlanStatus.AlreadyPrepared,
                        lostBayContents: GameMaterials.LostBayContents(atTarget.Matched));
                var after = ExchangeSimulation.Run(offers, entries);
                if (!after.Accepted.ContainsKey(target.Id)) return new MaterialPlan(PlanStatus.ConflictingOffer);
                if (before.Accepted.Keys.Any(id => !after.Accepted.ContainsKey(id)))
                    return new MaterialPlan(PlanStatus.ConflictingOffer);
                // Higher priority bars may eat part of what we add; everything a
                // trade consumes is fine, only leftovers would be cleaned away.
                var consumed = after.Accepted.Values.SelectMany(matched => matched).ToList();
                var trimmed = ConsumedOnly(consumed, additions);
                if (trimmed.Count != additions.Count)
                {
                    additions = trimmed;
                    continue;
                }
                var placements = FindPlacements(additions);
                if (placements == null) return new MaterialPlan(PlanStatus.NoSpace);
                return new MaterialPlan(PlanStatus.Ready, additions, placements,
                    GameMaterials.LostBayContents(consumed));
            }
            var chosen = Choose(additions, candidates, trash, atTarget, target);
            if (chosen == null) return new MaterialPlan(PlanStatus.MissingMaterials);
            additions.Add(chosen);
        }
        return new MaterialPlan(PlanStatus.ConflictingOffer);
    }

    private static List<GameItem> ConsumedOnly(IReadOnlyList<Entry> consumed, IReadOnlyList<GameItem> additions)
    {
        var available = new Dictionary<GameItem, int>();
        foreach (var entry in consumed)
            available[entry.Item] = available.TryGetValue(entry.Item, out int count) ? count + 1 : 1;
        var kept = new List<GameItem>();
        foreach (var item in additions)
        {
            if (!available.TryGetValue(item, out int count) || count == 0) continue;
            available[item] = count - 1;
            kept.Add(item);
        }
        return kept;
    }

    private static List<Entry> WithAdditions(IReadOnlyList<Entry> trash, IReadOnlyList<GameItem> additions)
        => additions.Count == 0
            ? trash.ToList()
            : trash.Concat(additions.SelectMany(GameMaterials.EntriesFor)).ToList();

    private static GameItem? Choose(IReadOnlyList<GameItem> additions, IReadOnlyList<GameItem> candidates,
        IReadOnlyList<Entry> trash, ExchangeSimulation.Settlement atTarget, Offer target)
    {
        var used = new HashSet<Entry>();
        var missing = new List<Func<GameItem, bool>>();
        foreach (var condition in target.Conditions)
        {
            var entry = atTarget.Remaining.FirstOrDefault(candidate =>
                !used.Contains(candidate) && condition(candidate.Item));
            if (entry != null) { used.Add(entry); continue; }
            missing.Add(condition);
        }
        foreach (var condition in missing)
            foreach (var candidate in candidates)
            {
                if (!condition(candidate)) continue;
                if (trash.Any(entry => ReferenceEquals(entry.Item, candidate))) continue;
                // A container already planned carries its contents into the trashcan.
                if (additions.Any(chosen => GraphUtils.FindAllChildrenType<GameItem>(chosen).Contains(candidate))) continue;
                // Never plan an item twice: a container already brings its contents along.
                if (additions.Any(chosen => GraphUtils.FindAllChildrenType<GameItem>(candidate).Contains(chosen))) continue;
                int planned = additions.Count(chosen => ReferenceEquals(chosen, candidate));
                if (planned == 0) return candidate;
                // Stacks can supply several units, each one split off separately.
                if (candidate.FindAllChildItems().Count > 0) continue;
                if (planned >= Math.Max(1, candidate.MaxNumRemove())) continue;
                return candidate;
            }
        return null;
    }

    private static List<SlotMarker>? FindPlacements(IReadOnlyList<GameItem> materials)
    {
        // The trash grid the game itself inspects is the one under the
        // trashcan's content window; trashcanInvElement can be a different
        // instance on loaded saves.
        var grid = GameMaterials.TrashGrid();
        if (grid == null) return null;
        var placements = new List<SlotMarker>();
        foreach (var item in materials)
        {
            var marker = FindSpace(grid, item, placements);
            if (marker == null) return null;
            placements.Add(marker);
        }
        return placements;
    }

    private static SlotMarker? FindSpace(GameGridInventory grid, GameItem item, IReadOnlyList<SlotMarker> used)
    {
        for (int flip = 0; flip < 2; flip++)
        for (int rotation = 0; rotation < 4; rotation++)
        for (int y = 0; y < grid.inventoryShape.height; y++)
        for (int x = 0; x < grid.inventoryShape.width; x++)
        {
            var shape = new GridShapeBuilder(item.modifiedShape).SetTransform(x, y, flip != 0, rotation).Build();
            if (used.Any(placement => shape.Intersects(placement.itemGridShape))) continue;
            var marker = grid.TryInventorySlot(item, 1, shape);
            if (marker.numTransfer == 1 && marker.targetItem == null) return marker;
        }
        return null;
    }
}
