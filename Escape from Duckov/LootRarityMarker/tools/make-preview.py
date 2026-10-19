"""生成 mod 的 256x256 preview.png（游戏要求 info.ini 同目录下必须有 preview.png）。

图形含义：一排六个交互点标记（环 + 点），颜色就是游戏自己的六档品质色
白/绿/蓝/紫/橙/红；下方是容器（箱子）剪影。
即「一眼看懂：箱子的交互点会被染成里面最好的那件东西的颜色」。
"""
import os
from PIL import Image, ImageDraw

SIZE = 256
BG = (22, 28, 38)
BORDER = (52, 64, 82)

# 游戏自己的 displayQualityLooks 颜色（从发行资源里挖出来的原值）
QUALITY_COLORS = [
    (255, 255, 255),  # White
    (139, 233, 19),   # Green  #8BE913
    (17, 184, 241),   # Blue   #11B8F1
    (153, 15, 245),   # Purple #990FF5
    (255, 125, 37),   # Orange #FF7D25
    (255, 47, 47),    # Red    #FF2F2F
]

img = Image.new("RGB", (SIZE, SIZE), BG)
d = ImageDraw.Draw(img)

# 外框
d.rectangle([2, 2, SIZE - 3, SIZE - 3], outline=BORDER, width=3)

# ---- 背景柔光（让中间亮一点）----
glow = Image.new("RGB", (SIZE, SIZE), (0, 0, 0))
gd = ImageDraw.Draw(glow)
for i in range(18, 0, -1):
    v = int(90 * (i / 18.0) ** 3)
    gd.ellipse([128 - i * 7, 104 - i * 7, 128 + i * 7, 104 + i * 7], fill=(v // 3, v // 2, v))
img = Image.blend(img, Image.blend(img, glow, 0.35), 0.6)
d = ImageDraw.Draw(img)

# ---- 六个品质标记 ----
y = 100
r = 13
for idx, color in enumerate(QUALITY_COLORS):
    cx = 34 + idx * 37.6
    cx = int(round(cx))
    # 环
    d.ellipse([cx - r, y - r, cx + r, y + r], outline=color, width=4)
    # 中心点
    d.ellipse([cx - 5, y - 5, cx + 5, y + 5], fill=color)
    # 一点点外发光
    for k in range(1, 4):
        a = 0.26 / k
        ring = tuple(int(BG[i] + (color[i] - BG[i]) * a) for i in range(3))
        d.ellipse([cx - r - k, y - r - k, cx + r + k, y + r + k], outline=ring, width=1)

# ---- 容器剪影 ----
body = (74, 63, 46)
edge = (128, 110, 76)
lock = (206, 176, 98)
d.rounded_rectangle([84, 174, 172, 220], radius=7, fill=body, outline=edge, width=2)
d.line([86, 189, 170, 189], fill=edge, width=2)
d.rounded_rectangle([122, 184, 134, 199], radius=2, fill=lock)
d.ellipse([126, 190, 130, 194], fill=(60, 50, 30))

# 地面阴影
d.line([70, 226, 186, 226], fill=(38, 46, 60), width=4)

out = os.path.normpath(
    os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "dist", "preview.png")
)
img.save(out)
print("preview.png written:", out, img.size)
