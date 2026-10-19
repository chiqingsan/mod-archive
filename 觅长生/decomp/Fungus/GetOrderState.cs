using System;
using KillSystem;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "获取悬赏任务状态", "获取悬赏任务状态（0未接取，1已接取未完成，2已完成）", 0)]
[AddComponentMenu("")]
public class GetOrderState : Command
{
	[Tooltip("0未接取，1已接取未完成，2已完成")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable state;

	public override void OnEnter()
	{
		state.Value = KillManager.Inst.RewardOrderService.GetOrderTaskState();
		Continue();
	}
}
