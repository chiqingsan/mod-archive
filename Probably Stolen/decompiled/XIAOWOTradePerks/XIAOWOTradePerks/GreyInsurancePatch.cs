using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "OnItemBought")]
internal static class GreyInsurancePatch
{
	private static void Postfix(GameItem gameItem)
	{
		TradeEffects.ApplyGreyInsurance(gameItem);
	}
}
