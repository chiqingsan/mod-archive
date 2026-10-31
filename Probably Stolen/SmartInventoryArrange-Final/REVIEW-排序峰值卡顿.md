# 「更好的库存排序」峰值卡顿审阅报告

- 审阅对象：`D:\code\mod\Probably Stolen\SmartInventoryArrange-Final`（v1.1.1 工作区，未提交改动包含固定物品修复）
- 审阅日期：2026-10-30
- 审阅方式：源码静态分析（Mod 层 + Core 求解器）+ 反编译游戏代码对照 + 本机运行日志实测
- 结论一句话：**卡顿的主因是「同一件昂贵操作被重复 6~7 遍」+「每次操作的实现是反射式序列化」，不是排序算法慢；后台求解实测极快。**

> 说明：除「实测数据」一节外，本文中的毫秒数为静态估算（基于调用次数 × 单次成本），不是实测值。游戏内各 API 的单价、`ForceRecomputeDynamicBackground` 的真实耗时尚未埋点测量，落地前应按第八节的方案补测量。

---

## 一、审阅范围

| 范围 | 文件 | 行数 |
|---|---|---|
| Mod 入口与协程 | `SmartInventoryArrange/SmartArrangeMod.cs` | 302 |
| 事务/快照/应用 | `SmartInventoryArrange/InventoryTransaction.cs` | 914 |
| 实例排序 | `SmartInventoryArrange/InstanceOrderNative.cs` | 366 |
| 触发补丁 | `SmartInventoryArrange/ContextTriggerPatch.cs` | 46 |
| 全局搜索调度 | `SmartInventoryArrange.Core/GlobalArrangeController.cs` | — |
| 搜索器 | `SmartInventoryArrange.Core/Native34Solver.cs` | 1774 |
| 评分与指标 | `SmartInventoryArrange.Core/DeterministicSolver.cs` | 2058 |
| 稳定序列化 | `StableText.cs`、`SystemTextJsonCompat.cs`、`HashCompat.cs` | 77 / 9 / 15 |
| 游戏侧对照 | `GameGridInventory.current.cs`（反编译） | — |

---

## 二、实测数据（本机日志）

日志位置：`%USERPROFILE%\AppData\LocalLow\Questing Goose Studio\Probably Stolen\SmartInventoryArrange\Logs\SmartInventoryArrange-Snapshot.log`

- 日志大小 1.72 MB，覆盖 **80 次排序**，最后更新 2026-10-30 18:45
- 每次排序另生成 `Inputs\<时间戳>-<guid>.json`，最新约 54 KB
- 容器尺寸分布：24×10（33 次）、20×10（19 次）、18×4（17 次）、7×5（5 次）、6×4（2 次）、10×10（2 次）、8×6（1 次）、3×3（1 次）
- 最大物品数 **51**，固定物品最多 10 件
- 结果分布：applied 65、kept-original 11、cancelled-before-apply 4

| 指标 | p50 | p90 | max |
|---|---|---|---|
| 后台实例排序 `InstanceOrderingMs` | 0.53 ms | 8.55 ms | 13.69 ms |
| 捕获完成 → 应用完成（点击到结果） | 140.5 ms | 357 ms | 424 ms |

**解读：**

1. 后台实例排序不是卡顿来源（毫秒级）。
2. 点击到结果最长 424 ms、p50 140 ms，量级与「主线程 100~250 ms 同步工作」的静态估算吻合。
3. 日志只记录了「捕获后」到「应用后」的间隔，**没有覆盖点击帧本身和应用帧内部**，所以日志里看不到「卡住」的帧，只能看到等待时长。玩家感知的峰值卡顿就落在这两段未记录区间。

---

## 三、主线程时间线

```
点击 TriggerButton（ContextTriggerPatch.cs:11-27，同步，返回 false）
 └─ Run()（SmartArrangeMod.cs:54-132）
     ├─ InventoryTransaction.Capture()                     全物品遍历 + 6~7 遍指纹
     ├─ WriteDiagnosticSnapshot()（:80）                   同步追加日志
     ├─ WriteExactInput()（:81）                           同步序列化 + 写 54KB JSON
     ├─ ProblemSignature()（GlobalArrangeController.cs:40）反射序列化 + SHA256
     ├─ Search.Begin()（:113）                             CreateSession 全量重建
     └─ Runner.RunCoroutine(SolveAndApply)（:124）         启动协程
          ├─ Task.Run：Advance / Evaluate / OrderInstances 后台，不卡帧
          ├─ while (!task.IsCompleted) yield return null   等待（主线程空闲）
          ├─ AssertSnapshotCurrent()（:163）               又一遍全物品指纹
          ├─ Search.Complete()（:233）                     再算一次 ProblemSignature
          ├─ tx.Apply()（:230）                            守卫×2 + 游戏校验 + 全格重刷
          └─ WriteResultSnapshot()（:232）                 同步序列化 + 追加写
```

