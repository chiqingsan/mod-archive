using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace WaterMerchant;

public sealed class Mod : IMod
{

    public void Init(ModManifest manifest) => Settings.Init(manifest);
    public void OnEnable()
    {
        Settings.Active = true;
        if (EmporiumEntry.Instance != null) Hooks.RefreshOwned();
        Debug.Log("WaterMerchant 1.1.0 enabled");
    }
    public void OnDisable()
    {
        Settings.Active = false;
        Recipes.Set(false);
        if (EmporiumEntry.Instance != null) Hooks.RestoreOwned();
    }

}

internal static class Settings
{
    internal static bool Active;
    internal static ConfigEntry<bool> FarmOutput = null!, FarmPower = null!, FarmAir = null!, FarmPrice = null!, FilterMicrobe = null!, FilterZero = null!, UvPitcher = null!, PurifierAmount = null!, PurifierQuality = null!, PortableCapacity = null!, PortableMix = null!, Recipes = null!;
    internal static void Init(ModManifest manifest)
    {
        var config = ModConfig.For(manifest.ID);
        FarmOutput = config.Define("FarmOutput", true, "集水器", () => "基础产量 1000 ml");
        FarmPower = config.Define("FarmPower", true, "集水器", () => "基础耗电 4");
        FarmAir = config.Define("FarmAir", true, "集水器", () => "取消多台产量削减");
        FarmPrice = config.Define("FarmPrice", true, "集水器", () => "基础价值降至 225");
        FilterMicrobe = config.Define("FilterMicrobe", true, "过滤", () => "滤芯过滤微生物");
        FilterZero = config.Define("FilterZero", true, "过滤", () => "滤芯过滤残留降至零");
        UvPitcher = config.Define("UvPitcher", true, "过滤", () => "紫外线灯可放入过滤壶");
        PurifierAmount = config.Define("PurifierAmount", true, "净水器", () => "纯度 96% 及以上时净化倍率为 1.5");
        PurifierQuality = config.Define("PurifierQuality", true, "净水器", () => "品质 150% 时最高纯度为 100%");
        PortableCapacity = config.Define("PortableCapacity", true, "海德拉", () => "耐久上限 400", () => "关闭时已超过 300 的耐久暂时保留，直到耐久不超过 300 后恢复原上限。");
        PortableMix = config.Define("PortableMix", true, "海德拉", () => "按比例减少各类杂质");
        Recipes = config.Define("WaterRecipes", true, "水质", () => "水中杂质配比保持修改前标准");
        foreach (var entry in new[] { FarmPower, FarmAir, FarmPrice, UvPitcher, PortableCapacity }) entry.OnChanged += _ => { if (Active && EmporiumEntry.Instance != null) Hooks.RefreshOwned(); };
        Recipes.OnChanged += enabled => { if (Active && !enabled) WaterMerchant.Recipes.Set(false); };
    }
}

internal static class Recipes
{
    private static readonly List<(Dictionary<string, float> recipe, Dictionary<string, float> original)> saved = new();
    private static readonly string[] ids = { "organic_waste", "microbe", "physical_contaminant", "chemical_contaminant", "mineral", "heavy_metal" };
    public static void Ensure() { if (Settings.Recipes.Value && saved.Count == 0) Set(true); }
    public static void Set(bool enabled)
    {
        if (!enabled)
        {
            foreach (var (recipe, original) in saved) { recipe.Clear(); foreach (var pair in original) recipe.Add(pair.Key, pair.Value); }
            saved.Clear();
            return;
        }
        var all = new[] { WaterPremadeHelper.waterContaminantDistribution, WaterPremadeHelper.rustWaterContaminantDistribution, WaterPremadeHelper.gutterflowContaminantDistribution, WaterPremadeHelper.baseWaterContaminantDistribution };
        foreach (var recipe in all)
        {
            saved.Add((recipe, new Dictionary<string, float>(recipe)));
            float[] values = ReferenceEquals(recipe, WaterPremadeHelper.baseWaterContaminantDistribution) ? new[] { .2f, .2f, .1f, .2f, .15f, .15f } : new[] { .2f, .25f, .1f, .2f, .15f, .1f };
            recipe.Clear();
            for (int i = 0; i < ids.Length; i++) recipe.Add(Liquid.GetCurrentPartFromId(ids[i]), values[i]);
        }
    }
}

