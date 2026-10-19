# 更好的搜索显示 / LootRarityMarker

为附近容器的交互点添加价值提示：不用打开箱子，就能从颜色判断里面是否值得查看。

当前发布版本：**1.0.2**

## 功能

- 根据容器内最高价值物品的稀有度，为交互点染色：白、绿、蓝、紫、橙、浅红、深红。
- 如果容器内包含玩家手动加入愿望单的物品，颜色覆盖为亮青色 `#00E5FF`。
- 愿望单颜色同时用于交互点和容器本体描边。
- 支持按物品价值显示，以及按容器内单件最高价值增强交互点亮度。
- 支持游戏自带 EPOOutline 容器描边；默认关闭。
- 愿望单变化、容器内容变化、换关卡和设置变化都会通过工作队列刷新。
- 容器离开 `scanRadius` 后会恢复原始交互点，并释放对应的描边名额。
- 不使用 Harmony，不需要额外安装或携带 `0Harmony.dll`。

空容器会恢复为游戏原本的显示状态。移除空容器由其他订阅 Mod 负责，本 Mod 不重复实现。

## 安装

### 手动安装

将 `dist/` 内的文件整体复制到：

```text
<游戏目录>\Duckov_Data\Mods\LootRarityMarker\
```

安装后目录应包含：

```text
LootRarityMarker.dll
info.ini
config.json
preview.png
```

### 使用安装脚本

在本项目目录中使用 PowerShell 7：

```powershell
dotnet build -c Release .\src\LootRarityMarker.csproj
pwsh -ExecutionPolicy Bypass -File .\tools\install.ps1
```

安装脚本会：

1. 将 Release DLL 同步到 `dist/`；
2. 检查 DLL 的游戏加载契约和依赖；
3. 将旧版本备份到 `Duckov_Data\ModBackups\`；
4. 安装新版本并检查必要文件。

如果只想制作发布包而不安装到游戏，构建后还需要把
`src\bin\Release\LootRarityMarker.dll` 复制到 `dist/`；安装脚本会自动完成这一步。

## 使用与显示规则

### 普通品质颜色

容器颜色沿用「物品价值稀有度与搜索音效」Mod 的判定规则，但本 Mod 不依赖该 Mod，相关规则已经内置。

| 档位 | 颜色 |
|---|---|
| White | 白色 |
| Green | 绿色 |
| Blue | 蓝色 |
| Purple | 紫色 |
| Orange | 橙色 |
| LightRed | 浅红色 |
| Red | 深红色 |

默认情况下，容器使用其中最高档物品的颜色。`rankByValue=true` 时，先按 `Item.Value` 选择目标物品，再使用该物品的品质颜色。

### 愿望单颜色

只要容器中存在一个玩家手动加入愿望单的物品，容器交互点就显示为亮青色 `#00E5FF`，覆盖普通品质颜色。

愿望单发生变化时：

1. 游戏触发 `ItemWishlist.OnWishlistChanged`；
2. 已跟踪容器被标记为脏数据并加入既有工作队列；
3. 工作队列在单帧预算内逐步刷新。

不会为了愿望单状态增加每帧扫描。

### 容器描边

在设置中打开 `Outline` 或将 `outline` 设为 `true` 后，Mod 会尝试使用游戏自带的 EPOOutline 为容器本体添加描边。

- 默认关闭，不影响默认性能。
- 描边颜色与交互点颜色一致；愿望单容器使用亮青色描边。
- 同时最多处理 16 个容器。
- 单个容器最多保留 6 个描边渲染器。
- 超过 4 米的巨型渲染器会被过滤，避免出现覆盖半个屏幕的大描边。
- 如果游戏版本没有可用的 EPOOutline，只会停用描边，交互点染色仍然有效。

## 配置

配置文件位于 Mod 目录下的 `config.json`。当前发布包中的默认配置为：

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

优先级为：**代码默认值 < `config.json` < ModConfig 存档值**。

| 配置项 | 默认值 | 说明 |
|---|---:|---|
| `enabled` | `true` | 总开关；关闭后会恢复已染色的交互点和描边 |
| `scanRadius` | `25` | 扫描半径，单位为米 |
| `scanIntervalMs` | `500` | 移动时的最小扫描间隔，单位为毫秒 |
| `scanOnlyWhenMoving` | `true` | 静止时不主动扫描，降低常态开销 |
| `scanMoveDistance` | `1.0` | 移动超过多少米后触发扫描 |
| `rankByValue` | `false` | 是否按 `Item.Value` 选择容器内目标物品 |
| `glowByValue` | `false` | 是否按容器内单件最高价值增强亮度 |
| `outline` | `false` | 是否启用容器本体描边 |
| `diagnostics` | `false` | 是否输出详细诊断日志 |
| `maxWorkPerFrame` | `8` | 每帧最多处理的容器数量，实际还受时间预算限制 |
| `colorProperties` | 空 | 自定义交互点颜色属性名，逗号分隔 |

