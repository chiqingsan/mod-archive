using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using YSGame;

public class UINPCQingJiao : MonoBehaviour, IESCClose
{
	private UINPCData npc;

	private bool inited;

	public List<RectTransform> RTList = new List<RectTransform>();

	public Image ShenTongBtnImage;

	public Image GongFaBtnImage;

	public Image ShenTongBtnIconImage;

	public Image GongFaBtnIconImage;

	public Text ShenTongBtnText;

	public Text GongFaBtnText;

	public Sprite ShenTongTabSprite;

	public Sprite ShenTongLoseSprite;

	public Sprite GongFaTabSprite;

	public Sprite GongFaLoseSprite;

	public Sprite TabBGSprite;

	public Sprite TabBGLoseSprite;

	public Text ShuXiText;

	public Text YouShanText;

	public Text QinMiText;

	public Text QingFenText;

	public Image ShuXiLockImage;

	public Image YouShanLockImage;

	public Image QinMiLockImage;

	private Color TabColor = new Color(0.972549f, 72f / 85f, 0.6392157f);

	private Color TabLoseColor = new Color(24f / 85f, 0.58431375f, 29f / 51f);

	private Color TextColor1 = new Color(42f / 85f, 0.88235295f, 0.8509804f);

	private Color TextColor2 = new Color(0.47843137f, 20f / 51f, 0.2901961f);

	public GameObject FavorShuXi;

	public GameObject FavorYouShan;

	public GameObject FavorQinMi;

	public GameObject IconPrefab;

	private int nowType;

	private static Dictionary<int, int> FavorDict = new Dictionary<int, int>
	{
		{ 1, 5 },
		{ 2, 6 },
		{ 3, 8 }
	};

	private static List<UINPCQingJiaoSkillData> SkillDataList = new List<UINPCQingJiaoSkillData>();

	private void OnEnable()
	{
		MusicMag.instance.PlayEffectMusic("13翻页");
	}

	private void Update()
	{
		AutoHide();
	}

	public bool CanShow()
	{
		if (UINPCJiaoHu.AllShouldHide)
		{
			return false;
		}
		if ((Object)(object)PanelMamager.inst != (Object)null && (Object)(object)PanelMamager.inst.UISceneGameObject == (Object)null)
		{
			return false;
		}
		if ((Object)(object)PanelMamager.inst != (Object)null && PanelMamager.inst.nowPanel != PanelMamager.PanelType.空)
		{
			return false;
		}
		return true;
	}

	private void AutoHide()
	{
		if (!CanShow())
		{
			UINPCJiaoHu.Inst.HideNPCQingJiaoPanel();
		}
	}

