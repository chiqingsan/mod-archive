using KillSystem;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "放弃悬赏任务", "放弃悬赏任务", 0)]
[AddComponentMenu("")]
public class GiveUpRewardOrder : Command
{
	public override void OnEnter()
	{
		if (KillManager.Inst.CurAcceptRewardOrder != null && KillManager.Inst.RewardOrderService.GetOrderTaskState() == 1)
		{
			KillManager.Inst.RewardOrderService.GiveUpRewardOrder();
		}
		Continue();
	}
}
