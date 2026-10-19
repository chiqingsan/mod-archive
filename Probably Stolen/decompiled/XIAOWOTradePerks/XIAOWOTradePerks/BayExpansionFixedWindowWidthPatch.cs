using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal static class BayExpansionFixedWindowWidthPatch
{
	private static void Postfix(GameGridInventory __instance, ref int __result)
	{
		BayExpansionPerk.TryOverrideChildWidth(__instance, ref __result);
	}
}
