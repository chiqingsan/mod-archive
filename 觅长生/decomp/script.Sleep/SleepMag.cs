using UnityEngine;

namespace script.Sleep;

public class SleepMag : ISleepMag
{
	public override void Sleep()
	{
		if (!NpcJieSuanManager.inst.isCanJieSuan)
		{
			UIPopTip.Inst.Pop("正在结算中，请稍等");
			return;
		}
		int hasTimeDay = PlayerEx.Player.zulinContorl.GetHasTimeDay(Tools.getScreenName());
		if (hasTimeDay <= 0)
		{
			UIPopTip.Inst.Pop("剩余时间不足");
			return;
		}
		int num2 = 0;
		num2 = ((hasTimeDay <= 30) ? hasTimeDay : 30);
		USelectNum.Show("休息{num}天", 1, num2, delegate(int num)
		{
			Resources.Load<GameObject>("talkPrefab/TalkPrefab/talk5000").Inst();
			if (num >= 4)
			{
				PlayerEx.Player.AddHp(PlayerEx.Player.HP_Max);
			}
			else
			{
				PlayerEx.Player.AddHp(PlayerEx.Player.HP_Max * num / 4);
			}
			PlayerEx.Player.AddTime(num);
		});
	}
}
