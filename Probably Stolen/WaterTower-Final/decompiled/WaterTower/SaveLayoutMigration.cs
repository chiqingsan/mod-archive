using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;

namespace WaterTower;

public static class SaveLayoutMigration
{
	public static readonly bool ApplyRemap = true;

	private static readonly int[] MapFrom = new int[7] { 4, 5, 6, 7, 8, 9, 10 };

	private static readonly int[] MapTo = new int[7] { 5, 9, 10, 11, 6, 7, 8 };

	private static bool _installed;

	public static void EnsureInstalled()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		if (_installed)
		{
			return;
		}
		try
		{
			Harmony modHarmony = WaterTowerMod.ModHarmony;
			if (modHarmony == null)
			{
				Diag.Once("save-layout-no-harmony", "存档布局迁移：拿不到 Harmony 实例，补丁未安装（读旧存档会错位）");
				return;
			}
			MethodInfo methodInfo = AccessTools.Method(typeof(SaveManager), "DecodeNodes", (Type[])null, (Type[])null);
			MethodInfo methodInfo2 = AccessTools.Method(typeof(SaveLayoutMigration), "DecodeNodesPrefix", (Type[])null, (Type[])null);
			if (methodInfo == null || methodInfo2 == null)
			{
				Diag.Once("save-layout-no-target", "存档布局迁移：SaveManager.DecodeNodes 目标找不到，补丁未安装（读旧存档会错位）");
				return;
			}
			modHarmony.Patch((MethodBase)methodInfo, new HarmonyMethod(methodInfo2), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			_installed = true;
			Diag.Once("save-layout-hook", "存档布局迁移补丁已安装：SaveManager.DecodeNodes 前缀（" + (ApplyRemap ? "**启用改写**" : "干跑模式：只观测、不改写序号") + "）");
		}
		catch (Exception ex)
		{
			Diag.Exception("save-layout-install", ex, "存档布局迁移：补丁安装失败（读旧存档会错位）");
		}
	}

