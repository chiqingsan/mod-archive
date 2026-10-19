using System;
using System.Collections.Generic;
using HarmonyLib;
using KBEngine;

namespace ZiranWuDao;

/// <summary>
/// 自然悟道主补丁：挂 Avatar.AddTime 的 Prefix，在游戏推进时间、清理过期思绪之前，
/// 为每条仍存活的思绪结算这一段时间自然产生的悟道经验。
///
/// 为什么是 AddTime：全游戏所有时间推进（闭关、睡觉、赶路、炼丹、剧情脚本等 30+ 处）
/// 最终都汇聚到这一个函数，其余地方不会直接改 worldTimeMag；挂这里一次即覆盖全部场景，
/// 且只在玩家推进时间时执行，没有常驻开销。
///
/// 为什么是 Prefix：AddTime 内部会调用 wuDaoMag.AutoReomveLingGuang() 删除过期思绪，
/// 只有推进前才看得到「本次跨月中途到期」的思绪，并正确结算它应得的那几个月。
/// </summary>
[HarmonyPatch(typeof(Avatar), "AddTime")]
public static class ZiranWuDaoPatch
{
	/// <summary>结算进度字段：已自然结算的月数（存于思绪条目内，随存档持久化）。</summary>
	private const string FieldMonths = "zdMonths";

	/// <summary>结算进度字段：已发放的微经验（千分之一经验，存于思绪条目内）。</summary>
	private const string FieldPaid = "zdPaid";

	[HarmonyPrefix]
	public static void Prefix(Avatar __instance, int addday, int addMonth, int Addyear)
	{
		try
		{
			if (!Plugin.Config_Enabled.Value || __instance == null || __instance.wuDaoMag == null)
			{
				return;
			}

			// 只为玩家结算：Avatar.AddTime 目前仅由玩家行为触发，但若将来有 mod 让 NPC 推进时间，
			// 结算其灵光会同时把提示弹给玩家，语义混乱，这里明确挡掉。
			if (!__instance.isPlayer())
			{
				return;
			}

			if (__instance.worldTimeMag == null)
			{
				return;
			}

			DateTime before = __instance.worldTimeMag.getNowTime();
			DateTime after = ZiranWuDaoMath.AdvanceTime(before, addday, addMonth, Addyear);
			if (ZiranWuDaoMath.MonthsBetween(before, after) <= 0)
			{
				return;
			}

			JSONObject lingGuang = __instance.LingGuang;
			if (lingGuang == null || lingGuang.list == null || lingGuang.list.Count == 0)
			{
				return;
			}

			Dictionary<int, int> gains = new Dictionary<int, int>();
			int errorCount = 0;

			foreach (JSONObject entry in lingGuang.list)
			{
				try
				{
					SettleOne(entry, before, after, gains);
				}
				catch (Exception)
				{
					// 单条思绪结算失败不应影响其它思绪与游戏本身，只统计条数避免刷日志。
					errorCount++;
				}
			}

			// 逐个大道发放：某个大道的类型数据异常（如被其它 mod 扩展的类型表有缺字段）
			// 只影响它自己，不阻断其余大道的经验入账。
			foreach (KeyValuePair<int, int> pair in gains)
			{
				try
				{
					__instance.wuDaoMag.addWuDaoEx(pair.Key, pair.Value);
				}
				catch (Exception e)
				{
					Plugin.Log.LogWarning($"[自然悟道] 大道类型 {pair.Key} 的经验发放失败（+{pair.Value}），已跳过：" + e.Message);
				}
			}

			if (gains.Count > 0)
			{
				if (errorCount > 0)
				{
					Plugin.Log.LogWarning($"[自然悟道] {errorCount} 条思绪结算异常，已跳过（多为数据缺失或格式不符，不影响其它思绪）");
				}
				if (Plugin.Config_ShowTip.Value)
				{
					ShowTip(gains);
				}
			}
		}
		catch (Exception e)
		{
			// 补丁自身异常绝不能干扰游戏的时间推进。
			Plugin.Log.LogError("[自然悟道] 结算失败：" + e);
		}
	}

