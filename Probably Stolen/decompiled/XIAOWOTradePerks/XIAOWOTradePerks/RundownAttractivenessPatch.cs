using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "GetCurrentStoreAttractiveness")]
internal static class RundownAttractivenessPatch
{
	private static void Postfix(ref int __result)
	{
		if (PerkRegistry.IsActive("xiaowo_trade_rundown_storefront"))
		{
			__result = TradeEffects.RundownAttractiveness(__result);
		}
	}
}
