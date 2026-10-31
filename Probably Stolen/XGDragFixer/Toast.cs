using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XGDragFixer;

/// <summary>
/// State-change hints (A/B switch, diagnostics messages). Prefers the game's own
/// notification bar (StoreUIManager.Notify, the same call other local mods such as
/// AquaSense use) and only falls back to a private overlay when the game UI is not
/// ready yet (startup). The overlay is created lazily and never blocks input.
/// </summary>
internal static class Toast
{
    private static GameObject? _root;
    private static CanvasGroup? _group;
    private static TextMeshProUGUI? _text;
    private static float _hideAt;
    private static bool _fontWarningLogged;

    internal static void Show(string message, float seconds = 2.5f)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        if (TryShowNative(message))
        {
            return;
        }

        ShowOverlay(message, seconds);
    }

    /// <summary>The game's own notification style (see AquaSense's filter-replacement toast).</summary>
    private static bool TryShowNative(string message)
    {
        try
        {
            var ui = StoreUIManager.Instance;
            if (ui == null)
            {
                return false;
            }

            ui.Notify(message, "default");
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void ShowOverlay(string message, float seconds)
    {
        try
        {
            Ensure();
            if (_text == null || _group == null)
            {
                return;
            }

            if (_text.font == null)
            {
                _text.font = GetFont();
            }

            _text.text = message;
            _text.gameObject.SetActive(true);
            _group.alpha = 1f;
            _hideAt = Time.realtimeSinceStartup + Mathf.Max(0.3f, seconds);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"提示条显示失败: {ex.GetType().Name} {ex.Message}");
        }
    }

    internal static void Update()
    {
        if (_text == null || _group == null || !_text.gameObject.activeSelf)
        {
            return;
        }

        var left = _hideAt - Time.realtimeSinceStartup;
        if (left <= 0f)
        {
            _text.gameObject.SetActive(false);
            return;
        }

        _group.alpha = Mathf.Clamp01(left / 0.45f);
    }

    private static void Ensure()
    {
        if (_root != null)
        {
            return;
        }

        _root = new GameObject("BetterPerformanceToast");
        Object.DontDestroyOnLoad(_root);

        var canvas = _root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32000;

        var scaler = _root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        _group = _root.AddComponent<CanvasGroup>();
        _group.blocksRaycasts = false;
        _group.interactable = false;

        var textGo = new GameObject("text");
        textGo.transform.SetParent(_root.transform, false);
        var rect = textGo.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -56f);
        rect.sizeDelta = new Vector2(1400f, 100f);

        _text = textGo.AddComponent<TextMeshProUGUI>();
        _text.font = GetFont();
        _text.fontSize = 28f;
        _text.fontStyle = FontStyles.Bold;
        _text.color = new Color(1f, 0.93f, 0.65f, 1f);
        _text.alignment = TextAlignmentOptions.Center;
        _text.horizontalAlignment = HorizontalAlignmentOptions.Center;
        _text.verticalAlignment = VerticalAlignmentOptions.Middle;
        _text.raycastTarget = false;
        textGo.SetActive(false);

        if (_text.font == null && !_fontWarningLogged)
        {
            _fontWarningLogged = true;
            Core.Log?.Warning("提示条字体未找到（仍会写入日志，屏幕提示可能不显示）");
        }
    }

    private static TMP_FontAsset? GetFont()
    {
        try
        {
            var loader = FontLoader.Instance;
            if (loader != null)
            {
                var locale = LocalizationSettings.SelectedLocale;
                if (locale != null)
                {
                    return loader.GetFontByRole(FontLoader.FontRole.Noto_Sans, locale);
                }

                return loader.notoSans != null ? loader.notoSans : loader.westernFont;
            }
        }
        catch
        {
            // The FontLoader lives in the game scene; retry on the next show.
        }

        try
        {
            return TMP_Settings.defaultFontAsset;
        }
        catch
        {
            return null;
        }
    }
}
