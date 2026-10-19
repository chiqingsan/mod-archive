using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(EscapeUIManager), "OpenEscapeMenu")]
internal static class PrecisionBayPauseMenuPatch
{
	private static void Postfix(EscapeUIManager __instance)
	{
		PrecisionBayMode.EnsurePauseButton(__instance);
	}
}
