using System;
using HarmonyLib;
using UnityEngine;

namespace SmartInventoryArrange;

[HarmonyPatch(typeof(ItemContextHandler), "OnEventInit")]
internal static class ContextInitPatch
{
	private static void Postfix(ItemContextHandler __instance, bool __result)
	{
		if (!__result || ContextAccess.SortWindow(__instance) == null || ContextAccess.SortSmallEnd(__instance) == null)
		{
			return;
		}
		try
		{
			NativeSmartMenu.Open(__instance);
		}
		catch (Exception ex)
		{
			NativeSmartMenu.Close();
			UnityEngine.Debug.LogError("Smart arrange menu injection failed: " + ex);
		}
	}

	internal static bool IsChinese()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		if (systemLanguage != SystemLanguage.Chinese && systemLanguage != SystemLanguage.ChineseSimplified)
		{
			return systemLanguage == SystemLanguage.ChineseTraditional;
		}
		return true;
	}
}
