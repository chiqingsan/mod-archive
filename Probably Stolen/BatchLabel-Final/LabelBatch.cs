using System;
using System.Collections.Generic;

namespace BatchLabelFinal;

/// <summary>
/// 批量复制标签的纯逻辑：只读写 GameItem / ItemFeature / ItemCondition 的数据，
/// 不触碰 Unity、Harmony 与单例，方便在测试工程里直接编译验证边界行为。
/// 复制来源是起点物品「当前显示的标签」；写入语义与游戏内 标签机 UI 一致：
/// 写 fakeCondition + SetExposed(false)；水质标签（CATEGORY_WATER_PURITY）额外写 customIntValue1。
/// </summary>
internal static class LabelBatch
{
    internal const string WaterPurityCategory = "CATEGORY_WATER_PURITY";

    /// <summary>一个已打上的标签：所属分类 + 假条件 + 水质标签需要一起复制的自定义值。</summary>
    internal readonly struct LabelEntry
    {
        public readonly string Category;
        public readonly ItemCondition Condition;
        public readonly int CustomIntValue;
        public readonly bool CopyCustomIntValue;

        public LabelEntry(string category, ItemCondition condition, int customIntValue, bool copyCustomIntValue)
        {
            Category = category;
            Condition = condition;
            CustomIntValue = customIntValue;
            CopyCustomIntValue = copyCustomIntValue;
        }
    }

    /// <summary>
    /// 收集起点物品「当前显示的标签」：标签机可编辑的每个分类，取它现在显示的条件。
    /// 未揭穿时显示的是假条件（贴过的标签），已揭穿时显示的是真实条件；
    /// 因此正品作起点就复制正品标签、赝品作起点就复制赝品标签，不要求先手动贴过标签。
    /// </summary>
    internal static List<LabelEntry> Collect(GameItem? source)
    {
        var labels = new List<LabelEntry>();

        // 与原版标签机的目标条件一致（ToolDirectory.CreateLabeler 的 canThisTargetItemFunc）：
        // 只把「自己拥有的物品」当作标签来源，避免从客户/非自有物品上复制条件。
        if (source?.itemFeatures == null || !GeneralHelper.IsItemOwned(source))
        {
            return labels;
        }

        foreach (ItemFeature feature in source.itemFeatures)
        {
            if (feature == null || !LabelerHelper.IsEditableLabelFeature(feature))
            {
                continue;
            }

            // 与物品提示 / 标签机 UI 显示的内容一致：未揭穿取假条件，已揭穿取真实条件。
            ItemCondition? shown = feature.GetCurrentItemCondition();
            if (shown == null)
            {
                continue;
            }

            labels.Add(new LabelEntry(feature.category!, shown, feature.customIntValue1,
                string.Equals(feature.category, WaterPurityCategory, StringComparison.Ordinal)));
        }

        return labels;
    }

    /// <summary>把标签写到单个物品上，返回实际改动的 feature 数。</summary>
    internal static int ApplyItem(GameItem? target, IReadOnlyList<LabelEntry> labels)
    {
        if (target?.itemFeatures == null || labels.Count == 0)
        {
            return 0;
        }

        int changedFeatures = 0;
        for (int i = 0; i < labels.Count; i++)
        {
            LabelEntry label = labels[i];
            ItemFeature? feature = FindEditableFeature(target, label.Category);
            if (feature == null)
            {
                continue;
            }

            bool conditionDiffers = !SameCondition(feature.fakeCondition, label.Condition);
            bool valueDiffers = label.CopyCustomIntValue && feature.customIntValue1 != label.CustomIntValue;

            // 已经是一模一样的假标签且未被揭穿时不再调 SetExposed：
            // 原版每次 SetExposed(false) 都会执行标签条件的 OnActivate 动作（改名 / 改价）。
            bool needsExpose = feature.IsExposed() || conditionDiffers;

            feature.fakeCondition = label.Condition;
            if (valueDiffers)
            {
                feature.customIntValue1 = label.CustomIntValue;
            }

            if (needsExpose)
            {
                feature.SetExposed(false);
            }

            if (conditionDiffers || valueDiffers || needsExpose)
            {
                changedFeatures++;
            }
        }

        return changedFeatures;
    }

    /// <summary>
    /// 对框选结果批量打标签：只处理与起点 identifier 相同的物品（跳过起点自己），
    /// 返回被打上标签的物品数。
    /// </summary>
    internal static int ApplyToMatchingItems(GameItem? source, IReadOnlyList<LabelEntry> labels,
        IEnumerable<GameItemElement>? selected)
    {
        if (source == null || labels.Count == 0 || selected == null)
        {
            return 0;
        }

        int changedItems = 0;
        foreach (GameItemElement item in selected)
        {
            if (item == null || ReferenceEquals(item, source))
            {
                continue;
            }

            if (!string.Equals(item.identifier, source.identifier, StringComparison.Ordinal))
            {
                continue;
            }

            // 与原版标签机一致：非自有物品不接受标签写入。
            if (!GeneralHelper.IsItemOwned(item))
            {
                continue;
            }

            if (ApplyItem(item, labels) > 0)
            {
                changedItems++;
            }
        }

        return changedItems;
    }

    private static ItemFeature? FindEditableFeature(GameItem target, string category)
    {
        foreach (ItemFeature feature in target.itemFeatures)
        {
            if (feature == null || !string.Equals(feature.category, category, StringComparison.Ordinal))
            {
                continue;
            }

            if (!LabelerHelper.IsEditableLabelFeature(feature))
            {
                continue;
            }

            return feature;
        }

        return null;
    }

    private static bool SameCondition(ItemCondition? a, ItemCondition? b)
    {
        if (a == null || b == null)
        {
            return ReferenceEquals(a, b);
        }

        return string.Equals(a.identifier, b.identifier, StringComparison.Ordinal);
    }
}
