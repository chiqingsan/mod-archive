using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(EmporiumEntry), "Start")]
internal static class LoadingProgressSceneReadyPatch
{
	private static void Prefix()
	{
		LoadingProgressOverlay.Advance(55, "正在载入游戏场景", "Loading game scene");
	}

	private static void Postfix()
	{
		LoadingProgressOverlay.Complete();
	}
}
