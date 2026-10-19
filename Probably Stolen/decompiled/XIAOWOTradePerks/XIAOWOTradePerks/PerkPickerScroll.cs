using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using MelonLoader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XIAOWOTradePerks;

internal static class PerkPickerScroll
{
	private sealed class ScrollArea
	{
		private readonly string _contentName;

		private readonly string _scrollbarName;

		private GameObject _root;

		private GameObject _content;

		private ScrollRect _scroll;

		private Scrollbar _scrollbar;

		private GridLayoutGroup _grid;

		private float _savedPosition = 1f;

		private int _lastPerkCount = -1;

		internal ScrollArea(string contentName, string scrollbarName)
		{
			_contentName = contentName;
			_scrollbarName = scrollbarName;
		}

		internal bool IsAttached(GameObject current)
		{
			if (!string.Equals((current != null) ? ((Object)current).name : null, _contentName, StringComparison.Ordinal))
			{
				return (Object)(object)((current != null) ? current.transform.Find(_contentName) : null) != (Object)null;
			}
			return true;
		}

		internal void RequestTop()
		{
			_savedPosition = 1f;
		}

		internal GameObject Ensure(GameObject current)
		{
			if (!Resolve(current))
			{
				Create(current);
			}
			if (!Resolve(current))
			{
				return null;
			}
			MoveDirectPerksToContent(_root, _content);
			Refresh();
			return _content;
		}

		internal void Capture(GameObject current)
		{
			if (Resolve(current))
			{
				_savedPosition = (HasOverflow() ? _scroll.verticalNormalizedPosition : 1f);
			}
		}

		internal void ForceTop()
		{
			_savedPosition = 1f;
			if ((Object)(object)_scroll != (Object)null)
			{
				_scroll.verticalNormalizedPosition = 1f;
			}
			if ((Object)(object)_scrollbar != (Object)null)
			{
				_scrollbar.value = 1f;
			}
		}

		internal void RefreshIfCountChanged()
		{
			if (!IsAlive((Object)(object)_root) || !IsAlive((Object)(object)_content))
			{
				return;
			}
			MoveDirectPerksToContent(_root, _content);
			if (CountDirectPerks(_content.transform) != _lastPerkCount)
			{
				if (HasOverflow())
				{
					_savedPosition = _scroll.verticalNormalizedPosition;
				}
				Refresh();
			}
		}

		internal void Refresh()
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			if (!IsAlive((Object)(object)_root) || !IsAlive((Object)(object)_content) || !IsAlive((Object)(object)_scroll) || !IsAlive((Object)(object)_scrollbar) || !IsAlive((Object)(object)_grid))
			{
				return;
			}
			RectTransform component = _root.GetComponent<RectTransform>();
			RectTransform component2 = _content.GetComponent<RectTransform>();
			if (!((Object)(object)component == (Object)null) && !((Object)(object)component2 == (Object)null))
			{
				Rect rect = component.rect;
				float num = Math.Max(1f, ((Rect)(ref rect)).width);
				float num2 = Math.Max(_grid.cellSize.x, num - 14f - 6f);
				int num3 = Math.Max(1, Mathf.FloorToInt((num2 + _grid.spacing.x) / (_grid.cellSize.x + _grid.spacing.x)));
				int num4 = Math.Max(1, ((_lastPerkCount = CountDirectPerks(_content.transform)) + num3 - 1) / num3);
				float num5 = (float)(((LayoutGroup)_grid).padding.top + ((LayoutGroup)_grid).padding.bottom) + (float)num4 * _grid.cellSize.y + (float)Math.Max(0, num4 - 1) * _grid.spacing.y;
				rect = component.rect;
				float num6 = Math.Max(((Rect)(ref rect)).height, num5);
				_grid.constraint = (Constraint)1;
				_grid.constraintCount = num3;
				component2.anchorMin = new Vector2(0f, 1f);
				component2.anchorMax = new Vector2(1f, 1f);
				component2.pivot = new Vector2(0f, 1f);
				component2.anchoredPosition = Vector2.zero;
				component2.sizeDelta = new Vector2(0f, num6);
				LayoutRebuilder.ForceRebuildLayoutImmediate(component2);
				Canvas.ForceUpdateCanvases();
				rect = component.rect;
				bool flag = num5 > ((Rect)(ref rect)).height + 0.5f;
				if (!flag)
				{
					_savedPosition = 1f;
				}
				((Component)_scrollbar).gameObject.SetActive(flag);
				_scroll.verticalNormalizedPosition = Mathf.Clamp01(_savedPosition);
			}
		}

