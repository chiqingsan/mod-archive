using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using SmartInventoryArrange.Core;

namespace SmartInventoryArrange;

public static class InstanceOrder
{
	public const string Policy = "semantic-slots-v3:real-state-appraisal-ignored;name;empty-last;usable-sealed-rustfree;wine-2-1-0-age-desc-ferment-asc;water-grade-then-volume;food-fresh-first;absolute-remaining-desc;quality-before-quantity;stable-id";

	public static long? SumVolume(IReadOnlyList<int?> components)
	{
		long num = 0L;
		bool flag = false;
		foreach (int? component in components)
		{
			if (component.HasValue)
			{
				num += component.Value;
				flag = true;
			}
		}
		if (!flag)
		{
			return null;
		}
		return num;
	}

	private static int Number<T>(T? a, T? b, bool descending = false) where T : struct, IComparable<T>
	{
		int num = ((!a.HasValue) ? 1 : 0).CompareTo((!b.HasValue) ? 1 : 0);
		if (num != 0)
		{
			return num;
		}
		if (a.HasValue)
		{
			if (!descending)
			{
				return a.Value.CompareTo(b.Value);
			}
			return b.Value.CompareTo(a.Value);
		}
		return 0;
	}

	private static double? Finite(double? n)
	{
		if (!n.HasValue || !double.IsFinite(n.Value))
		{
			return null;
		}
		return n;
	}

	private static int? WineRank(int? value)
	{
		return value switch
		{
			2 => 0, 
			1 => 1, 
			0 => 2, 
			_ => null, 
		};
	}

	public static int Compare(InstanceOrderInfo a, InstanceOrderInfo b)
	{
		int num = StringComparer.Ordinal.Compare(a.ItemId, b.ItemId);
		if (num != 0)
		{
			return num;
		}
		num = StringComparer.Ordinal.Compare(a.CurrentName, b.CurrentName);
		if (num != 0)
		{
			return num;
		}
		if (a.IsEmpty.HasValue || b.IsEmpty.HasValue)
		{
			num = Number(a.IsEmpty, b.IsEmpty);
			if (num != 0)
			{
				return num;
			}
		}
		num = StringComparer.Ordinal.Compare(a.Purpose, b.Purpose);
		if (num != 0)
		{
			return num;
		}
		num = StringComparer.Ordinal.Compare(a.Profile, b.Profile);
		if (num != 0)
		{
			return num;
		}
		num = Number(a.Expired, b.Expired);
		if (num != 0)
		{
			return num;
		}
		num = Number(a.Unavailable, b.Unavailable);
		if (num != 0)
		{
			return num;
		}
		num = Number(a.Opened, b.Opened);
		if (num != 0)
		{
			return num;
		}
		num = Number(a.Rusted, b.Rusted);
		if (num != 0)
		{
			return num;
		}
		num = Number(a.Dented, b.Dented);
		if (num != 0)
		{
			return num;
		}
		num = Number(a.Counterfeit, b.Counterfeit);
		if (num != 0)
		{
			return num;
		}
		switch (a.Profile)
		{
		case "wine":
			num = Number(WineRank(a.WineState), WineRank(b.WineState));
			if (num != 0)
			{
				return num;
			}
			if (a.WineState == 2)
			{
				num = Number(a.AgeDays, b.AgeDays, descending: true);
				if (num != 0)
				{
					return num;
				}
			}
			if (a.WineState == 1)
			{
				num = Number(a.FermentationRemaining, b.FermentationRemaining);
				if (num != 0)
				{
					return num;
				}
			}
			break;
		case "water":
			num = Number(a.QualityRank, b.QualityRank);
			if (num != 0)
			{
				return num;
			}
			num = Number(a.VolumeUl, b.VolumeUl, descending: true);
			if (num != 0)
			{
				return num;
			}
			break;
		case "chemical":
			num = Number(a.QualityRank, b.QualityRank);
			if (num != 0)
			{
				return num;
			}
			num = Number(Finite(a.Purity), Finite(b.Purity), descending: true);
			if (num != 0)
			{
				return num;
			}
			num = Number(a.VolumeUl, b.VolumeUl, descending: true);
			if (num != 0)
			{
				return num;
			}
			break;
		case "ingot":
			num = Number(a.QualityRank, b.QualityRank);
			if (num != 0)
			{
				return num;
			}
			break;
		case "food":
			num = Number(a.FreshnessRank, b.FreshnessRank);
			if (num != 0)
			{
				return num;
			}
			break;
		case "energy":
			num = Number(a.RemainingEnergy, b.RemainingEnergy, descending: true);
			if (num != 0)
			{
				return num;
			}
			break;
		case "uses":
			num = Number(a.RemainingUses, b.RemainingUses, descending: true);
			if (num != 0)
			{
				return num;
			}
			break;
		case "durability":
			num = Number(a.Durability, b.Durability, descending: true);
			if (num != 0)
			{
				return num;
			}
			break;
		case "condition":
			num = Number(a.EquipmentRank, b.EquipmentRank);
			if (num != 0)
			{
				return num;
			}
			break;
		}
		return StringComparer.Ordinal.Compare(a.StableId, b.StableId);
	}

