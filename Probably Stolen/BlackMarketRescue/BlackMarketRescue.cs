using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(BlackMarketRescue.Core), "Black Market Rescue", "1.0.1", "chiqingsan")]
[assembly: MelonGame("Questing Goose Studio", "Probably Stolen")]

namespace BlackMarketRescue;

public sealed class Core : MelonMod
{
    internal static MelonLogger.Instance Log = null!;
    internal static Core? Current { get; private set; }

    private float _nextCheckAt = -1f;
    private float _stopCheckingAt = -1f;
    private string _pendingReason = string.Empty;

    public override void OnInitializeMelon()
    {
        Current = this;
        Log = LoggerInstance;
        Log.Msg("Black Market Rescue loaded. It will rescue trashcan items to the counter after black market handling and day start.");
    }

    public override void OnLateInitializeMelon()
    {
        try
        {
            var harmony = new HarmonyLib.Harmony("chiqingsan.black-market-rescue");
            PatchPostfix(harmony, typeof(StoreOperationManager), nameof(StoreOperationManager.HandleBlackMarket), nameof(Hooks.AfterBlackMarket));
            PatchPostfix(harmony, typeof(StoreOperationManager), nameof(StoreOperationManager.OnDayStart), nameof(Hooks.AfterDayStart));
            PatchPostfix(harmony, typeof(ExchangeManager), nameof(ExchangeManager.HandleExchange), nameof(Hooks.AfterExchange));
            PatchPostfix(harmony, typeof(PlayerStore), nameof(PlayerStore.BeginDay), nameof(Hooks.AfterBeginDay));
            PatchPostfix(harmony, typeof(PlayerStore), nameof(PlayerStore.HandleOperation), nameof(Hooks.AfterHandleOperation));
            PatchPostfix(harmony, typeof(PlayerStore), nameof(PlayerStore.OnLateShutterOpen), nameof(Hooks.AfterLateShutterOpen));
            PatchPostfix(harmony, typeof(StoreEventManager), nameof(StoreEventManager.OnDayStart), nameof(Hooks.AfterEventDayStart));
        }
        catch (Exception ex)
        {
            Log.Error($"Harmony patch failed: {ex}");
        }
    }

    public override void OnUpdate()
    {
        var now = Time.unscaledTime;
        if (_nextCheckAt < 0f || now < _nextCheckAt)
            return;

        if (now > _stopCheckingAt)
        {
            _nextCheckAt = -1f;
            _stopCheckingAt = -1f;
            _pendingReason = string.Empty;
            return;
        }

        var result = Rescue.Run(_pendingReason);
        if (result.ShouldRetry)
        {
            _nextCheckAt = now + 0.5f;
            return;
        }

        _nextCheckAt = -1f;
        _stopCheckingAt = -1f;
        _pendingReason = string.Empty;
    }

    internal static void Schedule(string reason)
    {
        var mod = Current;
        if (mod == null)
            return;

        var now = Time.unscaledTime;
        mod._nextCheckAt = now + 0.25f;
        mod._stopCheckingAt = Math.Max(mod._stopCheckingAt, now + 30f);
        mod._pendingReason = reason;
        Log.Msg($"Scheduled trashcan rescue after {reason}.");
    }

    private static void PatchPostfix(HarmonyLib.Harmony harmony, Type type, string methodName, string postfixName)
    {
        var original = AccessTools.Method(type, methodName, Type.EmptyTypes);
        if (original == null)
        {
            Log.Warning($"Could not find {type.Name}.{methodName}; fallback hooks may still work.");
            return;
        }

        var postfix = AccessTools.Method(typeof(Hooks), postfixName);
        if (postfix == null)
            throw new MissingMethodException(typeof(Hooks).FullName, postfixName);

        harmony.Patch(original, postfix: new HarmonyMethod(postfix));
        Log.Msg($"Patched {type.Name}.{methodName}.");
    }
}

internal static class Hooks
{
    private static int _lastBlackMarketFrame = -1;
    private static int _lastDayStartFrame = -1;
    private static int _lastBeginDayFrame = -1;

    internal static void AfterBlackMarket()
    {
        if (_lastBlackMarketFrame == Time.frameCount)
            return;

        _lastBlackMarketFrame = Time.frameCount;
        Core.Schedule("black market");
    }

    internal static void AfterDayStart()
    {
        if (_lastDayStartFrame == Time.frameCount)
            return;

        _lastDayStartFrame = Time.frameCount;
        Core.Schedule("day start");
    }

    internal static void AfterExchange()
    {
        Core.Schedule("exchange");
    }

    internal static void AfterBeginDay()
    {
        if (_lastBeginDayFrame == Time.frameCount)
            return;

        _lastBeginDayFrame = Time.frameCount;
        Core.Schedule("begin day");
    }

    internal static void AfterHandleOperation()
    {
        Core.Schedule("player operation");
    }

    internal static void AfterLateShutterOpen()
    {
        Core.Schedule("late shutter open");
    }

    internal static void AfterEventDayStart()
    {
        Core.Schedule("event day start");
    }
}

internal static class Rescue
{
    internal readonly struct RescueResult
    {
        internal RescueResult(bool shouldRetry)
        {
            ShouldRetry = shouldRetry;
        }

        internal bool ShouldRetry { get; }
    }

