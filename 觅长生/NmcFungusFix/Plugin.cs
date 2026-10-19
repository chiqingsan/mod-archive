using System;
using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace NmcFungusFix;

/// <summary>
/// 修复 NextMoreCommands 的 OnEnterThreeScene.FungusPatch 在进入砺剑峰/月影寒渊等
/// 三场景时的原生崩溃（Unity 访问违例，进程直接闪退，托管层 try/catch 拦不住）。
///
/// 做法：Harmony Prefix 接管 FungusPatch，用等价但安全的实现替换（见 FungusPatchReplacement）。
/// 不修改工坊原件；未安装 NextMoreCommands 时本 mod 静默跳过，无副作用。
///
/// 加载顺序注意：本 mod（本地Mod测试）在日志里比 NextMoreCommands（创意工坊）先加载，
/// 所以启动时若目标类型还不存在，用轮询协程等待其加载完成再挂补丁。
/// </summary>
[BepInPlugin(Guid, "NextMoreCommands闪退修复", "1.0.0")]
[BepInDependency(NmcGuid, BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
	internal const string Guid = "chiqingsan.mcs.NmcFungusFix";

	private const string NmcGuid = "skyswordkill.plugin.NextMoreCommands";

	private const string NmcTriggerType = "SkySwordKill.NextMoreCommand.DialogTrigger.OnEnterThreeScene";

	internal static ManualLogSource Log;

	private static bool _firstRunLogged;

	public void Start()
	{
		Log = Logger;
		if (!TryApplyPatch())
		{
			StartCoroutine(ApplyWhenReady());
		}
	}

	private IEnumerator ApplyWhenReady()
	{
		float deadline = Time.realtimeSinceStartup + 180f;
		while (Time.realtimeSinceStartup < deadline)
		{
			yield return new WaitForSecondsRealtime(0.5f);
			if (TryApplyPatch())
			{
				yield break;
			}
		}
		Log.LogWarning("等待 NextMoreCommands 加载超时（未安装或加载失败？），本修复未生效");
	}

	/// <summary>返回 true 表示已有明确结果（成功挂上或确定无法挂），不应再重试。</summary>
	private bool TryApplyPatch()
	{
		try
		{
			Type type = AccessTools.TypeByName(NmcTriggerType);
			if (type == null)
			{
				return false;
			}

			MethodInfo method = AccessTools.Method(type, "FungusPatch");
			if (method == null)
			{
				Log.LogWarning("NextMoreCommands 已安装但未找到 FungusPatch 方法（版本不匹配？），本修复未生效");
				return true;
			}

			new Harmony(Guid).Patch(method, prefix: new HarmonyMethod(AccessTools.Method(typeof(FungusPatchReplacement), nameof(FungusPatchReplacement.Prefix))));
			Log.LogInfo("已接管 NextMoreCommands 的 OnEnterThreeScene.FungusPatch：进入砺剑峰/月影寒渊等场景不再因它原生崩溃");
			return true;
		}
		catch (Exception e)
		{
			Log.LogError("修复补丁应用失败（本补丁未生效，游戏行为与原来一致）：" + e);
			return true;
		}
	}

	/// <summary>替代实现每次执行的日志：首次 Info（进场景后可在日志里看到以确认生效），之后 Debug（避免刷屏）。</summary>
	internal static void LogRun(int flowchartCount)
	{
		if (Log == null)
		{
			return;
		}
		if (_firstRunLogged)
		{
			Log.LogDebug($"FungusPatch 安全替代：本次扫描 {flowchartCount} 个 Flowchart");
			return;
		}
		_firstRunLogged = true;
		Log.LogInfo($"FungusPatch 安全替代已首次执行：本次扫描 {flowchartCount} 个 Flowchart");
	}
}
