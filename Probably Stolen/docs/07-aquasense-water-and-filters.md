# AquaSense 水系统与过滤器

本文记录 AquaSense（版本 `1.1.0`）开发过程中在 Probably Stolen Demo `0.46D` 上实机验证过的游戏逻辑和 API。内容是当前版本的行为记录，不是官方 API 文档。

## 功能边界

AquaSense 当前包含五项功能：

1. 拥有水质扫描仪时，悬浮在装水容器上显示水质纯度和成分（不受设置开关影响，只受是否拥有扫描仪控制）。
2. 拥有水质扫描仪时，倒水或转移液体后调用游戏原有逻辑更新容器标签。可在 MelonPreferences 中开关 `AutoUpdateWaterLabels`，默认开启。
3. 悬浮水容器时显示槽内滤芯的名称与耐久度，水罐为空时同样显示。可在 MelonPreferences 中开关 `ShowFilterDurability`，默认开启。
4. 水罐中的过滤器耐久归零后，自动从玩家拥有的物品和嵌套容器中寻找替换品。可在 MelonPreferences 中开关 `AutoReplaceFilters`，默认开启；替换成功后通过游戏 UI 提示玩家（中英双语）。
5. 交换过滤器前后做安全检查：交换前检查两个物品交换位置的形状、边界和重叠情况，并确认槽内物品确实是滤芯；交换后校验结果，失败时恢复并把脱钩物品放回原处。

三个开关的具体说明见「设置开关」章节。

QuickCounter 不属于 AquaSense 的功能范围。

## 水质 Tooltip

当前使用的 Harmony Postfix：

```text
WaterHelper.CreateWaterContainerTooltip(RichTextBuilder, GameItem)
WaterFeatureHelper.CreateWaterFeatureTooltip(RichTextBuilder, GameItem)
```

处理顺序：

```text
生成原版 Tooltip
  -> 检查玩家是否拥有水质扫描仪
  -> 检查目标是否为装水容器
  -> WaterHelper.GetWaterPurity(item)
  -> 读取 Liquid.LiquidDict / Liquid.Liquids
  -> 从 GameItem.state.GetTag(...) 读取成分数量
  -> 添加水质标题和成分行
```

语言判断使用游戏当前 locale 是否以 `zh` 开头。不会使用 `cn` 作为语言代码。

标题格式：

```text
中文：水质详情 (9960)  优质水
其他语言：Water composition (9960)  High Quality Water
```

其中 `9960` 是 `WaterHelper.GetWaterPurity` 返回的容器水质纯度整数。

水质标签由游戏原版 `WaterFeatureHelper.GetWaterDisplayName(GameItem)` 返回，AquaSense 不自行维护普通水、优质水、幽灵水等标签映射。

当前已知的标准瓶装水标识为 `bottled_water`；它与高级瓶装水、大小瓶装水一样属于可显示水质详情的水容器。

成分数量统一按 `ml` 显示。内部读取的数量保持游戏当前数值，不再做额外单位换算；如果成分占比四舍五入后为 `0.00%`，该行隐藏。

没有水质扫描仪时，Postfix 不追加水质信息，保持原版 Tooltip。

水质扫描仪检测目前有约 1 秒缓存，避免每次 Tooltip 调用都递归扫描全部库存。

### 滤芯耐久行

容器中装有滤芯时，在水质成分之后追加一行滤芯耐久：

```text
WaterHelper.GetFilterItem(container)
  -> DurabilityHelper.GetRealDurability(filter)
  -> DurabilityHelper.GetMaxDurability(filter)
```

格式为 `[滤芯名称]  耐久度:  当前/上限`（英文为 `[name]  Durability:  current/max`）。名称取 `GameItem.GetDisplayName(false)`。`GetMaxDurability` 取不到或返回 0 时只显示当前值。容器中没有滤芯时该行不出现。

行的位置：AquaSense 的 Postfix 只能在**全部原版内容之后**追加，无法插入原版内部。原版 tooltip 的实际绘制顺序是（标题由 AquaSense 追加，位于最前）：

```text
水罐                              <- 原版物品名
[水]                              <- 原版
估价价值 / 基础价值                 <- 原版
[已检测]-基础水                     <- 原版
水质详情 (9898) 基础水              <- AquaSense（标题）
水: 2079.25 ml (98.99%)           <- AquaSense（成分）
重金属 / 化学污染物                  <- AquaSense（成分）
[滤水器]  耐久度:  140/400          <- AquaSense（耐久行）
[=======  ] 2100 ml / 3000 ml     <- 原版容量条
[标签]幽灵水 / 可安装滤水器 / 双击以打开  <- 原版
```

