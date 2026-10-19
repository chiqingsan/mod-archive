using System;
using KillSystem;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "设置悬赏任务状态", "设置悬赏任务状态", 0)]
[AddComponentMenu("")]
public class SetRewardOrderState : Command
{
	[Tooltip("状态(0失败，1成功)")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable State;

	public override void OnEnter()
	{
		if (KillManager.Inst.CurAcceptRewardOrder != null && KillManager.Inst.RewardOrderService.GetOrderTaskState() == 1)
		{
			if (State.Value == 0)
			{
				KillManager.Inst.RewardOrderService.GiveUpRewardOrder();
			}
			else
			{
				KillManager.Inst.RewardOrderService.CompleteRewardOrder();
			}
		}
		Continue();
	}
}
