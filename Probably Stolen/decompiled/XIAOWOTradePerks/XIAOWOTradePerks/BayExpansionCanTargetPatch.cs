using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(GameItem), "CanTarget")]
internal static class BayExpansionCanTargetPatch
{
	private static void Postfix(GameItem __instance, GameItem targetItem, ref bool __result)
	{
		if (!__result && BayExpansionPerk.CanTargetWithIngot(__instance, targetItem))
		{
			__result = true;
		}
	}
}
