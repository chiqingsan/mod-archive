using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XGDragFixer;

/// <summary>
/// Port of the original mod's middle-mouse container preview.
/// Holding MMB over a container opens a lightweight preview window and keeps
/// moving the preview to the container under the cursor; releasing MMB leaves
/// the last preview open, exactly like the original mod.
/// </summary>
internal static class HoverPeek
{
    private static readonly List<RaycastResult> RaycastResults = new(16);

    private static GameItemElement? _activeItem;
    private static PixelWindow? _activeWindow;
    private static GameItemElement? _suppressedItem;
    private static GameItemElement? _lastClosedItem;
    private static int _lastClosedFrame = -1000;
    private static int _openFrame = -1000;
    private static bool _busy;

    private static bool Enabled => Core.MiddleClickPreview?.Value == true;

    internal static void Tick()
    {
        if (!Enabled || !Input.GetMouseButton(2))
        {
            ResetState();
            return;
        }

        if (_busy)
        {
            return;
        }

        _busy = true;
        try
        {
            TickCore();
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"中键悬浮预览失败: {ex.GetType().Name} {ex.Message}");
        }
        finally
        {
            _busy = false;
        }
    }

    internal static void ResetState()
    {
        _activeItem = null;
        _activeWindow = null;
        _suppressedItem = null;
    }

    internal static bool BlockTooltipPrefix()
    {
        if (Core.MiddleClickPreview?.Value != true || Core.PreviewHideTooltip?.Value != true)
        {
            return true;
        }

        try
        {
            return !Input.GetMouseButton(2);
        }
        catch
        {
            return true;
        }
    }

    private static void TickCore()
    {
        if (_activeWindow != null && !ContainerShowFix.IsWindowOpen(_activeWindow))
        {
            _suppressedItem = _activeItem;
            _activeItem = null;
            _activeWindow = null;
        }

        var mouse = Input.mousePosition;
        var node = TopNode(mouse);
        if ((_activeWindow != null && node != null && IsUnderNode(node, _activeWindow)) ||
            Time.frameCount - _openFrame < 2)
        {
            return;
        }

        var item = node == null ? null : RenderHandler.RaycastElement<GameItemElement>(mouse);
        if (item == null)
        {
            _suppressedItem = null;
            ClosePeek();
            return;
        }

        if (ReferenceEquals(item, _suppressedItem))
        {
            ClosePeek();
            return;
        }

        _suppressedItem = null;
        var window = item.contentWindow;
        if (window == null)
        {
            ClosePeek();
            return;
        }

        if (ReferenceEquals(item, _activeItem))
        {
            return;
        }

        if (ContainerShowFix.IsWindowOpen(window))
        {
            ClosePeek();
            return;
        }

        ClosePeek();
        OpenPeek(item, window, mouse);
    }

    private static void OpenPeek(GameItemElement item, PixelWindow window, Vector2 mouse)
    {
        if (ReferenceEquals(item, _lastClosedItem) && Time.frameCount - _lastClosedFrame <= 2)
        {
            _suppressedItem = item;
            return;
        }

        var translation = new Vector2(6f, -6f);
        try
        {
            var doubleClick = ItemMouseDoubleClickHandler.current;
            if (doubleClick != null)
            {
                translation = doubleClick.openWindowTranslation;
            }
        }
        catch
        {
            // Keep the default translation.
        }

        if (Core.PreviewHideTooltip?.Value == true)
        {
            HideVisibleTooltips();
        }

        try
        {
            window.SetTitle(item.GetDisplayName(true));
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"预览窗口标题设置失败: {ex.GetType().Name} {ex.Message}");
        }

        window.Show(mouse, translation, false);

        try
        {
            item.RestoreContentWindowPosition();
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"预览窗口位置恢复失败: {ex.GetType().Name} {ex.Message}");
        }

        _activeItem = item;
        _activeWindow = window;
        _openFrame = Time.frameCount;
    }

    private static void ClosePeek()
    {
        var window = _activeWindow;
        if (window == null)
        {
            _activeItem = null;
            return;
        }

        _lastClosedItem = _activeItem;
        _lastClosedFrame = Time.frameCount;
        _activeItem = null;
        _activeWindow = null;

        try
        {
            window.Hide();
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"预览窗口关闭失败: {ex.GetType().Name} {ex.Message}");
        }
    }

    private static void HideVisibleTooltips()
    {
        try
        {
            var overlay = OverlayHandler.current;
            if (overlay != null)
            {
                var tooltipWindow = overlay.GetTooltipWindow();
                if (tooltipWindow != null && tooltipWindow.IsVisible())
                {
                    overlay.HideTooltip();
                }
            }
        }
        catch
        {
            // Ignore tooltip cleanup failures.
        }

        try
        {
            var manager = CustomUIManager.Instance;
            if (manager != null)
            {
                manager.HideTooltip(null);
            }
        }
        catch
        {
            // Ignore tooltip cleanup failures.
        }
    }

    private static GameObject? TopNode(Vector2 mouse)
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            return null;
        }

        var data = new PointerEventData(eventSystem) { position = mouse };
        RaycastResults.Clear();
        eventSystem.RaycastAll(data, RaycastResults);
        for (var i = 0; i < RaycastResults.Count; i++)
        {
            var gameObject = RaycastResults[i].gameObject;
            if (gameObject == null)
            {
                continue;
            }

            var render = gameObject.GetComponent<TreeNodeRender>();
            if (render != null)
            {
                return render.gameObject;
            }
        }

        return null;
    }

    private static bool IsUnderNode(GameObject node, PixelWindow window)
    {
        try
        {
            var handler = window.handler;
            if (handler == null)
            {
                return false;
            }

            var transform = node.transform;
            var root = handler.transform;
            for (var i = 0; i < 64 && transform != null; i++)
            {
                if (ReferenceEquals(transform, root))
                {
                    return true;
                }

                transform = transform.parent;
            }
        }
        catch
        {
            // Treat as not under the window.
        }

        return false;
    }
}
