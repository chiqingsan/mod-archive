using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "获取玩家代号", "获取玩家代号", 0)]
[AddComponentMenu("")]
public class GetPlayerDaiHao : Command
{
	public override void OnEnter()
	{
		GetFlowchart().SetStringVariable("daihao", PlayerEx.GetDaiHao());
		Continue();
	}
}
