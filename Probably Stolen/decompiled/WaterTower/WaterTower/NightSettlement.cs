using System;
using System.Collections.Generic;
using System.Text;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;

namespace WaterTower;

public static class NightSettlement
{
	private static DateTime _lastRun = DateTime.MinValue;

	private const double MinSecondsBetweenRuns = 3.0;

	private static readonly List<KeyValuePair<string, int>> _notes = new List<KeyValuePair<string, int>>();

	private static readonly List<KeyValuePair<string, int>> _regionNotes = new List<KeyValuePair<string, int>>();

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
		return string.IsNullOrEmpty(text) ? TankKey(tank) : ("gen:uid" + text);
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
		int producedByUs = 0;
		int withContainer = 0;
		int machineTotal = 0;
		int pouredCount = 0;
		int requestedTotalUl = 0;
		FillTankFromCollectors(tower, obj, out producedByUs, out withContainer, out machineTotal, out pouredCount, out requestedTotalUl);
		int purifiedCount = 0;
		int withContainer2 = 0;
		int machineTotal2 = 0;
		PurifyTank(tower, obj, out purifiedCount, out withContainer2, out machineTotal2);
		int num2 = ReadMl(obj);
		int num3 = ReadCapacityMl(obj);
		int num4 = FillOutputSlot(tower);
		RestoreTankLook(obj);
		WaterTowerWindow.RefreshStatus(tower);
		string text = NotesText();
		Log.Success("[" + tower.Label + "] 夜间结算完成：补产 " + producedByUs + " ml、净化 " + purifiedCount + " 台；带容器交给原版的机器 " + (withContainer + withContainer2) + " 台；蓄水箱 " + num + " ml → " + num2 + " ml" + text);
		CountPassiveMachines(tower, out var rechargers, out var powerBlocs);
		int num5 = NotesTotal();
		string text2 = NotesReasons();
		string text3 = RegionNotesText();
		int num6 = withContainer + withContainer2;
		int num7 = machineTotal + machineTotal2;
		int num8 = num6 + pouredCount + purifiedCount + num5;
		if (num7 != num8)
		{
			Diag.Once("ledger-mismatch-" + tower.Label, "[" + tower.Label + "] 账本不平（模组缺陷，请连同这一行反馈）：盘点 " + machineTotal + "+" + machineTotal2 + "=" + num7 + " 台，工作 " + num6 + "+" + pouredCount + "+" + purifiedCount + "+" + num5 + "=" + num8 + " 台（带容器+补产+净化+跳过）");
		}
		Diag.Summary("inv-" + tower.Label, "[" + tower.Label + "] 盘点：集水器 " + machineTotal + " 台、净水器 " + machineTotal2 + " 台（水塔结算范围）｜充能器 " + rechargers + " 台、能量阵列 " + powerBlocs + " 台（只放行、不结算）");
		Diag.Summary("work-" + tower.Label, "[" + tower.Label + "] 工作：带容器 " + num6 + " 台（集水 " + withContainer + " / 净水 " + withContainer2 + "，本晚不动作）｜空槽补产 " + pouredCount + " 台、净化 " + purifiedCount + " 台｜跳过 " + num5 + " 台" + ((text2.Length > 0) ? ("（" + text2 + "）") : "（无）") + text3);
		Diag.Summary("night-" + tower.Label, "[" + tower.Label + "] 结算：补产 " + producedByUs + " ml（请求 " + requestedTotalUl / 1000 + " ml）、净化 " + purifiedCount + " 台｜蓄水箱 " + num + " → " + num2 + " / " + num3 + " ml｜兜底取水 " + num4 + " ml");
	}

	private static void ResetNotes()
	{
		_notes.Clear();
		_regionNotes.Clear();
	}

	private static void Note(string reason)
	{
		AddCount(_notes, reason);
	}

	private static void RegionNote(string reason)
	{
		AddCount(_regionNotes, reason);
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
		return (text.Length == 0) ? "" : (" ｜未产原因：" + text);
	}

	private static string NotesReasons()
	{
		return Join(_notes);
	}

	private static string RegionNotesText()
	{
		string text = Join(_regionNotes);
		return (text.Length == 0) ? "" : (" ｜区域提示：" + text);
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
		int num = ToInt(WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("WaterHelper"), "GetFreeCapacity", new object[1] { tank }, "GetFreeCapacity(蓄水箱·算容量)"));
		return ReadMl(tank) + num / 1000;
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

	private static int FillOutputSlot(TowerInstance tower)
	{
		try
		{
			if (tower == null || tower.BottleSlot == null)
			{
				return 0;
			}
			object obj = TowerRegistry.ResolveTank(tower);
			if (obj == null)
			{
				return 0;
			}
			tower.TankItem = obj;
			string via;
			object memberRobust = WaterTowerItemFactory.GetMemberRobust(tower.BottleSlot.GetType(), tower.BottleSlot, "childItem", out via);
			if (memberRobust == null)
			{
				return 0;
			}
			Type type = WaterTowerItemFactory.ResolveType("WaterHelper");
			if (type == null)
			{
				return 0;
			}
			int num = ToInt(WaterTowerItemFactory.CallStatic(type, "GetFreeCapacity", new object[1] { memberRobust }, "GetFreeCapacity(取水槽容器)"));
			if (num <= 0)
			{
				return 0;
			}
			int num2 = ReadMl(obj);
			WaterTowerWindow.FillFromTank(tower, fromButton: false);
			int num3 = ReadMl(obj);
			return num3 - num2;
		}
		catch (Exception ex)
		{
			Diag.Exception("night-fill-" + tower.Label, ex, "[" + tower.Label + "] 夜结兜底取水失败（已忽略）");
			return 0;
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
		object zone = ((identifier == "moisture_farm") ? tower.ZoneCollection : tower.ZonePurification);
		foreach (object item2 in MachinesInZone(zone))
		{
			if (Identifier(item2) == identifier)
			{
				list.Add(item2);
			}
		}
		return list;
	}

	private static int FillTankFromCollectors(TowerInstance tower, object tank, out int producedByUs, out int withContainer, out int machineTotal, out int pouredCount, out int requestedTotalUl)
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
				bool flag = DriveCollectorInto(tank, item, out requestedUl);
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

	private static int DrainContainers(List<object> containers, object tank)
	{
		int num = 0;
		foreach (object container in containers)
		{
			if (container != null && container != tank)
			{
				int num2 = ReadMl(container);
				if (num2 <= 0)
				{
					Log.Info("       · " + Identifier(container) + " 是空的（0 ml）→ 没水可搬（原版这一晚也没产）");
					continue;
				}
				int num3 = ReadPurity(container);
				WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("WaterHelper"), "TransferLiquid", new object[2] { container, tank }, "TransferLiquid(" + Identifier(container) + " → 蓄水箱)");
				int num4 = ReadMl(container);
				Log.Info("       · [取水容器] " + Identifier(container) + "：" + num2 + " ml（水质 " + num3 + "）→ 搬入蓄水箱 " + (num2 - num4) + " ml，容器剩余 " + num4 + " ml");
				num += num2 - num4;
			}
		}
		return num;
	}

	private static string MachineLabel(object machine)
	{
		return Identifier(machine) + " " + TankKey(machine);
	}

	private static bool DriveCollectorInto(object tank, object machine, out int requestedUl)
	{
		requestedUl = 0;
		Type type = WaterTowerItemFactory.ResolveType("WaterHelper");
		Type type2 = WaterTowerItemFactory.ResolveType("MachineryHelper");
		Type type3 = WaterTowerItemFactory.ResolveType("PowerHelper");
		Type type4 = WaterTowerItemFactory.ResolveType("MachineHelper");
		Type type5 = WaterTowerItemFactory.ResolveType("MachineMoistureFarm");
		if (type == null || type2 == null || type3 == null || type4 == null || type5 == null)
		{
			Log.Error("   [补产] 类型解析失败，跳过补产");
			Note("补产：类型解析失败");
			return false;
		}
		int num = ToInt(WaterTowerItemFactory.CallStatic(type, "GetFreeCapacity", new object[1] { tank }, "GetFreeCapacity(蓄水箱)"));
		if (num <= 0)
		{
			Log.Info("   [补产] 蓄水箱已满，跳过生产");
			Note("蓄水箱已满（集水器）");
			return false;
		}
		object owner = WaterTowerItemFactory.CallStatic(type4, "GetBatterySlot", new object[1] { machine }, "GetBatterySlot(集水器)");
		object obj = FirstChild(owner);
		if (obj == null)
		{
			Log.Info("   [补产] 集水器没有电池，跳过（供电沿用游戏模型，不替它出电）");
			Note("集水器缺电池");
			return false;
		}
		if (WaterTowerItemFactory.IsItemDestroyed(obj))
		{
			Diag.Once("ghost-battery-farm", "夜结：集水器的电池显示元素当前不可用（元素已销毁）→ 仍正常取电补产");
		}
		int num2 = ToInt(WaterTowerItemFactory.CallStatic(type2, "GetMachinePowerUsage", new object[1] { machine }, "GetMachinePowerUsage(集水器)"));
		object obj2 = WaterTowerItemFactory.CallStatic(type3, "CanDrawPowerSource", new object[2] { obj, num2 }, "CanDrawPowerSource(电池, " + num2 + ")");
		bool flag = default(bool);
		int num3;
		if (obj2 is bool)
		{
			flag = (bool)obj2;
			num3 = 1;
		}
		else
		{
			num3 = 0;
		}
		if (((uint)num3 & (flag ? 1u : 0u)) == 0)
		{
			Log.Info("   [补产] 集水器电量不足（需要 " + num2 + "），跳过");
			Note("集水器电量不足");
			return false;
		}
		WaterTowerItemFactory.CallStatic(type3, "DrawPowerSource", new object[2] { obj, num2 }, "DrawPowerSource(电池, " + num2 + ")");
		int num4 = ToInt(WaterTowerItemFactory.CallStatic(type5, "GetOutputVolume", new object[1] { machine }, "GetOutputVolume(集水器)"));
		string text = WaterTowerItemFactory.CallStatic(type5, "GetOutputQuality", new object[1] { machine }, "GetOutputQuality(集水器)") as string;
		if (num4 <= 0 || string.IsNullOrEmpty(text))
		{
			Log.Info("   [补产] 产量/水质无效（volume=" + num4 + " quality=" + text + "），跳过");
			Note("集水器产量/水质无效");
			return false;
		}
		requestedUl = num4;
		int num5 = ReadPurity(tank);
		int num6 = ReadMl(tank);
		bool flag2 = PourInto(tank, text, num4);
		int num7 = ReadMl(tank);
		Log.Info("   [补产] 集水器直接产水进蓄水箱：水质=" + text + "，产量 " + num4 + " µl（" + num4 / 1000 + " ml）→ 实际 +" + (num7 - num6) + " ml，箱内水质 " + num5 + " → " + ReadPurity(tank) + (flag2 ? "" : "（未执行写入：见 Diag 的 pour-* 行）"));
		if (!flag2)
		{
			return false;
		}
		object obj3 = ModuleInvOf(machine);
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
				GameGridInventory val2 = ((val == null) ? null : val.TryCast<GameGridInventory>());
				if (val2 != null)
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

	private static bool PourInto(object tank, string quality, int volumeUl)
	{
		string text = PourMethodOf(quality);
		if (text == null)
		{
			Diag.Once("pour-unknown-quality", "补产：水质字符串未识别（" + quality + "）→ 本台跳过、未写入蓄水箱（见 docs\\07 第八节）");
			Note("集水器水质字符串未识别");
			return false;
		}
		Type type = WaterTowerItemFactory.ResolveType("WaterHelper");
		if (type == null)
		{
			Diag.Once("pour-no-helper", "补产：找不到 WaterHelper，跳过写入");
			Note("补产：类型解析失败");
			return false;
		}
		WaterTowerItemFactory.CallStatic(type, text, new object[2] { tank, volumeUl }, text + "(蓄水箱, " + volumeUl + " µl)");
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
			else if (PurifyWithOne(item, tank))
			{
				num++;
			}
		}
		purifiedCount = num;
		return num;
	}

	private static bool PurifyWithOne(object purifier, object tank)
	{
		Type type = WaterTowerItemFactory.ResolveType("MachineHelper");
		Type type2 = WaterTowerItemFactory.ResolveType("MachineryHelper");
		Type type3 = WaterTowerItemFactory.ResolveType("PowerHelper");
		Type type4 = WaterTowerItemFactory.ResolveType("MachinePurifier");
		if (type == null || type2 == null || type3 == null || type4 == null)
		{
			Log.Error("净水器净化：类型解析失败，跳过");
			Note("净化：类型解析失败");
			return false;
		}
		object owner = WaterTowerItemFactory.CallStatic(type, "GetBatterySlot", new object[1] { purifier }, "GetBatterySlot(净水器)");
		object obj = FirstChild(owner);
		if (obj == null)
		{
			Log.Info("   净水器没有电池，跳过（供电沿用游戏模型，不替它出电）");
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
			Log.Info("   净水器电量不足（需要 " + num + "），跳过");
			Note("净水器电量不足");
			return false;
		}
		WaterTowerItemFactory.CallStatic(type3, "DrawPowerSource", new object[2] { obj, num }, "DrawPowerSource(电池, " + num + ")");
		int num2 = ReadPurity(tank);
		object obj4 = WaterTowerItemFactory.CallStatic(type4, "PurifyContainer", new object[3] { purifier, tank, false }, "PurifyContainer(净水器, 蓄水箱)");
		int num3 = ReadPurity(tank);
		string text = "";
		if (num3 == num2)
		{
			text = "（水质无变化：这箱水已到这台净水器能提的上限；取电与机器动作仍按原版规则发生）";
		}
		else if (num3 < num2)
		{
			text = "（水质反而下降 —— 异常，请连同这一行反馈）";
		}
		Log.Info("   净化蓄水箱：水质 " + num2 + " → " + num3 + "（返回 " + ((obj4 == null) ? "<null>" : obj4.ToString()) + "）" + text);
		object obj5 = ModuleInvOf(purifier);
		WaterTowerItemFactory.CallStatic(type, "OnMachineActioned", new object[2] { purifier, obj5 }, "OnMachineActioned(净水器)");
		return true;
	}

	private static void RestoreTankLook(object tank)
	{
		try
		{
			if (!WaterTowerItemFactory.IsItemDestroyed(tank))
			{
				Type type = tank.GetType();
				string text = WaterTowerItemFactory.GetMemberValue(type, tank, "spriteAtlasPath") as string;
				string text2 = WaterTowerItemFactory.GetMemberValue(type, tank, "spritePath") as string;
				if (!(text == "Items/water_container4") || !(text2 == "xl"))
				{
					Log.Warn("   蓄水箱外观被改动过（实际 " + text + "/" + text2 + "），恢复为 Items/water_container4/xl");
					WaterTowerItemFactory.CallInstance(type, tank, "SetSpriteAndShape", new object[2] { "Items/water_container4", "xl" }, "恢复蓄水箱外观");
					WaterTowerItemFactory.SetMemberValue(type, tank, "name", "水塔蓄水箱（600L）");
				}
			}
		}
		catch (Exception ex)
		{
			Log.Debug("恢复蓄水箱外观失败：" + ex.GetType().Name);
			Diag.Once("tank-look-ex", "恢复蓄水箱外观抛异常（已忽略）：" + ex.GetType().Name + "（外观会保持被改动后的样子；不影响水力/库存）");
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
			object collection = WaterTowerItemFactory.GetMemberValue(type, owner, "childItems") ?? WaterTowerItemFactory.GetMemberValue(type, owner, "children");
			return ItemRegistry.EnumerateIl2Cpp(collection) ?? new List<object>();
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
		return (list.Count > 0) ? list[0] : null;
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
		return (obj is int) ? ((int)obj) : 0;
	}

	private static int ReadPurity(object container)
	{
		object obj = WaterTowerItemFactory.CallStatic(WaterTowerItemFactory.ResolveType("WaterHelper"), "GetWaterPurity", new object[1] { container }, "GetWaterPurity");
		return (obj is int) ? ((int)obj) : (-1);
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
