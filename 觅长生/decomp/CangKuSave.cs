using System.Collections.Generic;

public class CangKuSave
{
	public List<CangKuData> CangKuList = new List<CangKuData>();

	public JSONObject ToJSONObject()
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.ARRAY);
		foreach (CangKuData cangKu in CangKuList)
		{
			jSONObject.Add(cangKu.ToJSONObject());
		}
		return jSONObject;
	}

	public static CangKuSave FromJSONObject(JSONObject json)
	{
		CangKuSave cangKuSave = new CangKuSave();
		if (json != null && json.list != null)
		{
			foreach (JSONObject item2 in json.list)
			{
				CangKuData item = CangKuData.FromJSONObject(item2);
				cangKuSave.CangKuList.Add(item);
			}
		}
		return cangKuSave;
	}
}
