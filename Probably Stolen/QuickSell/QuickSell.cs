using System.Linq;
using UnityEngine;

namespace QuickSell;

public sealed class QuickSellMod : IMod
{
    private ConfigEntry<string> selections = null!;
    private GameObject? runner;
    private GameObject? tabletButton;

    public void Init(ModManifest manifest)
    {
        selections = ModConfig.For(manifest.ID).Define("TradeZones", "", "交易区", () => "已选择的交易区（按存档保存）");
    }

    public void OnEnable()
    {
        runner = new GameObject("QuickSell");
        UnityEngine.Object.DontDestroyOnLoad(runner);
        runner.AddComponent<QuickSellRunner>().Mod = this;
    }

    public void OnDisable()
    {
        if (tabletButton != null) UnityEngine.Object.Destroy(tabletButton);
        CustomUIManager.Instance?.CloseWindow("quicksell_zones");
        if (runner != null) UnityEngine.Object.Destroy(runner);
    }

    internal void Update()
    {
        var negotiation = NegociationUIManager.Instance;
        if (negotiation == null || negotiation.uiObject == null) return;
        if (tabletButton == null)
        {
            var frame = negotiation.uiObject.GetComponent<NegotiationTabletFrame>();
            if (frame == null || frame.panel == null) return;
            var button = TabletUI.SmallButton(frame, frame.panel, 0, 0, 150, 38, "快捷补货", OpenWindow);
            tabletButton = button.gameObject;
            tabletButton.name = "QuickSellRestockButton";
            var rect = tabletButton.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(8f, -85f);
        }
    }

    private string Scope(PlayerStore store) => store.saveSlotId + ":" + store.runNumber;

    private void OpenWindow()
    {
        var store = PlayerStore.Instance;
        var entry = EmporiumEntry.Instance;
        if (store == null || entry == null || CustomUIManager.Instance == null) return;
        var keys = selections.Value.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        var prefix = Scope(store) + ":";
        var ids = keys.Where(key => key.StartsWith(prefix, System.StringComparison.Ordinal))
            .Select(key => int.TryParse(key.Substring(prefix.Length), out var id) ? id : 0).Where(id => id > 0).ToHashSet();
        var open = WindowsHandler.current.visibleWindows
            .SelectMany(window => GraphUtils.FindParentsType<GameItem>(window))
            .Where(item => item.contentWindow != null && item.contentWindow.IsVisible() && GeneralHelper.IsItemOwned(item))
            .Distinct().ToList();
        var ui = CustomUIManager.Instance.CreateWindow("quicksell_zones", "快捷补货", "overlay")
            .SetSize(560, 600).Center().SetDraggable(true).SetCloseOnEscape();
        ui.AddLabel("选择供货容器，补货只会摆上柜台，不会确认出售。");
        ui.AddButton("一键补货", Sell);
        ui.AddLabel("已选择的交易区：");
        ui.BeginScroll(180);
        if (ids.Count == 0) ui.AddLabel("尚未选择容器。");
        foreach (var id in ids)
        {
            var item = entry.FindItemByUniqueID(id);
            var label = item == null ? "不可用的容器 #" + id : item.GetDisplayName() + " #" + id;
            ui.AddButton("移除：" + label, () => ToggleZone(prefix + id));
        }
        ui.End();
        ui.AddLabel("当前打开的容器：");
        ui.BeginScroll(180);
        if (open.Count == 0) ui.AddLabel("请先打开箱子或背包，再点击刷新。");
        foreach (var item in open)
        {
            int id = item.GetUniqueID();
            if (!ids.Contains(id)) ui.AddButton("加入：" + item.GetDisplayName() + " #" + id, () => ToggleZone(prefix + id));
        }
        ui.End();
        ui.AddButton("刷新容器列表", OpenWindow);
        ui.AddButton("关闭", () => CustomUIManager.Instance.CloseWindow("quicksell_zones"));
        ui.Show();
    }

    private void ToggleZone(string key)
    {
        var keys = selections.Value.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        if (!keys.Remove(key)) keys.Add(key);
        selections.Set(string.Join(";", keys));
        OpenWindow();
    }

