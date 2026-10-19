# 战利品品质标记 / LootRarityMarker

给**周围容器的交互点**按「里面最好那件东西」的品质染色：
白 / 绿 / 蓝 / 紫 / 橙 / 红。不用开箱子，扫一眼地上的点就知道值不值得跑过去。

颜色**直接取自游戏自己的品质颜色表**（`GameplayDataSettings.UIStyle.displayQualityLooks`），
所以和背包、掉落物、物品图标上的稀有度颜色完全一致，不会出现「mod 的绿和游戏的绿不是一个绿」。

---

## 1. 安装

把 `dist/` 里的东西整个放进：

```
<游戏目录>\Duckov_Data\Mods\LootRarityMarker\
    LootRarityMarker.dll
    info.ini
    config.json      （可选，见 §3）
    preview.png
```

或者直接用工具（会自动编译 → 验证加载契约 → 安装 → 复核文件）：

```powershell
# 先编译
dotnet build -c Release .\src\LootRarityMarker.csproj

# 再安装（--SkipVerify 可跳过契约校验）
powershell -ExecutionPolicy Bypass -File .\tools\install.ps1
```

游戏里第一次启动会在 `%USERPROFILE%\AppData\LocalLow\TeamSoda\Duckov\Saves\Global.json`
写入 `ModActive_LootRarityMarker`，默认是开的。

---

## 2. 它到底做了什么

```
每 0.5 秒（且只在你移动时）扫一次玩家周围 25 米内、位于 "Interactable" 层上的碰撞体
        ↓
从命中的碰撞体向上找 InteractableLootbox（箱子 / 尸体的容器）
        ↓
读它的 Inventory.Content，挑出「最好」的那件（档位 = 复刻「物品价值稀有度」mod 的判定，见 3.3）
        ↓
取出那件的品质 → 查颜色 → 写进交互点渲染器的 MaterialPropertyBlock 的 _Tint / _Tint2
```

**只染交互点，不碰容器本体的材质。** 用的是 `MaterialPropertyBlock`（每渲染器独立覆盖），
不 new `Material`、不改共享材质资源，所以不会影响别的容器、别的 mod、也不会打断 SRP Batcher 的材质分组。

### 两条路线

| | 路线 B（默认开） | 路线 A（默认关，`outline=true` 打开） |
|---|---|---|
| 做法 | 染已有的交互点（那个小圆点 + 光环） | 给容器**本体**加一圈描边 |
| 实现 | `MaterialPropertyBlock` 覆盖 `_Tint` / `_Tint2` | 反射调用游戏自带的 `EPOOutline.Outlinable` |
| 风险 | 极低 | 有固定上限和单帧预算，默认关闭 |
| 开关 | 始终染交互点 | `outline` |

两条可以同时开。路线 A 全走反射、编译期不引用 `EPO.dll`，
所以**游戏哪天把 EPO 删了，也只是描边不生效，染色功能完全不受影响**。

---

## 3. 配置

### 3.1 游戏内设置菜单（推荐）

装了社区框架 **ModConfig**（创意工坊 3590674339）时，本 mod 会在它的
`Mod Settings` 标签页里注册以下条目，改完立即生效、并且会被 ModConfig 存档：

| 键 | 默认 | 说明 |
|---|---|---|
| `Enabled` | 开 | 总开关。关掉会**立刻把已经染上的颜色还原成游戏原样** |
| `ScanRadius` | 25 | 扫描半径（米） |
| `ScanIntervalMs` | 500 | 扫描间隔（毫秒） |
| `ScanOnlyWhenMoving` | 开 | 站着不动时不扫描（本 mod 最大的一条性能优化） |
| `RankByValue` | 关 | 按价值（而非品质）挑「最好」的那件 |
| `GlowByValue` | 关 | 容器里最贵的那件越值钱，交互点越亮（见 3.4） |
| `Outline` | 关 | 容器本体描边（见第 6 节） |
| `Diagnostics` | 关 | 把材质属性清单和每次判定打到 `Player.log` |
| `MaxWorkPerFrame` | 8 | 每帧最多重算几个容器（任务队列上限） |

面板里的条目说明**一律只用中文**：ModConfig 是中文界面，中英混排的说明在窄面板里会被截断成半截英文，反而更难读。

没装 ModConfig 也完全没问题：`Update()` 里每 2 秒重试一次、最多 15 次，
放弃后打一行日志说明「改用 config.json」，其余功能一切照常。

### 3.2 `config.json`（与 dll 同目录，可选）

```json
{
  "enabled": true,
  "scanRadius": 25,
  "scanIntervalMs": 500,
  "scanOnlyWhenMoving": true,
  "scanMoveDistance": 1.0,
  "rankByValue": false,
  "glowByValue": false,
  "outline": false,
  "diagnostics": false,
  "maxWorkPerFrame": 8,
  "colorProperties": ""
}
```

取值优先级：**代码默认值 → config.json → ModConfig 存档值**（后者最高）。

`scanMoveDistance` 和 `colorProperties` 没上 ModConfig 面板
（框架的滑条只支持 int；颜色属性名是字符串），只能在这里改。

### 3.3 颜色：直接采用「物品价值稀有度与搜索音效」的判定标准

玩家眼里的「这件东西是什么级别」，其实是另一个创意工坊 mod
**「物品价值稀有度与搜索音效」**（`ItemLevelAndSearchSoundMod`，workshop 3588386576）
给物品名上色决定的。所以我们**没有另起一套**，而是用 `ilspycmd`
把那个 mod 反编译出来，**逐字复刻**了它的 `Util.GetItemValueLevel(Item)`。

这样交互点的颜色和物品名的颜色就是**同一个函数**算出来的，不可能再对不上。

#### 七个档位

那个 mod 用的是 7 档（比游戏的 6 档多一个「浅红」）：

