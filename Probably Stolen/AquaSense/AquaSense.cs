using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(AquaSense.Core), "AquaSense", "1.1.2", "chiqingsan")]
[assembly: MelonGame("Questing Goose Studio", "Probably Stolen")]

namespace AquaSense;

public sealed class Core : MelonMod
{
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
        catch (Exception ex)
        {
            Log.Warning($"Preference initialization failed: {ex.Message}");
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
            Patch(harmony, typeof(GameItemElement), "GetTooltipBasic",
                Type.EmptyTypes, nameof(Hooks.AfterBasicItemTooltip));
            Patch(harmony, typeof(PlayerStore), nameof(PlayerStore.LoadGame),
                Type.EmptyTypes, nameof(Hooks.AfterLoadGame));
        }
        catch (Exception ex)
        {
            Log?.Warning($"Patch initialization failed: {ex.Message}");
        }
    }

    public override void OnUpdate()
    {
        Hooks.UpdateMaintenance(AutoReplaceFilters is { Value: true });
    }

    public override void OnDeinitializeMelon()
    {
        Hooks.ResetMaintenance();
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
        try
        {
            var original = AccessTools.Method(type, methodName, argumentTypes);
            var postfix = AccessTools.Method(typeof(Hooks), postfixName);
            if (original == null || postfix == null)
            {
                Log?.Warning($"Missing patch method: {type.Name}.{methodName} / {postfixName}");
                return;
            }
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
        }
        catch (Exception ex)
        {
            Log?.Warning($"Could not patch {type.Name}.{methodName}: {ex.Message}");
        }
    }

}

