using System.Collections.Generic;
using Bag;
using JSONClass;
using KBEngine;
using UnityEngine;

public class CangKuBag : BaseBag2
{
	[HideInInspector]
	public CangKuData CangKu;

	public override void Init(int npcId, bool isPlayer = false)
	{
		_player = Tools.instance.getPlayer();
		ItemType = Bag.ItemType.丹药;
		NpcId = 0;
		IsPlayer = isPlayer;
		CreateTempList();
		MLoopListView.InitListView(GetCount(MItemTotalCount), base.OnGetItemByIndex);
		ItemType = Bag.ItemType.全部;
		ItemQuality = ItemQuality.全部;
		JiaoYiSkillType = JiaoYiSkillType.全部;
		LianQiCaiLiaoYinYang = LianQiCaiLiaoYinYang.全部;
		LianQiCaiLiaoType = LianQiCaiLiaoType.全部;
		InitLeftFilter();
		UpdateLeftFilter(LeftList[0]);
	}

	private void UpdateLeftFilter(BaseFilterLeft filterLeft)
	{
		foreach (BaseFilterLeft left in LeftList)
		{
			if ((Object)(object)left == (Object)(object)filterLeft)
			{
				left.IsSelect = true;
				ItemType = left.ItemType;
				UpdateItem();
				UpdateTopFilter();
			}
			else
			{
				left.IsSelect = false;
			}
			left.UpdateState();
		}
	}

	public override void CreateTempList()
	{
		TempBagList = new List<ITEM_INFO>();
		List<ITEM_INFO> list = new List<ITEM_INFO>();
		if (IsPlayer)
		{
			foreach (ITEM_INFO value in _player.itemList.values)
			{
				if (value.itemCount == 0)
				{
					list.Add(value);
				}
				else if (_ItemJsonData.DataDict.ContainsKey(value.itemId))
				{
					ITEM_INFO iTEM_INFO = new ITEM_INFO();
					iTEM_INFO.itemId = value.itemId;
					iTEM_INFO.itemCount = value.itemCount;
					iTEM_INFO.uuid = value.uuid;
					if (value.Seid != null)
					{
						iTEM_INFO.Seid = value.Seid.Copy();
					}
					TempBagList.Add(iTEM_INFO);
				}
			}
		}
		else
		{
			foreach (ITEM_INFO value2 in CangKu.ItemList.values)
			{
				TempBagList.Add(value2);
			}
		}
		foreach (ITEM_INFO item in list)
		{
			_player.itemList.values.Remove(item);
		}
	}
}
