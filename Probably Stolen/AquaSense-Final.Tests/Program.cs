using AquaSense;
using Il2Cpp;
using UnityEngine;
using System.Reflection;

internal static class Program
{
    static int passed;
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    static void Test(string name, Action test) { Reset(); test(); passed++; Console.WriteLine("PASS " + name); }
    static void Reset()
    {
        Hooks.ResetMaintenance(); PlayerStore.Instance = new(); EmporiumEntry.Instance = new();
        new Core().Init(new ModManifest()); Time.unscaledTime = 0; Time.frameCount = 0;
        GraphUtils.Swaps = 0; GraphUtils.SwapFault = null; GraphUtils.PlaceFault = null; ItemMouseDragHandler.current = null;
    }
    static GameItem Item(string id, GameInventory inv, int durability = 10)
    {
        var x = new GameItem { identifier = id, Durability = durability }; x.state.Set("IS_OWNED_TAG");
        if (id.Contains("water_filter")) x.state.Set("WATER_FILTER");
        inv.Put(x); return x;
    }
    static (GameItem jug, GameItem filter) Jug(int durability)
    {
        var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
        var slot = new GameSlotInventory { ParentItem = jug }; jug.child = slot; jug.children.Add(slot); jug.FilterSlot = slot;
        return (jug, Item("water_filter", slot, durability));
    }
    static (GameItem box, GameGridInventory inner) Box()
    {
        var box = Item("storage_box", EmporiumEntry.Instance.invWindow);
        var inner = new GameGridInventory { ParentItem = box }; box.child = inner; box.children.Add(inner); return (box, inner);
    }
    static void Frames(int count = 80) { for (var i = 0; i < count; i++) { Time.frameCount++; Hooks.UpdateMaintenance(true); } }
    static int Cached() { var f = typeof(Hooks).GetField("FilterCache", BindingFlags.Static | BindingFlags.NonPublic)!; return ((System.Collections.IEnumerable)f.GetValue(null)!).Cast<System.Collections.ICollection>().Sum(x => x.Count); }
    static void Main()
    {
        Test("filter durability without scanner, empty and filled containers", () => {
            var (jug, filter) = Jug(37);
            filter.MaxDurability = 400;
            Liquid.Liquids = new() { new Liquid() };
            try {
                foreach (var filled in new[] { false, true }) {
                    if (filled) jug.state.Set("water", 1000000);
                    var builder = new RichTextBuilder();
                    Hooks.AfterWaterContainerTooltip(builder, jug);
                    Hooks.AfterWaterFeatureTooltip(builder, jug);
                    Check(builder.Slices.Count == 1 && builder.Slices[0].text.Contains("37/400"), "missing or duplicate durability");
                    Check(!builder.Slices.Any(x => x.text.Contains("姘磋川璇︽儏")), "composition exposed without scanner");
                }
                Core.ShowFilterDurability!.Value = false;
                var disabled = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(disabled, jug);
                Check(disabled.Slices.Count == 0, "durability setting ignored");
            } finally { Liquid.Liquids.Clear(); }
        });
        Test("patches wait for gameplay and install once per enable", () => {
            HarmonyLib.Harmony.Patches = HarmonyLib.Harmony.Unpatches = 0;
            var core = new Core(); core.OnEnable(); core.OnEnable();
            Check(HarmonyLib.Harmony.Patches == 0, "patched during enable");
            var store = PlayerStore.Instance; PlayerStore.Instance = null;
            core.OnUpdate(); Check(HarmonyLib.Harmony.Patches == 0, "patched before store ready");
            PlayerStore.Instance = store; core.OnUpdate(); core.OnUpdate();
            Check(HarmonyLib.Harmony.Patches == 9, "duplicate or missing patches");
            core.OnDisable(); core.OnEnable(); core.OnUpdate();
            Check(HarmonyLib.Harmony.Patches == 18, "reenable failed"); core.OnDisable();
        });
        Test("patch failure rolls back and does not retry every frame", () => {
            HarmonyLib.Harmony.Patches = HarmonyLib.Harmony.Unpatches = 0;
            HarmonyLib.Harmony.Fail = true;
            try {
                var core = new Core(); core.OnEnable(); core.OnUpdate(); core.OnUpdate();
                Check(HarmonyLib.Harmony.Patches == 1 && HarmonyLib.Harmony.Unpatches == 1, "partial patches or repeated failure");
                core.OnDisable();
            } finally { HarmonyLib.Harmony.Fail = false; }
        });
        Test("exhausted on load, spare in nested box", () => {
            var (jug, _) = Jug(0); var (_, inner) = Box(); var spare = Item("water_filter", inner);
            Frames(); Check(jug.FilterSlot!.childItem == spare && GraphUtils.Swaps == 1, "nested spare not swapped");
            Check(PlayerStore.Instance!.RootCalls == 0, "synchronous full inventory query");
        });
        Test("filter event uses cached candidate", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); var spare = Item("water_filter", inner);
            Frames(); Check(Cached() == 1, "initial cache missing"); old.Durability=0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == spare && GraphUtils.Swaps == 1, "event swap failed");
        });
        Test("stale candidate then next cached candidate", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); var stale=Item("water_filter_adv", inner); var good=Item("water_filter", inner);
            Frames(); var elsewhere = new GameGridInventory { ParentItem = inner.ParentItem }; elsewhere.Put(stale);
            old.Durability=0; Hooks.AfterFilter(old); Frames(); Check(jug.FilterSlot!.childItem == good, "stale candidate blocked next");
        });
        Test("missing candidate is found after retry scan", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); Frames(); old.Durability=0; Hooks.AfterFilter(old); Frames();
            var spare=Item("water_filter",inner); Time.unscaledTime=6; Frames(180);
            Check(jug.FilterSlot!.childItem == spare, "new spare not found");
        });
        Test("de-duplicates exhausted events and does not steal unowned item", () => {
            var (jug, old) = Jug(10); var (_, inner)=Box(); var spare=Item("water_filter",inner); Frames();
            spare.state.Tags.Remove("IS_OWNED_TAG"); old.Durability=0; Hooks.AfterFilter(old); Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem==old && GraphUtils.Swaps==0, "unowned candidate used");
        });
        Test("multiple exhausted filters share cached spares", () => {
            var (first, a)=Jug(10); var (second, b)=Jug(10); var (_,inner)=Box();
            var spare1=Item("water_filter_adv",inner); var spare2=Item("water_filter",inner); spare2.modifiedShape.minX=2; Frames();
            a.Durability=0; b.Durability=0; Hooks.AfterFilter(a); Hooks.AfterFilter(b); Frames();
            Check(first.FilterSlot!.childItem==spare1 && second.FilterSlot!.childItem==spare2 && GraphUtils.Swaps==2, "incorrect queue or priority");
        });
        Test("dragging delays exchange without losing the pending filter", () => {
            var (jug,old)=Jug(10); var (_,inner)=Box(); var spare=Item("water_filter",inner); Frames();
            ItemMouseDragHandler.current=new ItemMouseDragHandler { IsDraggingItem=true }; old.Durability=0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem==old && GraphUtils.Swaps==0,"swapped while dragging");
            ItemMouseDragHandler.current=null; Time.unscaledTime=6; Frames();
            Check(jug.FilterSlot!.childItem==spare,"pending filter lost during drag");
        });
        Test("failed partial swap is recovered and blocks further swaps", () => {
            var (jug,old)=Jug(10); var (_,inner)=Box(); var spare=Item("water_filter",inner); Frames();
            GraphUtils.SwapFault=(a,b)=>{ GraphUtils.Detach(a); return false; };
            GraphUtils.PlaceFault=(_,_,_)=>false; old.Durability=0; Hooks.AfterFilter(old); Frames();
            Check(typeof(Hooks).GetField("_retryFilterRecovery",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!=null,"failed recovery not retained");
            GraphUtils.PlaceFault=null; GraphUtils.SwapFault=null; Time.unscaledTime=6; Frames(1);
            Check(jug.FilterSlot!.childItem==old && spare.parentInventory==inner,"original positions not recovered");
        });
        Test("reload clears stale references", () => {
            var (jug, old)=Jug(10); var (_,inner)=Box(); var spare=Item("water_filter",inner); Frames();
            Hooks.AfterLoadGame(); inner.childItems.Remove(spare); spare.Parent=null; old.Durability=0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem==old && GraphUtils.Swaps==0, "stale cache used after load");
        });
        Test("scanner addition and removal refresh its cache", () => {
            Frames(); var scan=Item("aquascan",EmporiumEntry.Instance.invWindow); Hooks.AfterItemParentChanged(scan,true); Frames();
            var cache=typeof(Hooks).GetField("_scannerCache",BindingFlags.Static|BindingFlags.NonPublic)!;
            Check((bool)cache.GetValue(null)!,"scanner addition not found");
            EmporiumEntry.Instance.invWindow.childItems.Remove(scan); scan.Parent=null; Hooks.AfterItemParentChanged(scan,true); Frames();
            Check(!(bool)cache.GetValue(null)!,"scanner removal not found");
        });
        Test("scan limits per frame", () => {
            var root=EmporiumEntry.Instance.invWindow; for (int i=0;i<500;i++) Item("object_"+i,root);
            Hooks.UpdateMaintenance(true); Check(typeof(Hooks).GetField("_maintenance",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!=null,"scan completed in one frame");
            Frames(100); Check(PlayerStore.Instance!.RootCalls==0,"unbounded inventory query");
        });
        Console.WriteLine($"{passed} scenarios passed");
    }
}


