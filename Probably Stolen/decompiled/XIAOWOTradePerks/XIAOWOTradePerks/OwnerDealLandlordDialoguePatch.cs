using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreClientList), "CreateLandlordClient")]
internal static class OwnerDealLandlordDialoguePatch
{
	private static void Postfix(StoreClient __result)
	{
		OwnerDeal.AttachCashBuyoutChoice(__result);
	}
}
