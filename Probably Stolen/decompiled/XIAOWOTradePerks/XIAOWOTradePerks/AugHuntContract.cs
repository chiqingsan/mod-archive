using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;

namespace XIAOWOTradePerks;

internal static class AugHuntContract
{
	internal const int ExtraVisitChance = 30;

	internal const int DailyAugLimit = 2;

	internal const string GameOverReason = "aug";

	private static readonly HashSet<long> SpawnedAugs = new HashSet<long>();

	private static readonly HashSet<long> RequiredKillAugs = new HashSet<long>();

	private static readonly HashSet<long> KilledAugs = new HashSet<long>();

	private static readonly HashSet<long> AddAugBaseline = new HashSet<long>();

	private static string _dayKey = string.Empty;

	private static string _extraAttemptKey = string.Empty;

	private static bool _escapedAlive;

	private static bool _sleepResolved;

	private static bool _ignoreLoadedDayForRetaliation;

	[ThreadStatic]
	private static bool _forceNextNativeAugRoll;

	[ThreadStatic]
	private static bool _blockNativeAugAdd;

	internal static int SpawnedCount => SpawnedAugs.Count;

	internal static int RequiredKillCount => RequiredKillAugs.Count;

	internal static int KilledCount => KilledAugs.Count;

	internal static bool EscapedAlive => _escapedAlive;

	internal static bool IgnoringLoadedDayForRetaliation => _ignoreLoadedDayForRetaliation;

	internal static bool ForceNextNativeAugRoll => _forceNextNativeAugRoll;

	internal static void BeforeDailyGeneration(StoreClientManager manager, out bool eligible)
	{
		EnsureDay();
		TrackExistingAugs(manager);
		PlayerStore instance = PlayerStore.Instance;
		bool flag = IsContractRunning(instance);
		_blockNativeAugAdd = flag && SpawnedCount > 0;
		eligible = flag && IsNativeAugWindow(manager, instance, StoreStation.instance);
	}

	internal static void AfterDailyGeneration(StoreClientManager manager, bool eligible)
	{
		try
		{
			EnsureDay();
			TrackExistingAugs(manager);
			if (!eligible || !IsContractRunning(PlayerStore.Instance) || string.Equals(_extraAttemptKey, _dayKey, StringComparison.Ordinal))
			{
				return;
			}
			_extraAttemptKey = _dayKey;
			if (SpawnedCount < 2 && RNG.Roll(30))
			{
				_forceNextNativeAugRoll = true;
				manager.AddAugClient();
				Instance log = Core.Log;
				if (log != null)
				{
					log.Msg("Aug hunt contract extra visit roll succeeded; daily Aug count=" + SpawnedCount + ".");
				}
			}
		}
		finally
		{
			_forceNextNativeAugRoll = false;
			_blockNativeAugAdd = false;
			TrackExistingAugs(manager);
		}
	}

	internal static void AbortDailyGeneration()
	{
		_forceNextNativeAugRoll = false;
		_blockNativeAugAdd = false;
		AddAugBaseline.Clear();
	}

	internal static bool BeforeAddAug(StoreClientManager manager)
	{
		EnsureDay();
		TrackExistingAugs(manager);
		AddAugBaseline.Clear();
		foreach (long spawnedAug in SpawnedAugs)
		{
			AddAugBaseline.Add(spawnedAug);
		}
		return CanAddAug(IsContractRunning(PlayerStore.Instance), SpawnedCount, _forceNextNativeAugRoll, _blockNativeAugAdd);
	}

	internal static void AfterAddAug(StoreClientManager manager)
	{
		EnsureDay();
		TrackExistingAugs(manager);
		if (IsContractRunning(PlayerStore.Instance) && !_ignoreLoadedDayForRetaliation)
		{
			foreach (long spawnedAug in SpawnedAugs)
			{
				if (!AddAugBaseline.Contains(spawnedAug))
				{
					RequiredKillAugs.Add(spawnedAug);
				}
			}
		}
		AddAugBaseline.Clear();
	}

	internal static void ForceNativeRoll(ref int chance)
	{
		if (_forceNextNativeAugRoll && chance == 80)
		{
			chance = 100;
			_forceNextNativeAugRoll = false;
		}
	}

