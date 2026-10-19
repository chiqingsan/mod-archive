# -*- coding: utf-8 -*-
"""量级探测 v2：逐关卡统计 InteractableBase 家族（含 InteractableLootbox）实例数。

v1 (probe_scale.py) 的教训：脚本识别那一步没挂 TypeTreeGenerator，
`read_typetree()` 全抛异常 → 家族集合是空的 → 全表 0。

v2 直接复用 probe_marker_visible.py 已经验出来的 14 个脚本 pathID
（在 resources.assets 里含 interactMarkerVisible 字段的组），
然后对每个容器只做「MonoBehaviour 固定头解析」，很快。

为什么要这两个数字：
  * InteractableBase 家族总数 = 25 米 Physics.OverlapSphereNonAlloc 命中数的绝对上限
    （InteractableBase.cs:216 会把 interactCollider 强制设到 Interactable 层，
     所以序列化层名不算数，组件数才是真的）
  * InteractableLootbox 总数 = 本 mod 会跟踪的容器数上限，
    也就是「每次扫描要遍历多少个 Inventory」的上限

只读。输出 tools/probe/probe_out/scale_probe2.txt
"""
import os
import struct
import sys
import time
import traceback
from collections import Counter

import UnityPy

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)
OUTFILE = os.path.join(OUT, "scale_probe2.txt")

# probe_marker_visible.py 在 resources.assets 里验出来的 InteractableBase 家族
FAMILY = {5078, 268, 598, 3920, 1287, 3827, 883, 1368,
          3018, 2731, 1504, 2071, 2005, 5104}
LOOTBOX = 268
# 另外单独关心的：LootBoxLoader（异步装填内容的那位）
EXTRA_WATCH = {}

lines = []


def log(msg=""):
    print(msg)
    sys.stdout.flush()
    lines.append(str(msg))


def flush():
    with open(OUTFILE, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))


def read_mb_header(raw):
    return (struct.unpack_from("<q", raw, 4)[0],   # m_GameObject pathID
            struct.unpack_from("<q", raw, 20)[0])  # m_Script pathID


def list_containers():
    out = []
    for name in sorted(os.listdir(GAME)):
        p = os.path.join(GAME, name)
        if not os.path.isfile(p):
            continue
        if name.endswith((".resS", ".json", ".info", ".config")):
            continue
        out.append(name)
    return out


def main():
    t0 = time.time()
    containers = list_containers()
    log("资源容器数量 = %d" % len(containers))
    log("InteractableBase 家族脚本 pathID = %s" % sorted(FAMILY))
    log("")

    rows = []
    for cname in containers:
        path = os.path.join(GAME, cname)
        try:
            env = UnityPy.load(path)
        except Exception:
            continue
        mb = 0
        family = 0
        loot = 0
        other_spid = Counter()
        try:
            for o in env.objects:
                if o.type.name != "MonoBehaviour":
                    continue
                mb += 1
                try:
                    _, spid = read_mb_header(o.get_raw_data())
                except Exception:
                    continue
                if spid in FAMILY:
                    family += 1
                    if spid == LOOTBOX:
                        loot += 1
                else:
                    other_spid[spid] += 1
        finally:
            del env
        rows.append((cname, mb, family, loot, other_spid))
        log("%-28s MonoBeh=%-7d 可交互家族=%-6d Lootbox=%-5d" % (cname, mb, family, loot))
        flush()

    log("")
    log("=" * 78)
    log("按「可交互家族实例数」排序（25 米球查询命中数的绝对上限）")
    log("=" * 78)
    log("%-28s %10s %10s %10s" % ("容器", "MonoBeh", "可交互家族", "Lootbox"))
    for r in sorted(rows, key=lambda x: -x[2])[:30]:
        log("%-28s %10d %10d %10d" % (r[0], r[1], r[2], r[3]))

    nz = [r for r in rows if r[2] > 0]
    log("")
    log("含可交互物的容器数 = %d / %d" % (len(nz), len(rows)))
    if nz:
        w = max(nz, key=lambda x: x[2])
        wl = max(nz, key=lambda x: x[3])
        log("  可交互家族最多 : %-24s %d 个" % (w[0], w[2]))
        log("  Lootbox 最多   : %-24s %d 个" % (wl[0], w[3]))
        log("  可交互家族合计 : %d 个" % sum(r[2] for r in nz))
        log("  Lootbox 合计   : %d 个" % sum(r[3] for r in nz))
    log("")
    log("总耗时 %.1f 秒" % (time.time() - t0))
    flush()


if __name__ == "__main__":
    try:
        main()
    except Exception:
        log(traceback.format_exc())
    flush()
    print("\n[写出] %s" % OUTFILE)
