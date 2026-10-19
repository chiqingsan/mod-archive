namespace WaterTower;

public sealed class TowerInstance
{
	public object Item;

	public object ZoneCollection;

	public object ZonePurification;

	public object TankSlot;

	public object TankItem;

	public object NoteSlot;

	public object NoteItem;

	public object BottleSlot;

	public object FillButton;

	public int Index;

	public string Label => "水塔#" + Index;

	public bool HasTank => TankItem != null;
}
