using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace QuickSell;

public sealed partial class QuickSellMod
{
    private PixelWindow? protectionWindow;
    private GridPixelElement? originalMenu, protectionMenu;
    private RichTextElement? protectionButton;
    private RichTextElement[] nativeMenuButtons = System.Array.Empty<RichTextElement>();

    internal void PrepareProtectionMenu(PixelWindow window, GameItem item)
    {
        if (protectionWindow != window)
        {
            RestoreProtectionMenu();
            protectionWindow = window;
            originalMenu = (GridPixelElement)window.childElement;
            nativeMenuButtons = Enumerable.Range(0, 7).Select(i => (RichTextElement)originalMenu.GetElement(0, i)).ToArray();
            originalMenu.DetachAll();
            protectionMenu = new GridPixelElement(1, 8);
            foreach (var button in nativeMenuButtons) protectionMenu.Attach(button);
            protectionButton = new RichTextElement();
            protectionMenu.Attach(protectionButton);
            window.Attach(protectionMenu);
            originalMenu.handler.gameObject.SetActive(false);
            foreach (var child in protectionMenu.handler.GetComponentsInChildren<Transform>(true))
                child.gameObject.tag = "ContextMenuNode";
        }
        UpdateProtectionMenu(item, null);
        protectionButton!.SetFixedSize(-1, -1);
        int width = System.Math.Max(protectionButton.widthPixels, nativeMenuButtons.Max(button => button.widthPixels));
        foreach (var button in nativeMenuButtons) button.SetFixedSize(width, -1);
        protectionButton.SetFixedSize(width, -1);
        protectionMenu!.Validate();
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
        if (protectionWindow != null && protectionWindow.handler != null && originalMenu != null)
        {
            protectionWindow.Hide();
            protectionMenu!.DetachAll();
            foreach (var button in nativeMenuButtons) originalMenu.Attach(button);
            originalMenu.handler.gameObject.SetActive(true);
            protectionWindow.Attach(originalMenu);
            originalMenu.Validate();
            protectionWindow.Validate();
            UnityEngine.Object.Destroy(protectionButton!.handler.gameObject);
            UnityEngine.Object.Destroy(protectionMenu.handler.gameObject);
        }
        protectionWindow = null; originalMenu = null; protectionMenu = null; protectionButton = null;
        nativeMenuButtons = System.Array.Empty<RichTextElement>();
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
