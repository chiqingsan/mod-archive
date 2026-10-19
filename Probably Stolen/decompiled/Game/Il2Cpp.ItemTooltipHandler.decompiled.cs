using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace Il2Cpp;

public class ItemTooltipHandler : EmptyActionHandler
{
	private static readonly IntPtr NativeFieldInfoPtr_current;

	private static readonly IntPtr NativeFieldInfoPtr_key;

	private static readonly IntPtr NativeFieldInfoPtr_tooltipTranslation;

	private static readonly IntPtr NativeFieldInfoPtr_BASE_TOOLTIP_CHAR_WIDTH;

	private static readonly IntPtr NativeFieldInfoPtr_MACHINE_TOOLTIP_WIDTH_MULT;

	private static readonly IntPtr NativeFieldInfoPtr_advanced;

	private static readonly IntPtr NativeFieldInfoPtr_currentTooltipListener;

	private static readonly IntPtr NativeMethodInfoPtr_Awake_Private_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr_get_keyListeners_Public_Virtual_get_List_1_KeyCode_0;

	private static readonly IntPtr NativeMethodInfoPtr_OnEventPress_Protected_Virtual_Void_ISet_1_KeyCode_0;

	private static readonly IntPtr NativeMethodInfoPtr_OnEventRelease_Protected_Virtual_Void_ISet_1_KeyCode_0;

	private static readonly IntPtr NativeMethodInfoPtr_OnEventUpdate_Protected_Virtual_Boolean_0;

	private static readonly IntPtr NativeMethodInfoPtr_OnEventReset_Protected_Virtual_Void_0;

	private static readonly IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

	public unsafe static ItemTooltipHandler current
	{
		get
		{
			Unsafe.SkipInit(out IntPtr intPtr);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_current, (void*)(&intPtr));
			IntPtr intPtr2 = intPtr;
			return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<ItemTooltipHandler>(intPtr2) : null;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_current, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
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

	public unsafe static int BASE_TOOLTIP_CHAR_WIDTH
	{
		get
		{
			Unsafe.SkipInit(out int result);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_BASE_TOOLTIP_CHAR_WIDTH, (void*)(&result));
			return result;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_BASE_TOOLTIP_CHAR_WIDTH, (void*)(&value));
		}
	}

	public unsafe static float MACHINE_TOOLTIP_WIDTH_MULT
	{
		get
		{
			Unsafe.SkipInit(out float result);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_MACHINE_TOOLTIP_WIDTH_MULT, (void*)(&result));
			return result;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_MACHINE_TOOLTIP_WIDTH_MULT, (void*)(&value));
		}
	}

	public unsafe bool advanced
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_advanced);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_advanced)) = value;
		}
	}

	public unsafe TooltipListener currentTooltipListener
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentTooltipListener);
			IntPtr intPtr = *(IntPtr*)num;
			return (intPtr != (IntPtr)0) ? Il2CppObjectPool.Get<TooltipListener>(intPtr) : null;
		}
		set
		{
			IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_currentTooltipListener)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe override List<KeyCode> keyListeners
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 378789, XrefRangeEnd = 378797, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IntPtr* ptr = null;
			Unsafe.SkipInit(out IntPtr intPtr);
			IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_keyListeners_Public_Virtual_get_List_1_KeyCode_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (IntPtr)0) ? Il2CppObjectPool.Get<List<KeyCode>>(intPtr2) : null;
		}
	}

	static ItemTooltipHandler()
	{
		Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "ItemTooltipHandler");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr);
		NativeFieldInfoPtr_current = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, "current");
		NativeFieldInfoPtr_key = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, "key");
		NativeFieldInfoPtr_tooltipTranslation = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, "tooltipTranslation");
		NativeFieldInfoPtr_BASE_TOOLTIP_CHAR_WIDTH = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, "BASE_TOOLTIP_CHAR_WIDTH");
		NativeFieldInfoPtr_MACHINE_TOOLTIP_WIDTH_MULT = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, "MACHINE_TOOLTIP_WIDTH_MULT");
		NativeFieldInfoPtr_advanced = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, "advanced");
		NativeFieldInfoPtr_currentTooltipListener = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, "currentTooltipListener");
		NativeMethodInfoPtr_Awake_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, 100676580);
		NativeMethodInfoPtr_get_keyListeners_Public_Virtual_get_List_1_KeyCode_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, 100676581);
		NativeMethodInfoPtr_OnEventPress_Protected_Virtual_Void_ISet_1_KeyCode_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, 100676582);
		NativeMethodInfoPtr_OnEventRelease_Protected_Virtual_Void_ISet_1_KeyCode_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, 100676583);
		NativeMethodInfoPtr_OnEventUpdate_Protected_Virtual_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, 100676584);
		NativeMethodInfoPtr_OnEventReset_Protected_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, 100676585);
		NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr, 100676586);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 378785, XrefRangeEnd = 378789, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Awake()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Awake_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 378797, XrefRangeEnd = 378798, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnEventPress(ISet<KeyCode> keysPressed)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)keysPressed);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventPress_Protected_Virtual_Void_ISet_1_KeyCode_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 378798, XrefRangeEnd = 378799, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnEventRelease(ISet<KeyCode> keysReleased)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = stackalloc IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)keysReleased);
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventRelease_Protected_Virtual_Void_ISet_1_KeyCode_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 378799, XrefRangeEnd = 378823, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool OnEventUpdate()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventUpdate_Protected_Virtual_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 378823, XrefRangeEnd = 378830, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void OnEventReset()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_OnEventReset_Protected_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 378830, XrefRangeEnd = 378831, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe ItemTooltipHandler()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<ItemTooltipHandler>.NativeClassPtr))
	{
		IntPtr* ptr = null;
		Unsafe.SkipInit(out IntPtr intPtr);
		IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	public ItemTooltipHandler(IntPtr pointer)
		: base(pointer)
	{
	}
}
