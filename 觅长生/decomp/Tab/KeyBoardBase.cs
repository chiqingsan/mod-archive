using UnityEngine;
using UnityEngine.UI;

namespace Tab;

public class KeyBoardBase : UIBase
{
	public Text 按键;

	public Text 功能;

	public GameObject 下划线;

	public KeyBoardBase(GameObject go)
	{
		_go = go;
		按键 = Get<Text>("按键");
		功能 = Get<Text>("功能");
		下划线 = Get("Line");
		go.SetActive(true);
	}

	public void Init(string key, string dec)
	{
		按键.SetText(key);
		功能.SetText(dec);
	}
}
