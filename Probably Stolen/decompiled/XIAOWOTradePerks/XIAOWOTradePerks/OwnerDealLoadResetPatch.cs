using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "LoadGame")]
internal static class OwnerDealLoadResetPatch
{
	[HarmonyPriority(800)]
	private static void Prefix()
	{
		OwnerDeal.OnGameLoading();
	}

	[HarmonyPriority(0)]
	private static void Postfix(PlayerStore __instance)
	{
		OwnerDeal.ScheduleWeeklyVisitors((__instance != null) ? __instance.storeClientManager : null);
	}
}
