using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "StartNewGame")]
internal static class NewGameCashflowPatch
{
	private static void Postfix()
	{
		PerkRegistry.EnsureRegistered();
		OperatingSurcharge.RecalculateKnownServices();
		StoreStation instance = StoreStation.instance;
		StoreEventManager val = ((instance != null) ? instance.storeEventManager : null);
		if (val != null)
		{
			DeterministicSchedule.EnsureCashflowCalendar(val);
		}
	}
}
