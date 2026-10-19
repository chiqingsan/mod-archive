import pefile, json, hashlib, os, sys

# 自证方案: 结果写入 JSON, 文件内嵌内容的 sha256, 读取方可复核
BASE = r"D:\app\steam\steamapps\common\A1"
TARGETS = ["WorldApart.exe", "UnityPlayer.dll", "GameAssembly.dll",
           "gpShell.dll", "gpHackerProc.dll"]
PROXIES = {"winhttp.dll", "version.dll", "winmm.dll", "dinput8.dll",
           "dsound.dll", "xinput1_4.dll", "wininet.dll", "dbghelp.dll",
           "shlwapi.dll", "iphlpapi.dll", "user32.dll", "d3d11.dll", "dxgi.dll"}

result = {"base": BASE, "cwd": os.getcwd(), "targets": {}}
for name in TARGETS:
    path = os.path.join(BASE, name)
    entry = {"exists": os.path.exists(path), "size": None, "imports": None, "proxy_hits": None, "error": None}
    try:
        if entry["exists"]:
            entry["size"] = os.path.getsize(path)
            pe = pefile.PE(path, fast_load=True)
            pe.parse_data_directories(directories=[pefile.DIRECTORY_ENTRY['IMAGE_DIRECTORY_ENTRY_IMPORT']])
            imps = sorted({e.dll.decode().lower() for e in (pe.DIRECTORY_ENTRY_IMPORT or [])})
            entry["imports"] = imps
            entry["proxy_hits"] = sorted(set(imps) & PROXIES)
            pe.close()
    except Exception as e:
        entry["error"] = f"{type(e).__name__}: {e}"
    result["targets"][name] = entry

body = json.dumps(result, ensure_ascii=False, indent=1)
digest = hashlib.sha256(body.encode("utf-8")).hexdigest()
out = body + f"\nSHA256_OF_ABOVE: {digest}\n"
with open(os.path.join(os.path.dirname(os.path.abspath(__file__)), "import_report_v2.json"), "w", encoding="utf-8") as f:
    f.write(out)
print("OK bytes:", len(out))
