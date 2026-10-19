using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;

namespace DeepSpacePawnshop.FutureTech;

internal static class FutureAcquisition
{
	private sealed class OfferSwap
	{
		internal GameItem Original { get; }

		internal GameItem Replacement { get; }

		internal bool Arg1 { get; }

		internal bool Arg2 { get; }

		internal bool Arg3 { get; }

		internal int Arg4 { get; }

		internal OfferSwap(GameItem original, GameItem replacement, bool arg1, bool arg2, bool arg3, int arg4)
		{
			Original = original;
			Replacement = replacement;
			Arg1 = arg1;
			Arg2 = arg2;
			Arg3 = arg3;
			Arg4 = arg4;
		}
	}

	[HarmonyPatch(typeof(StartingPerkList), "InitStartingPerk")]
	private static class PerkListPatch
	{
		private static void Postfix()
		{
			RegisterPerk();
		}
	}

	[HarmonyPatch(typeof(StoreClientList), "PlaceInventorInventory")]
	private static class DoctorInventoryPatch
	{
		private static void Prefix(bool isVisitingPlayerStore)
		{
			_placingInventorInventory = true;
			_insideNativeDoctorOffers = false;
			OfferedFutureItems.Clear();
			if (isVisitingPlayerStore && !_doctorVisitActive)
			{
				_doctorVisitBaseline = CounterItemPointers();
				_doctorVisitActive = true;
			}
		}

		private static Exception? Finalizer(Exception? __exception)
		{
			_placingInventorInventory = false;
			_insideNativeDoctorOffers = false;
			return __exception;
		}
	}

	[HarmonyPatch(typeof(ModHook), "FireOnPlaceInventorInventoryItemEarly")]
	private static class DoctorNativeOffersStartPatch
	{
		private static void Postfix()
		{
			_insideNativeDoctorOffers = _placingInventorInventory;
		}
	}

	[HarmonyPatch(typeof(ModHook), "FireOnPlaceInventorInventoryItemLate")]
	private static class DoctorNativeOffersEndPatch
	{
		private static void Prefix()
		{
			_insideNativeDoctorOffers = false;
		}
	}

	[HarmonyPatch(typeof(PlayerStore), "AddDirectSellingItemToTable")]
	private static class DoctorRandomOfferPatch
	{
		private static void Prefix(ref GameItem gameItem, bool __1, bool __2, bool __3, int __4, out OfferSwap? __state)
		{
			__state = null;
			if (!_insideNativeDoctorOffers || _restoringOriginalOffer || gameItem == null || !RandomDoctorOffers.Contains(gameItem.identifier))
			{
				return;
			}
			string[] array = FutureItems.All.Where((string id) => !OfferedFutureItems.Contains(id)).ToArray();
			if (array.Length == 0)
			{
				return;
			}
			int num = Random.Shared.Next(RandomDoctorOffers.Count + array.Length);
			if (num >= array.Length)
			{
				return;
			}
			try
			{
				string text = array[num];
				GameItem val = DirectoryMaster.Item(text, false);
				if (val == null)
				{
					return;
				}
				if (val.identifier != text)
				{
					val.Destroy();
					return;
				}
				EmporiumEntry instance = EmporiumEntry.Instance;
				GameGridInventory obj = ((instance != null) ? instance.frontInvinvElement : null);
				if (((obj != null) ? obj.TryFindOneValidInventorySlot(val, false) : null) == null)
				{
					val.Destroy();
					return;
				}
				__state = new OfferSwap(gameItem, val, __1, __2, __3, __4);
				gameItem = val;
			}
			catch (Exception value)
			{
				_log?.Invoke($"Could not prepare a Future Tech doctor offer: {value}");
			}
		}

