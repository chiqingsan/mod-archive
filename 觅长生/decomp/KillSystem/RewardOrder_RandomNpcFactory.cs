using System.Collections.Generic;
using JSONClass;
using UnityEngine;

namespace KillSystem;

public class RewardOrder_RandomNpcFactory : RewardOrderFactory
{
	public override RewardOrderModel Create(int id = 0)
	{
		RewardOrderModel rewardOrderModel = new RewardOrderModel();
		if (!CanCreate(id))
		{
			return rewardOrderModel;
		}
		int npcId = GetNpcId(id);
		int i = jsonData.instance.AvatarJsonData[npcId.ToString()]["Level"].I;
		KillRewardData killRewardData = KillRewardData.DataDict[i];
		int randomInt = Tools.instance.GetRandomInt(killRewardData.ShangJin[0], killRewardData.ShangJin[1]);
		rewardOrderModel.Name = jsonData.instance.AvatarRandomJsonData[string.Concat(npcId)]["Name"].Str;
		rewardOrderModel.NpcId = npcId;
		rewardOrderModel.RewardOrderLevel = killRewardData.NanDu;
		rewardOrderModel.AddReputation = killRewardData.ShengWang;
		rewardOrderModel.RewardMoney = ShengWangShangJinData.DataDict[randomInt].ShiJiShangJin;
		rewardOrderModel.DestroyTime = PlayerEx.Player.worldTimeMag.getNowTime().AddYears(GetCanKeepTime(killRewardData.NanDu));
		switch (rewardOrderModel.RewardOrderLevel)
		{
		case 3:
			KillManager.Inst.NextTian = PlayerEx.Player.worldTimeMag.getNowTime().AddYears(200);
			break;
		case 2:
			KillManager.Inst.NextDi = PlayerEx.Player.worldTimeMag.getNowTime().AddYears(80);
			break;
		case 1:
			KillManager.Inst.NextRen = PlayerEx.Player.worldTimeMag.getNowTime().AddYears(20);
			break;
		}
		return rewardOrderModel;
	}

	private int GetNpcId(int id)
	{
		List<int> liuPai = KillRandomNpcData.DataDict[id].LiuPai;
		List<int> level = KillRandomNpcData.DataDict[id].Level;
		List<int> xingGe = KillRandomNpcData.DataDict[id].XingGe;
		int num = liuPai[GetInt(0, liuPai.Count)];
		int num2 = level[GetInt(0, level.Count)];
		int num3 = xingGe[GetInt(0, xingGe.Count)];
		foreach (JSONObject item in jsonData.instance.AvatarJsonData.list)
		{
			if (item["id"].I >= 20000 && (!item.HasField("isImportant") || !item["isImportant"].b) && !KillManager.Inst.RewardOrderModels.ContainsKey(item["id"].I) && item["Level"].I == num2 && item["XingGe"].I == num3 && item["LiuPai"].I == num)
			{
				return item["id"].I;
			}
		}
		return FactoryManager.inst.npcFactory.CreateNpc(num, num2, num3);
	}

	private int GetInt(int a, int b)
	{
		return NpcJieSuanManager.inst.getRandomInt(a, b - 1);
	}

	protected override bool CanCreate(int id)
	{
		if (KillManager.Inst.RewardOrderModels.ContainsKey(id))
		{
			return false;
		}
		if (!KillRandomNpcData.DataDict.ContainsKey(id))
		{
			Debug.LogError((object)$"不存在此悬赏难度：{id}");
			return false;
		}
		int npcId = GetNpcId(id);
		if (npcId <= 0)
		{
			Debug.LogError((object)$"获取npcId出错：{npcId},悬赏难度为：{id}");
			return false;
		}
		int i = jsonData.instance.AvatarJsonData[npcId.ToString()]["Level"].I;
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
