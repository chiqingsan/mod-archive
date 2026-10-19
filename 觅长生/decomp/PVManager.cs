using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YSGame;

public class PVManager : MonoBehaviour
{
	public VideoPlayer VideoPlayer;

	public GameObject CheckEscUI;

	public Image 光圈;

	public bool CheckEsc;

	public bool StopCheck;

	public float 开始时间;

	public float 长按时间;

	public float 开始检测ESC时间;

	public float 当前不按ESC时间;

	public float 目标时间 = 2f;

	public float 不按ESC自动隐去时间 = 3f;

	public void PlayPV()
	{
		Init();
		VideoPlayer.Play();
		((MonoBehaviour)this).Invoke("CheckIsPlayComplete", 0.5f);
	}

	private void CheckIsPlayComplete()
	{
		((MonoBehaviour)this).StartCoroutine("VideoCallBack");
	}

	private void Init()
	{
		((MonoBehaviour)this).StopAllCoroutines();
		VideoPlayer.source = (VideoSource)1;
		VideoPlayer.url = Paths.VideoPaths + "/PV.mp4";
		((Component)this).gameObject.SetActive(true);
		VideoPlayer.SetDirectAudioVolume((ushort)0, MusicMag.instance.getBackgroundVoice());
		MusicMag.instance.stopMusic();
		CheckEscUI.SetActive(false);
		CheckEsc = false;
		StopCheck = false;
		开始时间 = 0f;
		长按时间 = 0f;
		开始检测ESC时间 = 0f;
		当前不按ESC时间 = 0f;
		光圈.fillAmount = 0f;
	}

	private IEnumerator VideoCallBack()
	{
		Debug.Log((object)VideoPlayer.isPlaying);
		while (VideoPlayer.isPlaying)
		{
			yield return (object)new WaitForFixedUpdate();
		}
		Close();
	}

	public void Close()
	{
		VideoPlayer.targetTexture.Release();
		VideoPlayer.targetTexture.MarkRestoreExpected();
		StopCheck = true;
		((Component)this).gameObject.SetActive(false);
		((MonoBehaviour)this).StopAllCoroutines();
		MainUIMag.inst.PlayBg();
	}

	private void Update()
	{
		if (StopCheck)
		{
			return;
		}
		if (!CheckEsc && Input.anyKeyDown)
		{
			开始时间 = 0f;
			CheckEsc = true;
			CheckEscUI.SetActive(true);
			开始检测ESC时间 = Time.time;
			Debug.Log((object)"开始检测");
		}
		if (!CheckEsc)
		{
			return;
		}
		if (Input.GetKeyDown((KeyCode)27))
		{
			开始时间 = Time.time;
			开始检测ESC时间 = Time.time;
		}
		if (Input.GetKey((KeyCode)27))
		{
			长按时间 = Time.time - 开始时间;
			if (长按时间 >= 目标时间)
			{
				Close();
			}
		}
		if (Input.GetKeyUp((KeyCode)27))
		{
			长按时间 = 0f;
		}
		光圈.fillAmount = 长按时间 / 目标时间;
		当前不按ESC时间 = Time.time - 开始检测ESC时间;
		if (当前不按ESC时间 >= 不按ESC自动隐去时间)
		{
			开始时间 = 0f;
			CheckEsc = false;
			CheckEscUI.SetActive(false);
		}
	}
}
