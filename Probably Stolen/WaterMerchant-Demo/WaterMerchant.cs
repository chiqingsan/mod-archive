using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace WaterMerchant;

public sealed class Mod : IMod
{
    private const string HarmonyId = "chiqingsan.WaterMerchant";

    private GameObject? _runner;
    private Harmony? _harmony;
    private bool _patchAttempted;

    public void Init(ModManifest manifest) => Settings.Init(manifest);
    public void OnEnable()
    {
        Settings.Active = true;
        _patchAttempted = false;
        _runner = new GameObject("WaterMerchant");
        UnityEngine.Object.DontDestroyOnLoad(_runner);
        _runner.AddComponent<WaterMerchantRunner>().Mod = this;
        Debug.Log("WaterMerchant 1.1.1 enabled");
    }

    internal void OnUpdate()
    {
        if (!_patchAttempted)
        {
            if (!PlayerStore.IsInstanceExist() || PlayerStore.Instance == null || EmporiumEntry.Instance == null)
                return;

            _patchAttempted = true;
            InstallPatches();
            if (_harmony == null)
                return;
            Hooks.RequestRefresh();
        }

        if (_harmony != null)
            Hooks.UpdatePendingRefresh();
    }

    private void InstallPatches()
    {
        try
        {
            var harmony = _harmony = new Harmony(HarmonyId);
            PatchPostfix(harmony, typeof(PlayerStore), nameof(PlayerStore.LoadGame), Type.EmptyTypes, typeof(Loaded), nameof(Loaded.Postfix));
            PatchPostfix(harmony, typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.MoistureFarm), Type.EmptyTypes, typeof(FarmCreated), nameof(FarmCreated.Postfix));
            PatchPostfix(harmony, typeof(AmenitiesItemDirectory), "WaterPitcher", Type.EmptyTypes, typeof(PitcherCreated), nameof(PitcherCreated.Postfix));
            PatchPostfix(harmony, typeof(AmenitiesItemDirectory), "PortableWaterPurifier", Type.EmptyTypes, typeof(PortableCreated), nameof(PortableCreated.Postfix));
            PatchPrefix(harmony, typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.GetOutputVolume), new[] { typeof(GameItem) }, typeof(FarmOutput), nameof(FarmOutput.Prefix));
            PatchPrefix(harmony, typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.ApplySharedAir), new[] { typeof(int), typeof(int) }, typeof(FarmAir), nameof(FarmAir.Prefix));
            PatchPrefix(harmony, typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.GetSharingFarmCount), new[] { typeof(GameItem) }, typeof(FarmTooltip), nameof(FarmTooltip.Prefix));
            PatchPrefix(harmony, typeof(MachinePurifier), nameof(MachinePurifier.GetPurityMult), new[] { typeof(int) }, typeof(PurifierQuality), nameof(PurifierQuality.Prefix));
            PatchPrefix(harmony, typeof(MachinePurifier), nameof(MachinePurifier.GetDirtBonus), new[] { typeof(int) }, typeof(PurifierAmount), nameof(PurifierAmount.Prefix));
            PatchPrefix(harmony, typeof(WaterHelper), nameof(WaterHelper.HandleFilter), new[] { typeof(string), typeof(int), typeof(GameItem), typeof(GameItem) }, typeof(Filter), nameof(Filter.Prefix));
            PatchPrefix(harmony, typeof(WaterHelper), nameof(WaterHelper.PurifyToBaseWater), new[] { typeof(GameItem) }, typeof(PortablePurify), nameof(PortablePurify.Prefix));
            PatchPrefix(harmony, typeof(WaterHelper), nameof(WaterHelper.AddWater), new[] { typeof(GameItem), typeof(int), typeof(int), typeof(bool), typeof(int), typeof(int), typeof(bool) }, typeof(RecipeInit), nameof(RecipeInit.Prefix));
            PatchPrefix(harmony, typeof(MachineMoistureFarm), nameof(MachineMoistureFarm.CountRunningFarms), Type.EmptyTypes, typeof(FarmCount), nameof(FarmCount.Prefix));
            PatchPostfix(harmony, typeof(DurabilityHelper), nameof(DurabilityHelper.ChangeDurability), new[] { typeof(GameItem), typeof(int) }, typeof(PortableCapacityAfterUse), nameof(PortableCapacityAfterUse.Postfix));
            Debug.Log("WaterMerchant patches installed after store readiness.");
        }
        catch (Exception ex)
        {
            _harmony?.UnpatchAll(HarmonyId);
            _harmony = null;
            Debug.LogWarning("WaterMerchant patch initialization failed; mod remains inactive: " + ex);
        }
    }

    private static void PatchPrefix(Harmony harmony, Type type, string methodName, Type[] argumentTypes, Type patchType, string prefixName)
    {
        var original = AccessTools.Method(type, methodName, argumentTypes)
            ?? throw new MissingMethodException(type.FullName, methodName);
        var prefix = AccessTools.Method(patchType, prefixName)
            ?? throw new MissingMethodException(patchType.FullName, prefixName);
        harmony.Patch(original, prefix: new HarmonyMethod(prefix));
    }

    private static void PatchPostfix(Harmony harmony, Type type, string methodName, Type[] argumentTypes, Type patchType, string postfixName)
    {
        var original = AccessTools.Method(type, methodName, argumentTypes)
            ?? throw new MissingMethodException(type.FullName, methodName);
        var postfix = AccessTools.Method(patchType, postfixName)
            ?? throw new MissingMethodException(patchType.FullName, postfixName);
        harmony.Patch(original, postfix: new HarmonyMethod(postfix));
    }

    public void OnDisable()
    {
        Settings.Active = false;
        Recipes.Set(false);
        if (PlayerStore.IsInstanceExist() && PlayerStore.Instance != null && EmporiumEntry.Instance != null)
            Hooks.RestoreOwned();
        _harmony?.UnpatchAll(HarmonyId);
        _harmony = null;
        Hooks.ResetPendingRefresh();
        if (_runner != null) UnityEngine.Object.Destroy(_runner);
        _runner = null;
    }

}

