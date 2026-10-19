using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(ItemFeature), "GetPublicDisplay")]
internal static class CashflowPublicFeatureDisplayPatch
{
	private static void Postfix(ItemFeature __instance, ref string __result)
	{
		string cashflowFeatureDisplay = DeterministicSchedule.GetCashflowFeatureDisplay((__instance != null) ? __instance.identifier : null);
		if (cashflowFeatureDisplay != null)
		{
			__result = cashflowFeatureDisplay;
		}
	}
}
