using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBatterySwap;

/// <summary>
/// 原 AutoBatterySwap 的逻辑移植：把库存里电量更高的电池与机器/供电中的电池交换
/// "电量数值"（不移动物品），与旧版行为保持一致。
/// </summary>
internal static class BatterySwapper
{
    private static bool _busy;

    internal static void RunAll()
    {
        if (_busy) return;
        _busy = true;
        try
        {
            SwapAllMachines();
        }
        catch (Exception error)
        {
            Debug.LogError("[BetterAutomotiveBattery] night: " + error);
        }
        finally
        {
            _busy = false;
        }
    }

    internal static void TryTopUpMachine(GameItem machine, GameSlotInventory? batterySlot)
    {
        if (_busy || machine == null) return;
        _busy = true;
        try
        {
            GameSlotInventory? slot = batterySlot;
            if (slot == null)
            {
                try { slot = MachineHelper.GetBatterySlot(machine); }
                catch { return; }
            }
            if (slot == null) return;

            GameItem? current;
            try { current = slot.childItem; }
            catch { return; }
            if (current == null) return;

            int need;
            try { need = MachineryHelper.GetMachinePowerUsage(machine); }
            catch { return; }
            if (need <= 0) need = 1;

            int currentEnergy = ReadEnergy(current);
            if (currentEnergy >= need) return;

            GameItem? best = FindBest(CollectBatteries(), current, need, currentEnergy, null);
            if (best == null) return;
            Exchange(current, best, currentEnergy, "instant", need);
        }
        catch (Exception error)
        {
            Debug.LogError("[BetterAutomotiveBattery] instant: " + error);
        }
        finally
        {
            _busy = false;
        }
    }

    internal static void TryTopUpBatteryItem(GameItem? battery, int need)
    {
        if (_busy || battery == null || need <= 0) return;
        int currentEnergy = ReadEnergy(battery);
        if (currentEnergy >= need) return;

        _busy = true;
        try
        {
            GameItem? best = FindBest(CollectBatteries(), battery, need, currentEnergy, null);
            if (best == null) return;
            Exchange(battery, best, currentEnergy, "draw", need);
        }
        catch (Exception error)
        {
            Debug.LogError("[BetterAutomotiveBattery] draw: " + error);
        }
        finally
        {
            _busy = false;
        }
    }

    private static void Exchange(GameItem current, GameItem donor, int currentEnergy, string tag, int need)
    {
        int donorEnergy = ReadEnergy(donor);
        PowerHelper.SetPowerSourceAt(current, donorEnergy);
        PowerHelper.SetPowerSourceAt(donor, currentEnergy);
        try { PowerHelper.UpdateEnergyCreditSprite(current); } catch { }
        try { PowerHelper.UpdateEnergyCreditSprite(donor); } catch { }
        Debug.Log($"[BetterAutomotiveBattery] {tag} need={need} {currentEnergy}<->{donorEnergy}");
    }

    private static int ReadEnergy(GameItem? item)
    {
        if (item == null) return 0;
        try { return PowerHelper.GetAvailableEnergyFromItem(item, false); }
        catch { return 0; }
    }

    private static void SwapAllMachines()
    {
        List<GameItem> items = CollectPlayerItems();
        List<GameItem> machines = new List<GameItem>();
        foreach (GameItem item in items)
        {
            try
            {
                if (MachineHelper.GetBatterySlot(item) != null) machines.Add(item);
            }
            catch { }
        }
        if (machines.Count == 0) return;

        List<GameItem> pool = CollectBatteries(items);
        if (pool.Count == 0) return;

        HashSet<GameItem> used = new HashSet<GameItem>();
        int count = 0;
        foreach (GameItem machine in machines)
        {
            GameSlotInventory? slot;
            try { slot = MachineHelper.GetBatterySlot(machine); }
            catch { continue; }
            if (slot == null) continue;

            GameItem? current;
            try { current = slot.childItem; }
            catch { continue; }
            if (current == null) continue;

            int need;
            try { need = MachineryHelper.GetMachinePowerUsage(machine); }
            catch { continue; }
            if (need <= 0) continue;

            int currentEnergy = ReadEnergy(current);
            if (currentEnergy >= need) continue;

            GameItem? best = FindBest(pool, current, need, currentEnergy, used);
            if (best == null) continue;

            try
            {
                Exchange(current, best, currentEnergy, "night", need);
                used.Add(best);
                count++;
            }
            catch (Exception error)
            {
                Debug.LogError("[BetterAutomotiveBattery] night: " + error);
            }
        }
        if (count > 0) Debug.Log($"[BetterAutomotiveBattery] night done={count}");
    }

