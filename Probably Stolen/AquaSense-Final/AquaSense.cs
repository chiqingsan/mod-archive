using System.Collections;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;


namespace AquaSense;

public sealed class Core : IMod
{
    public void Init(ModManifest manifest)
    {
        Log = new ModLog(manifest);
        ModId = manifest.ID;

        try
        {
            var category = ModConfig.For(manifest.ID);
            ShowWaterComposition = category.Define("ShowWaterComposition", true, "AquaSense", () => "显示水质详情");
            ShowCompositionWithoutScanner = category.Define("ShowCompositionWithoutScanner", false, "AquaSense", () => "无需水质扫描仪查看液体成分");
            ShowCompositionBars = category.Define("ShowCompositionBars", true, "AquaSense", () => "用彩色进度条显示成分占比");
            HideZeroComponents = category.Define("HideZeroComponents", true, "AquaSense", () => "隐藏四舍五入后为 0.00% 的成分");
            AutoReplaceFilters = category.Define("AutoReplaceFilters", true, "AquaSense", () => "滤芯耗尽后自动更换");
            AutoReplaceCloth = category.Define("AutoReplaceCloth", true, "AquaSense", () => "韧棉织物耗尽后自动更换");
            ShowFilterDurability = category.Define("ShowFilterDurability", true, "AquaSense", () => "悬浮面板显示滤芯耐久");
            AutoUpdateWaterLabels = category.Define("AutoUpdateWaterLabels", true, "AquaSense", () => "倒水后自动更新水质标签");
            HighlightWaterQuality = category.Define("HighlightWaterQuality", true, "AquaSense", () => "给水质标签添加文字颜色");
            SortCompositionByAmount = category.Define("SortCompositionByAmount", true, "AquaSense", () => "成分按占比从高到低排序");
            MinFilterDurabilityEnabled = category.Define("MinFilterDurabilityEnabled", false, "AquaSense", () => "耐久度以下的过滤器不参与更换");
            MinFilterDurability = category.Define("MinFilterDurability", 200, "AquaSense", () => "最低耐久度", 0, 600, () => "只有耐久度不低于此值的过滤器才会被自动换上");
        }
        catch (Exception ex)
        {
            Log.Warning($"Preference initialization failed: {ex.Message}");
        }
    }

    private GameObject? _runner;
    private HarmonyLib.Harmony? _harmony;
    private bool _patchAttempted;
    private bool _configPanelChecked;

    public void OnEnable()
    {
        if (_runner != null) return;
        _patchAttempted = false;
        _configPanelChecked = false;
        ConfigPanel.Reset();
        _runner = new GameObject("AquaSense");
        UnityEngine.Object.DontDestroyOnLoad(_runner);
        _runner.AddComponent<AquaSenseRunner>().Mod = this;
        CompositionApi.Register();
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
            Patch(harmony, typeof(TextMeshProUGUI), "GenerateTextMesh",
                Type.EmptyTypes, nameof(Hooks.AfterTooltipTextMesh));
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
        // The settings panel (and its slider values) is reachable from the main menu,
        // long before a save world exists. Patch its renderer as soon as the menu UI is
        // alive; waiting for the store would leave the durability slider without its
        // value text when the panel is opened from the main menu.
        if (!_configPanelChecked && ConfigPanel.Install())
            _configPanelChecked = true;

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
        CompositionApi.Unregister();
        ConfigPanel.Uninstall();
        _harmony?.UnpatchAll("chiqingsan.water-scan-tooltip");
        _harmony = null;
        if (_runner != null) UnityEngine.Object.Destroy(_runner);
        _runner = null;
        Hooks.ResetMaintenance();
        TooltipCompositionBars.HideAll();
    }

    internal static ConfigEntry<bool>? ShowWaterComposition { get; private set; }

    internal static ConfigEntry<bool>? ShowCompositionWithoutScanner { get; private set; }

    internal static ConfigEntry<bool>? ShowCompositionBars { get; private set; }

    internal static ConfigEntry<bool>? HideZeroComponents { get; private set; }

    internal static ConfigEntry<bool>? AutoReplaceFilters { get; private set; }

    internal static ConfigEntry<bool>? AutoReplaceCloth { get; private set; }

    internal static ConfigEntry<bool>? ShowFilterDurability { get; private set; }

    internal static ConfigEntry<bool>? AutoUpdateWaterLabels { get; private set; }

    internal static ConfigEntry<bool>? HighlightWaterQuality { get; private set; }

    internal static ConfigEntry<bool>? SortCompositionByAmount { get; private set; }

    internal static ConfigEntry<bool>? MinFilterDurabilityEnabled { get; private set; }

    internal static ConfigEntry<int>? MinFilterDurability { get; private set; }

    internal static ModLog? Log { get; private set; }

