global using Il2Cpp;
// Test doubles for boundary/fault injection only. Native geometry/IL2CPP timing
// must be checked in the game; this assembly never loads the game's DLLs.
using Il2CppInterop.Runtime.InteropTypes;
using NativeList = System.Collections.Generic.List<Il2Cpp.GameItem>;

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
namespace UnityEngine {
public static class Time { public static float unscaledTime; public static int frameCount; }
public static class Mathf { public static int CeilToInt(float value) => (int)System.Math.Ceiling(value); }
public class Object { public static void DontDestroyOnLoad(object o){} public static void Destroy(object o){} public static bool operator ==(Object a,Object b)=>ReferenceEquals(a,b); public static bool operator !=(Object a,Object b)=>!ReferenceEquals(a,b); public override bool Equals(object? o)=>base.Equals(o); public override int GetHashCode()=>base.GetHashCode(); }
public class Component : Object {
 public GameObject gameObject = null!;
 public Transform transform => gameObject.transform;
 public T? GetComponent<T>() where T:class => gameObject.GetComponent<T>();
}
public class MonoBehaviour : Component {}
public class GameObject : Object {
 private readonly Dictionary<Type,object> components = new();
 public readonly Transform transform; public bool activeSelf = true; public int layer; public string name = "";
 public GameObject(string name, params Type[] types) { this.name = name; transform = new RectTransform { gameObject = this }; }
 public T AddComponent<T>() where T:new() { var value = new T(); components[typeof(T)] = value; if(value is Component component) component.gameObject = this; return value; }
 public T? GetComponent<T>() where T:class => components.TryGetValue(typeof(T),out var value) ? (T)value : null;
 public void SetActive(bool active) => activeSelf = active;
}
public class Transform : Component {
 public readonly List<Transform> children = new(); public Transform? parent;
 public void SetParent(Transform value, bool worldPositionStays) { parent?.children.Remove(this); parent=value; value.children.Add(this); }
 public void SetAsFirstSibling() { if (parent == null) return; parent.children.Remove(this); parent.children.Insert(0, this); }
 public Vector3 TransformPoint(Vector3 local) { var rt = this as RectTransform; return new Vector3(local.x + (rt?.anchoredPosition.x ?? 0f), local.y + (rt?.anchoredPosition.y ?? 0f), 0f); }
 public Vector3 InverseTransformPoint(Vector3 world) { var rt = this as RectTransform; return new Vector3(world.x - (rt?.anchoredPosition.x ?? 0f), world.y - (rt?.anchoredPosition.y ?? 0f), 0f); }
}
public class RectTransform : Transform {
 public Vector2 anchorMin,anchorMax,pivot,anchoredPosition,sizeDelta,offsetMin,offsetMax;
 public Rect rect => new Rect(-pivot.x * sizeDelta.x, -(1f - pivot.y) * sizeDelta.y, sizeDelta.x, sizeDelta.y);
}
public struct Rect {
 public float xMin,yMin,width,height;
 public Rect(float x,float y,float width,float height) { xMin=x; yMin=y; this.width=width; this.height=height; }
 public float xMax => xMin + width; public float yMax => yMin + height;
}
public struct Vector2 {
 public float x,y; public Vector2(float x,float y) { this.x=x;this.y=y; }
 public static Vector2 zero => new(0,0);
 public static Vector2 operator -(Vector2 a,Vector2 b) => new(a.x-b.x,a.y-b.y);
 public static Vector2 operator +(Vector2 a,Vector2 b) => new(a.x+b.x,a.y+b.y);
}
public struct Vector3 {
 public float x,y,z; public Vector3(float x,float y,float z) { this.x=x;this.y=y;this.z=z; }
}
public struct Color {
 public float r,g,b,a; public Color(float r,float g,float b,float a) { this.r=r;this.g=g;this.b=b;this.a=a; }
}
public enum TextAnchor { UpperLeft, UpperCenter, UpperRight, MiddleLeft, MiddleCenter, MiddleRight, LowerLeft, LowerCenter, LowerRight }
}
namespace TMPro
{
    public enum TextOverflowModes { Overflow, Ellipsis, Masking, ScrollRect }
    public struct CharacterInfo { public char character; public float origin,ascender,descender,xAdvance; }
    public struct LinkInfo {
        public string id; public int linkTextfirstCharacterIndex;
        public string GetLinkID() => id;
    }
    public sealed class TMP_TextInfo {
        public CharacterInfo[] characterInfo = Array.Empty<CharacterInfo>();
        public int characterCount;
        public LinkInfo[] linkInfo = Array.Empty<LinkInfo>(); public int linkCount;
    }
    public sealed class TextMeshProUGUI : UnityEngine.Component
    {
        public TextMeshProUGUI() { gameObject = new UnityEngine.GameObject("text"); }
        public UnityEngine.RectTransform rectTransform => (UnityEngine.RectTransform)transform;
        public static System.Func<string, float>? PreferredWidthProvider;
        public TMP_TextInfo textInfo = new();
        public string text = string.Empty;
        public bool enableWordWrapping;
        public TextOverflowModes overflowMode;
        public float preferredWidth => PreferredWidthProvider != null ? PreferredWidthProvider(text) : text.Length * 8f;
    }
}
namespace UnityEngine.UI {
 public sealed class Image : UnityEngine.Component {
  public UnityEngine.Color color; public bool raycastTarget;
  public UnityEngine.RectTransform rectTransform => (UnityEngine.RectTransform)transform;
 }
}
namespace HarmonyLib
{
    public sealed class Harmony { public static int Patches, Unpatches; public static bool Fail; public Harmony(string id) {} public void Patch(object method, HarmonyMethod? prefix = null, HarmonyMethod? postfix = null) { Patches++; if (Fail) throw new InvalidOperationException("patch fault"); } public void UnpatchAll(string id) { Unpatches++; } public static Patches? GetPatchInfo(object method) => null; }
    public sealed class HarmonyMethod { public HarmonyMethod(object method) {} }
    public sealed class Patches { public readonly List<Patch> Prefixes = new(); }
    public sealed class Patch { }
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
    public interface GraphNodeStorage { List<GraphNodeStorage> children { get; } List<GraphNodeStorage> parents { get; } }
    public interface GraphNodeWindow { GraphNodeStorage? child { get; } }
    public sealed class Window : Il2CppObjectBase, GraphNodeWindow, GraphNodeStorage { public GraphNodeStorage? child { get; set; } public List<GraphNodeStorage> children => child == null ? new() : new(){child}; public List<GraphNodeStorage> parents {get;}=new(); }
    public sealed class Storage : Il2CppObjectBase, GraphNodeStorage { public List<GraphNodeStorage> children { get; } = new(); public List<GraphNodeStorage> parents { get; } = new(); }
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
        public GraphNodeStorage? child;
        public List<GraphNodeStorage> children {get;} = new(); public List<GraphNodeStorage> parents => Parent == null ? new() : new(){Parent};
        public GameSlotInventory? FilterSlot;
        public bool AddParent(GraphNodeStorage node)=>true; public bool RemoveParent(GraphNodeStorage node)=>true; public bool IsTag(string name)=>state.IsTag(name); public string GetDisplayName(bool unused)=>string.IsNullOrEmpty(_name)?identifier:_name;
        public bool CheckIsStateValid(GameInventory inv,SlotMarker marker,GridShape shape,TagSystem state)=>!RejectState;
        public void Destroy() {} public void Validate() {}
    }
    public sealed class GameItemElement : GameItem { public RichTextBuilder GetTooltipBasic()=>new(); }
    public class GameInventory : Il2CppObjectBase, GraphNodeStorage, GraphNodeWindow
    {
        public GameItem? ParentItem; public bool InsertLocked,RemoveLocked,Reject;
        public int NativeShapeChecks; public Func<GameItem,bool>? Accepts;
        public List<GraphNodeStorage> children => childItems.Cast<GraphNodeStorage>().ToList(); public List<GraphNodeStorage> parents => ParentItem == null ? new() : new(){ParentItem}; public GraphNodeStorage? child=>null;
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
    public sealed class GameGridInventory : GameInventory { private readonly NativeList items; public GameGridInventory(){items=childItems;} }
    public sealed class GameGridScrollableInventory : GameInventory { private readonly NativeList items; public GameGridScrollableInventory(){items=childItems;} }
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
        public GameGridInventory invWindow=new(), frontInvWindow=new(), backInvWindow=new(), backInvinvElementCounter=new(), showcaseWindow=new(), trashInvWindow=new(), docInvWindow=new(), hiddenWindow=new(), afterhourWindow=new(), vendingFountainWindow=new(), vendingMachineWindow=new();
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
        private static bool PlaceAtShape(GameItem item,GameInventory inventory,GridShape shape)
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
        public static int GetWaterPurity(GameItem item)=>WaterPurity;
        public static int WaterPurity = 10000;
        public static string GetLiquidContainerColor(GameItem item)=>"";
        public static void UpdateContainerGoodType(GameItem item) {}
        public static void TransferLiquid(GameItem a,GameItem b) {}
        public static void CreateWaterContainerTooltip(RichTextBuilder b,GameItem i) {} public static int HandleFilter(string id,int amount,GameItem source,GameItem filter)=>amount;
    }
    public static class WaterFeatureHelper
    {
        // Mirrors WaterFeatureHelper.WATER_PURITIES of the game.
        public static readonly int[] WATER_PURITIES = {9990,9900,9800,9600,9000,3500};
        public static string GetWaterDisplayName(GameItem item)=>"water";
        public static int GetWaterIndex(GameItem item)
        {
            var purity = WaterHelper.GetWaterPurity(item);
            for (var i = 0; i < WATER_PURITIES.Length; i++)
                if (purity >= WATER_PURITIES[i]) return i;
            return -1;
        }
        public static int GetPurityArrayIndex(int purity)
        {
            for (var i = 0; i < WATER_PURITIES.Length; i++)
                if (purity >= WATER_PURITIES[i]) return i;
            return 5;
        }
        public static string FormatPurityPercentageFromDouble(double p)=>$"{p:0.00}%";
        public static void UpdateWaterFeature(GameItem i,bool b) {} public static void OnWaterLabelUpdated(GameItem i) {}
        public static void OnLiquidContainerPourInto(GameItem a,GameItem b,bool c) {}
        public static void CreateWaterFeatureTooltip(RichTextBuilder b,GameItem i) {}
    }
    public static class PreBuiltItemHelper {public static GameItem WaterFilter()=>new(){identifier="water_filter",name="Water Filter"};}
    public static class LocHelper {public static string Locale="zh";public static string GetCurrentLocaleCode()=>Locale;public static string GetLocalizedMechanic(string key)=>"[label]";}
    public sealed class Liquid { public static Dictionary<string,Liquid>? LiquidDict;public static List<Liquid> Liquids=new();public string identifier="water",displayName="water",currentPart="water",part="water"; }
    public sealed class RichTextBuilder : Il2CppObjectBase
    {
        public sealed class TextSlice : Il2CppObjectBase {public string text="";public RenderHandler.ColorPalette color=RenderHandler.ColorPalette.None;public RenderHandler.ColorPalette backgroundColor=RenderHandler.ColorPalette.None;public bool isNewLine;}
        private List<TextSlice> slices=new();
        public List<TextSlice> Slices => slices;
        public void Clear()=>slices.Clear();
        public RichTextBuilder Add(string text,RenderHandler.ColorPalette color=RenderHandler.ColorPalette.None,string end="",bool bold=false,bool italic=false,bool underline=false,bool strikethrough=false,RenderHandler.ColorPalette backgroundColor=RenderHandler.ColorPalette.None,RenderHandler.ColorPalette underlineColor=RenderHandler.ColorPalette.None,RenderHandler.ColorPalette strikethroughColor=RenderHandler.ColorPalette.None)
        {if(!string.IsNullOrEmpty(text))slices.Add(new(){text= end == "\n" ? text + "\n" : text, color=color, backgroundColor=backgroundColor});return this;}
        public RichTextBuilder AddProgressBarOld(float ratio,int length,RenderHandler.ColorPalette color=RenderHandler.ColorPalette.None,RenderHandler.ColorPalette backgroundColor=RenderHandler.ColorPalette.None,char[]? style=null,string end="",bool addFullLength=true,bool bold=false)
        {
            // An empty bar adds no slice in the real builder; AquaSense only uses it to
            // mark the preceding slice as no-break.
            if(length<=0)return this;
            var filled=Math.Max(1,(int)Math.Round(Math.Clamp(ratio,0f,1f)*length));
            slices.Add(new(){text=new string('█',filled)});return this;
        }
        public void AddLine(string text,bool ignoreEmpty,RenderHandler.ColorPalette color,bool bold,bool italic,bool underline,bool strikethrough,RenderHandler.ColorPalette background,RenderHandler.ColorPalette uc,RenderHandler.ColorPalette sc)=>slices.Add(new(){text=text + "\n", color=color, backgroundColor=background});
    }
    public sealed class RenderHandler
    {
        public enum ColorPalette {None=-1}
        public static RenderHandler? current;
        public TreeNodeRender? tempRoot;
        public UnityEngine.GameObject? tempRootObject;
    }
    // Stand-in for the game's text render node used to measure text widths.
    public sealed class TreeNodeRender
    {
        public TMPro.TextMeshProUGUI? text;
        public static TreeNodeRender? InstantiateText() => RenderHandler.current == null ? null : new TreeNodeRender { text = new TMPro.TextMeshProUGUI() };
    }
    public sealed class StoreUIManager {public static StoreUIManager Instance=new();public List<string> Messages=new();public void Notify(string message,string color)=>Messages.Add(message);}
    public static class ModConfigWindow { public static int Builds; private static void BuildEntry(CustomUIBuilder builder, ModConfig config, ConfigEntry entry) { Builds++; } }
    public sealed class CustomUIElement
    {
        public string Text = ""; public float Width;
        public void SetText(string text) => Text = text;
        public void SetFixedWidth(float width) => Width = width;
    }
    public sealed class CustomUIBuilder
    {
        public readonly List<string> Calls = new();
        public readonly List<CustomUIElement> Labels = new();
        public CustomUIElement Last { get; private set; } = new();
        public Action<float>? SliderChanged;
        public CustomUIBuilder BeginRow(float spacing = 4f, UnityEngine.TextAnchor alignment = UnityEngine.TextAnchor.UpperLeft) { Calls.Add("row"); return this; }
        public CustomUIBuilder End() { Calls.Add("end"); return this; }
        public CustomUIBuilder AddLabel(string text, string? id = null) { Last = new CustomUIElement { Text = text }; Labels.Add(Last); Calls.Add("label:" + id); return this; }
        public CustomUIBuilder AddFlexibleSpace() { Calls.Add("space"); return this; }
        public CustomUIBuilder AddToggle(string label, bool initial, Action<bool>? onChanged = null, string? id = null) { Last = new CustomUIElement(); Calls.Add("toggle:" + id); return this; }
        public CustomUIBuilder AddSlider(float min, float max, float initial, Action<float>? onChanged = null, bool wholeNumbers = false, string? id = null) { Last = new CustomUIElement(); SliderChanged = onChanged; Calls.Add("slider:" + id); return this; }
        public CustomUIBuilder AddDropdown(IList<string> options, int initialIndex = 0, Action<int>? onChanged = null, string? id = null) { Last = new CustomUIElement(); Calls.Add("dropdown:" + id); return this; }
        public CustomUIBuilder AddInput(string placeholder, string initial = "", Action<string>? onChanged = null, string? id = null, Action<string>? onTyping = null) { Last = new CustomUIElement { Text = initial }; Calls.Add("input:" + id); return this; }
    }
    public static class TMPIconHover { public static readonly Dictionary<string, Func<string>> CustomLinks = new(); }
    public sealed class ModUIManager : UnityEngine.MonoBehaviour { public static ModUIManager? Instance; }
    public sealed class MainMenuUIController : UnityEngine.MonoBehaviour { public static MainMenuUIController? Instance; }
    public static class ModLoader { public static List<LoadedMod> Mods = new(); }
    public sealed class LoadedMod { public ModManifest Manifest = new(); public bool IsEnabled = true; }
}

