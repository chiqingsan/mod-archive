using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using YSGame;

namespace Fungus;

public class Clickable2D : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	[Tooltip("Is object clicking enabled")]
	[SerializeField]
	protected bool clickEnabled = true;

	[Tooltip("Mouse texture to use when hovering mouse over object")]
	[SerializeField]
	protected Texture2D hoverCursor;

	[Tooltip("Use the UI Event System to check for clicks. Clicks that hit an overlapping UI object will be ignored. Camera must have a PhysicsRaycaster component, or a Physics2DRaycaster for 2D colliders.")]
	[SerializeField]
	protected bool useEventSystem;

	public bool ShouldScale = true;

	public AudioClip ClickSound;

	public AudioClip HoverSound;

	private List<string> btnName = new List<string>
	{
		"likai", "caiji", "xiuxi", "biguan", "tupo", "shop", "kefang", "ui8", "yaofang", "shenbingge",
		"wudao", "chuhai", "shanglou", "liexi"
	};

	private bool isIn;

	private bool isCanDo = true;

	private Vector3 oriScale;

	public bool ClickEnabled
	{
		set
		{
			clickEnabled = value;
		}
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		oriScale = ((Component)this).transform.localScale;
	}

	protected virtual void ChangeCursor(Texture2D cursorTexture)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (clickEnabled)
		{
			Cursor.SetCursor(cursorTexture, Vector2.zero, (CursorMode)0);
		}
	}

	protected virtual void DoPointerClick()
	{
		if (clickEnabled && Tools.instance.canClick())
		{
			if ((Object)(object)ClickSound != (Object)null)
			{
				MusicMag.instance.PlayEffectMusic(ClickSound);
			}
			FungusManager.Instance.EventDispatcher.Raise(new ObjectClicked.ObjectClickedEvent(this));
		}
	}

	protected virtual void DoPointerEnter()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		MapComponent component = ((Component)this).GetComponent<MapComponent>();
		bool flag = true;
		if ((Object)(object)component != (Object)null && component.getAvatarNowMapIndex() == component.NodeIndex)
		{
			flag = false;
		}
		if (Tools.instance.canClick() && ShouldScale && flag)
		{
			ChangeCursor(hoverCursor);
			isIn = true;
		}
		if (ShouldScale)
		{
			((Component)this).transform.localScale = oriScale * 1.1f;
		}
		if ((Object)(object)HoverSound != (Object)null)
		{
			MusicMag.instance.PlayEffectMusic(HoverSound);
		}
	}

	protected virtual void DoPointerExit()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		MapComponent component = ((Component)this).GetComponent<MapComponent>();
		bool flag = true;
		if ((Object)(object)component != (Object)null && component.getAvatarNowMapIndex() == component.NodeIndex)
		{
			flag = false;
		}
		if ((Tools.instance.canClick() && ShouldScale && flag) || isIn)
		{
			SetMouseCursor.ResetMouseCursor();
			isIn = false;
		}
		if (ShouldScale)
		{
			((Component)this).transform.localScale = oriScale;
		}
	}

	private void OnMouseDown()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!useEventSystem)
		{
			isCanDo = true;
			if (ShouldScale)
			{
				((Component)this).transform.localScale = oriScale * 0.9f;
			}
		}
	}

	protected virtual void OnMouseUp()
	{
		if (!useEventSystem && isCanDo)
		{
			DoPointerClick();
		}
	}

	protected virtual void OnMouseEnter()
	{
		if (!useEventSystem)
		{
			DoPointerEnter();
		}
	}

	protected virtual void OnMouseExit()
	{
		if (!useEventSystem)
		{
			DoPointerExit();
			isCanDo = false;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (useEventSystem)
		{
			DoPointerClick();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (useEventSystem)
		{
			DoPointerEnter();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (useEventSystem)
		{
			DoPointerExit();
		}
	}
}
