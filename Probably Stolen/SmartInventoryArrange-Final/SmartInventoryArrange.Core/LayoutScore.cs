using System;

namespace SmartInventoryArrange.Core;

public readonly record struct LayoutScore(int PlacedCount, int PlacedArea, int NegativeTypeRuns, int SameTypeRowAlignment, int SameTypeHorizontalContact, int NegativeCohortRuns, int CohortRowAlignment, int CohortHorizontalContact, int OrientationConsistency, int VerticalPreference, int PriorityOrderAlignment, int CategoryOrderAlignment, int LargeFirstAlignment, int SameTypeContact, int SameCategoryContact, int TopLeftCompactness, int RowRegularity, int NegativeFragments, int NegativeInterleaving, int NegativeRotations, string StableTie) : IComparable<LayoutScore>
{
	public int CompareTo(LayoutScore o)
	{
		int[] array = new int[20]
		{
			PlacedCount, PlacedArea, NegativeTypeRuns, SameTypeRowAlignment, SameTypeHorizontalContact, NegativeCohortRuns, CohortRowAlignment, CohortHorizontalContact, OrientationConsistency, VerticalPreference,
			PriorityOrderAlignment, CategoryOrderAlignment, LargeFirstAlignment, SameTypeContact, SameCategoryContact, TopLeftCompactness, RowRegularity, NegativeFragments, NegativeInterleaving, NegativeRotations
		};
		int[] array2 = new int[20]
		{
			o.PlacedCount, o.PlacedArea, o.NegativeTypeRuns, o.SameTypeRowAlignment, o.SameTypeHorizontalContact, o.NegativeCohortRuns, o.CohortRowAlignment, o.CohortHorizontalContact, o.OrientationConsistency, o.VerticalPreference,
			o.PriorityOrderAlignment, o.CategoryOrderAlignment, o.LargeFirstAlignment, o.SameTypeContact, o.SameCategoryContact, o.TopLeftCompactness, o.RowRegularity, o.NegativeFragments, o.NegativeInterleaving, o.NegativeRotations
		};
		for (int i = 0; i < array.Length; i++)
		{
			int num = array[i].CompareTo(array2[i]);
			if (num != 0)
			{
				return num;
			}
		}
		return -StringComparer.Ordinal.Compare(StableTie, o.StableTie);
	}
}
