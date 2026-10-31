using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(StoreClientList), "PlaceInventorInventory")]
internal static class WaterTowerPlaceInventorInventoryPatch
{
	private static void Postfix()
	{
		JacksonSell.OnPlaceInventorInventory();
	}
}
