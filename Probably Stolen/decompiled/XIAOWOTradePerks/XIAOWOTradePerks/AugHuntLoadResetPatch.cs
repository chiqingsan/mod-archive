using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "LoadGame")]
internal static class AugHuntLoadResetPatch
{
	private static void Prefix()
	{
		AugHuntContract.OnGameLoading();
	}
}
