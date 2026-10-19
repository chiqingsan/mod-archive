using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace script.newUI.MainUI;

public class QQDataUI : MonoBehaviour
{
	public TMP_Text 前半段;

	public TMP_Text QQ群;

	public TMP_Text 后半段;

	public FpBtn FpBtn;

	public void Init(string desc1, int qq, string desc2)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		前半段.SetText(desc1);
		QQ群.SetText(qq.ToString());
		后半段.SetText(desc2);
		FpBtn.mouseUpEvent.AddListener((UnityAction)delegate
		{
			GUIUtility.systemCopyBuffer = qq.ToString();
			UIPopTip.Inst.Pop("已复制QQ群号");
		});
		((Component)this).gameObject.SetActive(true);
	}
}
