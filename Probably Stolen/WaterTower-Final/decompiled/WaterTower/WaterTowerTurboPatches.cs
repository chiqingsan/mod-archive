using System;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;

namespace WaterTower;

internal static class WaterTowerTurboPatches
{
	private static bool _installed;

	public static void Ensure()
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		if (_installed)
		{
			return;
		}
		_installed = true;
		try
		{
			Harmony modHarmony = WaterTowerMod.ModHarmony;
			if (modHarmony == null)
			{
				_installed = false;
				Diag.Once("turbo-no-harmony", "涡轮兼容：拿不到 Harmony 实例，补丁未安装");
				return;
			}
			Type type = WaterTowerItemFactory.ResolveType("ToolDirectory");
			if (type == null)
			{
				Diag.Once("turbo-no-directory", "涡轮兼容：找不到 ToolDirectory，补丁未安装");
				return;
			}
			MethodInfo methodInfo = FindTarget(type, advanced: false);
			MethodInfo methodInfo2 = FindTarget(type, advanced: true);
			if (methodInfo == null && methodInfo2 == null)
			{
				Diag.Once("turbo-no-target", "涡轮兼容：未找到基础/高级涡轮的目标动作（游戏更新？）—— 本次不接管，原版行为不变");
				return;
			}
			MethodInfo method = typeof(WaterTowerTurboPatches).GetMethod("TurboTargetPostfix", BindingFlags.Static | BindingFlags.NonPublic);
			if (method == null)
			{
				Diag.Once("turbo-no-postfix", "涡轮兼容：后处理方法缺失，补丁未安装");
				return;
			}
			HarmonyMethod val = new HarmonyMethod(method);
			if (methodInfo != null)
			{
				modHarmony.Patch((MethodBase)methodInfo, (HarmonyMethod)null, val, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			}
			if (methodInfo2 != null)
			{
				modHarmony.Patch((MethodBase)methodInfo2, (HarmonyMethod)null, val, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			}
			string text = ((methodInfo != null && methodInfo2 != null) ? "基础/高级" : ((methodInfo != null) ? "基础" : "高级"));
			Diag.Once("turbo-hook", "涡轮兼容：已接管" + text + "涡轮对水塔内无容器机器的动作（" + ((methodInfo != null) ? methodInfo.Name : "-") + " / " + ((methodInfo2 != null) ? methodInfo2.Name : "-") + "）");
			if (methodInfo == null || methodInfo2 == null)
			{
				Diag.Once("turbo-hook-partial", "涡轮兼容：" + ((methodInfo == null) ? "基础" : "高级") + "涡轮的目标动作未找到（游戏更新？）—— 该档仍按原版运行");
			}
		}
		catch (Exception ex)
		{
			_installed = false;
			Diag.Exception("turbo-hook-ex", ex, "涡轮兼容：补丁安装失败（原版行为不变）");
		}
	}

	private static MethodInfo FindTarget(Type toolDirectory, bool advanced)
	{
		Type[] nestedTypes = toolDirectory.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic);
		for (int i = 0; i < nestedTypes.Length; i++)
		{
			MethodInfo[] methods = nestedTypes[i].GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.ReturnType != typeof(void))
				{
					continue;
				}
				bool flag = methodInfo.Name.IndexOf("CreateTurboBoosterAdv", StringComparison.Ordinal) >= 0;
				bool flag2 = !flag && methodInfo.Name.IndexOf("CreateTurboBooster", StringComparison.Ordinal) >= 0;
				if (!(advanced ? (!flag) : (!flag2)))
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 2 && parameters[0].ParameterType == typeof(GameItem) && parameters[1].ParameterType == typeof(GameItem))
					{
						return methodInfo;
					}
				}
			}
		}
		return null;
	}

	private static void TurboTargetPostfix(GameItem __0, GameItem __1)
	{
		try
		{
			NightSettlement.RunTurboCycle(__1);
			if (!WaterTowerItemFactory.SameRef(__0, __1))
			{
				NightSettlement.RunTurboCycle(__0);
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("turbo-postfix-ex", ex, "涡轮兼容：后处理异常（已忽略；原版行为不变）");
		}
	}
}
