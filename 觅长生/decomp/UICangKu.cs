using KBEngine;
using UnityEngine;
using UnityEngine.UI;
using YSGame;

public class UICangKu : MonoBehaviour, IESCClose
{
	public static UICangKu Inst;

	public CangKuBag PlayerBag;

	public CangKuBag CangKuBag;

	public Text PlayerMoneyText;

	public Text PlayerNameText;

	public Text PlayerTitleText;

	public PlayerSetRandomFace PlayerFace;

	[HideInInspector]
	public CangKuData CangKu;

	private void Awake()
	{
		Inst = this;
		ESCCloseManager.Inst.RegisterClose(this);
	}

	private void Start()
	{
		MusicMag.instance.PlayEffectMusic("open_tradepage");
	}

	public void Init(CangKuData cangKu)
	{
		CangKu = cangKu;
		CangKuBag.CangKu = cangKu;
		PlayerBag.Init(0, isPlayer: true);
		CangKuBag.Init(0);
		Avatar player = PlayerEx.Player;
		PlayerNameText.SetText(Tools.GetPlayerName());
		PlayerTitleText.SetText(Tools.GetPlayerTitle());
		PlayerMoneyText.SetText(player.money);
		((Component)PlayerFace).gameObject.SetActive(true);
		PlayerFace.setFace();
	}

	public void RefreshUI()
	{
		PlayerBag.CreateTempList();
		PlayerBag.UpdateItem();
		CangKuBag.CreateTempList();
		CangKuBag.UpdateItem();
	}

	public bool TryEscClose()
	{
		Close();
		return true;
	}

	public void Close()
	{
		Inst = null;
		ESCCloseManager.Inst.UnRegisterClose(this);
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void ItemToCangKu(CangKuSlot slot)
	{
		int count = 1;
		bool flag = true;
		if (slot.Item.Count == 1)
		{
			flag = false;
		}
		if (Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303))
		{
			count = 5;
			if (slot.Item.Count < 5)
			{
				count = slot.Item.Count;
			}
			flag = false;
		}
		if (Input.GetKey((KeyCode)306) || Input.GetKey((KeyCode)305))
		{
			count = slot.Item.Count;
			flag = false;
		}
		if (flag)
		{
			USelectNum.Show("选择存放数量 x{num}", 1, slot.Item.Count, delegate(int num)
			{
				MusicMag.instance.PlayEffectMusic("open_tradebag");
				Tools.instance.RemoveItem(slot.Item.Uid, num);
				CangKu.AddItem(slot.Item.Id, num, slot.Item.Seid.Copy(), slot.Item.Uid);
				RefreshUI();
			});
		}
		else
		{
			MusicMag.instance.PlayEffectMusic("open_tradebag");
			Tools.instance.RemoveItem(slot.Item.Uid, count);
			CangKu.AddItem(slot.Item.Id, count, slot.Item.Seid.Copy(), slot.Item.Uid);
			RefreshUI();
		}
	}

	public void ItemToPlayer(CangKuSlot slot)
	{
		int count = 1;
		bool flag = true;
		if (slot.Item.Count == 1)
		{
			flag = false;
		}
		if (Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303))
		{
			count = 5;
			if (slot.Item.Count < 5)
			{
				count = slot.Item.Count;
			}
			flag = false;
		}
		if (Input.GetKey((KeyCode)306) || Input.GetKey((KeyCode)305))
		{
			count = slot.Item.Count;
			flag = false;
		}
		if (flag)
		{
			USelectNum.Show("选择取出数量 x{num}", 1, slot.Item.Count, delegate(int num)
			{
				MusicMag.instance.PlayEffectMusic("open_tradebag");
				CangKu.RemoveItem(slot.Item.Uid, num);
				Tools.instance.NewAddItem(slot.Item.Id, num, slot.Item.Seid.Copy(), slot.Item.Uid);
				RefreshUI();
			});
		}
		else
		{
			MusicMag.instance.PlayEffectMusic("open_tradebag");
			CangKu.RemoveItem(slot.Item.Uid, count);
			Tools.instance.NewAddItem(slot.Item.Id, count, slot.Item.Seid.Copy(), slot.Item.Uid);
			RefreshUI();
		}
	}

	public void SortCangKu()
	{
		CangKu.ItemList.SortItems();
		RefreshUI();
	}
}
