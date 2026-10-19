using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(PlayerStore), "BuyItemFromTable")]
internal static class PlayerStoreBuyItemFromTablePatch
{
	private static void Postfix(PlayerStore __instance)
	{
		JacksonSell.OnBoughtFromTable(__instance);
	}
}
