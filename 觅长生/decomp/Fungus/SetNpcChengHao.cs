using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSNew/Set", "设置Npc称号", "设置Npc称号", 0)]
[AddComponentMenu("")]
public class SetNpcChengHao : Command
{
	[Tooltip("NpcId")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable NpcId;

	[Tooltip("ChengHaoId")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable ChengHaoId;

	public override void OnEnter()
	{
		NpcId.Value = NPCEx.NPCIDToNew(NpcId.Value);
		if (NpcJieSuanManager.inst.isCanJieSuan)
		{
			NpcJieSuanManager.inst.npcChengHao.UpDateChengHao(NpcId.Value, ChengHaoId.Value);
		}
		Continue();
	}
}
