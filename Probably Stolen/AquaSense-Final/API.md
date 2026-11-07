# 成分数据接口（只读）

从 AquaSense 1.10.2 起，模组会把"水质详情"里的成分行通过 `AppDomain` 槽位暴露给其他模组。消费方不需要引用 `AquaSense.dll`，AquaSense 自身的悬浮面板渲染也不受影响。

## 槽位

| 槽位 | 类型 | 说明 |
| --- | --- | --- |
| `aquasense.composition.v1` | `Func<GameItem, IReadOnlyList<(string Name, string Amount, string Percent, float Ratio, int ColorRgb)>>` | 读取指定物品的成分行；不可用时返回 `null` |
| `aquasense.composition.version.v1` | `int` | 接口版本，当前为 `1` |

返回 `null` 的条件与 AquaSense 显示成分的条件一致：

- 物品不是装有液体的水容器；
- AquaSense 设置里的“显示水质详情”被关闭（1.12.0 起，关闭后面板不显示成分，接口也同步返回 `null`）；
- 玩家没有水质扫描仪，且未开启 AquaSense 设置里的“无需水质扫描仪查看液体成分”。

返回空列表表示容器没有可识别的液体成分。

## 行字段

- `Name`：液体显示名；
- `Amount`：显示用体积文本（如 `0.75 ml`、`3 uL`，与面板一致）；
- `Percent`：显示用百分比文本（如 `75.00%`；四舍五入为 0.00% 的微量成分标为 `‹0.01%`）；
- `Ratio`：`0`～`1` 的占比，可直接用于画条；
- `ColorRgb`：`0xRRGGBB` 建议条色，与 AquaSense 面板配色一致。

数据语义：

- 与进度条模式的行一致：微量成分始终返回（即使开着"隐藏四舍五入后为 0.00% 的成分"）；
- 与「用彩色进度条显示成分占比」开关无关，始终返回数据行；
- 行顺序与面板一致：默认按占比从高到低（可在设置中关闭排序，关闭后为游戏液体定义顺序）；
- 只读；AquaSense 不会调用消费方，也不持有消费方引用。

## 生命周期

模组启用时写入槽位，禁用时清空（`null`）。消费方每次渲染时读取（`GetData` + 类型转换，开销可忽略）或缓存委托并容忍其为 `null` 均可。

## 示例

```csharp
using System;
using System.Collections.Generic;

// 直接读取槽位即可，无编译期依赖。元组元素名在运行时被擦除，
// 消费方用自己习惯的元素名声明同一个 Func 即可完成转换。
private static Func<GameItem, IReadOnlyList<(string Name, string Amount, string Percent, float Ratio, int ColorRgb)>>?
    CompositionProvider()
{
    if (AppDomain.CurrentDomain.GetData("aquasense.composition.version.v1") is not int version || version != 1)
        return null;

    return AppDomain.CurrentDomain.GetData("aquasense.composition.v1")
        as Func<GameItem, IReadOnlyList<(string Name, string Amount, string Percent, float Ratio, int ColorRgb)>>;
}

// 示例一：追加为纯文本行。
private static void AppendCompositionRows(RichTextBuilder builder, GameItem item)
{
    var rows = CompositionProvider()?.Invoke(item);
    if (rows == null)
        return;

    foreach (var row in rows)
        builder.AddLine($"{row.Name}: {row.Amount} ({row.Percent})");
}

// 示例二：用自己的渲染器画条（伪代码）。Ratio 为 0～1，ColorRgb 为 0xRRGGBB。
// foreach (var row in rows)
// {
//     var colour = new Color(((row.ColorRgb >> 16) & 255) / 255f,
//                            ((row.ColorRgb >> 8) & 255) / 255f,
//                            (row.ColorRgb & 255) / 255f);
//     DrawBar(row.Ratio, colour);
// }
```

## 与《物品详情美化》（SectionedTooltips）配合

对方通过 `AppDomain` 槽位 `item_details.tooltip_sections.v1.read` 获取行到区块的映射；若希望自己的面板把成分归入它的 `liquid` 区块，可另外注册该回调（AquaSense 1.10.2 未默认注册，以免改变双装时的分组行为）。绘制进度条时，把上表的 `Ratio`/`ColorRgb` 转成对方原生 `Meter`/`DrawBar` 所需的条描述即可。