		private static void Postfix(PlayerStore __instance, OfferSwap? __state)
		{
			if (__state == null)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			try
			{
				EmporiumEntry instance = EmporiumEntry.Instance;
				GameGridInventory val = ((instance != null) ? instance.frontInvinvElement : null);
				int num;
				if (val != null)
				{
					GameInventory parentInventory = __state.Replacement.parentInventory;
					num = ((((parentInventory != null) ? new IntPtr?(((Il2CppObjectBase)parentInventory).Pointer) : null) == ((Il2CppObjectBase)val).Pointer) ? 1 : 0);
				}
				else
				{
					num = 0;
				}
				flag = (byte)num != 0;
				flag2 = __state.Replacement.parentInventory != null;
			}
			catch (Exception value)
			{
				_log?.Invoke($"Could not verify the replacement doctor offer: {value}");
			}
			if (flag)
			{
				OfferedFutureItems.Add(__state.Replacement.identifier);
				try
				{
					__state.Original.Destroy();
				}
				catch (Exception value2)
				{
					_log?.Invoke($"Could not dispose of the replaced doctor offer: {value2}");
				}
				_log?.Invoke($"Doctor offer replaced: {__state.Original.identifier} -> {__state.Replacement.identifier}.");
				return;
			}
			try
			{
				if (flag2)
				{
					__state.Replacement.Destroy();
				}
				RestoreOriginalOffer(__instance, __state);
			}
			catch (Exception value3)
			{
				_log?.Invoke($"Could not restore the original doctor offer: {value3}");
			}
		}
	}

	[HarmonyPatch(typeof(StoreClientMono), "OnLeft")]
	private static class DoctorDeparturePatch
	{
		private static void Prefix()
		{
			if (!_doctorVisitActive)
			{
				return;
			}
			PlayerStore instance = PlayerStore.Instance;
			object obj;
			if (instance == null)
			{
				obj = null;
			}
			else
			{
				StoreClientInstance currentClientInstance = instance.currentClientInstance;
				if (currentClientInstance == null)
				{
					obj = null;
				}
				else
				{
					StoreClient clientBlueprint = currentClientInstance.GetClientBlueprint();
					obj = ((clientBlueprint != null) ? clientBlueprint.identifier : null);
				}
			}
			string text = (string)obj;
			if (text != null && text != "inventor")
			{
				return;
			}
			try
			{
				int value = RemoveUnownedCounterItems(_doctorVisitBaseline);
				_log?.Invoke($"Cleared {value} unpurchased doctor offer(s) on departure.");
			}
			finally
			{
				_doctorVisitActive = false;
				_doctorVisitBaseline = null;
			}
		}
	}

	[HarmonyPatch(typeof(PlayerStore), "LoadGame")]
	private static class OldSaveCounterPatch
	{
		private static void Prefix()
		{
			_doctorVisitActive = false;
			_doctorVisitBaseline = null;
			_placingInventorInventory = false;
			_insideNativeDoctorOffers = false;
		}

		private static void Postfix()
		{
			MelonCoroutines.Start(CleanOrphanedOffersAfterLoad());
		}
	}

	[HarmonyPatch(typeof(MainMenuUIController), "OnNewGameClick")]
	private static class NewGameMenuPatch
	{
		private static void Prefix()
		{
			PreparePerk();
		}
	}

	[HarmonyPatch(typeof(MainMenuUIController), "OpenPerkInterface")]
	private static class PerkMenuPatch
	{
		private static void Prefix()
		{
			PreparePerk();
		}

		private static void Postfix()
		{
			PerkUIController instance = PerkUIController.Instance;
			if (instance != null)
			{
				EnsurePerkVisible(instance);
			}
		}
	}

	[HarmonyPatch(typeof(PerkUIController), "Start")]
	private static class PerkUiStartPatch
	{
		private static void Prefix()
		{
			PreparePerk();
		}

		private static void Postfix(PerkUIController __instance)
		{
			EnsurePerkVisible(__instance);
		}
	}

	[HarmonyPatch(typeof(PerkUIController), "OpenUI")]
	private static class PerkUiPatch
	{
		private static void Prefix()
		{
			PreparePerk();
		}

		private static void Postfix(PerkUIController __instance)
		{
			EnsurePerkVisible(__instance);
		}
	}

	[HarmonyPatch(typeof(StartingPerkIconLoader), "Start")]
	private static class PerkIconPatch
	{
		private static void Postfix()
		{
			RegisterPerkIcon();
		}
	}

