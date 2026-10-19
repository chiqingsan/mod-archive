using System;
using System.Collections.Generic;
using GUIPackage;
using JSONClass;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using YSGame;

public class UIDuiHuanShop : MonoBehaviour, IESCClose
{
	public static UIDuiHuanShop Inst;

	public GameObject UIMenPaiShopItemPrefab;

	public GameObject ScaleObj;

	public RectTransform ShopRT;

	public Image MoneyIcon;

	public Text MoneyText;

	private int ShopID;

	private void Awake()
	{
		Inst = this;
	}

	public void Show(int shopID)
	{
		MusicMag.instance.PlayEffectMusic("open_tradepage");
		ShopID = shopID;
		ScaleObj.SetActive(true);
		ESCCloseManager.Inst.RegisterClose(this);
	}

	public void RefreshUI()
	{
		_ = SceneEx.NowSceneName;
		List<jiaoHuanShopGoods> shopGoods = GetShopGoods(ShopID);
		if (shopGoods.Count == 0)
		{
			Debug.LogError((object)$"UIDuiHuanShop刷新UI异常，兑换商店{ShopID}没有商品信息");
			return;
		}
		int num2 = 0;
		PlayerEx.Player.getLevelType();
		ItemDatebase component = ((Component)jsonData.instance).GetComponent<ItemDatebase>();
		((Transform)(object)ShopRT).DestoryAllChild();
		foreach (jiaoHuanShopGoods good in shopGoods)
		{
			if (num2 == 0)
			{
				num2 = good.EXGoodsID;
			}
			_ItemJsonData item = _ItemJsonData.DataDict[good.GoodsID];
			UIMenPaiShopItem component2 = Object.Instantiate<GameObject>(UIMenPaiShopItemPrefab, (Transform)(object)ShopRT).GetComponent<UIMenPaiShopItem>();
			int price = item.price;
			if (good.Money > 0)
			{
				price = good.Money;
			}
			component2.PriceText.text = price.ToString();
			component2.PriceIcon.sprite = component.items[good.EXGoodsID].itemIconSprite;
			component2.IconShow.SetItem(good.GoodsID);
			component2.IconShow.Count = 1;
			UIIconShow iconShow = component2.IconShow;
			iconShow.OnClick = (UnityAction<PointerEventData>)(object)Delegate.Combine((Delegate)(object)iconShow.OnClick, (Delegate)(object)(UnityAction<PointerEventData>)delegate
			{
				//IL_00db: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Expected O, but got Unknown
				int playerHas = 0;
				if (good.EXGoodsID == 10035)
				{
					playerHas = (int)PlayerEx.Player.money;
				}
				else
				{
					playerHas = PlayerEx.Player.getItemNum(good.EXGoodsID);
				}
				int num3 = playerHas / price;
				num3 = Mathf.Min(num3, item.maxNum);
				switch (num3)
				{
				case 0:
					UIPopTip.Inst.Pop(_ItemJsonData.DataDict[good.EXGoodsID].name + "不足");
					break;
				case 1:
					USelectBox.Show("是否兑换" + item.name + " x1", (UnityAction)delegate
					{
						if (playerHas >= price)
						{
							if (good.EXGoodsID == 10035)
							{
								PlayerEx.Player.AddMoney(-price);
							}
							else
							{
								PlayerEx.Player.removeItem(good.EXGoodsID, price);
							}
							PlayerEx.Player.addItem(good.GoodsID, 1, Tools.CreateItemSeid(good.GoodsID));
							RefreshUI();
							UIPopTip.Inst.Pop($"兑换了{_ItemJsonData.DataDict[good.GoodsID].name}x{1}", PopTipIconType.包裹);
						}
						else
						{
							UIPopTip.Inst.Pop(_ItemJsonData.DataDict[good.EXGoodsID].name + "不足");
						}
					});
					break;
				default:
					USelectNum.Show("兑换数量 x{num}", 1, num3, delegate(int num)
					{
						if (playerHas >= num * price)
						{
							if (good.EXGoodsID == 10035)
							{
								PlayerEx.Player.AddMoney(-(num * price));
							}
							else
							{
								PlayerEx.Player.removeItem(good.EXGoodsID, num * price);
							}
							PlayerEx.Player.addItem(good.GoodsID, num, Tools.CreateItemSeid(good.GoodsID));
							RefreshUI();
							UIPopTip.Inst.Pop($"兑换了{_ItemJsonData.DataDict[good.GoodsID].name}x{num}", PopTipIconType.包裹);
						}
						else
						{
							UIPopTip.Inst.Pop(_ItemJsonData.DataDict[good.EXGoodsID].name + "不足");
						}
					});
					break;
				}
			});
		}
		MoneyIcon.sprite = component.items[num2].itemIconSprite;
		if (num2 == 10035)
		{
			MoneyText.text = PlayerEx.Player.money.ToString();
		}
		else
		{
			MoneyText.text = PlayerEx.Player.getItemNum(num2).ToString();
		}
	}

	public static List<jiaoHuanShopGoods> GetShopGoods(int shopID)
	{
		List<jiaoHuanShopGoods> list = new List<jiaoHuanShopGoods>();
		foreach (jiaoHuanShopGoods data in jiaoHuanShopGoods.DataList)
		{
			if (data.ShopID != shopID)
			{
				continue;
			}
			if (data.EventValue != null && data.EventValue.Count == 2 && !string.IsNullOrWhiteSpace(data.fuhao))
			{
				bool flag = false;
				int num = GlobalValue.Get(data.EventValue[0], "兑换商品能否出现判定");
				int num2 = data.EventValue[1];
				switch (data.fuhao)
				{
				case "=":
					if (num == num2)
					{
						flag = true;
					}
					break;
				case ">":
					if (num > num2)
					{
						flag = true;
					}
					break;
				case "<":
					if (num < num2)
					{
						flag = true;
					}
					break;
				default:
					Debug.LogError((object)$"商品 id:{data.id} GoodsID:{data.GoodsID} 的符号 {data.fuhao} 不在预期内");
					break;
				}
				if (flag)
				{
					list.Add(data);
				}
				Debug.Log((object)$"商品 id:{data.id} GoodsID:{data.GoodsID} StaticValueID:{data.EventValue[0]} X:{num} Y:{num2} X{data.fuhao}Y:{flag}");
			}
			else
			{
				list.Add(data);
			}
		}
		return list;
	}

	public void Close()
	{
		ScaleObj.SetActive(false);
		ESCCloseManager.Inst.UnRegisterClose(this);
	}

	public bool TryEscClose()
	{
		Close();
		return true;
	}
}
