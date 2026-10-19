namespace XIAOWOTradePerks;

internal readonly struct OwnerStockRoll
{
	internal int Count { get; }

	internal bool RestrictedNeural { get; }

	internal OwnerStockRoll(int count, bool restrictedNeural)
	{
		Count = count;
		RestrictedNeural = restrictedNeural;
	}
}
