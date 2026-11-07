# 《深空当铺》正式版「净水器产水路线」机制研究

- 游戏：Probably Stolen（Steam）正式版，buildid `26337366`
- 权威源码：`D:\code\mod\.tmp-decompile-current\Assembly-CSharp\`（935 个 .cs，ILSpy 反编译，2026-10-31 21:53 生成）
- 旧快照：`D:\code\mod\Probably Stolen\water-research\`（2026-10-28，仅交叉核对，见 §10）
- 结论标记：`已确认` = 已读正式版源码逐行确认；`待验证` = 依赖调用链/配置未完全展开；`推算` = 由公式手算、非源码直接给出
- 本文只写机制与数值，不含实现代码。

---

## 0. 结论速览

1. **净化器（water_purifier）是唯一能“按比例提升纯度”的机器**。
   - 每夜（`onCycleEndSlotItemFunc`）只处理**瓶槽里的 1 个容器**，耗电 **4**，且只有实际移除量 `removed > 0` 时才扣电。
   - 单夜可移除的杂质预算：`budget = (min(V,10L) × 0.003 + 25ml) × dirtBonus × 1.25`（V 为容器总液量）。
   - `dirtBonus` 由当前纯度查表（水越脏倍率越高，最高 20），并加上性能加成 `perf/100`。
   - **纯度上限**由品质加成决定：默认 cap=99.00%（= High Quality 门槛）；要到 **Pure（≥99.90%）需品质加成 ≥100**；100% 需 ≥200。
   - 移除顺序：普通杂质按 `particleSize` 从大到小；`mineral / chemical_contaminant / heavy_metal` 强制排在最后。
   - 被移除的杂质体积**折算成水**（总体积不变，所以纯度比例上升）。
2. **滤芯（WATER_FILTER）只按孔径 `FILTER_SIZE_TAG > particleSize` 判不截留**，且代码里**微生物一律放行**、耐久按 `filterVolumeRemoved/filterDurabilityUsage` 消耗；它无法去除 chemical_contaminant / heavy_metal（颗粒过小穿过）。
3. **紫外线灯（uv_filter）**：仅在“用 uv_filter 点容器”的转移过程中把 **microbe 全部转化为水**，无耐久、无耗电、可无限使用。
4. **净水片（water_tablet）不是直接调用 `OnWaterPurificationTabletAdded` 的旧代码**（该方法已死），而是**15ml 氯的可溶化学片**：每 tick 反应清 `chemical_contaminant`（保留 0.7% 体积地板）+ 灭微生物，**不补水、不去重金属**。
5. 六档纯度阈值：`9990 / 9900 / 9800 / 9600 / 9000 / 3500`（Pure / High Quality / Basewater / Ghostwater / Rustwater / Gutterflow）；**“达标”通常指 ≥9800（Basewater），商用/饮用安全线是 ≥9800**。
6. 原版**已有**“集水器/水龙头→容器→净化器→达标水”的链，也已有 **vending_fountain（售卖水机）**——它甚至能把**净化器机器本身当水源**（读净化器瓶槽）。**缺**的是“自动进/出料、自动灌装回瓶、自动交接给售卖机”的环节。

---

## 1. 净化器本体与工作周期

### 1.1 物品定义

| 项 | 值 | 证据 |
|---|---|---|
| 方法 | `MachinePurifier.WaterPurifier()` | `MachinePurifier.cs:30-112` |
| 单价 | `unitValue = 250` | `MachinePurifier.cs:46` |
| 贴图 | `Items/items_tool2` / `purifier`（插瓶后 `purifier_w`） | `MachinePurifier.cs:42,105,109` |
| 类型/标签 | `MACHINE`；`CONTAINER_TAG` + `manufacture` + `STANDARD_MACHINE_TAG` | `MachinePurifier.cs:48-51` |
| 机械初始化 | `MachineryHelper.InitMachinery(machine, 4)`（每夜 4 电） | `MachinePurifier.cs:52` |
| 品质比率 | `MACHINE_QUALITY_RATIO_INT = 2` | `MachinePurifier.cs:53-56` |
| 模块槽 | 只收 `MODULE_TYPE_UNIVERSAL` 且 owned | `MachinePurifier.cs:57-58` |
| 瓶槽 | `GameSlotInventory` 单槽，只收 owned + `LIQUID_CONTAINER_TAG` | `MachinePurifier.cs:40` |
| 便签槽 | 只收 `postit` | `MachinePurifier.cs:38-39` |

### 1.2 窗口结构

`MachinePurifier.CreateMachineInventoryWindow()`（`MachinePurifier.cs:143-150`）：
- 面板顺序：`Power` → `Module` → `Container` → `Note`（`:149`）
- 模块格：`GameGridInventory("10000010001000001110000010001000001", 7)`，7 列布局，共 **9 个可用格**（`:147`）
- 槽位索引（由 `MachineHelper.GetBodyStorage` 按面板顺序映射，`MachineHelper.cs:119-131`；`GetBatterySlot`=0 `:133`；模块=1 `:148-167`）：**0=电池、1=模块、2=瓶槽、3=便签**。

### 1.3 每夜流程（`onCycleEndSlotItemFunc`，`MachinePurifier.cs:79-102`）

1. 先把 `TOTAL_CONTAMINANT_REMOVED_TAG` 置 0（`:81-84`）。
2. 若瓶槽有物品且容器非空（`:85`）：
   - `machinePowerUsage = MachineryHelper.GetMachinePowerUsage(item)`（读 `machinery_drawn_power`，初始 4；`MachineryHelper.cs:194-197`）。
   - 电池槽有物品且 `PowerHelper.CanDrawPowerSource(battery, usage)` 为真（`:88`）→ `PurifyContainer(item, bottle)`（`:90`）。
   - **只有 `removedAmount > 0`** 才：扣电 `DrawPowerSource`、写回 TAG、`MachineHelper.OnMachineActioned(machine, moduleGrid)`（`:91-99`）。→ 已达标（无法再移除）时**不耗电**。
3. 插瓶/拔瓶时机器贴图在 `purifier` / `purifier_w` 间切换（`:103-110`）。

触发时机：在 `PlayerStore.EndNight` 的夜间序列里，对全部物品调 `onCycleEndSlotItemFunc`（`PlayerStore.cs:1655-1744`）；净化器位于「`RunNightlyChemicalSimulation()` 之后、`HopperHelper.RunWindow(1)` 之后的那个 all-items 循环」。即**每夜一次、每夜一个容器**。

### 1.4 耗电与电池

- `CanDrawPowerSource` / `DrawPowerSource` 只校验 `energy >= amount`，**不看 bandwidth**（`PowerHelper.cs:653-706`）。
- 电池（`MiscItemDirectory.cs`）：
  - `EnergyCredit()`：能量 100，带宽 10，价值 10（`:143-153`）→ 100/4 = **25 夜**。
  - `EnergyCreditExtended()`：250，价值 30（`:155-165`）→ **62 夜**。
  - `EnergyCreditBreeder()`：100 + 每夜自充 4（`InitRecharge(4)` + `onCycleEndEarlySlotItemFunc`），价值 400（`:167-192`）→ **自给自足**。
  - `DisposableBattery()`：1000，一次性，价值 0（`:194-206`）→ 250 夜。

---

## 2. 净化公式（核心）

### 2.1 入口 `PurifyContainer`（`MachinePurifier.cs:114-141`）

```
perf        = MachineryHelper.GetCurrentPerformanceBonus(machine)     // 百分比整数
bonusQuality= MachineryHelper.GetCurrentQualityBonus(machine)         // 百分比整数
V           = WaterHelper.GetTotalVolume(container)                   // µL
dirtBonus   = max(0, GetDirtBonus(purity) + perf/100) * 1.25
budget      = (long)((min(V, 10_000_000) * 3 / 1000 + 25000) * dirtBonus)   // µL
cap         = 10000 - round(100 * GetPurityMult(bonusQuality))        // “纯度上限”，万分比
removed     = RemoveLargestFirst(container, cap, budget)              // µL
WaterFeatureHelper.UpdateWaterFeature(container, isExpose: false)
return removed
```
证据：`MachinePurifier.cs:121-139`（`budget` 见 `:137`，`cap` 见 `:138`）。

要点：
- `V` 被夹到 **10L**：`BUDGET_VOLUME_CAP_UL = 10_000_000`（`:12,137`）。
- 固定项 **25ml**（`BUDGET_FLAT_UL=25000`，`:10`）、比例项 **3‰**（`BUDGET_VOLUME_PERMILLE=3`，`:8`）、总乘数 **1.25**（`REMOVAL_MULTIPLIER`，`:14`）。
- 结果用 `(int)` 截断（`:138` 返回 int）。

换算成 ml：`budget_ml = (V_ml × 0.003 + 25) × dirtBonus × 1.25`。

预算示例（`dirtBonus=1` 即纯度 ≥98% 时）：

| 容器 | V | budget（µL） | 折算 ml | 占体积 |
|---|---|---|---|---|
| 500ml 小瓶 | 500,000 | 33,125 | 33.1 | 6.6% |
| 1000ml | 1,000,000 | 36,250 | 36.3 | 3.6% |
| 3000ml 水壶(pitcher) | 3,000,000 | 42,500 | 42.5 | 1.4% |
| 6000ml 水罐(jug) | 6,000,000 | 53,750 | 53.8 | 0.9% |
| 20000ml 水箱 | 10,000,000（夹住） | 68,750 | 68.8 | — |

> 推算，按 §2.1 公式手算。**固定 25ml 对小容器占比极高**，所以小瓶更易单夜达标。

### 2.2 污浊度倍率 `GetDirtBonus`（`MachinePurifier.cs:193-210`）

| 当前纯度（万分比） | dirtBonus |
|---|---|
| ≥9800 | 1.0 |
| 9400 | 2.0 |
| 8850 | 5.0 |
| 8000 | 10.0 |
| 6250 | 15.0 |
| 3750 | 20.0 |
| <3750 | 20.0 |

- 曲线点之间**线性插值**（`:199-207`），低于最低点用最低点值（`:209`）。
- 曲线定义：`DIRT_BONUS_CURVE`（`:18-26`）。
- 性能加成：`dirtBonus += perf/100`（`:130`），并 clamp ≥0（`:131-134`）。即 **+100 性能 = 预算 ×2**；性能为负会削减预算（最低 0）。
- 注意连乘：`dirtBonus` 最终再 ×1.25（`:135`）。

### 2.3 品质上限 `GetMaxPurityCap` / `GetPurityMult`（`MachinePurifier.cs:164-191, 212-215`）

`cap = 10000 - round(100 × purityMult)`，purityMult 分档：

| 品质加成 bonusQuality | purityMult | cap（万分比） | cap（%） | 对应可达档位 |
|---|---|---|---|---|
| ≥200 | 0.0 | 10000 | 100.00% | Pure（含 100%） |
| ≥100 | 0.1 | 9990 | 99.90% | **Pure 门槛** |
| ≥75 | 0.2 | 9980 | 99.80% | Pure |
| ≥50 | 0.35 | 9965 | 99.65% | Pure |
| ≥25 | 0.65 | 9935 | 99.35% | High Quality |
| ≥15 | 0.85 | 9915 | 99.15% | High Quality |
| 其他（含 0） | 1.0 | 9900 | **99.00%** | High Quality（恰为门槛） |

- 纯度上限也写进 tooltip：`"Max Purity: {cap/100}%"`（`MachinePurifier.cs:285-298`；默认 fallback 文案 `mech_purifier_tooltip_max_purity`）。
- 只有品质加成能改 cap；性能/功率/速度加成都不改变 cap。
- **默认机器（0 品质）= 99.00%，到不了 Pure**。

### 2.4 移除顺序与数量 `RemoveLargestFirst`（`MachinePurifier.cs:217-283`）

```
num  = V * (10000 - cap) / 10000          // 允许残留的杂质体积
num2 = min(budget, V - water - num)       // 本夜还能移除的杂质体积
if num2 <= 0: return 0                    // 已达标，不耗电
```
证据：`:230-235`。

分组与顺序（`:236-248`）：
- key = `(REMOVED_LAST.Contains(id) ? 1_000_000 : 0) - particleSize`，用 `SortedDictionary` **升序**遍历 → 普通杂质按 `particleSize` 从大到小先移除；`mineral / chemical_contaminant / heavy_metal` 因 +1,000,000 排最后（`REMOVED_LAST`，`:28`）。
- 组内按量比例分摊（非最后一项 `int量×num4/num3`，最后一项取剩余），并即时写回标签（`:250-277`）。
- 所有移除量累加到 `water` 标签（`:278-281`）→ **总体积不变、纯度上升**。

按 `particleSize` 的移除优先级（正式版 Liquid 定义，见 §3.1 表）：
`physical_contaminant(100) → microbe(50) → organic_waste(40) → microplastic(25) →（最后组）mineral(30) → heavy_metal(12) → chemical_contaminant(2)`。

> 注意：`water` 本身 `canBeRemovedByPurifier=false`，不会被移除（`Liquid.cs` water 定义）。

### 2.5 单夜净化进度（推算示例）

以 6L 容器、无任何模块加成（cap=9900）、初始 Gutterflow 3500 为例，按 §2.1/§2.2 逐夜手算：

| 夜 | 起始纯度 | dirtBonus | 移除杂质 | 结束纯度 | 档位 |
|---|---|---|---|---|---|
| 1 | 3500 | 20.0 | 1.075L | 约 5292 | Ghostwater |
| 2 | 5292 | 16.92 | 0.909L | 约 6807 | Rustwater |
| 3 | 6807 | 13.41 | 0.721L | 约 8322 | Ghostwater |
| 4 | 8322 | 8.11 | 0.436L | 约 9048 | Rustwater |
| 5 | 9048 | 3.92 | 0.211L | 约 9400 | Ghostwater |
| 6 | 9400 | 2.0 | 0.108L | 约 9578 | Ghostwater |
| 7 | 9578 | 1.56 | 0.084L | 约 9718 | Ghostwater |
| 8 | 9718 | 1.21 | 0.065L | 约 9827 | **Basewater** |
| 9 | 9827 | 1.0 | 0.044L | 9900（cap） | **High Quality** |

> 上表为**推算**，用于量级判断：**6L 脏水需约 9 夜到默认上限**；而 500ml 小瓶在同样条件下 **1 夜即可到 99.00%**（budget 远大于杂质总量）。塔设计上“单夜达标”与“容器容量”强相关。

---

## 3. 加成来源：模块 / 品质 / 性能 / 功率

### 3.1 液体杂质体系（正式版 `Liquid.cs`）

| 液体 | particleSize | baseMaxPurityFromMachine | basePurifierRemoveAmount | filterMinimumPurity | filterDurabilityUsage | filterVolumeRemoved | canBeRemovedByPurifier |
|---|---|---|---|---|---|---|---|
| water | 10 | 0 | 0 | — | — | — | **false** |
| heavy_metal | 12 | 25 | 2000 | — | — | — | true |
| chemical_contaminant | 2 | 16 | 3000 | — | — | — | true |
| microbe | 50 | 9 | 5000 | — | — | — | true |
| organic_waste | 40 | 10 | 5000 | 0.0016 | 1 | 1000 | true |
| physical_contaminant | 100 | 7 | 5000 | 0.0016 | 2（默认） | 1500 | true |
| microplastic | 25 | 18 | 2500 | 0.002 | 2（默认） | 1500 | true |
| mineral | 30 | 7 | 5000 | 0.0016 | 1 | 2000 | true |
| debris / plant_matter / 菌类 | 200 | 0/12/12 | 6000/4000/4000 | — | — | — | true |
| alcohol | 10 | 12 | 4000 | — | — | — | true |
| chlorine | 5 | — | — | — | — | — | true |

证据：`Liquid.cs` 中各类静态字段定义（`water` 的 `canBeRemovedByPurifier=false`；`FILTER_SIZE_TAG` 常量同文件 `:121`）。

**关键**：`baseMaxPurityFromMachine` / `basePurifierRemoveAmount` 是**旧版公式字段，正式版净化器根本不用**（全文仅 `AquacultureHelper.cs:1803` 与 `PlantWaterQuality.cs:33,38` 使用）——见 §10 旧快照对比。正式版的净化完全由 §2 的“总体积预算 + 品质 cap + 粒子顺序”决定。

### 3.2 加成数值来源

- `MachineryHelper.GetCurrentPerformanceBonus`（`MachineryHelper.cs:537`）读 `TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT`；`GetCurrentQualityBonus`（`:532`）读 `TOTAL_PERCENTAGE_QUALITY_BONUS_INT`。
- 模块插拔回调（`MachinePurifier.cs:59-78`）→ `ModuleEffectHelper.OnAddedModuleToGrid`（`ModuleEffectHelper.cs:520`）/ `OnRemovedModuleFromGrid`（`:527`）→ `ModuleHelper.ComputeModuleEffect`（`ModuleHelper.cs:183,201`）/ `ApplyBasicModuleEffect`（`:235`）→ `MachineryHelper.UpdatePowerUsageProcessTypeMachine`（`MachineryHelper.cs:497`）。
- 模块触发 `onUsedAction` 由 `MachineHelper.OnMachineActioned`（`MachineHelper.cs:368`）在净化成功后被调用（`MachinePurifier.cs:98`）。

### 3.3 模块数值表（`ModuleDirectory.cs`，均 `MODULE_TYPE_UNIVERSAL`）

参数顺序：性能% / 效率% / 品质%。

| 模块 | 性能 | 效率 | 品质 | 价值 | 备注 |
|---|---|---|---|---|---|
| blank_module | 0 | 0 | 0 | 60 | — |
| system_performance_module | 16 | 0 | 0 | 70 | — |
| system_efficiency_module | 0 | 16 | 0 | 70 | — |
| system_quality_module | 0 | 0 | 8 | 70 | 8 品质/个 |
| system_overclock_module | 36 | -44 | 0 | 100 | 电耗风险 |
| system_eco_module | -16 | 32 | 0 | 100 | — |
| system_streamlining_module | 8 | 25 | -16 | 125 | 负品质 |
| system_fineness_module | -8 | -15 | **25** | 100 | 主品质模块 |
| system_ruined_module | 0 | 0 | 0 | 30 | — |
| system_corrupt_module | -8 | -5 | 0 | 45 | — |
| system_uncapped_neural_core_module | 200 | -100 | **100** | 1500 | 违禁品 tier4 |
| system_capped_neural_core_module | 100 | -100 | **50** | 250 | 违禁品 tier3 |
| equipment_performance_module | 30 | 0 | 0 | 100 | — |
| equipment_efficiency_module | 0 | 20 | 0 | 100 | — |
| equipment_overclock_module | 65 | -40 | 0 | 100 | — |
| equipment_eco_module | -30 | 40 | 0 | 100 | — |

- 到 **Pure（cap 9990）** 需要品质加成 ≥100：4× `system_fineness_module`（25×4=100）即可，但性能 -32、效率 -60；或 2× `capped_neural_core`（50×2=100）；或 1× `uncapped_neural_core`（100）但效率 -100。
- 到 **100%** 需 ≥200：2× uncapped 或 1×uncapped+2×capped 等。
- 品质加成是**直接读 raw 数值**；`MACHINE_QUALITY_RATIO_INT=2` 只用于其他机器的 `GetAjustedQuality` 路径，净化器不使用（`MachinePurifier.cs:121-122` 直读 `GetCurrentQualityBonus`）。

---

## 4. 滤芯系统 `WaterHelper.HandleFilter`（`WaterHelper.cs:333-395`）

转移液体时，目标容器若带 `FILTER_CONTAINER_TAG`，其子物品中的 `WATER_FILTER` 会被取为滤芯（`GetFilterItem`，`:318-331`）并逐液体过 `HandleFilter`（`TransferLiquid`，`:267-316`）。**被滤掉的部分从源容器扣，但不进入目标容器**（净损失/被截留）。

### 4.1 判定逻辑

1. `amount <= 0` → 0；`filter == null` → 原量（`:333-340`）。
2. `filter.identifier == "uv_filter"`：**microbe 返回 0（全部拦截）；其他原样**（`:341-348`）。
3. `filter.IsTag("WATER_FILTER")`：
   - 耐久 0 → 原量返回（`:351-356`）；
   - **microbe 一律原量返回（滤芯不去微生物）**（`:357-360`）；
   - 取 `filterSize = FILTER_SIZE_TAG`、`ps = liquid.particleSize`；**若 `filterSize > ps` → break（不过滤，原量）**（`:361-372`）；
   - `minRemain = amount × filterMinimumPurity`（该液体最小残留比例，`:373`）；
   - 循环：每轮 `耐久 -= filterDurabilityUsage`、`amount -= filterVolumeRemoved`，直到耐久 0 / amount≤minRemain / 10000 次（`:375-386`）；
   - `amount = clamp(amount, minRemain, 原量)`，返回 `max(0, amount)`（`:387-390`）。

### 4.2 孔径语义

`FILTER_SIZE_TAG > particleSize` ⇒ **不截留**。等价于“只截留 particleSize ≥ 孔径的颗粒”。

| 液体 | particleSize | size=22 滤芯(water_filter/adv/hydroreed) | size=150(kotton_fabric) |
|---|---|---|---|
| physical_contaminant | 100 | ✅ 截留 | ❌ 穿过(100<150) |
| debris/plant_matter/菌类 | 200 | ✅ 截留 | ✅ 截留 |
| organic_waste | 40 | ✅ 截留 | ❌ |
| microbe | 50 | ❌ 代码特例放行 | ❌ 特例放行 |
| mineral | 30 | ✅ 截留 | ❌ |
| microplastic | 25 | ✅ 截留 | ❌ |
| heavy_metal | 12 | ❌（12<22） | ❌ |
| water | 10 | ❌ | ❌ |
| chlorine | 5 | ❌ | ❌ |
| chemical_contaminant | 2 | ❌ | ❌ |

> ✅/❌ 为按公式推断（已确认公式，档位结论为**推算**）。**结论：滤芯只能处理“大颗粒”有机/物理/矿物/微塑料，无法去除微生物（特例）、重金属、化学污染物。**

### 4.3 滤芯物品数值（`AmenitiesItemDirectory.cs`）

| 物品 | 方法/行号 | 孔径 | 耐久 | 价值 | 特性 |
|---|---|---|---|---|---|
| water_filter | `WaterFilter()` `:427-445` | 22 | 400（`InitItemWithDurabilityPricingMode2(400,1)`） | 25 | 标准滤芯 |
| water_filter_adv | `WaterFilterAdvanced()` `:447-469` | 22 | 400 | 350 | `onCycleEndSlotItemFunc` 每晚 **+50 耐久**（`:465-468`），长期自维持 |
| hydroreed_filter | `HydroreedFilter()` `:529-547` | 22 | 200 | 20 | 便宜 |
| kotton_fabric | `KottonFabric()` `:407-425` | 150 | 1000 | 65 | 只挡超大颗粒（材质类） |

- 每个液体每轮消耗：`filterDurabilityUsage`（默认 2，organic/mineral 为 1），处理量 `filterVolumeRemoved`（1000/1500/2000 µL）。即 water_filter 400 耐久约可滤 **200L**（400/2 × 1L）——**滤芯本身不是瓶颈，孔径才是**。
- 滤芯容器：`water_pitcher`（3000ml、`isFilterContainer:true`、`isGraduated:true`）滤芯槽只收 `WATER_FILTER` 且 **identifier != "uv_filter"**（`AmenitiesItemDirectory.cs:549-565`，排除见 `:559`）。
- `TransferLiquid` 只在**转移**时过滤；容器静置不会自动过滤。

---

## 5. 紫外线灯 uv_filter

| 项 | 值 | 证据 |
|---|---|---|
| 定义 | `UVFilter()`，sprite `Items/items_amenities`/`uv`，type `WATER_PURIFICATION_SUPPLY`，厂商 davre，tag `WATER_FILTER`，价值 150 | `AmenitiesItemDirectory.cs:217-243` |
| 使用 | `mayItemTargetItemFunc`=目标非空且 `LIQUID_CONTAINER_TAG`；`onItemTargetItemFunc → WaterHelper.OnUVUsed(t)` | 同上 |
| 效果 | `OnUVUsed`：`microbe` 全部置 0，**等量转为 water**，`UpdateWaterFeature(isExpose:false)` | `WaterHelper.cs:838-851` |
| 消耗 | **无耐久、无次数、无耗电**（对比滤芯） | 已确认 |
| 限制 | 不能放进 pitcher 滤芯槽（`:559`）；只能“用物品点容器”时结算一次 | 已确认 |

- 语义：UV 只杀微生物，不改变其它杂质；把 microbe 体积折算成水，因此**纯度会小幅上升**。
- 对 chemical_contaminant / heavy_metal / mineral 无效。

---

## 6. 净水片（water_tablet）

### 6.1 物品定义（`AmenitiesItemDirectory.cs:357-371`）

| 项 | 值 |
|---|---|
| id | `water_tablet` |
| 类型 | `HOUSEHOLD_GOOD` + `WATER_PURIFICATION_SUPPLY` |
| 价值 | 6 |
| 标签 | `WATER_PURIFICATION_TABLET`（`:367`） |
| 化学 | `ChemicalHelper.InitChemItem(isBlendable:true, isSoluble:true)`（`:368`） |
| 内容物 | **15.0 ml chlorine** = 15000 µL（`:369`） |
| 药片盒 | `water_tablet_container`（1×9 格，只收 `WATER_PURIFICATION_TABLET`，`AmenitiesItemDirectory.cs:335-355`；白名单 `:350`） |

### 6.2 生效路径：可溶化学 → 容器每秒反应

1. 把药片“用”到液体容器上 → `ChemicalHelper.DissolveInto(chemicalItem, liquidContainer)`（`ChemicalHelper.cs:127-160`）：容量够则直接并入，不够先 `Remove` 等量液体；药片销毁；`UpdateWaterFeature`。
2. 液体容器每秒 tick `containerTempTick`（`ChemicalHelper.cs:24-28`）= 环境降温 + `HandleTemperatureReaction`（`:242-249`，容器非空时按固定顺序跑 23 个反应）。
3. 其中 `HandleChlorine`（`ChemicalHelper.cs:673-710`）：

```
num      = min(chlorine, 15000)                 // µL
num2     = num * 5.1                            // CHLORINE_POTENCY=5.1 反应力
num3     = (int)(totalVolume * 0.007)           // 化学污染物保留地板 = 0.7% 体积
num4     = chem > num3 ? min(chem - num3, num2) : 0
chem    -= num4
num2    -= num4
kill     = min(microbe, num2)
chlorineAfter = chlorine - num
selfDegraded  = min(chlorineAfter, max(1000, chlorineAfter * 0.2))
chlorine      = chlorineAfter - selfDegraded
microbe      -= kill
```
**被移除的 chemical_contaminant / microbe 不转化为水**（总体积下降，纯度比例上升）。

### 6.3 数值结论

- 15ml 药片在一次 tick 内即被消耗（`num=15000` → `chlorineAfter=0`），总反应力 **15000 × 5.1 = 76,500 µL**。
- 先清 `chemical_contaminant` 到 “0.7% × 体积” 地板，剩余反应力再灭 `microbe`。
- **不除** `heavy_metal`、`mineral`、`organic_waste`（与游戏内书 `StoreBookData.cs:30-42` 描述一致：“eliminate microbial and some chemical contaminants but leave behind heavy metals and organic waste”）。
- 不改变净化器品质 cap。
- **死代码警告**：`WaterHelper.OnWaterPurificationTabletAdded`（`:686-704`，microbe 移 75000 / chemical 移 10000）与 `OnWaterPurificationTabletAdded2`（`:666-684`）在正式版**全库无调用者**，是旧版残留，不可作为本版依据。

---

## 7. 纯度档位、显示与价格

### 7.1 六档阈值（`WaterFeatureHelper.cs:73` `WATER_PURITIES`）

| 档 | 阈值（万分比） | 显示精度范围（`WATER_VALUE_DISPLAY`，`:33-45`） | 判定函数 |
|---|---|---|---|
| Pure | ≥9990 | 99.9%+ | `GetPurityArrayIndex`：首个 `purity >= WATER_PURITIES[i]` 的 i（`:451-459`） |
| High Quality | ≥9900 | 99%–99.9% | 同上 |
| Basewater | ≥9800 | 98%–99% | 同上 |
| Ghostwater | ≥9600 | 96%–98% | 同上 |
| Rustwater | ≥9000 | 90%–96% | 同上 |
| Gutterflow | ≥3500 | 90%- | 同上 |
| 未知 | <3500 | — | `mech_unknown_water`（`:343-353`） |

- 纯度计算：`GetWaterPurity = water × 10000 / total`（`WaterHelper.cs:557-574`）。
- 饮用水安全线：`IsLabelledHazardousWater` 以 **<9800** 为危险（WaterFeatureHelper）。
- 被测出的标签：`GetWaterDisplayName` 同档（`:343-353`）。

### 7.2 价格：与水纯度**不直接**挂钩（基价恒定 40/L）

- `GetWaterPrice(int totalLiquidParts)`（`WaterFeatureHelper.cs:369-377`）：
  ```
  int num = 40;
  return Round(40 * totalLiquidParts / 1e6);   // µL → 升
  ```
  ⇒ 容器 `unitValue` 恒为 **40 信用点/升**，与纯度无关。
- `WATER_PRICES = {200,100,40,27,20,10}`（`:59`）与 `PURE/HQ/BASE/GHOST/RUST/GUTTERFLOW_WATER_PRICE_PER_LITER`（`:47-57`）在正式版**无读取点**（grep 只命中定义），是旧设计残留（**待验证**：是否有 mod/未来版本使用）。
- 纯度通过 **ItemCondition 的 `modValue`** 参与最终售价（`ItemConditionList.cs:325-395`）：

| 档 | condition identifier | customValue | modValue |
|---|---|---|---|
| Pure | `PURE_WATER_DISPLAY` | 9990 | **+100** |
| High Quality | `HIGH_QUALITY_WATER_DISPLAY` | 9900 | +50 |
| Basewater | `BASE_WATER_DISPLAY` | 9800 | 0 |
| Ghostwater | `GHOSTWATER_DISPLAY` | 9600 | -33 |
| Rustwater | `RUST_WATER_DISPLAY` | 9000 | -75 |
| Gutterflow | `GUTTERFLOW_WATER_DISPLAY` | 3500 | -90 |

- `ItemFeature.GetCurrentValueMod()` = exposed 时 `realCondition.modValue`，否则 `fakeCondition.modValue`（`ItemFeature.cs:380-399`）；`GameItem` 价值汇总（`GameItem.cs:1661-1688`）按 `ValueStage` 处理：Market 累加、Final 乘 `max(0,1+mod/100)`、其余 innate 累加。
- ⇒ 实际水价 ≈ `40/L × (1 + 纯度档修正)`（例如 Pure≈80/L、HQ≈60/L、Base≈40/L、Ghost≈26.8/L、Rust≈10/L）；**喷泉售卖**还会叠加 `retailMarkup`（默认 15，难度 0–20，`PlayerStore.cs:97`、`Difficulty.cs:210/252/294/336`）与 `VendingMachineBonus(+10)`（`ItemFeatureList.cs:184-196`）。
- 售卖机 `GetCurrentValue(useRetailMarkup:true, ..., forceMarkup:true)`（`VendingFountainHelper.cs:287`）。

### 7.3 净化后的显示更新

`UpdateWaterFeature(gameItem, isExpose:false)`（`WaterFeatureHelper.cs:227-264`）：
- 更新 `actualDisplay` 为“档名 (范围 Purity)”（`:239`）；
- `customIntValue1`（玩家上次记录/假标签）**仅当 `isExpose=true` 才更新**（`:235-238`）；
- `isFeatureMatch` 依据 `customIntValue1 == 实际纯度` 判定；不匹配时 `realCondition` 更新为实际档（`:240-249`）；
- `isExpose=false` 时禁用 `STRIP_TESTED_TAG`（`:252-256`）。
⇒ **净化后若容器从未被暴露/试纸检测，玩家看到的仍是旧假标签**；实际纯度已变，被检测时才更新（`SetPresumedPurity` 也可改假标，`:266-275`）。

---

## 8. 端到端“产水路线”（原版现状）

### 8.1 脏水来源

| 来源 | 机制 | 证据 |
|---|---|---|
| 水龙头 faucet（固定家具） | 用容器点水龙头 → `AddWater(grade=1, useRange:true, 今日水质)`；需先解锁网络升级 | `WaterHelper.cs:96-165`；`MiscItemDirectory.cs:1012-1033`（`faucet`/`drain`，`IMPORTANT_TAG` 不可搬） |
| 网络升级 | `GUTTERFLOW_TAP_COST=250`、`RUSTWATER_TAP_COST=350`（rust 需先买 gutter） | `NetworkUpgradeList.cs:56,58,842` |
| 每日水质 | Gutterflow 8000–8999；Rustwater 9000–9150，每 StartDay 重掷 | `StoreStation.cs:25-70` |
| 集水器 MachineMoistureFarm | 每夜产 `850ml × (100+性能)/100`；水质由 `品质加成+50` 分档（0 加成→Ghostwater）；多台共享空气 ×1/(1+0.25(n-1))；耗电 6 | `MachineMoistureFarm.cs`（`BASE_OUTPUT_ML=850`、`GetOutputQuality`、`ApplySharedAir`、`InitMachinery(6)`） |
| 直接生成的水容器 | `WaterHelper.FillWithPureWater/HighQuality/Base/Ghost/Rust/Gutterflow`（测试/生成用） | `WaterHelper.cs` |

### 8.2 装水 → 净化 → 达标

1. **装水**：容器点水龙头 → `AddWater`（按档位区间随机纯度 + 污染分布）；或集水器直接灌入瓶槽容器。
2. **预过滤（可选）**：把脏水倒进带滤芯的 `water_pitcher`（3000ml）→ `TransferLiquid` 按孔径截留大颗粒（§4）；或对容器用 uv_filter 灭微生物（§5）。
3. **净化**：容器放进净化器瓶槽 → **每夜 1 次**按 §2 公式提升纯度，直到 cap。
4. **达标判定**：实际纯度（`GetWaterPurity`）≥9800 = Basewater（安全线）；≥9900 = HQ；≥9990 = Pure。cap 由模块决定（§2.3），**没有模块只能到 99.00%**。
5. **便携净化器**：`PortableWaterPurifier`（价值 325，耐久 300）用一次直接把容器净化到 **98.00%（Basewater）**；费用 = `ceil(总µL/100000)` 耐久（每 100ml 1 点）（`AmenitiesItemDirectory.cs:472-519`）——不达标 Pure，但可作“快速达标 Basewater”手段。

### 8.3 灌装与售卖：vending_fountain（原版已有）

`VendingFountainHelper.cs`：
- 由网络升级 `VENDING_FOUNTAIN` 解锁，生成 `vending_fountain` + core（`IMPORTANT_TAG` 不可搬）（`:11-25`）；机器耗电 2（吸引力升级每 2 级 +1，`:305-317`）。
- 窗口（`:141`）：Power / base_water / hq_water / pure_water / Control / core。
- **水源槽接受两种东西**（`MayAcceptWaterSource` `:157-169`）：
  1. 带水的液体容器；
  2. **净化器或集水器机器本身**（`IsWaterSourceMachine` `:144-155`，identifier == `water_purifier` / `moisture_farm`）→ 销售时读取该机器 **slot 2（瓶槽）里的容器**（`GetWaterSource` `:171-183`）。
- 每夜 `onCycleEndLateSlotItemFunc` → `HandleNight`（`:195-267`），按 `VENDING_ATTRACTIVENESS`（初始 15）卖不同瓶型：
  - base 槽：750ml `bottled_water`，标签 **Basewater**（`:199`）；
  - HQ 槽：450ml `small_bottled_water`，标签 **High Quality**（`:200`）；
  - Pure 槽：吸引度 ≥20 才开卖，瓶型随吸引度升级（`:201-217`）；
  - 吸引力 ≥30/40/50/70/80/90/100 追加多笔 base/HQ 销量（`:218-265`）。
- `SimulateWaterSale`（`:269-303`）：取源容器**实际纯度** → 若档位是 rust/gutterflow **不卖**（`:276-278`）；否则 Spawn 新瓶 → `TransferLiquid` 从源抽水 → **强制贴 `fakeCondition = conditionWaterPurity[labelPurity]` 假标签**（`:283`）→ `OnWaterLabelUpdated` → `VendingMachineBonus` → 估值计入 `VENDING_CASH`（`:287-300`）。
- **伪标会被记录**：`SecData.GetMislabeledWaterWeight`（`SecData.cs:687,703`）与 `VendingMachineHelper.TrackMislabelledFountainSale`（`VendingFountainHelper.cs:289-293`）→ 犯罪/风险。
- 结论：**原版把“水”从净化器瓶槽自动卖出去是可行的（喷泉当水源）**；但喷泉只“抽水卖瓶”，**不会把水灌回玩家容器**，也不要求实际纯度达标（只拒绝 rust/gutterflow）——实际 <9800 会被伪标成 Basewater，属违法行为。

### 8.4 原版有 / 没有

| 环节 | 原版 | 说明 |
|---|---|---|
| 脏水源（水龙头/集水器/水罐） | ✅ | 需网络升级/机器 |
| 容器→净化器入料 | ⚠️ 手动 | 瓶槽单槽，只收 `LIQUID_CONTAINER_TAG` + owned |
| 净化器自动出料 | ❌ | 无出料逻辑；只能玩家拔瓶 |
| 单夜多容器 | ❌ | 每夜只处理瓶槽 1 个容器 |
| 达标判定（≥9800/9900/9990） | ✅ | 纯度+档位函数齐备 |
| 自动灌装回瓶 | ❌ | 喷泉内部 Spawn 瓶后销毁，不返还容器 |
| 自动售卖 | ✅（喷泉） | 可把净化器机器本身当水源，每夜自动卖 |
| 伪标/犯罪风险 | ✅ | 卖实际未达标水会被记录 |
| 自动“接水→净化→达标→灌装→售卖”全链 | ❌ | 缺自动进料/出料/灌装三段 |

---

## 9. 对「净化塔」设计最关键的 3 个约束

1. **节拍约束：净化器 = 每夜 1 容器 + 4 电，且无自动进/出料。**
   瓶槽是 `GameSlotInventory` 单槽（`MachinePurifier.cs:40`），`onCycleEndSlotItemFunc` 只处理它（`:79-102`）；没有 hopper 式输入输出。任何“自动接水→净化”的塔都必须**补“换瓶/上料”逻辑**，否则整塔吞吐 = 每夜 1 个容器。反过来说，**容器可以长期留在瓶槽里无人值守逐夜变纯**（`removed>0` 才耗电），这让“慢速自动净化”天然成立。
2. **品质上限约束：默认 99.00%，Pure(≥9990) 需品质加成 ≥100，100% 需 ≥200。**
   cap 只由模块品质加成决定（`MachinePurifier.cs:164-191,212-215`）；性能只影响单夜预算（`perf/100` 加进 dirtBonus，`:130`）。“达标”目标决定模块配置：Basewater(9800)/HQ(9900) 无需模块；Pure 必须 4× `system_fineness_module` 或 2× `capped_neural_core`/1× `uncapped_neural_core`，代价是性能 -8～-32、效率 -60～-100（`ModuleDirectory.cs`）。
3. **售卖接口约束：原版现成的“净化→售卖”桥梁是 vending_fountain 的机器水源槽，但它只卖不灌、且贴假标。**
   喷泉可直接读取净化器瓶槽（`VendingFountainHelper.cs:144-183`），每夜按吸引度自动卖瓶并累积现金——这是唯一不需要 mod 的自动化段；但它：① 不校验实际纯度（只排除 rust/gutterflow），实际<9800 会伪标成 Base/HQ/Pure 并被记录为 mislabeled（`SecData.cs:703`、`VendingFountainHelper.cs:289-293`）；② 每次卖水都 Spawn 新瓶后销毁，**不返还容器**，源容器逐步卖空；③ Pure 槽需吸引度 ≥20 才开卖（`:201-217`）。因此“达标→灌装→售卖”若要合法，塔必须在灌装/标签环节自己保证实际纯度达标，或接受犯罪风险。

---

## 10. 与旧快照 `water-research\`（2026-10-28）交叉核对

> 旧快照只能用于对照，**数值与公式不得直接引用**。

| 项 | 旧快照（2026-10-28） | 正式版（buildid 26337366） | 差异影响 |
|---|---|---|---|
| 净化公式 | 逐液体 `basePurifierRemoveAmount × 倍率` + 分段污浊倍率（<5000 ×20 / <7500 ×15 / <8500 ×10 / <9200 ×5 / <9600 ×2）+ 大体积加成（>3L 加 0.05–0.35）+ ×1.25，走 `RemoveContaminantFromContainer` | **总体积预算**（§2.1 `budget`）+ 品质 cap + 粒子顺序移除（`RemoveLargestFirst`） | 旧“每液体基础移除量”口径作废；正式版更看重**总液量/污浊度**，小容器反而更快 |
| `baseMaxPurityFromMachine`/`basePurifierRemoveAmount` | 核心公式参数 | 净化器**不再使用**（仅 AquacultureHelper/PlantWaterQuality 用） | 旧结论不可套用 |
| 每夜扣电时机 | 先 `DrawPowerSource` 再净化，无论是否移除成功 | 仅 `removed>0` 才扣电/写 tag/触发模块 | 正式版更省电 |
| 机器单价 | 300 | 250 | — |
| `GetPurityMult` / cap 表 | 相同（200/100/75/50/25/15 → cap 10000/9990/…/9900） | 相同 | 可直接沿用 |
| `HandleFilter` | 与正式版逐行一致（`water-research\WaterHelper.cs:331-393`） | 一致（§4） | 滤芯结论可沿用 |
| 净水片旧方法 `OnWaterPurificationTabletAdded` | 旧版已有“microbe-75000 / chemical-10000”实现 | 正式版保留方法但**无调用者**，改为氯化学片 | 旧净水片数值作废 |
| `GetWaterPrice` | 疑似同为硬编码 40/L（行号 369-377 与正式版一致，未逐行核对→**待验证**） | 40/L 恒定（`:369-377`） | 需注意售卖价与纯度关系 |

---

## 11. 待验证清单（不阻塞当前设计）

1. 喷泉 `retailMarkup` 在四档难度中的映射（默认 15；难度文件给 20/15/0/0，未确认哪档对应哪个）。
2. 净化器瓶槽/容器内容物是否会被 hopper 类家具自动搬运（当前判断：内容物窗口不参与 hopper graph，**待验证**；若是则自动上料可行性大幅提高）。
3. `MACHINE_QUALITY_RATIO_INT=2` 在净化器上的确切用途（当前只确认 `GetAjustedQuality` 使用它，净化器路径未使用）。
4. `WATER_PRICES` 数组是否有 UI/任务/客户报价读取点（grep 未发现，标注残留）。
5. 旧快照 `GetWaterPrice` 是否与正式版逐行相同（仅行号一致，未逐行核对）。
6. 净化后未暴露容器的 UI 展示细节（`publicDisplay` 是否仍显示旧假标签的完整路径，`WaterFeatureHelper.cs:227-264` 已确认不更新 `customIntValue1`，但显示层可能有其他刷新）。
7. `OnWaterPurificationTabletAdded/2` 是否被反射/事件字符串调用（当前 grep 无调用者，判定死代码）。
