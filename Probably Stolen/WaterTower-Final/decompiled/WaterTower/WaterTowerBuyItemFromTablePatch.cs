using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(PlayerStore), "BuyItemFromTable")]
internal static class WaterTowerBuyItemFromTablePatch
{
	private static void Postfix(PlayerStore __instance)
	{
		JacksonSell.OnBoughtFromTable(__instance);
	}
}
