using System;
using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(ScavHelper), "ScavengeDumpingGrounds")]
internal static class NightScavengingContextPatch
{
	[HarmonyPriority(800)]
	private static void Prefix()
	{
		NightScavengingRisk.Begin();
	}

	[HarmonyPriority(0)]
	private static Exception Finalizer(Exception __exception)
	{
		NightScavengingRisk.End();
		return __exception;
	}
}
