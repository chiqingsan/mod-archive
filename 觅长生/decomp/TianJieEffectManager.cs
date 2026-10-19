using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using DG.Tweening;
using JSONClass;
using UnityEngine;
using UnityEngine.UI;
using YSGame;
using YSGame.Fight;

public class TianJieEffectManager : MonoBehaviour
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CPlayMusic_003Ed__18 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncUniTaskVoidMethodBuilder _003C_003Et__builder;

		private Awaiter _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			try
			{
				Awaiter awaiter;
				if (num != 0)
				{
					UniTask val = UniTask.Delay(TimeSpan.FromSeconds(TianJieManager.Inst.WaitTime), true, (PlayerLoopTiming)8, default(CancellationToken));
					awaiter = ((UniTask)(ref val)).GetAwaiter();
					if (!((Awaiter)(ref awaiter)).IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						((AsyncUniTaskVoidMethodBuilder)(ref _003C_003Et__builder)).AwaitUnsafeOnCompleted<Awaiter, _003CPlayMusic_003Ed__18>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(Awaiter);
					num = (_003C_003E1__state = -1);
				}
				((Awaiter)(ref awaiter)).GetResult();
				MusicMag.instance.PlayMusic("渡劫");
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				((AsyncUniTaskVoidMethodBuilder)(ref _003C_003Et__builder)).SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			((AsyncUniTaskVoidMethodBuilder)(ref _003C_003Et__builder)).SetResult();
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			((AsyncUniTaskVoidMethodBuilder)(ref _003C_003Et__builder)).SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
	}

	public static TianJieEffectManager Inst;

	public Animator TianJieYunAnim;

	public Animator LightningAnim;

	public SpriteRenderer LightningBGSR;

	public Animator TianJieNameAnim;

	public Text TianJieIndexText;

	public Text TianJieNameText;

	public Text TianJieCountText;

	public UTooltipTianJieHelpTrigger TianJieTooltip;

	public Text TianJieDescText;

	public Text TianJieIndexText2;

	public Transform PlayerTransform;

	public SpriteRenderer PlayerSR;

	public UIFightAvatarStatus PlayerHPBar;

	public Animator QingTianAnim;

	private FightScreenShake shake;

	private float countCD = 1f;

	private ReplaceDuJieAnimSprite[] replaceSprites;

	public List<ReplaceDuJieAnimSprite> JieYunBGs = new List<ReplaceDuJieAnimSprite>();

	private ReplaceDuJieAnimSprite lastJieYunBG;

	private int jieYunBGIndex;

	private Action attackAnimAction;

	private List<string> tempAnimLeiList = new List<string> { "天雷劫", "罡雷劫", "乾天劫", "五行劫", "风火劫", "天地劫", "灭世劫" };

	private bool playedDieAnim;

	private void Awake()
	{
		Inst = this;
		shake = Object.FindObjectOfType<FightScreenShake>();
	}

	[AsyncStateMachine(typeof(_003CPlayMusic_003Ed__18))]
	public UniTaskVoid PlayMusic()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		_003CPlayMusic_003Ed__18 _003CPlayMusic_003Ed__ = default(_003CPlayMusic_003Ed__18);
		_003CPlayMusic_003Ed__._003C_003Et__builder = AsyncUniTaskVoidMethodBuilder.Create();
		_003CPlayMusic_003Ed__._003C_003E1__state = -1;
		AsyncUniTaskVoidMethodBuilder _003C_003Et__builder = _003CPlayMusic_003Ed__._003C_003Et__builder;
		((AsyncUniTaskVoidMethodBuilder)(ref _003C_003Et__builder)).Start<_003CPlayMusic_003Ed__18>(ref _003CPlayMusic_003Ed__);
		return ((AsyncUniTaskVoidMethodBuilder)(ref _003CPlayMusic_003Ed__._003C_003Et__builder)).Task;
	}

	private void Update()
	{
		countCD -= Time.deltaTime;
		if (countCD < 0f)
		{
			countCD = 0.3f;
			if (PlayerEx.Player != null)
			{
				RefreshXuLiCount();
				RefreshPlayerDieAnim();
			}
		}
	}

	public void Init()
	{
		replaceSprites = ((Component)this).GetComponentsInChildren<ReplaceDuJieAnimSprite>();
	}

	public void SetLeiJieSprite(string leiJie)
	{
		if (replaceSprites != null)
		{
			ReplaceDuJieAnimSprite[] array = replaceSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetSprite(leiJie);
			}
		}
	}

	public void SetLeiJieBG(string leiJie)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		if ((Object)(object)lastJieYunBG != (Object)null)
		{
			((Component)lastJieYunBG).GetComponent<TianJieYunAlpha>().CanChangeAlpha = false;
			lastJieYunBG.SR.DOColor(Color.clear, 0.5f);
		}
		ReplaceDuJieAnimSprite replaceDuJieAnimSprite = JieYunBGs[jieYunBGIndex++];
		if (jieYunBGIndex > 1)
		{
			jieYunBGIndex = 0;
		}
		replaceDuJieAnimSprite.SetSprite(leiJie);
		TianJieYunAlpha jieYunAlpha = ((Component)replaceDuJieAnimSprite).GetComponent<TianJieYunAlpha>();
		jieYunAlpha.CanChangeAlpha = false;
		((Tween)replaceDuJieAnimSprite.SR.DOColor(new Color(1f, 1f, 1f, jieYunAlpha.YunAlpha), 0.5f)).onComplete = (TweenCallback)delegate
		{
			jieYunAlpha.CanChangeAlpha = true;
		};
		lastJieYunBG = replaceDuJieAnimSprite;
		Debug.Log((object)("切换劫云背景到 " + leiJie));
	}

	public void PlayXuLi()
	{
		((MonoBehaviour)this).StartCoroutine(XuLiCoroutine());
	}

	private IEnumerator XuLiCoroutine()
	{
		TianJieNameAnim.Play("Enter");
		string text = (TianJieManager.Inst.LeiJieIndex + 1).ToCNNumber();
		TianJieIndexText.text = "第" + text + "道天劫";
		TianJieIndexText2.text = "第" + text + "道";
		TianJieTooltip.TianJieID = TianJieManager.Inst.NowLeiJie;
		TianJieNameText.text = TianJieManager.Inst.NowLeiJie;
		TianJieLeiJieType tianJieLeiJieType = TianJieLeiJieType.DataDict[TianJieManager.Inst.NowLeiJie];
		TianJieDescText.text = tianJieLeiJieType.CuLue;
		int num = GlobalValue.Get(1597, "天劫蓄力");
		PlayerEx.Player.OtherAvatar.spell.addBuff(3150, num);
		RefreshXuLiCount();
		if (TianJieManager.Inst.LeiJieIndex > 0)
		{
			SetLeiJieSprite(TianJieManager.Inst.NowLeiJie);
			SetLeiJieBG(TianJieManager.Inst.NowLeiJie);
			yield return (object)new WaitForSeconds(0.5f);
		}
		TianJieYunAnim.Play("劫云出现");
		MusicMag.instance.PlayEffectMusic(25);
		Debug.Log((object)"播放劫云出现动画和劫云蓄力动画");
	}

	public void PlayAttack(Action attackAction)
	{
		attackAnimAction = attackAction;
		((MonoBehaviour)this).StartCoroutine(AttackCoroutine());
	}

	private IEnumerator AttackCoroutine()
	{
		Debug.Log((object)$"[{Time.frameCount}]播放劫云攻击动画(云部分) 以及 播放劫云消散动画");
		TianJieYunAnim.Play("劫云攻击");
		int xuLi = TianJieManager.Inst.XuLiCount;
		yield return (object)new WaitForSeconds(2f);
		if (attackAnimAction != null)
		{
			attackAnimAction();
		}
		Debug.Log((object)$"[{Time.frameCount}] 启用血条刷新和伤害数字");
		UIFightPanel.Inst.PlayerStatus.NoRefresh = false;
		MessageMag.Instance.Send(MessageName.MSG_Fight_Effect_Special);
		if (xuLi < 100)
		{
			LightningAnim.Play("弱雷");
		}
		else if (tempAnimLeiList.Contains(TianJieManager.Inst.NowLeiJie))
		{
			LightningAnim.Play(TianJieManager.Inst.NowLeiJie);
		}
		else
		{
			LightningAnim.Play("天雷劫");
		}
		if ((Object)(object)shake != (Object)null)
		{
			float num = (float)xuLi / 100000f;
			shake.ShakePosition = new Vector3(0f, num, 0f);
			shake.Shake();
		}
		if ((Object)(object)LightningBGSR != (Object)null)
		{
			LightningBGSR.color = Color.white;
			LightningBGSR.DOColor(Color.clear, 0.3f);
		}
		MusicMag.instance.PlayEffectMusic(24);
		yield return (object)new WaitForSeconds(1f);
		TianJieNameAnim.Play("Exit");
		TianJieManager.Inst.LeiJieIndex++;
		yield return (object)new WaitForSeconds(1f);
		if (PlayerEx.Player.HP > 0 && TianJieManager.Inst.LeiJieIndex >= TianJieManager.Inst.LeiJieCount)
		{
			TianJieManager.Inst.DuJieSuccess(win: false);
		}
		PlayerEx.Player.OtherAvatar.buffmag.RemoveBuff(3150);
	}

	public void RefreshXuLiCount()
	{
		TianJieCountText.text = TianJieManager.Inst.XuLiCount.ToString();
	}

	public void RefreshPlayerDieAnim()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!playedDieAnim && PlayerHPBar.HP <= 0)
		{
			playedDieAnim = true;
			PlayerSR.DOColor(Color.clear, 5f);
		}
	}
}