    internal static string ModId { get; private set; } = string.Empty;

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

/// <summary>
/// The vanilla mod-config window renders number settings as bare sliders with no
/// visible value. This patch adds a live numeric readout to AquaSense's own ranged
/// number settings (the minimum filter durability slider). It is installed as soon as
/// the main-menu UI is alive, because the settings panel can be opened from the main
/// menu before a save world exists; patching at save-world time would leave the
/// slider without its value text there. When "更好的性能优化" is loaded it already
/// replaces the same renderer for every mod, so this patch stays out of the way.
/// </summary>
internal static class ConfigPanel
{
    private const string HarmonyId = "chiqingsan.water-scan-settings";
    private const string BetterPerformanceId = "chiqingsan.BetterPerformance";
    private const float ValueWidth = 62f;
    private const float SliderWidth = 224f;

    private static HarmonyLib.Harmony? _harmony;
    private static bool _installed;
    private static bool _unavailable;

    /// <summary>Called when the mod is enabled so a later enable can patch again.</summary>
    internal static void Reset()
    {
        _installed = false;
        _unavailable = false;
        _harmony = null;
    }

    internal static void Uninstall()
    {
        try { _harmony?.UnpatchAll(HarmonyId); } catch { }
        _harmony = null;
        _installed = false;
    }

    /// <summary>
    /// Returns true once the settings renderer is patched (or can never be patched);
    /// false while the menu UI is not alive yet.
    /// </summary>
    internal static bool Install()
    {
        if (_installed || _unavailable) return true;
        if (ModUIManager.Instance == null && MainMenuUIController.Instance == null)
            return false;

        if (IsBetterPerformanceLoaded())
        {
            _unavailable = true;
            Core.Log?.Log("更好的性能优化已接管设置面板渲染，跳过滑条数值显示");
            return true;
        }

        var target = AccessTools.Method(typeof(ModConfigWindow), "BuildEntry",
            new[] { typeof(CustomUIBuilder), typeof(ModConfig), typeof(ConfigEntry) });
        if (target == null)
        {
            _unavailable = true;
            Core.Log?.Warning("未找到 ModConfigWindow.BuildEntry，跳过滑条数值显示");
            return true;
        }

        try
        {
            var info = HarmonyLib.Harmony.GetPatchInfo(target);
            if (info != null && info.Prefixes != null && info.Prefixes.Count > 0)
            {
                _unavailable = true;
                Core.Log?.Log("设置面板渲染已被其他模组接管，跳过滑条数值显示");
                return true;
            }

            _harmony = new HarmonyLib.Harmony(HarmonyId);
            var prefix = AccessTools.Method(typeof(ConfigPanel), nameof(BuildEntryPrefix))
                ?? throw new MissingMethodException(typeof(ConfigPanel).FullName, nameof(BuildEntryPrefix));
            _harmony.Patch(target, prefix: new HarmonyMethod(prefix));
            _installed = true;
            Core.Log?.Log("已接管设置面板渲染：滑条实时显示数值");
            return true;
        }
        catch (Exception ex)
        {
            try { _harmony?.UnpatchAll(HarmonyId); } catch { }
            _harmony = null;
            _unavailable = true;
            Core.Log?.Warning($"设置面板补丁安装失败: {ex.GetType().Name} {ex.Message}");
            return true;
        }
    }

    /// <summary>
    /// Prefix replacement for the vanilla row builder: only AquaSense's ranged number
    /// settings are rendered here so their slider shows the current value.
    /// The first parameter must stay named "b": Harmony binds patch arguments to the
    /// target by parameter name, and the game's BuildEntry declares (CustomUIBuilder b, ...).
    /// </summary>
    internal static bool BuildEntryPrefix(CustomUIBuilder b, ModConfig config, ConfigEntry entry)
    {
        if (b == null || config == null || entry == null) return true;
        if (config.ModId != Core.ModId) return true;

        var valueType = entry.ValueType;
        if ((valueType != typeof(int) && valueType != typeof(float)) || !entry.HasRange)
            return true;

        try
        {
            BuildRangedEntry(b, config, entry);
            return false;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"设置项渲染失败 {entry.Key}: {ex.GetType().Name} {ex.Message}");
            return true; // fall back to the vanilla renderer
        }
    }

    private static void BuildRangedEntry(CustomUIBuilder builder, ModConfig config, ConfigEntry entry)
    {
        var label = Safe(entry.Label, entry.Key);
        if (entry.Tooltip != null)
        {
            var link = "modcfg:" + config.ModId + ":" + entry.Key;
            TMPIconHover.CustomLinks[link] = entry.Tooltip;
            label = label + " <link=\"" + link + "\"><sprite name=\"info\" tint=1></link>";
        }

        builder.BeginRow(4f, TextAnchor.MiddleLeft);
        builder.AddLabel(label, "label_" + entry.Key);
        builder.AddFlexibleSpace();

        var value = Convert.ToSingle(entry.BoxedValue);
        builder.AddLabel(FormatNumber(entry.ValueType, value), "value_" + entry.Key);
        var valueLabel = builder.Last;
        valueLabel.SetFixedWidth(ValueWidth);
        builder.AddSlider(entry.Min, entry.Max, value,
            v => valueLabel.SetText(FormatNumber(entry.ValueType, v)),
            entry.ValueType == typeof(int), entry.Key);
        builder.Last.SetFixedWidth(SliderWidth);
        builder.End();
    }

