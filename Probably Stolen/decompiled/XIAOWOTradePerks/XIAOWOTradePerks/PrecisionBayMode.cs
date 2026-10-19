using System;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XIAOWOTradePerks;

internal static class PrecisionBayMode
{
	internal const string SaveKey = "xiaowo_trade_precision_bay_enabled";

	internal const string PickerToggleName = "XIAOWO_PrecisionBayToggle";

	internal const string PauseButtonName = "XIAOWO_ModSettingsButton";

	internal const string WindowId = "xiaowo_save_mod_settings";

	private static GameObject _pickerToggleObject;

	private static Toggle _pickerToggle;

	private static UnityAction<bool> _pickerChangedAction;

	private static bool _newGameEnabled = true;

	private static GameObject _pauseButtonObject;

	private static Button _pauseButton;

	private static UnityAction _openPauseAction;

	private static Action<bool> _saveToggleAction;

	private static Action _manageModsAction;

	private static Action _closeAction;

	private static EscapeUIManager _escapeManager;

	private static int _checkCountdown;

	private static string _lastFailure = string.Empty;

	private static string PickerText => ModLocalization.Select("精工扩舱", "Precision Bay Expansion");

	private static string ModSettingsText => ModLocalization.Select("MOD 设置", "MOD SETTINGS");

	private static string CurrentSaveToggleText => ModLocalization.Select("允许精工扩舱继续升级", "Allow further Precision Bay upgrades");

	private static string DescriptionText => ModLocalization.Select("此开关只影响当前存档。关闭后停止金属锭触发的新升级；已有等级、容量、物品和窗口缩放均会保留。", "This affects only the current save. Disabling stops new metal-ingot upgrades while preserving existing levels, capacity, items, and window resizing.");

	internal static bool IsUpgradeEnabled(PlayerStore store = null)
	{
		if (store == null)
		{
			store = PlayerStore.instance;
		}
		if (store == null || string.IsNullOrEmpty(store.runID))
		{
			return _newGameEnabled;
		}
		Dictionary<string, string> modData = store.modData;
		string a = default(string);
		if (modData == null || !modData.TryGetValue("xiaowo_trade_precision_bay_enabled", ref a))
		{
			return true;
		}
		return !string.Equals(a, "0", StringComparison.Ordinal);
	}

	internal static void PrepareNewGame()
	{
		_newGameEnabled = true;
		if (IsAlive((Object)(object)_pickerToggle))
		{
			_pickerToggle.SetIsOnWithoutNotify(true);
		}
	}

	internal static void AfterPickerOpen(PerkUIController ui)
	{
		EnsurePickerToggle(ui);
	}

