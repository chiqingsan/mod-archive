using System;
using System.Collections.Generic;

namespace JSONClass;

public class CrateAvatarSeidJsonData27 : IJSONClass
{
	public static int SEIDID = 27;

	public static Dictionary<int, CrateAvatarSeidJsonData27> DataDict = new Dictionary<int, CrateAvatarSeidJsonData27>();

	public static List<CrateAvatarSeidJsonData27> DataList = new List<CrateAvatarSeidJsonData27>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int value1;

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.CrateAvatarSeidJsonData[27].list)
		{
			try
			{
				CrateAvatarSeidJsonData27 crateAvatarSeidJsonData = new CrateAvatarSeidJsonData27();
				crateAvatarSeidJsonData.id = item["id"].I;
				crateAvatarSeidJsonData.value1 = item["value1"].I;
				if (DataDict.ContainsKey(crateAvatarSeidJsonData.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典CrateAvatarSeidJsonData27.DataDict添加数据时出现重复的键，Key:{crateAvatarSeidJsonData.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(crateAvatarSeidJsonData.id, crateAvatarSeidJsonData);
				DataList.Add(crateAvatarSeidJsonData);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典CrateAvatarSeidJsonData27.DataDict添加数据时出现异常，已跳过，请检查配表");
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
