using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StolenHelper), "InitStolenItem")]
internal static class SecurityTargetHeatPatch
{
	private static void Postfix(GameItem item)
	{
		if (item == null || !PerkRegistry.IsActive("xiaowo_trade_security_target"))
		{
			return;
		}
		TagState tagReadonly = item.GetTagReadonly("STOLEN_VALUE_INT");
		if (tagReadonly != null)
		{
			int @int = tagReadonly.GetInt();
			if (@int > 0)
			{
				StolenHelper.SetHeat(item, TradeEffects.SecurityTargetHeat(@int));
			}
		}
	}
}