---

## 四、根因分析

### 4.1 `Fingerprint` 一次点击算 6~7 遍（主因）

`InventoryTransaction.Fingerprint`（`InventoryTransaction.cs:785-797`）→ `InstanceOrderNative.FingerprintSuffix`（`InstanceOrderNative.cs:358-365`），主线程调用点：

| # | 位置 | 范围 | 性质 |
|---|---|---|---|
| 1 | `InventoryTransaction.cs:538`（`Observe` ← `:216`） | 全部物品 | 捕获时计算 |
| 2 | `:246`（构造 `Snap`） | 可移动物品 | 与 #1 重复 |
| 3 | `:101`（`SessionContext`） | 全部物品 | 与 #1 重复 |
| 4 | `:576`（`AssertSnapshotCurrent` ← `:288`） | 全部物品 | 捕获后同帧守卫 |
| 5 | `:576`（← `SmartArrangeMod.cs:163`） | 全部物品 | 应用前守卫（必要） |
| 6 | `:767`（`VerifyUnchanged` ← `Apply:654`） | 可移动物品 | 应用前守卫（必要） |
| 7 | `:767`（← `Apply:707`） | 可移动物品 | **与 #6 重复** |
| 8 | `:611`（`AssertFixedSnapshotCurrent` ← `Apply:708`） | 固定物品 | 应用后守卫 |

其中 #2/#3 与 #1 重复，#4 与 #1 是同一帧冗余，#7 与 #6 重复。

### 4.2 每次指纹的常数极大（根因的核心）

`FingerprintSuffix` 的实现：

```
JsonSerializer.Serialize(new { order = Read(item, ""), stateIntegrity = StateIntegrity(item) })
```

看起来普通，但本项目把 `JsonSerializer` **替换成了反射实现**：

- `SystemTextJsonCompat.cs:5-9` 在 `System.Text.Json` 命名空间内定义了同名 `JsonSerializer`，转调 `StableText.Serialize`；
- `StableText.cs:45-56`：每个对象都 `GetType().GetMembers(...)` → `Where().Where().OrderBy().ToArray()`（4 个 LINQ 迭代器 + 一次数组分配）→ 逐成员 `FieldInfo/PropertyInfo.GetValue` **装箱** → `AppendString` 逐字符拼接；每次调用还 `new StringBuilder()` + `new HashSet<object>`（`:13-14`）。

因此每一层匿名对象、每一个 `Orientation`/`ShapeMask`/`Cell`/`TagState` 都走完整反射。叠加：

- `Read()`（`:153-323`）：
  - `Features()`（`:88-151`）对每个 feature 做一次反射序列化（`:123-135`），再排序 + SHA256（`:149`）；
  - `Rank()`（`:308-322`）被调用约 10 次（`:230`、`:253`×4、`:273`、`:276`、`:281`、`:298`、`:299`、`:302`、`:303`、`:304`），每次 `ToArray(Where)` + `ToArray(Select)` + `All` + `Max`；
  - `SemanticConditions.Index(id, params string[])` 每次调用分配 `params` 数组；
  - 约 30~40 次 tag 访问，`Tag()`（`:14-23`）是 `ContainsKey` + `[]` 双查；
  - `item.GetDisplayName(true)`（`:222`）。
- `StateIntegrity()`（`:325-356`）：遍历 `state.dict` 全部 tag，**每个 tag 一次反射序列化**（`:337-352`），最后排序 + 序列化 + SHA256。
- `HashCompat.Sha256Hex()`（`HashCompat.cs:6-11`）每次 `SHA256.Create()` + `BitConverter.ToString().Replace("-","")`。

**单次指纹估算 0.1~0.3 ms**；6~7 遍 × 51 件 ≈ 30~100 ms（100 件时 60~200 ms）。

### 4.3 同步写盘与诊断产物

