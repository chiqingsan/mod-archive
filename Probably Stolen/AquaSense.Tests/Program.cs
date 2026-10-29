using System.Collections;
using System.Reflection;
using AquaSense;
using Il2Cpp;
using UnityEngine;

internal static class Program
{
    private static readonly Core Mod = new();
    private static int _passed;

    private static object? Call(string name, params object?[] arguments) =>
        typeof(Hooks).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, arguments);
    private static T Field<T>(string name) => (T)typeof(Hooks).GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
    private static void Set(string name, object value) => typeof(Hooks).GetField(name, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, value);
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
    private static void Test(string name, Action action)
    {
        Reset();
        try { action(); _passed++; Console.WriteLine($"PASS {name}"); }
        catch (Exception ex) { throw new InvalidOperationException($"FAIL {name}", ex); }
    }
    private static void Reset()
    {
        Hooks.ResetMaintenance();
        PlayerStore.Instance = new(); EmporiumEntry.Instance = new(); ItemMouseDragHandler.current = null;
        GraphUtils.Swaps = GraphUtils.Placements = 0; GraphUtils.SwapFault = null; GraphUtils.PlaceFault = null;
        DurabilityHelper.Reads = 0; WaterHelper.Capability = null;
        Liquid.Liquids.Clear(); Liquid.LiquidDict = null; Time.unscaledTime = 0; Time.frameCount++;
        StoreUIManager.Instance.Messages.Clear(); Mod.LoggerInstance.Warnings.Clear();
        Core.AutoReplaceFilters!.Value = true; Core.ShowFilterDurability!.Value = true; Core.AutoUpdateWaterLabels!.Value = true;
        Set("_storePointer", PlayerStore.Instance.Pointer);
    }
    private static GameItem Item(string id, GameInventory? inventory = null, int x = 0, int y = 0)
    {
        var item = new GameItem { identifier = id, modifiedShape = new GridShape { minX = x, minY = y } };
        inventory?.Put(item); return item;
    }
    private static GameItem Water(out GameItem old)
    {
        var container = Item("water_jug");
        var slot = new GameSlotInventory { ParentItem = container };
        container.FilterSlot = slot; container.child = new Window { child = slot };
        old = Item("water_filter", slot); old.Durability = 0;
        PlayerStore.Instance!.Roots.Add(container); return container;
    }
    private static GameItem Spare(GameInventory? inventory = null, string id = "water_filter", int x = 5)
    {
        var spare = Item(id, inventory ?? new GameGridInventory(), x);
        PlayerStore.Instance!.Roots.Add(spare); return spare;
    }
    private static int Pass(bool replace = true)
    {
        var count = 0;
        foreach (var step in (IEnumerable<object?>)Call("MaintainFilters", PlayerStore.Instance!, replace)!) count++;
        return count;
    }
    private static bool Swap(GameItem water, GameItem old, GameItem spare)
    {
        object?[] args = {water, old, spare, null};
        return (bool)Call("ReplaceFilter", args)!;
    }
    private static bool IsWater(GameItem item) => (bool)Call("IsWaterContainer", item)!;
    private static bool Available(GameItem item) => (bool)Call("IsAvailableFilter", item)!;
    private static bool CanSwap(GameItem old, GameItem spare) => (bool)Call("CanSwapFilterPlacement", old, spare)!;

    public static void Main()
    {
        Mod.OnInitializeMelon();
        foreach (var n in new[] {0, 1, 100, 1000, 10000})
        {
            Test($"no water / {n} spares: no spare durability or priority scan", () =>
            {
                var inv = new GameGridInventory();
                for (var k = 0; k < n; k++) Spare(inv, x: k);
                var steps = Pass();
                Check(DurabilityHelper.Reads == 0 && GraphUtils.Swaps == 0, "unnecessary candidate scan");
                Check(steps >= n, "inventory enumeration is not resumable");
            });
        }
        Test("healthy container / 10000 spares: only installed durability read", () =>
        {
            Water(out var old); old.Durability = 1;
            var inventory = new GameGridInventory();
            for (var k = 0; k < 10000; k++) Spare(inventory, x: k);
            Pass(); Check(DurabilityHelper.Reads == 1 && GraphUtils.Swaps == 0, "healthy inventory scanned spares");
        });
        foreach (var remove in new[] {false, true})
        Test($"locked {(remove ? "removal" : "insertion")} first candidate falls through", () =>
        {
            var water = Water(out var old);
            var inv = new GameGridInventory { InsertLocked = !remove, RemoveLocked = remove };
            Spare(inv, "water_filter_adv"); var good = Spare();
            Pass(); Check(water.FilterSlot!.childItem == good && GraphUtils.Swaps == 1, "candidate fallback blocked");
        });
        Test("first candidate cannot fit old shape: use next candidate", () =>
        {
            var water = Water(out var old); old.modifiedShape.width = 2; old.modifiedShape.Cells.Add((1,0));
            Spare(new GameGridInventory { Width = 1 }, "water_filter_adv", 0); var good = Spare();
            Pass(); Check(water.FilterSlot!.childItem == good, "oversize candidate prevented fallback");
        });
        foreach (var value in new[] {int.MinValue,-1,0,1,int.MaxValue})
        Test($"durability boundary {value}", () =>
        {
            var water = Water(out var old); old.Durability = value; var spare = Spare(); Pass();
            Check(water.FilterSlot!.childItem == (value <= 0 ? spare : old), "wrong durability decision");
        });
        Test("durability read error does not trigger swap", () =>
        {
            var water = Water(out var old); old.ThrowDurability = true; Spare(); Pass();
            Check(water.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "failed read interpreted as zero");
        });
        Test("UV lamp and industrial cloth are never replaced", () =>
        {
            var water = Water(out var old); Spare();
            foreach (var id in new[] {"uv_filter","hydroreed_filter","black_lamp"})
            {old.identifier = id; Pass(); Check(water.FilterSlot!.childItem == old, id);}
        });
        Test("no spare keeps old filter; empty slot remains empty", () =>
        {
            var water = Water(out var old); Pass(); Check(water.FilterSlot!.childItem == old, "old lost");
            GraphUtils.Detach(old); Spare(); Pass(); Check(water.FilterSlot.childItem == null, "empty slot filled");
        });
        Test("two containers / one spare: installed filter not stolen", () =>
        {
            var a = Water(out var oldA); var b = Water(out var oldB); var spare = Spare(); Pass();
            Check(a.FilterSlot!.childItem == spare && b.FilterSlot!.childItem == oldB && GraphUtils.Swaps == 1, "spare reused");
        });
        Test("same ID different tags and runtime mutation do not share capability", () =>
        {
            var a = Item("new_mod_water"); var b = Item("new_mod_water");
            b.state.Set("LIQUID_CONTAINER_TAG"); b.state.Set("LIQUID_CONTAINER_CAPACITY",100);
            Check(!IsWater(a) && IsWater(b) && !IsWater(a), "instance capability leaked");
            a.state.Set("LIQUID_CONTAINER_TAG"); a.state.Set("LIQUID_CONTAINER_CAPACITY",1);
            Check(IsWater(a), "runtime tag change cached");
        });
        Test("native capability fallback still supports untagged containers", () =>
        {
            var a = Item("native_only_water"); WaterHelper.Capability = x => x == a;
            Check(IsWater(a), "native-only container omitted");
        });
        Test("known water container without FILTER_CONTAINER_TAG still serviced", () =>
        {
            var water = Water(out var old); water.identifier = "watertower_tank"; var spare = Spare(); Pass();
            Check(water.FilterSlot!.childItem == spare, "new tag gate excludes existing water slot");
        });
        foreach (var depth in new[] {1,15,16,17,100,1000})
        Test($"nested inventory depth {depth}", () =>
        {
            GameInventory inv = new GameGridInventory(); var root = Item("bag"); PlayerStore.Instance!.Roots.Add(root);
            for (var k = 0; k < depth; k++)
            {
                inv = new GameGridInventory { ParentItem = root }; root.child = new Window { child = inv };
                root = Item("bag", inv);
            }
            var spare = Item("water_filter", inv); Check(Available(spare), "artificial depth cutoff");
            var enumerated = ((IEnumerable<GameItem?>)Call("EnumerateOwnedItems", PlayerStore.Instance!)!).Where(x=>x!=null).ToList();
            Check(enumerated.Contains(spare) && enumerated.Count == depth + 2, "nested item coverage or duplicate");
        });
        Test("graph and ancestry cycles terminate without recursion", () =>
        {
            var inv = new GameGridInventory(); var bag = Item("bag",inv); inv.ParentItem = bag;
            bag.child = new Window { child = inv }; bag.children.Add(inv); PlayerStore.Instance!.Roots.Add(bag);
            var spare = Item("water_filter",inv); Check(!Available(spare), "cyclic parent selected");
            Check(((IEnumerable<GameItem?>)Call("EnumerateOwnedItems", PlayerStore.Instance!)!).Count() < 30, "graph cycle");
        });
        Test("repo / grid / scrollable / window paths are deduplicated", () =>
        {
            var bag = Item("bag"); var scroll = new GameGridScrollableInventory { ParentItem=bag };
            var spare = Item("water_filter",scroll); bag.child = new Window{child=scroll};bag.children.Add(scroll);
            PlayerStore.Instance!.Roots.Add(bag); PlayerStore.Instance.Repo.Add(spare);
            var items = ((IEnumerable<GameItem?>)Call("EnumerateOwnedItems", PlayerStore.Instance!)!).Where(x=>x!=null).ToList();
            Check(items.Count==2 && items.Contains(spare), "repo duplicates or omission");
        });
        Test("parent read error is not treated as detached", () =>
        {
            var water = Water(out var old); var spare = Spare(); var source = spare.Parent;
            spare.ThrowParent = true;
            Check(!Swap(water,old,spare) && spare.Parent==source && GraphUtils.Swaps==0 && GraphUtils.Placements==0,"moved unreadable parent");
        });
        foreach (var orientation in new[] {0,1,2,3})
        foreach (var flipped in new[] {false,true})
        Test($"native shape placement transform rotation={orientation} flipped={flipped}", () =>
        {
            Water(out var old); var inv = new GameGridInventory(); var spare = Spare(inv,x:7);
            spare.modifiedShape.minY=9; spare.modifiedShape.orientation=orientation; spare.modifiedShape.flipped=flipped;
            Check(CanSwap(old,spare) && inv.NativeShapeChecks>0,"native check bypassed or origin added twice");
        });
        Test("irregular shape fits adjacent item without bounding-box rejection", () =>
        {
            Water(out var old); old.modifiedShape.width=old.modifiedShape.height=2;
            old.modifiedShape.Cells=new(){(0,0),(0,1),(1,1)};
            var inv=new GameGridInventory();var spare=Spare(inv,x:5);Item("neighbor",inv,6,0);
            Check(CanSwap(old,spare),"bounding-box false collision");
            Item("real_collision",inv,5,1);Check(!CanSwap(old,spare),"native cell overlap ignored");
        });
        Test("inventory holes and right edge rejected by native checks", () =>
        {
            Water(out var old);var inv=new GameGridInventory{Width=1};var spare=Spare(inv,x:1);
            Check(!CanSwap(old,spare),"outside edge accepted");
            spare.modifiedShape.minX=0;inv.AllowedCells=new();Check(!CanSwap(old,spare),"inventory hole accepted");
        });
        Test("slot category and item-state rejection prevent native swap", () =>
        {
            var water=Water(out var old);var spare=Spare();water.FilterSlot!.Accepts=_=>false;
            Check(!CanSwap(old,spare),"slot category ignored");water.FilterSlot.Accepts=null;
            spare.RejectState=true;Check(!CanSwap(old,spare),"item state ignored");
        });
        Test("dragging or occupied native swap buffer defers automatic replacement", () =>
        {
            Water(out var old);var spare=Spare();ItemMouseDragHandler.current=new(){IsDraggingItem=true};
            Check(!CanSwap(old,spare),"drag altered");ItemMouseDragHandler.current=null;
            Item("other",EmporiumEntry.Instance.swapBufferElement);Check(!CanSwap(old,spare),"swap buffer stolen");
        });
        Test("advanced ID has priority independent of name and locale", () =>
        {
            foreach(var name in new[]{"","未知名称","Unbekannt"})
            {var item=Item("water_filter_adv");item.name=name;Check((int)Call("GetFilterPriority",item)! == 0,"advanced priority");}
        });
        for(var stage=0;stage<6;stage++)
        {
            var captured=stage;
            Test($"native partial-swap recovery stage {stage}", () =>
            {
                var water=Water(out var old);var spare=Spare(x:12);var target=old.Parent!;var source=spare.Parent!;
                GraphUtils.SwapFault=(a,b)=>
                {
                    switch(captured)
                    {
                        case 1: GraphUtils.Detach(a);break;
                        case 2: EmporiumEntry.Instance.swapBufferElement.Put(a);break;
                        case 3: EmporiumEntry.Instance.swapBufferElement.Put(a);target.Put(b);break;
                        case 4: source.Put(a);GraphUtils.Detach(b);break;
                        case 5: GraphUtils.Detach(a);target.Put(b);break;
                    }
                    return false;
                };
                Check(!Swap(water,old,spare),"failed swap reported success");
                Check(target==old.Parent && water.FilterSlot!.childItem==old && source==spare.Parent && spare.modifiedShape.minX==12,"original placements not restored");
                Check(EmporiumEntry.Instance.swapBufferElement.childItem==null,"native buffer leaked item");
            });
        }
        Test("successful verified swap accepted even when native return is false", () =>
        {
            var water=Water(out var old);var spare=Spare();var target=old.Parent!;var source=spare.Parent!;
            GraphUtils.SwapFault=(a,b)=>{target.Put(b);source.Put(a);a.modifiedShape.minX=5;b.modifiedShape.minX=0;return false;};
            Check(Swap(water,old,spare)&&water.FilterSlot!.childItem==spare,"verified end state discarded");
        });
        Test("unrecoverable native failure retains references and retries", () =>
        {
            var water=Water(out var old);var spare=Spare();
            GraphUtils.SwapFault=(a,b)=>{GraphUtils.Detach(a);return false;};GraphUtils.PlaceFault=(_,_,_)=>false;
            Check(!Swap(water,old,spare)&&Field<Func<bool>?>("_retryFilterRecovery")!=null,"unplaced item references lost");
            GraphUtils.PlaceFault=null;Time.unscaledTime=10;Hooks.UpdateMaintenance(true);
            Check(water.FilterSlot!.childItem==old&&Field<Func<bool>?>("_retryFilterRecovery")==null,"pending recovery not retried");
        });
        Test("Tooltip A/B/A and clear/reuse do not duplicate or suppress content", () =>
        {
            Set("_scannerCache",true);Set("_scannerCacheExpiresAt",10f);
            var water=Water(out var old);old.Durability=50;var a=new RichTextBuilder();var b=new RichTextBuilder();
            Hooks.AfterWaterContainerTooltip(a,water);Hooks.AfterWaterContainerTooltip(b,water);Hooks.AfterWaterContainerTooltip(a,water);
            Check(a.slices.Count==1&&b.slices.Count==1,"A/B/A duplicated");
            a.Clear();Hooks.AfterWaterContainerTooltip(a,water);Check(a.slices.Count==1,"cleared builder suppressed");
        });
        Test("large water composition sum does not overflow", () =>
        {
            Set("_scannerCache",true);Set("_scannerCacheExpiresAt",10f);var water=Item("water_jug");
            Liquid.Liquids.Add(new(){displayName="a",currentPart="a"});Liquid.Liquids.Add(new(){displayName="b",currentPart="b"});
            water.state.Set("a",1500000000);water.state.Set("b",1500000000);var builder=new RichTextBuilder();
            Hooks.AfterWaterContainerTooltip(builder,water);
            Check(builder.slices.Count==3&&builder.slices.Count(x=>x.text.Contains("50.00%"))==2,"composition overflow");
        });
        Test("Tooltip never performs synchronous inventory scan", () =>
        {
            var water=Water(out _);Hooks.AfterWaterContainerTooltip(new RichTextBuilder(),water);
            Check(PlayerStore.Instance!.RootCalls==0&&Field<bool>("_inventoryRefreshRequested"),"Tooltip blocks on scan");
        });
        Test("scanner cache refresh works with auto replacement off", () =>
        {
            Core.AutoReplaceFilters!.Value=false;PlayerStore.Instance!.Roots.Add(Item("aquascan"));
            var water=Water(out _);Hooks.AfterWaterContainerTooltip(new RichTextBuilder(),water);
            for(var i=0;i<100&&(Field<bool>("_inventoryRefreshRequested")||Field<object?>("_maintenance")!=null);i++)Mod.OnUpdate();
            Check(Field<bool>("_scannerCache"),"scanner disabled with maintenance switch");
            PlayerStore.Instance=new();Time.unscaledTime=20;Set("_inventoryRefreshRequested",true);Mod.OnUpdate();
            Check(!Field<bool>("_scannerCache"),"scanner cache leaked across stores");
        });
        Test("turning auto replacement off while a pass is yielded prevents swap", () =>
        {
            Water(out _);Spare();var iterator=((IEnumerable<object?>)Call("MaintainFilters",PlayerStore.Instance!,true)!).GetEnumerator();
            iterator.MoveNext();Core.AutoReplaceFilters!.Value=false;while(iterator.MoveNext()){}iterator.Dispose();
            Check(GraphUtils.Swaps==0,"disabled feature still swapped");
        });
        Test("frame budget yields control during expensive native work", () =>
        {
            for(var i=0;i<100;i++)PlayerStore.Instance!.Roots.Add(Item("unknown"));
            WaterHelper.Capability=_=>{Thread.Sleep(3);return false;};
            Mod.OnUpdate();Check(Field<object?>("_maintenance")!=null,"whole expensive pass blocked one update");
            Check(PlayerStore.Instance!.RootCalls==1,"roots repeatedly scanned within pass");
        });
        Test("sold candidate is skipped using live owned roots", () =>
        {
            var water=Water(out var old);var sold=Spare(id:"water_filter_adv");var good=Spare();
            PlayerStore.Instance!.Roots.Remove(sold);
            Check(!Swap(water,old,sold)&&GraphUtils.Swaps==0,"unowned spare moved");
            Check(Swap(water,old,good),"owned fallback rejected");
        });
        Test("sold container cannot be modified from a stale snapshot", () =>
        {
            var water=Water(out var old);var spare=Spare();PlayerStore.Instance!.Roots.Remove(water);
            Check(!Swap(water,old,spare)&&GraphUtils.Swaps==0,"sold container modified");
        });
        Test("reload with same PlayerStore clears pending work and scanner", () =>
        {
            Set("_scannerCache",true);Set("_inventoryRefreshRequested",true);
            Set("_retryFilterRecovery",(Func<bool>)(()=>false));Hooks.AfterLoadGame();
            Check(!Field<bool>("_scannerCache")&&Field<object?>("_maintenance")==null&&Field<Func<bool>?>("_retryFilterRecovery")==null,"same-instance reload retained old world");
        });
        Test("live inventory edits do not invalidate a yielded enumerator", () =>
        {
            var bag=Item("bag");var inv=new GameGridInventory{ParentItem=bag};bag.child=new Window{child=inv};
            PlayerStore.Instance!.Roots.Add(bag);for(var i=0;i<10;i++)Item("item",inv,i);
            var iterator=((IEnumerable<GameItem?>)Call("EnumerateOwnedItems",PlayerStore.Instance)!).GetEnumerator();
            var steps=0;while(iterator.MoveNext())
            {
                if(++steps==8) {inv.childItems.Clear();Item("new_item",inv);}
                Check(steps<100,"mutation scan failed to terminate");
            }
            iterator.Dispose();
        });
        foreach(var remove in new[]{false,true})
        Test($"target {(remove?"removal":"insertion")} lock prevents mutation", () =>
        {
            var water=Water(out var old);var spare=Spare();
            water.FilterSlot!.InsertLocked=!remove;water.FilterSlot.RemoveLocked=remove;
            Pass();Check(GraphUtils.Swaps==0&&water.FilterSlot.childItem==old,"target lock ignored");
        });
        Test("native exception after detaching item recovers original placement", () =>
        {
            var water=Water(out var old);var spare=Spare();
            GraphUtils.SwapFault=(a,b)=>{GraphUtils.Detach(a);throw new InvalidOperationException("native swap fault");};
            Check(!Swap(water,old,spare)&&water.FilterSlot!.childItem==old&&spare.Parent!=null,"exception lost item");
        });
        Test("occupied original position is not overwritten during recovery", () =>
        {
            var water=Water(out var old);var spare=Spare();var source=spare.Parent!;GameItem? obstruction=null;
            GraphUtils.SwapFault=(a,b)=>{GraphUtils.Detach(a);GraphUtils.Detach(b);obstruction=Item("obstruction",source,5);return false;};
            Check(!Swap(water,old,spare)&&water.FilterSlot!.childItem==old&&obstruction!.Parent==source&&spare.Parent==null,"recovery overwrote another item");
            Check(Field<Func<bool>?>("_retryFilterRecovery")!=null,"detached spare forgotten");
            GraphUtils.Detach(obstruction!);Time.unscaledTime=10;Hooks.UpdateMaintenance(true);
            Check(spare.Parent==source&&spare.modifiedShape.minX==5,"original position not restored after obstruction cleared");
        });
        Test("scanner not owned: Tooltip remains unmodified", () =>
        {
            var water=Water(out var old);var builder=new RichTextBuilder();
            Hooks.AfterWaterContainerTooltip(builder,water);Check(builder.slices.Count==0,"scanner requirement bypassed");
        });
        Test("durability display toggle is honored", () =>
        {
            Set("_scannerCache",true);Set("_scannerCacheExpiresAt",10f);Core.ShowFilterDurability!.Value=false;
            var water=Water(out _);var builder=new RichTextBuilder();Hooks.AfterWaterContainerTooltip(builder,water);
            Check(builder.slices.Count==0,"durability toggle ignored");
        });
        Test("scanner warms up before first hover with replacement disabled", () =>
        {
            Core.AutoReplaceFilters!.Value=false;PlayerStore.Instance!.Roots.Add(Item("aquascan"));
            for(var i=0;i<100&&(!Field<bool>("_scannerCache")||Field<object?>("_maintenance")!=null);i++)Mod.OnUpdate();
            Check(Field<bool>("_scannerCache"),"initial scanner detection waited for hover");
        });
        Console.WriteLine($"{_passed} regression scenarios passed. Native IL2CPP geometry/timing still requires in-game validation.");
    }
}
