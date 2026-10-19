using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppSystem;
using UnityEngine;

namespace AutoStorage;

internal static class ContainerList
{
	private const string PanelId = "AutoStorage.list";

	private static readonly List<object> _alive = new List<object>();

	private static Action Act(Action a)
	{
		if (a == null)
		{
			return null;
		}
		Action val = Action.op_Implicit(a);
		_alive.Add(a);
		_alive.Add(val);
		return val;
	}

	private static Action<bool> ActB(Action<bool> a)
	{
		if (a == null)
		{
			return null;
		}
		Action<bool> val = Action<bool>.op_Implicit(a);
		_alive.Add(a);
		_alive.Add(val);
		return val;
	}

	public static void Toggle()
	{
		CustomUIManager val = null;
		bool flag = false;
		try
		{
			val = CustomUIManager.Instance;
			if ((Object)(object)val != (Object)null)
			{
				flag = val.IsOpen("AutoStorage.list");
			}
		}
		catch
		{
		}
		if (flag)
		{
			try
			{
				val.CloseWindow("AutoStorage.list");
				return;
			}
			catch
			{
				return;
			}
		}
		Show();
	}

	public static void Show()
	{
		CustomUIManager val = null;
		try
		{
			val = CustomUIManager.Instance;
		}
		catch
		{
		}
		if ((Object)(object)val == (Object)null)
		{
			Core.Log.Warning("[AutoStorage] CustomUIManager 不可用，无法显示容器总览。");
			return;
		}
		try
		{
			val.CloseWindow("AutoStorage.list");
		}
		catch
		{
		}
		try
		{
			List<Container> list = World.Containers(force: true);
			CustomUIBuilder val2 = val.CreateWindow("AutoStorage.list", "AutoStorage — 容器总览", CustomUIManager.LAYER_OVERLAY);
			val2.SetDraggable(true);
			val2.SetCloseOnEscape(true);
			val2.SetSize(660f, 560f);
			val2.BeginColumn(6f);
			val2.AddLabel("<b>检测到 " + list.Count + " 个储藏容器</b>    (Ctrl+L 关闭)", (string)null);
			val2.BeginRow(6f);
			val2.AddButton("刷新", Act(delegate
			{
				Show();
			}), "refresh");
			val2.AddToggle("包含所有带背包的物品", World.IncludeAll, ActB(delegate(bool v)
			{
				World.IncludeAll = v;
				Show();
			}), "all");
			val2.AddButton("立即整理 (Ctrl+R)", Act(delegate
			{
				AutoStore.Run(verbose: true);
				Show();
			}), "run");
			val2.End();
			if (list.Count == 0)
			{
				val2.AddLabel("没有检测到容器。若游戏里确实有储藏区，按 Ctrl+Shift+D 把诊断写进日志后再看。", (string)null);
			}
			else
			{
				val2.BeginScroll(370f);
				for (int i = 0; i < list.Count; i++)
				{
					GameItem c = list[i].Item;
					CfgEntry cfgEntry = Cfg.Get(c);
					string text = (cfgEntry.Enabled ? "[已启用] " : "[未启用] ") + (cfgEntry.StolenPriority ? "[赃物专用] " : "") + ((!cfgEntry.AllowStolen) ? "[不收赃物] " : "") + U.SafeName(c) + "    id=" + U.SafeId(c) + "    uid=" + U.SafeUid(c);
					val2.BeginRow(4f);
					val2.AddLabel(text, (string)null);
					val2.AddButton("设置", Act(delegate
					{
						SettingsUI.Open(c);
					}), "s" + i);
					val2.AddButton(cfgEntry.Enabled ? "停用" : "启用", Act(delegate
					{
						CfgEntry cfgEntry2 = Cfg.Get(c);
						Cfg.Save(c, !cfgEntry2.Enabled, cfgEntry2.White, cfgEntry2.Black, cfgEntry2.AllowStolen, cfgEntry2.StolenPriority);
						Show();
					}), "t" + i);
					val2.End();
				}
				val2.End();
			}
			val2.AddLabel("白名单留空 = 接收除黑名单外的全部物品；黑名单优先。均支持 id: 前缀匹配。", (string)null);
			val2.End();
			val2.Center();
			val2.Show();
			Core.Log.Msg("[AutoStorage] 容器总览已打开：" + list.Count + " 个。");
		}
		catch (Exception ex)
		{
			Core.Log.Error("[AutoStorage] 容器总览创建失败: " + ex);
		}
	}
}
