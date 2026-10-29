using System;
using System.Text.Json;

namespace SmartInventoryArrange;

internal static class WineBottleNative
{
	internal static bool IsWine(GameItem item)
	{
		return StringComparer.Ordinal.Equals(item.identifier, "wine_bottle");
	}

	private static int? ReadInt(GameItem item, string key)
	{
		TagSystem state = item.state;
		Dictionary<string, TagState> val = ((state != null) ? state.dict : null);
		if (val == null || !val.ContainsKey(key))
		{
			return null;
		}
		TagState val2 = val[key];
		if (val2 != null)
		{
			return val2.valueInt;
		}
		return null;
	}

	internal static WineBottleInfo Read(GameItem item, string stableId)
	{
		return new WineBottleInfo(stableId, item.GetDisplayName(true) ?? "", ReadInt(item, "WINE_CONTAINER_STATE"), ReadInt(item, "AGE_INT"));
	}

	internal static string FingerprintSuffix(GameItem item)
	{
		return JsonSerializer.Serialize(Read(item, ""));
	}
}
