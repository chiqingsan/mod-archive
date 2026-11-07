import os
import re
import sys


def skip_ws(s, i):
    while i < len(s) and s[i] in " \t\r\n":
        i += 1
    return i


def read_string_end(s, i):
    j = i + 1
    while j < len(s):
        c = s[j]
        if c == "\\":
            j += 2
            continue
        if c == '"':
            return j + 1
        j += 1
    raise ValueError("unterminated")


def scan_value_end(s, i):
    c = s[i]
    if c == '"':
        return read_string_end(s, i)
    if c in "{[":
        depth = 0
        j = i
        while j < len(s):
            ch = s[j]
            if ch == '"':
                j = read_string_end(s, j)
                continue
            if ch in "{[":
                depth += 1
            elif ch in "}]":
                depth -= 1
                if depth == 0:
                    return j + 1
            j += 1
        raise ValueError("unclosed")
    k = i
    while k < len(s) and s[k] not in ",}] \t\r\n":
        k += 1
    return k


def unescape(x):
    out = []
    i = 0
    while i < len(x):
        c = x[i]
        if c == "\\" and i + 1 < len(x):
            n = x[i + 1]
            out.append({"b": "\b", "f": "\f", "n": "\n", "r": "\r", "t": "\t"}.get(n, n))
            i += 2
        else:
            out.append(c)
            i += 1
    return "".join(out)


def find_prop(s, obj_start, name):
    pos = skip_ws(s, obj_start + 1)
    while pos < len(s) and s[pos] != "}":
        name_end = read_string_end(s, pos)
        prop = unescape(s[pos + 1:name_end - 1])
        colon = skip_ws(s, name_end)
        v = skip_ws(s, colon + 1)
        end = scan_value_end(s, v)
        if prop == name:
            return v, end
        pos = skip_ws(s, end)
        if pos < len(s) and s[pos] == ",":
            pos = skip_ws(s, pos + 1)
    return None


def main():
    d = sys.argv[1]
    raw = open(os.path.join(d, "save_17.es3"), encoding="utf-8").read()
    top = skip_ws(raw, 0)
    ps = find_prop(raw, top, "playerStore")
    pobj = skip_ws(raw, ps[0])
    val = find_prop(raw, pobj, "value")
    vobj = skip_ws(raw, val[0])

    feats = find_prop(raw, vobj, "savedItemFeatureList")
    feats_text = unescape(raw[feats[0] + 1:feats[1] - 1])
    parent_ids = [int(x) for x in re.findall(r'"parentItemUniqueId":(-?\d+)', feats_text)]
    print(f"features 数量: {len(parent_ids)}，去重 parent: {len(set(parent_ids))}")

    sold_text = open(os.path.join(d, "dump_soldInvJSON.txt"), encoding="utf-8").read()
    sold_ids = {int(x) for x in re.findall(r'"uniqueId":(-?\d+)', sold_text)}
    sold_all_ids = {int(x) for x in re.findall(r'"(?:uniqueId|uuid)":(-?\d+)', sold_text)}
    print(f"sold 节点 uniqueId 数: {len(sold_ids)}（含 uuid {len(sold_all_ids)}）")

    main_text = open(os.path.join(d, "dump_mainInvJSON.txt"), encoding="utf-8").read()
    main_ids = {int(x) for x in re.findall(r'"uniqueId":(-?\d+)', main_text)}
    print(f"main 节点 uniqueId 数: {len(main_ids)}")

    on_sold = [p for p in parent_ids if p in sold_ids]
    on_main = [p for p in parent_ids if p in main_ids]
    print(f"features 挂到 sold 物品的: {len(on_sold)}（去重 {len(set(on_sold))}）")
    print(f"features 挂到 main 物品的: {len(on_main)}（去重 {len(set(on_main))}）")
    print(f"features 的 parent 不在 main 也不在 sold: {len([p for p in parent_ids if p not in main_ids and p not in sold_ids])}")
    return 0


sys.exit(main())
