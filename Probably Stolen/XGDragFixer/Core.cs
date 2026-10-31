using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XGDragFixer;

/// <summary>How the diagnostic A/B switch is driven.</summary>
public enum ABMode
{
    Off,
    Hotkey,
    Auto,
    Both
}

public sealed class Core : IMod
{
    internal const string HarmonyId = "chiqingsan.BetterPerformance";

    private GameObject _runner;
    private HarmonyLib.Harmony? _harmony;
    private bool _installAttempted;
    private bool _settingsPatchAttempted;

    public void Init(ModManifest manifest)
    {
        Log = new ModLog(manifest);

        try
        {
            var config = ModConfig.For(manifest.ID);
            FixGhostDrag = config.Define("FixGhostDrag", true, "拖拽",
                () => "修复点击后偶发粘住拖拽");
            FastSlotFind = config.Define("FastSlotFind", true, "拖拽",
                () => "装满物品的容器里拖入东西时不再卡顿");
            FastContainerOpen = config.Define("FastContainerOpen", true, "容器",
                () => "首次打开容器时提前挂载窗口");
            SkipLayoutMark = config.Define("SkipContainerLayoutMark", true, "容器",
                () => "打开容器时跳过冗余布局重建");
            StickyLayoutFrames = config.Define("SkipLayoutMarkFrames", 2, "容器",
                () => "打开后额外跳过的帧数", 0, 8);
            SkipMaskNotify = config.Define("SkipMaskNotify", true, "容器",
                () => "打开容器时跳过冗余遮罩通知");
            SetTitleSkipValidate = config.Define("SetTitleSkipValidate", true, "容器",
                () => "设置标题时跳过重复验证");
            MiddleClickPreview = config.Define("MiddleClickPreview", true, "中键预览",
                () => "按住中键悬停容器物品快速预览内容");
            PreviewHideTooltip = config.Define("PreviewHideTooltip", true, "中键预览",
                () => "预览内容时隐藏物品提示框");
            TooltipCacheEnabled = config.Define("TooltipCache", true, "提示框",
                () => "缓存悬停提示框，避免每帧重算");
            TooltipCacheFrames = config.Define("TooltipCacheFrames", 30, "提示框",
                () => "提示框缓存帧数", 1, 120);
            TooltipIdleSkip = config.Define("TooltipIdleSkip", false, "提示框",
                () => "提示框内容未变化时跳过重复刷新（实测会让动态内容更新变慢，默认关）");
            TooltipIdleFrames = config.Define("TooltipIdleFrames", 6, "提示框",
                () => "最多连续跳过的帧数", 1, 30);
            RaycastFrameCache = config.Define("RaycastFrameCache", true, "每帧",
                () => "同一帧内重复的 UI 射线直接复用");
            ContextMenuSkip = config.Define("ContextMenuSkip", false, "每帧",
                () => "右键菜单打开且鼠标未动时跳过刷新（实测收益低，默认关）");
            AgingBatch = config.Define("AgingBatch", true, "结算",
                () => "睡眠陈化合并刷新，避免逐件刷界面");
            NightValidateBatchEnabled = config.Define("NightValidateBatch", true, "结算",
                () => "结算期间合并库存界面刷新");
            NightSimUIQuiet = config.Define("NightSimUIQuiet", true, "结算",
                () => "夜间模拟跳过纯界面刷新，不影响结算（需重启）");
            ChemTagCacheEnabled = config.Define("ChemTagCache", true, "结算",
                () => "化学模拟缓存标签读取，避免重复克隆（需重启）");
            MachineModuleSkip = config.Define("MachineModuleSkip", true, "机器",
                () => "机器生效时跳过模块重复重算");
            SpriteRefreshGuard = config.Define("SpriteRefreshGuard", true, "贴图",
                () => "贴图状态未变化时跳过重复写入");
            SlotIndexDirect = config.Define("SlotIndexDirect", true, "库存",
                () => "槽位标记直接扫描列表，避免复制");
            SkipStoreHoverRaycast = config.Define("SkipStoreHoverRaycast", true, "每帧",
                () => "商店悬停检测：鼠标静止时跳过重复射线");
            StoreHoverIdleFrames = config.Define("StoreHoverIdleFrames", 15, "每帧",
                () => "鼠标静止时最多连续跳过的帧数", 1, 60);
            TextFieldFocusCache = config.Define("TextFieldFocusCache", true, "每帧",
                () => "输入框焦点检测每帧只算一次");
            SizeFitterGuard = config.Define("SizeFitterGuard", true, "每帧",
                () => "尺寸自适应组件只生效一次");
            StartupFaultGuard = config.Define("StartupFaultGuard", true, "稳定性",
                () => "读档时游戏在后台可能出现的界面未就绪报错自动忽略，避免连锁报错卡死（需重启）");
            LoadFocusBoost = config.Define("LoadFocusBoost", true, "稳定性",
                () => "读档时临时解除后台限帧，保证加载逻辑跑满帧（需重启）");
            PerfDiagnosticsEnabled = config.Define("PerfDiagnostics", false, "诊断调试",
                () => "记录帧时间与各补丁统计到游戏日志。仅供测试排查，普通游玩请保持关闭");
            PerfReportSeconds = config.Define("PerfReportSeconds", 10, "诊断调试",
                () => "日志输出间隔秒数", 2, 120);
            PerfABMode = config.DefineEnum("PerfABMode", ABMode.Hotkey, "诊断调试",
                () => "A/B 切换方式（热键/自动/两者）",
                () => "用于对比“补丁全开/全关”的帧时间；此项需 PerfDiagnostics 开启。修改后立即生效");
            PerfABKey = config.Define("PerfABKey", "F8", "诊断调试",
                () => "A/B 热键",
                new[] { "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12" },
                () => "热键/两者模式下生效；按下后屏幕顶部会提示当前状态。修改后立即生效");
            PerfAutoABSeconds = config.Define("PerfAutoABSeconds", 300, "诊断调试",
                () => "自动切换间隔（秒）", 30, 3600,
                () => "例如 300 = 每 5 分钟自动切换一次。修改后立即生效");
        }
        catch (Exception ex)
        {
            Log.Warning($"配置初始化失败: {ex.Message}");
        }
    }

