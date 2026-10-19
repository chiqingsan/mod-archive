using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal static class BayExpansionResizableWindowHeightPatch
{
	private static void Postfix(PixelWindow __instance, ref int __result)
	{
		BayExpansionPerk.TryOverrideWindowHeight(__instance, ref __result);
	}
}
