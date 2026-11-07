using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace FastLoad;

/// <summary>
/// Harmony 钩子：S0 阶段打点 + S1 读档提速（预览快速路径 / 精简副本 / 延迟 sold 解码）。
/// 所有提速分支都在 try/catch 内，任何异常都回退原版逻辑。
/// </summary>
internal static class Hooks
{
    private static Timing.Probe _startProbe;
    private static Timing.Probe _preloadProbe;
    private static Timing.Probe _loadGameProbe;
    private static Timing.Probe _ioProbe;
    private static Timing.Probe _decodeNodesProbe;
    private static Timing.Probe _itemProbe;

    private static string _ioName = "";
    private static string _itemName = "";
    private static bool _itemSkipped;
    private static bool _soldDeferred;
    private static bool _soldForce;
    private static List<ItemFeature>? _orphanFeatures;

    // FastDecode：批量解码期间推迟整库校验
    private static int _bulkDepth;
    private static bool _profile;
    private static int _shapeCalls;
    private static long _shapeTicks;
    private static int _shapeFallbacks;
    private static readonly List<GameInventory> TouchedInventories = new();
    private static int _ctorCalls;
    private static long _ctorTicks;
    private static long _ctorStart;
    private static int _ctorDepth;

    // A：细粒度打点（嵌套栈：区分“含子耗时”和“自耗时”）
    private struct SubFrame
    {
        internal string Key;
        internal long Start;
        internal long ChildTicks;
    }

    private static readonly List<SubFrame> SubStack = new();
    private static readonly HashSet<string> SubSeen = new();
    private static readonly Dictionary<string, int> SubCounts = new();
    private static readonly Dictionary<string, long> SubTicks = new();
    private static readonly Dictionary<string, long> SubExclusiveTicks = new();
    private static readonly List<string> SubOrder = new();

    // A：DirectoryMaster.Item 最慢物品（用于找出“哪类物品”拖慢解码）
    private const int SlowItemLimit = 12;
    private static readonly List<SlowItem> SlowItems = new();
    private static readonly List<long> ItemStartTicks = new();
    private static readonly List<string> ItemIdentifiers = new();

    private sealed class SlowItem
    {
        internal long Ticks;
        internal string Id = "";
    }

    // B：形状 / 本地化缓存
    private static readonly Dictionary<string, GridShapeBuilder> ShapeCache = new();
    private static readonly Dictionary<string, string> LocCache = new();
    private static string? _pendingShapeKey;
    private static string? _pendingLocKey;
    private static int _shapeCacheHits;
    private static int _shapeBuilt;
    private static int _locCacheHits;
    private static bool _validateSkipped;
    private static int _ctorSkippedValidate;
    private static int _unparentedSkipped;

    // ---------------------------------------------------------------- S0

    internal static void StartPrefix()
    {
        Timing.BeginTotal();
        _startProbe = Timing.Probe.Begin();
        _soldDeferred = false;
        _bulkDepth = 0;
        _profile = Timing.Enabled;
        _shapeCalls = 0;
        _shapeTicks = 0;
        _shapeFallbacks = 0;
        _ctorCalls = 0;
        _ctorTicks = 0;
        _ctorDepth = 0;
        _shapeCacheHits = 0;
        _shapeBuilt = 0;
        _locCacheHits = 0;
        _validateSkipped = false;
        _ctorSkippedValidate = 0;
        _unparentedSkipped = 0;
        SubStack.Clear();
        SubSeen.Clear();
        SubCounts.Clear();
        SubTicks.Clear();
        SubExclusiveTicks.Clear();
        SubOrder.Clear();
        SlowItems.Clear();
        ItemStartTicks.Clear();
        ItemIdentifiers.Clear();
        ShapeCache.Clear();
        LocCache.Clear();
        TouchedInventories.Clear();
        var ngd = NewGameData.Instance;
        Timing.Add($"读档开始：slot={ngd?.saveSlot.ToString() ?? "null"}，file={ngd?.saveFile ?? "(latest)"}");
    }

