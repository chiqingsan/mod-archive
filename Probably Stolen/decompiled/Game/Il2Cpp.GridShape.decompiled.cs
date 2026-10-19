using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;

namespace Il2Cpp;

public class GridShape : Il2CppObjectBase
{
	[System.Serializable]
	[ObfuscatedName("GridShape+<>c")]
	public sealed class __c : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___9;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__32_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__33_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__34_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__Intersects_b__32_0_Internal_Boolean_Byte_Byte_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__FullyIntersects_b__33_0_Internal_Boolean_Byte_Byte_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__FullyIntersectsRect_b__34_0_Internal_Boolean_Byte_Byte_0;

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

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__32_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__32_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__32_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__33_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__33_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__33_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<byte, byte, bool> __9__34_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__34_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<byte, byte, bool>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__34_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c()
		{
			Il2CppClassPointerStore<__c>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GridShape>.NativeClassPtr, "<>c");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c>.NativeClassPtr);
			NativeFieldInfoPtr___9 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9");
			NativeFieldInfoPtr___9__32_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__32_0");
			NativeFieldInfoPtr___9__33_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__33_0");
			NativeFieldInfoPtr___9__34_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__34_0");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674629);
			NativeMethodInfoPtr__Intersects_b__32_0_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674630);
			NativeMethodInfoPtr__FullyIntersects_b__33_0_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674631);
			NativeMethodInfoPtr__FullyIntersectsRect_b__34_0_Internal_Boolean_Byte_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674632);
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
		public unsafe bool _Intersects_b__32_0(byte b, byte bOther)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bOther;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__Intersects_b__32_0_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		public unsafe bool _FullyIntersects_b__33_0(byte b, byte bOther)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bOther;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__FullyIntersects_b__33_0_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		public unsafe bool _FullyIntersectsRect_b__34_0(byte b, byte bOther)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = (nint)(&b);
			*(byte**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &bOther;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__FullyIntersectsRect_b__34_0_Internal_Boolean_Byte_Byte_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeMethodInfoPtr_get_minX_Public_Abstract_Virtual_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_minY_Public_Abstract_Virtual_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_globalWidth_Public_Abstract_Virtual_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_globalHeight_Public_Abstract_Virtual_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_maxX_Public_Abstract_Virtual_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_maxY_Public_Abstract_Virtual_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_flipped_Public_Abstract_Virtual_New_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_orientation_Public_Abstract_Virtual_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_width_Public_Abstract_Virtual_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_height_Public_Abstract_Virtual_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Clone_Public_Abstract_Virtual_New_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetLocal_Public_Abstract_Virtual_New_Byte_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetOutsideBounds_Public_Virtual_New_Byte_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Get_Public_Virtual_New_Byte_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ForwardTransform_Public_Virtual_New_ValueTuple_2_Int32_Int32_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_InverseTransform_Public_Virtual_New_ValueTuple_2_Int32_Int32_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IntersectsBounds_Private_Boolean_Int32_Int32_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IntersectsBounds_Public_Virtual_New_Boolean_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IntersectsBounds_Public_Virtual_New_Boolean_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetIntersection_Public_Virtual_New_ValueTuple_5_Int32_Int32_Int32_Int32_Boolean_GridShape_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Intersects_Public_Virtual_New_Boolean_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Intersects_Public_Virtual_New_Boolean_GridShape_Func_3_Byte_Byte_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FullyIntersects_Public_Virtual_New_Boolean_GridShape_Func_3_Byte_Byte_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FullyIntersectsRect_Public_Virtual_New_Boolean_Int32_Int32_Func_3_Byte_Byte_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsTouchingTop_Public_Virtual_New_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsTouchingLeft_Public_Virtual_New_Boolean_0;

	public unsafe virtual int minX
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_minX_Public_Abstract_Virtual_New_get_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int minY
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_minY_Public_Abstract_Virtual_New_get_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int globalWidth
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_globalWidth_Public_Abstract_Virtual_New_get_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int globalHeight
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_globalHeight_Public_Abstract_Virtual_New_get_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int maxX
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_maxX_Public_Abstract_Virtual_New_get_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int maxY
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_maxY_Public_Abstract_Virtual_New_get_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual bool flipped
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_flipped_Public_Abstract_Virtual_New_get_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int orientation
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_orientation_Public_Abstract_Virtual_New_get_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int width
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_width_Public_Abstract_Virtual_New_get_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int height
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_height_Public_Abstract_Virtual_New_get_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	static GridShape()
	{
		Il2CppClassPointerStore<GridShape>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "GridShape");
		NativeMethodInfoPtr_get_minX_Public_Abstract_Virtual_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674602);
		NativeMethodInfoPtr_get_minY_Public_Abstract_Virtual_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674603);
		NativeMethodInfoPtr_get_globalWidth_Public_Abstract_Virtual_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674604);
		NativeMethodInfoPtr_get_globalHeight_Public_Abstract_Virtual_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674605);
		NativeMethodInfoPtr_get_maxX_Public_Abstract_Virtual_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674606);
		NativeMethodInfoPtr_get_maxY_Public_Abstract_Virtual_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674607);
		NativeMethodInfoPtr_get_flipped_Public_Abstract_Virtual_New_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674608);
		NativeMethodInfoPtr_get_orientation_Public_Abstract_Virtual_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674609);
		NativeMethodInfoPtr_get_width_Public_Abstract_Virtual_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674610);
		NativeMethodInfoPtr_get_height_Public_Abstract_Virtual_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674611);
		NativeMethodInfoPtr_Clone_Public_Abstract_Virtual_New_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674612);
		NativeMethodInfoPtr_GetLocal_Public_Abstract_Virtual_New_Byte_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674613);
		NativeMethodInfoPtr_GetOutsideBounds_Public_Virtual_New_Byte_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674614);
		NativeMethodInfoPtr_Get_Public_Virtual_New_Byte_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674615);
		NativeMethodInfoPtr_ForwardTransform_Public_Virtual_New_ValueTuple_2_Int32_Int32_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674616);
		NativeMethodInfoPtr_InverseTransform_Public_Virtual_New_ValueTuple_2_Int32_Int32_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674617);
		NativeMethodInfoPtr_IntersectsBounds_Private_Boolean_Int32_Int32_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674618);
		NativeMethodInfoPtr_IntersectsBounds_Public_Virtual_New_Boolean_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674619);
		NativeMethodInfoPtr_IntersectsBounds_Public_Virtual_New_Boolean_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674620);
		NativeMethodInfoPtr_GetIntersection_Public_Virtual_New_ValueTuple_5_Int32_Int32_Int32_Int32_Boolean_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674621);
		NativeMethodInfoPtr_Intersects_Public_Virtual_New_Boolean_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674622);
		NativeMethodInfoPtr_Intersects_Public_Virtual_New_Boolean_GridShape_Func_3_Byte_Byte_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674623);
		NativeMethodInfoPtr_FullyIntersects_Public_Virtual_New_Boolean_GridShape_Func_3_Byte_Byte_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674624);
		NativeMethodInfoPtr_FullyIntersectsRect_Public_Virtual_New_Boolean_Int32_Int32_Func_3_Byte_Byte_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674625);
		NativeMethodInfoPtr_IsTouchingTop_Public_Virtual_New_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674626);
		NativeMethodInfoPtr_IsTouchingLeft_Public_Virtual_New_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GridShape>.NativeClassPtr, 100674627);
	}

	[CallerCount(0)]
	public unsafe virtual GridShape Clone()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Clone_Public_Abstract_Virtual_New_GridShape_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe virtual byte GetLocal(int localX, int localY)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&localX);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &localY;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_GetLocal_Public_Abstract_Virtual_New_Byte_Int32_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(byte*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347522, XrefRangeEnd = 347525, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual byte GetOutsideBounds()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_GetOutsideBounds_Public_Virtual_New_Byte_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(byte*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347525, XrefRangeEnd = 347530, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual byte Get(int globalX, int globalY)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&globalX);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &globalY;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Get_Public_Virtual_New_Byte_Int32_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(byte*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347530, XrefRangeEnd = 347554, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual Il2CppSystem.ValueTuple<int, int> ForwardTransform(int localX, int localY)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&localX);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &localY;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_ForwardTransform_Public_Virtual_New_ValueTuple_2_Int32_Int32_Int32_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return new Il2CppSystem.ValueTuple<int, int>(pointer);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347554, XrefRangeEnd = 347578, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual Il2CppSystem.ValueTuple<int, int> InverseTransform(int x, int y)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&x);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &y;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_InverseTransform_Public_Virtual_New_ValueTuple_2_Int32_Int32_Int32_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return new Il2CppSystem.ValueTuple<int, int>(pointer);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 347588, RefRangeEnd = 347590, XrefRangeStart = 347578, XrefRangeEnd = 347588, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IntersectsBounds(int minX, int minY, int maxX, int maxY)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = (nint)(&minX);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &minY;
		*(int**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &maxX;
		*(int**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &maxY;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IntersectsBounds_Private_Boolean_Int32_Int32_Int32_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347590, XrefRangeEnd = 347591, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool IntersectsBounds(int x, int y)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&x);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &y;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_IntersectsBounds_Public_Virtual_New_Boolean_Int32_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347591, XrefRangeEnd = 347601, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool IntersectsBounds(GridShape other)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)other);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_IntersectsBounds_Public_Virtual_New_Boolean_GridShape_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347601, XrefRangeEnd = 347629, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual Il2CppSystem.ValueTuple<int, int, int, int, bool> GetIntersection(GridShape other)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)other);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_GetIntersection_Public_Virtual_New_ValueTuple_5_Int32_Int32_Int32_Int32_Boolean_GridShape_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return new Il2CppSystem.ValueTuple<int, int, int, int, bool>(pointer);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347629, XrefRangeEnd = 347637, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool Intersects(int x, int y)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&x);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &y;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Intersects_Public_Virtual_New_Boolean_Int32_Int32_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347637, XrefRangeEnd = 347661, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool Intersects(GridShape other, Il2CppSystem.Func<byte, byte, bool> condition = null)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)other);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)condition);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Intersects_Public_Virtual_New_Boolean_GridShape_Func_3_Byte_Byte_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347661, XrefRangeEnd = 347693, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool FullyIntersects(GridShape other, Il2CppSystem.Func<byte, byte, bool> condition = null)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)other);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)condition);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_FullyIntersects_Public_Virtual_New_Boolean_GridShape_Func_3_Byte_Byte_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347693, XrefRangeEnd = 347724, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool FullyIntersectsRect(int width, int height, Il2CppSystem.Func<byte, byte, bool> condition = null)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = (nint)(&width);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &height;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)condition);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_FullyIntersectsRect_Public_Virtual_New_Boolean_Int32_Int32_Func_3_Byte_Byte_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347724, XrefRangeEnd = 347727, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool IsTouchingTop()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_IsTouchingTop_Public_Virtual_New_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 347727, XrefRangeEnd = 347730, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool IsTouchingLeft()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_IsTouchingLeft_Public_Virtual_New_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	public GridShape(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
