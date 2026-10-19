using System;
using System.Collections.Generic;
using Il2Cpp;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using UnityEngine;

namespace XIAOWOTradePerks;

internal static class PerkRegistry
{
	internal static readonly PerkDefinition[] All = new PerkDefinition[18]
	{
		new PerkDefinition("xiaowo_trade_gold_appraiser", "金牌鉴定师", "Gold Appraiser", "鉴定服务实际收取的服务费提高20%。", "Appraisal service fees actually collected are increased by 20%.", 2, (StartingPerkType)0, "01_gold_appraiser.png", 0),
		new PerkDefinition("xiaowo_trade_appraisal_sign", "鉴定招牌", "Appraisal Sign", "橱窗摆放鉴定证书时，鉴定服务客人的出现机会提高30%。", "While an appraisal certificate is displayed in the showcase, appraisal customers are 30% more likely to appear.", 2, (StartingPerkType)0, "02_appraisal_sign.png", 0),
		new PerkDefinition("xiaowo_trade_grey_insurance", "灰色保单", "Grey Insurance", "每周首位出售赃物给你的商人，其本次来访中全部赃物热度减半。", "The first merchant each week who sells you stolen goods has the heat of every stolen item in that visit halved.", 1, (StartingPerkType)0, "03_grey_insurance.png", 0),
		new PerkDefinition("xiaowo_trade_blackout_broker", "停电掮客", "Blackout Broker", "停电期间，你向顾客出售物品的成交价提高10%。", "During a blackout, prices you receive when selling items to customers are increased by 10%.", 1, (StartingPerkType)0, "04_blackout_broker.png", 0),
		new PerkDefinition("xiaowo_trade_luxury_appointment", "贵客预约", "Luxury Appointment", "每周额外来一位奢侈品收购商；预算为普通顾客均值的150%，但收购价降低10%。", "One extra luxury-goods buyer visits each week. Their budget is 150% of the average ordinary customer budget, but their purchase prices are 10% lower.", 2, (StartingPerkType)0, "05_luxury_appointment.png", 0),
		new PerkDefinition("xiaowo_trade_stable_supply", "稳定货源", "Stable Supply", "普通供货商向你出售物品的成交价降低10%；剧情与特殊客人不受影响。", "Prices paid when buying from ordinary suppliers are reduced by 10%. Story and special clients are unaffected.", 2, (StartingPerkType)0, "14_stable_supply.png", 0),
		new PerkDefinition("xiaowo_trade_reputation_business", "口碑生意", "Reputation Business", "正常交易获得的阵营声望提高20%；声望损失不受影响。", "Positive faction reputation gained from normal trades is increased by 20%. Reputation losses are unaffected.", 1, (StartingPerkType)0, "15_reputation_business.png", 0),
		new PerkDefinition("xiaowo_trade_negotiation_veteran", "谈判老手", "Negotiation Veteran", "所有主动议价的成功率提高10个百分点，最高不超过95%。", "All active bargaining success chances are increased by 10 percentage points, up to 95%.", 2, (StartingPerkType)0, "16_negotiation_veteran.png", 0),
		new PerkDefinition("xiaowo_trade_cashflow_gambler", "现金流赌徒", "Cashflow Gambler", "每周有一天出现一种行情：红色繁荣使所有价格上涨20%，绿色紧缩使所有价格下降20%。", "One day each week has a market condition: a red Boom raises all prices by 20%, while a green Contraction lowers all prices by 20%.", 0, (StartingPerkType)2, "06_cashflow_gambler.png", 0),
		new PerkDefinition("xiaowo_trade_security_target", "安保眼中钉", "Security Target", "赃物初始热度提高25%，安保检查间隔缩短25%。", "Initial heat on stolen goods is increased by 25%, and the interval between security inspections is reduced by 25%.", -3, (StartingPerkType)1, "07_security_target.png", 0),
		new PerkDefinition("xiaowo_trade_rundown_storefront", "门面失修", "Rundown Storefront", "店铺吸引力降低150，所有店铺翻新费用提高20%。", "Store attractiveness is reduced by 150, and all store renovation costs are increased by 20%.", -2, (StartingPerkType)1, "08_rundown_storefront.png", 0),
		new PerkDefinition("xiaowo_trade_regular_credit", "熟客赊账", "Regulars on Credit", "普通回头客的可用预算降低25%；剧情和特殊客人不受影响。", "Ordinary returning customers have 25% less available budget. Story and special customers are unaffected.", -1, (StartingPerkType)1, "09_regular_credit.png", 0),
		new PerkDefinition("xiaowo_trade_owner_deal", "业主密约", "Owner's Deal", "买断后游戏不会结束；每逢原租金日，房东85折出售稀有货物，同行商人继续上门收货。", "Buying the property no longer ends the run. On each former rent day, the landlord sells rare goods at a 15% discount and the accompanying merchant still visits to buy goods.", 0, (StartingPerkType)2, "10_owner_deal.png", 0),
		new PerkDefinition("xiaowo_trade_peerless_prodigy", "旷世奇才", "Peerless Prodigy", "允许额外拥有12个初始天赋栏位。", "Allows 12 additional starting perk slots.", 3, (StartingPerkType)2, "11_peerless_prodigy.png", 12, "colorful_character"),
		new PerkDefinition("xiaowo_trade_reckless_scavenger", "莽撞拾荒者", "Reckless Scavenger", "夜间前往垃圾堆拾荒时，轻伤和重伤概率各提高15个百分点。", "When scavenging at the dumping grounds at night, minor and major wound chances each increase by 15 percentage points.", -2, (StartingPerkType)1, "12_reckless_scavenger.png", 0),
		new PerkDefinition("xiaowo_trade_aug_hunt_contract", "猎杀契约", "Aug Hunt Contract", "导师交付霰弹枪期间，改造体来访机会额外提高30%，每天最多出现2名；当天若有改造体未被击杀，入睡时你会遭到报复并立即结束游戏。", "While the mentor's shotgun is available, Aug visits gain an extra 30% chance, with at most two per day. If any Aug survives the day, they retaliate when you sleep and the run ends.", -3, (StartingPerkType)1, "13_aug_hunt_contract.png", 0),
		new PerkDefinition("xiaowo_trade_operating_surcharge", "运营附加费", "Operating Surcharge", "电力、清洁、保险以及买卖推荐服务的费用提高20%。", "Power, janitorial, insurance, and buy/sell referral service costs are increased by 20%.", -2, (StartingPerkType)1, "17_operating_surcharge.png", 0),
		new PerkDefinition("xiaowo_trade_precision_bay_expansion", "精工扩舱", "Precision Bay Expansion", "可使用金属锭升级机器区、储藏区及其扩容版本，并将老鼠笼的升级上限与最终容量翻倍；未满级时优先升级，满级后可正常放入金属锭。", "Metal ingots upgrade machine and storage bays, including expanded versions, and double the rat cage upgrade limit and final capacity. Upgrades take priority until maximum level, after which ingots can be stored normally.", 0, (StartingPerkType)0, "18_precision_bay_expansion.png", 0)
	};

