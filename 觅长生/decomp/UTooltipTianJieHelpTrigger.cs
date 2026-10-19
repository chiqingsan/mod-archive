using JSONClass;
using UnityEngine;
using UnityEngine.EventSystems;

public class UTooltipTianJieHelpTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public string TianJieID;

	private bool isShow;

	private void Awake()
	{
		MessageMag.Instance.Register(MessageName.MSG_APP_OnFocusChanged, OnFocusChanged);
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		MessageMag.Instance.Remove(MessageName.MSG_APP_OnFocusChanged, OnFocusChanged);
	}

	public void OnFocusChanged(MessageData data)
	{
		if (isShow)
		{
			isShow = false;
			UToolTip.Close();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (TianJieLeiJieType.DataDict.ContainsKey(TianJieID))
		{
			UToolTip.Show(TianJieLeiJieType.DataDict[TianJieID].XiangXi);
			isShow = true;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isShow = false;
		UToolTip.Close();
	}
}
