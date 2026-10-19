# -*- coding: utf-8 -*-
"""从逃离鸭科夫的发行资产里挖出 InteractMarker 预制体的渲染结构 + 材质真实属性名。

目的（对应 PLAN.md §6.7 的未确认项 #1/#2/#5）：
  * 交互点下面到底有哪些 Renderer（MeshRenderer / SpriteRenderer / ParticleSystemRenderer）
  * 它们用的是哪个 shader
  * 材质的 m_SavedProperties 里到底有哪几个颜色属性名（_Color? _Tint? _BaseColor?）
  * 顺带把 GameplayDataSettings 的 displayQualityLooks 品质色试出来

只读，不改任何游戏文件。
"""
import os
import sys
import json
import traceback

import UnityPy

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)

RENDERER_TYPES = {
    "MeshRenderer", "SkinnedMeshRenderer", "SpriteRenderer",
    "ParticleSystemRenderer", "TrailRenderer", "LineRenderer",
    "CanvasRenderer",
}

lines = []


def log(msg=""):
    print(msg)
    lines.append(str(msg))


def open_env(rel):
    p = os.path.join(GAME, rel)
    if not os.path.isfile(p):
        return None
    try:
        return UnityPy.load(p)
    except Exception as e:
        log("  !! 打不开 %s: %s" % (rel, e))
        return None


def dump_material(obj, indent="      "):
    """打印一个 Material 的全部已保存属性名 —— 这是本次探测的核心产出。"""
    try:
        mat = obj.read()
    except Exception as e:
        log("%s<Material 读取失败 %s>" % (indent, e))
        return None
    name = getattr(mat, "m_Name", "?")
    shader_name = "?"
    try:
        sp = mat.m_Shader
        sh = sp.read() if hasattr(sp, "read") else None
        if sh is not None:
            shader_name = getattr(sh, "m_ParsedForm", None)
            shader_name = getattr(shader_name, "m_Name", None) or getattr(sh, "m_Name", "?")
    except Exception:
        pass

    log("%sMaterial '%s'  shader=%s" % (indent, name, shader_name))
    try:
        saved = mat.m_SavedProperties
    except Exception:
        log("%s  (无 m_SavedProperties)" % indent)
        return name

    def dump_group(label, items, fmt):
        if not items:
            return
        log("%s  %s:" % (indent, label))
        for it in items:
            try:
                k, v = it[0], it[1]
                log("%s    %-28s %s" % (indent, k, fmt(v)))
            except Exception:
                log("%s    %r" % (indent, it))

    dump_group("Colors(m_Colors)", getattr(saved, "m_Colors", None),
               lambda c: "RGBA(%s, %s, %s, %s)" % (
                   getattr(c, "r", "?"), getattr(c, "g", "?"),
                   getattr(c, "b", "?"), getattr(c, "a", "?")))
    dump_group("Floats(m_Floats)", getattr(saved, "m_Floats", None), lambda f: str(f))
    dump_group("TexEnvs(m_TexEnvs)", getattr(saved, "m_TexEnvs", None), lambda t: "<tex>")
    dump_group("Ints(m_Ints)", getattr(saved, "m_Ints", None), lambda i: str(i))
    return name


def walk_tree(by_id, go_obj, depth=0, seen=None):
    """递归打印一个 GameObject 的层级与组件。"""
    if seen is None:
        seen = set()
    if go_obj.path_id in seen or depth > 8:
        return
    seen.add(go_obj.path_id)

    try:
        go = go_obj.read()
    except Exception as e:
        log("%s<GameObject 读取失败 %s>" % ("  " * depth, e))
        return

    pad = "  " * depth
    log("%s- %s  (pathID=%d)" % (pad, go.m_Name, go_obj.path_id))

    try:
        comps = go.m_Components
    except Exception:
        return

    for cp in comps:
        try:
            cid = cp.m_PathID if hasattr(cp, "m_PathID") else int(cp)
            cobj = by_id.get(cid)
            if cobj is None:
                continue
            ctype = cobj.type.name
            if ctype in RENDERER_TYPES:
                cr = cobj.read()
                log("%s  [%s] enabled=%s" % (pad, ctype, getattr(cr, "m_Enabled", "?")))
                mats = getattr(cr, "m_Materials", None)
                if mats:
                    for mp in mats:
                        mid = mp.m_PathID if hasattr(mp, "m_PathID") else int(mp)
                        mobj = by_id.get(mid)
                        if mobj is None:
                            log("%s    -> material pathID=%s (外部引用)" % (pad, mid))
                        else:
                            dump_material(mobj, pad + "    ")
            elif ctype in ("MonoBehaviour", "Transform"):
                pass
            else:
                log("%s  [%s]" % (pad, ctype))
        except Exception as e:
            log("%s  <组件读取失败: %s>" % (pad, e))

    # 子物体
    try:
        children = go.m_Children
    except Exception:
        children = []
    for ch in children or []:
        cid = ch.m_PathID if hasattr(ch, "m_PathID") else int(ch)
        cobj = by_id.get(cid)
        if cobj is not None:
            walk_tree(by_id, cobj, depth + 1, seen)


