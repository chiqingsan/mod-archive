using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(PlayerStore), "EndNight")]
internal static class WaterTowerEndNightPatch
{
	private static void Postfix()
	{
		NightSettlement.Run();
	}
}
