using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(BargainUIManager), "GetBargainSuccessChance")]
internal static class NegotiationVeteranPatch
{
	internal static int DiagnosticInvocationCount { get; private set; }

	internal static int DiagnosticOriginalResult { get; private set; }

	internal static int DiagnosticFinalResult { get; private set; }

	internal static void ResetDiagnostic()
	{
		DiagnosticInvocationCount = 0;
		DiagnosticOriginalResult = 0;
		DiagnosticFinalResult = 0;
	}

	private static void Postfix(ref int __result)
	{
		DiagnosticInvocationCount++;
		DiagnosticOriginalResult = __result;
		if (PerkRegistry.IsActive("xiaowo_trade_negotiation_veteran"))
		{
			__result = TradeEconomyEffects.NegotiationChance(__result);
		}
		DiagnosticFinalResult = __result;
	}
}
