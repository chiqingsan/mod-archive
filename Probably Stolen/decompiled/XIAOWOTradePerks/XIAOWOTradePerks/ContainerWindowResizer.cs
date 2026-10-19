using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XIAOWOTradePerks;

internal static class ContainerWindowResizer
{
	[Flags]
	private enum ResizeEdge
	{
		None = 0,
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8
	}

	private sealed class ResizeState
	{
		private readonly List<ResizeHandle> _handles = new List<ResizeHandle>();

		private TreeNodeRender _overlay;

		private Func<PointerEventData, bool> _maximizeAction;

		private ResizeEdge _activeEdge;

		private ResizeEdge _hoveredEdge;

		private Vector2 _startPointer;

		private Vector2 _startPosition;

		private Vector2 _startPivot;

		private int _startWidth;

		private int _startHeight;

		private int _startColumns;

		private int _startRows;

		private float _columnPixels;

		private float _rowPixels;

		private bool _maximized;

		private int _restoreWidth;

		private int _restoreHeight;

		private int _restoreColumns;

		private int _restoreRows;

		private Vector2 _restorePosition;

		private bool _wasVisible;

		private bool _pointerDrag;

		internal ResizeEdge ActiveEdge => _activeEdge;

		internal GameItem Item { get; set; }

		internal PixelWindow Window { get; }

		internal bool IsAlive
		{
			get
			{
				try
				{
					PixelWindow window = Window;
					return (Object)(object)((window != null) ? window.rectTransform : null) != (Object)null && _handles.Count == 8 && LiveHandleCount == 8;
				}
				catch
				{
					return false;
				}
			}
		}

		internal int LiveHandleCount
		{
			get
			{
				int num = 0;
				foreach (ResizeHandle handle in _handles)
				{
					try
					{
						TreeNodeRender node = handle.Node;
						if ((Object)(object)((node != null) ? node.rectTransform : null) != (Object)null)
						{
							num++;
						}
					}
					catch
					{
					}
				}
				return num;
			}
		}

		internal bool HasMaximizeHandler
		{
			get
			{
				try
				{
					int result;
					if ((Delegate)(object)_maximizeAction != (Delegate)null)
					{
						PixelWindow window = Window;
						if (window == null)
						{
							result = 0;
						}
						else
						{
							TreeNodeRender topDragBorder = window.topDragBorder;
							bool? obj;
							if (topDragBorder == null)
							{
								obj = null;
							}
							else
							{
								MultiEventHandler eventHandler = topDragBorder.eventHandler;
								obj = ((eventHandler == null) ? null : eventHandler.mouseDoubleClickFunc?.Contains(_maximizeAction));
							}
							bool? flag = obj;
							result = (flag.GetValueOrDefault() ? 1 : 0);
						}
					}
					else
					{
						result = 0;
					}
					return (byte)result != 0;
				}
				catch
				{
					return false;
				}
			}
		}

		internal ResizeState(GameItem item)
		{
			Item = item;
			Window = item.contentWindow;
		}

		internal ResizeEdge Tick(Vector2 pointer, bool pointerHeld, bool focused, out bool isActive, out int order)
		{
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			try
			{
				PixelWindow window = Window;
				int num;
				if (window == null)
				{
					num = 0;
				}
				else
				{
					RectTransform rectTransform = window.rectTransform;
					bool? obj;
					if (rectTransform == null)
					{
						obj = null;
					}
					else
					{
						GameObject gameObject = ((Component)rectTransform).gameObject;
						obj = ((gameObject != null) ? new bool?(gameObject.activeInHierarchy) : null);
					}
					bool? flag2 = obj;
					num = (flag2.GetValueOrDefault() ? 1 : 0);
				}
				flag = (byte)num != 0;
			}
			catch
			{
			}
			if (flag && !_wasVisible)
			{
				BayExpansionPerk.TryRestoreRememberedGridShape(Item);
				BringHandlesToFront();
			}
			_wasVisible = flag;
			order = GetWindowOrder();
			isActive = false;
			if (!flag || !focused)
			{
				FinishDrag(null);
				_hoveredEdge = ResizeEdge.None;
				return ResizeEdge.None;
			}
			SyncOverlay();
			if (_activeEdge != 0)
			{
				if (pointerHeld)
				{
					if (_pointerDrag)
					{
						DragAtPointer(pointer);
					}
					isActive = true;
					return _activeEdge;
				}
				FinishDrag(null);
			}
			_hoveredEdge = FindHandleAtPointer(pointer, null);
			return _hoveredEdge;
		}

