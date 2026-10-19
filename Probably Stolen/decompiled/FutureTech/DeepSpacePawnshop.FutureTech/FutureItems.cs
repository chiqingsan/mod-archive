using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace DeepSpacePawnshop.FutureTech;

internal static class FutureItems
{
	[HarmonyPatch(typeof(WaterHelper), "GetFreeCapacity")]
	private static class VoidJugFreeCapacityPatch
	{
		private static void Postfix(GameItem __0, ref int __result)
		{
			if (((__0 != null) ? __0.identifier : null) != "dspf_future_void_jug")
			{
				return;
			}
			int totalVolume = WaterHelper.GetTotalVolume(__0);
			if (totalVolume >= 18000000)
			{
				__result = 0;
				return;
			}
			if (TagInt(__0, "LIQUID_CONTAINER_CAPACITY") != 18000000)
			{
				SetTagInt(__0, "LIQUID_CONTAINER_CAPACITY", 18000000);
			}
			__result = Math.Max(0, 18000000 - totalVolume);
		}
	}

	[HarmonyPatch(typeof(GameItem), "MayTarget")]
	private static class IngotMayTargetPatch
	{
		private static bool Prefix(GameItem __instance, GameItem targetItem, ref bool __result)
		{
			if (__instance.identifier == "dspf_future_purifier")
			{
				__result = CanFuturePurify(__instance, targetItem);
				return false;
			}
			if (!IsAgingUpgrade(__instance, targetItem))
			{
				return true;
			}
			__result = AgingSize(targetItem) < 20;
			return false;
		}
	}

	[HarmonyPatch(typeof(GameItem), "CanTarget")]
	private static class IngotCanTargetPatch
	{
		private static bool Prefix(GameItem __instance, GameItem targetItem, ref bool __result)
		{
			if (__instance.identifier == "dspf_future_purifier")
			{
				__result = CanFuturePurify(__instance, targetItem);
				return false;
			}
			if (!IsAgingUpgrade(__instance, targetItem))
			{
				return true;
			}
			__result = AgingSize(targetItem) < 20 && __instance.unitCount > 0;
			return false;
		}
	}

	[HarmonyPatch(typeof(GameItem), "Target")]
	private static class IngotTargetPatch
	{
		private static bool Prefix(GameItem __instance, GameItem targetItem)
		{
			if (__instance.identifier == "dspf_future_purifier")
			{
				PurifyWater(__instance, targetItem);
				return false;
			}
			if (!IsAgingUpgrade(__instance, targetItem) || !IsDragRelease())
			{
				return true;
			}
			return !TryUpgradeAgingCabinet(__instance, targetItem);
		}
	}

	[HarmonyPatch(typeof(GameItem), "MayHaveValidInventorySlot")]
	private static class IngotContainerSlotPatch
	{
		private static bool Prefix(GameItem __instance, GameItem item, ref bool __result)
		{
			if (!IsAgingUpgrade(item, __instance) || !IsDragRelease())
			{
				return true;
			}
			if (!TryUpgradeAgingCabinet(item, __instance))
			{
				return true;
			}
			__result = false;
			return false;
		}
	}

	[HarmonyPatch(typeof(GameItem), "get_name")]
	private static class ItemNamePatch
	{
		private static void Postfix(GameItem __instance, ref string __result)
		{
			if (All.Contains(__instance.identifier))
			{
				__result = FutureText.Item(__instance.identifier).Name;
			}
		}
	}

	[HarmonyPatch(typeof(GameItem), "get_shortDescription")]
	private static class ItemShortDescriptionPatch
	{
		private static void Postfix(GameItem __instance, ref string __result)
		{
			if (All.Contains(__instance.identifier))
			{
				__result = FutureText.Item(__instance.identifier).Description;
			}
		}
	}

	[HarmonyPatch(typeof(GameItem), "get_longDescription")]
	private static class ItemLongDescriptionPatch
	{
		private static void Postfix(GameItem __instance, ref string __result)
		{
			if (All.Contains(__instance.identifier))
			{
				__result = FutureText.Item(__instance.identifier).Description;
			}
		}
	}

