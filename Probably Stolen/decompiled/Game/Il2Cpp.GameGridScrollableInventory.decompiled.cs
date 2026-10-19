using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Il2Cpp;

public class GameGridScrollableInventory : GameInventory
{
	[System.Serializable]
	[ObfuscatedName("GameGridScrollableInventory+<>c")]
	public new sealed class __c : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___9;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__74_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__87_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__88_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__89_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__96_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__98_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__98_2;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__98_3;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__GetMinVisibleHeight_b__74_0_Internal_Int32_GameItem_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckShapePlacement_b__87_0_Internal_Boolean_Byte_Byte_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckInventorySlotStillValid_b__88_0_Internal_Boolean_Byte_Byte_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__TryInventorySlot_b__89_0_Internal_Boolean_Byte_Byte_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__TryFindOneValidInventorySlot_b__96_0_Internal_Int32_GameItem_GameItem_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AddFastUnchecked_b__98_0_Internal_Void_GameItem_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AddFastUnchecked_b__98_2_Internal_Int32_GameItem_GameItem_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AddFastUnchecked_b__98_3_Internal_Boolean_Byte_Byte_0;

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

		public unsafe static Il2CppSystem.Func<GameItem, int> __9__74_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__74_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<GameItem, int>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__74_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__87_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__87_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__87_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__88_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__88_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__88_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__89_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__89_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__89_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Comparison<GameItem> __9__96_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__96_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Comparison<GameItem>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__96_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<GameItem> __9__98_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__98_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<GameItem>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__98_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Comparison<GameItem> __9__98_2
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__98_2, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Comparison<GameItem>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__98_2, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__98_3
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__98_3, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__98_3, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c()
		{
			Il2CppClassPointerStore<__c>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<>c");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c>.NativeClassPtr);
			NativeFieldInfoPtr___9 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9");
			NativeFieldInfoPtr___9__74_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__74_0");
			NativeFieldInfoPtr___9__87_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__87_0");
			NativeFieldInfoPtr___9__88_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__88_0");
			NativeFieldInfoPtr___9__89_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__89_0");
			NativeFieldInfoPtr___9__96_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__96_0");
			NativeFieldInfoPtr___9__98_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__98_0");
			NativeFieldInfoPtr___9__98_2 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__98_2");
			NativeFieldInfoPtr___9__98_3 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__98_3");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674849);
			NativeMethodInfoPtr__GetMinVisibleHeight_b__74_0_Internal_Int32_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674850);
			NativeMethodInfoPtr__CheckShapePlacement_b__87_0_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674851);
			NativeMethodInfoPtr__CheckInventorySlotStillValid_b__88_0_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674852);
			NativeMethodInfoPtr__TryInventorySlot_b__89_0_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674853);
			NativeMethodInfoPtr__TryFindOneValidInventorySlot_b__96_0_Internal_Int32_GameItem_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674854);
			NativeMethodInfoPtr__AddFastUnchecked_b__98_0_Internal_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674855);
			NativeMethodInfoPtr__AddFastUnchecked_b__98_2_Internal_Int32_GameItem_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674856);
			NativeMethodInfoPtr__AddFastUnchecked_b__98_3_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674857);
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
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349493, XrefRangeEnd = 349498, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe int _GetMinVisibleHeight_b__74_0(GameItem item)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__GetMinVisibleHeight_b__74_0_Internal_Int32_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		public unsafe bool _CheckShapePlacement_b__87_0(byte b, byte bo)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bo;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckShapePlacement_b__87_0_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		public unsafe bool _CheckInventorySlotStillValid_b__88_0(byte b, byte bo)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bo;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckInventorySlotStillValid_b__88_0_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		public unsafe bool _TryInventorySlot_b__89_0(byte b, byte bo)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bo;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__TryInventorySlot_b__89_0_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349498, XrefRangeEnd = 349507, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe int _TryFindOneValidInventorySlot_b__96_0(GameItem x, GameItem y)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)x);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)y);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__TryFindOneValidInventorySlot_b__96_0_Internal_Int32_GameItem_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _AddFastUnchecked_b__98_0(GameItem item)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AddFastUnchecked_b__98_0_Internal_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349507, XrefRangeEnd = 349516, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe int _AddFastUnchecked_b__98_2(GameItem o1, GameItem o2)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)o1);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)o2);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AddFastUnchecked_b__98_2_Internal_Int32_GameItem_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		public unsafe bool _AddFastUnchecked_b__98_3(byte b, byte bo)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bo;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AddFastUnchecked_b__98_3_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameGridScrollableInventory+<>c__DisplayClass87_0")]
	public sealed class __c__DisplayClass87_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_item;

		private static readonly System.IntPtr NativeFieldInfoPtr_ignoredItems;

		private static readonly System.IntPtr NativeFieldInfoPtr_shape;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckShapePlacement_b__1_Internal_Boolean_GameItem_0;

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

		public unsafe List<GameItem> ignoredItems
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ignoredItems);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GameItem>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ignoredItems)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe GridShape shape
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_shape);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_shape)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass87_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass87_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<>c__DisplayClass87_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass87_0>.NativeClassPtr);
			NativeFieldInfoPtr_item = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass87_0>.NativeClassPtr, "item");
			NativeFieldInfoPtr_ignoredItems = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass87_0>.NativeClassPtr, "ignoredItems");
			NativeFieldInfoPtr_shape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass87_0>.NativeClassPtr, "shape");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass87_0>.NativeClassPtr, 100674858);
			NativeMethodInfoPtr__CheckShapePlacement_b__1_Internal_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass87_0>.NativeClassPtr, 100674859);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass87_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass87_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349516, XrefRangeEnd = 349522, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _CheckShapePlacement_b__1(GameItem o)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)o);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckShapePlacement_b__1_Internal_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass87_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameGridScrollableInventory+<>c__DisplayClass88_0")]
	public sealed class __c__DisplayClass88_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_item;

		private static readonly System.IntPtr NativeFieldInfoPtr_tempShape;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckInventorySlotStillValid_b__1_Internal_Boolean_GameItem_0;

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

		public unsafe GridShape tempShape
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tempShape);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tempShape)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass88_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass88_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<>c__DisplayClass88_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass88_0>.NativeClassPtr);
			NativeFieldInfoPtr_item = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass88_0>.NativeClassPtr, "item");
			NativeFieldInfoPtr_tempShape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass88_0>.NativeClassPtr, "tempShape");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass88_0>.NativeClassPtr, 100674860);
			NativeMethodInfoPtr__CheckInventorySlotStillValid_b__1_Internal_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass88_0>.NativeClassPtr, 100674861);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass88_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass88_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349522, XrefRangeEnd = 349523, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _CheckInventorySlotStillValid_b__1(GameItem o)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)o);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckInventorySlotStillValid_b__1_Internal_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass88_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameGridScrollableInventory+<>c__DisplayClass89_0")]
	public sealed class __c__DisplayClass89_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_item;

		private static readonly System.IntPtr NativeFieldInfoPtr_requestedNum;

		private static readonly System.IntPtr NativeFieldInfoPtr_tempShape;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__TryInventorySlot_b__1_Internal_Boolean_GameItem_0;

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

		public unsafe int requestedNum
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_requestedNum);
				return *(int*)num;
			}
			set
			{
				*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_requestedNum)) = value;
			}
		}

		public unsafe GridShape tempShape
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tempShape);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tempShape)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass89_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass89_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<>c__DisplayClass89_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass89_0>.NativeClassPtr);
			NativeFieldInfoPtr_item = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass89_0>.NativeClassPtr, "item");
			NativeFieldInfoPtr_requestedNum = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass89_0>.NativeClassPtr, "requestedNum");
			NativeFieldInfoPtr_tempShape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass89_0>.NativeClassPtr, "tempShape");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass89_0>.NativeClassPtr, 100674862);
			NativeMethodInfoPtr__TryInventorySlot_b__1_Internal_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass89_0>.NativeClassPtr, 100674863);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass89_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass89_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349523, XrefRangeEnd = 349524, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _TryInventorySlot_b__1(GameItem o)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)o);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__TryInventorySlot_b__1_Internal_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass89_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameGridScrollableInventory+<>c__DisplayClass98_0")]
	public sealed class __c__DisplayClass98_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_tempShape;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__4;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AddFastUnchecked_b__4_Internal_Boolean_GameItem_0;

		public unsafe GridShape tempShape
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tempShape);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tempShape)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe Il2CppSystem.Func<GameItem, bool> __9__4
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___9__4);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<GameItem, bool>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___9__4)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass98_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass98_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<>c__DisplayClass98_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass98_0>.NativeClassPtr);
			NativeFieldInfoPtr_tempShape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass98_0>.NativeClassPtr, "tempShape");
			NativeFieldInfoPtr___9__4 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass98_0>.NativeClassPtr, "<>9__4");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass98_0>.NativeClassPtr, 100674864);
			NativeMethodInfoPtr__AddFastUnchecked_b__4_Internal_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass98_0>.NativeClassPtr, 100674865);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass98_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass98_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349524, XrefRangeEnd = 349527, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _AddFastUnchecked_b__4(GameItem o)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)o);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AddFastUnchecked_b__4_Internal_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass98_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeFieldInfoPtr__widthPixels_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__heightPixels_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__handler_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__inner_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_background;

	private static readonly System.IntPtr NativeFieldInfoPtr_insideBorder;

	private static readonly System.IntPtr NativeFieldInfoPtr_highlightNode;

	private static readonly System.IntPtr NativeFieldInfoPtr_innerMask;

	private static readonly System.IntPtr NativeFieldInfoPtr_items;

	private static readonly System.IntPtr NativeFieldInfoPtr_highlightSquares;

	private static readonly System.IntPtr NativeFieldInfoPtr__width_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__height_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_leftPadding;

	private static readonly System.IntPtr NativeFieldInfoPtr_rightPadding;

	private static readonly System.IntPtr NativeFieldInfoPtr_topPadding;

	private static readonly System.IntPtr NativeFieldInfoPtr_scrollPadding;

	private static readonly System.IntPtr NativeFieldInfoPtr_minButtonHeight;

	private static readonly System.IntPtr NativeFieldInfoPtr_minButtonWidth;

	private static readonly System.IntPtr NativeFieldInfoPtr_scrollBar;

	private static readonly System.IntPtr NativeFieldInfoPtr_scrollButton;

	private static readonly System.IntPtr NativeFieldInfoPtr__currentScroll_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_scrollOverflow;

	private static readonly System.IntPtr NativeFieldInfoPtr_savedBarRelativePosition;

	private static readonly System.IntPtr NativeFieldInfoPtr_savedBackgroundRelativePosition;

	private static readonly System.IntPtr NativeFieldInfoPtr_alwaysStack;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_children_Public_Virtual_get_List_1_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_childItems_Public_Virtual_get_List_1_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_widthPixels_Private_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_heightPixels_Private_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_width_Public_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_width_Private_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_height_Public_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_height_Private_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_currentScroll_Private_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_currentScroll_Private_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Int32_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Scroll_Public_Void_Vector2_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetLocalPosition_Private_Vector2_Vector2_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetGridPosition_Private_GridShape_Vector2_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Validate_Public_Virtual_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Validate_Public_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ValidateScrollBar_Private_Void_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_PlaceButtonAtMousePositionCenter_Private_Void_Vector2_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_PlaceButtonAtMousePositionRelative_Private_Void_Vector2_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetButtonScroll_Private_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetScroll_Public_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetScroll_Public_Void_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetVisibleHeight_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetMinVisibleHeight_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddParent_Public_Virtual_Boolean_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveParent_Public_Virtual_Boolean_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsAdjacent_Public_Virtual_Boolean_SlotMarker_SlotMarker_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InitHighlightSquare_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetHighlightSquare_Private_TreeNodeRender_Int32_Int32_BackgroundColor_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_HighlightSlot_Public_Virtual_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ClearHighlight_Public_Virtual_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_List_1_TreeNodeRender_BackgroundColor_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_SupportsGroupPlacement_Public_Virtual_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckShapePlacement_Public_Virtual_Boolean_GameItem_GridShape_List_1_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Virtual_Boolean_GameItem_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryInventorySlot_Public_Virtual_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Virtual_SlotMarker_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_UncheckedAccept_Public_Virtual_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Expel_Public_Virtual_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_SlotMarker_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_SlotMarker_GameItem_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FastStackUnchecked_Public_GameItem_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddFastUnchecked_Public_Void_List_1_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__54_0_Private_Boolean_PointerEventData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__54_1_Private_Boolean_PointerEventData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__54_2_Private_Boolean_PointerEventData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__54_3_Private_Boolean_PointerEventData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__54_4_Private_Boolean_PointerEventData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__54_5_Private_Boolean_PointerEventData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__AddFastUnchecked_b__98_1_Private_Boolean_GameItem_0;

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
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__handler_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender _inner_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__inner_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__inner_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender background
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_background);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_background)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender insideBorder
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_insideBorder);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_insideBorder)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender highlightNode
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlightNode);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlightNode)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender innerMask
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_innerMask);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_innerMask)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<GameItem> items
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_items);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GameItem>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_items)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<TreeNodeRender> highlightSquares
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlightSquares);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_highlightSquares)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int _width_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__width_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__width_k__BackingField)) = value;
		}
	}

	public unsafe int _height_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__height_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__height_k__BackingField)) = value;
		}
	}

	public unsafe int leftPadding
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_leftPadding);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_leftPadding)) = value;
		}
	}

	public unsafe int rightPadding
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rightPadding);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_rightPadding)) = value;
		}
	}

	public unsafe int topPadding
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_topPadding);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_topPadding)) = value;
		}
	}

	public unsafe int scrollPadding
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollPadding);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollPadding)) = value;
		}
	}

	public unsafe int minButtonHeight
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_minButtonHeight);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_minButtonHeight)) = value;
		}
	}

	public unsafe int minButtonWidth
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_minButtonWidth);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_minButtonWidth)) = value;
		}
	}

	public unsafe TreeNodeRender scrollBar
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollBar);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollBar)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender scrollButton
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollButton);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollButton)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int _currentScroll_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__currentScroll_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__currentScroll_k__BackingField)) = value;
		}
	}

	public unsafe float scrollOverflow
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollOverflow);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_scrollOverflow)) = value;
		}
	}

	public unsafe float savedBarRelativePosition
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_savedBarRelativePosition);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_savedBarRelativePosition)) = value;
		}
	}

	public unsafe float savedBackgroundRelativePosition
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_savedBackgroundRelativePosition);
			return *(float*)num;
		}
		set
		{
			*(float*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_savedBackgroundRelativePosition)) = value;
		}
	}

	public unsafe bool alwaysStack
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_alwaysStack);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_alwaysStack)) = value;
		}
	}

	public unsafe override List<GraphNodeStorage> children
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349527, XrefRangeEnd = 349533, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_children_Public_Virtual_get_List_1_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GraphNodeStorage>>(intPtr2) : null;
		}
	}

	public unsafe override List<GameItem> childItems
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349533, XrefRangeEnd = 349539, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_childItems_Public_Virtual_get_List_1_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GameItem>>(intPtr2) : null;
		}
	}

	public unsafe virtual bool canResize
	{
		[CallerCount(76)]
		[CachedScanResults(RefRangeStart = 16920, RefRangeEnd = 16996, XrefRangeStart = 16920, XrefRangeEnd = 16996, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_widthPixels_Private_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual int heightPixels
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_heightPixels_Private_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual TreeNodeRender handler
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual RectTransform rectTransform
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<RectTransform>(intPtr2) : null;
		}
	}

	public unsafe virtual TreeNodeRender inner
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe int width
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_width_Public_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_width_Private_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe int height
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_height_Public_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_height_Private_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe int currentScroll
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_currentScroll_Private_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_currentScroll_Private_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe override bool SupportsGroupPlacement
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_SupportsGroupPlacement_Public_Virtual_get_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	static GameGridScrollableInventory()
	{
		Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "GameGridScrollableInventory");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr);
		NativeFieldInfoPtr__widthPixels_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<widthPixels>k__BackingField");
		NativeFieldInfoPtr__heightPixels_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<heightPixels>k__BackingField");
		NativeFieldInfoPtr__handler_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<handler>k__BackingField");
		NativeFieldInfoPtr__inner_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<inner>k__BackingField");
		NativeFieldInfoPtr_background = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "background");
		NativeFieldInfoPtr_insideBorder = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "insideBorder");
		NativeFieldInfoPtr_highlightNode = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "highlightNode");
		NativeFieldInfoPtr_innerMask = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "innerMask");
		NativeFieldInfoPtr_items = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "items");
		NativeFieldInfoPtr_highlightSquares = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "highlightSquares");
		NativeFieldInfoPtr__width_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<width>k__BackingField");
		NativeFieldInfoPtr__height_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<height>k__BackingField");
		NativeFieldInfoPtr_leftPadding = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "leftPadding");
		NativeFieldInfoPtr_rightPadding = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "rightPadding");
		NativeFieldInfoPtr_topPadding = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "topPadding");
		NativeFieldInfoPtr_scrollPadding = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "scrollPadding");
		NativeFieldInfoPtr_minButtonHeight = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "minButtonHeight");
		NativeFieldInfoPtr_minButtonWidth = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "minButtonWidth");
		NativeFieldInfoPtr_scrollBar = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "scrollBar");
		NativeFieldInfoPtr_scrollButton = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "scrollButton");
		NativeFieldInfoPtr__currentScroll_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "<currentScroll>k__BackingField");
		NativeFieldInfoPtr_scrollOverflow = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "scrollOverflow");
		NativeFieldInfoPtr_savedBarRelativePosition = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "savedBarRelativePosition");
		NativeFieldInfoPtr_savedBackgroundRelativePosition = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "savedBackgroundRelativePosition");
		NativeFieldInfoPtr_alwaysStack = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, "alwaysStack");
		NativeMethodInfoPtr_get_children_Public_Virtual_get_List_1_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674787);
		NativeMethodInfoPtr_get_childItems_Public_Virtual_get_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674788);
		NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674789);
		NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674790);
		NativeMethodInfoPtr_set_widthPixels_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674791);
		NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674792);
		NativeMethodInfoPtr_set_heightPixels_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674793);
		NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674794);
		NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674795);
		NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674796);
		NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674797);
		NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674798);
		NativeMethodInfoPtr_get_width_Public_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674799);
		NativeMethodInfoPtr_set_width_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674800);
		NativeMethodInfoPtr_get_height_Public_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674801);
		NativeMethodInfoPtr_set_height_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674802);
		NativeMethodInfoPtr_get_currentScroll_Private_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674803);
		NativeMethodInfoPtr_set_currentScroll_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674804);
		NativeMethodInfoPtr__ctor_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674805);
		NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Int32_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674806);
		NativeMethodInfoPtr_Scroll_Public_Void_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674807);
		NativeMethodInfoPtr_GetLocalPosition_Private_Vector2_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674808);
		NativeMethodInfoPtr_GetGridPosition_Private_GridShape_Vector2_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674809);
		NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674810);
		NativeMethodInfoPtr_Validate_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674811);
		NativeMethodInfoPtr_Validate_Public_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674812);
		NativeMethodInfoPtr_ValidateScrollBar_Private_Void_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674813);
		NativeMethodInfoPtr_PlaceButtonAtMousePositionCenter_Private_Void_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674814);
		NativeMethodInfoPtr_PlaceButtonAtMousePositionRelative_Private_Void_Vector2_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674815);
		NativeMethodInfoPtr_GetButtonScroll_Private_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674816);
		NativeMethodInfoPtr_GetScroll_Public_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674817);
		NativeMethodInfoPtr_SetScroll_Public_Void_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674818);
		NativeMethodInfoPtr_GetVisibleHeight_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674819);
		NativeMethodInfoPtr_GetMinVisibleHeight_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674820);
		NativeMethodInfoPtr_AddParent_Public_Virtual_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674821);
		NativeMethodInfoPtr_RemoveParent_Public_Virtual_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674822);
		NativeMethodInfoPtr_IsAdjacent_Public_Virtual_Boolean_SlotMarker_SlotMarker_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674823);
		NativeMethodInfoPtr_InitHighlightSquare_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674824);
		NativeMethodInfoPtr_SetHighlightSquare_Private_TreeNodeRender_Int32_Int32_BackgroundColor_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674825);
		NativeMethodInfoPtr_HighlightSlot_Public_Virtual_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674826);
		NativeMethodInfoPtr_ClearHighlight_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674827);
		NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_List_1_TreeNodeRender_BackgroundColor_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674828);
		NativeMethodInfoPtr_get_SupportsGroupPlacement_Public_Virtual_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674829);
		NativeMethodInfoPtr_CheckShapePlacement_Public_Virtual_Boolean_GameItem_GridShape_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674830);
		NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Virtual_Boolean_GameItem_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674831);
		NativeMethodInfoPtr_TryInventorySlot_Public_Virtual_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674832);
		NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Virtual_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674833);
		NativeMethodInfoPtr_UncheckedAccept_Public_Virtual_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674834);
		NativeMethodInfoPtr_Expel_Public_Virtual_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674835);
		NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674836);
		NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674837);
		NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_SlotMarker_GameItem_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674838);
		NativeMethodInfoPtr_FastStackUnchecked_Public_GameItem_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674839);
		NativeMethodInfoPtr_AddFastUnchecked_Public_Void_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674840);
		NativeMethodInfoPtr___ctor_b__54_0_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674841);
		NativeMethodInfoPtr___ctor_b__54_1_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674842);
		NativeMethodInfoPtr___ctor_b__54_2_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674843);
		NativeMethodInfoPtr___ctor_b__54_3_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674844);
		NativeMethodInfoPtr___ctor_b__54_4_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674845);
		NativeMethodInfoPtr___ctor_b__54_5_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674846);
		NativeMethodInfoPtr__AddFastUnchecked_b__98_1_Private_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr, 100674847);
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 349651, RefRangeEnd = 349656, XrefRangeStart = 349539, XrefRangeEnd = 349651, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameGridScrollableInventory()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr))
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(12)]
	[CachedScanResults(RefRangeStart = 349661, RefRangeEnd = 349673, XrefRangeStart = 349656, XrefRangeEnd = 349661, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameGridScrollableInventory(int width, int displayHeight, int height = 400, float scrollOverflow = 0.5f)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameGridScrollableInventory>.NativeClassPtr))
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = (nint)(&width);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &displayHeight;
		*(int**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &height;
		*(float**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &scrollOverflow;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Int32_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 349678, RefRangeEnd = 349679, XrefRangeStart = 349673, XrefRangeEnd = 349678, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Scroll(Vector2 scrollDelta)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&scrollDelta);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Scroll_Public_Void_Vector2_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Vector2 GetLocalPosition(Vector2 position)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&position);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetLocalPosition_Private_Vector2_Vector2_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(Vector2*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 349703, RefRangeEnd = 349704, XrefRangeStart = 349679, XrefRangeEnd = 349703, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GridShape GetGridPosition(Vector2 position, GridShape shape)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&position);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)shape);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetGridPosition_Private_GridShape_Vector2_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349704, XrefRangeEnd = 349705, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual void ResizePixels(int width, int height)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&width);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &height;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349705, XrefRangeEnd = 349706, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void Validate()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Validate_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(13)]
	[CachedScanResults(RefRangeStart = 349741, RefRangeEnd = 349754, XrefRangeStart = 349706, XrefRangeEnd = 349741, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Validate(bool validateItems = true)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&validateItems);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Validate_Public_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 349768, RefRangeEnd = 349769, XrefRangeStart = 349754, XrefRangeEnd = 349768, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ValidateScrollBar(int buttonHeight, int barInnerHeight)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&buttonHeight);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &barInnerHeight;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ValidateScrollBar_Private_Void_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 349779, RefRangeEnd = 349781, XrefRangeStart = 349769, XrefRangeEnd = 349779, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void PlaceButtonAtMousePositionCenter(Vector2 mousePosition)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&mousePosition);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_PlaceButtonAtMousePositionCenter_Private_Void_Vector2_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349781, XrefRangeEnd = 349790, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void PlaceButtonAtMousePositionRelative(Vector2 mousePosition, float relativePosition)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&mousePosition);
		*(float**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &relativePosition;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_PlaceButtonAtMousePositionRelative_Private_Void_Vector2_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 349792, RefRangeEnd = 349795, XrefRangeStart = 349790, XrefRangeEnd = 349792, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe float GetButtonScroll()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetButtonScroll_Private_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(float*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349795, XrefRangeEnd = 349798, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe float GetScroll()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetScroll_Public_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(float*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 349809, RefRangeEnd = 349812, XrefRangeStart = 349798, XrefRangeEnd = 349809, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SetScroll(float percentage)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&percentage);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetScroll_Public_Void_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349812, XrefRangeEnd = 349814, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int GetVisibleHeight()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetVisibleHeight_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 349844, RefRangeEnd = 349849, XrefRangeStart = 349814, XrefRangeEnd = 349844, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int GetMinVisibleHeight()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetMinVisibleHeight_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool AddParent(GraphNodeStorage node)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)node);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_AddParent_Public_Virtual_Boolean_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool RemoveParent(GraphNodeStorage node)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)node);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_RemoveParent_Public_Virtual_Boolean_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349849, XrefRangeEnd = 349867, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool IsAdjacent(SlotMarker slot, SlotMarker slot2)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot2);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_IsAdjacent_Public_Virtual_Boolean_SlotMarker_SlotMarker_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349867, XrefRangeEnd = 349879, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void InitHighlightSquare()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InitHighlightSquare_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 349902, RefRangeEnd = 349903, XrefRangeStart = 349879, XrefRangeEnd = 349902, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe TreeNodeRender SetHighlightSquare(int x, int y, RenderHandler.BackgroundColor color, bool doReplace)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = (nint)(&x);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &y;
		*(RenderHandler.BackgroundColor**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &color;
		*(bool**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &doReplace;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetHighlightSquare_Private_TreeNodeRender_Int32_Int32_BackgroundColor_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349903, XrefRangeEnd = 349924, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override List<TreeNodeRender> HighlightSlot(SlotMarker slot, RenderHandler.BackgroundColor color, bool doReplace)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot);
		*(RenderHandler.BackgroundColor**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &color;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &doReplace;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_HighlightSlot_Public_Virtual_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349924, XrefRangeEnd = 349939, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void ClearHighlight()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_ClearHighlight_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349939, XrefRangeEnd = 349958, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override List<TreeNodeRender> HighlightWholeInventory(RenderHandler.BackgroundColor color)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&color);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_List_1_TreeNodeRender_BackgroundColor_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349958, XrefRangeEnd = 350000, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool CheckShapePlacement(GameItem item, GridShape shape, List<GameItem> ignoredItems)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)shape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)ignoredItems);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_CheckShapePlacement_Public_Virtual_Boolean_GameItem_GridShape_List_1_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350000, XrefRangeEnd = 350045, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool CheckInventorySlotStillValid(GameItem item, GridShape tempShape, TagSystem tempState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Virtual_Boolean_GameItem_GridShape_TagSystem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350045, XrefRangeEnd = 350105, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override SlotMarker TryInventorySlot(GameItem item, int requestedNum, Vector2 itemGraphicsCenterPosition, GridShape tempShape, TagSystem tempState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[5];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &requestedNum;
		*(Vector2**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &itemGraphicsCenterPosition;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_TryInventorySlot_Public_Virtual_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350105, XrefRangeEnd = 350115, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override SlotMarker GetIncompleteItemInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Virtual_SlotMarker_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350115, XrefRangeEnd = 350126, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool UncheckedAccept(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_UncheckedAccept_Public_Virtual_Boolean_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350126, XrefRangeEnd = 350148, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool Expel(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Expel_Public_Virtual_Boolean_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350148, XrefRangeEnd = 350178, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override bool MayHaveValidInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Boolean_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350178, XrefRangeEnd = 350179, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_SlotMarker_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 350240, RefRangeEnd = 350241, XrefRangeStart = 350179, XrefRangeEnd = 350240, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe SlotMarker TryFindOneValidInventorySlot(GameItem item, bool keepOrientation = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(bool**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &keepOrientation;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_SlotMarker_GameItem_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 350253, RefRangeEnd = 350255, XrefRangeStart = 350241, XrefRangeEnd = 350253, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem FastStackUnchecked(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FastStackUnchecked_Public_GameItem_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(8)]
	[CachedScanResults(RefRangeStart = 350393, RefRangeEnd = 350401, XrefRangeStart = 350255, XrefRangeEnd = 350393, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AddFastUnchecked(List<GameItem> allItems)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)allItems);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AddFastUnchecked_Public_Void_List_1_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350401, XrefRangeEnd = 350403, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__54_0(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__54_0_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350403, XrefRangeEnd = 350408, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__54_1(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__54_1_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350408, XrefRangeEnd = 350410, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__54_2(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__54_2_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350410, XrefRangeEnd = 350420, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__54_3(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__54_3_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350420, XrefRangeEnd = 350423, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__54_4(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__54_4_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__54_5(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__54_5_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350423, XrefRangeEnd = 350445, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool _AddFastUnchecked_b__98_1(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AddFastUnchecked_b__98_1_Private_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	public GameGridScrollableInventory(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
