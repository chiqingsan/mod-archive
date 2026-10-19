# -*- coding: utf-8 -*-
"""探测 v2：用 TypeTreeGeneratorAPI 从 DLL 生成类型树，读出 MonoBehaviour 的真实字段。

要关掉的未确认项：
  * InteractMarker 预制体到底长什么样（层级 + 组件 + MonoBehaviour 的 showIfUsedObject/hideIfUsedObject 指向）
  * GameplayDataSettings.displayQualityLooks 的**实际品质色数值**（代码里读不到，只在资产里）
  * PrefabsData.interactMarker 指向哪个预制体
"""
import os
import sys
import json
import traceback

import UnityPy
from UnityPy.helpers.TypeTreeGenerator import TypeTreeGenerator

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
MANAGED = os.path.join(GAME, "Managed")
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)

UNITY_VER = "2022.3.62f2"

lines = []


def log(msg=""):
    print(msg)
    lines.append(str(msg))


def fmt_color(c):
    try:
        r, g, b, a = c.r, c.g, c.b, c.a
    except Exception:
        return repr(c)
    hexs = "#%02X%02X%02X" % (
        max(0, min(255, int(round(r * 255)))),
        max(0, min(255, int(round(g * 255)))),
        max(0, min(255, int(round(b * 255)))),
    )
    return "RGBA(%.4f, %.4f, %.4f, %.4f) %s" % (r, g, b, a, hexs)


def build_gen():
    gen = TypeTreeGenerator(UNITY_VER)
    gen.load_local_dll_folder(MANAGED)
    log("TypeTreeGenerator: 载入 %d 个 dll" % len(gen.get_loaded_dll_names()))
    return gen


def script_class_cache(env, obj, cache):
    """拿到 MonoBehaviour 的脚本类名（只用固定头部，不需要类型树）。"""
    try:
        mb = obj.read()
    except Exception:
        return None, None
    try:
        sp = mb.m_Script
        pid = sp.m_PathID
        if pid in cache:
            return cache[pid], mb
        sc = sp.read()
        cache[pid] = getattr(sc, "m_ClassName", None)
        return cache[pid], mb
    except Exception:
        return None, mb


def tree(go_obj, by_id, depth=0, seen=None):
    if seen is None:
        seen = set()
    if go_obj.path_id in seen or depth > 10:
        return
    seen.add(go_obj.path_id)
    try:
        go = go_obj.read()
    except Exception as e:
        log("%s<读取失败 %s>" % ("  " * depth, e))
        return
    pad = "  " * depth
    info = []
    for ch in (go.m_Children or []):
        cid = ch.m_PathID if hasattr(ch, "m_PathID") else int(ch)
        co = by_id.get(cid)
        if co is not None:
            info.append(co)
    # 组件
    types = []
    for cp in (go.m_Components or []):
        cid = cp.m_PathID if hasattr(cp, "m_PathID") else int(cp)
        co = by_id.get(cid)
        if co is None:
            continue
        t = co.type.name
        if t == "Transform":
            continue
        types.append(t)
        if t in ("MeshRenderer", "SpriteRenderer", "SkinnedMeshRenderer",
                 "ParticleSystemRenderer", "MeshFilter"):
            try:
                cr = co.read()
                mats = getattr(cr, "m_Materials", None)
                mnames = []
                if mats:
                    for mp in mats:
                        mid = mp.m_PathID if hasattr(mp, "m_PathID") else int(mp)
                        mo = by_id.get(mid)
                        mnames.append(getattr(mo.read(), "m_Name", "?") if mo else ("ext:%d" % mid))
                extra = (" mats=" + ",".join(mnames)) if mnames else ""
                en = getattr(cr, "m_Enabled", None)
                log("%s- %s [%s enabled=%s]%s" % (pad, go.m_Name, t, en, extra))
                continue
            except Exception:
                pass
    log("%s- %s  {%s}  activeSelf=%s" % (pad, go.m_Name, ", ".join(types), getattr(go, "m_IsActive", "?")))
    for co in info:
        tree(co, by_id, depth + 1, seen)


