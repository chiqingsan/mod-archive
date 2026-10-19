using System;
using System.Collections.Generic;
using UnityEngine;
using YSGame;

public class SkillYuanDi : MonoBehaviour
{
	public List<AudioClip> HuDunClip;

	public List<AudioClip> NormalClip;

	public AudioClip RemoveClip;

	public void removeSelf()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void removeParent()
	{
		try
		{
			if ((Object)(object)RemoveClip != (Object)null)
			{
				MusicMag.instance.PlayEffectMusic(RemoveClip);
			}
			if ((Object)(object)((Component)this).transform.parent != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)((Component)this).transform.parent).gameObject);
			}
			else
			{
				Debug.LogError((object)("特效移除父物体时出错:" + ((Object)((Component)this).gameObject).name + "父物体为空"));
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("特效移除父物体时出现异常:" + ex.Message + "\n" + ex.StackTrace + "\n"));
		}
	}

	public void PlayHit()
	{
		Debug.Log((object)$"[{Time.frameCount}] {((Component)this).gameObject.GetPath()} 触发了playhit");
		if (RoundManager.instance.IsShanBi)
		{
			RoundManager.instance.IsShanBi = false;
		}
		else
		{
			try
			{
				if (HuDunClip.Count > 0 && PlayerEx.Player.OtherAvatar.spell.HasBuff(5))
				{
					MusicMag.instance.PlayEffectMusic(HuDunClip.GetRandomOne());
				}
				else if (NormalClip.Count > 0)
				{
					MusicMag.instance.PlayEffectMusic(NormalClip.GetRandomOne());
				}
			}
			catch (Exception ex)
			{
				Debug.LogError((object)ex);
			}
		}
		MessageMag.Instance.Send(MessageName.MSG_Fight_Effect_Special);
		YSFuncList.Ints.Continue();
	}

	public void ScreenShake()
	{
		FightScreenShake fightScreenShake = Object.FindObjectOfType<FightScreenShake>();
		if ((Object)(object)fightScreenShake != (Object)null)
		{
			fightScreenShake.Shake();
		}
	}

	public void FlashBlack()
	{
		FightFlashBlack fightFlashBlack = Object.FindObjectOfType<FightFlashBlack>();
		if ((Object)(object)fightFlashBlack != (Object)null)
		{
			fightFlashBlack.Flash();
		}
	}
}
