using System;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace XIAOWOTradePerks;

internal static class DeterministicSchedule
{
	private const string CashflowPrefix = "xiaowo_trade_cashflow_";

	internal const string AppraisalCertificateId = "cert1";

	internal const string CashflowAllItemsType = "XIAOWO_ALL_ITEMS";

	internal const int CashflowDurationDays = 1;

	private static string _lastDailyGenerationKey = string.Empty;

	internal static int CurrentDay => Math.Max(0, StoreStation.GetDayCounter());

	internal static int CurrentWeek => Math.Max(0, (CurrentDay - 1) / 7);

	internal static bool IsCashflowWindow(out bool boom)
	{
		int currentDay = CurrentDay;
		int currentWeek = CurrentWeek;
		int cashflowStartDay = GetCashflowStartDay(currentWeek);
		boom = GetCashflowBoom(currentWeek);
		return currentDay == cashflowStartDay;
	}

	internal static void HandleDailyClients(StoreClientManager manager)
	{
		if (manager == null || PlayerStore.Instance == null)
		{
			return;
		}
		string text = GetRunKey() + "|" + CurrentDay;
		if (!string.Equals(_lastDailyGenerationKey, text, StringComparison.Ordinal))
		{
			_lastDailyGenerationKey = text;
			if (PerkRegistry.IsActive("xiaowo_trade_appraisal_sign"))
			{
				TryAddAppraisalClient(manager);
			}
			if (PerkRegistry.IsActive("xiaowo_trade_luxury_appointment"))
			{
				TryAddLuxuryClient(manager);
			}
		}
	}

