namespace AutoStorage;

internal sealed class CfgEntry
{
	public bool Enabled;

	public string White = "";

	public string Black = "";

	public bool AllowStolen = true;

	public bool StolenPriority;

	public CfgEntry Clone()
	{
		return new CfgEntry
		{
			Enabled = Enabled,
			White = White,
			Black = Black,
			AllowStolen = AllowStolen,
			StolenPriority = StolenPriority
		};
	}

	public string FlagsSegment()
	{
		return "a" + (AllowStolen ? "1" : "0") + "p" + (StolenPriority ? "1" : "0");
	}

	public void ParseFlags(string seg)
	{
		if (!string.IsNullOrEmpty(seg) && seg.Length >= 4 && seg[0] == 'a' && seg[2] == 'p')
		{
			AllowStolen = seg[1] != '0';
			StolenPriority = seg[3] == '1';
		}
	}
}
