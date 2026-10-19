using System;
using System.Collections.Generic;

namespace WaterTower;

public static class ZoneTagger
{
	private static int _taggedTotal;

	private static int _tick;

	private static int _maintainTick;

	private const int MaintainInterval = 600;

	private static int _taggedByFactory;

	private const int TickInterval = 300;

	public static void Tick()
	{
		if (TowerRegistry.Count == 0)
		{
			return;
		}
		_tick++;
		if (_tick % 300 == 0)
		{
			try
			{
				TagAll(verbose: false);
			}
			catch (Exception ex)
			{
				Log.Debug("ZoneTagger.Tick 异常（已忽略）：" + ex.GetType().Name);
				Diag.Exception("zone-tick-ex", ex, "轮询补 tag 异常（已忽略）");
			}
		}
		_maintainTick++;
		if (_maintainTick % 600 != 0)
		{
			return;
		}
		try
		{
			TowerRegistry.MaintainTowers(verbose: false);
		}
		catch (Exception ex2)
		{
			Diag.Exception("tick-maintain", ex2, "轮询补锁维护异常（已忽略）");
		}
	}

	public static bool TagIfMachine(object item)
	{
		try
		{
			if (item == null)
			{
				return false;
			}
			bool flag = TagOne(item, verbose: false);
			if (flag)
			{
				_taggedByFactory++;
				Diag.Summary("zone-tag-factory", "机器创建即补 tag：本场累计 " + _taggedByFactory + " 台（本条是 " + Identifier(item) + "）");
			}
			return flag;
		}
		catch (Exception ex)
		{
			Diag.Exception("zone-tag-factory-ex", ex, "机器创建时补 tag 失败");
			return false;
		}
	}

	private static string Identifier(object item)
	{
		try
		{
			return (WaterTowerItemFactory.GetMemberValue(item.GetType(), item, "identifier") as string) ?? "?";
		}
		catch
		{
			return "?";
		}
	}

	public static int TagAll(bool verbose)
	{
		object obj = ItemRegistry.TryGetStore();
		if (obj == null)
		{
			return 0;
		}
		object obj2 = WaterTowerItemFactory.CallInstance(obj.GetType(), obj, "FindAllItem", new object[1] { true }, "FindAllItem(true)");
		if (obj2 == null)
		{
			return 0;
		}
		List<object> list = ItemRegistry.EnumerateIl2Cpp(obj2);
		if (list == null)
		{
			return 0;
		}
		int num = 0;
		foreach (object item in list)
		{
			num += (TagOne(item, verbose) ? 1 : 0);
		}
		if (num > 0)
		{
			_taggedTotal += num;
			Diag.Summary("zone-tag-scan", "轮询补 tag：本轮 " + num + " 件（本场轮询累计 " + _taggedTotal + " 件；加上「创建时」通道，本场合计 " + (_taggedTotal + _taggedByFactory) + " 件）");
		}
		return num;
	}

	public static bool TagItem(object item, bool verbose)
	{
		return TagOne(item, verbose);
	}

	private static bool TagOne(object item, bool verbose)
	{
		if (item == null)
		{
			return false;
		}
		if (WaterTowerItemFactory.IsItemDestroyed(item))
		{
			return false;
		}
		Type type = item.GetType();
		string text = WaterTowerItemFactory.GetMemberValue(type, item, "identifier") as string;
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		if (Array.IndexOf(WaterTowerConfig.ZoneTagStripByItemId, text) >= 0)
		{
			StripZoneTags(item, text);
			return false;
		}
		string[][] zoneTagByItemId = WaterTowerConfig.ZoneTagByItemId;
		foreach (string[] array in zoneTagByItemId)
		{
			if (!(array[0] != text))
			{
				string text2 = array[1];
				if (HasTag(item, text2))
				{
					return false;
				}
				object result;
				bool flag = WaterTowerItemFactory.TryCallInstance(type, item, "EnableTag", new object[2] { text2, false }, out result);
				if (flag)
				{
					Log.Debug("分区 tag：已给 " + text + " 补上 " + text2);
				}
				else
				{
					Diag.Once("zone-tag-fail-" + text, "补 tag 失败：" + text + " ← " + text2 + "（EnableTag 调用未成功，见调试版日志的同名步骤）");
				}
				return flag;
			}
		}
		return false;
	}

	private static void StripZoneTags(object item, string id)
	{
		Type type = item.GetType();
		string[] zoneTagsAll = WaterTowerConfig.ZoneTagsAll;
		foreach (string text in zoneTagsAll)
		{
			if (HasTag(item, text) && WaterTowerItemFactory.TryCallInstance(type, item, "DisableTag", new object[2] { text, false }, out var _))
			{
				Log.Info("分区 tag：已撤销 " + text + " ← " + id);
			}
		}
	}

	public static bool HasTag(object item, string tag)
	{
		if (item == null || string.IsNullOrEmpty(tag))
		{
			return false;
		}
		try
		{
			if (!WaterTowerItemFactory.TryCallInstance(item.GetType(), item, "IsTag", new object[1] { tag }, out var result))
			{
				return false;
			}
			return result is bool && (bool)result;
		}
		catch
		{
			return false;
		}
	}
}
