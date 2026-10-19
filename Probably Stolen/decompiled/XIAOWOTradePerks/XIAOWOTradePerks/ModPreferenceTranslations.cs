using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XIAOWOTradePerks;

internal static class ModPreferenceTranslations
{
	private static readonly Dictionary<string, string> Categories = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		["XIAOWOTradePerks_ModManager"] = "MOD 管理",
		["XIAOWORatColony"] = "小窝鼠群",
		["General"] = "常规",
		["Settings"] = "设置",
		["Gameplay"] = "玩法",
		["Graphics"] = "画面",
		["Video"] = "视频",
		["Audio"] = "音频",
		["Sound"] = "声音",
		["Controls"] = "操作",
		["Input"] = "输入",
		["Hotkeys"] = "快捷键",
		["Keybinds"] = "按键绑定",
		["Performance"] = "性能",
		["Accessibility"] = "辅助功能",
		["UI"] = "界面",
		["Interface"] = "界面",
		["Notifications"] = "通知",
		["Debug"] = "调试",
		["Economy"] = "经济",
		["Inventory"] = "物品栏",
		["Saving"] = "存档",
		["Save"] = "存档",
		["Network"] = "网络",
		["Items"] = "物品",
		["NPC"] = "NPC",
		["Difficulty"] = "难度"
	};

	private static readonly Dictionary<string, string> ScopedEntries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		[Key("XIAOWOTradePerks_ModManager", "ToggleHotkey")] = "切换管理窗口的快捷键",
		[Key("XIAOWORatColony", "TargetGrowth")] = "成长 S 级目标",
		[Key("XIAOWORatColony", "TargetHealth")] = "生命 S 级目标",
		[Key("XIAOWORatColony", "TargetHunger")] = "饥饿 S 级目标",
		[Key("XIAOWORatColony", "TargetImmunity")] = "免疫 S 级目标",
		[Key("XIAOWORatColony", "TargetLitter")] = "产仔 S 级目标",
		[Key("XIAOWORatColony", "TargetLongevity")] = "寿命 S 级目标"
	};

	private static readonly Dictionary<string, string> ScopedDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { [Key("XIAOWOTradePerks_ModManager", "ToggleHotkey")] = "Tab 上方的物理按键；也支持 Ctrl+Shift+M 等组合键。" };

	private static readonly Dictionary<string, string> CommonEntries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		["Enabled"] = "启用",
		["Enable"] = "启用",
		["IsEnabled"] = "是否启用",
		["Active"] = "激活",
		["Disabled"] = "禁用",
		["ToggleHotkey"] = "切换快捷键",
		["OpenHotkey"] = "打开快捷键",
		["CloseHotkey"] = "关闭快捷键",
		["MenuHotkey"] = "菜单快捷键",
		["ToggleKey"] = "切换按键",
		["OpenKey"] = "打开按键",
		["Shortcut"] = "快捷键",
		["Keybind"] = "按键绑定",
		["Language"] = "语言",
		["Locale"] = "语言代码",
		["FontSize"] = "字体大小",
		["UIScale"] = "界面缩放",
		["WindowScale"] = "窗口缩放",
		["Opacity"] = "不透明度",
		["Transparency"] = "透明度",
		["PositionX"] = "横向位置",
		["PositionY"] = "纵向位置",
		["Width"] = "宽度",
		["Height"] = "高度",
		["ShowFPS"] = "显示帧率",
		["ShowNotifications"] = "显示通知",
		["ShowTooltips"] = "显示提示",
		["ShowOverlay"] = "显示悬浮层",
		["ShowDebugInfo"] = "显示调试信息",
		["DebugMode"] = "调试模式",
		["DebugLogging"] = "调试日志",
		["VerboseLogging"] = "详细日志",
		["LogLevel"] = "日志级别",
		["AutoSave"] = "自动保存",
		["AutoBackup"] = "自动备份",
		["BackupCount"] = "备份数量",
		["SaveInterval"] = "保存间隔",
		["SaveIntervalSeconds"] = "保存间隔（秒）",
		["Volume"] = "音量",
		["MasterVolume"] = "主音量",
		["MusicVolume"] = "音乐音量",
		["SoundVolume"] = "音效音量",
		["SFXVolume"] = "音效音量",
		["Mute"] = "静音",
		["Fullscreen"] = "全屏",
		["VSync"] = "垂直同步",
		["FrameRateLimit"] = "帧率上限",
		["TargetFPS"] = "目标帧率",
		["SpawnChance"] = "出现概率",
		["SpawnRate"] = "出现频率",
		["DropChance"] = "掉落概率",
		["DropRate"] = "掉落率",
		["PriceMultiplier"] = "价格倍率",
		["CostMultiplier"] = "费用倍率",
		["SpeedMultiplier"] = "速度倍率",
		["DamageMultiplier"] = "伤害倍率",
		["ExperienceMultiplier"] = "经验倍率",
		["XpMultiplier"] = "经验倍率",
		["RewardMultiplier"] = "奖励倍率",
		["ProfitMultiplier"] = "利润倍率",
		["CooldownSeconds"] = "冷却时间（秒）",
		["IntervalSeconds"] = "间隔（秒）",
		["MinItems"] = "最少物品数",
		["MaxItems"] = "最多物品数",
		["MaxStackSize"] = "最大堆叠数量",
		["InventorySize"] = "物品栏容量",
		["MaxClients"] = "最多顾客数",
		["MaxCustomers"] = "最多顾客数",
		["StartingMoney"] = "初始资金",
		["StartingCash"] = "初始现金",
		["Difficulty"] = "难度",
		["Chance"] = "概率",
		["Probability"] = "概率",
		["Multiplier"] = "倍率",
		["Amount"] = "数量",
		["Count"] = "数量",
		["Duration"] = "持续时间",
		["Interval"] = "间隔",
		["Cooldown"] = "冷却时间",
		["MaxRetries"] = "最大重试次数",
		["RetryCount"] = "重试次数"
	};

	private static readonly Dictionary<string, string> Words = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		["Enable"] = "启用",
		["Disable"] = "禁用",
		["Enabled"] = "启用",
		["Show"] = "显示",
		["Hide"] = "隐藏",
		["Use"] = "使用",
		["Allow"] = "允许",
		["Auto"] = "自动",
		["Save"] = "保存",
		["Backup"] = "备份",
		["Debug"] = "调试",
		["Log"] = "日志",
		["Logging"] = "日志",
		["Notification"] = "通知",
		["Notifications"] = "通知",
		["Tooltip"] = "提示",
		["Tooltips"] = "提示",
		["Overlay"] = "悬浮层",
		["Menu"] = "菜单",
		["Window"] = "窗口",
		["UI"] = "界面",
		["Font"] = "字体",
		["Hotkey"] = "快捷键",
		["Shortcut"] = "快捷键",
		["Key"] = "按键",
		["Keybind"] = "按键绑定",
		["Toggle"] = "切换",
		["Open"] = "打开",
		["Close"] = "关闭",
		["Size"] = "大小",
		["Scale"] = "缩放",
		["Opacity"] = "不透明度",
		["Width"] = "宽度",
		["Height"] = "高度",
		["Position"] = "位置",
		["X"] = "X",
		["Y"] = "Y",
		["Max"] = "最大",
		["Min"] = "最小",
		["Minimum"] = "最小",
		["Maximum"] = "最大",
		["Starting"] = "初始",
		["Default"] = "默认",
		["Client"] = "顾客",
		["Clients"] = "顾客数",
		["Customer"] = "顾客",
		["Customers"] = "顾客数",
		["Item"] = "物品",
		["Items"] = "物品数",
		["Inventory"] = "物品栏",
		["Stack"] = "堆叠",
		["Slot"] = "格子",
		["Slots"] = "格子数",
		["Count"] = "数量",
		["Amount"] = "数量",
		["Money"] = "资金",
		["Cash"] = "现金",
		["Price"] = "价格",
		["Cost"] = "费用",
		["Profit"] = "利润",
		["Reward"] = "奖励",
		["Experience"] = "经验",
		["XP"] = "经验",
		["Damage"] = "伤害",
		["Speed"] = "速度",
		["Multiplier"] = "倍率",
		["Rate"] = "比例",
		["Chance"] = "概率",
		["Probability"] = "概率",
		["Spawn"] = "出现",
		["Drop"] = "掉落",
		["Cooldown"] = "冷却时间",
		["Duration"] = "持续时间",
		["Interval"] = "间隔",
		["Seconds"] = "（秒）",
		["Minutes"] = "（分钟）",
		["Hours"] = "（小时）",
		["Days"] = "（天）",
		["FPS"] = "帧率",
		["Volume"] = "音量",
		["Music"] = "音乐",
		["Sound"] = "音效",
		["SFX"] = "音效",
		["Master"] = "主",
		["Mute"] = "静音",
		["Power"] = "电力",
		["Energy"] = "能量",
		["Health"] = "生命",
		["Hunger"] = "饥饿",
		["Growth"] = "成长",
		["Litter"] = "产仔",
		["Immunity"] = "免疫",
		["Longevity"] = "寿命",
		["Quality"] = "品质",
		["Level"] = "等级",
		["Limit"] = "上限"
	};

	private static readonly Regex IdentifierWords = new Regex("[A-Z]+(?![a-z])|[A-Z]?[a-z]+|[0-9]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

	internal static string Category(string locale, string categoryId, string original)
	{
		if (!ModLocalization.IsChinese(locale) || !Categories.TryGetValue(categoryId ?? "", out var value))
		{
			return original;
		}
		return value;
	}

	internal static string Entry(string locale, string categoryId, string entryId, string original)
	{
		if (!ModLocalization.IsChinese(locale))
		{
			return original;
		}
		if (ScopedEntries.TryGetValue(Key(categoryId, entryId), out var value))
		{
			return value;
		}
		if (CommonEntries.TryGetValue(entryId ?? "", out var value2))
		{
			return value2;
		}
		if (!TryCompose(entryId, out var translation))
		{
			return original;
		}
		return translation;
	}

	internal static string Description(string locale, string categoryId, string entryId, string original)
	{
		if (!ModLocalization.IsChinese(locale) || !ScopedDescriptions.TryGetValue(Key(categoryId, entryId), out var value))
		{
			return original;
		}
		return value;
	}

	private static string Key(string categoryId, string entryId)
	{
		return categoryId + "\0" + entryId;
	}

	private static bool TryCompose(string identifier, out string translation)
	{
		translation = string.Empty;
		if (string.IsNullOrWhiteSpace(identifier) || identifier.Length > 80)
		{
			return false;
		}
		string text = identifier.Replace('_', ' ').Replace('-', ' ');
		MatchCollection matchCollection = IdentifierWords.Matches(text);
		if (matchCollection.Count < 2)
		{
			return false;
		}
		List<string> list = new List<string>(matchCollection.Count);
		int num = 0;
		foreach (Match item in matchCollection)
		{
			if (text.Substring(num, item.Index - num).Trim().Length != 0 || !Words.TryGetValue(item.Value, out var value))
			{
				return false;
			}
			list.Add(value);
			num = item.Index + item.Length;
		}
		if (text.Substring(num).Trim().Length != 0)
		{
			return false;
		}
		translation = string.Concat(list);
		return true;
	}
}
