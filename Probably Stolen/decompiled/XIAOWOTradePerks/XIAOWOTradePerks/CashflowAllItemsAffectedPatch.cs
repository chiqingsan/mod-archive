using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreEvent), "IsItemAffectedByThisEvent")]
internal static class CashflowAllItemsAffectedPatch
{
	private static void Postfix(StoreEvent __instance, ref bool __result)
	{
		if (DeterministicSchedule.IsCashflowEvent((__instance != null) ? __instance.identifier : null))
		{
			__result = true;
		}
	}
}
