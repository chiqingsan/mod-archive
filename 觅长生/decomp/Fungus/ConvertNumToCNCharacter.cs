using System;
using UnityEngine;

namespace Fungus;

[CommandInfo("YSTools", "阿拉伯数字汉字转化", "将阿拉伯数字转换成汉字\n直接输入数字或数字变量，输出String类型，勾选Is Add Redmark会自动加上红色标签", 0)]
[AddComponentMenu("")]
public class ConvertNumToCNCharacter : Command
{
	[Tooltip("待转换数字")]
	[SerializeField]
	protected IntegerData TargetNumber;

	[Tooltip("转换后汉字")]
	[VariableProperty(new Type[] { typeof(StringVariable) })]
	[SerializeField]
	protected StringVariable NumberCNCharacter;

	[Tooltip("是否加入红色标签")]
	[SerializeField]
	protected bool IsAddRedmark;

	public override void OnEnter()
	{
		ConvertNumb();
		Continue();
	}

	private void ConvertNumb()
	{
		NumberCNCharacter.Value = TargetNumber.Value.ToCNNumber();
		if (TargetNumber.Value == 2)
		{
			NumberCNCharacter.Value = "两";
		}
		if (IsAddRedmark)
		{
			NumberCNCharacter.Value = "{color=#d0380a}" + NumberCNCharacter.Value + "{/color}";
		}
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
