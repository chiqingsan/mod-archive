using System;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppSystem;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XIAOWOTradePerks;

internal static class CommunityOverlay
{
	internal const string RootName = "XIAOWOTradePerksCommunityOverlay";

	internal const string FollowButtonName = "XIAOWOTradePerksDouyinButton";

	internal const string FollowButtonFillName = "XIAOWOTradePerksDouyinButtonFill";

	internal const string FollowButtonText = "关注遗忘的小窝";

	internal const string DouyinUrl = "https://www.douyin.com/user/MS4wLjABAAAAdx49FthxYEyfr8TYeWa2AhwI2WSm_mLWTTSgpS9dTBQ";

	internal static readonly string ModTitle = "XIAOWO-TradePerks v" + (typeof(Core).Assembly.GetName().Version?.ToString(3) ?? "unknown");

	internal const string AuthorText = "作者：遗忘的小窝";

	internal const string QqText = "QQ群号：611790510";

	internal const string DouyinText = "抖音搜索：遗忘的小窝";

	internal const float ContentWidth = 244f;

	private static GameObject _root;

	private static GameObject _followButton;

	private static Sprite _solidSprite;

	private static UnityAction _openDouyinAction;

	private static int _checkCountdown;

	private static string _lastFailure = string.Empty;

	private static readonly Color FollowFillColor = new Color(0.16078432f, 2f / 15f, 0.20392157f, 1f);

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

	internal static void Tick()
	{
		if (_checkCountdown-- > 0)
		{
			return;
		}
		_checkCountdown = 30;
		try
		{
			RectTransform val = ResolveMainMenuRoot();
			if (!IsAlive((Object)(object)val))
			{
				if (IsAlive((Object)(object)_root))
				{
					Instance log = Core.Log;
					if (log != null)
					{
						log.Msg("Community text hidden outside the main menu.");
					}
				}
				DestroyRoot();
				return;
			}
			if (IsAlive((Object)(object)_root))
			{
				RectTransform component = _root.GetComponent<RectTransform>();
				if (IsAlive((Object)(object)component) && (Object)(object)((Transform)component).parent == (Object)(object)val)
				{
					EnsureFollowButton(MainMenuUIController.Instance);
					return;
				}
				DestroyRoot();
			}
			Build(val);
			EnsureFollowButton(MainMenuUIController.Instance);
			_lastFailure = string.Empty;
			Instance log2 = Core.Log;
			if (log2 != null)
			{
				log2.Msg("Community panel attached to the main menu UI layer.");
			}
		}
		catch (Exception ex)
		{
			DestroyRoot();
			string text = ex.GetType().Name + ": " + ex.Message;
			if (!string.Equals(text, _lastFailure, StringComparison.Ordinal))
			{
				_lastFailure = text;
				Instance log3 = Core.Log;
				if (log3 != null)
				{
					log3.Warning("Community panel could not be created: " + text);
				}
			}
		}
	}

	internal static void Dispose()
	{
		DestroyRoot();
		_lastFailure = string.Empty;
		_checkCountdown = 0;
	}

	internal static bool ValidateDefinition(out string details)
	{
		bool result = ModTitle == "XIAOWO-TradePerks v1.7.27";
		details = "text=" + result + ", width=" + 244f + ", followButton=关注遗忘的小窝";
		return result;
	}

