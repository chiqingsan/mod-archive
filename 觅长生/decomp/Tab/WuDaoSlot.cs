using System;
using System.Collections.Generic;
using Coffee.UIEffects;
using JSONClass;
using KBEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Tab;

[Serializable]
public class WuDaoSlot : UIBase
{
	private GameObject _name;

	private GameObject _cost;

	private GameObject _black;

	private Image _icon;

	private Image _white;

	private UIEffect _iconEffect;

	public int Id;

	public int State;

	public int Cost;

	public JSONObject WuDaoJson;

	public WuDaoSlot(GameObject go, int id)
	{
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		WuDaoSlot wuDaoSlot = this;
		_go = go;
		Id = id;
		WuDaoJson wuDaoJson = JSONClass.WuDaoJson.DataDict[Id];
		_icon = Get<Image>("Mask/Icon");
		_icon.sprite = ResManager.inst.LoadSprite("WuDao Icon/" + wuDaoJson.icon);
		_iconEffect = Get<UIEffect>("Mask/Icon");
		Get<Text>("Name/Value").text = wuDaoJson.name;
		Cost = wuDaoJson.Cast;
		Get<Text>("Cost/Value").text = Cost.ToString();
		_name = Get("Name");
		_cost = Get("Cost");
		_black = Get("Black");
		_white = _go.GetComponent<Image>();
		WuDaoJson = jsonData.instance.WuDaoJson[Id.ToString()];
		_go.AddComponent<UIListener>().mouseUpEvent.AddListener((UnityAction)delegate
		{
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Expected O, but got Unknown
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Expected O, but got Unknown
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Expected O, but got Unknown
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Expected O, but got Unknown
			WuDaoUtils.Inst.Update();
			if (WuDaoUtils.Inst.HasStudy.Contains(id) && PlayerEx.Player.SelectTianFuID.HasItem(316))
			{
				if (wuDaoSlot.CanYiWang())
				{
					SingletonMono<TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Show(wuDaoSlot._icon.sprite, wuDaoSlot.Id, new UnityAction(wuDaoSlot.ClickYiWang), WuDaoTooltipType.遗忘);
				}
				else
				{
					SingletonMono<TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Show(wuDaoSlot._icon.sprite, wuDaoSlot.Id, new UnityAction(wuDaoSlot.ClickYiWang), WuDaoTooltipType.不可遗忘);
				}
			}
			else if (wuDaoSlot.CanStudy())
			{
				SingletonMono<TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Show(wuDaoSlot._icon.sprite, wuDaoSlot.Id, new UnityAction(wuDaoSlot.Study), WuDaoTooltipType.感悟);
			}
			else
			{
				SingletonMono<TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Show(wuDaoSlot._icon.sprite, wuDaoSlot.Id, new UnityAction(wuDaoSlot.Study), WuDaoTooltipType.不可感悟);
			}
		});
	}

	public void SetState(int state)
	{
		State = state;
		switch (State)
		{
		case 1:
			_name.SetActive(false);
			_cost.SetActive(false);
			_black.SetActive(false);
			_white.Show();
			((Behaviour)_iconEffect).enabled = false;
			break;
		case 2:
			_name.SetActive(true);
			_cost.SetActive(true);
			_black.SetActive(false);
			_white.Show();
			((Behaviour)_iconEffect).enabled = true;
			_iconEffect.effectFactor = 0.72f;
			_iconEffect.colorMode = (ColorMode)3;
			_iconEffect.colorFactor = 0.25f;
			break;
		case 3:
			_name.SetActive(false);
			_cost.SetActive(false);
			_black.SetActive(true);
			_white.Hide();
			((Behaviour)_iconEffect).enabled = true;
			_iconEffect.effectFactor = 0.72f;
			_iconEffect.colorMode = (ColorMode)3;
			_iconEffect.colorFactor = 0.25f;
			break;
		}
	}

