using System.Collections.Generic;
using JSONClass;
using KillSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CyNpcList : MonoBehaviour
{
	public List<Sprite> npcCellSpriteList;

	public List<Sprite> tagSpriteList;

	public List<int> friendList;

	public FpBtn tagBtn;

	public GameObject cyNpcCell;

	public GameObject cySelectCell;

	public GameObject selectPanel;

	public GameObject npcCellParent;

	public Text npcNum;

	public Text curSelect;

	public Image sanJiao;

	public List<CyFriendCell> friendCells;

	public CyFriendCell curSelectFriend;

	public bool isShowSelectTag;

	public void Init()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		friendList = Tools.instance.getPlayer().emailDateMag.cyNpcList;
		npcNum.text = friendList.Count.ToString();
		curSelect.text = "全部";
		tagBtn.mouseUpEvent.AddListener(new UnityAction(ShowMoreSelect));
		InitNpcList(-2);
	}

	private void InitNpcList(int type)
	{
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		Tools.ClearObj(cyNpcCell.transform);
		friendCells = new List<CyFriendCell>();
		curSelectFriend = null;
		Dictionary<string, List<EmailData>>.KeyCollection keys = Tools.instance.getPlayer().emailDateMag.newEmailDictionary.Keys;
		List<int> list = new List<int>();
		foreach (string item in keys)
		{
			if ((type != -5 || int.Parse(item) == 3067) && (type != -1 || jsonData.instance.AvatarJsonData[item].TryGetField("IsTag").b) && (type != -4 || PlayerEx.IsDaoLv(int.Parse(item))) && (type != -3 || (CyTeShuNpc.DataDict.ContainsKey(int.Parse(item)) && CyTeShuNpc.DataDict[int.Parse(item)].Type == 1)) && (type < 0 || jsonData.instance.AvatarJsonData[item]["MenPai"].I == type))
			{
				CyFriendCell component = Tools.InstantiateGameObject(cyNpcCell, npcCellParent.transform).GetComponent<CyFriendCell>();
				component.Init(int.Parse(item));
				component.redDian.SetActive(true);
				list.Add(int.Parse(item));
				friendCells.Add(component);
			}
		}
		int num = 0;
		for (int i = 0; i < friendList.Count; i++)
		{
			num = friendList[i];
			if (list.Contains(friendList[i]) || friendList[i] == 0 || (type == -5 && num != 3067) || (type == -4 && !PlayerEx.IsDaoLv(num)) || (type == -3 && (!CyTeShuNpc.DataDict.ContainsKey(num) || CyTeShuNpc.DataDict[num].Type != 1)))
			{
				continue;
			}
			if (type == -1)
			{
				if (NpcJieSuanManager.inst.IsDeath(num))
				{
					continue;
				}
				jsonData.instance.AvatarJsonData[friendList[i].ToString()].TryGetField("IsTag");
				if (!jsonData.instance.AvatarJsonData[friendList[i].ToString()].TryGetField("IsTag").b)
				{
					continue;
				}
			}
			else if (type >= 0 && (NpcJieSuanManager.inst.IsDeath(num) || jsonData.instance.AvatarJsonData[friendList[i].ToString()].TryGetField("MenPai").I != type))
			{
				continue;
			}
			CyFriendCell component2 = Tools.InstantiateGameObject(cyNpcCell, npcCellParent.transform).GetComponent<CyFriendCell>();
			component2.Init(friendList[i]);
			friendCells.Add(component2);
		}
		isShowSelectTag = false;
		Quaternion localRotation = ((Component)sanJiao).transform.localRotation;
		((Quaternion)(ref localRotation)).Set(0f, 0f, 180f, 0f);
		CyUIMag.inst.cyEmail.cySendBtn.Hide();
		CyUIMag.inst.cyEmail.Restart();
		curSelectFriend = null;
	}

	public void ShowMoreSelect()
	{
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		isShowSelectTag = !isShowSelectTag;
		Quaternion localRotation;
		if (isShowSelectTag)
		{
			if (selectPanel.transform.childCount < 2)
			{
				Tools.InstantiateGameObject(cySelectCell, selectPanel.transform).GetComponent<CySelectCell>().Init("全部", (UnityAction)delegate
				{
					selectPanel.SetActive(false);
					curSelect.text = "全部";
					InitNpcList(-2);
				});
				if (KillManager.Inst.KillerModels.ContainsKey(1))
				{
					Tools.InstantiateGameObject(cySelectCell, selectPanel.transform).GetComponent<CySelectCell>().Init("风雨楼", (UnityAction)delegate
					{
						selectPanel.SetActive(false);
						curSelect.text = "风雨楼";
						InitNpcList(-5);
					});
				}
				Tools.InstantiateGameObject(cySelectCell, selectPanel.transform).GetComponent<CySelectCell>().Init("拍卖会", (UnityAction)delegate
				{
					selectPanel.SetActive(false);
					curSelect.text = "拍卖会";
					InitNpcList(-3);
				});
				Tools.InstantiateGameObject(cySelectCell, selectPanel.transform).GetComponent<CySelectCell>().Init("标记", (UnityAction)delegate
				{
					selectPanel.SetActive(false);
					curSelect.text = "标记";
					InitNpcList(-1);
				});
				Tools.InstantiateGameObject(cySelectCell, selectPanel.transform).GetComponent<CySelectCell>().Init("道侣", (UnityAction)delegate
				{
					selectPanel.SetActive(false);
					curSelect.text = "道侣";
					InitNpcList(-4);
				});
				foreach (JSONObject data in jsonData.instance.CyShiLiNameData.list)
				{
					Tools.InstantiateGameObject(cySelectCell, selectPanel.transform).GetComponent<CySelectCell>().Init(data["name"].Str, (UnityAction)delegate
					{
						selectPanel.SetActive(false);
						curSelect.text = data["name"].Str;
						InitNpcList(data["id"].I);
					});
				}
			}
			localRotation = ((Component)sanJiao).transform.localRotation;
			((Quaternion)(ref localRotation)).Set(0f, 0f, 0f, 0f);
			selectPanel.SetActive(true);
		}
		else
		{
			isShowSelectTag = false;
			selectPanel.SetActive(false);
			localRotation = ((Component)sanJiao).transform.localRotation;
			((Quaternion)(ref localRotation)).Set(0f, 0f, 180f, 0f);
		}
	}

	public void ClickCallBack()
	{
		if ((Object)(object)curSelectFriend != (Object)null)
		{
			curSelectFriend.isSelect = false;
			curSelectFriend.updateState();
		}
	}
}
