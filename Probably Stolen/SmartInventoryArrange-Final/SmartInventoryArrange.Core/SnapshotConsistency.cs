using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartInventoryArrange.Core;

public static class SnapshotConsistency
{
	public static bool SameNativeObject(nint left, nint right)
	{
		if (left != 0)
		{
			return left == right;
		}
		return false;
	}

	public static void AssertSame(InventoryObservation expected, InventoryObservation actual, string stage)
	{
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		List<string> differences = new List<string>();
		Diff("inventoryPointer", expected.Pointer, actual.Pointer);
		Diff("containerGeometry", expected.Geometry, actual.Geometry);
		Diff("itemCount", expected.Items.Count, actual.Items.Count);
		if (expected.Pointer == 0 || actual.Pointer == 0)
		{
			differences.Add("inventoryPointer: null");
		}
		HashSet<IntPtr> hashSet = Enumerable.ToHashSet(Enumerable.Select(expected.Items, (SnapshotItem x) => x.Pointer));
		Dictionary<IntPtr, SnapshotItem> dictionary = new Dictionary<IntPtr, SnapshotItem>();
		DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
		foreach (SnapshotItem item in actual.Items)
		{
			if (item.Pointer == 0)
			{
				differences.Add("currentMember: null-pointer");
				continue;
			}
			if (!dictionary.TryAdd(item.Pointer, item))
			{
				List<string> list = differences;
				val = new DefaultInterpolatedStringHandler(25, 1);
				val.AppendLiteral("duplicateMember: pointer=");
				val.AppendFormatted<IntPtr>((IntPtr)item.Pointer);
				list.Add(val.ToStringAndClear());
			}
			if (!hashSet.Contains(item.Pointer))
			{
				List<string> list2 = differences;
				val = new DefaultInterpolatedStringHandler(31, 2);
				val.AppendLiteral("unexpectedMember: id=");
				val.AppendFormatted(item.StableId);
				val.AppendLiteral(", pointer=");
				val.AppendFormatted<IntPtr>((IntPtr)item.Pointer);
				list2.Add(val.ToStringAndClear());
			}
		}
		if (hashSet.Count != expected.Items.Count || hashSet.Contains((IntPtr)0))
		{
			differences.Add("capturedMembers: invalid-or-duplicate-pointer");
		}
		foreach (SnapshotItem item2 in expected.Items.OrderBy((SnapshotItem i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal))
		{
			val = new DefaultInterpolatedStringHandler(23, 3);
			val.AppendLiteral("item=");
			val.AppendFormatted(item2.StableId);
			val.AppendLiteral(", fixed=");
			val.AppendFormatted<bool>(!item2.Movable);
			val.AppendLiteral(", pointer=");
			val.AppendFormatted<IntPtr>((IntPtr)item2.Pointer);
			string text = val.ToStringAndClear();
			if (!dictionary.TryGetValue(item2.Pointer, out var value))
			{
				differences.Add(text + ", member: missing");
				continue;
			}
			Diff(text + ", parentPointer", item2.ParentPointer, value.ParentPointer);
			if (item2.Movable && !SameNativeObject(item2.ParentPointer, expected.Pointer))
			{
				differences.Add(text + ", capturedParent: not-target");
			}
			Diff(text + ", state", item2.State, value.State);
			if (item2.Pose == null || value.Pose == null)
			{
				List<string> list3 = differences;
				val = new DefaultInterpolatedStringHandler(33, 2);
				val.AppendLiteral(", shape: oldPresent=");
				val.AppendFormatted<bool>(item2.Pose != null);
				val.AppendLiteral(", newPresent=");
				val.AppendFormatted<bool>(value.Pose != null);
				list3.Add(text + val.ToStringAndClear());
			}
			else
			{
				Diff(text + ", x", item2.Pose.X, value.Pose.X);
				Diff(text + ", y", item2.Pose.Y, value.Pose.Y);
				Diff(text + ", orientation", item2.Pose.Orientation, value.Pose.Orientation);
				Diff(text + ", width", item2.Pose.Width, value.Pose.Width);
				Diff(text + ", height", item2.Pose.Height, value.Pose.Height);
				Diff(text + ", cells", item2.Pose.Cells, value.Pose.Cells);
			}
		}
		if (differences.Count > 0)
		{
			val = new DefaultInterpolatedStringHandler(46, 2);
			val.AppendLiteral("Snapshot guard rejected stage=");
			val.AppendFormatted(stage);
			val.AppendLiteral("; differences=");
			val.AppendFormatted<int>(differences.Count);
			val.AppendLiteral("; ");
			throw new InvalidOperationException(val.ToStringAndClear() + string.Join(" | ", Enumerable.Take((IEnumerable<string>)differences, 12)));
		}
		void Diff(string field, object? before, object? after)
		{
			if (!object.Equals(before, after))
			{
				List<string> list4 = differences;
				DefaultInterpolatedStringHandler val2 = default(DefaultInterpolatedStringHandler);
				val2 = new DefaultInterpolatedStringHandler(12, 3);
				val2.AppendFormatted(field);
				val2.AppendLiteral(": old=");
				val2.AppendFormatted<object>(before ?? "null");
				val2.AppendLiteral(", new=");
				val2.AppendFormatted<object>(after ?? "null");
				list4.Add(val2.ToStringAndClear());
			}
		}
	}
}
