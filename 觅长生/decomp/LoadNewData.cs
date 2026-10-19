using System.Collections.Generic;
using UnityEngine;
using script.newUI.MainUI;

public class LoadNewData : MonoBehaviour
{
	public NewGongGaoInput New;

	public NewGongGaoInput Old;

	public void OldToNew()
	{
		New.群列表 = new List<QQData>();
		foreach (QQData item in Old.群列表)
		{
			New.群列表.Add(new QQData
			{
				前半段群描述 = item.前半段群描述,
				QQ群号 = item.QQ群号,
				后半段描述 = item.后半段描述
			});
		}
		New.百科网址 = Old.百科网址;
		New.频道号 = Old.频道号;
	}
}