| 档 | 枚举 | 颜色 | HEX |
|---|---|---|---|
| 0 | `White` | 白 | `#FFFFFF` |
| 1 | `Green` | 绿 | `#7CFF7C` |
| 2 | `Blue` | 蓝 | `#7CD5FF` |
| 3 | `Purple` | 紫 | `#D0ACFF` |
| 4 | `Orange` | 橙 | `#FFDC24` |
| 5 | `LightRed` | 浅红 | `#FF5858` |
| 6 | `Red` | 深红 | `#BB0000` |

颜色值直接取自反编译源码里那几行 `ColorUtility.TryParseHtmlString`。
它原本还带 alpha（`0x40` / `0x96`，给 UI 文字底纹用的），这里只取 RGB，
透明度沿用交互点材质自己的 alpha。

#### 判定规则（复刻要点）

**最容易搞错的一点：它先把 `Value` 除以 2 再查阈值表。**

```csharp
float num = item.Value / 2f;
```

所以阈值 `200 / 600 / 1200 / 2500 / 5000 / 10000` 对应的**真实 `Value`** 是
`400 / 1200 / 2400 / 5000 / 10000 / 20000`。

完整分支（顺序不能换）：

| 情况 | 判定 |
|---|---|
| 物品在 `ItemAssetsCollection.dynamicDic` 里（动态物品） | `DisplayQuality ≠ None` 用它，否则 `clamp(Quality - 1, 0, 6)` |
| Tag 含 `Bullet`（子弹） | 有品质用品质（橙→浅红）；否则 `Quality=1→白`、`Quality=2→绿`，再否则按 `num × 30` 查表，**封顶在橙** |
| Tag 含 `Equipment` + `Special` | 名字含 `StormProtection` → `Quality - 1`；否则 `Quality - 2`（>6 记深红，<0 记白） |
| Tag 含 `Equipment` | `Quality ≤ 7` → `Quality - 1`；否则按 `num` 查表 |
| Tag 含 `Accessory` | `Quality ≤ 7` → `Quality - 1`；否则看 `DisplayQuality` |
| `TypeID == 862` 或 `1238` | 固定橙 |
| 其它（绝大多数普通物品） | `max(按 num 查表, 按 DisplayQuality)` |

`ParseDisplayQuality` 里红品质还要再分一次：`Quality == 6` 算**浅红**，更高才算**深红**。

容器取**所有物品里的最高档**。

> **性能说明**：`TagCollection.Contains(string)` 内部是 `list.Any(t => t.name == tagName)`，
> 每次调用都要分配闭包和委托，而且每个元素都要取一次 `UnityEngine.Object.name`（原生调用 + 字符串分配）。
> 所以这里**按 `TypeID` 把 Tag 位标志缓存起来**（Tag 属于物品资产，同 TypeID 必然相同），
> 每个 TypeID 一辈子只查一次。758 个容器的首次扫描因此从「上万次分配」变成「几百次」。

#### 那为什么以前会「箱子里看着是绿的，交互点是白的」？

因为以前颜色只看 `Item.DisplayQuality`。而游戏在发行资源里一共定义了 1581 个物品，
其中 **1310 个（83%）的 `DisplayQuality` 就是 `None`** —— 游戏自己没给它们标品质。
于是「那个 mod 按价格说它是绿的」和「交互点按品质说它是白的」就打架了。

现在不会了：两边的判定函数完全一致。

顺带一个反直觉的实测结论：**关卡文件（`level0`…`level62`）里一个 `Item` 实例都没有**
（六个代表性关卡全部为 0）。箱子内容完全是运行时由掉落表生成的 —— 和源码里
`LootBoxLoader.Setup()` 的行为一致（见 5.4）。

### 3.4 发光强度（`glowByValue`）

**能调，而且是乘在真正的 HDR 亮度上。** 关着的时候就是游戏原样，一点都不改。

| 开关项 | 默认 | 效果 |
|---|---|---|
| `glowByValue` | 关 | 按容器里**最贵的那一件**发光：越贵越亮，白/绿/蓝完全不发光，深红最亮 |

#### 为什么是「单件最高价值」而不是「总价值」

这是被实测数据否掉的：**阈值是按单件标定的**（`Value / 2` 对 `200…10000`），
把 4 件物品的价值加起来喂进去是量纲错配。

从发行资源里把 1562 个物品的 `Value` 全读出来做蒙特卡洛抽样，4 件容器的总价值分布是：

| 件数 | p50 | p75 | p90 | **> 5000 的占比** |
|---|---|---|---|---|
| 3 件 | 4112 | 8291 | 16406 | **43.9%** |
| 4 件 | 6094 | 11183 | 22658 | **57.4%** |
| 5 件 | 7925 | 14014 | 29260 | **69.4%** |

也就是说，旧的「200 → 5000 线性」公式下，**一大半箱子一出生就顶格 3.0 倍**。
全都亮 = 都不亮。改成单件最高价值后，一个装满垃圾只塞了一件好东西的箱子会亮，
一个装满中等货的箱子不会 —— 这才是玩家真正想知道的信息。

#### 曲线

锚点就是上面那套阈值的「除以 2 之后」的刻度，正好落在档位边界上：

| 最贵单件的 `Value ÷ 2` | 对应档位 | 倍率 |
|---|---|---|
| ≤ 600 | 白 / 绿 / 蓝 | **1.00×（完全不发光）** |
| 1200 | 紫 | 1.15× |
| 2500 | 橙 | 1.50× |
| 5000 | 浅红 | 2.00× |
| ≥ 10000 | 深红 | **2.60×（封顶）** |

中间线性插值，所以「越贵越亮」仍然成立，但**低档位是完全无感的**，
不会出现一屏箱子都在发光。

#### 为什么能调到亮度

交互点材质的 `_Tint` / `_Tint2` 是 **HDR 颜色**：原版的值本来就超过 1
（`InteractMarkerDot` 最高 3.0，`InteractMarkerRing` 3.44），那不是笔误，是发光强度。
本 mod 换色相时会**保留这个亮度**（`MarkerTint.Rescale`：按原色 RGB 最大分量等比缩放），
发光开关就是在这个亮度上再乘一个系数。

