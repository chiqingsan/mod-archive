using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

[HarmonyPatch(typeof(StoreClientList), "PlaceInventorInventory")]
internal static class StoreClientListPlaceInventorInventoryPatch
{
	private static void Postfix()
	{
		JacksonSell.OnPlaceInventorInventory();
	}
}
