using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace SmartInventoryArrange.Core;

public sealed class Native34Solver
{
	private sealed class Ranked
	{
		public string Key = "";

		public Placement[] Q = Array.Empty<Placement>();

		public Metrics M;

		public double Value;

		public double? Preference;
	}

	public sealed record SearchProgress(LayoutResult Result, int Batch, long NewNodes, long TotalNodes, int NewCandidates, int TotalCandidates, int RawSolutions, bool Improved, bool Exhausted, bool CanContinue, string Reason);

	private sealed record SearchTick(Placement[]? Solution);

	private sealed class Route
	{
		public string Id = "";

		public IEnumerator<SearchTick> Iterator;

		public bool Done;

		public long Nodes;
	}

	public sealed class SearchSession : IDisposable
	{
		private readonly object _gate = new object();

		private readonly Native34Solver _owner;

		private readonly Placement[] _rollback;

		private readonly HashSet<string> _seen = new HashSet<string>((IEqualityComparer<string>)StringComparer.Ordinal);

		private readonly List<Route> _routes = new List<Route>();

		private bool _seeded;

		private bool _disposed;

		private bool _candidateCap;

		private int _route;

		private int _batch;

		private int _raw;

		private int _quantumUsed;

		private string? _bestKey;

		public long TotalNodes { get; private set; }

		public int UniqueCandidates => _seen.Count;

		public string Diagnostic => _owner.Diagnostic;

		public bool Exhausted
		{
			get
			{
				if (_seeded)
				{
					return Enumerable.All((IEnumerable<Route>)_routes, (Route r) => r.Done);
				}
				return false;
			}
		}

		public bool CanContinue
		{
			get
			{
				if (!_disposed && !_candidateCap)
				{
					return !Exhausted;
				}
				return false;
			}
		}

		internal SearchSession(Native34Solver owner, Placement[] rollback)
		{
			_owner = owner;
			_rollback = rollback;
		}

		private void Seeds()
		{
			if (_seeded)
			{
				return;
			}
			string[] array = new string[3] { "shelf", "compact", "unit-tail" };
			foreach (string layout in array)
			{
				string[] array2 = new string[8] { "item-area", "family-area", "height", "width", "cohort", "category", "constrained", "small" };
				foreach (string order in array2)
				{
					_owner.RunStage("fresh:" + layout + ":" + order, delegate
					{
						_owner.Shelf(order, "portrait", layout);
					});
				}
			}
			if (_owner._p.Items.Count == 0)
			{
				_owner.Add(Array.Empty<Placement>(), "fresh:empty");
			}
			foreach (string key in _owner._pool.Keys)
			{
				_seen.Add(key);
			}
			array = new string[4] { "item-area", "height", "constrained", "small" };
			foreach (string text in array)
			{
				bool[] array3 = new bool[2] { true, false };
				foreach (bool flag in array3)
				{
					string id = "resume:" + text + ":" + (flag ? "whole" : "flexible");
					_routes.Add(new Route
					{
						Id = id,
						Iterator = _owner.EnumerateFresh(text, flag).GetEnumerator()
					});
				}
			}
			_seeded = true;
		}

		public SearchProgress Advance(int nodeBudget = 25000, int solutionBatch = 64)
		{
			lock (_gate)
			{
				return AdvanceCore(nodeBudget, solutionBatch);
			}
		}

		private SearchProgress AdvanceCore(int nodeBudget, int solutionBatch)
		{
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			if (nodeBudget < 1 || solutionBatch < 1)
			{
				throw new ArgumentException("Positive work and solution batches required.");
			}
			if (_disposed)
			{
				throw new ObjectDisposedException("SearchSession");
			}
			Stopwatch val = Stopwatch.StartNew();
			long allocatedBytesForCurrentThread = GC.GetAllocatedBytesForCurrentThread();
			long totalNodes = TotalNodes;
			int count = _seen.Count;
			int raw = _raw;
			Seeds();
			while (TotalNodes - totalNodes < nodeBudget && _raw - raw < solutionBatch && !_candidateCap && Enumerable.Any((IEnumerable<Route>)_routes, (Route r) => !r.Done))
			{
				Route route = _routes[_route];
				if (route.Done)
				{
					_route = (_route + 1) % _routes.Count;
					_quantumUsed = 0;
					continue;
				}
				if (!route.Iterator.MoveNext())
				{
					route.Done = true;
					route.Iterator.Dispose();
					_route = (_route + 1) % _routes.Count;
					_quantumUsed = 0;
					continue;
				}
				_quantumUsed++;
				SearchTick current = route.Iterator.Current;
				if (current.Solution == null)
				{
					TotalNodes++;
					route.Nodes++;
				}
				else
				{
					_raw++;
					string text = Key(current.Solution);
					if (_seen.Add(text))
					{
						_owner.Add(current.Solution, route.Id);
					}
					else if (_owner._pool.ContainsKey(text))
					{
						_owner.Add(current.Solution, route.Id);
					}
					if (_seen.Count >= 4096)
					{
						_candidateCap = true;
					}
				}
				if (_quantumUsed == 128)
				{
					_route = (_route + 1) % _routes.Count;
					_quantumUsed = 0;
				}
			}
			List<Ranked> list = Enumerable.ToList(Enumerable.Select((IEnumerable<string>)(object)_owner._pool.Keys, _owner.Rank));
			list.Sort((Comparison<Ranked>)_owner.CompareRank);
			foreach (Ranked item in Enumerable.Skip((IEnumerable<Ranked>)list, 256))
			{
				_owner._pool.Remove(item.Key);
			}
			Ranked best = Enumerable.FirstOrDefault((IEnumerable<Ranked>)list);
			bool flag = best != null && best.Key != _bestKey;
			_bestKey = best?.Key;
			Placement[] array = Enumerable.ToArray(best?.Q ?? _rollback);
			if (!ValidateLayout(_owner._p, array))
			{
				throw new InvalidOperationException("Final immutable geometry validation failed.");
			}
			_owner.CandidateRecords = Enumerable.ToArray(Enumerable.Select(Enumerable.Take((IEnumerable<Ranked>)list, 256), (Ranked r) => new CandidateTrace(Hash(r.Key), Enumerable.ToArray((IEnumerable<string>)_owner._pool[r.Key].Item2), Valid: true, (r == best) ? "" : "ranked-lower", Summary(r.M), r == best)));
			Metrics m = _owner.Measure(array);
			LayoutScore score = DeterministicSolver.Evaluate(_owner._p, array);
			ValueTuple<int, int, int> val2 = DeterministicSolver.EvaluateSpaceMetrics(_owner._p, array);
			string text2 = ((best == null) ? "no-fresh-complete-layout" : (_candidateCap ? "candidate-memory-cap" : (Exhausted ? "generated-search-space-exhausted" : (flag ? "fresh-best-selected" : "no-better-candidate-this-batch"))));
			val.Stop();
			_owner._performance.Add(new StageTiming(_batch, "session-advance", val.Elapsed.TotalMilliseconds, GC.GetAllocatedBytesForCurrentThread() - allocatedBytesForCurrentThread, (int)(TotalNodes - totalNodes)));
			Native34Solver owner = _owner;
			DefaultInterpolatedStringHandler val3 = default(DefaultInterpolatedStringHandler);
			val3 = new DefaultInterpolatedStringHandler(140, 9);
			val3.AppendLiteral("engine=global313-session, batch=");
			val3.AppendFormatted<int>(_batch);
			val3.AppendLiteral(", newNodes=");
			val3.AppendFormatted<long>(TotalNodes - totalNodes);
			val3.AppendLiteral(", totalNodes=");
			val3.AppendFormatted<long>(TotalNodes);
			val3.AppendLiteral(", unique=");
			val3.AppendFormatted<int>(_seen.Count);
			val3.AppendLiteral(", retained=");
			val3.AppendFormatted<int>(_owner._pool.Count);
			val3.AppendLiteral(", rawSolutions=");
			val3.AppendFormatted<int>(_raw);
			val3.AppendLiteral(", source=fresh-only, reason=");
			val3.AppendFormatted(text2);
			val3.AppendLiteral(", after=[");
			val3.AppendFormatted(Summary(m));
			val3.AppendLiteral("], routes=[");
			val3.AppendFormatted(string.Join(';', Enumerable.Select((IEnumerable<Route>)_routes, delegate(Route r)
			{
				DefaultInterpolatedStringHandler val4 = default(DefaultInterpolatedStringHandler);
				val4 = new DefaultInterpolatedStringHandler(13, 3);
				val4.AppendFormatted(r.Id);
				val4.AppendLiteral(":nodes=");
				val4.AppendFormatted<long>(r.Nodes);
				val4.AppendLiteral(":done=");
				val4.AppendFormatted<bool>(r.Done);
				return val4.ToStringAndClear();
			})));
			val3.AppendLiteral("]");
			owner.Diagnostic = val3.ToStringAndClear();
			return new SearchProgress(new LayoutResult(Complete: true, array, score, best == null, TimedOut: false, val.Elapsed, val2.Item1, val2.Item2), _batch++, TotalNodes - totalNodes, TotalNodes, _seen.Count - count, _seen.Count, _raw - raw, flag, Exhausted, CanContinue, text2);
		}

		public void Dispose()
		{
			lock (_gate)
			{
				if (_disposed)
				{
					return;
				}
				foreach (Route route in _routes)
				{
					route.Iterator.Dispose();
				}
				_routes.Clear();
				_disposed = true;
			}
		}
	}

	public sealed record StageTiming(int Pass, string Stage, double Milliseconds, long AllocatedBytes, int Nodes);

	private sealed record PackedTemplate(Template T, BigInteger Bits, Cell First);

	private readonly record struct SearchState(BigInteger Occupied, BigInteger Remaining);

	private sealed class SearchChoice
	{
		public int Gi;

		public int Count;

		public int X;

		public int Y;

		public int Offset;

		public int Anchor;

		public int Continuation;

		public int Sequence;

