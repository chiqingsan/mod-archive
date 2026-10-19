using System;
using System.Collections.Generic;
using System.Reflection;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;

namespace WaterTower;

public static class WaterTowerWindow
{
	private const double FillClickDebounceSeconds = 0.35;

	private static readonly Dictionary<string, DateTime> _lastFillClickByTower = new Dictionary<string, DateTime>();

	public static bool SlotHolds(object slot, string identifier)
	{
		return SlotContains(slot, identifier);
	}

	public static bool BuildAndAttach(object item)
	{
		Log.Section("构建水塔内容窗口（手动拼装：双分区 + 蓄水箱 + 取水槽 + 按钮）");
		if (item == null)
		{
			Log.Error("item 为 null");
			return false;
		}
		Type type = item.GetType();
		object obj = CreateWindowShell();
		object obj2 = CreateLayoutGrid(1, 2);
		object obj3 = CreateLayoutGrid(1, 17);
		object obj4 = CreateLayoutGrid(4, 2);
		if (obj == null || obj2 == null || obj3 == null || obj4 == null)
		{
			Log.Error("窗口骨架构造失败，中止");
			return false;
		}
		object obj5 = CreateZone(16, 16);
		ApplyZoneShapeIfNeeded(obj5, "工作区", 16, 16);
		object obj6 = CreateTankSlot();
		object obj7 = CreateBottleSlot();
		object obj8 = CreateFillButton();
		object obj9 = CreateNoteSlot();
		object element = CreateLabel("工作区");
		object element2 = CreateLabel("蓄水箱");
		object element3 = CreateLabel("取水槽");
		object element4 = CreateLabel("备注");
		Type type2 = obj3.GetType();
		Attach(type2, obj3, element, 0, 0);
		Attach(type2, obj3, obj5, 0, 1);
		Type type3 = obj4.GetType();
		Attach(type3, obj4, element2, 0, 0);
		Attach(type3, obj4, element3, 1, 0);
		Attach(type3, obj4, element4, 3, 0);
		Attach(type3, obj4, obj6, 0, 1);
		Attach(type3, obj4, obj7, 1, 1);
		Attach(type3, obj4, obj8, 2, 1);
		Attach(type3, obj4, obj9, 3, 1);
		Type type4 = obj2.GetType();
		Attach(type4, obj2, obj3, 0, 0);
		Attach(type4, obj2, obj4, 0, 1);
		TowerInstance towerInstance = TowerRegistry.Register(new TowerInstance
		{
			Item = item,
			ZoneCollection = obj5,
			ZonePurification = obj5,
			TankSlot = obj6,
			NoteSlot = obj9,
			BottleSlot = obj7,
			FillButton = obj8
		});
		Type type5 = obj.GetType();
		WaterTowerItemFactory.CallInstance(type5, obj, "Attach", new object[1] { obj2 }, "PixelWindow.Attach(外层布局)");
		Log.Info("步骤 7  GameItem.SetContentWindow(window)");
		WaterTowerItemFactory.CallInstance(type, item, "SetContentWindow", new object[1] { obj }, "SetContentWindow");
		ModSprite.Ensure();
		Log.Info("步骤 7  设置贴图 " + ModSprite.AtlasPath + "/" + ModSprite.SpriteName + (ModSprite.Injected ? "（自定义 PNG）" : "（原版占位图）"));
		ModSprite.ApplyToItem(type, item, "步骤 7");
		ApplyFootprint(type, item);
		Log.Info("步骤 7  施加工作区白名单");
		ApplyWhitelist(obj5, item, "工作区", WaterTowerConfig.ZoneAllowedIds, WaterTowerConfig.ZoneTags);
		Log.Info("步骤 7  设置为容器：SetGameItemType(\"STORAGE\") + 两个 SYSTEM tag");
		WaterTowerItemFactory.CallInstance(type, item, "SetGameItemType", new object[1] { "STORAGE" }, "SetGameItemType(STORAGE)");
		WaterTowerItemFactory.CallInstance(type, item, "EnableTag", new object[2] { "SYSTEM_TAG", false }, "EnableTag(SYSTEM_TAG)");
		WaterTowerItemFactory.CallInstance(type, item, "EnableTag", new object[2] { "SYSTEM_TAG_UTILITY", false }, "EnableTag(SYSTEM_TAG_UTILITY)");
		ApplyIdentity(type, item);
		CheckElementHealth(item, "水塔本体");
		Log.Success("水塔内容窗口构建完成（工作区 " + 16 + "x" + 16 + ((towerInstance.TankItem != null) ? "，蓄水箱已锁定" : "，蓄水箱待创建（见后续 EnsureTank）") + "）");
		return true;
	}

	private static void ApplyIdentity(Type itemType, object item)
	{
		WaterTowerItemFactory.CallInstance(itemType, item, "SetName", new object[1] { "水塔" }, "SetName(水塔)");
		WaterTowerItemFactory.SetMemberValue(itemType, item, "shortDescription", WaterTowerConfig.TowerStatusText(0, 0, 0));
		WaterTowerItemFactory.SetMemberValue(itemType, item, "flavorText", "水塔，用于蓄水，您不再需要每日从集水器中重复取水。");
		object obj = WaterTowerItemFactory.CallInstance(itemType, item, "SetValue", new object[1] { 1000L }, "SetValue(" + 1000 + ")");
		Log.Info("   物品身份：name=水塔  价值=" + 1000 + "  备注=水塔，用于蓄水，您不再需要每日从集水器中重复取水。" + ((obj == null) ? "" : ("（SetValue 返回 " + obj?.ToString() + "）")));
	}

