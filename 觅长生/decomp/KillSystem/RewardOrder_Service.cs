using System;
using System.Collections.Generic;
using JSONClass;
using UnityEngine;

namespace KillSystem;

public class RewardOrder_Service : RewardOrderService
{
	private readonly RewardOrderFactory_PoolService pool = new RewardOrderFactory_PoolService();

	public override void CreateOrder()
	{
		int num = 5 - KillManager.Inst.RewardOrderModels.Count;
		if (num <= 0)
		{
			return;
		}
		RewardOrderFactory rewardOrderFactory = pool.Get<RewardOrder_OldNpcFactory>();
		RewardOrderModel rewardOrderModel = null;
		foreach (KillFixedNpcData data in KillFixedNpcData.DataList)
		{
			rewardOrderModel = rewardOrderFactory.Create(data.id);
			if (KillManager.Inst.AddRewardOrderModel(rewardOrderModel))
			{
				num--;
				if (num <= 0)
				{
					return;
				}
			}
		}
		rewardOrderFactory = pool.Get<RewardOrder_PlayerFactory>();
		rewardOrderModel = rewardOrderFactory.Create();
		if (KillManager.Inst.AddRewardOrderModel(rewardOrderModel))
		{
			num--;
			if (num <= 0)
			{
				return;
			}
		}
		rewardOrderFactory = pool.Get<RewardOrder_RandomNpcFactory>();
		while (GetCanCreateLevel() > 0 && num > 0)
		{
			rewardOrderModel = rewardOrderFactory.Create(GetCanCreateLevel());
			if (KillManager.Inst.AddRewardOrderModel(rewardOrderModel))
			{
				num--;
				continue;
			}
			Debug.LogError((object)"创建随机Npc悬赏失败");
			break;
		}
	}

	private int GetCanCreateLevel()
	{
		DateTime nowTime = PlayerEx.Player.worldTimeMag.getNowTime();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (KillManager.Inst.NextTian <= nowTime && PlayerEx.Player.StaticValue.Value[2100] == 1)
		{
			num = 1;
		}
		if (KillManager.Inst.NextDi <= nowTime)
		{
			num2 = 1;
		}
		if (KillManager.Inst.NextRen <= nowTime)
		{
			num3 = 1;
		}
		if (num == 1)
		{
			bool flag = false;
			foreach (RewardOrderModel value in KillManager.Inst.RewardOrderModels.Values)
			{
				if (value.IsGuDing && value.RewardOrderLevel == 3)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				if (num2 == 1)
				{
					return 2;
				}
				if (num3 == 1)
				{
					return 1;
				}
				return 3;
			}
			return 3;
		}
		if (num2 == 1)
		{
			return 2;
		}
		if (num3 == 1)
		{
			return 1;
		}
		return 0;
	}

	protected override bool IsOverdue(RewardOrderModel orderModel)
	{
		if (orderModel.NpcId == -1)
		{
			Debug.LogError((object)"无效的悬赏数据，需要移除");
			return true;
		}
		if (orderModel.OverdueTime < PlayerEx.Player.worldTimeMag.getNowTime())
		{
			return true;
		}
		return false;
	}

	protected override bool AddRewardOrder(RewardOrderModel orderModel)
	{
		if (KillManager.Inst.RewardOrderModels == null)
		{
			Debug.LogError((object)"KillManager.Inst.RewardOrderModels = null");
			return false;
		}
		if (KillManager.Inst.RewardOrderModels.ContainsKey(orderModel.NpcId))
		{
			Debug.LogError((object)$"悬赏中已存在相同的NpcId{orderModel.NpcId}");
			return false;
		}
		KillManager.Inst.RewardOrderModels.Add(orderModel.NpcId, orderModel);
		return true;
	}

	protected override bool RemoveRewardOrder(RewardOrderModel orderModel)
	{
		if (KillManager.Inst.RewardOrderModels == null)
		{
			Debug.LogError((object)"KillManager.Inst.RewardOrderModels = null");
			return false;
		}
		if (!KillManager.Inst.RewardOrderModels.ContainsKey(orderModel.NpcId))
		{
			Debug.LogError((object)$"悬赏中不存在此NpcId{orderModel.NpcId}的悬赏");
			return false;
		}
		KillManager.Inst.RewardOrderModels.Remove(orderModel.NpcId);
		return true;
	}

