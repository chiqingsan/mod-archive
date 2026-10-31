using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace BatchSelectFinal;

/// <summary>
/// 「按键拖拽多选框」：让原版物品批量框选可以从任意位置开始——按住触发键（默认空格）
/// 后拖动鼠标，即使起点压在已有物品的格子上也能拉出选择框。
/// 触发键可在 游戏设置 → 按键 →「修改触发拖拽的快捷键」中修改。
/// </summary>
public sealed class BatchSelectMod : IMod
{
    internal const string HarmonyId = "chiqingsan.BatchSelect";
    internal const string TriggerBindingId = "batch_select";
    internal const string TriggerOptionId = "key_batch_select";
    internal const string ConfigKey = "TriggerKey";
    internal const string DisplayNameText = "修改触发拖拽的快捷键";
    internal const string TooltipText =
        "按住此键后拖动鼠标，可以从任意位置（包括已有物品的格子）拉出批量选择框。默认 Space";

    internal static ModLog Log = null!;
    internal static ConfigEntry<string>? TriggerKeyConfig;
    internal static KeyCode TriggerKey { get; private set; } = KeyCode.Space;

    private HarmonyLib.Harmony? _harmony;

    public void Init(ModManifest manifest)
    {
        Log = new ModLog(manifest);

        try
        {
            LoadTriggerKey(manifest.ID);
        }
        catch (Exception ex)
        {
            Log.Warning("触发键初始化失败: " + ex);
        }

        try
        {
            _harmony = new HarmonyLib.Harmony(HarmonyId);
            Hooks.Install(_harmony);
        }
        catch (Exception ex)
        {
            Log.Error("补丁安装失败: " + ex);
        }
    }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {
        try
        {
            _harmony?.UnpatchAll(HarmonyId);
        }
        catch (Exception ex)
        {
            Log.Warning("卸载补丁失败: " + ex.Message);
        }

        _harmony = null;
    }

    private static void LoadTriggerKey(string modId)
    {
        var config = ModConfig.For(modId);
        TriggerKeyConfig = config.Define(ConfigKey, "Space", "批量选择",
            () => "触发键（备份）",
            null,
            () => "请到 游戏设置 → 按键 中修改触发键。这里是备份值，手动编辑后需重启游戏才生效。");

        KeyCode key = KeyCode.Space;
        string raw = TriggerKeyConfig.Value;
        if (!string.IsNullOrWhiteSpace(raw))
        {
            if (Enum.TryParse(raw.Trim(), true, out KeyCode parsed) && IsValidTriggerKey(parsed))
            {
                key = parsed;
            }
            else
            {
                Log.Warning("无法解析已保存的触发键 '" + raw + "'，使用默认 Space");
            }
        }

        ApplyTriggerKey(key);
        Hooks.TryRegisterOption();
    }

    internal static bool IsValidTriggerKey(KeyCode key)
    {
        if (key == KeyCode.None)
        {
            return true; // 允许设为 None 以停用功能
        }

        int value = (int)key;
        if (value >= (int)KeyCode.Mouse0 && value <= (int)KeyCode.Mouse6)
        {
            return false; // 触发键必须是键盘键，避免和左键框选冲突
        }

        return key != KeyCode.Escape; // Escape 是游戏按键设置的保留键
    }

    internal static void ApplyTriggerKey(KeyCode key)
    {
        if (!IsValidTriggerKey(key))
        {
            key = KeyCode.Space;
        }

        TriggerKey = key;
        KeyBindHelper.Set(TriggerBindingId, key);

        try
        {
            if (TriggerKeyConfig != null && TriggerKeyConfig.Value != key.ToString())
            {
                TriggerKeyConfig.Set(key.ToString());
            }
        }
        catch (Exception ex)
        {
            Log.Warning("保存触发键失败: " + ex.Message);
        }
    }

    internal static bool IsTriggerHeld()
    {
        // SuppressHotkeys 在设置界面捕获按键时为 true，避免捕获时误触
        return KeyBindHelper.GetKey(TriggerBindingId);
    }
}

internal static class Hooks
{
    private static readonly FieldInfo? StateField = AccessTools.Field(typeof(ItemMultiSelectHandler), "state");
    private static readonly FieldInfo? PressPositionField = AccessTools.Field(typeof(ItemMultiSelectHandler), "pressPosition");
    private static readonly FieldInfo? BandWindowField = AccessTools.Field(typeof(ItemMultiSelectHandler), "bandWindow");
    private static readonly FieldInfo? BandInventoryField = AccessTools.Field(typeof(ItemMultiSelectHandler), "bandInventory");
    private static readonly object? PendingBandState =
        StateField == null ? null : Enum.ToObject(StateField.FieldType, 1);

    private static readonly Dictionary<Type, MethodInfo?> ResetMethods = new();

