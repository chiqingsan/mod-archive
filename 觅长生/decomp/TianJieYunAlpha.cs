using UnityEngine;
using YSGame.Fight;

public class TianJieYunAlpha : MonoBehaviour
{
	public UIFightAvatarStatus TianJieStatus;

	public SpriteRenderer SR;

	public float YunAlpha = 1f;

	public int MinAlpha = 100;

	public int MaxAlpha = 255;

	public float ChangeSpeed = 0.1f;

	public float TianJieHPBili = 0.3f;

	public bool CanChangeAlpha;

	private void Update()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)TianJieStatus.Avatar.HP / (float)TianJieStatus.Avatar.HP_Max;
		if (num <= TianJieHPBili)
		{
			YunAlpha = ((float)MinAlpha + (float)(MaxAlpha - MinAlpha) * (num / TianJieHPBili)) / 255f;
		}
		else
		{
			YunAlpha = 1f;
		}
		if (CanChangeAlpha && SR.color.a != YunAlpha)
		{
			float num2 = Mathf.Clamp01(SR.color.a + (YunAlpha - SR.color.a) / Mathf.Abs(YunAlpha - SR.color.a) * ChangeSpeed * Time.deltaTime);
			SR.color = new Color(SR.color.r, SR.color.g, SR.color.b, num2);
		}
	}
}
