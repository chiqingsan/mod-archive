using System;
using System.Reflection;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// Port of the original mod's container opening fixes:
/// - warm up the floating-window canvas/dock before the window is populated,
///   so the first shown frame does not pay for the whole docking chain;
/// - skip the redundant layout validation that SetTitle performs.
/// </summary>
internal static class ContainerShowFix
{
    private static MethodInfo? _ensureCanvas;
    private static MethodInfo? _ensureFloatingDock;
    private static MethodInfo? _adopt;
    private static bool _methodsResolved;

    private static bool _inSetTitle;
    private static int _setTitleValidateSeen;

    internal static void ColdStartPrefix(PixelWindow __instance)
    {
        if (Core.FastContainerOpen?.Value != true || __instance == null)
        {
            return;
        }

        try
        {
            var handler = __instance.handler;
            var gameObject = handler != null ? handler.gameObject : null;
            if (gameObject == null || gameObject.activeSelf)
            {
                return;
            }

            if (!__instance.canMove)
            {
                return;
            }

            ResolveMethods();

            var rectTransform = handler.rectTransform;
            LayoutSuppressor.Begin(rectTransform);
            Invoke(_ensureCanvas, null, null);
            Invoke(_ensureFloatingDock, __instance, null);

            if (rectTransform != null)
            {
                var dockRoot = SafeDockRoot();
                var parent = rectTransform.parent;
                var alreadyDocked = ReferenceEquals(parent, dockRoot);
                if (!alreadyDocked)
                {
                    Invoke(_adopt, null, new object[] { rectTransform });
                    rectTransform.SetAsLastSibling();
                }
            }

            if (rectTransform != null && rectTransform.parent == null)
            {
                UIFamilyLayer.BringWindowsToFront(__instance);
            }
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"容器冷启动预处理失败: {ex.GetType().Name} {ex.Message}");
        }
    }

    internal static void SetTitlePrefix()
    {
        if (Core.SetTitleSkipValidate?.Value == true)
        {
            _inSetTitle = true;
            _setTitleValidateSeen = 0;
        }
    }

    internal static void SetTitlePostfix()
    {
        _inSetTitle = false;
    }

    internal static bool ValidatePrefix()
    {
        if (Core.SetTitleSkipValidate?.Value != true || !_inSetTitle)
        {
            return true;
        }

        // SetTitle validates twice (once before measuring, once after resizing);
        // the first validation is redundant because layout is rebuilt later anyway.
        if (_setTitleValidateSeen == 0)
        {
            _setTitleValidateSeen = 1;
            return false;
        }

        return true;
    }

    internal static bool IsWindowOpen(PixelWindow window)
    {
        try
        {
            return WindowsHandler.current?.visibleWindows?.Contains(window) ?? false;
        }
        catch
        {
            return true;
        }
    }

    private static RectTransform? SafeDockRoot()
    {
        try
        {
            return LetterboxManager.DockRoot;
        }
        catch
        {
            return null;
        }
    }

    private static void ResolveMethods()
    {
        if (_methodsResolved)
        {
            return;
        }

        _methodsResolved = true;
        const BindingFlags staticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        const BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        try
        {
            _ensureCanvas = typeof(UIFamilyLayer).GetMethod("EnsureCanvas", staticFlags, null, Type.EmptyTypes, null);
            _adopt = typeof(UIFamilyLayer).GetMethod("Adopt", staticFlags, null, new[] { typeof(RectTransform) }, null);
            _ensureFloatingDock = typeof(PixelWindow).GetMethod("EnsureFloatingDock", instanceFlags, null,
                Type.EmptyTypes, null);
        }
        catch
        {
            _ensureCanvas = null;
            _adopt = null;
            _ensureFloatingDock = null;
        }
    }

    private static void Invoke(MethodInfo? method, object? instance, object?[]? arguments)
    {
        if (method == null)
        {
            return;
        }

        try
        {
            method.Invoke(instance, arguments);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"容器冷启动反射调用 {method.Name} 失败: {ex.GetType().Name} {ex.Message}");
        }
    }
}
