using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FastLoad;

/// <summary>
/// 读档提速：预览只解析 networkUpgrade、读档用精简副本、延迟解码售出库存，
/// 并记录各阶段耗时用于量化收益。改动只发生在读档路径，失败回退原版。
/// </summary>
public sealed class Core : IMod
{
    internal const string HarmonyId = "chiqingsan.FastLoad";

    private GameObject? _runner;
    private HarmonyLib.Harmony? _harmony;
    private bool _patchAttempted;

    public void Init(ModManifest manifest)
    {
        Log = new ModLog(manifest);

        try
        {
            var config = ModConfig.For(manifest.ID);
            TimingLog = config.Define("TimingLog", true, "诊断",
                () => "记录读档各阶段耗时（S0 打点）");
            SkipPreLoadStore = config.Define("SkipPreLoadStore", true, "提速",
                () => "读档预览只解析 networkUpgrade，跳过整份存档反序列化");
            SlimLoadFile = config.Define("SlimLoadFile", true, "提速",
                () => "读档改用精简副本（去掉约 1500 万字符的库存 JSON，解码时按需补回）");
            LazySoldDecode = config.Define("LazySoldDecode", true, "提速",
                () => "延迟解码当天售出库存（约 368 件物品，用到时再解码）");
            FastDecode = config.Define("FastDecode", true, "提速",
                () => "批量解码时跳过逐件触发的整库校验，解码结束后统一校验一次（消除 O(n²)）");
            SkipUnparentedValidate = config.Define("SkipUnparentedValidate", true, "提速",
                () => "解码期间尚未挂到库存的物品只同步形状、跳过渲染校验（挂载时统一补）");
            SkipCtorValidate = config.Define("SkipCtorValidate", true, "提速",
                () => "构造物品时不重复做渲染校验（挂载时统一补）");
            CacheItemShapes = config.Define("CacheItemShapes", true, "提速",
                () => "缓存物品形状构建结果（同类物品只做一次像素统计）");
            CacheLocalization = config.Define("CacheLocalization", true, "提速",
                () => "缓存存档缺翻译的修复结果，避免重复查表与重复日志");
            DumpStats = config.Define("DumpStats", false, "诊断",
                () => "在日志中输出精简存档的构建统计");
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

        _runner = new GameObject("FastLoad");
        Object.DontDestroyOnLoad(_runner);
        _runner.AddComponent<ModRunner>().Mod = this;

        // EmporiumEntry.Start 可能早于首个 Update，补丁必须尽早安装。
        TryInstall();
    }

    internal void OnUpdate()
    {
        if (!_patchAttempted)
        {
            TryInstall();
        }
    }

    private void TryInstall()
    {
        _patchAttempted = true;
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
                Log?.Warning($"补丁失败 {type!.Name}.{methodName}: {ex.Message}");
            }
        }

        // S0：读档全过程与各阶段耗时
        Patch(typeof(EmporiumEntry), "Start", Type.EmptyTypes,
            nameof(Hooks.StartPrefix), nameof(Hooks.StartPostfix), false);
        Patch(typeof(PlayerStore), nameof(PlayerStore.PreLoadStore), Type.EmptyTypes,
            nameof(Hooks.PreLoadStorePrefix), nameof(Hooks.PreLoadStorePostfix), false);
        Patch(typeof(SaveFiles), nameof(SaveFiles.ResolveLoadFile), new[] { typeof(NewGameData) },
            null, nameof(Hooks.ResolveLoadFilePostfix), true);
        Patch(typeof(PlayerStore), nameof(PlayerStore.LoadGame), Type.EmptyTypes,
            nameof(Hooks.LoadGamePrefix), nameof(Hooks.LoadGamePostfix), false);
        Patch(typeof(SaveFiles), nameof(SaveFiles.WithIORetry), new[] { typeof(Action) },
            nameof(Hooks.WithIORetryPrefix), nameof(Hooks.WithIORetryPostfix), true);
        Patch(typeof(SaveManager), nameof(SaveManager.DecodeNodes), new[] { typeof(List<SaveItemNode>) },
            nameof(Hooks.DecodeNodesPrefix), nameof(Hooks.DecodeNodesPostfix), true);
        Patch(typeof(PlayerStore), nameof(PlayerStore.DecodeSaveItem), new[] { typeof(string), typeof(GameInventory) },
            nameof(Hooks.DecodeSaveItemPrefix), nameof(Hooks.DecodeSaveItemPostfix), false);

