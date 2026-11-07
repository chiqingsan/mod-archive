using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace CustomerCreditBoost;

public sealed class CustomerCreditBoostMod : IMod
{
    private const string HarmonyId = "chiqingsan.customer-credit-boost";

    private readonly Dictionary<StoreClient, int> _lastBoostedDay = new();
    private Harmony? _harmony;
    private ConfigEntry<int>? _valuePerTier;
    private ConfigEntry<int>? _boostPercentPerTier;

    internal static CustomerCreditBoostMod? Instance { get; private set; }

    public void Init(ModManifest manifest)
    {
        Instance = this;
        var category = ModConfig.For(manifest.ID);
        _valuePerTier = category.Define(
            "ValuePerTier",
            100,
            "预算加成",
            () => "每多少展示柜物品基础价值增加一档");
        _boostPercentPerTier = category.Define(
            "BoostPercentPerTier",
            8,
            "预算加成",
            () => "每档增加的顾客预算百分比");
        Debug.Log("[CustomerCreditBoost] 已加载：展示柜匹配物品每 100 基础价值使对应顾客预算 +8%。");
    }

    public void OnEnable()
    {
        _harmony = new Harmony(HarmonyId);
        _harmony.PatchAll();
    }

    public void OnDisable()
    {
        _harmony?.UnpatchAll(HarmonyId);
        _harmony = null;
        _lastBoostedDay.Clear();
        if (ReferenceEquals(Instance, this))
            Instance = null;
    }

    internal void BoostClient(StoreClient client)
    {
        var day = Math.Max(0, StoreStation.GetDayCounter());
        if (_lastBoostedDay.TryGetValue(client, out var boostedDay) && boostedDay == day)
            return;

        var attractedType = client.attractedType ?? string.Empty;
        var attractedItemId = client.attractedItemId ?? string.Empty;
        if (string.IsNullOrEmpty(attractedType) && string.IsNullOrEmpty(attractedItemId))
            return;

        var showcaseBaseValue = GetShowcaseBaseValue(attractedType, attractedItemId);
        var valuePerTier = Math.Max(1, _valuePerTier?.Value ?? 100);
        var boostPercentPerTier = Math.Max(0, _boostPercentPerTier?.Value ?? 8);
        var tiers = showcaseBaseValue / valuePerTier;
        if (tiers <= 0 || boostPercentPerTier <= 0)
            return;

        var totalBoostPercent = tiers * boostPercentPerTier;
        var budget = client.GetBudget();
        var boostedBudget = (long)budget * (100L + totalBoostPercent) / 100L;
        if (boostedBudget <= budget)
            return;

        var newBudget = (int)boostedBudget;
        client.OverrideBudget(newBudget);
        var syncedLines = SyncDialogueBudgets(client.mainDialogue, budget, newBudget);
        _lastBoostedDay[client] = day;
        Debug.Log($"[CustomerCreditBoost] {client.identifier} 展示柜匹配基础价值 {showcaseBaseValue}，预算 {budget} -> {boostedBudget}（+{totalBoostPercent}%，同步对话文本 {syncedLines} 处）。");
    }

    /// <summary>
    /// 顾客介绍文本在创建时就用当时的预算数字固化了，加成后只能把旧数字整体替换成新数字。
    /// 显示与对话历史都只读 <see cref="Dialogue.plainText"/>（与 LocGenderHelper 的做法一致）。
    /// </summary>
    private static int SyncDialogueBudgets(Dialogue? dialogue, int oldBudget, int newBudget)
    {
        return SyncDialogueBudgets(dialogue, oldBudget, newBudget, new HashSet<Dialogue>());
    }

    private static int SyncDialogueBudgets(Dialogue? dialogue, int oldBudget, int newBudget, HashSet<Dialogue> visited)
    {
        var syncedLines = 0;
        while (dialogue != null && visited.Add(dialogue))
        {
            var plainText = dialogue.plainText;
            if (!string.IsNullOrEmpty(plainText))
            {
                var replacedText = BudgetTextSync.ReplaceBudget(plainText, oldBudget, newBudget);
                if (!ReferenceEquals(replacedText, plainText))
                {
                    dialogue.plainText = replacedText;
                    syncedLines++;
                }
            }

            foreach (var choice in dialogue.choices)
            {
                if (choice.Item2 != null)
                    syncedLines += SyncDialogueBudgets(choice.Item2, oldBudget, newBudget, visited);
            }

            dialogue = dialogue.nextDialogue;
        }

        return syncedLines;
    }

    private static long GetShowcaseBaseValue(string attractedType, string attractedItemId)
    {
        var showcase = EmporiumEntry.Instance?.showcaseElement;
        if (showcase == null)
            return 0;

        long total = 0;
        foreach (var item in showcase.childItems)
        {
            if (item == null)
                continue;

            var matches = !string.IsNullOrEmpty(attractedType)
                ? item.IsGameItemType(attractedType)
                : string.Equals(item.identifier, attractedItemId, StringComparison.OrdinalIgnoreCase);
            if (matches)
                total += item.GetBaseValue();
        }

        return total;
    }
}

[HarmonyPatch(typeof(StoreClientManager), "TrackClient")]
internal static class StoreClientTrackingPatch
{
    private static void Postfix(StoreClient storeClient)
    {
        CustomerCreditBoostMod.Instance?.BoostClient(storeClient);
    }
}
