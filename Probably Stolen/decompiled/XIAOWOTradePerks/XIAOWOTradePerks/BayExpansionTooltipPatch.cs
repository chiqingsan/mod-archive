using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(GameItemElement), "GetTooltipBasic")]
internal static class BayExpansionTooltipPatch
{
	private static void Postfix(GameItemElement __instance, RichTextBuilder __result)
	{
		BayExpansionPerk.AppendUpgradeTooltip((GameItem)(object)__instance, __result);
	}
}
