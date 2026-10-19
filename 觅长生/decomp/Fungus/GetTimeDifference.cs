using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSTools", "获取当前时间到指定时间的时间差", "获取当前时间到指定时间的时间差（精确到月）\n其中指定时间需要分别赋值TargetYear/TargetMonth，可以指定未来时间和过去的时间。\n返回有两种方式:\n1. 不勾选IsReturnSeperateTimeDiff，以年+月的形式返回时间差，如1年又5月;\n2. 勾选IsReturnSeperateTimeDiff，分别返回年数差和月数差，年与月没有关联，如1年8月距离2年5月，分别返回YearDiff=1年、MonthDiff=9月.\n分别返回到YearDiff/MonthDiff中。", 0)]
[AddComponentMenu("")]
public class GetTimeDifference : Command
{
	[Tooltip("是否分别返回年数和月数差")]
	[SerializeField]
	protected bool IsReturnSeperateTimeDiff;

	[Tooltip("目标时间的年份")]
	[SerializeField]
	protected IntegerData TargetYear;

	[Tooltip("目标时间的月份")]
	[SerializeField]
	protected IntegerData TargetMonth;

	[Tooltip("返回的年份差")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable YearDiff;

	[Tooltip("返回的月份差")]
	[VariableProperty(new Type[] { typeof(IntegerVariable) })]
	[SerializeField]
	protected IntegerVariable MonthDiff;

	private void CalculateTimeDiff()
	{
		DateTime nowTime = Tools.instance.getPlayer().worldTimeMag.getNowTime();
		int num = 12;
		MonthDiff.Value = Mathf.Abs(num * (nowTime.Year - TargetYear.Value) + (nowTime.Month - TargetMonth.Value));
		if (IsReturnSeperateTimeDiff)
		{
			YearDiff.Value = Mathf.Abs(nowTime.Year - TargetYear.Value);
			return;
		}
		YearDiff.Value = MonthDiff.Value / num;
		MonthDiff.Value %= num;
	}

	public override void OnEnter()
	{
		CalculateTimeDiff();
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
