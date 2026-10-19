using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(Shotgun), "LockShotgun")]
internal static class AugHuntShotgunLockPatch
{
	private static void Postfix()
	{
		AugHuntContract.OnShotgunStateChanged();
	}
}
