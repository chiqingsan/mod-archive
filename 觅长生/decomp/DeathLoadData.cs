using System;
using System.Runtime.CompilerServices;
using Tab;
using UnityEngine;
using UnityEngine.Events;

public class DeathLoadData : TabLoadPanel
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static UnityAction _003C_003E9__1_0;

		internal void _003CInit_003Eb__1_0()
		{
			UIDeath.Inst.NeedClose = true;
		}
	}

	public DeathLoadData(GameObject go)
		: base(go)
	{
	}

	protected override void Init()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		Get<FpBtn>("关闭").mouseUpEvent.AddListener(new UnityAction(Hide));
		Transform transform = Get("读取/LoadList/ViewPort/Content").transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = ((Component)transform.GetChild(i)).gameObject;
			UnityEvent mouseUpEvent = gameObject.GetComponent<FpBtn>().mouseUpEvent;
			object obj = _003C_003Ec._003C_003E9__1_0;
			if (obj == null)
			{
				UnityAction val = delegate
				{
					UIDeath.Inst.NeedClose = true;
				};
				_003C_003Ec._003C_003E9__1_0 = val;
				obj = (object)val;
			}
			mouseUpEvent.AddListener((UnityAction)obj);
			SaveList.Add(new TabDataBase(gameObject, 1));
		}
	}

	public override void Hide()
	{
		base.Hide();
	}
}
