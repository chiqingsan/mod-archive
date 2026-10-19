using HarmonyLib;
using Il2Cpp;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(StartingPerkIconLoader), "Start")]
internal static class IconInjectionPatch
{
	private static void Postfix()
	{
		IconLoader.InjectAll();
	}
}
