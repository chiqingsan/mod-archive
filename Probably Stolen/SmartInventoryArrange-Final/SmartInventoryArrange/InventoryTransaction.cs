using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SmartInventoryArrange.Core;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SmartInventoryArrange;

internal sealed class InventoryTransaction
{
	private sealed record Snap(GameItem Item, string StableId, GridShape OriginalShape, string Fingerprint, LayoutItem Model);


	private static readonly ConditionalWeakTable<GameItem, FingerprintCacheEntry> FingerprintCache = new ConditionalWeakTable<GameItem, FingerprintCacheEntry>();

	private static int _fingerprintAuditsLeft = 64;

	private static int _fingerprintAuditMismatches;

	private sealed class FingerprintCacheEntry
	{
		internal ulong Signature;

		internal string Value = "";
	}

	internal sealed class CaptureResult
	{
		internal InventoryTransaction? Transaction;
	}

	private const int ChunkThreshold = 16;

	private const double MaxFrameBudgetMs = 6.0;

	private const double MinFrameBudgetMs = 1.0;

	private const double FrameBudgetShare = 0.35;

	private static double _frameBudgetMs = MaxFrameBudgetMs;

	private static double ResolveFrameBudgetMs()
	{
		double frameMs = Time.deltaTime * 1000.0;
		if (frameMs <= 0.0 || frameMs > 100.0)
		{
			frameMs = 16.6667;
		}
		return Math.Min(MaxFrameBudgetMs, Math.Max(MinFrameBudgetMs, frameMs * FrameBudgetShare));
	}

	private static bool OverBudget(long frameStart)
	{
		return PerfTrace.ElapsedMs(frameStart) >= _frameBudgetMs;
	}

	private readonly GameGridInventory _inventory;

	private readonly InventoryObservation _capturedObservation;

	private readonly bool _keepContainers;

	private readonly IReadOnlyDictionary<IntPtr, bool> _capturedWindowPresence;

	private readonly InstanceOrderInfo[] _instanceOrder;

	private double _instanceOrderingMs;

	private string _instanceOrderingNotes = "";

	private readonly List<Snap> _movable;

	private readonly List<ValueTuple<GameItem, GridShape>> _allOriginal;

	private readonly int _originX;

	private readonly int _originY;



	public LayoutProblem Problem { get; }

	public IReadOnlyList<Placement> OriginalPlacements { get; }

	public int PreservedContainerCount { get; }

	public int Width => Problem.Width;

	public int Height => Problem.Height;

	public int MovableCount => _movable.Count;

	public int FixedCount => Problem.Fixed.Count;

	public int CategoryCount => Enumerable.Count(Enumerable.Distinct(Enumerable.Select(Problem.Items, (LayoutItem i) => i.PrimaryCategory), (IEqualityComparer<string>?)StringComparer.Ordinal));

	public int MultiCategoryCount => Enumerable.Count(Problem.Items, (LayoutItem i) => i.Categories.Count > 1);

	public int UnknownCategoryCount => Enumerable.Count(Problem.Items, (LayoutItem i) => i.PrimaryCategory == "~unknown");

	internal GameGridInventory Inventory => _inventory;

	internal string DescribeLayoutChange(IReadOnlyList<Placement> candidate, int maxItems)
	{
		Dictionary<string, Placement> dictionary = new Dictionary<string, Placement>(StringComparer.Ordinal);
		foreach (Placement item in candidate)
		{
			dictionary[item.StableId] = item;
		}
		Dictionary<string, Snap> dictionary2 = new Dictionary<string, Snap>(StringComparer.Ordinal);
		foreach (Snap item2 in _movable)
		{
			dictionary2[item2.StableId] = item2;
		}
		StringBuilder builder = new StringBuilder();
		int num = 0;
		foreach (Placement originalPlacement in OriginalPlacements)
		{
			if (!dictionary.TryGetValue(originalPlacement.StableId, out Placement value))
			{
				continue;
			}
			if (num >= maxItems)
			{
				builder.Append("...");
				break;
			}
			if (num > 0)
			{
				builder.Append(';');
			}
			string text = (dictionary2.TryGetValue(originalPlacement.StableId, out Snap value2) ? (value2.Item.identifier + "#" + value2.Item.uniqueId) : originalPlacement.StableId);
			builder.Append(text).Append(':');
			builder.Append(originalPlacement.X).Append(',').Append(originalPlacement.Y).Append(',').Append(originalPlacement.GameOrientation);
			builder.Append("->");
			builder.Append(value.X).Append(',').Append(value.Y).Append(',').Append(value.GameOrientation);
			num++;
		}
		return builder.ToString();
	}

