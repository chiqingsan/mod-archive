using System;
using Il2Cpp;

namespace AutoStorage;

internal static class Owned
{
	private static bool _warned;

	public static bool Is(GameItem it)
	{
		if (it == null)
		{
			return false;
		}
		try
		{
			return GeneralHelper.IsItemEffectivelyOwned(it);
		}
		catch (Exception ex)
		{
			if (!_warned)
			{
				_warned = true;
				Core.Log.Warning("[AutoStorage] IsItemEffectivelyOwned 调用失败，本次会话不再校验所有权: " + ex.Message);
			}
			return true;
		}
	}
}
