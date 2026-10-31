# AquaSense 代码审阅报告

- 审阅日期：2026-10-31
- 审阅对象：`AquaSense-Final/AquaSense.cs`（1391 行）、`AquaSense-Final.Tests`（24 个场景）、`AquaSense.csproj`、`manifest.xml`
- 代码版本：`<Version>1.8.0</Version>` / `<ModVersion>1.8</ModVersion>`（工作区，未提交状态）
- 审阅方式：静态通读 + 对照组内测试桩 + 对照新版游戏反编译（`inspection-temp/game-20261031`）
- 范围说明：只审阅本 Mod 的代码质量与风险，不包含其他 Mod、游戏原生代码本身和实机行为验证

> 行号基于本次审阅时的 1391 行版本；后续代码增删后需重新定位（可用函数名检索）。

## 1. 结论摘要

**综合评分：7.5 / 10（社区 Mod 中的中上水平）。**

- 强项：对破坏性库存操作有完整的前置校验、执行后校验、失败恢复三段式设计；维护循环分帧且有预算；补丁安装时机正确（等商店就绪）；配有 24 个带故障注入的回归场景。
- 主要风险：**反射私有成员的初始化没有防御**（游戏更新改名会导致 Mod 整体失效且难以定位）；**多选组拖拽未被守卫**（与“移动物品时消失”类反馈存在潜在关联）；Tooltip 热路径存在反射与临时物品创建。
- 结论：业务逻辑本身没有发现必然导致物品丢失的路径；优先级最高的工作是“抗游戏更新”和“拖拽时序”加固，而不是重写换芯逻辑。

## 2. 架构概览

```text
Core : IMod                 入口、配置、延迟安装 9 个 Harmony Postfix
  └─ AquaSenseRunner        MonoBehaviour，每帧调用 Core.OnUpdate()
       └─ Hooks             静态实现：Tooltip 注入 / 标签刷新 / 扫描缓存 / 换芯
            ├─ 事件来源       HandleFilter、AddParent/RemoveParent、LoadGame、Tooltip
            ├─ 待办队列       PendingFilters + PendingFilterSet
            ├─ 分帧维护       ScanInventory / ProcessPendingFilters（2ms、64 步/帧）
            └─ 安全交换       CanSwapFilterPlacement → SwapItemsInPlace → Validate → Recover
```

数据流：游戏事件只做轻量入队/标脏 → `UpdateMaintenance` 按帧预算推进枚举 → 扫描建立 3 级优先级的滤芯缓存 → 出队耗尽滤芯 → 先校验再交换 → 校验失败走恢复，恢复不完整则挂起后续换芯并重试。

## 3. 值得保留的设计（优点）

| # | 设计 | 位置 |
| --- | --- | --- |
| S-1 | 只有 9 个 Postfix、无 Transpiler；找不到目标直接 `MissingMethodException`，不静默挂空补丁 | `AquaSense.cs:42-72`、`108-120` |
| S-2 | 延迟到 `PlayerStore.Instance`/`EmporiumEntry.Instance` 就绪后再装补丁，避免提前触发目标类型静态构造；失败 `UnpatchAll` 回滚 | `74-87`、`68-71` |
| S-3 | 维护循环分帧：每帧 2ms / 64 步双上限，扫描内部逐项 `yield return null` | `587-604`、`649`、`672`、`687` |
| S-4 | 换芯前置校验完整：拖拽态、原生 `swapBufferElement`、双向锁、形状可放性 | `810-881` |
| S-5 | 交换前克隆 `modifiedShape`，交换后校验位置与槽位安装 | `916-936` |
| S-6 | 恢复逻辑保守：只接受 parent 为 null/target/source/swapBuffer；先归还旧滤芯释放槽位；失败挂起重试 | `1023-1079`、`1054-1057`、`942-952`、`565-573` |
| S-7 | 诊断可定位：日志带物品标识、`SafeIdentifier` 兜底、`Validate` 异常单独捕获 | `749-759`、`1126-1136` |
| S-8 | Tooltip 去重：用 `slices` 引用判断本帧是否已注入，避免重复行 | `236-289`、`150-151` |
| S-9 | 测试有故障注入（`SwapFault`/`PlaceFault`）并覆盖恢复重试、拖拽延迟、所有权变化 | `AquaSense-Final.Tests/Program.cs` |
| S-10 | 注释解释“为什么”，例如为何不用 `UncheckedAccept`、为何先恢复旧滤芯、为何颜色不能传给 backgroundColor | 全文多处 |

