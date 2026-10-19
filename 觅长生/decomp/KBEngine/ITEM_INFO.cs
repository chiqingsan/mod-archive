namespace KBEngine;

public class ITEM_INFO
{
	public ulong UUID;

	public string uuid = "";

	public int itemId;

	public uint itemCount;

	public int itemIndex;

	public JSONObject Seid = new JSONObject(JSONObject.Type.OBJECT);

	public override string ToString()
	{
		return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("" + $"UUID:{UUID}\n", "uuid:", uuid, "\n"), $"itemId:{itemId}\n"), $"itemCount:{itemCount}\n"), $"itemIndex:{itemIndex}\n"), $"Seid:{Seid}\n");
	}

	public JSONObject ToJSONObject()
	{
		JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
		jSONObject.SetField("UUID", uuid);
		jSONObject.SetField("id", itemId);
		jSONObject.SetField("count", itemCount);
		jSONObject.SetField("index", itemIndex);
		jSONObject.SetField("Seid", Seid);
		return jSONObject;
	}

	public static ITEM_INFO FromJSONObject(JSONObject json)
	{
		ITEM_INFO iTEM_INFO = new ITEM_INFO();
		iTEM_INFO.uuid = json.GetString("UUID");
		if (iTEM_INFO.uuid == "")
		{
			iTEM_INFO.uuid = Tools.getUUID();
		}
		iTEM_INFO.itemId = json.GetInt("id");
		iTEM_INFO.itemCount = (uint)json.GetInt("count");
		iTEM_INFO.itemIndex = json.GetInt("index");
		iTEM_INFO.Seid = json.GetField("Seid");
		return iTEM_INFO;
	}
}
