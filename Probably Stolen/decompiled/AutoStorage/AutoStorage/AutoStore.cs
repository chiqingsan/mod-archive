using System.Collections.Generic;
using System.Text;
using Il2Cpp;

namespace AutoStorage;

internal static class AutoStore
{
	public static bool PickSawFull;

	public static void Run(bool verbose)
	{
		Cfg.Load();
		if (!Cfg.GlobalEnabled)
		{
			Core.Log.Warning("[AutoStorage] 功能已被全局关闭（Ctrl+Shift+O 切换）。");
			return;
		}
		List<Container> list = World.Containers(force: true);
		List<Container> list2 = new List<Container>();
		for (int i = 0; i < list.Count; i++)
		{
			if (Cfg.Get(list[i].Item).Enabled)
			{
				list2.Add(list[i]);
			}
		}
		Core.Log.Msg($"[AutoStorage] 发现 {list.Count} 个储藏容器，其中 {list2.Count} 个已启用自动入库。");
		if (list2.Count == 0)
		{
			Core.Log.Warning("[AutoStorage] 没有启用自动入库的容器。 右键储藏容器 -> 在弹出的面板里点「自动入库设置」 -> 勾选「启用自动入库」 -> 保存。");
			if (list.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder("[AutoStorage] 已识别到的容器：");
				for (int j = 0; j < list.Count && j < 30; j++)
				{
					stringBuilder.Append("\n    ").Append(U.SafeId(list[j].Item)).Append("  uid=")
						.Append(U.SafeUid(list[j].Item))
						.Append("  ")
						.Append(U.SafeName(list[j].Item));
				}
				Core.Log.Msg(stringBuilder.ToString());
			}
			return;
		}
		string report;
		List<GameItem> list3 = World.SourceItems(out report);
		Core.Log.Msg("[AutoStorage] 待整理物品 " + list3.Count + " 件：" + report);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		for (int k = 0; k < list3.Count; k++)
		{
			GameItem val = list3[k];
			if (val == null)
			{
				continue;
			}
			if (World.IsContainerItem(val, out var _))
			{
				num4++;
				continue;
			}
			if (!Owned.Is(val))
			{
				num5++;
				if (verbose)
				{
					Core.Log.Msg("    xx 未拥有，跳过: " + U.SafeName(val) + "  [" + U.SafeId(val) + "]");
				}
				continue;
			}
			bool flag = Stolen.Is(val);
			if (flag)
			{
				Container container = PickStolenPriority(list2, val);
				if (container != null && Mover.Move(val, container.Inv))
				{
					num++;
					if (verbose)
					{
						Core.Log.Msg("    -> " + U.SafeName(val) + "  =>  " + container.Name + "   [赃物专用 " + Stolen.Tooltip(val) + "]");
					}
					continue;
				}
			}
			Container container2 = Pick(list2, val, flag);
			if (container2 == null)
			{
				if (PickSawFull)
				{
					num6++;
				}
				else
				{
					num2++;
				}
			}
			else if (Mover.Move(val, container2.Inv))
			{
				num++;
				if (verbose)
				{
					Core.Log.Msg("    -> " + U.SafeName(val) + "  =>  " + container2.Name + (flag ? ("   [" + Stolen.Tooltip(val) + "]") : ""));
				}
			}
			else
			{
				num3++;
			}
		}
		try
		{
			for (int l = 0; l < list2.Count; l++)
			{
				if (list2[l].Inv != null)
				{
					list2[l].Inv.Validate();
				}
			}
		}
		catch
		{
		}
		Core.Log.Msg($"[AutoStorage] 完成：移动 {num} 件，无匹配容器 {num2} 件，移动失败 {num3} 件，跳过容器 {num4} 个，未拥有跳过 {num5} 件，容器已满留下 {num6} 件。");
	}

	private static Container PickStolenPriority(List<Container> active, GameItem item)
	{
		for (int i = 0; i < active.Count; i++)
		{
			Container container = active[i];
			if (container.Inv != null && Cfg.Get(container.Item).StolenPriority && Mover.CanAccept(container.Inv, item))
			{
				return container;
			}
		}
		return null;
	}

	private static Container Pick(List<Container> active, GameItem item, bool stolen)
	{
		PickSawFull = false;
		for (int i = 0; i < active.Count; i++)
		{
			Container container = active[i];
			if (container.Inv == null)
			{
				continue;
			}
			CfgEntry cfgEntry = Cfg.Get(container.Item);
			if (stolen && !cfgEntry.AllowStolen)
			{
				continue;
			}
			List<string> list = Cfg.SplitList(cfgEntry.Black);
			if ((list.Count <= 0 || !Tags.HasAny(item, list)) && Tags.HasAny(item, Cfg.SplitList(cfgEntry.White)))
			{
				if (Mover.CanAccept(container.Inv, item))
				{
					return container;
				}
				PickSawFull = true;
			}
		}
		return null;
	}
}
