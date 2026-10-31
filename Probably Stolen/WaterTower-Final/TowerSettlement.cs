using System;
using System.Collections.Generic;

namespace WaterTowerFinal;

// Settlement core only. The tower, inventory layout, persistence and registration
// have not yet been ported; this is not a loadable mod.
internal static class TowerSettlement
{
    internal static void Run(GameItem tank, IReadOnlyList<GameItem> machines, IReadOnlyList<GameItem?> filters, bool turbo = false)
    {
        foreach (var machine in machines)
        {
            if (machine.identifier != "moisture_farm" || !CanUse(machine)) continue;
            if (((GameSlotInventory)MachineHelper.GetSlot(machine, 2)).childItem != null) continue;
            if (WaterHelper.GetFreeCapacity(tank) <= 0) continue;
            var battery = MachineHelper.GetBatterySlot(machine).childItem;
            int power = MachineryHelper.GetMachinePowerUsage(machine);
            if (battery == null || !PowerHelper.CanDrawPowerSource(battery, power)) continue;
            if (!PowerHelper.DrawPowerSource(battery, power)) continue;
            int volume = MachineMoistureFarm.GetOutputVolume(machine);
            if (volume <= 0) continue;
            int grade = MachineMoistureFarm.GetOutputQuality(machine) switch
            {
                "Pure Water" => 0, "High Quality Water" => 1, "Basewater" => 2,
                "Ghostwater" => 3, "Rustwater" => 4, "Gutterflow" => 5,
                _ => throw new InvalidOperationException("Unknown collector water quality")
            };
            WaterHelper.AddWater(tank, grade, volume);
            FilterTank(tank, filters);
            MachineHelper.OnMachineActioned(machine, MachineHelper.GetModuleInv(machine));
        }
        foreach (var machine in machines)
        {
            if (machine.identifier != "water_purifier" || !CanUse(machine)) continue;
            if (((GameSlotInventory)MachineHelper.GetSlot(machine, 2)).childItem != null || WaterHelper.IsEmpty(tank)) continue;
            var battery = MachineHelper.GetBatterySlot(machine).childItem;
            int power = MachineryHelper.GetMachinePowerUsage(machine);
            if (battery == null || !PowerHelper.CanDrawPowerSource(battery, power)) continue;
            if (!PowerHelper.DrawPowerSource(battery, power)) continue;
            int removed = MachinePurifier.PurifyContainer(machine, tank);
            if (removed == 0 && !turbo) { PowerHelper.DrawPowerSource(battery, -power); continue; }
            MachineHelper.OnMachineActioned(machine, MachineHelper.GetModuleInv(machine));
        }
    }

    private static bool CanUse(GameItem machine) => GeneralHelper.IsItemOwned(machine) && !machine.IsTag("BROKEN_MACHINE_TAG");

    private static void FilterTank(GameItem tank, IReadOnlyList<GameItem?> filters, bool turbo = false)
    {
        foreach (var liquid in Liquid.Liquids)
        {
            int before = tank.GetTagReadonly(liquid.currentPart).GetInt();
            if (before <= 0) continue;
            int after = before;
            foreach (var filter in filters)
                if (filter != null) after = WaterHelper.HandleFilter(liquid.identifier, after, null, filter);
            if (after != before) tank.ModifyTag(liquid.currentPart, tag => tag.Enable().SetInt(after), skipValidate: true);
        }
        WaterFeatureHelper.OnLiquidChanged(tank, false);
    }
}

