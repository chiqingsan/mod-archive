# 深空当铺 · 更好的性能优化

《深空当铺》(Probably Stolen) 的社区模组，适配正式版 **0.46D**（Mono/ModLoader）。

本模组包含拖拽修复、容器打开、提示框缓存、陈化结算折叠、模块效果去重和空位位图。
离线测试覆盖部分托管逻辑；Unity 内的补丁兼容性仍需实机验证。
全店物品查询、物品视觉刷新和遮罩通知均保持原版处理。
安装后需要重开游戏生效。

## 功能（7 组，默认全开）

1. **拖拽沾手修复**（`ItemMouseDragHandler.StartDrag` 前缀）
   鼠标左键未按下时，原版会把 `ItemMouseDragHandler` 标记为拖拽中且不再复位，导致点击物品后偶发“粘住”、
   拖拽漂移。命中时直接复位该状态并跳过原版开始拖拽逻辑；左键按住时完全走原版。

2. **大容器空位搜索加速**（`GameGridInventory.TryFindOneValidInventorySlot` 前缀）
   先尝试堆叠到已有同类物品；否则把容器内所有物品的占位格烘成一张位图，用它预筛拖拽物品的候选位置，
   只把可能放得下的位置交给原版 `TryInventorySlot` 做权威判定（槽位、旋转、形状检查仍全部由原版完成）。
   库存准入检查、堆叠顺序与优先级保持原版；带子容器穿透回调的库存和滚动库存保持原版搜索；占位图每次查询按当前物品位置与形状重建，不跨调用缓存。

3. **容器冷启动加速 + 打开后布局抑制**（`PixelWindow.Show(Vector2,Vector2,bool)` 前缀/finalizer）
   首次打开容器时，窗口画布尚未挂到 UI 层，原版会先在默认位置构建、再重新布局一次。
   前缀在构建前把画布预挂到 UI 层（含窗口/遮罩/家族层级），finalizer 结束抑制作用域，异常也会清理。

4. **布局刷新抑制**（`LayoutRebuilder.MarkLayoutForRebuild` 前缀）
   容器打开期间只抑制该窗口子树的布局通知；打开后 N 帧内只抑制该子树中隐藏对象的布局重算。异常时通过 finalizer 结束作用域。

5. **中键悬浮预览**（`ItemContextHandler` 中键路径）
   按住鼠标中键扫过容器时直接显示容器预览，不需要打开；预览可见期间可屏蔽提示框（`PreviewHideTooltip`）。

6. **提示框缓存**（`GameItemElement.GetTooltip()` / `GetTooltipAdvanced()` 前缀+后缀）
   鼠标静止时同一物品的提示框 30 帧内复用（帧数可调），拖动/切换物品立即失效。

7. **性能优化**（3 项独立开关，见配置表）
   陈化机按天循环折叠成一次结算、模块动作效果重算去重、
   容器空位位图。物品视觉刷新保持原版同步执行。
   陈化与模块结算开始后如发生异常，保留异常，不再重复执行原版结算。
   全店物品查询不再挂接补丁；空位位图设置关闭时不挂接空位判断方法，修改后需重启游戏。


## 配置

设置 → 模组设置 → 本模组。功能开关默认开启，改动需重启游戏；诊断默认关闭，保存设置后立即生效。

| 配置项 | 分区 | 默认 | 说明 |
| --- | --- | --- | --- |
| `FixGhostDrag` | 拖拽 | 开 | 修复点击后偶发粘住拖拽 |
| `Fix4Enabled` | 拖拽 | 开 | 大容器空位搜索加速 |
| `FastContainerOpen` | 容器 | 开 | 容器冷启动预挂载 + 打开后布局抑制 |
| `SkipLayoutMark` | 容器 | 开 | 打开容器后跳过重复布局标记 |
| `StickyLayoutFrames` | 容器 | 2 | 打开后抑制布局的帧数（0–8） |
| `MiddleClickPreview` | 中键预览 | 开 | 中键悬浮预览容器 |
| `PreviewHideTooltip` | 中键预览 | 开 | 预览期间不弹提示框 |
| `TooltipCache` | 提示框 | 开 | 复用静止鼠标下的提示框结果 |
| `TooltipCacheFrames` | 提示框 | 30 | 缓存帧数（1–120） |
| `AgingCollapse` | 性能优化 | 开 | 陈化机按天循环折叠为一次结算 |
| `ModuleRecomputeDedup` | 性能优化 | 开 | 模块动作触发时效果只结算一次 |
| `GridOccupancyShortcut` | 性能优化 | 开 | 用占用位图加速容器空位判断 |
| `DiagnosticsEnabled` | 诊断 | 关 | 性能与异常诊断，每 10 秒汇总，开关立即生效 |