		internal string TestCloseButtonPriority()
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Expected O, but got Unknown
			PixelWindow window = Window;
			object obj;
			if (window == null)
			{
				obj = null;
			}
			else
			{
				TreeNodeRender topCloseButton = window.topCloseButton;
				obj = ((topCloseButton != null) ? topCloseButton.rectTransform : null);
			}
			RectTransform val = (RectTransform)obj;
			if ((Object)(object)val == (Object)null)
			{
				return "CONTAINER_CLOSE_PRIORITY FAIL: close button unavailable";
			}
			BringHandlesToFront();
			Rect rect = val.rect;
			Vector2[] array = (Vector2[])(object)new Vector2[5]
			{
				((Rect)(ref rect)).center,
				new Vector2(((Rect)(ref rect)).xMin + 1f, ((Rect)(ref rect)).yMin + 1f),
				new Vector2(((Rect)(ref rect)).xMin + 1f, ((Rect)(ref rect)).yMax - 1f),
				new Vector2(((Rect)(ref rect)).xMax - 1f, ((Rect)(ref rect)).yMin + 1f),
				new Vector2(((Rect)(ref rect)).xMax - 1f, ((Rect)(ref rect)).yMax - 1f)
			};
			int num = 0;
			Vector2[] array2 = array;
			foreach (Vector2 val2 in array2)
			{
				Vector2 pointer = RectTransformUtility.WorldToScreenPoint((Camera)null, ((Transform)val).TransformPoint(Vector2.op_Implicit(val2)));
				if (FindHandleAtPointer(pointer, null) == ResizeEdge.None)
				{
					num++;
				}
			}
			Vector2 position = RectTransformUtility.WorldToScreenPoint((Camera)null, ((Transform)val).TransformPoint(Vector2.op_Implicit(((Rect)(ref rect)).center)));
			EventSystem current = EventSystem.current;
			bool flag = false;
			string text = "<none>";
			if ((Object)(object)current != (Object)null)
			{
				PointerEventData val3 = new PointerEventData(current)
				{
					position = position
				};
				List<RaycastResult> val4 = new List<RaycastResult>();
				current.RaycastAll(val3, val4);
				if (val4.Count > 0)
				{
					GameObject gameObject = val4[0].gameObject;
					text = ((gameObject != null) ? ((Object)gameObject).name : null) ?? "<null>";
					Transform val5 = ((gameObject != null) ? gameObject.transform : null);
					flag = (Object)(object)val5 != (Object)null && (((Il2CppObjectBase)val5).Pointer == ((Il2CppObjectBase)val).Pointer || val5.IsChildOf((Transform)(object)val));
				}
			}
			TreeNodeRender overlay = _overlay;
			RectTransform val6 = ((overlay != null) ? overlay.rectTransform : null);
			bool flag2 = !((Object)(object)((val6 != null) ? ((Transform)val6).parent : null) != (Object)null) || !((Object)(object)((Transform)val).parent != (Object)null) || !(((Il2CppObjectBase)((Transform)val6).parent).Pointer == ((Il2CppObjectBase)((Transform)val).parent).Pointer) || ((Transform)val).GetSiblingIndex() > ((Transform)val6).GetSiblingIndex();
			PixelWindow window2 = Window;
			int? obj2;
			if (window2 == null)
			{
				obj2 = null;
			}
			else
			{
				TreeNodeRender topCloseButton2 = window2.topCloseButton;
				if (topCloseButton2 == null)
				{
					obj2 = null;
				}
				else
				{
					MultiEventHandler eventHandler = topCloseButton2.eventHandler;
					obj2 = ((eventHandler == null) ? null : eventHandler.mouseClickFunc?.Count);
				}
			}
			int? num2 = obj2;
			int valueOrDefault = num2.GetValueOrDefault();
			bool flag3 = num == array.Length && flag2 && valueOrDefault > 0;
			return "CONTAINER_CLOSE_PRIORITY " + (flag3 ? "PASS" : "FAIL") + "\nprotectedPoints=" + num + "/" + array.Length + " closeAboveOverlay=" + flag2 + " clickHandlers=" + valueOrDefault + "\ntopHit=" + text + " closeRaycast=" + flag + " active=" + ((Component)val).gameObject.activeInHierarchy + "\nedgeThickness=" + 8f + " cornerSize=" + 16f;
		}

