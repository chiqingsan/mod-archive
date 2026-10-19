# 高价值容器标记 (LootRarityMarker) — 修改计划（待审阅）

> **当前实现提示**：本文保留为早期设计记录，完整实现以 `README.md` 和 `src/` 为准。当前工作队列除 `maxWorkPerFrame` 数量上限外，还受约 2 毫秒的单帧时间预算限制；路线 A 描边是正式功能，默认关闭，并有 16 个容器上限。

> **目标**：玩家在关卡里靠近容器时，一眼看出"哪个容器里有好东西"——按**容器内最高品质物品的颜色**给容器做视觉标记。
>
> **两种表现方式（对应你的需求 1）**：
> - **路线 B（推荐，默认）**：把游戏默认的**白色交互点**染成对应品质色（白/绿/蓝/紫/橙/红）。
> - **路线 A（可选，默认关）**：给容器加**真·描边**——复用游戏**自己已经随包发行的 EPOOutline**，零新增着色器资产。
>
> **约束**：改动面尽量小、性能开销尽量小（目标：稳态每帧 2 次标量比较、0 字节 GC）。
>
> 目标游戏：逃离鸭科夫 / Escape from Duckov（Unity 2022.3.62f2, Mono, URP）。
> 安装目录 `d:\app\steam\steamapps\common\Escape from Duckov`。
> 全部结论来自反编译源码（`D:\code\mod\Escape from Duckov\decompiled\`）、游戏发行资产（`Duckov_Data\`）与自制 mod 模板（`HeadshotBulletRefund\`）的实测；证据一律标注 `文件:行号`。

---

## 0. 一句话方案

**不给容器加任何新物体、不碰游戏数据**：容器身上本来就有 `InteractableBase` 在 `Start()` 时实例化出来的那个交互点预制体（`InteractableBase.cs:253`）。本 mod 只在**玩家附近**的容器上，把这个交互点里几个既有 `Renderer` 的 `_Color` 用 `MaterialPropertyBlock` 改成"该容器最高品质的颜色"；容器离开范围就还原。发现容器靠一次低频 `Physics.OverlapSphereNonAlloc`（游戏自己就是这么找交互物的，`CA_Interact.cs:51`），内容变了下次扫描自然重算。

**零 Harmony 补丁、零每帧扫描、零 GC 分配、零新增美术资产。**

---

## 1. 关键事实基线（全部已亲自核对）

| # | 事实 | 证据 |
| --- | --- | --- |
| 1 | **品质枚举顺序正好是你要的六色 + 2 级预留** | `ItemStatsSystem\DisplayQuality.cs:3-14`：`enum DisplayQuality { None, White, Green, Blue, Purple, Orange, Red, Q7, Q8 }`（None=0…Red=6，无显式赋值） |
| 2 | 物品品质是**纯字段**，读取零开销零分配 | `ItemStatsSystem\Item.cs:224-234` `public int Quality`；`:236-246` `public DisplayQuality DisplayQuality` |
| 3 | 物品价值是**纯字段** | `ItemStatsSystem\Item.cs:212-222` `public int Value`；整树价值 `:1288` `public int GetTotalRawValue()` |
| 4 | 游戏自己的**品质→颜色**表（UI 图标辉光色） | `TeamSoda.Duckov.Core\Duckov.Utilities\GameplayDataSettings.cs:554-570` `class DisplayQualityLook { DisplayQuality quality; float shadowOffset; Color shadowColor; bool innerGlow; }`；列表 `:584-585`；查表 `:670` `GetDisplayQualityLook(DisplayQuality q)` |
| 5 | 从 mod 取色表的**唯一正确路径** | `GameplayDataSettings.cs:854` `public static UIStyleData UIStyle => Default.uiStyleData;` → `GameplayDataSettings.UIStyle.GetDisplayQualityLook(q).shadowColor`。**具体颜色值不在代码里**，在序列化资产 `GameplayDataSettings.asset` 里，只能运行时读 |
| 6 | 游戏自己在世界物体上用这个颜色（重要先例） | `Duckov\LotteryBox.cs:114-121` `int num = result.Quality; if (num > 6) num = 6; … GetDisplayQualityLook(q).shadowColor` → `:185-195` `glow.material.SetColor("_EmissionColor", …)` |
| 7 | **白色交互点是每个容器自己的子物体**，容器 `Start()` 时惰性创建 | `InteractableBase.cs:231-240` `protected virtual void Start()` 里 `MarkerActive = interactMarkerVisible;`（`:239`）；setter `:156-175` 为 true 时调 `ActiveMarker()` |
| 8 | 创建代码 | `InteractableBase.cs:242-257` `protected void ActiveMarker()`：`:253` `markerObject = UnityEngine.Object.Instantiate(GameplayDataSettings.Prefabs.InteractMarker, base.transform);`（**直接子物体**，所以 `GetComponentInChildren<InteractMarker>(true)` 就能拿到，不需要反射私有字段）；`:255` `markerObject.transform.localPosition = interactMarkerOffset;` |
| 9 | 预制体访问路径 | `GameplayDataSettings.cs:251` `public InteractMarker InteractMarker => interactMarker;` + `:832` `public static PrefabsData Prefabs => Default.prefabs;` |
| 10 | `InteractMarker` **本身没有任何渲染代码**，只有两个公开子物体开关 | `InteractMarker.cs`（全文 26 行）：`:7` `public GameObject showIfUsedObject;` `:9` `public GameObject hideIfUsedObject;` `:11-25` `MarkAsUsed()` 只是把前者 `SetActive(true)`、后者 `SetActive(false)` |
| 11 | 交互点预制体**实际有哪些子物体**（从发行资产里读出来的） | `Duckov_Data\resources.assets` 偏移 3 385 516 起，名字依次出现：`InteractMarker`、`InteractMarkerDot`、`InteractMarkerRing`、`InteractMarkerShadow`、`InteractMarkerStatic`、`InteractMarkerUsed`（偏移 3 387 004 / 3 388 492 / 3 391 468） |
| 12 | "已搜过"是**免费判决条件**，不用反射 | `hideIfUsedObject.activeSelf == false` ⟺ `MarkAsUsed()` 已执行（`InteractMarker.cs:11-25`）；触发点 `InteractableBase.cs:259-265` `SetMarkerUsed()` |
| 13 | **游戏自己怎么找交互物**：`Physics.OverlapSphereNonAlloc`，不是管理器单例 | `CA_Interact.cs:40` `interactLayers = 1 << LayerMask.NameToLayer("Interactable")`；`:41` `colliders = new Collider[5];`；`:51` `Physics.OverlapSphereNonAlloc(base.transform.position + Vector3.up*0.5f + characterController.CurrentAimDirection*0.2f, 0.3f, colliders, interactLayers);`（半径 **0.3**） |
| 14 | 这个探测**每帧**跑一次（本 mod 的性能对标基准） | `CharacterMainControl.cs:1697-1723` → `:1701` `interactAction.SearchInteractableAround();`，由 `if (LevelManager.LevelInited)`（`:1699`）把守 |
| 15 | 容器一定在 `"Interactable"` 层 | `InteractableBase.cs:216` `interactCollider.gameObject.layer = LayerMask.NameToLayer("Interactable");` |
| 16 | 容器内容**不是开箱时才生成**，是关卡加载时异步摇好的 | `InteractableLootbox.cs:132-137` `GetOrCreateInventory(...)` 里 `component.Setup().Forget()`；`LootBoxLoader.cs:142-223` 是 `async UniTask Setup()`，`:207` `Loading = true`、`:212-214` 逐件 `await InstantiateAsync` + `AddItem`、`:221` `Loading = false` |
| 17 | 生成中/生成完的**公开判据** | `Inventory.cs:46-56` `public bool Loading { get; set; }` |
| 18 | 容器内容用这个读（**零分配的索引循环**） | `Inventory.cs:76` `public List<Item> Content => content;`。**不要用 `foreach (Item i in inventory)`**——`Inventory.GetEnumerator()` 是 `yield return` 迭代器（`Inventory.cs` 约 `:524-533`），每次分配状态机 |
| 19 | 嵌套：`Item.Inventory` 是**纯字段读取**，没有副作用 | `Item.cs:360-370` `public Inventory Inventory { get { return inventory; } internal set {...} }` |
| 20 | ⚠️ **`InteractableLootbox.Inventory` getter 有副作用**，不能反复读 | `InteractableLootbox.cs:59-95`：`:83` `LevelManager.Instance.MainCharacter.PopText("空的Inventory")`、`:84` `Debug.LogError`、`:90` `SetMarkerUsed()`、`:92` `inventory.DisplayNameKey = displayNameKey;` |
| 21 | 关卡事件齐全（用来清理状态） | `LevelManager.cs:266` `public static event Action OnLevelBeginInitializing;`（触发点 `:351`）、`:270` `OnAfterLevelInitialized`（触发点 `:453`）、`:250` `public static bool LevelInited` |
| 22 | **游戏随包发行了完整的 EPOOutline（真描边库）** | `Duckov_Data\Managed\` 下有 `EPO.dll`(60 KB)、`EPOURP.dll`、`EPODemo.dll`、`EPOHDRP.dll`、`EPOUtilities.dll` |
| 23 | EPO 的**运行时与 URP RendererFeature 都已接好** | `Duckov_Data\globalgamemanagers.assets` 里出现字符串 `Outliner`(×2)、`OutlineTarget`、`OutlineParameters`、`OutlineEffect`、`EPOOutline`(×23)；`EPOURP.dll` 含 `AddRenderPasses`、`GetAllActiveOutlinables`、`GetOutlinersToRenderWith`、`EnqueuePass` → `OutlineEffect : ScriptableRendererFeature` 已经挂在 URP 资产上 |
| 24 | 游戏**自己就在用** EPO 描边 | `HalfObsticle.cs:2` `using EPOOutline;`；`:7` `public Outlinable outline;`；`:23` `outline.enabled = false;`（默认关，进出触发器时开关） |
| 25 | EPO 的颜色 API 确实存在（**具体成员名 Phase 0 用反射打印**） | `EPO.dll` 里含 `Outlinable`、`Outliner`、`OutlineTarget`、`ColorMask`、`SetColor`、`GetColor`、`SerializedPass`、`BoundsMode` |
| 26 | 游戏**不**用 `renderer.material`（实例材质）做逐物体变色，用的是 `MaterialPropertyBlock` | 全代码库 `.material` 仅 12 处 vs `sharedMaterial` 13 处，而 MPB 用法 17+ 处；范本 `HurtVisual.cs:123-139` `GetPropertyBlock` → `SetFloat` → `SetPropertyBlock`；悬停高亮先例 `Duckov.Crops.UI\GardenView.cs:430-432` `materialPropertyBlock.SetColor("_Tint", value)` |
| 27 | 游戏**没有**任何每帧 `FindObjectsOfType` 会与本 mod 冲突 | 全树只有 6 处，且都不在玩法循环里（`ModConfig\...\ModBehaviour.cs`、`DarkRoomFade.cs`、`MiniMapDisplay.cs`、`ShowDialogueOnQuestActivate.cs`） |
| 28 | 交付契约（照抄已跑通的模板） | `info.ini` 的 `name` = 文件夹名 = DLL 名 = 入口类型命名空间；入口类型 `<name>.ModBehaviour` 继承 `Duckov.Modding.ModBehaviour`；入口点 `protected virtual void OnAfterSetup()` / `OnBeforeDeactivate()`（`Duckov.Modding\ModBehaviour.cs:5-29`）。参考已安装成品 `HeadshotBulletRefund\dist\info.ini` |
| 29 | 当前已装 mod（共存面） | `Duckov_Data\Mods\` = `DuckovBetterRealDog`、`HeadshotBulletRefund`、`ImNotGonnaPay`、`LootNearByItem`、`ValuePerKg` |

---

## 2. 路线选型

### 2.1 路线 B：染交互点颜色（**推荐，默认启用**）

| 项 | 结论 |
| --- | --- |
| 新增依赖 | **零**。只读 3 个公开 API：`InteractableLootbox.Inventory`、`InteractMarker.hideIfUsedObject`、`Renderer.SetPropertyBlock` |
| 怎么找到目标 | `box.GetComponentInChildren<InteractMarker>(true)`（因为交互点是容器的直接子物体，`InteractableBase.cs:253`） |
| 怎么上色 | 遍历该交互点下的 `Renderer`，`MaterialPropertyBlock.SetColor(Shader.PropertyToID("_Color"), c)` + `SetPropertyBlock(mpb)`。备用属性名 `_Tint` / `_BaseColor` / `_EmissionColor`（见 §5 探针） |
| 绝对不做 | **不碰 `renderer.material`**（会实例化材质、破坏 SRP Batcher、产生 GC），只读 `renderer.sharedMaterial`；**不碰**全局 shader float `Interactable`（`InteractHUD.cs:29/75` 每帧在写它，碰了会污染全局） |
| 每次变化开销 | 1 次 O(n) 品质扫描（n = 容器物品数，通常 ≤ 20）+ ≤ 5 次 `SetPropertyBlock`，**零分配** |
| 稳态每帧开销 | **0** |
| 可逆性 | 一行还原：`renderer.SetPropertyBlock(null)` |
| 风险 | 该交互点用的是自定义 ShaderGraph 材质，**MPB 是否被采纳未实测**。降级路径见 §6.2 |

### 2.2 路线 A：真描边（**可选，默认关闭**）

| 项 | 结论 |
| --- | --- |
| 新增依赖 | **零新增美术资产**——EPOOutline 随游戏发行（事实 22-24） |
| 怎么做 | 对范围内的容器 `var o = box.gameObject.AddComponent<Outlinable>();` → 设颜色 → `o.enabled = true`；离开范围 `o.enabled = false`（或 `Destroy(o)`）。相机侧 `Outliner` + URP `OutlineEffect` 已经由游戏自己配好，不需要我们动渲染管线 |
| 开销 | EPO 每帧遍历所有 active `Outlinable`（`GetAllActiveOutlinables`），每个目标 front/back 各一次额外 draw。**成本与在场描边物体数线性相关**，所以必须严格限制在 `scanRadius` 内、并且默认关闭 |
| 风险 | ① `Outlinable` 的确切颜色成员名未知（只知道存在 `SetColor`/`GetColor`），Phase 0 反射打印；② 运行时新挂的 `Outlinable` 是否立刻被 `Outliner` 收进去，Phase 0 实测 |

### 2.3 为什么**不**选别的

| 方案 | 否决理由 |
| --- | --- |
| 自己写轮廓 shader（法线外扩 + `Cull Front`） | 要自己编译 shader、处理多子网格/蒙皮，且每个描边物体破坏一次 SRP 合批。**收益和 EPO 一样，成本高一个数量级** |
| 自定义 `ScriptableRendererFeature` 做屏幕空间描边 | 无既有 feature 可搭车、无 `CommandBuffer` 使用先例、无 `ShaderTagId`/Stencil/`RenderingLayerMask` 约定（全树 0 命中），等于从零写一条渲染通道。**最贵最险** |
| `FindObjectsOfType` 全场景找容器 | 关卡里容器数未知（可能上百），且尸体容器是运行时创建的；一次全扫虽不算贵，但**不如低频邻近扫描**——后者天然只覆盖玩家周围、天然覆盖新生成的尸体容器 |
| 每帧 `Update` 里实时算品质 | 完全没必要。容器内容变化是低频事件，0.5 秒的扫描延迟玩家感知不到 |

---

## 3. 架构与数据流

### 3.1 文件划分

```
D:\code\mod\Escape from Duckov\LootRarityMarker\
├─ PLAN.md                     本文件
├─ README.md                   安装/启用/配置/验证说明（照 HeadshotBulletRefund 体例）
├─ src\LootRarityMarker.csproj netstandard2.1；引用 Managed\ 下的 UnityEngine*.dll / ItemStatsSystem.dll / TeamSoda.Duckov.Core.dll / EPO.dll；无 NuGet 依赖
├─ src\ModBehaviour.cs         入口 <name>.ModBehaviour：生命周期、扫描泵、清理
├─ src\LootScanner.cs          OverlapSphereNonAlloc + 命中去重 + 出入范围管理
├─ src\RarityColors.cs         Color[7] 缓存（启动时从 UIStyle 取一次）+ config 覆盖
├─ src\MarkerPainter.cs        路线 B：找 InteractMarker、写 MPB、还原
├─ src\OutlinePainter.cs       路线 A：AddComponent<Outlinable> / 设色 / 禁用
├─ src\ModSettings.cs          配置模型 + config.json 读取（JsonUtility）+ 归一化
├─ src\ModConfigBridge.cs      对 ModConfig 的可选反射桥（**从 HeadshotBulletRefund 原样复用**，没装不报错）
├─ tools\install.ps1           编译产物同步 dist\ 并安装到游戏（含备份）
├─ tools\verify-contract.ps1   反射校验加载契约（类型名/继承/无缺失引用）
├─ tools\make-preview.py       生成 256×256 preview.png
└─ dist\                       info.ini + LootRarityMarker.dll + config.json + preview.png
```

新写 C# 预计 **~600 行**（其中 `ModConfigBridge.cs` ~180 行直接复用）。比 `HeadshotBulletRefund` 还小，因为核心逻辑是"扫描 → 算最大品质 → 写颜色"。

安装到：`d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data\Mods\LootRarityMarker\`

### 3.2 数据流

```
[开局/关卡初始化]
  LevelManager.OnLevelBeginInitializing (LevelManager.cs:266, 触发 :351)
        └─ 清空所有已跟踪容器 + 还原颜色

