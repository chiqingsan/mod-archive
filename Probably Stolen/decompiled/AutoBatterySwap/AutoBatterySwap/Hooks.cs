using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Collections.Generic;
using MelonLoader;

namespace AutoBatterySwap;

public static class Hooks
{
	private static bool _busy;

	public static void BeforeHandlePower(bool isFromShip)
	{
		RunAll();
	}

	public static void BeforePlayerHandlePower()
	{
		RunAll();
	}

	public static void BeforeTryDrawCyclePower(GameItem machine, GameSlotInventory batterySlot)
	{
		if (machine != null)
		{
			TryTopUpOne(machine, batterySlot);
		}
	}

	public static void BeforeCanPower(GameItem machine, GameSlotInventory batterySlot)
	{
		if (machine != null)
		{
			TryTopUpOne(machine, batterySlot);
		}
	}

	public static void BeforeDrawPowerSource(GameItem gameItem, int amount)
	{
		if (gameItem != null && amount > 0)
		{
			TryTopUpBatteryItem(gameItem, amount);
		}
	}

	public static void BeforeCanDrawPowerSource(GameItem gameItem, int amount)
	{
		if (gameItem != null && amount > 0)
		{
			TryTopUpBatteryItem(gameItem, amount);
		}
	}

	public static void BeforeTryRemoveEnergy(GameItem powerSourceItem, int desiredEnergy, bool useBandwidth)
	{
		if (powerSourceItem != null && desiredEnergy > 0)
		{
			TryTopUpBatteryItem(powerSourceItem, desiredEnergy);
		}
	}

	private static void RunAll()
	{
		if (_busy)
		{
			return;
		}
		_busy = true;
		try
		{
			SwapAllMachines();
		}
		catch (Exception value)
		{
			MelonLogger.Error($"[AutoBatterySwap] {value}");
		}
		finally
		{
			_busy = false;
		}
	}

	private static int ReadEnergy(GameItem item)
	{
		if (item == null)
		{
			return 0;
		}
		try
		{
			return PowerHelper.GetAvailableEnergyFromItem(item, false);
		}
		catch
		{
			return 0;
		}
	}

	private static void TryTopUpOne(GameItem machine, GameSlotInventory batterySlot)
	{
		if (_busy)
		{
			return;
		}
		_busy = true;
		try
		{
			if (batterySlot == null)
			{
				try
				{
					batterySlot = MachineHelper.GetBatterySlot(machine);
				}
				catch
				{
					return;
				}
			}
			if (batterySlot == null)
			{
				return;
			}
			GameItem val = null;
			try
			{
				val = batterySlot.childItem;
			}
			catch
			{
			}
			if (val == null)
			{
				return;
			}
			int num = 0;
			try
			{
				num = MachineryHelper.GetMachinePowerUsage(machine);
			}
			catch
			{
				return;
			}
			if (num <= 0)
			{
				num = 1;
			}
			int num2 = ReadEnergy(val);
			if (num2 >= num)
			{
				return;
			}
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null)
			{
				return;
			}
			List<GameItem> val2 = instance.FindAllItem(true);
			if (val2 == null)
			{
				return;
			}
			List<GameItem> pool = CollectPlayerBatteries(val2);
			GameItem val3 = FindBest(pool, val, num, num2);
			if (val3 != null)
			{
				int num3 = ReadEnergy(val3);
				PowerHelper.SetPowerSourceAt(val, num3);
				PowerHelper.SetPowerSourceAt(val3, num2);
				try
				{
					PowerHelper.UpdateEnergyCreditSprite(val);
				}
				catch
				{
				}
				try
				{
					PowerHelper.UpdateEnergyCreditSprite(val3);
				}
				catch
				{
				}
				MelonLogger.Msg($"[AutoBatterySwap] instant need={num} {num2}<->{num3}");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("[AutoBatterySwap] instant: " + ex.Message);
		}
		finally
		{
			_busy = false;
		}
	}

