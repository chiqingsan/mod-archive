using System;
using UnityEngine;

namespace PurifierTower;

/// <summary>
/// 测试/诊断开关：只有启动游戏时带命令行参数 <c>-pt-debug</c> 才启用
/// 「自动发塔、测试水瓶、工厂追踪补丁」。
///
/// 注意：这里刻意不读取任何文件（例如检测 debug.txt），否则 ModLoader 会判定
/// 本 mod 请求了未声明的 files（读取/写入任意文件）权限并拒绝加载。
/// 发布形态：不带参数 -> 只注册物品与窗口，不做任何测试性行为。
///
/// 启用方式（Steam）：库 -> Probably Stolen -> 属性 -> 通用 -> 启动选项，填：
///     -pt-debug
/// </summary>
internal static class DebugTools
{
    private const string DebugArg = "-pt-debug";

    public static bool Enabled { get; private set; }

    public static void Init()
    {
        try
        {
            string[] args = Environment.GetCommandLineArgs();
            Enabled = Array.Exists(args, arg => string.Equals(arg, DebugArg, StringComparison.OrdinalIgnoreCase));
            Debug.Log("[PurifierTower] debug mode = " + Enabled + " (launch arg " + DebugArg + ")");
        }
        catch (Exception error)
        {
            Enabled = false;
            Debug.LogWarning("[PurifierTower] debug flag check failed: " + error.Message);
        }
    }
}