	internal static void ApplyNewGameSelection(PlayerStore store)
	{
		if (store != null)
		{
			SetStoredState(store, _newGameEnabled);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Precision Bay Expansion new-save state: " + (_newGameEnabled ? "enabled" : "disabled"));
			}
		}
	}

	internal static bool SetForCurrentSave(bool enabled, bool saveImmediately)
	{
		PlayerStore instance = PlayerStore.instance;
		if (instance == null || string.IsNullOrEmpty(instance.runID))
		{
			return false;
		}
		SetStoredState(instance, enabled);
		if (saveImmediately)
		{
			instance.SaveGame();
		}
		Instance log = Core.Log;
		if (log != null)
		{
			log.Msg("Precision Bay Expansion for current save: " + (enabled ? "enabled" : "disabled"));
		}
		return true;
	}

	internal static void Tick()
	{
		if (_checkCountdown-- > 0)
		{
			return;
		}
		_checkCountdown = 15;
		try
		{
			EscapeUIManager val = Object.FindObjectOfType<EscapeUIManager>();
			if (IsAlive((Object)(object)((val != null) ? val.menuPanel : null)) && val.menuPanel.activeInHierarchy)
			{
				EnsurePauseButton(val);
				_lastFailure = string.Empty;
			}
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
					log.Warning("Save MOD settings button could not be created: " + text);
				}
			}
		}
	}

	internal static void EnsurePauseButton(EscapeUIManager manager)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		if (!IsAlive((Object)(object)((manager != null) ? manager.menuPanel : null)))
		{
			return;
		}
		GameObject val = FindNativeButton(manager.menuPanel.transform, "OnOption");
		if (!IsAlive((Object)(object)val))
		{
			throw new InvalidOperationException("The native pause-menu option button is unavailable.");
		}
		Transform parent = val.transform.parent;
		if (!IsAlive((Object)(object)parent))
		{
			throw new InvalidOperationException("The pause-menu button parent is unavailable.");
		}
		if (!IsAlive((Object)(object)_pauseButtonObject) || (Object)(object)_pauseButtonObject.transform.parent != (Object)(object)parent)
		{
			if (IsAlive((Object)(object)_pauseButtonObject))
			{
				Object.Destroy((Object)(object)_pauseButtonObject);
			}
			_pauseButtonObject = Object.Instantiate<GameObject>(val, parent, false);
			((Object)_pauseButtonObject).name = "XIAOWO_ModSettingsButton";
			_pauseButton = _pauseButtonObject.GetComponent<Button>() ?? _pauseButtonObject.GetComponentInChildren<Button>(true);
			if (!IsAlive((Object)(object)_pauseButton))
			{
				throw new InvalidOperationException("The cloned MOD settings button is incomplete.");
			}
			_pauseButton.onClick = new ButtonClickedEvent();
			if (_openPauseAction == null)
			{
				_openPauseAction = UnityAction.op_Implicit((Action)OpenPauseSettings);
			}
			((UnityEvent)_pauseButton.onClick).AddListener(_openPauseAction);
		}
		_escapeManager = manager;
		TextMeshProUGUI componentInChildren = _pauseButtonObject.GetComponentInChildren<TextMeshProUGUI>(true);
		if (IsAlive((Object)(object)componentInChildren))
		{
			((TMP_Text)componentInChildren).text = ModSettingsText;
		}
		_pauseButtonObject.transform.SetSiblingIndex(Math.Min(parent.childCount - 1, val.transform.GetSiblingIndex() + 1));
		((Selectable)_pauseButton).interactable = true;
		_pauseButtonObject.SetActive(true);
	}

	internal static string BuildStatus()
	{
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		PlayerStore instance = PlayerStore.instance;
		CustomUIManager instance2 = CustomUIManager.Instance;
		CustomUIWindow val = (IsAlive((Object)(object)instance2) ? instance2.GetWindow("xiaowo_save_mod_settings") : null);
		TextMeshProUGUI val2 = (IsAlive((Object)(object)_pickerToggleObject) ? _pickerToggleObject.GetComponentInChildren<TextMeshProUGUI>(true) : null);
		TextMeshProUGUI val3 = (IsAlive((Object)(object)_pauseButtonObject) ? _pauseButtonObject.GetComponentInChildren<TextMeshProUGUI>(true) : null);
		RectTransform val4 = (IsAlive((Object)(object)_pickerToggleObject) ? _pickerToggleObject.GetComponent<RectTransform>() : null);
		object obj;
		if (!IsAlive((Object)(object)_pickerToggleObject))
		{
			obj = null;
		}
		else
		{
			Transform parent = _pickerToggleObject.transform.parent;
			obj = ((parent != null) ? parent.Find("XIAOWO_FreeBuildToggle") : null);
		}
		Transform val5 = (Transform)obj;
		RectTransform val6 = (IsAlive((Object)(object)val5) ? ((Component)val5).GetComponent<RectTransform>() : null);
		return "PRECISION_BAY_MODE\nsaveKey=xiaowo_trade_precision_bay_enabled\nrun=" + ((instance != null) ? instance.runID : null) + "\nstored=" + HasStoredState(instance) + "\nenabled=" + IsUpgradeEnabled(instance) + "\npickerPresent=" + IsAlive((Object)(object)_pickerToggleObject) + "\npickerText=" + (((val2 != null) ? ((TMP_Text)val2).text : null) ?? "missing") + "\npickerX=" + (IsAlive((Object)(object)val4) ? val4.anchoredPosition.x : float.NaN) + "\npickerY=" + (IsAlive((Object)(object)val4) ? val4.anchoredPosition.y : float.NaN) + "\nfreeBuildX=" + (IsAlive((Object)(object)val6) ? val6.anchoredPosition.x : float.NaN) + "\nfreeBuildY=" + (IsAlive((Object)(object)val6) ? val6.anchoredPosition.y : float.NaN) + "\npickerRightOfFreeBuild=" + (IsAlive((Object)(object)val4) && IsAlive((Object)(object)val6) && val4.anchoredPosition.x > val6.anchoredPosition.x && Math.Abs(val4.anchoredPosition.y - val6.anchoredPosition.y) < 1f) + "\npauseButtonPresent=" + IsAlive((Object)(object)_pauseButtonObject) + "\npauseButtonText=" + (((val3 != null) ? ((TMP_Text)val3).text : null) ?? "missing") + "\nwindowOpen=" + (val != null && val.IsVisible);
	}

	internal static void RefreshLocalizedText()
	{
		TextMeshProUGUI val = (IsAlive((Object)(object)_pickerToggleObject) ? _pickerToggleObject.GetComponentInChildren<TextMeshProUGUI>(true) : null);
		if (IsAlive((Object)(object)val))
		{
			((TMP_Text)val).text = PickerText;
		}
		TextMeshProUGUI val2 = (IsAlive((Object)(object)_pauseButtonObject) ? _pauseButtonObject.GetComponentInChildren<TextMeshProUGUI>(true) : null);
		if (IsAlive((Object)(object)val2))
		{
			((TMP_Text)val2).text = ModSettingsText;
		}
	}

	internal static void OpenPauseSettingsForTest()
	{
		OpenPauseSettings();
	}

	internal static void Dispose()
	{
		CustomUIManager instance = CustomUIManager.Instance;
		if (IsAlive((Object)(object)instance))
		{
			instance.CloseWindow("xiaowo_save_mod_settings");
		}
		if (IsAlive((Object)(object)_pickerToggleObject))
		{
			Object.Destroy((Object)(object)_pickerToggleObject);
		}
		if (IsAlive((Object)(object)_pauseButtonObject))
		{
			Object.Destroy((Object)(object)_pauseButtonObject);
		}
		_pickerToggleObject = null;
		_pickerToggle = null;
		_pickerChangedAction = null;
		_pauseButtonObject = null;
		_pauseButton = null;
		_openPauseAction = null;
		_saveToggleAction = null;
		_closeAction = null;
		_escapeManager = null;
		_newGameEnabled = true;
		_checkCountdown = 0;
		_lastFailure = string.Empty;
	}

	private static void EnsurePickerToggle(PerkUIController ui)
	{
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		object obj;
		if (ui == null)
		{
			obj = null;
		}
		else
		{
			Button startButton = ui.startButton;
			obj = ((startButton != null) ? ((Component)startButton).GetComponent<RectTransform>() : null);
		}
		RectTransform val = (RectTransform)obj;
		Transform val2 = ((val != null) ? ((Transform)val).parent : null);
		if (!IsAlive((Object)(object)val) || !IsAlive((Object)(object)val2))
		{
			return;
		}
		if (!IsAlive((Object)(object)_pickerToggleObject) || (Object)(object)_pickerToggleObject.transform.parent != (Object)(object)val2)
		{
			if (IsAlive((Object)(object)_pickerToggleObject))
			{
				Object.Destroy((Object)(object)_pickerToggleObject);
			}
			CustomUIManager instance = CustomUIManager.Instance;
			GameObject val3 = ((instance != null) ? instance.togglePrefab : null);
			if (!IsAlive((Object)(object)val3))
			{
				return;
			}
			_pickerToggleObject = Object.Instantiate<GameObject>(val3, val2, false);
			((Object)_pickerToggleObject).name = "XIAOWO_PrecisionBayToggle";
			_pickerToggle = _pickerToggleObject.GetComponent<Toggle>() ?? _pickerToggleObject.GetComponentInChildren<Toggle>(true);
			RectTransform component = _pickerToggleObject.GetComponent<RectTransform>();
			TextMeshProUGUI componentInChildren = _pickerToggleObject.GetComponentInChildren<TextMeshProUGUI>(true);
			if (!IsAlive((Object)(object)_pickerToggle) || !IsAlive((Object)(object)component) || !IsAlive((Object)(object)componentInChildren))
			{
				Object.Destroy((Object)(object)_pickerToggleObject);
				_pickerToggleObject = null;
				_pickerToggle = null;
				return;
			}
			((TMP_Text)componentInChildren).fontSize = Math.Min(((TMP_Text)componentInChildren).fontSize, 18f);
			((TMP_Text)componentInChildren).enableWordWrapping = false;
			((TMP_Text)componentInChildren).overflowMode = (TextOverflowModes)0;
			_pickerToggle.onValueChanged = new ToggleEvent();
			if (_pickerChangedAction == null)
			{
				_pickerChangedAction = UnityAction<bool>.op_Implicit((Action<bool>)delegate(bool enabled)
				{
					_newGameEnabled = enabled;
				});
			}
			((UnityEvent<bool>)(object)_pickerToggle.onValueChanged).AddListener(_pickerChangedAction);
		}
		FreeBuildMode.ApplyPickerToggleLayout(_pickerToggleObject.GetComponent<RectTransform>(), val, placeOnRight: true);
		TextMeshProUGUI componentInChildren2 = _pickerToggleObject.GetComponentInChildren<TextMeshProUGUI>(true);
		if (IsAlive((Object)(object)componentInChildren2))
		{
			((TMP_Text)componentInChildren2).text = PickerText;
		}
		_pickerToggle.SetIsOnWithoutNotify(_newGameEnabled);
		_pickerToggleObject.SetActive(true);
	}

	private static void OpenPauseSettings()
	{
		try
		{
			EscapeUIManager escapeManager = _escapeManager;
			if (escapeManager != null)
			{
				escapeManager.OnAnyClick();
			}
			CustomUIManager instance = CustomUIManager.Instance;
			if (!IsAlive((Object)(object)instance))
			{
				throw new InvalidOperationException("Custom UI manager is unavailable.");
			}
			instance.CloseWindow("xiaowo_save_mod_settings");
			CustomUIBuilder val = CustomUIManager.Window("xiaowo_save_mod_settings", ModSettingsText, "overlay").SetSize(600f, 260f).Center()
				.SetCloseOnEscape(true)
				.SetDraggable(false);
			val.AddLabel(DescriptionText, "summary");
			val.Last.SetPreferredSize(560f, 72f);
			if ((Object)(object)val.Last.text != (Object)null)
			{
				((TMP_Text)val.Last.text).fontSize = 16f;
				((TMP_Text)val.Last.text).enableWordWrapping = true;
			}
			if (_saveToggleAction == null)
			{
				_saveToggleAction = DelegateSupport.ConvertDelegate<Action<bool>>((Delegate)(Action<bool>)delegate(bool enabled)
				{
					SetForCurrentSave(enabled, saveImmediately: true);
				});
			}
			val.AddToggle(CurrentSaveToggleText, IsUpgradeEnabled(), _saveToggleAction, "precision_bay_enabled");
			val.Last.SetPreferredSize(540f, 42f);
			if (_manageModsAction == null)
			{
				_manageModsAction = DelegateSupport.ConvertDelegate<Action>((Delegate)(Action)delegate
				{
					CloseWindow();
					ModManagerUI.OpenInGame();
				});
			}
			val.AddButton(ModLocalization.Select("管理 MOD", "MANAGE MODS"), _manageModsAction, "manage_mods");
			val.Last.SetPreferredSize(220f, 42f);
			if (_closeAction == null)
			{
				_closeAction = DelegateSupport.ConvertDelegate<Action>((Delegate)new Action(CloseWindow));
			}
			val.AddButton(ModLocalization.Select("关闭", "CLOSE"), _closeAction, "close");
			val.Last.SetPreferredSize(180f, 42f);
			val.Show();
		}
		catch (Exception ex)
		{
			Instance log = Core.Log;
			if (log != null)
			{
				log.Warning("Save MOD settings window could not be opened: " + ex);
			}
		}
	}

	private static void CloseWindow()
	{
		CustomUIManager instance = CustomUIManager.Instance;
		if (IsAlive((Object)(object)instance))
		{
			instance.CloseWindow("xiaowo_save_mod_settings");
		}
	}

	private static GameObject FindNativeButton(Transform root, string methodName)
	{
		if (!IsAlive((Object)(object)root))
		{
			return null;
		}
		foreach (Button componentsInChild in ((Component)root).gameObject.GetComponentsInChildren<Button>(true))
		{
			if (!IsAlive((Object)(object)componentsInChild) || string.Equals(((Object)((Component)componentsInChild).gameObject).name, "XIAOWO_ModSettingsButton", StringComparison.Ordinal))
			{
				continue;
			}
			for (int i = 0; i < ((UnityEventBase)componentsInChild.onClick).GetPersistentEventCount(); i++)
			{
				if (string.Equals(((UnityEventBase)componentsInChild.onClick).GetPersistentMethodName(i), methodName, StringComparison.Ordinal))
				{
					return ((Component)componentsInChild).gameObject;
				}
			}
		}
		return null;
	}

	private static void SetStoredState(PlayerStore store, bool enabled)
	{
		if (store.modData == null)
		{
			store.modData = new Dictionary<string, string>();
		}
		store.modData["xiaowo_trade_precision_bay_enabled"] = (enabled ? "1" : "0");
	}

	private static bool HasStoredState(PlayerStore store)
	{
		if (((store != null) ? store.modData : null) != null)
		{
			return store.modData.ContainsKey("xiaowo_trade_precision_bay_enabled");
		}
		return false;
	}

	private static bool IsAlive(Object value)
	{
		if (value != (Object)null)
		{
			return Object.op_Implicit(value);
		}
		return false;
	}
}
