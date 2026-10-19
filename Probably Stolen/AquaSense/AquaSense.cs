using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(AquaSense.Core), "AquaSense", "1.1.0", "chiqingsan")]
[assembly: MelonGame("Questing Goose Studio", "Probably Stolen")]

namespace AquaSense;

public sealed class Core : MelonMod
{
    private float _nextFilterCheckTime;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        try
        {
            var category = MelonPreferences.CreateCategory("AquaSense", "AquaSense");
            AutoReplaceFilters = category.CreateEntry(
                "AutoReplaceFilters",
                true,
                "滤芯耗尽后自动更换");
            ShowFilterDurability = category.CreateEntry(
                "ShowFilterDurability",
                true,
                "悬浮面板显示滤芯耐久");
            AutoUpdateWaterLabels = category.CreateEntry(
                "AutoUpdateWaterLabels",
                true,
                "倒水后自动更新水质标签");
        }
        catch
        {
        }
    }

    public override void OnLateInitializeMelon()
    {
        try
        {
            var harmony = new HarmonyLib.Harmony("chiqingsan.water-scan-tooltip");
            Patch(harmony, typeof(WaterHelper), nameof(WaterHelper.CreateWaterContainerTooltip),
                new[] { typeof(RichTextBuilder), typeof(GameItem) }, nameof(Hooks.AfterWaterContainerTooltip));
            Patch(harmony, typeof(WaterFeatureHelper), nameof(WaterFeatureHelper.CreateWaterFeatureTooltip),
                new[] { typeof(RichTextBuilder), typeof(GameItem) }, nameof(Hooks.AfterWaterFeatureTooltip));
            Patch(harmony, typeof(WaterHelper), nameof(WaterHelper.TransferLiquid),
                new[] { typeof(GameItem), typeof(GameItem) }, nameof(Hooks.AfterLiquidTransfer));
            Patch(harmony, typeof(WaterFeatureHelper), nameof(WaterFeatureHelper.OnLiquidContainerPourInto),
                new[] { typeof(GameItem), typeof(GameItem), typeof(bool) }, nameof(Hooks.AfterLiquidPour));
        }
        catch
        {
        }
    }

    public override void OnUpdate()
    {
        if (AutoReplaceFilters is not { Value: true })
            return;

        if (Time.unscaledTime < _nextFilterCheckTime)
            return;

        _nextFilterCheckTime = Time.unscaledTime + 2f;
        Hooks.TryReplaceDepletedFilters();
    }

    internal static MelonPreferences_Entry<bool>? AutoReplaceFilters { get; private set; }

    internal static MelonPreferences_Entry<bool>? ShowFilterDurability { get; private set; }

    internal static MelonPreferences_Entry<bool>? AutoUpdateWaterLabels { get; private set; }

    internal static MelonLogger.Instance? Log { get; private set; }

    private static void Patch(
        HarmonyLib.Harmony harmony,
        Type type,
        string methodName,
        Type[] argumentTypes,
        string postfixName)
    {
        var original = AccessTools.Method(type, methodName, argumentTypes);
        if (original == null)
            return;

        var postfix = AccessTools.Method(typeof(Hooks), postfixName);
        if (postfix == null)
            return;

        harmony.Patch(original, postfix: new HarmonyMethod(postfix));
    }

}

internal static class Hooks
{
    private static float _scannerCacheExpiresAt = -1f;
    private static bool _scannerCache;
    private static int _lastTooltipFrame = -1;
    private static IntPtr _lastTooltipItem = IntPtr.Zero;
    private static int _lastLabelFrame = -1;
    private static IntPtr _lastLabelItem = IntPtr.Zero;
    private static bool _filterReplacementRunning;
    private static bool _vanillaFilterIdentityLoaded;
    private static string _vanillaFilterIdentifier = string.Empty;
    private static string _vanillaFilterName = string.Empty;
    private static readonly HashSet<string> KnownWaterContainerIdentifiers = new(
        StringComparer.OrdinalIgnoreCase)
    {
        "water_jug",
        "water_pitcher",
        "bottled_water",
        "bottled_water_premium",
        "large_bottled_water",
        "small_bottled_water",
        "water_ration_small",
        "dspf_future_void_jug",
        "watertower_tank",
        "water_purifier",
        "portable_water_purifier",
        "dspf_future_purifier"
    };

    public static void AfterWaterContainerTooltip(RichTextBuilder __0, GameItem __1)
    {
        AppendScanTooltip(__0, __1);
    }

