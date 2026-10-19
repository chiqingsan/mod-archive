using System.Collections.Generic;
using Steamworks;
using script.Steam.UI;
using script.Steam.UI.Base;

namespace script.Steam.Ctr;

public class WorkShopCtr
{
	private EUGCQuery queryType = EUGCQuery.k_EUGCQuery_RankedByTotalUniqueSubscriptions;

	private List<string> tags = new List<string>();

	private List<ModUI> list = new List<ModUI>();

	private bool isFirstQuery = true;

	private bool isQuerying;

	public uint CurPage = 1u;

	public uint MaxPage = 1u;

	private UGCQueryHandle_t ugcQueryHandle;

	public WorkShopUI UI => WorkShopMag.Inst.WorkShopUI;

	public bool IsQuerying
	{
		get
		{
			return isQuerying;
		}
		set
		{
			UI.Loading.SetActive(value);
			isQuerying = value;
		}
	}

	public void AddPage()
	{
		if (CurPage + 1 <= MaxPage)
		{
			CurPage++;
			UpdateList();
		}
	}

	public void ReducePage()
	{
		if (CurPage - 1 >= 1)
		{
			CurPage--;
			UpdateList();
		}
	}

	public void SetQueryType(int value)
	{
		switch (value)
		{
		case 0:
			queryType = EUGCQuery.k_EUGCQuery_RankedByTotalUniqueSubscriptions;
			break;
		case 1:
			queryType = EUGCQuery.k_EUGCQuery_RankedByPublicationDate;
			break;
		case 2:
			queryType = EUGCQuery.k_EUGCQuery_RankedByTrend;
			break;
		}
		UpdateList();
	}

	public void AddTag(string tag)
	{
		if (!tags.Contains(tag))
		{
			tags.Add(tag);
			UpdateList();
		}
	}

	public void RemoveTag(string tag)
	{
		if (tags.Contains(tag))
		{
			tags.Remove(tag);
			UpdateList();
		}
	}

	public void UpdateList(bool isWithOutIsQuerying = false)
	{
		if (!isWithOutIsQuerying)
		{
			if (IsQuerying)
			{
				UIPopTip.Inst.Pop("正在查询中，请稍等");
				return;
			}
			if (!isFirstQuery)
			{
				SteamUGC.ReleaseQueryUGCRequest(ugcQueryHandle);
			}
		}
		Clear();
		list = new List<ModUI>();
		CallResult<SteamUGCQueryCompleted_t> callResult = CallResult<SteamUGCQueryCompleted_t>.Create();
		ugcQueryHandle = SteamUGC.CreateQueryAllUGCRequest(queryType, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, SteamUtils.GetAppID(), SteamUtils.GetAppID(), CurPage);
		if (queryType == EUGCQuery.k_EUGCQuery_NotYetRated)
		{
			SteamUGC.SetRankedByTrendDays(ugcQueryHandle, 30u);
		}
		foreach (string tag in tags)
		{
			SteamUGC.AddRequiredTag(ugcQueryHandle, tag);
		}
		SteamUGC.SetRankedByTrendDays(ugcQueryHandle, 90u);
		SteamUGC.SetAllowCachedResponse(ugcQueryHandle, 300u);
		SteamUGC.SetReturnChildren(ugcQueryHandle, bReturnChildren: true);
		SteamUGC.SetReturnMetadata(ugcQueryHandle, bReturnMetadata: true);
		SteamUGC.SetReturnKeyValueTags(ugcQueryHandle, bReturnKeyValueTags: true);
		SteamUGC.SetReturnLongDescription(ugcQueryHandle, bReturnLongDescription: true);
		SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(ugcQueryHandle);
		IsQuerying = true;
		callResult.Set(hAPICall, delegate(SteamUGCQueryCompleted_t t, bool failure)
		{
			if (!UI.IsActive())
			{
				isFirstQuery = false;
				IsQuerying = false;
			}
			else if (t.m_eResult == EResult.k_EResultOK)
			{
				SetMaxPage(t.m_unTotalMatchingResults);
				for (uint num = 0u; num < t.m_unNumResultsReturned; num++)
				{
					ModInfo modInfo = null;
					SteamUGC.GetQueryUGCResult(ugcQueryHandle, num, out var pDetails);
					if (!string.IsNullOrEmpty(pDetails.m_rgchTitle))
					{
						if (WorkShopMag.Inst.ModInfoDict.ContainsKey(pDetails.m_nPublishedFileId.m_PublishedFileId))
						{
							modInfo = WorkShopMag.Inst.ModInfoDict[pDetails.m_nPublishedFileId.m_PublishedFileId];
						}
						else
						{
							modInfo = new ModInfo();
							modInfo.Name = pDetails.m_rgchTitle;
							modInfo.Id = pDetails.m_nPublishedFileId.m_PublishedFileId;
							modInfo.Desc = pDetails.m_rgchDescription;
							modInfo.SetTags(pDetails.m_rgchTags);
							SteamUGC.GetQueryUGCPreviewURL(ugcQueryHandle, num, out modInfo.ImgUrl, (uint)pDetails.m_nPreviewFileSize);
							string pchMetadata = "";
							SteamUGC.GetQueryUGCMetadata(ugcQueryHandle, num, out pchMetadata, 5000u);
							modInfo.SetAuthor(pchMetadata);
							modInfo.UpNum = (int)pDetails.m_unVotesUp;
							modInfo.DownNum = (int)pDetails.m_unVotesDown;
							modInfo.Subscriptions = GetItemSubscriptions(ugcQueryHandle, num);
							modInfo.DependencyList = GetDependencies(ugcQueryHandle, num, pDetails.m_unNumChildren);
							WorkShopMag.Inst.ModInfoDict.Add(pDetails.m_nPublishedFileId.m_PublishedFileId, modInfo);
						}
						ModUI modUI = WorkShopMag.Inst.ModPoolUI.Ctr.GetModUI(1);
						modUI.BindingInfo(modInfo);
						list.Add(modUI);
					}
				}
				IsQuerying = false;
				UI.CurPage.SetText($"{CurPage}/{MaxPage}");
				IsQuerying = false;
				if (isFirstQuery)
				{
					isFirstQuery = false;
				}
			}
			else
			{
				UIPopTip.Inst.Pop("获取MOD列表失败，请重试");
			}
		});
	}

