using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UndergroundExchangeAssistant;

// The mod's own paper window. It follows the game's paper-window contract
// (CustomUIManager window + CustomUIWindowFrame + TabletSkin frames), so it reads
// as a proper in-game popup, but the layout, rows and exchange buttons are ours.
// The shipped exchange rows are used as icon and label artwork only.
internal sealed class ExchangeWindow
{
    private sealed class Row
    {
        internal string Id = "";
        internal GameObject Root = null!;
        internal TabletButton Button = null!;
        internal TabletTooltipTrigger Tip = null!;
        internal string TipText = "";
    }

    private const float Width = 720f;
    private const float RowLeft = 22f;
    private const float RowWidth = Width - RowLeft * 2f;
    private const float ButtonWidth = 96f;
    private const float ButtonZone = ButtonWidth + 28f;
    private const float RowTop = 104f;
    private const float RowGap = 5f;
    // The hint line and the paper border live here; the list itself is trimmed
    // to its own rows so the sheet does not end in an empty band.
    private const float BottomReserve = 56f;
    private const float MinHeight = 460f;
    private const float MaxHeight = 800f;

    private static readonly Color InkSoft = new(.40f, .31f, .40f, 1f);
    internal static readonly Color PaperColor = new(190f / 255, 159f / 255, 158f / 255, 1f);
    internal static readonly Color InkColor = new(69f / 255, 57f / 255, 69f / 255, 1f);

    private CustomUIWindow? window;
    private readonly List<Row> rows = new();
    private readonly List<GameObject> clones = new();
    private Action<string>? onExchange;
    private Action? onClose;
    private float sheetHeight = MaxHeight;

    internal bool IsOpen => window != null && window.IsAlive;

    internal IReadOnlyList<string> RowIds
    {
        get
        {
            var ids = new List<string>(rows.Count);
            foreach (var row in rows) ids.Add(row.Id);
            return ids;
        }
    }

    internal void Open(TabletSkin skin, IReadOnlyList<(string Id, Transform Source)> offers,
        Func<string, string> tooltip, Action<string> exchange, Action close)
    {
        if (IsOpen || offers.Count == 0) return;
        onExchange = exchange;
        onClose = close;
        sheetHeight = SheetHeight(offers);

        var template = new GameObject("UndergroundExchangeSheetTemplate",
            typeof(RectTransform), typeof(CustomUIWindowFrame));
        template.SetActive(false);
        window = CustomUIManager.Instance.CreateWindow("underground_exchange_sheet", null, "paper", template)
            .SetSize(Width, sheetHeight).Center().SetDraggable(true).SetCloseOnEscape()
            .OnClose(() => { window = null; var callback = onClose; onClose = null; callback?.Invoke(); })
            .GetWindow();
        UnityEngine.Object.Destroy(template);

        var root = window.Rect;
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(.5f, .5f);
        var canvas = root.GetComponentInParent<Canvas>();
        var area = canvas != null && canvas.rootCanvas != null
            ? canvas.rootCanvas.GetComponent<RectTransform>().rect
            : new Rect(0, 0, 1600, 900);
        float scale = Mathf.Min(1f, (area.width - 32f) / Width, (area.height - 32f) / sheetHeight);
        scale = Mathf.Max(scale, .35f);
        root.localScale = new Vector3(scale, scale, 1f);
        foreach (var storeUI in root.GetComponents<StoreUI>()) storeUI.zoomable = false;
        root.GetComponent<UIDragHandler>()?.SetHomeScale(scale);

        BuildSheet(root, skin, offers, tooltip);
        window.Show();
        window.SetTitle("地下交易所材料单");
    }

    private static float NativeRowHeight(Transform source)
    {
        return Mathf.Clamp((source as RectTransform)?.rect.height ?? 50f, 50f, 80f);
    }

