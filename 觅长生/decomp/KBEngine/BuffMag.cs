using System.Collections.Generic;
using JSONClass;
using UnityEngine;

namespace KBEngine;

public class BuffMag
{
	public Avatar entity;

	public BuffMag(Entity avater)
	{
		entity = (Avatar)avater;
	}

	private void _PlayBuffEffect(string name)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Object val = ResManager.inst.LoadBuffEffect(name);
		GameObject val2 = (GameObject)entity.renderObj;
		if ((Object)(object)val2.transform.Find("Buff_" + name) == (Object)null && val != (Object)null)
		{
			GameObject val3 = (GameObject)Object.Instantiate(val);
			val3.transform.parent = val2.transform;
			val3.transform.localScale = Vector3.one;
			val3.transform.localPosition = Vector3.zero;
			val3.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
			((Object)val3.transform).name = "Buff_" + name;
		}
	}

	public void PlayBuffEffect(string name)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Object val = ResManager.inst.LoadBuffEffect(name);
		GameObject val2 = (GameObject)entity.renderObj;
		if ((Object)(object)val2.transform.Find("Buff_" + name) == (Object)null && val != (Object)null)
		{
			GameObject val3 = (GameObject)Object.Instantiate(val);
			val3.transform.parent = val2.transform;
			val3.transform.localScale = Vector3.one;
			val3.transform.localPosition = Vector3.zero;
			val3.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
			((Object)val3.transform).name = "Buff_" + name;
		}
	}

	private GameObject PlayTianJieBuffEffect(string name)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		string text = "tianjie" + name;
		string text2 = "Buff_" + text;
		Object val = ResManager.inst.LoadBuffEffect(text);
		GameObject val2 = GameObject.Find(text2);
		if ((Object)(object)val2 == (Object)null)
		{
			if (val != (Object)null)
			{
				GameObject val3 = (GameObject)Object.Instantiate(val);
				if ((Object)(object)TianJieEffectManager.Inst != (Object)null)
				{
					val3.transform.parent = ((Component)TianJieEffectManager.Inst.PlayerTransform).transform;
				}
				if ((Object)(object)DuanTiFightManager.Inst != (Object)null)
				{
					val3.transform.parent = ((Component)DuanTiFightManager.Inst.PlayerTransform).transform;
				}
				val3.transform.localScale = Vector3.one;
				val3.transform.localPosition = Vector3.zero;
				val3.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
				((Object)val3.transform).name = text2;
				Debug.Log((object)("实例化了天劫buff特效 " + val3.GetPath()));
				return val3;
			}
			return null;
		}
		return val2;
	}

	private void _playEffect(string BuffName, string type)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		if (int.TryParse(BuffName, out var _))
		{
			Debug.Log((object)"");
		}
		GameObject val = (GameObject)entity.renderObj;
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		if ((Object)(object)TianJieEffectManager.Inst != (Object)null || (Object)(object)DuanTiFightManager.Inst != (Object)null)
		{
			GameObject val2 = PlayTianJieBuffEffect(BuffName);
			if ((Object)(object)val2 != (Object)null)
			{
				Animator componentInChildren = ((Component)val2.transform).GetComponentInChildren<Animator>();
				if ((Object)(object)componentInChildren == (Object)null)
				{
					Debug.Log((object)"粒子特效,没有animator");
				}
				else
				{
					componentInChildren.Play(type);
				}
				return;
			}
		}
		_PlayBuffEffect(BuffName);
		Transform val3 = val.transform.Find("Buff_" + BuffName);
		if ((Object)(object)val3 != (Object)null)
		{
			if ((Object)(object)((Component)((Component)val3).transform).GetComponentInChildren<Animator>() == (Object)null)
			{
				Debug.Log((object)"粒子特效,没有animator");
			}
			else
			{
				((Component)((Component)val3).transform).GetComponentInChildren<Animator>().Play(type);
			}
		}
	}

	public void PlayBuffAdd(string Buff)
	{
		if (!((Object)(object)RoundManager.instance != (Object)null) || !RoundManager.instance.IsVirtual)
		{
			_playEffect(Buff, "Start");
		}
	}

	public void PlayBuffTarget(string Buff)
	{
		if (!((Object)(object)RoundManager.instance != (Object)null) || !RoundManager.instance.IsVirtual)
		{
			_playEffect(Buff, "Target");
		}
	}

	public void PlayBuffRemove(string Buff)
	{
		if (!((Object)(object)RoundManager.instance != (Object)null) || !RoundManager.instance.IsVirtual)
		{
			_playEffect(Buff, "Remove");
		}
	}

	public bool HasBuff(int buffID)
	{
		foreach (List<int> item in entity.bufflist)
		{
			if (buffID == item[2])
			{
				return true;
			}
		}
		return false;
	}

	public bool checkHasBuff(int buffID, Avatar avatar)
	{
		for (int i = 0; i < avatar.bufflist.Count; i++)
		{
			if (buffID == avatar.bufflist[i][2])
			{
				return true;
			}
		}
		return false;
	}

	public bool HasBuffNumGraterY(int buffID, int num)
	{
		if (GetBuffHasYTime(buffID, num) > 0)
		{
			return true;
		}
		return false;
	}

	public int GetBuffHasYTime(int buffID, int Y)
	{
		if (HasBuff(buffID))
		{
			int buffSum = GetBuffSum(buffID);
			if (buffSum >= Y)
			{
				return buffSum / Y;
			}
			return 0;
		}
		return 0;
	}

	public List<List<int>> getAllBuffByType(int type)
	{
		List<int> buffList = new List<int>();
		return entity.bufflist.FindAll(delegate(List<int> aa)
		{
			int item = aa[2];
			int i = jsonData.instance.BuffJsonData[item.ToString()]["bufftype"].I;
			if (!buffList.Contains(item))
			{
				buffList.Add(item);
				if (i == type)
				{
					return true;
				}
			}
			return false;
		});
	}

	public List<List<int>> GetAllBuffByType(int type)
	{
		return entity.bufflist.FindAll(delegate(List<int> aa)
		{
			int num = aa[2];
			return (jsonData.instance.BuffJsonData[num.ToString()]["bufftype"].I == type) ? true : false;
		});
	}

	public int getBuffTypeNum(int type)
	{
		return getAllBuffByType(type).Count;
	}

	public int GetBuffSum(int buffID)
	{
		List<List<int>> buffByID = getBuffByID(buffID);
		int sumRound = 0;
		buffByID.ForEach(delegate(List<int> _aa)
		{
			sumRound += _aa[1];
		});
		return sumRound;
	}

	public bool HasXTypeBuff(int BuffType)
	{
		foreach (List<int> item in entity.bufflist)
		{
			if (BuffType == (int)jsonData.instance.BuffJsonData[item[2].ToString()]["bufftype"].n)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasBuffSeid(int buffSeid)
	{
		foreach (List<int> item in entity.bufflist)
		{
			int num = item[2];
			foreach (JSONObject item2 in jsonData.instance.BuffJsonData[string.Concat(num)]["seid"].list)
			{
				if (buffSeid == item2.I)
				{
					return true;
				}
			}
		}
		return false;
	}

	public List<List<int>> getBuffByID(int BuffID)
	{
		List<List<int>> list = new List<List<int>>();
		foreach (List<int> item in entity.bufflist)
		{
			if (item[2] == BuffID)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public List<int> GetBuffById(int buffId)
	{
		foreach (List<int> item in entity.bufflist)
		{
			if (item[2] == buffId)
			{
				return item;
			}
		}
		return null;
	}

	public int GetBuffRoundByID(int BuffID)
	{
		List<List<int>> buffByID = getBuffByID(BuffID);
		int num = 0;
		foreach (List<int> item in buffByID)
		{
			num += item[1];
		}
		return num;
	}

	public List<List<int>> getBuffBySeid(int BuffSeid)
	{
		List<List<int>> list = new List<List<int>>();
		foreach (List<int> item in entity.bufflist)
		{
			int key = item[2];
			foreach (int item2 in _BuffJsonData.DataDict[key].seid)
			{
				if (BuffSeid == item2)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public int getBuffIndex(List<int> buffinfo)
	{
		int num = 0;
		foreach (List<int> item in entity.bufflist)
		{
			if (item == buffinfo)
			{
				return num;
			}
			num++;
		}
		return num;
	}

	public int getHuDun()
	{
		List<List<int>> buffByID = getBuffByID(5);
		if (buffByID.Count > 0)
		{
			return buffByID[0][2];
		}
		return 0;
	}

	public int RemoveBuff(int BuffID)
	{
		List<List<int>> buffByID = getBuffByID(BuffID);
		int num = 0;
		foreach (List<int> item in buffByID)
		{
			num += item[1];
			entity.spell.removeBuff(item);
		}
		return num;
	}

	public static int RemoveBuff(Avatar target, int BuffID)
	{
		return target.buffmag.RemoveBuff(BuffID);
	}
}
