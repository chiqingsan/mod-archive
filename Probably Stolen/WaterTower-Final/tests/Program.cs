using System;
using System.Collections.Generic;
using WaterTowerFinal;

static class Tests
{
    static void Check(bool condition, string name) { if (!condition) throw new Exception(name); Console.WriteLine("PASS " + name); }
    static GameItem Machine(string id, int charge, bool hasContainer = false, int output = 1000000)
    {
        var m = new GameItem(id) { battery = new GameItem("battery") { energy = charge }, output = output };
        if (hasContainer) m.slot.childItem = new GameItem("bottle");
        return m;
    }
    static void Main()
    {
        var tank = new GameItem("tank"); var first = Machine("moisture_farm", 10); var bottled = Machine("moisture_farm", 10, true); var empty = Machine("moisture_farm", 0);
        TowerSettlement.Run(tank, new[] { first, bottled, empty }, Array.Empty<GameItem?>());
        Check(tank.GetTagReadonly("water").GetInt() == 1000000 && first.battery!.energy == 6 && first.actions == 1, "collector produces and charges once");
        Check(bottled.battery!.energy == 10 && empty.battery!.energy == 0, "bottled and insufficient power skipped");
        var huge = Machine("moisture_farm", 10, output: 12000000); tank = new GameItem("tank");
        TowerSettlement.Run(tank, new[] { huge }, Array.Empty<GameItem?>());
        Check(tank.GetTagReadonly("water").GetInt() == 12000000, "tank bypasses 6 L intermediate jug");
        tank = new GameItem("tank"); tank.ModifyTag("microbe", s => s.SetInt(2000)); var filter = new GameItem("filter"); first = Machine("moisture_farm",10);
        TowerSettlement.Run(tank, new[] { first }, new GameItem?[] { filter });
        Check(tank.GetTagReadonly("microbe").GetInt() == 0 && filter.calls == 1, "tank filter calls game filter");
        tank = new GameItem("tank"); tank.ModifyTag("water", s => s.SetInt(1000000)); var purifier = Machine("water_purifier", 10); purifier.removed = 1000;
        TowerSettlement.Run(tank, new[] { purifier }, Array.Empty<GameItem?>());
        Check(purifier.battery!.energy == 6 && purifier.actions == 1 && purifier.calls == 1, "purifier uses game method and consumes once");
        purifier = Machine("water_purifier",10); TowerSettlement.Run(tank, new[] { purifier }, Array.Empty<GameItem?>());
        Check(purifier.battery!.energy == 10 && purifier.actions == 0, "ordinary battery refunded if no contaminant removed");
        purifier = Machine("water_purifier",10); TowerSettlement.Run(tank, new[] { purifier }, Array.Empty<GameItem?>(), turbo: true);
        Check(purifier.battery!.energy == 6 && purifier.actions == 1, "turbo triggers modules even when purifier removes nothing");
    }
}
class TagState { public int value; public int GetInt()=>value; public TagState Enable()=>this; public TagState SetInt(int n){ value=n; return this; } }
class GameItem
{
    public string identifier; public int energy, output, removed, calls, actions; public GameItem? battery; public bool owned=true, broken; public GameSlotInventory slot=new(); public GameGridInventory modules=new(); public Dictionary<string,TagState> tags=new();
    public GameItem(string id){ identifier=id; }
    public bool IsTag(string tag)=>tag == "BROKEN_MACHINE_TAG" && broken;
    public TagState GetTagReadonly(string key){ if(!tags.ContainsKey(key)) tags[key]=new(); return tags[key]; }
    public void ModifyTag(string key, Action<TagState> action, bool skipValidate=false)=>action(GetTagReadonly(key));
}
class GameSlotInventory { public GameItem? childItem; }
class GameGridInventory { }
static class GeneralHelper { public static bool IsItemOwned(GameItem i)=>i.owned; }
static class MachineryHelper { public static int GetMachinePowerUsage(GameItem i)=>4; }
static class MachineHelper { public static GameSlotInventory GetSlot(GameItem i,int index)=>i.slot; public static GameSlotInventory GetBatterySlot(GameItem i)=>new(){childItem=i.battery}; public static GameGridInventory GetModuleInv(GameItem i)=>i.modules; public static void OnMachineActioned(GameItem i, GameGridInventory g)=>i.actions++; }
static class PowerHelper { public static bool CanDrawPowerSource(GameItem i,int amount)=>i.energy>=amount; public static bool DrawPowerSource(GameItem i,int amount){ if(i.energy<amount)return false; i.energy-=amount; return true; } }
class Liquid { public string identifier=""; public string currentPart=""; public static List<Liquid> Liquids=new(){new(){identifier="water",currentPart="water"},new(){identifier="microbe",currentPart="microbe"}}; }
static class MachineMoistureFarm { public static int GetOutputVolume(GameItem i)=>i.output; public static string GetOutputQuality(GameItem i)=>"Pure Water"; }
static class MachinePurifier { public static int PurifyContainer(GameItem i,GameItem t){i.calls++;return i.removed;} }
static class WaterHelper { public static int GetFreeCapacity(GameItem i)=>600000000-i.GetTagReadonly("water").GetInt(); public static bool IsEmpty(GameItem i)=>i.GetTagReadonly("water").GetInt()==0; public static void AddWater(GameItem i,int grade,int vol)=>i.GetTagReadonly("water").value+=vol; public static int HandleFilter(string id,int amount,GameItem? src,GameItem filter){if(id=="microbe") {filter.calls++;return 0;}return amount;} }
static class WaterFeatureHelper { public static void OnLiquidChanged(GameItem i,bool exposed){} }

