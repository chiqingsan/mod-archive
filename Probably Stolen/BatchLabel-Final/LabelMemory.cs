using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BatchLabelFinal;

/// <summary>
/// 「标签记忆」的纯逻辑：按物品类型（identifier）记住玩家给这一类物品打过的标签，
/// 之后装备标签机右键同类物品，就能把这些标签重新贴上去。
/// 只做合并 / 查找 / 文本序列化，不触碰 Unity、Harmony 与单例，方便在测试工程里直接编译验证。
/// </summary>
internal static class LabelMemory
{
    private const char ItemSeparator = '|';
    private const char ItemKeySeparator = '=';
    private const char LabelSeparator = ';';
    private const char FieldSeparator = ':';

    /// <summary>记忆里的一条标签：分类 + 条件 identifier（条件对象在套用时才解析）+ 水质标签的自定义值。</summary>
    internal readonly struct Remembered
    {
        internal readonly string Category;
        internal readonly string Condition;
        internal readonly int CustomIntValue;
        internal readonly bool CopyCustomIntValue;

        internal Remembered(string category, string condition, int customIntValue, bool copyCustomIntValue)
        {
            Category = category;
            Condition = condition;
            CustomIntValue = customIntValue;
            CopyCustomIntValue = copyCustomIntValue;
        }
    }

    // 物品 identifier → （标签分类 → 记住的标签）。
    // 同一分类后打上的标签覆盖先前的；不同分类各自累积，互不干扰。
    private static readonly Dictionary<string, Dictionary<string, Remembered>> Entries =
        new(StringComparer.Ordinal);

    internal static int RememberedItemCount => Entries.Count;

    internal static void Clear()
    {
        Entries.Clear();
    }

    /// <summary>记住「这类物品的这些分类应该贴成什么」，数据来自刚打上的标签。</summary>
    internal static void Remember(string? itemIdentifier, IReadOnlyList<LabelBatch.LabelEntry>? labels)
    {
        if (string.IsNullOrEmpty(itemIdentifier) || labels == null || labels.Count == 0)
        {
            return;
        }

        if (!Entries.TryGetValue(itemIdentifier!, out Dictionary<string, Remembered>? byCategory))
        {
            byCategory = new Dictionary<string, Remembered>(StringComparer.Ordinal);
            Entries[itemIdentifier!] = byCategory;
        }

        for (int i = 0; i < labels.Count; i++)
        {
            LabelBatch.LabelEntry label = labels[i];
            if (string.IsNullOrEmpty(label.Category) || label.Condition == null)
            {
                continue;
            }

            string condition = label.Condition.identifier;
            if (string.IsNullOrEmpty(condition))
            {
                continue;
            }

            byCategory[label.Category] = new Remembered(label.Category, condition, label.CustomIntValue,
                label.CopyCustomIntValue);
        }
    }

    /// <summary>
    /// 取回这类物品记住的标签，并把条件 identifier 解析回当前的 <see cref="ItemCondition"/>。
    /// 分类或条件已经不存在（游戏更新、物品改版）时跳过该条，其余照常返回。
    /// </summary>
    internal static bool TryResolve(string? itemIdentifier, out List<LabelBatch.LabelEntry> labels)
    {
        labels = new List<LabelBatch.LabelEntry>();
        if (string.IsNullOrEmpty(itemIdentifier)
            || !Entries.TryGetValue(itemIdentifier!, out Dictionary<string, Remembered>? byCategory))
        {
            return false;
        }

        foreach (Remembered entry in byCategory.Values)
        {
            ItemCondition? condition = FindCondition(entry.Category, entry.Condition);
            if (condition == null)
            {
                continue;
            }

            labels.Add(new LabelBatch.LabelEntry(entry.Category, condition, entry.CustomIntValue,
                entry.CopyCustomIntValue));
        }

        return labels.Count > 0;
    }

    internal static bool IsRemembered(string? itemIdentifier)
    {
        return !string.IsNullOrEmpty(itemIdentifier) && Entries.ContainsKey(itemIdentifier!);
    }

