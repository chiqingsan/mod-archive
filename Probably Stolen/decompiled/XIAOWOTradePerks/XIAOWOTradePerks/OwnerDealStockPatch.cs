using System;
using Il2Cpp;
using MelonLoader;

namespace XIAOWOTradePerks;

internal static class OwnerDealStockPatch
{
	internal static bool Prefix()
	{
		PlayerStore instance = PlayerStore.Instance;
		if (!OwnerDeal.IsCurrentOwnerVisit(instance))
		{
			return true;
		}
		try
		{
			return !OwnerDeal.TrySpawnStock(instance);
		}
		catch (Exception ex)
		{
			Instance log = Core.Log;
			if (log != null)
			{
				log.Error("Owner stock generation failed; using native wholesale stock instead: " + ex);
			}
			return true;
		}
	}
}
