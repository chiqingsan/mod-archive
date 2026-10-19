using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using KBEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YSGame;

public class UIShengPingPanel : MonoBehaviour
{
	public Vector2 HidePos;

	public Vector2 ShowPos;

	public Animator SceneAnim;

	public FpBtn OkBtn;

	public RectTransform SVRT;

	public RectTransform ContentRT;

	public RectTransform ShengPingTextRT;

	public Image SVImage;

	public Text ShengPingText;

	public ParticleSystem GuangPS;

	public Transform RenTransform;

	public Vector3 RenStartPos;

	public Vector3 RenEndPos;

	public Vector3 RenHidePos;

	public Text KuaiJinTips;

	public RectTransform ZhiXieRT;

	public Vector2 ZhiXieStartPos;

	public Vector2 ZhiXieEndPos;

	public float ZhiXieMoveSpeed;

	public Image BlackImage;

	private RectTransform rt;

	private float kuaiJinCD;

	private float kuaiJinAlpha;

	private float kuaiJinStep = 1f;

	private float zhiXieProgress;

	private float textStartY = -650f;

	private float textSpeed = 1f;

	private void Awake()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		ref RectTransform reference = ref rt;
		Transform transform = ((Component)this).transform;
		reference = (RectTransform)(object)((transform is RectTransform) ? transform : null);
		OkBtn.mouseUpEvent.AddListener(new UnityAction(OnOkBtnClick));
	}

	private void Start()
	{
		((Component)OkBtn).gameObject.SetActive(false);
		PlayEnterAnim();
		PlayTextAnim();
		((MonoBehaviour)this).StartCoroutine(KuaiJinCoroutine());
		MusicMag.instance.PlayMusicList(new List<string> { "结束_人员表", "结束_完整" }, firstImmediately: false, listImmediately: true);
	}

	private void Update()
	{
		if (Input.GetKey((KeyCode)306))
		{
			textSpeed = 240f;
		}
		else
		{
			textSpeed = 40f;
		}
		if (Input.anyKey)
		{
			if (kuaiJinCD <= 0f)
			{
				kuaiJinAlpha = 0f;
			}
			kuaiJinCD = 2f;
		}
		kuaiJinCD -= Time.deltaTime;
	}

	private IEnumerator KuaiJinCoroutine()
	{
		while (((Component)KuaiJinTips).gameObject.activeInHierarchy)
		{
			if (kuaiJinCD > 0f || kuaiJinAlpha >= 0f)
			{
				kuaiJinAlpha += kuaiJinStep * Time.deltaTime;
				((Graphic)KuaiJinTips).color = new Color(((Graphic)KuaiJinTips).color.r, ((Graphic)KuaiJinTips).color.g, ((Graphic)KuaiJinTips).color.b, Mathf.Clamp01(kuaiJinAlpha));
				if (kuaiJinAlpha >= 1f)
				{
					kuaiJinStep = -1f;
				}
				if (kuaiJinAlpha < 0f)
				{
					kuaiJinStep = 1f;
				}
			}
			yield return CoroutineTool.WaitForEndOfFrame;
		}
	}

	public void RefreshPlayerShengPing()
	{
		if (PlayerEx.Player == null)
		{
			return;
		}
		List<ShengPingData> shengPingList = PlayerEx.GetShengPingList();
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ShengPingData item in shengPingList)
		{
			stringBuilder.AppendLine(item.ToShengPingString());
		}
		ShengPingText.text = stringBuilder.ToString();
	}

	public void PlayEnterAnim()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		rt.anchoredPosition = HidePos;
		((MonoBehaviour)this).StartCoroutine(ShowCoroutine());
	}

	private IEnumerator ShowCoroutine()
	{
		SceneAnim.Play("EndSceneEnterAnim");
		yield return (object)new WaitForSeconds(0.7f);
		rt.DOAnchorPos(ShowPos, 1.7f);
	}

	public void OnOkBtnClick()
	{
		PlayExitAnim();
	}

	public void PlayExitAnim()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((Component)OkBtn).gameObject.SetActive(false);
		rt.anchoredPosition = ShowPos;
		GuangPS.startLifetime = 3f;
		((MonoBehaviour)this).StartCoroutine(ExitCoroutine());
	}

	private IEnumerator ExitCoroutine()
	{
		SceneAnim.Play("EndSceneEndAnim");
		rt.DOAnchorPos(HidePos, 1.7f);
		yield return CoroutineTool.WaitForSeconds2;
		TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(ShortcutExtensions.DOMove(RenTransform, RenHidePos, 2f, false), (Ease)20);
		yield return CoroutineTool.WaitForSeconds1;
		ShortcutExtensions.DOColor(((Renderer)((Component)RenTransform).GetComponent<MeshRenderer>()).material, Color.clear, 1f);
		yield return (object)new WaitForSeconds(0.5f);
		while (zhiXieProgress < 1f)
		{
			Vector2 anchoredPosition = Vector2.op_Implicit(Vector3.Lerp(Vector2.op_Implicit(ZhiXieStartPos), Vector2.op_Implicit(ZhiXieEndPos), zhiXieProgress));
			ZhiXieRT.anchoredPosition = anchoredPosition;
			zhiXieProgress += textSpeed * ZhiXieMoveSpeed * Time.deltaTime;
			yield return CoroutineTool.WaitForEndOfFrame;
		}
		((Component)GuangPS).gameObject.SetActive(false);
		BlackImage.DOColor(new Color(0f, 0f, 0f, 1f), 1f);
		yield return CoroutineTool.WaitForSeconds1;
		KBEngineApp.app.entities[10] = null;
		KBEngineApp.app.entities.Remove(10);
		Tools.instance.loadOtherScenes("MainMenu");
	}

	public void PlayTextAnim()
	{
		RefreshPlayerShengPing();
		((MonoBehaviour)this).StartCoroutine(TextAnimCoroutine());
	}

	private IEnumerator TextAnimCoroutine()
	{
		yield return (object)new WaitForSeconds(0.2f);
		((Graphic)SVImage).raycastTarget = false;
		float contentTargetY = ShengPingTextRT.sizeDelta.y - SVRT.sizeDelta.y;
		float totalLen = 0f - textStartY + Mathf.Clamp(contentTargetY, 0f, 10000f);
		ShengPingTextRT.anchoredPosition = new Vector2(ShengPingTextRT.anchoredPosition.x, textStartY);
		while (ShengPingTextRT.anchoredPosition.y < 0f)
		{
			ShengPingTextRT.anchoredPosition = new Vector2(ShengPingTextRT.anchoredPosition.x, ShengPingTextRT.anchoredPosition.y + textSpeed * Time.deltaTime);
			float num = Mathf.Abs(textStartY - ShengPingTextRT.anchoredPosition.y) / totalLen;
			RenTransform.position = Vector3.Lerp(RenStartPos, RenEndPos, num);
			yield return CoroutineTool.WaitForEndOfFrame;
		}
		ShengPingTextRT.anchoredPosition = new Vector2(ShengPingTextRT.anchoredPosition.x, 0f);
		if (contentTargetY > 0f)
		{
			while (ContentRT.anchoredPosition.y < contentTargetY)
			{
				ContentRT.anchoredPosition = new Vector2(ContentRT.anchoredPosition.x, ContentRT.anchoredPosition.y + textSpeed * Time.deltaTime);
				float num = Mathf.Clamp01((Mathf.Abs(textStartY) + ContentRT.anchoredPosition.y) / totalLen);
				RenTransform.position = Vector3.Lerp(RenStartPos, RenEndPos, num);
				yield return CoroutineTool.WaitForEndOfFrame;
			}
			ContentRT.anchoredPosition = new Vector2(ContentRT.anchoredPosition.x, contentTargetY);
		}
		((Component)OkBtn).gameObject.SetActive(true);
		((Graphic)SVImage).raycastTarget = true;
	}
}
