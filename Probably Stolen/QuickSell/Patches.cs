using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace QuickSell;

public sealed partial class QuickSellMod
{
    private bool approvingBatch;
    private ProtectedSaleDialog? protectedDialog;

    internal bool ConfirmProtectedDialog()
    {
        if (protectedDialog == null || !protectedDialog.IsShowing) return false;
        protectedDialog.Confirm();
        return true;
    }

    private void DestroyProtectedDialog()
    {
        if (protectedDialog == null) return;
        if (protectedDialog.IsShowing) protectedDialog.gameObject.SetActive(false);
        UnityEngine.Object.Destroy(protectedDialog);
        protectedDialog = null;
    }
    internal bool ConfirmProtectedBatch(NegociationUIManager ui, NegotiationTabletFrame frame, List<GameItem> items, bool ignoreBudget)
    {
        if (approvingBatch || !items.Any(HasProtectedContent)) return true;
        var store = PlayerStore.Instance;
        var customer = store?.currentClientInstance;
        if (customer == null || NegociationUIManager.IsTradeLocked() || IsRestocking) return false;
        var client = customer.GetClientBlueprint();
        var snapshot = items.Select(item => (Item: item, Quantity: item.unitCount, Price: item.GetNegociatedValue(), Protected: HasProtectedContent(item))).ToArray();
        var tableBefore = EmporiumEntry.Instance.GetBackInvItems().Where(item => GeneralHelper.IsItemOwned(item) && client.IsClientBuyingThisItem(item)).ToArray();
        int budget = client.GetBudget();
        long total = snapshot.Sum(item => item.Price);
        var protectedItems = snapshot.Where(item => item.Protected).ToArray();
        var names = protectedItems.Take(3).Select(item => item.Item.GetDisplayName() + " ×" + item.Quantity).ToList();
        if (protectedItems.Length > 3) names.Add($"另有 {protectedItems.Length - 3} 件受保护物品");
        string note = "出售后将解除保护";
        if (client.useClientBudget && total > budget)
            note += $"\n总价 {total}，预算 {budget}；超出部分抹零";
        protectedDialog = frame.confirmRoot.GetComponent<ProtectedSaleDialog>() ?? frame.confirmRoot.AddComponent<ProtectedSaleDialog>();
        protectedDialog.Show(frame, names.ToArray(), note, () =>
        {
            if (store != PlayerStore.Instance || customer != store.currentClientInstance || client.isBusinessComplete || NegociationUIManager.IsTradeLocked() || client.GetBudget() != budget) return;
            var current = EmporiumEntry.Instance.GetBackInvItems().Where(item => GeneralHelper.IsItemOwned(item) && client.IsClientBuyingThisItem(item)).ToList();
            bool changed = current.Count != tableBefore.Length || tableBefore.Any(item => !current.Contains(item)) || snapshot.Any(value => !current.Contains(value.Item) || value.Item.unitCount != value.Quantity
                || value.Item.GetNegociatedValue() != value.Price || HasProtectedContent(value.Item) != value.Protected);
            if (changed) { SetStatus("台面或保护状态已变化，请重新确认出售。"); return; }
            approvingBatch = true;
            try
            {
                TutorialUIManager.Instance.BuyDealed();
                ui.SellMultiple(snapshot.Select(value => value.Item).ToList(), ignoreBudget || (client.useClientBudget && total > budget));
            }
            finally { approvingBatch = false; }
        });
        return false;
    }

    internal bool ConfirmProtectedSingle(GameItem item)
    {
        if (approvingBatch || !HasProtectedContent(item)) return true;
        var ui = NegociationUIManager.Instance;
        ui.OpenUI();
        var frame = ui.uiObject.GetComponent<NegotiationTabletFrame>();
        return ConfirmProtectedBatch(ui, frame, new List<GameItem> { item }, false);
    }
}