	private static void TryTopUpBatteryItem(GameItem bat, int need)
	{
		if (_busy || bat == null)
		{
			return;
		}
		int num = ReadEnergy(bat);
		if (num >= need)
		{
			return;
		}
		_busy = true;
		try
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null)
			{
				return;
			}
			List<GameItem> val = instance.FindAllItem(true);
			if (val == null)
			{
				return;
			}
			List<GameItem> pool = CollectPlayerBatteries(val);
			GameItem val2 = FindBest(pool, bat, need, num);
			if (val2 != null)
			{
				int num2 = ReadEnergy(val2);
				PowerHelper.SetPowerSourceAt(bat, num2);
				PowerHelper.SetPowerSourceAt(val2, num);
				try
				{
					PowerHelper.UpdateEnergyCreditSprite(bat);
				}
				catch
				{
				}
				try
				{
					PowerHelper.UpdateEnergyCreditSprite(val2);
				}
				catch
				{
				}
				MelonLogger.Msg($"[AutoBatterySwap] draw need={need} {num}<->{num2}");
			}
		}
		catch (Exception ex)
		{
			MelonLogger.Error("[AutoBatterySwap] draw: " + ex.Message);
		}
		finally
		{
			_busy = false;
		}
	}

	private static void SwapAllMachines()
	{
		PlayerStore instance = PlayerStore.Instance;
		if (instance == null)
		{
			return;
		}
		List<GameItem> val = instance.FindAllItem(true);
		if (val == null)
		{
			return;
		}
		List<GameItem> list = new List<GameItem>();
		Enumerator<GameItem> enumerator = val.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameItem current = enumerator.Current;
			if (current == null)
			{
				continue;
			}
			try
			{
				if (MachineHelper.GetBatterySlot(current) != null)
				{
					list.Add(current);
				}
			}
			catch
			{
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		List<GameItem> list2 = CollectPlayerBatteries(val);
		if (list2.Count == 0)
		{
			return;
		}
		HashSet<IntPtr> hashSet = new HashSet<IntPtr>();
		int num = 0;
		foreach (GameItem item in list)
		{
			GameSlotInventory batterySlot;
			try
			{
				batterySlot = MachineHelper.GetBatterySlot(item);
			}
			catch
			{
				continue;
			}
			if (batterySlot == null)
			{
				continue;
			}
			GameItem val2 = null;
			try
			{
				val2 = batterySlot.childItem;
			}
			catch
			{
			}
			if (val2 == null)
			{
				continue;
			}
			int num2 = 0;
			try
			{
				num2 = MachineryHelper.GetMachinePowerUsage(item);
			}
			catch
			{
				continue;
			}
			if (num2 <= 0)
			{
				continue;
			}
			int num3 = ReadEnergy(val2);
			if (num3 >= num2)
			{
				continue;
			}
			GameItem val3 = FindBest(list2, val2, num2, num3, hashSet);
			if (val3 == null)
			{
				continue;
			}
			int num4 = ReadEnergy(val3);
			try
			{
				PowerHelper.SetPowerSourceAt(val2, num4);
				PowerHelper.SetPowerSourceAt(val3, num3);
				try
				{
					PowerHelper.UpdateEnergyCreditSprite(val2);
				}
				catch
				{
				}
				try
				{
					PowerHelper.UpdateEnergyCreditSprite(val3);
				}
				catch
				{
				}
				hashSet.Add(((Il2CppObjectBase)val3).Pointer);
				num++;
				MelonLogger.Msg($"[AutoBatterySwap] night need={num2} {num3}<->{num4}");
			}
			catch (Exception ex)
			{
				MelonLogger.Error("[AutoBatterySwap] night: " + ex.Message);
			}
		}
		if (num > 0)
		{
			MelonLogger.Msg($"[AutoBatterySwap] night done={num}");
		}
	}

	private static GameItem FindBest(List<GameItem> pool, GameItem current, int need, int curE, HashSet<IntPtr> used = null)
	{
		GameItem val = null;
		int num = curE;
		foreach (GameItem item in pool)
		{
			if (item == null || item == current || (used != null && used.Contains(((Il2CppObjectBase)item).Pointer)))
			{
				continue;
			}
			int num2 = ReadEnergy(item);
			if (num2 <= num)
			{
				continue;
			}
			if (val == null)
			{
				val = item;
				num = num2;
				continue;
			}
			bool flag = num2 >= need;
			bool flag2 = num >= need;
			if (flag && !flag2)
			{
				val = item;
				num = num2;
			}
			else if (flag == flag2 && num2 > num)
			{
				val = item;
				num = num2;
			}
		}
		if (val != null && num <= curE)
		{
			return null;
		}
		return val;
	}

	private static List<GameItem> CollectPlayerBatteries(List<GameItem> allItems)
	{
		List<GameItem> result = new List<GameItem>();
		HashSet<IntPtr> visited = new HashSet<IntPtr>();
		if (allItems == null)
		{
			return result;
		}
		Enumerator<GameItem> enumerator = allItems.GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameItem current = enumerator.Current;
			Walk(current, result, visited);
		}
		return result;
	}

	private static void Walk(GameItem item, List<GameItem> result, HashSet<IntPtr> visited)
	{
		if (item == null || !visited.Add(((Il2CppObjectBase)item).Pointer))
		{
			return;
		}
		if (IsBattery(item) && !IsInMachineSlot(item))
		{
			result.Add(item);
		}
		if (!IsContainer(item))
		{
			return;
		}
		try
		{
			object child = item.child;
			GameInventory val = (GameInventory)((child is GameInventory) ? child : null);
			if (val != null)
			{
				WalkInv(val, result, visited);
				return;
			}
			GraphNodeWindow val2 = (GraphNodeWindow)((child is GraphNodeWindow) ? child : null);
			if (val2 != null)
			{
				try
				{
					object child2 = val2.child;
					GameInventory val3 = (GameInventory)((child2 is GameInventory) ? child2 : null);
					if (val3 != null)
					{
						WalkInv(val3, result, visited);
						return;
					}
				}
				catch
				{
				}
			}
		}
		catch
		{
		}
		try
		{
			List<GraphNodeStorage> children = item.children;
			if (children == null)
			{
				return;
			}
			Enumerator<GraphNodeStorage> enumerator = children.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GraphNodeStorage current = enumerator.Current;
				if (current == null)
				{
					continue;
				}
				GameInventory val4 = ((Il2CppObjectBase)current).TryCast<GameInventory>();
				if (val4 != null)
				{
					WalkInv(val4, result, visited);
					continue;
				}
				GameItem val5 = ((Il2CppObjectBase)current).TryCast<GameItem>();
				if (val5 != null)
				{
					Walk(val5, result, visited);
				}
			}
		}
		catch
		{
		}
	}

	private static void WalkInv(GameInventory inv, List<GameItem> result, HashSet<IntPtr> visited)
	{
		if (inv == null)
		{
			return;
		}
		try
		{
			List<GameItem> childItems = inv.childItems;
			if (childItems != null)
			{
				Enumerator<GameItem> enumerator = childItems.GetEnumerator();
				while (enumerator.MoveNext())
				{
					GameItem current = enumerator.Current;
					Walk(current, result, visited);
				}
			}
		}
		catch
		{
		}
	}

	private static bool IsInMachineSlot(GameItem item)
	{
		try
		{
			GameInventory parentInventory = item.parentInventory;
			if (parentInventory == null)
			{
				return false;
			}
			GameItem parentItem = parentInventory.GetParentItem();
			if (parentItem == null)
			{
				return false;
			}
			return MachineHelper.GetBatterySlot(parentItem) == parentInventory;
		}
		catch
		{
			return false;
		}
	}

	private static bool IsBattery(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		try
		{
			if (MachineHelper.GetBatterySlot(item) != null)
			{
				return false;
			}
			string identifier = item.identifier;
			if (string.IsNullOrEmpty(identifier))
			{
				return false;
			}
			string text = identifier.ToLowerInvariant();
			return text.StartsWith("energy_credit") || text.Contains("battery");
		}
		catch
		{
			return false;
		}
	}

	private static bool IsContainer(GameItem item)
	{
		if (item == null)
		{
			return false;
		}
		try
		{
			if (item.child != null)
			{
				return true;
			}
		}
		catch
		{
		}
		try
		{
			List<GraphNodeStorage> children = item.children;
			if (children != null && children.Count > 0)
			{
				return true;
			}
		}
		catch
		{
		}
		try
		{
			string identifier = item.identifier;
			if (string.IsNullOrEmpty(identifier))
			{
				return false;
			}
			string text = identifier.ToLowerInvariant();
			return text.Contains("container") || text.Contains("crate") || text.Contains("chest") || text.Contains("box") || text.Contains("bag") || text.Contains("pouch") || text.Contains("case") || text.Contains("smuggler") || text.Contains("backpack");
		}
		catch
		{
		}
		return false;
	}
}
