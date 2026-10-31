using System;
using System.Collections.Generic;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// Suppresses redundant UGUI layout rebuilds while a container window is being
/// opened (and for a couple of frames after it). Only rebuilds that resolve to
/// the window that is currently opening (or, in the sticky phase, to windows
/// that are currently inactive) are dropped.
/// </summary>
internal static class LayoutSuppressor
{
    private static bool _active;
    private static int _depth;
    private static int _beginFrame = -1;
    private static readonly List<RectTransform> Roots = new();
    private static readonly List<Transform> RootTransforms = new();
    private static int _stickyUntil = -1;
    private static bool _pending;

    private static bool SuppressionEnabled =>
        Core.SkipLayoutMark?.Value == true || Core.SkipMaskNotify?.Value == true;

    internal static void Reset()
    {
        _active = false;
        _depth = 0;
        Roots.Clear();
        RootTransforms.Clear();
        _stickyUntil = -1;
        _pending = false;
    }

    internal static void Begin(RectTransform root)
    {
        if (!SuppressionEnabled)
        {
            return;
        }

        // Show() always runs and finishes within the same frame, so an "active"
        // suppression block from an earlier frame is a leftover from an exception.
        if (_active && _beginFrame != Time.frameCount)
        {
            Reset();
        }

        _depth++;
        if (_depth > 1)
        {
            return;
        }

        if (root == null)
        {
            _depth = 0;
            return;
        }

        Roots.Clear();
        Roots.Add(root);
        RootTransforms.Clear();
        try
        {
            RootTransforms.Add(root.transform);
        }
        catch
        {
            RootTransforms.Add(null);
        }

        _active = true;
        _beginFrame = Time.frameCount;
        _stickyUntil = -1;
        _pending = false;
    }

    internal static void End()
    {
        if (_depth == 0)
        {
            return;
        }

        if (!SuppressionEnabled)
        {
            Reset();
            return;
        }

        _depth--;
        if (_depth > 0 || !_active)
        {
            return;
        }

        _active = false;
        var frames = Core.StickyLayoutFrames?.Value ?? 0;
        _stickyUntil = frames > 0 ? Time.frameCount + frames : -1;
        _pending = true;
    }

    internal static void Tick()
    {
        if (_pending && Time.frameCount > _stickyUntil)
        {
            _pending = false;
        }
    }

    internal static bool MarkLayoutPrefix(RectTransform rect)
    {
        if (Core.SkipLayoutMark?.Value != true)
        {
            return true;
        }

        if (_active)
        {
            return false;
        }

        if (rect == null || Time.frameCount > _stickyUntil)
        {
            return true;
        }

        if (InWindow(rect))
        {
            return false;
        }

        // Sticky phase: keep suppressing rebuilds for windows that are not visible.
        // Their layout is rebuilt anyway the next time they get activated.
        return IsActive(rect);
    }

    internal static bool MaskNotifyPrefix(Component mask)
    {
        if (Core.SkipMaskNotify?.Value != true || !_active || mask == null)
        {
            return true;
        }

        try
        {
            var transform = mask.transform;
            if (transform == null)
            {
                return true;
            }

            for (var i = 0; i < RootTransforms.Count; i++)
            {
                if (IsUnder(transform, RootTransforms[i]))
                {
                    return false;
                }
            }
        }
        catch
        {
            // Fall through to the vanilla call.
        }

        return true;
    }

    private static bool InWindow(RectTransform rect)
    {
        try
        {
            var transform = rect.transform;
            for (var i = 0; i < RootTransforms.Count; i++)
            {
                if (IsUnder(transform, RootTransforms[i]))
                {
                    return true;
                }
            }
        }
        catch
        {
            // Treat as not part of the window.
        }

        return false;
    }

    private static bool IsUnder(Transform transform, Transform root)
    {
        try
        {
            if (transform == null || root == null)
            {
                return false;
            }

            return ReferenceEquals(transform, root) || transform.IsChildOf(root);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsActive(RectTransform rect)
    {
        try
        {
            return rect.gameObject != null && rect.gameObject.activeInHierarchy;
        }
        catch
        {
            return true;
        }
    }

}
