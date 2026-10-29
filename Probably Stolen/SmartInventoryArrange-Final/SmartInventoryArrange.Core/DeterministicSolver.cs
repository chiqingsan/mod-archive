using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SmartInventoryArrange.Core;

public sealed class DeterministicSolver
{
	private sealed class State
	{
		public bool[,] Occupied = new bool[0, 0];

		public List<Placement> Placements = new List<Placement>();

		public LayoutScore Score;
	}

	private enum PackingHeuristic
	{
		BestShortSideFit,
		BestAreaFit,
		BottomLeft,
		ContactPoint,
		CategoryContact
	}

	private readonly record struct FreeRect(int X, int Y, int Width, int Height)
	{
		public int Right => X + Width;

		public int Bottom => Y + Height;

		public int Area => Width * Height;

		public bool Contains(int x, int y, int width, int height)
		{
			if (x >= X && y >= Y && x + width <= Right)
			{
				return y + height <= Bottom;
			}
			return false;
		}

		public bool Contains(FreeRect other)
		{
			if (other.X >= X && other.Y >= Y && other.Right <= Right)
			{
				return other.Bottom <= Bottom;
			}
			return false;
		}
	}

	private sealed record AtomicBlock(string Key, string Category, IReadOnlyList<ValueTuple<LayoutItem, Orientation, int, int>> Entries, int Width, int Height, int Order);

	private sealed class BlockState
	{
		public bool[,] Occupied = new bool[0, 0];

		public List<Placement> Placements = new List<Placement>();

		public int Bottom;

		public int Raggedness;

		public int CategoryPenalty;

		public string Tie = string.Empty;

		public string VisualTie = string.Empty;
	}

	private Stopwatch? _activeWatch;

	public int BeamWidth { get; init; } = 32;


	public int CandidatesPerState { get; init; } = 24;


	public TimeSpan TimeBudget { get; init; } = TimeSpan.FromMilliseconds(250.0);


	public string Diagnostic { get; private set; } = string.Empty;


	private bool OutOfTime()
	{
		if (_activeWatch != null)
		{
			return _activeWatch.Elapsed >= TimeBudget;
		}
		return false;
	}

	public static double OccupancyRatio(LayoutProblem p)
	{
		int num = 0;
		for (int i = 0; i < p.Height; i++)
		{
			for (int j = 0; j < p.Width; j++)
			{
				if (p.Usable[j, i])
				{
					num++;
				}
			}
		}
		if (num == 0)
		{
			return 1.0;
		}
		return Math.Clamp((double)(Enumerable.Sum(p.Fixed, (Placement x) => x.Mask.Area) + Enumerable.Sum(p.Items, (LayoutItem x) => x.Area)) / (double)num, 0.0, 1.0);
	}

	public static string DensityMode(LayoutProblem p)
	{
		double num = OccupancyRatio(p);
		if (!(num >= 0.92))
		{
			if (!(num >= 0.8))
			{
				return "aesthetic";
			}
			return "balanced";
		}
		return "dense";
	}