[每帧] ModBehaviour.Update()
  if (_trackedCount == 0 && !ScanDue()) return;      ← 稳态只有这 2 次标量比较
  ScanDue() = Time.time >= _nextScanTime
           && (player.position - _lastScanPos).sqrMagnitude >= _scanStepDist²
                 ↑ 站着不动就不扫（默认 1.5 m）；"_nextScanTime" 默认每 0.5 s 一次

[扫描] LootScanner.Scan()
  1. 取主角位置（复用缓存的 Transform，不调 Camera.main）
  2. Physics.OverlapSphereNonAlloc(pos + up*0.5f, scanRadius(默认 20m),
                                   _colliderBuf(64), _interactableMask,
                                   QueryTriggerInteraction.Collide)     ← 照抄 CA_Interact.cs:51 的做法，只是半径更大、频率低 30 倍
  3. 每个命中：
     - _boxByCollider 字典查缓存，命中就直接拿 InteractableLootbox
     - 未命中 → collider.GetComponentInParent<InteractableLootbox>() 再写进字典
     - 不是容器（门/ATM/车辆…）→ 记成 "null 命中"，下次不再上溯
  4. 对范围内每个容器：
     - tracker 不存在 → 创建（读一次并缓存 Inventory，见 §6.1；读出 null 或 Loading==true 就跳过本次）
     - MaxQuality(tracker.inv.Content)                 ← 零分配索引循环
     - 品质 == tracker.lastApplied → 跳过（去重，不做任何 Unity 调用）
     - 否则 写颜色（路线 B 和/或 A）
  5. 清理：本轮 `lastSeenSerial != _serial` 的 tracker → 还原颜色 + 移出字典
     （用一个 releaseRadius(默认 30m) 做滞回，避免边界反复抖动：20 m 内上色，30 m 外还原）