	[HarmonyPatch(typeof(StartingPerk), "GetLocalizedDisplayName")]
	private static class PerkNamePatch
	{
		private static void Postfix(StartingPerk __instance, ref string __result)
		{
			if (__instance.id == "dspf_future_tech")
			{
				__result = FutureText.PerkName;
			}
		}
	}

	[HarmonyPatch(typeof(StartingPerk), "GetLocalizedDescription")]
	private static class PerkDescriptionPatch
	{
		private static void Postfix(StartingPerk __instance, ref string __result)
		{
			if (__instance.id == "dspf_future_tech")
			{
				__result = FutureText.PerkDescription;
			}
		}
	}

	[HarmonyPatch(typeof(StartingPerkElement), "SetTooltipContent")]
	private static class PerkTooltipPatch
	{
		private static void Postfix(StartingPerk perk)
		{
			if (!(((perk != null) ? perk.id : null) != "dspf_future_tech"))
			{
				PerkUIController instance = PerkUIController.Instance;
				if (((instance != null) ? instance.startingPerkTooltipTitle : null) != null)
				{
					((TMP_Text)instance.startingPerkTooltipTitle).text = FutureText.PerkName;
				}
				if (((instance != null) ? instance.startingPerkTooltipDescription : null) != null)
				{
					((TMP_Text)instance.startingPerkTooltipDescription).text = FutureText.PerkDescription;
				}
			}
		}
	}

	[HarmonyPatch(typeof(NewGameData), "HandleInitialItem")]
	private static class InitialItemsPatch
	{
		private static void Postfix()
		{
			GrantStarterItems();
		}
	}

	internal const string PerkId = "dspf_future_tech";

	private static Action<string>? _log;

	private static readonly HashSet<string> RandomDoctorOffers = new HashSet<string>(StringComparer.Ordinal)
	{
		"storage_bay", "machine_bay", "recharger", "smuggler_bay", "fridge", "desequencer", "crypto_module_sec", "crypto_module_cmd", "crypto_module_med", "crypto_module_eng",
		"crypto_module_ser", "powerblock", "alarm_module_transmitter", "furnace_module_blast", "furnace_module_junk", "system_module_performance", "system_module_efficiancy", "system_module_quality", "system_module_overclock", "system_module_eco",
		"system_module_fineness", "energy_credit_breeder", "module_extractor_advanced", "uv_filter", "farm_module_nutrifruit", "farm_module_dream_cap", "farm_module_kotton_loom", "farm_module_nightmare_cap", "farm_module_fillerweed_production", "farm_module_hydroreed_filter",
		"farm_module_smuggler", "chem_module", "wine_yeast_infinite", "mouse_trap", "black_lamp", "module_bay_expansion_kit"
	};

	private static readonly HashSet<string> OfferedFutureItems = new HashSet<string>(StringComparer.Ordinal);

	private static HashSet<nint>? _doctorVisitBaseline;

	private static bool _doctorVisitActive;

	private static bool _placingInventorInventory;

	private static bool _insideNativeDoctorOffers;

	private static bool _restoringOriginalOffer;

	internal static void Initialize(Action<string> log)
	{
		_log = log;
	}

