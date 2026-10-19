using System;
using Il2Cpp;
using MelonLoader;

namespace WaterTower;

public static class JacksonSell
{
	private static int _lastTriggerTick = -1073741824;

	public static void OnPlaceInventorInventory()
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (tickCount - _lastTriggerTick < 2000)
			{
				return;
			}
			_lastTriggerTick = tickCount;
			object obj = ItemRegistry.TryGetStore();
			if (obj == null)
			{
				Diag.Once("jackson-nostore", "[杰克逊] 拿不到 PlayerStore（主菜单/未进存档）→ 本次不摆货架");
				return;
			}
			int dayCounter;
			try
			{
				dayCounter = StoreStation.GetDayCounter();
			}
			catch
			{
				Diag.Once("jackson-noday", "[杰克逊] 读不到游戏日（StoreStation.GetDayCounter 抛异常）→ 本次不摆货架");
				return;
			}
			if (dayCounter < 15)
			{
				ReportVisit(dayCounter, "未上架（第 " + 15 + " 天起才卖）", -1, -1);
				return;
			}
			int num = CountOwnedTowers(obj);
			if (num >= 2)
			{
				ReportVisit(dayCounter, "未上架（已拥有上限台数）", num, -1);
				return;
			}
			int num2 = ((num == 0) ? 100 : 25);
			if (!RNG.Roll(num2))
			{
				ReportVisit(dayCounter, "未上架（掷骰未中）", num, num2);
				return;
			}
			object obj3 = WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("DirectoryMaster"), "Item", new object[2] { "watertower", false }, "DirectoryMaster.Item(watertower, false)");
			string text = ((obj3 == null) ? null : (WaterTowerItemFactory.GetMemberValue(obj3.GetType(), obj3, "identifier") as string));
			if (text != "watertower")
			{
				MelonLogger.Error("[WaterTower] 杰克逊货架造物未命中工厂（identifier=" + (text ?? "<null>") + "）—— 物品注册失败，本次及后续都不会上架");
				ReportVisit(dayCounter, "未上架（造物未命中工厂，identifier=" + (text ?? "<null>") + "）", num, num2);
				return;
			}
			TowerInstance last = TowerRegistry.Last;
			try
			{
				WaterTowerWindow.EnsureTank(last);
			}
			catch
			{
			}
			try
			{
				WaterTowerWindow.EnsureNote(last);
			}
			catch
			{
			}
			try
			{
				WaterTowerWindow.RefreshStatus(last);
			}
			catch
			{
			}
			WaterTowerItemFactory.CallInstance(obj.GetType(), obj, "AddDirectSellingItemToTable", new object[5] { obj3, false, false, false, 0 }, "AddDirectSellingItemToTable");
			ReportVisit(dayCounter, "**已摆上售货桌**", num, num2);
		}
		catch
		{
		}
	}

	public static void OnBoughtFromTable(object store)
	{
		try
		{
			if (store != null)
			{
				string via;
				object memberRobust = WaterTowerItemFactory.GetMemberRobust(store.GetType(), store, "currentNegociatedItem", out via);
				string text = ((memberRobust == null) ? null : (WaterTowerItemFactory.GetMemberValue(memberRobust.GetType(), memberRobust, "identifier") as string));
				if (!(text != "watertower"))
				{
					WaterTowerItemFactory.EnsureOwned(memberRobust, "买回的水塔");
				}
			}
		}
		catch
		{
		}
	}

	private static void ReportVisit(int day, string reason, int owned, int chance)
	{
		string text = "";
		if (owned >= 0)
		{
			text = text + "；玩家已拥有 " + owned + " 台（上限 " + 2 + "）";
		}
		if (chance >= 0)
		{
			text = text + "；本次几率 " + chance + "%";
		}
		Diag.Summary("jackson-visit", "[杰克逊] 第 " + day + " 天重建货架：" + reason + text);
	}

	private static int CountOwnedTowers(object store)
	{
		try
		{
			object obj = WaterTowerItemFactory.CallInstance(store.GetType(), store, "FindAllItem", new object[1] { true }, "FindAllItem(true)");
			if (obj == null)
			{
				return 0;
			}
			int num = 0;
			foreach (object item in ItemRegistry.EnumerateIl2Cpp(obj))
			{
				if (item != null)
				{
					string text = WaterTowerItemFactory.GetMemberValue(item.GetType(), item, "identifier") as string;
					if (text == "watertower")
					{
						num++;
					}
				}
			}
			return num;
		}
		catch
		{
			return 0;
		}
	}
}
