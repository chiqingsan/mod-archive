using System;
using System.Collections.Generic;

namespace SmartInventoryArrange.Core;

public sealed class LayoutProblem
{
	public int Width { get; init; }

	public int Height { get; init; }

	public bool[,] Usable { get; init; } = new bool[0, 0];


	public IReadOnlyList<Placement> Fixed { get; init; } = Array.Empty<Placement>();


	public IReadOnlyList<LayoutItem> Items { get; init; } = Array.Empty<LayoutItem>();


	public IReadOnlyDictionary<string, int> CategoryOrder { get; init; } = (IReadOnlyDictionary<string, int>)new Dictionary<string, int>((IEqualityComparer<string>)StringComparer.Ordinal);

}
