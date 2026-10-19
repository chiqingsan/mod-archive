using Fungus;
using UnityEngine;

[CommandInfo("渡劫", "开始天劫", "开始天劫", 0)]
[AddComponentMenu("")]
public class CmdOpenTianJie : Command
{
	public override void OnEnter()
	{
		UIDuJieZhunBei.OpenPanel(isDuJie: true);
		Continue();
	}
}
