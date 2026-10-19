using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(AugHelper), "CleanupKill")]
internal static class AugHuntCleanupKillPatch
{
	private static void Prefix()
	{
		AugHuntContract.MarkCurrentAugKilled();
	}
}
