using TMPro;
using UnityEngine;

namespace script.newUI.MainUI;

public class NewGongGaoMag : MonoBehaviour, IESCClose
{
	public NewGongGaoInput Input;

	public TMP_Text 百科;

	public TMP_Text 频道;

	public Transform QQ群父对象;

	public GameObject QQ群预制体;

	public void Open()
	{
		((Component)this).gameObject.SetActive(true);
		ESCCloseManager.Inst.RegisterClose(this);
	}

	public void Close()
	{
		((Component)this).gameObject.SetActive(false);
		ESCCloseManager.Inst.UnRegisterClose(this);
	}

	private void Start()
	{
		百科.SetText(Input.百科网址);
		频道.SetText("频道号: " + Input.频道号);
		for (int num = Input.群列表.Count - 1; num >= 0; num--)
		{
			QQ群预制体.Inst(QQ群父对象).GetComponent<QQDataUI>().Init(Input.群列表[num].前半段群描述, Input.群列表[num].QQ群号, Input.群列表[num].后半段描述);
		}
	}

	public void OpenBaiKe()
	{
		Application.OpenURL(Input.百科网址);
	}

	public void CopyPingDao()
	{
		GUIUtility.systemCopyBuffer = Input.频道号;
		UIPopTip.Inst.Pop("已复制频道号");
	}

	public bool TryEscClose()
	{
		Close();
		return true;
	}
}
