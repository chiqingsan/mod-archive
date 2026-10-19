using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fungus;
using JSONClass;
using KBEngine;
using UnityEngine;
using UnityEngine.UI;
using YSGame;

public class TianJieManager : MonoBehaviour
{
	public static Dictionary<string, string> LeiJieNames = new Dictionary<string, string>
	{
		{ "天雷劫", "tianlei" },
		{ "阴阳劫", "yinyang" },
		{ "风火劫", "fenghuo" },
		{ "心魔劫", "xinmo" },
		{ "罡雷劫", "ganglei" },
		{ "造化劫", "zaohua" },
		{ "混元劫", "hunyuan" },
		{ "五行劫", "wuxing" },
		{ "乾天劫", "qiantian" },
		{ "生死劫", "shengsi" },
		{ "天地劫", "tiandi" },
		{ "灭世劫", "mieshi" }
	};

	public static TianJieManager Inst;

	public TianJieEffectManager EffectManager;

	private bool tianJieFinish;

	public int LeiJieIndex;

	public Text LeiJieNameText;

	public Text LeiJieDamageText;

	[HideInInspector]
	public bool YiXuLi;

	public int WaitTime;

	[HideInInspector]
	public int LeiJieCount;

	[HideInInspector]
	public List<string> LeiJieList = new List<string>();

	[HideInInspector]
	public string NowLeiJie => LeiJieList[LeiJieIndex];

	[HideInInspector]
	public int XuLiCount => PlayerEx.Player.OtherAvatar.buffmag.GetBuffSum(3150) + 1;

	public static void StartTianJieCD()
	{
		Avatar player = PlayerEx.Player;
		player.TianJie.SetField("HuaShenTime", player.worldTimeMag.nowTime);
		player.TianJie.SetField("JiaSuYear", 0);
		OnAddTime();
		player.TianJie.SetField("ShowTianJieCD", val: true);
	}

	public static void TianJieJiaSu(int year)
	{
		int i = PlayerEx.Player.TianJie["JiaSuYear"].I;
		PlayerEx.Player.TianJie.SetField("JiaSuYear", i + year);
		OnAddTime();
	}

	public static void OnAddTime()
	{
		Avatar player = PlayerEx.Player;
		if (player == null)
		{
			return;
		}
		int tianJieShengYuTime = GetTianJieShengYuTime();
		if (tianJieShengYuTime > 0)
		{
			int num = tianJieShengYuTime / 365;
			if (num > 1000)
			{
				player.TianJie.SetField("ShengYuTimeValue", 1000);
			}
			else
			{
				player.TianJie.SetField("ShengYuTimeValue", num);
			}
			if (num > 1000)
			{
				player.TianJie.SetField("ShengYuTime", "1000年");
			}
			else if (num > 0)
			{
				player.TianJie.SetField("ShengYuTime", $"{num}年");
			}
			else
			{
				player.TianJie.SetField("ShengYuTime", "不足1年");
			}
			return;
		}
		player.TianJie.SetField("ShengYuTime", "0年0月0日");
		player.TianJie.SetField("ShowTianJieCD", val: false);
		Debug.Log((object)"开始天劫");
		ESCCloseManager.Inst.CloseAll();
		string nowSceneName = SceneEx.NowSceneName;
		if (nowSceneName == "S74")
		{
			GlobalValue.SetTalk(0, 642, "雷劫_浪方肚子");
			Tools.instance.loadMapScenes("Sea9");
			return;
		}
		if (SceneEx.GetNowSceneType() == 1)
		{
			GlobalValue.SetTalk(0, 640, "雷劫_宁州");
			Tools.instance.loadMapScenes("AllMaps");
			return;
		}
		int num2 = GetNowSeaID.Do();
		GlobalValue.SetTalk(0, 641, "雷劫_海上");
		if (num2 == 0)
		{
			if (SceneNameJsonData.DataDict.ContainsKey(nowSceneName))
			{
				SceneNameJsonData sceneNameJsonData = SceneNameJsonData.DataDict[nowSceneName];
				if (!string.IsNullOrWhiteSpace(sceneNameJsonData.OutsideSceneName))
				{
					if (!(sceneNameJsonData.OutsideSceneName == "LastSence"))
					{
						if (sceneNameJsonData.OutsideSceneName.StartsWith("Sea"))
						{
							LoadFuBen.loadfuben(sceneNameJsonData.OutsideSceneName, sceneNameJsonData.OutsideScenePos);
						}
						else
						{
							Tools.instance.loadMapScenes(sceneNameJsonData.OutsideSceneName);
						}
						return;
					}
					if (!string.IsNullOrWhiteSpace(player.lastScence))
					{
						Tools.instance.loadMapScenes(player.lastScence);
						return;
					}
				}
			}
			Tools.instance.loadMapScenes("Sea2");
		}
		else
		{
			Tools.instance.loadMapScenes($"Sea{num2}");
		}
	}

