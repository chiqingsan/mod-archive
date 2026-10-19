# -*- coding: utf-8 -*-
"""探测 v4：按 m_Script 分组，快速找到 GameplayDataSettings。

为什么不用 v3 的做法：resources.assets 里有 38606 个 MonoBehaviour，逐个 read_typetree()
要 ~70 分钟（实测 0.11s/个）。但同一个 C# 类在同一个文件里的所有实例共享同一个
m_Script 引用，所以：

  1) 手工解析 MonoBehaviour 的固定头部（不需要类型树）拿到 m_Script.m_PathID；
  2) 按 m_Script.m_PathID 分组 —— 只有几百组；
  3) 每组取一个代表做 read_typetree()，看哪个组带 uiStyleData / interactMarker。

固定头部布局（Unity 2022）：
  0   int32  m_GameObject.m_FileID
  4   int64  m_GameObject.m_PathID
  12  uint8  m_Enabled + 3 字节对齐
  16  int32  m_Script.m_FileID
  20  int64  m_Script.m_PathID
  28  字符串 m_Name（int32 长度 + UTF-8 + 4 字节对齐）
"""
import os
import struct
import json
import traceback
import collections

import UnityPy
from UnityPy.helpers.TypeTreeGenerator import TypeTreeGenerator

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
MANAGED = os.path.join(GAME, "Managed")
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)
UNITY_VER = "2022.3.62f2"

lines = []


def log(m=""):
    print(m)
    lines.append(str(m))


def fmt_color(c):
    try:
        r, g, b, a = c.r, c.g, c.b, c.a
    except Exception:
        return repr(c)
    return "RGBA(%.4f, %.4f, %.4f, %.4f)  #%02X%02X%02X" % (
        r, g, b, a,
        max(0, min(255, int(round(r * 255)))),
        max(0, min(255, int(round(g * 255)))),
        max(0, min(255, int(round(b * 255)))))


def parse_header(raw):
    """返回 (go_pathid, script_fileid, script_pathid, name)。"""
    if len(raw) < 32:
        return None
    try:
        go_fid = struct.unpack_from("<i", raw, 0)[0]
        go_pid = struct.unpack_from("<q", raw, 4)[0]
        s_fid = struct.unpack_from("<i", raw, 16)[0]
        s_pid = struct.unpack_from("<q", raw, 20)[0]
        nlen = struct.unpack_from("<i", raw, 28)[0]
        name = ""
        if 0 <= nlen <= 512 and 32 + nlen <= len(raw):
            name = raw[32:32 + nlen].decode("utf-8", "replace")
        return go_pid, s_fid, s_pid, name
    except Exception:
        return None


def main():
    gen = TypeTreeGenerator(UNITY_VER)
    gen.load_local_dll_folder(MANAGED)
    log("TypeTreeGenerator 载入 %d 个 dll" % len(gen.get_loaded_dll_names()))

    rel = "resources.assets"
    env = UnityPy.load(os.path.join(GAME, rel))
    env.typetree_generator = gen

    groups = collections.defaultdict(list)
    headers = {}
    for o in env.objects:
        if o.type.name != "MonoBehaviour":
            continue
        h = parse_header(o.get_raw_data())
        if h is None:
            continue
        headers[o.path_id] = h
        groups[(h[1], h[2])].append(o.path_id)

    log("%s: MonoBehaviour %d 个，分成 %d 个脚本组" % (rel, len(headers), len(groups)))
    for k, v in sorted(groups.items(), key=lambda kv: -len(kv[1]))[:12]:
        log("   script(0x%X,%d) -> %d 个实例" % (k[0] & 0xFFFFFFFF, k[1], len(v)))

    # 每组取一个代表解析类型树
    by_id = {o.path_id: o for o in env.objects}
    wanted = {}
    for key, pids in groups.items():
        rep = by_id[pids[0]]
        try:
            tt = rep.read_typetree()
        except Exception:
            continue
        ks = set(tt.keys()) if isinstance(tt, dict) else set()
        if "uiStyleData" in ks:
            wanted["uiStyleData"] = (key, pids, tt)
        if "interactMarker" in ks:
            wanted["interactMarker"] = (key, pids, tt)
        if "displayQualityLooks" in ks:
            wanted["displayQualityLooks"] = (key, pids, tt)

    log("")
    log("命中: %s" % ", ".join(sorted(wanted.keys())))

    gd = wanted.get("uiStyleData")
    if gd:
        key, pids, tt = gd
        log("")
        log("=" * 74)
        log("GameplayDataSettings  (%d 个实例, script pid=%d)" % (len(pids), key[1]))
        log("=" * 74)
        log("顶层键: %s" % ", ".join(sorted(tt.keys())))
        us = tt.get("uiStyleData")
        if isinstance(us, dict):
            log("")
            log("uiStyleData 键: %s" % ", ".join(sorted(us.keys())))
            looks = us.get("displayQualityLooks")
            if looks is not None:
                log("")
                log("displayQualityLooks (%d 项):" % len(looks))
                for e in looks:
                    if isinstance(e, dict):
                        sc = e.get("shadowColor")
                        log("   quality=%-8s shadowOffset=%-8s innerGlow=%-6s color=%s"
                            % (e.get("quality"), e.get("shadowOffset"),
                               e.get("innerGlow"), fmt_color(sc) if sc is not None else "?"))
                    else:
                        log("   %r" % (e,))
            for k in ("defaultDisplayQualityShadowColor", "defaultDisplayQualityShadowOffset",
                      "defaultDIsplayQualityShadowInnerGlow"):
                if k in us:
                    v = us[k]
                    log("   %s = %s" % (k, fmt_color(v) if hasattr(v, "r") else v))
        # 顺便看 prefabs.interactMarker
        pf = tt.get("prefabs")
        if isinstance(pf, dict):
            log("")
            log("prefabs.interactMarker -> %r" % (pf.get("interactMarker"),))

    im = wanted.get("interactMarker")
    if im and not gd:
        key, pids, tt = im
        log("")
        log("PrefabsData (%d 个实例): interactMarker -> %r" % (len(pids), tt.get("interactMarker")))

    with open(os.path.join(OUT, "asset_probe4.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))


if __name__ == "__main__":
    try:
        main()
    except Exception:
        log(traceback.format_exc())
        with open(os.path.join(OUT, "asset_probe4.txt"), "w", encoding="utf-8") as f:
            f.write("\n".join(lines))
