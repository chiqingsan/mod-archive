using System;
using System.Collections.Generic;
using System.Linq;
using SmartInventoryArrange.Core;

namespace SmartInventoryArrange;

public static class SupplyCrateOrientationPolicy
{
	public static IReadOnlyList<Orientation> Allowed(IReadOnlyList<string> categories, IReadOnlyList<Orientation> nativeOrientations)
	{
		if (!categories.Contains("SUPPLY_CRATE", (IEqualityComparer<string>?)StringComparer.Ordinal))
		{
			return nativeOrientations;
		}
		Orientation[] array = Enumerable.ToArray(Enumerable.Where(nativeOrientations, (Orientation o) => o.GameOrientation == 0));
		if (array.Length != 1 || array[0].Mask.Width < array[0].Mask.Height)
		{
			throw new InvalidOperationException("Supply crate has no unique native default-horizontal orientation; captured inventory unchanged.");
		}
		return array;
	}
}
