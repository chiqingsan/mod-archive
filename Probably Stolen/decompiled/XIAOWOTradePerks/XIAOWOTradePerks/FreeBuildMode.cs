using System;
using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XIAOWOTradePerks;

internal static class FreeBuildMode
{
	internal const string ToggleName = "XIAOWO_FreeBuildToggle";

	internal const int PointLimit = 99;

	internal const int PerkLimit = 99;

	private const float PickerToggleMinimumWidth = 220f;

	private const float PickerToggleGap = 12f;

	private static GameObject _toggleObject;

	private static Toggle _toggle;

	private static UnityAction<bool> _changedAction;

	private static bool _enabled;

	private static int _nativePointLimit;

	private static int _nativeBasePerkLimit;

	private static bool _nativeLimitsCaptured;

	internal static bool IsEnabled => _enabled;

	private static string LabelText => ModLocalization.Select("无上限特性点数", "Unlimited Perk Points");

	internal static void BeforeOpen(PerkUIController ui)
	{
		if (_enabled)
		{
			ApplyLimits(ui);
		}
	}

	internal static void AfterOpen(PerkUIController ui)
	{
		if (!((Object)(object)ui == (Object)null))
		{
			if (!_enabled)
			{
				CaptureNativeLimits(ui);
			}
			EnsureToggle(ui);
			ApplyLimits(ui);
			ui.OnChange();
			PerkRegistry.EnsurePicker(ui);
		}
	}

	internal static void ApplyLimits(PerkUIController ui)
	{
		if (_enabled && !((Object)(object)ui == (Object)null))
		{
			ui.maxPerkPoint = 99;
			ui.maxPerkCount = 99;
		}
	}

	internal static void RefreshLocalizedText()
	{
		TextMeshProUGUI val = (IsAlive((Object)(object)_toggleObject) ? _toggleObject.GetComponentInChildren<TextMeshProUGUI>(true) : null);
		if (IsAlive((Object)(object)val))
		{
			((TMP_Text)val).text = LabelText;
		}
	}

	internal static string BuildStatus(PerkUIController ui)
	{
		TextMeshProUGUI val = (IsAlive((Object)(object)_toggleObject) ? _toggleObject.GetComponentInChildren<TextMeshProUGUI>(true) : null);
		string[] obj = new string[18]
		{
			"FREE_BUILD\npresent=",
			IsAlive((Object)(object)_toggleObject).ToString(),
			"\nenabled=",
			_enabled.ToString(),
			"\nlabel=",
			IsAlive((Object)(object)val) ? ((TMP_Text)val).text : "missing",
			"\npoint=",
			((ui != null) ? ui.currentPerkPoint : (-1)).ToString(),
			"/",
			((ui != null) ? ui.maxPerkPoint : (-1)).ToString(),
			"\nperks=",
			((ui != null) ? ui.currentPerkCount : (-1)).ToString(),
			"/",
			((ui != null) ? ui.maxPerkCount : (-1)).ToString(),
			"\npointText=",
			null,
			null,
			null
		};
		object obj2;
		if (ui == null)
		{
			obj2 = null;
		}
		else
		{
			TextMeshProUGUI pointCount = ui.pointCount;
			obj2 = ((pointCount != null) ? ((TMP_Text)pointCount).text : null);
		}
		if (obj2 == null)
		{
			obj2 = "missing";
		}
		obj[15] = (string)obj2;
		obj[16] = "\nperkText=";
		object obj3;
		if (ui == null)
		{
			obj3 = null;
		}
		else
		{
			TextMeshProUGUI slotCounter = ui.slotCounter;
			obj3 = ((slotCounter != null) ? ((TMP_Text)slotCounter).text : null);
		}
		if (obj3 == null)
		{
			obj3 = "missing";
		}
		obj[17] = (string)obj3;
		return string.Concat(obj);
	}

	internal static void SetEnabledForTest(PerkUIController ui, bool enabled)
	{
		SetEnabled(ui, enabled);
	}

	internal static void Dispose()
	{
		if (IsAlive((Object)(object)_toggleObject))
		{
			Object.Destroy((Object)(object)_toggleObject);
		}
		_toggleObject = null;
		_toggle = null;
		_changedAction = null;
		_enabled = false;
		_nativeLimitsCaptured = false;
	}