	private InventoryTransaction(GameGridInventory inventory, List<Snap> movable, List<ValueTuple<GameItem, GridShape>> allOriginal, LayoutProblem problem, IReadOnlyList<Placement> originals, int originX, int originY, InventoryObservation capturedObservation, InstanceOrderInfo[] instanceOrder, bool keepContainers, IReadOnlyDictionary<IntPtr, bool> windowPresence, int preservedContainerCount)
	{
		_inventory = inventory;
		_capturedObservation = capturedObservation;
		_keepContainers = keepContainers;
		_capturedWindowPresence = windowPresence;
		PreservedContainerCount = preservedContainerCount;
		_instanceOrder = instanceOrder;
		_movable = movable;
		_allOriginal = allOriginal;
		Problem = problem;
		OriginalPlacements = originals;
		_originX = originX;
		_originY = originY;
	}

	internal string BuildSessionContext()
	{
		return HashCompat.Sha256Hex(JsonSerializer.SerializeToUtf8Bytes(new
		{
			inventory = ReferenceIdentity.Pointer(_inventory).ToString(),
			originX = _originX,
			originY = _originY,
			keepContainers = _keepContainers,
			windows = Enumerable.Select(Enumerable.OrderBy((IEnumerable<KeyValuePair<IntPtr, bool>>)_capturedWindowPresence, (KeyValuePair<IntPtr, bool> x) => x.Key.ToInt64()), (KeyValuePair<IntPtr, bool> x) => new
			{
				pointer = x.Key.ToString(),
				exists = x.Value
			}),
			instances = Enumerable.OrderBy(Enumerable.Select((IEnumerable<ValueTuple<GameItem, GridShape>>)_allOriginal, (ValueTuple<GameItem, GridShape> x) => ReferenceIdentity.Pointer(x.Item1).ToString()), (string x) => x, (IComparer<string>?)StringComparer.Ordinal),
			movableIdentity = Enumerable.OrderBy(Enumerable.Select((IEnumerable<Snap>)_movable, (Snap x) => new
			{
				StableId = x.StableId,
				pointer = ReferenceIdentity.Pointer(x.Item).ToString()
			}), x => x.StableId, (IComparer<string>?)StringComparer.Ordinal)
		}));
	}

