using System;
using System.Collections.Generic;

namespace JSONClass;

public class TianMoYanXuanXiang : IJSONClass
{
	public static Dictionary<int, TianMoYanXuanXiang> DataDict = new Dictionary<int, TianMoYanXuanXiang>();

	public static List<TianMoYanXuanXiang> DataList = new List<TianMoYanXuanXiang>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int value1;

	public int value2;

	public int value3;

	public int value4;

	public int value5;

	public int value8;

	public string EventName;

	public string desc;

	public List<int> value6 = new List<int>();

	public List<int> value7 = new List<int>();

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.TianMoYanXuanXiang.list)
		{
			try
			{
				TianMoYanXuanXiang tianMoYanXuanXiang = new TianMoYanXuanXiang();
				tianMoYanXuanXiang.id = item["id"].I;
				tianMoYanXuanXiang.value1 = item["value1"].I;
				tianMoYanXuanXiang.value2 = item["value2"].I;
				tianMoYanXuanXiang.value3 = item["value3"].I;
				tianMoYanXuanXiang.value4 = item["value4"].I;
				tianMoYanXuanXiang.value5 = item["value5"].I;
				tianMoYanXuanXiang.value8 = item["value8"].I;
				tianMoYanXuanXiang.EventName = item["EventName"].Str;
				tianMoYanXuanXiang.desc = item["desc"].Str;
				tianMoYanXuanXiang.value6 = item["value6"].ToList();
				tianMoYanXuanXiang.value7 = item["value7"].ToList();
				if (DataDict.ContainsKey(tianMoYanXuanXiang.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典TianMoYanXuanXiang.DataDict添加数据时出现重复的键，Key:{tianMoYanXuanXiang.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(tianMoYanXuanXiang.id, tianMoYanXuanXiang);
				DataList.Add(tianMoYanXuanXiang);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典TianMoYanXuanXiang.DataDict添加数据时出现异常，已跳过，请检查配表");
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