亮度确实能到屏幕上，因为游戏自己的 URP Asset 开了 HDR：

| 事实 | 值 | 怎么确认的 |
|---|---|---|
| `m_SupportsHDR` | `1`（开） | `globalgamemanagers.assets` pathID 5150 的类型树 |
| 交互点 pass 混合 | `SrcAlpha` / `OneMinusSrcAlpha`，`ZWrite 0`，`ZTest Always` | `BillBoardOverlay_Markers` 的 `m_State` |
| 交互点材质类型 | `UniversalMaterialType = Unlit`，队列 `Transparent` | 同一个 SubShader 的标签 |

所以这是一条「Unlit + 半透明混合 + 写进 HDR 缓冲」的通道，**乘 RGB 亮度 = 屏幕上真实的发光强度**，
不需要动材质、不需要换 shader、也不需要发光的后处理配合。

调高的代价：亮度顶到显示上限之后颜色会往白里飘（红色变粉白）。所以上限压在 **2.6 倍**。

> 一个刻意的取舍：倍率**只乘 RGB，不乘 alpha**。通道混合是 `SrcAlpha`，
> 乘 alpha 也能变暗，但那是「更透」而不是「更暗」—— 圆环会透出后面的景物，
> 看起来像掉帧而不是像调暗。乘 RGB 才是干净的亮度调节。

容器本体描边（`Outline`）的颜色用同一个倍率提亮 RGB（EPO 的 `OutlineParameters.Color`），保持两边观感一致。

---

## 4. 性能

这是本 mod 的**第一设计原则**（原始需求里写明了「着重考虑 mod 性能问题」）。

### 4.1 稳态：站着不动时每帧一次 float 比较

```csharp
float now = Time.unscaledTime;
if (!_scanRequested && now < _nextScanTime) return;   // ← 常态下每帧就这一行
```

`scanOnlyWhenMoving`（默认开）让「玩家站着不动」时**完全不做任何扫描**。
不会漏掉变化，因为下面这些事件都会强制补扫一次：

* 进入 / 加载关卡（`LevelManager.OnLevelBeginInitializing` 等三个事件）
* 开关战利品界面（`InteractableLootbox.OnStartLoot` / `OnStopLoot`）
* 玩家移动超过 `scanMoveDistance`（默认 1 米）
* 在设置里改了任何一项
* mod 被重新启用

### 4.2 扫描本身也比游戏自己便宜

游戏每帧都在做 `Physics.OverlapSphereNonAlloc(playerPos, 0.3f, colliders, interactLayers)`
找可交互物（`CA_Interact.SearchInteractableAround`，由 `CharacterMainControl.Update` 每帧调用）。

本 mod 用的是**同一个 API、同一层、同样的 `QueryTriggerInteraction.Collide`**，
但：半径 25 米、每 500 毫秒最多一次、站着不动时一次都不做。
稳态下不到游戏自身开销的百分之一。

### 4.3 零 GC

* `Collider[256]` 缓冲区复用，用 `OverlapSphereNonAlloc`
* `MaterialPropertyBlock` 每个交互点只有一个，反复 `Clear()` + 复用
* 遍历物品用**索引 for**而不是 `foreach`
  （`Inventory` 实现了 `IEnumerable<Item>`，`foreach` 会 new 一个迭代器）
* 字典 / 列表全部预分配复用
* 不 new `Material`、不 new `Texture`、不 `Shader.Find`
* 只有当**判定结果发生变化**时才写 `SetPropertyBlock`；绝大多数扫描一个字节都不写

### 4.4 容器很多时会怎样（实测数据，不是估算）

「范围内有 20 个容器会不会卡」这种问题不该靠猜，所以直接数了发行版里每个关卡的容器数量
（`tools/probe/probe_scale2.py`，只读资源文件，不需要开游戏）。

先确定两件事：

| 事实 | 值 | 出处 |
|---|---|---|
| `Interactable` 是第几层 | **layer 8** | `globalgamemanagers` 的 `TagManager.layers` |
| 交互碰撞体一定会在这个层上 | 是 | `InteractableBase.cs:216` `interactCollider.gameObject.layer = LayerMask.NameToLayer("Interactable")` |

后者很关键：因为游戏**强制**把交互碰撞体设到这一层，所以序列化数据里的层名不算数，
真正的上限是「InteractableBase 子类实例总数」。实测（全部 129 个资源容器，含 63 个场景）：

| 场景 | 可交互对象总数 | 其中战利品容器 | 场景内 GameObject | 场景内碰撞体 |
|---|---|---|---|---|
| **level10** | **1051** | **758** | 84,471 | 43,696 |
| level48 | 779 | 513 | 58,840 | 33,969 |
| level12 | 577 | 548 | 5,457 | 2,831 |
| level34 | 336 | 78 | 44,378 | 22,835 |
| level60 | 313 | 270 | — | — |
| level14 | 307 | 278 | 13,141 | 6,362 |
| level31 | 305 | 242 | 25,611 | 12,806 |
| level36 | 251 | 250 | 2,020 | 1,011 |
| level15 | 231 | **231** | 1,344 | 726 |
| 全 63 个场景合计 | 7,124 | 5,672 | — | — |

**所以一个关卡最多可能有 758 个战利品容器**（而且 level15 / level36 / level57 这种
是「可交互物几乎全是箱子」）。这正是脏标记要解决的问题。

#### 脏标记：扫过一遍就不再重算

```csharp
if (!entry.Dirty && !force) return;   // ← 挡在遍历物品之前
```

一个容器被算过之后，**之后每次被球查询命中，代价只有一次字典查找 + 一次布尔判断**，
不会再去遍历它的 `Inventory.Content`。脏标记由这些时刻置位：

* `Inventory.onContentChanged`（游戏自己每次增删物品都发，见 `Inventory.cs:130`）——
  回调只置一个 bool，真正的重算留到下一次扫描，
  所以连续搬十几件物品也只会重算一次，不会每件都刷一遍材质
