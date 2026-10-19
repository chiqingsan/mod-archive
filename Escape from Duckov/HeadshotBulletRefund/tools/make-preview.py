"""生成 mod 的 256x256 preview.png（游戏要求 info.ini 同目录下的 preview.png）。

图形：深色底 + 准星圈（爆头）+ 一颗金色子弹 + 右下角 "+1"。
"""
import os
from PIL import Image, ImageDraw, ImageFont

SIZE = 256
BG = (24, 30, 40)
ACCENT = (232, 76, 61)
GOLD = (240, 190, 74)
WHITE = (236, 240, 246)

img = Image.new("RGB", (SIZE, SIZE), BG)
d = ImageDraw.Draw(img)

# 外框
d.rectangle([2, 2, SIZE - 3, SIZE - 3], outline=(52, 64, 82), width=3)

# 准星圈（爆头）
cx, cy, r = 128, 104, 46
d.ellipse([cx - r, cy - r, cx + r, cy + r], outline=ACCENT, width=6)
d.line([cx - r - 14, cy, cx - r + 6, cy], fill=ACCENT, width=6)
d.line([cx + r - 6, cy, cx + r + 14, cy], fill=ACCENT, width=6)
d.line([cx, cy - r - 14, cx, cy - r + 6], fill=ACCENT, width=6)
d.line([cx, cy + r - 6, cx, cy + r + 14], fill=ACCENT, width=6)

# 中心弹孔
d.ellipse([cx - 12, cy - 12, cx + 12, cy + 12], fill=ACCENT)

# 子弹（斜放在底部）
bx, by = 128, 200
d.rounded_rectangle([bx - 26, by - 14, bx + 14, by + 14], radius=7, fill=GOLD)
d.polygon([(bx + 14, by - 14), (bx + 40, by), (bx + 14, by + 14)], fill=(214, 152, 46))
d.line([bx - 18, by - 14, bx - 18, by + 14], fill=(196, 140, 40), width=3)

# "+1"
try:
    font = ImageFont.truetype("C:/Windows/Fonts/arialbd.ttf", 46)
except OSError:
    font = ImageFont.load_default()

text = "+1"
bbox = d.textbbox((0, 0), text, font=font)
tw, th = bbox[2] - bbox[0], bbox[3] - bbox[1]
tx, ty = SIZE - tw - 16, 8 - bbox[1]
d.text((tx + 2, ty + 2), text, font=font, fill=(10, 14, 20))
d.text((tx, ty), text, font=font, fill=WHITE, stroke_width=1, stroke_fill=(10, 14, 20))

out = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "dist", "preview.png"))
img.save(out)
print("preview.png written:", out, img.size)
