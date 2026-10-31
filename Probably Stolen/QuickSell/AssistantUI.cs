using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace QuickSell;

public sealed partial class QuickSellMod
{
    private RectTransform? actionRoot;
    private TabletButton? restockButton, sellButton, lastNativeSale;
    private bool wantsActions;
    internal CustomUIWindow? settings;
    private TabletSkin skin = null!;
    private RectTransform pageRoot = null!;
    private TextMeshProUGUI pageStatus = null!;
    private readonly TabletButton[] tabs = new TabletButton[4];
    internal int page;
    private int listPage;

    internal void BeginTabletBuild() => wantsActions = false;
    internal void EndTabletBuild()
    {
        if (actionRoot != null) actionRoot.gameObject.SetActive(wantsActions);
    }

    internal void AttachButtons(NegociationUIManager manager, NegotiationTabletFrame frame, TabletButton original, StoreClient client)
    {
        if (client.clientItemFeatureBuying != null && client.clientItemFeatureBuying.Contains("bribeDonation")) return;
        wantsActions = true;
        lastNativeSale = original;
        original.gameObject.SetActive(false);
        if (actionRoot == null || actionRoot.parent != frame.buttonHost.parent)
        {
            if (actionRoot != null) UnityEngine.Object.Destroy(actionRoot.gameObject);
            actionRoot = TabletUI.NewRect(frame.buttonHost.parent, "QuickSellActions", 0, 0, 562, 42);
            // Same geometry as BuildSelling's native purchase / wholesale buttons.
            restockButton = TabletUI.BigButton(frame, actionRoot, 19, 0, "一键上货", BeginRestock);
            restockButton.label.enableAutoSizing = false;
            restockButton.label.fontSize = 22;
            restockButton.gameObject.AddComponent<QuickSellSettingsPointer>().Mod = this;
            TabletUI.AddTooltip(restockButton.gameObject, () => (IsRestocking ? "左键：取消上货" : "左键：一键上货") + "　右键：设置\n" + status);
            sellButton = TabletUI.BigButton(frame, actionRoot, 287, 0, "出售物品", manager.OnSellAllClick);
            sellButton.label.enableAutoSizing = false;
            sellButton.label.fontSize = 22;
        }
        actionRoot.anchorMin = frame.buttonHost.anchorMin;
        actionRoot.anchorMax = frame.buttonHost.anchorMax;
        actionRoot.pivot = frame.buttonHost.pivot;
        actionRoot.anchoredPosition = frame.buttonHost.anchoredPosition;
        actionRoot.sizeDelta = frame.buttonHost.sizeDelta;
        if (sellButton!.label.text != original.label.text) sellButton.SetLabel(original.label.text);
        sellButton.SetInteractable(original.Interactable && !IsRestocking);
        UpdateActionState();
    }

    private void UpdateActionState()
    {
        if (restockButton == null) return;
        if (sellButton != null && lastNativeSale != null)
        {
            bool enabled = lastNativeSale.Interactable && !IsRestocking;
            if (sellButton.Interactable != enabled) sellButton.SetInteractable(enabled);
        }
    }

    internal void OpenWindow()
    {
        if (EmporiumEntry.Instance == null || EmporiumEntry.Instance.invElement == null || NegociationUIManager.Instance == null) return;
        HideHints();
        if (settings != null && settings.IsAlive) { settings.Show(); return; }
        skin = NegociationUIManager.Instance.tabletPrefab.GetComponent<NegotiationTabletFrame>();
        var template = new GameObject("QuickSellWindowTemplate", typeof(RectTransform), typeof(CustomUIWindowFrame));
        template.SetActive(false);
        settings = CustomUIManager.Instance.CreateWindow("quicksell_zones", null, "paper", template)
            .SetSize(860, 650).Center().SetDraggable(true).SetCloseOnEscape()
            .OnClose(() => { CancelBinding(); settings = null; ReplaceJob(0, null); }).GetWindow();
        UnityEngine.Object.Destroy(template);
        var root = settings.Rect;
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(.5f, .5f);
        var canvas = root.GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>().rect;
        float scale = Mathf.Min(1f, (canvas.width - 32) / 860, (canvas.height - 32) / 650);
        root.localScale = new Vector3(scale, scale, 1);
        foreach (var storeUI in root.GetComponents<StoreUI>()) storeUI.zoomable = false;
        root.GetComponent<UIDragHandler>()?.SetHomeScale(scale);
        var bezel = TabletUI.NewFlat(root, "BezelFill", 0, 0, 860, 650, new Color(.64f, .66f, .61f), true);
        TabletUI.Stretch(bezel.rectTransform);
        TabletUI.NewStretched(root, "BezelBorder", skin.frame2, TabletStyle.INK);
        // frame2 is a hollow border, not a filled paper sprite.
        var paper = TabletUI.NewFlat(root, "PaperFill", 10, 10, 840, 630, TabletStyle.SCREEN, true);
        paper.raycastTarget = true;
        TabletUI.Stretch(paper.rectTransform, 10);
        var paperBorder = TabletUI.NewStretched(root, "PaperBorder", skin.frame2, TabletStyle.INK);
        TabletUI.Stretch(paperBorder.rectTransform, 10);
        var title = Text(root, "更好的交易体验", 26, 22, 730, 42, AssistantTypography.Title);
        settings.frame.titleText = title;
        TabletUI.SmallButton(skin, root, 794, 20, 40, 38, "×", CloseSettings);
        string[] names = { "供货区", "物品保护", "快捷与高亮", "选货规则" };
        for (int i = 0; i < names.Length; i++)
        {
            int index = i;
            tabs[i] = TabletUI.BigButton(skin, root, 26 + 204 * i, 80, names[i], () => SwitchPage(index), 196);
            // Large native buttons ignore the selected flag; small-kind painting
            // supplies the game's persistent selected-tab colour without shrinking text.
            tabs[i].kind = TabletButton.Kind.Small;
        }
        pageRoot = TabletUI.NewRect(root, "Page", 26, 140, 808, 430);
        pageStatus = Text(root, "", 26, 584, 600, 42, 18);
        TabletUI.BigButton(skin, root, 650, 584, "关闭", CloseSettings, 184);
        AssistantTypography.Buttons(root);
        settings.Show();
        settings.SetTitle("更好的交易体验");
        SwitchPage(page);
    }

