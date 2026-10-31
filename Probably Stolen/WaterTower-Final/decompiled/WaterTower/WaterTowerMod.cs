using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

namespace WaterTower;

public class WaterTowerMod : MelonMod
{
	public const string HarmonyId = "com.hungo.watertower";

	private const string PatchGuardKey = "WaterTower.HarmonyPatchGuard";

	private static bool _secondaryLoad;

	public static WaterTowerMod Instance { get; private set; }

	public static Harmony ModHarmony { get; private set; }

	public override void OnInitializeMelon()
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		Instance = this;
		Log.Init();
		Diag.Once("boot", "WaterTower 启动（纯净版）v1.0.8 · 构建 " + BuildStamp());
		Diag.Once("boot-env", "环境：Mods 目录里会被加载的 dll = " + ModsList());
		string text;
		try
		{
			text = typeof(WaterTowerMod).Assembly.Location ?? "?";
		}
		catch
		{
			text = "?";
		}
		object data = AppDomain.CurrentDomain.GetData("WaterTower.HarmonyPatchGuard");
		bool flag = data is string;
		if (flag)
		{
			_secondaryLoad = true;
			Diag.Once("boot-duplicate", "[WaterTower] 检测到本进程已由另一份副本注册补丁（已注册于 " + (data as string) + "，本副本 " + text + "）→ 本副本跳过补丁挂载与每帧逻辑。Mods 目录里应只保留一份 WaterTower 的 dll（不同文件名的副本也会被同时加载）。");
			MelonLogger.Warning("[WaterTower] 同一进程加载了第二份 WaterTower 副本，本副本已停用（见 WaterTower.log 的 boot-duplicate 行）");
			return;
		}
		try
		{
			ModHarmony = new Harmony("com.hungo.watertower");
			ModHarmony.UnpatchSelf();
			ModHarmony.PatchAll(typeof(WaterTowerMod).Assembly);
			AppDomain.CurrentDomain.SetData("WaterTower.HarmonyPatchGuard", text);
			Diag.Once("boot-harmony", "Harmony：ID=com.hungo.watertower · assembly=" + text + " · 本进程此前已注册补丁=" + flag + " → UnpatchSelf + PatchAll 完成");
		}
		catch (Exception ex)
		{
			MelonLogger.Error("[WaterTower] 补丁挂载失败：" + ex);
			Diag.Exception("patchall", ex, "补丁挂载失败");
		}
	}

	private static string BuildStamp()
	{
		try
		{
			return File.GetLastWriteTime(typeof(WaterTowerMod).Assembly.Location).ToString("yyyy-MM-dd HH:mm");
		}
		catch
		{
			return "?";
		}
	}

	private static string ModsList()
	{
		try
		{
			string directoryName = Path.GetDirectoryName(Application.dataPath);
			if (string.IsNullOrEmpty(directoryName))
			{
				return "?";
			}
			string path = Path.Combine(directoryName, "Mods");
			if (!Directory.Exists(path))
			{
				return "?（没有 Mods 目录）";
			}
			string[] files = Directory.GetFiles(path, "*.dll");
			if (files.Length == 0)
			{
				return "（无）";
			}
			for (int i = 0; i < files.Length; i++)
			{
				files[i] = Path.GetFileName(files[i]);
			}
			Array.Sort(files, (IComparer<string>?)StringComparer.OrdinalIgnoreCase);
			return string.Join("、", files) + "（共 " + files.Length + " 个）";
		}
		catch
		{
			return "?";
		}
	}

	public override void OnUpdate()
	{
		if (!_secondaryLoad)
		{
			ZoneTagger.Tick();
		}
	}

	public override void OnDeinitializeMelon()
	{
		Log.Flush();
		Instance = null;
	}
}
