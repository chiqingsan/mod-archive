using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Reflection;
using UnityEngine;

namespace Il2Cpp;

public class GameItem : GameItemFunc
{
	[ObfuscatedName("GameItem+<>c__DisplayClass246_0")]
	public sealed class __c__DisplayClass246_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_newDict;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ComputeModifiedShapeState_b__1_Internal_Void_ItemStateModifier_0;

		public unsafe TagSystem newDict
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newDict);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TagSystem>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newDict)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass246_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass246_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<>c__DisplayClass246_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass246_0>.NativeClassPtr);
			NativeFieldInfoPtr_newDict = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass246_0>.NativeClassPtr, "newDict");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass246_0>.NativeClassPtr, 100675442);
			NativeMethodInfoPtr__ComputeModifiedShapeState_b__1_Internal_Void_ItemStateModifier_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass246_0>.NativeClassPtr, 100675443);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass246_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass246_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353918, XrefRangeEnd = 353920, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _ComputeModifiedShapeState_b__1(ItemStateModifier s)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)s);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ComputeModifiedShapeState_b__1_Internal_Void_ItemStateModifier_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c__DisplayClass246_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[System.Serializable]
	[ObfuscatedName("GameItem+<>c")]
	public new sealed class __c : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___9;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__246_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__314_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ComputeModifiedShapeState_b__246_0_Internal_Int32_ItemStateModifier_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__RemoveTemporaryClientFeature_b__314_0_Internal_Boolean_ItemFeature_0;

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

		public unsafe static Il2CppSystem.Func<ItemStateModifier, int> __9__246_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__246_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<ItemStateModifier, int>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__246_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Predicate<ItemFeature> __9__314_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__314_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Predicate<ItemFeature>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__314_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c()
		{
			Il2CppClassPointerStore<__c>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<>c");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c>.NativeClassPtr);
			NativeFieldInfoPtr___9 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9");
			NativeFieldInfoPtr___9__246_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__246_0");
			NativeFieldInfoPtr___9__314_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__314_0");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100675445);
			NativeMethodInfoPtr__ComputeModifiedShapeState_b__246_0_Internal_Int32_ItemStateModifier_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100675446);
			NativeMethodInfoPtr__RemoveTemporaryClientFeature_b__314_0_Internal_Boolean_ItemFeature_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100675447);
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
		public unsafe int _ComputeModifiedShapeState_b__246_0(ItemStateModifier a)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)a);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ComputeModifiedShapeState_b__246_0_Internal_Int32_ItemStateModifier_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		public unsafe bool _RemoveTemporaryClientFeature_b__314_0(ItemFeature feature)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)feature);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__RemoveTemporaryClientFeature_b__314_0_Internal_Boolean_ItemFeature_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameItem+<>c__DisplayClass271_0")]
	public sealed class __c__DisplayClass271_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_value;

		private static readonly System.IntPtr NativeFieldInfoPtr_type;

		private static readonly System.IntPtr NativeFieldInfoPtr_use;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AddAmenities_b__0_Internal_Void_TagState_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AddAmenities_b__1_Internal_Void_TagState_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AddAmenities_b__2_Internal_Void_TagState_0;

		public unsafe int value
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_value);
				return *(int*)num;
			}
			set
			{
				*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_value)) = value;
			}
		}

		public unsafe string type
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_type);
				return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_type)), IL2CPP.ManagedStringToIl2Cpp(value));
			}
		}

		public unsafe int use
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_use);
				return *(int*)num;
			}
			set
			{
				*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_use)) = value;
			}
		}

		static __c__DisplayClass271_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<>c__DisplayClass271_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr);
			NativeFieldInfoPtr_value = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr, "value");
			NativeFieldInfoPtr_type = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr, "type");
			NativeFieldInfoPtr_use = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr, "use");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr, 100675448);
			NativeMethodInfoPtr__AddAmenities_b__0_Internal_Void_TagState_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr, 100675449);
			NativeMethodInfoPtr__AddAmenities_b__1_Internal_Void_TagState_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr, 100675450);
			NativeMethodInfoPtr__AddAmenities_b__2_Internal_Void_TagState_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr, 100675451);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass271_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass271_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _AddAmenities_b__0(TagState state)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)state);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AddAmenities_b__0_Internal_Void_TagState_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _AddAmenities_b__1(TagState state)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)state);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AddAmenities_b__1_Internal_Void_TagState_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _AddAmenities_b__2(TagState state)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)state);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AddAmenities_b__2_Internal_Void_TagState_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c__DisplayClass271_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameItem+<>c__DisplayClass272_0")]
	public sealed class __c__DisplayClass272_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_proficiencyType;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__EnableProficiency_b__0_Internal_Void_TagState_0;

		public unsafe string proficiencyType
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_proficiencyType);
				return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_proficiencyType)), IL2CPP.ManagedStringToIl2Cpp(value));
			}
		}

		static __c__DisplayClass272_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass272_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<>c__DisplayClass272_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass272_0>.NativeClassPtr);
			NativeFieldInfoPtr_proficiencyType = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass272_0>.NativeClassPtr, "proficiencyType");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass272_0>.NativeClassPtr, 100675452);
			NativeMethodInfoPtr__EnableProficiency_b__0_Internal_Void_TagState_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass272_0>.NativeClassPtr, 100675453);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass272_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass272_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _EnableProficiency_b__0(TagState state)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)state);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__EnableProficiency_b__0_Internal_Void_TagState_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c__DisplayClass272_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameItem+<>c__DisplayClass273_0")]
	public sealed class __c__DisplayClass273_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_manufacturer;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__EnableManufacturer_b__0_Internal_Void_TagState_0;

		public unsafe string manufacturer
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_manufacturer);
				return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_manufacturer)), IL2CPP.ManagedStringToIl2Cpp(value));
			}
		}

		static __c__DisplayClass273_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass273_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<>c__DisplayClass273_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass273_0>.NativeClassPtr);
			NativeFieldInfoPtr_manufacturer = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass273_0>.NativeClassPtr, "manufacturer");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass273_0>.NativeClassPtr, 100675454);
			NativeMethodInfoPtr__EnableManufacturer_b__0_Internal_Void_TagState_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass273_0>.NativeClassPtr, 100675455);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass273_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass273_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _EnableManufacturer_b__0(TagState state)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)state);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__EnableManufacturer_b__0_Internal_Void_TagState_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c__DisplayClass273_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameItem+<>c__DisplayClass275_0")]
	public sealed class __c__DisplayClass275_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_type;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__SetType_b__0_Internal_Void_TagState_0;

		public unsafe string type
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_type);
				return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_type)), IL2CPP.ManagedStringToIl2Cpp(value));
			}
		}

		static __c__DisplayClass275_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass275_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<>c__DisplayClass275_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass275_0>.NativeClassPtr);
			NativeFieldInfoPtr_type = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass275_0>.NativeClassPtr, "type");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass275_0>.NativeClassPtr, 100675456);
			NativeMethodInfoPtr__SetType_b__0_Internal_Void_TagState_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass275_0>.NativeClassPtr, 100675457);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass275_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass275_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _SetType_b__0(TagState state)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)state);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__SetType_b__0_Internal_Void_TagState_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c__DisplayClass275_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameItem+<>c__DisplayClass312_0")]
	public sealed class __c__DisplayClass312_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_itemFeature;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__AddItemFeature_b__0_Internal_Boolean_ItemFeature_0;

		public unsafe ItemFeature itemFeature
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_itemFeature);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<ItemFeature>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_itemFeature)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass312_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass312_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<>c__DisplayClass312_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass312_0>.NativeClassPtr);
			NativeFieldInfoPtr_itemFeature = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass312_0>.NativeClassPtr, "itemFeature");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass312_0>.NativeClassPtr, 100675458);
			NativeMethodInfoPtr__AddItemFeature_b__0_Internal_Boolean_ItemFeature_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass312_0>.NativeClassPtr, 100675459);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass312_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass312_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353920, XrefRangeEnd = 353938, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _AddItemFeature_b__0(ItemFeature feature)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)feature);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__AddItemFeature_b__0_Internal_Boolean_ItemFeature_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass312_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private sealed class MethodInfoStoreGeneric_FindAdjacentType_Public_List_1_T_0<T>
	{
		internal static System.IntPtr Pointer = IL2CPP.il2cpp_method_get_from_reflection(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)new MethodInfo(IL2CPP.il2cpp_method_get_object(NativeMethodInfoPtr_FindAdjacentType_Public_List_1_T_0, Il2CppClassPointerStore<GameItem>.NativeClassPtr)).MakeGenericMethod(new Il2CppReferenceArray<Il2CppSystem.Type>(new Il2CppSystem.Type[1] { Il2CppSystem.Type.internal_from_handle(IL2CPP.il2cpp_class_get_type(Il2CppClassPointerStore<T>.NativeClassPtr)) }))));
	}

	private sealed class MethodInfoStoreGeneric_EnableTag_Public_Boolean_T_0<T>
	{
		internal static System.IntPtr Pointer = IL2CPP.il2cpp_method_get_from_reflection(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)new MethodInfo(IL2CPP.il2cpp_method_get_object(NativeMethodInfoPtr_EnableTag_Public_Boolean_T_0, Il2CppClassPointerStore<GameItem>.NativeClassPtr)).MakeGenericMethod(new Il2CppReferenceArray<Il2CppSystem.Type>(new Il2CppSystem.Type[1] { Il2CppSystem.Type.internal_from_handle(IL2CPP.il2cpp_class_get_type(Il2CppClassPointerStore<T>.NativeClassPtr)) }))));
	}

	private static readonly System.IntPtr NativeFieldInfoPtr__parentInventory_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__shape_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__modifiedShape_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__modifiedXOrigin_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__modifiedYOrigin_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__state_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__modifiedState_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__identifier_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__identifierName_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__uniqueId_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__name_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__shortDescription_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__longDescription_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__flavorText_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__unitCount_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__unitValue_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__lateUnitValue_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__unitBaseValue_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__backupUnitValue_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__recipeManager_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__customText_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__contentWindow_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__examineWindow_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__forceDisableActivate_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__forceDisableUse_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__activateDefault_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__isCombatBackpack_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__spritePath_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__spriteAtlasPath_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__spriteChanged_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__useSpriteFromMod_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__isDebugMenu_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__soundDragStart_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__soundDragEnd_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__soundContainerOpen_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__soundContainerClose_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__soundSelect_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__soundExamineOpen_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__soundToggle_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__soundActivate_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__soundUse_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_ITEM_TYPE_STRING;

	private static readonly System.IntPtr NativeFieldInfoPtr_itemFeatures;

	private static readonly System.IntPtr NativeFieldInfoPtr_itemTypes;

	private static readonly System.IntPtr NativeFieldInfoPtr_warnedMisalignedSprites;

	private static readonly System.IntPtr NativeFieldInfoPtr__bonusAccuracy_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__effects_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__triggerOverwatch_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__canUseOutsideCombat_k__BackingField;

	private static readonly System.IntPtr NativeMethodInfoPtr_Create_Public_Static_GameItem_String_String_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_parents_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_children_Public_Virtual_New_get_List_1_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_parentInventory_Public_get_GameInventory_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_parentInventory_Public_set_Void_GameInventory_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_child_Public_get_GraphNodeWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_shape_Public_get_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_shape_Public_set_Void_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_modifiedShape_Public_get_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_modifiedShape_Public_set_Void_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_modifiedXOrigin_Public_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_modifiedXOrigin_Public_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_modifiedYOrigin_Public_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_modifiedYOrigin_Public_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_state_Public_get_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_state_Public_set_Void_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_modifiedState_Public_get_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_modifiedState_Public_set_Void_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_identifier_Public_Virtual_Final_New_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_identifier_Public_Virtual_Final_New_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_identifierName_Public_Virtual_Final_New_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_uniqueId_Public_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_uniqueId_Public_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_name_Public_Virtual_New_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_name_Public_Virtual_New_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_shortDescription_Public_Virtual_Final_New_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_shortDescription_Public_Virtual_Final_New_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_longDescription_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_longDescription_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_flavorText_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_flavorText_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_unitCount_Public_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_unitCount_Public_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_unitValue_Public_get_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_unitValue_Public_set_Void_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_lateUnitValue_Public_get_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_lateUnitValue_Public_set_Void_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_unitBaseValue_Public_get_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_unitBaseValue_Public_set_Void_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_backupUnitValue_Public_get_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_backupUnitValue_Public_set_Void_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_ability_Public_get_CombatAbility_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_ability_Public_set_Void_CombatAbility_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_recipeManager_Public_get_RecipeManager_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_recipeManager_Public_set_Void_RecipeManager_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_customText_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_customText_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_contentWindow_Public_get_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_contentWindow_Protected_set_Void_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_examineWindow_Public_get_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_examineWindow_Protected_set_Void_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_forceDisableActivate_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_forceDisableActivate_Public_set_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_forceDisableUse_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_forceDisableUse_Public_set_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_activateDefault_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_activateDefault_Public_set_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_isCombatBackpack_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_isCombatBackpack_Public_set_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_spritePath_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_spritePath_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_spriteAtlasPath_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_spriteAtlasPath_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_spriteChanged_Protected_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_spriteChanged_Protected_set_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_useSpriteFromMod_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_isDebugMenu_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_isDebugMenu_Public_set_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_soundDragStart_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_soundDragStart_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_soundDragEnd_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_soundDragEnd_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_soundContainerOpen_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_soundContainerOpen_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_soundContainerClose_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_soundContainerClose_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_soundSelect_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_soundSelect_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_soundExamineOpen_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_soundExamineOpen_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_soundToggle_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_soundToggle_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_soundActivate_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_soundActivate_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_soundUse_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_soundUse_Public_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_String_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddClientItemBuyingFeature_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CloneLinked_Public_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetSprite_Public_GameItem_String_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetSpriteQuick_Public_GameItem_String_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetShape_Public_GameItem_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetSpriteAndShape_Public_GameItem_String_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_WarnIfSliceNotGridAligned_Private_Static_Void_String_String_Rect_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetValue_Public_Int64_List_1_GameCharacterItem_GameCharacterItem_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetValue_Public_GameItem_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ValueModifier_Private_Static_Int64_GameItem_List_1_GameCharacterItem_GameCharacterItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetUnitCount_Public_GameItem_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetAmount_Public_GameItem_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetContentWindow_Public_GameItem_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetExamineWindow_Public_GameItem_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FindAdjacentType_Public_List_1_T_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Final_New_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_Final_New_SlotMarker_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Destroy_Public_Abstract_Virtual_New_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Validate_Public_Abstract_Virtual_New_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetDisplayName_Public_String_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsTag_Public_Boolean_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsOneOfTags_Public_Boolean_List_1_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetTagReadonly_Public_TagState_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ModifyTag_Public_GameItem_String_Action_1_TagState_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_EnableTag_Public_Boolean_String_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_DisableTag_Public_Boolean_String_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_EnableTag_Public_Boolean_T_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetName_Public_GameItem_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayToggleSlotItem_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CanToggleSlotItem_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ToggleSlotItem_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayActivateSlotItem_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CanActivateSlotItem_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ActivateSlotItem_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MaySelectSlotItem_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CanSelectSlotItem_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayTarget_Public_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CanTarget_Public_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Target_Public_Void_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayParentShapeModifier_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetParentShapeModifier_Public_ValueTuple_2_GridShape_GridShape_GameItem_SlotMarker_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetParentStateModifier_Public_ValueTuple_2_List_1_ItemStateModifier_List_1_ItemStateModifier_GameItem_SlotMarker_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayRemove_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MaxNumRemove_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MaxNumAdd_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MaxNumAdd_Public_Int32_SlotMarker_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayStack_Public_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetStackSlot_Public_SlotMarker_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetIncompleteSlotInInventory_Public_SlotMarker_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetSplitItem_Public_GameItem_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckIsStateValid_Public_Boolean_GameInventory_SlotMarker_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckValidShapeState_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SyncModifiedState_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ValidateShapeState_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ComputeModifiedShapeState_Public_ValueTuple_4_GridShape_Int32_Int32_TagSystem_GridShape_GridShape_List_1_ItemStateModifier_List_1_ItemStateModifier_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryAcceptAll_Public_GameItem_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryUnloadAll_Public_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FindAllChildItems_Public_List_1_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FindAllRecipes_Public_List_1_CraftingRecipe_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_bonusAccuracy_Public_Virtual_Final_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_bonusAccuracy_Public_Virtual_Final_New_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_effects_Public_Virtual_Final_New_get_List_1_CombatAbilityEffect_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_effects_Public_Virtual_Final_New_set_Void_List_1_CombatAbilityEffect_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_triggerOverwatch_Public_Virtual_Final_New_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_triggerOverwatch_Public_Virtual_Final_New_set_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_canUseOutsideCombat_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_canUseOutsideCombat_Public_set_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetPrimaryTarget_Public_Virtual_Final_New_TargetType_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsAlwaysAccurate_Public_Virtual_Final_New_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddAmenities_Public_Void_String_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_EnableProficiency_Public_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_EnableManufacturer_Public_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_DisplayManufacturer_Public_Void_RichTextBuilder_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetType_Public_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AccumulateFeatureStages_Private_Void_byref_Int64_byref_Int64_byref_Double_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ComposeStagedValue_Private_Static_Int64_Int64_Int64_Int64_Double_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetNegociatedValue_Public_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetCurrentValue_Public_Int64_Boolean_Boolean_Boolean_Boolean_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckValueSafe_Public_Static_Int64_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetComplexeItemValue_Public_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetRefreshedValue_Public_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetBaseValue_Public_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetItemMiscValue_Public_Int32_Boolean_Boolean_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsApplyMarkUp_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetCurrentChildValue_Public_Int32_Boolean_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetInheritableClientMarketMod_Public_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsAlreadyContainFeatureWithId_Public_Boolean_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FindItemFeatureByID_Public_ItemFeature_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveItemFeatureByID_Public_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FindItemFeatureByCategory_Public_ItemFeature_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetIndexByCategory_Public_Int32_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnAddedToClientArea_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnAddedToShowcaseArea_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnAddedToWeightedArea_Public_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnRemovedFromWeightedArea_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddFeatureRelatedToBuyer_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveFeatureRelatedToBuyer_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveFeatureRelatedToEvent_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveFeatureTemporaryBuying_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveFeatureTemporarySelling_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveFeatureByCategory_Public_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddFeatureRelatedToEvent_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetGameItemType_Public_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsGameItemType_Public_Boolean_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveGameItemType_Public_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveAllGameItemType_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TransferGameItemTypeFrom_Public_Void_GameItem_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetGameItemType_Public_List_1_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddItemFeature_Public_Void_ItemFeature_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveTemporaryClientFeature_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CloseContentWindow_Public_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RememberContentWindowPositionIfMine_Public_Void_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RestoreContentWindowPosition_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsRootedInBackInv_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsRootedInSmugglerBay_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetParentWithIdentifier_Public_GameInventory_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FindParentWithIdentifier_Public_Static_GameInventory_GraphNodeStorage_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetUniqueID_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveAllFeature_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsConditionActive_Public_Boolean_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsID_Public_Boolean_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetItemSize_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetModifiedItemSize_Public_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__177_0_Private_Void_GameItem_GameInventory_SlotMarker_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__TryUnloadAll_b__250_0_Private_Void_GameItem_0;

	public unsafe GameInventory _parentInventory_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__parentInventory_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameInventory>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__parentInventory_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe GridShape _shape_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__shape_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__shape_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe GridShape _modifiedShape_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__modifiedShape_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__modifiedShape_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int _modifiedXOrigin_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__modifiedXOrigin_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__modifiedXOrigin_k__BackingField)) = value;
		}
	}

	public unsafe int _modifiedYOrigin_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__modifiedYOrigin_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__modifiedYOrigin_k__BackingField)) = value;
		}
	}

	public unsafe TagSystem _state_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__state_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TagSystem>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__state_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TagSystem _modifiedState_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__modifiedState_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TagSystem>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__modifiedState_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe string _identifier_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__identifier_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__identifier_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _identifierName_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__identifierName_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__identifierName_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe int _uniqueId_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__uniqueId_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__uniqueId_k__BackingField)) = value;
		}
	}

	public unsafe string _name_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__name_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__name_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _shortDescription_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__shortDescription_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__shortDescription_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _longDescription_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__longDescription_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__longDescription_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _flavorText_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__flavorText_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__flavorText_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe int _unitCount_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__unitCount_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__unitCount_k__BackingField)) = value;
		}
	}

	public unsafe long _unitValue_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__unitValue_k__BackingField);
			return *(long*)num;
		}
		set
		{
			*(long*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__unitValue_k__BackingField)) = value;
		}
	}

	public unsafe long _lateUnitValue_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lateUnitValue_k__BackingField);
			return *(long*)num;
		}
		set
		{
			*(long*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__lateUnitValue_k__BackingField)) = value;
		}
	}

	public unsafe long _unitBaseValue_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__unitBaseValue_k__BackingField);
			return *(long*)num;
		}
		set
		{
			*(long*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__unitBaseValue_k__BackingField)) = value;
		}
	}

	public unsafe long _backupUnitValue_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__backupUnitValue_k__BackingField);
			return *(long*)num;
		}
		set
		{
			*(long*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__backupUnitValue_k__BackingField)) = value;
		}
	}

	public unsafe RecipeManager _recipeManager_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__recipeManager_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<RecipeManager>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__recipeManager_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe string _customText_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__customText_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__customText_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe PixelWindow _contentWindow_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__contentWindow_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__contentWindow_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe PixelWindow _examineWindow_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__examineWindow_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__examineWindow_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe bool _forceDisableActivate_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__forceDisableActivate_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__forceDisableActivate_k__BackingField)) = value;
		}
	}

	public unsafe bool _forceDisableUse_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__forceDisableUse_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__forceDisableUse_k__BackingField)) = value;
		}
	}

	public unsafe bool _activateDefault_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__activateDefault_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__activateDefault_k__BackingField)) = value;
		}
	}

	public unsafe bool _isCombatBackpack_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__isCombatBackpack_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__isCombatBackpack_k__BackingField)) = value;
		}
	}

	public unsafe string _spritePath_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__spritePath_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__spritePath_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _spriteAtlasPath_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__spriteAtlasPath_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__spriteAtlasPath_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe bool _spriteChanged_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__spriteChanged_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__spriteChanged_k__BackingField)) = value;
		}
	}

	public unsafe bool _useSpriteFromMod_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__useSpriteFromMod_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__useSpriteFromMod_k__BackingField)) = value;
		}
	}

	public unsafe bool _isDebugMenu_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__isDebugMenu_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__isDebugMenu_k__BackingField)) = value;
		}
	}

	public unsafe string _soundDragStart_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundDragStart_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundDragStart_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _soundDragEnd_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundDragEnd_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundDragEnd_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _soundContainerOpen_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundContainerOpen_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundContainerOpen_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _soundContainerClose_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundContainerClose_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundContainerClose_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _soundSelect_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundSelect_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundSelect_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _soundExamineOpen_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundExamineOpen_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundExamineOpen_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _soundToggle_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundToggle_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundToggle_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _soundActivate_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundActivate_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundActivate_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe string _soundUse_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundUse_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__soundUse_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe static string ITEM_TYPE_STRING
	{
		get
		{
			Unsafe.SkipInit(out System.IntPtr intPtr);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_ITEM_TYPE_STRING, (void*)(&intPtr));
			return IL2CPP.Il2CppStringToManaged(intPtr);
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_ITEM_TYPE_STRING, (void*)IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe List<ItemFeature> itemFeatures
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_itemFeatures);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<ItemFeature>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_itemFeatures)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<string> itemTypes
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_itemTypes);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<string>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_itemTypes)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe static HashSet<string> warnedMisalignedSprites
	{
		get
		{
			Unsafe.SkipInit(out System.IntPtr intPtr);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_warnedMisalignedSprites, (void*)(&intPtr));
			System.IntPtr intPtr2 = intPtr;
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<HashSet<string>>(intPtr2) : null;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_warnedMisalignedSprites, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int _bonusAccuracy_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__bonusAccuracy_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__bonusAccuracy_k__BackingField)) = value;
		}
	}

	public unsafe List<CombatAbilityEffect> _effects_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__effects_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<CombatAbilityEffect>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__effects_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe bool _triggerOverwatch_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__triggerOverwatch_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__triggerOverwatch_k__BackingField)) = value;
		}
	}

	public unsafe bool _canUseOutsideCombat_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__canUseOutsideCombat_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__canUseOutsideCombat_k__BackingField)) = value;
		}
	}

	public unsafe virtual List<GraphNodeStorage> parents
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353943, XrefRangeEnd = 353952, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_parents_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GraphNodeStorage>>(intPtr2) : null;
		}
	}

	public unsafe virtual List<GraphNodeStorage> children
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353952, XrefRangeEnd = 353963, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_children_Public_Virtual_New_get_List_1_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GraphNodeStorage>>(intPtr2) : null;
		}
	}

	public unsafe GameInventory parentInventory
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_parentInventory_Public_get_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameInventory>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353963, XrefRangeEnd = 353964, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_parentInventory_Public_set_Void_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe GraphNodeWindow child
	{
		[CallerCount(16)]
		[CachedScanResults(RefRangeStart = 353964, RefRangeEnd = 353980, XrefRangeStart = 353964, XrefRangeEnd = 353964, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_child_Public_get_GraphNodeWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GraphNodeWindow>(intPtr2) : null;
		}
	}

	public unsafe GridShape shape
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_shape_Public_get_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353980, XrefRangeEnd = 353981, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_shape_Public_set_Void_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe GridShape modifiedShape
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_modifiedShape_Public_get_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353981, XrefRangeEnd = 353982, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_modifiedShape_Public_set_Void_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe int modifiedXOrigin
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_modifiedXOrigin_Public_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_modifiedXOrigin_Public_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe int modifiedYOrigin
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_modifiedYOrigin_Public_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_modifiedYOrigin_Public_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe TagSystem state
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_state_Public_get_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TagSystem>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_state_Public_set_Void_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe TagSystem modifiedState
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_modifiedState_Public_get_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TagSystem>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353982, XrefRangeEnd = 353983, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_modifiedState_Public_set_Void_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual string identifier
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_identifier_Public_Virtual_Final_New_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353983, XrefRangeEnd = 353984, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_identifier_Public_Virtual_Final_New_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual string identifierName
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353984, XrefRangeEnd = 353985, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_identifierName_Public_Virtual_Final_New_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe int uniqueId
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_uniqueId_Public_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_uniqueId_Public_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual string name
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_name_Public_Virtual_New_get_String_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353985, XrefRangeEnd = 353986, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_set_name_Public_Virtual_New_set_Void_String_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual string shortDescription
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_shortDescription_Public_Virtual_Final_New_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353986, XrefRangeEnd = 353987, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_shortDescription_Public_Virtual_Final_New_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string longDescription
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_longDescription_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353987, XrefRangeEnd = 353988, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_longDescription_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string flavorText
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_flavorText_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 353988, XrefRangeEnd = 353989, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_flavorText_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe int unitCount
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_unitCount_Public_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_unitCount_Public_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe long unitValue
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_unitValue_Public_get_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_unitValue_Public_set_Void_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe long lateUnitValue
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_lateUnitValue_Public_get_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_lateUnitValue_Public_set_Void_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe long unitBaseValue
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_unitBaseValue_Public_get_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_unitBaseValue_Public_set_Void_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe long backupUnitValue
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_backupUnitValue_Public_get_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_backupUnitValue_Public_set_Void_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe CombatAbility ability
	{
		[CallerCount(1000)]
		[CachedScanResults(RefRangeStart = 16996, RefRangeEnd = 17996, XrefRangeStart = 16996, XrefRangeEnd = 17996, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_ability_Public_get_CombatAbility_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<CombatAbility>(intPtr2) : null;
		}
		[CallerCount(11)]
		[CachedScanResults(RefRangeStart = 354007, RefRangeEnd = 354018, XrefRangeStart = 353989, XrefRangeEnd = 354007, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_ability_Public_set_Void_CombatAbility_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe RecipeManager recipeManager
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_recipeManager_Public_get_RecipeManager_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<RecipeManager>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354018, XrefRangeEnd = 354019, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_recipeManager_Public_set_Void_RecipeManager_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string customText
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_customText_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354019, XrefRangeEnd = 354020, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_customText_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe PixelWindow contentWindow
	{
		[CallerCount(16)]
		[CachedScanResults(RefRangeStart = 353964, RefRangeEnd = 353980, XrefRangeStart = 353964, XrefRangeEnd = 353980, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_contentWindow_Public_get_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354020, XrefRangeEnd = 354021, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_contentWindow_Protected_set_Void_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe PixelWindow examineWindow
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_examineWindow_Public_get_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354021, XrefRangeEnd = 354022, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_examineWindow_Protected_set_Void_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe bool forceDisableActivate
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_forceDisableActivate_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_forceDisableActivate_Public_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe bool forceDisableUse
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_forceDisableUse_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_forceDisableUse_Public_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe bool activateDefault
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_activateDefault_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_activateDefault_Public_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe bool isCombatBackpack
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_isCombatBackpack_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_isCombatBackpack_Public_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string spritePath
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_spritePath_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354022, XrefRangeEnd = 354023, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_spritePath_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string spriteAtlasPath
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_spriteAtlasPath_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354023, XrefRangeEnd = 354024, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_spriteAtlasPath_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe bool spriteChanged
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_spriteChanged_Protected_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_spriteChanged_Protected_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe bool useSpriteFromMod
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_useSpriteFromMod_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe bool isDebugMenu
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_isDebugMenu_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_isDebugMenu_Public_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string soundDragStart
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_soundDragStart_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354024, XrefRangeEnd = 354025, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_soundDragStart_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string soundDragEnd
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_soundDragEnd_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354025, XrefRangeEnd = 354026, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_soundDragEnd_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string soundContainerOpen
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_soundContainerOpen_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354026, XrefRangeEnd = 354027, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_soundContainerOpen_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string soundContainerClose
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_soundContainerClose_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354027, XrefRangeEnd = 354028, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_soundContainerClose_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string soundSelect
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_soundSelect_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354028, XrefRangeEnd = 354029, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_soundSelect_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string soundExamineOpen
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_soundExamineOpen_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354029, XrefRangeEnd = 354030, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_soundExamineOpen_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string soundToggle
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_soundToggle_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354030, XrefRangeEnd = 354031, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_soundToggle_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string soundActivate
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_soundActivate_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354031, XrefRangeEnd = 354032, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_soundActivate_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe string soundUse
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_soundUse_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 354032, XrefRangeEnd = 354033, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_soundUse_Public_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual int bonusAccuracy
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_bonusAccuracy_Public_Virtual_Final_New_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_bonusAccuracy_Public_Virtual_Final_New_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual List<CombatAbilityEffect> effects
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_effects_Public_Virtual_Final_New_get_List_1_CombatAbilityEffect_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<CombatAbilityEffect>>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 360405, XrefRangeEnd = 360406, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_effects_Public_Virtual_Final_New_set_Void_List_1_CombatAbilityEffect_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual bool triggerOverwatch
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_triggerOverwatch_Public_Virtual_Final_New_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_triggerOverwatch_Public_Virtual_Final_New_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe bool canUseOutsideCombat
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_canUseOutsideCombat_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
		[CallerCount(0)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = (nint)(&value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_canUseOutsideCombat_Public_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	static GameItem()
	{
		Il2CppClassPointerStore<GameItem>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "GameItem");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<GameItem>.NativeClassPtr);
		NativeFieldInfoPtr__parentInventory_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<parentInventory>k__BackingField");
		NativeFieldInfoPtr__shape_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<shape>k__BackingField");
		NativeFieldInfoPtr__modifiedShape_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<modifiedShape>k__BackingField");
		NativeFieldInfoPtr__modifiedXOrigin_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<modifiedXOrigin>k__BackingField");
		NativeFieldInfoPtr__modifiedYOrigin_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<modifiedYOrigin>k__BackingField");
		NativeFieldInfoPtr__state_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<state>k__BackingField");
		NativeFieldInfoPtr__modifiedState_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<modifiedState>k__BackingField");
		NativeFieldInfoPtr__identifier_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<identifier>k__BackingField");
		NativeFieldInfoPtr__identifierName_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<identifierName>k__BackingField");
		NativeFieldInfoPtr__uniqueId_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<uniqueId>k__BackingField");
		NativeFieldInfoPtr__name_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<name>k__BackingField");
		NativeFieldInfoPtr__shortDescription_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<shortDescription>k__BackingField");
		NativeFieldInfoPtr__longDescription_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<longDescription>k__BackingField");
		NativeFieldInfoPtr__flavorText_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<flavorText>k__BackingField");
		NativeFieldInfoPtr__unitCount_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<unitCount>k__BackingField");
		NativeFieldInfoPtr__unitValue_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<unitValue>k__BackingField");
		NativeFieldInfoPtr__lateUnitValue_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<lateUnitValue>k__BackingField");
		NativeFieldInfoPtr__unitBaseValue_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<unitBaseValue>k__BackingField");
		NativeFieldInfoPtr__backupUnitValue_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<backupUnitValue>k__BackingField");
		NativeFieldInfoPtr__recipeManager_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<recipeManager>k__BackingField");
		NativeFieldInfoPtr__customText_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<customText>k__BackingField");
		NativeFieldInfoPtr__contentWindow_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<contentWindow>k__BackingField");
		NativeFieldInfoPtr__examineWindow_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<examineWindow>k__BackingField");
		NativeFieldInfoPtr__forceDisableActivate_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<forceDisableActivate>k__BackingField");
		NativeFieldInfoPtr__forceDisableUse_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<forceDisableUse>k__BackingField");
		NativeFieldInfoPtr__activateDefault_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<activateDefault>k__BackingField");
		NativeFieldInfoPtr__isCombatBackpack_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<isCombatBackpack>k__BackingField");
		NativeFieldInfoPtr__spritePath_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<spritePath>k__BackingField");
		NativeFieldInfoPtr__spriteAtlasPath_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<spriteAtlasPath>k__BackingField");
		NativeFieldInfoPtr__spriteChanged_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<spriteChanged>k__BackingField");
		NativeFieldInfoPtr__useSpriteFromMod_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<useSpriteFromMod>k__BackingField");
		NativeFieldInfoPtr__isDebugMenu_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<isDebugMenu>k__BackingField");
		NativeFieldInfoPtr__soundDragStart_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<soundDragStart>k__BackingField");
		NativeFieldInfoPtr__soundDragEnd_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<soundDragEnd>k__BackingField");
		NativeFieldInfoPtr__soundContainerOpen_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<soundContainerOpen>k__BackingField");
		NativeFieldInfoPtr__soundContainerClose_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<soundContainerClose>k__BackingField");
		NativeFieldInfoPtr__soundSelect_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<soundSelect>k__BackingField");
		NativeFieldInfoPtr__soundExamineOpen_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<soundExamineOpen>k__BackingField");
		NativeFieldInfoPtr__soundToggle_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<soundToggle>k__BackingField");
		NativeFieldInfoPtr__soundActivate_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<soundActivate>k__BackingField");
		NativeFieldInfoPtr__soundUse_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<soundUse>k__BackingField");
		NativeFieldInfoPtr_ITEM_TYPE_STRING = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "ITEM_TYPE_STRING");
		NativeFieldInfoPtr_itemFeatures = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "itemFeatures");
		NativeFieldInfoPtr_itemTypes = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "itemTypes");
		NativeFieldInfoPtr_warnedMisalignedSprites = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "warnedMisalignedSprites");
		NativeFieldInfoPtr__bonusAccuracy_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<bonusAccuracy>k__BackingField");
		NativeFieldInfoPtr__effects_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<effects>k__BackingField");
		NativeFieldInfoPtr__triggerOverwatch_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<triggerOverwatch>k__BackingField");
		NativeFieldInfoPtr__canUseOutsideCombat_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItem>.NativeClassPtr, "<canUseOutsideCombat>k__BackingField");
		NativeMethodInfoPtr_Create_Public_Static_GameItem_String_String_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675227);
		NativeMethodInfoPtr_get_parents_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675228);
		NativeMethodInfoPtr_get_children_Public_Virtual_New_get_List_1_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675229);
		NativeMethodInfoPtr_get_parentInventory_Public_get_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675230);
		NativeMethodInfoPtr_set_parentInventory_Public_set_Void_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675231);
		NativeMethodInfoPtr_get_child_Public_get_GraphNodeWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675232);
		NativeMethodInfoPtr_get_shape_Public_get_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675233);
		NativeMethodInfoPtr_set_shape_Public_set_Void_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675234);
		NativeMethodInfoPtr_get_modifiedShape_Public_get_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675235);
		NativeMethodInfoPtr_set_modifiedShape_Public_set_Void_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675236);
		NativeMethodInfoPtr_get_modifiedXOrigin_Public_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675237);
		NativeMethodInfoPtr_set_modifiedXOrigin_Public_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675238);
		NativeMethodInfoPtr_get_modifiedYOrigin_Public_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675239);
		NativeMethodInfoPtr_set_modifiedYOrigin_Public_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675240);
		NativeMethodInfoPtr_get_state_Public_get_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675241);
		NativeMethodInfoPtr_set_state_Public_set_Void_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675242);
		NativeMethodInfoPtr_get_modifiedState_Public_get_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675243);
		NativeMethodInfoPtr_set_modifiedState_Public_set_Void_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675244);
		NativeMethodInfoPtr_get_identifier_Public_Virtual_Final_New_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675245);
		NativeMethodInfoPtr_set_identifier_Public_Virtual_Final_New_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675246);
		NativeMethodInfoPtr_set_identifierName_Public_Virtual_Final_New_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675247);
		NativeMethodInfoPtr_get_uniqueId_Public_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675248);
		NativeMethodInfoPtr_set_uniqueId_Public_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675249);
		NativeMethodInfoPtr_get_name_Public_Virtual_New_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675250);
		NativeMethodInfoPtr_set_name_Public_Virtual_New_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675251);
		NativeMethodInfoPtr_get_shortDescription_Public_Virtual_Final_New_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675252);
		NativeMethodInfoPtr_set_shortDescription_Public_Virtual_Final_New_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675253);
		NativeMethodInfoPtr_get_longDescription_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675254);
		NativeMethodInfoPtr_set_longDescription_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675255);
		NativeMethodInfoPtr_get_flavorText_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675256);
		NativeMethodInfoPtr_set_flavorText_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675257);
		NativeMethodInfoPtr_get_unitCount_Public_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675258);
		NativeMethodInfoPtr_set_unitCount_Public_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675259);
		NativeMethodInfoPtr_get_unitValue_Public_get_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675260);
		NativeMethodInfoPtr_set_unitValue_Public_set_Void_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675261);
		NativeMethodInfoPtr_get_lateUnitValue_Public_get_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675262);
		NativeMethodInfoPtr_set_lateUnitValue_Public_set_Void_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675263);
		NativeMethodInfoPtr_get_unitBaseValue_Public_get_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675264);
		NativeMethodInfoPtr_set_unitBaseValue_Public_set_Void_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675265);
		NativeMethodInfoPtr_get_backupUnitValue_Public_get_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675266);
		NativeMethodInfoPtr_set_backupUnitValue_Public_set_Void_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675267);
		NativeMethodInfoPtr_get_ability_Public_get_CombatAbility_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675268);
		NativeMethodInfoPtr_set_ability_Public_set_Void_CombatAbility_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675269);
		NativeMethodInfoPtr_get_recipeManager_Public_get_RecipeManager_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675270);
		NativeMethodInfoPtr_set_recipeManager_Public_set_Void_RecipeManager_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675271);
		NativeMethodInfoPtr_get_customText_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675272);
		NativeMethodInfoPtr_set_customText_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675273);
		NativeMethodInfoPtr_get_contentWindow_Public_get_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675274);
		NativeMethodInfoPtr_set_contentWindow_Protected_set_Void_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675275);
		NativeMethodInfoPtr_get_examineWindow_Public_get_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675276);
		NativeMethodInfoPtr_set_examineWindow_Protected_set_Void_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675277);
		NativeMethodInfoPtr_get_forceDisableActivate_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675278);
		NativeMethodInfoPtr_set_forceDisableActivate_Public_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675279);
		NativeMethodInfoPtr_get_forceDisableUse_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675280);
		NativeMethodInfoPtr_set_forceDisableUse_Public_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675281);
		NativeMethodInfoPtr_get_activateDefault_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675282);
		NativeMethodInfoPtr_set_activateDefault_Public_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675283);
		NativeMethodInfoPtr_get_isCombatBackpack_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675284);
		NativeMethodInfoPtr_set_isCombatBackpack_Public_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675285);
		NativeMethodInfoPtr_get_spritePath_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675286);
		NativeMethodInfoPtr_set_spritePath_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675287);
		NativeMethodInfoPtr_get_spriteAtlasPath_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675288);
		NativeMethodInfoPtr_set_spriteAtlasPath_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675289);
		NativeMethodInfoPtr_get_spriteChanged_Protected_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675290);
		NativeMethodInfoPtr_set_spriteChanged_Protected_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675291);
		NativeMethodInfoPtr_get_useSpriteFromMod_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675292);
		NativeMethodInfoPtr_get_isDebugMenu_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675293);
		NativeMethodInfoPtr_set_isDebugMenu_Public_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675294);
		NativeMethodInfoPtr_get_soundDragStart_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675295);
		NativeMethodInfoPtr_set_soundDragStart_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675296);
		NativeMethodInfoPtr_get_soundDragEnd_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675297);
		NativeMethodInfoPtr_set_soundDragEnd_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675298);
		NativeMethodInfoPtr_get_soundContainerOpen_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675299);
		NativeMethodInfoPtr_set_soundContainerOpen_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675300);
		NativeMethodInfoPtr_get_soundContainerClose_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675301);
		NativeMethodInfoPtr_set_soundContainerClose_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675302);
		NativeMethodInfoPtr_get_soundSelect_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675303);
		NativeMethodInfoPtr_set_soundSelect_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675304);
		NativeMethodInfoPtr_get_soundExamineOpen_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675305);
		NativeMethodInfoPtr_set_soundExamineOpen_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675306);
		NativeMethodInfoPtr_get_soundToggle_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675307);
		NativeMethodInfoPtr_set_soundToggle_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675308);
		NativeMethodInfoPtr_get_soundActivate_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675309);
		NativeMethodInfoPtr_set_soundActivate_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675310);
		NativeMethodInfoPtr_get_soundUse_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675311);
		NativeMethodInfoPtr_set_soundUse_Public_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675312);
		NativeMethodInfoPtr__ctor_Public_Void_String_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675313);
		NativeMethodInfoPtr_AddClientItemBuyingFeature_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675314);
		NativeMethodInfoPtr_CloneLinked_Public_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675315);
		NativeMethodInfoPtr_SetSprite_Public_GameItem_String_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675316);
		NativeMethodInfoPtr_SetSpriteQuick_Public_GameItem_String_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675317);
		NativeMethodInfoPtr_SetShape_Public_GameItem_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675318);
		NativeMethodInfoPtr_SetSpriteAndShape_Public_GameItem_String_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675319);
		NativeMethodInfoPtr_WarnIfSliceNotGridAligned_Private_Static_Void_String_String_Rect_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675320);
		NativeMethodInfoPtr_GetValue_Public_Int64_List_1_GameCharacterItem_GameCharacterItem_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675321);
		NativeMethodInfoPtr_SetValue_Public_GameItem_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675322);
		NativeMethodInfoPtr_ValueModifier_Private_Static_Int64_GameItem_List_1_GameCharacterItem_GameCharacterItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675323);
		NativeMethodInfoPtr_SetUnitCount_Public_GameItem_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675324);
		NativeMethodInfoPtr_SetAmount_Public_GameItem_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675325);
		NativeMethodInfoPtr_SetContentWindow_Public_GameItem_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675326);
		NativeMethodInfoPtr_SetExamineWindow_Public_GameItem_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675327);
		NativeMethodInfoPtr_AddParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675328);
		NativeMethodInfoPtr_RemoveParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675329);
		NativeMethodInfoPtr_FindAdjacentType_Public_List_1_T_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675330);
		NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Final_New_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675331);
		NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_Final_New_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675332);
		NativeMethodInfoPtr_Destroy_Public_Abstract_Virtual_New_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675333);
		NativeMethodInfoPtr_Validate_Public_Abstract_Virtual_New_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675334);
		NativeMethodInfoPtr_GetDisplayName_Public_String_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675335);
		NativeMethodInfoPtr_IsTag_Public_Boolean_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675336);
		NativeMethodInfoPtr_IsOneOfTags_Public_Boolean_List_1_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675337);
		NativeMethodInfoPtr_GetTagReadonly_Public_TagState_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675338);
		NativeMethodInfoPtr_ModifyTag_Public_GameItem_String_Action_1_TagState_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675339);
		NativeMethodInfoPtr_EnableTag_Public_Boolean_String_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675340);
		NativeMethodInfoPtr_DisableTag_Public_Boolean_String_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675341);
		NativeMethodInfoPtr_EnableTag_Public_Boolean_T_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675342);
		NativeMethodInfoPtr_SetName_Public_GameItem_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675343);
		NativeMethodInfoPtr_MayToggleSlotItem_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675344);
		NativeMethodInfoPtr_CanToggleSlotItem_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675345);
		NativeMethodInfoPtr_ToggleSlotItem_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675346);
		NativeMethodInfoPtr_MayActivateSlotItem_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675347);
		NativeMethodInfoPtr_CanActivateSlotItem_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675348);
		NativeMethodInfoPtr_ActivateSlotItem_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675349);
		NativeMethodInfoPtr_MaySelectSlotItem_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675350);
		NativeMethodInfoPtr_CanSelectSlotItem_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675351);
		NativeMethodInfoPtr_MayTarget_Public_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675352);
		NativeMethodInfoPtr_CanTarget_Public_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675353);
		NativeMethodInfoPtr_Target_Public_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675354);
		NativeMethodInfoPtr_MayParentShapeModifier_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675355);
		NativeMethodInfoPtr_GetParentShapeModifier_Public_ValueTuple_2_GridShape_GridShape_GameItem_SlotMarker_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675356);
		NativeMethodInfoPtr_GetParentStateModifier_Public_ValueTuple_2_List_1_ItemStateModifier_List_1_ItemStateModifier_GameItem_SlotMarker_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675357);
		NativeMethodInfoPtr_MayRemove_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675358);
		NativeMethodInfoPtr_MaxNumRemove_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675359);
		NativeMethodInfoPtr_MaxNumAdd_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675360);
		NativeMethodInfoPtr_MaxNumAdd_Public_Int32_SlotMarker_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675361);
		NativeMethodInfoPtr_MayStack_Public_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675362);
		NativeMethodInfoPtr_GetStackSlot_Public_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675363);
		NativeMethodInfoPtr_GetIncompleteSlotInInventory_Public_SlotMarker_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675364);
		NativeMethodInfoPtr_GetSplitItem_Public_GameItem_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675365);
		NativeMethodInfoPtr_CheckIsStateValid_Public_Boolean_GameInventory_SlotMarker_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675366);
		NativeMethodInfoPtr_CheckValidShapeState_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675367);
		NativeMethodInfoPtr_SyncModifiedState_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675368);
		NativeMethodInfoPtr_ValidateShapeState_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675369);
		NativeMethodInfoPtr_ComputeModifiedShapeState_Public_ValueTuple_4_GridShape_Int32_Int32_TagSystem_GridShape_GridShape_List_1_ItemStateModifier_List_1_ItemStateModifier_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675370);
		NativeMethodInfoPtr_TryAcceptAll_Public_GameItem_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675371);
		NativeMethodInfoPtr_TryUnloadAll_Public_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675372);
		NativeMethodInfoPtr_FindAllChildItems_Public_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675373);
		NativeMethodInfoPtr_FindAllRecipes_Public_List_1_CraftingRecipe_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675374);
		NativeMethodInfoPtr_get_bonusAccuracy_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675375);
		NativeMethodInfoPtr_set_bonusAccuracy_Public_Virtual_Final_New_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675376);
		NativeMethodInfoPtr_get_effects_Public_Virtual_Final_New_get_List_1_CombatAbilityEffect_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675377);
		NativeMethodInfoPtr_set_effects_Public_Virtual_Final_New_set_Void_List_1_CombatAbilityEffect_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675378);
		NativeMethodInfoPtr_get_triggerOverwatch_Public_Virtual_Final_New_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675379);
		NativeMethodInfoPtr_set_triggerOverwatch_Public_Virtual_Final_New_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675380);
		NativeMethodInfoPtr_get_canUseOutsideCombat_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675381);
		NativeMethodInfoPtr_set_canUseOutsideCombat_Public_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675382);
		NativeMethodInfoPtr_GetPrimaryTarget_Public_Virtual_Final_New_TargetType_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675383);
		NativeMethodInfoPtr_IsAlwaysAccurate_Public_Virtual_Final_New_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675384);
		NativeMethodInfoPtr_AddAmenities_Public_Void_String_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675385);
		NativeMethodInfoPtr_EnableProficiency_Public_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675386);
		NativeMethodInfoPtr_EnableManufacturer_Public_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675387);
		NativeMethodInfoPtr_DisplayManufacturer_Public_Void_RichTextBuilder_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675388);
		NativeMethodInfoPtr_SetType_Public_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675389);
		NativeMethodInfoPtr_AccumulateFeatureStages_Private_Void_byref_Int64_byref_Int64_byref_Double_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675390);
		NativeMethodInfoPtr_ComposeStagedValue_Private_Static_Int64_Int64_Int64_Int64_Double_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675391);
		NativeMethodInfoPtr_GetNegociatedValue_Public_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675392);
		NativeMethodInfoPtr_GetCurrentValue_Public_Int64_Boolean_Boolean_Boolean_Boolean_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675393);
		NativeMethodInfoPtr_CheckValueSafe_Public_Static_Int64_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675394);
		NativeMethodInfoPtr_GetComplexeItemValue_Public_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675395);
		NativeMethodInfoPtr_GetRefreshedValue_Public_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675396);
		NativeMethodInfoPtr_GetBaseValue_Public_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675397);
		NativeMethodInfoPtr_GetItemMiscValue_Public_Int32_Boolean_Boolean_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675398);
		NativeMethodInfoPtr_IsApplyMarkUp_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675399);
		NativeMethodInfoPtr_GetCurrentChildValue_Public_Int32_Boolean_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675400);
		NativeMethodInfoPtr_GetInheritableClientMarketMod_Public_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675401);
		NativeMethodInfoPtr_IsAlreadyContainFeatureWithId_Public_Boolean_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675402);
		NativeMethodInfoPtr_FindItemFeatureByID_Public_ItemFeature_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675403);
		NativeMethodInfoPtr_RemoveItemFeatureByID_Public_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675404);
		NativeMethodInfoPtr_FindItemFeatureByCategory_Public_ItemFeature_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675405);
		NativeMethodInfoPtr_GetIndexByCategory_Public_Int32_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675406);
		NativeMethodInfoPtr_OnAddedToClientArea_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675407);
		NativeMethodInfoPtr_OnAddedToShowcaseArea_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675408);
		NativeMethodInfoPtr_OnAddedToWeightedArea_Public_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675409);
		NativeMethodInfoPtr_OnRemovedFromWeightedArea_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675410);
		NativeMethodInfoPtr_AddFeatureRelatedToBuyer_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675411);
		NativeMethodInfoPtr_RemoveFeatureRelatedToBuyer_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675412);
		NativeMethodInfoPtr_RemoveFeatureRelatedToEvent_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675413);
		NativeMethodInfoPtr_RemoveFeatureTemporaryBuying_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675414);
		NativeMethodInfoPtr_RemoveFeatureTemporarySelling_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675415);
		NativeMethodInfoPtr_RemoveFeatureByCategory_Public_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675416);
		NativeMethodInfoPtr_AddFeatureRelatedToEvent_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675417);
		NativeMethodInfoPtr_SetGameItemType_Public_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675418);
		NativeMethodInfoPtr_IsGameItemType_Public_Boolean_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675419);
		NativeMethodInfoPtr_RemoveGameItemType_Public_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675420);
		NativeMethodInfoPtr_RemoveAllGameItemType_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675421);
		NativeMethodInfoPtr_TransferGameItemTypeFrom_Public_Void_GameItem_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675422);
		NativeMethodInfoPtr_GetGameItemType_Public_List_1_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675423);
		NativeMethodInfoPtr_AddItemFeature_Public_Void_ItemFeature_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675424);
		NativeMethodInfoPtr_RemoveTemporaryClientFeature_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675425);
		NativeMethodInfoPtr_CloseContentWindow_Public_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675426);
		NativeMethodInfoPtr_RememberContentWindowPositionIfMine_Public_Void_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675427);
		NativeMethodInfoPtr_RestoreContentWindowPosition_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675428);
		NativeMethodInfoPtr_IsRootedInBackInv_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675429);
		NativeMethodInfoPtr_IsRootedInSmugglerBay_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675430);
		NativeMethodInfoPtr_GetParentWithIdentifier_Public_GameInventory_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675431);
		NativeMethodInfoPtr_FindParentWithIdentifier_Public_Static_GameInventory_GraphNodeStorage_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675432);
		NativeMethodInfoPtr_GetUniqueID_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675433);
		NativeMethodInfoPtr_RemoveAllFeature_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675434);
		NativeMethodInfoPtr_IsConditionActive_Public_Boolean_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675435);
		NativeMethodInfoPtr_IsID_Public_Boolean_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675436);
		NativeMethodInfoPtr_GetItemSize_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675437);
		NativeMethodInfoPtr_GetModifiedItemSize_Public_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675438);
		NativeMethodInfoPtr___ctor_b__177_0_Private_Void_GameItem_GameInventory_SlotMarker_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675440);
		NativeMethodInfoPtr__TryUnloadAll_b__250_0_Private_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItem>.NativeClassPtr, 100675441);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 353942, RefRangeEnd = 353943, XrefRangeStart = 353938, XrefRangeEnd = 353942, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static GameItem Create(string spriteAtlasPath, string spritePath, PixelWindow window = null)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(spriteAtlasPath);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(spritePath);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)window);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Create_Public_Static_GameItem_String_String_PixelWindow_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 354112, RefRangeEnd = 354113, XrefRangeStart = 354033, XrefRangeEnd = 354112, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem(string identifier, PixelWindow window = null)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameItem>.NativeClassPtr))
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(identifier);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)window);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_String_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 354145, RefRangeEnd = 354146, XrefRangeStart = 354113, XrefRangeEnd = 354145, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AddClientItemBuyingFeature()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AddClientItemBuyingFeature_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 354224, RefRangeEnd = 354225, XrefRangeStart = 354146, XrefRangeEnd = 354224, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem CloneLinked()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CloneLinked_Public_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 354227, RefRangeEnd = 354230, XrefRangeStart = 354225, XrefRangeEnd = 354227, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetSprite(string spriteAtlasPath, string spritePath)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(spriteAtlasPath);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(spritePath);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetSprite_Public_GameItem_String_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 354237, RefRangeEnd = 354240, XrefRangeStart = 354230, XrefRangeEnd = 354237, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetSpriteQuick(string spriteAtlasPath, string spritePath)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(spriteAtlasPath);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(spritePath);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetSpriteQuick_Public_GameItem_String_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 354245, RefRangeEnd = 354246, XrefRangeStart = 354240, XrefRangeEnd = 354245, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetShape(GridShape itemShape)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemShape);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetShape_Public_GameItem_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(795)]
	[CachedScanResults(RefRangeStart = 354285, RefRangeEnd = 355080, XrefRangeStart = 354246, XrefRangeEnd = 354285, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetSpriteAndShape(string spriteAtlasPath, string spritePath)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(spriteAtlasPath);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(spritePath);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetSpriteAndShape_Public_GameItem_String_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 355160, RefRangeEnd = 355161, XrefRangeStart = 355080, XrefRangeEnd = 355160, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void WarnIfSliceNotGridAligned(string spriteAtlasPath, string spritePath, Rect r, int gridSize)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(spriteAtlasPath);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(spritePath);
		*(Rect**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &r;
		*(int**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &gridSize;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_WarnIfSliceNotGridAligned_Private_Static_Void_String_String_Rect_Int32_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 355165, RefRangeEnd = 355167, XrefRangeStart = 355161, XrefRangeEnd = 355165, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe long GetValue(List<GameCharacterItem> party = null, GameCharacterItem vendor = null, bool singleValue = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)party);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)vendor);
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &singleValue;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetValue_Public_Int64_List_1_GameCharacterItem_GameCharacterItem_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(68)]
	[CachedScanResults(RefRangeStart = 355167, RefRangeEnd = 355235, XrefRangeStart = 355167, XrefRangeEnd = 355167, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetValue(long num)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&num);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetValue_Public_GameItem_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 355263, RefRangeEnd = 355266, XrefRangeStart = 355235, XrefRangeEnd = 355263, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static long ValueModifier(GameItem item, List<GameCharacterItem> party, GameCharacterItem vendor)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)party);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)vendor);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ValueModifier_Private_Static_Int64_GameItem_List_1_GameCharacterItem_GameCharacterItem_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(109)]
	[CachedScanResults(RefRangeStart = 355266, RefRangeEnd = 355375, XrefRangeStart = 355266, XrefRangeEnd = 355266, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetUnitCount(int num)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&num);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetUnitCount_Public_GameItem_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(109)]
	[CachedScanResults(RefRangeStart = 355266, RefRangeEnd = 355375, XrefRangeStart = 355266, XrefRangeEnd = 355375, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetAmount(int num)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&num);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetAmount_Public_GameItem_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(187)]
	[CachedScanResults(RefRangeStart = 355378, RefRangeEnd = 355565, XrefRangeStart = 355375, XrefRangeEnd = 355378, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetContentWindow(PixelWindow window)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)window);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetContentWindow_Public_GameItem_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 355568, RefRangeEnd = 355569, XrefRangeStart = 355565, XrefRangeEnd = 355568, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetExamineWindow(PixelWindow window)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)window);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetExamineWindow_Public_GameItem_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 355572, RefRangeEnd = 355575, XrefRangeStart = 355569, XrefRangeEnd = 355572, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool AddParent(GraphNodeStorage node)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)node);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AddParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 355576, RefRangeEnd = 355580, XrefRangeStart = 355575, XrefRangeEnd = 355576, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool RemoveParent(GraphNodeStorage node)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)node);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(7)]
	[CachedScanResults(RefRangeStart = 355604, RefRangeEnd = 355611, XrefRangeStart = 355580, XrefRangeEnd = 355604, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe List<T> FindAdjacentType<T>()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(MethodInfoStoreGeneric_FindAdjacentType_Public_List_1_T_0<T>.Pointer, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<T>>(intPtr2) : null;
	}

	[CallerCount(7)]
	[CachedScanResults(RefRangeStart = 355615, RefRangeEnd = 355622, XrefRangeStart = 355611, XrefRangeEnd = 355615, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool MayHaveValidInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Final_New_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(30)]
	[CachedScanResults(RefRangeStart = 355635, RefRangeEnd = 355665, XrefRangeStart = 355622, XrefRangeEnd = 355635, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_Final_New_SlotMarker_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe virtual void Destroy()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Destroy_Public_Abstract_Virtual_New_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	public unsafe virtual void Validate()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Validate_Public_Abstract_Virtual_New_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(38)]
	[CachedScanResults(RefRangeStart = 355674, RefRangeEnd = 355712, XrefRangeStart = 355665, XrefRangeEnd = 355674, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe string GetDisplayName(bool preferCustomName = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&preferCustomName);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetDisplayName_Public_String_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return IL2CPP.Il2CppStringToManaged(intPtr2);
	}

	[CallerCount(1029)]
	[CachedScanResults(RefRangeStart = 355714, RefRangeEnd = 356743, XrefRangeStart = 355712, XrefRangeEnd = 355714, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsTag(string tagName)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(tagName);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsTag_Public_Boolean_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 356754, RefRangeEnd = 356755, XrefRangeStart = 356743, XrefRangeEnd = 356754, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsOneOfTags(List<string> tags)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tags);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsOneOfTags_Public_Boolean_List_1_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1023)]
	[CachedScanResults(RefRangeStart = 356758, RefRangeEnd = 357781, XrefRangeStart = 356755, XrefRangeEnd = 356758, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe TagState GetTagReadonly(string tagName)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(tagName);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetTagReadonly_Public_TagState_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TagState>(intPtr2) : null;
	}

	[CallerCount(895)]
	[CachedScanResults(RefRangeStart = 357783, RefRangeEnd = 358678, XrefRangeStart = 357781, XrefRangeEnd = 357783, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem ModifyTag(string tagName, Il2CppSystem.Action<TagState> action, bool skipValidate = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(tagName);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)action);
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &skipValidate;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ModifyTag_Public_GameItem_String_Action_1_TagState_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(574)]
	[CachedScanResults(RefRangeStart = 358679, RefRangeEnd = 359253, XrefRangeStart = 358678, XrefRangeEnd = 358679, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool EnableTag(string tagName, bool skipValidate = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(tagName);
		*(bool**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &skipValidate;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_EnableTag_Public_Boolean_String_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(67)]
	[CachedScanResults(RefRangeStart = 359254, RefRangeEnd = 359321, XrefRangeStart = 359253, XrefRangeEnd = 359254, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool DisableTag(string tagName, bool skipValidate = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(tagName);
		*(bool**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &skipValidate;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_DisableTag_Public_Boolean_String_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 359321, RefRangeEnd = 359323, XrefRangeStart = 359321, XrefRangeEnd = 359321, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool EnableTag<T>(T tagName) where T : System.Enum
	{
		//IL_0061->IL0064: Incompatible stack types: I vs Ref
		//IL_0040->IL0064: Incompatible stack types: I vs Ref
		//IL_004d->IL0064: Incompatible stack types: I vs Ref
		//IL_0054->IL0064: Incompatible stack types: I vs Ref
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		ref T reference;
		if (!typeof(T).IsValueType)
		{
			object obj = tagName;
			if (obj is string)
			{
				reference = ref *(?*)IL2CPP.ManagedStringToIl2Cpp(obj as string);
			}
			else
			{
				System.IntPtr intPtr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)((obj is Il2CppObjectBase) ? obj : null));
				reference = ref *(?*)intPtr;
				if (intPtr != (System.IntPtr)0)
				{
					reference = ref *(?*)intPtr;
					if (IL2CPP.il2cpp_class_is_valuetype(IL2CPP.il2cpp_object_get_class(intPtr)))
					{
						reference = ref *(?*)IL2CPP.il2cpp_object_unbox(intPtr);
					}
				}
			}
		}
		else
		{
			reference = ref tagName;
		}
		*ptr = (nint)Unsafe.AsPointer(ref reference);
		Unsafe.SkipInit(out System.IntPtr intPtr2);
		System.IntPtr intPtr3 = IL2CPP.il2cpp_runtime_invoke(MethodInfoStoreGeneric_EnableTag_Public_Boolean_T_0<T>.Pointer, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr2);
		Il2CppException.RaiseExceptionIfNecessary(intPtr2);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr3);
	}

	[CallerCount(79)]
	[CachedScanResults(RefRangeStart = 359323, RefRangeEnd = 359402, XrefRangeStart = 359323, XrefRangeEnd = 359323, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem SetName(string name)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(name);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetName_Public_GameItem_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 359429, RefRangeEnd = 359430, XrefRangeStart = 359402, XrefRangeEnd = 359429, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool MayToggleSlotItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MayToggleSlotItem_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 359430, XrefRangeEnd = 359457, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CanToggleSlotItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CanToggleSlotItem_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 359469, RefRangeEnd = 359470, XrefRangeStart = 359457, XrefRangeEnd = 359469, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ToggleSlotItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ToggleSlotItem_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(9)]
	[CachedScanResults(RefRangeStart = 359504, RefRangeEnd = 359513, XrefRangeStart = 359470, XrefRangeEnd = 359504, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool MayActivateSlotItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MayActivateSlotItem_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(9)]
	[CachedScanResults(RefRangeStart = 359547, RefRangeEnd = 359556, XrefRangeStart = 359513, XrefRangeEnd = 359547, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CanActivateSlotItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CanActivateSlotItem_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(7)]
	[CachedScanResults(RefRangeStart = 359568, RefRangeEnd = 359575, XrefRangeStart = 359556, XrefRangeEnd = 359568, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ActivateSlotItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ActivateSlotItem_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(6)]
	[CachedScanResults(RefRangeStart = 359609, RefRangeEnd = 359615, XrefRangeStart = 359575, XrefRangeEnd = 359609, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool MaySelectSlotItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MaySelectSlotItem_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(6)]
	[CachedScanResults(RefRangeStart = 359646, RefRangeEnd = 359652, XrefRangeStart = 359615, XrefRangeEnd = 359646, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CanSelectSlotItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CanSelectSlotItem_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(12)]
	[CachedScanResults(RefRangeStart = 359751, RefRangeEnd = 359763, XrefRangeStart = 359652, XrefRangeEnd = 359751, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool MayTarget(GameItem targetItem)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)targetItem);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MayTarget_Public_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(8)]
	[CachedScanResults(RefRangeStart = 359861, RefRangeEnd = 359869, XrefRangeStart = 359763, XrefRangeEnd = 359861, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CanTarget(GameItem targetItem)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)targetItem);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CanTarget_Public_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 359885, RefRangeEnd = 359887, XrefRangeStart = 359869, XrefRangeEnd = 359885, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void Target(GameItem targetItem)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)targetItem);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Target_Public_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 359887, RefRangeEnd = 359888, XrefRangeStart = 359887, XrefRangeEnd = 359887, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool MayParentShapeModifier()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MayParentShapeModifier_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 359892, RefRangeEnd = 359895, XrefRangeStart = 359888, XrefRangeEnd = 359892, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Il2CppSystem.ValueTuple<GridShape, GridShape> GetParentShapeModifier(GameItem newParentItem, SlotMarker newSlot, GridShape newModifiedShape, TagSystem newState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newParentItem);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newSlot);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newModifiedShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetParentShapeModifier_Public_ValueTuple_2_GridShape_GridShape_GameItem_SlotMarker_GridShape_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return new Il2CppSystem.ValueTuple<GridShape, GridShape>(pointer);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 359899, RefRangeEnd = 359902, XrefRangeStart = 359895, XrefRangeEnd = 359899, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>> GetParentStateModifier(GameItem newParentItem, SlotMarker newSlot, GridShape newModifiedShape, TagSystem newState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newParentItem);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newSlot);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newModifiedShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetParentStateModifier_Public_ValueTuple_2_List_1_ItemStateModifier_List_1_ItemStateModifier_GameItem_SlotMarker_GridShape_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return new Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>>(pointer);
	}

	[CallerCount(11)]
	[CachedScanResults(RefRangeStart = 359933, RefRangeEnd = 359944, XrefRangeStart = 359902, XrefRangeEnd = 359933, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool MayRemove()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MayRemove_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(18)]
	[CachedScanResults(RefRangeStart = 359981, RefRangeEnd = 359999, XrefRangeStart = 359944, XrefRangeEnd = 359981, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int MaxNumRemove()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MaxNumRemove_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 360001, RefRangeEnd = 360005, XrefRangeStart = 359999, XrefRangeEnd = 360001, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int MaxNumAdd()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MaxNumAdd_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(10)]
	[CachedScanResults(RefRangeStart = 360042, RefRangeEnd = 360052, XrefRangeStart = 360005, XrefRangeEnd = 360042, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int MaxNumAdd(SlotMarker slot)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MaxNumAdd_Public_Int32_SlotMarker_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(15)]
	[CachedScanResults(RefRangeStart = 360068, RefRangeEnd = 360083, XrefRangeStart = 360052, XrefRangeEnd = 360068, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool MayStack(GameItem otherItem)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)otherItem);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MayStack_Public_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 360104, RefRangeEnd = 360107, XrefRangeStart = 360083, XrefRangeEnd = 360104, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe SlotMarker GetStackSlot(GameItem itemToBeStacked)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemToBeStacked);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetStackSlot_Public_SlotMarker_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(23)]
	[CachedScanResults(RefRangeStart = 360107, RefRangeEnd = 360130, XrefRangeStart = 360107, XrefRangeEnd = 360107, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe SlotMarker GetIncompleteSlotInInventory()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetIncompleteSlotInInventory_Public_SlotMarker_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 360136, RefRangeEnd = 360137, XrefRangeStart = 360130, XrefRangeEnd = 360136, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem GetSplitItem(int requestedAmount)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&requestedAmount);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetSplitItem_Public_GameItem_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(10)]
	[CachedScanResults(RefRangeStart = 360147, RefRangeEnd = 360157, XrefRangeStart = 360137, XrefRangeEnd = 360147, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CheckIsStateValid(GameInventory parentInventory, SlotMarker slot, GridShape tempShape, TagSystem itemState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)parentInventory);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CheckIsStateValid_Public_Boolean_GameInventory_SlotMarker_GridShape_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 360158, RefRangeEnd = 360159, XrefRangeStart = 360157, XrefRangeEnd = 360158, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CheckValidShapeState()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CheckValidShapeState_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 360162, RefRangeEnd = 360163, XrefRangeStart = 360159, XrefRangeEnd = 360162, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SyncModifiedState()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SyncModifiedState_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(29)]
	[CachedScanResults(RefRangeStart = 360194, RefRangeEnd = 360223, XrefRangeStart = 360163, XrefRangeEnd = 360194, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ValidateShapeState()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ValidateShapeState_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(6)]
	[CachedScanResults(RefRangeStart = 360346, RefRangeEnd = 360352, XrefRangeStart = 360223, XrefRangeEnd = 360346, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Il2CppSystem.ValueTuple<GridShape, int, int, TagSystem> ComputeModifiedShapeState(GridShape addModItem, GridShape addModInv, List<ItemStateModifier> addStateItem, List<ItemStateModifier> addStateInv, GameItem ignoreItem)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[5];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)addModItem);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)addModInv);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)addStateItem);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)addStateInv);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)ignoreItem);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ComputeModifiedShapeState_Public_ValueTuple_4_GridShape_Int32_Int32_TagSystem_GridShape_GridShape_List_1_ItemStateModifier_List_1_ItemStateModifier_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return new Il2CppSystem.ValueTuple<GridShape, int, int, TagSystem>(pointer);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 360353, RefRangeEnd = 360354, XrefRangeStart = 360352, XrefRangeEnd = 360353, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem TryAcceptAll(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TryAcceptAll_Public_GameItem_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 360366, RefRangeEnd = 360369, XrefRangeStart = 360354, XrefRangeEnd = 360366, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem TryUnloadAll()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TryUnloadAll_Public_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 360372, RefRangeEnd = 360375, XrefRangeStart = 360369, XrefRangeEnd = 360372, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe List<GameItem> FindAllChildItems()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FindAllChildItems_Public_List_1_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GameItem>>(intPtr2) : null;
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 360403, RefRangeEnd = 360405, XrefRangeStart = 360375, XrefRangeEnd = 360403, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe List<CraftingRecipe> FindAllRecipes()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FindAllRecipes_Public_List_1_CraftingRecipe_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<CraftingRecipe>>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 360406, XrefRangeEnd = 360410, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual CombatAbilityEffect.TargetType GetPrimaryTarget()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetPrimaryTarget_Public_Virtual_Final_New_TargetType_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(CombatAbilityEffect.TargetType*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	public unsafe virtual bool IsAlwaysAccurate()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsAlwaysAccurate_Public_Virtual_Final_New_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(22)]
	[CachedScanResults(RefRangeStart = 360453, RefRangeEnd = 360475, XrefRangeStart = 360410, XrefRangeEnd = 360453, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AddAmenities(string type, int value, int use)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(type);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &value;
		*(int**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &use;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AddAmenities_Public_Void_String_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(20)]
	[CachedScanResults(RefRangeStart = 360494, RefRangeEnd = 360514, XrefRangeStart = 360475, XrefRangeEnd = 360494, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void EnableProficiency(string proficiencyType)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(proficiencyType);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_EnableProficiency_Public_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(163)]
	[CachedScanResults(RefRangeStart = 360533, RefRangeEnd = 360696, XrefRangeStart = 360514, XrefRangeEnd = 360533, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void EnableManufacturer(string manufacturer)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(manufacturer);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_EnableManufacturer_Public_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 360792, RefRangeEnd = 360793, XrefRangeStart = 360696, XrefRangeEnd = 360792, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void DisplayManufacturer(RichTextBuilder builder, string manufacturer)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)builder);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(manufacturer);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_DisplayManufacturer_Public_Void_RichTextBuilder_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(43)]
	[CachedScanResults(RefRangeStart = 360817, RefRangeEnd = 360860, XrefRangeStart = 360793, XrefRangeEnd = 360817, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SetType(string type)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(type);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetType_Public_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 360884, RefRangeEnd = 360887, XrefRangeStart = 360860, XrefRangeEnd = 360884, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AccumulateFeatureStages(ref long innateMult, ref long marketMult, ref double finalFactor, bool skipEventFeatures = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = (nint)Unsafe.AsPointer(ref innateMult);
		*(void**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = Unsafe.AsPointer(ref marketMult);
		*(void**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = Unsafe.AsPointer(ref finalFactor);
		*(bool**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &skipEventFeatures;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AccumulateFeatureStages_Private_Void_byref_Int64_byref_Int64_byref_Double_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 360898, RefRangeEnd = 360901, XrefRangeStart = 360887, XrefRangeEnd = 360898, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static long ComposeStagedValue(long totalBaseValue, long innateMult, long marketMult, double finalFactor, int miscValue, int marketScaledValue = 0)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[6];
		*ptr = (nint)(&totalBaseValue);
		*(long**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &innateMult;
		*(long**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &marketMult;
		*(double**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &finalFactor;
		*(int**)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = &miscValue;
		*(int**)((byte*)ptr + checked((nuint)5u * unchecked((nuint)sizeof(System.IntPtr)))) = &marketScaledValue;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ComposeStagedValue_Private_Static_Int64_Int64_Int64_Int64_Double_Int32_Int32_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(34)]
	[CachedScanResults(RefRangeStart = 360995, RefRangeEnd = 361029, XrefRangeStart = 360901, XrefRangeEnd = 360995, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe long GetNegociatedValue()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetNegociatedValue_Public_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(17)]
	[CachedScanResults(RefRangeStart = 361181, RefRangeEnd = 361198, XrefRangeStart = 361029, XrefRangeEnd = 361181, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe long GetCurrentValue(bool useRetailMarkup = false, bool withChild = true, bool includeEvents = true, bool forceMarkup = false, long extraMarketMod = 0L)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[5];
		*ptr = (nint)(&useRetailMarkup);
		*(bool**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &withChild;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &includeEvents;
		*(bool**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &forceMarkup;
		*(long**)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = &extraMarketMod;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetCurrentValue_Public_Int64_Boolean_Boolean_Boolean_Boolean_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	public unsafe static long CheckValueSafe(long finalValue)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&finalValue);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CheckValueSafe_Public_Static_Int64_Int64_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 361198, XrefRangeEnd = 361199, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe long GetComplexeItemValue()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetComplexeItemValue_Public_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 361255, RefRangeEnd = 361256, XrefRangeStart = 361199, XrefRangeEnd = 361255, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe long GetRefreshedValue()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetRefreshedValue_Public_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(6)]
	[CachedScanResults(RefRangeStart = 361268, RefRangeEnd = 361274, XrefRangeStart = 361256, XrefRangeEnd = 361268, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe long GetBaseValue()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetBaseValue_Public_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 361278, RefRangeEnd = 361279, XrefRangeStart = 361274, XrefRangeEnd = 361278, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int GetItemMiscValue(bool useMarkup = false, bool withChild = true, bool includeEvents = true)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = (nint)(&useMarkup);
		*(bool**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &withChild;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &includeEvents;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetItemMiscValue_Public_Int32_Boolean_Boolean_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 361279, XrefRangeEnd = 361289, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsApplyMarkUp()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsApplyMarkUp_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 361312, RefRangeEnd = 361316, XrefRangeStart = 361289, XrefRangeEnd = 361312, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int GetCurrentChildValue(bool useMarkup = false, bool includeEvents = true)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&useMarkup);
		*(bool**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &includeEvents;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetCurrentChildValue_Public_Int32_Boolean_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 361335, RefRangeEnd = 361336, XrefRangeStart = 361316, XrefRangeEnd = 361335, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe long GetInheritableClientMarketMod()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetInheritableClientMarketMod_Public_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 361347, RefRangeEnd = 361352, XrefRangeStart = 361336, XrefRangeEnd = 361347, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsAlreadyContainFeatureWithId(string identifier)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(identifier);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsAlreadyContainFeatureWithId_Public_Boolean_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(22)]
	[CachedScanResults(RefRangeStart = 361363, RefRangeEnd = 361385, XrefRangeStart = 361352, XrefRangeEnd = 361363, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe ItemFeature FindItemFeatureByID(string id)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(id);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FindItemFeatureByID_Public_ItemFeature_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<ItemFeature>(intPtr2) : null;
	}

	[CallerCount(11)]
	[CachedScanResults(RefRangeStart = 361408, RefRangeEnd = 361419, XrefRangeStart = 361385, XrefRangeEnd = 361408, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveItemFeatureByID(string id)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(id);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveItemFeatureByID_Public_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(33)]
	[CachedScanResults(RefRangeStart = 361430, RefRangeEnd = 361463, XrefRangeStart = 361419, XrefRangeEnd = 361430, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe ItemFeature FindItemFeatureByCategory(string category)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(category);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FindItemFeatureByCategory_Public_ItemFeature_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<ItemFeature>(intPtr2) : null;
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 361469, RefRangeEnd = 361470, XrefRangeStart = 361463, XrefRangeEnd = 361469, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int GetIndexByCategory(string category)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(category);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetIndexByCategory_Public_Int32_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 361504, RefRangeEnd = 361505, XrefRangeStart = 361470, XrefRangeEnd = 361504, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void OnAddedToClientArea()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_OnAddedToClientArea_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 361512, RefRangeEnd = 361513, XrefRangeStart = 361505, XrefRangeEnd = 361512, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void OnAddedToShowcaseArea()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_OnAddedToShowcaseArea_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 361592, RefRangeEnd = 361595, XrefRangeStart = 361513, XrefRangeEnd = 361592, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void OnAddedToWeightedArea(bool ignoreOnItemAddedToWeighted = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&ignoreOnItemAddedToWeighted);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_OnAddedToWeightedArea_Public_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 361640, RefRangeEnd = 361641, XrefRangeStart = 361595, XrefRangeEnd = 361640, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void OnRemovedFromWeightedArea()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_OnRemovedFromWeightedArea_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(10210)]
	[CachedScanResults(RefRangeStart = 18631, RefRangeEnd = 28841, XrefRangeStart = 18631, XrefRangeEnd = 28841, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AddFeatureRelatedToBuyer()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AddFeatureRelatedToBuyer_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(10210)]
	[CachedScanResults(RefRangeStart = 18631, RefRangeEnd = 28841, XrefRangeStart = 18631, XrefRangeEnd = 28841, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveFeatureRelatedToBuyer()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveFeatureRelatedToBuyer_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(7)]
	[CachedScanResults(RefRangeStart = 361648, RefRangeEnd = 361655, XrefRangeStart = 361641, XrefRangeEnd = 361648, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveFeatureRelatedToEvent()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveFeatureRelatedToEvent_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 361662, RefRangeEnd = 361664, XrefRangeStart = 361655, XrefRangeEnd = 361662, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveFeatureTemporaryBuying()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveFeatureTemporaryBuying_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 361671, RefRangeEnd = 361672, XrefRangeStart = 361664, XrefRangeEnd = 361671, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveFeatureTemporarySelling()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveFeatureTemporarySelling_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 361680, RefRangeEnd = 361682, XrefRangeStart = 361672, XrefRangeEnd = 361680, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveFeatureByCategory(string category)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(category);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveFeatureByCategory_Public_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 361706, RefRangeEnd = 361710, XrefRangeStart = 361682, XrefRangeEnd = 361706, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AddFeatureRelatedToEvent()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AddFeatureRelatedToEvent_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(557)]
	[CachedScanResults(RefRangeStart = 361719, RefRangeEnd = 362276, XrefRangeStart = 361710, XrefRangeEnd = 361719, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SetGameItemType(string gameItemType)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(gameItemType);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetGameItemType_Public_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(77)]
	[CachedScanResults(RefRangeStart = 362280, RefRangeEnd = 362357, XrefRangeStart = 362276, XrefRangeEnd = 362280, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsGameItemType(string gameItemType)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(gameItemType);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsGameItemType_Public_Boolean_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(18)]
	[CachedScanResults(RefRangeStart = 362366, RefRangeEnd = 362384, XrefRangeStart = 362357, XrefRangeEnd = 362366, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveGameItemType(string gameItemType)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(gameItemType);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveGameItemType_Public_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 362410, RefRangeEnd = 362413, XrefRangeStart = 362384, XrefRangeEnd = 362410, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveAllGameItemType()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveAllGameItemType_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 362437, RefRangeEnd = 362438, XrefRangeStart = 362413, XrefRangeEnd = 362437, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void TransferGameItemTypeFrom(GameItem sourceItem, bool overwrite = true)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)sourceItem);
		*(bool**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &overwrite;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TransferGameItemTypeFrom_Public_Void_GameItem_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	public unsafe List<string> GetGameItemType()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetGameItemType_Public_List_1_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<string>>(intPtr2) : null;
	}

	[CallerCount(67)]
	[CachedScanResults(RefRangeStart = 362460, RefRangeEnd = 362527, XrefRangeStart = 362438, XrefRangeEnd = 362460, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void AddItemFeature(ItemFeature itemFeature)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemFeature);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AddItemFeature_Public_Void_ItemFeature_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 362546, RefRangeEnd = 362549, XrefRangeStart = 362527, XrefRangeEnd = 362546, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveTemporaryClientFeature()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveTemporaryClientFeature_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(8)]
	[CachedScanResults(RefRangeStart = 362568, RefRangeEnd = 362576, XrefRangeStart = 362549, XrefRangeEnd = 362568, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void CloseContentWindow(bool includeChildren = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&includeChildren);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CloseContentWindow_Public_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 362600, RefRangeEnd = 362602, XrefRangeStart = 362576, XrefRangeEnd = 362600, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RememberContentWindowPositionIfMine(PixelWindow window)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)window);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RememberContentWindowPositionIfMine_Public_Void_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362602, XrefRangeEnd = 362616, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RestoreContentWindowPosition()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RestoreContentWindowPosition_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(8)]
	[CachedScanResults(RefRangeStart = 362622, RefRangeEnd = 362630, XrefRangeStart = 362616, XrefRangeEnd = 362622, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsRootedInBackInv()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsRootedInBackInv_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 362636, RefRangeEnd = 362637, XrefRangeStart = 362630, XrefRangeEnd = 362636, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsRootedInSmugglerBay()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsRootedInSmugglerBay_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362637, XrefRangeEnd = 362641, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameInventory GetParentWithIdentifier(string targetIdentifier)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(targetIdentifier);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetParentWithIdentifier_Public_GameInventory_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameInventory>(intPtr2) : null;
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 362678, RefRangeEnd = 362682, XrefRangeStart = 362641, XrefRangeEnd = 362678, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static GameInventory FindParentWithIdentifier(GraphNodeStorage node, string targetIdentifier)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)node);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(targetIdentifier);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FindParentWithIdentifier_Public_Static_GameInventory_GraphNodeStorage_String_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameInventory>(intPtr2) : null;
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 362687, RefRangeEnd = 362691, XrefRangeStart = 362682, XrefRangeEnd = 362687, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int GetUniqueID()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetUniqueID_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 362711, RefRangeEnd = 362712, XrefRangeStart = 362691, XrefRangeEnd = 362711, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RemoveAllFeature()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RemoveAllFeature_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 362727, RefRangeEnd = 362728, XrefRangeStart = 362712, XrefRangeEnd = 362727, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsConditionActive(string conditionID)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(conditionID);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsConditionActive_Public_Boolean_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(142)]
	[CachedScanResults(RefRangeStart = 362730, RefRangeEnd = 362872, XrefRangeStart = 362728, XrefRangeEnd = 362730, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsID(string id)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(id);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsID_Public_Boolean_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 362881, RefRangeEnd = 362885, XrefRangeStart = 362872, XrefRangeEnd = 362881, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int GetItemSize()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetItemSize_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 362894, RefRangeEnd = 362895, XrefRangeStart = 362885, XrefRangeEnd = 362894, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe int GetModifiedItemSize()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetModifiedItemSize_Public_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362895, XrefRangeEnd = 362945, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void __ctor_b__177_0(GameItem _, GameInventory _, SlotMarker _)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)_);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)_);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)_);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__177_0_Private_Void_GameItem_GameInventory_SlotMarker_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362945, XrefRangeEnd = 362954, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void _TryUnloadAll_b__250_0(GameItem gi)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)gi);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__TryUnloadAll_b__250_0_Private_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	public GameItem(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
