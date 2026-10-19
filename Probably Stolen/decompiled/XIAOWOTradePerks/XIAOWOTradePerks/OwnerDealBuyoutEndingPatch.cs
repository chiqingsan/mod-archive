using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "ExecuteGameOver")]
internal static class OwnerDealBuyoutEndingPatch
{
	private static bool Prefix(PlayerStore __instance, string ending)
	{
		if (!OwnerDeal.ShouldSuppressEnding(__instance, ending))
		{
			return true;
		}
		Instance log = Core.Log;
		if (log != null)
		{
			log.Msg("Owner deal suppressed the cash-buyout ending; the run continues.");
		}
		return false;
	}
}
