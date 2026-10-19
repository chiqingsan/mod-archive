import struct, json, os

GAME = r"D:\app\steam\steamapps\common\A1"
FILES = ["WorldApart.exe", "UnityPlayer.dll", "GameAssembly.dll", "gpShell.dll"]
WATCH = {"winhttp.dll", "version.dll", "winmm.dll", "wininet.dll", "dbghelp.dll",
         "shlwapi.dll", "iphlpapi.dll", "user32.dll", "d3d11.dll", "dxgi.dll",
         "winusp.dll", "wintrust.dll", "mscoree.dll"}

def rva_to_off(sections, rva):
    for va, vsize, raw, rsize in sections:
        if va <= rva < va + max(vsize, rsize):
            off = raw + (rva - va)
            return off
    return None

def import_dlls(path):
    data = open(path, "rb").read()
    e_lfanew = struct.unpack_from("<I", data, 0x3C)[0]
    assert data[e_lfanew:e_lfanew+4] == b"PE\x00\x00", "not PE"
    coff = e_lfanew + 4
    machine, nsec, _, _, _, opt_size, _ = struct.unpack_from("<HHIIIHH", data, coff)
    opt = coff + 20
    magic = struct.unpack_from("<H", data, opt)[0]
    ddir = opt + (112 if magic == 0x20B else 96)
    imp_rva, imp_size = struct.unpack_from("<II", data, ddir + 8)
    sec0 = opt + opt_size
    sections = []
    for i in range(nsec):
        o = sec0 + i * 40
        vsize, va, rsize, raw = struct.unpack_from("<IIII", data, o + 8)
        sections.append((va, vsize, raw, rsize))
    out = []
    if imp_rva == 0:
        return out, len(data)
    off = rva_to_off(sections, imp_rva)
    if off is None:
        return None, len(data)
    while True:
        ent = data[off:off+20]
        if len(ent) < 20 or ent == b"\x00"*20:
            break
        name_rva = struct.unpack_from("<I", ent, 12)[0]
        no = rva_to_off(sections, name_rva)
        if no is None:
            break
        end = data.index(b"\x00", no)
        out.append(data[no:end].decode("ascii", "replace").lower())
        off += 20
    return out, len(data)

report = {"game_dir": GAME, "files": {}}
for name in FILES:
    path = os.path.join(GAME, name)
    entry = {"exists": os.path.exists(path)}
    if entry["exists"]:
        try:
            dlls, size = import_dlls(path)
            entry["file_size"] = size
            if dlls is None:
                entry["error"] = "import rva not mapped"
            else:
                entry["import_count"] = len(dlls)
                entry["imports"] = dlls
                entry["watch_hits"] = sorted(set(dlls) & WATCH)
        except Exception as e:
            entry["error"] = f"{type(e).__name__}: {e}"
    report["files"][name] = entry

body = json.dumps(report, indent=1)
with open(os.path.join(os.path.dirname(os.path.abspath(__file__)), "import_report.json"), "w", encoding="utf-8") as f:
    f.write(body)
print("WROTE import_report.json,", len(body), "bytes")
