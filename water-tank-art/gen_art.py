from PIL import Image, ImageDraw, ImageFilter
import math, os

W = H = 64
OUT = r"D:\code\mod\water-tank-art"
os.makedirs(OUT, exist_ok=True)

# ---------------- palette ----------------
OUTLINE  = (40, 49, 62, 255)
HI       = (249, 253, 254, 255)
T1       = (236, 247, 250, 255)
BASE     = (215, 234, 241, 255)
T2       = (194, 219, 228, 255)
MID      = (169, 199, 212, 255)
SH       = (139, 172, 188, 255)
CAP_HI   = (190, 227, 245, 255)
CAP_MID  = (88, 137, 176, 255)
CAP_LOW  = (56, 96, 132, 255)
CAP_DARK = (38, 67, 96, 255)
NECK     = (229, 245, 250, 255)
NECK_SH  = (183, 209, 219, 255)
RING     = (150, 186, 201, 255)

# ---------------- silhouette ----------------
spans = {}
def put(y0, y1, x0, x1):
    for y in range(y0, y1 + 1):
        spans[y] = (x0, x1)

CX = 31.5
def put_hw(y, hw):
    spans[y] = (int(round(CX - hw)), int(round(CX + hw)))

# cap 2..5 (blue screw cap)
put(2, 2, 27, 36)
put(3, 4, 26, 37)
put(5, 5, 27, 36)
# neck 6..9
put(6, 9, 29, 34)
# collar/tamper ring 10..11
put(10, 10, 27, 36)
put(11, 11, 28, 35)
# shoulder 12..21
BODY_HALF = 18.0
NECK_HALF = 4.5
Y0, Y1 = 12, 21
for y in range(Y0, Y1 + 1):
    u = (y - Y0) / float(Y1 - Y0)
    hw = NECK_HALF + (BODY_HALF - NECK_HALF) * math.sin(math.pi / 2 * (u ** 0.62))
    put_hw(y, hw)
# body 22..57 (straight barrel with a very slight bulge, flat base)
for y in range(22, 58):
    t = (y - 22) / 34.0
    hw = BODY_HALF + 0.25 - 0.35 * max(0.0, (t - 0.85) / 0.15) ** 2
    put_hw(y, hw)
# bottom rim row
put(58, 58, 16, 47)

# ---------------- masks ----------------
sil = Image.new('L', (W, H), 0); sp = sil.load()
for y, (x0, x1) in spans.items():
    for x in range(x0, x1 + 1):
        sp[x, y] = 255
eroded = sil.filter(ImageFilter.MinFilter(3))
edge = Image.new('L', (W, H), 0); ep = edge.load()
for y in range(H):
    for x in range(W):
        if sil.getpixel((x, y)) and not eroded.getpixel((x, y)):
            ep[x, y] = 255

def is_edge(x, y):
    return 0 <= x < W and 0 <= y < H and edge.getpixel((x, y)) > 0

def row_bounds_of(mask_img, y):
    xs = [x for x in range(W) if mask_img.getpixel((x, y)) > 0]
    return (xs[0], xs[-1]) if xs else None

img = Image.new('RGBA', (W, H), (0, 0, 0, 0)); px = img.load()

# --- body: fixed-x bands (cylinder) ---
BODY_L, BODY_R = 12, 51
def band_abs(x):
    u = (x - BODY_L) / float(BODY_R - BODY_L)
    if u < 0.26:
        return T1
    if u < 0.62:
        return BASE
    if u < 0.88:
        return T2
    return MID

for y in range(13, 59):
    if y not in spans:
        continue
    x0, x1 = spans[y]
    for x in range(x0, x1 + 1):
        if is_edge(x, y):
            continue
        px[x, y] = band_abs(x)

# specular highlight: 2px fixed columns on the left flank
for y in range(16, 55):
    if y not in spans:
        continue
    x0, x1 = spans[y]
    for sx, col in ((16, HI), (17, T1)):
        if x0 < sx < x1 and not is_edge(sx, y):
            px[sx, y] = col

# very bottom: one row of shade only, so the water line sits on the outline
for y in (57, 58):
    if y not in spans:
        continue
    x0, x1 = spans[y]
    for x in range(x0, x1 + 1):
        if not is_edge(x, y):
            px[x, y] = SH

# neck 6..9
for y in range(6, 10):
    x0, x1 = spans[y]
    for x in range(x0, x1 + 1):
        if is_edge(x, y):
            continue
        u = (x - x0) / float(max(1, x1 - x0))
        px[x, y] = NECK if u < 0.58 else NECK_SH

# tamper ring 10..11
for y, col in ((10, RING), (11, NECK_SH)):
    if y not in spans:
        continue
    x0, x1 = spans[y]
    for x in range(x0 + 1, x1):
        if not is_edge(x, y):
            px[x, y] = col

