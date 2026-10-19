using System.Collections.Generic;
using KBEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using YSGame;
using YSGame.TuJian;
using script.NewLianDan;

public class UIDeath : MonoBehaviour
{
	public static UIDeath Inst;

	public DeathLoadData LoadData;

	public GameObject ScaleObj;

	public List<GameObject> DeathAnimList;

	public bool NeedClose;

	private bool needShow;

	private DeathType showDeathType;

	private void Awake()
	{
		Inst = this;
		LoadData = new DeathLoadData(((Component)((Component)this).transform.Find("Scale/读档界面")).gameObject);
		SceneManager.activeSceneChanged += delegate
		{
			if (needShow)
			{
				if ((Object)(object)LianQiTotalManager.inst != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)LianQiTotalManager.inst).gameObject);
				}
				if ((Object)(object)LianDanUIMag.Instance != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)LianDanUIMag.Instance).gameObject);
				}
				Show();
			}
			if (NeedClose)
			{
				Close();
			}
		};
	}

	public void Show(DeathType deathType)
	{
		ESCCloseManager.Inst.CloseAll();
		if ((Object)(object)UIAnimProgressBar.Inst != (Object)null && UIAnimProgressBar.IsShow)
		{
			UIAnimProgressBar.Inst.Close();
		}
		TuJianManager.Inst.UnlockDeath((int)deathType);
		showDeathType = deathType;
		needShow = true;
		Tools.instance.loadOtherScenes("DeathScene");
	}

	private void Show()
	{
		if ((Object)(object)FpUIMag.inst != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)FpUIMag.inst).gameObject);
		}
		if ((Object)(object)TpUIMag.inst != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)TpUIMag.inst).gameObject);
		}
		((Component)this).transform.SetAsLastSibling();
		needShow = false;
		MusicMag.instance.StopMusic();
		MusicMag.instance.PlayEffectMusic("Dead");
		foreach (GameObject deathAnim in DeathAnimList)
		{
			deathAnim.SetActive(false);
		}
		ScaleObj.SetActive(true);
		int num = (int)(showDeathType - 1);
		if (DeathAnimList.Count > num)
		{
			try
			{
				DeathAnimList[num].SetActive(true);
				return;
			}
			catch
			{
				Debug.Log((object)"");
				return;
			}
		}
		Debug.LogError((object)$"死亡动画{showDeathType}播放失败，没有将引用加入列表");
	}

	public void OpenLoad()
	{
		LoadData.Show();
	}

	private void Close()
	{
		NeedClose = false;
		LoadData.Hide();
		ScaleObj.SetActive(false);
	}

	public void BackToMainMenu()
	{
		YSSaveGame.Reset();
		KBEngineApp.app.entities[10] = null;
		KBEngineApp.app.entities.Remove(10);
		NeedClose = true;
		Tools.instance.loadOtherScenes("MainMenu");
	}
}