    // Header + the shipped rows at their own heights + the hint band. Ten rows
    // come out around 730px, so the old fixed 780px sheet lost its bottom band.
    private static float SheetHeight(IReadOnlyList<(string Id, Transform Source)> offers)
    {
        float total = 0f;
        for (int i = 0; i < offers.Count; i++) total += NativeRowHeight(offers[i].Source);
        float height = RowTop + total + RowGap * Mathf.Max(0, offers.Count - 1) + BottomReserve;
        return Mathf.Clamp(height, MinHeight, MaxHeight);
    }

    private void BuildSheet(RectTransform sheet, TabletSkin skin,
        IReadOnlyList<(string Id, Transform Source)> offers, Func<string, string> tooltip)
    {
        // Same construction as the game's paper windows: a bezel, a frame2 border
        // and a SCREEN paper face, then our own content on top.
        var bezel = TabletUI.NewFlat(sheet, "BezelFill", 0, 0, Width, sheetHeight,
            InkSoft, true);
        TabletUI.Stretch(bezel.rectTransform);
        var bezelBorder = TabletUI.NewStretched(sheet, "BezelBorder", skin.frame2, TabletStyle.INK);
        TabletUI.Stretch(bezelBorder.rectTransform);

        var paper = TabletUI.NewFlat(sheet, "PaperFill", 10, 10, Width - 20, sheetHeight - 20,
            PaperColor, true);
        TabletUI.Stretch(paper.rectTransform, 10);
        var paperBorder = TabletUI.NewStretched(sheet, "PaperBorder", skin.frame2, TabletStyle.INK);
        TabletUI.Stretch(paperBorder.rectTransform, 10);

        var title = TabletUI.NewText(skin, sheet, "Title", 0, 12, Width, 40,
            FontLoader.FontRole.Noto_Sans, 26, InkColor, TextAlignmentOptions.Center);
        title.text = "地下交易所材料单";
        if (window != null && window.frame != null) window.frame.titleText = title;
        var subtitle = TabletUI.NewText(skin, sheet, "Subtitle", 0, 50, Width, 18,
            FontLoader.FontRole.Noto_Sans, 12, InkSoft, TextAlignmentOptions.Center);
        subtitle.text = "每晚一次 · 材料投入垃圾桶后由夜间清洁服务结算";
        TabletUI.NewFlat(sheet, "HeaderRule", RowLeft, 78, RowWidth, 2, InkSoft);

        // Each shipped row keeps its native height (the smuggler row is taller
        // than the rest). Stack them with a small even gap; only compress when
        // the window hits its maximum height.
        float available = sheetHeight - BottomReserve - RowTop;
        var heights = new float[offers.Count];
        float total = RowGap * Mathf.Max(0, offers.Count - 1);
        for (int i = 0; i < offers.Count; i++)
        {
            heights[i] = NativeRowHeight(offers[i].Source);
            total += heights[i];
        }
        float fitAll = total > available ? available / total : 1f;
        float y = RowTop;
        for (int i = 0; i < offers.Count; i++)
        {
            var (id, source) = offers[i];
            float rowHeight = heights[i] * fitAll;
            BuildRow(sheet, skin, id, source, y, rowHeight, tooltip);
            y += rowHeight + RowGap * fitAll;
        }

        var hint = TabletUI.NewText(skin, sheet, "Hint", RowLeft, sheetHeight - 38, RowWidth, 16,
            FontLoader.FontRole.Noto_Sans, 11, InkSoft, TextAlignmentOptions.Center);
        hint.text = "检索玩家各库存与内层容器；优先耗尽滤芯、损坏模组，不取机器内物品。";
        var close = TabletUI.SmallButton(skin, sheet, Width - 68, 16, 44, 34, "×", () => onClose?.Invoke());
        close.label.fontSize = 20;
        close.label.enableAutoSizing = false;
    }

