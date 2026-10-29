using System;

namespace SmartInventoryArrange;

public static class SemanticConditions
{
	public static int? Rank(string category, string id)
	{
		switch (category)
		{
		case "CATEGORY_INGOT_PURITY":
			return Index(id, "INGOT_PURITY_PERFECT", "INGOT_PURITY_VERY_HIGH", "INGOT_PURITY_HIGH", "INGOT_PURITY_FAIR", "INGOT_PURITY_LOW", "INGOT_PURITY_VERY_LOW");
		case "CATEGORY_FRESHNESS":
			return Index(id, "very_fresh", "fresh", "stale", "spoiled");
		case "CATEGORY_EQUIPMENT_CONDITION":
			return Index(id, "equipment_condition_new", "equipment_condition_excellent", "equipment_condition_good", "equipment_condition_fair", "equipment_condition_poor");
		case "CATEGORY_CHEMICAL_PURITY":
			return Index(id, "lab_pure", "pharma_grade", "near_pharma", "street_standard", "cut_product", "junk_grade", "starch_grade");
		case "CATEGORY_EXPIRED":
			return Index(id, "non_expired", "expired");
		case "CATEGORY_OPEN_BOX":
			return Index(id, "factory_sealed", "open_box");
		case "CATEGORY_RUSTED":
			return Index(id, "rust_free", "rusted");
		case "CATEGORY_DENTED":
			return Index(id, "non_dented", "dented");
		case "CATEGORY_REPLICA":
			return Index(id, "non_replica", "replica");
		case "CATEGORY_GENUINE_CIGARETTE":
			return Index(id, "genuineCigarette", "counterfeitCigarette");
		case "CATEGORY_GENUINE_STAMP":
			return Index(id, "genuineStamp", "counterfeitStamp");
		case "CATEGORY_GENUINE_INJECTOR":
		{
			int? result;
			switch (id)
			{
			default:
				result = null;
				break;
			case "counterfeitInjector":
			case "expiredInjector":
			case "unusableInjector":
				result = 1;
				break;
			case "genuineInjector":
				result = 0;
				break;
			}
			return result;
		}
		default:
			return null;
		}
	}

	private static int? Index(string id, params string[] values)
	{
		int num = Array.IndexOf<string>(values, id);
		if (num >= 0)
		{
			return num;
		}
		return null;
	}

	public static bool Relevant(string category)
	{
		switch (category)
		{
		default:
			return category == "CATEGORY_WINE_QUALITY";
		case "CATEGORY_INGOT_PURITY":
		case "CATEGORY_WATER_PURITY":
		case "CATEGORY_FRESHNESS":
		case "CATEGORY_EQUIPMENT_CONDITION":
		case "CATEGORY_CHEMICAL_PURITY":
		case "CATEGORY_EXPIRED":
		case "CATEGORY_OPEN_BOX":
		case "CATEGORY_RUSTED":
		case "CATEGORY_DENTED":
		case "CATEGORY_REPLICA":
		case "CATEGORY_GENUINE_CIGARETTE":
		case "CATEGORY_GENUINE_STAMP":
		case "CATEGORY_GENUINE_INJECTOR":
			return true;
		}
	}
}
