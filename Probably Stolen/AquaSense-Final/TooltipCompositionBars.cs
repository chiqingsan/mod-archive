using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AquaSense;

// Lives on the actual tooltip text object, so the bars follow its canvas, scale,
// clipping and lifetime. A TMP link supplies the final row position after layout.
public sealed class TooltipCompositionBars : MonoBehaviour
{
    internal const string LinkPrefix = "AquaSenseCompositionBar:";
    internal const int Width = 40;
    private static readonly HashSet<TooltipCompositionBars> Views = new();
    private readonly List<(Image Track, Image Fill)> _rows = new();
    private TextMeshProUGUI? _text;
    private bool _pending;
    private bool _hidePending;

    internal void Queue(TextMeshProUGUI text)
    {
        Views.Add(this);
        _text = text;
        _pending = true;
        _hidePending = false;
    }

    // Image creation/dirty registration must happen outside Canvas graphic rebuild.
    // The mesh hook queues final text coordinates; the next LateUpdate applies them.
    internal void RenderPending()
    {
        if (!_pending) return;
        _pending = false;
        Render(_text!);
    }

    // Hiding also touches UI objects, so it must wait for LateUpdate instead of
    // running inside TextMeshPro's rebuild loop.
    internal void RequestHide()
    {
        _pending = false;
        _hidePending = true;
    }

    private void LateUpdate() => Tick();

    internal void Tick()
    {
        if (_hidePending)
            Hide();

        RenderPending();
    }

    private void Render(TextMeshProUGUI text)
    {
        var count = 0;
        var info = text.textInfo;
        // The quality block can be requested on its own, so re-check the bar
        // toggle here instead of relying on the caller's branch.
        var barsEnabled = Core.ShowCompositionBars is { Value: true };
        for (var index = 0; barsEnabled && index < info.linkCount; index++)
        {
            var link = info.linkInfo[index];
            var id = link.GetLinkID();
            if (!id.StartsWith(LinkPrefix, StringComparison.Ordinal))
                continue;

            var values = id.Substring(LinkPrefix.Length).Split(':');
            var fraction = double.Parse(values[0], CultureInfo.InvariantCulture);
            var rgb = int.Parse(values[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var character = info.characterInfo[link.linkTextfirstCharacterIndex];
            if (count == _rows.Count)
            {
                var track = CreateImage("AquaSense Track", text.rectTransform);
                track.color = new Color(69f / 255f, 65f / 255f, 79f / 255f, 1f);
                var fill = CreateImage("AquaSense Fill", track.rectTransform);
                fill.rectTransform.anchorMin = Vector2.zero;
                fill.rectTransform.offsetMin = Vector2.zero;
                fill.rectTransform.offsetMax = Vector2.zero;
                _rows.Add((track, fill));
            }

            var row = _rows[count++];
            row.Track.gameObject.SetActive(true);
            var rect = row.Track.rectTransform;
            // Text mesh coordinates are relative to the text RectTransform's pivot.
            rect.anchorMin = rect.anchorMax = text.rectTransform.pivot;
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(character.origin,
                (character.ascender + character.descender) / 2f);
            rect.sizeDelta = new Vector2(Width, (character.ascender - character.descender) * 0.4f);
            row.Fill.color = new Color((rgb >> 16 & 255) / 255f,
                (rgb >> 8 & 255) / 255f, (rgb & 255) / 255f, 1f);
            // Keep traces visible while the percentage text retains the exact value.
            var fillRatio = fraction <= 0d ? 0f : fraction >= 1d ? 1f
                : (float)Math.Clamp(fraction, 0.5d / Width, 1d - 0.5d / Width);
            row.Fill.rectTransform.anchorMax = new Vector2(fillRatio, 1f);
        }
        for (var index = count; index < _rows.Count; index++)
            _rows[index].Track.gameObject.SetActive(false);

    }

    private static Image CreateImage(string name, Transform parent)
    {
        var node = new GameObject(name, typeof(RectTransform));
        node.layer = parent.gameObject.layer;
        node.transform.SetParent(parent, false);
        var image = node.AddComponent<Image>();
        image.raycastTarget = false;
        return image;
    }

    internal void Hide()
    {
        _pending = false;
        _hidePending = false;
        foreach (var row in _rows)
            row.Track.gameObject.SetActive(false);
    }

    internal static void HideAll()
    {
        foreach (var view in Views)
            if (view != null) view.Hide();
        Views.Clear();
    }

    private void OnDisable() => Hide();
    private void OnDestroy() => Views.Remove(this);
}
