using KillSystem;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "接取刚才的悬赏的任务", "接取刚才的悬赏的任务", 0)]
[AddComponentMenu("")]
public class AccpetRewardOrder : Command
{
	public override void OnEnter()
	{
		if (KillManager.Inst.TempRewardOrder != null && KillManager.Inst.RewardOrderService.GetOrderTaskState() == 0)
		{
			if ((int)PlayerEx.Player.money >= KillManager.Inst.TempRewardOrder.RewardMoney / 2)
			{
				KillManager.Inst.RewardOrderService.AcceptRewardOrder(KillManager.Inst.TempRewardOrder);
			}
			else
			{
				UIPopTip.Inst.Pop("灵石不足");
			}
		}
		Continue();
	}
}
