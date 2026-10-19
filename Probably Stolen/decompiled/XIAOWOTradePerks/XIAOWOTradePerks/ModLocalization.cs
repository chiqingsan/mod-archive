using System;
using Il2Cpp;

namespace XIAOWOTradePerks;

internal static class ModLocalization
{
	internal static string CurrentLocaleCode
	{
		get
		{
			try
			{
				return LocHelper.GetCurrentLocaleCode() ?? string.Empty;
			}
			catch
			{
				return string.Empty;
			}
		}
	}

	internal static bool IsChinese(string localeCode)
	{
		if (string.IsNullOrWhiteSpace(localeCode))
		{
			return false;
		}
		if (!localeCode.Equals("zh", StringComparison.OrdinalIgnoreCase) && !localeCode.StartsWith("zh-", StringComparison.OrdinalIgnoreCase))
		{
			return localeCode.StartsWith("zh_", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	internal static string Select(string chinese, string english)
	{
		return Select(CurrentLocaleCode, chinese, english);
	}

	internal static string Select(string localeCode, string chinese, string english)
	{
		if (!IsChinese(localeCode))
		{
			return english;
		}
		return chinese;
	}
}
