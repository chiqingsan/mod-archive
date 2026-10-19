# -*- coding: utf-8 -*-
"""
probe_container_total.py  —— 只读探测

目的：回答「容器总价值」的真实分布，用来给发光曲线选阈值，而不是拍脑袋。

做法：
  1. 从 resources.assets 读出全部 Item 的 value（脚本 pathID 644，已在前面的探测里确认）。
  2. 这 1581 个 Item 是「物品定义池」，容器开出来的物品就是从这个池子里按权重抽的
     （抽多少件、抽哪个池子由 LootBoxLoader 的掉落表决定，资产里没有把确定性结果序列化下来）。
  3. 因此用蒙特卡洛：按「物品池权重」有放回抽 N 件、把 value 加起来，重复很多次，
     得到「N 件容器」的总价值分布，输出各百分位。

这不是精确值，但足以回答「线性 200→5000 会不会全部发光」。
"""
import os
import random
import struct
from collections import Counter

import UnityPy
from UnityPy.helpers.TypeTreeGenerator import TypeTreeGenerator

DATA = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
MANAGED = os.path.join(DATA, "Managed")
OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "probe_out")
os.makedirs(OUT, exist_ok=True)
RES = os.path.join(DATA, "resources.assets")

ITEM_SCRIPT = 644


def collect_item_values():
    env = UnityPy.load(RES)
    gen = TypeTreeGenerator("2022.3.62f2")
    gen.load_local_dll_folder(MANAGED)
    env.typetree_generator = gen

    # 先按 MonoBehaviour 头部分组：offset 20 = m_Script.m_PathID
    groups = {}
    for obj in env.objects:
        if obj.type.name != "MonoBehaviour":
            continue
        raw = obj.get_raw_data()
        if len(raw) < 28:
            continue
        script = struct.unpack_from("<q", raw, 20)[0]
        groups.setdefault(script, []).append(obj)

    objs = groups.get(ITEM_SCRIPT, [])
    values = []
    for obj in objs:
        try:
            tree = obj.read_typetree()
        except Exception:
            continue
        v = tree.get("value")
        if v is None:
            continue
        try:
            v = int(v)
        except Exception:
            continue
        if v > 0:
            values.append(v)
    return values


def pct(sorted_vals, p):
    if not sorted_vals:
        return 0
    k = int(round((len(sorted_vals) - 1) * p / 100.0))
    return sorted_vals[max(0, min(len(sorted_vals) - 1, k))]


def main():
    lines = []
    def out(s=""):
        print(s)
        lines.append(s)

    values = collect_item_values()
    out("Item 实例（value>0）：%d 个" % len(values))
    if not values:
        out("没读到任何 Item，中止。")
        return
    s = sorted(values)
    out("单件 value 分布：min=%d p25=%d p50=%d p75=%d p90=%d p99=%d max=%d"
        % (s[0], pct(s, 25), pct(s, 50), pct(s, 75), pct(s, 90), pct(s, 99), s[-1]))
    out("单件平均 value = %.1f" % (sum(values) / float(len(values))))

    random.seed(20261019)
    pool = values
    out("")
    out("=== 蒙特卡洛：N 件容器总价值分布的百分位 ===")
    header = "件数 |  p25  |  p50  |  p75  |  p90  |  p95  |  p99  |  max  | >5000 占比 | >12000 占比"
    out(header)
    out("-" * len(header))
    for n in (2, 3, 4, 5, 6, 8):
        totals = []
        for _ in range(20000):
            tot = 0
            for _k in range(n):
                tot += pool[random.randrange(len(pool))]
            totals.append(tot)
        totals.sort()
        over5k = 100.0 * sum(1 for t in totals if t > 5000) / len(totals)
        over12k = 100.0 * sum(1 for t in totals if t > 12000) / len(totals)
        out("%4d | %5d | %5d | %5d | %5d | %5d | %6d | %6d | %9.1f%% | %10.1f%%"
            % (n, pct(totals, 25), pct(totals, 50), pct(totals, 75),
               pct(totals, 90), pct(totals, 95), pct(totals, 99),
               totals[-1], over5k, over12k))

    out("")
    out("=== 单件 value 出现次数 top 20 ===")
    for v, c in Counter(values).most_common(20):
        out("  value=%-6d x %d" % (v, c))

    path = os.path.join(OUT, "container_total_probe.txt")
    with open(path, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n写出：%s" % path)


if __name__ == "__main__":
    main()
