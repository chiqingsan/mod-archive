using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;

namespace WaterTower;

public static class TowerRegistry
{
	private static readonly List<TowerInstance> _all = new List<TowerInstance>();

	private static int _nextIndex = 1;

	private static bool _graphFailureLogged;

	public static List<TowerInstance> All => _all;

	public static TowerInstance Last { get; private set; }

	public static int Count => _all.Count;

	public static bool LastScanOk { get; private set; }

	public static TowerInstance Register(TowerInstance tower)
	{
		if (tower == null)
		{
			return null;
		}
		TowerInstance towerInstance = Find(tower.Item);
		if (towerInstance != null)
		{
			if (SameObject(towerInstance.Item, tower.Item))
			{
				Log.Info("水塔登记：该塔已在登记表中（第 " + towerInstance.Index + " 台），补齐引用");
				if (tower.TankItem != null)
				{
					towerInstance.TankItem = tower.TankItem;
				}
				if (tower.TankSlot != null)
				{
					towerInstance.TankSlot = tower.TankSlot;
				}
				if (tower.NoteItem != null)
				{
					towerInstance.NoteItem = tower.NoteItem;
				}
				if (tower.NoteSlot != null)
				{
					towerInstance.NoteSlot = tower.NoteSlot;
				}
				if (tower.BottleSlot != null)
				{
					towerInstance.BottleSlot = tower.BottleSlot;
				}
				if (tower.ZoneCollection != null)
				{
					towerInstance.ZoneCollection = tower.ZoneCollection;
				}
				if (tower.ZonePurification != null)
				{
					towerInstance.ZonePurification = tower.ZonePurification;
				}
				if (tower.FillButton != null)
				{
					towerInstance.FillButton = tower.FillButton;
				}
				Last = towerInstance;
				return towerInstance;
			}
			Log.Info("水塔登记：旧记录已作废（读档重建），替换为新实例（新 " + Describe(tower.Item) + "）");
			Diag.Summary("register-replace", "读档重建：旧登记记录已作废，已用新实例替换（旧记录的槽引用会失效，这正是读档后塔「不工作」的原因）");
			_all.Remove(towerInstance);
			if (Last == towerInstance)
			{
				Last = null;
			}
		}
		tower.Index = _nextIndex++;
		_all.Add(tower);
		Last = tower;
		try
		{
			WaterTowerWindow.RelockFixedParts(tower, verbose: false);
		}
		catch
		{
		}
		Log.Info("水塔登记：第 " + tower.Index + " 台已登记（" + Describe(tower.Item) + "，当前共 " + _all.Count + " 台）");
		return tower;
	}

	private static bool SameObject(object a, object b)
	{
		return WaterTowerItemFactory.SameRef(a, b);
	}

	public static TowerInstance FindByButton(object button)
	{
		if (button == null)
		{
			return null;
		}
		IntPtr intPtr = PointerOf(button);
		if (intPtr == IntPtr.Zero)
		{
			return null;
		}
		foreach (TowerInstance item in _all)
		{
			if (item == null || item.FillButton == null || !(PointerOf(item.FillButton) == intPtr))
			{
				continue;
			}
			return item;
		}
		return null;
	}

	public static TowerInstance Find(object item)
	{
		if (item == null)
		{
			return null;
		}
		IntPtr intPtr = PointerOf(item);
		string text = UniqueIdOf(item);
		foreach (TowerInstance item2 in _all)
		{
			if (item2.Item != null)
			{
				if (item2.Item == item)
				{
					return item2;
				}
				if (intPtr != IntPtr.Zero && PointerOf(item2.Item) == intPtr)
				{
					return item2;
				}
				if (!string.IsNullOrEmpty(text) && UniqueIdOf(item2.Item) == text)
				{
					return item2;
				}
			}
		}
		return null;
	}

	public static string Describe(object item)
	{
		if (item == null)
		{
			return "<null>";
		}
		IntPtr intPtr = PointerOf(item);
		string text = UniqueIdOf(item);
		return "ptr=" + ((intPtr == IntPtr.Zero) ? "0" : ((long)intPtr).ToString("x")) + " uid=" + (string.IsNullOrEmpty(text) ? "<无>" : text);
	}

	public static string UniqueIdOf(object item)
	{
		if (item == null)
		{
			return null;
		}
		try
		{
			return WaterTowerItemFactory.GetMemberValue(item.GetType(), item, "uniqueId")?.ToString();
		}
		catch
		{
			return null;
		}
	}

