using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace QuickSell;

internal static class AssistantTypography
{
    internal const float Title = 28, Body = 20, Caption = 18;

    internal static TextMeshProUGUI Create(TabletSkin skin, Transform parent, string value,
        float x, float y, float width, float height, float size, TextAlignmentOptions alignment)
    {
        // Static mod text has no localization key. Avoid attaching an empty
        // LocalizeStringEvent to text built under an initially hidden window.
        var rect = TabletUI.NewRect(parent, "QuickSellText", x, y, width, height);
        var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.font = FontLoader.Instance.GetFontByRole(FontLoader.FontRole.Noto_Sans, LocalizationSettings.SelectedLocale);
        text.fontSize = size;
        text.enableAutoSizing = false;
        text.enableWordWrapping = false;
        text.alignment = alignment;
        text.color = TabletStyle.INK;
        text.raycastTarget = false;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.text = value;
        return text;
    }

    internal static void Buttons(Transform root)
    {
        foreach (var button in root.GetComponentsInChildren<TabletButton>(true))
        {
            button.label.enableAutoSizing = false;
            button.label.fontSize = Body;
            button.label.overflowMode = TextOverflowModes.Ellipsis;
        }
    }
}