	internal static IEnumerator CaptureCoroutine(GameGridInventory inventory, bool keepContainers, CaptureResult result)
	{
		bool chunked = false;
		long frameStart = PerfTrace.Mark();
		int progressStep = 250;
		double passOneMs = 0.0;
		double itemsMs = 0.0;
		double layoutMs = 0.0;
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		if (!InventorySortHelper.MaySort(inventory) || !InventorySortHelper.CanSort(inventory))
		{
			throw new InvalidOperationException("Game reports this inventory cannot be sorted.");
		}
		GridShape container = inventory.inventoryShape ?? throw new InvalidOperationException("Inventory has no grid shape.");
		int width = container.width;
		int height = container.height;
		bool[,] array = new bool[width, height];
		for (int j = 0; j < height; j++)
		{
			for (int k = 0; k < width; k++)
			{
				array[k, j] = container.Get(k + container.minX, j + container.minY) == 0;
			}
		}
		Dictionary<IntPtr, string> fingerprints = new Dictionary<IntPtr, string>();
		List<SnapshotItem> list2 = new List<SnapshotItem>();
		long geometryStarted = PerfTrace.Mark();
		string geometry = Geometry(inventory);
		PerfTrace.Add("captureGeometry", geometryStarted);
		List<Snap> list3 = new List<Snap>();
		List<Placement> list4 = new List<Placement>();
		List<ValueTuple<GameItem, GridShape>> list5 = new List<ValueTuple<GameItem, GridShape>>();
		List<ValueTuple<GameItem, string, GridShape, List<string>, bool>> list6 = new List<ValueTuple<GameItem, string, GridShape, List<string>, bool>>();
		List<GameItem> childItems = new List<GameItem>(((GameInventory)inventory).childItems);
		_frameBudgetMs = ResolveFrameBudgetMs();
		chunked = childItems.Count > ChunkThreshold;
		progressStep = Math.Max(250, childItems.Count / 8);
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>((IEqualityComparer<string>)StringComparer.Ordinal);
		Dictionary<IntPtr, bool> windowPresence = new Dictionary<IntPtr, bool>();
		int preservedContainerCount = 0;
		long passOneStarted = PerfTrace.Mark();
		for (int l = 0; l < childItems.Count; l++)
		{
			GameItem val = childItems[l];
			if (val != null && val.modifiedShape != null)
			{
				string text = val.identifier + "\u001f" + val.uniqueId.ToString("D10", CultureInfo.InvariantCulture);
				dictionary2[text] = CollectionExtensions.GetValueOrDefault<string, int>((IReadOnlyDictionary<string, int>)dictionary2, text) + 1;
			}
			if (chunked && OverBudget(frameStart))
			{
				passOneMs += PerfTrace.ElapsedMs(passOneStarted);
				passOneStarted = PerfTrace.Mark();
				PerfTrace.Increment("captureFrames");
				yield return null;
				frameStart = PerfTrace.Mark();
			}
		}
		passOneMs += PerfTrace.ElapsedMs(passOneStarted);
		PerfTrace.AddMs("capturePass1", passOneMs);
		long itemsStarted = PerfTrace.Mark();
		for (int m = 0; m < childItems.Count; m++)
		{
			GameItem val3 = childItems[m];
			if (val3 == null || val3.modifiedShape == null)
			{
				throw new InvalidOperationException("Snapshot capture: missing item or shape at child=" + m + ".");
			}
			string text2 = StableId(val3, m, (IReadOnlyDictionary<string, int>)dictionary2);
			List<string> list7 = Categories(val3);
			int num2;
			if (!ItemPinHelper.IsPinned(val3) && !DebrisHelper.IsDebris(val3) && val3.MayRemove())
			{
				GameInventory parentInventory = val3.parentInventory;
				num2 = (SnapshotConsistency.SameNativeObject((parentInventory != null) ? ReferenceIdentity.Pointer(parentInventory) : IntPtr.Zero, ReferenceIdentity.Pointer(inventory)) ? 1 : 0);
			}
			else
			{
				num2 = 0;
			}
			bool flag = (byte)num2 != 0;
			if (keepContainers)
			{
				bool hasContentWindow = val3.contentWindow != null;
				windowPresence[ReferenceIdentity.Pointer(val3)] = hasContentWindow;
				if (hasContentWindow || string.Equals(val3.identifier, "powerblock", StringComparison.Ordinal))
				{
					if (flag)
					{
						preservedContainerCount++;
					}
					flag = false;
				}
			}
			SnapshotItem snapshotItem = Observe(val3, text2, flag);
			list2.Add(snapshotItem);
			fingerprints[ReferenceIdentity.Pointer(val3)] = snapshotItem.State;
			GridShape val4 = val3.modifiedShape.Clone();
			list5.Add(new ValueTuple<GameItem, GridShape>(val3, val4));
			list6.Add(new ValueTuple<GameItem, string, GridShape, List<string>, bool>(val3, text2, val4, list7, flag));
			if (chunked && OverBudget(frameStart))
			{
				itemsMs += PerfTrace.ElapsedMs(itemsStarted);
				itemsStarted = PerfTrace.Mark();
				PerfTrace.Increment("captureFrames");
				yield return null;
				frameStart = PerfTrace.Mark();
				if (m % progressStep == 0)
				{
					Debug.Log("Smart arrange capture: " + m + "/" + childItems.Count);
				}
			}
		}
		itemsMs += PerfTrace.ElapsedMs(itemsStarted);
		PerfTrace.AddMs("captureItems", itemsMs);
		long layoutStarted = PerfTrace.Mark();
		Dictionary<string, int> dictionary3 = Enumerable.ToDictionary(Enumerable.Select(Enumerable.OrderBy(Enumerable.Distinct(Enumerable.SelectMany((IEnumerable<ValueTuple<GameItem, string, GridShape, List<string>, bool>>)list6, (ValueTuple<GameItem, string, GridShape, List<string>, bool> r) => (IEnumerable<string>)r.Item4), (IEqualityComparer<string>?)StringComparer.Ordinal), CategoryRank).ThenBy((string x) => x, (IComparer<string>?)StringComparer.Ordinal), (string c, int i) => new ValueTuple<string, int>(c, i)), (ValueTuple<string, int> x) => x.Item1, (ValueTuple<string, int> x) => x.Item2, (IEqualityComparer<string>?)StringComparer.Ordinal);
		if (!dictionary3.ContainsKey("~unknown"))
		{
			dictionary3["~unknown"] = dictionary3.Count;
		}
		foreach (ValueTuple<GameItem, string, GridShape, List<string>, bool> item in Enumerable.OrderBy((IEnumerable<ValueTuple<GameItem, string, GridShape, List<string>, bool>>)list6, (ValueTuple<GameItem, string, GridShape, List<string>, bool> x) => x.Item2, (IComparer<string>?)StringComparer.Ordinal))
		{
			string primaryCategory = Enumerable.FirstOrDefault(Enumerable.OrderBy((IEnumerable<string>)item.Item4, CategoryRank).ThenBy((string x) => x, (IComparer<string>?)StringComparer.Ordinal)) ?? "~unknown";
			IReadOnlyList<Orientation> orientations = SupplyCrateOrientationPolicy.Allowed((IReadOnlyList<string>)item.Item4, BuildOrientations(item.Item3));
			LayoutItem layoutItem = new LayoutItem
			{
				StableId = item.Item2,
				ItemId = item.Item1.identifier,
				PrimaryCategory = primaryCategory,
				Categories = (IReadOnlyList<string>)item.Item4,
				Orientations = orientations,
				OriginalMask = Mask(item.Item3),
				OriginalOrientation = item.Item3.orientation,
				OriginalX = item.Item3.minX - container.minX,
				OriginalY = item.Item3.minY - container.minY,
				VisualPriority = VisualPriority(primaryCategory)
			};
			if (item.Item5)
			{
				list3.Add(new Snap(item.Item1, item.Item2, item.Item3, fingerprints[ReferenceIdentity.Pointer(item.Item1)], layoutItem));
			}
			else
			{
				list4.Add(new Placement(item.Item2, item.Item3.minX - container.minX, item.Item3.minY - container.minY, item.Item3.orientation, Mask(item.Item3), "~fixed", Rotated: false));
			}
			if (chunked && OverBudget(frameStart))
			{
				layoutMs += PerfTrace.ElapsedMs(layoutStarted);
				layoutStarted = PerfTrace.Mark();
				PerfTrace.Increment("captureFrames");
				yield return null;
				frameStart = PerfTrace.Mark();
			}
		}
		layoutMs += PerfTrace.ElapsedMs(layoutStarted);
		PerfTrace.AddMs("captureLayout", layoutMs);
		Placement[] originals = Enumerable.ToArray(Enumerable.Select((IEnumerable<Snap>)list3, delegate(Snap s)
		{
			ShapeMask shapeMask2 = s.Model.OriginalMask;
			return new Placement(s.StableId, s.OriginalShape.minX - container.minX, s.OriginalShape.minY - container.minY, s.OriginalShape.orientation, shapeMask2, s.Model.PrimaryCategory, shapeMask2.Width > shapeMask2.Height);
		}));
		LayoutProblem layoutProblem = new LayoutProblem
		{
			Width = width,
			Height = height,
			Usable = array,
			Fixed = (IReadOnlyList<Placement>)list4,
			Items = Enumerable.ToArray(Enumerable.Select((IEnumerable<Snap>)list3, (Snap s) => s.Model)),
			CategoryOrder = (IReadOnlyDictionary<string, int>)dictionary3
		};
		InventoryObservation capturedObservation = new InventoryObservation(ReferenceIdentity.Pointer(inventory), geometry, Array.AsReadOnly(list2.ToArray()));
		long instanceOrderStarted = PerfTrace.Mark();
		double instanceOrderMs = 0.0;
		InstanceOrderInfo[] instanceOrder = new InstanceOrderInfo[list3.Count];
		for (int q = 0; q < list3.Count; q++)
		{
			Snap snap = list3[q];
			instanceOrder[q] = InstanceOrderNative.ReadCached(snap.Item, snap.StableId);
			if (chunked && OverBudget(frameStart))
			{
				instanceOrderMs += PerfTrace.ElapsedMs(instanceOrderStarted);
				instanceOrderStarted = PerfTrace.Mark();
				PerfTrace.Increment("captureFrames");
				yield return null;
				frameStart = PerfTrace.Mark();
			}
		}
		instanceOrderMs += PerfTrace.ElapsedMs(instanceOrderStarted);
		PerfTrace.AddMs("captureInstanceOrder", instanceOrderMs);
		long constructorStarted = PerfTrace.Mark();
		InventoryTransaction inventoryTransaction = new InventoryTransaction(inventory, list3, list5, layoutProblem, originals, container.minX, container.minY, capturedObservation, instanceOrder, keepContainers, windowPresence, preservedContainerCount);
		PerfTrace.Add("captureConstructor", constructorStarted);
		result.Transaction = inventoryTransaction;
	}

