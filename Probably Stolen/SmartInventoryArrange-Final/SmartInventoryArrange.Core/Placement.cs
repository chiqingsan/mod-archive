namespace SmartInventoryArrange.Core;

public sealed record Placement(string StableId, int X, int Y, int GameOrientation, ShapeMask Mask, string Category, bool Rotated);
