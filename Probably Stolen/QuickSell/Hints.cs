using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuickSell;

public sealed partial class QuickSellMod
{
    private bool holding, hintsDirty, bindingKey;
    private KeyCode bindingModifier;
    private TabletButton? bindLabel;
    private ConfigEntry<string>? bindingTarget;
    private string parsedKey = "";
    private string parsedPanelKey = "";
    private string parsedRestockKey = "";
    private KeyCode[] keys = Array.Empty<KeyCode>();
    private KeyCode[] panelKeys = Array.Empty<KeyCode>();
    private KeyCode[] restockKeys = Array.Empty<KeyCode>();
    private static readonly Func<KeyCode, bool> KeyHeld = Input.GetKey, KeyDown = Input.GetKeyDown;
    private readonly Dictionary<GameItemElement, ItemHintGraphic> hints = new();
    private readonly HashSet<GameItemElement> displayedHints = new();
    private readonly KeyCode[] availableKeys = (KeyCode[])Enum.GetValues(typeof(KeyCode));
    private int windowSignature;
    private StoreClientInstance? hintCustomer;
    private GameItemElement? draggedHintItem;

    private static GameItemElement? DraggedItem()
    {
        var drag = ItemMouseDragHandler.current;
        var item = drag != null && drag.IsDraggingItem ? drag.GetCurrentItem() as GameItemElement : null;
        return item != null && !item.IsDestroyed() ? item : null;
    }

    internal void TickDraggedHint()
    {
        if (!holding) return;
        var item = DraggedItem();
        if (draggedHintItem != item)
        {
            if (draggedHintItem != null && !draggedHintItem.IsDestroyed()
                && hints.TryGetValue(draggedHintItem, out var previous) && previous != null)
                PositionHint(draggedHintItem, previous.rectTransform);
            draggedHintItem = item;
            hintsDirty = true;
        }
        // Native input updates/snap animation finish before LateUpdate. Only the
        // held item is touched; cursor movement does not schedule another scan.
        if (item != null && hints.TryGetValue(item, out var graphic) && graphic != null)
            PositionHint(item, graphic.rectTransform);
    }

    internal void HintsChanged() { if (holding) hintsDirty = true; }

    internal void HintLayoutChanged(GameItemElement item)
    {
        if (!holding) return;
        if (hints.TryGetValue(item, out var graphic) && graphic != null) PositionHint(item, graphic.rectTransform);
        hintsDirty = true;
    }

    private static void PositionHint(GameItemElement item, RectTransform rect)
    {
        // fakeMiddle follows the drag cursor without rotating its local axes.
        // Its origin is already the preview's top left, unlike inventory space.
        bool preview = DraggedItem() == item;
        var parent = preview ? item.fakeMiddle.rectTransform : item.handler.rectTransform;
        if (rect.parent != parent) rect.SetParent(parent, false);
        var topLeft = new Vector2(0, 1);
        if (rect.anchorMin != topLeft) rect.anchorMin = topLeft;
        if (rect.anchorMax != topLeft) rect.anchorMax = topLeft;
        if (rect.pivot != topLeft) rect.pivot = topLeft;
        var bounds = HintBounds.Calculate(preview ? item.tempShape : item.modifiedShape, UISettings.current.gridSize, preview);
        var position = new Vector2(bounds.X, bounds.Y);
        var size = new Vector2(bounds.Width, bounds.Height);
        if (rect.anchoredPosition != position) rect.anchoredPosition = position;
        if (rect.sizeDelta != size) rect.sizeDelta = size;
        if (rect.localScale != Vector3.one) rect.localScale = Vector3.one;
        if (rect.localRotation != Quaternion.identity) rect.localRotation = Quaternion.identity;
    }

    private int HintMask(GameItem item, StoreClient? client)
    {
        int mask = 0;
        if (demand.Value && client != null && client.IsClientBuying() && client.IsClientBuyingThisItem(item)) mask |= 1;
        if (stolen.Value && StolenHelper.IsStolenItem(item)) mask |= 2;
        if (contraband.Value && ContrabandHelper.IsContraband(item)) mask |= 4;
        return mask;
    }

