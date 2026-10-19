using System;
using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "WriteSlotFile")]
internal static class OperatingSurchargeSavePatch
{
	private static void Prefix()
	{
		OperatingSurcharge.BeginSave();
	}

	private static Exception Finalizer(Exception __exception)
	{
		OperatingSurcharge.EndSave();
		return __exception;
	}
}
