using System;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;

public class SiXuManager : MonoBehaviour, IESCClose
{
	public class LingGanBase : IComparable<LingGanBase>
	{
		public string Name;

		public int PingJie;

		public int OutTime;

		public int CompareTo(LingGanBase other)
		{
			if (PingJie > other.PingJie)
			{
				return -1;
			}
			if (PingJie == other.PingJie && OutTime <= other.OutTime)
			{
				return -1;
			}
			return 1;
		}
	}

	[SerializeField]
	private GameObject SiXuCell;

	public void open()
	{
		ESCCloseManager.Inst.RegisterClose(this);
		((Component)this).gameObject.SetActive(true);
		init();
	}

	public void close()
	{
		((Component)this).gameObject.SetActive(false);
		ESCCloseManager.Inst.UnRegisterClose(this);
	}

	private void init()
	{
		clear();
		Avatar player = Tools.instance.getPlayer();
		initLingGuan(player);
	}

	private void clear()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		foreach (Transform item in SiXuCell.transform.parent)
		{
			Transform val = item;
			if (((Component)val).gameObject.activeSelf)
			{
				Object.Destroy((Object)(object)((Component)val).gameObject);
			}
		}
	}

	private void initLingGuan(Avatar player)
	{
		List<JSONObject> list = player.LingGuang.list;
		List<LingGanBase> list2 = new List<LingGanBase>();
		for (int i = 0; i < list.Count; i++)
		{
			int days = (DateTime.Parse(list[i]["startTime"].str).AddDays(list[i]["guoqiTime"].I) - player.worldTimeMag.getNowTime()).Days;
			list2.Add(new LingGanBase
			{
				Name = "<color=#cfcea8>" + Tools.Code64(list[i]["name"].str) + "</color>",
				PingJie = list[i]["quality"].I,
				OutTime = days
			});
		}
		list2.Sort();
		foreach (LingGanBase item in list2)
		{
			SiXuTypeCell component = Tools.InstantiateGameObject(SiXuCell, SiXuCell.transform.parent).GetComponent<SiXuTypeCell>();
			if (item.OutTime <= 30)
			{
				component.SetContent(item.Name, item.PingJie, $"<color=#ff6554>{item.OutTime}日后</color>");
			}
			else if (item.OutTime <= 90)
			{
				component.SetContent(item.Name, item.PingJie, $"<color=#f2a16b>{item.OutTime}日后</color>");
			}
			else
			{
				component.SetContent(item.Name, item.PingJie, $"<color=#cfcea8>{item.OutTime}日后</color>");
			}
		}
	}

	public bool TryEscClose()
	{
		close();
		return true;
	}
}
