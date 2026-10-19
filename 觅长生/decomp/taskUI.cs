using System.Collections.Generic;
using UnityEngine;

public class taskUI : MonoBehaviour, IESCClose
{
	[SerializeField]
	private List<TaskTab> TaskTabList = new List<TaskTab>();

	[SerializeField]
	private TaskUIManager taskUIManager;

	private void Awake()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localPosition = Vector3.zero;
		((Component)this).transform.localScale = Vector3.one;
		((Component)this).transform.SetAsLastSibling();
		openTaskPanel();
	}

	public static void OpenTask()
	{
		ResManager.inst.LoadPrefab("TaskPanel").Inst(((Component)NewUICanvas.Inst).gameObject.transform);
	}

	public void selectTab(int index)
	{
		switch (index)
		{
		case 0:
			taskUIManager.initTaskList();
			break;
		case 1:
			taskUIManager.initTaskList(0);
			break;
		case 2:
			taskUIManager.initTaskList(2);
			break;
		}
		for (int i = 0; i < TaskTabList.Count; i++)
		{
			if (i != index)
			{
				TaskTabList[i].SetNoSelect();
			}
		}
	}

	public void openTaskPanel()
	{
		if (Tools.instance.canClick())
		{
			Tools.canClickFlag = false;
			TaskTabList[0].Click();
			selectTab(0);
			ESCCloseManager.Inst.RegisterClose(this);
		}
	}

	public void closeTaskPanel()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void OnDestroy()
	{
		ESCCloseManager.Inst.UnRegisterClose(this);
		Tools.canClickFlag = true;
	}

	public bool TryEscClose()
	{
		closeTaskPanel();
		return true;
	}
}