	private static readonly Dictionary<string, StartingPerk> Created = new Dictionary<string, StartingPerk>(StringComparer.Ordinal);

	internal static bool IsActive(string id)
	{
		if (IsAlwaysEnabledFeature(id))
		{
			return true;
		}
		try
		{
			return StartingPerk.IsPerkActive(id);
		}
		catch
		{
			return false;
		}
	}

	internal static PerkDefinition Find(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		for (int i = 0; i < All.Length; i++)
		{
			if (string.Equals(All[i].Id, id, StringComparison.Ordinal))
			{
				return All[i];
			}
		}
		return null;
	}

	internal static void EnsureRegistered()
	{
		List<StartingPerk> perks = StartingPerkList.Perks;
		if (perks == null)
		{
			return;
		}
		PerkDefinition[] all = All;
		foreach (PerkDefinition perkDefinition in all)
		{
			StartingPerk val = FindPerk(perks, perkDefinition.Id);
			if (val != null)
			{
				ApplyDefinition(val, perkDefinition);
				Created[perkDefinition.Id] = val;
				continue;
			}
			perks.Add(GetOrCreate(perkDefinition));
			Instance log = Core.Log;
			if (log != null)
			{
				log.Msg("Registered perk: " + perkDefinition.Name);
			}
		}
		EnsureMutualIncompatibility(perks);
	}

	internal static void RefreshLocalizedText()
	{
		PerkDefinition[] all = All;
		foreach (PerkDefinition perkDefinition in all)
		{
			StartingPerk value;
			StartingPerk val = (Created.TryGetValue(perkDefinition.Id, out value) ? value : FindPerk(StartingPerkList.Perks, perkDefinition.Id));
			if (val != null)
			{
				ApplyDefinition(val, perkDefinition);
			}
		}
	}