因此耐久行位于成分之后、原版容量条之前。原版容量条与后续说明行由 native 代码绘制，Postfix 无法插入到它们之间；强行改位置需要前缀补丁或替换整个 tooltip 方法，代价高于收益，因此保持当前位置。

颜色为 `#CCB987`。`RichTextBuilder.AddLine` 接受的是 `RenderHandler.ColorPalette`，而该枚举是 int 底层、值即字面 `0xRRGGBB` 颜色（如 `White = 12895408 = 0xC4C4B0`），因此可以直接把十六进制值强制转换后传入。

**关键：`AddLine` 有四个颜色参数，必须只填第 3 个（文字色）。** 完整参数表：

```text
AddLine(text, ignoreEmpty, color, bold, italic, underline, strikethrough,
        backgroundColor, underlineColor, strikethroughColor)
```

把同一个颜色同时传给 `backgroundColor` 会在该行后面画出一条整行宽度的色带（曾误传导致出现黄条）。正确写法：

```csharp
private const int FilterDurabilityColor = 0xCCB987;
private const int NoColor = -1;

builder.AddLine(
    text, true,
    (RenderHandler.ColorPalette)FilterDurabilityColor,   // 文字色
    false, false, false, false,
    (RenderHandler.ColorPalette)NoColor,                 // backgroundColor
    (RenderHandler.ColorPalette)NoColor,                 // underlineColor
    (RenderHandler.ColorPalette)NoColor);                // strikethroughColor
```

`ColorPalette.None = -1` 表示不设置。标题和成分行不使用这个 helper，仍沿用全 `-1`。

可通过 MelonPreferences 开关，键标识 `ShowFilterDurability`，默认开启。

## 自动标签

当前使用的 Harmony Postfix：

```text
WaterHelper.TransferLiquid(GameItem, GameItem)
WaterFeatureHelper.OnLiquidContainerPourInto(GameItem, GameItem, bool)
```

拥有水质扫描仪时，液体转移结束后对两个参数分别尝试执行：

```text
WaterHelper.UpdateContainerGoodType(container)
WaterFeatureHelper.UpdateWaterFeature(container, true)
WaterFeatureHelper.OnWaterLabelUpdated(container)
```

只有玩家拥有水质扫描仪、目标是已知水容器且不为空时才处理。没有水质扫描仪时，液体转移钩子不调用标签刷新方法，因此保持游戏原有的标签更新流程。标签名称和分类由游戏原有逻辑决定，例如普通水、优质水、幽灵水等。

可通过 MelonPreferences 开关，键标识 `AutoUpdateWaterLabels`，默认开启。关闭后液体转移钩子不刷新标签。

## 玩家物品遍历

过滤器替换需要检索玩家拥有的所有容器，而不是只检索当前顶层库存。当前遍历入口：

```text
PlayerStore.FindAllItem(true)
PlayerStore.GetAllRepoEligibleItems()
```

随后使用队列和指针去重，遍历：

- `GameItem.FindAllChildItems()`
- `GameItem.child`
- `GameItem.children`
- `GraphNodeWindow`
- `GraphNodeStorage`
- `GameInventory.childItems`
- `GameSlotInventory.childItem`
- `GameGridInventory.items`
- `GameGridScrollableInventory.items`
- `GameInventory.children`

因此可以覆盖：

```text
顶层库存
  -> 顶层容器
    -> 容器中的容器
      -> 更深层容器
```

遍历使用 `HashSet<IntPtr>` 防止同一 IL2CPP 对象重复加入，也使用独立集合防止存储节点循环。

## 过滤器识别和优先级

### 水容器槽位不是滤芯专用

`WaterHelper.GetFilterItem(container)` 返回的是**水容器槽位里的物品**，不保证是滤芯。该槽位可以放其他物品，游戏物品数据里已确认的同槽位物品包括：

```text
water_filter        滤水器                <- 滤芯
water_filter_adv    高级滤水器             <- 滤芯
uv_filter           紫外线灯              <- 不是滤芯，杀菌灯具
hydroreed_filter    韧棉织物              <- 不是滤芯，游戏描述写明无法用于净水
black_lamp          黑光灯                <- 不是滤芯
```

