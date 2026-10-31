import csv, statistics as st, collections

D = r"C:\Users\chiqingsan\AppData\LocalLow\Questing Goose Studio\Probably Stolen"
rows = list(csv.DictReader(open(D + r"\SmoothDragAccel-perf-summary.csv", encoding="utf-8-sig")))
def f(x):
    try: return float(x)
    except: return 0.0
prev = 0.0
for r in rows:
    t = f(r["time_s"]); r["dur"] = max(t - prev, 0.001); prev = t
    r["t0"] = t - r["dur"]

drows = list(csv.DictReader(open(D + r"\SmoothDragAccel-perf-detail.csv", encoding="utf-8-sig")))
dur_by = {int(r["interval"]): r["dur"] for r in rows}
t0_by = {int(r["interval"]): r["t0"] for r in rows}

# long phases only: ON t=259..1364, OFF t=1364..2393, ON t=2393..2571
phases = [("ON_long", 259.2, 1364.6, "1"), ("OFF_long", 1364.6, 2392.9, "0"), ("ON_tail", 2392.9, 9999, "1")]
for name, a, b, ab in phases:
    g = [r for r in rows if a <= r["t0"] < b and r["ab"] == ab]
    dur = sum(r["dur"] for r in g); frames = sum(int(r["frames"]) for r in g)
    if not g: continue
    p95 = [f(r["p95_ms"]) for r in g]; p50 = [f(r["p50_ms"]) for r in g]
    over = sum(int(r["over_33ms"]) for r in g)
    print(f"\n=== {name}: {len(g)} intervals {dur:.0f}s {frames} frames")
    print(f"  fps {frames/dur:6.1f} | p50 med {st.median(p50):5.2f} | p95 med {st.median(p95):5.2f} max {max(p95):5.1f} | >33/min {over/dur*60:5.2f}")
    print(f"  alloc med {st.median([f(r['alloc_kb_per_s']) for r in g]):5.0f} KB/s | gc0/min {sum(int(r['gc0']) for r in g)/dur*60:5.2f} | patch {sum(f(r['patch_ms']) for r in g)/dur:5.2f} ms/s")
    # workload fingerprint per minute
    ids = ["ContextMenu", "ContainerCold", "RaycastCache", "StoreHover", "PeekBlock", "TooltipIdle",
           "TextFieldFocus", "FastSlot", "DragFix", "ValidateSkip", "LayoutSuppress", "SlotIndex"]
    per = {}
    for r in drows:
        if int(r["interval"]) not in [int(x["interval"]) for x in g]: continue
        per[r["id"]] = per.get(r["id"], 0) + int(r["calls"])
    print("  workload/min: " + "  ".join(f"{i}={per.get(i,0)/dur*60:.0f}" for i in ids))

# top hitching intervals
print("\n=== top over_33ms intervals")
for r in sorted(rows, key=lambda r: -int(r["over_33ms"]))[:8]:
    print(f"  t={f(r['time_s']):7.1f}s int={r['interval']:>3s} ab={r['ab']} frames={r['frames']:>5s} fps={r['fps']:>6s} p50={r['p50_ms']:>5s} p95={r['p95_ms']:>5s} p99={r['p99_ms']:>5s} over33={r['over_33ms']:>3s}")
