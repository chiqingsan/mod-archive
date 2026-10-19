using System;
using HarmonyLib;

namespace TradeFix;

/// <summary>
/// 加固 NpcJieSuanManager.RemoveItem 的"按 UUID 删除"路径。
///
/// 原版逻辑：count == 1 时按 uid 在 NPC 背包中查找条目并删除；找不到时对空对象取数量，
/// 抛 NullReferenceException，导致"交易助手"等 mod 的购买流程中断（表现为点确认没反应、
/// 只能一件件买；且中断时玩家扣款未执行，交易状态不一致）。
///
/// 本补丁：
///  1) uid 仍能定位到条目 —— 原逻辑照常执行；
///  2) uid 失效但按 itemId 能找到同物品条目（界面缓存与背包数据不同步，常见于后台整理/结算后）
///     —— 把 uid 修正为该条目当前的真实 UUID，原逻辑即可正常删除；
///  3) 彻底找不到 —— 安全跳过，不再抛异常中断交易。
/// </summary>
[HarmonyPatch(typeof(NpcJieSuanManager), "RemoveItem", new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) })]
public static class RemoveItemPatch
{
	public static bool Prefix(int npcId, int itemId, int count, ref string uid)
	{
		if (count != 1)
		{
			return true;   // 买多件时走按物品 ID 删除的路径，不存在此问题
		}
		try
		{
			JSONObject backpack = GetNpcBackpack(npcId);
			if (backpack == null || backpack.list == null)
			{
				Plugin.Log.LogWarning($"[跳过] npc={npcId} 无背包数据，itemId={itemId}（原版此处会抛异常并中断整个购买）");
				return false;
			}
			bool uidFound = false;
			string fallbackUid = null;
			foreach (JSONObject entry in backpack.list)
			{
				JSONObject uidNode = entry["UUID"];
				string entryUid = uidNode != null ? uidNode.Str : null;
				if (entryUid == null)
				{
					continue;
				}
				if (entryUid == uid)
				{
					uidFound = true;
					break;
				}
				if (fallbackUid == null)
				{
					JSONObject idNode = entry["ItemID"];
					JSONObject numNode = entry["Num"];
					if (idNode != null && numNode != null && idNode.I == itemId && numNode.I > 0)
					{
						fallbackUid = entryUid;
					}
				}
			}
			if (uidFound)
			{
				return true;
			}
			if (fallbackUid != null)
			{
				Plugin.Log.LogInfo($"[修正] npc={npcId} itemId={itemId}：交易界面缓存的 uid 已失效，改用背包当前条目的 UUID");
				uid = fallbackUid;
				return true;
			}
			Plugin.Log.LogWarning($"[跳过] npc={npcId} itemId={itemId} uid={uid}：背包中已无此物品（原版此处会抛异常并中断整个购买）");
			return false;
		}
		catch (Exception e)
		{
			Plugin.Log.LogError("[兜底] 删除校验时异常，已安全跳过本次删除：" + e);
			return false;
		}
	}

	private static JSONObject GetNpcBackpack(int npcId)
	{
		JSONObject root = jsonData.instance.AvatarBackpackJsonData;
		if (root == null || !root.HasField(npcId.ToString()))
		{
			return null;
		}
		JSONObject npcNode = root[npcId.ToString()];
		return npcNode != null ? npcNode["Backpack"] : null;
	}
}
