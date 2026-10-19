using System;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using UnityEngine;

namespace XIAOWOTradePerks;

internal static class TradeEffects
{
	internal const string LuxuryClientId = "xiaowo_trade_luxury_buyer";

	internal const string StolenHeatTag = "STOLEN_VALUE_INT";

	private static int _playerSellingDepth;

	private static long _insuredVisitPointer;

	private static string _insuredRunWeek = string.Empty;

	internal static bool IsPlayerSelling => _playerSellingDepth > 0;

	internal static void BeginPlayerSale()
	{
		_playerSellingDepth++;
	}

	internal static void EndPlayerSale()
	{
		if (_playerSellingDepth > 0)
		{
			_playerSellingDepth--;
		}
	}

	internal static long ApplySaleModifiers(long value)
	{
		if (value <= 0)
		{
			return value;
		}
		double num = 1.0;
		if (IsPlayerSelling)
		{
			StoreStation instance = StoreStation.instance;
			if (PerkRegistry.IsActive("xiaowo_trade_blackout_broker") && ((instance != null) ? instance.storeEventManager : null) != null && instance.storeEventManager.IsBlackoutEventActive())
			{
				num *= 1.1;
			}
			PlayerStore instance2 = PlayerStore.Instance;
			object obj;
			if (instance2 == null)
			{
				obj = null;
			}
			else
			{
				StoreClientInstance currentClientInstance = instance2.currentClientInstance;
				obj = ((currentClientInstance != null) ? currentClientInstance.storeClient : null);
			}
			StoreClient val = (StoreClient)obj;
			if (val != null && string.Equals(val.identifier, "xiaowo_trade_luxury_buyer", StringComparison.Ordinal))
			{
				num *= 0.9;
			}
		}
		return Math.Max(1L, (long)Math.Round((double)value * num, MidpointRounding.AwayFromZero));
	}

	internal static long CashflowAdjusted(long value, bool boom)
	{
		if (value > 0)
		{
			return Math.Max(1L, (long)Math.Round((double)value * (boom ? 1.2 : 0.8), MidpointRounding.AwayFromZero));
		}
		return value;
	}

	internal static int AppraisalBonus(int actualFee)
	{
		if (actualFee > 0)
		{
			return Math.Max(1, (int)Math.Round((double)actualFee * 0.2, MidpointRounding.AwayFromZero));
		}
		return 0;
	}

	internal static int InsuredHeat(int heat)
	{
		return Math.Max(0, heat / 2);
	}

	internal static int SecurityTargetHeat(int heat)
	{
		if (heat > 0)
		{
			return Math.Max(1, (int)Math.Ceiling((double)heat * 1.25));
		}
		return heat;
	}

	internal static int ShortenedCooldown(int days)
	{
		return Math.Max(1, (int)Math.Floor((double)days * 0.75));
	}

	internal static int RundownAttractiveness(int attractiveness)
	{
		return Math.Max(0, attractiveness - 150);
	}

	internal static int RenovationCost(int cost)
	{
		return Math.Max(0, (int)Math.Ceiling((double)cost * 1.2));
	}

	internal static int ReturningBudget(int budget)
	{
		if (budget > 0)
		{
			return Math.Max(1, (int)Math.Floor((double)budget * 0.75));
		}
		return budget;
	}

	internal static void ApplyGreyInsurance(GameItem item)
	{
		if (item == null || !PerkRegistry.IsActive("xiaowo_trade_grey_insurance") || !StolenHelper.IsStolenItem(item))
		{
			return;
		}
		PlayerStore instance = PlayerStore.Instance;
		StoreClientInstance val = ((instance != null) ? instance.currentClientInstance : null);
		StoreClient val2 = ((val != null) ? val.storeClient : null);
		if (instance == null || val == null || val2 == null)
		{
			return;
		}
		string text = (instance.runID ?? "no-run") + "|" + DeterministicSchedule.CurrentWeek;
		if (!string.Equals(_insuredRunWeek, text, StringComparison.Ordinal))
		{
			_insuredRunWeek = text;
			_insuredVisitPointer = 0L;
		}
		string text2 = "XIAOWOTradePerks.GreyInsurance." + text;
		string @string = PlayerPrefs.GetString(text2, string.Empty);
		if (_insuredVisitPointer == 0L)
		{
			if (!string.IsNullOrEmpty(@string) && !string.Equals(@string, val2.identifier, StringComparison.Ordinal))
			{
				return;
			}
			_insuredVisitPointer = ((Il2CppObjectBase)val).Pointer.ToInt64();
			if (string.IsNullOrEmpty(@string))
			{
				PlayerPrefs.SetString(text2, val2.identifier ?? string.Empty);
				PlayerPrefs.Save();
			}
		}
		if (_insuredVisitPointer != ((Il2CppObjectBase)val).Pointer.ToInt64())
		{
			return;
		}
		TagState tagReadonly = item.GetTagReadonly("STOLEN_VALUE_INT");
		if (tagReadonly == null)
		{
			return;
		}
		int @int = tagReadonly.GetInt();
		int num = InsuredHeat(@int);
		if (num != @int)
		{
			StolenHelper.SetHeat(item, num);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Grey insurance reduced stolen heat " + @int + " -> " + num + ".");
			}
		}
	}

	internal static bool IsOrdinaryReturningClient(StoreClient client)
	{
		if (client == null || string.IsNullOrEmpty(client.identifier))
		{
			return false;
		}
		if (!client.CanReturn || client.isSecurity || client.isWanted)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(client.eventSourceId))
		{
			return false;
		}
		if (client.identifier.StartsWith("retired_", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		return StoreClientManager.GetCustomerTotalVisitedCount(client.identifier) > 0;
	}

	internal static bool IsRenovation(string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			return id.StartsWith("RENOVATION", StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}
}
