using System;
using System.Collections.Generic;

namespace JSONClass;

public class KillRewardData : IJSONClass
{
	public static Dictionary<int, KillRewardData> DataDict = new Dictionary<int, KillRewardData>();

	public static List<KillRewardData> DataList = new List<KillRewardData>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int NanDu;

	public int ShengWang;

	public List<int> ShangJin = new List<int>();

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.KillRewardData.list)
		{
			try
			{
				KillRewardData killRewardData = new KillRewardData();
				killRewardData.id = item["id"].I;
				killRewardData.NanDu = item["NanDu"].I;
				killRewardData.ShengWang = item["ShengWang"].I;
				killRewardData.ShangJin = item["ShangJin"].ToList();
				if (DataDict.ContainsKey(killRewardData.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典KillRewardData.DataDict添加数据时出现重复的键，Key:{killRewardData.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(killRewardData.id, killRewardData);
				DataList.Add(killRewardData);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典KillRewardData.DataDict添加数据时出现异常，已跳过，请检查配表");
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
