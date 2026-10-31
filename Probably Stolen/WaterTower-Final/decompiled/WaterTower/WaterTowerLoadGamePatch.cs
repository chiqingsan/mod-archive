using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(PlayerStore), "LoadGame")]
internal static class WaterTowerLoadGamePatch
{
	private static void Prefix()
	{
		SaveLayoutMigration.EnsureInstalled();
	}

	private static void Postfix()
	{
		TowerRegistry.RestoreAfterLoad();
	}
}
