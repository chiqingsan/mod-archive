# 储水罐自绘贴图（水桶造型）— 交付说明

生成脚本：`gen_art.py`（PIL，唯一来源，改参数后重跑即可再生成全部文件）

## 文件清单

| 文件 | 用途 |
| --- | --- |
| `watertank_body.png` | 桶身主图（静态，游戏不 tint） |
| `watertank_liquid_10/25/50/75/100.png` | 液体层 5 档（纯白骨架，游戏按液体颜色 tint） |
| `watertank_hl_10/25/50/75/100.png` | 高光层 5 档（水面闪光/气泡，不 tint） |
| `preview_4x.png` | 3 种液体色 × 6 档水位预览（4×） |
| `preview_1x.png` | 实际游戏尺寸预览（1×） |
| `preview_slots_2x.png` | 放在游戏槽位底色上的实际尺寸预览 |
| `compare_original_vs_new_4x.png` | 原版桶装水 xl 帧 vs 自制贴图对比 |

## 规格

- 尺寸 64×64（对应游戏 4×4 格，gridSize=16），PPU 100，pivot 0.5，Point/Clamp 采样。
- 主图不参与染色；液体层是白色骨架，游戏用 `Image.color = HexToColor(GetLiquidContainerColor(item))` 乘算上色，一套资源即可显示 blue/red/dark 三色水。
- 水位档位与游戏一致：10/25/50/75/100（0% 时不画液体层）。
- 液体层相对槽位向下填充、水面留 1px 白浪线；高光层叠加闪点与气泡。

## 接入 WaterTankLevel（后续改代码时）

ModLoader 会把 mod 目录 `Textures/Items/*.png` 自动加载为 Sprite（key = 文件名去扩展名）。
把本目录 PNG 放进 mod 的 `Textures/Items/` 后：

- 主图：`LoadedMod.Sprites["watertank_body"]`
- 液体层：`LoadedMod.Sprites[$"watertank_liquid_{档位}"]`（仍走游戏 tint）
- 高光层：`LoadedMod.Sprites[$"watertank_hl_{档位}"]`

替换点：`WaterTankLevel.cs` 的 `TankAppearance.LiquidLayers`（返回上面两层）与 `TankSpritePatch`（返回 body）。
