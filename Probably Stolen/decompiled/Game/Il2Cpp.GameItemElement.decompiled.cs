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

public class GameItemElement : GameItem
{
	[System.Serializable]
	[ObfuscatedName("GameItemElement+<>c")]
	public new sealed class __c : Il2CppSystem.Object
	{
		private static readonly System.IntPtr NativeFieldInfoPtr___9;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__66_0;

		private static readonly System.IntPtr NativeFieldInfoPtr___9__73_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__SortedByPriority_b__66_0_Internal_Int32_ItemSpriteModifier_0;

		private static readonly System.IntPtr NativeMethodInfoPtr__ValidateSelectLocked_b__73_0_Internal_Void_GameItem_0;

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

		public unsafe static Il2CppSystem.Func<ItemSpriteModifier, int> __9__66_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__66_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Func<ItemSpriteModifier, int>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__66_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		public unsafe static Il2CppSystem.Action<GameItem> __9__73_0
		{
			get
			{
				Unsafe.SkipInit(out System.IntPtr intPtr);
				IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr___9__73_0, (void*)(&intPtr));
				System.IntPtr intPtr2 = intPtr;
				return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Il2CppSystem.Action<GameItem>>(intPtr2) : null;
			}
			set
			{
				IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr___9__73_0, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
			}
		}

