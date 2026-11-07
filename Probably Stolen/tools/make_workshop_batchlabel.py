import hashlib
import shutil
from pathlib import Path

root = Path(r"D:\code\mod\Probably Stolen")
proj = root / "BatchLabel-Final"
wu = root / "WorkshopUpload"

title = "标签机批量打标签 | Batch Labeling"

# 用户审阅后的最终版（以用户提供版本为准，2026-11-01 21:45）
desc = """[h1]标签机批量打标签 | Batch Labeling[/h1]
装备标签机，按住触发键在物品上拖出选择框，松开就能把框内同一种类的物品批量贴成起点物品当前显示的标签。一次框选，替代几十次「打开标签机 → 逐条下拉 → 关闭」。

[h2]用法[/h2]
[list]
[*]先用标签机给第一件物品贴好想要的标签（这一步和原版一样）。
[*]装备标签机，按住触发键（默认 [b]空格[/b]），在这件物品上按住左键拖出选择框。
[*]松开左键，框内与它同一种类的物品全部更新为该物品[b]当前显示[/b]的标签。
[/list]
起点物品就是模板：在框选的范围中，所有和模板同类型的物品都会被打上同样的标签。
[img]https://i.postimg.cc/kgkV2B3c/Pix-Pin-2026-11-01-21-41-43.gif[/img]

[list]
[*]只处理与起点同一种类的物品，框内其他物品不受影响。
[*]只处理归你所有的物品（与原版标签机一致），顾客手里和柜台对面的物品不会被改动。
[*]只复制标签（物品显示条件），不改动真实条件，也不碰交易结算逻辑；已经是同样标签的物品不会重复写入，避免重复触发改名、改价等附带效果。
[*]水质容器会连同「假装的水质数值」一起复制，显示与数值保持一致。
[*]起点物品没有可编辑标签时，本次框选不改动任何物品。
[*]不按触发键时，所有操作与未安装模组一致。
[/list]

[h2]修改快捷键[/h2]
游戏设置 → [b]按键[/b] → 「修改标签机批量打标签的快捷键」→ 点击后按新键 → 点「应用」立即生效。

[h2]性能[/h2]
常态静默，只在按下触发键和框选落定时工作，无性能影响。"""

assert '"' not in desc and "\\" not in desc, "简介里不能有 ASCII 引号或反斜杠"

# 1) 封面（与项目 preview.png 一致，1080x635，1MB 以内）
shutil.copyfile(proj / "preview.png", wu / "BatchLabel-cover.png")

# 2) 上传内容目录（DLL + manifest + preview）
content = wu / "BatchLabel-1.0.2-Content"
content.mkdir(exist_ok=True)
shutil.copyfile(proj / "bin/Release/netstandard2.1/BatchLabel.dll", content / "BatchLabel.dll")
shutil.copyfile(proj / "manifest.xml", content / "manifest.xml")
shutil.copyfile(proj / "preview.png", content / "preview.png")

# 3) BBCode 介绍
(wu / "BatchLabel.bbcode").write_text(desc, encoding="utf-8")


# 4) 首发 VDF（尚未上传，暂无 publishedfileid）
def vdf_escape(s: str) -> str:
    return s.replace("\\", "\\\\").replace('"', '\\"')


vdf = (
    '"workshopitem"\n'
    "{\n"
    '\t"appid"\t\t"4348910"\n'
    f'\t"contentfolder"\t\t"{vdf_escape(str(content))}"\n'
    f'\t"previewfile"\t\t"{vdf_escape(str(wu / "BatchLabel-cover.png"))}"\n'
    '\t"visibility"\t\t"0"\n'
    f'\t"title"\t\t"{vdf_escape(title)}"\n'
    f'\t"description"\t\t"{vdf_escape(desc)}"\n'
    '\t"changenote"\t\t"首次发布：版本 1.0.2，适配游戏 0.46D。"\n'
    "}\n"
)
(wu / "BatchLabel.vdf").write_text(vdf, encoding="utf-8")


def info(p: Path) -> str:
    b = p.read_bytes()
    return f"{p.name}: {len(b)} B sha256={hashlib.sha256(b).hexdigest()}"


for p in (
    wu / "BatchLabel-cover.png",
    content / "BatchLabel.dll",
    content / "manifest.xml",
    content / "preview.png",
    wu / "BatchLabel.bbcode",
    wu / "BatchLabel.vdf",
):
    print(info(p))

# 一致性核对：VDF 里的 description 反转义后应等于 bbcode 内容
marker = '\t"description"\t\t"'
start = vdf.index(marker) + len(marker)
end = vdf.index('"\n\t"changenote"', start)
body = vdf[start:end].replace("\\\\", "\\").replace('\\"', '"')
print("desc_match:", body == desc, "chars:", len(desc))