## 4. 风险清单

### P0（建议下次优化优先处理）

**R-01 反射私有成员的静态初始化没有防御**

```csharp
private static readonly FieldInfo GridItems = typeof(GameGridInventory)
    .GetField("items", BindingFlags.Instance | BindingFlags.NonPublic)!;   // 134-135
```

同类还有 `RichTextBuilder.slices`（`254`、`281`）和 `GraphUtils.PlaceAtShape`（`1091`）。

- 现象：`!` 只消除可空警告；一旦游戏更新改掉成员名，`Hooks` 静态构造函数抛 `TypeInitializationException`。
- 影响：装补丁时触发即被捕获并停用 Mod，但日志只报 “Patch initialization failed”，不指明缺失的反射点；若补丁已装上，第一时间可能表现为换芯/托底全部失效。`OnDisable` 里的 `Hooks.ResetMaintenance()`（`95`）不在 try/catch 内，会再抛一次。
- 建议：反射字段改为惰性获取 + 逐项判空；缺失时只降级对应功能（例如禁用自动换芯、跳过 Tooltip 注入），并使用一次性告警说明“缺少哪个成员、哪个功能被禁用”。
- 工作量：约半天（含回归测试）。

**R-02 多选组拖拽未被守卫**

- 现状：`CanSwapFilterPlacement` 只检查 `ItemMouseDragHandler.current`（`814-816`）。
- 背景：1.0.0.5 之后游戏新增 `ItemMultiSelectHandler` 组拖拽（ghost 显示、`IsGroupDragging`；组拖拽结束 `RestoreGhosts()` 会对 `parentInventory == null` 的物品直接 `Destroy()`）。
- 影响：自动换芯若在组拖拽期间执行，可能和 ghost/放置逻辑交叉。概率低，但与“移动物品时物品消失”类反馈的关联度最高，建议补一行 `ItemMultiSelectHandler.current?.IsGroupDragging` 守卫。
- 工作量：约半小时（含回归测试）。

### P1（收益明显、建议排期）

**R-03 Tooltip 热路径存在反射与临时物品创建**

- `AppendScanTooltip` 每次调用都反射读取 `slices`（`254`、`281`）；
- `IsFilterItem → IsAtLeastAsFineAsVanilla → VanillaFilterSize → EnsureVanillaFilterIdentity` 会在悬停渲染路径中构造 `PreBuiltItemHelper.WaterFilter()` 原型再 `Destroy()`（`1292-1312`），失败还会每帧重试。
- 建议：缓存 `FieldInfo`；原型读取提前到装补丁阶段，或直接用 `VanillaFilterSizeFallback` 兜底，热路径零反射、零物品创建。

**R-04 周期全量扫描与重复分配**

- 每 30s 全库存 BFS；`AreItemsStillOwned` 对每个候选都重建 `InventoryRoots()` 的 HashSet（`956-977`、`761-767`）。
- 已有事件钩子（AddParent/RemoveParent、LoadGame）可以减少大部分轮询；至少应在一次扫描内缓存 roots 集合。
- 影响：属性能优化，不影响正确性。

**R-05 少量静默 catch 缺少诊断**

- `AppendFilterDurability` 的 `GetFilterItem` 失败、`GetDisplayName` 失败、`EnsureVanillaFilterIdentity` 失败都直接 return 不记日志（`301-304`、`998-1000`、`1309-1311`）。
- 入口兜底本身合理，但这些是 Mod 冲突排查的第一手信号；建议统一为一次性 WARN，避免刷屏。

**R-06 单文件与静态“上帝类”**

- 1391 行单文件；`Hooks` 约 1260 行、20+ 静态字段，并依赖多个隐式不变量（维护枚举与恢复重试互斥、缓存与库存真实状态一致、帧内 Tooltip 去重）。
- 建议：按功能拆为 Tooltip / InventoryScan / FilterSwap 三个 partial 或内部类，行为不变、可读性提升；同时给不变量加注释或断言入口。
- 风险：纯结构调整，需保证零行为差异；工作量约 1–2 天。

### P2（长期改善）

**R-07 标识符清单与关键字匹配仍在**

- `KnownWaterContainerIdentifiers`（16 个，含 BrewingExpansion 兼容项，`158-178`）、`GetFilterPriority` 的 "advanced/高级" 关键字（`1248-1277`）、`IsWaterScanner` 的 `aquascan/chemscanner`（`409-425`）。
- 滤芯识别已迁移到 `WATER_FILTER` 标签，是正确方向；容器与扫描仪可同样优先用标签/类型判断，标识符仅作 fallback。

