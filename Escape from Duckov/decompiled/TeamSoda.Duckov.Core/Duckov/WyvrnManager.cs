using System.Collections;
using UnityEngine;
using WyvrnSDK;

namespace Duckov;

public class WyvrnManager : MonoBehaviour
{
	private bool initialized;

	private int _mResult;

	public static WyvrnManager Instance { get; private set; }

	public static bool Initialized
	{
		get
		{
			if (Instance == null)
			{
				return false;
			}
			return Instance.initialized;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Multiple wyvrn managers detected");
		}
		Instance = this;
		HardwareSyncingManager.OnSetEvent += OnHardwareSyncingManagerSetEvent;
	}

	private void OnDestroy()
	{
		HardwareSyncingManager.OnSetEvent -= OnHardwareSyncingManagerSetEvent;
		if (Initialized)
		{
			int num = WyvrnAPI.CoreUnInit();
			if (num != 0)
			{
				Debug.LogError($"[WYVRN] Failed uninitializing wyvrn api. code: {num}");
			}
			initialized = false;
		}
	}

	private void OnHardwareSyncingManagerSetEvent(string eventName)
	{
		SetEvent(eventName);
	}

	public IEnumerator Start()
	{
		if (!WyvrnAPI.IsWyvrnSDKAvailable())
		{
			_mResult = 6023;
			yield break;
		}
		APPINFOTYPE appInfo = default(APPINFOTYPE);
		appInfo.Title = "Escape From Duckov";
		appInfo.Description = "Escape From Duckov";
		appInfo.Author_Name = "Team Soda";
		appInfo.Author_Contact = "https://game.bilibili.com/duckov/";
		appInfo.Category = 2u;
		_mResult = WyvrnAPI.CoreInitSDK(ref appInfo);
		if (_mResult == 0)
		{
			initialized = true;
		}
	}

	public void SetEvent(string eventName)
	{
		if (Initialized)
		{
			int num = WyvrnAPI.CoreSetEventName(eventName);
			if (num != 0)
			{
				Debug.LogError($"[WYVRN] Failed setting event in wyvrn api. code: {num}");
			}
		}
	}
}
