using System.IO;

namespace XIAOWOTradePerks;

internal sealed class ManagedModEntry
{
	internal string Name { get; set; } = string.Empty;


	internal string Version { get; set; } = string.Empty;


	internal string Author { get; set; } = string.Empty;


	internal string FilePath { get; set; } = string.Empty;


	internal bool LoadedNow { get; set; }

	internal bool EnabledNextLaunch { get; set; }

	internal bool IsSelf { get; set; }

	internal bool CanToggle { get; set; }

	internal bool IsRecognized { get; set; }

	internal string FileName => Path.GetFileName(FilePath);
}
