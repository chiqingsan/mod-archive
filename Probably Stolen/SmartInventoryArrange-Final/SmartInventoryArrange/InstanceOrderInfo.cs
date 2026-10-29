namespace SmartInventoryArrange;

public sealed record InstanceOrderInfo(string StableId, string ItemId, string CurrentName, int? WineState = null, int? AgeDays = null, long? VolumeUl = null, int? RemainingUses = null, int? RemainingEnergy = null, int? Durability = null, double? Purity = null, double? ChemicalContamination = null, double? PhysicalContamination = null)
{
	public string Profile { get; init; } = "name";


	public string Purpose { get; init; } = "";


	public bool IsUnidentified { get; init; }

	public bool? IsEmpty { get; init; }

	public int? QualityRank { get; init; }

	public int? FreshnessRank { get; init; }

	public int? EquipmentRank { get; init; }

	public int? Expired { get; init; }

	public int? Unavailable { get; init; }

	public int? Opened { get; init; }

	public int? Rusted { get; init; }

	public int? Dented { get; init; }

	public int? Counterfeit { get; init; }

	public int? FermentationRemaining { get; init; }

	public string Diagnostics { get; init; } = "";


	public string FeatureIntegrity { get; init; } = "";

}