	internal static string BuildStatus()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		ValidateDefinition(out var details);
		if (!IsAlive((Object)(object)_root))
		{
			return "COMMUNITY\nvisible=False\n" + details;
		}
		RectTransform component = _root.GetComponent<RectTransform>();
		TextMeshProUGUI val = FindLabel("CommunityModTitle");
		TextMeshProUGUI val2 = FindLabel("CommunityAuthor");
		TextMeshProUGUI label = FindLabel("CommunityQqText");
		TextMeshProUGUI label2 = FindLabel("CommunityDouyinText");
		bool flag = IsPureWhite(val) && IsPureWhite(val2) && IsPureWhite(label) && IsPureWhite(label2);
		bool flag2 = IsAlive((Object)(object)val) && (((TMP_Text)val).fontStyle & 1) != 0 && IsAlive((Object)(object)val2) && ((TMP_Text)val).fontSize > ((TMP_Text)val2).fontSize;
		RectTransform val3 = (IsAlive((Object)(object)_followButton) ? _followButton.GetComponent<RectTransform>() : null);
		TextMeshProUGUI val4 = (IsAlive((Object)(object)_followButton) ? _followButton.GetComponentInChildren<TextMeshProUGUI>(true) : null);
		Image val5 = FindFollowIcon();
		Image image = (IsAlive((Object)(object)_followButton) ? _followButton.GetComponent<Image>() : null);
		Transform val6 = (IsAlive((Object)(object)_followButton) ? _followButton.transform.Find("XIAOWOTradePerksDouyinButtonFill") : null);
		Image image2 = (IsAlive((Object)(object)val6) ? ((Component)val6).gameObject.GetComponent<Image>() : null);
		Button val7 = (IsAlive((Object)(object)_followButton) ? _followButton.GetComponent<Button>() : null);
		string[] obj = new string[36]
		{
			"COMMUNITY\nvisible=",
			IsVisible.ToString(),
			"\nmainMenu=",
			IsAlive((Object)(object)MainMenuUIController.Instance).ToString(),
			"\ngameplay=",
			(StoreStation.instance != null).ToString(),
			"\nparent=",
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
		Transform parent = _root.transform.parent;
		obj[7] = ((parent != null) ? ((Object)parent).name : null) ?? "none";
		obj[8] = "\nposition=";
		object obj2;
		Vector2 val8;
		if (!IsAlive((Object)(object)component))
		{
			obj2 = "missing";
		}
		else
		{
			val8 = component.anchoredPosition;
			obj2 = ((object)(Vector2)(ref val8)).ToString();
		}
		obj[9] = (string)obj2;
		obj[10] = "\nsize=";
		object obj3;
		if (!IsAlive((Object)(object)component))
		{
			obj3 = "missing";
		}
		else
		{
			val8 = component.sizeDelta;
			obj3 = ((object)(Vector2)(ref val8)).ToString();
		}
		obj[11] = (string)obj3;
		obj[12] = "\nallTextPureWhite=";
		obj[13] = flag.ToString();
		obj[14] = "\ntitleBoldAndLarger=";
		obj[15] = flag2.ToString();
		obj[16] = "\nfollowButtonVisible=";
		obj[17] = (IsAlive((Object)(object)_followButton) && _followButton.activeInHierarchy).ToString();
		obj[18] = "\nfollowButtonText=";
		obj[19] = ((val4 != null) ? ((TMP_Text)val4).text : null) ?? "missing";
		obj[20] = "\nfollowButtonSize=";
		object obj4;
		if (!IsAlive((Object)(object)val3))
		{
			obj4 = "missing";
		}
		else
		{
			val8 = val3.sizeDelta;
			obj4 = ((object)(Vector2)(ref val8)).ToString();
		}
		obj[21] = (string)obj4;
		obj[22] = "\nfollowIcon=";
		object obj5;
		if (!IsAlive((Object)(object)val5))
		{
			obj5 = "missing";
		}
		else
		{
			Sprite sprite = val5.sprite;
			obj5 = ((sprite != null) ? ((Object)sprite).name : null) ?? "missing";
		}
		obj[23] = (string)obj5;
		obj[24] = "\nfollowIconSize=";
		object obj6;
		if (!IsAlive((Object)(object)val5))
		{
			obj6 = "missing";
		}
		else
		{
			val8 = ((Graphic)val5).rectTransform.sizeDelta;
			obj6 = ((object)(Vector2)(ref val8)).ToString();
		}
		obj[25] = (string)obj6;
		obj[26] = "\nfollowFrameBlack=";
		obj[27] = IsColor(image, Color.black).ToString();
		obj[28] = "\nfollowFillOriginal=";
		obj[29] = IsColor(image2, FollowFillColor).ToString();
		obj[30] = "\nfollowTextWhite=";
		obj[31] = IsPureWhite(val4).ToString();
		obj[32] = "\nfollowPersistentListeners=";
		obj[33] = (IsAlive((Object)(object)val7) ? ((UnityEventBase)val7.onClick).GetPersistentEventCount() : (-1)).ToString();
		obj[34] = "\nfollowUrl=https://www.douyin.com/user/MS4wLjABAAAAdx49FthxYEyfr8TYeWa2AhwI2WSm_mLWTTSgpS9dTBQ\n";
		obj[35] = details;
		return string.Concat(obj);
	}

