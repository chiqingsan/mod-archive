"""Read-only inspection of the shipped native render node's enabled flags.

Unity keeps these MonoBehaviour base fields even without custom type trees:
GameObject PPtr (12 bytes), enabled + padding (4), MonoScript PPtr (12).
"""
import struct
from pathlib import Path
import UnityPy
from UnityPy.classes import PPtr

assets = Path(r"D:\app\steam\steamapps\common\Probably Stolen\Probably Stolen_Data\sharedassets1.assets")
env = UnityPy.load(str(assets))
prefab = next(obj for obj in env.objects if obj.type.name == "GameObject" and obj.read().m_Name == "RenderNodePrefab")
flags = {}
for component in prefab.read().m_Component:
    if component.component.type.name != "MonoBehaviour":
        continue
    raw = component.component.deref().get_raw_data()
    script = PPtr(m_FileID=struct.unpack_from("<i", raw, 16)[0],
                  m_PathID=struct.unpack_from("<q", raw, 20)[0], assetsfile=prefab.assets_file).read()
    flags[script.m_ClassName] = bool(raw[12])
assert flags["Image"] is False, "Native prefab contract changed: revisit caller initialisation"
assert flags["TreeNodeRender"] is True
print("Verified native RenderNodePrefab: Image disabled, TreeNodeRender enabled. Caller must explicitly enable Image.")
