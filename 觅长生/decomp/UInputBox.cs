using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UInputBox : MonoBehaviour
{
	public static UInputBox Inst;

	public static bool IsShow;

	private static string prefabPath = "UInputBox";

	private static bool needShow;

	private static string showText = "";

	private static UnityAction<string> OKAction;

	private static List<string> randomInput;

	private static string boxSkin = "";

	public Text ShowText;

	public InputField TextInputField;

	public Button CloseBtn;

	public Button OKBtn;

	public Button RandomBtn;

	public List<GameObject> BoxSkins;

	private void Awake()
	{
		if ((Object)(object)Inst != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		Inst = this;
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
	}

	private void Update()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		if (!needShow)
		{
			return;
		}
		needShow = false;
		ShowText.text = showText;
		TextInputField.text = "";
		((UnityEventBase)OKBtn.onClick).RemoveAllListeners();
		if (OKAction != null)
		{
			((UnityEvent)OKBtn.onClick).AddListener((UnityAction)delegate
			{
				OKAction.Invoke(TextInputField.text);
			});
		}
		if ((Object)(object)RandomBtn != (Object)null)
		{
			((UnityEventBase)RandomBtn.onClick).RemoveAllListeners();
			if (randomInput != null && randomInput.Count > 0)
			{
				((Component)RandomBtn).gameObject.SetActive(true);
				((UnityEvent)RandomBtn.onClick).AddListener(new UnityAction(OnRandomBtnClick));
			}
			else
			{
				((Component)RandomBtn).gameObject.SetActive(false);
			}
		}
		((UnityEvent)OKBtn.onClick).AddListener(new UnityAction(Close));
		((Component)((Component)this).transform.GetChild(0)).gameObject.SetActive(true);
		if (!string.IsNullOrWhiteSpace(boxSkin) && BoxSkins != null && BoxSkins.Count > 0)
		{
			foreach (GameObject boxSkin in BoxSkins)
			{
				boxSkin.gameObject.SetActive(((Object)boxSkin).name == UInputBox.boxSkin);
			}
		}
		IsShow = true;
	}

	private void OnRandomBtnClick()
	{
		int index = Random.Range(0, randomInput.Count);
		string text = randomInput[index];
		TextInputField.text = text;
	}

	private void Close()
	{
		IsShow = false;
		((Component)((Component)this).transform.GetChild(0)).gameObject.SetActive(false);
	}

	public static void Show(string text, UnityAction<string> onOK = null, List<string> randomStrings = null, string skin = "default")
	{
		if ((Object)(object)Inst == (Object)null)
		{
			Object.Instantiate<GameObject>(Resources.Load<GameObject>(prefabPath));
		}
		showText = text;
		OKAction = onOK;
		needShow = true;
		randomInput = randomStrings;
		boxSkin = skin;
	}
}
