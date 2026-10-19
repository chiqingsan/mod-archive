using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(AugHelper), "OnAugLeftAlive")]
internal static class AugHuntEscapePatch
{
	private static void Prefix()
	{
		AugHuntContract.MarkCurrentAugEscaped();
	}
}
