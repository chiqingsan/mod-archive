using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ReplaceThisWithYourModNameSpace;

public static class ModConfigAPI
{
	public static string ModConfigName = "ModConfig";

	private const int ModConfigVersion = 1;

	private static string TAG = $"ModConfig_v{1}";

	private static Type modBehaviourType;

	private static Type optionsManagerType;

	public static bool isInitialized = false;

	private static bool versionChecked = false;

	private static bool isVersionCompatible = false;

	private static bool CheckVersionCompatibility()
	{
		if (versionChecked)
		{
			return isVersionCompatible;
		}
		try
		{
			FieldInfo field = modBehaviourType.GetField("VERSION", BindingFlags.Static | BindingFlags.Public);
			if (field != null && field.FieldType == typeof(int))
			{
				int num = (int)field.GetValue(null);
				isVersionCompatible = num == 1;
				if (!isVersionCompatible)
				{
					Debug.LogError((object)$"[{TAG}] 版本不匹配！API版本: {1}, ModConfig版本: {num}");
					return false;
				}
				Debug.Log((object)$"[{TAG}] 版本检查通过: {1}");
				versionChecked = true;
				return true;
			}
			Debug.LogWarning((object)("[" + TAG + "] 未找到版本信息字段，跳过版本检查"));
			isVersionCompatible = true;
			versionChecked = true;
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[" + TAG + "] 版本检查失败: " + ex.Message));
			isVersionCompatible = false;
			versionChecked = true;
			return false;
		}
	}

	public static bool Initialize()
	{
		try
		{
			if (isInitialized)
			{
				return true;
			}
			modBehaviourType = FindTypeInAssemblies("ModConfig.ModBehaviour");
			if (modBehaviourType == null)
			{
				Debug.LogWarning((object)("[" + TAG + "] ModConfig.ModBehaviour 类型未找到，ModConfig 可能未加载"));
				return false;
			}
			optionsManagerType = FindTypeInAssemblies("ModConfig.OptionsManager_Mod");
			if (optionsManagerType == null)
			{
				Debug.LogWarning((object)("[" + TAG + "] ModConfig.OptionsManager_Mod 类型未找到"));
				return false;
			}
			if (!CheckVersionCompatibility())
			{
				Debug.LogWarning((object)("[" + TAG + "] ModConfig version mismatch!!!"));
				return false;
			}
			string[] array = new string[5] { "AddDropdownList", "AddInputWithSlider", "AddBoolDropdownList", "AddOnOptionsChangedDelegate", "RemoveOnOptionsChangedDelegate" };
			string[] array2 = array;
			foreach (string text in array2)
			{
				MethodInfo method = modBehaviourType.GetMethod(text, BindingFlags.Static | BindingFlags.Public);
				if (method == null)
				{
					Debug.LogError((object)("[" + TAG + "] 必要方法 " + text + " 未找到"));
					return false;
				}
			}
			isInitialized = true;
			Debug.Log((object)("[" + TAG + "] ModConfigAPI 初始化成功"));
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[" + TAG + "] 初始化失败: " + ex.Message));
			return false;
		}
	}

