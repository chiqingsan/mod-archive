using UnityEngine;
using UnityEngine.UI;

public class TaskRenWuCell : MonoBehaviour
{
	[SerializeField]
	private GameObject DefaultImage;

	[SerializeField]
	private GameObject ClikeImage;

	[SerializeField]
	private Text TaskName;

	private JSONObject CurTask;

	public GameObject NewTag;

	[SerializeField]
	private TaskDescManager taskDescManager;

	private bool isChuanWen;

	private void Awake()
	{
		TaskUIManager.inst.TaskList.Add(this);
	}

	public void Click()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		DefaultImage.SetActive(false);
		((Graphic)TaskName).color = new Color(0.94509804f, 73f / 85f, 0.6627451f);
		ClikeImage.SetActive(true);
		if (CurTask != null && NewTag.activeSelf)
		{
			CurTask.SetField("NewTask", val: false);
			NewTag.SetActive(false);
		}
		if (TaskUIManager.inst.getCurType() == 1)
		{
			taskDescManager.setCurTaskDesc(CurTask);
			TaskUIManager.inst.setCurSelectIsChuanWen(flag: false);
		}
		else if (TaskUIManager.inst.getCurType() == 0)
		{
			if (getIsChuanWen())
			{
				taskDescManager.setChuanWenWenTuo(CurTask);
				TaskUIManager.inst.setCurSelectIsChuanWen(flag: true);
			}
			else
			{
				taskDescManager.setChuanMiaoShu(CurTask);
				TaskUIManager.inst.setCurSelectIsChuanWen(flag: false);
			}
		}
		else
		{
			taskDescManager.setWeiTuoDesc(CurTask);
			TaskUIManager.inst.setCurSelectIsChuanWen(flag: false);
		}
		TaskUIManager.inst.setCurTaskID(CurTask["id"].I);
		TaskUIManager.inst.setCurTask(CurTask);
		taskDescManager.checkIsZhuiZhong();
		foreach (TaskRenWuCell task in TaskUIManager.inst.TaskList)
		{
			if (!((Object)(object)task == (Object)(object)this))
			{
				task.SetNoSelect();
			}
		}
	}

	public void SetNoSelect()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		DefaultImage.SetActive(true);
		((Graphic)TaskName).color = new Color(0.70980394f, 0.94509804f, 78f / 85f);
		ClikeImage.SetActive(false);
	}

	public void setTaskName(string name)
	{
		TaskName.text = name;
	}

	public void setTaskInfo(JSONObject task)
	{
		CurTask = task;
	}

	public void setIsChuanWen(bool flag)
	{
		isChuanWen = flag;
	}

	public bool getIsChuanWen()
	{
		return isChuanWen;
	}

	public void setWeTuoInfo(JSONObject WeiTuo)
	{
		CurTask = WeiTuo;
	}
}