[内容变化] 无需事件订阅
  玩家边搜边拿 → Inventory.RemoveAt (Inventory.cs:442) 触发 onContentChanged
  但本设计**不订阅它**：下一次扫描（≤0.5 s）就会重算。少一套订阅/退订/泄漏面。
```

### 3.3 核心算法（不到 20 行）

```csharp
// 返回 0..6（None..Red）；切片 7/8 按游戏自己的规则钳到 6（对齐 LotteryBox.cs:116-121）
static int MaxQuality(Inventory inv) {
    var list = inv.Content;                       // Inventory.cs:76，List<Item>，零分配
    int best = 0;
    for (int i = 0; i < list.Count; i++) {
        var it = list[i];
        if (it == null) continue;
        int q = it.Quality;                       // Item.cs:224，纯字段读
        if (q > best) best = q;
        if (best >= 6) break;                     // 已经最高，提前退出
    }
    return best;
}
```

`rankBy = Value` 时把它换成读 `it.Value * max(1, it.StackCount)`（或 `it.GetTotalRawValue()`，`Item.cs:1288`）；`minQuality` / `minValue` 阈值在同一个循环里判掉。

---

## 4. 性能账（对应你的需求 2）

| 场景 | 每帧开销 | 托管分配 |
| --- | --- | --- |
| **稳态：玩家站着不动 / 附近没有需要更新的容器** | **2 次标量比较 + 1 次 return**，0 次 Unity API 调用 | **0 B** |
| 玩家移动中 | 稳态 + 每 0.5 s 一次 `OverlapSphereNonAlloc`（≤64 命中）→ 均摊 **≈1/30 次/帧**；其中新增容器才走 `GetComponentInParent` | 0 B |
| 某容器品质变化 | 一次 O(n) 扫描（n ≤ 20）+ ≤5 次 `SetPropertyBlock` | 0 B |
| 容器出范围 | ≤5 次 `SetPropertyBlock(null)` | 0 B |
| 关卡加载 | 1 次事件订阅（`OnLevelBeginInitializing`） | 0 B |
| 启动 | 1 次 `Resources.Load<GameplayDataSettings>`（游戏自己缓存的，`GameplayDataSettings.cs:814-824`）+ 查 7 次色表 + 读 `config.json` | 一次性，可忽略 |

**对标**：游戏为了显示交互提示，**每帧**对主角做一次 `OverlapSphereNonAlloc`（`CharacterMainControl.cs:1701` → `CA_Interact.cs:51`）。本 mod 的移动态成本是它的 **1/30**，静止态为 **0**。

**必须缓存的 5 样东西**（否则会变成每帧分配）：

| 缓存 | 理由 |
| --- | --- |
| `Color[7]` 品质色表 | `GetDisplayQualityLook` 走 `List.Find` 且 **miss 时 new 一个 `DisplayQualityLook`**（`GameplayDataSettings.cs:670-684`）——绝不能每帧调 |
| `static readonly int _colorId = Shader.PropertyToID("_Color")` | 避免每帧字符串哈希 |
| `MaterialPropertyBlock` 单例 | 复用，不每次 new |
| `List<Renderer>` 复用 | `GetComponentsInChildren(bool, List<Renderer>)` 重载，只分配 1 个 List |
| `Collider[64] _colliderBuf` | `OverlapSphereNonAlloc` 要求预分配 |

---

## 5. 表现细节

### 5.1 品质 → 颜色

默认 `useGameColors = true`：`GameplayDataSettings.UIStyle.GetDisplayQualityLook((DisplayQuality)q).shadowColor`（游戏自己给品质物品图标用的辉光色，`LotteryBox` 也用它给世界物体上光）。

`config.json` 里的 `colors` 六项（`#RRGGBB` 或 `#RRGGBBAA`）**优先级更高**；查表失败或 `useGameColors=false` 时退到内置默认：

