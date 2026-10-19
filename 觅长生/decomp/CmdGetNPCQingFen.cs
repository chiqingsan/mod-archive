using System;
using Fungus;
using UnityEngine;

[CommandInfo("YSNPCJiaoHu", "获取NPC情分", "获取NPC情分，复制到TmpValue", 0)]
[AddComponentMenu("")]
public class CmdGetNPCQingFen : Command
{
	[Tooltip("NPC的ID")]
	[SerializeField]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	protected IntegerVariable NPCID;

	public override void OnEnter()
	{
		GetFlowchart().SetIntegerVariable("TmpValue", NPCEx.GetQingFen(NPCID.Value));
		Continue();
	}
}