	private static HashSet<nint> CounterItemPointers()
	{
		HashSet<IntPtr> hashSet = new HashSet<IntPtr>();
		EmporiumEntry instance = EmporiumEntry.Instance;
		List<GameItem> val = ((instance != null) ? instance.GetCountersItems() : null);
		if (val != null)
		{
			Enumerator<GameItem> enumerator = val.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GameItem current = enumerator.Current;
				if (current != null)
				{
					hashSet.Add(((Il2CppObjectBase)current).Pointer);
				}
			}
		}
		return hashSet;
	}

	private static int RemoveUnownedCounterItems(HashSet<nint>? keep)
	{
		EmporiumEntry instance = EmporiumEntry.Instance;
		List<GameItem> val = ((instance != null) ? instance.GetCountersItems() : null);
		if (val == null)
		{
			return 0;
		}
		int num = 0;
		Enumerator<GameItem> enumerator = val.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameItem current = enumerator.Current;
			if (current == null || (keep != null && keep.Contains(((Il2CppObjectBase)current).Pointer)))
			{
				continue;
			}
			try
			{
				if (!GeneralHelper.IsItemEffectivelyOwned(current))
				{
					current.Destroy();
					num++;
				}
			}
			catch (Exception value)
			{
				_log?.Invoke($"Could not clear an unowned doctor offer: {value}");
			}
		}
		return num;
	}

	private static IEnumerator CleanOrphanedOffersAfterLoad()
	{
		for (int attempt = 0; attempt < 20; attempt++)
		{
			yield return (object)new WaitForSeconds(0.5f);
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null || EmporiumEntry.Instance == null)
			{
				continue;
			}
			if (instance.currentClientInstance == null && !_doctorVisitActive)
			{
				int num = RemoveUnownedCounterItems(null);
				if (num > 0)
				{
					_log?.Invoke($"Cleared {num} orphaned unowned counter item(s) after load.");
				}
			}
			break;
		}
	}

	private static void RestoreOriginalOffer(PlayerStore store, OfferSwap swap)
	{
		_restoringOriginalOffer = true;
		try
		{
			store.AddDirectSellingItemToTable(swap.Original, swap.Arg1, swap.Arg2, swap.Arg3, swap.Arg4);
		}
		finally
		{
			_restoringOriginalOffer = false;
		}
	}

	private static void RegisterPerk()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		try
		{
			List<StartingPerk> perks = StartingPerkList.Perks;
			if (perks == null)
			{
				return;
			}
			for (int i = 0; i < perks.Count; i++)
			{
				StartingPerk obj = perks[i];
				if (((obj != null) ? obj.id : null) == "dspf_future_tech")
				{
					perks[i].cost = 2;
					perks[i].rawName = FutureText.PerkName;
					perks[i].rawDescription = FutureText.PerkDescription;
					return;
				}
			}
			perks.Add(new StartingPerk
			{
				id = "dspf_future_tech",
				cost = 2,
				maxSlot = 1,
				type = (StartingPerkType)0,
				rawName = FutureText.PerkName,
				rawDescription = FutureText.PerkDescription
			});
			_log?.Invoke("Registered Future Tech starting perk.");
		}
		catch (Exception value)
		{
			_log?.Invoke($"Future Tech perk registration failed: {value}");
		}
	}

	private static void PreparePerk()
	{
		try
		{
			if (StartingPerkList.Perks == null)
			{
				StartingPerkList.InitStartingPerk();
			}
			RegisterPerk();
			RegisterPerkIcon();
		}
		catch (Exception value)
		{
			_log?.Invoke($"Future Tech perk preparation failed: {value}");
		}
	}

	private static void RegisterPerkIcon()
	{
		try
		{
			Dictionary<string, Sprite> perkIcons = StartingPerkIconLoader.perkIcons;
			if (perkIcons == null)
			{
				return;
			}
			Sprite val = FutureIcons.PerkIcon();
			if (val == null)
			{
				string id = StartingPerkList.WellConnected().id;
				if (!perkIcons.TryGetValue(id, ref val))
				{
					return;
				}
			}
			perkIcons["dspf_future_tech"] = val;
		}
		catch (Exception value)
		{
			_log?.Invoke($"Future Tech perk icon registration failed: {value}");
		}
	}

	private static void EnsurePerkVisible(PerkUIController controller)
	{
		try
		{
			PreparePerk();
			GameObject availablePerks = controller.availablePerks;
			if (availablePerks == null)
			{
				return;
			}
			Il2CppArrayBase<StartingPerkElement> componentsInChildren = availablePerks.GetComponentsInChildren<StartingPerkElement>(true);
			Sprite sprite = default(Sprite);
			foreach (StartingPerkElement item in componentsInChildren)
			{
				if (item != null && !(item.id != "dspf_future_tech"))
				{
					Dictionary<string, Sprite> perkIcons = StartingPerkIconLoader.perkIcons;
					if (item.icon != null && perkIcons != null && perkIcons.TryGetValue("dspf_future_tech", ref sprite))
					{
						item.icon.sprite = sprite;
					}
					if (!((Component)item).gameObject.activeSelf)
					{
						((Component)item).gameObject.SetActive(true);
					}
					return;
				}
			}
			StartingPerkElement? obj = ((IEnumerable<StartingPerkElement>)componentsInChildren).FirstOrDefault((Func<StartingPerkElement, bool>)((StartingPerkElement element) => element != null && ((Component)element).gameObject.activeSelf));
			GameObject val = Object.Instantiate<GameObject>((((obj != null) ? ((Component)obj).gameObject : null) ?? controller.perkElementPrefab) ?? throw new InvalidOperationException("No starting perk UI template exists."), availablePerks.transform);
			((Object)val).name = "FutureTechStartingPerk";
			val.SetActive(false);
			StartingPerkElement component = val.GetComponent<StartingPerkElement>();
			StartingPerk perkById = StartingPerk.GetPerkById("dspf_future_tech");
			if (component == null || perkById == null)
			{
				Object.Destroy((Object)(object)val);
				throw new InvalidOperationException("Cloned perk UI element or perk definition is null.");
			}
			component.id = "dspf_future_tech";
			component.perk = perkById;
			component.isSelected = false;
			Dictionary<string, Sprite> perkIcons2 = StartingPerkIconLoader.perkIcons;
			Sprite sprite2 = default(Sprite);
			if (component.icon != null && perkIcons2 != null && perkIcons2.TryGetValue("dspf_future_tech", ref sprite2))
			{
				component.icon.sprite = sprite2;
			}
			val.SetActive(true);
			controller.SortPerkContainer(availablePerks);
		}
		catch (Exception value)
		{
			_log?.Invoke($"Future Tech starting perk UI failed: {value}");
		}
	}

	private static void GrantStarterItems()
	{
		NewGameData instance = NewGameData.Instance;
		List<StartingPerk> val = ((instance != null) ? instance.startingPerks : null);
		if (val == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < val.Count; i++)
		{
			StartingPerk obj = val[i];
			if (((obj != null) ? obj.id : null) == "dspf_future_tech")
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		EmporiumEntry instance2 = EmporiumEntry.Instance;
		if (instance2 == null || instance2.invElement == null)
		{
			_log?.Invoke("Future Tech starter items could not be granted: player inventory is unavailable.");
			return;
		}
		foreach (var (text2, num2) in (from id in FutureEdition.StarterItems
			group id by id).ToDictionary((IGrouping<string, string> group) => group.Key, (IGrouping<string, string> group) => group.Count()))
		{
			try
			{
				int num3 = 0;
				List<GameItem> allOwnedItems = instance2.GetAllOwnedItems(false, false);
				if (allOwnedItems != null)
				{
					Enumerator<GameItem> enumerator2 = allOwnedItems.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						GameItem current = enumerator2.Current;
						if (((current != null) ? current.identifier : null) == text2)
						{
							num3++;
						}
					}
				}
				while (num3 < num2)
				{
					GameItem val2 = DirectoryMaster.Item(text2, true);
					if (val2 == null || val2.identifier != text2)
					{
						throw new InvalidOperationException("Factory returned no valid " + text2 + ".");
					}
					if (!instance2.CanInsertIntoPlayerInventories(val2))
					{
						instance2.SetBigInv();
					}
					if (!instance2.CanInsertIntoPlayerInventories(val2))
					{
						throw new InvalidOperationException("Player inventory has no room after expansion.");
					}
					instance2.TryAddToPlayerInv(val2);
					if (val2.parentInventory == null)
					{
						throw new InvalidOperationException("Player inventory did not accept the granted item.");
					}
					num3++;
					_log?.Invoke($"Granted Future Tech starter item: {text2} ({num3}/{num2}).");
				}
			}
			catch (Exception value)
			{
				_log?.Invoke($"Could not grant Future Tech starter item {text2}: {value}");
			}
		}
	}
}