| 位置 | 工作 | 估算 |
|---|---|---|
| `InventoryTransaction.cs:386` | `WriteExactInput`：54 KB JSON 序列化 + `File.WriteAllText` | 10~25 ms |
| `:432` | `WriteDiagnosticSnapshot`：拼 512+N×160 字节 + `File.AppendAllText`（日志已 1.7 MB） | 1~4 ms |
| `:510` | `WriteResultSnapshot`：坐标三重排序 + 每物品实例 JSON 序列化 + 追加 | 3~8 ms |
| `:87-108` | `SessionContext`：再叠一遍全物品指纹 + JSON + SHA256 | 3~6 ms |

这些是调试产物，玩家发布版本不需要每次点击都同步写。

### 4.4 `ProblemSignature` 每次点击算两遍

`GlobalArrangeController.cs:84-123` 把 W×H 全部 usable 格、fixedItems、items、orientations、categoryOrder 用 `StableText.Serialize` 反射序列化后 SHA256；`:40`（Begin）与 `:58`（Complete）各算一次。估算 10~20 ms。

### 4.5 应用帧的游戏侧开销

`InventoryTransaction.Apply`（`:620-722`）在结果出现那一帧串行执行：

- `VerifyUnchanged()`（`:654`）全物品指纹；
- 逐物品 `GridShapeBuilder.Build`（`:665`）+ `CheckShapePlacement`（`:666`，游戏实现 `items.All` + `Intersects`）；
- 逐物品设置 `modifiedShape`（`:686`）+ `CheckInventorySlotStillValid`（`:690`，同样 `items.All`）；
- `((GameInventory)_inventory).Validate()`（`:705`）；
- `ForceRecomputeDynamicBackground()`（`:706`，游戏侧 `ValidateBackground` 对 `(W+2)×(H+2)` 全部格子调用 `SetGridFillBackground`）；
- `VerifyUnchanged(ignoreShape: true)`（`:707`，重复）；
- `AssertFixedSnapshotCurrent()`（`:708`，固定物品再一遍指纹）。

估算应用帧 30~80 ms，其中 `Validate`/`ForceRecomputeDynamicBackground` 是游戏方法，没有分段接口，不能拆帧。

### 4.6 后台 GC 尖峰（次要）

搜索初始预算 `InitialNodeBudget = 250000`（`GlobalArrangeController.cs:25`），每个搜索节点估算 2~15 KB 分配（`Native34Solver.cs:662/680/673/695-698/717` 等）。后台线程与主线程共用 GC 堆，分配尖峰会以 GC 停顿的形式落在主线程帧上。当前实测都因 `SolutionBatch = 64`（`:31`）提前退出而未跑满 250k，但大仓库可能触发。

### 4.7 为什么现在才明显

v1.1.1 的「固定物品不被移动」修复把 `AssertFixedSnapshotCurrent` 从条件调用改成无条件（`:705-708`），应用帧多了一整遍固定物品指纹；同时每次点击都写诊断文件（`Inputs` 目录持续增长、日志 1.7 MB 追加）。这两点叠加在原本就有的重复指纹上，放大了峰值。

---

## 五、修复点清单

> 本节是首轮审阅方案。复审后：R2、R6、R8 已否决（事务守卫/一致性职责），R1 收窄为仅在捕获循环内复用指纹；R3 按“默认关闭详细诊断、保留错误日志”实施；实际落地与验证见第十节。

### P0-1 R1：捕获阶段只算一次指纹

- **位置**：`InventoryTransaction.cs:101 / :216 / :246 / :288 / :538`；`InstanceOrderNative.cs:358-365`
- **问题**：`Observe`（`:216`）、`Snap`（`:246`）、`SessionContext`（`:101`）三处对同一物品重复计算指纹。
- **改法（按复审意见收窄）**：在 `Capture` 内建 `Dictionary<IntPtr, string> fingerprints`，循环里 `Observe` 算一次并保存，`Snap` 与 `SessionContext` 复用；不删除任何守卫。
- **保留**：`:288`（捕获后守卫）、`:163`（应用前）与 `Apply` 内两次 `VerifyUnchanged` 全部保留真实重算；捕获循环与守卫之间是否有回调修改物品，须由调用方/实机确认后再考虑进一步合并。
- **预计收益**：仅捕获帧少算约 2N 次指纹；不是原估的跨帧收益，实际值以 perf 埋点为准。
- **风险**：低（缓存生命周期限于单次 Capture；守卫语义不变）

### P0-2 R2【已否决，不实施】：保留 `Apply:707` 的重复校验