## 诊断

在“诊断”分区开启 `DiagnosticsEnabled` 后，现有游戏日志中会出现 `[诊断]` 行：

- `FindSlot`：网格找位；`WindowShow`：窗口打开；`Aging`：陈化结算；`Module`：模块动作结算；`Occupancy`：空位判断。
- 每 10 秒按探针汇总调用次数、平均/最大耗时、传播到 finalizer 的异常数量及本周期首次异常堆栈；空闲周期不输出统计。
- 同时统计找位准入拒绝、子容器穿透回原版、找位和空位位图异常回退次数。
- 补丁安装后输出一次 `GameItem.ModifyTag` 与 `ModuleEffectHelper.ResetAllTempStat` 的 Harmony owner 快照；若之后才开启诊断，则在开启后输出，重新开关可刷新。`none` 表示快照中没有 Harmony owner，不代表已验证 Mono 原生入口健康。
- 测量范围从本模组 prefix 到 finalizer，包含执行到的原版方法和其他模组补丁；陈化可能包含模块结算，不能把这些耗时相加。
- 关闭诊断或禁用模组时输出剩余统计；关闭后不读取诊断计时器、不累计样本。诊断不开新文件、不增加权限或新的游戏方法挂接点。

日志由游戏写入 `%USERPROFILE%/AppData/LocalLow/Questing Goose Studio/Probably Stolen/Player.log`。
只覆盖已挂接的上述路径，不能单凭这些统计判断其他方法的故障根因，也不能替代 Unity/Mono 实机验证。

## 安装 / 卸载

- 安装：把 `BetterPerformance.dll`、`manifest.xml`、`preview.png` 放到
  `游戏目录/Mods/BetterPerformance/`，启动游戏。
- 卸载：删除该目录，重启游戏。

## 说明

- 只改运行时行为，不自行写盘或创建诊断文件，不修改存档格式；诊断通过现有游戏日志输出，存档与官方版本互通。
- 优化条件不满足时放行原版；有状态的结算开始后不使用重跑原版作为异常回退。
- 与其它模组兼容性未做穷举；如果和某个模组冲突，可在设置中逐项关闭定位。
- 版本 1.1.0 移除了 v1.0.x 里自创的优化项（找位加速缓存、射线缓存、结算批处理、化学标签缓存等），
  原因见仓库内 `OPTIMOD_RESEARCH.md`：它们缺少运行时等价验证，且多次引发异常与物品异常。

## 构建

```
dotnet build -c Release
dotnet run -c Release --project ../XGDragFixer.Tests/XGDragFixer.Tests.csproj
```

## 版本与部署记录

