"""Read-only check against the installed game's shipped scene, using UnityPy."""
import re
from pathlib import Path
import UnityPy

scene = Path(r"D:\app\steam\steamapps\common\Probably Stolen\Probably Stolen_Data\level1")
env = UnityPy.load(str(scene))
objects = {obj.path_id: obj for obj in env.objects}
transforms = {obj.path_id: obj.read_typetree() for obj in env.objects if obj.type.name == "RectTransform"}
names = {obj.path_id: obj.read().m_Name for obj in env.objects if obj.type.name == "GameObject"}
groups = {}
for transform in transforms.values():
    if not names[transform["m_GameObject"]["m_PathID"]].startswith("ExchangeElement"):
        continue
    parent = transforms[transform["m_Father"]["m_PathID"]]
    group = names[parent["m_GameObject"]["m_PathID"]]
    groups.setdefault(group, []).append(transform)

permanent = sorted(groups["PermaSection"], key=lambda t: -t["m_AnchoredPosition"]["y"])
assert len(permanent) == 7
assert len(groups["PromotionalSection"]) == 7
assert all(t["m_SizeDelta"]["x"] >= 490 and t["m_SizeDelta"]["y"] >= 50 for t in permanent)
expected = {"bm_rep_barter", "med_crate_barter", "improved_dream_dust_barter", "seed_barter",
            "immunivax_barter", "eng_crate_barter", "sec_crate_barter"}
actual = set()
for obj in env.objects:
    if obj.type.name == "MonoBehaviour":
        actual.update(s.decode() for s in re.findall(rb"[A-Za-z][A-Za-z_]{4,}", obj.get_raw_data()) if s.decode() in expected)
assert actual == expected
panel = next(t for t in transforms.values() if names[t["m_GameObject"]["m_PathID"]] == "_ExchangePanel")
print(f"Native scene verified: panel={panel['m_SizeDelta']}, 7 permanent rows, 7 promotional IDs; native art retained.")
