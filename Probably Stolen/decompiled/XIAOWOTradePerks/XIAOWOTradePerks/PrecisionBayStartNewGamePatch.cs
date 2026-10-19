using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "StartNewGame")]
internal static class PrecisionBayStartNewGamePatch
{
	private static void Postfix(PlayerStore __instance)
	{
		PrecisionBayMode.ApplyNewGameSelection(__instance);
	}
}
