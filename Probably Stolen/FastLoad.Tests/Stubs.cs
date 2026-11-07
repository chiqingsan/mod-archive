using System;
using System.IO;

namespace FastLoad;

/// <summary>测试桩：用 System.IO 模拟 ES3 的文件读写，仅用于离线验证 SaveSurgeon 的字符串处理。</summary>
internal static class ES3
{
    internal static string BaseDir = "";

    private static string P(string filePath) => Path.Combine(BaseDir, filePath);

    internal static string LoadRawString(string filePath) => File.ReadAllText(P(filePath));

    internal static void SaveRaw(string str, string filePath) => File.WriteAllText(P(filePath), str);

    internal static bool FileExists(string filePath) => File.Exists(P(filePath));

    internal static DateTime GetTimestamp(string filePath) => File.GetLastWriteTimeUtc(P(filePath));
}

internal static class SaveFiles
{
    internal static string Slot(int slotId) => $"save_{slotId}.es3";
}

internal sealed class NewGameData
{
    internal static NewGameData? Instance;

    internal int saveSlot = -1;

    internal string? saveFile;
}

internal static class Core
{
    internal static Logger? Log;

    internal static Flag? DumpStats;

    internal sealed class Logger
    {
        internal void Log(string message) => Console.WriteLine(message);

        internal void Warning(string message) => Console.WriteLine("[Warning] " + message);
    }

    internal sealed class Flag
    {
        internal bool Value;
    }
}
