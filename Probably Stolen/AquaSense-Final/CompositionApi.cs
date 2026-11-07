using System;
using System.Collections.Generic;

namespace AquaSense;

// Read-only data surface for other mods. Nothing here touches tooltip
// rendering: the consumer reads the exact rows AquaSense would show.
//
// AppDomain slot "aquasense.composition.v1" holds
//   Func<GameItem, IReadOnlyList<(string Name, string Amount,
//                                 string Percent, float Ratio, int ColorRgb)>>
// and "aquasense.composition.version.v1" holds the int contract version.
//
// Only BCL types cross the slot, so a consumer never references AquaSense.dll:
// it casts the value to a Func with the same generic arguments (tuple element
// names are erased at runtime, so its own names are fine).
internal static class CompositionApi
{
    internal const string DataSlot = "aquasense.composition.v1";

    internal const string VersionSlot = "aquasense.composition.version.v1";

    internal const int Version = 1;

    internal static void Register()
    {
        try
        {
            AppDomain.CurrentDomain.SetData(
                DataSlot,
                (Func<GameItem, IReadOnlyList<(string Name, string Amount, string Percent, float Ratio, int ColorRgb)>?>)
                Hooks.GetCompositionApiRows);
            AppDomain.CurrentDomain.SetData(VersionSlot, Version);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Composition API registration failed: {ex.Message}");
        }
    }

    internal static void Unregister()
    {
        try
        {
            AppDomain.CurrentDomain.SetData(DataSlot, null);
            AppDomain.CurrentDomain.SetData(VersionSlot, null);
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"Composition API unregistration failed: {ex.Message}");
        }
    }
}
