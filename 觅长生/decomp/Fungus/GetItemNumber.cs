using System;
using KBEngine;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSNew/Get", "GetItemNumber", "检测物品的数量", 0)]
[AddComponentMenu("")]
public class GetItemNumber : Command
{
	[Tooltip("需要检测物品ID")]
	[SerializeField]
	protected IntegerData ItemID;

	[Tooltip("物品数量")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable ItemNum;

	public void setHasVariable(string name, int num, Flowchart flowchart)
	{
		if (flowchart.HasVariable(name))
		{
			flowchart.SetIntegerVariable(name, num);
		}
	}

	public override void OnEnter()
	{
		Avatar player = Tools.instance.getPlayer();
		ItemNum.Value = player.getItemNum(ItemID.Value);
		Continue();
	}

	public override Color GetButtonColor()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return Color32.op_Implicit(new Color32((byte)184, (byte)210, (byte)235, byte.MaxValue));
	}

	public override void OnReset()
	{
	}
}
