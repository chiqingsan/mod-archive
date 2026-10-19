using System;
using Il2Cpp;

namespace XIAOWOTradePerks;

internal static class NightScavengingRisk
{
	private enum WoundRollPhase
	{
		None,
		Minor,
		Major
	}

	internal const float WoundChanceBonus = 15f;

	[ThreadStatic]
	private static WoundRollPhase _nextRoll;

	[ThreadStatic]
	private static bool _majorRollEligible;

	internal static float AddToChance(float chance, bool active)
	{
		if (!active)
		{
			return chance;
		}
		return Math.Min(100f, chance + 15f);
	}

	internal static void Begin()
	{
		End();
		if (!PerkRegistry.IsActive("xiaowo_trade_reckless_scavenger"))
		{
			return;
		}
		PlayerStore instance = PlayerStore.Instance;
		if (instance != null)
		{
			_majorRollEligible = instance.dumpingGroundMajorWoundImmunitySearchesLeft <= 0;
			if (instance.dumpingGroundMinorWoundImmunitySearchesLeft <= 0)
			{
				_nextRoll = WoundRollPhase.Minor;
			}
			else if (_majorRollEligible)
			{
				_nextRoll = WoundRollPhase.Major;
			}
		}
	}

	internal static void BeginForTest(bool minorRollEligible, bool majorRollEligible)
	{
		End();
		_majorRollEligible = majorRollEligible;
		_nextRoll = (minorRollEligible ? WoundRollPhase.Minor : (majorRollEligible ? WoundRollPhase.Major : WoundRollPhase.None));
	}

	internal static float AdjustNativeWoundRoll(float value, float minimum, float maximum)
	{
		if (_nextRoll == WoundRollPhase.None || Math.Abs(minimum) > 0.001f || Math.Abs(maximum - 100f) > 0.001f)
		{
			return value;
		}
		value -= 15f;
		if (_nextRoll == WoundRollPhase.Minor && _majorRollEligible)
		{
			_nextRoll = WoundRollPhase.Major;
		}
		else
		{
			End();
		}
		return value;
	}

	internal static void End()
	{
		_nextRoll = WoundRollPhase.None;
		_majorRollEligible = false;
	}
}
