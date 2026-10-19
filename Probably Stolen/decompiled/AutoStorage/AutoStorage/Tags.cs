using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace AutoStorage;

internal static class Tags
{
	public sealed class TagRow
	{
		public string ItemName;

		public string ItemId;

		public string Key;

		public string Display;

		public bool On;

		public string Note = "";

		public string Label
		{
			get
			{
				string display = Display;
				if (string.IsNullOrEmpty(display) || string.Equals(display, Key, StringComparison.OrdinalIgnoreCase))
				{
					return Key;
				}
				return display + "  (" + Key + ")";
			}
		}
	}

	public static int StatNoState;

	public static int StatNoDict;

	public static int StatEmptyDict;

	public static int StatOk;

	public static int StatReflFail;

	public static int StatEnabled;

	public static int StatDisabled;

	public static int StatRepaired;

	private static readonly Dictionary<string, string> _itemLabelCache = new Dictionary<string, string>(StringComparer.Ordinal);

	private static readonly Dictionary<string, string> _labelCache = new Dictionary<string, string>(StringComparer.Ordinal);

	private static readonly List<TagRow> _rows = new List<TagRow>();

	public static bool ChineseOnly = true;

	public static GameItem Scope;

	public static List<TagRow> Rows => _rows;

	public static bool Has(GameItem it, string tag)
	{
		if (it == null || string.IsNullOrEmpty(tag))
		{
			return false;
		}
		try
		{
			if (it.IsTag(tag))
			{
				return true;
			}
		}
		catch
		{
		}
		try
		{
			List<string> itemTypes = it.itemTypes;
			if (itemTypes != null)
			{
				int count = itemTypes.Count;
				if (count > 0 && count < 64)
				{
					for (int i = 0; i < count; i++)
					{
						if (string.Equals(itemTypes[i], tag, StringComparison.OrdinalIgnoreCase))
						{
							return true;
						}
					}
				}
			}
		}
		catch
		{
		}
		return false;
	}

	public static bool HasAny(GameItem it, List<string> list)
	{
		if (it == null || list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			if (text.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
			{
				string text2 = text.Substring(3).Trim();
				string text3 = U.SafeId(it);
				if (text3.Length > 0 && text2.Length > 0 && text3.StartsWith(text2, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			else if (Has(it, text))
			{
				return true;
			}
		}
		return false;
	}

	public static void ResetStats()
	{
		StatNoState = (StatNoDict = (StatEmptyDict = (StatOk = (StatReflFail = 0))));
		StatEnabled = (StatDisabled = (StatRepaired = 0));
	}

	public static string StatLine()
	{
		return "state==null:" + StatNoState + "  dict==null:" + StatNoDict + "  dict空:" + StatEmptyDict + "  读到tag:" + StatOk + "  反射失败:" + StatReflFail + "  启用:" + StatEnabled + "  禁用:" + StatDisabled + "  修复:" + StatRepaired;
	}

	private static bool TryTyped(GameItem it, HashSet<string> outTags)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		try
		{
			TagSystem state = it.state;
			if (state == null)
			{
				StatNoState++;
				return true;
			}
			Dictionary<string, TagState> dict = state.dict;
			if (dict == null)
			{
				StatNoDict++;
				return true;
			}
			KeyCollection<string, TagState> keys = dict.Keys;
			if (keys == null)
			{
				StatNoDict++;
				return true;
			}
			int count = keys.Count;
			if (count <= 0)
			{
				StatEmptyDict++;
				return true;
			}
			if (count > 4096)
			{
				StatEmptyDict++;
				return true;
			}
			Il2CppStringArray val = new Il2CppStringArray((long)count);
			keys.CopyTo((Il2CppArrayBase<string>)(object)val, 0);
			for (int i = 0; i < count; i++)
			{
				string text = ((Il2CppArrayBase<string>)(object)val)[i];
				if (!string.IsNullOrEmpty(text))
				{
					if (!IsTagEnabled(dict, text, out var enabled))
					{
						StatReflFail++;
						continue;
					}
					if (!enabled)
					{
						StatDisabled++;
						continue;
					}
					StatEnabled++;
					outTags.Add(text);
				}
			}
			StatOk++;
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static string Display(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return "";
		}
		string text = TypeLabel(key);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		text = ItemNameLabel(key);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return key;
	}

	public static string DisplayBoth(string key)
	{
		string text = Display(key);
		if (string.IsNullOrEmpty(text) || string.Equals(text, key, StringComparison.OrdinalIgnoreCase))
		{
			return key;
		}
		return text + " (" + key + ")";
	}

	public static string DisplayFor(GameItem it, string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return "";
		}
		return TypeLabel(key);
	}

	public static string TypeLabel(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return "";
		}
		if (_labelCache.TryGetValue(key, out var value))
		{
			return value;
		}
		string text = "";
		try
		{
			string text2 = "type_" + key.ToLowerInvariant();
			string localizedMechanic = LocHelper.GetLocalizedMechanic(text2, (Il2CppReferenceArray<Object>)null);
			if (!string.IsNullOrEmpty(localizedMechanic) && !string.Equals(localizedMechanic, text2, StringComparison.OrdinalIgnoreCase) && !localizedMechanic.StartsWith("Translation Error", StringComparison.OrdinalIgnoreCase))
			{
				text = localizedMechanic.Trim();
			}
		}
		catch
		{
		}
		if (text.Length > 0)
		{
			_labelCache[key] = text;
		}
		return text;
	}

	public static string ItemNameLabel(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return "";
		}
		if (key.IndexOf(' ') >= 0)
		{
			return "";
		}
		if (_itemLabelCache.TryGetValue(key, out var value))
		{
			return value;
		}
		string text = "";
		try
		{
			string text2 = "item_" + key + "_name";
			string localizedItem = LocHelper.GetLocalizedItem(text2, (Il2CppReferenceArray<Object>)null);
			if (!string.IsNullOrEmpty(localizedItem) && !string.Equals(localizedItem, text2, StringComparison.OrdinalIgnoreCase) && !localizedItem.StartsWith("Translation Error", StringComparison.OrdinalIgnoreCase))
			{
				text = localizedItem.Trim();
			}
		}
		catch
		{
		}
		if (text.Length > 0)
		{
			_itemLabelCache[key] = text;
		}
		return text;
	}

	public static void ClearLabelCache()
	{
		_labelCache.Clear();
		_itemLabelCache.Clear();
	}

	public static string RenderTypeTooltip(GameItem it)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		if (it == null)
		{
			return null;
		}
		try
		{
			RichTextBuilder val = new RichTextBuilder(200);
			CommonTooltipHelper.CreateItemTypeTooltip(val, it);
			RichText val2 = val.Build(-1);
			if (val2 == null)
			{
				return null;
			}
			return val2.nakedText;
		}
		catch (Exception ex)
		{
			Core.Log.Warning("[AutoStorage] RenderTypeTooltip: " + ex.Message);
			return null;
		}
	}