	private static void EnsureToggle(PerkUIController ui)
	{
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
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
		if ((Object)(object)val == (Object)null || (Object)(object)val2 == (Object)null)
		{
			return;
		}
		if (IsAlive((Object)(object)_toggleObject) && (Object)(object)_toggleObject.transform.parent == (Object)(object)val2)
		{
			ApplyPickerToggleLayout(_toggleObject.GetComponent<RectTransform>(), val, placeOnRight: false);
			_toggle.SetIsOnWithoutNotify(_enabled);
			RefreshLocalizedText();
			return;
		}
		if (IsAlive((Object)(object)_toggleObject))
		{
			Object.Destroy((Object)(object)_toggleObject);
		}
		CustomUIManager instance = CustomUIManager.Instance;
		GameObject val3 = ((instance != null) ? instance.togglePrefab : null);
		if (!IsAlive((Object)(object)val3))
		{
			return;
		}
		_toggleObject = Object.Instantiate<GameObject>(val3, val2, false);
		((Object)_toggleObject).name = "XIAOWO_FreeBuildToggle";
		_toggle = _toggleObject.GetComponent<Toggle>() ?? _toggleObject.GetComponentInChildren<Toggle>(true);
		RectTransform component = _toggleObject.GetComponent<RectTransform>();
		TextMeshProUGUI componentInChildren = _toggleObject.GetComponentInChildren<TextMeshProUGUI>(true);
		if (!IsAlive((Object)(object)_toggle) || !IsAlive((Object)(object)component) || !IsAlive((Object)(object)componentInChildren))
		{
			Object.Destroy((Object)(object)_toggleObject);
			_toggleObject = null;
			_toggle = null;
			return;
		}
		ApplyPickerToggleLayout(component, val, placeOnRight: false);
		((TMP_Text)componentInChildren).text = LabelText;
		((TMP_Text)componentInChildren).fontSize = Math.Min(((TMP_Text)componentInChildren).fontSize, 18f);
		((TMP_Text)componentInChildren).enableWordWrapping = false;
		((TMP_Text)componentInChildren).overflowMode = (TextOverflowModes)0;
		_toggle.onValueChanged = new ToggleEvent();
		if (_changedAction == null)
		{
			_changedAction = UnityAction<bool>.op_Implicit((Action<bool>)delegate(bool enabled)
			{
				SetEnabled(PerkUIController.Instance, enabled);
			});
		}
		((UnityEvent<bool>)(object)_toggle.onValueChanged).AddListener(_changedAction);
		_toggle.SetIsOnWithoutNotify(_enabled);
		_toggleObject.SetActive(true);
	}

	private static void SetEnabled(PerkUIController ui, bool enabled)
	{
		if (!((Object)(object)ui == (Object)null))
		{
			if (!_enabled && enabled)
			{
				CaptureNativeLimits(ui);
			}
			_enabled = enabled;
			if (_enabled)
			{
				ApplyLimits(ui);
			}
			else if (_nativeLimitsCaptured)
			{
				ui.maxPerkPoint = _nativePointLimit;
				ui.maxPerkCount = _nativeBasePerkLimit + SelectedExtraSlots(ui);
			}
			if (IsAlive((Object)(object)_toggle))
			{
				_toggle.SetIsOnWithoutNotify(_enabled);
			}
			ui.OnChange();
			PerkRegistry.EnsurePicker(ui);
		}
	}

	private static void CaptureNativeLimits(PerkUIController ui)
	{
		if (!((Object)(object)ui == (Object)null))
		{
			_nativePointLimit = ui.maxPerkPoint;
			_nativeBasePerkLimit = Math.Max(0, ui.maxPerkCount - SelectedExtraSlots(ui));
			_nativeLimitsCaptured = true;
		}
	}

	private static int SelectedExtraSlots(PerkUIController ui)
	{
		if ((Object)(object)((ui != null) ? ui.selectedPerks : null) == (Object)null)
		{
			return 0;
		}
		int num = 0;
		foreach (StartingPerkElement componentsInChild in ui.selectedPerks.GetComponentsInChildren<StartingPerkElement>(true))
		{
			if (((componentsInChild != null) ? componentsInChild.perk : null) != null)
			{
				num += componentsInChild.perk.maxSlot;
			}
		}
		return num;
	}

	internal static void ApplyPickerToggleLayout(RectTransform rect, RectTransform startRect, bool placeOnRight)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (IsAlive((Object)(object)rect) && IsAlive((Object)(object)startRect))
		{
			Rect rect2 = startRect.rect;
			float num = Math.Max(220f, ((Rect)(ref rect2)).width);
			rect2 = startRect.rect;
			float num2 = Math.Max(34f, ((Rect)(ref rect2)).height + 8f);
			float num3 = (num + 12f) * 0.5f;
			rect.anchorMin = startRect.anchorMin;
			rect.anchorMax = startRect.anchorMax;
			rect.pivot = startRect.pivot;
			rect.anchoredPosition = startRect.anchoredPosition + new Vector2(placeOnRight ? num3 : (0f - num3), num2);
			rect.sizeDelta = new Vector2(num, 30f);
			((Transform)rect).localScale = Vector3.one;
		}
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
