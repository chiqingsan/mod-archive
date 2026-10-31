using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(SpriteHelper), "UpdateWaterContainerSprite")]
internal static class WaterTowerTankLevelSpritePatch
{
	private static void Postfix(GameItem __0)
	{
		WaterTowerTankLevelSprite.OnUpdate(__0);
	}
}
