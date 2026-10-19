# -*- coding: utf-8 -*-
"""回答「能不能控制发光强度」需要先确认三件事：

1. BillBoardOverlay_Markers 的 pass 落在哪个渲染阶段、写到什么目标
   —— 决定 _Tint 超过 1 的值是「真的 HDR 发光」还是「会被直接裁掉的白」。
2. 游戏的 URP Asset 有没有开 HDR（m_SupportsHDR / m_ColorGradingMode）
   —— 没开 HDR 的话，>1 的颜色会被 clamp 到 1，调高亮度只会越来越白。
3. 全局 Volume 里有没有 Bloom，强度多少
   —— 有 Bloom，>1 的部分才会真的「发光扩散」；没有的话只能靠 alpha / 颜色本身。

只读。
"""
import os
import traceback

import UnityPy

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)

SHADER_TARGET = "BillBoardOverlay_Markers"
lines = []


def log(msg=""):
    print(msg)
    lines.append(str(msg))


def all_asset_files():
    files = []
    for root, _dirs, names in os.walk(GAME):
        for n in names:
            if n.endswith(".assets") or n.endswith(".asset") or n.endswith(".unity3d"):
                files.append(os.path.join(root, n))
    # resources.assets 优先，后面再扫其余的
    files.sort(key=lambda p: (0 if p.endswith("resources.assets") else 1, p))
    return files


def probe_shader(env, obj):
    tt = obj.read_typetree()
    parsed = tt.get("m_ParsedForm") or {}
    log("")
    log("=" * 78)
    log("1) shader %s" % SHADER_TARGET)
    log("=" * 78)

    subs = parsed.get("m_SubShaders") or []
    log("SubShader 数 = %d" % len(subs))
    for si, sub in enumerate(subs):
        log("")
        log("  SubShader[%d] 键: %s" % (si, sorted(sub.keys())))
        stags = sub.get("m_Tags") or {}
        log("  SubShader 标签: %s" % stags)
        passes = sub.get("m_Passes") or []
        log("  Pass 数 = %d" % len(passes))
        for pi, p in enumerate(passes):
            log("    Pass[%d] 键: %s" % (pi, sorted(p.keys())))
            log("      m_Name        = %s" % p.get("m_Name"))
            log("      m_Tags        = %s" % (p.get("m_Tags") or p.get("m_Tag")))
            log("      m_ProgramMask = %s" % p.get("m_ProgramMask"))
            state = p.get("m_State") or {}
            if isinstance(state, dict):
                log("      m_State       = %s" % {k: state[k] for k in sorted(state.keys())
                                                 if k in ("rtBlend0", "rtBlend1", "rtSeparateBlend",
                                                          "zWrite", "zTest", "culling", "rtColorMask",
                                                          "alphaToMask")})
            progs = p.get("m_Programs")
            if isinstance(progs, dict):
                log("      m_Programs 键 = %s" % sorted(progs.keys()))
                for k, v in sorted(progs.items()):
                    if isinstance(v, (str, bytes)):
                        n = len(v)
                        head = (v[:120] if isinstance(v, str) else v[:120]).replace("\n", "\\n")
                        log("         %s : %d 字节  头: %s" % (k, n, head))
                    else:
                        log("         %s : %s" % (k, type(v).__name__))

    # 编译好的平台变体里通常还能捞到关键字 / 常量
    plats = parsed.get("m_Platforms") or []
    log("")
    log("m_Platforms 数 = %d" % len(plats))
    for pl in plats[:3]:
        if isinstance(pl, dict):
            log("  platform 键: %s" % sorted(pl.keys()))
            blob = pl.get("m_Blob")
            if isinstance(blob, bytes):
                for kw in (b"_Tint", b"_Tint2", b"_EmissionColor", b"_IsInteractMarker", b"_Near", b"_Far"):
                    log("    blob 含 %s : %s" % (kw.decode(), kw in blob))
                # 编译后的 constant buffer 名字表里能看到哪些 uniform 真被用了
    log("m_ShaderIsBaked = %s" % parsed.get("m_ShaderIsBaked"))
    log("parsed 其它键 = %s" % [k for k in sorted(parsed.keys()) if k not in ("m_SubShaders", "m_Platforms", "m_PropInfo")])


