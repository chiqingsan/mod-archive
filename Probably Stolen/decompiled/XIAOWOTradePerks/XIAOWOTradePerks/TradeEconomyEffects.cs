using System;
using System.Collections.Generic;
using Il2Cpp;

namespace XIAOWOTradePerks;

internal static class TradeEconomyEffects
{
	private static readonly HashSet<string> SurchargedServiceIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "POWER_SERVICE", "JANITORIAL_SERVICE", "INSURANCE_SERVICE", "BUY_REFERAL", "SELL_REFERAL" };

	internal const int StableSupplyDiscountPercent = 10;

	internal const int ReputationBonusPercent = 20;

	internal const int NegotiationBonusPoints = 10;

	internal const int NegotiationChanceCap = 95;

	internal const int OperatingSurchargePercent = 20;

	[ThreadStatic]
	private static int _playerBuyingDepth;

	internal static bool IsPlayerBuying => _playerBuyingDepth > 0;

	internal static void BeginPlayerPurchase()
	{
		_playerBuyingDepth++;
	}

	internal static void EndPlayerPurchase()
	{
		if (_playerBuyingDepth > 0)
		{
			_playerBuyingDepth--;
		}
	}

	internal static long StableSupplyPrice(long value)
	{
		if (value > 0)
		{
			return Math.Max(1L, (long)Math.Round((double)value * 0.9, MidpointRounding.AwayFromZero));
		}
		return value;
	}

	internal static double ReputationGain(double value)
	{
		if (!(value > 0.0))
		{
			return value;
		}
		return value * 1.2;
	}

	internal static int NegotiationChance(int value)
	{
		return Math.Min(95, Math.Max(0, value) + 10);
	}

	internal static int ServiceCost(int value)
	{
		if (value > 0)
		{
			return Math.Max(1, (int)Math.Ceiling((double)value * 1.2));
		}
		return value;
	}

	internal static bool IsOrdinarySupplier(StoreClient client)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		if (client == null || (int)client.clientIntent != 2 || !client.CanReturn || client.isSecurity || client.isWanted || !string.IsNullOrEmpty(client.eventSourceId))
		{
			return false;
		}
		string text = client.identifier ?? string.Empty;
		if (!text.StartsWith("xiaowo_", StringComparison.OrdinalIgnoreCase))
		{
			return !text.StartsWith("retired_", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	internal static bool IsOrdinaryTradeClient(StoreClient client)
	{
		if (client == null || !client.CanReturn || client.isSecurity || client.isWanted || !string.IsNullOrEmpty(client.eventSourceId))
		{
			return false;
		}
		string text = client.identifier ?? string.Empty;
		if (!text.StartsWith("xiaowo_", StringComparison.OrdinalIgnoreCase))
		{
			return !text.StartsWith("retired_", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	internal static bool IsSurchargedService(StoreService service)
	{
		if (service != null)
		{
			return SurchargedServiceIds.Contains((service.id ?? string.Empty).TrimEnd('\0'));
		}
		return false;
	}
}
