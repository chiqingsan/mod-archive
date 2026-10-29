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
		if (Grid != null && Button != null && PreserveButton != null && ReferenceEquals(_window, sortWindow) && ReferenceEquals(val, Grid))
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
		RichTextElement val4 = MakeButton(text3);
		width = Math.Max(width, Math.Max(val3.widthPixels, val4.widthPixels));
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetFixedSize(width, -1);
		}
		GridPixelElement val5 = new GridPixelElement(1, 7, false).SetSpacing(ContextAccess.GridXSpacing(val2), ContextAccess.GridYSpacing(val2));
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
			if (!val5.AttachPos(val3, 0, 4) || !val5.AttachPos(val4, 0, 5) || !val5.AttachPos(array[4], 0, 6))
			{
				throw new InvalidOperationException("Failed to attach smart sort rows.");
			}
			if (!sortWindow.Attach(val5))
			{
				throw new InvalidOperationException("Failed to attach the expanded 1x7 sort grid.");
			}
			sortWindow.ResizePixels(width + 2 * sortWindow.xPadding, heightPixels + 2 * (rowHeight + ContextAccess.GridYSpacing(val2)));
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
		val6 = new DefaultInterpolatedStringHandler(99, 7);
		val6.AppendLiteral("Smart arrange menu injected: label=");
		val6.AppendFormatted(text2);
		val6.AppendLiteral(", preserveLabel=");
		val6.AppendFormatted(text3);
		val6.AppendLiteral(", nativeGrid=");
		val6.AppendFormatted<int>(val2.gridWidth);
		val6.AppendLiteral("x");
		val6.AppendFormatted<int>(val2.gridHeight);
		val6.AppendLiteral(", expandedGrid=1x7, row=");
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
