using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UndergroundExchangeAssistant;

public sealed class ExchangeMod : IMod
{
    private GameObject? runner;
    public void Init(ModManifest manifest) { }
    public void OnEnable()
    {
        runner = new GameObject("UndergroundExchangeAssistant");
        UnityEngine.Object.DontDestroyOnLoad(runner);
        runner.AddComponent<ExchangeAssistantUI>();
    }
    public void OnDisable()
    {
        if (runner != null) { runner.SetActive(false); UnityEngine.Object.Destroy(runner); }
        runner = null;
    }
}

public sealed class ExchangeAssistantUI : MonoBehaviour
{
    private Button? entrance;
    private Sprite? paper;
    private Texture2D? paperTexture;
    private Sprite? outline;
    private Texture2D? outlineTexture;
    private GameObject? hoverVisual;
    private GameItem? trashcan;
    private PlayerStore? store;
    private TabletSkin skin = null!;
    private float nextRefresh;
    private CustomUIWindow? confirmation;
    private readonly ExchangeWindow window = new();
    private readonly Dictionary<string, string> tips = new();
    private int lastSignature;
    // Slow loads can hand us inventories before their items are deserialized;
    // while the window is open, an empty scan is retried for a while instead of
    // leaving every row disabled until the player reopens it.
    private float scanRetryUntil;
    // SetAsLastSibling alone marks the canvas dirty, so the per-frame layout
    // pass skips it while the entrance already sits on top.
    private int laidOutHoverSibling = -1;

    private bool IsOpen => window.IsOpen;

    private void Update()
    {
        EmporiumEntry? entry = EmporiumEntry.Instance;
        if (entry == null || entry.invElement == null || ExchangeUIManager.Instance == null
            || NegociationUIManager.Instance == null || NegociationUIManager.Instance.tabletPrefab == null
            || RenderHandler.current == null || UISettings.current == null || CustomUIManager.Instance == null
            || OverlayHandler.current == null || OverlayHandler.current.GetTooltipWindow() == null)
        { ResetSession(); return; }
        if (store != PlayerStore.Instance || trashcan != entry.trashcan)
        { ResetSession(); store = PlayerStore.Instance; trashcan = entry.trashcan; }
        if (!GameMaterials.IsExchangeVisible(PlayerStore.Instance))
        {
            if (entrance != null) entrance.gameObject.SetActive(false);
            if (hoverVisual != null) hoverVisual.SetActive(false);
            if (IsOpen) CloseWindow();
            return;
        }
        if (entry.trashcan is not GameItemElement item) return;
        if (skin == null)
        {
            skin = NegociationUIManager.Instance.tabletPrefab.GetComponent<NegotiationTabletFrame>();
            if (skin == null) return;
        }
        if (entrance == null) BuildEntrance(item);
        if (!entrance!.gameObject.activeSelf) entrance.gameObject.SetActive(true);
        LayoutEntrance(item);
        if (!IsOpen) return;
        if (confirmation != null) return;
        if (Time.unscaledTime < nextRefresh) return;
        nextRefresh = Time.unscaledTime + 1f;
        // Planning walks the whole inventory and probes the trashcan grid, so
        // only rebuild the button states when something actually changed. The
        // context is built once and shared by every row of this refresh.
        ExchangeTransfer.ExchangeContext context;
        try
        {
            context = ExchangeTransfer.CreateContext();
        }
        catch (Exception error)
        {
            Debug.LogWarning("[UndergroundExchange] 材料扫描失败：" + error.Message);
            return;
        }
        int signature = Signature(entry, context);
        bool retryEmpty = context.Candidates.Count == 0 && Time.unscaledTime < scanRetryUntil;
        if (signature == lastSignature && !retryEmpty) return;
        lastSignature = signature;
        if (retryEmpty) nextRefresh = Time.unscaledTime + .5f;
        Refresh(context);
    }

    private void BuildEntrance(GameItemElement item)
    {
        if (paper == null)
        {
            LoadPaper();
        }
        // The empty TreeNodeRender is what keeps the trashcan's own double click,
        // drag and tooltip handlers from seeing this paper as the trashcan: the
        // game's raycast stops at the first TreeNodeRender, which has no parent
        // GameItem, so the click only reaches our Button.
        // A plain uGUI image is used for the sheet itself: it renders above the
        // trashcan art exactly like the hover outline does. The TreeNodeRender
        // below is only a raycast stopper, so the game's item handlers cannot
        // reach the trashcan through the paper.
        var go = new GameObject("UndergroundExchangePaper",
            typeof(RectTransform), typeof(Image), typeof(Button), typeof(TreeNodeRender),
            typeof(PaperPointerBlocker), typeof(PaperHover));
        go.transform.SetParent(item.background.rectTransform, false);
        var image = go.GetComponent<Image>();
        image.sprite = paper; image.color = Color.white; image.preserveAspect = true; image.raycastTarget = true;
        image.alphaHitTestMinimumThreshold = .1f;
        hoverVisual = BuildHoverVisual(item);
        laidOutHoverSibling = -1;
        entrance = go.GetComponent<Button>(); entrance.targetGraphic = image;
        entrance.transition = Selectable.Transition.None;
        entrance.onClick.AddListener(Open);
        go.GetComponent<PaperHover>().Init(hoverVisual);
        LayoutEntrance(item);
    }

