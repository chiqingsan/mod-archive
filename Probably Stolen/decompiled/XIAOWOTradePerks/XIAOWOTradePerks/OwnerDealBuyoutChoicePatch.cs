using Il2Cpp;

namespace XIAOWOTradePerks;

internal static class OwnerDealBuyoutChoicePatch
{
	internal static void Postfix(PlayerStore __instance, ref bool __result)
	{
		if (!__result && OwnerDeal.CanOfferCashBuyout(__instance))
		{
			__result = true;
		}
	}
}