* 容器第一次被发现、换关卡
* `Inventory.Loading` 还是 true（内容异步装填中）
* `InteractableLootbox.OnStartLoot` / `OnStopLoot`
* 改设置

**兜底**：每 10 秒无视脏标记全量重算一次。万一哪个事件漏了，颜色最多旧 10 秒，
不会永远错下去。（`scanOnlyWhenMoving` 打开时这条兜底也被压住 —— 站着不动是真的什么都不做。
这是刻意的：站着不动时容器内容只可能被你自己的开箱动作改变，而那是会强制补扫的。）

#### 各处的上下限

| 量 | 上限 | 说明 |
|---|---|---|
| 一次扫描处理的容器数 | **256** | `HitBufferSize`，满了会打一次警告日志 |
| 一次扫描的物理查询次数 | **1** | `OverlapSphereNonAlloc`，每 500 ms 最多一次 |
| 稳态每帧开销（站着不动） | **1 次 float 比较** | 上面的快速返回 |
| 稳态每帧开销（走路） | 每 500 ms 一次球查询 + 每次命中一次字典查找 | 命中容器已是干净的 |
| 每个容器的委托分配 | **1 次，一辈子一次** | 挂 `onContentChanged` 用；不是每帧 |
| 每帧 GC | **0 B** | 缓冲区、MPB、字典全部复用 |

### 4.5 任务队列：把「一次几百个」摊成「每帧几个」

有一条优化会让重活在同一帧里集中爆发，队列就是为它准备的：

* **改设置**：`RebuildTints()` 作废所有染色配方，下一次扫描时**所有**容器一起变脏；
* **换关卡**：账本清空，头几次扫描要把新关卡的容器**全部**从头算一遍；
* **一帧内命中很多容器**：实测 `level10` 里有 **758 个**容器。

所以扫描和重算被拆成了两件事：

```
Scan()   发现容器 → 只做「字典查找 + 置脏 + 入队」，然后立刻返回
Update() 每帧调用 DrainWorkQueue()，按预算消化队列
```

每帧最多放行固定个数，并受约 2 毫秒的内部时间预算限制；剩下的留到下一帧：

| 键 | 默认 | 作用 |
|---|---|---|
| `maxWorkPerFrame` | 8 | 一帧最多重算 8 个容器；同时受约 2 毫秒的内部工作预算限制 |

因此多个容器重建会被分散到多帧；实际每帧还会受约 2 毫秒的内部工作预算限制，
避免多个大型背包或描边初始化集中在同一帧。
视觉上的副作用是换关卡后远处容器的颜色会**逐帧亮起来**（像波纹扩散），这是刻意的取舍。

三个细节值得说明，因为它们决定了队列不会成为新的开销来源：

* **队列为空时只有一次 `int` 比较。** `DrainWorkQueue()` 第一件事就是
  `if (_queueHead >= _workQueue.Count)`，空队列直接返回，所以它可以无条件每帧调用。
* **入队本身会去重。** 每个账本有 `Queued` 标记，同一个容器在一轮里只会进队一次；
  `onContentChanged` 连续触发十几次也只是把同一个容器标脏一次（已经在队列里就什么都不做）。
* **死掉的容器不占预算。** 容器被销毁（换关卡、尸体消失）时 `PruneDeadEntries()`
  给它打上 `Dead` 标记，队列扫到它直接跳过、不计入 8 个的额度。

`DrainWorkQueue()` 被刻意放在**稳态快路径之前**——否则「站着不动」时队列永远排不空。
它放在 `LevelManager.LevelInited` 判断之后，因为队列里的容器全都来自已初始化的关卡。

#### 第四个已修的 bug：队列自己喂自己（无界增长）

第一版队列有个致命缺陷：`UpdateEntry()` 里「容器还在异步装填」那条分支写的是

```csharp
if (inventory.Loading) { entry.Dirty = true; Enqueue(entry); return; }
```

而 `UpdateEntry()` 本身**就是被 `DrainWorkQueue()` 调用的**。于是形成闭环：

```
出队 → 发现 Loading → 又入队 → 出队 → 又入队 → ……
```

队列头部永远追不上队尾，`_workQueue.Clear()` 永远不会执行，
每帧最多按数量和时间预算消化队列。死亡时一次性刷出一堆尸体容器、
它们又全是 `Loading` 状态，几秒内就能把这个循环点着火。

现在的写法是 Loading 时**只留脏标记、不入队**：

```csharp
if (inventory.Loading) { entry.Dirty = true; return; }
```

「装完的瞬间就上色」依然成立，因为装填过程中每 `AddItem` 一次游戏都会发
`onContentChanged`，而 `ContentHandler` 会把它重新入队。不需要靠自旋轮询。

另外补了三道保险：

| 保险 | 作用 |
|---|---|
| `MaxQueueLength = 4096` | 未处理条目上限，到顶就跳过本轮入队并警告一次（功能无损，下一轮扫描补上） |
| 入队时整理头部 | 已消化的前缀及时 `RemoveRange` 丢掉，`List` 不会因为不清空而无界增长 |
| 每帧最多检查 512 个条目 | 防止队列里堆一大片已销毁容器时同一帧把它们全扫一遍 |

同时给所有会碰游戏对象的路径加了 `try/catch` 兜底：`UpdateEntry` / `Scan` /
`DrainWorkQueue` / `RebuildTints` 任何一处抛异常，都只损失这一帧的这一次操作，
不会每帧抛一次把控制台刷爆。

### 4.6 和游戏的交互点渲染是解耦的

不碰 `InteractHUD`、不碰 `InteractSelectionHUD`、
**不碰那个每帧被写的全局 shader float `Interactable`**（那是全屏 HUD 共用的，改它会波及整个游戏）。

---

## 5. 为什么敢这么做（关键事实与出处）

这些不是猜的，都是从**反编译源码**和**发行版资源**里挖出来验证过的。