    private static string FormatNumber(Type type, float value)
    {
        return type == typeof(int)
            ? ((int)Math.Round(value)).ToString()
            : value.ToString("0.##");
    }

    private static string Safe(Func<string>? getter, string fallback)
    {
        if (getter == null) return fallback;
        try { return getter(); } catch { return fallback; }
    }

    private static bool IsBetterPerformanceLoaded()
    {
        try
        {
            var mods = ModLoader.Mods;
            if (mods == null) return false;
            foreach (var mod in mods)
            {
                if (mod == null || !mod.IsEnabled || mod.Manifest == null) continue;
                if (mod.Manifest.ID == BetterPerformanceId) return true;
            }
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"无法检查已加载模组: {ex.Message}");
        }
        return false;
    }
}

internal static class Hooks
{
    private const double MaintenanceBudgetMilliseconds = 2d;
    private const int MaintenanceStepsPerFrame = 64;
    private const float FullScanIntervalSeconds = 30f;
    private const float EmptyCacheRetrySeconds = 5f;

    // FILTER_SIZE_TAG of the vanilla water filter (game data 0.46D). Only used
    // when the prototype's tag cannot be read for some reason.
    private const int VanillaFilterSizeFallback = 22;

    // Fallback for the preference value when the entry could not be created.
    private const int DefaultMinFilterDurability = 200;
    private static readonly FieldInfo GridItems = typeof(GameGridInventory).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic)!;
    private static readonly FieldInfo ScrollItems = typeof(GameGridScrollableInventory).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic)!;
    private static List<(GameItem Item, GameInventory Inventory)>[] FilterCache = { new(), new(), new() };
    private static List<(GameItem Item, GameInventory Inventory)>[] ClothCache = { new() };
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
    private static int _vanillaFilterSize;
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

    // Hosts whose built-in filter slots belong to the host's own water loop,
    // not to the player's spare stock. Currently the recirculating aquaculture
    // system (循环水养殖系统): AquacultureHelper.TickTank reads its four filter
    // slots every tank cycle, but those filters sit in machine slots instead of
    // inside a water container, so IsInstalledInWaterContainer cannot see them.
    // Treating them as spares would silently starve the fish tank (reported by
    // players whose drug crafting kept pulling the tank's filters).
    private static readonly string[] ExcludedFilterHostTags = { "RAS_TAG" };
    private static readonly string[] ExcludedFilterHostIdentifiers = { "ras", "rasempty" };

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
        if (__3 == null || !__3.IsTag("WATER_FILTER") ||
            DurabilityHelper.GetRealDurability(__3) > 0)
            return;
        if (IsInExcludedFilterHost(__3))
            return;
        if (IsFilterItem(__3))
        {
            if (Core.AutoReplaceFilters is { Value: true }) QueueFilter(__3);
            return;
        }
        if (IsClothFilterItem(__3) && Core.AutoReplaceCloth is { Value: true })
            QueueFilter(__3);
    }

    public static void AfterItemParentChanged(GameItem __instance, bool __result)
    {
        if (__result && IsWaterScanner(__instance))
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

            TryHighlightWaterLabelerLine(builder, item);

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
            if (Core.ShowWaterComposition is { Value: true } &&
                (Core.ShowCompositionWithoutScanner is { Value: true } || HasWaterScanner()) &&
                IsFilledWaterContainer(item))
            {
                var purity = WaterHelper.GetWaterPurity(item);
                var locale = LocHelper.GetCurrentLocaleCode();
                var titlePrefix = IsChineseLocale(locale)
                    ? $"水质详情 ({purity})"
                    : $"Water composition ({purity})";
                var waterLabel = GetWaterLabel(item);
                var qualityIndex = GetWaterQualityIndex(item);
                if (Core.HighlightWaterQuality is { Value: true } && !string.IsNullOrEmpty(waterLabel))
                {
                    // Two slices keep the prefix neutral and tint only the quality tag.
                    // Unknown water (purity below the gutterflow tier) shares the last
                    // colour instead of falling back to the game's default tint.
                    var colorIndex = qualityIndex >= 0 ? qualityIndex : WaterQualityForegroundColors.Length - 1;
                    builder.Add(titlePrefix + "  ");
                    builder.Add(
                        waterLabel,
                        WaterQualityForegroundColors[colorIndex],
                        "\n");
                }
                else
                {
                    var title = string.IsNullOrEmpty(waterLabel)
                        ? titlePrefix
                        : $"{titlePrefix}  {waterLabel}";

                    AddLine(builder, title);
                }

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

        // The slot can also hold a UV lamp; the durability line is for
        // consumable filters and coarse cloth alike (both are consumed through
        // durability in HandleFilter). Auto-replacement still uses
        // IsFilterItem, which rejects cloth too coarse to be worth swapping.
        if (!IsWaterFilterSlotItem(filter))
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

    // One colour per water quality tier (pure, high quality, base, ghost, rusty,
    // gutterflow) plus the unknown tier (purity below 3500), which shares the
    // gutterflow colour. ColorPalette is an int-backed enum whose value is the
    // literal 0xRRGGBB colour, so arbitrary tints cast straight into it.
    private static readonly RenderHandler.ColorPalette[] WaterQualityForegroundColors =
    {
        (RenderHandler.ColorPalette)0x8FF7F0,
        (RenderHandler.ColorPalette)0x9FE6A0,
        (RenderHandler.ColorPalette)0xA6C8F5,
        (RenderHandler.ColorPalette)0xD3A6F5,
        (RenderHandler.ColorPalette)0xF0B27A,
        (RenderHandler.ColorPalette)0xC73E3A
    };

    private static int GetWaterQualityIndex(GameItem item)
    {
        try
        {
            var index = WaterFeatureHelper.GetWaterIndex(item);
            return index >= 0 && index < WaterQualityForegroundColors.Length ? index : -1;
        }
        catch
        {
            return -1;
        }
    }

    // The game's labelled-feature line ("[标签] 优质水") is appended by
    // LabelerHelper inside GetTooltipBasic, before our postfix runs, so it is
    // found by slice pattern instead of a Harmony patch: the localized prefix
    // slice is directly followed by the displayed water name.
    internal static void TryHighlightWaterLabelerLine(RichTextBuilder builder, GameItem item)
    {
        if (builder == null || Core.HighlightWaterQuality is not { Value: true })
            return;

        var qualityIndex = GetWaterQualityIndex(item);
        var label = GetWaterLabel(item);
        if (string.IsNullOrEmpty(label))
            return;
        var colorIndex = qualityIndex >= 0 ? qualityIndex : WaterQualityForegroundColors.Length - 1;

        try
        {
            var slices = (IList)typeof(RichTextBuilder).GetField("slices", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(builder)!;
            if (slices == null)
                return;

            var prefix = (LocHelper.GetLocalizedMechanic("mech_label_tooltip_prefix") ?? string.Empty) + " ";
            for (var index = 0; index < slices.Count; index++)
            {
                var slice = slices[index];
                if (slice == null)
                    continue;

                var sliceType = slice.GetType();
                if ((string?)sliceType.GetField("text")?.GetValue(slice) != prefix)
                    continue;

                for (var next = index + 1; next < slices.Count; next++)
                {
                    var candidate = slices[next];
                    if (candidate == null)
                        continue;

                    var candidateType = candidate.GetType();
                    if (candidateType.GetField("isNewLine")?.GetValue(candidate) is true)
                        continue;

                    var display = (string?)candidateType.GetField("text")?.GetValue(candidate);
                    if (display == null)
                        continue;

                    if (display.TrimEnd('\n') != label)
                        break;

                    candidateType.GetField("color")?.SetValue(candidate, WaterQualityForegroundColors[colorIndex]);
                    return;
                }

                return;
            }
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Water label highlight failed: {ex.Message}");
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
            var identifier = NormalizeIdentifier(item.identifier ?? string.Empty);
            var name = NormalizeIdentifier(item.name ?? string.Empty);
            return identifier == "aquascan" || identifier == "chemscanner" ||
                   name == "aquascan" || name == "chemscanner";
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

    // One composition row. Shared by the tooltip renderer and the read-only API
    // so both expose the same names, volume text and percentages.
    private readonly struct CompositionRowData
    {
        internal readonly string Name;
        internal readonly string AmountText;
        internal readonly double Percentage;
        internal readonly string PercentageText;
        internal readonly bool Tiny;
        internal readonly int ColorIndex;

        internal CompositionRowData(
            string name,
            string amountText,
            double percentage,
            string percentageText,
            bool tiny,
            int colorIndex)
        {
            Name = name;
            AmountText = amountText;
            Percentage = percentage;
            PercentageText = percentageText;
            Tiny = tiny;
            ColorIndex = colorIndex;
        }
    }

    // Read-only rows for the interoperability slot ("aquasense.composition.v1").
    // Returns null when the composition feature is unavailable for this item (the
    // same conditions the tooltip uses) and an empty list when the container holds
    // no recognized liquid. Bar-mode semantics: traces are included even when the
    // hide-zero display setting is on, and the "bars" display setting is ignored.
    internal static IReadOnlyList<(string Name, string Amount, string Percent, float Ratio, int ColorRgb)>?
        GetCompositionApiRows(GameItem item)
    {
        try
        {
            if (item == null || !IsFilledWaterContainer(item))
                return null;
            if (Core.ShowWaterComposition is not { Value: true })
                return null;
            if (!(Core.ShowCompositionWithoutScanner is { Value: true } || HasWaterScanner()))
                return null;

            var rows = BuildCompositionRows(item);
            if (rows.Count == 0)
                return Array.Empty<(string, string, string, float, int)>();

            var result = new (string Name, string Amount, string Percent, float Ratio, int ColorRgb)[rows.Count];
            for (var index = 0; index < rows.Count; index++)
            {
                var row = rows[index];
                result[index] = (row.Name, row.AmountText, row.PercentageText,
                    Math.Clamp((float)(row.Percentage / 100d), 0f, 1f),
                    CompositionBarColors[row.ColorIndex % CompositionBarColors.Length]);
            }

            return result;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Composition API failed for {(item == null ? "null" : item.identifier)}: {ex.Message}");
            return null;
        }
    }

    private static List<CompositionRowData> BuildCompositionRows(GameItem item)
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

        var rows = new List<CompositionRowData>();
        if (entries.Count == 0 || totalAmount <= 0)
            return rows;

        var colorIndex = 0;
        foreach (var entry in entries)
        {
            var percentage = 100d * entry.Amount / totalAmount;
            var tiny = Math.Round(percentage, 2, MidpointRounding.AwayFromZero) <= 0d;

            var percentageText = WaterFeatureHelper.FormatPurityPercentageFromDouble(percentage);
            if (string.IsNullOrEmpty(percentageText))
                percentageText = $"{percentage:0.00}%";

            // A share that rounds to 0.00% would read as "nothing", so it is marked
            // instead of pretending to be a whole percent. Bar mode always shows
            // such rows; text mode only does when the hide-zero setting is off.
            // The single-angle quote avoids '<', which TMP reads as a tag opener.
            if (tiny)
                percentageText = TinyPercentageText;

            rows.Add(new CompositionRowData(
                entry.Name,
                FormatVolume(entry.Amount),
                percentage,
                percentageText,
                tiny,
                colorIndex++));
        }

        if (Core.SortCompositionByAmount is { Value: true } && rows.Count > 1)
        {
            // Highest share first; the colour index is the tie-breaker so equal
            // shares keep a stable order across frames.
            rows.Sort((left, right) =>
            {
                var byShare = right.Percentage.CompareTo(left.Percentage);
                return byShare != 0 ? byShare : left.ColorIndex.CompareTo(right.ColorIndex);
            });
        }

        return rows;
    }

    private static void AppendComposition(RichTextBuilder builder, GameItem item)
    {
        var rows = BuildCompositionRows(item);
        if (rows.Count == 0)
        {
            var color = WaterHelper.GetLiquidContainerColor(item);
            if (!string.IsNullOrEmpty(color))
                AddLine(builder, $"Color: {color}");
            return;
        }

        var showBars = Core.ShowCompositionBars?.Value ?? true;
        var hideZero = Core.HideZeroComponents?.Value ?? true;

        if (!showBars)
        {
            foreach (var row in rows)
                if (!(hideZero && row.Tiny))
                    AddLine(builder, $"{row.Name}: {row.AmountText} ({row.PercentageText})");
            return;
        }

        // The bar mode lays every row out as
        //   "name: amount" | bar | right-aligned percentage
        // Columns use absolute <pos> tags in pixels: the
        // tooltip font is proportional, so counting display cells cannot line the
        // columns up.
        var rowsText = new List<(string Text, double Percentage, string PercentageText, int ColorIndex)>(rows.Count);
        var maxPercentageChars = 0;
        var hasWideText = false;
        foreach (var row in rows)
        {
            var text = $"{row.Name}: {row.AmountText}";
            maxPercentageChars = Math.Max(maxPercentageChars, row.PercentageText.Length);
            hasWideText |= HasWideText(text);
            rowsText.Add((text, row.Percentage, row.PercentageText, row.ColorIndex));
        }

        // The tooltip builder counts characters (a CJK glyph counts as one) and wraps
        // a row past 35 characters in CJK tooltips, 40 in Latin ones. Only names
        // that still exceed this budget get an ellipsis.
        var rowBudget = hasWideText ? 35 : 40;
        // One invisible link character anchors each native UI bar.
        var textLimit = Math.Max(4, rowBudget - maxPercentageChars - 1);
        var textColumn = 0;
        var percentageColumn = 0;
        for (var index = 0; index < rowsText.Count; index++)
        {
            var row = rowsText[index];
            var text = TruncateChars(row.Text, textLimit);
            rowsText[index] = (text, row.Percentage, row.PercentageText, row.ColorIndex);
            textColumn = Math.Max(textColumn, MeasureTextWidth(text));
            percentageColumn = Math.Max(percentageColumn, MeasureTextWidth(row.PercentageText));
        }
        var barColumn = CompositionBarWidth;

        for (var index = 0; index < rowsText.Count; index++)
        {
            var row = rowsText[index];
            AddCompositionRow(
                builder,
                row.Text,
                textColumn + CompositionColumnGap,
                row.Percentage,
                textColumn + CompositionColumnGap + barColumn + CompositionColumnGap
                    + percentageColumn - MeasureTextWidth(row.PercentageText),
                row.PercentageText,
                row.ColorIndex);
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
        // Below 0.1 ml the millilitre form needs three decimals and reads badly
        // ("0.003 ml"), so traces switch to whole microlitres. The game's own
        // scanner panel uses the ASCII "uL" wording for the same range.
        if (amountInMicroLiters > 0 && amountInMicroLiters < 100)
            return $"{amountInMicroLiters} uL";

        return $"{amountInMicroLiters / 1000d:0.###} ml";
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
            else if ((replaceFilters || Core.AutoReplaceCloth is { Value: true }) && PendingFilters.Count > 0 && Time.unscaledTime >= _nextMaintenanceAt)
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
        foreach (var bucket in ClothCache) bucket.Clear();
        TooltipSections.Clear();
        MeasureTextWidths.Clear();
        _spaceUnitScale = -1f;
    }

    private static IEnumerable<object?> ScanInventory()
    {
        var buckets = new[] { new List<(GameItem, GameInventory)>(), new List<(GameItem, GameInventory)>(), new List<(GameItem, GameInventory)>() };
        var clothBucket = new List<(GameItem, GameInventory)>();
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
                if (Core.AutoReplaceCloth is { Value: true })
                {
                    if (IsWaterContainer(item) && TryGetDepletedCloth(item, out var cloth)) QueueFilter(cloth!);
                    if (IsAvailableCloth(item) && item.parentInventory != null)
                        clothBucket.Add((item, item.parentInventory));
                }
            }
            yield return null;
        }
        FilterCache = buckets;
        ClothCache = new[] { clothBucket };
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
            var cloth = IsClothFilterItem(filter);
            var container = filter.parentInventory?.GetParentItem();
            if (container == null || !IsWaterContainer(container))
            {
                yield return null;
                continue;
            }
            GameItem? installed = null;
            var upToDate = cloth
                ? TryGetDepletedCloth(container, out installed)
                : TryGetDepletedFilter(container, out installed);
            if (!upToDate || installed != filter)
            {
                yield return null;
                continue;
            }
            var swapped = false;
            var attempted = false;
            foreach (var bucket in (cloth ? ClothCache : FilterCache))
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
                        if (cloth
                                ? Core.AutoReplaceCloth is not { Value: true }
                                : Core.AutoReplaceFilters is not { Value: true })
                            yield break;
                        if (cached.Item.parentInventory != cached.Inventory ||
                            !(cloth ? IsAvailableCloth(cached.Item) : IsAvailableFilter(cached.Item)))
                        {
                            bucket.RemoveAt(j);
                            continue;
                        }
                        var durability = DurabilityHelper.GetRealDurability(cached.Item);
                        if (!MeetsFilterDurabilityThreshold(durability))
                        {
                            j++;
                            continue;
                        }
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

    // Called for every cached candidate before it can become the replacement:
    // when the player set a durability floor, a filter below it is skipped and
    // the next cached filter is compared instead.
    private static bool MeetsFilterDurabilityThreshold(int durability)
    {
        if (Core.MinFilterDurabilityEnabled is not { Value: true })
            return true;
        return durability >= (Core.MinFilterDurability?.Value ?? DefaultMinFilterDurability);
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

    private static bool TryGetDepletedCloth(GameItem container, out GameItem? cloth)
    {
        cloth = null;
        try
        {
            cloth = WaterHelper.GetFilterItem(container);
            return cloth != null && IsClothFilterItem(cloth) &&
                   DurabilityHelper.GetRealDurability(cloth) <= 0;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Cannot inspect cloth in {SafeIdentifier(container)}: {ex.Message}");
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
                IsInstalledInWaterContainer(item) ||
                IsInExcludedFilterHost(item))
                return false;

            return DurabilityHelper.GetRealDurability(item) > 0;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Cannot inspect replacement {SafeIdentifier(item)}: {ex.Message}");
            return false;
        }
    }

    private static bool IsAvailableCloth(GameItem item)
    {
        try
        {
            if (!IsClothFilterItem(item) ||
                !TryGetParentInventory(item, out var parent) || parent == null ||
                IsInstalledInWaterContainer(item) ||
                IsInExcludedFilterHost(item))
                return false;

            return DurabilityHelper.GetRealDurability(item) > 0;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Cannot inspect cloth replacement {SafeIdentifier(item)}: {ex.Message}");
            return false;
        }
    }

    private static bool ReplaceFilter(
        GameItem container, GameItem depletedFilter, GameItem replacement, out string toastMessage, out bool attempted)
    {
        toastMessage = string.Empty;
        attempted = false;
        if (!CanSwapFilterPlacement(depletedFilter, replacement) ||
            !AreItemsStillOwned(container, replacement))
            return false;

        GameItem? installed;
        var upToDate = IsClothFilterItem(depletedFilter)
            ? TryGetDepletedCloth(container, out installed)
            : TryGetDepletedFilter(container, out installed);
        if (!upToDate || installed! != depletedFilter ||
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

    // Filters slotted in these hosts belong to the host's own water loop: the
    // host reads them every cycle, and unlike the pitcher slot they are not
    // stored inside a water container item, so IsInstalledInWaterContainer
    // cannot see them. They must never be used as spares or replaced in place.
    private static bool IsInExcludedFilterHost(GameItem filter)
    {
        var visited = new HashSet<object>();
        if (!TryGetParentInventory(filter, out var inventory))
            return false;

        while (inventory != null)
        {
            if (!visited.Add(inventory))
                return false;

            var host = inventory.GetParentItem();
            if (host == null)
                return false;
            foreach (var tag in ExcludedFilterHostTags)
            {
                if (host.IsTag(tag))
                    return true;
            }

            var identifier = NormalizeIdentifier(host.identifier ?? string.Empty);
            if (ExcludedFilterHostIdentifiers.Contains(identifier))
                return true;

            if (!TryGetParentInventory(host, out inventory))
                return false;
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

    // The water-container slot also accepts items that are not consumable
    // filters. The UV lamp carries WATER_FILTER because its UV cleaning is
    // triggered from the slot, but the game consumes it through
    // WaterHelper.OnUVUsed instead of durability, so it must never be picked
    // as a filter. Black lamps are not filters and carry no WATER_FILTER tag,
    // so the tag check below excludes them without another identifier list.
    private static readonly string[] UvLampIdentifiers = { "uvfilter", "uvlamp" };

    // True for anything the game treats as a filter in the water-container
    // slot. Both consumable filters and coarse chemistry cloth are tagged
    // WATER_FILTER and consumed by durability in WaterHelper.HandleFilter; the
    // UV lamp is tagged too, but the game consumes it through
    // WaterHelper.OnUVUsed instead of durability. Black lamps carry no
    // WATER_FILTER tag, so the tag check also excludes them.
    private static bool IsWaterFilterSlotItem(GameItem item)
    {
        try
        {
            var identifier = NormalizeIdentifier(item.identifier ?? string.Empty);
            foreach (var uvLamp in UvLampIdentifiers)
            {
                if (identifier == uvLamp)
                    return false;
            }

            return item.IsTag("WATER_FILTER");
        }
        catch
        {
            return false;
        }
    }

    // WATER_FILTER is the flag the game itself reads in
    // WaterHelper.GetFilterItem/HandleFilter. It also covers
    // hydroreed_filter, which an older keyword blacklist wrongly rejected even
    // though a water purifier accepts and consumes it like any other filter.
    // The pitcher slot additionally accepts coarse chemistry cloth such as
    // kotton_fabric (韧棉织物, FILTER_SIZE_TAG 150). It can be placed in a
    // water container, but it only catches very large particles, so it must
    // not replace a real filter.
    private static bool IsFilterItem(GameItem item)
    {
        try
        {
            return IsWaterFilterSlotItem(item) && IsAtLeastAsFineAsVanilla(item);
        }
        catch
        {
            return false;
        }
    }

    // Coarse chemistry cloth (韧棉织物, kotton_fabric) shares the WATER_FILTER
    // tag and is consumed by durability in WaterHelper.HandleFilter, but its
    // FILTER_SIZE_TAG is 150, far coarser than a real filter, so IsFilterItem
    // deliberately rejects it. Cloth auto-replacement is a separate opt-in path
    // that only ever swaps cloth for cloth: a fine filter must not be spent on
    // coarse filtration, and a coarse cloth must not replace a fine filter.
    private static bool IsClothFilterItem(GameItem item)
    {
        try
        {
            return IsWaterFilterSlotItem(item) &&
                   NormalizeIdentifier(item.identifier ?? string.Empty) == "kottonfabric";
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
            _vanillaFilterSize = ReadFilterSize(prototype);
            _vanillaFilterIdentityLoaded = true;
            prototype.Destroy();
        }
        catch
        {
        }
    }

    private static int ReadFilterSize(GameItem item)
    {
        try
        {
            return item.state?.GetTag("FILTER_SIZE_TAG")?.valueInt ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    // The pitcher slot also accepts coarse tagged items. In
    // WaterHelper.HandleFilter a filter only catches liquids whose
    // particleSize reaches FILTER_SIZE_TAG, so a larger size means a coarser
    // filter; only filters at least as fine as the vanilla one are useful for
    // water trading. Items without the tag are kept so that filters added by
    // other mods do not need to know about it.
    private static bool IsAtLeastAsFineAsVanilla(GameItem item)
    {
        var filterSize = ReadFilterSize(item);
        return filterSize <= 0 || filterSize <= VanillaFilterSize();
    }

    private static int VanillaFilterSize()
    {
        EnsureVanillaFilterIdentity();
        return _vanillaFilterSize > 0 ? _vanillaFilterSize : VanillaFilterSizeFallback;
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

    // Width in the same coordinates as measured text.
    private const int CompositionBarWidth = TooltipCompositionBars.Width;

    // Pixels between two composition columns.
    private const int CompositionColumnGap = 8;

    // Fallback cell width used when the game's text renderer is not reachable yet:
    // the UI grid is 8 px wide and CJK glyphs take two cells.
    private const int CompositionCellPixels = 8;

    // A share too small to round to "0.01%" is marked with this label. The
    // single-angle quote avoids '<', which TMP would read as a tag opener.
    private const string TinyPercentageText = "‹0.01%";

    // One colour per row keeps the bars easy to tell apart; the values are
    // literal 0xRRGGBB colours because ColorPalette is an int-backed enum.
    private static readonly int[] CompositionBarColors =
    {
        0x63ABA5,
        0xA494BC,
        0x8FA9BF,
        0xC28D7E,
        0xC4BE95,
        0xBA8C9C,
        0x3FB8AF,
        0xE37FA6
    };

    // The tooltip font is proportional, so column positions are measured in pixels
    // and emitted as <pos> tags. A hidden TMP node borrowed from the game's own text
    // render prefab gives the exact advance width; the cache is dropped on load
    // because a locale change can swap the font.
    private static readonly System.Collections.Generic.Dictionary<string, int> MeasureTextWidths = new();
    private static TextMeshProUGUI? _measureText;
    private static bool _measureHasNode;
    private static float _spaceUnitScale = -1f;

    private static void AddCompositionRow(
        RichTextBuilder builder,
        string text,
        int barPosition,
        double percentage,
        int percentagePosition,
        string percentageText,
        int index)
    {
        var none = (RenderHandler.ColorPalette)NoColor;
        var color = CompositionBarColors[index % CompositionBarColors.Length];
        var fraction = (percentage / 100d).ToString("R", System.Globalization.CultureInfo.InvariantCulture);
        var anchor = PositionTag(barPosition)
            + $"<link=\"{TooltipCompositionBars.LinkPrefix}{fraction}:{color:X6}\">\u00A0</link>";
        // The link only marks the row position; Image components draw the bar.
        builder.Add(text + anchor + PositionTag(percentagePosition) + percentageText, none, "\n");
        builder.AddProgressBarOld(0f, 0, none, none);
    }

    public static void AfterTooltipTextMesh(TextMeshProUGUI __instance)
    {
        if (__instance == null)
            return;

        var bars = __instance.GetComponent<TooltipCompositionBars>();
        // TMP clears m_text while rebuilding, so the mesh hook cannot assume a string here.
        var text = __instance.text;
        if (Core.ShowCompositionBars is { Value: true }
            && !string.IsNullOrEmpty(text)
            && text.IndexOf(TooltipCompositionBars.LinkPrefix, StringComparison.Ordinal) >= 0)
        {
            (bars ??= __instance.gameObject.AddComponent<TooltipCompositionBars>()).Queue(__instance);
        }
        else if (bars != null)
        {
            bars.RequestHide();
        }
    }

    private static string PositionTag(float pixels)
    {
        var units = (pixels / SpaceUnitScale()).ToString("0.####", System.Globalization.CultureInfo.InvariantCulture);
        return $"<pos={units}px>";
    }

    private static float SpaceUnitScale()
    {
        if (!_measureHasNode)
            return 1f;

        if (_spaceUnitScale > 0f)
            return _spaceUnitScale;

        var baseline = MeasureTextWidth("A");
        var probe = MeasureTextWidth("<space=100px>A");
        _spaceUnitScale = Math.Max(0.01f, (probe - baseline) / 100f);
        return _spaceUnitScale;
    }

    private static int MeasureTextWidth(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;

        if (MeasureTextWidths.TryGetValue(text, out var cached))
            return cached;

        var width = EstimateTextWidth(text);
        var measured = false;
        try
        {
            if (RenderHandler.current != null)
            {
                if (_measureText == null)
                {
                    var node = TreeNodeRender.InstantiateText();
                    var measure = node != null ? node.text : null;
                    if (measure != null)
                    {
                        measure.enableWordWrapping = false;
                        measure.overflowMode = TextOverflowModes.Overflow;
                    }
                    _measureText = measure;
                }

                if (_measureText != null)
                {
                    _measureText.text = text;
                    width = Math.Max(1, Mathf.CeilToInt(_measureText.preferredWidth));
                    _measureText.text = string.Empty;
                    measured = true;
                }
            }
        }
        catch (Exception ex)
        {
            _measureText = null;
            Core.Log?.Warning($"Cannot measure tooltip text: {ex.Message}");
            measured = false;
        }

        if (measured != _measureHasNode)
        {
            // Estimates cached while the renderer was unreachable must not leak into a
            // pixel layout (and vice versa): drop everything when the mode flips.
            _measureHasNode = measured;
            _spaceUnitScale = -1f;
            MeasureTextWidths.Clear();
        }

        if (MeasureTextWidths.Count > 256)
            MeasureTextWidths.Clear();
        MeasureTextWidths[text] = width;
        return width;
    }

    // Fallback used before the game's renderer is reachable; the tests run on it.
    private static int EstimateTextWidth(string text)
    {
        var width = 0;
        foreach (var value in text)
            width += IsWideCharacter(value) ? CompositionCellPixels * 2 : CompositionCellPixels;
        return width;
    }

    private static string TruncateChars(string text, int maxChars)
    {
        if (text.Length <= maxChars)
            return text;
        return text.Substring(0, Math.Max(0, maxChars - 1)) + '…';
    }

    private static bool HasWideText(string text)
    {
        foreach (var value in text)
            if (IsWideCharacter(value))
                return true;
        return false;
    }

    private static bool IsWideCharacter(char value)
    {
        return value >= '\u1100' && (
            value <= '\u115F' ||
            (value >= '\u2E80' && value <= '\uA4CF' && value != '\u303F') ||
            (value >= '\uAC00' && value <= '\uD7A3') ||
            (value >= '\uF900' && value <= '\uFAFF') ||
            (value >= '\uFE30' && value <= '\uFE4F') ||
            (value >= '\uFF00' && value <= '\uFF60') ||
            (value >= '\uFFE0' && value <= '\uFFE6'));
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
