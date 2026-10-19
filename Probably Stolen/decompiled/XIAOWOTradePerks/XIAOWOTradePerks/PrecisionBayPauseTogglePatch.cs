using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(EscapeUIManager), "ToggleEscapeMenu")]
internal static class PrecisionBayPauseTogglePatch
{
	private static void Postfix(EscapeUIManager __instance)
	{
		if (__instance != null)
		{
			GameObject menuPanel = __instance.menuPanel;
			if (((menuPanel != null) ? new bool?(menuPanel.activeInHierarchy) : null).GetValueOrDefault())
			{
				PrecisionBayMode.EnsurePauseButton(__instance);
			}
		}
	}
}
