using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace XIAOWOTradePerks;

internal static class LoadingProgressOverlay
{
	internal const string RootName = "XIAOWOTradePerksLoadingProgress";

	private const float BarWidth = 680f;

	private const float CompletionHoldSeconds = 1.5f;

	private static readonly Color FrameColor = new Color(48f / 85f, 67f / 85f, 0.77254903f, 1f);

	private static readonly Color TrackColor = new Color(13f / 85f, 0.12156863f, 0.19215687f, 0.96f);

	private static readonly List<string> Trace = new List<string>();

	private static GameObject _root;

	private static RectTransform _fillRect;

	private static TextMeshProUGUI _statusLabel;

	private static Sprite _solidSprite;

	private static bool _active;

	private static bool _completed;

	private static int _slotId = -1;

	private static int _percent;

	private static string _stage = string.Empty;

	private static DateTime _hideAtUtc;

	internal static bool IsActive => _active;

	internal static bool IsVisible
	{
		get
		{
			if (IsAlive((Object)(object)_root))
			{
				return _root.activeInHierarchy;
			}
			return false;
		}
	}

	internal static int ProgressPercent => _percent;

	internal static void Tick()
	{
		if (_active)
		{
			EnsureRoot();
			ApplyVisuals();
		}
		if (_completed && _active && !(DateTime.UtcNow < _hideAtUtc))
		{
			_active = false;
			if (IsAlive((Object)(object)_root))
			{
				_root.SetActive(false);
			}
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Save loading progress hidden after completion.");
			}
		}
	}

	internal static void Begin(int slotId)
	{
		Trace.Clear();
		_slotId = slotId;
		_active = true;
		_completed = false;
		_hideAtUtc = DateTime.MinValue;
		SetStage(10, Select("已选择存档", "Save selected"));
		Instance log = Core.Log;
		if (log != null)
		{
			log.Msg("Save loading progress started for slot " + slotId + ".");
		}
	}

	internal static void Advance(int percent, string chinese, string english)
	{
		if (_active && !_completed)
		{
			SetStage(percent, Select(chinese, english));
		}
	}

	internal static void Complete()
	{
		if (_active)
		{
			SetStage(100, Select("载入完成", "Load complete"));
			_completed = true;
			_hideAtUtc = DateTime.UtcNow.AddSeconds(1.5);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Save loading progress completed for slot " + _slotId + ".");
			}
		}
	}

	internal static string BuildStatus()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Invalid comparison between Unknown and I4
		float num = (IsAlive((Object)(object)_fillRect) ? _fillRect.anchorMax.x : (-1f));
		Canvas val = (IsAlive((Object)(object)_root) ? _root.GetComponent<Canvas>() : null);
		return "LOADING_PROGRESS\nactive=" + _active + "\nvisible=" + IsVisible + "\ncompleted=" + _completed + "\nslot=" + _slotId + "\npercent=" + _percent + "\nstage=" + _stage + "\nfillRatio=" + num.ToString("0.###") + "\noverlayCanvas=" + (IsAlive((Object)(object)val) && (int)val.renderMode == 0) + "\nraycastBlocked=" + IsRaycastBlocked() + "\ntrace=" + string.Join("|", Trace);
	}

	internal static void Dispose()
	{
		if (IsAlive((Object)(object)_root))
		{
			Object.Destroy((Object)(object)_root);
		}
		_root = null;
		_fillRect = null;
		_statusLabel = null;
		if (IsAlive((Object)(object)_solidSprite))
		{
			Object.Destroy((Object)(object)_solidSprite);
		}
		_solidSprite = null;
		_active = false;
		_completed = false;
		_slotId = -1;
		_percent = 0;
		_stage = string.Empty;
		_hideAtUtc = DateTime.MinValue;
		Trace.Clear();
	}

	private static void SetStage(int percent, string stage)
	{
		_percent = Math.Max(_percent, Math.Clamp(percent, 0, 100));
		_stage = stage ?? string.Empty;
		Trace.Add(_percent + ":" + _stage);
		EnsureRoot();
		ApplyVisuals();
		Instance log = Core.Log;
		if (log != null)
		{
			log.Msg("Save loading progress: " + _percent + "% " + _stage);
		}
	}

	private static void EnsureRoot()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		if (IsAlive((Object)(object)_root))
		{
			if (!_root.activeSelf)
			{
				_root.SetActive(true);
			}
			return;
		}
		CustomUIManager instance = CustomUIManager.Instance;
		if (IsAlive((Object)(object)((instance != null) ? instance.labelPrefab : null)))
		{
			_root = new GameObject("XIAOWOTradePerksLoadingProgress", (Type[])(object)new Type[3]
			{
				Il2CppType.Of<RectTransform>(),
				Il2CppType.Of<Canvas>(),
				Il2CppType.Of<CanvasScaler>()
			});
			Object.DontDestroyOnLoad((Object)(object)_root);
			Canvas component = _root.GetComponent<Canvas>();
			component.renderMode = (RenderMode)0;
			component.overrideSorting = true;
			component.sortingOrder = 32767;
			CanvasScaler component2 = _root.GetComponent<CanvasScaler>();
			component2.uiScaleMode = (ScaleMode)1;
			component2.referenceResolution = new Vector2(1920f, 1080f);
			component2.matchWidthOrHeight = 0.5f;
			Image val = CreateImage(_root.transform, "LoadingProgressPanel", new Color(0f, 0f, 0f, 0.38f));
			SetBottomCenter(((Graphic)val).rectTransform, 0f, 26f, 712f, 72f);
			_statusLabel = CreateLabel(instance, ((Graphic)val).rectTransform);
			SetBottomCenter(((TMP_Text)_statusLabel).rectTransform, 0f, 34f, 680f, 26f);
			Image obj = CreateImage(((Component)val).transform, "LoadingProgressFrame", FrameColor);
			SetBottomCenter(((Graphic)obj).rectTransform, 0f, 10f, 684f, 20f);
			Image obj2 = CreateImage(((Component)obj).transform, "LoadingProgressTrack", TrackColor);
			Stretch(((Graphic)obj2).rectTransform, 2f);
			_fillRect = ((Graphic)CreateImage(((Component)obj2).transform, "LoadingProgressFill", FrameColor)).rectTransform;
			_fillRect.anchorMin = Vector2.zero;
			_fillRect.anchorMax = new Vector2(0f, 1f);
			_fillRect.pivot = new Vector2(0f, 0.5f);
			_fillRect.offsetMin = Vector2.zero;
			_fillRect.offsetMax = Vector2.zero;
			((Transform)_fillRect).localScale = Vector3.one;
			_root.SetActive(true);
		}
	}

	private static void ApplyVisuals()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (IsAlive((Object)(object)_root))
		{
			_root.SetActive(_active);
			if (IsAlive((Object)(object)_fillRect))
			{
				_fillRect.anchorMax = new Vector2((float)_percent / 100f, 1f);
				_fillRect.offsetMin = Vector2.zero;
				_fillRect.offsetMax = Vector2.zero;
			}
			if (IsAlive((Object)(object)_statusLabel))
			{
				((TMP_Text)_statusLabel).text = _stage + "  " + _percent + "%";
			}
			Canvas.ForceUpdateCanvases();
		}
	}

	private static Image CreateImage(Transform parent, string name, Color color)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name, (Type[])(object)new Type[3]
		{
			Il2CppType.Of<RectTransform>(),
			Il2CppType.Of<CanvasRenderer>(),
			Il2CppType.Of<Image>()
		});
		val.transform.SetParent(parent, false);
		Image component = val.GetComponent<Image>();
		component.sprite = GetSolidSprite();
		component.type = (Type)0;
		((Graphic)component).color = color;
		((Graphic)component).raycastTarget = false;
		return component;
	}

	private static TextMeshProUGUI CreateLabel(CustomUIManager manager, RectTransform parent)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(manager.labelPrefab, (Transform)(object)parent, false);
		((Object)val).name = "LoadingProgressStatus";
		TextMeshProUGUI componentInChildren = val.GetComponentInChildren<TextMeshProUGUI>(true);
		if (!IsAlive((Object)(object)componentInChildren))
		{
			throw new InvalidOperationException("Loading progress label prefab is incomplete.");
		}
		((Graphic)componentInChildren).color = Color.white;
		((TMP_Text)componentInChildren).fontSize = 16f;
		((TMP_Text)componentInChildren).fontStyle = (FontStyles)1;
		((TMP_Text)componentInChildren).alignment = (TextAlignmentOptions)514;
		((TMP_Text)componentInChildren).enableWordWrapping = false;
		((Graphic)componentInChildren).raycastTarget = false;
		val.SetActive(true);
		return componentInChildren;
	}

	private static Sprite GetSolidSprite()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (IsAlive((Object)(object)_solidSprite))
		{
			return _solidSprite;
		}
		Texture2D whiteTexture = Texture2D.whiteTexture;
		_solidSprite = Sprite.Create(whiteTexture, new Rect(0f, 0f, (float)((Texture)whiteTexture).width, (float)((Texture)whiteTexture).height), new Vector2(0.5f, 0.5f), 100f);
		((Object)_solidSprite).name = "xiaowo_loading_solid";
		((Object)_solidSprite).hideFlags = (HideFlags)61;
		return _solidSprite;
	}

	private static void SetBottomCenter(RectTransform rect, float x, float y, float width, float height)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		rect.anchorMin = new Vector2(0.5f, 0f);
		rect.anchorMax = new Vector2(0.5f, 0f);
		rect.pivot = new Vector2(0.5f, 0f);
		rect.anchoredPosition = new Vector2(x, y);
		rect.sizeDelta = new Vector2(width, height);
		((Transform)rect).localScale = Vector3.one;
	}

	private static void Stretch(RectTransform rect, float inset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.offsetMin = new Vector2(inset, inset);
		rect.offsetMax = new Vector2(0f - inset, 0f - inset);
		((Transform)rect).localScale = Vector3.one;
	}

	private static bool IsRaycastBlocked()
	{
		if (!IsAlive((Object)(object)_root))
		{
			return false;
		}
		foreach (Graphic componentsInChild in _root.GetComponentsInChildren<Graphic>(true))
		{
			if (IsAlive((Object)(object)componentsInChild) && componentsInChild.raycastTarget)
			{
				return true;
			}
		}
		return false;
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
}
