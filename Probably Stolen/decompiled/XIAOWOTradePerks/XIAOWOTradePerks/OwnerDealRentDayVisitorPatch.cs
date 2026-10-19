using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreClientManager), "CheckRentDay")]
internal static class OwnerDealRentDayVisitorPatch
{
	private static void Postfix(StoreClientManager __instance)
	{
		OwnerDeal.ScheduleWeeklyVisitors(__instance);
	}
}
