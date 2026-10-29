using System;
using System.Collections.Generic;
using System.Linq;
using SmartInventoryArrange.Core;

namespace SmartInventoryArrange;

public static class WineBottleOrder
{
	public const string TargetItemId = "wine_bottle";

	private static int StateRank(int? value)
	{
		bool flag;
		if (value.HasValue)
		{
			int valueOrDefault = value.GetValueOrDefault();
			if ((uint)valueOrDefault <= 2u)
			{
				flag = true;
				goto IL_001b;
			}
		}
		flag = false;
		goto IL_001b;
		IL_001b:
		if (!flag)
		{
			if (!value.HasValue)
			{
				return 4;
			}
			return 3;
		}
		return value.Value;
	}

	public static int Compare(WineBottleInfo a, WineBottleInfo b)
	{
		int num = StringComparer.Ordinal.Compare(a.CurrentName, b.CurrentName);
		if (num != 0)
		{
			return num;
		}
		num = StateRank(a.ContainerState).CompareTo(StateRank(b.ContainerState));
		if (num != 0)
		{
			return num;
		}
		num = Nullable.Compare(a.ContainerState, b.ContainerState);
		if (num != 0)
		{
			return num;
		}
		num = ((!a.AgeDays.HasValue) ? 1 : 0).CompareTo((!b.AgeDays.HasValue) ? 1 : 0);
		if (num != 0)
		{
			return num;
		}
		if (a.AgeDays.HasValue)
		{
			num = b.AgeDays.Value.CompareTo(a.AgeDays.Value);
			if (num != 0)
			{
				return num;
			}
		}
		return StringComparer.Ordinal.Compare(a.StableId, b.StableId);
	}

	public static IReadOnlyList<Placement> Apply(LayoutProblem problem, IReadOnlyList<Placement> placements, IReadOnlyList<WineBottleInfo> metadata)
	{
		Dictionary<string, LayoutItem> wine = Enumerable.Where(problem.Items, (LayoutItem i) => StringComparer.Ordinal.Equals(i.ItemId, "wine_bottle")).ToDictionary((LayoutItem i) => i.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		if (wine.Count < 2)
		{
			return placements;
		}
		if (!Native34Solver.ValidateLayout(problem, placements))
		{
			throw new InvalidOperationException("Wine ordering requires a complete legal layout.");
		}
		Dictionary<string, WineBottleInfo> byId = Enumerable.Where(metadata, (WineBottleInfo i) => wine.ContainsKey(i.StableId)).ToDictionary((WineBottleInfo i) => i.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		if (Enumerable.Any((IEnumerable<string>)(object)wine.Keys, (string id) => !byId.ContainsKey(id)))
		{
			throw new InvalidOperationException("Wine ordering metadata missing for a movable wine instance.");
		}
		WineBottleInfo[] array = Enumerable.ToArray(Enumerable.OrderBy(Enumerable.Select((IEnumerable<string>)(object)wine.Keys, (string id) => byId[id]), (WineBottleInfo i) => i, (IComparer<WineBottleInfo>?)Comparer<WineBottleInfo>.Create((Comparison<WineBottleInfo>)Compare)));
		Placement[] array2 = Enumerable.ToArray(Enumerable.OrderBy(Enumerable.Where(placements, (Placement p) => wine.ContainsKey(p.StableId)), (Placement p) => p.Y).ThenBy((Placement p) => p.X).ThenBy((Placement p) => p.StableId, (IComparer<string>?)StringComparer.Ordinal));
		if (array2.Length != array.Length)
		{
			throw new InvalidOperationException("Wine slot conservation check failed.");
		}
		Dictionary<string, Placement> assigned = new Dictionary<string, Placement>((IEqualityComparer<string>)StringComparer.Ordinal);
		bool flag = false;
		DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
		for (int j = 0; j < array2.Length; j++)
		{
			Placement slot = array2[j];
			LayoutItem layoutItem = wine[array[j].StableId];
			Orientation orientation = Enumerable.FirstOrDefault(Enumerable.OrderBy(Enumerable.Where(layoutItem.Orientations, (Orientation o) => o.Mask.Signature == slot.Mask.Signature), (Orientation o) => (o.GameOrientation != slot.GameOrientation) ? 1 : 0).ThenBy((Orientation o) => o.GameOrientation));
			if (orientation == null)
			{
				val = new DefaultInterpolatedStringHandler(78, 3);
				val.AppendLiteral("Wine ordering incompatible native shape: instance=");
				val.AppendFormatted(layoutItem.StableId);
				val.AppendLiteral(", slot=");
				val.AppendFormatted<int>(slot.X);
				val.AppendLiteral(",");
				val.AppendFormatted<int>(slot.Y);
				val.AppendLiteral("; no layout applied.");
				throw new InvalidOperationException(val.ToStringAndClear());
			}
			assigned.Add(slot.StableId, new Placement(layoutItem.StableId, slot.X, slot.Y, orientation.GameOrientation, slot.Mask, layoutItem.PrimaryCategory, slot.Mask.Width > slot.Mask.Height));
			flag |= slot.StableId != layoutItem.StableId || slot.GameOrientation != orientation.GameOrientation;
		}
		if (!flag)
		{
			return placements;
		}
		Placement value;
		Placement[] array3 = Enumerable.ToArray(Enumerable.Select(placements, (Placement p) => (!assigned.TryGetValue(p.StableId, out value)) ? p : value));
		if (!Native34Solver.ValidateLayout(problem, array3))
		{
			throw new InvalidOperationException("Wine ordering final legality check failed.");
		}
		return array3;
	}
}
