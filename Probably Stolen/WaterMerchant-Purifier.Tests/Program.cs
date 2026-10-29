using System.Reflection;
static void Check(bool ok,string message) { if (!ok) throw new Exception(message); Console.WriteLine("PASS " + message); }
var assembly = typeof(WaterMerchant.Mod).Assembly;
var settings = assembly.GetType("WaterMerchant.Settings")!;
var configType = typeof(ConfigEntry<bool>);
var ctor = configType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(bool) }, null)!;
var amount = (ConfigEntry<bool>)ctor.Invoke(new object[] { true });
var quality = (ConfigEntry<bool>)ctor.Invoke(new object[] { true });
settings.GetField("PurifierAmount",BindingFlags.Static|BindingFlags.NonPublic)!.SetValue(null,amount);
settings.GetField("PurifierQuality",BindingFlags.Static|BindingFlags.NonPublic)!.SetValue(null,quality);
var amountPrefix = assembly.GetType("WaterMerchant.PurifierAmount")!.GetMethod("Prefix",BindingFlags.Static|BindingFlags.NonPublic)!;
var qualityPrefix = assembly.GetType("WaterMerchant.PurifierQuality")!.GetMethod("Prefix",BindingFlags.Static|BindingFlags.NonPublic)!;
double Apply(MethodInfo prefix,int arg,Func<int,double> original) { object[] a = {arg,0d}; var run=(bool)prefix.Invoke(null,a)!; return run ? original(arg) : (double)a[1]; }
double Dirt(int n)=>Apply(amountPrefix,n,MachinePurifier.GetDirtBonus);
double Quality(int n)=>Apply(qualityPrefix,n,MachinePurifier.GetPurityMult);
int Cap(int n)=>10000-(int)Math.Round(100*Quality(n));
Check(Math.Abs(Dirt(9599)-1.5025)<0.00001,"below 96% uses native interpolation");
foreach (var purity in new[] {9600,9700,9800,9900,10000}) Check(Dirt(purity)==1.5,$"purity {purity/100.0}% bonus 1.5");
Check(Cap(149)==9990,"149% quality capped at 99.9%");
Check(Cap(150)==10000,"150% quality capped at 100%");
amount.Set(false); quality.Set(false);
Check(Dirt(9800)==1.0,"disabled amount uses native curve");
Check(Cap(150)==9990,"disabled quality uses native threshold");
Check(MachinePurifier.GetMaxPurityCap(150)==9990,"native cap method agrees with game formula");

