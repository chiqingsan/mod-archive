using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(SaveSlotElement), "LoadSlot")]
internal static class LoadingProgressSaveSlotPatch
{
	private static void Prefix(SaveSlotElement __instance)
	{
		LoadingProgressOverlay.Begin((__instance != null) ? __instance.slotId : (-1));
	}
}
