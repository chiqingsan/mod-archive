using System;
using HarmonyLib;
using UnityEngine;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(Random), "Range", new Type[]
{
	typeof(float),
	typeof(float)
})]
internal static class NightScavengingRandomPatch
{
	[HarmonyPriority(0)]
	private static void Postfix(float __0, float __1, ref float __result)
	{
		__result = NightScavengingRisk.AdjustNativeWoundRoll(__result, __0, __1);
	}
}
