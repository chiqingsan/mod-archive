using JSONClass;
using UnityEngine;

namespace KillSystem.Killer;

public class Killer_Factory : KillerFactory
{
	public override KillerModel Create(int npcId)
	{
		int i = NpcJieSuanManager.inst.GetNpcData(npcId)["Level"].I;
		if (!jsonData.instance.AvatarJsonData.HasField(npcId.ToString()))
		{
			Debug.LogError((object)$"不存在NpcId为：{npcId}的Npc");
			return null;
		}
		if (!KillerCreateData.DataDict.ContainsKey(i))
		{
			Debug.LogError((object)$"不能创建境界为：{i}的杀手，表中无此等级");
			return null;
		}
		NpcJieSuanManager.inst.npcSetField.NpcAddGongXian(npcId, KillerCreateData.DataDict[i].ShengWang);
		KillerModel killerModel = new KillerModel
		{
			Id = npcId,
			TotalRewardMoney = KillerCreateData.DataDict[i].ZongShangJin
		};
		jsonData.instance.AvatarRandomJsonData[killerModel.Id.ToString()].SetField("a_mask", killerModel.GetLevel());
		killerModel.DaiHao = GetRandomDaiHao();
		jsonData.instance.AvatarRandomJsonData[killerModel.Id.ToString()].SetField("Name", killerModel.DaiHao);
		return killerModel;
	}

	private string GetRandomDaiHao()
	{
		int randomInt = Tools.instance.GetRandomInt(0, KillerDaiHaoData.DataDict.Count - 1);
		return KillerDaiHaoData.DataList[randomInt].Name;
	}
}
