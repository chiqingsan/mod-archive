# -*- coding: utf-8 -*-
"""量级探测：关卡里到底有多少个"可交互容器"、多少个 Interactable 层的碰撞体。

为什么要查：mod 每次扫描做一次 25 米 Physics.OverlapSphereNonAlloc。
这个查询的成本上限 = 整个关卡里 Interactable 层的碰撞体总数（层过滤发生在
PhysX 把结果交给我们之前，所以我们永远不可能收到更多）。同时，每个命中
（容器）我们都要走一次 GetComponentInParent + 遍历整个 Inventory。

所以两个数字决定了性能天花板：
  1) 单关卡 Interactable 层的碰撞体总数   -> 物理查询的绝对上限
  2) 单关卡 InteractableLootbox 实例总数  -> 每个命中要做的工作量上限

顺带把 TagManager 的层名表读出来，确认 Interactable 的层索引。

只读。输出 tools/probe/probe_out/scale_probe.txt
"""
import os
import struct
import sys
import time
import traceback
from collections import Counter, defaultdict

import UnityPy

GAME = r"d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data"
HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, "probe_out")
os.makedirs(OUT, exist_ok=True)
OUTFILE = os.path.join(OUT, "scale_probe.txt")

# InteractableBase 家族的特征序列化字段（probe_marker_visible.py 已证实存在）
INTERACTABLE_MARKER_FIELD = "interactMarkerVisible"
# InteractableLootbox 的特征字段
LOOTBOX_SIG = ("inventoryReference", "displayNameKey")
# 容器类脚本 pathID（resources.assets 里探到的 InteractableLootbox）
LOOTBOX_SCRIPT_PID = 268

COLLIDER_TYPES = {
    "BoxCollider", "SphereCollider", "CapsuleCollider",
    "MeshCollider", "TerrainCollider", "WheelCollider",
}

lines = []


def log(msg=""):
    print(msg)
    sys.stdout.flush()
    lines.append(str(msg))


def flush():
    with open(OUTFILE, "w", encoding="utf-8") as f:
        f.write("\n".join(lines))


def read_mb_header(raw):
    """MonoBehaviour 固定头：go_pid@4, script_pid@20, name@28(长度前缀,4 字节对齐)。"""
    go_pid = struct.unpack_from("<q", raw, 4)[0]
    script_pid = struct.unpack_from("<q", raw, 20)[0]
    return go_pid, script_pid


def read_go_header(raw):
    """任意 Component 的固定头：m_GameObject = (int32 fileID, int64 pathID)。"""
    return struct.unpack_from("<q", raw, 4)[0]


def list_containers():
    """列出 Duckov_Data 下的资源容器（含无扩展名的 level* 场景）。"""
    out = []
    for name in sorted(os.listdir(GAME)):
        p = os.path.join(GAME, name)
        if not os.path.isfile(p):
            continue
        # 跳过 .resS / .json / .info / .config 之类的附属文件
        if name.endswith(".resS") or name.endswith(".json"):
            continue
        if name in ("app.info", "boot.config", "RuntimeInitializeOnLoads.json",
                    "ScriptingAssemblies.json"):
            continue
        out.append(name)
    return out


def find_layer_names(containers):
    """从 TagManager 里读层名表，返回 {index: name}。"""
    for cname in containers:
        try:
            env = UnityPy.load(os.path.join(GAME, cname))
        except Exception:
            continue
        try:
            for o in env.objects:
                if o.type.name != "TagManager":
                    continue
                try:
                    tt = o.read_typetree()
                except Exception:
                    try:
                        tt = o.read().__dict__
                    except Exception:
                        continue
                layers = tt.get("layers")
                if layers:
                    return {i: n for i, n in enumerate(layers)}, cname
        finally:
            del env
    return {}, None


def scan_container(cname, interactable_scripts):
    """扫一个容器：统计 MonoBehaviour / GameObject+层 / 碰撞体+层。"""
    path = os.path.join(GAME, cname)
    size = os.path.getsize(path)
    env = UnityPy.load(path)

    mb_total = 0
    mb_by_script = Counter()
    go_layer = {}
    go_total = 0
    layer_go = Counter()
    coll_total = 0
    layer_coll = Counter()
    coll_by_layer_name = Counter()
    tt_fail = 0

    for o in env.objects:
        t = o.type.name
        if t == "MonoBehaviour":
            mb_total += 1
            try:
                raw = o.get_raw_data()
                _, spid = read_mb_header(raw)
                mb_by_script[spid] += 1
            except Exception:
                pass
        elif t == "GameObject":
            go_total += 1
            try:
                tt = o.read_typetree()
                lay = tt.get("m_Layer")
                if lay is None:
                    raise ValueError("no m_Layer")
                go_layer[o.path_id] = lay
                layer_go[lay] += 1
            except Exception:
                tt_fail += 1
        elif t in COLLIDER_TYPES:
            coll_total += 1
            try:
                raw = o.get_raw_data()
                gpid = read_go_header(raw)
                lay = go_layer.get(gpid)
                if lay is None:
                    lay = -1
                layer_coll[lay] += 1
            except Exception:
                layer_coll[-2] += 1

    del env
    return {
        "name": cname, "size": size,
        "mb_total": mb_total, "mb_by_script": mb_by_script,
        "go_total": go_total, "go_tt_fail": tt_fail, "layer_go": layer_go,
        "coll_total": coll_total, "layer_coll": layer_coll,
        "interactable_scripts": interactable_scripts,
    }


