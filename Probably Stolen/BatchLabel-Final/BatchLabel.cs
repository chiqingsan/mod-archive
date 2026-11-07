using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace BatchLabelFinal;

/// <summary>
/// 「标签机批量打标签」：装备标签机后按住触发键（默认空格），在物品上拖出选择框，
/// 松开左键后，框内与起点同类型的物品会批量复制起点物品当前显示的标签
/// （起点显示正品就贴正品标签，显示赝品就贴赝品标签）。
/// 触发键可在 游戏设置 → 按键 →「修改标签机批量打标签的快捷键」中修改，设为 None 可停用。
/// 框选期间游戏会临时取消标签机装备（否则框选处理器收不到鼠标事件），框选结束后本模组会自动把标签机装回去。
/// </summary>
public sealed class BatchLabelMod : IMod
{
    internal const string HarmonyId = "chiqingsan.BatchLabel";
    internal const string TriggerBindingId = "batch_label";
    internal const string TriggerOptionId = "key_batch_label";
    internal const string ConfigKey = "TriggerKey";
    internal const string DisplayNameText = "修改标签机批量打标签的快捷键";
    internal const string TooltipText =
        "装备标签机后，按住此键在物品上拖出选择框，松开左键后框内同类物品批量复制该物品当前显示的标签。默认 Space";
    internal const string MemoryConfigKey = "LabelMemory";
    internal const string MemorySection = "标签机批量打标签";

