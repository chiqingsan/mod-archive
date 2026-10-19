using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "LoadGame")]
internal static class LoadingProgressStoreRestorePatch
{
	private static void Prefix()
	{
		LoadingProgressOverlay.Advance(80, "正在恢复店铺、物品和事件", "Restoring store, items and events");
	}

	private static void Postfix()
	{
		LoadingProgressOverlay.Advance(90, "正在初始化 MOD 数据", "Initializing mod data");
	}
}
