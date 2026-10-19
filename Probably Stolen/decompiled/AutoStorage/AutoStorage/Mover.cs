using System;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;

namespace AutoStorage;

internal static class Mover
{
	private static bool _scanWarned;

	public static bool CanAccept(GameInventory inv, GameItem item)
	{
		if (inv == null || item == null)
		{
			return false;
		}
		SlotMarker val = null;
		try
		{
			val = inv.TryFindOneValidInventorySlot(item);
		}
		catch (Exception ex)
		{
			if (!_scanWarned)
			{
				_scanWarned = true;
				Core.Log.Warning("[AutoStorage] 容量扫描异常（按容器已满处理）: " + ex.Message);
			}
			return false;
		}
		return val != null;
	}

	public static bool Move(GameItem item, GameInventory target)
	{
		if (item == null || target == null)
		{
			return false;
		}
		GameInventory val = null;
		try
		{
			val = item.parentInventory;
		}
		catch
		{
		}
		if (val == null)
		{
			return false;
		}
		if (U.Same((Il2CppObjectBase)(object)val, (Il2CppObjectBase)(object)target))
		{
			return false;
		}
		bool flag = false;
		try
		{
			flag = val.Expel(item);
		}
		catch (Exception ex)
		{
			Core.Log.Warning("[AutoStorage] Expel: " + ex.Message);
		}
		if (!flag)
		{
			return false;
		}
		try
		{
			if (item.parentInventory != null)
			{
				return false;
			}
		}
		catch
		{
		}
		if (Place(item, target))
		{
			return true;
		}
		if (!Place(item, val))
		{
			Core.Log.Warning("[AutoStorage] !! " + U.SafeName(item) + " 未能放回原处，请检查物品栏。");
		}
		return false;
	}

	private static bool Place(GameItem item, GameInventory inv)
	{
		if (item == null || inv == null)
		{
			return false;
		}
		SlotMarker val = null;
		try
		{
			val = inv.TryFindOneValidInventorySlot(item);
		}
		catch (Exception ex)
		{
			Core.Log.Warning("[AutoStorage] TryFindOneValidInventorySlot: " + ex.Message);
		}
		if (val == null)
		{
			return false;
		}
		try
		{
			val.AcceptUnchecked();
		}
		catch (Exception ex2)
		{
			Core.Log.Warning("[AutoStorage] AcceptUnchecked: " + ex2.Message);
			return false;
		}
		try
		{
			return U.Same((Il2CppObjectBase)(object)item.parentInventory, (Il2CppObjectBase)(object)inv);
		}
		catch
		{
			return false;
		}
	}
}
