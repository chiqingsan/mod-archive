using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreClientManager), "AddAugClient")]
internal static class AugHuntDailyCapPatch
{
	private static bool Prefix(StoreClientManager __instance)
	{
		return AugHuntContract.BeforeAddAug(__instance);
	}

	private static void Postfix(StoreClientManager __instance)
	{
		AugHuntContract.AfterAddAug(__instance);
	}
}