    private TextMeshProUGUI Text(Transform host, string value, float x, float y, float width, float height, float size = AssistantTypography.Body)
    {
        return AssistantTypography.Create(skin, host, value, x, y, width, height, size, TextAlignmentOptions.MidlineLeft);
    }

    private void CloseSettings()
    {
        CancelBinding();
        if (settings != null) settings.Close();
        settings = null;
        ReplaceJob(0, null);
    }

    private void SwitchPage(int index)
    {
        CancelBinding(); page = index; listPage = 0;
        ReplaceJob(0, null);
        for (int i = 0; i < tabs.Length; i++) tabs[i].SetSelected(i == page);
        RefreshPage();
        if (page == 0 || page == 1) StartDiscovery();
    }

    private void ClearPage()
    {
        for (int i = pageRoot.childCount - 1; i >= 0; i--)
        {
            var child = pageRoot.GetChild(i).gameObject;
            child.SetActive(false); UnityEngine.Object.Destroy(child);
        }
    }

    // 保护、放行与供货区记录可能指向没有被扫描到的库存位置（机器工作槽、文件柜、垃圾桶等），
    // 按编号从全量库存补齐名称，只有真正不存在的记录才显示为「暂不可用」。
    private void RefreshKnownItems()
    {
        var entry = EmporiumEntry.Instance;
        if (entry == null) return;
        bool missing = zoneIds.Any(id => !knownItems.ContainsKey(id))
            || protectedIds.Any(id => !knownItems.ContainsKey(id))
            || unprotectedIds.Any(id => !knownItems.ContainsKey(id));
        if (!missing) return;
        var lookup = new Dictionary<int, GameItem>();
        foreach (var item in entry.GetAllItems())
            if (item != null && item.uniqueId > 0 && !lookup.ContainsKey(item.uniqueId)) lookup[item.uniqueId] = item;
        int filled = 0;
        foreach (int id in zoneIds.Concat(protectedIds).Concat(unprotectedIds))
            if (!knownItems.ContainsKey(id) && lookup.TryGetValue(id, out var item))
            {
                knownItems[id] = item;
                filled++;
            }
        if (filled > 0) Debug.Log($"[QuickSell] 保护页按全量库存补齐 {filled} 条名称记录。");
    }

