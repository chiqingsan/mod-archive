using JSONClass;
using UnityEngine;

namespace Tab;

public class TabKeyboardPanel : ISysPanelBase
{
	private bool _isInit;

	private Transform listParent;

	private GameObject keyBoardUI;

	public TabKeyboardPanel(GameObject go)
	{
		_go = go;
		_isInit = false;
	}

	public override void Show()
	{
		if (!_isInit)
		{
			Init();
			_isInit = true;
		}
		_go.SetActive(true);
	}

	private void Init()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		listParent = Get("快捷键列表").transform;
		keyBoardUI = Get("快捷键列表/Temp");
		KeyBoardBase keyBoardBase = null;
		float num = keyBoardUI.transform.localPosition.y;
		foreach (TuJianChunWenBen value in TuJianChunWenBen.DataDict.Values)
		{
			if (value.typenum == 104)
			{
				keyBoardBase = new KeyBoardBase(keyBoardUI.Inst(listParent));
				keyBoardBase.GetTransform().SetLocalPositionY(num);
				keyBoardBase.Init(value.name2, value.descr);
				num -= 38f;
			}
		}
		if (keyBoardBase != null)
		{
			keyBoardBase.下划线.SetActive(false);
		}
	}
}