- **1.3.12**（2026-11-07 构建，发布候选）：去取证版本，用于本地部署验证与后续工坊更新。
  移除 1.3.10/1.3.11 的临时取证补丁与 `ForensicsEnabled` 配置（`Forensics.cs` 已从工程删除，备份在 `D:\code\mod\tmp\bp-forensics-20261106\`），
  保留 1.3.9 的三项修复（销毁容器物品时释放隐藏窗口、搅拌机混合液体合并批次、化学精炼机压片合并形状校验）与原有全部优化；
  离线回归通过。1.3.10/1.3.11 为临时诊断版本，未部署、未上传工坊。
  DLL `c039e964d59b0f6e429f2a02f097f6f756451671ac343639c74a05b0fd1e4920`（`47104` B，AssemblyVersion 1.3.12.0），manifest 1.3.12，仅 `reflection-external`。

- **1.3.11**（2026-11-06 构建，取证诊断增强版，未部署、未上传工坊）：在 1.3.10 取证基础上补上读档链的异常捕获。
  1.3.10 实机复现发现：第一次读档正常，重进存档时 `SaveManager.DecodeNodes` 抛 NRE（`DecodeSaveItem` → `LoadGame_Patch7` → `EmporiumEntry.Start`），
  而旧取证只挂了 postfix，异常时不会执行，因此没抓到具体位置。本版新增：
  `GameItem.Destroy` 之外，为 `PlayerStore.LoadGame`、`SaveManager.DecodeNodes`、`SaveManager.DecodeNodesInternal`、
  `PlayerStore.DecodeSaveItem`、`DirectoryMaster.Item` 增加 finalizer（异常抛出时记录异常、参数与调用栈）；
  `DecodeNodesInternal` 前置检查统一扫描存档节点，报告 `itemState`/`itemShape`/`itemModifiedShape` 为 null 的节点标识与数量；
  重复目录警告改为拦截 `UnityEngine.Debug.LogWarning`（`AddDirectory` 为泛型方法无法挂接），抓取打警告时的调用栈。
  仍然只读、限流（前 20 条详情，之后每 10 秒汇总），不改变游戏行为。
  离线回归通过；Unity 内 finalizer 挂接数量与日志格式需实机确认。
  DLL `8281fc41…`（59,392 B，AssemblyVersion 1.3.11.0），manifest 1.3.11，仅 `reflection-external`；未部署到 Mods、未上传工坊。

- **1.3.10**（2026-11-06 构建，取证诊断版，未部署、未上传工坊）：临时取证版本，用于定位"偶现坏物品 / 营业卡住"的时序来源。
  在“诊断”分区新增 `ForensicsEnabled`（默认开，可即时关闭）。开启时在现有游戏日志中输出 `[取证]` 行，记录：
  读档完成与每个存档节点解码后的坏物品扫描（`state`/`modifiedState` 为 null、`TagSystem.dict` 为 null 或含 null 值，附物品标识）；
  `GameItem.state` / `modifiedState` 被赋 null 时的调用栈；`CustomEventHandler.Update`、`GameItemElement.GetTooltipBasic`、
  `StoreClientList.CreateThief`、`StoreClientListSupplier.CreateThiefMedSeller`、`WaterHelper.TransferLiquid` 的异常上下文
  （EventSystem 状态、场景、物品状态、调用栈）；`DirectoryMaster.AddDirectory` 重复注册时的目录类型与调用栈。
  取证只读取不写入、限流输出（前 20 条详情，之后每 10 秒一条累计汇总），不改变游戏行为；
  复现一次异常后把日志中 `[取证]` 开头的行发回即可定位来源。这是临时诊断版，不应长期使用，定位完成后应关闭开关或回退 1.3.9。
  离线回归通过；Unity 内取证补丁的挂接数量与日志格式需实机确认。
  DLL `3bbb9ddf…`（55,808 B，AssemblyVersion 1.3.10.0），manifest 1.3.10，仅 `reflection-external`；未部署到 Mods、未上传工坊。

- **1.3.9**（2026-11-05 23:40 构建，未部署、未上传工坊）：新增三项容器修复/优化——
  原版 `GameItemElement.Destroy()` 只对 `contentWindow` 调 `Hide()`，窗口 GameObject 及其子树从不释放
  （无 Mod 新开局实测：销毁 60 个空啤酒箱后界面对象 6177 → 7317，约 19 个/箱）；本版在 Destroy 后置补丁中
  确认窗口为空后销毁 `contentWindow.handler.gameObject`，窗口内仍有内容物时保持原版，只记诊断计数。
  搅拌机 `ChemicalHelper.BlendInto` 原版把每种液体分 10 批调用 `AddLiquid`（每批都做温度混合、水特征与
  sprite 刷新）；本版 prefix 按原版取整规则合并为每种液体 1 次写入，总量超过容器剩余容量时回退原版分批截断，
  写入异常时跳过原版避免重复添加。
  化学精炼机压片原版依次执行 `RemoveAllLiquid` / `TransferLiquidToChem` / `IntroduceImpurity`，
  内部每个标签写入都触发一次完整 `ValidateShapeState`；本版在这三个方法执行期间临时跳过形状校验、
  每个方法结束时补一次（压片一次约从 15 次校验降到 3 次），标签数据写入路径完全不变。
  三项均可在设置面板单独关闭，目标缺失或异常时保持原版行为。
  离线验证：分批/合并的总量与温度混合在 3000 组随机输入下等价（取整与线性混合公式对照）；
  压片校验合并为纯校验时序调整（未改数据写入），需实机压片流程验证；
  窗口释放依赖 Unity 对象生命周期，只能实机验证（用同一无 Mod 新开局对象数法对照 6177）。
  未部署到 Mods、未上传工坊；Mods 目录仍为 1.3.8。
  DLL `23f39baa…`（47,104 B，AssemblyVersion 1.3.9.0），manifest 1.3.9，仅 `reflection-external`。

- **1.3.8**（2026-11-05 20:10 本地部署，未上传工坊）：撤掉 `GameItem.ModifyTag` 和 `ModuleEffectHelper.ResetAllTempStat` 的两处挂接，
  删除临时属性跳过校验逻辑及 `TempStatSkip` 设置，恢复原版三次标签写入和中间校验。旧配置中残留的同名键不再读取，无需手工清理。
  此变更用于排除偶发标签入口异常中的本模组挂接因素，尚未证明读档故障根因或通过实机复测；其他结算与拖拽逻辑保持不变。
  诊断开启时只读取上述两处的 Harmony owner，不调用目标方法、不预热入口、不新增挂接或权限。
  离线回归覆盖原版三项临时属性重置及三次校验、诊断开关/快照去重/无新增挂接；仍需完整重启游戏后复测读档与营业。
  DLL、manifest、preview 三件套已同步并核对 SHA-256；DLL `290c2867…`，版本 1.3.8，仅 `reflection-external`。
  旧版三件套已备份至 `deploy-backups/20261105-201033-before-1.3.8/`，不在 Mods 扫描目录内。

- **1.3.7**（2026-11-05 13:50 本地部署，未上传工坊）：修复找位加速堆叠分支遗漏的库存准入检查，
  恢复原版空间排序和槽位优先级；带子容器穿透回调时交回原版完整搜索。
  找位候选坐标恢复原版范围，允许形状右侧/底部空白格越界；回归对照覆盖空白边缘与实际占位格越界拒绝。
  移除 SetTitle/Validate 补丁及其设置，保留原版计算标题前的尺寸校验。
  新增默认关闭、可即时切换的诊断开关，在已有补丁入口计时并汇总异常和回退原因；不新增游戏方法挂接点或文件权限。
  离线回归覆盖拒绝堆叠、堆叠顺序/数量/优先级的原版对照、两种穿透回调的原版放行、诊断开关/定时汇总/异常保留及已有回归；
  Unity 中标题显示、子容器转存与整体营业流程仍需实机验证。
  DLL、manifest、preview 三件套已同步并核对 SHA-256；DLL `27fc6abc…`，版本 1.3.7，仅 `reflection-external`。
  旧版三件套已备份至 `deploy-backups/20261105-135004-before-1.3.7/`，不在 Mods 扫描目录内。

- **1.3.6**（2026-11-05 04:01 本地部署）：移除 `PlayerStore.FindAllItem` / `EmporiumEntry.GetAllItems` 的查询替换补丁、
  遍历缓冲实现及对应设置，恢复原版全店物品查询，保留空位位图修复。
  1.3.5 的 03:51:17 会话出现 30 次 `EmporiumEntry.GetAllItems` 空引用，其中开店在顾客生成阶段中断。
  1.3.4 的设置门控没有移除默认开启的查询补丁；1.3.5 撤掉遮罩补丁也不能解决此查询路径的问题。
  本次移除可疑接管点用于缩小运行时故障范围，不宣称已找到 Harmony/Mono 异常的根因或已通过实机验证。
  DLL、manifest、preview 已同步并逐项核对 SHA-256；DLL `c62c3d68…`（37,888 B），适配 0.46D，仅 `reflection-external`。
  1.3.5 备份：`deploy-backups/20261105-040149-before-1.3.6/`。未上传工坊。
  验证限制：尝试在现有 .NET 6 离线测试中实际安装 Harmony 补丁时，游戏附带 MonoMod 因
  `ILGenerator.MarkSequencePoint` 缺失而失败。已移除不适用此环境的包装实验，保留托管逻辑回归；
  离线通过不能代表 Unity/Mono 的补丁入口通过。

- **1.3.5**（2026-11-05 03:50 本地部署）：撤掉 `MaskUtilities.Notify2DMaskStateChanged` 全局补丁及遮罩通知抑制设置，
  恢复原版遮罩生命周期。03:45:14 会话共 702 次空引用均位于该方法；03:47:24 呼叫下一位顾客时，
  `StoreClientInstance` 创建库存触发该异常，入场流程中断。移除补丁用于排除本模组对该方法的影响，
  仍需同一存档完整重启后复测，尚未证明运行时入口异常的根因。
  已同步 DLL、manifest、preview 并核对 SHA-256；DLL `355aa529…`（40,960 B），适配 0.46D，仅 `reflection-external`。
  1.3.4 备份：`deploy-backups/20261105-035025-before-1.3.5/`。未上传工坊。

- **1.3.4**（2026-11-05 03:42 本地部署）：修正空位位图端点、取消找位旧位图缓存；布局抑制限定在窗口子树，
  纠正隐藏状态判断并补充异常 finalizer；机器窗口设置标题保留唯一一次校验。移除帧末延迟刷新，
  物品视觉刷新恢复同步；陈化与模块结算失败不再重跑或吞掉异常。查询设置关闭时不再挂接对应方法。
  新增空位原版对照、移动后找位、嵌套标题/机器标题、窗口异常清理、模块异常与查询关闭回归测试。
  仍需游戏内验证同一存档的读档、开店、收摊，不能据离线测试宣称营业故障已解决。
  已同步 DLL、manifest、preview 并逐项核对 SHA-256；DLL `6bccb707…`（41,984 B），仅 `reflection-external`，适配 0.46D。
  上版备份：`deploy-backups/20261105-034207-before-1.3.4/`。未上传工坊。

- **1.3.3**（2026-11-04 19:58 本地部署）：修复“陈化结算折叠”从加入起一直未生效的问题。原因是我方扫描条件写成
  “0 参数、返回 `GameItem`”的方法，而游戏里该逻辑位于编译器闭包
  `MachineWineRack+<>c__DisplayClass3_0/<AgeWell>b__5`，签名是 `void(GameItem, GameInventory, SlotMarker)`；
  本版按参考实现硬定位该闭包（命名变化时退回按 IL 特征扫描，找不到仍 `[兼容]` 跳过），prefix 同步改为
  四个闭包字段注入签名。其余功能与 v1.3.2 完全一致。
  DLL `5f76e856…`（43,008 B）、manifest `8f3ff0f8…`（1.3.3，仅 reflection-external）、
  preview `d586868b…` 未变；上一版 v1.3.2 三件套备份在 `deploy-backups/20261104-195744-before-v1.3.3/`。
- **1.3.2**（2026-11-03 01:05 本地部署）：去掉面向玩家的“参考/移植”措辞——配置分区“参考优化”改为
  “性能优化”，功能名、配置说明、模组介绍与日志前缀不再标注来源；功能与行为不变。
  DLL `7c29dbec…`（43,520 B）、manifest `7a4ad05d…`（1.3.2，仅 reflection-external）、preview `d586868b…`
  未变；上一版 v1.3.1 三件套备份在 `deploy-backups/2026-11-03-before-v1.3.2/`。
- **1.3.1**（2026-11-03 00:55 本地部署）：撤掉 v1.3.0 的两项 transpiler（输入动作排序短路、射线结果
  排序短路）。这两项改写了 `InputActionManager.Update`/`EventSystem.RaycastAll` 的 IL，扫描器因此判定本模组
  需要 `dynamic-code`（运行时加载或生成代码）能力，而 manifest 未声明，游戏将其标为 `Changed` 并拒绝加载。
  本版回到只依赖 `reflection-external` 的权限面；其余 6 项优化保留。
  DLL `70a6517b…`（43,520 B）、manifest `ba13d655…`（1.3.1，仅 reflection-external）、preview `d586868b…`
  未变；被拒绝的 v1.3.0 三件套备份在 `deploy-backups/2026-11-03-before-v1.3.1/`。
- **1.3.0**（2026-11-03 00:45 本地部署）：新增 8 项逻辑优化——视觉刷新合并、陈化结算折叠、
  模块效果去重、临时属性跳过校验、空位占用位图、输入动作排序短路、射线结果排序短路、大规模物品列表
  遍历缓冲。全部独立开关、目标缺失或判定不成立时回退原版；本版不包含任何读档期跳过或异常吞并。
  DLL `c33bbdfa…`（47,104 B）、manifest `7fbd5bbf…`（1.3.0，仅 reflection-external）、
  preview `d586868b…` 未变；上一版 v1.2.0 备份在 `deploy-backups/2026-11-03-before-v1.3.0/`。
- **1.2.0**（2026-11-03 00:17 本地部署）：补上大容器空位搜索加速（Fix4），并修正三处
  与原版不一致的地方——SetTitle 作用域不再区分窗口类型；布局抑制在“显示期间”按原版无条件跳过、
  sticky 阶段只跳过隐藏窗口（此前方向相反）。DLL `e90800c2…`（30,720 B）、manifest `fedd31de…`（1.2.0，
  仅 reflection-external）、preview `d586868b…` 未变；v1.1.1 停用时的备份在
  `deploy-backups/2026-11-02-v1.1.1-disabled/`。
- **1.1.1**（2026-11-02 23:49 本地部署）：代码审查整改——拖拽复位反射失败改为记录并放行原版；
  容器冷启动反射缺失时记录并允许重试；布局抑制只作用于正在打开的窗口子树；提示框缓存（TooltipCache）
  增加离线回归测试；设置面板渲染补丁目标缺失时不再反复重试。
  DLL `0ffe4704…`、manifest `a1b960f0…`、preview 未变；上一版 v1.0.15 备份在
  `deploy-backups/2026-11-02-before-v1.1.1/`。
- **1.1.0**：删除无法验证的自创优化项，只保留 6 组补丁。
- **1.0.0**：工坊首发版本。
