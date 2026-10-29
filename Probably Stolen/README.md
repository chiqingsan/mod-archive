# Probably Stolen Demo Mod 工作区

本仓库存放针对 **Probably Stolen Demo**（Questing Goose Studio，IL2CPP + MelonLoader）的 Mod 源码、发布包和开发文档。

> 游戏不是官方 Mod 平台，所有 API 均来自反编译与实机验证，游戏更新后签名和行为都可能失效。任何改动前先读 [`docs/`](docs/README.md)。

## 快速入口

- **开发文档库**：[docs/README.md](docs/README.md) —— 环境、生命周期、UI/物品/交易 API、验证配方，以及 API 签名快照。
- **签名快照**：[docs/api-snapshot-2026-09-22.txt](docs/api-snapshot-2026-09-22.txt)（对应游戏 0.46D / `Assembly-CSharp` SHA-256 `C1A0CA0F…`）。
- **发布前检查清单**：[docs/06-recipes-and-verification.md](docs/06-recipes-and-verification.md)。

## 目录结构

正式版使用游戏内置 ModLoader。以下为本地正式版源码项目：

- [更好的水商操作体验（AquaSense）](AquaSense-Final/README.md)
- [回来吧，水商！](WaterMerchant-Demo/README.md)
- [伪人数量保持](AugPresenceGuard-Final/README.md)
- [CustomerCreditBoost](CustomerCreditBoost-Final/README.md)
- [QuickCounter](QuickCounter-Final/README.md)
- [QuickSell](QuickSell/README.md)
- [更好的库存排序](SmartInventoryArrange-Final/README.md)

各项目的清单与说明记载对应版本；本地源码可能包含尚未部署或尚未完成实机验证的更新。

| 目录 | 说明 |
| --- | --- |
| [`docs/`](docs/README.md) | 跨 Mod 通用的开发资料库（01~07 主文档 + API 快照），是本仓库的唯一权威文档源。 |
| [`QuickCounter/`](QuickCounter/README.md) | **QuickCounter** 源码与发布包，可配置柜台快捷键 Mod（当前 v1.2.0）。 |
| [`BlackMarketRescue/`](BlackMarketRescue/README.md) | **BlackMarketRescue** 源码，黑市交换后自动把垃圾桶物品搬回柜台。 |
| [`AquaSense/`](AquaSense/AquaSense.cs) | **AquaSense** 源码工程（当前 v1.1.2，发布包见 [`release/`](release/)）。 |
| [`release/`](release/) | 发布包归档：AquaSense 1.0.0 / 1.0.1 / 1.0.2 / 1.1.0、BlackMarketRescue 1.0.0 / 1.0.1（含 zip）。 |
| [`decompiled/`](decompiled/) | 其他 Mod 的反编译参考工程：AutoBatterySwap、AutoStorage、FutureTech、WaterTower、XIAOWOTradePerks。 |
| [`tools/`](tools/) | 辅助工具（`ilspycmd` 反编译 CLI）。 |

## 环境基线

```text
Game:       Probably Stolen Demo 0.46D
Namespace:  Il2Cpp
Unity:      2021.3.45f2
Loader:     MelonLoader 0.7.3 Open-Beta / net6
Target:     net6.0 / x64
Snapshot:   2026-09-22
游戏路径:   D:\app\steam\steamapps\common\Probably Stolen Demo
```

## 新增或更新 Mod 的推荐流程

1. 复制一个最小 Mod 项目，先只验证 `OnInitializeMelon()` 和日志输出。
2. 用 [docs/06](docs/06-recipes-and-verification.md) 中的 Cecil 脚本确认当前程序集签名。
3. 优先调用游戏已有 UI 控件的 `OnClick()` / `OpenUI()` / `CloseUI()`，不要直接改底层字段。
4. 涉及长按动作时同步 `ShouldRequireHold()`、`HoldDuration` 与 `CustomUIManager.ShowHoldRing()`。
5. 用独立存档实机验证后，再补配置、部署和发布包，并按 `docs/06` 的检查清单走完发布流程。
