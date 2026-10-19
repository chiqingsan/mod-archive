using GUIPackage;
using JSONClass;
using KBEngine;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSFight", "战斗中释放技能", "战斗中释放技能", 0)]
[AddComponentMenu("")]
public class FightPutSkill : Command
{
	[Tooltip("增加的角色(0主角,1敌人)")]
	[SerializeField]
	protected int type;

	[Tooltip("技能ID")]
	[SerializeField]
	protected int skillID;

	public override void OnEnter()
	{
		Avatar player = Tools.instance.getPlayer();
		GUIPackage.Skill skill = new GUIPackage.Skill(skillID, 0, 10);
		Avatar avatar = player.OtherAvatar;
		if (type == 0)
		{
			avatar = player;
		}
		string script = _skillJsonData.DataDict[skillID].script;
		if (script == "SkillAttack")
		{
			skill.PutingSkill(avatar, avatar.OtherAvatar);
		}
		else if (script == "SkillSelf")
		{
			skill.PutingSkill(avatar, avatar);
		}
		Continue();
	}
}
