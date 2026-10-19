using System;
using System.IO;
using Saves;
using UnityEngine;

namespace ModConfig;

public class OptionsManager_Mod : MonoBehaviour
{
	public const string FileName = "ModConfig.ES3";

	private static ES3Settings? _saveSettings;

	private static string Folder => SavesSystem.SavesFolder;

	public static string FilePath => Path.Combine(Folder, "ModConfig.ES3");

	public static ES3Settings SaveSettings
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			if (_saveSettings == null)
			{
				_saveSettings = new ES3Settings(true);
				_saveSettings.path = FilePath;
				_saveSettings.location = (Location)0;
			}
			return _saveSettings;
		}
	}

	public static event Action<string> OnConfigSaved;

	public static void Save<T>(string key, T obj)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		try
		{
			ES3.Save<T>(key, obj, SaveSettings);
			OptionsManager_Mod.OnConfigSaved?.Invoke(key);
			ES3.CreateBackup(SaveSettings);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			Debug.LogError((object)("Error: Failed saving options: " + key));
		}
	}

	public static T Load<T>(string key, T defaultValue = default(T))
	{
		if (string.IsNullOrEmpty(key))
		{
			return default(T);
		}
		try
		{
			if (ES3.KeyExists(key, SaveSettings))
			{
				return ES3.Load<T>(key, SaveSettings);
			}
			ES3.Save<T>(key, defaultValue, SaveSettings);
			return defaultValue;
		}
		catch
		{
			if (ES3.RestoreBackup(SaveSettings))
			{
				try
				{
					if (ES3.KeyExists(key, SaveSettings))
					{
						return ES3.Load<T>(key, SaveSettings);
					}
					ES3.Save<T>(key, defaultValue, SaveSettings);
					return defaultValue;
				}
				catch
				{
					Debug.LogError((object)"[OPTIONS MANAGER] Failed restoring backup");
				}
			}
			ES3.DeleteFile(SaveSettings);
			return defaultValue;
		}
	}
}