	[HarmonyPatch(typeof(GameItem), "GetDisplayName")]
	private static class ItemDisplayNamePatch
	{
		private static void Postfix(GameItem __instance, ref string __result)
		{
			if (All.Contains(__instance.identifier))
			{
				__result = FutureText.Item(__instance.identifier).Name;
			}
		}
	}

	[HarmonyPatch(typeof(AmenitiesItemDirectory), "CanPurifierTarget")]
	private static class FuturePurifierCanTargetPatch
	{
		private static void Postfix(GameItem purifier, GameItem target, ref bool __result)
		{
			if (!(((purifier != null) ? purifier.identifier : null) != "dspf_future_purifier"))
			{
				__result = CanFuturePurify(purifier, target);
			}
		}
	}

	[HarmonyPatch(typeof(AmenitiesItemDirectory), "OnPortableWaterPurifierUsed")]
	private static class FuturePurifierPatch
	{
		private static bool Prefix(GameItem purifier, GameItem container)
		{
			if (((purifier != null) ? purifier.identifier : null) != "dspf_future_purifier")
			{
				return true;
			}
			PurifyWater(purifier, container);
			return false;
		}
	}

	[HarmonyPatch(typeof(MachinePurifier), "PurifyContainer")]
	private static class LegacyFuturePurifierPatch
	{
		private static bool Prefix(GameItem machine, GameItem waterContainer, ref int __result)
		{
			if (((machine != null) ? machine.identifier : null) != "dspf_future_purifier")
			{
				return true;
			}
			__result = PurifyWater(machine, waterContainer);
			return false;
		}
	}

	internal const string Purifier = "dspf_future_purifier";

	internal const string Charger = "dspf_future_charger";

	internal const string Module = "dspf_future_module";

	internal const string AgingCabinet = "dspf_future_aging_cabinet";

	internal const string NuclearBattery = "dspf_future_nuclear_battery";

	internal const string Pocket = "dspf_future_pocket";

	internal const string VoidJug = "dspf_future_void_jug";

	internal const int PurifierMaxDurability = 1000;

	internal const int BatteryMaxEnergy = 1000;

	internal const int VoidJugCapacityMl = 18000;

	internal const int VoidJugCapacityUl = 18000000;

	internal const string PurifierBatteryInventory = "dspf_future_purifier_power";

	internal const string CabinetBatteryInventory = "dspf_future_aging_cabinet_power";

	internal const string CabinetStorageInventory = "dspf_future_aging_cabinet_inventory";

	internal const int PocketColumns = 30;

	internal const int PocketRows = 10;

	internal static readonly string[] All = new string[7] { "dspf_future_purifier", "dspf_future_charger", "dspf_future_module", "dspf_future_aging_cabinet", "dspf_future_nuclear_battery", "dspf_future_pocket", "dspf_future_void_jug" };

	private static Action<string>? _log;

	private static Action<ModItemDirectory>? _directoryHook;

	private const string AgingLevelTag = "DSPF_FUTURE_AGING_LEVEL_INT";

	private const string MetalIngot = "metal_ingot";

	private static int _lastUpgradeFrame = -1;

	private static int _lastUpgradeSource;

	private static int _lastUpgradeTarget;

	internal static long PriceOf(string id)
	{
		return id switch
		{
			"dspf_future_purifier" => 1200, 
			"dspf_future_charger" => 1000, 
			"dspf_future_module" => 5000, 
			"dspf_future_aging_cabinet" => 1800, 
			"dspf_future_nuclear_battery" => 800, 
			"dspf_future_pocket" => 3500, 
			"dspf_future_void_jug" => 150, 
			_ => throw new ArgumentOutOfRangeException("id", id, "Unknown Future Tech item."), 
		};
	}

	internal static void Initialize(Action<string> log)
	{
		_log = log;
		_directoryHook = DelegateSupport.ConvertDelegate<Action<ModItemDirectory>>((Delegate)new Action<ModItemDirectory>(Register));
		ModHook.OnModItemDirectoryInit += _directoryHook;
	}

