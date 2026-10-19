using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XIAOWOTradePerks;

internal static class ModManagerUI
{
	internal const string WindowId = "xiaowo_mod_manager";

	internal const string ButtonTextChinese = "MOD 管理";

	internal const string ButtonTextEnglish = "MOD MANAGER";

	internal const string ApplyTextChinese = "应用";

	internal const string ApplyTextEnglish = "APPLY";

	private static GameObject _menuButtonObject;

	private static Button _menuButton;

	private static UnityAction _openAction;

	private static readonly List<Action> ButtonActions = new List<Action>();

	private static readonly List<Action<bool>> ToggleActions = new List<Action<bool>>();

	private static readonly List<Action<string>> InputActions = new List<Action<string>>();

	private static readonly Dictionary<string, bool> PendingStates = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

	private const string ConfirmWindowId = "xiaowo_mod_manager_confirm";

	private static string _tab = "mods";

	private static string _search = string.Empty;

	private static string _selectedCategory = string.Empty;

	private static float _listScrollPosition = 1f;

	private static MelonPreferences_Entry _capturingEntry;

	private static int _captureDelay;

	private static int _checkCountdown;

	private static string _notice = string.Empty;

	private static string _lastFailure = string.Empty;

	internal static bool IsCapturingHotkey => _capturingEntry != null;

	internal static int PendingChangeCount => PendingStates.Count;

	internal static void Tick()
	{
		if (_checkCountdown-- > 0)
		{
			return;
		}
		_checkCountdown = 15;
		try
		{
			MainMenuUIController instance = MainMenuUIController.Instance;
			if (!IsMainMenuVisible(instance))
			{
				ForgetButton();
				return;
			}
			ConfigureMenuButton(instance);
			_lastFailure = string.Empty;
		}
		catch (Exception ex)
		{
			string text = ex.GetType().Name + ": " + ex.Message;
			if (!string.Equals(text, _lastFailure, StringComparison.Ordinal))
			{
				_lastFailure = text;
				Instance log = Core.Log;
				if (log != null)
				{
					log.Warning("Mod manager button could not be created: " + text);
				}
			}
		}
	}