- **位置**：`InventoryTransaction.cs:707`
- **原方案**：`:654` 已做过 `VerifyUnchanged`，删除 `:707`。
- **复审否决**：`:707` 位于 `:705 Validate` + `:706 ForceRecomputeDynamicBackground` 之后，负责检查应用过程是否改动数量、固定状态或指纹，是事务一致性守卫；保留。

### P0-3 R3【已实施】：诊断默认关闭，错误日志保留

- **位置**：`SmartArrangeMod.cs:80 / :81`；`InventoryTransaction.cs:386 / :432 / :510`
- **问题**：每次点击同步写 54 KB JSON、追加 1.7 MB 日志、再追加结果块。
- **改法（已实施）**：新增配置 `VerboseDiagnostics`，默认关闭；关闭时 `WriteDiagnosticSnapshot`、`WriteExactInput`、`WriteResultSnapshot` 都不执行；错误/守卫日志照常输出，不改成后台写，避免改变错误时序。
- **预计收益**：约 20~45 ms（写盘与序列化），以 perf 埋点实测为准。
- **风险**：低（诊断产物；开启开关后行为与旧版一致）

### P0-4 R4【已实施】：`AssertSnapshotCurrent` 避免重复访问 `childItems`

- **位置**：`InventoryTransaction.cs:547 / :549`
- **问题**：游戏 `GameInventory.childItems` 每次访问都 `new List<GameItem>(items)` 复制（`GameGridInventory.current.cs:50`），循环条件与取元素各访问一次 → O(N²) 分配/拷贝。
- **改法**：进入循环前 `var items = ((GameInventory)_inventory).childItems;` 只取一次。
- **预计收益**：1~3 ms（51 件）
- **风险**：极低

### P0-5 R5：`Apply` 中的 `Single` 改字典

- **位置**：`InventoryTransaction.cs:664`
- **问题**：`:662` 循环内 `Enumerable.Single(placements, ...)` → O(N²)，5050 次比较（100 件）。
- **改法**：先建 `Dictionary<string, Placement>`（`StringComparer.Ordinal`）。
- **预计收益**：约 0.05 ms（顺手修，非必需）
- **风险**：极低

### P0-6 R6【已否决，不实施】：`ProblemSignature` 保留 Begin/Complete 各一次

- **位置**：`GlobalArrangeController.cs:40 / :58 / :84-123`
- **原方案**：Begin 算一次存入会话，Complete 复用。
- **复审否决**：Complete 重算签名本身是“输入是否变化”的一致性检查；`LayoutProblem` 内部 `bool[,]` 和集合内容仍可能被修改，缓存签名会削弱该检查。

### P1-1 R7：捕获阶段分帧（2 ms 预算）

- **位置**：`InventoryTransaction.cs:111-290`（改造为可由协程切片执行）
- **问题**：Capture 一次性在点击帧完成。
- **改法**：按物品分块，每帧软预算 2 ms、硬帽 4 ms；块大小 1~16 件自适应（见第六节）。
- **预计收益**：点击帧从 80 ms 级降到 ≤2 ms/帧；总时长增加约 30~60 ms
- **风险**：中（跨帧需守卫；`Capture` 当前是静态/一次性完成，需要改造状态机）
- **触发条件**：仅当 R1~R3 落地后点击帧仍 >30 ms 时启用

### P1-2 R8【已否决，不实施】：应用前守卫滚动校验

- **位置**：`SmartArrangeMod.cs:163`；`InventoryTransaction.cs:541-588`
- **问题**：应用前守卫一次性算全部物品指纹。
- **改法**：把守卫拆成游标，在等待后台求解的帧里每帧 2 ms 逐物品比对；发现差异立即中止；守卫队列清空的同一帧进入提交（保证「检查完到写状态之间没有事件循环」）。
- **复审否决**：滚动校验只能保证“最后一项检查完到提交”之间无事件循环，不能保证整个守卫窗口；语义不完整，不作为本轮方案。

### P1-3 R9：`Apply` 预校验提前

- **位置**：`InventoryTransaction.cs:664-666`（构造 `GridShape` + `CheckShapePlacement`）
- **问题**：应用帧串行做只读校验。
- **改法**：求解结果出来后，在等待帧里分摊预计算；提交帧只做工写入 + 最终复核。
- **预计收益**：应用帧 -10~30 ms
- **风险**：中高（预校验结果需最终复核；拆分不当会出现中间态）

### P2-1 R10：后台搜索分批 / 时间片

