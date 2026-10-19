using KBEngine;
using UnityEngine;
using YSGame;

public class TianJieYunShouJiAnim : MonoBehaviour
{
	public Avatar TianJieAvatar;

	public Animator ShakeAnim;

	private int lastHP;

	private void Update()
	{
		if (TianJieAvatar != null)
		{
			if (TianJieAvatar.HP < lastHP)
			{
				ShakeAnim.Play("Shake");
				MusicMag.instance.PlayEffectMusic(26);
			}
			lastHP = TianJieAvatar.HP;
		}
	}
}
