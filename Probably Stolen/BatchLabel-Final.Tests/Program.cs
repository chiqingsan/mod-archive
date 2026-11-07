using BatchLabelFinal;

internal static class Program
{
    private const string CigaretteCategory = "CATEGORY_GENUINE_CIGARETTE";
    private const string StampCategory = "CATEGORY_GENUINE_STAMP";
    private const string WaterCategory = "CATEGORY_WATER_PURITY";
    private const string WineCategory = "CATEGORY_WINE_QUALITY";
    private const string StuckCategory = "CATEGORY_MODULE_STUCK";
    private const string ChemicalCategory = "CATEGORY_CHEMICAL_PURITY";

    private static int _passed;
    private static int _failed;

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        SetupConditionRegistry();

        Run("起点没贴过标签（当前显示=真实条件）→ 按显示条件复制：正品起点把正品标签贴给赝品目标", () =>
        {
            ItemCondition genuine = Condition("realGenuine");
            ItemCondition counterfeit = Condition("realCounterfeit");
            GameItemElement source = Item("cigarette_red", Feature(CigaretteCategory, genuine, genuine));
            GameItemElement target = Item("cigarette_red", Feature(CigaretteCategory, counterfeit, counterfeit));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            Check(labels.Count == 1, "未贴过标签的起点也应按其显示条件收集到标签");

            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, target));
            Check(changed == 1, "赝品目标应被打上正品标签（实际 " + changed + "）");
            Check(target.itemFeatures[0].realCondition!.identifier == "realCounterfeit", "目标的真实条件不应被改动");
            Check(target.itemFeatures[0].fakeCondition!.identifier == "realGenuine", "目标假条件未变成起点当前显示的条件");
            Check(!target.itemFeatures[0].IsExposed(), "贴标签后应处于未揭穿状态");
        });

        Run("赝品起点 → 正品目标贴上赝品标签（两个方向对称）", () =>
        {
            ItemCondition genuine = Condition("realGenuine");
            ItemCondition counterfeit = Condition("realCounterfeit");
            GameItemElement source = Item("cigarette_red", Feature(CigaretteCategory, counterfeit, counterfeit));
            GameItemElement target = Item("cigarette_red", Feature(CigaretteCategory, genuine, genuine));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            Check(labels[0].Condition.identifier == "realCounterfeit", "赝品起点应复制赝品标签");

            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, target));
            Check(changed == 1, "正品目标应被打上赝品标签（实际 " + changed + "）");
            Check(target.itemFeatures[0].fakeCondition!.identifier == "realCounterfeit", "目标假条件未变成起点的显示条件");
            Check(target.itemFeatures[0].realCondition!.identifier == "realGenuine", "目标的真实条件不应被改动");
        });

        Run("起点已被揭穿 → 复制它当前显示的真实条件", () =>
        {
            ItemCondition genuine = Condition("realGenuine");
            ItemCondition counterfeit = Condition("realCounterfeit");
            GameItemElement source = Item("cigarette_red",
                Feature(CigaretteCategory, genuine, counterfeit, exposed: true));
            GameItemElement target = Item("cigarette_red", Feature(CigaretteCategory, genuine, genuine));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            Check(labels.Count == 1, "已揭穿的起点也应有可复制的显示条件");
            Check(labels[0].Condition.identifier == "realCounterfeit", "已揭穿时显示的是真实条件，应复制它");

            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, target));
            Check(changed == 1, "应改动 1 个目标");
            Check(target.itemFeatures[0].fakeCondition!.identifier == "realCounterfeit", "目标未按显示条件贴上标签");
            Check(!target.itemFeatures[0].IsExposed(), "贴标签后应处于未揭穿状态");
        });

        Run("边界1b：起点只有不可编辑/被排除的标签分类 → 收集为空", () =>
        {
            ItemCondition a = Condition("a");
            ItemCondition b = Condition("b");
            GameItemElement stuck = Item("module", Feature(StuckCategory, a, b));
            GameItemElement wine = Item("wine", Feature(WineCategory, a, b));
            ItemFeature eventFeature = Feature(CigaretteCategory, a, b);
            eventFeature.featureType = FeatureType.Event;
            GameItemElement temporary = Item("cigarette_red", eventFeature);

            Check(LabelBatch.Collect(stuck).Count == 0, "未注册分类不应参与标签复制");
            Check(LabelBatch.Collect(wine).Count == 0, "被排除的分类不应参与标签复制");
            Check(LabelBatch.Collect(temporary).Count == 0, "非 Normale 的 feature 不应参与标签复制");
        });

        Run("正常复制：只改与起点同 identifier 的物品，每个目标只 SetExposed 一次", () =>
        {
            ItemCondition genuine = Condition("genuine");
            ItemCondition counterfeit = Condition("counterfeit");
            GameItemElement source = Item("cigarette_red", Feature(CigaretteCategory, genuine, counterfeit));
            GameItemElement target1 = Item("cigarette_red", Feature(CigaretteCategory, counterfeit, counterfeit));
            GameItemElement target2 = Item("cigarette_red", Feature(CigaretteCategory, counterfeit, counterfeit));
            GameItemElement other = Item("cigarette_blue", Feature(CigaretteCategory, counterfeit, counterfeit));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            Check(labels.Count == 1, "应收集到 1 个标签");

            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, target1, target2, other));
            Check(changed == 2, "应只处理 2 个同类型物品（实际 " + changed + "）");
            Check(target1.itemFeatures[0].fakeCondition!.identifier == "genuine", "target1 未贴上新标签");
            Check(target2.itemFeatures[0].fakeCondition!.identifier == "genuine", "target2 未贴上新标签");
            Check(target1.itemFeatures[0].ExposeCount == 1, "target1 SetExposed 次数应为 1");
            Check(target2.itemFeatures[0].ExposeCount == 1, "target2 SetExposed 次数应为 1");
            Check(!target1.itemFeatures[0].IsExposed(), "贴标签后应处于未揭穿状态");
            Check(other.itemFeatures[0].fakeCondition!.identifier == "counterfeit", "其它类型物品不应被改动");
            Check(other.itemFeatures[0].ExposeCount == 0, "其它类型物品不应触发 SetExposed");
        });

        Run("边界3：框内没有任何起点类型的物品 → 0 改动", () =>
        {
            ItemCondition genuine = Condition("genuine");
            ItemCondition counterfeit = Condition("counterfeit");
            GameItemElement source = Item("cigarette_red", Feature(CigaretteCategory, genuine, counterfeit));
            GameItemElement other1 = Item("cigarette_blue", Feature(CigaretteCategory, counterfeit, counterfeit));
            GameItemElement other2 = Item("water_pitcher", Feature(WaterCategory, Condition("pure"), Condition("pure")));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, other1, other2));
            Check(changed == 0, "没有同类型物品时不应改动任何物品");
            Check(other1.itemFeatures[0].fakeCondition!.identifier == "counterfeit", "其它类型被意外改写");
            Check(other2.itemFeatures[0].fakeCondition!.identifier == "pure", "其它类型被意外改写");
        });

        Run("边界3b：框内只有起点自己 → 0 改动", () =>
        {
            ItemCondition genuine = Condition("genuine");
            ItemCondition counterfeit = Condition("counterfeit");
            GameItemElement source = Item("cigarette_red", Feature(CigaretteCategory, genuine, counterfeit));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source));
            Check(changed == 0, "只有起点时不应改动");
            Check(source.itemFeatures[0].ExposeCount == 0, "起点不应被 SetExposed");
        });

        Run("起点不在框内（被原版过滤）时，仍对框内同类型物品生效", () =>
        {
            ItemCondition genuine = Condition("genuine");
            ItemCondition counterfeit = Condition("counterfeit");
            GameItemElement source = Item("cigarette_red", Feature(CigaretteCategory, genuine, counterfeit));
            GameItemElement target = Item("cigarette_red", Feature(CigaretteCategory, counterfeit, counterfeit));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(target));
            Check(changed == 1, "起点不在框内时仍应处理同类型目标");
            Check(target.itemFeatures[0].fakeCondition!.identifier == "genuine", "目标未贴上新标签");
        });

        Run("水质标签：额外复制 customIntValue1（标签机 UI 也是这样写）", () =>
        {
            ItemCondition pure = Condition("waterPure");
            ItemCondition baseWater = Condition("waterBase");
            GameItemElement source = Item("water_pitcher",
                Feature(WaterCategory, baseWater, pure, customIntValue1: 9800));
            GameItemElement target = Item("water_pitcher",
                Feature(WaterCategory, pure, pure, customIntValue1: 9990));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            Check(labels.Count == 1, "应收集到 1 个水质标签");
            Check(labels[0].CopyCustomIntValue, "水质标签应标记为需要复制 customIntValue1");

            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, target));
            Check(changed == 1, "应改动 1 个水质容器");
            Check(target.itemFeatures[0].fakeCondition!.identifier == "waterBase", "水质假标签未复制");
            Check(target.itemFeatures[0].customIntValue1 == 9800,
                "水质标签的 customIntValue1 未复制（实际 " + target.itemFeatures[0].customIntValue1 + "）");
            Check(!target.itemFeatures[0].IsExposed(), "水质标签应处于未揭穿状态");
        });

        Run("非水质标签：不写 customIntValue1（该字段只有水质标签读取）", () =>
        {
            ItemCondition genuine = Condition("genuine");
            ItemCondition counterfeit = Condition("counterfeit");
            GameItemElement source = Item("cigarette_red",
                Feature(CigaretteCategory, genuine, counterfeit, customIntValue1: 9800));
            GameItemElement target = Item("cigarette_red", Feature(CigaretteCategory, counterfeit, counterfeit));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            Check(!labels[0].CopyCustomIntValue, "非水质标签不应标记为复制 customIntValue1");

            LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, target));
            Check(target.itemFeatures[0].customIntValue1 == 0, "非水质标签不应改写 customIntValue1");
        });

        Run("目标已经是同样的假标签且未被揭穿 → 不重复 SetExposed、不计数", () =>
        {
            ItemCondition genuine = Condition("genuine");
            ItemCondition counterfeit = Condition("counterfeit");
            GameItemElement source = Item("cigarette_red", Feature(CigaretteCategory, genuine, counterfeit));
            GameItemElement target = Item("cigarette_red",
                Feature(CigaretteCategory, Condition("genuine"), counterfeit));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, target));
            Check(changed == 0, "已经是同样标签时不应算作改动");
            Check(target.itemFeatures[0].ExposeCount == 0, "已经贴好标签时不应再触发 SetExposed（会重复执行改名/改价动作）");
        });

        Run("被揭穿的目标会被重新贴上假标签", () =>
        {
            ItemCondition genuine = Condition("genuine");
            ItemCondition counterfeit = Condition("counterfeit");
            GameItemElement source = Item("cigarette_red", Feature(CigaretteCategory, genuine, counterfeit));
            GameItemElement target = Item("cigarette_red",
                Feature(CigaretteCategory, counterfeit, counterfeit, exposed: true));
            int before = target.itemFeatures[0].ExposeCount;

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, target));
            Check(changed == 1, "被揭穿的目标应被重新贴标签");
            Check(target.itemFeatures[0].fakeCondition!.identifier == "genuine", "被揭穿的目标未贴上新标签");
            Check(!target.itemFeatures[0].IsExposed(), "重新贴标签后应处于未揭穿状态");
            Check(target.itemFeatures[0].ExposeCount == before + 1, "应只触发一次 SetExposed");
        });

        Run("起点有多个标签分类：逐个分类复制，目标缺分类时跳过该分类", () =>
        {
            ItemCondition genuine = Condition("genuine");
            ItemCondition counterfeit = Condition("counterfeit");
            ItemCondition genuineStamp = Condition("genuineStamp");
            ItemCondition counterfeitStamp = Condition("counterfeitStamp");
            GameItemElement source = Item("scam_bundle",
                Feature(CigaretteCategory, genuine, counterfeit),
                Feature(StampCategory, genuineStamp, counterfeitStamp));
            GameItemElement both = Item("scam_bundle",
                Feature(CigaretteCategory, counterfeit, counterfeit),
                Feature(StampCategory, counterfeitStamp, counterfeitStamp));
            GameItemElement onlyCigarette = Item("scam_bundle",
                Feature(CigaretteCategory, counterfeit, counterfeit));

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            Check(labels.Count == 2, "应收集到 2 个分类的标签");

            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, both, onlyCigarette));
            Check(changed == 2, "两个目标都应被处理");
            Check(both.itemFeatures[0].fakeCondition!.identifier == "genuine", "分类 1 未复制");
            Check(both.itemFeatures[1].fakeCondition!.identifier == "genuineStamp", "分类 2 未复制");
            Check(onlyCigarette.itemFeatures[0].fakeCondition!.identifier == "genuine", "单分类目标未复制");
            Check(onlyCigarette.itemFeatures.Count == 1, "缺少的分类不应凭空创建");
        });

        Run("非自有物品：不作为标签来源、也不会被贴标签（与原版标签机目标条件一致）", () =>
        {
            ItemCondition genuine = Condition("genuine");
            ItemCondition counterfeit = Condition("counterfeit");
            GameItemElement source = Item("cigarette_red", Feature(CigaretteCategory, genuine, counterfeit));
            GameItemElement nonOwnedSource = NonOwnedItem("cigarette_red",
                Feature(CigaretteCategory, genuine, counterfeit));
            GameItemElement target = Item("cigarette_red", Feature(CigaretteCategory, counterfeit, counterfeit));
            GameItemElement nonOwnedTarget = NonOwnedItem("cigarette_red",
                Feature(CigaretteCategory, counterfeit, counterfeit));

            Check(LabelBatch.Collect(nonOwnedSource).Count == 0, "非自有起点不应收集到标签");

            List<LabelBatch.LabelEntry> labels = LabelBatch.Collect(source);
            int changed = LabelBatch.ApplyToMatchingItems(source, labels, Selection(source, target, nonOwnedTarget));
            Check(changed == 1, "只有自有目标应被贴标签（实际 " + changed + "）");
            Check(target.itemFeatures[0].fakeCondition!.identifier == "genuine", "自有目标未贴上新标签");
            Check(nonOwnedTarget.itemFeatures[0].ExposeCount == 0, "非自有目标不应被写入");
            Check(nonOwnedTarget.itemFeatures[0].fakeCondition!.identifier == "counterfeit",
                "非自有目标的标签被意外改写");
        });

        Run("标签记忆：记住「咖啡片→实验室纯级」后，同类型物品能解析出这条标签", () =>
        {
            LabelMemory.Clear();
            ItemCondition labPure = Condition("lab_pure");
            GameItemElement labeled = Item("coffee_tablet",
                Feature(ChemicalCategory, labPure, Condition("junk_grade")));

            LabelMemory.Remember(labeled.identifier, LabelBatch.Collect(labeled));
            Check(LabelMemory.IsRemembered("coffee_tablet"), "应记住该物品类型");

            Check(LabelMemory.TryResolve("coffee_tablet", out List<LabelBatch.LabelEntry> labels), "应能解析出记忆的标签");
            Check(labels.Count == 1, "应只有 1 条记忆标签（实际 " + labels.Count + "）");
            Check(labels[0].Category == ChemicalCategory, "记忆的分类不对");
            Check(labels[0].Condition.identifier == "lab_pure", "记忆的条件应为实验室纯级");
            Check(!LabelMemory.TryResolve("cigarette_red", out _), "没打过的物品类型不应有记忆");
            LabelMemory.Clear();
        });

        Run("标签记忆：同分类后打覆盖先打、不同分类累积、不同类型互不影响", () =>
        {
            LabelMemory.Clear();
            Remember("coffee_tablet", ChemicalCategory, "junk_grade");
            Remember("coffee_tablet", ChemicalCategory, "lab_pure");
            Remember("coffee_tablet", StampCategory, "genuineStamp");
            Remember("coffee_tablet", WaterCategory, "waterBase", customIntValue1: 9800, copyCustomIntValue: true);
            Remember("scam_bundle", StampCategory, "counterfeitStamp");

            Check(LabelMemory.TryResolve("coffee_tablet", out List<LabelBatch.LabelEntry> labels), "应能解析出记忆");
            Check(labels.Count == 3, "同类不同分类应累积成 3 条（实际 " + labels.Count + "）");
            Check(Find(labels, ChemicalCategory)?.Condition.identifier == "lab_pure", "同分类后打上的标签应覆盖先前的");
            Check(Find(labels, StampCategory)?.Condition.identifier == "genuineStamp", "第二分类未被记住");
            LabelBatch.LabelEntry? water = Find(labels, WaterCategory);
            Check(water?.CustomIntValue == 9800 && water.Value.CopyCustomIntValue, "水质标签的自定义值与复制标记未被记住");

            Check(LabelMemory.TryResolve("scam_bundle", out List<LabelBatch.LabelEntry> other), "另一类型应各自独立");
            Check(other.Count == 1 && other[0].Condition.identifier == "counterfeitStamp", "不同物品类型的记忆互相污染了");
            LabelMemory.Clear();
        });

        Run("标签记忆：序列化往返一致（含分隔符转义、水质自定义值、排序稳定）", () =>
        {
            LabelMemory.Clear();
            Remember("coffee_tablet", ChemicalCategory, "lab_pure");
            Remember("coffee_tablet", WaterCategory, "waterBase", customIntValue1: 9800, copyCustomIntValue: true);
            Remember("weird|id=x;y:z", StampCategory, "genuineStamp");
            Remember("cigarette_red", StampCategory, "counterfeitStamp");

            string blob = LabelMemory.Serialize();
            LabelMemory.Clear();
            Check(LabelMemory.RememberedItemCount == 0, "Clear 之后应为空");

            LabelMemory.Load(blob);
            Check(LabelMemory.Serialize() == blob, "反序列化后再序列化应与原文一致");
            Check(LabelMemory.TryResolve("weird|id=x;y:z", out List<LabelBatch.LabelEntry> escaped), "含分隔符的物品类型应能还原");
            Check(escaped.Count == 1 && escaped[0].Condition.identifier == "genuineStamp", "转义后的数据未正确还原");
            Check(LabelMemory.TryResolve("coffee_tablet", out _), "普通物品类型应能还原");
            LabelMemory.Clear();
        });

        Run("标签记忆：空文本/损坏文本不崩溃、也不留下垃圾记忆", () =>
        {
            LabelMemory.Clear();
            LabelMemory.Load(null);
            LabelMemory.Load("");
            LabelMemory.Load("   ");
            Check(LabelMemory.RememberedItemCount == 0, "空文本不应产生记忆");

            LabelMemory.Load("garbage;;::|" + "=x=:|" + ":");
            Check(LabelMemory.RememberedItemCount == 0, "损坏的记录不应产生记忆（实际 " +
                LabelMemory.RememberedItemCount + "）");
            LabelMemory.Clear();
        });

        Run("标签记忆：记忆里的分类或条件在当前游戏已不存在时跳过该条，其余照常解析", () =>
        {
            LabelMemory.Clear();
            LabelMemory.Load("coffee_tablet=" + ChemicalCategory + ":removed_condition:0:0;" +
                             StampCategory + ":genuineStamp:0:0;" +
                             "CATEGORY_REMOVED:whatever:0:0");

            Check(LabelMemory.TryResolve("coffee_tablet", out List<LabelBatch.LabelEntry> labels), "仍有效的分类应被解析出来");
            Check(labels.Count == 1, "失效的分类/条件应被跳过（实际 " + labels.Count + "）");
            Check(labels[0].Category == StampCategory, "解析出的应为仍然有效的那个分类");
            LabelMemory.Clear();
        });

        Run("右键套用记忆：把记住的标签写进该类物品（假条件改写 + 未揭穿）", () =>
        {
            LabelMemory.Clear();
            Remember("coffee_tablet", ChemicalCategory, "lab_pure");

            GameItemElement target = Item("coffee_tablet",
                Feature(ChemicalCategory, Condition("junk_grade"), Condition("junk_grade")));
            Check(LabelMemory.TryResolve(target.identifier, out List<LabelBatch.LabelEntry> labels), "应解析出记忆标签");

            int changed = LabelBatch.ApplyItem(target, labels);
            Check(changed == 1, "应改动 1 个标签槽位（实际 " + changed + "）");
            Check(target.itemFeatures[0].fakeCondition!.identifier == "lab_pure", "假条件未变成记忆中的实验室纯级");
            Check(target.itemFeatures[0].realCondition!.identifier == "junk_grade", "真实条件不应被改动");
            Check(!target.itemFeatures[0].IsExposed(), "套用记忆标签后应处于未揭穿状态");
            LabelMemory.Clear();
        });

        Run("右键套用记忆：水质标签连 customIntValue1 一起套用", () =>
        {
            LabelMemory.Clear();
            Remember("water_pitcher", WaterCategory, "waterBase", customIntValue1: 9800, copyCustomIntValue: true);

            GameItemElement target = Item("water_pitcher",
                Feature(WaterCategory, Condition("waterPure"), Condition("waterPure"), customIntValue1: 9990));
            Check(LabelMemory.TryResolve(target.identifier, out List<LabelBatch.LabelEntry> labels), "应解析出水质的记忆标签");

            LabelBatch.ApplyItem(target, labels);
            Check(target.itemFeatures[0].fakeCondition!.identifier == "waterBase", "水质假标签未套用");
            Check(target.itemFeatures[0].customIntValue1 == 9800,
                "水质自定义值未套用（实际 " + target.itemFeatures[0].customIntValue1 + "）");
            LabelMemory.Clear();
        });

        Run("标签记忆：空物品类型、空标签列表一律忽略", () =>
        {
            LabelMemory.Clear();
            LabelMemory.Remember(null, new[] { new LabelBatch.LabelEntry(ChemicalCategory, Condition("lab_pure"), 0, false) });
            LabelMemory.Remember("", new[] { new LabelBatch.LabelEntry(ChemicalCategory, Condition("lab_pure"), 0, false) });
            LabelMemory.Remember("coffee_tablet", null);
            LabelMemory.Remember("coffee_tablet", new List<LabelBatch.LabelEntry>());
            Check(LabelMemory.RememberedItemCount == 0, "这些调用都不应产生记忆（实际 " +
                LabelMemory.RememberedItemCount + " 种）");
        });

        Console.WriteLine();
        Console.WriteLine(_failed == 0 ? "全部通过：" + _passed + " 项" : "失败 " + _failed + " 项 / 共 " + (_passed + _failed) + " 项");
        Environment.ExitCode = _failed == 0 ? 0 : 1;
    }

    private static void SetupConditionRegistry()
    {
        ItemConditionList.conditionLookup[CigaretteCategory] = new List<ItemCondition>();
        ItemConditionList.conditionLookup[StampCategory] = new List<ItemCondition>
        {
            Condition("genuineStamp"), Condition("counterfeitStamp")
        };
        ItemConditionList.conditionLookup[WaterCategory] = new List<ItemCondition>
        {
            Condition("waterPure"), Condition("waterBase")
        };
        ItemConditionList.conditionLookup[WineCategory] = new List<ItemCondition>();
        ItemConditionList.conditionLookup[ChemicalCategory] = new List<ItemCondition>
        {
            Condition("lab_pure"), Condition("junk_grade")
        };
    }

    private static void Run(string name, Action test)
    {
        try
        {
            test();
            _passed++;
            Console.WriteLine("PASS " + name);
        }
        catch (Exception ex)
        {
            _failed++;
            Console.WriteLine("FAIL " + name + " :: " + ex.Message);
        }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception(message);
        }
    }

    private static ItemCondition Condition(string identifier)
    {
        return new ItemCondition { identifier = identifier, display = identifier };
    }

    private static ItemFeature Feature(string category, ItemCondition fake, ItemCondition real,
        int customIntValue1 = 0, bool exposed = false)
    {
        var feature = new ItemFeature
        {
            category = category,
            identifier = category,
            fakeCondition = fake,
            realCondition = real,
            customIntValue1 = customIntValue1,
        };

        if (exposed)
        {
            feature.SetExposed(true);
        }

        return feature;
    }

    private static GameItemElement Item(string identifier, params ItemFeature[] features)
    {
        return CreateItem(identifier, owned: true, features);
    }

    private static GameItemElement NonOwnedItem(string identifier, params ItemFeature[] features)
    {
        return CreateItem(identifier, owned: false, features);
    }

    private static GameItemElement CreateItem(string identifier, bool owned, params ItemFeature[] features)
    {
        var item = new GameItemElement { identifier = identifier };
        if (owned)
        {
            item.EnableTag("IS_OWNED_TAG");
        }

        item.itemFeatures.AddRange(features);
        foreach (ItemFeature feature in item.itemFeatures)
        {
            feature.parentItem = item;
        }

        return item;
    }

    private static LabelBatch.LabelEntry? Find(List<LabelBatch.LabelEntry> labels, string category)
    {
        foreach (LabelBatch.LabelEntry label in labels)
        {
            if (label.Category == category)
            {
                return label;
            }
        }

        return null;
    }

    private static void Remember(string itemIdentifier, string category, string conditionIdentifier,
        int customIntValue1 = 0, bool copyCustomIntValue = false)
    {
        LabelMemory.Remember(itemIdentifier, new[]
        {
            new LabelBatch.LabelEntry(category, Condition(conditionIdentifier), customIntValue1, copyCustomIntValue)
        });
    }

    private static List<GameItemElement> Selection(params GameItemElement[] items)
    {
        return new List<GameItemElement>(items);
    }
}
