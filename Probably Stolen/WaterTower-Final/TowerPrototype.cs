using System;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace WaterTowerFinal;

public sealed class WaterTowerMod : IMod
{
    public void Init(ModManifest manifest) { }
    public void OnEnable() { }
    public void OnDisable() => TowerFactory.Instances = new();
}

internal sealed class Tower
{
    internal GameItem Item = null!;
    internal GameGridInventory Zone = null!;
    internal GameSlotInventory Tank = null!, Bottle = null!, Note = null!;
    internal GameSlotInventory[] Filters = null!;
}

internal static class TowerFactory
{
    internal static ConditionalWeakTable<GameItem, Tower> Instances = new();
    internal static GameItem CreateTank()
    {
        var item = ItemDirectory.CreateEmptyItem("watertower_tank");
        item.SetSpriteAndShape("Items/water_container4", "xl");
        WaterHelper.InitLiquidContainerItem(item, 600000, 0, false, false, false, true, 4);
        item.name = "水塔蓄水箱（600L）";
        return item;
    }
    internal static GameItem CreateTower()
    {
        var item = ItemDirectory.CreateEmptyItem("watertower");
        item.SetSprite("Items/watertower_mod", "water_tower");
        item.SetShape(new GridShapeBuilder(3, 4).SetDataFill(3, 4, 1).Build());
        item.name = "水塔";
        item.shortDescription = "工作区可放集水器、净水器和充电器。空槽机器将水送入蓄水箱。";
        item.unitValue = 1000;
        item.EnableTag("CONTAINER_TAG");
        item.EnableTag("WATERTOWER_LAYOUT2");
        item.SetGameItemType("STORAGE");
        var tower = new Tower { Item = item, Zone = new GameGridInventory(16, 16), Tank = new GameSlotInventory(), Bottle = new GameSlotInventory(), Note = new GameSlotInventory(), Filters = new[] { new GameSlotInventory(), new GameSlotInventory(), new GameSlotInventory() } };
        tower.Zone.mayInventoryAddItemFunc = (x, _) => GeneralHelper.IsItemOwned(x) && (x.identifier == "moisture_farm" || x.identifier == "water_purifier" || x.identifier == "recharger" || x.identifier == "powerblock");
        tower.Bottle.mayInventoryAddItemFunc = (x, _) => GeneralHelper.IsItemOwned(x) && x.IsTag("LIQUID_CONTAINER_TAG");
        foreach (var slot in tower.Filters) ContainerHelper.AllowOnlyTaggedItem(slot, "WATER_FILTER", true);
        var window = new PixelWindow(true, "水塔");
        var outer = new GridPixelElement(1, 3);
        var area = new GridPixelElement(1, 17);
        var filterRow = new GridPixelElement(3, 2);
        var outputRow = new GridPixelElement(4, 2);
        area.Attach(new TagElement(-1, false).SetText("工作区"), 0, 0);
        area.Attach(tower.Zone, 0, 1);
        for (int i = 0; i < 3; i++) { filterRow.Attach(new TagElement(-1, false).SetText("过滤器" + (i + 1)), i, 0); filterRow.Attach(tower.Filters[i], i, 1); }
        outputRow.Attach(new TagElement(-1, false).SetText("蓄水箱"), 0, 0);
        outputRow.Attach(new TagElement(-1, false).SetText("取水槽"), 1, 0);
        outputRow.Attach(new TagElement(-1, false).SetText("备注"), 3, 0);
        outputRow.Attach(tower.Tank, 0, 1);
        outputRow.Attach(tower.Bottle, 1, 1);
        outputRow.Attach(new ButtonElement("取水", callback: () => Fill(tower)), 2, 1);
        outputRow.Attach(tower.Note, 3, 1);
        outer.Attach(area, 0, 0); outer.Attach(filterRow, 0, 1); outer.Attach(outputRow, 0, 2);
        window.Attach(outer);
        item.SetContentWindow(window);
        item.maySelectSlotItemFunc = (x, _, _) => GeneralHelper.IsItemOwned(x);
        item.canSelectSlotItemFunc = (x, _, _) => GeneralHelper.IsItemOwned(x);
        item.mayThisTargetItemFunc = (x, target) => CanTarget(tower, target);
        item.canThisTargetItemFunc = (x, target) => CanTarget(tower, target);
        item.onThisTargetItemFunc = (x, target) => Pour(tower, target);
        Instances.Add(item, tower);
        return item;
    }
    private static bool CanTarget(Tower tower, GameItem target)
    {
        if (!GeneralHelper.IsItemOwned(tower.Item) || tower.Tank.childItem == null || target == null) return false;
        return (target.IsTag("LIQUID_CONTAINER_TAG") && GeneralHelper.IsItemOwned(target)) || WaterHelper.IsDrain(target) || WaterHelper.IsFaucet(target);
    }
    private static void Pour(Tower tower, GameItem target)
    {
        if (!CanTarget(tower, target)) return;
        var tank = tower.Tank.childItem;
        if (target.IsTag("LIQUID_CONTAINER_TAG")) WaterHelper.TransferLiquid(tank, target);
        else if (WaterHelper.IsDrain(target)) WaterHelper.EmptyContainer(tank);
        else if (WaterHelper.IsFaucet(target))
        {
            var rust = NetworkUpgrade.GetUpgradeById("RUSTWATER_TAP");
            var gutter = NetworkUpgrade.GetUpgradeById("GUTTERFLOW_TAP");
            if (rust.IsUnlocked()) WaterHelper.AddWater(tank, 1, 9999999, true, StoreStation.Instance.todayRustwaterPurity, StoreStation.Instance.todayRustwaterPurity);
            else if (gutter.IsUnlocked()) WaterHelper.AddWater(tank, 1, 9999999, true, StoreStation.Instance.todayGutterflowPurity, StoreStation.Instance.todayGutterflowPurity);
        }
    }
    internal static void EnsureNote(Tower tower)
    {
        if (tower.Note.childItem == null)
        {
            var note = DirectoryMaster.Item("postit", true);
            note.name = "水塔";
            note.shortDescription = "工作区内放置集水器和净水器。机器内有容器时水会留在机器内；空槽时水进入水塔。过滤器按槽位顺序处理。";
            note.unitValue = 0;
            if (!tower.Note.UncheckedAccept(note)) throw new InvalidOperationException("Water tower note placement failed");
        }
        tower.Note.LockInv();
    }
    internal static void Fill(Tower tower)
    {
        var tank = tower.Tank.childItem; var target = tower.Bottle.childItem;
        if (tank != null && target != null && GeneralHelper.IsItemOwned(target)) WaterHelper.TransferLiquid(tank, target);
    }
    internal static void EnsureTank(Tower tower)
    {
        if (tower.Tank.childItem == null)
        {
            var tank = DirectoryMaster.Item("watertower_tank", true);
            if (!tower.Tank.UncheckedAccept(tank)) throw new InvalidOperationException("Water tower tank placement failed");
        }
        tower.Tank.LockInv();
    }
}