### 5.1 交互点是什么

* `InteractableBase.ActiveMarker()`（`InteractableBase.cs:244-255`）：
  `markerObject = Instantiate(GameplayDataSettings.Prefabs.InteractMarker, base.transform);`
  —— 交互点被实例化成**可交互物的直接子物体**，所以
  `box.GetComponentInChildren<InteractMarker>(true)` 就能拿到，不需要反射私有字段。
* `InteractMarker.cs` 整个文件只有 26 行，**没有任何渲染代码**，
  只负责在 `MarkAsUsed()` 时切换 `showIfUsedObject` / `hideIfUsedObject` 两个 GameObject。
* `InteractableBase.cs:154` `public bool MarkerActive { get; set; }`，setter 只在 `value == true` 时建交互点；
  `:239` `MarkerActive = interactMarkerVisible;`（默认 `true`）。

### 5.2 颜色到底写在哪两个属性上（本 mod 最关键的结论）

交互点预制体（`resources.assets` pathID 6907）的层级：

```
InteractMarker (6907)                       ← GameplayDataSettings.Prefabs.InteractMarker 指向的组件在这上面
    .MeshRenderer  enabled=False   mats=[InteractMarkerRing]   ← 被禁用，从不绘制
    .MonoBehaviour showIfUsedObject=17575  hideIfUsedObject=17349
  - Shadow (26596)      .MeshRenderer  mats=[InteractMarkerShadow]
  - HideIfUsed (17349)  ← 未开箱时的样子
        Quad_1 (28283)  .MeshRenderer  mats=[InteractMarkerDot]
        Ring   (26431)  .MeshRenderer  mats=[InteractMarkerRing]
  - ShowIfUsed (17575)  ← MarkAsUsed() 之后的样子
        Quad_1 (21584)  .MeshRenderer  mats=[InteractMarkerUsed]
        Ring_1 (20591)  .MeshRenderer  mats=[InteractMarkerStatic]
```

五个材质都用 shader **`BillBoardOverlay_Markers`**（`resources.assets` pathID 4762）。
把这个 shader 的属性表读出来，正好 18 个属性：

```
_Tint        type=Color  flags=HDR     ← 有
_Tint2       type=Color  flags=HDR     ← 有
_EmissionColor type=Color
_AlphaCutoff / _Scale / _Scale2 / _ScaleChangeSpeed / _IsInteractMarker
_Near / _Far / _QueueOffset / _QueueControl / _MainTex / _texcoord + 4 个 unity_*
_Color       ← 没有这个属性
_BaseColor   ← 没有这个属性
_EdgeColor   ← 没有这个属性
```

材质里序列化着 `_Color` / `_BaseColor` / `_EdgeColor`，但 **shader 根本没声明它们**
——那是 ShaderGraph 模板留下的死数据，写它们不会有任何效果。
真正承载颜色的只有 `_Tint` / `_Tint2`：它们**是唯一一组在五个材质之间按外观规律变化的颜色属性**
（点偏蓝、光环亮、Used 近白、Shadow 纯黑），而且都标了 **HDR**。

材质里的真实值：

| 材质 | `_Tint` | `_Tint2` |
|---|---|---|
| InteractMarkerDot | `1.048, 1.519, 2.001, 1.0` | `1.880, 2.438, 2.996, 1.0` |
| InteractMarkerRing | `3.442, 3.442, 3.442, 0.502` | `3.732, 3.732, 3.732, 1.0` |
| InteractMarkerStatic | `3.442, 3.442, 3.442, 0.502` | 同左 |
| InteractMarkerUsed | `0.986, 0.993, 1.000, 1.0` | `0.984, 0.992, 1.000, 1.0` |
| InteractMarkerShadow | `0, 0, 0, 0.588` | 同左 |

**值全都大于 1.0 —— 它们是 HDR 颜色，编码的是发光强度。**
所以本 mod 染色时**保留原始 RGB 的模长、只换色相**：

```csharp
k = 模长 / max(r, g, b);      // 把品质颜色放大到和原值同一个亮度量级
alpha = 原值的 alpha;         // 透明度不动
```

直接把一个 LDR 的品质颜色写进去会把发光压平，交互点会变暗变脏。
这一条是整个路线 B 里最容易踩的坑。

另外，因为 `InteractMarkerShadow` 的 `_Tint` 是纯黑，本 mod 有一条自动规则：
**源颜色 RGB 最大值 < 0.02 的渲染器自动跳过**，所以阴影永远不会被染色。

### 5.3 箱子身上真的有交互点吗

有。用 TypeTree 把 `resources.assets` 里 `InteractableLootbox`（脚本 pathID 268）的
**全部 36 个实例**的序列化值读出来：

```
interactMarkerVisible 分布: {'1': 35, '0': 1}
interactMarkerOffset 分布: {'y=0.43': 18, 'y=0.0': 12, 'y=0.61': 4, 'y=0.5': 1, 'y=0.34': 1}
```

**35 / 36 的箱子预制体都是 `interactMarkerVisible = true`**，也就是说它们 `Start()` 时都会实例化交互点。
（对剩下那 1 个，本 mod 会在 `diagnostics=true` 时打一行日志说明它没有交互点然后跳过。）

### 5.4 容器内容什么时候才有

* 内容**不是**开箱时才生成的，是关卡加载时滚好的：
  `InteractableLootbox.GetOrCreateInventory` → `LootBoxLoader.Setup().Forget()`（一个 `async UniTask`）。
* 所以 `Start()` 时枚举可能只拿到一部分。本 mod 的处理是：
  **`Inventory.Loading` 为真就跳过这个容器，等下一轮扫描**（不阻塞、不等待、不注册回调），
  并且**保持它的脏标记**，这样装填完的那一刻它一定还会被重算。
* 也就是说这里不需要任何异步编排：装填中的容器一直是脏的，装完自然就被算了。

### 5.5 关于那个「有副作用的」`box.Inventory`

