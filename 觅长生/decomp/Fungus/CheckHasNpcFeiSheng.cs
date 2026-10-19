using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("Npc飞升", "检测当前是否有邀请过玩家的Npc飞升", "检测当前是否有邀请过玩家的Npc飞升", 0)]
[AddComponentMenu("")]
public class CheckHasNpcFeiSheng : Command
{
	[Tooltip("NpcId")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable NpcId;

	public override void OnEnter()
	{
		int year = PlayerEx.Player.worldTimeMag.getNowTime().Year;
		NpcId.Value = 0;
		foreach (int key in NpcJieSuanManager.inst.ZhunBeiFeiShengNpc.Keys)
		{
			if (NpcJieSuanManager.inst.ZhunBeiFeiShengNpc[key].Year - 1 == year)
			{
				NpcId.Value = key;
				break;
			}
		}
		Continue();
	}
}