    private void RefreshPage()
    {
        if (settings == null || !settings.IsAlive) return;
        ClearPage();
        if (page == 0 || page == 1) RefreshKnownItems();
        pageStatus.text = page == 1 ? $"当前 {protectedIds.Count} 件物品、{protectedNames.Count} 个名称保护中" + (unprotectedIds.Count > 0 ? $"，{unprotectedIds.Count} 件单独放行" : "") : "自动保存";
        if (page == 2) { BuildHintsPage(); AssistantTypography.Buttons(pageRoot); return; }
        if (page == 3) { BuildRulesPage(); AssistantTypography.Buttons(pageRoot); return; }
        var rows = new List<(string Name, string Action, Action Click, GameItem? Item, bool Protect)>();
        if (page == 0)
        {
            foreach (int id in zoneIds.OrderBy(id => id))
            {
                int key = id;
                string name = knownItems.TryGetValue(id, out var item) ? item.GetDisplayName() : "暂不可用的容器";
                var choice = containers.FirstOrDefault(value => value.Item.uniqueId == id);
                if (choice.Item != null) name = choice.Path;
                rows.Add((name + " #" + id, "移除", () => { zoneIds.Remove(key); SaveIds(selections, zoneIds); RefreshPage(); }, null, false));
            }
            foreach (var choice in containers)
            {
                var item = choice.Item;
                int id = item.GetUniqueID();
                if (zoneIds.Contains(id)) continue;
                rows.Add((choice.Path + " #" + id, "加入", () => { zoneIds.Add(id); SaveIds(selections, zoneIds); RefreshPage(); }, null, false));
            }
            Text(pageRoot, $"已加入 {zoneIds.Count} 个供货区", 0, 0, 808, 30);
            Text(pageRoot, "主物品栏上货", 414, 0, 210, 30);
            var mainState = Text(pageRoot, mainInventory.Value ? "已开启" : "已关闭", 638, 0, 126, 30);
            mainState.alignment = TextAlignmentOptions.MidlineRight;
            var mainToggle = TabletUI.Checkbox(skin, pageRoot, 788, 5, mainInventory.Value,
                on => { mainInventory.Set(on); mainState.text = on ? "已开启" : "已关闭"; });
            TabletUI.AddTooltip(mainToggle.gameObject, () => "仅从主物品栏直接上货，不取容器或检索容器内部。");
        }
        else
        {
            var listed = new HashSet<int>();
            foreach (var item in EmporiumEntry.Instance.backInvinvElement.childItems)
            {
                if (!GeneralHelper.IsItemOwned(item)) continue;
                int id = item.uniqueId;
                if (id > 0) listed.Add(id);
                if (IsProtected(item)) continue;
                var target = item;
                if (id > 0 && unprotectedIds.Contains(id))
                {
                    rows.Add(("[已放行] " + item.GetDisplayName(), "恢复保护", () => RestoreProtected(target), target, false));
                    continue;
                }
                rows.Add((item.GetDisplayName(), "保护", () => Protect(target), target, true));
            }
            foreach (int id in unprotectedIds.OrderBy(id => id).Where(id => !listed.Contains(id)))
            {
                int key = id;
                string name = knownItems.TryGetValue(id, out var item) ? item.GetDisplayName() : "暂不可用的物品 #" + id;
                rows.Add(("[已放行] " + name, "恢复保护", () => RestoreProtected(key), null, false));
            }
            foreach (int id in protectedIds.OrderBy(id => id))
            {
                int key = id;
                string name = knownItems.TryGetValue(id, out var item) ? item.GetDisplayName() : "暂不可用的物品 #" + id;
                rows.Add(("[已保护] " + name, "解保", () => Unprotect(key), item, false));
            }
            foreach (string name in protectedNames.OrderBy(name => name, StringComparer.Ordinal))
            {
                string target = name;
                rows.Add(("[已保护] " + name + "（同类）", "解保", () => UnprotectSimilar(target), null, false));
            }
            Text(pageRoot, "受保护物品出售前需确认", 0, 0, 808, 30);
        }
        int pageCount = Math.Max(1, (rows.Count + 6) / 7);
        listPage = Math.Min(listPage, pageCount - 1);
        foreach (var row in rows.Skip(listPage * 7).Take(7).Select((value, index) => (value, index)))
        {
            float y = 40 + row.index * 44;
            var name = Text(pageRoot, row.value.Name, 0, y, page == 1 ? 474 : 634, 38);
            name.raycastTarget = true;
            TabletUI.AddTooltip(name.gameObject, () => row.value.Name);
            if (page == 1)
            {
                var item = row.value.Item;
                var batch = TabletUI.SmallButton(skin, pageRoot, 498, y, 150, 38, row.value.Protect ? "保护同类" : "解保同类",
                    () => { if (item != null) { if (row.value.Protect) ProtectSimilar(item.GetDisplayName()); else UnprotectSimilar(item.GetDisplayName()); } });
                batch.SetInteractable(item != null);
                TabletUI.AddTooltip(batch.gameObject, () => row.value.Protect
                    ? "按显示名称保护所有同名物品：之后获得的同名物品也不会自动上货。"
                    : "解除这类物品的同类保护：之后获得的同名物品恢复自动上货。");
            }
            TabletUI.SmallButton(skin, pageRoot, 658, y, 150, 38, row.value.Action, row.value.Click);
        }
        if (rows.Count == 0) Text(pageRoot, page == 0 ? "暂无容器，点击刷新。" : "暂无台面物品或保护记录。", 0, 55, 808, 40);
        TabletUI.SmallButton(skin, pageRoot, 0, 370, 150, 40, "刷新", StartDiscovery);
        var previous = TabletUI.SmallButton(skin, pageRoot, 508, 370, 100, 40, "上一页", () => { if (listPage > 0) { listPage--; RefreshPage(); } });
        previous.SetInteractable(listPage > 0);
        Text(pageRoot, $"{listPage + 1} / {pageCount}", 620, 370, 76, 40).alignment = TextAlignmentOptions.Midline;
        var next = TabletUI.SmallButton(skin, pageRoot, 708, 370, 100, 40, "下一页", () => { if (listPage + 1 < pageCount) { listPage++; RefreshPage(); } });
        next.SetInteractable(listPage + 1 < pageCount);
        if (IsScanning && page == 0) pageStatus.text = "正在扫描…";
        AssistantTypography.Buttons(pageRoot);
    }

