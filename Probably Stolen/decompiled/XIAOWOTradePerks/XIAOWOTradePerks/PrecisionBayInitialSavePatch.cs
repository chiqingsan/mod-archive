using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "InitialSave")]
internal static class PrecisionBayInitialSavePatch
{
	private static void Prefix(PlayerStore __instance)
	{
		PrecisionBayMode.ApplyNewGameSelection(__instance);
	}
}
