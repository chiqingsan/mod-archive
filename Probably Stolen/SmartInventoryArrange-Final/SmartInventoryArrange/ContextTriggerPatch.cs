using HarmonyLib;
using UnityEngine;

namespace SmartInventoryArrange;

[HarmonyPatch(typeof(ItemContextHandler), "TriggerButton")]
internal static class ContextTriggerPatch
{
	private static bool Prefix(ItemContextHandler __instance, RichTextElement button)
	{
		bool smart = NativeSmartMenu.Button != null && ReferenceEquals(button, NativeSmartMenu.Button);
		bool preserve = NativeSmartMenu.PreserveButton != null && ReferenceEquals(button, NativeSmartMenu.PreserveButton);
		if (!smart && !preserve)
		{
			return true;
		}
		bool menuCurrent = NativeSmartMenu.IsCurrent(__instance);
		GameGridInventory? currentInventory = ContextAccess.CurrentInventory(__instance);
		Debug.Log("Smart arrange click: button=" + (preserve ? "preserve" : "smart") + ", menuCurrent=" + menuCurrent + ", inventory=" + ((currentInventory != null) ? "yes" : "null") + ", busy=" + SmartArrangeMod.Busy);
		if (!menuCurrent)
		{
			return true;
		}
		PixelWindow sortWindow = ContextAccess.SortWindow(__instance);
		if (sortWindow != null)
		{
			sortWindow.Hide();
		}
		if (currentInventory != null)
		{
			SmartArrangeMod.Run(currentInventory, preserve);
		}
		else
		{
			Debug.LogWarning("Smart arrange click ignored: no current inventory.");
		}
		return false;
	}
}
