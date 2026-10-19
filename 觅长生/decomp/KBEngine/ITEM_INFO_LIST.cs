using System.Collections.Generic;
using JSONClass;

namespace KBEngine;

public class ITEM_INFO_LIST
{
	public List<ITEM_INFO> values = new List<ITEM_INFO>();

	public JSONObject ToJSONObject()
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.ARRAY);
		foreach (ITEM_INFO value in values)
		{
			JSONObject obj = value.ToJSONObject();
			jSONObject.Add(obj);
		}
		return jSONObject;
	}

	public static ITEM_INFO_LIST FromJSONObject(JSONObject json)
	{
		ITEM_INFO_LIST iTEM_INFO_LIST = new ITEM_INFO_LIST();
		foreach (JSONObject item2 in json.list)
		{
			ITEM_INFO item = ITEM_INFO.FromJSONObject(item2);
			iTEM_INFO_LIST.values.Add(item);
		}
		return iTEM_INFO_LIST;
	}

	public void SortItems()
	{
		values.Sort(delegate(ITEM_INFO a, ITEM_INFO b)
		{
			try
			{
				_ItemJsonData itemJsonData = _ItemJsonData.DataDict[a.itemId];
				_ItemJsonData itemJsonData2 = _ItemJsonData.DataDict[b.itemId];
				JSONObject seid = a.Seid;
				JSONObject seid2 = b.Seid;
				int num = itemJsonData.GetHashCode();
				int num2 = itemJsonData2.GetHashCode();
				int num3 = itemJsonData.quality;
				int num4 = itemJsonData2.quality;
				if (seid != null && seid.HasField("quality"))
				{
					num3 = seid["quality"].I;
					num += seid.GetHashCode();
				}
				if (seid2 != null && seid2.HasField("quality"))
				{
					num4 = seid2["quality"].I;
					num2 += seid2.GetHashCode();
				}
				if (itemJsonData.type == 3 || itemJsonData.type == 4)
				{
					num3 *= 2;
				}
				if (itemJsonData2.type == 3 || itemJsonData2.type == 4)
				{
					num4 *= 2;
				}
				if (itemJsonData.type == 0 || itemJsonData.type == 1 || itemJsonData.type == 2)
				{
					num3++;
				}
				if (itemJsonData2.type == 0 || itemJsonData2.type == 1 || itemJsonData2.type == 2)
				{
					num4++;
				}
				if (num3 != num4)
				{
					return num4.CompareTo(num3);
				}
				if (itemJsonData.type != itemJsonData2.type)
				{
					return itemJsonData.type.CompareTo(itemJsonData2.type);
				}
				if (itemJsonData.id != itemJsonData2.id)
				{
					return itemJsonData.id.CompareTo(itemJsonData2.id);
				}
				return num.CompareTo(num2);
			}
			catch
			{
				return 1;
			}
		});
	}
}
