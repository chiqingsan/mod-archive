using System.Collections;
using System.Reflection;
using HarmonyLib;
using UnityEngine;


namespace AquaSense;

public sealed class Core : IMod
{
    public void Init(ModManifest manifest)
    {
        Log = new ModLog(manifest);

        try
        {
            var category = ModConfig.For(manifest.ID);
            ShowCompositionWithoutScanner = category.Define("ShowCompositionWithoutScanner", false, "AquaSense", () => "无需水质扫描仪查看液体成分");
            AutoReplaceFilters = category.Define("AutoReplaceFilters", true, "AquaSense", () => "滤芯耗尽后自动更换");
            ShowFilterDurability = category.Define("ShowFilterDurability", true, "AquaSense", () => "悬浮面板显示滤芯耐久");
            AutoUpdateWaterLabels = category.Define("AutoUpdateWaterLabels", true, "AquaSense", () => "倒水后自动更新水质标签");
        }
        catch (Exception ex)
        {
            Log.Warning($"Preference initialization failed: {ex.Message}");
        }
    }

    private GameObject? _runner;
    private HarmonyLib.Harmony? _harmony;
    private bool _patchAttempted;

    public void OnEnable()
    {
        if (_runner != null) return;
        _patchAttempted = false;
        _runner = new GameObject("AquaSense");
        UnityEngine.Object.DontDestroyOnLoad(_runner);
        _runner.AddComponent<AquaSenseRunner>().Mod = this;
    }

    private void InstallPatches()
    {
        try
        {
            var harmony = _harmony = new HarmonyLib.Harmony("chiqingsan.water-scan-tooltip");
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
            Patch(harmony, typeof(WaterHelper), nameof(WaterHelper.HandleFilter),
                new[] { typeof(string), typeof(int), typeof(GameItem), typeof(GameItem) }, nameof(Hooks.AfterFilter));
            Patch(harmony, typeof(GameItem), nameof(GameItem.AddParent),
                new[] { typeof(GraphNodeStorage) }, nameof(Hooks.AfterItemParentChanged));
            Patch(harmony, typeof(GameItem), nameof(GameItem.RemoveParent),
                new[] { typeof(GraphNodeStorage) }, nameof(Hooks.AfterItemParentChanged));
        }
        catch (Exception ex)
        {
            _harmony?.UnpatchAll("chiqingsan.water-scan-tooltip");
            _harmony = null;
            Log?.Warning($"Patch initialization failed; AquaSense remains inactive: {ex}");
        }
    }

    internal void OnUpdate()
    {
        // Wait until a playable store exists. OnEnable runs inside ModLoader.Awake,
        // before assets/localization are ready; patching can run target static constructors.
        if (!_patchAttempted)
        {
            if (!PlayerStore.IsInstanceExist() || PlayerStore.Instance == null || EmporiumEntry.Instance == null)
                return;
            _patchAttempted = true;
            InstallPatches();
        }
        if (_harmony != null)
            Hooks.UpdateMaintenance(AutoReplaceFilters is { Value: true });
    }

    public void OnDisable()
    {
        _harmony?.UnpatchAll("chiqingsan.water-scan-tooltip");
        _harmony = null;
        if (_runner != null) UnityEngine.Object.Destroy(_runner);
        _runner = null;
        Hooks.ResetMaintenance();
    }

    internal static ConfigEntry<bool>? ShowCompositionWithoutScanner { get; private set; }

    internal static ConfigEntry<bool>? AutoReplaceFilters { get; private set; }

    internal static ConfigEntry<bool>? ShowFilterDurability { get; private set; }

    internal static ConfigEntry<bool>? AutoUpdateWaterLabels { get; private set; }

    internal static ModLog? Log { get; private set; }

