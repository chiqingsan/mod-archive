using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(__c__DisplayClass21_0), "_OnEndDayButtonClick_b__0")]
internal static class AugHuntSleepPatch
{
	private static bool Prefix()
	{
		PlayerStore store = PlayerStore.Instance;
		if (store == null)
		{
			return true;
		}
		if (!AugHuntContract.ResolveSleep(delegate(string reason)
		{
			store.ExecuteGameOver(reason);
		}))
		{
			return true;
		}
		Instance log = Core.Log;
		if (log != null)
		{
			log.Msg("Aug hunt contract retaliation triggered the native 'aug' game over.");
		}
		return false;
	}
}
