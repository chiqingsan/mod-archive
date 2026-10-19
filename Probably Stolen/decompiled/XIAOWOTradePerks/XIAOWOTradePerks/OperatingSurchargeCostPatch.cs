using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreService), "UpdateCost")]
internal static class OperatingSurchargeCostPatch
{
	private static void Prefix(StoreService __instance)
	{
		OperatingSurcharge.BeforeNativeUpdate(__instance);
	}

	private static void Postfix(StoreService __instance)
	{
		OperatingSurcharge.AfterNativeUpdate(__instance);
	}
}
