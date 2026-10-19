using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;

namespace XIAOWOTradePerks;

internal static class FridgeExpansion
{
	internal const string LevelTag = "XIAOWO_FRIDGE_EXPANSION_LEVEL";

	internal const string NativeStorageTag = "FRIDGE_STORAGE_UPGRADE_COUNT_TAG";

	internal const string NativePowerTag = "FRIDGE_POWER_UPGRADE_COUNT_TAG";

	internal const int MaximumLevel = 4;

	[ThreadStatic]
	private static bool _applying;

	internal static bool IsFridge(GameItem item)
	{
		if (item != null && string.Equals(item.identifier?.Replace("\0", "").Trim(), "fridge", StringComparison.Ordinal))
		{
			return item.IsTag("FRIDGE_TAG");
		}
		return false;
	}

	internal static int Level(GameItem item)
	{
		if (!IsFridge(item))
		{
			return 0;
		}
		TagSystem state = item.state;
		int? obj;
		if (state == null)
		{
			obj = null;
		}
		else
		{
			TagState tag = state.GetTag("XIAOWO_FRIDGE_EXPANSION_LEVEL");
			obj = ((tag != null) ? new int?(tag.GetInt()) : null);
		}
		int? num = obj;
		return Math.Clamp(num.GetValueOrDefault(), 0, 4);
	}

	internal static bool NativeStorageComplete(GameItem item)
	{
		if (IsFridge(item))
		{
			TagState tagReadonly = item.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG");
			return ((tagReadonly != null) ? tagReadonly.GetInt() : 0) >= 7;
		}
		return false;
	}

	internal static int Remaining(GameItem item)
	{
		if (!NativeStorageComplete(item))
		{
			return 4;
		}
		return 4 - Level(item);
	}

	internal static string Status(GameItem item)
	{
		if (!PerkRegistry.IsActive("xiaowo_trade_precision_bay_expansion"))
		{
			return string.Empty;
		}
		if (!PrecisionBayMode.IsUpgradeEnabled())
		{
			return ModLocalization.Select("精工扩舱已关闭", "Precision Bay Expansion disabled");
		}
		if (!NativeStorageComplete(item))
		{
			return ModLocalization.Select("完成冰箱原生存储升级后开放精工扩舱", "Complete native fridge storage upgrades first");
		}
		int num = Level(item);
		return ModLocalization.Select("精工扩舱 ", "Precision expansion ") + num + "/4 | " + ModLocalization.Select("冷藏 ", "Chill ") + (5 + num) + "x" + (9 + num) + " | " + ModLocalization.Select("冷冻 ", "Freeze ") + (3 + num) + "x" + (9 + num);
	}

	internal static bool CanUpgrade(GameItem item)
	{
		if (PerkRegistry.IsActive("xiaowo_trade_precision_bay_expansion") && PrecisionBayMode.IsUpgradeEnabled() && NativeStorageComplete(item) && Level(item) < 4)
		{
			GameGridInventory chillZone = MachineFridge.GetChillZone(item);
			if (((chillZone != null) ? chillZone.inventoryShape : null) != null)
			{
				GameGridInventory freezeZone = MachineFridge.GetFreezeZone(item);
				return ((freezeZone != null) ? freezeZone.inventoryShape : null) != null;
			}
		}
		return false;
	}

