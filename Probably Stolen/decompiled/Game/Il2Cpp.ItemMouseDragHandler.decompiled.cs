using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace Il2Cpp;

public class ItemMouseDragHandler : EmptyActionHandler
{
	[ObfuscatedName("ItemMouseDragHandler+<>c__DisplayClass38_0")]
	public sealed class __c__DisplayClass38_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___4__this;

		private static readonly System.IntPtr NativeFieldInfoPtr_thisItem;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__2;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__StartDrag_b__0_Internal_Boolean_PixelWindow_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__StartDrag_b__2_Internal_Boolean_GameItem_0;

		public unsafe ItemMouseDragHandler __4__this
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___4__this);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<ItemMouseDragHandler>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___4__this)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe GameItemElement thisItem
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_thisItem);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItemElement>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_thisItem)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe Il2CppSystem.Func<GameItem, bool> __9__2
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___9__2);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<GameItem, bool>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___9__2)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass38_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass38_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "<>c__DisplayClass38_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass38_0>.NativeClassPtr);
			NativeFieldInfoPtr___4__this = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass38_0>.NativeClassPtr, "<>4__this");
			NativeFieldInfoPtr_thisItem = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass38_0>.NativeClassPtr, "thisItem");
			NativeFieldInfoPtr___9__2 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass38_0>.NativeClassPtr, "<>9__2");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass38_0>.NativeClassPtr, 100676461);
			NativeMethodInfoPtr__StartDrag_b__0_Internal_Boolean_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass38_0>.NativeClassPtr, 100676462);
			NativeMethodInfoPtr__StartDrag_b__2_Internal_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass38_0>.NativeClassPtr, 100676463);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass38_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass38_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376347, XrefRangeEnd = 376352, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _StartDrag_b__0(PixelWindow w)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)w);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__StartDrag_b__0_Internal_Boolean_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376352, XrefRangeEnd = 376355, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _StartDrag_b__2(GameItem r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__StartDrag_b__2_Internal_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass38_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[System.Serializable]
	[ObfuscatedName("ItemMouseDragHandler+<>c")]
	public sealed class __c : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___9;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__38_1;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__44_2;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__44_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__StartDrag_b__38_1_Internal_Void_PixelWindow_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__EndDrag_b__44_2_Internal_Void_PixelWindow_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__EndDrag_b__44_0_Internal_Void_TreeNodeRender_0;

		public unsafe static __c __9
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<__c>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<PixelWindow> __9__38_1
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__38_1, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<PixelWindow>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__38_1, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<PixelWindow> __9__44_2
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__44_2, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<PixelWindow>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__44_2, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<TreeNodeRender> __9__44_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__44_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<TreeNodeRender>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__44_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c()
		{
			Il2CppClassPointerStore<__c>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "<>c");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c>.NativeClassPtr);
			NativeFieldInfoPtr___9 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9");
			NativeFieldInfoPtr___9__38_1 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__38_1");
			NativeFieldInfoPtr___9__44_2 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__44_2");
			NativeFieldInfoPtr___9__44_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__44_0");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100676465);
			NativeMethodInfoPtr__StartDrag_b__38_1_Internal_Void_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100676466);
			NativeMethodInfoPtr__EndDrag_b__44_2_Internal_Void_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100676467);
			NativeMethodInfoPtr__EndDrag_b__44_0_Internal_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100676468);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376355, XrefRangeEnd = 376357, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _StartDrag_b__38_1(PixelWindow w)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)w);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__StartDrag_b__38_1_Internal_Void_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _EndDrag_b__44_2(PixelWindow w)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)w);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__EndDrag_b__44_2_Internal_Void_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _EndDrag_b__44_0(TreeNodeRender n)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)n);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__EndDrag_b__44_0_Internal_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeFieldInfoPtr_current;

	private static readonly System.IntPtr NativeFieldInfoPtr_dragThreshold;

	private static readonly System.IntPtr NativeFieldInfoPtr_key;

	private static readonly System.IntPtr NativeFieldInfoPtr_rotateCWKey;

	private static readonly System.IntPtr NativeFieldInfoPtr_rotateCCWKey;

	private static readonly System.IntPtr NativeFieldInfoPtr_flipKey;

	private static readonly System.IntPtr NativeFieldInfoPtr_resetKey;

	private static readonly System.IntPtr NativeFieldInfoPtr_tooltipTranslation;

	private static readonly System.IntPtr NativeFieldInfoPtr_scrollMult10Key;

	private static readonly System.IntPtr NativeFieldInfoPtr_scrollMult100Key;

	private static readonly System.IntPtr NativeFieldInfoPtr_scrollIgnoreKey;

	private static readonly System.IntPtr NativeFieldInfoPtr_dragSnapAnimationSpeed;

	private static readonly System.IntPtr NativeFieldInfoPtr_currentItem;

	private static readonly System.IntPtr NativeFieldInfoPtr_lastMouseClickedPosition;

	private static readonly System.IntPtr NativeFieldInfoPtr_lastMouseClickedKey;

	private static readonly System.IntPtr NativeFieldInfoPtr_isDragging;

	private static readonly System.IntPtr NativeFieldInfoPtr_isMult10;

	private static readonly System.IntPtr NativeFieldInfoPtr_isMult100;

	private static readonly System.IntPtr NativeFieldInfoPtr_isScrollIgnore;

	private static readonly System.IntPtr NativeFieldInfoPtr_builder;

	private static readonly System.IntPtr NativeFieldInfoPtr_maxRemove;

	private static readonly System.IntPtr NativeFieldInfoPtr_currentRemove;

	private static readonly System.IntPtr NativeFieldInfoPtr_highlightedMayNodes;

	private static readonly System.IntPtr NativeFieldInfoPtr_lastItem;

	private static readonly System.IntPtr NativeFieldInfoPtr_lastInventory;

	private static readonly System.IntPtr NativeFieldInfoPtr_lastSlot;

	private static readonly System.IntPtr NativeFieldInfoPtr_highlightManager;

	private static readonly System.IntPtr NativeMethodInfoPtr_Awake_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_keyListeners_Public_Virtual_get_List_1_KeyCode_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_IsDraggingItem_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnEventPress_Protected_Virtual_Void_ISet_1_KeyCode_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnEventRelease_Protected_Virtual_Void_ISet_1_KeyCode_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnEventMouseMove_Protected_Virtual_Void_Vector3_Vector3_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FlipHeldItem_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RefreshDragHighlight_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnEventMouseScroll_Protected_Virtual_Void_Vector2_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnEventUpdate_Protected_Virtual_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnEventReset_Protected_Virtual_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsDragTargetDestroyed_Private_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AbortDrag_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_StartDrag_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Drag_Private_Void_Vector3_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_EndDrag_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CustomCancel_Public_Boolean_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Cancel_Public_Virtual_Final_New_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetCurrentItem_Public_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__EndDrag_b__44_1_Private_Boolean_PixelWindow_0;

	public unsafe static ItemMouseDragHandler current
	{
		get
		{
			Unsafe.SkipInit(out System.IntPtr intPtr);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_current, (void*)(&intPtr));
			System.IntPtr intPtr2 = intPtr;
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<ItemMouseDragHandler>(intPtr2) : null;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_current, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe float dragThreshold
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dragThreshold);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dragThreshold)) = value;
		}
	}

	public unsafe KeyCode key
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_key);
			return *(KeyCode*)num;
		}
		set
		{
			*(KeyCode*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_key)) = value;
		}
	}

	public unsafe KeyCode rotateCWKey
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rotateCWKey);
			return *(KeyCode*)num;
		}
		set
		{
			*(KeyCode*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rotateCWKey)) = value;
		}
	}

	public unsafe KeyCode rotateCCWKey
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rotateCCWKey);
			return *(KeyCode*)num;
		}
		set
		{
			*(KeyCode*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rotateCCWKey)) = value;
		}
	}

	public unsafe KeyCode flipKey
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_flipKey);
			return *(KeyCode*)num;
		}
		set
		{
			*(KeyCode*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_flipKey)) = value;
		}
	}

	public unsafe KeyCode resetKey
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_resetKey);
			return *(KeyCode*)num;
		}
		set
		{
			*(KeyCode*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_resetKey)) = value;
		}
	}

	public unsafe Vector2 tooltipTranslation
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tooltipTranslation);
			return *(Vector2*)num;
		}
		set
		{
			*(Vector2*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tooltipTranslation)) = value;
		}
	}

	public unsafe KeyCode scrollMult10Key
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollMult10Key);
			return *(KeyCode*)num;
		}
		set
		{
			*(KeyCode*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollMult10Key)) = value;
		}
	}

	public unsafe KeyCode scrollMult100Key
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollMult100Key);
			return *(KeyCode*)num;
		}
		set
		{
			*(KeyCode*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollMult100Key)) = value;
		}
	}

	public unsafe KeyCode scrollIgnoreKey
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollIgnoreKey);
			return *(KeyCode*)num;
		}
		set
		{
			*(KeyCode*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollIgnoreKey)) = value;
		}
	}

	public unsafe float dragSnapAnimationSpeed
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dragSnapAnimationSpeed);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dragSnapAnimationSpeed)) = value;
		}
	}

	public unsafe GameItemElement currentItem
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentItem);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItemElement>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentItem)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Vector3 lastMouseClickedPosition
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastMouseClickedPosition);
			return *(Vector3*)num;
		}
		set
		{
			*(Vector3*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastMouseClickedPosition)) = value;
		}
	}

	public unsafe KeyCode lastMouseClickedKey
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastMouseClickedKey);
			return *(KeyCode*)num;
		}
		set
		{
			*(KeyCode*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastMouseClickedKey)) = value;
		}
	}

	public unsafe bool isDragging
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isDragging);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isDragging)) = value;
		}
	}

	public unsafe bool isMult10
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isMult10);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isMult10)) = value;
		}
	}

	public unsafe bool isMult100
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isMult100);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isMult100)) = value;
		}
	}

	public unsafe bool isScrollIgnore
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isScrollIgnore);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isScrollIgnore)) = value;
		}
	}

	public unsafe RichTextBuilder builder
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_builder);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<RichTextBuilder>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_builder)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int maxRemove
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_maxRemove);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_maxRemove)) = value;
		}
	}

	public unsafe int currentRemove
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentRemove);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentRemove)) = value;
		}
	}

	public unsafe List<TreeNodeRender> highlightedMayNodes
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlightedMayNodes);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlightedMayNodes)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe GameItem lastItem
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastItem);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastItem)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe GameInventory lastInventory
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastInventory);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameInventory>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastInventory)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe SlotMarker lastSlot
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastSlot);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_lastSlot)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe HighlightManager highlightManager
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlightManager);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<HighlightManager>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlightManager)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe override List<KeyCode> keyListeners
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376361, XrefRangeEnd = 376369, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_keyListeners_Public_Virtual_get_List_1_KeyCode_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<KeyCode>>(intPtr2) : null;
		}
	}

	public unsafe bool IsDraggingItem
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_IsDraggingItem_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	static ItemMouseDragHandler()
	{
		Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "ItemMouseDragHandler");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr);
		NativeFieldInfoPtr_current = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "current");
		NativeFieldInfoPtr_dragThreshold = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "dragThreshold");
		NativeFieldInfoPtr_key = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "key");
		NativeFieldInfoPtr_rotateCWKey = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "rotateCWKey");
		NativeFieldInfoPtr_rotateCCWKey = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "rotateCCWKey");
		NativeFieldInfoPtr_flipKey = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "flipKey");
		NativeFieldInfoPtr_resetKey = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "resetKey");
		NativeFieldInfoPtr_tooltipTranslation = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "tooltipTranslation");
		NativeFieldInfoPtr_scrollMult10Key = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "scrollMult10Key");
		NativeFieldInfoPtr_scrollMult100Key = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "scrollMult100Key");
		NativeFieldInfoPtr_scrollIgnoreKey = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "scrollIgnoreKey");
		NativeFieldInfoPtr_dragSnapAnimationSpeed = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "dragSnapAnimationSpeed");
		NativeFieldInfoPtr_currentItem = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "currentItem");
		NativeFieldInfoPtr_lastMouseClickedPosition = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "lastMouseClickedPosition");
		NativeFieldInfoPtr_lastMouseClickedKey = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "lastMouseClickedKey");
		NativeFieldInfoPtr_isDragging = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "isDragging");
		NativeFieldInfoPtr_isMult10 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "isMult10");
		NativeFieldInfoPtr_isMult100 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "isMult100");
		NativeFieldInfoPtr_isScrollIgnore = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "isScrollIgnore");
		NativeFieldInfoPtr_builder = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "builder");
		NativeFieldInfoPtr_maxRemove = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "maxRemove");
		NativeFieldInfoPtr_currentRemove = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "currentRemove");
		NativeFieldInfoPtr_highlightedMayNodes = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "highlightedMayNodes");
		NativeFieldInfoPtr_lastItem = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "lastItem");
		NativeFieldInfoPtr_lastInventory = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "lastInventory");
		NativeFieldInfoPtr_lastSlot = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "lastSlot");
		NativeFieldInfoPtr_highlightManager = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, "highlightManager");
		NativeMethodInfoPtr_Awake_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676440);
		NativeMethodInfoPtr_get_keyListeners_Public_Virtual_get_List_1_KeyCode_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676441);
		NativeMethodInfoPtr_get_IsDraggingItem_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676442);
		NativeMethodInfoPtr_OnEventPress_Protected_Virtual_Void_ISet_1_KeyCode_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676443);
		NativeMethodInfoPtr_OnEventRelease_Protected_Virtual_Void_ISet_1_KeyCode_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676444);
		NativeMethodInfoPtr_OnEventMouseMove_Protected_Virtual_Void_Vector3_Vector3_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676445);
		NativeMethodInfoPtr_FlipHeldItem_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676446);
		NativeMethodInfoPtr_RefreshDragHighlight_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676447);
		NativeMethodInfoPtr_OnEventMouseScroll_Protected_Virtual_Void_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676448);
		NativeMethodInfoPtr_OnEventUpdate_Protected_Virtual_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676449);
		NativeMethodInfoPtr_OnEventReset_Protected_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676450);
		NativeMethodInfoPtr_IsDragTargetDestroyed_Private_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676451);
		NativeMethodInfoPtr_AbortDrag_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676452);
		NativeMethodInfoPtr_StartDrag_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676453);
		NativeMethodInfoPtr_Drag_Private_Void_Vector3_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676454);
		NativeMethodInfoPtr_EndDrag_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676455);
		NativeMethodInfoPtr_CustomCancel_Public_Boolean_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676456);
		NativeMethodInfoPtr_Cancel_Public_Virtual_Final_New_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676457);
		NativeMethodInfoPtr_GetCurrentItem_Public_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676458);
		NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676459);
		NativeMethodInfoPtr__EndDrag_b__44_1_Private_Boolean_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr, 100676460);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376357, XrefRangeEnd = 376361, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Awake()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Awake_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376369, XrefRangeEnd = 376390, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)keysPressed);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventPress_Protected_Virtual_Void_ISet_1_KeyCode_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376390, XrefRangeEnd = 376401, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnEventRelease(ISet<KeyCode> keysReleased)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)keysReleased);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventRelease_Protected_Virtual_Void_ISet_1_KeyCode_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376401, XrefRangeEnd = 376416, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnEventMouseMove(Vector3 mousePosition, Vector3 mouseDelta)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&mousePosition);
		*(Vector3**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &mouseDelta;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventMouseMove_Protected_Virtual_Void_Vector3_Vector3_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 376419, RefRangeEnd = 376420, XrefRangeStart = 376416, XrefRangeEnd = 376419, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void FlipHeldItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FlipHeldItem_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 376423, RefRangeEnd = 376424, XrefRangeStart = 376420, XrefRangeEnd = 376423, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RefreshDragHighlight()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RefreshDragHighlight_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376424, XrefRangeEnd = 376441, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnEventMouseScroll(Vector2 scrollDelta)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&scrollDelta);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventMouseScroll_Protected_Virtual_Void_Vector2_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376441, XrefRangeEnd = 376443, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool OnEventUpdate()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventUpdate_Protected_Virtual_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376443, XrefRangeEnd = 376450, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnEventReset()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventReset_Protected_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	public unsafe bool IsDragTargetDestroyed()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsDragTargetDestroyed_Private_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 376474, RefRangeEnd = 376477, XrefRangeStart = 376450, XrefRangeEnd = 376474, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AbortDrag()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AbortDrag_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 376591, RefRangeEnd = 376592, XrefRangeStart = 376477, XrefRangeEnd = 376591, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void StartDrag()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_StartDrag_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 376679, RefRangeEnd = 376680, XrefRangeStart = 376592, XrefRangeEnd = 376679, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Drag(Vector3 mouseDelta)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&mouseDelta);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Drag_Private_Void_Vector3_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 376783, RefRangeEnd = 376784, XrefRangeStart = 376680, XrefRangeEnd = 376783, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void EndDrag()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_EndDrag_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 376788, RefRangeEnd = 376790, XrefRangeStart = 376784, XrefRangeEnd = 376788, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CustomCancel(bool nonOwnedOnly = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&nonOwnedOnly);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CustomCancel_Public_Boolean_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 376790, RefRangeEnd = 376791, XrefRangeStart = 376790, XrefRangeEnd = 376790, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool Cancel()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Cancel_Public_Virtual_Final_New_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	public unsafe GameItem GetCurrentItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetCurrentItem_Public_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376791, XrefRangeEnd = 376810, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe ItemMouseDragHandler()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<ItemMouseDragHandler>.NativeClassPtr))
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 376810, XrefRangeEnd = 376813, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool _EndDrag_b__44_1(PixelWindow w)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)w);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__EndDrag_b__44_1_Private_Boolean_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	public ItemMouseDragHandler(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
