using KBEngine;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSFight", "战斗获取玩家或对方血量，并赋值TempValue", "战斗获取玩家或对方血量，并赋值TempValue", 0)]
[AddComponentMenu("")]
public class FightGetCurHp : Command
{
	[Tooltip("增加的角色(0主角,1敌人)")]
	[SerializeField]
	protected int type;

	public override void OnEnter()
	{
		Avatar player = Tools.instance.getPlayer();
		Avatar avatar = player.OtherAvatar;
		if (type == 0)
		{
			avatar = player;
		}
		GetFlowchart().SetIntegerVariable("TempValue", avatar.HP);
		Continue();
	}
}
