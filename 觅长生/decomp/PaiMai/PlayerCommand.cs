using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using YSGame;

namespace PaiMai;

public class PlayerCommand : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static UnityAction _003C_003E9__10_0;

		internal void _003CAddMouseEnterListener_003Eb__10_0()
		{
			MusicMag.instance.PlayEffectMusic("Slip - Wood");
		}
	}

	private FpBtn _btn;

	private Animator _animator;

	public CommandType CommandType;

	public CeLueType CeLueType;

	private UnityAction _toBack;

	public bool CanClick { get; private set; }

	public void AddClickAction(UnityAction action)
	{
		if ((Object)(object)_btn == (Object)null)
		{
			_btn = ((Component)this).GetComponentInChildren<FpBtn>();
			_btn.mouseUpEvent.AddListener(action);
		}
		if ((Object)(object)_animator == (Object)null)
		{
			_animator = ((Component)this).GetComponent<Animator>();
		}
	}

	public void AddMouseEnterListener(UnityAction action)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		if ((Object)(object)_btn == (Object)null)
		{
			_btn = ((Component)this).GetComponentInChildren<FpBtn>();
		}
		_btn.mouseEnterEvent.AddListener(action);
		UnityEvent mouseEnterEvent = _btn.mouseEnterEvent;
		object obj = _003C_003Ec._003C_003E9__10_0;
		if (obj == null)
		{
			UnityAction val = delegate
			{
				MusicMag.instance.PlayEffectMusic("Slip - Wood");
			};
			_003C_003Ec._003C_003E9__10_0 = val;
			obj = (object)val;
		}
		mouseEnterEvent.AddListener((UnityAction)obj);
	}

	public void AddMouseOutsListener(UnityAction action)
	{
		if ((Object)(object)_btn == (Object)null)
		{
			_btn = ((Component)this).GetComponentInChildren<FpBtn>();
		}
		_btn.mouseOutEvent.AddListener(action);
	}

	public void PlayToBack()
	{
		_btn.SetCanClick(flag: false);
		CanClick = false;
		_animator.Play("ToBack");
	}

	public void PlayBackTo()
	{
		_animator.Play("BackTo");
	}

	public void BackTo()
	{
		CanClick = true;
		_btn.SetCanClick(flag: true);
	}
}
