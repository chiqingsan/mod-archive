using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(MainMenuUIController), "StartNewGameClick")]
internal static class LoadingProgressSceneRequestPatch
{
	private static void Prefix()
	{
		if (LoadingProgressOverlay.IsActive)
		{
			LoadingProgressOverlay.Advance(25, "正在读取存档", "Reading save data");
		}
	}
}
