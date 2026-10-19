using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StartingPerkElement), "Start")]
internal static class PerkElementIconPatch
{
	private static void Postfix(StartingPerkElement __instance)
	{
		PerkDefinition perkDefinition = PerkRegistry.Find((__instance != null) ? __instance.id : null);
		if (perkDefinition != null && !((Object)(object)__instance.icon == (Object)null))
		{
			Sprite val = IconLoader.Get(perkDefinition);
			if ((Object)(object)val != (Object)null)
			{
				__instance.icon.sprite = val;
			}
		}
	}
}