internal static class Hooks
{
    public static void ApplyFarmCreated(GameItem item) { item.identifier = "moisture_farm"; Apply(item); }
    public static void RefreshOwned() { foreach (var item in EmporiumEntry.Instance.GetAllOwnedItems()) Apply(item); }
    public static void RestoreOwned() { foreach (var item in EmporiumEntry.Instance.GetAllOwnedItems()) Restore(item); }
    public static void Apply(GameItem item)
    {
        if (item.identifier == "moisture_farm")
        {
            SetFarmPower(item, Settings.FarmPower.Value ? 4 : 6);
            item.shortDescription = LocHelper.GetLocalizedItem("item_moisture_farm_desc") + (Settings.FarmAir.Value ? "" : "\n" + LocHelper.GetLocalizedItem("item_moisture_farm_desc_shared_air"));
            if (Settings.FarmPrice.Value && item.unitValue > 225 && !item.IsTag("WM_ORIGINAL_FARM_VALUE"))
            {
                long original = item.unitValue;
                item.ModifyTag("WM_ORIGINAL_FARM_VALUE", t => { t.Enable(); t.valueLong = original; });
                item.unitValue = 225;
            }
            else if (!Settings.FarmPrice.Value && item.IsTag("WM_ORIGINAL_FARM_VALUE"))
            {
                item.unitValue = item.GetTagReadonly("WM_ORIGINAL_FARM_VALUE").valueLong;
                item.DisableTag("WM_ORIGINAL_FARM_VALUE");
            }
        }
        else if (item.identifier == "portable_water_purifier")
        {
            int max = DurabilityHelper.GetMaxDurability(item);
            if (Settings.PortableCapacity.Value && max == 300)
            {
                item.ModifyTag("MAX_DURABILITY_TAG", t => t.SetInt(400));
                item.ModifyTag("DEFAULT_DURABILITY_TAG", t => t.SetInt(400));
                item.EnableTag("WM_PORTABLE_CAPACITY");
            }
            else if (!Settings.PortableCapacity.Value && max == 400 && DurabilityHelper.GetRealDurability(item) <= 300 && DurabilityHelper.GetFakeDurability(item) <= 300)
            {
                item.ModifyTag("MAX_DURABILITY_TAG", t => t.SetInt(300));
                item.ModifyTag("DEFAULT_DURABILITY_TAG", t => t.SetInt(300));
                item.DisableTag("WM_PORTABLE_CAPACITY");
            }
        }
        else if (item.identifier == "water_pitcher") SetPitcher(item, Settings.UvPitcher.Value);
    }
    public static void Restore(GameItem item)
    {
        if (item.identifier == "moisture_farm")
        {
            SetFarmPower(item, 6);
            item.shortDescription = LocHelper.GetLocalizedItem("item_moisture_farm_desc") + "\n" + LocHelper.GetLocalizedItem("item_moisture_farm_desc_shared_air");
            if (item.IsTag("WM_ORIGINAL_FARM_VALUE")) { item.unitValue = item.GetTagReadonly("WM_ORIGINAL_FARM_VALUE").valueLong; item.DisableTag("WM_ORIGINAL_FARM_VALUE"); }
        }
        else if (item.identifier == "water_pitcher") SetPitcher(item, false);
    }
    private static void SetFarmPower(GameItem item, int basis)
    {
        int efficiency = item.GetTagReadonly("TOTAL_PERCENTAGE_POWER_BONUS_INT").GetInt();
        item.ModifyTag("MACHINERY_DRAWN_POWER_BASE", s => s.SetInt(basis));
        item.ModifyTag("machinery_drawn_power", s => s.SetInt(Math.Max(1, basis - basis * efficiency / 100)));
    }
    private static void SetPitcher(GameItem item, bool allowUv)
    {
        var slot = (GameSlotInventory)item.contentWindow.childElement;
        slot.mayInventoryAddItemFunc = (candidate, _) => GeneralHelper.IsItemOwned(candidate) && candidate.IsTag("WATER_FILTER") && (allowUv || candidate.identifier != "uv_filter");
    }
}

[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.LoadGame))]
internal static class Loaded { private static void Postfix() => Hooks.RefreshOwned(); }
[HarmonyPatch(typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.MoistureFarm))]
internal static class FarmCreated { private static void Postfix(GameItem __result) => Hooks.ApplyFarmCreated(__result); }
[HarmonyPatch(typeof(AmenitiesItemDirectory), "WaterPitcher")]
internal static class PitcherCreated { private static void Postfix(GameItem __result) => Hooks.Apply(__result); }
[HarmonyPatch(typeof(AmenitiesItemDirectory), "PortableWaterPurifier")]
internal static class PortableCreated { private static void Postfix(GameItem __result) { if (Settings.PortableCapacity.Value) DurabilityHelper.SetDurability(__result, 400, 400, 400); } }
[HarmonyPatch(typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.GetOutputVolume))]
internal static class FarmOutput
{
    private static bool Prefix(GameItem gameItem, ref int __result) { if (!Settings.FarmOutput.Value) return true; __result = Math.Max(1000000 * (100 + MachineryHelper.GetCurrentPerformanceBonus(gameItem)) / 100, 0); return false; }
}
[HarmonyPatch(typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.ApplySharedAir))]
internal static class FarmAir { private static bool Prefix(int volume, ref int __result) { if (!Settings.FarmAir.Value) return true; __result = volume; return false; } }
[HarmonyPatch(typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.GetSharingFarmCount))]
internal static class FarmTooltip { private static bool Prefix(ref int __result) { if (!Settings.FarmAir.Value) return true; __result = 1; return false; } }
[HarmonyPatch(typeof(MachinePurifier), nameof(MachinePurifier.GetPurityMult))]
internal static class PurifierQuality { private static bool Prefix(int bonusQuality, ref double __result) { if (!Settings.PurifierQuality.Value || bonusQuality < 150) return true; __result = 0; return false; } }

