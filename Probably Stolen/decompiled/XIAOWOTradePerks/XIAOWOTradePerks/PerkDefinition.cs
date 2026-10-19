using System;
using Il2Cpp;

namespace XIAOWOTradePerks;

internal sealed class PerkDefinition
{
	internal string Id { get; }

	internal string ChineseName { get; }

	internal string EnglishName { get; }

	internal string ChineseDescription { get; }

	internal string EnglishDescription { get; }

	internal string Name => ModLocalization.Select(ChineseName, EnglishName);

	internal string Description => ModLocalization.Select(ChineseDescription, EnglishDescription);

	internal int Cost { get; }

	internal StartingPerkType Type { get; }

	internal string IconFile { get; }

	internal int MaxSlot { get; }

	internal string[] IncompatibleIds { get; }

	internal PerkDefinition(string id, string chineseName, string englishName, string chineseDescription, string englishDescription, int cost, StartingPerkType type, string iconFile, int maxSlot = 0, params string[] incompatibleIds)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Id = id;
		ChineseName = chineseName;
		EnglishName = englishName;
		ChineseDescription = chineseDescription;
		EnglishDescription = englishDescription;
		Cost = cost;
		Type = type;
		IconFile = iconFile;
		MaxSlot = maxSlot;
		IncompatibleIds = incompatibleIds ?? Array.Empty<string>();
	}
}