	private static TextMeshProUGUI FindLabel(string name)
	{
		Transform val = _root.transform.Find(name);
		if (!IsAlive((Object)(object)val))
		{
			return null;
		}
		return ((Component)val).gameObject.GetComponentInChildren<TextMeshProUGUI>(true);
	}

	private static bool IsPureWhite(TextMeshProUGUI label)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (IsAlive((Object)(object)label) && Math.Abs(((Graphic)label).color.r - 1f) < 0.001f && Math.Abs(((Graphic)label).color.g - 1f) < 0.001f && Math.Abs(((Graphic)label).color.b - 1f) < 0.001f)
		{
			return Math.Abs(((Graphic)label).color.a - 1f) < 0.001f;
		}
		return false;
	}

	private static bool IsColor(Image image, Color expected)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (IsAlive((Object)(object)image) && Math.Abs(((Graphic)image).color.r - expected.r) < 0.001f && Math.Abs(((Graphic)image).color.g - expected.g) < 0.001f && Math.Abs(((Graphic)image).color.b - expected.b) < 0.001f)
		{
			return Math.Abs(((Graphic)image).color.a - expected.a) < 0.001f;
		}
		return false;
	}

	private static RectTransform ResolveMainMenuRoot()
	{
		MainMenuUIController instance = MainMenuUIController.Instance;
		if (!IsAlive((Object)(object)instance) || !((Component)instance).gameObject.activeInHierarchy)
		{
			return null;
		}
		if (!IsAlive((Object)(object)instance.moddingButton) || !IsAlive((Object)(object)instance.moddingButton.transform.parent) || !((Component)instance.moddingButton.transform.parent).gameObject.activeInHierarchy)
		{
			return null;
		}
		if (!IsAlive((Object)(object)CustomUIManager.Instance) || CustomUIManager.anchors == null)
		{
			return null;
		}
		CustomUICanvasAnchor val = default(CustomUICanvasAnchor);
		if (!CustomUIManager.anchors.TryGetValue("menu", ref val) || !IsAlive((Object)(object)val))
		{
			return null;
		}
		return val.Root;
	}

	private static void Build(RectTransform parent)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		CustomUIManager instance = CustomUIManager.Instance;
		if (!IsAlive((Object)(object)((instance != null) ? instance.labelPrefab : null)))
		{
			throw new InvalidOperationException("Custom UI label prefab is unavailable.");
		}
		_root = new GameObject("XIAOWOTradePerksCommunityOverlay", (Type[])(object)new Type[1] { Il2CppType.Of<RectTransform>() });
		RectTransform component = _root.GetComponent<RectTransform>();
		((Transform)component).SetParent((Transform)(object)parent, false);
		SetTopLeft(component, 10f, -10f, 244f, 94f);
		CreateLabel(instance, component, "CommunityModTitle", "<b>" + ModTitle + "</b>", 17f, 0f, 0f, 244f, 24f);
		CreateLabel(instance, component, "CommunityAuthor", "作者：遗忘的小窝", 12f, 0f, -25f, 244f, 20f);
		CreateLabel(instance, component, "CommunityQqText", "QQ群号：611790510", 13f, 0f, -49f, 244f, 20f);
		CreateLabel(instance, component, "CommunityDouyinText", "抖音搜索：遗忘的小窝", 13f, 0f, -71f, 244f, 20f);
		((Transform)component).SetAsLastSibling();
		_root.SetActive(true);
	}

	private static void EnsureFollowButton(MainMenuUIController menu)
	{
		GameObject val = ((menu != null) ? menu.discordButton : null);
		Transform val2 = (IsAlive((Object)(object)val) ? val.transform.parent : null);
		if (!IsAlive((Object)(object)val) || !IsAlive((Object)(object)val2))
		{
			throw new InvalidOperationException("The original main-menu social button is unavailable.");
		}
		if (IsAlive((Object)(object)_followButton))
		{
			if ((Object)(object)_followButton.transform.parent == (Object)(object)val2)
			{
				ConfigureFollowButton();
				return;
			}
			Object.Destroy((Object)(object)_followButton);
			_followButton = null;
		}
		_followButton = Object.Instantiate<GameObject>(val, val2, false);
		((Object)_followButton).name = "XIAOWOTradePerksDouyinButton";
		_followButton.transform.SetAsLastSibling();
		ConfigureFollowButton();
		_followButton.SetActive(true);
	}

	private static void ConfigureFollowButton()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		Button obj = _followButton.GetComponent<Button>() ?? _followButton.GetComponentInChildren<Button>(true);
		TextMeshProUGUI componentInChildren = _followButton.GetComponentInChildren<TextMeshProUGUI>(true);
		Image val = FindFollowIcon();
		Sprite embeddedSprite = IconLoader.GetEmbeddedSprite("tiktok_logo.png", "ui.tiktok", (FilterMode)1);
		if (!IsAlive((Object)(object)obj) || !IsAlive((Object)(object)componentInChildren) || !IsAlive((Object)(object)val) || !IsAlive((Object)(object)embeddedSprite))
		{
			throw new InvalidOperationException("The cloned social button or TikTok icon is incomplete.");
		}
		((Object)embeddedSprite).name = "tiktok_logo";
		((TMP_Text)componentInChildren).text = "关注遗忘的小窝";
		((Graphic)componentInChildren).color = Color.white;
		((Object)((Component)val).gameObject).name = "TikTok";
		val.sprite = embeddedSprite;
		((Graphic)val).color = Color.white;
		val.preserveAspect = true;
		ConfigureFollowButtonFrame(obj);
		obj.onClick = new ButtonClickedEvent();
		if (_openDouyinAction == null)
		{
			_openDouyinAction = UnityAction.op_Implicit((Action)OpenDouyin);
		}
		((UnityEvent)obj.onClick).AddListener(_openDouyinAction);
	}

	private static void ConfigureFollowButtonFrame(Button button)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		Image component = _followButton.GetComponent<Image>();
		if (!IsAlive((Object)(object)component))
		{
			throw new InvalidOperationException("The cloned social button frame is missing.");
		}
		Sprite sprite = (component.sprite = GetSolidSprite());
		component.type = (Type)0;
		((Graphic)component).color = Color.black;
		((Selectable)button).targetGraphic = (Graphic)(object)component;
		Transform val = _followButton.transform.Find("XIAOWOTradePerksDouyinButtonFill");
		GameObject val2;
		if (IsAlive((Object)(object)val))
		{
			val2 = ((Component)val).gameObject;
		}
		else
		{
			val2 = new GameObject("XIAOWOTradePerksDouyinButtonFill", (Type[])(object)new Type[2]
			{
				Il2CppType.Of<RectTransform>(),
				Il2CppType.Of<Image>()
			});
			val2.transform.SetParent(_followButton.transform, false);
		}
		RectTransform component2 = val2.GetComponent<RectTransform>();
		Image component3 = val2.GetComponent<Image>();
		if (!IsAlive((Object)(object)component2) || !IsAlive((Object)(object)component3))
		{
			throw new InvalidOperationException("The follow button fill could not be created.");
		}
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		component2.pivot = new Vector2(0.5f, 0.5f);
		component2.offsetMin = new Vector2(2f, 2f);
		component2.offsetMax = new Vector2(-2f, -2f);
		((Transform)component2).localScale = Vector3.one;
		component3.sprite = sprite;
		component3.type = (Type)0;
		((Graphic)component3).color = FollowFillColor;
		((Graphic)component3).raycastTarget = false;
		val2.transform.SetAsFirstSibling();
		val2.SetActive(true);
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
		((Object)_solidSprite).name = "xiaowo_ui_solid";
		((Object)_solidSprite).hideFlags = (HideFlags)61;
		return _solidSprite;
	}

	private static Image FindFollowIcon()
	{
		if (!IsAlive((Object)(object)_followButton))
		{
			return null;
		}
		Transform val = _followButton.transform.Find("TikTok");
		if (IsAlive((Object)(object)val))
		{
			return ((Component)val).gameObject.GetComponent<Image>();
		}
		foreach (Image componentsInChild in _followButton.GetComponentsInChildren<Image>(true))
		{
			if (IsAlive((Object)(object)componentsInChild) && (Object)(object)((Component)componentsInChild).gameObject != (Object)(object)_followButton && ((Object)((Component)componentsInChild).gameObject).name != "XIAOWOTradePerksDouyinButtonFill")
			{
				return componentsInChild;
			}
		}
		return null;
	}

	private static void OpenDouyin()
	{
		try
		{
			MainMenuUIController instance = MainMenuUIController.Instance;
			if (instance != null)
			{
				instance.OnAnyButtonClick();
			}
			Application.OpenURL("https://www.douyin.com/user/MS4wLjABAAAAdx49FthxYEyfr8TYeWa2AhwI2WSm_mLWTTSgpS9dTBQ");
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Opened the Douyin follow page.");
			}
		}
		catch (Exception ex)
		{
			Instance log2 = Core.Log;
			if (log2 != null)
			{
				log2.Warning("Could not open the Douyin follow page: " + ex.Message);
			}
		}
	}

	private static void CreateLabel(CustomUIManager manager, RectTransform parent, string name, string text, float fontSize, float x, float y, float width, float height)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(manager.labelPrefab, (Transform)(object)parent, false);
		((Object)val).name = name;
		RectTransform component = val.GetComponent<RectTransform>();
		TextMeshProUGUI componentInChildren = val.GetComponentInChildren<TextMeshProUGUI>(true);
		if (!IsAlive((Object)(object)component) || !IsAlive((Object)(object)componentInChildren))
		{
			Object.Destroy((Object)(object)val);
			throw new InvalidOperationException("Custom UI label prefab is incomplete.");
		}
		SetTopLeft(component, x, y, width, height);
		((TMP_Text)componentInChildren).text = text;
		((Graphic)componentInChildren).color = Color.white;
		((TMP_Text)componentInChildren).fontSize = fontSize;
		((TMP_Text)componentInChildren).fontStyle = (FontStyles)(text.StartsWith("<b>", StringComparison.Ordinal) ? 1 : 0);
		((TMP_Text)componentInChildren).alignment = (TextAlignmentOptions)257;
		((TMP_Text)componentInChildren).enableWordWrapping = false;
		((TMP_Text)componentInChildren).richText = true;
		((Graphic)componentInChildren).raycastTarget = false;
		val.SetActive(true);
	}

	private static void SetTopLeft(RectTransform rect, float x, float y, float width, float height)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		rect.anchorMin = new Vector2(0f, 1f);
		rect.anchorMax = new Vector2(0f, 1f);
		rect.pivot = new Vector2(0f, 1f);
		rect.anchoredPosition = new Vector2(x, y);
		rect.sizeDelta = new Vector2(width, height);
		((Transform)rect).localScale = Vector3.one;
	}

	private static void DestroyRoot()
	{
		if (IsAlive((Object)(object)_followButton))
		{
			Object.Destroy((Object)(object)_followButton);
		}
		_followButton = null;
		if (IsAlive((Object)(object)_root))
		{
			Object.Destroy((Object)(object)_root);
		}
		_root = null;
		if (IsAlive((Object)(object)_solidSprite))
		{
			Object.Destroy((Object)(object)_solidSprite);
		}
		_solidSprite = null;
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
