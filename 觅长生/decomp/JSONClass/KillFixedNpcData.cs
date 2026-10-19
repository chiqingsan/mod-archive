using System;
using System.Collections.Generic;

namespace JSONClass;

public class KillFixedNpcData : IJSONClass
{
	public static Dictionary<int, KillFixedNpcData> DataDict = new Dictionary<int, KillFixedNpcData>();

	public static List<KillFixedNpcData> DataList = new List<KillFixedNpcData>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int AvaterID;

	public string StarTime;

	public string EndTime;

	public string fuhao;

	public List<int> JingJie = new List<int>();

	public List<int> EventValue = new List<int>();

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.KillFixedNpcData.list)
		{
			try
			{
				KillFixedNpcData killFixedNpcData = new KillFixedNpcData();
				killFixedNpcData.id = item["id"].I;
				killFixedNpcData.AvaterID = item["AvaterID"].I;
				killFixedNpcData.StarTime = item["StarTime"].Str;
				killFixedNpcData.EndTime = item["EndTime"].Str;
				killFixedNpcData.fuhao = item["fuhao"].Str;
				killFixedNpcData.JingJie = item["JingJie"].ToList();
				killFixedNpcData.EventValue = item["EventValue"].ToList();
				if (DataDict.ContainsKey(killFixedNpcData.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典KillFixedNpcData.DataDict添加数据时出现重复的键，Key:{killFixedNpcData.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(killFixedNpcData.id, killFixedNpcData);
				DataList.Add(killFixedNpcData);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典KillFixedNpcData.DataDict添加数据时出现异常，已跳过，请检查配表");
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
