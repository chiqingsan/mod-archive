using System;

namespace XIAOWOTradePerks;

internal static class OwnerDealRules
{
	internal const int VisitIntervalDays = 7;

	internal const int MerchantCompleted = 1;

	internal const int LandlordCompleted = 2;

	internal const int AllVisitorsCompleted = 3;

	internal static bool CanOfferCashBuyout(bool perkActive, bool propertyPaid, bool payingRent, int cash, int price)
	{
		if (perkActive && !propertyPaid && payingRent && price > 0)
		{
			return cash >= price;
		}
		return false;
	}

	internal static bool IsFormerRentDay(int day)
	{
		if (day > 0)
		{
			return day % 7 == 0;
		}
		return false;
	}

	internal static int EncodeVisitState(int day, int completedMask)
	{
		return (Math.Max(0, day) << 2) | (completedMask & 3);
	}

	internal static int DecodeVisitState(int encoded, int day)
	{
		if (encoded >> 2 != Math.Max(0, day))
		{
			return 0;
		}
		return encoded & 3;
	}

	internal static bool NeedsVisitor(int completedMask, int visitorBit, bool present)
	{
		if ((completedMask & visitorBit) == 0)
		{
			return !present;
		}
		return false;
	}

	internal static OwnerStockRoll Roll(Random random)
	{
		bool restrictedNeural = random.Next(100) < 5;
		return new OwnerStockRoll(random.Next(3, 7), restrictedNeural);
	}

	internal static int GetVisitSeed(string runId, int day)
	{
		return StableHash((string.IsNullOrEmpty(runId) ? "no-run" : runId) + "|owner-deal|" + day);
	}

	internal static int StableHash(string value)
	{
		int num = -2128831035;
		for (int i = 0; i < value.Length; i++)
		{
			num = (num ^ value[i]) * 16777619;
		}
		return num;
	}
}
