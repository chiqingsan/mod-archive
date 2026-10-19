using KillSystem;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "发放悬赏任务奖励", "发放悬赏任务奖励", 0)]
[AddComponentMenu("")]
public class GiverOrderReward : Command
{
	public override void OnEnter()
	{
		KillManager.Inst.RewardOrderService.GiveOrderReward();
		Continue();
	}
}
