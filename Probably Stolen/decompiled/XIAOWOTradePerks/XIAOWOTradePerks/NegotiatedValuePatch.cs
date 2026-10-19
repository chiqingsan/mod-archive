using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(GameItem), "GetNegociatedValue")]
internal static class NegotiatedValuePatch
{
	private static void Postfix(ref long __result)
	{
		__result = TradeEffects.ApplySaleModifiers(__result);
	}
}
