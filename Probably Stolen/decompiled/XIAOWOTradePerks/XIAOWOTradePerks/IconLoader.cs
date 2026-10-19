using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace XIAOWOTradePerks;

internal static class IconLoader
{
	private static readonly Dictionary<string, Sprite> Cache = new Dictionary<string, Sprite>(StringComparer.Ordinal);

	internal static Sprite Get(PerkDefinition definition)
	{
		return GetEmbeddedSprite(definition.IconFile, definition.Id, (FilterMode)0);
	}

	internal static Sprite GetEmbeddedSprite(string resourceSuffix, string cacheKey, FilterMode filterMode)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (Cache.TryGetValue(cacheKey, out var value))
		{
			return value;
		}
		Assembly assembly = typeof(Core).Assembly;
		string text = assembly.GetManifestResourceNames().FirstOrDefault((string name) => name.EndsWith(resourceSuffix, StringComparison.OrdinalIgnoreCase));
		if (text == null)
		{
			return null;
		}
		using Stream stream = assembly.GetManifestResourceStream(text);
		if (stream == null)
		{
			return null;
		}
		byte[] array = new byte[stream.Length];
		if (stream.Read(array, 0, array.Length) != array.Length)
		{
			return null;
		}
		Texture2D val = new Texture2D(2, 2, (TextureFormat)4, false, false)
		{
			filterMode = filterMode,
			wrapMode = (TextureWrapMode)1,
			hideFlags = (HideFlags)61
		};
		if (!ImageConversion.LoadImage(val, new Il2CppStructArray<byte>(array), false))
		{
			Object.Destroy((Object)(object)val);
			return null;
		}
		Sprite val2 = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), 100f);
		((Object)val2).hideFlags = (HideFlags)61;
		Cache[cacheKey] = val2;
		return val2;
	}

	internal static void InjectAll()
	{
		Dictionary<string, Sprite> perkIcons = StartingPerkIconLoader.perkIcons;
		if (perkIcons == null)
		{
			Instance log = Core.Log;
			if (log != null)
			{
				log.Warning("Perk icon dictionary was not initialized.");
			}
			return;
		}
		int num = 0;
		PerkDefinition[] all = PerkRegistry.All;
		foreach (PerkDefinition perkDefinition in all)
		{
			Sprite val = Get(perkDefinition);
			if (!((Object)(object)val == (Object)null))
			{
				perkIcons[perkDefinition.Id] = val;
				num++;
			}
		}
		if (num == PerkRegistry.All.Length)
		{
			Instance log2 = Core.Log;
			if (log2 != null)
			{
				log2.Msg("Embedded perk icons injected: " + num + "/" + PerkRegistry.All.Length + ".");
			}
		}
		else
		{
			Instance log3 = Core.Log;
			if (log3 != null)
			{
				log3.Warning("Embedded perk icons injected: " + num + "/" + PerkRegistry.All.Length + ".");
			}
		}
	}
}
