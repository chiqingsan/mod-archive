using System;
using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "SellItem")]
internal static class PlayerSaleContextPatch
{
	[HarmonyPriority(800)]
	private static void Prefix()
	{
		TradeEffects.BeginPlayerSale();
	}

	[HarmonyPriority(0)]
	private static Exception Finalizer(Exception __exception)
	{
		TradeEffects.EndPlayerSale();
		return __exception;
	}
}
