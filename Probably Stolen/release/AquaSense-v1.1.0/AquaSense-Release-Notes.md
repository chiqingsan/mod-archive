# AquaSense 1.1.0 Release Notes

## 中文发布说明

AquaSense 是一个面向 Probably Stolen Demo 的水系统辅助 Mod。它把水质扫描、原版水质标签和过滤器维护整合到日常库存操作中，同时保留没有扫描仪时的原版游戏流程。

### 核心功能

- 拥有水质扫描仪后，悬浮在装水容器上即可查看水质。
- 中文标题示例：`水质详情 (9960)  优质水`。
- 非中文标题示例：`Water composition (9960)  High Quality Water`。
- 纯度数字使用游戏内部返回的整数值，不额外换算。
- 水质成分统一使用 `ml`，四舍五入后为 `0.00%` 的项目自动隐藏。
- 支持标准瓶装水以及游戏中其他可识别的装水容器。
- 水质标签直接使用游戏原版逻辑，因此可以显示普通水、优质水、幽灵水等游戏已有标签。

### 游戏流程保持

没有购买水质扫描仪时，不会显示水质成分，也不主动刷新水质标签，原版 Tooltip 和原版标签流程保持不变。

购买水质扫描仪后，倒水或转移液体完成时，会刷新游戏原有的水质标签，减少重复查看和手动整理的操作。

### 过滤器自动替换

当水罐内过滤器耐久降为 0 时，AquaSense 会检索玩家拥有的顶层物品以及多层嵌套容器中的替换过滤器。

替换优先级为：

1. 高级过滤器
2. 普通过滤器
3. 其他 Mod 添加的兼容过滤器

交换前会检查替换过滤器能否放入水罐，以及旧过滤器能否放回替换过滤器原来的位置。如果形状、边界或重叠检查失败，则不进行交换。

交换完成后会在游戏界面提示玩家，例如：

```text
滤水器耐久已耗尽, 已替换为高级滤水器
```

### 1.1.0 新增

- **滤芯耐久显示**：悬浮装有过滤器的水容器时，在面板中显示过滤器名称与耐久度，例如 `[滤水器]  耐久度:  140/400`。该行使用 `#CCB987` 金色文字，水罐为空时依然显示。
- **过滤器替换提示**：替换成功后弹出提示，告知替换前后的过滤器名称，中英双语。
- **三项功能开关**：自动更换滤芯、显示滤芯耐久、倒水后自动更新标签均可在 MelonPreferences 中独立开关，默认全部开启。
- **交换失败保护**：替换后校验两个过滤器是否落到预期位置。若交换未完成，会把脱离库存的过滤器放回原处，避免出现滤芯位空白；同时不再把"读取耐久失败"当作"滤芯仍然可用"，修复了耐久耗尽后偶发不替换的问题。

### 1.1.0 修复

- **不再把紫外线灯（`uv_filter`）当成滤芯替换**。紫外线灯的内部标识符本身包含 `filter`，旧版按名称判断时会把它识别为滤芯；放在水容器里时会被换成滤水器。现已显式排除。
- **同样修复工业布料（`hydroreed_filter`，韧棉织物）**。该物品的游戏描述写明无法用于净水，但标识符同样包含 `filter`。
- 替换前会先确认水容器槽中的物品确实是滤芯，才会进行替换。
- 滤芯耐久行也只会对真正的滤芯显示。

> 请注意：本版本的过滤器识别仍基于物品名称与标识符。如果还有其他可以放入水容器槽、名称中又包含 `filter` 的物品，仍可能被误判。遇到这种情况请反馈该物品的名称。

### 设置

在 MelonPreferences 的 `AquaSense` 分类下：

| 配置项 | 默认 | 说明 |
| --- | --- | --- |
| 滤芯耗尽后自动更换 | 开启 | 关闭后不再自动替换耗尽的过滤器 |
| 悬浮面板显示滤芯耐久 | 开启 | 关闭后悬浮面板不显示耐久行 |
| 倒水后自动更新水质标签 | 开启 | 关闭后倒水、转移液体不再刷新标签 |

水质成分显示本身不受以上开关影响，只取决于玩家是否拥有水质扫描仪。

### 兼容性