# cap
for y in range(2, 6):
    x0, x1 = spans[y]
    for x in range(x0, x1 + 1):
        if is_edge(x, y):
            continue
        if y == 2:
            c = CAP_HI
        elif y == 3:
            c = CAP_MID
        elif y == 4:
            c = CAP_HI if x < 30 else CAP_MID
        else:
            c = CAP_LOW
        if x <= x0 + 1:
            c = tuple(min(255, v + 18) for v in c[:3]) + (255,)
        elif x >= x1 - 1:
            c = CAP_DARK
        px[x, y] = c

# cap ribs (screw threads)
for x in range(27, 37, 2):
    if 4 in spans and spans[4][0] < x < spans[4][1]:
        px[x, 4] = CAP_LOW
    if 3 in spans and spans[3][0] < x < spans[3][1]:
        px[x + 1, 3] = CAP_LOW

# outline
for y in range(H):
    for x in range(W):
        if is_edge(x, y):
            px[x, y] = OUTLINE

img.save(os.path.join(OUT, 'watertank_body.png'))

# ---------------- liquid layers ----------------
inner = sil.filter(ImageFilter.MinFilter(3)).filter(ImageFilter.MinFilter(3))
def inside(x, y):
    return 0 <= x < W and 0 <= y < H and inner.getpixel((x, y)) > 0

LEVELS = [('10', 0.10), ('25', 0.25), ('50', 0.50), ('75', 0.75), ('100', 1.00)]
Y_BOT, FILL_TOP = 58, 22

def surf(lv):
    return int(round(Y_BOT - (Y_BOT - FILL_TOP) * lv))

for name, lv in LEVELS:
    im = Image.new('RGBA', (W, H), (0, 0, 0, 0)); p = im.load()
    sy = surf(lv)
    for y in range(sy, Y_BOT + 1):
        for x in range(W):
            if not inside(x, y):
                continue
            rel = y - sy
            if rel <= 2:
                c = (253, 255, 255, 255)
            elif rel <= 9:
                c = (244, 250, 251, 255)
            else:
                c = (231, 241, 243, 255)
            p[x, y] = c
    for y in range(sy, Y_BOT + 1):
        b = row_bounds_of(inner, y)
        if b:
            l, r = b
            if r - l > 5:
                p[l, y] = (250, 253, 254, 255)
                p[l + 1, y] = (246, 251, 252, 255)
                p[r - 1, y] = (220, 231, 234, 255)
                p[r, y] = (204, 217, 221, 255)
    for x in range(W):
        wave = int(round(math.sin(x / 4.2 + lv * 6.0) * 0.6))
        yy = sy + wave
        if inside(x, yy):
            p[x, yy] = (255, 255, 255, 255)
            if inside(x, yy + 1):
                p[x, yy + 1] = (250, 253, 254, 255)
    im.save(os.path.join(OUT, f'watertank_liquid_{name}.png'))

# highlight layers
for name, lv in LEVELS:
    im = Image.new('RGBA', (W, H), (0, 0, 0, 0)); p = im.load()
    sy = surf(lv)
    for x in range(W):
        if ((x * 7 + int(lv * 19)) % 13) < 5 and inside(x, sy + 1):
            p[x, sy + 1] = (255, 255, 255, 226)
    amt = 0.35 + 0.65 * lv
    dashes = [(20, 5, 6), (30, 8, 5), (39, 11, 6), (23, 15, 5), (33, 17, 7), (26, 21, 4), (36, 24, 6), (21, 28, 5), (41, 31, 5)]
    for i, (dx, dy, ln) in enumerate(dashes):
        if i / float(len(dashes)) > amt:
            continue
        yy = sy + dy
        if yy > Y_BOT - 2:
            continue
        for x in range(dx, dx + ln):
            if inside(x, yy):
                p[x, yy] = (255, 255, 255, 82 if (x - dx) in (0, ln - 1) else 184)
    bubbles = [(25, 8), (33, 12), (39, 15), (23, 19), (30, 22), (35, 26), (26, 30), (40, 21)]
    for (bx, by) in bubbles:
        yy = sy + by
        if yy > Y_BOT - 1:
            continue
        if inside(bx, yy):
            p[bx, yy] = (255, 255, 255, 158)
            if inside(bx, yy + 1):
                p[bx, yy + 1] = (255, 255, 255, 84)
    b = row_bounds_of(inner, sy + 4)
    if b:
        for yy in range(sy + 4, min(sy + 13, Y_BOT)):
            if inside(b[0] + 3, yy):
                p[b[0] + 3, yy] = (255, 255, 255, 108)
    im.save(os.path.join(OUT, f'watertank_hl_{name}.png'))

