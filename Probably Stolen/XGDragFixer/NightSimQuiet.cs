using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using HarmonyLib;
using TMPro;

namespace XGDragFixer;

/// <summary>
/// Silences visual-only work while the vanilla nightly chemical simulation runs.
/// The simulation calls every machine's "one second" callback 360 times in a row;
/// those callbacks update labels, button states and machinery layout even though
/// the settlement happens with the windows closed. Those writes are pure visuals:
/// the very same callbacks run once per normal gameplay second and re-apply the
/// latest state, so skipping them during the settlement cannot change any result.
/// </summary>
internal static class NightSimQuiet
{
    private static bool _quiet;
    private static int _depth;
    private static bool _installed;
    private static long _uiTicks;
    private static int _uiCalls;
    private static int _uiSkipped;
    private static int _layoutSkipped;

    internal static bool Quiet => _quiet;
    internal static bool Installed => _installed;

    internal static void Begin()
    {
        if (_installed && Core.NightSimUIQuiet?.Value == true)
        {
            _depth++;
            _quiet = true;
        }
    }

    internal static void End()
    {
        if (_depth <= 0)
        {
            return;
        }

        _depth--;
        if (_depth == 0)
        {
            _quiet = false;
        }
    }

    internal static void Reset()
    {
        _quiet = false;
        _depth = 0;
        _uiTicks = 0;
        _uiCalls = 0;
        _uiSkipped = 0;
        _layoutSkipped = 0;
    }

    internal static void Install(HarmonyLib.Harmony harmony)
    {
        if (_installed)
        {
            return;
        }

        _installed = true;
        if (Core.NightSimUIQuiet?.Value != true)
        {
            Core.Log?.Log("[结算加速] 夜间界面静默未启用（配置关闭），不安装相关补丁");
            return;
        }

        var textPrefix = new HarmonyMethod(AccessTools.Method(typeof(NightSimQuiet), nameof(TextPrefix)));
        var textPostfix = new HarmonyMethod(AccessTools.Method(typeof(NightSimQuiet), nameof(TextPostfix)));
        var skipPrefix = new HarmonyMethod(AccessTools.Method(typeof(NightSimQuiet), nameof(SkipPrefix)));

        var textPatched = 0;
        foreach (var type in new[] { typeof(TMP_Text), typeof(TextMeshProUGUI) })
        {
            try
            {
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public |
                                                       BindingFlags.DeclaredOnly))
                {
                    if (method.Name != "SetText")
                    {
                        continue;
                    }

                    try
                    {
                        harmony.Patch(method, textPrefix, textPostfix);
                        textPatched++;
                    }
                    catch
                    {
                        // A single overload is not worth failing the whole install for.
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Log?.Warning($"夜间界面静默：扫描文字刷新失败: {ex.Message}");
            }
        }

        var layoutPatched = 0;
        layoutPatched += PatchSkip(harmony, typeof(MachineryStack), "Validate", Type.EmptyTypes, skipPrefix);
        layoutPatched += PatchSkip(harmony, typeof(MachineryRadioList), "Select",
            new[] { typeof(int), typeof(bool) }, skipPrefix);

        Core.Log?.Log(
            $"[结算加速] 已挂接夜间界面静默：文字刷新 {textPatched} 个重载，机器布局/选择 {layoutPatched} 个入口");
    }

    private static int PatchSkip(HarmonyLib.Harmony harmony, Type type, string name, Type[] arguments,
        HarmonyMethod prefix)
    {
        try
        {
            var method = arguments.Length == 0
                ? AccessTools.Method(type, name)
                : AccessTools.Method(type, name, arguments);
            if (method == null)
            {
                Core.Log?.Warning($"[兼容] 未找到 {type.Name}.{name}，夜间界面静默跳过该入口");
                return 0;
            }

            harmony.Patch(method, prefix);
            return 1;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"夜间界面静默补丁失败 {type.Name}.{name}: {ex.Message}");
            return 0;
        }
    }

    private static bool TextPrefix(out long __state)
    {
        _uiCalls++;
        if (!_quiet)
        {
            __state = NightBreakdown.Active ? Stopwatch.GetTimestamp() : 0L;
            return true;
        }

        _uiSkipped++;
        __state = 0;
        return false;
    }

    private static void TextPostfix(long __state)
    {
        if (__state != 0)
        {
            _uiTicks += Stopwatch.GetTimestamp() - __state;
        }
    }

    private static bool SkipPrefix()
    {
        if (!_quiet)
        {
            return true;
        }

        _layoutSkipped++;
        return false;
    }

    internal static string Summary()
    {
        var ms = _uiTicks * 1000.0 / Stopwatch.Frequency;
        return $"UI文字={ms:0.0}ms/{_uiCalls}次(跳过{_uiSkipped}) 机器布局跳过={_layoutSkipped}次";
    }
}
