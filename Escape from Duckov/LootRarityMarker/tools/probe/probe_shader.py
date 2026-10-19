# -*- coding: utf-8 -*-
"""确认 InteractMarker 用的 shader (BillBoardOverlay_Markers) 到底声明了哪些属性。

为什么必须查这个：MaterialPropertyBlock 只能覆盖「shader 真正声明了的属性」。
如果 shader 里没有 _Tint，那么无论我们往 MPB 里写什么，渲染结果都不会变，
路线 B（染交互点）就会静默失效。这是整个 mod 唯一一个「靠反编译源码无法确认」的点。

只读，不改任何游戏文件。
"""
import os
import struct
import traceback

import UnityPy

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)

TARGET = "BillBoardOverlay_Markers"
lines = []


def log(msg=""):
    print(msg)
    lines.append(str(msg))


def main():
    assets = sorted(
        f for f in os.listdir(GAME)
        if f.endswith(".assets") or f.startswith("level")
    )
    log("扫描 %d 个资产文件，找 shader '%s'" % (len(assets), TARGET))

    hits = 0
    for rel in assets:
        path = os.path.join(GAME, rel)
        if not os.path.isfile(path):
            continue
        try:
            env = UnityPy.load(path)
        except Exception:
            continue

        for o in env.objects:
            if o.type.name != "Shader":
                continue
            try:
                sh = o.read_typetree()
            except Exception:
                continue
            parsed = sh.get("m_ParsedForm")
            if not parsed:
                continue
            name = parsed.get("m_Name") or sh.get("m_Name")
            if name != TARGET:
                continue

            hits += 1
            log("")
            log("=" * 78)
            log("命中：%s  pathID=%d  shader=%s" % (rel, o.path_id, name))
            log("=" * 78)

            # ---- 属性表（MPB 能覆盖的正是这些）----
            prop_info = parsed.get("m_PropInfo") or {}
            props = prop_info.get("m_Props") or []
            log("m_PropInfo 属性数 = %d" % len(props))
            names = []
            for p in props:
                pname = p.get("m_Name")
                ptype = p.get("m_Type")
                pflags = p.get("m_Flags")
                names.append(pname)
                log("   %-28s type=%-3s flags=%s" % (pname, ptype, pflags))

            for want in ("_Tint", "_Tint2", "_Color", "_BaseColor", "_EmissionColor", "_EdgeColor"):
                log("   [%s] %s" % ("有" if want in names else "无", want))

            # ---- 其它可能藏名字的地方 ----
            log("")
            log("m_SubShaders 数量 = %d" % len(parsed.get("m_SubShaders") or []))
            log("m_ShaderIsBaked = %s" % parsed.get("m_ShaderIsBaked"))
            for key in ("m_Name", "m_ShaderIsBaked", "m_Dependencies"):
                if key in parsed:
                    log("  parsed.%-18s = %s" % (key, str(parsed[key])[:200]))

            # ---- 属性默认值（DefValue）也顺便看看，能反映作者的意图 ----
            log("")
            log("m_PropInfo.m_DefValue 长度 = %d (每项 8 个 float?)" % len(prop_info.get("m_DefValue") or []))
            try:
                tex_envs = prop_info.get("m_DefTexture") or []
                for i, te in enumerate(tex_envs):
                    log("   tex[%d] name=%s" % (i, te.get("m_Name") if isinstance(te, dict) else te))
            except Exception:
                pass
            break

        if hits:
            break

    if not hits:
        log("")
        log("!! 没有在任何资产的 Shader 对象里找到 '%s'。" % TARGET)
        log("   可能是：shader 被剥离了 ParsedForm（只留编译后的变体），或者它在别的位置。")
        log("   退而求其次：直接在原始字节里找属性名字符串。")
        for rel in assets:
            path = os.path.join(GAME, rel)
            if not os.path.isfile(path):
                continue
            try:
                with open(path, "rb") as f:
                    blob = f.read()
            except Exception:
                continue
            if TARGET.encode() not in blob:
                continue
            log("   %s 含 '%s' 字符串 (%d bytes)" % (rel, TARGET, len(blob)))
            i = blob.find(TARGET.encode())
            seg = blob[max(0, i - 200): i + 2000]
            found = [n for n in (b"_Tint", b"_Tint2", b"_Color", b"_BaseColor", b"_EmissionColor") if n in seg]
            log("     附近 2000 字节内出现的属性名: %s" % [n.decode() for n in found])

    with open(os.path.join(OUT, "shader_probe.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % os.path.join(OUT, "shader_probe.txt"))


if __name__ == "__main__":
    try:
        main()
    except Exception:
        log(traceback.format_exc())
        with open(os.path.join(OUT, "shader_probe.txt"), "w", encoding="utf-8") as f:
            f.write("\n".join(lines))