`InteractableLootbox.Inventory`（`InteractableLootbox.cs:59-95`）的 getter 不是纯读的。
本 mod **只在缓存的引用失效时调一次**，之后每轮扫描只读 `Inventory.Content`（纯字段）。
理由：

* `inventoryReference` 非空时直接返回，不会新建任何东西；
* `hasBeenInspectedInLootBox` 为真时调一次 `SetMarkerUsed()`，而 `MarkAsUsed()` 内部有
  `markedAsUsed` 守卫，重复调用无副作用；
* 写一次 `DisplayNameKey` —— **游戏自己也在写**（`InteractableLootbox.Start()` 里就是这么干的）。

也就是说，游戏自己在 `Start()` 里已经把这个 getter 走过一遍了，本 mod 的调用不引入任何新行为。

### 5.6 关于排序依据

默认按「稀有度」挑「最好」的那件，颜色就是那件的品质。稀有度怎么算见 **3.3**
（默认 = 游戏标的 `DisplayQuality` 与 `Item.Value` 换算的品级取高）。

* 故意**读未鉴定物品的品质**：本 mod 的目的就是让玩家在开箱之前知道里面有没有好东西。
  `DisplayQuality` 是普通字段，不随 `Inspected` 变化，雾里的物品照样能算出颜色。
* `RankByValue=true` 时按 `Item.Value` 挑。**刻意不用 `GetTotalRawValue()`**：
  它会按耐久折算、乘堆叠数、还会递归进子容器，每个物品都调一次太贵，
  而且结果不单调（一堆废纸的总价值可能超过一件红装），拿来做颜色依据会很怪。

---

## 6. 路线 A（容器本体描边）

游戏**自带** EPOOutline 运行时并且真的在用：
`Duckov_Data\Managed\` 下有 `EPO.dll` / `EPOURP.dll` / `EPODemo.dll` / `EPOHDRP.dll` / `EPOUtilities.dll`，
`globalgamemanagers.assets` 里 `EPOOutline` 出现 23 次、`OutlineEffect`（URP 的 RendererFeature）也有，
游戏自己用它做半掩体的轮廓（`HalfObsticle.cs`）。

本 mod 通过反射调用 `EPOOutline.Outlinable`：

```csharp
AddComponent<EPOOutline.Outlinable>()          // 反射
AddAllChildRenderersToRenderingList(MeshRenderer)
// ↓ 关键的一步：收完之后立刻筛选，见下
RemoveTarget(<不该描的目标>)
OutlineParameters.Enabled = true
OutlineParameters.Color = <品质色>
```

#### 为什么必须筛选渲染器（这是一个已修的实测 bug）

`AddAllChildRenderersToRenderingList` 收的是容器**整个子树**的 `MeshRenderer`。
问题在于关卡里的「一个容器」往往不是一只箱子，而是**一整组场景道具**——
衣柜、货架、带圆形贴花的机器……这些巨物的网格也在子树里，于是：

> 实测症状：开箱式容器被描出了**横跨半个屏幕的大红圈**（用户截图复现）。

第一次交付时这一步没做，因为当时的假设是「容器 = 一只箱子」。真实数据推翻了它。
所以现在 `Attach()` 收完渲染器之后立刻按三条规则剔除：

| 规则 | 剔除什么 | 为什么 |
|---|---|---|
| 1 | 属于 `InteractMarker` 的渲染器 | 交互点不是容器本体，描它只是把标记再圈一圈 |
| 2 | 包围盒最长边 > **4 米**的渲染器 | 巨型贴花/圆环/整组道具，就是它们画出大圈 |
| 3 | 第 **6** 个之后的渲染器 | 防止一个容器挂几十个描边目标 |

剔除用的是 EPO 自己的公开方法 `Outlinable.RemoveTarget`（倒序删，索引不会错位），
不需要构造 `OutlineTargetProvider` 委托或表达式树。

这几个 API 是**必需**的：`TryInit()` 拿不到 `OutlineTargets` / `RemoveTarget` / `OutlineTarget.Renderer`
就**整条路线 A 停用**，而不是退化成「不筛选」——因为「不筛选」正是上面那个坏行为。

#### 第二个已修的 bug：`IReadOnlyList` 不是 `IList`

第一版的「必需 API」检查里写的是
`typeof(IList).IsAssignableFrom(_targetsProperty.PropertyType)`，结果是**永远为 false**：
`Outlinable.OutlineTargets` 的声明类型是 `IReadOnlyList<OutlineTarget>`，
而 `IReadOnlyList<T>` **不实现**非泛型的 `System.Collections.IList`
（只有 `List<T>` 才两个都实现）。于是路线 A 被自己静默停用，症状就是
「明明开了描边却一个描边都看不到」。

#### 第三个已修的 bug（同一个坑的第二层）：`Count` 不在 `IReadOnlyList` 上

第二版把检查改成「反射 `Count` 属性 + `Item` 索引器」，**还是永远失败**。
原因：`Type.GetProperty` **不会返回从其它接口继承来的成员**，
而 `IReadOnlyList<T>` 自己只声明了 `T this[int]`，
`Count` 是声明在父接口 `IReadOnlyCollection<T>` 上的 —— 所以 `GetProperty("Count")` 返回 `null`。
游戏日志里那行就是这个 bug 的现场：

```
[LootRarityMarker] 路线 A 不可用，描边已停用（只影响描边，染色照常）：
OutlineTargets（IReadOnlyList`1）没有可用的 Count / 索引器（无法筛选渲染器，停用路线 A）
```

现在的做法是**完全不要 `Count`**：`IReadOnlyList<T>` 一定实现 `IEnumerable<T>`，
因此也一定实现非泛型的 `System.Collections.IEnumerable`。所以直接枚举一遍，
把要剔除的目标对象收集到暂存列表，枚举结束后逐个调 `RemoveTarget(victim)`。
**按对象引用删**天然不受下标位移影响，也就不需要 `Count`。

