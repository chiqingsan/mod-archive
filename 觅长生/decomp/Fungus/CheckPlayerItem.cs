using System;
using JSONClass;
using KBEngine;
using UnityEngine;

namespace Fungus;

[CommandInfo("新手引导", "检测玩家配置物品", "检测玩家配置物品", 0)]
[AddComponentMenu("")]
public class CheckPlayerItem : Command
{
	[SerializeField]
	public CheckItem CheckItem;

	[Tooltip("TempValue")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable TmpValue;

	[Tooltip("TmpBoolValue")]
	[VariableProperty(new Type[] { typeof(BooleanVariable) })]
	[SerializeField]
	protected BooleanVariable TmpBoolValue;

	public override void OnEnter()
	{
		switch (CheckItem)
		{
		case CheckItem.装备的武器:
			CheckEquip(2);
			break;
		case CheckItem.装备的防具:
			CheckEquip(1);
			break;
		case CheckItem.装备的饰品:
			CheckEquip(3);
			break;
		case CheckItem.装备的功法数量:
			CheckGongFangSum();
			break;
		case CheckItem.装备的最高功法突破等级:
			CheckMaxGongFang();
			break;
		}
		Continue();
	}

	public void CheckEquip(int type)
	{
		foreach (ITEM_INFO value in PlayerEx.Player.equipItemList.values)
		{
			foreach (JSONObject item in jsonData.instance.ItemJsonData[string.Concat(value.itemId)]["seid"].list)
			{
				if (item.I == type)
				{
					TmpBoolValue.Value = true;
					if (value.Seid.HasField("quality"))
					{
						TmpValue.Value = value.Seid["quality"].I;
					}
					else
					{
						TmpValue.Value = _ItemJsonData.DataDict[value.itemId].quality;
					}
					return;
				}
			}
		}
	}

	public void CheckGongFangSum()
	{
		TmpValue.Value = PlayerEx.Player.equipStaticSkillList.Count;
	}

	public void CheckMaxGongFang()
	{
		int num = 0;
		int num2 = 0;
		foreach (SkillItem equipStaticSkill in PlayerEx.Player.equipStaticSkillList)
		{
			num2 = Tools.instance.getStaticSkillKeyByID(equipStaticSkill.itemId);
			if (StaticSkillJsonData.DataDict[num2].Skill_Lv > num)
			{
				num = StaticSkillJsonData.DataDict[num2].Skill_Lv;
			}
		}
		TmpValue.Value = num;
	}
}
