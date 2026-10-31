using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuickSell;

// Uses the native confirmation host so native close/hotkey handling still sees
// a modal. Its original children and background are restored when it closes.
public sealed class ProtectedSaleDialog : MonoBehaviour
{
    private readonly List<(GameObject Object, bool Active)> originalChildren = new();
    private RectTransform? panel;
    private Image? background;
    private Color originalColor;
    private bool originalEnabled, originalRaycast, addedBackground;
    private Action? confirm;
    internal bool IsShowing { get; private set; }

    internal void Show(NegotiationTabletFrame frame, string[] items, string note, Action onConfirm)
    {
        Restore();
        foreach (Transform child in transform)
        {
            originalChildren.Add((child.gameObject, child.gameObject.activeSelf));
            child.gameObject.SetActive(false);
        }
        background = GetComponent<Image>();
        addedBackground = background == null;
        if (addedBackground) background = gameObject.AddComponent<Image>();
        originalColor = background!.color;
        originalEnabled = background.enabled;
        originalRaycast = background.raycastTarget;
        background.color = new Color(0, 0, 0, .18f);
        background.enabled = true;
        background.raycastTarget = true;
        IsShowing = true;
        confirm = onConfirm;

        float listHeight = items.Length * 34;
        float noteHeight = note.Contains("\n") ? 62 : 30;
        float noteY = 64 + listHeight + 14;
        float buttonY = noteY + noteHeight + 14;
        panel = TabletUI.NewRect(transform, "QuickSellProtectedSale", 0, 0, 430, buttonY + 62);
        panel.anchorMin = panel.anchorMax = panel.pivot = new Vector2(.5f, .5f);
        panel.anchoredPosition = Vector2.zero;
        var paper = TabletUI.NewFlat(panel, "Paper", 0, 0, 430, buttonY + 62, TabletStyle.SCREEN, true);
        TabletUI.Stretch(paper.rectTransform);
        TabletUI.NewStretched(panel, "Border", frame.frame2, TabletStyle.INK);
        var inner = TabletUI.NewStretched(panel, "InnerBorder", frame.frame2, TabletStyle.INK);
        TabletUI.Stretch(inner.rectTransform, 7);
        Label(frame, "出售受保护物品？", 22, 18, 386, 38, AssistantTypography.Title, TextAlignmentOptions.Midline);
        var list = TabletUI.NewRect(panel, "Items", 22, 64, 386, listHeight);
        TabletUI.NewStretched(list, "Border", frame.frame2, new Color(.25f, .27f, .34f, .5f));
        for (int i = 0; i < items.Length; i++)
        {
            string name = items[i];
            var row = Label(frame, name, 36, 64 + 34 * i, 358, 34, AssistantTypography.Body, TextAlignmentOptions.MidlineLeft);
            row.raycastTarget = true;
            TabletUI.AddTooltip(row.gameObject, () => name);
            if (i > 0) TabletUI.NewFlat(list, "Separator", 0, 34 * i, 386, 1, new Color(.25f, .27f, .34f, .2f));
        }
        var description = Label(frame, note, 22, noteY, 386, noteHeight, AssistantTypography.Caption, TextAlignmentOptions.Midline);
        description.enableWordWrapping = true;
        TabletUI.BigButton(frame, panel, 22, buttonY, "取消", () => gameObject.SetActive(false), 187);
        TabletUI.BigButton(frame, panel, 221, buttonY, "仍然出售", Confirm, 187);
        AssistantTypography.Buttons(panel);
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    internal void Confirm()
    {
        var action = confirm!;
        gameObject.SetActive(false);
        action();
    }

    private TextMeshProUGUI Label(TabletSkin skin, string value, float x, float y, float width, float height, float size, TextAlignmentOptions alignment)
    {
        return AssistantTypography.Create(skin, panel!, value, x, y, width, height, size, alignment);
    }

    private void OnDisable() => Restore();
    private void OnDestroy() => Restore();
    private void Restore()
    {
        if (!IsShowing) return;
        IsShowing = false;
        confirm = null;
        if (panel != null) { panel.gameObject.SetActive(false); Destroy(panel.gameObject); }
        panel = null;
        foreach (var child in originalChildren) if (child.Object != null) child.Object.SetActive(child.Active);
        originalChildren.Clear();
        if (background != null)
        {
            if (addedBackground) Destroy(background);
            else { background.color = originalColor; background.enabled = originalEnabled; background.raycastTarget = originalRaycast; }
        }
        background = null;
    }
}
