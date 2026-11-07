import os
import sys

STRIPPED = ["mainInvJSON", "soldInvJSON", "docInvJSON", "showcaseInvJSON"]


def skip_ws(s, i):
    while i < len(s) and s[i] in " \t\r\n":
        i += 1
    return i


def read_string_end(s, i):
    assert s[i] == '"', f"expected string at {i}"
    j = i + 1
    while j < len(s):
        c = s[j]
        if c == "\\":
            j += 2
            continue
        if c == '"':
            return j + 1
        j += 1
    raise ValueError("unterminated string")


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
        raise ValueError("unclosed object/array")
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


def escape(x):
    out = []
    for c in x:
        if c in '"\\/“”':
            out.append("\\" + c)
        elif c == "\b":
            out.append("\\b")
        elif c == "\f":
            out.append("\\f")
        elif c == "\n":
            out.append("\\n")
        elif c == "\r":
            out.append("\\r")
        elif c == "\t":
            out.append("\\t")
        else:
            out.append(c)
    return "".join(out)


def props(s, obj_start):
    """返回 {属性名: (name_start, val_start, val_end)}，保留顺序。"""
    result = {}
    assert s[obj_start] == "{", f"expected object at {obj_start}, got {s[obj_start]!r}"
    pos = skip_ws(s, obj_start + 1)
    while pos < len(s) and s[pos] != "}":
        assert s[pos] == '"', f"expected property name at {pos}: {s[pos:pos + 20]!r}"
        name_end = read_string_end(s, pos)
        name = unescape(s[pos + 1:name_end - 1])
        colon = skip_ws(s, name_end)
        assert s[colon] == ":", f"expected colon at {colon}"
        v = skip_ws(s, colon + 1)
        end = scan_value_end(s, v)
        assert name not in result, f"duplicate property {name}"
        result[name] = (pos, v, end)
        pos = skip_ws(s, end)
        if pos < len(s) and s[pos] == ",":
            pos = skip_ws(s, pos + 1)
    assert s[pos] == "}", f"expected object end at {pos}"
    return result


def main():
    d = sys.argv[1]
    raw = open(os.path.join(d, "save_17.es3"), encoding="utf-8").read()
    slim = open(os.path.join(d, "FastLoad_slim.es3"), encoding="utf-8").read()
    preload = open(os.path.join(d, "FastLoad_preload.es3"), encoding="utf-8").read()
    errors = []

    def check(cond, msg):
        if not cond:
            errors.append(msg)

    top = skip_ws(raw, 0)
    check(raw[top] == "{", "源存档不是对象")
    orig_top = props(raw, top)
    check(set(orig_top) == {"playerStore", "storeStation"}, f"源顶层键异常: {set(orig_top)}")

    p = orig_top["playerStore"]
    pobj = skip_ws(raw, p[1])
    orig_ps = props(raw, pobj)
    check(list(orig_ps) == ["__type", "value"], f"playerStore 包装键异常: {list(orig_ps)}")
    type_literal = raw[orig_ps["__type"][1]:orig_ps["__type"][2]]
    check(type_literal.startswith('"') and type_literal.endswith('"'), "__type 不是字符串")

    vobj = skip_ws(raw, orig_ps["value"][1])
    orig_fields = props(raw, vobj)
    net_start, net_end = orig_fields["networkUpgrade"][1], orig_fields["networkUpgrade"][2]
    net_literal = raw[net_start:net_end]
    orig_literals = {k: raw[v[1]:v[2]] for k, v in orig_fields.items()}

    # ---- slim 校验
    st = skip_ws(slim, 0)
    check(slim[st] == "{", "slim 不是对象")
    slim_top = props(slim, st)
    check(list(slim_top) == ["storeStation", "playerStore"], f"slim 顶层键/顺序异常: {list(slim_top)}")
    check(slim[slim_top["storeStation"][1]:slim_top["storeStation"][2]]
          == raw[orig_top["storeStation"][1]:orig_top["storeStation"][2]], "storeStation 原样校验失败")

    sp = slim_top["playerStore"]
    spobj = skip_ws(slim, sp[1])
    slim_ps = props(slim, spobj)
    check(list(slim_ps) == ["__type", "value"], f"slim playerStore 包装键异常: {list(slim_ps)}")
    check(slim[slim_ps["__type"][1]:slim_ps["__type"][2]] == type_literal, "__type 未原样保留")

    svobj = skip_ws(slim, slim_ps["value"][1])
    slim_fields = props(slim, svobj)
    check(list(slim_fields) == list(orig_fields), "slim 字段集合/顺序与原档不一致")

    for name, (_, vs, ve) in slim_fields.items():
        literal = slim[vs:ve]
        if name in STRIPPED:
            check(literal == '""', f"{name} 未被清空: {literal[:40]!r}")
            dump_path = os.path.join(d, f"dump_{name}.txt")
            if not os.path.exists(dump_path):
                errors.append(f"{name} 缺少缓存 dump")
                continue
            real = open(dump_path, encoding="utf-8").read()
            check('"' + escape(real) + '"' == orig_literals[name],
                  f"{name} 缓存内容转义后与原档不一致（长度 {len(real)}）")
        else:
            check(literal == orig_literals[name], f"{name} 未原样保留")

    # ---- preload 校验
    expected_preload = '{"playerStore":{"__type":' + type_literal + ',"value":{"networkUpgrade":' + net_literal + '}}}\n'
    check(preload == expected_preload,
          f"preload 内容不正确（实际 {len(preload):,} 字符，期望 {len(expected_preload):,}）")

    print(f"原始 {len(raw):,} 字符 / slim {len(slim):,} 字符 / preload {len(preload):,} 字符")
    print(f"playerStore 字段数 {len(orig_fields)}，networkUpgrade {len(net_literal):,} 字符")
    if errors:
        print("校验失败：")
        for e in errors:
            print("  -", e)
        return 1

    print("校验通过：slim/preload 结构、字段顺序、缓存还原、preload 内容全部一致")
    return 0


sys.exit(main())
