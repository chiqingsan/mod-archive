using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreClient), "GetTradeRepMultiplier")]
internal static class ReputationBusinessPatch
{
	internal static int DiagnosticInvocationCount { get; private set; }

	internal static double DiagnosticOriginalResult { get; private set; }

	internal static double DiagnosticFinalResult { get; private set; }

	internal static void ResetDiagnostic()
	{
		DiagnosticInvocationCount = 0;
		DiagnosticOriginalResult = 0.0;
		DiagnosticFinalResult = 0.0;
	}

	private static void Postfix(StoreClient __instance, ref double __result)
	{
		DiagnosticInvocationCount++;
		DiagnosticOriginalResult = __result;
		if (PerkRegistry.IsActive("xiaowo_trade_reputation_business") && TradeEconomyEffects.IsOrdinaryTradeClient(__instance) && __result > 0.0)
		{
			__result = TradeEconomyEffects.ReputationGain(__result);
		}
		DiagnosticFinalResult = __result;
	}
}
