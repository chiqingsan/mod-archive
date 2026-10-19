using System;
using System.Collections.Generic;

namespace JSONClass;

public class ItemsSeidJsonData38 : IJSONClass
{
	public static int SEIDID = 38;

	public static Dictionary<int, ItemsSeidJsonData38> DataDict = new Dictionary<int, ItemsSeidJsonData38>();

	public static List<ItemsSeidJsonData38> DataList = new List<ItemsSeidJsonData38>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int value1;

	public int value2;

	public string yunsuan;

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.ItemsSeidJsonData[38].list)
		{
			try
			{
				ItemsSeidJsonData38 itemsSeidJsonData = new ItemsSeidJsonData38();
				itemsSeidJsonData.id = item["id"].I;
				itemsSeidJsonData.value1 = item["value1"].I;
				itemsSeidJsonData.value2 = item["value2"].I;
				itemsSeidJsonData.yunsuan = item["yunsuan"].Str;
				if (DataDict.ContainsKey(itemsSeidJsonData.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典ItemsSeidJsonData38.DataDict添加数据时出现重复的键，Key:{itemsSeidJsonData.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(itemsSeidJsonData.id, itemsSeidJsonData);
				DataList.Add(itemsSeidJsonData);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典ItemsSeidJsonData38.DataDict添加数据时出现异常，已跳过，请检查配表");
				PreloadManager.LogException($"异常信息:\n{arg}");
				PreloadManager.LogException($"数据序列化:\n{item}");
			}
		}
		if (OnInitFinishAction != null)
		{
			OnInitFinishAction();
		}
	}

	private static void OnInitFinish()
	{
	}
}