	internal static void CaptureTick()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (_capturingEntry == null || _captureDelay-- > 0)
		{
			return;
		}
		if (Input.GetKeyDown((KeyCode)27))
		{
			_capturingEntry = null;
			BuildWindow();
			return;
		}
		KeyCode[] values = Enum.GetValues<KeyCode>();
		for (int i = 0; i < values.Length; i++)
		{
			KeyCode val = values[i];
			if ((int)val > 0 && (int)val < 323 && !IsModifier(val) && Input.GetKeyDown(val))
			{
				bool flag = Input.GetKey((KeyCode)306) || Input.GetKey((KeyCode)305);
				bool flag2 = Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303);
				bool flag3 = Input.GetKey((KeyCode)308) || Input.GetKey((KeyCode)307);
				string value = ((_capturingEntry.GetReflectedType() == typeof(KeyCode)) ? ((object)(KeyCode)(ref val)).ToString() : ((flag ? "Ctrl+" : "") + (flag2 ? "Shift+" : "") + (flag3 ? "Alt+" : "") + ((object)(KeyCode)(ref val)).ToString()));
				StagePreference(_capturingEntry, value);
				_capturingEntry = null;
				BuildWindow();
				break;
			}
		}
	}

	private static bool IsModifier(KeyCode key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		if ((int)key != 306 && (int)key != 305 && (int)key != 304 && (int)key != 303 && (int)key != 308)
		{
			return (int)key == 307;
		}
		return true;
	}

	internal static void Open()
	{
		try
		{
			if (IsMainMenuVisible(MainMenuUIController.Instance))
			{
				MainMenuUIController.Instance.OnAnyButtonClick();
				OpenWindow();
			}
		}
		catch (Exception ex)
		{
			_notice = Select("无法打开 MOD 管理：", "Could not open the mod manager: ") + ex.Message;
			Instance log = Core.Log;
			if (log != null)
			{
				log.Warning("Mod manager window could not be opened: " + ex);
			}
		}
	}

	internal static void OpenInGame()
	{
		if (PlayerStore.instance != null)
		{
			OpenWindow();
		}
	}

	internal static void Toggle()
	{
		CustomUIManager instance = CustomUIManager.Instance;
		if (IsAlive((Object)(object)instance))
		{
			CustomUIWindow window = instance.GetWindow("xiaowo_mod_manager");
			if (window != null && window.IsVisible)
			{
				RequestClose();
				return;
			}
		}
		if (IsMainMenuVisible(MainMenuUIController.Instance))
		{
			Open();
		}
		else
		{
			OpenInGame();
		}
	}

	private static void OpenWindow()
	{
		try
		{
			_notice = string.Empty;
			BuildWindow();
		}
		catch (Exception ex)
		{
			_notice = Select("无法打开 MOD 管理：", "Could not open the mod manager: ") + ex.Message;
			Instance log = Core.Log;
			if (log != null)
			{
				log.Warning("Mod manager window could not be opened: " + ex);
			}
		}
	}

	internal static void Dispose()
	{
		CloseWindow();
		if (IsAlive((Object)(object)_menuButtonObject))
		{
			_menuButtonObject.SetActive(false);
		}
		ForgetButton();
		ButtonActions.Clear();
		ToggleActions.Clear();
		InputActions.Clear();
		_capturingEntry = null;
		PendingStates.Clear();
		ModPreferences.Revert();
		_notice = string.Empty;
		_lastFailure = string.Empty;
		_checkCountdown = 0;
	}

	internal static string BuildStatus()
	{
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<ManagedModEntry> readOnlyList = ModManager.Scan();
		CustomUIManager instance = CustomUIManager.Instance;
		CustomUIWindow val = (IsAlive((Object)(object)instance) ? instance.GetWindow("xiaowo_mod_manager") : null);
		CustomUIElement val2 = (IsAlive((Object)(object)instance) ? instance.GetElement("xiaowo_mod_manager", "summary") : null);
		CustomUIElement val3 = (IsAlive((Object)(object)instance) ? instance.GetElement("xiaowo_mod_manager", "apply") : null);
		CustomUIElement val4 = (IsAlive((Object)(object)instance) ? instance.GetElement("xiaowo_mod_manager", "mod_0") : null);
		TextMeshProUGUI val5 = (IsAlive((Object)(object)_menuButtonObject) ? _menuButtonObject.GetComponentInChildren<TextMeshProUGUI>(true) : null);
		RectTransform val6 = (IsAlive((Object)(object)_menuButtonObject) ? _menuButtonObject.GetComponent<RectTransform>() : null);
		GameObject val7 = (IsAlive((Object)(object)_menuButtonObject) ? FindExitButton(_menuButtonObject.transform.parent) : null);
		bool flag = IsAlive((Object)(object)val7) && (Object)(object)_menuButtonObject.transform.parent == (Object)(object)val7.transform.parent && _menuButtonObject.transform.GetSiblingIndex() + 1 == val7.transform.GetSiblingIndex();
		int num = (IsAlive((Object)(object)((val != null) ? val.root : null)) ? val.root.GetComponentsInChildren<Toggle>(true).Length : 0);
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (ManagedModEntry item in readOnlyList)
		{
			if (item.IsSelf)
			{
				num2++;
			}
			if (item.LoadedNow)
			{
				num3++;
			}
			if (!item.EnabledNextLaunch)
			{
				num4++;
			}
		}
		string[] obj = new string[36]
		{
			"MOD_MANAGER\nmainMenu=",
			IsAlive((Object)(object)MainMenuUIController.Instance).ToString(),
			"\nbuttonVisible=",
			(IsAlive((Object)(object)_menuButtonObject) && _menuButtonObject.activeInHierarchy).ToString(),
			"\nbuttonText=",
			((val5 != null) ? ((TMP_Text)val5).text : null) ?? "missing",
			"\nbuttonSize=",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		object obj2;
		Vector2 val8;
		if (!IsAlive((Object)(object)val6))
		{
			obj2 = "missing";
		}
		else
		{
			val8 = val6.sizeDelta;
			obj2 = ((object)(Vector2)(ref val8)).ToString();
		}
		obj[7] = (string)obj2;
		obj[8] = "\nbuttonPosition=";
		object obj3;
		if (!IsAlive((Object)(object)val6))
		{
			obj3 = "missing";
		}
		else
		{
			val8 = val6.anchoredPosition;
			obj3 = ((object)(Vector2)(ref val8)).ToString();
		}
		obj[9] = (string)obj3;
		obj[10] = "\nbuttonSibling=";
		obj[11] = (IsAlive((Object)(object)_menuButtonObject) ? _menuButtonObject.transform.GetSiblingIndex() : (-1)).ToString();
		obj[12] = "\nexitSibling=";
		obj[13] = (IsAlive((Object)(object)val7) ? val7.transform.GetSiblingIndex() : (-1)).ToString();
		obj[14] = "\nimmediatelyBeforeExit=";
		obj[15] = flag.ToString();
		obj[16] = "\nwindowOpen=";
		obj[17] = (val != null && val.IsVisible).ToString();
		obj[18] = "\nsummaryPresent=";
		obj[19] = ((Object)(object)((val2 != null) ? val2.text : null) != (Object)null).ToString();
		obj[20] = "\napplyPresent=";
		obj[21] = (val3 != null).ToString();
		obj[22] = "\npendingChanges=";
		obj[23] = PendingStates.Count.ToString();
		obj[24] = "\ntoggleCount=";
		obj[25] = num.ToString();
		obj[26] = "\nselfToggleLocked=";
		obj[27] = ((Object)(object)((val4 != null) ? val4.toggle : null) != (Object)null && !((Selectable)val4.toggle).interactable).ToString();
		obj[28] = "\nentries=";
		obj[29] = readOnlyList.Count.ToString();
		obj[30] = "\nloadedNow=";
		obj[31] = num3.ToString();
		obj[32] = "\ndisabledNextLaunch=";
		obj[33] = num4.ToString();
		obj[34] = "\nselfLocked=";
		obj[35] = (num2 == 1).ToString();
		return string.Concat(obj);
	}

	internal static bool ValidateDefinition(out string details)
	{
		details = "window=xiaowo_mod_manager, disabled=Disabled, button=" + Select("MOD 管理", "MOD MANAGER") + ", apply=" + Select("应用", "APPLY");
		return true;
	}

	private static void ConfigureMenuButton(MainMenuUIController menu)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		GameObject moddingButton = menu.moddingButton;
		if (!IsAlive((Object)(object)moddingButton))
		{
			throw new InvalidOperationException("The original modding button is unavailable.");
		}
		Button val = moddingButton.GetComponent<Button>() ?? moddingButton.GetComponentInChildren<Button>(true);
		TextMeshProUGUI componentInChildren = moddingButton.GetComponentInChildren<TextMeshProUGUI>(true);
		if (!IsAlive((Object)(object)val) || !IsAlive((Object)(object)componentInChildren))
		{
			throw new InvalidOperationException("The original modding button is incomplete.");
		}
		if ((Object)(object)_menuButtonObject != (Object)(object)moddingButton || (Object)(object)_menuButton != (Object)(object)val)
		{
			_menuButtonObject = moddingButton;
			_menuButton = val;
			_menuButton.onClick = new ButtonClickedEvent();
			if (_openAction == null)
			{
				_openAction = UnityAction.op_Implicit((Action)Open);
			}
			((UnityEvent)_menuButton.onClick).AddListener(_openAction);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Original main-menu modding button enabled for MelonLoader mod management.");
			}
		}
		((TMP_Text)componentInChildren).text = Select("MOD 管理", "MOD MANAGER");
		moddingButton.SetActive(true);
		((Selectable)val).interactable = true;
		GameObject val2 = FindExitButton(moddingButton.transform.parent);
		if (!IsAlive((Object)(object)val2))
		{
			throw new InvalidOperationException("The original exit button is unavailable.");
		}
		PlaceImmediatelyBefore(moddingButton.transform, val2.transform);
	}

	private static GameObject FindExitButton(Transform parent)
	{
		if (!IsAlive((Object)(object)parent))
		{
			return null;
		}
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			Button val = ((child != null) ? ((Component)child).gameObject.GetComponent<Button>() : null);
			if (!IsAlive((Object)(object)val))
			{
				continue;
			}
			for (int j = 0; j < ((UnityEventBase)val.onClick).GetPersistentEventCount(); j++)
			{
				if (string.Equals(((UnityEventBase)val.onClick).GetPersistentMethodName(j), "OnExitClick", StringComparison.Ordinal))
				{
					return ((Component)child).gameObject;
				}
			}
		}
		return null;
	}

	private static void PlaceImmediatelyBefore(Transform source, Transform target)
	{
		if (IsAlive((Object)(object)source) && IsAlive((Object)(object)target) && !((Object)(object)source.parent != (Object)(object)target.parent))
		{
			int siblingIndex = source.GetSiblingIndex();
			int siblingIndex2 = target.GetSiblingIndex();
			if (siblingIndex + 1 != siblingIndex2)
			{
				source.SetSiblingIndex((siblingIndex > siblingIndex2) ? siblingIndex2 : (siblingIndex2 - 1));
			}
		}
	}

	private static void BuildWindow()
	{
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Expected O, but got Unknown
		CustomUIManager instance = CustomUIManager.Instance;
		if (!IsAlive((Object)(object)instance))
		{
			throw new InvalidOperationException("Custom UI manager is unavailable.");
		}
		instance.CloseWindow("xiaowo_mod_manager");
		ButtonActions.Clear();
		ToggleActions.Clear();
		InputActions.Clear();
		IReadOnlyList<ManagedModEntry> readOnlyList = ModManager.Scan();
		string locale = ModLocalization.CurrentLocaleCode;
		CustomUIBuilder val = CustomUIManager.Window("xiaowo_mod_manager", Select("MOD 管理", "MOD MANAGER"), "overlay").SetSize(900f, 650f).Center()
			.SetCloseOnEscape(false)
			.SetDraggable(true);
		val.AddLabel(Select("更改先暂存，按“应用”后保存；DLL 启停和部分配置需要重启。", "Changes are staged until APPLY. DLL toggles and some preferences need a restart."), "summary");
		ConfigureLabel(val.Last, 15f, (FontStyles)0, 860f, 28f);
		val.AddLabel(_notice, "notice");
		ConfigureLabel(val.Last, 14f, (FontStyles)1, 860f, 28f);
		val.BeginRow(8f);
		AddTab(val, "mods", Select("MOD 列表", "MODS"));
		AddTab(val, "config", Select("配置", "PREFERENCES"));
		AddTab(val, "hotkeys", Select("热键", "HOTKEYS"));
		val.End();
		val.BeginRow(8f);
		val.BeginColumn(5f);
		Action<string> val2 = ToInputAction(delegate(string text)
		{
			_search = text ?? string.Empty;
		});
		InputActions.Add(val2);
		val.AddInput(Select("搜索", "Search"), _search, val2, "search");
		val.Last.SetPreferredSize(330f, 38f);
		Action val3 = ToAction(BuildWindow);
		ButtonActions.Add(val3);
		val.AddButton(Select("搜索 / 刷新", "SEARCH / REFRESH"), val3, "refresh");
		val.Last.SetPreferredSize(170f, 32f);
		val.BeginScroll(365f);
		if (_tab == "mods")
		{
			for (int i = 0; i < readOnlyList.Count; i++)
			{
				if (MatchesSearch(readOnlyList[i].Name + " " + readOnlyList[i].FileName))
				{
					AddEntry(val, readOnlyList[i], i);
				}
			}
		}
		else
		{
			foreach (MelonPreferences_Category category in ModPreferences.Categories())
			{
				IReadOnlyList<MelonPreferences_Entry> source = ModPreferences.Entries(category);
				if (!source.Any((MelonPreferences_Entry entry) => _tab == "hotkeys" == ModPreferences.IsHotkey(entry)))
				{
					continue;
				}
				string text2 = ModPreferenceTranslations.Category(locale, category.Identifier, category.DisplayName);
				if (!MatchesSearch(text2 + " " + category.DisplayName + " " + category.Identifier) && !source.Any((MelonPreferences_Entry entry) => _tab == "hotkeys" == ModPreferences.IsHotkey(entry) && MatchesSearch(ModPreferenceTranslations.Entry(locale, category.Identifier, entry.Identifier, entry.DisplayName) + " " + entry.DisplayName + " " + entry.Identifier)))
				{
					continue;
				}
				string id = category.Identifier;
				Action val4 = ToAction((Action)delegate
				{
					CustomUIManager instance2 = CustomUIManager.Instance;
					CustomUIWindow obj2 = ((instance2 != null) ? instance2.GetWindow("xiaowo_mod_manager") : null);
					object obj3;
					if (obj2 == null)
					{
						obj3 = null;
					}
					else
					{
						GameObject root2 = obj2.root;
						obj3 = ((root2 != null) ? ((IEnumerable<ScrollRect>)root2.GetComponentsInChildren<ScrollRect>(true)).FirstOrDefault() : null);
					}
					ScrollRect val10 = (ScrollRect)obj3;
					_listScrollPosition = (IsAlive((Object)(object)val10) ? val10.verticalNormalizedPosition : 1f);
					_selectedCategory = id;
					BuildWindow();
				});
				ButtonActions.Add(val4);
				val.AddButton(text2 + ((id == _selectedCategory) ? "  *" : ""), val4, "category_" + ButtonActions.Count);
				val.Last.SetPreferredSize(330f, 42f);
			}
		}
		val.End();
		val.End();
		val.BeginColumn(5f);
		val.BeginScroll(438f);
		if (_tab == "mods")
		{
			val.AddLabel(Select("选择要启停的 MOD；游戏中已加载的 DLL 不会立即卸载。", "Toggle mods for the next launch; loaded DLLs remain active in this session."), "details");
		}
		else
		{
			AddPreferenceDetails(val);
		}
		val.End();
		val.End();
		val.End();
		val.BeginRow(8f);
		Action val5 = ToAction(RevertChanges);
		ButtonActions.Add(val5);
		val.AddButton(Select("还原", "REVERT"), val5, "revert");
		val.Last.SetPreferredSize(180f, 42f);
		Action val6 = ToAction(ApplyChanges);
		ButtonActions.Add(val6);
		val.AddButton(Select("应用", "APPLY"), val6, "apply");
		val.Last.SetPreferredSize(180f, 42f);
		Action val7 = ToAction(RequestClose);
		ButtonActions.Add(val7);
		val.AddButton(Select("关闭", "CLOSE"), val7, "close");
		val.Last.SetPreferredSize(180f, 42f);
		val.End();
		CustomUIWindow val8 = val.Show();
		object obj;
		if (val8 == null)
		{
			obj = null;
		}
		else
		{
			GameObject root = val8.root;
			obj = ((root != null) ? root.GetComponentsInChildren<ScrollRect>(true) : null);
		}
		Il2CppArrayBase<ScrollRect> val9 = (Il2CppArrayBase<ScrollRect>)obj;
		if (val9 != null && val9.Length >= 2)
		{
			SetScrollWidth(val9[0], 350f);
			SetScrollWidth(val9[1], 505f);
			Canvas.ForceUpdateCanvases();
			val9[0].verticalNormalizedPosition = _listScrollPosition;
		}
		object value;
		if (val8 == null)
		{
			value = null;
		}
		else
		{
			CustomUIWindowFrame frame = val8.frame;
			value = ((frame != null) ? frame.closeButton : null);
		}
		if (IsAlive((Object)value))
		{
			val8.frame.closeButton.onClick = new ButtonClickedEvent();
			((UnityEvent)val8.frame.closeButton.onClick).AddListener(UnityAction.op_Implicit((Action)RequestClose));
		}
	}

	private static void AddEntry(CustomUIBuilder builder, ManagedModEntry entry, int index)
	{
		bool desiredState = GetDesiredState(entry);
		string text = BuildEntryDisplay(entry, desiredState);
		ManagedModEntry captured = entry;
		Action<bool> val = ToAction(delegate(bool enabled)
		{
			ChangeState(captured, index, enabled);
		});
		ToggleActions.Add(val);
		builder.AddToggle(text, desiredState, val, "mod_" + index);
		CustomUIElement last = builder.Last;
		last.SetPreferredSize(330f, 62f);
		if ((Object)(object)last.text != (Object)null)
		{
			((TMP_Text)last.text).fontSize = 14f;
			((TMP_Text)last.text).enableWordWrapping = false;
			((TMP_Text)last.text).overflowMode = (TextOverflowModes)1;
		}
		if ((Object)(object)last.toggle != (Object)null)
		{
			((Selectable)last.toggle).interactable = entry.CanToggle && !entry.IsSelf;
		}
		last.SetTooltip(entry.FilePath + "\n" + GetDisplayStatus(entry, desiredState));
	}

	private static void SetScrollWidth(ScrollRect scroll, float width)
	{
		if (IsAlive((Object)(object)scroll))
		{
			LayoutElement obj = ((Component)scroll).GetComponent<LayoutElement>() ?? ((Component)scroll).gameObject.AddComponent<LayoutElement>();
			obj.minWidth = width;
			obj.preferredWidth = width;
			obj.flexibleWidth = 0f;
		}
	}

	private static void AddTab(CustomUIBuilder builder, string id, string label)
	{
		Action val = ToAction((Action)delegate
		{
			_tab = id;
			_listScrollPosition = 1f;
			BuildWindow();
		});
		ButtonActions.Add(val);
		builder.AddButton(label + ((_tab == id) ? "  *" : ""), val, "tab_" + id);
		builder.Last.SetPreferredSize(160f, 36f);
	}

	private static void AddPreferenceDetails(CustomUIBuilder builder)
	{
		string currentLocaleCode = ModLocalization.CurrentLocaleCode;
		IReadOnlyList<MelonPreferences_Category> source = ModPreferences.Categories();
		MelonPreferences_Category val = ((IEnumerable<MelonPreferences_Category>)source).FirstOrDefault((Func<MelonPreferences_Category, bool>)((MelonPreferences_Category item) => item.Identifier == _selectedCategory && HasRows(item))) ?? ((IEnumerable<MelonPreferences_Category>)source).FirstOrDefault((Func<MelonPreferences_Category, bool>)HasRows);
		if (val == null)
		{
			builder.AddLabel(Select("没有可编辑的注册配置。", "No registered editable preferences."), (string)null);
			return;
		}
		_selectedCategory = val.Identifier;
		builder.AddLabel(ModPreferenceTranslations.Category(currentLocaleCode, val.Identifier, val.DisplayName), "selected_category");
		ConfigureLabel(builder.Last, 18f, (FontStyles)1, 480f, 30f);
		foreach (MelonPreferences_Entry item in ModPreferences.Entries(val))
		{
			if (_tab == "hotkeys" != ModPreferences.IsHotkey(item))
			{
				continue;
			}
			MelonPreferences_Entry current = item;
			string text = ModPreferenceTranslations.Entry(currentLocaleCode, val.Identifier, item.Identifier, item.DisplayName);
			string tooltip = ModPreferenceTranslations.Description(currentLocaleCode, val.Identifier, item.Identifier, item.Description ?? item.Identifier);
			if (item.GetReflectedType() == typeof(bool))
			{
				Action<bool> val2 = ToAction(delegate(bool value)
				{
					StagePreference(current, value.ToString());
				});
				ToggleActions.Add(val2);
				builder.AddToggle(text, bool.TryParse(ModPreferences.Value(item), out var result) && result, val2, "pref_" + ToggleActions.Count);
				builder.Last.SetPreferredSize(480f, 44f);
				builder.Last.SetTooltip(tooltip);
				continue;
			}
			builder.AddLabel(text, (string)null);
			ConfigureLabel(builder.Last, 14f, (FontStyles)0, 480f, 23f);
			Action<string> val3 = ToInputAction(delegate(string value)
			{
				StagePreference(current, value);
			});
			InputActions.Add(val3);
			if (ModPreferences.IsHotkey(item))
			{
				builder.BeginRow(5f);
			}
			builder.AddInput(text, ModPreferences.Value(item), val3, "pref_" + InputActions.Count);
			builder.Last.SetPreferredSize(ModPreferences.IsHotkey(item) ? 345f : 480f, 39f);
			builder.Last.SetTooltip(tooltip);
			if (!ModPreferences.IsHotkey(item))
			{
				continue;
			}
			Action val4 = ToAction((Action)delegate
			{
				_capturingEntry = current;
				_captureDelay = 2;
				_notice = Select("按下新快捷键；Esc 取消。", "Press a shortcut; Esc cancels.");
				CustomUIManager instance = CustomUIManager.Instance;
				CustomUIElement val5 = ((instance != null) ? instance.GetElement("xiaowo_mod_manager", "notice") : null);
				if ((Object)(object)((val5 != null) ? val5.text : null) != (Object)null)
				{
					((TMP_Text)val5.text).text = _notice;
				}
			});
			ButtonActions.Add(val4);
			builder.AddButton(Select("按键绑定", "BIND KEY"), val4, "capture_" + ButtonActions.Count);
			builder.Last.SetPreferredSize(125f, 39f);
			builder.End();
		}
		if (!(_tab == "hotkeys"))
		{
			return;
		}
		foreach (string item2 in ModPreferences.Conflicts())
		{
			builder.AddLabel(Select("冲突：", "Conflict: ") + item2, (string)null);
			ConfigureLabel(builder.Last, 13f, (FontStyles)0, 480f, 36f);
		}
		static bool HasRows(MelonPreferences_Category item)
		{
			return ModPreferences.Entries(item).Any((MelonPreferences_Entry entry) => _tab == "hotkeys" == ModPreferences.IsHotkey(entry));
		}
	}

	private static bool MatchesSearch(string value)
	{
		if (_search.Length != 0)
		{
			return value.Contains(_search, StringComparison.CurrentCultureIgnoreCase);
		}
		return true;
	}

	private static void StagePreference(MelonPreferences_Entry entry, string value)
	{
		string error;
		bool num = ModPreferences.Stage(entry, value, out error);
		_notice = (num ? (Select("待应用配置：", "Pending preferences: ") + ModPreferences.PendingCount) : (ModPreferenceTranslations.Entry(ModLocalization.CurrentLocaleCode, entry.Category.Identifier, entry.Identifier, entry.DisplayName) + ": " + error));
		if (num && ModPreferences.IsHotkey(entry))
		{
			string text = ModPreferences.Conflicts().FirstOrDefault((string item) => item.Contains(entry.Category.DisplayName + "/" + entry.DisplayName, StringComparison.Ordinal));
			if (text != null)
			{
				_notice = Select("热键冲突：", "Hotkey conflict: ") + text;
			}
		}
		CustomUIManager instance = CustomUIManager.Instance;
		CustomUIElement val = (IsAlive((Object)(object)instance) ? instance.GetElement("xiaowo_mod_manager", "notice") : null);
		if ((Object)(object)((val != null) ? val.text : null) != (Object)null)
		{
			((TMP_Text)val.text).text = _notice;
		}
	}

	private static void RevertChanges()
	{
		PendingStates.Clear();
		ModPreferences.Revert();
		_notice = Select("已还原未应用的更改。", "Unapplied changes reverted.");
		BuildWindow();
	}

	private static void ChangeState(ManagedModEntry entry, int index, bool enabled)
	{
		StageState(entry, enabled, out _notice);
		CustomUIManager instance = CustomUIManager.Instance;
		if (IsAlive((Object)(object)instance))
		{
			bool desiredState = GetDesiredState(entry);
			CustomUIElement element = instance.GetElement("xiaowo_mod_manager", "mod_" + index);
			if ((Object)(object)((element != null) ? element.text : null) != (Object)null)
			{
				((TMP_Text)element.text).text = BuildEntryDisplay(entry, desiredState);
			}
			if (element != null)
			{
				element.SetTooltip(entry.FilePath + "\n" + GetDisplayStatus(entry, desiredState));
			}
			CustomUIElement element2 = instance.GetElement("xiaowo_mod_manager", "notice");
			if ((Object)(object)((element2 != null) ? element2.text : null) != (Object)null)
			{
				((TMP_Text)element2.text).text = _notice;
			}
		}
	}

	internal static bool StageState(ManagedModEntry entry, bool enabled, out string message)
	{
		if (entry == null)
		{
			message = Select("未找到该 MOD。", "The mod entry is unavailable.");
			return false;
		}
		if (entry.IsSelf || !entry.CanToggle)
		{
			message = (entry.IsSelf ? Select("主 MOD 不能停用。", "The core mod cannot be disabled.") : Select("该 MOD 当前不能管理。", "This mod cannot currently be managed."));
			return false;
		}
		string key = NormalizePath(entry.FilePath);
		if (enabled == entry.EnabledNextLaunch)
		{
			PendingStates.Remove(key);
		}
		else
		{
			PendingStates[key] = enabled;
		}
		message = ((PendingStates.Count == 0) ? Select("没有待应用的更改。", "There are no pending changes.") : (Select("待应用更改：", "Pending changes: ") + PendingStates.Count));
		return true;
	}

	private static void ApplyChanges()
	{
		ApplyPendingChanges(out _notice);
		BuildWindow();
	}

	internal static bool ApplyPendingChanges(out string message)
	{
		if (PendingStates.Count == 0 && ModPreferences.PendingCount == 0)
		{
			message = Select("没有待应用的更改。", "There are no pending changes.");
			return true;
		}
		if (!ModPreferences.Apply(out var message2))
		{
			message = message2;
			return false;
		}
		Dictionary<string, ManagedModEntry> dictionary = ModManager.Scan().ToDictionary<ManagedModEntry, string>((ManagedModEntry entry) => NormalizePath(entry.FilePath), StringComparer.OrdinalIgnoreCase);
		KeyValuePair<string, bool>[] array = PendingStates.ToArray();
		int num = 0;
		List<string> list = new List<string>();
		KeyValuePair<string, bool>[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			KeyValuePair<string, bool> keyValuePair = array2[i];
			if (!dictionary.TryGetValue(keyValuePair.Key, out var value))
			{
				list.Add(Path.GetFileName(keyValuePair.Key) + ": " + Select("文件状态已改变", "file state changed"));
				continue;
			}
			if (!ModManager.TrySetEnabled(value, keyValuePair.Value, out var message3))
			{
				list.Add(value.FileName + ": " + message3);
				continue;
			}
			PendingStates.Remove(keyValuePair.Key);
			num++;
		}
		if (list.Count == 0)
		{
			message = Select("已应用 ", "Applied ") + num + Select(" 项 DLL 更改；配置已保存。部分修改重启后生效。", " DLL change(s); preferences saved. Some changes require restart.");
			return true;
		}
		message = Select("已应用 ", "Applied ") + num + Select(" 项；失败：", "; failed: ") + string.Join("；", list);
		return false;
	}

	internal static void ClearPendingChanges()
	{
		PendingStates.Clear();
		ModPreferences.Revert();
	}

	private static bool GetDesiredState(ManagedModEntry entry)
	{
		if (!PendingStates.TryGetValue(NormalizePath(entry.FilePath), out var value))
		{
			return entry.EnabledNextLaunch;
		}
		return value;
	}

	private static string GetDisplayStatus(ManagedModEntry entry, bool desiredState)
	{
		if (desiredState != entry.EnabledNextLaunch)
		{
			if (!desiredState)
			{
				return Select("待应用 / 下次停用", "Pending / disable next launch");
			}
			return Select("待应用 / 下次启用", "Pending / enable next launch");
		}
		return ModManager.GetStatus(entry);
	}

	private static string BuildEntryDisplay(ManagedModEntry entry, bool desiredState)
	{
		string text = entry.Name;
		if (!string.IsNullOrWhiteSpace(entry.Version))
		{
			text = text + "  v" + entry.Version;
		}
		if (!string.IsNullOrWhiteSpace(entry.Author))
		{
			text = text + "  |  " + entry.Author;
		}
		return text + "\n" + GetDisplayStatus(entry, desiredState) + "  |  " + entry.FileName;
	}

	private static string NormalizePath(string path)
	{
		return Path.GetFullPath(path ?? string.Empty);
	}

	private static void ConfigureLabel(CustomUIElement element, float size, FontStyles style, float width, float height)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (element != null)
		{
			element.SetPreferredSize(width, height);
			if (!((Object)(object)element.text == (Object)null))
			{
				((TMP_Text)element.text).fontSize = size;
				((TMP_Text)element.text).fontStyle = style;
				((TMP_Text)element.text).enableWordWrapping = false;
				((TMP_Text)element.text).overflowMode = (TextOverflowModes)1;
			}
		}
	}

	private static void CloseWindow()
	{
		CustomUIManager instance = CustomUIManager.Instance;
		if (IsAlive((Object)(object)instance))
		{
			instance.CloseWindow("xiaowo_mod_manager_confirm");
			instance.CloseWindow("xiaowo_mod_manager");
		}
		PendingStates.Clear();
		ModPreferences.Revert();
		_capturingEntry = null;
		_notice = string.Empty;
	}

	private static void RequestClose()
	{
		if (PendingStates.Count == 0 && ModPreferences.PendingCount == 0)
		{
			CloseWindow();
			return;
		}
		CustomUIManager manager = CustomUIManager.Instance;
		if (!IsAlive((Object)(object)manager))
		{
			return;
		}
		CustomUIWindow window = manager.GetWindow("xiaowo_mod_manager_confirm");
		if (window == null || !window.IsVisible)
		{
			CustomUIBuilder obj = CustomUIManager.Window("xiaowo_mod_manager_confirm", Select("未应用的更改", "UNAPPLIED CHANGES"), "overlay").SetSize(440f, 195f).Center()
				.SetCloseOnEscape(true);
			obj.AddLabel(Select("离开会放弃尚未应用的修改。", "Leaving will discard unapplied changes."), (string)null);
			obj.BeginRow(8f);
			Action val = ToAction((Action)delegate
			{
				manager.CloseWindow("xiaowo_mod_manager_confirm");
			});
			Action val2 = ToAction(CloseWindow);
			ButtonActions.Add(val);
			ButtonActions.Add(val2);
			obj.AddButton(Select("继续编辑", "KEEP EDITING"), val, "keep");
			obj.AddButton(Select("放弃更改", "DISCARD"), val2, "discard");
			obj.End();
			obj.Show();
		}
	}

	private static void ForgetButton()
	{
		_menuButtonObject = null;
		_menuButton = null;
	}

	private static Action ToAction(Action action)
	{
		return DelegateSupport.ConvertDelegate<Action>((Delegate)action);
	}

	private static Action<bool> ToAction(Action<bool> action)
	{
		return DelegateSupport.ConvertDelegate<Action<bool>>((Delegate)action);
	}

	private static Action<string> ToInputAction(Action<string> action)
	{
		return DelegateSupport.ConvertDelegate<Action<string>>((Delegate)action);
	}

	private static string Select(string chinese, string english)
	{
		return ModLocalization.Select(chinese, english);
	}

	private static bool IsAlive(Object value)
	{
		if (value != (Object)null)
		{
			return Object.op_Implicit(value);
		}
		return false;
	}

	private static bool IsMainMenuVisible(MainMenuUIController menu)
	{
		if (!IsAlive((Object)(object)menu) || !((Component)menu).gameObject.activeInHierarchy)
		{
			return false;
		}
		GameObject moddingButton = menu.moddingButton;
		if (IsAlive((Object)(object)moddingButton) && IsAlive((Object)(object)moddingButton.transform.parent))
		{
			return ((Component)moddingButton.transform.parent).gameObject.activeInHierarchy;
		}
		return false;
	}
}
