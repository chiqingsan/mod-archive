using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KillSystem.Killer;

namespace KillSystem.WTData;

[Serializable]
public class KillData
{
	public Dictionary<int, KillerModel> KillerModels = new Dictionary<int, KillerModel>();

	public Dictionary<int, RewardOrderModel> RewardOrderModels = new Dictionary<int, RewardOrderModel>();

	public DateTime NextRen = DateTime.Parse("0001-01-01");

	public DateTime NextDi = DateTime.Parse("0001-01-01");

	public DateTime NextTian = DateTime.Parse("0001-01-01");

	public RewardOrderModel TempRewardOrder;

	public bool IsOpenTrace;

	public RewardOrderModel CurAcceptRewardOrder;

	[OnDeserialized]
	private void AfterLoad(StreamingContext context)
	{
		KillManager.Inst.KillerModels = KillerModels;
		KillManager.Inst.RewardOrderModels = RewardOrderModels;
		KillManager.Inst.NextRen = NextRen;
		KillManager.Inst.NextDi = NextDi;
		KillManager.Inst.NextTian = NextTian;
		KillManager.Inst.TempRewardOrder = TempRewardOrder;
		KillManager.Inst.IsOpenTrace = IsOpenTrace;
		KillManager.Inst.CurAcceptRewardOrder = CurAcceptRewardOrder;
	}

	[OnSerializing]
	private void BeforeSave(StreamingContext context)
	{
		KillerModels = KillManager.Inst.KillerModels;
		RewardOrderModels = KillManager.Inst.RewardOrderModels;
		NextRen = KillManager.Inst.NextRen;
		NextDi = KillManager.Inst.NextDi;
		NextTian = KillManager.Inst.NextTian;
		TempRewardOrder = KillManager.Inst.TempRewardOrder;
		IsOpenTrace = KillManager.Inst.IsOpenTrace;
		CurAcceptRewardOrder = KillManager.Inst.CurAcceptRewardOrder;
	}
}
