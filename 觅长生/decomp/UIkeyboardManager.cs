using Fungus;
using UnityEngine;
using script.NewLianDan;

public class UIkeyboardManager : MonoBehaviour
{
	public static bool CanUse = true;

	private void Update()
	{
		if (!CanUse)
		{
			return;
		}
		if (Input.GetKeyUp((KeyCode)114) && Tools.instance.canClick())
		{
			taskUI.OpenTask();
		}
		if (Input.GetKeyUp((KeyCode)99) && Tools.instance.canClick())
		{
			PanelMamager.inst.OpenPanel(PanelMamager.PanelType.传音符);
		}
		if (Input.GetKeyUp((KeyCode)116) && Tools.instance.canClick())
		{
			PanelMamager.inst.OpenPanel(PanelMamager.PanelType.图鉴);
		}
		if (Input.GetKeyUp((KeyCode)106) && Tools.instance.canClick() && (Object)(object)UIHeadPanel.Inst != (Object)null)
		{
			UIHeadPanel.Inst.OnTieJianBtnClick();
		}
		if (!Input.GetKeyUp((KeyCode)113) || !Tools.instance.canClick())
		{
			return;
		}
		if ((Object)(object)LianQiTotalManager.inst != (Object)null)
		{
			LianQiTotalManager.inst.closeLianQiPanel();
			return;
		}
		if ((Object)(object)LianDanUIMag.Instance != (Object)null)
		{
			LianDanUIMag.Instance.Close();
			return;
		}
		GameObject val = GameObject.Find("likai");
		if ((Object)(object)val != (Object)null)
		{
			val.GetComponentInChildren<Flowchart>().ExecuteBlock("onClick");
		}
	}
}
