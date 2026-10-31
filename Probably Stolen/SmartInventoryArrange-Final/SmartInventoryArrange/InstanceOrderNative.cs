using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.Json;

namespace SmartInventoryArrange;

internal static class InstanceOrderNative
{
	private sealed record Condition(string Category, string Id, int Custom);

	private sealed class InstanceOrderCacheEntry
	{
		internal ulong Signature;

		internal InstanceOrderInfo Info = null!;
	}

	private static readonly ConditionalWeakTable<GameItem, InstanceOrderCacheEntry> InstanceOrderCache = new ConditionalWeakTable<GameItem, InstanceOrderCacheEntry>();

	private static TagState? Tag(GameItem item, string key)
	{
		TagSystem state = item.state;
		Dictionary<string, TagState> val = ((state != null) ? state.dict : null);
		if (val != null && val.ContainsKey(key))
		{
			return val[key];
		}
		return null;
	}

	private static int? Int(GameItem item, string key)
	{
		TagState? obj = Tag(item, key);
		if (obj == null)
		{
			return null;
		}
		return obj.valueInt;
	}

	private static string Text(GameItem item, string key)
	{
		TagState? obj = Tag(item, key);
		return ((obj != null) ? obj.valueString : null) ?? "";
	}

	private static bool Has(GameItem item, string key)
	{
		TagState? obj = Tag(item, key);
		if (obj == null)
		{
			return false;
		}
		return obj.valueEnabled;
	}

	private static double? Double(GameItem item, string key)
	{
		TagState val = Tag(item, key);
		if (val == null)
		{
			return null;
		}
		if (!double.IsFinite(val.valueDouble))
		{
			return null;
		}
		return val.valueDouble;
	}

	private static long? Volume(GameItem item)
	{
		if (!Has(item, "LIQUID_CONTAINER_TAG") && !Has(item, "CHEM_ITEM_TAG"))
		{
			return null;
		}
		List<Liquid> liquids = Liquid.Liquids;
		if (liquids == null)
		{
			return null;
		}
		int?[] array = new int?[liquids.Count];
		for (int i = 0; i < array.Length; i++)
		{
			Liquid val = liquids[i];
			if (val != null && !string.IsNullOrEmpty(val.currentPart))
			{
				array[i] = Int(item, val.currentPart);
			}
		}
		return InstanceOrder.SumVolume(array);
	}

	private static ValueTuple<Condition[], string> Features(GameItem item)
	{
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Invalid comparison between Unknown and I4
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected I4, but got Unknown
		List<Condition> list = new List<Condition>();
		List<string> list2 = new List<string>();
		List<ItemFeature> itemFeatures = item.itemFeatures;
		if (itemFeatures != null)
		{
			for (int i = 0; i < itemFeatures.Count; i++)
			{
				ItemFeature val = itemFeatures[i];
				if (val == null)
				{
					continue;
				}
				string text = val.category ?? "";
				if ((int)val.featureType != 0 && (int)val.featureType != 2)
				{
					continue;
				}
				bool flag = SemanticConditions.Relevant(text);
				bool flag2 = val.isExposable && text != "CATEGORY_FREE";
				if (!flag && !flag2)
				{
					continue;
				}
				bool discovered = val.IsFeatureDiscovered();
				bool exposed = val.IsExposed();
				ItemCondition realCondition = val.realCondition;
				ItemCondition fakeCondition = val.fakeCondition;
				list2.Add(JsonSerializer.Serialize(new
				{
					category = text,
					id = val.identifier,
					disabled = val.isDisabled,
					kind = (int)val.featureType,
					useCondition = val.useCondition,
					isExposable = val.isExposable,
					discovered = discovered,
					exposed = exposed,
					real = ((realCondition == null) ? null : new { realCondition.identifier, realCondition.category, realCondition.customValue, realCondition.compareValue, realCondition.display }),
					fake = ((fakeCondition == null) ? null : new { fakeCondition.identifier, fakeCondition.category, fakeCondition.customValue, fakeCondition.compareValue, fakeCondition.display })
				}));
				if (!val.isDisabled)
				{
					if (flag && val.useCondition && realCondition != null)
					{
						list.Add(new Condition(text, realCondition.identifier ?? "", realCondition.customValue));
					}
					else if (flag2)
					{
						list.Add(new Condition(text, "", 0));
					}
				}
			}
		}
		string text2 = HashCompat.Sha256Hex(JsonSerializer.SerializeToUtf8Bytes(Enumerable.OrderBy((IEnumerable<string>)list2, (string x) => x, (IComparer<string>?)StringComparer.Ordinal)));
		return new ValueTuple<Condition[], string>(Enumerable.ToArray(((IEnumerable<Condition>)list).OrderBy((Condition x) => x.Category, (IComparer<string>?)StringComparer.Ordinal).ThenBy((Condition x) => x.Id, (IComparer<string>?)StringComparer.Ordinal).ThenBy((Condition x) => x.Custom)), text2);
	}

