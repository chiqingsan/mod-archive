using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PerkUIController), "OnChange")]
internal static class FreeBuildLimitPatch
{
	[HarmonyPriority(800)]
	private static void Prefix(PerkUIController __instance)
	{
		FreeBuildMode.ApplyLimits(__instance);
	}
}
