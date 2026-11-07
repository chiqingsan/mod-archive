using System;
using System.IO;
using FastLoad;

var dir = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
ES3.BaseDir = dir;
Core.Log = new Core.Logger();
Core.DumpStats = new Core.Flag { Value = true };

Console.WriteLine($"源目录: {dir}");
var plan = SaveSurgeon.GetPlan("save_17.es3");
if (plan == null)
{
    Console.WriteLine("构建失败");
    return 1;
}

Console.WriteLine($"raw={plan.RawLength:N0} slim={plan.SlimLength:N0} preload={plan.PreloadLength:N0} 缓存库存={plan.InvJson.Count}");
foreach (var pair in plan.InvJson)
{
    File.WriteAllText(Path.Combine(dir, "dump_" + pair.Key + ".txt"), pair.Value);
    Console.WriteLine($"  {pair.Key}: {pair.Value.Length:N0} 字符");
}

Console.WriteLine("SlimReady=" + plan.SlimReady + " PreloadReady=" + plan.PreloadReady + " SourceStamp=" + plan.SourceStamp.ToString("O"));
return 0;
