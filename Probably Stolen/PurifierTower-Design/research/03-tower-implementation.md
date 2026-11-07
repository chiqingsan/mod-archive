# 03 净水塔：可落地实现路径与风险（正式版 buildid 26337366）

> 任务：为《深空当铺》正式版「净水塔」设计文档沉淀可落地的实现路径与风险；只做技术调研与方案证据，**不含实现代码**。
> 权威源码：`D:\code\mod\.tmp-decompile-current\Assembly-CSharp\`（935 个 .cs，快照 2026-10-31 21:53，正式版 buildid 26337366）。
> 现成资产：`D:\code\mod\Probably Stolen\WaterTower-Final\`（水塔 mod 1.0.8，26 个反编译 .cs/7033 行 + 官方反编译快照）、`D:\code\mod\Probably Stolen\CollectorTower-Design\`（上一轮集水塔设计，0 Harmony 路线）。
> 证据标注：**已确认(file:line)** = 本快照中读到实现；**待验证** = 需实机/M0 探针；**风险** = 已知失败模式或设计隐患。
> 边界：`research/01-purify.md`（task-1 净化机制）、`research/02-bottling-selling.md`（task-2 灌装售卖）由其他成员负责，本文只写支撑设计所需的实现路径与证据，不改动这两个文件。

---

## 1. 结论摘要（可直接引用）

### 1.1 0 Harmony 可行性结论：**本质可行；目标 0 处补丁，最坏 1 处兜底**

净水塔需要的全部介入面都有官方公开扩展点，不需要 Harmony：

| 介入面 | 0 Harmony 手段 | 证据 |
|---|---|---|
| 物品注册 | 自建 `ModItemDirectory` 实例触发 `OnModItemDirectoryInit` 后 `Directory.Add` | `Directory.cs:7-21`、`DirectoryMaster.cs:21-42`、`ModItemDirectory.cs:5`、`ModHook.cs:10`（机制已确认；触发待 M0） |
| 机器窗口/面板 | `MachineHelper.CreateMachineryWindow / Panel / PowerPanel / NotePanel`（全 public static） | `MachineHelper.cs:56-97` |
| 机器周期/夜结 | 物品自身 `onCycleEndSlotItemFunc` / `onCycleEndLateSlotItemFunc`，原版 `EndNight` 自动遍历调用 | `PlayerStore.cs:1720-1729`、`GameItemFunc.cs:90-94`（已确认） |
| 夜结兜底/前后时点 | `ModHook.OnGoingSleepEarly/Late` | `PlayerStore.cs:1664/1743`、`ModHook.cs:38/40`（已确认） |
| 倒水/灌装 | `WaterHelper.TransferLiquid` + `FoodItemDirectory` 瓶工厂 | `WaterHelper.cs:267-317`、`FoodItemDirectory.cs:164-173/303/313`（已确认） |
| 自动售卖 | `VendingFountainHelper.SimulateWaterSale / HandleNight`（public static） | `VendingFountainHelper.cs:269/195`（已确认） |
| 现金入账 | `VendingMachineHelper.TransferCashToPlayer` → `PlayerStore.ModCash` | `VendingMachineHelper.cs:511`、`PlayerStore.cs:3718`（已确认） |
| 上架获取 | `ModHook.OnPlaceInventorInventoryItemLate` + `PlayerStore.AddDirectSellingItemToTable` | `StoreClientList.cs:553/1128`、`PlayerStore.cs:2484`（已确认） |
| 存档 tag/迁移 | `SaveItemNode.itemState` 自动随 `GameItem` 的 tag 存；解码走 `DirectoryMaster.Item(identifier)` | `SaveManager.cs:149-246`、`DirectoryMaster.cs:252-286`（已确认） |

**唯一未决点**：`ModItemDirectory` 在游戏里只有"消费方"（`PaperBookRegistry.cs:585-608` 订阅事件并注册），全工程 grep 不到任何创建该目录实例的代码（已确认，见 §8.1）。因此 0 Harmony 方案依赖"自建 `new GameObject + AddComponent<ModItemDirectory>()` 能同步触发 `Awake` 与事件"——`AddComponent` 会同步调用 `Awake` 是 Unity 语义，但该目录的 `Awake` 上挂着 `[RuntimeInitializeOnLoadMethod(SubsystemRegistration)]`（实例方法，是否另被 Unity 自动调用存疑），必须 M0 探针实测。
**退路**：若自建不触发，退化 1 处 Harmony Postfix（`FurnitureItemDirectory.InitDirectory` 或 `ModItemDirectory.InitDirectory`）；水塔 1.0.8 已用 `FoodItemDirectory.InitDirectory` Postfix 实机验证过（`WaterTower-Final/decompiled/WaterTower/*Patch.cs`）。即便退化，也远优于水塔 1.0.8 的 13 处补丁。

### 1.2 复用原版净化器逻辑的最大障碍：不是算法，而是"机器外壳 + 标签合规"

- **算法可直接复用**：`MachinePurifier.PurifyContainer(machine, waterContainer, ignoreBonus)` 是 `public static`（`MachinePurifier.cs:114-141`），仅要求容器带 `LIQUID_CONTAINER_TAG`（`:116`）；`machine` 参数**只**用于读性能/品质加成（`:121-122`），不校验机器类型、电量、窗口、是否在店内。塔的蓄水箱是液体容器时可直接净化。
- **障碍一：加成来源必须是"机器"**。加成读取 `MachineryHelper.GetCurrentPerformanceBonus`（`MachineryHelper.cs:537-540`，读 tag `TOTAL_PERCENTAGE_PERFORMANCE_BONUS_INT`）与 `GetCurrentQualityBonus`（`:532`）。塔若不建立完整机器骨架（`InitMachinery`、电池槽、模块槽、模块回调），加成为 0，净化预算/纯度上限永久最差档。
- **障碍二：净化函数不负责"标签/定价"**。`PurifyContainer` 内部只调 `WaterFeatureHelper.UpdateWaterFeature(container, false)`（`:139`）刷新 feature，不保证玩家看到的标签与实际纯度一致；塔必须在每次净化/灌装后按**实际**纯度重贴标签（`WaterFeatureHelper.UpdateWaterFeature:227` / `OnWaterLabelUpdated:568`），否则触发假标签统计（`SecData.GetMislabeledWaterWeight:703`，即 VendingFountain 的 mislabeled 链）与脏水检查（`PlayerStore.PerformOnSoldCheck:2718-2737`）。
- **障碍三：UI 外壳要自己搭**。原版净化器窗口 = 电板 + 模块面板（7 格 bay）+ 容器槽 + note（`MachinePurifier.cs:143-150`），塔要复刻的是这套窗口/存档语义（读者=机器物品自身），而水塔 1.0.8 是自建 `new PixelWindow(true,"水塔")`（`WaterTower-Final/decompiled/WaterTower/WaterTowerWindow.cs:145`）并用反射取电池槽（`NightSettlement.cs:553/725/1176` 的 `CallStatic(...,"GetBatterySlot",machine)`）——净水塔若改走 `MachineHelper` 工厂路线属于新路径，签名已确认 public，但"自定义物品内是否正常"需 M0。
- **产能还需设计旋钮**：单次净化预算很小且被纯度曲线反向惩罚（`MachinePurifier.cs:137`，详见 §4.2），纯水阶段几乎只能微调；"塔的日处理量"事实上等于 `调用次数 × 单次预算`，必须由 mod 决定调用次数（模块/品质加成可作为旋钮），不能指望原版一次调用就够。

### 1.3 自动售卖挂载点结论：**有，0 Harmony**；柜台/展示柜没有 mod 可用的自动成交点

- **可行**：`VendingFountainHelper.SimulateWaterSale(labelPurity, containerID, waterInv, vendingMachine)` 是 `public static`（`VendingFountainHelper.cs:269-303`）。它内部已完成"造瓶 → `TransferLiquid` 从水源槽取水 → 贴纯度标签特征 → 零售估值 → `PerformOnSoldCheck` → 销毁物品 → `VENDING_CASH` 累加 → `RunStats.OnFountainSale`"整条链，塔只需传入自己的液体槽与塔物品即可模拟成交。多槽批量还可复用 `HandleNight(machine, baseInv, hqInv, pureInv)`（`:195`，public static）。
- **现金**：`VendingMachineHelper.TransferCashToPlayer(machine)`（`VendingMachineHelper.cs:511-522`，public static）读机器的 `VENDING_CASH` tag → `PlayerStore.ModCash`（`PlayerStore.cs:3718`）→ tag 清零。塔自带 `VENDING_CASH` 即可每夜入账。
- **不可行（需明确排除）**：
  - 普通售货机 `vending_machine`：白名单排除 `LIQUID_CONTAINER_TAG`（`VendingMachineHelper.cs:78-167`），瓶装水进不去。
  - 柜台：`frontInvinvElement` 是"客户台"（客户主动卖给玩家，`AddDirectSellingItemToTable:2484`），`backInvinvElement` 是"加权台"（玩家摆货等客户买，`AddDirectToWeightedTable:2541`）；两者都只是摆货，成交由白天顾客流（`StoreClient`）驱动，**无 mod 可挂的自动成交回调**（已确认：成交链 `SellItem:2676` / `BuyItem:2613` 内部无 ModHook 事件、无自定义回调；且 `SellItem` 需要 `currentClientInstance`，夜结时为 null 会 NRE，不能用于夜间自动售卖）。
  - 展示柜：同样无自动成交，靠 `ShowcaseHelper.HandleDisplayCase:362` 生成顾客白天上门（水客户阈值见 `:475-499`）——只能作"间接自动售卖"，依赖客流，不满足"无人值守"。

---

## 2. 水塔 1.0.8 已验证扩展点 → 净水塔 0 Harmony 映射

水塔 1.0.8 共 13 处 Harmony（`WaterTower-Final/decompiled/WaterTower/`，详见 `CollectorTower-Design/research/01-watertower-reference.md`）。下表逐项给出"它证明了什么能力可用"以及"净水塔能否用官方扩展点替代"。

| # | 能力 | 水塔 1.0.8 做法（已实机验证） | 净水塔 0 Harmony 替代 | 状态 |
|---|---|---|---|---|
| 1 | 物品/机器注册 | Postfix `FoodItemDirectory.InitDirectory` + `DirectoryMaster.Item` 强塞 | 自建 `ModItemDirectory` + 订阅 `OnModItemDirectoryInit` 后 `dir.Add(id, factory)` | 机制已确认；**触发待 M0** |
| 2 | 机器窗口 | 自建 `new PixelWindow(true,"水塔")`（`WaterTowerWindow.cs:145`），反射 `GetBatterySlot` 取槽 | `MachineHelper.CreateMachineryWindow`（`MachineHelper.cs:56-61`）+ `Panel/PowerPanel/NotePanel`（`:70-97`），`GetBatterySlot:133`、`GetModuleInv:164` | 签名已确认；**自定义物品内待 M0** |
| 3 | 夜间补产/结算 | Postfix `PlayerStore.EndNight` + 3 秒去重（`NightSettlement.cs:66-86`） | ①物品自身 `onCycleEndSlotItemFunc` / `onCycleEndLateSlotItemFunc`（原版 EndNight 自动遍历，`PlayerStore.cs:1720-1729`）；②兜底 `ModHook.OnGoingSleepLate`（`:1743`，夜结最后一步） | **已确认** |
| 4 | 状态持久化 | tag + 补丁探针 | `SaveItemNode.itemState` 自动保存 `GameItem` 的 tag（`SaveManager.cs:114-246`）；解码按 `identifier` 走工厂（`DirectoryMaster.cs:252-286`） | **已确认，无需补丁** |
| 5 | 存档迁移 | Prefix `SaveManager.DecodeNodes` 改写旧档槽索引（`SaveLayoutMigration.cs:22-34/52-60`） | 0 Harmony 不可行 → 采用"布局 append-only + 版本 tag + onLoaded 自愈"（限制见 §11） | **风险** |
| 6 | 拖拽倒水/灌装 | 反射 + 自定义 target 规则 | `WaterHelper.TransferLiquid`（`:267`，双方需 `LIQUID_CONTAINER_TAG`）；拖拽目标判定已被 `InitLiquidContainerItem` 内置（`WaterHelper.cs:96-165`：容器互倒 / drain / faucet） | **已确认** |
| 7 | 自动售卖 | 水塔不卖水（无此能力） | `VendingFountainHelper.SimulateWaterSale:269` / `HandleNight:195`（public static） | **已确认** |
| 8 | 入账 | — | `VendingMachineHelper.TransferCashToPlayer:511` → `PlayerStore.ModCash:3718` | **已确认** |
| 9 | 上架获取（杰克逊） | Postfix `PlaceInventorInventory` | `ModHook.OnPlaceInventorInventoryItemLate`（`StoreClientList.cs:553/1128`）+ `AddDirectSellingItemToTable:2484` | **已确认** |
| 10 | 按钮/UI | Postfix `ButtonElement.OnSelect` 拦点击 | `PixelButton.SetCallback` + `PixelWindow.openCallback/closeCallback`（`PixelWindow.cs:128/130`） | **已确认** |
| 11 | 自定义贴图 | Postfix `GameItemElement.ResolveSpriteByName`、`SpriteHelper.UpdateWaterContainerSprite` | `SetSpriteAndShapeFromMod`（mod 自带 sprite 加载）；水位换图用槽回调 + `SetSpriteAndShape` | **待验证（mod 资源路径）** |
| 12 | 拖拽目标判定 | Postfix `GameItem.MayTarget/CanTarget/Target` | 物品自身 func 字段可赋值（`InitLiquidContainerItem` 已内置容器互倒规则） | **已确认** |
| 13 | 购买特判/工具 | Postfix `BuyItemFromTable`、`ToolDirectory.CreateTurboBooster` | 本设计用不到；如需要，`AddDirectSellingItemToTable` 可先做前置校验 | 不需要 |

**要点**：水塔 1.0.8 之所以需要 13 处补丁，主要是因为它的技术选型是"绕过官方机器工厂自建窗口 + 反射取槽 + 强塞目录"，并且需求包含贴图/按钮/购买特判。净水塔若走"官方机器骨架 + 官方销售链"，同样需求可以用 public API 满足；补丁只在"自建目录触发失败"时才需要（1 处）。

---

## 3. 系统架构候选（A/B/C）

### 方案 A：单物品自建机器（推荐）

塔 = 一个自定义机器物品；窗口 = `MachineHelper.CreateMachineryWindow` 组合：
- 电源面板 `PowerPanel`（电池槽 `SetupBatterySlot`）
- 模块面板（7 格 bay，`GameGridInventory` 配置串 `10000010001000001110000010001000001`，`MachineHelper.DEFAULT_MODULE_BAY_CONFIG`）
- 进水槽（`GameSlotInventory`，白名单：owned + 液体容器）
- 蓄水箱：窗口内**锁定槽**中的液体容器子物品（容量由自定义 tank 物品决定，参考水塔 600 L）
- 成品槽（如需自产自卖以外的玩法）
- 按钮面板：`巡检`、`净化`、`取现`（也可全自动无按钮）

周期：`onCycleEndSlotItemFunc` 做"接水 + 净化"，`onCycleEndLateSlotItemFunc` 做"灌装 + 售卖"（都在原版 EndNight 内自动触发，顺序在集水器/净化器之后）。作者已确认这两个委托是 `Action<GameItem,GameInventory,SlotMarker>` 属性（`GameItemFunc.cs:90-94`），并且 `FindAllItem` 会扫到店内任何 owned 物品（`PlayerStore.cs:1746-1768`）。

- 优点：单物品、单窗口、单存档链；全部用官方 API；性能=每夜一次 O(全店物品) 遍历；无 Harmony 依赖。
- 缺点：塔的"机器骨架"要自己搭全（模块/电池/加成），UI 工作量大；自定义 tank 子物品的存档/解锁细节需 M0 验证。

### 方案 B：塔内嵌原版净化器/售水机（复用优先，仅作对照）

窗口内放原版 `water_purifier` 子物品（`FurnitureItemDirectory.cs:23` 已注册，可 `DirectoryMaster.Item` 生成）和/或 `vending_fountain`（`:33`），靠原版各自周期自动运行，塔只负责供水/取现。

- 优点：净化/售水逻辑 0 复用成本；原版 UI 直接呈现。
- 缺点：双层窗口（塔窗口里再开机器窗口）；玩家可能拆走子物品（需锁槽 + `mayInventoryRemoveItemFunc`）；`GraphUtils.FindAllChildrenType` 会递归发现嵌套机器（`:593-630`），夜结顺序取决于父/子周期，调试复杂；存档嵌套层级深；`DirectoryMaster.Item` 会**销毁工厂产生的子物品**（`DirectoryMaster.cs:252-286`），塔工厂不能直接塞子机器，只能在放置/onLoaded 后装配。
- 结论：作为 M2/M3 的**对照实验**，不作为正式形态。

### 方案 C：混合（A 为主 + 官方售水链）

= 方案 A 的机器骨架，但"卖水"不自己实现，直接调 `VendingFountainHelper.SimulateWaterSale`（public static）。这是推荐落地形态：净化复用 `MachinePurifier.PurifyContainer`，售卖复用 `VendingFountainHelper`，塔只贡献"水从哪来、卖多少、标签是否合规"。

| 维度 | A | B | C |
|---|---|---|---|
| 0 Harmony | 0（最坏 1） | 0 | 0（最坏 1） |
| 复用净化器 | 调 PurifyContainer | 原版机器自动 | 同 A |
| 复用售卖 | 调 SimulateWaterSale | 原版售水机自动 | 同 A |
| 存档复杂度 | 中 | 高（嵌套解析失败风险） | 中 |
| UI 工作量 | 高 | 低-中（双层窗口） | 高 |
| 可维护/可控 | 高 | 低 | 高 |

---

## 4. 复用原版净化器逻辑的可行性

### 4.1 入口与语义（已确认）

| 项 | 内容 | 证据 |
|---|---|---|
| 入口 | `public static int PurifyContainer(GameItem machine, GameItem waterContainer, bool ignoreBonus = false)` | `MachinePurifier.cs:114` |
| 前置 | `waterContainer.IsTag("LIQUID_CONTAINER_TAG")`，否则返回 0 | `:116` |
| 加成 | `GetCurrentPerformanceBonus(machine)` / `GetCurrentQualityBonus(machine)`；`ignoreBonus=true` 时置 0 | `:121-122` |
| 预算 | `budget = (long)((min(totalVolume, 10_000_000) * 3 / 1000 + 25_000) * dirtBonus)` | `:137` |
| 执行 | `RemoveLargestFirst(container, GetMaxPurityCap(bonusQuality), budget)`：按 particleSize 由大到小从各液体 tag 扣除，加回 water tag | `:217-283` |
| 收尾 | `WaterFeatureHelper.UpdateWaterFeature(container, isExpose:false)` | `:139` |
| 返回值 | 实际移除的污染量（int，parts）；≤0 表示没净化 | `:141` |
| 机器校验 | **无**（不看机器类型/电量/窗口/是否在店） | `:114-141` |

常量：`BUDGET_VOLUME_PERMILLE=3`(`:8`)、`BUDGET_FLAT_UL=25000`(`:10`)、`BUDGET_VOLUME_CAP_UL=10_000_000`(`:12`)、`REMOVAL_MULTIPLIER=1.25`(`:14`)、`BASE_MAX_PURITY=9900`(`:16`)、`DIRT_BONUS_CURVE={9800:1.0, 9400:2.0, 8850:5.0, 8000:10.0, 6250:15.0, 3750:20.0}`(`:18-26`)、`REMOVED_LAST={mineral, chemical_contaminant, heavy_metal}`(`:28`)。

### 4.2 净化数学与"产能直觉"（已确认公式，数值为推演）

- 利用率曲线：`GetPurityMult(bonusQuality)`（`:164-191`）：quality 加成 ≥200→0，≥100→0.1，≥75→0.2，≥50→0.35，≥25→0.65，≥15→0.85，否则 1.0；`GetMaxPurityCap = 10000 - round(100*mult)`（`:212-215`）。即品质加成越高，净化后的上限纯度越高（最高 10000）。
- `dirtBonus = GetDirtBonus(当前纯度) + performance/100`，clamp≥0，再 ×1.25（`:193-210`、`:137`）。**水越脏 bonus 越大**（3750 及以下约 20；6250→15；8000→10；8850→5；9400→2；9800→1），所以脏水一夜能大幅提纯，接近纯水后每夜只能微调。
- 单夜移除上限 ≈ `budget`，但不得超过"非水部分 − 达 cap 所需保留量"（`:217-283` 的 `num2`）。**注意 budget 对总量做了 10,000,000 parts（=10 L）封顶**，所以大蓄水箱的单夜处理量与箱容量脱钩——塔的实际日处理量必须靠 `调用次数 × 单次预算` 设计（例如按模块/品质决定每夜循环几次），否则 600 L 大箱从沟流水到纯净需要极多夜。
- 净化是"替换"过程：被移除的污染量会加到 water tag 上，`WaterFeatureHelper.UpdateWaterFeature` 再重算显示；因此塔的每夜收益 = `min(请求量, tank 实际可提升量)`。
- **风险**：若塔直接对装满 600 L 的 tank 调一次，收益很低，容易被误判为"不工作"；设计阶段就需要把"每夜净化次数 / 单夜处理量上限"写成明示参数（M2 验证项）。

### 4.3 塔必须自建的"机器骨架"（已确认接口）

| 需要 | API | 证据 |
|---|---|---|
| 机器初始化 | `MachineryHelper.InitMachinery(item, drawnPower, isTogglable)` | `MachineryHelper.cs:117` |
| 电池槽 | `MachineHelper.SetupBatterySlot(slotInventory, machine)` | `MachineHelper.cs:41-54` |
| 电池/电量 | `MachineHelper.GetBatterySlot:133`、`TryDrawCyclePower:358`、`PowerHelper.CanDrawPowerSource:681` / `DrawPowerSource:653` | 已确认 |
| 模块槽 | `MachineHelper.SetupModuleBay(inventory, machine, onChanged, allowedModuleTypes)`；模块面板 `GameGridInventory(config, 7)` | `MachineHelper.cs:307-338`；`MachinePurifier.cs:58/143-150` |
| 模块结算 | `MachineHelper.OnMachineActioned(machine, moduleGrid)` | `MachineHelper.cs:368-378` |
| 机器 tag | `STANDARD_MACHINE_TAG`、`CONTAINER_TAG`、`manufacture`、`MACHINE` 类型、`MACHINE_QUALITY_RATIO_INT` | `MachinePurifier.cs:48-56` |
| 窗口 | `MachineHelper.CreateMachineryWindow(canMove, panels)` + `Panel/PowerPanel/NotePanel/WrapInIntake` | `MachineHelper.cs:56-97` |

**结论**：塔必须"长得像机器"（`SetGameItemType("MACHINE")` 会同时 EnableTag，`GameItem.cs:2229`），否则加成为 0、模块无效、也不能享受未来 mod/vanilla 的机器通用逻辑。

### 4.4 蓄水箱与容器设计

| 方案 | 容量 | 优缺点 | 证据 |
|---|---|---|---|
| 自定义 tank 物品（推荐） | 自定（水塔用 600 L） | 真液体容器，可直接被 `PurifyContainer`/`TransferLiquid` 处理；需自己注册物品 + 安装到窗口锁槽 | 水塔 1.0.8：`WaterHelper.InitLiquidContainerItem(obj,600000,0,...,true,4)` + 锁槽 |
| 复用 `water_jug` | 6 L | 现成物品，但容量小、tank 是"可买物品"语义，不适合做机器内脏 | `FoodItemDirectory.cs:313`（6000 ml / unitValue 15） |
| 复用 `watertower_tank` | — | 属于水塔 mod，不在官方；净水塔不能依赖其它 mod | — |

**容量硬约束**：`InitLiquidContainerItem` 对 `capacityInMl > 2,147,483` 会 LogError 并截断（`WaterHelper.cs:51-204`，已确认）；内部 `capacityInParts = ml * 1000` 为 int，600,000 ml = 6 亿 parts 仍在 int 内，但**不要超过约 2.1e6 ml**。

### 4.5 复用风险清单

1. **加成缺失→永久最差档**：塔忘记 `InitMachinery`/模块槽时，`GetCurrentPerformanceBonus` 读不到 tag（默认 0），净化预算和纯度上限都是最低档（`MachinePurifier.cs:121-122`）。
2. **标签/定价脱节**：`PurifyContainer` 只调 `UpdateWaterFeature`（`:139`），但若塔随后把水灌进瓶并自行设置过期标签，会触发假标签（`SecData.GetMislabeledWaterWeight:703`）与脏水检查（`PlayerStore.PerformOnSoldCheck:2718-2737`）。塔必须每次按实际纯度重贴（`WaterFeatureHelper.UpdateWaterFeature:227`）。
3. **忽略电量语义**：原版净化器是"有电才净化、先净化后扣电"（`MachinePurifier.cs:79-102`）。塔若抄"先扣电后退钱"会引入电费异常；建议沿用原版"先净化，removed>0 才扣电"。
4. **忽略液体温度/化学反应副作用**：`TransferLiquid` 内部会逐液体 `HandleFilter` 与 `ChemicalHelper.HandleReceiveLiquidTemp`（`WaterHelper.cs:267-317`）；自定义 tank 若没有 `FILTER_CONTAINER_TAG`，滤芯逻辑不生效（纯净度来源要靠净化而非过滤）。
5. **性能**：`GetRunningFarmCountTonight`/集水器统计这类"遍历全店"逻辑在 vanilla 里靠 10 秒缓存（`MachineMoistureFarm.cs:166-176`），塔夜结只跑一次可接受，但**不要**在 Update 里调。

---

## 5. 自动接水（进水路径）

### 5.1 水源矩阵

| 水源 | 位置/接口 | 自动可行性 | 事件时点 | 证据 |
|---|---|---|---|---|
| 玩家放入塔进水槽的任意液体容器（jug、瓶、水塔） | 塔窗口 `GameSlotInventory` | **100%**（拖拽即入槽） | 玩家操作时 | `WaterHelper.cs:96-165`（容器互倒规则）、`InitLiquidContainerItem` 内置 |
| 集水器 `moisture_farm` 的第 2 槽容器 | `MachineHelper.GetSlot(farm, 2)` 取 `GameSlotInventory.childItem` | **可自动**：夜结扫描全店集水器 → 抽走其槽内液体 | 集水器周期在 `PlayerStore.cs:1723` 先跑，塔在 `:1743` 后跑 → 当夜产水可被抽 | `MachineHelper.cs:148`、`MachineMoistureFarm.cs:152-176`（CanRunTonight/CountRunningFarms）、`WaterHelper.TransferLiquid:267` |
| 水龙头 `faucet` | 常驻设施（`EmporiumEntry.cs:1039`）；交互在 `WaterHelper.cs:98-165` | **仅手动**：拖容器到 faucet 补当日锈水/沟流水；**无自动 API/事件** | 玩家拖拽时 | `WaterHelper.cs:150-164`（`RUSTWATER_TAP`/`GUTTERFLOW_TAP` 解锁判定） |
| 其它容器（柜台/展示柜上的水瓶） | `PlayerStore.FindAllItem()` 遍历 11 窗口 | **可自动但成本高**：全店扫描 + 抽水 | 夜结 `OnGoingSleepLate` 或塔自身 late cycle | `PlayerStore.cs:1746-1768`、`GraphUtils.FindAllChildrenType:593-630` |

### 5.2 实现约束（已确认）

- `TransferLiquid(source, target)`：**两容器都必须带 `LIQUID_CONTAINER_TAG`**，目标容量不足时按 `min(目标剩余, 源总量)` 比例转移，逐液体 `HandleFilter`（`WaterHelper.cs:267-317`）；塔的 tank / 进水槽容器必须都是液体容器。
- 集水器取水：先 `CanRunTonight(farm)`（`MachineMoistureFarm.cs:125`）判定当晚是否真在产水，再用 `GetOutputVolume/GetOutputQuality`（`:192/198`）或直接 `TransferLiquid` 抽槽内实际液体；**共享空气惩罚已经在集水器自己产水时算过**（`ApplySharedAir:119`），塔抽水不重复打折。
- 不能把塔的 tank 子物品"放进集水器槽里让原版灌"：同一 `GameItem` 只能有一个父库存；塔 tank 在塔窗口里，不能再插入集水器槽。**只能塔主动抽**。
- 落格/容量判断前置于扣水扣电：`GraphUtils.CanAccept:111` / `TryAcceptAll:166` / `TryAcceptAllOrDestroy:96`。
- 夜结约束：塔的结算不要在 `OnUpdate` 或 `Update` 里做；一次夜结只跑一次，用 `StoreStation.GetDayCounter()`（`StoreStation.cs:112`）写 `lastSettledDay` tag 做幂等守卫（**风险**：`OnGoingSleepLate` 与物品周期双挂会双结算，二选一或 day guard）。

### 5.3 风险

- **风险**：塔抽干集水器容器会让玩家困惑（"我的集水器不产水了"）——需要 UI 明示（塔的进水槽/提示文案）与优先级策略（只抽进水槽为空的塔、或留最低水位）。
- **风险**：若玩家把塔的 tank 当普通容器拖到别处，塔窗口锁槽必须禁止移除（`mayInventoryRemoveItemFunc` 返回 false），否则状态环不一致（参考 `VendingFountainHelper.cs:34-124` 机器不可移除）。
- **待验证**：集水器第 2 槽容器被塔抽走后，原版集水器 UI 的"水量/品质"显示是否刷新（其面板依赖槽内容，可能只在周期回调后刷新）。

---

## 6. 自动灌装（塔内液体 → 空瓶）

### 6.1 两条可复用链

**链 (a)：边产边销（推荐，不产生瓶堆）**
直接复用 `VendingFountainHelper.SimulateWaterSale`（`VendingFountainHelper.cs:269-303`），它内部就是"灌装 + 估值 + 成交"：`ItemSpawner.Spawn(containerID)` → `unitValue=0` → `TransferLiquid(水源, 瓶)` → 贴纯度标签特征 → 零售估值 → `PerformOnSoldCheck` → `Destroy` → `VENDING_CASH`。塔只提供"水源槽"与"塔物品"。

**链 (b)：塔自产成品瓶（让玩家自己拿去卖/摆柜）**
`DirectoryMaster.Item(bottleID)` 造瓶 → `WaterHelper.TransferLiquid(tank, bottle)` → `WaterFeatureHelper.UpdateWaterFeature(bottle, false)`（按实际纯度贴标签）→ `GraphUtils.CanAccept/TryAcceptAll` 落到成品槽。物品会真实存在于存档中（§10/§11 的爆炸风险要计入）。

### 6.2 瓶型与容量（已确认）

| identifier | 容量 | unitValue | sprite | 证据 |
|---|---|---|---|---|
| `mini_bottle` | 250 ml | 3 | `Items/water_container3` "mini" | `FoodItemDirectory.cs:182-188` |
| `small_bottled_water` | 450 ml | 4 | "small" | `:174-180` |
| `bottled_water` | 750 ml | 5 | "normal" | `:164-173` |
| `large_bottled_water` | 2000 ml | 9 | "large" | `:303` |
| `water_jug` | 6000 ml | 15 | `Items/water_container4` "xl" | `:313` |

> 实测价格由 `GetCurrentValue(useRetailMarkup:true,...)` 计算（见 `SimulateWaterSale`），水价基数见 `WaterFeatureHelper.WATER_PRICES[6]={200,100,40,27,20,10}`（`:59`，已确认）。

### 6.3 标签合规与"WATER"类型（已确认）

- 灌装后容器非空 → `WaterHelper.UpdateContainerGoodType(item)` 会 `SetGameItemType("WATER")`；空容器 → `HOUSEHOLD_GOOD`（`WaterHelper.cs:586-604`）。`SetGameItemType` 内部同时 `EnableTag`（`GameItem.cs:2229`）→ 展示柜水客户、售货机收水判定都依赖 `IsTag("WATER")`。
- 纯度档映射：`WaterFeatureHelper.GetConditionFromPurity:222-225` = `conditionWaterPurity[GetPurityArrayIndex(purity)]`；`GetPurityArrayIndex:451-462` 遍历 `WATER_PURITIES[6]={9990,9900,9800,9600,9000,3500}`，`purity >= 该档` 取第一个命中，否则 5 档（0 档最高）。**必须用实际纯度取档**，不可手写高档位。
- 调用点已确认：`InitLiquidContainerItem:93`、`OnLiquidContainerPourInto:151`、`UpdateWaterFeature:214` 都会触发类型刷新（`SpriteHelper:9` 另有）。

### 6.4 失败处理顺序（建议）

1. 先判断"成品槽是否有空位 / 电量是否足够"（`CanAccept:111`、`CanDrawPowerSource:681`）；
2. 再扣水/扣电（`TransferLiquid` / `DrawPowerSource:653`）；
3. 失败则不扣、不写 `lastSettledDay`（导致下夜重试或直接跳过，按设计选）；
4. 每夜自动灌装/销售数量设上限，避免物品爆炸（§10）。

---

## 7. 自动售卖与取现

### 7.1 挂载点矩阵

| 候选 | API 修饰符 | 是否自动 | 证据 | 结论 |
|---|---|---|---|---|
| 原版售水机（fountain）的夜间售水 | `public static void SimulateWaterSale(int labelPurity, string containerID, GameSlotInventory waterInv, GameItem vendingMachine)` | 是 | `VendingFountainHelper.cs:269-303` | **推荐**：塔直接调用 |
| 原版售水机批量售水 | `public static void HandleNight(GameItem, GameSlotInventory, GameSlotInventory, GameSlotInventory)` | 是 | `:195-267` | 可选（塔有多个品质槽时） |
| 内嵌原版售水机 | `DirectoryMaster.Item("vending_fountain")`（`FurnitureItemDirectory.cs:33` 已注册） | 是（原版周期） | `:33`、`VendingFountainHelper.cs:103-117` | 仅作 M3 对照（嵌套风险） |
| 普通售货机 | `DirectoryMaster.Item("vending_machine")` | 是，但不收液体 | `VendingMachineHelper.cs:78-167`（白名单排除 `LIQUID_CONTAINER_TAG`） | **不可行** |
| 柜台（前/后） | `AddDirectSellingItemToTable:2484` / `AddDirectToWeightedTable:2541` | 否（只是摆货，成交靠白天客户） | `PlayerStore.cs:2484/2541`、`SellItem:2676` 需 `currentClientInstance` | **无 mod 自动成交点** |
| 展示柜 | `ShowcaseHelper.HandleDisplayCase:362` | 间接（顾客白天上门，还要客流/招牌） | `ShowcaseHelper.cs:362/475-499` | 只能"间接自动售卖" |
| 现金入账 | `TransferCashToPlayer:511` → `ModCash:3718` | 是 | 已确认 | 推荐 |

### 7.2 推荐链（时序文字描述）

1. 原版 `EndNight` 第二波先跑所有物品的 `onCycleEndSlotItemFunc`（`PlayerStore.cs:1720-1724`）：塔在此"接水 + 净化"，把 tank 调整到可售状态；
2. 原版 `EndNight` 后再跑 `onCycleEndLateSlotItemFunc`（`:1726-1729`）：塔在此"灌装/模拟售水"，按 tank 实际纯度档位与自身"吸引力/产能"参数循环 `SimulateWaterSale`，现金累加进塔的 `VENDING_CASH` tag；
3. `EndNight` 结束后 `EndOfDayUIManager.cs:273` 调 `PlayerStore.SaveGame()`，当夜写入自动落盘（已确认）；
4. 玩家次日点塔面板"取现"（或塔自动每夜入账）→ `TransferCashToPlayer(塔)` → `PlayerStore.ModCash(n)`（`PlayerStore.cs:3718`）；
5. 若走"自动入账"：注意别在 `EndNight` 内部直接 `ModCash`（mod 的 playerCash 变更会在 SaveGame 时持久化，语义上等价）；建议在夜结末尾（塔的 late cycle）内完成，不额外挂 `OnWakingUp` 重复结算。

### 7.3 风险

- **风险（平衡）**：全自动收入没有顾客流约束，必须给塔设每夜销量上限/维护费/电力消耗，否则经济失衡。原版售水机的"吸引力"参数可作为塔的升级旋钮（`VENDING_ATTRACTIVENESS`，`VendingFountainHelper.cs:34-124`）。
- **风险（合规）**：`SimulateWaterSale` 内部会走 `PerformOnSoldCheck(gameItem, null)`（`:269-303` → `PlayerStore.cs:2718`），脏水/走私会被查；塔必须只卖"标签=实际纯度"的水，否则触发 `SecData.GetMislabeledWaterWeight:703` 的 mislabeled 统计（原版售水机也吃这条，见 `TrackMislabelledFountainSale:208`）。
- **风险（数据）**：`RunStats.OnFountainSale:317` 会记账，若塔的收入不应计入"售水机收入"统计需另作映射（待验证统计口径是否影响成就/结算）。
- **待验证**：`SimulateWaterSale` 需要一个"水源槽"`GameSlotInventory`，它内部 `GetWaterSource` 只认"槽内物品是液体容器"或"槽内物品是硬编码水源机（water_purifier/moisture_farm）"（`:144-183`）。塔如果直接把 tank 子物品临时放进自己的销售槽再调用、调用完再拿回来是否可行（同一物品不能双父）——**M3 必须实测**：可考虑塔窗口里就有一个"销售槽"，tank 与该槽分离，调用时把水先 `TransferLiquid` 到槽内容器。

### 7.4 备选（对照）

- **内嵌 `vending_fountain`**：原版周期自动卖水（`VendingFountainHelper.cs:103-117`），但塔窗口里套机器窗口，复杂度高。
- **展示柜间接**：塔把瓶装水自动上架 `showcaseElement`（`EmporiumEntry.cs:213-222`），靠 `HandleDisplayCase` 顾客购买；收入走正常客户成交，**依赖客流**，只能作为"低维护版"备选。

---

## 8. 物品注册与目录体系（0 Harmony 关键路径）

### 8.1 机制链（已确认）

1. `Directory<T> : MonoBehaviour` 的 `Awake`：`factoryDictionary = new; if (DirectoryMaster.AddDirectory(this)) InitDirectory();`（`Directory.cs:7-21`，`Awake` 还带 `[RuntimeInitializeOnLoadMethod(SubsystemRegistration)]` 特性，实例方法是否被 Unity 自动调用存疑）。
2. `DirectoryMaster.AddDirectory<T>(Directory<T>)`：按 `typeof(T)` 维护列表，**只按 `typeof(T)` + 具体类型去重**——自建 `ModItemDirectory` 与官方实例类型相同会被判重（返回 false 并 LogWarning），所以只能有一个 `ModItemDirectory` 实例；若官方已建，自建会被拒（**M0 必须探测**）。
3. `ModItemDirectory.InitDirectory()` 唯一行为是 `ModHook.FireOnModItemDirectoryInit(this)`（`ModItemDirectory.cs:1-7`）。
4. 全工程只有消费方：`PaperBookRegistry.cs:585-608` 订阅该事件、把注册条目 `directory.Add(id, factory)`；`BrewingExpansion`、`FutureTech` 等 mod 的 IL2CPP 样本也只订阅不创建（`D:\code\mod\Probably Stolen\decompiled\`）。
5. → **必须由 mod 自建 `new GameObject(...) + AddComponent<ModItemDirectory>()`**，`AddComponent` 同步触发 `Awake`。

### 8.2 注册时序（已确认）

- 读档：`PlayerStore.LoadGame()` 顺序 `FireOnGameLoadedInit`（`:968`）→ `FireOnGameLoadedEarly`（`:986`）→ `FireOnGameLoadedNormal`（`:997`）→ **然后才** `DecodeSaveItem` 各窗口（`:998-1016`）→ `FireOnGameLoadedLate`（`:1066`）。
- 新档：`ModHook.OnNewGameStarted(scenarioId)`（`NewGameData.cs:401`）。
- **结论**：在 `OnGameLoadedInit/Early`（或更早的 `IMod.OnEnable`，需 M0 确认加载时序）完成目录注册与物品工厂注册，存档解码就能按 `identifier` 还原塔。
- 解码失败后果（已确认）：`DirectoryMaster.Item` 找不到 identifier 时 LogWarning "Could not find GameItem identifier ... Creating dummy GameItem instead..."，并返回空物品 → 塔窗口/子物品全部丢失，`SaveManager` 另报 "child item load failure"（`SaveManager.cs:149-246`）。

### 8.3 工厂与存档约束（已确认）

- `DirectoryMaster.Item(identifier, isOwned=true)`（`:252-286`）：工厂返回的物品**不能带子物品**——若带，会被 LogWarning "contains child items ... Removing all child items as a fix" 并全部销毁；同时会分配 `uniqueId`、`SetItemOwned`、`PermitHelper.OnNewItemCreated`。
- 工厂必须**确定性**：面板数量/顺序、库存数量/顺序不得随分支变化（编码按 BFS 序号，解码按同序号重放，`SaveManager.cs:114-146`）。
- 塔的 tank/子物品只能在**放置时**（或 `onLoaded`）装配进窗口，不能在工厂里塞。
- `GameItem.onLoaded`（`PlayerStore.DecodeSaveItem:1078-1122` 末尾调用）是唯一允许重建瞬态引用（面板标签、槽位缓存）的地方；存档只回写 tag/shape/子库存结构。

### 8.4 兜底方案（1 处 Harmony）

若 M0 证明自建目录不触发/被官方实例挡回：Postfix `FurnitureItemDirectory.InitDirectory`（`FurnitureItemDirectory.cs:22-25` 是原版机器注册点，水塔 1.0.8 实机验证过 Postfix `FoodItemDirectory.InitDirectory` 可行）或在 `IMod.OnEnable` 里直接 `DirectoryMaster.Item` 注册（需绕过 AddDirectory 判重，不推荐）。**这会引入唯一 1 处补丁**，与"0 Harmony"目标冲突时以功能可用为先，并在设计文档中记录原因。

### 8.5 M0 探针清单

| # | 探针 | 成功判据 |
|---|---|---|
| P1 | 自建 `ModItemDirectory` 后 `OnModItemDirectoryInit` 是否触发 | 事件回调 1 次、`DirectoryMaster.Has<GameItem>(id)` 为 true |
| P2 | 官方是否已存在 `ModItemDirectory` | 自建 `AddDirectory` 返回 false 即为已存在 → 改注册时机/兜底 |
| P3 | `DirectoryMaster.Item(tower_id)` 是否生成完整塔 | 返回非 dummy、窗口/面板数量正确 |
| P4 | 塔放在 `invElement` 后 `FindAllItem()` 是否能找到 | 返回列表含塔、`IS_OWNED_TAG` 为真 |
| P5 | 手动 `onCycleEndSlotItemFunc?.Invoke` 是否被原版 EndNight 调用 | 塔周期回调日志每夜 1 次、顺序在集水器之后 |
| P6 | 存读档 3 次往返 | 无 dummy/child item load failure 日志、tag/子物品完整 |
| P7 | 塔窗口在自定义物品内正常开合/拖放 | 拖入液体容器成功、`AddInventoryStateChangeAction` 刷新正常 |

---

## 9. ModHook 事件全表（签名 + 触发点，已确认）

> 声明位置均在 `ModHook.cs`，全部是 `public static event Action...`；Fire 方法 `internal static` 且逐个 handler `try/catch`（`SafeInvoke:109-170`，异常只 `Debug.LogError`，不会打断其它 handler）。**注意：没有"机器周期 / 物品装入取出 / 窗口打开 / 顾客成交"事件**，这几类只能用原生委托（`GameInventoryFunc.cs:18-26`、`GameItemFunc.cs:90-94`、`PixelWindow.openCallback/closeCallback:128/130`）。

| 事件（签名） | 声明 | 触发点（file:line） | 净水塔用途 |
|---|---|---|---|
| `OnModItemDirectoryInit(ModItemDirectory)` | `:10` | `ModItemDirectory.cs:5` | 物品注册（0 Harmony 关键） |
| `OnGenerateCustomerVeryEarly/Early/Normal/Late/VeryLate(StoreClientManager)` | `:12-20` | `StoreClientManager.cs:151/164/172/177/187` | 不需要 |
| `OnGameLoadedInit/Early/Normal/Late()` | `:22-28` | `PlayerStore.cs:968/986/997/1066` | **注册目录/工厂（Init/Early）** |
| `OnShutterOpenedEarly/Late()` | `:30/32` | `StoreShutterButton.cs:110/190` | 不需要 |
| `OnShutterClosedEarly/Late()` | `:34/36` | `StoreShutterButton.cs:214/250` | 可做"日开启提示" |
| `OnGoingSleepEarly/Late()` | `:38/40` | `PlayerStore.cs:1664/1743` | **夜结兜底（Late 最安全）** |
| `OnWakingUpEarly/Late()` | `:42/44` | `StartOfDayUIManager.cs:49/55` | 可做次日 UI 刷新（不要做重活） |
| `OnLeavingStoreEarly/Late()` | `:46/48` | `EmporiumEntry.cs:655/787` | 不需要 |
| `OnReturningStoreEarly/Late()` | `:50/52` | `EmporiumEntry.cs:817/837` | 不需要 |
| `OnHandlingNightlyServicesEarly/Late()` | `:54/56` | `PlayerStore.cs:1687/1707` | 可选：与其他服务结算对齐 |
| `OnPlaceInventorInventoryItemEarly/Late(List<GameItem>)` | `:58/60` | `StoreClientList.cs:493/553`（SpecialStock）、`:587/1128`（PlaceInventorInventory） | **获取路径：上架杰克逊** |
| `OnCreateTooltipEarly/Late(RichTextBuilder, GameItem)` | `:62/64` | `ModInternal.cs:5/10`（同文件另有 DevTooltip 两条） | 塔 tooltip 定制 |
| `OnCreateDevTooltipEarly/Late()` | `:66/68` | `ModInternal.cs:15/20` | 不需要 |
| `OnModAssetsLoaded(LoadedMod)` | `:70` | `ModLoader.cs:1395` | mod 资源加载完成（贴图/本地化） |
| `OnModConfigApplied(string modId)` | `:72` | `ModConfigWindow.cs:233` | 不需要 |
| `OnLootTablesLoaded()` | `:74` | `LootRegistry.cs:117` | 不需要 |
| `OnLootRolled(LootRollContext)` | `:76` | `LootRegistry.cs:364` | 不需要 |
| `OnPaperOpened(string, PaperUI)` | `:78` | `PaperBookRegistry.cs:332` | 不需要 |
| `OnNetworkUpgradeUnlocked(string)` | `:80` | `NetworkUpgrade.cs:856` | 水龙头/售水相关解锁联动 |
| `OnNewGameStarted(string scenarioId)` | `:82` | `NewGameData.cs:401` | 新档注册兜底 |
| `OnDifficultyApplied(string)` | `:84` | `PlayerStore.cs:790/984` | 不需要 |
| `OnAfterhourArriveEarly/Late(string locationId)` | `:86/88` | `AfterhourEvents.cs:176/226`、`MapUIManager.cs:174/196`（该组与 4 个事件的一一对应待复核） | 不需要 |
| `OnAfterhourLeaveEarly/Late(string)` | `:90/92` | 同上 | 不需要 |
| `OnAfterhourEventStarted/Ended(string,string)` | `:94/96` | `MapUIManager.cs:241/253`（待复核） | 不需要 |

**夜结内部顺序（已确认，`PlayerStore.cs:1655-1744`）**：关客户 → `FireOnGoingSleepEarly`(:1664) → 投影/电力夜间状态 → 早期波 `onCycleEndEarlySlotItemFunc`(:1669-1672) → 服务扣费 → `FireOnHandlingNightlyServicesEarly`(:1687) → 各服务 → `FireOnHandlingNightlyServicesLate`(:1707) → 探险/化学/冰箱 → `HopperHelper.RunWindow(1)` → **第二波 `onCycleEndSlotItemFunc`(:1720-1724，原版机器周期)** → `HopperHelper.RunWindow(2)` → **`onCycleEndLateSlotItemFunc`(:1726-1729)** → `HopperHelper.RunWindow(3)` → `isNightCycle=false` → 冰箱过期 → `storeState=MORNING` → 隐藏售货机窗口 → **`FireOnGoingSleepLate`(:1743)**。随后 `EndOfDayUIManager.cs:273` 存档。

---

## 10. 性能约束与风险

- **禁止 `Update`/每帧扫描**：原版 `StoreVendingFountain` 每秒协程只换图（`StoreVendingFountain.cs`，56 行），那是 vanilla 特例；塔不要模仿。
- 夜结一次 O(n)：`FindAllItem` 遍历 11 个窗口 + 递归 BFS（`PlayerStore.cs:1746-1768`、`GraphUtils.cs:593-630`），货架/柜台/主库存/展示柜/售货机都在列；塔只需一次全店遍历（找自己 + 找集水器），不要在每个物品周期里重复全店扫描。
- 面板刷新用 `PixelWindow.AddInventoryStateChangeAction:856` 或槽回调（`GameInventoryFunc.cs:18-26`，`{get;set;}` 可用 `+=`）按需触发，不轮询。
- **物品爆炸**：自动灌装若把瓶落进世界/货架，每夜新增物品 → 存档体积与遍历时间线性增长；优先"边产边销"（`SimulateWaterSale` 内部 `Destroy`），必须落瓶时设每夜上限与满仓停止。
- **异常隔离**：原版 EndNight 里 `onCycleEnd*SlotItemFunc` 是**裸调用**（`PlayerStore.cs:1723/1728`，无 try/catch），塔的周期回调一旦抛异常会中断整夜结算 → 塔必须自己 try/catch（对比 ModHook handler 有 `SafeInvoke:109-170` 保护）。
- **夜结禁止 UI/存档操作**：`EndNight` 在关闭界面后的黑屏序列里（`EndOfDayUIManager.cs:255`），不要弹窗、不要 `SaveGame`；存档在 `:273` 自动发生。
- **待验证**：塔的周期回调若调用较重的 `SimulateWaterSale`（含物品生成/估值/统计），对夜结总时长影响；建议 M3 用 10 夜样本测 EndNight 耗时。

---

## 11. 存档与迁移风险

1. **风险（静态引用失效，水塔 1.0.8 最大坑）**：读档后窗口/槽引用会失效，水塔靠 `TowerRegistry` 重建 + 反推槽位。净水塔建议**不跨读档保存任何静态引用**：夜结用 `FindAllItem` + 自身 tag 定位，窗口内引用在 `onLoaded`（`PlayerStore.DecodeSaveItem:1078-1122`）重建。
2. **风险（布局变更=槽位序号错配）**：`SaveManager` 的子库存按 BFS 序号配对（`SaveManager.cs:149-246`）；水塔用 Harmony Prefix 改写旧档 `childItemInventoryNode` 序号（`WaterTower-Design` 反编译 `SaveLayoutMigration.cs:22-34/52-60`）。**0 Harmony 下不可做** → 规则：塔窗口布局一经发布 append-only（只在末尾加面板/槽），版本 tag 记录布局版本，`onLoaded` 校验"槽内物品类型/贴图"做自愈；不能依赖读档后修改历史顺序。
3. **风险（卸载）**：mod 卸载后存档里塔的 identifier 找不到 → dummy item + "child item load failure" 日志（`SaveManager.cs:149-246`）。塔物品应带 `version`/`layout` tag，重装后可自愈；用户提示写在 mod 文档。
4. **风险（夜结中断）**：`EndNight` 内塔的周期回调若异常/玩家强退，可能只写了一半；用 `lastSettledDay` tag + 幂等（重复结算只补差额或跳过）保证重入安全。
5. **存档体积**：液体以 int tag 存（parts = ml×1000），塔 tank + 少数瓶可忽略；真正膨胀来源是"每夜落瓶"。tank 容量上限受 int 约束（§4.4）。
6. **风险（迁移检测）**：旧版塔（若先发布过布局）读档时，用 `version` tag 走 mod 侧迁移（读档后手动调整 tag/子物品），不改存档二进制；这是 0 Harmony 下唯一可行的迁移形态。

---

## 12. M0→M5 里程碑（每阶段验证点，不写代码）

| 阶段 | 目标 | 验证点（退出标准） | 失败退路 |
|---|---|---|---|
| **M0 打样/探针** | 注册塔物品、开窗口、拖放、tag 读写、存读档往返 | P1-P7（§8.5）全通过；3 次存读档无 dummy/child failure；塔在 `FindAllItem` 内、周期回调每夜 1 次 | 自建目录失败 → 1 处 Harmony Postfix 兜底 |
| **M1 夜结骨架** | tank 子物品、进水槽、每夜结算、电量、day guard | 连续 10 夜：断电/空仓/满仓/无进水四种边界不报错、不中断 EndNight；`lastSettledDay` 幂等；夜结耗时增量可接受 | 若物品周期不稳定 → 改用 `OnGoingSleepLate` 单钩子（二选一，不双挂） |
| **M2 净化** | 复用 `PurifyContainer` + 模块/品质加成 + 标签合规 | 指定水量从沟流水→纯水的可预期夜数；标签=实际纯度（假标签检测不报警）；品质加成提升 cap/预算可测 | 单次预算不够 → 设计"每夜处理次数"参数（mod 侧循环），不改原版函数 |
| **M3 获取与售卖** | 杰克逊上架、`SimulateWaterSale` 现金链、`ModCash` 入账、销量上限 | 购买→放置→每夜稳定收入；无 NRE；`PerformOnSoldCheck` 不因脏水/假标签报错；收入统计口径确认 | 若 `SimulateWaterSale` 对塔槽不适用 → 内嵌 `vending_fountain` 对照（方案 B） |
| **M4 水网络** | 集水器自动抽水、进水槽、外部容器、faucet 手动/可选自动策略 | 三种水源端到端；集水器 UI 正常；塔抽水优先级可配；无玩家资产丢失 | 若集水器抽水体验差 → 改为"仅玩家拖拽 + 集水器可选开关" |
| **M5 打磨与交付** | UI 文案/图标/tooltip、性能实测、存档体积、旧档兼容、平衡与文档 | 无 Update/每夜 O(n) 实测达标；发布检查表全过；10 夜回归无回归问题 | — |

> 阶段之间的依赖是**单向**的：M0 未通过不得进入 M1；M1 未稳不得叠加 M2+（否则无法区分是周期机制问题还是净化问题）。

---

## 13. 待验证清单

**P0（阻塞设计定型）**
1. 自建 `ModItemDirectory` 是否触发 `Awake`/`OnModItemDirectoryInit`；官方是否已有实例（§8.5 P1/P2）。
2. 塔物品放在 `invElement` 后是否被 `FindAllItem` 扫到、`onCycleEnd*SlotItemFunc` 是否被原版 `EndNight` 调用（顺序在集水器之后）。
3. `MachineHelper.CreateMachineryWindow` 在自定义物品内的开窗/拖放/存档是否正常（面板数量与顺序确定性）。
4. `PurifyContainer` 对塔 tank 生效，且性能/品质加成来自塔自身模块（无加成时产量下降可观察）。
5. `SimulateWaterSale` 传入塔的销售槽与塔物品是否成功、`VENDING_CASH` 是否累加 → `TransferCashToPlayer(塔)` → `ModCash` 是否到账。

**P1（影响体验/平衡）**
6. 假标签检测（`SecData:703`）的具体触发条件与处罚；塔"标签=实际"是否足够安全。
7. 展示柜间接售卖的转化率（是否值得作为低维护备选）。
8. 集水器抽水对原版集水器 UI/共享空气体验的影响。
9. 内嵌 `vending_fountain` 方案的可行性与复杂度（M3 对照）。
10. 自定义 tank 子物品在窗口锁槽中的存档/拖动/卸载表现；`water_purifier` 子物品复用作为 M2 对照。
11. 瓶型 `IsTag("WATER")` 判定与展示柜水客户阈值（`ShowcaseHelper.cs:475-499`）的匹配度。
12. 夜结耗时/存档体积的 10 夜回归数据。
13. 若走"自动入账"，收入是否应计入 `RunStats.OnFountainSale` 统计（成就/结算影响）。

---

## 14. 证据索引

**源码（`D:\code\mod\.tmp-decompile-current\Assembly-CSharp\`）**
- 目录/注册：`Directory.cs:7-21,24-105`、`ItemDirectory.cs:3-21`、`ModItemDirectory.cs:1-7`、`DirectoryMaster.cs:21-42,83-145,252-286`
- 夜结/存档：`PlayerStore.cs:1655-1744`（EndNight 顺序）、`:1746-1768`（FindAllItem）、`:938-957`（EncodeSaveItem）、`:958-1076`（LoadGame）、`:1078-1122`（DecodeSaveItem）、`:2484-2546`（摆货）、`:2605-2737`（买卖/检查）、`:3718`（ModCash）；`SaveManager.cs:114-146,149-246,252-258`；`SaveItemNode.cs:6-`；`EndOfDayUIManager.cs:255/273`；`StoreStation.cs:23,63,112`
- 机器/净化：`MachinePurifier.cs:6-28,30-112,114-141,143-150,164-283`；`MachineHelper.cs:7-39,41-97,119-131,133,148,153,164-167,307-338,358-378`；`MachineryHelper.cs:117,194,199,532,537`；`PowerHelper.cs:653,681`
- 液体/水质：`WaterHelper.cs:51-204,257,267-317,397,412,586-604,941,1384-1395`；`WaterFeatureHelper.cs:5-73,222-225,227,277,451-462,568/573`；`ItemConditionList.cs:39`；`FoodItemDirectory.cs:164-173,174-180,182-188,303,313`；`FurnitureItemDirectory.cs:22-25,32-35`；`ItemSpawner.cs:3-17`；`WaterPremadeHelper.cs:146,157-230`
- 水塔/售水：`VendingFountainHelper.cs:34-124,144-183,195-267,269-303,305-317,336-395`；`VendingMachineHelper.cs:78-167,169-188,190,208,511-522`；`StoreVendingFountain.cs`（56 行）；`ShowcaseHelper.cs:362-377,379-410,436-473,475-499`
- 事件/框架：`ModHook.cs:10-96,109-170`；`GameInventoryFunc.cs:18-26`；`GameItemFunc.cs:90-94`；`PixelWindow.cs:128,130,136,141,146,856`；`GameItem.cs:656,673,2229,2238`；`EmporiumEntry.cs:12-380,1039,1412-1415,1531-1537,1152-`；`StoreClientList.cs:493,553,587,1128`；`PaperBookRegistry.cs:585-608`；`IMod.cs`、`LoadedMod.cs`、`ModLoader.cs:1395`

**水塔 mod（`D:\code\mod\Probably Stolen\WaterTower-Final\decompiled\WaterTower\`）**
- `WaterTowerWindow.cs:145`（自建 PixelWindow）；`NightSettlement.cs:66-86`（EndNight Postfix + 3 秒去重）、`:437-487`（集水器代产）、`:553/725/1176`（反射取电池槽）、`:704-782`（DriveCollectorInto）、`:1126-1248`（PurifyTank/PurifyWithOne）；`SaveLayoutMigration.cs:22-34,52-60`（槽索引迁移 + DecodeNodes Prefix）；`LazyPatches.cs`（EncodeSaveItem 探针）

**前序设计**
- `D:\code\mod\Probably Stolen\CollectorTower-Design\DESIGN.md:553,595,600,607-615,622,880,1015`（0 Harmony 原则与风险）；`research/01-watertower-reference.md`、`research/02-game-water-and-bottling.md`（本任务不重复其内容）

---

## 15. 一句话结论（给设计文档主笔）

净水塔的 0 Harmony 路线成立的前提只有一条：**自建 `ModItemDirectory` 能在读档前触发 `OnModItemDirectoryInit`**；其余全部能力（机器窗口、夜结周期、净化、倒水灌装、原生售水链、现金、上架、tag 存档）均已有 public static/官方事件支撑。工程上最大的真实工作量不是"能不能接"，而是**塔的机器骨架（电池/模块/加成）与标签合规（标签=实际纯度）**——这两点决定净水塔是"原版净化器的合格复刻"还是"会出假水案的经济漏洞"。