	private static object CreateWindowShell()
	{
		Type type = WaterTowerItemFactory.ResolveType("PixelWindow");
		if (type == null)
		{
			Log.Error("找不到 PixelWindow 类型");
			return null;
		}
		return WaterTowerItemFactory.NewInstance(type, new object[2] { true, "水塔" }, "new PixelWindow(true, 标题)");
	}

	private static object CreateLayoutGrid(int cols, int rows)
	{
		Type type = WaterTowerItemFactory.ResolveType("GridPixelElement");
		if (type == null)
		{
			Log.Error("找不到 GridPixelElement 类型");
			return null;
		}
		return WaterTowerItemFactory.NewInstance(type, new object[3] { cols, rows, false }, "new GridPixelElement(" + cols + ", " + rows + ", false)");
	}

	private static object CreateZone(int w, int h)
	{
		return CreateZoneFromVanillaWindow(w, h);
	}

	private static object CreateZoneFromVanillaWindow(int w, int h)
	{
		Type type = WaterTowerItemFactory.ResolveType("DirectoryUtils");
		if (type == null)
		{
			Log.Error("找不到 DirectoryUtils，无法建分区");
			return null;
		}
		object obj = WaterTowerItemFactory.CallStatic(type, "CreateInventoryWindow", new object[3] { w, h, true }, "CreateInventoryWindow(" + w + ", " + h + ", true)（原版构造器）", new Type[3]
		{
			typeof(int),
			typeof(int),
			typeof(bool)
		});
		if (obj == null)
		{
			Log.Error("CreateInventoryWindow 未返回结果");
			return null;
		}
		object tupleField = GetTupleField(obj, "Item1");
		object tupleField2 = GetTupleField(obj, "Item2");
		if (tupleField2 == null)
		{
			Log.Error("CreateInventoryWindow 的库存为 null");
			return null;
		}
		if (tupleField != null)
		{
			try
			{
				WaterTowerItemFactory.CallInstance(tupleField.GetType(), tupleField, "Detach", new object[1] { tupleField2 }, "捐赠窗口.Detach(库存)");
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Detach 分区失败（继续）");
			}
		}
		Log.Info("   分区已用原版构造器建好：" + tupleField2.GetType().Name);
		return tupleField2;
	}

	private static void ApplyZoneShapeIfNeeded(object zone, string label, int w, int h)
	{
		if (zone == null)
		{
			return;
		}
		Type type = zone.GetType();
		if (type.GetProperty("inventoryShape") != null || type.GetField("inventoryShape") != null)
		{
			ApplyZoneShape(zone, label, w, h);
			return;
		}
		Il2CppObjectBase val = (Il2CppObjectBase)((zone is Il2CppObjectBase) ? zone : null);
		GameGridScrollableInventory val2 = ((val == null) ? null : val.TryCast<GameGridScrollableInventory>());
		if (val2 != null)
		{
			Type type2 = ((object)val2).GetType();
			object memberValue = WaterTowerItemFactory.GetMemberValue(type2, val2, "width");
			object memberValue2 = WaterTowerItemFactory.GetMemberValue(type2, val2, "height");
			object result = null;
			WaterTowerItemFactory.TryCallInstance(type2, val2, "GetVisibleHeight", null, out result);
			Log.Info("   [" + label + "] 滚动网格分区：总 " + Fmt(memberValue) + "x" + Fmt(memberValue2) + "，一屏可见 " + Fmt(result) + " 行（可放置区域就是整块，无需设形状）");
		}
		else
		{
			Log.Info("   [" + label + "] 分区类型 " + type.Name + "（原版构造器）：可放置区域由构造参数 " + w + "x" + h + " 决定，不设形状");
		}
	}

	private static void ApplyZoneShape(object zone, string label, int w, int h)
	{
		if (zone == null)
		{
			return;
		}
		Type type = zone.GetType();
		LogShape(type, zone, label + "（设置前）");
		object obj = WaterTowerItemFactory.CallInstance(type, zone, "SetShape", new object[2] { w, h }, "分区.SetShape(" + w + ", " + h + ")", new Type[2]
		{
			typeof(int),
			typeof(int)
		});
		object obj2 = WaterTowerItemFactory.GetMemberValue(type, zone, "inventoryShape") ?? WaterTowerItemFactory.GetMemberValue(type, zone, "shape");
		if (obj != null && obj2 != null && ToInt(WaterTowerItemFactory.GetMemberValue(obj2.GetType(), obj2, "width")) == w && ToInt(WaterTowerItemFactory.GetMemberValue(obj2.GetType(), obj2, "height")) == h)
		{
			WaterTowerItemFactory.CallInstance(type, zone, "Validate", null, "Validate()");
			LogShape(type, zone, label + "（SetShape 后）");
			return;
		}
		Log.Warn("   [" + label + "] SetShape 未达预期，改用 GridShapeBuilder 写入 inventoryShape");
		Type type2 = WaterTowerItemFactory.ResolveType("GridShapeBuilder");
		if (type2 == null)
		{
			Log.Error("   [" + label + "] 找不到 GridShapeBuilder，无法设置形状");
			return;
		}
		object obj3 = WaterTowerItemFactory.NewInstance(type2, new object[2] { w, h }, "new GridShapeBuilder(" + w + ", " + h + ")");
		if (obj3 == null)
		{
			return;
		}
		object obj4 = WaterTowerItemFactory.CallInstance(obj3.GetType(), obj3, "SetDataFill", new object[3]
		{
			w,
			h,
			(byte)1
		}, "SetDataFill(" + w + ", " + h + ", 1)");
		object obj5 = obj4 ?? obj3;
		object obj6 = WaterTowerItemFactory.CallInstance(obj5.GetType(), obj5, "Build", null, "GridShapeBuilder.Build()");
		if (obj6 == null)
		{
			Log.Error("   [" + label + "] 形状构造失败");
			return;
		}
		if (!WaterTowerItemFactory.SetMemberValue(type, zone, "inventoryShape", obj6))
		{
			Log.Warn("   [" + label + "] inventoryShape 写入失败");
		}
		WaterTowerItemFactory.CallInstance(type, zone, "Validate", null, "Validate()");
		LogShape(type, zone, label + "（写入 inventoryShape 后）");
	}

