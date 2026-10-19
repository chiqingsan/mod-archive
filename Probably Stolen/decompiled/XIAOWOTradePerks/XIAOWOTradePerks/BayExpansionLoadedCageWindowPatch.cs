using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(AnimalCage), "GetCageInventory")]
internal static class BayExpansionLoadedCageWindowPatch
{
	private static void Postfix(GameItem cage, GameGridInventory __result)
	{
		BayExpansionPerk.RestoreLoadedCageWindow(cage, __result);
	}
}
