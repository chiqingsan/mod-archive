using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(MaterialDirectory), "CanIngotTarget")]
internal static class BayExpansionIngotTargetPatch
{
	private static void Postfix(GameItem source, GameItem target, ref bool __result)
	{
		if (!__result && BayExpansionPerk.CanTargetWithIngot(source, target))
		{
			__result = true;
		}
	}
}