public sealed class WaterMerchantRunner : MonoBehaviour
{
    internal Mod Mod = null!;

    private void Update() => Mod.OnUpdate();
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
        foreach (var entry in new[] { FarmPower, FarmAir, FarmPrice, UvPitcher, PortableCapacity }) entry.OnChanged += _ => { if (Active) Hooks.RequestRefresh(); };
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
    private static bool _refreshRequested;
    private static int _refreshNotBeforeFrame;

    internal static void RequestRefresh()
    {
        _refreshRequested = true;
        _refreshNotBeforeFrame = Time.frameCount + 1;
    }

    internal static void UpdatePendingRefresh()
    {
        if (!_refreshRequested || Time.frameCount < _refreshNotBeforeFrame ||
            !PlayerStore.IsInstanceExist() || PlayerStore.Instance == null || EmporiumEntry.Instance == null)
            return;

        _refreshRequested = false;
        RefreshOwned();
    }

    internal static void ResetPendingRefresh()
    {
        _refreshRequested = false;
        _refreshNotBeforeFrame = 0;
    }

    public static void ApplyFarmCreated(GameItem item) { item.identifier = "moisture_farm"; Apply(item); }
    public static void RefreshOwned()
    {
        var count = 0;
        foreach (var item in EmporiumEntry.Instance.GetAllOwnedItems())
        {
            Apply(item);
            count++;
        }
        Debug.Log("WaterMerchant owned item refresh complete: " + count + " item(s).");
    }
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

internal static class Loaded { internal static void Postfix() => Hooks.RequestRefresh(); }
internal static class FarmCreated { internal static void Postfix(GameItem __result) => Hooks.ApplyFarmCreated(__result); }
internal static class PitcherCreated { internal static void Postfix(GameItem __result) => Hooks.Apply(__result); }
internal static class PortableCreated { internal static void Postfix(GameItem __result) { if (Settings.PortableCapacity.Value) DurabilityHelper.SetDurability(__result, 400, 400, 400); } }
internal static class FarmOutput
{
    internal static bool Prefix(GameItem gameItem, ref int __result) { if (!Settings.FarmOutput.Value) return true; __result = Math.Max(1000000 * (100 + MachineryHelper.GetCurrentPerformanceBonus(gameItem)) / 100, 0); return false; }
}
internal static class FarmAir { internal static bool Prefix(int volume, ref int __result) { if (!Settings.FarmAir.Value) return true; __result = volume; return false; } }
internal static class FarmTooltip { internal static bool Prefix(ref int __result) { if (!Settings.FarmAir.Value) return true; __result = 1; return false; } }
internal static class PurifierQuality { internal static bool Prefix(int bonusQuality, ref double __result) { if (!Settings.PurifierQuality.Value || bonusQuality < 150) return true; __result = 0; return false; } }

internal static class PurifierAmount
{
    internal static bool Prefix(int purity, ref double __result)
    {
        if (!Settings.PurifierAmount.Value || purity < 9600) return true;
        __result = 1.5;
        return false;
    }
}

internal static class Filter
{
    internal static bool Prefix(string identifier, int amount, GameItem filter, ref int __result)
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
internal static class PortablePurify
{
    internal static bool Prefix(GameItem container)
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


internal static class RecipeInit { internal static void Prefix() => Recipes.Ensure(); }
internal static class FarmCount { internal static bool Prefix(ref int __result) { if (!Settings.FarmAir.Value) return true; __result = 1; return false; } }


internal static class PortableCapacityAfterUse
{
    internal static void Postfix(GameItem gameItem)
    {
        if (!Settings.PortableCapacity.Value && gameItem.identifier == "portable_water_purifier" && DurabilityHelper.GetMaxDurability(gameItem) == 400 && DurabilityHelper.GetRealDurability(gameItem) <= 300 && DurabilityHelper.GetFakeDurability(gameItem) <= 300)
            Hooks.Apply(gameItem);
    }
}