		internal bool Resolve(GameObject current)
		{
			if (IsAlive((Object)(object)_root) && IsAlive((Object)(object)_content) && IsAlive((Object)(object)_scroll) && IsAlive((Object)(object)_scrollbar) && IsAlive((Object)(object)_grid) && ((Object)(object)current == (Object)(object)_root || (Object)(object)current == (Object)(object)_content))
			{
				return true;
			}
			Clear();
			if ((Object)(object)current == (Object)null)
			{
				return false;
			}
			if (string.Equals(((Object)current).name, _contentName, StringComparison.Ordinal))
			{
				_content = current;
				Transform parent = current.transform.parent;
				_root = ((parent != null) ? ((Component)parent).gameObject : null);
			}
			else
			{
				_root = current;
				Transform obj = _root.transform.Find(_contentName);
				_content = ((obj != null) ? ((Component)obj).gameObject : null);
			}
			GameObject root = _root;
			_scroll = ((root != null) ? root.GetComponent<ScrollRect>() : null);
			GameObject root2 = _root;
			object scrollbar;
			if (root2 == null)
			{
				scrollbar = null;
			}
			else
			{
				Transform obj2 = root2.transform.Find(_scrollbarName);
				scrollbar = ((obj2 != null) ? ((Component)obj2).gameObject.GetComponent<Scrollbar>() : null);
			}
			_scrollbar = (Scrollbar)scrollbar;
			GameObject content = _content;
			_grid = ((content != null) ? content.GetComponent<GridLayoutGroup>() : null);
			if (IsAlive((Object)(object)_root) && IsAlive((Object)(object)_content) && IsAlive((Object)(object)_scroll) && IsAlive((Object)(object)_scrollbar))
			{
				return IsAlive((Object)(object)_grid);
			}
			return false;
		}