| 品质 | 内置默认 |
| --- | --- |
| 1 White | `#FFFFFF` |
| 2 Green | `#4CD964` |
| 3 Blue | `#3B9BFF` |
| 4 Purple | `#B65CFF` |
| 5 Orange | `#FFA02E` |
| 6 Red | `#FF4D4D` |
| 0 None（空箱） | 不染色（还原原生白） |

### 5.2 染哪些子物体

默认：交互点下**除 `showIfUsedObject` 子树以外**的所有 `Renderer`（即"未搜过"的点/环/影）。这样：
- 搜过的容器保持游戏原生的 used 视觉，不会误导玩家；
- 不依赖 `InteractMarker.cs` 里的私有字段，只用两个 public GameObject。

`tintUsedMarkers = true` 时连 `showIfUsedObject` 一起染。

---

## 6. 边界与风险

### 6.1 ⚠️ `InteractableLootbox.Inventory` getter 有副作用（最高风险点）

`InteractableLootbox.cs:59-95`：读这个属性会**建 Inventory GameObject**、失败时 `PopText("空的Inventory")` + `LogError`（`:83-84`），还会在 `hasBeenInspectedInLootBox` 时 `SetMarkerUsed()`（`:90`）、写 `DisplayNameKey`（`:92`）。

**对策**：每个容器**只在首次注册时读一次**，结果连同容器的引用一起存进 `tracker.inv`；此后一律用缓存。读出 `null` → 该容器永久跳过（不再重试，避免每 0.5 秒刷一条 `LogError`）。

