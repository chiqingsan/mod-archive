# 深空当铺 · 更好的性能优化（BetterPerformance）

正式版《深空当铺》(Probably Stolen) 0.46D 的原生 ModLoader 模组，把 Nexus 上 Demo 版的
[XG Drag Fixer](https://www.nexusmods.com/probablystolen/mods/405)（作者 798924038）针对
Mono / Harmony 环境独立重写，并加入针对正式版原生逻辑的性能优化。原 mod 文件不包含在本工程中，
也未做任何转载。

- 模组 ID：`chiqingsan.BetterPerformance`
- 程序集：`BetterPerformance.dll`
- 部署目录：`Mods/BetterPerformance/`
- 配置文件：`ModConfigs/chiqingsan.BetterPerformance.json`
- 能力：仅 `reflection-external`（不再需要 `files`）
- 发布版本：`1.0.0`（首发版本号；内部迭代记录保留在下方章节）

## 功能

| 配置项 | 默认 | 说明 |
| --- | --- | --- |
| `FixGhostDrag` | 开 | 修复点击/双击物品后偶发“粘住拖拽”：`StartDrag` 触发时左键已松开则重置拖拽状态 |
| `FastSlotFind` | 开 | 覆写 `GameGridInventory` 与 `GameGridScrollableInventory.TryFindOneValidInventorySlot`：用占用位图预筛候选方向/位置，只对通过预筛的候选调用原版 `TryInventorySlot`。搜索顺序、排序、堆叠、穿透（容器套容器）规则与原版一致 |
| `FastContainerOpen` | 开 | 容器首次打开前预挂载浮动窗口画布/停靠层（`UIFamilyLayer.EnsureCanvas` / `PixelWindow.EnsureFloatingDock` / `UIFamilyLayer.Adopt`） |
| `SkipContainerLayoutMark` | 开 | 打开容器期间（及之后 `SkipLayoutMarkFrames` 帧）跳过窗口子树内冗余的 `LayoutRebuilder.MarkLayoutForRebuild` |
| `SkipLayoutMarkFrames` | 2 | 容器打开后继续抑制布局重建的帧数（0–8） |
| `SkipMaskNotify` | 开 | 打开容器期间跳过窗口子树内的 `MaskUtilities.Notify2DMaskStateChanged` |
| `SetTitleSkipValidate` | 开 | `PixelWindow.SetTitle` 内重复的第一次 `Validate()` 跳过 |
| `MiddleClickPreview` | 开 | 按住鼠标中键悬浮在容器物品上快速预览内容；松开中键后预览窗口保持打开（与原 mod 一致） |
| `PreviewHideTooltip` | 开 | 预览时隐藏物品提示框 |
| `TooltipCache` | 开 | 缓存同一物品的基础/高级提示框 `RichText`，避免停住时每帧重算容器价值 |
| `TooltipCacheFrames` | 30 | 提示框缓存帧数（1–120） |
| `TooltipIdleSkip` | 开 | 提示框文本、位置、宽度在连续 `TooltipIdleFrames` 帧内未变时，跳过 `ShowTooltip` 里的重复重设/校验 |
| `TooltipIdleFrames` | 6 | 允许连续跳过重复刷新的最大帧数（1–30） |
| `RaycastFrameCache` | 开 | 同一帧内相同位置/指针的 `EventSystem.RaycastAll` 直接复用结果（提示框、拖拽、右键菜单等多处每帧射线检测） |
| `ContextMenuSkip` | 关 | 物品右键菜单打开且鼠标未移动时，跳过菜单高亮重绘（鼠标静止最多 60 帧后自动重新校验；实测跳过率仅 0.6%，默认关闭） |
| `NightValidateBatch` | 开 | 过夜结算期间合并库存界面刷新，结算结束后统一刷新一次 |
| `NightSimUIQuiet` | 开 | 夜间化学模拟期间跳过纯界面的文字/布局刷新（模拟结束后正常刷新，不改变任何结算结果；修改后需重启生效） |
| `ChemTagCache` | 开 | 化学模拟期间缓存容器标签读取，避免每次读取都克隆 `TagState`：任何标签写入立即失效，不改变模拟结果（修改后需重启生效） |
| `AgingBatch` | 开 | 睡眠时酒架“n 格 × m 天”的陈化调用合并为每件物品一次刷新，最终天数与价值变化与原版一致 |
| `MachineModuleSkip` | 开 | 机器生效时跳过每个模块内部重复的全量效果重算，只保留 `OnMachineActioned` 末尾的最终一次 |
| `SpriteRefreshGuard` | 开 | `UpdateModSprite` 目标状态与当前贴图一致时跳过重复写入（外观动画帧不受影响） |
| `SlotIndexDirect` | 开 | `GetIncompleteItemInventorySlot` / `GetItemInventorySlot` 直接扫描私有 items 列表，避免每次复制 `childItems` |
| `SkipStoreHoverRaycast` | 开 | 商店界面悬停检测（`StoreUIManager.CheckRaycast`）在鼠标完全静止且无任何输入时跳过重复的 4–5 次全量射线；最多连续跳过 `StoreHoverIdleFrames` 帧后强制重算 |
| `StoreHoverIdleFrames` | 15 | 鼠标静止时连续跳过商店悬停检测的最大帧数（1–60） |
| `TextFieldFocusCache` | 开 | `StoreUIManager.IsTextFieldFocused` 每帧只计算一次 |
| `SizeFitterGuard` | 开 | `AdvancedContentSizeFitter` 的尺寸只在首次生效，避免每帧 `SetSizeWithCurrentAnchors` 标脏布局 |
| `StartupFaultGuard` | 开 | 读档时游戏处于后台，界面对象可能比逻辑晚一帧就绪并连锁抛 `NullReferenceException`（清空容器提示框、库存校验、物品标签校验等），这里只吞掉这类空引用异常并计数写日志，避免一次异常打断整帧初始化导致容器打不开/卡死；其它异常照常抛出（需重启） |
| `LoadFocusBoost` | 开 | 读档窗口期（启动后 60 秒、每次场景加载后 30 秒）内游戏处于后台时临时解除“后台限帧 10 FPS”，保证加载期逻辑帧率，结束后把帧率交还给游戏设置（需重启） |

### 诊断调试（默认关闭，普通游玩请勿开启）

| 配置项 | 默认 | 说明 |
| --- | --- | --- |
| `PerfDiagnostics` | 关 | 性能诊断：统计每帧耗时、分配、GC 与各补丁调用/跳过，定期写入游戏日志。仅供排查测试，会增加少量开销 |
| `PerfReportSeconds` | 10 | 报告间隔秒数（2–120） |
| `PerfABMode` | `Hotkey` | A/B 切换方式：`Hotkey` / `Auto` / `Both` / `Off`；仅诊断开启时生效 |
| `PerfABKey` | `F8` | 手动切换 A/B 的按键（`F1`–`F12`） |
| `PerfAutoABSeconds` | 300 | 自动切换间隔（30–3600），300 = 每 5 分钟 |

以上诊断设置均为即时生效：在设置里改完立刻应用并弹出提示，无需重启。

## 构建

需要 .NET SDK 6+。游戏目录可用 `-p:GameDirectory=...` 覆盖（默认
`D:\app\steam\steamapps\common\Probably Stolen`）。

```powershell
dotnet build XGDragFixer.csproj -c Release
```

产物：`bin/Release/netstandard2.1/BetterPerformance.dll`，与 `manifest.xml`、`preview.png`
一起放入 `Mods/BetterPerformance/`。

## 目录结构

```
XGDragFixer.csproj      # 工程文件（AssemblyName=BetterPerformance）
manifest.xml            # ModLoader 清单
Core.cs                 # IMod 入口、配置、延迟安装补丁、Runner 驱动
Hooks.cs                # Harmony 前后缀入口（名字与 Core 中的注册保持一致）
FastSlotFind.cs         # 空位搜索覆写 + 占用位图缓存（固定格 / 滚动格）
SlotFit.cs              # 纯函数候选预筛（可离线测试）
LayoutSuppressor.cs     # 打开容器期间的布局/遮罩抑制状态机
ContainerShowFix.cs     # 冷启动预挂载、SetTitle 去重、窗口可见性查询
HoverPeek.cs            # 中键悬浮预览
TooltipCache.cs         # 提示框缓存
TooltipIdleSkip.cs      # 提示框空闲跳过重复刷新
RaycastFrameCache.cs    # 同帧 UI 射线检测缓存
ContextMenuSkip.cs      # 右键菜单静止跳过
AgingBatch.cs           # 睡眠陈化合并刷新
MachineBatch.cs         # 机器模块效果跳过重复重算
SpriteGuards.cs         # 贴图写入守卫
SlotIndexDirect.cs      # 槽位索引直查
StoreHoverCache.cs      # 商店悬停检测跳过（鼠标静止）
TextFieldFocusCache.cs  # 输入框焦点每帧缓存
SizeFitterGuard.cs      # 尺寸自适应只应用一次
NightValidateBatch.cs   # 过夜结算期间合并库存界面刷新
NightSimQuiet.cs        # 化学模拟期间的界面静默
ChemBreakdown.cs        # 化学模拟子阶段计时
ChemTagCache.cs         # 化学模拟标签读取缓存（避免 TagState 克隆）
NightBreakdown.cs       # 过夜结算细分诊断
PerfGate.cs             # A/B 总开关
Toast.cs                # 提示（游戏原生通知条，回退到自绘覆盖层）
ConfigValueLabels.cs    # 模组设置滑条数值显示
PerfDiagnostics.cs      # 可选性能诊断（日志输出每补丁计数/耗时 + 帧时间/分配）
```

## 离线测试

`../XGDragFixer.Tests` 复用 `SlotFit.cs`，用反编译的 `GridShape.ForwardTransform` /
`InverseTransform` 与 `TryInventorySlot` 的相交规则做差分验证（2 万次往返 + 5 万次随机
形状/占用差分 + 手选边界用例）：

```powershell
dotnet run --project ../XGDragFixer.Tests/XGDragFixer.Tests.csproj -c Release
```

### 离线微基准（空位搜索）

```powershell
dotnet run --project ../XGDragFixer.Tests/XGDragFixer.Tests.csproj -c Release -- --bench
```

在 20×20 库存、大件物品的模拟数据上对比“原版逐候选检查”（包围盒快拒绝 + 重叠格比较）与
“占用位图预筛”的搜索 CPU 工作量，输出候选数、包围盒/格子检查次数、首个可放置位置是否一致
与耗时倍数。注意：这是按反编译条件模拟的原版代价模型，不包含 Unity 侧真实分配，用于证明
预筛确实减少了计算量，不等价于帧时间收益。

## 验证性能收益（实机 A/B，日志版）

诊断默认关闭，避免平时影响性能。最省事的方式是用内置的 A/B 切换：**一次正常游玩**即可同时
采集“补丁全开”和“补丁全关”两种状态的样本。

1. 游戏内 设置 → 模组设置 → 深空当铺 · 更好的性能优化 → **诊断调试** → 把 `PerfDiagnostics`
   打开（会弹出提示确认开启）；
2. 选择切换方式（诊断调试分组，或直接改 JSON）：

   | `PerfABMode` | 行为 |
   | --- | --- |
   | `Hotkey`（默认） | 按 `PerfABKey`（默认 `F8`）手动切换全开/全关 |
   | `Auto` | 每 `PerfAutoABSeconds` 秒自动切换一次（默认 300 秒 = 5 分钟） |
   | `Both` | 自动切换，同时热键可以提前切换并重置计时 |
   | `Off` | 不切换（只记录当前状态） |

3. 正常游玩即可（例如打烊结算、拖物品、开容器、滚动库存、商店买卖各来几轮），不需要固定脚本；
4. 每次报告间隔（默认 10 秒）游戏日志会输出：

   - `[性能诊断] #N ab=全开/全关 fps=… p50=… p95=… p99=… >33ms=… 分配=…KB/s GC0=… 补丁耗时=…ms`；
   - `[性能诊断] 明细 #N：FastSlot=…次/跳过…%/…ms；…`（每补丁调用次数、跳过率、自身耗时）。

   对比 `ab=全开` 与 `ab=全关` 的 `p50/p95/p99`、`>33ms` 长帧数、分配量、GC0，看分布差异
   而不是单次 FPS；`skip_pct` 长期接近 0 而调用次数很高的补丁可能是负收益。

切换时屏幕顶部会弹出游戏原生通知条（与 AquaSense 等本地 mod 相同，走
`StoreUIManager.Instance.Notify`），不用看日志也能确认状态。

> 诊断模式自身会增加少量耗时（计数器 + 每帧采样），只用于对比测量；正式游玩时请保持关闭。
> A/B 切换时清空当前帧缓存，避免两种状态的样本互相污染。

## 注意

- 所有功能都可单独关闭；`FastSlotFind` 遇到位图无法表示的形状（形状值 ≥ 32、库存形状缺失）
  时自动回退到原版实现。
- 与 `SmartInventoryArrange`（库存排序）、`QuickSell` 等现有 mod 无补丁目标重叠。
- 从旧名称（`chiqingsan.SmoothDragAccel` / `Mods/SmoothDragAccel/`）升级时，旧配置文件
  `chiqingsan.SmoothDragAccel.json` 不会自动迁移，新配置会重置为默认值。

## 过夜结算（v1.6.0 起）

夜间结算 `PlayerStore.EndNight` 是游戏里单次耗时最长的原生流程（实测约 3.9 秒）。

1. **合并库存界面刷新**（`NightValidateBatch`，默认开启）：结算期间每次物品状态变化都会触发
   `物品.ValidateShapeState() → 所属库存.Validate()`，整场结算会把同一批库存重复刷新上百次。
   现在把这些刷新记下来，结算结束后统一刷新一次，可见结果一致，设置里可关闭。
2. **结算细分诊断**（在 `PerfDiagnostics` 开启时自动生效）：结算结束后游戏日志输出一行
   `[结算诊断] 过夜结算细分`，按耗时列出每个原生阶段（化学模拟、冷库、漏斗、物品扫描、
   物品刷新、结算 Handler 集合等）以及库存刷新被合并的次数/耗时。

### v1.7.0：化学模拟细分与夜间界面静默

实测（2026-11-01 会话）过夜结算总耗时约 3.57 秒，其中 **化学模拟 3.34 秒（94%）**，
其余阶段合计不到 0.2 秒。化学模拟是原版 `RunNightlyChemicalSimulation()` 对每个
“每秒回调”物品连续执行 360 次（相当于把一晚 6 分钟拆成 360 个模拟秒），机器回调里
的大头是**不可见的界面刷新**：加热板的温度/标签文字、化学完成机的布局 `Validate()`、
模式选择按钮状态等，每模拟一秒都完整刷新一遍。

v1.7.0 的“夜间界面静默”（`NightSimUIQuiet`，默认开启）：

- 在 `RunNightlyChemicalSimulation()` 执行期间给界面刷新加静默标记；
- 期间跳过 `TMP_Text.SetText` 的所有重载、`MachineryStack.Validate()`、
  `MachineryRadioList.Select()` 这类纯显示刷新；
- 化学数值计算（环境降温、温度反应、加热、冷库降温）完全照常执行，结果不变；
- 这些回调本来每个正常游戏秒都会再执行一次，所以窗口打开后显示立即恢复正确；
- 修改该开关需重启游戏生效。

同时新增“化学模拟细分”诊断（诊断开启时自动输出），把化学模拟拆成
`环境降温 / 温度反应 / 冷库降温` 三个原生方法的耗时与调用次数。

### v1.8.0 / v1.8.1：化学标签读取缓存

v1.7.0 的实机结算细分：总 3974ms，其中化学模拟 3740ms（`温度反应` 3279ms / 26640 次、
`环境降温` 379ms / 26640 次）；`TMP_Text.SetText` 静默命中 0 次（机器标签不走这条路径），
只跳过了 9000 次机器布局刷新，所以 v1.7.0 对化学模拟几乎无帮助。

继续追原版实现，找到真正的开销：

1. 每模拟一秒，`ChemicalHelper.containerTempTick` 执行 `HandleAmbientTempDrop` +
   `HandleTemperatureReaction`；后者串联检查 23 个反应 handler；
2. 每个 handler 入口都调用 `WaterHelper.IsEmpty(容器)`，而它为了判断“液体是否为空”会遍历
   全部 36 种液体、逐个读取 `GetTagReadonly(液体对应的 tag)`；
3. 原版 `GameItem.GetTagReadonly(string)` 的实现是 `return modifiedState.GetTag(name).Clone();`
   —— **每次读取都新建一个 `TagState` 并复制字段**；
4. 于是单个液体容器每模拟一秒要做约 24 次 `IsEmpty` × 36 次读取 ≈ 800 多次克隆；整晚
   74 个液体容器 × 360 秒 ≈ 2000 万次克隆，这就是化学模拟那 3.7 秒的来源。

v1.8.0 的“化学标签读取缓存”（`ChemTagCache`，默认开启）：

- 缓存 `GetTagReadonly` 与 `IsEmpty` 的结果；
- 缓存只消除“重复读取同一份未改动数据”的克隆开销，不改模拟步数、不改任何反应判断、
  不改变结算结果；
- 诊断开启时日志会追加 `化学标签缓存命中 X / 未命中 Y`；
- 修改该开关需重启游戏生效。

v1.8.0 第一版每结束一个容器 tick 就清空缓存，实测命中率只有 **53.5%**（命中 4,994,640 /
未命中 4,341,960），化学模拟仅从 4.5s 降到 3.0s。

v1.8.1 把缓存按“容器”维度保留到整场结算结束，只在真正会改变读取结果的地方失效：
`GetTagReadonly` 返回的是 `modifiedState` 的克隆，而 `modifiedState` 只在原版
`GameItem.ValidateShapeState()` 里重建 —— 因此只在该方法执行时丢弃该容器的克隆缓存；
只改 live state 的写入（`skipValidate`）不会改变 `GetTagReadonly` 的返回值，因此只重置
`IsEmpty` 这一项。

### v1.9.0：改名、游戏原生提示条、诊断只写日志

- 模组改名为 **深空当铺 · 更好的性能优化**（ID `chiqingsan.BetterPerformance`，
  程序集 `BetterPerformance.dll`，部署目录 `Mods/BetterPerformance/`）；
- A/B 切换、诊断开启等提示改用游戏原生通知条（`StoreUIManager.Instance.Notify`，
  与 AquaSense 等本地 mod 一致），异常时回退到自绘覆盖层；
- 诊断不再写 CSV 文件，所有统计改为写游戏日志；因此 manifest 移除 `files` 权限，
  只保留 `reflection-external`；
- 设置分组“诊断”改名为“诊断调试”，并标注“仅供测试排查，普通游玩请勿开启”；
- 全部配置说明精简。

### v1.9.1：读档期稳定性

2026-11-01 凌晨连续出现“进入游戏后容器打不开、重启恢复”“点击容器无反应后回主菜单再读档卡死”。
日志（`GameLog_2026-11-01_03-43-59.txt`，174,189 行）里同一秒内 `PixelWindow.Validate`、`GameGridInventory.Validate`、`EmporiumEntry.Validate`、`GameItem.EnableTag` 总计抛了 25,178 个 `NullReferenceException`：这些异常发生在读档瞬间，游戏会记录每一个并中断当帧后续初始化，`PlayerStore` 构造因此每帧重试，编辑器日志被写爆后表现为卡死。
复现场景与用户观察一致：**读档时游戏窗口在后台**。而游戏自带“窗口失焦时降到 10 FPS”的选项（`EmporiumOption.ApplyFpsCap`），后台读档会让加载期的逻辑帧严重不足，部分纯界面对象比其宿主晚一帧就绪。

v1.9.1 增加两个默认开启的稳定性选项（都在“稳定性”分组，修改后需重启）：

- `StartupFaultGuard`：对 `GameGridInventory/GameGridScrollableInventory/GameSlotInventory/PixelWindow.Validate`、`EmporiumEntry.Validate`、`GameItem.EnableTag/ValidateShapeState/SetGameItemType`、`WaterHelper.UpdateContainerGoodType` 加 Harmony finalizer，只吞 `NullReferenceException` / `MissingReferenceException`，首次按方法名记一次警告、每 30 秒汇总一次计数；
- `LoadFocusBoost`：启动后 60 秒内、每次 `SceneManager.sceneLoaded` 后 30 秒内，若窗口失焦则把 `targetFrameRate` 临时抬到 60（`vSyncCount=0`），窗口恢复焦点或窗口期结束时调用游戏自己的 `EmporiumOption.ApplyFpsCap` 把限帧交还。

两者都不会改变任何结算/库存数据；关掉后行为与 v1.9.0 完全一致。

另外修复一处 v1.9.0 的界面回归：模组设置面板如果从**主菜单**（存档尚未载入）打开，滑条左侧的数值不再显示——
因为本模组此前要等场景里商店就绪才安装补丁。现在 `ConfigValueLabels` 会在模组页 UI 真正存在时
（`ModUIManager.Instance` 就绪）安装补丁，主菜单与游戏内打开设置面板都能看到数值。

### v1.9.2：设置面板补丁不再在 `OnEnable` 里提前安装

v1.9.1 首次修复主菜单设置数值时，把 `ConfigValueLabels` 的安装放进了 `OnEnable`。但 `OnEnable` 是在
`ModLoader.Awake` 内部执行的，此时游戏资源与本地化尚未就绪，提前给界面类型打补丁可能触发目标的静态构造、
进而影响界面文字渲染。v1.9.2 改为在 `ModRunner.Update` 里轮询：只有 `ModUIManager.Instance` 已存在
（模组页 UI 已就绪、设置面板确实可以打开）时才安装，仍比等商店就绪早，但已离开启动期最危险的时间窗。
若主菜单一路没出现模组页，也会在商店就绪后的常规安装阶段补上。

同一版还修了提示框的一个回归：鼠标停在物品上不动时，提示框内容会一直停在旧值，必须把鼠标移开再移回来才刷新。
原因是 `TooltipCache` 与 `TooltipIdleSkip` 两个补丁都用前缀拦截了原方法，但 Harmony 在前缀返回 false 时
**仍会执行后缀**，后缀又把“上次刷新帧号/缓存时间戳”更新成当前帧，导致超时永远不触发、缓存永不过期。
v1.9.2 让两个后缀只在原方法真正执行过时才更新时间戳（缓存命中则跳过），并把 `TooltipIdleSkip` 的默认值改为关；
即使手动打开，提示框内容也会在约 `TooltipIdleFrames` 帧内重新刷新，不会再卡住。

### v1.9.3：设置面板补丁改为主菜单就绪后安装

实测发现 v1.9.2 的轮询条件（`ModUIManager.Instance != null`）在正常流程里几乎不会满足：`ModUIManager`
的实例要等玩家打开“模组”页面才会 Awake，而设置面板正是在那个页面里打开的，等于补丁永远晚一步。v1.9.3
改为在主菜单 UI 就绪（`MainMenuUIController.Instance != null`）时就安装，仍处于游戏资源/本地化加载完成之后、
早于任何可能打开设置面板的时机；不再依赖模组页是否被打开过。

同一版本实测了一次过夜结算（2026-11-01 06:06，开启性能诊断）：总耗时 **1872.6ms**，其中化学模拟
1583.3ms（温度反应 1343.7ms / 23760 次、环境降温 165.4ms / 23760 次），化学标签缓存命中 99.7%；
库存刷新抑制 1808 次、实际只刷新 82 个；`NightValidateBatch` 合并了 1581 次 Validate；夜间静默跳过
9000 次机器布局。对比 v1.6 的 3565ms、v1.7 的 3974ms、v1.8.1 的约 3000ms，已是目前最好的一次。

## 权限声明

manifest 只声明 `reflection-external`：Harmony 补丁安装与少量私有成员访问（冷启动预挂载、
滚动库存私有字段、化学模拟委托等）。不再写入任何文件。

首次启用时游戏模组列表可能弹出能力确认，确认后才会加载。

## 本地部署记录

> 2026-11-01 03:xx 起改名为 BetterPerformance，旧目录 `Mods/SmoothDragAccel/` 在部署新版时
> 应删除，新目录为 `Mods/BetterPerformance/`。

- 2026-10-31：v1.3.0 部署到 `Mods/SmoothDragAccel/`（`SmoothDragAccel.dll` + `manifest.xml` + `preview.png`）。
- 2026-10-31：v1.5.0 部署（DLL SHA256 `db29e9ef…`、manifest 1.5.0、preview 未变；v1.3.0 备份在
  `XGDragFixer/deploy-backups/2026-10-31-v1.3.0/`）。
- 2026-11-01：v1.6.0 部署（DLL SHA256 `2e3bdce9…`、manifest 1.6.0、preview 未变；v1.5.0 备份在
  `XGDragFixer/deploy-backups/2026-11-01-v1.5.0/`）。
- 2026-11-01：v1.7.0 部署（DLL SHA256 `c4f5ccf0…`、78,848 B；manifest 1.7.0、`fc4c1755…`；preview 未变；v1.6.0 备份在
  `XGDragFixer/deploy-backups/2026-11-01-v1.6.0/`）。
- 2026-11-01：v1.8.0 部署（DLL SHA256 `2ba2e92c…`、81,920 B；manifest 1.8.0、`9a9c2b1b…`；preview 未变；v1.7.0 备份在
  `XGDragFixer/deploy-backups/2026-11-01-v1.7.0/`）。
- 2026-11-01：v1.8.1 部署（DLL SHA256 `33d99855…`、82,432 B；manifest 1.8.1、`f9c583d4…`；preview 未变；v1.8.0 备份在
  `XGDragFixer/deploy-backups/2026-11-01-v1.8.0/`）。
- 2026-11-01 03:30：**v1.9.0 改名后重新部署**到 `Mods/BetterPerformance/`（DLL `BetterPerformance.dll`
  80,896 B SHA256 `bcab3f2b…`；manifest `1b4bbeff…`，ID `chiqingsan.BetterPerformance`、仅
  `reflection-external`；preview `d586868b…` 1280×720 508 KB）。旧目录 `Mods/SmoothDragAccel/` 已删除，
  其 v1.8.1 三件套备份在 `XGDragFixer/deploy-backups/2026-11-01-v1.8.1/`。旧配置
  `chiqingsan.SmoothDragAccel.json` 未迁移，新 ID 配置从默认值开始（诊断默认关闭）。
- 2026-11-01 04:16：**v1.9.1 部署**（DLL `BetterPerformance.dll` 86,016 B SHA256 `dca4c11b…`；
  manifest 1.9.1 `158dfaae…`；preview 未变 `d586868b…`）。v1.9.0 三件套备份在
  `XGDragFixer/deploy-backups/2026-11-01-v1.9.0/`。本次包含读档期异常兜底、后台限帧临时解除，
  以及主菜单设置面板数值显示修复。
- 2026-11-01 05:27：**v1.9.2 部署**（DLL `BetterPerformance.dll` 92,160 B SHA256 `92eced76…`；
  manifest 1.9.2 `5d19c014…`；preview 未变 `d586868b…`）。v1.9.1 三件套备份在
  `XGDragFixer/deploy-backups/2026-11-01-v1.9.1/`。本次包含设置面板补丁改到 `ModUIManager` 就绪后安装、
  提示框缓存时间戳修复（悬停时内容不再冻结）、`TooltipIdleSkip` 默认关闭。
- 2026-11-01 06:24：**v1.9.3 部署**（DLL `BetterPerformance.dll` 92,160 B SHA256 `eeb660f7…`；
  manifest 1.9.3 `c6e1a87e…`；preview 未变 `d586868b…`）。v1.9.2 三件套备份在
  `XGDragFixer/deploy-backups/2026-11-01-v1.9.2/`。本次把设置面板补丁的安装时机改为主菜单 UI 就绪
  （`MainMenuUIController.Instance != null`），修复“主菜单打开设置看不到滑条数值”。
- 2026-11-01 06:45：**v1.0.0（首发版本号）部署**（DLL `BetterPerformance.dll` 86,016 B SHA256
  `f6c01a98…`，Release 构建；manifest 1.0.0 `d147490f…`（869 B，简短版 `<Description>`，无署名）；
  preview 未变 `d586868b…`）。上一版 Debug 1.9.3 三件套备份在
  `XGDragFixer/deploy-backups/2026-11-01-v1.9.3-debug/`。对外发布版本号统一重置为 1.0.0，
  内部迭代记录保留在上方章节。
- 2026-11-01 06:52：**首发上传 Steam 创意工坊**，Workshop ID `3831227725`
  （https://steamcommunity.com/sharedfiles/filedetails/?id=3831227725），公开可见；
  内容为 `BetterPerformance-1.0.0-Content/` 三件套 + 封面 `BetterPerformance-cover.jpg`。
  工坊登记已写入 `WorkshopUpload/README.md`；后续更新使用最小 VDF
  `BetterPerformance-3831227725-1.0.0.vdf`（只含 appid/publishedfileid/contentfolder）。
