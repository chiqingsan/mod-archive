using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace XGDragFixer;

/// <summary>
/// 拖拽与容器优化（行为与游戏内实现对齐）:
/// ghost-drag fix, container cold start, scoped layout suppression while a container
/// opens, middle-click preview and the 30-frame tooltip cache.
/// </summary>
public sealed class Core : IMod
{
    internal const string HarmonyId = "chiqingsan.BetterPerformance";

    private GameObject? _runner;
    private HarmonyLib.Harmony? _harmony;
    private bool _installAttempted;

    public void Init(ModManifest manifest)
    {
        Log = new ModLog(manifest);

        try
        {
            var config = ModConfig.For(manifest.ID);
            FixGhostDrag = config.Define("FixGhostDrag", true, "拖拽",
                () => "修复点击后偶发粘住拖拽");
            Fix4 = config.Define("Fix4Enabled", true, "拖拽",
                () => "大容器空位搜索加速");
            FastContainerOpen = config.Define("FastContainerOpen", true, "容器",
                () => "首次打开容器时提前挂载窗口");
            SkipLayoutMark = config.Define("SkipContainerLayoutMark", true, "容器",
                () => "打开容器时跳过冗余布局重建");
            StickyLayoutFrames = config.Define("SkipLayoutMarkFrames", 2, "容器",
                () => "打开后额外跳过的帧数", 0, 8);
            MiddleClickPreview = config.Define("MiddleClickPreview", true, "中键预览",
                () => "按住中键悬停容器物品快速预览内容");
            PreviewHideTooltip = config.Define("PreviewHideTooltip", true, "中键预览",
                () => "预览内容时隐藏物品提示框");
            TooltipCacheEnabled = config.Define("TooltipCache", true, "提示框",
                () => "缓存悬停提示框，避免每帧重算");
            TooltipCacheFrames = config.Define("TooltipCacheFrames", 30, "提示框",
                () => "提示框缓存帧数（原版为 30）", 1, 120);

            AgingCollapse = config.Define("AgingCollapse", true, "性能优化",
                () => "陈化机按天循环折叠为一次结算");
            ModuleRecomputeDedup = config.Define("ModuleRecomputeDedup", true, "性能优化",
                () => "模块动作触发时效果只结算一次");
            DiagnosticsEnabled = config.Define("DiagnosticsEnabled", false, "诊断",
                () => "记录性能与异常诊断（每10秒汇总，开关立即生效）");
            GridOccupancyShortcut = config.Define("GridOccupancyShortcut", true, "性能优化",
                () => "用占用位图加速容器空位判断");
            WindowLeakFix = config.Define("WindowLeakFix", true, "容器",
                () => "销毁容器物品时释放其隐藏窗口（原版只隐藏）");
            BlendBatch = config.Define("BlendBatch", true, "性能优化",
                () => "搅拌机混合液体时合并批次写入（每液体 1 次代替 10 次）");
            PressBatch = config.Define("PressBatch", true, "性能优化",
                () => "化学精炼机压片时合并形状校验（每步收尾校验一次）");
        }
        catch (Exception ex)
        {
            Log?.Warning($"配置初始化失败: {ex.Message}");
        }
    }

    public void OnEnable()
    {
        if (_runner != null)
        {
            return;
        }

        _installAttempted = false;
        _runner = new GameObject("BetterPerformance");
        Object.DontDestroyOnLoad(_runner);
        _runner.AddComponent<ModRunner>().Mod = this;
    }

    internal void OnUpdate()
    {
        Diagnostics.Tick();
        // The mod settings panel is reachable from the main menu, before the save world
        // exists. Patch its renderer once the main menu UI is alive.
        if (!ConfigValueLabels.Installed
            && (ModUIManager.Instance != null || MainMenuUIController.Instance != null))
        {
            try
            {
                ConfigValueLabels.Install(_harmony ??= new HarmonyLib.Harmony(HarmonyId));
            }
            catch (Exception ex)
            {
                Log?.Warning($"设置面板补丁安装失败: {ex.Message}");
            }
        }

        // OnEnable runs inside ModLoader.Awake, before the game world and its assets
        // are ready; patch once the save world exists instead.
        if (!_installAttempted)
        {
            if (!PlayerStore.IsInstanceExist() || PlayerStore.Instance == null || EmporiumEntry.Instance == null)
            {
                return;
            }

            _installAttempted = true;
            InstallPatches();
        }

        Diagnostics.ReportTagPatchOwners();
        HoverPeek.Tick();
    }

    public void OnDisable()
    {
        Diagnostics.Flush();
        Diagnostics.Reset();
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
        XGDragFixer.RefVisual.Reset();
        XGDragFixer.RefQueries.Reset();
        ConfigValueLabels.Reset();
    }