	internal static void EnsurePicker(PerkUIController ui)
	{
		if ((Object)(object)ui == (Object)null || (Object)(object)ui.availablePerks == (Object)null || (Object)(object)ui.perkElementPrefab == (Object)null)
		{
			return;
		}
		EnsureRegistered();
		RemoveAlwaysEnabledFeatures(ui);
		bool flag = false;
		PerkDefinition[] all = All;
		foreach (PerkDefinition perkDefinition in all)
		{
			if (IsAlwaysEnabledFeature(perkDefinition.Id))
			{
				continue;
			}
			StartingPerkElement val = FindElement(ui.selectedPerks, perkDefinition.Id) ?? FindElement(ui.availablePerks, perkDefinition.Id);
			if ((Object)(object)val == (Object)null)
			{
				GameObject val2 = Object.Instantiate<GameObject>(ui.perkElementPrefab, ui.availablePerks.transform);
				val = val2.GetComponent<StartingPerkElement>();
				if ((Object)(object)val == (Object)null)
				{
					Object.Destroy((Object)(object)val2);
					continue;
				}
				val.id = perkDefinition.Id;
				val.isSelected = false;
				val2.SetActive(true);
				flag = true;
			}
			val.perk = GetOrCreate(perkDefinition);
			Sprite val3 = IconLoader.Get(perkDefinition);
			if ((Object)(object)val3 != (Object)null && (Object)(object)val.icon != (Object)null)
			{
				val.icon.sprite = val3;
			}
		}
		if (flag)
		{
			ui.SortPerkContainer(ui.availablePerks);
		}
		PlacePeerlessBeforeOddball(ui.availablePerks);
		PerkPickerScroll.Ensure(ui);
	}

	internal static void AfterPickerMove(PerkUIController ui, StartingPerkElement movedPerk, bool selected)
	{
		if ((Object)(object)ui == (Object)null || (Object)(object)movedPerk == (Object)null)
		{
			return;
		}
		RemoveAlwaysEnabledFeatures(ui);
		if (IsAlwaysEnabledFeature(movedPerk.id))
		{
			PerkPickerScroll.Refresh(ui);
			return;
		}
		if (selected && string.Equals(movedPerk.id, "xiaowo_trade_peerless_prodigy", StringComparison.Ordinal))
		{
			MoveToEndOfPerks(ui.selectedPerks, movedPerk);
		}
		PlacePeerlessBeforeOddball(ui.availablePerks);
		PerkPickerScroll.Refresh(ui);
	}