当前版本针对 Probably Stolen Demo `0.46D`、Unity `2021.3.45f2` 和当前 IL2CPP 程序集构建。游戏更新后，如果水系统、库存系统或方法签名发生变化，可能需要重新编译。

正式版不安装高风险的通用库存移动监听，也不输出正常运行诊断日志。

## English Release Notes

AquaSense is a water-system utility mod for Probably Stolen Demo. It combines scanner-gated water inspection, the game's original water labels, and automatic filter maintenance while preserving the original gameplay flow before the scanner is purchased.

### Core features

- Once you own a water quality scanner, simply hover over a water container to view its water quality.
- Chinese title example: `水质详情 (9960)  优质水`.
- Non-Chinese title example: `Water composition (9960)  High Quality Water`.
- Purity values use the raw integer returned by the game engine without additional conversion.
- Water composition is displayed in `ml`; components rounding to `0.00%` are automatically hidden.
- Supports standard bottled water as well as other recognizable water containers in the game.
- Uses the game's native logic for quality labels, ensuring existing labels such as Ordinary Water, High-Quality Water, and Ghost Water are displayed correctly.

### Gameplay flow preservation

Without the water quality scanner, water composition is not displayed and quality labels are not automatically refreshed; the vanilla tooltip and labeling behavior remain unchanged.

After purchasing the scanner, the game's native quality labels refresh upon completing a pour or liquid transfer, reducing the need for repetitive checks and manual sorting.

### Automatic filter replacement

When a filter inside a water tank reaches 0 durability, AquaSense searches for a replacement filter among items in the player's top-level inventory or within nested containers.

Replacement priority:

1. Advanced Filter
2. Vanilla Standard Filter
3. Compatible filters added by other mods

Before swapping, the mod checks if the replacement filter fits into the water tank and if the old filter can be placed in the replacement filter's original slot. If checks regarding shape, boundaries, or overlaps fail, no swap occurs.

When the swap succeeds, the game shows a notification such as:

```text
Basic Water Filter ran out of durability and was replaced with Advanced Water Filter
```

### New in 1.1.0

- **Filter durability display**: hovering a water container that holds a filter now shows the filter's name and durability, for example `[Basic Water Filter]  Durability:  140/400`. The line is drawn in `#CCB987` and also appears when the container is empty.
- **Replacement notification**: a bilingual message reports which filter was replaced and by what.
- **Three feature toggles**: automatic replacement, durability display, and automatic label refresh each have an independent MelonPreferences switch, all enabled by default.
- **Swap failure protection**: after a swap the mod verifies that both filters landed where intended. If the swap did not complete, a detached filter is returned to its original position so the filter slot never ends up empty. A failed durability read is no longer treated as "filter still good", fixing intermittent missed replacements at zero durability.

### Fixed in 1.1.0

- **The UV lamp (`uv_filter`) is no longer mistaken for a filter and swapped out.** Its internal identifier contains `filter`, so the previous name-based check identified it as a filter; when placed in a water container it was replaced by a water filter. It is now explicitly excluded.
- **The same fix applies to the industrial cloth (`hydroreed_filter`).** Its game description states it cannot be used for water purification, yet its identifier also contains `filter`.
- Replacement now verifies that the item in the water-container slot really is a filter before swapping.
- The durability line is likewise only shown for real filters.

> Note: filter detection in this version is still based on item names and identifiers. Any other item that can be placed in the water-container slot and whose name contains `filter` could still be misidentified. If you encounter one, please report the item's name.

### Settings

Under the `AquaSense` category in MelonPreferences:

| Setting | Default | Effect |
| --- | --- | --- |
| Automatically replace depleted filters | On | When off, depleted filters are no longer swapped |
| Show filter durability in tooltip | On | When off, the durability line is hidden |
| Refresh water labels after pouring | On | When off, pouring or transferring no longer refreshes labels |

The water composition readout itself is not affected by these switches; it only depends on whether the player owns a water quality scanner.

### Compatibility

This release targets Probably Stolen Demo `0.46D`, Unity `2021.3.45f2`, and the current IL2CPP assemblies. A game update that changes the water system, inventory system, or method signatures may require a rebuild.

The release does not install high-risk generic inventory movement listeners and does not write normal runtime diagnostics to the MelonLoader log.