- **位置**：`GlobalArrangeController.cs:25`；`SmartArrangeMod.cs:143-150`；`Native34Solver.cs:176`
- **问题**：一次 250k 节点预算、每节点 2~15 KB 分配，GC 尖峰可能落在主线程帧。
- **改法**：`InitialNodeBudget` 250000 → 20000~30000，协程连续 `Advance`；`:176` 增加 `val.Elapsed.TotalMilliseconds < 12` 时间片；输出已有埋点 `Native34Solver.cs:386` 的 `Performance`（目前全项目无人读取）。
- **预计收益**：削 GC 尖峰；不减少总量
- **风险**：中（状态机时序）
- **触发条件**：实测 GC 停顿或大仓库（200+ 物品）才需要

### P2-2 R11：削减每批固定开销

- **位置**：`Native34Solver.cs:233 / :237`；`GlobalArrangeController.cs:225-228 / :343 / :359`
- **问题**：每批尾 `ValidateLayout`、256 条 `CandidateTrace` 每条一次 SHA256、`_rankCache`/`_metricCache` 只增不清。
- **改法**：CandidateTrace 降频/开关；缓存按会话清理。
- **预计收益**：后台总时长（主线程无直接收益）
- **风险**：低中

### P3-1 R12：指纹去反射（根治）

- **位置**：`StableText.cs:45-56`、`InstanceOrderNative.cs:88-151 / :325-356 / :358-365`
- **问题**：所有序列化走反射 + 装箱 + 排序。
- **改法**：手写固定字段顺序的 `StringBuilder`（或 `Span<byte>` + 一次哈希）替代 `StableText`；Feature/StateIntegrity 直接拼串而非反射匿名对象。
- **预计收益**：单次指纹 0.1~0.3 ms → 0.01~0.03 ms；R1 之后仍可再省 10~25 ms
- **风险**：中（必须保证哈希稳定性，测试覆盖）

### P3-2 R13：死代码/未接入路径标注

- `DeterministicSolver.Solve`（`DeterministicSolver.cs:366`）全项目无调用点；
- `CompareForProblem`（`:316`）每次比较都经 `:243` 跑 `DensityMode→OccupancyRatio`（O(W·H)/次），当前不可达；
- `ImproveCompleteLayout`（`:1234`）、`WineBottleOrder.Apply`（`WineBottleOrder.cs:71-127`）无调用点。
- **建议**：加注释或断言标注「未接入主流程」，避免未来接入后退化为 O(N log N × W·H) 比较。

### 不建议做

- 拆分 `Apply` 的写状态循环（`:686`）：会产生物品中间态、画面闪动与重叠，语义风险远大于收益。
- 拆分游戏 `Validate()` / `ForceRecomputeDynamicBackground()`：游戏方法无分段接口。
- 把游戏对象读取（`GameItem`、`TagSystem`）整体移到后台线程：Unity/IL2CPP 对象线程安全未验证，风险高。

---

## 六、尖峰分摊设计（若采用 P1）

### 6.1 预算参数

| 参数 | 建议值 | 说明 |
|---|---|---|
| 软预算 | 2.0 ms/帧 | 60 fps 下 16.7 ms 预算，给 mod 的同步工作上限约占 12% |
| 硬帽 | 4.0 ms/块 | 单件超 4 ms 说明该物品指纹异常（大量 tag/液体），记录日志后继续 |
| 块大小 | 1~16 件自适应 | 初始 4 件；超软预算减半，低于 25% 翻倍 |
| 计量 | `Stopwatch` | 不要用 `Time.deltaTime`（上一帧值，不准） |

### 6.2 调度器骨架

```csharp
private static readonly Stopwatch FrameClock = new();

private static bool BudgetLeft(double softMs = 2.0)
    => FrameClock.Elapsed.TotalMilliseconds < softMs;

// 分块工作：预算内多干，超了就下一帧
int step = 4;
while (cursor < count)
{
    int n = Math.Min(step, count - cursor);
    for (int k = 0; k < n; k++) CaptureOne(cursor++);

    double ms = FrameClock.Elapsed.TotalMilliseconds;
    if (ms >= 2.0) { step = Math.Max(1, step / 2); yield return null; FrameClock.Restart(); }
    else if (ms < 0.5) step = Math.Min(16, step * 2);
}
```

### 6.3 使用原则