    private static bool Modifier(KeyCode key) => key == KeyCode.LeftControl || key == KeyCode.RightControl
        || key == KeyCode.LeftAlt || key == KeyCode.RightAlt || key == KeyCode.LeftShift || key == KeyCode.RightShift;

    private static string BindingLabel(string text) => text.Length == 0 ? "点击绑定快捷键" : text
        .Replace("LeftControl", "Ctrl").Replace("RightControl", "Ctrl")
        .Replace("LeftAlt", "Alt").Replace("RightAlt", "Alt")
        .Replace("LeftShift", "Shift").Replace("RightShift", "Shift");

    private void SaveBinding(string text)
    {
        bindingTarget!.Set(text); bindingKey = false; bindingModifier = KeyCode.None;
        bindLabel?.SetLabel(BindingLabel(text));
        bindingTarget = null;
    }

    private void CancelBinding()
    {
        if (bindingKey && bindLabel != null) bindLabel.SetLabel(BindingLabel(bindingTarget!.Value));
        bindingKey = false; bindingModifier = KeyCode.None; bindingTarget = null;
    }

    private void BeginBinding(ConfigEntry<string> target, TabletButton button)
    {
        CancelBinding(); HideHints();
        bindingTarget = target; bindLabel = button; bindingKey = true;
        button.SetLabel("按键 · Esc取消 · Del清除");
    }

