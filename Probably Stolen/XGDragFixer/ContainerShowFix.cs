using System;
using System.Reflection;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// Port of the original mod's container opening fixes:
/// - warm up the floating-window canvas/dock before the window is populated,
///   so the first shown frame does not pay for the whole docking chain;
/// </summary>
internal static class ContainerShowFix
{
    private static MethodInfo? _ensureCanvas;
    private static MethodInfo? _ensureFloatingDock;
    private static MethodInfo? _adopt;
    private static bool _methodsResolved;
    private static bool _missingLogged;

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

        const BindingFlags staticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        const BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        try
        {
            _ensureCanvas = typeof(UIFamilyLayer).GetMethod("EnsureCanvas", staticFlags, null, Type.EmptyTypes, null);
            _adopt = typeof(UIFamilyLayer).GetMethod("Adopt", staticFlags, null, new[] { typeof(RectTransform) }, null);
            _ensureFloatingDock = typeof(PixelWindow).GetMethod("EnsureFloatingDock", instanceFlags, null,
                Type.EmptyTypes, null);
        }
        catch (Exception ex)
        {
            _ensureCanvas = null;
            _adopt = null;
            _ensureFloatingDock = null;
            if (!_missingLogged)
            {
                _missingLogged = true;
                Core.Log?.Warning($"[兼容] 容器冷启动反射解析失败: {ex.GetType().Name}");
            }

            return;
        }

        if (_ensureCanvas != null && _adopt != null && _ensureFloatingDock != null)
        {
            _methodsResolved = true;
            return;
        }

        if (!_missingLogged)
        {
            _missingLogged = true;
            Core.Log?.Warning(
                $"[兼容] 容器冷启动所需方法缺失（EnsureCanvas={_ensureCanvas != null}, " +
                $"Adopt={_adopt != null}, EnsureFloatingDock={_ensureFloatingDock != null}），本次运行跳过冷启动优化");
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