	public void SetMaxPage(uint num)
	{
		MaxPage = num / 50;
		if (num % 50 != 0)
		{
			MaxPage++;
		}
	}

	public void SubscriptionMod(ulong id, bool isShow = true)
	{
		SteamUGC.SubscribeItem(new PublishedFileId_t(id));
		WorkShopMag.Inst.ModMagUI.Ctr.AddSubscribeMod(id);
		if (isShow)
		{
			UIPopTip.Inst.Pop("订阅成功");
		}
	}

	public void UnSubscriptionMod(ulong id)
	{
		SteamUGC.UnsubscribeItem(new PublishedFileId_t(id));
		WorkShopMag.Inst.ModMagUI.Ctr.UnSubscribeMod(id);
		UIPopTip.Inst.Pop("取消订阅成功");
	}

	public List<ulong> GetNoSubscriptDependency(ModInfo info)
	{
		List<ulong> list = new List<ulong>();
		foreach (ulong dependency in info.DependencyList)
		{
			if (!WorkShopMag.Inst.ModMagUI.Ctr.IsSubscribe(dependency))
			{
				list.Add(dependency);
			}
		}
		return list;
	}

	private ulong GetItemSubscriptions(UGCQueryHandle_t handle, uint index)
	{
		ulong pStatValue = 1uL;
		SteamUGC.GetQueryUGCStatistic(handle, index, EItemStatistic.k_EItemStatistic_NumSubscriptions, out pStatValue);
		return pStatValue;
	}

	private List<ulong> GetDependencies(UGCQueryHandle_t handle, uint index, uint count)
	{
		PublishedFileId_t[] array = new PublishedFileId_t[count];
		SteamUGC.GetQueryUGCChildren(handle, index, array, count);
		List<ulong> list = new List<ulong>();
		PublishedFileId_t[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			PublishedFileId_t publishedFileId_t = array2[i];
			list.Add(publishedFileId_t.m_PublishedFileId);
		}
		return list;
	}

	public void Clear()
	{
		if (UI.CurSelect != null)
		{
			UI.CurSelect.CancelSelect();
		}
		WorkShopMag.Inst.MoreModInfoUI.Hide();
		UI.CurSelect = null;
		foreach (ModUI item in list)
		{
			WorkShopMag.Inst.ModPoolUI.Ctr.BackMod(item);
		}
		UI.CurSelect = null;
	}
}