	private static StartingPerk GetOrCreate(PerkDefinition definition)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		if (Created.TryGetValue(definition.Id, out var value) && value != null)
		{
			return value;
		}
		value = new StartingPerk();
		ApplyDefinition(value, definition);
		Created[definition.Id] = value;
		return value;
	}

	private static void ApplyDefinition(StartingPerk perk, PerkDefinition definition)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		perk.id = definition.Id;
		perk.cost = definition.Cost;
		perk.maxSlot = definition.MaxSlot;
		perk.type = definition.Type;
		perk.rawName = definition.Name;
		perk.rawDescription = definition.Description;
		if (perk.incompatiblePerks == null)
		{
			List<string> val2 = (perk.incompatiblePerks = new List<string>());
		}
		for (int i = 0; i < definition.IncompatibleIds.Length; i++)
		{
			AddIncompatible(perk, definition.IncompatibleIds[i]);
		}
	}

	private static void EnsureMutualIncompatibility(List<StartingPerk> list)
	{
		StartingPerk val = FindPerk(list, "xiaowo_trade_peerless_prodigy");
		if (val == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			StartingPerk val2 = list[i];
			string a = CleanId((val2 != null) ? val2.id : null);
			if (string.Equals(a, "colorful_character", StringComparison.Ordinal) || string.Equals(a, "奇葩人物", StringComparison.Ordinal))
			{
				AddIncompatible(val, val2.id);
				AddIncompatible(val2, "xiaowo_trade_peerless_prodigy");
			}
		}
	}

	private static void AddIncompatible(StartingPerk perk, string id)
	{
		if (perk == null || string.IsNullOrEmpty(id))
		{
			return;
		}
		if (perk.incompatiblePerks == null)
		{
			List<string> val2 = (perk.incompatiblePerks = new List<string>());
		}
		for (int i = 0; i < perk.incompatiblePerks.Count; i++)
		{
			if (string.Equals(perk.incompatiblePerks[i], id, StringComparison.Ordinal))
			{
				return;
			}
		}
		perk.incompatiblePerks.Add(id);
	}

	private static string CleanId(string id)
	{
		if (!string.IsNullOrEmpty(id))
		{
			return id.Replace("\0", string.Empty).Trim();
		}
		return string.Empty;
	}

	private static StartingPerk FindPerk(List<StartingPerk> list, string id)
	{
		if (list == null)
		{
			return null;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && string.Equals(list[i].id, id, StringComparison.Ordinal))
			{
				return list[i];
			}
		}
		return null;
	}

	private static StartingPerkElement FindElement(GameObject root, string id)
	{
		if ((Object)(object)root == (Object)null)
		{
			return null;
		}
		foreach (StartingPerkElement componentsInChild in root.GetComponentsInChildren<StartingPerkElement>(true))
		{
			if ((Object)(object)componentsInChild != (Object)null && string.Equals(componentsInChild.id, id, StringComparison.Ordinal))
			{
				return componentsInChild;
			}
		}
		return null;
	}

	private static void PlacePeerlessBeforeOddball(GameObject root)
	{
		StartingPerkElement val = FindElement(root, "xiaowo_trade_peerless_prodigy");
		StartingPerkElement val2 = FindElementByCleanId(root, "奇葩人物");
		if ((Object)(object)val == (Object)null || (Object)(object)val2 == (Object)null)
		{
			return;
		}
		Transform transform = ((Component)val).transform;
		Transform transform2 = ((Component)val2).transform;
		if (!((Object)(object)transform == (Object)null) && !((Object)(object)transform2 == (Object)null) && !((Object)(object)transform.parent != (Object)(object)transform2.parent))
		{
			int siblingIndex = transform.GetSiblingIndex();
			int siblingIndex2 = transform2.GetSiblingIndex();
			int num = ((siblingIndex < siblingIndex2) ? (siblingIndex2 - 1) : siblingIndex2);
			if (siblingIndex != num)
			{
				transform.SetSiblingIndex(num);
			}
		}
	}

	internal static bool IsAlwaysEnabledFeature(string id)
	{
		if (!string.Equals(id, "xiaowo_trade_owner_deal", StringComparison.Ordinal))
		{
			return string.Equals(id, "xiaowo_trade_precision_bay_expansion", StringComparison.Ordinal);
		}
		return true;
	}

	internal static bool IsSelectablePerk(string id)
	{
		return !IsAlwaysEnabledFeature(id);
	}

	private static void RemoveAlwaysEnabledFeatures(PerkUIController ui)
	{
		RemoveElements((ui != null) ? ui.availablePerks : null, "xiaowo_trade_owner_deal", "xiaowo_trade_precision_bay_expansion");
		RemoveElements((ui != null) ? ui.selectedPerks : null, "xiaowo_trade_owner_deal", "xiaowo_trade_precision_bay_expansion");
	}

	private static void RemoveElements(GameObject root, params string[] ids)
	{
		if ((Object)(object)root == (Object)null)
		{
			return;
		}
		foreach (StartingPerkElement componentsInChild in root.GetComponentsInChildren<StartingPerkElement>(true))
		{
			if ((Object)(object)componentsInChild == (Object)null)
			{
				continue;
			}
			bool flag = false;
			for (int i = 0; i < ids.Length; i++)
			{
				if (string.Equals(componentsInChild.id, ids[i], StringComparison.Ordinal))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				((Component)componentsInChild).gameObject.SetActive(false);
				((Component)componentsInChild).transform.SetParent((Transform)null, false);
				Object.Destroy((Object)(object)((Component)componentsInChild).gameObject);
			}
		}
	}

	private static void MoveToEndOfPerks(GameObject root, StartingPerkElement movedPerk)
	{
		if ((Object)(object)root == (Object)null || (Object)(object)((movedPerk != null) ? ((Component)movedPerk).transform : null) == (Object)null)
		{
			return;
		}
		Transform parent = ((Component)movedPerk).transform.parent;
		int num = ((Component)movedPerk).transform.GetSiblingIndex();
		foreach (StartingPerkElement componentsInChild in root.GetComponentsInChildren<StartingPerkElement>(true))
		{
			if (!((Object)(object)((componentsInChild != null) ? ((Component)componentsInChild).transform : null) == (Object)null) && !((Object)(object)((Component)componentsInChild).transform.parent != (Object)(object)parent))
			{
				num = Math.Max(num, ((Component)componentsInChild).transform.GetSiblingIndex());
			}
		}
		((Component)movedPerk).transform.SetSiblingIndex(num);
	}

	private static StartingPerkElement FindElementByCleanId(GameObject root, string id)
	{
		if ((Object)(object)root == (Object)null)
		{
			return null;
		}
		foreach (StartingPerkElement componentsInChild in root.GetComponentsInChildren<StartingPerkElement>(true))
		{
			if ((Object)(object)componentsInChild != (Object)null && string.Equals(CleanId(componentsInChild.id), id, StringComparison.Ordinal))
			{
				return componentsInChild;
			}
		}
		return null;
	}
}
