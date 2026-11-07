using System.Collections.Generic;
using System.Linq;
using Entry = UndergroundExchangeAssistant.GameMaterials.TrashEntry;
using Offer = UndergroundExchangeAssistant.GameMaterials.Offer;

namespace UndergroundExchangeAssistant;

// Mirrors ExchangeManager.HandleExchange and ExchangeBarter.GetRequiredItems.
internal static class ExchangeSimulation
{
    internal sealed class Settlement
    {
        internal readonly Dictionary<string, List<Entry>> Accepted = new();
        internal readonly List<Entry> Matched = new();
        internal readonly List<Entry> Remaining = new();
        internal bool Complete;
    }

    // Runs the night settlement in descending priority order. When stopAt is
    // set the simulation stops in front of that offer, exactly where the offer
    // would look for its own materials.
    internal static Settlement Run(IReadOnlyList<Offer> offers, IReadOnlyList<Entry> trash, string? stopAt = null)
    {
        var settlement = new Settlement();
        var remaining = trash.ToList();
        var ordered = offers.ToList();
        ordered.Sort((x, y) => y.Priority.CompareTo(x.Priority));
        foreach (var offer in ordered)
        {
            var matched = Match(offer, remaining);
            if (offer.Id == stopAt)
            {
                settlement.Complete = matched.Count == offer.Conditions.Length;
                settlement.Matched.AddRange(matched);
                settlement.Remaining.AddRange(remaining);
                return settlement;
            }
            if (offer.Conditions.Length == 0 || matched.Count != offer.Conditions.Length) continue;
            settlement.Accepted[offer.Id] = matched;
            Consume(remaining, matched);
        }
        settlement.Remaining.AddRange(remaining);
        return settlement;
    }

    // ExchangeBarter.GetRequiredItems: one distinct item per condition.
    internal static List<Entry> Match(Offer offer, IReadOnlyList<Entry> remaining)
    {
        var matched = new List<Entry>();
        foreach (var condition in offer.Conditions)
        {
            var entry = remaining.FirstOrDefault(candidate =>
                !matched.Contains(candidate) && condition(candidate.Item));
            if (entry != null) matched.Add(entry);
        }
        return matched;
    }

    // Accepted materials are destroyed, so an accepted container also takes its
    // (now unreachable) contents out of the pool for later offers.
    internal static void Consume(List<Entry> remaining, IReadOnlyList<Entry> matched)
        => remaining.RemoveAll(entry => matched.Any(parent => entry.IsSelfOrDescendantOf(parent)));
}