1. **优先利用已有空闲帧**：后台求解等待期（`SmartArrangeMod.cs:147-150`）本来空转，把只读校验塞进去，总延迟几乎不增加。
2. **原子提交**：`modifiedShape` 写入 + `Validate` + 重刷背景必须同一帧完成；守卫队列必须在提交前的最后一帧清空。
3. **不追加全局帧预算**：Unity 没有共享帧预算 API，只约束本 mod 自己的同步循环。
4. **失败处理**：跨帧期间检测到库存变化即中止并回滚；面板关闭/库存失效/`OnDisable` 均取消协程并复位 `Busy`。
5. **代价**：分摊会把「点击 → 结果」从 140 ms 级延长到 170~220 ms 级，换来单帧不超过 4 ms；是否接受取决于更在意「卡一下」还是「慢一点」。

---

## 七、验证方案

1. **埋点（已实施）**：`PerfTrace` 在 `Run()` / `SolveAndApply()` / `InventoryTransaction.Apply` 内记录阶段耗时、GC 次数与分配量，输出一行 `Smart arrange perf: ...`。
2. **已有埋点**：`Native34Solver.cs:386` 的 `Performance` 属性当前无人读取，接一行日志即可得到每批耗时与分配字节。
3. **对比口径（已按复审修正）**：
   - 用本轮新增的 `Smart arrange perf:` 日志拆出 `capture / writeDiagnostic / writeInput / fingerprint / begin / backgroundWait / preApplyGuard / apply* / applyFrame / clickFrame / total / gc0-2 / allocKB`；
   - 同一存档、同一库存连续排序 5 次，保留原始日志行；
   - 主线程帧峰值不要用 `Time.frameCount` 推算，使用 Unity Profiler 或外部帧时间工具，并与 perf 日志的阶段耗时应答；
   - 点击→结果间隔只能作为端到端参考，不能直接当成主线程单帧耗时。
4. **验收标准（建议，待实测校准）**：
   - 点击帧同步工作降到可感知阈值以下（先以实测基线为准，不预设毫秒数）；
   - 应用帧中 `applyBackground`、`applyVerifyAfter`、`preApplyGuard` 不再出现异常尖峰；
   - 默认关闭诊断后不再有同步磁盘 IO；开启后仍能复现旧版诊断内容。
5. **回归**：固定物品不被移动（v1.1.1 修复）必须复测；空库存、库存被外部改变时取消、排序期间切换/关闭面板，均按现有行为验证；“保留容器模式”已移除，不再作为回归项。
6. **不夸大**：静态估算、编译通过、模拟测试均不能替代实机帧时间测量；在拿到实测前，不对外宣称「零卡顿」或具体毫秒数。

---

## 八、实施顺序（按复审意见调整）

| 顺序 | 内容 | 状态 |
|---|---|---|
| 1 | 阶段埋点：`PerfTrace` + 捕获/写盘/指纹/求解等待/守卫/应用/Validate/背景刷新/GC | 已实施 |
| 2 | R4 `childItems` 快照（仅捕获/守卫读取一次） | 已实施 |
| 3 | R3 详细诊断默认关闭（`VerboseDiagnostics`，错误日志保留） | 已实施 |
| 4 | R1 有条件：仅捕获循环内复用指纹；应用前/后守卫保留 | 已实施 |
| 5 | 移除“保留容器模式”（按钮、配置、代码分支、README） | 已实施 |
| 6 | 实机测基线：点击帧/应用帧/GC/各阶段，同一存档重复测量 | 已测 45 件；1000 件待测 |
| 7 | 指纹缓存：`InstanceOrderNative.Signature` 快速签名 + `ConditionalWeakTable` 缓存完整指纹，前 64 次命中做慢速对照审计 | 已实施（实测冷启动指纹 55 ms，热缓存 0.5~4 ms，审计 mismatches=0） |
| 8 | 分帧捕获：超过 16 件时按自适应帧预算拆分捕获与快照守卫，每 `max(250, N/8)` 件输出进度日志；`Arrange` 用 `DriveOne` 手动驱动 enumerator，未分帧时不分帧等待 | 已实施（实测 93 件捕获跨 4–9 帧；预算 = 帧时间 × 35%，夹 1–6 ms） |
| 9 | 后台 Begin：`ValidateProblem`/指标/`SessionContext`/`Search.Begin`（ProblemSignature + CreateSession）移入 `Task.Run`；`Complete` 复用请求签名 | 已实施（实测后台等待 18–60 ms；冷启动 fresh 仍约 0.5 s） |
| 10 | 1000+ 件防御性上限：单帧峰值受自适应预算（≤6 ms）限制，总耗时随件数线性增长 | 设计已覆盖；数百件已实测（93 件点击总等待约 0.1 s、冷启动约 0.6 s），1000 件待有存档时实测 |
| 11 | R5（`Single`→字典）、R9、R10~R12 | 帧率已平稳，收尾不再实施 |
| 12 | R2/R6/R8 按复审意见不实施 | 已否决 |

