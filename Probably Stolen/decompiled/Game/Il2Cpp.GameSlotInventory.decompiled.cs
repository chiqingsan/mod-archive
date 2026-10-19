using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Il2Cpp;

public class GameSlotInventory : GameInventory
{
	private static readonly IntPtr NativeFieldInfoPtr__widthPixels_k__BackingField;

	private static readonly IntPtr NativeFieldInfoPtr__heightPixels_k__BackingField;

	private static readonly IntPtr NativeFieldInfoPtr__handler_k__BackingField;

	private static readonly IntPtr NativeFieldInfoPtr__inner_k__BackingField;

	private static readonly IntPtr NativeFieldInfoPtr_stealUI;

	private static readonly IntPtr NativeFieldInfoPtr_isUIDown;

	private static readonly IntPtr NativeFieldInfoPtr_stolenUI;

	private static readonly IntPtr NativeFieldInfoPtr_defaultOrientation;

	private static readonly IntPtr NativeFieldInfoPtr_background;

	private static readonly IntPtr NativeFieldInfoPtr_insideBorder;

	private static readonly IntPtr NativeFieldInfoPtr_stolenInner;

	private static readonly IntPtr NativeFieldInfoPtr_backgroundImage;

	private static readonly IntPtr NativeFieldInfoPtr_currentItem;

	private static readonly IntPtr NativeFieldInfoPtr_highlight;

	private static readonly IntPtr NativeFieldInfoPtr_defaultGridWidth;

	private static readonly IntPtr NativeFieldInfoPtr_defaultGridHeight;

	private static readonly IntPtr NativeFieldInfoPtr_tooltipName;

	private static readonly IntPtr NativeFieldInfoPtr__transparency_k__BackingField;

	private static readonly IntPtr NativeFieldInfoPtr__isHidden_k__BackingField;

	private static readonly IntPtr NativeFieldInfoPtr_validateLock;

	private static readonly IntPtr NativeMethodInfoPtr_get_children_Public_Virtual_get_List_1_GraphNodeStorage_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_childItems_Public_Virtual_get_List_1_GameItem_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0;

	private static readonly IntPtr NativeMethodInfoPtr_set_widthPixels_Private_set_Void_Int32_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0;

	private static readonly IntPtr NativeMethodInfoPtr_set_heightPixels_Private_set_Void_Int32_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0;

	private static readonly IntPtr NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0;

	private static readonly IntPtr NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_childItem_Public_get_GameItem_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_transparency_Public_get_Single_0;

	private static readonly IntPtr NativeMethodInfoPtr_set_transparency_Private_set_Void_Single_0;

	private static readonly IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Boolean_Boolean_0;

	private static readonly IntPtr NativeMethodInfoPtr__ctor_Public_Void_String_String_Boolean_Boolean_0;

	private static readonly IntPtr NativeMethodInfoPtr_SetTransparency_Public_GameSlotInventory_Single_0;

	private static readonly IntPtr NativeMethodInfoPtr_SetBackgroundFadeSprite_Public_GameSlotInventory_String_String_Single_Single_0;

	private static readonly IntPtr NativeMethodInfoPtr_SetBackgroundSprite_Public_GameSlotInventory_String_String_Color_0;

	private static readonly IntPtr NativeMethodInfoPtr_ClearBackgroundSprite_Public_GameSlotInventory_0;

	private static readonly IntPtr NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_isHidden_Public_get_Boolean_0;

	private static readonly IntPtr NativeMethodInfoPtr_set_isHidden_Private_set_Void_Boolean_0;

	private static readonly IntPtr NativeMethodInfoPtr_Hide_Public_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_Show_Public_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_Validate_Public_Virtual_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_AddParent_Public_Virtual_Boolean_GraphNodeStorage_0;

	private static readonly IntPtr NativeMethodInfoPtr_RemoveParent_Public_Virtual_Boolean_GraphNodeStorage_0;

	private static readonly IntPtr NativeMethodInfoPtr_IsAdjacent_Public_Virtual_Boolean_SlotMarker_SlotMarker_0;

	private static readonly IntPtr NativeMethodInfoPtr_HighlightSlot_Public_Virtual_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0;

