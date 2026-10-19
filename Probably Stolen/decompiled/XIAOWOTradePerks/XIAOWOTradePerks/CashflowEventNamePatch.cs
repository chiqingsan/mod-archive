using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreEvent), "GetDisplayName")]
internal static class CashflowEventNamePatch
{
	private static void Postfix(StoreEvent __instance, ref string __result)
	{
		string cashflowEventName = DeterministicSchedule.GetCashflowEventName((__instance != null) ? __instance.identifier : null);
		if (cashflowEventName != null)
		{
			__result = cashflowEventName;
		}
	}
}
