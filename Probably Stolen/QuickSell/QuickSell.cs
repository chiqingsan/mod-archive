
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;

namespace QuickSell;

public sealed partial class QuickSellMod : IMod
{
    internal static QuickSellMod? Current;
    internal ConfigEntry<string> selections = null!, protection = null!, nameProtection = null!, unprotected = null!, holdKey = null!, panelKey = null!, restockKey = null!;
    internal ConfigEntry<bool> nested = null!, split = null!, demand = null!, stolen = null!, contraband = null!, learning = null!, keepOne = null!, mainInventory = null!, menuProtection = null!;
    internal ConfigEntry<string> stolenRule = null!, contrabandRule = null!;
    internal ConfigEntry<string> allowance = null!;
    internal int AllowanceAmount => int.Parse(allowance.Value, System.Globalization.CultureInfo.InvariantCulture);
    internal ConfigEntry<string> priority = null!;
    private GameObject? runner;
    private readonly IEnumerator?[] jobs = new IEnumerator?[3];
    private readonly Stopwatch frameClock = new();
    private int jobCursor;
    private string scope = "";
    private PlayerStore? sessionStore;
    private int sessionSlot = -1, sessionRun = -1;
    private readonly HashSet<int> zoneIds = new(), protectedIds = new(), unprotectedIds = new();
    private readonly HashSet<string> protectedNames = new();
    internal readonly Dictionary<int, GameItem> knownItems = new();
    internal readonly List<(GameItem Item, string Path)> containers = new();
    internal string status = "尚未执行上货。";
    internal bool IsRestocking => jobs[1] != null;
    internal bool IsScanning => jobs[0] != null;

    public void Init(ModManifest manifest)
    {
        var config = ModConfig.For(manifest.ID);
        panelKey = config.Define("PanelKey", "LeftControl+L", "交易助手", () => "面板快捷键", tooltip: () => "默认 Ctrl+L 打开／关闭面板，进入存档后可用；也可在高亮物品页绑定。");
        restockKey = config.Define("RestockKey", "", "交易助手", () => "一键上货快捷键");
        menuProtection = config.Define("MenuProtection", true, "物品保护", () => "右键菜单显示保护按钮", tooltip: () => "与其它修改物品右键菜单的 Mod 冲突时可关闭；关闭后右键不再注入保护按钮，面板内保护照常可用。");
        mainInventory = config.Define("MainInventorySource", true, "供货区", () => "从主物品栏上货", tooltip: () => "仅选择主物品栏直接放置的非容器物品，不检索其内容。");
        nested = config.Define("SearchNested", true, "选货规则", () => "检索内层容器");
        split = config.Define("SplitStacks", true, "选货规则", () => "按需拆分堆叠（仅通过状态与报价校验的物品）");
        keepOne = config.Define("KeepOne", false, "选货规则", () => "每种在货源保留一件");
        stolenRule = config.Define("StolenRule", "正常选货", "选货规则", () => "赃物", new[] { "正常选货", "低优先级", "不自动上货" });
        contrabandRule = config.Define("ContrabandRule", "正常选货", "选货规则", () => "违禁物", new[] { "正常选货", "低优先级", "不自动上货" });
        priority = config.Define("SelectionPriority", "尽量用满预算", "选货规则", () => "选货优先级", new[] { "尽量用满预算", "优先腾空间" });
        allowance = config.Define("OverBudget", "0", "选货规则", () => "允许超预算金额", new[] { "0", "5", "10", "20" }, tooltip: () => "0 表示关闭；超出顾客预算的部分需手动确认抹零。");
        holdKey = config.Define("HoldKey", "N", "高亮物品", () => "按住查看快捷键", tooltip: () => "默认按住 N；也可在交易助手中绑定。");
        demand = config.Define("ShowDemand", true, "高亮物品", () => "顾客需求物品");
        stolen = config.Define("ShowStolen", true, "高亮物品", () => "赃物");
        contraband = config.Define("ShowContraband", true, "高亮物品", () => "违禁物");
        learning = config.Define("ShowLearning", false, "高亮物品", () => "自学习模组");
        // 内部存档记录不注册进本 Mod 的可见设置；旧版本存在主配置里的记录会一次性迁移。
        InitRecordConfig(config);
    }