	/// <summary>结算单条思绪，把本次应发放的经验累加到 gains（按大道类型聚合）。</summary>
	private static void SettleOne(JSONObject entry, DateTime before, DateTime after, Dictionary<int, int> gains)
	{
		if (entry == null || !entry.HasField("startTime") || !entry.HasField("studyTime"))
		{
			return;
		}

		int type = GetInt(entry, "type");
		if (type <= 0)
		{
			return;
		}

		int maxMonths = Plugin.Config_MaxSettleMonths.Value;
		int alreadyMonths = GetInt(entry, FieldMonths);
		if (alreadyMonths >= maxMonths)
		{
			// 已达上限：不再解析日期、不再计算，直接跳过（性能优化，也是常见情形）。
			return;
		}

		int totalExp = GetTotalExp(entry);
		if (totalExp <= 0)
		{
			return;
		}

		if (!DateTime.TryParse(entry["startTime"].str, out DateTime startTime))
		{
			// 时间字段损坏的思绪：跳过，不让它拖垮整次结算。
			return;
		}
		DateTime expireTime = startTime.AddDays(GetInt(entry, "guoqiTime"));

		int settleMonths = ZiranWuDaoMath.MonthsToSettle(before, after, startTime, expireTime, alreadyMonths, maxMonths);
		if (settleMonths <= 0)
		{
			return;
		}

		long paidMilli = GetInt(entry, FieldPaid);
		int granted = ZiranWuDaoMath.GrantExp(totalExp, alreadyMonths + settleMonths, Plugin.PerMonthPermille, Plugin.MaxTotalPermille, ref paidMilli);

		// 无论本次是否凑满 1 点经验，都推进进度：月数照记，零头留在 paidMilli 里续存。
		// GrantExp 已把累计值封顶在 int 范围内，可安全写回思绪的整数字段。
		entry.SetField(FieldMonths, alreadyMonths + settleMonths);
		entry.SetField(FieldPaid, (int)paidMilli);

		if (granted <= 0)
		{
			return;
		}

		gains.TryGetValue(type, out int sum);
		gains[type] = sum + granted;
	}

	/// <summary>该条思绪手动感悟可获得的悟道经验总量 = studyTime × 品阶倍率。</summary>
	private static int GetTotalExp(JSONObject entry)
	{
		int studyTime = GetInt(entry, "studyTime");
		int quality = GetInt(entry, "quality");
		JSONObject table = jsonData.instance.WuDaoExBeiLuJson["1"];
		string key = "lingguang" + quality;
		if (table == null || !table.HasField(key))
		{
			return studyTime;
		}
		return (int)((float)studyTime * table[key].n);
	}

	/// <summary>读取整数字段，缺失时返回 0。</summary>
	private static int GetInt(JSONObject entry, string field)
	{
		if (entry == null || !entry.HasField(field))
		{
			return 0;
		}
		return entry[field].I;
	}

	/// <summary>
	/// 弹出结算提示：**每个大道一条**（如「自然悟道：土之道+38」）。
	/// 一条一提示长度短且可预测，从根本上避免气泡内换行；按收益从高到低依次入队，
	/// 由 UIPopTip 自己按 0.8 秒间隔逐条展示。
	/// </summary>
	private static void ShowTip(Dictionary<int, int> gains)
	{
		if ((UnityEngine.Object)UIPopTip.Inst == (UnityEngine.Object)null)
		{
			return;
		}

		List<KeyValuePair<int, int>> ordered = new List<KeyValuePair<int, int>>(gains);
		ordered.Sort((a, b) => b.Value.CompareTo(a.Value));

		foreach (KeyValuePair<int, int> pair in ordered)
		{
			string msg = ZiranWuDaoMath.BuildTip("自然悟道：", GetTypeName(pair.Key), pair.Value);
			UIPopTip.Inst.Pop(msg, PopTipIconType.感悟);
		}
	}

	/// <summary>
	/// 取大道类型的显示名：从类型表读 name1 / name 原始字段，交给纯函数套用显示规则
	/// （见 ZiranWuDaoMath.FormatDaoName，含「不得重复拼之道」与逐级退化）。类型表可被
	/// 前置框架扩展（例如「灵界」mod 追加的 45 灵道），任何缺失都不能把结算带崩。
	/// </summary>
	private static string GetTypeName(int type)
	{
		string name1 = null;
		string name = null;
		try
		{
			JSONObject entry = jsonData.instance.WuDaoAllTypeJson[type.ToString()];
			if (entry != null)
			{
				if (entry.HasField("name1"))
				{
					name1 = Tools.Code64(entry["name1"].str);
				}
				if (entry.HasField("name"))
				{
					name = Tools.Code64(entry["name"].str);
				}
			}
		}
		catch (Exception)
		{
			// 忽略：类型表缺失该类型时由 FormatDaoName 兜底。
		}
		return ZiranWuDaoMath.FormatDaoName(name1, name, type);
	}
}
