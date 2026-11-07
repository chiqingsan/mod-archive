using System;
using HarmonyLib;
using UnityEngine;

namespace ClickToAdvanceFinal;

public sealed class ClickToAdvanceMod : IMod
{
    private const string HarmonyId = "chiqingsan.click-to-advance";
    private const string BypassEnabledConfigKey = "BypassEnabled";
    private const string BypassKeyConfigKey = "BypassKey";
    private const string SettingsSection = "点击推进对话";

    internal const string BypassEnabledOptionId = "click_to_advance_bypass_enabled";
    internal const string BypassKeyOptionId = "key_click_to_advance_bypass";

    private GameObject? _runner;
    private Harmony? _harmony;
    private ConfigEntry<bool>? _bypassEnabledConfig;
    private ConfigEntry<string>? _bypassKeyConfig;

    internal static ClickToAdvanceMod? Instance { get; private set; }

    /// <summary>按住该键时，点击左键不推进对话；None 表示不屏蔽。</summary>
    internal static KeyCode BypassKey { get; private set; } = KeyCode.LeftAlt;

    /// <summary>屏蔽键总开关，默认关闭；关闭时与 1.0.0 行为一致。</summary>
    internal static bool BypassEnabled { get; private set; }

    public void Init(ModManifest manifest)
    {
        Instance = this;
        LoadSettings(manifest.ID);
        TryRegisterOptions();
    }

    public void OnEnable()
    {
        Instance = this;

        _runner = new GameObject("ClickToAdvance");
        UnityEngine.Object.DontDestroyOnLoad(_runner);
        _runner.AddComponent<ClickToAdvanceRunner>();

        _harmony = new Harmony(HarmonyId);
        _harmony.PatchAll();
    }

    public void OnDisable()
    {
        _harmony?.UnpatchAll(HarmonyId);
        _harmony = null;

        if (_runner != null) UnityEngine.Object.Destroy(_runner);
        _runner = null;

        if (ReferenceEquals(Instance, this)) Instance = null;
    }

    /// <summary>读取 ModConfigs/&lt;modId&gt;.json 里的设置；改键入口在 游戏设置 → 按键。</summary>
    private void LoadSettings(string modId)
    {
        try
        {
            var config = ModConfig.For(modId);

            _bypassEnabledConfig = config.Define(
                BypassEnabledConfigKey,
                false,
                SettingsSection,
                () => "按住屏蔽键时禁用点击推进",
                () => "开启后，按住屏蔽键（默认 Alt）时点击鼠标左键不会推进对话；关闭则保持原行为。");

            _bypassKeyConfig = config.Define(
                BypassKeyConfigKey,
                nameof(KeyCode.LeftAlt),
                SettingsSection,
                () => "屏蔽键（备份值）",
                null,
                () => "请到 游戏设置 → 按键 中修改屏蔽键；这里只是备份值，手动编辑后需重启游戏才生效。");

            BypassEnabled = _bypassEnabledConfig.Value;
            _bypassEnabledConfig.OnChanged += value => BypassEnabled = value;

            BypassKey = ParseBypassKey(_bypassKeyConfig.Value);
            _bypassKeyConfig.OnChanged += value => BypassKey = ParseBypassKey(value);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ClickToAdvance] 读取设置失败，使用默认值: " + ex.Message);
        }
    }

    private static KeyCode ParseBypassKey(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return KeyCode.LeftAlt;
        if (!Enum.TryParse(raw.Trim(), true, out KeyCode parsed)) return KeyCode.LeftAlt;
        return IsValidBypassKey(parsed) ? parsed : KeyCode.LeftAlt;
    }

    /// <summary>屏蔽键必须是键盘键：鼠标键会让点击永远被屏蔽，Escape 是游戏改键保留键；None 表示不屏蔽。</summary>
    internal static bool IsValidBypassKey(KeyCode key)
    {
        if (key == KeyCode.None) return true;
        if (key == KeyCode.Escape) return false;

        var value = (int)key;
        return value < (int)KeyCode.Mouse0 || value > (int)KeyCode.Mouse6;
    }

    internal static void SetBypassKey(KeyCode key)
    {
        if (!IsValidBypassKey(key)) key = KeyCode.LeftAlt;
        BypassKey = key;

        try
        {
            var config = Instance?._bypassKeyConfig;
            if (config != null && config.Value != key.ToString()) config.Set(key.ToString());
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ClickToAdvance] 保存屏蔽键失败: " + ex.Message);
        }
    }

    internal static void SetBypassEnabled(bool enabled)
    {
        BypassEnabled = enabled;

        try
        {
            var config = Instance?._bypassEnabledConfig;
            if (config != null && config.Value != enabled) config.Set(enabled);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ClickToAdvance] 保存屏蔽开关失败: " + ex.Message);
        }
    }

    /// <summary>按住屏蔽键时返回 true，本帧点击不推进对话；点击不被消费，游戏原逻辑照常处理。</summary>
    internal static bool ShouldSuppressAdvance()
    {
        if (!BypassEnabled) return false;

        var key = BypassKey;
        if (key == KeyCode.None) return false;
        if (key == KeyCode.LeftAlt || key == KeyCode.RightAlt)
            return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

        return Input.GetKey(key);
    }

    /// <summary>把开关和屏蔽键注册进 游戏设置 → 按键（挂 EnsureVanilla 之后，等原版按键分区建好）。</summary>
    internal static void TryRegisterOptions()
    {
        try
        {
            if (GameOptionRegistry.FindSection(GameOptionRegistry.SECTION_KEYBIND) == null) return;

            if (GameOptionRegistry.Find(BypassEnabledOptionId) == null)
            {
                GameOptionRegistry.Add(GameOptionRegistry.SECTION_KEYBIND, GameOptionRegistry.Toggle(
                    BypassEnabledOptionId,
                    () => "点击推进：按住屏蔽键时禁用",
                    () => BypassEnabled,
                    SetBypassEnabled,
                    () => "开启后，按住屏蔽键（默认 Alt）时点击鼠标左键不会推进对话。"));
            }

            if (GameOptionRegistry.Find(BypassKeyOptionId) == null)
            {
                GameOptionRegistry.Add(GameOptionRegistry.SECTION_KEYBIND, GameOptionRegistry.KeyBind(
                    BypassKeyOptionId,
                    () => "点击推进：屏蔽键",
                    () => BypassKey,
                    SetBypassKey,
                    () => "按住此键时点击鼠标左键不会推进对话；设为 None 可关闭屏蔽。"));
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ClickToAdvance] 注册按键选项失败: " + ex.Message);
        }
    }
}

public sealed class ClickToAdvanceRunner : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (ClickToAdvanceMod.ShouldSuppressAdvance()) return; // 按住屏蔽键：这次点击交给游戏
        Advance(DialogUIManager.Instance);
    }

    internal static void Advance(DialogUIManager? dialog)
    {
        if (dialog == null || dialog.currentDialog == null || dialog.IsChoice()) return;
        dialog.OnAdvancePressed();
    }
}

[HarmonyPatch(typeof(GameOptionRegistry), "EnsureVanilla")]
internal static class EnsureVanillaPatch
{
    private static void Postfix() => ClickToAdvanceMod.TryRegisterOptions();
}
