namespace WaterTower;

public static class WaterTowerConfig
{
	public const string ModVersion = "1.0.5";

	public const int TankCapacityMl = 600000;

	public const int FootprintWidth = 3;

	public const int FootprintHeight = 4;

	public const int ZoneWidth = 16;

	public const int ZoneHeight = 16;

	public const string IdWaterTower = "watertower";

	public const string IdWaterTank = "watertower_tank";

	public const string IdNote = "postit";

	public const string IdMoistureFarm = "moisture_farm";

	public const string IdWaterPurifier = "water_purifier";

	public const string IdRecharger = "recharger";

	public const string IdPowerBloc = "powerblock";

	public const string IdEnergyCell = "energy_credit";

	public const string IdEnergyCellExt = "energy_credit_ext";

	public const string IdEnergyCellBreeder = "energy_credit_breeder";

	public static readonly string[] ZoneAllowedIds = new string[3] { "moisture_farm", "water_purifier", "recharger" };

	public const string TagImportant = "IMPORTANT_TAG";

	public const string TagLiquidContainer = "LIQUID_CONTAINER_TAG";

	public const string TagOwned = "IS_OWNED_TAG";

	public const string ItemTypeStorage = "STORAGE";

	public const string TagSystem = "SYSTEM_TAG";

	public const string TagSystemUtility = "SYSTEM_TAG_UTILITY";

	public const string ItemTypeDocument = "DOCUMENT";

	public const string TagZoneCatchment = "WATERTOWER_ZONE_CATCHMENT";

	public const string TagZonePurify = "WATERTOWER_ZONE_PURIFY";

	public const string TagZonePower = "WATERTOWER_ZONE_POWER";

	public static readonly string[] ZoneTags = new string[3] { "WATERTOWER_ZONE_CATCHMENT", "WATERTOWER_ZONE_PURIFY", "WATERTOWER_ZONE_POWER" };

	public static readonly string[][] ZoneTagByItemId = new string[4][]
	{
		new string[2] { "moisture_farm", "WATERTOWER_ZONE_CATCHMENT" },
		new string[2] { "water_purifier", "WATERTOWER_ZONE_PURIFY" },
		new string[2] { "powerblock", "WATERTOWER_ZONE_POWER" },
		new string[2] { "recharger", "WATERTOWER_ZONE_POWER" }
	};

	public static readonly string[] ZoneTagStripByItemId = new string[3] { "energy_credit", "energy_credit_ext", "energy_credit_breeder" };

	public static readonly string[] ZoneTagsAll = new string[3] { "WATERTOWER_ZONE_CATCHMENT", "WATERTOWER_ZONE_PURIFY", "WATERTOWER_ZONE_POWER" };

	public static readonly bool NightSettlementEnabled = true;

	public const int NightSettlementMaxMachines = 64;

	public const string WindowTitle = "水塔";

	public const string LabelZone = "工作区";

	public const string LabelTank = "蓄水箱";

	public const string LabelOutputSlot = "取水槽";

	public const string LabelNote = "备注";

	public const string FillButtonText = "取水";

	public const string TowerDisplayName = "水塔";

	public const string TankDisplayName = "水塔蓄水箱（600L）";

	public const int TowerValue = 1000;

	public const string TowerFlavorText = "水塔，用于蓄水，您不再需要每日从集水器中重复取水。";

	public const string NoteAtlas = "Items/items_tool";

	public const string NoteSprite = "postit";

	public const string NoteDisplayName = "水塔";

	public const string NoteFlavor = "水塔";

	public const string NoteDescription = "1.\u00a0工作区内需正常放置集水器或净水器，以及能量阵列。集水器和净水器内需安装电池，建议不放置储水容器。 2.\u00a0集水器或净水器内，若放置储水容器，则产物不会进入水塔的蓄水箱。 3.\u00a0集水器或净水器内，若不放置储水容器，则产物会直接进入水塔的蓄水箱。";

	public static readonly string[] SpriteFileNames = new string[2] { "WaterTower.png", "水塔.png" };

	public const string ModAtlasPath = "Items/watertower_mod";

	public const string ModSpriteName = "water_tower";

	public const float WorldUnitsPerCell = 0.16f;

	public const int SpritePixelsPerCell = 16;

	public const string TankAtlas = "Items/water_container4";

	public const string TankSprite = "xl";

	public const float SlotBgOpacity = 0.5f;

	public const float SlotBgLightness = 0.5f;

	public const string PlaceholderAtlas = "Items/items_tool2";

	public const string PlaceholderSprite = "machine_bay0";

	public const int JacksonSellStartDay = 15;

	public const int JacksonSellChanceOwnedZero = 100;

	public const int JacksonSellChanceOwnedOne = 25;

	public const int JacksonMaxOwnedTowers = 2;

	public const int JacksonSellCooldownMs = 2000;

	public static string TowerStatusText(int currentMl, int capacityMl, int purity)
	{
		double num = (double)currentMl / 1000.0;
		double num2 = (double)capacityMl / 1000.0;
		return "蓄水箱：" + num.ToString("0.#") + " L / " + num2.ToString("0.#") + " L";
	}
}