	public void RefreshUI()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		Init();
		npc = UINPCJiaoHu.Inst.NowJiaoHuNPC;
		npc.RefreshData();
		FavorShuXi.SetActive(false);
		FavorYouShan.SetActive(false);
		FavorQinMi.SetActive(false);
		QingFenText.text = npc.QingFen.ToString();
		((Graphic)ShuXiText).color = TextColor1;
		((Graphic)YouShanText).color = TextColor1;
		((Graphic)QinMiText).color = TextColor1;
		((Behaviour)ShuXiLockImage).enabled = false;
		((Behaviour)YouShanLockImage).enabled = false;
		((Behaviour)QinMiLockImage).enabled = false;
		if (npc.FavorLevel >= FavorDict[3])
		{
			FavorQinMi.SetActive(true);
		}
		else if (npc.FavorLevel >= FavorDict[2])
		{
			FavorYouShan.SetActive(true);
			((Graphic)QinMiText).color = TextColor2;
			((Behaviour)QinMiLockImage).enabled = true;
		}
		else if (npc.FavorLevel >= FavorDict[1])
		{
			FavorShuXi.SetActive(true);
			((Graphic)QinMiText).color = TextColor2;
			((Graphic)YouShanText).color = TextColor2;
			((Behaviour)YouShanLockImage).enabled = true;
			((Behaviour)QinMiLockImage).enabled = true;
		}
		else
		{
			((Graphic)ShuXiText).color = TextColor2;
			((Graphic)QinMiText).color = TextColor2;
			((Graphic)YouShanText).color = TextColor2;
			((Behaviour)ShuXiLockImage).enabled = true;
			((Behaviour)YouShanLockImage).enabled = true;
			((Behaviour)QinMiLockImage).enabled = true;
		}
		foreach (RectTransform rT in RTList)
		{
			ClearChild(rT);
		}
		List<UINPCQingJiaoSkillData> datas = SearchSkillFromExcel();
		if (nowType == 0)
		{
			for (int i = 1; i <= 3; i++)
			{
				foreach (UINPCQingJiaoSkillData.SData item in SearchCanQingJiaoGongFa(datas, i))
				{
					CreateQingJiaoGongFaSlot(item);
				}
			}
			return;
		}
		for (int j = 1; j <= 3; j++)
		{
			foreach (UINPCQingJiaoSkillData.SData item2 in SearchCanQingJiaoShenTong(datas, j))
			{
				CreateQingJiaoShenTongSlot(item2);
			}
		}
	}

	private List<UINPCQingJiaoSkillData> SearchSkillFromExcel()
	{
		List<UINPCQingJiaoSkillData> list = new List<UINPCQingJiaoSkillData>();
		foreach (UINPCQingJiaoSkillData skillData in SkillDataList)
		{
			if (skillData.LiuPai == npc.LiuPai && skillData.NPCLevel <= npc.Level)
			{
				list.Add(skillData);
			}
		}
		return list;
	}

	private List<UINPCQingJiaoSkillData.SData> SearchCanQingJiaoGongFa(List<UINPCQingJiaoSkillData> datas, int pinJie)
	{
		List<UINPCQingJiaoSkillData.SData> list = new List<UINPCQingJiaoSkillData.SData>();
		foreach (UINPCQingJiaoSkillData data in datas)
		{
			foreach (UINPCQingJiaoSkillData.SData sd in data.StaticSkills.Where((UINPCQingJiaoSkillData.SData s) => s.Quality == pinJie).ToList())
			{
				if (list.Find((UINPCQingJiaoSkillData.SData s) => s.SkillID == sd.SkillID) != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].SkillID == sd.SkillID && list[i].ID < sd.ID)
						{
							list[i] = sd;
						}
					}
				}
				else
				{
					list.Add(sd);
				}
			}
		}
		foreach (UINPCQingJiaoSkillData.SData exQingJiaoStaticSkill in npc.ExQingJiaoStaticSkills)
		{
			if (!list.Contains(exQingJiaoStaticSkill) && exQingJiaoStaticSkill.Quality == pinJie)
			{
				list.Add(exQingJiaoStaticSkill);
			}
		}
		return list;
	}

	private List<UINPCQingJiaoSkillData.SData> SearchCanQingJiaoShenTong(List<UINPCQingJiaoSkillData> datas, int pinJie)
	{
		List<UINPCQingJiaoSkillData.SData> list = new List<UINPCQingJiaoSkillData.SData>();
		foreach (UINPCQingJiaoSkillData data in datas)
		{
			foreach (UINPCQingJiaoSkillData.SData sd in data.Skills.Where((UINPCQingJiaoSkillData.SData s) => s.Quality == pinJie).ToList())
			{
				if (list.Where((UINPCQingJiaoSkillData.SData tmp) => tmp.SkillID == sd.SkillID && tmp.ID >= sd.ID).Count() <= 0)
				{
					list.Add(sd);
				}
			}
		}
		foreach (UINPCQingJiaoSkillData.SData exQingJiaoSkill in npc.ExQingJiaoSkills)
		{
			if (!list.Contains(exQingJiaoSkill) && exQingJiaoSkill.Quality == pinJie)
			{
				list.Add(exQingJiaoSkill);
			}
		}
		return list;
	}

	private void CreateQingJiaoGongFaSlot(UINPCQingJiaoSkillData.SData data)
	{
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		UIIconShow icon = Object.Instantiate<GameObject>(IconPrefab, (Transform)(object)RTList[data.Quality - 1]).GetComponent<UIIconShow>();
		icon.SetStaticSkill(data.ID, showStudy: true);
		icon.CanDrag = false;
		JSONObject skill = jsonData.instance.StaticSkillJsonData.list.Find((JSONObject s) => s["id"].I == data.ID);
		int qingJiaoType = skill["qingjiaotype"].I;
		bool isMiChuan = qingJiaoType == 3 || qingJiaoType == 5;
		bool isShiFu = PlayerEx.IsTheather(npc.ID);
		if (isMiChuan && npc.IsNingZhouNPC && !isShiFu)
		{
			if (!PlayerEx.IsNingZhouMaxShengWangLevel())
			{
				icon.SetBuChuan();
			}
		}
		else if (isMiChuan && !npc.IsNingZhouNPC && !isShiFu)
		{
			if (!PlayerEx.IsSeaMaxShengWangLevel())
			{
				icon.SetBuChuan();
			}
		}
		else if (qingJiaoType == 6 && !isShiFu)
		{
			icon.NowJiaoBiao = UIIconShow.JiaoBiaoType.Mo;
		}
		UnityAction action = (UnityAction)delegate
		{
			GongFaSlotAction(icon, isMiChuan, isShiFu, qingJiaoType, data.Quality, skill);
		};
		UIIconShow uIIconShow = icon;
		uIIconShow.OnClick = (UnityAction<PointerEventData>)(object)Delegate.Combine((Delegate)(object)uIIconShow.OnClick, (Delegate)(object)(UnityAction<PointerEventData>)delegate
		{
			USelectBox.Show("是否确定请教此功法？", action);
		});
	}

	private void GongFaSlotAction(UIIconShow slot, bool isMiChuan, bool isShiFu, int qingJiaoType, int pinJie, JSONObject skill)
	{
		if (slot.IsLingWu)
		{
			UIPopTip.Inst.Pop("你已经领悟这个功法了");
			return;
		}
		UINPCJiaoHu.Inst.QingJiaoName = skill["name"].Str.RemoveNumber();
		if (isMiChuan && npc.IsNingZhouNPC && !isShiFu)
		{
			if (!PlayerEx.IsNingZhouMaxShengWangLevel())
			{
				UIPopTip.Inst.Pop("你的声望不足以学习此功法");
				UINPCJiaoHu.Inst.IsQingJiaoShiBaiSW = true;
				return;
			}
		}
		else if (isMiChuan && !npc.IsNingZhouNPC && !isShiFu)
		{
			if (!PlayerEx.IsSeaMaxShengWangLevel())
			{
				UIPopTip.Inst.Pop("你的声望不足以学习此功法");
				UINPCJiaoHu.Inst.IsQingJiaoShiBaiSW = true;
				return;
			}
		}
		else if (qingJiaoType == 6 && !isShiFu)
		{
			UIPopTip.Inst.Pop("此乃魔道不传之密");
			UINPCJiaoHu.Inst.IsQingJiaoShiBaiSW = true;
			return;
		}
		if (npc.FavorLevel < FavorDict[pinJie])
		{
			UIPopTip.Inst.Pop("你的好感度不足以学习此功法");
			return;
		}
		int qingFenCost = NPCEx.GetQingFenCost(skill, isGongFa: true);
		if (npc.QingFen < qingFenCost)
		{
			UIPopTip.Inst.Pop("你们的情分不足以学习此功法");
			UINPCJiaoHu.Inst.IsQingJiaoShiBaiQF = true;
			return;
		}
		NPCEx.AddQingFen(npc.ID, -qingFenCost);
		UINPCJiaoHu.Inst.JiaoHuItemID = UINPCQingJiaoSkillData.GongFaItemDict[skill["Skill_ID"].I];
		PlayerEx.Player.addItem(UINPCJiaoHu.Inst.JiaoHuItemID, 1, null, ShowText: true);
		UINPCJiaoHu.Inst.IsQingJiaoChengGong = true;
		NpcJieSuanManager.inst.isUpDateNpcList = true;
	}

	private void CreateQingJiaoShenTongSlot(UINPCQingJiaoSkillData.SData data)
	{
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		UIIconShow icon = Object.Instantiate<GameObject>(IconPrefab, (Transform)(object)RTList[data.Quality - 1]).GetComponent<UIIconShow>();
		icon.SetSkill(data.ID, showStudy: true);
		icon.CanDrag = false;
		JSONObject skill = jsonData.instance._skillJsonData.list.Find((JSONObject s) => s["id"].I == data.ID);
		int qingJiaoType = skill["qingjiaotype"].I;
		bool isMiChuan = qingJiaoType == 3 || qingJiaoType == 5;
		bool isShiFu = PlayerEx.IsTheather(npc.ID);
		if (isMiChuan && npc.IsNingZhouNPC && !isShiFu)
		{
			if (!PlayerEx.IsNingZhouMaxShengWangLevel())
			{
				icon.SetBuChuan();
			}
		}
		else if (isMiChuan && !npc.IsNingZhouNPC && !isShiFu)
		{
			if (!PlayerEx.IsSeaMaxShengWangLevel())
			{
				icon.SetBuChuan();
			}
		}
		else if (qingJiaoType == 6 && !isShiFu)
		{
			icon.NowJiaoBiao = UIIconShow.JiaoBiaoType.Mo;
		}
		UnityAction action = (UnityAction)delegate
		{
			ShenTongSlotAction(icon, isMiChuan, isShiFu, qingJiaoType, data.Quality, skill);
		};
		UIIconShow uIIconShow = icon;
		uIIconShow.OnClick = (UnityAction<PointerEventData>)(object)Delegate.Combine((Delegate)(object)uIIconShow.OnClick, (Delegate)(object)(UnityAction<PointerEventData>)delegate
		{
			USelectBox.Show("是否确定请教此神通？", action);
		});
	}

	private void ShenTongSlotAction(UIIconShow slot, bool isMiChuan, bool isShiFu, int qingJiaoType, int pinJie, JSONObject skill)
	{
		if (slot.IsLingWu)
		{
			UIPopTip.Inst.Pop("你已经领悟这个神通了");
			return;
		}
		UINPCJiaoHu.Inst.QingJiaoName = skill["name"].Str.RemoveNumber();
		if (isMiChuan && npc.IsNingZhouNPC && !isShiFu)
		{
			if (!PlayerEx.IsNingZhouMaxShengWangLevel())
			{
				UIPopTip.Inst.Pop("你的声望不足以学习此神通");
				UINPCJiaoHu.Inst.IsQingJiaoShiBaiSW = true;
				return;
			}
		}
		else if (isMiChuan && !npc.IsNingZhouNPC && !isShiFu)
		{
			if (!PlayerEx.IsSeaMaxShengWangLevel())
			{
				UIPopTip.Inst.Pop("你的声望不足以学习此神通");
				UINPCJiaoHu.Inst.IsQingJiaoShiBaiSW = true;
				return;
			}
		}
		else if (qingJiaoType == 6 && !isShiFu)
		{
			UIPopTip.Inst.Pop("此乃魔道不传之密");
			UINPCJiaoHu.Inst.IsQingJiaoShiBaiSW = true;
			return;
		}
		if (npc.FavorLevel < FavorDict[pinJie])
		{
			UIPopTip.Inst.Pop("你的好感度不足以学习此神通");
			return;
		}
		int qingFenCost = NPCEx.GetQingFenCost(skill, isGongFa: false);
		if (npc.QingFen < qingFenCost)
		{
			UIPopTip.Inst.Pop("你们的情分不足以学习此神通");
			UINPCJiaoHu.Inst.IsQingJiaoShiBaiQF = true;
			return;
		}
		NPCEx.AddQingFen(npc.ID, -qingFenCost);
		UINPCJiaoHu.Inst.JiaoHuItemID = UINPCQingJiaoSkillData.SkillItemDict[skill["Skill_ID"].I];
		PlayerEx.Player.addItem(UINPCJiaoHu.Inst.JiaoHuItemID, 1, null, ShowText: true);
		UINPCJiaoHu.Inst.IsQingJiaoChengGong = true;
		NpcJieSuanManager.inst.isUpDateNpcList = true;
	}

	private void Init()
	{
		if (inited)
		{
			return;
		}
		foreach (JSONObject item in jsonData.instance.NPCLeiXingDate.list)
		{
			SkillDataList.Add(new UINPCQingJiaoSkillData(item));
		}
		inited = true;
	}

	public void OnShenTongBtnClick()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (nowType != 1)
		{
			nowType = 1;
			ShenTongBtnImage.sprite = TabBGSprite;
			ShenTongBtnIconImage.sprite = ShenTongTabSprite;
			((Graphic)ShenTongBtnText).color = TabColor;
			GongFaBtnImage.sprite = TabBGLoseSprite;
			GongFaBtnIconImage.sprite = GongFaLoseSprite;
			((Graphic)GongFaBtnText).color = TabLoseColor;
			RefreshUI();
			MusicMag.instance.PlayEffectMusic(13);
		}
	}

	public void OnGongFaBtnClick()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (nowType != 0)
		{
			nowType = 0;
			ShenTongBtnImage.sprite = TabBGLoseSprite;
			ShenTongBtnIconImage.sprite = ShenTongLoseSprite;
			((Graphic)ShenTongBtnText).color = TabLoseColor;
			GongFaBtnImage.sprite = TabBGSprite;
			GongFaBtnIconImage.sprite = GongFaTabSprite;
			((Graphic)GongFaBtnText).color = TabColor;
			RefreshUI();
			MusicMag.instance.PlayEffectMusic(13);
		}
	}

	public void ClearChild(RectTransform t)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < ((Transform)t).childCount; i++)
		{
			GameObject gameObject = ((Component)((Transform)t).GetChild(i)).gameObject;
			if (((Object)gameObject).name != "Lock")
			{
				list.Add(gameObject);
			}
		}
		foreach (GameObject item in list)
		{
			Object.Destroy((Object)(object)item);
		}
	}

	public bool TryEscClose()
	{
		UINPCJiaoHu.Inst.HideNPCQingJiaoPanel();
		return true;
	}
}
