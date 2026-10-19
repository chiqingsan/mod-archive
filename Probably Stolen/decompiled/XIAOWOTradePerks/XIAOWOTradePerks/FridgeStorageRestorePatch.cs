using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(MachineFridge), "ApplyStorageSize")]
internal static class FridgeStorageRestorePatch
{
	private static void Postfix(GameItem fridge)
	{
		FridgeExpansion.Restore(fridge);
	}
}