	private static void Register(ModItemDirectory directory)
	{
		try
		{
			Add(directory, "dspf_future_purifier", CreatePurifier);
			Add(directory, "dspf_future_charger", CreateCharger);
			Add(directory, "dspf_future_module", CreateModule);
			Add(directory, "dspf_future_aging_cabinet", CreateAgingCabinet);
			Add(directory, "dspf_future_nuclear_battery", CreateBattery);
			Add(directory, "dspf_future_pocket", CreatePocket);
			Add(directory, "dspf_future_void_jug", CreateVoidJug);
			_log?.Invoke("Future Tech item directory registered (7 items).");
		}
		catch (Exception value)
		{
			_log?.Invoke($"Future Tech item registration failed: {value}");
		}
	}

	private static void Add(ModItemDirectory directory, string id, Func<GameItem> create)
	{
		if (!((Directory<GameItem>)(object)directory).Has(id))
		{
			Func<GameItem> val = DelegateSupport.ConvertDelegate<Func<GameItem>>((Delegate)create);
			if (!((Directory<GameItem>)(object)directory).Add(id, val))
			{
				throw new InvalidOperationException("Directory rejected " + id + ".");
			}
		}
	}

	private static GameItem CreateCharger()
	{
		ValueTuple<PixelWindow, GameInventory> val = DirectoryUtils.CreateInventoryWindow(4, 4, true);
		GameItem val2 = ItemDirectory.CreateEmptyItem((string)null) ?? throw new InvalidOperationException("Could not create the Future Tech charger.");
		val2.SetContentWindow(val.Item1);
		val.Item2.identifier = "dspf_future_charger_inventory";
		ContainerHelper.InitContainerItem(val.Item2, val2, (List<string>)null, (List<string>)null);
		ContainerHelper.AllowOnlyTaggedItem(val.Item2, "ENERGY_CELL", false, false);
		val2.SetGameItemType("STORAGE");
		val2.SetShape(RectangleShape(2, 2));
		return Configure(val2, "dspf_future_charger");
	}

	private static GameItem CreatePurifier()
	{
		GameItem val = Configure(DirectoryMaster.Item("portable_water_purifier", true), "dspf_future_purifier");
		ValueTuple<PixelWindow, GameInventory> val2 = DirectoryUtils.CreateInventoryWindow(2, 2, true);
		val2.Item2.identifier = "dspf_future_purifier_power";
		val.SetContentWindow(val2.Item1);
		ContainerHelper.InitContainerItem(val2.Item2, val, (List<string>)null, (List<string>)null);
		ContainerHelper.AllowOnlyTaggedItem(val2.Item2, "ENERGY_CELL", false, false);
		DurabilityHelper.InitItemWithDurabilityPricingMode2(val, 1000, 25);
		val.SetShape(RectangleShape(2, 1));
		val.SetValue(PriceOf("dspf_future_purifier"));
		val.unitBaseValue = PriceOf("dspf_future_purifier");
		return val;
	}

	private static GameItem CreateModule()
	{
		GameItem obj = Configure(DirectoryMaster.Item("system_module_performance", true), "dspf_future_module");
		obj.SetShape(RectangleShape(2, 1));
		ModuleHelper.InitModuleItem(obj, "MODULE_TYPE_UNIVERSAL", 200, 200, 200, false, "", 1);
		ApplyModuleBonuses(obj);
		return obj;
	}

	private static GameItem CreateAgingCabinet()
	{
		ValueTuple<PixelWindow, GameInventory, GameInventory> val = DirectoryUtils.CreateDoubleInventoryWindowSlotInput(10, 10, true, true, 0);
		GameItem val2 = ItemDirectory.CreateEmptyItem((string)null) ?? throw new InvalidOperationException("Could not create the Future Tech aging cabinet.");
		val2.SetContentWindow(val.Item1);
		val.Item2.identifier = "dspf_future_aging_cabinet_power";
		val.Item3.identifier = "dspf_future_aging_cabinet_inventory";
		GameSlotInventory obj = ((Il2CppObjectBase)val.Item2).TryCast<GameSlotInventory>();
		if (obj != null)
		{
			obj.ResizePixels(64, 64);
		}
		ContainerHelper.InitContainerItem(val.Item2, val2, (List<string>)null, (List<string>)null);
		ContainerHelper.AllowOnlyTaggedItem(val.Item2, "ENERGY_CELL", false, false);
		ContainerHelper.InitContainerItem(val.Item3, val2, (List<string>)null, (List<string>)null);
		val2.SetGameItemType("STORAGE");
		val2.SetShape(RectangleShape(2, 1));
		val2 = Configure(val2, "dspf_future_aging_cabinet");
		SetTagInt(val2, "DSPF_FUTURE_AGING_LEVEL_INT", 0);
		return val2;
	}

