using HarmonyLib;

namespace SmartInventoryArrange;

[HarmonyPatch(typeof(ItemContextHandler), "TriggerButton")]
internal static class ContextTriggerPatch
{
	private static bool Prefix(ItemContextHandler __instance, RichTextElement button)
	{
		if (!NativeSmartMenu.IsCurrent(__instance))
		{
			return true;
		}
		bool num = NativeSmartMenu.Button != null && ReferenceEquals(button, NativeSmartMenu.Button);
		bool flag = NativeSmartMenu.PreserveButton != null && ReferenceEquals(button, NativeSmartMenu.PreserveButton);
		if (!num && !flag)
		{
			return true;
		}
		GameGridInventory? currentInventory = ContextAccess.CurrentInventory(__instance);
		PixelWindow sortWindow = ContextAccess.SortWindow(__instance);
		if (sortWindow != null)
		{
			sortWindow.Hide();
		}
		if (currentInventory != null)
		{
			SmartArrangeMod.Run(currentInventory, flag);
		}
		return false;
	}
}
