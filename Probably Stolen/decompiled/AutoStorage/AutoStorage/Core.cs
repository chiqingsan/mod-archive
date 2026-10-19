using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using UnityEngine;

namespace AutoStorage;

public class Core : MelonMod
{
	public static Instance Log;

	private IntPtr _lastCtx = IntPtr.Zero;

	public override void OnInitializeMelon()
	{
		Log = ((MelonBase)this).LoggerInstance;
		Log.Msg("=========================================================");
		Log.Msg(" AutoStorage 1.5.6  —  Probably Stolen");
		Log.Msg("  Ctrl+R        把 物品栏 / 谈判柜台 里符合 tag 规则的物品自动放进储藏容器");
		Log.Msg("  Ctrl+L        容器总览：列出所有检测到的容器，逐个打开设置（右键失效时用这个）");
		Log.Msg("  右键储藏容器  弹出面板 -> 点「自动入库设置」配置白名单/黑名单 tag");
		Log.Msg("  Ctrl+Shift+O  临时启用/禁用整个功能");
		Log.Msg("=========================================================");
	}

	public override void OnLateInitializeMelon()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		try
		{
			Cfg.Load();
			Harmony val = new Harmony("workbuddy.autostorage");
			MethodInfo methodInfo = AccessTools.Method(typeof(ItemContextHandler), "OnEventInit", new Type[0], (Type[])null);
			if (methodInfo == null)
			{
				Log.Warning("[AutoStorage] 未找到 ItemContextHandler.OnEventInit，右键按钮改用轮询兜底。");
			}
			else
			{
				MethodInfo method = typeof(Patch_ContextMenu).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic);
				val.Patch((MethodBase)methodInfo, (HarmonyMethod)null, new HarmonyMethod(method), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				Log.Msg("[AutoStorage] 已挂钩 ItemContextHandler.OnEventInit。");
			}
			MethodInfo methodInfo2 = AccessTools.Method(typeof(ItemContextHandler), "OnEventUpdate", new Type[0], (Type[])null);
			if (methodInfo2 == null)
			{
				Log.Warning("[AutoStorage] 未找到 ItemContextHandler.OnEventUpdate，右键二次打开可能失效。");
				return;
			}
			MethodInfo method2 = typeof(Patch_ContextUpdate).GetMethod("Postfix", BindingFlags.Static | BindingFlags.NonPublic);
			val.Patch((MethodBase)methodInfo2, (HarmonyMethod)null, new HarmonyMethod(method2), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			Log.Msg("[AutoStorage] 已挂钩 ItemContextHandler.OnEventUpdate。");
		}
		catch (Exception ex)
		{
			Log.Error("[AutoStorage] Harmony 挂钩失败: " + ex);
		}
	}

	public override void OnUpdate()
	{
		try
		{
			if (Input.GetMouseButtonDown(1))
			{
				MenuButton.OnRightClick();
			}
			MenuButton.Tick();
			bool flag = Input.GetKey((KeyCode)306) || Input.GetKey((KeyCode)305);
			bool flag2 = Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303);
			if (flag && !flag2 && Input.GetKeyDown((KeyCode)114))
			{
				AutoStore.Run(verbose: true);
				return;
			}
			if (flag && Input.GetKeyDown((KeyCode)108))
			{
				ContainerList.Toggle();
				return;
			}
			if (flag && flag2 && Input.GetKeyDown((KeyCode)111))
			{
				Cfg.GlobalEnabled = !Cfg.GlobalEnabled;
				Cfg.Save();
				Log.Msg("[AutoStorage] 全局开关 = " + Cfg.GlobalEnabled);
				return;
			}
			ItemContextHandler val = null;
			try
			{
				val = ItemContextHandler.current;
			}
			catch
			{
			}
			if ((Object)(object)val != (Object)null)
			{
				IntPtr intPtr = U.Ptr((Il2CppObjectBase)(object)val);
				if (intPtr != IntPtr.Zero && intPtr != _lastCtx)
				{
					_lastCtx = intPtr;
					MenuButton.Inject(val);
				}
			}
			else
			{
				_lastCtx = IntPtr.Zero;
			}
		}
		catch (Exception ex)
		{
			Log.Error("[AutoStorage] OnUpdate: " + ex);
		}
	}

	private static void Dump()
	{
		try
		{
			List<Container> list = World.Containers(force: true);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[AutoStorage] 诊断 — 容器数=").Append(list.Count);
			string value;
			try
			{
				value = (((Object)(object)CustomUIManager.Instance != (Object)null) ? "可用" : "null");
			}
			catch (Exception ex)
			{
				value = "异常:" + ex.Message;
			}
			stringBuilder.Append("  CustomUIManager=").Append(value).Append("  includeAll=")
				.Append(World.IncludeAll)
				.Append("  globalOn=")
				.Append(Cfg.GlobalEnabled);
			Tags.ResetStats();
			for (int i = 0; i < list.Count; i++)
			{
				Container container = list[i];
				CfgEntry cfgEntry = Cfg.Get(container.Item);
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				Tags.Collect(container.Item, hashSet);
				stringBuilder.Append("\n  #").Append(i).Append("  id=")
					.Append(U.SafeId(container.Item))
					.Append("  uid=")
					.Append(U.SafeUid(container.Item))
					.Append("  name=")
					.Append(U.SafeName(container.Item))
					.Append("  inv=")
					.Append((container.Inv != null) ? "yes" : "NO")
					.Append("  invId=")
					.Append(InvId(container.Inv))
					.Append("  items=")
					.Append(InvCount(container.Inv))
					.Append("  on=")
					.Append(cfgEntry.Enabled)
					.Append("  wl=[")
					.Append(cfgEntry.White)
					.Append("]")
					.Append("  bl=[")
					.Append(cfgEntry.Black)
					.Append("]")
					.Append("  tags=[")
					.Append(U.Join(new List<string>(hashSet).ToArray()))
					.Append("]");
			}
			string report;
			List<GameItem> list2 = World.SourceItems(out report);
			stringBuilder.Append("\n  来源物品=").Append(list2.Count).Append(report);
			stringBuilder.Append("\n  tag 统计 = ").Append(Tags.StatLine());
			for (int j = 0; j < list2.Count && j < 40; j++)
			{
				GameItem val = list2[j];
				HashSet<string> hashSet2 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				Tags.Collect(val, hashSet2);
				int value2 = 0;
				try
				{
					value2 = val.unitCount;
				}
				catch
				{
				}
				stringBuilder.Append("\n    ").Append(U.SafeId(val)).Append("  x")
					.Append(value2)
					.Append(Owned.Is(val) ? "  拥有" : "  未拥有")
					.Append("  tags=[")
					.Append(U.Join(new List<string>(hashSet2).ToArray()))
					.Append("]");
			}
			Log.Msg(stringBuilder.ToString());
		}
		catch (Exception ex2)
		{
			Log.Error("[AutoStorage] Dump: " + ex2);
		}
	}

	private static string InvId(GameInventory inv)
	{
		if (inv == null)
		{
			return "-";
		}
		try
		{
			return inv.identifier ?? "-";
		}
		catch
		{
			return "-";
		}
	}

	private static int InvCount(GameInventory inv)
	{
		if (inv == null)
		{
			return -1;
		}
		try
		{
			return (inv.childItems != null) ? inv.childItems.Count : (-1);
		}
		catch
		{
			return -1;
		}
	}
}