    internal static ModLog Log = null!;
    internal static ConfigEntry<string>? TriggerKeyConfig;
    internal static KeyCode TriggerKey { get; private set; } = KeyCode.Space;
    internal static ConfigEntry<string>? MemoryConfig;

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
            LoadLabelMemory(manifest.ID);
        }
        catch (Exception ex)
        {
            Log.Warning("标签记忆初始化失败: " + ex);
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
        TriggerKeyConfig = config.Define(ConfigKey, "Space", "标签机批量打标签",
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

    /// <summary>
    /// 标签记忆：玩家给某类物品打过的标签，按物品类型记在模组配置里（跨重启保留）。
    /// 走 ModConfig 而不是自己写文件，模组才不需要额外申请文件读写权限。
    /// </summary>
    private static void LoadLabelMemory(string modId)
    {
        var config = ModConfig.For(modId);
        MemoryConfig = config.Define(MemoryConfigKey, "", MemorySection,
            () => "已记住的标签（自动保存，请勿手动修改）",
            null,
            () => "每类物品上次打上的标签，装备标签机后右键该类物品即可重新贴上去。内容是自动保存的数据，手动修改会导致该项失效。");

        LabelMemory.Load(MemoryConfig.Value);
        if (LabelMemory.RememberedItemCount > 0)
        {
            Log.Log($"标签记忆：已载入 {LabelMemory.RememberedItemCount} 种物品");
        }
    }

    /// <summary>记忆有变化时写回模组配置；失败只记警告，不影响游戏。</summary>
    internal static void SaveLabelMemory()
    {
        try
        {
            if (MemoryConfig == null)
            {
                return;
            }

            string blob = LabelMemory.Serialize();
            if (string.Equals(MemoryConfig.Value, blob, StringComparison.Ordinal))
            {
                return;
            }

            MemoryConfig.Set(blob);
            MemoryConfig.Config.Save();
        }
        catch (Exception ex)
        {
            Log.Warning("保存标签记忆失败: " + ex.Message);
        }
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

    /// <summary>当前是否装备着标签机（能改假标签的白色标签机）。</summary>
    internal static bool IsLabelerEquipped()
    {
        ItemSelectHandler? handler = ItemSelectHandler.current;
        if (handler == null || !handler.IsEquipped)
        {
            return false;
        }

        GameItem? equipped = handler.GetEquippedItem();
        return equipped != null && equipped.IsTag(LabelerHelper.LABELER_TAG);
    }
}

internal static class Hooks
{
    private static readonly FieldInfo? StateField = AccessTools.Field(typeof(ItemMultiSelectHandler), "state");
    private static readonly FieldInfo? PressPositionField = AccessTools.Field(typeof(ItemMultiSelectHandler), "pressPosition");
    private static readonly FieldInfo? BandWindowField = AccessTools.Field(typeof(ItemMultiSelectHandler), "bandWindow");
    private static readonly FieldInfo? BandInventoryField = AccessTools.Field(typeof(ItemMultiSelectHandler), "bandInventory");
    private static readonly FieldInfo? SelectedItemsField = AccessTools.Field(typeof(ItemMultiSelectHandler), "selectedItems");
    private static readonly object? PendingBandState =
        StateField == null ? null : Enum.ToObject(StateField.FieldType, 1); // 原版 State.PendingBand = 1

    // 标签记忆要读「当前正在打标签的物品」和「这个下拉框对应的标签槽位」，两者都是私有字段。
    private static readonly FieldInfo? LabelerUiItemField = AccessTools.Field(typeof(LabelerUIManager), "curItem");
    private static readonly FieldInfo? LabelerElementFeatureField = AccessTools.Field(typeof(LabelerElement), "thisFeature");

    // 打开标签机 UI 时游戏会程序化设置下拉框的值，那不算玩家的选择，不该被记住。
    // 用帧号做守卫：即使 OpenUI 的收尾没执行到，守卫也会在下一帧自然过期，不会永久停止记录。
    private static int _labelerUiOpenFrame = -1;

    private static readonly Dictionary<Type, MethodInfo?> ResetMethods = new();

    // 按下左键起手框选时，本模组必须跳过 ItemSelectHandler 的点击处理并重置它，
    // 而 ItemSelectHandler.OnEventReset 正好就是「取消装备」。为了不把玩家装备着的标签机弄丢，
    // 这里记下这次接管和当时的装备物品，框选结束后再把标签机装回去。
    private static bool _armed;
    private static GameItemElement? _armedSource;
    private static GameItem? _armedLabeler;
    private static int _armedFrame;

    internal static void Install(HarmonyLib.Harmony harmony)
    {
        // 落点补丁必须先装上：装不上就不要再接管鼠标，否则会出现「框能拉出来但不会打标签」。
        if (!Patch(harmony, typeof(ItemMultiSelectHandler), "FinalizeBand", Type.EmptyTypes, null,
                nameof(FinalizeBandPostfix)))
        {
            BatchLabelMod.Log.Warning("无法挂接框选落点，批量打标签不启用");
            return;
        }

        // 原版框选只能从空白位置起手；装备标签机 + 按住触发键时允许从物品上起手。
        // ArmPostfix 是兜底：即使前缀被其它模组（如 BatchSelect）抢跑，按下时也能记录装备与起点。
        if (!Patch(harmony, typeof(ItemMultiSelectHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
                nameof(MultiSelectPrefix), nameof(ArmPostfix)))
        {
            BatchLabelMod.Log.Warning("无法挂接框选起手，从物品上起手的批量打标签将不可用");
        }

        // 同一帧内这些处理器也会响应左键，必须跳过，否则会打开标签机 / 拖动物品，框选被打断。
        Patch(harmony, typeof(ItemMouseDragHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
            nameof(SuppressPrefix), null);
        Patch(harmony, typeof(ItemMouseDoubleClickHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
            nameof(SuppressPrefix), null);
        Patch(harmony, typeof(ItemDefaultActivateHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
            nameof(SuppressPrefix), null);
        Patch(harmony, typeof(ItemSelectHandler), "OnEventPress", new[] { typeof(ISet<KeyCode>) },
            nameof(SuppressPrefix), null);

        // 兜底：点击/取消/其它路径没走到 FinalizeBand 时，也要把标签机装回去。
        Patch(harmony, typeof(ItemMultiSelectHandler), "OnEventRelease", new[] { typeof(ISet<KeyCode>) },
            null, nameof(ReleasePostfix));
        Patch(harmony, typeof(ItemMultiSelectHandler), "OnEventReset", Type.EmptyTypes,
            null, nameof(ResetPostfix));

        // 把触发键追加到 游戏设置 → 按键。
        Patch(harmony, typeof(GameOptionRegistry), "EnsureVanilla", Type.EmptyTypes,
            null, nameof(EnsureVanillaPostfix));

        // 标签记忆：玩家在标签机 UI 里选了什么就记住，关掉 UI 时写入模组配置。
        if (!Patch(harmony, typeof(LabelerUIManager), "OpenUI", new[] { typeof(GameItem) },
                nameof(LabelerOpenPrefix), nameof(LabelerOpenPostfix)))
        {
            BatchLabelMod.Log.Warning("无法挂接标签机界面，标签记忆不会记录新的选择");
        }

        Patch(harmony, typeof(LabelerElement), "OnDropdownValueChanged", Type.EmptyTypes,
            null, nameof(DropdownChangedPostfix));
        Patch(harmony, typeof(LabelerUIManager), "CloseUI", Type.EmptyTypes,
            null, nameof(LabelerClosePostfix));
    }

    private static bool Patch(HarmonyLib.Harmony harmony, Type type, string methodName, Type[] argumentTypes,
        string? prefixName, string? postfixName)
    {
        try
        {
            MethodInfo? original = AccessTools.DeclaredMethod(type, methodName, argumentTypes);
            if (original == null)
            {
                BatchLabelMod.Log.Warning($"未找到补丁目标 {type.Name}.{methodName}");
                return false;
            }

            HarmonyMethod? prefix = prefixName == null
                ? null
                : new HarmonyMethod(AccessTools.Method(typeof(Hooks), prefixName));
            HarmonyMethod? postfix = postfixName == null
                ? null
                : new HarmonyMethod(AccessTools.Method(typeof(Hooks), postfixName));

            harmony.Patch(original, prefix, postfix);
            return true;
        }
        catch (Exception ex)
        {
            BatchLabelMod.Log.Warning($"修补 {type.Name}.{methodName} 失败: {ex.Message}");
            return false;
        }
    }

    /// <summary>按下瞬间是否应由本模组接管：装备标签机、触发键按住、左键按下、非钉住模式、非商店 UI、非 UI 拖拽、起点是可打标签物品。</summary>
    private static bool ShouldTakeOver(ISet<KeyCode>? keysPressed)
    {
        if (keysPressed != null && !keysPressed.Contains(KeyCode.Mouse0))
        {
            return false;
        }

        return TryArm();
    }

    /// <summary>
    /// 记录这次接管：装备着的标签机、起点物品、起点是否可打标签。
    /// 必须在任何 ResetHandler 之前调用，否则 ItemSelectHandler 被重置后装备就查不到了。
    /// </summary>
    private static bool TryArm()
    {
        if (_armed)
        {
            // 同一次按下会在同一帧里被多个处理器询问，直接复用记录。
            // 跨帧时只有框选仍在进行（state != Idle）才认为记录有效；否则说明上次手势没走到
            // OnEventRelease / OnEventReset（例如别的模组提前 break 了输入更新循环），
            // 必须清掉这份陈旧记录，避免之后任意左键被误判成接管。
            if (Time.frameCount == _armedFrame)
            {
                return true;
            }

            ItemMultiSelectHandler? multiSelect = ItemMultiSelectHandler.current;
            if (multiSelect != null && multiSelect.HasSelection)
            {
                return true;
            }

            Disarm();
        }

        if (!BatchLabelMod.IsTriggerHeld() || !BatchLabelMod.IsLabelerEquipped())
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

        if (UIDragHandler.isDraggingAnyUI)
        {
            return false;
        }

        GameItemElement? element = FindLabelableItem(Input.mousePosition);
        if (element == null)
        {
            return false;
        }

        _armed = true;
        _armedSource = element;
        _armedLabeler = ItemSelectHandler.current?.GetEquippedItem();
        _armedFrame = Time.frameCount;
        return true;
    }

    /// <summary>结束接管：清掉记录，并把框选期间被取消的标签机装备恢复回去。</summary>
    private static void Disarm()
    {
        if (!_armed)
        {
            return;
        }

        GameItem? labeler = _armedLabeler;
        _armed = false;
        _armedSource = null;
        _armedLabeler = null;
        _armedFrame = 0;

        if (labeler == null)
        {
            return;
        }

        try
        {
            ItemSelectHandler? select = ItemSelectHandler.current;
            if (select != null && !select.IsEquipped)
            {
                select.TryEquipItem(labeler);
            }
        }
        catch (Exception ex)
        {
            BatchLabelMod.Log.Warning("恢复标签机装备失败: " + ex.Message);
        }
    }

    /// <summary>兜底：其它模组的前缀先返回 false 时本模组的前缀可能不执行，这里在按下后补记一次。</summary>
    private static void ArmPostfix(ISet<KeyCode> keysPressed)
    {
        if (keysPressed.Contains(KeyCode.Mouse0))
        {
            TryArm();
        }
    }

    /// <summary>松开左键但没走到 FinalizeBand（点击、框选被取消等）时，恢复标签机装备。</summary>
    private static void ReleasePostfix()
    {
        Disarm();
    }

    /// <summary>框选状态被重置（Escape、切窗口等）时，恢复标签机装备。</summary>
    private static void ResetPostfix()
    {
        Disarm();
    }

    /// <summary>
    /// 屏幕位置下可打标签的自有物品：与原版标签机目标条件一致（自有 + 有可编辑标签槽位）。
    /// </summary>
    private static GameItemElement? FindLabelableItem(Vector2 screenPosition)
    {
        GameItemElement? element = RenderHandler.RaycastElement<GameItemElement>(screenPosition);
        if (element == null || !GeneralHelper.IsItemOwned(element))
        {
            return null;
        }

        return LabelerHelper.GetEditableLabelCount(element) > 0 ? element : null;
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

        // ShouldTakeOver / TryArm 已经把起点记进 _armedSource（自有 + 可打标签），这里直接复用，
        // 不再重复做一次屏幕射线。
        if (_armedSource == null)
        {
            return true;
        }

        // 起点压在可打标签的物品上：原版会因为“射线命中物品”而完全不进入框选状态，这里手动补上。
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
                BatchLabelMod.Log.Warning("找不到 ItemMultiSelectHandler.state 字段，回退原版行为");
                return true;
            }

            StateField.SetValue(__instance, PendingBandState);
            return false;
        }
        catch (Exception ex)
        {
            BatchLabelMod.Log.Warning("从物品上开始框选失败，回退原版: " + ex.Message);
            return true;
        }
    }

    private static bool SuppressPrefix(InputActionHandler __instance, ISet<KeyCode> keysPressed)
    {
        // 装备标签机时右键物品：套用记忆里的标签，并保持装备（原版右键是取消装备）。
        if (__instance is ItemSelectHandler && TryApplyRememberedLabel(keysPressed))
        {
            return false;
        }

        if (!ShouldTakeOver(keysPressed))
        {
            return true;
        }

        // 只有确实要从物品上起手框选时才跳过这些处理器，其它情况保持原版行为。
        if (_armedSource == null)
        {
            return true;
        }

        // 清掉这些处理器上一次点击留下的状态，避免它们在松开左键时误触发。
        try
        {
            ResetHandler(__instance);
        }
        catch (Exception ex)
        {
            BatchLabelMod.Log.Warning($"重置 {__instance.GetType().Name} 状态失败: {ex.Message}");
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

    /// <summary>
    /// 框选落定（松开左键）时批量打标签：起点物品当前显示的标签是模板，框内同类型物品复制它。
    /// </summary>
    private static void FinalizeBandPostfix(ItemMultiSelectHandler __instance)
    {
        bool tookOver = false;
        try
        {
            // 按下时已经记下这次接管：框选过程中 ItemSelectHandler 会被重置（装备被取消），
            // 所以这里不能再实时检查装备状态，否则永远打不上标签。
            GameItem? source = _armed ? _armedSource : null;
            tookOver = source != null;

            // 兜底：本模组的前缀被其它模组抢跑时，仍按「触发键 + 装备标签机 + 起点位置」识别。
            if (!tookOver && BatchLabelMod.IsTriggerHeld() && BatchLabelMod.IsLabelerEquipped())
            {
                source = RaycastPressItem(__instance);
                tookOver = source != null;
            }

            if (!tookOver || source == null)
            {
                return; // 不是本模组接管的框选（原版 / 其他模组），保持原样
            }

            // 框内没有同类型物品、或它们已经显示同样的标签时，changedItems 为 0，不做任何改动。
            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            int changedItems = 0;
            if (labels.Count > 0
                && SelectedItemsField?.GetValue(__instance) is List<GameItemElement> selected
                && selected.Count > 0)
            {
                changedItems = LabelBatch.ApplyToMatchingItems(source, labels, selected);
            }

            if (changedItems > 0)
            {
                EmporiumAudioPlayer.Instance?.audioLabeler?.Play();
                BatchLabelMod.Log.Log($"批量打标签：{changedItems} 个物品复制了 {source.identifier} 的标签");
            }
        }
        catch (Exception ex)
        {
            BatchLabelMod.Log.Warning("批量打标签失败: " + ex.Message);
        }
        finally
        {
            if (tookOver)
            {
                // 先把标签机装回去，再结束框选，避免高亮残留、误拖动物品。
                Disarm();
                __instance.Cancel();
            }
        }
    }

    /// <summary>用框选起手位置（pressPosition）反查起点物品。</summary>
    private static GameItem? RaycastPressItem(ItemMultiSelectHandler handler)
    {
        if (PressPositionField == null)
        {
            return null;
        }

        object? raw = PressPositionField.GetValue(handler);
        if (raw is not Vector3 position || position == Vector3.zero)
        {
            return null;
        }

        return FindLabelableItem(position);
    }

    private static void LabelerOpenPrefix()
    {
        _labelerUiOpenFrame = Time.frameCount;
    }

    private static void LabelerOpenPostfix()
    {
        _labelerUiOpenFrame = -1;
    }

    /// <summary>玩家在标签机 UI 里改了一个标签 → 记住「这类物品的这个分类该贴成什么」。</summary>
    private static void DropdownChangedPostfix(LabelerElement __instance)
    {
        // 打开界面时的初始选中不是玩家的选择，不记。
        if (_labelerUiOpenFrame == Time.frameCount)
        {
            return;
        }

        try
        {
            LabelerUIManager? ui = LabelerUIManager.Instance;
            if (ui == null || LabelerElementFeatureField == null || LabelerUiItemField == null)
            {
                return;
            }

            if (LabelerElementFeatureField.GetValue(__instance) is not ItemFeature feature
                || string.IsNullOrEmpty(feature.category))
            {
                return;
            }

            if (LabelerUiItemField.GetValue(ui) is not GameItem item || string.IsNullOrEmpty(item.identifier))
            {
                return;
            }

            ItemCondition? condition = feature.fakeCondition;
            if (condition == null)
            {
                return;
            }

            // 水质标签除了条件还要记自定义数值（水量），与批量复制的规则保持一致。
            bool copyCustomIntValue = feature.category == LabelBatch.WaterPurityCategory;
            LabelMemory.Remember(item.identifier, new[]
            {
                new LabelBatch.LabelEntry(feature.category, condition, feature.customIntValue1, copyCustomIntValue)
            });
        }
        catch (Exception ex)
        {
            BatchLabelMod.Log.Warning("记住标签失败: " + ex.Message);
        }
    }

    private static void LabelerClosePostfix()
    {
        BatchLabelMod.SaveLabelMemory();
    }

    /// <summary>
    /// 装备标签机时右键某类物品：把记住的标签贴上去，并保持装备（原版右键是取消装备）。
    /// 返回 true 表示这次右键已由本模组处理，应跳过原版逻辑。
    /// </summary>
    private static bool TryApplyRememberedLabel(ISet<KeyCode>? keysPressed)
    {
        if (keysPressed == null || !keysPressed.Contains(KeyCode.Mouse1))
        {
            return false;
        }

        if (!BatchLabelMod.IsLabelerEquipped() || ItemPinHelper.BlocksClicks)
        {
            return false;
        }

        StoreUIManager? storeUI = StoreUIManager.Instance;
        if (storeUI != null && storeUI.hoveringStoreUI)
        {
            return false;
        }

        if (UIDragHandler.isDraggingAnyUI)
        {
            return false;
        }

        GameItemElement? element = FindLabelableItem(Input.mousePosition);
        if (element == null || string.IsNullOrEmpty(element.identifier))
        {
            return false;
        }

        // 右键身上这件标签机本身、或者这类物品还没记住过标签时，保持原版行为（取消装备）。
        if (ReferenceEquals(element, ItemSelectHandler.current?.GetEquippedItem()))
        {
            return false;
        }

        if (!LabelMemory.TryResolve(element.identifier, out List<LabelBatch.LabelEntry> labels))
        {
            return false;
        }

        try
        {
            int changed = LabelBatch.ApplyItem(element, labels);
            if (changed > 0)
            {
                EmporiumAudioPlayer.Instance?.audioLabeler?.Play();
                BatchLabelMod.Log.Log($"右键打标签：{element.identifier} 套用了 {labels.Count} 个记忆标签");
            }

            return true;
        }
        catch (Exception ex)
        {
            BatchLabelMod.Log.Warning("右键套用记忆标签失败: " + ex.Message);
            return false;
        }
    }

    private static void EnsureVanillaPostfix()
    {
        TryRegisterOption();
    }

    internal static void TryRegisterOption()
    {
        try
        {
            if (GameOptionRegistry.Find(BatchLabelMod.TriggerOptionId) != null)
            {
                return;
            }

            // 只有 vanilla 设置界面已经建立按键分区时才追加，避免生成未本地化的分区。
            if (GameOptionRegistry.FindSection(GameOptionRegistry.SECTION_KEYBIND) == null)
            {
                return;
            }

            GameOptionRegistry.Add(GameOptionRegistry.SECTION_KEYBIND, GameOptionRegistry.KeyBind(
                BatchLabelMod.TriggerOptionId,
                () => BatchLabelMod.DisplayNameText,
                () => BatchLabelMod.TriggerKey,
                key => BatchLabelMod.ApplyTriggerKey(key),
                () => BatchLabelMod.TooltipText));
        }
        catch (Exception ex)
        {
            BatchLabelMod.Log.Warning("注册按键选项失败: " + ex.Message);
        }
    }
}
