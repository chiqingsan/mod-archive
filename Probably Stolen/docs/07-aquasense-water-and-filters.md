# AquaSense 水系统与过滤器

本文记录 AquaSense `1.1.2` 在 Probably Stolen Demo `0.46D` 上的接口与实现。原生库存接口已用本地二进制静态核对，当前修改已编译并通过隔离回归测试；1.1.2 的实机帧耗时尚待采集。本文不是官方 API 文档。

## 功能边界

AquaSense 当前包含五项功能：

1. 拥有水质扫描仪时，悬浮在装水容器上显示水质纯度和成分（不受设置开关影响，只受是否拥有扫描仪控制）。
2. 拥有水质扫描仪时，倒水或转移液体后调用游戏原有逻辑更新容器标签。可在 MelonPreferences 中开关 `AutoUpdateWaterLabels`，默认开启。
3. 悬浮水容器时显示槽内滤芯的名称与耐久度，水罐为空时同样显示。可在 MelonPreferences 中开关 `ShowFilterDurability`，默认开启。
4. 水罐中的过滤器耐久归零后，自动从玩家拥有的物品和嵌套容器中寻找替换品。可在 MelonPreferences 中开关 `AutoReplaceFilters`，默认开启；替换成功后通过游戏 UI 提示玩家（中英双语）。
5. 交换过滤器前后做安全检查：检查所有权、实时槽位、插入/移除锁和原生形状规则；失败时按保存的形状恢复，暂时无法恢复时保留物品引用并优先重试恢复。

三个开关的具体说明见「设置开关」章节。

QuickCounter 不属于 AquaSense 的功能范围。

## 水质 Tooltip

当前使用的 Harmony Postfix：

```text
WaterHelper.CreateWaterContainerTooltip(RichTextBuilder, GameItem)
WaterFeatureHelper.CreateWaterFeatureTooltip(RichTextBuilder, GameItem)
GameItemElement.GetTooltipBasic()
```

处理顺序：

```text
生成原版 Tooltip
  -> 检查目标是否为装水容器
  -> 检查玩家是否拥有水质扫描仪
  -> WaterHelper.GetWaterPurity(item)
  -> 读取 Liquid.LiquidDict / Liquid.Liquids
  -> 从 GameItem.state.GetTag(...) 读取成分数量
  -> 添加水质标题和成分行
```

前两个 Postfix 覆盖原版水系统 Tooltip；`GameItemElement.GetTooltipBasic()` 是通用兜底，
用于兼容没有进入原版水 Tooltip 分支、但仍使用游戏液体容器数据结构的 Mod 物品。同帧按 builder 与物品去重，并记录追加内容的 slice 指针：A/B/A 交错不会重复，builder 清空后可以重新追加。

语言判断使用游戏当前 locale 是否以 `zh` 开头。不会使用 `cn` 作为语言代码。

标题格式：

```text
中文：水质详情 (9960)  优质水
其他语言：Water composition (9960)  High Quality Water
```

其中 `9960` 是 `WaterHelper.GetWaterPurity` 返回的容器水质纯度整数。

水质标签由游戏原版 `WaterFeatureHelper.GetWaterDisplayName(GameItem)` 返回，AquaSense 不自行维护普通水、优质水、幽灵水等标签映射。

水容器识别采用三层策略：

1. 现有 identifier 白名单，作为零额外调用的快速路径和异常 Mod 的兜底。
2. `LIQUID_CONTAINER_TAG` 与正数 `LIQUID_CONTAINER_CAPACITY`，自动覆盖按原版液体容器契约创建的 Mod 物品。
3. 对前两层无法判断的物品调用原生 `WaterHelper.CanUseWaterContainer(GameItem)`。

不再按 identifier 缓存实例能力。相同物品 ID 的实例标签可能不同，动态标签变化也必须立即生效；白名单和标签仍作为短路路径，未知容器使用原生查询。

当前已知的标准瓶装水标识为 `bottled_water`；它与高级瓶装水、大小瓶装水一样属于可显示水质详情的水容器。
BrewingExpansion 3.6.2 的水容器标识为 `be_xuxu_super_water_bottle`、
`be_xuxu_small_water_bottle`、`be_xuxu_slim_water_bottle` 和 `be_clear_brew_water_bucket`，
这些标识也纳入 AquaSense 的水容器识别。

成分数量统一按 `ml` 显示。单项数量仍按游戏 int 标签读取，总量累加使用 long，避免大容量容器的多项正数成分相加溢出；如果成分占比四舍五入后为 `0.00%`，该行隐藏。

没有水质扫描仪时，Postfix 不追加水质信息，保持原版 Tooltip。

Tooltip 仅查询扫描仪检测缓存，缓存过期时请求后台式的主线程分帧扫描，不在悬停回调中同步遍历库存。首次检测尚未完成时可能暂不显示详情，扫描完成后后续 Tooltip 生效。扫描仪检测和维护共享同一轮遍历；关闭自动更换后，悬停仍能请求扫描仪检测。倒水不改变库存所有权，不再为倒水重复丢弃库存快照。切换 PlayerStore 或执行 LoadGame 后清除旧缓存和待处理工作。

### 滤芯耐久行