	public static object ResolveTank(TowerInstance tower)
	{
		if (tower == null)
		{
			return null;
		}
		object obj = WaterTowerWindow.ItemInSlot(tower.TankSlot, "watertower_tank");
		if (obj != null)
		{
			return obj;
		}
		if (tower.TankItem != null)
		{
			return tower.TankItem;
		}
		return FindDescendant(tower.Item, "watertower_tank");
	}

	public static bool IsAlive(TowerInstance tower)
	{
		if (tower == null || tower.Item == null)
		{
			return false;
		}
		if (tower.TankSlot != null)
		{
			return WaterTowerWindow.SlotHolds(tower.TankSlot, "watertower_tank");
		}
		return tower.TankItem != null;
	}

	public static int Refresh()
	{
		LastScanOk = false;
		object obj = ItemRegistry.TryGetStore();
		if (obj == null)
		{
			return 0;
		}
		List<object> list = null;
		try
		{
			object collection = WaterTowerItemFactory.CallInstance(obj.GetType(), obj, "FindAllItem", new object[1] { true }, "FindAllItem(true)");
			list = ItemRegistry.EnumerateIl2Cpp(collection);
		}
		catch (Exception ex)
		{
			Log.Debug("TowerRegistry.Refresh 扫描失败：" + ex.GetType().Name);
			Diag.Exception("refresh-scan", ex, "水塔扫描失败（登记表保持原状）");
			return 0;
		}
		if (list == null || list.Count == 0)
		{
			return 0;
		}
		LastScanOk = true;
		int num = 0;
		List<TowerInstance> list2 = new List<TowerInstance>();
		foreach (object item in list)
		{
			if (item == null)
			{
				continue;
			}
			string text = WaterTowerItemFactory.GetMemberValue(item.GetType(), item, "identifier") as string;
			if (text != "watertower")
			{
				continue;
			}
			TowerInstance towerInstance = Find(item);
			if (towerInstance == null)
			{
				object obj2 = FindDescendant(item, "watertower_tank");
				object noteItem = FindDescendant(item, "postit");
				towerInstance = Register(new TowerInstance
				{
					Item = item,
					TankItem = obj2,
					NoteItem = noteItem
				});
				Log.Info("发现一台读档恢复的水塔，已登记（蓄水箱" + ((obj2 != null) ? "已找到" : "未找到") + "）");
				num++;
			}
			if (towerInstance != null)
			{
				WaterTowerWindow.TryDiscoverSlots(towerInstance);
				if (!list2.Contains(towerInstance))
				{
					list2.Add(towerInstance);
				}
			}
		}
		foreach (TowerInstance item2 in list2)
		{
			WaterTowerItemFactory.EnsureOwned(item2.Item, item2.Label + " 塔本体");
			if (item2.TankItem != null)
			{
				WaterTowerItemFactory.EnsureOwned(item2.TankItem, item2.Label + " 蓄水箱");
			}
		}
		PruneSameLineage();
		Prune(list, list2);
		return num;
	}

