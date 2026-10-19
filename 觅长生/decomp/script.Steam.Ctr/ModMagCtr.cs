using System.Collections.Generic;
using Steamworks;
using script.Steam.UI;
using script.Steam.UI.Base;

namespace script.Steam.Ctr;

public class ModMagCtr
{
	private readonly List<ulong> subscribeModList = new List<ulong>();

	private List<ModUI> list = new List<ModUI>();

	public int ModNum;

	private bool isFirstQuery = true;

	private bool isQuerying;

	public uint CurPage = 1u;

	public uint MaxPage = 1u;

	private UGCQueryHandle_t ugcQueryHandle;

	public ModMagUI UI => WorkShopMag.Inst.ModMagUI;

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

	public List<ulong> GetSubscribeModList()
	{
		return subscribeModList;
	}

	public ModMagCtr()
	{
		ModNum = (int)SteamUGC.GetNumSubscribedItems();
		PublishedFileId_t[] array = new PublishedFileId_t[ModNum];
		SteamUGC.GetSubscribedItems(array, (uint)ModNum);
		PublishedFileId_t[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			PublishedFileId_t publishedFileId_t = array2[i];
			subscribeModList.Add(publishedFileId_t.m_PublishedFileId);
		}
		MaxPage = (uint)ModNum / 50u;
		if (ModNum % 50 != 0)
		{
			MaxPage++;
		}
	}

	public void AddSubscribeMod(ulong id)
	{
		if (!subscribeModList.Contains(id))
		{
			SteamUGC.DownloadItem(new PublishedFileId_t(id), bHighPriority: false);
			subscribeModList.Add(id);
		}
	}

	public void UnSubscribeMod(ulong id)
	{
		if (subscribeModList.Contains(id))
		{
			subscribeModList.Remove(id);
		}
	}

	public bool IsSubscribe(ulong id)
	{
		return subscribeModList.Contains(id);
	}

	public void OpenMod(ulong id)
	{
		WorkshopTool.OpenMod(id.ToString());
		UIPopTip.Inst.Pop("Mod已启用");
	}

	public void CloseMod(ulong id)
	{
		WorkshopTool.CloseMod(id.ToString());
		UIPopTip.Inst.Pop("Mod已取消启用");
	}

	public void VoteMod(ulong id, bool flag)
	{
		SteamUGC.SetUserItemVote(new PublishedFileId_t(id), flag);
	}

	public bool IsOpen(ulong id)
	{
		return !WorkshopTool.CheckModIsDisable(id.ToString());
	}

	public void AddPage()
	{
		if (CurPage + 1 <= MaxPage)
		{
			CurPage++;
			UI.CurPage.SetText($"{CurPage}/{MaxPage}");
			UpdateList();
		}
	}

	public void ReducePage()
	{
		if (CurPage - 1 >= 1)
		{
			CurPage--;
			UI.CurPage.SetText($"{CurPage}/{MaxPage}");
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
		WorkShopMag.Inst.MoreModInfoUI.Hide();
		Clear();
		list = new List<ModUI>();
		CallResult<SteamUGCQueryCompleted_t> callResult = CallResult<SteamUGCQueryCompleted_t>.Create();
		ugcQueryHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Subscribed, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, SteamUtils.GetAppID(), SteamUtils.GetAppID(), CurPage);
		SteamUGC.SetRankedByTrendDays(ugcQueryHandle, 90u);
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
						ModUI modUI = WorkShopMag.Inst.ModPoolUI.Ctr.GetModUI();
						modUI.BindingInfo(modInfo);
						list.Add(modUI);
					}
				}
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
