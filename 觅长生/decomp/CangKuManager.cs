using UnityEngine;

public class CangKuManager
{
	private static CangKuManager instance;

	public CangKuSave CangKuSave;

	public static CangKuManager Inst
	{
		get
		{
			if (instance == null)
			{
				instance = new CangKuManager();
			}
			return instance;
		}
	}

	public CangKuData CreateCangKu(string id, string name, bool overrideCangKu = false)
	{
		for (int num = CangKuSave.CangKuList.Count - 1; num >= 0; num--)
		{
			CangKuData cangKuData = CangKuSave.CangKuList[num];
			if (cangKuData.CangKuID == id)
			{
				if (overrideCangKu)
				{
					CangKuSave.CangKuList.RemoveAt(num);
					break;
				}
				return cangKuData;
			}
		}
		CangKuData cangKuData2 = new CangKuData();
		cangKuData2.CangKuID = id;
		cangKuData2.Name = name;
		CangKuSave.CangKuList.Add(cangKuData2);
		return cangKuData2;
	}

	public CangKuData GetCangKu(string id)
	{
		foreach (CangKuData cangKu in CangKuSave.CangKuList)
		{
			if (cangKu.CangKuID == id)
			{
				return cangKu;
			}
		}
		return null;
	}

	public void OpenCangKuUI(string id)
	{
		CangKuData cangKu = GetCangKu(id);
		if (cangKu == null)
		{
			UIPopTip.Inst.Pop("没有仓库数据:" + id);
		}
		else
		{
			Object.Instantiate<GameObject>(Resources.Load<GameObject>("UICangKu"), ((Component)NewUICanvas.Inst.Canvas).transform).GetComponent<UICangKu>().Init(cangKu);
		}
	}
}
