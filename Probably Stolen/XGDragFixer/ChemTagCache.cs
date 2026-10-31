using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using XGDragFixer;

namespace XGDragFixer;

/// <summary>
/// The nightly chemical simulation reads container tags once per simulated second
/// through <see cref="GameItem.GetTagReadonly(string)"/>, which clones a
/// <see cref="TagState"/> on every call. A single container tick runs 20+ reaction
/// handlers, each re-reading the same tags (and <see cref="WaterHelper.IsEmpty"/>
/// alone walks every liquid), so the same values get cloned hundreds of times per
/// tick — and the same container is ticked 360 times per night.
///
/// The cached value is the clone of an entry of <see cref="GameItem.modifiedState"/>,
/// which only changes when the game rebuilds it in <see cref="GameItem.ValidateShapeState"/>;
/// that is the only point where the cache is dropped for that container. Writes that
/// only touch the live state (skipValidate) do not change what a read would return,
/// so they cannot make the cache stale; they only reset the cheap IsEmpty result.
/// </summary>
internal static class ChemTagCache
{
    private sealed class RefComparer : IEqualityComparer<GameItem>
    {
        internal static readonly RefComparer Instance = new();

        public bool Equals(GameItem? x, GameItem? y) => ReferenceEquals(x, y);

        public int GetHashCode(GameItem obj) => RuntimeHelpers.GetHashCode(obj);
    }

    private static readonly Dictionary<GameItem, Dictionary<string, TagState>> Cache =
        new(RefComparer.Instance);

    private static readonly Dictionary<GameItem, bool> EmptyCache = new(RefComparer.Instance);

    private static GameItem? _item;
    private static int _depth;
    private static bool _active;
    private static long _hits;
    private static long _misses;

    internal static bool Installed { get; private set; }

    private static bool Enabled => Core.ChemTagCacheEnabled?.Value == true;

