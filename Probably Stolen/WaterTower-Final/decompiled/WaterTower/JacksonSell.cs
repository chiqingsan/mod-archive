using System;
using Il2Cpp;
using MelonLoader;

namespace WaterTower;

public static class JacksonSell
{
	private static long _lastTriggerTick = -4611686018427387904L;

	public static void OnPlaceInventorInventory()
	{
		try
		{
			long tickCount = Environment.TickCount64;
			if (tickCount - _lastTriggerTick < 2000)
			{
				Diag.Once("jackson-tick", "[杰克逊] 冷却挡下一次货架重建回调（距上次 " + (tickCount - _lastTriggerTick) + " ms；正常情形 = 原版同一次重建连发两次回调）");
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
			if (num >= 1)
			{
				ReportVisit(dayCounter, "未上架（已拥有水塔；本 MOD 只出售一台）", num, -1);
				return;
			}
			int num2 = 100;
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
			catch (Exception ex)
			{
				Diag.Exception("jackson-tank", ex, "[杰克逊] 蓄水箱创建失败（仍上架，塔可能缺件）");
			}
			try
			{
				WaterTowerWindow.EnsureNote(last);
			}
			catch (Exception ex2)
			{
				Diag.Exception("jackson-note", ex2, "[杰克逊] 便签创建失败（仍上架，塔可能缺件）");
			}
			try
			{
				WaterTowerWindow.RefreshStatus(last);
			}
			catch (Exception ex3)
			{
				Diag.Exception("jackson-status", ex3, "[杰克逊] 刷新描述失败（仍上架）");
			}
			WaterTowerItemFactory.CallInstance(obj.GetType(), obj, "AddDirectSellingItemToTable", new object[5] { obj3, false, false, false, 0 }, "AddDirectSellingItemToTable");
			ReportVisit(dayCounter, "**已摆上售货桌**", num, num2);
		}
		catch (Exception ex4)
		{
			Diag.Exception("jackson-fail", ex4, "[杰克逊] 摆桌流程异常（已吞掉，不影响游戏；本次没上架）");
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
				if (!(((memberRobust == null) ? null : (WaterTowerItemFactory.GetMemberValue(memberRobust.GetType(), memberRobust, "identifier") as string)) != "watertower"))
				{
					WaterTowerItemFactory.EnsureOwned(memberRobust, "买回的水塔");
				}
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("jackson-buy", ex, "[杰克逊] 买回水塔的所有权保险失败（塔可能拖不动；不影响游戏）");
		}
	}

	private static void ReportVisit(int day, string reason, int owned, int chance)
	{
		string text = "";
		if (owned >= 0)
		{
			text = text + "；玩家已拥有 " + owned + " 台（上限 " + 1 + "）";
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
				if (item != null && WaterTowerItemFactory.GetMemberValue(item.GetType(), item, "identifier") as string == "watertower")
				{
					num++;
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
