import csv, statistics as st, collections

D = r"C:\Users\chiqingsan\AppData\LocalLow\Questing Goose Studio\Probably Stolen"
rows = list(csv.DictReader(open(D + r"\SmoothDragAccel-perf-summary.csv", encoding="utf-8-sig")))
def f(x):
    try: return float(x)
    except: return 0.0

# phases by ab column segments
segs = []
cur = None
for r in rows:
    ab = r["ab"]; t = f(r["time_s"])
    if cur is None or cur["ab"] != ab:
        cur = {"ab": ab, "t0": t, "rows": []}
        segs.append(cur)
    cur["rows"].append(r)

print("segments:")
for s in segs:
    fr = sum(f(r["frames"]) for r in s["rows"]); dur = f(s["rows"][-1]["time_s"]) - s["t0"]
    print(f"  ab={s['ab']} n={len(s['rows'])} dur={dur:.0f}s frames={fr:.0f}")

def agg(sel):
    g = [r for r in rows if sel(r) and f(r["fps"]) >= 90]
    frames = sum(f(r["frames"]) for r in g); dur = sum(f(r["frames"]) / f(r["fps"]) for r in g)
    return {
        "n": len(g), "frames": frames, "dur": dur,
        "fps_med": st.median([f(r["fps"]) for r in g]),
        "p50": st.median([f(r["p50_ms"]) for r in g]),
        "p95_med": st.median([f(r["p95_ms"]) for r in g]),
        "p99_med": st.median([f(r["p99_ms"]) for r in g]),
        "over33_min": sum(f(r["over_33ms"]) for r in g) / (dur / 60),
        "alloc": st.median([f(r["alloc_kb_per_s"]) for r in g]),
        "gc0": st.median([f(r["gc0"]) for r in g]) * 6,
        "patch_ms_s": sum(f(r["patch_ms"]) for r in g) / (dur / 1000),
    }

# long phases
for name, sel in [
    ("ON  (23:59:19-00:17:34)", lambda r: f(r["time_s"]) >= 259.2 and f(r["time_s"]) < 1364.6 and r["ab"] == "1"),
    ("OFF (00:17:34-00:34:43)", lambda r: f(r["time_s"]) >= 1364.6 and f(r["time_s"]) < 2392.9 and r["ab"] == "0"),
]:
    a = agg(sel)
    print(f"\n{name}: n={a['n']} frames={a['frames']:.0f} dur={a['dur']:.0f}s fps_med={a['fps_med']:.0f} p50={a['p50']:.2f} p95={a['p95_med']:.2f} p99={a['p99_med']:.2f} over33/min={a['over33_min']:.2f} alloc={a['alloc']:.0f}KB/s gc0/min={a['gc0']:.1f} patch={a['patch_ms_s']:.2f}ms/s")

# detail totals ab=1
det = list(csv.DictReader(open(D + r"\SmoothDragAccel-perf-detail.csv", encoding="utf-8-sig")))
print("\nper-patch (ab=1 rows, all):")
by = collections.defaultdict(lambda: [0, 0, 0.0])
for r in det:
    if r["ab"] != "1": continue
    b = by[r["id"]]
    b[0] += int(r["calls"]); b[1] += int(r["skips"]); b[2] += f(r["self_ms"])
tot_calls = sum(v[0] for v in by.values())
for k, (c, s, ms) in sorted(by.items(), key=lambda kv: -kv[1][2]):
    print(f"  {k:14s} calls={c:>9,} skips={s:>9,} ({100*s/max(c,1):5.1f}%) self={ms:8.1f}ms")
