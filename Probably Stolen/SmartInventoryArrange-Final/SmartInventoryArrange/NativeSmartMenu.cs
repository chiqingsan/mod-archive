using System;
using System.Linq;
using UnityEngine;

namespace SmartInventoryArrange;

internal static class NativeSmartMenu
{
	internal static RichTextElement? Button;

	internal static RichTextElement? PreserveButton;

	internal static GridPixelElement? Grid;

	private static PixelWindow? _window;

	internal static void Open(ItemContextHandler owner)
	{
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		ItemContextHandler owner2 = owner;
		PixelWindow sortWindow = ContextAccess.SortWindow(owner2);
		PixelElement val = sortWindow.childElement ?? throw new InvalidOperationException("Sort window has no child element.");
		bool preserveEnabled = SmartArrangeMod.EnablePreserveContainers?.Value ?? false;
		if (Grid != null && Button != null && ReferenceEquals(_window, sortWindow) && ReferenceEquals(val, Grid) && (PreserveButton != null) == preserveEnabled)
		{
			return;
		}
		Close();
		GridPixelElement val2 = (GridPixelElement)val;
		RichTextElement[] array = new RichTextElement[5] { ContextAccess.SortBigStart(owner2), ContextAccess.SortSmallStart(owner2), ContextAccess.SortBigEnd(owner2), ContextAccess.SortSmallEnd(owner2), ContextAccess.SortUndo(owner2) };
		if (Enumerable.Any(array, (RichTextElement x) => x == null))
		{
			throw new InvalidOperationException("One or more native sort buttons are missing.");
		}
		RichTextElement source = ContextAccess.SortSmallEnd(owner2);
		int rowHeight = Math.Max(1, source.heightPixels);
		int contentWidth = Math.Max(0, sortWindow.widthPixels - 2 * sortWindow.xPadding);
		int width = Math.Max(contentWidth, Enumerable.Max(array, (RichTextElement x) => x.widthPixels));
		string text2 = (ContextInitPatch.IsChinese() ? "智能排列" : "Smart Arrange");
		string text3 = (ContextInitPatch.IsChinese() ? "保留容器排列" : "Preserve Containers");
		RichTextElement val3 = MakeButton(text2);
		RichTextElement? val4 = (preserveEnabled ? MakeButton(text3) : null);
		width = Math.Max(width, val3.widthPixels);
		if (val4 != null)
		{
			width = Math.Max(width, val4.widthPixels);
		}
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetFixedSize(width, -1);
		}
		GridPixelElement val5 = new GridPixelElement(1, preserveEnabled ? 7 : 6, false).SetSpacing(ContextAccess.GridXSpacing(val2), ContextAccess.GridYSpacing(val2));
		val5.justifyWidth = val2.justifyWidth;
		val5.searchUsingPriority = val2.searchUsingPriority;
		int widthPixels = sortWindow.widthPixels;
		int heightPixels = sortWindow.heightPixels;
		if (!sortWindow.Detach(val))
		{
			throw new InvalidOperationException("Failed to detach the native sort grid.");
		}
		val2.DetachAll();
		DefaultInterpolatedStringHandler val6 = default(DefaultInterpolatedStringHandler);
		try
		{
			for (int i = 0; i < array.Length - 1; i++)
			{
				if (!val5.AttachPos(array[i], 0, i))
				{
					val6 = new DefaultInterpolatedStringHandler(34, 1);
					val6.AppendLiteral("Failed to attach native sort row ");
					val6.AppendFormatted<int>(i);
					val6.AppendLiteral(".");
					throw new InvalidOperationException(val6.ToStringAndClear());
				}
			}
			bool attached = val5.AttachPos(val3, 0, 4);
			if (attached && val4 != null)
			{
				attached = val5.AttachPos(val4, 0, 5);
			}
			if (attached)
			{
				attached = val5.AttachPos(array[4], 0, preserveEnabled ? 6 : 5);
			}
			if (!attached)
			{
				throw new InvalidOperationException("Failed to attach smart sort rows.");
			}
			if (!sortWindow.Attach(val5))
			{
				throw new InvalidOperationException("Failed to attach the expanded sort grid.");
			}
			sortWindow.ResizePixels(width + 2 * sortWindow.xPadding, heightPixels + (preserveEnabled ? 2 : 1) * (rowHeight + ContextAccess.GridYSpacing(val2)));
			sortWindow.Validate();
			Button = val3;
			PreserveButton = val4;
			Grid = val5;
			_window = sortWindow;
		}
		catch
		{
			sortWindow.Detach(val5);
			val5.DetachAll();
			for (int j = 0; j < array.Length; j++)
			{
				val2.AttachPos(array[j], 0, j);
			}
			sortWindow.Attach(val);
			sortWindow.ResizePixels(widthPixels, heightPixels);
			sortWindow.Validate();
			Close();
			throw;
		}
		val6 = new DefaultInterpolatedStringHandler(110, 8);
		val6.AppendLiteral("Smart arrange menu injected: label=");
		val6.AppendFormatted(text2);
		val6.AppendLiteral(", preserve=");
		val6.AppendFormatted<bool>(preserveEnabled);
		val6.AppendLiteral(", nativeGrid=");
		val6.AppendFormatted<int>(val2.gridWidth);
		val6.AppendLiteral("x");
		val6.AppendFormatted<int>(val2.gridHeight);
		val6.AppendLiteral(", expandedGrid=1x");
		val6.AppendFormatted<int>(preserveEnabled ? 7 : 6);
		val6.AppendLiteral(", row=");
		val6.AppendFormatted<int>(rowHeight);
		val6.AppendLiteral(", window=");
		val6.AppendFormatted<int>(sortWindow.widthPixels);
		val6.AppendLiteral("x");
		val6.AppendFormatted<int>(sortWindow.heightPixels);
		Debug.Log(val6.ToStringAndClear());
		RichTextElement MakeButton(string text)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected O, but got Unknown
			RichTextElement val7 = new RichTextElement(false, width, rowHeight, true);
			val7.SetPadding(ContextAccess.LeftPadding(source), ContextAccess.RightPadding(source), ContextAccess.TopPadding(source), ContextAccess.BottomPadding(source));
			val7.SetText(text, Math.Max(source.maxCharWidth, text.Length), ContextAccess.TextColor(owner2));
			val7.Validate();
			return val7;
		}
	}

	internal static bool IsCurrent(ItemContextHandler owner)
	{
		if (Grid != null && ContextAccess.SortWindow(owner) != null && ReferenceEquals(ContextAccess.SortWindow(owner), _window))
		{
			return ReferenceEquals(ContextAccess.SortWindow(owner).childElement, Grid);
		}
		return false;
	}

	internal static void Close()
	{
		Button = null;
		PreserveButton = null;
		Grid = null;
		_window = null;
	}
}
