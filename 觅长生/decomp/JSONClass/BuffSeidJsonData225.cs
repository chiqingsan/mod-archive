using System;
using System.Collections.Generic;

namespace JSONClass;

public class BuffSeidJsonData225 : IJSONClass
{
	public static int SEIDID = 225;

	public static Dictionary<int, BuffSeidJsonData225> DataDict = new Dictionary<int, BuffSeidJsonData225>();

	public static List<BuffSeidJsonData225> DataList = new List<BuffSeidJsonData225>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int target;

	public int value1;

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.BuffSeidJsonData[225].list)
		{
			try
			{
				BuffSeidJsonData225 buffSeidJsonData = new BuffSeidJsonData225();
				buffSeidJsonData.id = item["id"].I;
				buffSeidJsonData.target = item["target"].I;
				buffSeidJsonData.value1 = item["value1"].I;
				if (DataDict.ContainsKey(buffSeidJsonData.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典BuffSeidJsonData225.DataDict添加数据时出现重复的键，Key:{buffSeidJsonData.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(buffSeidJsonData.id, buffSeidJsonData);
				DataList.Add(buffSeidJsonData);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典BuffSeidJsonData225.DataDict添加数据时出现异常，已跳过，请检查配表");
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
