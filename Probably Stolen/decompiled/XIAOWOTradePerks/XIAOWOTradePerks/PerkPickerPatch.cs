using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PerkUIController), "OpenUI")]
internal static class PerkPickerPatch
{
	private static void Prefix(PerkUIController __instance)
	{
		PerkPickerScroll.BeforeOpen(__instance);
		FreeBuildMode.BeforeOpen(__instance);
	}

	[HarmonyPriority(0)]
	private static void Postfix(PerkUIController __instance)
	{
		PerkRegistry.EnsurePicker(__instance);
		FreeBuildMode.AfterOpen(__instance);
		PrecisionBayMode.AfterPickerOpen(__instance);
	}
}
