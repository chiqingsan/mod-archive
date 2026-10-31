using System;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// MachineHelper.OnMachineActioned recomputes every module's effect stats for
/// each module (ModuleEffectHelper.ActivateOnUseAction ends with the same
/// ComputeAllModuleTempStat / ComputeModuleEffect / ApplyBasicModuleEffect trio
/// that OnMachineActioned runs again once the loop is done). Inside that loop the
/// per-module trio is redundant: only the onUsedAction delegates matter, and the
/// final recompute after the loop produces the identical end state.
/// </summary>
internal static class MachineBatch
{
    private static int _depth;
    private static int _frame = -1;
    private static bool _logged;

    internal static bool Enabled => Core.MachineModuleSkip?.Value == true;

    internal static void Reset()
    {
        _depth = 0;
        _frame = -1;
    }

    internal static void Begin()
    {
        if (!Enabled)
        {
            return;
        }

        _frame = Time.frameCount;
        _depth++;
    }

    internal static void End()
    {
        if (_depth > 0)
        {
            _depth--;
        }
    }

    internal static bool Prefix(GameItem item)
    {
        // Guard against a postfix that never ran (exception inside the target).
        if (_depth > 0 && Time.frameCount != _frame)
        {
            _depth = 0;
        }

        if (!Enabled || _depth <= 0 || item == null)
        {
            return true;
        }

        try
        {
            Apply(item, "MODULE_EFFECT1_TAG");
            Apply(item, "MODULE_EFFECT2_TAG");

            // Final state is recomputed by OnMachineActioned after the loop; skip
            // the per-module temp-stat trio entirely.
            return false;
        }
        catch (Exception ex)
        {
            if (!_logged)
            {
                _logged = true;
                Core.Log?.Warning($"模块效果批量处理异常，已回退原版: {ex.GetType().Name} {ex.Message}");
            }

            return true;
        }
    }

    private static void Apply(GameItem item, string tag)
    {
        if (!item.IsTag(tag))
        {
            return;
        }

        var id = item.GetTagReadonly(tag).GetString();
        if (ModuleEffectHelper.moduleEffects.TryGetValue(id, out var effect) && effect?.onUsedAction != null)
        {
            effect.onUsedAction(item);
        }
    }
}