	internal static void EnsureCashflowCalendar(StoreEventManager manager)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		if (manager == null || !PerkRegistry.IsActive("xiaowo_trade_cashflow_gambler"))
		{
			return;
		}
		PlayerStore instance = PlayerStore.Instance;
		if (string.IsNullOrEmpty((instance != null) ? instance.runID : null))
		{
			return;
		}
		int currentWeek = CurrentWeek;
		int cashflowStartDay = GetCashflowStartDay(currentWeek);
		bool cashflowBoom = GetCashflowBoom(currentWeek);
		string id = "xiaowo_trade_cashflow_" + GetRunHash().ToString("x8") + "_" + currentWeek + (cashflowBoom ? "_boom" : "_bust");
		StoreEvent val = FindEvent(manager.activeEvents, id) ?? FindEvent(manager.futurEvents, id);
		if (val != null)
		{
			ConfigureCashflowEvent(val, id, cashflowStartDay, cashflowBoom);
		}
		else if (CurrentDay <= cashflowStartDay)
		{
			StoreEvent val2 = new StoreEvent();
			val2.InitNormalEvent();
			ConfigureCashflowEvent(val2, id, cashflowStartDay, cashflowBoom);
			int num = cashflowStartDay - CurrentDay;
			if (num <= 0)
			{
				manager.ActivateEvent(val2, false, true);
			}
			else
			{
				manager.QueueFuturEvent(val2, num, true);
			}
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Cashflow calendar scheduled: " + val2.displayName + " day " + cashflowStartDay);
			}
		}
	}

	private static void ConfigureCashflowEvent(StoreEvent storeEvent, string id, int startDay, bool boom)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		storeEvent.identifier = id;
		ApplyCashflowText(storeEvent, boom);
		storeEvent.eventType = (EventType)0;
		storeEvent.eventArea = (EventArea)2;
		storeEvent.duration = 1;
		storeEvent.totalDuration = 1;
		storeEvent.startDay = startDay;
		storeEvent.isPermanent = false;
		storeEvent.isHidden = false;
		storeEvent.isDurationVisible = true;
		storeEvent.IsKnown = true;
		storeEvent.isHiddenFromNewsPaper = false;
		storeEvent.negociationDatas.Clear();
		NegociationData val = new NegociationData("XIAOWO_ALL_ITEMS", boom ? 20 : (-20), GetCashflowDescription(boom), storeEvent.displayName);
		val.identifier = id + "_price";
		storeEvent.negociationDatas.Add(val);
	}

	internal static string GetCashflowEventName(string id)
	{
		if (string.IsNullOrEmpty(id) || !id.StartsWith("xiaowo_trade_cashflow_", StringComparison.Ordinal))
		{
			return null;
		}
		return GetCashflowName(IsBoomId(id));
	}

	internal static bool IsCashflowEvent(string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			return id.StartsWith("xiaowo_trade_cashflow_", StringComparison.Ordinal);
		}
		return false;
	}

	internal static string GetCashflowPriceEffect(string id)
	{
		if (!IsCashflowEvent(id))
		{
			return null;
		}
		if (!IsBoomId(id))
		{
			return ModLocalization.Select("-20% 所有物品", "-20% All Items");
		}
		return ModLocalization.Select("+20% 所有物品", "+20% All Items");
	}

	internal static string GetCashflowFeatureDisplay(string id)
	{
		if (!IsCashflowEvent(id))
		{
			return null;
		}
		bool boom = IsBoomId(id);
		return GetCashflowName(boom) + ModLocalization.Select("：", ": ") + GetCashflowDescription(boom);
	}

	internal static void RefreshLocalizedText(StoreEventManager manager)
	{
		if (manager != null)
		{
			RefreshLocalizedText(manager.activeEvents);
			RefreshLocalizedText(manager.futurEvents);
		}
	}

	private static void RefreshLocalizedText(List<StoreEvent> events)
	{
		if (events == null)
		{
			return;
		}
		for (int i = 0; i < events.Count; i++)
		{
			StoreEvent val = events[i];
			if (IsCashflowEvent((val != null) ? val.identifier : null))
			{
				ApplyCashflowText(val, IsBoomId(val.identifier));
			}
		}
	}

	private static void ApplyCashflowText(StoreEvent storeEvent, bool boom)
	{
		storeEvent.displayName = GetCashflowName(boom);
		storeEvent.newsName = storeEvent.displayName;
		storeEvent.newsDescription = ModLocalization.Select(boom ? "市场繁荣：所有物品价格上涨20%。" : "市场紧缩：所有物品价格下降20%。", boom ? "Market boom: all item prices increase by 20%." : "Market contraction: all item prices decrease by 20%.");
		if (storeEvent.negociationDatas == null)
		{
			return;
		}
		for (int i = 0; i < storeEvent.negociationDatas.Count; i++)
		{
			NegociationData val = storeEvent.negociationDatas[i];
			if (val != null && string.Equals(val.itemType, "XIAOWO_ALL_ITEMS", StringComparison.Ordinal))
			{
				val.publicDisplay = GetCashflowDescription(boom);
				val.reason = storeEvent.displayName;
			}
		}
	}

	private static string GetCashflowName(bool boom)
	{
		return ModLocalization.Select(boom ? "繁荣" : "紧缩", boom ? "Boom" : "Contraction");
	}

	private static string GetCashflowDescription(bool boom)
	{
		return ModLocalization.Select(boom ? "所有物品价格上涨20%。" : "所有物品价格下降20%。", boom ? "All item prices increase by 20%." : "All item prices decrease by 20%.");
	}

	private static bool IsBoomId(string id)
	{
		return id?.Contains("_boom", StringComparison.Ordinal) ?? false;
	}

	private static void TryAddAppraisalClient(StoreClientManager manager)
	{
		PlayerStore instance = PlayerStore.Instance;
		if (instance == null)
		{
			return;
		}
		bool certificateDisplayed = ShowcaseHelper.IsShowcaseContainItemById("cert1");
		int roll = PositiveModulo(StableHash(GetRunKey() + "|appraisal|" + CurrentDay), 100);
		if (!CanScheduleAppraisalClient(instance.isAppraisalIntroduced, certificateDisplayed, roll) || ContainsEventSource(manager.clientStack, "xiaowo_trade_appraisal_sign"))
		{
			return;
		}
		StoreClient val = StoreClientListAppraisal.CreateAppraisalRandom();
		if (val != null)
		{
			val.eventSourceId = "xiaowo_trade_appraisal_sign";
			manager.TryAddClient(val, false);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Appraisal sign scheduled an extra appraisal client on day " + CurrentDay + ".");
			}
		}
	}

	internal static bool CanScheduleAppraisalClient(bool appraisalIntroduced, bool certificateDisplayed, int roll)
	{
		if (appraisalIntroduced && certificateDisplayed && roll >= 0)
		{
			return roll < 30;
		}
		return false;
	}

	private static void TryAddLuxuryClient(StoreClientManager manager)
	{
		int currentWeek = CurrentWeek;
		int num = PositiveModulo(StableHash(GetRunKey() + "|luxury|" + currentWeek), 7) + 1;
		if ((CurrentDay - 1) % 7 + 1 != num || ContainsIdentifier(manager.clientStack, "xiaowo_trade_luxury_buyer"))
		{
			return;
		}
		StoreClient val = StoreClientList.CreateUpperLowerVisitorLuxury();
		if (val != null)
		{
			val.identifier = "xiaowo_trade_luxury_buyer";
			val.eventSourceId = "xiaowo_trade_luxury_buyer";
			val.CanReturn = false;
			val.CanReturnAsBuyer = false;
			val.OverrideBudget(GetAverageOrdinaryBudget(manager) * 3 / 2);
			manager.TryAddClient(val, true);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Luxury appointment scheduled on day " + CurrentDay + " with budget " + val.GetBudget() + ".");
			}
		}
	}

	private static int GetAverageOrdinaryBudget(StoreClientManager manager)
	{
		long num = 0L;
		int num2 = 0;
		List<StoreClient> clientStack = manager.clientStack;
		if (clientStack != null)
		{
			for (int i = 0; i < clientStack.Count; i++)
			{
				StoreClient val = clientStack[i];
				if (val != null && !val.isSecurity && !val.isWanted && string.IsNullOrEmpty(val.eventSourceId))
				{
					int budget = val.GetBudget();
					if (budget > 0)
					{
						num += budget;
						num2++;
					}
				}
			}
		}
		if (num2 > 0)
		{
			return Math.Max(1, (int)(num / num2));
		}
		StoreClient val2 = StoreClientList.CreateUpperLowerVisitorLuxury();
		if (val2 != null)
		{
			return Math.Max(1, val2.GetBudget());
		}
		return 250;
	}

	private static int GetCashflowStartDay(int week)
	{
		int num = PositiveModulo(StableHash(GetRunKey() + "|cashflow-day|" + week), 7) + 1;
		return week * 7 + num;
	}

	private static bool GetCashflowBoom(int week)
	{
		return (StableHash(GetRunKey() + "|cashflow-kind|" + week) & 1) == 0;
	}

	private static string GetRunKey()
	{
		PlayerStore instance = PlayerStore.Instance;
		string text = ((instance != null) ? instance.runID : null);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return "no-run";
	}

	private static int GetRunHash()
	{
		return StableHash(GetRunKey());
	}

	private static int StableHash(string value)
	{
		int num = -2128831035;
		for (int i = 0; i < value.Length; i++)
		{
			num = (num ^ value[i]) * 16777619;
		}
		return num;
	}

	private static int PositiveModulo(int value, int modulo)
	{
		int num = value % modulo;
		if (num >= 0)
		{
			return num;
		}
		return num + modulo;
	}

	private static StoreEvent FindEvent(List<StoreEvent> events, string id)
	{
		if (events == null)
		{
			return null;
		}
		for (int i = 0; i < events.Count; i++)
		{
			if (events[i] != null && string.Equals(events[i].identifier, id, StringComparison.Ordinal))
			{
				return events[i];
			}
		}
		return null;
	}

	private static bool ContainsIdentifier(List<StoreClient> clients, string id)
	{
		if (clients == null)
		{
			return false;
		}
		for (int i = 0; i < clients.Count; i++)
		{
			if (clients[i] != null && string.Equals(clients[i].identifier, id, StringComparison.Ordinal))
			{
				return true;
			}
		}
		return false;
	}

	private static bool ContainsEventSource(List<StoreClient> clients, string source)
	{
		if (clients == null)
		{
			return false;
		}
		for (int i = 0; i < clients.Count; i++)
		{
			if (clients[i] != null && string.Equals(clients[i].eventSourceId, source, StringComparison.Ordinal))
			{
				return true;
			}
		}
		return false;
	}
}
