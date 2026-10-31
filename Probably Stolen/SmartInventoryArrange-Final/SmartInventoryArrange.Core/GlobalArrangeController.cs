using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SmartInventoryArrange.Core;

public sealed class GlobalArrangeController : IDisposable
{
	public sealed record Request(Native34Solver.SearchSession Session, bool Restarted, long Generation, string InputSignature, int NodeBudget, int SolutionBatch, string RestartDetail);

	private Native34Solver.SearchSession? _session;

	private string? _problem;

	private string? _expectedPose;

	private string? _context;

	private long _generation;

	private bool _pending;

	public int InitialNodeBudget { get; init; } = 250000;


	public int ContinueNodeBudget { get; init; } = 25000;


	public int SolutionBatch { get; init; } = 64;


	public int ResumeSolutionBatch { get; init; } = 16;


	public int LargeInventoryThreshold { get; init; } = 100;


	public int LargeSolutionBatch { get; init; } = 128;


	public int LargeResumeSolutionBatch { get; init; } = 32;


	public Request Begin(LayoutProblem p, IReadOnlyList<Placement> original, string context)
	{
		if (_pending)
		{
			throw new InvalidOperationException("Previous request still pending.");
		}
		string text = ProblemSignature(p);
		string text2 = Native34Solver.Key(original);
		bool noSession = _session == null;
		bool problemChanged = !noSession && text != _problem;
		bool poseChanged = !noSession && text2 != _expectedPose;
		bool contextChanged = !noSession && context != _context;
		bool restarted = noSession || problemChanged || poseChanged || contextChanged;
		string reason = (noSession ? "no-session" : (problemChanged ? "problem" : (poseChanged ? "pose" : (contextChanged ? "context" : "resume"))));
		string detail = "problemChanged=" + (problemChanged ? "true" : "false") + ",poseChanged=" + (poseChanged ? "true" : "false") + ",contextChanged=" + (contextChanged ? "true" : "false") + ",reason=" + reason;
		if (restarted)
		{
			_session?.Dispose();
			_session = new Native34Solver().CreateSession(p, original);
			_problem = text;
			_context = context;
			_expectedPose = text2;
			_generation++;
		}
		_pending = true;
		bool large = p.Items.Count >= LargeInventoryThreshold;
		return new Request(_session, restarted, _generation, text, restarted ? InitialNodeBudget : ContinueNodeBudget, restarted ? (large ? LargeSolutionBatch : SolutionBatch) : (large ? LargeResumeSolutionBatch : ResumeSolutionBatch), detail);
	}

	public void Complete(Request request, LayoutProblem p, IReadOnlyList<Placement> output)
	{
		if (!_pending || request.Generation != _generation || request.Session != _session || request.InputSignature != _problem)
		{
			throw new InvalidOperationException("Stale search-session completion.");
		}
		if (!Native34Solver.ValidateLayout(p, output))
		{
			throw new InvalidOperationException("Illegal completed output.");
		}
		_expectedPose = Native34Solver.Key(output);
		_pending = false;
	}

	public void Reset()
	{
		_session?.Dispose();
		_session = null;
		_problem = (_expectedPose = (_context = null));
		_pending = false;
		_generation++;
	}

	public void Dispose()
	{
		Reset();
	}

	public static string ProblemSignature(LayoutProblem p)
	{
		List<bool> list = new List<bool>();
		for (int j = 0; j < p.Height; j++)
		{
			for (int l = 0; l < p.Width; l++)
			{
				list.Add(p.Usable[l, j]);
			}
		}
		return HashCompat.Sha256Hex(Encoding.UTF8.GetBytes(StableText.Serialize(new
		{
			Width = p.Width,
			Height = p.Height,
			usable = list,
			fixedItems = Enumerable.Select(p.Fixed.OrderBy((Placement q) => q.StableId, (IComparer<string>?)StringComparer.Ordinal), (Placement q) => new
			{
				StableId = q.StableId,
				X = q.X,
				Y = q.Y,
				GameOrientation = q.GameOrientation,
				Category = q.Category,
				mask = q.Mask.Signature
			}),
			items = Enumerable.Select(p.Items.OrderBy((LayoutItem i) => i.StableId, (IComparer<string>?)StringComparer.Ordinal), (LayoutItem i) => new
			{
				StableId = i.StableId,
				ItemId = i.ItemId,
				PrimaryCategory = i.PrimaryCategory,
				VisualPriority = i.VisualPriority,
				categories = Enumerable.OrderBy(i.Categories, (string c) => c, (IComparer<string>?)StringComparer.Ordinal),
				orientations = Enumerable.Select(Enumerable.OrderBy(i.Orientations, (Orientation o) => o.GameOrientation).ThenBy((Orientation o) => o.Mask.Signature, (IComparer<string>?)StringComparer.Ordinal), (Orientation o) => new
				{
					GameOrientation = o.GameOrientation,
					mask = o.Mask.Signature
				})
			}),
			categoryOrder = Enumerable.Select(Enumerable.OrderBy<KeyValuePair<string, int>, string>(p.CategoryOrder, (KeyValuePair<string, int> k) => k.Key, (IComparer<string>?)StringComparer.Ordinal), (KeyValuePair<string, int> k) => new { k.Key, k.Value })
		})));
	}
}
