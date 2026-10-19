using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;
using MelonLoader.Utils;

namespace XIAOWOTradePerks;

internal static class ModManager
{
	internal const string DisabledDirectoryName = "Disabled";

	internal static string ModsDirectory => MelonEnvironment.ModsDirectory;

	internal static string DisabledDirectory => Path.Combine(ModsDirectory, "Disabled");

	internal static IReadOnlyList<ManagedModEntry> Scan()
	{
		string fullPath = Path.GetFullPath(ModsDirectory);
		string fullPath2 = Path.GetFullPath(DisabledDirectory);
		string selfPath = NormalizePath(typeof(Core).Assembly.Location);
		Dictionary<string, List<MelonMod>> loadedModsByPath = GetLoadedModsByPath();
		List<ManagedModEntry> list = new List<ManagedModEntry>();
		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		AddFiles(list, hashSet, loadedModsByPath, fullPath, enabledNextLaunch: true, selfPath, null);
		AddFiles(list, hashSet, loadedModsByPath, fullPath2, enabledNextLaunch: false, selfPath, fullPath);
		foreach (KeyValuePair<string, List<MelonMod>> item in loadedModsByPath)
		{
			if (!hashSet.Contains(item.Key))
			{
				list.Add(BuildEntry(item.Key, File.Exists(item.Key) && IsImmediateChild(item.Key, fullPath), canToggle: false, item.Value, selfPath));
			}
		}
		return list.OrderByDescending((ManagedModEntry entry) => entry.IsSelf).ThenBy<ManagedModEntry, string>((ManagedModEntry entry) => entry.Name, StringComparer.CurrentCultureIgnoreCase).ThenBy<ManagedModEntry, string>((ManagedModEntry entry) => entry.FileName, StringComparer.CurrentCultureIgnoreCase)
			.ToArray();
	}

	internal static bool TrySetEnabled(ManagedModEntry entry, bool enabled, out string message)
	{
		if (entry == null)
		{
			message = Select("未找到该 MOD。", "The mod entry is unavailable.");
			return false;
		}
		if (entry.IsSelf)
		{
			message = Select("主 MOD 不能停用。", "The core mod cannot be disabled.");
			return false;
		}
		if (!entry.CanToggle)
		{
			message = Select("当前仅支持管理 Mods 根目录中的 DLL。", "Only DLLs in the Mods root are currently manageable.");
			return false;
		}
		if (entry.EnabledNextLaunch == enabled)
		{
			message = Select("状态没有变化。", "The state is unchanged.");
			return true;
		}
		string directory = (enabled ? DisabledDirectory : ModsDirectory);
		string text = (enabled ? ModsDirectory : DisabledDirectory);
		string fullPath = Path.GetFullPath(entry.FilePath);
		if (!IsImmediateChild(fullPath, directory))
		{
			message = Select("拒绝移动不在受管目录中的文件。", "Refused to move a file outside the managed directories.");
			return false;
		}
		if (!File.Exists(fullPath))
		{
			message = Select("DLL 文件已经不存在。", "The DLL file no longer exists.");
			return false;
		}
		string text2 = Path.Combine(text, Path.GetFileName(fullPath));
		if (File.Exists(text2))
		{
			message = Select("目标位置已有同名 DLL，未执行覆盖。", "A DLL with the same name already exists at the destination.");
			return false;
		}
		try
		{
			Directory.CreateDirectory(text);
			File.Move(fullPath, text2);
			entry.FilePath = text2;
			entry.EnabledNextLaunch = enabled;
			message = (enabled ? Select("已设为下次启动时启用。", "Enabled for the next launch.") : Select("已设为下次启动时停用。", "Disabled for the next launch."));
			return true;
		}
		catch (Exception ex)
		{
			message = Select("操作失败：", "Operation failed: ") + ex.Message;
			Instance log = Core.Log;
			if (log != null)
			{
				log.Warning("Mod manager could not change '" + entry.FileName + "': " + ex);
			}
			return false;
		}
	}

	internal static string GetStatus(ManagedModEntry entry)
	{
		if (entry.IsSelf)
		{
			return Select("主 MOD / 已锁定", "Core mod / locked");
		}
		if (entry.LoadedNow && entry.EnabledNextLaunch)
		{
			return Select("本次已加载", "Loaded now");
		}
		if (entry.LoadedNow)
		{
			return Select("本次仍已加载 / 下次停用", "Loaded now / disabled next launch");
		}
		if (entry.EnabledNextLaunch)
		{
			if (!entry.IsRecognized)
			{
				return Select("未注册或加载失败", "Unregistered or failed to load");
			}
			return Select("下次启用", "Enabled next launch");
		}
		return Select("已停用", "Disabled");
	}