	private static void LogShape(Type zoneType, object zone, string stage)
	{
		object obj = WaterTowerItemFactory.GetMemberValue(zoneType, zone, "inventoryShape") ?? WaterTowerItemFactory.GetMemberValue(zoneType, zone, "shape");
		if (obj == null)
		{
			Log.Warn("   " + stage + " 读不到形状");
			return;
		}
		Type type = obj.GetType();
		Log.Info("   " + stage + " 网格形状 = " + Fmt(WaterTowerItemFactory.GetMemberValue(type, obj, "width")) + " x " + Fmt(WaterTowerItemFactory.GetMemberValue(type, obj, "height")));
	}

	private static object CreateTankSlot()
	{
		Type type = WaterTowerItemFactory.ResolveType("GameSlotInventory");
		if (type == null)
		{
			Log.Error("蓄水箱：找不到 GameSlotInventory");
			return null;
		}
		object obj = WaterTowerItemFactory.NewInstance(type, new object[0], "new GameSlotInventory()（水箱槽）");
		if (obj == null)
		{
			return null;
		}
		WaterTowerItemFactory.CallInstance(type, obj, "SetBackgroundFadeSprite", new object[4] { "Items/water_container4", "xl", 0.5f, 0.5f }, "水箱槽背景贴图 Items/water_container4/xl");
		return obj;
	}

	private static object CreateNoteSlot()
	{
		Type type = WaterTowerItemFactory.ResolveType("GameSlotInventory");
		if (type == null)
		{
			Log.Error("便签槽：找不到 GameSlotInventory");
			return null;
		}
		object obj = WaterTowerItemFactory.NewInstance(type, new object[0], "new GameSlotInventory()（便签槽）");
		if (obj == null)
		{
			return null;
		}
		WaterTowerItemFactory.CallInstance(type, obj, "SetBackgroundFadeSprite", new object[4] { "Items/items_tool", "postit", 0.5f, 0.5f }, "便签槽背景贴图 Items/items_tool/postit");
		return obj;
	}

	public static bool EnsureTank(TowerInstance tower)
	{
		if (tower == null)
		{
			Log.Warn("EnsureTank：塔实例为空");
			return false;
		}
		if (tower.TankItem != null && tower.TankSlot != null && SlotContains(tower.TankSlot, "watertower_tank"))
		{
			return true;
		}
		if (tower.TankSlot == null)
		{
			TryDiscoverSlots(tower);
		}
		if (tower.TankSlot == null)
		{
			Log.Warn("EnsureTank：水箱槽尚未建立（窗口未构建）");
			return false;
		}
		Type type = tower.TankSlot.GetType();
		Log.Section("创建蓄水箱（物品创建完成之后） —— " + tower.Label);
		object obj = WaterTowerItemFactory.SpawnWaterTank();
		if (obj == null)
		{
			Log.Error("蓄水箱物品创建失败");
			return false;
		}
		CheckElementHealth(obj, "蓄水箱");
		WaterTowerItemFactory.CallInstance(type, tower.TankSlot, "UncheckedAccept", new object[1] { obj }, "水箱槽.UncheckedAccept(水箱)");
		WaterTowerItemFactory.CallInstance(type, tower.TankSlot, "LockInv", null, "水箱槽.LockInv()");
		object v = WaterTowerItemFactory.CallInstance(type, tower.TankSlot, "IsInsertLocked", null, "IsInsertLocked");
		object v2 = WaterTowerItemFactory.CallInstance(type, tower.TankSlot, "IsRemoveLocked", null, "IsRemoveLocked");
		Log.Info("   水箱槽锁定状态：insert=" + Fmt(v) + " remove=" + Fmt(v2));
		tower.TankItem = obj;
		return true;
	}

