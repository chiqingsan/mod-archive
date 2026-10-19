using Spine.Unity;
using UnityEngine;

public class InitAvatar : MonoBehaviour
{
	public int SexType = 1;

	public SkeletonAnimation SkeletonAnimation;

	public MeshRenderer MeshRenderer;

	private void Awake()
	{
		((Component)SkeletonAnimation).gameObject.SetActive(false);
		((Renderer)MeshRenderer).sortingOrder = 0;
		((SkeletonRenderer)SkeletonAnimation).fixDrawOrder = true;
		if (SexType == 1)
		{
			((SkeletonRenderer)SkeletonAnimation).skeletonDataAsset = ResManager.inst.LoadABSkeletonDataAsset(SexType, "Fight_new_sanxiu_2_SkeletonData");
		}
		else
		{
			((SkeletonRenderer)SkeletonAnimation).skeletonDataAsset = ResManager.inst.LoadABSkeletonDataAsset(SexType, "Fight_womensanxiu_0_SkeletonData");
		}
		((Component)SkeletonAnimation).gameObject.SetActive(true);
	}
}