    private void TickInput()
    {
        if (settings != null && !settings.IsAlive) { CancelBinding(); settings = null; ReplaceJob(0, null); }
        bool wasBinding = bindingKey;
        if (bindingKey)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) CancelBinding();
            else if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace)) SaveBinding("");
            else
            {
                foreach (var key in availableKeys)
                {
                    if (key == KeyCode.None || key == KeyCode.Mouse0 || key == KeyCode.Mouse1 || !Input.GetKeyDown(key)) continue;
                    if (Modifier(key)) { bindingModifier = key; continue; }
                    string prefix = "";
                    foreach (var mod in new[] { KeyCode.LeftControl, KeyCode.RightControl, KeyCode.LeftAlt, KeyCode.RightAlt, KeyCode.LeftShift, KeyCode.RightShift })
                        if (Input.GetKey(mod)) prefix += mod + "+";
                    SaveBinding(prefix + key); break;
                }
                if (bindingKey && bindingModifier != KeyCode.None && Input.GetKeyUp(bindingModifier)) SaveBinding(bindingModifier.ToString());
            }
        }
        if (parsedKey != holdKey.Value)
        {
            parsedKey = holdKey.Value;
            if (!ShortcutKeys.TryParse(parsedKey, out keys)) Debug.LogWarning("[QuickSell] 无效的查看快捷键：" + parsedKey);
        }
        if (parsedPanelKey != panelKey.Value)
        {
            parsedPanelKey = panelKey.Value;
            if (!ShortcutKeys.TryParse(parsedPanelKey, out panelKeys)) Debug.LogWarning("[QuickSell] 无效的面板快捷键：" + parsedPanelKey);
        }
        if (parsedRestockKey != restockKey.Value)
        {
            parsedRestockKey = restockKey.Value;
            if (!ShortcutKeys.TryParse(parsedRestockKey, out restockKeys)) Debug.LogWarning("[QuickSell] 无效的上货快捷键：" + parsedRestockKey);
        }
        bool canUseKeys = Application.isFocused && !wasBinding && !bindingKey
            && !(StoreUIManager.Instance != null && StoreUIManager.Instance.IsTyping())
            && EmporiumEntry.Instance != null && EmporiumEntry.Instance.invElement != null;
        if (canUseKeys && ShortcutKeys.Pressed(panelKeys, KeyHeld, KeyDown))
        {
            if (settings != null && settings.IsAlive && settings.IsVisible) CloseSettings();
            else OpenWindow();
        }
        if (canUseKeys && ShortcutKeys.Pressed(restockKeys, KeyHeld, KeyDown)
            && (IsRestocking || (PlayerStore.Instance?.currentClientInstance != null && wantsActions))) BeginRestock();
        bool next = canUseKeys && (demand.Value || stolen.Value || contraband.Value) && ShortcutKeys.Held(keys, KeyHeld);
        if (!next) { if (holding) HideHints(); return; }
        if (!holding) { holding = true; hintsDirty = true; }
        var windows = WindowsHandler.current?.visibleWindows;
        int signature = 17;
        if (windows != null) foreach (var window in windows) signature = unchecked(signature * 31 + window.GetHashCode());
        var customer = PlayerStore.Instance?.currentClientInstance;
        if (signature != windowSignature || hintCustomer != customer)
        { windowSignature = signature; hintCustomer = customer; hintsDirty = true; }
        if (hintsDirty && jobs[2] == null) { hintsDirty = false; ReplaceJob(2, RefreshHints()); }
    }

    private IEnumerator RefreshHints()
    {
        var windows = WindowsHandler.current;
        if (windows == null) yield break;
        var visible = windows.visibleWindows.ToArray();
        var visited = new HashSet<GameInventory>();
        var snapshots = new Dictionary<GameInventory, List<GameItem>>();
        var active = new HashSet<GameItemElement>();
        var visibleItems = new HashSet<GameItemElement>();
        var client = PlayerStore.Instance?.currentClientInstance?.GetClientBlueprint();
        foreach (var window in visible)
        {
            if (!holding || window.handler == null || !window.IsVisible()) continue;
            foreach (var inventory in GraphUtils.FindChildrenType<GameInventory>(window))
            {
                if (!visited.Add(inventory)) continue;
                var items = inventory.childItems;
                snapshots[inventory] = items;
                yield return null;
                foreach (var item in items)
                {
                    if (!holding) yield break;
                    if (item is GameItemElement element && !element.IsDestroyed() && element.middle != null && element.middle.gameObject.activeInHierarchy)
                        visibleItems.Add(element);
                    yield return null;
                }
            }
        }
        var dragged = DraggedItem();
        if (dragged != null) visibleItems.Add(dragged);
        // One post-order pass shares descendant masks between visible containers.
        // Closed contents are read only to aggregate the visible container badge;
        // no graphics are created for those hidden contents.
        var masks = new Dictionary<GameItem, int>();
        var children = new Dictionary<GameItem, List<GameItem>>();
        var visiting = new HashSet<GameItem>();
        var stack = new Stack<(GameItem Item, bool Expanded)>();
        foreach (var rootItem in visibleItems)
        {
            stack.Push((rootItem, false));
            while (stack.Count > 0)
            {
                if (!holding) yield break;
                var step = stack.Pop();
                if (masks.ContainsKey(step.Item)) continue;
                if (step.Expanded)
                {
                    int mask = HintMask(step.Item, client);
                    foreach (var child in children[step.Item]) if (masks.TryGetValue(child, out int childMask)) mask |= childMask;
                    masks[step.Item] = mask;
                    visiting.Remove(step.Item);
                }
                else if (visiting.Add(step.Item))
                {
                    var content = new List<GameItem>();
                    if (step.Item.contentWindow != null)
                        foreach (var inventory in GraphUtils.FindChildrenType<GameInventory>(step.Item.contentWindow))
                        {
                            // childItems copies once per inventory in this traversal.
                            if (!snapshots.TryGetValue(inventory, out var snapshot))
                            {
                                snapshot = inventory.childItems;
                                snapshots[inventory] = snapshot;
                            }
                            content.AddRange(snapshot);
                            yield return null;
                        }
                    children[step.Item] = content;
                    stack.Push((step.Item, true));
                    for (int i = content.Count - 1; i >= 0; i--) if (!visiting.Contains(content[i])) stack.Push((content[i], false));
                }
                yield return null;
            }
        }
        foreach (var element in visibleItems)
        {
            if (!holding) yield break;
            if (!element.IsDestroyed() && element.handler != null && (element.handler.gameObject.activeInHierarchy || DraggedItem() == element)
                && masks.TryGetValue(element, out int mask) && mask != 0)
            {
                if (!hints.TryGetValue(element, out var graphic) || graphic == null)
                {
                    var root = new GameObject("QuickSellHint", typeof(RectTransform));
                    root.transform.SetParent(element.handler.rectTransform, false);
                    graphic = root.AddComponent<ItemHintGraphic>();
                    graphic.raycastTarget = false;
                    hints[element] = graphic;
                }
                PositionHint(element, graphic.rectTransform);
                graphic.SetMask(mask); graphic.enabled = true;
                active.Add(element); displayedHints.Add(element);
            }
            yield return null;
        }
        foreach (var item in displayedHints)
            if (!active.Contains(item) && hints.TryGetValue(item, out var graphic) && graphic != null) graphic.enabled = false;
        displayedHints.Clear(); displayedHints.UnionWith(active);
    }

    private void HideHints()
    {
        holding = false; ReplaceJob(2, null);
        foreach (var item in displayedHints) if (hints.TryGetValue(item, out var graphic) && graphic != null) graphic.enabled = false;
        displayedHints.Clear();
    }

    private void DestroyHints()
    {
        foreach (var graphic in hints.Values) if (graphic != null) UnityEngine.Object.Destroy(graphic.gameObject);
        hints.Clear();
        draggedHintItem = null;
    }

    internal void RemoveHint(GameItemElement item)
    {
        displayedHints.Remove(item);
        if (!hints.TryGetValue(item, out var graphic)) return;
        hints.Remove(item);
        if (graphic != null) UnityEngine.Object.Destroy(graphic.gameObject);
    }
}

