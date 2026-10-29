
namespace SmartInventoryArrange.Core;

public static class InstanceIdentity
{
	public static string From(string itemId, int uniqueId, int index, int multiplicity)
	{
		DefaultInterpolatedStringHandler val = default(DefaultInterpolatedStringHandler);
		val = new DefaultInterpolatedStringHandler(1, 2);
		val.AppendFormatted(itemId);
		val.AppendLiteral("\u001f");
		val.AppendFormatted<int>(uniqueId, "D10");
		string text = val.ToStringAndClear();
		if (uniqueId == 0 || multiplicity != 1)
		{
			val = new DefaultInterpolatedStringHandler(1, 2);
			val.AppendFormatted(text);
			val.AppendLiteral("\u001f");
			val.AppendFormatted<int>(index, "D6");
			return val.ToStringAndClear();
		}
		return text;
	}
}
