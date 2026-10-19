using System.Collections.Generic;
using JSONClass;
using KBEngine;
using UnityEngine;
using UnityEngine.Events;

public class DongFuScene : MonoBehaviour
{
	public static DongFuScene Inst;

	public static bool CanShowDongKu;

	public bool ShowDongKu;

	public ModSpriteRenderer LingYanShow;

	public List<string> LingYanShowSprites;

	public ModSpriteRenderer JuLingZhenShow;

	public List<string> JuLingZhenShowSprites;

	public GameObject LianQiBuild;

	public GameObject LianQiBreak;

	public GameObject LianDanBuild;

	public GameObject LianDanBreak;

	public GameObject CangKuBuild;

	public GameObject CangKuBreak;

	public GameObject LianGongBuild;

	public GameObject LianGongBreak;

	public GameObject LingTianBuild;

	public GameObject LingTianBreak;

	public ModSpriteRenderer LingTianShow;

	public List<string> LingTianShowSprites;

	public GameObject DongKu;

	public DFInteractiveMode InteractiveMode;

	private DongFuData DongFu;

	private void Awake()
	{
		Inst = this;
		ShowDongKu = CanShowDongKu;
	}

	private void Start()
	{
		RefreshShow();
	}

	private void Update()
	{
		if ((Object)(object)UILingTianPanel.Inst != (Object)null && UILingTianPanel.Inst.IsShouGe && Input.GetMouseButtonDown(1))
		{
			UILingTianPanel.Inst.IsShouGe = false;
		}
	}

	public void RefreshShow()
	{
		DongFu = new DongFuData(DongFuManager.NowDongFuID);
		DongFu.Load();
		LingYanShow.SpritePath = LingYanShowSprites[DongFu.LingYanLevel - 1];
		LingYanShow.Refresh();
		for (int i = 1; i <= 3; i++)
		{
			((Component)((Component)LingYanShow).transform.GetChild(i - 1)).gameObject.SetActive(i == DongFu.LingYanLevel);
		}
		JuLingZhenShow.SpritePath = JuLingZhenShowSprites[DongFu.JuLingZhenLevel - 1];
		JuLingZhenShow.Refresh();
		if (DongFu.AreaUnlock[0] == 0)
		{
			LianQiBreak.SetActive(true);
			LianQiBuild.SetActive(false);
		}
		else
		{
			LianQiBreak.SetActive(false);
			LianQiBuild.SetActive(true);
		}
		if (DongFu.AreaUnlock[1] == 0)
		{
			LianDanBreak.SetActive(true);
			LianDanBuild.SetActive(false);
		}
		else
		{
			LianDanBreak.SetActive(false);
			LianDanBuild.SetActive(true);
		}
		if (DongFu.AreaUnlock[2] == 0)
		{
			LingTianBreak.SetActive(true);
			LingTianBuild.SetActive(false);
		}
		else
		{
			LingTianBreak.SetActive(false);
			LingTianBuild.SetActive(true);
			LingTianType lingTianType = LingTianType.未种植;
			for (int j = 0; j < DongFuManager.LingTianCount; j++)
			{
				int iD = DongFu.LingTian[j].ID;
				int lingLi = DongFu.LingTian[j].LingLi;
				if (iD <= 0)
				{
					continue;
				}
				_ItemJsonData itemJsonData = _ItemJsonData.DataDict[iD];
				int num = lingLi / itemJsonData.price;
				switch (lingTianType)
				{
				case LingTianType.未种植:
					if (num > 0)
					{
						lingTianType = LingTianType.有成熟;
						break;
					}
					lingTianType = LingTianType.生长中;
					continue;
				case LingTianType.生长中:
					if (num <= 0)
					{
						continue;
					}
					lingTianType = LingTianType.有成熟;
					break;
				default:
					continue;
				}
				break;
			}
			SetLingTianShow(lingTianType);
		}
		if (DongFu.AreaUnlock[3] == 0)
		{
			CangKuBreak.SetActive(true);
			CangKuBuild.SetActive(false);
		}
		else
		{
			CangKuBreak.SetActive(false);
			CangKuBuild.SetActive(true);
		}
		if (DongFu.AreaUnlock[4] == 0)
		{
			LianGongBreak.SetActive(true);
			LianGongBuild.SetActive(false);
		}
		else
		{
			LianGongBreak.SetActive(false);
			LianGongBuild.SetActive(true);
		}
		DongKu.SetActive(ShowDongKu);
	}

