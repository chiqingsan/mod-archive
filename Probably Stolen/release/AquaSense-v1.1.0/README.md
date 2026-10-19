# AquaSense

**Version / 版本：** 1.1.0  
**Author / 作者：** chiqingsan  
**Game / 游戏：** Probably Stolen Demo 0.46D

AquaSense adds a scanner-gated water-quality view, automatic water labels, filter durability readout, and reliable depleted-filter replacement.

AquaSense 为 Probably Stolen Demo 增加扫描仪门控的水质查看、自动水质标签、滤芯耐久显示和过滤器自动替换功能。

## Features / 功能

- **Scanner-gated inspection / 扫描仪门控：** When the player owns a water-quality scanner, hover over a water container to see its purity and composition. Without the scanner, the original tooltip remains unchanged.
- **Clear water readout / 水质详情：** Chinese displays `水质详情 (9960)  优质水`; other languages display `Water composition (9960)  High Quality Water`. The purity value is the game's integer purity value.
- **Original labels / 原版标签：** The label after the purity value comes from the game's own water-display logic, including standard water, high-quality water, ghost water, and other game-defined labels.
- **Container coverage / 容器支持：** Standard bottled water and the other recognized water containers are supported.
- **Composition amounts / 成分单位：** Composition values use `ml`. Entries that round to `0.00%` are hidden.
- **Automatic label refresh / 自动标签：** With a scanner, liquid transfer and pouring refresh the game's existing water-quality label logic. Without a scanner, AquaSense does not force a label refresh.
- **Filter durability / 滤芯耐久：** Hovering a water container that holds a filter shows its name and durability, for example `[滤水器]  耐久度:  140/400`. The line uses `#CCB987` and is shown even when the container is empty.
- **Nested filter search / 嵌套过滤器搜索：** When a water-container filter reaches 0 durability, the mod searches the player's top-level inventory and nested containers.
- **Replacement priority / 替换优先级：** Advanced filters first, then the vanilla filter, then other compatible mod filters.
- **Replacement notice / 替换提示：** A bilingual message reports the replaced filter and its replacement.
- **Exchange protection / 交换保护：** The swap is verified afterwards. A replacement is skipped if the old filter cannot fit at the replacement filter's position, or if the replacement cannot fit in the water container. If a swap does not complete, the detached filter is returned to its original position.
- **Low overhead / 低性能占用：** The stable replacement check runs at a low frequency instead of scanning the inventory every frame.

## Settings / 设置

In the `AquaSense` category of MelonPreferences:

| Setting / 配置项 | Default / 默认 | Effect / 说明 |
| --- | --- | --- |
| 滤芯耗尽后自动更换 | 开启 | 关闭后不再自动替换耗尽的过滤器 |
| 悬浮面板显示滤芯耐久 | 开启 | 关闭后悬浮面板不显示耐久行 |
| 倒水后自动更新水质标签 | 开启 | 关闭后倒水不再刷新标签 |

The water composition readout itself is not affected by these switches; it only depends on whether the player owns a water quality scanner.

水质成分显示不受这些开关影响，只取决于玩家是否拥有水质扫描仪。

## Requirements / 依赖

- Probably Stolen Demo 0.46D
- MelonLoader with the game's Unity/IL2CPP runtime

This release does not bundle MelonLoader or game files.

## Installation / 安装

1. Install MelonLoader for Probably Stolen Demo and launch the game once.
2. Close the game.
3. Copy `AquaSense.dll` into the game's `Mods` folder.
4. Launch the game and load a save.

Typical path:

```text
D:\app\steam\steamapps\common\Probably Stolen Demo\Mods\AquaSense.dll
```

Keep only one AquaSense DLL in the `Mods` folder.

## Compatibility / 兼容性

Built for game version `0.46D`, Unity `2021.3.45f2`, and the current IL2CPP assemblies. A game update that changes water, inventory, or IL2CPP method signatures may require a new build.

The release does not install generic inventory movement listeners on `GameInventory.UncheckedAccept`, `GameInventory.Expel`, or `GraphUtils.SwapItemsInPlace`. This keeps the release away from the high-risk hooks that caused startup instability during development.

## Logging / 日志

The release does not write normal runtime diagnostics to the MelonLoader log. It only logs a warning if a filter replacement pass or swap fails, which is useful when reporting an issue.

## Uninstall / 卸载

Close the game and delete `AquaSense.dll` from the `Mods` folder. AquaSense does not create save data.
