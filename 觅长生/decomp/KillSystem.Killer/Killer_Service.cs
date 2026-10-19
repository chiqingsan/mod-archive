using UnityEngine;

namespace KillSystem.Killer;

public class Killer_Service : KillerService
{
	private readonly KillerFactory killerFactory;

	public Killer_Service()
	{
		killerFactory = new Killer_Factory();
	}

	public override void CreateKiller(int id)
	{
		KillerModel killerModel = killerFactory.Create(id);
		if (killerModel == null)
		{
			Debug.LogError((object)$"创建杀手失败，id：{id}");
		}
		else
		{
			KillManager.Inst.KillerModels.Add(id, killerModel);
		}
	}

	public override bool RemoveKiller(int id)
	{
		if (KillManager.Inst.KillerModels.ContainsKey(id))
		{
			KillManager.Inst.KillerModels.Remove(id);
			return true;
		}
		return false;
	}
}