    internal static void Install(HarmonyLib.Harmony harmony)
    {
        // 原版多选处理器：按住触发键 + 左键、且起点压在物品上时，手动进入待框选状态。
        Patch(harmony, typeof(ItemMultiSelectHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
            nameof(MultiSelectPrefix), null);

        // 以下处理器会在同一帧对左键按下做出反应（拖物品、单击打开/激活、双击、选中），
        // 按住触发键时必须跳过它们，否则框选会被打断。
        Patch(harmony, typeof(ItemMouseDragHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
            nameof(SuppressPrefix), null);
        Patch(harmony, typeof(ItemMouseDoubleClickHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
            nameof(SuppressPrefix), null);
        Patch(harmony, typeof(ItemDefaultActivateHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
            nameof(SuppressPrefix), null);
        Patch(harmony, typeof(ItemSelectHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
            nameof(SuppressPrefix), null);

        // 把触发键追加到 游戏设置 → 按键。
        Patch(harmony, typeof(GameOptionRegistry), "EnsureVanilla", Type.EmptyTypes,
            null, nameof(EnsureVanillaPostfix));
    }

    private static void Patch(HarmonyLib.Harmony harmony, Type type, string methodName, Type[] argumentTypes,
        string? prefixName, string? postfixName)
    {
        try
        {
            MethodInfo? original = AccessTools.DeclaredMethod(type, methodName, argumentTypes);
            if (original == null)
            {
                BatchSelectMod.Log.Warning($"未找到补丁目标 {type.Name}.{methodName}");
                return;
            }

            HarmonyMethod? prefix = prefixName == null
                ? null
                : new HarmonyMethod(AccessTools.Method(typeof(Hooks), prefixName));
            HarmonyMethod? postfix = postfixName == null
                ? null
                : new HarmonyMethod(AccessTools.Method(typeof(Hooks), postfixName));

            harmony.Patch(original, prefix, postfix);
        }
        catch (Exception ex)
        {
            BatchSelectMod.Log.Warning($"修补 {type.Name}.{methodName} 失败: {ex.Message}");
        }
    }

    /// <summary>按下瞬间是否应由本模组接管：触发键按住、左键按下、非钉住模式、非商店 UI、非 UI 拖拽。</summary>
    private static bool ShouldTakeOver(ISet<KeyCode>? keysPressed)
    {
        if (!BatchSelectMod.IsTriggerHeld())
        {
            return false;
        }

        if (keysPressed != null && !keysPressed.Contains(KeyCode.Mouse0))
        {
            return false;
        }

        if (ItemPinHelper.BlocksClicks)
        {
            return false;
        }

        StoreUIManager? storeUI = StoreUIManager.Instance;
        if (storeUI != null && storeUI.hoveringStoreUI)
        {
            return false;
        }

        return !UIDragHandler.isDraggingAnyUI;
    }

    private static bool MultiSelectPrefix(ItemMultiSelectHandler __instance, ISet<KeyCode> keysPressed)
    {
        if (!ShouldTakeOver(keysPressed))
        {
            return true;
        }

        if (!keysPressed.Contains(__instance.key))
        {
            return true;
        }

        Vector3 mouse = Input.mousePosition;

        // 起点是空的：原版逻辑本来就能从这里拉框，交回原版（也保留原版对拖拽表面的判断）。
        if (RenderHandler.RaycastElement<GameItemElement>(mouse) == null)
        {
            return true;
        }

        // 起点压在物品上：原版会因为“射线命中物品”而完全不进入框选状态，这里手动补上。
        try
        {
            if (__instance.HasSelection)
            {
                __instance.Cancel();
            }

            PressPositionField?.SetValue(__instance, mouse);
            BandInventoryField?.SetValue(__instance, RenderHandler.RaycastFindParentElement<GameInventory>(mouse));
            BandWindowField?.SetValue(__instance, RenderHandler.RaycastFindParentElement<PixelWindow>(mouse));

            if (StateField == null || PendingBandState == null)
            {
                BatchSelectMod.Log.Warning("找不到 ItemMultiSelectHandler.state 字段，回退原版行为");
                return true;
            }

            StateField.SetValue(__instance, PendingBandState);
            return false;
        }
        catch (Exception ex)
        {
            BatchSelectMod.Log.Warning("从物品上开始框选失败，回退原版: " + ex.Message);
            return true;
        }
    }

    private static bool SuppressPrefix(InputActionHandler __instance, ISet<KeyCode> keysPressed)
    {
        if (!ShouldTakeOver(keysPressed))
        {
            return true;
        }

        // 清掉这些处理器上一次点击留下的状态，避免它们在松开左键时误触发
        try
        {
            ResetHandler(__instance);
        }
        catch (Exception ex)
        {
            BatchSelectMod.Log.Warning($"重置 {__instance.GetType().Name} 状态失败: {ex.Message}");
        }

        return false;
    }

    private static void ResetHandler(InputActionHandler handler)
    {
        Type type = handler.GetType();
        if (!ResetMethods.TryGetValue(type, out MethodInfo? reset))
        {
            reset = AccessTools.DeclaredMethod(type, "OnEventReset", Type.EmptyTypes)
                    ?? AccessTools.Method(type, "OnEventReset", Type.EmptyTypes);
            ResetMethods[type] = reset;
        }

        reset?.Invoke(handler, null);
    }

    private static void EnsureVanillaPostfix()
    {
        TryRegisterOption();
    }

    internal static void TryRegisterOption()
    {
        try
        {
            if (GameOptionRegistry.Find(BatchSelectMod.TriggerOptionId) != null)
            {
                return;
            }

            // 只有 vanilla 设置界面已经建立按键分区时才追加，避免生成未本地化的分区。
            if (GameOptionRegistry.FindSection(GameOptionRegistry.SECTION_KEYBIND) == null)
            {
                return;
            }

            GameOptionRegistry.Add(GameOptionRegistry.SECTION_KEYBIND, GameOptionRegistry.KeyBind(
                BatchSelectMod.TriggerOptionId,
                () => BatchSelectMod.DisplayNameText,
                () => BatchSelectMod.TriggerKey,
                key => BatchSelectMod.ApplyTriggerKey(key),
                () => BatchSelectMod.TooltipText));
        }
        catch (Exception ex)
        {
            BatchSelectMod.Log.Warning("注册按键选项失败: " + ex.Message);
        }
    }
}
