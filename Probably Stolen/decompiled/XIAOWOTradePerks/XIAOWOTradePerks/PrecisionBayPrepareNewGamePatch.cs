using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(MainMenuUIController), "OnNewGameClick")]
internal static class PrecisionBayPrepareNewGamePatch
{
	private static void Prefix()
	{
		PrecisionBayMode.PrepareNewGame();
	}
}
