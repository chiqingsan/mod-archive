using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using UnityEngine;

namespace AutoStorage;

internal static class MenuButton
{
	private const string PanelId = "AutoStorage.menu";

	private static readonly List<object> _alive = new List<object>();

	private static IntPtr _panelFor = IntPtr.Zero;

	private static float _shownAt = -1000f;

	private static bool _dirty = true;

	private static IntPtr _lastSeenItem = IntPtr.Zero;

	private static float _lastMenuFrame = -1000f;

	private static ItemContextHandler _src;

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

	public static void ClosePanel()
	{
		try
		{
			CustomUIManager.Instance.CloseWindow("AutoStorage.menu");
		}
		catch
		{
		}
		_panelFor = IntPtr.Zero;
	}

	public static void Inject(ItemContextHandler h)
	{
		if (!((Object)(object)h == (Object)null))
		{
			_src = h;
			_dirty = true;
			Core.Log.Msg("[AutoStorage] OnEventInit 触发（handler=" + U.Ptr((Il2CppObjectBase)(object)h) + "）。");
		}
	}

	public static void OnMenuFrame(ItemContextHandler h)
	{
		if (!((Object)(object)h == (Object)null))
		{
			_src = h;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - _lastMenuFrame > 0.5f)
			{
				_dirty = true;
				Core.Log.Msg("[AutoStorage] 检测到右键菜单活动（OnEventUpdate 沿）。");
			}
			_lastMenuFrame = realtimeSinceStartup;
		}
	}

	public static void OnRightClick()
	{
		_dirty = true;
		Core.Log.Msg("[AutoStorage] 检测到鼠标右键按下。");
	}

	public static void Tick()
	{
		ItemContextHandler val = _src;
		if ((Object)(object)val == (Object)null)
		{
			try
			{
				val = ItemContextHandler.current;
			}
			catch
			{
			}
		}
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		GameItem val2 = null;
		try
		{
			val2 = val.currentItem;
		}
		catch
		{
		}
		if (val2 == null)
		{
			return;
		}
		IntPtr intPtr = U.Ptr((Il2CppObjectBase)(object)val2);
		if (intPtr == IntPtr.Zero || (intPtr == _lastSeenItem && !_dirty))
		{
			return;
		}
		bool flag = false;
		try
		{
			PixelWindow contextWindow = val.contextWindow;
			if (contextWindow != null)
			{
				flag = contextWindow.IsVisible();
			}
		}
		catch
		{
		}
		if (!flag)
		{
			return;
		}
		_lastSeenItem = intPtr;
		_dirty = false;
		if (intPtr == _panelFor)
		{
			bool flag2 = false;
			try
			{
				flag2 = CustomUIManager.Instance.IsOpen("AutoStorage.menu");
			}
			catch
			{
			}
			if (flag2 || Time.realtimeSinceStartup - _shownAt < 1f)
			{
				return;
			}
		}
		Handle(val2);
	}

	private static void Handle(GameItem item)
	{
		GameInventory val = null;
		try
		{
			val = (GameInventory)(object)ItemContextHandler.current.currentInventory;
		}
		catch
		{
		}
		GameInventory inv = null;
		GameInventory inv2 = null;
		bool flag = World.IsInventoryItem(item, out inv);
		bool flag2 = World.IsContainerItem(item, out inv2);
		Core.Log.Msg("[AutoStorage] 右键命中 → item='" + U.SafeId(item) + "' 名称=" + U.SafeName(item) + "  currentInventory=" + ((val != null) ? "有" : "无") + "  自带背包=" + flag + "  inventory=" + ((inv != null) ? "有" : "无") + "  判定为储藏容器=" + flag2 + "  (includeAll=" + World.IncludeAll + ")");
		if (!flag2)
		{
			Core.Log.Warning("[AutoStorage] 该物品不算储藏容器，未弹面板。 若要强制允许所有带背包的物品，按 Ctrl+L 打开容器总览并勾选「包含所有带背包的物品」。");
		}
		else
		{
			ShowPanel(item);
		}
	}

	public static void ShowPanel(GameItem container)
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
			Core.Log.Warning("[AutoStorage] CustomUIManager.Instance 为 null，右键面板无法创建。请改用 Ctrl+L 容器总览（若它也打不开，说明本作没有可用的 CustomUI 框架）。");
			return;
		}
		try
		{
			val.CloseWindow("AutoStorage.menu");
		}
		catch
		{
		}
		try
		{
			string text = U.SafeName(container);
			if (string.IsNullOrEmpty(text))
			{
				text = "储藏容器";
			}
			bool enabled = Cfg.Get(container).Enabled;
			CustomUIBuilder obj3 = val.CreateWindow("AutoStorage.menu", "储藏容器 — " + text, CustomUIManager.LAYER_OVERLAY);
			obj3.SetDraggable(true);
			obj3.SetCloseOnEscape(true);
			obj3.SetSize(420f, 210f);
			obj3.BeginColumn(6f);
			obj3.AddLabel("<b>" + text + "</b>  id=" + U.SafeId(container), (string)null);
			obj3.AddToggle("启用自动入库", enabled, ActB(delegate(bool v)
			{
				CfgEntry cfgEntry = Cfg.Get(container);
				Cfg.Save(container, v, cfgEntry.White, cfgEntry.Black);
			}), "on");
			obj3.AddLabel("白名单 = " + ShowList(Cfg.Get(container).White) + "    黑名单 = " + ShowList(Cfg.Get(container).Black), (string)null);
			obj3.BeginRow(6f);
			obj3.AddButton("设置白名单/黑名单 tag", Act(delegate
			{
				SettingsUI.Open(container);
				ClosePanel();
			}), "open");
			obj3.AddButton("立即整理 (Ctrl+R)", Act(delegate
			{
				AutoStore.Run(verbose: true);
				ClosePanel();
			}), "run");
			obj3.AddButton("关闭", Act(delegate
			{
				ClosePanel();
			}), "close");
			obj3.End();
			obj3.End();
			obj3.Center();
			obj3.Show();
			_panelFor = U.Ptr((Il2CppObjectBase)(object)container);
			_shownAt = Time.realtimeSinceStartup;
			Core.Log.Msg("[AutoStorage] 右键面板已显示：'" + U.SafeId(container) + "'");
		}
		catch (Exception ex)
		{
			Core.Log.Warning("[AutoStorage] 右键面板创建失败: " + ex.Message);
		}
	}

	private static string ShowList(string csv)
	{
		if (!string.IsNullOrEmpty(csv))
		{
			return csv;
		}
		return "(空)";
	}
}
