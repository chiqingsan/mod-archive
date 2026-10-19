using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;

namespace Il2Cpp;

public class SlotMarker : Il2CppSystem.Object
{
	private static readonly System.IntPtr NativeFieldInfoPtr_item;

	private static readonly System.IntPtr NativeFieldInfoPtr_inventory;

	private static readonly System.IntPtr NativeFieldInfoPtr_itemGridShape;

	private static readonly System.IntPtr NativeFieldInfoPtr_index;

	private static readonly System.IntPtr NativeFieldInfoPtr_targetItem;

	private static readonly System.IntPtr NativeFieldInfoPtr_numTransfer;

	private static readonly System.IntPtr NativeFieldInfoPtr_priority;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsValid_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsEquipmentSlot_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_StackItemUnchecked_Public_Static_Void_GameItem_GameItem_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryAcceptOnce_Public_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AcceptUnchecked_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

	public unsafe GameItem item
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_item);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_item)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe GameInventory inventory
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_inventory);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameInventory>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_inventory)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe GridShape itemGridShape
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_itemGridShape);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_itemGridShape)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int index
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_index);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_index)) = value;
		}
	}

	public unsafe GameItem targetItem
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_targetItem);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_targetItem)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int numTransfer
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_numTransfer);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_numTransfer)) = value;
		}
	}

	public unsafe int priority
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_priority);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_priority)) = value;
		}
	}

	static SlotMarker()
	{
		Il2CppClassPointerStore<SlotMarker>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "SlotMarker");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr);
		NativeFieldInfoPtr_item = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, "item");
		NativeFieldInfoPtr_inventory = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, "inventory");
		NativeFieldInfoPtr_itemGridShape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, "itemGridShape");
		NativeFieldInfoPtr_index = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, "index");
		NativeFieldInfoPtr_targetItem = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, "targetItem");
		NativeFieldInfoPtr_numTransfer = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, "numTransfer");
		NativeFieldInfoPtr_priority = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, "priority");
		NativeMethodInfoPtr_IsValid_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, 100675646);
		NativeMethodInfoPtr_IsEquipmentSlot_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, 100675647);
		NativeMethodInfoPtr_StackItemUnchecked_Public_Static_Void_GameItem_GameItem_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, 100675648);
		NativeMethodInfoPtr_TryAcceptOnce_Public_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, 100675649);
		NativeMethodInfoPtr_AcceptUnchecked_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, 100675650);
		NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr, 100675651);
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 364623, RefRangeEnd = 364628, XrefRangeStart = 364623, XrefRangeEnd = 364623, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsValid()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsValid_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 364632, RefRangeEnd = 364633, XrefRangeStart = 364628, XrefRangeEnd = 364632, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsEquipmentSlot()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsEquipmentSlot_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 364639, RefRangeEnd = 364641, XrefRangeStart = 364633, XrefRangeEnd = 364639, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void StackItemUnchecked(GameItem item, GameItem targetItem, int amount = -1)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)targetItem);
		*(int**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &amount;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_StackItemUnchecked_Public_Static_Void_GameItem_GameItem_Int32_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(46)]
	[CachedScanResults(RefRangeStart = 364659, RefRangeEnd = 364705, XrefRangeStart = 364641, XrefRangeEnd = 364659, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int TryAcceptOnce(int amount = -1)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&amount);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TryAcceptOnce_Public_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 364712, RefRangeEnd = 364713, XrefRangeStart = 364705, XrefRangeEnd = 364712, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int AcceptUnchecked()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AcceptUnchecked_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(25)]
	[CachedScanResults(RefRangeStart = 364714, RefRangeEnd = 364739, XrefRangeStart = 364713, XrefRangeEnd = 364714, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe SlotMarker()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<SlotMarker>.NativeClassPtr))
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	public SlotMarker(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