	public LayoutResult OrderInstances(LayoutResult result)
	{
		if (!result.Complete || result.UsedFallback || _instanceOrder.Length < 2)
		{
			return result;
		}
		Stopwatch val = Stopwatch.StartNew();
		Dictionary<string, InstanceOrderInfo> metadata = _instanceOrder.ToDictionary((InstanceOrderInfo i) => i.StableId, (IEqualityComparer<string>?)StringComparer.Ordinal);
		_instanceOrderingNotes = string.Join(";", Enumerable.OrderBy(Enumerable.Select(Enumerable.Where(Enumerable.GroupBy(Problem.Items, (LayoutItem i) => new ValueTuple<string, string>(i.ItemId, InstanceOrder.ShapeClass(i))), (IGrouping<ValueTuple<string, string>, LayoutItem> g) => Enumerable.Any(Enumerable.Skip(Enumerable.Distinct(Enumerable.Where(Enumerable.Select(g, (LayoutItem i) => metadata[i.StableId].Profile), (string x) => x != "name"), (IEqualityComparer<string>?)StringComparer.Ordinal), 1))), (IGrouping<ValueTuple<string, string>, LayoutItem> g) => g.Key.Item1 + ":profile-conflict-name-only"), (string x) => x, (IComparer<string>?)StringComparer.Ordinal));
		IReadOnlyList<Placement> readOnlyList = InstanceOrder.Apply(Problem, result.Placements, _instanceOrder);
		LayoutScore score = ((readOnlyList == result.Placements) ? result.Score : DeterministicSolver.Evaluate(Problem, readOnlyList));
		val.Stop();
		_instanceOrderingMs = val.Elapsed.TotalMilliseconds;
		return result with
		{
			Placements = readOnlyList,
			Score = score,
			Elapsed = result.Elapsed + val.Elapsed
		};
	}

