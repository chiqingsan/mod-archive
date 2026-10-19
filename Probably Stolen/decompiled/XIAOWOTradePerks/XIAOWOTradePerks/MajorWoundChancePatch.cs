using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(ScavHelper), "GetMajorWoundChance")]
internal static class MajorWoundChancePatch
{
	private static void Postfix(ref float __result)
	{
		__result = NightScavengingRisk.AddToChance(__result, PerkRegistry.IsActive("xiaowo_trade_reckless_scavenger"));
	}
}