    private void InstallPatches()
    {
        var harmony = _harmony ??= new HarmonyLib.Harmony(HarmonyId);
        var ok = 0;
        var failed = 0;

        void Patch(Type? type, string methodName, Type[] argumentTypes, string? prefix, string? postfix, bool optional,
            string? finalizer = null)
        {
            var original = type == null ? null : AccessTools.Method(type, methodName, argumentTypes);
            if (original == null)
            {
                if (optional)
                {
                    Log?.Log($"[兼容] 未找到 {type?.FullName}.{methodName}，跳过该子功能");
                }
                else
                {
                    failed++;
                    Log?.Warning($"未找到补丁目标 {type?.FullName}.{methodName}");
                }

                return;
            }

            try
            {
                var pre = prefix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(Hooks), prefix));
                var post = postfix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(Hooks), postfix));
                var final = finalizer == null ? null : new HarmonyMethod(AccessTools.Method(typeof(Hooks), finalizer));
                if (pre == null && post == null && final == null)
                {
                    return;
                }

                harmony.Patch(original, pre, post, finalizer: final);
                ok++;
            }
            catch (Exception ex)
            {
                failed++;
                Log?.Warning($"补丁失败 {type!.Name}.{methodName}: {ex.Message}");
            }
        }

        // 1) 拖拽沾手修复
        Patch(typeof(ItemMouseDragHandler), "StartDrag", Type.EmptyTypes,
            nameof(Hooks.StartDragPrefix), null, false);

        // 1b) 大容器空位搜索加速（原版 Fix4，滚动库保持原版搜索）
        Patch(typeof(GameGridInventory), nameof(GameGridInventory.TryFindOneValidInventorySlot),
            new[] { typeof(GameItem), typeof(bool) }, nameof(Hooks.Fix4Prefix), null, true, nameof(Hooks.FindSlotFinalizer));
        Patch(typeof(GameGridScrollableInventory), nameof(GameGridScrollableInventory.TryFindOneValidInventorySlot),
            new[] { typeof(GameItem), typeof(bool) }, nameof(Hooks.Fix4ScrollablePrefix), null, true, nameof(Hooks.FindSlotFinalizer));

        // 2) 容器冷启动 + 打开后的布局抑制
        Patch(typeof(PixelWindow), nameof(PixelWindow.Show),
            new[] { typeof(Vector2), typeof(Vector2), typeof(bool) },
            nameof(Hooks.ColdStartPrefix), null, false, nameof(Hooks.ShowFinalizer));

        // 3) 布局通知抑制；遮罩生命周期保持原版，不挂接全局 MaskUtilities。
        Patch(typeof(LayoutRebuilder), nameof(LayoutRebuilder.MarkLayoutForRebuild),
            new[] { typeof(RectTransform) }, nameof(Hooks.MarkLayoutPrefix), null, true);

        // 4) 中键预览：预览期间不弹提示框
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

        // 5) 提示框 30 帧缓存
        Patch(typeof(GameItemElement), nameof(GameItemElement.GetTooltip), Type.EmptyTypes,
            nameof(Hooks.GetTooltipPrefix), nameof(Hooks.GetTooltipPostfix), true);
        Patch(typeof(GameItemElement), nameof(GameItemElement.GetTooltipAdvanced), Type.EmptyTypes,
            nameof(Hooks.GetAdvancedTooltipPrefix), nameof(Hooks.GetAdvancedTooltipPostfix), true);

        // 6) 逻辑优化（陈化折叠/模块去重/空位位图）
        try
        {
            XGDragFixer.RefVisual.Install(harmony);
        }
        catch (Exception ex)
        {
            Log?.Warning($"视觉与机器结算优化安装失败: {ex.Message}");
        }

        try
        {
            XGDragFixer.RefQueries.Install(harmony);
        }
        catch (Exception ex)
        {
            Log?.Warning($"查询优化安装失败: {ex.Message}");
        }

        try
        {
            ConfigValueLabels.Install(harmony);
        }
        catch (Exception ex)
        {
            Log?.Warning($"设置面板补丁安装失败: {ex.Message}");
        }

        try
        {
            ContainerFixes.Install(harmony);
        }
        catch (Exception ex)
        {
            Log?.Warning($"容器修复安装失败: {ex.Message}");
        }

        try
        {
            ChemFinisherFix.Install(harmony);
        }
        catch (Exception ex)
        {
            Log?.Warning($"压片校验优化安装失败: {ex.Message}");
        }

        Log?.Log($"补丁安装完成：成功 {ok} 个，失败/缺失 {failed} 个");
    }

    internal static ModLog? Log { get; private set; }

    internal static ConfigEntry<bool>? FixGhostDrag { get; private set; }

    internal static ConfigEntry<bool>? Fix4 { get; private set; }

    internal static ConfigEntry<bool>? FastContainerOpen { get; private set; }

    internal static ConfigEntry<bool>? SkipLayoutMark { get; private set; }

    internal static ConfigEntry<int>? StickyLayoutFrames { get; private set; }

    internal static ConfigEntry<bool>? MiddleClickPreview { get; private set; }

    internal static ConfigEntry<bool>? PreviewHideTooltip { get; private set; }

    internal static ConfigEntry<bool>? TooltipCacheEnabled { get; private set; }

    internal static ConfigEntry<int>? TooltipCacheFrames { get; private set; }

    internal static ConfigEntry<bool>? AgingCollapse { get; private set; }

    internal static ConfigEntry<bool>? ModuleRecomputeDedup { get; private set; }

    internal static ConfigEntry<bool>? DiagnosticsEnabled { get; private set; }

    internal static ConfigEntry<bool>? GridOccupancyShortcut { get; private set; }

    internal static ConfigEntry<bool>? WindowLeakFix { get; private set; }

    internal static ConfigEntry<bool>? BlendBatch { get; private set; }

    internal static ConfigEntry<bool>? PressBatch { get; private set; }

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
        LayoutSuppressor.Reset();
        TooltipCache.Clear();
        XGDragFixer.RefVisual.Reset();
        XGDragFixer.RefQueries.Reset();
    }

    private void Update()
    {
        Mod?.OnUpdate();
    }
}
