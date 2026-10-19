using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(ButtonElement), "OnSelect")]
internal static class ButtonElementOnSelectPatch
{
	private static void Postfix(ButtonElement __instance)
	{
		WaterTowerWindow.OnButtonSelected(__instance);
	}
}