    public void OnEnable()
    {
        if (_runner != null)
        {
            return;
        }

        _installAttempted = false;
        _settingsPatchAttempted = false;
        _runner = new GameObject("BetterPerformance");
        Object.DontDestroyOnLoad(_runner);
        _runner.AddComponent<ModRunner>().Mod = this;
        XGDragFixer.LoadFocusBoost.Init();

        if (PerfDiagnosticsEnabled?.Value == true)
        {
            XGDragFixer.PerfDiagnostics.Enable(
                PerfReportSeconds?.Value ?? 10,
                PerfABMode?.Value ?? ABMode.Hotkey,
                PerfAutoABSeconds?.Value ?? 300,
                PerfABKey?.Value ?? "F8");
        }
    }

    internal void OnUpdate()
    {
        // The load-time safety net runs from the first frame on, before the world is
        // ready: both the swallowed-exception counter and the background frame boost
        // matter while a save is still loading.
        XGDragFixer.StartupFaultGuard.Tick();
        XGDragFixer.LoadFocusBoost.Tick();

        // The mod settings panel is reachable from the main menu, long before the save
        // world exists and InstallPatches runs. Patch its renderer once the main menu UI
        // is alive (ModUIManager itself only awakes when the mods page is opened, which
        // would be too late for that first open): patching inside ModLoader.Awake is too
        // early, because early patches can run target static constructors before the
        // game's assets and localization are ready.
        if (!_settingsPatchAttempted && !ConfigValueLabels.Installed
            && (ModUIManager.Instance != null || MainMenuUIController.Instance != null))
        {
            _settingsPatchAttempted = true;
            try
            {
                ConfigValueLabels.Install(_harmony ??= new HarmonyLib.Harmony(HarmonyId));
            }
            catch (Exception ex)
            {
                Log.Warning($"设置面板补丁安装失败: {ex.Message}");
            }
        }

        // OnEnable runs inside ModLoader.Awake, before the game world and its assets
        // are ready; patching then could run target static constructors too early.
        if (!_installAttempted)
        {
            if (!PlayerStore.IsInstanceExist() || PlayerStore.Instance == null || EmporiumEntry.Instance == null)
            {
                return;
            }

            _installAttempted = true;
            InstallPatches();
        }

        SyncDiagnostics();
        HoverPeek.Tick();
        LayoutSuppressor.Tick();
    }

