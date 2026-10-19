using System.Collections.Generic;

namespace KillSystem;

public class RewardOrderFactory_PoolService : RewardOrderFactoryPoolService
{
	private readonly Dictionary<string, RewardOrderFactory> dict = new Dictionary<string, RewardOrderFactory>();

	public override RewardOrderFactory Get<T>()
	{
		string name = typeof(T).Name;
		if (!dict.ContainsKey(name))
		{
			dict.Add(name, new T());
		}
		return dict[name];
	}
}