	internal static void MarkCurrentAugKilled()
	{
		EnsureDay();
		PlayerStore instance = PlayerStore.Instance;
		object obj;
		if (instance == null)
		{
			obj = null;
		}
		else
		{
			StoreClientInstance currentClientInstance = instance.currentClientInstance;
			obj = ((currentClientInstance != null) ? currentClientInstance.storeClient : null);
		}
		StoreClient val = (StoreClient)obj;
		if (val == null || !val.isAug)
		{
			return;
		}
		long num = ((Il2CppObjectBase)val).Pointer.ToInt64();
		if (num != 0L)
		{
			SpawnedAugs.Add(num);
			RequiredKillAugs.Add(num);
			KilledAugs.Add(num);
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Aug hunt contract recorded a kill; " + KilledCount + "/" + RequiredKillCount + ".");
			}
		}
	}

	internal static void MarkCurrentAugEscaped()
	{
		EnsureDay();
		PlayerStore instance = PlayerStore.Instance;
		object obj;
		if (instance == null)
		{
			obj = null;
		}
		else
		{
			StoreClientInstance currentClientInstance = instance.currentClientInstance;
			obj = ((currentClientInstance != null) ? currentClientInstance.storeClient : null);
		}
		StoreClient val = (StoreClient)obj;
		if (val != null && val.isAug)
		{
			long num = ((Il2CppObjectBase)val).Pointer.ToInt64();
			if (num != 0L)
			{
				SpawnedAugs.Add(num);
				RequiredKillAugs.Add(num);
			}
		}
		_escapedAlive = true;
	}

	internal static bool ResolveSleep(Action<string> executeGameOver)
	{
		EnsureDay();
		PlayerStore instance = PlayerStore.Instance;
		TrackExistingAugs((instance != null) ? instance.storeClientManager : null);
		return InvokeGameOverIfNeeded(IsContractRunning(PlayerStore.Instance), RequiredKillCount, KilledCount, _escapedAlive, executeGameOver);
	}

	internal static bool InvokeGameOverIfNeeded(bool active, int spawned, int killed, bool escapedAlive, Action<string> executeGameOver)
	{
		if (!ShouldGameOver(active, spawned, killed, escapedAlive) || executeGameOver == null)
		{
			return false;
		}
		if (_sleepResolved)
		{
			return false;
		}
		_sleepResolved = true;
		executeGameOver("aug");
		return true;
	}

	internal static bool ShouldGameOver(bool active, int spawned, int killed, bool escapedAlive)
	{
		if (active && spawned > 0)
		{
			if (!escapedAlive)
			{
				return killed < spawned;
			}
			return true;
		}
		return false;
	}

	internal static bool ShouldContractRun(bool perkActive, bool shootingUnlocked)
	{
		return perkActive && shootingUnlocked;
	}

	internal static bool IsContractRunning(PlayerStore store)
	{
		return ShouldContractRun(PerkRegistry.IsActive("xiaowo_trade_aug_hunt_contract"), store != null && store.isShootingUnlocked);
	}

	internal static bool CanAddAug(bool contractRunning, int spawned, bool isExtraAdd, bool blockNativeAdd)
	{
		if (!contractRunning)
		{
			return true;
		}
		if (spawned >= 2)
		{
			return false;
		}
		return !blockNativeAdd || isExtraAdd;
	}

	internal static bool IsNativeAugWindow(StoreClientManager manager, PlayerStore store, StoreStation station)
	{
		if (manager == null || store == null || station == null)
		{
			return false;
		}
		int dayCounter = StoreStation.GetDayCounter();
		if (!store.isShootingUnlocked || !store.isAugIntroduced || store.isAugEnded || manager.isAugSpawnHandledToday)
		{
			return false;
		}
		if (dayCounter == 13 || dayCounter == 18)
		{
			return false;
		}
		if (dayCounter >= 62 && store.augPresence <= 0)
		{
			return false;
		}
		if (store.nextAugSpawnDay >= 0)
		{
			return dayCounter >= store.nextAugSpawnDay;
		}
		return false;
	}

	internal static void ResetForTest()
	{
		_dayKey = string.Empty;
		_extraAttemptKey = string.Empty;
		SpawnedAugs.Clear();
		RequiredKillAugs.Clear();
		KilledAugs.Clear();
		AddAugBaseline.Clear();
		_escapedAlive = false;
		_sleepResolved = false;
		_ignoreLoadedDayForRetaliation = false;
		_forceNextNativeAugRoll = false;
		_blockNativeAugAdd = false;
	}

	internal static void OnShotgunStateChanged()
	{
		ClearTracking();
		_ignoreLoadedDayForRetaliation = false;
		_dayKey = GetDayKey();
	}

	internal static void OnGameLoading()
	{
		_dayKey = string.Empty;
		ClearTracking();
		_ignoreLoadedDayForRetaliation = true;
	}

	private static void EnsureDay()
	{
		string dayKey = GetDayKey();
		if (!string.Equals(dayKey, _dayKey, StringComparison.Ordinal))
		{
			bool num = _ignoreLoadedDayForRetaliation && _dayKey.Length == 0;
			_dayKey = dayKey;
			ClearTracking();
			if (!num)
			{
				_ignoreLoadedDayForRetaliation = false;
			}
		}
	}

	private static string GetDayKey()
	{
		PlayerStore instance = PlayerStore.Instance;
		return (((instance != null) ? instance.runID : null) ?? "no-run") + "|" + Math.Max(0, StoreStation.GetDayCounter());
	}

	private static void ClearTracking()
	{
		SpawnedAugs.Clear();
		RequiredKillAugs.Clear();
		KilledAugs.Clear();
		AddAugBaseline.Clear();
		_extraAttemptKey = string.Empty;
		_escapedAlive = false;
		_sleepResolved = false;
		_forceNextNativeAugRoll = false;
		_blockNativeAugAdd = false;
	}

	private static void TrackExistingAugs(StoreClientManager manager)
	{
		if (((manager != null) ? manager.clientStack : null) != null)
		{
			for (int i = 0; i < manager.clientStack.Count; i++)
			{
				TrackAug(manager.clientStack[i]);
			}
		}
		PlayerStore instance = PlayerStore.Instance;
		object client;
		if (instance == null)
		{
			client = null;
		}
		else
		{
			StoreClientInstance currentClientInstance = instance.currentClientInstance;
			client = ((currentClientInstance != null) ? currentClientInstance.storeClient : null);
		}
		TrackAug((StoreClient)client);
	}

	private static void TrackAug(StoreClient client)
	{
		if (client != null && client.isAug)
		{
			long num = ((Il2CppObjectBase)client).Pointer.ToInt64();
			if (num != 0L)
			{
				SpawnedAugs.Add(num);
			}
		}
	}
}