### 6.2 ⚠️ MPB 可能不被交互点材质采纳（路线 B 的生死线）

`InteractMarker` 的材质是自定义 ShaderGraph（子物体名 `InteractMarkerDot`/`Ring`/`Shadow`/`Static`/`Used`，见事实 11），**它认哪个属性名，只能运行时试**。

**Phase 0 用一次性探针打印**：每个子 Renderer 的 `sharedMaterial.shader.name` + `GetPropertyCount()`/`GetPropertyName(i)` 全清单。

**降级路径（保证方案不失败）**：若 MPB 无效 → 改用 `renderer.material`（实例化材质）。代价是每容器多一份材质实例、破坏这几个 renderer 的 SRP 合批；但**只在首次上色时创建一次**，稳态每帧仍是 0。而且这些交互点本来就是每个容器各实例化一个（`Instantiate`，`InteractableBase.cs:253`），不存在"共享材质被全局改坏"的问题。

### 6.3 容器内容异步生成

关卡加载时 `Setup()` 是异步的（事实 16）。对策：`inv.Loading == true` 时**本次直接跳过**，下次扫描（≤0.5 s）再算。因为内容变化是"加物品"，晚算只是颜色慢半秒出现，不会算错。

### 6.4 空箱 / 尸体容器

- 空箱：`MaxQuality` 返回 0 → 还原原生颜色（不做特殊处理）。
- 尸体容器是运行时创建的（`InteractableLootbox.CreateFromItem`），**低频邻近扫描天然覆盖**，不需要额外钩子。

