using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using SmartInventoryArrange.Core;
using UnityEngine;

namespace SmartInventoryArrange;

public sealed class SmartArrangeMod : IMod
{
	internal static bool Busy;

	internal static ConfigEntry<bool>? KeepContainers;

	private static Harmony? Harmony;
	private static SmartArrangeRunner? Runner;

	private static readonly GlobalArrangeController Search = new GlobalArrangeController();

	public void Init(ModManifest manifest)
	{
		KeepContainers = ModConfig.For(manifest.ID).Define(
			"KeepContainers",
			true,
			"保留容器模式",
			() => "保留容器排列时固定容器",
			() => "启用后，选择“保留容器排列”时，会将有内容窗口的容器和电源块作为固定障碍，只移动其他物品。");
	}

	public void OnEnable()
	{
		var runnerObject = new GameObject("SmartInventoryArrange");
		UnityEngine.Object.DontDestroyOnLoad(runnerObject);
		Runner = runnerObject.AddComponent<SmartArrangeRunner>();
		Harmony = new Harmony("chiqingsan.SmartInventoryArrange");
		Harmony.PatchAll(typeof(SmartArrangeMod).Assembly);
		Debug.Log("Smart Inventory Arrange 已加载。");
	}

	public void OnDisable()
	{
		Harmony?.UnpatchAll("chiqingsan.SmartInventoryArrange");
		Harmony = null;
		NativeSmartMenu.Close();
		Search.Reset();
		Busy = false;
		if (Runner != null)
			UnityEngine.Object.Destroy(Runner.gameObject);
		Runner = null;
	}