另外 `TryInit()` 失败时**一定会打一行日志**说明原因 —— 之前这条路径是静默返回的，
所以这两个 bug 在日志里都完全查不到。

#### 上限：同时最多 16 个描边容器

`Outlinable` 是往容器本体上 `AddComponent`，EPO 每帧要为每个 `Outlinable`
生成一遍描边（额外 draw call + 模板缓冲读写）。一个关卡里有 758 个容器，
如果范围内一次刷出几百个描边，那就是纯粹的「瞬间抢占性能」——正是要避免的。

所以路线 A 有一条硬上限 `MaxOutlinedContainers = 16`：范围内的容器按扫描顺序，
前 16 个挂描边，其余的只用交互点染色表达。到上限时日志会报一次：
`描边数量已达上限 16 个，范围内的其余容器只用交互点染色表达。`

依然不确定的地方：游戏序列化的 `OutlineEffect.OutlineLayerMask` 可能不包含容器所在的层，
那样描边根本不显示（不影响染色）。打开 `diagnostics: true` 后会为每个容器打一行
`路线A 给 <名字> 挂描边：描边目标 N 个 → 保留 M 个（剔除：标记 a、超尺寸 b）`，
用它就能确认筛选是否按预期工作。

所以默认关、单独一个开关，两种路线互不影响。**不建议同时开**。

---

## 7. 明确不做的事

* 不加任何 **Harmony 补丁**（不需要，也不需要 `0Harmony.dll` 依赖）
* 不改游戏数据、不改存档格式
* 不读不写全局 shader float `Interactable`
* 不碰 `InteractHUD` / `InteractSelectionHUD`
* 不写 `renderer.sharedMaterial` / 不 new `Material`
* 不新增任何美术资产（不需要贴图、不需要 shader 文件）

---

## 8. 故障排查

**先打开 `diagnostics`（config.json 或 ModConfig 面板），重启游戏，然后看 `Player.log`。**

游戏日志位置（Windows）：
```
%USERPROFILE%\AppData\LocalLow\TeamSoda\Duckov\Player.log
```

本 mod 会打这些前缀的日志：

| 日志 | 含义 |
|---|---|
| `[LootRarityMarker] 开始工作：…` | 第一次扫描时的配置汇总 |
| `[LootRarityMarker][诊断] 首个交互点的材质清单：…` | 每个渲染器 + 材质 + shader + 候选属性的当前值 |
| `[LootRarityMarker][诊断] <容器名>：物品数=… 最高品质=…` | 每次判定变化 |
| `[LootRarityMarker] 交互点材质上没找到可染的属性…` | **染色会是无效的**，日志里有完整排查指引 |
| `[LootRarityMarker] 找不到 "Interactable" 层…` | 层名变了（游戏大版本更新） |
| `[LootRarityMarker] 路线 A 可用：已找到游戏自带的 EPOOutline（EPO）。` | 描边可用。**没看到这行就说明描边没生效**，下一行会写原因 |
| `[LootRarityMarker] 路线 A 不可用，描边已停用（只影响描边，染色照常）：…` | 描边不可用的**确切原因**。常见：该关卡没加载 EPO 程序集 |
| `[LootRarityMarker][诊断] 路线A 给 <名字> 挂描边：描边目标 N 个 → 保留 M 个（剔除：标记 a、超尺寸 b）` | 筛选是否按预期工作 |
| `[LootRarityMarker] 一次扫描内的可交互物达到缓冲区上限 256…` | 把 `scanRadius` 调小 |
| `[LootRarityMarker] 描边数量已达上限 16 个…` | 路线 A 只给前 16 个容器挂描边，其余只染色 |
| `[LootRarityMarker] 本帧耗时 X 毫秒（待办队列剩余 N 个…）` | **看门狗**。本 mod 自己这一帧花了超过 6 毫秒。反复出现就把 `maxWorkPerFrame` 调小或暂时关闭 `outline` |
| `[LootRarityMarker] 待办队列已达上限 4096 个…` | 入队速度长期超过处理能力，把 `maxWorkPerFrame` 调大，或减小扫描范围 / 增大扫描间隔 |
| `[LootRarityMarker] 处理容器 … 时出错（已跳过）` / `扫描时出错（已跳过本次扫描）` | 单个容器/单次扫描抛异常被兜住，看不到就等于没发生 |
| `[LootRarityMarker] 未检测到可用的 ModConfig…` | 正常，用 config.json 就行 |

