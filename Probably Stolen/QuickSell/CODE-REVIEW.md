# QuickSell 代码审核报告

日期：2026-10-31　|　范围：`Probably Stolen/QuickSell` 1.2.1 本地版（含「同类保护按名称」「尽量用满预算卡住修复」「单件解保」）　|　性质：静态审阅，**本次未修改任何代码**。

## 1. 结论与评分

工程化程度明显高于常见游戏 mod：分层清楚、防御性强、纯逻辑有测试、文档闭环。主要短板集中在 `Trading.cs` 的巨型协程和少数健壮性细节。

| 维度 | 评分 | 一句话 |
| --- | --- | --- |
| 结构 / 可维护性 | 7 / 10 | 分层清楚，但 `RestockPass` 单体过大、校验逻辑重复 |
| 健壮性 | 8.5 / 10 | 边界处理出色；配置解析与 UI 侧少数路径缺少兜底 |
| 性能 | 7.5 / 10 | 协作式时间片是亮点；候选搜索、UI 重建非最优 |
| 测试 | 7 / 10 | 纯逻辑覆盖好（27 项）；风险最高的 Unity 侧无自动化 |
| 可读性 | 7 / 10 | 命名/注释好；魔数偏多、平行列表靠下标耦合 |
| **综合** | **7.5 ～ 8 / 10** | 在 Unity mod 生态里属于前 10% |

## 2. 代码构成速览

- 规模：14 个源文件 2152 行；测试项目 138 行、27 项检查；构建 0 警告 0 错误（`netstandard2.1`、`Nullable enable`）。
- 组织方式：`QuickSellMod` 一个 partial class 按职责拆到 9 个文件（核心生命周期、交易、设置 UI、高亮、补丁、右键菜单、确认弹窗）。
- 纯逻辑层（不碰 Unity，可控制台测试）：`SelectionPlanner`（有界 DP 选货）、`SelectionRules`（风险/货源规则）、`ShortcutKeys`、`HintBounds`、`NativeContracts`（启用自检）。
- 数据流：`Tick()` 三 job 协作调度 → `Discover`/`Restock` 协程 → `SelectionPlanner` 规划 → 逐件搬运并复核 → `OnSold` 清理保护记录。

## 3. 优点（已核对）

1. **纯逻辑抽离 + 测试**：选货、规则、快捷键、提示边界均不依赖 Unity；测试含 100 组穷举对照、100/1000/10000 候选的性能与 8 MiB 分配上限、旋转/翻转边界。
2. **协作式调度**：3 个 job 槽 + 每帧约 1 ms 预算 + `IDisposable` 枚举器 + 统一异常转日志（`QuickSell.cs:112-146`），扫描/搬运不卡帧。
3. **搬运循环防御式复核**：每次 `yield` 后重新校验 store/customer、父库存、保护/规则、赃物热度、配置变化（`Trading.cs:319-365`）；失败回滚原容器、失败即停并报错（`Trading.cs:404-428`）；异常信息带物品/来源/台面状态（`Trading.cs:389-402`）。
4. **原版契约自检**：启用时用未初始化对象验证 `GameItem.AddParent/RemoveParent` 语义，不满足拒绝启用（`NativeContracts.cs:9-21`）。
5. **补丁克制**：声明式 Harmony、只在自身状态内 Prefix 拦截、无 Transpiler、不改游戏 DLL（`Patches.cs`，`REVIEW.md:35-39`）。
6. **资源/配置**：三张高亮图合成 512 图集、每物品最多 6 个共享矩形面（`HintArtwork.cs:13-45`、`Hints.cs:302-344`）；配置按 `slot:run` 分区、名称 `Uri` 转义、坏数据只告警（`QuickSell.cs:154-192`）。

## 4. 问题清单

### A. 建议优先修复（中高）

| 编号 | 位置 | 问题 | 建议 |
| --- | --- | --- | --- |
| Q-01 | `QuickSell/Trading.cs:157-447` | `RestockPass` 约 290 行巨型协程；`:323-336` 与 `:345-361` 校验块几乎逐行重复，`:337-340` 与 `:362-365` 重复台面扫描。本轮「尽量用满预算」修复就必须在两个分支各加一次回填，已实际暴露维护风险 | 抽 `Revalidate()` 与一次性 `TableSnapshot`（`onTable` + `remaining`）；扫描/规划与搬运拆成两阶段；`candidates/candidateHeat/types/groups` 平行列表改 struct |
| Q-02 | `QuickSell/QuickSell.cs:21`、`QuickSell/AssistantUI.cs:292` | `AllowanceAmount => int.Parse(...)` 无保护；配置被手改为非法值时抛 `FormatException`，且 UI 回调线程调用不在 any try/catch 内 | `int.TryParse` + 钳制 0～20；`priority`/`stolenRule`/`contrabandRule` 也做取值校验 |
| Q-03 | `QuickSell/AssistantUI.cs:185` | `RefreshPage` 第 1 页直接访问 `EmporiumEntry.Instance.backInvinvElement`，读档/关店时序异常时可能空引用 | 入口判空，为空时显示占位并安全返回 |

