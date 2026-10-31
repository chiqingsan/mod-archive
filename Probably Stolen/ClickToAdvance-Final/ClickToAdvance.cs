using UnityEngine;

namespace ClickToAdvanceFinal;

public sealed class ClickToAdvanceMod : IMod
{
    private GameObject? _runner;

    public void Init(ModManifest manifest) { }

    public void OnEnable()
    {
        _runner = new GameObject("ClickToAdvance");
        UnityEngine.Object.DontDestroyOnLoad(_runner);
        _runner.AddComponent<ClickToAdvanceRunner>();
    }

    public void OnDisable()
    {
        if (_runner != null) UnityEngine.Object.Destroy(_runner);
        _runner = null;
    }
}

public sealed class ClickToAdvanceRunner : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) Advance(DialogUIManager.Instance);
    }

    internal static void Advance(DialogUIManager? dialog)
    {
        if (dialog == null || dialog.currentDialog == null || dialog.IsChoice()) return;
        dialog.OnAdvancePressed();
    }
}
