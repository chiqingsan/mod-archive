using System;
using JSONClass;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSTools", "检测是否可以截杀", "检测是否可以截杀", 0)]
[AddComponentMenu("")]
public class CanJieSha : Command
{
	[Tooltip("NpcId")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable NpcId;

	[Tooltip("结果")]
	[VariableProperty(new Type[] { typeof(BooleanVariable) })]
	[SerializeField]
	protected BooleanVariable Result;

	public override void OnEnter()
	{
		Result.Value = false;
		try
		{
			int npcid = NPCEx.NPCIDToNew(NpcId.Value);
			int key = NPCEx.NPCIDToOld(npcid);
			if (!NPCEx.IsDeath(npcid))
			{
				if (NPCImportantDate.DataDict.ContainsKey(key))
				{
					NPCImportantDate nPCImportantDate = NPCImportantDate.DataDict[key];
					if (nPCImportantDate.EventValue.Count == 2 && ToolsEx.IsMatching(PlayerEx.Player.StaticValue.Value[nPCImportantDate.EventValue[0]], nPCImportantDate.EventValue[1], nPCImportantDate.fuhao))
					{
						Result.Value = true;
					}
				}
				else
				{
					Result.Value = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)ex);
			Result.Value = false;
		}
		Continue();
	}
}