internal static class Hooks
{
    private const double MaintenanceBudgetMilliseconds = 2d;
    private static float _scannerCacheExpiresAt = -1f;
    private static bool _scannerCache;
    private static bool _inventoryRefreshRequested;
    private static float _nextMaintenanceAt;
    private static IntPtr _storePointer;
    private static IEnumerator<object?>? _maintenance;
    private static Func<bool>? _retryFilterRecovery;
    private static int _lastTooltipFrame = -1;
    private static readonly Dictionary<(IntPtr Builder, IntPtr Item), (int Index, IntPtr Slice)> TooltipSections = new();
    private static int _lastLabelFrame = -1;
    private static IntPtr _lastLabelItem = IntPtr.Zero;
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
        "dspf_future_purifier",
        // BrewingExpansion 3.6.2 water containers.
        "be_xuxu_super_water_bottle",
        "be_xuxu_small_water_bottle",
        "be_xuxu_slim_water_bottle",
        "be_clear_brew_water_bucket"
    };

    public static void AfterWaterContainerTooltip(RichTextBuilder __0, GameItem __1)
    {
        AppendScanTooltip(__0, __1);
    }

    public static void AfterWaterFeatureTooltip(RichTextBuilder __0, GameItem __1)
    {
        AppendScanTooltip(__0, __1);
    }

    public static void AfterBasicItemTooltip(GameItemElement __instance, RichTextBuilder __result)
    {
        AppendScanTooltip(__result, __instance);
    }

    public static void AfterLoadGame()
    {
        ResetMaintenance();
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
            if (!IsWaterContainer(item) || !HasWaterScanner())
                return;

            var frame = Time.frameCount;
            if (_lastTooltipFrame != frame)
            {
                TooltipSections.Clear();
                _lastTooltipFrame = frame;
            }

            var key = (builder.Pointer, item.Pointer);
            var slices = builder.slices;
            if (TooltipSections.TryGetValue(key, out var section) &&
                slices != null && section.Index < slices.Count &&
                slices[section.Index].Pointer == section.Slice)
                return;

            var firstSlice = slices?.Count ?? 0;

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
            slices = builder.slices;
            if (slices != null && firstSlice < slices.Count)
                TooltipSections[key] = (firstSlice, slices[firstSlice].Pointer);
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
        if (Time.unscaledTime >= _scannerCacheExpiresAt)
            _inventoryRefreshRequested = true;

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
        long totalAmount = 0;

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
        ref long totalAmount)
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

    internal static void UpdateMaintenance(bool replaceFilters)
    {
        if (_maintenance == null && _retryFilterRecovery == null && !_inventoryRefreshRequested &&
            _scannerCacheExpiresAt >= 0f &&
            (!replaceFilters || Time.unscaledTime < _nextMaintenanceAt))
            return;

        if (!PlayerStore.IsInstanceExist() || PlayerStore.Instance == null)
        {
            ResetMaintenance();
            return;
        }

        var store = PlayerStore.Instance;
        if (_storePointer != store.Pointer)
        {
            ResetMaintenance();
            _storePointer = store.Pointer;
        }

        if (_retryFilterRecovery != null)
        {
            if (Time.unscaledTime >= _nextMaintenanceAt)
            {
                if (_retryFilterRecovery())
                    _retryFilterRecovery = null;
                _nextMaintenanceAt = Time.unscaledTime + 2f;
            }
            return;
        }

        if (_maintenance == null)
        {
            _inventoryRefreshRequested = false;
            _maintenance = MaintainFilters(store, replaceFilters).GetEnumerator();
        }

        var started = System.Diagnostics.Stopwatch.GetTimestamp();
        try
        {
            do
            {
                if (!_maintenance.MoveNext())
                {
                    _maintenance.Dispose();
                    _maintenance = null;
                    _nextMaintenanceAt = Time.unscaledTime + 2f;
                    break;
                }
            }
            while ((System.Diagnostics.Stopwatch.GetTimestamp() - started) * 1000d /
                   System.Diagnostics.Stopwatch.Frequency < MaintenanceBudgetMilliseconds);
        }
        catch (Exception ex)
        {
            ResetMaintenance();
            _nextMaintenanceAt = Time.unscaledTime + 2f;
            _scannerCacheExpiresAt = _nextMaintenanceAt;
            Core.Log?.Warning($"Water maintenance failed: {ex.Message}");
        }
    }

    internal static void ResetMaintenance()
    {
        _maintenance?.Dispose();
        _maintenance = null;
        _retryFilterRecovery = null;
        _storePointer = IntPtr.Zero;
        _scannerCache = false;
        _scannerCacheExpiresAt = -1f;
        _inventoryRefreshRequested = false;
        _nextMaintenanceAt = 0f;
        TooltipSections.Clear();
    }

    private static IEnumerable<object?> MaintainFilters(PlayerStore store, bool replaceFilters)
    {
        var allItems = new List<GameItem>();
        var depleted = new List<(GameItem Container, GameItem Filter)>();
        var hasScanner = false;
        foreach (var item in EnumerateOwnedItems(store))
        {
            if (item != null)
            {
                allItems.Add(item);
                if (!hasScanner && IsWaterScanner(item))
                    hasScanner = true;

                if (replaceFilters && IsWaterContainer(item) &&
                    TryGetDepletedFilter(item, out var filter))
                    depleted.Add((item, filter!));
            }
            yield return null;
        }

        _scannerCache = hasScanner;
        _scannerCacheExpiresAt = Time.unscaledTime + 1f;
        _inventoryRefreshRequested = false;

        if (!replaceFilters || depleted.Count == 0 ||
            Core.AutoReplaceFilters is not { Value: true })
            yield break;

        // No spare-filter work is performed until a depleted filter is found.
        // Three stable buckets avoid repeated native name reads in a comparator.
        var buckets = new[] { new List<GameItem>(), new List<GameItem>(), new List<GameItem>() };
        foreach (var item in allItems)
        {
            if (IsAvailableFilter(item))
                buckets[GetFilterPriority(item)].Add(item);
            yield return null;
        }

        foreach (var entry in depleted)
        {
            foreach (var bucket in buckets)
            {
                for (var candidateIndex = 0; candidateIndex < bucket.Count; candidateIndex++)
                {
                    var replacement = bucket[candidateIndex];
                    yield return null;
                    if (Core.AutoReplaceFilters is not { Value: true })
                        yield break;

                    // Inventory membership and durability may change between frames.
                    if (!TryGetDepletedFilter(entry.Container, out var current) ||
                        current!.Pointer != entry.Filter.Pointer)
                        goto NextContainer;

                    if (!IsAvailableFilter(replacement) ||
                        !CanSwapFilterPlacement(entry.Filter, replacement) ||
                        !AreItemsStillOwned(entry.Container, replacement))
                        continue;

                    if (ReplaceFilter(entry.Container, entry.Filter, replacement, out var toast))
                    {
                        bucket.RemoveAt(candidateIndex);
                        ShowToast(toast);
                    }
                    else
                        Core.Log?.Warning(
                            $"Filter swap for {SafeIdentifier(entry.Filter)} was skipped or failed.");

                    if (_retryFilterRecovery != null)
                        yield break;

                    // A native swap can partially mutate inventories. Re-scan on
                    // the next pass instead of trying another item in this state.
                    goto NextContainer;
                }
            }
        NextContainer:;
        }
    }

    private static bool TryGetDepletedFilter(GameItem container, out GameItem? filter)
    {
        filter = null;
        try
        {
            filter = WaterHelper.GetFilterItem(container);
            return filter != null && IsFilterItem(filter) &&
                   DurabilityHelper.GetRealDurability(filter) <= 0;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Cannot inspect filter in {SafeIdentifier(container)}: {ex.Message}");
            return false;
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

    private static IEnumerable<GameItem?> EnumerateOwnedItems(PlayerStore store)
    {
        var pending = new Queue<Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase>();
        var visited = new HashSet<IntPtr>();

        void Enqueue(Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase? node)
        {
            if (node != null && visited.Add(node.Pointer))
                pending.Enqueue(node);
        }

        var roots = store.FindAllItem(true);
        for (var i = 0; i < roots.Count; i++)
        {
            Enqueue(roots[i]);
            yield return null;
        }
        var repo = store.GetAllRepoEligibleItems();
        for (var i = 0; i < repo.Count; i++)
        {
            Enqueue(repo[i]);
            yield return null;
        }

        // Traverse each graph object once. FindAllChildItems recursively walks
        // this same graph and must not be repeated for every descendant.
        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            var item = node.TryCast<GameItem>();
            yield return item;

            if (item != null)
            {
                Enqueue(item.child);
                var children = item.children;
                for (var i = 0; i < children.Count; i++)
                {
                    Enqueue(children[i]);
                    yield return null;
                }
            }
            else
            {
                var storage = node.TryCast<GraphNodeStorage>();
                if (storage != null)
                {
                    var children = storage.children;
                    for (var i = 0; i < children.Count; i++)
                    {
                        Enqueue(children[i]);
                        yield return null;
                    }
                }
                var window = node.TryCast<GraphNodeWindow>();
                if (window != null)
                    Enqueue(window.child);
            }

            var inventory = node.TryCast<GameInventory>();
            if (inventory == null)
                continue;

            var childItems = inventory.childItems;
            for (var i = 0; i < childItems.Count; i++)
            {
                Enqueue(childItems[i]);
                yield return null;
            }
            var slot = inventory.TryCast<GameSlotInventory>();
            if (slot != null)
                Enqueue(slot.childItem);
            var grid = inventory.TryCast<GameGridInventory>();
            if (grid != null)
            {
                var items = grid.items;
                for (var i = 0; i < items.Count; i++)
                {
                    Enqueue(items[i]);
                    yield return null;
                }
            }
            var scrollable = inventory.TryCast<GameGridScrollableInventory>();
            if (scrollable != null)
            {
                var items = scrollable.items;
                for (var i = 0; i < items.Count; i++)
                {
                    Enqueue(items[i]);
                    yield return null;
                }
            }
        }
    }

    private static bool CanSwapFilterPlacement(GameItem depletedFilter, GameItem replacement)
    {
        try
        {
            var drag = ItemMouseDragHandler.current;
            if (drag != null && (drag.IsDraggingItem || drag.GetCurrentItem() != null))
                return false;
            var buffer = EmporiumEntry.Instance?.swapBufferElement;
            if (buffer == null || buffer.childItem != null)
                return false;

            if (!TryGetParentInventory(depletedFilter, out var target) ||
                !TryGetParentInventory(replacement, out var source) ||
                target == null || source == null || target.Pointer == source.Pointer)
                return false;

            if (!CanMoveBothWays(target) || !CanMoveBothWays(source))
                return false;

            return CanPlaceAtRecordedPosition(depletedFilter, replacement, source) &&
                   CanPlaceAtRecordedPosition(replacement, depletedFilter, target);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Cannot validate filter placement: {ex.Message}");
            return false;
        }
    }

    private static bool CanMoveBothWays(GameInventory inventory)
    {
        return !inventory.IsInsertLocked() && !inventory.IsRemoveLocked();
    }

    private static bool CanPlaceAtRecordedPosition(
        GameItem itemToPlace, GameItem slotItem, GameInventory inventory)
    {
        var itemShape = itemToPlace.modifiedShape;
        var slotShape = slotItem.modifiedShape;
        if (itemShape == null || slotShape == null)
            return false;

        // modifiedShape already carries global position and rotation. Follow
        // GraphUtils' placement transform; never add modifiedXOrigin again.
        var placedShape = new GridShapeBuilder(itemShape).SetTransform(slotShape).Build();
        var slot = inventory.TryCast<GameSlotInventory>();
        if (slot != null)
        {
            var occupant = slot.childItem;
            if (occupant != null && occupant.Pointer != slotItem.Pointer)
                return false;
            if (!inventory.MayHaveValidInventorySlot(itemToPlace))
                return false;

            var marker = new SlotMarker
            {
                item = itemToPlace,
                inventory = inventory,
                itemGridShape = placedShape,
                numTransfer = itemToPlace.unitCount
            };
            return itemToPlace.CheckIsStateValid(
                       inventory, marker, placedShape, itemToPlace.modifiedState) &&
                   inventory.CheckAcceptParentShapeStateModifier(
                       itemToPlace, marker, placedShape, itemToPlace.modifiedState);
        }

        var ignored = new Il2CppSystem.Collections.Generic.List<GameItem>();
        ignored.Add(itemToPlace);
        ignored.Add(slotItem);
        return inventory.CheckShapePlacement(itemToPlace, placedShape, ignored);
    }

    private static bool IsAvailableFilter(GameItem item)
    {
        try
        {
            if (!IsFilterItem(item) ||
                !TryGetParentInventory(item, out var parent) || parent == null ||
                IsInstalledInWaterContainer(item))
                return false;

            return DurabilityHelper.GetRealDurability(item) > 0;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Cannot inspect replacement {SafeIdentifier(item)}: {ex.Message}");
            return false;
        }
    }

    private static bool ReplaceFilter(
        GameItem container, GameItem depletedFilter, GameItem replacement, out string toastMessage)
    {
        toastMessage = string.Empty;
        if (!CanSwapFilterPlacement(depletedFilter, replacement) ||
            !AreItemsStillOwned(container, replacement) ||
            !TryGetDepletedFilter(container, out var installed) ||
            installed!.Pointer != depletedFilter.Pointer ||
            !IsAvailableFilter(replacement) ||
            !TryGetParentInventory(depletedFilter, out var targetInventory) ||
            !TryGetParentInventory(replacement, out var sourceInventory) ||
            targetInventory == null || sourceInventory == null)
            return false;

        // Capture actual native shapes before any mutation, including recovery.
        var targetShape = depletedFilter.modifiedShape.Clone();
        var sourceShape = replacement.modifiedShape.Clone();
        var depletedName = GetDisplayName(depletedFilter);
        var replacementName = GetDisplayName(replacement);

        try
        {
            GraphUtils.SwapItemsInPlace(depletedFilter, replacement);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"SwapItemsInPlace threw: {ex.Message}");
        }

        Validate(targetInventory);
        Validate(sourceInventory);
        Validate(container);
        if (IsAtPlacement(depletedFilter, sourceInventory, sourceShape) &&
            IsAtPlacement(replacement, targetInventory, targetShape) &&
            IsInstalledFilter(container, replacement))
        {
            BuildFilterToast(depletedName, replacementName, out toastMessage);
            return true;
        }

        if (!RecoverFilterSwap(container, depletedFilter, replacement,
                targetInventory, sourceInventory, targetShape, sourceShape))
        {
            // Hold references to unplaced items and retry recovery before any
            // further maintenance; otherwise a native failure could lose them.
            _retryFilterRecovery = () => RecoverFilterSwap(container, depletedFilter, replacement,
                targetInventory, sourceInventory, targetShape, sourceShape);
            Core.Log?.Warning(
                $"Filter recovery incomplete: {SafeIdentifier(depletedFilter)} / " +
                $"{SafeIdentifier(replacement)}; recovery will retry before further swaps.");
        }
        return false;
    }

    private static bool AreItemsStillOwned(GameItem container, GameItem replacement)
    {
        if (!PlayerStore.IsInstanceExist() || PlayerStore.Instance == null)
            return false;

        var roots = new HashSet<IntPtr>();
        foreach (var item in PlayerStore.Instance.FindAllItem(true))
            roots.Add(item.Pointer);
        foreach (var item in PlayerStore.Instance.GetAllRepoEligibleItems())
            roots.Add(item.Pointer);

        return IsUnderOwnedRoot(container, roots) && IsUnderOwnedRoot(replacement, roots);
    }

    private static bool IsUnderOwnedRoot(GameItem item, HashSet<IntPtr> roots)
    {
        var visited = new HashSet<IntPtr>();
        while (visited.Add(item.Pointer))
        {
            if (roots.Contains(item.Pointer))
                return true;
            if (!TryGetParentInventory(item, out var parent) || parent == null)
                return false;
            var parentItem = parent.GetParentItem();
            if (parentItem == null)
                return false;
            item = parentItem;
        }
        return false;
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

    private static bool RecoverFilterSwap(
        GameItem container, GameItem depletedFilter, GameItem replacement,
        GameInventory targetInventory, GameInventory sourceInventory,
        GridShape targetShape, GridShape sourceShape)
    {
        try
        {
            if (!TryGetParentInventory(depletedFilter, out var oldParent) ||
                !TryGetParentInventory(replacement, out var newParent))
                return false;

            // Never move an item adopted by an unrelated inventory or treat a
            // failed parent read as evidence that an item is detached.
            if (!IsRecoveryParent(oldParent, targetInventory, sourceInventory) ||
                !IsRecoveryParent(newParent, targetInventory, sourceInventory))
                return false;

            var targetSlot = targetInventory.TryCast<GameSlotInventory>();
            var occupant = targetSlot?.childItem;
            if (occupant != null && occupant.Pointer != depletedFilter.Pointer &&
                occupant.Pointer != replacement.Pointer)
                return false;

            if (IsInInventory(replacement, targetInventory))
            {
                if (IsInInventory(depletedFilter, sourceInventory))
                    GraphUtils.SwapItemsInPlace(replacement, depletedFilter);
                else if (!RestoreAtShape(replacement, sourceInventory, sourceShape))
                    return false;
            }

            // Restore the exhausted filter FIRST to free its occupied position
            // in the spare inventory before returning the replacement.
            var oldRestored = RestoreAtShape(depletedFilter, targetInventory, targetShape);
            var newRestored = RestoreAtShape(replacement, sourceInventory, sourceShape);
            return oldRestored && newRestored && IsInstalledFilter(container, depletedFilter);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Filter recovery failed: {ex.Message}");
            return false;
        }
        finally
        {
            Validate(targetInventory);
            Validate(sourceInventory);
            Validate(container);
        }
    }

    private static bool IsRecoveryParent(
        GameInventory? parent, GameInventory target, GameInventory source)
    {
        var buffer = EmporiumEntry.Instance?.swapBufferElement;
        return parent == null || parent.Pointer == target.Pointer || parent.Pointer == source.Pointer ||
               (buffer != null && parent.Pointer == buffer.Pointer);
    }

    private static bool RestoreAtShape(GameItem item, GameInventory inventory, GridShape shape)
    {
        if (!TryGetParentInventory(item, out var parent))
            return false;
        if (IsAtPlacement(item, inventory, shape))
            return true;
        if (inventory.IsInsertLocked() || (parent != null && parent.IsRemoveLocked()) ||
            !inventory.CheckInventorySlotStillValid(item, shape, item.modifiedState))
            return false;

        return GraphUtils.PlaceAtShape(item, inventory, shape) &&
               IsAtPlacement(item, inventory, shape);
    }

    private static bool IsAtPlacement(GameItem item, GameInventory inventory, GridShape shape)
    {
        if (!IsInInventory(item, inventory))
            return false;
        var actual = item.modifiedShape;
        return actual != null &&
               actual.minX == shape.minX && actual.minY == shape.minY &&
               actual.orientation == shape.orientation && actual.flipped == shape.flipped;
    }

    private static bool IsInInventory(GameItem item, GameInventory inventory)
    {
        return TryGetParentInventory(item, out var parent) &&
               parent != null && parent.Pointer == inventory.Pointer;
    }

    private static bool TryGetParentInventory(GameItem item, out GameInventory? inventory)
    {
        try
        {
            inventory = item.parentInventory;
            return true;
        }
        catch (Exception ex)
        {
            inventory = null;
            Core.Log?.Warning($"Cannot read inventory of {SafeIdentifier(item)}: {ex.Message}");
            return false;
        }
    }

    private static void Validate(GameInventory inventory)
    {
        try { inventory.Validate(); }
        catch (Exception ex) { Core.Log?.Warning($"Inventory validation failed: {ex.Message}"); }
    }

    private static void Validate(GameItem item)
    {
        try { item.Validate(); }
        catch (Exception ex) { Core.Log?.Warning($"Item validation failed: {ex.Message}"); }
    }

    private static bool IsInstalledInWaterContainer(GameItem item)
    {
        var visited = new HashSet<IntPtr>();
        if (!TryGetParentInventory(item, out var inventory))
            return true;

        while (inventory != null)
        {
            if (!visited.Add(inventory.Pointer))
                return true;

            var parentItem = inventory.GetParentItem();
            if (parentItem == null)
                return false;
            if (IsWaterContainer(parentItem))
                return true;
            if (!TryGetParentInventory(parentItem, out inventory))
                return true;
        }
        return false;
    }

    private static bool IsInstalledFilter(GameItem container, GameItem expectedFilter)
    {
        try
        {
            var installedFilter = WaterHelper.GetFilterItem(container);
            return installedFilter != null &&
                   installedFilter.Pointer == expectedFilter.Pointer;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsWaterContainer(GameItem item)
    {
        try
        {
            return KnownWaterContainerIdentifiers.Contains(item.identifier ?? string.Empty) ||
                   HasLiquidContainerMetadata(item) ||
                   WaterHelper.CanUseWaterContainer(item);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Cannot inspect water container {SafeIdentifier(item)}: {ex.Message}");
            return false;
        }
    }

    private static bool HasLiquidContainerMetadata(GameItem item)
    {
        var state = item.state;
        return state?.IsTag("LIQUID_CONTAINER_TAG") == true &&
               state.GetTag("LIQUID_CONTAINER_CAPACITY")?.valueInt > 0;
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
            if (identifier == "water_filter_adv")
                return 0;
            if (identifier == "water_filter")
                return 1;
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