	internal static void Run(GameGridInventory inventory, bool preserveContainers = false)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		if (Busy)
		{
			Debug.LogWarning("Smart arrange ignored: an arrangement is already running.");
			return;
		}
		Busy = true;
		try
		{
			bool flag = preserveContainers && (KeepContainers?.Value ?? true);
			InventoryTransaction inventoryTransaction = InventoryTransaction.Capture(inventory, flag);
			DefaultInterpolatedStringHandler val = new DefaultInterpolatedStringHandler(69, 3);
			val.AppendLiteral("Smart arrange mode: requested=");
			val.AppendFormatted(preserveContainers ? "preserve-containers" : "smart");
			val.AppendLiteral(", keepContainers=");
			val.AppendFormatted<bool>(flag);
			val.AppendLiteral(", preservedContainers=");
			val.AppendFormatted<int>(inventoryTransaction.PreservedContainerCount);
			Debug.Log(val.ToStringAndClear());
			string text = inventoryTransaction.WriteDiagnosticSnapshot();
			Debug.Log("Smart arrange exact input: " + inventoryTransaction.WriteExactInput());
			double num = DeterministicSolver.OccupancyRatio(inventoryTransaction.Problem);
			string text2 = DeterministicSolver.DensityMode(inventoryTransaction.Problem);
			val = new DefaultInterpolatedStringHandler(112, 10);
			val.AppendLiteral("Smart arrange snapshot: ");
			val.AppendFormatted<int>(inventoryTransaction.Width);
			val.AppendLiteral("x");
			val.AppendFormatted<int>(inventoryTransaction.Height);
			val.AppendLiteral(", occupancy=");
			val.AppendFormatted<double>(num, "P1");
			val.AppendLiteral(", mode=");
			val.AppendFormatted(text2);
			val.AppendLiteral(", movable=");
			val.AppendFormatted<int>(inventoryTransaction.MovableCount);
			val.AppendLiteral(", fixed=");
			val.AppendFormatted<int>(inventoryTransaction.FixedCount);
			val.AppendLiteral(", categories=");
			val.AppendFormatted<int>(inventoryTransaction.CategoryCount);
			val.AppendLiteral(", multi-category=");
			val.AppendFormatted<int>(inventoryTransaction.MultiCategoryCount);
			val.AppendLiteral(", unknown=");
			val.AppendFormatted<int>(inventoryTransaction.UnknownCategoryCount);
			val.AppendLiteral(", details=");
			val.AppendFormatted(text);
			Debug.Log(val.ToStringAndClear());
			if (inventoryTransaction.MovableCount == 0)
			{
				inventoryTransaction.WriteResultSnapshot(inventoryTransaction.OriginalPlacements, "empty");
				Debug.Log("Smart arrange: warehouse is empty or contains only fixed items.");
				Busy = false;
				return;
			}
			GlobalArrangeController.Request request = Search.Begin(inventoryTransaction.Problem, inventoryTransaction.OriginalPlacements, inventoryTransaction.SessionContext);
			val = new DefaultInterpolatedStringHandler(67, 4);
			val.AppendLiteral("Smart arrange request: action=");
			val.AppendFormatted(request.Restarted ? "fresh-global" : "resume-frontier");
			val.AppendLiteral(", input=");
			val.AppendFormatted(request.InputSignature);
			val.AppendLiteral(", nodeBudget=");
			val.AppendFormatted<int>(request.NodeBudget);
			val.AppendLiteral(", previousNodes=");
			val.AppendFormatted<long>(request.Session.TotalNodes);
			Debug.Log(val.ToStringAndClear());
            Runner!.RunCoroutine(SolveAndApply(inventoryTransaction, request));
		}
		catch (Exception ex)
		{
			Busy = false;
			Search.Reset();
			Debug.LogError("Smart arrange snapshot cancelled: " + ex);
		}
	}

	private static IEnumerator SolveAndApply(InventoryTransaction tx, GlobalArrangeController.Request request)
	{
		GlobalArrangeController.Request request2 = request;
		InventoryTransaction tx2 = tx;
		bool applied = false;
		bool applicationStarted = false;
		Task<ValueTuple<LayoutResult, LayoutScore, string, Native34Solver.SearchProgress>> task = Task.Run<ValueTuple<LayoutResult, LayoutScore, string, Native34Solver.SearchProgress>>((Func<ValueTuple<LayoutResult, LayoutScore, string, Native34Solver.SearchProgress>>)delegate
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			Native34Solver.SearchProgress searchProgress = request2.Session.Advance(request2.NodeBudget, request2.SolutionBatch);
			LayoutScore layoutScore = DeterministicSolver.Evaluate(tx2.Problem, tx2.OriginalPlacements);
			return new ValueTuple<LayoutResult, LayoutScore, string, Native34Solver.SearchProgress>(tx2.OrderInstances(searchProgress.Result), layoutScore, request2.Session.Diagnostic, searchProgress);
		});
		while (!((Task)task).IsCompleted)
		{
			yield return null;
		}
		try
		{
			if (((Task)task).IsFaulted)
			{
				throw ((Exception)(object)((Task)task).Exception)?.GetBaseException() ?? new InvalidOperationException("Background solver failed.");
			}
			if (((Task)task).IsCanceled)
			{
				throw new OperationCanceledException("Background solver was cancelled.");
			}
			ValueTuple<LayoutResult, LayoutScore, string, Native34Solver.SearchProgress> result = task.Result;
			LayoutResult item = result.Item1;
			tx2.AssertSnapshotCurrent();
			DefaultInterpolatedStringHandler val = new DefaultInterpolatedStringHandler(119, 7);
			val.AppendLiteral("Smart arrange search complete: batch=");
			val.AppendFormatted<int>(result.Item4.Batch);
			val.AppendLiteral(", newNodes=");
			val.AppendFormatted<long>(result.Item4.NewNodes);
			val.AppendLiteral(", totalNodes=");
			val.AppendFormatted<long>(result.Item4.TotalNodes);
			val.AppendLiteral(", newCandidates=");
			val.AppendFormatted<int>(result.Item4.NewCandidates);
			val.AppendLiteral(", uniqueCandidates=");
			val.AppendFormatted<int>(result.Item4.TotalCandidates);
			val.AppendLiteral(", canContinue=");
			val.AppendFormatted<bool>(result.Item4.CanContinue);
			val.AppendLiteral(", reason=");
			val.AppendFormatted(result.Item4.Reason);
			Debug.Log(val.ToStringAndClear());
			int num = ((!Native34Solver.SameLayout(item.Placements, tx2.OriginalPlacements)) ? 1 : 0);
			if (!Native34Solver.ValidateLayout(tx2.Problem, item.Placements))
			{
				throw new InvalidOperationException("Native34 final immutable geometry validation failed.");
			}
			if (!item.Complete || num <= 0)
			{
				Search.Complete(request2, tx2.Problem, tx2.OriginalPlacements);
				tx2.WriteResultSnapshot(tx2.OriginalPlacements, "kept-original");
				string text = (item.UsedFallback ? "no-fresh-complete-candidate-original-preserved" : result.Item4.Reason);
				ValueTuple<int, int, int> val2 = DeterministicSolver.EvaluateSpaceMetrics(tx2.Problem, tx2.OriginalPlacements);
				string text2 = DeterministicSolver.FirstDifferingMetric(tx2.Problem, item.Score, result.Item2);
				val = new DefaultInterpolatedStringHandler(251, 16);
				val.AppendLiteral("Smart arrange kept original layout: reason=");
				val.AppendFormatted(text);
				val.AppendLiteral(", firstDifference=");
				val.AppendFormatted(text2);
				val.AppendLiteral(", mode=");
				val.AppendFormatted(DeterministicSolver.DensityMode(tx2.Problem));
				val.AppendLiteral(", occupancy=");
				val.AppendFormatted<double>(DeterministicSolver.OccupancyRatio(tx2.Problem), "P1");
				val.AppendLiteral(", candidate complete=");
				val.AppendFormatted<bool>(item.Complete);
				val.AppendLiteral(", timeout=");
				val.AppendFormatted<bool>(item.TimedOut);
				val.AppendLiteral(", fallback=");
				val.AppendFormatted<bool>(item.UsedFallback);
				val.AppendLiteral(", elapsed=");
				val.AppendFormatted<double>(item.Elapsed.TotalMilliseconds, "F1");
				val.AppendLiteral("ms, candidateSpace=holes:");
				val.AppendFormatted<int>(item.Holes);
				val.AppendLiteral(",narrow:");
				val.AppendFormatted<int>(item.NarrowFragments);
				val.AppendLiteral(", baselineSpace=holes:");
				val.AppendFormatted<int>(val2.Item1);
				val.AppendLiteral(",narrow:");
				val.AppendFormatted<int>(val2.Item2);
				val.AppendLiteral(",ragged:");
				val.AppendFormatted<int>(val2.Item3);
				val.AppendLiteral(", candidateVisual=");
				val.AppendFormatted(VisualSummary(item.Score));
				val.AppendLiteral(", baselineVisual=");
				val.AppendFormatted(VisualSummary(result.Item2));
				val.AppendLiteral(", diagnostic=");
				val.AppendFormatted(result.Item3);
				Debug.Log(val.ToStringAndClear());
			}
			else
			{
				applicationStarted = true;
				tx2.Apply(item.Placements);
				applied = true;
				tx2.WriteResultSnapshot(item.Placements, "applied");
				Search.Complete(request2, tx2.Problem, item.Placements);
				int num2 = Enumerable.Count(item.Placements, (Placement p) => p.Rotated);
				val = new DefaultInterpolatedStringHandler(155, 11);
				val.AppendLiteral("Smart arrange success: mode=");
				val.AppendFormatted(DeterministicSolver.DensityMode(tx2.Problem));
				val.AppendLiteral(", occupancy=");
				val.AppendFormatted<double>(DeterministicSolver.OccupancyRatio(tx2.Problem), "P1");
				val.AppendLiteral(", placed=");
				val.AppendFormatted<int>(item.Placements.Count);
				val.AppendLiteral(", horizontalExceptions=");
				val.AppendFormatted<int>(num2);
				val.AppendLiteral(", holes=");
				val.AppendFormatted<int>(item.Holes);
				val.AppendLiteral(", narrowFragments=");
				val.AppendFormatted<int>(item.NarrowFragments);
				val.AppendLiteral(", elapsed=");
				val.AppendFormatted<double>(item.Elapsed.TotalMilliseconds, "F1");
				val.AppendLiteral("ms, fallback=");
				val.AppendFormatted<bool>(item.UsedFallback);
				val.AppendLiteral(", visual=");
				val.AppendFormatted(VisualSummary(item.Score));
				val.AppendLiteral(", splitGroups=");
				val.AppendFormatted(DeterministicSolver.DescribeSplits(item.Placements));
				val.AppendLiteral(", native34=");
				val.AppendFormatted(result.Item3);
				Debug.Log(val.ToStringAndClear());
			}
		}
		catch (Exception ex2)
		{
			Search.Reset();
			try
			{
				if (applied)
				{
					tx2.RestoreCapturedSnapshot();
				}
				tx2.WriteResultSnapshot(tx2.OriginalPlacements, applied ? "cancelled-restored" : (applicationStarted ? "application-failed" : "cancelled-before-apply"));
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Final coordinate snapshot failed: " + ex.Message);
			}
			Debug.LogError("Smart arrange " + (applied ? "cancelled-restored" : (applicationStarted ? "application-failed" : "cancelled-before-apply")) + ": " + ex2);
		}
		finally
		{
			Busy = false;
		}
	}

	private static string VisualSummary(LayoutScore score)
	{
		DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
		val = new DefaultInterpolatedStringHandler(71, 6);
		val.AppendLiteral("typeSplits=");
		val.AppendFormatted<int>(-score.NegativeTypeRuns);
		val.AppendLiteral(",typeRows=");
		val.AppendFormatted<int>(score.SameTypeRowAlignment);
		val.AppendLiteral(",typeEdges=");
		val.AppendFormatted<int>(score.SameTypeHorizontalContact);
		val.AppendLiteral(",cohortSplits=");
		val.AppendFormatted<int>(-score.NegativeCohortRuns);
		val.AppendLiteral(",cohortRows=");
		val.AppendFormatted<int>(score.CohortRowAlignment);
		val.AppendLiteral(",cohortEdges=");
		val.AppendFormatted<int>(score.CohortHorizontalContact);
		return val.ToStringAndClear();
	}
}
