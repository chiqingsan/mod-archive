using System.Collections.Generic;
using System;
using HarmonyLib;
using UnityEngine;

namespace WaterTankLevel;

public sealed class WaterTankLevelMod : IMod
{
    // ModLoader installs and removes the attributed Harmony patches.
    public void Init(ModManifest manifest) { }

    public void OnEnable() { }

    public void OnDisable() { }
}

internal static class TankAppearance
{
    private const string ModId = "chiqingsan.WaterTankLevel";
    private static readonly string[] Colors = { "blue", "red", "dark" };
    private static readonly string[] Levels = { "10", "25", "50", "75", "100" };
    private static LoadedMod spritesOwner;

    private static bool TryGetSprites(out Dictionary<string, Sprite> sprites)
    {
        var owner = ModLoader.Mods.Find(mod => mod.Manifest.ID == ModId && mod.IsEnabled);
        sprites = owner?.Sprites;
        if (sprites == null) return false;
        if (spritesOwner == owner) return true;
        if (!sprites.ContainsKey("tank_body")) return false;
        foreach (string color in Colors)
            foreach (string level in Levels)
                if (!sprites.ContainsKey("tank_" + color + "_" + level)) return false;
        spritesOwner = owner;
        return true;
    }

    internal static bool TryGetBody(out Sprite sprite)
    {
        sprite = null;
        return TryGetSprites(out var sprites) && sprites.TryGetValue("tank_body", out sprite);
    }

    internal static bool IsOriginal(GameItem item)
    {
        return item.identifier == "water_tank" && !item.useSpriteFromMod
            && item.spriteAtlasPath == "Items/items_tool2" && item.spritePath == "water_tank";
    }

    internal static List<ItemSpriteModifier> LiquidLayers(GameItem item)
    {
        // EnableTag triggers a UI refresh before InitLiquidContainerItem sets capacity.
        if (!TryGetSprites(out var sprites) || item.GetTagReadonly("LIQUID_CONTAINER_CAPACITY").GetInt() <= 0) return null;
        int capacity = WaterHelper.GetCurrentCapacity(item);
        if (capacity == 0) return null;

        string level = SpriteHelper.MapCapacity(capacity);
        string color = WaterHelper.GetLiquidContainerColor(item);
        return new List<ItemSpriteModifier>
        {
            new ItemSpriteModifier(sprites["tank_" + Liquid.GetColorBase(color) + "_" + level],
                Vector2.zero, 0, false, 0, false, MetaHelper.HexToColor(color))
        };
    }

    internal static bool IsManaged(GameItem item)
    {
        return IsOriginal(item) && TryGetSprites(out _) && (item.getItemSpriteModifierForItemFunc == null
            || item.getItemSpriteModifierForItemFunc == LiquidLayers);
    }
}

[HarmonyPatch(typeof(GameItemElement), nameof(GameItemElement.HandleInnateModSprite))]
internal static class TankLayersPatch
{
    private static void Prefix(GameItemElement __instance, out bool __state)
    {
        __state = TankAppearance.IsManaged(__instance) && __instance.getItemSpriteModifierForItemFunc == null;
        if (__state)
            __instance.getItemSpriteModifierForItemFunc = TankAppearance.LiquidLayers;
    }

    private static Exception Finalizer(GameItemElement __instance, bool __state, Exception __exception)
    {
        if (__state && __instance.getItemSpriteModifierForItemFunc == TankAppearance.LiquidLayers)
            __instance.getItemSpriteModifierForItemFunc = null;
        return __exception;
    }
}

[HarmonyPatch(typeof(GameItemElement), nameof(GameItemElement.ResolveSpriteByName))]
internal static class TankSpritePatch
{
    private static bool Prefix(GameItemElement __instance, string name, ref Sprite __result)
    {
        if (!TankAppearance.IsManaged(__instance) || name != "water_tank") return true;
        return !TankAppearance.TryGetBody(out __result);
    }
}

[HarmonyPatch(typeof(WaterTankHelper), nameof(WaterTankHelper.Create))]
internal static class TankCreatedPatch
{
    private static void Postfix(GameItem __result)
    {
        // Create sets the stock sprite before assigning the water_tank identifier.
        // Refresh only the rendered body once initialization has completed.
        if (__result is GameItemElement element && TankAppearance.IsManaged(element))
            element.ValidatePartialSprite();
    }
}

[HarmonyPatch(typeof(SpriteHelper), nameof(SpriteHelper.UpdateWaterContainerSprite))]
internal static class TankLiquidChangedPatch
{
    private static void Postfix(GameItem gameItem)
    {
        if (TankAppearance.IsManaged(gameItem) && gameItem is GameItemElement element)
            element.HandleInnateModSprite();
    }
}