	private static ValueTuple<string, int, int>[] ComparisonFields(LayoutProblem p, LayoutScore a, LayoutScore b)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		string text = DensityMode(p);
		if (!(text == "dense"))
		{
			if (!(text == "balanced"))
			{
				return new ValueTuple<string, int, int>[20]
				{
					new ValueTuple<string, int, int>("placedCount", a.PlacedCount, b.PlacedCount),
					new ValueTuple<string, int, int>("placedArea", a.PlacedArea, b.PlacedArea),
					new ValueTuple<string, int, int>("typeRegions", a.NegativeTypeRuns, b.NegativeTypeRuns),
					new ValueTuple<string, int, int>("typeRows", a.SameTypeRowAlignment, b.SameTypeRowAlignment),
					new ValueTuple<string, int, int>("typeEdges", a.SameTypeHorizontalContact, b.SameTypeHorizontalContact),
					new ValueTuple<string, int, int>("cohortRegions", a.NegativeCohortRuns, b.NegativeCohortRuns),
					new ValueTuple<string, int, int>("cohortRows", a.CohortRowAlignment, b.CohortRowAlignment),
					new ValueTuple<string, int, int>("cohortEdges", a.CohortHorizontalContact, b.CohortHorizontalContact),
					new ValueTuple<string, int, int>("orientationConsistency", a.OrientationConsistency, b.OrientationConsistency),
					new ValueTuple<string, int, int>("verticalPreference", a.VerticalPreference, b.VerticalPreference),
					new ValueTuple<string, int, int>("priorityOrder", a.PriorityOrderAlignment, b.PriorityOrderAlignment),
					new ValueTuple<string, int, int>("categoryOrder", a.CategoryOrderAlignment, b.CategoryOrderAlignment),
					new ValueTuple<string, int, int>("largeFirst", a.LargeFirstAlignment, b.LargeFirstAlignment),
					new ValueTuple<string, int, int>("typeContact", a.SameTypeContact, b.SameTypeContact),
					new ValueTuple<string, int, int>("categoryEdges", a.SameCategoryContact, b.SameCategoryContact),
					new ValueTuple<string, int, int>("topLeft", a.TopLeftCompactness, b.TopLeftCompactness),
					new ValueTuple<string, int, int>("edgeRegularity", a.RowRegularity, b.RowRegularity),
					new ValueTuple<string, int, int>("fragments", a.NegativeFragments, b.NegativeFragments),
					new ValueTuple<string, int, int>("categoryInterleaving", a.NegativeInterleaving, b.NegativeInterleaving),
					new ValueTuple<string, int, int>("rotations", a.NegativeRotations, b.NegativeRotations)
				};
			}
			return new ValueTuple<string, int, int>[17]
			{
				new ValueTuple<string, int, int>("placedCount", a.PlacedCount, b.PlacedCount),
				new ValueTuple<string, int, int>("placedArea", a.PlacedArea, b.PlacedArea),
				new ValueTuple<string, int, int>("typeRegions", a.NegativeTypeRuns, b.NegativeTypeRuns),
				new ValueTuple<string, int, int>("typeRows", a.SameTypeRowAlignment, b.SameTypeRowAlignment),
				new ValueTuple<string, int, int>("typeEdges", a.SameTypeHorizontalContact, b.SameTypeHorizontalContact),
				new ValueTuple<string, int, int>("cohortRegions", a.NegativeCohortRuns, b.NegativeCohortRuns),
				new ValueTuple<string, int, int>("fragments", a.NegativeFragments, b.NegativeFragments),
				new ValueTuple<string, int, int>("edgeRegularity", a.RowRegularity, b.RowRegularity),
				new ValueTuple<string, int, int>("categoryEdges", a.SameCategoryContact, b.SameCategoryContact),
				new ValueTuple<string, int, int>("categoryInterleaving", a.NegativeInterleaving, b.NegativeInterleaving),
				new ValueTuple<string, int, int>("priorityOrder", a.PriorityOrderAlignment, b.PriorityOrderAlignment),
				new ValueTuple<string, int, int>("categoryOrder", a.CategoryOrderAlignment, b.CategoryOrderAlignment),
				new ValueTuple<string, int, int>("largeFirst", a.LargeFirstAlignment, b.LargeFirstAlignment),
				new ValueTuple<string, int, int>("orientationConsistency", a.OrientationConsistency, b.OrientationConsistency),
				new ValueTuple<string, int, int>("verticalPreference", a.VerticalPreference, b.VerticalPreference),
				new ValueTuple<string, int, int>("rotations", a.NegativeRotations, b.NegativeRotations),
				new ValueTuple<string, int, int>("topLeft", a.TopLeftCompactness, b.TopLeftCompactness)
			};
		}
		return new ValueTuple<string, int, int>[18]
		{
			new ValueTuple<string, int, int>("placedCount", a.PlacedCount, b.PlacedCount),
			new ValueTuple<string, int, int>("placedArea", a.PlacedArea, b.PlacedArea),
			new ValueTuple<string, int, int>("typeRegions", a.NegativeTypeRuns, b.NegativeTypeRuns),
			new ValueTuple<string, int, int>("typeRows", a.SameTypeRowAlignment, b.SameTypeRowAlignment),
			new ValueTuple<string, int, int>("typeEdges", a.SameTypeHorizontalContact, b.SameTypeHorizontalContact),
			new ValueTuple<string, int, int>("cohortRegions", a.NegativeCohortRuns, b.NegativeCohortRuns),
			new ValueTuple<string, int, int>("cohortRows", a.CohortRowAlignment, b.CohortRowAlignment),
			new ValueTuple<string, int, int>("categoryEdges", a.SameCategoryContact, b.SameCategoryContact),
			new ValueTuple<string, int, int>("categoryInterleaving", a.NegativeInterleaving, b.NegativeInterleaving),
			new ValueTuple<string, int, int>("priorityOrder", a.PriorityOrderAlignment, b.PriorityOrderAlignment),
			new ValueTuple<string, int, int>("categoryOrder", a.CategoryOrderAlignment, b.CategoryOrderAlignment),
			new ValueTuple<string, int, int>("largeFirst", a.LargeFirstAlignment, b.LargeFirstAlignment),
			new ValueTuple<string, int, int>("fragments", a.NegativeFragments, b.NegativeFragments),
			new ValueTuple<string, int, int>("edgeRegularity", a.RowRegularity, b.RowRegularity),
			new ValueTuple<string, int, int>("orientationConsistency", a.OrientationConsistency, b.OrientationConsistency),
			new ValueTuple<string, int, int>("verticalPreference", a.VerticalPreference, b.VerticalPreference),
			new ValueTuple<string, int, int>("rotations", a.NegativeRotations, b.NegativeRotations),
			new ValueTuple<string, int, int>("topLeft", a.TopLeftCompactness, b.TopLeftCompactness)
		};
	}

	public static int CompareForProblem(LayoutProblem p, LayoutScore a, LayoutScore b)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ValueTuple<string, int, int>[] array = ComparisonFields(p, a, b);
		for (int i = 0; i < array.Length; i++)
		{
			ValueTuple<string, int, int> val = array[i];
			int num = val.Item2.CompareTo(val.Item3);
			if (num != 0)
			{
				return num;
			}
		}
		return -StringComparer.Ordinal.Compare(a.StableTie, b.StableTie);
	}

	public static string FirstDifferingMetric(LayoutProblem p, LayoutScore candidate, LayoutScore baseline)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		ValueTuple<string, int, int>[] array = ComparisonFields(p, candidate, baseline);
		DefaultInterpolatedStringHandler val2 = default(DefaultInterpolatedStringHandler);
		for (int i = 0; i < array.Length; i++)
		{
			ValueTuple<string, int, int> val = array[i];
			if (val.Item2 != val.Item3)
			{
				val2 = new DefaultInterpolatedStringHandler(21, 3);
				val2.AppendFormatted(val.Item1);
				val2.AppendLiteral(":candidate=");
				val2.AppendFormatted<int>(val.Item2);
				val2.AppendLiteral(",baseline=");
				val2.AppendFormatted<int>(val.Item3);
				return val2.ToStringAndClear();
			}
		}
		if (!StringComparer.Ordinal.Equals(candidate.StableTie, baseline.StableTie))
		{
			return "stableTie";
		}
		return "equal";
	}

	public LayoutResult Solve(LayoutProblem p, IReadOnlyList<Placement>? original = null)
	{
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
		LayoutProblem p2 = p;
		ValidateProblem(p2);
		string text = DensityMode(p2);
		double num = OccupancyRatio(p2);
		Stopwatch val = (_activeWatch = Stopwatch.StartNew());
		bool[,] baseOcc = new bool[p2.Width, p2.Height];
		for (int j = 0; j < p2.Height; j++)
		{
			for (int k = 0; k < p2.Width; k++)
			{
				baseOcc[k, j] = !p2.Usable[k, j];
			}
		}
		foreach (Placement item4 in p2.Fixed)
		{
			Mark(baseOcc, item4, value: true);
		}
		Dictionary<string, int> categoryArea = Enumerable.ToDictionary(p2.Items.GroupBy((LayoutItem i) => i.PrimaryCategory, (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, LayoutItem> g) => g.Key, (IGrouping<string, LayoutItem> g) => Enumerable.Sum(g, (LayoutItem i) => i.Area), (IEqualityComparer<string>?)StringComparer.Ordinal);
		LayoutItem[] array = Enumerable.ToArray(Enumerable.OrderBy(p2.Items, (LayoutItem i) => EstimateLegalPositions(p2, baseOcc, i)).ThenByDescending((LayoutItem i) => i.Complexity).ThenByDescending((LayoutItem i) => i.Area)
			.ThenByDescending((LayoutItem i) => Enumerable.Max(i.Orientations, (Orientation o) => Math.Max(o.Mask.Width, o.Mask.Height)))
			.ThenBy((LayoutItem i) => i.Orientations.Count)
			.ThenBy((LayoutItem i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal));
		LayoutItem[] array2 = Enumerable.ToArray(VisualOrder(p2, p2.Items));
		string diagnostic;
		string diagnostic2;
		Placement[][] obj = new Placement[2][]
		{
			AtomicSkylineBlocks(p2, baseOcc, array2, portraitOnly: true, out diagnostic),
			AtomicSkylineBlocks(p2, baseOcc, array2, portraitOnly: false, out diagnostic2)
		};
		Diagnostic = "atomic skyline: portrait=" + diagnostic + "; any=" + diagnostic2;
		State state = null;
		Placement[][] array3 = obj;
		foreach (Placement[] array4 in array3)
		{
			if (array4 != null && array4.Length == p2.Items.Count)
			{
				bool[,] array5 = BuildOccupied(p2, array4);
				LayoutScore layoutScore = Score(p2, Enumerable.ToArray(p2.Fixed.Concat(array4)), array5);
				State state2 = new State
				{
					Occupied = array5,
					Placements = Enumerable.ToList(p2.Fixed.Concat(array4)),
					Score = layoutScore
				};
				if (state == null || CompareForProblem(p2, layoutScore, state.Score) > 0)
				{
					state = state2;
				}
			}
		}
		DefaultInterpolatedStringHandler val3 = default(DefaultInterpolatedStringHandler);
		if (text == "aesthetic" && state != null && state.Score.NegativeTypeRuns == 0)
		{
			Placement[] placements = Enumerable.ToArray(Enumerable.Where((IEnumerable<Placement>)state.Placements, (Placement x) => Enumerable.All(p2.Fixed, (Placement f) => f.StableId != x.StableId)).OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
			val.Stop();
			ValueTuple<int, int> val2 = FragmentMetrics(p2, state.Occupied);
			val3 = new DefaultInterpolatedStringHandler(65, 2);
			val3.AppendLiteral("mode=");
			val3.AppendFormatted(text);
			val3.AppendLiteral(", occupancy=");
			val3.AppendFormatted<double>(num, "P1");
			val3.AppendLiteral(", complete atomic mixed-shape skyline-block seed");
			Diagnostic = val3.ToStringAndClear();
			return new LayoutResult(Complete: true, placements, state.Score, UsedFallback: false, val.Elapsed >= TimeBudget, val.Elapsed, val2.Item1, val2.Item2);
		}
		List<Placement> placements2 = new List<Placement>((IEnumerable<Placement>)p2.Fixed);
		List<State> list = new List<State>
		{
			new State
			{
				Occupied = baseOcc,
				Placements = placements2,
				Score = Score(p2, (IReadOnlyList<Placement>)placements2, baseOcc, includeStableTie: false)
			}
		};
		bool flag = false;
		LayoutItem[] array6 = array2;
		foreach (LayoutItem item in array6)
		{
			if (OutOfTime())
			{
				flag = true;
				break;
			}
			List<State> list2 = new List<State>();
			foreach (State item5 in list)
			{
				if (OutOfTime())
				{
					flag = true;
					break;
				}
				Placement[] array7 = Enumerable.ToArray(Enumerable.Take(EnumerateCandidates(p2, item5, item, categoryArea), CandidatesPerState));
				foreach (Placement placement in array7)
				{
					if (OutOfTime())
					{
						flag = true;
						break;
					}
					bool[,] array8 = (bool[,])item5.Occupied.Clone();
					Mark(array8, placement, value: true);
					List<Placement> placements3 = new List<Placement>((IEnumerable<Placement>)item5.Placements) { placement };
					list2.Add(new State
					{
						Occupied = array8,
						Placements = placements3,
						Score = Score(p2, (IReadOnlyList<Placement>)placements3, array8, includeStableTie: false)
					});
				}
				if (flag)
				{
					break;
				}
			}
			if (flag || list2.Count == 0)
			{
				break;
			}
			list = Enumerable.ToList(Enumerable.Take(Enumerable.OrderByDescending((IEnumerable<State>)list2, (State s) => s.Score).ThenBy((State s) => PlacementKey((IEnumerable<Placement>)s.Placements), (IComparer<string>?)StringComparer.Ordinal), BeamWidth));
		}
		State state3 = Enumerable.First(Enumerable.OrderByDescending((IEnumerable<State>)list, (State s) => s.Score).ThenBy((State s) => PlacementKey((IEnumerable<Placement>)s.Placements), (IComparer<string>?)StringComparer.Ordinal));
		state3.Score = Score(p2, (IReadOnlyList<Placement>)state3.Placements, state3.Occupied);
		Placement[] array9 = Enumerable.ToArray(Enumerable.Where((IEnumerable<Placement>)state3.Placements, (Placement x) => Enumerable.All(p2.Fixed, (Placement f) => f.StableId != x.StableId)));
		bool flag2 = array9.Length == p2.Items.Count;
		LayoutItem[] array10 = array2;
		LayoutItem[] array11 = Enumerable.ToArray(Enumerable.OrderByDescending(p2.Items, (LayoutItem i) => i.Area).ThenByDescending((LayoutItem i) => i.Complexity).ThenBy((LayoutItem i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal));
		List<Placement[]> list3 = new List<Placement[]>
		{
			(text == "dense" && original != null) ? DenseSlotReassignment(p2, original) : null,
			(state != null) ? Enumerable.ToArray(Enumerable.Where((IEnumerable<Placement>)state.Placements, (Placement x) => Enumerable.All(p2.Fixed, (Placement f) => f.StableId != x.StableId)).OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal)) : null,
			GreedyShelfGroups(p2, baseOcc, array10, allowHorizontal: false),
			GreedyShelfGroups(p2, baseOcc, array10, allowHorizontal: true),
			GreedyFirstFit(p2, baseOcc, array10, reverseX: false),
			GreedyFirstFit(p2, baseOcc, array10, reverseX: true)
		};
		if (!OutOfTime())
		{
			LayoutItem[][] array12 = new LayoutItem[3][] { array10, array, array11 };
			foreach (LayoutItem[] items in array12)
			{
				PackingHeuristic[] values = (PackingHeuristic[])Enum.GetValues(typeof(PackingHeuristic));
				foreach (PackingHeuristic heuristic in values)
				{
					if (OutOfTime())
					{
						flag = true;
						break;
					}
					list3.Add(GreedyMaxRects(p2, baseOcc, items, heuristic, text == "dense"));
				}
			}
		}
		foreach (Placement[] item6 in list3)
		{
			if (item6 != null)
			{
				bool[,] array13 = BuildOccupied(p2, item6);
				LayoutScore layoutScore2 = Score(p2, Enumerable.ToArray(p2.Fixed.Concat(item6)), array13);
				if (!flag2 || CompareForProblem(p2, layoutScore2, state3.Score) > 0)
				{
					array9 = item6;
					flag2 = true;
					state3 = new State
					{
						Occupied = array13,
						Placements = Enumerable.ToList(p2.Fixed.Concat(item6)),
						Score = layoutScore2
					};
				}
			}
		}
		if (!OutOfTime() && original != null && original.Count == p2.Items.Count)
		{
			Placement[] array14 = ImproveCompleteLayout(p2, original, val);
			if (array14 != null)
			{
				LayoutScore layoutScore3 = Evaluate(p2, array14);
				if (!flag2 || CompareForProblem(p2, layoutScore3, state3.Score) > 0)
				{
					array9 = array14;
					flag2 = true;
					bool[,] occupied = BuildOccupied(p2, array14);
					state3 = new State
					{
						Occupied = occupied,
						Placements = Enumerable.ToList(p2.Fixed.Concat(array14)),
						Score = layoutScore3
					};
				}
			}
		}
		if (!flag2)
		{
			Placement[] array15 = OriginalLayout(p2);
			if (array15 != null)
			{
				bool[,] array16 = (bool[,])baseOcc.Clone();
				Placement[] array7 = array15;
				foreach (Placement p3 in array7)
				{
					Mark(array16, p3, value: true);
				}
				Placement[] placements4 = Enumerable.ToArray(p2.Fixed.Concat(array15));
				val.Stop();
				ValueTuple<int, int> val4 = FragmentMetrics(p2, array16);
				int item2 = val4.Item1;
				int item3 = val4.Item2;
				return new LayoutResult(Complete: true, array15, Score(p2, placements4, array16), UsedFallback: true, flag, val.Elapsed, item2, item3);
			}
		}
		val.Stop();
		ValueTuple<int, int> val5 = FragmentMetrics(p2, state3.Occupied);
		val3 = new DefaultInterpolatedStringHandler(19, 3);
		val3.AppendLiteral("mode=");
		val3.AppendFormatted(text);
		val3.AppendLiteral(", occupancy=");
		val3.AppendFormatted<double>(num, "P1");
		val3.AppendLiteral("; ");
		val3.AppendFormatted(Diagnostic);
		Diagnostic = val3.ToStringAndClear();
		return new LayoutResult(flag2, array9, state3.Score, UsedFallback: false, flag, val.Elapsed, val5.Item1, val5.Item2);
	}

	private static Placement[]? AtomicSkylineBlocks(LayoutProblem p, bool[,] baseOcc, IReadOnlyList<LayoutItem> items, bool portraitOnly, out string diagnostic)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		diagnostic = "building";
		List<AtomicBlock> list = new List<AtomicBlock>();
		int num = 0;
		DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
		foreach (IGrouping<string, LayoutItem> item in items.GroupBy((LayoutItem i) => ItemTypeKey(i.StableId), (IEqualityComparer<string>?)StringComparer.Ordinal))
		{
			LayoutItem[] array = Enumerable.ToArray(item.OrderBy((LayoutItem i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal));
			List<ValueTuple<LayoutItem, Orientation, int, int>> list2 = new List<ValueTuple<LayoutItem, Orientation, int, int>>();
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			LayoutItem[] array2 = array;
			foreach (LayoutItem layoutItem in array2)
			{
				Orientation orientation = Enumerable.FirstOrDefault(PreferredOrientations(layoutItem, portraitOnly));
				if (orientation == null)
				{
					val = new DefaultInterpolatedStringHandler(21, 3);
					val.AppendLiteral("no ");
					val.AppendFormatted(portraitOnly ? "portrait" : "legal");
					val.AppendLiteral(" orientation for ");
					val.AppendFormatted(item.Key);
					val.AppendLiteral("/");
					val.AppendFormatted(layoutItem.StableId);
					diagnostic = val.ToStringAndClear();
					return null;
				}
				if (orientation.Mask.Width > p.Width)
				{
					diagnostic = "item wider than container: " + item.Key + "/" + layoutItem.StableId;
					return null;
				}
				if (num2 > 0 && num2 + orientation.Mask.Width > p.Width)
				{
					num3 += num4;
					num2 = 0;
					num4 = 0;
				}
				if (num3 + orientation.Mask.Height > p.Height)
				{
					diagnostic = "wrapped family taller than container: " + item.Key;
					return null;
				}
				list2.Add(new ValueTuple<LayoutItem, Orientation, int, int>(layoutItem, orientation, num2, num3));
				num2 += orientation.Mask.Width;
				num5 = Math.Max(num5, num2);
				num4 = Math.Max(num4, orientation.Mask.Height);
			}
			int height = num3 + num4;
			list.Add(new AtomicBlock(item.Key, array[0].PrimaryCategory, (IReadOnlyList<ValueTuple<LayoutItem, Orientation, int, int>>)list2, num5, height, num++));
		}
		val = new DefaultInterpolatedStringHandler(27, 1);
		val.AppendLiteral("built ");
		val.AppendFormatted<int>(list.Count);
		val.AppendLiteral(" mixed-shape families");
		diagnostic = val.ToStringAndClear();
		List<BlockState> list3 = new List<BlockState>
		{
			new BlockState
			{
				Occupied = (bool[,])baseOcc.Clone()
			}
		};
		foreach (AtomicBlock item2 in Enumerable.OrderBy((IEnumerable<AtomicBlock>)list, (AtomicBlock b) => b.Order))
		{
			List<BlockState> list4 = new List<BlockState>();
			foreach (BlockState item3 in list3)
			{
				List<ValueTuple<int, int, int, int, int, int, string>> list5 = new List<ValueTuple<int, int, int, int, int, int, string>>();
				for (int k = 0; k <= p.Width - item2.Width; k++)
				{
					for (int l = 0; l <= p.Height - item2.Height; l++)
					{
						bool flag = true;
						foreach (ValueTuple<LayoutItem, Orientation, int, int> entry in item2.Entries)
						{
							if (!Fits(p, item3.Occupied, entry.Item2.Mask, k + entry.Item3, l + entry.Item4))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							int num6 = l + item2.Height;
							int num7 = SkylineHoles(item3.Occupied, k, l, item2.Width);
							int num8 = CategoryBlockDistance((IReadOnlyList<Placement>)item3.Placements, item2.Category, k, l, item2.Width, item2.Height);
							int num9 = ((k == 0) ? 1 : 0);
							int num10 = k;
							int num11 = l;
							val = new DefaultInterpolatedStringHandler(1, 2);
							val.AppendFormatted<int>(l, "D3");
							val.AppendLiteral(",");
							val.AppendFormatted<int>(k, "D3");
							list5.Add(new ValueTuple<int, int, int, int, int, int, string>(num10, num11, num6, num7, num8, num9, val.ToStringAndClear()));
							break;
						}
					}
				}
				foreach (ValueTuple<int, int, int, int, int, int, string> item4 in Enumerable.Take(Enumerable.OrderBy((IEnumerable<ValueTuple<int, int, int, int, int, int, string>>)list5, (ValueTuple<int, int, int, int, int, int, string> v) => v.Item5).ThenBy((ValueTuple<int, int, int, int, int, int, string> v) => v.Item3).ThenBy((ValueTuple<int, int, int, int, int, int, string> v) => v.Item4)
					.ThenByDescending((ValueTuple<int, int, int, int, int, int, string> v) => v.Item6)
					.ThenBy((ValueTuple<int, int, int, int, int, int, string> v) => v.Item2)
					.ThenBy((ValueTuple<int, int, int, int, int, int, string> v) => v.Item1)
					.ThenBy((ValueTuple<int, int, int, int, int, int, string> v) => v.Item7, (IComparer<string>?)StringComparer.Ordinal), 18))
				{
					bool[,] occupied = (bool[,])item3.Occupied.Clone();
					List<Placement> list6 = new List<Placement>((IEnumerable<Placement>)item3.Placements);
					foreach (ValueTuple<LayoutItem, Orientation, int, int> entry2 in item2.Entries)
					{
						Placement placement = new Placement(entry2.Item1.StableId, item4.Item1 + entry2.Item3, item4.Item2 + entry2.Item4, entry2.Item2.GameOrientation, entry2.Item2.Mask, entry2.Item1.PrimaryCategory, entry2.Item2.Rotated);
						Mark(occupied, placement, value: true);
						list6.Add(placement);
					}
					BlockState obj = new BlockState
					{
						Occupied = occupied,
						Placements = list6,
						Bottom = Math.Max(item3.Bottom, item4.Item3),
						Raggedness = 0,
						CategoryPenalty = item3.CategoryPenalty + item4.Item5,
						Tie = string.Empty
					};
					string visualTie = item3.VisualTie;
					val = new DefaultInterpolatedStringHandler(2, 2);
					val.AppendFormatted<int>(item4.Item2, "D3");
					val.AppendLiteral(",");
					val.AppendFormatted<int>(item4.Item1, "D3");
					val.AppendLiteral(";");
					obj.VisualTie = visualTie + val.ToStringAndClear();
					list4.Add(obj);
				}
			}
			if (list4.Count == 0)
			{
				diagnostic = "no skyline position for " + item2.Key;
				return null;
			}
			list3 = Enumerable.ToList(Enumerable.Take(Enumerable.OrderBy((IEnumerable<BlockState>)list4, (BlockState s) => s.CategoryPenalty).ThenBy((BlockState s) => s.Bottom).ThenBy((BlockState s) => s.VisualTie, (IComparer<string>?)StringComparer.Ordinal), 24));
		}
		return Enumerable.ToArray(((IEnumerable<Placement>)Enumerable.First(Enumerable.OrderBy((IEnumerable<BlockState>)list3, (BlockState s) => s.CategoryPenalty).ThenBy((BlockState s) => s.Bottom).ThenBy((BlockState s) => s.VisualTie, (IComparer<string>?)StringComparer.Ordinal)).Placements).OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
	}

	private static int CategoryBlockDistance(IReadOnlyList<Placement> placed, string category, int x, int y, int width, int height)
	{
		int num = int.MaxValue;
		bool flag = false;
		int num2 = x + width;
		int num3 = y + height;
		foreach (Placement item in placed)
		{
			if (StringComparer.Ordinal.Equals(item.Category, category))
			{
				flag = true;
				int num4 = item.X + item.Mask.Width;
				int num5 = item.Y + item.Mask.Height;
				int num6 = Math.Max(0, Math.Max(item.X - num2, x - num4));
				int num7 = Math.Max(0, Math.Max(item.Y - num3, y - num5));
				num = Math.Min(num, num6 + num7);
			}
		}
		if (!flag)
		{
			return 0;
		}
		return num;
	}

	private static int SkylineHoles(bool[,] occupied, int x, int y, int width)
	{
		int num = 0;
		for (int i = x; i < x + width; i++)
		{
			for (int j = 0; j < y; j++)
			{
				if (!occupied[i, j])
				{
					num++;
				}
			}
		}
		return num;
	}

	private static Placement[]? GreedyShelfGroups(LayoutProblem p, bool[,] baseOcc, IReadOnlyList<LayoutItem> items, bool allowHorizontal)
	{
		bool[,] occupied = (bool[,])baseOcc.Clone();
		List<Placement> list = new List<Placement>();
		foreach (IGrouping<string, LayoutItem> item2 in items.GroupBy(ShelfCohortKey, (IEqualityComparer<string>?)StringComparer.Ordinal))
		{
			LayoutItem[] array = Enumerable.ToArray(item2.OrderBy((LayoutItem i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal));
			Placement[] array2 = FindWholeRowPlan(p, occupied, array, portraitOnly: true);
			if (array2 == null && allowHorizontal)
			{
				array2 = FindWholeRowPlan(p, occupied, array, portraitOnly: false);
			}
			if (array2 != null)
			{
				Placement[] array3 = array2;
				foreach (Placement placement in array3)
				{
					Mark(occupied, placement, value: true);
					list.Add(placement);
				}
				continue;
			}
			LayoutItem[] array4 = array;
			foreach (LayoutItem item in array4)
			{
				Placement placement2 = BestShelfPlacement(p, occupied, (IReadOnlyList<Placement>)list, item, portraitOnly: true);
				if (placement2 == null && allowHorizontal)
				{
					placement2 = BestShelfPlacement(p, occupied, (IReadOnlyList<Placement>)list, item, portraitOnly: false);
				}
				if (placement2 == null)
				{
					return null;
				}
				Mark(occupied, placement2, value: true);
				list.Add(placement2);
			}
		}
		return Enumerable.ToArray(((IEnumerable<Placement>)list).OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
	}

	private static Placement[]? FindWholeRowPlan(LayoutProblem p, bool[,] occupied, IReadOnlyList<LayoutItem> items, bool portraitOnly)
	{
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		if (items.Count == 0)
		{
			return Array.Empty<Placement>();
		}
		List<ValueTuple<Placement[], int, int, int, int>> list = new List<ValueTuple<Placement[], int, int, int, int>>();
		foreach (Orientation leader in PreferredOrientations(items[0], portraitOnly))
		{
			Orientation[] array = new Orientation[items.Count];
			array[0] = leader;
			bool flag = true;
			for (int i = 1; i < items.Count; i++)
			{
				array[i] = Enumerable.FirstOrDefault(PreferredOrientations(items[i], portraitOnly), (Orientation o) => o.Mask.Signature == leader.Mask.Signature) ?? Enumerable.FirstOrDefault(PreferredOrientations(items[i], portraitOnly), (Orientation o) => o.Mask.Width == leader.Mask.Width && o.Mask.Height == leader.Mask.Height);
				if (array[i] == null)
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			int num = Enumerable.Sum(array, (Orientation o) => o.Mask.Width);
			if (num > p.Width)
			{
				continue;
			}
			int num2 = Enumerable.Max(array, (Orientation o) => o.Mask.Height);
			for (int j = 0; j <= p.Height - num2; j++)
			{
				for (int k = 0; k <= p.Width - num; k++)
				{
					bool[,] occupied2 = (bool[,])occupied.Clone();
					Placement[] array2 = new Placement[items.Count];
					int num3 = k;
					bool flag2 = true;
					for (int l = 0; l < items.Count; l++)
					{
						Orientation orientation = array[l];
						if (!Fits(p, occupied2, orientation.Mask, num3, j))
						{
							flag2 = false;
							break;
						}
						array2[l] = new Placement(items[l].StableId, num3, j, orientation.GameOrientation, orientation.Mask, items[l].PrimaryCategory, orientation.Rotated);
						Mark(occupied2, array2[l], value: true);
						num3 += orientation.Mask.Width;
					}
					if (flag2)
					{
						int num4 = ((leader.Mask.Height <= leader.Mask.Width) ? ((leader.Mask.Height == leader.Mask.Width) ? 1 : 2) : 0);
						list.Add(new ValueTuple<Placement[], int, int, int, int>(array2, num4, j, k, Enumerable.Count(array2, (Placement q) => q.Rotated)));
					}
				}
			}
		}
		return Enumerable.FirstOrDefault(Enumerable.Select(Enumerable.OrderBy((IEnumerable<ValueTuple<Placement[], int, int, int, int>>)list, (ValueTuple<Placement[], int, int, int, int> v) => v.Item2).ThenBy((ValueTuple<Placement[], int, int, int, int> v) => v.Item3).ThenBy((ValueTuple<Placement[], int, int, int, int> v) => v.Item4)
			.ThenBy((ValueTuple<Placement[], int, int, int, int> v) => v.Item5)
			.ThenBy((ValueTuple<Placement[], int, int, int, int> v) => PlacementKey(v.Item1), (IComparer<string>?)StringComparer.Ordinal), (ValueTuple<Placement[], int, int, int, int> v) => v.Item1));
	}

	private static Placement? BestShelfPlacement(LayoutProblem p, bool[,] occupied, IReadOnlyList<Placement> placed, LayoutItem item, bool portraitOnly)
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		List<ValueTuple<Placement, int, int, int, int>> list = new List<ValueTuple<Placement, int, int, int, int>>();
		foreach (Orientation item2 in PreferredOrientations(item, portraitOnly))
		{
			for (int i = 0; i <= p.Height - item2.Mask.Height; i++)
			{
				for (int j = 0; j <= p.Width - item2.Mask.Width; j++)
				{
					if (Fits(p, occupied, item2.Mask, j, i))
					{
						Placement placement = new Placement(item.StableId, j, i, item2.GameOrientation, item2.Mask, item.PrimaryCategory, item2.Rotated);
						int num = ((item2.Mask.Height <= item2.Mask.Width) ? ((item2.Mask.Height == item2.Mask.Width) ? 1 : 2) : 0);
						list.Add(new ValueTuple<Placement, int, int, int, int>(placement, num, SameTypeRowMates(placed, placement), HorizontalTypeContact(placed, placement), TypeContact(placed, placement)));
					}
				}
			}
		}
		return Enumerable.FirstOrDefault(Enumerable.Select(Enumerable.OrderBy((IEnumerable<ValueTuple<Placement, int, int, int, int>>)list, (ValueTuple<Placement, int, int, int, int> v) => v.Item2).ThenByDescending((ValueTuple<Placement, int, int, int, int> v) => v.Item3).ThenByDescending((ValueTuple<Placement, int, int, int, int> v) => v.Item4)
			.ThenByDescending((ValueTuple<Placement, int, int, int, int> v) => v.Item5)
			.ThenBy((ValueTuple<Placement, int, int, int, int> v) => v.Item1.Y)
			.ThenBy((ValueTuple<Placement, int, int, int, int> v) => v.Item1.X)
			.ThenBy((ValueTuple<Placement, int, int, int, int> v) => v.Item1.Rotated)
			.ThenBy((ValueTuple<Placement, int, int, int, int> v) => v.Item1.GameOrientation), (ValueTuple<Placement, int, int, int, int> v) => v.Item1));
	}

	private static IEnumerable<Orientation> PreferredOrientations(LayoutItem item, bool portraitOnly)
	{
		IOrderedEnumerable<Orientation> orderedEnumerable = Enumerable.OrderByDescending(item.Orientations, (Orientation o) => o.Mask.Height > o.Mask.Width).ThenByDescending((Orientation o) => o.Mask.Height - o.Mask.Width).ThenBy((Orientation o) => o.Rotated)
			.ThenBy((Orientation o) => o.GameOrientation);
		if (!portraitOnly)
		{
			return orderedEnumerable;
		}
		return Enumerable.Where(orderedEnumerable, (Orientation o) => o.Mask.Height >= o.Mask.Width);
	}

	private static Placement[]? GreedyMaxRects(LayoutProblem p, bool[,] baseOcc, IReadOnlyList<LayoutItem> items, PackingHeuristic heuristic, bool dense)
	{
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		bool[,] array = (bool[,])baseOcc.Clone();
		List<FreeRect> list = BuildFreeRectangles(p, array);
		List<Placement> list2 = new List<Placement>();
		foreach (LayoutItem item2 in items)
		{
			List<ValueTuple<Placement, int, int, int, int, int>> list3 = new List<ValueTuple<Placement, int, int, int, int, int>>();
			foreach (Orientation o in Enumerable.OrderBy(item2.Orientations, (Orientation x) => x.GameOrientation))
			{
				for (int y = 0; y <= p.Height - o.Mask.Height; y++)
				{
					int x2;
					for (x2 = 0; x2 <= p.Width - o.Mask.Width; x2++)
					{
						if (Fits(p, array, o.Mask, x2, y))
						{
							Placement placement = new Placement(item2.StableId, x2, y, o.GameOrientation, o.Mask, item2.PrimaryCategory, o.Rotated);
							FreeRect[] array2 = Enumerable.ToArray(Enumerable.Where((IEnumerable<FreeRect>)list, (FreeRect r) => r.Contains(x2, y, o.Mask.Width, o.Mask.Height)));
							int num = ((array2.Length == 0) ? 536870911 : Enumerable.Min(array2, (FreeRect r) => r.Area - o.Mask.Width * o.Mask.Height));
							int num2 = ((array2.Length == 0) ? 536870911 : Enumerable.Min(array2, (FreeRect r) => Math.Min(r.Width - o.Mask.Width, r.Height - o.Mask.Height)));
							int num3 = ((array2.Length == 0) ? 536870911 : Enumerable.Min(array2, (FreeRect r) => Math.Max(r.Width - o.Mask.Width, r.Height - o.Mask.Height)));
							int num4 = OccupiedContact(p, array, placement);
							int num5 = CategoryContact((IReadOnlyList<Placement>)list2, placement);
							int num6 = ((o.Mask.Height <= o.Mask.Width) ? ((o.Mask.Height == o.Mask.Width) ? 1 : 2) : 0);
							ValueTuple<int, int, int, int> val = (ValueTuple<int, int, int, int>)(heuristic switch
							{
								PackingHeuristic.BestShortSideFit => new ValueTuple<int, int, int, int>(num2, num3, y, x2), 
								PackingHeuristic.BestAreaFit => new ValueTuple<int, int, int, int>(num, num2, y, x2), 
								PackingHeuristic.BottomLeft => new ValueTuple<int, int, int, int>(y, x2, num, num2), 
								PackingHeuristic.ContactPoint => new ValueTuple<int, int, int, int>(-num4, y, x2, num), 
								_ => new ValueTuple<int, int, int, int>(-num5, y, x2, num), 
							});
							list3.Add(new ValueTuple<Placement, int, int, int, int, int>(placement, num6, val.Item1, val.Item2, val.Item3, val.Item4));
						}
					}
				}
			}
			if (list3.Count == 0)
			{
				return null;
			}
			Placement item = Enumerable.First((dense ? Enumerable.OrderBy((IEnumerable<ValueTuple<Placement, int, int, int, int, int>>)list3, (ValueTuple<Placement, int, int, int, int, int> v) => v.Item3).ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item4).ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item5)
				.ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item6)
				.ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item2) : Enumerable.OrderBy((IEnumerable<ValueTuple<Placement, int, int, int, int, int>>)list3, (ValueTuple<Placement, int, int, int, int, int> v) => v.Item2).ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item3).ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item4)
				.ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item5)
				.ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item6)).ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item1.GameOrientation).ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item1.StableId, (IComparer<string>?)StringComparer.Ordinal)).Item1;
			list2.Add(item);
			Mark(array, item, value: true);
			foreach (Cell cell in item.Mask.Cells)
			{
				SubtractCell(list, item.X + cell.X, item.Y + cell.Y);
			}
		}
		return Enumerable.ToArray(((IEnumerable<Placement>)list2).OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
	}

	private static int OccupiedContact(LayoutProblem p, bool[,] occ, Placement placement)
	{
		int num = 0;
		foreach (Cell cell2 in placement.Mask.Cells)
		{
			Cell[] array = new Cell[4]
			{
				new Cell(1, 0),
				new Cell(-1, 0),
				new Cell(0, 1),
				new Cell(0, -1)
			};
			for (int i = 0; i < array.Length; i++)
			{
				Cell cell = array[i];
				int num2 = placement.X + cell2.X + cell.X;
				int num3 = placement.Y + cell2.Y + cell.Y;
				if ((uint)num2 >= p.Width || (uint)num3 >= p.Height || occ[num2, num3] || !p.Usable[num2, num3])
				{
					num++;
				}
			}
		}
		return num;
	}

	private static List<FreeRect> BuildFreeRectangles(LayoutProblem p, bool[,] occ)
	{
		List<FreeRect> list = new List<FreeRect>
		{
			new FreeRect(0, 0, p.Width, p.Height)
		};
		for (int i = 0; i < p.Height; i++)
		{
			for (int j = 0; j < p.Width; j++)
			{
				if (occ[j, i] || !p.Usable[j, i])
				{
					SubtractCell(list, j, i);
				}
			}
		}
		return list;
	}

	private static void SubtractCell(List<FreeRect> free, int x, int y)
	{
		List<FreeRect> list = new List<FreeRect>();
		foreach (FreeRect item in free)
		{
			if (x < item.X || x >= item.Right || y < item.Y || y >= item.Bottom)
			{
				list.Add(item);
				continue;
			}
			if (x > item.X)
			{
				list.Add(new FreeRect(item.X, item.Y, x - item.X, item.Height));
			}
			if (x + 1 < item.Right)
			{
				list.Add(new FreeRect(x + 1, item.Y, item.Right - x - 1, item.Height));
			}
			if (y > item.Y)
			{
				list.Add(new FreeRect(item.X, item.Y, item.Width, y - item.Y));
			}
			if (y + 1 < item.Bottom)
			{
				list.Add(new FreeRect(item.X, y + 1, item.Width, item.Bottom - y - 1));
			}
		}
		List<FreeRect> list2 = Enumerable.ToList(Enumerable.Distinct(Enumerable.Where((IEnumerable<FreeRect>)list, (FreeRect r) => r.Width > 0 && r.Height > 0)));
		free.Clear();
		for (int i = 0; i < list2.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < list2.Count; j++)
			{
				if (flag)
				{
					break;
				}
				if (i != j && list2[j].Contains(list2[i]))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				free.Add(list2[i]);
			}
		}
		free.Sort((Comparison<FreeRect>)delegate(FreeRect a, FreeRect b)
		{
			int num = a.Y.CompareTo(b.Y);
			if (num != 0)
			{
				return num;
			}
			num = a.X.CompareTo(b.X);
			if (num != 0)
			{
				return num;
			}
			num = a.Area.CompareTo(b.Area);
			if (num != 0)
			{
				return num;
			}
			num = a.Width.CompareTo(b.Width);
			return (num == 0) ? a.Height.CompareTo(b.Height) : num;
		});
	}

	private static Placement[]? GreedyFirstFit(LayoutProblem p, bool[,] baseOcc, IReadOnlyList<LayoutItem> items, bool reverseX)
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		bool[,] occupied = (bool[,])baseOcc.Clone();
		List<Placement> list = new List<Placement>();
		foreach (LayoutItem item in items)
		{
			List<ValueTuple<Placement, int, int, int, int, int>> list2 = new List<ValueTuple<Placement, int, int, int, int, int>>();
			foreach (Orientation item2 in PreferredOrientations(item, portraitOnly: false))
			{
				for (int i = 0; i <= p.Height - item2.Mask.Height; i++)
				{
					for (int j = 0; j <= p.Width - item2.Mask.Width; j++)
					{
						int x2 = (reverseX ? (p.Width - item2.Mask.Width - j) : j);
						if (Fits(p, occupied, item2.Mask, x2, i))
						{
							Placement placement = new Placement(item.StableId, x2, i, item2.GameOrientation, item2.Mask, item.PrimaryCategory, item2.Rotated);
							int num = ((item2.Mask.Height <= item2.Mask.Width) ? ((item2.Mask.Height == item2.Mask.Width) ? 1 : 2) : 0);
							list2.Add(new ValueTuple<Placement, int, int, int, int, int>(placement, num, SameTypeRowMates((IReadOnlyList<Placement>)list, placement), HorizontalTypeContact((IReadOnlyList<Placement>)list, placement), TypeContact((IReadOnlyList<Placement>)list, placement), j));
						}
					}
				}
			}
			Placement placement2 = Enumerable.FirstOrDefault(Enumerable.Select(Enumerable.OrderBy((IEnumerable<ValueTuple<Placement, int, int, int, int, int>>)list2, (ValueTuple<Placement, int, int, int, int, int> v) => v.Item2).ThenByDescending((ValueTuple<Placement, int, int, int, int, int> v) => v.Item3).ThenByDescending((ValueTuple<Placement, int, int, int, int, int> v) => v.Item4)
				.ThenByDescending((ValueTuple<Placement, int, int, int, int, int> v) => v.Item5)
				.ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item1.Y)
				.ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item6)
				.ThenBy((ValueTuple<Placement, int, int, int, int, int> v) => v.Item1.GameOrientation), (ValueTuple<Placement, int, int, int, int, int> v) => v.Item1));
			if (placement2 == null)
			{
				return null;
			}
			Mark(occupied, placement2, value: true);
			list.Add(placement2);
		}
		return Enumerable.ToArray(((IEnumerable<Placement>)list).OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
	}

	private static Placement[]? DenseSlotReassignment(LayoutProblem p, IReadOnlyList<Placement> original)
	{
		Placement[] array = Enumerable.ToArray(original.OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
		try
		{
			BuildOccupied(p, array);
		}
		catch
		{
			return null;
		}
		Dictionary<string, LayoutItem> itemById = p.Items.ToDictionary((LayoutItem x) => x.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		List<Placement> list = new List<Placement>(array.Length);
		foreach (IGrouping<string, Placement> item2 in Enumerable.OrderBy(array.GroupBy((Placement x) => x.Mask.Signature, (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Placement> g) => g.Key, (IComparer<string>?)StringComparer.Ordinal))
		{
			Placement[] slots = Enumerable.ToArray(Enumerable.OrderBy(item2, (Placement x) => x.Y).ThenBy((Placement x) => x.X).ThenBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
			LayoutItem[] array2 = Enumerable.ToArray(VisualOrder(p, Enumerable.Select(item2, (Placement x) => itemById[x.StableId])));
			int i;
			for (i = 0; i < slots.Length; i++)
			{
				LayoutItem item = array2[i];
				Placement source = Enumerable.Single(array, (Placement x) => x.StableId == item.StableId);
				Orientation orientation = Enumerable.FirstOrDefault(Enumerable.OrderBy(Enumerable.Where(item.Orientations, (Orientation o) => StringComparer.Ordinal.Equals(o.Mask.Signature, slots[i].Mask.Signature)), (Orientation o) => (o.GameOrientation != source.GameOrientation) ? 1 : 0).ThenBy((Orientation o) => o.Rotated).ThenBy((Orientation o) => o.GameOrientation));
				if (orientation == null)
				{
					return null;
				}
				list.Add(new Placement(item.StableId, slots[i].X, slots[i].Y, orientation.GameOrientation, orientation.Mask, item.PrimaryCategory, orientation.Rotated));
			}
		}
		Placement[] array3 = Enumerable.ToArray(((IEnumerable<Placement>)list).OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
		try
		{
			BuildOccupied(p, array3);
			return array3;
		}
		catch
		{
			return null;
		}
	}

	private Placement[]? ImproveCompleteLayout(LayoutProblem p, IReadOnlyList<Placement> original, Stopwatch sw)
	{
		Placement[] array = Enumerable.ToArray(original.OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
		bool[,] occ;
		try
		{
			occ = BuildOccupied(p, array);
		}
		catch (Exception ex)
		{
			Diagnostic = ex.Message;
			return null;
		}
		LayoutScore layoutScore = Score(p, Enumerable.ToArray(p.Fixed.Concat(array)), occ);
		Dictionary<string, LayoutItem> dictionary = p.Items.ToDictionary((LayoutItem x) => x.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		for (int j = 0; j < 4; j++)
		{
			if (!(sw.Elapsed < TimeBudget))
			{
				break;
			}
			bool flag = false;
			string[] array2 = Enumerable.ToArray(Enumerable.OrderBy(Enumerable.Select(array, (Placement x) => x.StableId), (string x) => x, (IComparer<string>?)StringComparer.Ordinal));
			foreach (string id in array2)
			{
				if (sw.Elapsed >= TimeBudget)
				{
					break;
				}
				LayoutItem layoutItem = dictionary[id];
				int index = Array.FindIndex(array, (Placement x) => x.StableId == id);
				Placement placement = array[index];
				Placement[] movable = Enumerable.ToArray(Enumerable.Where(array, (Placement _, int i) => i != index));
				bool[,] array3 = BuildOccupied(p, movable);
				Placement placement2 = placement;
				LayoutScore layoutScore2 = layoutScore;
				foreach (Orientation item in Enumerable.OrderBy(layoutItem.Orientations, (Orientation x) => x.Rotated).ThenBy((Orientation x) => x.GameOrientation))
				{
					for (int l = 0; l <= p.Height - item.Mask.Height; l++)
					{
						for (int m = 0; m <= p.Width - item.Mask.Width; m++)
						{
							if (sw.Elapsed >= TimeBudget)
							{
								break;
							}
							if (Fits(p, array3, item.Mask, m, l))
							{
								Placement placement3 = new Placement(id, m, l, item.GameOrientation, item.Mask, layoutItem.PrimaryCategory, item.Rotated);
								Placement[] array4 = (Placement[])array.Clone();
								array4[index] = placement3;
								bool[,] array5 = (bool[,])array3.Clone();
								Mark(array5, placement3, value: true);
								LayoutScore layoutScore3 = Score(p, Enumerable.ToArray(p.Fixed.Concat(array4)), array5);
								if (CompareForProblem(p, layoutScore3, layoutScore2) > 0)
								{
									layoutScore2 = layoutScore3;
									placement2 = placement3;
								}
							}
						}
					}
				}
				if (!SamePlacement(placement, placement2))
				{
					array[index] = placement2;
					layoutScore = layoutScore2;
					flag = true;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		return Enumerable.ToArray(array.OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal));
	}

	private static bool SamePlacement(Placement a, Placement b)
	{
		if (a.StableId == b.StableId && a.X == b.X && a.Y == b.Y)
		{
			return a.GameOrientation == b.GameOrientation;
		}
		return false;
	}

	private static bool[,] BuildOccupied(LayoutProblem p, IReadOnlyList<Placement> movable)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		bool[,] array = new bool[p.Width, p.Height];
		for (int i = 0; i < p.Height; i++)
		{
			for (int j = 0; j < p.Width; j++)
			{
				array[j, i] = !p.Usable[j, i];
			}
		}
		Dictionary<ValueTuple<int, int>, string> dictionary = new Dictionary<ValueTuple<int, int>, string>();
		foreach (Placement item in p.Fixed.Concat(movable))
		{
			foreach (Cell cell in item.Mask.Cells)
			{
				int num = item.X + cell.X;
				int num2 = item.Y + cell.Y;
				DefaultInterpolatedStringHandler val;
				if ((uint)num >= p.Width || (uint)num2 >= p.Height)
				{
					val = new DefaultInterpolatedStringHandler(37, 8);
					val.AppendLiteral("Out of bounds: ");
					val.AppendFormatted(item.StableId);
					val.AppendLiteral(" at ");
					val.AppendFormatted<int>(item.X);
					val.AppendLiteral(",");
					val.AppendFormatted<int>(item.Y);
					val.AppendLiteral(" o=");
					val.AppendFormatted<int>(item.GameOrientation);
					val.AppendLiteral(" mask=");
					val.AppendFormatted<int>(item.Mask.Width);
					val.AppendLiteral("x");
					val.AppendFormatted<int>(item.Mask.Height);
					val.AppendLiteral(" cell=");
					val.AppendFormatted<int>(num);
					val.AppendLiteral(",");
					val.AppendFormatted<int>(num2);
					throw new InvalidOperationException(val.ToStringAndClear());
				}
				if (!p.Usable[num, num2])
				{
					val = new DefaultInterpolatedStringHandler(20, 3);
					val.AppendLiteral("Disabled cell: ");
					val.AppendFormatted(item.StableId);
					val.AppendLiteral(" at ");
					val.AppendFormatted<int>(num);
					val.AppendLiteral(",");
					val.AppendFormatted<int>(num2);
					throw new InvalidOperationException(val.ToStringAndClear());
				}
				if (dictionary.TryGetValue(new ValueTuple<int, int>(num, num2), out var value))
				{
					val = new DefaultInterpolatedStringHandler(37, 9);
					val.AppendLiteral("Overlap cell ");
					val.AppendFormatted<int>(num);
					val.AppendLiteral(",");
					val.AppendFormatted<int>(num2);
					val.AppendLiteral(": ");
					val.AppendFormatted(item.StableId);
					val.AppendLiteral(" at ");
					val.AppendFormatted<int>(item.X);
					val.AppendLiteral(",");
					val.AppendFormatted<int>(item.Y);
					val.AppendLiteral(" o=");
					val.AppendFormatted<int>(item.GameOrientation);
					val.AppendLiteral(" mask=");
					val.AppendFormatted<int>(item.Mask.Width);
					val.AppendLiteral("x");
					val.AppendFormatted<int>(item.Mask.Height);
					val.AppendLiteral(" with ");
					val.AppendFormatted(value);
					throw new InvalidOperationException(val.ToStringAndClear());
				}
			}
			foreach (Cell cell2 in item.Mask.Cells)
			{
				dictionary[new ValueTuple<int, int>(item.X + cell2.X, item.Y + cell2.Y)] = item.StableId;
			}
			Mark(array, item, value: true);
		}
		return array;
	}

	public static ValueTuple<int, int, int> EvaluateSpaceMetrics(LayoutProblem p, IReadOnlyList<Placement> movable)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		bool[,] occ = BuildOccupied(p, movable);
		ValueTuple<int, int> val = FragmentMetrics(p, occ);
		return new ValueTuple<int, int, int>(val.Item1, val.Item2, Raggedness(p, occ));
	}

	public static LayoutScore Evaluate(LayoutProblem p, IReadOnlyList<Placement> movable)
	{
		ValidateProblem(p);
		bool[,] array = new bool[p.Width, p.Height];
		for (int i = 0; i < p.Height; i++)
		{
			for (int j = 0; j < p.Width; j++)
			{
				array[j, i] = !p.Usable[j, i];
			}
		}
		Placement[] array2 = Enumerable.ToArray(p.Fixed.Concat(movable));
		Placement[] array3 = array2;
		foreach (Placement p2 in array3)
		{
			Mark(array, p2, value: true);
		}
		return Score(p, array2, array);
	}

	private IEnumerable<Placement> EnumerateCandidates(LayoutProblem p, State state, LayoutItem item, Dictionary<string, int> categoryArea)
	{
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		List<ValueTuple<Placement, int, int, int, int, int>> list = new List<ValueTuple<Placement, int, int, int, int, int>>();
		foreach (Orientation item2 in Enumerable.OrderBy(item.Orientations, (Orientation x) => x.GameOrientation))
		{
			for (int i = 0; i <= p.Height - item2.Mask.Height; i++)
			{
				for (int j = 0; j <= p.Width - item2.Mask.Width; j++)
				{
					if (OutOfTime())
					{
						return Enumerable.Select((IEnumerable<ValueTuple<Placement, int, int, int, int, int>>)list, (ValueTuple<Placement, int, int, int, int, int> c) => c.Item1);
					}
					if (Fits(p, state.Occupied, item2.Mask, j, i))
					{
						Placement placement = new Placement(item.StableId, j, i, item2.GameOrientation, item2.Mask, item.PrimaryCategory, item2.Rotated);
						int num = ((item2.Mask.Height <= item2.Mask.Width) ? ((item2.Mask.Height == item2.Mask.Width) ? 1 : 2) : 0);
						list.Add(new ValueTuple<Placement, int, int, int, int, int>(placement, num, SameTypeRowMates((IReadOnlyList<Placement>)state.Placements, placement), HorizontalTypeContact((IReadOnlyList<Placement>)state.Placements, placement), TypeContact((IReadOnlyList<Placement>)state.Placements, placement), CategoryContact((IReadOnlyList<Placement>)state.Placements, placement)));
					}
				}
			}
		}
		return Enumerable.Select(Enumerable.OrderBy((IEnumerable<ValueTuple<Placement, int, int, int, int, int>>)list, (ValueTuple<Placement, int, int, int, int, int> c) => c.Item2).ThenByDescending((ValueTuple<Placement, int, int, int, int, int> c) => c.Item3).ThenByDescending((ValueTuple<Placement, int, int, int, int, int> c) => c.Item4)
			.ThenByDescending((ValueTuple<Placement, int, int, int, int, int> c) => c.Item5)
			.ThenByDescending((ValueTuple<Placement, int, int, int, int, int> c) => c.Item6)
			.ThenBy((ValueTuple<Placement, int, int, int, int, int> c) => c.Item1.Y)
			.ThenBy((ValueTuple<Placement, int, int, int, int, int> c) => c.Item1.X)
			.ThenBy((ValueTuple<Placement, int, int, int, int, int> c) => c.Item1.GameOrientation)
			.ThenBy((ValueTuple<Placement, int, int, int, int, int> c) => c.Item1.StableId, (IComparer<string>?)StringComparer.Ordinal), (ValueTuple<Placement, int, int, int, int, int> c) => c.Item1);
	}

	private static int CategoryAnchor(LayoutProblem p, int categoryIndex)
	{
		if (categoryIndex == int.MaxValue || p.CategoryOrder.Count <= 1)
		{
			return 0;
		}
		return (int)Math.Round((double)((p.Width - 1) * categoryIndex) / (double)(p.CategoryOrder.Count - 1));
	}

	private static IEnumerable<LayoutItem> VisualOrder(LayoutProblem p, IEnumerable<LayoutItem> items)
	{
		LayoutProblem p2 = p;
		LayoutItem[] source = Enumerable.ToArray(items);
		Dictionary<string, ValueTuple<int, int>> groupSize = Enumerable.ToDictionary(source.GroupBy((LayoutItem i) => VisualGroupKey(i), (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, LayoutItem> g) => g.Key, (IGrouping<string, LayoutItem> g) => new ValueTuple<int, int>(Enumerable.Max(g, (LayoutItem i) => i.Area), Enumerable.Max(g, (LayoutItem i) => Enumerable.Max(i.Orientations, (Orientation o) => Math.Max(o.Mask.Width, o.Mask.Height)))), (IEqualityComparer<string>?)StringComparer.Ordinal);
		int value;
		return Enumerable.OrderBy(source, (LayoutItem i) => i.VisualPriority).ThenBy((LayoutItem i) => (!p2.CategoryOrder.TryGetValue(i.PrimaryCategory, out value)) ? int.MaxValue : value).ThenByDescending((LayoutItem i) => groupSize[VisualGroupKey(i)].Item1)
			.ThenByDescending((LayoutItem i) => groupSize[VisualGroupKey(i)].Item2)
			.ThenBy((LayoutItem i) => ItemTypeKey(i.StableId), (IComparer<string>?)StringComparer.Ordinal)
			.ThenByDescending((LayoutItem i) => i.Area)
			.ThenByDescending((LayoutItem i) => i.Complexity)
			.ThenBy((LayoutItem i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal);
	}

	private static string VisualGroupKey(LayoutItem i)
	{
		return ItemTypeKey(i.StableId);
	}

	private static string ShelfCohortKey(LayoutItem i)
	{
		Orientation orientation = Enumerable.First(Enumerable.OrderByDescending(i.Orientations, (Orientation x) => x.Mask.Height > x.Mask.Width).ThenByDescending((Orientation x) => x.Mask.Height - x.Mask.Width).ThenBy((Orientation x) => x.Rotated)
			.ThenBy((Orientation x) => x.GameOrientation));
		return i.PrimaryCategory + "\u001e" + orientation.Mask.Width + "x" + orientation.Mask.Height + "a" + orientation.Mask.Area;
	}

	private static string ShelfCohortKey(Placement p)
	{
		int num = Math.Min(p.Mask.Width, p.Mask.Height);
		int num2 = Math.Max(p.Mask.Width, p.Mask.Height);
		return p.Category + "\u001e" + num + "x" + num2 + "a" + p.Mask.Area;
	}

	private static int EstimateLegalPositions(LayoutProblem p, bool[,] occupied, LayoutItem item)
	{
		int num = 0;
		foreach (Orientation orientation in item.Orientations)
		{
			for (int i = 0; i <= p.Height - orientation.Mask.Height; i++)
			{
				for (int j = 0; j <= p.Width - orientation.Mask.Width; j++)
				{
					if (Fits(p, occupied, orientation.Mask, j, i))
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	private static bool Fits(LayoutProblem p, bool[,] occupied, ShapeMask mask, int x, int y)
	{
		foreach (Cell cell in mask.Cells)
		{
			int num = x + cell.X;
			int num2 = y + cell.Y;
			if ((uint)num >= p.Width || (uint)num2 >= p.Height || !p.Usable[num, num2] || occupied[num, num2])
			{
				return false;
			}
		}
		return true;
	}

	private static void Mark(bool[,] occupied, Placement p, bool value)
	{
		foreach (Cell cell in p.Mask.Cells)
		{
			occupied[p.X + cell.X, p.Y + cell.Y] = value;
		}
	}

	private static int TypeContact(IReadOnlyList<Placement> placed, Placement q)
	{
		string type = ItemTypeKey(q.StableId);
		int num = 0;
		foreach (Placement item in Enumerable.Where(placed, (Placement p) => ItemTypeKey(p.StableId) == type))
		{
			foreach (Cell cell in item.Mask.Cells)
			{
				foreach (Cell cell2 in q.Mask.Cells)
				{
					if (Math.Abs(item.X + cell.X - (q.X + cell2.X)) + Math.Abs(item.Y + cell.Y - (q.Y + cell2.Y)) == 1)
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	private static int SameTypeRowMates(IReadOnlyList<Placement> placed, Placement q)
	{
		Placement q2 = q;
		string type = ItemTypeKey(q2.StableId);
		return Enumerable.Count(placed, (Placement p) => ItemTypeKey(p.StableId) == type && p.Y == q2.Y && p.Mask.Height == q2.Mask.Height);
	}

	private static int HorizontalTypeContact(IReadOnlyList<Placement> placed, Placement q)
	{
		string type = ItemTypeKey(q.StableId);
		int num = 0;
		foreach (Placement item in Enumerable.Where(placed, (Placement p) => ItemTypeKey(p.StableId) == type))
		{
			foreach (Cell cell in item.Mask.Cells)
			{
				foreach (Cell cell2 in q.Mask.Cells)
				{
					if (item.Y + cell.Y == q.Y + cell2.Y && Math.Abs(item.X + cell.X - (q.X + cell2.X)) == 1)
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	private static ValueTuple<int, int, int> ShelfMetrics(IReadOnlyList<Placement> movable, Func<Placement, string> key)
	{
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (IGrouping<string, Placement> item in Enumerable.Where(movable.GroupBy(key, (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Placement> g) => Enumerable.Count(g) > 1))
		{
			Placement[] array = Enumerable.ToArray(item.OrderBy((Placement p) => p.StableId, (IComparer<string>?)StringComparer.Ordinal));
			int[] parent = Enumerable.ToArray(Enumerable.Range(0, array.Length));
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = i + 1; j < array.Length; j++)
				{
					if (array[i].Y == array[j].Y)
					{
						num2++;
					}
					bool flag = false;
					foreach (Cell cell in array[i].Mask.Cells)
					{
						foreach (Cell cell2 in array[j].Mask.Cells)
						{
							int num4 = Math.Abs(array[i].X + cell.X - (array[j].X + cell2.X));
							int num5 = Math.Abs(array[i].Y + cell.Y - (array[j].Y + cell2.Y));
							if (num4 + num5 == 1)
							{
								flag = true;
								if (num5 == 0)
								{
									num3++;
								}
							}
						}
					}
					if (flag)
					{
						Union(i, j);
					}
				}
			}
			num += Enumerable.Count(Enumerable.Distinct(Enumerable.Select(Enumerable.Range(0, array.Length), Find))) - 1;
			int Find(int x)
			{
				while (parent[x] != x)
				{
					parent[x] = parent[parent[x]];
					x = parent[x];
				}
				return x;
			}
			void Union(int x, int y)
			{
				x = Find(x);
				y = Find(y);
				if (x != y)
				{
					parent[y] = x;
				}
			}
		}
		return new ValueTuple<int, int, int>(num, num2, num3);
	}

	private static int CategoryContact(IReadOnlyList<Placement> placed, Placement q)
	{
		Placement q2 = q;
		int num = 0;
		foreach (Placement item in Enumerable.Where(placed, (Placement p) => p.Category == q2.Category))
		{
			foreach (Cell cell in item.Mask.Cells)
			{
				foreach (Cell cell2 in q2.Mask.Cells)
				{
					if (Math.Abs(item.X + cell.X - (q2.X + cell2.X)) + Math.Abs(item.Y + cell.Y - (q2.Y + cell2.Y)) == 1)
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	private static LayoutScore Score(LayoutProblem p, IReadOnlyList<Placement> placements, bool[,] occ, bool includeStableTie = true)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		LayoutProblem p2 = p;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		Dictionary<ValueTuple<int, int>, Placement> dictionary = new Dictionary<ValueTuple<int, int>, Placement>();
		foreach (Placement placement3 in placements)
		{
			foreach (Cell cell in placement3.Mask.Cells)
			{
				dictionary[new ValueTuple<int, int>(placement3.X + cell.X, placement3.Y + cell.Y)] = placement3;
			}
		}
		foreach (KeyValuePair<ValueTuple<int, int>, Placement> item in dictionary)
		{
			ValueTuple<int, int>[] array = new ValueTuple<int, int>[2]
			{
				new ValueTuple<int, int>(1, 0),
				new ValueTuple<int, int>(0, 1)
			};
			for (int j = 0; j < array.Length; j++)
			{
				ValueTuple<int, int> val = array[j];
				if (dictionary.TryGetValue(new ValueTuple<int, int>(item.Key.Item1 + val.Item1, item.Key.Item2 + val.Item2), out var value) && !(item.Value.StableId == value.StableId))
				{
					if (ItemTypeKey(item.Value.StableId) == ItemTypeKey(value.StableId))
					{
						num++;
					}
					if (item.Value.Category == value.Category)
					{
						num2++;
					}
					else
					{
						num3++;
					}
				}
			}
		}
		HashSet<string> movableIds = Enumerable.ToHashSet(Enumerable.Select(p2.Items, (LayoutItem i) => i.StableId), (IEqualityComparer<string>?)StringComparer.Ordinal);
		Placement[] array2 = Enumerable.ToArray(Enumerable.Where(placements, (Placement x) => movableIds.Contains(x.StableId)));
		Dictionary<string, LayoutItem> dictionary2 = p2.Items.ToDictionary((LayoutItem x) => x.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		for (int k = 0; k < array2.Length; k++)
		{
			for (int l = k + 1; l < array2.Length; l++)
			{
				Placement placement = array2[k];
				Placement placement2 = array2[l];
				int num8 = placement.Y * p2.Width + placement.X;
				int num9 = placement2.Y * p2.Width + placement2.X;
				LayoutItem layoutItem = dictionary2[placement.StableId];
				LayoutItem layoutItem2 = dictionary2[placement2.StableId];
				if (layoutItem.VisualPriority != layoutItem2.VisualPriority)
				{
					num5 += ((layoutItem.VisualPriority < layoutItem2.VisualPriority == num8 < num9) ? 1 : (-1));
				}
				int value2;
				int num10 = (p2.CategoryOrder.TryGetValue(layoutItem.PrimaryCategory, out value2) ? value2 : int.MaxValue);
				int value3;
				int num11 = (p2.CategoryOrder.TryGetValue(layoutItem2.PrimaryCategory, out value3) ? value3 : int.MaxValue);
				if (num10 != num11)
				{
					num6 += ((num10 < num11 == num8 < num9) ? 1 : (-1));
				}
				if (placement.Category == placement2.Category && placement.Mask.Area != placement2.Mask.Area)
				{
					num7 += ((placement.Mask.Area > placement2.Mask.Area == num8 < num9) ? 1 : (-1));
				}
			}
		}
		foreach (IGrouping<string, Placement> item2 in array2.GroupBy((Placement x) => ItemTypeKey(x.StableId), (IEqualityComparer<string>?)StringComparer.Ordinal))
		{
			num4 += Enumerable.Max(item2.GroupBy((Placement x) => x.Mask.Signature, (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Placement> x) => Enumerable.Count(x));
		}
		int verticalPreference = Enumerable.Count(array2, (Placement x) => x.Mask.Height > x.Mask.Width);
		ValueTuple<int, int, int> val2 = ShelfMetrics(array2, (Placement p) => ItemTypeKey(p.StableId));
		ValueTuple<int, int, int> val3 = ShelfMetrics(array2, ShelfCohortKey);
		int topLeftCompactness = -Enumerable.Sum(array2, (Placement x) => x.Y * p2.Width + x.X);
		int rowRegularity = -Raggedness(p2, occ);
		int num12 = Enumerable.Count(array2, (Placement x) => x.Rotated);
		ValueTuple<int, int> val4 = FragmentMetrics(p2, occ);
		return new LayoutScore(placements.Count, Enumerable.Sum(placements, (Placement x) => x.Mask.Area), -val2.Item1, val2.Item2, val2.Item3, -val3.Item1, val3.Item2, val3.Item3, num4, verticalPreference, num5, num6, num7, num, num2, topLeftCompactness, rowRegularity, -(val4.Item1 * 8 + val4.Item2), -num3, -num12, includeStableTie ? PlacementKey(placements) : string.Empty);
	}

	private static int Raggedness(LayoutProblem p, bool[,] occ)
	{
		int num = 0;
		int num2 = 0;
		int num3 = -1;
		for (int i = 0; i < p.Height; i++)
		{
			int num4 = -1;
			for (int j = 0; j < p.Width; j++)
			{
				if (p.Usable[j, i] && occ[j, i])
				{
					num4 = j;
				}
			}
			if (num4 < 0)
			{
				continue;
			}
			for (int k = 0; k <= num4; k++)
			{
				if (p.Usable[k, i] && !occ[k, i])
				{
					num++;
				}
			}
			if (num3 >= 0)
			{
				num2 += Math.Abs(num3 - num4);
			}
			num3 = num4;
		}
		return num * 4 + num2;
	}

	private static ValueTuple<int, int> FragmentMetrics(LayoutProblem p, bool[,] occ)
	{
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		bool[,] array = new bool[p.Width, p.Height];
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < p.Height; i++)
		{
			for (int j = 0; j < p.Width; j++)
			{
				if (array[j, i] || occ[j, i] || !p.Usable[j, i])
				{
					continue;
				}
				Queue<Cell> queue = new Queue<Cell>();
				queue.Enqueue(new Cell(j, i));
				array[j, i] = true;
				int num3 = 0;
				bool flag = false;
				while (queue.Count > 0)
				{
					Cell cell = queue.Dequeue();
					num3++;
					flag |= cell.X == 0 || cell.Y == 0 || cell.X == p.Width - 1 || cell.Y == p.Height - 1;
					Cell[] array2 = new Cell[4]
					{
						new Cell(1, 0),
						new Cell(-1, 0),
						new Cell(0, 1),
						new Cell(0, -1)
					};
					for (int k = 0; k < array2.Length; k++)
					{
						Cell cell2 = array2[k];
						int num4 = cell.X + cell2.X;
						int num5 = cell.Y + cell2.Y;
						if ((uint)num4 < p.Width && (uint)num5 < p.Height && !array[num4, num5] && !occ[num4, num5] && p.Usable[num4, num5])
						{
							array[num4, num5] = true;
							queue.Enqueue(new Cell(num4, num5));
						}
					}
				}
				if (!flag)
				{
					num += num3;
				}
				if (num3 <= 2)
				{
					num2 += num3;
				}
			}
		}
		return new ValueTuple<int, int>(num, num2);
	}

	private static int EdgeRegularity(LayoutProblem p, bool[,] occ)
	{
		int num = 0;
		for (int i = 0; i < p.Height; i++)
		{
			for (int j = 0; j < p.Width; j++)
			{
				if (occ[j, i])
				{
					if (j == 0 || occ[j - 1, i])
					{
						num++;
					}
					if (i == 0 || occ[j, i - 1])
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	private static string ItemTypeKey(string stableId)
	{
		int num = stableId.IndexOf('\u001f');
		if (num >= 0)
		{
			return stableId.Substring(0, num);
		}
		return stableId;
	}

	private static string PlacementKey(IEnumerable<Placement> placements)
	{
		return string.Join("|", Enumerable.Select(placements.OrderBy((Placement x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal), delegate(Placement x)
		{
			DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
			val = new DefaultInterpolatedStringHandler(3, 4);
			val.AppendFormatted(x.StableId);
			val.AppendLiteral(":");
			val.AppendFormatted<int>(x.X);
			val.AppendLiteral(",");
			val.AppendFormatted<int>(x.Y);
			val.AppendLiteral(",");
			val.AppendFormatted<int>(x.GameOrientation);
			return val.ToStringAndClear();
		}));
	}

	private static Placement[]? OriginalLayout(LayoutProblem p)
	{
		bool[,] array = new bool[p.Width, p.Height];
		for (int k = 0; k < p.Height; k++)
		{
			for (int l = 0; l < p.Width; l++)
			{
				array[l, k] = !p.Usable[l, k];
			}
		}
		foreach (Placement item in p.Fixed)
		{
			if (!Fits(p, array, item.Mask, item.X, item.Y))
			{
				return null;
			}
			Mark(array, item, value: true);
		}
		List<Placement> list = new List<Placement>();
		foreach (LayoutItem j in p.Items.OrderBy((LayoutItem i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal))
		{
			Orientation orientation = Enumerable.FirstOrDefault(j.Orientations, (Orientation o) => o.GameOrientation == j.OriginalOrientation) ?? j.Orientations[0];
			if (!Fits(p, array, orientation.Mask, j.OriginalX, j.OriginalY))
			{
				return null;
			}
			Placement placement = new Placement(j.StableId, j.OriginalX, j.OriginalY, orientation.GameOrientation, orientation.Mask, j.PrimaryCategory, orientation.Mask.Width > orientation.Mask.Height);
			Mark(array, placement, value: true);
			list.Add(placement);
		}
		return list.ToArray();
	}

	public static string DescribeSplits(IReadOnlyList<Placement> placements)
	{
		List<string> list = new List<string>();
		foreach (IGrouping<string, Placement> item in Enumerable.Where(Enumerable.Where(placements, (Placement p) => p.Category != "~fixed").GroupBy((Placement p) => ItemTypeKey(p.StableId), (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Placement> g) => Enumerable.Count(g) > 1))
		{
			int num = HorizontalRunCount(Enumerable.ToArray(item));
			if (num > 1)
			{
				list.Add(item.Key + "x" + num);
			}
		}
		if (list.Count != 0)
		{
			return string.Join(",", Enumerable.Take(Enumerable.OrderBy((IEnumerable<string>)list, (string x) => x, (IComparer<string>?)StringComparer.Ordinal), 16));
		}
		return "none";
	}

	private static int HorizontalRunCount(IReadOnlyList<Placement> a)
	{
		int[] parent = Enumerable.ToArray(Enumerable.Range(0, a.Count));
		for (int i = 0; i < a.Count; i++)
		{
			for (int j = i + 1; j < a.Count; j++)
			{
				bool flag = false;
				foreach (Cell cell in a[i].Mask.Cells)
				{
					foreach (Cell cell2 in a[j].Mask.Cells)
					{
						if (Math.Abs(a[i].X + cell.X - (a[j].X + cell2.X)) + Math.Abs(a[i].Y + cell.Y - (a[j].Y + cell2.Y)) == 1)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					Union(i, j);
				}
			}
		}
		return Enumerable.Count(Enumerable.Distinct(Enumerable.Select(Enumerable.Range(0, a.Count), Find)));
		int Find(int x)
		{
			while (parent[x] != x)
			{
				parent[x] = parent[parent[x]];
				x = parent[x];
			}
			return x;
		}
		void Union(int x, int y)
		{
			x = Find(x);
			y = Find(y);
			if (x != y)
			{
				parent[y] = x;
			}
		}
	}

	public static void ValidateProblem(LayoutProblem p)
	{
		if (p.Width <= 0 || p.Height <= 0 || p.Usable.GetLength(0) != p.Width || p.Usable.GetLength(1) != p.Height)
		{
			throw new ArgumentException("Invalid warehouse dimensions.");
		}
		if (Enumerable.Count(Enumerable.Distinct(Enumerable.Select(p.Items, (LayoutItem i) => i.StableId), (IEqualityComparer<string>?)StringComparer.Ordinal)) != p.Items.Count)
		{
			throw new ArgumentException("Stable IDs must be unique.");
		}
		if (Enumerable.Any(p.Items, (LayoutItem i) => i.Orientations.Count == 0))
		{
			throw new ArgumentException("Every item needs an orientation.");
		}
	}
}