def probe_marker_prefab(gen):
    log("=" * 78)
    log("探测 A：InteractMarker 预制体完整层级")
    log("=" * 78)
    env = UnityPy.load(os.path.join(GAME, "resources.assets"))
    env.typetree_generator = gen
    objs = list(env.objects)
    by_id = {o.path_id: o for o in objs}

    # 找出所有 GameObject，用 m_Father 建父->子索引
    gos = {}
    for o in objs:
        if o.type.name == "GameObject":
            try:
                gos[o.path_id] = o.read()
            except Exception:
                pass
    children = {}
    for pid, go in gos.items():
        try:
            f = go.m_Father
            fid = f.m_PathID if f is not None else 0
        except Exception:
            fid = 0
        if fid:
            children.setdefault(fid, []).append(pid)

    # 找名字为 InteractMarker 的 GameObject
    targets = [pid for pid, go in gos.items() if go.m_Name == "InteractMarker"]
    log("名为 'InteractMarker' 的 GameObject: %s" % targets)

    def dump(pid, depth=0, seen=None):
        if seen is None:
            seen = set()
        if pid in seen or depth > 10:
            return
        seen.add(pid)
        go = gos[pid]
        pad = "  " * depth
        comps = []
        for cp in (go.m_Components or []):
            cid = cp.m_PathID if hasattr(cp, "m_PathID") else int(cp)
            co = by_id.get(cid)
            if co is None or co.type.name == "Transform":
                continue
            t = co.type.name
            det = ""
            if t in ("MeshRenderer", "SpriteRenderer", "SkinnedMeshRenderer", "ParticleSystemRenderer"):
                try:
                    cr = co.read()
                    mats = []
                    for mp in (getattr(cr, "m_Materials", None) or []):
                        mid = mp.m_PathID if hasattr(mp, "m_PathID") else int(mp)
                        mo = by_id.get(mid)
                        mats.append(getattr(mo.read(), "m_Name", "?") if mo else "ext")
                    det = " enabled=%s mats=[%s]" % (getattr(cr, "m_Enabled", "?"), ", ".join(mats))
                except Exception as e:
                    det = " <err %s>" % e
            comps.append(t + det)
        log("%s- %s (pathID=%d) active=%s" % (pad, go.m_Name, pid, getattr(go, "m_IsActive", "?")))
        for c in comps:
            log("%s    .%s" % (pad, c))
        for ch in children.get(pid, []):
            dump(ch, depth + 1, seen)

    for t in targets:
        dump(t)
        log("")
        # 打印该预制体上的 MonoBehaviour 字段（关键：showIfUsedObject / hideIfUsedObject）
        go = gos[t]
        for cp in (go.m_Components or []):
            cid = cp.m_PathID if hasattr(cp, "m_PathID") else int(cp)
            co = by_id.get(cid)
            if co is None or co.type.name != "MonoBehaviour":
                continue
            log("  --- MonoBehaviour pathID=%d ---" % cid)
            try:
                tt = co.read_typetree()
                log(json.dumps(tt, indent=2, ensure_ascii=False, default=str)[:3000])
            except Exception as e:
                log("    类型树读取失败: %s" % e)
                log("    raw head: %s" % co.get_raw_data()[:120].hex())


def find_mono_by_field(gen, assets_rel, key_fields, max_scan=None):
    """在所有 MonoBehaviour 里找带指定字段名的那个（用类名匹配更省事）。"""
    env = UnityPy.load(os.path.join(GAME, assets_rel))
    env.typetree_generator = gen
    objs = list(env.objects)
    cache = {}
    hits = []
    n = 0
    for o in objs:
        if o.type.name != "MonoBehaviour":
            continue
        n += 1
        if max_scan and n > max_scan:
            break
        cls, mb = script_class_cache(env, o, cache)
        if cls is None:
            continue
        if cls in key_fields:
            hits.append((cls, o, mb))
    return hits


CLASSES = {"GameplayDataSettings", "PrefabsData", "InteractMarker"}


def probe_settings(gen):
    log("")
    log("=" * 78)
    log("探测 B：GameplayDataSettings 的品质色表 + PrefabsData.interactMarker")
    log("=" * 78)
    for rel in ("resources.assets", "globalgamemanagers.assets"):
        p = os.path.join(GAME, rel)
        if not os.path.isfile(p):
            continue
        env = UnityPy.load(p)
        env.typetree_generator = gen
        cache = {}
        for o in env.objects:
            if o.type.name != "MonoBehaviour":
                continue
            cls, mb = script_class_cache(env, o, cache)
            if cls not in CLASSES:
                continue
            log("")
            log("  >>> %s  (%s pathID=%d)" % (cls, rel, o.path_id))
            try:
                tt = o.read_typetree()
            except Exception as e:
                log("      类型树读取失败: %s" % e)
                continue
            if cls == "GameplayDataSettings":
                dump_ui_style(tt)
            elif cls == "PrefabsData":
                v = tt.get("interactMarker")
                log("      interactMarker -> %r" % (v,))
                log("      (PrefabsData 全部键: %s)" % ", ".join(sorted(tt.keys()))[:1500])
            elif cls == "InteractMarker":
                log(json.dumps(tt, indent=2, ensure_ascii=False, default=str)[:1500])


def dump_ui_style(tt):
    us = tt.get("uiStyleData")
    log("      uiStyleData keys: %s" % (", ".join(sorted(us.keys())) if isinstance(us, dict) else type(us)))
    if not isinstance(us, dict):
        return
    looks = us.get("displayQualityLooks")
    if looks is None:
        log("      !! 没有 displayQualityLooks")
        return
    log("      displayQualityLooks  (%d 项):" % len(looks))
    for e in looks:
        if not isinstance(e, dict):
            log("        %r" % (e,))
            continue
        q = e.get("quality")
        sc = e.get("shadowColor")
        so = e.get("shadowOffset")
        ig = e.get("innerGlow")
        log("        quality=%-8s shadowOffset=%-6s innerGlow=%-5s shadowColor=%s"
            % (q, so, ig, fmt_color(sc) if sc is not None else "?"))
    for k in ("defaultDisplayQualityShadowColor", "defaultDisplayQualityShadowOffset",
              "defaultDIsplayQualityShadowInnerGlow"):
        if k in us:
            v = us[k]
            log("      %s = %s" % (k, fmt_color(v) if hasattr(v, "r") else v))


def main():
    gen = build_gen()
    for fn in (probe_marker_prefab, probe_settings):
        try:
            fn(gen)
        except Exception:
            log("!! %s 抛异常：" % fn.__name__)
            log(traceback.format_exc())
    with open(os.path.join(OUT, "asset_probe2.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % os.path.join(OUT, "asset_probe2.txt"))


if __name__ == "__main__":
    main()
