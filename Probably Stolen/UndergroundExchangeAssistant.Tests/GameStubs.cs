using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
    public class MonoBehaviour { }
    public class GameObject { public bool activeSelf = true; public void SetActive(bool value) => activeSelf = value; }
    public static class Debug
    {
        public static void LogError(object message) => Console.WriteLine("  [LogError] " + message);
        public static void LogWarning(object message) => Console.WriteLine("  [LogWarning] " + message);
    }
}

namespace UnityEngine.EventSystems
{
    public interface IPointerEnterHandler { void OnPointerEnter(PointerEventData data); }
    public interface IPointerExitHandler { void OnPointerExit(PointerEventData data); }
    public class PointerEventData { public bool Used; public void Use() => Used = true; }
    public interface IBeginDragHandler { void OnBeginDrag(PointerEventData data); }
    public interface IDragHandler { void OnDrag(PointerEventData data); }
    public interface IEndDragHandler { void OnEndDrag(PointerEventData data); }
}

public class TagState { public int Value; public int GetInt() => Value; }
public class TagSystem { public Dictionary<string, TagState> dict = new(); public TagSystem Clone() => new(); }

public enum WindowStyle { Classic, Machinery }

public class PixelWindow : GraphNodeStorage
{
    public WindowStyle style;
    // Real PixelWindow exposes its attached inventory as child/children.
    public GraphNodeStorage? child;
    public override List<GraphNodeStorage> children
        => child == null ? new List<GraphNodeStorage>() : new List<GraphNodeStorage> { child };
}

public class GridShape
{
    public int x, y, width = 3, height = 3;
    public GridShape Clone() => (GridShape)MemberwiseClone();
    public bool Intersects(GridShape b)
        => b.x < x + width && x < b.x + b.width && b.y < y + height && y < b.y + b.height;
}

public class GridShapeBuilder
{
    private readonly GridShape shape;
    public GridShapeBuilder(GridShape s) { shape = s.Clone(); }
    public GridShapeBuilder SetTransform(int x, int y, bool flip, int rotation)
    {
        shape.x = x; shape.y = y;
        if (rotation == 1 || rotation == 3) (shape.width, shape.height) = (shape.height, shape.width);
        return this;
    }
    public GridShape Build() => shape;
}

public class GraphNodeStorage
{
    public virtual List<GraphNodeStorage> children => new();
    public virtual SlotMarker? TryFindOneValidInventorySlot(GameItem item) => null;
}

public class GameInventory : GraphNodeStorage
{
    public List<GameItem> items = new(); public GameItem? owner;
    public List<GameItem> childItems => items;
    public override List<GraphNodeStorage> children => items.Cast<GraphNodeStorage>().ToList();
    public GameItem? GetParentItem() => owner;
    public virtual bool Expel(GameItem i)
    {
        if (!items.Remove(i)) return false;
        i.parentInventory = null!;
        return true;
    }
    public virtual bool UncheckedAccept(GameItem i)
    {
        if (i.parentInventory != null) return false;
        items.Add(i); i.parentInventory = this;
        return true;
    }
    public SlotMarker GetIncompleteItemInventorySlot(GameItem i) => new() { item = i, inventory = this };
    public Action<GameItem, GameInventory, SlotMarker>? onSlotChangeAmountItemFunc;
}

public class GameGridInventory : GameInventory
{
    public GridShape inventoryShape = new() { width = 10, height = 10 };
    public int FailAt = -1, accepts, probes;

    public override bool UncheckedAccept(GameItem i)
    {
        if (++accepts == FailAt) return false;
        return base.UncheckedAccept(i);
    }

    public override SlotMarker? TryFindOneValidInventorySlot(GameItem item)
    {
        if (++probes == FailAt) return null;
        if (item.MaxNumRemove() < 1) return null;
        for (int y = 0; y < inventoryShape.height; y++)
        {
            for (int x = 0; x < inventoryShape.width; x++)
            {
                var shape = new GridShape { x = x, y = y, width = item.modifiedShape.width, height = item.modifiedShape.height };
                if (shape.x + shape.width > inventoryShape.width || shape.y + shape.height > inventoryShape.height) continue;
                if (items.Any(other => other.modifiedShape.Intersects(shape))) continue;
                return new SlotMarker { item = item, inventory = this, itemGridShape = shape, numTransfer = Math.Min(1, item.MaxNumRemove()) };
            }
        }
        return null;
    }

    public SlotMarker TryInventorySlot(GameItem i, int amount, GridShape shape)
    {
        var marker = new SlotMarker { item = i, inventory = this, itemGridShape = shape };
        if (shape.x < 0 || shape.y < 0
            || shape.x + shape.width > inventoryShape.width
            || shape.y + shape.height > inventoryShape.height) return marker;
        if (items.Any(other => other.modifiedShape.Intersects(shape))) return marker;
        marker.numTransfer = Math.Min(amount, i.MaxNumRemove());
        return marker;
    }
}

