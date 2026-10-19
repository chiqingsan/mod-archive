using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;

namespace Il2Cpp;

public static class InventorySortHelper : Il2CppSystem.Object
{
	[ObfuscatedName("InventorySortHelper+<>c__DisplayClass2_0")]
	public sealed class __c__DisplayClass2_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_bigFirst;

		private static readonly System.IntPtr NativeFieldInfoPtr_inventoryShape;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__Sort_b__0_Internal_Int32_GameItem_GameItem_0;

		public unsafe bool bigFirst
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_bigFirst);
				return *(bool*)num;
			}
			set
			{
				*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_bigFirst)) = value;
			}
		}

		public unsafe GridShape inventoryShape
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_inventoryShape);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_inventoryShape)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass2_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass2_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr, "<>c__DisplayClass2_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass2_0>.NativeClassPtr);
			NativeFieldInfoPtr_bigFirst = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass2_0>.NativeClassPtr, "bigFirst");
			NativeFieldInfoPtr_inventoryShape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass2_0>.NativeClassPtr, "inventoryShape");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass2_0>.NativeClassPtr, 100667303);
			NativeMethodInfoPtr__Sort_b__0_Internal_Int32_GameItem_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass2_0>.NativeClassPtr, 100667304);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass2_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass2_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 184026, XrefRangeEnd = 184043, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe int _Sort_b__0(GameItem a, GameItem b)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)a);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)b);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__Sort_b__0_Internal_Int32_GameItem_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass2_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeMethodInfoPtr_MaySort_Public_Static_Boolean_GameGridInventory_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CanSort_Public_Static_Boolean_GameGridInventory_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Sort_Public_Static_Boolean_GameGridInventory_Boolean_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FindSpot_Private_Static_GridShape_GameGridInventory_GameItem_List_1_GameItem_Il2CppObjectBase_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FitsOccupancy_Private_Static_Boolean_GridShape_Il2CppObjectBase_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MarkOccupied_Private_Static_Void_Il2CppObjectBase_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CellCount_Private_Static_Int32_GridShape_0;

	static InventorySortHelper()
	{
		Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "InventorySortHelper");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr);
		NativeMethodInfoPtr_MaySort_Public_Static_Boolean_GameGridInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr, 100667296);
		NativeMethodInfoPtr_CanSort_Public_Static_Boolean_GameGridInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr, 100667297);
		NativeMethodInfoPtr_Sort_Public_Static_Boolean_GameGridInventory_Boolean_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr, 100667298);
		NativeMethodInfoPtr_FindSpot_Private_Static_GridShape_GameGridInventory_GameItem_List_1_GameItem_Il2CppObjectBase_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr, 100667299);
		NativeMethodInfoPtr_FitsOccupancy_Private_Static_Boolean_GridShape_Il2CppObjectBase_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr, 100667300);
		NativeMethodInfoPtr_MarkOccupied_Private_Static_Void_Il2CppObjectBase_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr, 100667301);
		NativeMethodInfoPtr_CellCount_Private_Static_Int32_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<InventorySortHelper>.NativeClassPtr, 100667302);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 184049, RefRangeEnd = 184050, XrefRangeStart = 184043, XrefRangeEnd = 184049, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static bool MaySort(GameGridInventory inventory)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)inventory);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MaySort_Public_Static_Boolean_GameGridInventory_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 184080, RefRangeEnd = 184082, XrefRangeStart = 184050, XrefRangeEnd = 184080, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static bool CanSort(GameGridInventory inventory)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)inventory);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CanSort_Public_Static_Boolean_GameGridInventory_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 184193, RefRangeEnd = 184194, XrefRangeStart = 184082, XrefRangeEnd = 184193, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static bool Sort(GameGridInventory inventory, bool bigFirst, bool fromEnd)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)inventory);
		*(bool**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bigFirst;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &fromEnd;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Sort_Public_Static_Boolean_GameGridInventory_Boolean_Boolean_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 184194, XrefRangeEnd = 184210, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static GridShape FindSpot(GameGridInventory inventory, GameItem item, List<GameItem> movingItems, Il2CppObjectBase itemLayers, bool fromEnd)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[5];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)inventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)movingItems);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr(itemLayers);
		*(bool**)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = &fromEnd;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FindSpot_Private_Static_GridShape_GameGridInventory_GameItem_List_1_GameItem_Il2CppObjectBase_Boolean_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr2) : null;
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 184223, RefRangeEnd = 184225, XrefRangeStart = 184210, XrefRangeEnd = 184223, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static bool FitsOccupancy(GridShape shape, Il2CppObjectBase itemLayers)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)shape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr(itemLayers);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FitsOccupancy_Private_Static_Boolean_GridShape_Il2CppObjectBase_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 184239, RefRangeEnd = 184240, XrefRangeStart = 184225, XrefRangeEnd = 184239, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void MarkOccupied(Il2CppObjectBase itemLayers, GridShape shape)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr(itemLayers);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)shape);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MarkOccupied_Private_Static_Void_Il2CppObjectBase_GridShape_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 184249, RefRangeEnd = 184251, XrefRangeStart = 184240, XrefRangeEnd = 184249, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static int CellCount(GridShape shape)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)shape);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CellCount_Private_Static_Int32_GridShape_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	public InventorySortHelper(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
