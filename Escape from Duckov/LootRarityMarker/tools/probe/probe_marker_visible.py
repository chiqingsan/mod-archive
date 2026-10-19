# -*- coding: utf-8 -*-
"""确认容器预制体上 interactMarkerVisible 的序列化取值（v2：按类名归类）。

v1 的教训：脚本组是按 m_Script.m_PathID 分的，派生类各有自己的组，
v1 在第一个命中组（InteractableBase 本体，只有 8 个实例）就 break 了，漏掉了所有子类。
v2 改成先建立 MonoScript 的 pathID -> 类名 映射，再扫全部 850 个组。

为什么必须查：InteractableBase.Start() 里是 `MarkerActive = interactMarkerVisible;`，
而 MarkerActive 的 setter 只在 value==true 时才 ActiveMarker()（实例化交互点）。
如果箱子（InteractableLootbox）的预制体里 interactMarkerVisible 是 false，
那箱子身上根本不会有 InteractMarker，路线 B（染交互点）就没有作用对象。

只读。
"""
import os
import struct
import traceback
from collections import Counter, defaultdict

import UnityPy
from UnityPy.helpers.TypeTreeGenerator import TypeTreeGenerator

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
MANAGED = os.path.join(GAME, "Managed")
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)

FIELD = "interactMarkerVisible"
# 用来认类的特征字段
SIGNATURES = {
    "InteractableLootbox": ("inventoryReference", "displayNameKey"),
    "InteractablePickup": ("itemId", "pickupSound"),
    "InteractableBase": (),
}
lines = []


def log(msg=""):
    print(msg)
    lines.append(str(msg))


def read_header(raw):
    go_pid = struct.unpack_from("<q", raw, 4)[0]
    script_pid = struct.unpack_from("<q", raw, 20)[0]
    nlen = struct.unpack_from("<i", raw, 28)[0]
    name = raw[32:32 + nlen].decode("utf-8", "replace") if 0 < nlen < 200 else ""
    return go_pid, script_pid, name


def main():
    gen = TypeTreeGenerator("2022.3.62f2")
    gen.load_local_dll_folder(MANAGED)

    path = os.path.join(GAME, "resources.assets")
    env = UnityPy.load(path)
    env.typetree_generator = gen

    # ---- 1) MonoScript pathID -> 类名 ----
    script_names = {}
    for o in env.objects:
        if o.type.name != "MonoScript":
            continue
        try:
            tt = o.read_typetree()
        except Exception:
            try:
                tt = o.read().__dict__
            except Exception:
                continue
        cls = tt.get("m_ClassName") or ""
        ns = tt.get("m_Namespace") or ""
        asm = tt.get("m_AssemblyName") or ""
        if cls:
            script_names[o.path_id] = (ns + "." + cls) if ns else cls
            if asm:
                script_names[o.path_id] += "  [%s]" % asm
    log("MonoScript 数量 = %d（其中能拿到类名的 %d）" % (
        sum(1 for o in env.objects if o.type.name == "MonoScript"), len(script_names)))

    # ---- 2) 按脚本分组 ----
    groups = defaultdict(list)
    for o in env.objects:
        if o.type.name != "MonoBehaviour":
            continue
        try:
            raw = o.get_raw_data()
        except Exception:
            continue
        try:
            go_pid, spid, name = read_header(raw)
        except Exception:
            continue
        groups[spid].append((o, go_pid, name))

    log("MonoBehaviour 组数 = %d" % len(groups))
    log("")

    # ---- 3) 找出所有含 interactMarkerVisible 的组 ----
    parsed = 0
    found_any = False
    for spid, members in sorted(groups.items(), key=lambda kv: -len(kv[1])):
        o = members[0][0]
        try:
            tt = o.read_typetree()
        except Exception:
            continue
        parsed += 1
        if FIELD not in tt:
            continue

        found_any = True
        cls = script_names.get(spid, "<未知脚本 pathID=%d>" % spid)
        keys = set(tt.keys())
        kind = "其它 Interactable 子类"
        for k, sig in SIGNATURES.items():
            if sig and all(s in keys for s in sig):
                kind = k
                break
            if not sig and "inventoryReference" not in keys and "itemId" not in keys:
                kind = k

        counter = Counter()
        offsets = Counter()
        for mo, go_pid, name in members:
            try:
                mtt = mo.read_typetree()
            except Exception:
                counter["<解析失败>"] += 1
                continue
            counter[str(mtt.get(FIELD))] += 1
            off = mtt.get("interactMarkerOffset") or {}
            offsets["y=%s" % off.get("y")] += 1

        log("=" * 78)
        log("%s   (脚本 pathID=%d, 实例 %d 个)" % (cls, spid, len(members)))
        log("  判定为: %s" % kind)
        log("  %s 分布: %s" % (FIELD, dict(counter)))
        log("  interactMarkerOffset 分布: %s" % dict(offsets))
        log("  该类全部序列化字段: %s" % sorted(keys))
        log("")

    log("扫了 %d 个组的 typetree" % parsed)
    if not found_any:
        log("!! 没有任何组含 %s 字段" % FIELD)

    with open(os.path.join(OUT, "marker_visible_probe.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % os.path.join(OUT, "marker_visible_probe.txt"))


if __name__ == "__main__":
    try:
        main()
    except Exception:
        log(traceback.format_exc())
        with open(os.path.join(OUT, "marker_visible_probe.txt"), "w", encoding="utf-8") as f:
            f.write("\n".join(lines))