安装 ModConfig 后，以下项目可以在游戏设置面板中调整：

- `enabled`
- `scanRadius`
- `scanIntervalMs`
- `scanOnlyWhenMoving`
- `rankByValue`
- `glowByValue`
- `outline`
- `diagnostics`
- `maxWorkPerFrame`

`scanMoveDistance` 和 `colorProperties` 只能通过 `config.json` 调整。

## 性能设计

- 使用 `Physics.OverlapSphereNonAlloc`，复用碰撞体缓冲区。
- 默认静止时不扫描；稳态每帧只进行轻量状态判断。
- 容器内容未变化时只检查脏标记，不重复遍历物品。
- 内容变化和愿望单变化通过事件将容器加入工作队列。
- 完整扫描确认容器离开范围后，会清除颜色、销毁描边并解绑内容事件。
- 工作队列同时受数量上限和约 2 毫秒的单帧时间预算限制。
- `MaterialPropertyBlock`、字典和列表复用，不修改共享材质。
- 描边有容器数量、渲染器数量和尺寸限制，默认关闭。

换关卡、修改设置或大量容器同时变化时，颜色可能分几帧完成刷新，这是为了避免集中占用单帧时间。

## 兼容性与限制

- 只处理 `InteractableLootbox`，包括箱子、尸体容器和任务容器。
- 不处理拾取物、门、NPC 等没有容器内容的交互对象。
- 扫描范围是以玩家为中心的球形范围，不做视线遮挡检测。
- 容器按整体判定，不区分单个物品槽位。
- 不修改掉落、存档、物品品质、物品价值或容器容量。
- 不修改游戏共享材质，不添加 Shader、贴图或其他美术资产。
- 不使用 Harmony，不会与依赖 Harmony 的其他 Mod 产生补丁依赖关系。

## 故障排查

### 交互点没有颜色

1. 确认 `enabled=true`。
2. 确认安装目录是 `Duckov_Data\Mods\LootRarityMarker`。
3. 删除 Mods 目录中遗留的同名备份目录，或重新运行 `install.ps1`。
4. 将 `diagnostics` 设为 `true` 后重启游戏，查看：

```text
%USERPROFILE%\AppData\LocalLow\TeamSoda\Duckov\Player.log
```

重点搜索以下日志：

```text
[LootRarityMarker] 找不到 "Interactable" 层
[LootRarityMarker] 交互点材质上没找到可染的属性
[LootRarityMarker] 未检测到可用的 ModConfig
```

### 描边没有显示

确认 `outline=true`。然后查看日志中是否出现：

```text
[LootRarityMarker] 路线 A 可用：已找到游戏自带的 EPOOutline（EPO）。
```

如果出现“路线 A 不可用”，通常表示当前游戏版本没有加载可用的 EPOOutline。此时交互点染色不受影响。

### 性能或刷新延迟

- 将 `maxWorkPerFrame` 调小，减少单帧工作量；
- 将 `scanRadius` 调小，减少扫描对象数量；
- 将 `scanIntervalMs` 调大，降低移动时扫描频率；
- 如果开启了描边，先关闭 `outline` 对比。

## 发布内容

对普通玩家发布时，只需要提供 `dist/`：

```text
dist/
├── LootRarityMarker.dll
├── info.ini
├── config.json
└── preview.png
```

源码、构建脚本、验证脚本和探测资料保留在仓库中，不需要复制到游戏 Mods 目录。

## 开发者验证

```powershell
dotnet build -c Release .\src\LootRarityMarker.csproj

pwsh -ExecutionPolicy Bypass -File .\tools\verify-contract.ps1 `
  -Dll .\dist\LootRarityMarker.dll `
  -ManagedDir 'D:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data\Managed' `
  -ModName LootRarityMarker
```

发布前应确认：

- 构建无错误、无警告；
- `CONTRACT: PASS`；
- `dist/` 不包含 `0Harmony.dll`、PDB 或临时文件；
- `info.ini` 的版本号与发布说明一致；
- 游戏内实际启动后确认交互点颜色和愿望单亮青色效果。

## 版本记录

### 1.0.2

- 修复容器离开 `scanRadius` 后仍长期占用描边名额的问题。
- 超出范围的容器会恢复交互点原样、销毁描边并解绑内容事件。
- 碰撞体缓冲区满时跳过范围释放，避免误清理未被枚举到的容器。

### 1.0.1

- 增加愿望单容器标记。
- 愿望单颜色改为亮青色 `#00E5FF`，避免与紫色稀有度混淆。
- 愿望单变化通过现有工作队列刷新，不做每帧扫描。
- 保留正式容器描边功能，默认关闭。
- 移除自定义搜索速度功能及 Harmony 依赖。