	private static Type FindTypeInAssemblies(string typeName)
	{
		try
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			Assembly[] array = assemblies;
			foreach (Assembly assembly in array)
			{
				try
				{
					if (assembly.FullName.Contains("ModConfig"))
					{
						Debug.Log((object)("[" + TAG + "] 找到 ModConfig 相关程序集: " + assembly.FullName));
					}
					Type type = assembly.GetType(typeName);
					if (type != null)
					{
						Debug.Log((object)("[" + TAG + "] 在程序集 " + assembly.FullName + " 中找到类型 " + typeName));
						return type;
					}
				}
				catch (Exception)
				{
				}
			}
			Debug.LogWarning((object)$"[{TAG}] 在所有程序集中未找到类型 {typeName}，已加载程序集数量: {assemblies.Length}");
			foreach (Assembly item in assemblies.Where((Assembly a) => a.FullName.Contains("ModConfig")))
			{
				Debug.Log((object)("[" + TAG + "] ModConfig 相关程序集: " + item.FullName));
			}
			return null;
		}
		catch (Exception ex2)
		{
			Debug.LogError((object)("[" + TAG + "] 程序集扫描失败: " + ex2.Message));
			return null;
		}
	}

	public static bool SafeAddOnOptionsChangedDelegate(Action<string> action)
	{
		if (!Initialize())
		{
			return false;
		}
		if (action == null)
		{
			Debug.LogWarning((object)("[" + TAG + "] 不能添加空的事件委托"));
			return false;
		}
		try
		{
			MethodInfo method = modBehaviourType.GetMethod("AddOnOptionsChangedDelegate", BindingFlags.Static | BindingFlags.Public);
			method.Invoke(null, new object[1] { action });
			Debug.Log((object)("[" + TAG + "] 成功添加选项变更事件委托"));
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[" + TAG + "] 添加选项变更事件委托失败: " + ex.Message));
			return false;
		}
	}

	public static bool SafeRemoveOnOptionsChangedDelegate(Action<string> action)
	{
		if (!Initialize())
		{
			return false;
		}
		if (action == null)
		{
			Debug.LogWarning((object)("[" + TAG + "] 不能移除空的事件委托"));
			return false;
		}
		try
		{
			MethodInfo method = modBehaviourType.GetMethod("RemoveOnOptionsChangedDelegate", BindingFlags.Static | BindingFlags.Public);
			method.Invoke(null, new object[1] { action });
			Debug.Log((object)("[" + TAG + "] 成功移除选项变更事件委托"));
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[" + TAG + "] 移除选项变更事件委托失败: " + ex.Message));
			return false;
		}
	}

	public static bool SafeAddDropdownList(string modName, string key, string description, SortedDictionary<string, object> options, Type valueType, object defaultValue)
	{
		key = modName + "_" + key;
		if (!Initialize())
		{
			return false;
		}
		try
		{
			MethodInfo method = modBehaviourType.GetMethod("AddDropdownList", BindingFlags.Static | BindingFlags.Public);
			method.Invoke(null, new object[6] { modName, key, description, options, valueType, defaultValue });
			Debug.Log((object)("[" + TAG + "] 成功添加下拉列表: " + modName + "." + key));
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[" + TAG + "] 添加下拉列表失败 " + modName + "." + key + ": " + ex.Message));
			return false;
		}
	}

	public static bool SafeAddInputWithSlider(string modName, string key, string description, Type valueType, object defaultValue, Vector2? sliderRange = null)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		key = modName + "_" + key;
		if (!Initialize())
		{
			return false;
		}
		try
		{
			MethodInfo method = modBehaviourType.GetMethod("AddInputWithSlider", BindingFlags.Static | BindingFlags.Public);
			object[] parameters = ((!sliderRange.HasValue) ? new object[6] { modName, key, description, valueType, defaultValue, null } : new object[6] { modName, key, description, valueType, defaultValue, sliderRange.Value });
			method.Invoke(null, parameters);
			Debug.Log((object)("[" + TAG + "] 成功添加滑条输入框: " + modName + "." + key));
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[" + TAG + "] 添加滑条输入框失败 " + modName + "." + key + ": " + ex.Message));
			return false;
		}
	}

	public static bool SafeAddBoolDropdownList(string modName, string key, string description, bool defaultValue)
	{
		key = modName + "_" + key;
		if (!Initialize())
		{
			return false;
		}
		try
		{
			MethodInfo method = modBehaviourType.GetMethod("AddBoolDropdownList", BindingFlags.Static | BindingFlags.Public);
			method.Invoke(null, new object[4] { modName, key, description, defaultValue });
			Debug.Log((object)("[" + TAG + "] 成功添加布尔下拉列表: " + modName + "." + key));
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[" + TAG + "] 添加布尔下拉列表失败 " + modName + "." + key + ": " + ex.Message));
			return false;
		}
	}

	public static T SafeLoad<T>(string mod_name, string key, T defaultValue = default(T))
	{
		key = mod_name + "_" + key;
		if (!Initialize())
		{
			return defaultValue;
		}
		if (string.IsNullOrEmpty(key))
		{
			Debug.LogWarning((object)("[" + TAG + "] 配置键不能为空"));
			return defaultValue;
		}
		try
		{
			MethodInfo method = optionsManagerType.GetMethod("Load", BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				Debug.LogError((object)("[" + TAG + "] 未找到 OptionsManager_Mod.Load 方法"));
				return defaultValue;
			}
			MethodInfo methodInfo = method.MakeGenericMethod(typeof(T));
			object obj = methodInfo.Invoke(null, new object[2] { key, defaultValue });
			Debug.Log((object)$"[{TAG}] 成功加载配置: {key} = {obj}");
			return (T)obj;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[" + TAG + "] 加载配置失败 " + key + ": " + ex.Message));
			return defaultValue;
		}
	}

	public static bool SafeSave<T>(string mod_name, string key, T value)
	{
		key = mod_name + "_" + key;
		if (!Initialize())
		{
			return false;
		}
		if (string.IsNullOrEmpty(key))
		{
			Debug.LogWarning((object)("[" + TAG + "] 配置键不能为空"));
			return false;
		}
		try
		{
			MethodInfo method = optionsManagerType.GetMethod("Save", BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				Debug.LogError((object)("[" + TAG + "] 未找到 OptionsManager_Mod.Save 方法"));
				return false;
			}
			MethodInfo methodInfo = method.MakeGenericMethod(typeof(T));
			methodInfo.Invoke(null, new object[2] { key, value });
			Debug.Log((object)$"[{TAG}] 成功保存配置: {key} = {value}");
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("[" + TAG + "] 保存配置失败 " + key + ": " + ex.Message));
			return false;
		}
	}

	public static bool IsAvailable()
	{
		return Initialize();
	}

	public static string GetVersionInfo()
	{
		if (!Initialize())
		{
			return "ModConfig 未加载 | ModConfig not loaded";
		}
		try
		{
			FieldInfo field = modBehaviourType.GetField("VERSION", BindingFlags.Static | BindingFlags.Public);
			if (field != null && field.FieldType == typeof(int))
			{
				int num = (int)field.GetValue(null);
				string arg = ((num == 1) ? "兼容" : "不兼容");
				return $"ModConfig v{num} (API v{1}, {arg})";
			}
			PropertyInfo property = modBehaviourType.GetProperty("VERSION", BindingFlags.Static | BindingFlags.Public);
			if (property != null)
			{
				return property.GetValue(null)?.ToString() ?? "未知版本 | Unknown version";
			}
			return "ModConfig 已加载（版本信息不可用） | ModConfig loaded (version info unavailable)";
		}
		catch
		{
			return "ModConfig 已加载（版本检查失败） | ModConfig loaded (version check failed)";
		}
	}

	public static bool IsVersionCompatible()
	{
		if (!Initialize())
		{
			return false;
		}
		return isVersionCompatible;
	}
}
