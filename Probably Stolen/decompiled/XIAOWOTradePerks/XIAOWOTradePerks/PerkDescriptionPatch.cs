using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StartingPerk), "GetLocalizedDescription")]
internal static class PerkDescriptionPatch
{
	private static void Postfix(StartingPerk __instance, ref string __result)
	{
		PerkDefinition perkDefinition = PerkRegistry.Find((__instance != null) ? __instance.id : null);
		if (perkDefinition != null)
		{
			__result = perkDefinition.Description;
		}
	}
}