### 6.5 关卡切换

`LevelManager.OnLevelBeginInitializing`（`LevelManager.cs:266`）里 `_tracked.Clear()` + 清空 collider 缓存。此时旧对象即将被销毁，**不尝试还原**（对象都没了）。`OnBeforeDeactivate()` 里再幂等清一次（对齐 `HeadshotBulletRefund\src\ModBehaviour.cs` 的写法）。

### 6.6 与现有 mod 共存

| 对象 | 分析 | 处理 |
| --- | --- | --- |
| `LootNearByItem`（已装，批量搜附近容器） | 它会让一批容器同时 `RemoveAt` → 本 mod 下次扫描重算。**功能兼容** | 无需处理（本设计不订阅事件，天然免疫"一次爆一堆事件"） |
| 别的 mod 也染交互点颜色 | MPB 优先级高于材质属性，谁后写谁生效 | README 注明本 mod 用 MPB 染 `InteractMarker`；本 mod 从不写 `sharedMaterial` |
| `ValuePerKg` / `ImNotGonnaPay` / `DuckovBetterRealDog` | 不同子系统，无重叠 | — |
| 全局 shader float `Interactable` | `InteractHUD.cs:29/75` 每帧写，控制交互点整体显隐 | **绝对不碰** |
| 别的 mod 用 Harmony 补 `InteractableBase` | 本 mod 零补丁，只调公开 API | 天然共存 |

