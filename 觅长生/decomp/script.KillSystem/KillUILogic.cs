using System;
using System.Collections.Generic;
using JSONClass;
using KillSystem;
using UnityEngine;
using UnityEngine.Events;
using VContainer;
using VContainer.Unity;

namespace script.KillSystem;

public class KillUILogic : IStartable, IESCClose
{
	[Inject]
	private KillView killView;

	[Inject]
	private RewardOrderView rewardOrderView;

	private List<RewardOrderView> list;

	public static void Open()
	{
		if (KillManager.Inst.RewardOrderService.GetOrderTaskState() == 0)
		{
			ResManager.inst.LoadPrefab("KillTaskPanel").Inst(((Component)NewUICanvas.Inst).transform);
		}
	}

	public void Start()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		if ((Object)(object)killView == (Object)null || (Object)(object)rewardOrderView == (Object)null)
		{
			Debug.LogError((object)($"自动注入异常，killView==null {(Object)(object)killView == (Object)null}," + $"rewardOrderView == null {(Object)(object)rewardOrderView == (Object)null}"));
			UIPopTip.Inst.Pop("悬赏榜异常，请反馈！");
		}
		else
		{
			ESCCloseManager.Inst.RegisterClose(this);
			killView.CloseBtn.mouseUpEvent.AddListener(new UnityAction(Close));
			CreateRewardOrderList();
		}
	}

	private void CreateRewardOrderList()
	{
		list = new List<RewardOrderView>();
		KillManager.Inst.RewardOrderService.UpdateRewardOrderList();
		Dictionary<int, RewardOrderModel> rewardOrderModels = KillManager.Inst.RewardOrderModels;
		if (rewardOrderModels.Count == 0)
		{
			return;
		}
		foreach (RewardOrderModel value in rewardOrderModels.Values)
		{
			GameObject val = null;
			try
			{
				val = ((Component)rewardOrderView).gameObject.Inst(killView.RewardOrderParent);
				RewardOrderView component = val.GetComponent<RewardOrderView>();
				InitRewardOrderView(component, value);
				val.SetActive(true);
				list.Add(component);
			}
			catch (Exception ex)
			{
				if ((Object)(object)val != (Object)null)
				{
					val.SetActive(false);
				}
				Debug.LogError((object)ex);
				Debug.LogError((object)"创建悬赏列表失败");
			}
		}
	}

	private void InitRewardOrderView(RewardOrderView view, RewardOrderModel order)
	{
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		view.Face.SetNPCFace(order.NpcId);
		int num = 0;
		if (order.NpcId == 1)
		{
			view.NpcName.SetText(Tools.GetPlayerName());
			view.ChengHao.SetText(Tools.GetPlayerTitle());
			num = PlayerEx.Player.level;
		}
		else
		{
			view.NpcName.SetText(jsonData.instance.AvatarRandomJsonData[order.NpcId.ToString()]["Name"].Str);
			view.ChengHao.SetText(jsonData.instance.AvatarJsonData[order.NpcId.ToString()]["Title"].Str);
			num = jsonData.instance.AvatarJsonData[order.NpcId.ToString()]["Level"].I;
		}
		view.Level.SetText(LevelUpDataJsonData.DataDict[num].Name);
		((Component)view.RewardOrderLevel[order.RewardOrderLevel - 1]).gameObject.SetActive(true);
		view.RewardMoney.SetText(order.RewardMoney);
		view.AcceptBtn.mouseUpEvent.AddListener((UnityAction)delegate
		{
			AccpetOrder(view.Model);
		});
		view.Model = order;
	}

	private void AccpetOrder(RewardOrderModel orderModel)
	{
		if (CanAccpet(orderModel))
		{
			KillManager.Inst.TempRewardOrder = orderModel;
			PlayerEx.Player.StaticValue.Value[1550] = orderModel.NpcId;
			PlayerEx.Player.StaticValue.Value[1551] = orderModel.RewardMoney / 2;
			PlayerEx.Player.StaticValue.Value[1552] = orderModel.RewardOrderLevel;
			Object.Instantiate<GameObject>(Resources.Load<GameObject>($"talkPrefab/TalkPrefab/talk{7501}"));
			Close();
		}
	}

	private bool CanAccpet(RewardOrderModel orderModel)
	{
		if (orderModel == null || orderModel.NpcId == -1 || !KillManager.Inst.RewardOrderModels.ContainsKey(orderModel.NpcId))
		{
			Debug.LogError((object)"无效的悬赏数据,接取失败");
			return false;
		}
		if (KillManager.Inst.RewardOrderService.HasAcceptOrder())
		{
			Debug.LogError((object)"已有悬赏任务，无法接取");
			return false;
		}
		return true;
	}

	public void Close()
	{
		ESCCloseManager.Inst.UnRegisterClose(this);
		Object.Destroy((Object)(object)((Component)killView).gameObject);
	}

	public bool TryEscClose()
	{
		Close();
		return true;
	}
}