[HarmonyPatch(typeof(MachinePurifier), nameof(MachinePurifier.GetDirtBonus))]
internal static class PurifierAmount
{
    private static bool Prefix(int purity, ref double __result)
    {
        if (!Settings.PurifierAmount.Value || purity < 9600) return true;
        __result = 1.5;
        return false;
    }
}

[HarmonyPatch(typeof(WaterHelper), nameof(WaterHelper.HandleFilter))]
internal static class Filter
{
    private static bool Prefix(string identifier, int amount, GameItem filter, ref int __result)
    {
        if (!Settings.FilterZero.Value && (!Settings.FilterMicrobe.Value || identifier != "microbe")) return true;
        if (identifier == "microbe" && !Settings.FilterMicrobe.Value) return true;
        if (amount <= 0 || filter == null || filter.identifier == "uv_filter" || !filter.IsTag("WATER_FILTER")) return true;
        var liquid = Liquid.GetLiquidFromId(identifier);
        if (liquid == null || filter.GetTagReadonly("FILTER_SIZE_TAG").GetInt() > liquid.particleSize) { __result = amount; return false; }
        int durability = DurabilityHelper.GetRealDurability(filter);
        int step = identifier == "microbe" ? 1000 : liquid.filterVolumeRemoved;
        int wear = identifier == "microbe" ? 1 : liquid.filterDurabilityUsage;
        if (step <= 0) throw new InvalidOperationException("Invalid filter volume for " + identifier);
        int original = amount;
        int minimum = Settings.FilterZero.Value ? 0 : (int)(amount * liquid.filterMinimumPurity);
        int count = 0;
        while (durability > 0 && amount > minimum && count < 10000)
        {
            DurabilityHelper.ChangeDurability(filter, -wear);
            durability = DurabilityHelper.GetRealDurability(filter);
            amount -= step;
            count++;
        }
        __result = Math.Max(minimum, Math.Min(original, amount));
        return false;
    }
}
[HarmonyPatch(typeof(WaterHelper), nameof(WaterHelper.PurifyToBaseWater))]
internal static class PortablePurify
{
    private static bool Prefix(GameItem container)
    {
        if (!Settings.PortableMix.Value) return true;
        if (!container.IsTag("LIQUID_CONTAINER_TAG")) return false;
        int total = WaterHelper.GetTotalVolume(container);
        if (total <= 0) return false;
        string water = Liquid.GetCurrentPartFromId("water");
        int current = container.GetTagReadonly(water).GetInt();
        int target = (int)(((long)total * 9800 + 9999) / 10000);
        if (current >= target) return false;
        int contaminants = total - current;
        int remainingTarget = total - target;
        int remaining = 0;
        foreach (var liquid in Liquid.Liquids)
        {
            if (liquid.identifier == "water") continue;
            int old = container.GetTagReadonly(liquid.currentPart).GetInt();
            if (old <= 0) continue;
            int next = (int)((long)old * remainingTarget / contaminants);
            container.ModifyTag(liquid.currentPart, s => s.Enable().SetInt(next));
            remaining += next;
        }
        container.ModifyTag(water, s => s.Enable().SetInt(total - remaining));
        WaterFeatureHelper.SetPresumedPurity(container, WaterHelper.GetWaterPurity(container));
        return false;
    }
}


[HarmonyPatch(typeof(WaterHelper), nameof(WaterHelper.AddWater))]
internal static class RecipeInit { private static void Prefix() => Recipes.Ensure(); }
[HarmonyPatch(typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.CountRunningFarms))]
internal static class FarmCount { private static bool Prefix(ref int __result) { if (!Settings.FarmAir.Value) return true; __result = 1; return false; } }


[HarmonyPatch(typeof(DurabilityHelper), nameof(DurabilityHelper.ChangeDurability))]
internal static class PortableCapacityAfterUse
{
    private static void Postfix(GameItem gameItem)
    {
        if (!Settings.PortableCapacity.Value && gameItem.identifier == "portable_water_purifier" && DurabilityHelper.GetMaxDurability(gameItem) == 400 && DurabilityHelper.GetRealDurability(gameItem) <= 300 && DurabilityHelper.GetFakeDurability(gameItem) <= 300)
            Hooks.Apply(gameItem);
    }
}