	public static bool EnsureNote(TowerInstance tower)
	{
		if (tower == null)
		{
			Log.Warn("EnsureNote：塔实例为空");
			return false;
		}
		if (tower.NoteItem != null && tower.NoteSlot != null && SlotContains(tower.NoteSlot, "postit"))
		{
			return true;
		}
		if (tower.NoteSlot == null)
		{
			TryDiscoverSlots(tower);
		}
		if (tower.NoteSlot == null)
		{
			Log.Warn("EnsureNote：便签槽尚未建立（窗口未构建）");
			return false;
		}
		Type type = WaterTowerItemFactory.ResolveType("DirectoryMaster");
		Type type2 = tower.NoteSlot.GetType();
		if (type == null)
		{
			Log.Error("EnsureNote：找不到 DirectoryMaster");
			return false;
		}
		Log.Section("创建便签（物品创建完成之后） —— " + tower.Label);
		object obj = WaterTowerItemFactory.CallStatic(type, "Item", new object[2] { "postit", true }, "DirectoryMaster.Item(\"postit\", true)（原版便签）");
		if (obj == null)
		{
			Log.Error("便签物品创建失败");
			return false;
		}
		Type type3 = obj.GetType();
		WaterTowerItemFactory.CallInstance(type3, obj, "SetName", new object[1] { "水塔" }, "SetName(水塔)");
		WaterTowerItemFactory.SetMemberValue(type3, obj, "shortDescription", "1.\u00a0工作区内需正常放置集水器或净水器，以及能量阵列。集水器和净水器内需安装电池，建议不放置储水容器。 2.\u00a0集水器或净水器内，若放置储水容器，则产物不会进入水塔的蓄水箱。 3.\u00a0集水器或净水器内，若不放置储水容器，则产物会直接进入水塔的蓄水箱。");
		WaterTowerItemFactory.SetMemberValue(type3, obj, "flavorText", "水塔");
		WaterTowerItemFactory.CallInstance(type3, obj, "SetGameItemType", new object[1] { "DOCUMENT" }, "SetGameItemType(DOCUMENT)");
		WaterTowerItemFactory.CallInstance(type3, obj, "EnableTag", new object[2] { "IMPORTANT_TAG", false }, "EnableTag(IMPORTANT_TAG)");
		WaterTowerItemFactory.CallInstance(type3, obj, "SetValue", new object[1] { 0L }, "SetValue(0)（便签不值钱）");
		CheckElementHealth(obj, "便签");
		WaterTowerItemFactory.CallInstance(type2, tower.NoteSlot, "UncheckedAccept", new object[1] { obj }, "便签槽.UncheckedAccept(便签)");
		WaterTowerItemFactory.CallInstance(type2, tower.NoteSlot, "LockInv", null, "便签槽.LockInv()");
		object v = WaterTowerItemFactory.CallInstance(type2, tower.NoteSlot, "IsInsertLocked", null, "IsInsertLocked");
		object v2 = WaterTowerItemFactory.CallInstance(type2, tower.NoteSlot, "IsRemoveLocked", null, "IsRemoveLocked");
		Log.Info("   便签槽锁定状态：insert=" + Fmt(v) + " remove=" + Fmt(v2) + "；便签文字 = 水塔");
		tower.NoteItem = obj;
		return true;
	}

	private static bool SlotContains(object slot, string identifier)
	{
		return ItemInSlot(slot, identifier) != null;
	}

	public static object ItemInSlot(object slot, string identifier)
	{
		if (slot == null || string.IsNullOrEmpty(identifier))
		{
			return null;
		}
		try
		{
			object memberValue = WaterTowerItemFactory.GetMemberValue(slot.GetType(), slot, "childItems");
			foreach (object item in ItemRegistry.EnumerateIl2Cpp(memberValue))
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
		}
		catch (Exception ex)
		{
			Log.Debug("ItemInSlot 读取失败：" + ex.GetType().Name);
		}
		return null;
	}

