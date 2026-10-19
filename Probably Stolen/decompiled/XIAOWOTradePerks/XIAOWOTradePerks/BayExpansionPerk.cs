using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using UnityEngine;

namespace XIAOWOTradePerks;

internal static class BayExpansionPerk
{
	internal readonly struct WindowMetrics
	{
		internal int WindowWidth { get; }

		internal int WindowHeight { get; }

		internal int ChildWidth { get; }

		internal int ChildHeight { get; }

		internal WindowMetrics(int windowWidth, int windowHeight, int childWidth, int childHeight)
		{
			WindowWidth = windowWidth;
			WindowHeight = windowHeight;
			ChildWidth = childWidth;
			ChildHeight = childHeight;
		}
	}

	internal readonly struct WindowLayoutSnapshot
	{
		internal bool Tracked { get; }

		internal int Width { get; }

		internal int Height { get; }

		internal Vector2 Position { get; }

		internal WindowLayoutSnapshot(bool tracked, int width, int height, Vector2 position)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			Tracked = tracked;
			Width = width;
			Height = height;
			Position = position;
		}
	}

	internal const string UpgradeLevelTag = "XIAOWO_BAY_EXPANSION_LEVEL";

	internal const string UnlockedColumnsTag = "XIAOWO_BAY_UNLOCKED_COLUMNS";

	internal const string UnlockedRowsTag = "XIAOWO_BAY_UNLOCKED_ROWS";

	internal const string BayLayoutVersionTag = "XIAOWO_BAY_LAYOUT_VERSION";

	private const int BayLayoutVersion = 2;

	internal const string CageSizeTag = "CAGE_SIZE_TAG";

	internal const string CageMaxHealthTag = "CAGE_MAX_HEALTH_TAG";

	internal const string CageHealthTag = "CAGE_HEALTH_TAG";

	private const string CageLayoutVersionTag = "XIAOWO_CAGE_LAYOUT_VERSION";

	private const int CageLayoutVersion = 1;

	private static readonly Dictionary<string, (int Width, int Height)[]> LegacyShapes = new Dictionary<string, (int, int)[]>(StringComparer.Ordinal)
	{
		["machine_bay"] = new(int, int)[4]
		{
			(6, 8),
			(14, 8),
			(22, 8),
			(30, 8)
		},
		["storage_bay"] = new(int, int)[4]
		{
			(10, 10),
			(17, 17),
			(23, 23),
			(30, 30)
		},
		["machine_bay_ext"] = new(int, int)[7]
		{
			(11, 14),
			(18, 16),
			(25, 18),
			(32, 20),
			(40, 23),
			(47, 25),
			(55, 28)
		},
		["storage_bay_large"] = new(int, int)[7]
		{
			(17, 10),
			(23, 13),
			(28, 17),
			(34, 20),
			(40, 23),
			(45, 27),
			(51, 30)
		},
		["smuggler_bay"] = new(int, int)[3]
		{
			(5, 5),
			(8, 8),
			(10, 10)
		},
		["smuggler_bay_mod"] = new(int, int)[6]
		{
			(8, 8),
			(11, 11),
			(14, 14),
			(16, 16),
			(18, 18),
			(20, 20)
		}
	};

	private static readonly int[] CageSizes = new int[8] { 3, 4, 5, 6, 7, 8, 12, 16 };

	private const string CageWindowKey = "__animal_cage__";

	private static readonly Dictionary<string, WindowMetrics> InitialWindowMetrics = new Dictionary<string, WindowMetrics>(StringComparer.Ordinal);

	[ThreadStatic]
	private static bool _resolvingInitialWindow;

	[ThreadStatic]
	private static bool _applyingWindowLayout;

	[ThreadStatic]
	private static IntPtr _layoutInventory;

	[ThreadStatic]
	private static IntPtr _layoutWindow;

	[ThreadStatic]
	private static int _layoutWindowWidth;

	[ThreadStatic]
	private static int _layoutWindowHeight;

	[ThreadStatic]
	private static int _layoutChildWidth;

	[ThreadStatic]
	private static int _layoutChildHeight;

	[ThreadStatic]
	private static bool _restoringVisibleShape;

	[ThreadStatic]
	private static bool _restoringCageWindow;

	[ThreadStatic]
	private static bool _upgradingBay;

	internal static long GridResizeLayoutPassCount { get; private set; }

	internal static bool IsSupportedBay(GameItem item)
	{
		if (item != null)
		{
			return LegacyShapes.ContainsKey(CleanId(item.identifier));
		}
		return false;
	}

	internal static bool IsMetalIngot(GameItem item)
	{
		if (item != null)
		{
			return string.Equals(CleanId(item.identifier), "metal_ingot", StringComparison.Ordinal);
		}
		return false;
	}

	internal static int MaximumLevel(string identifier)
	{
		if (!LegacyShapes.TryGetValue(CleanId(identifier), out (int, int)[] value))
		{
			return 0;
		}
		(int, int) tuple = value[0];
		(int, int) tuple2 = value[^1];
		return Math.Max(tuple2.Item1 - tuple.Item1, tuple2.Item2 - tuple.Item2);
	}

	internal static bool TryGetShape(string identifier, int level, out int width, out int height)
	{
		width = 0;
		height = 0;
		if (!LegacyShapes.TryGetValue(CleanId(identifier), out (int, int)[] value) || level < 0 || level > MaximumLevel(identifier))
		{
			return false;
		}
		(int, int) tuple = value[0];
		(int, int) tuple2 = value[^1];
		width = Math.Min(tuple2.Item1, tuple.Item1 + level);
		height = Math.Min(tuple2.Item2, tuple.Item2 + level);
		return true;
	}

	internal static bool TryGetUnlockedShape(GameItem item, out int width, out int height)
	{
		width = 0;
		height = 0;
		if (!IsSupportedBay(item))
		{
			return false;
		}
		int bayLevel = GetBayLevel(item);
		if (!TryGetShape(item.identifier, Math.Min(bayLevel, MaximumLevel(item.identifier)), out width, out height))
		{
			return false;
		}
		if (GetTagInt(item, "XIAOWO_BAY_LAYOUT_VERSION", 0) < 2)
		{
			return true;
		}
		string key = CleanId(item.identifier);
		(int, int) tuple = LegacyShapes[key][0];
		(int, int) tuple2 = LegacyShapes[key][^1];
		width = Math.Max(tuple.Item1, Math.Min(tuple2.Item1, GetTagInt(item, "XIAOWO_BAY_UNLOCKED_COLUMNS", width)));
		height = Math.Max(tuple.Item2, Math.Min(tuple2.Item2, GetTagInt(item, "XIAOWO_BAY_UNLOCKED_ROWS", height)));
		return true;
	}

	internal static int NextCageSize(int currentSize)
	{
		for (int i = 0; i < CageSizes.Length - 1; i++)
		{
			if (CageSizes[i] == currentSize)
			{
				return CageSizes[i + 1];
			}
		}
		return currentSize;
	}

	internal static bool TryGetCageShape(int size, out int width, out int height)
	{
		width = 0;
		height = 0;
		if (size >= 3 && size <= 8)
		{
			width = 10;
			height = size;
			return true;
		}
		switch (size)
		{
		case 12:
			width = 15;
			height = 12;
			return true;
		case 16:
			width = 20;
			height = 16;
			return true;
		default:
			return false;
		}
	}

	internal static int RemainingUpgradeCount(GameItem item)
	{
		if (item == null || !PerkRegistry.IsActive("xiaowo_trade_precision_bay_expansion"))
		{
			return -1;
		}
		if (FridgeExpansion.IsFridge(item))
		{
			return FridgeExpansion.Remaining(item);
		}
		if (IsSupportedBay(item))
		{
			return Math.Max(0, MaximumLevel(item.identifier) - GetBayLevel(item));
		}
		if (!item.IsTag("CAGE_SIZE_TAG"))
		{
			return -1;
		}
		int tagInt = GetTagInt(item, "CAGE_SIZE_TAG", CageSizes[0]);
		for (int i = 0; i < CageSizes.Length; i++)
		{
			if (CageSizes[i] == tagInt)
			{
				return CageSizes.Length - 1 - i;
			}
		}
		if (tagInt < CageSizes[^1])
		{
			return -1;
		}
		return 0;
	}

	internal static string UpgradeStatusText(GameItem item)
	{
		if (FridgeExpansion.IsFridge(item))
		{
			return FridgeExpansion.Status(item);
		}
		int num = RemainingUpgradeCount(item);
		if (num < 0)
		{
			return string.Empty;
		}
		if (!PrecisionBayMode.IsUpgradeEnabled())
		{
			return ModLocalization.Select("精工扩舱已关闭", "Precision Bay Expansion disabled");
		}
		if (num != 0)
		{
			return ModLocalization.Select("剩余升级次数：", "Upgrades remaining: ") + num;
		}
		return ModLocalization.Select("最大等级", "Maximum level");
	}

	internal static void AppendUpgradeTooltip(GameItem item, RichTextBuilder tooltip)
	{
		string text = UpgradeStatusText(item);
		if (text.Length > 0 && tooltip != null)
		{
			tooltip.AddLine(text, true, (ColorPalette)(-1), false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
		}
	}

	internal static bool CanUseIngot(GameItem target)
	{
		if (target == null || !PrecisionBayMode.IsUpgradeEnabled())
		{
			return false;
		}
		if (IsSupportedBay(target))
		{
			return GetBayLevel(target) < MaximumLevel(target.identifier);
		}
		if (!target.IsTag("CAGE_SIZE_TAG"))
		{
			return false;
		}
		int tagInt = GetTagInt(target, "CAGE_SIZE_TAG", 0);
		if (tagInt >= 8)
		{
			return NextCageSize(tagInt) > tagInt;
		}
		return false;
	}

	internal static bool CanTargetWithIngot(GameItem source, GameItem target)
	{
		if (!IsMetalIngot(source) || target == null || !PrecisionBayMode.IsUpgradeEnabled())
		{
			return false;
		}
		if (FridgeExpansion.IsFridge(target))
		{
			return FridgeExpansion.CanUpgrade(target);
		}
		if (!IsSupportedBay(target))
		{
			return CanUseIngot(target);
		}
		if (GetBayLevel(target) < MaximumLevel(target.identifier))
		{
			return true;
		}
		GameGridInventory bayInventory = GetBayInventory(target);
		if (bayInventory != null)
		{
			return ((GameInventory)bayInventory).MayHaveValidInventorySlot(source);
		}
		return false;
	}

	internal static bool TryHandleIngot(GameItem source, GameItem target)
	{
		if (target == null || !PrecisionBayMode.IsUpgradeEnabled())
		{
			return false;
		}
		string details;
		if (IsMetalIngot(source) && FridgeExpansion.IsFridge(target))
		{
			if (!FridgeExpansion.CanUpgrade(target))
			{
				return false;
			}
			FridgeExpansion.TryUpgrade(target, source, out details);
			return true;
		}
		if (IsSupportedBay(target))
		{
			if (GetBayLevel(target) >= MaximumLevel(target.identifier))
			{
				GameGridInventory bayInventory = GetBayInventory(target);
				if (source != null && bayInventory != null && ((GameInventory)bayInventory).MayHaveValidInventorySlot(source))
				{
					return TryTransferToInventory(source, (GameInventory)(object)bayInventory);
				}
				return true;
			}
			TryUpgradeBay(target, source, out details);
			return true;
		}
		if (!target.IsTag("CAGE_SIZE_TAG"))
		{
			return false;
		}
		int tagInt = GetTagInt(target, "CAGE_SIZE_TAG", 0);
		if (tagInt < 8)
		{
			return false;
		}
		if (NextCageSize(tagInt) <= tagInt)
		{
			return true;
		}
		TryUpgradeCage(target, source, out details);
		return true;
	}

	internal static bool TryUpgradeBay(GameItem target, GameItem ingot, out string details)
	{
		details = string.Empty;
		if (!IsSupportedBay(target))
		{
			details = "unsupported bay";
			return false;
		}
		int bayLevel = GetBayLevel(target);
		int num = MaximumLevel(target.identifier);
		if (bayLevel >= num)
		{
			details = "maximum level";
			return false;
		}
		GameGridInventory bayInventory = GetBayInventory(target);
		if (bayInventory == null || bayInventory.inventoryShape == null)
		{
			details = "inventory unavailable";
			return false;
		}
		if (!TryGetUnlockedShape(target, out var width, out var height))
		{
			details = "unlocked shape unavailable";
			return false;
		}
		(int Width, int Height)[] array = LegacyShapes[CleanId(target.identifier)];
		int num2 = Math.Min(array[^1].Width, width + 1);
		int num3 = Math.Min(array[^1].Height, height + 1);
		int width2 = bayInventory.inventoryShape.width;
		int height2 = bayInventory.inventoryShape.height;
		int level = bayLevel;
		List<(IntPtr, int, int)> list = CaptureItems(bayInventory);
		EnsureInitialWindowMetrics(target, bayInventory, cage: false);
		WindowLayoutSnapshot snapshot = CaptureUpgradeWindowLayout(target);
		try
		{
			_upgradingBay = true;
			SetBayShapeState(target, bayLevel + 1, num2, num3);
			if (!ApplyStoredShape(target) || !ItemsMatch(bayInventory, list))
			{
				throw new InvalidOperationException("inventory contents changed while resizing");
			}
			target.ValidateShapeState();
			ApplyResizableWindow(target, bayInventory, cage: false);
			RestoreUpgradeWindowLayout(target, bayInventory, snapshot);
			if (bayInventory.inventoryShape.width != num2 || bayInventory.inventoryShape.height != num3 || !ItemsMatch(bayInventory, list))
			{
				throw new InvalidOperationException("unlocked shape or inventory contents changed during validation");
			}
			RememberGridShape(target, num2, num3);
			if (ingot != null)
			{
				ingot.Destroy();
			}
			details = CleanId(target.identifier) + " level=" + (bayLevel + 1) + " shape=" + bayInventory.inventoryShape.width + "x" + bayInventory.inventoryShape.height + " items=" + list.Count;
			return true;
		}
		catch (Exception ex)
		{
			SetBayShapeState(target, level, width, height);
			bayInventory.SetShape(width2, height2);
			PixelWindow contentWindow = target.contentWindow;
			if (contentWindow != null)
			{
				contentWindow.Validate();
			}
			ApplyResizableWindow(target, bayInventory, cage: false);
			RestoreUpgradeWindowLayout(target, bayInventory, snapshot);
			details = ex.GetType().Name + ": " + ex.Message;
			Instance log = Core.Log;
			if (log != null)
			{
				log.Error("Bay expansion rolled back: " + details);
			}
			return false;
		}
		finally
		{
			_upgradingBay = false;
		}
	}

	internal static bool TryUpgradeCage(GameItem cage, GameItem ingot, out string details)
	{
		details = string.Empty;
		GameGridInventory val = ((cage == null) ? null : AnimalCage.GetCageInventory(cage));
		int tagInt = GetTagInt(cage, "CAGE_SIZE_TAG", 0);
		int num = NextCageSize(tagInt);
		if (val == null || num <= tagInt)
		{
			details = "cage cannot be expanded";
			return false;
		}
		List<(IntPtr, int, int)> list = CaptureItems(val);
		int width = val.inventoryShape.width;
		int height = val.inventoryShape.height;
		int tagInt2 = GetTagInt(cage, "CAGE_MAX_HEALTH_TAG", 0);
		int tagInt3 = GetTagInt(cage, "CAGE_HEALTH_TAG", 0);
		int tagInt4 = GetTagInt(cage, "XIAOWO_CAGE_LAYOUT_VERSION", 0);
		long unitValue = cage.unitValue;
		EnsureInitialWindowMetrics(cage, val, cage: true);
		WindowLayoutSnapshot snapshot = CaptureUpgradeWindowLayout(cage);
		try
		{
			SetTagInt(cage, "CAGE_SIZE_TAG", num);
			SetTagInt(cage, "CAGE_MAX_HEALTH_TAG", tagInt2 + 20);
			SetTagInt(cage, "CAGE_HEALTH_TAG", Math.Min(tagInt3 + 20, tagInt2 + 20));
			SetTagInt(cage, "XIAOWO_CAGE_LAYOUT_VERSION", 1);
			cage.unitValue = unitValue + 40;
			if (!TryGetCageShape(num, out var width2, out var height2))
			{
				throw new InvalidOperationException("cage shape unavailable");
			}
			val.SetShape(width2, height2);
			PixelWindow contentWindow = cage.contentWindow;
			if (contentWindow != null)
			{
				contentWindow.Validate();
			}
			if (!ItemsMatch(val, list))
			{
				throw new InvalidOperationException("cage contents changed while resizing");
			}
			cage.ValidateShapeState();
			ApplyResizableWindow(cage, val, cage: true);
			RestoreUpgradeWindowLayout(cage, val, snapshot);
			if (!ItemsMatch(val, list))
			{
				throw new InvalidOperationException("cage contents changed during validation");
			}
			RememberGridShape(cage, val.inventoryShape.width, val.inventoryShape.height);
			if (ingot != null)
			{
				ingot.Destroy();
			}
			details = "cage size=" + width2 + "x" + height2 + " items=" + list.Count;
			return true;
		}
		catch (Exception ex)
		{
			SetTagInt(cage, "CAGE_SIZE_TAG", tagInt);
			SetTagInt(cage, "CAGE_MAX_HEALTH_TAG", tagInt2);
			SetTagInt(cage, "CAGE_HEALTH_TAG", tagInt3);
			SetTagInt(cage, "XIAOWO_CAGE_LAYOUT_VERSION", tagInt4);
			cage.unitValue = unitValue;
			val.SetShape(width, height);
			PixelWindow contentWindow2 = cage.contentWindow;
			if (contentWindow2 != null)
			{
				contentWindow2.Validate();
			}
			ApplyResizableWindow(cage, val, cage: true);
			RestoreUpgradeWindowLayout(cage, val, snapshot);
			details = ex.GetType().Name + ": " + ex.Message;
			Instance log = Core.Log;
			if (log != null)
			{
				log.Error("Cage expansion rolled back: " + details);
			}
			return false;
		}
	}

	internal static bool ApplyStoredShape(GameItem item)
	{
		if (!IsSupportedBay(item))
		{
			return false;
		}
		if (!TryGetUnlockedShape(item, out var width, out var height))
		{
			return false;
		}
		GameGridInventory bayInventory = GetBayInventory(item);
		if (((bayInventory != null) ? bayInventory.inventoryShape : null) == null)
		{
			return false;
		}
		if (bayInventory.inventoryShape.width != width || bayInventory.inventoryShape.height != height)
		{
			bayInventory.SetShape(width, height);
			if (bayInventory.inventoryShape.width != width || bayInventory.inventoryShape.height != height)
			{
				bayInventory.SetShape(width, height);
			}
		}
		ApplyResizableWindow(item, bayInventory, cage: false);
		return true;
	}

	internal static void RestoreExpandedItem(GameItem item)
	{
		if (((item != null) ? item.contentWindow : null) == null)
		{
			return;
		}
		if (IsSupportedBay(item))
		{
			if (!TryRestoreRememberedGridShape(item))
			{
				ApplyStoredShape(item);
			}
		}
		else if (item != null && item.IsTag("CAGE_SIZE_TAG") && GetTagInt(item, "CAGE_SIZE_TAG", 0) >= 8 && !TryRestoreRememberedGridShape(item))
		{
			GameGridInventory cageInventory = AnimalCage.GetCageInventory(item);
			if (cageInventory != null)
			{
				ApplyResizableWindow(item, cageInventory, cage: true);
			}
		}
	}

	internal static void RestoreLoadedCageWindow(GameItem cage, GameGridInventory inventory)
	{
		if (_restoringCageWindow || cage == null || inventory == null || !PerkRegistry.IsActive("xiaowo_trade_precision_bay_expansion") || !cage.IsTag("CAGE_SIZE_TAG") || GetTagInt(cage, "CAGE_SIZE_TAG", 0) < 8)
		{
			return;
		}
		try
		{
			_restoringCageWindow = true;
			NormalizeLoadedCageShape(cage, inventory);
			ApplyResizableWindow(cage, inventory, cage: true);
		}
		finally
		{
			_restoringCageWindow = false;
		}
	}

	internal static bool TryGetInitialWindowSize(GameItem item, out int width, out int height)
	{
		width = 0;
		height = 0;
		bool flag = item != null && item.IsTag("CAGE_SIZE_TAG");
		GameGridInventory val = (flag ? AnimalCage.GetCageInventory(item) : GetBayInventory(item));
		if (val == null || !EnsureInitialWindowMetrics(item, val, flag))
		{
			return false;
		}
		string key = (flag ? "__animal_cage__" : CleanId(item.identifier));
		WindowMetrics windowMetrics = InitialWindowMetrics[key];
		width = windowMetrics.WindowWidth;
		height = windowMetrics.WindowHeight;
		return true;
	}

	internal static bool TryGetInitialWindowMetrics(GameItem item, out WindowMetrics metrics)
	{
		metrics = default(WindowMetrics);
		bool flag = item != null && item.IsTag("CAGE_SIZE_TAG") && !IsSupportedBay(item);
		GameGridInventory val = (flag ? AnimalCage.GetCageInventory(item) : GetBayInventory(item));
		if (val == null || !EnsureInitialWindowMetrics(item, val, flag))
		{
			return false;
		}
		string key = (flag ? "__animal_cage__" : CleanId(item.identifier));
		metrics = InitialWindowMetrics[key];
		return true;
	}

	internal static bool IsResizableContainer(GameItem item)
	{
		if (item == null || !PerkRegistry.IsActive("xiaowo_trade_precision_bay_expansion"))
		{
			return false;
		}
		if (IsSupportedBay(item))
		{
			return true;
		}
		if (item.IsTag("CAGE_SIZE_TAG"))
		{
			return GetTagInt(item, "CAGE_SIZE_TAG", 0) >= 8;
		}
		return false;
	}

	internal static WindowLayoutSnapshot CaptureWindowLayout(GameItem item)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		PixelWindow val = ((item != null) ? item.contentWindow : null);
		RectTransform val2 = ((val != null) ? val.rectTransform : null);
		bool num;
		if (!IsSupportedBay(item))
		{
			if (item == null || !item.IsTag("CAGE_SIZE_TAG"))
			{
				goto IL_007b;
			}
			num = GetTagInt(item, "CAGE_SIZE_TAG", 0) > 8;
		}
		else
		{
			num = GetBayLevel(item) > 0;
		}
		if (num && IsResizableContainer(item) && !((Object)(object)val2 == (Object)null))
		{
			return new WindowLayoutSnapshot(tracked: true, val.widthPixels, val.heightPixels, val2.anchoredPosition);
		}
		goto IL_007b;
		IL_007b:
		return default(WindowLayoutSnapshot);
	}

	internal static WindowLayoutSnapshot CaptureUpgradeWindowLayout(GameItem item)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		PixelWindow val = ((item != null) ? item.contentWindow : null);
		RectTransform val2 = ((val != null) ? val.rectTransform : null);
		if ((IsSupportedBay(item) || (item != null && item.IsTag("CAGE_SIZE_TAG") && GetTagInt(item, "CAGE_SIZE_TAG", 0) >= 8)) && IsResizableContainer(item) && !((Object)(object)val2 == (Object)null) && ((Component)val2).gameObject.activeInHierarchy)
		{
			return new WindowLayoutSnapshot(tracked: true, val.widthPixels, val.heightPixels, val2.anchoredPosition);
		}
		return default(WindowLayoutSnapshot);
	}

	internal static void RestoreWindowLayout(GameItem item, WindowLayoutSnapshot snapshot, bool rememberSize = false)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (snapshot.Tracked)
		{
			object obj;
			if (item == null)
			{
				obj = null;
			}
			else
			{
				PixelWindow contentWindow = item.contentWindow;
				obj = ((contentWindow != null) ? contentWindow.rectTransform : null);
			}
			if (!((Object)obj == (Object)null))
			{
				TryResizeWindow(item, snapshot.Width, snapshot.Height, rememberSize, out var _, out var _);
				item.contentWindow.rectTransform.anchoredPosition = snapshot.Position;
				item.RememberContentWindowPositionIfMine(item.contentWindow);
			}
		}
	}

	internal static void RestoreUpgradeWindowLayout(GameItem item, GameGridInventory inventory, WindowLayoutSnapshot snapshot)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (snapshot.Tracked && inventory != null && TryGetInitialWindowMetrics(item, out var metrics))
		{
			int val = inventory.widthPixels + (metrics.WindowWidth - metrics.ChildWidth);
			int val2 = inventory.heightPixels + (metrics.WindowHeight - metrics.ChildHeight);
			int requestedWidth = Math.Max(snapshot.Width, val);
			int requestedHeight = Math.Max(snapshot.Height, val2);
			TryResizeWindow(item, requestedWidth, requestedHeight, remember: true, out var _, out var _);
			item.contentWindow.rectTransform.anchoredPosition = snapshot.Position;
			item.RememberContentWindowPositionIfMine(item.contentWindow);
		}
	}

	internal static bool TryResizeWindow(GameItem item, int requestedWidth, int requestedHeight, bool remember, out int actualWidth, out int actualHeight)
	{
		actualWidth = 0;
		actualHeight = 0;
		if (!IsResizableContainer(item) || !TryGetInitialWindowMetrics(item, out var metrics))
		{
			return false;
		}
		PixelWindow contentWindow = item.contentWindow;
		GameGridInventory val = ((!IsSupportedBay(item)) ? AnimalCage.GetCageInventory(item) : GetBayInventory(item));
		if (contentWindow == null || val == null)
		{
			return false;
		}
		actualWidth = Math.Max(metrics.WindowWidth, requestedWidth);
		actualHeight = Math.Max(metrics.WindowHeight, requestedHeight);
		ApplyWindowSize(contentWindow, val, metrics, actualWidth, actualHeight, validate: false);
		actualWidth = contentWindow.widthPixels;
		actualHeight = contentWindow.heightPixels;
		if (remember)
		{
			RememberWindowSize(item, actualWidth, actualHeight);
		}
		return true;
	}

	internal static bool TryResizeGridWindow(GameItem item, int requestedColumns, int requestedRows, bool remember, out int actualWidth, out int actualHeight)
	{
		actualWidth = 0;
		actualHeight = 0;
		if (!TryGetGridShapeLimits(item, out var minimumColumns, out var minimumRows, out var maximumColumns, out var maximumRows) || !TryGetInitialWindowMetrics(item, out var metrics))
		{
			return false;
		}
		GameGridInventory resizableInventory = GetResizableInventory(item);
		PixelWindow val = ((item != null) ? item.contentWindow : null);
		if (((resizableInventory != null) ? resizableInventory.inventoryShape : null) == null || val == null)
		{
			return false;
		}
		int num = Math.Max(minimumColumns, Math.Min(maximumColumns, requestedColumns));
		int num2 = Math.Max(minimumRows, Math.Min(maximumRows, requestedRows));
		int width = resizableInventory.inventoryShape.width;
		int height = resizableInventory.inventoryShape.height;
		if (num == width && num2 == height)
		{
			if (!InventoryBackgroundMatches(resizableInventory))
			{
				ApplyWindowSize(val, resizableInventory, metrics, val.widthPixels, val.heightPixels, validate: true);
			}
			actualWidth = val.widthPixels;
			actualHeight = val.heightPixels;
			return true;
		}
		GridResizeLayoutPassCount++;
		List<(IntPtr, int, int)> expected = CaptureItems(resizableInventory);
		if ((num < width || num2 < height) && !CanContainItems(resizableInventory, num, num2))
		{
			ApplyWindowToCurrentShape(val, resizableInventory, metrics, out actualWidth, out actualHeight);
			return false;
		}
		try
		{
			if (num != width || num2 != height)
			{
				resizableInventory.SetShape(num, num2);
				if (resizableInventory.inventoryShape.width != num || resizableInventory.inventoryShape.height != num2)
				{
					resizableInventory.SetShape(num, num2);
				}
				if (resizableInventory.inventoryShape.width != num || resizableInventory.inventoryShape.height != num2)
				{
					throw new InvalidOperationException("inventory rejected requested visible grid shape");
				}
			}
			if (!ItemsMatch(resizableInventory, expected))
			{
				throw new InvalidOperationException("inventory contents changed while changing visible grid shape");
			}
			resizableInventory.ForceRecomputeDynamicBackground();
			ApplyWindowToCurrentShape(val, resizableInventory, metrics, out actualWidth, out actualHeight);
			if (remember)
			{
				RememberGridShape(item, num, num2);
				RememberWindowSize(item, actualWidth, actualHeight);
			}
			return true;
		}
		catch (Exception ex)
		{
			if (resizableInventory.inventoryShape.width != width || resizableInventory.inventoryShape.height != height)
			{
				resizableInventory.SetShape(width, height);
			}
			ApplyWindowToCurrentShape(val, resizableInventory, metrics, out actualWidth, out actualHeight);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Error("Visible container grid resize rolled back: " + ex);
			}
			return false;
		}
	}

	internal static bool NeedsShapeRestore(GameItem item)
	{
		if (((item != null) ? item.contentWindow : null) == null || !PerkRegistry.IsActive("xiaowo_trade_precision_bay_expansion"))
		{
			return false;
		}
		if (IsSupportedBay(item))
		{
			return true;
		}
		if (item.IsTag("CAGE_SIZE_TAG"))
		{
			return GetTagInt(item, "CAGE_SIZE_TAG", 0) >= 8;
		}
		return false;
	}

	internal static bool TryGetGridShapeLimits(GameItem item, out int minimumColumns, out int minimumRows, out int maximumColumns, out int maximumRows)
	{
		minimumColumns = 0;
		minimumRows = 0;
		maximumColumns = 0;
		maximumRows = 0;
		if (!IsResizableContainer(item))
		{
			return false;
		}
		if (IsSupportedBay(item))
		{
			if (!TryGetShape(item.identifier, 0, out minimumColumns, out minimumRows))
			{
				return false;
			}
			return TryGetUnlockedShape(item, out maximumColumns, out maximumRows);
		}
		minimumColumns = 10;
		minimumRows = 8;
		return TryGetCageShape(GetTagInt(item, "CAGE_SIZE_TAG", 0), out maximumColumns, out maximumRows);
	}

	internal static GameGridInventory GetResizableInventory(GameItem item)
	{
		if (item == null || !item.IsTag("CAGE_SIZE_TAG") || IsSupportedBay(item))
		{
			return GetBayInventory(item);
		}
		return AnimalCage.GetCageInventory(item);
	}

	private static void NormalizeLoadedCageShape(GameItem cage, GameGridInventory inventory)
	{
		if (cage == null || ((inventory != null) ? inventory.inventoryShape : null) == null || GetTagInt(cage, "XIAOWO_CAGE_LAYOUT_VERSION", 0) >= 1)
		{
			return;
		}
		int tagInt = GetTagInt(cage, "CAGE_SIZE_TAG", 0);
		if (tagInt == 10 || tagInt == 12 || tagInt == 14 || tagInt == 15 || tagInt == 16)
		{
			tagInt = 16;
			SetTagInt(cage, "CAGE_SIZE_TAG", tagInt);
			SetTagInt(cage, "XIAOWO_CAGE_LAYOUT_VERSION", 1);
			if (TryGetCageShape(tagInt, out var width, out var height) && (inventory.inventoryShape.width != width || inventory.inventoryShape.height != height))
			{
				inventory.SetShape(width, height);
			}
		}
	}

	internal static void RememberGridShape(GameItem item, int columns, int rows)
	{
		if (TryGetGridShapeLimits(item, out var minimumColumns, out var minimumRows, out var maximumColumns, out var maximumRows))
		{
			PlayerPrefs.SetInt(WindowSizeKey(item, "Columns"), Math.Max(minimumColumns, Math.Min(maximumColumns, columns)));
			PlayerPrefs.SetInt(WindowSizeKey(item, "Rows"), Math.Max(minimumRows, Math.Min(maximumRows, rows)));
			PlayerPrefs.Save();
		}
	}

	internal static bool TryRestoreRememberedGridShape(GameItem item)
	{
		if (!_upgradingBay && !_restoringVisibleShape)
		{
			object obj;
			if (item == null)
			{
				obj = null;
			}
			else
			{
				PixelWindow contentWindow = item.contentWindow;
				obj = ((contentWindow != null) ? contentWindow.rectTransform : null);
			}
			if (!((Object)obj == (Object)null) && ((Component)item.contentWindow.rectTransform).gameObject.activeInHierarchy)
			{
				string text = WindowSizeKey(item, "Columns");
				string text2 = WindowSizeKey(item, "Rows");
				if (!PlayerPrefs.HasKey(text) || !PlayerPrefs.HasKey(text2) || !TryGetGridShapeLimits(item, out var minimumColumns, out var minimumRows, out var maximumColumns, out var maximumRows))
				{
					return false;
				}
				int requestedColumns = Math.Max(minimumColumns, Math.Min(maximumColumns, PlayerPrefs.GetInt(text, maximumColumns)));
				int requestedRows = Math.Max(minimumRows, Math.Min(maximumRows, PlayerPrefs.GetInt(text2, maximumRows)));
				try
				{
					_restoringVisibleShape = true;
					if (TryResizeGridWindow(item, requestedColumns, requestedRows, remember: false, out var _, out var _))
					{
						return true;
					}
					GameGridInventory resizableInventory = GetResizableInventory(item);
					if (((resizableInventory != null) ? resizableInventory.inventoryShape : null) != null)
					{
						RememberGridShape(item, resizableInventory.inventoryShape.width, resizableInventory.inventoryShape.height);
					}
					return false;
				}
				finally
				{
					_restoringVisibleShape = false;
				}
			}
		}
		return false;
	}

	internal static void RememberWindowSize(GameItem item, int width, int height)
	{
		if (TryGetInitialWindowMetrics(item, out var metrics))
		{
			PlayerPrefs.SetInt(WindowSizeKey(item, "Width"), Math.Max(metrics.WindowWidth, width));
			PlayerPrefs.SetInt(WindowSizeKey(item, "Height"), Math.Max(metrics.WindowHeight, height));
			PlayerPrefs.Save();
		}
	}

	internal static void ForgetWindowSize(GameItem item)
	{
		if (item != null)
		{
			PlayerPrefs.DeleteKey(WindowSizeKey(item, "Width"));
			PlayerPrefs.DeleteKey(WindowSizeKey(item, "Height"));
			PlayerPrefs.DeleteKey(WindowSizeKey(item, "Columns"));
			PlayerPrefs.DeleteKey(WindowSizeKey(item, "Rows"));
			PlayerPrefs.Save();
		}
	}

	internal static bool TryOverrideChildWidth(GameGridInventory inventory, ref int value)
	{
		if (_layoutInventory == IntPtr.Zero || inventory == null || ((Il2CppObjectBase)inventory).Pointer != _layoutInventory)
		{
			return false;
		}
		value = _layoutChildWidth;
		return true;
	}

	internal static bool TryOverrideChildHeight(GameGridInventory inventory, ref int value)
	{
		if (_layoutInventory == IntPtr.Zero || inventory == null || ((Il2CppObjectBase)inventory).Pointer != _layoutInventory)
		{
			return false;
		}
		value = _layoutChildHeight;
		return true;
	}

	internal static bool TryOverrideWindowWidth(PixelWindow window, ref int value)
	{
		if (_layoutWindow == IntPtr.Zero || window == null || ((Il2CppObjectBase)window).Pointer != _layoutWindow)
		{
			return false;
		}
		value = _layoutWindowWidth;
		return true;
	}

	internal static bool TryOverrideWindowHeight(PixelWindow window, ref int value)
	{
		if (_layoutWindow == IntPtr.Zero || window == null || ((Il2CppObjectBase)window).Pointer != _layoutWindow)
		{
			return false;
		}
		value = _layoutWindowHeight;
		return true;
	}

	private static void ApplyResizableWindow(GameItem item, GameGridInventory inventory, bool cage)
	{
		if (!_applyingWindowLayout && ((item != null) ? item.contentWindow : null) != null && inventory != null && EnsureInitialWindowMetrics(item, inventory, cage))
		{
			string key = (cage ? "__animal_cage__" : CleanId(item.identifier));
			WindowMetrics metrics = InitialWindowMetrics[key];
			int widthPixels = inventory.widthPixels;
			int heightPixels = inventory.heightPixels;
			int val = widthPixels + (metrics.WindowWidth - metrics.ChildWidth);
			int val2 = heightPixels + (metrics.WindowHeight - metrics.ChildHeight);
			int width = Math.Max(val, Math.Max(metrics.WindowWidth, PlayerPrefs.GetInt(WindowSizeKey(item, "Width"), metrics.WindowWidth)));
			int height = Math.Max(val2, Math.Max(metrics.WindowHeight, PlayerPrefs.GetInt(WindowSizeKey(item, "Height"), metrics.WindowHeight)));
			ApplyWindowSize(item.contentWindow, inventory, metrics, width, height, validate: true);
			ContainerWindowResizer.Attach(item);
		}
	}

	private static void ApplyWindowSize(PixelWindow window, GameGridInventory inventory, WindowMetrics metrics, int width, int height, bool validate)
	{
		try
		{
			_applyingWindowLayout = true;
			_layoutInventory = ((Il2CppObjectBase)inventory).Pointer;
			_layoutWindow = ((Il2CppObjectBase)window).Pointer;
			_layoutWindowWidth = width;
			_layoutWindowHeight = height;
			_layoutChildWidth = Math.Max(metrics.ChildWidth, width - (metrics.WindowWidth - metrics.ChildWidth));
			_layoutChildHeight = Math.Max(metrics.ChildHeight, height - (metrics.WindowHeight - metrics.ChildHeight));
			window._widthPixels_k__BackingField = width;
			window._heightPixels_k__BackingField = height;
			window.Validate();
			window._widthPixels_k__BackingField = width;
			window._heightPixels_k__BackingField = height;
			inventory.ForceRecomputeDynamicBackground();
			Canvas.ForceUpdateCanvases();
		}
		finally
		{
			_layoutInventory = IntPtr.Zero;
			_layoutWindow = IntPtr.Zero;
			_layoutWindowWidth = 0;
			_layoutWindowHeight = 0;
			_layoutChildWidth = 0;
			_layoutChildHeight = 0;
			_applyingWindowLayout = false;
		}
	}

	private static bool InventoryBackgroundMatches(GameGridInventory inventory)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (inventory == null)
		{
			return true;
		}
		try
		{
			TreeNodeRender background = inventory.background;
			RectTransform val = ((background != null) ? background.rectTransform : null);
			Rect rect;
			if ((Object)(object)val != (Object)null)
			{
				rect = val.rect;
				if (Approximately(((Rect)(ref rect)).width, inventory.widthPixels))
				{
					rect = val.rect;
					if (Approximately(((Rect)(ref rect)).height, inventory.heightPixels))
					{
						goto IL_005e;
					}
				}
				return false;
			}
			goto IL_005e;
			IL_005e:
			TreeNodeRender insideBorder = inventory.insideBorder;
			RectTransform val2 = ((insideBorder != null) ? insideBorder.rectTransform : null);
			int result;
			if (!((Object)(object)val2 == (Object)null))
			{
				rect = val2.rect;
				if (Approximately(((Rect)(ref rect)).width, (float)inventory.widthPixels + 2f))
				{
					rect = val2.rect;
					result = (Approximately(((Rect)(ref rect)).height, (float)inventory.heightPixels + 2f) ? 1 : 0);
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}
		catch
		{
			return false;
		}
	}

	private static bool Approximately(float left, float right)
	{
		return Math.Abs(left - right) < 0.1f;
	}

	private static void ApplyWindowToCurrentShape(PixelWindow window, GameGridInventory inventory, WindowMetrics metrics, out int actualWidth, out int actualHeight)
	{
		int num = metrics.WindowWidth - metrics.ChildWidth;
		int num2 = metrics.WindowHeight - metrics.ChildHeight;
		actualWidth = Math.Max(metrics.WindowWidth, inventory.widthPixels + num);
		actualHeight = Math.Max(metrics.WindowHeight, inventory.heightPixels + num2);
		ApplyWindowSize(window, inventory, metrics, actualWidth, actualHeight, validate: true);
		actualWidth = window.widthPixels;
		actualHeight = window.heightPixels;
	}

	private static bool EnsureInitialWindowMetrics(GameItem item, GameGridInventory inventory, bool cage)
	{
		string text = (cage ? "__animal_cage__" : CleanId((item != null) ? item.identifier : null));
		if (text.Length == 0)
		{
			return false;
		}
		if (InitialWindowMetrics.ContainsKey(text))
		{
			return true;
		}
		if ((cage ? (GetTagInt(item, "CAGE_SIZE_TAG", 0) <= 8) : (GetBayLevel(item) <= 0)) && TryCaptureWindowMetrics(item, inventory, out var metrics))
		{
			InitialWindowMetrics[text] = metrics;
			return true;
		}
		if (_resolvingInitialWindow)
		{
			return false;
		}
		GameItem val = null;
		try
		{
			_resolvingInitialWindow = true;
			val = (cage ? AnimalCage.CreateAnimalCage() : DirectoryMaster.Item(text, true));
			GameGridInventory inventory2 = (cage ? AnimalCage.GetCageInventory(val) : GetBayInventory(val));
			if (!TryCaptureWindowMetrics(val, inventory2, out var metrics2))
			{
				return false;
			}
			InitialWindowMetrics[text] = metrics2;
			return true;
		}
		catch (Exception ex)
		{
			Instance log = Core.Log;
			if (log != null)
			{
				log.Warning("Could not resolve initial container window size for " + text + ": " + ex.Message);
			}
			return false;
		}
		finally
		{
			if (val != null)
			{
				val.Destroy();
			}
			_resolvingInitialWindow = false;
		}
	}

	private static bool TryCaptureWindowMetrics(GameItem item, GameGridInventory inventory, out WindowMetrics metrics)
	{
		metrics = default(WindowMetrics);
		PixelWindow val = ((item != null) ? item.contentWindow : null);
		if (val == null || inventory == null)
		{
			return false;
		}
		int widthPixels = val.widthPixels;
		int heightPixels = val.heightPixels;
		int widthPixels2 = inventory.widthPixels;
		int heightPixels2 = inventory.heightPixels;
		if (widthPixels <= 0 || heightPixels <= 0 || widthPixels2 <= 0 || heightPixels2 <= 0)
		{
			return false;
		}
		metrics = new WindowMetrics(widthPixels, heightPixels, widthPixels2, heightPixels2);
		return true;
	}

	internal static GameGridInventory GetBayInventory(GameItem item)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		object obj;
		if (item == null)
		{
			obj = null;
		}
		else
		{
			PixelWindow contentWindow = item.contentWindow;
			obj = ((contentWindow != null) ? contentWindow.child : null);
		}
		GraphNodeStorage val = (GraphNodeStorage)obj;
		if (val != null)
		{
			return new GameGridInventory(((Il2CppObjectBase)val).Pointer);
		}
		return null;
	}

	private static bool TryTransferToInventory(GameItem item, GameInventory target)
	{
		if (item == null || target == null)
		{
			return true;
		}
		GameInventory parentInventory = item.parentInventory;
		if (parentInventory == null)
		{
			target.UncheckedAccept(item);
			return true;
		}
		if (((Il2CppObjectBase)parentInventory).Pointer == ((Il2CppObjectBase)target).Pointer)
		{
			return true;
		}
		if (!parentInventory.Expel(item))
		{
			return true;
		}
		if (target.UncheckedAccept(item))
		{
			return true;
		}
		if (!parentInventory.UncheckedAccept(item))
		{
			Instance log = Core.Log;
			if (log != null)
			{
				log.Error("Failed to return a capped upgrade ingot to its source inventory.");
			}
		}
		return true;
	}

	internal static int GetBayLevel(GameItem item)
	{
		int num = Math.Max(0, GetTagInt(item, "XIAOWO_BAY_EXPANSION_LEVEL", 0));
		if (num == 0 || !IsSupportedBay(item) || GetTagInt(item, "XIAOWO_BAY_LAYOUT_VERSION", 0) >= 2)
		{
			return num;
		}
		(int, int)[] array = LegacyShapes[CleanId(item.identifier)];
		(int, int) tuple = array[Math.Min(num, array.Length - 1)];
		GameGridInventory bayInventory = GetBayInventory(item);
		(int, int) tuple2 = array[^1];
		var (val, _) = tuple2;
		var (val2, _) = tuple;
		int? obj;
		if (bayInventory == null)
		{
			obj = null;
		}
		else
		{
			GridShape inventoryShape = bayInventory.inventoryShape;
			obj = ((inventoryShape != null) ? new int?(inventoryShape.width) : null);
		}
		int? num2 = obj;
		int num3 = Math.Min(val, Math.Max(val2, num2.GetValueOrDefault()));
		int item2 = tuple2.Item2;
		int item3 = tuple.Item2;
		int? obj2;
		if (bayInventory == null)
		{
			obj2 = null;
		}
		else
		{
			GridShape inventoryShape2 = bayInventory.inventoryShape;
			obj2 = ((inventoryShape2 != null) ? new int?(inventoryShape2.height) : null);
		}
		num2 = obj2;
		int num4 = Math.Min(item2, Math.Max(item3, num2.GetValueOrDefault()));
		int num5 = Math.Max(num3 - array[0].Item1, num4 - array[0].Item2);
		SetBayShapeState(item, num5, num3, num4);
		return num5;
	}

	private static void SetBayShapeState(GameItem item, int level, int width, int height)
	{
		SetTagInt(item, "XIAOWO_BAY_UNLOCKED_COLUMNS", width);
		SetTagInt(item, "XIAOWO_BAY_UNLOCKED_ROWS", height);
		SetTagInt(item, "XIAOWO_BAY_EXPANSION_LEVEL", level);
		SetTagInt(item, "XIAOWO_BAY_LAYOUT_VERSION", 2);
	}

	private static int GetTagInt(GameItem item, string tag, int fallback)
	{
		try
		{
			TagState val = ((item != null) ? item.GetTagReadonly(tag) : null);
			return (val == null) ? fallback : val.GetInt();
		}
		catch
		{
			return fallback;
		}
	}

	private static void SetTagInt(GameItem item, string tag, int value)
	{
		if (((item != null) ? item.state : null) == null)
		{
			throw new InvalidOperationException("item state unavailable");
		}
		if (!item.IsTag(tag))
		{
			item.state.EnableTag(tag);
		}
		(item.state.GetTag(tag) ?? throw new InvalidOperationException("tag unavailable: " + tag)).SetInt(value);
	}

	private static List<(IntPtr Pointer, int UniqueId, int UnitCount)> CaptureItems(GameGridInventory inventory)
	{
		List<(IntPtr, int, int)> list = new List<(IntPtr, int, int)>();
		if (((inventory != null) ? inventory.items : null) == null)
		{
			return list;
		}
		for (int i = 0; i < inventory.items.Count; i++)
		{
			GameItem val = inventory.items[i];
			if (val != null)
			{
				list.Add((((Il2CppObjectBase)val).Pointer, val.uniqueId, val.unitCount));
			}
		}
		return list;
	}

	private static bool ItemsMatch(GameGridInventory inventory, IReadOnlyList<(IntPtr Pointer, int UniqueId, int UnitCount)> expected)
	{
		if (((inventory != null) ? inventory.items : null) == null || inventory.items.Count != expected.Count)
		{
			return false;
		}
		for (int i = 0; i < expected.Count; i++)
		{
			GameItem val = inventory.items[i];
			if (val == null || ((Il2CppObjectBase)val).Pointer != expected[i].Pointer || val.uniqueId != expected[i].UniqueId || val.unitCount != expected[i].UnitCount)
			{
				return false;
			}
		}
		return true;
	}

	private static bool CanContainItems(GameGridInventory inventory, int columns, int rows)
	{
		if (((inventory != null) ? inventory.items : null) == null)
		{
			return true;
		}
		for (int i = 0; i < inventory.items.Count; i++)
		{
			GameItem val = inventory.items[i];
			GridShape val2 = ((val != null) ? val.modifiedShape : null) ?? ((val != null) ? val.shape : null);
			if (val2 != null && (val2.minX < 0 || val2.minY < 0 || val2.maxX >= columns || val2.maxY >= rows))
			{
				return false;
			}
		}
		return true;
	}

	private static string CleanId(string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			return id.Replace("\0", string.Empty).Trim();
		}
		return string.Empty;
	}

	private static string WindowSizeKey(GameItem item, string dimension)
	{
		return "XIAOWOTradePerks.ContainerWindow." + CleanId((item != null) ? item.identifier : null) + "." + ((item != null) ? item.uniqueId : 0) + "." + dimension;
	}
}
