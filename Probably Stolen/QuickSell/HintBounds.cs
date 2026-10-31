namespace QuickSell;

internal static class HintBounds
{
    internal static (int X, int Y, int Width, int Height) Calculate(GridShape shape, int grid, bool preview)
        => (preview ? 0 : shape.minX * grid, preview ? 0 : -shape.minY * grid,
            shape.globalWidth * grid, shape.globalHeight * grid);
}
