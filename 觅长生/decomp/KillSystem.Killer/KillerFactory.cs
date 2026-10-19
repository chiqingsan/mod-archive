namespace KillSystem.Killer;

public abstract class KillerFactory
{
	public abstract KillerModel Create(int npcId);
}
