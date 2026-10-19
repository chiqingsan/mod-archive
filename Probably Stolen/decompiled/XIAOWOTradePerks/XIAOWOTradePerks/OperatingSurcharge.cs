using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Collections.Generic;

namespace XIAOWOTradePerks;

internal static class OperatingSurcharge
{
	private readonly struct AppliedCost
	{
		internal int BaseCost { get; }

		internal int AdjustedCost { get; }

		internal AppliedCost(int baseCost, int adjustedCost)
		{
			BaseCost = baseCost;
			AdjustedCost = adjustedCost;
		}
	}

	private const int NativeReferralBaseCost = 10;

	private static readonly Dictionary<long, AppliedCost> AppliedCosts = new Dictionary<long, AppliedCost>();

	private static int _saveDepth;

	internal static void BeforeNativeUpdate(StoreService service)
	{
		if (service != null)
		{
			long key = ((Il2CppObjectBase)service).Pointer.ToInt64();
			if (AppliedCosts.TryGetValue(key, out var value) && service.cost == value.AdjustedCost)
			{
				service.cost = value.BaseCost;
			}
			AppliedCosts.Remove(key);
		}
	}

	internal static void AfterNativeUpdate(StoreService service)
	{
		if (service != null && PerkRegistry.IsActive("xiaowo_trade_operating_surcharge") && TradeEconomyEffects.IsSurchargedService(service))
		{
			ApplyCurrentCost(service);
		}
	}

	internal static void RefreshKnownServices()
	{
		if (PerkRegistry.IsActive("xiaowo_trade_operating_surcharge"))
		{
			ForEachKnownService(ApplyCurrentCost);
		}
	}

	internal static void RecalculateKnownServices()
	{
		AppliedCosts.Clear();
		if (!PerkRegistry.IsActive("xiaowo_trade_operating_surcharge"))
		{
			return;
		}
		ForEachKnownService(delegate(StoreService service)
		{
			string a = CleanId(service);
			if (string.Equals(a, "BUY_REFERAL", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "SELL_REFERAL", StringComparison.OrdinalIgnoreCase))
			{
				service.cost = 10;
			}
			service.UpdateCost();
		});
	}

	internal static void RestoreKnownServices()
	{
		ForEachKnownService(RestoreCurrentCost);
	}

	internal static void DiscardTrackedServices()
	{
		AppliedCosts.Clear();
		_saveDepth = 0;
	}

	internal static void BeginSave()
	{
		if (_saveDepth++ == 0)
		{
			RestoreKnownServices();
		}
	}

	internal static void EndSave()
	{
		if (_saveDepth > 0)
		{
			_saveDepth--;
			if (_saveDepth == 0)
			{
				RefreshKnownServices();
			}
		}
	}

	private static void ApplyCurrentCost(StoreService service)
	{
		if (service != null)
		{
			long key = ((Il2CppObjectBase)service).Pointer.ToInt64();
			if (!AppliedCosts.TryGetValue(key, out var value) || service.cost != value.AdjustedCost)
			{
				int cost = service.cost;
				int adjustedCost = (service.cost = TradeEconomyEffects.ServiceCost(cost));
				AppliedCosts[key] = new AppliedCost(cost, adjustedCost);
			}
		}
	}

	private static void RestoreCurrentCost(StoreService service)
	{
		if (service != null)
		{
			long key = ((Il2CppObjectBase)service).Pointer.ToInt64();
			if (AppliedCosts.TryGetValue(key, out var value) && service.cost == value.AdjustedCost)
			{
				service.cost = value.BaseCost;
			}
			AppliedCosts.Remove(key);
		}
	}

	private static void ForEachKnownService(Action<StoreService> action)
	{
		HashSet<long> visited = new HashSet<long>();
		PlayerStore instance = PlayerStore.instance;
		List<StoreService> val = ((instance != null) ? instance.storeService : null);
		if (val != null)
		{
			for (int i = 0; i < val.Count; i++)
			{
				Visit(val[i]);
			}
		}
		Visit(StoreService.GetPowerService());
		Visit(StoreService.GetJanitorialService());
		Visit(StoreService.GetInsuranceService());
		Visit(StoreService.GetBuyReferalService());
		Visit(StoreService.GetSellReferalService());
		void Visit(StoreService service)
		{
			if (service != null && TradeEconomyEffects.IsSurchargedService(service))
			{
				long num = ((Il2CppObjectBase)service).Pointer.ToInt64();
				if (num != 0L && visited.Add(num))
				{
					action(service);
				}
			}
		}
	}

	private static string CleanId(StoreService service)
	{
		return (((service != null) ? service.id : null) ?? string.Empty).TrimEnd('\0');
	}
}
