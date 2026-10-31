using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Debug = UnityEngine.Debug;

namespace SmartInventoryArrange;

/// <summary>
/// 单次排序的阶段计时与分配统计。输出一行日志，用于定位点击帧和应用帧的耗时来源。
/// </summary>
internal static class PerfTrace
{
	private static readonly Dictionary<string, double> StageMs = new Dictionary<string, double>(StringComparer.Ordinal);

	private static readonly List<string> StageOrder = new List<string>();

	private static readonly Dictionary<string, long> Counters = new Dictionary<string, long>(StringComparer.Ordinal);

	private static readonly Dictionary<string, long> Allocated = new Dictionary<string, long>(StringComparer.Ordinal);

	private static readonly List<string> AllocatedOrder = new List<string>();

	private static long _startTimestamp;

	private static long _startAllocated;

	private static int _startGen0;

	private static int _startGen1;

	private static int _startGen2;

	private static bool _active;

	internal static void Begin()
	{
		StageMs.Clear();
		StageOrder.Clear();
		Counters.Clear();
		Allocated.Clear();
		AllocatedOrder.Clear();
		_startTimestamp = Stopwatch.GetTimestamp();
		_startAllocated = GC.GetTotalMemory(forceFullCollection: false);
		_startGen0 = GC.CollectionCount(0);
		_startGen1 = GC.CollectionCount(1);
		_startGen2 = GC.CollectionCount(2);
		_active = true;
	}

	internal static long Mark()
	{
		return Stopwatch.GetTimestamp();
	}

	internal static long MarkAlloc()
	{
		return GC.GetAllocatedBytesForCurrentThread();
	}

	internal static long MarkGc()
	{
		return GC.CollectionCount(0);
	}

	internal static void AddGc(string name, long startCount)
	{
		if (!_active)
		{
			return;
		}
		Counters.TryGetValue(name, out long value);
		Counters[name] = value + (GC.CollectionCount(0) - startCount);
	}

	internal static void AddAllocated(string name, long bytes)
	{
		if (!_active)
		{
			return;
		}
		if (!Allocated.ContainsKey(name))
		{
			AllocatedOrder.Add(name);
			Allocated[name] = 0L;
		}
		Allocated[name] += bytes;
	}

	internal static double ElapsedMs(long startTimestamp)
	{
		return (Stopwatch.GetTimestamp() - startTimestamp) * 1000.0 / Stopwatch.Frequency;
	}

	internal static void Add(string name, long startTimestamp)
	{
		if (!_active)
		{
			return;
		}
		AddMs(name, (Stopwatch.GetTimestamp() - startTimestamp) * 1000.0 / Stopwatch.Frequency);
	}

	internal static void AddMs(string name, double milliseconds)
	{
		if (!_active)
		{
			return;
		}
		if (!StageMs.ContainsKey(name))
		{
			StageOrder.Add(name);
			StageMs[name] = 0.0;
		}
		StageMs[name] += milliseconds;
	}

	internal static void Increment(string counter)
	{
		if (!_active)
		{
			return;
		}
		Counters.TryGetValue(counter, out long value);
		Counters[counter] = value + 1;
	}

	internal static void Flush(string tag)
	{
		if (!_active)
		{
			return;
		}
		_active = false;
		double totalMs = (Stopwatch.GetTimestamp() - _startTimestamp) * 1000.0 / Stopwatch.Frequency;
		long allocated = GC.GetTotalMemory(forceFullCollection: false) - _startAllocated;
		StringBuilder builder = new StringBuilder(256);
		builder.Append("Smart arrange perf: ").Append(tag);
		foreach (string name in StageOrder)
		{
			builder.Append(';').Append(name).Append('=')
				.Append(StageMs[name].ToString("F2", CultureInfo.InvariantCulture));
		}
		foreach (KeyValuePair<string, long> counter in Counters)
		{
			builder.Append(';').Append(counter.Key).Append('#').Append(counter.Value);
		}
		foreach (string name in AllocatedOrder)
		{
			builder.Append(';').Append(name).Append("KB=")
				.Append((Allocated[name] / 1024).ToString(CultureInfo.InvariantCulture));
		}
		builder.Append(";total=").Append(totalMs.ToString("F2", CultureInfo.InvariantCulture));
		builder.Append(";gc0=").Append(GC.CollectionCount(0) - _startGen0);
		builder.Append(";gc1=").Append(GC.CollectionCount(1) - _startGen1);
		builder.Append(";gc2=").Append(GC.CollectionCount(2) - _startGen2);
		builder.Append(";allocKB=").Append((allocated / 1024).ToString(CultureInfo.InvariantCulture));
		Debug.Log(builder.ToString());
	}
}
