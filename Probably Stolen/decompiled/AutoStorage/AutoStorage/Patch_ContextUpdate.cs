using System;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;

namespace AutoStorage;

[HarmonyPatch]
internal static class Patch_ContextUpdate
{
	private static MethodBase TargetMethod()
	{
		try
		{
			return AccessTools.Method(typeof(ItemContextHandler), "OnEventUpdate", new Type[0], (Type[])null);
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
			MenuButton.OnMenuFrame(__instance);
		}
		catch
		{
		}
	}
}
