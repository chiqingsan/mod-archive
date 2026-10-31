using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

public static class TowerDragPour
{
	private const int KindNone = 0;

	private const int KindContainer = 1;

	private const int KindDrain = 2;

	private const int KindFaucet = 3;

	private const int FaucetMaxAddUl = 9999999;

	private const int FaucetWaterGrade = 1;

	private static bool _installed;

	public static void EnsureInstalled()
	{
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		if (_installed || !WaterTowerConfig.DragPourEnabled)
		{
			return;
		}
		try
		{
			Harmony modHarmony = WaterTowerMod.ModHarmony;
			if (modHarmony == null)
			{
				Diag.Once("dragpour-no-target", "拖动倒水：补丁未安装（拿不到 Harmony 实例）→ 本功能不可用");
				return;
			}
			Type typeFromHandle = typeof(GameItem);
			MethodInfo methodInfo = AccessTools.Method(typeFromHandle, "MayTarget", new Type[1] { typeFromHandle }, (Type[])null);
			MethodInfo methodInfo2 = AccessTools.Method(typeFromHandle, "CanTarget", new Type[1] { typeFromHandle }, (Type[])null);
			MethodInfo methodInfo3 = AccessTools.Method(typeFromHandle, "Target", new Type[1] { typeFromHandle }, (Type[])null);
			MethodInfo methodInfo4 = AccessTools.Method(typeof(TowerDragPour), "MayTargetPostfix", (Type[])null, (Type[])null);
			MethodInfo methodInfo5 = AccessTools.Method(typeof(TowerDragPour), "CanTargetPostfix", (Type[])null, (Type[])null);
			MethodInfo methodInfo6 = AccessTools.Method(typeof(TowerDragPour), "TargetPostfix", (Type[])null, (Type[])null);
			if (methodInfo == null || methodInfo2 == null || methodInfo3 == null || methodInfo4 == null || methodInfo5 == null || methodInfo6 == null)
			{
				Diag.Once("dragpour-no-target", "拖动倒水：补丁未安装（GameItem.Target 目标方法找不到，可能是游戏更新改了签名）→ 本功能不可用");
				return;
			}
			modHarmony.Patch((MethodBase)methodInfo, (HarmonyMethod)null, new HarmonyMethod(methodInfo4), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			modHarmony.Patch((MethodBase)methodInfo2, (HarmonyMethod)null, new HarmonyMethod(methodInfo5), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			modHarmony.Patch((MethodBase)methodInfo3, (HarmonyMethod)null, new HarmonyMethod(methodInfo6), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			_installed = true;
		}
		catch (Exception ex)
		{
			Diag.Exception("dragpour-no-target", ex, "拖动倒水：补丁安装失败 → 本功能不可用");
		}
	}

	private static void MayTargetPostfix(GameItem __instance, GameItem targetItem, ref bool __result)
	{
		if (__result)
		{
			return;
		}
		try
		{
			if (IsPourSource(__instance, targetItem))
			{
				__result = true;
			}
		}
		catch
		{
		}
	}

	private static void CanTargetPostfix(GameItem __instance, GameItem targetItem, ref bool __result)
	{
		if (__result)
		{
			return;
		}
		try
		{
			if (IsPourSource(__instance, targetItem))
			{
				__result = true;
			}
		}
		catch
		{
		}
	}

	private static void TargetPostfix(GameItem __instance, GameItem targetItem)
	{
		if (!WaterTowerConfig.DragPourEnabled)
		{
			return;
		}
		string text = "水塔?";
		try
		{
			TowerInstance towerInstance = TowerOf(__instance);
			if (towerInstance == null)
			{
				return;
			}
			text = towerInstance.Label;
			int num = KindOf(targetItem);
			switch (num)
			{
			case 0:
				return;
			case 1:
				if (!ZoneTagger.HasTag(targetItem, "IS_OWNED_TAG"))
				{
					return;
				}
				break;
			}
			object obj = TowerRegistry.ResolveTank(towerInstance);
			if (obj == null)
			{
				Diag.Once("dragpour-broken-" + text, "[" + text + "] 拖动倒水：取不到蓄水箱引用（塔可能未完整构造），本次没有搬运");
			}
			else if (!WaterTowerItemFactory.SameRef(obj, targetItem) && !WaterTowerItemFactory.SameRef(towerInstance.Item, targetItem))
			{
				towerInstance.TankItem = obj;
				Act(towerInstance, obj, targetItem, num);
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("dragpour-ex-" + text, ex, "[" + text + "] 拖动倒水失败（已忽略）");
		}
	}

	private static bool IsPourSource(GameItem src, GameItem dst)
	{
		if (src == null || dst == null || WaterTowerItemFactory.SameRef(src, dst))
		{
			return false;
		}
		if (TowerOf(src) == null)
		{
			return false;
		}
		return KindOf(dst) switch
		{
			0 => false, 
			1 => ZoneTagger.HasTag(dst, "IS_OWNED_TAG"), 
			_ => true, 
		};
	}

	private static TowerInstance TowerOf(object src)
	{
		if (src == null)
		{
			return null;
		}
		try
		{
			List<TowerInstance> all = TowerRegistry.All;
			for (int i = 0; i < all.Count; i++)
			{
				TowerInstance towerInstance = all[i];
				if (towerInstance != null && towerInstance.Item != null && WaterTowerItemFactory.SameRef(towerInstance.Item, src))
				{
					return towerInstance;
				}
			}
		}
		catch
		{
		}
		return null;
	}

	private static int KindOf(GameItem dst)
	{
		if (dst == null)
		{
			return 0;
		}
		string identifier = dst.identifier;
		if (!string.IsNullOrEmpty(identifier))
		{
			if (identifier == "drain")
			{
				return 2;
			}
			if (identifier == "faucet")
			{
				return 3;
			}
		}
		return dst.IsTag("LIQUID_CONTAINER_TAG") ? 1 : 0;
	}

	private static void Act(TowerInstance tower, object tank, object dst, int kind)
	{
		Type type = WaterTowerItemFactory.ResolveType("WaterHelper");
		if (type == null)
		{
			Diag.Once("dragpour-broken-" + tower.Label, "[" + tower.Label + "] 拖动倒水：找不到 WaterHelper，本次没有搬运");
			return;
		}
		int capMl = CapacityMl(type, tank);
		int num = TankUl(type, tank);
		string text = NameOf(dst, kind);
		if (num <= 0)
		{
			LogLine(tower, text, capMl, num, num, "倒出", 0, -1, -1, "（蓄水箱是空的）");
			return;
		}
		switch (kind)
		{
		case 1:
		{
			int num4 = ToInt(WaterTowerItemFactory.CallStatic(type, "GetFreeCapacity", new object[1] { dst }, "GetFreeCapacity(倒水目标)"));
			if (num4 <= 0)
			{
				LogLine(tower, text, capMl, num, num, "倒出", 0, num4, num4, "（目标已满）");
				return;
			}
			WaterTowerItemFactory.CallStatic(type, "TransferLiquid", new object[2] { tank, dst }, "TransferLiquid(蓄水箱 → " + text + ")");
			int num5 = TankUl(type, tank);
			int freeAfterUl = ToInt(WaterTowerItemFactory.CallStatic(type, "GetFreeCapacity", new object[1] { dst }, "GetFreeCapacity(倒水目标·倒完)"));
			LogLine(tower, text, capMl, num, num5, "倒出", Ml(num) - Ml(num5), num4, freeAfterUl, null);
			break;
		}
		case 2:
		{
			WaterTowerItemFactory.CallStatic(type, "EmptyContainer", new object[1] { tank }, "EmptyContainer(蓄水箱)");
			WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("AudioHelper"), "DumpWater", null, "AudioHelper.DumpWater()");
			int num3 = TankUl(type, tank);
			LogLine(tower, text, capMl, num, num3, "倒掉", Ml(num) - Ml(num3), -1, -1, null);
			break;
		}
		case 3:
		{
			if (!TryResolveFaucetPurity(tower, out var purity))
			{
				LogLine(tower, text, capMl, num, num, "加水", 0, -1, -1, "（水龙头未解锁，本次没有加水）");
				return;
			}
			WaterTowerItemFactory.CallStatic(type, "AddWater", new object[7] { tank, 1, 9999999, true, purity, purity, true }, "AddWater(蓄水箱, 水龙头)");
			WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("AudioHelper"), "UseFaucet", null, "AudioHelper.UseFaucet()");
			int num2 = TankUl(type, tank);
			LogLine(tower, text, capMl, num, num2, "加水", Ml(num2) - Ml(num), -1, -1, (num2 > num) ? "（水龙头加水：游戏水源，不是水塔自产）" : "（蓄水箱已满）");
			break;
		}
		}
		WaterTowerWindow.RefreshStatus(tower);
	}

	private static bool TryResolveFaucetPurity(TowerInstance tower, out int purity)
	{
		purity = 0;
		try
		{
			NetworkUpgrade upgradeById = NetworkUpgrade.GetUpgradeById("RUSTWATER_TAP");
			if (upgradeById != null && upgradeById.IsUnlocked())
			{
				StoreStation instance = StoreStation.Instance;
				if (instance == null)
				{
					return false;
				}
				purity = instance.todayRustwaterPurity;
				return true;
			}
			NetworkUpgrade upgradeById2 = NetworkUpgrade.GetUpgradeById("GUTTERFLOW_TAP");
			if (upgradeById2 != null && upgradeById2.IsUnlocked())
			{
				StoreStation instance2 = StoreStation.Instance;
				if (instance2 == null)
				{
					return false;
				}
				purity = instance2.todayGutterflowPurity;
				return true;
			}
		}
		catch (Exception ex)
		{
			Diag.Once("dragpour-broken-" + tower.Label, "[" + tower.Label + "] 拖动倒水：读水龙头升级/水质失败（按未解锁处理）：" + ex.GetType().Name);
		}
		return false;
	}

	private static void LogLine(TowerInstance tower, string dstName, int capMl, int beforeUl, int afterUl, string verb, int movedMl, int freeUl, int freeAfterUl, string tail)
	{
		string text = "[" + tower.Label + "] 倒水（→ " + dstName + "）：蓄水箱 " + beforeUl / 1000 + " → " + afterUl / 1000 + " / " + capMl + " ml（" + verb + " " + movedMl + " ml）";
		if (freeUl >= 0 && freeAfterUl >= 0)
		{
			text = text + "；目标剩余容量 " + freeUl / 1000 + " → " + freeAfterUl / 1000 + " ml";
		}
		text = text + "（" + beforeUl + " → " + afterUl + " µl）";
		if (!string.IsNullOrEmpty(tail))
		{
			text += tail;
		}
		Log.Info(text);
	}

	private static string NameOf(object dst, int kind)
	{
		switch (kind)
		{
		case 2:
			return "排水口";
		case 3:
			return "水龙头";
		default:
			try
			{
				return (WaterTowerItemFactory.GetMemberValue(dst.GetType(), dst, "identifier") as string) ?? "储水容器";
			}
			catch
			{
				return "储水容器";
			}
		}
	}

	private static int TankUl(Type waterHelper, object tank)
	{
		return ToInt(WaterTowerItemFactory.CallStatic(waterHelper, "GetTotalVolume", new object[1] { tank }, "GetTotalVolume(蓄水箱)"));
	}

	private static int Ml(int ul)
	{
		return ul / 1000;
	}

	private static int CapacityMl(Type waterHelper, object tank)
	{
		int num = ToInt(WaterTowerItemFactory.CallStatic(waterHelper, "GetFreeCapacity", new object[1] { tank }, "GetFreeCapacity(蓄水箱·算容量)"));
		return Ml(TankUl(waterHelper, tank) + num);
	}

	private static int ToInt(object v)
	{
		return WaterTowerItemFactory.ToInt(v);
	}
}
