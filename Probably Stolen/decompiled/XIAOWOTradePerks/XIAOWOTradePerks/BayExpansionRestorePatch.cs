using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;

namespace XIAOWOTradePerks;

[HarmonyPatch(typeof(GameItem), "ValidateShapeState")]
internal static class BayExpansionRestorePatch
{
	private readonly struct RestoreState
	{
		internal bool Required { get; }

		internal BayExpansionPerk.WindowLayoutSnapshot Layout { get; }

		internal RestoreState(bool required, BayExpansionPerk.WindowLayoutSnapshot layout)
		{
			Required = required;
			Layout = layout;
		}
	}

	[ThreadStatic]
	private static bool _restoring;

	private static void Prefix(GameItem __instance, out RestoreState __state)
	{
		bool flag = !_restoring && BayExpansionPerk.NeedsShapeRestore(__instance);
		__state = (flag ? new RestoreState(required: true, BayExpansionPerk.CaptureWindowLayout(__instance)) : default(RestoreState));
	}

	[HarmonyPriority(0)]
	private static void Postfix(GameItem __instance, RestoreState __state)
	{
		if (_restoring || !__state.Required)
		{
			return;
		}
		try
		{
			_restoring = true;
			BayExpansionPerk.RestoreExpandedItem(__instance);
			BayExpansionPerk.RestoreWindowLayout(__instance, __state.Layout);
		}
		catch (Exception ex)
		{
			Instance log = Core.Log;
			if (log != null)
			{
				log.Error("Failed to restore expanded bay shape: " + ex);
			}
		}
		finally
		{
			_restoring = false;
		}
	}
}
