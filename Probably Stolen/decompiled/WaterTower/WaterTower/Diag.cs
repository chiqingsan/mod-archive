using System;
using System.Collections.Generic;

namespace WaterTower;

public static class Diag
{
	private const double MinSecondsSameKey = 3.0;

	private static readonly object _lock = new object();

	private static readonly HashSet<string> _once = new HashSet<string>();

	private static readonly Dictionary<string, DateTime> _lastByKey = new Dictionary<string, DateTime>();

	public static void Once(string key, string message)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		lock (_lock)
		{
			if (!_once.Add(key))
			{
				return;
			}
		}
		Log.Info(message);
	}

	public static void Summary(string key, string message)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		lock (_lock)
		{
			if (_lastByKey.TryGetValue(key, out var value) && (DateTime.Now - value).TotalSeconds < 3.0)
			{
				return;
			}
			_lastByKey[key] = DateTime.Now;
		}
		Log.Info(message);
	}

	public static void Exception(string key, Exception ex, string context)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		lock (_lock)
		{
			if (!_once.Add(key))
			{
				return;
			}
		}
		Log.Exception(ex, context);
	}

	public static void Flush()
	{
		Log.Flush();
	}
}