		internal string TestPointerDrag()
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			GameGridInventory resizableInventory = BayExpansionPerk.GetResizableInventory(Item);
			ResizeHandle resizeHandle = _handles.Find((ResizeHandle handle) => handle.Edge == ResizeEdge.Right);
			if (((resizableInventory != null) ? resizableInventory.inventoryShape : null) != null)
			{
				object obj;
				if (resizeHandle == null)
				{
					obj = null;
				}
				else
				{
					TreeNodeRender node = resizeHandle.Node;
					obj = ((node != null) ? node.rectTransform : null);
				}
				if (!((Object)obj == (Object)null))
				{
					int width = resizableInventory.inventoryShape.width;
					Vector2 val = RectTransformUtility.WorldToScreenPoint((Camera)null, ((Transform)resizeHandle.Node.rectTransform).position);
					if (!BeginPointerDrag(val, ResizeEdge.Right))
					{
						return "CONTAINER_POINTER_DRAG FAIL: pointer press did not reach front window";
					}
					bool isActive;
					int order;
					ResizeEdge resizeEdge = Tick(val + new Vector2(100f, 0f), pointerHeld: true, focused: true, out isActive, out order);
					int width2 = resizableInventory.inventoryShape.width;
					Tick(val + new Vector2(100f, 0f), pointerHeld: false, focused: true, out var _, out order);
					bool flag = isActive && resizeEdge == ResizeEdge.Right && width2 > width && _activeEdge == ResizeEdge.None && !_pointerDrag;
					return "CONTAINER_POINTER_DRAG " + (flag ? "PASS" : "FAIL") + " initial=" + width + " expanded=" + width2 + " active=" + isActive + " released=" + (_activeEdge == ResizeEdge.None);
				}
			}
			return "CONTAINER_POINTER_DRAG FAIL: inventory or right edge unavailable";
		}

		internal string TestCursorFeedback()
		{
			string text = TestCursor(ResizeEdge.Left);
			string text2 = TestCursor(ResizeEdge.Top);
			string text3 = TestCursor(ResizeEdge.Left | ResizeEdge.Top);
			string text4 = TestCursor(ResizeEdge.Right | ResizeEdge.Top);
			ResizeCursor.Reset();
			string currentName = ResizeCursor.CurrentName;
			bool flag = text == "Horizontal" && text2 == "Vertical" && text3 == "DiagonalDown" && text4 == "DiagonalUp" && currentName == "Default";
			return "CONTAINER_CURSOR_FEEDBACK " + (flag ? "PASS" : "FAIL") + "\nhorizontal=" + text + " vertical=" + text2 + "\ndiagonalDown=" + text3 + " diagonalUp=" + text4 + "\nreset=" + currentName + " visible=" + Cursor.visible + " mode=ForceSoftware";
		}

		internal string TestCursorRelease()
		{
			ResizeHandle resizeHandle = _handles.Find((ResizeHandle handle) => handle.Edge == ResizeEdge.Right);
			if (resizeHandle == null)
			{
				return "CONTAINER_CURSOR_RELEASE FAIL: right handle unavailable";
			}
			ResizeCursor.Reset();
			resizeHandle.Enter.Invoke((PointerEventData)null);
			_activeEdge = ResizeEdge.Right;
			resizeHandle.Exit.Invoke((PointerEventData)null);
			string currentName = ResizeCursor.CurrentName;
			resizeHandle.End.Invoke((PointerEventData)null);
			string currentName2 = ResizeCursor.CurrentName;
			resizeHandle.Enter.Invoke((PointerEventData)null);
			_activeEdge = ResizeEdge.Right;
			resizeHandle.End.Invoke((PointerEventData)null);
			string currentName3 = ResizeCursor.CurrentName;
			resizeHandle.Exit.Invoke((PointerEventData)null);
			string currentName4 = ResizeCursor.CurrentName;
			bool flag = currentName == "Horizontal" && currentName2 == "Default" && currentName3 == "Horizontal" && currentName4 == "Default";
			return "CONTAINER_CURSOR_RELEASE " + (flag ? "PASS" : "FAIL") + "\ndraggingOutside=" + currentName + " releasedOutside=" + currentName2 + "\nreleasedInside=" + currentName3 + " exitedAfterRelease=" + currentName4;
		}

		internal string TestCursorReconciliation()
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			ResizeHandle resizeHandle = _handles.Find((ResizeHandle handle) => handle.Edge == ResizeEdge.Right);
			object obj;
			if (resizeHandle == null)
			{
				obj = null;
			}
			else
			{
				TreeNodeRender node = resizeHandle.Node;
				obj = ((node != null) ? node.rectTransform : null);
			}
			if ((Object)obj == (Object)null)
			{
				return "CONTAINER_CURSOR_RECONCILIATION FAIL: right handle unavailable";
			}
			Vector2 pointer = default(Vector2);
			((Vector2)(ref pointer))._002Ector(-10000f, -10000f);
			Vector2 pointer2 = RectTransformUtility.WorldToScreenPoint((Camera)null, ((Transform)resizeHandle.Node.rectTransform).position);
			_activeEdge = ResizeEdge.None;
			_hoveredEdge = ResizeEdge.None;
			ResizeCursor.Reset();
			bool isActive;
			int order;
			ResizeEdge resizeEdge = Tick(pointer2, pointerHeld: false, focused: true, out isActive, out order);
			if (resizeEdge == ResizeEdge.None)
			{
				ResizeCursor.Reset();
			}
			else
			{
				ResizeCursor.Set(resizeEdge);
			}
			string currentName = ResizeCursor.CurrentName;
			_hoveredEdge = ResizeEdge.Right;
			ResizeCursor.Set(ResizeEdge.Right);
			ResizeEdge resizeEdge2 = Tick(pointer, pointerHeld: false, focused: true, out isActive, out order);
			if (resizeEdge2 == ResizeEdge.None)
			{
				ResizeCursor.Reset();
			}
			else
			{
				ResizeCursor.Set(resizeEdge2);
			}
			string currentName2 = ResizeCursor.CurrentName;
			_activeEdge = ResizeEdge.Right;
			ResizeCursor.Set(ResizeEdge.Right);
			ResizeEdge resizeEdge3 = Tick(pointer, pointerHeld: false, focused: true, out isActive, out order);
			if (resizeEdge3 == ResizeEdge.None)
			{
				ResizeCursor.Reset();
			}
			else
			{
				ResizeCursor.Set(resizeEdge3);
			}
			string currentName3 = ResizeCursor.CurrentName;
			_hoveredEdge = ResizeEdge.Right;
			ResizeCursor.Set(ResizeEdge.Right);
			ResizeEdge resizeEdge4 = Tick(pointer2, pointerHeld: false, focused: false, out isActive, out order);
			if (resizeEdge4 == ResizeEdge.None)
			{
				ResizeCursor.Reset();
			}
			else
			{
				ResizeCursor.Set(resizeEdge4);
			}
			string currentName4 = ResizeCursor.CurrentName;
			bool flag = currentName == "Horizontal" && currentName2 == "Default" && currentName3 == "Default" && currentName4 == "Default" && _activeEdge == ResizeEdge.None && _hoveredEdge == ResizeEdge.None;
			return "CONTAINER_CURSOR_RECONCILIATION " + (flag ? "PASS" : "FAIL") + "\nentered=" + currentName + " missedExit=" + currentName2 + "\nmissedEnd=" + currentName3 + " focusLoss=" + currentName4 + "\nactive=" + _activeEdge.ToString() + " hover=" + _hoveredEdge;
		}

		private string TestCursor(ResizeEdge edge)
		{
			foreach (ResizeHandle handle in _handles)
			{
				if (handle.Edge == edge)
				{
					handle.Enter.Invoke((PointerEventData)null);
					string currentName = ResizeCursor.CurrentName;
					handle.Exit.Invoke((PointerEventData)null);
					return currentName;
				}
			}
			return "Missing";
		}

		internal void CreateHandles()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			DestroyHandles();
			CreateOverlay();
			AddHandle("Left", ResizeEdge.Left, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(8f, -32f));
			AddHandle("Right", ResizeEdge.Right, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(8f, -32f));
			AddHandle("Top", ResizeEdge.Top, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-32f, 8f));
			AddHandle("Bottom", ResizeEdge.Bottom, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(-32f, 8f));
			AddHandle("TopLeft", ResizeEdge.Left | ResizeEdge.Top, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16f, 16f));
			AddHandle("TopRight", ResizeEdge.Right | ResizeEdge.Top, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(16f, 16f));
			AddHandle("BottomLeft", ResizeEdge.Left | ResizeEdge.Bottom, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(16f, 16f));
			AddHandle("BottomRight", ResizeEdge.Right | ResizeEdge.Bottom, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(16f, 16f));
			AttachMaximizeHandler();
		}

		internal void BringHandlesToFront()
		{
			SyncOverlay();
			try
			{
				TreeNodeRender overlay = _overlay;
				if (overlay != null)
				{
					RectTransform rectTransform = overlay.rectTransform;
					if (rectTransform != null)
					{
						((Transform)rectTransform).SetAsLastSibling();
					}
				}
			}
			catch
			{
			}
			foreach (ResizeHandle handle in _handles)
			{
				try
				{
					TreeNodeRender node = handle.Node;
					if (node != null)
					{
						RectTransform rectTransform2 = node.rectTransform;
						if (rectTransform2 != null)
						{
							((Transform)rectTransform2).SetAsLastSibling();
						}
					}
				}
				catch
				{
				}
			}
			try
			{
				PixelWindow window = Window;
				if (window == null)
				{
					return;
				}
				TreeNodeRender topCloseButton = window.topCloseButton;
				if (topCloseButton != null)
				{
					RectTransform rectTransform3 = topCloseButton.rectTransform;
					if (rectTransform3 != null)
					{
						((Transform)rectTransform3).SetAsLastSibling();
					}
				}
			}
			catch
			{
			}
		}

		internal void DestroyHandles()
		{
			try
			{
				if ((Delegate)(object)_maximizeAction != (Delegate)null)
				{
					PixelWindow window = Window;
					if (window != null)
					{
						TreeNodeRender topDragBorder = window.topDragBorder;
						if (topDragBorder != null)
						{
							MultiEventHandler eventHandler = topDragBorder.eventHandler;
							if (eventHandler != null)
							{
								eventHandler.mouseDoubleClickFunc?.Remove(_maximizeAction);
							}
						}
					}
				}
			}
			catch
			{
			}
			foreach (ResizeHandle handle in _handles)
			{
				try
				{
					TreeNodeRender node = handle.Node;
					if ((Object)(object)((node != null) ? node.rectTransform : null) != (Object)null)
					{
						Object.Destroy((Object)(object)((Component)handle.Node.rectTransform).gameObject);
					}
				}
				catch
				{
				}
			}
			try
			{
				TreeNodeRender overlay = _overlay;
				if ((Object)(object)((overlay != null) ? overlay.rectTransform : null) != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)_overlay.rectTransform).gameObject);
				}
			}
			catch
			{
			}
			_handles.Clear();
			_overlay = null;
			_maximizeAction = null;
			_activeEdge = ResizeEdge.None;
			_hoveredEdge = ResizeEdge.None;
			ResizeCursor.Reset();
		}

		private void AttachMaximizeHandler()
		{
			TreeNodeRender topDragBorder = Window.topDragBorder;
			if (!((Object)(object)topDragBorder == (Object)null))
			{
				topDragBorder.InitEventHandler();
				_maximizeAction = Convert(ToggleMaximize);
				topDragBorder.eventHandler.mouseDoubleClickFunc.Add(_maximizeAction);
			}
		}

		private void CreateOverlay()
		{
			_overlay = TreeNodeRender.Instantiate(Window, "XIAOWOContainerResizeOverlay");
			TreeNodeRender overlay = _overlay;
			if ((Object)(object)((overlay != null) ? overlay.rectTransform : null) == (Object)null)
			{
				throw new InvalidOperationException("container resize overlay could not be created");
			}
			((Transform)_overlay.rectTransform).SetParent((Transform)(object)Window.rectTransform, false);
			Image image = _overlay.image;
			if ((Object)(object)image != (Object)null)
			{
				((Behaviour)image).enabled = false;
				((Graphic)image).raycastTarget = false;
			}
			if ((Object)(object)_overlay.mask != (Object)null)
			{
				((Behaviour)_overlay.mask).enabled = false;
			}
			SyncOverlay();
			((Transform)_overlay.rectTransform).SetAsLastSibling();
		}

		private void SyncOverlay()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			PixelWindow window = Window;
			object obj;
			if (window == null)
			{
				obj = null;
			}
			else
			{
				TreeNodeRender outsideBorder = window.outsideBorder;
				obj = ((outsideBorder != null) ? outsideBorder.rectTransform : null);
			}
			RectTransform val = (RectTransform)obj;
			TreeNodeRender overlay = _overlay;
			RectTransform val2 = ((overlay != null) ? overlay.rectTransform : null);
			if (!((Object)(object)val == (Object)null) && !((Object)(object)val2 == (Object)null))
			{
				val2.anchorMin = val.anchorMin;
				val2.anchorMax = val.anchorMax;
				val2.pivot = val.pivot;
				val2.anchoredPosition = val.anchoredPosition;
				val2.sizeDelta = val.sizeDelta;
				((Transform)val2).localScale = Vector3.one;
			}
		}

		private void AddHandle(string name, ResizeEdge edge, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta)
		{
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			TreeNodeRender val = TreeNodeRender.Instantiate(Window, "XIAOWOContainerResize" + name);
			if ((Object)(object)((val != null) ? val.rectTransform : null) == (Object)null)
			{
				throw new InvalidOperationException("container resize handle could not be created");
			}
			RectTransform rectTransform = val.rectTransform;
			TreeNodeRender overlay = _overlay;
			object obj = ((overlay != null) ? overlay.rectTransform : null);
			if (obj == null)
			{
				TreeNodeRender outsideBorder = Window.outsideBorder;
				obj = ((outsideBorder != null) ? outsideBorder.rectTransform : null) ?? Window.rectTransform;
			}
			RectTransform val2 = (RectTransform)obj;
			((Transform)rectTransform).SetParent((Transform)(object)val2, false);
			rectTransform.anchorMin = anchorMin;
			rectTransform.anchorMax = anchorMax;
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = Vector2.zero;
			rectTransform.sizeDelta = sizeDelta;
			((Transform)rectTransform).localScale = Vector3.one;
			((Transform)rectTransform).SetAsLastSibling();
			Image val4 = (val.image = val.image ?? ((Component)rectTransform).gameObject.AddComponent<Image>());
			((Behaviour)val4).enabled = true;
			((Graphic)val4).color = new Color(1f, 1f, 1f, 0.001f);
			((Graphic)val4).raycastTarget = true;
			val.InitEventHandler();
			Func<PointerEventData, bool> val5 = Convert((PointerEventData data) => PrepareDrag(data));
			Func<PointerEventData, bool> val6 = Convert((PointerEventData data) => BeginDrag(data, edge));
			Func<PointerEventData, bool> val7 = Convert(Drag);
			Func<PointerEventData, bool> val8 = Convert(EndDrag);
			Func<PointerEventData, bool> val9 = Convert((PointerEventData data) => EnterHandle(data, edge));
			Func<PointerEventData, bool> val10 = Convert((PointerEventData data) => ExitHandle(data, edge));
			val.eventHandler.mousePotentialDragFunc.Add(val5);
			val.eventHandler.mouseBeginDragFunc.Add(val6);
			val.eventHandler.mouseDragFunc.Add(val7);
			val.eventHandler.mouseEndDragFunc.Add(val8);
			val.eventHandler.mouseEnterFunc.Add(val9);
			val.eventHandler.mouseExitFunc.Add(val10);
			_handles.Add(new ResizeHandle(val, edge, val5, val6, val7, val8, val9, val10));
		}

		private static Func<PointerEventData, bool> Convert(Func<PointerEventData, bool> action)
		{
			return DelegateSupport.ConvertDelegate<Func<PointerEventData, bool>>((Delegate)action);
		}

		private bool PrepareDrag(PointerEventData data)
		{
			if (data != null)
			{
				data.useDragThreshold = false;
			}
			return true;
		}

		private bool BeginDrag(PointerEventData data, ResizeEdge edge)
		{
			if (data != null)
			{
				PixelWindow window = Window;
				if (!((Object)(object)((window != null) ? window.rectTransform : null) == (Object)null) && TryPointerInParent(data, out _startPointer))
				{
					if (_activeEdge != 0)
					{
						((AbstractEventData)data).Use();
						return true;
					}
					if (!StartDrag(edge))
					{
						return false;
					}
					((AbstractEventData)data).Use();
					return true;
				}
			}
			return false;
		}

		internal bool BeginPointerDrag(Vector2 pointer, ResizeEdge edge)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (edge == ResizeEdge.None || _activeEdge != 0 || !IsTopmostWindowHit(pointer) || !TryPointerInParent(pointer, null, out _startPointer))
			{
				return false;
			}
			if (!StartDrag(edge))
			{
				return false;
			}
			_pointerDrag = true;
			return true;
		}

		private bool IsTopmostWindowHit(Vector2 pointer)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Expected O, but got Unknown
			EventSystem current = EventSystem.current;
			if ((Object)(object)current == (Object)null)
			{
				return false;
			}
			PointerEventData val = new PointerEventData(current)
			{
				position = pointer
			};
			List<RaycastResult> val2 = new List<RaycastResult>();
			current.RaycastAll(val, val2);
			object obj;
			if (val2.Count <= 0)
			{
				obj = null;
			}
			else
			{
				GameObject gameObject = val2[0].gameObject;
				obj = ((gameObject != null) ? gameObject.transform : null);
			}
			Transform val3 = (Transform)obj;
			if ((Object)(object)val3 != (Object)null)
			{
				return val3.IsChildOf((Transform)(object)Window.rectTransform);
			}
			return false;
		}

		private bool StartDrag(ResizeEdge edge)
		{
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			GameGridInventory resizableInventory = BayExpansionPerk.GetResizableInventory(Item);
			if (((resizableInventory != null) ? resizableInventory.inventoryShape : null) == null)
			{
				return false;
			}
			_activeEdge = edge;
			ResizeCursor.Set(edge);
			_maximized = false;
			_startWidth = Window.widthPixels;
			_startHeight = Window.heightPixels;
			_startColumns = resizableInventory.inventoryShape.width;
			_startRows = resizableInventory.inventoryShape.height;
			_columnPixels = Math.Max(1f, (float)resizableInventory.widthPixels / (float)Math.Max(1, _startColumns));
			_rowPixels = Math.Max(1f, (float)resizableInventory.heightPixels / (float)Math.Max(1, _startRows));
			_startPosition = Window.rectTransform.anchoredPosition;
			_startPivot = Window.rectTransform.pivot;
			Window.ToFront(true);
			return true;
		}

		private bool ToggleMaximize(PointerEventData data)
		{
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			PixelWindow window = Window;
			RectTransform val = ((window != null) ? window.rectTransform : null);
			Transform val2 = ((val != null) ? ((Transform)val).parent : null);
			if ((Object)(object)val == (Object)null || (Object)(object)val2 == (Object)null)
			{
				return false;
			}
			int actualWidth;
			int actualHeight;
			if (_maximized)
			{
				BayExpansionPerk.TryResizeGridWindow(Item, _restoreColumns, _restoreRows, remember: true, out actualWidth, out actualHeight);
				val.anchoredPosition = _restorePosition;
				_maximized = false;
			}
			else
			{
				_restoreWidth = Window.widthPixels;
				_restoreHeight = Window.heightPixels;
				_restorePosition = val.anchoredPosition;
				GameGridInventory resizableInventory = BayExpansionPerk.GetResizableInventory(Item);
				if (((resizableInventory != null) ? resizableInventory.inventoryShape : null) == null || !BayExpansionPerk.TryGetGridShapeLimits(Item, out actualHeight, out actualWidth, out var maximumColumns, out var maximumRows))
				{
					return false;
				}
				_restoreColumns = resizableInventory.inventoryShape.width;
				_restoreRows = resizableInventory.inventoryShape.height;
				BayExpansionPerk.TryResizeGridWindow(Item, maximumColumns, maximumRows, remember: true, out actualWidth, out actualHeight);
				Window.Center();
				_maximized = true;
			}
			Window.FixBounds();
			Item.RememberContentWindowPositionIfMine(Window);
			BringHandlesToFront();
			if (data != null)
			{
				((AbstractEventData)data).Use();
			}
			return true;
		}

		private bool Drag(PointerEventData data)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (_pointerDrag)
			{
				if (data != null)
				{
					((AbstractEventData)data).Use();
				}
				return true;
			}
			if (_activeEdge == ResizeEdge.None || data == null || !TryPointerInParent(data, out var point))
			{
				return false;
			}
			if (!DragAt(point))
			{
				return false;
			}
			((AbstractEventData)data).Use();
			return true;
		}

		private bool DragAtPointer(Vector2 pointer)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (TryPointerInParent(pointer, null, out var point))
			{
				return DragAt(point);
			}
			return false;
		}

		private bool DragAt(Vector2 pointer)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			ResizeCursor.Set(_activeEdge);
			Vector2 val = pointer - _startPointer;
			int num = _startWidth;
			int num2 = _startHeight;
			if ((_activeEdge & ResizeEdge.Left) != 0)
			{
				num = (int)Math.Round((float)_startWidth - val.x);
			}
			else if ((_activeEdge & ResizeEdge.Right) != 0)
			{
				num = (int)Math.Round((float)_startWidth + val.x);
			}
			if ((_activeEdge & ResizeEdge.Bottom) != 0)
			{
				num2 = (int)Math.Round((float)_startHeight - val.y);
			}
			else if ((_activeEdge & ResizeEdge.Top) != 0)
			{
				num2 = (int)Math.Round((float)_startHeight + val.y);
			}
			int num3 = _startColumns;
			int num4 = _startRows;
			if ((_activeEdge & (ResizeEdge.Left | ResizeEdge.Right)) != 0)
			{
				num3 += (int)Math.Round((float)(num - _startWidth) / _columnPixels);
			}
			if ((_activeEdge & (ResizeEdge.Top | ResizeEdge.Bottom)) != 0)
			{
				num4 += (int)Math.Round((float)(num2 - _startHeight) / _rowPixels);
			}
			if (!BayExpansionPerk.TryResizeGridWindow(Item, num3, num4, remember: false, out var actualWidth, out var actualHeight))
			{
				return false;
			}
			int num5 = actualWidth - _startWidth;
			int num6 = actualHeight - _startHeight;
			Vector2 startPosition = _startPosition;
			if ((_activeEdge & ResizeEdge.Left) != 0)
			{
				startPosition.x -= (1f - _startPivot.x) * (float)num5;
			}
			else if ((_activeEdge & ResizeEdge.Right) != 0)
			{
				startPosition.x += _startPivot.x * (float)num5;
			}
			if ((_activeEdge & ResizeEdge.Bottom) != 0)
			{
				startPosition.y -= (1f - _startPivot.y) * (float)num6;
			}
			else if ((_activeEdge & ResizeEdge.Top) != 0)
			{
				startPosition.y += _startPivot.y * (float)num6;
			}
			Window.rectTransform.anchoredPosition = startPosition;
			BringHandlesToFront();
			return true;
		}

		private bool EndDrag(PointerEventData data)
		{
			if (!FinishDrag(data))
			{
				return false;
			}
			ResizeEdge hoveredEdge = FindHandleAtPointer(data);
			if (data != null)
			{
				_hoveredEdge = hoveredEdge;
			}
			if (_hoveredEdge == ResizeEdge.None)
			{
				ResizeCursor.Reset();
			}
			else
			{
				ResizeCursor.Set(_hoveredEdge);
			}
			if (data != null)
			{
				((AbstractEventData)data).Use();
			}
			return true;
		}

		private bool FinishDrag(PointerEventData data)
		{
			if (_activeEdge == ResizeEdge.None)
			{
				return false;
			}
			_activeEdge = ResizeEdge.None;
			_pointerDrag = false;
			try
			{
				BayExpansionPerk.RememberWindowSize(Item, Window.widthPixels, Window.heightPixels);
				GameGridInventory resizableInventory = BayExpansionPerk.GetResizableInventory(Item);
				if (((resizableInventory != null) ? resizableInventory.inventoryShape : null) != null)
				{
					BayExpansionPerk.RememberGridShape(Item, resizableInventory.inventoryShape.width, resizableInventory.inventoryShape.height);
				}
				Window.FixBounds();
				Item.RememberContentWindowPositionIfMine(Window);
			}
			catch
			{
			}
			return true;
		}

		private bool EnterHandle(PointerEventData data, ResizeEdge edge)
		{
			_hoveredEdge = edge;
			ResizeCursor.Set(edge);
			return true;
		}

		private bool ExitHandle(PointerEventData data, ResizeEdge edge)
		{
			if (_hoveredEdge == edge)
			{
				_hoveredEdge = ResizeEdge.None;
			}
			if (_activeEdge == ResizeEdge.None)
			{
				ResizeCursor.Reset();
			}
			return true;
		}

		private ResizeEdge FindHandleAtPointer(PointerEventData data)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			if (data == null)
			{
				return _hoveredEdge;
			}
			return FindHandleAtPointer(data.position, data.pressEventCamera);
		}

		private ResizeEdge FindHandleAtPointer(Vector2 pointer, Camera camera)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (IsPointerOverCloseButton(pointer, camera))
			{
				return ResizeEdge.None;
			}
			for (int num = _handles.Count - 1; num >= 0; num--)
			{
				ResizeHandle resizeHandle = _handles[num];
				try
				{
					TreeNodeRender node = resizeHandle.Node;
					RectTransform val = ((node != null) ? node.rectTransform : null);
					if ((Object)(object)val != (Object)null && RectTransformUtility.RectangleContainsScreenPoint(val, pointer, camera))
					{
						return resizeHandle.Edge;
					}
				}
				catch
				{
				}
			}
			return ResizeEdge.None;
		}

		private bool IsPointerOverCloseButton(Vector2 pointer, Camera camera)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				PixelWindow window = Window;
				object obj;
				if (window == null)
				{
					obj = null;
				}
				else
				{
					TreeNodeRender topCloseButton = window.topCloseButton;
					obj = ((topCloseButton != null) ? topCloseButton.rectTransform : null);
				}
				RectTransform val = (RectTransform)obj;
				if ((Object)(object)val == (Object)null || !((Component)val).gameObject.activeInHierarchy)
				{
					return false;
				}
				Vector2 val2 = default(Vector2);
				if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(val, pointer, camera, ref val2))
				{
					return false;
				}
				Rect rect = val.rect;
				return val2.x >= ((Rect)(ref rect)).xMin - 2f && val2.x <= ((Rect)(ref rect)).xMax + 2f && val2.y >= ((Rect)(ref rect)).yMin - 2f && val2.y <= ((Rect)(ref rect)).yMax + 2f;
			}
			catch
			{
				return false;
			}
		}

		private int GetWindowOrder()
		{
			try
			{
				PixelWindow window = Window;
				int? obj;
				if (window == null)
				{
					obj = null;
				}
				else
				{
					RectTransform rectTransform = window.rectTransform;
					obj = ((rectTransform != null) ? new int?(((Transform)rectTransform).GetSiblingIndex()) : null);
				}
				int? num = obj;
				return num.GetValueOrDefault(int.MinValue);
			}
			catch
			{
				return int.MinValue;
			}
		}

		private bool TryPointerInParent(PointerEventData data, out Vector2 point)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return TryPointerInParent(data.position, data.pressEventCamera, out point);
		}

		private bool TryPointerInParent(Vector2 pointer, Camera camera, out Vector2 point)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			point = Vector2.zero;
			PixelWindow window = Window;
			object obj;
			if (window == null)
			{
				obj = null;
			}
			else
			{
				RectTransform rectTransform = window.rectTransform;
				obj = ((rectTransform != null) ? ((Transform)rectTransform).parent : null);
			}
			Transform val = (Transform)obj;
			if ((Object)(object)val == (Object)null)
			{
				return false;
			}
			return RectTransformUtility.ScreenPointToLocalPointInRectangle(new RectTransform(((Il2CppObjectBase)val).Pointer), pointer, camera, ref point);
		}
	}

	private sealed class ResizeHandle
	{
		internal TreeNodeRender Node { get; }

		internal ResizeEdge Edge { get; }

		internal Func<PointerEventData, bool> Potential { get; }

		internal Func<PointerEventData, bool> Begin { get; }

		internal Func<PointerEventData, bool> Drag { get; }

		internal Func<PointerEventData, bool> End { get; }

		internal Func<PointerEventData, bool> Enter { get; }

		internal Func<PointerEventData, bool> Exit { get; }

		internal ResizeHandle(TreeNodeRender node, ResizeEdge edge, Func<PointerEventData, bool> potential, Func<PointerEventData, bool> begin, Func<PointerEventData, bool> drag, Func<PointerEventData, bool> end, Func<PointerEventData, bool> enter, Func<PointerEventData, bool> exit)
		{
			Node = node;
			Edge = edge;
			Potential = potential;
			Begin = begin;
			Drag = drag;
			End = end;
			Enter = enter;
			Exit = exit;
		}
	}

	private static class ResizeCursor
	{
		private enum Kind
		{
			Default,
			Horizontal,
			Vertical,
			DiagonalDown,
			DiagonalUp
		}

		private static readonly Dictionary<Kind, Texture2D> Textures = new Dictionary<Kind, Texture2D>();

		private static Kind _current;

		internal static string CurrentName => _current.ToString();

		internal static void Set(ResizeEdge edge)
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			Kind kind;
			switch (edge)
			{
			case ResizeEdge.Left:
			case ResizeEdge.Right:
				kind = Kind.Horizontal;
				break;
			case ResizeEdge.Top:
			case ResizeEdge.Bottom:
				kind = Kind.Vertical;
				break;
			case ResizeEdge.Left | ResizeEdge.Top:
			case ResizeEdge.Right | ResizeEdge.Bottom:
				kind = Kind.DiagonalDown;
				break;
			default:
				kind = Kind.DiagonalUp;
				break;
			}
			Kind kind2 = kind;
			if (!Textures.TryGetValue(kind2, out var value))
			{
				value = Create(kind2);
				Textures[kind2] = value;
			}
			Cursor.visible = true;
			Cursor.SetCursor(value, new Vector2(12f, 12f), (CursorMode)1);
			_current = kind2;
		}

		internal static void Maintain()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (_current != 0 && Textures.TryGetValue(_current, out var value))
			{
				Cursor.visible = true;
				Cursor.SetCursor(value, new Vector2(12f, 12f), (CursorMode)1);
			}
		}

		internal static void Reset()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if (_current != 0)
			{
				Cursor.SetCursor((Texture2D)null, Vector2.zero, (CursorMode)0);
				_current = Kind.Default;
			}
		}

		internal static void Dispose()
		{
			Reset();
			foreach (Texture2D value in Textures.Values)
			{
				try
				{
					Object.Destroy((Object)(object)value);
				}
				catch
				{
				}
			}
			Textures.Clear();
		}

		private static Texture2D Create(Kind kind)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			Texture2D val = new Texture2D(24, 24, (TextureFormat)4, false, false)
			{
				name = "XIAOWOContainerResizeCursor" + kind,
				filterMode = (FilterMode)0,
				wrapMode = (TextureWrapMode)1
			};
			for (int i = 0; i < 24; i++)
			{
				for (int j = 0; j < 24; j++)
				{
					val.SetPixel(i, j, Color.clear);
				}
			}
			(int, int, int, int)[] array = kind switch
			{
				Kind.Horizontal => new(int, int, int, int)[5]
				{
					(3, 12, 20, 12),
					(3, 12, 8, 7),
					(3, 12, 8, 17),
					(20, 12, 15, 7),
					(20, 12, 15, 17)
				}, 
				Kind.Vertical => new(int, int, int, int)[5]
				{
					(12, 3, 12, 20),
					(12, 3, 7, 8),
					(12, 3, 17, 8),
					(12, 20, 7, 15),
					(12, 20, 17, 15)
				}, 
				Kind.DiagonalDown => new(int, int, int, int)[5]
				{
					(4, 19, 19, 4),
					(4, 19, 4, 13),
					(4, 19, 10, 19),
					(19, 4, 13, 4),
					(19, 4, 19, 10)
				}, 
				_ => new(int, int, int, int)[5]
				{
					(4, 4, 19, 19),
					(4, 4, 4, 10),
					(4, 4, 10, 4),
					(19, 19, 13, 19),
					(19, 19, 19, 13)
				}, 
			};
			(int, int, int, int)[] array2 = array;
			for (int k = 0; k < array2.Length; k++)
			{
				(int, int, int, int) tuple = array2[k];
				DrawLine(val, tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, 2, Color.black);
			}
			array2 = array;
			for (int k = 0; k < array2.Length; k++)
			{
				(int, int, int, int) tuple2 = array2[k];
				DrawLine(val, tuple2.Item1, tuple2.Item2, tuple2.Item3, tuple2.Item4, 0, Color.white);
			}
			val.Apply(false, false);
			return val;
		}

		private static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, int radius, Color color)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			int num = Math.Abs(x1 - x0);
			int num2 = ((x0 < x1) ? 1 : (-1));
			int num3 = -Math.Abs(y1 - y0);
			int num4 = ((y0 < y1) ? 1 : (-1));
			int num5 = num + num3;
			while (true)
			{
				for (int i = x0 - radius; i <= x0 + radius; i++)
				{
					for (int j = y0 - radius; j <= y0 + radius; j++)
					{
						if (i >= 0 && i < 24 && j >= 0 && j < 24)
						{
							texture.SetPixel(i, j, color);
						}
					}
				}
				if (x0 != x1 || y0 != y1)
				{
					int num6 = num5 * 2;
					if (num6 >= num3)
					{
						num5 += num3;
						x0 += num2;
					}
					if (num6 <= num)
					{
						num5 += num;
						y0 += num4;
					}
					continue;
				}
				break;
			}
		}
	}

	private const float EdgeThickness = 8f;

	private const float CornerSize = 16f;

	private const float CloseButtonPadding = 2f;

	private static readonly Dictionary<IntPtr, ResizeState> States = new Dictionary<IntPtr, ResizeState>();

	internal static string CurrentCursorName => ResizeCursor.CurrentName;

	internal static void Attach(GameItem item)
	{
		if (!BayExpansionPerk.IsResizableContainer(item))
		{
			return;
		}
		PixelWindow contentWindow = item.contentWindow;
		if (!((Object)(object)((contentWindow != null) ? contentWindow.rectTransform : null) == (Object)null))
		{
			IntPtr pointer = ((Il2CppObjectBase)item.contentWindow).Pointer;
			if (!States.TryGetValue(pointer, out var value) || !value.IsAlive)
			{
				value = new ResizeState(item);
				States[pointer] = value;
				value.CreateHandles();
			}
			else
			{
				value.Item = item;
				value.BringHandlesToFront();
			}
		}
	}

	internal static int HandleCount(GameItem item)
	{
		if (((item != null) ? item.contentWindow : null) == null || !States.TryGetValue(((Il2CppObjectBase)item.contentWindow).Pointer, out var value))
		{
			return 0;
		}
		return value.LiveHandleCount;
	}

	internal static bool HasMaximizeHandler(GameItem item)
	{
		if (((item != null) ? item.contentWindow : null) == null || !States.TryGetValue(((Il2CppObjectBase)item.contentWindow).Pointer, out var value))
		{
			return false;
		}
		return value.HasMaximizeHandler;
	}

	internal static void Tick()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		Vector2 pointer;
		bool mouseButton;
		bool mouseButtonDown;
		bool isFocused;
		try
		{
			pointer = Vector2.op_Implicit(Input.mousePosition);
			mouseButton = Input.GetMouseButton(0);
			mouseButtonDown = Input.GetMouseButtonDown(0);
			isFocused = Application.isFocused;
		}
		catch
		{
			ResizeCursor.Reset();
			return;
		}
		ResizeEdge resizeEdge = ResizeEdge.None;
		bool flag = false;
		int num = int.MinValue;
		ResizeState resizeState = null;
		foreach (ResizeState value in States.Values)
		{
			try
			{
				bool isActive;
				int order;
				ResizeEdge resizeEdge2 = value.Tick(pointer, mouseButton, isFocused, out isActive, out order);
				if (resizeEdge2 != 0 && ((isActive && !flag) || (isActive == flag && order >= num)))
				{
					resizeEdge = resizeEdge2;
					flag = isActive;
					num = order;
					resizeState = value;
				}
			}
			catch
			{
			}
		}
		if (mouseButtonDown && isFocused && !flag && resizeState != null && resizeState.BeginPointerDrag(pointer, resizeEdge))
		{
			resizeEdge = resizeState.ActiveEdge;
		}
		if (resizeEdge == ResizeEdge.None)
		{
			ResizeCursor.Reset();
		}
		else
		{
			ResizeCursor.Set(resizeEdge);
		}
	}

	internal static string TestCursorFeedback(GameItem item)
	{
		if (((item != null) ? item.contentWindow : null) == null || !States.TryGetValue(((Il2CppObjectBase)item.contentWindow).Pointer, out var value))
		{
			return "CONTAINER_CURSOR_FEEDBACK FAIL: resize state unavailable";
		}
		return value.TestCursorFeedback();
	}

	internal static string TestCursorRelease(GameItem item)
	{
		if (((item != null) ? item.contentWindow : null) == null || !States.TryGetValue(((Il2CppObjectBase)item.contentWindow).Pointer, out var value))
		{
			return "CONTAINER_CURSOR_RELEASE FAIL: resize state unavailable";
		}
		return value.TestCursorRelease();
	}

	internal static string TestCursorReconciliation(GameItem item)
	{
		if (((item != null) ? item.contentWindow : null) == null || !States.TryGetValue(((Il2CppObjectBase)item.contentWindow).Pointer, out var value))
		{
			return "CONTAINER_CURSOR_RECONCILIATION FAIL: resize state unavailable";
		}
		return value.TestCursorReconciliation();
	}

	internal static string TestCloseButtonPriority(GameItem item)
	{
		if (((item != null) ? item.contentWindow : null) == null || !States.TryGetValue(((Il2CppObjectBase)item.contentWindow).Pointer, out var value))
		{
			return "CONTAINER_CLOSE_PRIORITY FAIL: resize state unavailable";
		}
		return value.TestCloseButtonPriority();
	}

	internal static string TestPointerDrag(GameItem item)
	{
		if (((item != null) ? item.contentWindow : null) == null || !States.TryGetValue(((Il2CppObjectBase)item.contentWindow).Pointer, out var value))
		{
			return "CONTAINER_POINTER_DRAG FAIL: resize state unavailable";
		}
		return value.TestPointerDrag();
	}

	internal static void Dispose()
	{
		foreach (ResizeState value in States.Values)
		{
			value.DestroyHandles();
		}
		States.Clear();
		ResizeCursor.Dispose();
	}
}
