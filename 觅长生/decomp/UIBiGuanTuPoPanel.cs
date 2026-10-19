using System.Collections.Generic;
using GUIPackage;
using KBEngine;
using UnityEngine;
using UnityEngine.UI;

public class UIBiGuanTuPoPanel : TabPanelBase
{
	public RectTransform SVContent;

	public RectTransform RightContent;

	public Dropdown Fillter1;

	public Dropdown Filter2;

	public Text RightTitle;

	public Text TuPoXiaoHaoText;

	public Scrollbar JieDuanBar;

	public Image BtnImage1;

	public Image BtnImage2;

	public Material GreyMat;

	private UIIconShow tmpIcon;

	public List<Text> DescItem;

	public override void OnPanelShow()
	{
		base.OnPanelShow();
		RefreshUI();
	}

	public void RefreshUI()
	{
		RefreshInventory();
		SetNull();
		UIBiGuanPanel.Inst.RefreshKeFangTime();
	}

	public void RefreshInventory()
	{
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		((Transform)(object)SVContent).DestoryAllChild();
		Avatar player = PlayerEx.Player;
		int value = Fillter1.value;
		int value2 = Filter2.value;
		foreach (SkillItem hasStaticSkill in player.hasStaticSkillList)
		{
			bool flag = false;
			if (hasStaticSkill.level >= 5)
			{
				continue;
			}
			GUIPackage.Skill skill = SkillStaticDatebase.instence.Dict[hasStaticSkill.itemId][hasStaticSkill.level];
			JSONObject jSONObject = jsonData.instance.StaticSkillJsonData[skill.skill_ID.ToString()];
			if (value2 == 0 || skill.SkillQuality == value2)
			{
				if (value == 0)
				{
					flag = true;
				}
				else
				{
					int i = jSONObject["AttackType"].I;
					if (i < 5)
					{
						if (i == value - 1)
						{
							flag = true;
						}
					}
					else if (value == 6)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				continue;
			}
			UIIconShow show = Object.Instantiate<GameObject>(UIIconShow.Prefab, (Transform)(object)SVContent).GetComponent<UIIconShow>();
			show.SetStaticSkill(skill);
			show.CanDrag = false;
			int num = Tools.CalcTuPoTime(skill.skill_ID);
			string topoTime = Tools.getStr("tupozhixiayijie").Replace("{X}", (num / 12).ToString()).Replace("{Y}", (num % 12).ToString())
				.Replace("突破至下一阶需要：", "");
			show.OnClick = delegate
			{
				SetTuPo(show, topoTime);
				SetAllIconUnSelected();
				show.SelectedImage.SetActive(true);
			};
			for (int j = 0; j < player.equipStaticSkillList.Count; j++)
			{
				SkillItem skillItem = player.equipStaticSkillList[j];
				if (skillItem != null && skillItem.itemId == hasStaticSkill.itemId)
				{
					switch ((GongFaSlotType)skillItem.itemIndex)
					{
					case GongFaSlotType.主修:
						show.JiaoBiaoTextContent = "<color=#fbed9f>主</color>";
						show.JiaoBiaoTextOutlineColor = new Color(0.20392157f, 0.11764706f, 0.043137256f);
						break;
					case GongFaSlotType.元婴:
						show.JiaoBiaoTextContent = "<color=#ffc9c1>婴</color>";
						show.JiaoBiaoTextOutlineColor = new Color(18f / 85f, 0.05490196f, 0.02745098f);
						break;
					case GongFaSlotType.遁术:
						show.JiaoBiaoTextContent = "<color=#c8f5eb>遁</color>";
						show.JiaoBiaoTextOutlineColor = new Color(0.10980392f, 12f / 85f, 0.20784314f);
						break;
					default:
						show.JiaoBiaoTextContent = "<color=#dff7b1>辅</color>";
						show.JiaoBiaoTextOutlineColor = new Color(0.16078432f, 0.20392157f, 0.043137256f);
						break;
					}
					show.NowJiaoBiao = UIIconShow.JiaoBiaoType.Text;
				}
			}
		}
	}

	public void SetAllIconUnSelected()
	{
		int childCount = ((Transform)SVContent).childCount;
		for (int i = 0; i < childCount; i++)
		{
			((Component)((Transform)SVContent).GetChild(i)).GetComponent<UIIconShow>().SelectedImage.SetActive(false);
		}
	}

	public void SetNull()
	{
		tmpIcon = null;
		RightTitle.text = "突破";
		TuPoXiaoHaoText.text = "";
		for (int i = 0; i < 5; i++)
		{
			((Component)DescItem[i]).gameObject.SetActive(false);
		}
		((Graphic)BtnImage1).material = GreyMat;
		((Graphic)BtnImage2).material = GreyMat;
		((Component)JieDuanBar).gameObject.SetActive(false);
	}

	public void SetTuPo(UIIconShow iconShow, string tupoTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		RightContent.anchoredPosition = new Vector2(RightContent.anchoredPosition.x, 0f);
		tmpIcon = iconShow;
		RightTitle.text = iconShow.tmpSkill.skill_Name.RemoveNumber();
		((Graphic)BtnImage1).material = null;
		((Graphic)BtnImage2).material = null;
		TuPoXiaoHaoText.text = tupoTime;
		GUIPackage.Skill tmpSkill = tmpIcon.tmpSkill;
		((Component)JieDuanBar).gameObject.SetActive(true);
		JieDuanBar.size = (float)(tmpSkill.Skill_Lv - 1) / 4f;
		for (int i = 1; i <= 5; i++)
		{
			((Component)DescItem[i - 1]).gameObject.SetActive(true);
			DescItem[i - 1].text = SkillStaticDatebase.instence.Dict[tmpSkill.SkillID][i].skill_Desc;
			if (tmpSkill.Skill_Lv >= i)
			{
				((Component)((Component)DescItem[i - 1]).transform.GetChild(1)).gameObject.SetActive(true);
			}
			else
			{
				((Component)((Component)DescItem[i - 1]).transform.GetChild(1)).gameObject.SetActive(false);
			}
		}
	}

	public void OnTuPoButtonClick()
	{
		if (!((Object)(object)tmpIcon == (Object)null))
		{
			TuPo();
		}
	}

	public void TuPo()
	{
		Avatar player = PlayerEx.Player;
		Dictionary<int, GUIPackage.Skill> dicSkills = SkillStaticDatebase.instence.dicSkills;
		int skill_ID = tmpIcon.tmpSkill.skill_ID;
		JSONObject jSONObject = jsonData.instance.StaticSkillJsonData[skill_ID.ToString()];
		int num = Tools.CalcTuPoTime(skill_ID);
		int num2 = (int)jSONObject["Skill_Lv"].n;
		if (!XiuLian.CheckCanUse(num))
		{
			UIPopTip.Inst.Pop("房间剩余时间不足，无法进行突破");
			return;
		}
		int num3 = (int)jSONObject["Skill_LV"].n;
		if (num2 < dicSkills[skill_ID].Max_level && num2 < jsonData.instance.StaticLVToLevelJsonData[Tools.instance.getPlayer().getLevelType().ToString()]["Max" + num3].I)
		{
			UIAnimProgressBar.Show(new ProgressBarShowData("正在突破功法...", delegate
			{
				RefreshUI();
			}));
			player.AddTime(0, num % 12, num / 12);
			JSONObject jSONObject2 = jsonData.instance.StaticSkillJsonData[(skill_ID + 1).ToString()];
			_ = jsonData.instance.WuDaoExBeiLuJson["1"]["tupo"].n;
			_ = jSONObject2["Skill_castTime"].n;
			int i = jSONObject2["Skill_ID"].I;
			foreach (KeyValuePair<string, JSONObject> itemJsonDatum in jsonData.instance.ItemJsonData)
			{
				if (itemJsonDatum.Value["type"].I == 4)
				{
					float result = 0f;
					if (float.TryParse(itemJsonDatum.Value["desc"].str, out result) && (int)result == i)
					{
						item.AddWuDao(jSONObject2["Skill_castTime"].I * 30, jsonData.instance.WuDaoExBeiLuJson["1"]["tupo"].n, Tools.JsonListToList(itemJsonDatum.Value["wuDao"]));
						break;
					}
				}
			}
			foreach (SkillItem _skillTemp in player.hasStaticSkillList)
			{
				if (_skillTemp.itemId == jSONObject["Skill_ID"].I && _skillTemp.level < 5)
				{
					_skillTemp.level++;
					int equipindex = 0;
					if (player.equipStaticSkillList.Find(delegate(SkillItem equipInfo)
					{
						if (equipInfo.itemId == _skillTemp.itemId)
						{
							equipindex = equipInfo.itemIndex;
						}
						return equipInfo.itemId == _skillTemp.itemId;
					}) != null)
					{
						player.UnEquipStaticSkill(_skillTemp.itemId);
						player.equipStaticSkill(_skillTemp.itemId, equipindex);
					}
					new StaticSkill(Tools.instance.getStaticSkillKeyByID(_skillTemp.itemId), 0, 5).Puting(player, player, 3);
				}
			}
			player.HP = player.HP_Max;
			PlayTutorial.CheckGongFaTask();
		}
		else if (num2 >= dicSkills[skill_ID].Max_level)
		{
			UIPopTip.Inst.Pop("已到最大等级");
		}
		else
		{
			string str = jsonData.instance.StaticLVToLevelJsonData[(player.getLevelType() + 1).ToString()]["Name"].Str;
			UIPopTip.Inst.Pop(str + "后方可继续突破");
		}
	}
}
