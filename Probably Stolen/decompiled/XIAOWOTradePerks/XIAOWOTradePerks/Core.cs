using System;
using System.Collections.Generic;
using Il2Cpp;
using MelonLoader;

namespace XIAOWOTradePerks;

public sealed class Core : MelonMod
{
	private string _lastLocaleCode = string.Empty;

	private int _localeCheckCountdown;

	private bool _applicationQuitting;

	private int _patchReportCountdown = 120;

	private readonly Dictionary<string, int> _tickFailures = new Dictionary<string, int>(StringComparer.Ordinal);

	internal static Instance Log { get; private set; }

	public override void OnInitializeMelon()
	{
		Log = ((MelonBase)this).LoggerInstance;
		try
		{
			((MelonBase)this).HarmonyInstance.UnpatchSelf();
			PatchCompatibility.Install(((MelonBase)this).HarmonyInstance);
		}
		catch (Exception ex)
		{
			Log.Error("Patch registration failed: " + ex);
		}
		TryCleanup("mod manager hotkey registration", ModManagerHotkey.Initialize);
		_lastLocaleCode = ModLocalization.CurrentLocaleCode;
		Log.Msg("Sixteen starting perks and two always-on features initialized. Locale: " + ((_lastLocaleCode.Length == 0) ? "default/en" : _lastLocaleCode));
	}

	public override void OnUpdate()
	{
		RunTick("game window", GameWindowResizer.Tick);
		if (PatchCompatibility.Available("perk-picker"))
		{
			RunTick("perk picker scroll", PerkPickerScroll.Tick);
		}
		RunTick("community overlay", CommunityOverlay.Tick);
		RunTick("mod manager", ModManagerUI.Tick);
		RunTick("mod manager capture", ModManagerUI.CaptureTick);
		RunTick("mod manager hotkey", ModManagerHotkey.Tick);
		if (PatchCompatibility.Available("precision-bay"))
		{
			RunTick("precision bay", PrecisionBayMode.Tick);
		}
		if (PatchCompatibility.Available("loading"))
		{
			RunTick("loading progress", LoadingProgressOverlay.Tick);
		}
		if (_patchReportCountdown-- == 0)
		{
			RunTick("patch overlap report", delegate
			{
				PatchCompatibility.ReportSharedMethods();
			});
		}
		if (_localeCheckCountdown-- > 0)
		{
			return;
		}
		_localeCheckCountdown = 30;
		string currentLocaleCode;
		try
		{
			currentLocaleCode = ModLocalization.CurrentLocaleCode;
		}
		catch (Exception ex)
		{
			Instance log = Log;
			if (log != null)
			{
				log.Warning("Language check skipped: " + ex.Message);
			}
			return;
		}
		if (!string.Equals(currentLocaleCode, _lastLocaleCode, StringComparison.OrdinalIgnoreCase))
		{
			_lastLocaleCode = currentLocaleCode;
			RunTick("perk language", PerkRegistry.RefreshLocalizedText);
			RunTick("free-build language", FreeBuildMode.RefreshLocalizedText);
			RunTick("precision bay language", PrecisionBayMode.RefreshLocalizedText);
			RunTick("calendar language", delegate
			{
				StoreStation instance2 = StoreStation.instance;
				DeterministicSchedule.RefreshLocalizedText((instance2 != null) ? instance2.storeEventManager : null);
			});
			RunTick("owner language", delegate
			{
				PlayerStore instance = PlayerStore.instance;
				OwnerDeal.RefreshLocalizedDialogue((instance != null) ? instance.storeClientManager : null);
			});
			Instance log2 = Log;
			if (log2 != null)
			{
				log2.Msg("Trade perk language changed to: " + ((currentLocaleCode.Length == 0) ? "default/en" : currentLocaleCode));
			}
		}
	}

	public override void OnLateUpdate()
	{
		RunTick("container window", ContainerWindowResizer.Tick);
	}

	public override void OnApplicationQuit()
	{
		_applicationQuitting = true;
		OperatingSurcharge.DiscardTrackedServices();
	}

	public override void OnDeinitializeMelon()
	{
		if (_applicationQuitting)
		{
			OperatingSurcharge.DiscardTrackedServices();
		}
		else
		{
			TryCleanup("restore operating costs", OperatingSurcharge.RestoreKnownServices);
		}
		TryCleanup("game window resizer", GameWindowResizer.Dispose);
		TryCleanup("container window resizer", ContainerWindowResizer.Dispose);
		TryCleanup("free-build mode", FreeBuildMode.Dispose);
		TryCleanup("precision bay mode", PrecisionBayMode.Dispose);
		TryCleanup("mod manager", ModManagerUI.Dispose);
		TryCleanup("community overlay", CommunityOverlay.Dispose);
		TryCleanup("loading overlay", LoadingProgressOverlay.Dispose);
		TryCleanup("feature patches", PatchCompatibility.Dispose);
	}

	private void RunTick(string name, Action action)
	{
		if (_tickFailures.TryGetValue(name, out var value) && value >= 3)
		{
			return;
		}
		try
		{
			action();
			if (value != 0)
			{
				_tickFailures.Remove(name);
			}
		}
		catch (Exception ex)
		{
			value++;
			_tickFailures[name] = value;
			if (value == 1 || value == 3)
			{
				Instance log = Log;
				if (log != null)
				{
					log.Warning(name + " failed" + ((value == 3) ? " three times; paused until restart" : "") + ": " + ex);
				}
			}
		}
	}

	private static void TryCleanup(string component, Action action)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			Instance log = Log;
			if (log != null)
			{
				log.Warning("Cleanup skipped for " + component + ": " + ex.Message);
			}
		}
	}
}
