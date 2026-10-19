using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModConfig;

internal class DropDownOptionsProvider : OptionsProviderBase
{
	private SortedDictionary<string, object>? options = null;

	private string key = "";

	private string description = "";

	private Type valueType;

	private object defaultValue;

	public override string Key => key;

	public override string GetCurrentOption()
	{
		object currentValue = GetCurrentValue();
		if (options != null)
		{
			foreach (KeyValuePair<string, object> option in options)
			{
				if (option.Value.Equals(currentValue))
				{
					return option.Key;
				}
			}
		}
		return options?.First().Key ?? currentValue?.ToString() ?? "null";
	}

	public override string[] GetOptions()
	{
		return options?.Keys.ToArray() ?? Array.Empty<string>();
	}

	public override void Set(int index)
	{
		if (options != null && index >= 0 && index < options.Count)
		{
			object value = options.ElementAt(index).Value;
			SaveValue(value);
			ApplySetting(value);
		}
	}

	private object GetCurrentValue()
	{
		if (valueType == typeof(int))
		{
			return OptionsManager_Mod.Load(((OptionsProviderBase)this).Key, (int)defaultValue);
		}
		if (valueType == typeof(float))
		{
			return OptionsManager_Mod.Load(((OptionsProviderBase)this).Key, (float)defaultValue);
		}
		if (valueType == typeof(string))
		{
			return OptionsManager_Mod.Load(((OptionsProviderBase)this).Key, (string)defaultValue);
		}
		if (valueType == typeof(bool))
		{
			return OptionsManager_Mod.Load(((OptionsProviderBase)this).Key, (bool)defaultValue);
		}
		Debug.LogError((object)$"不支持的配置值类型: {valueType}");
		return defaultValue;
	}

	private void SaveValue(object value)
	{
		if (valueType == typeof(int))
		{
			OptionsManager_Mod.Save(((OptionsProviderBase)this).Key, (int)value);
		}
		else if (valueType == typeof(float))
		{
			OptionsManager_Mod.Save(((OptionsProviderBase)this).Key, (float)value);
		}
		else if (valueType == typeof(string))
		{
			OptionsManager_Mod.Save(((OptionsProviderBase)this).Key, (string)value);
		}
		else if (valueType == typeof(bool))
		{
			OptionsManager_Mod.Save(((OptionsProviderBase)this).Key, (bool)value);
		}
		else
		{
			Debug.LogError((object)$"不支持的配置值类型: {valueType}");
		}
	}

	private void ApplySetting(object value)
	{
		Debug.Log((object)$"应用设置: {((OptionsProviderBase)this).Key} = {value}");
	}

	public int GetCurrentIndex()
	{
		if (options == null || options.Count == 0)
		{
			return 0;
		}
		object currentValue = GetCurrentValue();
		for (int i = 0; i < options.Count; i++)
		{
			if (options.ElementAt(i).Value.Equals(currentValue))
			{
				return i;
			}
		}
		return 0;
	}

	public string GetOptionByValue(object value)
	{
		if (options == null)
		{
			return string.Empty;
		}
		foreach (KeyValuePair<string, object> option in options)
		{
			if (option.Value.Equals(value))
			{
				return option.Key;
			}
		}
		return options.First().Key;
	}

	public void init(string key, string description, SortedDictionary<string, object> options, Type valueType, object defaultValue)
	{
		try
		{
			Debug.Log((object)"=== DropDownOptionsProvider 构造函数开始 ===");
			Debug.Log((object)("参数 - key: " + key + ", description: " + description));
			Debug.Log((object)$"参数 - options: {options != null}, valueType: {valueType}, defaultValue: {defaultValue}");
			this.key = key;
			this.description = description;
			this.options = options;
			this.valueType = valueType;
			this.defaultValue = defaultValue;
			Debug.Log((object)$"字段设置完成 - Key: {this.key}, OptionsCount: {this.options?.Count ?? 0}");
			Debug.Log((object)$"this == null: {(Object)(object)this == (Object)null}");
			Debug.Log((object)"=== DropDownOptionsProvider 构造函数结束 ===");
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("DropDownOptionsProvider 构造函数异常: " + ex.Message + "\n" + ex.StackTrace));
		}
	}
}
