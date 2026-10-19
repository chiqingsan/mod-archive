using System;
using KillSystem;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "玩家加入风雨楼", "玩家加入风雨楼", 0)]
[AddComponentMenu("")]
public class PlayerJoinFengYuLou : Command
{
	[Tooltip("初始声望")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable StartShengWang;

	[Tooltip("初始总赏金")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable StartTotalRewardMoney;

	public override void OnEnter()
	{
		KillManager.Inst.PlayerJoinFengYuLou(StartShengWang.Value, StartTotalRewardMoney.Value);
		Continue();
	}
}
