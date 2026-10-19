using System;

namespace KillSystem;

[Serializable]
public class RewardOrderModel
{
	public int NpcId;

	public string Name;

	public int RewardOrderLevel;

	public int RewardMoney;

	public int AddReputation;

	public DateTime OverdueTime;

	public DateTime DestroyTime;

	public bool IsGuDing;

	public bool IsComplete;

	public string Place;

	public RewardOrderModel()
	{
		NpcId = -1;
		RewardOrderLevel = -1;
		RewardMoney = -1;
		AddReputation = -1;
	}

	public void SetTarGetPlace(string place)
	{
		Place = place;
	}
}