**怀疑卡顿/卡死是本 mod 造成的？**
先看日志里有没有 `[LootRarityMarker]` 开头的**异常堆栈**（不是上面那些提示性日志）。
本 mod 的每一条会碰游戏对象的路径都套了 `try/catch` 并且**限流**（同类每分钟最多一条），
所以「每帧刷屏的堆栈」如果存在，堆栈里一定会写明是哪个 mod 的哪个方法。
再把 `enabled` 设成 `false`（或直接删掉 `Mods\LootRarityMarker\` 整个目录）复现一次：
如果照样卡死，那就与本 mod 无关。

**如果看到两个同名 mod / 数值改了没生效**：
`install.ps1` 每次安装都会把旧目录备份下来，但**备份放在 `Duckov_Data\ModBackups\`（Mods 之外）**。
原因是 `ModManager.Rescan()` 就是 `Directory.GetDirectories(Mods)`，
而备份目录里带着一份 `name` 完全相同的 `info.ini`，而 `ScanAndActivateMods()` 是
`if (!activeMods.ContainsKey(modInfo.name))` —— **同名只激活第一个，谁先被枚举到谁生效**。
留在 Mods 里的备份有可能反过来把旧版本顶上去，所以一律挪走。

万一你还是手动留下了 `Mods\LootRarityMarker.bak-*` 目录，重跑一次 `install.ps1` 会自动把它移出去。

**游戏更新后属性名变了怎么办？**
`colorProperties` 就是为这件事准备的：填一个逗号分隔的候选列表（默认 `_Tint,_Tint2`），
配合 `diagnostics=true` 看日志里「材质清单」那段，把真正承载颜色的属性名填进去即可，不用改代码。

---

## 9. 目录结构

```
LootRarityMarker/
├── PLAN.md                      设计文档（事实基线 / 架构 / 性能预算 / 阶段划分）
├── README.md                    本文件
├── src/
│   ├── LootRarityMarker.csproj  netstandard2.1 / LangVersion 9，引用全部 Private=false
│   ├── ModBehaviour.cs          入口：生命周期、扫描、判定、应用、设置
│   ├── ModSettings.cs           设置数据结构 + config.json 读取 + Normalize
│   ├── QualityPalette.cs        物品价值稀有度 mod 同款颜色
│   ├── RarityRules.cs           物品价值稀有度判定规则
│   ├── MarkerTint.cs            交互点染色单元（HDR 保模长换色相 / MPB 复用）
│   ├── OutlineTint.cs           路线 A：EPOOutline 反射封装
│   └── ModConfigBridge.cs       ModConfig 框架反射桥（无硬依赖）
├── tools/
│   ├── install.ps1              编译产物 → dist → 游戏 Mods 目录（含备份与复核）
│   ├── verify-contract.ps1      反射复现游戏的加载契约并检查依赖闭包
│   ├── make-preview.py          生成 256x256 preview.png
│   └── probe/                   只读资源探测脚本（见下）
└── dist/                        最终交付物：dll + info.ini + config.json + preview.png
```

### `tools/probe/` 里的探测脚本（只读，不改游戏文件）

用 [UnityPy](https://github.com/K0lbond/UnityPy) 直接从发行版资产里挖事实，
这样不用为了「shader 里到底有没有 `_Tint`」这种问题先跑一遍游戏：

| 脚本 | 产出 |
|---|---|
| `probe_assets.py` | 交互点预制体层级 + 五个材质的全部序列化属性 |
| `probe_assets2.py` / `3.py` / `4.py` | typetree 版迭代（修父子和分组问题） |
| `probe_quality_colors.py` | 从 `GameplayDataSettings` 里解出六档品质色 |
| `probe_shader.py` | **`BillBoardOverlay_Markers` 的属性表**（确认 `_Tint`/`_Tint2` 存在且是 HDR Color） |
| `probe_marker_visible.py` | **各 Interactable 子类 `interactMarkerVisible` 的取值分布**（确认箱子有交互点） |
| `probe_scale.py` / `probe_scale2.py` | 每张地图的可交互物 / 箱子数量（性能论证的实测依据） |
| `probe_glow.py` / `probe_hdr_bloom.py` / `probe_volume.py` | 交互点 pass 状态、URP 的 HDR 开关、有没有 Bloom |
| `probe_item_quality.py` | **全量 `Item` 的 `displayQuality` / `quality` 分布**（证明 83% 是 None） |
| `probe_item_value.py` | **全量 `Item` 的 `Value` 分布**（验证价值分档阈值是否合理；顺带证明关卡里没有序列化的物品） |
| `dump_itemlevel_mod.ps1` / `dump_itemlevel_il.ps1` | 早期用反射 / 反汇编粗读「物品价值稀有度」mod 的阈值（已被 `ilspycmd` 完整反编译取代） |

输出都在 `tools/probe/probe_out/*.txt`。

---

## 10. 已知限制

* 只覆盖 `InteractableLootbox`（箱子、尸体容器、任务容器）。
  拾取物（`InteractablePickup`）、门、NPC 不染色 —— 它们没有「里面装了什么」这个概念。
* 白色品质看起来和原版几乎一样。这是 HDR 保亮度染色的必然结果（原版 `_Tint` 本身就是近白的
  `1.0 / 1.5 / 3.4`），也意味着「白色 = 没有值得跑一趟的东西」这个信息是**隐式**的。
  这也是为什么提供了 `glowByValue`：颜色按统一标准判定，而「值不值得跑一趟」交给亮度表达。
* 已经开过的箱子会不会染色，取决于它**还剩不剩东西**：
  `InteractMarkerUsed` / `InteractMarkerStatic` 这两个「开过」材质的 `_Tint` 也是非黑的，
  所以照样会被染色 —— 箱子被打开但没搬空时，颜色会保留；搬空后判定变空 → 自动还原成原版样式。
  一箱全是白装的箱子，开过之后看起来会和原版一样。
* 扫描是「玩家周围」的球形范围，**不是**「视野内」。隔墙的箱子也会被染上颜色。
  这是刻意的：球形范围更便宜、也不需要做遮挡测试；如果你希望只染看得见的，
  把 `scanRadius` 调小即可。
* 容器内容的判定粒度是「整箱」，不是「每个物品槽」。
* 路线 A 的描边会主动剔除超过 4 米的渲染器，所以如果某个容器本体
  真的比 4 米还大（卡车、大型机械之类），它不会被描边（染色不受影响）。
  这是为了换掉「描出横跨半屏的大圈」那个更糟的行为，见 §6。
* 任务队列让「改设置 / 换关卡」后的染色**分几帧渐进完成**。
  `maxWorkPerFrame` 只是数量上限，实际还受约 2 毫秒的内部时间预算限制。
* 颜色用的是**复刻**「物品价值稀有度与搜索音效」mod 的判定（见 3.3），不再看游戏自己的
  `DisplayQuality`。所以「箱子里看着是绿的、交互点也是绿的」现在是保证的。
  但那个 mod 的规则本身有特例（子弹封顶在橙、装备看 `Quality` 不看价格），
  本 mod 一并照搬，不另立标准 —— 想改就得两边一起改。
* 发光用的是「容器内**单件最高** `Value`」，不是求和（原因和数据见 3.4）。
  不递归子容器：背包里的背包不会再展开算一遍，单次判定是严格 O(物品数)。
* `Item.Value` 是物品的基础价值，不随耐久衰减。一件磨到 1% 的枪和全新的枪算同一个价。
