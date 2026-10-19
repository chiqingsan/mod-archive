using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using UnityEngine;

namespace WaterTower;

public class WaterTowerMod : MelonMod
{
	public static WaterTowerMod Instance { get; private set; }

	public override void OnInitializeMelon()
	{
		Instance = this;
		Log.Init();
		Diag.Once("boot", "WaterTower 启动（纯净版）v1.0.5 · 构建 " + BuildStamp());
		Diag.Once("boot-env", "环境：Mods 目录里会被加载的 dll = " + ModsList());
		try
		{
			((MelonBase)this).HarmonyInstance.PatchAll();
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
		ZoneTagger.Tick();
	}

	public override void OnDeinitializeMelon()
	{
		Log.Flush();
		Instance = null;
	}
}
