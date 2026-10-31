using UnityEngine;

namespace AutoBatterySwap;

public sealed class AutoBatterySwapMod : IMod
{
    public void Init(ModManifest manifest)
    {
    }

    public void OnEnable()
    {
        Debug.Log("[BetterAutomotiveBattery] enabled");
    }

    public void OnDisable()
    {
        Debug.Log("[BetterAutomotiveBattery] disabled");
    }
}
