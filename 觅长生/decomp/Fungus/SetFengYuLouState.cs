using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "开关玩家风雨楼模式", "开关玩家风雨楼模式", 0)]
[AddComponentMenu("")]
public class SetFengYuLouState : Command
{
	[Tooltip("开关")]
	[VariableProperty(new Type[] { typeof(BooleanVariable) })]
	[SerializeField]
	protected BooleanVariable IsOpen;

	public override void OnEnter()
	{
		if (IsOpen.Value)
		{
			jsonData.instance.AvatarRandomJsonData["1"].SetField("a_mask", PlayerEx.GetShiLiChengHaoLevel(10));
		}
		else
		{
			jsonData.instance.AvatarRandomJsonData["1"].SetField("a_mask", 0);
		}
		if ((Object)(object)UIHeadPanel.Inst != (Object)null)
		{
			UIHeadPanel.Inst.Face.setFace();
		}
		Continue();
	}
}
