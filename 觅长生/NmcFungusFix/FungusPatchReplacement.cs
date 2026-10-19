using System;
using System.Reflection;
using Fungus;
using HarmonyLib;
using UnityEngine;

namespace NmcFungusFix;

/// <summary>
/// NextMoreCommands.OnEnterThreeScene.FungusPatch 的安全替代实现。
///
/// 原实现（1.0.4）在每次进入三场景时：
///   1) 对每个带 PatchTag 的对象调 GetComponentsInChildren&lt;Flowchart&gt;(true) —— 全子树遍历；
///   2) 对 Resources.FindObjectsOfTypeAll&lt;Flowchart&gt;() 的每个元素再调
///      GetComponentsInChildren&lt;Flowchart&gt;(false, 列表) —— 又一次全子树遍历，且列表跨根对象
///      持续累积、重复处理，整体接近 O(n²)。
/// 砺剑峰/月影寒渊这类对象多、层级深的迷宫场景下，这些遍历在 Unity 原生层触发访问违例
/// （Read from 0），且原实现未对 FindObjectsOfTypeAll 可能返回的已销毁对象判空。
///
/// 为什么可以安全去掉遍历：
///   - GetComponentsInChildren 的结果并集 ⊆ FindObjectsOfTypeAll&lt;Flowchart&gt;()（后者本就是
///     内存中全部 Flowchart），逐根遍历纯属重复；
///   - 原代码用「已由 Next 框架处理过的 Flowchart」列表排除重复打补丁，而
///     Main.FPatch.PatchFlowchart 内部已有 FPatchRecord 组件幂等守卫（已打过或无补丁数据直接返回），
///     该排除列表冗余。
/// 因此等价实现 = 遍历全部 Flowchart，判空（跳过已销毁的伪 null 对象）后逐个 PatchFlowchart。
///
/// 反射说明：通过反射取 Next 框架的 PatchFlowchart，不写编译期引用——本 mod 的 dll 与 Next.dll
/// 不在同一目录，避免跨 mod 程序集解析问题；调用点为低频路径（进出场景一次），开销可忽略。
/// </summary>
public static class FungusPatchReplacement
{
	private static MethodInfo _patchFlowchart;

	private static object _patchManager;

	private static bool _resolveWarned;

	/// <summary>返回 false 完全接管原 FungusPatch：执行安全替代实现，不再执行原方法（原方法会原生崩溃）。</summary>
	public static bool Prefix()
	{
		try
		{
			Apply();
		}
		catch (Exception e)
		{
			// 兜底：替代实现自身出错也绝不回落到原实现。
			Plugin.Log.LogError("FungusPatch 安全替代执行失败（已拦截原实现，避免闪退）：" + e);
		}
		return false;
	}

	private static void Apply()
	{
		if (!TryResolve())
		{
			return;
		}

		Flowchart[] flowcharts = Resources.FindObjectsOfTypeAll<Flowchart>();
		int count = 0;
		foreach (Flowchart flowchart in flowcharts)
		{
			// 已销毁对象（伪 null）绝不能进原生调用；原实现缺这一步。
			if (flowchart == null)
			{
				continue;
			}
			try
			{
				// 幂等：已打过补丁（FPatchRecord 存在）或无该 Flowchart 的补丁数据时内部直接返回。
				_patchFlowchart.Invoke(_patchManager, new object[] { flowchart });
				count++;
			}
			catch (Exception)
			{
				// 与原实现一致：单个 Flowchart 打补丁失败不影响其它。
			}
		}
		Plugin.LogRun(count);
	}

	private static bool TryResolve()
	{
		if (_patchFlowchart != null)
		{
			return true;
		}

		// 解析失败不做永久缓存：极端时序下（管理器尚未初始化）下次场景加载时重试；
		// 警告只打一次，避免刷日志。
		try
		{
			Type mainType = AccessTools.TypeByName("SkySwordKill.Next.Main");
			object manager = mainType?.GetProperty("FPatch", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
			MethodInfo method = manager?.GetType().GetMethod("PatchFlowchart", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(Flowchart) }, null);
			if (method != null)
			{
				_patchManager = manager;
				_patchFlowchart = method;
				return true;
			}
		}
		catch (Exception e)
		{
			if (!_resolveWarned)
			{
				_resolveWarned = true;
				Plugin.Log.LogWarning("反射取得 Next 补丁管理器失败：" + e.Message);
			}
			return false;
		}

		if (!_resolveWarned)
		{
			_resolveWarned = true;
			Plugin.Log.LogWarning("未能取得 Next 的 PatchFlowchart（版本不匹配？），跳过补丁应用（不会闪退，但 FPatch 数据不生效）");
		}
		return false;
	}
}