游戏侧证据：`WaterHelper.OnUVUsed(GameItem waterContainer)` 的参数是水容器而不是灯具，说明 UV 功能作用在容器槽位上。

因此在做任何替换之前，必须同时校验**被替换物**和**替换物**都是滤芯。

### 识别规则

识别使用物品的 `identifier` 和 `name`：先排除已知的非滤芯槽位物品，再要求命中滤芯关键字。

```csharp
private static readonly string[] NonFilterSlotIdentifiers =
{
    "uv_filter", "uvfilter", "uv_lamp", "uvlamp",
    "black_lamp", "hydroreed"
};

// 命中排除表 -> 不是滤芯
// 否则：包含 filter / 过滤器 / 滤水器 之一 -> 是滤芯
```

**不能只用"名字含 filter"做正向匹配。** 紫外线灯的 identifier 是 `uv_filter`，韧棉织物的 identifier 是 `hydroreed_filter`，两者都包含 `filter`，纯正向匹配会把它们当成滤芯。这是 1.0.2 及更早版本的真实缺陷：把紫外线灯放进水容器后，它会被当作滤芯替换掉（用户实机反馈）。

排除表按 identifier 子串匹配，因为 identifier 稳定、不随语言变化；显示名会随本地化改变，不能作为判断依据。

不使用过宽的 `AmenitiesItemDirectory.WATER_FILTER` tag 判断，因为实测该判断会把普通物品误加入候选列表。

### 候选过滤器条件

- 不是当前水罐或当前耗尽过滤器。
- 是滤芯（先过排除表，再命中关键字）。
- 有 `parentInventory`。
- 不在其他水容器内部。
- `DurabilityHelper.GetRealDurability(item) > 0`。

优先级：

```text
0  高级过滤器：advanced / adv_filter / high_filter / 高级
1  原版普通过滤器：由 PreBuiltItemHelper.WaterFilter() 记录 identifier/name
2  其他符合过滤器命名规则的 Mod 过滤器
```

### 已知局限

排除表是黑名单而不是白名单。改成白名单（只认已知滤芯 identifier）会挡掉其他 Mod 添加的滤芯，破坏兼容性，因此保留"关键字通配 + 黑名单排除"的折中方案。**代价是：如果游戏或 Mod 新增了名称含 `filter` 的非滤芯槽位物品，仍可能被误判。** 遇到时把该物品的名称加入排除表即可。

### 结论口径

- 标识符清单（`uv_filter` / `hydroreed_filter` / `black_lamp`）：已从游戏数据 `resources.assets` 的本地化键确认。
- 紫外线灯被误替换：用户实机反馈确认。
- 排除表修复后的效果：待验证行为，尚未完成实机验证。

## 设置开关

三个功能都可以通过 MelonPreferences 开关，默认全部开启：

```csharp
var category = MelonPreferences.CreateCategory("AquaSense", "AquaSense");
var replace = category.CreateEntry("AutoReplaceFilters", true, "滤芯耗尽后自动更换");
var durability = category.CreateEntry("ShowFilterDurability", true, "悬浮面板显示滤芯耐久");
var labels = category.CreateEntry("AutoUpdateWaterLabels", true, "倒水后自动更新水质标签");
```

三个开关默认全部开启，分别对应：

| 键标识 | 默认 | 关闭后 |
| --- | --- | --- |
| `AutoReplaceFilters` | 开启 | 不再自动更换耗尽的滤芯 |
| `ShowFilterDurability` | 开启 | 悬浮面板不显示滤芯耐久行 |
| `AutoUpdateWaterLabels` | 开启 | 倒水/转移液体后不刷新水质标签 |

水质 Tooltip 本身（纯度与成分）不受这三个开关影响，只受玩家是否拥有水质扫描仪控制。

配置分类标识与显示名称均为 `AquaSense`；三个键标识已发布，不要改名。开关在游戏运行时读取，修改配置后无需重启即可生效（`AutoReplaceFilters` 在下一轮轮询时生效）。

## 自动更换的轮询

`AutoReplaceFilters` 开启时使用低频轮询：

```text
MelonMod.OnUpdate()
  -> 每 2 秒执行一次 TryReplaceDepletedFilters()
  -> 递归收集玩家物品
  -> 找到水罐
  -> WaterHelper.GetFilterItem(container)
  -> DurabilityHelper.GetRealDurability(filter)
```

## 替换提示

交换成功后通过游戏 UI 提示玩家：

```text
StoreUIManager.Instance?.Notify(message, "default")
```

