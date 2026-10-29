namespace SmartInventoryArrange.Core;

public sealed record SnapshotItem(nint Pointer, nint ParentPointer, string StableId, bool Movable, string State, SnapshotPose? Pose);
