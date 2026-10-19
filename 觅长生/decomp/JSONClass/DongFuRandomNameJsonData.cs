using System;
using System.Collections.Generic;

namespace JSONClass;

public class DongFuRandomNameJsonData : IJSONClass
{
	public static Dictionary<int, DongFuRandomNameJsonData> DataDict = new Dictionary<int, DongFuRandomNameJsonData>();

	public static List<DongFuRandomNameJsonData> DataList = new List<DongFuRandomNameJsonData>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public string Name;

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.DongFuRandomNameJsonData.list)
		{
			try
			{
				DongFuRandomNameJsonData dongFuRandomNameJsonData = new DongFuRandomNameJsonData();
				dongFuRandomNameJsonData.id = item["id"].I;
				dongFuRandomNameJsonData.Name = item["Name"].Str;
				if (DataDict.ContainsKey(dongFuRandomNameJsonData.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典DongFuRandomNameJsonData.DataDict添加数据时出现重复的键，Key:{dongFuRandomNameJsonData.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(dongFuRandomNameJsonData.id, dongFuRandomNameJsonData);
				DataList.Add(dongFuRandomNameJsonData);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典DongFuRandomNameJsonData.DataDict添加数据时出现异常，已跳过，请检查配表");
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
