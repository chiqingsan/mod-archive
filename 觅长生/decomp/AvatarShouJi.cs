using Spine;
using Spine.Unity;
using UnityEngine;

public class AvatarShouJi : MonoBehaviour
{
	public Vector3 OriPos;

	public float BackDir;

	public float MoveSpeed;

	private float MaxSpeed = 10f;

	public float G;

	public float ImpulseStrength;

	public SkeletonAnimation AvatarSkeleton;

	private float moveLen;

	private float backCD;

	private void Start()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		AvatarSkeleton = ((Component)this).GetComponentInChildren<SkeletonAnimation>();
		if ((Object)(object)AvatarSkeleton != (Object)null)
		{
			AvatarSkeleton.AnimationState.Start += new TrackEntryDelegate(AnimationState_Start);
		}
		ImpulseStrength = 5f;
		OriPos = ((Component)this).gameObject.transform.position;
		if (OriPos.x < 0f)
		{
			BackDir = -1f;
		}
		else
		{
			BackDir = 1f;
		}
		G = BackDir * -50f;
	}

	private void AnimationState_Start(TrackEntry trackEntry)
	{
		if (trackEntry.Animation.Name.ToLower().Contains("hit"))
		{
			StartBack();
		}
	}

	private void Update()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		backCD -= Time.deltaTime;
		Vector3 position = ((Component)this).transform.position;
		MoveSpeed += G * Time.deltaTime;
		position.x += MoveSpeed * Time.deltaTime;
		moveLen = (position.x - OriPos.x) * BackDir;
		if (moveLen <= 0f)
		{
			position.x = OriPos.x;
			MoveSpeed = 0f;
		}
		((Component)this).transform.position = position;
	}

	public void StartBack(float strength = 1f)
	{
		if (!(backCD > 0f))
		{
			backCD = 0.1f;
			float num = Mathf.Pow(strength, 0.3f);
			MoveSpeed += BackDir * num * ImpulseStrength;
			MoveSpeed = Mathf.Clamp(MoveSpeed, 0f - MaxSpeed, MaxSpeed);
		}
	}
}