### 6.7 已知不确定项

| # | 未确认 | 关闭方式 |
| --- | --- | --- |
| 1 | MPB 是否影响交互点可见颜色 | Phase 0 探针 |
| 2 | 交互点的具体材质属性名（`_Color`？`_Tint`？`_EmissionColor`？） | Phase 0 探针 |
| 3 | 白色交互点在多远处还看得见（决定 `scanRadius` 默认值） | Phase 0 目视 + 探针 |
| 4 | `Outlinable` 的确切颜色成员名 + 运行时新挂能否立刻渲染 | Phase 0 反射打印 + 目视 |
| 5 | 游戏自带的品质色具体数值（在序列化资产里，代码里读不到） | Phase 0 打印 7 个 `shadowColor` |
| 6 | 一个关卡实际有多少容器 | Phase 1 日志计数 |

---

## 7. 配置项

两种方式，优先级：**内置默认 < `config.json` < 游戏内 ModConfig 菜单**（沿用 `HeadshotBulletRefund` 的三级覆盖；ModConfig 未安装时只用前两级，不产生硬依赖）。

| 界面名 | 键 | 默认 | 范围 | 说明 |
| --- | --- | --- | --- | --- |
| 总开关 | `Enabled` | 开 | 开/关 | 关掉后 `Update` 第一行就返回 |
| 表现方式 | `Mode` | `Marker` | `Marker` / `Outline` / `Both` | 路线 B / 路线 A / 两者 |
| 排序依据 | `RankBy` | `Quality` | `Quality` / `Value` | 用最高品质还是最高价值选色 |
| 最低品质 | `MinQuality` | `White` | `None`–`Red` | 低于此品质不上色（空箱永远不上色） |
| 上色半径 | `ScanRadius` | 20 | 5–60 | 米 |
| 还原半径 | `ReleaseRadius` | 30 | 5–80 | 超出即还原，滞回避免抖动 |
| 扫描间隔 | `ScanInterval` | 0.5 | 0.1–2.0 | 秒 |
| 移动触发距离 | `ScanOnMoveDistance` | 1.5 | 0–10 | 站着不动就完全不扫 |
| 用游戏品质色 | `UseGameColors` | 开 | 开/关 | 关掉用内置六色 |
| 已搜容器也上色 | `TintUsedMarkers` | 关 | 开/关 | |
| 描边强度 | `OutlineWidth` | 1.0 | 0.1–5.0 | 仅 `Mode` 含 `Outline` 时有效 |
| 自定义颜色 | `Colors` | 空 | 6 个 `#RRGGBB` | **只在 `config.json`**，不进 ModConfig 菜单 |

---

## 8. 明确不改的东西

- 不打任何 Harmony 补丁；不修改 `InteractableBase` / `InteractableLootbox` / `InteractMarker` / `Inventory` / `Item` 的任何代码。
- 不改掉落、不改容器容量、不改物品品质/价值、不改存档结构。
- 不读也不写全局 shader float `Interactable`（`InteractHUD.cs:29/75`）。
- 不动 `InteractHUD` / `InteractSelectionHUD`（屏幕空间那套瞄准 UI）。
- 不写 `renderer.sharedMaterial`；除 §6.2 的降级路径外不写 `renderer.material`。
- 不硬依赖 ModConfig、EPO 之外无第三方依赖（EPO 是**游戏自带**的，不是我们引入的）。
- 不新增任何美术资产（路线 A 复用的 `Outlinable` 来自游戏自带 EPO）。

---

## 9. 分阶段实施计划

> 每阶段先过 `tools\verify-contract.ps1`（静态校验：入口类型名 == `<name>.ModBehaviour`、DLL 名 == `info.ini` 的 `name`、基类 == `Duckov.Modding.ModBehaviour`），再做运行时验证。