	public static void RestoreAfterLoad()
	{
		try
		{
			int num = Refresh();
			int num2 = MaintainTowers(verbose: true);
			if (_all.Count > 0)
			{
				Diag.Summary("restore", "读档恢复：当前登记 " + _all.Count + " 台塔（本次新增 " + num + "），补锁 " + num2 + " 个槽");
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("restore-ex", ex, "读档恢复失败（已忽略）");
		}
	}

	public static int MaintainTowers(bool verbose)
	{
		int num = 0;
		try
		{
			List<TowerInstance> list = new List<TowerInstance>(_all);
			foreach (TowerInstance item in list)
			{
				if (item != null && item.Item != null)
				{
					num += WaterTowerWindow.RelockFixedParts(item, verbose);
				}
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("maintain", ex, "维护补锁异常（已忽略）");
		}
		return num;
	}

	private static void Prune(List<object> playerItems, List<TowerInstance> scanned)
	{
		if (!LastScanOk)
		{
			return;
		}
		for (int num = _all.Count - 1; num >= 0; num--)
		{
			TowerInstance towerInstance = _all[num];
			if (towerInstance == null || towerInstance.Item == null)
			{
				RemoveAt(num, towerInstance);
			}
			else if (!scanned.Contains(towerInstance) && (playerItems == null || Find(playerItems, towerInstance.Item) == null))
			{
				Log.Info("水塔登记：清理已离开玩家的记录（第 " + towerInstance.Index + " 台，" + Describe(towerInstance.Item) + "）");
				Diag.Summary("prune-" + towerInstance.Index, "清理登记：第 " + towerInstance.Index + " 台水塔已不在玩家手里（卖掉/丢掉/被销毁），不再结算它");
				RemoveAt(num, towerInstance);
			}
		}
	}

	private static void RemoveAt(int index, TowerInstance t)
	{
		if (Last == t)
		{
			Last = null;
		}
		if (index >= 0 && index < _all.Count)
		{
			_all.RemoveAt(index);
		}
	}

	private static void PruneSameLineage()
	{
		if (!LastScanOk)
		{
			return;
		}
		Dictionary<string, TowerInstance> dictionary = new Dictionary<string, TowerInstance>();
		List<TowerInstance> list = new List<TowerInstance>();
		foreach (TowerInstance item2 in _all)
		{
			if (item2 == null || item2.Item == null)
			{
				continue;
			}
			object obj = ResolveTank(item2);
			string text = ((obj == null) ? null : UniqueIdOf(obj));
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			if (!dictionary.TryGetValue(text, out var value))
			{
				dictionary[text] = item2;
				continue;
			}
			bool flag = !WaterTowerItemFactory.IsItemDestroyed(item2.Item);
			bool flag2 = !WaterTowerItemFactory.IsItemDestroyed(value.Item);
			TowerInstance item;
			if (flag && !flag2)
			{
				item = value;
				dictionary[text] = item2;
			}
			else if (!flag && flag2)
			{
				item = item2;
			}
			else
			{
				item = value;
				dictionary[text] = item2;
			}
			list.Add(item);
		}
		foreach (TowerInstance item3 in list)
		{
			Log.Warn("水塔登记：剪掉同代系旧记录（第 " + item3.Index + " 台，" + Describe(item3.Item) + "）—— 与保留的那台蓄水箱 uid 相同（同一座塔的存档代次，源头见 docs/07 第十七节），今后只结算保留的那台");
			_all.Remove(item3);
			if (Last == item3)
			{
				Last = null;
			}
		}
	}

	private static object Find(List<object> list, object item)
	{
		IntPtr intPtr = PointerOf(item);
		string text = UniqueIdOf(item);
		foreach (object item2 in list)
		{
			if (item2 != null)
			{
				if (item2 == item)
				{
					return item2;
				}
				if (intPtr != IntPtr.Zero && PointerOf(item2) == intPtr)
				{
					return item2;
				}
				if (!string.IsNullOrEmpty(text) && UniqueIdOf(item2) == text)
				{
					return item2;
				}
			}
		}
		return null;
	}

	private static IntPtr PointerOf(object obj)
	{
		try
		{
			Il2CppObjectBase val = (Il2CppObjectBase)((obj is Il2CppObjectBase) ? obj : null);
			return (val == null) ? IntPtr.Zero : val.Pointer;
		}
		catch
		{
			return IntPtr.Zero;
		}
	}

	public static object FindDescendant(object root, string identifier)
	{
		List<object> list = Descendants(root);
		foreach (object item in list)
		{
			if (item != null)
			{
				string text = WaterTowerItemFactory.GetMemberValue(item.GetType(), item, "identifier") as string;
				if (text == identifier)
				{
					return item;
				}
			}
		}
		return null;
	}

	public static List<object> Descendants(object root)
	{
		List<object> list = new List<object>();
		if (root == null)
		{
			return list;
		}
		try
		{
			Il2CppObjectBase val = (Il2CppObjectBase)((root is Il2CppObjectBase) ? root : null);
			if (val == null)
			{
				return list;
			}
			GraphNodeStorage val2 = val.TryCast<GraphNodeStorage>();
			if (val2 == null)
			{
				LogOnceGraphFailure("TryCast<GraphNodeStorage> 返回 null");
				return list;
			}
			List<GameItem> val3 = GraphUtils.FindAllChildrenType<GameItem>(val2, (Func<GameItem, bool>)null, (Func<GameItem, bool>)null);
			if (val3 != null)
			{
				List<object> list2 = ItemRegistry.EnumerateIl2Cpp(val3);
				if (list2 != null)
				{
					list.AddRange(list2);
				}
			}
			_graphFailureLogged = false;
		}
		catch (Exception ex)
		{
			LogOnceGraphFailure(ex.GetType().Name + "：" + ex.Message);
		}
		return list;
	}

	private static void LogOnceGraphFailure(string reason)
	{
		if (!_graphFailureLogged)
		{
			_graphFailureLogged = true;
			Log.Warn("TowerRegistry：物品图遍历失败（" + reason + "）—— 读档恢复的塔会退化成按分区找机器");
			Diag.Once("graph-fail", "物品图遍历失败（" + reason + "）：读档恢复的塔会退化按分区找机器/水箱；若同时报「集水器 0 台」，说明这一晚的机器是靠分区兜底（分区引用可能已失效）");
		}
	}
}