[HarmonyPatch(typeof(FoodItemDirectory), "InitDirectory")]
internal static class RegisterItems
{
    private static void Postfix(FoodItemDirectory __instance)
    {
        __instance.Add("watertower", TowerFactory.CreateTower);
        __instance.Add("watertower_tank", TowerFactory.CreateTank);
    }
}

[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.EndNight))]
internal static class EndNight
{
    private static void Postfix()
    {
        foreach (var item in PlayerStore.Instance.FindAllItem())
        {
            if (item.identifier != "watertower" || !TowerFactory.Instances.TryGetValue(item, out var tower)) continue;
            var tank = tower.Tank.childItem;
            if (tank == null || tank.identifier != "watertower_tank") continue;
            tower.Tank.LockInv();
            bool previousNightCycle = CurrentConverterHelper.isNightCycle;
            try
            {
                CurrentConverterHelper.isNightCycle = true;
                TowerSettlement.Run(tank, tower.Zone.childItems, tower.Filters.Select(s => s.childItem).ToArray());
            }
            finally { CurrentConverterHelper.isNightCycle = previousNightCycle; }
        }
    }
}
[HarmonyPatch(typeof(StoreClientList), nameof(StoreClientList.PlaceInventorInventory))]
internal static class OfferTower
{
    private static void Postfix()
    {
        if (StoreStation.GetDayCounter() < 15 || PlayerStore.Instance.FindAllItem().Any(i => i.identifier == "watertower")) return;
        var item = DirectoryMaster.Item("watertower", false);
        if (!TowerFactory.Instances.TryGetValue(item, out var tower)) throw new InvalidOperationException("Water tower was not registered");
        TowerFactory.EnsureTank(tower);
        TowerFactory.EnsureNote(tower);
        PlayerStore.Instance.AddDirectSellingItemToTable(item);
    }
}


