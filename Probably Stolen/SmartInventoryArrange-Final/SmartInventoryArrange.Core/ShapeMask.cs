using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartInventoryArrange.Core;

public sealed class ShapeMask
{
	public int Width { get; }

	public int Height { get; }

	public IReadOnlyList<Cell> Cells { get; }

	public int Area => Cells.Count;

	public string Signature { get; }

	public ShapeMask(IEnumerable<Cell> cells)
	{
		Cell[] array = Enumerable.ToArray(Enumerable.OrderBy(Enumerable.Distinct(cells), (Cell c) => c.Y).ThenBy((Cell c) => c.X));
		if (array.Length == 0)
		{
			throw new ArgumentException("Shape must contain at least one cell.");
		}
		int minX = Enumerable.Min(array, (Cell c) => c.X);
		int minY = Enumerable.Min(array, (Cell c) => c.Y);
		Cell[] source = (Cell[])(Cells = Enumerable.ToArray(Enumerable.OrderBy(Enumerable.Select(array, (Cell c) => new Cell(c.X - minX, c.Y - minY)), (Cell c) => c.Y).ThenBy((Cell c) => c.X)));
		Width = Enumerable.Max(source, (Cell c) => c.X) + 1;
		Height = Enumerable.Max(source, (Cell c) => c.Y) + 1;
		Signature = string.Join(";", Enumerable.Select(source, delegate(Cell c)
		{
			DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
			val = new DefaultInterpolatedStringHandler(1, 2);
			val.AppendFormatted<int>(c.X);
			val.AppendLiteral(",");
			val.AppendFormatted<int>(c.Y);
			return val.ToStringAndClear();
		}));
	}

	public ShapeMask Rotate90()
	{
		return new ShapeMask(Enumerable.Select(Cells, (Cell c) => new Cell(Height - 1 - c.Y, c.X)));
	}
}
