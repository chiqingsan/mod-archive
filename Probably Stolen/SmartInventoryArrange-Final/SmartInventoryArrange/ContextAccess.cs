using System;
using System.Reflection;

namespace SmartInventoryArrange;

internal static class ContextAccess
{
    private const BindingFlags InstancePrivate = BindingFlags.Instance | BindingFlags.NonPublic;

    private static T Field<T>(ItemContextHandler owner, string name) where T : class
    {
        return (T)typeof(ItemContextHandler).GetField(name, InstancePrivate)!.GetValue(owner)!;
    }

    internal static PixelWindow SortWindow(ItemContextHandler owner) => Field<PixelWindow>(owner, "sortWindow");
    internal static GameGridInventory? CurrentInventory(ItemContextHandler owner) => (GameGridInventory?)typeof(ItemContextHandler).GetField("currentInventory", InstancePrivate)!.GetValue(owner);
    internal static RichTextElement SortBigStart(ItemContextHandler owner) => Field<RichTextElement>(owner, "buttonSortBigStart");
    internal static RichTextElement SortSmallStart(ItemContextHandler owner) => Field<RichTextElement>(owner, "buttonSortSmallStart");
    internal static RichTextElement SortBigEnd(ItemContextHandler owner) => Field<RichTextElement>(owner, "buttonSortBigEnd");
    internal static RichTextElement SortSmallEnd(ItemContextHandler owner) => Field<RichTextElement>(owner, "buttonSortSmallEnd");
    internal static RichTextElement SortUndo(ItemContextHandler owner) => Field<RichTextElement>(owner, "buttonSortUndo");
    internal static RenderHandler.ColorPalette TextColor(ItemContextHandler owner) => (RenderHandler.ColorPalette)typeof(ItemContextHandler).GetField("textColor", InstancePrivate)!.GetValue(owner)!;
    internal static int GridXSpacing(GridPixelElement grid) => (int)typeof(GridPixelElement).GetField("_xSpacing", InstancePrivate)!.GetValue(grid)!;
    internal static int GridYSpacing(GridPixelElement grid) => (int)typeof(GridPixelElement).GetField("_ySpacing", InstancePrivate)!.GetValue(grid)!;
    internal static int LeftPadding(RichTextElement element) => (int)typeof(RichTextElement).GetField("leftPadding", InstancePrivate)!.GetValue(element)!;
    internal static int RightPadding(RichTextElement element) => (int)typeof(RichTextElement).GetField("rightPadding", InstancePrivate)!.GetValue(element)!;
    internal static int TopPadding(RichTextElement element) => (int)typeof(RichTextElement).GetField("topPadding", InstancePrivate)!.GetValue(element)!;
    internal static int BottomPadding(RichTextElement element) => (int)typeof(RichTextElement).GetField("bottomPadding", InstancePrivate)!.GetValue(element)!;
}
