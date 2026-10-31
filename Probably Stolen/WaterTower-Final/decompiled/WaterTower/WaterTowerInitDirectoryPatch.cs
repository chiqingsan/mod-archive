using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(FoodItemDirectory), "InitDirectory")]
internal static class WaterTowerInitDirectoryPatch
{
	private static void Postfix(FoodItemDirectory __instance)
	{
		ItemRegistry.RegisterInto(__instance);
		LazyPatches.Ensure();
	}
}