def main():
    t0 = time.time()
    containers = list_containers()
    log("资源容器数量 = %d" % len(containers))
    log("")

    layers, layer_src = find_layer_names(containers)
    log("TagManager 来源 = %s" % layer_src)
    for i in sorted(layers):
        if i < 8 or layers[i]:
            log("  layer[%2d] = %s" % (i, layers[i]))
    log("")

    # ---- 第一步：在 resources.assets 里找出所有 InteractableBase 家族的脚本 pathID ----
    log("=" * 78)
    log("第一步：识别 InteractableBase 家族的脚本 pathID（扫 resources.assets）")
    log("=" * 78)
    interactable_scripts = set()
    lootbox_scripts = set()
    env = UnityPy.load(os.path.join(GAME, "resources.assets"))
    groups = defaultdict(list)
    for o in env.objects:
        if o.type.name != "MonoBehaviour":
            continue
        try:
            _, spid = read_mb_header(o.get_raw_data())
        except Exception:
            continue
        groups[spid].append(o)
    log("resources.assets 的脚本组数 = %d" % len(groups))
    for spid, members in sorted(groups.items(), key=lambda kv: -len(kv[1])):
        try:
            tt = members[0].read_typetree()
        except Exception:
            continue
        keys = set(tt.keys())
        if INTERACTABLE_MARKER_FIELD in keys:
            interactable_scripts.add(spid)
            is_lootbox = all(s in keys for s in LOOTBOX_SIG)
            if is_lootbox:
                lootbox_scripts.add(spid)
            log("  脚本 pathID=%-6d 实例 %-4d %s" % (
                spid, len(members), "<= InteractableLootbox" if is_lootbox else ""))
    log("")
    log("InteractableBase 家族脚本数 = %d，其中 Lootbox = %d" % (
        len(interactable_scripts), len(lootbox_scripts)))
    if LOOTBOX_SCRIPT_PID not in lootbox_scripts:
        log("!! 注意：预期 lootbox 脚本 pathID=%d 不在识别结果里，识别可能不准"
            % LOOTBOX_SCRIPT_PID)
    del env
    log("")

    # ---- 第二步：逐关卡统计 ----
    log("=" * 78)
    log("第二步：逐容器统计")
    log("=" * 78)
    log("%-26s %10s %8s %8s %8s %10s %10s" % (
        "容器", "字节", "MonoBeh", "GameObj", "Collider",
        "可交互", "Interact层碰撞体"))
    results = []
    for cname in containers:
        try:
            r = scan_container(cname, interactable_scripts)
        except Exception:
            log("%-26s  <失败: %s>" % (cname, traceback.format_exc().strip().splitlines()[-1]))
            continue
        n_inter = sum(v for k, v in r["mb_by_script"].items() if k in interactable_scripts)
        n_loot = sum(v for k, v in r["mb_by_script"].items() if k in lootbox_scripts)
        n_inter_coll = r["layer_coll"].get(
            next((i for i, n in layers.items() if n == "Interactable"), -999), 0)
        r["n_inter"] = n_inter
        r["n_loot"] = n_loot
        r["n_inter_coll"] = n_inter_coll
        results.append(r)
        log("%-26s %10d %8d %8d %8d %10d %10d" % (
            cname, r["size"], r["mb_total"], r["go_total"], r["coll_total"],
            n_inter, n_inter_coll))
        flush()

    # ---- 第三步：排序汇总 ----
    log("")
    log("=" * 78)
    log("第三步：按「可交互对象数」排序（这就是每个关卡里能被本 mod 碰到的东西）")
    log("=" * 78)
    log("%-26s %8s %8s %8s %8s %10s" % (
        "容器", "可交互", "Lootbox", "GameObj", "Collider", "Interact层碰撞体"))
    for r in sorted(results, key=lambda x: -x["n_inter"])[:25]:
        log("%-26s %8d %8d %8d %8d %10d" % (
            r["name"], r["n_inter"], r["n_loot"], r["go_total"],
            r["coll_total"], r["n_inter_coll"]))

    worst_inter = max(results, key=lambda x: x["n_inter"]) if results else None
    worst_go = max(results, key=lambda x: x["go_total"]) if results else None
    worst_coll = max(results, key=lambda x: x["coll_total"]) if results else None
    worst_inter_coll = max(results, key=lambda x: x["n_inter_coll"]) if results else None
    log("")
    log("最大值汇总：")
    if worst_inter:
        log("  可交互对象最多 : %-24s %d 个" % (worst_inter["name"], worst_inter["n_inter"]))
    if worst_go:
        log("  GameObject 最多: %-24s %d 个" % (worst_go["name"], worst_go["go_total"]))
    if worst_coll:
        log("  碰撞体最多     : %-24s %d 个" % (worst_coll["name"], worst_coll["coll_total"]))
    if worst_inter_coll:
        log("  Interact层碰撞体最多: %-17s %d 个" % (
            worst_inter_coll["name"], worst_inter_coll["n_inter_coll"]))
    log("")
    log("总耗时 %.1f 秒" % (time.time() - t0))
    flush()


if __name__ == "__main__":
    try:
        main()
    except Exception:
        log(traceback.format_exc())
    flush()
    print("\n[写出] %s" % OUTFILE)
