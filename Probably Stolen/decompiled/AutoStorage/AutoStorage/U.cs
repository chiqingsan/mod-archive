using System;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;

namespace AutoStorage;

internal static class U
{
	public static IntPtr Ptr(Il2CppObjectBase o)
	{
		if (o == null)
		{
			return IntPtr.Zero;
		}
		try
		{
			return o.Pointer;
		}
		catch
		{
			return IntPtr.Zero;
		}
	}

	public static bool Same(Il2CppObjectBase a, Il2CppObjectBase b)
	{
		IntPtr intPtr = Ptr(a);
		IntPtr intPtr2 = Ptr(b);
		if (intPtr != IntPtr.Zero)
		{
			return intPtr == intPtr2;
		}
		return false;
	}

	public static string SafeId(GameItem it)
	{
		if (it == null)
		{
			return "";
		}
		try
		{
			if (!string.IsNullOrEmpty(it.identifier))
			{
				return it.identifier;
			}
		}
		catch
		{
		}
		try
		{
			return it.name ?? "";
		}
		catch
		{
		}
		return "";
	}

	public static string SafeName(GameItem it)
	{
		if (it == null)
		{
			return "";
		}
		try
		{
			string displayName = it.GetDisplayName(false);
			if (!string.IsNullOrEmpty(displayName))
			{
				return displayName;
			}
		}
		catch
		{
		}
		return SafeId(it);
	}

	public static int SafeUid(GameItem it)
	{
		if (it == null)
		{
			return 0;
		}
		try
		{
			return it.uniqueId;
		}
		catch
		{
		}
		try
		{
			return it.GetUniqueID();
		}
		catch
		{
		}
		return 0;
	}

	public static string Join(string[] a)
	{
		if (a != null)
		{
			return string.Join(",", a);
		}
		return "";
	}
}
