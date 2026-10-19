using System;
using System.Runtime.CompilerServices;
using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace Il2Cpp;

public class GameInventory : GameInventoryFunc
{
	[ObfuscatedName("GameInventory+<>c__DisplayClass4_0")]
	public sealed class __c__DisplayClass4_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_parentItemMods;

		private static readonly System.IntPtr NativeFieldInfoPtr_newItem;

		private static readonly System.IntPtr NativeFieldInfoPtr_newSlot;

		private static readonly System.IntPtr NativeFieldInfoPtr_newGridShape;

		private static readonly System.IntPtr NativeFieldInfoPtr_newState;

		private static readonly System.IntPtr NativeFieldInfoPtr_parentItemStateMods;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__0_Internal_Void_GameItem_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__1_Internal_Void_GameItem_0;

		public unsafe List<Il2CppSystem.ValueTuple<GridShape, GridShape>> parentItemMods
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentItemMods);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<Il2CppSystem.ValueTuple<GridShape, GridShape>>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentItemMods)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe GameItem newItem
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newItem);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newItem)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe SlotMarker newSlot
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newSlot);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newSlot)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe GridShape newGridShape
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newGridShape);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newGridShape)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe TagSystem newState
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newState);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TagSystem>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newState)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe List<Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>>> parentItemStateMods
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentItemStateMods);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>>>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentItemStateMods)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass4_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, "<>c__DisplayClass4_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr);
			NativeFieldInfoPtr_parentItemMods = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr, "parentItemMods");
			NativeFieldInfoPtr_newItem = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr, "newItem");
			NativeFieldInfoPtr_newSlot = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr, "newSlot");
			NativeFieldInfoPtr_newGridShape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr, "newGridShape");
			NativeFieldInfoPtr_newState = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr, "newState");
			NativeFieldInfoPtr_parentItemStateMods = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr, "parentItemStateMods");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr, 100674903);
			NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__0_Internal_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr, 100674904);
			NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__1_Internal_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr, 100674905);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass4_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass4_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350445, XrefRangeEnd = 350448, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _CheckAcceptParentShapeStateModifier_b__0(GameItem pi)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)pi);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__0_Internal_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350448, XrefRangeEnd = 350451, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _CheckAcceptParentShapeStateModifier_b__1(GameItem pi)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)pi);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__1_Internal_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c__DisplayClass4_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[System.Serializable]
	[ObfuscatedName("GameInventory+<>c")]
	public sealed class __c : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___9;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__4_2;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__4_3;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__5_2;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__5_3;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__4_2_Internal_ValueTuple_3_GameItem_GridShape_GridShape_GameItem_ValueTuple_2_GridShape_GridShape_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__4_3_Internal_ValueTuple_5_GameItem_GridShape_GridShape_List_1_ItemStateModifier_List_1_ItemStateModifier_ValueTuple_3_GameItem_GridShape_GridShape_ValueTuple_2_List_1_ItemStateModifier_List_1_ItemStateModifier_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__5_2_Internal_ValueTuple_3_GameItem_GridShape_GridShape_GameItem_ValueTuple_2_GridShape_GridShape_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__5_3_Internal_ValueTuple_5_GameItem_GridShape_GridShape_List_1_ItemStateModifier_List_1_ItemStateModifier_ValueTuple_3_GameItem_GridShape_GridShape_ValueTuple_2_List_1_ItemStateModifier_List_1_ItemStateModifier_0;

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

		public unsafe static Il2CppSystem.Func<GameItem, Il2CppSystem.ValueTuple<GridShape, GridShape>, Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>> __9__4_2
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__4_2, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<GameItem, Il2CppSystem.ValueTuple<GridShape, GridShape>, Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__4_2, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>, Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>>, Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape, List<ItemStateModifier>, List<ItemStateModifier>>> __9__4_3
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__4_3, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>, Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>>, Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape, List<ItemStateModifier>, List<ItemStateModifier>>>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__4_3, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<GameItem, Il2CppSystem.ValueTuple<GridShape, GridShape>, Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>> __9__5_2
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__5_2, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<GameItem, Il2CppSystem.ValueTuple<GridShape, GridShape>, Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__5_2, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Func<Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>, Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>>, Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape, List<ItemStateModifier>, List<ItemStateModifier>>> __9__5_3
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__5_3, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>, Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>>, Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape, List<ItemStateModifier>, List<ItemStateModifier>>>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__5_3, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c()
		{
			Il2CppClassPointerStore<__c>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, "<>c");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c>.NativeClassPtr);
			NativeFieldInfoPtr___9 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9");
			NativeFieldInfoPtr___9__4_2 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__4_2");
			NativeFieldInfoPtr___9__4_3 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__4_3");
			NativeFieldInfoPtr___9__5_2 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__5_2");
			NativeFieldInfoPtr___9__5_3 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__5_3");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674907);
			NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__4_2_Internal_ValueTuple_3_GameItem_GridShape_GridShape_GameItem_ValueTuple_2_GridShape_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674908);
			NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__4_3_Internal_ValueTuple_5_GameItem_GridShape_GridShape_List_1_ItemStateModifier_List_1_ItemStateModifier_ValueTuple_3_GameItem_GridShape_GridShape_ValueTuple_2_List_1_ItemStateModifier_List_1_ItemStateModifier_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674909);
			NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__5_2_Internal_ValueTuple_3_GameItem_GridShape_GridShape_GameItem_ValueTuple_2_GridShape_GridShape_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674910);
			NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__5_3_Internal_ValueTuple_5_GameItem_GridShape_GridShape_List_1_ItemStateModifier_List_1_ItemStateModifier_ValueTuple_3_GameItem_GridShape_GridShape_ValueTuple_2_List_1_ItemStateModifier_List_1_ItemStateModifier_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100674911);
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
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350451, XrefRangeEnd = 350454, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape> _CheckAcceptParentShapeStateModifier_b__4_2(GameItem a, Il2CppSystem.ValueTuple<GridShape, GridShape> b)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)a);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)b));
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__4_2_Internal_ValueTuple_3_GameItem_GridShape_GridShape_GameItem_ValueTuple_2_GridShape_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return new Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>(pointer);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350454, XrefRangeEnd = 350457, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape, List<ItemStateModifier>, List<ItemStateModifier>> _CheckAcceptParentShapeStateModifier_b__4_3(Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape> a, Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>> b)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)a));
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)b));
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckAcceptParentShapeStateModifier_b__4_3_Internal_ValueTuple_5_GameItem_GridShape_GridShape_List_1_ItemStateModifier_List_1_ItemStateModifier_ValueTuple_3_GameItem_GridShape_GridShape_ValueTuple_2_List_1_ItemStateModifier_List_1_ItemStateModifier_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return new Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape, List<ItemStateModifier>, List<ItemStateModifier>>(pointer);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350457, XrefRangeEnd = 350460, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape> _CheckChangeParentShapeStateModifier_b__5_2(GameItem a, Il2CppSystem.ValueTuple<GridShape, GridShape> b)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)a);
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)b));
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__5_2_Internal_ValueTuple_3_GameItem_GridShape_GridShape_GameItem_ValueTuple_2_GridShape_GridShape_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return new Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape>(pointer);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350460, XrefRangeEnd = 350463, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape, List<ItemStateModifier>, List<ItemStateModifier>> _CheckChangeParentShapeStateModifier_b__5_3(Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape> a, Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>> b)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[2];
			*ptr = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)a));
			*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)b));
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__5_3_Internal_ValueTuple_5_GameItem_GridShape_GridShape_List_1_ItemStateModifier_List_1_ItemStateModifier_ValueTuple_3_GameItem_GridShape_GridShape_ValueTuple_2_List_1_ItemStateModifier_List_1_ItemStateModifier_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return new Il2CppSystem.ValueTuple<GameItem, GridShape, GridShape, List<ItemStateModifier>, List<ItemStateModifier>>(pointer);
		}

		public __c(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameInventory+<>c__DisplayClass5_0")]
	public sealed class __c__DisplayClass5_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_parentItemMods;

		private static readonly System.IntPtr NativeFieldInfoPtr_newItem;

		private static readonly System.IntPtr NativeFieldInfoPtr_newSlot;

		private static readonly System.IntPtr NativeFieldInfoPtr_newGridShape;

		private static readonly System.IntPtr NativeFieldInfoPtr_newState;

		private static readonly System.IntPtr NativeFieldInfoPtr_parentItemStateMods;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__0_Internal_Void_GameItem_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__1_Internal_Void_GameItem_0;

		public unsafe List<Il2CppSystem.ValueTuple<GridShape, GridShape>> parentItemMods
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentItemMods);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<Il2CppSystem.ValueTuple<GridShape, GridShape>>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentItemMods)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe GameItem newItem
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newItem);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newItem)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe SlotMarker newSlot
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newSlot);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newSlot)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe GridShape newGridShape
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newGridShape);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShape>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newGridShape)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe TagSystem newState
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newState);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TagSystem>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_newState)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe List<Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>>> parentItemStateMods
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentItemStateMods);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<Il2CppSystem.ValueTuple<List<ItemStateModifier>, List<ItemStateModifier>>>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_parentItemStateMods)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass5_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, "<>c__DisplayClass5_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr);
			NativeFieldInfoPtr_parentItemMods = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr, "parentItemMods");
			NativeFieldInfoPtr_newItem = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr, "newItem");
			NativeFieldInfoPtr_newSlot = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr, "newSlot");
			NativeFieldInfoPtr_newGridShape = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr, "newGridShape");
			NativeFieldInfoPtr_newState = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr, "newState");
			NativeFieldInfoPtr_parentItemStateMods = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr, "parentItemStateMods");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr, 100674912);
			NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__0_Internal_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr, 100674913);
			NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__1_Internal_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr, 100674914);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass5_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass5_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350463, XrefRangeEnd = 350466, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _CheckChangeParentShapeStateModifier_b__0(GameItem pi)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)pi);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__0_Internal_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350466, XrefRangeEnd = 350469, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _CheckChangeParentShapeStateModifier_b__1(GameItem pi)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)pi);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__CheckChangeParentShapeStateModifier_b__1_Internal_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c__DisplayClass5_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	[ObfuscatedName("GameInventory+<>c__DisplayClass21_0")]
	public sealed class __c__DisplayClass21_0 : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr_party;

		private static readonly System.IntPtr NativeFieldInfoPtr_vendor;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__GetTotalValue_b__0_Internal_Int64_GameItem_0;

		public unsafe List<GameCharacterItem> party
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_party);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GameCharacterItem>>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_party)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe GameCharacterItem vendor
		{
			get
			{
				nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_vendor);
				System.IntPtr intPtr = *(System.IntPtr*)num;
				return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameCharacterItem>(intPtr) : null;
			}
			set
			{
				System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
				IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_vendor)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c__DisplayClass21_0()
		{
			Il2CppClassPointerStore<__c__DisplayClass21_0>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, "<>c__DisplayClass21_0");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c__DisplayClass21_0>.NativeClassPtr);
			NativeFieldInfoPtr_party = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass21_0>.NativeClassPtr, "party");
			NativeFieldInfoPtr_vendor = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c__DisplayClass21_0>.NativeClassPtr, "vendor");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass21_0>.NativeClassPtr, 100674915);
			NativeMethodInfoPtr__GetTotalValue_b__0_Internal_Int64_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c__DisplayClass21_0>.NativeClassPtr, 100674916);
		}

		[CallerCount(1403)]
		[CachedScanResults(RefRangeStart = 1155, RefRangeEnd = 2558, XrefRangeStart = 1155, XrefRangeEnd = 2558, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe __c__DisplayClass21_0()
			: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<__c__DisplayClass21_0>.NativeClassPtr))
		{
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350469, XrefRangeEnd = 350470, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe long _GetTotalValue_b__0(GameItem item)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__GetTotalValue_b__0_Internal_Int64_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		public __c__DisplayClass21_0(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeFieldInfoPtr_identifier;

	private static readonly System.IntPtr NativeFieldInfoPtr_overrideLockInsert;

	private static readonly System.IntPtr NativeFieldInfoPtr_overrideLockRemove;

	private static readonly System.IntPtr NativeFieldInfoPtr__parent_k__BackingField;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetParentItem_Public_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckAcceptParentShapeStateModifier_Public_Boolean_GameItem_SlotMarker_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckChangeParentShapeStateModifier_Public_Boolean_GameItem_SlotMarker_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckRemoveParentShapeModifier_Public_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Abstract_Virtual_New_Boolean_GameItem_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryInventorySlot_Public_SlotMarker_GameItem_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryInventorySlot_Public_SlotMarker_GameItem_Int32_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryInventorySlot_Public_Abstract_Virtual_New_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Abstract_Virtual_New_SlotMarker_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_HighlightSlot_Public_Abstract_Virtual_New_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ClearHighlight_Public_Abstract_Virtual_New_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_New_List_1_TreeNodeRender_BackgroundColor_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_SupportsGroupPlacement_Public_Virtual_New_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_CheckShapePlacement_Public_Virtual_New_Boolean_GameItem_GridShape_List_1_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_UncheckedAccept_Public_Abstract_Virtual_New_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Expel_Public_Abstract_Virtual_New_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetTotalValue_Public_Int64_List_1_GameCharacterItem_GameCharacterItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_UncheckedAcceptAll_Public_GameInventory_List_1_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ExpelAll_Public_GameInventory_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_parent_Public_get_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_parent_Protected_set_Void_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_childItems_Public_Abstract_Virtual_New_get_List_1_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_parents_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_children_Public_Abstract_Virtual_New_get_List_1_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsAdjacent_Public_Abstract_Virtual_New_Boolean_SlotMarker_SlotMarker_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Validate_Public_Abstract_Virtual_New_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Abstract_Virtual_New_Boolean_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Abstract_Virtual_New_SlotMarker_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_AddParent_Public_Abstract_Virtual_New_Boolean_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RemoveParent_Public_Abstract_Virtual_New_Boolean_GraphNodeStorage_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsInsertLocked_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsRemoveLocked_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_LockInv_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_UnlockInv_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Protected_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__UncheckedAcceptAll_b__22_0_Private_Void_GameItem_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ExpelAll_b__24_0_Private_Void_GameItem_0;

	public unsafe string identifier
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_identifier);
			return IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_identifier)), IL2CPP.ManagedStringToIl2Cpp(value));
		}
	}

	public unsafe bool overrideLockInsert
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_overrideLockInsert);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_overrideLockInsert)) = value;
		}
	}

	public unsafe bool overrideLockRemove
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_overrideLockRemove);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_overrideLockRemove)) = value;
		}
	}

	public unsafe GraphNodeStorage _parent_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__parent_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GraphNodeStorage>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__parent_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe virtual bool SupportsGroupPlacement
	{
		[CallerCount(76)]
		[CachedScanResults(RefRangeStart = 16920, RefRangeEnd = 16996, XrefRangeStart = 16920, XrefRangeEnd = 16996, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_SupportsGroupPlacement_Public_Virtual_New_get_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe GraphNodeStorage parent
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_parent_Public_get_GraphNodeStorage_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GraphNodeStorage>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350796, XrefRangeEnd = 350797, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_parent_Protected_set_Void_GraphNodeStorage_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe virtual List<GameItem> childItems
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_childItems_Public_Abstract_Virtual_New_get_List_1_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GameItem>>(intPtr2) : null;
		}
	}

	public unsafe virtual List<GraphNodeStorage> parents
	{
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350797, XrefRangeEnd = 350806, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_get_children_Public_Abstract_Virtual_New_get_List_1_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<GraphNodeStorage>>(intPtr2) : null;
		}
	}

	static GameInventory()
	{
		Il2CppClassPointerStore<GameInventory>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "GameInventory");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<GameInventory>.NativeClassPtr);
		NativeFieldInfoPtr_identifier = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, "identifier");
		NativeFieldInfoPtr_overrideLockInsert = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, "overrideLockInsert");
		NativeFieldInfoPtr_overrideLockRemove = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, "overrideLockRemove");
		NativeFieldInfoPtr__parent_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, "<parent>k__BackingField");
		NativeMethodInfoPtr_GetParentItem_Public_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674866);
		NativeMethodInfoPtr_CheckAcceptParentShapeStateModifier_Public_Boolean_GameItem_SlotMarker_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674867);
		NativeMethodInfoPtr_CheckChangeParentShapeStateModifier_Public_Boolean_GameItem_SlotMarker_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674868);
		NativeMethodInfoPtr_CheckRemoveParentShapeModifier_Public_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674869);
		NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Abstract_Virtual_New_Boolean_GameItem_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674870);
		NativeMethodInfoPtr_TryInventorySlot_Public_SlotMarker_GameItem_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674871);
		NativeMethodInfoPtr_TryInventorySlot_Public_SlotMarker_GameItem_Int32_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674872);
		NativeMethodInfoPtr_TryInventorySlot_Public_Abstract_Virtual_New_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674873);
		NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Abstract_Virtual_New_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674874);
		NativeMethodInfoPtr_HighlightSlot_Public_Abstract_Virtual_New_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674875);
		NativeMethodInfoPtr_ClearHighlight_Public_Abstract_Virtual_New_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674876);
		NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_New_List_1_TreeNodeRender_BackgroundColor_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674877);
		NativeMethodInfoPtr_get_SupportsGroupPlacement_Public_Virtual_New_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674878);
		NativeMethodInfoPtr_CheckShapePlacement_Public_Virtual_New_Boolean_GameItem_GridShape_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674879);
		NativeMethodInfoPtr_UncheckedAccept_Public_Abstract_Virtual_New_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674880);
		NativeMethodInfoPtr_Expel_Public_Abstract_Virtual_New_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674881);
		NativeMethodInfoPtr_GetTotalValue_Public_Int64_List_1_GameCharacterItem_GameCharacterItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674882);
		NativeMethodInfoPtr_UncheckedAcceptAll_Public_GameInventory_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674883);
		NativeMethodInfoPtr_ExpelAll_Public_GameInventory_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674884);
		NativeMethodInfoPtr_get_parent_Public_get_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674885);
		NativeMethodInfoPtr_set_parent_Protected_set_Void_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674886);
		NativeMethodInfoPtr_get_childItems_Public_Abstract_Virtual_New_get_List_1_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674887);
		NativeMethodInfoPtr_get_parents_Public_Virtual_Final_New_get_List_1_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674888);
		NativeMethodInfoPtr_get_children_Public_Abstract_Virtual_New_get_List_1_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674889);
		NativeMethodInfoPtr_IsAdjacent_Public_Abstract_Virtual_New_Boolean_SlotMarker_SlotMarker_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674890);
		NativeMethodInfoPtr_Validate_Public_Abstract_Virtual_New_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674891);
		NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Abstract_Virtual_New_Boolean_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674892);
		NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Abstract_Virtual_New_SlotMarker_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674893);
		NativeMethodInfoPtr_AddParent_Public_Abstract_Virtual_New_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674894);
		NativeMethodInfoPtr_RemoveParent_Public_Abstract_Virtual_New_Boolean_GraphNodeStorage_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674895);
		NativeMethodInfoPtr_IsInsertLocked_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674896);
		NativeMethodInfoPtr_IsRemoveLocked_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674897);
		NativeMethodInfoPtr_LockInv_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674898);
		NativeMethodInfoPtr_UnlockInv_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674899);
		NativeMethodInfoPtr__ctor_Protected_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674900);
		NativeMethodInfoPtr__UncheckedAcceptAll_b__22_0_Private_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674901);
		NativeMethodInfoPtr__ExpelAll_b__24_0_Private_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameInventory>.NativeClassPtr, 100674902);
	}

	[CallerCount(33)]
	[CachedScanResults(RefRangeStart = 350477, RefRangeEnd = 350510, XrefRangeStart = 350470, XrefRangeEnd = 350477, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItem GetParentItem()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetParentItem_Public_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameItem>(intPtr2) : null;
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 350601, RefRangeEnd = 350605, XrefRangeStart = 350510, XrefRangeEnd = 350601, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CheckAcceptParentShapeStateModifier(GameItem newItem, SlotMarker newSlot, GridShape newGridShape, TagSystem newState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newItem);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newSlot);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newGridShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CheckAcceptParentShapeStateModifier_Public_Boolean_GameItem_SlotMarker_GridShape_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 350696, RefRangeEnd = 350700, XrefRangeStart = 350605, XrefRangeEnd = 350696, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CheckChangeParentShapeStateModifier(GameItem newItem, SlotMarker newSlot, GridShape newGridShape, TagSystem newState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newItem);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newSlot);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newGridShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)newState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CheckChangeParentShapeStateModifier_Public_Boolean_GameItem_SlotMarker_GridShape_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 350722, RefRangeEnd = 350723, XrefRangeStart = 350700, XrefRangeEnd = 350722, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool CheckRemoveParentShapeModifier(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_CheckRemoveParentShapeModifier_Public_Boolean_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	public unsafe virtual bool CheckInventorySlotStillValid(GameItem item, GridShape tempShape, TagSystem tempState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_CheckInventorySlotStillValid_Public_Abstract_Virtual_New_Boolean_GameItem_GridShape_TagSystem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(6)]
	[CachedScanResults(RefRangeStart = 350726, RefRangeEnd = 350732, XrefRangeStart = 350723, XrefRangeEnd = 350726, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe SlotMarker TryInventorySlot(GameItem item, GridShape tempShape, TagSystem itemState = null)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TryInventorySlot_Public_SlotMarker_GameItem_GridShape_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350732, XrefRangeEnd = 350734, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe SlotMarker TryInventorySlot(GameItem item, int requestedNum, GridShape tempShape, TagSystem itemState = null)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &requestedNum;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_TryInventorySlot_Public_SlotMarker_GameItem_Int32_GridShape_TagSystem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe virtual SlotMarker TryInventorySlot(GameItem item, int requestedNum, Vector2 itemGraphicsCenterPosition, GridShape tempShape, TagSystem itemState)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[5];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(int**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &requestedNum;
		*(Vector2**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &itemGraphicsCenterPosition;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)tempShape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)itemState);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_TryInventorySlot_Public_Abstract_Virtual_New_SlotMarker_GameItem_Int32_Vector2_GridShape_TagSystem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe virtual SlotMarker GetIncompleteItemInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_GetIncompleteItemInventorySlot_Public_Abstract_Virtual_New_SlotMarker_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe virtual List<TreeNodeRender> HighlightSlot(SlotMarker slot, RenderHandler.BackgroundColor color, bool doReplace)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot);
		*(RenderHandler.BackgroundColor**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &color;
		*(bool**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &doReplace;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_HighlightSlot_Public_Abstract_Virtual_New_List_1_TreeNodeRender_SlotMarker_BackgroundColor_Boolean_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe virtual void ClearHighlight()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_ClearHighlight_Public_Abstract_Virtual_New_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 350734, XrefRangeEnd = 350740, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual List<TreeNodeRender> HighlightWholeInventory(RenderHandler.BackgroundColor color)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&color);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_HighlightWholeInventory_Public_Virtual_New_List_1_TreeNodeRender_BackgroundColor_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr2) : null;
	}

	[CallerCount(76)]
	[CachedScanResults(RefRangeStart = 16920, RefRangeEnd = 16996, XrefRangeStart = 16920, XrefRangeEnd = 16996, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool CheckShapePlacement(GameItem item, GridShape shape, List<GameItem> ignoredItems)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)shape);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)ignoredItems);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_CheckShapePlacement_Public_Virtual_New_Boolean_GameItem_GridShape_List_1_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	public unsafe virtual bool UncheckedAccept(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_UncheckedAccept_Public_Abstract_Virtual_New_Boolean_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	public unsafe virtual bool Expel(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Expel_Public_Abstract_Virtual_New_Boolean_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 350763, RefRangeEnd = 350766, XrefRangeStart = 350740, XrefRangeEnd = 350763, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe long GetTotalValue(List<GameCharacterItem> party = null, GameCharacterItem vendor = null)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)party);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)vendor);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetTotalValue_Public_Int64_List_1_GameCharacterItem_GameCharacterItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(long*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 350775, RefRangeEnd = 350779, XrefRangeStart = 350766, XrefRangeEnd = 350775, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameInventory UncheckedAcceptAll(List<GameItem> items)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)items);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_UncheckedAcceptAll_Public_GameInventory_List_1_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameInventory>(intPtr2) : null;
	}

	[CallerCount(8)]
	[CachedScanResults(RefRangeStart = 350788, RefRangeEnd = 350796, XrefRangeStart = 350779, XrefRangeEnd = 350788, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameInventory ExpelAll()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ExpelAll_Public_GameInventory_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GameInventory>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe virtual bool IsAdjacent(SlotMarker slot, SlotMarker slot2)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)slot2);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_IsAdjacent_Public_Abstract_Virtual_New_Boolean_SlotMarker_SlotMarker_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
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

	[CallerCount(0)]
	public unsafe virtual bool MayHaveValidInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_MayHaveValidInventorySlot_Public_Abstract_Virtual_New_Boolean_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	public unsafe virtual SlotMarker TryFindOneValidInventorySlot(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_TryFindOneValidInventorySlot_Public_Abstract_Virtual_New_SlotMarker_GameItem_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<SlotMarker>(intPtr2) : null;
	}

	[CallerCount(0)]
	public unsafe virtual bool AddParent(GraphNodeStorage node)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)node);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_AddParent_Public_Abstract_Virtual_New_Boolean_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	public unsafe virtual bool RemoveParent(GraphNodeStorage node)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)node);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_RemoveParent_Public_Abstract_Virtual_New_Boolean_GraphNodeStorage_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 350806, RefRangeEnd = 350811, XrefRangeStart = 350806, XrefRangeEnd = 350806, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsInsertLocked()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsInsertLocked_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 350811, RefRangeEnd = 350814, XrefRangeStart = 350811, XrefRangeEnd = 350811, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsRemoveLocked()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsRemoveLocked_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 350814, RefRangeEnd = 350817, XrefRangeStart = 350814, XrefRangeEnd = 350814, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void LockInv()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_LockInv_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 350817, RefRangeEnd = 350818, XrefRangeStart = 350817, XrefRangeEnd = 350817, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void UnlockInv()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_UnlockInv_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 350819, RefRangeEnd = 350820, XrefRangeStart = 350818, XrefRangeEnd = 350819, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameInventory()
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameInventory>.NativeClassPtr))
	{
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Protected_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	public unsafe void _UncheckedAcceptAll_b__22_0(GameItem item)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)item);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__UncheckedAcceptAll_b__22_0_Private_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	public unsafe void _ExpelAll_b__24_0(GameItem i)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)i);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ExpelAll_b__24_0_Private_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	public GameInventory(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