	internal static InstanceOrderInfo Read(GameItem item, string stableId)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		GameItem item2 = item;
		ValueTuple<Condition[], string> val = Features(item2);
		Condition[] conditions = val.Item1;
		long? num = Volume(item2);
		bool flag = Has(item2, "LIQUID_CONTAINER_TAG");
		bool flag2 = StringComparer.Ordinal.Equals(item2.identifier, "wine_bottle");
		int? num2 = (flag2 ? Int(item2, "WINE_CONTAINER_STATE") : null);
		bool? flag4;
		if (flag2)
		{
			bool? flag3;
			switch (num2)
			{
			case 0:
				flag3 = true;
				break;
			case 1:
			case 2:
				flag3 = false;
				break;
			default:
				flag3 = null;
				break;
			}
			flag4 = flag3;
		}
		else
		{
			flag4 = ((flag && num.HasValue) ? new bool?(num.Value == 0) : null);
		}
		bool? isEmpty = flag4;
		string[] source = ((item2.itemTypes == null) ? Array.Empty<string>() : Enumerable.ToArray(Enumerable.Select(Enumerable.Range(0, item2.itemTypes.Count), (int k) => item2.itemTypes[k])));
		bool flag5 = Enumerable.Any(source, delegate(string t)
		{
			switch (t)
			{
			default:
				return t == "BACKPACK";
			case "WEAPON":
			case "FIREARM":
			case "MELEE_WEAPON":
			case "MODULE":
			case "MACHINE":
			case "MACHINERY":
			case "STORAGE":
				return true;
			}
		}) || Has(item2, "MODULE_TAG") || Has(item2, "STANDARD_MACHINE_TAG") || Has(item2, "machinery") || Has(item2, "BACKPACK_TAG") || Has(item2, "CONTAINER_TAG");
		bool flag6 = Enumerable.Any(conditions, (Condition c) => c.Category == "CATEGORY_WATER_PURITY");
		bool flag7 = Has(item2, "CHEMICAL_PRODUCT_TAG") || Enumerable.Any(conditions, (Condition c) => c.Category == "CATEGORY_CHEMICAL_PURITY");
		string text = (flag2 ? "wine" : (flag5 ? "condition" : (flag7 ? "chemical" : ((flag && (flag6 || Int(item2, "CHEMICAL_INTENT_TAG") == 1)) ? "water" : (flag ? "chemical" : ((Has(item2, "power_source_item") || Enumerable.Contains(source, "ENERGY_CELL") || Int(item2, "power_source_item_energy").HasValue) ? "energy" : ((Has(item2, "USE_COUNT_ITEM_TAG") || Int(item2, "CURRENT_USE_COUNT_INT").HasValue) ? "uses" : ((Tag(item2, "MAX_DURABILITY_TAG") != null || Int(item2, "DURABILITY_TAG").HasValue) ? "durability" : ((Enumerable.Any(conditions, delegate(Condition c)
		{
			if (c.Category == "CATEGORY_FRESHNESS")
			{
				string id = c.Id;
				if (!(id == "module_normale"))
				{
					return !(id == "module_stuck");
				}
				return false;
			}
			return false;
		}) || Enumerable.Contains(source, "FOOD")) ? "food" : ((item2.identifier == "metal_ingot" || Tag(item2, "INGOT_PURITY_TAG") != null) ? "ingot" : "name"))))))))));
		InstanceOrderInfo obj = new InstanceOrderInfo(stableId, item2.identifier, item2.GetDisplayName(true) ?? "")
		{
			Profile = text,
			IsUnidentified = false,
			IsEmpty = isEmpty,
			FeatureIntegrity = val.Item2
		};
		Condition? condition = Enumerable.FirstOrDefault(conditions, (Condition c) => c.Category == "CATEGORY_GENUINE_INJECTOR");
		int? expired = Rank("CATEGORY_EXPIRED");
		if (condition?.Id == "expiredInjector")
		{
			expired = 1;
		}
		int? unavailable = ((condition?.Id == "unusableInjector") ? new int?(1) : null);
		if (text == "condition")
		{
			int? num3 = Int(item2, "DURABILITY_TAG");
			if (num3.HasValue)
			{
				int valueOrDefault = num3.GetValueOrDefault();
				unavailable = ((valueOrDefault <= 0) ? 1 : 0);
			}
		}
		if (Enumerable.Any(conditions, (Condition c) => c.Id == "module_stuck"))
		{
			unavailable = 1;
		}
		else if (Enumerable.Any(conditions, (Condition c) => c.Id == "module_normale"))
		{
			unavailable = 0;
		}
		int? counterfeit = Rank("CATEGORY_GENUINE_CIGARETTE") ?? Rank("CATEGORY_GENUINE_STAMP") ?? Rank("CATEGORY_GENUINE_INJECTOR") ?? Rank("CATEGORY_REPLICA");
		int? qualityRank = null;
		string text2 = "appraisal-ignored:real-state";
		switch (text)
		{
		case "water":
		{
			long? num4 = Int(item2, "CURRENT_PART_WATER");
			if (num4.HasValue && num > 0)
			{
				long num5 = Math.Clamp(num4.Value * 10000 / num.Value, 0L, 10000L);
				qualityRank = WaterFeatureHelper.GetPurityArrayIndex((int)num5);
			}
			else if (num != 0)
			{
				text2 += ";missing-water-composition";
			}
			break;
		}
		case "chemical":
			qualityRank = Rank("CATEGORY_CHEMICAL_PURITY");
			break;
		case "ingot":
			qualityRank = Rank("CATEGORY_INGOT_PURITY") ?? SemanticConditions.Rank("CATEGORY_INGOT_PURITY", Text(item2, "INGOT_PURITY_TAG"));
			break;
		}
		string text3 = Text(item2, "ACTIVATE_INGREDIENT_TAG");
		int? num6 = Int(item2, "CHEMICAL_INTENT_TAG");
		if (text == "food" && !Rank("CATEGORY_FRESHNESS").HasValue)
		{
			text2 += ";unmapped-freshness: native condition absent; no tag direction guessed";
		}
		string purpose = ((!(text == "chemical") && !(text == "water")) ? "" : ((text3.Length > 0) ? text3 : (num6.HasValue ? num6.Value.ToString("D10", (IFormatProvider)CultureInfo.InvariantCulture) : "")));
		return obj with
		{
			Purpose = purpose,
			WineState = num2,
			AgeDays = (flag2 ? Int(item2, "AGE_INT") : null),
			FermentationRemaining = (flag2 ? Int(item2, "FERMENTATION_TIME_REMAINING_INT") : null),
			VolumeUl = num,
			RemainingUses = ((text == "uses") ? Int(item2, "CURRENT_USE_COUNT_INT") : null),
			RemainingEnergy = ((text == "energy") ? Int(item2, "power_source_item_energy") : null),
			Durability = ((text == "durability") ? Int(item2, "DURABILITY_TAG") : null),
			Purity = ((text == "chemical") ? Double(item2, "CHEMICAL_PRODUCT_PURITY_TAG") : null),
			QualityRank = qualityRank,
			FreshnessRank = Rank("CATEGORY_FRESHNESS"),
			EquipmentRank = Rank("CATEGORY_EQUIPMENT_CONDITION"),
			Expired = expired,
			Unavailable = unavailable,
			Opened = Rank("CATEGORY_OPEN_BOX"),
			Rusted = Rank("CATEGORY_RUSTED"),
			Dented = Rank("CATEGORY_DENTED"),
			Counterfeit = counterfeit,
			Diagnostics = text2
		};
		int? Rank(string category)
		{
			string category2 = category;
			Condition[] array = Enumerable.ToArray(Enumerable.Where(conditions, (Condition c) => c.Category == category2));
			if (array.Length == 0)
			{
				return null;
			}
			int?[] source2 = Enumerable.ToArray(Enumerable.Select(array, (Condition c) => SemanticConditions.Rank(c.Category, c.Id)));
			if (!Enumerable.All(source2, (int? r) => r.HasValue))
			{
				return null;
			}
			return Enumerable.Max(source2);
		}
	}

	private static string StateIntegrity(GameItem item)
	{
		List<string> list = new List<string>();
		TagSystem state = item.state;
		Dictionary<string, TagState> val = ((state != null) ? state.dict : null);
		if (val != null)
		{
			var enumerator = val.Keys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string current = enumerator.Current;
				TagState val2 = val[current];
				list.Add((val2 == null) ? JsonSerializer.Serialize(new
				{
					key = current,
					missing = true
				}) : JsonSerializer.Serialize(new
				{
					key = current,
					identifier = val2.identifier,
					valueEnabled = val2.valueEnabled,
					valueInt = val2.valueInt,
					valueLong = val2.valueLong,
					floatBits = HashCompat.SingleBits(val2.valueFloat),
					doubleBits = HashCompat.DoubleBits(val2.valueDouble),
					valueBool = val2.valueBool,
					internalValueString = val2.valueString
				}));
			}
		}
		return HashCompat.Sha256Hex(JsonSerializer.SerializeToUtf8Bytes(Enumerable.OrderBy((IEnumerable<string>)list, (string x) => x, (IComparer<string>?)StringComparer.Ordinal)));
	}

	internal static ulong Signature(GameItem item)
	{
		ulong hash = 14695981039346656037UL;
		hash = MixText(hash, item.identifier);
		hash = MixValue(hash, (ulong)(uint)item.uniqueId);
		hash = MixValue(hash, (ulong)(uint)item.unitCount);
		hash = MixText(hash, item.GetDisplayName(true));
		TagSystem state = item.state;
		Dictionary<string, TagState> val = ((state != null) ? state.dict : null);
		ulong tagHash = 0UL;
		int tagCount = 0;
		if (val != null)
		{
			foreach (KeyValuePair<string, TagState> item2 in val)
			{
				tagHash ^= HashTag(item2.Key, item2.Value);
				tagCount++;
			}
		}
		hash = MixValue(hash, tagHash);
		hash = MixValue(hash, (ulong)(uint)tagCount);
		List<ItemFeature> itemFeatures = item.itemFeatures;
		ulong featureHash = 0UL;
		int featureCount = 0;
		if (itemFeatures != null)
		{
			for (int i = 0; i < itemFeatures.Count; i++)
			{
				ItemFeature val2 = itemFeatures[i];
				featureHash ^= ((val2 == null) ? 0x9E3779B97F4A7C15UL : HashFeature(val2));
				featureCount++;
			}
		}
		hash = MixValue(hash, featureHash);
		hash = MixValue(hash, (ulong)(uint)featureCount);
		List<string> itemTypes = item.itemTypes;
		if (itemTypes == null)
		{
			hash = MixValue(hash, 0UL);
		}
		else
		{
			hash = MixValue(hash, (ulong)(uint)itemTypes.Count);
			for (int j = 0; j < itemTypes.Count; j++)
			{
				hash = MixText(hash, itemTypes[j]);
			}
		}
		return hash;
	}

	private static ulong MixValue(ulong hash, ulong value)
	{
		hash ^= value + 0x9E3779B97F4A7C15UL + (hash << 6) + (hash >> 2);
		return hash;
	}

	private static ulong MixText(ulong hash, string? text)
	{
		if (text == null)
		{
			return MixValue(hash, 0xFFFFFFFFFFFFFFFFUL);
		}
		hash = MixValue(hash, (ulong)(uint)text.Length);
		for (int i = 0; i < text.Length; i++)
		{
			hash = MixValue(hash, text[i]);
		}
		return hash;
	}

	private static ulong HashTag(string key, TagState tag)
	{
		ulong hash = MixText(14695981039346656037UL, key);
		if (tag == null)
		{
			return MixValue(hash, 1UL);
		}
		hash = MixText(hash, tag.identifier);
		hash = MixText(hash, tag.identifierName);
		hash = MixValue(hash, tag.valueEnabled ? 1UL : 0UL);
		hash = MixValue(hash, (ulong)(uint)tag.valueInt);
		hash = MixValue(hash, (ulong)tag.valueLong);
		hash = MixValue(hash, (ulong)(uint)HashCompat.SingleBits(tag.valueFloat));
		hash = MixValue(hash, (ulong)HashCompat.DoubleBits(tag.valueDouble));
		hash = MixValue(hash, tag.valueBool ? 1UL : 0UL);
		return MixText(hash, tag.valueString);
	}

	private static ulong HashFeature(ItemFeature feature)
	{
		ulong hash = 14695981039346656037UL;
		hash = MixText(hash, feature.category);
		hash = MixText(hash, feature.identifier);
		hash = MixValue(hash, feature.isDisabled ? 1UL : 0UL);
		hash = MixValue(hash, (ulong)(uint)(int)feature.featureType);
		hash = MixValue(hash, (ulong)(uint)(int)feature.valueStage);
		hash = MixValue(hash, (ulong)(uint)feature.parentItemUniqueId);
		hash = MixValue(hash, feature.isExposing ? 1UL : 0UL);
		hash = MixValue(hash, feature.isPublicHidden ? 1UL : 0UL);
		hash = MixValue(hash, (ulong)(uint)feature.customIntValue1);
		hash = MixValue(hash, feature.isFeatureMatch ? 1UL : 0UL);
		hash = MixValue(hash, feature.useCondition ? 1UL : 0UL);
		hash = MixValue(hash, feature.isExposable ? 1UL : 0UL);
		hash = MixValue(hash, feature.IsInitiallyShown() ? 1UL : 0UL);
		hash = MixValue(hash, feature.IsFeatureDiscovered() ? 1UL : 0UL);
		hash = MixValue(hash, feature.IsExposed() ? 1UL : 0UL);
		ItemCondition realCondition = feature.realCondition;
		ItemCondition fakeCondition = feature.fakeCondition;
		int actualModifier = (feature.useCondition ? ((realCondition == null) ? 0 : realCondition.modValue) : feature.GetActualValueModifier());
		int fakeModifier = (feature.useCondition ? ((fakeCondition == null) ? 0 : fakeCondition.modValue) : feature.GetFakeValueModifier());
		hash = MixValue(hash, (ulong)(uint)actualModifier);
		hash = MixValue(hash, (ulong)(uint)fakeModifier);
		hash = MixText(hash, feature.useCondition ? fakeCondition?.display : feature.GetPublicDisplay());
		hash = MixText(hash, feature.useCondition ? realCondition?.display : feature.GetActualDisplay());
		hash = MixCondition(hash, realCondition);
		hash = MixCondition(hash, fakeCondition);
		return hash;
	}

	private static ulong MixCondition(ulong hash, ItemCondition condition)
	{
		if (condition == null)
		{
			return MixValue(hash, 0UL);
		}
		hash = MixValue(hash, 1UL);
		hash = MixText(hash, condition.identifier);
		hash = MixText(hash, condition.category);
		hash = MixText(hash, Convert.ToString(condition.customValue, CultureInfo.InvariantCulture));
		hash = MixText(hash, Convert.ToString(condition.compareValue, CultureInfo.InvariantCulture));
		hash = MixText(hash, Convert.ToString(condition.display, CultureInfo.InvariantCulture));
		return hash;
	}

	internal static InstanceOrderInfo ReadCached(GameItem item, string stableId)
	{
		ulong signature = Signature(item);
		if (InstanceOrderCache.TryGetValue(item, out InstanceOrderCacheEntry? cached) && cached != null && cached.Signature == signature && string.Equals(cached.Info.StableId, stableId, StringComparison.Ordinal))
		{
			PerfTrace.Increment("instanceOrderHits");
			return cached.Info;
		}
		InstanceOrderInfo info = Read(item, stableId);
		InstanceOrderCache.Remove(item);
		InstanceOrderCache.Add(item, new InstanceOrderCacheEntry
		{
			Signature = signature,
			Info = info
		});
		PerfTrace.Increment("instanceOrderMisses");
		return info;
	}

	internal static string FingerprintSuffix(GameItem item)
	{
		return JsonSerializer.Serialize(Read(item, ""));
	}
}
