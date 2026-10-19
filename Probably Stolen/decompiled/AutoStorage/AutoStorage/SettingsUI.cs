using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppSystem;
using UnityEngine;

namespace AutoStorage;

internal static class SettingsUI
{
	private static readonly List<object> _alive = new List<object>();

	private static readonly Dictionary<string, string> _draftW = new Dictionary<string, string>(StringComparer.Ordinal);

	private static readonly Dictionary<string, string> _draftB = new Dictionary<string, string>(StringComparer.Ordinal);

	private static readonly Dictionary<string, bool> _draftOn = new Dictionary<string, bool>(StringComparer.Ordinal);

	private static readonly Dictionary<string, bool> _draftAS = new Dictionary<string, bool>(StringComparer.Ordinal);

	private static readonly Dictionary<string, bool> _draftSP = new Dictionary<string, bool>(StringComparer.Ordinal);

	private static readonly Dictionary<string, string> _tagCache = new Dictionary<string, string>(StringComparer.Ordinal);

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

	private static Action<string> ActS(Action<string> a)
	{
		if (a == null)
		{
			return null;
		}
		Action<string> val = Action<string>.op_Implicit(a);
		_alive.Add(a);
		_alive.Add(val);
		return val;
	}

	public static string WindowId(GameItem c)
	{
		return "AutoStorage.cfg." + Cfg.Key(c).Replace('#', '_').Replace('.', '_');
	}

	public static void Close(GameItem c)
	{
		try
		{
			CustomUIManager.Instance.CloseWindow(WindowId(c));
		}
		catch
		{
		}
		string key = Cfg.Key(c);
		_draftOn.Remove(key);
		_draftW.Remove(key);
		_draftB.Remove(key);
		_draftAS.Remove(key);
		_draftSP.Remove(key);
		_tagCache.Remove(key);
	}

	private static void CacheDraft(GameItem container)
	{
		string key = Cfg.Key(container);
		CfgEntry cfgEntry = Cfg.Get(container);
		_draftOn[key] = cfgEntry.Enabled;
		_draftW[key] = cfgEntry.White;
		_draftB[key] = cfgEntry.Black;
		_draftAS[key] = cfgEntry.AllowStolen;
		_draftSP[key] = cfgEntry.StolenPriority;
	}

	public static void Open(GameItem container)
	{
		Open(container, rebuildDraft: true);
	}

