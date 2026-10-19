# -*- coding: utf-8 -*-
"""探测 v5：直接从 GameplayDataSettings 的原始字节里抠出品质色表。

v4 已经定位到对象：resources.assets pathID=76721，m_Name="GameplayDataSettings"。
但 read_typetree() 抛 EOFError（自动生成的类型树与数据长度对不上，多半是某个字段被剥离），
所以改成按序列化布局手工扫描。

从反编译代码可知布局（GameplayDataSettings.cs:551-570）：
    [Serializable] class DisplayQualityLook {
        public DisplayQuality quality;   // int32 枚举
        public float shadowOffset;       // float32
        public Color shadowColor;        // 4 x float32
        public bool  innerGlow;          // uint8 + 3 字节对齐
    }
    [SerializeField] private List<DisplayQualityLook> displayQualityLooks;

List 的序列化 = int32 count + count 个元素（每元素 28 字节）。

扫描策略：在原始字节里找 count∈[1,10] 且后续 count 个 28 字节记录都满足
  quality ∈ [0,8]，shadowOffset ∈ [-4,64]，color 各分量 ∈ [-0.05,4]，innerGlow ∈ {0,1}
的位置，按“valid 记录数”打分，输出最高分。
"""
import os
import struct

import UnityPy

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)

REC = 28  # int32 + float32 + 4*float32 + uint8(+3 pad)

QUALITY_NAMES = {0: "None", 1: "White", 2: "Green", 3: "Blue",
                 4: "Purple", 5: "Orange", 6: "Red", 7: "Q7", 8: "Q8"}

lines = []


def log(m=""):
    print(m)
    lines.append(str(m))


def try_record(raw, off):
    if off + REC > len(raw):
        return None
    q, so = struct.unpack_from("<if", raw, off)
    c = struct.unpack_from("<4f", raw, off + 8)
    glow = raw[off + 24]
    if not (0 <= q <= 8):
        return None
    if not (-4.0 <= so <= 64.0):
        return None
    if not all(-0.05 <= v <= 4.0 for v in c):
        return None
    if glow not in (0, 1):
        return None
    # 3 字节对齐填充必须是 0
    if raw[off + 25] != 0 or raw[off + 26] != 0 or raw[off + 27] != 0:
        return None
    return (q, so, c, bool(glow))


def scan(raw):
    best = None
    for off in range(0, len(raw) - 4, 4):
        cnt = struct.unpack_from("<i", raw, off)[0]
        if not (1 <= cnt <= 10):
            continue
        recs = []
        ok = True
        for i in range(cnt):
            r = try_record(raw, off + 4 + i * REC)
            if r is None:
                ok = False
                break
            recs.append(r)
        if not ok:
            continue
        # 品质应当互不相同（每档一条）
        qs = [r[0] for r in recs]
        if len(set(qs)) != len(qs):
            continue
        score = cnt * 10 + (5 if all(1 <= q <= 6 for q in qs) else 0)
        if best is None or score > best[0]:
            best = (score, off, cnt, recs)
    return best


def main():
    env = UnityPy.load(os.path.join(GAME, "resources.assets"))
    tgt = None
    for o in env.objects:
        if o.type.name == "MonoBehaviour":
            continue
    # 直接按已知 pathID 取
    by_id = {o.path_id: o for o in env.objects}
    tgt = by_id.get(76721)
    if tgt is None:
        log("!! 找不到 pathID 76721")
        return
    raw = tgt.get_raw_data()
    log("GameplayDataSettings pathID=76721, raw %d 字节" % len(raw))

    best = scan(raw)
    if best is None:
        log("!! 没扫到符合条件的品质色表")
        return
    score, off, cnt, recs = best
    log("最佳命中: offset=%d count=%d score=%d" % (off, cnt, score))
    log("")
    log("品质色表（来自游戏资产，非猜测）:")
    log("  %-8s %-14s %-8s %s" % ("quality", "shadowOffset", "innerGlow", "shadowColor"))
    for q, so, c, glow in recs:
        log("  %-8s %-14.4f %-8s RGBA(%.4f, %.4f, %.4f, %.4f)  #%02X%02X%02X"
            % ("%s(%d)" % (QUALITY_NAMES.get(q, "?"), q), so, glow,
               c[0], c[1], c[2], c[3],
               max(0, min(255, int(round(c[0] * 255)))),
               max(0, min(255, int(round(c[1] * 255)))),
               max(0, min(255, int(round(c[2] * 255))))))

    # 上下文十六进制，便于人工复核
    lo = max(0, off - 32)
    hi = min(len(raw), off + 4 + cnt * REC + 32)
    log("")
    log("上下文 hexdump (offset %d..%d):" % (lo, hi))
    for i in range(lo, hi, 16):
        chunk = raw[i:i + 16]
        log("  %06x  %-47s  %s" % (i, " ".join("%02x" % b for b in chunk),
                                   "".join(chr(b) if 32 <= b < 127 else "." for b in chunk)))

    with open(os.path.join(OUT, "quality_colors.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))


if __name__ == "__main__":
    main()
