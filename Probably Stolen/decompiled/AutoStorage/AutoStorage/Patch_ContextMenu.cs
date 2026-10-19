using System;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;

namespace AutoStorage;

[HarmonyPatch]
internal static class Patch_ContextMenu
{
	private static MethodBase TargetMethod()
	{
		try
		{
			return AccessTools.Method(typeof(ItemContextHandler), "OnEventInit", new Type[0], (Type[])null);
		}
		catch
		{
			return null;
		}
	}

	private static void Postfix(ItemContextHandler __instance)
	{
		try
		{
			MenuButton.Inject(__instance);
		}
		catch (Exception ex)
		{
			Core.Log.Warning("[AutoStorage] OnEventInit: " + ex.Message);
		}
	}
}
