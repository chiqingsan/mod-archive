namespace KillSystem;

public abstract class RewardOrderFactory
{
	public abstract RewardOrderModel Create(int id = 0);

	protected int GetCanKeepTime(int level)
	{
		return level switch
		{
			1 => 20, 
			2 => 80, 
			3 => 200, 
			_ => 1, 
		};
	}

	protected abstract bool CanCreate(int id);
}