	private static readonly IntPtr NativeMethodInfoPtr_ClearHighlight_Public_Virtual_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_List_1_TreeNodeRender_BackgroundColor_0;

	private static readonly IntPtr NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Boolean_GameItem_0;

	private static readonly IntPtr NativeMethodInfoPtr_GetLocalPosition_Private_Vector2_Vector2_0;

	private static readonly IntPtr NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Virtual_Boolean_GameItem_GridShape_TagSystem_0;

	private static readonly IntPtr NativeMethodInfoPtr_TryInventorySlot_Public_Virtual_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0;

	private static readonly IntPtr NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Virtual_SlotMarker_GameItem_0;

	private static readonly IntPtr NativeMethodInfoPtr_UncheckedAccept_Public_Virtual_Boolean_GameItem_0;

	private static readonly IntPtr NativeMethodInfoPtr_Expel_Public_Virtual_Boolean_GameItem_0;

	private static readonly IntPtr NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_SlotMarker_GameItem_0;

	private static readonly IntPtr NativeMethodInfoPtr_SetTooltipName_Public_GameSlotInventory_String_0;

	private static readonly IntPtr NativeMethodInfoPtr_HasTooltip_Public_Virtual_Final_New_Boolean_0;

	private static readonly IntPtr NativeMethodInfoPtr_GetTooltip_Public_Virtual_Final_New_RichText_0;

	private static readonly IntPtr NativeMethodInfoPtr_GetTooltipAdvanced_Public_Virtual_Final_New_RichText_0;

	private static readonly IntPtr NativeMethodInfoPtr___ctor_b__44_0_Private_Boolean_PointerEventData_0;

