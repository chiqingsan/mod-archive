using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace XIAOWOTradePerks;

internal static class GameWindowResizer
{
	private delegate bool EnumWindowsProc(IntPtr window, IntPtr parameter);

	private struct NativeRect
	{
		internal int Left;

		internal int Top;

		internal int Right;

		internal int Bottom;

		internal int Width => Right - Left;

		internal int Height => Bottom - Top;

		internal NativeRect(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}
	}

	private const int GwlStyle = -16;

	private const int GwlExStyle = -20;

	private const uint WsThickFrame = 262144u;

	private const uint WsMaximizeBox = 65536u;

	private const uint SwpNoSize = 1u;

	private const uint SwpNoMove = 2u;

	private const uint SwpNoZOrder = 4u;

	private const uint SwpNoActivate = 16u;

	private const uint SwpFrameChanged = 32u;

	private static IntPtr _window;

	private static int _tickCountdown;

	private static bool _styleApplied;

	private static int _testWidth;

	private static int _testHeight;

	internal static void Tick()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I4
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || _tickCountdown-- > 0)
		{
			return;
		}
		_tickCountdown = 30;
		if ((int)Screen.fullScreenMode != 3)
		{
			_styleApplied = false;
			return;
		}
		IntPtr intPtr = FindUnityWindow();
		if (intPtr == IntPtr.Zero)
		{
			return;
		}
		if (_window != intPtr)
		{
			_window = intPtr;
			_styleApplied = false;
		}
		uint windowStyle = GetWindowStyle(intPtr);
		uint num = windowStyle | 0x40000u | 0x10000u;
		if (num != windowStyle)
		{
			GetClientRect(intPtr, out var rect);
			SetWindowStyle(intPtr, num);
			NativeRect rect2 = new NativeRect(0, 0, rect.Width, rect.Height);
			if (AdjustWindowRectEx(ref rect2, num, hasMenu: false, GetWindowExStyle(intPtr)))
			{
				SetWindowPos(intPtr, IntPtr.Zero, 0, 0, rect2.Width, rect2.Height, 54u);
			}
			else
			{
				SetWindowPos(intPtr, IntPtr.Zero, 0, 0, 0, 0, 55u);
			}
		}
		_styleApplied = (GetWindowStyle(intPtr) & 0x50000) == 327680;
	}

	internal static string BuildStatus()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		IntPtr intPtr = FindUnityWindow();
		FullScreenMode fullScreenMode;
		if (intPtr == IntPtr.Zero)
		{
			fullScreenMode = Screen.fullScreenMode;
			return "GAME_WINDOW_RESIZE\nwindow=False mode=" + ((object)(FullScreenMode)(ref fullScreenMode)).ToString();
		}
		GetClientRect(intPtr, out var rect);
		uint windowStyle = GetWindowStyle(intPtr);
		string[] obj = new string[16]
		{
			"GAME_WINDOW_RESIZE\nwindow=True mode=", null, null, null, null, null, null, null, null, null,
			null, null, null, null, null, null
		};
		fullScreenMode = Screen.fullScreenMode;
		obj[1] = ((object)(FullScreenMode)(ref fullScreenMode)).ToString();
		obj[2] = "\nstyleApplied=";
		obj[3] = _styleApplied.ToString();
		obj[4] = " thickFrame=";
		obj[5] = ((windowStyle & 0x40000) != 0).ToString();
		obj[6] = " maximize=";
		obj[7] = ((windowStyle & 0x10000) != 0).ToString();
		obj[8] = "\nclient=";
		obj[9] = rect.Width.ToString();
		obj[10] = "x";
		obj[11] = rect.Height.ToString();
		obj[12] = "\nunity=";
		obj[13] = Screen.width.ToString();
		obj[14] = "x";
		obj[15] = Screen.height.ToString();
		return string.Concat(obj);
	}

	internal static string BeginResizeTest()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		TickNow();
		if ((int)Screen.fullScreenMode != 3)
		{
			FullScreenMode fullScreenMode = Screen.fullScreenMode;
			return "GAME_WINDOW_RESIZE_TEST SKIP: mode=" + ((object)(FullScreenMode)(ref fullScreenMode)).ToString();
		}
		IntPtr intPtr = FindUnityWindow();
		if (intPtr == IntPtr.Zero || !GetClientRect(intPtr, out var rect))
		{
			return "GAME_WINDOW_RESIZE_TEST FAIL: window unavailable";
		}
		_testWidth = rect.Width;
		_testHeight = rect.Height;
		int num = Math.Max(800, rect.Width - 96);
		int num2 = Math.Max(450, rect.Height - 54);
		if (num == rect.Width && num2 == rect.Height)
		{
			num = rect.Width + 96;
			num2 = rect.Height + 54;
		}
		if (!ResizeClient(intPtr, num, num2))
		{
			return "GAME_WINDOW_RESIZE_TEST FAIL: SetWindowPos failed";
		}
		return "GAME_WINDOW_RESIZE_TEST STARTED original=" + _testWidth + "x" + _testHeight + " target=" + num + "x" + num2;
	}

	internal static string RestoreResizeTest()
	{
		IntPtr intPtr = FindUnityWindow();
		if (intPtr == IntPtr.Zero || _testWidth <= 0 || _testHeight <= 0)
		{
			return "GAME_WINDOW_RESIZE_RESTORE SKIP";
		}
		bool flag = ResizeClient(intPtr, _testWidth, _testHeight);
		string result = "GAME_WINDOW_RESIZE_RESTORE " + (flag ? "PASS" : "FAIL") + " target=" + _testWidth + "x" + _testHeight;
		_testWidth = 0;
		_testHeight = 0;
		return result;
	}

	internal static void Dispose()
	{
		_window = IntPtr.Zero;
		_styleApplied = false;
		_testWidth = 0;
		_testHeight = 0;
	}

	private static void TickNow()
	{
		_tickCountdown = 0;
		Tick();
	}

	private static bool ResizeClient(IntPtr window, int width, int height)
	{
		NativeRect rect = new NativeRect(0, 0, width, height);
		if (!AdjustWindowRectEx(ref rect, GetWindowStyle(window), hasMenu: false, GetWindowExStyle(window)))
		{
			return false;
		}
		return SetWindowPos(window, IntPtr.Zero, 0, 0, rect.Width, rect.Height, 22u);
	}

	private static IntPtr FindUnityWindow()
	{
		if (_window != IntPtr.Zero && IsWindow(_window))
		{
			return _window;
		}
		uint processId = (uint)Process.GetCurrentProcess().Id;
		IntPtr fallback = IntPtr.Zero;
		IntPtr unity = IntPtr.Zero;
		EnumWindows(delegate(IntPtr window, IntPtr _)
		{
			GetWindowThreadProcessId(window, out var processId2);
			if (processId2 != processId || !IsWindowVisible(window))
			{
				return true;
			}
			fallback = window;
			StringBuilder stringBuilder = new StringBuilder(128);
			GetClassName(window, stringBuilder, stringBuilder.Capacity);
			if (!stringBuilder.ToString().Contains("UnityWndClass", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			unity = window;
			return false;
		}, IntPtr.Zero);
		_window = ((unity != IntPtr.Zero) ? unity : fallback);
		return _window;
	}

	private static uint GetWindowStyle(IntPtr window)
	{
		return (uint)((IntPtr.Size == 8) ? GetWindowLongPtr64(window, -16).ToInt64() : GetWindowLong32(window, -16));
	}

	private static uint GetWindowExStyle(IntPtr window)
	{
		return (uint)((IntPtr.Size == 8) ? GetWindowLongPtr64(window, -20).ToInt64() : GetWindowLong32(window, -20));
	}

	private static void SetWindowStyle(IntPtr window, uint style)
	{
		if (IntPtr.Size == 8)
		{
			SetWindowLongPtr64(window, -16, new IntPtr(style));
		}
		else
		{
			SetWindowLong32(window, -16, (int)style);
		}
	}

	[DllImport("user32.dll")]
	private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr parameter);

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	private static extern int GetClassName(IntPtr window, StringBuilder className, int maximumCount);

	[DllImport("user32.dll")]
	private static extern bool IsWindow(IntPtr window);

	[DllImport("user32.dll")]
	private static extern bool IsWindowVisible(IntPtr window);

	[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
	private static extern int GetWindowLong32(IntPtr window, int index);

	[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
	private static extern IntPtr GetWindowLongPtr64(IntPtr window, int index);

	[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
	private static extern int SetWindowLong32(IntPtr window, int index, int value);

	[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
	private static extern IntPtr SetWindowLongPtr64(IntPtr window, int index, IntPtr value);

	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(IntPtr window, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

	[DllImport("user32.dll")]
	private static extern bool AdjustWindowRectEx(ref NativeRect rect, uint style, bool hasMenu, uint extendedStyle);

	[DllImport("user32.dll")]
	private static extern bool GetClientRect(IntPtr window, out NativeRect rect);
}