    private void LoadPaper()
    {
        using var input = typeof(ExchangeMod).Assembly.GetManifestResourceStream("UndergroundExchangeAssistant.Paper.png")!;
        using var bytes = new MemoryStream(); input.CopyTo(bytes);
        var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        try
        {
            if (!ImageConversion.LoadImage(source, bytes.ToArray())) throw new InvalidDataException("材料单图片无法解码");
            var pixels = source.GetPixels32().Select(c => new PaperArtwork.Pixel(c.r, c.g, c.b, c.a)).ToArray();
            var art = PaperArtwork.Build(pixels, source.width, source.height);
            paperTexture = MakeTexture(art.Normal, art.Width, art.Height, "UndergroundExchangePaper");
            outlineTexture = MakeTexture(art.Outline, art.Width, art.Height, "UndergroundExchangeOutline");
            var bounds = new Rect(0, 0, art.Width, art.Height);
            paper = Sprite.Create(paperTexture, bounds, new Vector2(.5f, .5f), 1);
            outline = Sprite.Create(outlineTexture, bounds, new Vector2(.5f, .5f), 1);
        }
        finally { UnityEngine.Object.Destroy(source); }
    }

    private static Texture2D MakeTexture(PaperArtwork.Pixel[] pixels, int width, int height, string name)
    {
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        { name = name, filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
        texture.SetPixels32(pixels.Select(c => new Color32(c.R, c.G, c.B, c.A)).ToArray()); texture.Apply();
        return texture;
    }

    // A silhouette-only overlay shares the normal sprite's padded canvas.
    private GameObject BuildHoverVisual(GameItemElement item)
    {
        var go = new GameObject("UndergroundExchangeHover", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(item.background.rectTransform, false);
        var image = go.GetComponent<Image>();
        image.sprite = outline;
        image.color = Color.white;
        image.preserveAspect = true;
        image.raycastTarget = false;
        go.SetActive(false);
        return go;
    }

    private void LayoutEntrance(GameItemElement item)
    {
        if (entrance == null || paper == null) return;
        var rect = (RectTransform)entrance.transform;
        float width = item.background.rectTransform.rect.width * PaperArtwork.RelativeWidth;
        float height = width * paper.rect.height / paper.rect.width;
        var size = new Vector2(width, height);
        if (rect.sizeDelta != size || rect.anchorMin.x != PaperArtwork.AnchorX)
        {
            // Centred on the painted front panel of the bin: the background rect
            // of the sprite is taller than the artwork, so the anchors are measured.
            rect.anchorMin = rect.anchorMax = new Vector2(PaperArtwork.AnchorX, PaperArtwork.AnchorY);
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
        }
        if (rect.GetSiblingIndex() != rect.parent.childCount - 1) rect.SetAsLastSibling();
        if (hoverVisual != null)
        {
            var hoverRect = (RectTransform)hoverVisual.transform;
            var hoverSize = size * PaperArtwork.HoverScale;
            if (hoverRect.sizeDelta != hoverSize)
            {
                hoverRect.anchorMin = hoverRect.anchorMax = new Vector2(PaperArtwork.AnchorX, PaperArtwork.AnchorY);
                hoverRect.pivot = new Vector2(.5f, .5f);
                hoverRect.anchoredPosition = Vector2.zero;
                hoverRect.sizeDelta = hoverSize;
            }
            // The rim highlight sits on top of the sheet, so it stays visible
            // against the pink paper.
            int last = hoverRect.parent.childCount - 1;
            if (laidOutHoverSibling != last || hoverRect.GetSiblingIndex() != last)
            {
                laidOutHoverSibling = last;
                hoverRect.SetAsLastSibling();
            }
        }
    }

    private void Open()
    {
        if (!GameMaterials.IsExchangeVisible(PlayerStore.Instance)) return;
        if (IsOpen) return;
        try
        {
            var offers = BuildOffers();
            if (offers.Count == 0) { Notify("交易清单尚未就绪"); return; }
            if (skin == null) { Notify("交易窗口尚未就绪"); return; }
            window.Open(skin, offers, Tooltip, id => Exchange(id, false), CloseWindow);
            if (hoverVisual != null) hoverVisual.SetActive(false);
            if (TooltipManager._instance != null)
                TooltipManager._instance.HideTooltip(entrance!.GetComponent<TabletTooltipTrigger>());
            // The first update tick recomputes the row states from a single
            // inventory scan; opening only builds the layout.
            lastSignature = 0;
            scanRetryUntil = Time.unscaledTime + 15f;
            nextRefresh = Time.unscaledTime + .2f;
            Refresh();
        }
        catch (Exception error)
        {
            Debug.LogError("[UndergroundExchangeAssistant] " + error);
            Notify("交易窗口初始化失败");
            CloseWindow();
        }
    }

    // Source artwork for the rows comes from the shipped exchange panel, but the
    // window itself is drawn by the mod.
    private static List<(string Id, Transform Source)> BuildOffers()
    {
        var result = new List<(string, Transform)>();
        var exchange = ExchangeUIManager.Instance;
        if (exchange == null || exchange.panel == null) return result;
        var panel = exchange.panel.transform;
        var section = panel.GetComponentsInChildren<RectTransform>(true).Single(t => t.name == "PermaSection");
        var permanent = section.Cast<Transform>()
            .Where(t => t.name.StartsWith("ExchangeElement", StringComparison.Ordinal))
            .OrderBy(t => -t.GetComponent<RectTransform>().anchoredPosition.y).ToArray();
        if (permanent.Length != NativeOfferRows.PermanentIds.Length)
            throw new InvalidOperationException("原版常驻交易行结构发生变化");
        for (int i = 0; i < permanent.Length; i++)
            result.Add((NativeOfferRows.PermanentIds[i], permanent[i]));

        var promotional = exchange.promotionalSection;
        var active = PlayerStore.Instance?.exchangeManager?.activePromotionalBarterIds;
        if (promotional == null || active == null) return result;
        foreach (Transform row in promotional.transform)
        {
            var element = row.GetComponent<ExchangeElement>();
            if (element == null || string.IsNullOrEmpty(element.identifier)) continue;
            if (!active.Contains(element.identifier)) continue;
            result.Add((element.identifier!, row));
        }
        return result;
    }

    private static int Signature(EmporiumEntry entry, ExchangeTransfer.ExchangeContext context)
    {
        try
        {
            var manager = PlayerStore.Instance?.exchangeManager;
            if (manager == null) return 0;
            var janitor = StoreService.GetJanitorialService();
            int hash = 17;
            hash = hash * 31 + (manager.isExchangeUnlocked ? 1 : 0);
            hash = hash * 31 + (janitor != null && janitor.isUsing ? 1 : 0);
            hash = hash * 31 + (StoreReputation.IsPerkUnlocked("BM_DISTRUSTED") ? 1 : 0);
            var promos = manager.activePromotionalBarterIds;
            if (promos != null) foreach (var id in promos) hash = hash * 31 + id.GetHashCode();
            foreach (var item in context.Candidates) hash = hash * 31 + ItemHash(item);
            foreach (var trashEntry in context.Trash) hash = hash * 31 + ItemHash(trashEntry.Item);
            return hash;
        }
        catch (Exception)
        {
            // Signature only decides when to refresh the labels; a damaged save
            // must not take the update loop down.
            return 0;
        }
    }

    private static int ItemHash(GameItem item)
    {
        int hash = item.identifier.GetHashCode();
        hash = hash * 31 + item.unitCount;
        hash = hash * 31 + (item.parentInventory != null ? item.parentInventory.GetHashCode() : 0);
        return hash;
    }

    private string Tooltip(string id)
    {
        if (tips.TryGetValue(id, out var cached)) return cached;
        try
        {
            using var plan = new ExchangeTransfer(id);
            return Describe(plan);
        }
        catch (Exception)
        {
            return "物品状态异常，暂时无法准备该交易的材料。";
        }
    }

    private static string Describe(ExchangeTransfer plan)
    {
        string text = plan.Reason.Length == 0
            ? "准备一份材料，每晚交换一次；奖励由夜间清洁服务发放。检索玩家各库存及内层容器，不取机器内物品。"
            : plan.Reason;
        if (plan.LostBayContents > 0)
            text += "\n注意：交换会销毁走私者暗格内的 " + plan.LostBayContents + " 件物品。";
        return text;
    }

    private void Refresh()
    {
        try
        {
            Refresh(ExchangeTransfer.CreateContext());
        }
        catch (Exception error)
        {
            Debug.LogWarning("[UndergroundExchange] 材料扫描失败：" + error.Message);
        }
    }

    private void Refresh(ExchangeTransfer.ExchangeContext context)
    {
        tips.Clear();
        foreach (var id in window.RowIds)
        {
            try
            {
                using var plan = new ExchangeTransfer(id, context);
                bool ready = plan.Reason.Length == 0;
                string label = ready ? "交换" : plan.Reason == "垃圾桶已备齐" ? "已备齐" : "不可用";
                string tip = Describe(plan);
                tips[id] = tip;
                window.SetRowState(id, ready, label, tip);
            }
            catch (Exception error)
            {
                Debug.LogWarning("[UndergroundExchange] 交易行状态计算失败 " + id + ": " + error.Message);
                string tip = "物品状态异常，暂时无法准备该交易的材料。";
                tips[id] = tip;
                window.SetRowState(id, false, "不可用", tip);
            }
        }
    }

    private void Exchange(string id, bool confirmed)
    {
        try
        {
            using var plan = new ExchangeTransfer(id);
            if (plan.Reason.Length != 0) { Notify(plan.Reason); Refresh(); return; }
            if (plan.LostBayContents > 0 && !confirmed) { ConfirmBay(id, plan.LostBayContents); return; }
            plan.Commit();
            CloseConfirmation();
            Notify("已投入一份材料，等待夜间清洁服务结算。");
            Refresh();
        }
        catch (Exception error)
        {
            Debug.LogError("[UndergroundExchangeAssistant] " + error);
            Notify("投放失败：" + error.Message);
            CloseConfirmation();
            Refresh();
        }
    }

    private void ConfirmBay(string id, int lostItems)
    {
        if (confirmation != null && confirmation.IsAlive) return;
        var template = new GameObject("ExchangeConfirmationTemplate", typeof(RectTransform), typeof(CustomUIWindowFrame));
        template.SetActive(false);
        confirmation = CustomUIManager.Instance.CreateWindow("underground_exchange_confirmation", null, "overlay", template)
            .SetSize(620, 270).Center().SetDraggable(false).SetCloseOnEscape()
            .OnClose(() => confirmation = null).GetWindow();
        UnityEngine.Object.Destroy(template);
        var root = confirmation.Rect;
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(.5f, .5f);
        var canvas = root.GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>().rect;
        float scale = Mathf.Min(1, (canvas.width - 32) / 620, (canvas.height - 32) / 270);
        root.localScale = Vector3.one * scale;
        foreach (var ui in root.GetComponents<StoreUI>()) ui.zoomable = false;
        var blocker = TabletUI.NewFlat(root, "ModalBlocker", 0, 0, canvas.width / scale, canvas.height / scale,
            new Color(0, 0, 0, .35f), true).rectTransform;
        blocker.anchorMin = blocker.anchorMax = blocker.pivot = new Vector2(.5f, .5f);
        blocker.anchoredPosition = Vector2.zero;
        TabletUI.NewFlat(root, "Paper", 0, 0, 620, 270, ExchangeWindow.PaperColor, true);
        TabletUI.NewText(skin, root, "Warning", 28, 24, 564, 150, FontLoader.FontRole.Noto_Sans, 23, ExchangeWindow.InkColor,
            TextAlignmentOptions.MidlineLeft, true).text =
            $"走私者暗格内还有 {lostItems} 件物品。\n交换暗格会使这些物品丢失。\n请先取出物品，或确认继续投放。";
        TabletUI.SmallButton(skin, root, 48, 198, 230, 44, "取消", CloseConfirmation);
        TabletUI.SmallButton(skin, root, 342, 198, 230, 44, "确认继续", () => Exchange(id, true));
        confirmation.Show();
    }

    private static void Notify(string message) => StoreUIManager.Instance.Notify(message, "default");

    private void CloseWindow()
    {
        CloseConfirmation();
        window.Close();
        tips.Clear();
        lastSignature = 0;
    }

    private void CloseConfirmation()
    {
        if (confirmation != null && confirmation.IsAlive) confirmation.Close();
        confirmation = null;
    }

    private void ResetSession()
    {
        CloseWindow();
        if (entrance != null) { entrance.gameObject.SetActive(false); UnityEngine.Object.Destroy(entrance.gameObject); }
        if (hoverVisual != null) { hoverVisual.SetActive(false); UnityEngine.Object.Destroy(hoverVisual); }
        entrance = null; hoverVisual = null; store = null; trashcan = null;
    }

    private void OnDestroy()
    {
        ResetSession();
        if (paper != null) UnityEngine.Object.Destroy(paper);
        if (paperTexture != null) UnityEngine.Object.Destroy(paperTexture);
        if (outline != null) UnityEngine.Object.Destroy(outline);
        if (outlineTexture != null) UnityEngine.Object.Destroy(outlineTexture);
    }
}
