using Fungus;
using UnityEngine;

namespace Fight;

public class FightResultMag : MonoBehaviour
{
	public static FightResultMag inst;

	public bool IsComplete;

	private void Awake()
	{
		inst = this;
		IsComplete = false;
	}

	public void ShowVictory()
	{
		IsComplete = true;
		((MonoBehaviour)this).Invoke("LaterShowVictory", 1f);
	}

	private void LaterShowVictory()
	{
		if (Tools.instance.monstarMag.FightType != StartFight.FightEnumType.FeiSheng)
		{
			if (Tools.instance.monstarMag.CanDrowpItem())
			{
				ResManager.inst.LoadPrefab("VictoryPanel").Inst();
			}
			else
			{
				VictoryClick();
			}
		}
	}

	public void VictoryClick()
	{
		Tools.instance.getPlayer().OtherAvatar?.die();
		PanelMamager.CanOpenOrClose = true;
		Tools.canClickFlag = true;
		Tools.instance.loadMapScenes(Tools.instance.FinalScene);
	}
}
