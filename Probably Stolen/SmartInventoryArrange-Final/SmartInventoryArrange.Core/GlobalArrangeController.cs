using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SmartInventoryArrange.Core;

public sealed class GlobalArrangeController : IDisposable
{
	public sealed record Request(Native34Solver.SearchSession Session, bool Restarted, long Generation, string InputSignature, int NodeBudget, int SolutionBatch);

	private Native34Solver.SearchSession? _session;

	private string? _problem;

	private string? _expectedPose;

	private string? _context;

	private long _generation;

	private bool _pending;

	public int InitialNodeBudget { get; init; } = 250000;


	public int ContinueNodeBudget { get; init; } = 25000;


	public int SolutionBatch { get; init; } = 64;


	public Request Begin(LayoutProblem p, IReadOnlyList<Placement> original, string context)
	{
		if (_pending)
		{
			throw new InvalidOperationException("Previous request still pending.");
		}
		string text = ProblemSignature(p);
		string text2 = Native34Solver.Key(original);
		bool flag = _session == null || text != _problem || text2 != _expectedPose || context != _context;
		if (flag)
		{
			_session?.Dispose();
			_session = new Native34Solver().CreateSession(p, original);
			_problem = text;
			_context = context;
			_expectedPose = text2;
			_generation++;
		}
		_pending = true;
		return new Request(_session, flag, _generation, text, flag ? InitialNodeBudget : ContinueNodeBudget, SolutionBatch);
	}

	public void Complete(Request request, LayoutProblem p, IReadOnlyList<Placement> output)
	{
		if (!_pending || request.Generation != _generation || request.Session != _session || ProblemSignature(p) != _problem)
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
