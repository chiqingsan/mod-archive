using Fungus;
using UnityEngine;

[CommandInfo("YSNPCJiaoHu", "获取左侧NPC列表人数", "获取左侧NPC列表人数，赋值到TmpValue", 0)]
[AddComponentMenu("")]
public class CmdGetNPCLeftListCount : Command
{
	public override void OnEnter()
	{
		GetFlowchart().SetIntegerVariable("TmpValue", UINPCJiaoHu.Inst.NPCList.NowShowedNPCCount);
		Continue();
	}
}
