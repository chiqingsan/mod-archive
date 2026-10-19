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

public class PixelWindow : Il2CppSystem.Object
{
	[ObfuscatedName("PixelWindow+<>c__DisplayClass80_0")]
	public sealed class __c__DisplayClass80_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_savedOrigin;

		private static readonly System.IntPtr NativeFieldInfoPtr_savedPointer;

		private static readonly System.IntPtr NativeFieldInfoPtr_xButtonInvertedSprite;

		private static readonly System.IntPtr NativeFieldInfoPtr_isButtonPressed;

		private static readonly System.IntPtr NativeFieldInfoPtr_xButtonFlatSprite;

		private static readonly System.IntPtr NativeFieldInfoPtr_xButtonSprite;

		private static readonly System.IntPtr NativeFieldInfoPtr_resizeButtonDownSprite;

		private static readonly System.IntPtr NativeFieldInfoPtr_resizeButtonSprite;

		private static readonly System.IntPtr NativeFieldInfoPtr___4__this;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__0_Internal_Boolean_PointerEventData_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__1_Internal_Boolean_PointerEventData_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__4_Internal_Boolean_PointerEventData_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__5_Internal_Boolean_PointerEventData_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__6_Internal_Boolean_PointerEventData_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__7_Internal_Boolean_PointerEventData_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__9_Internal_Boolean_PointerEventData_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__10_Internal_Boolean_PointerEventData_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__11_Internal_Boolean_PointerEventData_0;

		private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__12_Internal_Boolean_PointerEventData_0;

