using System;
using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "BuyItem")]
internal static class StableSupplyPurchaseContextPatch
{
	[HarmonyPriority(800)]
	private static void Prefix()
	{
		TradeEconomyEffects.BeginPlayerPurchase();
	}

	[HarmonyPriority(0)]
	private static Exception Finalizer(Exception __exception)
	{
		TradeEconomyEffects.EndPlayerPurchase();
		return __exception;
	}
}