    public static void AfterWaterFeatureTooltip(RichTextBuilder __0, GameItem __1)
    {
        AppendScanTooltip(__0, __1);
    }

    public static void AfterLiquidTransfer(GameItem __0, GameItem __1)
    {
        TryAutoLabel(__0);
        TryAutoLabel(__1);
    }

    public static void AfterLiquidPour(GameItem __0, GameItem __1)
    {
        TryAutoLabel(__0);
        TryAutoLabel(__1);
    }

    private static void AppendScanTooltip(RichTextBuilder builder, GameItem item)
    {
        if (builder == null || item == null)
            return;

        try
        {
            var frame = Time.frameCount;
            var itemPointer = item.Pointer;
            if (frame == _lastTooltipFrame && itemPointer == _lastTooltipItem)
                return;

            _lastTooltipFrame = frame;
            _lastTooltipItem = itemPointer;

            if (!HasWaterScanner() || !IsWaterContainer(item))
                return;

            // The water composition section needs actual liquid to describe; the
            // filter durability line does not, so an empty jug still shows it.
            if (IsFilledWaterContainer(item))
            {
                var purity = WaterHelper.GetWaterPurity(item);
                var locale = LocHelper.GetCurrentLocaleCode();
                var titlePrefix = IsChineseLocale(locale)
                    ? $"水质详情 ({purity})"
                    : $"Water composition ({purity})";
                var waterLabel = GetWaterLabel(item);
                var title = string.IsNullOrEmpty(waterLabel)
                    ? titlePrefix
                    : $"{titlePrefix}  {waterLabel}";

                AddLine(builder, title);
                AppendComposition(builder, item);
            }

            AppendFilterDurability(builder, item);
        }
        catch
        {
        }
    }

    private static void AppendFilterDurability(RichTextBuilder builder, GameItem container)
    {
        if (Core.ShowFilterDurability is not { Value: true })
            return;

        GameItem? filter;
        try
        {
            filter = WaterHelper.GetFilterItem(container);
        }
        catch
        {
            return;
        }

        if (filter == null)
            return;

        // The slot can also hold a UV lamp; the durability line is for filters only.
        if (!IsFilterItem(filter))
            return;

        var name = GetDisplayName(filter);

        int current;
        try
        {
            current = DurabilityHelper.GetRealDurability(filter);
        }
        catch
        {
            return;
        }

        var maximum = 0;
        try
        {
            maximum = DurabilityHelper.GetMaxDurability(filter);
        }
        catch
        {
        }

        var durability = maximum > 0
            ? $"{current}/{maximum}"
            : current.ToString();

        AddColoredLine(
            builder,
            IsChineseLocale()
                ? $"[{name}]  耐久度:  {durability}"
                : $"[{name}]  Durability:  {durability}",
            FilterDurabilityColor);
    }

