# 02 · 达标纯水 → 灌装 → 售卖：可行路线与 API 证据

> 作者：sell-researcher（task-2）
> 权威源码：`D:\code\mod\.tmp-decompile-current\Assembly-CSharp\`（935 个 .cs，反编译时间 2026-10-31 21:53，对应 Steam buildid 26337366）
> 交叉核对：旧快照 `D:\code\mod\Probably Stolen\water-research\`、既有研究 `CollectorTower-Design/research/02-game-water-and-bottling.md`（行号可能有偏移，仅作旁证）
> 标注约定：**【已确认】= 在权威源码中逐行核对过，附 `文件:行号`**；**【待验证】= 有线索但未逐行确认，或依赖运行时行为**。
> 本文只写结论与证据，不写实现代码。

---

## 0. 结论速览（给净水塔设计）

| 问题 | 结论 | 风险 |
|---|---|---|
| 能否**自动灌装**（把塔内水变成满瓶水） | **能，且是纯官方 API**：`WaterHelper.TransferLiquid(source, target)` 为 `public static`，无耗时/耐久/纯度校验；也可用 `WaterHelper.AddWater(container, grade, …)` 直接生成指定档位的水。装水后瓶子自动变 `WATER` 类型、`unitValue` 自动 = 40 credits/L。 | **低** |
| 能否**自动卖给顾客** | **原版没有任何"顾客自动取货并付款"的 API**，也没有成交事件 hook。玩家卖水必须：把瓶装水拖到称重台 → 议价 UI → 点 Sell。`ModHook` 里没有 OnItemSold/OnDeal 事件。 | **高（默认不可行）** |
| 有没有原生自动售卖容器可借 | **有**：`vending_fountain`（饮水机）每夜自动"灌瓶→按标签卖→现金进机器"，玩家按按钮取钱；但其卖的是**机器现场生成的瓶子**，不是玩家自己的瓶子，且会触发**假标风险**（FRAUD/ENDANGERMENT 犯罪+声望）。 | **中** |
| 售货机 `vending_machine` 能卖水瓶吗 | **不能**：上架过滤器明确拒绝 `LIQUID_CONTAINER_TAG`（非 WINE）物品。 | 已确认 |
| 水到底卖多少钱 | `unitValue` 硬编码 **40 credits/L**（与纯度无关，`WATER_PRICES` 六档常量是死代码）；但**成交价**额外按水质条件的 `modValue` 乘算：纯 **+100%（2×）**、优质 +50%、基础 0、幽灵 −33%、锈 −75%、沟渠 −90%。即纯水实际约 **80 credits/L**（另加容器底价、零售加价）。 | 已确认 |
| 净水塔最可行售卖方案 | **夜间自动灌装 + 自带售卖机器/复用饮水机结算逻辑**（纯 Mod，不改原版交易流程）；若要进入"顾客预算/声望/暴露"完整流程，需要 Harmony 或程序化模拟拖拽（QuickSell 已有先例，中高风险）。 | 中 |
| 关键卡点 | "达标纯水（≥9990，显示 99.9%+）"用原版净水器很难：`MachinePurifier` 默认纯度上限 **9900**，需品质加成 ≥100 才到 9990、≥200 才到 10000。**这是净水塔设计的核心机制点**。 | 已确认公式 |

---

## 1. 水容器 / 瓶子物品清单

【已确认】全部来自 `FoodItemDirectory.cs`；容量单位 ml，`InitLiquidContainerItem` 内部把容量写成 parts = ml × 1000（`WaterHelper.cs:51-…`，容量标签 `LIQUID_CONTAINER_CAPACITY`）。

| identifier | 容量 | 容器底价（BASE_PRICE_TAG） | 出厂 unitValue | 精灵/形状 | 证据 |
|---|---|---|---|---|---|
| `mini_bottle` | 250 ml | 3 | 3 | water_container3 "mini" | `FoodItemDirectory.cs:184-192` |
| `small_bottled_water` | 450 ml | 4 | 4 | water_container3 "small" | `FoodItemDirectory.cs:174-182` |
| `bottled_water` | 750 ml | 5 | 5 | water_container3 "normal"（透明容器） | `FoodItemDirectory.cs:164-172` |
| `large_bottled_water` | 2000 ml | 9 | 9 | water_container3 "large" | `FoodItemDirectory.cs:303-311` |
| `water_jug` | 6000 ml | 15 | 15 | water_container4 "xl" | `FoodItemDirectory.cs:313-321` |
| `med_bottle_red/blue/violet/orange` | 250 ml | 3 | 3 | 药瓶 | `FoodItemDirectory.cs:194-204` |

要点：

- **空瓶 vs 满瓶**：容器本身同一物品，状态由三个东西决定：
  1. `WaterHelper.IsEmpty(item)`（`WaterHelper.cs:641`）；
  2. `WaterHelper.UpdateContainerGoodType`（`WaterHelper.cs:586-607`）：**空 → 移除 `WATER` 标签并设 `HOUSEHOLD_GOOD`；非空 → 设 `WATER` 并移除 `HOUSEHOLD_GOOD`**。顾客"买 WATER 类型"就是靠这个标签识别。
  3. 水质特征 `CATEGORY_WATER_PURITY` + 公开/实际显示（见 §3）。
- 满瓶的 `unitValue` 由灌装流程自动改写为 `GetWaterPrice(总量)`（`WaterFeatureHelper.cs:199-220`、`:77-92`），即 40/L，**不叠加容器底价**；容器底价只在最终成交价公式里单独加（`GameItem.cs:1690-1711` 的 `marketScaledValue`）。
- 非液体容器的"水"物品：`bottled_water_premium`（高级瓶装水）——**不是液体容器**，固定物品 `unitValue=325`、类型 `LUXURY_ITEM + WATER`（`FoodItemDirectory.cs:115-125`）；`water_ration`（配给水）是消耗性食物，不是容器（`FoodItemDirectory.cs:532-551`、`SetupWaterRationPour` :574）。
- 玩家只能操作**自己拥有的**容器：`WaterHelper.CanUseWaterContainer(item) = GeneralHelper.IsItemOwned(item)`（`WaterHelper.cs:657-664`）；`CanPourVolume` 同时检查剩余容量（`:1304-1311`）。
- 瓶子来源：`bottle_printer`（水瓶打印机）只打印**空瓶**，不耗水只耗电：小 4 / 标准 6 / 大 11 点电源（`MachineBottlePrinter.cs:TryPrint` 95-106，常量区 `SMALL_PRINT_COST=4/NORMAL_PRINT_COST=6/LARGE_PRINT_COST=11`）；输出格只接受玩家拥有的液体容器（`:24`）。升级阈值：标准瓶需升级计数 ≥3、大瓶 ≥9。**不能灌水**。
- 空瓶还会从掉落/商店出现：`DropDirectory.cs:451-453`、`DropTableDirectory.cs:33-35`、`CommissaryData.cs:70`（待验证：具体掉落权重）。

---

## 2. 灌装 API

### 2.1 核心：`WaterHelper.TransferLiquid(source, target)`

【已确认】`WaterHelper.cs:267-316`，签名 `public static void TransferLiquid(GameItem sourceContainer, GameItem targetContainer)`。

执行顺序（逐行）：

1. `bool wasEmpty = IsEmpty(targetContainer);`
2. 双方必须都有 `LIQUID_CONTAINER_TAG`，否则**静默 return**（无异常、无提示）。
3. 目标剩余容量 = `LIQUID_CONTAINER_CAPACITY` − 目标各液体现量合计；≤0 静默 return。
4. 源各液体现量合计；≤0 静默 return。
5. `amount = min(目标剩余, 源总量)`；`SplitProportionally` 按源中各液体比例切分。
6. 逐个液体：目标若挂了滤芯（`FILTER_CONTAINER_TAG` + 子物品 `WATER_FILTER`）先过 `HandleFilter`；写 target += amount、source −= amount。
7. 最后调 `WaterFeatureHelper.OnLiquidContainerPourInto(target, source, wasEmpty)` 与 `OnLiquidContainerPourOutOf(source)`。

特性与约束：
- **无耗时、无耐久消耗、无纯度校验**；失败全部静默，**返回 void**——调用方只能自己对比前后体积判断灌了多少（这是净水塔脚本必须处理的坑）。
- 液体按**比例混合**，混合后纯度由全部 parts 重新计算，不会"平均两个标签"。
- 滤芯会改变转移量/成分（`WaterHelper.HandleFilter` 333-395；`uv_filter` 移除全部 microbe，普通滤芯按 `FILTER_SIZE_TAG`/particleSize/`filterMinimumPurity`/`filterVolumeRemoved` 生效）。
- 目标满时多余的水留在源容器，不会丢（与集水器 `Fill()` 的 water_jug 中转不同——那条路径超出 6 L 会丢，见 `CollectorTower-Design/research/02-game-water-and-bottling.md` §2.6）。

### 2.2 灌装时水质标签如何继承（关键）

【已确认】`WaterFeatureHelper.OnLiquidContainerPourInto(target, source, wasEmpty)` 94-147：

- `source` 是酒容器（`WINE_CONTAINER_TAG`）→ 直接 return，不走水质体系。
- **目标原本是空瓶**：
  - 源**未被暴露**（未验水/未贴试纸）：把源的"**标称纯度**"（`customIntValue1`，即 fakeCondition 对应的 presumed purity）复制给目标，目标 `SetExposed(false)`；
  - 源**已被暴露**（验过水/被顾客看穿/贴过试纸）：目标标称纯度 = 源**实际纯度**，目标 `SetExposed(true)`；
  - 复制 `STRIP_TESTED_TAG`；`fakeCondition = GetConditionFromPurity(presumed)`、`realCondition = GetConditionFromPurity(actual)`；公开/实际显示 = `"水质名 (99.9%+ Purity)"`；`unitValue = GetWaterPrice(总量)`。
- **目标原本非空**：清 `STRIP_TESTED_TAG`，**保留目标自己的标称纯度**（即旧瓶里原有水的展示标签），然后按混合后的实际纯度刷新 condition/显示/unitValue。
- 结论：**用作弊/未验水源去灌新瓶，会原样继承"假标签"**；`GhostWaterAsPureWater` 这类预制水可以骗过灌装链（`WaterPremadeHelper.cs:185-225`）。对净水塔而言：灌装正确做法是保证塔内水本身达标，标签自然正确。

### 2.3 主动写入：`AddWater` / `FillWith*` 系列

【已确认】`WaterHelper.cs:941-1070`：

`public static void AddWater(GameItem waterContainer, int grade, int addAmount = -1, bool useRange = false, int lowPara = 0, int highPara = 1, bool initWaterFeature = true)`

- 无需源容器，直接按 `grade` 生成指定档位的水（grade 0 纯 / 1 优质 / 2 基础 / 3 幽灵 / 4 锈 / 5 沟渠），按该档污染物分布字典填充，并做 `InitWaterFeature`（默认不暴露）。
- 目标纯度区间：0=(9990,9999)、1=(9900,9989)、2=(9800,9899)、3=(9600,9799)、4=(9000,9599)、5=(3500,8999)；`useRange:true` 时改用 `lowPara/highPara` 显式区间（调高纯度上限用这个）。
- 便捷包装：`FillWithPureWater` 1189 → `AddWater(…,0)`；`AddPureWater` 1194；`FillWithHighQualityWater` 1199；`FillWithBaseWater` 1209；`FillWithGhostwater` 1219（HighEnd 9700-9799 / LowEnd 9600-9650）；`FillWithRustWater` 1239（HighEnd 9550-9599 / LowEnd 9000-9200）；`FillWithGutterflow` 1259。
- **风险（设计必须注意）**：`AddWater(grade)` 会"洗白"——用沟渠水也可以重新生成 9990 纯水，破坏经济平衡。既有研究（`CollectorTower…02`）明确建议塔的**灌装走 `TransferLiquid`，不要用 `AddWater(grade)`**，除非塔内水来源单一。净水塔如果只有自己的净化产物，可用 `AddWater` 但会绕过"净化"成本。

### 2.4 官方"夜间自动灌装"范式：集水器 `MachineMoistureFarm`

【已确认】`MachineMoistureFarm.cs`：

- `machine.onCycleEndSlotItemFunc`（65-79）：`if (bottleInv.childItem != null && WaterHelper.GetFreeCapacity(bottleInv.childItem) != 0)` → 检查电力 → `volume = ApplySharedAir(GetOutputVolume(machine), runningFarmCountTonight)` → `Fill(GetOutputQuality(item), bottleInv.childItem, volume)` → `MachineHelper.OnMachineActioned`。
- `Fill(quality, container, volume)` 在 237 行；内部先 spawn 一个 `water_jug`（6000 ml，`:239`）当临时桶，`AddWater` 后 `TransferLiquid(gameItem, container)` 转入目标（`:268`）。
- `ApplySharedAir` 119-123：多台同夜运行时每多 1 台产量 ÷(1+0.25×(n−1))；`CanRunTonight` 125-150 要求电池电量足够、容器有空位。
- **净水塔可完全照抄这个"夜结回调里检查空位→灌固定体积水→刷特征/精灵"的范式**，区别只是水源换成塔内储罐、灌入目标换成空瓶。

### 2.5 其它可复用辅助 API（`WaterHelper.cs`）

| API | 行号 | 用途 |
|---|---|---|
| `GetFreeCapacity(item)` | 397 | 剩余容量（parts） |
| `GetTotalCapacity` / `GetTotalCapacityML` | 412 / 421 | 容量 |
| `GetCurrentCapacity` / `GetCurrentCapacityML` / `GetCurrentCapacityUL` | 440 / 455 / 469 | 现有量 |
| `GetWaterPurity(item)` | 557-574 | 实际纯度 = `water_parts*10000/total_parts`（整数截断） |
| `GetTotalVolume(item)` | 576-584 | 总液体量（parts） |
| `GetEstimatedVolume` | 483 | 满/几乎满/…/空 文字估算 |
| `IsEmpty` | 641 | 空判断 |
| `CanUseWaterContainer` | 657-664 | 仅玩家拥有的容器可操作 |
| `GetContainerPrice(item)` | 609-616 | 容器底价（BASE_PRICE_TAG） |
| `EmptyContainer(item)` | 618-639 | 倒空并重置水质/unitValue |
| `WillAcceptWater(target, acceptedDisplays)` | 1436-1444 | 判断目标的水质显示名是否在白名单内 |
| `GetWaterDisplayName(purity)` | `WaterFeatureHelper.cs:343-352` | 纯度 → 档位显示名 |

---

## 3. 装水后的品名 / 描述 / 顾客看到的水质

### 3.1 六档水质与显示

【已确认】`WaterFeatureHelper.cs` 顶部常量（5-73）：

| 档 | 纯度阈值 | 显示名（本地化 key 顺序） | condition modValue | condition customValue |
|---|---|---|---|---|
| 0 纯 | ≥9990 | "99.9%+"（`PURE_WATER_DISPLAY`） | +100 | 9990 |
| 1 优质 | ≥9900 | "99%-99.9%"（`HIGH_QUALITY_WATER_DISPLAY`） | +50 | 9900 |
| 2 基础 | ≥9800 | "98%-99%"（`BASE_WATER_DISPLAY`） | 0 | 9800 |
| 3 幽灵 | ≥9600 | "96%-98%"（`GHOSTWATER_DISPLAY`） | −33 | 9600 |
| 4 锈 | ≥9000 | "90%-96%"（`RUST_WATER_DISPLAY`） | −75 | 9000 |
| 5 沟渠 | ≥3500 | "90%-"（`GUTTERFLOW_WATER_DISPLAY`） | −90 | 3500 |

- 阈值数组 `WATER_PURITIES = {9990, 9900, 9800, 9600, 9000, 3500}`（`WaterFeatureHelper.cs:73`）；`GetWaterDisplayName` 从高到低取第一个 `purity >= 阈值`（:343-352），低于 3500 显示 `mech_unknown_water`。
- 物品显示文案由 `InitWaterFeature` 写成 **`"水质名 (99.9%+ Purity)"`**（`WaterFeatureHelper.cs:199-220`：`GetWaterDisplayName(gameItem) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(purity)] + " Purity)"`）。这是玩家/顾客看到的公开文案。
- condition 定义在 `ItemConditionList.cs:325-389`，数组顺序 `conditionWaterPurity`（`:39-47`）= {Pure, HighQuality, Base, Ghost, Rust, Gutterflow}，**下标即 `GetPurityArrayIndex` 的返回值**（灌装/机器代码都用这个下标）。

### 3.2 标称 vs 实际：顾客看到的是"假标签"

【已确认】

- 水质特征 ItemFeature 同时挂 `fakeCondition`（标称）与 `realCondition`（实际），`useCondition=true`。
- 售价取哪个由 `ItemFeature.GetCurrentValueMod()` 决定（`ItemFeature.cs:380-399`）：**未暴露 → `fakeCondition.modValue`；已暴露 → `realCondition.modValue`**。
- 暴露渠道：
  - 手电检查：`HandleWaterCheckFlashlight`（`WaterFeatureHelper.cs:389-403`），实际纯度 <9600 才能被照出来，否则加 `WATER_PURITY_INCONCLUSIVE` 标记；
  - 试纸：`HandleWaterCheckTestingStrip`（:426-439）→ `EnableTag(STRIP_TESTED_TAG)` + `DiscoverFeature` + `ExposeFeature(lowkey:true)` + `InitWaterFeature(isExpose:true)`；
  - 顾客对话/验水流程：`UpdateWaterFeatureDialog`（:476-520，实际比标称差 → 暴露并播 rust/gutter/ghost 等台词；实际更好 → +3 阵营声望）。
- "危险水"判定：`IsLabelledHazardousWater`（:329-341）= 液体容器 && 非化学产品 && 非空 && 特征存在 && **（已暴露 ? 实际 : 标称）纯度 < 9800**；顾客属性 `refusesHazardousWaterLabel` 会因此拒收（`StoreClient.IsClientRefusingItem`，`StoreClient.cs:356-405`，具体行 386 附近；游客 `StoreClientList.cs:3550-3600` 设置 `refusesHazardousWaterLabel = true`）。
- 顾客会要求先取出滤芯："Take that filter out first. I only want to buy the water."（`StoreClient.IsRefusingForFilterInside` / `TrySayRemoveFilterLine`，`StoreClient.cs:469-502`）。

**净水塔设计含义**：把塔内水灌进瓶子时，`TransferLiquid` 会继承塔内水的真实纯度与"是否暴露"状态。塔的净化应保证真实纯度达标（≥9800 才不算危险水，≥9600 才可能被高端水客买，≥9990 才是"纯水档"）。

---

## 4. 水价

### 4.1 基础价：`GetWaterPrice` 恒为 40 credits/L

【已确认】`WaterFeatureHelper.cs:369-377`：

`GetWaterPrice(int totalLiquidParts)` 内部 `int num = 40;`，返回 `round(40 × parts / 1_000_000)`；`parts = ml × 1000`，所以即是 **40 credits / L**，**与纯度、容器、难度完全无关**。`GetWaterPrice(GameItem)`（:379-387）先把所有液体 parts 加起来。

`WATER_PRICES = {200,100,40,27,20,10}`、`PURE_WATER_PRICE_PER_LITER=200` 等六档常量（`WaterFeatureHelper.cs:47-59`）**全仓库无引用，是死代码**（grep 只命中定义处）。因此"纯水 200/L"的传闻在正式版**不成立**（旧快照/攻略文档可能过时）。

### 4.2 实际成交价：基础价 × 水质条件乘数 + 容器底价

【已确认】`GameItem.cs`：

- `AccumulateFeatureStages` 1661-1681：非 disabled 特征按 `GetValueStage()` 计入——默认 **Innate**（水 purity 特征属于此类，`legacyFinalStageIds` 只含 bargain/blackmail 系列，`ItemFeature.cs:85`），`innateMult += GetCurrentValueMod()`；
- `ComposeStagedValue` 1683-1688：`round((unitValue × (1 + innateMult/100) + 容器底价) × (1 + marketMult/100) + misc)`；
- `GetNegociatedValue()` 1690-1711：即玩家卖出成交价（`PlayerStore.SellItem` 用它），`marketScaledValue = WaterHelper.GetContainerPrice(this)`。
- 水质 feature 的 `valueStage` 默认 `Innate`（`ItemFeature.cs:16-29` 枚举、构造函数 `:103-108`、`GetValueStage` 116-130），所以 **六个 condition modValue 直接乘在 unitValue 上**。

等价公式（忽略 misc / 事件）：

```
成交价 ≈ round( 40 × 升数 × (1 + modValue/100) + 容器底价 )
```

其中 modValue：纯 +100、优质 +50、基础 0、幽灵 −33、锈 −75、沟渠 −90。

### 4.3 价格样例表（议价成交价，`GetNegociatedValue`，未含商店事件/零售加价）

| 容器（容量/底价） | 纯水 9990+ | 优质 9900 | 基础 9800 | 幽灵 9600 | 锈 9000 | 沟渠 3500 |
|---|---|---|---|---|---|---|
| mini_bottle 250ml/3 | 23 | 18 | 13 | 10 | 6 | 4 |
| small_bottled_water 450ml/4 | 40 | 31 | 22 | 16 | 8 | 6 |
| bottled_water 750ml/5 | **65** | 50 | 35 | 25 | 12 | 8 |
| large_bottled_water 2000ml/9 | 169 | 129 | 89 | 63 | 29 | 17 |
| water_jug 6000ml/15 | 495 | 375 | 255 | 176 | 75 | 39 |

（单位 credits；纯水 750ml 的实际基础单价约 **65/0.75 ≈ 86.7 credits/L**，即 40 × 2 + 容器摊销；水源本身仍是 80/L。）

### 4.4 零售加价与自动售卖机加成（`GetCurrentValue`，非议价）

【已确认】

- `GameItem.GetCurrentValue(...)` 1721-1764 在 `useRetailMarkup`/`forceMarkup` 时把 `PlayerStore.Instance.retailMarkup` 加进 marketMult（`:1745`）；默认值 **15**（`PlayerStore.cs:97`），难度覆盖：`Difficulty.cs:210=20 / 252=15 / 294=0 / 336=0`（难度名对应待验证）。
- `VendingMachineBonus()`（`ItemFeatureList.cs:184-195`）：`valueStage=Market`、`SetPreExposedValueMod(10)` → **+10% 市场乘数**；夜售时会被加到物品上。
- 饮水机/售货机用 `GetCurrentValue(useRetailMarkup:true, withChild:true, includeEvents:false, forceMarkup:true, 0)`。默认难度下：纯水 750ml ≈ `(30×2 + 5) × 1.25 = 81`；优质 750ml ≈ 62；基础 750ml ≈ 44；纯水 2L ≈ `(160+9)×1.25 = 211`。
- **玩家没有"逐件标价"入口**：原版只有全局 `retailMarkup` 与议价/特价特征（bargain/blackmail 系列，`ItemFeature.cs:85` 的 legacyFinalStageIds）；饮水机没有价格参数，按 `GetCurrentValue` 结算。定价策略实际是"选水质档 + 选瓶型 + 全局零售加价"。

---

## 5. 售卖逻辑：谁能买、怎么成交

### 5.1 重要纠正：`StoreClientListWater.cs` 是"水供应商"，不是"买水顾客"

【已确认】`StoreClientListWater.cs`（178 行，全文核对）：

- `GetRandomWaterSupplier()`（3-18）：20% 诚实水贩 `CreateHonestWaterSeller`、12% 水公司 `CreateWaterCompany`、7% 不诚实水贩 `CreateDishonestWaterSellScav`、其余 `CreateWaterSellScav`。
- 四种全部 `storeClient.clientIntent = ClientIntent.SELL`（它们**卖水给玩家**），并 `SetWholesale(-15~-40)`。
- 到店时通过 `PlayerStore.Instance.AddDirectSellingItemToTable(...)` 把水**放到玩家的称重台/后仓**（玩家可议价买下）：水公司 1-2 瓶随机 + 可能 GhostWaterAsBaseWater / large_bottled_water（20-60 行）；水贩 3-5 瓶随机（71-78）；不诚实水贩加密 GhostWaterAsPureWater 等（84-121）；诚实水贩在未解锁 `waterTrader` 网络升级时改卖 `water_ration`（140-177，`NetworkUpgrade.IsWaterTraderUnlocked` 147 行）。
- **所以：水不是卖给"供应商"，供应商是进货渠道。玩家卖水的对象见 5.2。**

### 5.2 真正买水的顾客（水客）

【已确认】定义在 `StoreClientListTier.cs`；ID 映射在 `StoreClientListDict.cs:678-690`（字典 key→Create 方法）：

| 顾客 id | 显示/身份 | 预算 | 买（willAccept）档位 | 同时卖给你 | 证据 |
|---|---|---|---|---|---|
| `waterScavenger` | name_scavenger 拾荒者 | 60 + rng(0,40) | 非空 WATER 且显示 ∈ {**基础, 幽灵**} 或 `water_ration` | 2× small_bottled_water | `StoreClientListTier.cs:47-102` |
| `thirstySpacer` | 口渴宇航员 | 90 + rng(0,20) | ∈ {**基础, 优质, 幽灵**} 或 `water_ration` | small + normal | `:104-157` |
| `scavWaterShopper`（HydrationSeeker） | 拾荒买水人 | 120 + rng(0,30) | ∈ {**基础, 优质, 纯**} | small + normal | `:159-210` |
| `waterSpace2`（WaterShopper） | name_spacer 下城区居民 | 145 + rng(0,50) + day/4 | ∈ {**基础, 优质**} | small/normal/large | `:212-267` |
| `waterStockpiler` | 上层囤水客（upper level） | 500 + rng(0,100) + dayCounter | ∈ {**纯, 优质**}；**例外：`bottled_water_premium` 直接收** | water_jug + large | `:269-327` |

- 全部是 `ClientIntent.SELLNBUY`（既卖水给玩家，也买玩家的水）。
- 所有水客的 `willAcceptItem` 都先接受 `water_ration`；高端囤水客只认纯水/优质水，且 `canClientExposeFeature = ALLExposeCapacity`（会验水，假标容易被抓）。
- 顾客买水判定入口：`StoreClient.IsClientBuyingThisItem`（`StoreClient.cs:428-467`，匹配 `clientBuyingTagList` 里的 `"WATER"` 或 ID 白名单）；`PlayerStore.CanSellThisItem`（`PlayerStore.cs:2569-2598`）还会检查顾客预算、明细暴露风险等。
- 另：游客 `CreateTourist`（`StoreClientList.cs:3550-3600`）也会 `clientBuyingTagList.Add("WATER")`、预算 180-250，但 `refusesHazardousWaterLabel=true`（<9800 标称的未曝光水会被拒）。

### 5.3 顾客如何被吸引（陈列柜机制）

【已确认】

- 陈列柜 = `EmporiumEntry.Instance.showcaseElement`，`new GameGridInventory(7,5)`（`EmporiumEntry.cs:215`；可通过升级扩到 8x6/9x7，`EmporiumEntry.cs:1429-1438`，行号待复核）。
- 每个开门日 `ShowcaseHelper.GetDisplayCasePrediction()`（`ShowcaseHelper.cs:210-265`）→ `HandleDisplayCase()`（:362-377）生成顾客；`StoreClientManager` 消费预测（`StoreClientManager.cs:1293/1304`）。
- **水客由陈列柜里的水"最主打档位"决定**：`GetMostCommonWaterTypeInDisplay()`（`ShowcaseHelper.cs:436-473`）遍历陈列柜中 `WATER` 物品，按 `waterFeature.fakeCondition.identifier` 分组、按 `GetCurrentValue` 加权（`water_ration` 记为基础水）。
- `CreateClientByStockType("WATER")`（:475-505）：
  - 总价值 ≥350 **或** 最主打为纯水档，且 day ≥14 → **`waterStockpiler`**（高端）；
  - 总价值 ≥145 **或** 有纯/优质档 → `waterSpace2`；
  - 店铺吸引力 ≥400 → `waterSpace2`；
  - 否则从 `t1WaterClientIds = {thirstySpacer, scavWaterShopper}` 随机（`:12`）。
- 另有 `sign_water` 招牌类陈列品，可提高水供应商到访（`GetDisplayCasePrediction` 210-265；`ToolDirectory.cs:294-297`，待验证具体权重）。
- **结论：把纯水瓶放进陈列柜能显著提高高端买水客概率，但陈列柜只负责"吸引"，不负责成交。**

### 5.4 成交必须经过玩家操作（没有自动成交 API）

【已确认】

- 卖水流程：玩家把瓶装水从后仓**拖到称重台**（`PlayerStore.AddDirectToWeightedTable`，`PlayerStore.cs:2541-2545`；目标是 `EmporiumEntry.Instance.backInvinvElement`）→ `NegociationUIManager` 议价 UI → 点 Sell → `PlayerStore.SellItemFromTable()`（`:2739-2770`）→ `PlayerStore.SellItem(GameItem)`（`:2676-2716`）。
- `SellItem` 内：`PerformOnSoldCheck(item, storeClient)`（`:2718-2745`：违禁品/脏水/危险品检查）→ `num = (int)item.GetNegociatedValue()` → 顾客预算截断 → `playerCash += num` → 声望 `StoreReputation.OnItemTraded` → `RunStats.OnItemSold` → `SetItemOwned(false)` → `SoldItem.TrackItem`。
- `HandleAutoNegoUIDisplay`（`:2873-2893`）只在物品放到称重台时**自动打开议价 UI**（依赖 `EmporiumOption.AutoOpenNegotiationUI`），**不是自动成交**。
- 全仓库 grep `AutoNego|autoNegociation|autoSell|AutoSell` 只命中这两处（`PlayerStore.cs:2829/2873`）→ **原版不存在自动售卖开关**。
- `ModHook`（`ModHook.cs`，430 行全文核对）**没有物品成交/顾客购买相关事件**；只有顾客生成（`OnGenerateCustomer*`）、开关门、睡眠、夜结（`OnHandlingNightlyServicesEarly/Late`）、夜间地点等。
- 卖给当铺 / Dr. Jackson：`DrJacksonLocation.cs`（153 行）是升级商人，`StoreClientList.cs:1188-1240 CreateInventorStorage` 只卖设备，**Jackson 不收水**；`StoreClientListProcurement.cs`（143 行）/`AfterhourEvents.cs`（296 行）待验证（无证据表明收水）。→ **水的唯一"回收"是水客/游客购买。**

### 5.5 顺带一提：议价以外的官方售卖路径都不可用

- `vending_machine`（售货机）：`VendingMachineHelper.cs:108-127` 上架过滤器 `return (!item.IsTag("LIQUID_CONTAINER_TAG") || item.IsTag("WINE_CONTAINER_TAG")) ? true : false;`（行 126）→ **普通水瓶被拒**；夜结 `HandleNight` 169-188 只卖普通商品。
- `vending_fountain`（饮水机）：原生支持水，但卖的是**机器自己生成的瓶子**（见 §6.1）。

---

## 6. 自动售卖可行性

### 6.1 原生唯一自动水售卖：`vending_fountain`（饮水机）

【已确认】`VendingFountainHelper.cs`（396 行，全文核对）：

- 解锁：网络升级 `VENDING_FOUNTAIN` → `VendingFountainHelper.UnlockVendingFountain()`（`NetworkUpgradeList.cs:540-560`；也可在房东 `landlordVending` 对话花 rent+150 解锁，`StoreClientList.cs:2711-2765`）。家具注册 `FurnitureItemDirectory.cs:32-35`。
- 三水槽：`baseWaterInv` / `highQualityWaterInv` / `pureWaterInv`，可放"水容器"（`WATER` 类型 + `LIQUID_CONTAINER_TAG` + 玩家拥有）**或直接放水机器**（`water_purifier` / `moisture_farm`，`IsWaterSourceMachine` 144-155，`GetWaterSource` 171-183 会穿透取机器 slot 2 内的容器）。
- 夜结触发：`onCycleEndLateSlotItemFunc`（103-117）→ 任一水槽非空且有电 → `HandleNight(machine, base, hq, pure)`；耗电 2 + 吸引力升级数/2（305-317，常量 `PARTS_PER_POWER_STEP=2/POWER_STEP=1`）。
- **`HandleNight` 195-267 售卖规则**（`@int = VENDING_ATTRACTIVENESS`，初始 15，可升级到 100）：

| 吸引力 | 基础水槽(label 2) | 优质水槽(label 1) | 纯水槽(label 0) |
|---|---|---|---|
| 任意 | 固定卖 1× bottled_water | 固定卖 1× small_bottled_water | — |
| ≥20 | 追加 1× mini_bottle | — | 1× mini_bottle |
| ≥25 | — | 追加 1× mini_bottle | — |
| ≥30 | 追加 1× small | — | — |
| ≥40 | 追加 1× bottled | — | — |
| ≥50 | 追加 1× large | 追加 1× small | 纯水槽瓶型 small |
| ≥70 | 追加 1× mini（第二轮） | — | — |
| ≥75 | — | 追加 1× bottled | 纯水槽瓶型 bottled |
| ≥80 | 追加 1× small（第二轮） | — | — |
| ≥90 | 追加 1× bottled（第二轮） | — | — |
| ≥100 | 追加 1× large（第二轮） | 追加 1× large | 纯水槽瓶型 large |

- **`SimulateWaterSale(labelPurity, containerID, waterInv, machine)` 269-303（public static，可被 Mod 直接调用）** 流程：
  1. `source = GetWaterSource(waterInv)`；空则 return；
  2. 取**实际**纯度 → `GetConditionFromPurity(...).identifier`；若为 **沟渠或锈水档 → return（不卖）**；
  3. `ItemSpawner.Spawn(containerID)` 生成新瓶 → `unitValue = 0` → `WaterHelper.TransferLiquid(source, 新瓶)`（自动灌满、自动设 unitValue）；
  4. 取 `CATEGORY_WATER_PURITY` 特征 → `fakeCondition = conditionWaterPurity[labelPurity]`、`SetExposed(false)`、`OnWaterLabelUpdated`；即**按传入档位贴标签，与实际纯度无关**（这就是假标来源）；
  5. `AddItemFeature(VendingMachineBonus())` → `value = GetCurrentValue(useRetailMarkup:true, withChild:true, includeEvents:false, forceMarkup:true, 0)`；
  6. 若 `SecData.GetMislabeledWaterWeight(...) > 0` → `VendingMachineHelper.TrackMislabelledFountainSale(labelPurity, value*weight)`；
  7. `PerformOnSoldCheck(新瓶, null)` → `Destroy()` → `VENDING_CASH += value` → `RunStats.OnFountainSale(value)`。
- 现金：卖的钱记在机器 `VENDING_CASH` 标签，玩家点按钮 → `VendingMachineHelper.TransferCashToPlayer(machine)`（`VendingMachineHelper.cs:511-520`）。
- **假标风险**：TrackMislabelledSale/FountainSale（`VendingMachineHelper.cs:190-221`）+ `SettleVendingMislabels`（347-380）会在夜结结算 `FRAUD`/`ENDANGERMENT` 犯罪、按 LL/UL 声望比例扣声望、写 `mech_nightlog_vending_mislabel` 日志（细节参数待验证）。**把高纯度水当低档卖（label 比实际低）通常无风险；把低纯度水按纯水卖才是欺诈**。

### 6.2 净水塔自动售卖的三条路线

| 路线 | 做法 | 依赖 | 风险 |
|---|---|---|---|
| **A. 灌装 + 复用饮水机结算**（推荐） | 夜间把塔内纯水灌成满瓶（`TransferLiquid`，范式抄 `MachineMoistureFarm` 65-79）→ 放入 `vending_fountain` 水槽 → 由原版 `HandleNight`/`SimulateWaterSale` 自动卖、现金进机器 | 纯官方 API；需要饮水机家具/电力/解锁；可只调 `VendingFountainHelper.SimulateWaterSale`（`public static`）而不用改任何原版逻辑 | **中低**：假标只由 label 决定，Mod 传入与实际档位一致的 labelPurity 即可规避；卖出的是机器生成瓶（不是玩家自己的瓶子），但经济效果等价 |
| **B. 自建售卖机器（夜结结算）** | 注册自己的机器物品，挂 `onCycleEndSlotItemFunc`/`onCycleEndLateSlotItemFunc`（夜结时 `PlayerStore` 会遍历所有 owned item 并调用，`PlayerStore.cs:1717-1730`），内部复制 `SimulateWaterSale` 的结算步骤（`GetCurrentValue(forceMarkup)` + `PerformOnSoldCheck` + `RunStats` + 现金标签） | 注册物品（`FoodItemDirectory`/`FurnitureItemDirectory` postfix，参考 WaterTower-Final `RegisterItems`）、`ModHook` 夜结事件或物品回调 | **中**：结算字段/统计/犯罪接口较多，容易漏（如 SoldItem.TrackItem、SecData 检查）；不触碰原版交易 UI，但仍属"模拟原版行为" |
| **C. 程序化卖给真实顾客** | 顾客到店时把瓶装水移到 `backInvinvElement`/称重台并调用 `PlayerStore.SellItemFromTable()` 或 `SellItem()`；需先通过 `CanSellThisItem`（顾客确实买、预算足够、特征不可暴露等） | 纯 Mod 可做（QuickSell 已实现"快速卖"：`QuickSell/Trading.cs:84 GetStagedQuote`、`:191/347` 检查顾客/预算/交易锁、`:479 CheckedSplitQuote`），但要处理 UI、当前顾客、曝光风险、预算、TryReinspectBeforeSale 等 | **中高**：`TryReinspectBeforeSale`/`CanClientExposeAnyFeatureDeep` 可能强制验水或锁 UI；不同顾客 intent/interaction 状态多；建议仅在"顾客已在店且 intent=BUY/SELLNBUY"窗口内触发 |

### 6.3 自动灌装可行性（低风险）

- 官方有 `public static WaterHelper.TransferLiquid`、`AddWater`、`GetFreeCapacity`、`InitWaterFeature` 等全部公开 API，无耗时/耐久/权限校验；夜结窗口由原版 `PlayerStore` 遍历所有物品的 `onCycleEndSlotItemFunc` 提供（`PlayerStore.cs:1717-1730`），Mod 只需给自己的机器挂回调即可（`MachineMoistureFarm` 是官方范例，`MachineMoistureFarm.cs:65-79`）。
- 唯一注意：`TransferLiquid` 静默失败、返回 void；要自己先检查 `GetFreeCapacity>0`、`GetTotalVolume>0`、双方 tag。
- **"达标纯水"是经济核心**：原版 `MachinePurifier.PurifyContainer`（`MachinePurifier.cs:114-141`）默认上限 `GetMaxPurityCap(0) = 9900`（只能到 99%），需品质加成 **≥100** 才到 9990（纯档阈值），**≥200** 才到 10000；`GetPurityMult` 164-191（≥100→0.1、≥200→0）。品质加成来自模块/机器品质（`MACHINE_QUALITY_RATIO_INT=2`、`ModuleEffectHelper`，待验证具体叠加公式）。**净水塔若想产出 9990+ 的纯水，必须设计品质加成 ≥100 的路线（升级/模块），否则只能卖优质档（9900，+50%）**。

---

## 7. 现有 mod 先例（可复用模式）

| 项目 | 关键点 | 证据 |
|---|---|---|
| `WaterTower-Final`（自建水塔） | `TowerPrototype.cs:134-150 EndNight`：Harmony postfix 原版 EndNight，遍历塔内集水器调 `TowerSettlement.Run(tank, machines, filters)`（夜间结算范式）；`TowerSettlement.cs:10 Run / 49 FilterTank`：把机器产出汇入塔罐并过滤；`OfferTower` 155-168：`PlayerStore.Instance.AddDirectSellingItemToTable(item)` 把塔物品直接摆上成交台（"上架"入口）；`RegisterItems` 124-132：postfix `FoodItemDirectory` 注册自定义物品；`RestoreSlotLocks` 170-181、`TurboTarget` 183-196（对拖拽目标即时结算） | 已确认（读取方法签名与结构） |
| `WaterMerchant-Demo`（水商参数 mod） | `WaterMerchant.cs`：Harmony patch 辅助 `PatchPrefix/PatchPostfix`（74-91）、Settings 开关（FarmOutput/FarmPower/FarmAir/FarmPrice/FilterMicrobe/FilterZero/UvPitcher/PurifierAmount/PurifierQuality/PortableCapacity/PortableMix/Recipes，117）；`FarmCreated/PitcherCreated/PortableCreated/FarmOutput` 等 postfix 类（260-264+）改集水器产量/电力/共享空气/价格、滤芯、UV、净水器产量与品质、便携容器容量 | 已确认（结构与常量）；细节待验证 |
| `QuickSell`（快速售卖） | `Trading.cs:84 GetStagedQuote(item, client)`；`:191` 检查 `client.IsClientBuying()`/预算/`NegociationUIManager.IsTradeLocked()`；`:347/:373` 顾客/交易锁/`table.IsInsertLocked()` 守卫；`:479 CheckedSplitQuote`、`:495 SplitCandidate`、`:510 ChangeStackAmount`——**程序化"批量摆上柜台并成交"的成熟范式**，可用于参考路线 C 的窗口与守卫逻辑 | 已确认（方法签名与守卫条件） |
| `AquaSense` / `AquaSense-Final` | 水相关 mod（目录存在，`AquaSense/AquaSense.cs` 命中 TransferLiquid 关键词） | 待验证（未读） |
| `CollectorTower-Design/research/02-game-water-and-bottling.md` | 既有研究：集水器每夜 850ml×(1+性能)、共享空气 25%/台、water_jug 6L 中转会截断、`TransferLiquid`/`AddWater` 静默失败清单、六档水与水瓶定义等；**行号基于旧快照**（`TransferLiquid` 记 :265-314，当前权威文件为 :267-316），其"纯度不影响 unitValue"结论需修正为"不影响 unitValue，但通过 condition modValue 影响**成交价**"（本文 §4.2） | 交叉核对 |

---

## 8. 待验证清单（按优先级）

1. **纯水品质加成来源**：`MachinePurifier` 的 `GetCurrentQualityBonus` 如何叠加到 ≥100（模块/机器品质/工具）；`MACHINE_QUALITY_RATIO_INT=2` 的语义。→ 决定净水塔能否稳定产出 9990。
2. **假标结算细节**：`SecData.GetMislabeledWaterWeight` 的判定边界（label 与实际纯度差多少构成 fraud）、`SettleVendingMislabels` 的声望损失数值、`TrackMislabelledFountainSale(labelPurity...)` 中 1→0.5、0→1.0 的具体含义。
3. **Elective vending fountain 现金上限/取钱交互**：`TransferCashToPlayer` 511-520 的实现细节（是否有上限/税费）。
4. **零售加价可控性**：`retailMarkup` 是否有升级/网络/事件可改（仅确认默认 15 与难度 20/15/0/0，`Difficulty.cs:210/252/294/336` 对应难度名待验证）。
5. **陈列柜物品是否会被顾客直接买走**：现有证据表明陈列柜只吸引顾客、成交仍需称重台（`ShowcaseHelper` 无成交逻辑）；需再查 `StoreClientManager.cs:1293` 附近确认。
6. **顾客验水/暴露时机**：`CanClientExposeAnyFeatureDeep`、`TryReinspectBeforeSale`（`PlayerStore.cs` 中调用）何时触发，决定路线 C 的可行性。
7. **Jackson/Afterhour 是否收水**：`StoreClientListProcurement.cs`、`AfterhourEvents.cs` 未读。
8. `waterStockpiler` 收 `bottled_water_premium` 的路径（`StoreClientListTier.cs:291-307` 已确认 accept，但 premium 非容器、无法灌装；只能靠捡/买）。
9. `PlayerStore.SellAllItemFromTable`（2788-2802）疑似重复累加 bug——若路线 C 复用需注意。
10. `EmporiumEntry.vendingFountainElement` 的独立夜结入口（`PlayerStore.cs:1735-1743` 只 Hide 窗口；实际结算由 `FindAllItem()` 遍历 `vendingFountainWindow`（1748-1763）后调 `onCycleEndLateSlotItemFunc`，已确认）。

---

## 附录 A · 关键文件与行号索引

| 文件（权威源码目录下） | 关键行 |
|---|---|
| `WaterHelper.cs` | 24 WaterBottleIds；51 InitLiquidContainerItem；267-316 TransferLiquid；397 GetFreeCapacity；557-574 GetWaterPurity；576-584 GetTotalVolume；586-607 UpdateContainerGoodType；609-616 GetContainerPrice；618-639 EmptyContainer；641 IsEmpty；657-664 CanUseWaterContainer；941-1070 AddWater；1189-1266 Fill 系列；1304-1311 CanPourVolume；1436-1444 WillAcceptWater |
| `WaterFeatureHelper.cs` | 5-73 六档常量/阈值；77-92 PourOutOf；94-147 PourInto；169-197 GetWaterFeature；199-220 InitWaterFeature；222-225 GetConditionFromPurity；227-264 UpdateWaterFeature；329-341 IsLabelledHazardousWater；343-352 GetWaterDisplayName；369-377 GetWaterPrice；379-387 GetWaterPrice(GameItem)；389-439 手电/试纸；476-520 暴露对话；568-598 OnWaterLabelUpdated |
| `FoodItemDirectory.cs` | 164-172 bottled_water；174-182 small；184-192 mini；194-204 med_bottle_red；303-311 large；313-321 water_jug；532-551/574 water_ration |
| `ItemConditionList.cs` | 39-47 conditionWaterPurity；325-389 六档 condition（modValue/customValue） |
| `ItemFeature.cs` | 16-29 ValueStage；85 legacyFinalStageIds；116-130 GetValueStage；380-399 GetCurrentValueMod |
| `GameItem.cs` | 1661-1681 AccumulateFeatureStages；1683-1688 ComposeStagedValue；1690-1711 GetNegociatedValue；1721-1764 GetCurrentValue；1810-1815 GetBaseValue |
| `PlayerStore.cs` | 97 retailMarkup；1686-1735 夜结（1717-1730 物品回调）；2541 AddDirectToWeightedTable；2569-2598 CanSellThisItem；2676-2716 SellItem；2718-2745 PerformOnSoldCheck；2739-2770 SellItemFromTable；2873-2893 HandleAutoNegoUIDisplay |
| `VendingFountainHelper.cs` | 103-117 夜结回调；144-169 水源接受；171-193 GetWaterSource/IsSourceEmpty；195-267 HandleNight；269-303 SimulateWaterSale；305-317 耗电 |
| `VendingMachineHelper.cs` | 50-69 解锁；108-127 上架过滤（行 126 拒水瓶）；169-188 HandleNight；190-221 假标追踪；347-380 假标结算；425-458 售出概率；511-520 TransferCashToPlayer |
| `ShowcaseHelper.cs` | 12 t1 水客；210-265 陈列预测；362-395 HandleDisplayCase/GetDisplayCaseType；436-473 GetMostCommonWaterTypeInDisplay；475-505 CreateClientByStockType |
| `StoreClientListWater.cs` | 全文 178 行（水供应商，SELL） |
| `StoreClientListTier.cs` | 47-102/104-157/159-210/212-267/269-327 五类水客 |
| `StoreClientList.cs` | 1188-1240 Jackson 库存客户；3550-3600 游客（买 WATER）；2711-2765 landlordVending |
| `MachineMoistureFarm.cs` | 65-79 夜结灌装；119-123 ApplySharedAir；237 Fill |
| `MachinePurifier.cs` | 18-26 常量/曲线；30-112 机器初始化；114-141 PurifyContainer；164-191 GetPurityMult/GetMaxPurityCap；217-283 RemoveLargestFirst |
| `MachineBottlePrinter.cs` | 24 输出格过滤；95-106 TryPrint（4/6/11 电） |
| `ModHook.cs` | 1-430 事件表（无成交事件） |

## 附录 B · 与旧快照/既有文档的差异

- `CollectorTower-Design/research/02-game-water-and-bottling.md` 引用的行号基于旧快照 `water-research/`（如 `TransferLiquid :265`），权威源码为 `:267-316`；结论层面无冲突，但"纯度不影响价格"应补充"通过 condition modValue 影响成交价"。
- 旧快照中的 `WATER_PRICES` 六档价（200/100/40/27/20/10）在正式版仍是**死代码**，实际基础价恒 40/L。
