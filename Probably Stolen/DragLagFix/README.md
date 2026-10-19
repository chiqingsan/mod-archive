# DragLagFix

缓解《Probably Stolen》往容器（尤其是快满的容器）拖放物品时的卡顿。

## 原理

反编译分析（见 `../decompiled/Game/`）表明卡顿来自拖拽悬停期间游戏每帧重新执行
`ItemMouseDragHandler.RefreshDragHighlight()` → `TryInventorySlot` → `CheckShapePlacement`
的穷举式放置检测：容器越满越慢，而"放不下"恰好是穷举的最坏情况，且没有任何缓存。

本 mod 不改判定逻辑本身，只减少重复计算：拖拽中悬停状态（拖拽物品、目标容器、
鼠标所在屏幕格、内容未过期）未变化时跳过一次高亮重算，静止悬停按
`StaticRefreshIntervalMs` 兜底刷新（防止容器内容被其他 mod/NPC 改动后高亮过期）。
放置执行仍完全由游戏自身逻辑完成，本 mod 不介入任何放置/移动判定结果。

## 配置（MelonPreferences，category: DragLagFix）

| 条目 | 默认 | 说明 |
| --- | --- | --- |
| `SkipStaticDragRefresh` | `true` | 悬停状态未变化时跳过重复高亮重算 |
| `StaticRefreshIntervalMs` | `100` | 悬停静止时兜底刷新间隔（毫秒） |
| `CacheFindSlotResults` | `true` | 缓存找空位结果（失败结果也缓存） |
| `FindSlotCacheTtlMs` | `800` | 找空位缓存过期时间（毫秒） |
| `ProbeHotPaths` | `false` | 输出热点方法耗时日志（排查用） |
| `FrameStats` | `true` | 每 10 秒输出帧时间统计（排查用） |

## 找空位结果缓存（v1.0.3）

探针实测（2026-09-28）确认最大热点：批量转移/搬运循环逐件调用
`TryFindOneValidInventorySlot`，对着近满容器每次都是 40-85ms 的穷举失败，
最差单次 326ms——这才是"往快满容器放物品很卡"的主要来源（悬停重算已被节流解决）。

缓存 key 为（容器, 物品, keepOrientation），命中条件：未过期且
`inventory.childItems.Count` 与缓存时一致（容器内容变化几乎必然改变该计数）。
"找不到空位"的失败结果同样缓存——失败穷举恰是最贵的路径。过期结果的最坏
影响是放置方被游戏自身校验拒绝后回退，不会损坏数据。

## 探针

`ProbeHotPaths` 开启后计时（self-time，单次 ≥5ms 记录，前缀 `[probe]`，每秒最多 20 条）：

- `ItemMouseDragHandler.RefreshDragHighlight`（与节流 patch 共存，耗时即每次高亮重算的真实成本）
- `GameGridInventory` / `GameGridScrollableInventory` 的 `TryFindOneValidInventorySlot`
- `ContainerHelper.CreateContainerTooltip`

**注意**：探针只挂外层低频方法。内层热方法（`TryInventorySlot`、`CheckShapePlacement`
在一次穷举中被游戏内部调用数千次）不能挂 Harmony patch——native detour 每次调用的
托管转换开销会把穷举成本放大一个数量级，v1.0.0 因此反而更卡，已移除。
UI 层方法同理：`ResizePixels`/`ValidateBackground` 名义上是物品增删才调用，
实际是每帧级热路径，v1.0.5 挂探针导致常态变卡，v1.0.6 回滚。

## 构建

```powershell
dotnet build .\DragLagFix.csproj -c Release -v minimal
```

构建成功后自动部署到游戏 `Mods` 目录（游戏运行时文件被锁会部署失败，先关游戏）。
