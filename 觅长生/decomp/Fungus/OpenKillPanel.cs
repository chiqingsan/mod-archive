using UnityEngine;
using script.KillSystem;

namespace Fungus;

[CommandInfo("风雨楼", "打开悬赏任务界面", "打开悬赏任务界面", 0)]
[AddComponentMenu("")]
public class OpenKillPanel : Command
{
	public override void OnEnter()
	{
		KillUILogic.Open();
		Continue();
	}

	public override Color GetButtonColor()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return Color32.op_Implicit(new Color32((byte)184, (byte)210, (byte)235, byte.MaxValue));
	}
}