### Phase 0 — 运行时探针（**最高优先级**，因为 §6.7 的 5 个未确认全在这一步关闭）
- **做什么**：一个只打日志、不改任何颜色的最小 DLL。进游戏后按一个热键，对视野内最近的容器打印：交互点层级、每个子 Renderer 的 `sharedMaterial.shader.name` 与全部属性名、7 个 `shadowColor` 的值、主角到该容器的距离；然后再按一次，对一个容器强写 6 种颜色，逐个截图。
- **另外单测**：`AddComponent<Outlinable>()` + 反射打印 `Outlinable` 的全部 public 成员 → 找出颜色成员名 → 挂上并观察是否真的出现描边。
- **验证**：在容器最密集的关卡里完成上述打印与截图；把答案回填到本文件 §6.7，并据此定 `scanRadius` 默认值。
- **交付**：`探针报告`（一段结论 + 截图），不是代码。

### Phase 1 — 骨架 + 数据层（先不上色）
- **做什么**：`ModBehaviour` 生命周期、`LootScanner` 邻近扫描、`MaxQuality` 计算，输出日志 `[box] q=4 at 12.3m`。
- **验证**：a) 手工清点 5 个容器内的最高品质，与日志逐一对上；b) 边搜边拿，日志里品质单调下降、清空后为 0；c) `Loading` 期间不产出错误值（日志能看到"延后"）；d) Profiler 里本 mod 稳态 GC Alloc = **0 B/帧**；e) 日志里打出全关卡容器总数（关闭 §6.7#6）。

### Phase 2 — 路线 B 上色
- **做什么**：`MarkerPainter` + `RarityColors`，按 `Mode=Marker` 真正上色。
- **验证**：a) 六种品质各造一个容器 → 6 张对比截图；b) 搜过的容器转回原生 used 视觉且不再染色；c) 空箱无残留颜色；d) 走出 30 m 后颜色还原；e) 反复开关 mod 不留残留。

### Phase 3 — 路线 A 描边（可选，默认关）
- **做什么**：`OutlinePainter`，按 `Mode` 挂/摘 `Outlinable`。
- **验证**：a) 描边颜色随品质变化；b) 只对 `scanRadius` 内的容器描边；c) 用 Profiler 量出"每多一个描边物体的帧时间增量"，写进 README；d) 关闭 `Mode` 后帧时间回到基线。

### Phase 4 — 配置与交付形态
- **做什么**：`ModSettings` + `ModConfigBridge`（复用）+ `info.ini` + `config.json` + `preview.png` + `install.ps1` + `verify-contract.ps1` + README。
- **验证**：a) 干净环境（先删掉 mod 目录再装）启动成功、日志出现 `Mod Loaded: LootRarityMarker`、无 `NullReferenceException`；b) `Mod Settings` 标签页出现本 mod 的选项且改完立刻生效；c) 每个配置项单独改一遍，行为符合 §7 表；d) 与已装的 5 个 mod 全部同时启用跑一局，无报错、无帧时间尖峰。

---

## 10. 交付物

```
LootRarityMarker\
├─ PLAN.md                      本文件
├─ README.md                    安装/启用/配置/验证/性能说明
├─ src\*.csproj + *.cs          §3.1
├─ tools\install.ps1            同步 dist\ 并安装到游戏（同名目录自动备份）
├─ tools\verify-contract.ps1    反射校验加载契约
├─ tools\make-preview.py        生成 256×256 preview.png
└─ dist\                        info.ini / LootRarityMarker.dll / config.json / preview.png
```

安装位置：`d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data\Mods\LootRarityMarker\`

重建：
```powershell
dotnet build .\src\LootRarityMarker.csproj -c Release
.\tools\install.ps1
```

---

## 11. 需要你现在拍板的 3 件事

1. **默认走哪条路线？** 我建议 **路线 B（染交互点）默认开、路线 A（真描边）默认关但做出来给你切**。理由：B 零风险零成本、改动最小；A 更醒目但每帧成本与描边物体数线性相关，且需要 Phase 0 先证实 `Outlinable` 的 API。
2. **排序依据默认用"最高品质"还是"最高价值"？** 你原话是"最高价值"但颜色是按品质给的；我建议默认 `Quality`（两者在本作里高度相关，而颜色语义天然属于品质），`Value` 作为可切换项。
3. **要不要"移动才扫描"这个优化？** 我建议要（站着不动完全不扫描）。代价是原地转视角时不会重扫——但原地转视角本来也不需要重扫。
