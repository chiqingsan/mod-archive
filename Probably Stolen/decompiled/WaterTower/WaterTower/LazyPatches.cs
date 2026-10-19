using System;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace WaterTower;

internal static class LazyPatches
{
	private static bool _installed;

	internal static int SaveEncodeDepth;

	internal static bool InsideSaveEncode => SaveEncodeDepth > 0;

	public static void Ensure()
	{
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		//IL_01c9: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		if (_installed)
		{
			return;
		}
		_installed = true;
		try
		{
			Harmony val = ((WaterTowerMod.Instance == null) ? null : ((MelonBase)WaterTowerMod.Instance).HarmonyInstance);
			if (val == null)
			{
				_installed = false;
				Diag.Once("lazy-no-harmony", "惰性补丁：拿不到 Harmony 实例，机器 tag 补丁未安装");
				return;
			}
			MethodInfo methodInfo = AccessTools.Method(typeof(DirectoryMaster), "Item", new Type[2]
			{
				typeof(string),
				typeof(bool)
			}, (Type[])null);
			MethodInfo methodInfo2 = AccessTools.Method(typeof(LazyPatches), "ItemPostfix", (Type[])null, (Type[])null);
			if (methodInfo == null || methodInfo2 == null)
			{
				Diag.Once("lazy-no-target", "惰性补丁：DirectoryMaster.Item 目标方法找不到，机器 tag 补丁未安装");
			}
			else
			{
				val.Patch((MethodBase)methodInfo, (HarmonyMethod)null, new HarmonyMethod(methodInfo2), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				Diag.Once("lazy-ok", "惰性补丁已安装：DirectoryMaster.Item（机器分区 tag）+ ButtonElement.OnSelect（取水按钮）");
			}
		}
		catch (Exception ex)
		{
			_installed = false;
			Diag.Exception("lazy-patch-fail", ex, "惰性补丁安装失败");
		}
		try
		{
			Harmony val2 = ((WaterTowerMod.Instance == null) ? null : ((MelonBase)WaterTowerMod.Instance).HarmonyInstance);
			MethodInfo methodInfo3 = AccessTools.Method(typeof(PlayerStore), "EncodeSaveItem", (Type[])null, (Type[])null);
			MethodInfo methodInfo4 = AccessTools.Method(typeof(LazyPatches), "EncodeSaveItemPrefix", (Type[])null, (Type[])null);
			MethodInfo methodInfo5 = AccessTools.Method(typeof(LazyPatches), "EncodeSaveItemPostfix", (Type[])null, (Type[])null);
			if (val2 == null || methodInfo3 == null || methodInfo4 == null || methodInfo5 == null)
			{
				Diag.Once("lazy-no-encode", "存档编码探针：目标找不到（不影响功能，只是少了「存档期现造」的证据行）");
			}
			else
			{
				val2.Patch((MethodBase)methodInfo3, new HarmonyMethod(methodInfo4), new HarmonyMethod(methodInfo5), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
				Diag.Once("lazy-encode", "存档编码探针已安装：PlayerStore.EncodeSaveItem（只观测，不改行为）");
			}
		}
		catch (Exception ex2)
		{
			Diag.Exception("lazy-encode-fail", ex2, "存档编码探针安装失败（不影响其它功能）");
		}
		Diag.Once("button-patch", "取水按钮补丁：ButtonElement.OnSelect postfix 已随 PatchAll 挂载");
	}

	private static void EncodeSaveItemPrefix()
	{
		SaveEncodeDepth++;
	}

	private static void EncodeSaveItemPostfix()
	{
		SaveEncodeDepth = Math.Max(0, SaveEncodeDepth - 1);
	}

	private static void ItemPostfix(GameItem __result)
	{
		ZoneTagger.TagIfMachine(__result);
	}
}
