# Probably Stolen Demo Mod 开发资料库

这套文档用于后续制作 MelonLoader / IL2CPP Mod。内容基于本机 **Probably Stolen Demo** 当前安装版本的 `Assembly-CSharp.dll`、MelonLoader 程序集和已经运行过的 Mod 整理而成。

> 重要：这些不是官方 API。游戏更新后，类型、字段、方法签名和行为都可能变化。先用程序集检查方法确认签名，再在游戏内验证行为。

## 文档导航

| 文档 | 内容 | 主要对应 Mod | 风险等级 |
| --- | --- | --- | --- |
| [01-环境与程序集](01-environment-and-assemblies.md) | 游戏路径、引用 DLL、目标框架、IL2CPP 注意事项 | 全部 | 低 |
| [02-MelonLoader 生命周期与配置](02-melonloader-lifecycle-and-config.md) | Mod 入口、日志、MelonPreferences、XIAOWO 热键配置 | 全部 | 低 |
| [03-商店与 UI API](03-store-and-ui-api.md) | 商店单例、按钮、顾客流程、平板、日历、原版长按进度圈、消息提示控件、`RichTextBuilder` 富文本 | QuickCounter | 中 |
| [04-物品库存与交易](04-items-inventory-and-trading.md) | `GameItem`、物品标识符查找、背包、库存移动、买卖、协商 UI | QuickCounter / BlackMarketRescue | 高 |
| [05-顾客电话事件与存档](05-clients-phone-events-and-save.md) | 顾客生成、电话、操作、事件、存档边界 | QuickCounter | 高 |
| [06-开发配方与验证](06-recipes-and-verification.md) | 常用代码模式、程序集检索（Cecil / ilspycmd）、发布前检查清单 | 全部 | 低 |
| [07-AquaSense 水系统与过滤器](07-aquasense-water-and-filters.md) | 水质扫描、液体转移、嵌套容器遍历、过滤器识别与交换、滤芯耐久显示 | AquaSense | 高 |
| [api-snapshot-2026-09-22.txt](api-snapshot-2026-09-22.txt) | 从当前 `Assembly-CSharp.dll` 导出的重点类型签名快照 | 全部 | 参考 |

风险等级指直接按文档调用对应接口的危险程度：低 = 只读/日志/配置；中 = 涉及 UI 与场景对象；高 = 涉及库存、金钱、事件或存档，改动前必须备份存档并实机验证。

## 当前环境基线

```text
Game:       Probably Stolen Demo 0.46D
Company:    Questing Goose Studio
Namespace:  Il2Cpp
Unity:      2021.3.45f2
Target:     net6.0 / x64
Loader:     MelonLoader 0.7.3 Open-Beta / net6
Snapshot:   2026-09-22
```

本次签名快照对应的程序集：

```text
Assembly: Assembly-CSharp 0.0.0.0
SHA-256: C1A0CA0FD8B5C250D3AED419301662C0247C2763B9FB25AEDA1B90CE5C20969D
```

## 推荐工作流

1. 复制一个最小 Mod 项目，先只验证 `OnInitializeMelon` 和日志。
2. 从 [api-snapshot](api-snapshot-2026-09-22.txt) 找到候选类型，再用 `06` 中的 Cecil 脚本确认当前程序集。
3. 优先调用游戏已有 UI 控件的 `OnClick()`、`OpenUI()`、`CloseUI()` 等入口；不要一开始就直接改底层字段。
4. 对长按动作同步 `ShouldRequireHold()`、`HoldDuration` 和 `CustomUIManager.ShowHoldRing()`。
5. 游戏内验证后再加入配置、部署和发布包。

## 已确认的安全边界

- `Assembly-CSharp.dll` 多数方法是 IL2CPP 包装器；签名可靠，但包装器本身不能说明完整业务逻辑。
- 单例属性常见为 `Type.Instance`，也有少数类型使用小写 `instance`，使用前要以快照为准。
- `FindObjectOfType<T>()` 适合查找场景按钮，但对象可能不存在、未激活或随场景销毁。
- 修改存档、金钱、库存和顾客状态前应先备份存档，并避免在每帧直接写入。

## 维护约定

- 游戏更新后，先重新导出 `api-snapshot-<日期>.txt`，再按 01 → 07 顺序核对签名与行为，并同步更新本页的环境基线。
- 每篇文档的结论按 [06](06-recipes-and-verification.md) 中的四级口径标注：已确认签名 / 已实机验证 / 待验证行为 / 高风险内部接口。
- 快照文件按日期命名；建立新快照后，旧快照移入归档，不要覆盖历史文件。
- 单个 Mod 的功能实现细节写在该 Mod 目录下（如 `QuickCounter/README.md`），本资料库只放跨 Mod 通用的游戏 API 与验证结论。
