using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ModCapability
{
	public const string Files = "files";

	public const string Network = "network";

	public const string Process = "process";

	public const string DynamicCode = "dynamic-code";

	public const string Native = "native";

	public const string ReflectionExternal = "reflection-external";

	public static readonly string[] All = new string[6] { "files", "network", "process", "dynamic-code", "native", "reflection-external" };

	public static bool IsKnown(string capability)
	{
		return All.Contains(capability);
	}

	public static string DisplayName(string capability)
	{
		return capability switch
		{
			"files" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_files", "Read and write files anywhere on this computer"), 
			"network" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_network", "Access the internet"), 
			"process" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_process", "Start other programs or open links"), 
			"dynamic-code" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_dynamic_code", "Load or generate code at runtime"), 
			"native" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_native", "Call native system libraries"), 
			"reflection-external" => LocHelper.GetLocalizedUIOrDefault("ui_mods_cap_reflection_external", "Reach into system and engine internals through reflection"), 
			_ => capability, 
		};
	}

	public static string FormatList(IEnumerable<string> capabilities)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item in Order(capabilities))
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append('\n');
			}
			stringBuilder.Append("- ").Append(DisplayName(item));
		}
		return stringBuilder.ToString();
	}

	public static string FormatIds(IEnumerable<string> capabilities)
	{
		List<string> list = Order(capabilities).ToList();
		if (list.Count != 0)
		{
			return string.Join(", ", list);
		}
		return "none";
	}

	public static IEnumerable<string> Order(IEnumerable<string> capabilities)
	{
		HashSet<string> hashSet = new HashSet<string>(capabilities ?? Enumerable.Empty<string>());
		return All.Where(hashSet.Contains).Concat(from c in hashSet
			where !IsKnown(c)
			orderby c
			select c);
	}

	public static List<string> Undeclared(IEnumerable<string> detected, IEnumerable<string> declared)
	{
		HashSet<string> declaredSet = new HashSet<string>(declared ?? Enumerable.Empty<string>());
		return (from c in Order(detected)
			where !declaredSet.Contains(c)
			select c).ToList();
	}
}