		static __c()
		{
			Il2CppClassPointerStore<__c>.NativeClassPtr = IL2CPP.GetIl2CppNestedType(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<>c");
			IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<__c>.NativeClassPtr);
			NativeFieldInfoPtr___9 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9");
			NativeFieldInfoPtr___9__66_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__66_0");
			NativeFieldInfoPtr___9__73_0 = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<__c>.NativeClassPtr, "<>9__73_0");
			NativeMethodInfoPtr__ctor_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100675524);
			NativeMethodInfoPtr__SortedByPriority_b__66_0_Internal_Int32_ItemSpriteModifier_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100675525);
			NativeMethodInfoPtr__ValidateSelectLocked_b__73_0_Internal_Void_GameItem_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<__c>.NativeClassPtr, 100675526);
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
		public unsafe int _SortedByPriority_b__66_0(ItemSpriteModifier a)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)a));
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__SortedByPriority_b__66_0_Internal_Int32_ItemSpriteModifier_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}

		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362954, XrefRangeEnd = 362955, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		public unsafe void _ValidateSelectLocked_b__73_0(GameItem n)
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)n);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ValidateSelectLocked_b__73_0_Internal_Void_GameItem_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}

		public __c(System.IntPtr pointer)
			: base(pointer)
		{
		}
	}

	private static readonly System.IntPtr NativeFieldInfoPtr__handler_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__inner_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__fakeInner_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__middle_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__rotator_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__background_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_stackCountText;

	private static readonly System.IntPtr NativeFieldInfoPtr_stackCountTextBackground;

	private static readonly System.IntPtr NativeFieldInfoPtr__fakeMiddle_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__fakeRotator_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr__fakeBackground_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_fakeStackCountText;

	private static readonly System.IntPtr NativeFieldInfoPtr_fakeStackCountTextBackground;

	private static readonly System.IntPtr NativeFieldInfoPtr__tempShape_k__BackingField;

	private static readonly System.IntPtr NativeFieldInfoPtr_spriteMods;

	private static readonly System.IntPtr NativeFieldInfoPtr_fakeSpriteMods;

	private static readonly System.IntPtr NativeFieldInfoPtr_innateSpriteMods;

	private static readonly System.IntPtr NativeFieldInfoPtr_innateFakeSpriteMods;

	private static readonly System.IntPtr NativeFieldInfoPtr_validateLock;

	private static readonly System.IntPtr NativeFieldInfoPtr_noSpriteModifiers;

	private static readonly System.IntPtr NativeFieldInfoPtr_innerNodeBuffer;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_fakeInner_Public_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_fakeInner_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_middle_Public_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_middle_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_rotator_Public_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_rotator_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_background_Public_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_background_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_fakeMiddle_Public_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_fakeMiddle_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_fakeRotator_Public_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_fakeRotator_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_fakeBackground_Public_get_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_fakeBackground_Private_set_Void_TreeNodeRender_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_get_tempShape_Public_get_GridShapeBuilder_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_set_tempShape_Private_set_Void_GridShapeBuilder_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ctor_Public_Void_String_String_String_PixelWindow_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_UpdateRectTransform_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Validate_Public_Virtual_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SortedByPriority_Private_Static_List_1_ItemSpriteModifier_List_1_ItemSpriteModifier_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ResolveSpriteByName_Public_Sprite_String_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ResolveItemSprite_Private_Sprite_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ValidatePartialSprite_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ApplyAnimationFrame_Public_Void_Sprite_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ValidateSelect_Public_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ValidateSelectLocked_Private_Void_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_HandleInnateModSprite_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_UpdateModSprite_Private_Void_Int32_Sprite_Vector2_Int32_Boolean_ItemSpriteModifier_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_UpdateInnateModSprite_Private_Void_Int32_Sprite_Vector2_Int32_Boolean_ItemSpriteModifier_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_UpdateFakeReal_Public_Void_Int32_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ToggleBackground_Public_Void_Boolean_Single_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ToggleFakeBackground_Public_Void_Boolean_Vector4_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_ToggleFakeBackground_Public_Void_Boolean_Single_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GridFlipX_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GridFlipY_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GridRotCCW_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GridRotCW_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FlipX_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_FlipY_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RotCCW_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_RotCW_Public_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetCenteredPosition_Public_ValueTuple_2_Single_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_SetCenteredPosition_Public_Void_Single_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_MovePosition_Public_Void_Single_Single_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetFakeRotatorCenteredPosition_Public_Vector2_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetRotatorCenteredPosition_Public_Vector2_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_Destroy_Public_Virtual_Void_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_IsDestroyed_Public_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_HasTooltip_Public_Virtual_Final_New_Boolean_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetTooltipBasic_Private_RichTextBuilder_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetTooltip_Public_Virtual_New_RichText_0;

	private static readonly System.IntPtr NativeMethodInfoPtr_GetTooltipAdvanced_Public_Virtual_New_RichText_0;

	private static readonly System.IntPtr NativeMethodInfoPtr__ResolveSpriteByName_b__67_0_Private_Boolean_LoadedMod_0;

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

	public unsafe TreeNodeRender _fakeInner_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__fakeInner_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__fakeInner_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender _middle_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__middle_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__middle_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender _rotator_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__rotator_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__rotator_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender _background_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__background_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__background_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender stackCountText
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stackCountText);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stackCountText)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender stackCountTextBackground
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stackCountTextBackground);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_stackCountTextBackground)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender _fakeMiddle_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__fakeMiddle_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__fakeMiddle_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender _fakeRotator_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__fakeRotator_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__fakeRotator_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender _fakeBackground_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__fakeBackground_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__fakeBackground_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender fakeStackCountText
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_fakeStackCountText);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_fakeStackCountText)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe TreeNodeRender fakeStackCountTextBackground
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_fakeStackCountTextBackground);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_fakeStackCountTextBackground)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe GridShapeBuilder _tempShape_k__BackingField
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__tempShape_k__BackingField);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShapeBuilder>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__tempShape_k__BackingField)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<TreeNodeRender> spriteMods
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_spriteMods);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_spriteMods)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<TreeNodeRender> fakeSpriteMods
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_fakeSpriteMods);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_fakeSpriteMods)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<TreeNodeRender> innateSpriteMods
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_innateSpriteMods);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_innateSpriteMods)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe List<TreeNodeRender> innateFakeSpriteMods
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_innateFakeSpriteMods);
			System.IntPtr intPtr = *(System.IntPtr*)num;
			return (intPtr != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr) : null;
		}
		set
		{
			System.IntPtr num = IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			IL2CPP.il2cpp_gc_wbarrier_set_field(num, (System.IntPtr)((nint)num + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_innateFakeSpriteMods)), IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe bool validateLock
	{
		get
		{
			nint num = (nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_validateLock);
			return *(bool*)num;
		}
		set
		{
			*(bool*)((nint)IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this) + (int)IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr_validateLock)) = value;
		}
	}

	public unsafe static List<ItemSpriteModifier> noSpriteModifiers
	{
		get
		{
			Unsafe.SkipInit(out System.IntPtr intPtr);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_noSpriteModifiers, (void*)(&intPtr));
			System.IntPtr intPtr2 = intPtr;
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<ItemSpriteModifier>>(intPtr2) : null;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_noSpriteModifiers, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
		}
	}

	public unsafe static List<TreeNodeRender> innerNodeBuffer
	{
		get
		{
			Unsafe.SkipInit(out System.IntPtr intPtr);
			IL2CPP.il2cpp_field_static_get_value(NativeFieldInfoPtr_innerNodeBuffer, (void*)(&intPtr));
			System.IntPtr intPtr2 = intPtr;
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<TreeNodeRender>>(intPtr2) : null;
		}
		set
		{
			IL2CPP.il2cpp_field_static_set_value(NativeFieldInfoPtr_innerNodeBuffer, (void*)IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value));
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
		[CallerCount(3)]
		[CachedScanResults(RefRangeStart = 362960, RefRangeEnd = 362963, XrefRangeStart = 362955, XrefRangeEnd = 362960, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
		}
	}

	public unsafe virtual int heightPixels
	{
		[CallerCount(2)]
		[CachedScanResults(RefRangeStart = 362968, RefRangeEnd = 362970, XrefRangeStart = 362963, XrefRangeEnd = 362968, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return *(int*)IL2CPP.il2cpp_object_unbox(intPtr2);
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
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362970, XrefRangeEnd = 362971, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
		[CallerCount(12)]
		[CachedScanResults(RefRangeStart = 362971, RefRangeEnd = 362983, XrefRangeStart = 362971, XrefRangeEnd = 362971, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362983, XrefRangeEnd = 362984, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
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

	public unsafe TreeNodeRender fakeInner
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_fakeInner_Public_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362984, XrefRangeEnd = 362985, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_fakeInner_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe TreeNodeRender middle
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_middle_Public_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362985, XrefRangeEnd = 362986, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_middle_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe TreeNodeRender rotator
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_rotator_Public_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362986, XrefRangeEnd = 362987, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_rotator_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe TreeNodeRender background
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_background_Public_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362987, XrefRangeEnd = 362988, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_background_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe TreeNodeRender fakeMiddle
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_fakeMiddle_Public_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362988, XrefRangeEnd = 362989, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_fakeMiddle_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe TreeNodeRender fakeRotator
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_fakeRotator_Public_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362989, XrefRangeEnd = 362990, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_fakeRotator_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe TreeNodeRender fakeBackground
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_fakeBackground_Public_get_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<TreeNodeRender>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362990, XrefRangeEnd = 362991, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_fakeBackground_Private_set_Void_TreeNodeRender_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	public unsafe GridShapeBuilder tempShape
	{
		[CallerCount(0)]
		get
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = null;
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_get_tempShape_Public_get_GridShapeBuilder_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
			return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<GridShapeBuilder>(intPtr2) : null;
		}
		[CallerCount(0)]
		[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 362991, XrefRangeEnd = 362992, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
		set
		{
			IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
			System.IntPtr* ptr = stackalloc System.IntPtr[1];
			*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)value);
			Unsafe.SkipInit(out System.IntPtr intPtr);
			System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_set_tempShape_Private_set_Void_GridShapeBuilder_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
			Il2CppException.RaiseExceptionIfNecessary(intPtr);
		}
	}

	static GameItemElement()
	{
		Il2CppClassPointerStore<GameItemElement>.NativeClassPtr = IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "", "GameItemElement");
		IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr);
		NativeFieldInfoPtr__handler_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<handler>k__BackingField");
		NativeFieldInfoPtr__inner_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<inner>k__BackingField");
		NativeFieldInfoPtr__fakeInner_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<fakeInner>k__BackingField");
		NativeFieldInfoPtr__middle_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<middle>k__BackingField");
		NativeFieldInfoPtr__rotator_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<rotator>k__BackingField");
		NativeFieldInfoPtr__background_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<background>k__BackingField");
		NativeFieldInfoPtr_stackCountText = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "stackCountText");
		NativeFieldInfoPtr_stackCountTextBackground = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "stackCountTextBackground");
		NativeFieldInfoPtr__fakeMiddle_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<fakeMiddle>k__BackingField");
		NativeFieldInfoPtr__fakeRotator_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<fakeRotator>k__BackingField");
		NativeFieldInfoPtr__fakeBackground_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<fakeBackground>k__BackingField");
		NativeFieldInfoPtr_fakeStackCountText = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "fakeStackCountText");
		NativeFieldInfoPtr_fakeStackCountTextBackground = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "fakeStackCountTextBackground");
		NativeFieldInfoPtr__tempShape_k__BackingField = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "<tempShape>k__BackingField");
		NativeFieldInfoPtr_spriteMods = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "spriteMods");
		NativeFieldInfoPtr_fakeSpriteMods = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "fakeSpriteMods");
		NativeFieldInfoPtr_innateSpriteMods = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "innateSpriteMods");
		NativeFieldInfoPtr_innateFakeSpriteMods = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "innateFakeSpriteMods");
		NativeFieldInfoPtr_validateLock = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "validateLock");
		NativeFieldInfoPtr_noSpriteModifiers = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "noSpriteModifiers");
		NativeFieldInfoPtr_innerNodeBuffer = IL2CPP.GetIl2CppField(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, "innerNodeBuffer");
		NativeMethodInfoPtr_get_canResize_Public_Virtual_Final_New_get_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675460);
		NativeMethodInfoPtr_get_widthPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675461);
		NativeMethodInfoPtr_get_heightPixels_Public_Virtual_Final_New_get_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675462);
		NativeMethodInfoPtr_get_handler_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675463);
		NativeMethodInfoPtr_set_handler_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675464);
		NativeMethodInfoPtr_get_rectTransform_Public_Virtual_Final_New_get_RectTransform_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675465);
		NativeMethodInfoPtr_get_inner_Public_Virtual_Final_New_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675466);
		NativeMethodInfoPtr_set_inner_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675467);
		NativeMethodInfoPtr_get_fakeInner_Public_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675468);
		NativeMethodInfoPtr_set_fakeInner_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675469);
		NativeMethodInfoPtr_get_middle_Public_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675470);
		NativeMethodInfoPtr_set_middle_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675471);
		NativeMethodInfoPtr_get_rotator_Public_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675472);
		NativeMethodInfoPtr_set_rotator_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675473);
		NativeMethodInfoPtr_get_background_Public_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675474);
		NativeMethodInfoPtr_set_background_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675475);
		NativeMethodInfoPtr_get_fakeMiddle_Public_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675476);
		NativeMethodInfoPtr_set_fakeMiddle_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675477);
		NativeMethodInfoPtr_get_fakeRotator_Public_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675478);
		NativeMethodInfoPtr_set_fakeRotator_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675479);
		NativeMethodInfoPtr_get_fakeBackground_Public_get_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675480);
		NativeMethodInfoPtr_set_fakeBackground_Private_set_Void_TreeNodeRender_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675481);
		NativeMethodInfoPtr_get_tempShape_Public_get_GridShapeBuilder_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675482);
		NativeMethodInfoPtr_set_tempShape_Private_set_Void_GridShapeBuilder_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675483);
		NativeMethodInfoPtr__ctor_Public_Void_String_String_String_PixelWindow_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675484);
		NativeMethodInfoPtr_ResizePixels_Public_Virtual_Final_New_Void_Int32_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675485);
		NativeMethodInfoPtr_UpdateRectTransform_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675486);
		NativeMethodInfoPtr_Validate_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675487);
		NativeMethodInfoPtr_SortedByPriority_Private_Static_List_1_ItemSpriteModifier_List_1_ItemSpriteModifier_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675488);
		NativeMethodInfoPtr_ResolveSpriteByName_Public_Sprite_String_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675489);
		NativeMethodInfoPtr_ResolveItemSprite_Private_Sprite_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675490);
		NativeMethodInfoPtr_ValidatePartialSprite_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675491);
		NativeMethodInfoPtr_ApplyAnimationFrame_Public_Void_Sprite_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675492);
		NativeMethodInfoPtr_ValidateSelect_Public_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675493);
		NativeMethodInfoPtr_ValidateSelectLocked_Private_Void_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675494);
		NativeMethodInfoPtr_HandleInnateModSprite_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675495);
		NativeMethodInfoPtr_UpdateModSprite_Private_Void_Int32_Sprite_Vector2_Int32_Boolean_ItemSpriteModifier_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675496);
		NativeMethodInfoPtr_UpdateInnateModSprite_Private_Void_Int32_Sprite_Vector2_Int32_Boolean_ItemSpriteModifier_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675497);
		NativeMethodInfoPtr_UpdateFakeReal_Public_Void_Int32_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675498);
		NativeMethodInfoPtr_ToggleBackground_Public_Void_Boolean_Single_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675499);
		NativeMethodInfoPtr_ToggleFakeBackground_Public_Void_Boolean_Vector4_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675500);
		NativeMethodInfoPtr_ToggleFakeBackground_Public_Void_Boolean_Single_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675501);
		NativeMethodInfoPtr_GridFlipX_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675502);
		NativeMethodInfoPtr_GridFlipY_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675503);
		NativeMethodInfoPtr_GridRotCCW_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675504);
		NativeMethodInfoPtr_GridRotCW_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675505);
		NativeMethodInfoPtr_FlipX_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675506);
		NativeMethodInfoPtr_FlipY_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675507);
		NativeMethodInfoPtr_RotCCW_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675508);
		NativeMethodInfoPtr_RotCW_Public_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675509);
		NativeMethodInfoPtr_GetCenteredPosition_Public_ValueTuple_2_Single_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675510);
		NativeMethodInfoPtr_SetCenteredPosition_Public_Void_Single_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675511);
		NativeMethodInfoPtr_MovePosition_Public_Void_Single_Single_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675512);
		NativeMethodInfoPtr_GetFakeRotatorCenteredPosition_Public_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675513);
		NativeMethodInfoPtr_GetRotatorCenteredPosition_Public_Vector2_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675514);
		NativeMethodInfoPtr_Destroy_Public_Virtual_Void_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675515);
		NativeMethodInfoPtr_IsDestroyed_Public_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675516);
		NativeMethodInfoPtr_HasTooltip_Public_Virtual_Final_New_Boolean_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675517);
		NativeMethodInfoPtr_GetTooltipBasic_Private_RichTextBuilder_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675518);
		NativeMethodInfoPtr_GetTooltip_Public_Virtual_New_RichText_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675519);
		NativeMethodInfoPtr_GetTooltipAdvanced_Public_Virtual_New_RichText_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675520);
		NativeMethodInfoPtr__ResolveSpriteByName_b__67_0_Private_Boolean_LoadedMod_0 = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr, 100675522);
	}

	[CallerCount(4)]
	[CachedScanResults(RefRangeStart = 363091, RefRangeEnd = 363095, XrefRangeStart = 362992, XrefRangeEnd = 363091, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe GameItemElement(string identifier, string spriteAtlasPath, string spritePath, PixelWindow window = null)
		: this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GameItemElement>.NativeClassPtr))
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[4];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(identifier);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(spriteAtlasPath);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.ManagedStringToIl2Cpp(spritePath);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)window);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ctor_Public_Void_String_String_String_PixelWindow_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
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

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363134, RefRangeEnd = 363135, XrefRangeStart = 363095, XrefRangeEnd = 363134, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void UpdateRectTransform()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_UpdateRectTransform_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363156, RefRangeEnd = 363157, XrefRangeStart = 363135, XrefRangeEnd = 363156, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void Validate()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Validate_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 363180, RefRangeEnd = 363183, XrefRangeStart = 363157, XrefRangeEnd = 363180, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe static List<ItemSpriteModifier> SortedByPriority(List<ItemSpriteModifier> mods)
	{
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)mods);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SortedByPriority_Private_Static_List_1_ItemSpriteModifier_List_1_ItemSpriteModifier_0, (System.IntPtr)0, (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<List<ItemSpriteModifier>>(intPtr2) : null;
	}

	[CallerCount(6)]
	[CachedScanResults(RefRangeStart = 363197, RefRangeEnd = 363203, XrefRangeStart = 363183, XrefRangeEnd = 363197, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Sprite ResolveSpriteByName(string name)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.ManagedStringToIl2Cpp(name);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ResolveSpriteByName_Public_Sprite_String_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Sprite>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 363203, XrefRangeEnd = 363204, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Sprite ResolveItemSprite()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ResolveItemSprite_Private_Sprite_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<Sprite>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 363204, XrefRangeEnd = 363208, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ValidatePartialSprite()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ValidatePartialSprite_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363214, RefRangeEnd = 363215, XrefRangeStart = 363208, XrefRangeEnd = 363214, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ApplyAnimationFrame(Sprite frame)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)frame);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ApplyAnimationFrame_Public_Void_Sprite_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363217, RefRangeEnd = 363218, XrefRangeStart = 363215, XrefRangeEnd = 363217, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ValidateSelect(bool updateBackground = true)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&updateBackground);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ValidateSelect_Public_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363360, RefRangeEnd = 363361, XrefRangeStart = 363218, XrefRangeEnd = 363360, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ValidateSelectLocked(bool updateBackground)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&updateBackground);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ValidateSelectLocked_Private_Void_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 363392, RefRangeEnd = 363394, XrefRangeStart = 363361, XrefRangeEnd = 363392, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void HandleInnateModSprite()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_HandleInnateModSprite_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 363474, RefRangeEnd = 363476, XrefRangeStart = 363394, XrefRangeEnd = 363474, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void UpdateModSprite(int i, Sprite modSprite, Vector2 modPosition, int modOrientation, bool modFlip, ItemSpriteModifier mod)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[6];
		*ptr = (nint)(&i);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)modSprite);
		*(Vector2**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &modPosition;
		*(int**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &modOrientation;
		*(bool**)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = &modFlip;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)5u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)mod));
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_UpdateModSprite_Private_Void_Int32_Sprite_Vector2_Int32_Boolean_ItemSpriteModifier_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363560, RefRangeEnd = 363561, XrefRangeStart = 363476, XrefRangeEnd = 363560, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void UpdateInnateModSprite(int i, Sprite modSprite, Vector2 modPosition, int modOrientation, bool modFlip, ItemSpriteModifier mod)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[6];
		*ptr = (nint)(&i);
		*(System.IntPtr*)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)modSprite);
		*(Vector2**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &modPosition;
		*(int**)((byte*)ptr + checked((nuint)3u * unchecked((nuint)sizeof(System.IntPtr)))) = &modOrientation;
		*(bool**)((byte*)ptr + checked((nuint)4u * unchecked((nuint)sizeof(System.IntPtr)))) = &modFlip;
		*(System.IntPtr*)((byte*)ptr + checked((nuint)5u * unchecked((nuint)sizeof(System.IntPtr)))) = IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)mod));
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_UpdateInnateModSprite_Private_Void_Int32_Sprite_Vector2_Int32_Boolean_ItemSpriteModifier_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 363575, RefRangeEnd = 363577, XrefRangeStart = 363561, XrefRangeEnd = 363575, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void UpdateFakeReal(int numPickedUp)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = (nint)(&numPickedUp);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_UpdateFakeReal_Public_Void_Int32_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(7)]
	[CachedScanResults(RefRangeStart = 363659, RefRangeEnd = 363666, XrefRangeStart = 363577, XrefRangeEnd = 363659, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ToggleBackground(bool enabled, float opacity = 1f, float lightness = 1f)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = (nint)(&enabled);
		*(float**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &opacity;
		*(float**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &lightness;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ToggleBackground_Public_Void_Boolean_Single_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(3)]
	[CachedScanResults(RefRangeStart = 363744, RefRangeEnd = 363747, XrefRangeStart = 363666, XrefRangeEnd = 363744, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ToggleFakeBackground(bool enabled, Vector4 color)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&enabled);
		*(Vector4**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &color;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ToggleFakeBackground_Public_Void_Boolean_Vector4_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 363748, RefRangeEnd = 363753, XrefRangeStart = 363747, XrefRangeEnd = 363748, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void ToggleFakeBackground(bool enabled, float opacity = 1f, float lightness = 1f)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[3];
		*ptr = (nint)(&enabled);
		*(float**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &opacity;
		*(float**)((byte*)ptr + checked((nuint)2u * unchecked((nuint)sizeof(System.IntPtr)))) = &lightness;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_ToggleFakeBackground_Public_Void_Boolean_Single_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 363753, XrefRangeEnd = 363758, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void GridFlipX()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GridFlipX_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363769, RefRangeEnd = 363770, XrefRangeStart = 363758, XrefRangeEnd = 363769, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void GridFlipY()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GridFlipY_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363782, RefRangeEnd = 363783, XrefRangeStart = 363770, XrefRangeEnd = 363782, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void GridRotCCW()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GridRotCCW_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363795, RefRangeEnd = 363796, XrefRangeStart = 363783, XrefRangeEnd = 363795, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void GridRotCW()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GridRotCW_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363804, RefRangeEnd = 363805, XrefRangeStart = 363796, XrefRangeEnd = 363804, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void FlipX()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FlipX_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363808, RefRangeEnd = 363809, XrefRangeStart = 363805, XrefRangeEnd = 363808, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void FlipY()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_FlipY_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363812, RefRangeEnd = 363813, XrefRangeStart = 363809, XrefRangeEnd = 363812, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RotCCW()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RotCCW_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363816, RefRangeEnd = 363817, XrefRangeStart = 363813, XrefRangeEnd = 363816, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void RotCW()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_RotCW_Public_Void_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(6)]
	[CachedScanResults(RefRangeStart = 363827, RefRangeEnd = 363833, XrefRangeStart = 363817, XrefRangeEnd = 363827, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Il2CppSystem.ValueTuple<float, float> GetCenteredPosition()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr pointer = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetCenteredPosition_Public_ValueTuple_2_Single_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return new Il2CppSystem.ValueTuple<float, float>(pointer);
	}

	[CallerCount(5)]
	[CachedScanResults(RefRangeStart = 363840, RefRangeEnd = 363845, XrefRangeStart = 363833, XrefRangeEnd = 363840, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void SetCenteredPosition(float x, float y)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&x);
		*(float**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &y;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_SetCenteredPosition_Public_Void_Single_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363847, RefRangeEnd = 363848, XrefRangeStart = 363845, XrefRangeEnd = 363847, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe void MovePosition(float x, float y)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[2];
		*ptr = (nint)(&x);
		*(float**)((byte*)ptr + checked((nuint)1u * unchecked((nuint)sizeof(System.IntPtr)))) = &y;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_MovePosition_Public_Void_Single_Single_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(6)]
	[CachedScanResults(RefRangeStart = 363851, RefRangeEnd = 363857, XrefRangeStart = 363848, XrefRangeEnd = 363851, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Vector2 GetFakeRotatorCenteredPosition()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetFakeRotatorCenteredPosition_Public_Vector2_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(Vector2*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(1)]
	[CachedScanResults(RefRangeStart = 363860, RefRangeEnd = 363861, XrefRangeStart = 363857, XrefRangeEnd = 363860, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe Vector2 GetRotatorCenteredPosition()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetRotatorCenteredPosition_Public_Vector2_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(Vector2*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 363861, XrefRangeEnd = 363882, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe override void Destroy()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_Destroy_Public_Virtual_Void_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
	}

	[CallerCount(25)]
	[CachedScanResults(RefRangeStart = 363886, RefRangeEnd = 363911, XrefRangeStart = 363882, XrefRangeEnd = 363886, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool IsDestroyed()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_IsDestroyed_Public_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 363911, XrefRangeEnd = 363914, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual bool HasTooltip()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_HasTooltip_Public_Virtual_Final_New_Boolean_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	[CallerCount(2)]
	[CachedScanResults(RefRangeStart = 364240, RefRangeEnd = 364242, XrefRangeStart = 363914, XrefRangeEnd = 364240, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe RichTextBuilder GetTooltipBasic()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr_GetTooltipBasic_Private_RichTextBuilder_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<RichTextBuilder>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 364242, XrefRangeEnd = 364246, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual RichText GetTooltip()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_GetTooltip_Public_Virtual_New_RichText_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<RichText>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 0, XrefRangeEnd = 0, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe virtual RichText GetTooltipAdvanced()
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = null;
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(IL2CPP.il2cpp_object_get_virtual_method(IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)this), NativeMethodInfoPtr_GetTooltipAdvanced_Public_Virtual_New_RichText_0), IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return (intPtr2 != (System.IntPtr)0) ? Il2CppObjectPool.Get<RichText>(intPtr2) : null;
	}

	[CallerCount(0)]
	[CachedScanResults(RefRangeStart = 0, RefRangeEnd = 0, XrefRangeStart = 364246, XrefRangeEnd = 364248, MetadataInitTokenRva = 0L, MetadataInitFlagRva = 0L)]
	public unsafe bool _ResolveSpriteByName_b__67_0(LoadedMod m)
	{
		IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this);
		System.IntPtr* ptr = stackalloc System.IntPtr[1];
		*ptr = IL2CPP.Il2CppObjectBaseToPtr((Il2CppObjectBase)(object)m);
		Unsafe.SkipInit(out System.IntPtr intPtr);
		System.IntPtr intPtr2 = IL2CPP.il2cpp_runtime_invoke(NativeMethodInfoPtr__ResolveSpriteByName_b__67_0_Private_Boolean_LoadedMod_0, IL2CPP.Il2CppObjectBaseToPtrNotNull((Il2CppObjectBase)(object)this), (void**)ptr, ref intPtr);
		Il2CppException.RaiseExceptionIfNecessary(intPtr);
		return *(bool*)IL2CPP.il2cpp_object_unbox(intPtr2);
	}

	public GameItemElement(System.IntPtr pointer)
		: base(pointer)
	{
	}
}