	public static int TryDiscoverSlots(TowerInstance tower)
	{
		if (tower == null || tower.Item == null)
		{
			return 0;
		}
		int num = 0;
		try
		{
			if (tower.TankItem == null || WaterTowerItemFactory.IsItemDestroyed(tower.TankItem))
			{
				object obj = TowerRegistry.ResolveTank(tower);
				if (obj != null && !SameRef(obj, tower.TankItem))
				{
					tower.TankItem = obj;
					num++;
				}
			}
			if (tower.TankSlot == null && tower.TankItem != null)
			{
				object obj2 = ParentInventoryOf(tower.TankItem);
				if (obj2 != null)
				{
					tower.TankSlot = obj2;
					num++;
				}
			}
			if (tower.NoteItem == null)
			{
				object obj3 = TowerRegistry.FindDescendant(tower.Item, "postit");
				if (obj3 != null)
				{
					tower.NoteItem = obj3;
					num++;
				}
			}
			if (tower.NoteSlot == null && tower.NoteItem != null)
			{
				object obj4 = ParentInventoryOf(tower.NoteItem);
				if (obj4 != null)
				{
					tower.NoteSlot = obj4;
					num++;
				}
			}
			if (tower.BottleSlot == null)
			{
				object obj5 = FindBottleSlotInWindow(tower);
				if (obj5 != null)
				{
					tower.BottleSlot = obj5;
					num++;
				}
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("discover-slots", ex, "回填槽引用失败（已忽略）");
		}
		if (num > 0)
		{
			Log.Info("   [" + tower.Label + "] 引用回填 " + num + " 项（parentInventory / 元素树反推）");
		}
		return num;
	}

	private static object FindBottleSlotInWindow(TowerInstance tower)
	{
		try
		{
			object memberValue = WaterTowerItemFactory.GetMemberValue(tower.Item.GetType(), tower.Item, "contentWindow");
			if (memberValue == null)
			{
				return null;
			}
			object memberValue2 = WaterTowerItemFactory.GetMemberValue(memberValue.GetType(), memberValue, "childElement");
			List<object> list = new List<object>();
			CollectSlotInventories(memberValue2, list, 0);
			foreach (object item in list)
			{
				if (SameRef(item, tower.TankSlot) || SameRef(item, tower.NoteSlot))
				{
					continue;
				}
				return item;
			}
			return null;
		}
		catch (Exception ex)
		{
			Diag.Exception("bottle-slot-scan", ex, "取水槽认领失败（已忽略）");
			return null;
		}
	}

	private static void CollectSlotInventories(object element, List<object> into, int depth)
	{
		if (element == null || depth > 6 || into.Count > 16)
		{
			return;
		}
		try
		{
			Il2CppObjectBase val = (Il2CppObjectBase)((element is Il2CppObjectBase) ? element : null);
			if (val == null)
			{
				return;
			}
			object obj = null;
			try
			{
				obj = val.TryCast<GameSlotInventory>();
			}
			catch
			{
			}
			if (obj != null)
			{
				into.Add(obj);
				return;
			}
			object memberValue = WaterTowerItemFactory.GetMemberValue(element.GetType(), element, "childElements");
			foreach (object item in ItemRegistry.EnumerateIl2Cpp(memberValue))
			{
				CollectSlotInventories(item, into, depth + 1);
			}
		}
		catch
		{
		}
	}

	private static bool SameRef(object a, object b)
	{
		return WaterTowerItemFactory.SameRef(a, b);
	}

	private static object ParentInventoryOf(object item)
	{
		try
		{
			return WaterTowerItemFactory.GetMemberValue(item.GetType(), item, "parentInventory");
		}
		catch
		{
			return null;
		}
	}

	public static void CheckElementHealth(object item, string label)
	{
		if (item == null)
		{
			return;
		}
		try
		{
			Type type = item.GetType();
			string[] array = new string[5] { "handler", "inner", "background", "spriteMods", "innateSpriteMods" };
			string[] array2 = array;
			foreach (string text in array2)
			{
				object memberValue = WaterTowerItemFactory.GetMemberValue(type, item, text);
				if (memberValue == null)
				{
					string text2 = (WaterTowerItemFactory.GetMemberValue(type, item, "identifier") as string) ?? "?";
					Diag.Once("element-" + text + "-" + text2, "元素体检异常：" + label + "（" + text2 + "）的 " + text + " == null —— 这正是玩家侧 ToggleBackground NRE 的成因（该字段只有构造函数写一次）");
				}
			}
			object memberValue2 = WaterTowerItemFactory.GetMemberValue(type, item, "background");
			if (memberValue2 != null)
			{
				object memberValue3 = WaterTowerItemFactory.GetMemberValue(memberValue2.GetType(), memberValue2, "image");
				if (memberValue3 == null)
				{
					string text3 = (WaterTowerItemFactory.GetMemberValue(type, item, "identifier") as string) ?? "?";
					Diag.Once("element-bgimage-" + text3, "元素体检异常：" + label + "（" + text3 + "）的 background.image == null");
				}
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("element-check", ex, "元素体检读取失败（已忽略）");
		}
	}

	public static int RelockFixedParts(TowerInstance tower, bool verbose)
	{
		int num = 0;
		try
		{
			if (tower == null || tower.Item == null)
			{
				return 0;
			}
			TryDiscoverSlots(tower);
			num += EnsureSlotLocked(tower.TankSlot, "watertower_tank", tower, "蓄水箱", verbose);
			num += EnsureSlotLocked(tower.NoteSlot, "postit", tower, "便签", verbose);
		}
		catch (Exception ex)
		{
			Diag.Exception("relock", ex, "补锁固定件失败（已忽略）");
		}
		return num;
	}

	private static int EnsureSlotLocked(object slot, string expectedId, TowerInstance tower, string label, bool verbose)
	{
		if (slot == null)
		{
			return 0;
		}
		try
		{
			if (!SlotContains(slot, expectedId))
			{
				return 0;
			}
			Type type = slot.GetType();
			object obj = WaterTowerItemFactory.CallInstance(type, slot, "IsRemoveLocked", null, "IsRemoveLocked(" + label + ")");
			object obj2 = WaterTowerItemFactory.CallInstance(type, slot, "IsInsertLocked", null, "IsInsertLocked(" + label + ")");
			bool flag = default(bool);
			bool flag2 = default(bool);
			int num;
			if (obj is bool)
			{
				flag = (bool)obj;
				if (obj2 is bool)
				{
					flag2 = (bool)obj2;
					num = 1;
				}
				else
				{
					num = 0;
				}
			}
			else
			{
				num = 0;
			}
			if (((uint)num & (flag ? 1u : 0u) & (flag2 ? 1u : 0u)) != 0)
			{
				return 0;
			}
			WaterTowerItemFactory.CallInstance(type, slot, "LockInv", null, "补锁 " + label + "槽");
			if (verbose)
			{
				Log.Info("   [" + tower.Label + "] 补锁 " + label + "槽（读档后 LockInv 不落存档，这里补上）");
			}
			Diag.Summary("relock-done", "补锁：" + label + "槽（读档后 LockInv 不落存档，已自动补上）");
			return 1;
		}
		catch (Exception ex)
		{
			Diag.Exception("relock-" + label, ex, "补锁 " + label + "失败（已忽略）");
			return 0;
		}
	}

	public static void RefreshStatus(TowerInstance tower)
	{
		if (tower == null || tower.Item == null || tower.TankItem == null || WaterTowerItemFactory.IsItemDestroyed(tower.Item))
		{
			return;
		}
		try
		{
			Type type = WaterTowerItemFactory.ResolveType("WaterHelper");
			if (!(type == null))
			{
				int num = ToInt(WaterTowerItemFactory.CallStatic(type, "GetCurrentCapacityML", new object[1] { tower.TankItem }, "GetCurrentCapacityML"));
				int num2 = ToInt(WaterTowerItemFactory.CallStatic(type, "GetFreeCapacity", new object[1] { tower.TankItem }, "GetFreeCapacity"));
				int purity = ToInt(WaterTowerItemFactory.CallStatic(type, "GetWaterPurity", new object[1] { tower.TankItem }, "GetWaterPurity"));
				int capacityMl = num + num2 / 1000;
				string text = WaterTowerConfig.TowerStatusText(num, capacityMl, purity);
				WaterTowerItemFactory.SetMemberValue(tower.Item.GetType(), tower.Item, "shortDescription", text);
				Log.Info("   [" + tower.Label + "] 描述已刷新：" + text);
			}
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "刷新水塔描述失败");
		}
	}

	private static object CreateBottleSlot()
	{
		Type type = WaterTowerItemFactory.ResolveType("GameSlotInventory");
		Type type2 = WaterTowerItemFactory.ResolveType("ContainerHelper");
		if (type == null || type2 == null)
		{
			Log.Error("取水槽：类型解析失败");
			return null;
		}
		int gw = 0;
		int gh = 0;
		object obj = ((!TryGetMachineContainerSlotGrid(out gw, out gh) || gw <= 0 || gh <= 0) ? WaterTowerItemFactory.NewInstance(type, new object[0], "new GameSlotInventory()（取水槽）") : WaterTowerItemFactory.NewInstance(type, new object[4] { gw, gh, false, false }, "new GameSlotInventory(" + gw + ", " + gh + ")（取水槽，对齐机器储水槽）"));
		if (obj == null)
		{
			return null;
		}
		WaterTowerItemFactory.CallInstance(type, obj, "SetBackgroundFadeSprite", new object[4] { "Items/water_container4", "xl", 0.5f, 0.5f }, "取水槽.SetBackgroundFadeSprite(Items/water_container4/xl)");
		WaterTowerItemFactory.CallStatic(type2, "AllowOnlyTaggedItem", new object[4] { obj, "LIQUID_CONTAINER_TAG", false, false }, "AllowOnlyTaggedItem(取水槽, LIQUID_CONTAINER_TAG)");
		return obj;
	}

	private static bool TryGetMachineContainerSlotGrid(out int gw, out int gh)
	{
		gw = 0;
		gh = 0;
		Type type = WaterTowerItemFactory.ResolveType("MachineMoistureFarm");
		if (type == null)
		{
			Log.Warn("   找不到 MachineMoistureFarm，取水槽用默认尺寸");
			return false;
		}
		object obj = WaterTowerItemFactory.CallStatic(type, "CreateMachineInventoryWindow", new object[3] { true, true, 0 }, "借机器窗口读储水槽尺寸");
		if (obj == null)
		{
			Log.Warn("   机器窗口构造失败，取水槽用默认尺寸");
			return false;
		}
		int num = -1;
		for (int i = 2; i <= 5; i++)
		{
			object tupleField = GetTupleField(obj, "Item" + i);
			if (tupleField == null)
			{
				continue;
			}
			Type type2 = tupleField.GetType();
			if (type2.Name != "GameSlotInventory")
			{
				Log.Info("   Item" + i + " = " + type2.Name + "（非槽位，跳过）");
				continue;
			}
			int num2 = ToInt(WaterTowerItemFactory.GetMemberValue(type2, tupleField, "defaultGridWidth"));
			int num3 = ToInt(WaterTowerItemFactory.GetMemberValue(type2, tupleField, "defaultGridHeight"));
			Log.Info("   Item" + i + " = GameSlotInventory  defaultGrid = " + num2 + " x " + num3);
			if (num2 * num3 > num)
			{
				num = num2 * num3;
				gw = num2;
				gh = num3;
			}
		}
		if (num > 0)
		{
			Log.Info("   选定机器储水槽网格 = " + gw + " x " + gh + "（取水槽将与之对齐）");
			return true;
		}
		Log.Warn("   读不到机器储水槽网格尺寸，取水槽用默认尺寸");
		return false;
	}

	private static int ToInt(object v)
	{
		return WaterTowerItemFactory.ToInt(v);
	}

	private static object CreateFillButton()
	{
		Type type = WaterTowerItemFactory.ResolveType("ButtonElement");
		if (type == null)
		{
			Log.Error("找不到 ButtonElement 类型");
			return null;
		}
		return WaterTowerItemFactory.NewInstance(type, new object[6] { "取水", -1, -1, null, 2, "interface_click" }, "new ButtonElement(\"取水\", …, 回调=null)");
	}

	private static object CreateLabel(string text)
	{
		try
		{
			Type type = WaterTowerItemFactory.ResolveType("TagElement");
			if (type == null)
			{
				return null;
			}
			object obj = WaterTowerItemFactory.NewInstance(type, new object[2] { -1, false }, "new TagElement(-1, false)");
			if (obj == null)
			{
				return null;
			}
			WaterTowerItemFactory.CallInstance(type, obj, "SetText", new object[3] { text, 10000, 12822958 }, "TagElement.SetText(" + text + ")");
			return obj;
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "构造标签失败（不影响窗口其余部分）");
			return null;
		}
	}

