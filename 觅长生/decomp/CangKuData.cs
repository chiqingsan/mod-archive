using JSONClass;
using KBEngine;
using UnityEngine;

public class CangKuData
{
	public string CangKuID = "";

	public string Name = "";

	public string Style = "default";

	public string Description = "";

	public ITEM_INFO_LIST ItemList = new ITEM_INFO_LIST();

	public JSONObject ToJSONObject()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.SetField("CangKuID", CangKuID);
		jSONObject.SetField("Name", Name);
		jSONObject.SetField("Style", Style);
		jSONObject.SetField("Description", Description);
		jSONObject.SetField("ItemList", ItemList.ToJSONObject());
		return jSONObject;
	}

	public static CangKuData FromJSONObject(JSONObject json)
	{
		return new CangKuData
		{
			CangKuID = json["CangKuID"].Str,
			Name = json["Name"].Str,
			Style = json["Style"].Str,
			Description = json["Description"].Str,
			ItemList = ITEM_INFO_LIST.FromJSONObject(json["ItemList"])
		};
	}

	public void AddItem(int id, int count, JSONObject seid, string uuid = "无")
	{
		int type = _ItemJsonData.DataDict[id].type;
		if (type <= 2 || type == 14)
		{
			if (uuid == "无")
			{
				uuid = Tools.getUUID();
			}
			if (seid == null)
			{
				seid = Tools.CreateItemSeid(id);
			}
		}
		if (!_ItemJsonData.DataDict.ContainsKey(id))
		{
			Debug.LogError((object)$"添加物品出现异常，不存在ID为{id}的物品");
			return;
		}
		_ItemJsonData itemJsonData = _ItemJsonData.DataDict[id];
		if (seid == null)
		{
			seid = new JSONObject(JSONObject.Type.OBJECT);
		}
		if (itemJsonData.maxNum == 1 || (seid != null && seid.Count > 0))
		{
			for (int i = 0; i < count; i++)
			{
				ITEM_INFO iTEM_INFO = new ITEM_INFO();
				iTEM_INFO.uuid = Tools.getUUID();
				iTEM_INFO.itemId = id;
				iTEM_INFO.itemCount = 1u;
				iTEM_INFO.Seid = seid;
				ItemList.values.Add(iTEM_INFO);
			}
		}
		else if (GetItemNum(id) == 0)
		{
			ITEM_INFO iTEM_INFO2 = new ITEM_INFO();
			iTEM_INFO2.uuid = Tools.getUUID();
			iTEM_INFO2.itemId = id;
			iTEM_INFO2.itemCount = (uint)count;
			ItemList.values.Add(iTEM_INFO2);
		}
		else
		{
			GetItemInfo(id).itemCount += (uint)count;
		}
	}

	public void RemoveItem(string uid, int count)
	{
		for (int num = count; num > 0; num--)
		{
			ITEM_INFO iTEM_INFO = FindItemByUUID(uid);
			if (iTEM_INFO == null)
			{
				break;
			}
			iTEM_INFO.itemCount--;
			if (iTEM_INFO.itemCount == 0)
			{
				ItemList.values.Remove(iTEM_INFO);
			}
		}
	}

	public ITEM_INFO FindItemByUUID(string itemUUId)
	{
		return ItemList.values.Find((ITEM_INFO aa) => aa.uuid == itemUUId);
	}

	public int GetItemNum(int itemID)
	{
		int num = 0;
		int maxNum = _ItemJsonData.DataDict[itemID].maxNum;
		foreach (ITEM_INFO value in ItemList.values)
		{
			if (value.itemId == itemID && value.itemCount <= maxNum)
			{
				num += (int)value.itemCount;
			}
		}
		return num;
	}

	public ITEM_INFO GetItemInfo(int itemID)
	{
		foreach (ITEM_INFO value in ItemList.values)
		{
			if (value.itemId == itemID)
			{
				return value;
			}
		}
		return null;
	}
}
