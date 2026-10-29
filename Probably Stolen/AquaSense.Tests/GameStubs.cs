// Test doubles for boundary/fault injection only. Native geometry/IL2CPP timing
// must be checked in the game; this assembly never loads the game's DLLs.
using Il2CppInterop.Runtime.InteropTypes;
using NativeList = Il2CppSystem.Collections.Generic.List<Il2Cpp.GameItem>;

namespace Il2CppInterop.Runtime.InteropTypes
{
    public class Il2CppObjectBase
    {
        private static long _next;
        public IntPtr Pointer { get; } = (IntPtr)Interlocked.Increment(ref _next);
        public T? TryCast<T>() where T : class => this as T;
    }
}
namespace Il2CppSystem.Collections.Generic { public class List<T> : System.Collections.Generic.List<T> { } }
namespace UnityEngine { public static class Time { public static float unscaledTime; public static int frameCount; } }
namespace HarmonyLib
{
    public sealed class Harmony { public Harmony(string id) {} public void Patch(object method, HarmonyMethod postfix) {} }
    public sealed class HarmonyMethod { public HarmonyMethod(object method) {} }
    public static class AccessTools { public static object Method(Type type, string name, params Type[] args) => new object(); }
}
namespace MelonLoader
{
    [AttributeUsage(AttributeTargets.Assembly)] public sealed class MelonInfoAttribute : Attribute { public MelonInfoAttribute(Type t,string n,string v,string a) {} }
    [AttributeUsage(AttributeTargets.Assembly)] public sealed class MelonGameAttribute : Attribute { public MelonGameAttribute(string d,string n) {} }
    public class MelonMod
    {
        public MelonLogger.Instance LoggerInstance = new();
        public virtual void OnInitializeMelon() {} public virtual void OnLateInitializeMelon() {}
        public virtual void OnUpdate() {} public virtual void OnDeinitializeMelon() {}
    }
    public static class MelonLogger { public sealed class Instance { public readonly List<string> Warnings = new(); public void Warning(string message) => Warnings.Add(message); } }
    public sealed class MelonPreferences_Entry<T> { public T Value; public MelonPreferences_Entry(T value) {Value=value;} }
    public sealed class Category { public MelonPreferences_Entry<T> CreateEntry<T>(string k,T v,string label) => new(v); }
    public static class MelonPreferences { public static Category CreateCategory(string k,string n) => new(); }
}
namespace Il2Cpp
{
    public interface GraphNodeStorage { List<Il2CppObjectBase> children { get; } }
    public interface GraphNodeWindow { Il2CppObjectBase? child { get; } }
    public sealed class Window : Il2CppObjectBase, GraphNodeWindow { public Il2CppObjectBase? child { get; set; } }
    public sealed class Storage : Il2CppObjectBase, GraphNodeStorage { public List<Il2CppObjectBase> children { get; } = new(); }
    public sealed class TagState { public int valueInt; }
    public sealed class TagSystem
    {
        public readonly Dictionary<string,TagState> Tags = new();
        public bool IsTag(string key) => Tags.ContainsKey(key);
        public TagState? GetTag(string key) => Tags.GetValueOrDefault(key);
        public void Set(string key,int value=1) => Tags[key]=new TagState{valueInt=value};
    }
    public class GridShape : Il2CppObjectBase
    {
        public int minX,minY,orientation; public bool flipped;
        public int width=1,height=1;
        public HashSet<(int X,int Y)> Cells = new() {(0,0)};
        public int globalWidth => orientation%2==0 ? width : height;
        public int globalHeight => orientation%2==0 ? height : width;
        public int maxX => minX+globalWidth-1; public int maxY => minY+globalHeight-1;
        public GridShape Clone() => new() {minX=minX,minY=minY,orientation=orientation,flipped=flipped,width=width,height=height,Cells=new(Cells)};
        public IEnumerable<(int X,int Y)> GlobalCells()
        {
            foreach(var cell in Cells)
            {
                var x=flipped ? width-1-cell.X : cell.X; var y=cell.Y;
                var (tx,ty)=(orientation%4) switch {1=>(height-1-y,x),2=>(width-1-x,height-1-y),3=>(y,width-1-x),_=>(x,y)};
                yield return (minX+tx,minY+ty);
            }
        }
    }
    public sealed class GridShapeBuilder
    {
        private readonly GridShape _shape;
        public GridShapeBuilder(GridShape shape) {_shape=shape.Clone();}
        public GridShapeBuilder SetTransform(GridShape other) {_shape.minX=other.minX;_shape.minY=other.minY;_shape.orientation=other.orientation;_shape.flipped=other.flipped;return this;}
        public GridShape Build()=>_shape.Clone();
    }
    public class GameItem : Il2CppObjectBase, GraphNodeStorage
    {
        private string _name=""; public int NameReads;
        public string identifier=""; public string name { get {NameReads++;return _name;} set {_name=value;} }
        public TagSystem state=new(); public TagSystem modifiedState=>state;
        public GridShape modifiedShape=new(); public GridShape shape=>modifiedShape;
        public int modifiedXOrigin=100,modifiedYOrigin=100; // Deliberately not position.
        public int unitCount=1,Durability=10,MaxDurability=100;
        public bool ThrowParent,ThrowDurability,ThrowFilterRead,RejectState;
        public GameInventory? Parent;
        public GameInventory? parentInventory { get {if(ThrowParent)throw new InvalidOperationException("parent fault");return Parent;} }
        public Il2CppObjectBase? child;
        public List<Il2CppObjectBase> children {get;} = new();
        public GameSlotInventory? FilterSlot;
        public string GetDisplayName(bool unused)=>string.IsNullOrEmpty(_name)?identifier:_name;
        public bool CheckIsStateValid(GameInventory inv,SlotMarker marker,GridShape shape,TagSystem state)=>!RejectState;
        public void Destroy() {} public void Validate() {}
    }
    public sealed class GameItemElement : GameItem { public RichTextBuilder GetTooltipBasic()=>new(); }
    public class GameInventory : Il2CppObjectBase, GraphNodeStorage, GraphNodeWindow
    {
        public GameItem? ParentItem; public bool InsertLocked,RemoveLocked,Reject;
        public int NativeShapeChecks; public Func<GameItem,bool>? Accepts;
        public List<Il2CppObjectBase> children {get;} = new(); public Il2CppObjectBase? child=>null;
        public NativeList childItems=new(); public int Width=100,Height=100;
        public HashSet<(int X,int Y)>? AllowedCells;
        public GameItem? GetParentItem()=>ParentItem;
        public bool IsInsertLocked()=>InsertLocked; public bool IsRemoveLocked()=>RemoveLocked;
        public bool MayHaveValidInventorySlot(GameItem item)=>!Reject && !InsertLocked && (Accepts?.Invoke(item)??true);
        public bool CheckAcceptParentShapeStateModifier(GameItem item,SlotMarker marker,GridShape shape,TagSystem state)=>!Reject;
        public bool CheckShapePlacement(GameItem item,GridShape shape,NativeList ignored)
        {
            NativeShapeChecks++;
            if(!MayHaveValidInventorySlot(item))return false;
            var cells=shape.GlobalCells().ToHashSet();
            if(cells.Any(p=>p.X<0||p.Y<0||p.X>=Width||p.Y>=Height||(AllowedCells!=null&&!AllowedCells.Contains(p))))return false;
            return childItems.Where(x=>!ignored.Contains(x)).All(x=>!x.modifiedShape.GlobalCells().Any(cells.Contains));
        }
        public bool CheckInventorySlotStillValid(GameItem item,GridShape shape,TagSystem state)
        {
            if(this is GameSlotInventory slot)return MayHaveValidInventorySlot(item) && (slot.childItem==null||slot.childItem==item);
            return CheckShapePlacement(item,shape,new NativeList{item});
        }
        public void Put(GameItem item)
        {
            item.Parent?.childItems.Remove(item);
            if(item.Parent is GameSlotInventory old && old.childItem==item)old.childItem=null;
            childItems.Add(item);item.Parent=this;
            if(this is GameSlotInventory slot)slot.childItem=item;
        }
        public void Validate() {}
    }
    public sealed class GameSlotInventory : GameInventory { public GameItem? childItem; }
    public sealed class GameGridInventory : GameInventory { public NativeList items=>childItems; }
    public sealed class GameGridScrollableInventory : GameInventory { public NativeList items=>childItems; }
    public sealed class SlotMarker { public GameItem item=null!;public GameInventory inventory=null!;public GridShape itemGridShape=null!;public int numTransfer; }
    public sealed class PlayerStore : Il2CppObjectBase
    {
        public static PlayerStore? Instance=new(); public NativeList Roots=new(),Repo=new(); public int RootCalls;
        public static bool IsInstanceExist()=>Instance!=null;
        public NativeList FindAllItem(bool own) {RootCalls++;return Roots;}
        public NativeList GetAllRepoEligibleItems()=>Repo;
        public void LoadGame() {}
    }
    public sealed class EmporiumEntry
    {
        public static EmporiumEntry Instance=new(); public GameSlotInventory swapBufferElement=new();
    }
    public sealed class ItemMouseDragHandler { public static ItemMouseDragHandler? current;public bool IsDraggingItem;public GameItem? Held;public GameItem? GetCurrentItem()=>Held; }
    public static class GraphUtils
    {
        public static int Swaps,Placements; public static Func<GameItem,GameItem,bool>? SwapFault;
        public static Func<GameItem,GameInventory,GridShape,bool>? PlaceFault;
        public static void Detach(GameItem item)
        {
            if(item.Parent is GameSlotInventory slot&&slot.childItem==item)slot.childItem=null;
            item.Parent?.childItems.Remove(item);item.Parent=null;
        }
        public static bool PlaceAtShape(GameItem item,GameInventory inventory,GridShape shape)
        {
            Placements++;
            if(PlaceFault!=null)return PlaceFault(item,inventory,shape);
            if(!inventory.CheckInventorySlotStillValid(item,shape,item.state))return false;
            inventory.Put(item);item.modifiedShape=new GridShapeBuilder(item.modifiedShape).SetTransform(shape).Build();return true;
        }
        public static bool SwapItemsInPlace(GameItem a,GameItem b)
        {
            Swaps++;
            if(SwapFault!=null)return SwapFault(a,b);
            var target=a.Parent!;var source=b.Parent!;var sa=a.modifiedShape.Clone();var sb=b.modifiedShape.Clone();
            Detach(a);Detach(b);target.Put(b);source.Put(a);
            b.modifiedShape=new GridShapeBuilder(b.modifiedShape).SetTransform(sa).Build();
            a.modifiedShape=new GridShapeBuilder(a.modifiedShape).SetTransform(sb).Build();return true;
        }
    }
    public static class DurabilityHelper
    {
        public static int Reads;
        public static int GetRealDurability(GameItem item) {Reads++;if(item.ThrowDurability)throw new InvalidOperationException("durability fault");return item.Durability;}
        public static int GetMaxDurability(GameItem item)=>item.MaxDurability;
    }
    public static class WaterHelper
    {
        public static Func<GameItem,bool>? Capability;
        public static GameItem? GetFilterItem(GameItem item) {if(item.ThrowFilterRead)throw new InvalidOperationException("filter fault");return item.FilterSlot?.childItem;}
        public static bool CanUseWaterContainer(GameItem item)=>Capability?.Invoke(item)??false;
        public static bool IsEmpty(GameItem item)=>!Liquid.Liquids.Any(l=>(item.state.GetTag(l.currentPart)?.valueInt??0)>0);
        public static int GetWaterPurity(GameItem item)=>10000;
        public static string GetLiquidContainerColor(GameItem item)=>"";
        public static void UpdateContainerGoodType(GameItem item) {}
        public static void TransferLiquid(GameItem a,GameItem b) {}
        public static void CreateWaterContainerTooltip(RichTextBuilder b,GameItem i) {}
    }
    public static class WaterFeatureHelper
    {
        public static string GetWaterDisplayName(GameItem item)=>"water";
        public static string FormatPurityPercentageFromDouble(double p)=>$"{p:0.00}%";
        public static void UpdateWaterFeature(GameItem i,bool b) {} public static void OnWaterLabelUpdated(GameItem i) {}
        public static void OnLiquidContainerPourInto(GameItem a,GameItem b,bool c) {}
        public static void CreateWaterFeatureTooltip(RichTextBuilder b,GameItem i) {}
    }
    public static class PreBuiltItemHelper {public static GameItem WaterFilter()=>new(){identifier="water_filter",name="Water Filter"};}
    public static class LocHelper {public static string Locale="zh";public static string GetCurrentLocaleCode()=>Locale;}
    public sealed class Liquid { public static Dictionary<string,Liquid>? LiquidDict;public static List<Liquid> Liquids=new();public string identifier="water",displayName="water",currentPart="water",part="water"; }
    public sealed class RichTextBuilder : Il2CppObjectBase
    {
        public sealed class TextSlice : Il2CppObjectBase {public string text="";}
        public List<TextSlice> slices=new();
        public void Clear()=>slices.Clear();
        public void AddLine(string text,bool ignoreEmpty,RenderHandler.ColorPalette color,bool bold,bool italic,bool underline,bool strikethrough,RenderHandler.ColorPalette background,RenderHandler.ColorPalette uc,RenderHandler.ColorPalette sc)=>slices.Add(new(){text=text});
    }
    public static class RenderHandler {public enum ColorPalette {None=-1}}
    public sealed class StoreUIManager {public static StoreUIManager Instance=new();public List<string> Messages=new();public void Notify(string message,string color)=>Messages.Add(message);}
}
