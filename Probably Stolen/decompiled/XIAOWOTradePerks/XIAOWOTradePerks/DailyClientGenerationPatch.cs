using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreClientManager), "HandleNormalClient")]
internal static class DailyClientGenerationPatch
{
	private static void Postfix(StoreClientManager __instance)
	{
		DeterministicSchedule.HandleDailyClients(__instance);
	}
}