public interface IMod { void Init(ModManifest m); void OnEnable(); void OnDisable(); }
public class ModManifest { public string ID="test"; }
public class ModLog { public ModLog(ModManifest m){} public void Log(string s)=>System.Console.WriteLine(s); public void Warning(string s)=>System.Console.WriteLine(s); }
public abstract class ConfigEntry
{
    public string Key = "";
    public string Section = "";
    public Func<string>? Label;
    public Func<string>? Tooltip;
    public bool RequiresRestart;
    public bool HasRange;
    public float Min, Max;
    public string[]? Choices;
    public abstract Type ValueType { get; }
    public abstract object BoxedValue { get; }
    public abstract object BoxedDefault { get; }
}
public class ConfigEntry<T> : ConfigEntry
{
    public T Value;
    public event Action<T>? OnChanged;
    public ConfigEntry(T v){Value=v;}
    public override Type ValueType => typeof(T);
    public override object BoxedValue => Value!;
    public override object BoxedDefault => Value!;
}
public class ModConfig
{
    public string ModId = "";
    public readonly List<ConfigEntry> Entries = new();
    public static ModConfig For(string s)=>new(){ModId=s};
    public ConfigEntry<T> Define<T>(string k,T v,string c,Func<string> l,Func<string>? tooltip=null,bool requiresRestart=false)
    {
        var e = new ConfigEntry<T>(v){Key=k,Section=c,Label=l,Tooltip=tooltip,RequiresRestart=requiresRestart};
        Entries.Add(e); return e;
    }
    public ConfigEntry<int> Define(string k,int v,string c,Func<string> l,int? min=null,int? max=null,Func<string>? tooltip=null,bool requiresRestart=false)
    {
        var e = new ConfigEntry<int>(v){Key=k,Section=c,Label=l,Tooltip=tooltip,RequiresRestart=requiresRestart};
        if(min.HasValue&&max.HasValue){e.HasRange=true;e.Min=min.Value;e.Max=max.Value;}
        Entries.Add(e); return e;
    }
}
public static class GeneralHelper { public static bool IsItemOwned(GameItem item)=>item.state.IsTag("IS_OWNED_TAG"); }


