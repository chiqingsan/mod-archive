using System;
using System.Collections.Generic;

namespace SmartInventoryArrange.Core;

public sealed record LayoutResult(bool Complete, IReadOnlyList<Placement> Placements, LayoutScore Score, bool UsedFallback, bool TimedOut, TimeSpan Elapsed, int Holes, int NarrowFragments);
