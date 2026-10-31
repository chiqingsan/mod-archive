using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// OverlayHandler.ShowTooltip repositions and re-validates the tooltip window on
/// every call, even when the caller passes the exact same (cached) RichText at the
/// same mouse position. Skip those redundant reparses while the text is unchanged;
/// any position/translation/width change, window close or frame timeout falls back
/// to the vanilla path.
/// </summary>
internal static class TooltipIdleSkip
{
    private static RichText? _lastRich;
    private static Vector2 _lastPosition;
    private static Vector2 _lastTranslation;
    private static int _lastWidth;
    private static int _lastFrame = -1000;

    /// <summary>
    /// Whether the last prefix call let the vanilla body run. Harmony still runs
    /// the postfix after a prefix returned false, so the postfix must not refresh
    /// <see cref="_lastFrame"/> for skipped calls — otherwise the timeout would be
    /// pushed forward every frame and the tooltip would freeze until hover ends.
    /// </summary>
    private static bool _ranOriginal = true;

    internal static void Reset()
    {
        _lastRich = null;
        _lastFrame = -1000;
        _ranOriginal = true;
    }

    internal static bool Prefix(Vector2 position, Vector2 localTranslation, RichText? richText, int maxCharWidth)
    {
        var run = ShouldRun(position, localTranslation, richText, maxCharWidth);
        _ranOriginal = run;
        return run;
    }

    private static bool ShouldRun(Vector2 position, Vector2 localTranslation, RichText? richText, int maxCharWidth)
    {
        if (Core.TooltipIdleSkip?.Value != true || richText == null)
        {
            return true;
        }

        OverlayHandler? overlay = OverlayHandler.current;
        if (overlay == null)
        {
            return true;
        }

        PixelWindow? window = overlay.GetTooltipWindow();
        if (window == null || !window.IsVisible())
        {
            _lastRich = null;
            return true;
        }

        var frames = Core.TooltipIdleFrames?.Value ?? 6;
        if (Time.frameCount - _lastFrame > frames)
        {
            return true;
        }

        if (!ReferenceEquals(_lastRich, richText))
        {
            return true;
        }

        if ((_lastPosition - position).sqrMagnitude > 1f)
        {
            return true;
        }

        if ((_lastTranslation - localTranslation).sqrMagnitude > 0.001f)
        {
            return true;
        }

        if (_lastWidth != maxCharWidth)
        {
            return true;
        }

        return false;
    }

    internal static void Postfix(Vector2 position, Vector2 localTranslation, RichText? richText, int maxCharWidth)
    {
        if (!_ranOriginal)
        {
            _ranOriginal = true;
            return;
        }

        if (richText == null)
        {
            return;
        }

        _lastRich = richText;
        _lastPosition = position;
        _lastTranslation = localTranslation;
        _lastWidth = maxCharWidth;
        _lastFrame = Time.frameCount;
    }

    internal static void HidePrefix()
    {
        _lastRich = null;
        _lastFrame = -1000;
        _ranOriginal = true;
    }
}