	public unsafe int _widthPixels_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__widthPixels_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__widthPixels_k__BackingField)) = value;
		}
	}

	public unsafe int _heightPixels_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__heightPixels_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__heightPixels_k__BackingField)) = value;
		}
	}

	public unsafe TreeNodeRender _handler_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__handler_k__BackingField);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__handler_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender _inner_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__inner_k__BackingField);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__inner_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe bool stealUI
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stealUI);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stealUI)) = value;
		}
	}

	public unsafe bool isUIDown
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isUIDown);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isUIDown)) = value;
		}
	}

	public unsafe PixelElement stolenUI
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stolenUI);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<PixelElement>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stolenUI)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int defaultOrientation
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_defaultOrientation);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_defaultOrientation)) = value;
		}
	}

	public unsafe TreeNodeRender background
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_background);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_background)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender insideBorder
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_insideBorder);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_insideBorder)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender stolenInner
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stolenInner);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stolenInner)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender backgroundImage
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_backgroundImage);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_backgroundImage)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe GameItem currentItem
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentItem);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentItem)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender highlight
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlight);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlight)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int defaultGridWidth
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_defaultGridWidth);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_defaultGridWidth)) = value;
		}
	}

	public unsafe int defaultGridHeight
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_defaultGridHeight);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_defaultGridHeight)) = value;
		}
	}

	public unsafe string tooltipName
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tooltipName);
			return IL2CPP.Il2CppStringToManaged(*(IntPtr*)num);
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tooltipName)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe float _transparency_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__transparency_k__BackingField);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__transparency_k__BackingField)) = value;
		}
	}

	public unsafe bool _isHidden_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__isHidden_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__isHidden_k__BackingField)) = value;
		}
	}

	public unsafe bool validateLock
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_validateLock);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_validateLock)) = value;
		}
	}

	public unsafe override List<GraphNodeStorage> children
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350831, XrefRangeEnd = 350845, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_children_Public_Virtual_get_List_1_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<List<GraphNodeStorage>>(intPtr2) : null;
		}
	}

	public unsafe override List<GameItem> childItems
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350845, XrefRangeEnd = 350854, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_childItems_Public_Virtual_get_List_1_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<List<GameItem>>(intPtr2) : null;
		}
	}

	public unsafe virtual bool canResize
	{
		[CallerCount(76)]
		[CachedScanResults(RefRangeStart = 16920, RefRangeEnd = 16996, XrefRangeStart = 16920, XrefRangeEnd = 16996, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int widthPixels
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = stackalloc IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_widthPixels_Private_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual int heightPixels
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = stackalloc IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_heightPixels_Private_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual TreeNodeRender handler
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = stackalloc IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual RectTransform rectTransform
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<RectTransform>(intPtr2) : null;
		}
	}

	public unsafe virtual TreeNodeRender inner
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = stackalloc IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe GameItem childItem
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_childItem_Public_get_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
		}
	}

	public unsafe float transparency
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_transparency_Public_get_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(float*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = stackalloc IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_transparency_Private_set_Void_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe bool isHidden
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_isHidden_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = stackalloc IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_isHidden_Private_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	static GameSlotInventory()
	{
		Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "GameSlotInventory");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr);
		NativeFieldInfoPtr__widthPixels_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "<widthPixels>k__BackingField");
		NativeFieldInfoPtr__heightPixels_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "<heightPixels>k__BackingField");
		NativeFieldInfoPtr__handler_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "<handler>k__BackingField");
		NativeFieldInfoPtr__inner_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "<inner>k__BackingField");
		NativeFieldInfoPtr_stealUI = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "stealUI");
		NativeFieldInfoPtr_isUIDown = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "isUIDown");
		NativeFieldInfoPtr_stolenUI = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "stolenUI");
		NativeFieldInfoPtr_defaultOrientation = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "defaultOrientation");
		NativeFieldInfoPtr_background = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "background");
		NativeFieldInfoPtr_insideBorder = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "insideBorder");
		NativeFieldInfoPtr_stolenInner = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "stolenInner");
		NativeFieldInfoPtr_backgroundImage = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "backgroundImage");
		NativeFieldInfoPtr_currentItem = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "currentItem");
		NativeFieldInfoPtr_highlight = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "highlight");
		NativeFieldInfoPtr_defaultGridWidth = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "defaultGridWidth");
		NativeFieldInfoPtr_defaultGridHeight = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "defaultGridHeight");
		NativeFieldInfoPtr_tooltipName = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "tooltipName");
		NativeFieldInfoPtr__transparency_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "<transparency>k__BackingField");
		NativeFieldInfoPtr__isHidden_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "<isHidden>k__BackingField");
		NativeFieldInfoPtr_validateLock = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, "validateLock");
		NativeMethodInfoPtr_get_children_Public_Virtual_get_List_1_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674965);
		NativeMethodInfoPtr_get_childItems_Public_Virtual_get_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674966);
		NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674967);
		NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674968);
		NativeMethodInfoPtr_set_widthPixels_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674969);
		NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674970);
		NativeMethodInfoPtr_set_heightPixels_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674971);
		NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674972);
		NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674973);
		NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674974);
		NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674975);
		NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674976);
		NativeMethodInfoPtr_get_childItem_Public_get_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674977);
		NativeMethodInfoPtr_get_transparency_Public_get_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674978);
		NativeMethodInfoPtr_set_transparency_Private_set_Void_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674979);
		NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674980);
		NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Boolean_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674981);
		NativeMethodInfoPtr__ctor_Public_Void_String_String_Boolean_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674982);
		NativeMethodInfoPtr_SetTransparency_Public_GameSlotInventory_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674983);
		NativeMethodInfoPtr_SetBackgroundFadeSprite_Public_GameSlotInventory_String_String_Single_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674984);
		NativeMethodInfoPtr_SetBackgroundSprite_Public_GameSlotInventory_String_String_Color_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674985);
		NativeMethodInfoPtr_ClearBackgroundSprite_Public_GameSlotInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674986);
		NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674987);
		NativeMethodInfoPtr_get_isHidden_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674988);
		NativeMethodInfoPtr_set_isHidden_Private_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674989);
		NativeMethodInfoPtr_Hide_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674990);
		NativeMethodInfoPtr_Show_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674991);
		NativeMethodInfoPtr_Validate_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674992);
		NativeMethodInfoPtr_AddParent_Public_Virtual_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674993);
		NativeMethodInfoPtr_RemoveParent_Public_Virtual_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674994);
		NativeMethodInfoPtr_IsAdjacent_Public_Virtual_Boolean_SlotMarker_SlotMarker_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674995);
		NativeMethodInfoPtr_HighlightSlot_Public_Virtual_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674996);
		NativeMethodInfoPtr_ClearHighlight_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674997);
		NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_List_1_TreeNodeRender_BackgroundColor_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674998);
		NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100674999);
		NativeMethodInfoPtr_GetLocalPosition_Private_Vector2_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675000);
		NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Virtual_Boolean_GameItem_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675001);
		NativeMethodInfoPtr_TryInventorySlot_Public_Virtual_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675002);
		NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Virtual_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675003);
		NativeMethodInfoPtr_UncheckedAccept_Public_Virtual_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675004);
		NativeMethodInfoPtr_Expel_Public_Virtual_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675005);
		NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675006);
		NativeMethodInfoPtr_SetTooltipName_Public_GameSlotInventory_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675007);
		NativeMethodInfoPtr_HasTooltip_Public_Virtual_Final_New_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675008);
		NativeMethodInfoPtr_GetTooltip_Public_Virtual_Final_New_RichText_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675009);
		NativeMethodInfoPtr_GetTooltipAdvanced_Public_Virtual_Final_New_RichText_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675010);
		NativeMethodInfoPtr___ctor_b__44_0_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr, 100675011);
	}

	[CallerCount(63)]
	[CachedScanResults(RefRangeStart = 350855, RefRangeEnd = 350918, XrefRangeStart = 350854, XrefRangeEnd = 350855, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameSlotInventory()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr))
	{
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(20)]
	[CachedScanResults(RefRangeStart = 350959, RefRangeEnd = 350979, XrefRangeStart = 350918, XrefRangeEnd = 350959, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameSlotInventory(int defaultGridWidth, int defaultGridHeight, bool stealUI = false, bool isUIDown = false)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr))
	{
		IntPtr* ptr = stackalloc IntPtr[4];
		*ptr = (nint)(&defaultGridWidth);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = &defaultGridHeight;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(IntPtr)))) = &stealUI;
		*(bool**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(IntPtr)))) = &isUIDown;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Boolean_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(23)]
	[CachedScanResults(RefRangeStart = 350990, RefRangeEnd = 351013, XrefRangeStart = 350979, XrefRangeEnd = 350990, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameSlotInventory(string backgroundAtlas, string backgroundSprite, bool stealUI = false, bool isUIDown = false)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameSlotInventory>.NativeClassPtr))
	{
		IntPtr* ptr = stackalloc IntPtr[4];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(backgroundAtlas);
		*(IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(backgroundSprite);
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(IntPtr)))) = &stealUI;
		*(bool**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(IntPtr)))) = &isUIDown;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_String_String_Boolean_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(7)]
	[CachedScanResults(RefRangeStart = 351013, RefRangeEnd = 351020, XrefRangeStart = 351013, XrefRangeEnd = 351013, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameSlotInventory SetTransparency(float transparency)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = (nint)(&transparency);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetTransparency_Public_GameSlotInventory_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<GameSlotInventory>(intPtr2) : null;
	}

	[CallerCount(65)]
	[CachedScanResults(RefRangeStart = 351029, RefRangeEnd = 351094, XrefRangeStart = 351020, XrefRangeEnd = 351029, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameSlotInventory SetBackgroundFadeSprite(string backgroundAtlas, string backgroundSprite, float opacity = 0.5f, float lightness = 0.5f)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[4];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(backgroundAtlas);
		*(IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(backgroundSprite);
		*(float**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(IntPtr)))) = &opacity;
		*(float**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(IntPtr)))) = &lightness;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetBackgroundFadeSprite_Public_GameSlotInventory_String_String_Single_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<GameSlotInventory>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351094, XrefRangeEnd = 351103, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameSlotInventory SetBackgroundSprite(string backgroundAtlas, string backgroundSprite, Color color)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[3];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(backgroundAtlas);
		*(IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(backgroundSprite);
		*(Color**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(IntPtr)))) = &color;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetBackgroundSprite_Public_GameSlotInventory_String_String_Color_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<GameSlotInventory>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351103, XrefRangeEnd = 351104, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameSlotInventory ClearBackgroundSprite()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ClearBackgroundSprite_Public_GameSlotInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<GameSlotInventory>(intPtr2) : null;
	}

	[CallerCount(10210)]
	[CachedScanResults(RefRangeStart = 18631, RefRangeEnd = 28841, XrefRangeStart = 18631, XrefRangeEnd = 28841, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual void ResizePixels(int width, int height)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[2];
		*ptr = (nint)(&width);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = &height;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	public unsafe void Hide()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Hide_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	public unsafe void Show()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Show_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351104, XrefRangeEnd = 351154, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void Validate()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Validate_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool AddParent(GraphNodeStorage node)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)node);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_AddParent_Public_Virtual_Boolean_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool RemoveParent(GraphNodeStorage node)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)node);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_RemoveParent_Public_Virtual_Boolean_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(76)]
	[CachedScanResults(RefRangeStart = 16920, RefRangeEnd = 16996, XrefRangeStart = 16920, XrefRangeEnd = 16996, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool IsAdjacent(SlotMarker slot, SlotMarker slot2)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot);
		*(IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot2);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_IsAdjacent_Public_Virtual_Boolean_SlotMarker_SlotMarker_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351154, XrefRangeEnd = 351169, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override List<TreeNodeRender> HighlightSlot(SlotMarker slot, RenderHandler.BackgroundColor color, bool doReplace)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot);
		*(RenderHandler.BackgroundColor**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = &color;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(IntPtr)))) = &doReplace;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_HighlightSlot_Public_Virtual_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351169, XrefRangeEnd = 351174, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void ClearHighlight()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_ClearHighlight_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351174, XrefRangeEnd = 351179, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override List<TreeNodeRender> HighlightWholeInventory(RenderHandler.BackgroundColor color)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = (nint)(&color);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_List_1_TreeNodeRender_BackgroundColor_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351179, XrefRangeEnd = 351200, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool MayHaveValidInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Boolean_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Vector2 GetLocalPosition(Vector2 position)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = (nint)(&position);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetLocalPosition_Private_Vector2_Vector2_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(Vector2*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351200, XrefRangeEnd = 351216, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool CheckInventorySlotStillValid(GameItem item, GridShape tempShape, TagSystem tempState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempShape);
		*(IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempState);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Virtual_Boolean_GameItem_GridShape_TagSystem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351216, XrefRangeEnd = 351248, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override SlotMarker TryInventorySlot(GameItem item, int requestedNum, Vector2 itemGraphicsCenterPosition, GridShape tempShape, TagSystem tempState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[5];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(IntPtr)))) = &requestedNum;
		*(Vector2**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(IntPtr)))) = &itemGraphicsCenterPosition;
		*(IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempShape);
		*(IntPtr*)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempState);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_TryInventorySlot_Public_Virtual_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351248, XrefRangeEnd = 351258, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override SlotMarker GetIncompleteItemInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Virtual_SlotMarker_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351258, XrefRangeEnd = 351287, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool UncheckedAccept(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_UncheckedAccept_Public_Virtual_Boolean_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351287, XrefRangeEnd = 351310, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool Expel(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Expel_Public_Virtual_Boolean_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351310, XrefRangeEnd = 351332, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_SlotMarker_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(20)]
	[CachedScanResults(RefRangeStart = 351333, RefRangeEnd = 351353, XrefRangeStart = 351332, XrefRangeEnd = 351333, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameSlotInventory SetTooltipName(string name)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(name);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetTooltipName_Public_GameSlotInventory_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<GameSlotInventory>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe virtual bool HasTooltip()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_HasTooltip_Public_Virtual_Final_New_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351353, XrefRangeEnd = 351360, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual RichText GetTooltip()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetTooltip_Public_Virtual_Final_New_RichText_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<RichText>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual RichText GetTooltipAdvanced()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetTooltipAdvanced_Public_Virtual_Final_New_RichText_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<RichText>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 351360, XrefRangeEnd = 351362, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__44_0(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__44_0_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	public GameSlotInventory(IntPtr pointer)
		: base(pointer)
	{
	}
}
