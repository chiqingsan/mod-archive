using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace XGDragFixer;

/// <summary>
/// Every aged day calls GameItem.ModifyTag, and ModifyTag runs the very expensive
/// ValidateShapeState (shape recompute + gets, parent inventory revalidate, UI
/// refresh). A wine rack applied N days to M bottles therefore refreshed the world
/// N*M times while sleeping. During PlayerStore.EndNight the day increments are
/// pure arithmetic, so collect them and apply one aggregated update per item.
/// </summary>
internal static class AgingBatch
{
    private sealed class ReferenceComparer : IEqualityComparer<GameItem>
    {
        public static readonly ReferenceComparer Instance = new();

        public bool Equals(GameItem x, GameItem y) => ReferenceEquals(x, y);

        public int GetHashCode(GameItem obj) => RuntimeHelpers.GetHashCode(obj);
    }

    private static readonly Dictionary<GameItem, int> Pending = new(ReferenceComparer.Instance);

    private static bool _active;
    private static bool _logged;

    internal static void Reset()
    {
        _active = false;
        Pending.Clear();
    }

    internal static bool Begin()
    {
        if (Core.AgingBatch?.Value != true)
        {
            return false;
        }

        _active = true;
        Pending.Clear();
        return true;
    }

    internal static void End()
    {
        if (!_active)
        {
            return;
        }

        _active = false;
        Flush();
    }

    internal static bool Prefix(GameItem item)
    {
        if (!_active || item == null)
        {
            return true;
        }

        Pending.TryGetValue(item, out var days);
        Pending[item] = days + 1;

        // The accumulated days are applied by Flush once the night is over.
        return false;
    }

    private static void Flush()
    {
        if (Pending.Count == 0)
        {
            return;
        }

        foreach (var pair in Pending)
        {
            try
            {
                GameItem? item = pair.Key;
                var days = pair.Value;
                if (item == null || days <= 0)
                {
                    continue;
                }

                var perDay = item.IsTag("AGE_VALUE_MOD_PER_DAY_INT")
                    ? item.GetTagReadonly("AGE_VALUE_MOD_PER_DAY_INT").GetInt()
                    : AgableHelper.DEFAULT_AGE_VALUE_MOD_PER_DAY;

                item.ModifyTag("AGE_INT", state => state.Enable().SetInt(state.GetInt() + days));
                item.unitValue += (long)perDay * days;
            }
            catch (System.Exception ex)
            {
                if (!_logged)
                {
                    _logged = true;
                    Core.Log?.Warning($"陈化批量刷新异常，已跳过该物品: {ex.GetType().Name} {ex.Message}");
                }
            }
        }

        Pending.Clear();
    }
}
