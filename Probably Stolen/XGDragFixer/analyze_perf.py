import csv, statistics as st, collections

D = r"C:\Users\chiqingsan\AppData\LocalLow\Questing Goose Studio\Probably Stolen"
rows = list(csv.DictReader(open(D + r"\SmoothDragAccel-perf-summary.csv", encoding="utf-8-sig")))
def f(x):
    try: return float(x)
    except: return 0.0

# time_s is cumulative; interval duration = delta
prev = 0.0
for i, r in enumerate(rows):
    t = f(r["time_s"]); r["dur"] = max(t - prev, 0.001); prev = t
total_span = f(rows[-1]["time_s"]) - f(rows[0]["time_s"]) + f(rows[0]["dur"])
print("intervals:", len(rows), "session span ~%.0fs" % total_span)

good = [r for r in rows if f(r["fps"]) >= 90 and int(r["frames"]) >= 300]
print("kept:", len(good), "dropped:", len(rows) - len(good))

agg = {}
for ab in ("1", "0"):
    g = [r for r in good if r["ab"] == ab]
    dur = sum(r["dur"] for r in g); frames = sum(int(r["frames"]) for r in g)
    over = sum(int(r["over_33ms"]) for r in g); gc0 = sum(int(r["gc0"]) for r in g)
    patch = sum(f(r["patch_ms"]) for r in g)
    p95 = [f(r["p95_ms"]) for r in g]; p99 = [f(r["p99_ms"]) for r in g]
    allocs = [f(r["alloc_kb_per_s"]) for r in g]
    stop = [r for r in g if abs(f(r["p95_ms"]) - f(r["p50_ms"])) < 3]  # steady intervals
    agg[ab] = dict(n=len(g), dur=dur, frames=frames, fps=frames / dur,
                   p95_med=st.median(p95), p95_mean=sum(p95) / len(p95), p95_max=max(p95),
                   p99_med=st.median(p99), p99_mean=sum(p99) / len(p99),
                   over=over, over_pm=over / dur * 60,
                   alloc_med=st.median(allocs), gc0_pm=gc0 / dur * 60, patch=patch / dur,
                   steady_p95=st.median([f(r["p95_ms"]) for r in stop]) if stop else 0)
for ab in ("1", "0"):
    a = agg[ab]
    print(f"\n=== ab={ab}: {a['n']} intervals, {a['dur']:.0f}s, {a['frames']} frames")
    print(f"  weighted fps   : {a['fps']:7.1f}")
    print(f"  p95  median/mean/max: {a['p95_med']:.2f} / {a['p95_mean']:.2f} / {a['p95_max']:.1f} ms")
    print(f"  p99  median/mean    : {a['p99_med']:.2f} / {a['p99_mean']:.2f} ms")
    print(f"  >33ms per min       : {a['over_pm']:.3f}  (total {a['over']})")
    print(f"  alloc median        : {a['alloc_med']:.0f} KB/s")
    print(f"  GC0 per min         : {a['gc0_pm']:.3f}")
    print(f"  patch self cost     : {a['patch']:.3f} ms/s")

# phase transitions
print("\n=== ab phase transitions")
cur = None
for r in rows:
    if r["ab"] != cur:
        print(f"  t={f(r['time_s']):8.1f}s  interval={r['interval']:>3s}  ab={r['ab']}")
        cur = r["ab"]

# how much wall time each ab phase got
ph = []; cur = None
prev = 0.0
for r in rows:
    if r["ab"] != cur:
        if cur is not None: ph[-1]["dur"] = f(r["time_s"]) - ph[-1]["start"]
        ph.append({"ab": r["ab"], "start": f(r["time_s"])}); cur = r["ab"]
ph[-1]["dur"] = f(rows[-1]["time_s"]) - ph[-1]["start"]
print("\nphase lengths (s):", [f"{p['ab']}:{p['dur']:.0f}" for p in ph])

# detail: cost & savings per patch, over ab=1 phase wall time
drows = list(csv.DictReader(open(D + r"\SmoothDragAccel-perf-detail.csv", encoding="utf-8-sig")))
dur_by_interval = {int(r["interval"]): r["dur"] for r in rows}
d1 = [r for r in drows if r["ab"] == "1"]
per = {}
for r in d1:
    k = r["id"]; a = per.setdefault(k, {"calls": 0, "skips": 0, "self_ms": 0.0, "dur": 0.0})
    a["calls"] += int(r["calls"]); a["skips"] += int(r["skips"]); a["self_ms"] += f(r["self_ms"])
    a["dur"] += dur_by_interval.get(int(r["interval"]), 0)
ab1_dur = sum(v["dur"] for v in per.values()) / max(len(per), 1)
print(f"\n=== per-patch (ab=1; ~{ab1_dur:.0f}s wall)")
print(f"{'id':16s} {'calls':>9s} {'skips':>9s} {'skip%':>6s} {'self_ms':>9s} {'ms/s':>9s} {'us/call':>8s}")
for k, a in sorted(per.items(), key=lambda kv: -kv[1]["self_ms"]):
    if a["calls"] == 0 and a["self_ms"] == 0: continue
    pct = a["skips"] * 100.0 / a["calls"] if a["calls"] else 0.0
    ms_s = a["self_ms"] / max(a["dur"], 1)
    us = a["self_ms"] * 1000.0 / a["calls"] if a["calls"] else 0.0
    print(f"{k:16s} {a['calls']:9d} {a['skips']:9d} {pct:5.1f}% {a['self_ms']:9.1f} {ms_s:9.3f} {us:8.2f}")
