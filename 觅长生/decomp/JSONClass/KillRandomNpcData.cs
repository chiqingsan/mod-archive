using System;
using System.Collections.Generic;

namespace JSONClass;

public class KillRandomNpcData : IJSONClass
{
	public static Dictionary<int, KillRandomNpcData> DataDict = new Dictionary<int, KillRandomNpcData>();

	public static List<KillRandomNpcData> DataList = new List<KillRandomNpcData>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int Time;

	public List<int> LiuPai = new List<int>();

	public List<int> Level = new List<int>();

	public List<int> XingGe = new List<int>();

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.KillRandomNpcData.list)
		{
			try
			{
				KillRandomNpcData killRandomNpcData = new KillRandomNpcData();
				killRandomNpcData.id = item["id"].I;
				killRandomNpcData.Time = item["Time"].I;
				killRandomNpcData.LiuPai = item["LiuPai"].ToList();
				killRandomNpcData.Level = item["Level"].ToList();
				killRandomNpcData.XingGe = item["XingGe"].ToList();
				if (DataDict.ContainsKey(killRandomNpcData.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典KillRandomNpcData.DataDict添加数据时出现重复的键，Key:{killRandomNpcData.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(killRandomNpcData.id, killRandomNpcData);
				DataList.Add(killRandomNpcData);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典KillRandomNpcData.DataDict添加数据时出现异常，已跳过，请检查配表");
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
