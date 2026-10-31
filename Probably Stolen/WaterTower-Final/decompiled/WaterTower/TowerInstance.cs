using System;
using System.Collections.Generic;

namespace WaterTower;

public sealed class TowerInstance
{
	public object Item;

	public object ZoneCollection;

	public object ZonePurification;

	public object TankSlot;

	public object TankItem;

	public int LastStatusMl = -1;

	public object NoteSlot;

	public object NoteItem;

	public object BottleSlot;

	public List<object> FilterSlots = new List<object>();

	public object FilterBar;

	public object FillButton;

	public int Index;

	public DateTime FirstSeenUtc;

	public bool LayoutChecked;

	public string Label => "水塔#" + Index;

	public bool HasTank => TankItem != null;
}
