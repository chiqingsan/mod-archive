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
		List<string> differences = new List<string>();
		if (expected.Pointer != actual.Pointer)
		{
			differences.Add("inventoryPointer: old=" + expected.Pointer + ", new=" + actual.Pointer);
		}
		if (!object.Equals(expected.Geometry, actual.Geometry))
		{
			differences.Add("containerGeometry: old=" + expected.Geometry + ", new=" + actual.Geometry);
		}
		if (expected.Items.Count != actual.Items.Count)
		{
			differences.Add("itemCount: old=" + expected.Items.Count + ", new=" + actual.Items.Count);
		}
		if (expected.Pointer == 0 || actual.Pointer == 0)
		{
			differences.Add("inventoryPointer: null");
		}
		HashSet<IntPtr> hashSet = new HashSet<IntPtr>();
		foreach (SnapshotItem item in expected.Items)
		{
			hashSet.Add(item.Pointer);
		}
		Dictionary<IntPtr, SnapshotItem> dictionary = new Dictionary<IntPtr, SnapshotItem>();
		foreach (SnapshotItem item2 in actual.Items)
		{
			if (item2.Pointer == 0)
			{
				differences.Add("currentMember: null-pointer");
				continue;
			}
			if (!dictionary.TryAdd(item2.Pointer, item2))
			{
				differences.Add("duplicateMember: pointer=" + item2.Pointer);
			}
			if (!hashSet.Contains(item2.Pointer))
			{
				differences.Add("unexpectedMember: id=" + item2.StableId + ", pointer=" + item2.Pointer);
			}
		}
		if (hashSet.Count != expected.Items.Count || hashSet.Contains(IntPtr.Zero))
		{
			differences.Add("capturedMembers: invalid-or-duplicate-pointer");
		}
		foreach (SnapshotItem item3 in expected.Items)
		{
			if (!dictionary.TryGetValue(item3.Pointer, out var value))
			{
				differences.Add(ItemText(item3) + ", member: missing");
				continue;
			}
			DiffItem(differences, item3, "parentPointer", item3.ParentPointer, value.ParentPointer);
			if (item3.Movable && !SameNativeObject(item3.ParentPointer, expected.Pointer))
			{
				differences.Add(ItemText(item3) + ", capturedParent: not-target");
			}
			DiffItem(differences, item3, "state", item3.State, value.State);
			if (item3.Pose == null || value.Pose == null)
			{
				differences.Add(ItemText(item3) + ", shape: oldPresent=" + (item3.Pose != null) + ", newPresent=" + (value.Pose != null));
			}
			else
			{
				DiffItem(differences, item3, "x", item3.Pose.X, value.Pose.X);
				DiffItem(differences, item3, "y", item3.Pose.Y, value.Pose.Y);
				DiffItem(differences, item3, "orientation", item3.Pose.Orientation, value.Pose.Orientation);
				DiffItem(differences, item3, "width", item3.Pose.Width, value.Pose.Width);
				DiffItem(differences, item3, "height", item3.Pose.Height, value.Pose.Height);
				DiffItem(differences, item3, "cells", item3.Pose.Cells, value.Pose.Cells);
			}
		}
		if (differences.Count > 0)
		{
			throw new InvalidOperationException("Snapshot guard rejected stage=" + stage + "; differences=" + differences.Count + "; " + string.Join(" | ", Enumerable.Take(differences, 12)));
		}
	}

	private static string ItemText(SnapshotItem item)
	{
		return "item=" + item.StableId + ", fixed=" + (!item.Movable) + ", pointer=" + item.Pointer;
	}

	private static void DiffItem<T>(List<string> differences, SnapshotItem item, string field, T before, T after)
	{
		if (!EqualityComparer<T>.Default.Equals(before, after))
		{
			differences.Add(ItemText(item) + ", " + field + ": old=" + ((object?)before ?? "null") + ", new=" + ((object?)after ?? "null"));
		}
	}
}