    private static string GetWaterLabel(GameItem item)
    {
        try
        {
            return WaterFeatureHelper.GetWaterDisplayName(item) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static void TryAutoLabel(GameItem container)
    {
        if (Core.AutoUpdateWaterLabels is not { Value: true })
            return;

        if (!HasWaterScanner() ||
            container == null ||
            !IsFilledWaterContainer(container))
            return;

        try
        {
            var frame = Time.frameCount;
            var itemPointer = container.Pointer;
            if (frame == _lastLabelFrame && itemPointer == _lastLabelItem)
                return;

            _lastLabelFrame = frame;
            _lastLabelItem = itemPointer;

            WaterHelper.UpdateContainerGoodType(container);
            WaterFeatureHelper.UpdateWaterFeature(container, true);
            WaterFeatureHelper.OnWaterLabelUpdated(container);
        }
        catch
        {
        }
    }

    private static bool HasWaterScanner()
    {
        var now = Time.unscaledTime;
        if (now < _scannerCacheExpiresAt)
            return _scannerCache;

        _scannerCacheExpiresAt = now + 1f;
        _scannerCache = false;

        try
        {
            if (!PlayerStore.IsInstanceExist())
                return false;

            var store = PlayerStore.Instance;
            if (store == null)
                return false;

            var items = CollectOwnedItems(store);
            if (items.Count == 0)
                return false;

            foreach (var item in items)
            {
                if (IsWaterScanner(item))
                {
                    _scannerCache = true;
                    break;
                }
            }
        }
        catch
        {
        }

        return _scannerCache;
    }

    private static bool IsWaterScanner(GameItem item)
    {
        if (item == null)
            return false;

        try
        {
            return NormalizeIdentifier(item.identifier ?? string.Empty) == "aquascan" ||
                   NormalizeIdentifier(item.name ?? string.Empty) == "aquascan";
        }
        catch
        {
            return false;
        }
    }

    private static bool IsChineseLocale(string? locale)
    {
        return !string.IsNullOrEmpty(locale) &&
               locale.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsChineseLocale()
    {
        try
        {
            return IsChineseLocale(LocHelper.GetCurrentLocaleCode());
        }
        catch
        {
            return false;
        }
    }

    private static bool IsFilledWaterContainer(GameItem item)
    {
        try
        {
            return IsWaterContainer(item) && !WaterHelper.IsEmpty(item);
        }
        catch
        {
            return false;
        }
    }

    private static void AppendComposition(RichTextBuilder builder, GameItem item)
    {
        var liquidDictionary = Liquid.LiquidDict;
        var liquids = Liquid.Liquids;
        var entries = new List<(string Name, int Amount)>();
        var totalAmount = 0;

        if (liquidDictionary != null)
        {
            foreach (var pair in liquidDictionary!)
            {
                AddLiquidEntry(entries, item, pair.Value!, ref totalAmount);
            }
        }
        else if (liquids != null)
        {
            foreach (var liquid in liquids)
            {
                AddLiquidEntry(entries, item, liquid, ref totalAmount);
            }
        }

        if (entries.Count == 0 || totalAmount <= 0)
        {
            var color = WaterHelper.GetLiquidContainerColor(item);
            if (!string.IsNullOrEmpty(color))
                AddLine(builder, $"Color: {color}");
            return;
        }

        foreach (var entry in entries)
        {
            var percentage = 100d * entry.Amount / totalAmount;
            if (Math.Round(percentage, 2, MidpointRounding.AwayFromZero) <= 0d)
                continue;

            var percentageText = WaterFeatureHelper.FormatPurityPercentageFromDouble(percentage);
            if (string.IsNullOrEmpty(percentageText))
                percentageText = $"{percentage:0.00}%";

            AddLine(builder, $"{entry.Name}: {FormatVolume(entry.Amount)} ({percentageText})");
        }
    }

    private static void AddLiquidEntry(
        List<(string Name, int Amount)> entries,
        GameItem item,
        Liquid? liquid,
        ref int totalAmount)
    {
        if (liquid == null)
            return;

        var amount = ReadLiquidAmount(item, liquid);
        if (amount <= 0)
            return;

        var name = liquid.displayName;
        if (string.IsNullOrEmpty(name))
            name = liquid.identifier;
        if (string.IsNullOrEmpty(name))
            return;

        entries.Add((name, amount));
        totalAmount += amount;
    }

    private static int ReadLiquidAmount(GameItem item, Liquid liquid)
    {
        var currentPart = liquid.currentPart;
        if (!string.IsNullOrEmpty(currentPart))
        {
            var amount = ReadTagInt(item, currentPart);
            if (amount > 0)
                return amount;
        }

        var part = liquid.part;
        return string.IsNullOrEmpty(part) ? 0 : ReadTagInt(item, part);
    }

    private static int ReadTagInt(GameItem item, string identifier)
    {
        var state = item.state;
        var tag = state?.GetTag(identifier);
        return tag?.valueInt ?? 0;
    }

    private static string FormatVolume(int amountInMicroLiters)
    {
        return $"{amountInMicroLiters / 1000d:0.##} ml";
    }

    internal static void TryReplaceDepletedFilters()
    {
        if (_filterReplacementRunning)
            return;

        _filterReplacementRunning = true;
        try
        {
            if (!PlayerStore.IsInstanceExist())
                return;

            var store = PlayerStore.Instance;
            if (store == null)
                return;

            var allItems = CollectOwnedItems(store);
            if (allItems.Count == 0)
                return;

            foreach (var item in allItems)
            {
                if (!IsWaterContainer(item))
                    continue;

                var container = item;
                var depletedFilter = WaterHelper.GetFilterItem(container);
                if (depletedFilter == null)
                    continue;

                // The container holds exactly one insertable item, and it is not
                // always a filter: a UV lamp shares that slot and also has
                // durability. Without this check a spent lamp is treated as a
                // depleted filter and swapped out for one.
                if (!IsFilterItem(depletedFilter))
                    continue;

                // A filter that has just hit zero durability may already be in a
                // transient state where reading it throws. Treating a failed read
                // as "still good" would skip exactly the case this feature exists
                // for, so only a readable, positive durability means "leave alone".
                var filterIsAlive = true;
                try
                {
                    filterIsAlive = DurabilityHelper.GetRealDurability(depletedFilter) > 0;
                }
                catch
                {
                    filterIsAlive = false;
                }

                if (filterIsAlive)
                    continue;

                var replacement = FindReplacementFilter(allItems, container, depletedFilter);
                if (replacement == null)
                    continue;

                // The precheck reads inventory state that the swap itself will
                // mutate, so it can only reject obviously impossible swaps; the
                // real guarantee comes from verifying after the swap.
                if (!CanSwapFilterPlacement(depletedFilter, replacement))
                    continue;

                if (ReplaceFilter(container, depletedFilter, replacement, out var toastMessage))
                {
                    ShowToast(toastMessage);
                }
                else
                {
                    Core.Log?.Warning(
                        $"Filter swap for {SafeIdentifier(depletedFilter)} did not complete; inventory left unchanged.");
                }
            }
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Filter replacement pass failed: {ex.Message}");
        }
        finally
        {
            _filterReplacementRunning = false;
        }
    }

    private static string SafeIdentifier(GameItem item)
    {
        try
        {
            return item.identifier ?? "<unknown>";
        }
        catch
        {
            return "<unknown>";
        }
    }

    private static List<GameItem> CollectOwnedItems(PlayerStore store)
    {
        var result = new List<GameItem>();
        var pending = new Queue<GameItem>();
        var visited = new HashSet<IntPtr>();
        var visitedStorages = new HashSet<IntPtr>();

        AddItemRoots(store.FindAllItem(true), pending, visited);

        try
        {
            AddItemRoots(store.GetAllRepoEligibleItems(), pending, visited);
        }
        catch
        {
        }

        while (pending.Count > 0)
        {
            var item = pending.Dequeue();
            result.Add(item);

            try
            {
                AddItemRoots(item.FindAllChildItems(), pending, visited);
            }
            catch
            {
            }

            try
            {
                AddNestedWindowItems(item.child, pending, visited, visitedStorages);
            }
            catch
            {
            }

            try
            {
                foreach (var child in item.children)
                {
                    AddNestedStorageItems(
                        child,
                        pending,
                        visited,
                        visitedStorages);
                }
            }
            catch
            {
            }
        }

        return result;
    }

    private static void AddItemRoots(
        Il2CppSystem.Collections.Generic.List<GameItem>? items,
        Queue<GameItem> pending,
        HashSet<IntPtr> visited)
    {
        if (items == null)
            return;

        foreach (var item in items)
        {
            if (item == null || !visited.Add(item.Pointer))
                continue;

            pending.Enqueue(item);
        }
    }

    private static void AddNestedWindowItems(
        GraphNodeWindow? window,
        Queue<GameItem> pending,
        HashSet<IntPtr> visited,
        HashSet<IntPtr> visitedStorages)
    {
        if (window == null)
            return;

        var inventory = window.TryCast<GameInventory>();
        if (inventory != null)
            AddInventoryItems(inventory, pending, visited, visitedStorages);

        AddNestedStorageItems(
            window.child,
            pending,
            visited,
            visitedStorages);
    }

    private static void AddNestedStorageItems(
        GraphNodeStorage? storage,
        Queue<GameItem> pending,
        HashSet<IntPtr> visited,
        HashSet<IntPtr> visitedStorages)
    {
        if (storage == null || !visitedStorages.Add(storage.Pointer))
            return;

        var inventory = storage.TryCast<GameInventory>();
        if (inventory != null)
            AddInventoryItems(inventory, pending, visited, visitedStorages);

        var item = storage.TryCast<GameItem>();
        if (item != null)
            AddItemRoot(item, pending, visited);

        try
        {
            foreach (var child in storage.children)
                AddNestedStorageItems(
                    child,
                    pending,
                    visited,
                    visitedStorages);
        }
        catch
        {
        }
    }

    private static void AddInventoryItems(
        GameInventory inventory,
        Queue<GameItem> pending,
        HashSet<IntPtr> visited,
        HashSet<IntPtr> visitedStorages)
    {
        AddItemRoots(inventory.childItems, pending, visited);

        var slotInventory = inventory.TryCast<GameSlotInventory>();
        if (slotInventory != null)
            AddItemRoot(slotInventory.childItem, pending, visited);

        var gridInventory = inventory.TryCast<GameGridInventory>();
        if (gridInventory != null)
            AddItemRoots(gridInventory.items, pending, visited);

        var scrollableInventory = inventory.TryCast<GameGridScrollableInventory>();
        if (scrollableInventory != null)
            AddItemRoots(scrollableInventory.items, pending, visited);

        try
        {
            foreach (var child in inventory.children)
            {
                    AddNestedStorageItems(
                        child,
                        pending,
                        visited,
                        visitedStorages);
            }
        }
        catch
        {
        }
    }

    private static void AddItemRoot(
        GameItem item,
        Queue<GameItem> pending,
        HashSet<IntPtr> visited)
    {
        if (item != null && visited.Add(item.Pointer))
            pending.Enqueue(item);
    }

    private sealed class GridInventoryInfo
    {
        internal GridInventoryInfo(
            int minX,
            int minY,
            int width,
            int height,
            Il2CppSystem.Collections.Generic.List<GameItem>? items)
        {
            MinX = minX;
            MinY = minY;
            Width = width;
            Height = height;
            Items = items;
        }

        internal int MinX { get; }
        internal int MinY { get; }
        internal int Width { get; }
        internal int Height { get; }
        internal Il2CppSystem.Collections.Generic.List<GameItem>? Items { get; }
    }

    private readonly struct ItemBounds
    {
        internal ItemBounds(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        internal int Left { get; }
        internal int Top { get; }
        internal int Right { get; }
        internal int Bottom { get; }
    }

    private static bool CanSwapFilterPlacement(
        GameItem depletedFilter,
        GameItem replacement)
    {
        var targetInventory = depletedFilter.parentInventory;
        var sourceInventory = replacement.parentInventory;
        if (targetInventory == null || sourceInventory == null)
            return false;

        if (!CanPlaceAtRecordedPosition(
                depletedFilter,
                replacement,
                sourceInventory))
            return false;

        if (!CanPlaceAtRecordedPosition(
                replacement,
                depletedFilter,
                targetInventory))
            return false;

        return true;
    }

    private static bool CanPlaceAtRecordedPosition(
        GameItem itemToPlace,
        GameItem slotItem,
        GameInventory inventory)
    {
        var grid = GetGridInventoryInfo(inventory);
        if (grid == null)
        {
            // A slot inventory holds exactly one item, so it can only accept the
            // incoming item if that slot is the one the two items are trading.
            // Approving it unconditionally is what allowed occupied slots to be
            // treated as free.
            if (inventory.TryCast<GameSlotInventory>() != null)
            {
                if (IsInsertLocked(inventory))
                    return false;

                var occupant = GetSlotOccupant(inventory);
                return occupant == null ||
                       occupant.Pointer == itemToPlace.Pointer ||
                       occupant.Pointer == slotItem.Pointer;
            }

            try
            {
                return inventory.MayHaveValidInventorySlot(itemToPlace);
            }
            catch
            {
            }

            return false;
        }

        var itemShape = itemToPlace.modifiedShape ?? itemToPlace.shape;
        var slotShape = slotItem.modifiedShape ?? slotItem.shape;
        if (itemShape == null || slotShape == null)
            return false;

        var itemBounds = GetItemBoundsAtSlot(itemShape, slotItem);
        var inventoryRight = grid.MinX + grid.Width - 1;
        var inventoryBottom = grid.MinY + grid.Height - 1;
        if (itemBounds.Left < grid.MinX ||
            itemBounds.Top < grid.MinY ||
            itemBounds.Right > inventoryRight ||
            itemBounds.Bottom > inventoryBottom)
            return false;

        var checkedItems = new HashSet<IntPtr>();
        foreach (var other in EnumerateGridItems(inventory, grid))
        {
            if (other == null || !checkedItems.Add(other.Pointer))
            {
                continue;
            }

            if (other.Pointer == itemToPlace.Pointer ||
                other.Pointer == slotItem.Pointer)
            {
                continue;
            }

            var otherShape = other.modifiedShape ?? other.shape;
            if (otherShape == null)
                continue;

            var otherBounds = GetItemBounds(
                other,
                other.modifiedXOrigin,
                other.modifiedYOrigin,
                otherShape);
            if (BoundsOverlap(itemBounds, otherBounds))
                return false;
        }

        return true;
    }

    private static IEnumerable<GameItem> EnumerateGridItems(
        GameInventory inventory,
        GridInventoryInfo grid)
    {
        if (grid.Items != null)
        {
            foreach (var item in grid.Items)
                yield return item;
        }

        foreach (var item in inventory.childItems)
            yield return item;
    }

    private static GridInventoryInfo? GetGridInventoryInfo(GameInventory inventory)
    {
        var gridInventory = inventory.TryCast<GameGridInventory>();
        if (gridInventory != null)
        {
            var shape = gridInventory.inventoryShape;
            if (shape == null)
                return null;

            return new GridInventoryInfo(
                shape.minX,
                shape.minY,
                Math.Max(shape.globalWidth, shape.width),
                Math.Max(shape.globalHeight, shape.height),
                gridInventory.items);
        }

        var scrollableInventory = inventory.TryCast<GameGridScrollableInventory>();
        if (scrollableInventory != null)
        {
            return new GridInventoryInfo(
                0,
                0,
                scrollableInventory.width,
                scrollableInventory.height,
                scrollableInventory.items);
        }

        return null;
    }

    private static ItemBounds GetItemBounds(
        GameItem item,
        int originX,
        int originY,
        GridShape shape)
    {
        var minX = shape.minX;
        var minY = shape.minY;
        var maxX = shape.maxX;
        var maxY = shape.maxY;
        if (maxX < minX)
            maxX = minX + Math.Max(shape.width, 1) - 1;
        if (maxY < minY)
            maxY = minY + Math.Max(shape.height, 1) - 1;

        return new ItemBounds(
            originX + minX,
            originY + minY,
            originX + maxX,
            originY + maxY);
    }

    private static ItemBounds GetItemBoundsAtSlot(GridShape itemShape, GameItem slotItem)
    {
        var slotShape = slotItem.modifiedShape ?? slotItem.shape;
        var slotX = slotShape?.minX ?? slotItem.modifiedXOrigin;
        var slotY = slotShape?.minY ?? slotItem.modifiedYOrigin;
        var width = Math.Max(
            itemShape.globalWidth,
            Math.Max(itemShape.width, 1));
        var height = Math.Max(
            itemShape.globalHeight,
            Math.Max(itemShape.height, 1));

        return new ItemBounds(
            slotX,
            slotY,
            slotX + width - 1,
            slotY + height - 1);
    }

    private static bool BoundsOverlap(ItemBounds left, ItemBounds right)
    {
        return left.Left <= right.Right &&
               left.Right >= right.Left &&
               left.Top <= right.Bottom &&
               left.Bottom >= right.Top;
    }

    private static GameItem? FindReplacementFilter(
        List<GameItem> allItems,
        GameItem container,
        GameItem depletedFilter)
    {
        GameItem? best = null;
        var bestPriority = int.MaxValue;

        foreach (var item in allItems)
        {
            if (!IsAvailableFilter(item, container, depletedFilter))
                continue;

            var priority = GetFilterPriority(item);
            if (priority < bestPriority)
            {
                best = item;
                bestPriority = priority;
            }
        }

        return best;
    }

    private static bool IsAvailableFilter(GameItem item, GameItem container, GameItem depletedFilter)
    {
        if (item == null ||
            item.Pointer == container.Pointer ||
            item.Pointer == depletedFilter.Pointer ||
            !IsFilterItem(item) ||
            item.parentInventory == null ||
            IsInstalledInWaterContainer(item))
        {
            return false;
        }

        try
        {
            return DurabilityHelper.GetRealDurability(item) > 0;
        }
        catch
        {
            return false;
        }
    }

    private static bool ReplaceFilter(
        GameItem container,
        GameItem depletedFilter,
        GameItem replacement,
        out string toastMessage)
    {
        toastMessage = string.Empty;

        // Re-read the live parents instead of trusting whatever the precheck saw:
        // the container may have been moved, or the filter slot re-populated,
        // since the item list was collected.
        var targetInventory = GetParentInventorySafely(depletedFilter);
        var sourceInventory = GetParentInventorySafely(replacement);
        if (targetInventory == null || sourceInventory == null)
            return false;

        if (SameInventory(targetInventory, sourceInventory))
            return false;

        if (IsInsertLocked(targetInventory) || IsInsertLocked(sourceInventory))
            return false;

        // Names are read before the swap so the toast still reports what was
        // replaced even if the swap leaves an item in a transient state.
        var depletedName = GetDisplayName(depletedFilter);
        var replacementName = GetDisplayName(replacement);

        bool swapped;
        try
        {
            swapped = GraphUtils.SwapItemsInPlace(depletedFilter, replacement);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"SwapItemsInPlace threw: {ex.Message}");
            swapped = false;
        }

        // SwapItemsInPlace is not guaranteed to be atomic: if the second half of
        // the move fails, an item can end up detached from every inventory. Verify
        // both landed where intended and repair the container slot if not.
        var depletedPlaced = IsInInventory(depletedFilter, sourceInventory);
        var replacementPlaced = IsInInventory(replacement, targetInventory);

        if (!swapped || !depletedPlaced || !replacementPlaced)
        {
            RecoverFilterSwap(container, depletedFilter, replacement, targetInventory, sourceInventory);
            return false;
        }

        Validate(targetInventory);
        Validate(sourceInventory);
        Validate(container);

        BuildFilterToast(depletedName, replacementName, out toastMessage);
        return true;
    }

    private static void BuildFilterToast(
        string depletedName,
        string replacementName,
        out string message)
    {
        message = IsChineseLocale()
            ? $"{depletedName}耐久已耗尽, 已替换为{replacementName}"
            : $"{depletedName} ran out of durability and was replaced with {replacementName}";
    }

    private static string GetDisplayName(GameItem item)
    {
        try
        {
            var displayName = item.GetDisplayName(false);
            if (!string.IsNullOrEmpty(displayName))
                return displayName;
        }
        catch
        {
        }

        return SafeIdentifier(item);
    }

    private static void ShowToast(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        try
        {
            // Notify(string message, string color = "default"): the second argument
            // is a colour label, not a body. Passing the body there made the game
            // show the title alone.
            StoreUIManager.Instance?.Notify(message, "default");
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Could not show filter replacement toast: {ex.Message}");
        }
    }

    // Puts the depleted filter back into the container slot when a swap leaves it
    // detached, so a failed replacement never results in an empty filter slot.
    private static void RecoverFilterSwap(
        GameItem container,
        GameItem depletedFilter,
        GameItem replacement,
        GameInventory targetInventory,
        GameInventory sourceInventory)
    {
        try
        {
            // The exhausted filter must end up back in the slot it came from.
            if (IsDetached(depletedFilter) && !AcceptInto(targetInventory, depletedFilter))
            {
                Core.Log?.Warning(
                    $"Could not return {SafeIdentifier(depletedFilter)} to the water container; " +
                    "the filter may need to be re-inserted manually.");
            }

            // A replacement that never made it in goes back to its original spot;
            // it is never pushed into some unrelated inventory.
            if (IsDetached(replacement) && !AcceptInto(sourceInventory, replacement))
            {
                Core.Log?.Warning(
                    $"Could not return {SafeIdentifier(replacement)} to its original inventory.");
            }
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Filter recovery failed: {ex.Message}");
        }
        finally
        {
            Validate(targetInventory);
            Validate(sourceInventory);
            Validate(container);
        }
    }

    private static bool AcceptInto(GameInventory inventory, GameItem item)
    {
        try
        {
            var slot = inventory.TryFindOneValidInventorySlot(item);
            if (slot == null)
                return false;

            slot.AcceptUnchecked();
            return IsInInventory(item, inventory);
        }
        catch
        {
            return false;
        }
    }

    private static bool IsInInventory(GameItem item, GameInventory inventory)
    {
        var parent = GetParentInventorySafely(item);
        return parent != null && SameInventory(parent, inventory);
    }

    private static bool IsDetached(GameItem item)
    {
        return GetParentInventorySafely(item) == null;
    }

    private static GameInventory? GetParentInventorySafely(GameItem item)
    {
        try
        {
            return item.parentInventory;
        }
        catch
        {
            return null;
        }
    }

    private static bool SameInventory(GameInventory left, GameInventory right)
    {
        try
        {
            return left.Pointer == right.Pointer;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsInsertLocked(GameInventory inventory)
    {
        try
        {
            return inventory.IsInsertLocked();
        }
        catch
        {
            return false;
        }
    }

    private static GameItem? GetSlotOccupant(GameInventory inventory)
    {
        var slot = inventory.TryCast<GameSlotInventory>();
        if (slot == null)
            return null;

        try
        {
            return slot.childItem;
        }
        catch
        {
            return null;
        }
    }

    private static void Validate(GameInventory inventory)
    {
        try
        {
            inventory.Validate();
        }
        catch
        {
        }
    }

    private static void Validate(GameItem item)
    {
        try
        {
            item.Validate();
        }
        catch
        {
        }
    }

    private static bool IsInstalledInWaterContainer(GameItem item)
    {
        var inventory = item.parentInventory;
        for (var depth = 0; inventory != null && depth < 16; depth++)
        {
            var parentItem = inventory.GetParentItem();
            if (parentItem == null)
                return false;

            if (IsWaterContainer(parentItem))
                return true;

            inventory = parentItem.parentInventory;
        }

        return false;
    }

    private static bool IsWaterContainer(GameItem item)
    {
        try
        {
            var identifier = item.identifier ?? string.Empty;
            return KnownWaterContainerIdentifiers.Contains(identifier);
        }
        catch
        {
            return false;
        }
    }

    // Items that occupy the water-container slot but are not water filters.
    // Matched on identifier substrings because the id is stable while the
    // display name is localized. Confirmed against the game's item data:
    //   uv_filter       紫外线灯（杀菌灯具）
    //   hydroreed_filter 韧棉织物（工业布料，游戏描述明确说无法用于净水）
    //   black_lamp       黑光灯
    private static readonly string[] NonFilterSlotIdentifiers =
    {
        "uv_filter",
        "uvfilter",
        "uv_lamp",
        "uvlamp",
        "black_lamp",
        "hydroreed"
    };

    private static bool IsFilterItem(GameItem item)
    {
        try
        {
            var identifier = (item.identifier ?? string.Empty).ToLowerInvariant();
            var name = (item.name ?? string.Empty).ToLowerInvariant();
            var text = $"{identifier} {name}";

            // Substring matching on "filter" alone is not safe here: the game's
            // UV lamp identifier is `uv_filter` and the industrial cloth is
            // `hydroreed_filter`, so both would pass a naive "contains filter"
            // test. Exclude the known non-filter slot items first.
            foreach (var blocked in NonFilterSlotIdentifiers)
            {
                if (text.Contains(blocked))
                    return false;
            }

            return text.Contains("filter") ||
                   text.Contains("过滤器") ||
                   text.Contains("滤水器");
        }
        catch
        {
            return false;
        }
    }

    private static int GetFilterPriority(GameItem item)
    {
        try
        {
            var identifier = (item.identifier ?? string.Empty).ToLowerInvariant();
            var name = (item.name ?? string.Empty).ToLowerInvariant();
            var text = $"{identifier} {name}";

            if (text.Contains("advanced") ||
                text.Contains("adv_filter") ||
                text.Contains("high_filter") ||
                text.Contains("高级"))
            {
                return 0;
            }

            if (IsVanillaStandardFilter(item))
                return 1;

            return 2;
        }
        catch
        {
            return 2;
        }
    }

    private static bool IsVanillaStandardFilter(GameItem item)
    {
        EnsureVanillaFilterIdentity();
        var identifier = item.identifier ?? string.Empty;
        var name = item.name ?? string.Empty;

        return (!string.IsNullOrEmpty(_vanillaFilterIdentifier) &&
                string.Equals(identifier, _vanillaFilterIdentifier, StringComparison.OrdinalIgnoreCase)) ||
               (!string.IsNullOrEmpty(_vanillaFilterName) &&
                string.Equals(name, _vanillaFilterName, StringComparison.OrdinalIgnoreCase)) ||
               NormalizeIdentifier(identifier) == "waterfilter";
    }

    private static void EnsureVanillaFilterIdentity()
    {
        if (_vanillaFilterIdentityLoaded)
            return;

        try
        {
            var prototype = PreBuiltItemHelper.WaterFilter();
            if (prototype == null)
                return;

            _vanillaFilterIdentifier = prototype.identifier ?? string.Empty;
            _vanillaFilterName = prototype.name ?? string.Empty;
            _vanillaFilterIdentityLoaded = true;
            prototype.Destroy();
        }
        catch
        {
        }
    }

    private static string NormalizeIdentifier(string value)
    {
        return value.Replace("_", string.Empty)
            .Replace("-", string.Empty)
            .Replace(" ", string.Empty)
            .ToLowerInvariant();
    }

    private static void AddLine(RichTextBuilder builder, string text)
    {
        AddColoredLine(builder, text, NoColor);
    }

    // RenderHandler.ColorPalette is an int-backed enum whose values are literal
    // 0xRRGGBB colors (White = 12895408 = 0xC4C4B0), so any hex can be passed
    // by casting it.
    private const int NoColor = -1;

    private const int FilterDurabilityColor = 0xCCB987;

    private static void AddColoredLine(RichTextBuilder builder, string text, int color)
    {
        // AddLine(text, ignoreEmpty, color, bold, italic, underline, strikethrough,
        //         backgroundColor, underlineColor, strikethroughColor)
        // Only the text colour may be set here: passing the same value as
        // backgroundColor paints a full-width band behind the line.
        var textColor = (RenderHandler.ColorPalette)color;
        var none = (RenderHandler.ColorPalette)NoColor;

        builder.AddLine(
            text,
            true,
            textColor,
            false,
            false,
            false,
            false,
            none,
            none,
            none);
    }
}
