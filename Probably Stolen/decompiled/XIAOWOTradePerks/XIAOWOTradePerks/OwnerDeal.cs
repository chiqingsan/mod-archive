using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace XIAOWOTradePerks;

internal static class OwnerDeal
{
	internal const string ClientId = "landlord";

	internal const string MerchantClientId = "merchant";

	internal const string RestrictedNeuralId = "system_capped_neural_core";

	internal const string EvidenceBoxId = "evidence_box";

	internal const string SecurityBoxId = "sec_box";

	internal const string MedicalBoxId = "med_box";

	internal const string EngineeringBoxId = "eng_box";

	internal const string ServiceBoxId = "service_box";

	internal const int DiscountPercent = 15;

	private const string VisitStatePrefix = "XIAOWOTradePerks.OwnerDeal.PairV3.";

	private const string ExcludedUncappedNeuralId = "system_uncapped_neural_core";

	private static readonly List<string> RegularPool = new List<string>();

	private static string _lastScheduledKey = string.Empty;

	private static bool _poolBuilt;

	private static long _merchantClientPointer;

	private static long _ownerClientPointer;

	private static readonly HashSet<long> CashBuyoutChoiceDialogues = new HashSet<long>();

	private static readonly List<Action> CashBuyoutActions = new List<Action>();

	internal static bool IsEnabledAndOwned(PlayerStore store)
	{
		if (store != null && store.IsPropertyPaid)
		{
			return PerkRegistry.IsActive("xiaowo_trade_owner_deal");
		}
		return false;
	}

	internal static bool ShouldSuppressEnding(PlayerStore store, string ending)
	{
		if (IsEnabledAndOwned(store))
		{
			return string.Equals(ending, "buyout", StringComparison.Ordinal);
		}
		return false;
	}

	internal static bool CanOfferCashBuyout(PlayerStore store)
	{
		if (store == null)
		{
			return false;
		}
		return OwnerDealRules.CanOfferCashBuyout(PerkRegistry.IsActive("xiaowo_trade_owner_deal"), store.IsPropertyPaid, store.IsPayingRent(), store.GetCash(), store.GetStoreBuyPrice(false));
	}

	internal static void ScheduleWeeklyVisitors(StoreClientManager manager)
	{
		if (manager == null)
		{
			return;
		}
		PlayerStore instance = PlayerStore.Instance;
		if (!IsEnabledAndOwned(instance) || StoreStation.instance == null)
		{
			return;
		}
		int currentDay = DeterministicSchedule.CurrentDay;
		if (!OwnerDealRules.IsFormerRentDay(currentDay))
		{
			return;
		}
		string text = (string.IsNullOrEmpty(instance.runID) ? "no-run" : instance.runID);
		string text2 = text + "|" + currentDay;
		if (string.Equals(_lastScheduledKey, text2, StringComparison.Ordinal))
		{
			return;
		}
		int num = OwnerDealRules.DecodeVisitState(PlayerPrefs.GetInt("XIAOWOTradePerks.OwnerDeal.PairV3." + text, -1), currentDay);
		if (num == 3)
		{
			_lastScheduledKey = text2;
			return;
		}
		bool flag = IsClientPresent(instance, manager, "merchant");
		bool flag2 = IsClientPresent(instance, manager, "landlord");
		bool flag3 = false;
		bool flag4 = false;
		try
		{
			if (OwnerDealRules.NeedsVisitor(num, 1, flag))
			{
				StoreClient val = CreateMerchantClient();
				if (val != null)
				{
					manager.TryAddClient(val, true);
					flag3 = ContainsClient(manager.clientStack, "merchant");
				}
			}
			if (OwnerDealRules.NeedsVisitor(num, 2, flag2))
			{
				StoreClient val2 = CreateLandlordClient();
				if (val2 != null)
				{
					manager.TryAddClient(val2, true);
					flag4 = ContainsClient(manager.clientStack, "landlord");
				}
			}
		}
		catch (Exception ex)
		{
			Instance log = Core.Log;
			if (log != null)
			{
				log.Error("Owner deal weekly visitor scheduling failed: " + ex);
			}
			return;
		}
		flag = flag || flag3 || IsClientPresent(instance, manager, "merchant");
		flag2 = flag2 || flag4 || IsClientPresent(instance, manager, "landlord");
		bool num2 = (num & 1) != 0 || flag;
		bool flag5 = (num & 2) != 0 || flag2;
		if (!num2 || !flag5)
		{
			Instance log2 = Core.Log;
			if (log2 != null)
			{
				log2.Warning("Owner deal could not schedule both weekly visitors on day " + currentDay + "; it will retry.");
			}
			return;
		}
		_lastScheduledKey = text2;
		Instance log3 = Core.Log;
		if (log3 != null)
		{
			log3.Msg("Owner deal weekly visitors scheduled on former rent day " + currentDay + " (merchant=" + flag3 + ", landlord=" + flag4 + ", completedMask=" + num + ").");
		}
	}

