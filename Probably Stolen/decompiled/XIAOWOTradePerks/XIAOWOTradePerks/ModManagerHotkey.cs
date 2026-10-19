using System;
using Il2CppTMPro;
using MelonLoader;
using MelonLoader.Preferences;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XIAOWOTradePerks;

internal static class ModManagerHotkey
{
	private static MelonPreferences_Entry<string> _binding;

	internal static void Initialize()
	{
		_binding = MelonPreferences.CreateCategory("XIAOWOTradePerks_ModManager", "MOD Manager").CreateEntry<string>("ToggleHotkey", "BackQuote", "Toggle window", "Physical key above Tab; combinations such as Ctrl+Shift+M are supported.", false, false, (ValueValidator)null, (string)null);
	}

	internal static void Tick()
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (_binding == null || ModManagerUI.IsCapturingHotkey || IsEditingText())
		{
			return;
		}
		string text = ModPreferences.NormalizeHotkey(_binding.Value);
		string[] array = text.Split('+', StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 0 && Enum.TryParse<KeyCode>(array[^1], ignoreCase: true, out KeyCode result))
		{
			bool num = Input.GetKey((KeyCode)306) || Input.GetKey((KeyCode)305);
			bool flag = Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303);
			bool flag2 = Input.GetKey((KeyCode)308) || Input.GetKey((KeyCode)307);
			if (num == text.StartsWith("Ctrl+", StringComparison.Ordinal) && flag == text.Contains("Shift+", StringComparison.Ordinal) && flag2 == text.Contains("Alt+", StringComparison.Ordinal) && Input.GetKeyDown(result))
			{
				ModManagerUI.Toggle();
			}
		}
	}

	private static bool IsEditingText()
	{
		EventSystem current = EventSystem.current;
		GameObject val = ((current != null) ? current.currentSelectedGameObject : null);
		if ((Object)(object)val != (Object)null)
		{
			TMP_InputField component = val.GetComponent<TMP_InputField>();
			if (component == null || !component.isFocused)
			{
				InputField component2 = val.GetComponent<InputField>();
				if (component2 == null)
				{
					return false;
				}
				return component2.isFocused;
			}
			return true;
		}
		return false;
	}
}
