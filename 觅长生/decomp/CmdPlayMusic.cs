using Fungus;
using UnityEngine;
using YSGame;

[CommandInfo("YSTool", "播放音乐", "播放音乐", 0)]
[AddComponentMenu("")]
public class CmdPlayMusic : Command
{
	[Tooltip("音乐名字")]
	[SerializeField]
	protected string MusicName = "";

	[Tooltip("立即硬切")]
	[SerializeField]
	protected bool Immediately;

	public override void OnEnter()
	{
		if (Immediately)
		{
			MusicMag.instance.PlayMusicImmediately(MusicName);
		}
		else
		{
			MusicMag.instance.playMusic(MusicName);
		}
		Continue();
	}
}
