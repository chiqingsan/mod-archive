# AquaSense 回归验证

运行：

```powershell
dotnet run --project "Probably Stolen/AquaSense.Tests/AquaSense.Tests.csproj" -c Release
dotnet build "Probably Stolen/AquaSense/AquaSense.csproj" -c Release -p:SkipDeploy=true
```

测试工程直接编译 `../AquaSense/AquaSense.cs`，不复制业务实现，不依赖测试框架或 NuGet 测试包。`GameStubs.cs` 只在该控制台测试工程中使用；不会随游戏 Mod 部署。失败以异常和非零退出码报告。

## 2026-10-22 / 1.1.2 的修改依据

1. 1.1.1 未发现耗尽滤芯也会收集、排序所有候选。改为按需建立三个稳定优先级桶，每个候选只算一次优先级；测试无水罐和健康水罐各带 10000 个备用滤芯时，没有备用滤芯耐久扫描。
2. 原先锁定网格可能通过预检，后续失败后阻塞其他候选。统一检查插入/移除锁，预检不可用时继续寻找。
3. 原先读取 parentInventory 失败和 detached 共用 null，部分交换后可能两个物品都回到来源库存。区分读取成功与结果为空，保存原形状并按交换阶段恢复；恢复失败保留引用，先重试恢复再进行其他交换。
4. 原先按 ID 缓存的是实例标签状态，且缓存周期与维护周期均为 2 秒。删除该缓存，保留已知 ID/实时标签/原生能力三层判定；移除额外 FILTER_CONTAINER_TAG 门槛。
5. 原先只记录最后一个 Tooltip，A/B/A 可重复。改为同帧记录 builder/item 与实际追加 slice 的位置/指针，Clear 后允许重建。
6. 原先 int 成分总量可能溢出。总量改为 long；单项仍保持游戏标签类型。
7. 原先每轮工作集中在一个 OnUpdate，悬停也可能同步扫描。扫描和检查改为主线程迭代器，约 2ms 预算分段；Tooltip 仅请求刷新并查询结果。单次原生调用不能被预算强制中断。
8. 原先原生位置约定不明确，自写包围盒会漏掉旋转、异形和空洞。以下静态证据表明原生方法使用完整 modifiedShape，因此改用原生形状校验/放置 API。
9. 分帧引入的状态时效要求：交换前重读父库存、滤芯/耐久和拥有的根物品；跨帧遍历动态库存列表使用索引而非持有 foreach 枚举器；LoadGame 或 PlayerStore 变化取消旧工作。

## 原生证据范围

目标：本机 Probably Stolen Demo 0.46D `GameAssembly.dll`。

SHA256：`A2CAA0CECBF197CCD6B12DDEC7BB9E6FA0BC2CD2A5C52ABEF627AA919E2B9BFD`。

通过已有 Cpp2IL dummy 元数据地址及 Capstone 静态反汇编核对：

| 方法 | RVA | 依据 |
| --- | --- | --- |
| `GraphUtils.SwapItemsInPlace` | `0x7C8E40` | 读取两个 GameItem 的 `+0x198` 父库存与 `+0x1A8` modifiedShape；经过 `EmporiumEntry.swapBufferElement`，调用 PlaceAtShape 完成双向放置及失败回退 |
| `GraphUtils.PlaceAtShape` | `0x7C8D40` | 构造带 item/inventory/itemGridShape 的 SlotMarker，调用 TryAcceptOnce |
| `GameGridInventory.CheckShapePlacement` | `0x7B99D0` | 接收 shape 和 ignoredItems，经实际库存形状、物品碰撞和状态规则验证 |
| `GameSlotInventory.MayHaveValidInventorySlot` | `0x7C5E50` | 类别/父关系预检，不以槽已被交换对象占用作为直接拒绝条件 |
| `GameSlotInventory.CheckInventorySlotStillValid` | `0x7C54A0` | 检查当前占用，供实际恢复时调用；不能拿它直接预检仍被旧滤芯占据的交换目标 |
| `GameItem.FindAllChildItems` | `0x7DCCA0` | 转入递归图查找；不应在每个后代上重复调用 |

`GridShapeBuilder.SetTransform`、`Build`、`GraphUtils.PlaceAtShape` 等签名另经当前游戏的 IL2CPP 互操作程序集编译核对。以上为本地静态证据，不是 REA 运行输出，不等于实机执行或性能基准。

## 覆盖范围

目前 68 组场景通过，包括：

- 0/1/100/1000/10000 个备用滤芯；健康水罐、无备用滤芯、空槽、多罐单备用。
- 插入锁、移除锁、候选回退、类型拒收、物品状态拒绝、拖拽、原生缓冲槽被占用。
- 耐久 int.MinValue/-1/0/1/int.MaxValue；耐久读取异常；UV 灯与工业布料排除。
- 1/15/16/17/100/1000 层嵌套；父链和库存图成环；根/仓储/窗口/滚动库存重复入口。
- 旋转 0/1/2/3、翻转、非零坐标、异形相邻物品、真实格子重叠、库存空洞、越界。
- 六种部分交换状态；已完成但原生返回 false；交换抛异常；原位被占用；恢复失败后保留引用并重试。
- 相同 ID 的实例标签不同/动态修改；原生能力兜底；旧容器不额外要求 FILTER_CONTAINER_TAG。
- Tooltip A/B/A、Clear 复用、关闭耐久显示、未拥有扫描仪、大容量成分合计超过 int.MaxValue。
- Tooltip 不同步扫描库存；关闭自动更换后仍检测扫描仪；关闭功能中断未执行的交换。
- 扫描期间物品售出、库存列表增删、同 PlayerStore 读档、换 PlayerStore、分帧控制权归还。

## 仍需实机验收

桩模型验证调用方向、业务分支、状态恢复顺序和工作量，不验证 IL2CPP 的真实格子实现、图接口转换、Harmony 原生启动、存档语义或毫秒耗时。真实故障也可能暂时阻止恢复，此时保留引用和重试比宣称必然立即恢复更准确。

同一份存档、相同库存布局下，分别检查普通/满格/旋转/嵌套滤芯交换、酿酒瓶悬停、持续倒水、读档与功能开关；记录维护和 Tooltip 的耗时。不能仅凭本测试通过就声称游戏卡顿已经消失。
