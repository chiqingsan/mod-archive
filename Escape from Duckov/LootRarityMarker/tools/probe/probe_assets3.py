# -*- coding: utf-8 -*-
"""探测 v3：修正 v2 的两处错误。

v2 的错误：
  1) 用 GameObject.m_Father 建层级 —— 错，父子关系在 Transform.m_Father / m_Children 上，
     所以 InteractMarker 预制体的子物体一个都没遍历出来。
  2) 用 m_Script.read().m_ClassName 认 MonoBehaviour 的类 —— 外部队件（m_FileID=16777216）
     的脚本读不出类名，所以 GameplayDataSettings 没被找到。改用 m_Name 匹配。

要拿到：
  * InteractMarker 预制体的完整层级（showIfUsedObject=17575 / hideIfUsedObject=17349 是什么）
  * GameplayDataSettings.uiStyleData.displayQualityLooks 的**实际品质色数值**
"""
import os
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
    return "RGBA(%.4f, %.4f, %.4f, %.4f)  #%02X%02X%02X" % (
        r, g, b, a,
        max(0, min(255, int(round(r * 255)))),
        max(0, min(255, int(round(g * 255)))),
        max(0, min(255, int(round(b * 255)))))


class Assets:
    def __init__(self, rel, gen):
        self.rel = rel
        self.env = UnityPy.load(os.path.join(GAME, rel))
        self.env.typetree_generator = gen
        self.objs = list(self.env.objects)
        self.by_id = {o.path_id: o for o in self.objs}
        self._go = {}

    def go(self, pid):
        if pid not in self._go:
            o = self.by_id.get(pid)
            if o is None or o.type.name != "GameObject":
                self._go[pid] = None
            else:
                try:
                    self._go[pid] = o.read()
                except Exception:
                    self._go[pid] = None
        return self._go[pid]

    def transform_of(self, go):
        for cp in (go.m_Components or []):
            cid = cp.m_PathID if hasattr(cp, "m_PathID") else int(cp)
            co = self.by_id.get(cid)
            if co is not None and co.type.name == "Transform":
                try:
                    return co.read()
                except Exception:
                    return None
        return None

    def roots(self):
        """没有父 Transform 的 GameObject。"""
        out = []
        for o in self.objs:
            if o.type.name != "GameObject":
                continue
            g = self.go(o.path_id)
            if g is None:
                continue
            tr = self.transform_of(g)
            if tr is None:
                continue
            try:
                fid = tr.m_Father.m_PathID if tr.m_Father is not None else 0
            except Exception:
                fid = 0
            if not fid:
                out.append(o.path_id)
        return out


def comps_of(a, go, pid):
    out = []
    for cp in (go.m_Components or []):
        cid = cp.m_PathID if hasattr(cp, "m_PathID") else int(cp)
        co = a.by_id.get(cid)
        if co is None or co.type.name == "Transform":
            continue
        out.append((co.type.name, cid, co))
    return out


def dump(a, pid, depth=0, seen=None):
    if seen is None:
        seen = set()
    if pid in seen or depth > 12:
        return
    seen.add(pid)
    g = a.go(pid)
    if g is None:
        log("%s<pathID %d 不是 GameObject>" % ("  " * depth, pid))
        return
    pad = "  " * depth
    log("%s- %s (pathID=%d) active=%s" % (pad, g.m_Name, pid, getattr(g, "m_IsActive", "?")))
    for tname, cid, co in comps_of(a, g, pid):
        det = ""
        if tname in ("MeshRenderer", "SpriteRenderer", "SkinnedMeshRenderer",
                     "ParticleSystemRenderer"):
            try:
                cr = co.read()
                mats = []
                for mp in (getattr(cr, "m_Materials", None) or []):
                    mid = mp.m_PathID if hasattr(mp, "m_PathID") else int(mp)
                    mo = a.by_id.get(mid)
                    mats.append(getattr(mo.read(), "m_Name", "?") if mo else ("ext:%d" % mid))
                det = " enabled=%s  mats=[%s]" % (getattr(cr, "m_Enabled", "?"), ", ".join(mats))
            except Exception as e:
                det = " <err %s>" % e
        log("%s    .%s%s" % (pad, tname, det))
    tr = a.transform_of(g)
    if tr is not None:
        kids = []
        for ch in (getattr(tr, "m_Children", None) or []):
            cid = ch.m_PathID if hasattr(ch, "m_PathID") else int(ch)
            ctr = a.by_id.get(cid)
            if ctr is None:
                continue
            try:
                ct = ctr.read()
                gp = ct.m_GameObject.m_PathID if ct.m_GameObject is not None else 0
            except Exception:
                gp = 0
            if gp:
                kids.append(gp)
        for k in kids:
            dump(a, k, depth + 1, seen)