// One shared atlas, at most six quads per item. No extra child graphics or animation.
public sealed class ItemHintGraphic : MaskableGraphic
{
    private int mask;
    public override Texture mainTexture => HintArtwork.Texture!;
    internal void SetMask(int value) { if (mask == value) return; mask = value; SetVerticesDirty(); }
    protected override void OnPopulateMesh(VertexHelper mesh)
    {
        mesh.Clear(); var rect = rectTransform.rect;
        float x = rect.xMin, y = rect.yMin, w = rect.width, h = rect.height;
        if ((mask & 1) != 0)
        {
            float length = Mathf.Min(6, Mathf.Min(w, h) / 3);
            Quad(mesh, x, y + h - length, length, length, HintArtwork.Regions[0]);
            Quad(mesh, x + w - length, y + h - length, length, length, HintArtwork.Regions[1]);
            Quad(mesh, x, y, length, length, HintArtwork.Regions[2]);
            Quad(mesh, x + w - length, y, length, length, HintArtwork.Regions[3]);
        }
        // Same UI size for all items, including single-cell cards and large gear.
        const float size = 7f;
        float bx = x + w - size, by = y + h - size;
        if ((mask & 4) != 0)
        {
            // Fit each badge within the same square without changing its aspect.
            float height = size * 847 / 946;
            Quad(mesh, bx, by + size - height, size, height, HintArtwork.Regions[5]);
            bx -= size + 1;
        }
        if ((mask & 2) != 0)
        {
            float height = size * 757 / 784;
            Quad(mesh, bx, by + size - height, size, height, HintArtwork.Regions[4]);
        }
    }
    private static void Quad(VertexHelper mesh, float x, float y, float w, float h, Rect uv)
    {
        int start = mesh.currentVertCount;
        mesh.AddVert(new Vector3(x, y), Color.white, new Vector2(uv.xMin, uv.yMin));
        mesh.AddVert(new Vector3(x, y + h), Color.white, new Vector2(uv.xMin, uv.yMax));
        mesh.AddVert(new Vector3(x + w, y + h), Color.white, new Vector2(uv.xMax, uv.yMax));
        mesh.AddVert(new Vector3(x + w, y), Color.white, new Vector2(uv.xMax, uv.yMin));
        mesh.AddTriangle(start, start + 1, start + 2); mesh.AddTriangle(start, start + 2, start + 3);
    }
}