    private void Sell()
    {
        var store = PlayerStore.Instance;
        var entry = EmporiumEntry.Instance;
        var negotiation = NegociationUIManager.Instance;
        if (store == null || entry == null || negotiation == null || !store.isClientArrived || store.currentClientInstance == null || NegociationUIManager.IsTradeLocked())
        {
            StoreUIManager.Instance.Notify("当前没有可补货的顾客，或交易已锁定。", "default");
            return;
        }
        var client = store.currentClientInstance.GetClientBlueprint();
        if (client.isBusinessComplete || client.isRegularSellDisable || !client.IsClientBuying() || !client.useClientBudget || entry.backInvinvElement.IsInsertLocked())
        {
            StoreUIManager.Instance.Notify("顾客当前不购买、预算无限，或柜台已锁定。", "default");
            return;
        }
        var ids = selections.Value.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries)
            .Where(key => key.StartsWith(Scope(store) + ":", System.StringComparison.Ordinal))
            .Select(key => int.TryParse(key.Substring(key.LastIndexOf(':') + 1), out var id) ? id : 0).Where(id => id > 0).ToHashSet();
        if (ids.Count == 0)
        {
            StoreUIManager.Instance.Notify("尚未指定交易区：打开容器后在快捷补货窗口中选择。", "default");
            return;
        }
        var selected = ids.Select(entry.FindItemByUniqueID).Where(item => item != null).ToList();
        var sources = selected.Where(item => item.contentWindow != null)
            .SelectMany(item => GraphUtils.FindChildrenType<GameInventory>(item.contentWindow)).Distinct().ToList();
        var candidates = sources.Where(source => !source.IsRemoveLocked())
            .SelectMany(source => source.childItems.ToList()).Distinct().ToList();
        var table = entry.backInvinvElement;
        var existing = table.childItems.Where(item => GeneralHelper.IsItemOwned(item) && client.IsClientBuyingThisItem(item)).ToList();
        long remaining = client.GetBudget() - existing.Sum(item => (long)item.GetNegociatedValue());
        var offered = new System.Collections.Generic.List<GameItem>();
        var varieties = new System.Collections.Generic.HashSet<string>(existing.Select(item => item.identifier));
        foreach (var item in candidates)
        {
            if (remaining <= 0 || (client.isMultiBuyDisabled && (offered.Count > 0 || existing.Count > 0))) break;
            if (selected.Contains(item) || item.FindAllChildItems().Any(selected.Contains)) continue;
            if (!GeneralHelper.IsItemOwned(item) || client.IsClientRefusingItem(item) || !client.IsClientBuyingThisItem(item) || client.CanClientExposeAnyFeatureDeep(item)) continue;
            if (client.noAcceptingContraband && (ContrabandHelper.IsContraband(item) || StolenHelper.IsStolenItem(item))) continue;
            if (client.isRequireVariety && (client.IsItemInVarietyList(item) || varieties.Contains(item.identifier))) continue;
            if (item.GetNegociatedValue() <= 0 || item.GetNegociatedValue() > remaining) continue;
            var slot = table.TryFindOneValidInventorySlot(item);
            if (slot == null || slot.targetItem != null || slot.numTransfer < item.unitCount || item.MaxNumRemove() < item.unitCount) continue;
            var source = item.parentInventory;
            slot.TryAcceptOnce(item.unitCount);
            if (item.parentInventory != table) continue;
            var price = item.GetNegociatedValue();
            if (price <= 0 || price > remaining || !store.CanSellThisItem(item))
            {
                var restore = source.TryFindOneValidInventorySlot(item);
                if (restore != null && restore.targetItem == null && restore.numTransfer >= item.unitCount && item.MaxNumRemove() >= item.unitCount)
                    restore.TryAcceptOnce(item.unitCount);
                if (item.parentInventory != source)
                {
                    StoreUIManager.Instance.Notify("商品无法放回原容器，已停止补货，请检查柜台。", "default");
                    return;
                }
                continue;
            }
            offered.Add(item);
            varieties.Add(item.identifier);
            remaining -= price;
        }
        if (offered.Count == 0)
        {
            StoreUIManager.Instance.Notify("交易区内没有符合条件和剩余预算的商品。", "default");
            return;
        }
        negotiation.RefreshUI();
        StoreUIManager.Instance.Notify("已摆上 " + offered.Count + " 件商品，请检查后手动确认交易。", "default");
    }
}

public sealed class QuickSellRunner : MonoBehaviour
{
    internal QuickSellMod Mod = null!;
    private void Update() => Mod.Update();
}


