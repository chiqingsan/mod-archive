using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// StoreUIManager.CheckRaycast runs 4-5 full GraphicRaycaster passes every frame
/// just to answer "is the pointer over the store UI". When the pointer is completely
/// idle (no movement, no buttons, no scroll, no keys) the answer cannot change unless
/// the UI itself changes under the cursor; keep the previous value and recompute at
/// least once every StoreHoverIdleFrames frames as a safety net.
/// </summary>
internal static class StoreHoverCache
{
    private static Vector2 _lastMouse;
    private static int _lastFrame = -1000;
    private static bool _has;
    private static bool _computed;

    internal static void Reset()
    {
        _has = false;
        _lastFrame = -1000;
        _computed = false;
    }

    internal static bool Prefix()
    {
        _computed = false;

        if (Core.SkipStoreHoverRaycast?.Value != true)
        {
            return true;
        }

        try
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) ||
                Input.mouseScrollDelta.sqrMagnitude > 0.01f || Input.anyKey)
            {
                return true;
            }

            var mouse = (Vector2)Input.mousePosition;
            var frames = Core.StoreHoverIdleFrames?.Value ?? 15;
            if (_has && Time.frameCount - _lastFrame <= frames &&
                (mouse - _lastMouse).sqrMagnitude < 0.25f)
            {
                // Keep the previously computed hoveringStoreUI.
                return false;
            }
        }
        catch
        {
            // Any surprise: recompute like vanilla.
        }

        _computed = true;
        return true;
    }

    internal static void Postfix()
    {
        if (!_computed || Core.SkipStoreHoverRaycast?.Value != true)
        {
            return;
        }

        _computed = false;

        try
        {
            _lastMouse = Input.mousePosition;
            _lastFrame = Time.frameCount;
            _has = true;
        }
        catch
        {
            _has = false;
        }
    }
}