    internal static void Install(Harmony harmony)
    {
        try
        {
            // containerTempTick is a static lambda held in a private readonly
            // delegate field; patch the delegate's target so the window covers
            // HandleAmbientTempDrop + HandleTemperatureReaction.
            var field = AccessTools.Field(typeof(ChemicalHelper), "containerTempTick");
            if (field?.GetValue(null) is not Delegate del || del.Method == null)
            {
                Core.Log?.Warning("化学标签缓存：未找到 containerTempTick 委托，跳过");
                return;
            }

            harmony.Patch(del.Method,
                prefix: new HarmonyMethod(typeof(ChemTagCache), nameof(TickPrefix)),
                postfix: new HarmonyMethod(typeof(ChemTagCache), nameof(TickPostfix)));

            var getReadonly = AccessTools.Method(typeof(GameItem), "GetTagReadonly", new[] { typeof(string) });
            if (getReadonly == null)
            {
                Core.Log?.Warning("化学标签缓存：未找到 GameItem.GetTagReadonly(string)，跳过");
                return;
            }

            harmony.Patch(getReadonly,
                prefix: new HarmonyMethod(typeof(ChemTagCache), nameof(ReadonlyPrefix)),
                postfix: new HarmonyMethod(typeof(ChemTagCache), nameof(ReadonlyPostfix)));

            var isEmpty = AccessTools.Method(typeof(WaterHelper), "IsEmpty", new[] { typeof(GameItem) });
            if (isEmpty != null)
            {
                harmony.Patch(isEmpty,
                    prefix: new HarmonyMethod(typeof(ChemTagCache), nameof(EmptyPrefix)),
                    postfix: new HarmonyMethod(typeof(ChemTagCache), nameof(EmptyPostfix)));
            }

            // modifiedState is only rebuilt here; anything cached from it must be
            // dropped at that exact point.
            var validate = AccessTools.Method(typeof(GameItem), "ValidateShapeState", Type.EmptyTypes);
            if (validate != null)
            {
                harmony.Patch(validate,
                    prefix: new HarmonyMethod(typeof(ChemTagCache), nameof(ModifiedPrefix)));
            }

            // The cheap IsEmpty answer also depends on live state writes (IsTag),
            // so those reset it even though the cloned tags stay valid.
            PatchEmptyInvalidator(harmony, "ModifyTag",
                new[] { typeof(string), typeof(Action<TagState>), typeof(bool) });
            PatchEmptyInvalidator(harmony, "EnableTag", new[] { typeof(string), typeof(bool) });
            PatchEmptyInvalidator(harmony, "DisableTag", new[] { typeof(string), typeof(bool) });
            PatchEmptyInvalidator(harmony, "ToggleTag", new[] { typeof(string), typeof(bool) });

            Installed = true;
            Core.Log?.Log("已挂接化学标签读取缓存");
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"化学标签缓存挂接失败: {ex.GetType().Name} {ex.Message}");
        }
    }

    private static void PatchEmptyInvalidator(Harmony harmony, string methodName, Type[] argumentTypes)
    {
        var method = AccessTools.Method(typeof(GameItem), methodName, argumentTypes);
        if (method != null)
        {
            harmony.Patch(method,
                prefix: new HarmonyMethod(typeof(ChemTagCache), nameof(StateChangedPrefix)));
        }
    }

    internal static void Begin()
    {
        if (!Enabled)
        {
            return;
        }

        Cache.Clear();
        EmptyCache.Clear();
        _item = null;
        _depth = 0;
        _hits = 0;
        _misses = 0;
        _active = true;
    }

    internal static void End()
    {
        Cache.Clear();
        EmptyCache.Clear();
        _item = null;
        _depth = 0;
        _active = false;
    }

    private static void TickPrefix(object[] __args)
    {
        if (!_active)
        {
            return;
        }

        GameItem? container = null;
        if (__args != null && __args.Length > 0)
        {
            container = __args[0] as GameItem;
        }

        if (container == null)
        {
            return;
        }

        if (_depth > 0 && !ReferenceEquals(_item, container))
        {
            // Defensive: a previous tick did not unwind (exception inside vanilla).
            _depth = 0;
        }

        _item = container;
        _depth++;
    }

    private static void TickPostfix()
    {
        if (_depth > 0)
        {
            _depth--;
        }

        if (_depth == 0)
        {
            _item = null;
        }
    }

    private static bool ReadonlyPrefix(GameItem __instance, string tagName, ref TagState __result)
    {
        if (!_active || tagName == null)
        {
            return true;
        }

        if (Cache.TryGetValue(__instance, out var map) && map.TryGetValue(tagName, out var state) &&
            state != null)
        {
            _hits++;
            __result = state;
            return false;
        }

        _misses++;
        return true;
    }

    private static void ReadonlyPostfix(GameItem __instance, string tagName, TagState __result)
    {
        if (!_active || tagName == null || __result == null)
        {
            return;
        }

        if (!Cache.TryGetValue(__instance, out var map))
        {
            map = new Dictionary<string, TagState>(32);
            Cache[__instance] = map;
        }

        map[tagName] = __result;
    }

    private static bool EmptyPrefix(GameItem gameItem, ref bool __result)
    {
        if (!_active || !EmptyCache.TryGetValue(gameItem, out var empty))
        {
            return true;
        }

        _hits++;
        __result = empty;
        return false;
    }

    private static void EmptyPostfix(GameItem gameItem, bool __result)
    {
        if (_active)
        {
            EmptyCache[gameItem] = __result;
        }
    }

    /// <summary>Live-state write: the IsEmpty answer may change, cloned tags do not.</summary>
    private static void StateChangedPrefix(GameItem __instance)
    {
        if (_active)
        {
            EmptyCache.Remove(__instance);
        }
    }

    /// <summary>modifiedState rebuilt: drop the cached clones of this container.</summary>
    private static void ModifiedPrefix(GameItem __instance)
    {
        if (!_active)
        {
            return;
        }

        Cache.Remove(__instance);
        EmptyCache.Remove(__instance);
    }

    internal static string Summary()
    {
        return $"化学标签缓存命中 {_hits} / 未命中 {_misses}";
    }

    internal static void ResetCounters()
    {
        _hits = 0;
        _misses = 0;
    }

    internal static void Reset()
    {
        Cache.Clear();
        EmptyCache.Clear();
        _item = null;
        _depth = 0;
        _active = false;
        ResetCounters();
    }
}