public class SlotMarker
{
    public GameItem item = null!, targetItem = null!; public GameInventory inventory = null!;
    public GridShape itemGridShape = new(); public int numTransfer;
    public int AcceptUnchecked()
    {
        if (item == null || inventory == null || itemGridShape == null || numTransfer < 1) return 0;
        item.modifiedShape = itemGridShape;
        return inventory.UncheckedAccept(item) ? item.unitCount : 0;
    }

    public int TryAcceptOnce(int amount = -1)
    {
        if (item == null || inventory == null || itemGridShape == null) return 0;
        if (amount < 0) amount = item.unitCount;
        amount = Math.Min(amount, Math.Min(numTransfer, item.MaxNumRemove()));
        if (amount <= 0) return 0;
        if (amount < item.unitCount) item = item.GetSplitItem(amount);
        else item.parentInventory?.Expel(item);
        item.modifiedShape = itemGridShape;
        return inventory.UncheckedAccept(item) ? amount : 0;
    }
}

public class GameItem : GraphNodeStorage
{
    public string identifier = "", name = "", customText = "";
    public int unitCount = 1;
    public int durability
    {
        get => modifiedState.dict.TryGetValue("DURABILITY_TAG", out var tag) ? tag.GetInt() : 0;
        set => modifiedState.dict["DURABILITY_TAG"] = new TagState { Value = value };
    }
    public bool owned = true, pinned, locked, machine;
    public long unitValue, unitBaseValue, lateUnitValue, backupUnitValue;
    public TagSystem state = new(), modifiedState = new();
    public GridShape shape = new(), modifiedShape = new() { x = 0, y = 0, width = 1, height = 1 };
    public PixelWindow? contentWindow, examineWindow;
    public List<object> itemFeatures = new();
    public GameInventory parentInventory = null!;
    public GameInventory? contents;
    public Action<GameItem>? onChangeAmountItemFunc;
    public Action<GameItem, GameInventory, SlotMarker>? onSlotChangeAmountItemFunc;

    public override List<GraphNodeStorage> children
    {
        get
        {
            var list = new List<GraphNodeStorage>();
            if (contents != null) list.Add(contents);
            if (contentWindow != null && contentWindow is GraphNodeStorage storage) list.Add(storage);
            return list;
        }
    }

    public override SlotMarker? TryFindOneValidInventorySlot(GameItem item)
        => contents == null ? null : contents.TryFindOneValidInventorySlot(item);

    public readonly HashSet<string> tags = new();
    public void EnableTag(string tag) => tags.Add(tag);
    public bool IsTag(string tag) => tags.Contains(tag) || (tag == "STANDARD_MACHINE_TAG" && machine);
    public bool IsGameItemType(string gameItemType) => machine && gameItemType == "MACHINE";
    public bool MayRemove() => parentInventory != null;
    public int MaxNumRemove() => unitCount;
    public void Validate() { }

    public List<GameItem> FindAllChildItems()
        => contents == null ? new List<GameItem>() : GraphUtils.FindAllChildrenType<GameItem>(contents);

    public GameItem GetSplitItem(int amount)
    {
        unitCount -= amount;
        Validate();
        onChangeAmountItemFunc?.Invoke(this);
        if (parentInventory != null)
        {
            var slot = parentInventory.GetIncompleteItemInventorySlot(this);
            onSlotChangeAmountItemFunc?.Invoke(this, parentInventory, slot);
            parentInventory.onSlotChangeAmountItemFunc?.Invoke(this, parentInventory, slot);
        }
        return new GameItem { identifier = identifier, unitCount = amount };
    }

    public void Destroy() { parentInventory?.Expel(this); }
}

public static class GraphUtils
{
    public static List<T> FindAllChildrenType<T>(object root)
    {
        var result = new List<T>();
        var inventory = root as GameInventory ?? (root as GameItem)?.contents;
        if (inventory == null) return result;
        foreach (var item in inventory.items)
        {
            result.Add((T)(object)item);
            result.AddRange(FindAllChildrenType<T>(item));
        }
        return result;
    }

    // Mirrors GraphUtils.FindChildrenType: breadth first descendants of T that
    // stop at the first matched node unless passthrough says otherwise.
    public static List<T> FindChildrenType<T>(GraphNodeStorage root, Func<T, bool>? filter = null,
        Func<T, bool>? passthrough = null)
    {
        var result = new List<T>();
        if (root == null) return result;
        filter ??= _ => true;
        passthrough ??= _ => false;
        var seen = new HashSet<T>();
        var queue = new Queue<GraphNodeStorage>();
        queue.Enqueue(root);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node is T typed)
            {
                if (!ReferenceEquals(node, root) && filter(typed)) seen.Add(typed);
                if (!ReferenceEquals(node, root) && !passthrough(typed)) continue;
            }
            foreach (var child in node.children) queue.Enqueue(child);
        }
        result.AddRange(seen);
        return result;
    }

    public static int TryAcceptAll(GraphNodeStorage node, GameItem item, int amount = -1)
    {
        if (node == null || item == null) return amount < 0 ? item?.unitCount ?? 0 : amount;
        if (amount < 0) amount = item.unitCount;
        if (amount <= 0) return 0;
        int accepted = 0;
        for (int i = 0; i < 40 && accepted < amount; i++)
        {
            var marker = node.TryFindOneValidInventorySlot(item);
            if (marker == null || marker.numTransfer <= 0) break;
            int num = marker.TryAcceptOnce(amount - accepted);
            if (num <= 0) break;
            accepted += num;
        }
        return amount - accepted;
    }

    public static bool CanAccept(GraphNodeStorage node, GameItem item)
    {
        var marker = node == null ? null : node.TryFindOneValidInventorySlot(item);
        return marker != null && marker.numTransfer > 0;
    }

    public static List<T> FindParentsType<T>(GameInventory inventory)
    {
        var result = new List<T>();
        if (inventory == null) return result;
        for (var parent = inventory.owner; parent != null; parent = parent.parentInventory?.owner)
            result.Add((T)(object)parent);
        return result;
    }
}