	internal static bool TryUpgrade(GameItem fridge, GameItem ingot, out string details)
	{
		details = string.Empty;
		if (!CanUpgrade(fridge))
		{
			details = "native fridge storage not ready or capped";
			return false;
		}
		GameGridInventory chillZone = MachineFridge.GetChillZone(fridge);
		GameGridInventory freezeZone = MachineFridge.GetFreezeZone(fridge);
		int num = Level(fridge);
		(int, int) tuple = (chillZone.inventoryShape.width, chillZone.inventoryShape.height);
		(int, int) tuple2 = (freezeZone.inventoryShape.width, freezeZone.inventoryShape.height);
		List<(IntPtr, int, int, int, int)> expected = Snapshot(chillZone);
		List<(IntPtr, int, int, int, int)> expected2 = Snapshot(freezeZone);
		TagState tagReadonly = fridge.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG");
		int num2 = ((tagReadonly != null) ? tagReadonly.GetInt() : 0);
		TagState tagReadonly2 = fridge.GetTagReadonly("FRIDGE_POWER_UPGRADE_COUNT_TAG");
		int num3 = ((tagReadonly2 != null) ? tagReadonly2.GetInt() : 0);
		int num4 = num + 1;
		bool result;
		try
		{
			_applying = true;
			SetLevel(fridge, num4);
			Apply(fridge);
			if (!Matches(chillZone, expected) || !Matches(freezeZone, expected2) || chillZone.inventoryShape.width != 5 + num4 || chillZone.inventoryShape.height != 9 + num4 || freezeZone.inventoryShape.width != 3 + num4 || freezeZone.inventoryShape.height != 9 + num4)
			{
				goto IL_0158;
			}
			TagState tagReadonly3 = fridge.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG");
			if (((tagReadonly3 != null) ? tagReadonly3.GetInt() : 0) != num2)
			{
				goto IL_0158;
			}
			TagState tagReadonly4 = fridge.GetTagReadonly("FRIDGE_POWER_UPGRADE_COUNT_TAG");
			if (((tagReadonly4 != null) ? tagReadonly4.GetInt() : 0) != num3)
			{
				goto IL_0158;
			}
			if (ingot != null)
			{
				ingot.Destroy();
			}
			details = "fridge expansion=" + num4 + "/4 chill=" + (5 + num4) + "x" + (9 + num4) + " freeze=" + (3 + num4) + "x" + (9 + num4);
			result = true;
			goto end_IL_00ae;
			IL_0158:
			string[] obj = new string[16]
			{
				"fridge contents, dimensions or native levels changed: chill=",
				chillZone.inventoryShape.width.ToString(),
				"x",
				chillZone.inventoryShape.height.ToString(),
				" freeze=",
				freezeZone.inventoryShape.width.ToString(),
				"x",
				freezeZone.inventoryShape.height.ToString(),
				" items=",
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			result = Matches(chillZone, expected);
			obj[9] = result.ToString();
			obj[10] = "/";
			result = Matches(freezeZone, expected2);
			obj[11] = result.ToString();
			obj[12] = " native=";
			TagState tagReadonly5 = fridge.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG");
			obj[13] = ((tagReadonly5 != null) ? tagReadonly5.GetInt() : 0).ToString();
			obj[14] = "/";
			TagState tagReadonly6 = fridge.GetTagReadonly("FRIDGE_POWER_UPGRADE_COUNT_TAG");
			obj[15] = ((tagReadonly6 != null) ? tagReadonly6.GetInt() : 0).ToString();
			throw new InvalidOperationException(string.Concat(obj));
			end_IL_00ae:;
		}
		catch (Exception ex2)
		{
			try
			{
				SetLevel(fridge, num);
				chillZone.SetShape(tuple.Item1, tuple.Item2);
				freezeZone.SetShape(tuple2.Item1, tuple2.Item2);
				PixelWindow contentWindow = fridge.contentWindow;
				if (contentWindow != null)
				{
					contentWindow.Validate();
				}
			}
			catch (Exception ex)
			{
				Instance log = Core.Log;
				if (log != null)
				{
					log.Error("Fridge expansion rollback failed: " + ex);
				}
			}
			details = ex2.GetType().Name + ": " + ex2.Message;
			Instance log2 = Core.Log;
			if (log2 != null)
			{
				log2.Error("Fridge expansion rejected: " + details);
			}
			result = false;
		}
		finally
		{
			_applying = false;
		}
		return result;
	}

	internal static void Apply(GameItem fridge)
	{
		if (!NativeStorageComplete(fridge))
		{
			return;
		}
		int num = Level(fridge);
		if (num == 0)
		{
			return;
		}
		GameGridInventory chillZone = MachineFridge.GetChillZone(fridge);
		GameGridInventory freezeZone = MachineFridge.GetFreezeZone(fridge);
		if (((chillZone != null) ? chillZone.inventoryShape : null) == null || ((freezeZone != null) ? freezeZone.inventoryShape : null) == null)
		{
			return;
		}
		bool flag = false;
		if (chillZone.inventoryShape.width != 5 + num || chillZone.inventoryShape.height != 9 + num)
		{
			chillZone.SetShape(5 + num, 9 + num);
			flag = true;
		}
		if (freezeZone.inventoryShape.width != 3 + num || freezeZone.inventoryShape.height != 9 + num)
		{
			freezeZone.SetShape(3 + num, 9 + num);
			flag = true;
		}
		if (flag)
		{
			chillZone.ForceRecomputeDynamicBackground();
			freezeZone.ForceRecomputeDynamicBackground();
			PixelWindow contentWindow = fridge.contentWindow;
			if (contentWindow != null)
			{
				contentWindow.Validate();
			}
			if (chillZone.inventoryShape.width != 5 + num || chillZone.inventoryShape.height != 9 + num)
			{
				chillZone.SetShape(5 + num, 9 + num);
			}
			if (freezeZone.inventoryShape.width != 3 + num || freezeZone.inventoryShape.height != 9 + num)
			{
				freezeZone.SetShape(3 + num, 9 + num);
			}
			MachineFridge.UpdateFridgePowerUsage(fridge);
		}
	}

	private static void SetLevel(GameItem fridge, int level)
	{
		if (!fridge.IsTag("XIAOWO_FRIDGE_EXPANSION_LEVEL"))
		{
			fridge.state.EnableTag("XIAOWO_FRIDGE_EXPANSION_LEVEL");
		}
		fridge.state.GetTag("XIAOWO_FRIDGE_EXPANSION_LEVEL").SetInt(level);
	}

	private static List<(IntPtr Pointer, int Id, int Count, int X, int Y)> Snapshot(GameGridInventory zone)
	{
		List<(IntPtr, int, int, int, int)> list = new List<(IntPtr, int, int, int, int)>();
		for (int i = 0; i < zone.items.Count; i++)
		{
			GameItem val = zone.items[i];
			IntPtr pointer = ((Il2CppObjectBase)val).Pointer;
			int uniqueId = val.uniqueId;
			int unitCount = val.unitCount;
			GridShape modifiedShape = val.modifiedShape;
			int item = ((modifiedShape != null) ? modifiedShape.minX : (-1));
			GridShape modifiedShape2 = val.modifiedShape;
			list.Add((pointer, uniqueId, unitCount, item, (modifiedShape2 != null) ? modifiedShape2.minY : (-1)));
		}
		return list;
	}

	private static bool Matches(GameGridInventory zone, List<(IntPtr Pointer, int Id, int Count, int X, int Y)> expected)
	{
		if (zone.items.Count != expected.Count)
		{
			return false;
		}
		int num = 0;
		while (num < expected.Count)
		{
			GameItem val = zone.items[num];
			IntPtr? intPtr = ((val != null) ? new IntPtr?(((Il2CppObjectBase)val).Pointer) : null);
			IntPtr item = expected[num].Pointer;
			if (intPtr.HasValue && !(intPtr.GetValueOrDefault() != item) && val.uniqueId == expected[num].Id && val.unitCount == expected[num].Count)
			{
				GridShape modifiedShape = val.modifiedShape;
				if (((modifiedShape != null) ? modifiedShape.minX : (-1)) == expected[num].X)
				{
					GridShape modifiedShape2 = val.modifiedShape;
					if (((modifiedShape2 != null) ? modifiedShape2.minY : (-1)) == expected[num].Y)
					{
						num++;
						continue;
					}
				}
			}
			return false;
		}
		return true;
	}

	internal static void Restore(GameItem fridge)
	{
		if (_applying || !IsFridge(fridge) || Level(fridge) == 0)
		{
			return;
		}
		try
		{
			_applying = true;
			Apply(fridge);
		}
		finally
		{
			_applying = false;
		}
	}

	internal static void CorrectPower(GameItem fridge)
	{
		if (!NativeStorageComplete(fridge))
		{
			return;
		}
		int num = Level(fridge);
		if (num != 0)
		{
			TagState tagReadonly = fridge.GetTagReadonly("FRIDGE_STORAGE_UPGRADE_COUNT_TAG");
			int num2 = 2 + 3 * ((tagReadonly != null) ? tagReadonly.GetInt() : 0);
			TagState tagReadonly2 = fridge.GetTagReadonly("FRIDGE_POWER_UPGRADE_COUNT_TAG");
			int num3 = Math.Max(2, num2 - 2 * ((tagReadonly2 != null) ? tagReadonly2.GetInt() : 0));
			TagSystem state = fridge.state;
			TagState obj = ((state != null) ? state.GetTag("machinery_drawn_power") : null);
			if (obj != null)
			{
				obj.SetInt(num3 + 3 * num);
			}
		}
	}
}
