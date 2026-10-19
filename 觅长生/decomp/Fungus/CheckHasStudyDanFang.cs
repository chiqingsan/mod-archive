using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSTools", "检查是否学习过此丹方", "检查是否学习过此丹方", 0)]
[AddComponentMenu("")]
public class CheckHasStudyDanFang : Command
{
	[Tooltip("DanYaoId")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable DanYaoId;

	[Tooltip("Result")]
	[VariableProperty(new Type[] { typeof(BooleanVariable) })]
	[SerializeField]
	protected BooleanVariable Result;

	public override void OnEnter()
	{
		Result.Value = false;
		foreach (JSONObject item in Tools.instance.getPlayer().DanFang.list)
		{
			if (item["ID"].I == DanYaoId.Value)
			{
				Result.Value = true;
				break;
			}
		}
		Continue();
	}
}
