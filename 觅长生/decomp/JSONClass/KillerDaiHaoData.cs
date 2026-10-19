using System;
using System.Collections.Generic;

namespace JSONClass;

public class KillerDaiHaoData : IJSONClass
{
	public static Dictionary<int, KillerDaiHaoData> DataDict = new Dictionary<int, KillerDaiHaoData>();

	public static List<KillerDaiHaoData> DataList = new List<KillerDaiHaoData>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public string Name;

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.KillerDaiHaoData.list)
		{
			try
			{
				KillerDaiHaoData killerDaiHaoData = new KillerDaiHaoData();
				killerDaiHaoData.id = item["id"].I;
				killerDaiHaoData.Name = item["Name"].Str;
				if (DataDict.ContainsKey(killerDaiHaoData.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典KillerDaiHaoData.DataDict添加数据时出现重复的键，Key:{killerDaiHaoData.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(killerDaiHaoData.id, killerDaiHaoData);
				DataList.Add(killerDaiHaoData);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典KillerDaiHaoData.DataDict添加数据时出现异常，已跳过，请检查配表");
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