    private void ToggleRow(float y, string label, ConfigEntry<bool> value)
    {
        Text(pageRoot, label, 0, y, 600, 44, AssistantTypography.Body);
        var state = Text(pageRoot, value.Value ? "已开启" : "已关闭", 638, y, 126, 44, AssistantTypography.Body);
        state.alignment = TextAlignmentOptions.MidlineRight;
        TabletUI.Checkbox(skin, pageRoot, 788, y + 12, value.Value,
            on => { value.Set(on); state.text = on ? "已开启" : "已关闭"; });
    }

    private void BuildHintsPage()
    {
        Text(pageRoot, "面板按一次，高亮按住显示", 0, 0, 808, 40, AssistantTypography.Body);
        BindingRow(45, "面板快捷键", panelKey);
        BindingRow(95, "一键上货快捷键", restockKey);
        BindingRow(145, "高亮快捷键", holdKey);
        ToggleRow(210, "顾客需求物品", demand);
        ToggleRow(270, "赃物", stolen);
        ToggleRow(330, "违禁物", contraband);
    }

    private void BindingRow(float y, string label, ConfigEntry<string> value)
    {
        TabletButton button = null!;
        button = TabletUI.SmallButton(skin, pageRoot, 448, y, 360, 42, BindingLabel(value.Value), () => BeginBinding(value, button));
        TabletUI.AddTooltip(button.gameObject, () => ReferenceEquals(value, panelKey) ? "按一次打开／关闭面板，无需顾客。" : ReferenceEquals(value, restockKey) ? "按一次上货，再按取消。" : "按住显示，松开隐藏。");
        Text(pageRoot, label, 0, y, 400, 42);
    }

    private void BuildRulesPage()
    {
        Text(pageRoot, "选货优先级", 0, 0, 400, 44, AssistantTypography.Body);
        TabletButton choice = null!;
        choice = TabletUI.SmallButton(skin, pageRoot, 448, 0, 360, 42, priority.Value,
            () => { priority.Set(priority.Value == "尽量用满预算" ? "优先腾空间" : "尽量用满预算"); choice.SetLabel(priority.Value); });
        TabletUI.AddTooltip(choice.gameObject, () => "尽量用满预算：优先总价；优先腾空间：优先释放库存占格。");
        Text(pageRoot, "允许超预算", 0, 65, 400, 44, AssistantTypography.Body);
        int[] amounts = { 0, 5, 10, 20 };
        var options = new List<(int Amount, TabletButton Button)>();
        for (int i = 0; i < amounts.Length; i++)
        {
            int amount = amounts[i];
            var button = TabletUI.SmallButton(skin, pageRoot, 448 + 92 * i, 65, 84, 42, amount == 0 ? "关" : amount.ToString(),
                () => { allowance.Set(amount.ToString(System.Globalization.CultureInfo.InvariantCulture)); foreach (var option in options) option.Button.SetSelected(option.Amount == amount); });
            button.SetSelected(AllowanceAmount == amount); options.Add((amount, button));
        }
        ToggleRow(120, "检索内层容器", nested);
        ToggleRow(170, "按需拆分堆叠", split);
        ToggleRow(220, "每种在货源保留一件", keepOne);
        RiskRow(280, "赃物", stolenRule);
        RiskRow(335, "违禁物", contrabandRule);
    }

    private void RiskRow(float y, string label, ConfigEntry<string> value)
    {
        Text(pageRoot, label, 0, y, 400, 42);
        TabletButton button = null!;
        button = TabletUI.SmallButton(skin, pageRoot, 448, y, 360, 42, value.Value, () =>
        {
            value.Set(value.Value == "正常选货" ? "低优先级" : value.Value == "低优先级" ? "不自动上货" : "正常选货");
            button.SetLabel(value.Value);
        });
        TabletUI.AddTooltip(button.gameObject, () => "点击切换：正常选货、低优先级、不自动上货。");
    }
}