    /// <summary>
    /// The diagnostic switch is config-driven and applied live: turning it on/off in
    /// the mod settings starts/stops log-based reporting without a restart.
    /// </summary>
    private void SyncDiagnostics()
    {
        var enabled = PerfDiagnosticsEnabled?.Value ?? false;
        if (enabled && !XGDragFixer.PerfDiagnostics.Enabled)
        {
            XGDragFixer.PerfDiagnostics.Enable(
                PerfReportSeconds?.Value ?? 10,
                PerfABMode?.Value ?? ABMode.Hotkey,
                PerfAutoABSeconds?.Value ?? 300,
                PerfABKey?.Value ?? "F8");
            XGDragFixer.PerfDiagnostics.ShowStartupNotice();
        }
        else if (!enabled && XGDragFixer.PerfDiagnostics.Enabled)
        {
            XGDragFixer.PerfDiagnostics.Shutdown();
        }
    }

    public void OnDisable()
    {
        _harmony?.UnpatchAll(HarmonyId);
        _harmony = null;

        if (_runner != null)
        {
            Object.Destroy(_runner);
        }

        _runner = null;

        HoverPeek.ResetState();
        LayoutSuppressor.Reset();
        TooltipCache.Clear();
        XGDragFixer.FastSlotFind.Reset();
        XGDragFixer.RaycastFrameCache.Reset();
        XGDragFixer.TooltipIdleSkip.Reset();
        XGDragFixer.ContextMenuSkip.Reset();
        XGDragFixer.AgingBatch.Reset();
        XGDragFixer.MachineBatch.Reset();
        XGDragFixer.NightValidateBatch.Reset();
        XGDragFixer.NightSimQuiet.Reset();
        XGDragFixer.ChemBreakdown.Reset();
        XGDragFixer.ChemTagCache.Reset();
        XGDragFixer.NightBreakdown.Reset();
        XGDragFixer.StoreHoverCache.Reset();
        XGDragFixer.TextFieldFocusCache.Reset();
        XGDragFixer.SizeFitterGuard.Reset();
        XGDragFixer.StartupFaultGuard.Reset();
        XGDragFixer.LoadFocusBoost.Reset();
        ConfigValueLabels.Reset();
        XGDragFixer.PerfDiagnostics.Shutdown();
    }

