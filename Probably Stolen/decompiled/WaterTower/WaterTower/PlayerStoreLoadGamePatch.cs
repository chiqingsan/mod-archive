using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(PlayerStore), "LoadGame")]
internal static class PlayerStoreLoadGamePatch
{
	private static void Postfix()
	{
		TowerRegistry.RestoreAfterLoad();
	}
}
