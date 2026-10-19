# Black Market Rescue

Probably Stolen 的 MelonLoader 小 Mod。

## 功能

- 在黑市交换处理后检查垃圾桶。
- 每天开始时再做一次兜底检查。
- 如果垃圾桶里还有物品，会尝试搬到柜台区域：
  1. 优先放到谈判柜台库存 `backInvinvElementCounter`
  2. 其次放到前台柜台/陈列库存 `frontInvinvElement`
- 不会把物品搬到玩家顶层物品栏 `PlayerStore.gridInv`。
- 如果柜台没有空间，会把物品留在/放回垃圾桶，并在日志里提示。

## 安装

把 `BlackMarketRescue.dll` 放入游戏目录的 `Mods` 文件夹。

当前默认游戏路径：

```text
D:\app\steam\steamapps\common\Probably Stolen Demo
```

## 验证

已完成：

```text
dotnet build BlackMarketRescue\BlackMarketRescue.csproj -c Release
```

构建结果：0 warning, 0 error。

仍建议在游戏内用一个测试存档验证黑市交换后和隔天开局时的实际搬运位置。
