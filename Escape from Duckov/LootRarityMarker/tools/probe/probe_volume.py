# -*- coding: utf-8 -*-
"""探测：游戏里的 Volume / VolumeProfile 到底挂了哪些后处理组件，有没有 Bloom。

为什么重做：上一版 probe_hdr_bloom.py 用 UnityPy.load(path) 直接读没有扩展名的
level* 文件，很可能被 UnityPy 判成未知格式而静默跳过（异常被 except 吃掉），
所以「没找到 Volume 组件」这个结论不可靠。

本版：
  1. 直接遍历所有顶层容器对象（SerializedFile），不依赖扩展名判断；
  2. 找出所有 VolumeProfile（带 m_Components 的 MonoBehaviour），把每个组件
     解析出来，打印它的字段名 —— 有 Bloom 就会看到 m_Scatter / m_Threshold；
  3. 顺便统计全局是否真的存在任何 m_Scatter 字段的对象。
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

TARGETS = ["globalgamemanagers.assets", "resources.assets"]
for n in sorted(os.listdir(GAME)):
    if n.startswith("level") and not n.endswith(".resS") and os.path.isfile(os.path.join(GAME, n)):
        TARGETS.append(n)

lines = []


def log(m=""):
    print(m)
    lines.append(str(m))


def parse_header(raw):
    if len(raw) < 32:
        return None
    try:
        s_pid = struct.unpack_from("<q", raw, 20)[0]
        nlen = struct.unpack_from("<i", raw, 28)[0]
        name = ""
        if 0 <= nlen <= 512 and 32 + nlen <= len(raw):
            name = raw[32:32 + nlen].decode("utf-8", "replace")
        return s_pid, name
    except Exception:
        return None


def main():
    gen = TypeTreeGenerator(UNITY_VER)
    gen.load_local_dll_folder(MANAGED)
    log("TypeTreeGenerator 载入 %d 个 dll" % len(gen.get_loaded_dll_names()))

    profiles = []    # (file, pathid, componentTypeNames[])
    bloom_like = []  # (file, pathid, keys)
    volume_comp = [] # (file, pathid, keys) —— 带 m_IsGlobal/m_Priority 的 Volume
    scanned = []
    failed_files = []

    for rel in TARGETS:
        path = os.path.join(GAME, rel)
        try:
            env = UnityPy.load(path)
        except Exception as e:
            failed_files.append((rel, repr(e)[:80]))
            continue
        try:
            n_obj = len(env.objects)
        except Exception:
            n_obj = -1
        if n_obj <= 0:
            failed_files.append((rel, "loaded but 0 objects"))
            continue
        env.typetree_generator = gen
        scanned.append((rel, n_obj))

        by_id = {}
        mobs = []
        for o in env.objects:
            if o.type.name != "MonoBehaviour":
                continue
            by_id[o.path_id] = o
            mobs.append(o)

        # 只对「代表」做 typetree：按 script 分组
        groups = collections.defaultdict(list)
        for o in mobs:
            h = parse_header(o.get_raw_data())
            if h:
                groups[h[0]].append(o.path_id)

        for key, pids in groups.items():
            rep = by_id.get(pids[0])
            if rep is None:
                continue
            try:
                tt = rep.read_typetree()
            except Exception:
                continue
            if not isinstance(tt, dict):
                continue
            ks = set(tt.keys())
            if "m_Scatter" in ks or ("m_Threshold" in ks and "m_Intensity" in ks):
                bloom_like.append((rel, rep.path_id, sorted(ks)))
            if "m_IsGlobal" in ks and "m_Priority" in ks:
                volume_comp.append((rel, rep.path_id, sorted(ks)))
            if "m_Components" in ks:
                comps = []
                for c in (tt.get("m_Components") or []):
                    nm = "?"
                    try:
                        co = c.read() if hasattr(c, "read") else None
                    except Exception:
                        co = None
                    try:
                        if co is not None:
                            ctt = co.read_typetree()
                            nm = ",".join(sorted(ctt.keys())[:6]) if isinstance(ctt, dict) else "?"
                    except Exception:
                        nm = "<解析失败>"
                    comps.append(nm)
                profiles.append((rel, rep.path_id, len(pids), comps))

    log("")
    log("扫描到的容器文件：")
    for rel, n in scanned:
        log("   %-28s %d 个对象" % (rel, n))
    if failed_files:
        log("")
        log("读失败的文件：")
        for rel, why in failed_files:
            log("   %-28s %s" % (rel, why))

    log("")
    log("=" * 78)
    log("VolumeProfile（带 m_Components 的 MonoBehaviour）共 %d 组" % len(profiles))
    log("=" * 78)
    for rel, pid, n, comps in profiles:
        log("  %s pathID %d（该脚本 %d 个实例）" % (rel, pid, n))
        log("    组件 %d 个：" % len(comps))
        for c in comps:
            log("       - %s" % c)

    log("")
    log("=" * 78)
    log("Volume 组件（带 m_IsGlobal + m_Priority）：%d 组" % len(volume_comp))
    log("=" * 78)
    for rel, pid, ks in volume_comp[:12]:
        log("  %s pathID %d  %s" % (rel, pid, ks))

    log("")
    log("=" * 78)
    log("Bloom 特征（m_Scatter / m_Threshold+m_Intensity）：%d 组" % len(bloom_like))
    log("=" * 78)
    for rel, pid, ks in bloom_like[:12]:
        log("  %s pathID %d  %s" % (rel, pid, ks))
    if not bloom_like:
        log("  → 游戏里没有任何 Bloom 组件（UBER 后处理 / URP Bloom 都没启用）")

    with open(os.path.join(OUT, "volume_probe.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % os.path.join(OUT, "volume_probe.txt"))


if __name__ == "__main__":
    try:
        main()
    except Exception:
        log(traceback.format_exc())
        with open(os.path.join(OUT, "volume_probe.txt"), "w", encoding="utf-8") as f:
            f.write("\n".join(lines))