	internal static bool IsImmediateChild(string filePath, string directory)
	{
		if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(directory))
		{
			return false;
		}
		string directoryName = Path.GetDirectoryName(Path.GetFullPath(filePath));
		if (string.IsNullOrWhiteSpace(directoryName))
		{
			return false;
		}
		return string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(directoryName)), Path.TrimEndingDirectorySeparator(Path.GetFullPath(directory)), StringComparison.OrdinalIgnoreCase);
	}

	private static void AddFiles(ICollection<ManagedModEntry> entries, ISet<string> seen, IReadOnlyDictionary<string, List<MelonMod>> loadedByPath, string directory, bool enabledNextLaunch, string selfPath, string loadedAliasDirectory)
	{
		if (!Directory.Exists(directory))
		{
			return;
		}
		string[] files = Directory.GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly);
		for (int i = 0; i < files.Length; i++)
		{
			string text = NormalizePath(files[i]);
			loadedByPath.TryGetValue(text, out var value);
			if ((value == null || value.Count == 0) && !string.IsNullOrWhiteSpace(loadedAliasDirectory))
			{
				string text2 = NormalizePath(Path.Combine(loadedAliasDirectory, Path.GetFileName(text)));
				if (loadedByPath.TryGetValue(text2, out var value2) && value2.Count > 0)
				{
					value = value2;
					seen.Add(text2);
				}
			}
			entries.Add(BuildEntry(text, enabledNextLaunch, canToggle: true, value, selfPath));
			seen.Add(text);
		}
	}

	private static ManagedModEntry BuildEntry(string path, bool enabledNextLaunch, bool canToggle, List<MelonMod> loaded, string selfPath)
	{
		ManagedModEntry managedModEntry = new ManagedModEntry
		{
			FilePath = path,
			EnabledNextLaunch = enabledNextLaunch,
			CanToggle = canToggle,
			LoadedNow = (loaded != null && loaded.Count > 0),
			IsRecognized = (loaded != null && loaded.Count > 0),
			IsSelf = string.Equals(path, selfPath, StringComparison.OrdinalIgnoreCase)
		};
		if (loaded != null && loaded.Count > 0)
		{
			managedModEntry.Name = string.Join(" + ", from name in loaded.Select(delegate(MelonMod mod)
				{
					MelonInfoAttribute info3 = ((MelonBase)mod).Info;
					return (info3 == null) ? null : info3.Name;
				})
				where !string.IsNullOrWhiteSpace(name)
				select name);
			managedModEntry.Version = string.Join(" / ", (from version in loaded.Select(delegate(MelonMod mod)
				{
					MelonInfoAttribute info2 = ((MelonBase)mod).Info;
					return (info2 == null) ? null : info2.Version;
				})
				where !string.IsNullOrWhiteSpace(version)
				select version).Distinct());
			managedModEntry.Author = string.Join(" / ", (from author in loaded.Select(delegate(MelonMod mod)
				{
					MelonInfoAttribute info = ((MelonBase)mod).Info;
					return (info == null) ? null : info.Author;
				})
				where !string.IsNullOrWhiteSpace(author)
				select author).Distinct());
		}
		if (string.IsNullOrWhiteSpace(managedModEntry.Name))
		{
			managedModEntry.Name = Path.GetFileNameWithoutExtension(path);
			TryReadAssemblyIdentity(path, managedModEntry);
		}
		if (managedModEntry.IsSelf)
		{
			managedModEntry.CanToggle = false;
		}
		return managedModEntry;
	}

	private static Dictionary<string, List<MelonMod>> GetLoadedModsByPath()
	{
		Dictionary<string, List<MelonMod>> dictionary = new Dictionary<string, List<MelonMod>>(StringComparer.OrdinalIgnoreCase);
		foreach (MelonBase registeredMelon in MelonBase.RegisteredMelons)
		{
			MelonMod val = (MelonMod)(object)((registeredMelon is MelonMod) ? registeredMelon : null);
			if (val == null)
			{
				continue;
			}
			MelonAssembly melonAssembly = ((MelonBase)val).MelonAssembly;
			if (!string.IsNullOrWhiteSpace((melonAssembly != null) ? melonAssembly.Location : null))
			{
				string key = NormalizePath(((MelonBase)val).MelonAssembly.Location);
				if (!dictionary.TryGetValue(key, out var value))
				{
					value = (dictionary[key] = new List<MelonMod>());
				}
				value.Add(val);
			}
		}
		return dictionary;
	}

	private static void TryReadAssemblyIdentity(string path, ManagedModEntry entry)
	{
		try
		{
			if (File.Exists(path))
			{
				AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);
				if (!string.IsNullOrWhiteSpace(assemblyName.Name))
				{
					entry.Name = assemblyName.Name;
				}
				if (assemblyName.Version != null)
				{
					entry.Version = assemblyName.Version.ToString();
				}
				entry.IsRecognized = true;
			}
		}
		catch
		{
			entry.IsRecognized = false;
		}
	}

	private static string NormalizePath(string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return string.Empty;
		}
		if (!Path.IsPathRooted(path))
		{
			path = Path.Combine(AppContext.BaseDirectory, path);
		}
		return Path.GetFullPath(path);
	}

	private static string Select(string chinese, string english)
	{
		return ModLocalization.Select(chinese, english);
	}
}
