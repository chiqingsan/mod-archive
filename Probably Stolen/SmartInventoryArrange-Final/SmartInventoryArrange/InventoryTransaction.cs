using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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

	private sealed record DiagnosticItem(string StableId, string Name, string ItemId, int Width, int Height, string Type, string[] Categories, int Orientation, bool CanRotate, bool Movable, int OriginalX, int OriginalY);

	private readonly GameGridInventory _inventory;

	private readonly bool _keepContainers;

	private readonly IReadOnlyDictionary<IntPtr, bool> _capturedWindowPresence;

	private readonly InventoryObservation _capturedObservation;

	private readonly InstanceOrderInfo[] _instanceOrder;

	private double _instanceOrderingMs;

	private string _instanceOrderingNotes = "";

	private readonly List<Snap> _movable;

	private readonly List<ValueTuple<GameItem, GridShape>> _allOriginal;

	private readonly int _originX;

	private readonly int _originY;

	private readonly IReadOnlyList<DiagnosticItem> _diagnostics;

	private readonly IReadOnlyList<ValueTuple<LayoutItem, bool>> _capturedModels;

	public int PreservedContainerCount { get; }

	public string SessionContext { get; }

	public LayoutProblem Problem { get; }

	public IReadOnlyList<Placement> OriginalPlacements { get; }

	public int Width => Problem.Width;

	public int Height => Problem.Height;

	public int MovableCount => _movable.Count;

	public int FixedCount => Problem.Fixed.Count;

	public int CategoryCount => Enumerable.Count(Enumerable.Distinct(Enumerable.Select(Problem.Items, (LayoutItem i) => i.PrimaryCategory), (IEqualityComparer<string>?)StringComparer.Ordinal));

	public int MultiCategoryCount => Enumerable.Count(Problem.Items, (LayoutItem i) => i.Categories.Count > 1);

	public int UnknownCategoryCount => Enumerable.Count(Problem.Items, (LayoutItem i) => i.PrimaryCategory == "~unknown");

	private InventoryTransaction(GameGridInventory inventory, List<Snap> movable, List<ValueTuple<GameItem, GridShape>> allOriginal, LayoutProblem problem, IReadOnlyList<Placement> originals, int originX, int originY, IReadOnlyList<DiagnosticItem> diagnostics, IReadOnlyList<ValueTuple<LayoutItem, bool>> capturedModels, InventoryObservation capturedObservation, bool keepContainers, IReadOnlyDictionary<IntPtr, bool> windowPresence, int preservedContainerCount)
	{
		_inventory = inventory;
		_capturedObservation = capturedObservation;
		_keepContainers = keepContainers;
		_capturedWindowPresence = windowPresence;
		PreservedContainerCount = preservedContainerCount;
		_instanceOrder = Enumerable.ToArray(Enumerable.Select((IEnumerable<Snap>)movable, (Snap s) => InstanceOrderNative.Read(s.Item, s.StableId)));
		_movable = movable;
		_allOriginal = allOriginal;
		Problem = problem;
		OriginalPlacements = originals;
		_originX = originX;
		_originY = originY;
		_diagnostics = diagnostics;
		_capturedModels = capturedModels;
		SessionContext = HashCompat.Sha256Hex(JsonSerializer.SerializeToUtf8Bytes(new
		{
			inventory = ReferenceIdentity.Pointer(inventory).ToString(),
			originX = originX,
			originY = originY,
			keepContainers = keepContainers,
			windows = Enumerable.Select(Enumerable.OrderBy(windowPresence, (KeyValuePair<IntPtr, bool> x) => x.Key.ToInt64()), (KeyValuePair<IntPtr, bool> x) => new
			{
				pointer = x.Key.ToString(),
				exists = x.Value
			}),
			instances = Enumerable.OrderBy(Enumerable.Select((IEnumerable<ValueTuple<GameItem, GridShape>>)allOriginal, (ValueTuple<GameItem, GridShape> x) => new
			{
				pointer = ReferenceIdentity.Pointer(x.Item1).ToString(),
				state = Fingerprint(x.Item1)
			}), x => x.pointer, (IComparer<string>?)StringComparer.Ordinal),
			movableIdentity = Enumerable.OrderBy(Enumerable.Select((IEnumerable<Snap>)movable, (Snap x) => new
			{
				StableId = x.StableId,
				pointer = ReferenceIdentity.Pointer(x.Item).ToString()
			}), x => x.StableId, (IComparer<string>?)StringComparer.Ordinal)
		}));
	}

	public static InventoryTransaction Capture(GameGridInventory inventory, bool keepContainers = false)
	{
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
		Dictionary<IntPtr, bool> dictionary = new Dictionary<IntPtr, bool>();
		int num = 0;
		List<ValueTuple<LayoutItem, bool>> list = new List<ValueTuple<LayoutItem, bool>>();
		List<SnapshotItem> list2 = new List<SnapshotItem>();
		string geometry = Geometry(inventory);
		List<Snap> list3 = new List<Snap>();
		List<Placement> list4 = new List<Placement>();
		List<ValueTuple<GameItem, GridShape>> list5 = new List<ValueTuple<GameItem, GridShape>>();
		List<ValueTuple<GameItem, string, GridShape, List<string>, bool>> list6 = new List<ValueTuple<GameItem, string, GridShape, List<string>, bool>>();
		List<GameItem> childItems = ((GameInventory)inventory).childItems;
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>((IEqualityComparer<string>)StringComparer.Ordinal);
		DefaultInterpolatedStringHandler val2 = default(DefaultInterpolatedStringHandler);
		for (int l = 0; l < childItems.Count; l++)
		{
			GameItem val = childItems[l];
			if (val != null && val.modifiedShape != null)
			{
				val2 = new DefaultInterpolatedStringHandler(1, 2);
				val2.AppendFormatted(val.identifier);
				val2.AppendLiteral("\u001f");
				val2.AppendFormatted<int>(val.uniqueId, "D10");
				string text = val2.ToStringAndClear();
				dictionary2[text] = CollectionExtensions.GetValueOrDefault<string, int>((IReadOnlyDictionary<string, int>)dictionary2, text) + 1;
			}
		}
		for (int m = 0; m < childItems.Count; m++)
		{
			GameItem val3 = childItems[m];
			if (val3 == null || val3.modifiedShape == null)
			{
				val2 = new DefaultInterpolatedStringHandler(50, 1);
				val2.AppendLiteral("Snapshot capture: missing item or shape at child=");
				val2.AppendFormatted<int>(m);
				val2.AppendLiteral(".");
				throw new InvalidOperationException(val2.ToStringAndClear());
			}
			string text2 = StableId(val3, m, (IReadOnlyDictionary<string, int>)dictionary2);
			List<string> list7 = Categories(val3);
			int num2;
			if (val3.MayRemove())
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
				bool flag2 = val3.contentWindow != null;
				dictionary.Add(ReferenceIdentity.Pointer(val3), flag2);
				if (flag2 || string.Equals(val3.identifier, "powerblock", StringComparison.Ordinal))
				{
					if (flag)
					{
						num++;
					}
					flag = false;
				}
			}
			list2.Add(Observe(val3, text2, flag));
			GridShape val4 = val3.modifiedShape.Clone();
			list5.Add(new ValueTuple<GameItem, GridShape>(val3, val4));
			list6.Add(new ValueTuple<GameItem, string, GridShape, List<string>, bool>(val3, text2, val4, list7, flag));
		}
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
			list.Add(new ValueTuple<LayoutItem, bool>(layoutItem, item.Item5));
			if (item.Item5)
			{
				list3.Add(new Snap(item.Item1, item.Item2, item.Item3, Fingerprint(item.Item1), layoutItem));
			}
			else
			{
				list4.Add(new Placement(item.Item2, item.Item3.minX - container.minX, item.Item3.minY - container.minY, item.Item3.orientation, Mask(item.Item3), "~fixed", Rotated: false));
			}
		}
		Placement[] originals = Enumerable.ToArray(Enumerable.Select((IEnumerable<Snap>)list3, delegate(Snap s)
		{
			ShapeMask shapeMask2 = Mask(s.OriginalShape);
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
		DiagnosticItem[] diagnostics = Enumerable.ToArray(Enumerable.Select(Enumerable.OrderBy((IEnumerable<ValueTuple<GameItem, string, GridShape, List<string>, bool>>)list6, (ValueTuple<GameItem, string, GridShape, List<string>, bool> x) => x.Item2, (IComparer<string>?)StringComparer.Ordinal), delegate(ValueTuple<GameItem, string, GridShape, List<string>, bool> x)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			string type = Enumerable.FirstOrDefault(Enumerable.OrderBy((IEnumerable<string>)x.Item4, CategoryRank).ThenBy((string v) => v, (IComparer<string>?)StringComparer.Ordinal)) ?? "~unknown";
			ShapeMask shapeMask = Mask(x.Item3);
			return new DiagnosticItem(x.Item2, x.Item1.GetDisplayName(true) ?? "", x.Item1.identifier, shapeMask.Width, shapeMask.Height, type, x.Item4.ToArray(), x.Item3.orientation, SupplyCrateOrientationPolicy.Allowed((IReadOnlyList<string>)x.Item4, BuildOrientations(x.Item3)).Count > 1, x.Item5, x.Item3.minX - container.minX, x.Item3.minY - container.minY);
		}));
		DeterministicSolver.ValidateProblem(layoutProblem);
		InventoryObservation capturedObservation = new InventoryObservation(ReferenceIdentity.Pointer(inventory), geometry, Array.AsReadOnly(list2.ToArray()));
		InventoryTransaction inventoryTransaction = new InventoryTransaction(inventory, list3, list5, layoutProblem, originals, container.minX, container.minY, diagnostics, (IReadOnlyList<ValueTuple<LayoutItem, bool>>)list, capturedObservation, keepContainers, (IReadOnlyDictionary<IntPtr, bool>)dictionary, num);
		inventoryTransaction.AssertSnapshotCurrent("capture-complete");
		return inventoryTransaction;
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

	public string WriteExactInput()
	{
		string text = Path.Combine(Application.persistentDataPath, "SmartInventoryArrange", "Inputs");
		System.IO.Directory.CreateDirectory(text);
		string text2 = Path.Combine(text, DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + "-" + Guid.NewGuid().ToString("N") + ".json");
		Dictionary<string, string> state = ((IEnumerable<Snap>)_movable).ToDictionary((Snap s) => s.StableId, (Snap s) => HashCompat.Sha256Hex(Encoding.UTF8.GetBytes(s.Fingerprint)), (IEqualityComparer<string>?)StringComparer.Ordinal);
		List<int[]> list = new List<int[]>();
		for (int j = 0; j < Height; j++)
		{
			for (int k = 0; k < Width; k++)
			{
				if (!Problem.Usable[k, j])
				{
					list.Add(new int[2] { k, j });
				}
			}
		}
		var value = new
		{
			schemaVersion = 2,
			version = "3.17.9",
			capturedAt = DateTimeOffset.Now,
			keepContainers = _keepContainers,
			preservedContainers = PreservedContainerCount,
			instanceOrderPolicy = "semantic-slots-v3:real-state-appraisal-ignored;name;empty-last;usable-sealed-rustfree;wine-2-1-0-age-desc-ferment-asc;water-grade-then-volume;food-fresh-first;absolute-remaining-desc;quality-before-quantity;stable-id",
			instanceOrder = _instanceOrder,
			wineOrder = Enumerable.Select(Enumerable.Where(_instanceOrder, (InstanceOrderInfo i) => i.ItemId == "wine_bottle"), (InstanceOrderInfo i) => new WineBottleInfo(i.StableId, i.CurrentName, i.WineState, i.AgeDays)),
			provenance = new
			{
				nativeMasks = true,
				nativeStableIds = true,
				nativeStateVerified = false
			},
			@case = new
			{
				input = new
				{
					width = Width,
					height = Height,
					disabled = list,
					items = Enumerable.Select(_capturedModels, (ValueTuple<LayoutItem, bool> s) => new
					{
						stableId = s.Item1.StableId,
						itemId = s.Item1.ItemId,
						category = (s.Item2 ? s.Item1.PrimaryCategory : "~fixed"),
						categories = s.Item1.Categories,
						priority = s.Item1.VisualPriority,
						@fixed = !s.Item2,
						orientations = Enumerable.Select(s.Item1.Orientations, (Orientation o) => new
						{
							orientation = o.GameOrientation,
							cells = Enumerable.Select(o.Mask.Cells, (Cell c) => new int[2] { c.X, c.Y })
						})
					})
				},
				baseline = Enumerable.Select(_capturedModels, (ValueTuple<LayoutItem, bool> s) => new
				{
					stableId = s.Item1.StableId,
					itemId = s.Item1.ItemId,
					x = s.Item1.OriginalX,
					y = s.Item1.OriginalY,
					orientation = s.Item1.OriginalOrientation,
					category = (s.Item2 ? s.Item1.PrimaryCategory : "~fixed"),
					@fixed = !s.Item2,
					stateToken = CollectionExtensions.GetValueOrDefault<string, string>((IReadOnlyDictionary<string, string>)state, s.Item1.StableId, "fixed-snapshot"),
					cells = Enumerable.Select(s.Item1.OriginalMask.Cells, (Cell c) => new
					{
						x = c.X,
						y = c.Y
					})
				})
			}
		};
		File.WriteAllText(text2, JsonSerializer.Serialize(value), (Encoding)(object)new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
		return text2;
	}

	public string WriteDiagnosticSnapshot()
	{
		string text = Path.Combine(Application.persistentDataPath, "SmartInventoryArrange", "Logs");
		System.IO.Directory.CreateDirectory(text);
		string text2 = Path.Combine(text, "SmartInventoryArrange-Snapshot.log");
		StringBuilder stringBuilder = new StringBuilder(512 + _diagnostics.Count * 160);
		stringBuilder.AppendLine("========== Smart Arrange Snapshot ==========");
		stringBuilder.Append("Time: ").AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
		stringBuilder.Append("ContainerSize: ").Append(Width).Append('x')
			.AppendLine(Height.ToString());
		stringBuilder.Append("TotalItems: ").AppendLine(_diagnostics.Count.ToString());
		stringBuilder.Append("MovableItems: ").AppendLine(MovableCount.ToString());
		stringBuilder.Append("FixedItems: ").AppendLine(FixedCount.ToString());
		stringBuilder.Append("KeepContainers: ").AppendLine(_keepContainers.ToString());
		stringBuilder.Append("PreservedContainers: ").AppendLine(PreservedContainerCount.ToString());
		stringBuilder.AppendLine("---------- Item Details ----------");
		int num = 0;
		foreach (DiagnosticItem diagnostic in _diagnostics)
		{
			StringBuilder stringBuilder2 = stringBuilder.Append('[');
			int num2 = ++num;
			stringBuilder2.Append(num2.ToString("D3")).Append("] Name: ").Append(diagnostic.Name)
				.Append(" | ItemId: ")
				.Append(diagnostic.ItemId)
				.Append(" | Size: ")
				.Append(diagnostic.Width)
				.Append('x')
				.Append(diagnostic.Height)
				.Append(" | Type: ")
				.Append(diagnostic.Type)
				.Append(" | Categories: [")
				.Append(string.Join(",", diagnostic.Categories))
				.Append(']')
				.Append(" | Orientation: ")
				.Append(diagnostic.Orientation)
				.Append(" | CanRotate: ")
				.Append(diagnostic.CanRotate)
				.Append(" | Movable: ")
				.AppendLine(diagnostic.Movable.ToString());
		}
		stringBuilder.AppendLine("========== Snapshot End ==========");
		stringBuilder.AppendLine();
		File.AppendAllText(text2, stringBuilder.ToString(), (Encoding)(object)new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
		return text2;
	}

	public void WriteResultSnapshot(IReadOnlyList<Placement> movablePlacements, string outcome)
	{
		string text = Path.Combine(Application.persistentDataPath, "SmartInventoryArrange", "Logs");
		System.IO.Directory.CreateDirectory(text);
		string text2 = Path.Combine(text, "SmartInventoryArrange-Snapshot.log");
		Dictionary<string, Placement> positions = Problem.Fixed.Concat(movablePlacements).ToDictionary((Placement x) => x.StableId, (Placement x) => x, (IEqualityComparer<string>?)StringComparer.Ordinal);
		StringBuilder stringBuilder = new StringBuilder(512 + _diagnostics.Count * 150);
		stringBuilder.AppendLine("========== Smart Arrange Result ==========");
		stringBuilder.Append("Time: ").AppendLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
		stringBuilder.Append("Outcome: ").AppendLine(outcome);
		stringBuilder.Append("ContainerSize: ").Append(Width).Append('x')
			.AppendLine(Height.ToString());
		int num = 0;
		foreach (DiagnosticItem diagnostic in _diagnostics)
		{
			if (positions.TryGetValue(diagnostic.StableId, out var value))
			{
				StringBuilder stringBuilder2 = stringBuilder.Append('[');
				int num2 = ++num;
				stringBuilder2.Append(num2.ToString("D3")).Append("] Name: ").Append(diagnostic.Name)
					.Append(" | ItemId: ")
					.Append(diagnostic.ItemId)
					.Append(" | Coordinate: (")
					.Append(value.X)
					.Append(',')
					.Append(value.Y)
					.Append(')')
					.Append(" | Size: ")
					.Append(value.Mask.Width)
					.Append('x')
					.Append(value.Mask.Height)
					.Append(" | Type: ")
					.Append(diagnostic.Type)
					.Append(" | Orientation: ")
					.AppendLine(value.GameOrientation.ToString());
			}
			else
			{
				StringBuilder stringBuilder3 = stringBuilder.Append('[');
				int num2 = ++num;
				stringBuilder3.Append(num2.ToString("D3")).Append("] Name: ").Append(diagnostic.Name)
					.Append(" | ItemId: ")
					.Append(diagnostic.ItemId)
					.Append(" | Coordinate: (")
					.Append(diagnostic.OriginalX)
					.Append(',')
					.Append(diagnostic.OriginalY)
					.Append(')')
					.Append(" | Size: ")
					.Append(diagnostic.Width)
					.Append('x')
					.Append(diagnostic.Height)
					.Append(" | Type: ")
					.Append(diagnostic.Type)
					.Append(" | Orientation: ")
					.Append(diagnostic.Orientation)
					.AppendLine(" | Status: original/unresolved");
			}
		}
		if (_instanceOrder.Length != 0)
		{
			stringBuilder.AppendLine("---------- Instance slot order ----------");
			stringBuilder.Append("Policy: ").AppendLine("semantic-slots-v3:real-state-appraisal-ignored;name;empty-last;usable-sealed-rustfree;wine-2-1-0-age-desc-ferment-asc;water-grade-then-volume;food-fresh-first;absolute-remaining-desc;quality-before-quantity;stable-id");
			stringBuilder.Append("InstanceOrderingNotes: ").AppendLine(_instanceOrderingNotes);
			stringBuilder.Append("InstanceOrderingMs: ").AppendLine(_instanceOrderingMs.ToString("F3", (IFormatProvider)CultureInfo.InvariantCulture));
			Placement value3;
			Placement value2;
			foreach (InstanceOrderInfo item in Enumerable.OrderBy(_instanceOrder, (InstanceOrderInfo i) => (!positions.TryGetValue(i.StableId, out value3)) ? int.MaxValue : value3.Y).ThenBy((InstanceOrderInfo i) => (!positions.TryGetValue(i.StableId, out value2)) ? int.MaxValue : value2.X).ThenBy((InstanceOrderInfo i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal))
			{
				stringBuilder.Append("InstanceOrder: ").AppendLine(JsonSerializer.Serialize(item));
			}
		}
		stringBuilder.AppendLine("========== Result End ==========");
		stringBuilder.AppendLine();
		File.AppendAllText(text2, stringBuilder.ToString(), (Encoding)(object)new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
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

	public void AssertSnapshotCurrent(string stage = "before-apply")
	{
		Dictionary<IntPtr, SnapshotItem> dictionary = Enumerable.ToDictionary(_capturedObservation.Items, (SnapshotItem x) => x.Pointer);
		List<SnapshotItem> list = new List<SnapshotItem>();
		int num = 0;
		DefaultInterpolatedStringHandler val2 = default(DefaultInterpolatedStringHandler);
		for (int i = 0; i < ((GameInventory)_inventory).childItems.Count; i++)
		{
			GameItem val = ((GameInventory)_inventory).childItems[i];
			if (val == null)
			{
				nint pointer = 0;
				nint parentPointer = 0;
				val2 = new DefaultInterpolatedStringHandler(6, 1);
				val2.AppendLiteral("child-");
				val2.AppendFormatted<int>(i);
				list.Add(new SnapshotItem(pointer, parentPointer, val2.ToStringAndClear(), Movable: false, "null", null));
				continue;
			}
			if (_keepContainers && _capturedWindowPresence.TryGetValue(ReferenceIdentity.Pointer(val), out var value) && val.contentWindow != null != value)
			{
				val2 = new DefaultInterpolatedStringHandler(68, 2);
				val2.AppendLiteral("Snapshot guard rejected stage=");
				val2.AppendFormatted(stage);
				val2.AppendLiteral("; contentWindow presence changed for ");
				val2.AppendFormatted(val.identifier);
				val2.AppendLiteral(".");
				throw new InvalidOperationException(val2.ToStringAndClear());
			}
			dictionary.TryGetValue(ReferenceIdentity.Pointer(val), out var value2);
			GameInventory parentInventory = val.parentInventory;
			if (parentInventory != null && parentInventory != _inventory && SnapshotConsistency.SameNativeObject(ReferenceIdentity.Pointer(parentInventory), ReferenceIdentity.Pointer(_inventory)))
			{
				num++;
			}
			list.Add(Observe(val, value2?.StableId ?? val.identifier, value2?.Movable ?? false));
		}
		SnapshotConsistency.AssertSame(_capturedObservation, new InventoryObservation(ReferenceIdentity.Pointer(_inventory), Geometry(_inventory), Array.AsReadOnly(list.ToArray())), stage);
		val2 = new DefaultInterpolatedStringHandler(93, 3);
		val2.AppendLiteral("Smart arrange snapshot guard passed: stage=");
		val2.AppendFormatted(stage);
		val2.AppendLiteral(", members=");
		val2.AppendFormatted<int>(list.Count);
		val2.AppendLiteral(", nativeWrapperAliases=");
		val2.AppendFormatted<int>(num);
		val2.AppendLiteral(", changedFields=0");
		Debug.Log(val2.ToStringAndClear());
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
		foreach (KeyValuePair<IntPtr, bool> item in _capturedWindowPresence)
		{
			if (members[item.Key].contentWindow != null != item.Value)
			{
				throw new InvalidOperationException("Post-apply contentWindow presence changed.");
			}
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
		VerifyUnchanged();
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
		try
		{
			foreach (ValueTuple<Snap, GridShape, Placement> item2 in list)
			{
				item2.Item1.Item.modifiedShape = item2.Item2;
			}
			foreach (ValueTuple<Snap, GridShape, Placement> item3 in list)
			{
				if (!((GameInventory)_inventory).CheckInventorySlotStillValid(item3.Item1.Item, item3.Item2, item3.Item1.Item.state))
				{
					val3 = new DefaultInterpolatedStringHandler(55, 4);
					val3.AppendLiteral("Game slot/state validation rejected ");
					val3.AppendFormatted(item3.Item1.StableId);
					val3.AppendLiteral(" at ");
					val3.AppendFormatted<int>(item3.Item3.X);
					val3.AppendLiteral(",");
					val3.AppendFormatted<int>(item3.Item3.Y);
					val3.AppendLiteral(" orientation ");
					val3.AppendFormatted<int>(item3.Item3.GameOrientation);
					val3.AppendLiteral(".");
					throw new InvalidOperationException(val3.ToStringAndClear());
				}
			}
			((GameInventory)_inventory).Validate();
			_inventory.ForceRecomputeDynamicBackground();
			VerifyUnchanged(ignoreShape: true);
			if (_keepContainers)
			{
				AssertFixedSnapshotCurrent();
			}
			foreach (ValueTuple<Snap, GridShape, Placement> item4 in list)
			{
				if (item4.Item1.Item.modifiedShape.minX != item4.Item2.minX || item4.Item1.Item.modifiedShape.minY != item4.Item2.minY || item4.Item1.Item.modifiedShape.orientation != item4.Item2.orientation)
				{
					throw new InvalidOperationException("Post-apply shape verification failed for " + item4.Item1.Item.identifier);
				}
			}
		}
		catch
		{
			Rollback();
			throw;
		}
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
