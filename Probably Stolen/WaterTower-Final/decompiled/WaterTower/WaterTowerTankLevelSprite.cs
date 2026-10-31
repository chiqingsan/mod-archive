using System;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;

namespace WaterTower;

internal static class WaterTowerTankLevelSprite
{
	public static void OnUpdate(GameItem item)
	{
		try
		{
			if (item != null && !(item.identifier != "watertower_tank"))
			{
				if (item.spritePath != "xl")
				{
					item.SetSpriteAndShape("Items/water_container4", "xl");
				}
				GameItemElement val = ((item != null) ? ((Il2CppObjectBase)item).TryCast<GameItemElement>() : null);
				if (val != null)
				{
					val.HandleInnateModSprite();
				}
			}
		}
		catch
		{
		}
	}

	public static void RefreshNow(object tank)
	{
		try
		{
			if (tank != null)
			{
				WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("SpriteHelper"), "UpdateWaterContainerSprite", new object[1] { tank }, "UpdateWaterContainerSprite(蓄水箱)");
			}
		}
		catch (Exception ex)
		{
			Log.Debug("水位贴图刷新失败（忽略）：" + ex.GetType().Name);
		}
	}
}
