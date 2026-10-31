using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;

namespace WaterTower;

public static class NightSettlement
{
	private sealed class FilterPassStats
	{
		public sealed class SlotState
		{
			public int Slot;

			public object Filter;

			public string Id = "空";

			public int DurabilityBefore = -1;

			public int DurabilityAfter = -1;
		}

		public List<SlotState> Slots = new List<SlotState>();

		public int Passes;

		public int PurityBefore = -1;

		public bool VolumeGuardFailed;

		public long VolumeDropUl;

		public int FilledSlots
		{
			get
			{
				int num = 0;
				foreach (SlotState slot in Slots)
				{
					if (slot.Filter != null)
					{
						num++;
					}
				}
				return num;
			}
		}
	}

	private static DateTime _lastRun = DateTime.MinValue;

	private const double MinSecondsBetweenRuns = 3.0;

	private static readonly List<KeyValuePair<string, int>> _notes = new List<KeyValuePair<string, int>>();

	private static readonly List<KeyValuePair<string, int>> _regionNotes = new List<KeyValuePair<string, int>>();

	private static bool _suppressNotes;

	public static void Run()
	{
		if (!WaterTowerConfig.NightSettlementEnabled)
		{
			return;
		}
		DateTime now = DateTime.Now;
		double totalSeconds = (now - _lastRun).TotalSeconds;
		if (totalSeconds < 3.0)
		{
			Diag.Summary("endnight-dup", "夜结：EndNight 在 " + 3.0 + " 秒内重复触发（距上次 " + totalSeconds.ToString("F1") + " 秒），本轮整轮跳过");
			return;
		}
		_lastRun = now;
		List<TowerInstance> all = TowerRegistry.All;
		try
		{
			TowerRegistry.Refresh();
		}
		catch (Exception ex)
		{
			Log.Debug("TowerRegistry.Refresh 异常（已忽略）：" + ex.GetType().Name);
		}
		try
		{
			TowerRegistry.MaintainTowers(verbose: false);
		}
		catch (Exception ex2)
		{
			Diag.Exception("night-maintain", ex2, "夜结前补锁维护异常（已忽略）");
		}
		if (all.Count == 0)
		{
			Log.Debug("水塔夜间结算：还没有水塔，跳过");
			return;
		}
		Log.Section("水塔夜间结算（原版 EndNight 之后，共 " + all.Count + " 台水塔）");
		int num = 0;
		int num2 = 0;
		List<TowerInstance> list = new List<TowerInstance>();
		Dictionary<TowerInstance, string> dictionary = new Dictionary<TowerInstance, string>();
		Dictionary<string, TowerInstance> dictionary2 = new Dictionary<string, TowerInstance>();
		foreach (TowerInstance item in all)
		{
			if (!TowerRegistry.IsAlive(item))
			{
				Log.Info("   [" + item.Label + "] 蓄水箱不在槽里（塔已拆或未建完），跳过");
				num2++;
				continue;
			}
			string text2 = (dictionary[item] = GenKeyOf(TowerRegistry.ResolveTank(item)));
			list.Add(item);
			if (text2 == null)
			{
				continue;
			}
			if (!dictionary2.TryGetValue(text2, out var value))
			{
				dictionary2[text2] = item;
				continue;
			}
			bool flag = !WaterTowerItemFactory.IsItemDestroyed(item.Item);
			bool flag2 = value != null && !WaterTowerItemFactory.IsItemDestroyed(value.Item);
			if ((flag && !flag2) || (flag == flag2 && item.Index > value.Index))
			{
				dictionary2[text2] = item;
			}
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (TowerInstance item2 in list)
		{
			string text3 = dictionary[item2];
			if (text3 != null && dictionary2.TryGetValue(text3, out var value2) && value2 != item2)
			{
				Log.Info("   [" + item2.Label + "] 与 [" + value2.Label + "] 是同一座塔的不同存档代次（蓄水箱 uid 相同），本轮由 [" + value2.Label + "] 结算，本台跳过（残影记录）");
				num2++;
				continue;
			}
			string text4 = TankKey(TowerRegistry.ResolveTank(item2));
			if (!hashSet.Add(text4))
			{
				Log.Info("   [" + item2.Label + "] 与前面某台塔指向同一只蓄水箱（" + text4 + "），跳过（防止重复结算）");
				num2++;
			}
			else
			{
				try
				{
					SettleOne(item2);
					num++;
				}
				catch (Exception ex3)
				{
					Diag.Exception("settle-ex-" + item2.Label, ex3, "[" + item2.Label + "] 夜间结算抛异常（这一晚该塔没有任何产出）；已吞掉，不影响其它塔");
				}
			}
		}
		Log.Info("夜间结算覆盖 " + num + " 台（跳过 " + num2 + " 条重复/失效记录）");
		Log.Flush();
	}

	private static string TankKey(object tank)
	{
		if (tank == null)
		{
			return "<null>";
		}
		try
		{
			Il2CppObjectBase val = (Il2CppObjectBase)((tank is Il2CppObjectBase) ? tank : null);
			if (val != null && val.Pointer != IntPtr.Zero)
			{
				return "ptr:" + ((long)val.Pointer).ToString("x");
			}
		}
		catch
		{
		}
		string text = TowerRegistry.UniqueIdOf(tank);
		if (!string.IsNullOrEmpty(text))
		{
			return "uid:" + text;
		}
		return "ref:" + tank.GetHashCode();
	}

	private static string GenKeyOf(object tank)
	{
		if (tank == null)
		{
			return null;
		}
		string text = TowerRegistry.UniqueIdOf(tank);
		if (!string.IsNullOrEmpty(text))
		{
			return "gen:uid" + text;
		}
		return TankKey(tank);
	}

	private static void SettleOne(TowerInstance tower)
	{
		object obj = TowerRegistry.ResolveTank(tower);
		if (obj == null)
		{
			Diag.Summary("night-no-tank", "[" + tower.Label + "] 蓄水箱不在槽里（被拿走/拆掉/换成别的东西），这一晚跳过");
			return;
		}
		if (tower.TankItem != null && TankKey(tower.TankItem) != TankKey(obj))
		{
			Diag.Summary("tank-rebind-" + tower.Label, "[" + tower.Label + "] 蓄水箱引用自愈：登记的那只（" + TankKey(tower.TankItem) + "）已失效（读档重建/被换过），已改用槽里那只（" + TankKey(obj) + "）");
		}
		tower.TankItem = obj;
		if (WaterTowerItemFactory.IsItemDestroyed(obj))
		{
			Diag.Summary("night-tank-nodes-gone", "[" + tower.Label + "] 蓄水箱的显示元素当前不可用（元素已销毁）→ 仍照常结算（该状态只影响外观刷新，水力/库存不受影响；本次修复就是不再因此跳过整晚）");
		}
		Log.Debug("---- [" + tower.Label + "] ----");
		int num = ReadMl(obj);
		ResetNotes();
		FilterPassStats filterPassStats = BeginFilterPasses(tower, obj);
		int producedByUs = 0;
		int withContainer = 0;
		int machineTotal = 0;
		int pouredCount = 0;
		int requestedTotalUl = 0;
		FillTankFromCollectors(tower, obj, filterPassStats, out producedByUs, out withContainer, out machineTotal, out pouredCount, out requestedTotalUl);
		EndFilterPasses(tower, filterPassStats, obj);
		int purifiedCount = 0;
		int withContainer2 = 0;
		int machineTotal2 = 0;
		PurifyTank(tower, obj, out purifiedCount, out withContainer2, out machineTotal2);
		int num2 = ReadMl(obj);
		int num3 = ReadCapacityMl(obj);
		RestoreTankLook(obj);
		WaterTowerWindow.RefreshStatus(tower);
		string text = NotesText();
		Log.Success("[" + tower.Label + "] 夜间结算完成：补产 " + producedByUs + " ml、净化 " + purifiedCount + " 台；带容器交给原版的机器 " + (withContainer + withContainer2) + " 台；蓄水箱 " + num + " ml → " + num2 + " ml" + text);
		CountPassiveMachines(tower, out var rechargers, out var powerBlocs);
		int num4 = NotesTotal();
		string text2 = NotesReasons();
		string text3 = RegionNotesText();
		int num5 = withContainer + withContainer2;
		int num6 = machineTotal + machineTotal2;
		int num7 = num5 + pouredCount + purifiedCount + num4;
		if (num6 != num7)
		{
			Diag.Once("ledger-mismatch-" + tower.Label, "[" + tower.Label + "] 账本不平（模组缺陷，请连同这一行反馈）：盘点 " + machineTotal + "+" + machineTotal2 + "=" + num6 + " 台，工作 " + num5 + "+" + pouredCount + "+" + purifiedCount + "+" + num4 + "=" + num7 + " 台（带容器+补产+净化+跳过）");
		}
		Diag.Summary("inv-" + tower.Label, "[" + tower.Label + "] 盘点：集水器 " + machineTotal + " 台、净水器 " + machineTotal2 + " 台（水塔结算范围）｜充能器 " + rechargers + " 台、能量阵列 " + powerBlocs + " 台（只放行、不结算）");
		Diag.Summary("work-" + tower.Label, "[" + tower.Label + "] 工作：带容器 " + num5 + " 台（集水 " + withContainer + " / 净水 " + withContainer2 + "，本晚不动作）｜空槽补产 " + pouredCount + " 台、净化 " + purifiedCount + " 台｜跳过 " + num4 + " 台" + ((text2.Length > 0) ? ("（" + text2 + "）") : "（无）") + text3);
		Diag.Summary("night-" + tower.Label, "[" + tower.Label + "] 结算：补产 " + producedByUs + " ml（请求 " + requestedTotalUl / 1000 + " ml）、净化 " + purifiedCount + " 台｜蓄水箱 " + num + " → " + num2 + " / " + num3 + " ml");
	}

	private static void ResetNotes()
	{
		_notes.Clear();
		_regionNotes.Clear();
	}

	private static void Note(string reason)
	{
		if (!_suppressNotes)
		{
			AddCount(_notes, reason);
		}
	}

	private static void RegionNote(string reason)
	{
		if (!_suppressNotes)
		{
			AddCount(_regionNotes, reason);
		}
	}

	private static void AddCount(List<KeyValuePair<string, int>> bucket, string reason)
	{
		if (string.IsNullOrEmpty(reason))
		{
			return;
		}
		for (int i = 0; i < bucket.Count; i++)
		{
			if (bucket[i].Key == reason)
			{
				bucket[i] = new KeyValuePair<string, int>(reason, bucket[i].Value + 1);
				return;
			}
		}
		bucket.Add(new KeyValuePair<string, int>(reason, 1));
	}

	private static string Join(List<KeyValuePair<string, int>> bucket)
	{
		if (bucket.Count == 0)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < bucket.Count; i++)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append("、");
			}
			stringBuilder.Append(bucket[i].Key);
			if (bucket[i].Value > 1)
			{
				stringBuilder.Append("×").Append(bucket[i].Value);
			}
		}
		return stringBuilder.ToString();
	}

	private static int Total(List<KeyValuePair<string, int>> bucket)
	{
		int num = 0;
		for (int i = 0; i < bucket.Count; i++)
		{
			num += bucket[i].Value;
		}
		return num;
	}

	private static string NotesText()
	{
		string text = Join(_notes);
		if (text.Length != 0)
		{
			return " ｜未产原因：" + text;
		}
		return "";
	}

	private static string NotesReasons()
	{
		return Join(_notes);
	}

	private static string RegionNotesText()
	{
		string text = Join(_regionNotes);
		if (text.Length != 0)
		{
			return " ｜区域提示：" + text;
		}
		return "";
	}

	private static int NotesTotal()
	{
		return Total(_notes);
	}

	private static int ReadCapacityMl(object tank)
	{
		if (tank == null)
		{
			return 0;
		}
		Type type = WaterTowerItemFactory.ResolveType("WaterHelper");
		int num = ToInt(WaterTowerItemFactory.CallStatic(type, "GetTotalVolume", new object[1] { tank }, "GetTotalVolume(蓄水箱·算容量)"));
		int num2 = ToInt(WaterTowerItemFactory.CallStatic(type, "GetFreeCapacity", new object[1] { tank }, "GetFreeCapacity(蓄水箱·算容量)"));
		return (num + num2) / 1000;
	}

	private static void CountPassiveMachines(TowerInstance tower, out int rechargers, out int powerBlocs)
	{
		rechargers = 0;
		powerBlocs = 0;
		if (tower == null || tower.Item == null)
		{
			return;
		}
		foreach (object item in TowerRegistry.Descendants(tower.Item))
		{
			if (item != null && item != tower.Item)
			{
				string text = Identifier(item);
				if (text == "recharger")
				{
					rechargers++;
				}
				else if (text == "powerblock")
				{
					powerBlocs++;
				}
			}
		}
	}

	private static List<object> MachinesOf(TowerInstance tower, string identifier)
	{
		List<object> list = new List<object>();
		List<object> list2 = TowerRegistry.Descendants(tower.Item);
		if (list2.Count > 0)
		{
			foreach (object item in list2)
			{
				if (item != null && item != tower.Item)
				{
					if (WaterTowerItemFactory.IsItemDestroyed(item))
					{
						Diag.Once("ghost-machine", "夜结：有机器的显示元素当前不可用（元素已销毁）→ 仍按规则结算（该状态只影响外观刷新；游戏自己的物品扫描也会跳过这类物品）");
					}
					if (Identifier(item) == identifier)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 0)
			{
				return list;
			}
		}
		else
		{
			RegionNote("物品图为空（已退化按分区找机器）");
		}
		foreach (object item2 in MachinesInZone((identifier == "moisture_farm") ? tower.ZoneCollection : tower.ZonePurification))
		{
			if (Identifier(item2) == identifier)
			{
				list.Add(item2);
			}
		}
		return list;
	}

	private static int FillTankFromCollectors(TowerInstance tower, object tank, FilterPassStats filterStats, out int producedByUs, out int withContainer, out int machineTotal, out int pouredCount, out int requestedTotalUl)
	{
		producedByUs = 0;
		withContainer = 0;
		machineTotal = 0;
		pouredCount = 0;
		requestedTotalUl = 0;
		List<object> list = MachinesOf(tower, "moisture_farm");
		if (list.Count == 0)
		{
			RegionNote("集水器 0 台（物品图与分区都没找到）");
			return 0;
		}
		Log.Info("   集水器 " + list.Count + " 台 —— 逐台判定「有容器：只对容器工作（水不进水塔） / 没容器：补产进蓄水箱」");
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (object item in list)
		{
			Log.Debug("   ---- [来源机器] " + MachineLabel(item) + " ----");
			int num4 = ReadMl(tank);
			List<object> list2 = FindMachineContainers(item);
			if (list2.Count > 0)
			{
				withContainer++;
				Log.Info("     [判定] 机器里已有水容器 " + list2.Count + " 个 → **机器只对容器工作，水进水塔这一步跳过**（原版负责产水，本塔这次不搬）");
			}
			else
			{
				int num5 = ReadMl(tank);
				int requestedUl;
				bool moduleGridFound;
				bool flag = DriveCollectorInto(tank, item, filterStats, out requestedUl, out moduleGridFound);
				int num6 = ReadMl(tank) - num5;
				num += num6;
				num2 += requestedUl;
				if (flag)
				{
					num3++;
				}
				Log.Info("     [判定] 机器里没有任何水容器 → 原版不会产水，由水塔补产一次、直接进蓄水箱（" + (flag ? ("成功 +" + num6 + " ml") : "未产出（见上一行原因：没电/已满/产量为 0）") + "）");
			}
			Log.Info("     [本机水量] 蓄水箱 " + num4 + " ml → " + ReadMl(tank) + " ml" + ((list2.Count > 0) ? "（本机按规则不搬）" : ""));
		}
		Log.Info("   本塔合计：补产 " + num + " ml；带容器（只对容器工作）" + withContainer + " 台");
		machineTotal = list.Count;
		pouredCount = num3;
		requestedTotalUl = num2;
		producedByUs = num;
		return num;
	}

	private static List<object> FindMachineContainers(object machine)
	{
		List<object> list = new List<object>();
		HashSet<string> hashSet = new HashSet<string>();
		List<KeyValuePair<string, object>> list2 = MachineSlots(machine);
		if (list2.Count == 0)
		{
			RegionNote("机器槽位读不到（已退化用物品图，有重复产水风险）");
		}
		else
		{
			Log.Info("     探测：内容窗口里识别出 " + list2.Count + " 个槽位");
			object obj = BottleSlotOf(machine, list2);
			if (obj != null)
			{
				Log.Info("       · 水容器槽已识别（= 排除 电池槽 / 模块格 / 便签槽 后的那个槽），只看它里面的东西");
				CollectLiquidContainers(obj, list, hashSet, "");
			}
			else
			{
				RegionNote("水容器槽识别失败（已退化按任一槽判定）");
				foreach (KeyValuePair<string, object> item in list2)
				{
					CollectLiquidContainers(item.Value, list, hashSet, item.Key + "：");
				}
			}
		}
		if (list.Count == 0)
		{
			foreach (object item2 in TowerRegistry.Descendants(machine))
			{
				if (item2 != null && hashSet.Add(TankKey(item2)) && IsLiquidContainer(item2))
				{
					Log.Info("       · （物品图兜底）" + Identifier(item2) + " 是水容器，" + ReadMl(item2) + " ml");
					list.Add(item2);
				}
			}
		}
		return list;
	}

	private static void CollectLiquidContainers(object slot, List<object> containers, HashSet<string> seen, string labelPrefix)
	{
		foreach (object item in SlotItems(slot))
		{
			if (item != null && seen.Add(TankKey(item)))
			{
				bool flag = IsLiquidContainer(item);
				Log.Info("       · " + labelPrefix + Identifier(item) + (flag ? ("（水容器，" + ReadMl(item) + " ml）") : "（非液体容器）"));
				if (flag)
				{
					containers.Add(item);
				}
			}
		}
	}

	private static object BottleSlotOf(object machine, List<KeyValuePair<string, object>> slots)
	{
		Type type = WaterTowerItemFactory.ResolveType("MachineHelper");
		if (type == null)
		{
			return null;
		}
		object obj = WaterTowerItemFactory.CallStatic(type, "GetBatterySlot", new object[1] { machine }, "GetBatterySlot(机器)");
		object obj2 = ModuleInvOf(machine, slots);
		if (obj == null && obj2 == null)
		{
			return null;
		}
		List<object> list = new List<object>();
		foreach (KeyValuePair<string, object> slot in slots)
		{
			object value = slot.Value;
			if (value != null && !WaterTowerItemFactory.SameRef(value, obj) && !WaterTowerItemFactory.SameRef(value, obj2) && WaterTowerWindow.ItemInSlot(value, "postit") == null)
			{
				list.Add(value);
			}
		}
		if (list.Count == 1)
		{
			return list[0];
		}
		if (list.Count == 2)
		{
			object result = null;
			int num = 0;
			foreach (object item in list)
			{
				if (SlotItems(item).Count > 0)
				{
					result = item;
					num++;
				}
			}
			if (num == 1)
			{
				return result;
			}
		}
		object obj3 = null;
		foreach (object item2 in list)
		{
			foreach (object item3 in SlotItems(item2))
			{
				if (IsLiquidContainer(item3))
				{
					if (obj3 != null && !WaterTowerItemFactory.SameRef(obj3, item2))
					{
						return null;
					}
					obj3 = item2;
				}
			}
		}
		return obj3;
	}

	private static List<KeyValuePair<string, object>> MachineSlots(object machine)
	{
		List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
		try
		{
			Il2CppObjectBase val = (Il2CppObjectBase)((machine is Il2CppObjectBase) ? machine : null);
			GameItem val2 = ((val == null) ? null : val.TryCast<GameItem>());
			if (val2 == null)
			{
				Log.Debug("     槽位探测：这台机器不是 GameItem");
				return list;
			}
			PixelWindow contentWindow = val2.contentWindow;
			if (contentWindow == null)
			{
				Log.Debug("     槽位探测：contentWindow 为空（机器窗口还没建立）");
				return list;
			}
			PixelElement childElement = contentWindow.childElement;
			if (childElement == null)
			{
				Log.Debug("     槽位探测：窗口里没有挂元件");
				return list;
			}
			GridPixelElement val3 = ((Il2CppObjectBase)childElement).TryCast<GridPixelElement>();
			if (val3 == null)
			{
				Log.Debug("     槽位探测：窗口挂的元件不是 GridPixelElement（实际 " + ((object)childElement).GetType().Name + "）→ 退化用物品图");
				RegionNote("机器槽位结构变了（已退化用物品图）");
				return list;
			}
			Il2CppReferenceArray<PixelElement> childElements = val3.childElements;
			if (childElements == null)
			{
				Log.Debug("     槽位探测：GridPixelElement.childElements 为空");
				return list;
			}
			int num = 0;
			for (int i = 0; i < ((Il2CppArrayBase<PixelElement>)(object)childElements).Length; i++)
			{
				PixelElement val4 = ((Il2CppArrayBase<PixelElement>)(object)childElements)[i];
				if (val4 != null)
				{
					GameInventory val5 = ((Il2CppObjectBase)val4).TryCast<GameInventory>();
					if (val5 != null)
					{
						list.Add(new KeyValuePair<string, object>("槽" + num, val5));
						num++;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Debug("槽位探测失败（退化用物品图）：" + ex.GetType().Name);
			Diag.Once("slots-ex", "槽位探测抛异常（已退化用物品图找容器）：" + ex.GetType().Name + "；若同时出现「集水器 0 台」或产量为 0，本晚的判定就建立在物品图兜底之上");
		}
		return list;
	}

	private static List<object> SlotItems(object slot)
	{
		List<object> list = new List<object>();
		if (slot == null)
		{
			return list;
		}
		try
		{
			Il2CppObjectBase val = (Il2CppObjectBase)((slot is Il2CppObjectBase) ? slot : null);
			GameInventory val2 = ((val == null) ? null : val.TryCast<GameInventory>());
			if (val2 == null)
			{
				return list;
			}
			List<GameItem> childItems = val2.childItems;
			if (childItems == null)
			{
				return list;
			}
			for (int i = 0; i < childItems.Count; i++)
			{
				list.Add(childItems[i]);
			}
		}
		catch (Exception ex)
		{
			Log.Debug("     读槽内物品失败：" + ex.GetType().Name);
		}
		return list;
	}

	private static string MachineLabel(object machine)
	{
		return Identifier(machine) + " " + TankKey(machine);
	}

	private static bool DriveCollectorInto(object tank, object machine, FilterPassStats filterStats, out int requestedUl, out bool moduleGridFound, string prefix = "   [补产] ", bool turboMode = false)
	{
		requestedUl = 0;
		moduleGridFound = false;
		Type type = WaterTowerItemFactory.ResolveType("WaterHelper");
		Type type2 = WaterTowerItemFactory.ResolveType("MachineryHelper");
		Type type3 = WaterTowerItemFactory.ResolveType("PowerHelper");
		Type type4 = WaterTowerItemFactory.ResolveType("MachineHelper");
		Type type5 = WaterTowerItemFactory.ResolveType("MachineMoistureFarm");
		if (type == null || type2 == null || type3 == null || type4 == null || type5 == null)
		{
			Log.Error(prefix + "类型解析失败，跳过补产");
			Note("补产：类型解析失败");
			return false;
		}
		if (ToInt(WaterTowerItemFactory.CallStatic(type, "GetFreeCapacity", new object[1] { tank }, "GetFreeCapacity(蓄水箱)")) <= 0)
		{
			Log.Info(prefix + "蓄水箱已满，跳过生产");
			Note("蓄水箱已满（集水器）");
			return false;
		}
		object obj = FirstChild(WaterTowerItemFactory.CallStatic(type4, "GetBatterySlot", new object[1] { machine }, "GetBatterySlot(集水器)"));
		if (obj == null)
		{
			Log.Info(prefix + "集水器没有电池，跳过（供电沿用游戏模型，不替它出电）");
			Note("集水器缺电池");
			return false;
		}
		if (WaterTowerItemFactory.IsItemDestroyed(obj))
		{
			Diag.Once("ghost-battery-farm", "夜结：集水器的电池显示元素当前不可用（元素已销毁）→ 仍正常取电补产");
		}
		int num = ToInt(WaterTowerItemFactory.CallStatic(type2, "GetMachinePowerUsage", new object[1] { machine }, "GetMachinePowerUsage(集水器)"));
		object obj2 = WaterTowerItemFactory.CallStatic(type3, "CanDrawPowerSource", new object[2] { obj, num }, "CanDrawPowerSource(电池, " + num + ")");
		bool flag = default(bool);
		int num2;
		if (obj2 is bool)
		{
			flag = (bool)obj2;
			num2 = 1;
		}
		else
		{
			num2 = 0;
		}
		if (((uint)num2 & (flag ? 1u : 0u)) == 0)
		{
			Log.Info(prefix + "集水器电量不足（需要 " + num + "），跳过");
			Note("集水器电量不足");
			return false;
		}
		WaterTowerItemFactory.CallStatic(type3, "DrawPowerSource", new object[2] { obj, num }, "DrawPowerSource(电池, " + num + ")");
		int num3 = ToInt(WaterTowerItemFactory.CallStatic(type5, "GetOutputVolume", new object[1] { machine }, "GetOutputVolume(集水器)"));
		string text = WaterTowerItemFactory.CallStatic(type5, "GetOutputQuality", new object[1] { machine }, "GetOutputQuality(集水器)") as string;
		if (num3 <= 0 || string.IsNullOrEmpty(text))
		{
			Log.Info(prefix + "产量/水质无效（volume=" + num3 + " quality=" + text + "），跳过");
			Note("集水器产量/水质无效");
			return false;
		}
		requestedUl = num3;
		int num4 = ReadPurity(tank);
		int num5 = ReadMl(tank);
		bool flag2 = PourInto(tank, text, num3, filterStats, prefix);
		int num6 = ReadMl(tank);
		object obj3 = null;
		if (flag2)
		{
			obj3 = ModuleInvOf(machine);
			moduleGridFound = obj3 != null;
		}
		Log.Info(prefix + "集水器直接产水进蓄水箱：水质=" + text + "，产量 " + num3 + " µl（" + num3 / 1000 + " ml）→ 实际 +" + (num6 - num5) + " ml，箱内水质 " + num4 + " → " + ReadPurity(tank) + (flag2 ? "" : "（未执行写入：见 Diag 的 pour-* 行）") + ((turboMode && flag2) ? ("；模块格" + (moduleGridFound ? "正常" : "未找到")) : ""));
		if (!flag2)
		{
			return false;
		}
		WaterTowerItemFactory.CallStatic(type4, "OnMachineActioned", new object[2] { machine, obj3 }, "OnMachineActioned(集水器)");
		return true;
	}

	private static object ModuleInvOf(object machine, List<KeyValuePair<string, object>> slots = null)
	{
		Type type = WaterTowerItemFactory.ResolveType("MachineHelper");
		object obj = ((type == null) ? null : WaterTowerItemFactory.CallStatic(type, "GetModuleInv", new object[1] { machine }, "GetModuleInv"));
		if (obj != null)
		{
			return obj;
		}
		try
		{
			if (slots == null)
			{
				slots = MachineSlots(machine);
			}
			object obj2 = null;
			foreach (KeyValuePair<string, object> slot in slots)
			{
				object value = slot.Value;
				Il2CppObjectBase val = (Il2CppObjectBase)((value is Il2CppObjectBase) ? value : null);
				if (((val == null) ? null : val.TryCast<GameGridInventory>()) != null)
				{
					if (obj2 != null && !WaterTowerItemFactory.SameRef(obj2, slot.Value))
					{
						obj2 = null;
						break;
					}
					obj2 = slot.Value;
				}
			}
			if (obj2 != null)
			{
				Diag.Once("moduleinv-fallback", "模块格：MachineHelper.GetModuleInv 返回 null，已用自己的槽位探测兜底找回 —— 否则 OnMachineActioned 会静默空转，「自学习算法」等模块效果当晚不成长");
				return obj2;
			}
		}
		catch (Exception ex)
		{
			Log.Debug("   模块格兜底识别失败：" + ex.GetType().Name);
		}
		Diag.Once("moduleinv-null", "模块格识别失败（GetModuleInv 与兜底都没找到）→ 本台 OnMachineActioned 将空转：「自学习算法」等模块效果的成长当晚不会发生；产水/净化/取电不受影响。玩家反馈“自学习在水塔里不涨”时，这行就是原因");
		return null;
	}

	private static bool PourInto(object tank, string quality, int volumeUl, FilterPassStats filterStats, string prefix = "   [补产] ")
	{
		string text = PourMethodOf(quality);
		if (text == null)
		{
			Diag.Once("pour-unknown-quality", prefix + "水质字符串未识别（" + quality + "）→ 本台跳过、未写入蓄水箱（见 docs\\07 第八节）");
			Note("集水器水质字符串未识别");
			return false;
		}
		Type type = WaterTowerItemFactory.ResolveType("WaterHelper");
		if (type == null)
		{
			Diag.Once("pour-no-helper", prefix + "找不到 WaterHelper，跳过写入");
			Note("补产：类型解析失败");
			return false;
		}
		WaterTowerItemFactory.CallStatic(type, text, new object[2] { tank, volumeUl }, text + "(蓄水箱, " + volumeUl + " µl)");
		RunFilterPass(tank, filterStats);
		return true;
	}

	private static string PourMethodOf(string quality)
	{
		return quality switch
		{
			"Gutterflow" => "AddGutterflow", 
			"Rustwater" => "AddRustWater", 
			"Ghostwater" => "AddGhostWater", 
			"Basewater" => "AddBaseWater", 
			"High Quality Water" => "AddHighQualityWater", 
			"Pure Water" => "AddPureWater", 
			_ => null, 
		};
	}

	private static FilterPassStats BeginFilterPasses(TowerInstance tower, object tank)
	{
		FilterPassStats filterPassStats = new FilterPassStats();
		List<object> list = tower?.FilterSlots;
		if (list == null || list.Count == 0)
		{
			return filterPassStats;
		}
		for (int i = 0; i < list.Count; i++)
		{
			FilterPassStats.SlotState slotState = new FilterPassStats.SlotState
			{
				Slot = i + 1
			};
			try
			{
				slotState.Filter = WaterTowerWindow.FilterInSlot(list[i]);
			}
			catch (Exception ex)
			{
				Log.Debug("BeginFilterPasses：读过滤器槽 " + (i + 1) + " 异常 " + ex.GetType().Name);
				slotState.Filter = null;
			}
			if (slotState.Filter != null)
			{
				slotState.Id = Identifier(slotState.Filter);
				slotState.DurabilityBefore = ReadDurability(slotState.Filter);
			}
			filterPassStats.Slots.Add(slotState);
		}
		if (filterPassStats.FilledSlots > 0)
		{
			filterPassStats.PurityBefore = ReadPurity(tank);
		}
		return filterPassStats;
	}

	private static void EndFilterPasses(TowerInstance tower, FilterPassStats stats, object tank, bool turbo = false)
	{
		if (stats == null || stats.Passes == 0 || stats.FilledSlots == 0 || tower == null)
		{
			return;
		}
		int num = ReadPurity(tank);
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (FilterPassStats.SlotState slot in stats.Slots)
		{
			if (slot.Filter == null)
			{
				list.Add("槽" + slot.Slot + " 空");
				continue;
			}
			list.Add("槽" + slot.Slot + " " + slot.Id + " 耐久 " + slot.DurabilityBefore + " → " + slot.DurabilityAfter);
			if (slot.DurabilityBefore > 0 && slot.DurabilityAfter <= 0)
			{
				list2.Add("槽" + slot.Slot);
			}
		}
		string text = (turbo ? ("[涡轮] " + tower.Label + "：过滤 ") : ("[" + tower.Label + "] 过滤 "));
		Diag.Summary((turbo ? "watertower-turbo-filter-" : "filter-") + tower.Label, text + stats.Passes + " 次×" + stats.FilledSlots + " 支滤芯（" + 3 + " 槽串联）｜" + string.Join("｜", list.ToArray()) + "｜箱内水质 " + stats.PurityBefore + " → " + num + "｜水量守卫" + (stats.VolumeGuardFailed ? "未通过（见 filter-volume-guard）" : ("通过（总水量 -" + stats.VolumeDropUl + " µl）")));
		if (list2.Count > 0)
		{
			string text2 = (turbo ? ("[涡轮] " + tower.Label + "：") : ("[" + tower.Label + "] "));
			Diag.Once((turbo ? "watertower-turbo-filter-dead-" : "filter-dead-") + tower.Label, text2 + "过滤器滤芯耐久耗尽（" + string.Join("、", list2.ToArray()) + "）：耗尽的槽水将原样通过（原版行为），其余槽继续工作；放一个新滤芯即可恢复该槽");
		}
	}

	private static void RunFilterPass(object tank, FilterPassStats stats)
	{
		if (stats == null || stats.FilledSlots == 0 || tank == null)
		{
			return;
		}
		try
		{
			Type type = WaterTowerItemFactory.ResolveType("Liquid");
			Type type2 = WaterTowerItemFactory.ResolveType("WaterHelper");
			Type type3 = WaterTowerItemFactory.ResolveType("ChemicalHelper");
			if (type == null || type2 == null || type3 == null)
			{
				Diag.Once("filter-no-type", "过滤：找不到 Liquid/WaterHelper/ChemicalHelper 类型，本次过滤跳过");
				return;
			}
			object obj = ReadStaticGameMember(type, "LiquidDict");
			if (obj == null)
			{
				Diag.Once("filter-no-liquiddict", "过滤：读不到 Liquid.LiquidDict（静态），本次过滤跳过");
				return;
			}
			List<object> list = ItemRegistry.EnumerateIl2Cpp(WaterTowerItemFactory.GetMemberValue(obj.GetType(), obj, "Values"));
			if (list == null)
			{
				return;
			}
			stats.Passes++;
			long num = ReadTotalVolumeUl(tank);
			HashSet<string> hashSet = new HashSet<string>();
			List<string> list2 = new List<string>();
			int num2 = 0;
			foreach (object item in list)
			{
				if (item == null)
				{
					continue;
				}
				string text = WaterTowerItemFactory.GetMemberValue(item.GetType(), item, "identifier") as string;
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				string text2 = WaterTowerItemFactory.CallStatic(type, "GetCurrentPartFromId", new object[1] { text }, "GetCurrentPartFromId(" + text + ")") as string;
				if (string.IsNullOrEmpty(text2) || !hashSet.Add(text2))
				{
					continue;
				}
				object obj2 = WaterTowerItemFactory.CallInstance(tank.GetType(), tank, "GetTagReadonly", new object[1] { text2 }, "GetTagReadonly(" + text2 + ")");
				if (obj2 == null)
				{
					continue;
				}
				int num3 = ToInt(WaterTowerItemFactory.GetMemberValue(obj2.GetType(), obj2, "valueInt"));
				if (num3 <= 0)
				{
					continue;
				}
				int num4 = num3;
				bool flag = false;
				foreach (FilterPassStats.SlotState slot in stats.Slots)
				{
					if (slot.Filter != null)
					{
						if (!(WaterTowerItemFactory.CallStatic(type2, "HandleFilter", new object[4] { text, num4, null, slot.Filter }, "HandleFilter(" + text + ", " + num4 + ", null, 槽" + slot.Slot + "滤芯)") is int num5))
						{
							Log.Debug("   [过滤] HandleFilter 调用失败（槽" + slot.Slot + "，" + text + "）→ 该成分本遍停止串滤，保留前缀");
							flag = true;
							break;
						}
						num4 = num5;
					}
				}
				if (num4 != num3)
				{
					WaterTowerItemFactory.CallStatic(type3, "SetChemContentUL", new object[3] { tank, text, num4 }, "SetChemContentUL(蓄水箱, " + text + ", " + num4 + ")");
					num2 += num3 - num4;
					list2.Add(text + " " + num3 + " → " + num4 + (flag ? "（链中断）" : ""));
				}
			}
			if (num2 > 0)
			{
				RefreshTankWaterLook(tank);
			}
			foreach (FilterPassStats.SlotState slot2 in stats.Slots)
			{
				if (slot2.Filter != null)
				{
					slot2.DurabilityAfter = ReadDurability(slot2.Filter);
				}
			}
			long num6 = ReadTotalVolumeUl(tank);
			stats.VolumeDropUl += num - num6;
			if (num6 > num || (num2 > 0 && num6 >= num))
			{
				stats.VolumeGuardFailed = true;
				Diag.Once("filter-volume-guard", "[守卫报警] 过滤前后蓄水箱总水量未按预期下降：" + num + " → " + num6 + " µl（本遍削减合计 " + num2 + " µl）—— 写回没落地或写错了成分键，请带着这一行反馈");
			}
			if (list2.Count > 0)
			{
				Log.Debug("   [过滤] 本遍削减：" + string.Join("、", list2.ToArray()));
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("filter-pass", ex, "过滤步骤异常（本次跳过，已忽略）");
		}
	}

	private static void RefreshTankWaterLook(object tank)
	{
		try
		{
			Type type = WaterTowerItemFactory.ResolveType("WaterFeatureHelper");
			if (type != null)
			{
				WaterTowerItemFactory.CallStatic(type, "UpdateWaterFeature", new object[2] { tank, false }, "UpdateWaterFeature(蓄水箱, false)");
			}
			Type type2 = WaterTowerItemFactory.ResolveType("SpriteHelper");
			if (type2 != null)
			{
				WaterTowerItemFactory.CallStatic(type2, "UpdateWaterContainerSprite", new object[1] { tank }, "UpdateWaterContainerSprite(蓄水箱)");
			}
		}
		catch (Exception ex)
		{
			Log.Debug("刷新蓄水箱水特征/贴图异常 " + ex.GetType().Name);
		}
	}

	private static int ReadDurability(object item)
	{
		object obj = WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("DurabilityHelper"), "GetRealDurability", new object[1] { item }, "GetRealDurability(滤芯)");
		if (!(obj is int))
		{
			return -1;
		}
		return (int)obj;
	}

	private static long ReadTotalVolumeUl(object container)
	{
		object obj = WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("WaterHelper"), "GetTotalVolume", new object[1] { container }, "GetTotalVolume(蓄水箱)");
		if (!(obj is int))
		{
			return -1L;
		}
		return (int)obj;
	}

	private static object ReadStaticGameMember(Type type, string name)
	{
		try
		{
			FieldInfo field = type.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj = ((field == null) ? null : field.GetValue(null));
			if (obj != null)
			{
				return UnwrapIl2CppFieldWrapper(obj);
			}
		}
		catch (Exception ex)
		{
			Log.Debug("ReadStaticGameMember（字段 " + name + "）：" + ex.GetType().Name);
		}
		try
		{
			PropertyInfo property = type.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object obj2 = ((property == null) ? null : property.GetValue(null, null));
			if (obj2 != null)
			{
				return UnwrapIl2CppFieldWrapper(obj2);
			}
		}
		catch (Exception ex2)
		{
			Log.Debug("ReadStaticGameMember（属性 " + name + "）：" + ex2.GetType().Name);
		}
		return null;
	}

	private static object UnwrapIl2CppFieldWrapper(object v)
	{
		string name = v.GetType().Name;
		if (name.IndexOf("ReferenceField", StringComparison.Ordinal) >= 0 || name.IndexOf("ValueField", StringComparison.Ordinal) >= 0)
		{
			PropertyInfo property = v.GetType().GetProperty("Value");
			if (!(property == null))
			{
				return property.GetValue(v, null);
			}
			return null;
		}
		return v;
	}

	private static int PurifyTank(TowerInstance tower, object tank, out int purifiedCount, out int withContainer, out int machineTotal)
	{
		purifiedCount = 0;
		withContainer = 0;
		machineTotal = 0;
		List<object> list = MachinesOf(tower, "water_purifier");
		machineTotal = list.Count;
		if (list.Count == 0)
		{
			RegionNote("净水器 0 台（物品图与分区都没找到）");
			return 0;
		}
		int num = 0;
		foreach (object item in list)
		{
			Log.Debug("   ---- [净水器] " + MachineLabel(item) + " ----");
			List<object> list2 = FindMachineContainers(item);
			bool moduleGridFound;
			if (list2.Count > 0)
			{
				withContainer++;
				Log.Info("   [判定] 净水器里已有水容器 " + list2.Count + " 个 → **净水器只对容器工作，水进水塔这一步跳过**（原版负责净化它自己的容器）");
			}
			else if (IsEmpty(tank))
			{
				Log.Info("   [判定] 净水器里没有水容器，但蓄水箱为空，无需净化");
				Note("蓄水箱为空（净水器没有可净化的水）");
			}
			else if (PurifyWithOne(item, tank, out moduleGridFound))
			{
				num++;
			}
		}
		purifiedCount = num;
		return num;
	}

	private static bool PurifyWithOne(object purifier, object tank, out bool moduleGridFound, string prefix = "   ", bool turboMode = false)
	{
		moduleGridFound = false;
		Type type = WaterTowerItemFactory.ResolveType("MachineHelper");
		Type type2 = WaterTowerItemFactory.ResolveType("MachineryHelper");
		Type type3 = WaterTowerItemFactory.ResolveType("PowerHelper");
		Type type4 = WaterTowerItemFactory.ResolveType("MachinePurifier");
		if (type == null || type2 == null || type3 == null || type4 == null)
		{
			Log.Error(prefix + "净水器净化：类型解析失败，跳过");
			Note("净化：类型解析失败");
			return false;
		}
		object obj = FirstChild(WaterTowerItemFactory.CallStatic(type, "GetBatterySlot", new object[1] { purifier }, "GetBatterySlot(净水器)"));
		if (obj == null)
		{
			Log.Info(prefix + "净水器没有电池，跳过（供电沿用游戏模型，不替它出电）");
			Note("净水器缺电池");
			return false;
		}
		if (WaterTowerItemFactory.IsItemDestroyed(obj))
		{
			Diag.Once("ghost-battery-purifier", "夜结：净水器的电池显示元素当前不可用（元素已销毁）→ 仍正常取电净化");
		}
		object obj2 = WaterTowerItemFactory.CallStatic(type2, "GetMachinePowerUsage", new object[1] { purifier }, "GetMachinePowerUsage(净水器)");
		int num = ((obj2 is int) ? ((int)obj2) : 0);
		object obj3 = WaterTowerItemFactory.CallStatic(type3, "CanDrawPowerSource", new object[2] { obj, num }, "CanDrawPowerSource(电池, " + num + ")");
		if (!(obj3 is bool) || !(bool)obj3)
		{
			Log.Info(prefix + "净水器电量不足（需要 " + num + "），跳过");
			Note("净水器电量不足");
			return false;
		}
		WaterTowerItemFactory.CallStatic(type3, "DrawPowerSource", new object[2] { obj, num }, "DrawPowerSource(电池, " + num + ")");
		int num2 = ReadPurity(tank);
		object obj4 = WaterTowerItemFactory.CallStatic(type4, "PurifyContainer", new object[3] { purifier, tank, false }, "PurifyContainer(净水器, 蓄水箱)");
		int num3 = ReadPurity(tank);
		if (!(obj4 is int num4) || num4 <= 0)
		{
			bool flag = obj4 == null;
			string text = (flag ? "调用失败" : "无可除杂质或水质已到这台净水器的上限（返回 0）");
			if (turboMode && !flag)
			{
				object obj5 = ModuleInvOf(purifier);
				moduleGridFound = obj5 != null;
				Log.Info(prefix + "净化蓄水箱：水质 " + num2 + " → " + num3 + "，未变化（" + text + "）；涡轮动作已计入；模块格" + (moduleGridFound ? "正常" : "未找到"));
				WaterTowerItemFactory.CallStatic(type, "OnMachineActioned", new object[2] { purifier, obj5 }, "OnMachineActioned(净水器/涡轮)");
				return true;
			}
			RefundPower(type3, obj, num, purifier, obj4);
			string text2 = (turboMode ? "本次" : "本晚");
			Log.Info(prefix + "净化蓄水箱：水质 " + num2 + " → " + num3 + "，" + text2 + "空转（" + text + "）→ 已退还电量 " + num + "、跳过机器动作");
			Note("净水器空转（已退电）");
			return false;
		}
		string text3 = "";
		if (num3 == num2)
		{
			text3 = "（水质无变化：已到这台净水器能提的上限附近；已移除 " + num4 + " µl 杂质）";
		}
		else if (num3 < num2)
		{
			text3 = "（水质反而下降 —— 异常，请连同这一行反馈）";
		}
		object obj6 = ModuleInvOf(purifier);
		moduleGridFound = obj6 != null;
		Log.Info(prefix + "净化蓄水箱：水质 " + num2 + " → " + num3 + "（移除 " + num4 + " µl）" + text3 + (turboMode ? ("；模块格" + (moduleGridFound ? "正常" : "未找到")) : ""));
		WaterTowerItemFactory.CallStatic(type, "OnMachineActioned", new object[2] { purifier, obj6 }, "OnMachineActioned(净水器)");
		return true;
	}

	private static void RefundPower(Type powerHelper, object battery, int powerUsage, object purifier, object purified)
	{
		try
		{
			WaterTowerItemFactory.CallStatic(powerHelper, "DrawPowerSource", new object[2]
			{
				battery,
				-powerUsage
			}, "DrawPowerSource(电池, -" + powerUsage + ")（空转退电）");
		}
		catch (Exception ex)
		{
			Log.Debug("空转退电失败（电量略亏）：" + ex.GetType().Name);
		}
	}

	public static void RunTurboCycle(object machine)
	{
		try
		{
			if (machine == null)
			{
				return;
			}
			string text = Identifier(machine);
			if (text != "moisture_farm" && text != "water_purifier")
			{
				return;
			}
			TowerInstance towerInstance = FindTowerForMachine(machine);
			if (towerInstance == null)
			{
				return;
			}
			bool suppressNotes = _suppressNotes;
			_suppressNotes = true;
			try
			{
				if (HasWaterContainer(machine))
				{
					return;
				}
				object obj = TowerRegistry.ResolveTank(towerInstance);
				if (obj == null)
				{
					Diag.Once("turbo-no-tank", "涡轮兼容：" + towerInstance.Label + " 的蓄水箱不在槽里，本次涡轮不执行");
					return;
				}
				string text2 = "   [涡轮] " + towerInstance.Label + "：";
				bool moduleGridFound;
				if (text == "moisture_farm")
				{
					FilterPassStats filterPassStats = BeginFilterPasses(towerInstance, obj);
					DriveCollectorInto(obj, machine, filterPassStats, out var _, out moduleGridFound, text2, turboMode: true);
					EndFilterPasses(towerInstance, filterPassStats, obj, turbo: true);
				}
				else
				{
					if (IsEmpty(obj))
					{
						Log.Info(text2 + "蓄水箱为空，跳过净化");
						return;
					}
					PurifyWithOne(machine, obj, out moduleGridFound, text2, turboMode: true);
				}
				RestoreTankLook(obj);
				WaterTowerWindow.RefreshStatusQuiet(towerInstance);
			}
			finally
			{
				_suppressNotes = suppressNotes;
			}
		}
		catch (Exception ex)
		{
			Diag.Exception("turbo-cycle-ex", ex, "涡轮兼容：执行涡轮动作异常（已忽略；原版行为不变）");
		}
	}

	private static TowerInstance FindTowerForMachine(object machine)
	{
		try
		{
			Il2CppObjectBase val = (Il2CppObjectBase)((machine is Il2CppObjectBase) ? machine : null);
			GameItem val2 = ((val == null) ? null : val.TryCast<GameItem>());
			GameInventory val3 = ((val2 == null) ? null : val2.parentInventory);
			if (val3 == null)
			{
				return null;
			}
			List<TowerInstance> all = TowerRegistry.All;
			for (int num = all.Count - 1; num >= 0; num--)
			{
				TowerInstance towerInstance = all[num];
				if (towerInstance != null && (WaterTowerItemFactory.SameRef(val3, towerInstance.ZoneCollection) || WaterTowerItemFactory.SameRef(val3, towerInstance.ZonePurification)))
				{
					return towerInstance;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Debug("涡轮兼容：查找所属水塔失败（忽略）：" + ex.GetType().Name);
		}
		return null;
	}

	private static bool HasWaterContainer(object machine)
	{
		try
		{
			List<KeyValuePair<string, object>> slots = MachineSlots(machine);
			object obj = BottleSlotOf(machine, slots);
			if (obj == null)
			{
				Diag.Once("turbo-bottle-unknown", "涡轮兼容：无法判断机器的水容器槽 → 按有容器跳过（原版行为不变）");
				return true;
			}
			return FirstChild(obj) != null;
		}
		catch (Exception ex)
		{
			Diag.Once("turbo-bottle-ex", "涡轮兼容：读取机器水容器槽异常 → 按有容器跳过（原版行为不变）");
			Log.Debug("涡轮兼容：水容器槽检查异常：" + ex.GetType().Name);
			return true;
		}
	}

	private static void RestoreTankLook(object tank)
	{
		try
		{
			if (!WaterTowerItemFactory.IsItemDestroyed(tank))
			{
				Type type = tank.GetType();
				string text = WaterTowerItemFactory.GetMemberValue(type, tank, "name") as string;
				if (text != "水塔蓄水箱（600L）")
				{
					Log.Warn("   蓄水箱名字被改动过（实际 " + text + "），恢复为 水塔蓄水箱（600L）");
					WaterTowerItemFactory.SetMemberValue(type, tank, "name", "水塔蓄水箱（600L）");
				}
				WaterTowerTankLevelSprite.RefreshNow(tank);
			}
		}
		catch (Exception ex)
		{
			Log.Debug("蓄水箱外观收尾失败：" + ex.GetType().Name);
			Diag.Once("tank-look-ex", "蓄水箱外观收尾抛异常（已忽略）：" + ex.GetType().Name + "（水位显示可能未刷新；不影响水力/库存）");
		}
	}

	private static List<object> MachinesInZone(object zone)
	{
		List<object> list = new List<object>();
		if (zone == null)
		{
			return list;
		}
		foreach (object item in Children(zone))
		{
			if (item != null)
			{
				if (list.Count >= 64)
				{
					RegionNote("机器数超上限 " + 64 + "（其余跳过）");
					break;
				}
				list.Add(item);
			}
		}
		return list;
	}

	private static List<object> Children(object owner)
	{
		try
		{
			Type type = owner.GetType();
			return ItemRegistry.EnumerateIl2Cpp(WaterTowerItemFactory.GetMemberValue(type, owner, "childItems") ?? WaterTowerItemFactory.GetMemberValue(type, owner, "children")) ?? new List<object>();
		}
		catch (Exception ex)
		{
			Diag.Once("children-ex", "读子物品列表抛异常（该次枚举当作空）：" + ex.GetType().Name + "；分区兜底路径可能因此漏机器");
			return new List<object>();
		}
	}

	private static object FirstChild(object owner)
	{
		if (owner == null)
		{
			return null;
		}
		List<object> list = Children(owner);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[0];
	}

	private static string Identifier(object item)
	{
		if (item == null)
		{
			return "<null>";
		}
		try
		{
			return (WaterTowerItemFactory.GetMemberValue(item.GetType(), item, "identifier") as string) ?? "<无名>";
		}
		catch
		{
			return "<读取失败>";
		}
	}

	private static bool IsLiquidContainer(object item)
	{
		return ZoneTagger.HasTag(item, "LIQUID_CONTAINER_TAG");
	}

	private static int ReadMl(object container)
	{
		object obj = WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("WaterHelper"), "GetCurrentCapacityML", new object[1] { container }, "GetCurrentCapacityML");
		if (!(obj is int))
		{
			return 0;
		}
		return (int)obj;
	}

	private static int ReadPurity(object container)
	{
		object obj = WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("WaterHelper"), "GetWaterPurity", new object[1] { container }, "GetWaterPurity");
		if (!(obj is int))
		{
			return -1;
		}
		return (int)obj;
	}

	private static bool IsEmpty(object container)
	{
		object obj = WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("WaterHelper"), "IsEmpty", new object[1] { container }, "WaterHelper.IsEmpty");
		bool flag = default(bool);
		int num;
		if (obj is bool)
		{
			flag = (bool)obj;
			num = ((1 == 0) ? 1 : 0);
		}
		else
		{
			num = 1;
		}
		return (byte)((uint)num | (flag ? 1u : 0u)) != 0;
	}

	private static int ToInt(object v)
	{
		return WaterTowerItemFactory.ToInt(v);
	}
}
