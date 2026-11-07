using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace PurifierTower;

/// <summary>
/// 把本 mod 的物品名写入游戏自带的 Unity Localization「Item」字符串表。
/// 游戏与物品管理器都按「Item」表 + `item_&lt;identifier&gt;_name` 的约定解析物品名；
/// 注入后物品管理器才能按中文名搜索/显示，游戏内也走同一来源。
/// 只改内存里的 LocalizationTable，不读写任何文件（manifest 未声明 files 权限）。
/// </summary>
internal static class LocalizationInject
{
    private const string TableName = "Item";

    private static readonly (string Identifier, string Zh, string En)[] ItemNames =
    {
        ("purifier_tower", "海德拉净水箱", "Hydra Purifier Tank"),
        ("purifier_tower_container_200l", "200L 水容器", "200L Water Container"),
        ("purifier_tower_bottle_1l", "1L 水瓶", "1L Water Bottle"),
        ("purifier_tower_bottle_3l", "3L 水瓶", "3L Water Bottle"),
        ("purifier_tower_bottle_6l", "6L 水瓶", "6L Water Bottle"),
        ("purifier_tower_bottle_20l", "20L 水桶", "20L Water Barrel"),
        ("purifier_tower_bottle_60l", "60L 水桶", "60L Water Barrel"),
    };

    private static bool _injected;

    public static string KeyOf(string identifier)
    {
        return "item_" + identifier + "_name";
    }

    /// <summary>
    /// 注入全部物品名。本地化系统/表尚未就绪时返回 false，调用方可在后续加载钩子里重试。
    /// 幂等：已注入过直接返回 true。
    /// </summary>
    public static bool TryInject()
    {
        if (_injected)
        {
            return true;
        }

        try
        {
            var locales = LocalizationSettings.AvailableLocales?.Locales;
            if (locales == null || locales.Count == 0)
            {
                return false;
            }

            int written = 0;
            foreach (Locale locale in locales)
            {
                StringTable? table = LocalizationSettings.StringDatabase.GetTable(TableName, locale);
                if (table == null)
                {
                    continue;
                }

                // 简体中文 locale 的 code 是 "zh"（游戏日志 primary=zh）；其余 locale 写英文兜底。
                bool isChinese = (locale.Identifier.Code ?? string.Empty)
                    .StartsWith("zh", StringComparison.OrdinalIgnoreCase);
                foreach (var (identifier, zhName, enName) in ItemNames)
                {
                    table.AddEntry(KeyOf(identifier), isChinese ? zhName : enName);
                    written++;
                }
            }

            if (written == 0)
            {
                return false;
            }

            _injected = true;
            Debug.Log($"[PurifierTower] localization injected: {ItemNames.Length} item names ({written} locale entries)");
            return true;
        }
        catch (Exception error)
        {
            Debug.LogWarning($"[PurifierTower] localization inject failed: {error.GetType().Name}: {error.Message}");
            return false;
        }
    }

    /// <summary>取显示名；注入尚未就绪/条目缺失时返回硬编码兜底，保证 item.name 永远可读。</summary>
    public static string GetItemName(string identifier, string fallback)
    {
        string key = KeyOf(identifier);
        try
        {
            StringTable? table = LocalizationSettings.StringDatabase.GetTable(TableName);
            string? value = table?.GetEntry(key)?.Value;
            if (!string.IsNullOrEmpty(value))
            {
                return value!;
            }
        }
        catch
        {
            // 名称不是关键路径：任何异常都退回兜底名。
        }

        return fallback;
    }
}
