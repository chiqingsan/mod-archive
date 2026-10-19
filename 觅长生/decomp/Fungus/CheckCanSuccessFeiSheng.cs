using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("Npc飞升", "测试该Npc能否飞升", "测试该Npc能否飞升", 0)]
[AddComponentMenu("")]
public class CheckCanSuccessFeiSheng : Command
{
	[Tooltip("NpcId")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable NpcId;

	[Tooltip("Result")]
	[VariableProperty(new Type[] { typeof(BooleanVariable) })]
	[SerializeField]
	protected BooleanVariable Result;

	public override void OnEnter()
	{
		int npcId = NPCEx.NPCIDToNew(NpcId.Value);
		Result.Value = NpcJieSuanManager.inst.npcTuPo.NpcFlyToSky(npcId);
		Continue();
	}
}
