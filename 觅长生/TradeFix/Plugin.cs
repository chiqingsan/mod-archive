using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace TradeFix;

[BepInPlugin("chiqingsan.mcs.TradeFix", "交易防崩补丁", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	internal static ManualLogSource Log;

	public void Start()
	{
		Log = Logger;
		try
		{
			Harmony.CreateAndPatchAll(typeof(RemoveItemPatch), null);
			Log.LogInfo("交易防崩补丁已加载：NpcJieSuanManager.RemoveItem 的按 UUID 删除路径已加固");
		}
		catch (Exception e)
		{
			Log.LogError("补丁应用失败（游戏代码可能已变动，本补丁未生效）：" + e);
		}
	}
}
