using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;

namespace DeepSpacePawnshop.FutureTech;

internal static class FutureNight
{
	private const int CabinetEnergyPerNight = 12;

	private static readonly Dictionary<int, int> ChargerBatteryAtDusk = new Dictionary<int, int>();

	private static readonly Dictionary<int, HashSet<int>> CabinetItemsAtDusk = new Dictionary<int, HashSet<int>>();

	private static Action<string>? _log;

	private static Action? _earlyHook;

	private static Action? _lateHook;

	private static Action? _loadHook;

	internal static void Initialize(Action<string> log)
	{
		_log = log;
		_earlyHook = DelegateSupport.ConvertDelegate<Action>((Delegate)new Action(CaptureAtDusk));
		_lateHook = DelegateSupport.ConvertDelegate<Action>((Delegate)new Action(FinishAtDawn));
		_loadHook = DelegateSupport.ConvertDelegate<Action>((Delegate)new Action(RestoreLayouts));
		ModHook.OnHandlingNightlyServicesEarly += _earlyHook;
		ModHook.OnHandlingNightlyServicesLate += _lateHook;
		ModHook.OnGameLoadedLate += _loadHook;
	}

	private static IEnumerable<GameItem> Owned(string id)
	{
		EmporiumEntry instance = EmporiumEntry.Instance;
		List<GameItem> val = ((instance != null) ? instance.GetAllOwnedItems(false, false) : null);
		if (val == null)
		{
			yield break;
		}
		Enumerator<GameItem> enumerator = val.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameItem current = enumerator.Current;
			if (current != null && current.identifier == id)
			{
				yield return current;
			}
		}
	}

	private static GameInventory? FindInventory(GameItem owner, string id)
	{
		Stack<Il2CppObjectBase> stack = new Stack<Il2CppObjectBase>();
		HashSet<IntPtr> hashSet = new HashSet<IntPtr>();
		stack.Push((Il2CppObjectBase)(object)owner);
		while (stack.Count > 0)
		{
			Il2CppObjectBase val = stack.Pop();
			if (val == null || !hashSet.Add(val.Pointer))
			{
				continue;
			}
			GameInventory val2 = val.TryCast<GameInventory>();
			if (((val2 != null) ? val2.identifier : null) == id)
			{
				return val2;
			}
			GameItem val3 = val.TryCast<GameItem>();
			Enumerator<GraphNodeStorage> enumerator;
			if (val3 != null)
			{
				if (val3.child != null)
				{
					stack.Push((Il2CppObjectBase)(object)val3.child);
				}
				if (val3.contentWindow != null)
				{
					stack.Push((Il2CppObjectBase)(object)val3.contentWindow);
				}
				if (val3.children != null)
				{
					enumerator = val3.children.GetEnumerator();
					while (enumerator.MoveNext())
					{
						GraphNodeStorage current = enumerator.Current;
						if (current != null)
						{
							stack.Push((Il2CppObjectBase)(object)current);
						}
					}
				}
			}
			GraphNodeWindow val4 = val.TryCast<GraphNodeWindow>();
			if (((val4 != null) ? val4.child : null) != null)
			{
				stack.Push((Il2CppObjectBase)(object)val4.child);
			}
			PixelWindow val5 = val.TryCast<PixelWindow>();
			if (((val5 != null) ? val5.child : null) != null)
			{
				stack.Push((Il2CppObjectBase)(object)val5.child);
			}
			GraphNodeStorage val6 = val.TryCast<GraphNodeStorage>();
			if (((val6 != null) ? val6.children : null) == null)
			{
				continue;
			}
			enumerator = val6.children.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GraphNodeStorage current2 = enumerator.Current;
				if (current2 != null)
				{
					stack.Push((Il2CppObjectBase)(object)current2);
				}
			}
		}
		return null;
	}

	private static void EnsurePowerBays(GameItem item)
	{
		if (item.identifier == "dspf_future_purifier")
		{
			if (FindInventory(item, "dspf_future_purifier_power") == null)
			{
				ValueTuple<PixelWindow, GameInventory> val = DirectoryUtils.CreateInventoryWindow(2, 2, true);
				val.Item2.identifier = "dspf_future_purifier_power";
				item.SetContentWindow(val.Item1);
				ContainerHelper.InitContainerItem(val.Item2, item, (List<string>)null, (List<string>)null);
				ContainerHelper.AllowOnlyTaggedItem(val.Item2, "ENERGY_CELL", false, false);
			}
		}
		else
		{
			if (FindInventory(item, "dspf_future_aging_cabinet_power") != null)
			{
				return;
			}
			GameGridInventory val2 = FutureItems.FindStorageGrid(item);
			int num = FutureItems.AgingSize(item);
			ValueTuple<PixelWindow, GameInventory, GameInventory> val3 = DirectoryUtils.CreateDoubleInventoryWindowSlotInput(num, num, true, true, 0);
			val3.Item2.identifier = "dspf_future_aging_cabinet_power";
			val3.Item3.identifier = "dspf_future_aging_cabinet_inventory";
			GameSlotInventory obj = ((Il2CppObjectBase)val3.Item2).TryCast<GameSlotInventory>();
			if (obj != null)
			{
				obj.ResizePixels(64, 64);
			}
			ContainerHelper.InitContainerItem(val3.Item2, item, (List<string>)null, (List<string>)null);
			ContainerHelper.AllowOnlyTaggedItem(val3.Item2, "ENERGY_CELL", false, false);
			ContainerHelper.InitContainerItem(val3.Item3, item, (List<string>)null, (List<string>)null);
			List<GameItem> list = new List<GameItem>();
			if (((val2 != null) ? ((GameInventory)val2).childItems : null) != null)
			{
				Enumerator<GameItem> enumerator = ((GameInventory)val2).childItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameItem current = enumerator.Current;
					if (current != null)
					{
						list.Add(current);
					}
				}
			}
			foreach (GameItem item2 in list)
			{
				if (val2 == null || !((GameInventory)val2).Expel(item2) || !val3.Item3.UncheckedAccept(item2))
				{
					throw new InvalidOperationException("Could not migrate cabinet contents.");
				}
			}
			item.SetContentWindow(val3.Item1);
		}
	}

	private static void RestoreLayouts()
	{
		try
		{
			foreach (GameItem item in Owned("dspf_future_charger"))
			{
				item.SetShape(FutureItems.RectangleShape(2, 2));
				FutureItems.ResizeStorage(item, 4, 4);
			}
			foreach (GameItem item2 in Owned("dspf_future_purifier"))
			{
				int num = Math.Max(1, DurabilityHelper.GetMaxDurability(item2));
				int num2 = Math.Clamp(DurabilityHelper.GetRealDurability(item2), 0, num);
				if (num != 1000)
				{
					DurabilityHelper.InitItemWithDurabilityPricingMode2(item2, 1000, 25);
					DurabilityHelper.SetCurrentDurability(item2, (int)Math.Round((double)num2 * 1000.0 / (double)num));
				}
				item2.SetShape(FutureItems.RectangleShape(2, 1));
				EnsurePowerBays(item2);
			}
			foreach (GameItem item3 in Owned("dspf_future_module"))
			{
				item3.SetShape(FutureItems.RectangleShape(2, 1));
				FutureItems.ApplyModuleBonuses(item3);
			}
			string[] array;
			foreach (GameItem item4 in Owned("dspf_future_aging_cabinet"))
			{
				item4.SetGameItemType("STORAGE");
				array = new string[4] { "MACHINE", "STANDARD_MACHINE_TAG", "MACHINE_RECHARGABLE_TAG", "machinery" };
				foreach (string text in array)
				{
					item4.state.DisableTag(text);
				}
				item4.SetShape(FutureItems.RectangleShape(2, 1));
				EnsurePowerBays(item4);
				int num3 = FutureItems.AgingSize(item4);
				FutureItems.ResizeStorage(item4, num3, num3);
			}
			foreach (GameItem item5 in Owned("dspf_future_pocket"))
			{
				GameGridInventory val = FutureItems.FindStorageGrid(item5);
				if (val != null && !item5.state.IsTag("BACKPACK_TAG"))
				{
					ContainerHelper.InitBackpackItem((GameInventory)(object)val, item5);
				}
				item5.SetShape(FutureItems.RectangleShape(2, 1));
				if (val != null && val.inventoryShape.height > 10)
				{
					bool flag = false;
					Enumerator<GameItem> enumerator2 = item5.FindAllChildItems().GetEnumerator();
					while (enumerator2.MoveNext())
					{
						GameItem current6 = enumerator2.Current;
						if (current6 != null && current6.uniqueId != item5.uniqueId)
						{
							flag = true;
						}
					}
					if (flag)
					{
						continue;
					}
				}
				FutureItems.ResizeStorage(item5, 30, 10);
			}
			foreach (GameItem item6 in Owned("dspf_future_nuclear_battery"))
			{
				int num4 = FutureItems.TagInt(item6, "power_source_item_max_energy") ?? 500;
				int num5 = FutureItems.TagInt(item6, "power_source_item_energy") ?? num4;
				if (num4 != 1000)
				{
					FutureItems.SetTagInt(item6, "power_source_item_max_energy", 1000);
					PowerHelper.SetPowerSourceAt(item6, Math.Clamp((int)Math.Round((double)num5 * 1000.0 / (double)Math.Max(1, num4)), 0, 1000));
				}
				item6.SetShape(FutureItems.RectangleShape(2, 2));
			}
			foreach (GameItem item7 in Owned("dspf_future_void_jug"))
			{
				item7.SetShape(FutureItems.RectangleShape(1, 2));
				FutureItems.SetTagInt(item7, "LIQUID_CONTAINER_CAPACITY", Math.Max(18000000, WaterHelper.GetTotalVolume(item7)));
			}
			array = FutureItems.All;
			foreach (string id in array)
			{
				foreach (GameItem item8 in Owned(id))
				{
					FutureItems.UpgradeLegacyPrice(item8);
					FutureIcons.ApplyToItem(item8, id);
				}
			}
		}
		catch (Exception value)
		{
			_log?.Invoke($"Future Tech layout restoration failed: {value}");
		}
	}

	private static bool IsWork(GameItem item)
	{
		GameInventory parentInventory = item.parentInventory;
		if (((parentInventory != null) ? parentInventory.identifier : null) == "dspf_future_aging_cabinet_power")
		{
			return false;
		}
		if (item.state.IsTag("WINE_CONTAINER_TAG"))
		{
			if (WaterHelper.GetTotalVolume(item) > 0)
			{
				if (!WineHelper.IsFermentingOrFinished(item))
				{
					return MachineWineRack.CanAgeWine(item);
				}
				return true;
			}
			return false;
		}
		return AgableHelper.HasAged(item);
	}

	private static GameItem? BatteryIn(GameItem owner, string bay)
	{
		Enumerator<GameItem> enumerator = owner.FindAllChildItems().GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameItem current = enumerator.Current;
			if (current == null)
			{
				continue;
			}
			GameInventory parentInventory = current.parentInventory;
			if (((parentInventory != null) ? parentInventory.identifier : null) == bay)
			{
				int? num = FutureItems.TagInt(current, "power_source_item_max_energy");
				if (num.HasValue && num.GetValueOrDefault() > 0)
				{
					return current;
				}
			}
		}
		return null;
	}

	private static void CaptureAtDusk()
	{
		ChargerBatteryAtDusk.Clear();
		CabinetItemsAtDusk.Clear();
		try
		{
			foreach (GameItem item in Owned("dspf_future_charger"))
			{
				Enumerator<GameItem> enumerator2 = item.FindAllChildItems().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					GameItem current = enumerator2.Current;
					if (current != null && FutureItems.TagInt(current, "power_source_item_max_energy").HasValue)
					{
						int? num = FutureItems.TagInt(current, "power_source_item_energy");
						if (num.HasValue)
						{
							ChargerBatteryAtDusk[current.uniqueId] = num.Value;
						}
					}
				}
			}
			foreach (GameItem item2 in Owned("dspf_future_aging_cabinet"))
			{
				HashSet<int> hashSet = new HashSet<int>();
				Enumerator<GameItem> enumerator2 = item2.FindAllChildItems().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					GameItem current3 = enumerator2.Current;
					if (current3 != null && IsWork(current3))
					{
						hashSet.Add(current3.uniqueId);
					}
				}
				if (hashSet.Count > 0)
				{
					CabinetItemsAtDusk[item2.uniqueId] = hashSet;
				}
			}
		}
		catch (Exception value)
		{
			_log?.Invoke($"Future Tech night preparation failed: {value}");
		}
	}

	private static void FinishAtDawn()
	{
		try
		{
			int num = (NetworkUpgrade.IsCommercialPowerUnlocked() ? 18 : 9);
			foreach (GameItem item in Owned("dspf_future_charger"))
			{
				Enumerator<GameItem> enumerator2 = item.FindAllChildItems().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					GameItem current = enumerator2.Current;
					if (current != null && ChargerBatteryAtDusk.TryGetValue(current.uniqueId, out var value))
					{
						int? num2 = FutureItems.TagInt(current, "power_source_item_max_energy");
						if (num2.HasValue && num2.GetValueOrDefault() > 0)
						{
							PowerHelper.SetPowerSourceAt(current, Math.Min(num2.Value, value + num));
						}
					}
				}
			}
			foreach (GameItem item2 in Owned("dspf_future_purifier"))
			{
				GameItem val = BatteryIn(item2, "dspf_future_purifier_power");
				if (val != null)
				{
					int valueOrDefault = FutureItems.TagInt(val, "power_source_item_energy").GetValueOrDefault();
					int realDurability = DurabilityHelper.GetRealDurability(item2);
					int num3 = Math.Min(valueOrDefault, Math.Max(0, 1000 - realDurability));
					if (num3 > 0)
					{
						PowerHelper.SetPowerSourceAt(val, valueOrDefault - num3);
						DurabilityHelper.SetCurrentDurability(item2, realDurability + num3);
					}
				}
			}
			foreach (GameItem item3 in Owned("dspf_future_aging_cabinet"))
			{
				if (!CabinetItemsAtDusk.TryGetValue(item3.uniqueId, out HashSet<int> value2))
				{
					continue;
				}
				GameItem val2 = BatteryIn(item3, "dspf_future_aging_cabinet_power");
				int num4 = ((val2 != null) ? FutureItems.TagInt(val2, "power_source_item_energy").GetValueOrDefault() : 0);
				if (num4 < 12)
				{
					continue;
				}
				bool flag = false;
				Enumerator<GameItem> enumerator2 = item3.FindAllChildItems().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					GameItem current4 = enumerator2.Current;
					if (current4 == null || !value2.Contains(current4.uniqueId) || !IsWork(current4))
					{
						continue;
					}
					if (current4.state.IsTag("WINE_CONTAINER_TAG"))
					{
						if (!WineHelper.IsFinishedWine(current4))
						{
							if (WineHelper.IsFermentingOrFinished(current4))
							{
								WineHelper.FinishFermentation(current4);
								flag |= WineHelper.IsFinishedWine(current4);
							}
						}
						else if (MachineWineRack.CanAgeWine(current4))
						{
							int age = AgableHelper.GetAge(current4);
							WineHelper.OnAgeWine(current4);
							flag |= AgableHelper.GetAge(current4) > age;
						}
					}
					else if (AgableHelper.HasAged(current4))
					{
						int age2 = AgableHelper.GetAge(current4);
						AgableHelper.OnAge(current4);
						flag |= AgableHelper.GetAge(current4) > age2;
					}
				}
				if (flag && val2 != null)
				{
					PowerHelper.SetPowerSourceAt(val2, num4 - 12);
				}
			}
		}
		catch (Exception value3)
		{
			_log?.Invoke($"Future Tech nightly processing failed: {value3}");
		}
		finally
		{
			ChargerBatteryAtDusk.Clear();
			CabinetItemsAtDusk.Clear();
		}
	}
}