	internal static int AgingSize(GameItem cabinet)
	{
		return 10 + Math.Clamp(TagInt(cabinet, "DSPF_FUTURE_AGING_LEVEL_INT").GetValueOrDefault(), 0, 10);
	}

	private static bool IsAgingUpgrade(GameItem source, GameItem target)
	{
		if (((source != null) ? source.identifier : null) == "metal_ingot")
		{
			return ((target != null) ? target.identifier : null) == "dspf_future_aging_cabinet";
		}
		return false;
	}

	internal static void ApplyModuleBonuses(GameItem item)
	{
		string[] array = new string[3] { "PERFORMANCE", "EFFICIENCY", "QUALITY" };
		foreach (string text in array)
		{
			SetTagInt(item, "BONUS_PERCENTAGE_" + text + "_INT", 200);
			SetTagInt(item, "TEMP_PERCENTAGE_" + text + "_INT", 200);
		}
	}

	private static int PurificationCost(GameItem? container)
	{
		if (container != null)
		{
			return (int)(((long)Math.Max(0, WaterHelper.GetTotalVolume(container)) + 99999L) / 100000);
		}
		return 0;
	}

	private static bool CanFuturePurify(GameItem? purifier, GameItem? container)
	{
		if (purifier != null && container != null && WaterHelper.CanUseWaterContainer(container) && PurificationCost(container) > 0)
		{
			return DurabilityHelper.GetRealDurability(purifier) >= PurificationCost(container);
		}
		return false;
	}

	private static int PurifyWater(GameItem? purifier, GameItem? container)
	{
		if (!CanFuturePurify(purifier, container))
		{
			return 0;
		}
		int totalVolume = WaterHelper.GetTotalVolume(container);
		int num = PurificationCost(container);
		if (FutureEdition.HighQualityPurifier)
		{
			WaterHelper.EmptyContainer(container);
			WaterHelper.AddHighQualityWater(container, totalVolume);
		}
		else
		{
			WaterHelper.PurifyToBaseWater(container);
		}
		WaterHelper.UpdateContainerGoodType(container);
		DurabilityHelper.ChangeDurability(purifier, -num);
		_log?.Invoke($"Future Tech purified {totalVolume} liquid units (purity {WaterHelper.GetWaterPurity(container)}).");
		return totalVolume;
	}

	private static bool IsDragRelease()
	{
		ItemMouseDragHandler current = ItemMouseDragHandler.current;
		if (current != null && current.IsDraggingItem)
		{
			return !Input.GetMouseButton(0);
		}
		return false;
	}

	private static bool TryUpgradeAgingCabinet(GameItem ingot, GameItem cabinet)
	{
		if (!IsAgingUpgrade(ingot, cabinet))
		{
			return false;
		}
		if (_lastUpgradeFrame == Time.frameCount && _lastUpgradeSource == ingot.uniqueId && _lastUpgradeTarget == cabinet.uniqueId)
		{
			return true;
		}
		int num = AgingSize(cabinet);
		if (num >= 20 || ingot.unitCount <= 0)
		{
			return false;
		}
		try
		{
			ResizeStorage(cabinet, num + 1, num + 1);
			SetTagInt(cabinet, "DSPF_FUTURE_AGING_LEVEL_INT", num - 9);
			_lastUpgradeFrame = Time.frameCount;
			_lastUpgradeSource = ingot.uniqueId;
			_lastUpgradeTarget = cabinet.uniqueId;
			if (ingot.unitCount == 1)
			{
				ingot.Destroy();
			}
			else
			{
				ingot.SetUnitCount(ingot.unitCount - 1);
			}
			_log?.Invoke($"Future Tech aging cabinet upgraded to {num + 1}x{num + 1}.");
			return true;
		}
		catch (Exception value)
		{
			_log?.Invoke($"Future Tech aging cabinet upgrade failed: {value}");
			return false;
		}
	}

