using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Collections.Generic;

namespace AutoStorage;

internal static class Cfg
{
	public static bool GlobalEnabled = true;

	private static readonly Dictionary<string, CfgEntry> _mem = new Dictionary<string, CfgEntry>(StringComparer.Ordinal);

	private static bool _loaded;

	private static string FilePath()
	{
		try
		{
			string location = typeof(Cfg).Assembly.Location;
			if (!string.IsNullOrEmpty(location))
			{
				string directoryName = Path.GetDirectoryName(location);
				if (!string.IsNullOrEmpty(directoryName))
				{
					return Path.Combine(directoryName, "AutoStorage.cfg");
				}
			}
		}
		catch
		{
		}
		return "AutoStorage.cfg";
	}

	public static void Load()
	{
		if (_loaded)
		{
			return;
		}
		_loaded = true;
		try
		{
			string text = FilePath();
			if (!File.Exists(text))
			{
				MirrorFromModData();
				return;
			}
			string[] array = File.ReadAllLines(text);
			foreach (string text2 in array)
			{
				string text3 = ((text2 == null) ? "" : text2.Trim());
				if (text3.Length == 0 || text3[0] == '#')
				{
					continue;
				}
				int num = text3.IndexOf('=');
				if (num <= 0)
				{
					continue;
				}
				string text4 = text3.Substring(0, num).Trim();
				string text5 = text3.Substring(num + 1).Trim();
				if (text4 == "global")
				{
					GlobalEnabled = text5 != "0";
					continue;
				}
				if (!_mem.TryGetValue(text4, out var value))
				{
					value = new CfgEntry();
					_mem[text4] = value;
				}
				if (text5.StartsWith("1", StringComparison.Ordinal))
				{
					value.Enabled = true;
				}
				else if (text5.StartsWith("0", StringComparison.Ordinal))
				{
					value.Enabled = false;
				}
				int num2 = text5.IndexOf('|');
				if (num2 < 0)
				{
					continue;
				}
				int num3 = text5.IndexOf('|', num2 + 1);
				if (num3 >= 0)
				{
					value.White = text5.Substring(num2 + 1, num3 - num2 - 1);
					int num4 = text5.IndexOf('|', num3 + 1);
					if (num4 >= 0)
					{
						value.Black = text5.Substring(num3 + 1, num4 - num3 - 1);
						value.ParseFlags(text5.Substring(num4 + 1));
					}
					else
					{
						value.Black = text5.Substring(num3 + 1);
					}
				}
			}
			Core.Log.Msg("[AutoStorage] 已载入配置 (" + _mem.Count + " 个容器) 来自 " + text);
		}
		catch (Exception ex)
		{
			Core.Log.Warning("[AutoStorage] Load: " + ex.Message);
		}
		MirrorFromModData();
	}

