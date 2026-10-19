using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(PlayerStore), "DismissCurrentClient")]
internal static class OwnerDealVisitorCompletionPatch
{
	[HarmonyPriority(800)]
	private static void Prefix(PlayerStore __instance, out int __state)
	{
		__state = OwnerDeal.CaptureCurrentVisitor(__instance);
	}

	[HarmonyPriority(0)]
	private static void Postfix(int __state)
	{
		OwnerDeal.MarkVisitorCompleted(__state);
	}
}