public class BarterCondition
{
    public string Id = "";
    public bool IsItemValid(GameItem item) => item.identifier == Id;
}

public class ExchangeBarter
{
    public string identifier = ""; public int priority;
    public List<BarterCondition> conditions = new();

    public List<GameItem> GetRequiredItems()
    {
        var source = GraphUtils.FindAllChildrenType<GameItem>(EmporiumEntry.Instance.trashcan);
        var used = new HashSet<GameItem>();
        var result = new List<GameItem>();
        foreach (var condition in conditions)
        {
            var match = source.FirstOrDefault(item => !used.Contains(item) && condition.IsItemValid(item));
            if (match == null) return result;
            used.Add(match);
            result.Add(match);
        }
        return result;
    }
}

public class ExchangeManager
{
    public bool isExchangeUnlocked = true;
    public List<ExchangeBarter> activeBarter = new(), promotionalBarter = new();
    public List<string> activePromotionalBarterIds = new();
}

public class PlayerStore
{
    public static PlayerStore Instance = new();
    public ExchangeManager exchangeManager = new();
}

public class EmporiumEntry
{
    public static EmporiumEntry Instance = new();
    public GameGridInventory invElement = new();
    public GameGridInventory frontInvinvElement = new();
    public GameGridInventory backInvinvElement = new();
    public GameGridInventory backInvinvElementCounter = new();
    public GameGridInventory afterhourInventory = new();
    public GameGridInventory bazarLeftinvElement = new();
    public GameGridInventory showcaseElement = new();
    public GameGridInventory hiddenElement = new();
    public GameGridInventory docInvElement = new();
    public PixelWindow afterhourWindow = new();
    public GameGridInventory hirelingInv = new();
    public GameGridInventory trashcanInvElement = new();
    public GameItem trashcan;

    public EmporiumEntry()
    {
        trashcan = new GameItem
        {
            identifier = "trashcan",
            contents = trashcanInvElement,
            // The grid the game's own exchange scan walks sits under the
            // trashcan's content window, not only in trashcanInvElement.
            contentWindow = new PixelWindow { child = trashcanInvElement }
        };
        trashcanInvElement.owner = trashcan;
    }
}

public static class GeneralHelper
{
    public static bool IsItemOwned(GameItem item) => item.owned;
    public static bool IsItemLocked(GameItem item) => item.locked;

    // Items inside a player container often rely on the container's ownership.
    public static bool IsItemEffectivelyOwned(GameItem item)
    {
        if (IsItemOwned(item)) return true;
        for (var inventory = item.parentInventory; inventory != null; inventory = inventory.owner?.parentInventory)
            if (inventory.owner != null && IsItemOwned(inventory.owner)) return true;
        return false;
    }
}

public static class ItemPinHelper { public static bool IsPinned(GameItem item) => item.pinned; }
public static class MachineHelper { public const string STANDARD_MACHINE_TAG = "STANDARD_MACHINE_TAG"; }
public static class WaterHelper { public const string FILTER_CONTAINER_TAG = "FILTER_CONTAINER_TAG"; }
public static class AmenitiesItemDirectory { public const string WATER_FILTER = "WATER_FILTER"; }
public static class DurabilityHelper
{
    public static int GetRealDurability(GameItem item)
    {
        // Native GetTagReadonly calls GetTag, which inserts a missing tag.
        if (!item.modifiedState.dict.ContainsKey("DURABILITY_TAG"))
            item.modifiedState.dict.Add("DURABILITY_TAG", new TagState());
        return item.modifiedState.dict["DURABILITY_TAG"].GetInt();
    }
}
public static class DirectoryMaster { public static GameItem Item(string id) => new() { identifier = id }; }

public static class StoreReputation
{
    public static bool Blocked;
    public static bool IsPerkUnlocked(string id) => Blocked;
}

public class StoreService
{
    public bool isUsing = true;
    public static StoreService Service = new();
    public static StoreService GetJanitorialService() => Service;
}

public class ItemMouseDragHandler
{
    public static ItemMouseDragHandler? current;
    public bool IsDraggingItem;
}
