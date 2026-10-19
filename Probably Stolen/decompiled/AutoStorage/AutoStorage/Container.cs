using Il2Cpp;

namespace AutoStorage;

internal sealed class Container
{
	public GameItem Item;

	public GameInventory Inv;

	public string Id => U.SafeId(Item);

	public string Name => U.SafeName(Item);

	public string Key => Cfg.Key(Item);
}