	public override bool AcceptRewardOrder(RewardOrderModel orderModel)
	{
		if (orderModel == null || orderModel.NpcId == -1 || !KillManager.Inst.RewardOrderModels.ContainsKey(orderModel.NpcId))
		{
			Debug.LogError((object)"无效的悬赏数据,接取失败");
			return false;
		}
		if (HasAcceptOrder())
		{
			Debug.LogError((object)"已有悬赏任务，无法接取");
			return false;
		}
		if (orderModel.NpcId == 1)
		{
			orderModel.OverdueTime = PlayerEx.Player.worldTimeMag.getNowTime().AddYears(50);
		}
		else
		{
			orderModel.OverdueTime = PlayerEx.Player.worldTimeMag.getNowTime().AddYears(KillRandomNpcData.DataDict[orderModel.RewardOrderLevel].Time);
			KillManager.Inst.RewardOrderModels.Remove(orderModel.NpcId);
		}
		KillManager.Inst.CurAcceptRewardOrder = orderModel;
		KillManager.Inst.TempRewardOrder = null;
		PlayerEx.Player.AddMoney(-orderModel.RewardMoney / 2);
		KillManager.Inst.IsOpenTrace = true;
		return true;
	}

	public override bool GiveUpRewardOrder()
	{
		if (KillManager.Inst.CurAcceptRewardOrder == null)
		{
			return false;
		}
		KillManager.Inst.CurAcceptRewardOrder = null;
		KillManager.Inst.IsOpenTrace = false;
		return true;
	}

	public override bool CompleteRewardOrder()
	{
		if (KillManager.Inst.CurAcceptRewardOrder == null)
		{
			return false;
		}
		KillManager.Inst.CurAcceptRewardOrder.IsComplete = true;
		return true;
	}

	public override bool GiveOrderReward()
	{
		if (KillManager.Inst.CurAcceptRewardOrder == null)
		{
			return false;
		}
		int num = KillManager.Inst.CurAcceptRewardOrder.RewardMoney * 3 / 2;
		PlayerEx.Player.AddMoney(num);
		PlayerEx.AddShengWang(10, KillManager.Inst.CurAcceptRewardOrder.AddReputation, show: true);
		KillManager.Inst.CurAcceptRewardOrder = null;
		KillManager.Inst.KillerModels[1].TotalRewardMoney += num;
		KillManager.Inst.IsOpenTrace = false;
		return true;
	}

	public override bool HasAcceptOrder()
	{
		if (KillManager.Inst.CurAcceptRewardOrder == null)
		{
			return false;
		}
		return true;
	}

	public override bool IsRewarded(int npcId)
	{
		return KillManager.Inst.RewardOrderModels.ContainsKey(npcId);
	}

	public override void UpdateRewardOrderList()
	{
		DateTime nowTime = PlayerEx.Player.worldTimeMag.getNowTime();
		List<int> list = new List<int>();
		foreach (int key in KillManager.Inst.RewardOrderModels.Keys)
		{
			if (key == 1)
			{
				if (PlayerEx.GetShangJinPingFen(key) == 1)
				{
					list.Add(1);
				}
			}
			else if (nowTime >= KillManager.Inst.RewardOrderModels[key].DestroyTime || NPCEx.IsDeath(key))
			{
				list.Add(key);
			}
		}
		foreach (int item in list)
		{
			RemoveRewardOrder(KillManager.Inst.RewardOrderModels[item]);
		}
		if (KillManager.Inst.RewardOrderModels.Count < 5)
		{
			CreateOrder();
		}
	}

	public override void UpdateAcceptedRewardOrder()
	{
		if (KillManager.Inst.CurAcceptRewardOrder != null && KillManager.Inst.CurAcceptRewardOrder.NpcId != -1 && !KillManager.Inst.CurAcceptRewardOrder.IsComplete)
		{
			if (NPCEx.IsDeath(KillManager.Inst.CurAcceptRewardOrder.NpcId))
			{
				GiveUpRewardOrder();
				PlayerEx.Player.emailDateMag.AuToSendToPlayer(3067, 1002, 1002, NpcJieSuanManager.inst.JieSuanTime);
			}
			else if (KillManager.Inst.CurAcceptRewardOrder.OverdueTime < PlayerEx.Player.worldTimeMag.getNowTime())
			{
				GiveUpRewardOrder();
				PlayerEx.Player.emailDateMag.AuToSendToPlayer(3067, 1003, 1003, NpcJieSuanManager.inst.JieSuanTime);
			}
		}
	}

	public override int GetOrderTaskState()
	{
		if (KillManager.Inst.CurAcceptRewardOrder == null)
		{
			return 0;
		}
		if (KillManager.Inst.CurAcceptRewardOrder.IsComplete)
		{
			return 2;
		}
		return 1;
	}
}