	private static void DecodeNodesPrefix(List<SaveItemNode> savedItems)
	{
		try
		{
			if (savedItems == null)
			{
				return;
			}
			for (int i = 0; i < savedItems.Count; i++)
			{
				SaveItemNode val = savedItems[i];
				if (val != null && !(val.identifier != "watertower"))
				{
					InspectTowerNode(val);
				}
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("save-layout-ex", ex, "存档布局迁移：检查节点时抛异常（已忽略，未改写任何东西）");
		}
	}

	private static void InspectTowerNode(SaveItemNode node)
	{
		string text = node.uuid.ToString();
		List<int> list = ReadIndices(node);
		string reason;
		string text2 = Decide(HasLayoutMarker(node), list, out reason);
		string text3 = DescribeSet(list);
		string text4;
		switch (text2)
		{
		case "remap":
		case "conflict":
		{
			string text5 = DescribeSet(MapIndices(list));
			string text6 = ((ApplyRemap && text2 == "remap") ? ("**已按上表改写 " + list.Count + " 条**（启用模式）") : "**未改写任何东西**（干跑模式）");
			if (text2 == "conflict")
			{
				Diag.Once("save-layout-conflict-" + text, "[守卫报警] 存档布局迁移：水塔节点 uuid=" + text + " 的序号集合 " + text3 + " 自相矛盾（" + reason + "）→ **拒绝迁移**，保持原样；请带着这一行反馈");
			}
			else
			{
				Diag.Summary((ApplyRemap ? "save-layout-remapped-" : "save-layout-would-remap-") + text, "存档布局迁移（" + (ApplyRemap ? "启用" : "干跑") + "）：水塔节点 uuid=" + text + " 序号集合 " + text3 + "（" + list.Count + " 条）→ 判定：需要迁移（" + reason + "）→ 改写后为 " + text5 + "；" + text6);
			}
			if (ApplyRemap && text2 == "remap")
			{
				WriteIndices(node, list);
			}
			return;
		}
		default:
			text4 = "save-layout-skip-" + text;
			break;
		case "empty":
			text4 = "save-layout-empty-" + text;
			break;
		}
		string key = text4;
		if (text2 == "ambiguous")
		{
			Diag.Once("save-layout-ambiguous-" + text, "存档布局迁移：水塔节点 uuid=" + text + " 判不出布局（无标记，且序号集合 " + text3 + " 里没有判据：" + reason + "）→ 按**不迁移**处理，请带着这一行反馈");
		}
		else
		{
			Diag.Once(key, "存档布局迁移：水塔节点 uuid=" + text + " 不需要迁移（" + reason + "；序号集合 " + text3 + "）");
		}
	}

	private static List<int> ReadIndices(SaveItemNode node)
	{
		List<int> list = new List<int>();
		List<int> childItemInventoryNode = node.childItemInventoryNode;
		if (childItemInventoryNode == null)
		{
			return list;
		}
		for (int i = 0; i < childItemInventoryNode.Count; i++)
		{
			list.Add(childItemInventoryNode[i]);
		}
		return list;
	}

	private static List<int> MapIndices(List<int> indices)
	{
		List<int> list = new List<int>(indices.Count);
		foreach (int index in indices)
		{
			list.Add(MapOne(index));
		}
		return list;
	}

	public static int MapOne(int index)
	{
		for (int i = 0; i < MapFrom.Length; i++)
		{
			if (MapFrom[i] == index)
			{
				return MapTo[i];
			}
		}
		return index;
	}

	private static void WriteIndices(SaveItemNode node, List<int> indices)
	{
		List<int> childItemInventoryNode = node.childItemInventoryNode;
		if (childItemInventoryNode != null)
		{
			int num = Math.Min(childItemInventoryNode.Count, indices.Count);
			for (int i = 0; i < num; i++)
			{
				childItemInventoryNode[i] = MapOne(indices[i]);
			}
		}
	}

	private static bool HasLayoutMarker(SaveItemNode node)
	{
		try
		{
			TagSystem itemState = node.itemState;
			if (itemState == null)
			{
				return false;
			}
			if (!WaterTowerItemFactory.TryCallInstance(((object)itemState).GetType(), itemState, "IsTag", new object[1] { "WATERTOWER_LAYOUT2" }, out var result))
			{
				return false;
			}
			return result is bool && (bool)result;
		}
		catch
		{
			return false;
		}
	}

	public static string Decide(bool marker, List<int> indices, out string reason)
	{
		if (indices == null || indices.Count == 0)
		{
			reason = "该节点没有子物品";
			return "empty";
		}
		if (marker)
		{
			reason = "塔本体带布局标记（布局 B 写的存档）";
			return "skip-marked";
		}
		bool flag = indices.Contains(4);
		bool flag2 = indices.Contains(11);
		if (flag && flag2)
		{
			reason = "4 与 11 同时出现";
			return "conflict";
		}
		if (flag)
		{
			reason = "出现序号 4（布局 B 里 4 号是底部布局格、永不装物品）";
			return "remap";
		}
		if (flag2)
		{
			reason = "出现序号 11（旧布局序号最大到 10）";
			return "skip-new";
		}
		reason = "无标记、且 4/11 都不在场";
		return "ambiguous";
	}

	public static string DescribeSet(List<int> indices)
	{
		if (indices == null || indices.Count == 0)
		{
			return "{}";
		}
		List<int> list = new List<int>();
		foreach (int index in indices)
		{
			if (!list.Contains(index))
			{
				list.Add(index);
			}
		}
		list.Sort();
		List<string> list2 = new List<string>(list.Count);
		foreach (int item in list)
		{
			list2.Add(item.ToString());
		}
		return "{" + string.Join(",", list2.ToArray()) + "}";
	}
}
