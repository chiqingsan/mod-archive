using UnityEngine;
using YSGame;
using YSGame.Fight;

public class DuanTiAnimEvent : MonoBehaviour
{
	public AudioClip LeiAudio;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Hit()
	{
		FightScreenShake fightScreenShake = Object.FindObjectOfType<FightScreenShake>();
		if ((Object)(object)fightScreenShake != (Object)null)
		{
			fightScreenShake.Shake();
		}
		UIFightPanel.Inst.PlayerStatus.NoRefresh = false;
		MusicMag.instance.PlayEffectMusic(LeiAudio);
	}
}