	internal static StoreClient CreateMerchantClient()
	{
		StoreClient obj = StoreClientList.CreateMerchant();
		_merchantClientPointer = ((obj != null) ? ((Il2CppObjectBase)obj).Pointer.ToInt64() : 0);
		return obj;
	}

	internal static StoreClient CreateLandlordClient()
	{
		StoreClient val = StoreClientList.CreateLandlordClient();
		StoreClient val2 = StoreClientUniqueList.LandlordWholesale();
		if (val == null || ((val2 != null) ? val2.mainDialogue : null) == null)
		{
			return null;
		}
		val.mainDialogue = val2.mainDialogue;
		ApplyPostBuyoutVendorState(val);
		ApplyDialogue(val);
		_ownerClientPointer = ((Il2CppObjectBase)val).Pointer.ToInt64();
		return val;
	}

	internal static void ApplyPostBuyoutVendorState(StoreClient client)
	{
		if (client != null)
		{
			client.clientIntent = (ClientIntent)2;
			client.dismissable = true;
			client.arrestable = false;
			client.CanReturn = false;
			client.CanReturnAsBuyer = false;
			client.SetWholesale(15);
		}
	}

	internal static IReadOnlyList<string> BuildStockPlan(string runId, int day)
	{
		EnsureRegularPool();
		Random random = new Random(OwnerDealRules.GetVisitSeed(runId, day));
		OwnerStockRoll ownerStockRoll = OwnerDealRules.Roll(random);
		List<string> list = new List<string>(ownerStockRoll.Count);
		if (ownerStockRoll.RestrictedNeural)
		{
			list.Add("system_capped_neural_core");
		}
		int num = Math.Max(ownerStockRoll.Count, list.Count);
		if (RegularPool.Count == 0)
		{
			return list;
		}
		List<string> list2 = new List<string>(RegularPool);
		while (list.Count < num && list2.Count > 0)
		{
			int index = random.Next(list2.Count);
			list.Add(list2[index]);
			list2.RemoveAt(index);
		}
		return list;
	}

	internal static bool IsEligibleRegularItem(GameItem item)
	{
		if (item != null && item.unitValue > 0)
		{
			if (!item.IsTag("MODULE_TAG") && !item.IsTag("CONTRABAND_ITEM_TAG"))
			{
				return item.IsTag("LUXURY_ITEM");
			}
			return true;
		}
		return false;
	}

	internal static bool IsCurrentOwnerVisit(PlayerStore store)
	{
		object obj;
		if (store == null)
		{
			obj = null;
		}
		else
		{
			StoreClientInstance currentClientInstance = store.currentClientInstance;
			obj = ((currentClientInstance != null) ? currentClientInstance.storeClient : null);
		}
		StoreClient val = (StoreClient)obj;
		if (IsEnabledAndOwned(store) && val != null && string.Equals(val.identifier, "landlord", StringComparison.Ordinal))
		{
			if (_ownerClientPointer == 0L || ((Il2CppObjectBase)val).Pointer.ToInt64() != _ownerClientPointer)
			{
				return IsRestoredOwnerVendor(val);
			}
			return true;
		}
		return false;
	}