    private void BuildRow(RectTransform window, TabletSkin skin, string id, Transform source,
        float y, float height, Func<string, string> tooltip)
    {
        var rowRect = TabletUI.NewRect(window, "Row_" + id, RowLeft, y, RowWidth, height);
        TabletUI.NewFlat(rowRect, "Rule", 0, height - 1, RowWidth, 1, InkSoft);

        // Reuse the shipped row artwork for the item icons and their labels. It
        // keeps its native scale (494x50) so icons, notes and fonts look exactly
        // like the native exchange list instead of a shrunken copy.
        try
        {
            var clone = UnityEngine.Object.Instantiate(source.gameObject, rowRect, false);
            clone.name = "Art_" + id;
            clone.SetActive(true);
            clones.Add(clone);
            var art = (RectTransform)clone.transform;
            art.anchorMin = art.anchorMax = new Vector2(0f, .5f);
            art.pivot = new Vector2(0f, .5f);
            art.anchoredPosition = new Vector2(10f, 0f);
            // The shipped row is drawn on the native panel's list background, so
            // its own backplate would cover the mod's paper and make the list
            // look grey against the bright header. Only the icons and notes are
            // kept; the row background and the click surfaces are disabled.
            foreach (var graphic in clone.GetComponents<Graphic>()) graphic.enabled = false;
            foreach (var clonedGraphic in clone.GetComponentsInChildren<Graphic>(true))
                clonedGraphic.raycastTarget = false;
            foreach (var label in clone.GetComponentsInChildren<TextMeshProUGUI>(true))
                if (label.name.StartsWith("CountTMP", StringComparison.Ordinal))
                    label.gameObject.SetActive(false);
            // Keep the shipped artwork at its native size, down to the row height
            // when the whole list had to be compressed.
            var native = art.rect.size;
            float fit = 1f;
            if (native.x > 1f)
                fit = Mathf.Min(fit, (RowWidth - ButtonZone - 20f) / native.x);
            if (native.y > 1f)
                fit = Mathf.Min(fit, height / native.y);
            art.localScale = Vector3.one * Mathf.Clamp(fit, .55f, 1f);
        }
        catch (Exception error)
        {
            Debug.LogWarning("[UndergroundExchange] 交易行美术复用失败，改用文字：" + error.Message);
            var fallback = TabletUI.NewText(skin, rowRect, "Text", 14, (height - 22) * .5f,
                RowWidth - ButtonZone, height - 10, FontLoader.FontRole.Noto_Sans, 16,
                TabletStyle.INK, TextAlignmentOptions.MidlineLeft, true);
            fallback.text = id;
        }

        var row = new Row { Id = id, Root = rowRect.gameObject };
        float buttonY = (height - 28f) * .5f;
        var button = TabletUI.SmallButton(skin, rowRect, RowWidth - ButtonWidth - 18f, buttonY,
            ButtonWidth, 28f, "交换", () => onExchange?.Invoke(id));
        button.label.fontSize = 15;
        button.label.enableAutoSizing = false;
        // Only an unavailable row explains itself on hover; a working row would
        // just repeat the row header, so it stays silent.
        row.Tip = TabletUI.AddTooltip(button.gameObject, () => row.TipText);
        var caption = TabletUI.NewText(skin, rowRect, "NightLimit", RowWidth - ButtonWidth - 18f,
            buttonY + 30f, ButtonWidth, 12f, FontLoader.FontRole.Noto_Sans, 10, InkSoft,
            TextAlignmentOptions.Center);
        caption.text = "每晚一次";
        row.Button = button;
        rows.Add(row);
    }

    internal void SetRowState(string id, bool enabled, string label, string tip)
    {
        foreach (var row in rows)
        {
            if (row.Id != id) continue;
            row.Button.SetInteractable(enabled);
            row.Button.SetLabel(label);
            row.TipText = enabled ? "" : tip;
            return;
        }
    }

    internal void Close()
    {
        rows.Clear();
        clones.Clear();
        onExchange = null;
        onClose = null;
        if (window == null) return;
        if (window.IsAlive) window.Close();
        window = null;
    }
}
