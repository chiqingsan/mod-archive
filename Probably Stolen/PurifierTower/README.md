# 海德拉净水箱（PurifierTower）mod

Probably Stolen 0.46D 的净水塔原型：一个可放在店里的大型机器，内部包含「净水器区 / 电池区 /
待处理水 / 已处理水 / 输出区」，目标是把「接水 → 紫外线 → 用玩家自己的净水器净化 → 达标 →
打印瓶子并灌装」串成一条自动化流程。

**当前版本 v0.5.0**：修复 v0.4.0 验收问题——
- 固定水箱不再在建塔工厂里创建（`DirectoryMaster.Item` 会 Destroy 工厂产物的所有子物品），
  改为塔创建完成 / 窗口打开时运行时补建，并立即 `GeneralHelper.SetItemOwned`，
  解决「水倒不进待处理水箱、拖空瓶取不到水」。
- 打印出的满水瓶同样设置所有权与 uniqueId，并改成逐瓶协程（每帧一瓶），
  避免一次插入多瓶触发整树 `ValidateShapeState` 导致点击卡顿。
- 运行页/电池页改为「一行一个全宽面板」，统一 336 宽，修复布局边框不对齐。
- 开窗/补水/插 UV 增加耗时探针（`-pt-debug` 下打印 `[trace]`），用于继续定位卡顿。
- 新增 `-pt-debug` 冒烟自检：发放塔后自动校验水箱存在/所有权/容量/加水量/灌装/输出区插入。

净化 / 自动接水逻辑尚未实现（M1+）。

## 构建

```powershell
cd "D:\code\mod\Probably Stolen\PurifierTower"
dotnet build -c Release
```

产物：`bin\Release\netstandard2.1\PurifierTower.dll`。

## 部署（需用户当次明确许可）

