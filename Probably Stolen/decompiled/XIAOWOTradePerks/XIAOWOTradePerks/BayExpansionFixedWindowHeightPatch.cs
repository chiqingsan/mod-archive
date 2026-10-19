using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal static class BayExpansionFixedWindowHeightPatch
{
	private static void Postfix(GameGridInventory __instance, ref int __result)
	{
		BayExpansionPerk.TryOverrideChildHeight(__instance, ref __result);
	}
}
