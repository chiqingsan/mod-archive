using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "PreLoadStore")]
internal static class LoadingProgressStorePreloadPatch
{
	private static void Prefix()
	{
		LoadingProgressOverlay.Advance(65, "正在准备店铺数据", "Preparing store data");
	}

	private static void Postfix()
	{
		LoadingProgressOverlay.Advance(75, "存档数据读取完成", "Save data read");
	}
}
