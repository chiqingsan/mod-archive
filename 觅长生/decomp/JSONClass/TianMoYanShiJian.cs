using System;
using System.Collections.Generic;

namespace JSONClass;

public class TianMoYanShiJian : IJSONClass
{
	public static Dictionary<int, TianMoYanShiJian> DataDict = new Dictionary<int, TianMoYanShiJian>();

	public static List<TianMoYanShiJian> DataList = new List<TianMoYanShiJian>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int option1;

	public int option2;

	public int option3;

	public int optionID;

	public string EventName;

	public string desc;

	public string optionDesc1;

	public string optionDesc2;

	public string optionDesc3;

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.TianMoYanShiJian.list)
		{
			try
			{
				TianMoYanShiJian tianMoYanShiJian = new TianMoYanShiJian();
				tianMoYanShiJian.id = item["id"].I;
				tianMoYanShiJian.option1 = item["option1"].I;
				tianMoYanShiJian.option2 = item["option2"].I;
				tianMoYanShiJian.option3 = item["option3"].I;
				tianMoYanShiJian.optionID = item["optionID"].I;
				tianMoYanShiJian.EventName = item["EventName"].Str;
				tianMoYanShiJian.desc = item["desc"].Str;
				tianMoYanShiJian.optionDesc1 = item["optionDesc1"].Str;
				tianMoYanShiJian.optionDesc2 = item["optionDesc2"].Str;
				tianMoYanShiJian.optionDesc3 = item["optionDesc3"].Str;
				if (DataDict.ContainsKey(tianMoYanShiJian.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典TianMoYanShiJian.DataDict添加数据时出现重复的键，Key:{tianMoYanShiJian.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(tianMoYanShiJian.id, tianMoYanShiJian);
				DataList.Add(tianMoYanShiJian);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典TianMoYanShiJian.DataDict添加数据时出现异常，已跳过，请检查配表");
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
