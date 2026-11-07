using System.Reflection;
using WaterTankLevel;

static class Program
{
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    static object Call(Type type, string method, params object[] args) => type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, args);
    static void Main()
    {
        var assets = new LoadedMod { Manifest = new ModManifest { ID = "chiqingsan.WaterTankLevel" }, IsEnabled = true };
        foreach (var color in new[] { "blue", "red", "dark" })
            foreach (var level in new[] { "10", "25", "50", "75", "100" })
            {
                string key = "tank_" + color + "_" + level;
                assets.Sprites.Add(key, new UnityEngine.Sprite { name = key });
            }
        assets.Sprites.Add("tank_body", new UnityEngine.Sprite { name = "tank_body" });
        ModLoader.Mods.Add(assets);
        var loaderHarmony = new HarmonyLib.Harmony("mod.chiqingsan.WaterTankLevel");
        loaderHarmony.PatchAll(typeof(WaterTankLevelMod).Assembly);
        var mod = new WaterTankLevelMod();
        mod.Init(new ModManifest());
        mod.OnEnable();
        Check(HarmonyLib.Harmony.PatchAllCalls == 1, "mod initialization must not repeat the loader's patch installation");
        mod.OnDisable();
        Check(HarmonyLib.Harmony.UnpatchCalls == 0, "the loader owns patch removal");
        var tank = new GameItemElement { identifier = "water_tank", spriteAtlasPath = "Items/items_tool2", spritePath = "water_tank" };
        Check(TankAppearance.LiquidLayers(tank) == null, "uninitialized capacity must not divide by zero");
        Check(WaterHelper.CapacityCalls == 0, "capacity calculation must wait for initialization");
        object[] prefix = { tank, false };
        Call(typeof(TankLayersPatch), "Prefix", prefix);
        Check((bool)prefix[1] && tank.getItemSpriteModifierForItemFunc != null, "original tank should receive temporary layers");
        var error = new InvalidOperationException("test");
        Check(ReferenceEquals(Call(typeof(TankLayersPatch), "Finalizer", tank, true, error), error), "errors must propagate");
        Check(tank.getItemSpriteModifierForItemFunc == null, "temporary delegate must be removed even on error");
        tank.Capacity = 100; tank.Percent = 50;
        var layers = TankAppearance.LiquidLayers(tank);
        Check(layers.Count == 1 && layers[0].sprite.name == "tank_blue_50", "custom half-full liquid layer without extra highlight");
        tank.Percent = 0;
        Check(TankAppearance.LiquidLayers(tank) == null, "empty tank has no layers");
        Func<GameItem, List<ItemSpriteModifier>> custom = _ => null;
        tank.getItemSpriteModifierForItemFunc = custom;
        prefix = new object[] { tank, false };
        Call(typeof(TankLayersPatch), "Prefix", prefix);
        Check(!(bool)prefix[1] && ReferenceEquals(tank.getItemSpriteModifierForItemFunc, custom), "custom layers must remain untouched");
        Check(!TankAppearance.IsManaged(tank), "custom appearance is excluded");
        tank.getItemSpriteModifierForItemFunc = null;
        tank.spriteAtlasPath = "custom";
        Check(!TankAppearance.IsManaged(tank), "custom atlas is excluded");
        var fresh = new GameItemElement { spriteAtlasPath = "Items/items_tool2", spritePath = "water_tank" };
        fresh.ValidatePartialSprite();
        Check(fresh.Body.name == "water_tank", "before identifier assignment, construction uses the stock body");
        fresh.identifier = "water_tank";
        Call(typeof(TankCreatedPatch), "Postfix", fresh);
        Check(fresh.Body.name == "tank_body" && ReferenceEquals(fresh.Body, fresh.FakeBody),
            "completed construction must refresh both body images");
        Check(fresh.BodyRefreshes == 2 && fresh.LayerRefreshes == 0, "creation adds exactly one body-only refresh");
        Check(fresh.identifier == "water_tank" && fresh.spriteAtlasPath == "Items/items_tool2"
            && fresh.spritePath == "water_tank" && !fresh.useSpriteFromMod && fresh.Capacity == 0,
            "creation refresh must not modify stored sprite paths, capacity or identity");
        Check(fresh.getItemSpriteModifierForItemFunc == null, "creation must not leave a permanent liquid delegate");
        fresh.Capacity = 100;
        foreach (var level in new[] { 10, 25, 50, 75, 100 })
        {
            fresh.Percent = level;
            Check(TankAppearance.LiquidLayers(fresh)[0].sprite.name == "tank_blue_" + level,
                "all non-empty water levels retain their custom layers");
        }
        Call(typeof(TankLiquidChangedPatch), "Postfix", fresh);
        Check(fresh.LayerRefreshes == 1, "liquid changes must still refresh layers");
        fresh.getItemSpriteModifierForItemFunc = custom;
        Call(typeof(TankCreatedPatch), "Postfix", fresh);
        Check(fresh.BodyRefreshes == 2, "creation must leave another mod's custom layers untouched");
        fresh.getItemSpriteModifierForItemFunc = null;
        fresh.useSpriteFromMod = true;
        Call(typeof(TankCreatedPatch), "Postfix", fresh);
        Check(fresh.BodyRefreshes == 2, "mod-provided sprite paths remain excluded");
        fresh.useSpriteFromMod = false;
        assets.IsEnabled = false;
        Call(typeof(TankCreatedPatch), "Postfix", fresh);
        Check(fresh.BodyRefreshes == 2, "disabled assets must not trigger replacement");
        assets.IsEnabled = true;
        ModLoader.Mods.Clear();
        ModLoader.Mods.Add(new LoadedMod { Manifest = assets.Manifest, IsEnabled = true });
        Call(typeof(TankCreatedPatch), "Postfix", fresh);
        Check(fresh.BodyRefreshes == 2 && TankAppearance.LiquidLayers(fresh) == null,
            "incomplete texture packages must keep the original appearance");
        Console.WriteLine("WaterTankLevel regression checks passed.");
    }
}

namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)] public class HarmonyPatch : Attribute { public HarmonyPatch(Type type, string method) { } }
    public class Harmony { public static int PatchAllCalls, UnpatchCalls; public Harmony(string id) { } public void PatchAll(Assembly assembly) { PatchAllCalls++; } public void UnpatchAll(string id) { UnpatchCalls++; } }
}
namespace UnityEngine
{
    public struct Vector2 { public static Vector2 zero => new(); }
    public class Sprite { public string name; }
}
public interface IMod { void Init(ModManifest manifest); void OnEnable(); void OnDisable(); }
public class ModManifest { public string ID; }
public class LoadedMod { public ModManifest Manifest; public bool IsEnabled; public Dictionary<string, UnityEngine.Sprite> Sprites = new(); }
public static class ModLoader { public static List<LoadedMod> Mods = new(); }
public class TagState { public int Value; public int GetInt() => Value; }
public class GameItem
{
    public string identifier, spriteAtlasPath, spritePath;
    public bool useSpriteFromMod;
    public int Capacity, Percent;
    public Func<GameItem, List<ItemSpriteModifier>> getItemSpriteModifierForItemFunc;
    public TagState GetTagReadonly(string name) => new() { Value = Capacity };
}
public class GameItemElement : GameItem
{
    public UnityEngine.Sprite Body, FakeBody;
    public int BodyRefreshes, LayerRefreshes;
    public void HandleInnateModSprite() { LayerRefreshes++; }
    public UnityEngine.Sprite ResolveSpriteByName(string name)
    {
        object[] args = { this, name, null };
        var method = typeof(TankSpritePatch).GetMethod("Prefix", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (bool)method.Invoke(null, args) ? new UnityEngine.Sprite { name = name } : (UnityEngine.Sprite)args[2];
    }
    public void ValidatePartialSprite()
    {
        BodyRefreshes++;
        Body = ResolveSpriteByName(spritePath);
        FakeBody = Body;
    }
}
public static class WaterTankHelper { public static GameItem Create() => null; }
public static class WaterHelper
{
    public static int CapacityCalls;
    public static int GetCurrentCapacity(GameItem item) { CapacityCalls++; return item.Percent * 100 / item.Capacity; }
    public static string GetLiquidContainerColor(GameItem item) => "color";
}
public static class SpriteHelper { public static string MapCapacity(int n) => n.ToString(); public static void UpdateWaterContainerSprite(GameItem item) { } }
public static class Liquid { public static string GetColorBase(string color) => "blue"; }
public static class MetaHelper { public static object HexToColor(string color) => null; }
public static class RenderHandler { public static UnityEngine.Sprite LoadFromAtlas(string atlas, string name) => new() { name = name }; }
public class ItemSpriteModifier
{
    public UnityEngine.Sprite sprite;
    public ItemSpriteModifier(UnityEngine.Sprite sprite, UnityEngine.Vector2 position, int rotation = 0, bool flipped = false, int priority = 0, bool mask = false, object color = null) { this.sprite = sprite; }
}
