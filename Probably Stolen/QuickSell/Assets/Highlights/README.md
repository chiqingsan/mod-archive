# 高亮图片资源

模组：帮我上货! (更好的交易体验)，英文名 QuickSell。

用户于 2026-10-30 选定：`demand-corners.png`、`stolen-tag-v1.png`、`contraband-warning-v2.png`。1.3.4 将这三张原始图片内嵌 DLL，启用时一次性合成共享图集；`../../Hints.cs` 中的 `ItemHintGraphic` 使用图集绘制。源码与安装包完成不代表已经替换运行中的游戏 DLL，部署状态见当次交付说明。

- `demand-corners.png`：顾客需求四角框，透明背景。
- `stolen-tag-v1.png`：用户选定的赃物标签；`contraband-warning-v1.png` 为未采用第一版。
- `contraband-warning-v2.png`：用户选定的违禁物警告；`stolen-tag-v2.png` 为未采用重绘版。
- `style-reference.png`：用户选定的整体高亮样式参考。
- `badge-reference.png`：用户指定的两个角标局部参考。

生成方式：内置 imagegen，参考图重绘。提示要求：分别绘制单独的赃物标签／违禁物警告，贴合参考轮廓与比例，粗深色描边、轻微像素圆角；去除网格、物品和文字，使用真实透明背景。原始 PNG 保留生成尺寸和像素，不修改；运行时仅通过 UV 选择图案区域。
