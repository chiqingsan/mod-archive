using AquaSense;
using Il2Cpp;
using UnityEngine;
using System.Reflection;

internal static class Program
{
    static int passed;
    const int CompositionBarCells = 40; // track width in measured coordinates
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    static int DisplayWidth(string text)
    {
        var width = 0;
        foreach (var value in text)
            width += value >= '\u1100' && (
                value <= '\u115F' ||
                (value >= '\u2E80' && value <= '\uA4CF' && value != '\u303F') ||
                (value >= '\uAC00' && value <= '\uD7A3') ||
                (value >= '\uF900' && value <= '\uFAFF') ||
                (value >= '\uFE30' && value <= '\uFE4F') ||
                (value >= '\uFF00' && value <= '\uFF60') ||
                (value >= '\uFFE0' && value <= '\uFFE6')) ? 2 : 1;
        return width;
    }
    static int GuessTextWidth(string text) => text.Sum(value => DisplayWidth(value.ToString()) * 8);
    static List<string> CompositionRows(RichTextBuilder builder)
    {
        var joined = string.Concat(builder.Slices.Select(slice => slice.text));
        return joined.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(row => row.Contains(" ml") || row.Contains(" uL"))
            .ToList();
    }
    // Supplies the text-layout boundary only; production creates/updates the Images.
    static TMPro.TextMeshProUGUI RenderRows(params string[] rows)
    {
        var text = new TMPro.TextMeshProUGUI { text = string.Join("\n", rows) };
        text.gameObject.layer = 5;
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        text.textInfo.linkCount = rows.Length;
        text.textInfo.linkInfo = new TMPro.LinkInfo[rows.Length];
        text.textInfo.characterInfo = new TMPro.CharacterInfo[rows.Length];
        for (var i = 0; i < rows.Length; i++) {
            var id = System.Text.RegularExpressions.Regex.Match(rows[i], "<link=\"([^\"]+)\">").Groups[1].Value;
            var position = System.Text.RegularExpressions.Regex.Match(rows[i], @"<pos=([\d.]+)px>").Groups[1].Value;
            text.textInfo.linkInfo[i] = new TMPro.LinkInfo { id = id, linkTextfirstCharacterIndex = i };
            text.textInfo.characterInfo[i] = new TMPro.CharacterInfo {
                origin = float.Parse(position, System.Globalization.CultureInfo.InvariantCulture),
                ascender = 9 - i * 14, descender = -3 - i * 14
            };
        }
        Hooks.AfterTooltipTextMesh(text);
        Check(text.transform.children.Count == 0, "Images must not be created inside the Canvas text mesh rebuild");
        text.GetComponent<TooltipCompositionBars>()?.RenderPending();
        return text;
    }
    static void Redraw(TMPro.TextMeshProUGUI text)
    {
        Hooks.AfterTooltipTextMesh(text);
        text.GetComponent<TooltipCompositionBars>()?.Tick();
    }
    static List<(double Left, double Width)> BarRectangles(string row)
    {
        var text = RenderRows(row);
        var track = (RectTransform)text.transform.children.Single();
        var fill = (RectTransform)track.children.Single();
        return new() { (track.anchoredPosition.x, track.sizeDelta.x),
            (track.anchoredPosition.x, track.sizeDelta.x * fill.anchorMax.x) };
    }
    static double TrackWidth(string row) => BarRectangles(row).First().Width;
    static double FilledWidth(string row) => BarRectangles(row).Last().Width;
    static double RowWidth(string row, Func<string, int> measure)
    {
        double width = 0, size = 1;
        foreach (System.Text.RegularExpressions.Match token in System.Text.RegularExpressions.Regex.Matches(row, @"<[^>]+>|[^<]+"))
        {
            var value = token.Value;
            if (value.StartsWith("<pos=")) width = double.Parse(value[5..^3], System.Globalization.CultureInfo.InvariantCulture);
            else if (value.StartsWith("<space=")) width += double.Parse(value[7..^3], System.Globalization.CultureInfo.InvariantCulture);
            else if (value == "<size=50%>") size = 0.5;
            else if (value == "</size>") size = 1;
            else if (value.StartsWith("<sprite")) throw new Exception("Tooltip must not depend on sprite resources");
            else if (!value.StartsWith("<")) width += measure(value) * size;
        }
        return width;
    }
    static void Test(string name, Action test) { Reset(); test(); passed++; Console.WriteLine("PASS " + name); }
    static void Reset()
    {
        Hooks.ResetMaintenance(); PlayerStore.Instance = new(); EmporiumEntry.Instance = new();
        new Core().Init(new ModManifest()); TooltipCompositionBars.HideAll(); Time.unscaledTime = 0; Time.frameCount = 0;
        GraphUtils.Swaps = 0; GraphUtils.SwapFault = null; GraphUtils.PlaceFault = null; ItemMouseDragHandler.current = null;
    }
    static GameItem Item(string id, GameInventory inv, int durability = 10)
    {
        var x = new GameItem { identifier = id, Durability = durability }; x.state.Set("IS_OWNED_TAG");
        if (id.Contains("filter")) x.state.Set("WATER_FILTER");
        if (id is "water_filter" or "water_filter_adv" or "hydroreed_filter") x.state.Set("FILTER_SIZE_TAG", 22);
        inv.Put(x); return x;
    }
    static (GameItem jug, GameItem filter) Jug(int durability) => JugOf("water_filter", durability);
    static (GameItem jug, GameItem filter) JugOf(string filterIdentifier, int durability = 10)
    {
        var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
        var slot = new GameSlotInventory { ParentItem = jug }; jug.child = slot; jug.children.Add(slot); jug.FilterSlot = slot;
        return (jug, Item(filterIdentifier, slot, durability));
    }
    static (GameItem box, GameGridInventory inner) Box()
    {
        var box = Item("storage_box", EmporiumEntry.Instance.invWindow);
        var inner = new GameGridInventory { ParentItem = box }; box.child = inner; box.children.Add(inner); return (box, inner);
    }
    static (GameItem ras, GameSlotInventory slot) Ras()
    {
        var ras = Item("ras_empty", EmporiumEntry.Instance.invWindow);
        ras.state.Set("RAS_TAG");
        var slot = new GameSlotInventory { ParentItem = ras }; ras.child = slot; ras.children.Add(slot);
        return (ras, slot);
    }
    static void Frames(int count = 80) { for (var i = 0; i < count; i++) { Time.frameCount++; Hooks.UpdateMaintenance(true); } }
    static int Cached() { var f = typeof(Hooks).GetField("FilterCache", BindingFlags.Static | BindingFlags.NonPublic)!; return ((System.Collections.IEnumerable)f.GetValue(null)!).Cast<System.Collections.ICollection>().Sum(x => x.Count); }
    // 韧棉织物: WATER_FILTER tag + FILTER_SIZE_TAG 150 (AmenitiesItemDirectory),
    // so it can sit in a water-container slot but is coarser than a real filter.
    static GameItem ClothItem(GameInventory inv, int durability)
    {
        var cloth = Item("kotton_fabric", inv, durability);
        cloth.state.Set("WATER_FILTER"); cloth.state.Set("FILTER_SIZE_TAG", 150);
        return cloth;
    }
    static (GameItem jug, GameItem cloth) ClothJug(int durability)
    {
        var (jug, cloth) = JugOf("kotton_fabric", durability);
        cloth.state.Set("WATER_FILTER"); cloth.state.Set("FILTER_SIZE_TAG", 150);
        return (jug, cloth);
    }
    static void Main()
    {
        Test("highest remaining durability among fitting cached filters", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box();
            var low = Item("water_filter", inner, 20);
            var full = Item("water_filter", inner, 90); full.modifiedShape.minX = 2;
            var blocked = Item("water_filter", inner, 100); blocked.modifiedShape.minX = 4;
            jug.FilterSlot!.Accepts = item => item != blocked;
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot.childItem == full, "did not choose highest fitting durability");
        });
        Test("filter type priority precedes durability", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box();
            var normal = Item("water_filter", inner, 100);
            var advanced = Item("water_filter_adv", inner, 1); advanced.modifiedShape.minX = 2;
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == advanced, "durability overrode type priority");
        });
        Test("minimum filter durability floor defaults off and skips low-durability filters", () => {
            Check(Core.MinFilterDurabilityEnabled!.Value == false, "durability floor must default off");
            Check(Core.MinFilterDurability!.Value == 200, "durability floor default must be 200");
            var (jug, old) = Jug(10); var (_, inner) = Box(); var low = Item("water_filter", inner, 100);
            Frames(); old.Durability = 0;
            Core.MinFilterDurabilityEnabled!.Value = true;
            Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "candidate below the floor was swapped in");
            Core.MinFilterDurability!.Value = 50; Time.unscaledTime = 6; Frames();
            Check(jug.FilterSlot!.childItem == low && GraphUtils.Swaps == 1, "floor below candidate blocked the swap");
        });
        Test("composition bypass defaults off and responds to toggle", () => {
            Check(Core.ShowCompositionWithoutScanner!.Value == false, "bypass must default off");
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.Liquids = new() { new Liquid() };
            Liquid.LiquidDict = null;
            jug.state.Set("water", 1000000);
            try {
                var hidden = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(hidden, jug);
                Check(hidden.Slices.Count == 0, "composition visible by default");
                Core.ShowCompositionWithoutScanner.Value = true;
                var shown = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(shown, jug);
                Hooks.AfterWaterFeatureTooltip(shown, jug);
                Check(shown.Slices.Count == 3 && shown.Slices.Any(x => x.text.Contains("1000 ml")), "composition missing or duplicated");
                Core.ShowCompositionWithoutScanner.Value = false;
                var hiddenAgain = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(hiddenAgain, jug);
                Check(hiddenAgain.Slices.Count == 0, "toggle off ignored");
                Item("aquascan", EmporiumEntry.Instance.invWindow); Frames();
                var scanned = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(scanned, jug);
                Check(scanned.Slices.Count == 3, "scanner no longer enables composition");
            } finally { Liquid.Liquids.Clear(); }
        });
        Test("zero-percentage components stay visible in bar mode and hide in text mode", () => {
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() {
                new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" },
                new Liquid { identifier = "chem", displayName = "chem", currentPart = "chem", part = "chem" },
            };
            jug.state.Set("water", 1000000);
            jug.state.Set("chem", 1);
            Core.ShowCompositionWithoutScanner!.Value = true;
            try {
                Check(Core.HideZeroComponents!.Value, "hide-zero must default on");
                Check(Core.ShowCompositionBars!.Value, "bars must default on");
                Core.ShowCompositionBars.Value = true;
                var bars = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(bars, jug);
                Check(bars.Slices.Any(x => x.text.Contains("water:")), "main component missing in bar mode");
                Check(bars.Slices.Any(x => x.text.Contains("‹0.01%")), "tiny component missing in bar mode");
                Core.ShowCompositionBars.Value = false;
                var text = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(text, jug);
                Check(text.Slices.Any(x => x.text.Contains("water: ")), "main component missing in text mode");
                Check(!text.Slices.Any(x => x.text.Contains("chem:")), "zero component shown in text mode");
                Core.HideZeroComponents.Value = false;
                var textAll = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(textAll, jug);
                Check(textAll.Slices.Any(x => x.text.Contains("chem: ")), "zero component missing after disabling hide");
            } finally {
                Core.HideZeroComponents!.Value = true;
                Core.ShowCompositionBars!.Value = false;
                Core.ShowCompositionWithoutScanner!.Value = false;
                Liquid.Liquids.Clear();
            }
        });
        Test("composition rows align columns and draw a full track", () => {
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() {
                new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" },
                new Liquid { identifier = "chem", displayName = "chem", currentPart = "chem", part = "chem" },
            };
            jug.state.Set("water", 750);
            jug.state.Set("chem", 250);
            Core.ShowCompositionWithoutScanner!.Value = true;
            Core.ShowCompositionBars!.Value = true;
            try {
                var builder = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(builder, jug);
                var rows = CompositionRows(builder);
                Check(rows.Count == 2, "composition rows missing");
                Check(rows.Any(x => x.Contains("75.00%")), "percentage missing");
                Check(rows.Select(x => RowWidth(x, GuessTextWidth)).Distinct().Count() == 1, "columns not aligned");
                Check(rows.All(row => TrackWidth(row) == CompositionBarCells), "track width changed between rows");
                Check(FilledWidth(rows[0]) == 30 && FilledWidth(rows[1]) == 10, "75/25 shares rendered incorrectly: " + string.Join(" | ", rows));
                Check(rows.Select(row => System.Text.RegularExpressions.Regex.Match(row, @"<pos=[^>]+>").Value).Distinct().Count() == 1, "bar starts differ");
            } finally {
                Core.ShowCompositionWithoutScanner!.Value = false;
                Core.ShowCompositionBars!.Value = false;
                Liquid.Liquids.Clear();
            }
        });
        Test("tiny shares keep a thin sliver and a visible track", () => {
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() {
                new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" },
                new Liquid { identifier = "chem", displayName = "chem", currentPart = "chem", part = "chem" },
            };
            jug.state.Set("water", 999999);
            jug.state.Set("chem", 1);
            Core.ShowCompositionWithoutScanner!.Value = true;
            Core.ShowCompositionBars!.Value = true;
            try {
                var builder = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(builder, jug);
                Check(builder.Slices.Any(x => x.text.Contains("‹0.01%")), "tiny label missing");
                var rows = CompositionRows(builder);
                Check(FilledWidth(rows[0]) == CompositionBarCells - 0.5, "nearly full row lost its remainder");
                Check(FilledWidth(rows[1]) == 0.5, "trace must remain visible");
                Check(rows.All(row => TrackWidth(row) == CompositionBarCells), "trace changed track width");
            } finally {
                Core.ShowCompositionWithoutScanner!.Value = false;
                Core.ShowCompositionBars!.Value = false;
                Liquid.Liquids.Clear();
            }
        });
        Test("trace components show a non-zero amount and tiny label", () => {
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() {
                new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" },
                new Liquid { identifier = "chem", displayName = "chem", currentPart = "chem", part = "chem" },
                new Liquid { identifier = "mineral", displayName = "mineral", currentPart = "mineral", part = "mineral" },
            };
            jug.state.Set("water", 9106000);
            jug.state.Set("chem", 3);
            jug.state.Set("mineral", 150);
            Core.ShowCompositionWithoutScanner!.Value = true;
            try {
                Core.ShowCompositionBars.Value = true;
                var bars = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(bars, jug);
                Check(bars.Slices.Any(x => x.text.Contains("chem: 3 uL")), "trace amount not shown in microlitres");
                Check(bars.Slices.Any(x => x.text.Contains("‹0.01%")), "trace percentage missing in bar mode");
                Core.ShowCompositionBars.Value = false;
                Core.HideZeroComponents.Value = false;
                var text = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(text, jug);
                Check(text.Slices.Any(x => x.text.Contains("chem: 3 uL")), "trace amount not shown in microlitres in text mode");
                Check(text.Slices.Any(x => x.text.Contains("mineral: 0.15 ml")), "amounts above 0.1 ml must stay in millilitres");
                Check(text.Slices.Any(x => x.text.Contains("‹0.01%")), "trace percentage missing in text mode");
                Check(!text.Slices.Any(x => x.text.Contains(": 0 ml")), "zero volume rendered as 0 ml");
            } finally {
                Core.ShowCompositionWithoutScanner!.Value = false;
                Core.ShowCompositionBars!.Value = false;
                Core.HideZeroComponents!.Value = true;
                Liquid.Liquids.Clear();
            }
        });
        Test("long component names retain the same track width", () => {
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() {
                new Liquid { identifier = "contaminant", displayName = "physical contaminants", currentPart = "contaminant", part = "contaminant" },
                new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" },
            };
            jug.state.Set("contaminant", 500);
            jug.state.Set("water", 500);
            Core.ShowCompositionWithoutScanner!.Value = true;
            Core.ShowCompositionBars!.Value = true;
            try {
                var builder = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(builder, jug);
                var rows = CompositionRows(builder);
                Check(rows.Count == 2, "composition rows missing");
                Check(!rows.Any(x => x.Contains("…")), "a component name was truncated");
                Check(rows.Select(x => RowWidth(x, GuessTextWidth)).Distinct().Count() == 1, "columns not aligned");
                Check(rows.All(row => TrackWidth(row) == CompositionBarCells), "long text changed track width");
            } finally {
                Core.ShowCompositionWithoutScanner!.Value = false;
                Core.ShowCompositionBars!.Value = false;
                Liquid.Liquids.Clear();
            }
        });
        Test("tank rows keep the full text and share one left column", () => {
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() {
                new Liquid { identifier = "water", displayName = "水", currentPart = "water", part = "water" },
                new Liquid { identifier = "heavy", displayName = "重金属", currentPart = "heavy", part = "heavy" },
                new Liquid { identifier = "organic", displayName = "有机废物", currentPart = "organic", part = "organic" },
                new Liquid { identifier = "microbe", displayName = "微生物", currentPart = "microbe", part = "microbe" },
                new Liquid { identifier = "chemical", displayName = "化学污染物", currentPart = "chemical", part = "chemical" },
                new Liquid { identifier = "physical", displayName = "物理污染物", currentPart = "physical", part = "physical" },
                new Liquid { identifier = "mineral", displayName = "矿物质", currentPart = "mineral", part = "mineral" },
            };
            jug.state.Set("water", 9106000);
            jug.state.Set("heavy", 89400);
            jug.state.Set("organic", 178800);
            jug.state.Set("microbe", 223500);
            jug.state.Set("chemical", 178800);
            jug.state.Set("physical", 89400);
            jug.state.Set("mineral", 134100);
            Core.ShowCompositionWithoutScanner!.Value = true;
            try {
                Check(Core.ShowCompositionBars!.Value, "bars must default on");
                Core.ShowCompositionBars.Value = false;
                var textOnly = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(textOnly, jug);
                Check(textOnly.Slices.Any(x => x.text.Contains("水: 9106 ml")), "text mode lost the amount");
                Core.ShowCompositionBars.Value = true;
                var builder = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(builder, jug);
                var rows = CompositionRows(builder);
                string[] expected = { "水: 9106 ml", "重金属: 89.4 ml", "有机废物: 178.8 ml", "微生物: 223.5 ml", "化学污染物: 178.8 ml", "物理污染物: 89.4 ml", "矿物质: 134.1 ml" };
                Check(rows.Count == expected.Length, "rows missing");
                Check(!builder.Slices.Any(x => x.text.Contains("…")), "a component name was truncated");
                foreach (var prefix in expected)
                    Check(rows.Any(x => x.Contains(prefix)), "row lost its text: " + prefix);
                Check(rows.Select(x => RowWidth(x, GuessTextWidth)).Distinct().Count() == 1, "columns not aligned");
                Check(builder.Slices.Any(x => x.text.Contains("91.06%")), "percentage missing");
                Check(builder.Slices.Any(x => x.text.Contains("0.89%")), "small percentage missing");
                Check(FilledWidth(rows[5]) == 0.5, "small share must remain visible");
            } finally {
                Core.ShowCompositionWithoutScanner!.Value = false;
                Core.ShowCompositionBars!.Value = false;
                Liquid.Liquids.Clear();
            }
        });
        Test("columns align on the game text node measurements", () => {
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() {
                new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" },
                new Liquid { identifier = "chem", displayName = "chem", currentPart = "chem", part = "chem" },
            };
            jug.state.Set("water", 750);
            jug.state.Set("chem", 250);
            Core.ShowCompositionWithoutScanner!.Value = true;
            Core.ShowCompositionBars!.Value = true;
            try {
                // Simulate the game text node: the provider measures the plain text and
                // treats <space=NNpx> as NN pixels, like an orthographic TMP label.
                RenderHandler.current = new RenderHandler { tempRootObject = new GameObject("measure") };
                TMPro.TextMeshProUGUI.PreferredWidthProvider = text => (float)RowWidth(text, GuessTextWidth);
                var builder = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(builder, jug);
                var rows = CompositionRows(builder);
                Check(rows.Count == 2, "composition rows missing");
                Check(rows.Select(x => RowWidth(x, GuessTextWidth)).Distinct().Count() == 1, "columns not aligned with real measurements");
                Check(rows.All(x => x.Contains("<pos=")), "no absolute column positions emitted");
            } finally {
                TMPro.TextMeshProUGUI.PreferredWidthProvider = null;
                RenderHandler.current = null;
                Core.ShowCompositionWithoutScanner!.Value = false;
                Core.ShowCompositionBars!.Value = false;
                Liquid.Liquids.Clear();
            }
        });
        Test("native bar Images follow rows and clean up on reuse, toggle and mod disable", () => {
            Core.ShowCompositionBars!.Value = true;
            string Row(double fraction) => "water: 10 ml<pos=120px><link=\"AquaSenseCompositionBar:"
                + fraction.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":63ABA5\">\u00A0</link><pos=180px>50.00%";
            var text = RenderRows(Row(0.5), Row(0.0001));
            Check(text.transform.children.Count == 2, "native tracks were not created");
            var track = (RectTransform)text.transform.children[0];
            var fill = (RectTransform)track.children.Single();
            Check(track.sizeDelta.x == 40 && Math.Abs(track.sizeDelta.y - 4.8) < 0.001, "track size incorrect");
            Check(track.anchoredPosition.x == 120 && track.anchoredPosition.y == 3, "bar does not follow rendered text coordinates");
            Check(fill.anchorMax.x == 0.5f && fill.anchorMax.y == 1, "50% Image fill incorrect");
            var fillImage = fill.GetComponent<UnityEngine.UI.Image>()!;
            Check(fillImage.color.a == 1f && Math.Abs(fillImage.color.r - 99f / 255f) < 0.001, "fill is transparent or the colour is incorrect");
            Check(TrackWidth(Row(0)) == 40 && FilledWidth(Row(0)) == 0, "zero fill should retain only the track");
            Check(FilledWidth(Row(1)) == 40, "100% fill should cover the full track");
            Check(track.gameObject.layer == 5 && fill.gameObject.layer == 5, "bars are on the wrong camera layer");
            Check(!track.GetComponent<UnityEngine.UI.Image>()!.raycastTarget && !fill.GetComponent<UnityEngine.UI.Image>()!.raycastTarget, "bars intercept mouse input");
            Redraw(text);
            Check(text.transform.children.Count == 2, "redraw leaked UI objects");
            text.textInfo.linkCount = 1;
            text.textInfo.characterInfo[0].origin = 140;
            text.textInfo.characterInfo[0].ascender = 25;
            text.textInfo.characterInfo[0].descender = 5;
            Redraw(text);
            Check(track.anchoredPosition.x == 140 && track.anchoredPosition.y == 15 && track.sizeDelta.y == 8, "bar did not follow new layout/font metrics");
            Check(!text.transform.children[1].gameObject.activeSelf, "old row remains visible");
            Core.ShowCompositionBars.Value = false;
            Redraw(text);
            Check(!track.gameObject.activeSelf, "toggle left a stale bar");
            Core.ShowCompositionBars.Value = true;
            Redraw(text);
            Check(track.gameObject.activeSelf, "bar did not reappear");
            text.text = "ordinary item";
            Redraw(text);
            Check(!track.gameObject.activeSelf, "pooled text retained water bars");
            text.text = Row(0.5);
            Redraw(text);
            new Core().OnDisable();
            Check(!track.gameObject.activeSelf, "mod disable left an orphaned bar");
            Core.ShowCompositionBars.Value = false;
        });
        Test("tooltip mesh hook skips cleared text and hides outside the rebuild", () => {
            Core.ShowCompositionBars!.Value = true;
            string Row(double fraction) => "water: 10 ml<pos=120px><link=\"AquaSenseCompositionBar:"
                + fraction.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":63ABA5\">\u00A0</link><pos=180px>50.00%";
            var text = RenderRows(Row(0.5));
            var track = (RectTransform)text.transform.children[0];
            Check(track.gameObject.activeSelf, "row was not drawn");
            // TMP clears m_text while rebuilding; reading it blindly threw inside the mesh hook.
            text.text = null!;
            Hooks.AfterTooltipTextMesh(text);
            Check(track.gameObject.activeSelf, "hide must not run inside the mesh rebuild");
            text.GetComponent<TooltipCompositionBars>()!.Tick();
            Check(!track.gameObject.activeSelf, "deferred hide did not run");
            // A null argument must be ignored instead of crashing TMP's loop.
            Hooks.AfterTooltipTextMesh(null!);
            Core.ShowCompositionBars!.Value = false;
        });
        Test("screenshot tank proportions keep continuous tracks and aligned endpoints", () => {
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            string[] names = { "水", "重金属", "化学污染物", "物理污染物", "矿物质", "芬太" };
            int[] amounts = { 49671230, 68098, 251668, 1276, 7659, 69 };
            Liquid.Liquids = names.Select((name, i) => new Liquid {
                identifier = "part" + i, displayName = name, currentPart = "part" + i, part = "part" + i
            }).ToList();
            for (var i = 0; i < amounts.Length; i++) jug.state.Set("part" + i, amounts[i]);
            Core.ShowCompositionWithoutScanner!.Value = true;
            Core.ShowCompositionBars!.Value = true;
            try {
                var builder = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(builder, jug);
                var rows = CompositionRows(builder);
                Check(rows.Count == 6, "screenshot rows missing");
                string[] percentages = { "99.34%", "0.50%", "0.14%", "0.02%", "‹0.01%", "‹0.01%" };
                for (var i = 0; i < rows.Count; i++) {
                    Check(rows[i].Contains(percentages[i]), "wrong displayed percentage: " + rows[i]);
                    Check(TrackWidth(rows[i]) == 40, "track length depends on percentage");
                    Check(FilledWidth(rows[i]) == (i == 0 ? 39.5 : 0.5), "wrong screenshot fill");
                    var rectangles = BarRectangles(rows[i]);
                    Check(rectangles.Count == 2, "track/fill glyphs missing");
                    Check(Math.Abs(rectangles[0].Left - rectangles[1].Left) < 0.001, "fill starts outside the track");
                    var percentagePosition = double.Parse(System.Text.RegularExpressions.Regex.Matches(rows[i], @"<pos=([\d.]+)px>").Last().Groups[1].Value,
                        System.Globalization.CultureInfo.InvariantCulture);
                    Check(percentagePosition >= rectangles[0].Left + rectangles[0].Width + 8, "percentage overlaps the track");
                    Check(!rows[i].Contains("<sprite"), "unresolved sprite tags can widen the tooltip");
                }
                Check(rows.Select(row => RowWidth(row, GuessTextWidth)).Distinct().Count() == 1, "percentage endpoints differ");
                for (var i = 1; i < amounts.Length; i++) jug.state.Set("part" + i, 0);
                var pure = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(pure, jug);
                var pureRow = CompositionRows(pure).Single();
                Check(pureRow.Contains("100.00%") && FilledWidth(pureRow) == 40, "pure water should fill the entire track");
            } finally {
                Core.ShowCompositionWithoutScanner!.Value = false;
                Core.ShowCompositionBars!.Value = false;
                Liquid.Liquids.Clear();
            }
        });
        Test("filter durability without scanner, empty and filled containers", () => {
            var (jug, filter) = Jug(37);
            filter.MaxDurability = 400;
            Liquid.Liquids = new() { new Liquid() };
            try {
                foreach (var filled in new[] { false, true }) {
                    if (filled) jug.state.Set("water", 1000000);
                    var builder = new RichTextBuilder();
                    Hooks.AfterWaterContainerTooltip(builder, jug);
                    Hooks.AfterWaterFeatureTooltip(builder, jug);
                    Check(builder.Slices.Count == 1 && builder.Slices[0].text.Contains("37/400"), "missing or duplicate durability");
                    Check(!builder.Slices.Any(x => x.text.Contains("姘磋川璇︽儏")), "composition exposed without scanner");
                }
                Core.ShowFilterDurability!.Value = false;
                var disabled = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(disabled, jug);
                Check(disabled.Slices.Count == 0, "durability setting ignored");
            } finally { Liquid.Liquids.Clear(); }
        });
        Test("patches wait for gameplay and install once per enable", () => {
            HarmonyLib.Harmony.Patches = HarmonyLib.Harmony.Unpatches = 0;
            var core = new Core(); core.OnEnable(); core.OnEnable();
            Check(HarmonyLib.Harmony.Patches == 0, "patched during enable");
            var store = PlayerStore.Instance; PlayerStore.Instance = null;
            core.OnUpdate(); Check(HarmonyLib.Harmony.Patches == 0, "patched before store ready");
            PlayerStore.Instance = store; core.OnUpdate(); core.OnUpdate();
            Check(HarmonyLib.Harmony.Patches == 10, "duplicate or missing patches");
            core.OnDisable(); core.OnEnable(); core.OnUpdate();
            Check(HarmonyLib.Harmony.Patches == 20, "reenable failed"); core.OnDisable();
        });
        Test("patch failure rolls back and does not retry every frame", () => {
            HarmonyLib.Harmony.Patches = HarmonyLib.Harmony.Unpatches = 0;
            HarmonyLib.Harmony.Fail = true;
            try {
                var core = new Core(); core.OnEnable(); core.OnUpdate(); core.OnUpdate();
                Check(HarmonyLib.Harmony.Patches == 1 && HarmonyLib.Harmony.Unpatches == 1, "partial patches or repeated failure");
                core.OnDisable();
            } finally { HarmonyLib.Harmony.Fail = false; }
        });
        Test("exhausted on load, spare in nested box", () => {
            var (jug, _) = Jug(0); var (_, inner) = Box(); var spare = Item("water_filter", inner);
            Frames(); Check(jug.FilterSlot!.childItem == spare && GraphUtils.Swaps == 1, "nested spare not swapped");
            Check(PlayerStore.Instance!.RootCalls == 0, "synchronous full inventory query");
        });
        Test("filter event uses cached candidate", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); var spare = Item("water_filter", inner);
            Frames(); Check(Cached() == 1, "initial cache missing"); old.Durability=0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == spare && GraphUtils.Swaps == 1, "event swap failed");
        });
        Test("stale candidate then next cached candidate", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); var stale=Item("water_filter_adv", inner); var good=Item("water_filter", inner);
            Frames(); var elsewhere = new GameGridInventory { ParentItem = inner.ParentItem }; elsewhere.Put(stale);
            old.Durability=0; Hooks.AfterFilter(old); Frames(); Check(jug.FilterSlot!.childItem == good, "stale candidate blocked next");
        });
        Test("missing candidate is found after retry scan", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); Frames(); old.Durability=0; Hooks.AfterFilter(old); Frames();
            var spare=Item("water_filter",inner); Time.unscaledTime=6; Frames(180);
            Check(jug.FilterSlot!.childItem == spare, "new spare not found");
        });
        Test("de-duplicates exhausted events and does not steal unowned item", () => {
            var (jug, old) = Jug(10); var (_, inner)=Box(); var spare=Item("water_filter",inner); Frames();
            spare.state.Tags.Remove("IS_OWNED_TAG"); old.Durability=0; Hooks.AfterFilter(old); Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem==old && GraphUtils.Swaps==0, "unowned candidate used");
        });
        Test("multiple exhausted filters share cached spares", () => {
            var (first, a)=Jug(10); var (second, b)=Jug(10); var (_,inner)=Box();
            var spare1=Item("water_filter_adv",inner); var spare2=Item("water_filter",inner); spare2.modifiedShape.minX=2; Frames();
            a.Durability=0; b.Durability=0; Hooks.AfterFilter(a); Hooks.AfterFilter(b); Frames();
            Check(first.FilterSlot!.childItem==spare1 && second.FilterSlot!.childItem==spare2 && GraphUtils.Swaps==2, "incorrect queue or priority");
        });
        Test("dragging delays exchange without losing the pending filter", () => {
            var (jug,old)=Jug(10); var (_,inner)=Box(); var spare=Item("water_filter",inner); Frames();
            ItemMouseDragHandler.current=new ItemMouseDragHandler { IsDraggingItem=true }; old.Durability=0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem==old && GraphUtils.Swaps==0,"swapped while dragging");
            ItemMouseDragHandler.current=null; Time.unscaledTime=6; Frames();
            Check(jug.FilterSlot!.childItem==spare,"pending filter lost during drag");
        });
        Test("failed partial swap is recovered and blocks further swaps", () => {
            var (jug,old)=Jug(10); var (_,inner)=Box(); var spare=Item("water_filter",inner); Frames();
            GraphUtils.SwapFault=(a,b)=>{ GraphUtils.Detach(a); return false; };
            GraphUtils.PlaceFault=(_,_,_)=>false; old.Durability=0; Hooks.AfterFilter(old); Frames();
            Check(typeof(Hooks).GetField("_retryFilterRecovery",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!=null,"failed recovery not retained");
            GraphUtils.PlaceFault=null; GraphUtils.SwapFault=null; Time.unscaledTime=6; Frames(1);
            Check(jug.FilterSlot!.childItem==old && spare.parentInventory==inner,"original positions not recovered");
        });
        Test("hydroreed filter durability is shown in tooltips", () => {
            var (jug, hydroreed) = JugOf("hydroreed_filter", 37); hydroreed.MaxDurability = 200;
            var builder = new RichTextBuilder();
            Hooks.AfterWaterContainerTooltip(builder, jug);
            Check(builder.Slices.Count == 1 && builder.Slices[0].text.Contains("37/200"), "hydroreed durability missing");
        });
        Test("hydroreed filter is used when no better filter exists", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); var hydroreed = Item("hydroreed_filter", inner, 90);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == hydroreed && GraphUtils.Swaps == 1, "hydroreed filter not used as replacement");
        });
        Test("standard filter is preferred over hydroreed filter", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box();
            Item("hydroreed_filter", inner, 90);
            var standard = Item("water_filter", inner, 20); standard.modifiedShape.minX = 2;
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == standard, "hydroreed outranked a standard filter");
        });
        Test("depleted hydroreed filter is replaced", () => {
            var (jug, _) = JugOf("hydroreed_filter", 0); var (_, inner) = Box(); var spare = Item("water_filter", inner, 90);
            Frames();
            Check(jug.FilterSlot!.childItem == spare && GraphUtils.Swaps == 1, "depleted hydroreed not replaced");
        });
        Test("coarse chemistry cloth shows durability but is not installed", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box();
            var cloth = Item("kotton_fabric", inner, 100);
            cloth.state.Set("WATER_FILTER"); cloth.state.Set("FILTER_SIZE_TAG", 150);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "coarse cloth installed as replacement");
            var (jug2, coarse) = JugOf("kotton_fabric", 100); coarse.MaxDurability = 1000;
            coarse.state.Set("WATER_FILTER"); coarse.state.Set("FILTER_SIZE_TAG", 150);
            var builder = new RichTextBuilder();
            Hooks.AfterWaterContainerTooltip(builder, jug2);
            Check(builder.Slices.Count == 1 && builder.Slices[0].text.Contains("100/1000"), "coarse chemistry cloth durability missing");
        });
        Test("UV lamp is neither shown as filter nor installed", () => {
            var (jug, lamp) = JugOf("uv_filter", 50); lamp.MaxDurability = 400;
            var builder = new RichTextBuilder();
            Hooks.AfterWaterContainerTooltip(builder, jug);
            Check(builder.Slices.Count == 0, "UV lamp shown as filter durability");
            var (other, old) = Jug(10); var (_, inner) = Box();
            Item("uv_filter", inner, 50);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(other.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "UV lamp installed as replacement");
        });
        Test("filter slotted in a RAS machine is not used as a spare", () => {
            var (jug, old) = Jug(10); var (_, inner) = Box(); var (_, slot) = Ras();
            Item("water_filter_adv", slot, 100);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "RAS machine filter was taken as a spare");
        });
        Test("depleted filter slotted in a RAS machine is not replaced", () => {
            var (_, slot) = Ras(); var rasFilter = Item("water_filter", slot, 0);
            var (jug, old) = Jug(10); var (_, inner) = Box(); var spare = Item("water_filter", inner, 90);
            Frames(); Hooks.AfterFilter(rasFilter); Frames();
            Check(slot.childItem == rasFilter && jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "RAS machine filter was swapped");
            Check(spare.parentInventory == inner, "RAS machine filter consumed the spare");
        });
        Test("reload clears stale references", () => {
            var (jug, old)=Jug(10); var (_,inner)=Box(); var spare=Item("water_filter",inner); Frames();
            Hooks.AfterLoadGame(); inner.childItems.Remove(spare); spare.Parent=null; old.Durability=0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem==old && GraphUtils.Swaps==0, "stale cache used after load");
        });
        Test("scanner addition and removal refresh its cache", () => {
            Frames(); var scan=Item("aquascan",EmporiumEntry.Instance.invWindow); Hooks.AfterItemParentChanged(scan,true); Frames();
            var cache=typeof(Hooks).GetField("_scannerCache",BindingFlags.Static|BindingFlags.NonPublic)!;
            Check((bool)cache.GetValue(null)!,"scanner addition not found");
            EmporiumEntry.Instance.invWindow.childItems.Remove(scan); scan.Parent=null; Hooks.AfterItemParentChanged(scan,true); Frames();
            Check(!(bool)cache.GetValue(null)!,"scanner removal not found");
        });
        Test("chemical scanner is accepted as a water scanner", () => {
            Frames(); var scan=Item("chem_scanner",EmporiumEntry.Instance.invWindow); Hooks.AfterItemParentChanged(scan,true); Frames();
            var cache=typeof(Hooks).GetField("_scannerCache",BindingFlags.Static|BindingFlags.NonPublic)!;
            Check((bool)cache.GetValue(null)!,"chemical scanner addition not found");
            EmporiumEntry.Instance.invWindow.childItems.Remove(scan); scan.Parent=null; Hooks.AfterItemParentChanged(scan,true); Frames();
            Check(!(bool)cache.GetValue(null)!,"chemical scanner removal not found");
        });
        Test("scan limits per frame", () => {
            var root=EmporiumEntry.Instance.invWindow; for (int i=0;i<500;i++) Item("object_"+i,root);
            Hooks.UpdateMaintenance(true); Check(typeof(Hooks).GetField("_maintenance",BindingFlags.Static|BindingFlags.NonPublic)!.GetValue(null)!=null,"scan completed in one frame");
            Frames(100); Check(PlayerStore.Instance!.RootCalls==0,"unbounded inventory query");
        });
        Test("cloth replacement defaults on and can be disabled", () => {
            Check(Core.AutoReplaceCloth!.Value == true, "cloth replacement must default on");
            Core.AutoReplaceCloth.Value = false;
            var (jug, old) = ClothJug(10); var (_, inner) = Box(); var spare = ClothItem(inner, 900);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "cloth swapped while setting is off");
            Core.AutoReplaceCloth.Value = true;
        });
        Test("exhausted cloth is replaced by another cloth", () => {
            var (jug, old) = ClothJug(10); var (_, inner) = Box(); var spare = ClothItem(inner, 900);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == spare && GraphUtils.Swaps == 1, "exhausted cloth not swapped");
        });
        Test("cloth replacement never spends a fine filter", () => {
            var (jug, old) = ClothJug(10); var (_, inner) = Box();
            var fine = Item("water_filter", inner, 90);
            Frames(); old.Durability = 0; Hooks.AfterFilter(old); Frames();
            Check(jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0 && fine.parentInventory == inner, "fine filter spent on cloth");
        });
        Test("exhausted cloth on load is replaced by periodic scan", () => {
            var (jug, _) = ClothJug(0); var (_, inner) = Box(); var spare = ClothItem(inner, 900);
            Frames(); Check(jug.FilterSlot!.childItem == spare && GraphUtils.Swaps == 1, "periodic scan missed cloth");
        });
        Test("cloth in a RAS machine slot is not replaced", () => {
            var (_, slot) = Ras(); var rasCloth = ClothItem(slot, 0);
            var (jug, old) = ClothJug(10); var (_, inner) = Box(); var spare = ClothItem(inner, 900);
            Frames(); Hooks.AfterFilter(rasCloth); Frames();
            Check(slot.childItem == rasCloth && jug.FilterSlot!.childItem == old && GraphUtils.Swaps == 0, "RAS cloth was swapped");
            Check(spare.parentInventory == inner, "RAS cloth consumed the spare");
        });
        Test("settings panel shows the slider value live", () => {
            var config = ModConfig.For("test");
            var entry = Core.MinFilterDurability!;
            var builder = new CustomUIBuilder();
            Check(!ConfigPanel.BuildEntryPrefix(builder, config, entry), "aquasense ranged entry was not rendered");
            Check(builder.Labels.Any(l => l.Text == "200"), "value label must show the current value");
            Check(builder.Calls.Any(c => c.StartsWith("slider:")), "slider was not created");
            builder.SliderChanged!(300f);
            Check(builder.Labels.Any(l => l.Text == "300"), "value label must follow the slider");
            Check(ConfigPanel.BuildEntryPrefix(new CustomUIBuilder(), config, Core.AutoReplaceFilters!), "bool entry must keep vanilla rendering");
            Check(ConfigPanel.BuildEntryPrefix(new CustomUIBuilder(), ModConfig.For("other"), entry), "other mods must keep vanilla rendering");
        });
        Test("composition api slot follows the mod lifetime", () => {
            var core = new Core(); core.Init(new ModManifest()); core.OnEnable();
            try {
                var slot = AppDomain.CurrentDomain.GetData(CompositionApi.DataSlot)
                    as Func<GameItem, IReadOnlyList<(string Name, string Amount, string Percent, float Ratio, int ColorRgb)>>;
                Check(slot != null, "composition data slot missing while the mod is enabled");
                Check(AppDomain.CurrentDomain.GetData(CompositionApi.VersionSlot) is int version && version == CompositionApi.Version, "composition version slot missing");
                var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
                Liquid.LiquidDict = null;
                Liquid.Liquids = new() {
                    new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" },
                    new Liquid { identifier = "chem", displayName = "chem", currentPart = "chem", part = "chem" },
                };
                jug.state.Set("water", 750); jug.state.Set("chem", 250);
                Core.ShowCompositionWithoutScanner!.Value = true;
                try {
                    var rows = slot!(jug);
                    Check(rows != null && rows.Count == 2, "api rows missing");
                    Check(rows![0].Name == "water" && rows[1].Name == "chem", "api row names/order wrong");
                    Check(rows[0].Amount == "0.75 ml" && rows[1].Amount == "0.25 ml", "api amount text wrong");
                    Check(rows[0].Percent == "75.00%" && rows[1].Percent == "25.00%", "api percentage text wrong");
                    Check(Math.Abs(rows[0].Ratio - 0.75f) < 0.0001f && Math.Abs(rows[1].Ratio - 0.25f) < 0.0001f, "api ratio wrong");
                    Check(rows[0].ColorRgb == 0x63ABA5 && rows[1].ColorRgb == 0xA494BC, "api bar colour wrong");
                } finally {
                    Core.ShowCompositionWithoutScanner!.Value = false;
                    Liquid.Liquids.Clear();
                }
            } finally { core.OnDisable(); }
            Check(AppDomain.CurrentDomain.GetData(CompositionApi.DataSlot) == null, "composition data slot outlived the mod");
            Check(AppDomain.CurrentDomain.GetData(CompositionApi.VersionSlot) == null, "composition version slot outlived the mod");
        });
        Test("composition api follows the tooltip availability gate", () => {
            CompositionApi.Register();
            try {
                var slot = AppDomain.CurrentDomain.GetData(CompositionApi.DataSlot)
                    as Func<GameItem, IReadOnlyList<(string Name, string Amount, string Percent, float Ratio, int ColorRgb)>>;
                Check(slot != null, "composition data slot missing");
                var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
                Liquid.LiquidDict = null;
                Liquid.Liquids = new() {
                    new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" },
                    new Liquid { identifier = "chem", displayName = "chem", currentPart = "chem", part = "chem" },
                };
                jug.state.Set("water", 1000000); jug.state.Set("chem", 1);
                Check(slot!(jug) == null, "api ignored the missing scanner");
                Core.ShowCompositionWithoutScanner!.Value = true;
                Core.HideZeroComponents!.Value = true;
                Core.ShowCompositionBars!.Value = false;
                try {
                    var rows = slot(jug);
                    Check(rows != null && rows.Count == 2, "api dropped trace rows or display-toggle independence");
                    Check(rows![1].Percent == "‹0.01%" && rows[1].Ratio > 0f, "api trace row wrong");
                    var empty = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
                    Check(slot(empty) == null, "api described an empty container");
                } finally {
                    Core.ShowCompositionWithoutScanner!.Value = false;
                    Core.HideZeroComponents!.Value = true;
                    Core.ShowCompositionBars!.Value = true;
                    Liquid.Liquids.Clear();
                }
            } finally { CompositionApi.Unregister(); }
            Check(AppDomain.CurrentDomain.GetData(CompositionApi.DataSlot) == null, "api slot not cleared");
        });
        Test("water quality highlight tints the title tag and the labeler line", () => {
            Check(Core.HighlightWaterQuality!.Value, "quality highlight must default on");
            WaterHelper.WaterPurity = 9971;
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() { new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" } };
            jug.state.Set("water", 1000);
            Core.ShowCompositionWithoutScanner!.Value = true;
            var none = RenderHandler.ColorPalette.None;
            try {
                var builder = new RichTextBuilder();
                builder.Add("[label] ", (RenderHandler.ColorPalette)0xFFFFFF);
                builder.AddLine("water", false, none, false, false, false, false, none, none, none);
                Hooks.AfterWaterContainerTooltip(builder, jug);
                var tinted = builder.Slices.Where(x => x.text == "water\n").ToList();
                Check(tinted.Count == 2, "expected the labeler line and the title tag");
                Check(tinted.All(x => (int)x.color == 0x9FE6A0 && (int)x.backgroundColor == -1), "quality text tint not applied");
                var prefix = builder.Slices.Single(x => x.text.Contains("水质详情 (9971)"));
                Check((int)prefix.backgroundColor == -1, "title prefix must stay neutral");
                Core.HighlightWaterQuality!.Value = false;
                var plain = new RichTextBuilder();
                plain.Add("[label] ", (RenderHandler.ColorPalette)0xFFFFFF);
                plain.AddLine("water", false, none, false, false, false, false, none, none, none);
                Hooks.AfterWaterContainerTooltip(plain, jug);
                Check(plain.Slices.Any(x => x.text == "水质详情 (9971)  water\n"), "title line was split with the highlight off");
                Check(plain.Slices.Where(x => x.text == "water\n").All(x => (int)x.backgroundColor == -1), "labeler line tinted with the highlight off");
            } finally {
                Core.HighlightWaterQuality!.Value = true;
                Core.ShowCompositionWithoutScanner!.Value = false;
                WaterHelper.WaterPurity = 10000;
                Liquid.Liquids.Clear();
            }
        });
        Test("gutterflow and unknown water share the warning tint", () => {
            Check(Core.HighlightWaterQuality!.Value, "quality highlight must default on");
            WaterHelper.WaterPurity = 4000; // gutterflow tier
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() { new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" } };
            jug.state.Set("water", 1000);
            Core.ShowCompositionWithoutScanner!.Value = true;
            try {
                var builder = new RichTextBuilder();
                Hooks.AfterWaterContainerTooltip(builder, jug);
                var tinted = builder.Slices.Where(x => x.text == "water\n").ToList();
                Check(tinted.Count > 0 && tinted.All(x => (int)x.color == 0xC73E3A), "gutterflow must use the warning tint");
                WaterHelper.WaterPurity = 3000; // below every tier: unknown water
                var unknown = new RichTextBuilder();
                Hooks.AfterWaterContainerTooltip(unknown, jug);
                var unknownTinted = unknown.Slices.Where(x => x.text == "water\n").ToList();
                Check(unknownTinted.Count > 0 && unknownTinted.All(x => (int)x.color == 0xC73E3A), "unknown water must share the warning tint");
            } finally {
                Core.ShowCompositionWithoutScanner!.Value = false;
                WaterHelper.WaterPurity = 10000;
                Liquid.Liquids.Clear();
            }
        });
        Test("water composition toggle hides the whole section", () => {
            Check(Core.ShowWaterComposition!.Value, "composition section must default on");
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() { new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" } };
            jug.state.Set("water", 1000);
            Core.ShowCompositionWithoutScanner!.Value = true;
            try {
                var shown = new RichTextBuilder();
                Hooks.AfterWaterContainerTooltip(shown, jug);
                Check(shown.Slices.Any(x => x.text.Contains("水质详情")), "section missing with the toggle on");
                Check(CompositionRows(shown).Count == 1, "composition rows missing with the toggle on");
                Core.ShowWaterComposition!.Value = false;
                var hidden = new RichTextBuilder();
                Hooks.AfterWaterContainerTooltip(hidden, jug);
                Check(!hidden.Slices.Any(x => x.text.Contains("水质详情")), "section shown with the toggle off");
                Check(CompositionRows(hidden).Count == 0, "composition rows shown with the toggle off");
                Check(Hooks.GetCompositionApiRows(jug) == null, "api exposed rows with the toggle off");
            } finally {
                Core.ShowWaterComposition!.Value = true;
                Core.ShowCompositionWithoutScanner!.Value = false;
                Liquid.Liquids.Clear();
            }
        });
        Test("composition rows sort by share and follow the toggle", () => {
            Check(Core.SortCompositionByAmount!.Value, "composition sort must default on");
            var jug = Item("water_pitcher", EmporiumEntry.Instance.invWindow);
            Liquid.LiquidDict = null;
            Liquid.Liquids = new() {
                new Liquid { identifier = "chem", displayName = "chem", currentPart = "chem", part = "chem" },
                new Liquid { identifier = "water", displayName = "water", currentPart = "water", part = "water" },
                new Liquid { identifier = "mineral", displayName = "mineral", currentPart = "mineral", part = "mineral" },
            };
            jug.state.Set("chem", 200);
            jug.state.Set("water", 700);
            jug.state.Set("mineral", 100);
            Core.ShowCompositionWithoutScanner!.Value = true;
            Core.ShowCompositionBars!.Value = true;
            try {
                var builder = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(builder, jug);
                var rows = CompositionRows(builder);
                Check(rows.Count == 3, "composition rows missing");
                Check(rows[0].Contains("water:") && rows[1].Contains("chem:") && rows[2].Contains("mineral:"), "rows not sorted by share: " + string.Join(" | ", rows));
                Check(FilledWidth(rows[0]) == 28 && FilledWidth(rows[1]) == 8 && FilledWidth(rows[2]) == 4, "sorted shares rendered wrong: " + string.Join(" | ", rows));
                Core.SortCompositionByAmount!.Value = false;
                var unsorted = new RichTextBuilder(); Hooks.AfterWaterContainerTooltip(unsorted, jug);
                var gameOrder = CompositionRows(unsorted);
                Check(gameOrder[0].Contains("chem:") && gameOrder[1].Contains("water:") && gameOrder[2].Contains("mineral:"), "sort toggle ignored");
            } finally {
                Core.SortCompositionByAmount!.Value = true;
                Core.ShowCompositionWithoutScanner!.Value = false;
                Core.ShowCompositionBars!.Value = false;
                Liquid.Liquids.Clear();
            }
        });
        Console.WriteLine($"{passed} scenarios passed");
    }
}


