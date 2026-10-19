using System;
using System.Collections.Generic;

namespace JSONClass;

public class GaoShiWenAn : IJSONClass
{
	public static Dictionary<int, GaoShiWenAn> DataDict = new Dictionary<int, GaoShiWenAn>();

	public static List<GaoShiWenAn> DataList = new List<GaoShiWenAn>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public int type;

	public string param;

	public string text;

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.GaoShiWenAn.list)
		{
			try
			{
				GaoShiWenAn gaoShiWenAn = new GaoShiWenAn();
				gaoShiWenAn.id = item["id"].I;
				gaoShiWenAn.type = item["type"].I;
				gaoShiWenAn.param = item["param"].Str;
				gaoShiWenAn.text = item["text"].Str;
				if (DataDict.ContainsKey(gaoShiWenAn.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典GaoShiWenAn.DataDict添加数据时出现重复的键，Key:{gaoShiWenAn.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(gaoShiWenAn.id, gaoShiWenAn);
				DataList.Add(gaoShiWenAn);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典GaoShiWenAn.DataDict添加数据时出现异常，已跳过，请检查配表");
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
