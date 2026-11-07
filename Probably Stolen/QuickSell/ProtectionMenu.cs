using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace QuickSell;

public sealed partial class QuickSellMod
{
    private PixelWindow? protectionWindow, protectionMenuSkippedFor;
    private GridPixelElement? originalMenu, protectionMenu, menuContainer;
    private int menuContainerX, menuContainerY;
    private RichTextElement? protectionButton;
    private RichTextElement[] nativeMenuButtons = System.Array.Empty<RichTextElement>();

    internal void PrepareProtectionMenu(PixelWindow window, GameItem item)
    {
        if (!menuProtection.Value)
        {
            // 关闭时不碰原生菜单：已接管过的窗口还原，未接管过的保持原样。
            if (protectionWindow != null) RestoreProtectionMenu();
            return;
        }
        if (protectionMenuSkippedFor == window) return;
        if (protectionWindow != window)
        {
            try
            {
                InstallProtectionMenu(window);
            }
            catch (System.Exception error)
            {
                try { RestoreProtectionMenu(); } catch (System.Exception) { }
                SkipProtectionMenu(window, error.GetType().Name);
            }
            if (protectionWindow != window) return;
        }
        UpdateProtectionMenu(item, null);
        protectionButton!.SetFixedSize(-1, -1);
        int width = System.Math.Max(protectionButton.widthPixels, nativeMenuButtons.Max(button => button.widthPixels));
        foreach (var button in nativeMenuButtons) button.SetFixedSize(width, -1);
        protectionButton.SetFixedSize(width, -1);
        protectionMenu!.Validate();
    }

    private void InstallProtectionMenu(PixelWindow window)
    {
        RestoreProtectionMenu();
        menuContainer = null;
        menuContainerX = 0;
        menuContainerY = 0;
        GridPixelElement? nativeMenu = null;
        if (window.childElement is GridPixelElement root)
        {
            if (MenuStructure.IsNativeRow(root.gridWidth, root.gridHeight, (x, y) => CellKind(root.GetElement(x, y))))
            {
                nativeMenu = root;
            }
            else if (MenuStructure.TryLocateWrappedRow(root.gridWidth, root.gridHeight,
                         (x, y) => CellKind(root.GetElement(x, y)),
                         (x, y) => root.GetElement(x, y) is GridPixelElement inner
                                   && MenuStructure.IsNativeRow(inner.gridWidth, inner.gridHeight, (ix, iy) => CellKind(inner.GetElement(ix, iy))),
                         out menuContainerX, out menuContainerY))
            {
                nativeMenu = (GridPixelElement)root.GetElement(menuContainerX, menuContainerY);
                menuContainer = root;
            }
        }
        if (nativeMenu == null)
        {
            SkipProtectionMenu(window, "未识别的菜单结构 " + DescribeStructure(window));
            return;
        }

        originalMenu = nativeMenu;
        protectionWindow = window;
        nativeMenuButtons = Enumerable.Range(0, nativeMenu.gridHeight).Select(i => (RichTextElement)nativeMenu.GetElement(0, i)).ToArray();
        originalMenu.DetachAll();
        protectionMenu = new GridPixelElement(nativeMenu.gridWidth, nativeMenu.gridHeight + 1);
        foreach (var button in nativeMenuButtons) protectionMenu.Attach(button);
        protectionButton = new RichTextElement();
        protectionMenu.Attach(protectionButton);
        if (menuContainer == null)
        {
            window.Attach(protectionMenu);
        }
        else
        {
            if (!menuContainer.Detach(nativeMenu) || !menuContainer.AttachPos(protectionMenu, menuContainerX, menuContainerY))
                throw new System.InvalidOperationException("菜单容器替换失败");
            menuContainer.Validate();
            window.Validate();
        }
        originalMenu.handler.gameObject.SetActive(false);
        foreach (var child in protectionMenu.handler.GetComponentsInChildren<Transform>(true))
            child.gameObject.tag = "ContextMenuNode";
    }

    private static MenuCellKind CellKind(PixelElement? element)
        => element == null ? MenuCellKind.Empty
            : element is RichTextElement ? MenuCellKind.Text
            : element is GridPixelElement ? MenuCellKind.Grid
            : MenuCellKind.Other;

