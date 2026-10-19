using System;
using Fungus;
using UnityEngine;

[CommandInfo("YSNPCJiaoHu", "获取玩家向NPC说话的文本替换", "获取玩家向NPC说话的文本替换，赋值到指定的变量", 0)]
[AddComponentMenu("")]
public class CmdGetPlayerTalkToNPCReplace : Command
{
	[Tooltip("是否使用当前交互的NPC，如果使用，则忽略NPCID")]
	[SerializeField]
	protected bool UseNowJiaoHuNPC;

	[Tooltip("NPC的ID")]
	[SerializeField]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	protected IntegerVariable NPCID;

	[Tooltip("输入字符串")]
	[SerializeField]
	[VariableProperty(new Type[] { typeof(StringVariable) })]
	protected StringVariable InputTalkString;

	[Tooltip("输出字符串")]
	[SerializeField]
	[VariableProperty(new Type[] { typeof(StringVariable) })]
	protected StringVariable OutputTalkString;

	public override void OnEnter()
	{
		Flowchart flowchart = GetFlowchart();
		if ((Object)(object)InputTalkString == (Object)null || (Object)(object)OutputTalkString == (Object)null)
		{
			Debug.LogError((object)("CmdGetPlayerTalkToNPCReplace 出错，没有填写输入或者输出的变量，所在Flowchart: " + flowchart.GetParentName()));
			Continue();
			return;
		}
		if (UseNowJiaoHuNPC)
		{
			UINPCData nowJiaoHuNPC = UINPCJiaoHu.Inst.NowJiaoHuNPC;
			if (nowJiaoHuNPC == null)
			{
				Debug.LogError((object)("CmdGetPlayerTalkToNPCReplace 出错，当前没有正在交互的NPC，所在Flowchart: " + flowchart.GetParentName()));
				Continue();
				return;
			}
			OutputTalkString.Value = InputTalkString.Value.ReplacePlayerTalkToNPCWord(nowJiaoHuNPC);
		}
		else
		{
			if ((Object)(object)NPCID == (Object)null)
			{
				Debug.LogError((object)("CmdGetPlayerTalkToNPCReplace 出错，没有填写NPCID，所在Flowchart: " + flowchart.GetParentName()));
				Continue();
				return;
			}
			UINPCData uINPCData = new UINPCData(NPCEx.NPCIDToNew(NPCID.Value));
			uINPCData.RefreshData();
			OutputTalkString.Value = InputTalkString.Value.ReplacePlayerTalkToNPCWord(uINPCData);
		}
		Continue();
	}
}