    private const string RecordConfigSuffix = ".records";

    // 记录单独放在 ModConfigs/chiqingsan.QuickSell.records.json：原版 Mod 设置页只列主配置条目，
    // 内部记录因此不会显示成可编辑的长输入框；写入仍走游戏自带的 ModConfig，不需要额外的文件权限。
    private void InitRecordConfig(ModConfig mainConfig)
    {
        bool useMainConfig = false;
        try
        {
            var records = ModConfig.For(mainConfig.ModId + RecordConfigSuffix);
            selections = records.Define("TradeZones", "", "记录", () => "供货区记录");
            protection = records.Define("ProtectedItems", "", "记录", () => "物品保护记录");
            nameProtection = records.Define("ProtectedNames", "", "记录", () => "同类保护记录");
            unprotected = records.Define("UnprotectedItems", "", "记录", () => "单件解保记录");
            useMainConfig = !TryMigrateLegacyRecords(mainConfig);
        }
        catch (Exception error)
        {
            Debug.LogWarning("[QuickSell] 独立记录文件不可用，记录仍保存在主配置：" + error.Message);
            useMainConfig = true;
        }
        if (!useMainConfig) return;
        // 回退：记录继续放主配置（会显示在设置页，但不会丢数据）。
        selections = mainConfig.Define("TradeZones", "", "存档数据", () => "供货区记录（请在交易助手内修改）");
        protection = mainConfig.Define("ProtectedItems", "", "存档数据", () => "物品保护记录（请在交易助手内修改）");
        nameProtection = mainConfig.Define("ProtectedNames", "", "存档数据", () => "同类保护记录（请在交易助手内修改）");
        unprotected = mainConfig.Define("UnprotectedItems", "", "存档数据", () => "单件解保记录（请在交易助手内修改）");
    }