容器中装有滤芯时，在水质成分之后追加一行滤芯耐久。由 `WaterHelper.GetFilterItem` 判断实际槽内物品，不额外添加必须具备 `FILTER_CONTAINER_TAG` 的限制：

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
  -> UpdateMaintenance：一轮结束后约 2 秒启动下一轮
  -> 按约 2ms 的每帧预算推进迭代器
  -> 队列遍历玩家物品图，按指针去重，不递归调用 FindAllChildItems
  -> 同轮检测扫描仪、读取容器中的滤芯耐久
  -> 只有存在耗尽滤芯才收集备用滤芯
  -> 计算一次优先级，按三个稳定桶依次尝试候选
  -> 交换前复核实时父库存、所有权、耐久、锁和放置条件
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

交换前保存两个过滤器原始父库存和 `modifiedShape.Clone()`。形状已包含放置变换，不再额外叠加 `modifiedXOrigin/YOrigin`。

预检使用 `GridShapeBuilder(itemShape).SetTransform(destinationShape).Build()` 构造放置形状：

- 网格调用原生 `CheckShapePlacement`，将参与交换的两个物品加入忽略列表。边界、异形空洞与碰撞由游戏规则判断。
- 单槽检查实际占用、`MayHaveValidInventorySlot`、物品状态与父形状/状态修饰规则。
- 双向检查 `IsInsertLocked` 和 `IsRemoveLocked`；锁定候选会被跳过，不阻塞后续候选。
- 拖拽物品或原生 `EmporiumEntry.swapBufferElement` 已被占用时延后自动交换。
- 真正交换前重新查询当前拥有的根物品，沿实时父链确认容器与备用滤芯仍归玩家拥有。扫描期间售出的物品不会继续参与交换。

交换后 Validate 并校验两个物品的父库存、位置/旋转/翻转，以及 `WaterHelper.GetFilterItem` 返回的实际滤芯。最终状态确实完成交换时才提示成功，不单纯依赖原生 bool 返回值。

失败恢复使用 `GraphUtils.PlaceAtShape` 与原始形状，不再随机寻找一个空位。处理原生交换缓冲槽、脱离库存、旧滤芯已到来源库存等部分交换状态；一般先归还旧滤芯以腾出备用滤芯原位，目标槽中已有替换品时先处理对应的反向移动。读取父库存失败不等于 detached，不对无法读取的物品盲目接收。

恢复仍受真实锁、占用和原生方法失败影响，不能保证每次原生故障都能立即修复。未完成恢复时保留闭包中的物品、库存和形状引用，约 2 秒重试一次，并暂停其他自动交换。只允许在原来源、原目标及本次使用的游戏缓冲槽之间恢复，不强制抢回被其他库存接管的物品。切换存档后不再重试旧世界对象。

过滤器父链使用指针集合检测循环，不再以 16 层作为兼容上限。`water_filter_adv` 和 `water_filter` 的优先级先按稳定 identifier 判断，未知 Mod 继续使用名称/标识符兼容识别与已有非滤芯排除规则。

## 已确认的风险边界

- `GameInventory.UncheckedAccept` 是高风险 Harmony 目标，正式版本不监听。
- 不直接修改 `GameItem.identifier`；高级过滤器的 identifier 应保持 `water_filter_adv`。
- 不直接修改库存集合；移动使用游戏的 `GraphUtils.SwapItemsInPlace`。
- 遍历和候选检查分帧推进，预算约 2ms；预算检查不能中断正在执行的一次原生方法，所以不承诺任何极端情况下都不超过 2ms。恢复与真实交换也需要完整执行。
- 正常运行不输出诊断明细；缺失补丁、读取失败、交换/恢复异常会输出 warning，避免静默失效。
- 诊断日志、候选过滤器明细和补丁安装过程日志只存在于开发排查阶段，不属于发布版。

## 当前已验证环境

```text
Game: Probably Stolen Demo 0.46D
Unity: 2021.3.45f2
MelonLoader: 0.7.3 Open-Beta / net6
Assembly-CSharp snapshot: 2026-09-22
Mod: AquaSense 1.1.2
```

## 1.1.2 修改依据与验证

修改针对 1.1.1 的无需求候选排序、锁定候选阻塞、部分交换恢复、实例缓存串状态、Tooltip 重入及成分总量溢出等复现问题。

当前游戏 `GameAssembly.dll` SHA256：`A2CAA0CECBF197CCD6B12DDEC7BB9E6FA0BC2CD2A5C52ABEF627AA919E2B9BFD`。
通过 Cpp2IL 元数据定位并静态反汇编核对 `SwapItemsInPlace`（RVA `0x7C8E40`）、`PlaceAtShape`（`0x7C8D40`）、`CheckShapePlacement`（`0x7B99D0`）和单槽检查。这些是静态接口/调用约定依据，不是实机运行结果。

回归工程直接链接当前 AquaSense.cs，使用边界桩模拟原生调用，不加载游戏 DLL，也不部署测试程序集：

```powershell
dotnet run --project "Probably Stolen/AquaSense.Tests/AquaSense.Tests.csproj" -c Release
dotnet build "Probably Stolen/AquaSense/AquaSense.csproj" -c Release -p:SkipDeploy=true
```

场景与局限见 [AquaSense.Tests/README.md](../AquaSense.Tests/README.md)。原生碰撞、IL2CPP 补丁启动和真实帧耗时仍需要在游戏内验收，不能用桩测试替代。