    /// <summary>序列化成单行文本（存进模组配置项），顺序固定，便于比较是否需要重新写盘。</summary>
    internal static string Serialize()
    {
        if (Entries.Count == 0)
        {
            return "";
        }

        List<string> itemIdentifiers = new List<string>(Entries.Keys);
        itemIdentifiers.Sort(StringComparer.Ordinal);

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < itemIdentifiers.Count; i++)
        {
            string itemIdentifier = itemIdentifiers[i];
            Dictionary<string, Remembered> byCategory = Entries[itemIdentifier];
            if (byCategory.Count == 0)
            {
                continue;
            }

            if (builder.Length > 0)
            {
                builder.Append(ItemSeparator);
            }

            builder.Append(Escape(itemIdentifier)).Append(ItemKeySeparator);

            List<string> categories = new List<string>(byCategory.Keys);
            categories.Sort(StringComparer.Ordinal);
            for (int j = 0; j < categories.Count; j++)
            {
                if (j > 0)
                {
                    builder.Append(LabelSeparator);
                }

                Remembered entry = byCategory[categories[j]];
                builder.Append(Escape(entry.Category)).Append(FieldSeparator)
                    .Append(Escape(entry.Condition)).Append(FieldSeparator)
                    .Append(entry.CustomIntValue.ToString(CultureInfo.InvariantCulture)).Append(FieldSeparator)
                    .Append(entry.CopyCustomIntValue ? '1' : '0');
            }
        }

        return builder.ToString();
    }

    /// <summary>从单行文本还原；空文本、损坏的记录、未知格式一律跳过，不会抛异常。</summary>
    internal static void Load(string? text)
    {
        Entries.Clear();
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        foreach (string row in SplitEscaped(text!, ItemSeparator))
        {
            int separator = IndexOfUnescaped(row, ItemKeySeparator);
            if (separator <= 0)
            {
                continue;
            }

            string itemIdentifier = Unescape(row.Substring(0, separator));
            if (itemIdentifier.Length == 0)
            {
                continue;
            }

            Dictionary<string, Remembered> byCategory = new Dictionary<string, Remembered>(StringComparer.Ordinal);
            foreach (string field in SplitEscaped(row.Substring(separator + 1), LabelSeparator))
            {
                Remembered? entry = ParseLabel(field);
                if (entry.HasValue)
                {
                    byCategory[entry.Value.Category] = entry.Value;
                }
            }

            if (byCategory.Count > 0)
            {
                Entries[itemIdentifier] = byCategory;
            }
        }
    }

    private static Remembered? ParseLabel(string field)
    {
        List<string> parts = SplitEscaped(field, FieldSeparator);
        if (parts.Count < 2)
        {
            return null;
        }

        string category = Unescape(parts[0]);
        string condition = Unescape(parts[1]);
        if (category.Length == 0 || condition.Length == 0)
        {
            return null;
        }

        int customIntValue = 0;
        if (parts.Count > 2 && !int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture,
                out customIntValue))
        {
            customIntValue = 0;
        }

        bool copyCustomIntValue = parts.Count > 3 && parts[3] == "1";
        return new Remembered(category, condition, customIntValue, copyCustomIntValue);
    }

    private static ItemCondition? FindCondition(string category, string conditionIdentifier)
    {
        if (!ItemConditionList.conditionLookup.TryGetValue(category, out List<ItemCondition>? conditions)
            || conditions == null)
        {
            return null;
        }

        foreach (ItemCondition condition in conditions)
        {
            if (condition != null && string.Equals(condition.identifier, conditionIdentifier, StringComparison.Ordinal))
            {
                return condition;
            }
        }

        return null;
    }

    private static string Escape(string value)
    {
        StringBuilder? builder = null;
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (c != '\\' && c != ItemSeparator && c != ItemKeySeparator && c != LabelSeparator && c != FieldSeparator)
            {
                builder?.Append(c);
                continue;
            }

            if (builder == null)
            {
                builder = new StringBuilder(value.Length + 4);
                builder.Append(value, 0, i);
            }

            builder.Append('\\').Append(c);
        }

        return builder == null ? value : builder.ToString();
    }

    private static string Unescape(string value)
    {
        if (value.IndexOf('\\') < 0)
        {
            return value;
        }

        StringBuilder builder = new StringBuilder(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == '\\' && i + 1 < value.Length)
            {
                builder.Append(value[++i]);
                continue;
            }

            builder.Append(value[i]);
        }

        return builder.ToString();
    }

    private static int IndexOfUnescaped(string value, char separator)
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == '\\')
            {
                i++;
                continue;
            }

            if (value[i] == separator)
            {
                return i;
            }
        }

        return -1;
    }

    private static List<string> SplitEscaped(string value, char separator)
    {
        List<string> parts = new List<string>();
        int start = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == '\\')
            {
                i++;
                continue;
            }

            if (value[i] != separator)
            {
                continue;
            }

            parts.Add(value.Substring(start, i - start));
            start = i + 1;
        }

        parts.Add(value.Substring(start));
        return parts;
    }
}
