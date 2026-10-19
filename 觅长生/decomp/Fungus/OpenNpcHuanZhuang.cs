using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("洞府", "打开Npc换装", "打开Npc换装", 0)]
[AddComponentMenu("")]
public class OpenNpcHuanZhuang : Command
{
	[Tooltip("NpcId")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable NpcId;

	public override void OnEnter()
	{
		SetFaceUI.Open(NpcId.Value);
		Continue();
	}
}
