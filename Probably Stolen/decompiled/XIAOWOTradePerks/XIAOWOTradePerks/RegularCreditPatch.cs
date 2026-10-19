using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreClientManager), "TrackClient")]
internal static class RegularCreditPatch
{
	private static readonly HashSet<long> Processed = new HashSet<long>();

	private static void Prefix(StoreClient storeClient)
	{
		if (!PerkRegistry.IsActive("xiaowo_trade_regular_credit") || !TradeEffects.IsOrdinaryReturningClient(storeClient))
		{
			return;
		}
		long item = ((Il2CppObjectBase)storeClient).Pointer.ToInt64();
		if (Processed.Add(item))
		{
			int budget = storeClient.GetBudget();
			if (budget > 0)
			{
				storeClient.OverrideBudget(TradeEffects.ReturningBudget(budget));
			}
		}
	}
}
