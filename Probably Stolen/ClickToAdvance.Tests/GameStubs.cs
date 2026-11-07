using System;
using System.Collections.Generic;

public interface IMod { void Init(ModManifest manifest); void OnEnable(); void OnDisable(); }

public sealed class ModManifest
{
    public string ID { get; set; } = "chiqingsan.ClickToAdvance";
}

public sealed class DialogUIManager
{
    public static DialogUIManager? Instance { get; set; }
    public object? currentDialog;
    public bool Choice;
    public int Calls;
    public bool IsChoice() => Choice;
    public void OnAdvancePressed() => Calls++;
}

public sealed class ModConfig
{
    public static ModConfig For(string modId) => new ModConfig();

    public ConfigEntry<bool> Define(string key, bool defaultValue, string section, Func<string> label,
        Func<string>? tooltip = null, bool requiresRestart = false)
        => new ConfigEntry<bool>(defaultValue);

    public ConfigEntry<string> Define(string key, string? defaultValue, string section, Func<string> label,
        string[]? choices = null, Func<string>? tooltip = null, bool requiresRestart = false)
        => new ConfigEntry<string>(defaultValue ?? "");
}

public sealed class ConfigEntry<T>
{
    public T Value { get; private set; }
    public T Default { get; }

    public event Action<T>? OnChanged;

    internal ConfigEntry(T defaultValue)
    {
        Default = defaultValue;
        Value = defaultValue;
    }

    public void Set(T newValue)
    {
        if (EqualityComparer<T>.Default.Equals(Value, newValue)) return;
        Value = newValue;
        OnChanged?.Invoke(newValue);
    }

    public void Reset() => Set(Default);
}

public sealed class GameOptionRegistry
{
    public const string SECTION_KEYBIND = "keybind";

    public sealed class Section
    {
        public string id = "";
        public readonly List<Entry> entries = new();
    }

    public sealed class Entry
    {
        public string id = "";
    }

    private static readonly List<Section> Sections = new();

    public static Section? FindSection(string id) => Sections.Find(s => s.id == id);

    public static Entry? Find(string id)
    {
        foreach (var section in Sections)
        {
            var entry = section.entries.Find(e => e.id == id);
            if (entry != null) return entry;
        }
        return null;
    }

    public static Entry? Add(string sectionId, Entry entry)
    {
        var section = FindSection(sectionId);
        if (section == null) return null;
        section.entries.RemoveAll(e => e.id == entry.id);
        section.entries.Add(entry);
        return entry;
    }

    public static void EnsureVanilla()
    {
        if (FindSection(SECTION_KEYBIND) == null)
            Sections.Add(new Section { id = SECTION_KEYBIND });
    }

    public static Entry Toggle(string id, Func<string> label, Func<bool> get, Action<bool> set,
        Func<string>? tooltip = null)
        => new Entry { id = id };

    public static Entry KeyBind(string id, Func<string> label, Func<UnityEngine.KeyCode> get,
        Action<UnityEngine.KeyCode> set, Func<string>? tooltip = null)
        => new Entry { id = id };
}

namespace UnityEngine
{
    public enum KeyCode
    {
        None = 0,
        Backspace = 8,
        Tab = 9,
        Return = 13,
        Escape = 27,
        Space = 32,
        RightShift = 303,
        LeftShift = 304,
        RightControl = 305,
        LeftControl = 306,
        LeftAlt = 308,
        RightAlt = 309,
        Mouse0 = 323,
        Mouse1 = 324,
        Mouse2 = 325,
        Mouse3 = 326,
        Mouse4 = 327,
        Mouse5 = 328,
        Mouse6 = 329,
    }

    public class Object
    {
        public static void DontDestroyOnLoad(object value) { }
        public static void Destroy(object value) { }
    }

    public class MonoBehaviour : Object { }

    public class GameObject : Object
    {
        public GameObject(string name) { }
        public T AddComponent<T>() where T : new() => new();
    }

    public static class Input
    {
        public static bool MouseButtonDown;
        public static readonly HashSet<KeyCode> Keys = new();

        public static bool GetMouseButtonDown(int button) => MouseButtonDown;
        public static bool GetKey(KeyCode key) => Keys.Contains(key);
    }

    public static class Debug
    {
        public static void Log(string message) { }
        public static void LogWarning(string message) { }
        public static void LogError(string message) { }
    }
}

namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class HarmonyPatchAttribute : Attribute
    {
        public HarmonyPatchAttribute(Type type, string methodName) { }
        public HarmonyPatchAttribute(Type type, string methodName, Type[] argumentTypes) { }
    }

    public class Harmony
    {
        public Harmony(string id) { }
        public void PatchAll() { }
        public void UnpatchAll(string? harmonyId = null) { }
    }
}
