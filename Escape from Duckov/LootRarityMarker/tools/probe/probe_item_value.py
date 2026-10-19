#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
只读探测：把 Item 实例的 Value 分布打出来，用来验证「价值 → 颜色」的阈值是否合理。

为什么需要：
    useValueRarity 用 200 / 600 / 1200 / 2500 / 5000 把 Item.Value 分成白/绿/蓝/紫/橙/红。
    这套阈值是从「物品价值稀有度与搜索音效」mod 里挖出来的，但还得确认它们
    在**实际关卡里的物品**上会分出一个合理的颜色比例 ——
    如果 90% 的物品都 > 2500，那满地图都是橙红，反而不如不染。

做法：
    1) 在 resources.assets 里按「字段签名」找出 Item 的 script pathID
       （Item 组必然同时有 value / quality / displayQuality 三个字段），扫全部组，不提前 break；
    2) 其它文件只按 MonoBehaviour 头解析分组，仅对第 1 步找到的 pathID 读 typetree。

输出：probe_out/item_value_probe.txt
"""

import os
import struct
from collections import Counter

import UnityPy
from UnityPy.helpers.TypeTreeGenerator import TypeTreeGenerator

DATA = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
MANAGED = os.path.join(DATA, "Managed")
OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "probe_out")

TARGETS = ["resources.assets", "level10", "level12", "level15", "level36", "level48"]

VALUE_FIELD = "value"
QUALITY_FIELD = "quality"
DISPLAY_FIELD = "displayQuality"

# 与 ModSettings 的默认阈值保持一致
LIMITS = [(5000, "红"), (2500, "橙"), (1200, "紫"), (600, "蓝"), (200, "绿")]
ORDER = ["白", "绿", "蓝", "紫", "橙", "红"]


def bucket(value):
    for limit, name in LIMITS:
        if value > limit:
            return name
    return "白"


def group_by_script(env):
    """MonoBehaviour 按 m_Script.m_PathID 分组 —— 只用固定头，不读 typetree。"""
    groups = {}
    for obj in env.objects:
        if obj.type.name != "MonoBehaviour":
            continue
        try:
            raw = obj.get_raw_data()
        except Exception:
            continue
        if len(raw) < 32:
            continue
        try:
            spid = struct.unpack_from("<q", raw, 20)[0]
        except struct.error:
            continue
        groups.setdefault(spid, []).append(obj)
    return groups


def main():
    os.makedirs(OUT, exist_ok=True)
    lines = []

    def emit(s=""):
        print(s)
        lines.append(s)

    gen = TypeTreeGenerator("2022.3.62f2")
    gen.load_local_dll_folder(MANAGED)

    # ---- 第 1 步：在 resources.assets 里定位 Item 组 ----
    emit("在 resources.assets 里定位 Item 组（按 value/quality/displayQuality 字段签名）…")
    env = UnityPy.load(os.path.join(DATA, "resources.assets"))
    env.typetree_generator = gen
    groups = group_by_script(env)

    item_pids = []
    for spid, objs in groups.items():
        try:
            tree = objs[0].read_typetree()
        except Exception:
            continue
        if isinstance(tree, dict) and VALUE_FIELD in tree and QUALITY_FIELD in tree and DISPLAY_FIELD in tree:
            item_pids.append(spid)

    emit("Item 组 script pathID: %s" % item_pids)
    if not item_pids:
        emit("没找到 Item 组，放弃。")
        with open(os.path.join(OUT, "item_value_probe.txt"), "w", encoding="utf-8") as f:
            f.write("\n".join(lines))
        return

    grand = Counter()
    total_items = 0
    value_hist = Counter()

    # ---- 第 2 步：逐个关卡统计 ----
    for target in TARGETS:
        path = os.path.join(DATA, target)
        if not os.path.exists(path):
            emit("跳过（不存在）：%s" % target)
            continue

        env = UnityPy.load(path)
        env.typetree_generator = gen
        groups = group_by_script(env)

        local = Counter()
        values = []
        for spid in item_pids:
            for obj in groups.get(spid, []):
                try:
                    tree = obj.read_typetree()
                except Exception:
                    continue
                if not isinstance(tree, dict) or VALUE_FIELD not in tree:
                    continue
                v = int(tree[VALUE_FIELD])
                local[bucket(v)] += 1
                values.append(v)

        n = sum(local.values())
        total_items += n
        grand.update(local)
        for v in values:
            value_hist[v] += 1

        emit("")
        emit("=== %s ===  Item 实例数: %d" % (target, n))
        for name in ORDER:
            c = local.get(name, 0)
            emit("  %s : %6d  (%5.1f%%)" % (name, c, 100.0 * c / n if n else 0.0))
        if values:
            sv = sorted(values)
            emit("  value min / 中位 / max = %d / %d / %d" % (sv[0], sv[len(sv) // 2], sv[-1]))

    emit("")
    emit("=" * 62)
    emit("合计 Item 实例数: %d" % total_items)
    if total_items:
        for name in ORDER:
            c = grand.get(name, 0)
            emit("  %s : %7d  (%5.1f%%)" % (name, c, 100.0 * c / total_items))
        colored = total_items - grand.get("白", 0)
        emit("  其中「非白」（会被染上颜色）: %d (%.1f%%)" % (colored, 100.0 * colored / total_items))

    if value_hist:
        emit("")
        emit("最常见的 25 个 value（value : 次数）:")
        for v, c in value_hist.most_common(25):
            emit("  %6d : %d" % (v, c))

    out_path = os.path.join(OUT, "item_value_probe.txt")
    with open(out_path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % out_path)


if __name__ == "__main__":
    main()
