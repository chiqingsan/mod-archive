using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(GameItem), "Target")]
internal static class BayExpansionTargetPatch
{
	private static bool Prefix(GameItem __instance, GameItem targetItem)
	{
		if (BayExpansionPerk.IsMetalIngot(__instance))
		{
			return !BayExpansionPerk.TryHandleIngot(__instance, targetItem);
		}
		return true;
	}
}
