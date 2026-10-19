import struct, json

BASE = r"D:\app\steam\steamapps\common\A1"
TARGETS = ["WorldApart.exe", "UnityPlayer.dll", "GameAssembly.dll",
           "gpShell.dll", "gpHackerProc.dll", "winhttp.dll"]
PROXY_SET = {"winhttp.dll", "version.dll", "winmm.dll", "dinput8.dll",
             "dsound.dll", "xinput1_4.dll", "wininet.dll", "dbghelp.dll",
             "shlwapi.dll", "iphlpapi.dll"}

def imports_of(path):
    data = open(path, "rb").read()
    e_lfanew = struct.unpack_from("<I", data, 0x3C)[0]
    assert data[e_lfanew:e_lfanew+4] == b"PE\0\0", "not PE"
    coff = e_lfanew + 4
    nsec, opt_size = struct.unpack_from("<HH", data, coff + 2)
    opt = coff + 20
    magic = struct.unpack_from("<H", data, opt)[0]
    ddir_off = opt + (112 if magic == 0x20B else 96)
    imp_rva, imp_size = struct.unpack_from("<II", data, ddir_off + 8)
    sec0 = opt + opt_size
    secs = []
    for i in range(nsec):
        o = sec0 + i * 40
        vsize, va, rsize, raw = struct.unpack_from("<IIII", data, o + 8)
        secs.append((va, max(vsize, rsize), raw, rsize))
    def rva2off(rva):
        for va, sz, raw, rsize in secs:
            if va <= rva < va + sz:
                off = raw + (rva - va)
                return off if off < len(data) else None
        return None
    out = []
    if imp_rva == 0:
        return out
    off = rva2off(imp_rva)
    assert off is not None, "import rva not mapped"
    while True:
        ent = data[off:off + 20]
        if len(ent) < 20 or ent == b"\x00" * 20:
            break
        name_rva, first_thunk = struct.unpack_from("<II", ent, 12)
        no = rva2off(name_rva)
        if no is None:
            break
        end = data.index(b"\x00", no)
        dll = data[no:end].decode("ascii", "replace").lower()
        out.append(dll)
        off += 20
    return out

result = {}
for name in TARGETS:
    try:
        imps = imports_of(BASE + "\\" + name)
        result[name] = {
            "count": len(imps),
            "all": imps,
            "proxy_hits": sorted(set(imps) & PROXY_SET),
        }
    except Exception as e:
        result[name] = {"error": f"{type(e).__name__}: {e}"}

with open("import_report.json", "w", encoding="utf-8") as f:
    json.dump(result, f, indent=1)
print("written import_report.json")