> 说明：本轮在“先测基线”之前就落地了 R1/R3/R4，因为写盘与捕获复用会改变后续阶段的绝对耗时；当前 perf 日志记录的是“修复后”的分段数据，旧版基线只能参考历史日志中的点击→结果间隔，不能逐阶段对比。若必须取得严格的前后对照，可用备份 DLL 回退再测一轮。

---

## 九、不确定性与未验证项

1. 游戏 API 单价（`GameItem` getter、`TagSystem` 字典、`GetDisplayName`、`CheckShapePlacement`）未单独测量，估算基于调用次数与反射开销；
2. `ForceRecomputeDynamicBackground` 实测 `applyBackground=0.01~0.04 ms`，不是瓶颈；
3. GC 已分阶段归因：`captureGc=0`、`applyGc=0`，GC 全部发生在后台搜索窗口（`backgroundGc`）；单次 GC 暂停时长未单独测量；
4. 日志中的「点击到结果」间隔不等于「点击帧卡顿时长」；分帧后 `clickFrame` 是跨帧墙钟，不能当单帧耗时用；
5. 估算区间基于 51~100 件、24×10 级仓库；数百件属于常见场景，已按 6 ms 预算分帧覆盖，但尚未实测；1000+ 件属防御性上限（常规库存不会出现，叠加其他扩展 Mod 才可能达到），不专门优化。

---

## 十、实施记录（2026-10-30）

