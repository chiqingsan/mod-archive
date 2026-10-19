using System;
using JSONClass;
using UnityEngine;

namespace KillSystem;

public class RewardOrder_OldNpcFactory : RewardOrderFactory
{
	public override RewardOrderModel Create(int id = 0)
	{
		RewardOrderModel rewardOrderModel = new RewardOrderModel();
		if (!CanCreate(id))
		{
			return rewardOrderModel;
		}
		KillFixedNpcData killFixedNpcData = KillFixedNpcData.DataDict[id];
		int num = NPCEx.NPCIDToNew(killFixedNpcData.AvaterID);
		int i = jsonData.instance.AvatarJsonData[num.ToString()]["Level"].I;
		KillRewardData killRewardData = KillRewardData.DataDict[i];
		int randomInt = Tools.instance.GetRandomInt(killRewardData.ShangJin[0], killRewardData.ShangJin[1]);
		rewardOrderModel.NpcId = num;
		rewardOrderModel.RewardOrderLevel = killRewardData.NanDu;
		rewardOrderModel.AddReputation = killRewardData.ShengWang;
		rewardOrderModel.RewardMoney = ShengWangShangJinData.DataDict[randomInt].ShiJiShangJin;
		rewardOrderModel.DestroyTime = DateTime.Parse(killFixedNpcData.EndTime);
		rewardOrderModel.Name = jsonData.instance.AvatarRandomJsonData[string.Concat(num)]["Name"].Str;
		return rewardOrderModel;
	}

	protected override bool CanCreate(int id)
	{
		if (KillManager.Inst.RewardOrderModels.ContainsKey(id))
		{
			return false;
		}
		if (!KillFixedNpcData.DataDict.ContainsKey(id))
		{
			Debug.LogError((object)$"固定杀手表中没有此Id{id}");
			return false;
		}
		KillFixedNpcData killFixedNpcData = KillFixedNpcData.DataDict[id];
		if (NPCEx.IsDeath(killFixedNpcData.AvaterID))
		{
			Debug.Log((object)$"此固定Npc已死亡{killFixedNpcData.AvaterID}");
			return false;
		}
		if (PlayerEx.Player.level < killFixedNpcData.JingJie[0] || PlayerEx.Player.level > killFixedNpcData.JingJie[1])
		{
			return false;
		}
		DateTime nowTime = PlayerEx.Player.worldTimeMag.getNowTime();
		if (nowTime < DateTime.Parse(killFixedNpcData.StarTime) || nowTime > DateTime.Parse(killFixedNpcData.EndTime))
		{
			return false;
		}
		if (killFixedNpcData.EventValue.Count > 0 && !ToolsEx.IsMatching(PlayerEx.Player.StaticValue.Value[killFixedNpcData.EventValue[0]], killFixedNpcData.EventValue[1], killFixedNpcData.fuhao))
		{
			return false;
		}
		int num = NPCEx.NPCIDToNew(killFixedNpcData.AvaterID);
		int i = jsonData.instance.AvatarJsonData[num.ToString()]["Level"].I;
		if (!KillRewardData.DataDict.ContainsKey(i))
		{
			Debug.LogError((object)$"Npc境界异常{i}，不能生成悬赏");
			return false;
		}
		KillRewardData killRewardData = KillRewardData.DataDict[i];
		int randomInt = Tools.instance.GetRandomInt(killRewardData.ShangJin[0], killRewardData.ShangJin[1]);
		if (!ShengWangShangJinData.DataDict.ContainsKey(randomInt))
		{
			Debug.LogError((object)$"Npc声望赏金异常{i}，不能生成悬赏");
			return false;
		}
		return true;
	}
}