Mods 目录三件套：`PurifierTower.dll` + `manifest.xml` + `preview.png`，
目标 `D:\app\steam\steamapps\common\Probably Stolen\Mods\PurifierTower\`。
部署后需重启游戏（DLL 变化会触发 ModLoader 的信任检查）；旧塔窗口结构参与存档编码，
升级后建议用物品管理器重新生成一台新塔再测试。

**测试开关**：启动游戏时带命令行参数 `-pt-debug`（Steam 库 → Probably Stolen → 属性 →
通用 → 启动选项 填 `-pt-debug`），读档后会自动：
发放一台海德拉净水箱到主库存；把六种测试道具（200L 水容器 + 五种水瓶）放进库存；
安装工厂/放置链路与耗时追踪补丁，并跑一次水箱/打印冒烟自检（日志前缀 `[smoke]`）。
发布版不带该参数，此时只注册物品，不发任何测试道具。
**注意**：不要用「检测 debug.txt 文件是否存在」来做开关——ModLoader 会判定本 mod 请求了
未声明的 files（读取/写入任意文件）权限并拒绝加载。

## 获取方式

- 无 `-pt-debug`（发布形态）：用物品管理器（Steam 工坊 mod「Probably Stolen Item Manager」）
  搜索 **海德拉净水箱** 即可找到并生成；「200L 水容器」与瓶子同理搜「1L 水瓶」…「60L 水桶」。
  物品名由 `LocalizationInject.cs` 在启动时注入游戏自带 Unity Localization 的「Item」表
  （键约定 `item_<identifier>_name`，中英双语），这也是物品管理器解析 mod 物品名的唯一可靠来源；
  注入失败时仍可按标识符（如 `purifier_tower`）搜索。
- 有 `-pt-debug`（测试形态）：读档时自动发放一台到主库存，并把六种道具放进库存。

## 物品

| 标识符 | 说明 | 容量 | 底价 |
| --- | --- | --- | --- |
| `purifier_tower` | 海德拉净水箱 | — | 1250 |
| `purifier_tower_container_200l` | 200L 水容器（自备） | 200000ml | 500 |
| `purifier_tower_bottle_1l` | 1L 水瓶 | 1000ml | 4 |
| `purifier_tower_bottle_3l` | 3L 水瓶 | 3000ml | 8 |
| `purifier_tower_bottle_6l` | 6L 水瓶 | 6000ml | 15 |
| `purifier_tower_bottle_20l` | 20L 水桶 | 20000ml | 45 |
| `purifier_tower_bottle_60l` | 60L 水桶 | 60000ml | 120 |

贴图全部复用游戏原版素材（塔/水箱/自备容器 = `items_tool2/water_tank`，瓶 = `water_container3` 的
mini/small/normal/large 与 `water_container4` 的 xl），占格由贴图 alpha 推导，不额外覆盖。

## 玩法（v0.5.0）

- **待处理水（固定 100L）**：把装脏水的瓶子/容器拖到内置水箱上，水会倒进去；
  把 200L 自备容器放进右侧槽位也可以作为额外待处理容量（放了自备容器 → 打印停用）。
- **已处理水（固定 600L）**：净化后的水在这里；拖一个空瓶到水箱上 = 从水箱灌满该瓶。
- **打印**：运行页选容量（1/3/6/20/60L），点「打印 1 瓶」或「打满输出」，
  从已处理水箱取水灌成「满水瓶」放进输出区；耗电 `round(3+0.004×ml)`
  （1L 7 / 3L 15 / 6L 27 / 20L 83 / 60L 243），从电池区合并电量扣；
  水不足/电不足/输出区满/已放自备容器时不打印，并在面板上提示。
- **净水器区 21×18**：放入限制与原版「机器区」`machine_bay` 一致；双击塔内净水器可打开它自己的窗口。
- **电池页**：电池区 4×4、合并电量条、紫外线灯槽（注水前自动用 1 次）、打印耗电参考、能量列阵。

## 代码导览

- `PurifierTowerMod.cs`：IMod 入口、Harmony 注册兜底、塔创建后补建水箱的 Postfix、测试发放、冒烟自检。
- `TowerFactory.cs`：物品与窗口工厂（三页签布局、槽位过滤、固定水箱/自备容器/打印/运行时补建）。
- `ProbePatches.cs`：仅 `-pt-debug` 时安装的追踪/耗时补丁（定位 UV 插入与开窗卡顿）。
- `DebugTools.cs`：`-pt-debug` 启动参数开关（不读取任何文件，避免触发 files 权限）。
- `LocalizationInject.cs`：把 7 个物品名注入游戏「Item」本地化表（幂等、不读文件），供游戏与物品管理器显示/搜索。

## 已知限制（v0.5.0）

- 净化 / 自动接水 / 达标判定尚未实现（M1）：目前水需要玩家手动倒入待处理水箱，
  已处理水箱的水也需要玩家用净水器自己处理后再倒回来。
- 打印只消耗电池区电量，不校验水龙头开关（接水逻辑还没接上）。
- 旧档里的塔（v0.2.5 之前创建）没有新窗口结构，建议用物品管理器重新生成。
- 窗口结构参与存档编码：以后增删页签/面板需要做迁移，否则旧档内嵌物品索引会错位。
- 本塔不使用 `MachineHelper.GetBatterySlot/GetSlot/GetModules` 的槽位索引约定（电池在页签里），
  后续代码不要在这些 API 上读塔的槽位。
- 紫外线灯插入卡顿：已加 `[trace]` 耗时探针（`UncheckedAccept`/`OverlayHandler`/`Validate`），
  需要一次 `-pt-debug` 日志确认耗时点后再针对性优化。
- 控制台/物品管理器刷出 mod 物品无预览图：待复现（控制台图标走 `ResolveIcon` 的
  `GameItemElement.background.image.sprite`，与物品渲染时机有关）。

## 验证清单（实机）

1. 日志出现 `registered item factories`、`load-late: towerRegistered=True`，且无 `EnsureFixedTanks` 报错。
2. `-pt-debug` 读档后出现 `[smoke][ok]` 系列（pending/processed tank exists、owned、容量、加 6000ml、灌瓶、输出区插入）。
3. 物品管理器搜索「海德拉净水箱」「200L 水容器」能搜到并生成（无 `-pt-debug` 时的主要获取途径）。
4. 双击塔打开三页签窗口：四个面板左右边框对齐、切页窗口尺寸不跳变、开窗日志 `[trace] openCallback` 毫秒数。
5. 净水器页可拖入原版 `water_purifier`（21×18）；双击塔内净水器能打开它自己的窗口。
6. 电池页可放电池/能量列阵；输出区网格为 7 列 × 4 行横向。
7. 把装水的瓶子拖到待处理水箱上 → 水量增加；拖空瓶到已处理水箱上 → 瓶子被灌满。
8. 已处理水箱有水时点「打印 1 瓶」→ 输出区出现满水瓶、电量下降、水面下降；反复点不卡。
9. 放一个 200L 自备容器到任一侧槽位 → 打印提示停用。
10. 插紫外线灯观察 `[trace] … took N ms` 日志，回传日志以便定位卡顿。
11. 睡一晚出现 `night-end-late tick` 日志（仅 `-pt-debug`）。
