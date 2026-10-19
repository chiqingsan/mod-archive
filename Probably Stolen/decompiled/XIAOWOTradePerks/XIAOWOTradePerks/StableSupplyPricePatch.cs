using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(GameItem), "GetNegociatedValue")]
internal static class StableSupplyPricePatch
{
	private static void Postfix(ref long __result)
	{
		if (TradeEconomyEffects.IsPlayerBuying && PerkRegistry.IsActive("xiaowo_trade_stable_supply"))
		{
			PlayerStore instance = PlayerStore.Instance;
			object client;
			if (instance == null)
			{
				client = null;
			}
			else
			{
				StoreClientInstance currentClientInstance = instance.currentClientInstance;
				client = ((currentClientInstance != null) ? currentClientInstance.storeClient : null);
			}
			if (TradeEconomyEffects.IsOrdinarySupplier((StoreClient)client))
			{
				__result = TradeEconomyEffects.StableSupplyPrice(__result);
			}
		}
	}
}