[HarmonyPatch(typeof(NegociationUIManager), nameof(NegociationUIManager.OnConfirmPanelYes))]
internal static class ProtectedDialogConfirmPatch
{
    private static bool Prefix() => QuickSellMod.Current == null || !QuickSellMod.Current.ConfirmProtectedDialog();
}

[HarmonyPatch(typeof(NegociationUIManager), "Rebuild")]
internal static class TabletRebuildPatch
{
    private static void Prefix() => QuickSellMod.Current?.BeginTabletBuild();
    private static void Postfix() => QuickSellMod.Current?.EndTabletBuild();
}

[HarmonyPatch(typeof(NegociationUIManager), "BuildBuying")]
internal static class BuyingButtonsPatch
{
    private static void Postfix(NegociationUIManager __instance, NegotiationTabletFrame ___frame, TabletButton ___primaryButton, StoreClient storeClient)
        => QuickSellMod.Current?.AttachButtons(__instance, ___frame, ___primaryButton, storeClient);
}

[HarmonyPatch(typeof(NegociationUIManager), nameof(NegociationUIManager.OnSellAllClick))]
internal static class BatchClickPatch
{
    private static bool Prefix(NegociationUIManager __instance, NegotiationTabletFrame ___frame)
    {
        var mod = QuickSellMod.Current;
        if (mod == null) return true;
        if (mod.IsRestocking) return false;
        var client = PlayerStore.Instance?.currentClientInstance?.GetClientBlueprint();
        if (client == null) return true;
        var items = EmporiumEntry.Instance.GetBackInvItems().Where(item => GeneralHelper.IsItemOwned(item) && client.IsClientBuyingThisItem(item)).ToList();
        return mod.ConfirmProtectedBatch(__instance, ___frame, items, false);
    }
}

[HarmonyPatch(typeof(NegociationUIManager), nameof(NegociationUIManager.SellMultiple))]
internal static class BatchDirectPatch
{
    private static bool Prefix(NegociationUIManager __instance, NegotiationTabletFrame ___frame, List<GameItem> items, bool ignoreBudget)
        => QuickSellMod.Current == null || QuickSellMod.Current.ConfirmProtectedBatch(__instance, ___frame, items, ignoreBudget);
}

[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.SellItemFromTable))]
internal static class SingleSaleClickPatch
{
    private static bool Prefix(PlayerStore __instance) => QuickSellMod.Current == null || __instance.currentNegociatedItem == null
        || QuickSellMod.Current.ConfirmProtectedSingle(__instance.currentNegociatedItem);
}

[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.SellItem))]
internal static class SaleCleanupPatch
{
    private static bool Prefix(GameItem item) => QuickSellMod.Current == null || QuickSellMod.Current.ConfirmProtectedSingle(item);
    private static void Postfix(GameItem item) => QuickSellMod.Current?.OnSold(item);
}

[HarmonyPatch(typeof(GameItemElement), nameof(GameItemElement.UpdateRectTransform))]
internal static class HintLayoutPatch
{
    private static void Postfix(GameItemElement __instance) => QuickSellMod.Current?.HintLayoutChanged(__instance);
}

[HarmonyPatch(typeof(GameItemElement), nameof(GameItemElement.ValidateSelect))]
internal static class HintStatePatch
{
    private static void Postfix() => QuickSellMod.Current?.HintsChanged();
}

[HarmonyPatch(typeof(GameItemElement), nameof(GameItemElement.Destroy))]
internal static class HintCleanupPatch
{
    private static void Prefix(GameItemElement __instance) => QuickSellMod.Current?.RemoveHint(__instance);
}

[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.LoadGame))]
internal static class LoadSessionPatch
{
    private static void Postfix() => QuickSellMod.Current?.ResetSession();
}

[HarmonyPatch(typeof(StoreEventManager), nameof(StoreEventManager.OnDayEnd))]
internal static class DayEndCleanupPatch
{
    private static void Postfix()
    {
        try { QuickSellMod.Current?.PruneStaleProtectionRecords(); }
        catch (System.Exception error) { Debug.LogException(error); }
    }
}
