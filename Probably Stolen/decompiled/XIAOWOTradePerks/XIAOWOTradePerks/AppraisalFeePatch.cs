using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(AppraisalServiceHelper), "GiveAppraisalFee")]
internal static class AppraisalFeePatch
{
	private static void Prefix(out int __state)
	{
		__state = ((PlayerStore.Instance != null) ? PlayerStore.Instance.GetCash() : 0);
	}

	private static void Postfix(int __state)
	{
		PlayerStore instance = PlayerStore.Instance;
		if (instance == null || !PerkRegistry.IsActive("xiaowo_trade_gold_appraiser"))
		{
			return;
		}
		int num = instance.GetCash() - __state;
		if (num > 0)
		{
			int num2 = TradeEffects.AppraisalBonus(num);
			instance.ModCash(num2, false);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Gold appraiser bonus: " + num2 + " (base service fee " + num + ").");
			}
		}
	}
}
