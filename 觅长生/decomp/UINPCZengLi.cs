using UnityEngine;
using UnityEngine.UI;
using YSGame;

public class UINPCZengLi : MonoBehaviour, IESCClose
{
	private UINPCData npc;

	public UIInventory PlayerInventory;

	public UIIconShow ZengLiSlot;

	public UINPCXingQu NPCXingQu;

	public Text QingFenText;

	private void OnEnable()
	{
		MusicMag.instance.PlayEffectMusic("open_tradepage");
	}

	private void Start()
	{
	}

	public void RefreshUI()
	{
		npc = UINPCJiaoHu.Inst.NowJiaoHuNPC;
		npc.RefreshData();
		NPCXingQu.npc = npc;
		NPCXingQu.RefreshUI();
		PlayerInventory.NPCID = npc.ID;
		PlayerInventory.RefreshUI();
		QingFenText.text = npc.QingFen.ToString();
		ZengLiSlot.NPCID = npc.ID;
		ZengLiSlot.SetNull();
	}

	public void OnOKBtnClick()
	{
		if (ZengLiSlot.NowType != 0)
		{
			NPCEx.ZengLiToNPC(npc, ZengLiSlot.tmpItem, ZengLiSlot.Count);
			RefreshUI();
			NpcJieSuanManager.inst.isUpDateNpcList = true;
		}
	}

	public void OnReturnBtnClick()
	{
		UINPCJiaoHu.Inst.HideNPCZengLi();
		UINPCJiaoHu.Inst.ShowJiaoHuPop();
	}

	public bool TryEscClose()
	{
		OnReturnBtnClick();
		return true;
	}
}
