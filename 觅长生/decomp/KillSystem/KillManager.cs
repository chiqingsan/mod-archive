using System;
using System.Collections.Generic;
using KillSystem.Killer;

namespace KillSystem;

public class KillManager
{
	private static KillManager _inst;

	public KillerService KillerService = new Killer_Service();

	public RewardOrderService RewardOrderService = new RewardOrder_Service();

	public Dictionary<int, KillerModel> KillerModels = new Dictionary<int, KillerModel>();

	public Dictionary<int, RewardOrderModel> RewardOrderModels = new Dictionary<int, RewardOrderModel>();

	public RewardOrderModel CurAcceptRewardOrder;

	public DateTime NextRen = DateTime.Parse("0001-01-01");

	public DateTime NextDi = DateTime.Parse("0001-01-01");

	public DateTime NextTian = DateTime.Parse("0001-01-01");

	public RewardOrderModel TempRewardOrder;

	public bool IsOpenTrace;

	public static KillManager Inst
	{
		get
		{
			if (_inst == null)
			{
				_inst = new KillManager();
			}
			return _inst;
		}
	}

	public void Restart()
	{
		_inst = new KillManager();
	}

	public bool AddRewardOrderModel(RewardOrderModel model)
	{
		if (model.NpcId != -1 && !RewardOrderModels.ContainsKey(model.NpcId))
		{
			RewardOrderModels.Add(model.NpcId, model);
			return true;
		}
		return false;
	}

	public void PlayerJoinFengYuLou(int shengWang, int money)
	{
		if (!KillerModels.ContainsKey(1))
		{
			KillerModel killerModel = new KillerModel();
			killerModel.Id = 1;
			killerModel.TotalRewardMoney = money;
			PlayerEx.AddShengWang(10, shengWang);
			KillerModels.Add(1, killerModel);
		}
	}
}