    private void InstallPatches()
    {
        var harmony = _harmony ??= new HarmonyLib.Harmony(HarmonyId);
        var ok = 0;
        var failed = 0;

        void Patch(Type? type, string methodName, Type[] argumentTypes, string? prefix, string? postfix, bool optional)
        {
            var original = type == null ? null : AccessTools.Method(type, methodName, argumentTypes);
            if (original == null)
            {
                if (optional)
                {
                    Log.Log($"[兼容] 未找到 {type?.FullName}.{methodName}，跳过该子功能");
                }
                else
                {
                    failed++;
                    Log.Warning($"未找到补丁目标 {type?.FullName}.{methodName}");
                }

                return;
            }

            try
            {
                var pre = prefix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(Hooks), prefix));
                var post = postfix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(Hooks), postfix));
                if (pre == null && post == null)
                {
                    return;
                }

                harmony.Patch(original, pre, post);
                ok++;
            }
            catch (Exception ex)
            {
                failed++;
                Log.Warning($"补丁失败 {type!.Name}.{methodName}: {ex.Message}");
            }
        }

        try
        {
            Patch(typeof(ItemMouseDragHandler), "StartDrag", Type.EmptyTypes,
                nameof(Hooks.StartDragPrefix), null, false);
            Patch(typeof(GameGridInventory), nameof(GameGridInventory.TryFindOneValidInventorySlot),
                new[] { typeof(GameItem), typeof(bool) }, nameof(Hooks.TryFindSlotPrefix), null, false);

            Patch(typeof(PixelWindow), nameof(PixelWindow.Show),
                new[] { typeof(Vector2), typeof(Vector2), typeof(bool) },
                nameof(Hooks.ColdStartPrefix), nameof(Hooks.ShowPostfix), false);
            Patch(typeof(PixelWindow), nameof(PixelWindow.SetTitle), new[] { typeof(string) },
                nameof(Hooks.SetTitlePrefix), nameof(Hooks.SetTitlePostfix), false);
            Patch(typeof(PixelWindow), nameof(PixelWindow.Validate), Type.EmptyTypes,
                nameof(Hooks.ValidatePrefix), null, false);

            Patch(typeof(LayoutRebuilder), nameof(LayoutRebuilder.MarkLayoutForRebuild),
                new[] { typeof(RectTransform) }, nameof(Hooks.MarkLayoutPrefix), null, true);
            Patch(AccessTools.TypeByName("UnityEngine.UI.MaskUtilities"), "Notify2DMaskStateChanged",
                new[] { typeof(Component) }, nameof(Hooks.MaskNotifyPrefix), null, true);

            Patch(typeof(OverlayHandler), nameof(OverlayHandler.ShowTooltip),
                new[] { typeof(Vector2), typeof(RichText), typeof(int) },
                nameof(Hooks.BlockTooltipPrefix), null, true);
            Patch(typeof(OverlayHandler), nameof(OverlayHandler.ShowTooltip),
                new[] { typeof(Vector2), typeof(Vector2), typeof(RichText), typeof(int) },
                nameof(Hooks.BlockTooltipPrefix), null, true);
            Patch(typeof(OverlayHandler), nameof(OverlayHandler.ShowTooltip2),
                new[] { typeof(Vector2), typeof(Vector2), typeof(RichText), typeof(int) },
                nameof(Hooks.BlockTooltipPrefix), null, true);
            Patch(typeof(CustomUIManager), nameof(CustomUIManager.ShowTooltip),
                new[] { typeof(string), typeof(CustomUITooltipTrigger) },
                nameof(Hooks.BlockTooltipPrefix), null, true);

            Patch(typeof(GameItemElement), nameof(GameItemElement.GetTooltip), Type.EmptyTypes,
                nameof(Hooks.GetTooltipPrefix), nameof(Hooks.GetTooltipPostfix), true);
            Patch(typeof(GameItemElement), nameof(GameItemElement.GetTooltipAdvanced), Type.EmptyTypes,
                nameof(Hooks.GetAdvancedTooltipPrefix), nameof(Hooks.GetAdvancedTooltipPostfix), true);

            // v1.1.0 performance patches.
            Patch(typeof(OverlayHandler), nameof(OverlayHandler.ShowTooltip),
                new[] { typeof(Vector2), typeof(Vector2), typeof(RichText), typeof(int) },
                nameof(Hooks.TooltipIdlePrefix), nameof(Hooks.TooltipIdlePostfix), true);
            Patch(typeof(OverlayHandler), "HideTooltip", Type.EmptyTypes,
                null, nameof(Hooks.TooltipHidePostfix), true);
            Patch(typeof(EventSystem), nameof(EventSystem.RaycastAll),
                new[] { typeof(PointerEventData), typeof(List<RaycastResult>) },
                nameof(Hooks.RaycastPrefix), nameof(Hooks.RaycastPostfix), true);
            Patch(typeof(ItemContextHandler), "OnEventUpdate", Type.EmptyTypes,
                nameof(Hooks.ContextMenuPrefix), nameof(Hooks.ContextMenuPostfix), true);
            Patch(typeof(PlayerStore), "EndNight", Type.EmptyTypes,
                nameof(Hooks.EndNightPrefix), nameof(Hooks.EndNightPostfix), true);
            Patch(typeof(AgableHelper), "OnAge", new[] { typeof(GameItem) },
                nameof(Hooks.OnAgePrefix), null, true);
            Patch(typeof(MachineHelper), "OnMachineActioned",
                new[] { typeof(GameItem), typeof(GameInventory) },
                nameof(Hooks.MachineActionedPrefix), nameof(Hooks.MachineActionedPostfix), true);
            Patch(typeof(ModuleEffectHelper), "ActivateOnUseAction",
                new[] { typeof(GameItem), typeof(GameInventory), typeof(GameItem) },
                nameof(Hooks.ModuleEffectPrefix), null, true);
            Patch(typeof(GameItemElement), "UpdateModSprite",
                new[]
                {
                    typeof(int), typeof(Sprite), typeof(Vector2), typeof(int), typeof(bool),
                    typeof(ItemSpriteModifier)
                },
                nameof(Hooks.UpdateModSpritePrefix), null, true);
            Patch(typeof(GameGridScrollableInventory),
                nameof(GameGridScrollableInventory.TryFindOneValidInventorySlot),
                new[] { typeof(GameItem), typeof(bool) },
                nameof(Hooks.ScrollSlotFindPrefix), null, false);
            Patch(typeof(GameGridInventory), nameof(GameGridInventory.GetIncompleteItemInventorySlot),
                new[] { typeof(GameItem) }, nameof(Hooks.SlotIndexPrefix), null, true);
            Patch(typeof(GameGridInventory), nameof(GameGridInventory.GetItemInventorySlot),
                new[] { typeof(GameItem) }, nameof(Hooks.ItemSlotIndexPrefix), null, true);
            Patch(typeof(GameGridScrollableInventory), nameof(GameGridScrollableInventory.GetIncompleteItemInventorySlot),
                new[] { typeof(GameItem) }, nameof(Hooks.ScrollSlotIndexPrefix), null, true);
            Patch(typeof(GameGridScrollableInventory), nameof(GameGridScrollableInventory.GetItemInventorySlot),
                new[] { typeof(GameItem) }, nameof(Hooks.ScrollItemSlotIndexPrefix), null, true);

            // v1.2.0 store UI / per-frame patches (from the re-decompiled source scan).
            Patch(typeof(StoreUIManager), nameof(StoreUIManager.CheckRaycast), Type.EmptyTypes,
                nameof(Hooks.StoreHoverPrefix), nameof(Hooks.StoreHoverPostfix), true);
            Patch(typeof(StoreUIManager), nameof(StoreUIManager.IsTextFieldFocused), Type.EmptyTypes,
                nameof(Hooks.TextFieldFocusPrefix), nameof(Hooks.TextFieldFocusPostfix), true);
            Patch(typeof(AdvancedContentSizeFitter), "Update", Type.EmptyTypes,
                nameof(Hooks.SizeFitterPrefix), null, true);

            // Settings UI polish: show the current value next to every mod-config slider.
            try
            {
                ConfigValueLabels.Install(harmony);
            }
            catch (Exception ex)
            {
                Log.Warning($"滑条数值显示安装失败: {ex.Message}");
            }

            // Night settlement: defer redundant inventory refreshes, silence purely
            // visual work inside the nightly chemical simulation, and (while the
            // diagnostic mode is running) time every vanilla settlement stage.
            Patch(typeof(ChemicalHelper), "RunNightlyChemicalSimulation", Type.EmptyTypes,
                nameof(Hooks.NightSimPrefix), nameof(Hooks.NightSimPostfix), true);

            try
            {
                global::XGDragFixer.NightValidateBatch.Install(harmony);
            }
            catch (Exception ex)
            {
                Log.Warning($"结算刷新合并安装失败: {ex.Message}");
            }

            try
            {
                NightSimQuiet.Install(harmony);
            }
            catch (Exception ex)
            {
                Log.Warning($"夜间界面静默安装失败: {ex.Message}");
            }

            try
            {
                NightBreakdown.Install(harmony);
            }
            catch (Exception ex)
            {
                Log.Warning($"结算诊断安装失败: {ex.Message}");
            }

            try
            {
                ChemBreakdown.Install(harmony);
            }
            catch (Exception ex)
            {
                Log.Warning($"化学细分诊断安装失败: {ex.Message}");
            }

            try
            {
                ChemTagCache.Install(harmony);
            }
            catch (Exception ex)
            {
                Log.Warning($"化学标签缓存安装失败: {ex.Message}");
            }

            try
            {
                XGDragFixer.StartupFaultGuard.Install(harmony);
            }
            catch (Exception ex)
            {
                Log.Warning($"界面未就绪兜底安装失败: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Log.Warning($"补丁安装异常: {ex}");
        }

        Log.Log($"补丁安装完成：成功 {ok} 个，失败/缺失 {failed} 个");

        if (XGDragFixer.PerfDiagnostics.Enabled)
        {
            XGDragFixer.PerfDiagnostics.ShowStartupNotice();
        }
    }

    internal static ModLog? Log { get; private set; }

    internal static ConfigEntry<bool>? FixGhostDrag { get; private set; }

    internal static ConfigEntry<bool>? FastSlotFind { get; private set; }

    internal static ConfigEntry<bool>? FastContainerOpen { get; private set; }

    internal static ConfigEntry<bool>? SkipLayoutMark { get; private set; }

    internal static ConfigEntry<int>? StickyLayoutFrames { get; private set; }

    internal static ConfigEntry<bool>? SkipMaskNotify { get; private set; }

    internal static ConfigEntry<bool>? SetTitleSkipValidate { get; private set; }

    internal static ConfigEntry<bool>? MiddleClickPreview { get; private set; }

    internal static ConfigEntry<bool>? PreviewHideTooltip { get; private set; }

    internal static ConfigEntry<bool>? TooltipCacheEnabled { get; private set; }

    internal static ConfigEntry<int>? TooltipCacheFrames { get; private set; }

    internal static ConfigEntry<bool>? TooltipIdleSkip { get; private set; }

    internal static ConfigEntry<int>? TooltipIdleFrames { get; private set; }

    internal static ConfigEntry<bool>? RaycastFrameCache { get; private set; }

    internal static ConfigEntry<bool>? ContextMenuSkip { get; private set; }

    internal static ConfigEntry<bool>? AgingBatch { get; private set; }

    internal static ConfigEntry<bool>? NightValidateBatchEnabled { get; private set; }

    internal static ConfigEntry<bool>? NightSimUIQuiet { get; private set; }

    internal static ConfigEntry<bool>? ChemTagCacheEnabled { get; private set; }

    internal static ConfigEntry<bool>? MachineModuleSkip { get; private set; }

    internal static ConfigEntry<bool>? SpriteRefreshGuard { get; private set; }

    internal static ConfigEntry<bool>? SlotIndexDirect { get; private set; }

    internal static ConfigEntry<bool>? SkipStoreHoverRaycast { get; private set; }

    internal static ConfigEntry<int>? StoreHoverIdleFrames { get; private set; }

    internal static ConfigEntry<bool>? TextFieldFocusCache { get; private set; }

    internal static ConfigEntry<bool>? SizeFitterGuard { get; private set; }

    internal static ConfigEntry<bool>? StartupFaultGuard { get; private set; }

    internal static ConfigEntry<bool>? LoadFocusBoost { get; private set; }

    internal static ConfigEntry<bool>? PerfDiagnosticsEnabled { get; private set; }

    internal static ConfigEntry<int>? PerfReportSeconds { get; private set; }

    internal static ConfigEntry<ABMode>? PerfABMode { get; private set; }

    internal static ConfigEntry<string>? PerfABKey { get; private set; }

    internal static ConfigEntry<int>? PerfAutoABSeconds { get; private set; }
}

internal sealed class ModRunner : MonoBehaviour
{
    internal Core? Mod;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadFocusBoost.NotifySceneLoaded();
    }

    private void Update()
    {
        Mod?.OnUpdate();
        XGDragFixer.PerfDiagnostics.Frame();
        Toast.Update();
    }
}