	private void Study()
	{
		Avatar player = Tools.instance.getPlayer();
		if (State == 1)
		{
			UIPopTip.Inst.Pop("已领悟过该大道");
		}
		else if (State == 2)
		{
			if (player.wuDaoMag.GetNowWuDaoDian() >= Cost)
			{
				if (CanStudyWuDao())
				{
					foreach (int item in JSONClass.WuDaoJson.DataDict[Id].Type)
					{
						player.wuDaoMag.addWuDaoSkill(item, Id);
						SingletonMono<TabUIMag>.Instance.WuDaoPanel.UpdateWuDaoDian();
					}
					SetState(1);
				}
				else if (MoreCheck())
				{
					UIPopTip.Inst.Pop("未达到领悟条件");
				}
				else
				{
					UIPopTip.Inst.Pop("未领悟前置悟道");
				}
			}
			else
			{
				UIPopTip.Inst.Pop("悟道点不足");
			}
		}
		else if (State == 3)
		{
			UIPopTip.Inst.Pop("未达到领悟条件");
		}
		SingletonMono<TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Close();
	}

	private bool CanStudy()
	{
		Avatar player = Tools.instance.getPlayer();
		if (State == 1)
		{
			return false;
		}
		if (State == 2)
		{
			if (player.wuDaoMag.GetNowWuDaoDian() >= Cost)
			{
				if (CanStudyWuDao())
				{
					return true;
				}
				MoreCheck();
				return false;
			}
			return false;
		}
		_ = State;
		_ = 3;
		return false;
	}

	private bool CanYiWang()
	{
		if (JSONClass.WuDaoJson.DataDict[Id].CanForget == 1)
		{
			return false;
		}
		int id = SingletonMono<TabUIMag>.Instance.WuDaoPanel.CurSelectType.Id;
		int lv = JSONClass.WuDaoJson.DataDict[Id].Lv;
		_ = WuDaoUtils.Inst.WuDaoDictionary[id][lv];
		int num = 0;
		if (WuDaoUtils.Inst.WuDaoDictionary[id].ContainsKey(lv + 1))
		{
			foreach (int item in JSONClass.WuDaoJson.DataDict[Id].Type)
			{
				foreach (int item2 in WuDaoUtils.Inst.WuDaoDictionary[item][lv + 1])
				{
					if (WuDaoUtils.Inst.HasStudy.Contains(item2))
					{
						num++;
						break;
					}
				}
			}
			if (num > 0)
			{
				num = 0;
				foreach (int item3 in JSONClass.WuDaoJson.DataDict[Id].Type)
				{
					foreach (int item4 in WuDaoUtils.Inst.WuDaoDictionary[item3][lv])
					{
						if (item4 != Id && WuDaoUtils.Inst.HasStudy.Contains(item4))
						{
							num++;
							break;
						}
					}
				}
				if (num == JSONClass.WuDaoJson.DataDict[Id].Type.Count)
				{
					return true;
				}
			}
			else
			{
				foreach (int item5 in WuDaoUtils.Inst.WuDaoDictionary[id][lv])
				{
					if (item5 != Id && WuDaoUtils.Inst.HasStudy.Contains(item5))
					{
						return true;
					}
				}
			}
		}
		lv++;
		if (!WuDaoUtils.Inst.WuDaoDictionary[id].ContainsKey(lv))
		{
			return true;
		}
		foreach (int item6 in JSONClass.WuDaoJson.DataDict[Id].Type)
		{
			foreach (int item7 in WuDaoUtils.Inst.WuDaoDictionary[item6][lv])
			{
				if (WuDaoUtils.Inst.HasStudy.Contains(item7))
				{
					return false;
				}
			}
		}
		return true;
	}

