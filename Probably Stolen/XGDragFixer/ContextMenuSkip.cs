using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// While the context/sort menu is open the vanilla handler rebuilds the hover
/// highlight three times per frame (rich text node, context node, parent window)
/// and rewrites twelve TMP labels. None of that can change while the pointer is
/// still; skip it and only re-run when the mouse moves or the state is stale.
/// </summary>
internal static class ContextMenuSkip
{
    private static readonly FieldInfo? ItemField =
        AccessTools.Field(typeof(ItemContextHandler), "currentItem");

    private static readonly FieldInfo? InventoryField =
        AccessTools.Field(typeof(ItemContextHandler), "currentInventory");

    private static readonly FieldInfo? DelayField =
        AccessTools.Field(typeof(ItemContextHandler), "delayFrame");

    private static Vector2 _lastMouse;
    private static int _lastFrame = -1000;
    private static bool _has;
    private static bool _record;

    internal static void Reset()
    {
        _has = false;
        _lastFrame = -1000;
        _record = false;
    }

    internal static bool Prefix(ItemContextHandler __instance, ref bool __result)
    {
        _record = false;
        if (Core.ContextMenuSkip?.Value != true || __instance == null)
        {
            return true;
        }

        try
        {
            var menuOpen = ItemField?.GetValue(__instance) != null ||
                           InventoryField?.GetValue(__instance) != null;
            if (!menuOpen)
            {
                // Vanilla returns immediately as well.
                return true;
            }

            if (DelayField != null && (bool)DelayField.GetValue(__instance))
            {
                return true;
            }

            var mouse = (Vector2)Input.mousePosition;
            if (_has && Time.frameCount - _lastFrame <= 60 &&
                (mouse - _lastMouse).sqrMagnitude < 0.25f)
            {
                // Menu open, pointer static: the vanilla body would find the same
                // nodes and repaint the same highlight colors.
                __result = true;
                return false;
            }
        }
        catch
        {
            // Any reflection surprise: use the vanilla implementation.
        }

        _record = true;
        return true;
    }

    internal static void Postfix()
    {
        if (!_record)
        {
            return;
        }

        _record = false;

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
