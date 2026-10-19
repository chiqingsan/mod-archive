using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(GameItem), "MayTarget")]
internal static class BayExpansionMayTargetPatch
{
	private static void Postfix(GameItem __instance, GameItem targetItem, ref bool __result)
	{
		if (!__result && BayExpansionPerk.CanTargetWithIngot(__instance, targetItem))
		{
			__result = true;
		}
	}
}
