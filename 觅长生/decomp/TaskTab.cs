using UnityEngine;

public class TaskTab : MonoBehaviour
{
	public GameObject 已选择;

	public GameObject 未选择;

	public void Click()
	{
		未选择.SetActive(false);
		已选择.SetActive(true);
	}

	public void SetNoSelect()
	{
		未选择.SetActive(true);
		已选择.SetActive(false);
	}
}