	public static void Save()
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("# AutoStorage 1.4.0 — key = <itemIdentifier>#<uniqueId>\n");
			stringBuilder.Append("# value = <enabled>|<whitelist tags>|<blacklist tags>|a<收赃物0/1>p<赃物专用0/1>\n");
			stringBuilder.Append("global=").Append(GlobalEnabled ? "1" : "0").Append('\n');
			foreach (KeyValuePair<string, CfgEntry> item in _mem)
			{
				stringBuilder.Append(item.Key).Append('=').Append(item.Value.Enabled ? "1" : "0")
					.Append('|')
					.Append(item.Value.White ?? "")
					.Append('|')
					.Append(item.Value.Black ?? "")
					.Append('|')
					.Append(item.Value.FlagsSegment())
					.Append('\n');
			}
			File.WriteAllText(FilePath(), stringBuilder.ToString(), Encoding.UTF8);
		}
		catch (Exception ex)
		{
			Core.Log.Warning("[AutoStorage] Save: " + ex.Message);
		}
	}

	private static Dictionary<string, string> ModData(bool create)
	{
		try
		{
			PlayerStore instance = PlayerStore.instance;
			if (instance == null)
			{
				return null;
			}
			Dictionary<string, string> val = instance.modData;
			if (val == null && create)
			{
				val = (instance.modData = new Dictionary<string, string>());
			}
			return val;
		}
		catch
		{
			return null;
		}
	}

	private static void MirrorFromModData()
	{
		try
		{
			Dictionary<string, string> val2 = ModData(create: false);
			if (val2 == null || val2.Count == 0)
			{
				return;
			}
			int added = 0;
			foreachRecoverable((Il2CppObjectBase)(object)val2, delegate(string key, string val)
			{
				if (!_mem.ContainsKey(key))
				{
					CfgEntry cfgEntry = new CfgEntry();
					int num = val.IndexOf('|');
					if (num >= 0)
					{
						int num2 = val.IndexOf('|', num + 1);
						if (num2 >= 0)
						{
							cfgEntry.Enabled = val.StartsWith("1", StringComparison.Ordinal);
							int num3 = val.IndexOf('|', num2 + 1);
							if (num3 >= 0)
							{
								cfgEntry.White = val.Substring(num + 1, num2 - num - 1);
								cfgEntry.Black = val.Substring(num2 + 1, num3 - num2 - 1);
								cfgEntry.ParseFlags(val.Substring(num3 + 1));
							}
							else
							{
								cfgEntry.White = val.Substring(num + 1, num2 - num - 1);
								cfgEntry.Black = val.Substring(num2 + 1);
							}
							_mem[key] = cfgEntry;
							added++;
						}
					}
				}
			});
			if (added > 0)
			{
				Core.Log.Msg("[AutoStorage] modData 中恢复 " + added + " 条容器配置。");
			}
		}
		catch
		{
		}
	}

	private static void MirrorToModData(string key)
	{
		try
		{
			Dictionary<string, string> val = ModData(create: true);
			if (val != null && _mem.TryGetValue(key, out var value))
			{
				val[key] = (value.Enabled ? "1" : "0") + "|" + value.White + "|" + value.Black + "|" + value.FlagsSegment();
			}
		}
		catch
		{
		}
	}

	private static void foreachRecoverable(Il2CppObjectBase dict, Action<string, string> cb)
	{
		if (dict == null || cb == null)
		{
			return;
		}
		try
		{
			Type type = ((object)dict).GetType();
			PropertyInfo property = type.GetProperty("Keys");
			if (property == null)
			{
				return;
			}
			object value = property.GetValue(dict, null);
			if (value == null)
			{
				return;
			}
			Type type2 = value.GetType();
			PropertyInfo property2 = type2.GetProperty("Count");
			if (property2 == null)
			{
				return;
			}
			int num = (int)property2.GetValue(value, null);
			if (num <= 0 || num > 10000)
			{
				return;
			}
			MethodInfo methodInfo = null;
			MethodInfo[] methods = type2.GetMethods();
			foreach (MethodInfo methodInfo2 in methods)
			{
				if (methodInfo2.Name == "CopyTo" && methodInfo2.GetParameters().Length == 2)
				{
					methodInfo = methodInfo2;
					break;
				}
			}
			if (methodInfo == null)
			{
				return;
			}
			Type parameterType = methodInfo.GetParameters()[0].ParameterType;
			object obj;
			if (parameterType == typeof(string[]))
			{
				obj = new string[num];
			}
			else
			{
				try
				{
					obj = Activator.CreateInstance(parameterType, (long)num);
				}
				catch
				{
					return;
				}
			}
			if (obj == null)
			{
				return;
			}
			methodInfo.Invoke(value, new object[2] { obj, 0 });
			IList list = obj as IList;
			for (int j = 0; j < num; j++)
			{
				string text = null;
				if (list != null)
				{
					try
					{
						text = list[j] as string;
					}
					catch
					{
					}
				}
				if (text == null)
				{
					break;
				}
				string text2 = null;
				try
				{
					PropertyInfo property3 = type.GetProperty("Item");
					if (property3 != null)
					{
						text2 = property3.GetValue(dict, new object[1] { text }) as string;
					}
				}
				catch
				{
				}
				cb(text, text2 ?? "");
			}
		}
		catch
		{
		}
	}

	public static string Key(GameItem container)
	{
		string text = U.SafeId(container);
		if (string.IsNullOrEmpty(text))
		{
			text = "container";
		}
		return text + "#" + U.SafeUid(container);
	}

	public static CfgEntry Get(GameItem container)
	{
		if (container == null)
		{
			return new CfgEntry();
		}
		Load();
		string key = Key(container);
		if (_mem.TryGetValue(key, out var value))
		{
			return value.Clone();
		}
		return new CfgEntry();
	}

	public static void Save(GameItem container, bool enabled, string white, string black)
	{
		if (container != null)
		{
			Load();
			if (_mem.TryGetValue(Key(container), out var value))
			{
				Save(container, enabled, white, black, value.AllowStolen, value.StolenPriority);
				return;
			}
		}
		Save(container, enabled, white, black, allowStolen: true, stolenPriority: false);
	}

	public static void Save(GameItem container, bool enabled, string white, string black, bool allowStolen, bool stolenPriority)
	{
		if (container != null)
		{
			Load();
			string text = Key(container);
			_mem[text] = new CfgEntry
			{
				Enabled = enabled,
				White = (white ?? ""),
				Black = (black ?? ""),
				AllowStolen = allowStolen,
				StolenPriority = stolenPriority
			};
			MirrorToModData(text);
			Save();
			Core.Log.Msg("[AutoStorage] 保存 " + text + "  启用=" + enabled + "  白=[" + white + "]  黑=[" + black + "]  收赃物=" + allowStolen + "  赃物专用=" + stolenPriority);
		}
	}

	public static List<string> SplitList(string csv)
	{
		List<string> list = new List<string>();
		if (string.IsNullOrEmpty(csv))
		{
			return list;
		}
		string[] array = csv.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Trim();
			if (text.Length > 0)
			{
				list.Add(text);
			}
		}
		return list;
	}
}