# ---------------- previews ----------------
body = Image.open(os.path.join(OUT, 'watertank_body.png')).convert('RGBA')

def tint(im, hexc):
    r, g, b = int(hexc[0:2], 16), int(hexc[2:4], 16), int(hexc[4:6], 16)
    out = im.copy(); o = out.load(); i = im.load()
    for y in range(im.height):
        for x in range(im.width):
            R, G, B, A = i[x, y]
            if A:
                o[x, y] = (R * r // 255, G * g // 255, B * b // 255, A)
    return out

def composite(lv, hexc):
    comp = Image.alpha_composite(Image.new('RGBA', (W, H), (0, 0, 0, 0)), body)
    if lv:
        comp = Image.alpha_composite(comp, tint(Image.open(os.path.join(OUT, f'watertank_liquid_{lv}.png')), hexc))
        comp = Image.alpha_composite(comp, Image.open(os.path.join(OUT, f'watertank_hl_{lv}.png')))
    return comp

labels = ['empty', '10%', '25%', '50%', '75%', '100%']
preview_levels = [None, '10', '25', '50', '75', '100']
colsets = [('water', '67aaa5'), ('cold', '5f9fc4'), ('dirty', '9c8fc2')]

def text(draw, x, y, s, fill=(206, 214, 218, 255)):
    draw.text((x, y), s, fill=fill)

cell, pad = 256, 16
sheet = Image.new('RGBA', (6 * cell + 7 * pad, 3 * cell + 6 * pad), (26, 30, 34, 255))
d = ImageDraw.Draw(sheet)
for ri, (_, hexc) in enumerate(colsets):
    for ci, lv in enumerate(preview_levels):
        x = pad + ci * (cell + pad)
        y = pad + ri * (cell + pad)
        sheet.alpha_composite(composite(lv, hexc).resize((cell, cell), Image.NEAREST), (x, y))
for ci, lab in enumerate(labels):
    text(d, pad + ci * (cell + pad) + 8, 3 * cell + 5 * pad - 4, lab)
sheet.save(os.path.join(OUT, 'preview_4x.png'))

strip = Image.new('RGBA', (6 * 202 + 7 * 10, 202 + 40), (26, 30, 34, 255))
d = ImageDraw.Draw(strip)
for i, lv in enumerate(preview_levels):
    x = 10 + i * 202
    strip.alpha_composite(composite(lv, '67aaa5').resize((192, 192), Image.NEAREST), (x, 8))
    text(d, x + 76, 208, labels[i])
strip.save(os.path.join(OUT, 'preview_strip_3x.png'))

slot = (58, 73, 68, 255)
a = Image.new('RGBA', (6 * 68 + 16, 88), slot)
d = ImageDraw.Draw(a)
for i, lv in enumerate(preview_levels):
    comp = composite(lv, '67aaa5')
    cellbg = Image.new('RGBA', (64, 64), slot)
    a.alpha_composite(Image.alpha_composite(cellbg, comp), (8 + i * 68, 4))
    text(d, (8 + i * 68 + 16), 72, labels[i])
a.resize((a.width * 2, a.height * 2), Image.NEAREST).save(os.path.join(OUT, 'preview_slots_2x.png'))

orig = Image.open(r'D:\code\mod\.tmp-water-art\xl_composite_full.png').convert('RGBA')
cmp_ = Image.new('RGBA', (2 * 256 + 3 * 20, 256 + 64), (26, 30, 34, 255))
d = ImageDraw.Draw(cmp_)
cmp_.alpha_composite(orig.resize((256, 256), Image.NEAREST), (20, 14))
cmp_.alpha_composite(composite('75', '67aaa5').resize((256, 256), Image.NEAREST), (296, 14))
text(d, 20, 282, 'VANILLA xl (water_container4)')
text(d, 296, 282, 'CUSTOM self-drawn jug')
cmp_.save(os.path.join(OUT, 'compare_original_vs_new_4x.png'))

z = 8
zoom = Image.new('RGBA', (W * z, H * z), (26, 30, 34, 255))
zoom.alpha_composite(body.resize((W * z, H * z), Image.NEAREST), (0, 0))
zoom.save(os.path.join(OUT, 'zoom_empty_8x.png'))

row = Image.new('RGBA', (6 * 74 + 10, 92), (26, 30, 34, 255))
d = ImageDraw.Draw(row)
for i, lv in enumerate(preview_levels):
    x = 6 + i * 74
    row.alpha_composite(composite(lv, '67aaa5'), (x, 6))
    text(d, x + 20, 76, labels[i])
row.save(os.path.join(OUT, 'preview_1x.png'))

print('done v20')
