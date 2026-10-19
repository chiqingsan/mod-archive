using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(GameItem), "ValidateShapeState")]
internal static class FridgeValidationRestorePatch
{
	[HarmonyPriority(0)]
	private static void Postfix(GameItem __instance)
	{
		FridgeExpansion.Restore(__instance);
	}
}