    internal static void StartPostfix()
    {
        Timing.Add($"EmporiumEntry.Start 总计 {_startProbe.End()}");
        Timing.EndTotal();
    }

    internal static void LoadGamePrefix()
    {
        _loadGameProbe = Timing.Probe.Begin();
    }

    internal static void LoadGamePostfix()
    {
        Timing.Add($"PlayerStore.LoadGame {_loadGameProbe.End()}");
        if (_profile && _ctorCalls > 0)
        {
            var ms = _ctorTicks * 1000.0 / Stopwatch.Frequency;
            Timing.Add($"GameItemElement 构造统计：{_ctorCalls:N0} 次，合计 {ms:F0}ms（{ms / _ctorCalls:F2}ms/件）");
        }

        if (_profile)
        {
            ReportSubTimers();
        }
    }

    private static void ReportSubTimers()
    {
        try
        {
            // 原方法抛异常时帧可能没闭合，先按当前时间收尾。
            while (SubStack.Count > 0)
            {
                SubLeave(SubStack[SubStack.Count - 1].Key);
            }

            var keys = new List<string>(SubOrder);
            keys.Sort((a, b) => SubExclusiveTicks.GetValueOrDefault(b).CompareTo(SubExclusiveTicks.GetValueOrDefault(a)));
            var totalExclusive = 0.0;
            foreach (var key in keys)
            {
                if (!SubCounts.TryGetValue(key, out var count) || count <= 0)
                {
                    continue;
                }

                var incl = SubTicks.GetValueOrDefault(key) * 1000.0 / Stopwatch.Frequency;
                var excl = SubExclusiveTicks.GetValueOrDefault(key) * 1000.0 / Stopwatch.Frequency;
                totalExclusive += excl;
                Timing.Add($"[细] {key}：{count:N0} 次，含子 {incl:F0}ms / 自耗时 {excl:F0}ms（{excl / count:F2}ms/次）");
            }

            Timing.Add($"[细] 自耗时合计 {totalExclusive:F0}ms；形状缓存 {_shapeCacheHits:N0} 命中 / {_shapeBuilt:N0} 构建，本地化缓存 {_locCacheHits:N0} 命中，"
                + $"构造期跳过校验 {_ctorSkippedValidate:N0} 次，未挂载跳过校验 {_unparentedSkipped:N0} 次");

            if (SlowItems.Count > 0)
            {
                SlowItems.Sort((a, b) => b.Ticks.CompareTo(a.Ticks));
                var sb = new StringBuilder();
                sb.Append("[慢] DirectoryMaster.Item 最慢：");
                var n = Math.Min(SlowItemLimit, SlowItems.Count);
                for (var i = 0; i < n; i++)
                {
                    if (i > 0)
                    {
                        sb.Append("；");
                    }

                    var item = SlowItems[i];
                    sb.Append($"{item.Id} {item.Ticks * 1000.0 / Stopwatch.Frequency:F1}ms");
                }

                Timing.Add(sb.ToString());
            }
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"细粒度打点汇总失败: {ex.Message}");
        }
    }

    internal static void WithIORetryPrefix(Action fileOperation)
    {
        _ioProbe = Timing.Probe.Begin();
        _ioName = Describe(fileOperation);
    }

    internal static void WithIORetryPostfix()
    {
        Timing.Add($"SaveFiles.WithIORetry[{_ioName}] {_ioProbe.End()}");
    }

    internal static void DecodeNodesPrefix()
    {
        _decodeNodesProbe = Timing.Probe.Begin();
    }

    internal static void DecodeNodesPostfix(GameItem __result)
    {
        Timing.Add($"SaveManager.DecodeNodes{(__result == null ? "(空)" : "")} {_decodeNodesProbe.End()}");
    }

    // ------------------------------------------------- S1-① PreLoadStore

    internal static bool PreLoadStorePrefix(ref PlayerStore __result)
    {
        _preloadProbe = Timing.Probe.Begin();
        Timing.Add("-- PlayerStore.PreLoadStore --");
        try
        {
            if (Core.SkipPreLoadStore?.Value != true)
            {
                return true;
            }

            var ngd = NewGameData.Instance;
            if (ngd == null || ngd.saveSlot == -1)
            {
                return true;
            }

            var source = SaveSurgeon.ResolveSource(ngd);
            if (string.IsNullOrEmpty(source) || !ES3.FileExists(source))
            {
                return true;
            }

            var plan = SaveSurgeon.GetPlan(source);
            if (plan == null || !plan.PreloadReady)
            {
                return true;
            }

            __result = ES3.Load<PlayerStore>("playerStore", SaveSurgeon.PreloadFile);
            Timing.Add($"PreLoadStore 快速路径命中：原始 {plan.RawLength:N0} 字符 → 预览 {plan.PreloadLength:N0} 字符");
            return false;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"PreLoadStore 快速路径失败，回退原版: {ex.Message}");
            return true;
        }
    }

    internal static void PreLoadStorePostfix()
    {
        Timing.Add($"PlayerStore.PreLoadStore {_preloadProbe.End()}");
    }

    // ------------------------------------------------- S1-② 精简副本

    internal static void ResolveLoadFilePostfix(ref string __result)
    {
        if (Core.SlimLoadFile?.Value != true || __result == null)
        {
            return;
        }

        try
        {
            var plan = SaveSurgeon.Current;
            if (plan == null || !string.Equals(__result, plan.SourceFile, StringComparison.OrdinalIgnoreCase))
            {
                // PreLoadStore 被关闭时不会预构建，这里补一次。
                plan = SaveSurgeon.GetPlan(__result);
            }

            if (plan == null || !plan.SlimReady)
            {
                return;
            }

            if (!string.Equals(__result, plan.SourceFile, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            __result = SaveSurgeon.SlimFile;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"精简副本切换失败，回退原版: {ex.Message}");
        }
    }

    // ------------------------------------------------- S1-③ 延迟 sold

    internal static bool DecodeSaveItemPrefix(PlayerStore __instance, ref string JSON, GameInventory gameInventory)
    {
        _itemProbe = Timing.Probe.Begin();
        _itemSkipped = false;
        _itemName = SafeName(gameInventory);
        _bulkDepth++;

        try
        {
            var plan = SaveSurgeon.Current;

            // 延迟解码当天售出库存：跳过 368 件物品，用到时由 EnsureSoldDecoded 补回。
            if (Core.LazySoldDecode?.Value == true && plan != null && !_soldForce
                && IsSoldInventory(gameInventory))
            {
                if (plan.InvJson.ContainsKey("soldInvJSON"))
                {
                    _itemSkipped = true;
                    _soldDeferred = true;
                    Timing.Add($"soldInvJSON 延迟解码（跳过 {JSON?.Length ?? 0:N0} 字符）");
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(JSON))
            {
                return true;
            }

            // 精简副本里被清空的库存串，按目标库存补回原值。
            var kind = FindInvKind(gameInventory);
            if (plan != null && kind != null && plan.InvJson.TryGetValue(kind, out var real)
                && !string.IsNullOrEmpty(real))
            {
                JSON = real;
                Timing.Add($"{kind} 从缓存补回 {real.Length:N0} 字符");
                return true;
            }

            Core.Log?.Warning($"读档缺少 {kind ?? _itemName} 的库存数据，跳过该库存解码以免崩溃");
            _itemSkipped = true;
            return false;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"DecodeSaveItem 前缀异常（回退原版）: {ex.Message}");
            return true;
        }
    }

    internal static void DecodeSaveItemPostfix(string JSON, GameInventory gameInventory)
    {
        try
        {
            if (Timing.Enabled && !_itemSkipped)
            {
                Timing.Add($"{_itemName} 解码 {JSON?.Length ?? 0:N0} 字符 {_itemProbe.End()}");
            }
        }
        finally
        {
            if (_bulkDepth > 0)
            {
                _bulkDepth--;
            }

            if (_bulkDepth == 0)
            {
                FlushDeferredValidates();
            }
        }
    }

    // ------------------------------------------------- S1-④ FastDecode

    /// <summary>
    /// 批量解码时的 ValidateShapeState 替代实现：只做自身形状/状态同步与自身校验，
    /// 跳过原版每次挂载都触发的 parentInventory.Validate()（单次 O(库存件数)，累计 O(n²)），
    /// 目标库存推迟到本次 DecodeSaveItem 结束后统一校验一次。父物品形状传播仍然照做。
    /// </summary>
    internal static bool ValidateShapeStatePrefix(GameItem __instance)
    {
        if (_bulkDepth <= 0 || Core.FastDecode?.Value != true)
        {
            return true;
        }

        try
        {
            if (__instance == null)
            {
                return false;
            }

            var start = Stopwatch.GetTimestamp();
            __instance.SyncModifiedState();

            var inv = __instance.parentInventory;
            if (inv != null)
            {
                __instance.Validate();
                TrackInventory(inv);
                PropagateParentShapes(inv);
            }
            else if (Core.SkipUnparentedValidate?.Value == true)
            {
                // 物品尚未挂到库存：渲染校验推迟到挂载时（FastDecode 前缀会补一次 Validate）。
                _unparentedSkipped++;
            }
            else
            {
                __instance.Validate();
            }

            _shapeTicks += Stopwatch.GetTimestamp() - start;
            _shapeCalls++;
            return false;
        }
        catch (Exception ex)
        {
            _shapeFallbacks++;
            if (_shapeFallbacks <= 3)
            {
                Core.Log?.Warning($"FastDecode 单件校验异常，回退原版: {ex.Message}");
            }

            return true;
        }
    }

    private static void TrackInventory(GameInventory inv)
    {
        if (inv != null && !TouchedInventories.Contains(inv))
        {
            TouchedInventories.Add(inv);
        }
    }

    private static void PropagateParentShapes(GameInventory inv)
    {
        try
        {
            foreach (var parent in GraphUtils.FindParentsType<GameItem>(inv))
            {
                if (parent == null)
                {
                    continue;
                }

                try
                {
                    parent.SyncModifiedState();
                    parent.Validate();
                }
                catch
                {
                    // 单个父物品异常不影响其余物品。
                }
            }
        }
        catch
        {
            // 父物品形状传播失败可忽略，末尾整库校验仍会刷新位置。
        }
    }

    private static void FlushDeferredValidates()
    {
        try
        {
            if (TouchedInventories.Count == 0)
            {
                return;
            }

            if (Core.FastDecode?.Value != true)
            {
                TouchedInventories.Clear();
                return;
            }

            var probe = Timing.Probe.Begin();
            var ok = 0;
            foreach (var inv in TouchedInventories)
            {
                try
                {
                    if (inv != null)
                    {
                        inv.Validate();
                        ok++;
                    }
                }
                catch
                {
                    // 临时包裹库存可能已销毁，忽略。
                }
            }

            var shapeMs = _shapeTicks * 1000.0 / Stopwatch.Frequency;
            if (Timing.Enabled)
            {
                Timing.Add($"FastDecode：拦截逐件整库校验 {_shapeCalls:N0} 次（自校验 {shapeMs:F0}ms），"
                    + $"末尾统一校验 {ok}/{TouchedInventories.Count} 个库存 {probe.End()}");
            }

            TouchedInventories.Clear();
            _shapeCalls = 0;
            _shapeTicks = 0;
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"FastDecode 末尾校验失败: {ex.Message}");
        }
    }

    internal static void ItemElementCtorPrefix()
    {
        if (_bulkDepth <= 0)
        {
            return;
        }

        _ctorDepth++;
        if (_profile)
        {
            _ctorStart = Stopwatch.GetTimestamp();
            SubEnter("GameItemElement..ctor/4");
        }
    }

    internal static void ItemElementCtorPostfix()
    {
        if (_bulkDepth <= 0)
        {
            return;
        }

        if (_profile)
        {
            if (_ctorStart != 0)
            {
                _ctorTicks += Stopwatch.GetTimestamp() - _ctorStart;
                _ctorStart = 0;
            }

            SubLeave("GameItemElement..ctor/4");
        }

        _ctorCalls++;
        if (_ctorDepth > 0)
        {
            _ctorDepth--;
        }
    }

    internal static void SaveGamePrefix()
    {
        EnsureSoldDecoded();
    }

    // ------------------------------------------------- A：细粒度通用计时

    internal static void SubTimerPrefix(MethodBase __originalMethod)
    {
        if (!_profile || __originalMethod == null)
        {
            return;
        }

        try
        {
            SubEnter(SubKey(__originalMethod));
        }
        catch
        {
            // 打点失败不影响游戏逻辑。
        }
    }

    internal static void SubTimerPostfix(MethodBase __originalMethod)
    {
        if (!_profile || __originalMethod == null)
        {
            return;
        }

        try
        {
            SubLeave(SubKey(__originalMethod));
        }
        catch
        {
            // 打点失败不影响游戏逻辑。
        }
    }

    /// <summary>DirectoryMaster.Item 专用：除通用计时外，记录每个物品的耗时用于找“最慢物品”。</summary>
    internal static void ItemTimerPrefix(string identifier, MethodBase __originalMethod)
    {
        if (!_profile)
        {
            return;
        }

        try
        {
            ItemStartTicks.Add(Stopwatch.GetTimestamp());
            ItemIdentifiers.Add(identifier ?? "(null)");
            SubEnter(SubKey(__originalMethod));
        }
        catch
        {
            // 打点失败不影响游戏逻辑。
        }
    }

    internal static void ItemTimerPostfix(string identifier, MethodBase __originalMethod)
    {
        if (!_profile)
        {
            return;
        }

        try
        {
            SubLeave(SubKey(__originalMethod));
            if (ItemStartTicks.Count == 0)
            {
                return;
            }

            var idx = ItemStartTicks.Count - 1;
            var start = ItemStartTicks[idx];
            var id = ItemIdentifiers[idx];
            ItemStartTicks.RemoveAt(idx);
            ItemIdentifiers.RemoveAt(idx);

            SlowItems.Add(new SlowItem { Ticks = Stopwatch.GetTimestamp() - start, Id = id });
            if (SlowItems.Count > SlowItemLimit * 4)
            {
                SlowItems.Sort((a, b) => b.Ticks.CompareTo(a.Ticks));
                SlowItems.RemoveRange(SlowItemLimit, SlowItems.Count - SlowItemLimit);
            }
        }
        catch
        {
            // 打点失败不影响游戏逻辑。
        }
    }

    private static string SubKey(MethodBase method)
    {
        return $"{method.DeclaringType?.Name}.{method.Name}/{method.GetParameters().Length}";
    }

    private static void SubEnter(string key)
    {
        if (SubSeen.Add(key))
        {
            SubOrder.Add(key);
        }

        SubStack.Add(new SubFrame { Key = key, Start = Stopwatch.GetTimestamp() });
    }

    private static void SubLeave(string key)
    {
        var idx = -1;
        for (var i = SubStack.Count - 1; i >= 0; i--)
        {
            if (SubStack[i].Key == key)
            {
                idx = i;
                break;
            }
        }

        if (idx < 0)
        {
            return;
        }

        var now = Stopwatch.GetTimestamp();

        // 收尾比目标帧更晚进入、但没正常闭合的帧（原方法抛异常），只计含子耗时。
        while (SubStack.Count - 1 > idx)
        {
            var top = SubStack[SubStack.Count - 1];
            SubStack.RemoveAt(SubStack.Count - 1);
            AddSubTicks(top.Key, now - top.Start, 0);
        }

        var frame = SubStack[idx];
        SubStack.RemoveAt(idx);
        var elapsed = now - frame.Start;
        var exclusive = elapsed - frame.ChildTicks;
        if (exclusive < 0)
        {
            exclusive = 0;
        }

        AddSubTicks(key, elapsed, exclusive);

        if (SubStack.Count > 0)
        {
            var parent = SubStack[SubStack.Count - 1];
            parent.ChildTicks += elapsed;
            SubStack[SubStack.Count - 1] = parent;
        }
    }

    private static void AddSubTicks(string key, long inclusive, long exclusive)
    {
        SubTicks[key] = SubTicks.GetValueOrDefault(key) + inclusive;
        SubExclusiveTicks[key] = SubExclusiveTicks.GetValueOrDefault(key) + exclusive;
        SubCounts[key] = SubCounts.GetValueOrDefault(key) + 1;
    }

    // ------------------------------------------------- B-① 形状缓存

    internal static bool BuildShapeFromSpritePrefix(string spriteAtlasPath, string spritePath, ref GridShapeBuilder __result)
    {
        if (Core.CacheItemShapes?.Value == true)
        {
            try
            {
                var key = spriteAtlasPath + "|" + spritePath;
                if (ShapeCache.TryGetValue(key, out var cached) && cached != null)
                {
                    _shapeCacheHits++;
                    __result = cached;
                    return false;
                }

                _pendingShapeKey = key;
            }
            catch
            {
                _pendingShapeKey = null;
            }
        }

        if (_profile)
        {
            SubEnter("GameItem.BuildShapeFromSprite/2");
        }

        return true;
    }

    internal static void BuildShapeFromSpritePostfix(GridShapeBuilder __result)
    {
        if (_profile)
        {
            SubLeave("GameItem.BuildShapeFromSprite/2");
        }

        var key = _pendingShapeKey;
        _pendingShapeKey = null;
        if (key != null && __result != null)
        {
            ShapeCache[key] = __result;
            _shapeBuilt++;
        }
    }

    // ------------------------------------------------- B-② 本地化缓存

    internal static bool RelocalizePrefix(string text, ref string __result)
    {
        if (Core.CacheLocalization?.Value == true)
        {
            try
            {
                if (!string.IsNullOrEmpty(text) && LocCache.TryGetValue(text, out var cached))
                {
                    _locCacheHits++;
                    __result = cached;
                    return false;
                }

                _pendingLocKey = text;
            }
            catch
            {
                _pendingLocKey = null;
            }
        }

        if (_profile)
        {
            SubEnter("LocHelper.RelocalizeIfMissing/1");
        }

        return true;
    }

    internal static void RelocalizePostfix(string text, string __result)
    {
        if (_profile)
        {
            SubLeave("LocHelper.RelocalizeIfMissing/1");
        }

        var key = _pendingLocKey;
        _pendingLocKey = null;
        if (key != null && __result != null)
        {
            LocCache[key] = __result;
        }
    }

    // ------------------------------------------------- B-③ 构造期校验跳过

    internal static bool ItemValidatePrefix()
    {
        _validateSkipped = false;
        if (_bulkDepth > 0 && _ctorDepth > 0 && Core.SkipCtorValidate?.Value == true)
        {
            _validateSkipped = true;
            _ctorSkippedValidate++;
            return false;
        }

        if (_profile)
        {
            SubEnter("GameItemElement.Validate/0");
        }

        return true;
    }

    internal static void ItemValidatePostfix()
    {
        if (_validateSkipped)
        {
            return;
        }

        if (_profile)
        {
            SubLeave("GameItemElement.Validate/0");
        }
    }

    /// <summary>延迟 sold 解码会跳过 LoadGame 里的特性重挂，先把特性列表快照下来，等补解码时回填。</summary>
    internal static void GetAllItemsPrefix()
    {
        try
        {
            if (!_soldDeferred || SaveSurgeon.Current == null)
            {
                return;
            }

            var list = PlayerStore.Instance?.savedItemFeatureList;
            if (list == null || list.Count == 0)
            {
                return;
            }

            _orphanFeatures = new List<ItemFeature>(list);
            Timing.Add($"快照待回填特性 {_orphanFeatures.Count} 条（sold 延迟解码）");
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"快照 sold 特性失败: {ex.Message}");
        }
    }

    internal static void OnShutterOpenedPrefix()
    {
        EnsureSoldDecoded();
    }

    internal static void GetItemFromSoldElementPrefix()
    {
        EnsureSoldDecoded();
    }

    private static void EnsureSoldDecoded()
    {
        if (!_soldDeferred || _soldForce)
        {
            return;
        }

        _soldDeferred = false;
        var plan = SaveSurgeon.Current;
        if (plan == null || !plan.InvJson.TryGetValue("soldInvJSON", out var real) || string.IsNullOrEmpty(real))
        {
            Core.Log?.Warning("无法补解码 sold 库存（缺少原始 JSON），本次读档将保持售出库存为空");
            return;
        }

        _soldForce = true;
        try
        {
            var store = PlayerStore.Instance;
            var sold = EmporiumEntry.Instance != null ? EmporiumEntry.Instance.soldElement : null;
            if (store == null || sold == null)
            {
                return;
            }

            var probe = Timing.Probe.Begin();
            store.DecodeSaveItem(real, sold);
            Timing.Add($"soldInvJSON 补解码 {real.Length:N0} 字符 {probe.End()}");
            RestoreSoldFeatures(sold);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"sold 库存补解码失败: {ex.Message}");
        }
        finally
        {
            _soldForce = false;
        }
    }

    /// <summary>把 LoadGame 时快照下来的特性回填到刚补解码的 sold 物品上。</summary>
    private static void RestoreSoldFeatures(GameInventory sold)
    {
        var orphan = _orphanFeatures;
        _orphanFeatures = null;
        if (orphan == null || orphan.Count == 0)
        {
            return;
        }

        try
        {
            var byUniqueId = new Dictionary<int, GameItem>();
            foreach (var item in sold.childItems)
            {
                if (item != null && !byUniqueId.ContainsKey(item.uniqueId))
                {
                    byUniqueId.Add(item.uniqueId, item);
                }
            }

            var restored = 0;
            foreach (var feature in orphan)
            {
                if (feature == null)
                {
                    continue;
                }

                if (byUniqueId.TryGetValue(feature.parentItemUniqueId, out var item))
                {
                    item.itemFeatures.Add(feature);
                    restored++;
                }
            }

            Timing.Add($"sold 物品特性回填 {restored} 条");
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"sold 物品特性回填失败: {ex.Message}");
        }
    }

    // ------------------------------------------------------------- helpers

    private static bool IsSoldInventory(GameInventory inv)
    {
        var entry = EmporiumEntry.Instance;
        return entry != null && entry.soldElement != null && inv == entry.soldElement;
    }

    private static string? FindInvKind(GameInventory inv)
    {
        var entry = EmporiumEntry.Instance;
        if (entry == null || inv == null)
        {
            return null;
        }

        try
        {
            if (inv == entry.invElement) return "mainInvJSON";
            if (inv == entry.backInvinvElement) return "backInvJSON";
            if (inv == entry.hiddenElement) return "hiddenInvJSON";
            if (inv == entry.showcaseElement) return "showcaseInvJSON";
            if (inv == entry.soldElement) return "soldInvJSON";
            if (inv == entry.hirelingInv) return "hirelingInvJSON";
            if (inv == entry.vendingMachineElement) return "vendingMachineInvJSON";
            if (inv == entry.vendingFountainElement) return "vendingFountainInvJSON";
            if (inv == entry.cassettePlayerElement) return "cassettePlayerInvJSON";

            var trash = entry.trashcan != null && entry.trashcan.contentWindow != null
                ? entry.trashcan.contentWindow.child as GameInventory
                : null;
            if (trash != null && inv == trash) return "trashcanInvJSON";

            var dossier = entry.dossier != null && entry.dossier.contentWindow != null
                ? entry.dossier.contentWindow.child as GameInventory
                : null;
            if (dossier != null && inv == dossier) return "docInvJSON";
        }
        catch
        {
            // 读档早期窗口可能尚未就绪，忽略。
        }

        return null;
    }

    private static string SafeName(GameInventory? inv)
    {
        try
        {
            return inv == null ? "(null)" : inv.GetType().Name;
        }
        catch
        {
            return "?";
        }
    }

    private static string Describe(Action? action)
    {
        try
        {
            return action == null ? "null" : $"{action.Method.Name}@{action.Target?.GetType().Name ?? "static"}";
        }
        catch
        {
            return "?";
        }
    }

    internal static void ResetAll()
    {
        _soldDeferred = false;
        _soldForce = false;
        _orphanFeatures = null;
        _bulkDepth = 0;
        _shapeCalls = 0;
        _shapeTicks = 0;
        _ctorCalls = 0;
        _ctorTicks = 0;
        _ctorDepth = 0;
        _ctorSkippedValidate = 0;
        _unparentedSkipped = 0;
        _validateSkipped = false;
        _pendingShapeKey = null;
        _pendingLocKey = null;
        SubStack.Clear();
        SubSeen.Clear();
        SubCounts.Clear();
        SubTicks.Clear();
        SubExclusiveTicks.Clear();
        SubOrder.Clear();
        SlowItems.Clear();
        ItemStartTicks.Clear();
        ItemIdentifiers.Clear();
        ShapeCache.Clear();
        LocCache.Clear();
        TouchedInventories.Clear();
        Timing.Reset();
    }
}