def probe_interact_marker():
    log("=" * 78)
    log("探测 1：InteractMarker 预制体（resources.assets）")
    log("=" * 78)
    env = open_env("resources.assets")
    if env is None:
        return
    objs = list(env.objects)
    by_id = {o.path_id: o for o in objs}
    log("  resources.assets: %d 个对象" % len(objs))

    # 1a) 名字里带 InteractMarker 的 GameObject
    roots = []
    for o in objs:
        if o.type.name != "GameObject":
            continue
        try:
            go = o.read()
        except Exception:
            continue
        if go.m_Name and "InteractMarker" in go.m_Name:
            try:
                is_root = go.m_Father is None or int(go.m_Father.m_PathID) == 0
            except Exception:
                is_root = True
            roots.append((o, go.m_Name, is_root))

    if not roots:
        log("  !! 没找到 InteractMarker GameObject")
    for o, nm, is_root in roots:
        log("")
        log("  >>> GameObject '%s' root=%s pathID=%d" % (nm, is_root, o.path_id))
        walk_tree(by_id, o, 1)

    # 1b) 名字里带 InteractMarker 的 Material（可能是独立资产）
    log("")
    log("  --- 名字含 'InteractMarker' 的 Material 资产 ---")
    n = 0
    for o in objs:
        if o.type.name != "Material":
            continue
        try:
            mat = o.read()
        except Exception:
            continue
        nm = getattr(mat, "m_Name", "") or ""
        if "InteractMarker" in nm:
            n += 1
            dump_material(o, "    ")
    log("  共 %d 个" % n)


def probe_settings():
    log("")
    log("=" * 78)
    log("探测 2：GameplayDataSettings 的 displayQualityLooks 品质色")
    log("=" * 78)
    for rel in ("resources.assets", "globalgamemanagers.assets", "sharedassets0.assets"):
        env = open_env(rel)
        if env is None:
            continue
        found = False
        for o in env.objects:
            if o.type.name != "MonoBehaviour":
                continue
            try:
                raw = o.get_raw_data()
            except Exception:
                continue
            if b"displayQualityLooks" not in raw:
                continue
            found = True
            log("  命中 %s pathID=%d (%d bytes)" % (rel, o.path_id, len(raw)))
            # 无 typetree 时，手工扫 Color 结构：Unity 序列化里 Color 是 4 个 float。
            # displayQualityLooks 是 List<DisplayQualityLook>，每项 = int quality,
            # float shadowOffset, Color shadowColor, bool innerGlow。
            # 用启发式：在 "displayQualityLooks" 之后按 4 字节对齐扫描连续的
            # [int 0..8][float][float x4][bool] 模式。
            parse_quality_looks(raw)
        if found:
            break
    else:
        log("  !! 没有在任何资产里找到 displayQualityLooks")


def parse_quality_looks(raw):
    import struct
    idx = raw.find(b"displayQualityLooks")
    if idx < 0:
        return
    log("    'displayQualityLooks' 字节偏移 = %d" % idx)
    # 打印该字符串后面 400 字节的十六进制，便于人工/后续精确解析
    seg = raw[idx: idx + 400]
    log("    hexdump:")
    for i in range(0, len(seg), 16):
        chunk = seg[i:i + 16]
        hexs = " ".join("%02x" % b for b in chunk)
        asc = "".join(chr(b) if 32 <= b < 127 else "." for b in chunk)
        log("      %04x  %-47s  %s" % (i, hexs, asc))

    # 启发式解析：在字符串之后按 4 字节对齐找 quality(0..8) + float + 4*float 的组合
    body = raw[idx + len(b"displayQualityLooks"):]
    log("    启发式扫描（quality 0..6 + shadowOffset + RGBA）:")
    for off in range(0, min(len(body) - 24, 200), 4):
        try:
            q = struct.unpack_from("<i", body, off)[0]
        except Exception:
            continue
        if q < 1 or q > 6:
            continue
        try:
            shadow_off = struct.unpack_from("<f", body, off + 4)[0]
            c = struct.unpack_from("<4f", body, off + 8)
        except Exception:
            continue
        if not (0.0 <= shadow_off <= 200.0):
            continue
        if not all(-0.01 <= v <= 1.01 for v in c):
            continue
        log("      off=%3d  quality=%d shadowOffset=%.2f  color=(%.3f, %.3f, %.3f, %.3f)  #%02X%02X%02X"
            % (off, q, shadow_off, c[0], c[1], c[2], c[3],
               max(0, min(255, int(c[0] * 255))),
               max(0, min(255, int(c[1] * 255))),
               max(0, min(255, int(c[2] * 255)))))


def main():
    for fn in (probe_interact_marker, probe_settings):
        try:
            fn()
        except Exception:
            log("!! %s 抛异常：" % fn.__name__)
            log(traceback.format_exc())
    with open(os.path.join(OUT, "asset_probe.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % os.path.join(OUT, "asset_probe.txt"))


if __name__ == "__main__":
    main()
