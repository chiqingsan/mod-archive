using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StartingPerkList), "InitStartingPerk")]
internal static class StartingPerkRegistrationPatch
{
	private static void Postfix()
	{
		PerkRegistry.EnsureRegistered();
	}
}
