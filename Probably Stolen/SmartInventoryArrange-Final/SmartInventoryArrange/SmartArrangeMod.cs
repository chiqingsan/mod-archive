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

	internal static ConfigEntry<bool>? EnablePreserveContainers;

	private static Harmony? Harmony;
	private static SmartArrangeRunner? Runner;

	private static readonly GlobalArrangeController Search = new GlobalArrangeController();

	public void Init(ModManifest manifest)
	{
		EnablePreserveContainers = ModConfig.For(manifest.ID).Define(
			"EnablePreserveContainers",
			false,
			"保留容器排列",
			() => "启用保留容器排列功能",
			() => "启用后，排序菜单会多出“保留容器排列”选项：固定有内容窗口的容器和电源块，只移动其他物品。");
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
		if (Busy)
		{
			Debug.LogWarning("Smart arrange ignored: an arrangement is already running.");
			return;
		}
		if (Runner == null)
		{
			Debug.LogWarning("Smart arrange ignored: runner is not available.");
			return;
		}
		bool keepContainers = preserveContainers && (EnablePreserveContainers?.Value ?? false);
		Debug.Log("Smart arrange mode: requested=" + (preserveContainers ? "preserve-containers" : "smart") + ", keepContainers=" + keepContainers);
		Busy = true;
		PerfTrace.Begin();
		long clickFrameStarted = PerfTrace.Mark();
		Runner.RunCoroutine(Arrange(inventory, clickFrameStarted, keepContainers));
	}

	private sealed record BackgroundResult(
		GlobalArrangeController.Request Request,
		LayoutResult Result,
		LayoutScore Score,
		Native34Solver.SearchProgress Progress,
		string Diagnostic,
		double Occupancy,
		string Mode,
		double ValidateMs,
		double MetricsMs,
		double ContextMs,
		double BeginMs,
		long BackgroundAlloc,
		long AdvanceAlloc);

	private static IEnumerator Arrange(GameGridInventory inventory, long clickFrameStarted, bool keepContainers)
	{
		long captureGcStart = PerfTrace.MarkGc();
		InventoryTransaction? transaction = null;
		BackgroundResult? background = null;
		Exception? failure = null;
		bool applied = false;
		bool applicationStarted = false;
		try
		{
			InventoryTransaction.CaptureResult capture = new InventoryTransaction.CaptureResult();
			IEnumerator captureRoutine = InventoryTransaction.CaptureCoroutine(inventory, keepContainers, capture);
			while (!DriveOne(captureRoutine, ref failure))
			{
				yield return captureRoutine.Current;
			}
			if (failure != null)
			{
				HandleFailure(null, false, false, failure);
				PerfTrace.Flush("capture-failed");
				yield break;
			}
			transaction = capture.Transaction ?? throw new InvalidOperationException("Snapshot capture produced no transaction.");
			PerfTrace.Add("capture", clickFrameStarted);
			if (transaction.PreservedContainerCount > 0)
			{
				Debug.Log("Smart arrange preserved containers: " + transaction.PreservedContainerCount);
			}
			failure = null;
			long captureGuardStarted = PerfTrace.Mark();
			IEnumerator captureGuardRoutine = transaction.AssertSnapshotCurrentCoroutine("capture-complete");
			while (!DriveOne(captureGuardRoutine, ref failure))
			{
				yield return captureGuardRoutine.Current;
			}
			if (failure != null)
			{
				HandleFailure(transaction, false, false, failure);
				PerfTrace.Flush("capture-changed");
				yield break;
			}
			PerfTrace.Add("captureGuard", captureGuardStarted);
			PerfTrace.AddGc("captureGc", captureGcStart);
			PerfTrace.Add("clickFrame", clickFrameStarted);
			if (transaction.MovableCount == 0)
			{
				double emptyOccupancy = DeterministicSolver.OccupancyRatio(transaction.Problem);
				string emptyMode = DeterministicSolver.DensityMode(transaction.Problem);
				LogSnapshot(transaction, emptyOccupancy, emptyMode);
				Debug.Log("Smart arrange: warehouse is empty or contains only fixed items.");
				PerfTrace.Flush("empty");
				yield break;
			}
			InventoryTransaction capturedTransaction = transaction;
			long backgroundGcStart = PerfTrace.MarkGc();
			long waitStarted = PerfTrace.Mark();
			Task<BackgroundResult> task = Task.Run(() => RunBackground(capturedTransaction));
			while (!task.IsCompleted)
			{
				yield return null;
			}
			PerfTrace.Add("backgroundWait", waitStarted);
			try
			{
				if (task.IsFaulted)
				{
					throw task.Exception?.GetBaseException() ?? new InvalidOperationException("Background solver failed.");
				}
				if (task.IsCanceled)
				{
					throw new OperationCanceledException("Background solver was cancelled.");
				}
				background = task.Result;
			}
			catch (Exception ex2)
			{
				failure = ex2;
			}
			if (failure != null || background == null)
			{
				HandleFailure(transaction, false, false, failure ?? new InvalidOperationException("Background solver failed."));
				PerfTrace.Flush("background-failed");
				yield break;
			}
			PerfTrace.AddMs("validateProblem", background.ValidateMs);
			PerfTrace.AddMs("problemMetrics", background.MetricsMs);
			PerfTrace.AddMs("sessionContext", background.ContextMs);
			PerfTrace.AddMs("begin", background.BeginMs);
			PerfTrace.AddGc("backgroundGc", backgroundGcStart);
			LogSnapshot(transaction, background.Occupancy, background.Mode);
			LogRequest(background.Request);
			long applyFrameStarted = PerfTrace.Mark();
			failure = null;
			long guardStarted = PerfTrace.Mark();
			IEnumerator guardRoutine = transaction.AssertSnapshotCurrentCoroutine("before-apply");
			while (!DriveOne(guardRoutine, ref failure))
			{
				yield return guardRoutine.Current;
			}
			if (failure != null)
			{
				HandleFailure(transaction, false, false, failure);
				PerfTrace.Flush("guard-failed");
				yield break;
			}
			PerfTrace.Add("preApplyGuard", guardStarted);
			long applyGcStart = PerfTrace.MarkGc();
			try
			{
				ApplyOrKeep(transaction, background, applyFrameStarted, ref applied, ref applicationStarted);
			}
			catch (Exception ex3)
			{
				failure = ex3;
			}
			PerfTrace.AddGc("applyGc", applyGcStart);
			if (failure != null)
			{
				HandleFailure(transaction, applied, applicationStarted, failure);
				PerfTrace.Flush("apply-failed");
				yield break;
			}
		}
		finally
		{
			PerfTrace.Flush("arrange");
			Busy = false;
		}
	}

	private static BackgroundResult RunBackground(InventoryTransaction tx)
	{
		long backgroundAllocStart = PerfTrace.MarkAlloc();
		long validateStarted = PerfTrace.Mark();
		DeterministicSolver.ValidateProblem(tx.Problem);
		double validateMs = PerfTrace.ElapsedMs(validateStarted);
		long metricsStarted = PerfTrace.Mark();
		double occupancy = DeterministicSolver.OccupancyRatio(tx.Problem);
		string mode = DeterministicSolver.DensityMode(tx.Problem);
		double metricsMs = PerfTrace.ElapsedMs(metricsStarted);
		long contextStarted = PerfTrace.Mark();
		string context = tx.BuildSessionContext();
		double contextMs = PerfTrace.ElapsedMs(contextStarted);
		long beginStarted = PerfTrace.Mark();
		GlobalArrangeController.Request request = Search.Begin(tx.Problem, tx.OriginalPlacements, context);
		double beginMs = PerfTrace.ElapsedMs(beginStarted);
		Native34Solver.SearchProgress progress = request.Session.Advance(request.NodeBudget, request.SolutionBatch);
		long advanceAlloc = request.Session.LastAdvanceAllocatedBytes;
		string diagnostic = request.Session.Diagnostic;
		LayoutScore score = DeterministicSolver.Evaluate(tx.Problem, tx.OriginalPlacements);
		LayoutResult result = tx.OrderInstances(progress.Result);
		long backgroundAlloc = PerfTrace.MarkAlloc() - backgroundAllocStart;
		return new BackgroundResult(request, result, score, progress, diagnostic, occupancy, mode, validateMs, metricsMs, contextMs, beginMs, backgroundAlloc, advanceAlloc);
	}

	internal static string DescribeInventory(GameGridInventory? inventory)
	{
		try
		{
			RectTransform rectTransform = inventory?.rectTransform;
			if (rectTransform is not RectTransform current)
			{
				return "unknown";
			}
			string text = "";
			Transform transform = current;
			for (int i = 0; i < 3; i++)
			{
				if (transform == null)
				{
					break;
				}
				text = ((text.Length == 0) ? transform.gameObject.name : (transform.gameObject.name + "/" + text));
				transform = transform.parent;
			}
			return text;
		}
		catch
		{
			return "unknown";
		}
	}

	private static void ApplyOrKeep(InventoryTransaction tx, BackgroundResult background, long applyFrameStarted, ref bool applied, ref bool applicationStarted)
	{
		LayoutResult item = background.Result;
		Native34Solver.SearchProgress progress = background.Progress;
		Debug.Log("Smart arrange search complete: batch=" + progress.Batch + ", newNodes=" + progress.NewNodes + ", totalNodes=" + progress.TotalNodes + ", newCandidates=" + progress.NewCandidates + ", uniqueCandidates=" + progress.TotalCandidates + ", canContinue=" + progress.CanContinue + ", reason=" + progress.Reason);
		long layoutValidateStarted = PerfTrace.Mark();
		int num = ((!Native34Solver.SameLayout(item.Placements, tx.OriginalPlacements)) ? 1 : 0);
		if (!Native34Solver.ValidateLayout(tx.Problem, item.Placements))
		{
			throw new InvalidOperationException("Native34 final immutable geometry validation failed.");
		}
		PerfTrace.Add("layoutValidate", layoutValidateStarted);
		int comparison = 0;
		string difference = "equal";
		bool spaceBetter = false;
		if (item.Complete && num > 0)
		{
			comparison = DeterministicSolver.CompareForProblem(tx.Problem, item.Score, background.Score);
			difference = DeterministicSolver.FirstDifferingMetric(tx.Problem, item.Score, background.Score);
			if (comparison <= 0)
			{
				ValueTuple<int, int, int> candidateSpace = DeterministicSolver.EvaluateSpaceMetrics(tx.Problem, item.Placements);
				ValueTuple<int, int, int> baselineSpace = DeterministicSolver.EvaluateSpaceMetrics(tx.Problem, tx.OriginalPlacements);
				spaceBetter = candidateSpace.Item1 < baselineSpace.Item1 && candidateSpace.Item2 <= baselineSpace.Item2 && candidateSpace.Item3 <= baselineSpace.Item3;
			}
		}
		bool materiallyBetter = comparison > 0 && difference != "stableTie";
		bool acceptable = materiallyBetter || comparison == 0 || spaceBetter;
		if (!item.Complete || num <= 0 || !acceptable)
		{
			Search.Complete(background.Request, tx.Problem, tx.OriginalPlacements);
			string text = (item.UsedFallback ? "no-fresh-complete-candidate-original-preserved" : background.Progress.Reason);
			ValueTuple<int, int, int> val2 = DeterministicSolver.EvaluateSpaceMetrics(tx.Problem, tx.OriginalPlacements);
			string text2 = difference;
			DefaultInterpolatedStringHandler val = new DefaultInterpolatedStringHandler(251, 16);
			val.AppendLiteral("Smart arrange kept original layout: reason=");
			val.AppendFormatted(text);
			val.AppendLiteral(", firstDifference=");
			val.AppendFormatted(text2);
			val.AppendLiteral(", mode=");
			val.AppendFormatted(DeterministicSolver.DensityMode(tx.Problem));
			val.AppendLiteral(", occupancy=");
			val.AppendFormatted<double>(DeterministicSolver.OccupancyRatio(tx.Problem), "P1");
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
			val.AppendFormatted(VisualSummary(background.Score));
			val.AppendLiteral(", diagnostic=");
			val.AppendFormatted(background.Diagnostic);
			Debug.Log(val.ToStringAndClear() + ", spaceBetter=" + spaceBetter + ", comparison=" + comparison + ", container=" + DescribeInventory(tx.Inventory) + ", layout=" + tx.DescribeLayoutChange(item.Placements, 48));
		}
		else
		{
			applicationStarted = true;
			long applyStarted = PerfTrace.Mark();
			tx.Apply(item.Placements);
			PerfTrace.Add("apply", applyStarted);
			applied = true;
			long completeStarted = PerfTrace.Mark();
			Search.Complete(background.Request, tx.Problem, item.Placements);
			PerfTrace.Add("searchComplete", completeStarted);
			int num2 = Enumerable.Count(item.Placements, (Placement p) => p.Rotated);
			DefaultInterpolatedStringHandler val = new DefaultInterpolatedStringHandler(167, 12);
			val.AppendLiteral("Smart arrange success: mode=");
			val.AppendFormatted(DeterministicSolver.DensityMode(tx.Problem));
			val.AppendLiteral(", occupancy=");
			val.AppendFormatted<double>(DeterministicSolver.OccupancyRatio(tx.Problem), "P1");
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
			val.AppendLiteral(", gate=");
			val.AppendFormatted((comparison > 0) ? "quality" : ((comparison == 0) ? "tie" : "space"));
			val.AppendLiteral(", visual=");
			val.AppendFormatted(VisualSummary(item.Score));
			val.AppendLiteral(", splitGroups=");
			val.AppendFormatted(DeterministicSolver.DescribeSplits(item.Placements));
			val.AppendLiteral(", native34=");
			val.AppendFormatted(background.Diagnostic);
			Debug.Log(val.ToStringAndClear() + ", container=" + DescribeInventory(tx.Inventory));
		}
		PerfTrace.Add("applyFrame", applyFrameStarted);
	}

	private static void LogSnapshot(InventoryTransaction transaction, double occupancy, string mode)
	{
		DefaultInterpolatedStringHandler val = new DefaultInterpolatedStringHandler(112, 10);
		val.AppendLiteral("Smart arrange snapshot: ");
		val.AppendFormatted<int>(transaction.Width);
		val.AppendLiteral("x");
		val.AppendFormatted<int>(transaction.Height);
		val.AppendLiteral(", occupancy=");
		val.AppendFormatted<double>(occupancy, "P1");
		val.AppendLiteral(", mode=");
		val.AppendFormatted(mode);
		val.AppendLiteral(", movable=");
		val.AppendFormatted<int>(transaction.MovableCount);
		val.AppendLiteral(", fixed=");
		val.AppendFormatted<int>(transaction.FixedCount);
		val.AppendLiteral(", categories=");
		val.AppendFormatted<int>(transaction.CategoryCount);
		val.AppendLiteral(", multi-category=");
		val.AppendFormatted<int>(transaction.MultiCategoryCount);
		val.AppendLiteral(", unknown=");
		val.AppendFormatted<int>(transaction.UnknownCategoryCount);
		Debug.Log(val.ToStringAndClear());
	}

	private static void LogRequest(GlobalArrangeController.Request request)
	{
		DefaultInterpolatedStringHandler val = new DefaultInterpolatedStringHandler(67, 5);
		val.AppendLiteral("Smart arrange request: action=");
		val.AppendFormatted(request.Restarted ? "fresh-global" : "resume-frontier");
		val.AppendLiteral(", input=");
		val.AppendFormatted(request.InputSignature);
		val.AppendLiteral(", nodeBudget=");
		val.AppendFormatted<int>(request.NodeBudget);
		val.AppendLiteral(", previousNodes=");
		val.AppendFormatted<long>(request.Session.TotalNodes);
		val.AppendLiteral(", ");
		val.AppendFormatted(request.RestartDetail);
		Debug.Log(val.ToStringAndClear());
	}

	private static void HandleFailure(InventoryTransaction? tx, bool applied, bool applicationStarted, Exception failure)
	{
		Search.Reset();
		try
		{
			if (tx != null && applied)
			{
				tx.RestoreCapturedSnapshot();
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("Final coordinate snapshot failed: " + ex.Message);
		}
		Debug.LogError("Smart arrange " + (applied ? "cancelled-restored" : (applicationStarted ? "application-failed" : "cancelled-before-apply")) + ": " + failure);
	}

	private static bool DriveOne(IEnumerator routine, ref Exception? failure)
	{
		try
		{
			return !routine.MoveNext();
		}
		catch (Exception ex)
		{
			failure = ex;
			return true;
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
