using System.Collections.Generic;
using JSONClass;
using UnityEngine;

namespace Fungus;

[CommandInfo("风雨楼", "打开代号设置界面", "打开代号设置界面", 0)]
[AddComponentMenu("")]
public class OpenSetDaiHao : Command, INoCommand
{
	public override void OnEnter()
	{
		List<string> list = new List<string>();
		foreach (KillerDaiHaoData data in KillerDaiHaoData.DataList)
		{
			list.Add(data.Name);
		}
		UInputBox.Show("", delegate(string arg0)
		{
			PlayerEx.SetDaiHao(arg0);
			Continue();
		}, list, "shashou");
	}
}
