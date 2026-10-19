using UnityEngine;
using UnityEngine.UI;

public class SiXuTypeCell : MonoBehaviour
{
	[SerializeField]
	private Text SiXuContent;

	[SerializeField]
	private Text PingJie;

	[SerializeField]
	private Text OutTime;

	public void setContent(string content, string outTime)
	{
		SiXuContent.text = content;
		OutTime.text = outTime;
	}

	public void SetContent(string content, int quality, string outTime)
	{
		SiXuContent.text = content;
		PingJie.text = $"{quality}品";
		OutTime.text = outTime;
	}
}
