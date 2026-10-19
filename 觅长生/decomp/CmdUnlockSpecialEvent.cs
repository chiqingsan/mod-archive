using Fungus;
using UnityEngine;
using YSGame.TuJian;

[CommandInfo("YSTool", "解锁跨存档特殊事件", "解锁跨存档特殊事件(如天赋解锁)", 0)]
[AddComponentMenu("")]
public class CmdUnlockSpecialEvent : Command
{
	[SerializeField]
	[Tooltip("事件名")]
	protected string EventName;

	public override void OnEnter()
	{
		TuJianManager.Inst.UnlockSpecialEvent(EventName);
		YSNewSaveSystem.Save("TuJianSave.json", TuJianManager.Inst.TuJianSave, autoPath: false);
		Continue();
	}
}
