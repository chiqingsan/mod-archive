# 读档报错审查与 1.1.5 修复

## 日志证据与限制

检查本机 Player.log（2026-11-02 18:43:30）。ModLoader 的组合指纹为 8a3540766c55f86fdf6c34fc06501adbe1edc039d9b9fb0febf0b17ce6e99bd0，按 ModFingerprint.Compute 算法复算后与当前安装的 1.1.4 一致。它不是 DLL 单独的 SHA-256。

本次日志出现约 2035 次 OverlayHandler.HideTooltip 空引用、1701 次 GameItemElement.GetTooltipBasic 空引用，还有化学 tick 与 WaterMerchant 状态读取异常。对应异常栈没有交易助手调用帧。用户确认发生在读档阶段，不能据此断定交易助手库存扫描是这些异常的根因，也不能断定与交易助手无关。原生 GetTooltipWindow 返回未初始化窗口时，不应该创建交易助手界面。

## 本轮修复

1. UI 创建增加实际就绪门槛：原生渲染器、UISettings、CustomUIManager、OverlayHandler 和原生 tooltipWindow 均已创建后才允许初始化。不是对 Update 的异常进行吞错。
2. 原来的直接 AddComponent<TreeNodeRender> 不初始化该类型私有的 rect/image/mask 属性。改为原生 InstantiateAsRoot 工厂，保留逻辑根节点隔离垃圾桶射线。
3. 库存扫描不再给每个物品调用原生耐久 getter。该 getter 会初始化缺失标签；现在只对滤芯使用字典 TryGetValue 只读读取，跳过尚未反序列化完成的物品。
4. 实际投放原先没有使用 MaterialPlan 的预留格位，而是再次自动找位置，可能自动堆叠或投入内层容器。现在重新校验预留格位，再通过原生 SlotMarker.TryAcceptOnce 投放。
5. 原生 TryAcceptOnce 在接收失败前可能已经拆堆/移出。记录 slot.item 中实际脱离源库存的物品，并走回滚；回滚只记录真正搬入的顶层物品，保留暗格内容关系。
6. 模拟排序改为与原版相同的 List.Sort 比较方式。

## 验证

Release 构建 0 警告、0 错误。全部替身回归测试通过，新增覆盖未完成反序列化、扫描不初始化标签、接收失败时物品守恒与暗格内容关系、预留格位变化后拒绝投放。测试替身的 SlotMarker.item 语义已按当前反编译的原生实现修正。

1.1.5 尚未部署，尚未进行真实读档复测。这些代码缺陷已修复，但现有证据不足以宣布日志中的读档刷屏根因已经解决。不得以构建或替身测试通过代替原生验收。