    /// <summary>把 1.3.1 及更早版本存在主配置里的记录搬到独立配置，并从主配置里删掉旧键。</summary>
    private bool TryMigrateLegacyRecords(ModConfig mainConfig)
    {
        if (!string.IsNullOrEmpty(selections.Value) || !string.IsNullOrEmpty(protection.Value)
            || !string.IsNullOrEmpty(nameProtection.Value) || !string.IsNullOrEmpty(unprotected.Value)) return true;
        Dictionary<string, string>? raw;
        try
        {
            raw = typeof(ModConfig).GetField("rawValues", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.GetValue(mainConfig) as Dictionary<string, string>;
        }
        catch (Exception error)
        {
            Debug.LogWarning("[QuickSell] 读取旧记录失败，本次不迁移：" + error.Message);
            return false;
        }
        if (raw == null) return false;
        int moved = 0;
        void Move(ConfigEntry<string> target, string key)
        {
            if (!raw.TryGetValue(key, out var value) || string.IsNullOrEmpty(value)) return;
            target.Set(value);
            raw.Remove(key);
            moved++;
        }
        Move(selections, "TradeZones");
        Move(protection, "ProtectedItems");
        Move(nameProtection, "ProtectedNames");
        Move(unprotected, "UnprotectedItems");
        if (moved == 0) return true;
        mainConfig.Save();
        Debug.Log($"[QuickSell] 已把 {moved} 条旧记录迁移到独立配置文件。");
        return true;
    }

    private long clearRecordsUntil;

    /// <summary>清空本存档的供货区、物品保护与单件解保记录；3 秒内再次点击确认。</summary>
    internal void ClearAllRecords()
    {
        long now = DateTime.UtcNow.Ticks;
        if (now > clearRecordsUntil)
        {
            clearRecordsUntil = now + TimeSpan.FromSeconds(3).Ticks;
            SetStatus("再次点击「清空记录」确认清空本存档的供货区与保护记录。", true);
            return;
        }
        clearRecordsUntil = 0;
        zoneIds.Clear(); protectedIds.Clear(); protectedNames.Clear(); unprotectedIds.Clear();
        SaveIds(selections, zoneIds);
        SaveIds(protection, protectedIds);
        SaveNames();
        SaveIds(unprotected, unprotectedIds);
        Debug.Log("[QuickSell] 已清空本存档的供货区与保护记录。");
        SetStatus("已清空本存档的供货区与保护记录。", true);
        if (settings != null && settings.IsAlive && (page == 0 || page == 1)) RefreshPage();
    }

    public void OnEnable()
    {
        Current = this;
        runner = new GameObject("QuickSell");
        UnityEngine.Object.DontDestroyOnLoad(runner);
        runner.AddComponent<QuickSellRunner>().Mod = this;
        try
        {
            // The built-in loader installs these attributes before OnEnable and
            // removes its own "mod.<ID>" patches after OnDisable.
            NativeContracts.CheckParenting();
            HintArtwork.Load();
        }
        catch { OnDisable(); throw; }
        InvalidateTablet();
    }

    public void OnDisable()
    {
        if (runner != null) runner.SetActive(false);
        for (int i = 0; i < jobs.Length; i++) ReplaceJob(i, null);
        HideHints(); DestroyHints(); CloseSettings();
        DestroyProtectedDialog();
        HintArtwork.Unload();
        RestoreProtectionMenu();
        if (actionRoot != null) { actionRoot.gameObject.SetActive(false); UnityEngine.Object.Destroy(actionRoot.gameObject); }
        actionRoot = null;
        if (lastNativeSale != null) lastNativeSale.gameObject.SetActive(true);
        lastNativeSale = null; restockButton = null; sellButton = null;
        knownItems.Clear(); containers.Clear(); sessionStore = null;
        Current = null;
        InvalidateTablet();
        if (runner != null) UnityEngine.Object.Destroy(runner);
        runner = null;
    }

    private static void InvalidateTablet()
    {
        var ui = NegociationUIManager.Instance;
        if (ui == null) return;
        AccessTools.Field(typeof(NegociationUIManager), "builtSignature").SetValue(ui, null);
        ui.RefreshUI();
    }

    internal void ResetSession()
    {
        for (int i = 0; i < jobs.Length; i++) ReplaceJob(i, null);
        knownItems.Clear(); containers.Clear(); HideHints(); DestroyHints(); CloseSettings(); DestroyProtectedDialog(); RestoreProtectionMenu();
        scope = ""; sessionStore = null;
    }

    internal void Tick()
    {
        // Instance creates a PlayerStore when absent. Never call it before the store scene has built its inventory.
        var entry = EmporiumEntry.Instance;
        var store = entry != null && entry.invElement != null ? PlayerStore.Instance : null;
        if (store != sessionStore || (store != null && (store.saveSlotId != sessionSlot || store.runNumber != sessionRun)))
        {
            for (int i = 0; i < jobs.Length; i++) ReplaceJob(i, null);
            sessionStore = store;
            sessionSlot = store == null ? -1 : store.saveSlotId;
            sessionRun = store == null ? -1 : store.runNumber;
            scope = store == null ? "" : sessionSlot + ":" + sessionRun;
            LoadIds(selections.Value, zoneIds); LoadIds(protection.Value, protectedIds); LoadIds(unprotected.Value, unprotectedIds); LoadNames(nameProtection.Value);
            knownItems.Clear(); containers.Clear(); HideHints(); DestroyHints(); CloseSettings(); DestroyProtectedDialog(); RestoreProtectionMenu();
        }
        TickInput();
        frameClock.Restart();
        int idle = 0;
        while (frameClock.Elapsed.TotalMilliseconds < 1 && idle < jobs.Length)
        {
            int index = jobCursor;
            jobCursor = (jobCursor + 1) % jobs.Length;
            var job = jobs[index];
            if (job == null) { idle++; continue; }
            idle = 0;
            try { if (!job.MoveNext() && jobs[index] == job) ReplaceJob(index, null); }
            catch (Exception error)
            {
                ReplaceJob(index, null);
                SetStatus("操作失败，详情已记录到游戏日志。", index == 1);
                Debug.LogException(error);
            }
        }
        UpdateActionState();
    }

    private void ReplaceJob(int index, IEnumerator? job)
    {
        (jobs[index] as IDisposable)?.Dispose();
        jobs[index] = job;
    }

    private void LoadIds(string text, HashSet<int> ids)
    {
        ids.Clear();
        foreach (var key in text.Split(';'))
        {
            if (!key.StartsWith(scope + ":", StringComparison.Ordinal)) continue;
            if (int.TryParse(key.Substring(scope.Length + 1), out int id) && id > 0) ids.Add(id);
            else Debug.LogWarning("[QuickSell] 无效的物品记录：" + key);
        }
    }

    private void SaveIds(ConfigEntry<string> config, HashSet<int> ids)
    {
        var other = config.Value.Split(';').Where(key => key.Length > 0 && !key.StartsWith(scope + ":", StringComparison.Ordinal));
        config.Set(string.Join(";", other.Concat(ids.OrderBy(id => id).Select(id => scope + ":" + id))));
    }

    private void LoadNames(string text)
    {
        protectedNames.Clear();
        foreach (var key in text.Split(';'))
        {
            if (!key.StartsWith(scope + ":", StringComparison.Ordinal)) continue;
            string name;
            try { name = Uri.UnescapeDataString(key.Substring(scope.Length + 1)); }
            catch (Exception)
            {
                Debug.LogWarning("[QuickSell] 无效的同类保护记录：" + key);
                continue;
            }
            if (name.Length > 0) protectedNames.Add(name);
        }
    }

    private void SaveNames()
    {
        var other = nameProtection.Value.Split(';').Where(key => key.Length > 0 && !key.StartsWith(scope + ":", StringComparison.Ordinal));
        nameProtection.Set(string.Join(";", other.Concat(protectedNames.OrderBy(name => name, StringComparer.Ordinal).Select(name => scope + ":" + Uri.EscapeDataString(name)))));
    }

    internal bool IsNameProtected(GameItem item) => protectedNames.Count > 0 && protectedNames.Contains(item.GetDisplayName());
    // 先看单件是否单独解保，再看实例保护，最后按显示名称判断同类保护。
    internal bool IsProtected(GameItem item)
    {
        if (item.uniqueId > 0)
        {
            if (unprotectedIds.Contains(item.uniqueId)) return false;
            if (protectedIds.Contains(item.uniqueId)) return true;
        }
        return IsNameProtected(item);
    }
    internal void Protect(GameItem item)
    {
        int id = item.GetUniqueID(); knownItems[id] = item;
        bool released = unprotectedIds.Remove(id);
        protectedIds.Add(id); SaveIds(protection, protectedIds);
        if (released) SaveIds(unprotected, unprotectedIds);
        RefreshPage();
    }
    internal void Unprotect(int id)
    {
        bool released = unprotectedIds.Remove(id);
        protectedIds.Remove(id); SaveIds(protection, protectedIds);
        if (released) SaveIds(unprotected, unprotectedIds);
        RefreshPage();
    }

    // 单件解保：只放行这一件，同类保护保留，之后获得的同名物品仍受保护。
    internal void ReleaseProtected(GameItem item)
    {
        int id = item.GetUniqueID(); knownItems[id] = item;
        bool released = unprotectedIds.Add(id);
        bool removed = protectedIds.Remove(id);
        if (released) SaveIds(unprotected, unprotectedIds);
        if (removed) SaveIds(protection, protectedIds);
        RefreshPage();
    }

    internal void RestoreProtected(GameItem item)
    {
        int id = item.GetUniqueID(); knownItems[id] = item;
        bool released = unprotectedIds.Remove(id);
        bool added = false;
        if (!IsProtected(item)) { protectedIds.Add(id); added = true; }
        if (released) SaveIds(unprotected, unprotectedIds);
        if (added) SaveIds(protection, protectedIds);
        RefreshPage();
    }

    internal void RestoreProtected(int id)
    {
        if (unprotectedIds.Remove(id)) SaveIds(unprotected, unprotectedIds);
        RefreshPage();
    }

    // 同类保护按显示名称记录，之后获得的同名物品会自动生效，不再依赖保护时的实例编号。
    internal void ProtectSimilar(string name)
    {
        if (name.Length == 0) return;
        if (IsRestocking || IsScanning) { SetStatus("请等待当前操作完成。", true); return; }
        bool changed = protectedNames.Add(name);
        if (changed) SaveNames();
        RefreshPage();
        if (settings != null && settings.IsAlive && page == 1)
            pageStatus.text = changed ? "已保护同类：" + name + "（之后获得的同名物品也会保护）" : "该类物品已在保护中：" + name;
    }

    internal void UnprotectSimilar(string name)
    {
        if (name.Length == 0) return;
        if (IsRestocking || IsScanning) { SetStatus("请等待当前操作完成。", true); return; }
        bool changed = protectedNames.Remove(name);
        foreach (int id in protectedIds.ToArray())
            if (knownItems.TryGetValue(id, out var item) && string.Equals(item.GetDisplayName(), name, StringComparison.Ordinal))
            {
                protectedIds.Remove(id);
                changed = true;
            }
        bool released = false;
        foreach (int id in unprotectedIds.ToArray())
            if (knownItems.TryGetValue(id, out var item) && string.Equals(item.GetDisplayName(), name, StringComparison.Ordinal))
                released |= unprotectedIds.Remove(id);
        if (changed) { SaveNames(); SaveIds(protection, protectedIds); }
        if (released) SaveIds(unprotected, unprotectedIds);
        RefreshPage();
        if (settings != null && settings.IsAlive && page == 1)
            pageStatus.text = changed ? "已解除同类保护：" + name : "未找到该类的保护记录：" + name;
    }

    internal void OnSold(GameItem item)
    {
        if (GeneralHelper.IsItemOwned(item)) return;
        bool changed = protectedIds.Remove(item.uniqueId);
        bool released = unprotectedIds.Remove(item.uniqueId);
        knownItems.Remove(item.uniqueId);
        foreach (var child in item.FindAllChildItems())
        {
            changed |= protectedIds.Remove(child.uniqueId);
            released |= unprotectedIds.Remove(child.uniqueId);
            knownItems.Remove(child.uniqueId);
        }
        if (changed) SaveIds(protection, protectedIds);
        if (released) SaveIds(unprotected, unprotectedIds);
        if ((changed || released) && settings != null && page == 1) RefreshPage();
    }

    // 每天结算时清理一次失效记录：物品被用掉、卖掉或丢掉后，其保护与单件解保记录不再有意义。
    internal void PruneStaleProtectionRecords()
    {
        if (protectedIds.Count == 0 && unprotectedIds.Count == 0) return;
        var entry = EmporiumEntry.Instance;
        if (entry == null) return;
        var existing = new HashSet<int>();
        try
        {
            foreach (var item in StoreGraph.GetAllItems())
                if (item != null && item.uniqueId > 0) existing.Add(item.uniqueId);
        }
        catch (Exception error)
        {
            Debug.LogWarning("[QuickSell] 每天结算清理读取商店物品失败，本次跳过：" + error);
            return;
        }
        int removed = protectedIds.RemoveWhere(id => !existing.Contains(id));
        int released = unprotectedIds.RemoveWhere(id => !existing.Contains(id));
        foreach (int id in knownItems.Keys.ToArray())
            if (!existing.Contains(id)) knownItems.Remove(id);
        if (removed == 0 && released == 0) return;
        if (removed > 0) SaveIds(protection, protectedIds);
        if (released > 0) SaveIds(unprotected, unprotectedIds);
        if (settings != null && settings.IsAlive && page == 1)
        {
            RefreshPage();
            pageStatus.text = "已清理 " + (removed + released) + " 条失效记录。" + pageStatus.text;
        }
        Debug.Log("[QuickSell] 每天结算清理：失效保护记录 " + removed + " 条、失效单件解保记录 " + released + " 条，剩余 " + protectedIds.Count + " 件物品、" + protectedNames.Count + " 个名称、" + unprotectedIds.Count + " 件单件放行。");
    }

    internal bool HasProtectedContent(GameItem item)
        => (protectedIds.Count > 0 || protectedNames.Count > 0)
            && (IsProtected(item) || (item.contentWindow != null && item.FindAllChildItems().Any(IsProtected)));

    internal void SetStatus(string value, bool notify = false)
    {
        status = value;
        if (notify && StoreUIManager.Instance != null) StoreUIManager.Instance.Notify(value, "default");
    }
}

public sealed class QuickSellRunner : MonoBehaviour
{
    internal QuickSellMod Mod = null!;
    private void Update() => Mod.Tick();
    private void LateUpdate() => Mod.TickDraggedHint();
}

public sealed class QuickSellSettingsPointer : MonoBehaviour, IPointerClickHandler
{
    internal QuickSellMod Mod = null!;
    public void OnPointerClick(PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right) Mod.OpenWindow();
    }
}
