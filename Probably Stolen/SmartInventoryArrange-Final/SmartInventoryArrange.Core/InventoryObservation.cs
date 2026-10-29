using System.Collections.Generic;

namespace SmartInventoryArrange.Core;

public sealed record InventoryObservation(nint Pointer, string Geometry, IReadOnlyList<SnapshotItem> Items);
