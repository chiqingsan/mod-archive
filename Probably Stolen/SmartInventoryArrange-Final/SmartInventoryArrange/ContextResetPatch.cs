using HarmonyLib;

namespace SmartInventoryArrange;

[HarmonyPatch(typeof(ItemContextHandler), "OnEventReset")]
internal static class ContextResetPatch
{
	private static void Postfix()
	{
	}
}
