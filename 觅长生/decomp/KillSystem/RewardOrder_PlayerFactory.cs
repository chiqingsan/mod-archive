using JSONClass;
using UnityEngine;

namespace KillSystem;

public class RewardOrder_PlayerFactory : RewardOrderFactory
{
	public override RewardOrderModel Create(int id = 0)
	{
		RewardOrderModel rewardOrderModel = new RewardOrderModel();
		if (!CanCreate(id))
		{
			return rewardOrderModel;
		}
		(int, int) rewardOrder = GetRewardOrder();
		int level = PlayerEx.Player.level;
		rewardOrderModel.RewardOrderLevel = KillRewardData.DataDict[level].NanDu;
		rewardOrderModel.AddReputation = KillRewardData.DataDict[level].ShengWang;
		rewardOrderModel.Name = PlayerEx.Player.name;
		rewardOrderModel.NpcId = 1;
		rewardOrderModel.RewardMoney = rewardOrder.Item2;
		rewardOrderModel.IsGuDing = true;
		return rewardOrderModel;
	}

	private (int, int) GetRewardOrder()
	{
		int ningZhouShengWang = PlayerEx.GetNingZhouShengWang();
		int shangJinPingFen = PlayerEx.GetShangJinPingFen(0);
		PlayerEx.CalcXuanShang(ningZhouShengWang, shangJinPingFen, out var shangjin, out var _);
		return (shangJinPingFen, shangjin);
	}

	protected override bool CanCreate(int id)
	{
		if (KillManager.Inst.RewardOrderModels.ContainsKey(id))
		{
			return false;
		}
		if (GetRewardOrder().Item2 <= 0)
		{
			Debug.Log((object)"玩家没有赏金，不能生成悬赏");
			return false;
		}
		int level = PlayerEx.Player.level;
		if (!KillRewardData.DataDict.ContainsKey(level))
		{
			Debug.LogError((object)$"玩家境界异常{level}，不能生成悬赏");
			return false;
		}
		return true;
	}
}