	private static bool IsRepeatedFillClick(string towerLabel)
	{
		lock (_lastFillClickByTower)
		{
			if (_lastFillClickByTower.TryGetValue(towerLabel, out var value) && (DateTime.Now - value).TotalSeconds < 0.35)
			{
				return true;
			}
			_lastFillClickByTower[towerLabel] = DateTime.Now;
			return false;
		}
	}

	public static void OnButtonSelected(object button)
	{
		if (button == null)
		{
			return;
		}
		TowerInstance towerInstance = null;
		try
		{
			towerInstance = TowerRegistry.FindByButton(button);
			if (towerInstance != null)
			{
				if (IsRepeatedFillClick(towerInstance.Label))
				{
					Log.Debug("   [" + towerInstance.Label + "] 取水：同一次点击的重复通知（" + 0.35 + " 秒内），已忽略");
				}
				else
				{
					FillFromTank(towerInstance, fromButton: true);
				}
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("fill-dispatch-" + ((towerInstance == null) ? "?" : towerInstance.Label), ex, "[" + ((towerInstance == null) ? "水塔?" : towerInstance.Label) + "] 取水按钮点击分派异常（已吞掉）");
		}
	}

	public static bool FillFromTank(TowerInstance tower, bool fromButton)
	{
		string text = (fromButton ? "按钮" : "夜结兜底");
		try
		{
			if (tower != null && (tower.TankItem == null || tower.BottleSlot == null))
			{
				TryDiscoverSlots(tower);
			}
			if (tower != null)
			{
				object obj = TowerRegistry.ResolveTank(tower);
				if (obj != null)
				{
					tower.TankItem = obj;
				}
			}
			if (tower == null || tower.TankItem == null || tower.BottleSlot == null)
			{
				string text2 = ((tower == null) ? "水塔?" : tower.Label);
				string text3 = ((tower == null) ? "塔实例" : ((tower.TankItem == null) ? "蓄水箱引用" : "取水槽引用"));
				Diag.Once("fill-incomplete-" + text2, "[" + text2 + "] 取水：缺失 " + text3 + "（水塔可能未完整构造；已尝试用物品图/元素树自愈但未成功）");
				return false;
			}
			object tankItem = tower.TankItem;
			object bottleSlot = tower.BottleSlot;
			string via;
			object memberRobust = WaterTowerItemFactory.GetMemberRobust(bottleSlot.GetType(), bottleSlot, "childItem", out via);
			if (memberRobust == null)
			{
				Diag.Summary("fill-empty-" + tower.Label, "[" + tower.Label + "] 取水：取水槽里没有容器（先放一个空瓶/空容器）");
				return false;
			}
			Type type = WaterTowerItemFactory.ResolveType("WaterHelper");
			if (type == null)
			{
				Diag.Once("fill-no-helper-" + tower.Label, "[" + tower.Label + "] 取水：找不到 WaterHelper");
				return false;
			}
			object v = WaterTowerItemFactory.CallStatic(type, "GetTotalVolume", new object[1] { tankItem }, "水箱当前水量");
			object v2 = WaterTowerItemFactory.CallStatic(type, "GetFreeCapacity", new object[1] { memberRobust }, "瓶子剩余容量");
			int num = WaterTowerItemFactory.ToInt(v);
			int num2 = WaterTowerItemFactory.ToInt(v2);
			if (num2 <= 0)
			{
				if (fromButton)
				{
					Diag.Summary("fill-full-" + tower.Label, "[" + tower.Label + "] 取水（按钮）：取水槽里的容器**已满**（余 0 ml），本次没有搬运 —— 换一个空容器再按，或先把容器里的水倒掉");
				}
				return false;
			}
			WaterTowerItemFactory.CallStatic(type, "TransferLiquid", new object[2] { tankItem, memberRobust }, "TransferLiquid(水箱 → 瓶子)");
			object v3 = WaterTowerItemFactory.CallStatic(type, "GetTotalVolume", new object[1] { tankItem }, "水箱剩余水量");
			int num3 = WaterTowerItemFactory.ToInt(v3);
			int num4 = num - num3;
			Log.Info("[" + tower.Label + "] 取水（" + text + "）：水箱 " + num / 1000 + " → " + num3 / 1000 + " ml（搬走 " + num4 / 1000 + " ml）；容器余 " + num2 / 1000 + " ml（" + num + " → " + num3 + " µl）");
			RefreshStatus(tower);
			return true;
		}
		catch (Exception ex)
		{
			Diag.Exception("fill-ex-" + ((tower == null) ? "?" : tower.Label), ex, "[" + ((tower == null) ? "水塔?" : tower.Label) + "] 取水失败");
			return false;
		}
	}

	private static void Attach(Type layoutType, object layout, object element, int x, int y)
	{
		Attach(layoutType, layout, element, x, y, 0, 0);
	}

	private static void Attach(Type layoutType, object layout, object element, int x, int y, int minW, int minH)
	{
		if (element == null)
		{
			Log.Warn("   跳过一个 null 元件的挂载（位置 " + x + "," + y + "）");
			return;
		}
		WaterTowerItemFactory.CallInstance(layoutType, layout, "Attach", new object[5] { element, x, y, minW, minH }, "Attach(" + element.GetType().Name + ", " + x + ", " + y + ", " + minW + ", " + minH + ")");
	}

	private static void ApplyWhitelist(object zone, object item, string label, string[] allowedIds, string[] zoneTags)
	{
		if (zone == null)
		{
			Log.Error("   [" + label + "] 区域为 null，跳过白名单");
			return;
		}
		Type type = WaterTowerItemFactory.ResolveType("ContainerHelper");
		if (type == null)
		{
			Log.Error("   [" + label + "] 找不到 ContainerHelper");
			return;
		}
		WaterTowerItemFactory.CallStatic(type, "InitContainerItem", new object[4] { zone, item, null, null }, "InitContainerItem(" + label + ", null, null)（与 machine_bay 一致）");
		Type type2 = WaterTowerItemFactory.ResolveType("ContainerHelper");
		WaterTowerItemFactory.CallStatic(type2, "AllowOnlyTaggedItemsOr", new object[3] { zone, zoneTags, true }, "AllowOnlyTaggedItemsOr(" + label + ", [" + string.Join(",", zoneTags) + "], ownedOnly)");
		Log.Info("   [" + label + "] 容器初始化完成，过滤器 = 原版 AllowOnlyTaggedItemsOr([" + string.Join(", ", zoneTags) + "], ownedOnly)（本区期望 ID = [" + string.Join(", ", allowedIds) + "]，由日晚结算按 ID 判定）");
	}

	private static void ApplyFootprint(Type itemType, object item)
	{
		int num = 3;
		int num2 = 4;
		Log.Info("   把占格改写为目标 " + num + "x" + num2);
		Type type = WaterTowerItemFactory.ResolveType("GridShapeBuilder");
		if (type == null)
		{
			Log.Error("   找不到 GridShapeBuilder，占格保持贴图推导值");
			return;
		}
		object obj = WaterTowerItemFactory.NewInstance(type, new object[2] { num, num2 }, "new GridShapeBuilder(" + num + ", " + num2 + ")");
		if (obj == null)
		{
			return;
		}
		Type type2 = obj.GetType();
		object obj2 = WaterTowerItemFactory.CallInstance(type2, obj, "SetDataFill", new object[3]
		{
			num,
			num2,
			(byte)1
		}, "SetDataFill(" + num + ", " + num2 + ", 1)");
		object obj3 = obj2 ?? obj;
		object obj4 = WaterTowerItemFactory.CallInstance(obj3.GetType(), obj3, "Build", null, "GridShapeBuilder.Build()");
		if (obj4 == null)
		{
			Log.Error("   形状构造失败，占格保持贴图推导值");
			return;
		}
		object obj5 = WaterTowerItemFactory.CallInstance(itemType, item, "SetShape", new object[1] { obj4 }, "GameItem.SetShape(" + num + "x" + num2 + ")");
		if (obj5 == null)
		{
			bool flag = WaterTowerItemFactory.SetMemberValue(itemType, item, "shape", obj4);
			Log.Info("   SetShape 无返回，改用属性写入：" + (flag ? "成功" : "失败"));
		}
		WaterTowerItemFactory.CallInstance(itemType, item, "ValidateShapeState", null, "ValidateShapeState()");
		ProbeShape(itemType, item, "改写后");
	}

	private static void ProbeShape(Type itemType, object item, string stage)
	{
		object obj = WaterTowerItemFactory.GetMemberValue(itemType, item, "shape") ?? WaterTowerItemFactory.GetMemberValue(itemType, item, "inventoryShape");
		if (obj == null)
		{
			Log.Warn("   读不到 shape");
			return;
		}
		Type type = obj.GetType();
		object memberValue = WaterTowerItemFactory.GetMemberValue(type, obj, "width");
		object memberValue2 = WaterTowerItemFactory.GetMemberValue(type, obj, "height");
		Log.Info("   [" + stage + "] 占格形状 = " + Fmt(memberValue) + " x " + Fmt(memberValue2) + "（目标占格 " + 3 + " x " + 4 + "）");
	}

	private static string Fmt(object v)
	{
		return WaterTowerItemFactory.Fmt(v);
	}

	private static object GetTupleField(object tuple, string fieldName)
	{
		if (tuple == null)
		{
			return null;
		}
		try
		{
			Type type = tuple.GetType();
			FieldInfo field = type.GetField(fieldName);
			if (field != null)
			{
				return field.GetValue(tuple);
			}
			PropertyInfo property = type.GetProperty(fieldName);
			if (property != null)
			{
				return property.GetValue(tuple);
			}
		}
		catch (Exception ex)
		{
			Log.Exception(ex, "读取元组字段 " + fieldName + " 失败");
		}
		return null;
	}
}