	public static string ShapeClass(LayoutItem i)
	{
		return JsonSerializer.Serialize(Enumerable.OrderBy(Enumerable.Distinct(Enumerable.Concat(Enumerable.Select(i.Orientations, (Orientation o) => o.Mask.Signature), (i.OriginalMask == null) ? Array.Empty<string>() : new string[1] { i.OriginalMask.Signature }), (IEqualityComparer<string>?)StringComparer.Ordinal), (string s) => s, (IComparer<string>?)StringComparer.Ordinal));
	}

	public static IReadOnlyList<Placement> Apply(LayoutProblem problem, IReadOnlyList<Placement> placements, IReadOnlyList<InstanceOrderInfo> metadata)
	{
		if (!Native34Solver.ValidateLayout(problem, placements))
		{
			throw new InvalidOperationException("Instance ordering requires a complete legal layout.");
		}
		Dictionary<string, LayoutItem> dictionary = problem.Items.ToDictionary((LayoutItem i) => i.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		Dictionary<string, InstanceOrderInfo> info = metadata.ToDictionary((InstanceOrderInfo i) => i.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		if (info.Count != dictionary.Count || Enumerable.Any((IEnumerable<LayoutItem>)(object)dictionary.Values, (LayoutItem i) => !info.TryGetValue(i.StableId, out var value2) || value2.ItemId != i.ItemId))
		{
			throw new InvalidOperationException("Instance ordering metadata identity/membership mismatch.");
		}
		Dictionary<string, Placement> bySlot = placements.ToDictionary((Placement q) => q.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		Dictionary<string, Placement> assigned = new Dictionary<string, Placement>((IEqualityComparer<string>)StringComparer.Ordinal);
		bool flag = false;
		foreach (IGrouping<ValueTuple<string, string>, LayoutItem> item in Enumerable.OrderBy(Enumerable.GroupBy(problem.Items, (LayoutItem i) => new ValueTuple<string, string>(i.ItemId, ShapeClass(i))), (IGrouping<ValueTuple<string, string>, LayoutItem> g) => g.Key.Item1, (IComparer<string>?)StringComparer.Ordinal).ThenBy((IGrouping<ValueTuple<string, string>, LayoutItem> g) => g.Key.Item2, (IComparer<string>?)StringComparer.Ordinal))
		{
			if (Enumerable.Count(item) < 2)
			{
				continue;
			}
			InstanceOrderInfo[] source = Enumerable.ToArray(Enumerable.Select(item, (LayoutItem i) => info[i.StableId]));
			string[] profiles = Enumerable.ToArray(Enumerable.Distinct(Enumerable.Where(Enumerable.Select(source, (InstanceOrderInfo x) => x.Profile), (string x) => x != "name"), (IEqualityComparer<string>?)StringComparer.Ordinal));
			bool profileConflict = profiles.Length > 1;
			InstanceOrderInfo[] array = Enumerable.ToArray(Enumerable.OrderBy(Enumerable.Select(source, (InstanceOrderInfo m) => (!profileConflict) ? ((!(m.Profile == "name") || profiles.Length != 1) ? m : m with
			{
				Profile = profiles[0]
			}) : new InstanceOrderInfo(m.StableId, m.ItemId, m.CurrentName)
			{
				IsUnidentified = m.IsUnidentified
			}), (InstanceOrderInfo i) => i, (IComparer<InstanceOrderInfo>?)Comparer<InstanceOrderInfo>.Create((Comparison<InstanceOrderInfo>)Compare)));
			Placement[] array2 = Enumerable.ToArray(Enumerable.OrderBy(Enumerable.Select(item, (LayoutItem i) => bySlot[i.StableId]), (Placement q) => q.Y).ThenBy((Placement q) => q.X).ThenBy((Placement q) => q.StableId, (IComparer<string>?)StringComparer.Ordinal));
			for (int j = 0; j < array2.Length; j++)
			{
				Placement slot = array2[j];
				LayoutItem layoutItem = dictionary[array[j].StableId];
				IEnumerable<Orientation> source2 = Enumerable.AsEnumerable(layoutItem.Orientations);
				if (layoutItem.OriginalMask != null)
				{
					source2 = Enumerable.Append(source2, new Orientation(layoutItem.OriginalOrientation, layoutItem.OriginalMask, Rotated: false));
				}
				Orientation orientation = Enumerable.FirstOrDefault(Enumerable.OrderBy(Enumerable.Where(source2, (Orientation o) => o.Mask.Signature == slot.Mask.Signature), (Orientation o) => (o.GameOrientation != slot.GameOrientation) ? 1 : 0).ThenBy((Orientation o) => o.GameOrientation));
				if (orientation == null)
				{
					throw new InvalidOperationException("Instance ordering incompatible native shape; no layout applied.");
				}
				bool flag2 = slot.StableId == layoutItem.StableId && slot.GameOrientation == orientation.GameOrientation;
				assigned.Add(slot.StableId, flag2 ? slot : new Placement(layoutItem.StableId, slot.X, slot.Y, orientation.GameOrientation, slot.Mask, layoutItem.PrimaryCategory, slot.Mask.Width > slot.Mask.Height));
				flag = flag || !flag2;
			}
		}
		if (!flag)
		{
			return placements;
		}
		Placement value;
		Placement[] array3 = Enumerable.ToArray(Enumerable.Select(placements, (Placement q) => (!assigned.TryGetValue(q.StableId, out value)) ? q : value));
		if (!Native34Solver.ValidateLayout(problem, array3))
		{
			throw new InvalidOperationException("Instance ordering final conservation/legality check failed.");
		}
		return array3;
	}
}
