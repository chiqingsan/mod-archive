using System;
using System.Collections.Generic;

namespace SmartInventoryArrange.Core;

public sealed class LayoutItem
{
	public string StableId { get; init; } = "";


	public string ItemId { get; init; } = "";


	public string PrimaryCategory { get; init; } = "~unknown";


	public IReadOnlyList<string> Categories { get; init; } = Array.Empty<string>();


	public IReadOnlyList<Orientation> Orientations { get; init; } = Array.Empty<Orientation>();


	public ShapeMask? OriginalMask { get; init; }

	public int OriginalOrientation { get; init; }

	public int OriginalX { get; init; }

	public int OriginalY { get; init; }

	public int VisualPriority { get; init; } = 1000;


	public int Area => Orientations[0].Mask.Area;

	public int Complexity => Orientations[0].Mask.Width * Orientations[0].Mask.Height - Area;
}
