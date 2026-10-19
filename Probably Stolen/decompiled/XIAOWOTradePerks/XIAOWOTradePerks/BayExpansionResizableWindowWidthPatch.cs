using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal static class BayExpansionResizableWindowWidthPatch
{
	private static void Postfix(PixelWindow __instance, ref int __result)
	{
		BayExpansionPerk.TryOverrideWindowWidth(__instance, ref __result);
	}
}
