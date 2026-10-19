using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace ZiranWuDao;

[BepInPlugin("chiqingsan.mcs.ZiranWuDao", "自然悟道", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
	internal static ManualLogSource Log;

	internal static ConfigEntry<bool> Config_Enabled;

	internal static ConfigEntry<bool> Config_ShowTip;

	internal static ConfigEntry<float> Config_PerMonthPercent;

	internal static ConfigEntry<int> Config_MaxSettleMonths;

	internal static ConfigEntry<float> Config_MaxTotalPercent;

	public void Start()
	{
		Log = Logger;
		Config_Enabled = Config.Bind("General", "Enabled", true, "自然悟道（启用：true/禁用：false）");
		Config_ShowTip = Config.Bind("General", "ShowTip", true, "每月自然结算后弹出汇总提示（启用：true/禁用：false）");
		Config_PerMonthPercent = Config.Bind("Balance", "PerMonthPercent", 0.5f, "每月自然结算的比例（%），默认 0.5 表示每个月发放该条思绪总量的 0.5%");
		Config_MaxSettleMonths = Config.Bind("Balance", "MaxSettleMonths", 12, "单条思绪最多自然结算的月数，默认 12（与 PerMonthPercent 一起决定自然结算的总收益）");
		Config_MaxTotalPercent = Config.Bind("Balance", "MaxTotalPercent", 6f, "单条思绪自然结算的累计上限（占该条思绪经验总量的百分比），默认 6；与 MaxSettleMonths 两个上限同时生效，哪个先到就以哪个为准");
		ClampConfig();
		try
		{
			Harmony.CreateAndPatchAll(typeof(ZiranWuDaoPatch), null);
			Log.LogInfo($"自然悟道已加载：每游戏月按每条思绪总量的 {Config_PerMonthPercent.Value}% 结算悟道经验，单条上限 {Config_MaxSettleMonths.Value} 个月 / {Config_MaxTotalPercent.Value}%");
		}
		catch (Exception e)
		{
			Log.LogError("补丁应用失败（游戏代码可能已变动，本补丁未生效）：" + e);
		}
	}

	private static void ClampConfig()
	{
		if (Config_PerMonthPercent.Value < 0f)
		{
			Config_PerMonthPercent.Value = 0f;
		}
		if (Config_MaxSettleMonths.Value < 0)
		{
			Config_MaxSettleMonths.Value = 0;
		}
		if (Config_MaxTotalPercent.Value < 0f)
		{
			Config_MaxTotalPercent.Value = 0f;
		}
	}

	/// <summary>每月的结算比例，换算成千分比整数（0.5% → 5），避免浮点误差。</summary>
	internal static int PerMonthPermille => (int)Math.Round(Config_PerMonthPercent.Value * 10f, MidpointRounding.AwayFromZero);

	/// <summary>累计上限，换算成总量的千分比整数（6% → 60）。</summary>
	internal static int MaxTotalPermille => (int)Math.Round(Config_MaxTotalPercent.Value * 10f, MidpointRounding.AwayFromZero);
}
