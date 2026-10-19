# -*- coding: utf-8 -*-
"""探测：URP 的 HDR 开关 + 有没有 Bloom（决定「调高 _Tint 亮度」到底是发光还是被裁白）。

为什么用「按 m_Script 分组 + 每组取一个代表」：
resources.assets 有 38606 个 MonoBehaviour，逐个 read_typetree() 要 ~70 分钟。
同一 C# 类在同一文件里的实例共享同一个 m_Script 引用，所以先手工解析固定头部
（不需要类型树）拿到 m_Script.m_PathID，分组后每组只读一个代表即可。

要回答的问题：
  Q1  URP Asset 的 m_SupportsHDR 是不是 true？
      false → 渲染目标是 LDR，_Tint 超过 1 的部分会被 clamp 到 1，调亮度只会越来越白。
  Q2  场景里的 Volume / VolumeProfile 有没有挂 Bloom？强度多少？
      有 → _Tint 超过 1 的部分会真的扩散成光晕，亮度旋钮才有「发光强度」的语义。
  Q3  Bloom 的 m_Threshold 是多少？（低于阈值的亮度不会触发泛光）
只读。
"""
import os
import struct
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

# 要扫的容器文件：URP Asset / VolumeProfile 可能在任意一个里
SCAN = ["globalgamemanagers.assets", "resources.assets"]
for n in sorted(os.listdir(GAME)):
    p = os.path.join(GAME, n)
    if os.path.isfile(p) and (n.startswith("level")) and not n.endswith(".resS"):
        SCAN.append(n)

lines = []


def log(m=""):
    print(m)
    lines.append(str(m))


def parse_header(raw):
    if len(raw) < 32:
        return None
    try:
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


def show(d, keys):
    return {k: d.get(k) for k in keys if k in d}


def main():
    gen = TypeTreeGenerator(UNITY_VER)
    gen.load_local_dll_folder(MANAGED)
    log("TypeTreeGenerator 载入 %d 个 dll" % len(gen.get_loaded_dll_names()))

    urp_hits = []       # (file, pathid, typetree)
    bloom_hits = []     # (file, pathid, typetree)
    volume_hits = []    # (file, pathid, typetree)

    URP_KEYS = ("m_SupportsHDR", "m_ColorGradingMode", "m_ColorGradingLut",
                "m_MSAA", "m_RenderScale", "m_ShadowDistance", "m_MainLightRenderingMode")
    BLOOM_KEYS = ("m_Active", "m_Intensity", "m_Threshold", "m_Scatter", "m_Tint",
                  "m_HighQualityFiltering", "m_DirtIntensity")
    VOL_KEYS = ("m_IsGlobal", "m_Priority", "m_BlendDistance", "m_Weight", "m_SharedProfile")

    for rel in SCAN:
        path = os.path.join(GAME, rel)
        if not os.path.exists(path):
            continue
        try:
            env = UnityPy.load(path)
        except Exception:
            continue
        env.typetree_generator = gen

        groups = collections.defaultdict(list)
        by_id = {}
        for o in env.objects:
            if o.type.name != "MonoBehaviour":
                continue
            by_id[o.path_id] = o
            h = parse_header(o.get_raw_data())
            if h is None:
                continue
            groups[(h[1], h[2])].append((o.path_id, h[3]))

        n_new = 0
        for key, members in groups.items():
            rep_pid = members[0][0]
            rep = by_id.get(rep_pid)
            if rep is None:
                continue
            try:
                tt = rep.read_typetree()
            except Exception:
                continue
            if not isinstance(tt, dict):
                continue
            ks = set(tt.keys())
            if "m_SupportsHDR" in ks:
                urp_hits.append((rel, rep_pid, tt, len(members)))
                n_new += 1
            elif "m_Scatter" in ks and "m_Threshold" in ks:
                bloom_hits.append((rel, rep_pid, tt, len(members)))
                n_new += 1
            elif "m_IsGlobal" in ks and "m_Priority" in ks:
                volume_hits.append((rel, rep_pid, tt, len(members)))
                n_new += 1
        if n_new:
            log("  %-26s 命中新组 %d 个" % (rel, n_new))

    log("")
    log("=" * 78)
    log("Q1) URP Render Pipeline Asset —— HDR 开关")
    log("=" * 78)
    if not urp_hits:
        log("  ！没找到带 m_SupportsHDR 的 MonoBehaviour（可能在别处或有嵌套结构）")
    for rel, pid, tt, n in urp_hits:
        log("  文件 %s  pathID %d  该脚本共 %d 个实例" % (rel, pid, n))
        for k in URP_KEYS:
            if k in tt:
                log("     %-26s = %s" % (k, tt[k]))
        log("     全部键: %s" % ", ".join(sorted(tt.keys())))

    log("")
    log("=" * 78)
    log("Q2/Q3) Bloom 组件")
    log("=" * 78)
    if not bloom_hits:
        log("  ！没找到 Bloom 组件（说明游戏没有启用 URP Bloom 后处理）")
    for rel, pid, tt, n in bloom_hits:
        log("  文件 %s  pathID %d  该脚本共 %d 个实例" % (rel, pid, n))
        log("     %s" % show(tt, BLOOM_KEYS))

    log("")
    log("=" * 78)
    log("附带) Volume 组件")
    log("=" * 78)
    if not volume_hits:
        log("  ！没找到 Volume 组件")
    for rel, pid, tt, n in volume_hits:
        log("  文件 %s  pathID %d  共 %d 个 Volume；%s" % (rel, pid, n, show(tt, VOL_KEYS)))

    with open(os.path.join(OUT, "glow_hdr_probe.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % os.path.join(OUT, "glow_hdr_probe.txt"))


if __name__ == "__main__":
    try:
        main()
    except Exception:
        log(traceback.format_exc())
        with open(os.path.join(OUT, "glow_hdr_probe.txt"), "w", encoding="utf-8") as f:
            f.write("\n".join(lines))
