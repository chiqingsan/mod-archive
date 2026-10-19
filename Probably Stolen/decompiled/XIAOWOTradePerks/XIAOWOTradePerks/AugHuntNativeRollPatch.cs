using System;
using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(RNG), "Roll", new Type[] { typeof(int) })]
internal static class AugHuntNativeRollPatch
{
	private static void Prefix(ref int chance)
	{
		AugHuntContract.ForceNativeRoll(ref chance);
	}
}
