import csv, re, collections

D = r"C:\Users\chiqingsan\AppData\LocalLow\Questing Goose Studio\Probably Stolen"
L = D + r"\GameLog_2026-10-31_23-55-55.txt"
rows = list(csv.DictReader(open(D + r"\SmoothDragAccel-perf-summary.csv", encoding="utf-8-sig")))
def f(x):
    try: return float(x)
    except: return 0.0
prev = 0.0
for r in rows:
    t = f(r["time_s"]); r["dur"] = max(t - prev, 0.001); prev = t; r["t0"] = t - r["dur"]

# convert phase windows (relative t) to wall clock; log starts 23:55:55
import datetime
base = datetime.datetime(2026, 10, 31, 23, 55, 55)
def wall(t):
    return (base + datetime.timedelta(seconds=t)).strftime("%H:%M:%S")

print("phase windows:", "ON", wall(259.2), "-", wall(1364.6), "| OFF", wall(1364.6), "-", wall(2392.9), "| ON2", wall(2392.9), "-end")

# log activity per minute
cts = collections.Counter()
for line in open(L, encoding="utf-8", errors="ignore"):
    m = re.match(r"\[(\d\d):(\d\d):(\d\d)\]", line)
    if m:
        h, mi = int(m.group(1)), int(m.group(2))
        cts[(h, mi)] += 1

def seg(a, b, h0=23, m0=55):
    # a,b are seconds relative to base
    t0 = base + datetime.timedelta(seconds=a); t1 = base + datetime.timedelta(seconds=b)
    tot = 0; cur = t0.replace(second=0, microsecond=0)
    while cur < t1:
        if cur >= t0:  # only count minutes fully/partially in window
            tot += cts.get((cur.hour, cur.minute), 0)
        cur += datetime.timedelta(minutes=1)
    return tot / max((b - a) / 60.0, 1e-9)

print("log lines/min  ON_long:", round(seg(259.2, 1364.6), 1), " OFF_long:", round(seg(1364.6, 2392.9), 1), " ON_tail:", round(seg(2392.9, 2571), 1))

# per-interval fps distribution in each long phase
def stats(a, b):
    g = [r for r in rows if a <= r["t0"] < b]
    fps = [f(r["fps"]) for r in g]
    low = [x for x in fps if x < 100]
    print(f"  n={len(g)} fps min/med/max = {min(fps):.0f}/{sorted(fps)[len(fps)//2]:.0f}/{max(fps):.0f}; <100fps intervals = {len(low)}")
    hh = [f(r["over_33ms"]) for r in g]
    print(f"  over33 median={sorted(hh)[len(hh)//2]} max={max(hh)} sum={sum(hh)}")
    big = [r for r in g if int(r["over_33ms"]) >= 20]
    print("  intervals with over33>=20:", [(r["interval"], wall(r["t0"]), r["fps"], r["over_33ms"]) for r in big][:12], "..." if len(big) > 12 else "")

print("\nON_long:"); stats(259.2, 1364.6)
print("OFF_long:"); stats(1364.6, 2392.9)
print("ON_tail:"); stats(2392.9, 2571)
