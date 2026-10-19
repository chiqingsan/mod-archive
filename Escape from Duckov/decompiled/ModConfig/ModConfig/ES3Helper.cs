using System;
using System.Collections.Generic;
using System.IO;
using Utilities;

namespace ModConfig;

internal class ES3Helper
{
	private static Dictionary<string, Type> cachedDict = new Dictionary<string, Type>();

	private static ES3Settings SaveSettings => OptionsManager_Mod.SaveSettings;

	public static void DeleteKey(string key)
	{
		ES3.DeleteKey(key, SaveSettings);
	}

	public static Type? getOldTypeOfKeyCached(string key)
	{
		if (cachedDict.ContainsKey(key))
		{
			return cachedDict[key];
		}
		ES3Reader val = ES3Reader.Create(SaveSettings);
		try
		{
			if (val == null)
			{
				throw new FileNotFoundException("File \"" + SaveSettings.FullPath + "\" could not be found.");
			}
			if (!(bool)ReflectionHelper.InvokeInstanceMethod(val, "Goto", new object[1] { key }))
			{
				return null;
			}
			Type type = (Type)ReflectionHelper.InvokeInstanceMethod(val, "ReadKeyPrefix", new object[1] { false });
			cachedDict.Add(key, type);
			return type;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
