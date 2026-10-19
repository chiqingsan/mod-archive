using System;
using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreClientManager), "ComputeInspectionCooldownRange")]
internal static class SecurityTargetInspectionPatch
{
	private static void Postfix(ref int min, ref int max)
	{
		if (PerkRegistry.IsActive("xiaowo_trade_security_target"))
		{
			min = TradeEffects.ShortenedCooldown(min);
			max = Math.Max(min, TradeEffects.ShortenedCooldown(max));
		}
	}
}