	public static int GetTianJieShengYuTime()
	{
		Avatar player = PlayerEx.Player;
		if (player.TianJie.HasField("ShowTianJieCD") && player.TianJie["ShowTianJieCD"].b)
		{
			DateTime nowTime = player.worldTimeMag.getNowTime();
			DateTime dateTime = DateTime.Parse(player.TianJie["HuaShenTime"].str).AddYears(1000 - player.TianJie["JiaSuYear"].I);
			if (nowTime >= dateTime)
			{
				return -1;
			}
			return (int)dateTime.Subtract(nowTime).TotalDays;
		}
		return int.MaxValue;
	}

	private void Awake()
	{
		Inst = this;
	}

	private void OnDestroy()
	{
		Inst = null;
	}

	public void InitTianJieData()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		EffectManager.Init();
		LeiJieIndex = 0;
		LeiJieCount = TianJieLeiJieShangHai.DataList.Count;
		for (int i = 0; i < LeiJieCount; i++)
		{
			LeiJieList.Add(RollLeiJie(i));
		}
		EffectManager.SetLeiJieBG(LeiJieList[LeiJieIndex]);
		EffectManager.SetLeiJieSprite(LeiJieList[LeiJieIndex]);
		UniTaskVoid val = EffectManager.PlayMusic();
		((UniTaskVoid)(ref val)).Forget();
	}

	public string RollLeiJie(int leiJieIndex)
	{
		List<QuanZhongItem> list = new List<QuanZhongItem>();
		foreach (TianJieLeiJieType data in TianJieLeiJieType.DataList)
		{
			int num = 0;
			if (data.id == "心魔劫")
			{
				if (PlayerEx.Player.xinjin < 1000)
				{
					num = data.QuanZhongTiSheng[leiJieIndex];
				}
			}
			else if (data.id == "灭世劫")
			{
				int ningZhouShengWang = PlayerEx.GetNingZhouShengWang();
				int seaShengWang = PlayerEx.GetSeaShengWang();
				if (ningZhouShengWang <= -1000 || seaShengWang <= -1000)
				{
					num = data.QuanZhongTiSheng[leiJieIndex];
				}
			}
			int weight = data.QuanZhong[leiJieIndex] + num;
			QuanZhongItem item = new QuanZhongItem(data, weight);
			list.Add(item);
		}
		return (QuanZhongItem.Roll(list, log: true).Obj as TianJieLeiJieType).id;
	}

	public void DuJieSuccess(bool win)
	{
		if (tianJieFinish)
		{
			return;
		}
		tianJieFinish = true;
		if (PlayerEx.Player.buffmag.GetBuffSum(3153) >= 30)
		{
			GlobalValue.Set(1690, 2, "渡劫成功");
		}
		if (win)
		{
			Debug.Log((object)"以战斗胜利方式渡劫成功");
			GlobalValue.Set(1598, 2, "渡劫成功");
			MusicMag.instance.PlayEffectMusic(27);
		}
		else
		{
			Debug.Log((object)"以抗住9道雷劫方式渡劫成功");
			GlobalValue.Set(1598, 1, "渡劫成功");
		}
		UIBarAutoSync[] array = Object.FindObjectsOfType<UIBarAutoSync>();
		for (int num = array.Length - 1; num >= 0; num--)
		{
			Object.Destroy((Object)(object)array[num]);
		}
		((MonoBehaviour)this).StartCoroutine(PlayJieYunXiaoSan());
		((Component)TianJieEffectManager.Inst.QingTianAnim).gameObject.SetActive(true);
		foreach (ReplaceDuJieAnimSprite jieYunBG in TianJieEffectManager.Inst.JieYunBGs)
		{
			((Component)jieYunBG).GetComponent<TianJieYunAlpha>().CanChangeAlpha = false;
		}
		TianJieEffectManager.Inst.TianJieNameAnim.Play("Exit");
		Avatar.UnlockShenXianDouFa(4);
	}

	private IEnumerator PlayJieYunXiaoSan()
	{
		TianJieEffectManager.Inst.TianJieYunAnim.Play("劫云消散");
		foreach (ReplaceDuJieAnimSprite jieYunBG in TianJieEffectManager.Inst.JieYunBGs)
		{
			jieYunBG.SR.DOColor(Color.clear, 0.5f);
		}
		yield return (object)new WaitForSeconds(3f);
		Tools.instance.getPlayer().OtherAvatar?.die();
		PanelMamager.CanOpenOrClose = true;
		Tools.canClickFlag = true;
		Tools.instance.loadMapScenes(Tools.instance.FinalScene);
	}
}