def probe_marker(gen):
    log("=" * 78)
    log("探测 A：InteractMarker 预制体（用 Transform 层级）")
    log("=" * 78)
    a = Assets("resources.assets", gen)
    log("  resources.assets: %d 对象" % len(a.objs))
    log("  根 GameObject 数: %d" % len(a.roots()))

    dump(a, 6907)
    log("")
    log("  --- showIfUsedObject = 17575 ---")
    dump(a, 17575)
    log("")
    log("  --- hideIfUsedObject = 17349 ---")
    dump(a, 17349)


def probe_named_mono(gen, names):
    log("")
    log("=" * 78)
    log("探测 B：MonoBehaviour（按 m_Name 找 ScriptableObject）")
    log("=" * 78)
    for rel in ("resources.assets", "globalgamemanagers.assets"):
        p = os.path.join(GAME, rel)
        if not os.path.isfile(p):
            continue
        a = Assets(rel, gen)
        for o in a.objs:
            if o.type.name != "MonoBehaviour":
                continue
            try:
                mb = o.read()
            except Exception:
                continue
            nm = getattr(mb, "m_Name", "") or ""
            if nm not in names:
                continue
            log("")
            log("  >>> '%s'  (%s pathID=%d)" % (nm, rel, o.path_id))
            try:
                tt = o.read_typetree()
            except Exception as e:
                log("      read_typetree 失败: %s" % e)
                continue
            if "uiStyleData" in tt:
                us = tt["uiStyleData"]
                log("      uiStyleData 类型: %s" % type(us).__name__)
                if isinstance(us, dict):
                    log("      键: %s" % ", ".join(sorted(us.keys())))
                    dump_quality(us)
            if "interactMarker" in tt:
                log("      interactMarker -> %r" % (tt["interactMarker"],))
            if "prefabs" in tt and isinstance(tt["prefabs"], dict):
                pm = tt["prefabs"].get("interactMarker")
                log("      prefabs.interactMarker -> %r" % (pm,))


def dump_quality(us):
    looks = us.get("displayQualityLooks")
    if looks is None:
        log("      !! uiStyleData 里没有 displayQualityLooks")
        return
    log("      displayQualityLooks (%d 项):" % len(looks))
    for e in looks:
        if not isinstance(e, dict):
            log("        %r" % (e,))
            continue
        sc = e.get("shadowColor")
        log("        quality=%-8s shadowOffset=%-8s innerGlow=%-6s shadowColor=%s"
            % (e.get("quality"), e.get("shadowOffset"), e.get("innerGlow"),
               fmt_color(sc) if sc is not None else "?"))
    for k in ("defaultDisplayQualityShadowColor", "defaultDisplayQualityShadowOffset",
              "defaultDIsplayQualityShadowInnerGlow"):
        if k in us:
            v = us[k]
            log("      %s = %s" % (k, fmt_color(v) if hasattr(v, "r") else v))


def main():
    gen = TypeTreeGenerator(UNITY_VER)
    gen.load_local_dll_folder(MANAGED)
    log("TypeTreeGenerator 载入 %d 个 dll" % len(gen.get_loaded_dll_names()))
    for fn in (lambda: probe_marker(gen),
               lambda: probe_named_mono(gen, {"GameplayDataSettings", "PrefabsData"})):
        try:
            fn()
        except Exception:
            log("!! 抛异常：")
            log(traceback.format_exc())
    with open(os.path.join(OUT, "asset_probe3.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % os.path.join(OUT, "asset_probe3.txt"))


if __name__ == "__main__":
    main()
