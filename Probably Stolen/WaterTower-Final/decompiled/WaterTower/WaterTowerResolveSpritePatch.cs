using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace WaterTower;

[HarmonyPatch(typeof(GameItemElement), "ResolveSpriteByName")]
internal static class WaterTowerResolveSpritePatch
{
	private static bool Prefix(string name, ref Sprite __result)
	{
		return !ModSprite.TryResolve(name, ref __result);
	}
}