/// <summary>阶段计时器：仅主线程使用。</summary>
internal static class Timing
{
    private static readonly Stopwatch TotalClock = new();
    private static readonly List<string> Lines = new();
    private static bool _active;

    internal static bool Enabled
    {
        get
        {
            try
            {
                return Core.TimingLog?.Value ?? true;
            }
            catch
            {
                return true;
            }
        }
    }

    internal static void BeginTotal()
    {
        if (!Enabled)
        {
            return;
        }

        Lines.Clear();
        TotalClock.Restart();
        _active = true;
    }

    internal static void Add(string line)
    {
        if (!Enabled)
        {
            return;
        }

        if (_active)
        {
            Lines.Add(line);
        }
        else
        {
            Core.Log?.Log("[FastLoad] " + line);
        }
    }

    internal static void EndTotal()
    {
        if (!_active)
        {
            return;
        }

        TotalClock.Stop();
        _active = false;
        var sb = new StringBuilder();
        sb.Append($"[FastLoad] 读档总耗时 {TotalClock.Elapsed.TotalMilliseconds:F0}ms");
        foreach (var line in Lines)
        {
            sb.Append("\n    ").Append(line);
        }

        Core.Log?.Log(sb.ToString());
        Lines.Clear();
    }

    internal static void Reset()
    {
        _active = false;
        Lines.Clear();
    }

    internal struct Probe
    {
        private long _ticks;
        private long _alloc;
        private int _gc0;
        private int _gc1;

        internal static Probe Begin()
        {
            return new Probe
            {
                _ticks = Stopwatch.GetTimestamp(),
                _alloc = GC.GetTotalMemory(false),
                _gc0 = GC.CollectionCount(0),
                _gc1 = GC.CollectionCount(1),
            };
        }

        internal string End()
        {
            var ms = (Stopwatch.GetTimestamp() - _ticks) * 1000.0 / Stopwatch.Frequency;
            var alloc = GC.GetTotalMemory(false) - _alloc;
            return $"{ms:F1}ms，分配 {alloc / 1024.0:F0}KB，GC {GC.CollectionCount(0) - _gc0}/{GC.CollectionCount(1) - _gc1}";
        }
    }
}
