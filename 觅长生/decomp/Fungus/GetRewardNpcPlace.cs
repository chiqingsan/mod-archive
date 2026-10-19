using System;
using KillSystem;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "获取悬赏Npc位置", "获取悬赏Npc位置", 0)]
[AddComponentMenu("")]
public class GetRewardNpcPlace : Command
{
	[Tooltip("位置")]
	[VariableProperty(new Type[] { typeof(StringVariable) })]
	[SerializeField]
	protected StringVariable Place;

	public override void OnEnter()
	{
		if (KillManager.Inst.CurAcceptRewardOrder != null && !NpcJieSuanManager.inst.IsDeath(KillManager.Inst.CurAcceptRewardOrder.NpcId))
		{
			Place.Value = NpcJieSuanManager.inst.npcMap.GetRewardNpcSceneName(KillManager.Inst.CurAcceptRewardOrder.NpcId);
			if (Place.Value != "null")
			{
				KillManager.Inst.CurAcceptRewardOrder.SetTarGetPlace(Place.Value);
			}
		}
		else
		{
			Place.Value = "null";
		}
		Continue();
	}
}