	internal static bool IsRestoredOwnerVendor(StoreClient client)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		if (client != null && string.Equals(client.identifier, "landlord", StringComparison.Ordinal) && (int)client.clientIntent == 2 && client.dismissable)
		{
			return client.isOfferWholesale;
		}
		return false;
	}

	internal static void OnGameLoading()
	{
		_lastScheduledKey = string.Empty;
		_merchantClientPointer = 0L;
		_ownerClientPointer = 0L;
		CashBuyoutChoiceDialogues.Clear();
		CashBuyoutActions.Clear();
	}

	internal static void AttachCashBuyoutChoice(StoreClient client)
	{
		if (!OwnerDealCompatibility.UsesCustomCashBuyoutChoice)
		{
			return;
		}
		StoreClient obj = client;
		if (((obj != null) ? obj.mainDialogue : null) == null)
		{
			return;
		}
		Dialogue mainDialogue = client.mainDialogue;
		Action nativeStartAction = mainDialogue.startAction;
		Action val = DelegateSupport.ConvertDelegate<Action>((Delegate)(Action)delegate
		{
			Action obj2 = nativeStartAction;
			if (obj2 != null)
			{
				obj2.Invoke();
			}
			TryAddCashBuyoutChoice(client);
		});
		CashBuyoutActions.Add(val);
		mainDialogue.SetStartAction(val);
	}

	internal static bool TryAddCashBuyoutChoice(StoreClient client)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		PlayerStore store = PlayerStore.Instance;
		Dialogue val = ((client != null) ? client.mainDialogue : null);
		if (val == null || !CanOfferCashBuyout(store))
		{
			return false;
		}
		long item = ((Il2CppObjectBase)val).Pointer.ToInt64();
		if (!CashBuyoutChoiceDialogues.Add(item))
		{
			return false;
		}
		int storeBuyPrice = store.GetStoreBuyPrice(false);
		Dialogue val2 = new Dialogue().SetText("PLAYER", ModLocalization.Select("我用 " + storeBuyPrice + " 信用点一次买断这间店。", "I will buy the shop outright for " + storeBuyPrice + " Credits."));
		Dialogue obj = val2.NextDialogue().SetText(client.displayName, ModLocalization.Select("成交。从现在起，这间店归你了。", "Deal. From now on, the shop is yours."));
		Action val3 = DelegateSupport.ConvertDelegate<Action>((Delegate)(Action)delegate
		{
			DialogUIManager instance = DialogUIManager.Instance;
			if (instance != null)
			{
				instance.CurrentClientLeave(1f);
			}
		});
		CashBuyoutActions.Add(val3);
		obj.SetEndAction(val3);
		Action val4 = DelegateSupport.ConvertDelegate<Action>((Delegate)(Action)delegate
		{
			CompleteCashBuyout(store);
		});
		CashBuyoutActions.Add(val4);
		OwnerDealCompatibility.AddChoice(val, DirectoryMaster.Item("$", true), "xiaowo cash buyout choice", string.Empty, val2, val4);
		return true;
	}

	internal static bool CompleteCashBuyout(PlayerStore store)
	{
		if (!CanOfferCashBuyout(store))
		{
			return false;
		}
		int storeBuyPrice = store.GetStoreBuyPrice(false);
		store.ModCash(-storeBuyPrice, true);
		store.propertyBoughtAt = storeBuyPrice;
		store.mortgageWeekLeft = 0;
		store.mortgageWeeklyPayment = 0;
		store.isPayingMortgage = false;
		store.IsPropertyPaid = true;
		store.OnStoreOwned();
		Instance log = Core.Log;
		if (log != null)
		{
			log.Msg("Owner deal cash buyout completed for " + storeBuyPrice + " Credits; the run continues.");
		}
		return true;
	}

	internal static int CaptureCurrentVisitor(PlayerStore store)
	{
		if (!IsEnabledAndOwned(store) || !OwnerDealRules.IsFormerRentDay(DeterministicSchedule.CurrentDay))
		{
			return 0;
		}
		StoreClientInstance currentClientInstance = store.currentClientInstance;
		StoreClient val = ((currentClientInstance != null) ? currentClientInstance.storeClient : null);
		if (val == null)
		{
			return 0;
		}
		long num = ((Il2CppObjectBase)val).Pointer.ToInt64();
		if (string.Equals(val.identifier, "merchant", StringComparison.Ordinal) && (_merchantClientPointer == 0L || num == _merchantClientPointer))
		{
			return 1;
		}
		if (string.Equals(val.identifier, "landlord", StringComparison.Ordinal) && (_ownerClientPointer == 0L || num == _ownerClientPointer))
		{
			return 2;
		}
		return 0;
	}

	internal static void MarkVisitorCompleted(int visitorBit)
	{
		if (visitorBit == 0)
		{
			return;
		}
		PlayerStore instance = PlayerStore.Instance;
		int currentDay = DeterministicSchedule.CurrentDay;
		if (!IsEnabledAndOwned(instance) || !OwnerDealRules.IsFormerRentDay(currentDay))
		{
			return;
		}
		string text = (string.IsNullOrEmpty(instance.runID) ? "no-run" : instance.runID);
		string text2 = "XIAOWOTradePerks.OwnerDeal.PairV3." + text;
		int num = OwnerDealRules.DecodeVisitState(PlayerPrefs.GetInt(text2, -1), currentDay);
		int num2 = num | visitorBit;
		if (num2 != num)
		{
			PlayerPrefs.SetInt(text2, OwnerDealRules.EncodeVisitState(currentDay, num2));
			PlayerPrefs.Save();
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Owner deal visitor completed on day " + currentDay + " (mask=" + num2 + ").");
			}
		}
	}

	internal static bool TrySpawnStock(PlayerStore store)
	{
		if (!IsCurrentOwnerVisit(store))
		{
			return false;
		}
		IReadOnlyList<string> readOnlyList = BuildStockPlan(store.runID, DeterministicSchedule.CurrentDay);
		int num = 0;
		foreach (string item in readOnlyList)
		{
			try
			{
				GameItem val = CreateSaleItem(item);
				if (val != null)
				{
					store.AddDirectSellingItemToTable(val, false, false, false, 0);
					num++;
				}
			}
			catch (Exception ex)
			{
				Instance log = Core.Log;
				if (log != null)
				{
					log.Warning("Owner stock item failed (" + item + "): " + ex.Message);
				}
			}
		}
		Instance log2 = Core.Log;
		if (log2 != null)
		{
			log2.Msg("Owner deal stocked " + num + "/" + readOnlyList.Count + " items.");
		}
		return num > 0;
	}

	internal static GameItem CreateSaleItem(string id)
	{
		return (GameItem)(id switch
		{
			"evidence_box" => PreBuiltItemHelper.LootCrateEvidence(), 
			"sec_box" => PreBuiltItemHelper.LootCrateSecurity(), 
			"med_box" => PreBuiltItemHelper.LootCrateMedical(), 
			"eng_box" => PreBuiltItemHelper.LootCrateEngineering(), 
			"service_box" => PreBuiltItemHelper.LootCrateService(), 
			_ => DirectoryMaster.Item(id, false), 
		});
	}

	internal static int GetRegularPoolCount()
	{
		EnsureRegularPool();
		return RegularPool.Count;
	}

	internal static bool RegularPoolContains(string id)
	{
		EnsureRegularPool();
		return RegularPool.Contains(id);
	}

	private static void EnsureRegularPool()
	{
		if (_poolBuilt)
		{
			return;
		}
		_poolBuilt = true;
		RegularPool.Clear();
		List<string> identifierList = DirectoryMaster.GetIdentifierList<GameItem>((string)null);
		if (identifierList == null)
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
		for (int i = 0; i < identifierList.Count; i++)
		{
			string text = identifierList[i];
			if (string.IsNullOrEmpty(text) || string.Equals(text, "system_capped_neural_core", StringComparison.Ordinal) || string.Equals(text, "system_uncapped_neural_core", StringComparison.Ordinal) || !hashSet.Add(text))
			{
				continue;
			}
			try
			{
				if (IsEligibleRegularItem(DirectoryMaster.Item(text, false)))
				{
					RegularPool.Add(text);
				}
			}
			catch (Exception ex)
			{
				Instance log = Core.Log;
				if (log != null)
				{
					log.Warning("Owner stock scan skipped " + text + ": " + ex.Message);
				}
			}
		}
		Instance log2 = Core.Log;
		if (log2 != null)
		{
			log2.Msg("Owner deal regular stock pool: " + RegularPool.Count + " items.");
		}
	}

	private static void ApplyDialogue(StoreClient client)
	{
		string[] array = new string[4]
		{
			ModLocalization.Select("这间店已经归你，今天我不是来收租的。", "The shop is yours now. I am not here to collect rent today."),
			ModLocalization.Select("我从旧渠道里挑了几件货：模组、违禁品，或者真正值钱的奢侈品。", "I picked up a few things through old contacts: modules, contraband, and some genuinely valuable luxuries."),
			ModLocalization.Select("东西来路我不多问，你也别问。看中了就按老熟人的价格拿走。", "I do not ask where they came from, and neither should you. Take what you want at the old-friend price."),
			ModLocalization.Select("货就在桌上。下个租金日，我再看看能替你弄到什么。", "The goods are on the table. On the next old rent day, I will see what else I can find for you.")
		};
		Dialogue val = client.mainDialogue;
		for (int i = 0; i < array.Length; i++)
		{
			if (val == null)
			{
				break;
			}
			val.SetText(client.displayName, array[i]);
			val = val.nextDialogue;
		}
	}

	internal static void RefreshLocalizedDialogue(StoreClientManager manager)
	{
		if (manager == null)
		{
			return;
		}
		PlayerStore instance = PlayerStore.instance;
		object obj;
		if (instance == null)
		{
			obj = null;
		}
		else
		{
			StoreClientInstance currentClientInstance = instance.currentClientInstance;
			obj = ((currentClientInstance != null) ? currentClientInstance.storeClient : null);
		}
		StoreClient val = (StoreClient)obj;
		if (string.Equals((val != null) ? val.identifier : null, "landlord", StringComparison.Ordinal))
		{
			ApplyDialogue(val);
		}
		List<StoreClient> clientStack = manager.clientStack;
		if (clientStack == null)
		{
			return;
		}
		for (int i = 0; i < clientStack.Count; i++)
		{
			StoreClient obj2 = clientStack[i];
			if (string.Equals((obj2 != null) ? obj2.identifier : null, "landlord", StringComparison.Ordinal))
			{
				ApplyDialogue(clientStack[i]);
			}
		}
	}

	private static bool ContainsClient(List<StoreClient> clients, string id)
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

	private static bool IsClientPresent(PlayerStore store, StoreClientManager manager, string id)
	{
		object a;
		if (store == null)
		{
			a = null;
		}
		else
		{
			StoreClientInstance currentClientInstance = store.currentClientInstance;
			if (currentClientInstance == null)
			{
				a = null;
			}
			else
			{
				StoreClient storeClient = currentClientInstance.storeClient;
				a = ((storeClient != null) ? storeClient.identifier : null);
			}
		}
		if (!string.Equals((string?)a, id, StringComparison.Ordinal))
		{
			return ContainsClient((manager != null) ? manager.clientStack : null, id);
		}
		return true;
	}
}
