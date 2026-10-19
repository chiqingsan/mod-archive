using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(Shotgun), "Unlock")]
internal static class AugHuntShotgunUnlockPatch
{
	private static void Postfix()
	{
		AugHuntContract.OnShotgunStateChanged();
	}
}
