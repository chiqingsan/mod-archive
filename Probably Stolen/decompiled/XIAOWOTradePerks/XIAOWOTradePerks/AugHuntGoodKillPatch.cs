using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(AugHelper), "OnGoodKill")]
internal static class AugHuntGoodKillPatch
{
	private static void Prefix()
	{
		AugHuntContract.MarkCurrentAugKilled();
	}
}
