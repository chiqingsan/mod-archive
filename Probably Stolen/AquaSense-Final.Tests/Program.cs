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
        if (id.Contains("filter")) x.state.Set("WATER_FILTER");
        if (id is "water_filter" or "water_filter_adv" or "hydroreed_filter") x.state.Set("FILTER_SIZE_TAG", 22);
        inv.Put(x); return x;
    }
    static (GameItem jug, GameItem filter) Jug(int durability) => JugOf("water_filter", durability);
    static (GameItem jug, GameItem filter) JugOf(string filterIdentifier, int durability = 10)
    {
        var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
        var slot = new GameSlotInventory { ParentItem = jug }; jug.child = slot; jug.children.Add(slot); jug.FilterSlot = slot;
        return (jug, Item(filterIdentifier, slot, durability));
    }
    static (GameItem box, GameGridInventory inner) Box()
    {
        var box = Item("storage_box", EmporiumEntry.Instance.invWindow);
        var inner = new GameGridInventory { ParentItem = box }; box.child = inner; box.children.Add(inner); return (box, inner);
    }
    static (GameItem ras, GameSlotInventory slot) Ras()
    {
        var ras = Item("ras_empty", EmporiumEntry.Instance.invWindow);
        ras.state.Set("RAS_TAG");
        var slot = new GameSlotInventory { ParentItem = ras }; ras.child = slot; ras.children.Add(slot);
        return (ras, slot);
    }
    static void Frames(int count = 80) { for (var i = 0; i < count; i++) { Time.frameCount++; Hooks.UpdateMaintenance(true); } }
    static int Cached() { var f = typeof(Hooks).GetField("FilterCache", BindingFlags.Static | BindingFlags.NonPublic)!; return ((System.Collections.IEnumerable)f.GetValue(null)!).Cast<System.Collections.ICollection>().Sum(x => x.Count); }
    static void Main()
    {
        Test("highest remaining durability among fitting cached filters", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box();
            var low = Item("water_filter", inner, 20);
            var full = Item("water_filter", inner, 90); full.modifiedShape.minX = 2;
            var blocked = Item("water_filter", inner, 100); blocked.modifiedShape.minX = 4;
            jug.FilterSlot!.Accepts = item => item != blocked;
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot.childItem == full, "did not choose highest fitting durability");
        });
        Test("filter type priority precedes durability", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box();
            var normal = Item("water_filter", inner, 100);
            var advanced = Item("water_filter_adv", inner, 1); advanced.modifiedShape.minX = 2;
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == advanced, "durability overrode type priority");
        });
        Test("composition bypass defaults off and responds to toggle", () => {
            Check(Core.ShowCompositionWithoutScanner!.Value == false, "bypass must default off");
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.Liquids = new() { new Liquid() };
            Liquid.LiquidDict = null;
            jug.state.Set("water", 1000000);
            try {
                var hidden = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(hidden, jug);
                Check(hidden.Slices.Count == 0, "composition visible by default");
                Core.ShowCompositionWithoutScanner.Value = true;
                var shown = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(shown, jug);
                Hooks.AfterWaterFeatureTooltip(shown, jug);
                Check(shown.Slices.Count == 2 && shown.Slices.Any(x => x.text.Contains("1000 ml")), "composition missing or duplicated");
                Core.ShowCompositionWithoutScanner.Value = false;
                var hiddenAgain = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(hiddenAgain, jug);
                Check(hiddenAgain.Slices.Count == 0, "toggle off ignored");
                Item("aquascan", EmporiumEntry.Instance.invWindow); Frames();
                var scanned = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(scanned, jug);
                Check(scanned.Slices.Count == 2, "scanner no longer enables composition");
            } finally { Liquid.Liquids.Clear(); }
        });
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
        Test("hydroreed filter durability is shown in tooltips", () => {
            var (jug, hydroreed) = JugOf("hydroreed_filter", 37); hydroreed.MaxDurability = 200;
            var builder = new RichTextBuilder();
            Hooks.AfterWaterContainerTooltip(builder, jug);
            Check(builder.Slices.Count == 1 && builder.Slices[0].text.Contains("37/200"), "hydroreed durability missing");
        });
        Test("hydroreed filter is used when no better filter exists", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); var hydroreed = Item("hydroreed_filter", inner, 90);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == hydroreed && GraphUtils.Swaps == 1, "hydroreed filter not used as replacement");
        });
        Test("standard filter is preferred over hydroreed filter", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box();
            Item("hydroreed_filter", inner, 90);
            var standard = Item("water_filter", inner, 20); standard.modifiedShape.minX = 2;
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == standard, "hydroreed outranked a standard filter");
        });
        Test("depleted hydroreed filter is replaced", () => {
            var (jug, _) = JugOf("hydroreed_filter", 0); var (_, inner) = Box(); var spare = Item("water_filter", inner, 90);
            Frames();
            Check(jug.FilterSlot!.childItem == spare && GraphUtils.Swaps == 1, "depleted hydroreed not replaced");
        });
        Test("coarse chemistry cloth shows durability but is not installed", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box();
            var cloth = Item("kotton_fabric", inner, 100);
            cloth.state.Set("WATER_FILTER"); cloth.state.Set("FILTER_SIZE_TAG", 150);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "coarse cloth installed as replacement");
            var (jug2, coarse) = JugOf("kotton_fabric", 100); coarse.MaxDurability = 1000;
            coarse.state.Set("WATER_FILTER"); coarse.state.Set("FILTER_SIZE_TAG", 150);
            var builder = new RichTextBuilder();
            Hooks.AfterWaterContainerTooltip(builder, jug2);
            Check(builder.Slices.Count == 1 && builder.Slices[0].text.Contains("100/1000"), "coarse chemistry cloth durability missing");
        });
        Test("UV lamp is neither shown as filter nor installed", () => {
            var (jug, lamp) = JugOf("uv_filter", 50); lamp.MaxDurability = 400;
            var builder = new RichTextBuilder();
            Hooks.AfterWaterContainerTooltip(builder, jug);
            Check(builder.Slices.Count == 0, "UV lamp shown as filter durability");
            var (other, old) = Jug(10); var (_, inner) = Box();
            Item("uv_filter", inner, 50);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(other.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "UV lamp installed as replacement");
        });
        Test("filter slotted in a RAS machine is not used as a spare", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); var (_, slot) = Ras();
            Item("water_filter_adv", slot, 100);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "RAS machine filter was taken as a spare");
        });
        Test("depleted filter slotted in a RAS machine is not replaced", () => {
            var (_, slot) = Ras(); var rasFilter = Item("water_filter", slot, 0);
            var (jug, old) = Jug(10); var (_, inner) = Box(); var spare = Item("water_filter", inner, 90);
            Frames(); Hooks.AfterFilter(rasFilter); Frames();
            Check(slot.childItem == rasFilter && jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "RAS machine filter was swapped");
            Check(spare.parentInventory == inner, "RAS machine filter consumed the spare");
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
        Test("chemical scanner is accepted as a water scanner", () => {
            Frames(); var scan=Item("chem_scanner",EmporiumEntry.Instance.invWindow); Hooks.AfterItemParentChanged(scan,true); Frames();
            var cache=typeof(Hooks).GetField("_scannerCache",BindingFlags.Static|BindingFlags.NonPublic)!;
            Check((bool)cache.GetValue(null)!,"chemical scanner addition not found");
            EmporiumEntry.Instance.invWindow.childItems.Remove(scan); scan.Parent=null; Hooks.AfterItemParentChanged(scan,true); Frames();
            Check(!(bool)cache.GetValue(null)!,"chemical scanner removal not found");
        });
        Test("scan limits per frame", () => {
            var root=EmporiumEntry.Instance.invWindow; for (int i=0;i<500;i++) Item("object_"+i,root);
            Hooks.UpdateMaintenance(true); Check(typeof(Hooks).GetField("_maintenance",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!=null,"scan completed in one frame");
            Frames(100); Check(PlayerStore.Instance!.RootCalls==0,"unbounded inventory query");
        });
        Console.WriteLine($"{passed} scenarios passed");
    }
}


