using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MelonLoader;
using UnityEngine;

namespace WaterTower;

public static class Log
{
	private const string Prefix = "[WaterTower]";

	private const string FileName = "WaterTower.log";

	private const string PrevName = "WaterTower.prev.log";

	private const int MaxLines = 4000;

	private const int KeepLines = 2000;

	private static readonly object _lock = new object();

	private static readonly List<string> _pending = new List<string>();

	private static string _filePath;

	private static bool _fileReady;

	private static int _lines;

	public static bool Verbose = false;

	public static string FilePath => _filePath;

	public static void Init()
	{
		_filePath = ResolveLogPath();
		try
		{
			string directoryName = Path.GetDirectoryName(_filePath);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			try
			{
				if (File.Exists(_filePath))
				{
					string destFileName = Path.Combine(string.IsNullOrEmpty(directoryName) ? "." : directoryName, "WaterTower.prev.log");
					File.Copy(_filePath, destFileName, overwrite: true);
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Warning("[WaterTower] 备份上一场日志失败（不影响新日志）: " + ex.Message);
			}
			File.WriteAllText(_filePath, "# WaterTower 日志（纯净版；上一场见同目录 WaterTower.prev.log）" + Environment.NewLine + "# 每塔每晚三行账本（盘点/工作/结算）可互相验算；行格式：时间 [级别] 内容" + Environment.NewLine, Encoding.UTF8);
			_lines = 2;
			_fileReady = true;
		}
		catch (Exception ex2)
		{
			_fileReady = false;
			MelonLogger.Warning("[WaterTower] 无法创建日志文件，将只输出到 MelonLoader 日志: " + ex2.Message);
		}
	}

	private static string ResolveLogPath()
	{
		try
		{
			string dataPath = Application.dataPath;
			if (!string.IsNullOrEmpty(dataPath))
			{
				string directoryName = Path.GetDirectoryName(dataPath);
				if (!string.IsNullOrEmpty(directoryName))
				{
					return Path.Combine(directoryName, "MelonLoader", "WaterTower.log");
				}
			}
		}
		catch
		{
		}
		try
		{
			return Path.Combine(Path.GetTempPath(), "WaterTower.log");
		}
		catch
		{
			return "WaterTower.log";
		}
	}

	private static string Stamp()
	{
		return DateTime.Now.ToString("HH:mm:ss.fff");
	}

	private static void Emit(string level, string message)
	{
		string text = Stamp() + " [" + level + "] " + message;
		lock (_lock)
		{
			if (level == "ERROR")
			{
				MelonLogger.Error("[WaterTower] " + message);
			}
			else if (level == "WARN")
			{
				MelonLogger.Warning("[WaterTower] " + message);
			}
			if (!_fileReady)
			{
				_pending.Add(text);
				return;
			}
			try
			{
				File.AppendAllText(_filePath, text + Environment.NewLine, Encoding.UTF8);
				_lines++;
				RollIfNeeded();
			}
			catch
			{
				_pending.Add(text);
			}
		}
	}

	private static void RollIfNeeded()
	{
		if (_lines <= 4000)
		{
			return;
		}
		try
		{
			string[] array = File.ReadAllLines(_filePath, Encoding.UTF8);
			int num = ((array.Length < 2000) ? array.Length : 2000);
			string[] array2 = new string[num + 1];
			array2[0] = "# [日志滚动] 超过 " + 4000 + " 行，只保留最新 " + num + " 行（最新现场优先）";
			Array.Copy(array, array.Length - num, array2, 1, num);
			File.WriteAllLines(_filePath, array2, Encoding.UTF8);
			_lines = array2.Length;
		}
		catch
		{
		}
	}

	public static void Info(string message)
	{
		Emit("INFO", message);
	}

	public static void Success(string message)
	{
		Emit("OK  ", message);
	}

	public static void Warn(string message)
	{
		Emit("WARN", message);
	}

	public static void Error(string message)
	{
		Emit("ERROR", message);
	}

	public static void Debug(string message)
	{
		if (Verbose)
		{
			Emit("DEBUG", message);
		}
	}

	public static void Section(string title)
	{
		Debug("-----------------------------------------------------------");
		Emit("INFO", "== " + title);
		Debug("-----------------------------------------------------------");
	}

	public static void KV(string key, object value)
	{
		Emit("INFO", "   " + key.PadRight(28) + " : " + ((value == null) ? "<null>" : value.ToString()));
	}

	public static void Exception(Exception ex, string context)
	{
		if (ex == null)
		{
			Emit("ERROR", context + " : <null exception>");
			return;
		}
		Emit("ERROR", context + " : " + ex.GetType().Name + " - " + ex.Message);
		Exception innerException = ex.InnerException;
		int num = 1;
		while (innerException != null && num <= 6)
		{
			Emit("ERROR", "   inner[" + num + "] " + innerException.GetType().Name + " - " + innerException.Message);
			innerException = innerException.InnerException;
			num++;
		}
		if (!string.IsNullOrEmpty(ex.StackTrace))
		{
			string[] array = ex.StackTrace.Split('\n');
			int num2 = ((array.Length < 12) ? array.Length : 12);
			for (int i = 0; i < num2; i++)
			{
				Emit("ERROR", "   " + array[i].Trim());
			}
		}
	}

	public static void Flush()
	{
		lock (_lock)
		{
			if (_pending.Count == 0)
			{
				return;
			}
			try
			{
				if (!_fileReady)
				{
					string directoryName = Path.GetDirectoryName(_filePath);
					if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
					File.AppendAllText(_filePath, string.Empty, Encoding.UTF8);
					_fileReady = true;
					MelonLogger.Msg("[WaterTower] 日志文件已恢复写入: " + _filePath);
				}
				File.AppendAllLines(_filePath, _pending, Encoding.UTF8);
				_lines += _pending.Count;
				_pending.Clear();
				RollIfNeeded();
			}
			catch
			{
			}
		}
	}
}