	public void SetLingTianShow(LingTianType type)
	{
		LingTianShow.SpritePath = LingTianShowSprites[(int)type];
		LingTianShow.Refresh();
	}

	private void BuyArea(int id, string areaName, int lingShi = 10000)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		USelectBox.Show($"是否花费{lingShi}灵石布置{areaName}?", (UnityAction)delegate
		{
			Avatar player = PlayerEx.Player;
			if (player.money >= (ulong)lingShi)
			{
				player.AddMoney(-lingShi);
				DongFu.Load();
				DongFu.AreaUnlock[id] = 1;
				DongFu.Save();
				RefreshShow();
			}
			else
			{
				UIPopTip.Inst.Pop("灵石不足");
			}
		});
	}

	public void OnLingYanFuncClick()
	{
		UIDongFu.Inst.ShowJuLingZhenPanel();
	}

	public void OnBiGuanFuncClick()
	{
		UIBiGuanPanel.Inst.OpenBiGuan(1);
	}

	public void OnLianQiBreakFuncClick()
	{
		BuyArea(0, "炼器室");
	}

	public void OnLianQiBuildFuncClick()
	{
		PanelMamager.inst.OpenPanel(PanelMamager.PanelType.炼器);
	}

	public void OnLianDanBreakFuncClick()
	{
		BuyArea(1, "炼丹室");
	}

	public void OnLianDanBuildFuncClick()
	{
		PanelMamager.inst.OpenPanel(PanelMamager.PanelType.炼丹);
	}

	public void OnLingTianBreakFuncClick()
	{
		BuyArea(2, "灵田");
	}

	public void OnLingTianBuildFuncClick()
	{
		UIDongFu.Inst.ShowLingTianPanel();
	}

	public void OnCangKuBreakFuncClick()
	{
		BuyArea(3, "仓库");
	}

	public void OnCangKuBuildFuncClick()
	{
		string id = $"DongFu_{DongFu.ID}_CangKu_Main";
		if (CangKuManager.Inst.GetCangKu(id) == null)
		{
			CangKuManager.Inst.CreateCangKu(id, "仓库");
		}
		CangKuManager.Inst.OpenCangKuUI(id);
	}

	public void OnLianGongBreakFuncClick()
	{
		BuyArea(4, "假人", 1000);
	}

	public void OnLianGongBuildFuncClick()
	{
		NPCEx.OpenTalk(4013);
	}

	public void OnDongKuFuncClick()
	{
	}

	public void OnLingYanDecorateClick()
	{
		UIPopTip.Inst.Pop("Todo");
	}

	public void OnBiGuanDecorateClick()
	{
		UIPopTip.Inst.Pop("Todo");
	}

	public void OnLianQiBreakDecorateClick()
	{
		BuyArea(0, "炼器室");
	}

	public void OnLianQiBuildDecorateClick()
	{
		UIPopTip.Inst.Pop("Todo");
	}

	public void OnLianDanBreakDecorateClick()
	{
		BuyArea(1, "炼丹室");
	}

	public void OnLianDanBuildDecorateClick()
	{
		UIPopTip.Inst.Pop("Todo");
	}

	public void OnLingTianBreakDecorateClick()
	{
		BuyArea(2, "灵田");
	}

	public void OnLingTianBuildDecorateClick()
	{
		UIPopTip.Inst.Pop("Todo");
	}
}