	private static string Geometry(GameGridInventory inventory)
	{
		GridShape val = inventory.inventoryShape ?? throw new InvalidOperationException("Snapshot guard: container shape missing.");
		StringBuilder stringBuilder = new StringBuilder().Append(val.minX).Append(',').Append(val.minY)
			.Append(',')
			.Append(val.width)
			.Append(',')
			.Append(val.height)
			.Append(':');
		for (int i = 0; i < val.height; i++)
		{
			for (int j = 0; j < val.width; j++)
			{
				stringBuilder.Append((val.Get(j + val.minX, i + val.minY) == 0) ? '1' : '0');
			}
		}
		return stringBuilder.ToString();
	}

	private static SnapshotItem Observe(GameItem item, string stableId, bool movable)
	{
		GridShape modifiedShape = item.modifiedShape;
		SnapshotPose pose = ((modifiedShape == null) ? null : new SnapshotPose(modifiedShape.minX, modifiedShape.minY, modifiedShape.orientation, modifiedShape.globalWidth, modifiedShape.globalHeight, Mask(modifiedShape).Signature));
		IntPtr pointer = ReferenceIdentity.Pointer(item);
		GameInventory parentInventory = item.parentInventory;
		return new SnapshotItem(pointer, (parentInventory != null) ? ReferenceIdentity.Pointer(parentInventory) : IntPtr.Zero, stableId, movable, Fingerprint(item), pose);
	}

	public IEnumerator AssertSnapshotCurrentCoroutine(string stage = "before-apply")
	{
		Dictionary<IntPtr, SnapshotItem> captured = Enumerable.ToDictionary(_capturedObservation.Items, (SnapshotItem x) => x.Pointer);
		List<SnapshotItem> current = new List<SnapshotItem>(_capturedObservation.Items.Count);
		int aliases = 0;
		List<GameItem> childItems = ((GameInventory)_inventory).childItems;
		_frameBudgetMs = ResolveFrameBudgetMs();
		bool chunked = childItems.Count > ChunkThreshold;
		long frameStart = PerfTrace.Mark();
		for (int i = 0; i < childItems.Count; i++)
		{
			GameItem item = childItems[i];
			if (item == null)
			{
				current.Add(new SnapshotItem(0, 0, "child-" + i, false, "null", null));
			}
			else
			{
				captured.TryGetValue(ReferenceIdentity.Pointer(item), out SnapshotItem? value);
				if (_keepContainers && value != null && _capturedWindowPresence.TryGetValue(ReferenceIdentity.Pointer(item), out bool wasOpen) && (item.contentWindow != null) != wasOpen)
				{
					throw new InvalidOperationException("Snapshot guard rejected stage=" + stage + "; contentWindow presence changed for " + item.identifier + ".");
				}
				GameInventory parentInventory = item.parentInventory;
				if (parentInventory != null && parentInventory != _inventory && SnapshotConsistency.SameNativeObject(ReferenceIdentity.Pointer(parentInventory), ReferenceIdentity.Pointer(_inventory)))
				{
					aliases++;
				}
				current.Add(Observe(item, value?.StableId ?? item.identifier, value?.Movable ?? false));
			}
			if (chunked && OverBudget(frameStart))
			{
				PerfTrace.Increment("captureFrames");
				yield return null;
				frameStart = PerfTrace.Mark();
			}
		}
		SnapshotConsistency.AssertSame(_capturedObservation, new InventoryObservation(ReferenceIdentity.Pointer(_inventory), Geometry(_inventory), Array.AsReadOnly(current.ToArray())), stage);
		Debug.Log("Smart arrange snapshot guard passed: stage=" + stage + ", members=" + current.Count + ", nativeWrapperAliases=" + aliases + ", changedFields=0");
	}