	private static bool IsTagEnabled(Dictionary<string, TagState> d, string key, out bool enabled)
	{
		enabled = false;
		if (d == null || string.IsNullOrEmpty(key))
		{
			return true;
		}
		try
		{
			TagState val = default(TagState);
			if (!d.TryGetValue(key, ref val) || val == null)
			{
				return true;
			}
			enabled = val.valueEnabled;
			return true;
		}
		catch
		{
		}
		try
		{
			TagState val2 = d[key];
			if (val2 != null)
			{
				enabled = val2.valueEnabled;
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	public static string Scan(GameItem container)
	{
		_rows.Clear();
		ResetStats();
		ClearLabelCache();
		Scope = container;
		List<GameItem> list = World.ItemsIn(container);
		for (int i = 0; i < list.Count; i++)
		{
			ScanItem(list[i]);
			DumpTags(list[i]);
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		StringBuilder stringBuilder = new StringBuilder();
		string b = null;
		for (int j = 0; j < _rows.Count; j++)
		{
			TagRow tagRow = _rows[j];
			if (!string.IsNullOrEmpty(tagRow.Key))
			{
				if (!dictionary.ContainsKey(tagRow.Key))
				{
					dictionary[tagRow.Key] = tagRow.Display;
				}
				if (!string.Equals(tagRow.ItemName, b, StringComparison.Ordinal))
				{
					stringBuilder.Append("\n    ").Append(tagRow.ItemName).Append(" [")
						.Append(tagRow.ItemId)
						.Append("]")
						.Append(string.IsNullOrEmpty(tagRow.Note) ? "" : ("  " + tagRow.Note));
					b = tagRow.ItemName;
				}
				stringBuilder.Append("\n        ").Append(tagRow.Label);
			}
		}
		List<string> list2 = new List<string>(dictionary.Keys);
		list2.Sort(StringComparer.OrdinalIgnoreCase);
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int k = 0; k < list2.Count; k++)
		{
			string text = dictionary[list2[k]];
			if (string.IsNullOrEmpty(text) || string.Equals(text, list2[k], StringComparison.OrdinalIgnoreCase))
			{
				stringBuilder2.Append(list2[k]);
			}
			else
			{
				stringBuilder2.Append(text).Append("|").Append(list2[k]);
			}
			if (k < list2.Count - 1)
			{
				stringBuilder2.Append(",");
			}
		}
		Core.Log.Msg("[AutoStorage] 扫描容器 '" + U.SafeName(container) + "' 内 " + list.Count + " 个物品 → " + _rows.Count + " 条记录 / " + list2.Count + " 个不同 tag   [" + StatLine() + "]");
		if (stringBuilder.Length > 0)
		{
			Core.Log.Msg("[AutoStorage] 逐物品 tag:" + stringBuilder);
		}
		return stringBuilder2.ToString();
	}

	public static string Scan()
	{
		_rows.Clear();
		ResetStats();
		Scope = null;
		List<Container> list = World.Containers(force: true);
		for (int i = 0; i < list.Count; i++)
		{
			ScanItem(list[i].Item);
		}
		try
		{
			EmporiumEntry instance = EmporiumEntry.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				ScanList(instance.GetAllItems());
				ScanList(instance.GetAllOwnedItems(false, true));
			}
		}
		catch
		{
		}
		try
		{
			PlayerStore instance2 = PlayerStore.instance;
			if (instance2 != null)
			{
				ScanList(instance2.FindAllItem(true));
			}
		}
		catch
		{
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		StringBuilder stringBuilder = new StringBuilder();
		string b = null;
		for (int j = 0; j < _rows.Count; j++)
		{
			TagRow tagRow = _rows[j];
			if (!string.IsNullOrEmpty(tagRow.Key))
			{
				if (!dictionary.ContainsKey(tagRow.Key))
				{
					dictionary[tagRow.Key] = tagRow.Display;
				}
				if (!string.Equals(tagRow.ItemName, b, StringComparison.Ordinal))
				{
					stringBuilder.Append("\n    ").Append(tagRow.ItemName).Append(" [")
						.Append(tagRow.ItemId)
						.Append("]")
						.Append(string.IsNullOrEmpty(tagRow.Note) ? "" : ("  " + tagRow.Note));
					b = tagRow.ItemName;
				}
				stringBuilder.Append("\n        ").Append(tagRow.Label);
			}
		}
		List<string> list2 = new List<string>(dictionary.Keys);
		list2.Sort(StringComparer.OrdinalIgnoreCase);
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int k = 0; k < list2.Count; k++)
		{
			string text = dictionary[list2[k]];
			if (string.IsNullOrEmpty(text) || string.Equals(text, list2[k], StringComparison.OrdinalIgnoreCase))
			{
				stringBuilder2.Append(list2[k]);
			}
			else
			{
				stringBuilder2.Append(text).Append("|").Append(list2[k]);
			}
			if (k < list2.Count - 1)
			{
				stringBuilder2.Append(",");
			}
		}
		Core.Log.Msg("[AutoStorage] 全存档扫描 → " + _rows.Count + " 条 tag 记录 / " + list2.Count + " 个不同 tag   [" + StatLine() + "]");
		if (stringBuilder.Length > 0)
		{
			Core.Log.Msg("[AutoStorage] 逐物品 tag:" + stringBuilder);
		}
		return stringBuilder2.ToString();
	}

	private static void ScanList(List<GameItem> items)
	{
		if (items == null)
		{
			return;
		}
		int num = 0;
		try
		{
			num = items.Count;
		}
		catch
		{
		}
		if (num <= 0 || num > 4096)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			GameItem val = null;
			try
			{
				val = items[i];
			}
			catch
			{
			}
			if (val != null)
			{
				ScanItem(val);
			}
		}
	}

	private static int RepairItem(GameItem it)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		try
		{
			TagSystem state = it.state;
			if (state == null)
			{
				return 0;
			}
			Dictionary<string, TagState> dict = state.dict;
			if (dict == null)
			{
				return 0;
			}
			KeyCollection<string, TagState> keys = dict.Keys;
			if (keys == null)
			{
				return 0;
			}
			int count = keys.Count;
			if (count <= 0 || count > 128)
			{
				return 0;
			}
			Il2CppStringArray val = new Il2CppStringArray((long)count);
			keys.CopyTo((Il2CppArrayBase<string>)(object)val, 0);
			bool flag = false;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				string text = ((Il2CppArrayBase<string>)(object)val)[i];
				if (!string.IsNullOrEmpty(text))
				{
					if (!IsTagEnabled(dict, text, out var enabled))
					{
						return 0;
					}
					if (enabled)
					{
						num++;
					}
					else if (!string.IsNullOrEmpty(TypeLabel(text)))
					{
						flag = true;
					}
				}
			}
			if (!flag || num >= count)
			{
				return 0;
			}
			int num2 = 0;
			for (int j = 0; j < count; j++)
			{
				string text2 = ((Il2CppArrayBase<string>)(object)val)[j];
				if (string.IsNullOrEmpty(text2))
				{
					continue;
				}
				TagState val2 = null;
				try
				{
					dict.TryGetValue(text2, ref val2);
				}
				catch
				{
				}
				if (val2 == null)
				{
					continue;
				}
				bool flag2 = false;
				try
				{
					flag2 = val2.valueEnabled;
				}
				catch
				{
				}
				if (!flag2)
				{
					try
					{
						val2.valueEnabled = true;
						num2++;
					}
					catch
					{
					}
				}
			}
			return num2;
		}
		catch
		{
			return 0;
		}
	}

	private static void ScanItem(GameItem it)
	{
		if (it == null)
		{
			return;
		}
		int num = RepairItem(it);
		if (num > 0)
		{
			StatRepaired += num;
			Core.Log.Warning("[AutoStorage] 修复 '" + U.SafeName(it) + "'：重新启用 " + num + " 个被旧版本扫描禁用的 tag");
		}
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		Collect(it, hashSet);
		if (hashSet.Count == 0)
		{
			return;
		}
		string text = "";
		try
		{
			text = it.GetDisplayName(false);
		}
		catch
		{
		}
		if (string.IsNullOrEmpty(text))
		{
			try
			{
				text = U.SafeName(it);
			}
			catch
			{
			}
		}
		string itemId = U.SafeId(it);
		string note = (Stolen.Is(it) ? Stolen.Tooltip(it) : "");
		List<string> list = new List<string>(hashSet);
		list.Sort(StringComparer.OrdinalIgnoreCase);
		for (int i = 0; i < list.Count; i++)
		{
			string text2 = list[i];
			string text3 = DisplayFor(it, text2);
			bool flag = !string.IsNullOrEmpty(text3) && !string.Equals(text3, text2, StringComparison.OrdinalIgnoreCase);
			if (!ChineseOnly || flag)
			{
				_rows.Add(new TagRow
				{
					ItemName = text,
					ItemId = itemId,
					Key = text2,
					Display = Display(text2),
					On = true,
					Note = note
				});
			}
		}
	}

	public static string ScanTags()
	{
		return Scan();
	}

	public static void DumpTags(GameItem it)
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		if (it == null)
		{
			Core.Log.Msg("[AutoStorage] DumpTags: item == null");
			return;
		}
		string value = "";
		try
		{
			value = it.GetDisplayName(false);
		}
		catch
		{
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("\n  === ").Append(value).Append("  [")
			.Append(U.SafeId(it))
			.Append("] ===");
		try
		{
			string value2 = RenderTypeTooltip(it);
			stringBuilder.Append("\n  tooltip.nakedText = '").Append(value2).Append("'");
		}
		catch (Exception ex)
		{
			stringBuilder.Append("\n  tooltip 异常: ").Append(ex.Message);
		}
		try
		{
			TagSystem state = it.state;
			if (state == null)
			{
				Core.Log.Msg("[AutoStorage] DumpTags: state == null");
				return;
			}
			Dictionary<string, TagState> dict = state.dict;
			if (dict == null)
			{
				Core.Log.Msg("[AutoStorage] DumpTags: dict == null");
				return;
			}
			KeyCollection<string, TagState> keys = dict.Keys;
			int count = keys.Count;
			Il2CppStringArray val = new Il2CppStringArray((long)count);
			keys.CopyTo((Il2CppArrayBase<string>)(object)val, 0);
			stringBuilder.Append("\n  总共 ").Append(count).Append(" 个 key");
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				string text = ((Il2CppArrayBase<string>)(object)val)[i];
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				TagState val2 = null;
				try
				{
					dict.TryGetValue(text, ref val2);
				}
				catch
				{
				}
				if (val2 != null && val2.valueEnabled)
				{
					num++;
					string value3 = "";
					string value4 = "";
					try
					{
						value3 = val2.identifierName;
					}
					catch
					{
					}
					try
					{
						_ = val2.identifier;
					}
					catch
					{
					}
					try
					{
						value4 = val2.valueString;
					}
					catch
					{
					}
					stringBuilder.Append("\n    ").Append(text).Append("   name='")
						.Append(value3)
						.Append("'")
						.Append("   label='")
						.Append(Display(text))
						.Append("'")
						.Append("   valueString='")
						.Append(value4)
						.Append("'");
				}
			}
			stringBuilder.Append("\n  启用 ").Append(num).Append(" / ")
				.Append(count);
		}
		catch (Exception ex2)
		{
			stringBuilder.Append("\n  异常: ").Append(ex2.Message);
		}
		Core.Log.Msg("[AutoStorage] DumpTags:" + stringBuilder);
	}

	private static void CollectItemTypes(GameItem it, HashSet<string> outTags)
	{
		try
		{
			List<string> itemTypes = it.itemTypes;
			if (itemTypes == null)
			{
				return;
			}
			int count = itemTypes.Count;
			if (count <= 0 || count > 64)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				string text = itemTypes[i];
				if (!string.IsNullOrEmpty(text))
				{
					outTags.Add(text);
				}
			}
		}
		catch
		{
		}
	}

	public static void Collect(GameItem it, HashSet<string> outTags)
	{
		if (it == null || outTags == null)
		{
			return;
		}
		CollectItemTypes(it, outTags);
		if (TryTyped(it, outTags))
		{
			return;
		}
		StatReflFail++;
		object obj = null;
		try
		{
			TagSystem state = it.state;
			if (state == null)
			{
				return;
			}
			obj = state.dict;
		}
		catch
		{
		}
		if (obj == null)
		{
			return;
		}
		try
		{
			Type type = obj.GetType();
			object obj3 = null;
			PropertyInfo property = type.GetProperty("Keys");
			if (property != null)
			{
				obj3 = property.GetValue(obj, null);
			}
			int num = 0;
			if (obj3 != null)
			{
				PropertyInfo property2 = obj3.GetType().GetProperty("Count");
				if (property2 != null)
				{
					num = (int)property2.GetValue(obj3, null);
				}
			}
			if (num <= 0 || num > 512 || obj3 == null)
			{
				return;
			}
			Type type2 = obj3.GetType();
			PropertyInfo property3 = type2.GetProperty("Item", new Type[1] { typeof(int) });
			if (property3 != null)
			{
				for (int i = 0; i < num; i++)
				{
					object obj4 = null;
					try
					{
						obj4 = property3.GetValue(obj3, new object[1] { i });
					}
					catch
					{
					}
					string text = obj4 as string;
					if (!string.IsNullOrEmpty(text) && ReflectIsEnabled(type, obj, text))
					{
						outTags.Add(text);
					}
				}
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
			if (!(methodInfo != null))
			{
				return;
			}
			Type parameterType = methodInfo.GetParameters()[0].ParameterType;
			object obj6 = null;
			if (parameterType == typeof(string[]))
			{
				obj6 = new string[num];
			}
			else
			{
				try
				{
					obj6 = Activator.CreateInstance(parameterType, (long)num);
				}
				catch
				{
				}
			}
			if (obj6 == null)
			{
				return;
			}
			methodInfo.Invoke(obj3, new object[2] { obj6, 0 });
			if (!(obj6 is IList list))
			{
				return;
			}
			for (int k = 0; k < num; k++)
			{
				string text2 = null;
				try
				{
					text2 = list[k] as string;
				}
				catch
				{
				}
				if (text2 != null)
				{
					if (text2.Length > 0 && ReflectIsEnabled(type, obj, text2))
					{
						outTags.Add(text2);
					}
					continue;
				}
				break;
			}
		}
		catch
		{
		}
	}

	private static bool ReflectIsEnabled(Type dictType, object dict, string key)
	{
		object obj = null;
		try
		{
			PropertyInfo property = dictType.GetProperty("Item");
			if (property != null)
			{
				obj = property.GetValue(dict, new object[1] { key });
			}
		}
		catch
		{
		}
		if (obj == null)
		{
			try
			{
				MethodInfo method = dictType.GetMethod("get_Item");
				if (method != null)
				{
					obj = method.Invoke(dict, new object[1] { key });
				}
			}
			catch
			{
			}
		}
		if (obj == null)
		{
			return false;
		}
		try
		{
			FieldInfo field = obj.GetType().GetField("valueEnabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				return (bool)field.GetValue(obj);
			}
		}
		catch
		{
		}
		try
		{
			PropertyInfo property2 = obj.GetType().GetProperty("valueEnabled");
			if (property2 != null)
			{
				return (bool)property2.GetValue(obj, null);
			}
		}
		catch
		{
		}
		return false;
	}
}
