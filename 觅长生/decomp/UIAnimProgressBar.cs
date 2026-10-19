using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GUIPackage;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimProgressBar : MonoBehaviour, IESCClose
{
	public static UIAnimProgressBar Inst;

	private static string prefabPath = "UIAnimProgressBar";

	private static bool needShow;

	public static bool IsShow;

	public GameObject Root;

	public Image Bar;

	public Text ContentText;

	private static ProgressBarShowData showData;

	public List<ProgressBarConfig> ProcessBarConfigs;

	private bool isNeedJieSuan;

	private TweenerCore<float, float, FloatOptions> obj;

	private void Awake()
	{
		if ((Object)(object)Inst != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		Inst = this;
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		if (needShow)
		{
			needShow = false;
			ESCCloseManager.Inst.RegisterClose(this);
			IsShow = true;
			isNeedJieSuan = false;
			Tools.canClickFlag = false;
			NpcJieSuanManager.inst.JieSuanAnimation = false;
			ContentText.text = showData.Content;
			foreach (ProgressBarConfig processBarConfig in ProcessBarConfigs)
			{
				processBarConfig.AnimObj.SetActive(processBarConfig.ProgressType == showData.ProcessBarType);
			}
			Bar.fillAmount = 0f;
			obj = TweenExtensions.Play<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(Bar.DOFillAmount(0.2f, 1.5f), (TweenCallback)delegate
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Expected O, but got Unknown
				if (NpcJieSuanManager.inst.isCanJieSuan)
				{
					TweenExtensions.Play<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(Bar.DOFillAmount(1f, 0.5f), (TweenCallback)delegate
					{
						Tools.canClickFlag = true;
						if (showData.OnComplete != null)
						{
							showData.OnComplete();
						}
						Root.SetActive(false);
						IsShow = false;
						ESCCloseManager.Inst.UnRegisterClose(this);
					}));
				}
				else
				{
					isNeedJieSuan = true;
					obj = Bar.DOFillAmount(0.98f, 20f);
				}
			}));
			((Component)this).transform.SetAsLastSibling();
			Root.SetActive(true);
		}
		if (IsShow && isNeedJieSuan && NpcJieSuanManager.inst.JieSuanAnimation)
		{
			NpcJieSuanManager.inst.JieSuanAnimation = false;
			CallBack();
		}
	}

	public static void Show(ProgressBarShowData data)
	{
		if (data != null)
		{
			if ((Object)(object)Inst == (Object)null)
			{
				Object.Instantiate<GameObject>(Resources.Load<GameObject>(prefabPath), UI_Manager.inst.JieSuanDongHuaParent);
			}
			showData = data;
			needShow = true;
		}
	}

	private void CallBack()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		if (obj != null)
		{
			TweenExtensions.Kill((Tween)(object)obj, false);
		}
		TweenExtensions.Play<TweenerCore<float, float, FloatOptions>>(TweenSettingsExtensions.OnComplete<TweenerCore<float, float, FloatOptions>>(Bar.DOFillAmount(1f, 0.5f * (1f - Bar.fillAmount)), (TweenCallback)delegate
		{
			Tools.canClickFlag = true;
			if (showData.OnComplete != null)
			{
				showData.OnComplete();
			}
			Root.SetActive(false);
			IsShow = false;
			ESCCloseManager.Inst.UnRegisterClose(this);
		}));
	}

	public void Close()
	{
		ESCCloseManager.Inst.UnRegisterClose(this);
		Object.Destroy((Object)(object)((Component)this).gameObject);
		IsShow = false;
		Inst = null;
	}

	public bool TryEscClose()
	{
		return true;
	}
}
