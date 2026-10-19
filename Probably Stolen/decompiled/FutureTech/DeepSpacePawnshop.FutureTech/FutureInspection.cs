using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;

namespace DeepSpacePawnshop.FutureTech;

internal static class FutureInspection
{
	[HarmonyPatch(typeof(ContrabandHelper), "RecursiveInspect")]
	private static class RecursiveInspectPatch
	{
		private static bool Prefix(GameItem gameItem)
		{
			return ((gameItem != null) ? gameItem.identifier : null) != "dspf_future_pocket";
		}
	}

	[HarmonyPatch(typeof(ContrabandHelper), "IsBlockSearchContainer")]
	private static class BlockSearchPatch
	{
		private static void Postfix(GameItem gameItem, ref bool __result)
		{
			if (((gameItem != null) ? gameItem.identifier : null) == "dspf_future_pocket")
			{
				__result = true;
			}
		}
	}

	[HarmonyPatch(typeof(ContrabandHelper), "IsConcealedFromInspection")]
	private static class ConcealedPatch
	{
		private static void Postfix(GameItem gameItem, ref bool __result)
		{
			if (!__result && IsInsidePocket(gameItem))
			{
				__result = true;
			}
		}
	}

	private static bool IsInsidePocket(GameItem? item)
	{
		try
		{
			HashSet<IntPtr> hashSet = new HashSet<IntPtr>();
			while (item != null && hashSet.Add(((Il2CppObjectBase)item).Pointer))
			{
				GameInventory parentInventory = item.parentInventory;
				GameItem val = ((parentInventory != null) ? parentInventory.GetParentItem() : null);
				if (val == null)
				{
					return false;
				}
				if (val.identifier == "dspf_future_pocket")
				{
					return true;
				}
				item = val;
			}
		}
		catch
		{
		}
		return false;
	}
}
