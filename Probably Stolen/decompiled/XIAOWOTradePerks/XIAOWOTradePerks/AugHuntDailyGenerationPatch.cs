using System;
using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StoreClientManager), "GenerateClient")]
internal static class AugHuntDailyGenerationPatch
{
	[HarmonyPriority(800)]
	private static void Prefix(StoreClientManager __instance, out bool __state)
	{
		AugHuntContract.BeforeDailyGeneration(__instance, out __state);
	}

	[HarmonyPriority(0)]
	private static void Postfix(StoreClientManager __instance, bool __state)
	{
		AugHuntContract.AfterDailyGeneration(__instance, __state);
	}

	[HarmonyPriority(0)]
	private static Exception Finalizer(Exception __exception)
	{
		if (__exception != null)
		{
			AugHuntContract.AbortDailyGeneration();
		}
		return __exception;
	}
}
