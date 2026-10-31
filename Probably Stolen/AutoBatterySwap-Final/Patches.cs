using HarmonyLib;

namespace AutoBatterySwap;

[HarmonyPatch(typeof(PowerHelper), nameof(PowerHelper.HandlePower))]
internal static class HandlePowerPatch
{
    private static void Prefix() => BatterySwapper.RunAll();
}

[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.HandlePower))]
internal static class PlayerHandlePowerPatch
{
    private static void Prefix() => BatterySwapper.RunAll();
}

[HarmonyPatch(typeof(MachineHelper), nameof(MachineHelper.TryDrawCyclePower))]
internal static class TryDrawCyclePowerPatch
{
    private static void Prefix(GameItem machine, GameSlotInventory batterySlot)
    {
        if (machine != null) BatterySwapper.TryTopUpMachine(machine, batterySlot);
    }
}

[HarmonyPatch(typeof(MachineHelper), nameof(MachineHelper.CanPower), new[] { typeof(GameItem), typeof(GameSlotInventory) })]
internal static class CanPowerPatch
{
    private static void Prefix(GameItem machine, GameSlotInventory batterySlot)
    {
        if (machine != null) BatterySwapper.TryTopUpMachine(machine, batterySlot);
    }
}

[HarmonyPatch(typeof(PowerHelper), nameof(PowerHelper.DrawPowerSource))]
internal static class DrawPowerSourcePatch
{
    private static void Prefix(GameItem gameItem, int amount)
    {
        if (gameItem != null && amount > 0) BatterySwapper.TryTopUpBatteryItem(gameItem, amount);
    }
}

[HarmonyPatch(typeof(PowerHelper), nameof(PowerHelper.CanDrawPowerSource))]
internal static class CanDrawPowerSourcePatch
{
    private static void Prefix(GameItem gameItem, int amount)
    {
        if (gameItem != null && amount > 0) BatterySwapper.TryTopUpBatteryItem(gameItem, amount);
    }
}

[HarmonyPatch(typeof(PowerHelper), nameof(PowerHelper.TryRemoveEnergy))]
internal static class TryRemoveEnergyPatch
{
    private static void Prefix(GameItem powerSourceItem, int desiredEnergy, bool useBandwidth)
    {
        if (powerSourceItem != null && desiredEnergy > 0) BatterySwapper.TryTopUpBatteryItem(powerSourceItem, desiredEnergy);
    }
}