	private static GameItem CreatePocket()
	{
		GameItem obj = Configure(DirectoryMaster.Item("backpack_large", true), "dspf_future_pocket");
		obj.SetShape(RectangleShape(2, 1));
		ResizeStorage(obj, 30, 10);
		return obj;
	}

	private static GameItem CreateVoidJug()
	{
		GameItem val = DirectoryMaster.Item("water_jug", true) ?? throw new InvalidOperationException("Water jug icon prototype is unavailable.");
		GameItem obj = ItemDirectory.CreateEmptyItem("dspf_future_void_jug") ?? throw new InvalidOperationException("Could not create the void-tech water jug.");
		obj.SetSprite(val.spriteAtlasPath, val.spritePath);
		WaterHelper.InitLiquidContainerItem(obj, 18000, (int)PriceOf("dspf_future_void_jug"), false, false, false, false, 3);
		obj.SetShape(RectangleShape(1, 2));
		return Configure(obj, "dspf_future_void_jug");
	}

	internal static void ResizeStorage(GameItem item, int width, int height)
	{
		GameGridInventory val = FindStorageGrid(item);
		if (val == null)
		{
			throw new InvalidOperationException("Storage grid was not found on " + item.identifier + ".");
		}
		if (val.inventoryShape.width != width || val.inventoryShape.height != height)
		{
			int widthPixels = val.widthPixels;
			int heightPixels = val.heightPixels;
			PixelWindow contentWindow = item.contentWindow;
			int num = ((contentWindow != null) ? contentWindow.widthPixels : 0);
			int num2 = ((contentWindow != null) ? contentWindow.heightPixels : 0);
			val.SetShape(width, height);
			((GameInventory)val).Validate();
			if (contentWindow != null)
			{
				contentWindow.ResizePixels(Math.Max(val.widthPixels, num + val.widthPixels - widthPixels), Math.Max(val.heightPixels, num2 + val.heightPixels - heightPixels));
				contentWindow.Validate();
			}
		}
	}

	private static GameItem CreateBattery()
	{
		GameItem obj = Configure(DirectoryMaster.Item("energy_credit_ext", true), "dspf_future_nuclear_battery");
		obj.SetShape(RectangleShape(2, 2));
		PowerHelper.InitPowerSourceItem(obj, 1000, 100);
		PowerHelper.SetPowerSourceAt(obj, 1000);
		obj.SetValue(PriceOf("dspf_future_nuclear_battery"));
		obj.unitBaseValue = PriceOf("dspf_future_nuclear_battery");
		return obj;
	}

	private static GameItem Configure(GameItem? item, string id)
	{
		if (item == null)
		{
			throw new InvalidOperationException("Prototype for " + id + " is unavailable.");
		}
		string spriteAtlasPath = item.spriteAtlasPath;
		string spritePath = item.spritePath;
		(string, string) tuple = FutureText.Item(id);
		item.identifier = id;
		item.identifierName = id;
		item.SetName(tuple.Item1);
		item.shortDescription = tuple.Item2;
		item.longDescription = tuple.Item2;
		item.SetValue(PriceOf(id));
		item.unitBaseValue = PriceOf(id);
		if (!string.IsNullOrWhiteSpace(spriteAtlasPath) && !string.IsNullOrWhiteSpace(spritePath))
		{
			item.SetSprite(spriteAtlasPath, spritePath);
		}
		FutureIcons.ApplyToItem(item, id);
		return item;
	}

