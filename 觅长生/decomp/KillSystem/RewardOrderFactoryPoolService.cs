namespace KillSystem;

public abstract class RewardOrderFactoryPoolService
{
	public abstract RewardOrderFactory Get<T>() where T : RewardOrderFactory, new();
}
