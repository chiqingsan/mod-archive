using UnityEngine;
using UnityEngine.EventSystems;
using YSGame;

public class UIHover : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public GameObject ShowObj;

	public AudioClip Sound;

	private void Start()
	{
	}

	private void OnEnable()
	{
		if ((Object)(object)ShowObj != (Object)null)
		{
			ShowObj.SetActive(false);
		}
	}

	private void OnDisable()
	{
		if ((Object)(object)ShowObj != (Object)null)
		{
			ShowObj.SetActive(false);
		}
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
	{
		if ((Object)(object)ShowObj != (Object)null)
		{
			ShowObj.SetActive(true);
		}
		if ((Object)(object)Sound != (Object)null)
		{
			MusicMag.instance.PlayEffectMusic(Sound);
		}
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
	{
		if ((Object)(object)ShowObj != (Object)null)
		{
			ShowObj.SetActive(false);
		}
	}
}