	internal static void UpgradeLegacyPrice(GameItem item)
	{
		long num = item.identifier switch
		{
			"dspf_future_purifier" => 2000L, 
			"dspf_future_charger" => 800L, 
			"dspf_future_module" => 3500L, 
			"dspf_future_aging_cabinet" => 2000L, 
			"dspf_future_nuclear_battery" => 800L, 
			"dspf_future_pocket" => 3500L, 
			"dspf_future_void_jug" => 8000L, 
			_ => 0L, 
		};
		if (item.unitBaseValue == 500 || item.unitBaseValue == num)
		{
			long num2 = PriceOf(item.identifier);
			if (item.identifier == "dspf_future_void_jug")
			{
				SetTagInt(item, "LIQUID_CONTAINER_BASE_PRICE_TAG", (int)num2);
			}
			item.SetValue(Math.Max(0L, item.unitValue + num2 - item.unitBaseValue));
			item.unitBaseValue = num2;
		}
	}

	internal static GridShape RectangleShape(int width, int height)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return new GridShapeBuilder().SetDataFill(width, height, (byte)1).Build();
	}

	internal static GameGridInventory? FindStorageGrid(GameItem item)
	{
		GameGridInventory val = null;
		try
		{
			val = MachineHelper.GetModuleInv(item);
		}
		catch
		{
		}
		GameGridInventory result = null;
		int num = -1;
		Stack<Il2CppObjectBase> stack = new Stack<Il2CppObjectBase>();
		HashSet<IntPtr> hashSet = new HashSet<IntPtr>();
		stack.Push((Il2CppObjectBase)(object)item);
		while (stack.Count > 0)
		{
			Il2CppObjectBase val2 = stack.Pop();
			if (val2 == null || !hashSet.Add(val2.Pointer))
			{
				continue;
			}
			GameGridInventory val3 = val2.TryCast<GameGridInventory>();
			if (val3 != null && (val == null || ((Il2CppObjectBase)val3).Pointer != ((Il2CppObjectBase)val).Pointer))
			{
				int num2 = val3.inventoryShape.width * val3.inventoryShape.height;
				if (num2 > num)
				{
					result = val3;
					num = num2;
				}
			}
			GameItem val4 = val2.TryCast<GameItem>();
			Enumerator<GraphNodeStorage> enumerator;
			if (val4 != null)
			{
				if (val4.child != null)
				{
					stack.Push((Il2CppObjectBase)(object)val4.child);
				}
				if (val4.contentWindow != null)
				{
					stack.Push((Il2CppObjectBase)(object)val4.contentWindow);
				}
				if (val4.children != null)
				{
					enumerator = val4.children.GetEnumerator();
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
			GraphNodeWindow val5 = val2.TryCast<GraphNodeWindow>();
			if (((val5 != null) ? val5.child : null) != null)
			{
				stack.Push((Il2CppObjectBase)(object)val5.child);
			}
			PixelWindow val6 = val2.TryCast<PixelWindow>();
			if (((val6 != null) ? val6.child : null) != null)
			{
				stack.Push((Il2CppObjectBase)(object)val6.child);
			}
			GraphNodeStorage val7 = val2.TryCast<GraphNodeStorage>();
			if (((val7 != null) ? val7.children : null) == null)
			{
				continue;
			}
			enumerator = val7.children.GetEnumerator();
			while (enumerator.MoveNext())
			{
				GraphNodeStorage current2 = enumerator.Current;
				if (current2 != null)
				{
					stack.Push((Il2CppObjectBase)(object)current2);
				}
			}
		}
		return result;
	}

	internal static void SetTagInt(GameItem item, string key, int value)
	{
		TagState tag = item.state.GetTag(key);
		if (tag == null)
		{
			item.state.InitTagString(key);
			tag = item.state.GetTag(key);
		}
		if (tag == null)
		{
			throw new InvalidOperationException($"Could not initialize {key} on {item.identifier}.");
		}
		tag.SetInt(value).Enable();
	}

	internal static int? TagInt(GameItem item, string key)
	{
		TagState tagReadonly = item.GetTagReadonly(key);
		if (tagReadonly == null)
		{
			return null;
		}
		return tagReadonly.GetInt();
	}
}