def probe_urp(env, obj):
    tt = obj.read_typetree()
    log("")
    log("=" * 78)
    log("2) URP Asset: %s" % tt.get("m_Name"))
    log("=" * 78)
    interesting = [k for k in sorted(tt.keys())
                   if any(s in k.lower() for s in ("hdr", "color", "grade", "post", "msaa",
                                                   "renderer", "shadow", "srp", "lut", "scale"))]
    for k in interesting:
        v = tt[k]
        if isinstance(v, dict):
            v = {kk: v[kk] for kk in sorted(v.keys())[:8]}
        log("   %-34s = %s" % (k, v))


def probe_bloom(env, obj, name):
    try:
        tt = obj.read_typetree()
    except Exception:
        return False
    if "m_Components" not in tt:
        return False
    comps = tt.get("m_Components") or []
    found = []
    for c in comps:
        try:
            co = c.read() if hasattr(c, "read") else None
        except Exception:
            co = None
        if co is None and hasattr(c, "m_PathID"):
            co = env.objects_by_pathid.get(c.m_PathID) if hasattr(env, "objects_by_pathid") else None
        # 直接按 typetree 判断组件类型更稳
        try:
            ctt = c.read_typetree()
        except Exception:
            ctt = None
        if ctt is None:
            continue
        if "m_Intensity" in ctt and ("m_Threshold" in ctt or "m_Scatter" in ctt or "m_Tint" in ctt):
            found.append(ctt)
        elif "m_Active" in ctt and "m_Intensity" in ctt:
            found.append(ctt)
    if not found:
        return False
    log("")
    log("=" * 78)
    log("3) VolumeProfile '%s'  组件 %d 个" % (name, len(comps)))
    log("=" * 78)
    log("  该 profile 全部组件的关键字段：")
    for ctt in found:
        log("   - m_Active=%s m_Intensity=%s m_Threshold=%s m_Scatter=%s m_Tint=%s m_Color=%s" % (
            ctt.get("m_Active"), ctt.get("m_Intensity"), ctt.get("m_Threshold"),
            ctt.get("m_Scatter"), ctt.get("m_Tint"), ctt.get("m_Color")))
    return True


def main():
    shader_done = urp_done = bloom_hits = 0
    for path in all_asset_files():
        rel = os.path.relpath(path, GAME)
        try:
            env = UnityPy.load(path)
        except Exception:
            continue
        for o in env.objects:
            tn = o.type.name
            try:
                if tn == "Shader" and not shader_done:
                    tt = o.read_typetree()
                    if (tt.get("m_ParsedForm") or {}).get("m_Name") == SHADER_TARGET:
                        probe_shader(env, o)
                        shader_done = 1
                elif tn == "MonoBehaviour":
                    # URP Asset / VolumeProfile 都是 MonoBehaviour，靠字段名认
                    raw = o.get_raw_data()
                    if b"m_SupportsHDR" in raw and not urp_done:
                        probe_urp(env, o)
                        urp_done = 1
                    elif b"m_Scatter" in raw and b"m_Threshold" in raw:
                        if probe_bloom(env, o, o.read_typetree().get("m_Name") or "<unnamed>"):
                            bloom_hits += 1
                            if bloom_hits >= 4:
                                bloom_hits = 100  # 够了
            except Exception:
                continue
        if shader_done and urp_done and bloom_hits >= 100:
            log("")
            log("（已收集齐，提前结束；当前文件 %s）" % rel)
            break

    log("")
    log("汇总：shader=%s urp=%s bloom命中=%s" % (bool(shader_done), bool(urp_done), bloom_hits))

    with open(os.path.join(OUT, "glow_probe.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % os.path.join(OUT, "glow_probe.txt"))


if __name__ == "__main__":
    try:
        main()
    except Exception:
        log(traceback.format_exc())
        with open(os.path.join(OUT, "glow_probe.txt"), "w", encoding="utf-8") as f:
            f.write("\n".join(lines))
