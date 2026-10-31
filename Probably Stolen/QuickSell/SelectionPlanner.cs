using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickSell;

// Pure bounded planner: one choice per physical item; no game or UI access.
internal sealed class SelectionPlanner
{
    internal readonly struct Option
    {
        internal readonly int Item, Quantity, Space;
        internal readonly long Price;
        internal Option(int item, int quantity, long price, int space) { Item = item; Quantity = quantity; Price = price; Space = space; }
    }
    private sealed class Node
    {
        internal Node? Previous;
        internal Option Choice;
        internal long Value;
        internal int Space, Moves;
        internal ulong Types;
    }
    internal List<Option> Result { get; private set; } = new();
    internal const int StateLimit = 1024, TransitionLimit = 200000;
    internal int Transitions { get; private set; }

    internal IEnumerable<int> Build(IReadOnlyList<List<Option>> groups, long budget, bool spaceFirst, bool single, bool variety, IReadOnlyList<string> types)
    {
        var states = new Dictionary<(long Value, ulong Types), Node> { [(0, 0)] = new Node() };
        var variants = variety ? new Dictionary<long, int> { [0] = 1 } : null;
        var typeHashes = variety ? new ulong[types.Count] : Array.Empty<ulong>();
        var greedy = new Node();
        var greedyTypes = variety ? new HashSet<string>() : null;
        // Always examine all items for a usable baseline, even after the DP work limit.
        foreach (var group in groups)
        {
            foreach (var option in group)
            {
                if (option.Price <= 0 || option.Price > budget - greedy.Value || (single && greedy.Moves > 0)) continue;
                if (variety && !greedyTypes!.Add(types[option.Item])) continue;
                greedy = Add(greedy, option);
                break;
            }
            yield return 0;
        }
        foreach (var group in groups)
        {
            var prior = states.Values.ToArray();
            foreach (var node in prior)
            {
                foreach (var option in group)
                {
                    if (++Transitions > TransitionLimit) goto Finished;
                    if (option.Price > 0 && option.Price <= budget - node.Value && (!single || node.Moves == 0)
                        && (!variety || !ContainsType(node, types[option.Item], types)))
                    {
                        long value = node.Value + option.Price;
                        if (variety && typeHashes[option.Item] == 0) typeHashes[option.Item] = TypeHash(types[option.Item]);
                        ulong typeSet = variety ? node.Types ^ typeHashes[option.Item] : 0;
                        var key = (value, typeSet);
                        if (states.TryGetValue(key, out var old))
                        {
                            if (Better(value, node.Space + option.Space, node.Moves + 1, old, spaceFirst))
                                states[key] = Add(node, option, typeSet);
                        }
                        else if (states.Count < StateLimit && (!variety || !variants!.TryGetValue(value, out int count) || count < 4))
                        {
                            states.Add(key, Add(node, option, typeSet));
                            if (variety) variants![value] = variants.TryGetValue(value, out int existingCount) ? existingCount + 1 : 1;
                        }
                    }
                    yield return 0;
                }
            }
        }
        Finished:
        var best = greedy;
        foreach (var node in states.Values) if (Better(node, best, spaceFirst)) best = node;
        var result = new List<Option>();
        for (var node = best; node.Previous != null; node = node.Previous) result.Add(node.Choice);
        result.Reverse(); Result = result;
    }
    private static Node Add(Node previous, Option option, ulong types = 0) => new() { Previous = previous, Choice = option, Value = previous.Value + option.Price, Space = previous.Space + option.Space, Moves = previous.Moves + 1, Types = types };

    // When a planned move has to be rolled back (for example the customer's
    // temporary features raised the on-counter value), append the remaining
    // candidates in priority order so a failed plan still makes progress.
    internal static void AppendAlternatives(IReadOnlyList<List<Option>> groups, List<Option> selected, HashSet<int> selectedIds, int selectedIndex, bool spaceFirst)
    {
        var alternatives = new List<Option>();
        foreach (var group in groups)
        {
            if (!selectedIds.Contains(group[0].Item)) alternatives.Add(group[0]);
        }
        alternatives.Sort((left, right) => spaceFirst ? right.Space.CompareTo(left.Space) : right.Price.CompareTo(left.Price));
        foreach (var alternative in alternatives)
        {
            if (!selectedIds.Add(alternative.Item)) continue;
            selected.Add(alternative);
            if (selected.Count - selectedIndex > 32) break;
        }
    }
    private static ulong TypeHash(string type)
    {
        ulong hash = 14695981039346656037UL;
        foreach (char value in type) hash = unchecked((hash ^ value) * 1099511628211UL);
        return hash;
    }
    private static bool ContainsType(Node node, string type, IReadOnlyList<string> types)
    {
        for (; node.Previous != null; node = node.Previous) if (types[node.Choice.Item] == type) return true;
        return false;
    }
    private static bool Better(Node a, Node b, bool spaceFirst)
        => Better(a.Value, a.Space, a.Moves, b, spaceFirst);

    private static bool Better(long value, int space, int moves, Node b, bool spaceFirst)
    {
        if (spaceFirst && space != b.Space) return space > b.Space;
        if (value != b.Value) return value > b.Value;
        if (moves != b.Moves) return moves < b.Moves;
        return space > b.Space;
    }
}
