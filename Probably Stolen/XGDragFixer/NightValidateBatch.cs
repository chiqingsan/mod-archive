using System;
using System.Collections.Generic;
using System.Diagnostics;
using HarmonyLib;

namespace XGDragFixer;

/// <summary>
/// Batches the redundant "refresh the whole inventory view" work that happens
/// while the vanilla night settlement is running. Every <see cref="GameItem.ValidateShapeState"/>
/// triggered by a night handler ends in <c>parentInventory.Validate()</c>, and the
/// settlement touches hundreds of items, so the same grid ends up rebuilding its
/// background, cell backdrops and every child item dozens of times for a result
/// that is only visible once the settlement has finished.
///
/// This patch remembers which inventories would have been validated and validates
/// each of them once after the settlement completes. Nothing about the settlement
/// logic itself changes; only the visual refresh is deferred. The deferred flush
/// is what actually decides the end state, so the visible result is identical.
/// </summary>
internal static class NightValidateBatch
{
    private static readonly HashSet<GameInventory> Dirty = new HashSet<GameInventory>();
    private static readonly List<Type> InventoryTypes = new List<Type>(4);

    private static bool _active;
    private static bool _uiDirty;

    internal static int SuppressedCalls { get; private set; }

    internal static int FlushedInventories { get; private set; }

    internal static int FlushFailures { get; private set; }

    internal static double FlushMs { get; private set; }

    internal static bool Enabled => Core.NightValidateBatchEnabled?.Value == true;

    internal static void Install(HarmonyLib.Harmony harmony)
    {
        InventoryTypes.Clear();
        InventoryTypes.Add(typeof(GameGridInventory));
        InventoryTypes.Add(typeof(GameGridScrollableInventory));
        InventoryTypes.Add(typeof(GameSlotInventory));
        InventoryTypes.Add(typeof(GameCharacterRaidInventory));

        var pre = new HarmonyMethod(AccessTools.Method(typeof(NightValidateBatch), nameof(ValidatePrefix)));
        var patched = 0;
        foreach (var type in InventoryTypes)
        {
            try
            {
                var validate = AccessTools.Method(type, "Validate", Type.EmptyTypes);
                if (validate == null)
                {
                    continue;
                }

                harmony.Patch(validate, pre, null);
                patched++;
            }
            catch (Exception ex)
            {
                Core.Log?.Warning($"结算刷新合并补丁失败 {type.Name}.Validate: {ex.Message}");
            }
        }

        var uiPre = new HarmonyMethod(AccessTools.Method(typeof(NightValidateBatch), nameof(UiStatePrefix)));
        foreach (var name in new[] { "UpdateInventoryStateUI", "UpdateInventoryStateUIImmediately" })
        {
            try
            {
                var method = AccessTools.Method(typeof(OverlayHandler), name, Type.EmptyTypes);
                if (method != null)
                {
                    harmony.Patch(method, uiPre, null);
                    patched++;
                }
            }
            catch (Exception ex)
            {
                Core.Log?.Warning($"结算刷新合并补丁失败 OverlayHandler.{name}: {ex.Message}");
            }
        }

        Core.Log?.Log($"[结算加速] 已挂接 {patched} 个库存刷新合并点");
    }

    /// <summary>Called after the EndNight postfix's own bookkeeping, before the flush.</summary>
    internal static void Begin()
    {
        Dirty.Clear();
        _uiDirty = false;
        SuppressedCalls = 0;
        FlushedInventories = 0;
        FlushFailures = 0;
        FlushMs = 0.0;
        _active = Enabled;
    }

    internal static void End()
    {
        if (!_active)
        {
            return;
        }

        _active = false;
        var start = Stopwatch.GetTimestamp();
        try
        {
            foreach (var inventory in Dirty)
            {
                if (inventory == null)
                {
                    continue;
                }

                try
                {
                    inventory.Validate();
                    FlushedInventories++;
                }
                catch (Exception ex)
                {
                    FlushFailures++;
                    if (FlushFailures <= 3)
                    {
                        Core.Log?.Warning($"结算刷新库存失败: {ex.GetType().Name} {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"结算刷新库存异常: {ex.GetType().Name} {ex.Message}");
        }
        finally
        {
            FlushMs = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;
            Dirty.Clear();
        }

        if (_uiDirty)
        {
            _uiDirty = false;
            try
            {
                OverlayHandler.current?.UpdateInventoryStateUI();
            }
            catch (Exception ex)
            {
                Core.Log?.Warning($"结算库存界面刷新失败: {ex.GetType().Name} {ex.Message}");
            }
        }
    }

    internal static void Reset()
    {
        _active = false;
        _uiDirty = false;
        Dirty.Clear();
        SuppressedCalls = 0;
        FlushedInventories = 0;
        FlushFailures = 0;
        FlushMs = 0.0;
    }

    private static bool ValidatePrefix(GameInventory __instance)
    {
        if (!_active)
        {
            return true;
        }

        SuppressedCalls++;
        if (__instance != null && Dirty.Add(__instance))
        {
            // A settlement can touch an inventory many times; remember it once.
        }

        return false;
    }

    private static bool UiStatePrefix()
    {
        if (!_active)
        {
            return true;
        }

        _uiDirty = true;
        return false;
    }
}