### B. 中期优化（中）

| 编号 | 位置 | 问题 | 建议 |
| --- | --- | --- | --- |
| Q-04 | `QuickSell/QuickSell.cs:165-192` | `SaveIds`/`SaveNames` 保留其它 `slot:run` 的旧条目，只增不减；周目多后配置字符串膨胀、每次读档全量 `Split` | 保留最近 N 个 run，或读档时清理当前存档旧 run |
| Q-05 | `QuickSell/Trading.cs:449-493` | `SplitCandidate`/`ChangeStackAmount` 手工克隆十余个字段造物品，对游戏版本变化极脆；`CheckedSplitQuote` 的报价不变量无纯函数测试 | 把报价数学抽成纯函数并加用例；在 `NativeContracts` 增加拆分前置校验 |
| Q-06 | `QuickSell/Trading.cs:96-114` | `FindCounterSlot` 先走原版一次、不满足再扫 `格数 × 2 flips × 4 rotations`，每个候选都执行，候选多时上货变慢（虽有时间片让出） | 一次 pass 内缓存格位/放置结果复用 |
| Q-07 | `QuickSell/AssistantUI.cs:140-147` 等 | 设置页每次刷新销毁重建全部子物体；窗口 860×650、列坐标 498/658 等硬编码魔数；界面文本纯中文硬编码 | 关键常量集中命名；如后续支持多语言再引入本地化键 |

### C. 低优先 / 长期

| 编号 | 位置 | 说明 |
| --- | --- | --- |
| Q-08 | 多处 | 魔数集中：32（回填上限）、1024/200000（规划器）、1 ms、8 pass、7 行/页、512 图集、1254 原图坐标；建议命名常量并注明来源 |
| Q-09 | `QuickSell/SelectionPlanner.cs:28-83` | `Build` 是迭代器 + `Result` 属性副作用，必须枚举完才有效，易误用；可改为返回结果 |
| Q-10 | `QuickSell/QuickSell.cs:24,35` | `jobs[0..2]` 魔术下标散布多文件（`IsRestocking => jobs[1] != null`）；建议 enum 或具名属性 |
| Q-11 | 整体 | `QuickSellMod` 约 1100 行 partial class 有上帝类倾向；mod 规模可接受，长期可按「交易/UI/高亮/保护」拆独立服务 |
| Q-12 | `QuickSell/ProtectionMenu.cs:14-38` | 右键菜单接管依赖原版 7 按钮结构，第三方改菜单即坏（`REVIEW.md:40` 已承认）；建议结构校验失败时降级为不接管 |
| Q-13 | 发布流程 | 发布时需同步：csproj `Version`、`manifest.xml` 的 `<ModVersion>`/`<Capability>`、README、DLL 哈希、preview 哈希；本机更新按 AGENTS 规则先备份再核对 |

## 5. 后续优化建议（分批）

- **P0（低成本、直接消除崩溃类风险）**：Q-02、Q-03。验收：构建零警告；27 项检查保持通过；手工构造非法 `OverBudget` 配置不再抛异常；打开面板/读档过程中无空引用日志。
- **P1（降低维护成本）**：Q-01。验收：行为无变化（27 项检查 + 新增 Revalidate 边界用例）；实机回归「上货 → 预算复核 → 回填候选 → 保护确认」全流程。
- **P2（数据与风险）**：Q-04、Q-05、Q-06。验收：`CheckedSplitQuote` 纯函数用例通过；旧 run 配置在多次读档后不增长；相同候选规模下上货耗时与分配不劣化。
- **P3（长期重构）**：Q-07～Q-13。可作为大版本重构项，不阻塞当前发布。

## 6. 复审与验证方式

- 构建：`dotnet build "Probably Stolen/QuickSell/QuickSell.csproj" -c Release`，要求 0 警告 0 错误。
- 控制台检查：`cd "Probably Stolen/QuickSell.Tests" && dotnet run -c Release`，当前 27 项全部 PASS。
- 实机回归清单：一键上货（尽量用满预算 / 优先腾空间）、保护确认（单件/批量）、单件解保与恢复、高亮按住、切档/读档后保护记录、快捷键绑定。
- 禁止事项：未经用户当次明确许可，不得部署到本机 Mods、不得上传创意工坊。

## 7. 本次审阅边界

- 已覆盖：全部源码静态阅读 + 与游戏反编译实现的关键契约核对（预算/售价/上台特征/出售流程）。
- 未覆盖：实机运行时行为、长时间存档与其它 mod 冲突矩阵、Unity 帧内性能实测。
- 遗留问题：①「一键上货超预算／提示预算不足」类问题已定位为「规划用台下价、上台时原版临时加价（零售加价／多样化／游客税等）」的估价偏差，现已改为按上台后预计报价估算并在规划阶段排除超预算候选；香水、事件、违禁品等少数未建模加价仍靠上台后回滚兜底。②本轮三处修复（同类保护、排序回填、估价口径）均待实机验证。③版本号/部署/上传未做。详见 `DESIGN.md` 1.2.x 条目与 `REVIEW.md`。
