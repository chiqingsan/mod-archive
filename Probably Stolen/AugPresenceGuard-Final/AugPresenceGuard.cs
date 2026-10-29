using HarmonyLib;
using UnityEngine;

namespace AugPresenceGuard;

public sealed class Mod : IMod
{
    private const string HarmonyId = "chiqingsan.aug-presence-guard";
    private Harmony? _harmony;

    public void Init(ModManifest manifest)
    {
        Debug.Log("[AugPresenceGuard] 已加载：改造体存在度限制为 1～99。 ");
    }

    public void OnEnable()
    {
        _harmony = new Harmony(HarmonyId);
        _harmony.PatchAll();
    }

    public void OnDisable()
    {
        _harmony?.UnpatchAll(HarmonyId);
        _harmony = null;
    }
}

internal static class Presence
{
    private const int Minimum = 1;
    private const int Maximum = 99;

    internal static void Clamp()
    {
        var store = PlayerStore.Instance;
        if (store == null || !store.isAugIntroduced || store.isAugEnded)
            return;

        store.augPresence = Mathf.Clamp(store.augPresence, Minimum, Maximum);
    }
}

[HarmonyPatch(typeof(AugHelper), nameof(AugHelper.OnGoodKill))]
internal static class GoodKillPatch
{
    private static void Postfix() => Presence.Clamp();
}

[HarmonyPatch(typeof(AugHelper), nameof(AugHelper.OnAugLeftAlive))]
internal static class AugLeftAlivePatch
{
    private static void Postfix() => Presence.Clamp();
}

[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.LoadGame))]
internal static class LoadGamePatch
{
    private static void Postfix() => Presence.Clamp();
}

[HarmonyPatch(typeof(StoreClientManager), nameof(StoreClientManager.HandleAugClient))]
internal static class AugGenerationPatch
{
    private static void Prefix() => Presence.Clamp();
}
