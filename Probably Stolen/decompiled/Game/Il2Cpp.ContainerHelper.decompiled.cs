using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;

namespace Il2Cpp;

public class ContainerHelper : Il2CppSystem.Object
{
	[ObfuscatedName("ContainerHelper+<>c__DisplayClass7_0")]
	public sealed class __c__DisplayClass7_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_ownedOnly;

		private static readonly System.IntPtr NativeFieldInfoPtr_excludeContainer;

		private static readonly System.IntPtr NativeFieldInfoPtr_tag;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AllowOnlyTaggedItem_b__0_Internal_Boolean_GameItem_GameInventory_0;

		public unsafe bool ownedOnly
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ownedOnly);
				return *(bool*)num;
			}
			set
			{
				*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ownedOnly)) = value;
			}
		}

		public unsafe bool excludeContainer
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_excludeContainer);
				return *(bool*)num;
			}
			set
			{
				*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_excludeContainer)) = value;
			}
		}

		public unsafe string tag
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tag);
				return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tag)), IL2CPP.ManagedStringToIl2Cpp(value));
			}
		}

		static __c__DisplayClass7_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass7_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, "<>c__DisplayClass7_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass7_0>.NativeClassPtr);
			NativeFieldInfoPtr_ownedOnly = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass7_0>.NativeClassPtr, "ownedOnly");
			NativeFieldInfoPtr_excludeContainer = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass7_0>.NativeClassPtr, "excludeContainer");
			NativeFieldInfoPtr_tag = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass7_0>.NativeClassPtr, "tag");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass7_0>.NativeClassPtr, 100666576);
			NativeMethodInfoPtr__AllowOnlyTaggedItem_b__0_Internal_Boolean_GameItem_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass7_0>.NativeClassPtr, 100666577);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass7_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass7_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 170406, XrefRangeEnd = 170415, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _AllowOnlyTaggedItem_b__0(GameItem item, GameInventory _)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)_);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AllowOnlyTaggedItem_b__0_Internal_Boolean_GameItem_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass7_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("ContainerHelper+<>c__DisplayClass8_0")]
	public sealed class __c__DisplayClass8_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_excludeContainer;

		private static readonly System.IntPtr NativeFieldInfoPtr_tag;

		private static readonly System.IntPtr NativeFieldInfoPtr_invItem;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AllowOnlyTaggedItemOwn_b__0_Internal_Boolean_GameItem_GameInventory_0;

		public unsafe bool excludeContainer
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_excludeContainer);
				return *(bool*)num;
			}
			set
			{
				*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_excludeContainer)) = value;
			}
		}

		public unsafe string tag
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tag);
				return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tag)), IL2CPP.ManagedStringToIl2Cpp(value));
			}
		}

		public unsafe GameItem invItem
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_invItem);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_invItem)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass8_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass8_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, "<>c__DisplayClass8_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass8_0>.NativeClassPtr);
			NativeFieldInfoPtr_excludeContainer = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass8_0>.NativeClassPtr, "excludeContainer");
			NativeFieldInfoPtr_tag = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass8_0>.NativeClassPtr, "tag");
			NativeFieldInfoPtr_invItem = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass8_0>.NativeClassPtr, "invItem");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass8_0>.NativeClassPtr, 100666578);
			NativeMethodInfoPtr__AllowOnlyTaggedItemOwn_b__0_Internal_Boolean_GameItem_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass8_0>.NativeClassPtr, 100666579);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass8_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass8_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 170415, XrefRangeEnd = 170431, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _AllowOnlyTaggedItemOwn_b__0(GameItem item, GameInventory _)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)_);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AllowOnlyTaggedItemOwn_b__0_Internal_Boolean_GameItem_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass8_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("ContainerHelper+<>c__DisplayClass9_0")]
	public sealed class __c__DisplayClass9_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_ownedOnly;

		private static readonly System.IntPtr NativeFieldInfoPtr_tags;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AllowOnlyTaggedItemsOr_b__0_Internal_Boolean_GameItem_GameInventory_0;

		public unsafe bool ownedOnly
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ownedOnly);
				return *(bool*)num;
			}
			set
			{
				*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_ownedOnly)) = value;
			}
		}

		public unsafe Il2CppStringArray tags
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tags);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppStringArray>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tags)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass9_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass9_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, "<>c__DisplayClass9_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass9_0>.NativeClassPtr);
			NativeFieldInfoPtr_ownedOnly = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass9_0>.NativeClassPtr, "ownedOnly");
			NativeFieldInfoPtr_tags = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass9_0>.NativeClassPtr, "tags");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass9_0>.NativeClassPtr, 100666580);
			NativeMethodInfoPtr__AllowOnlyTaggedItemsOr_b__0_Internal_Boolean_GameItem_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass9_0>.NativeClassPtr, 100666581);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass9_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass9_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 170431, XrefRangeEnd = 170437, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _AllowOnlyTaggedItemsOr_b__0(GameItem item, GameInventory _)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)_);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AllowOnlyTaggedItemsOr_b__0_Internal_Boolean_GameItem_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass9_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("ContainerHelper+<>c__DisplayClass10_0")]
	public sealed class __c__DisplayClass10_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_tags;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AllowOnlyTaggedItemsAnd_b__0_Internal_Boolean_GameItem_GameInventory_0;

		public unsafe Il2CppStringArray tags
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tags);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppStringArray>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_tags)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass10_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass10_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, "<>c__DisplayClass10_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass10_0>.NativeClassPtr);
			NativeFieldInfoPtr_tags = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass10_0>.NativeClassPtr, "tags");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass10_0>.NativeClassPtr, 100666582);
			NativeMethodInfoPtr__AllowOnlyTaggedItemsAnd_b__0_Internal_Boolean_GameItem_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass10_0>.NativeClassPtr, 100666583);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass10_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass10_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 170437, XrefRangeEnd = 170439, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _AllowOnlyTaggedItemsAnd_b__0(GameItem item, GameInventory _)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)_);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AllowOnlyTaggedItemsAnd_b__0_Internal_Boolean_GameItem_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass10_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[System.Serializable]
	[ObfuscatedName("ContainerHelper+<>c")]
	public sealed class __c : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___9;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__11_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__18_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AllowOnlyOwnedItems_b__11_0_Internal_Boolean_GameItem_GameInventory_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__InitSmugglerBay_b__18_0_Internal_Boolean_GameItem_GameInventory_0;

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

		public unsafe static Il2CppSystem.Func<GameItem, GameInventory, bool> __9__11_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__11_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<GameItem, GameInventory, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__11_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<GameItem, GameInventory, bool> __9__18_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__18_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<GameItem, GameInventory, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__18_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c()
		{
			Il2CppClassPointerStore<__c>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, "<>c");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c>.NativeClassPtr);
			NativeFieldInfoPtr___9 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9");
			NativeFieldInfoPtr___9__11_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__11_0");
			NativeFieldInfoPtr___9__18_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__18_0");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100666585);
			NativeMethodInfoPtr__AllowOnlyOwnedItems_b__11_0_Internal_Boolean_GameItem_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100666586);
			NativeMethodInfoPtr__InitSmugglerBay_b__18_0_Internal_Boolean_GameItem_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100666587);
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
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 170439, XrefRangeEnd = 170445, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _AllowOnlyOwnedItems_b__11_0(GameItem item, GameInventory _)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)_);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AllowOnlyOwnedItems_b__11_0_Internal_Boolean_GameItem_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 170445, XrefRangeEnd = 170487, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _InitSmugglerBay_b__18_0(GameItem addedItem, GameInventory inv)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)addedItem);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)inv);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__InitSmugglerBay_b__18_0_Internal_Boolean_GameItem_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("ContainerHelper+<>c__DisplayClass16_0")]
	public sealed class __c__DisplayClass16_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_allowedTags;

		private static readonly System.IntPtr NativeFieldInfoPtr_allowedId;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__InitContainerItem_b__0_Internal_Boolean_GameItem_GameInventory_0;

		public unsafe List<string> allowedTags
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_allowedTags);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<string>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_allowedTags)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe List<string> allowedId
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_allowedId);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<string>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_allowedId)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass16_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass16_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, "<>c__DisplayClass16_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass16_0>.NativeClassPtr);
			NativeFieldInfoPtr_allowedTags = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass16_0>.NativeClassPtr, "allowedTags");
			NativeFieldInfoPtr_allowedId = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass16_0>.NativeClassPtr, "allowedId");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass16_0>.NativeClassPtr, 100666588);
			NativeMethodInfoPtr__InitContainerItem_b__0_Internal_Boolean_GameItem_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass16_0>.NativeClassPtr, 100666589);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass16_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass16_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 170487, XrefRangeEnd = 170512, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _InitContainerItem_b__0(GameItem addedItem, GameInventory inv)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)addedItem);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)inv);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__InitContainerItem_b__0_Internal_Boolean_GameItem_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass16_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeMethodInfoPtr_AllowOnlyTaggedItem_Public_Static_Void_GameInventory_String_Boolean_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AllowOnlyTaggedItemOwn_Public_Static_Void_GameInventory_GameItem_String_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AllowOnlyTaggedItemsOr_Public_Static_Void_GameInventory_Il2CppStringArray_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AllowOnlyTaggedItemsAnd_Public_Static_Void_GameInventory_Il2CppStringArray_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AllowOnlyOwnedItems_Public_Static_Void_GameInventory_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InitPouchItem_Public_Static_Void_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsNestableDevice_Public_Static_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InitContainerItem_Public_Static_Void_GameInventory_GameItem_List_1_String_List_1_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InitSmugglerBay_Public_Static_Void_GameInventory_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InitContainerItemTagOnly_Public_Static_Void_GameInventory_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InitContainerItemTagOnly_Public_Static_Void_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InitBackpackItem_Public_Static_Void_GameInventory_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InitModularBackpack_Public_Static_Void_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CreateContainerTooltip_Public_Static_Void_RichTextBuilder_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddContainerTooltipTag_Public_Static_Void_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveContainerTooltipTag_Public_Static_Void_GameItem_0;

	static ContainerHelper()
	{
		Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "ContainerHelper");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr);
		NativeMethodInfoPtr_AllowOnlyTaggedItem_Public_Static_Void_GameInventory_String_Boolean_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666560);
		NativeMethodInfoPtr_AllowOnlyTaggedItemOwn_Public_Static_Void_GameInventory_GameItem_String_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666561);
		NativeMethodInfoPtr_AllowOnlyTaggedItemsOr_Public_Static_Void_GameInventory_Il2CppStringArray_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666562);
		NativeMethodInfoPtr_AllowOnlyTaggedItemsAnd_Public_Static_Void_GameInventory_Il2CppStringArray_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666563);
		NativeMethodInfoPtr_AllowOnlyOwnedItems_Public_Static_Void_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666564);
		NativeMethodInfoPtr_InitPouchItem_Public_Static_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666565);
		NativeMethodInfoPtr_IsNestableDevice_Public_Static_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666566);
		NativeMethodInfoPtr_InitContainerItem_Public_Static_Void_GameInventory_GameItem_List_1_String_List_1_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666567);
		NativeMethodInfoPtr_InitSmugglerBay_Public_Static_Void_GameInventory_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666568);
		NativeMethodInfoPtr_InitContainerItemTagOnly_Public_Static_Void_GameInventory_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666569);
		NativeMethodInfoPtr_InitContainerItemTagOnly_Public_Static_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666570);
		NativeMethodInfoPtr_InitBackpackItem_Public_Static_Void_GameInventory_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666571);
		NativeMethodInfoPtr_InitModularBackpack_Public_Static_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666572);
		NativeMethodInfoPtr_CreateContainerTooltip_Public_Static_Void_RichTextBuilder_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666573);
		NativeMethodInfoPtr_AddContainerTooltipTag_Public_Static_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666574);
		NativeMethodInfoPtr_RemoveContainerTooltipTag_Public_Static_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<ContainerHelper>.NativeClassPtr, 100666575);
	}

	[CallerCount(43)]
	[CachedScanResults(RefRangeStart = 170525, RefRangeEnd = 170568, XrefRangeStart = 170512, XrefRangeEnd = 170525, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void AllowOnlyTaggedItem(GameInventory itemInventory, string tag, bool ownedOnly = false, bool excludeContainer = false)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemInventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(tag);
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &ownedOnly;
		*(bool**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &excludeContainer;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AllowOnlyTaggedItem_Public_Static_Void_GameInventory_String_Boolean_Boolean_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 170582, RefRangeEnd = 170585, XrefRangeStart = 170568, XrefRangeEnd = 170582, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void AllowOnlyTaggedItemOwn(GameInventory itemInventory, GameItem invItem, string tag, bool excludeContainer = false)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemInventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)invItem);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(tag);
		*(bool**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &excludeContainer;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AllowOnlyTaggedItemOwn_Public_Static_Void_GameInventory_GameItem_String_Boolean_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(28)]
	[CachedScanResults(RefRangeStart = 170602, RefRangeEnd = 170630, XrefRangeStart = 170585, XrefRangeEnd = 170602, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void AllowOnlyTaggedItemsOr(GameInventory itemInventory, Il2CppStringArray tags, bool ownedOnly = false)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemInventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tags);
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &ownedOnly;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AllowOnlyTaggedItemsOr_Public_Static_Void_GameInventory_Il2CppStringArray_Boolean_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 170647, RefRangeEnd = 170651, XrefRangeStart = 170630, XrefRangeEnd = 170647, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void AllowOnlyTaggedItemsAnd(GameInventory itemInventory, Il2CppStringArray tags)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemInventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tags);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AllowOnlyTaggedItemsAnd_Public_Static_Void_GameInventory_Il2CppStringArray_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 170668, RefRangeEnd = 170671, XrefRangeStart = 170651, XrefRangeEnd = 170668, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void AllowOnlyOwnedItems(GameInventory itemInventory)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemInventory);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AllowOnlyOwnedItems_Public_Static_Void_GameInventory_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(15)]
	[CachedScanResults(RefRangeStart = 170680, RefRangeEnd = 170695, XrefRangeStart = 170671, XrefRangeEnd = 170680, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void InitPouchItem(GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InitPouchItem_Public_Static_Void_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 170695, XrefRangeEnd = 170699, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static bool IsNestableDevice(GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsNestableDevice_Public_Static_Boolean_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(23)]
	[CachedScanResults(RefRangeStart = 170723, RefRangeEnd = 170746, XrefRangeStart = 170699, XrefRangeEnd = 170723, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void InitContainerItem(GameInventory itemInventory, GameItem item, List<string> allowedTags = null, List<string> allowedId = null)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemInventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)allowedTags);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)allowedId);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InitContainerItem_Public_Static_Void_GameInventory_GameItem_List_1_String_List_1_String_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 170773, RefRangeEnd = 170776, XrefRangeStart = 170746, XrefRangeEnd = 170773, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void InitSmugglerBay(GameInventory itemInventory, GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemInventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InitSmugglerBay_Public_Static_Void_GameInventory_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(7)]
	[CachedScanResults(RefRangeStart = 170783, RefRangeEnd = 170790, XrefRangeStart = 170776, XrefRangeEnd = 170783, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void InitContainerItemTagOnly(GameInventory itemInventory, GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemInventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InitContainerItemTagOnly_Public_Static_Void_GameInventory_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 170797, RefRangeEnd = 170799, XrefRangeStart = 170790, XrefRangeEnd = 170797, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void InitContainerItemTagOnly(GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InitContainerItemTagOnly_Public_Static_Void_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(24)]
	[CachedScanResults(RefRangeStart = 170831, RefRangeEnd = 170855, XrefRangeStart = 170799, XrefRangeEnd = 170831, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void InitBackpackItem(GameInventory itemInventory, GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemInventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InitBackpackItem_Public_Static_Void_GameInventory_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 170860, RefRangeEnd = 170863, XrefRangeStart = 170855, XrefRangeEnd = 170860, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void InitModularBackpack(GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_InitModularBackpack_Public_Static_Void_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 170879, RefRangeEnd = 170880, XrefRangeStart = 170863, XrefRangeEnd = 170879, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void CreateContainerTooltip(RichTextBuilder builder, GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)builder);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CreateContainerTooltip_Public_Static_Void_RichTextBuilder_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 170884, RefRangeEnd = 170888, XrefRangeStart = 170880, XrefRangeEnd = 170884, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void AddContainerTooltipTag(GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AddContainerTooltipTag_Public_Static_Void_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 170892, RefRangeEnd = 170894, XrefRangeStart = 170888, XrefRangeEnd = 170892, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void RemoveContainerTooltipTag(GameItem item)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveContainerTooltipTag_Public_Static_Void_GameItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	public ContainerHelper(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
