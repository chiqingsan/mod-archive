using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(MachineFridge), "UpdateFridgePowerUsage")]
internal static class FridgePowerPatch
{
	private static void Postfix(GameItem fridge)
	{
		FridgeExpansion.CorrectPower(fridge);
	}
}