		public PackedTemplate Pattern;

		public BigInteger Bits;

		public string? TieKey;
	}

	private sealed record Family(string Key, LayoutItem[] Items);

	private sealed record Template(Placement[] Items, int Width, int Height, int Rows, int Landscape);

	private sealed record GroupMetric(string Id, int Count, int Regions, int DetachedArea, int Rows, int Mixed, int Portrait, bool Interlock, int Left, int Top, int Bottom, int ItemArea);

	private sealed record Metrics(GroupMetric[] Groups, int Regions, int Detached, int ExtraRows, int Mixed, int Landscape, int PortraitInterlock, int Overlaps, int Left, int Inversions, int Middle, int LargestMiddle, int Height, double AnchorY, int AnchorArea, double Priority, int CohortRows, double RowDebt, int AnchorIntrusion);

	public sealed record CandidateTrace(string Signature, string[] Generators, bool Valid, string RejectedReason, string Metrics, bool Selected);

	private SearchSession? _activeSession;

	private readonly HashSet<string> _interlockAnchors = new HashSet<string>((IEqualityComparer<string>)StringComparer.Ordinal);

	private readonly Dictionary<string, Ranked> _rankCache = new Dictionary<string, Ranked>((IEqualityComparer<string>)StringComparer.Ordinal);

	private LayoutProblem _p;

	private FrozenPreferenceModel _model;

	private Dictionary<string, LayoutItem> _docs;

	private List<Family> _families;

	private Dictionary<string, int> _currentOrientations = new Dictionary<string, int>((IEqualityComparer<string>)StringComparer.Ordinal);

	private Dictionary<string, List<Template>> _templates = new Dictionary<string, List<Template>>((IEqualityComparer<string>)StringComparer.Ordinal);

	private readonly List<string> _runs = new List<string>();

	private readonly Dictionary<string, Metrics> _metricCache = new Dictionary<string, Metrics>((IEqualityComparer<string>)StringComparer.Ordinal);

	private readonly Dictionary<string, bool> _interlockCache = new Dictionary<string, bool>((IEqualityComparer<string>)StringComparer.Ordinal);

	private readonly Dictionary<ValueTuple<string, string>, Orientation[]> _optionCache = new Dictionary<ValueTuple<string, string>, Orientation[]>();

	private readonly Dictionary<Template, PackedTemplate> _packed = new Dictionary<Template, PackedTemplate>(ReferenceEqualityComparer<Template>.Instance);

	private readonly Dictionary<string, Dictionary<string, Orientation>> _legalOrientations = new Dictionary<string, Dictionary<string, Orientation>>((IEqualityComparer<string>)StringComparer.Ordinal);

	private readonly List<StageTiming> _performance = new List<StageTiming>();

	private int _pass;

	private int _lastNodes;

	private readonly Dictionary<string, ValueTuple<Placement[], SortedSet<string>>> _pool = new Dictionary<string, ValueTuple<Placement[], SortedSet<string>>>((IEqualityComparer<string>)StringComparer.Ordinal);

	public int NodeLimit { get; init; } = 250000;


	public int MaxPasses { get; init; } = 8;


	public string Diagnostic { get; private set; } = "";


	public IReadOnlyList<StageTiming> Performance => (IReadOnlyList<StageTiming>)_performance;

	public IReadOnlyList<CandidateTrace> CandidateRecords { get; private set; } = Array.Empty<CandidateTrace>();


