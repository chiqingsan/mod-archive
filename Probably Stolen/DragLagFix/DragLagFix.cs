using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(DragLagFix.Core), "DragLagFix", "1.0.6", "chiqingsan")]
[assembly: MelonGame("Questing Goose Studio", "Probably Stolen")]

namespace DragLagFix;

public sealed class Core : MelonMod
{
    internal static MelonLogger.Instance? Log { get; private set; }
    internal static MelonPreferences_Entry<bool>? SkipStaticRefresh { get; private set; }
    internal static MelonPreferences_Entry<int>? StaticRefreshIntervalMs { get; private set; }
    internal static MelonPreferences_Entry<bool>? ProbeEnabled { get; private set; }
    internal static MelonPreferences_Entry<bool>? FrameStatsEnabled { get; private set; }
    internal static MelonPreferences_Entry<bool>? CacheFindSlotResults { get; private set; }
    internal static MelonPreferences_Entry<int>? FindSlotCacheTtlMs { get; private set; }

    private readonly List<float> _frameDeltas = new();
    private float _frameStatsAt;
    private float _worstFrameMs;

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;
        try
        {
            var category = MelonPreferences.CreateCategory("DragLagFix", "DragLagFix");
            SkipStaticRefresh = category.CreateEntry(
                "SkipStaticDragRefresh",
                true,
                "拖拽中悬停状态（物品/容器/屏幕格）未变化时跳过重复的高亮重算");
            StaticRefreshIntervalMs = category.CreateEntry(
                "StaticRefreshIntervalMs",
                100,
                "悬停静止时兜底刷新间隔（毫秒）");
            ProbeEnabled = category.CreateEntry(
                "ProbeHotPaths",
                false,
                "输出放置检测等热点方法的耗时日志（排查用）");
            FrameStatsEnabled = category.CreateEntry(
                "FrameStats",
                true,
                "每10秒输出帧时间统计（平均/P95/最差帧，排查用）");
            CacheFindSlotResults = category.CreateEntry(
                "CacheFindSlotResults",
                true,
                "缓存找空位结果：容器内容未变且未过期时直接复用（失败结果也缓存）");
            FindSlotCacheTtlMs = category.CreateEntry(
                "FindSlotCacheTtlMs",
                800,
                "找空位缓存的过期时间（毫秒）");
        }
        catch (Exception e)
        {
            Log?.Warning($"初始化配置失败: {e.Message}");
        }
    }

    public override void OnUpdate()
    {
        if (FrameStatsEnabled is not { Value: true })
            return;

        float dtMs = Time.unscaledDeltaTime * 1000f;
        _frameDeltas.Add(dtMs);
        if (dtMs > _worstFrameMs)
            _worstFrameMs = dtMs;

        float now = Time.unscaledTime;
        if (_frameStatsAt == 0f)
        {
            _frameStatsAt = now + 10f;
            return;
        }
        if (now < _frameStatsAt || _frameDeltas.Count == 0)
            return;

        _frameStatsAt = now + 10f;
        var sorted = new List<float>(_frameDeltas);
        sorted.Sort();
        double avgMs = 0;
        foreach (var d in _frameDeltas) avgMs += d;
        avgMs /= _frameDeltas.Count;
        float p95 = sorted[Mathf.Min(sorted.Count - 1, (int)(sorted.Count * 0.95f))];
        Log?.Msg($"[frame] 10s: avg={avgMs:F1}ms p95={p95:F1}ms worst={_worstFrameMs:F1}ms n={_frameDeltas.Count}");
        _frameDeltas.Clear();
        _worstFrameMs = 0f;
    }

    public override void OnLateInitializeMelon()
    {
        var harmony = new HarmonyLib.Harmony("chiqingsan.draglagfix");

        TryPatch("拖拽刷新节流与计时", () => Hooks.PatchDragRefreshThrottle(harmony));

        // 探针只挂外层低频方法：内层热方法（TryInventorySlot/CheckShapePlacement 在一次
        // 穷举中被调用数千次）挂 native detour 会让每次调用的托管转换开销放大成倍卡顿。
        // v1.0.5 教训：ResizePixels/ValidateBackground 名义上是"物品增删才调用"的 UI 方法，
        // 实际是每帧级热路径（97+27 条探针记录，常态帧均 17→20-38ms），一律不得挂探针。
        TryPatch("探针", () =>
        {
            Hooks.PatchProbe(harmony, typeof(GameGridInventory), nameof(GameGridInventory.TryFindOneValidInventorySlot),
                new[] { typeof(GameItem), typeof(bool) });
            Hooks.PatchProbe(harmony, typeof(GameGridScrollableInventory), nameof(GameGridScrollableInventory.TryFindOneValidInventorySlot),
                new[] { typeof(GameItem), typeof(bool) });
            Hooks.PatchProbe(harmony, typeof(ContainerHelper), nameof(ContainerHelper.CreateContainerTooltip),
                new[] { typeof(RichTextBuilder), typeof(GameItem) });
        });

        // 实测热点：批量转移/搬运循环逐件调 TryFindOneValidInventorySlot，对着近满容器
        // 每次都是 40-85ms 的穷举失败。结果缓存把重复找位压成一次。
        TryPatch("找空位结果缓存", () =>
        {
            Hooks.PatchFindSlotCache(harmony, typeof(GameGridInventory), nameof(GameGridInventory.TryFindOneValidInventorySlot));
            Hooks.PatchFindSlotCache(harmony, typeof(GameGridScrollableInventory), nameof(GameGridScrollableInventory.TryFindOneValidInventorySlot));
        });
    }

    private static void TryPatch(string label, Action action)
    {
        try
        {
            action();
            Log?.Msg($"{label} patch 完成。");
        }
        catch (Exception e)
        {
            Log?.Warning($"{label} patch 失败: {e.Message}");
        }
    }
}

