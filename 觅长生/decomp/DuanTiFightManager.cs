using System.Collections;
using JSONClass;
using UnityEngine;
using UnityEngine.UI;

public class DuanTiFightManager : MonoBehaviour
{
	public static DuanTiFightManager Inst;

	public Text LeiCountText;

	public Text LeiDescText;

	public Text LeiIndexText;

	public Text DuanTiValueText;

	public Image DuanTiBarImage;

	public Animator LeiAtkAnim;

	public Transform PlayerTransform;

	public int LeiSkillID;

	public int LeiDamage;

	public int DamageRecord;

	private int duanTiValue = -1;

	private bool isDuanTiFinish;

	public int DuanTiValue
	{
		get
		{
			return duanTiValue;
		}
		set
		{
			if (duanTiValue != value)
			{
				duanTiValue = value;
				DuanTiBarImage.fillAmount = Mathf.Clamp01((float)duanTiValue / 100f);
				DuanTiValueText.text = $"{duanTiValue}/100";
				if (!isDuanTiFinish && DuanTiValue >= 100)
				{
					OnDuanTiSucc();
				}
			}
		}
	}

	private void Awake()
	{
		Inst = this;
		DuanTiValue = 0;
	}

	private void Start()
	{
	}

	private void Update()
	{
		DuanTiValue = PlayerEx.Player.buffmag.GetBuffSum(4250);
	}

	public void OnPlayerRoundStart()
	{
		CreateLei(RoundManager.instance.StaticRoundNum);
		LeiCountText.text = LeiDamage.ToString();
		LeiIndexText.text = "第" + RoundManager.instance.StaticRoundNum.ToCNNumber() + "道";
		if (_skillJsonData.DataDict.ContainsKey(LeiSkillID))
		{
			_skillJsonData skillJsonData = _skillJsonData.DataDict[LeiSkillID];
			LeiDescText.text = skillJsonData.descr;
		}
		else
		{
			LeiDescText.text = $"错误，不存在ID为{LeiSkillID}的技能";
		}
	}

	public void OnPlayerRoundEnd()
	{
		if (PlayerEx.Player.spell.HasBuff(4259))
		{
			int staticRoundNum = RoundManager.instance.StaticRoundNum;
			int num = 1 + (staticRoundNum - 1) * 4;
			if (num > 0)
			{
				PlayerEx.Player.spell.addBuff(4250, num);
			}
		}
	}

	public void OnDuanTiSucc()
	{
		if (PlayerEx.Player.HP > 0)
		{
			isDuanTiFinish = true;
			((MonoBehaviour)this).StartCoroutine(DuanTiSuccChuZhan());
		}
	}

	public IEnumerator DuanTiSuccChuZhan()
	{
		yield return (object)new WaitForSeconds(3f);
		Tools.instance.getPlayer().OtherAvatar?.die();
		PanelMamager.CanOpenOrClose = true;
		Tools.canClickFlag = true;
		Tools.instance.loadMapScenes(Tools.instance.FinalScene);
	}

	public void CreateLei(int round)
	{
		LeiDamage = 1200 + 100 * (round * (round - 1) - (round - 1) * (round - 2) / 2);
		if (round % 2 == 1)
		{
			LeiSkillID = 1262;
		}
		else if (Random.Range(0, 100) < 50)
		{
			LeiSkillID = 1263;
			LeiDamage = (int)((float)LeiDamage * 0.4f);
		}
		else
		{
			LeiSkillID = 1264;
			LeiDamage = Mathf.Max(0, LeiDamage - 1000);
		}
		LeiSkillID = Tools.instance.getSkillKeyByID(LeiSkillID, PlayerEx.Player);
	}

	public void PlayAtkAnim()
	{
		LeiAtkAnim.Play("DuanTiLeiAtkAnim");
	}
}