    /// <summary>降级日志用：描述当前窗口的实际结构，便于排查其它 Mod 的改写形态。</summary>
    private static string DescribeStructure(PixelWindow window)
    {
        var child = window.childElement;
        return child == null ? "（空菜单）"
            : child is GridPixelElement grid ? $"（{grid.gridWidth}×{grid.gridHeight} 网格）"
            : $"（{child.GetType().Name}）";
    }

    private void SkipProtectionMenu(PixelWindow window, string reason)
    {
        protectionWindow = null;
        protectionMenuSkippedFor = window;
        Debug.LogWarning($"[QuickSell] 物品右键菜单结构无法识别（{reason}），已跳过右键保护按钮；仍可在面板中保护物品。");
    }

    internal void UpdateProtectionMenu(GameItem item, RichTextElement? selected)
    {
        if (protectionButton == null || item == null) return;
        bool owned = GeneralHelper.IsItemOwned(item);
        var color = !owned ? RenderHandler.ColorPalette.DarkGray
            : selected == protectionButton ? RenderHandler.ColorPalette.VeryLightGray : RenderHandler.ColorPalette.LightGray;
        bool nameProtected = IsNameProtected(item);
        bool instanceProtected = item.uniqueId > 0 && protectedIds.Contains(item.uniqueId);
        bool released = item.uniqueId > 0 && unprotectedIds.Contains(item.uniqueId);
        protectionButton.SetText(!released && (instanceProtected || nameProtected) ? "解保" : "保护", 10000, color);
    }

    internal bool TriggerProtectionMenu(ItemContextHandler handler, GameItem item, RichTextElement button)
    {
        if (button != protectionButton) return true;
        if (item != null && GeneralHelper.IsItemOwned(item))
        {
            handler.Stop();
            int id = item.uniqueId;
            // 本身受保护：解保；仅受同类保护：单件解保；已单件解保：恢复保护。
            if (id > 0 && unprotectedIds.Contains(id)) RestoreProtected(item);
            else if (id > 0 && protectedIds.Contains(id)) Unprotect(id);
            else if (IsNameProtected(item)) ReleaseProtected(item);
            else Protect(item);
        }
        return false;
    }

    private void RestoreProtectionMenu()
    {
        if (protectionWindow != null && originalMenu != null && originalMenu.handler != null)
        {
            if (protectionWindow.handler != null) protectionWindow.Hide();
            if (protectionMenu != null)
            {
                protectionMenu.DetachAll();
                if (menuContainer != null && menuContainer.handler != null) menuContainer.Detach(protectionMenu);
            }
            foreach (var button in nativeMenuButtons) originalMenu.Attach(button);
            originalMenu.handler.gameObject.SetActive(true);
            if (menuContainer != null && menuContainer.handler != null)
            {
                menuContainer.AttachPos(originalMenu, menuContainerX, menuContainerY);
                menuContainer.Validate();
            }
            else if (protectionWindow.handler != null)
            {
                protectionWindow.Attach(originalMenu);
            }
            originalMenu.Validate();
            if (protectionWindow.handler != null) protectionWindow.Validate();
            if (protectionMenu?.handler != null) UnityEngine.Object.Destroy(protectionMenu.handler.gameObject);
        }
        if (protectionButton?.handler != null) UnityEngine.Object.Destroy(protectionButton.handler.gameObject);
        protectionWindow = null; originalMenu = null; protectionMenu = null; protectionButton = null;
        menuContainer = null; nativeMenuButtons = System.Array.Empty<RichTextElement>();
        protectionMenuSkippedFor = null;
    }
}

[HarmonyPatch(typeof(ItemContextHandler), "EqualizeButtonWidths")]
internal static class ProtectionMenuLayoutPatch
{
    private static void Postfix(PixelWindow ___contextWindow, GameItem ___currentItem)
        => QuickSellMod.Current?.PrepareProtectionMenu(___contextWindow, ___currentItem);
}

[HarmonyPatch(typeof(ItemContextHandler), "UpdateSelectButtonColor")]
internal static class ProtectionMenuHoverPatch
{
    private static void Postfix(GameItem ___currentItem, RichTextElement selectedButton)
        => QuickSellMod.Current?.UpdateProtectionMenu(___currentItem, selectedButton);
}

[HarmonyPatch(typeof(ItemContextHandler), "TriggerButton")]
internal static class ProtectionMenuClickPatch
{
    private static bool Prefix(ItemContextHandler __instance, GameItem ___currentItem, RichTextElement button)
        => QuickSellMod.Current == null || QuickSellMod.Current.TriggerProtectionMenu(__instance, ___currentItem, button);
}
