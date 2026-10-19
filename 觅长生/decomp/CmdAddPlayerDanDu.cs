using Fungus;
using UnityEngine;

[CommandInfo("YSPlayer", "增加玩家丹毒", "增加玩家丹毒", 0)]
[AddComponentMenu("")]
public class CmdAddPlayerDanDu : Command
{
	[Tooltip("丹毒量")]
	[SerializeField]
	protected int Count;

	public override void OnEnter()
	{
		PlayerEx.Player.AddDandu(Count);
		Continue();
	}
}
