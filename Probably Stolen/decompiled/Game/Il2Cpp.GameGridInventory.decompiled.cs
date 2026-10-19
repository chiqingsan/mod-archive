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

public class GameGridInventory : GameInventory
{
	[System.Serializable]
	[ObfuscatedName("GameGridInventory+<>c")]
	public new sealed class __c : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___9;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__62_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__62_1;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__62_2;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__62_3;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__62_4;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__76_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__77_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__78_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__85_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__86_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ValidateBackground_b__62_0_Internal_Void_TreeNodeRender_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ValidateBackground_b__62_1_Internal_Void_TreeNodeRender_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ValidateBackground_b__62_2_Internal_Void_TreeNodeRender_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ValidateBackground_b__62_3_Internal_Void_TreeNodeRender_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ValidateBackground_b__62_4_Internal_Void_TreeNodeRender_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckShapePlacement_b__76_0_Internal_Boolean_Byte_Byte_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckInventorySlotStillValid_b__77_0_Internal_Boolean_Byte_Byte_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__TryInventorySlot_b__78_0_Internal_Boolean_Byte_Byte_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__TryFindOneValidInventorySlot_b__85_0_Internal_Int32_GameItem_GameItem_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__TryFindOneValidInventorySlot_TopMiddle_b__86_0_Internal_Int32_GameItem_GameItem_0;

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

