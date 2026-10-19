using System;

namespace KillSystem.Killer;

[Serializable]
public class KillerModel
{
	public int Id = -1;

	public long TotalRewardMoney = -1L;

	public string DaiHao;

	public int GetLevel()
	{
		if (Id <= 0)
		{
			return 1;
		}
		if (Id == 1)
		{
			return PlayerEx.GetShiLiChengHaoLevel(10);
		}
		return jsonData.instance.AvatarJsonData[Id.ToString()]["ChengHaoID"].I switch
		{
			437 => 1, 
			438 => 2, 
			_ => 3, 
		};
	}
}
