using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

namespace XIAOWOTradePerks;

internal static class ModPreferences
{
	private static readonly Dictionary<string, string> Pending = new Dictionary<string, string>(StringComparer.Ordinal);

	internal static int PendingCount => Pending.Count;

	internal static IReadOnlyList<MelonPreferences_Category> Categories()
	{
		return (from category in MelonPreferences.Categories
			where !category.IsHidden && category.Entries.Any(IsSupported)
			orderby category.DisplayName
			select category).ToArray();
	}

	internal static IReadOnlyList<MelonPreferences_Entry> Entries(MelonPreferences_Category category)
	{
		return (from entry in category?.Entries.Where(IsSupported)
			orderby entry.DisplayName
			select entry).ToArray() ?? Array.Empty<MelonPreferences_Entry>();
	}

	internal static bool IsHotkey(MelonPreferences_Entry entry)
	{
		if (entry != null)
		{
			if (!(entry.GetReflectedType() == typeof(KeyCode)))
			{
				if (entry.GetReflectedType() == typeof(string))
				{
					if (!entry.Identifier.Contains("hotkey", StringComparison.OrdinalIgnoreCase) && !entry.Identifier.Contains("shortcut", StringComparison.OrdinalIgnoreCase))
					{
						return entry.Identifier.Contains("keybind", StringComparison.OrdinalIgnoreCase);
					}
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	internal static string Value(MelonPreferences_Entry entry)
	{
		if (!Pending.TryGetValue(Key(entry), out var value))
		{
			return Format(entry.BoxedValue);
		}
		return value;
	}

	internal static bool Stage(MelonPreferences_Entry entry, string text, out string error)
	{
		error = string.Empty;
		if (!IsSupported(entry))
		{
			error = "Unsupported preference";
			return false;
		}
		if (string.Equals(text, Format(entry.BoxedValue), StringComparison.Ordinal))
		{
			Pending.Remove(Key(entry));
		}
		else
		{
			Pending[Key(entry)] = text;
		}
		object value;
		return TryParse(entry, text, out value, out error);
	}

	internal static void Revert()
	{
		Pending.Clear();
	}

	internal static bool Apply(out string message)
	{
		List<(MelonPreferences_Entry, object)> list = new List<(MelonPreferences_Entry, object)>();
		foreach (MelonPreferences_Category item in Categories())
		{
			foreach (MelonPreferences_Entry item2 in Entries(item))
			{
				if (Pending.TryGetValue(Key(item2), out var value))
				{
					if (!TryParse(item2, value, out var value2, out message))
					{
						message = item.DisplayName + " / " + item2.DisplayName + ": " + message;
						return false;
					}
					list.Add((item2, value2));
				}
			}
		}
		if (list.Count != Pending.Count)
		{
			message = "Registered preferences changed; refresh and try again.";
			return false;
		}
		if (list.Count == 0)
		{
			message = "No pending preferences.";
			return true;
		}
		(MelonPreferences_Entry, object)[] array = list.Select<(MelonPreferences_Entry, object), (MelonPreferences_Entry, object)>(((MelonPreferences_Entry Entry, object Value) change) => (Entry: change.Entry, Value: change.Entry.BoxedValue)).ToArray();
		try
		{
			string text = Path.Combine(MelonEnvironment.UserDataDirectory, "MelonPreferences.cfg");
			if (File.Exists(text))
			{
				string destFileName = text + ".xiaowo-backup-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
				File.Copy(text, destFileName);
			}
			foreach (var item3 in list)
			{
				item3.Item1.BoxedEditedValue = item3.Item2;
			}
			foreach (MelonPreferences_Category item4 in list.Select<(MelonPreferences_Entry, object), MelonPreferences_Category>(((MelonPreferences_Entry Entry, object Value) change) => change.Entry.Category).Distinct())
			{
				item4.SaveToFile(false);
			}
			Pending.Clear();
			message = "Saved " + list.Count + " preference(s). Some mods require a restart.";
			return true;
		}
		catch (Exception ex)
		{
			(MelonPreferences_Entry, object)[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				(MelonPreferences_Entry, object) tuple = array2[i];
				tuple.Item1.BoxedEditedValue = tuple.Item2;
			}
			message = ex.GetType().Name + ": " + ex.Message;
			return false;
		}
	}

	internal static IReadOnlyList<string> Conflicts()
	{
		return (from @group in Categories().SelectMany(Entries).Where(IsHotkey).GroupBy<MelonPreferences_Entry, string>((MelonPreferences_Entry entry) => NormalizeHotkey(Value(entry)), StringComparer.OrdinalIgnoreCase)
			where @group.Key.Length > 0 && @group.Count() > 1
			select @group.Key + ": " + string.Join(", ", @group.Select((MelonPreferences_Entry entry) => entry.Category.DisplayName + "/" + entry.DisplayName))).ToArray();
	}

	internal static string NormalizeHotkey(string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return string.Empty;
		}
		string[] array = text.Replace(" ", "").Split('+', StringSplitOptions.RemoveEmptyEntries);
		string text2 = ((array[^1].Equals("`", StringComparison.Ordinal) || array[^1] == "·") ? "BackQuote" : array[^1]);
		bool num = array.Take(array.Length - 1).Any((string part) => part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) || part.Equals("Control", StringComparison.OrdinalIgnoreCase));
		bool flag = array.Take(array.Length - 1).Any((string part) => part.Equals("Shift", StringComparison.OrdinalIgnoreCase));
		return string.Concat(str2: array.Take(array.Length - 1).Any((string part) => part.Equals("Alt", StringComparison.OrdinalIgnoreCase)) ? "Alt+" : "", str0: num ? "Ctrl+" : "", str1: flag ? "Shift+" : "", str3: text2);
	}

	private static bool IsSupported(MelonPreferences_Entry entry)
	{
		if (entry == null || entry.IsHidden)
		{
			return false;
		}
		Type reflectedType = entry.GetReflectedType();
		if (!(reflectedType == typeof(bool)) && !(reflectedType == typeof(int)) && !(reflectedType == typeof(float)) && !(reflectedType == typeof(double)))
		{
			return IsHotkey(entry);
		}
		return true;
	}

	private static bool TryParse(MelonPreferences_Entry entry, string text, out object value, out string error)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		value = null;
		error = string.Empty;
		Type reflectedType = entry.GetReflectedType();
		int result2;
		float result3;
		double result4;
		KeyCode result5;
		if (reflectedType == typeof(bool) && bool.TryParse(text, out var result))
		{
			value = result;
		}
		else if (reflectedType == typeof(int) && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out result2))
		{
			value = result2;
		}
		else if (reflectedType == typeof(float) && float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result3) && float.IsFinite(result3))
		{
			value = result3;
		}
		else if (reflectedType == typeof(double) && double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result4) && double.IsFinite(result4))
		{
			value = result4;
		}
		else if (reflectedType == typeof(KeyCode) && Enum.TryParse<KeyCode>(text, ignoreCase: true, out result5) && Enum.IsDefined<KeyCode>(result5))
		{
			value = result5;
		}
		else if (reflectedType == typeof(string) && IsHotkey(entry) && ValidCombo(text))
		{
			value = NormalizeHotkey(text);
		}
		if (value == null)
		{
			error = "Invalid value";
			return false;
		}
		if (entry.Validator != null && !entry.Validator.IsValid(value))
		{
			error = "Value is outside this mod's allowed range";
			return false;
		}
		return true;
	}

	private static bool ValidCombo(string text)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}
		string[] array = text.Replace(" ", "").Split('+', StringSplitOptions.RemoveEmptyEntries);
		int num = array.Length;
		if ((num < 1 || num > 4) ? true : false)
		{
			return false;
		}
		for (int i = 0; i < array.Length - 1; i++)
		{
			if (!new string[4] { "Ctrl", "Control", "Shift", "Alt" }.Contains<string>(array[i], StringComparer.OrdinalIgnoreCase))
			{
				return false;
			}
		}
		if (!(array[^1] == "`") && !(array[^1] == "·"))
		{
			if (Enum.TryParse<KeyCode>(array[^1], ignoreCase: true, out KeyCode result))
			{
				return Enum.IsDefined<KeyCode>(result);
			}
			return false;
		}
		return true;
	}

	private static string Format(object value)
	{
		object obj;
		if (!(value is IFormattable formattable))
		{
			obj = value?.ToString();
			if (obj == null)
			{
				return string.Empty;
			}
		}
		else
		{
			obj = formattable.ToString(null, CultureInfo.InvariantCulture);
		}
		return (string)obj;
	}

	private static string Key(MelonPreferences_Entry entry)
	{
		return entry.Category.Identifier + "\0" + entry.Identifier;
	}
}