- 新增 `SmartInventoryArrange/PerfTrace.cs`：每次排序输出一行 `Smart arrange perf:`，包含各阶段毫秒、`fingerprint#调用次数`、`total`、`gc0/1/2`、`allocKB`。
- `SmartArrangeMod.cs`：`Run`/`SolveAndApply` 加阶段埋点；新增 `VerboseDiagnostics`（默认 `false`）控制详细诊断写盘（1.2.1 已整体移除，见第八轮）。
- `InventoryTransaction.cs`：`Capture` 内指纹复用（`Observe` 算一次，`Snap`/`SessionContext` 复用）；`AssertSnapshotCurrent` 只取一次 `childItems`；`Apply` 内各子阶段计时；移除保留容器模式相关字段/分支/窗口存在性检查；`Fingerprint` 计数埋点。
- `NativeSmartMenu.cs` / `ContextTriggerPatch.cs`：删除“保留容器排列”按钮，排序菜单由 1×7 恢复为 1×6。
- `README.md`：移除保留容器说明，补充诊断开关与 perf 日志说明。
- 构建与测试：`dotnet build -c Release` 通过；`SmartInventoryArrange-Final.Tests` 输出 `PASS fixed position reserved while movable items are arranged`。
- 第二轮（指纹缓存）：`InstanceOrderNative.Signature` 快速签名 + `InventoryTransaction.Fingerprint` 的 `ConditionalWeakTable` 缓存；`StableText` 成员缓存；`HashCompat` 复用 `SHA256` 实例；前 64 次命中做慢速对照审计，实测 `mismatches=0`。冷启动指纹从 178.6 ms 降至 55.3 ms，热缓存 0.5~4.4 ms。
- 第三轮（分帧捕获 + 后台 Begin）：`InventoryTransaction.CaptureCoroutine` / `AssertSnapshotCurrentCoroutine` 按 6 ms 预算分帧（超过 16 件启用），`SmartArrangeMod.Arrange` 协程化；`Arrange` 用 `DriveOne` 手动驱动 enumerator，只在内部真正分帧时才让出帧，避免嵌套协程固定多等一帧；`ValidateProblem`、指标、`SessionContext`、`Search.Begin` 移入后台 `Task.Run`；`GlobalArrangeController.Complete` 不再重算 `ProblemSignature`，改用请求签名比对。
- 实测（45 件，指纹缓存版）：冷启动 `clickFrame=110.0 ms`（其中指纹 55.3 ms、`capture=84.1 ms`）、`begin=24.4 ms`、`applyFrame=11.2 ms`；热缓存 `clickFrame=3~11 ms`、`applyFrame=1~11 ms`。
- 第四轮（守卫误判修复，2026-10-30 夜）：实测 76 件库存出现一次 `before-apply` 守卫取消，差异物品为两瓶瓶装水，`order` 字段完全一致、仅 `stateIntegrity` 不同（隐藏 tag 变化，如其他 Mod/游戏刷新标签值），而排序只依赖 `order` 字段。`InstanceOrderNative.FingerprintSuffix` 因此改为只序列化 `Read` 的排序字段，去掉全量 tag 哈希：守卫不再因隐藏状态抖动而误取消，同时冷启动指纹成本进一步下降。
- 第五轮（分配削减 A+B+C）：`SnapshotConsistency.AssertSame` 改为先比较、仅差异时拼字符串；删除死代码 `Native34Solver.CandidateRecords`；`InstanceOrderNative.ReadCached` 按 `Signature` 缓存 `InstanceOrderInfo`。实测 93 件冷启动分配 12.75→6.09 MB，`instanceOrderHits#92`，守卫尖峰消失。
- 第六轮（GC 归因）：`GC.GetAllocatedBytesForCurrentThread()` 在 Unity Mono 恒返回 0，分配计量方案失败；改为 `captureGc/backgroundGc/applyGc` 分阶段统计，确认主线程捕获与应用阶段 0 次 GC，GC 全部发生在后台搜索窗口。
- 第七轮（自适应预算 + resume batch 16）：分帧预算改为当前帧时间的 35%（夹 1–6 ms），`ResumeSolutionBatch=16`。实测 93 件：后台等待 140–180→18–60 ms，GC 2–3→0–1 次，总耗时 150–210→54–122 ms；`captureFrames` 1–2→4–9，单帧占用下降，`clickFrame` 墙钟相应变长。
- 第八轮（1.2.1：移除诊断写盘与 `files` 权限）：排序本身不读写文件，`files` 权限只为默认关闭的诊断快照服务。删除 `WriteExactInput`/`WriteDiagnosticSnapshot`/`WriteResultSnapshot`、`DiagnosticItem`/`_diagnostics`/`_capturedModels`、`VerboseDiagnostics` 配置项与全部调用点；`CaptureCoroutine` 不再构造诊断列表与 `capturedModels`；`manifest.xml` 仅保留 `reflection-external`。性能日志仍走 Unity `Player.log`（`Debug.Log`，不需要 `files` 权限）。构建通过、测试 PASS。
- 最终部署：1.2.1 Release DLL `ce6f44cd…`；Mods 目录已替换（备份 `SmartInventoryArrange-before-diagnostics-removal.dll.bak` = 1.2.0 的 `97f08e08…`）；工坊已上传 1.2.1（ManifestID `5774019150558647888`），下载回验 manifest `<ModVersion>1.2.1`、DLL 哈希一致、封面未变。
- 用户实测反馈：帧率平稳；93 件仓库点击到出结果约 0.3 s 量级（含游戏侧动画与感知），比官方整理明显更好。收尾，不再改代码。
- 未做：R2/R6/R8（复审否决）；R5、R9、R10~R12（帧率已平稳，收尾）；1000 件大仓库实测未做；不宣称“零卡顿”。

---

## 附录 A：关键调用点索引

> 行号为审阅时版本；本轮重构后 `InventoryTransaction.cs` / `SmartArrangeMod.cs` 行号已大幅变化，附录改为按函数名定位。

| 函数 | 位置 |
|---|---|
| `SmartArrangeMod.Run` / `Arrange` / `DriveOne` | `SmartArrangeMod.cs`（按函数名定位） |
| `SmartArrangeMod.RunBackground` / `ApplyOrKeep` / `HandleFailure` | 同上 |
| `InventoryTransaction.CaptureCoroutine` | `InventoryTransaction.cs` |
| `InventoryTransaction.AssertSnapshotCurrentCoroutine` | 同上 |
| `InventoryTransaction.BuildSessionContext` | 同上 |
| `InventoryTransaction.Fingerprint` / `FingerprintSlow` | 同上 |
| `InventoryTransaction.Apply` / `VerifyUnchanged` | 同上 |
| `InstanceOrderNative.Signature` / `FingerprintSuffix` / `Read` / `StateIntegrity` | `InstanceOrderNative.cs` |
| `GlobalArrangeController.Begin` / `Complete` / `ProblemSignature` | `GlobalArrangeController.cs` |
| `StableText.Serialize` / `HashCompat.Sha256Hex` | 项目根 |
| `PerfTrace.Flush` | `SmartInventoryArrange/PerfTrace.cs` |
