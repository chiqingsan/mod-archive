using System;
using System.Collections.Generic;

namespace JSONClass;

public class NpcTalkSpecialNPCAddress : IJSONClass
{
	public static Dictionary<int, NpcTalkSpecialNPCAddress> DataDict = new Dictionary<int, NpcTalkSpecialNPCAddress>();

	public static List<NpcTalkSpecialNPCAddress> DataList = new List<NpcTalkSpecialNPCAddress>();

	public static Action OnInitFinishAction = OnInitFinish;

	public int id;

	public string daolvAddress;

	public string normalAddress;

	public static void InitDataDict()
	{
		foreach (JSONObject item in jsonData.instance.NpcTalkSpecialNPCAddress.list)
		{
			try
			{
				NpcTalkSpecialNPCAddress npcTalkSpecialNPCAddress = new NpcTalkSpecialNPCAddress();
				npcTalkSpecialNPCAddress.id = item["id"].I;
				npcTalkSpecialNPCAddress.daolvAddress = item["daolvAddress"].Str;
				npcTalkSpecialNPCAddress.normalAddress = item["normalAddress"].Str;
				if (DataDict.ContainsKey(npcTalkSpecialNPCAddress.id))
				{
					PreloadManager.LogException($"!!!错误!!!向字典NpcTalkSpecialNPCAddress.DataDict添加数据时出现重复的键，Key:{npcTalkSpecialNPCAddress.id}，已跳过，请检查配表");
					continue;
				}
				DataDict.Add(npcTalkSpecialNPCAddress.id, npcTalkSpecialNPCAddress);
				DataList.Add(npcTalkSpecialNPCAddress);
			}
			catch (Exception arg)
			{
				PreloadManager.LogException("!!!错误!!!向字典NpcTalkSpecialNPCAddress.DataDict添加数据时出现异常，已跳过，请检查配表");
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
