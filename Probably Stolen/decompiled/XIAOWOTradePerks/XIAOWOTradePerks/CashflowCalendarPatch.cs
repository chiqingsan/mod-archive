using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreEventManager), "OnDayStart")]
internal static class CashflowCalendarPatch
{
	private static void Postfix(StoreEventManager __instance)
	{
		DeterministicSchedule.EnsureCashflowCalendar(__instance);
	}
}
