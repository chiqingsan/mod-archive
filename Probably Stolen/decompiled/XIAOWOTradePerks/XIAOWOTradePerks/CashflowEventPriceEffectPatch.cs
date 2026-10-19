using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreEvent), "GetEventPriceEffect")]
internal static class CashflowEventPriceEffectPatch
{
	private static void Postfix(StoreEvent __instance, ref string __result)
	{
		string cashflowPriceEffect = DeterministicSchedule.GetCashflowPriceEffect((__instance != null) ? __instance.identifier : null);
		if (cashflowPriceEffect != null)
		{
			__result = cashflowPriceEffect;
		}
	}
}