	private void AssertFixedSnapshotCurrent()
	{
		List<GameItem> childItems = ((GameInventory)_inventory).childItems;
		Dictionary<IntPtr, GameItem> members = new Dictionary<IntPtr, GameItem>();
		for (int i = 0; i < childItems.Count; i++)
		{
			GameItem val = childItems[i] ?? throw new InvalidOperationException("Post-apply inventory member missing.");
			members.Add(ReferenceIdentity.Pointer(val), val);
		}
		if (members.Count != _capturedObservation.Items.Count || Enumerable.Any(_capturedObservation.Items, (SnapshotItem s) => !members.ContainsKey(s.Pointer)))
		{
			throw new InvalidOperationException("Post-apply inventory membership changed.");
		}
		ReadOnlyCollection<SnapshotItem> readOnlyCollection = Array.AsReadOnly(Enumerable.ToArray(Enumerable.Where(_capturedObservation.Items, (SnapshotItem s) => !s.Movable)));
		ReadOnlyCollection<SnapshotItem> items = Array.AsReadOnly(Enumerable.ToArray(Enumerable.Select(readOnlyCollection, (SnapshotItem s) => Observe(members[s.Pointer], s.StableId, movable: false))));
		SnapshotConsistency.AssertSame(new InventoryObservation(ReferenceIdentity.Pointer(_inventory), _capturedObservation.Geometry, readOnlyCollection), new InventoryObservation(ReferenceIdentity.Pointer(_inventory), Geometry(_inventory), items), "after-apply-fixed");
	}

	public void RestoreCapturedSnapshot()
	{
		Rollback();
	}

	public void Apply(IReadOnlyList<Placement> placements)
	{
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		if (placements.Count != _movable.Count || Enumerable.Count(Enumerable.Distinct(Enumerable.Select(placements, (Placement p) => p.StableId), (IEqualityComparer<string>?)StringComparer.Ordinal)) != _movable.Count)
		{
			throw new InvalidOperationException("Candidate does not contain every movable item exactly once.");
		}
		long verifyBeforeStarted = PerfTrace.Mark();
		VerifyUnchanged();
		PerfTrace.Add("applyVerifyBefore", verifyBeforeStarted);
		long shapePrecheckStarted = PerfTrace.Mark();
		List<ValueTuple<Snap, GridShape, Placement>> list = new List<ValueTuple<Snap, GridShape, Placement>>();
		List<GameItem> val = new List<GameItem>();
		foreach (Snap item in _movable)
		{
			val.Add(item.Item);
		}
		DefaultInterpolatedStringHandler val3;
		foreach (Snap s in ((IEnumerable<Snap>)_movable).OrderBy((Snap x) => x.StableId, (IComparer<string>?)StringComparer.Ordinal))
		{
			Placement placement = Enumerable.Single(placements, (Placement x) => x.StableId == s.StableId);
			GridShape val2 = new GridShapeBuilder(s.OriginalShape).SetOrientation(placement.GameOrientation).SetPosition(placement.X + _originX, placement.Y + _originY).Build();
			if (!((GameInventory)_inventory).CheckShapePlacement(s.Item, val2, val))
			{
				val3 = new DefaultInterpolatedStringHandler(52, 4);
				val3.AppendLiteral("Game collision precheck rejected ");
				val3.AppendFormatted(s.StableId);
				val3.AppendLiteral(" at ");
				val3.AppendFormatted<int>(placement.X);
				val3.AppendLiteral(",");
				val3.AppendFormatted<int>(placement.Y);
				val3.AppendLiteral(" orientation ");
				val3.AppendFormatted<int>(placement.GameOrientation);
				val3.AppendLiteral(".");
				throw new InvalidOperationException(val3.ToStringAndClear());
			}
			list.Add(new ValueTuple<Snap, GridShape, Placement>(s, val2, placement));
		}
		PerfTrace.Add("applyShapePrecheck", shapePrecheckStarted);
		try
		{
			long writeShapesStarted = PerfTrace.Mark();
			foreach (ValueTuple<Snap, GridShape, Placement> item2 in list)
			{
				item2.Item1.Item.modifiedShape = item2.Item2;
			}
			PerfTrace.Add("applyWriteShapes", writeShapesStarted);
			long slotCheckStarted = PerfTrace.Mark();
			foreach (ValueTuple<Snap, GridShape, Placement> item3 in list)
			{
				if (!((GameInventory)_inventory).CheckInventorySlotStillValid(item3.Item1.Item, item3.Item2, item3.Item1.Item.modifiedState))
				{
					string slotChecks = DescribeSlotValidation(_inventory, item3.Item1.Item, item3.Item2, item3.Item1.Item.modifiedState);
					Debug.LogWarning("Smart arrange slot check warning: " + item3.Item1.StableId + " at " + item3.Item3.X + "," + item3.Item3.Y + " orientation " + item3.Item3.GameOrientation + "; checks=" + slotChecks + "; applying anyway (game sort does not run this check).");
				}
			}
			PerfTrace.Add("applySlotCheck", slotCheckStarted);
			long validateStarted = PerfTrace.Mark();
			((GameInventory)_inventory).Validate();
			PerfTrace.Add("applyValidate", validateStarted);
			long backgroundStarted = PerfTrace.Mark();
			_inventory.ForceRecomputeDynamicBackground();
			PerfTrace.Add("applyBackground", backgroundStarted);
			long verifyAfterStarted = PerfTrace.Mark();
			VerifyUnchanged(ignoreShape: true);
			AssertFixedSnapshotCurrent();
			PerfTrace.Add("applyVerifyAfter", verifyAfterStarted);
			long postShapeStarted = PerfTrace.Mark();
			foreach (ValueTuple<Snap, GridShape, Placement> item4 in list)
			{
				if (item4.Item1.Item.modifiedShape.minX != item4.Item2.minX || item4.Item1.Item.modifiedShape.minY != item4.Item2.minY || item4.Item1.Item.modifiedShape.orientation != item4.Item2.orientation)
				{
					throw new InvalidOperationException("Post-apply shape verification failed for " + item4.Item1.Item.identifier);
				}
			}
			PerfTrace.Add("applyPostShapeCheck", postShapeStarted);
		}
		catch
		{
			Rollback();
			throw;
		}
	}