	public static void Open(GameItem container, bool rebuildDraft)
	{
		if (container == null)
		{
			return;
		}
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
			Core.Log.Warning("[AutoStorage] CustomUIManager 不可用。");
			return;
		}
		string text = WindowId(container);
		string key = Cfg.Key(container);
		if (rebuildDraft)
		{
			CacheDraft(container);
		}
		try
		{
			if (val.IsOpen(text))
			{
				val.CloseWindow(text);
			}
		}
		catch
		{
		}
		string text2 = U.SafeName(container);
		if (string.IsNullOrEmpty(text2))
		{
			text2 = "容器";
		}
		try
		{
			CustomUIBuilder val2 = val.CreateWindow(text, "自动入库设置 — " + text2, CustomUIManager.LAYER_OVERLAY);
			val2.SetDraggable(true);
			val2.SetCloseOnEscape(true);
			val2.SetSize(620f, 580f);
			val2.BeginColumn(6f);
			val2.AddLabel("<b>" + text2 + "</b>    id=" + U.SafeId(container) + "    uid=" + U.SafeUid(container), (string)null);
			val2.AddToggle("启用自动入库 (Ctrl+R)", _draftOn[key], ActB(delegate(bool v)
			{
				_draftOn[key] = v;
			}), "on");
			val2.AddToggle("整理时接收赃物（带 [赃物(热度)] 标签的物品）", _draftAS[key], ActB(delegate(bool v)
			{
				_draftAS[key] = v;
			}), "as");
			val2.AddToggle("赃物专用容器（最高优先级：赃物无视名单优先放这里）", _draftSP[key], ActB(delegate(bool v)
			{
				_draftSP[key] = v;
			}), "sp");
			val2.AddLabel("白名单 tag（逗号分隔；留空 = 接收所有未被黑名单排除的物品）", (string)null);
			val2.AddInput("例如 food,medical,weapon", _draftW[key], ActS(delegate(string s)
			{
				_draftW[key] = s ?? "";
			}), "wl");
			val2.AddLabel("黑名单 tag（逗号分隔；优先级高于白名单）", (string)null);
			val2.AddInput("例如 contraband,fuel", _draftB[key], ActS(delegate(string s)
			{
				_draftB[key] = s ?? "";
			}), "bl");
			val2.AddLabel("也支持 id: 前缀，按物品内部 id 前缀匹配，例如 id:storage_bay", (string)null);
			val2.BeginRow(6f);
			val2.AddButton("保存", Act(delegate
			{
				Cfg.Save(container, _draftOn[key], _draftW[key], _draftB[key], _draftAS[key], _draftSP[key]);
				Close(container);
			}), "save");
			val2.AddButton("清空白/黑名单", Act(delegate
			{
				_draftW[key] = "";
				_draftB[key] = "";
				Cfg.Save(container, _draftOn[key], "", "", _draftAS[key], _draftSP[key]);
				Open(container, rebuildDraft: false);
			}), "clear");
			val2.AddButton("扫描可用 tag", Act(delegate
			{
				_tagCache[key] = Tags.Scan(container);
				Open(container, rebuildDraft: false);
			}), "scan");
			val2.AddButton("关闭", Act(delegate
			{
				Close(container);
			}), "close");
			val2.End();
			List<Tags.TagRow> rows = Tags.Rows;
			if (_tagCache.ContainsKey(key) && rows.Count > 0)
			{
				val2.AddLabel("已发现的 tag（" + rows.Count + " 条；点 +白 / +黑 按 tag 加入名单，分组行的 +白id / +黑id 按物品 id 加入）：", (string)null);
				val2.AddToggle("只看游戏界面显示的类别 tag（关掉可看全部内部 tag）", Tags.ChineseOnly, ActB(delegate(bool v)
				{
					Tags.ChineseOnly = v;
				}), "cnonly");
				val2.BeginScroll(200f);
				string b = null;
				for (int i = 0; i < rows.Count && i < 400; i++)
				{
					Tags.TagRow tagRow = rows[i];
					if (string.IsNullOrEmpty(tagRow.Key))
					{
						continue;
					}
					string text3 = tagRow.ItemName + "  [" + tagRow.ItemId + "]" + (string.IsNullOrEmpty(tagRow.Note) ? "" : ("  " + tagRow.Note));
					if (!string.Equals(text3, b, StringComparison.Ordinal))
					{
						b = text3;
						if (!string.IsNullOrEmpty(tagRow.ItemId))
						{
							string gid = "id:" + tagRow.ItemId;
							val2.BeginRow(4f);
							val2.AddLabel("<b>" + text3 + "</b>", (string)null);
							val2.AddButton("+白id", Act(delegate
							{
								_draftW[key] = AddPart(_draftW[key], gid);
								Cfg.Save(container, _draftOn[key], _draftW[key], _draftB[key], _draftAS[key], _draftSP[key]);
								Open(container, rebuildDraft: false);
							}), "gw_" + tagRow.ItemId);
							val2.AddButton("+黑id", Act(delegate
							{
								_draftB[key] = AddPart(_draftB[key], gid);
								Cfg.Save(container, _draftOn[key], _draftW[key], _draftB[key], _draftAS[key], _draftSP[key]);
								Open(container, rebuildDraft: false);
							}), "gb_" + tagRow.ItemId);
							val2.End();
						}
						else
						{
							val2.AddLabel("<b>" + text3 + "</b>", (string)null);
						}
					}
					string tt = tagRow.Key;
					val2.BeginRow(4f);
					val2.AddLabel(tagRow.Label, (string)null);
					val2.AddButton("+白", Act(delegate
					{
						_draftW[key] = AddPart(_draftW[key], tt);
						Cfg.Save(container, _draftOn[key], _draftW[key], _draftB[key], _draftAS[key], _draftSP[key]);
						Open(container, rebuildDraft: false);
					}), "w_" + tt);
					val2.AddButton("+黑", Act(delegate
					{
						_draftB[key] = AddPart(_draftB[key], tt);
						Cfg.Save(container, _draftOn[key], _draftW[key], _draftB[key], _draftAS[key], _draftSP[key]);
						Open(container, rebuildDraft: false);
					}), "b_" + tt);
					val2.End();
				}
				val2.End();
			}
			else
			{
				val2.AddLabel("提示：点「扫描可用 tag」可从当前存档的物品上收集 tag 名称。", (string)null);
			}
			val2.End();
			val2.Center();
			val2.Show();
		}
		catch (Exception ex)
		{
			Core.Log.Error("[AutoStorage] 设置窗口创建失败: " + ex);
		}
	}

	private static string AddPart(string csv, string part)
	{
		List<string> list = Cfg.SplitList(csv);
		for (int i = 0; i < list.Count; i++)
		{
			if (string.Equals(list[i], part, StringComparison.OrdinalIgnoreCase))
			{
				return csv;
			}
		}
		list.Add(part);
		return string.Join(",", list.ToArray());
	}
}
