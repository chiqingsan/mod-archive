using UnityEngine;
using UnityEngine.Events;
using YSGame;

public class NewMainUIManager : MonoBehaviour
{
	public FpBtn 神仙斗法;

	public FpBtn 创意工坊;

	public FpBtn 图鉴;

	public FpBtn 云存档;

	public FpBtn 公告;

	public FpBtn 设置;

	public FpBtn 退出;

	public void Awake()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Expected O, but got Unknown
		神仙斗法.mouseUpEvent.AddListener(new UnityAction(MainUIMag.inst.DoFaClick));
		创意工坊.mouseUpEvent.AddListener(new UnityAction(MainUIMag.inst.OpenWorkShop));
		图鉴.mouseUpEvent.AddListener(new UnityAction(MainUIMag.inst.OpenTuJian));
		云存档.mouseUpEvent.AddListener(new UnityAction(MainUIMag.inst.CloudSavePanel.OpenPanel));
		公告.mouseUpEvent.AddListener(new UnityAction(MainUIMag.inst.GongGaoMag.Open));
		设置.mouseUpEvent.AddListener(new UnityAction(MainUIMag.inst.GameSet));
		退出.mouseUpEvent.AddListener(new UnityAction(MainUIMag.inst.QuitGame));
		((Component)((Component)this).transform.Find("开始按钮")).GetComponent<FpBtn>().mouseUpEvent.AddListener(new UnityAction(MainUIMag.inst.StartGame));
		神仙斗法.mouseEnterEvent.AddListener(new UnityAction(PlayMusic));
		创意工坊.mouseEnterEvent.AddListener(new UnityAction(PlayMusic));
		图鉴.mouseEnterEvent.AddListener(new UnityAction(PlayMusic));
		云存档.mouseEnterEvent.AddListener(new UnityAction(PlayMusic));
		公告.mouseEnterEvent.AddListener(new UnityAction(PlayMusic));
		设置.mouseEnterEvent.AddListener(new UnityAction(PlayMusic));
		退出.mouseEnterEvent.AddListener(new UnityAction(PlayMusic));
	}

	public void PlayMusic()
	{
		MusicMag.instance.PlayEffectMusic("Slip - Wood");
	}
}
