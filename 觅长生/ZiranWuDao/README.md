# 自然悟道 ZiranWuDao

原版里「思绪/感悟」只能手动感悟：花时间一次性获得该条思绪的全部悟道经验。

本 mod 让思绪在**时间流逝中自然结算**：每个游戏月自动获得该条思绪经验总量的 **0.5%**，单条最多结算 **12 个月**（合计 **6%**）。两个上限都可在配置里调整。

## 背景

原版的时间推进（闭关、休息、赶路、炼丹、剧情等 30 多处）最终都汇聚到 `Avatar.AddTime`，而感悟（思绪）只在玩家手动感悟时才转化为悟道经验。这意味着：

- 攒着一堆思绪不感悟，它们只会随时间过期消失，白白浪费；
- 玩家必须专门腾出闭关时间来「消化」思绪。

本 mod 在这条唯一的结算入口上挂钩：每次游戏月份推进时，为**每条仍存活**的思绪按比例自动发放悟道经验，无需手动感悟、无需额外操作。

### 结算规则

| 规则 | 说明 |
|---|---|
| 月度结算 | 每个游戏月发放该条思绪经验总量的 0.5% |
| 单条上限 | 最多结算 12 个月（0.5% × 12 = 6%），到顶后不再产出 |
| 独立结算 | 多条思绪各自结算，进度互不影响 |
| 一次性补足 | 一次跨多个月（如闭关 3 年）按实际跨越月数一次补足 |
| 中途过期截断 | 思绪在其存续期内结算——只存在 8 个月的思绪，即使闭关 36 个月也只结算 8 个月 |
| 手动感悟不变 | 手动感悟仍按原版获得全额经验，自然结算是额外收益 |

### 其它 mod 的大道

结算按思绪自身的类型字段进行，不做原版大道的白名单，因此「灵界」等 mod 通过前置框架追加的大道（冰/鬼/灵等）同样生效。

### 提示

有实际经验入账时逐条提示，**每个大道一条**（如「自然悟道：土之道+38」），按收益从高到低依次弹出。可在配置里关闭。

## 配置项（BepInEx 配置文件 `BepInEx/config/chiqingsan.mcs.ZiranWuDao.cfg`）

| 配置 | 默认值 | 说明 |
|---|---|---|
| `General.Enabled` | `true` | 总开关 |
| `General.ShowTip` | `true` | 结算后弹提示 |
| `Balance.PerMonthPercent` | `0.5` | 每月结算比例（%），按每条思绪经验总量的百分比 |
| `Balance.MaxSettleMonths` | `12` | 单条思绪最多自然结算的月数 |
| `Balance.MaxTotalPercent` | `6` | 单条思绪累计上限（占该条思绪总量的百分比） |

`MaxSettleMonths` 与 `MaxTotalPercent` 同时生效，哪个先到以哪个为准，改配置无需改代码。三个数值相互独立，不自动换算：

| 想要的收益 | 需要同时设置 |
|---|---|
| 0.5% × 12 个月 = 6%（默认） | `PerMonthPercent = 0.5`、`MaxSettleMonths = 12`、`MaxTotalPercent = 6` |
| 1% × 12 个月 = 12% | `PerMonthPercent = 1`、`MaxSettleMonths = 12`、`MaxTotalPercent = 12` |
| 1% × 6 个月 = 6%（更快到顶） | `PerMonthPercent = 1`、`MaxSettleMonths = 12`、`MaxTotalPercent = 6` |

只把 `PerMonthPercent` 调成 1 而不管上限，效果是**提前到顶**而非收益翻倍：1% × 6 个月就撞上 6% 的上限，第 7 个月起不再发放。想拿到 12% 必须把 `MaxTotalPercent` 一起放开。

中途改配置是安全的：结算按「累计月数」重算应得总额、只补差额，不会重复发放也不会倒退。

## 构建与代码结构

前置：.NET SDK（LangVersion 10）；本机装有游戏与工坊 BepInEx。路径在 `ZiranWuDao.csproj` 中配置。

```
dotnet build -c Release
```

产物：`bin\Release\ZiranWuDao.dll`。代码结构：

```
ZiranWuDao.csproj           工程文件（引用路径按本机配置）
Plugin.cs                   BepInEx 入口 + 配置项 + 注册 patch
ZiranWuDaoPatch.cs          Harmony Prefix patch Avatar.AddTime：结算主逻辑
ZiranWuDaoMath.cs           纯算法（时间推进/月数/上限/取整/提示文本），无游戏依赖
tools/MathCheck/            算法离线验证程序
```

算法可离线验证（33 项断言，覆盖跨月补足、过期截断、双上限、配置组合、取整、提示文案、时间推进顺序、数据异常防护）：

```
dotnet run --project tools\MathCheck -c Release
```

## 性能

- 无常驻开销：不挂 `Update`、不轮询、不扫 UI，只在玩家推进时间时执行。
- 未跨月即刻返回：绝大多数赶路、采集走这条路径。
- 已达上限的思绪直接跳过，不解析日期。
- 遍历成本为 O(思绪数)，每条只做整数与日期运算；同一条思绪本次的多个合并为一次 API 调用。
