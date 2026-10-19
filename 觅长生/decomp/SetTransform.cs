using UnityEngine;

public class SetTransform : MonoBehaviour
{
	public bool isLocal;

	public Vector3 SetPosition;

	public Vector3 SetRotation;

	public Vector3 SetScale;

	private void Start()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (isLocal)
		{
			((Component)this).transform.localPosition = SetPosition;
			((Component)this).transform.localEulerAngles = SetRotation;
			((Component)this).transform.localScale = SetScale;
		}
		else
		{
			((Component)this).transform.position = SetPosition;
			((Component)this).transform.eulerAngles = SetRotation;
			((Component)this).transform.localScale = SetScale;
		}
	}
}
