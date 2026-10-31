using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace XGDragFixer;

/// <summary>
/// AdvancedContentSizeFitter.Update calls AdjustSize every frame, writing the same
/// constant size (min(1, maxWidth) / 1) with SetSizeWithCurrentAnchors — which marks
/// the layout dirty each frame. Apply the size once per fitter configuration and
/// skip the redundant writes afterwards; a re-enable runs the vanilla OnEnable path.
/// </summary>
internal static class SizeFitterGuard
{
    private sealed class State
    {
        internal int Horizontal;
        internal int Vertical;
        internal float MaxWidth;
    }

    private static readonly FieldInfo? RectField =
        AccessTools.Field(typeof(AdvancedContentSizeFitter), "rectTransform");

    private static readonly ConditionalWeakTable<AdvancedContentSizeFitter, State> Applied = new();

    internal static void Reset()
    {
        Applied.Clear();
    }

    internal static bool Prefix(AdvancedContentSizeFitter __instance)
    {
        if (Core.SizeFitterGuard?.Value != true || __instance == null)
        {
            return true;
        }

        try
        {
            if (RectField?.GetValue(__instance) == null)
            {
                return true;
            }

            var state = new State
            {
                Horizontal = (int)__instance.horizontalFitWithMax,
                Vertical = (int)__instance.verticalFitWithMax,
                MaxWidth = __instance.maxWidth
            };

            if (Applied.TryGetValue(__instance, out var applied) &&
                applied.Horizontal == state.Horizontal &&
                applied.Vertical == state.Vertical &&
                applied.MaxWidth.Equals(state.MaxWidth))
            {
                return false;
            }

            Applied.Remove(__instance);
            Applied.Add(__instance, state);
        }
        catch
        {
            // Unknown state shape: run the vanilla Update.
        }

        return true;
    }
}