internal static class Hooks
{
    // ---- 拖拽刷新节流：状态未变化时跳过 RefreshDragHighlight ----

    private static nint _lastItemPtr;
    private static nint _lastInvPtr;
    private static long _lastPosBucket;
    private static float _lastRefreshAt = -999f;

    private const float PosBucketPixels = 12f;

    internal static void PatchDragRefreshThrottle(HarmonyLib.Harmony harmony)
    {
        var original = AccessTools.Method(
            typeof(ItemMouseDragHandler), nameof(ItemMouseDragHandler.RefreshDragHighlight), Type.EmptyTypes)
            ?? throw new MissingMethodException("ItemMouseDragHandler.RefreshDragHighlight not found");
        // 节流 prefix + 探针计时：RefreshDragHighlight 每帧最多一次，detour 开销可忽略，
        // 其耗时即"每次高亮重算"的真实成本（内部对 TryInventorySlot 的调用是原生互调，无需单独测）。
        harmony.Patch(original,
            prefix: new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(RefreshDragHighlightPrefix))));
        harmony.Patch(original,
            prefix: new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(ProbePrefix))),
            finalizer: new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(ProbeFinalizer))));
    }

    internal static bool RefreshDragHighlightPrefix(ItemMouseDragHandler __instance)
    {
        try
        {
            if (Core.SkipStaticRefresh is not { Value: true })
                return true;

            var item = __instance.currentItem;
            var inv = __instance.lastInventory;
            nint itemKey = item != null ? item.Pointer : 0;
            nint invKey = inv != null ? inv.Pointer : 0;

            Vector3 mp = Input.mousePosition;
            long posBucket = ((long)Mathf.FloorToInt(mp.x / PosBucketPixels) << 32)
                             ^ (uint)Mathf.FloorToInt(mp.y / PosBucketPixels);

            float now = Time.unscaledTime;
            float interval = Mathf.Max(0.02f, (Core.StaticRefreshIntervalMs?.Value ?? 100) / 1000f);
            bool expired = now - _lastRefreshAt >= interval;

            if (itemKey == _lastItemPtr && invKey == _lastInvPtr && posBucket == _lastPosBucket && !expired)
                return false;

            _lastItemPtr = itemKey;
            _lastInvPtr = invKey;
            _lastPosBucket = posBucket;
            _lastRefreshAt = now;
            return true;
        }
        catch
        {
            return true;
        }
    }

    // ---- 找空位结果缓存：同一 (容器, 物品) 在 TTL 内且容器内容未变时复用结果 ----
    // 容器内容变化几乎必然改变 childItems.Count，作为失效条件；TTL 兜底覆盖堆叠等少数不变数量的操作。
    // 返回过期结果的最坏影响是放置方（TryAcceptOnce）被游戏自身校验拒绝后回退，不会损坏数据。

    private struct FindSlotEntry
    {
        public SlotMarker? Slot;
        public int ChildCount;
        public long ExpiresAt;
    }

    private static readonly Dictionary<(nint Inv, nint Item, bool KeepOrientation), FindSlotEntry> _findSlotCache = new();
    private static (nint Inv, nint Item, bool KeepOrientation)? _pendingFindKey;
    private static int _pendingChildCount;

    internal static void PatchFindSlotCache(HarmonyLib.Harmony harmony, Type type, string methodName)
    {
        var original = AccessTools.Method(type, methodName, new[] { typeof(GameItem), typeof(bool) })
            ?? throw new MissingMethodException($"{type.Name}.{methodName}(GameItem, bool) not found");
        harmony.Patch(original,
            prefix: new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(TryFindCachePrefix))),
            postfix: new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(TryFindCachePostfix))));
    }

    internal static bool TryFindCachePrefix(GameInventory __instance, GameItem item, bool keepOrientation, ref SlotMarker __result)
    {
        try
        {
            nint invKey = __instance.Pointer;
            nint itemKey = item != null ? item.Pointer : 0;

            // 调用模式诊断：记录每次找位的 (容器, 物品)，用于判断重复找位的是同一组合还是不同物品
            if (Core.ProbeEnabled is { Value: true })
            {
                var childItemsNow = __instance.childItems;
                LogFindKey(invKey, itemKey, keepOrientation, childItemsNow != null ? childItemsNow.Count : -1);
            }

            if (Core.CacheFindSlotResults is not { Value: true })
                return true;

            var childItems = __instance.childItems;
            int childCount = childItems != null ? childItems.Count : -1;
            var key = (invKey, itemKey, keepOrientation);
            long now = Stopwatch.GetTimestamp();

            if (_findSlotCache.TryGetValue(key, out var entry)
                && now < entry.ExpiresAt
                && entry.ChildCount == childCount)
            {
                __result = entry.Slot!;
                return false;
            }

            _pendingFindKey = key;
            _pendingChildCount = childCount;
            return true;
        }
        catch
        {
            return true;
        }
    }

    private static int _findLogCount;
    private static float _findLogWindowStart = -999f;

    private static void LogFindKey(nint invKey, nint itemKey, bool keepOrientation, int childCount)
    {
        float now = Time.unscaledTime;
        if (now - _findLogWindowStart > 1f)
        {
            _findLogWindowStart = now;
            _findLogCount = 0;
        }
        if (++_findLogCount > 20)
            return;
        Core.Log?.Msg($"[find] inv=0x{invKey:x} item=0x{itemKey:x} keep={keepOrientation} cnt={childCount}");
    }

    internal static void TryFindCachePostfix(SlotMarker __result)
    {
        try
        {
            if (_pendingFindKey is not { } key)
                return;
            _pendingFindKey = null;

            if (Core.CacheFindSlotResults is not { Value: true })
                return;

            int ttl = Math.Max(20, Core.FindSlotCacheTtlMs?.Value ?? 300);
            if (_findSlotCache.Count > 512)
                _findSlotCache.Clear();

            _findSlotCache[key] = new FindSlotEntry
            {
                Slot = __result,
                ChildCount = _pendingChildCount,
                ExpiresAt = Stopwatch.GetTimestamp() + ttl * Stopwatch.Frequency / 1000,
            };
        }
        catch
        {
        }
    }

    // ---- 探针：prefix 入栈、finalizer 出栈，统计排除嵌套子调用的 self-time ----

    private static readonly Stack<long> _startStack = new();
    private static readonly Stack<long> _childStack = new();
    private static int _probeLogCount;
    private static float _probeLogWindowStart = -999f;

    private const double ProbeSelfMsThreshold = 5.0;
    private const int ProbeMaxLogsPerSecond = 20;

    internal static void PatchProbe(HarmonyLib.Harmony harmony, Type type, string methodName, Type[] argumentTypes)
    {
        var original = AccessTools.Method(type, methodName, argumentTypes);
        if (original == null)
        {
            Core.Log?.Warning($"[probe] 未找到目标: {type.Name}.{methodName}");
            return;
        }
        harmony.Patch(original,
            prefix: new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(ProbePrefix))),
            finalizer: new HarmonyMethod(AccessTools.Method(typeof(Hooks), nameof(ProbeFinalizer))));
    }

    internal static void ProbePrefix()
    {
        _startStack.Push(Stopwatch.GetTimestamp());
        _childStack.Push(0);
    }

    internal static void ProbeFinalizer(MethodInfo __originalMethod, Exception __exception)
    {
        if (_startStack.Count == 0)
            return;

        long start = _startStack.Pop();
        long childTicks = _childStack.Pop();
        long totalTicks = Stopwatch.GetTimestamp() - start;
        if (_childStack.Count > 0)
            _childStack.Push(_childStack.Pop() + totalTicks);

        if (Core.ProbeEnabled is not { Value: true })
            return;

        double selfMs = (totalTicks - childTicks) * 1000.0 / Stopwatch.Frequency;
        if (selfMs < ProbeSelfMsThreshold)
            return;

        float now = Time.unscaledTime;
        if (now - _probeLogWindowStart > 1f)
        {
            _probeLogWindowStart = now;
            _probeLogCount = 0;
        }
        if (++_probeLogCount > ProbeMaxLogsPerSecond)
            return;

        string name = $"{__originalMethod.DeclaringType?.Name}.{__originalMethod.Name}";
        double totalMs = totalTicks * 1000.0 / Stopwatch.Frequency;
        Core.Log?.Msg($"[probe] {name} self={selfMs:F1}ms total={totalMs:F1}ms");
    }
}