**R-08 仓库卫生与版本双改**

- `AquaSense.csproj` 的 `Version 1.8.0` 与 `manifest.xml` 的 `ModVersion 1.8` 手动同步，容易漂移；
- 旧的 `AquaSense.Tests`（指向 IL2CPP 分支）已无产出价值但仍存在；
- 项目根有 `.bak`、`deployment-backups/` 等未跟踪文件。
- 建议：加一个构建期版本一致性校验；归档旧测试工程；`.gitignore` 收纳备份产物。

**R-09 测试覆盖缺口**

- 桩把原生行为简化了（`HandleFilter` 直接返回 amount、`GetFilterItem` 不看标签），因此“耐久真实消耗 → 耗尽 → 触发换芯”这条耦合链没有端到端覆盖。
- 建议：保持现有单元测试，补充实机冒烟清单（换芯、拖拽中换芯、读档后换芯、锁定库存时跳过、紫外线灯/韧棉织物/水培芦苇滤芯三种特殊物品）。

## 5. 后续优化 backlog（按优先级）

| 优先级 | 编号 | 事项 | 预估工作量 | 备注 |
| --- | --- | --- | --- | --- |
| P0 | R-01 | 反射字段防御 + 功能级降级 + 明确日志 | ~0.5 天 | 抗游戏更新第一优先 |
| P0 | R-02 | 多选组拖拽守卫 | ~0.5 小时 | 与物品消失反馈关联度最高 |
| P1 | R-03 | 去热路径反射 / 提前原型读取 | ~0.5 天 | 影响悬停流畅度 |
| P1 | R-04 | 事件驱动刷新 / roots 缓存 | ~1 天 | 可选，视性能实测 |
| P1 | R-05 | 静默 catch 统一为一次性诊断 | ~0.5 天 | 改善冲突排查 |
| P2 | R-06 | 拆分 Tooltip / Scan / Swap | ~1–2 天 | 无行为变更，需回归 |
| P2 | R-07 | 容器/扫描仪标签化识别 | ~1 天 | 需游戏数据核对 |
| P2 | R-08 | 版本校验 + 仓库卫生 | ~0.5 天 | 工程化 |
| P2 | R-09 | 实机冒烟清单 | 持续 | 与每次发版绑定 |

## 6. 与工程规范的符合性

对照 `MOD_DEVELOPMENT.md`：

- ✅ 最小修改、不重写原生逻辑：只做 Postfix 注入与只读判断，库存变更仅通过原生 `GraphUtils.SwapItemsInPlace`。
- ✅ 性能意识：分帧预算、缓存、事件标脏。
- ✅ 不擅自重置配置/存档、不删除玩家数据。
- ✅ 归因谨慎：失败日志保留现场，恢复失败不吞物品引用。
- ⚠️ 入口异常兜底较宽，部分 catch 缺日志（见 R-05）。
- ⚠️ 反射依赖私有成员，游戏更新后需要人工核对（见 R-01）。

## 7. 附录：关键代码位置索引

| 功能 | 位置 |
| --- | --- |
| 配置项 | `AquaSense.cs:18-21` |
| 补丁安装与回滚 | `42-72`、`108-120` |
| 延迟安装 / 每帧入口 | `74-87` |
| 维护循环与分帧预算 | `550-611` |
| 全量扫描 | `633-659` |
| 待处理滤芯队列 | `661-731` |
| 换芯前置校验 | `810-881` |
| 换芯与恢复 | `901-954`、`1023-1093` |
| 滤芯识别 | `1188-1246` |
| 优先级的原版身份判定 | `1248-1290` |
| Tooltip 注入 | `236-347` |
| 测试工程 | `AquaSense-Final.Tests/Program.cs`、`GameStubs.cs` |

## 8. 结论

本 Mod 的核心竞争力是**对危险操作保持克制**：先校验、再执行、失败可恢复，这在社区 Mod 中并不常见。当前不需要推翻重写；下一次迭代按 P0 → P1 顺序加固“反射防御”和“拖拽守卫”，即可显著降低游戏更新后的失效概率与物品异常风险。

---

*本文档为静态审阅结论，未修改任何业务代码；列出的建议需在改动后重新运行回归测试与实机验证。*