    internal static RescueResult Run(string reason)
    {
        try
        {
            var entry = EmporiumEntry.Instance;
            if (entry == null)
                return new RescueResult(true);

            var trash = entry.trashcanInvElement;
            if (trash == null)
                return new RescueResult(true);

            var trashItems = SnapshotItems(trash);
            if (trashItems.Count == 0)
                return new RescueResult(true);

            var targets = CounterTargets(entry);
            if (targets.Count == 0)
            {
                Core.Log.Warning("Trashcan has items, but no counter inventory is available yet.");
                return new RescueResult(true);
            }

            var moved = 0;
            var stuck = 0;
            foreach (var item in trashItems)
            {
                if (item == null || !Same(item.parentInventory, trash))
                    continue;

                if (MoveToFirstAvailableCounter(item, targets, trash))
                    moved++;
                else
                    stuck++;
            }

            Validate(trash);
            foreach (var target in targets)
                Validate(target);

            if (moved > 0)
            {
                var title = IsChineseLocale() ? "黑市回收" : "Black Market Rescue";
                var body = IsChineseLocale()
                    ? $"已从垃圾桶取回 {moved} 件物品并放到柜台。"
                    : $"Moved {moved} item(s) from the trashcan to the counter.";
                StoreUIManager.Instance?.Notify(title, body);
                Core.Log.Msg($"Moved {moved} trashcan item(s) to the counter after {reason}.");
            }

            if (stuck > 0)
                Core.Log.Warning($"Could not move {stuck} trashcan item(s); the counter may be full.");

            return new RescueResult(false);
        }
        catch (Exception ex)
        {
            Core.Log.Warning($"Rescue check failed: {ex.Message}");
            return new RescueResult(true);
        }
    }

    private static List<GameItem> SnapshotItems(GameInventory inventory)
    {
        var result = new List<GameItem>();
        try
        {
            var childItems = inventory.childItems;
            if (childItems != null)
            {
                foreach (var item in childItems)
                {
                    if (item != null)
                        result.Add(item);
                }
            }
        }
        catch
        {
        }

        try
        {
            var grid = inventory.TryCast<GameGridInventory>();
            var items = grid?.items;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item != null && !ContainsSame(result, item))
                        result.Add(item);
                }
            }
        }
        catch
        {
        }

        try
        {
            var scrollable = inventory.TryCast<GameGridScrollableInventory>();
            var items = scrollable?.items;
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item != null && !ContainsSame(result, item))
                        result.Add(item);
                }
            }
        }
        catch
        {
        }

        return result;
    }

    private static List<GameInventory> CounterTargets(EmporiumEntry entry)
    {
        var result = new List<GameInventory>();
            AddTarget(result, entry.frontInvinvElement);
            AddTarget(result, entry.backInvinvElementCounter);
            return result;
    }

    private static void AddTarget(List<GameInventory> targets, GameInventory? inventory)
    {
        if (inventory == null)
            return;

        foreach (var target in targets)
        {
            if (Same(target, inventory))
                return;
        }

        targets.Add(inventory);
    }

    private static bool MoveToFirstAvailableCounter(GameItem item, List<GameInventory> targets, GameInventory source)
    {
        foreach (var target in targets)
        {
            if (target == null || Same(source, target) || IsInsertLocked(target) || !CanAccept(target, item))
                continue;

            if (Move(item, source, target))
                return true;
        }

        return false;
    }

    private static bool Move(GameItem item, GameInventory source, GameInventory target)
    {
        var removed = false;
        try
        {
            removed = source.Expel(item);
        }
        catch (Exception ex)
        {
            Core.Log.Warning($"Expel failed for {SafeName(item)}: {ex.Message}");
        }

        if (!removed)
            return false;

        if (Place(item, target))
            return true;

        if (!Place(item, source))
            Core.Log.Warning($"Failed to restore {SafeName(item)} to the trashcan after counter placement failed.");

        return false;
    }

    private static bool CanAccept(GameInventory inventory, GameItem item)
    {
        try
        {
            return inventory.TryFindOneValidInventorySlot(item) != null;
        }
        catch
        {
            return false;
        }
    }

    private static bool Place(GameItem item, GameInventory inventory)
    {
        try
        {
            var slot = inventory.TryFindOneValidInventorySlot(item);
            if (slot == null)
                return false;

            slot.AcceptUnchecked();
            return Same(item.parentInventory, inventory);
        }
        catch (Exception ex)
        {
            Core.Log.Warning($"Place failed for {SafeName(item)}: {ex.Message}");
            return false;
        }
    }

    private static bool IsInsertLocked(GameInventory inventory)
    {
        try
        {
            return inventory.IsInsertLocked();
        }
        catch
        {
            return true;
        }
    }

    private static void Validate(GameInventory inventory)
    {
        try
        {
            inventory.Validate();
        }
        catch
        {
        }
    }

    private static bool ContainsSame(List<GameItem> items, GameItem item)
    {
        foreach (var existing in items)
        {
            if (Same(existing, item))
                return true;
        }

        return false;
    }

    private static bool Same(Il2CppObjectBase? left, Il2CppObjectBase? right)
    {
        var leftPointer = Pointer(left);
        return leftPointer != IntPtr.Zero && leftPointer == Pointer(right);
    }

    private static IntPtr Pointer(Il2CppObjectBase? value)
    {
        try
        {
            return value?.Pointer ?? IntPtr.Zero;
        }
        catch
        {
            return IntPtr.Zero;
        }
    }

    private static string SafeName(GameItem item)
    {
        try
        {
            var displayName = item.GetDisplayName(false);
            if (!string.IsNullOrEmpty(displayName))
                return displayName;
        }
        catch
        {
        }

        try
        {
            return item.identifier ?? "<unknown>";
        }
        catch
        {
            return "<unknown>";
        }
    }

    private static bool IsChineseLocale()
    {
        try
        {
            var locale = LocHelper.GetCurrentLocaleCode();
            return !string.IsNullOrEmpty(locale) && locale.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