		public unsafe static Il2CppSystem.Action<TreeNodeRender> __9__62_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__62_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<TreeNodeRender>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__62_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<TreeNodeRender> __9__62_1
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__62_1, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<TreeNodeRender>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__62_1, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<TreeNodeRender> __9__62_2
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__62_2, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<TreeNodeRender>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__62_2, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<TreeNodeRender> __9__62_3
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__62_3, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<TreeNodeRender>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__62_3, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<TreeNodeRender> __9__62_4
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__62_4, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<TreeNodeRender>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__62_4, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__76_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__76_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__76_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__77_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__77_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__77_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__78_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__78_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__78_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Comparison<GameItem> __9__85_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__85_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Comparison<GameItem>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__85_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Comparison<GameItem> __9__86_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__86_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Comparison<GameItem>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__86_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c()
		{
			Il2CppClassPointerStore<__c>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<>c");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c>.NativeClassPtr);
			NativeFieldInfoPtr___9 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9");
			NativeFieldInfoPtr___9__62_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__62_0");
			NativeFieldInfoPtr___9__62_1 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__62_1");
			NativeFieldInfoPtr___9__62_2 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__62_2");
			NativeFieldInfoPtr___9__62_3 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__62_3");
			NativeFieldInfoPtr___9__62_4 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__62_4");
			NativeFieldInfoPtr___9__76_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__76_0");
			NativeFieldInfoPtr___9__77_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__77_0");
			NativeFieldInfoPtr___9__78_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__78_0");
			NativeFieldInfoPtr___9__85_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__85_0");
			NativeFieldInfoPtr___9__86_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__86_0");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674770);
			NativeMethodInfoPtr__ValidateBackground_b__62_0_Internal_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674771);
			NativeMethodInfoPtr__ValidateBackground_b__62_1_Internal_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674772);
			NativeMethodInfoPtr__ValidateBackground_b__62_2_Internal_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674773);
			NativeMethodInfoPtr__ValidateBackground_b__62_3_Internal_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674774);
			NativeMethodInfoPtr__ValidateBackground_b__62_4_Internal_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674775);
			NativeMethodInfoPtr__CheckShapePlacement_b__76_0_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674776);
			NativeMethodInfoPtr__CheckInventorySlotStillValid_b__77_0_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674777);
			NativeMethodInfoPtr__TryInventorySlot_b__78_0_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674778);
			NativeMethodInfoPtr__TryFindOneValidInventorySlot_b__85_0_Internal_Int32_GameItem_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674779);
			NativeMethodInfoPtr__TryFindOneValidInventorySlot_TopMiddle_b__86_0_Internal_Int32_GameItem_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674780);
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
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348520, XrefRangeEnd = 348522, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _ValidateBackground_b__62_0(TreeNodeRender n)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)n);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ValidateBackground_b__62_0_Internal_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _ValidateBackground_b__62_1(TreeNodeRender n)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)n);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ValidateBackground_b__62_1_Internal_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _ValidateBackground_b__62_2(TreeNodeRender n)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)n);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ValidateBackground_b__62_2_Internal_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _ValidateBackground_b__62_3(TreeNodeRender n)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)n);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ValidateBackground_b__62_3_Internal_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _ValidateBackground_b__62_4(TreeNodeRender n)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)n);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ValidateBackground_b__62_4_Internal_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		public unsafe bool _CheckShapePlacement_b__76_0(byte b, byte bo)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bo;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckShapePlacement_b__76_0_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		public unsafe bool _CheckInventorySlotStillValid_b__77_0(byte b, byte bo)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bo;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckInventorySlotStillValid_b__77_0_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		public unsafe bool _TryInventorySlot_b__78_0(byte b, byte bo)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bo;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__TryInventorySlot_b__78_0_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348522, XrefRangeEnd = 348531, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe int _TryFindOneValidInventorySlot_b__85_0(GameItem x, GameItem y)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)x);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)y);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__TryFindOneValidInventorySlot_b__85_0_Internal_Int32_GameItem_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348531, XrefRangeEnd = 348540, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe int _TryFindOneValidInventorySlot_TopMiddle_b__86_0(GameItem x, GameItem y)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)x);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)y);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__TryFindOneValidInventorySlot_TopMiddle_b__86_0_Internal_Int32_GameItem_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameGridInventory+<>c__DisplayClass76_0")]
	public sealed class __c__DisplayClass76_0 : Il2CppSystem.Object
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

		static __c__DisplayClass76_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass76_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<>c__DisplayClass76_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass76_0>.NativeClassPtr);
			NativeFieldInfoPtr_item = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass76_0>.NativeClassPtr, "item");
			NativeFieldInfoPtr_ignoredItems = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass76_0>.NativeClassPtr, "ignoredItems");
			NativeFieldInfoPtr_shape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass76_0>.NativeClassPtr, "shape");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass76_0>.NativeClassPtr, 100674781);
			NativeMethodInfoPtr__CheckShapePlacement_b__1_Internal_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass76_0>.NativeClassPtr, 100674782);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass76_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass76_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348540, XrefRangeEnd = 348546, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

		public __c__DisplayClass76_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameGridInventory+<>c__DisplayClass77_0")]
	public sealed class __c__DisplayClass77_0 : Il2CppSystem.Object
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

		static __c__DisplayClass77_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass77_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<>c__DisplayClass77_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass77_0>.NativeClassPtr);
			NativeFieldInfoPtr_item = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass77_0>.NativeClassPtr, "item");
			NativeFieldInfoPtr_tempShape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass77_0>.NativeClassPtr, "tempShape");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass77_0>.NativeClassPtr, 100674783);
			NativeMethodInfoPtr__CheckInventorySlotStillValid_b__1_Internal_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass77_0>.NativeClassPtr, 100674784);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass77_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass77_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348546, XrefRangeEnd = 348547, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

		public __c__DisplayClass77_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameGridInventory+<>c__DisplayClass78_0")]
	public sealed class __c__DisplayClass78_0 : Il2CppSystem.Object
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

		static __c__DisplayClass78_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass78_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<>c__DisplayClass78_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass78_0>.NativeClassPtr);
			NativeFieldInfoPtr_item = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass78_0>.NativeClassPtr, "item");
			NativeFieldInfoPtr_requestedNum = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass78_0>.NativeClassPtr, "requestedNum");
			NativeFieldInfoPtr_tempShape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass78_0>.NativeClassPtr, "tempShape");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass78_0>.NativeClassPtr, 100674785);
			NativeMethodInfoPtr__TryInventorySlot_b__1_Internal_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass78_0>.NativeClassPtr, 100674786);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass78_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass78_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348547, XrefRangeEnd = 348548, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

		public __c__DisplayClass78_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeFieldInfoPtr__widthPixels_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__heightPixels_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__handler_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__inner_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_dynamicBackgroundBase;

	private static readonly System.IntPtr NativeFieldInfoPtr_background;

	private static readonly System.IntPtr NativeFieldInfoPtr_insideBorder;

	private static readonly System.IntPtr NativeFieldInfoPtr_renderDynamicBackground;

	private static readonly System.IntPtr NativeFieldInfoPtr_dynamicBackgrounds;

	private static readonly System.IntPtr NativeFieldInfoPtr_dynamicR0;

	private static readonly System.IntPtr NativeFieldInfoPtr_dynamicR1;

	private static readonly System.IntPtr NativeFieldInfoPtr_dynamicR2;

	private static readonly System.IntPtr NativeFieldInfoPtr_dynamicR3;

	private static readonly System.IntPtr NativeFieldInfoPtr_items;

	private static readonly System.IntPtr NativeFieldInfoPtr__transparency_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_highlightSquares;

	private static readonly System.IntPtr NativeFieldInfoPtr__inventoryShape_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__lastShapeWidth;

	private static readonly System.IntPtr NativeFieldInfoPtr__lastShapeHeight;

	private static readonly System.IntPtr NativeFieldInfoPtr__lastShapeHash;

	private static readonly System.IntPtr NativeFieldInfoPtr__lastRenderDynamicBackground;

	private static readonly System.IntPtr NativeFieldInfoPtr_hideBorder;

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

	private static readonly System.IntPtr NativeMethodInfoPtr_get_transparency_Public_get_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_transparency_Private_set_Void_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_inventoryShape_Public_get_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_inventoryShape_Private_set_Void_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_String_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetShape_Public_GameGridInventory_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetShape_Public_GameGridInventory_String_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetLocalPosition_Private_Vector2_Vector2_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetGridPosition_Private_GridShape_Vector2_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Validate_Public_Virtual_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetTransparency_Public_GameGridInventory_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ValidateBackground_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ForceRecomputeDynamicBackground_Public_Void_0;

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

	private static readonly System.IntPtr NativeMethodInfoPtr_TryFindOneValidInventorySlot_TopMiddle_Public_SlotMarker_GameItem_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ComputeShapeHash_Private_Int32_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_HideBorder_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__48_0_Private_Boolean_PointerEventData_0;

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

	public unsafe TreeNodeRender dynamicBackgroundBase
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicBackgroundBase);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicBackgroundBase)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
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

	public unsafe bool renderDynamicBackground
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_renderDynamicBackground);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_renderDynamicBackground)) = value;
		}
	}

	public unsafe List<TreeNodeRender> dynamicBackgrounds
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicBackgrounds);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicBackgrounds)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<TreeNodeRender> dynamicR0
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicR0);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicR0)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<TreeNodeRender> dynamicR1
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicR1);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicR1)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<TreeNodeRender> dynamicR2
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicR2);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicR2)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<TreeNodeRender> dynamicR3
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicR3);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_dynamicR3)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
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

	public unsafe GridShape _inventoryShape_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__inventoryShape_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__inventoryShape_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int _lastShapeWidth
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lastShapeWidth);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lastShapeWidth)) = value;
		}
	}

	public unsafe int _lastShapeHeight
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lastShapeHeight);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lastShapeHeight)) = value;
		}
	}

	public unsafe int _lastShapeHash
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lastShapeHash);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lastShapeHash)) = value;
		}
	}

	public unsafe bool _lastRenderDynamicBackground
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lastRenderDynamicBackground);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lastRenderDynamicBackground)) = value;
		}
	}

	public unsafe bool hideBorder
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_hideBorder);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_hideBorder)) = value;
		}
	}

	public unsafe override List<GraphNodeStorage> children
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348548, XrefRangeEnd = 348554, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348554, XrefRangeEnd = 348560, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	public unsafe float transparency
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_transparency_Public_get_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(float*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_transparency_Private_set_Void_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe GridShape inventoryShape
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_inventoryShape_Public_get_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348560, XrefRangeEnd = 348561, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_inventoryShape_Private_set_Void_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe override bool SupportsGroupPlacement
	{
		[CallerCount(9)]
		[CachedScanResults(RefRangeStart = 18019, RefRangeEnd = 18028, XrefRangeStart = 18019, XrefRangeEnd = 18028, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	static GameGridInventory()
	{
		Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "GameGridInventory");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr);
		NativeFieldInfoPtr__widthPixels_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<widthPixels>k__BackingField");
		NativeFieldInfoPtr__heightPixels_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<heightPixels>k__BackingField");
		NativeFieldInfoPtr__handler_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<handler>k__BackingField");
		NativeFieldInfoPtr__inner_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<inner>k__BackingField");
		NativeFieldInfoPtr_dynamicBackgroundBase = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "dynamicBackgroundBase");
		NativeFieldInfoPtr_background = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "background");
		NativeFieldInfoPtr_insideBorder = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "insideBorder");
		NativeFieldInfoPtr_renderDynamicBackground = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "renderDynamicBackground");
		NativeFieldInfoPtr_dynamicBackgrounds = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "dynamicBackgrounds");
		NativeFieldInfoPtr_dynamicR0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "dynamicR0");
		NativeFieldInfoPtr_dynamicR1 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "dynamicR1");
		NativeFieldInfoPtr_dynamicR2 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "dynamicR2");
		NativeFieldInfoPtr_dynamicR3 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "dynamicR3");
		NativeFieldInfoPtr_items = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "items");
		NativeFieldInfoPtr__transparency_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<transparency>k__BackingField");
		NativeFieldInfoPtr_highlightSquares = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "highlightSquares");
		NativeFieldInfoPtr__inventoryShape_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "<inventoryShape>k__BackingField");
		NativeFieldInfoPtr__lastShapeWidth = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "_lastShapeWidth");
		NativeFieldInfoPtr__lastShapeHeight = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "_lastShapeHeight");
		NativeFieldInfoPtr__lastShapeHash = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "_lastShapeHash");
		NativeFieldInfoPtr__lastRenderDynamicBackground = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "_lastRenderDynamicBackground");
		NativeFieldInfoPtr_hideBorder = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, "hideBorder");
		NativeMethodInfoPtr_get_children_Public_Virtual_get_List_1_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674719);
		NativeMethodInfoPtr_get_childItems_Public_Virtual_get_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674720);
		NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674721);
		NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674722);
		NativeMethodInfoPtr_set_widthPixels_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674723);
		NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674724);
		NativeMethodInfoPtr_set_heightPixels_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674725);
		NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674726);
		NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674727);
		NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674728);
		NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674729);
		NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674730);
		NativeMethodInfoPtr_get_transparency_Public_get_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674731);
		NativeMethodInfoPtr_set_transparency_Private_set_Void_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674732);
		NativeMethodInfoPtr_get_inventoryShape_Public_get_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674733);
		NativeMethodInfoPtr_set_inventoryShape_Private_set_Void_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674734);
		NativeMethodInfoPtr__ctor_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674735);
		NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674736);
		NativeMethodInfoPtr__ctor_Public_Void_String_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674737);
		NativeMethodInfoPtr_SetShape_Public_GameGridInventory_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674738);
		NativeMethodInfoPtr_SetShape_Public_GameGridInventory_String_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674739);
		NativeMethodInfoPtr_GetLocalPosition_Private_Vector2_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674740);
		NativeMethodInfoPtr_GetGridPosition_Private_GridShape_Vector2_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674741);
		NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674742);
		NativeMethodInfoPtr_Validate_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674743);
		NativeMethodInfoPtr_SetTransparency_Public_GameGridInventory_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674744);
		NativeMethodInfoPtr_ValidateBackground_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674745);
		NativeMethodInfoPtr_ForceRecomputeDynamicBackground_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674746);
		NativeMethodInfoPtr_AddParent_Public_Virtual_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674747);
		NativeMethodInfoPtr_RemoveParent_Public_Virtual_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674748);
		NativeMethodInfoPtr_IsAdjacent_Public_Virtual_Boolean_SlotMarker_SlotMarker_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674749);
		NativeMethodInfoPtr_InitHighlightSquare_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674750);
		NativeMethodInfoPtr_SetHighlightSquare_Private_TreeNodeRender_Int32_Int32_BackgroundColor_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674751);
		NativeMethodInfoPtr_HighlightSlot_Public_Virtual_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674752);
		NativeMethodInfoPtr_ClearHighlight_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674753);
		NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_List_1_TreeNodeRender_BackgroundColor_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674754);
		NativeMethodInfoPtr_get_SupportsGroupPlacement_Public_Virtual_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674755);
		NativeMethodInfoPtr_CheckShapePlacement_Public_Virtual_Boolean_GameItem_GridShape_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674756);
		NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Virtual_Boolean_GameItem_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674757);
		NativeMethodInfoPtr_TryInventorySlot_Public_Virtual_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674758);
		NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Virtual_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674759);
		NativeMethodInfoPtr_UncheckedAccept_Public_Virtual_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674760);
		NativeMethodInfoPtr_Expel_Public_Virtual_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674761);
		NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674762);
		NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674763);
		NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_SlotMarker_GameItem_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674764);
		NativeMethodInfoPtr_TryFindOneValidInventorySlot_TopMiddle_Public_SlotMarker_GameItem_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674765);
		NativeMethodInfoPtr_ComputeShapeHash_Private_Int32_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674766);
		NativeMethodInfoPtr_HideBorder_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674767);
		NativeMethodInfoPtr___ctor_b__48_0_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr, 100674768);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 348639, RefRangeEnd = 348642, XrefRangeStart = 348561, XrefRangeEnd = 348639, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameGridInventory()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr))
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(60)]
	[CachedScanResults(RefRangeStart = 348652, RefRangeEnd = 348712, XrefRangeStart = 348642, XrefRangeEnd = 348652, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameGridInventory(int width, int height)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr))
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&width);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &height;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(8)]
	[CachedScanResults(RefRangeStart = 348722, RefRangeEnd = 348730, XrefRangeStart = 348712, XrefRangeEnd = 348722, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameGridInventory(string shape, int width)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameGridInventory>.NativeClassPtr))
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(shape);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &width;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_String_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(12)]
	[CachedScanResults(RefRangeStart = 348738, RefRangeEnd = 348750, XrefRangeStart = 348730, XrefRangeEnd = 348738, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameGridInventory SetShape(int width, int height)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&width);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &height;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetShape_Public_GameGridInventory_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameGridInventory>(intPtr2) : null;
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 348759, RefRangeEnd = 348761, XrefRangeStart = 348750, XrefRangeEnd = 348759, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameGridInventory SetShape(string shape, int width)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(shape);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &width;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetShape_Public_GameGridInventory_String_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameGridInventory>(intPtr2) : null;
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
	[CachedScanResults(RefRangeStart = 348785, RefRangeEnd = 348786, XrefRangeStart = 348761, XrefRangeEnd = 348785, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	[CallerCount(10210)]
	[CachedScanResults(RefRangeStart = 18631, RefRangeEnd = 28841, XrefRangeStart = 18631, XrefRangeEnd = 28841, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348786, XrefRangeEnd = 348809, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void Validate()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Validate_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(10)]
	[CachedScanResults(RefRangeStart = 348809, RefRangeEnd = 348819, XrefRangeStart = 348809, XrefRangeEnd = 348809, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameGridInventory SetTransparency(float transparency)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&transparency);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetTransparency_Public_GameGridInventory_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameGridInventory>(intPtr2) : null;
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 348986, RefRangeEnd = 348989, XrefRangeStart = 348819, XrefRangeEnd = 348986, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ValidateBackground()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ValidateBackground_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348989, XrefRangeEnd = 348990, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ForceRecomputeDynamicBackground()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ForceRecomputeDynamicBackground_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 348990, XrefRangeEnd = 349008, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349008, XrefRangeEnd = 349020, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void InitHighlightSquare()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InitHighlightSquare_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 349046, RefRangeEnd = 349047, XrefRangeStart = 349020, XrefRangeEnd = 349046, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349047, XrefRangeEnd = 349070, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349070, XrefRangeEnd = 349085, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void ClearHighlight()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_ClearHighlight_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349085, XrefRangeEnd = 349102, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349102, XrefRangeEnd = 349144, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349144, XrefRangeEnd = 349189, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349189, XrefRangeEnd = 349249, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349249, XrefRangeEnd = 349259, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349259, XrefRangeEnd = 349283, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349283, XrefRangeEnd = 349305, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349305, XrefRangeEnd = 349343, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349343, XrefRangeEnd = 349344, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 349406, RefRangeEnd = 349410, XrefRangeStart = 349344, XrefRangeEnd = 349406, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 349472, RefRangeEnd = 349473, XrefRangeStart = 349410, XrefRangeEnd = 349472, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe SlotMarker TryFindOneValidInventorySlot_TopMiddle(GameItem item, bool keepOrientation = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(bool**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &keepOrientation;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TryFindOneValidInventorySlot_TopMiddle_Public_SlotMarker_GameItem_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349473, XrefRangeEnd = 349486, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int ComputeShapeHash(GridShape shape)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)shape);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ComputeShapeHash_Private_Int32_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 349488, RefRangeEnd = 349491, XrefRangeStart = 349486, XrefRangeEnd = 349488, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void HideBorder()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_HideBorder_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 349491, XrefRangeEnd = 349493, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__48_0(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__48_0_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	public GameGridInventory(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
