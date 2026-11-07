namespace QuickSell;

internal enum MenuCellKind
{
    Empty,
    Text,
    Grid,
    Other,
}

/// <summary>物品右键菜单的布局识别（纯逻辑，可console 测试）。其它 mod 可能在原生菜单外包一层容器网格。</summary>
internal static class MenuStructure
{
    /// <summary>原生菜单：1 列 × 7 行的文本按钮列（GridPixelElement(1, 7)）。</summary>
    internal static bool IsNativeRow(int width, int height, System.Func<int, int, MenuCellKind> cell)
    {
        if (width != 1 || height != 7) return false;
        for (int y = 0; y < 7; y++)
            if (cell(0, y) != MenuCellKind.Text) return false;
        return true;
    }

    /// <summary>在容器网格中找“内层是原生菜单”的格子（其它 mod 的包裹形态）。</summary>
    internal static bool TryLocateWrappedRow(int width, int height, System.Func<int, int, MenuCellKind> cell,
        System.Func<int, int, bool> innerIsNative, out int x, out int y)
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (cell(col, row) != MenuCellKind.Grid || !innerIsNative(col, row)) continue;
                x = col;
                y = row;
                return true;
            }
        }
        x = 0;
        y = 0;
        return false;
    }
}
