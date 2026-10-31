using System;
using HarmonyLib;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// The vanilla mod-config window renders number settings as bare sliders with no
/// visible value. This patch adds a live numeric readout that updates while the
/// slider is dragged. Implemented as a prefix that replaces ModConfigWindow.BuildEntry
/// so it also benefits every other mod's settings panel; falls back to vanilla when
/// anything unexpected happens.
/// </summary>
internal static class ConfigValueLabels
{
    private const float ValueWidth = 62f;
    private const float SliderWidth = 224f;
    private const float FieldWidth = 245f;

    private static bool _installed;

    internal static bool Installed => _installed;

    /// <summary>Harmony patches are removed on disable, so the next enable must patch again.</summary>
    internal static void Reset()
    {
        _installed = false;
    }

    internal static void Install(Harmony harmony)
    {
        if (_installed)
        {
            return;
        }

        var target = AccessTools.Method(typeof(ModConfigWindow), "BuildEntry",
            new[] { typeof(CustomUIBuilder), typeof(ModConfig), typeof(ConfigEntry) });
        if (target == null)
        {
            Core.Log?.Log("[兼容] 未找到 ModConfigWindow.BuildEntry，跳过滑条数值显示");
            return;
        }

        harmony.Patch(target,
            prefix: new HarmonyMethod(AccessTools.Method(typeof(ConfigValueLabels), nameof(BuildEntryPrefix))));
        _installed = true;
        Core.Log?.Log("已安装：模组设置滑条数值显示");
    }

    private static bool BuildEntryPrefix(CustomUIBuilder b, ModConfig config, ConfigEntry entry)
    {
        try
        {
            Build(b, config, entry);
            return false;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"设置项渲染失败 {entry.Key}: {ex.GetType().Name} {ex.Message}");
            return true; // fall back to the vanilla renderer
        }
    }

    private static void Build(CustomUIBuilder b, ModConfig config, ConfigEntry entry)
    {
        var label = Safe(entry.Label, entry.Key);
        if (entry.Tooltip != null)
        {
            var link = "modcfg:" + config.ModId + ":" + entry.Key;
            TMPIconHover.CustomLinks[link] = entry.Tooltip;
            label = label + " <link=\"" + link + "\"><sprite name=\"info\" tint=1></link>";
        }

        b.BeginRow(4f, TextAnchor.MiddleLeft);
        b.AddLabel(label, "label_" + entry.Key);
        b.AddFlexibleSpace();

        var valueType = entry.ValueType;
        var boxed = entry.BoxedValue;
        if (valueType == typeof(bool))
        {
            b.AddToggle("", (bool)boxed, null, entry.Key);
        }
        else if (IsNumber(valueType) && entry.HasRange)
        {
            b.AddLabel(FormatNumber(valueType, Convert.ToSingle(boxed)), "value_" + entry.Key);
            var valueLabel = b.Last;
            valueLabel.SetFixedWidth(ValueWidth);
            b.AddSlider(entry.Min, entry.Max, Convert.ToSingle(boxed),
                v => valueLabel.SetText(FormatNumber(valueType, v)),
                valueType == typeof(int), entry.Key);
            b.Last.SetFixedWidth(SliderWidth);
        }
        else if (entry.Choices != null && entry.Choices.Length != 0)
        {
            b.AddDropdown(entry.Choices,
                Mathf.Max(0, Array.IndexOf(entry.Choices, ConfigEntry.Format(boxed))), null, entry.Key);
            b.Last.SetFixedWidth(FieldWidth);
        }
        else
        {
            b.AddInput("", ConfigEntry.Format(boxed), null, entry.Key);
            b.Last.SetFixedWidth(FieldWidth);
        }

        b.End();
    }

    private static bool IsNumber(Type type)
    {
        return type == typeof(int) || type == typeof(float);
    }

    private static string FormatNumber(Type type, float value)
    {
        return type == typeof(int)
            ? Mathf.RoundToInt(value).ToString()
            : value.ToString("0.##");
    }

    private static string Safe(Func<string> getter, string fallback)
    {
        if (getter == null)
        {
            return fallback;
        }

        try
        {
            return getter();
        }
        catch
        {
            return fallback;
        }
    }
}
