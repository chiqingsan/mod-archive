using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "获取玩家总赏金", "获取玩家总赏金", 0)]
[AddComponentMenu("")]
public class GetPlayerRewardMoney : Command
{
	[Tooltip("总赏金")]
	[VariableProperty(new Type[] { typeof(StringVariable) })]
	[SerializeField]
	protected StringVariable Money;

	public override void OnEnter()
	{
		Money.Value = PlayerEx.GetRewardMoney().ToString();
		Continue();
	}
}
