public interface IMod { void Init(ModManifest manifest); void OnEnable(); void OnDisable(); }
public sealed class ModManifest { }
public sealed class DialogUIManager
{
    public static DialogUIManager? Instance { get; set; }
    public object? currentDialog;
    public bool Choice;
    public int Calls;
    public bool IsChoice() => Choice;
    public void OnAdvancePressed() => Calls++;
}
namespace UnityEngine
{
    public class Object { public static void DontDestroyOnLoad(object value) { } public static void Destroy(object value) { } }
    public class MonoBehaviour : Object { }
    public class GameObject : Object { public GameObject(string name) { } public T AddComponent<T>() where T : new() => new(); }
    public static class Input { public static bool GetMouseButtonDown(int button) => false; }
}