		public unsafe Vector2 savedOrigin
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_savedOrigin);
				return *(Vector2*)num;
			}
			set
			{
				*(Vector2*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_savedOrigin)) = value;
			}
		}

		public unsafe Vector2 savedPointer
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_savedPointer);
				return *(Vector2*)num;
			}
			set
			{
				*(Vector2*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_savedPointer)) = value;
			}
		}

		public unsafe Sprite xButtonInvertedSprite
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_xButtonInvertedSprite);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Sprite>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_xButtonInvertedSprite)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe bool isButtonPressed
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isButtonPressed);
				return *(bool*)num;
			}
			set
			{
				*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isButtonPressed)) = value;
			}
		}

		public unsafe Sprite xButtonFlatSprite
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_xButtonFlatSprite);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Sprite>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_xButtonFlatSprite)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe Sprite xButtonSprite
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_xButtonSprite);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Sprite>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_xButtonSprite)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe Sprite resizeButtonDownSprite
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_resizeButtonDownSprite);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Sprite>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_resizeButtonDownSprite)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe Sprite resizeButtonSprite
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_resizeButtonSprite);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Sprite>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_resizeButtonSprite)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe PixelWindow __4__this
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___4__this);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr___4__this)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass80_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<>c__DisplayClass80_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr);
			NativeFieldInfoPtr_savedOrigin = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, "savedOrigin");
			NativeFieldInfoPtr_savedPointer = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, "savedPointer");
			NativeFieldInfoPtr_xButtonInvertedSprite = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, "xButtonInvertedSprite");
			NativeFieldInfoPtr_isButtonPressed = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, "isButtonPressed");
			NativeFieldInfoPtr_xButtonFlatSprite = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, "xButtonFlatSprite");
			NativeFieldInfoPtr_xButtonSprite = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, "xButtonSprite");
			NativeFieldInfoPtr_resizeButtonDownSprite = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, "resizeButtonDownSprite");
			NativeFieldInfoPtr_resizeButtonSprite = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, "resizeButtonSprite");
			NativeFieldInfoPtr___4__this = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, "<>4__this");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676187);
			NativeMethodInfoPtr___ctor_b__0_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676188);
			NativeMethodInfoPtr___ctor_b__1_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676189);
			NativeMethodInfoPtr___ctor_b__4_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676190);
			NativeMethodInfoPtr___ctor_b__5_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676191);
			NativeMethodInfoPtr___ctor_b__6_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676192);
			NativeMethodInfoPtr___ctor_b__7_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676193);
			NativeMethodInfoPtr___ctor_b__9_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676194);
			NativeMethodInfoPtr___ctor_b__10_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676195);
			NativeMethodInfoPtr___ctor_b__11_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676196);
			NativeMethodInfoPtr___ctor_b__12_Internal_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr, 100676197);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass80_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass80_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370702, XrefRangeEnd = 370706, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__0(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__0_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370706, XrefRangeEnd = 370717, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__1(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__1_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370717, XrefRangeEnd = 370719, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__4(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__4_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370719, XrefRangeEnd = 370721, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__5(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__5_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370721, XrefRangeEnd = 370722, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__6(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__6_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370722, XrefRangeEnd = 370723, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__7(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__7_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370723, XrefRangeEnd = 370724, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__9(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__9_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370724, XrefRangeEnd = 370725, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__10(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__10_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370725, XrefRangeEnd = 370741, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__11(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__11_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool __ctor_b__12(PointerEventData r)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__12_Internal_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass80_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[System.Serializable]
	[ObfuscatedName("PixelWindow+<>c")]
	public sealed class __c : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___9;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__91_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__92_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__101_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ReclampDockedWindows_b__91_0_Internal_Boolean_PixelWindow_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__OnLetterboxLayoutChanged_b__92_0_Internal_Boolean_PixelWindow_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__Show_b__101_0_Internal_Void_GameInventory_0;

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

		public unsafe static Il2CppSystem.Predicate<PixelWindow> __9__91_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__91_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Predicate<PixelWindow>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__91_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Predicate<PixelWindow> __9__92_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__92_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Predicate<PixelWindow>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__92_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<GameInventory> __9__101_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__101_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<GameInventory>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__101_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c()
		{
			Il2CppClassPointerStore<__c>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<>c");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c>.NativeClassPtr);
			NativeFieldInfoPtr___9 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9");
			NativeFieldInfoPtr___9__91_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__91_0");
			NativeFieldInfoPtr___9__92_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__92_0");
			NativeFieldInfoPtr___9__101_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__101_0");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100676199);
			NativeMethodInfoPtr__ReclampDockedWindows_b__91_0_Internal_Boolean_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100676200);
			NativeMethodInfoPtr__OnLetterboxLayoutChanged_b__92_0_Internal_Boolean_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100676201);
			NativeMethodInfoPtr__Show_b__101_0_Internal_Void_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100676202);
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
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370741, XrefRangeEnd = 370742, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _ReclampDockedWindows_b__91_0(PixelWindow w)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)w);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ReclampDockedWindows_b__91_0_Internal_Boolean_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370742, XrefRangeEnd = 370743, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe bool _OnLetterboxLayoutChanged_b__92_0(PixelWindow w)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)w);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__OnLetterboxLayoutChanged_b__92_0_Internal_Boolean_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370743, XrefRangeEnd = 370744, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _Show_b__101_0(GameInventory inv)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)inv);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__Show_b__101_0_Internal_Void_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeFieldInfoPtr__parentItems_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_canMove;

	private static readonly System.IntPtr NativeFieldInfoPtr__widthPixels_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__heightPixels_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__handler_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__inner_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__childElement_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__titleHeight_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__xPadding_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__yPadding_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_background;

	private static readonly System.IntPtr NativeFieldInfoPtr_outsideBorder;

	private static readonly System.IntPtr NativeFieldInfoPtr_topDragBorder;

	private static readonly System.IntPtr NativeFieldInfoPtr_topCloseButton;

	private static readonly System.IntPtr NativeFieldInfoPtr_topTitleBackground;

	private static readonly System.IntPtr NativeFieldInfoPtr_bottomResizeButton;

	private static readonly System.IntPtr NativeFieldInfoPtr__titleString_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_titleText;

	private static readonly System.IntPtr NativeFieldInfoPtr__transparency_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_forceRaycast;

	private static readonly System.IntPtr NativeFieldInfoPtr_hideBorder;

	private static readonly System.IntPtr NativeFieldInfoPtr__isImportant_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__closeCallback_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__openCallback_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_preDockParent;

	private static readonly System.IntPtr NativeFieldInfoPtr_dockedFloating;

	private static readonly System.IntPtr NativeFieldInfoPtr_dockLayoutHooked;

	private static readonly System.IntPtr NativeFieldInfoPtr_focusStampCounter;

	private static readonly System.IntPtr NativeFieldInfoPtr__focusStamp_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_isLocked;

	private static readonly System.IntPtr NativeFieldInfoPtr__stateChangeActionFunc_k__BackingField;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_parents_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_children_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_parentItems_Public_Virtual_Final_New_get_List_1_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_parentItems_Private_set_Void_List_1_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_child_Public_Virtual_Final_New_get_GraphNodeStorage_0;

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

	private static readonly System.IntPtr NativeMethodInfoPtr_get_childElement_Public_get_PixelElement_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_childElement_Private_set_Void_PixelElement_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_titleHeight_Public_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_titleHeight_Private_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_xPadding_Public_Virtual_Final_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_xPadding_Public_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_yPadding_Public_Virtual_Final_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_yPadding_Public_set_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_hasTitle_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_titleString_Public_get_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_titleString_Private_set_Void_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_transparency_Public_get_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_transparency_Private_set_Void_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_isImportant_Public_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_isImportant_Public_set_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_closeCallback_Public_get_Action_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_closeCallback_Public_set_Void_Action_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_openCallback_Public_get_Action_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_openCallback_Public_set_Void_Action_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_Boolean_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Boolean_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Boolean_Int32_Int32_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetTitle_Public_PixelWindow_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetTransparency_Public_PixelWindow_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Validate_Public_Virtual_Final_New_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetBoundsCanvas_Private_Canvas_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_EnsureFloatingDock_Private_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_DockAnchorOffset_Private_Vector2_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ReclampDockedWindows_Public_Static_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_OnLetterboxLayoutChanged_Private_Static_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FixBounds_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FixTooltipBounds_Private_Void_Vector2_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_NextFocusStamp_Public_Static_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_focusStamp_Public_get_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_focusStamp_Private_set_Void_Int64_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Show_Public_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Show_Public_PixelWindow_Vector2_Vector2_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Center_Public_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Hide_Public_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Lock_Public_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Unlock_Public_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsVisible_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Attach_Public_Virtual_Final_New_Boolean_PixelElement_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Detach_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Detach_Public_Virtual_Final_New_Boolean_PixelElement_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ToFront_Public_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ToBack_Public_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Final_New_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_Final_New_SlotMarker_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_stateChangeActionFunc_Public_get_Action_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_stateChangeActionFunc_Private_set_Void_Action_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddInventoryStateChangeAction_Public_PixelWindow_Action_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_HideBorder_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__80_2_Private_Boolean_PointerEventData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__80_3_Private_Boolean_PointerEventData_0;

	private static readonly System.IntPtr NativeMethodInfoPtr___ctor_b__80_8_Private_Boolean_PointerEventData_0;

	public unsafe List<GameItem> _parentItems_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__parentItems_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GameItem>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__parentItems_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe bool canMove
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_canMove);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_canMove)) = value;
		}
	}

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

	public unsafe PixelElement _childElement_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__childElement_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelElement>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__childElement_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe int _titleHeight_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__titleHeight_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__titleHeight_k__BackingField)) = value;
		}
	}

	public unsafe int _xPadding_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__xPadding_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__xPadding_k__BackingField)) = value;
		}
	}

	public unsafe int _yPadding_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__yPadding_k__BackingField);
			return *(int*)num;
		}
		set
		{
			*(int*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__yPadding_k__BackingField)) = value;
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

	public unsafe TreeNodeRender outsideBorder
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_outsideBorder);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_outsideBorder)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender topDragBorder
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_topDragBorder);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_topDragBorder)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender topCloseButton
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_topCloseButton);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_topCloseButton)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender topTitleBackground
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_topTitleBackground);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_topTitleBackground)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender bottomResizeButton
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_bottomResizeButton);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_bottomResizeButton)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe string _titleString_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__titleString_k__BackingField);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__titleString_k__BackingField)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe TreeNodeRender titleText
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_titleText);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_titleText)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
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

	public unsafe bool forceRaycast
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_forceRaycast);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_forceRaycast)) = value;
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

	public unsafe bool _isImportant_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__isImportant_k__BackingField);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__isImportant_k__BackingField)) = value;
		}
	}

	public unsafe Il2CppSystem.Action _closeCallback_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__closeCallback_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__closeCallback_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Il2CppSystem.Action _openCallback_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__openCallback_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__openCallback_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe Transform preDockParent
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_preDockParent);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Transform>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_preDockParent)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe static List<PixelWindow> dockedFloating
	{
		get
		{
			Unsafe.SkipInit(out System.IntPtr intPtr);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_dockedFloating, (void*)(&intPtr));
			System.IntPtr intPtr2 = intPtr;
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<PixelWindow>>(intPtr2) : null;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_dockedFloating, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe static bool dockLayoutHooked
	{
		get
		{
			Unsafe.SkipInit(out bool result);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_dockLayoutHooked, (void*)(&result));
			return result;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_dockLayoutHooked, (void*)(&value));
		}
	}

	public unsafe static long focusStampCounter
	{
		get
		{
			Unsafe.SkipInit(out long result);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_focusStampCounter, (void*)(&result));
			return result;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_focusStampCounter, (void*)(&value));
		}
	}

	public unsafe long _focusStamp_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__focusStamp_k__BackingField);
			return *(long*)num;
		}
		set
		{
			*(long*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__focusStamp_k__BackingField)) = value;
		}
	}

	public unsafe bool isLocked
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isLocked);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_isLocked)) = value;
		}
	}

	public unsafe Il2CppSystem.Action _stateChangeActionFunc_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__stateChangeActionFunc_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__stateChangeActionFunc_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe virtual List<GraphNodeStorage> parents
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370744, XrefRangeEnd = 370750, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
		[CallerCount(2)]
		[CachedScanResults(RefRangeStart = 370761, RefRangeEnd = 370763, XrefRangeStart = 370750, XrefRangeEnd = 370761, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_children_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GraphNodeStorage>>(intPtr2) : null;
		}
	}

	public unsafe virtual List<GameItem> parentItems
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_parentItems_Public_Virtual_Final_New_get_List_1_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GameItem>>(intPtr2) : null;
		}
		[CallerCount(8)]
		[CachedScanResults(RefRangeStart = 10099, RefRangeEnd = 10107, XrefRangeStart = 10099, XrefRangeEnd = 10107, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_parentItems_Private_set_Void_List_1_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual GraphNodeStorage child
	{
		[CallerCount(25)]
		[CachedScanResults(RefRangeStart = 370767, RefRangeEnd = 370792, XrefRangeStart = 370763, XrefRangeEnd = 370767, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_child_Public_Virtual_Final_New_get_GraphNodeStorage_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GraphNodeStorage>(intPtr2) : null;
		}
	}

	public unsafe virtual bool canResize
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370792, XrefRangeEnd = 370795, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
		[CallerCount(1)]
		[CachedScanResults(RefRangeStart = 104428, RefRangeEnd = 104429, XrefRangeStart = 104428, XrefRangeEnd = 104429, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
		[CallerCount(170)]
		[CachedScanResults(RefRangeStart = 41069, RefRangeEnd = 41239, XrefRangeStart = 41069, XrefRangeEnd = 41239, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
		[CallerCount(95)]
		[CachedScanResults(RefRangeStart = 370795, RefRangeEnd = 370890, XrefRangeStart = 370795, XrefRangeEnd = 370795, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
		[CallerCount(4)]
		[CachedScanResults(RefRangeStart = 39627, RefRangeEnd = 39631, XrefRangeStart = 39627, XrefRangeEnd = 39631, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	public unsafe PixelElement childElement
	{
		[CallerCount(8)]
		[CachedScanResults(RefRangeStart = 42476, RefRangeEnd = 42484, XrefRangeStart = 42476, XrefRangeEnd = 42484, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_childElement_Public_get_PixelElement_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelElement>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_childElement_Private_set_Void_PixelElement_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe int titleHeight
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_titleHeight_Public_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_titleHeight_Private_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual int xPadding
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_xPadding_Public_Virtual_Final_New_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_xPadding_Public_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual int yPadding
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_yPadding_Public_Virtual_Final_New_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_yPadding_Public_set_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe bool hasTitle
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 370890, XrefRangeEnd = 370891, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_hasTitle_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe string titleString
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_titleString_Public_get_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return IL2CPP.Il2CppStringToManaged(intPtr2);
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.ManagedStringToIl2Cpp(value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_titleString_Private_set_Void_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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

	public unsafe bool isImportant
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_isImportant_Public_get_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_isImportant_Public_set_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe Il2CppSystem.Action closeCallback
	{
		[CallerCount(4)]
		[CachedScanResults(RefRangeStart = 350823, RefRangeEnd = 350827, XrefRangeStart = 350823, XrefRangeEnd = 350827, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_closeCallback_Public_get_Action_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_closeCallback_Public_set_Void_Action_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe Il2CppSystem.Action openCallback
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_openCallback_Public_get_Action_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_openCallback_Public_set_Void_Action_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe long focusStamp
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_focusStamp_Public_get_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
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
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_focusStamp_Private_set_Void_Int64_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe Il2CppSystem.Action stateChangeActionFunc
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_stateChangeActionFunc_Public_get_Action_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_stateChangeActionFunc_Private_set_Void_Action_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	static PixelWindow()
	{
		Il2CppClassPointerStore<PixelWindow>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "PixelWindow");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr);
		NativeFieldInfoPtr__parentItems_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<parentItems>k__BackingField");
		NativeFieldInfoPtr_canMove = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "canMove");
		NativeFieldInfoPtr__widthPixels_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<widthPixels>k__BackingField");
		NativeFieldInfoPtr__heightPixels_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<heightPixels>k__BackingField");
		NativeFieldInfoPtr__handler_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<handler>k__BackingField");
		NativeFieldInfoPtr__inner_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<inner>k__BackingField");
		NativeFieldInfoPtr__childElement_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<childElement>k__BackingField");
		NativeFieldInfoPtr__titleHeight_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<titleHeight>k__BackingField");
		NativeFieldInfoPtr__xPadding_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<xPadding>k__BackingField");
		NativeFieldInfoPtr__yPadding_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<yPadding>k__BackingField");
		NativeFieldInfoPtr_background = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "background");
		NativeFieldInfoPtr_outsideBorder = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "outsideBorder");
		NativeFieldInfoPtr_topDragBorder = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "topDragBorder");
		NativeFieldInfoPtr_topCloseButton = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "topCloseButton");
		NativeFieldInfoPtr_topTitleBackground = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "topTitleBackground");
		NativeFieldInfoPtr_bottomResizeButton = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "bottomResizeButton");
		NativeFieldInfoPtr__titleString_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<titleString>k__BackingField");
		NativeFieldInfoPtr_titleText = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "titleText");
		NativeFieldInfoPtr__transparency_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<transparency>k__BackingField");
		NativeFieldInfoPtr_forceRaycast = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "forceRaycast");
		NativeFieldInfoPtr_hideBorder = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "hideBorder");
		NativeFieldInfoPtr__isImportant_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<isImportant>k__BackingField");
		NativeFieldInfoPtr__closeCallback_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<closeCallback>k__BackingField");
		NativeFieldInfoPtr__openCallback_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<openCallback>k__BackingField");
		NativeFieldInfoPtr_preDockParent = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "preDockParent");
		NativeFieldInfoPtr_dockedFloating = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "dockedFloating");
		NativeFieldInfoPtr_dockLayoutHooked = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "dockLayoutHooked");
		NativeFieldInfoPtr_focusStampCounter = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "focusStampCounter");
		NativeFieldInfoPtr__focusStamp_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<focusStamp>k__BackingField");
		NativeFieldInfoPtr_isLocked = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "isLocked");
		NativeFieldInfoPtr__stateChangeActionFunc_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, "<stateChangeActionFunc>k__BackingField");
		NativeMethodInfoPtr_get_parents_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676112);
		NativeMethodInfoPtr_get_children_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676113);
		NativeMethodInfoPtr_get_parentItems_Public_Virtual_Final_New_get_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676114);
		NativeMethodInfoPtr_set_parentItems_Private_set_Void_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676115);
		NativeMethodInfoPtr_get_child_Public_Virtual_Final_New_get_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676116);
		NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676117);
		NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676118);
		NativeMethodInfoPtr_set_widthPixels_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676119);
		NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676120);
		NativeMethodInfoPtr_set_heightPixels_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676121);
		NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676122);
		NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676123);
		NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676124);
		NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676125);
		NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676126);
		NativeMethodInfoPtr_get_childElement_Public_get_PixelElement_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676127);
		NativeMethodInfoPtr_set_childElement_Private_set_Void_PixelElement_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676128);
		NativeMethodInfoPtr_get_titleHeight_Public_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676129);
		NativeMethodInfoPtr_set_titleHeight_Private_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676130);
		NativeMethodInfoPtr_get_xPadding_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676131);
		NativeMethodInfoPtr_set_xPadding_Public_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676132);
		NativeMethodInfoPtr_get_yPadding_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676133);
		NativeMethodInfoPtr_set_yPadding_Public_set_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676134);
		NativeMethodInfoPtr_get_hasTitle_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676135);
		NativeMethodInfoPtr_get_titleString_Public_get_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676136);
		NativeMethodInfoPtr_set_titleString_Private_set_Void_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676137);
		NativeMethodInfoPtr_get_transparency_Public_get_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676138);
		NativeMethodInfoPtr_set_transparency_Private_set_Void_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676139);
		NativeMethodInfoPtr_get_isImportant_Public_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676140);
		NativeMethodInfoPtr_set_isImportant_Public_set_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676141);
		NativeMethodInfoPtr_get_closeCallback_Public_get_Action_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676142);
		NativeMethodInfoPtr_set_closeCallback_Public_set_Void_Action_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676143);
		NativeMethodInfoPtr_get_openCallback_Public_get_Action_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676144);
		NativeMethodInfoPtr_set_openCallback_Public_set_Void_Action_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676145);
		NativeMethodInfoPtr__ctor_Public_Void_Boolean_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676146);
		NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Boolean_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676147);
		NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Boolean_Int32_Int32_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676148);
		NativeMethodInfoPtr_SetTitle_Public_PixelWindow_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676149);
		NativeMethodInfoPtr_SetTransparency_Public_PixelWindow_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676150);
		NativeMethodInfoPtr_Validate_Public_Virtual_Final_New_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676151);
		NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676152);
		NativeMethodInfoPtr_GetBoundsCanvas_Private_Canvas_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676153);
		NativeMethodInfoPtr_EnsureFloatingDock_Private_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676154);
		NativeMethodInfoPtr_DockAnchorOffset_Private_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676155);
		NativeMethodInfoPtr_ReclampDockedWindows_Public_Static_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676156);
		NativeMethodInfoPtr_OnLetterboxLayoutChanged_Private_Static_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676157);
		NativeMethodInfoPtr_FixBounds_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676158);
		NativeMethodInfoPtr_FixTooltipBounds_Private_Void_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676159);
		NativeMethodInfoPtr_NextFocusStamp_Public_Static_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676160);
		NativeMethodInfoPtr_get_focusStamp_Public_get_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676161);
		NativeMethodInfoPtr_set_focusStamp_Private_set_Void_Int64_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676162);
		NativeMethodInfoPtr_Show_Public_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676163);
		NativeMethodInfoPtr_Show_Public_PixelWindow_Vector2_Vector2_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676164);
		NativeMethodInfoPtr_Center_Public_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676165);
		NativeMethodInfoPtr_Hide_Public_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676166);
		NativeMethodInfoPtr_Lock_Public_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676167);
		NativeMethodInfoPtr_Unlock_Public_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676168);
		NativeMethodInfoPtr_IsVisible_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676169);
		NativeMethodInfoPtr_Attach_Public_Virtual_Final_New_Boolean_PixelElement_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676170);
		NativeMethodInfoPtr_Detach_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676171);
		NativeMethodInfoPtr_Detach_Public_Virtual_Final_New_Boolean_PixelElement_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676172);
		NativeMethodInfoPtr_ToFront_Public_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676173);
		NativeMethodInfoPtr_ToBack_Public_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676174);
		NativeMethodInfoPtr_AddParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676175);
		NativeMethodInfoPtr_RemoveParent_Public_Virtual_Final_New_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676176);
		NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Virtual_Final_New_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676177);
		NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Virtual_Final_New_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676178);
		NativeMethodInfoPtr_get_stateChangeActionFunc_Public_get_Action_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676179);
		NativeMethodInfoPtr_set_stateChangeActionFunc_Private_set_Void_Action_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676180);
		NativeMethodInfoPtr_AddInventoryStateChangeAction_Public_PixelWindow_Action_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676181);
		NativeMethodInfoPtr_HideBorder_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676182);
		NativeMethodInfoPtr___ctor_b__80_2_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676184);
		NativeMethodInfoPtr___ctor_b__80_3_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676185);
		NativeMethodInfoPtr___ctor_b__80_8_Private_Boolean_PointerEventData_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr, 100676186);
	}

	[CallerCount(117)]
	[CachedScanResults(RefRangeStart = 370892, RefRangeEnd = 371009, XrefRangeStart = 370891, XrefRangeEnd = 370892, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow(bool canMove = false, string title = null)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr))
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&canMove);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(title);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_Boolean_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 371009, XrefRangeEnd = 371010, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow(int width, int height, bool canMove, string title = null)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr))
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = (nint)(&width);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &height;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &canMove;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(title);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Boolean_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 371274, RefRangeEnd = 371279, XrefRangeStart = 371010, XrefRangeEnd = 371274, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow(int width, int height, bool canMove, int xPadding, int yPadding, string title = null)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<PixelWindow>.NativeClassPtr))
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[6];
		*ptr = (nint)(&width);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &height;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &canMove;
		*(int**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &xPadding;
		*(int**)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = &yPadding;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)5u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(title);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_Int32_Int32_Boolean_Int32_Int32_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(19)]
	[CachedScanResults(RefRangeStart = 371294, RefRangeEnd = 371313, XrefRangeStart = 371279, XrefRangeEnd = 371294, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow SetTitle(string title)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(title);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetTitle_Public_PixelWindow_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
	}

	[CallerCount(20)]
	[CachedScanResults(RefRangeStart = 371327, RefRangeEnd = 371347, XrefRangeStart = 371313, XrefRangeEnd = 371327, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow SetTransparency(float transparency)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&transparency);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetTransparency_Public_PixelWindow_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
	}

	[CallerCount(102)]
	[CachedScanResults(RefRangeStart = 371441, RefRangeEnd = 371543, XrefRangeStart = 371347, XrefRangeEnd = 371441, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual void Validate()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Validate_Public_Virtual_Final_New_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(9)]
	[CachedScanResults(RefRangeStart = 371544, RefRangeEnd = 371553, XrefRangeStart = 371543, XrefRangeEnd = 371544, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 371569, RefRangeEnd = 371572, XrefRangeStart = 371553, XrefRangeEnd = 371569, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Canvas GetBoundsCanvas()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetBoundsCanvas_Private_Canvas_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Canvas>(intPtr2) : null;
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 371621, RefRangeEnd = 371623, XrefRangeStart = 371572, XrefRangeEnd = 371621, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void EnsureFloatingDock()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_EnsureFloatingDock_Private_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 371651, RefRangeEnd = 371652, XrefRangeStart = 371623, XrefRangeEnd = 371651, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Vector2 DockAnchorOffset()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_DockAnchorOffset_Private_Vector2_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(Vector2*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 371706, RefRangeEnd = 371707, XrefRangeStart = 371652, XrefRangeEnd = 371706, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void ReclampDockedWindows()
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ReclampDockedWindows_Public_Static_Void_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 371707, XrefRangeEnd = 371764, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static void OnLetterboxLayoutChanged()
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_OnLetterboxLayoutChanged_Private_Static_Void_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 371776, RefRangeEnd = 371781, XrefRangeStart = 371764, XrefRangeEnd = 371776, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void FixBounds()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FixBounds_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 371791, RefRangeEnd = 371792, XrefRangeStart = 371781, XrefRangeEnd = 371791, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void FixTooltipBounds(Vector2 worldTranslation)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&worldTranslation);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FixTooltipBounds_Private_Void_Vector2_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 371796, RefRangeEnd = 371797, XrefRangeStart = 371792, XrefRangeEnd = 371796, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static long NextFocusStamp()
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_NextFocusStamp_Public_Static_Int64_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(72)]
	[CachedScanResults(RefRangeStart = 371836, RefRangeEnd = 371908, XrefRangeStart = 371797, XrefRangeEnd = 371836, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow Show()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Show_Public_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 371936, RefRangeEnd = 371940, XrefRangeStart = 371908, XrefRangeEnd = 371936, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow Show(Vector2 mousePosition, Vector2 worldTranslation, bool fixTooltipBounds = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = (nint)(&mousePosition);
		*(Vector2**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &worldTranslation;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &fixTooltipBounds;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Show_Public_PixelWindow_Vector2_Vector2_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
	}

	[CallerCount(10)]
	[CachedScanResults(RefRangeStart = 371949, RefRangeEnd = 371959, XrefRangeStart = 371940, XrefRangeEnd = 371949, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow Center()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Center_Public_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
	}

	[CallerCount(66)]
	[CachedScanResults(RefRangeStart = 371981, RefRangeEnd = 372047, XrefRangeStart = 371959, XrefRangeEnd = 371981, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow Hide()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Hide_Public_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 372048, RefRangeEnd = 372053, XrefRangeStart = 372047, XrefRangeEnd = 372048, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow Lock()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Lock_Public_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 372054, RefRangeEnd = 372059, XrefRangeStart = 372053, XrefRangeEnd = 372054, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow Unlock()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Unlock_Public_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
	}

	[CallerCount(12)]
	[CachedScanResults(RefRangeStart = 372062, RefRangeEnd = 372074, XrefRangeStart = 372059, XrefRangeEnd = 372062, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsVisible()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsVisible_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(121)]
	[CachedScanResults(RefRangeStart = 372110, RefRangeEnd = 372231, XrefRangeStart = 372074, XrefRangeEnd = 372110, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool Attach(PixelElement element)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)element);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Attach_Public_Virtual_Final_New_Boolean_PixelElement_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 372241, RefRangeEnd = 372245, XrefRangeStart = 372231, XrefRangeEnd = 372241, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool Detach()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Detach_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 372255, RefRangeEnd = 372256, XrefRangeStart = 372245, XrefRangeEnd = 372255, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool Detach(PixelElement element)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)element);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_Detach_Public_Virtual_Final_New_Boolean_PixelElement_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(19)]
	[CachedScanResults(RefRangeStart = 372271, RefRangeEnd = 372290, XrefRangeStart = 372256, XrefRangeEnd = 372271, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ToFront(bool force = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&force);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ToFront_Public_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 372290, XrefRangeEnd = 372291, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ToBack(bool force = false)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&force);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ToBack_Public_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 372305, RefRangeEnd = 372309, XrefRangeStart = 372291, XrefRangeEnd = 372305, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 372325, RefRangeEnd = 372327, XrefRangeStart = 372309, XrefRangeEnd = 372325, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 372327, XrefRangeEnd = 372332, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 372336, RefRangeEnd = 372340, XrefRangeStart = 372332, XrefRangeEnd = 372336, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 372344, RefRangeEnd = 372346, XrefRangeStart = 372340, XrefRangeEnd = 372344, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe PixelWindow AddInventoryStateChangeAction(Il2CppSystem.Action action)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)action);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_AddInventoryStateChangeAction_Public_PixelWindow_Action_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<PixelWindow>(intPtr2) : null;
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 372348, RefRangeEnd = 372351, XrefRangeStart = 372346, XrefRangeEnd = 372348, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void HideBorder()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_HideBorder_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 372351, XrefRangeEnd = 372352, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__80_2(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__80_2_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__80_3(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__80_3_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 372352, XrefRangeEnd = 372370, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool __ctor_b__80_8(PointerEventData r)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)r);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr___ctor_b__80_8_Private_Boolean_PointerEventData_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	public PixelWindow(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