		internal string BuildStatus(string prefix)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			RectTransform component = _root.GetComponent<RectTransform>();
			RectTransform component2 = _content.GetComponent<RectTransform>();
			string[] obj = new string[25]
			{
				prefix, ".present=True\n", prefix, ".rootSize=", null, null, null, null, null, null,
				null, null, null, null, null, null, null, null, null, null,
				null, null, null, null, null
			};
			Rect rect;
			object obj2;
			if (!((Object)(object)component == (Object)null))
			{
				rect = component.rect;
				string text = ((Rect)(ref rect)).width.ToString();
				rect = component.rect;
				obj2 = text + "x" + ((Rect)(ref rect)).height;
			}
			else
			{
				obj2 = "missing";
			}
			obj[4] = (string)obj2;
			obj[5] = "\n";
			obj[6] = prefix;
			obj[7] = ".contentSize=";
			object obj3;
			if (!((Object)(object)component2 == (Object)null))
			{
				rect = component2.rect;
				string text2 = ((Rect)(ref rect)).width.ToString();
				rect = component2.rect;
				obj3 = text2 + "x" + ((Rect)(ref rect)).height;
			}
			else
			{
				obj3 = "missing";
			}
			obj[8] = (string)obj3;
			obj[9] = "\n";
			obj[10] = prefix;
			obj[11] = ".columns=";
			GridLayoutGroup grid = _grid;
			obj[12] = ((grid != null) ? grid.constraintCount : 0).ToString();
			obj[13] = "\n";
			obj[14] = prefix;
			obj[15] = ".perkCount=";
			obj[16] = CountDirectPerks(_content.transform).ToString();
			obj[17] = "\n";
			obj[18] = prefix;
			obj[19] = ".scrollbarVisible=";
			Scrollbar scrollbar = _scrollbar;
			obj[20] = (scrollbar != null && ((Component)scrollbar).gameObject.activeInHierarchy).ToString();
			obj[21] = "\n";
			obj[22] = prefix;
			obj[23] = ".position=";
			obj[24] = _scroll.verticalNormalizedPosition.ToString();
			return string.Concat(obj);
		}

		internal bool SetPosition(GameObject current, float position)
		{
			if (!Resolve(current))
			{
				return false;
			}
			_savedPosition = Mathf.Clamp01(position);
			_scroll.verticalNormalizedPosition = _savedPosition;
			_forceTopFrames = 0;
			return true;
		}

		internal bool SetRawPosition(GameObject current, float position)
		{
			if (!Resolve(current))
			{
				return false;
			}
			_scrollbar.value = Mathf.Clamp01(position);
			Canvas.ForceUpdateCanvases();
			return true;
		}

		internal float GetPosition(GameObject current)
		{
			if (!Resolve(current))
			{
				return -1f;
			}
			if (!HasOverflow())
			{
				return 1f;
			}
			return _scroll.verticalNormalizedPosition;
		}

		internal bool IsScrollbarVisible(GameObject current)
		{
			if (Resolve(current))
			{
				return ((Component)_scrollbar).gameObject.activeInHierarchy;
			}
			return false;
		}

		internal bool TestScrollbarValue(GameObject current, float position, out float actual)
		{
			actual = -1f;
			if (!Resolve(current))
			{
				return false;
			}
			_scrollbar.value = Mathf.Clamp01(position);
			Canvas.ForceUpdateCanvases();
			actual = _scroll.verticalNormalizedPosition;
			_savedPosition = actual;
			return Math.Abs(actual - Mathf.Clamp01(position)) < 0.01f;
		}

		internal bool TestMouseWheel(GameObject current, float delta, out float before, out float after)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			before = -1f;
			after = -1f;
			if (!Resolve(current) || (Object)(object)EventSystem.current == (Object)null)
			{
				return false;
			}
			before = _scroll.verticalNormalizedPosition;
			PointerEventData val = new PointerEventData(EventSystem.current)
			{
				scrollDelta = new Vector2(0f, delta)
			};
			_scroll.OnScroll(val);
			Canvas.ForceUpdateCanvases();
			after = _scroll.verticalNormalizedPosition;
			_savedPosition = after;
			return Math.Abs(after - before) > 0.001f;
		}

		private void Create(GameObject root)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Expected O, but got Unknown
			RectTransform val = ((root != null) ? root.GetComponent<RectTransform>() : null);
			GridLayoutGroup val2 = ((root != null) ? root.GetComponent<GridLayoutGroup>() : null);
			if (!((Object)(object)root == (Object)null) && !((Object)(object)val == (Object)null) && !((Object)(object)val2 == (Object)null))
			{
				_root = root;
				((Behaviour)val2).enabled = false;
				if (_root.GetComponent<RectMask2D>() == null)
				{
					_root.AddComponent<RectMask2D>();
				}
				_content = new GameObject(_contentName, (Type[])(object)new Type[2]
				{
					Il2CppType.Of<RectTransform>(),
					Il2CppType.Of<GridLayoutGroup>()
				});
				_content.transform.SetParent(_root.transform, false);
				_grid = _content.GetComponent<GridLayoutGroup>();
				CopyGrid(val2, _grid);
				MoveDirectPerksToContent(_root, _content);
				_scrollbar = CreateScrollbar(_root.transform, _scrollbarName);
				_scroll = _root.GetComponent<ScrollRect>() ?? _root.AddComponent<ScrollRect>();
				_scroll.content = _content.GetComponent<RectTransform>();
				_scroll.viewport = val;
				_scroll.horizontal = false;
				_scroll.vertical = true;
				_scroll.movementType = (MovementType)2;
				_scroll.inertia = false;
				_scroll.scrollSensitivity = 48f;
				_scroll.verticalScrollbar = _scrollbar;
				_scroll.verticalScrollbarVisibility = (ScrollbarVisibility)1;
				ForceTop();
				Instance log = Core.Log;
				if (log != null)
				{
					log.Msg("Scrollable starting-perk area attached: " + ((Object)_root).name);
				}
			}
		}

		private void Clear()
		{
			_root = null;
			_content = null;
			_scroll = null;
			_scrollbar = null;
			_grid = null;
			_lastPerkCount = -1;
		}

		private bool HasOverflow()
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			GameObject root = _root;
			RectTransform val = ((root != null) ? root.GetComponent<RectTransform>() : null);
			GameObject content = _content;
			RectTransform val2 = ((content != null) ? content.GetComponent<RectTransform>() : null);
			if ((Object)(object)val != (Object)null && (Object)(object)val2 != (Object)null)
			{
				Rect rect = val2.rect;
				float height = ((Rect)(ref rect)).height;
				rect = val.rect;
				return height > ((Rect)(ref rect)).height + 0.5f;
			}
			return false;
		}
	}

	internal const string AvailableScrollbarName = "XIAOWO_AvailablePerkScrollbar";

	internal const string SelectedScrollbarName = "XIAOWO_SelectedPerkScrollbar";

	private const float ScrollbarWidth = 14f;

	private const float ScrollbarMargin = 3f;

	private static readonly ScrollArea Available = new ScrollArea("XIAOWO_AvailablePerkScrollContent", "XIAOWO_AvailablePerkScrollbar");

	private static readonly ScrollArea Selected = new ScrollArea("XIAOWO_SelectedPerkScrollContent", "XIAOWO_SelectedPerkScrollbar");

	private static int _refreshCountdown;

	private static int _forceTopFrames;

	internal static void BeforeOpen(PerkUIController ui)
	{
		_forceTopFrames = 1;
		Available.RequestTop();
		Selected.RequestTop();
	}

	internal static void Tick()
	{
		PerkUIController instance = PerkUIController.Instance;
		if ((Object)(object)((instance != null) ? instance.ui : null) == (Object)null || !instance.ui.activeInHierarchy)
		{
			return;
		}
		if (_forceTopFrames > 0)
		{
			_forceTopFrames--;
			Available.ForceTop();
			Selected.ForceTop();
		}
		if (_refreshCountdown-- <= 0)
		{
			_refreshCountdown = 10;
			if (!Available.Resolve(instance.availablePerks))
			{
				instance.availablePerks = Available.Ensure(instance.availablePerks) ?? instance.availablePerks;
			}
			ExpandSelectedArea(instance);
			if (!Selected.Resolve(instance.selectedPerks))
			{
				instance.selectedPerks = Selected.Ensure(instance.selectedPerks) ?? instance.selectedPerks;
			}
			Available.RefreshIfCountChanged();
			Selected.RefreshIfCountChanged();
		}
	}

	internal static void Capture(PerkUIController ui)
	{
		if (_forceTopFrames <= 0)
		{
			Available.Capture((ui != null) ? ui.availablePerks : null);
			Selected.Capture((ui != null) ? ui.selectedPerks : null);
		}
	}

	internal static void Ensure(PerkUIController ui)
	{
		if (!((Object)(object)ui == (Object)null))
		{
			ui.availablePerks = Available.Ensure(ui.availablePerks) ?? ui.availablePerks;
			ExpandSelectedArea(ui);
			ui.selectedPerks = Selected.Ensure(ui.selectedPerks) ?? ui.selectedPerks;
			Available.Refresh();
			Selected.Refresh();
			if (_forceTopFrames > 0)
			{
				Available.ForceTop();
				Selected.ForceTop();
			}
		}
	}

	internal static void Refresh(PerkUIController ui)
	{
		if (!((Object)(object)ui == (Object)null))
		{
			Available.Resolve(ui.availablePerks);
			Selected.Resolve(ui.selectedPerks);
			Available.Refresh();
			Selected.Refresh();
		}
	}

	internal static string BuildStatus(PerkUIController ui)
	{
		string text = (Available.Resolve((ui != null) ? ui.availablePerks : null) ? Available.BuildStatus("available") : "available.present=False");
		string text2 = (Selected.Resolve((ui != null) ? ui.selectedPerks : null) ? Selected.BuildStatus("selected") : "selected.present=False");
		return "PERK_SCROLL\n" + text + "\n" + text2;
	}

	internal static bool SetTestPosition(PerkUIController ui, float position)
	{
		return Available.SetPosition((ui != null) ? ui.availablePerks : null, position);
	}

	internal static float GetTestPosition(PerkUIController ui)
	{
		return Available.GetPosition((ui != null) ? ui.availablePerks : null);
	}

	internal static float GetSelectedTestPosition(PerkUIController ui)
	{
		return Selected.GetPosition((ui != null) ? ui.selectedPerks : null);
	}

	internal static bool SetSelectedTestPosition(PerkUIController ui, float position)
	{
		return Selected.SetPosition((ui != null) ? ui.selectedPerks : null, position);
	}

	internal static bool IsSelectedScrollbarVisible(PerkUIController ui)
	{
		return Selected.IsScrollbarVisible((ui != null) ? ui.selectedPerks : null);
	}

	internal static bool TestScrollbarValue(PerkUIController ui, float position, out float actual)
	{
		return Available.TestScrollbarValue((ui != null) ? ui.availablePerks : null, position, out actual);
	}

	internal static bool TestMouseWheel(PerkUIController ui, float delta, out float before, out float after)
	{
		return Available.TestMouseWheel((ui != null) ? ui.availablePerks : null, delta, out before, out after);
	}

	internal static bool SetRawAvailableTestPosition(PerkUIController ui, float position)
	{
		return Available.SetRawPosition((ui != null) ? ui.availablePerks : null, position);
	}

	internal static bool SetRawSelectedTestPosition(PerkUIController ui, float position)
	{
		return Selected.SetRawPosition((ui != null) ? ui.selectedPerks : null, position);
	}

	private static void ExpandSelectedArea(PerkUIController ui)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = ((ui != null) ? ui.selectedPerks : null);
		if ((Object)(object)val == (Object)null || Selected.IsAttached(val))
		{
			return;
		}
		RectTransform component = val.GetComponent<RectTransform>();
		Transform parent = val.transform.parent;
		if ((Object)(object)component == (Object)null || (Object)(object)parent == (Object)null)
		{
			return;
		}
		Rect rect = component.rect;
		float height = ((Rect)(ref rect)).height;
		if (!(height <= 0f))
		{
			ResizeKeepingTop(component, height * 2f);
			Transform obj = parent.Find("SelectedPerkBG");
			RectTransform val2 = ((obj != null) ? ((Component)obj).gameObject.GetComponent<RectTransform>() : null);
			if ((Object)(object)val2 != (Object)null)
			{
				rect = val2.rect;
				ResizeKeepingTop(val2, ((Rect)(ref rect)).height + height);
			}
			Transform obj2 = parent.Find("PerkPoint");
			MoveDown((obj2 != null) ? ((Component)obj2).gameObject.GetComponent<RectTransform>() : null, height);
			Transform obj3 = parent.Find("PerkSlot");
			MoveDown((obj3 != null) ? ((Component)obj3).gameObject.GetComponent<RectTransform>() : null, height);
		}
	}

	private static void ResizeKeepingTop(RectTransform rect, float newHeight)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = rect.rect;
		float height = ((Rect)(ref rect2)).height;
		float num = rect.anchoredPosition.y + (1f - rect.pivot.y) * height;
		rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + newHeight - height);
		rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, num - (1f - rect.pivot.y) * newHeight);
	}

	private static void MoveDown(RectTransform rect, float amount)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rect != (Object)null)
		{
			rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - amount);
		}
	}

	private static Scrollbar CreateScrollbar(Transform parent, string name)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name, (Type[])(object)new Type[4]
		{
			Il2CppType.Of<RectTransform>(),
			Il2CppType.Of<CanvasRenderer>(),
			Il2CppType.Of<Image>(),
			Il2CppType.Of<Scrollbar>()
		});
		val.transform.SetParent(parent, false);
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(1f, 0f);
		component.anchorMax = new Vector2(1f, 1f);
		component.pivot = new Vector2(1f, 0.5f);
		component.anchoredPosition = new Vector2(-3f, 0f);
		component.sizeDelta = new Vector2(14f, -6f);
		((Graphic)val.GetComponent<Image>()).color = new Color(7f / 51f, 0.11764706f, 0.18431373f, 1f);
		GameObject val2 = new GameObject("Sliding Area", (Type[])(object)new Type[1] { Il2CppType.Of<RectTransform>() });
		val2.transform.SetParent(val.transform, false);
		RectTransform component2 = val2.GetComponent<RectTransform>();
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		component2.offsetMin = new Vector2(2f, 2f);
		component2.offsetMax = new Vector2(-2f, -2f);
		GameObject val3 = new GameObject("Handle", (Type[])(object)new Type[3]
		{
			Il2CppType.Of<RectTransform>(),
			Il2CppType.Of<CanvasRenderer>(),
			Il2CppType.Of<Image>()
		});
		val3.transform.SetParent(val2.transform, false);
		RectTransform component3 = val3.GetComponent<RectTransform>();
		component3.anchorMin = Vector2.zero;
		component3.anchorMax = Vector2.one;
		component3.offsetMin = Vector2.zero;
		component3.offsetMax = Vector2.zero;
		Image component4 = val3.GetComponent<Image>();
		((Graphic)component4).color = new Color(0.57254905f, 67f / 85f, 38f / 51f, 1f);
		Scrollbar component5 = val.GetComponent<Scrollbar>();
		component5.handleRect = component3;
		((Selectable)component5).targetGraphic = (Graphic)(object)component4;
		component5.direction = (Direction)2;
		component5.numberOfSteps = 0;
		component5.value = 1f;
		return component5;
	}

	private static void CopyGrid(GridLayoutGroup source, GridLayoutGroup target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		target.cellSize = source.cellSize;
		target.spacing = source.spacing;
		target.startCorner = source.startCorner;
		target.startAxis = source.startAxis;
		((LayoutGroup)target).childAlignment = ((LayoutGroup)source).childAlignment;
		((LayoutGroup)target).padding = new RectOffset(((LayoutGroup)source).padding.left, ((LayoutGroup)source).padding.right, ((LayoutGroup)source).padding.top, ((LayoutGroup)source).padding.bottom);
	}

	private static void MoveDirectPerksToContent(GameObject root, GameObject content)
	{
		if ((Object)(object)root == (Object)null || (Object)(object)content == (Object)null || (Object)(object)root == (Object)(object)content)
		{
			return;
		}
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < root.transform.childCount; i++)
		{
			Transform child = root.transform.GetChild(i);
			if ((Object)(object)child != (Object)null && (Object)(object)((Component)child).gameObject.GetComponent<StartingPerkElement>() != (Object)null)
			{
				list.Add(child);
			}
		}
		foreach (Transform item in list)
		{
			item.SetParent(content.transform, false);
		}
	}

	private static int CountDirectPerks(Transform content)
	{
		if ((Object)(object)content == (Object)null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < content.childCount; i++)
		{
			Transform child = content.GetChild(i);
			if ((Object)(object)child != (Object)null && (Object)(object)((Component)child).gameObject.GetComponent<StartingPerkElement>() != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	private static bool IsAlive(Object value)
	{
		if (value != (Object)null)
		{
			return ((Il2CppObjectBase)value).Pointer != IntPtr.Zero;
		}
		return false;
	}
}
