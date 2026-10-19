using System;
using System.Collections.Generic;

namespace JSONClass;

public class KillerCreateData : IJSONClass
{
	public static Dictionary<int, KillerCreateData> DataDict = new Dictionary<int, KillerCreateData>();

	public static List<KillerCreateData> DataList = new List<KillerCreateData>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int ZongShangJin;

	public int ShengWang;

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.KillerCreateData.list)
		{
			try
			{
				KillerCreateData killerCreateData = new KillerCreateData();
				killerCreateData.id = item["id"].I;
				killerCreateData.ZongShangJin = item["ZongShangJin"].I;
				killerCreateData.ShengWang = item["ShengWang"].I;
				if (DataDict.ContainsKey(killerCreateData.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典KillerCreateData.DataDict添加数据时出现重复的键，Key:{killerCreateData.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(killerCreateData.id, killerCreateData);
				DataList.Add(killerCreateData);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典KillerCreateData.DataDict添加数据时出现异常，已跳过，请检查配表");
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
