using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace DeepSpacePawnshop.FutureTech;

internal static class FutureIcons
{
	[HarmonyPatch(typeof(GameItemElement), "ResolveSpriteByName")]
	private static class ResolveSpriteByNamePatch
	{
		private static bool Prefix(string name, ref Sprite __result)
		{
			Sprite val = Get(name);
			if (val == null)
			{
				return true;
			}
			__result = val;
			return false;
		}
	}

	[HarmonyPatch(typeof(RenderHandler), "LoadFromAtlas")]
	private static class LoadFromAtlasPatch
	{
		private static bool Prefix(string __0, string __1, ref Sprite __result)
		{
			if (__0 != "Items/deep_space_future_tech")
			{
				return true;
			}
			Sprite val = Get(__1);
			if (val == null)
			{
				return true;
			}
			__result = val;
			return false;
		}
	}

	[HarmonyPatch(typeof(GameItemElement), "ResolveItemSprite")]
	private static class ResolveItemSpritePatch
	{
		private static void Postfix(GameItemElement __instance, ref Sprite __result)
		{
			Sprite val = Get(((GameItem)__instance).identifier);
			if (val != null)
			{
				__result = val;
			}
		}
	}

	private const string ResourcePrefix = "DeepSpacePawnshop.FutureTech.Assets.Icons.";

	private const string ItemAtlas = "Items/deep_space_future_tech";

	private static readonly Dictionary<string, string> Files = new Dictionary<string, string>
	{
		["dspf_future_purifier"] = "purifier.png",
		["dspf_future_charger"] = "charger.png",
		["dspf_future_module"] = "module.png",
		["dspf_future_aging_cabinet"] = "aging-cabinet.png",
		["dspf_future_nuclear_battery"] = "nuclear-battery.png",
		["dspf_future_pocket"] = "dimensional-pocket.png",
		["dspf_future_void_jug"] = "void-jug.png",
		["dspf_future_tech"] = "future-perk.png"
	};

	private static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

	private static readonly HashSet<string> Failed = new HashSet<string>();

	private static Action<string>? _log;

	internal static void Initialize(Action<string> log)
	{
		_log = log;
	}

	internal static Sprite? PerkIcon()
	{
		return Get("dspf_future_tech");
	}

	internal static void ApplyToItem(GameItem item, string id)
	{
		Sprite val = Get(id);
		if (val == null)
		{
			return;
		}
		try
		{
			Dictionary<string, Dictionary<string, Sprite>> val2 = RenderHandler.atlasCache;
			if (val2 == null)
			{
				val2 = (RenderHandler.atlasCache = new Dictionary<string, Dictionary<string, Sprite>>());
			}
			Dictionary<string, Sprite> val4 = default(Dictionary<string, Sprite>);
			if (!val2.TryGetValue("Items/deep_space_future_tech", ref val4) || val4 == null)
			{
				val4 = (val2["Items/deep_space_future_tech"] = new Dictionary<string, Sprite>());
			}
			val4[id] = val;
			item.SetSprite("Items/deep_space_future_tech", id);
		}
		catch (Exception value)
		{
			_log?.Invoke($"Future Tech icon {id} could not be assigned: {value}");
		}
	}

	private static Sprite? Get(string? id)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		if (Sprites.TryGetValue(id, out Sprite value))
		{
			return value;
		}
		if (Failed.Contains(id) || !Files.TryGetValue(id, out string value2))
		{
			return null;
		}
		try
		{
			using Stream stream = typeof(FutureIcons).Assembly.GetManifestResourceStream("DeepSpacePawnshop.FutureTech.Assets.Icons." + value2) ?? throw new InvalidOperationException("Missing embedded icon: " + value2);
			using MemoryStream memoryStream = new MemoryStream();
			stream.CopyTo(memoryStream);
			Texture2D val = new Texture2D(2, 2, (TextureFormat)4, false);
			((Object)val).hideFlags = (HideFlags)61;
			if (!ImageConversion.LoadImage(val, new Il2CppStructArray<byte>(memoryStream.ToArray()), false))
			{
				Object.Destroy((Object)(object)val);
				throw new InvalidOperationException("Could not decode embedded icon: " + value2);
			}
			((Object)val).name = "future-tech-" + id;
			((Texture)val).filterMode = (FilterMode)0;
			((Texture)val).wrapMode = (TextureWrapMode)1;
			Sprite val2 = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0f), 200f);
			((Object)val2).hideFlags = (HideFlags)61;
			((Object)val2).name = ((Object)val).name;
			Sprites.Add(id, val2);
			_log?.Invoke($"Future Tech icon loaded: {value2} ({((Texture)val).width}x{((Texture)val).height}).");
			return val2;
		}
		catch (Exception value3)
		{
			Failed.Add(id);
			_log?.Invoke($"Future Tech icon {id} failed to load: {value3}");
			return null;
		}
	}
}
