using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PerkUIController), "DeselectPerk")]
internal static class PerkDeselectedOrderPatch
{
	private static void Prefix(PerkUIController __instance)
	{
		PerkPickerScroll.Capture(__instance);
	}

	[HarmonyPriority(0)]
	private static void Postfix(PerkUIController __instance, StartingPerkElement perk)
	{
		PerkRegistry.AfterPickerMove(__instance, perk, selected: false);
	}
}