	private Ranked Rank(string key)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (_rankCache.TryGetValue(key, out Ranked value))
		{
			return value;
		}
		Placement[] item = _pool[key].Item1;
		Metrics m = Measure(item);
		value = new Ranked
		{
			Key = key,
			Q = item,
			M = m,
			Value = Value(m)
		};
		_rankCache[key] = value;
		return value;
	}

	private int CompareRank(Ranked a, Ranked b)
	{
		int num = a.Value.CompareTo(b.Value);
		if (num != 0)
		{
			return num;
		}
		Ranked ranked = a;
		double valueOrDefault = ranked.Preference.GetValueOrDefault();
		if (!ranked.Preference.HasValue)
		{
			valueOrDefault = _model.Value(FeatureVector(a.Q));
			ranked.Preference = valueOrDefault;
		}
		ranked = b;
		valueOrDefault = ranked.Preference.GetValueOrDefault();
		if (!ranked.Preference.HasValue)
		{
			valueOrDefault = _model.Value(FeatureVector(b.Q));
			ranked.Preference = valueOrDefault;
		}
		num = b.Preference.Value.CompareTo(a.Preference.Value);
		if (num != 0)
		{
			return num;
		}
		num = a.M.Priority.CompareTo(b.M.Priority);
		if (num == 0)
		{
			return StringComparer.Ordinal.Compare(a.Key, b.Key);
		}
		return num;
	}

	private Family[] OrderedFamilies(string order, bool unitTail = false)
	{
		IOrderedEnumerable<Family> source = Enumerable.OrderBy((IEnumerable<Family>)_families, (Family g) => unitTail && g.Items[0].Area == 1).ThenBy((Family g) => !Enumerable.Any(g.Items, Anchor));
		return Enumerable.ToArray((order switch
		{
			"height" => source.ThenByDescending(Height).ThenByDescending((Family g) => Enumerable.Sum(g.Items, (LayoutItem i) => i.Area)), 
			"width" => source.ThenByDescending(Width).ThenByDescending(Height), 
			"cohort" => source.ThenBy((Family g) => g.Items[0].PrimaryCategory, (IComparer<string>?)StringComparer.Ordinal).ThenByDescending(Height).ThenByDescending(Width), 
			"category" => source.ThenBy((Family g) => g.Items[0].PrimaryCategory, (IComparer<string>?)StringComparer.Ordinal).ThenByDescending((Family g) => Enumerable.Sum(g.Items, (LayoutItem i) => i.Area)), 
			"constrained" => source.ThenBy((Family g) => g.Items[0].Orientations.Count).ThenByDescending((Family g) => Enumerable.Sum(g.Items, (LayoutItem i) => i.Area)), 
			"small" => source.ThenBy((Family g) => Enumerable.Max(g.Items, (LayoutItem i) => i.Area)).ThenByDescending(Height), 
			"family-area" => source.ThenByDescending((Family g) => Enumerable.Sum(g.Items, (LayoutItem i) => i.Area)), 
			_ => source.ThenByDescending((Family g) => Enumerable.Max(g.Items, (LayoutItem i) => i.Area)).ThenByDescending((Family g) => Enumerable.Sum(g.Items, (LayoutItem i) => i.Area)), 
		}).ThenBy((Family g) => g.Key, (IComparer<string>?)StringComparer.Ordinal));
		int Height(Family g)
		{
			return Options(g.Items[0], "portrait")[0].Mask.Height;
		}
		int Width(Family g)
		{
			return Options(g.Items[0], "portrait")[0].Mask.Width;
		}
	}

	public SearchSession CreateSession(LayoutProblem p, IReadOnlyList<Placement>? original = null)
	{
		DeterministicSolver.ValidateProblem(p);
		if (NodeLimit < 1 || MaxPasses < 1)
		{
			throw new ArgumentException("Positive deterministic budgets required.");
		}
		_activeSession?.Dispose();
		LayoutItem[] array = Enumerable.ToArray(Enumerable.Select(p.Items.OrderBy((LayoutItem i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal), (LayoutItem i) => new LayoutItem
		{
			StableId = i.StableId,
			ItemId = i.ItemId,
			PrimaryCategory = i.PrimaryCategory,
			Categories = Enumerable.ToArray(i.Categories),
			VisualPriority = i.VisualPriority,
			OriginalX = i.OriginalX,
			OriginalY = i.OriginalY,
			OriginalOrientation = i.OriginalOrientation,
			OriginalMask = ((i.OriginalMask == null) ? null : Mask(i.OriginalMask)),
			Orientations = Enumerable.ToArray(Enumerable.Select(i.Orientations, (Orientation o) => new Orientation(o.GameOrientation, Mask(o.Mask), o.Rotated)))
		}));
		_p = new LayoutProblem
		{
			Width = p.Width,
			Height = p.Height,
			Usable = (bool[,])p.Usable.Clone(),
			Items = array,
			Fixed = Enumerable.ToArray(Enumerable.Select(p.Fixed, (Placement q) => q with
			{
				Mask = Mask(q.Mask)
			})),
			CategoryOrder = (IReadOnlyDictionary<string, int>)new Dictionary<string, int>((IEnumerable<KeyValuePair<string, int>>)p.CategoryOrder, (IEqualityComparer<string>)StringComparer.Ordinal)
		};
		_model = FrozenPreferenceModel.LoadEmbedded();
		_docs = array.ToDictionary((LayoutItem i) => i.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		_templates.Clear();
		_runs.Clear();
		_metricCache.Clear();
		_interlockCache.Clear();
		_optionCache.Clear();
		_packed.Clear();
		_performance.Clear();
		_legalOrientations.Clear();
		_rankCache.Clear();
		_pool.Clear();
		_pass = 0;
		_currentOrientations.Clear();
		LayoutItem[] array2 = array;
		foreach (LayoutItem layoutItem in array2)
		{
			_legalOrientations[layoutItem.StableId] = Enumerable.ToDictionary(layoutItem.Orientations.GroupBy((Orientation o) => o.Mask.Signature, (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Orientation> g) => g.Key, (IGrouping<string, Orientation> g) => Enumerable.First(Enumerable.OrderBy(g, (Orientation o) => o.GameOrientation)), (IEqualityComparer<string>?)StringComparer.Ordinal);
		}
		_families = Enumerable.ToList(Enumerable.Select(Enumerable.OrderBy(Enumerable.GroupBy(array, (LayoutItem i) => Type(i) + "\u001e" + ShapeKey(i), (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, LayoutItem> g) => g.Key, (IComparer<string>?)StringComparer.Ordinal), (IGrouping<string, LayoutItem> g) => new Family(g.Key, Enumerable.ToArray(g))));
		_interlockAnchors.Clear();
		foreach (Family family in _families)
		{
			if (family.Items.Length > 1 && Enumerable.Any(family.Items, Anchor) && Interlocking(family.Items[0]))
			{
				array2 = family.Items;
				foreach (LayoutItem layoutItem2 in array2)
				{
					_interlockAnchors.Add(layoutItem2.StableId);
				}
			}
		}
		Placement[] array3 = Enumerable.ToArray(Enumerable.Select(original ?? Enumerable.ToArray(Enumerable.Select(array, (LayoutItem i) => At(i, i.OriginalX, i.OriginalY, (i.OriginalMask != null) ? new Orientation(i.OriginalOrientation, i.OriginalMask, Rotated: false) : Enumerable.First(i.Orientations, (Orientation o) => o.GameOrientation == i.OriginalOrientation)))), (Placement q) => q with
		{
			Mask = Mask(q.Mask)
		}).OrderBy((Placement q) => q.StableId, (IComparer<string>?)StringComparer.Ordinal));
		if (!ValidateLayout(_p, array3))
		{
			throw new ArgumentException("Complete valid original snapshot required for rollback only.");
		}
		_activeSession = new SearchSession(this, array3);
		return _activeSession;
		static ShapeMask Mask(ShapeMask m)
		{
			return new ShapeMask(m.Cells);
		}
	}

	private IEnumerable<SearchTick> EnumerateFresh(string order, bool whole)
	{
		int W = _p.Width;
		int H = _p.Height;
		BigInteger full = (BigInteger.One << checked(W * H)) - 1;
		BigInteger bigInteger = 0;
		for (int j = 0; j < H; j++)
		{
			for (int k = 0; k < W; k++)
			{
				if (!_p.Usable[k, j])
				{
					bigInteger |= BigInteger.One << j * W + k;
				}
			}
		}
		foreach (Cell item2 in Cells(_p.Fixed))
		{
			bigInteger |= BigInteger.One << item2.Y * W + item2.X;
		}
		Family[] groups = OrderedFamilies(order);
		int[] counts = Enumerable.ToArray(Enumerable.Select(groups, (Family g) => g.Items.Length));
		List<ValueTuple<int, PackedTemplate>>[] patterns = new List<ValueTuple<int, PackedTemplate>>[groups.Length];
		BigInteger[] radix = new BigInteger[groups.Length];
		BigInteger bigInteger2 = 1;
		BigInteger remainingCode2 = 0;
		for (int l = 0; l < groups.Length; l++)
		{
			radix[l] = bigInteger2;
			remainingCode2 += bigInteger2 * counts[l];
			bigInteger2 *= (BigInteger)(counts[l] + 1);
			patterns[l] = new List<ValueTuple<int, PackedTemplate>>();
			IEnumerable<int> enumerable;
			if (!whole)
			{
				enumerable = Enumerable.Reverse(Enumerable.Range(1, counts[l]));
			}
			else
			{
				IEnumerable<int> enumerable2 = new int[1] { counts[l] };
				enumerable = enumerable2;
			}
			foreach (int item3 in enumerable)
			{
				foreach (Template item4 in Templates(Enumerable.ToArray(Enumerable.Take(groups[l].Items, item3)), "portrait"))
				{
					patterns[l].Add(new ValueTuple<int, PackedTemplate>(item3, Pack(item4)));
				}
			}
			patterns[l] = Enumerable.ToList(Enumerable.OrderByDescending((IEnumerable<ValueTuple<int, PackedTemplate>>)patterns[l], (ValueTuple<int, PackedTemplate> t) => t.Item1).ThenBy((ValueTuple<int, PackedTemplate> t) => t.Item2.T.Rows).ThenBy((ValueTuple<int, PackedTemplate> t) => t.Item2.T.Landscape)
				.ThenBy((ValueTuple<int, PackedTemplate> t) => t.Item2.T.Height)
				.ThenBy((ValueTuple<int, PackedTemplate> t) => t.Item2.T.Width));
		}
		int[] anchors = Enumerable.ToArray(Enumerable.Select(groups, (Family g) => (!Enumerable.Any(g.Items, Anchor)) ? 1 : 0));
		List<SearchChoice> path = new List<SearchChoice>();
		HashSet<SearchState> dead = new HashSet<SearchState>();
		long solutions = 0L;
		int num = Enumerable.Sum(groups, (Family g) => Enumerable.Sum(g.Items, (LayoutItem i) => i.Area));
		int num2 = W * H - CountBits(bigInteger) - num;
		if (num2 < 0)
		{
			yield break;
		}
		foreach (SearchTick item5 in Dfs(bigInteger, remainingCode2, num, num2, -1))
		{
			yield return item5;
		}
		int Compare(SearchChoice a, SearchChoice b)
		{
			int num3 = a.Continuation.CompareTo(b.Continuation);
			if (num3 != 0)
			{
				return num3;
			}
			num3 = a.Anchor.CompareTo(b.Anchor);
			if (num3 != 0)
			{
				return num3;
			}
			num3 = b.Count.CompareTo(a.Count);
			if (num3 != 0)
			{
				return num3;
			}
			num3 = a.Pattern.T.Rows.CompareTo(b.Pattern.T.Rows);
			if (num3 != 0)
			{
				return num3;
			}
			num3 = a.Pattern.T.Landscape.CompareTo(b.Pattern.T.Landscape);
			if (num3 != 0)
			{
				return num3;
			}
			num3 = a.Gi.CompareTo(b.Gi);
			if (num3 != 0)
			{
				return num3;
			}
			StringComparer ordinal = StringComparer.Ordinal;
			SearchChoice searchChoice = a;
			string? obj = searchChoice.TieKey ?? (searchChoice.TieKey = Key(Materialize(a)));
			searchChoice = b;
			num3 = ordinal.Compare(obj, searchChoice.TieKey ?? (searchChoice.TieKey = Key(Materialize(b))));
			if (num3 == 0)
			{
				return a.Sequence.CompareTo(b.Sequence);
			}
			return num3;
		}
		IEnumerable<SearchTick> Dfs(BigInteger occupied, BigInteger remainingCode, int area, int holes, int last)
		{
			yield return new SearchTick(null);
			if (area == 0)
			{
				solutions++;
				yield return new SearchTick(Enumerable.ToArray(Enumerable.SelectMany((IEnumerable<SearchChoice>)path, Materialize)));
			}
			else
			{
				SearchState state = new SearchState(occupied, remainingCode);
				if (!dead.Contains(state))
				{
					BigInteger value = full ^ occupied;
					if (!value.IsZero)
					{
						int first = LowestBit(value);
						int num4 = first % W;
						int num5 = first / W;
						long priorSolutions = solutions;
						List<SearchChoice> list = new List<SearchChoice>();
						for (int n = 0; n < groups.Length; n++)
						{
							if (counts[n] > 0)
							{
								foreach (ValueTuple<int, PackedTemplate> item6 in patterns[n])
								{
									if (item6.Item1 <= counts[n])
									{
										PackedTemplate item = item6.Item2;
										Template t2 = item.T;
										int num6 = num4 - item.First.X;
										int num7 = num5 - item.First.Y;
										if (num6 >= 0 && num7 >= 0 && num6 + t2.Width <= W && num7 + t2.Height <= H)
										{
											BigInteger bigInteger3 = item.Bits << num7 * W + num6;
											if (!((bigInteger3 & occupied) != 0L))
											{
												list.Add(new SearchChoice
												{
													Gi = n,
													Count = item6.Item1,
													X = num6,
													Y = num7,
													Offset = groups[n].Items.Length - counts[n],
													Pattern = item,
													Bits = bigInteger3,
													Anchor = anchors[n],
													Continuation = ((n != last) ? 1 : 0),
													Sequence = list.Count
												});
											}
										}
									}
								}
							}
						}
						list.Sort((Comparison<SearchChoice>)Compare);
						foreach (SearchChoice c2 in list)
						{
							counts[c2.Gi] -= c2.Count;
							path.Add(c2);
							foreach (SearchTick item7 in Dfs(occupied | c2.Bits, remainingCode - radix[c2.Gi] * c2.Count, area - groups[c2.Gi].Items[0].Area * c2.Count, holes, c2.Gi))
							{
								yield return item7;
							}
							path.RemoveAt(path.Count - 1);
							counts[c2.Gi] += c2.Count;
						}
						if (holes > 0)
						{
							foreach (SearchTick item8 in Dfs(occupied | (BigInteger.One << first), remainingCode, area, holes - 1, last))
							{
								yield return item8;
							}
						}
						if (priorSolutions == solutions && dead.Count < 60000)
						{
							dead.Add(state);
						}
					}
				}
			}
		}
		Placement[] Materialize(SearchChoice c)
		{
			Placement[] items = c.Pattern.T.Items;
			Placement[] array = new Placement[items.Length];
			for (int m = 0; m < items.Length; m++)
			{
				LayoutItem layoutItem = groups[c.Gi].Items[c.Offset + m];
				array[m] = At(layoutItem, items[m].X + c.X, items[m].Y + c.Y, _legalOrientations[layoutItem.StableId][items[m].Mask.Signature]);
			}
			return array;
		}
	}

	private void RunStage(string name, Action action)
	{
		long timestamp = Stopwatch.GetTimestamp();
		long allocatedBytesForCurrentThread = GC.GetAllocatedBytesForCurrentThread();
		_lastNodes = 0;
		action();
		_performance.Add(new StageTiming(_pass, name, (double)(Stopwatch.GetTimestamp() - timestamp) * 1000.0 / (double)Stopwatch.Frequency, GC.GetAllocatedBytesForCurrentThread() - allocatedBytesForCurrentThread, _lastNodes));
	}

	public static string Type(LayoutItem i)
	{
		if (i.ItemId.Length <= 0)
		{
			return i.StableId.Split('\u001f')[0];
		}
		return i.ItemId;
	}

	private string Type(Placement q)
	{
		if (!_docs.TryGetValue(q.StableId, out LayoutItem value))
		{
			return "~fixed:" + q.StableId;
		}
		return Type(value);
	}

	private bool Anchor(LayoutItem i)
	{
		if (i.Area < 12)
		{
			return Enumerable.Any(i.Categories.Append(i.PrimaryCategory), (string c) => Enumerable.Any(new string[7] { "container", "storage", "backpack", "bag", "machine", "device", "appliance" }, (string s) => c.Contains(s, StringComparison.OrdinalIgnoreCase)));
		}
		return true;
	}

	private static string ShapeKey(LayoutItem i)
	{
		return string.Join("/", Enumerable.OrderBy(Enumerable.Distinct(Enumerable.Select(i.Orientations, (Orientation o) => o.Mask.Signature)), (string s) => s, (IComparer<string>?)StringComparer.Ordinal));
	}

	public static string Key(IEnumerable<Placement> q)
	{
		return string.Join("|", Enumerable.Select(q.OrderBy((Placement a) => a.StableId, (IComparer<string>?)StringComparer.Ordinal), delegate(Placement a)
		{
			DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
			val = new DefaultInterpolatedStringHandler(4, 5);
			val.AppendFormatted(a.StableId);
			val.AppendLiteral("@");
			val.AppendFormatted<int>(a.X);
			val.AppendLiteral(",");
			val.AppendFormatted<int>(a.Y);
			val.AppendLiteral(",");
			val.AppendFormatted<int>(a.GameOrientation);
			val.AppendLiteral(":");
			val.AppendFormatted(a.Mask.Signature);
			return val.ToStringAndClear();
		}));
	}

	public static bool SameLayout(IEnumerable<Placement> a, IEnumerable<Placement> b)
	{
		return StringComparer.Ordinal.Equals(Key(a), Key(b));
	}

	private static string Hash(string s)
	{
		return HashCompat.Sha256Hex(Encoding.UTF8.GetBytes(s));
	}

	public static bool ValidateLayout(LayoutProblem p, IReadOnlyList<Placement> q)
	{
		try
		{
			if (q.Count != p.Items.Count || Enumerable.Count(Enumerable.Distinct(Enumerable.Select(q, (Placement x) => x.StableId), (IEqualityComparer<string>?)StringComparer.Ordinal)) != q.Count)
			{
				return false;
			}
			Dictionary<string, LayoutItem> docs = p.Items.ToDictionary((LayoutItem i) => i.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
			if (Enumerable.Any(p.Fixed, (Placement f) => docs.ContainsKey(f.StableId)) || Enumerable.Count(Enumerable.Distinct(Enumerable.Select(p.Fixed, (Placement f) => f.StableId))) != p.Fixed.Count)
			{
				return false;
			}
			bool[,] array = new bool[p.Width, p.Height];
			foreach (Placement a2 in p.Fixed.Concat(q))
			{
				if (docs.TryGetValue(a2.StableId, out var value))
				{
					if (!Enumerable.Any(value.Orientations, (Orientation o) => o.GameOrientation == a2.GameOrientation && o.Mask.Signature == a2.Mask.Signature) && (a2.GameOrientation != value.OriginalOrientation || value.OriginalMask == null || !(value.OriginalMask.Signature == a2.Mask.Signature)))
					{
						return false;
					}
					if (a2.Category != value.PrimaryCategory)
					{
						return false;
					}
				}
				else if (!Enumerable.Any(p.Fixed, (Placement f) => f == a2))
				{
					return false;
				}
				foreach (Cell cell in a2.Mask.Cells)
				{
					int num = a2.X + cell.X;
					int num2 = a2.Y + cell.Y;
					if (num < 0 || num2 < 0 || num >= p.Width || num2 >= p.Height || !p.Usable[num, num2] || array[num, num2])
					{
						return false;
					}
					array[num, num2] = true;
				}
			}
			return Enumerable.All(q, (Placement a) => docs.ContainsKey(a.StableId));
		}
		catch (Exception ex) when (ex is ArgumentException || ex is IndexOutOfRangeException || ex is NullReferenceException)
		{
			return false;
		}
	}

	private static Placement At(LayoutItem i, int x, int y, Orientation o)
	{
		return new Placement(i.StableId, x, y, o.GameOrientation, o.Mask, i.PrimaryCategory, o.Mask.Width > o.Mask.Height);
	}

	private static HashSet<Cell> Cells(IEnumerable<Placement> q)
	{
		return Enumerable.ToHashSet(Enumerable.SelectMany(q, (Placement a) => Enumerable.Select(a.Mask.Cells, (Cell c) => new Cell(a.X + c.X, a.Y + c.Y))));
	}

	private bool Fits(IEnumerable<Placement> q, int x, int y, HashSet<Cell> used)
	{
		HashSet<Cell> used2 = used;
		return Enumerable.All(q, (Placement a) => Enumerable.All(a.Mask.Cells, delegate(Cell c)
		{
			int num = x + a.X + c.X;
			int num2 = y + a.Y + c.Y;
			return num >= 0 && num2 >= 0 && num < _p.Width && num2 < _p.Height && _p.Usable[num, num2] && !used2.Contains(new Cell(num, num2));
		}));
	}

	private static Placement[] Move(IEnumerable<Placement> q, int x, int y)
	{
		return Enumerable.ToArray(Enumerable.Select(q, (Placement a) => a with
		{
			X = a.X + x,
			Y = a.Y + y
		}));
	}

	private Orientation[] Options(LayoutItem i, string style)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (_optionCache.TryGetValue(new ValueTuple<string, string>(i.StableId, style), out Orientation[] value))
		{
			return value;
		}
		Orientation[] array = Enumerable.ToArray(Enumerable.OrderBy(Enumerable.Select(i.Orientations.GroupBy((Orientation o) => o.Mask.Signature, (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Orientation> g) => Enumerable.First(Enumerable.OrderBy(g, (Orientation o) => o.GameOrientation))), (Orientation o) => o.Mask.Width > o.Mask.Height).ThenBy((Orientation o) => o.GameOrientation).ThenBy((Orientation o) => o.Mask.Signature, (IComparer<string>?)StringComparer.Ordinal));
		_optionCache[new ValueTuple<string, string>(i.StableId, style)] = array;
		return array;
	}

	private bool Interlocking(LayoutItem i)
	{
		if (_interlockCache.TryGetValue(i.StableId, out var value))
		{
			return value;
		}
		bool flag = InterlockingCore(i);
		_interlockCache[i.StableId] = flag;
		return flag;
	}

	private bool InterlockingCore(LayoutItem i)
	{
		Orientation[] array = Options(i, "portrait");
		Orientation[] array2 = Enumerable.ToArray(Enumerable.Where(array, (Orientation o) => o.Mask.Width <= o.Mask.Height));
		if (array2.Length != 0)
		{
			array = array2;
		}
		Orientation[] array3 = array;
		foreach (Orientation orientation in array3)
		{
			Orientation[] array4 = array;
			foreach (Orientation orientation2 in array4)
			{
				HashSet<Cell> ac = Enumerable.ToHashSet(orientation.Mask.Cells);
				int dx;
				for (dx = 0; dx < orientation.Mask.Width; dx++)
				{
					if (!Enumerable.Any(orientation2.Mask.Cells, (Cell c) => ac.Contains(new Cell(c.X + dx, c.Y))) && Math.Max(orientation.Mask.Width, dx + orientation2.Mask.Width) < orientation.Mask.Width + orientation2.Mask.Width)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private List<Template> InterlockRows(LayoutItem[] items, int beam = 6)
	{
		List<Template> list = new List<Template>
		{
			new Template(Array.Empty<Placement>(), 0, 0, 1, 0)
		};
		foreach (LayoutItem i2 in items)
		{
			Orientation[] array = Options(i2, "portrait");
			Orientation[] array2 = Enumerable.ToArray(Enumerable.Where(array, (Orientation o) => o.Mask.Width <= o.Mask.Height));
			if (array2.Length != 0)
			{
				array = array2;
			}
			List<Template> list2 = new List<Template>();
			foreach (Template item in list)
			{
				Orientation[] array3 = array;
				foreach (Orientation orientation in array3)
				{
					if (orientation.Mask.Height > _p.Height || orientation.Mask.Width > _p.Width)
					{
						continue;
					}
					HashSet<Cell> used = Cells(item.Items);
					int num = ((item.Items.Length != 0) ? item.Items[^1].X : 0);
					int x;
					for (x = num; x <= item.Width && x + orientation.Mask.Width <= _p.Width; x++)
					{
						Placement placement = At(i2, x, 0, orientation);
						if (!Enumerable.Any(placement.Mask.Cells, (Cell c) => used.Contains(new Cell(c.X + x, c.Y))))
						{
							list2.Add(new Template(Enumerable.ToArray(item.Items.Append(placement)), Math.Max(item.Width, x + orientation.Mask.Width), Math.Max(item.Height, orientation.Mask.Height), 1, item.Landscape + ((orientation.Mask.Width > orientation.Mask.Height) ? 1 : 0)));
							break;
						}
					}
				}
			}
			list = Enumerable.ToList(Enumerable.Take(Enumerable.OrderBy((IEnumerable<Template>)list2, (Template t) => t.Width).ThenBy((Template t) => t.Height).ThenBy((Template t) => Key(t.Items), (IComparer<string>?)StringComparer.Ordinal), beam));
			if (list.Count == 0)
			{
				break;
			}
		}
		return list;
	}

	private List<Template> Templates(LayoutItem[] members, string style)
	{
		string style2 = style;
		string key = string.Join("/", Enumerable.Select(members, (LayoutItem i) => i.StableId)) + ":" + style2;
		if (_templates.TryGetValue(key, out List<Template> value))
		{
			return value;
		}
		List<Template> list = new List<Template>();
		if (members.Length > 1 && Interlocking(members[0]))
		{
			list.AddRange((IEnumerable<Template>)InterlockRows(members));
			for (int num = members.Length - 1; num >= 1; num--)
			{
				List<Placement> list2 = new List<Placement>();
				int num2 = 0;
				int num3 = 0;
				bool flag = true;
				for (int j = 0; j < members.Length; j += num)
				{
					List<Template> list3 = InterlockRows(Enumerable.ToArray(Enumerable.Take(Enumerable.Skip(members, j), num)), 1);
					if (list3.Count == 0)
					{
						flag = false;
						break;
					}
					Template template = list3[0];
					list2.AddRange((IEnumerable<Placement>)Move(template.Items, 0, num2));
					num2 += template.Height;
					num3 = Math.Max(num3, template.Width);
				}
				if (flag && num2 <= _p.Height)
				{
					list.Add(new Template(list2.ToArray(), num3, num2, Enumerable.Count(Enumerable.Distinct(Enumerable.Select((IEnumerable<Placement>)list2, (Placement a) => a.Y))), Enumerable.Count((IEnumerable<Placement>)list2, (Placement a) => a.Mask.Width > a.Mask.Height)));
				}
			}
		}
		else
		{
			HashSet<string> hashSet = Enumerable.ToHashSet(Enumerable.Select(members[0].Orientations, (Orientation o) => o.Mask.Signature), (IEqualityComparer<string>?)StringComparer.Ordinal);
			foreach (LayoutItem layoutItem in members)
			{
				hashSet.IntersectWith(Enumerable.Select(layoutItem.Orientations, (Orientation o) => o.Mask.Signature));
			}
			foreach (string mask in Enumerable.OrderBy((IEnumerable<string>)hashSet, (string x) => x, (IComparer<string>?)StringComparer.Ordinal))
			{
				Orientation[] os = Enumerable.ToArray(Enumerable.Select(members, (LayoutItem i) => Enumerable.First(Options(i, style2), (Orientation o) => o.Mask.Signature == mask)));
				int w = os[0].Mask.Width;
				int h = os[0].Mask.Height;
				int per;
				for (per = Math.Min(_p.Width / w, members.Length); per >= 1; per--)
				{
					int num4 = (members.Length + per - 1) / per;
					if (num4 * h <= _p.Height)
					{
						Placement[] array = Enumerable.ToArray(Enumerable.Select(members, (LayoutItem i, int k) => At(i, k % per * w, k / per * h, os[k])));
						list.Add(new Template(array, Math.Min(per, members.Length) * w, num4 * h, num4, Enumerable.Count(array, (Placement q) => q.Mask.Width > q.Mask.Height)));
					}
				}
			}
		}
		list = Enumerable.ToList(Enumerable.OrderBy(Enumerable.Select(((IEnumerable<Template>)list).GroupBy((Template t) => Key(t.Items), (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Template> g) => Enumerable.First(g)), (Template t) => t.Rows).ThenBy((Template t) => t.Landscape).ThenBy((Template t) => t.Height)
			.ThenBy((Template t) => -t.Width)
			.ThenBy((Template t) => Key(t.Items), (IComparer<string>?)StringComparer.Ordinal));
		_templates[key] = list;
		return list;
	}

	private void Add(IEnumerable<Placement> q, string generator)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Placement[] array = Enumerable.ToArray(q.OrderBy((Placement a) => a.StableId, (IComparer<string>?)StringComparer.Ordinal));
		if (!ValidateLayout(_p, array))
		{
			throw new InvalidOperationException("Generated candidate violated immutable geometry: " + generator);
		}
		string key = Key(array);
		if (!_pool.TryGetValue(key, out ValueTuple<Placement[], SortedSet<string>> value))
		{
			value = new ValueTuple<Placement[], SortedSet<string>>(array, new SortedSet<string>((IComparer<string>)StringComparer.Ordinal));
		}
		value.Item2.Add(generator);
		_pool[key] = value;
	}

	private bool UnitTail(LayoutItem[] ms, List<Placement> placed)
	{
		LayoutItem[] ms2 = ms;
		HashSet<Cell> hashSet = Cells((IEnumerable<Placement>)placed);
		HashSet<Cell> hashSet2 = new HashSet<Cell>();
		for (int i = 0; i < _p.Height; i++)
		{
			for (int j = 0; j < _p.Width; j++)
			{
				if (_p.Usable[j, i] && !hashSet.Contains(new Cell(j, i)))
				{
					hashSet2.Add(new Cell(j, i));
				}
			}
		}
		HashSet<Cell> hashSet3 = Enumerable.FirstOrDefault(Enumerable.OrderByDescending(Enumerable.Where((IEnumerable<HashSet<Cell>>)Components(hashSet2), (HashSet<Cell> c) => c.Count >= ms2.Length), (HashSet<Cell> c) => c.Count).ThenBy((HashSet<Cell> c) => Enumerable.Min((IEnumerable<Cell>)c, (Cell z) => z.Y * _p.Width + z.X)));
		if (hashSet3 == null)
		{
			return false;
		}
		HashSet<Cell> hashSet4 = new HashSet<Cell> { Enumerable.First(Enumerable.OrderBy((IEnumerable<Cell>)hashSet3, (Cell c) => c.Y).ThenBy((Cell c) => c.X)) };
		HashSet<Cell> hashSet5 = new HashSet<Cell>();
		LayoutItem[] array = ms2;
		foreach (LayoutItem layoutItem in array)
		{
			Cell cell = Enumerable.First(Enumerable.OrderBy((IEnumerable<Cell>)hashSet4, (Cell c) => c.Y).ThenBy((Cell c) => c.X));
			hashSet4.Remove(cell);
			hashSet5.Add(cell);
			placed.Add(At(layoutItem, cell.X, cell.Y, Enumerable.First(Enumerable.OrderBy(layoutItem.Orientations, (Orientation o) => o.GameOrientation))));
			Cell[] array2 = Neighbours(cell);
			foreach (Cell item in array2)
			{
				if (hashSet3.Contains(item) && !hashSet5.Contains(item))
				{
					hashSet4.Add(item);
				}
			}
		}
		return true;
	}

	private void Shelf(string order, string style, string layout)
	{
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		Family[] array = OrderedFamilies(order, layout == "unit-tail");
		List<Placement> list = Enumerable.ToList(_p.Fixed);
		int num = 0;
		int num2 = -1;
		int num3 = 0;
		string text = null;
		Family[] array2 = array;
		foreach (Family family in array2)
		{
			LayoutItem[] items = family.Items;
			bool flag = Enumerable.Any(items, Anchor);
			if (layout == "unit-tail" && items[0].Area == 1 && Enumerable.Count((IEnumerable<Family>)_families, (Family g) => g.Items[0].Area == 1) == 1)
			{
				if (!UnitTail(items, list))
				{
					text = family.Key;
					break;
				}
				continue;
			}
			List<ValueTuple<int, int, int, int, int, int, Placement[]>> list2 = new List<ValueTuple<int, int, int, int, int, int, Placement[]>>();
			HashSet<Cell> used = Cells((IEnumerable<Placement>)list);
			foreach (Template item in Templates(items, style))
			{
				int landscape = item.Landscape;
				int num4 = ((!flag && !(layout != "shelf")) ? ((num2 >= 0 && item.Height == num3 && item.Rows == 1) ? num2 : num) : 0);
				bool flag2 = false;
				for (int j = num4; j + item.Height <= _p.Height; j++)
				{
					if (flag2)
					{
						break;
					}
					for (int k = 0; k + item.Width <= _p.Width; k++)
					{
						if (Fits(item.Items, k, j, used))
						{
							list2.Add(new ValueTuple<int, int, int, int, int, int, Placement[]>(item.Rows, landscape, j, k, item.Height, item.Width, Move(item.Items, k, j)));
							flag2 = true;
							break;
						}
					}
				}
			}
			if (list2.Count == 0)
			{
				text = family.Key;
				break;
			}
			ValueTuple<int, int, int, int, int, int, Placement[]> val = Enumerable.First(Enumerable.OrderBy((IEnumerable<ValueTuple<int, int, int, int, int, int, Placement[]>>)list2, (ValueTuple<int, int, int, int, int, int, Placement[]> t) => t.Item1).ThenBy((ValueTuple<int, int, int, int, int, int, Placement[]> t) => t.Item2).ThenBy((ValueTuple<int, int, int, int, int, int, Placement[]> t) => t.Item3)
				.ThenBy((ValueTuple<int, int, int, int, int, int, Placement[]> t) => t.Item4)
				.ThenBy((ValueTuple<int, int, int, int, int, int, Placement[]> t) => t.Item5)
				.ThenBy((ValueTuple<int, int, int, int, int, int, Placement[]> t) => t.Item6)
				.ThenBy((ValueTuple<int, int, int, int, int, int, Placement[]> t) => Key(t.Item7), (IComparer<string>?)StringComparer.Ordinal));
			list.AddRange((IEnumerable<Placement>)val.Item7);
			num = Math.Max(num, val.Item3 + val.Item5);
			if (!flag)
			{
				num2 = val.Item3;
				num3 = val.Item5;
			}
		}
		DefaultInterpolatedStringHandler val2 = default(DefaultInterpolatedStringHandler);
		val2 = new DefaultInterpolatedStringHandler(9, 3);
		val2.AppendLiteral("global:");
		val2.AppendFormatted(layout);
		val2.AppendLiteral(":");
		val2.AppendFormatted(order);
		val2.AppendLiteral(":");
		val2.AppendFormatted(style);
		string text2 = val2.ToStringAndClear();
		List<string> runs = _runs;
		val2 = new DefaultInterpolatedStringHandler(26, 4);
		val2.AppendFormatted(text2);
		val2.AppendLiteral(":complete=");
		val2.AppendFormatted<bool>(text == null);
		val2.AppendLiteral(":placed=");
		val2.AppendFormatted<int>(list.Count - _p.Fixed.Count);
		val2.AppendLiteral(":failed=");
		val2.AppendFormatted(text ?? "none");
		runs.Add(val2.ToStringAndClear());
		if (text == null)
		{
			Add(Enumerable.Where((IEnumerable<Placement>)list, (Placement q) => _docs.ContainsKey(q.StableId)), text2);
		}
	}

	private static Cell[] Neighbours(Cell c)
	{
		return new Cell[4]
		{
			new Cell(c.X - 1, c.Y),
			new Cell(c.X + 1, c.Y),
			new Cell(c.X, c.Y - 1),
			new Cell(c.X, c.Y + 1)
		};
	}

	private static List<HashSet<Cell>> Components(HashSet<Cell> input)
	{
		HashSet<Cell> hashSet = new HashSet<Cell>((IEnumerable<Cell>)input);
		List<HashSet<Cell>> list = new List<HashSet<Cell>>();
		while (hashSet.Count > 0)
		{
			Cell item = Enumerable.First(Enumerable.OrderBy((IEnumerable<Cell>)hashSet, (Cell c) => c.Y).ThenBy((Cell c) => c.X));
			hashSet.Remove(item);
			HashSet<Cell> hashSet2 = new HashSet<Cell> { item };
			Stack<Cell> stack = new Stack<Cell>();
			stack.Push(item);
			while (stack.Count > 0)
			{
				Cell[] array = Neighbours(stack.Pop());
				foreach (Cell item2 in array)
				{
					if (hashSet.Remove(item2))
					{
						hashSet2.Add(item2);
						stack.Push(item2);
					}
				}
			}
			list.Add(hashSet2);
		}
		return list;
	}

	private Metrics Measure(IReadOnlyList<Placement> movable)
	{
		string key = Key(movable);
		if (_metricCache.TryGetValue(key, out Metrics value))
		{
			return value;
		}
		Metrics metrics = MeasureCore(movable);
		_metricCache[key] = metrics;
		return metrics;
	}

	private Metrics MeasureCore(IReadOnlyList<Placement> movable)
	{
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		IReadOnlyList<Placement> movable2 = movable;
		Placement[] array = Enumerable.ToArray(_p.Fixed.Concat(movable2));
		List<GroupMetric> list = new List<GroupMetric>();
		foreach (IGrouping<string, Placement> item in Enumerable.OrderBy(movable2.GroupBy(Type, (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Placement> g) => g.Key, (IComparer<string>?)StringComparer.Ordinal))
		{
			Placement[] array2 = Enumerable.ToArray(item);
			List<HashSet<Cell>> list2 = Components(Cells(array2));
			int num = Enumerable.Max((IEnumerable<HashSet<Cell>>)list2, (HashSet<Cell> c) => c.Count);
			bool interlock = array2.Length > 1 && Interlocking(_docs[array2[0].StableId]);
			int mixed = Enumerable.Sum(array2.GroupBy((Placement q) => ShapeKey(_docs[q.StableId]), (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Placement> g) => (!Interlocking(_docs[Enumerable.First(g).StableId])) ? Math.Max(0, Enumerable.Count(Enumerable.Distinct(Enumerable.Select(g, (Placement q) => q.Mask.Signature), (IEqualityComparer<string>?)StringComparer.Ordinal)) - 1) : 0);
			list.Add(new GroupMetric(item.Key, array2.Length, list2.Count, Enumerable.Sum(array2, (Placement q) => q.Mask.Area) - num, Enumerable.Count(Enumerable.Distinct(Enumerable.Select(array2, (Placement q) => q.Y))), mixed, Enumerable.Count(array2, (Placement q) => q.Mask.Width <= q.Mask.Height), interlock, Enumerable.Min(array2, (Placement q) => q.X), Enumerable.Min(array2, (Placement q) => q.Y), Enumerable.Max(array2, (Placement q) => q.Y + q.Mask.Height), Enumerable.Max(array2, (Placement q) => q.Mask.Area)));
		}
		HashSet<Cell> hashSet = Cells(array);
		Dictionary<int, ValueTuple<int, int>> dictionary = Enumerable.ToDictionary(Enumerable.GroupBy((IEnumerable<Cell>)hashSet, (Cell c) => c.Y), (IGrouping<int, Cell> g) => g.Key, (IGrouping<int, Cell> g) => new ValueTuple<int, int>(Enumerable.Min(g, (Cell c) => c.X), Enumerable.Max(g, (Cell c) => c.X)));
		Dictionary<int, ValueTuple<int, int>> dictionary2 = Enumerable.ToDictionary(Enumerable.GroupBy((IEnumerable<Cell>)hashSet, (Cell c) => c.X), (IGrouping<int, Cell> g) => g.Key, (IGrouping<int, Cell> g) => new ValueTuple<int, int>(Enumerable.Min(g, (Cell c) => c.Y), Enumerable.Max(g, (Cell c) => c.Y)));
		HashSet<Cell> hashSet2 = new HashSet<Cell>();
		for (int i = 0; i < _p.Height; i++)
		{
			for (int j = 0; j < _p.Width; j++)
			{
				if (_p.Usable[j, i] && !hashSet.Contains(new Cell(j, i)) && ((dictionary.TryGetValue(i, out var value) && value.Item1 < j && j < value.Item2) || (dictionary2.TryGetValue(j, out var value2) && value2.Item1 < i && i < value2.Item2)))
				{
					hashSet2.Add(new Cell(j, i));
				}
			}
		}
		int num2 = 0;
		int num3 = 0;
		for (int k = 0; k < list.Count; k++)
		{
			for (int l = k + 1; l < list.Count; l++)
			{
				GroupMetric groupMetric = list[k];
				GroupMetric groupMetric2 = list[l];
				if (Math.Min(groupMetric.Bottom, groupMetric2.Bottom) > Math.Max(groupMetric.Top, groupMetric2.Top))
				{
					int top = groupMetric.Top;
					int bottom = groupMetric.Bottom;
					int top2 = groupMetric2.Top;
					int bottom2 = groupMetric2.Bottom;
					if (top != top2 || bottom != bottom2)
					{
						num2++;
					}
				}
				if ((groupMetric.ItemArea > groupMetric2.ItemArea && groupMetric.Top > groupMetric2.Top) || (groupMetric2.ItemArea > groupMetric.ItemArea && groupMetric2.Top > groupMetric.Top))
				{
					num3++;
				}
			}
		}
		int cohortRows = Enumerable.Sum(Enumerable.Where(Enumerable.GroupBy(Enumerable.Where(movable2, (Placement q) => q.Mask.Area >= 4), (Placement q) => new ValueTuple<string, string>(q.Category, q.Mask.Signature)), (IGrouping<ValueTuple<string, string>, Placement> g) => Enumerable.Count(Enumerable.Distinct(Enumerable.Select(g, Type), (IEqualityComparer<string>?)StringComparer.Ordinal)) > 1), (IGrouping<ValueTuple<string, string>, Placement> g) => Math.Max(0, Enumerable.Count(Enumerable.Distinct(Enumerable.Select(g, (Placement q) => q.Y))) - 1));
		double rowDebt = Enumerable.Sum((IEnumerable<GroupMetric>)list, (GroupMetric g) => (double)Math.Max(0, g.Rows - (int)Math.Ceiling((double)(g.Count * Enumerable.Min(_docs[Enumerable.First(movable2, (Placement q) => Type(q) == g.Id).StableId].Orientations, (Orientation o) => o.Mask.Width)) / (double)_p.Width)) / (double)g.Count);
		Placement[] source = Enumerable.ToArray(Enumerable.Where(movable2, (Placement q) => Anchor(_docs[q.StableId])));
		int anchorArea = Enumerable.Sum(source, (Placement q) => q.Mask.Area);
		int anchorBottom = Enumerable.Max(Enumerable.DefaultIfEmpty(Enumerable.Select(Enumerable.Where(source, (Placement q) => _interlockAnchors.Contains(q.StableId)), (Placement q) => q.Y + q.Mask.Height)));
		int anchorIntrusion = Enumerable.Sum(Enumerable.Where(movable2, (Placement q) => !Anchor(_docs[q.StableId])), (Placement q) => Enumerable.Count(q.Mask.Cells, (Cell c) => q.Y + c.Y < anchorBottom));
		return new Metrics(list.ToArray(), Enumerable.Sum((IEnumerable<GroupMetric>)list, (GroupMetric g) => Math.Max(0, g.Regions - 1)), Enumerable.Sum((IEnumerable<GroupMetric>)list, (GroupMetric g) => g.DetachedArea), Enumerable.Sum((IEnumerable<GroupMetric>)list, (GroupMetric g) => Math.Max(0, g.Rows - 1)), Enumerable.Sum((IEnumerable<GroupMetric>)list, (GroupMetric g) => g.Mixed), Enumerable.Count(movable2, (Placement q) => q.Mask.Width > q.Mask.Height), Enumerable.Sum(Enumerable.Where((IEnumerable<GroupMetric>)list, (GroupMetric g) => g.Interlock), (GroupMetric g) => g.Portrait), num2, Enumerable.Sum((IEnumerable<GroupMetric>)list, (GroupMetric g) => g.Left), num3, hashSet2.Count, LargestRectangle(hashSet2), (array.Length != 0) ? Enumerable.Max(array, (Placement q) => q.Y + q.Mask.Height) : 0, Enumerable.Sum(source, (Placement q) => (double)q.Mask.Area * (double)q.Y), anchorArea, Enumerable.Sum(movable2, (Placement q) => (double)q.Mask.Area * (double)(q.Y * _p.Width + q.X)), cohortRows, rowDebt, anchorIntrusion);
	}

	private int LargestRectangle(HashSet<Cell> cells)
	{
		int[] array = new int[_p.Width];
		int num = 0;
		for (int i = 0; i < _p.Height; i++)
		{
			for (int j = 0; j < _p.Width; j++)
			{
				array[j] = (cells.Contains(new Cell(j, i)) ? (array[j] + 1) : 0);
			}
			Stack<int> stack = new Stack<int>();
			for (int k = 0; k <= _p.Width; k++)
			{
				int num2 = ((k != _p.Width) ? array[k] : 0);
				while (stack.Count > 0 && array[stack.Peek()] > num2)
				{
					int num3 = stack.Pop();
					int num4 = ((stack.Count > 0) ? (k - stack.Peek() - 1) : k);
					num = Math.Max(num, array[num3] * num4);
				}
				stack.Push(k);
			}
		}
		return num;
	}

	private string Guard(Placement[] candidate)
	{
		if (!ValidateLayout(_p, candidate))
		{
			return "hard-validation";
		}
		return "";
	}

	private double Value(Metrics m)
	{
		int num = Math.Max(1, _p.Items.Count);
		int num2 = Math.Max(1, m.Groups.Length);
		int num3 = _p.Width * _p.Height;
		double num4 = Math.Max(1.0, (double)(num2 * (num2 - 1)) / 2.0);
		int num5 = Enumerable.Sum(_p.Items, (LayoutItem i) => i.Area);
		int num6 = 0;
		bool[,] usable = _p.Usable;
		int upperBound = usable.GetUpperBound(0);
		int upperBound2 = usable.GetUpperBound(1);
		for (int j = usable.GetLowerBound(0); j <= upperBound; j++)
		{
			for (int k = usable.GetLowerBound(1); k <= upperBound2; k++)
			{
				if (usable[j, k])
				{
					num6++;
				}
			}
		}
		double num7 = (double)(num5 + Enumerable.Sum(_p.Fixed, (Placement q) => q.Mask.Area)) / (double)Math.Max(1, num6);
		return 1000.0 * (double)m.Regions / (double)num + 200.0 * (double)m.Detached / (double)Math.Max(1, Enumerable.Sum(_p.Items, (LayoutItem i) => i.Area)) + 50.0 * m.RowDebt / (double)num2 + 30.0 * (double)m.Mixed / (double)num + 8.0 * (double)m.Landscape / (double)num + 6.0 * m.AnchorY / (double)Math.Max(1, m.AnchorArea) / (double)_p.Height + 2.0 * (double)m.Height / (double)_p.Height + 3.0 * (double)m.Middle / (double)num3 + 2.0 * (double)m.LargestMiddle / (double)num3 + 3.0 * (double)m.CohortRows / (double)num + 4.0 * (1.0 - num7) * (double)m.AnchorIntrusion / (double)Math.Max(1, num5 - m.AnchorArea) + 0.1 * (double)m.Overlaps / num4 + 0.1 * (double)m.Inversions / num4 + 0.1 * (double)m.Left / (double)(_p.Width * num2) + 0.1 * m.Priority / (double)Math.Max(1, Enumerable.Sum(_p.Items, (LayoutItem i) => i.Area)) / (double)num3 + 0.5 * (double)(num - m.PortraitInterlock) / (double)num;
	}

	private static string Summary(Metrics m)
	{
		DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
		val = new DefaultInterpolatedStringHandler(173, 15);
		val.AppendLiteral("regions=");
		val.AppendFormatted<int>(m.Regions);
		val.AppendLiteral(",detached=");
		val.AppendFormatted<int>(m.Detached);
		val.AppendLiteral(",extraRows=");
		val.AppendFormatted<int>(m.ExtraRows);
		val.AppendLiteral(",mixed=");
		val.AppendFormatted<int>(m.Mixed);
		val.AppendLiteral(",portraitInterlock=");
		val.AppendFormatted<int>(m.PortraitInterlock);
		val.AppendLiteral(",anchorY=");
		val.AppendFormatted<double>(m.AnchorY);
		val.AppendLiteral(",bandOverlaps=");
		val.AppendFormatted<int>(m.Overlaps);
		val.AppendLiteral(",familyLeft=");
		val.AppendFormatted<int>(m.Left);
		val.AppendLiteral(",largeInversions=");
		val.AppendFormatted<int>(m.Inversions);
		val.AppendLiteral(",middle=");
		val.AppendFormatted<int>(m.Middle);
		val.AppendLiteral(",middleRect=");
		val.AppendFormatted<int>(m.LargestMiddle);
		val.AppendLiteral(",height=");
		val.AppendFormatted<int>(m.Height);
		val.AppendLiteral(",cohortRows=");
		val.AppendFormatted<int>(m.CohortRows);
		val.AppendLiteral(",rowDebt=");
		val.AppendFormatted<double>(m.RowDebt, "F5");
		val.AppendLiteral(",anchorIntrusion=");
		val.AppendFormatted<int>(m.AnchorIntrusion);
		return val.ToStringAndClear();
	}

	public LayoutResult Solve(LayoutProblem p, IReadOnlyList<Placement>? original = null)
	{
		return CreateSession(p, original).Advance(NodeLimit).Result;
	}

	private PackedTemplate Pack(Template t)
	{
		if (_packed.TryGetValue(t, out PackedTemplate value))
		{
			return value;
		}
		HashSet<Cell> hashSet = Cells(t.Items);
		Cell first = Enumerable.First(Enumerable.OrderBy((IEnumerable<Cell>)hashSet, (Cell c) => c.Y).ThenBy((Cell c) => c.X));
		BigInteger bits = 0;
		foreach (Cell item in hashSet)
		{
			bits |= BigInteger.One << item.Y * _p.Width + item.X;
		}
		PackedTemplate packedTemplate = new PackedTemplate(t, bits, first);
		_packed[t] = packedTemplate;
		return packedTemplate;
	}

	private static int LowestBit(BigInteger value)
	{
		byte[] bytes = value.ToByteArray(isUnsigned: true);
		for (int i = 0; i < bytes.Length; i++)
		{
			if (bytes[i] == 0)
				continue;
			for (int bit = 0; bit < 8; bit++)
			{
				if ((bytes[i] & (1 << bit)) != 0)
					return i * 8 + bit;
			}
		}
		return -1;
	}

	private static int CountBits(BigInteger value)
	{
		int num = 0;
		byte[] array = value.ToByteArray(isUnsigned: true);
		foreach (byte b in array)
		{
			byte bits = b;
			while (bits != 0)
			{
				num += bits & 1;
				bits >>= 1;
			}
		}
		return num;
	}

	public IReadOnlyDictionary<string, double> FeatureVector(IReadOnlyList<Placement> movable)
	{
		Placement[] array = Enumerable.ToArray(_p.Fixed.Concat(movable));
		int W = _p.Width;
		int H = _p.Height;
		int num = W * H;
		int num2 = Math.Max(1, array.Length);
		int num3 = 0;
		bool[,] usable = _p.Usable;
		int upperBound = usable.GetUpperBound(0);
		int upperBound2 = usable.GetUpperBound(1);
		for (int i = usable.GetLowerBound(0); i <= upperBound; i++)
		{
			for (int j = usable.GetLowerBound(1); j <= upperBound2; j++)
			{
				if (usable[i, j])
				{
					num3++;
				}
			}
		}
		num3 = Math.Max(1, num3);
		Metrics metrics = Measure(movable);
		Dictionary<Cell, Placement> byCell = new Dictionary<Cell, Placement>();
		Placement[] array2 = array;
		foreach (Placement placement in array2)
		{
			foreach (Cell cell in placement.Mask.Cells)
			{
				byCell[new Cell(placement.X + cell.X, placement.Y + cell.Y)] = placement;
			}
		}
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		int num12 = 0;
		int num13 = 0;
		int num14 = 0;
		double num15 = 0.0;
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		SortedDictionary<int, HashSet<int>> sortedDictionary = new SortedDictionary<int, HashSet<int>>();
		IGrouping<string, Placement>[] array3 = Enumerable.ToArray(array.GroupBy(Type, (IEqualityComparer<string>?)StringComparer.Ordinal));
		array2 = array;
		foreach (Placement placement2 in array2)
		{
			if (!sortedDictionary.TryGetValue(placement2.Y, out var value))
			{
				value = (sortedDictionary[placement2.Y] = new HashSet<int>());
			}
			foreach (Cell cell2 in placement2.Mask.Cells)
			{
				value.Add(placement2.X + cell2.X);
			}
		}
		ValueTuple<int, int, int>[] array4 = Enumerable.ToArray(Enumerable.Select((IEnumerable<KeyValuePair<int, HashSet<int>>>)sortedDictionary, (KeyValuePair<int, HashSet<int>> kv) => new ValueTuple<int, int, int>(kv.Key, Enumerable.Min((IEnumerable<int>)kv.Value), Enumerable.Max((IEnumerable<int>)kv.Value) + 1)));
		for (int k = 1; k < array4.Length; k++)
		{
			num14 += Math.Abs(array4[k].Item2 - array4[k - 1].Item2) + Math.Abs(array4[k].Item3 - array4[k - 1].Item3);
		}
		foreach (KeyValuePair<Cell, Placement> item in byCell)
		{
			Cell[] array5 = new Cell[2]
			{
				new Cell(item.Key.X + 1, item.Key.Y),
				new Cell(item.Key.X, item.Key.Y + 1)
			};
			foreach (Cell key in array5)
			{
				if (byCell.TryGetValue(key, out Placement value2) && !(value2.StableId == item.Value.StableId))
				{
					Placement value3 = item.Value;
					if (Type(value3) == Type(value2))
					{
						num4++;
					}
					if (value3.Category == value2.Category)
					{
						num5++;
					}
					else
					{
						num6++;
					}
					if (Type(value3) != Type(value2) && !value3.Category.StartsWith('~') && value3.Category == value2.Category && value3.Y == value2.Y && value3.Mask.Signature == value2.Mask.Signature)
					{
						num7++;
					}
				}
			}
		}
		int y2;
		for (y2 = 0; y2 < H; y2++)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>((IEqualityComparer<string>)StringComparer.Ordinal);
			string text = null;
			List<int> list3 = new List<int>();
			for (int l = 0; l < W; l++)
			{
				Placement value4;
				string text2 = (byCell.TryGetValue(new Cell(l, y2), out value4) ? Type(value4) : "~empty");
				if (text2 != text)
				{
					dictionary[text2] = CollectionExtensions.GetValueOrDefault<string, int>((IReadOnlyDictionary<string, int>)dictionary, text2) + 1;
					text = text2;
				}
				if (value4 != null)
				{
					list3.Add(l);
				}
			}
			num8 += Enumerable.Sum(Enumerable.Where((IEnumerable<KeyValuePair<string, int>>)dictionary, (KeyValuePair<string, int> kv) => kv.Key != "~empty"), (KeyValuePair<string, int> kv) => Math.Max(0, kv.Value - 1));
			if (list3.Count > 0)
			{
				list.Add(list3[0]);
				list2.Add(list3[list3.Count - 1]);
				int num16 = Enumerable.Count(Enumerable.Range(list3[0], list3[list3.Count - 1] - list3[0] + 1), (int x) => _p.Usable[x, y2] && !byCell.ContainsKey(new Cell(x, y2)));
				num9 += num16;
				num15 += (double)(num16 * (H - y2)) / (double)H;
			}
		}
		IGrouping<string, Placement>[] array6 = array3;
		foreach (IGrouping<string, Placement> source in array6)
		{
			IGrouping<int, Placement>[] array7 = Enumerable.ToArray(Enumerable.OrderBy(Enumerable.GroupBy(source, (Placement q) => q.Y), (IGrouping<int, Placement> g) => g.Key));
			if (Enumerable.Count(source) > 1)
			{
				num10 += array7.Length - 1;
			}
			for (int m = 1; m < array7.Length; m++)
			{
				if (array7[m].Key != Enumerable.Max(array7[m - 1], (Placement q) => q.Y + q.Mask.Height))
				{
					num11++;
				}
			}
			foreach (IGrouping<int, Placement> item2 in Enumerable.GroupBy(Enumerable.Where(source, (Placement q) => q.Mask.Area >= 4 || q.Category == "MODULE"), (Placement q) => q.X))
			{
				Placement[] array8 = Enumerable.ToArray(Enumerable.OrderBy(item2, (Placement q) => q.Y).ThenBy((Placement q) => q.StableId, (IComparer<string>?)StringComparer.Ordinal));
				for (int n = 1; n < array8.Length; n++)
				{
					num12 += ((array8[n - 1].Mask.Width != array8[n].Mask.Width) ? 1 : 0) + ((array8[n].Y != array8[n - 1].Y + array8[n - 1].Mask.Height) ? 1 : 0);
				}
			}
		}
		Placement[] array9 = Enumerable.ToArray(Enumerable.Where(array, (Placement q) => q.Mask.Area >= 4 || q.Category == "MODULE"));
		array2 = array9;
		foreach (Placement a in array2)
		{
			Placement[] array10 = Enumerable.ToArray(Enumerable.Where(array9, (Placement b) => b.Y == a.Y + a.Mask.Height && b.Mask.Width == a.Mask.Width && Math.Max(a.X, b.X) < Math.Min(a.X + a.Mask.Width, b.X + b.Mask.Width)));
			if (array10.Length != 0)
			{
				num13 += Enumerable.Min(array10, (Placement b) => Math.Abs(a.X - b.X));
			}
		}
		int num17 = Enumerable.Sum(array3, (IGrouping<string, Placement> g) => Math.Max(0, InstanceComponents(Enumerable.ToArray(g)) - 1));
		int num18 = Enumerable.Sum(array.GroupBy((Placement q) => q.Category, (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Placement> g) => Math.Max(0, InstanceComponents(Enumerable.ToArray(g)) - 1));
		double value5 = Enumerable.Average(Enumerable.DefaultIfEmpty(Enumerable.Select(array.GroupBy((Placement q) => q.Category, (IEqualityComparer<string>?)StringComparer.Ordinal), (IGrouping<string, Placement> g) => (double)Enumerable.Sum(g, (Placement q) => q.Mask.Area) / (double)Math.Max(1, (Enumerable.Max(g, (Placement q) => q.X + q.Mask.Width) - Enumerable.Min(g, (Placement q) => q.X)) * (Enumerable.Max(g, (Placement q) => q.Y + q.Mask.Height) - Enumerable.Min(g, (Placement q) => q.Y))))));
		int num19 = Enumerable.Sum(array3, (IGrouping<string, Placement> g) => Enumerable.Max(Enumerable.GroupBy(g, (Placement q) => (q.Mask.Width <= q.Mask.Height) ? ((q.Mask.Width >= q.Mask.Height) ? "square" : "portrait") : "landscape"), (IGrouping<string, Placement> g) => Enumerable.Count(g)));
		HashSet<Cell> free = new HashSet<Cell>();
		for (int num20 = 0; num20 < H; num20++)
		{
			for (int num21 = 0; num21 < W; num21++)
			{
				if (_p.Usable[num21, num20] && !byCell.ContainsKey(new Cell(num21, num20)))
				{
					free.Add(new Cell(num21, num20));
				}
			}
		}
		int num22 = Enumerable.Sum(Enumerable.Where((IEnumerable<HashSet<Cell>>)Components(free), (HashSet<Cell> c) => !Enumerable.Any((IEnumerable<Cell>)c, (Cell z) => z.X == 0 || z.Y == 0 || z.X == W - 1 || z.Y == H - 1)), (HashSet<Cell> c) => c.Count);
		int num23 = 0;
		int y;
		for (y = H - 1; y >= 0 && Enumerable.All(Enumerable.Range(0, W), (int x) => free.Contains(new Cell(x, y))); y--)
		{
			num23 += W;
		}
		HashSet<Cell> hashSet2 = new HashSet<Cell>();
		for (int num24 = 0; num24 < W; num24++)
		{
			int num25 = H - 1;
			while (num25 >= 0 && free.Contains(new Cell(num24, num25)))
			{
				hashSet2.Add(new Cell(num24, num25));
				num25--;
			}
		}
		int num26 = LargestRectangle(hashSet2);
		double num27 = 0.0;
		for (int num28 = 1; num28 < list.Count; num28++)
		{
			num27 += (double)(Math.Abs(list[num28] - list[num28 - 1]) + Math.Abs(list2[num28] - list2[num28 - 1]));
		}
		int num29 = Math.Max(1, Enumerable.Sum(array, (Placement q) => q.Mask.Area));
		Placement[] source2 = Enumerable.ToArray(Enumerable.Where(movable, (Placement q) => _docs[q.StableId].VisualPriority < 100));
		int num30 = Math.Max(1, Enumerable.Sum(source2, (Placement q) => q.Mask.Area));
		return (IReadOnlyDictionary<string, double>)new Dictionary<string, double>((IEqualityComparer<string>)StringComparer.Ordinal)
		{
			["typeConnectivity"] = (double)(-num17) / (double)num2,
			["typeRowContinuity"] = (double)(-num10) / (double)num2,
			["adjacentWrapContinuity"] = (double)(-num11) / (double)num2,
			["middleSpace"] = (double)(-metrics.Middle) / (double)num3,
			["largestMiddleSpace"] = (double)(-metrics.LargestMiddle) / (double)num3,
			["columnSeamAlignment"] = (double)(-num13) / (double)(W * num2),
			["rowBoundaryAlignment"] = (double)(-num14) / (double)(2 * W * Math.Max(1, sortedDictionary.Count - 1)),
			["columnContinuity"] = (double)(-num12) / (double)num2,
			["categoryConnectivity"] = (double)(-num18) / (double)num2,
			["categoryCompactness"] = value5,
			["categorySeparation"] = (double)(-num6) / (double)num,
			["typeContact"] = (double)num4 / (double)num,
			["categoryContact"] = (double)num5 / (double)num,
			["sameShapeCohortRowContact"] = (double)num7 / (double)num3,
			["rowRunContinuity"] = (double)(-num8) / (double)num2,
			["directionUniformity"] = (double)num19 / (double)num2,
			["uprightPreference"] = (double)(-Enumerable.Count(array, (Placement q) => q.Mask.Width > q.Mask.Height)) / (double)num2,
			["enclosedSpace"] = (double)(-num22) / (double)num,
			["bottomBlank"] = (double)num23 / (double)num,
			["bottomEmptyRectangle"] = (double)num26 / (double)num,
			["interiorRowContinuity"] = (double)(-num9) / (double)num,
			["upperRowContinuity"] = (0.0 - num15) / (double)num,
			["contourAlignment"] = (0.0 - num27) / (double)num,
			["areaTopPriority"] = (0.0 - Enumerable.Sum(array, (Placement q) => (double)q.Mask.Area * (double)(q.Y * W + q.X))) / (double)num / (double)num29,
			["anchorTopPriority"] = (0.0 - Enumerable.Sum(source2, (Placement q) => (double)q.Mask.Area * (double)(q.Y * W + q.X))) / (double)num / (double)num30
		};
	}

	private static int InstanceComponents(Placement[] items)
	{
		if (items.Length == 0)
		{
			return 0;
		}
		int[] parent = Enumerable.ToArray(Enumerable.Range(0, items.Length));
		Dictionary<Cell, int> dictionary = new Dictionary<Cell, int>();
		for (int j = 0; j < items.Length; j++)
		{
			foreach (Cell cell in items[j].Mask.Cells)
			{
				dictionary[new Cell(items[j].X + cell.X, items[j].Y + cell.Y)] = j;
			}
		}
		foreach (KeyValuePair<Cell, int> item in dictionary)
		{
			Cell[] array = Neighbours(item.Key);
			foreach (Cell key in array)
			{
				if (dictionary.TryGetValue(key, out var value))
				{
					int num = Find(item.Value);
					int num2 = Find(value);
					if (num != num2)
					{
						parent[num2] = num;
					}
				}
			}
		}
		return Enumerable.Count(Enumerable.Distinct(Enumerable.Select(Enumerable.Range(0, items.Length), Find)));
		int Find(int i)
		{
			while (parent[i] != i)
			{
				parent[i] = parent[parent[i]];
				i = parent[i];
			}
			return i;
		}
	}
}