    private static void Patch(
        HarmonyLib.Harmony harmony,
        Type type,
        string methodName,
        Type[] argumentTypes,
        string postfixName)
    {
        var original = AccessTools.Method(type, methodName, argumentTypes)
            ?? throw new MissingMethodException(type.FullName, methodName);
        var postfix = AccessTools.Method(typeof(Hooks), postfixName)
            ?? throw new MissingMethodException(typeof(Hooks).FullName, postfixName);
        harmony.Patch(original, postfix: new HarmonyMethod(postfix));
    }

}

internal static class Hooks
{
    private const double MaintenanceBudgetMilliseconds = 2d;
    private const int MaintenanceStepsPerFrame = 64;
    private const float FullScanIntervalSeconds = 30f;
    private const float EmptyCacheRetrySeconds = 5f;
    private static readonly FieldInfo GridItems = typeof(GameGridInventory).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic)!;
    private static readonly FieldInfo ScrollItems = typeof(GameGridScrollableInventory).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic)!;
    private static List<(GameItem Item, GameInventory Inventory)>[] FilterCache = { new(), new(), new() };
    private static readonly Queue<GameItem> PendingFilters = new();
    private static readonly HashSet<GameItem> PendingFilterSet = new();
    private static float _nextFullScanAt;
    private static float _nextCacheRetryAt;
    private static float _scannerCacheExpiresAt = -1f;
    private static bool _scannerCache;
    private static GameItem? _scannerItem;
    private static float _nextScannerValidationAt;
    private static bool _inventoryRefreshRequested;
    private static float _nextMaintenanceAt;
    private static PlayerStore? _storePointer;
    private static IEnumerator<object?>? _maintenance;
    private static Func<bool>? _retryFilterRecovery;
    private static int _lastTooltipFrame = -1;
    private static readonly Dictionary<(RichTextBuilder Builder, GameItem Item), (int Index, object Slice)> TooltipSections = new();
    private static int _lastLabelFrame = -1;
    private static GameItem? _lastLabelItem;
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

    public static void AfterFilter(GameItem __3)
    {
        if (Core.AutoReplaceFilters is not { Value: true } || __3 == null ||
            !__3.IsTag("WATER_FILTER") || !IsFilterItem(__3) ||
            DurabilityHelper.GetRealDurability(__3) > 0)
            return;
        QueueFilter(__3);
    }

    public static void AfterItemParentChanged(GameItem __instance, bool __result)
    {
        if (__result && string.Equals(__instance.identifier, "aquascan", StringComparison.OrdinalIgnoreCase))
        {
            _scannerCache = false;
            _scannerItem = null;
            _inventoryRefreshRequested = true;
        }
    }

    private static void QueueFilter(GameItem filter)
    {
        if (PendingFilterSet.Add(filter)) PendingFilters.Enqueue(filter);
    }

    private static void AppendScanTooltip(RichTextBuilder builder, GameItem item)
    {
        if (builder == null || item == null)
            return;

        try
        {
            if (!IsWaterContainer(item))
                return;

            var frame = Time.frameCount;
            if (_lastTooltipFrame != frame)
            {
                TooltipSections.Clear();
                _lastTooltipFrame = frame;
            }

            var key = (builder, item);
            var slices = (IList)typeof(RichTextBuilder).GetField("slices", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(builder)!;
            if (TooltipSections.TryGetValue(key, out var section) &&
                slices != null && section.Index < slices.Count &&
                slices[section.Index] == section.Slice)
                return;

            var firstSlice = slices?.Count ?? 0;

            // The water composition section needs actual liquid to describe; the
            // filter durability line does not, so an empty jug still shows it.
            if ((Core.ShowCompositionWithoutScanner is { Value: true } || HasWaterScanner()) && IsFilledWaterContainer(item))
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
            slices = (IList)typeof(RichTextBuilder).GetField("slices", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(builder)!;
            if (slices != null && firstSlice < slices.Count)
                TooltipSections[key] = (firstSlice, slices[firstSlice]);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Water tooltip failed for {item.identifier}: {ex}");
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
            var itemPointer = container;
            if (frame == _lastLabelFrame && itemPointer == _lastLabelItem)
                return;

            _lastLabelFrame = frame;
            _lastLabelItem = itemPointer;

            WaterHelper.UpdateContainerGoodType(container);
            WaterFeatureHelper.UpdateWaterFeature(container, true);
            WaterFeatureHelper.OnWaterLabelUpdated(container);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Water label update failed for {container.identifier}: {ex}");
        }
    }

    private static bool HasWaterScanner()
    {
        if (_scannerItem != null && Time.unscaledTime >= _nextScannerValidationAt && EmporiumEntry.Instance != null)
        {
            _nextScannerValidationAt = Time.unscaledTime + 1f;
            if (!GeneralHelper.IsItemOwned(_scannerItem) || !IsUnderOwnedRoot(_scannerItem, new HashSet<GraphNodeStorage>(InventoryRoots())))
            {
                _scannerItem = null;
                _scannerCache = false;
                _inventoryRefreshRequested = true;
            }
        }
        if (_maintenance == null && Time.unscaledTime >= _scannerCacheExpiresAt)
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
        if (!PlayerStore.IsInstanceExist() || PlayerStore.Instance == null || EmporiumEntry.Instance == null)
        {
            if (_storePointer != null) ResetMaintenance();
            return;
        }
        var store = PlayerStore.Instance;
        if (_storePointer != store)
        {
            ResetMaintenance();
            _storePointer = store;
            _inventoryRefreshRequested = true;
        }

        if (_retryFilterRecovery != null)
        {
            if (Time.unscaledTime >= _nextMaintenanceAt)
            {
                if (_retryFilterRecovery()) _retryFilterRecovery = null;
                _nextMaintenanceAt = Time.unscaledTime + 2f;
            }
            return;
        }

        if (_maintenance == null)
        {
            if (_inventoryRefreshRequested || Time.unscaledTime >= _nextFullScanAt)
            {
                _inventoryRefreshRequested = false;
                _maintenance = ScanInventory().GetEnumerator();
            }
            else if (replaceFilters && PendingFilters.Count > 0 && Time.unscaledTime >= _nextMaintenanceAt)
                _maintenance = ProcessPendingFilters().GetEnumerator();
            else return;
        }

        var started = System.Diagnostics.Stopwatch.GetTimestamp();
        try
        {
            var steps = 0;
            do
            {
                if (!_maintenance.MoveNext())
                {
                    _maintenance.Dispose();
                    _maintenance = null;
                    break;
                }
                steps++;
            }
            while (steps < MaintenanceStepsPerFrame &&
                   (System.Diagnostics.Stopwatch.GetTimestamp() - started) * 1000d /
                   System.Diagnostics.Stopwatch.Frequency < MaintenanceBudgetMilliseconds);
        }
        catch (Exception ex)
        {
            ResetMaintenance();
            _nextFullScanAt = Time.unscaledTime + EmptyCacheRetrySeconds;
            Core.Log?.Warning($"Water maintenance failed: {ex.Message}");
        }
    }

    internal static void ResetMaintenance()
    {
        _maintenance?.Dispose();
        _maintenance = null;
        _retryFilterRecovery = null;
        _storePointer = null;
        _scannerCache = false;
        _scannerItem = null;
        _nextScannerValidationAt = 0f;
        _scannerCacheExpiresAt = -1f;
        _inventoryRefreshRequested = false;
        _nextMaintenanceAt = 0f;
        _nextFullScanAt = 0f;
        _nextCacheRetryAt = 0f;
        PendingFilters.Clear();
        PendingFilterSet.Clear();
        foreach (var bucket in FilterCache) bucket.Clear();
        TooltipSections.Clear();
    }

    private static IEnumerable<object?> ScanInventory()
    {
        var buckets = new[] { new List<(GameItem, GameInventory)>(), new List<(GameItem, GameInventory)>(), new List<(GameItem, GameInventory)>() };
        GameItem? scanner = null;
        foreach (var item in EnumerateOwnedItems())
        {
            if (item != null && GeneralHelper.IsItemOwned(item))
            {
                if (scanner == null && IsWaterScanner(item)) scanner = item;
                if (Core.AutoReplaceFilters is { Value: true })
                {
                    if (IsWaterContainer(item) && TryGetDepletedFilter(item, out var filter)) QueueFilter(filter!);
                    if (IsAvailableFilter(item) && item.parentInventory != null)
                        buckets[GetFilterPriority(item)].Add((item, item.parentInventory));
                }
            }
            yield return null;
        }
        FilterCache = buckets;
        _scannerItem = scanner;
        _scannerCache = scanner != null;
        _nextScannerValidationAt = Time.unscaledTime + 1f;
        _scannerCacheExpiresAt = Time.unscaledTime + FullScanIntervalSeconds;
        _nextFullScanAt = _scannerCacheExpiresAt;
        _nextCacheRetryAt = Time.unscaledTime + EmptyCacheRetrySeconds;
        if (PendingFilters.Count > 0) _nextMaintenanceAt = Time.unscaledTime;
    }

    private static IEnumerable<object?> ProcessPendingFilters()
    {
        var count = PendingFilters.Count;
        for (var n = 0; n < count; n++)
        {
            var filter = PendingFilters.Dequeue();
            PendingFilterSet.Remove(filter);
            var container = filter.parentInventory?.GetParentItem();
            if (container == null || !IsWaterContainer(container) ||
                !TryGetDepletedFilter(container, out var installed) || installed != filter)
            {
                yield return null;
                continue;
            }
            var swapped = false;
            var attempted = false;
            foreach (var bucket in FilterCache)
            {
                for (var i = 0; i < bucket.Count;)
                {
                    // Select from the existing cache only; keep filter-type priority.
                    var bestIndex = -1;
                    var bestDurability = 0;
                    for (var j = i; j < bucket.Count;)
                    {
                        var cached = bucket[j];
                        yield return null;
                        if (Core.AutoReplaceFilters is not { Value: true }) yield break;
                        if (cached.Item.parentInventory != cached.Inventory || !IsAvailableFilter(cached.Item))
                        {
                            bucket.RemoveAt(j);
                            continue;
                        }
                        var durability = DurabilityHelper.GetRealDurability(cached.Item);
                        if (durability > bestDurability && AreItemsStillOwned(container, cached.Item) &&
                            CanSwapFilterPlacement(filter, cached.Item))
                        {
                            bestIndex = j;
                            bestDurability = durability;
                        }
                        j++;
                    }
                    if (bestIndex < 0) break;
                    (bucket[i], bucket[bestIndex]) = (bucket[bestIndex], bucket[i]);
                    var candidate = bucket[i];
                    if (ReplaceFilter(container, filter, candidate.Item, out var toast, out attempted))
                    {
                        bucket.RemoveAt(i);
                        ShowToast(toast);
                        swapped = true;
                        break;
                    }
                    if (attempted || _retryFilterRecovery != null) break;
                    i++;
                }
                if (swapped || attempted || _retryFilterRecovery != null) break;
            }
            if (!swapped)
            {
                QueueFilter(filter);
                _nextMaintenanceAt = Time.unscaledTime + EmptyCacheRetrySeconds;
                if (!attempted && Time.unscaledTime >= _nextCacheRetryAt)
                {
                    _inventoryRefreshRequested = true;
                    _nextCacheRetryAt = Time.unscaledTime + EmptyCacheRetrySeconds;
                }
            }
            if (_retryFilterRecovery != null) yield break;
            yield return null;
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

    private static GraphNodeStorage[] InventoryRoots()
    {
        var e = EmporiumEntry.Instance;
        return new GraphNodeStorage[] { e.frontInvWindow, e.backInvWindow, e.backInvinvElementCounter,
            e.invWindow, e.showcaseWindow, e.trashInvWindow, e.docInvWindow, e.hiddenWindow,
            e.afterhourWindow, e.vendingFountainWindow, e.vendingMachineWindow };
    }

    private static IEnumerable<GameItem?> EnumerateOwnedItems()
    {
        var pending = new Queue<GraphNodeStorage>();
        var visited = new HashSet<GraphNodeStorage>();
        foreach (var root in InventoryRoots())
            if (root != null && visited.Add(root)) pending.Enqueue(root);
        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            yield return node as GameItem;
            if (node is GameGridInventory grid)
            {
                var items = (List<GameItem>)GridItems.GetValue(grid)!;
                for (var i = 0; i < items.Count; i++)
                {
                    if (visited.Add(items[i])) pending.Enqueue(items[i]);
                    yield return null;
                }
            }
            else if (node is GameGridScrollableInventory scroll)
            {
                var items = (List<GameItem>)ScrollItems.GetValue(scroll)!;
                for (var i = 0; i < items.Count; i++)
                {
                    if (visited.Add(items[i])) pending.Enqueue(items[i]);
                    yield return null;
                }
            }
            else
            {
                var children = node.children;
                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    if (child != null && visited.Add(child)) pending.Enqueue(child);
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
                target == null || source == null || target == source)
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
        var slot = (inventory as GameSlotInventory);
        if (slot != null)
        {
            var occupant = slot.childItem;
            if (occupant != null && occupant != slotItem)
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

        var ignored = new List<GameItem>();
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
        GameItem container, GameItem depletedFilter, GameItem replacement, out string toastMessage, out bool attempted)
    {
        toastMessage = string.Empty;
        attempted = false;
        if (!CanSwapFilterPlacement(depletedFilter, replacement) ||
            !AreItemsStillOwned(container, replacement) ||
            !TryGetDepletedFilter(container, out var installed) ||
            installed! != depletedFilter ||
            !TryGetParentInventory(depletedFilter, out var targetInventory) ||
            !TryGetParentInventory(replacement, out var sourceInventory) ||
            targetInventory == null || sourceInventory == null)
            return false;

        // Capture actual native shapes before any mutation, including recovery.
        var targetShape = depletedFilter.modifiedShape.Clone();
        var sourceShape = replacement.modifiedShape.Clone();
        var depletedName = GetDisplayName(depletedFilter);
        var replacementName = GetDisplayName(replacement);

        attempted = true;
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
        if (!PlayerStore.IsInstanceExist() || PlayerStore.Instance == null || EmporiumEntry.Instance == null ||
            !GeneralHelper.IsItemOwned(container) || !GeneralHelper.IsItemOwned(replacement)) return false;
        var roots = new HashSet<GraphNodeStorage>(InventoryRoots());
        return IsUnderOwnedRoot(container, roots) && IsUnderOwnedRoot(replacement, roots);
    }

    private static bool IsUnderOwnedRoot(GameItem item, HashSet<GraphNodeStorage> roots)
    {
        var pending = new Queue<GraphNodeStorage>();
        var visited = new HashSet<GraphNodeStorage>();
        pending.Enqueue(item);
        while (pending.Count > 0)
        {
            var node = pending.Dequeue();
            if (!visited.Add(node)) continue;
            if (roots.Contains(node)) return true;
            foreach (var parent in node.parents)
                if (parent != null) pending.Enqueue(parent);
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

            var targetSlot = (targetInventory as GameSlotInventory);
            var occupant = targetSlot?.childItem;
            if (occupant != null && occupant != depletedFilter &&
                occupant != replacement)
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
        return parent == null || parent == target || parent == source ||
               (buffer != null && parent == buffer);
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

        return (bool)typeof(GraphUtils).GetMethod("PlaceAtShape", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, new object[] { item, inventory, shape })! &&
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
               parent != null && parent == inventory;
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
        var visited = new HashSet<object>();
        if (!TryGetParentInventory(item, out var inventory))
            return true;

        while (inventory != null)
        {
            if (!visited.Add(inventory))
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
                   installedFilter == expectedFilter;
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
                   HasLiquidContainerMetadata(item);
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

public sealed class AquaSenseRunner : MonoBehaviour
{
    internal Core Mod = null!;
    private void Update() => Mod.OnUpdate();
}
