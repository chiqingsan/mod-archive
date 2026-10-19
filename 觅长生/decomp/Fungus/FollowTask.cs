using System;
using JSONClass;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSTask", "追踪任务", "追踪任务", 0)]
[AddComponentMenu("")]
public class FollowTask : Command
{
	[Tooltip("需要追踪的任务ID")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable TaskId;

	[Tooltip("顶替当前任务")]
	[VariableProperty(new Type[] { typeof(BooleanVariable) })]
	[SerializeField]
	protected BooleanVariable Boolean;

	public override void OnEnter()
	{
		try
		{
			if (!Boolean.Value && PlayerEx.Player.TaskZhuiZhong.HasField("CurTaskID") && PlayerEx.Player.TaskZhuiZhong["CurTaskID"].I != -1)
			{
				Continue();
				return;
			}
			if (TaskJsonData.DataDict.ContainsKey(TaskId.Value) && PlayerEx.Player.taskMag._TaskData["Task"].HasField(TaskId.Value.ToString()))
			{
				PlayerEx.Player.TaskZhuiZhong.SetField("CurTaskID", TaskId.Value);
				PlayerEx.Player.TaskZhuiZhong.SetField("curType", TaskJsonData.DataDict[TaskId.Value].Type);
				PlayerEx.Player.TaskZhuiZhong.SetField("CurisChuanWen", val: false);
				PlayerEx.Player.TaskZhuiZhong.SetField("curTask", PlayerEx.Player.taskMag._TaskData["Task"][TaskId.Value.ToString()]);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)ex);
		}
		Continue();
	}
}