    /// <summary>
    /// 玩家拥有的全部物品：店铺库存（含窗口里的物品）、飞船上的物品、非船上场景的物品。
    /// </summary>
    private static List<GameItem> CollectPlayerItems()
    {
        List<GameItem> result = new List<GameItem>();
        HashSet<GameItem> seen = new HashSet<GameItem>();

        try
        {
            List<GameItem>? storeItems = PlayerStore.Instance?.FindAllItem(true);
            AddRange(result, seen, storeItems);
        }
        catch { }

        try
        {
            GameItem? ship = GameMaster.current?.shipManager?.ship;
            if (ship != null && seen.Add(ship))
            {
                result.Add(ship);
                AddRange(result, seen, GraphUtils.FindAllChildrenType<GameItem>(ship));
            }
        }
        catch { }

        try
        {
            AddRange(result, seen, GameMaster.current?.raidManager?.GetAllGameItemExcludeOnShip());
        }
        catch { }

        return result;
    }

    private static void AddRange(List<GameItem> result, HashSet<GameItem> seen, List<GameItem>? items)
    {
        if (items == null) return;
        foreach (GameItem item in items)
        {
            if (item != null && seen.Add(item)) result.Add(item);
        }
    }

    private static GameItem? FindBest(List<GameItem> pool, GameItem current, int need, int currentEnergy, HashSet<GameItem>? used)
    {
        GameItem? best = null;
        int bestEnergy = currentEnergy;
        foreach (GameItem item in pool)
        {
            if (item == null || item == current || (used != null && used.Contains(item))) continue;
            int energy = ReadEnergy(item);
            if (energy <= bestEnergy) continue;
            if (best == null)
            {
                best = item;
                bestEnergy = energy;
                continue;
            }
            bool itemCovers = energy >= need;
            bool bestCovers = bestEnergy >= need;
            if (itemCovers && !bestCovers)
            {
                best = item;
                bestEnergy = energy;
            }
            else if (itemCovers == bestCovers && energy > bestEnergy)
            {
                best = item;
                bestEnergy = energy;
            }
        }
        if (best != null && bestEnergy <= currentEnergy) return null;
        return best;
    }

    private static List<GameItem> CollectBatteries() => CollectBatteries(CollectPlayerItems());

    private static List<GameItem> CollectBatteries(List<GameItem> allItems)
    {
        List<GameItem> result = new List<GameItem>();
        HashSet<GameItem> visited = new HashSet<GameItem>();
        foreach (GameItem item in allItems)
        {
            Walk(item, result, visited);
        }
        return result;
    }

    private static void Walk(GameItem item, List<GameItem> result, HashSet<GameItem> visited)
    {
        if (item == null || !visited.Add(item)) return;

        if (IsBattery(item) && !IsInMachineSlot(item) && IsOwned(item)) result.Add(item);
        if (!IsContainer(item)) return;

        try
        {
            GraphNodeWindow? window = item.child;
            if (window != null && window.child is GameInventory inventory)
            {
                WalkInventory(inventory, result, visited);
                return;
            }
        }
        catch { }

        try
        {
            List<GraphNodeStorage>? children = item.children;
            if (children == null) return;
            foreach (GraphNodeStorage child in children)
            {
                if (child == null) continue;
                if (child is GameInventory inventory)
                {
                    WalkInventory(inventory, result, visited);
                }
                else if (child is GameItem childItem)
                {
                    Walk(childItem, result, visited);
                }
            }
        }
        catch { }
    }

    private static void WalkInventory(GameInventory inventory, List<GameItem> result, HashSet<GameItem> visited)
    {
        if (inventory == null) return;
        try
        {
            List<GameItem>? children = inventory.childItems;
            if (children == null) return;
            foreach (GameItem item in children)
            {
                Walk(item, result, visited);
            }
        }
        catch { }
    }

    private static bool IsOwned(GameItem item)
    {
        try { return GeneralHelper.IsItemOwned(item); }
        catch { return false; }
    }

    private static bool IsInMachineSlot(GameItem item)
    {
        try
        {
            GameInventory? parentInventory = item.parentInventory;
            if (parentInventory == null) return false;
            GameItem? parentItem = parentInventory.GetParentItem();
            if (parentItem == null) return false;
            return MachineHelper.GetBatterySlot(parentItem) == parentInventory;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsBattery(GameItem item)
    {
        if (item == null) return false;
        try
        {
            if (MachineHelper.GetBatterySlot(item) != null) return false;
            string? identifier = item.identifier;
            if (string.IsNullOrEmpty(identifier)) return false;
            string lower = identifier!.ToLowerInvariant();
            return lower.StartsWith("energy_credit") || lower.Contains("battery");
        }
        catch
        {
            return false;
        }
    }

    private static bool IsContainer(GameItem item)
    {
        if (item == null) return false;
        try
        {
            if (item.child != null) return true;
        }
        catch { }
        try
        {
            List<GraphNodeStorage>? children = item.children;
            if (children != null && children.Count > 0) return true;
        }
        catch { }
        try
        {
            string? identifier = item.identifier;
            if (string.IsNullOrEmpty(identifier)) return false;
            string lower = identifier!.ToLowerInvariant();
            return lower.Contains("container") || lower.Contains("crate") || lower.Contains("chest")
                || lower.Contains("box") || lower.Contains("bag") || lower.Contains("pouch")
                || lower.Contains("case") || lower.Contains("smuggler") || lower.Contains("backpack");
        }
        catch { }
        return false;
    }
}