`Notify` 的真实签名是 `Notify(string message, string color = "default")`：**第二个参数是颜色标签，不是正文**。只传一条消息；传两个字符串会让界面只显示第一个参数（曾经因此只显示"滤芯更换"）。该调用渲染为对话气泡样式，游戏没有独立的 Toast 控件。

提示文案使用两个过滤器的显示名（`GameItem.GetDisplayName(false)`，取不到时回退到 `identifier`），并按当前 locale 选择语言（`LocHelper.GetCurrentLocaleCode()` 是否以 `zh` 开头）：

```text
中文：滤水器耐久已耗尽, 已替换为高级滤水器
其他语言：Basic Water Filter ran out of durability and was replaced with Advanced Water Filter
```

名称在交换前读取，这样即使交换过程让物品短暂处于中间状态，提示内容仍然反映实际被替换的物品。交换失败（含恢复流程）时不提示，只记录 warning。

目前没有把 `GameInventory.UncheckedAccept`、`GameInventory.Expel` 或 `GraphUtils.SwapItemsInPlace` 安装成 Harmony 监听补丁。此前对 `GameInventory.UncheckedAccept` 安装补丁时曾造成游戏启动阶段原生崩溃，因此正式版本不依赖这些通用库存移动事件。

`DurabilityHelper.ChangeDurability` 是未来可以验证的事件候选。其他 Mod 已确认使用它扣除耐久，但尚未确认所有原版过滤器消耗路径都统一经过该方法。因此当前保留轮询作为稳定方案。

## 过滤器交换

交换使用游戏已有方法：

```text
GraphUtils.SwapItemsInPlace(depletedFilter, replacement)
```

交换前记录两个过滤器各自的 `parentInventory` 和当前网格位置，并验证双向交换：

```text
旧过滤器 -> 替换过滤器原来的位置
替换过滤器 -> 旧过滤器在水罐中的位置
```

网格库存检查包括：

- 目标位置是否超出库存边界。
- 替换后物品的完整形状是否可放入目标位置。
- 是否与目标库存中其他物品重叠。
- `GameGridInventory.inventoryShape` 的实际宽高。
- `GameGridScrollableInventory.width/height`。
- `GameSlotInventory` 单槽是否已被占用（occupant 必须是参与交换的两个物品之一），以及 `IsInsertLocked()`。

如果新旧物品尺寸不兼容，直接跳过交换，不删除任何过滤器。

`GraphUtils.SwapItemsInPlace` 不保证原子性：如果"放入"阶段失败，物品可能脱离所有库存，表现为水罐滤芯位变空且旧过滤器消失。因此交换后必须重新读取两个过滤器的 `parentInventory` 做校验：

```text
旧过滤器 parentInventory == 替换过滤器原库存？
替换过滤器 parentInventory == 水罐滤芯槽？
```

任一校验失败时执行恢复：把脱离库存的旧过滤器放回水罐槽位，把未成功放入的替换过滤器放回其原库存（`TryFindOneValidInventorySlot` + `SlotMarker.AcceptUnchecked`）。恢复不会把替换过滤器塞进无关的容器。

交换成功后调用：

```text
sourceInventory.Validate()
targetInventory.Validate()
container.Validate()
```

交换前会重新读取两个过滤器的实时 `parentInventory`，并拒绝源槽和目标槽为同一库存、或任一槽位 `IsInsertLocked()` 的情况；不再依赖物品快照中缓存的旧引用。

游戏的交换行为会把旧过滤器放回替换过滤器原来的位置，而不是删除旧过滤器。

## 已确认的风险边界

- `GameInventory.UncheckedAccept` 是高风险 Harmony 目标，正式版本不监听。
- 不直接修改 `GameItem.identifier`；高级过滤器的 identifier 应保持 `water_filter_adv`。
- 不直接修改库存集合；移动使用游戏的 `GraphUtils.SwapItemsInPlace`。
- 不在每帧递归扫描库存；当前间隔为 2 秒。
- AquaSense 正式版默认不向 MelonLoader 输出日志。
- 诊断日志、候选过滤器明细和补丁安装过程日志只存在于开发排查阶段，不属于发布版。

## 当前已验证环境

```text
Game: Probably Stolen Demo 0.46D
Unity: 2021.3.45f2
MelonLoader: 0.7.3 Open-Beta / net6
Assembly-CSharp snapshot: 2026-09-22
Mod: AquaSense 1.1.0
```