	private static string DescribeSlotValidation(GameGridInventory inventory, GameItem item, GridShape shape, TagSystem state)
	{
		string result;
		try
		{
			SlotMarker slot = new SlotMarker();
			slot.item = item;
			slot.inventory = inventory;
			slot.itemGridShape = shape;
			result = "insertLocked=" + inventory.IsInsertLocked()
				+ ", mayHaveValid=" + inventory.MayHaveValidInventorySlot(item)
				+ ", stateValid=" + item.CheckIsStateValid(inventory, slot, shape, state)
				+ ", parentModifier=" + inventory.CheckChangeParentShapeStateModifier(item, slot, shape, state)
				+ ", maxNumAdd=" + item.MaxNumAdd(slot);
		}
		catch (Exception ex)
		{
			result = "subCheckFailed=" + ex.GetType().Name + ":" + ex.Message;
		}
		return result;
	}

	private void Rollback()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		foreach (ValueTuple<GameItem, GridShape> item in _allOriginal)
		{
			item.Item1.modifiedShape = item.Item2.Clone();
		}
		((GameInventory)_inventory).Validate();
		_inventory.ForceRecomputeDynamicBackground();
		foreach (ValueTuple<GameItem, GridShape> item2 in _allOriginal)
		{
			if (item2.Item1.modifiedShape.minX != item2.Item2.minX || item2.Item1.modifiedShape.minY != item2.Item2.minY || item2.Item1.modifiedShape.orientation != item2.Item2.orientation)
			{
				Debug.LogError("Rollback verification failed for " + item2.Item1.identifier);
			}
		}
		Debug.LogWarning("Smart arrange transaction rolled back to the captured layout.");
	}

	private void VerifyUnchanged(bool ignoreShape = false)
	{
		if (((GameInventory)_inventory).childItems.Count != _allOriginal.Count)
		{
			throw new InvalidOperationException("Inventory item count changed during arrangement.");
		}
		foreach (Snap item in _movable)
		{
			if (ItemPinHelper.IsPinned(item.Item) || DebrisHelper.IsDebris(item.Item))
			{
				throw new InvalidOperationException("Item became fixed during arrangement: " + item.StableId);
			}
			if (Fingerprint(item.Item) != item.Fingerprint)
			{
				throw new InvalidOperationException("Item state changed during arrangement: " + item.StableId);
			}
		}
	}

	private static string StableId(GameItem item, int index, IReadOnlyDictionary<string, int> counts)
	{
		DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
		val = new DefaultInterpolatedStringHandler(1, 2);
		val.AppendFormatted(item.identifier);
		val.AppendLiteral("\u001f");
		val.AppendFormatted<int>(item.uniqueId, "D10");
		string key = val.ToStringAndClear();
		return InstanceIdentity.From(item.identifier, item.uniqueId, index, counts[key]);
	}

	private static string Fingerprint(GameItem i)
	{
		long started = PerfTrace.Mark();
		ulong signature = InstanceOrderNative.Signature(i);
		if (FingerprintCache.TryGetValue(i, out FingerprintCacheEntry? cached) && cached != null && cached.Signature == signature && cached.Value.Length != 0)
		{
			PerfTrace.Increment("fingerprintHits");
			if (_fingerprintAuditsLeft > 0)
			{
				_fingerprintAuditsLeft--;
				PerfTrace.Increment("fingerprintAudits");
				long auditStarted = PerfTrace.Mark();
				string auditValue = FingerprintSlow(i);
				PerfTrace.Add("fingerprintAudit", auditStarted);
				if (!string.Equals(auditValue, cached.Value, StringComparison.Ordinal))
				{
					_fingerprintAuditMismatches++;
					PerfTrace.Increment("fingerprintAuditMismatches");
					Debug.LogError("Smart arrange fingerprint audit mismatch: " + i.identifier + "#" + i.uniqueId + " signature=" + signature.ToString("X16", CultureInfo.InvariantCulture));
					FingerprintCache.Remove(i);
					PerfTrace.Add("fingerprint", started);
					PerfTrace.Increment("fingerprintCalls");
					return auditValue;
				}
				if (_fingerprintAuditsLeft == 0)
				{
					Debug.Log("Smart arrange fingerprint audit: checked=64 mismatches=" + _fingerprintAuditMismatches);
				}
			}
			PerfTrace.Add("fingerprint", started);
			PerfTrace.Increment("fingerprintCalls");
			return cached.Value;
		}
		string value = FingerprintSlow(i);
		FingerprintCache.Remove(i);
		FingerprintCache.Add(i, new FingerprintCacheEntry
		{
			Signature = signature,
			Value = value
		});
		PerfTrace.Increment("fingerprintMisses");
		PerfTrace.Add("fingerprint", started);
		PerfTrace.Increment("fingerprintCalls");
		return value;
	}

	private static string FingerprintSlow(GameItem i)
	{
		DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
		val = new DefaultInterpolatedStringHandler(3, 4);
		val.AppendFormatted(i.identifier);
		val.AppendLiteral("|");
		val.AppendFormatted<int>(i.uniqueId);
		val.AppendLiteral("|");
		val.AppendFormatted<int>(i.unitCount);
		val.AppendLiteral("|");
		val.AppendFormatted<TagSystem>(i.state);
		return val.ToStringAndClear() + "|instance-order=" + InstanceOrderNative.FingerprintSuffix(i);
	}

	private static List<string> Categories(GameItem item)
	{
		List<string> list = new List<string>();
		try
		{
			List<string> itemTypes = item.itemTypes;
			if (itemTypes != null)
			{
				for (int i = 0; i < itemTypes.Count; i++)
				{
					string text = itemTypes[i];
					if (!string.IsNullOrWhiteSpace(text))
					{
						list.Add(text.Trim());
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Item type read failed for " + item.identifier + ": " + ex.Message);
		}
		if (list.Count == 0)
		{
			list.Add("~unknown");
		}
		return Enumerable.ToList(Enumerable.Distinct((IEnumerable<string>)list, (IEqualityComparer<string>?)StringComparer.Ordinal));
	}

	private static int VisualPriority(string primaryCategory)
	{
		int num = CategoryRank(primaryCategory);
		if (num > 3)
		{
			return 100 + num;
		}
		return 10 + num;
	}

	private static int CategoryRank(string category)
	{
		if (category == "~unknown")
		{
			return 3;
		}
		string text = category.ToLowerInvariant();
		string[] array = new string[20]
		{
			"container", "storage", "bag", "backpack", "machine", "device", "equipment", "appliance", "weapon", "ammo",
			"armor", "medical", "med", "food", "drink", "tool", "material", "component", "valuable", "misc"
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (text.Contains(array[i], StringComparison.Ordinal))
			{
				if (i <= 1)
				{
					return 0;
				}
				if (i <= 3)
				{
					return 1;
				}
				if (i <= 7)
				{
					return 2;
				}
				return 10 + i;
			}
		}
		return 1000;
	}

	private static IReadOnlyList<Orientation> BuildOrientations(GridShape shape)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		List<Orientation> list = new List<Orientation>();
		HashSet<string> hashSet = new HashSet<string>((IEqualityComparer<string>)StringComparer.Ordinal);
		int[] obj = new int[5] { 0, 1, 2, 3, 0 };
		obj[4] = shape.orientation;
		foreach (int item in Enumerable.Distinct(obj))
		{
			ShapeMask shapeMask = ReadBuiltMask(new GridShapeBuilder(shape).SetOrientation(item).SetPosition(0, 0).Build());
			if (hashSet.Add(shapeMask.Signature))
			{
				list.Add(new Orientation(item, shapeMask, shapeMask.Width > shapeMask.Height));
			}
		}
		return Enumerable.ToArray(Enumerable.OrderByDescending((IEnumerable<Orientation>)list, (Orientation x) => x.Mask.Height > x.Mask.Width).ThenByDescending((Orientation x) => x.Mask.Height - x.Mask.Width).ThenBy((Orientation x) => x.GameOrientation));
	}

	private static ShapeMask ReadBuiltMask(GridShape shape)
	{
		List<Cell> list = new List<Cell>();
		for (int i = 0; i < shape.globalHeight; i++)
		{
			for (int j = 0; j < shape.globalWidth; j++)
			{
				if (shape.Get(shape.minX + j, shape.minY + i) != 0)
				{
					list.Add(new Cell(j, i));
				}
			}
		}
		if (list.Count == 0)
		{
			throw new InvalidOperationException("Item has an empty collision mask.");
		}
		return new ShapeMask((IEnumerable<Cell>)list);
	}

	private static ShapeMask Mask(GridShape shape)
	{
		return ReadBuiltMask(shape);
	}
}
