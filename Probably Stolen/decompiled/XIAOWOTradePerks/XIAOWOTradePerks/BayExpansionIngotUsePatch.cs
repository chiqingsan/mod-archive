using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(MaterialDirectory), "OnIngotTarget")]
internal static class BayExpansionIngotUsePatch
{
	private static bool Prefix(GameItem source, GameItem target)
	{
		return !BayExpansionPerk.TryHandleIngot(source, target);
	}
}
