# 生成 BatchLabel-Final/preview.png（占位预览图，正式发布前建议换成实机截图）
from PIL import Image, ImageDraw, ImageFont

W, H = 483, 276
img = Image.new("RGB", (W, H), (18, 21, 27))
d = ImageDraw.Draw(img, "RGBA")

# 背景：上下渐变 + 淡淡的库存格
for y in range(H):
    t = y / (H - 1)
    d.line([(0, y), (W, y)], fill=(int(24 + 12 * t), int(28 + 12 * t), int(36 + 12 * t)))

cell = 34
pad = 5
grid_x, grid_y = 26, 128
for row in range(3):
    for col in range(9):
        x = grid_x + col * (cell + pad)
        y = grid_y + row * (cell + pad)
        if x + cell > W - 26:
            continue
        d.rounded_rectangle([x, y, x + cell, y + cell], radius=5,
                            fill=(38, 42, 52, 220), outline=(58, 64, 78, 255))

# 选中的物品格 + 标签角标
for (col, row) in [(2, 0), (3, 0), (4, 0), (2, 1), (3, 1), (4, 1)]:
    x = grid_x + col * (cell + pad)
    y = grid_y + row * (cell + pad)
    d.rounded_rectangle([x, y, x + cell, y + cell], radius=5, fill=(54, 93, 148, 235))
    # 小标签图标
    tx, ty = x + cell - 14, y + 4
    d.polygon([(tx, ty), (tx + 8, ty), (tx + 11, ty + 3), (tx + 11, ty + 11), (tx, ty + 11)],
              fill=(255, 176, 64))
    d.ellipse([tx + 3, ty + 3, tx + 6, ty + 6], fill=(30, 32, 38))

# 多选框
bx, by = grid_x + 2 * (cell + pad) - 6, grid_y - 6
bw, bh = 3 * (cell + pad) - pad + 12, 2 * (cell + pad) - pad + 12
d.rectangle([bx, by, bx + bw, by + bh], outline=(96, 166, 255), width=2)
d.rectangle([bx, by, bx + bw, by + bh], fill=(96, 166, 255, 26))

font_title = ImageFont.truetype(r"C:\Windows\Fonts\msyh.ttc", 27)
font_sub = ImageFont.truetype(r"C:\Windows\Fonts\msyh.ttc", 13)
font_key = ImageFont.truetype(r"C:\Windows\Fonts\msyhbd.ttc", 13)

d.text((26, 22), "标签机批量打标签", font=font_title, fill=(240, 244, 250))

# 键帽 Space
kx, ky = 26, 66
key_w, key_h = 58, 24
d.rounded_rectangle([kx, ky, kx + key_w, ky + key_h], radius=5, fill=(255, 176, 64))
d.text((kx + 9, ky + 4), "Space", font=font_key, fill=(24, 26, 32))

d.text((kx + key_w + 10, ky + 5), "+ 拖出框选 → 松开后同类型物品批量复制标签",
       font=font_sub, fill=(176, 184, 198))

d.text((26, H - 34), "装备标签机 · 在已打标签的物品上起手 · 只影响框内同类物品",
       font=font_sub, fill=(126, 134, 150))

img.save("preview.png")
print("saved preview.png", img.size)