        // S1-④：批量解码期间跳过逐件触发的整库校验（O(n²) → O(n)），并统计单件构造开销
        Patch(typeof(GameItem), "ValidateShapeState", Type.EmptyTypes,
            nameof(Hooks.ValidateShapeStatePrefix), null, true);
        try
        {
            var itemCtor = AccessTools.Constructor(typeof(GameItemElement),
                new[] { typeof(string), typeof(string), typeof(string), typeof(PixelWindow) });
            if (itemCtor == null)
            {
                Log?.Log("[兼容] 未找到 GameItemElement 构造器，跳过构造耗时打点");
            }
            else
            {
                harmony.Patch(itemCtor,
                    new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(Hooks.ItemElementCtorPrefix))),
                    new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(Hooks.ItemElementCtorPostfix))));
                ok++;
            }
        }
        catch (Exception ex)
        {
            failed++;
            Log?.Warning($"补丁失败 GameItemElement..ctor: {ex.Message}");
        }

        // S1：延迟解码售出库存时，用到前先补解码（开闸处理昨天卖货、存盘、UI 取货）
        Patch(typeof(EmporiumEntry), nameof(EmporiumEntry.GetAllItems), Type.EmptyTypes,
            nameof(Hooks.GetAllItemsPrefix), null, true);
        Patch(typeof(PlayerStore), nameof(PlayerStore.SaveGame), Type.EmptyTypes,
            nameof(Hooks.SaveGamePrefix), null, true);
        Patch(typeof(PlayerStore), nameof(PlayerStore.OnShutterOpened), Type.EmptyTypes,
            nameof(Hooks.OnShutterOpenedPrefix), null, true);
        Patch(typeof(EmporiumEntry), nameof(EmporiumEntry.GetItemFromSoldElement), new[] { typeof(int) },
            nameof(Hooks.GetItemFromSoldElementPrefix), null, true);

        // A：细粒度打点；B：形状/本地化缓存、构造期与未挂载期校验跳过
        void PatchMethod(MethodInfo? original, string? prefix, string? postfix, bool optional)
        {
            if (original == null)
            {
                if (optional)
                {
                    Log?.Log("[兼容] 未找到细粒度目标，跳过该子功能");
                }
                else
                {
                    failed++;
                    Log?.Warning("未找到细粒度目标");
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
                Log?.Warning($"细粒度补丁失败 {original.DeclaringType?.Name}.{original.Name}: {ex.Message}");
            }
        }

        // A-① 通用计时：解码链路上最可能的耗时点
        foreach (var m in new MethodInfo?[]
                 {
                     typeof(RenderHandler).GetMethod("LoadFromAtlas", new[] { typeof(string), typeof(string) }),
                     typeof(GameItem).GetMethod("SetShape"),
                     typeof(GameItem).GetMethod("SetSpriteAndShape", new[] { typeof(string), typeof(string) }),
                     typeof(ItemDirectory).GetMethod("CreateEmptyItem", new[] { typeof(string) }),
                     typeof(GeneralHelper).GetMethod("SetItemOwned", new[] { typeof(GameItem), typeof(bool) }),
                     typeof(PermitHelper).GetMethod("OnNewItemCreated", new[] { typeof(GameItem) }),
                     typeof(GameItem).GetMethod("EnableTag", new[] { typeof(string), typeof(bool) }),
                     typeof(GameItem).GetMethod("DisableTag", new[] { typeof(string), typeof(bool) }),
                     typeof(GridShapeBuilder).GetMethod("Build", Type.EmptyTypes),
                     typeof(TagSystem).GetMethod("Clone", Type.EmptyTypes),
                 })
        {
            PatchMethod(m, nameof(Hooks.SubTimerPrefix), nameof(Hooks.SubTimerPostfix), true);
        }

        // A-③ DirectoryMaster.Item 单独打点：额外记录最慢物品的 identifier
        PatchMethod(typeof(DirectoryMaster).GetMethod("Item", new[] { typeof(string), typeof(bool) }),
            nameof(Hooks.ItemTimerPrefix), nameof(Hooks.ItemTimerPostfix), true);

        // A-④ ModifyTag(string, Action<TagState>, bool)：用反射按参数特征取，避免引用内部类型
        try
        {
            foreach (var m in typeof(GameItem).GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (m.Name != "ModifyTag")
                {
                    continue;
                }

                var ps = m.GetParameters();
                if (ps.Length == 3 && ps[0].ParameterType == typeof(string)
                    && ps[1].ParameterType.Name.StartsWith("Action", StringComparison.Ordinal)
                    && ps[2].ParameterType == typeof(bool))
                {
                    PatchMethod(m, nameof(Hooks.SubTimerPrefix), nameof(Hooks.SubTimerPostfix), true);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            failed++;
            Log?.Warning($"ModifyTag 计时补丁失败: {ex.Message}");
        }

        // A-② TreeNodeRender 三个实例化重载分开计时（避免引用未公开的 PixelElement 类型）
        try
        {
            foreach (var m in typeof(TreeNodeRender).GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (m.Name == "Instantiate" && m.GetParameters().Length == 0)
                {
                    PatchMethod(m, nameof(Hooks.SubTimerPrefix), nameof(Hooks.SubTimerPostfix), true);
                }
                else if (m.Name == "InstantiateText" && m.GetParameters().Length == 0)
                {
                    PatchMethod(m, nameof(Hooks.SubTimerPrefix), nameof(Hooks.SubTimerPostfix), true);
                }
                else if (m.Name == "Instantiate" && m.GetParameters().Length == 2
                         && m.GetParameters()[1].ParameterType == typeof(string)
                         && m.GetParameters()[0].ParameterType.Name == "PixelElement")
                {
                    PatchMethod(m, nameof(Hooks.SubTimerPrefix), nameof(Hooks.SubTimerPostfix), true);
                }
            }
        }
        catch (Exception ex)
        {
            failed++;
            Log?.Warning($"TreeNodeRender 计时补丁失败: {ex.Message}");
        }

        // B-① 形状缓存 + 计时
        PatchMethod(typeof(GameItem).GetMethod("BuildShapeFromSprite", new[] { typeof(string), typeof(string) }),
            nameof(Hooks.BuildShapeFromSpritePrefix), nameof(Hooks.BuildShapeFromSpritePostfix), true);

        // B-② 本地化缓存 + 计时
        PatchMethod(typeof(LocHelper).GetMethod("RelocalizeIfMissing", new[] { typeof(string) }),
            nameof(Hooks.RelocalizePrefix), nameof(Hooks.RelocalizePostfix), true);

        // B-③ 构造期/未挂载期跳过渲染校验 + 计时
        PatchMethod(typeof(GameItemElement).GetMethod("Validate", Type.EmptyTypes),
            nameof(Hooks.ItemValidatePrefix), nameof(Hooks.ItemValidatePostfix), true);

        Log?.Log($"补丁安装完成：成功 {ok} 个，失败/缺失 {failed} 个");
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
        Hooks.ResetAll();
    }

    internal static ModLog? Log { get; private set; }

    internal static ConfigEntry<bool>? TimingLog { get; private set; }

    internal static ConfigEntry<bool>? SkipPreLoadStore { get; private set; }

    internal static ConfigEntry<bool>? SlimLoadFile { get; private set; }

    internal static ConfigEntry<bool>? LazySoldDecode { get; private set; }

    internal static ConfigEntry<bool>? FastDecode { get; private set; }

    internal static ConfigEntry<bool>? SkipUnparentedValidate { get; private set; }

    internal static ConfigEntry<bool>? SkipCtorValidate { get; private set; }

    internal static ConfigEntry<bool>? CacheItemShapes { get; private set; }

    internal static ConfigEntry<bool>? CacheLocalization { get; private set; }

    internal static ConfigEntry<bool>? DumpStats { get; private set; }
}

internal sealed class ModRunner : MonoBehaviour
{
    internal Core? Mod;

    private void Update()
    {
        Mod?.OnUpdate();
    }
}
