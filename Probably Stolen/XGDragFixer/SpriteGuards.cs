using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace XGDragFixer;

/// <summary>
/// A single item-state refresh calls GameItemElement.ValidateSelectLocked, which
/// rewrites every sprite modifier of the item even when none of them changed.
/// Several systems refresh the same item several times in one frame, so remember
/// what was applied and skip the write when the visual result would be identical.
/// </summary>
internal static class SpriteGuards
{
    private static readonly FieldInfo? SpriteModsField =
        AccessTools.Field(typeof(GameItemElement), "spriteMods");

    private static readonly FieldInfo? FakeSpriteModsField =
        AccessTools.Field(typeof(GameItemElement), "fakeSpriteMods");

    internal static bool Prefix(GameItemElement __instance, int i, Sprite modSprite, Vector2 modPosition, int modOrientation, bool modFlip)
    {
        if (Core.SpriteRefreshGuard?.Value != true || __instance == null || modSprite == null)
        {
            return true;
        }

        try
        {
            var nodes = SpriteModsField?.GetValue(__instance) as List<TreeNodeRender>;
            var fakeNodes = FakeSpriteModsField?.GetValue(__instance) as List<TreeNodeRender>;
            if (nodes == null || fakeNodes == null || i < 0 || i >= nodes.Count || i >= fakeNodes.Count)
            {
                return true;
            }

            var node = nodes[i];
            if (node == null || node.gameObject == null || !node.gameObject.activeSelf)
            {
                return true;
            }

            if (node.image == null || !node.image.enabled)
            {
                return true;
            }

            if (!ReferenceEquals(node.image.sprite, modSprite))
            {
                return true;
            }

            var background = __instance.background;
            if (background == null || background.rectTransform == null)
            {
                return true;
            }

            var target = modPosition + background.rectTransform.anchoredPosition;
            var rect = node.rectTransform;
            if (rect == null || (rect.anchoredPosition - target).sqrMagnitude > 0.01f)
            {
                return true;
            }

            var scale = rect.localScale;
            var wantScaleX = modFlip ? -1f : 1f;
            if (Mathf.Abs(scale.x - wantScaleX) > 0.01f || Mathf.Abs(scale.y - 1f) > 0.01f)
            {
                return true;
            }

            var normalizedOrientation = (modOrientation % 4 + 4) % 4;
            var wantAngle = 90f * normalizedOrientation;
            if (Mathf.Abs(Mathf.DeltaAngle(rect.localEulerAngles.z, wantAngle)) > 0.5f)
            {
                return true;
            }

            var fake = fakeNodes[i];
            if (fake == null || fake.gameObject == null || !fake.gameObject.activeSelf)
            {
                return true;
            }

            if (fake.image == null || fake.image.enabled || !ReferenceEquals(fake.image.sprite, modSprite))
            {
                return true;
            }

            if (fake.rectTransform == null || (fake.rectTransform.anchoredPosition - target).sqrMagnitude > 0.01f)
            {
                return true;
            }

            // The visible sprite already matches the requested state.
            return false;
        }
        catch
        {
            // Unexpected reflection/state shape: run the vanilla implementation.
        }

        return true;
    }
}
