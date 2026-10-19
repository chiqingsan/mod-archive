using System;
using KBEngine;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSNew/Get", "GetStaticValueByVar", "获取全局变量保存到TempValue中", 0)]
[AddComponentMenu("")]
public class GetStaticValueByVar : Command
{
	[Tooltip("全局变量的ID")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	public IntegerVariable StaticValueID;

	public override void OnEnter()
	{
		Avatar player = Tools.instance.getPlayer();
		Flowchart flowchart = GetFlowchart();
		switch (StaticValueID.Value)
		{
		case 1:
			flowchart.SetIntegerVariable("TempValue", player.HP);
			break;
		case 2:
			flowchart.SetIntegerVariable("TempValue", (int)player.exp);
			break;
		case 3:
			flowchart.SetIntegerVariable("TempValue", (int)player.shouYuan);
			break;
		default:
		{
			int value = GlobalValue.Get(StaticValueID.Value, GetCommandSourceDesc());
			flowchart.SetIntegerVariable("TempValue", value);
			break;
		}
		}
		Continue();
	}

	public override Color GetButtonColor()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return Color32.op_Implicit(new Color32((byte)184, (byte)210, (byte)235, byte.MaxValue));
	}
}
