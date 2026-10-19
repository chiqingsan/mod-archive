using Il2Cpp;

namespace AutoStorage;

internal static class Stolen
{
	public static bool Is(GameItem it)
	{
		if (it == null)
		{
			return false;
		}
		try
		{
			return StolenHelper.IsStolenItem(it);
		}
		catch
		{
			return false;
		}
	}

	public static string Tooltip(GameItem it)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		if (it == null)
		{
			return "";
		}
		try
		{
			RichTextBuilder val = new RichTextBuilder(100);
			StolenHelper.CreateStolenItemTooltip(val, it);
			RichText val2 = val.Build(-1);
			if (val2 == null)
			{
				return "";
			}
			string nakedText = val2.nakedText;
			return string.IsNullOrEmpty(nakedText) ? "" : nakedText.Trim();
		}
		catch
		{
			return "";
		}
	}
}