[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.LoadGame))]
internal static class RestoreSlotLocks
{
    private static void Postfix()
    {
        foreach (var item in PlayerStore.Instance.FindAllItem())
        {
            if (item.identifier != "watertower" || !TowerFactory.Instances.TryGetValue(item, out var tower)) continue;
            if (tower.Tank.childItem?.identifier == "watertower_tank") tower.Tank.LockInv();
            if (tower.Note.childItem?.identifier == "postit") tower.Note.LockInv();
        }
    }
}
[HarmonyPatch(typeof(GameItem), nameof(GameItem.Target))]
internal static class TurboTarget
{
    private static void Postfix(GameItem __instance, GameItem targetItem)
    {
        if (__instance.identifier != "turbo_booster" && __instance.identifier != "turbo_booster_adv" && __instance.identifier != "turbo_booster_hypertech") return;
        if (targetItem == null || (targetItem.identifier != "moisture_farm" && targetItem.identifier != "water_purifier")) return;
        var owner = targetItem.parentInventory?.GetParentItem();
        if (owner == null || !TowerFactory.Instances.TryGetValue(owner, out var tower)) return;
        if (targetItem.parentInventory != tower.Zone || tower.Tank.childItem == null) return;
        TowerSettlement.Run(tower.Tank.childItem, new[] { targetItem }, tower.Filters.Select(s => s.childItem).ToArray(), turbo: true);
    }
}


[HarmonyPatch(typeof(RenderHandler), nameof(RenderHandler.LoadFromAtlas))]
internal static class TowerSprite
{
    private static Sprite? sprite;
    private static bool Prefix(string atlasPath, string name, ref Sprite __result)
    {
        if (atlasPath != "Items/watertower_mod" || name != "water_tower") return true;
        if (sprite == null)
        {
            using var stream = typeof(TowerSprite).Assembly.GetManifestResourceStream("WaterTower.png")!;
            using var bytes = new System.IO.MemoryStream();
            stream.CopyTo(bytes);
            var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!ImageConversion.LoadImage(source, bytes.ToArray())) throw new InvalidOperationException("Water tower sprite could not be decoded");
            var previous = RenderTexture.active;
            var render = RenderTexture.GetTemporary(48, 64, 0, RenderTextureFormat.ARGB32);
            var texture = new Texture2D(48, 64, TextureFormat.RGBA32, false);
            try
            {
                Graphics.Blit(source, render);
                RenderTexture.active = render;
                texture.ReadPixels(new Rect(0, 0, 48, 64), 0, 0);
                texture.Apply();
            }
            finally
            {
                RenderTexture.active = previous;
                RenderTexture.ReleaseTemporary(render);
                UnityEngine.Object.Destroy(source);
            }
            texture.filterMode = FilterMode.Point;
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, 0), texture.width / .48f);
        }
        __result = sprite;
        return false;
    }
}