	private void ClickYiWang()
	{
		if (JSONClass.WuDaoJson.DataDict[Id].CanForget == 1)
		{
			UIPopTip.Inst.Pop("该悟道不可遗忘");
			SingletonMono<TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Close();
			return;
		}
		int id = SingletonMono<TabUIMag>.Instance.WuDaoPanel.CurSelectType.Id;
		int lv = JSONClass.WuDaoJson.DataDict[Id].Lv;
		_ = WuDaoUtils.Inst.WuDaoDictionary[id][lv];
		int num = 0;
		if (WuDaoUtils.Inst.WuDaoDictionary[id].ContainsKey(lv + 1))
		{
			foreach (int item in JSONClass.WuDaoJson.DataDict[Id].Type)
			{
				foreach (int item2 in WuDaoUtils.Inst.WuDaoDictionary[item][lv + 1])
				{
					if (WuDaoUtils.Inst.HasStudy.Contains(item2))
					{
						num++;
						break;
					}
				}
			}
			if (num > 0)
			{
				num = 0;
				foreach (int item3 in JSONClass.WuDaoJson.DataDict[Id].Type)
				{
					foreach (int item4 in WuDaoUtils.Inst.WuDaoDictionary[item3][lv])
					{
						if (item4 != Id && WuDaoUtils.Inst.HasStudy.Contains(item4))
						{
							num++;
							break;
						}
					}
				}
				if (num == JSONClass.WuDaoJson.DataDict[Id].Type.Count)
				{
					YiWang();
					return;
				}
			}
			else
			{
				foreach (int item5 in WuDaoUtils.Inst.WuDaoDictionary[id][lv])
				{
					if (item5 != Id && WuDaoUtils.Inst.HasStudy.Contains(item5))
					{
						YiWang();
						return;
					}
				}
			}
		}
		lv++;
		if (!WuDaoUtils.Inst.WuDaoDictionary[id].ContainsKey(lv))
		{
			YiWang();
			return;
		}
		foreach (int item6 in JSONClass.WuDaoJson.DataDict[Id].Type)
		{
			foreach (int item7 in WuDaoUtils.Inst.WuDaoDictionary[item6][lv])
			{
				if (WuDaoUtils.Inst.HasStudy.Contains(item7))
				{
					UIPopTip.Inst.Pop("需先遗忘下一层的悟道");
					SingletonMono<TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Close();
					return;
				}
			}
		}
		YiWang();
	}

	private void YiWang()
	{
		foreach (int item in JSONClass.WuDaoJson.DataDict[Id].Type)
		{
			PlayerEx.Player.wuDaoMag.DeleteWuDaoSkill(item, Id);
		}
		SetState(2);
		SingletonMono<TabUIMag>.Instance.WuDaoPanel.UpdateWuDaoDian();
		SingletonMono<TabUIMag>.Instance.WuDaoPanel.WuDaoTooltip.Close();
	}

	public bool CanStudyWuDao()
	{
		JSONObject jSONObject = jsonData.instance.WuDaoJson[Id.ToString()];
		bool flag = true;
		foreach (JSONObject item in jSONObject["Type"].list)
		{
			if (!CanEx(item.I))
			{
				return false;
			}
			if (CanLastWuDao(item.I))
			{
				flag = false;
			}
		}
		if (flag)
		{
			return false;
		}
		return true;
	}

	public bool CanEx(int WuDaoType)
	{
		int wuDaoLevelByType = Tools.instance.getPlayer().wuDaoMag.getWuDaoLevelByType(WuDaoType);
		int i = WuDaoJson["Lv"].I;
		if (wuDaoLevelByType >= i)
		{
			return true;
		}
		return false;
	}

	public bool MoreCheck()
	{
		foreach (JSONObject item in jsonData.instance.WuDaoJson[Id.ToString()]["Type"].list)
		{
			if (!CanEx(item.I))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanLastWuDao(int wudaoType)
	{
		Avatar player = Tools.instance.getPlayer();
		JSONObject wuDaoJson = WuDaoJson;
		JSONObject wuDaoStudy = player.wuDaoMag.getWuDaoStudy(wudaoType);
		int i = wuDaoJson["Lv"].I;
		if (i == 1)
		{
			return true;
		}
		Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		for (int j = 1; j < i; j++)
		{
			dictionary[j] = false;
		}
		foreach (JSONObject item in wuDaoStudy.list)
		{
			JSONObject jSONObject = jsonData.instance.WuDaoJson[item.I.ToString()];
			if (dictionary.ContainsKey(jSONObject["Lv"].I) && !dictionary[jSONObject["Lv"].I])
			{
				dictionary[jSONObject["Lv"].I] = true;
			}
		}
		foreach (KeyValuePair<int, bool> item2 in dictionary)
		{
			if (!item2.Value)
			{
				return false;
			}
		}
		return true;
	}
}
