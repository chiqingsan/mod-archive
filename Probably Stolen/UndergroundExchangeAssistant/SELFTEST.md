# 1.1.4 本轮自测

Release 构建 0 警告、0 错误，全部控制台测试通过。新增跨库存联合取材并搬运、顾客容器和垃圾桶排除、共享库存去重、未解锁和无存档隐藏入口的测试。封面与纸张导出图已目视检查。

悬停静态依据：当前游戏 ItemMouseHandler → HighlightManager → GameGridInventory.HighlightSlot → RenderHandler.SetHighlightColor(White, Grid)。模组复用原版素材平铺并关闭 Button ColorTint。

1.1.4 本轮未部署；原生悬停、窗口视觉与跨库存现场操作仍待游戏内验证。

以下保留此前测试记录。

# 1.1.1 自测记录

## 通过

- 正式版 Assembly-CSharp 与 Unity UI 引用的 Release 构建：0 警告、0 错误。
- 结算模拟和材料计划：每晚一次、单对象不重复匹配、交易优先级、已有完整/部分材料、材料不足、容器子项随父项移除、促销与常驻争用。
- 使用替身游戏接口执行实际 ExchangeTransfer 代码：六个滤芯投入三个、重复点击不可用、拆堆中途失败回滚、普通物品中途失败回滚、空间不足时不搬运、低耐久滤芯优先、损坏模组优先、机器内模组排除、暗格非空确认判定、取消不搬运、未解锁和清洁服务关闭时禁用、投放后匹配核对。
- 纸张像素尺寸、透明边缘，以及四种垃圾桶缩放尺寸下纸张保持在桶身正面范围。
- PaperPointerBlocker 对开始拖动、拖动、结束拖动事件调用 Use。
- 只读检查当前安装的 level1 资源：原版面板 666×930，七条常驻行与七个促销标识；常驻行宽度至少 490、高度至少 50。

## 尚未验证

没有将 1.1.1 写入 Mods。上述替身/资源检查不代表游戏内运行；纸张的最终显示、`TreeNodeRender` 点击隔离、原版窗口行内按钮位置与字体、完整跨夜结算需要部署后进行游戏内验收。

## 复现命令

```powershell
dotnet run --project '../UndergroundExchangeAssistant.Tests/UndergroundExchangeAssistant.Tests.csproj'
python '../UndergroundExchangeAssistant.Tests/verify_native_layout.py'
./Build-Package.ps1
```
