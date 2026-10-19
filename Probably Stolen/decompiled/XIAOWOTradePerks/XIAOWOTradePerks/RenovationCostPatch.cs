using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(NetworkUpgrade), "GetCost")]
internal static class RenovationCostPatch
{
	private static void Postfix(NetworkUpgrade __instance, ref int __result)
	{
		if (__instance != null && TradeEffects.IsRenovation(__instance.id) && PerkRegistry.IsActive("xiaowo_trade_rundown_storefront"))
		{
			__result = TradeEffects.RenovationCost(__result);
		}
	}
}
