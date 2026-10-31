using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XGDragFixer;

/// <summary>
/// Several vanilla systems raycast the UI for the mouse position once per frame
/// (tooltip, item mouse handler, selectable handler, context menu, store UI).
/// They all ask the exact same question in the same frame, so the first answer
/// can be reused instead of walking the graphic raycaster again.
/// </summary>
internal static class RaycastFrameCache
{
    private static readonly List<RaycastResult> Cache = new();

    private static int _frame = -1;
    private static int _pointerId;
    private static Vector2 _position;
    private static bool _has;

    internal static bool Enabled => Core.RaycastFrameCache?.Value == true;

    internal static void Reset()
    {
        _has = false;
        Cache.Clear();
    }

    internal static bool Prefix(PointerEventData eventData, List<RaycastResult> raycastResults, ref bool __state)
    {
        __state = false;
        if (!Enabled || eventData == null || raycastResults == null)
        {
            return true;
        }

        var frame = Time.frameCount;
        if (_has && _frame == frame && _pointerId == eventData.pointerId && _position == eventData.position)
        {
            raycastResults.Clear();
            raycastResults.AddRange(Cache);
            __state = true;
            return false;
        }

        return true;
    }

    internal static void Postfix(PointerEventData eventData, List<RaycastResult> raycastResults, bool __state)
    {
        if (__state || !Enabled || eventData == null || raycastResults == null)
        {
            return;
        }

        _frame = Time.frameCount;
        _pointerId = eventData.pointerId;
        _position = eventData.position;
        Cache.Clear();
        Cache.AddRange(raycastResults);
        _has = true;
    }
}
