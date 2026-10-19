# -*- coding: utf-8 -*-
"""查清「物品的品质到底存在哪个字段里」。

起因（运行时日志）：
    [LootRarityMarker][诊断] LootBox_Tools3_23：物品数=3，最高品质=None，排序键=0，不染色=True
    [LootRarityMarker][诊断] LootBox_EnemyDie_Box(Clone)：物品数=9，最高品质=Orange，排序键=5，不染色=False

同一套代码，尸体包（EnemyDie）能读出 Blue/Orange，而世界容器（Tools3/Lux/Locker/Normal/Food/
Hangclothes）一律读出 None。说明问题不在 mod，在**数据源**：Item.DisplayQuality 对大量物品就是 None。

Item.cs 里 quality / displayQuality 都只是 [SerializeField] 字段：
    :36  private int quality;
    :39  private DisplayQuality displayQuality;
而 ItemAssetsCollection.InstantiateAsync 只是 Object.Instantiate(prefab)，
**不会**把 ItemMetaData 里的品质写进实例。所以实例上的品质值 == 预制体上序列化的值。

本脚本直接从 resources.assets 里把所有 Item 预制体的三个字段读出来，给出真实分布：
    quality (int) / displayQuality (enum) / value (int)
用来决定「按品质染色」该取哪个字段、以及 Value→品质 的分档阈值。

只读。
"""
import os
import struct
import traceback
from collections import Counter, defaultdict

import UnityPy
from UnityPy.helpers.TypeTreeGenerator import TypeTreeGenerator

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
MANAGED = os.path.join(GAME, "Managed")
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)

# Item 类的特征字段（三个都要有才算）
SIGNATURE = ("quality", "displayQuality", "value")

QUALITY_NAMES = ["None", "White", "Green", "Blue", "Purple", "Orange", "Red", "Q7", "Q8"]
lines = []


def log(msg=""):
    print(msg)
    lines.append(str(msg))


def read_header(raw):
    go_pid = struct.unpack_from("<q", raw, 4)[0]
    script_pid = struct.unpack_from("<q", raw, 20)[0]
    nlen = struct.unpack_from("<i", raw, 28)[0]
    name = raw[32:32 + nlen].decode("utf-8", "replace") if 0 < nlen < 200 else ""
    return go_pid, script_pid, name


def qname(v):
    try:
        i = int(v)
    except Exception:
        return str(v)
    if 0 <= i < len(QUALITY_NAMES):
        return QUALITY_NAMES[i]
    return "?%s" % v


def main():
    gen = TypeTreeGenerator("2022.3.62f2")
    gen.load_local_dll_folder(MANAGED)

    path = os.path.join(GAME, "resources.assets")
    env = UnityPy.load(path)
    env.typetree_generator = gen

    groups = defaultdict(list)
    for o in env.objects:
        if o.type.name != "MonoBehaviour":
            continue
        try:
            raw = o.get_raw_data()
            go_pid, spid, name = read_header(raw)
        except Exception:
            continue
        groups[spid].append((o, go_pid, name))
    log("MonoBehaviour 总数 = %d，组数 = %d" % (
        sum(len(v) for v in groups.values()), len(groups)))
    log("")

    # ---- 找出所有匹配 Item 特征字段的组（派生类各占一组，全部扫） ----
    item_groups = []
    scanned = 0
    for spid, members in sorted(groups.items(), key=lambda kv: -len(kv[1])):
        try:
            tt = members[0][0].read_typetree()
        except Exception:
            continue
        scanned += 1
        if all(k in tt for k in SIGNATURE):
            item_groups.append((spid, members, tt))

    log("扫了 %d 组；匹配 Item 特征字段的组：%d 个" % (scanned, len(item_groups)))
    for spid, members, tt in item_groups:
        log("  pathID=%-8d 实例=%-5d 字段=%s" % (spid, len(members), sorted(tt.keys())[:14]))
    log("")

    # ---- 把每个组的实例全部读出来 ----
    for spid, members, sample in item_groups:
        log("=" * 78)
        log("脚本 pathID=%d，实例 %d 个" % (spid, len(members)))

        dq_counter = Counter()
        q_counter = Counter()
        pair_counter = Counter()
        value_by_dq = defaultdict(list)
        value_by_q = defaultdict(list)
        q_by_dq = defaultdict(Counter)
        names = Counter()
        failed = 0

        for o, go_pid, name in members:
            try:
                mtt = o.read_typetree()
            except Exception:
                failed += 1
                continue
            dq = mtt.get("displayQuality")
            q = mtt.get("quality")
            val = mtt.get("value")
            try:
                val_i = int(val)
            except Exception:
                val_i = -1

            dq_counter[qname(dq)] += 1
            q_counter[int(q) if q is not None else -1] += 1
            pair_counter[(int(q) if q is not None else -1, qname(dq))] += 1
            value_by_dq[qname(dq)].append(val_i)
            value_by_q[int(q) if q is not None else -1].append(val_i)
            q_by_dq[qname(dq)][int(q) if q is not None else -1] += 1
            names[name or "<无名>"] += 1

        if failed:
            log("  （%d 个实例解析失败）" % failed)

        log("  displayQuality 分布: %s" % dict(dq_counter))
        log("  quality(int)   分布: %s" % dict(sorted(q_counter.items())))
        log("")
        log("  quality × displayQuality 交叉表:")
        for (q, dq), n in sorted(pair_counter.items(), key=lambda kv: -kv[1])[:20]:
            log("      quality=%-3d displayQuality=%-8s → %d 个" % (q, dq, n))
        log("")

        def stat(vals):
            vals = [v for v in vals if v >= 0]
            if not vals:
                return "无"
            vals.sort()
            n = len(vals)
            return "n=%d min=%d p25=%d 中位=%d p75=%d p90=%d max=%d" % (
                n, vals[0], vals[n // 4], vals[n // 2], vals[3 * n // 4],
                vals[min(n - 1, int(n * 0.9))], vals[-1])

        log("  按 displayQuality 分组的 value 分布:")
        for dq in QUALITY_NAMES[:7]:
            if value_by_dq.get(dq):
                log("      %-8s %s" % (dq, stat(value_by_dq[dq])))
        log("")
        log("  按 quality(int) 分组的 value 分布:")
        for q in sorted(value_by_q):
            log("      quality=%-3d %s" % (q, stat(value_by_q[q])))
        log("")
        log("  最常见的前 15 个物品名: %s" % names.most_common(15))
        log("")

    with open(os.path.join(OUT, "item_quality_probe.txt"), "w", encoding="utf-8") as f:
        f.write("\n".join(lines))
    print("\n[写出] %s" % os.path.join(OUT, "item_quality_probe.txt"))


if __name__ == "__main__":
    try:
        main()
    except Exception:
        log(traceback.format_exc())
        with open(os.path.join(OUT, "item_quality_probe.txt"), "w", encoding="utf-8") as f:
            f.write("\n".join(lines))
