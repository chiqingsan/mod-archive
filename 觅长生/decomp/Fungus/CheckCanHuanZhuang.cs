using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("洞府", "检测Npc能否换装", "检测Npc能否换装", 0)]
[AddComponentMenu("")]
public class CheckCanHuanZhuang : Command
{
	[Tooltip("NpcId")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable NpcId;

	[Tooltip("NpcId")]
	[VariableProperty(new Type[] { typeof(BooleanVariable) })]
	[SerializeField]
	protected BooleanVariable Result;

	public override void OnEnter()
	{
		NpcId.Value = NPCEx.NPCIDToNew(NpcId.Value);
		try
		{
			if (jsonData.instance.AvatarRandomJsonData[NpcId.Value.ToString()]["a_suit"].I < 80)
			{
				Result.Value = true;
			}
			else
			{
				Result.Value = false;
			}
		}
		catch (Exception ex)
		{
			Result.Value = false;
			Debug.LogError((object)ex);
		}
		Continue();
	}
}
