using Fungus;
using UnityEngine;

[CommandInfo("YSPlayer", "进入飞升生平界面", "进入飞升生平界面", 0)]
[AddComponentMenu("")]
public class CmdWarpShengPing : Command
{
	public override void OnEnter()
	{
		Tools.instance.loadOtherScenes("EndScene");
		Continue();
	}
}
