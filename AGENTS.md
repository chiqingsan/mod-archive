# 工作区记忆

## 上传与发布（硬性）

- 上传 Steam 工坊（`workshop_build_item`）或任何对外发布，必须由用户在当次对话中明确许可（例如“上传吧”）。
- 不得因为此前授权过、任务描述里提到“重传/更新工坊”、或用户同意了某项改动，就推断为上传授权。
- 改完代码后的默认动作：构建、测试、报告改动与结果，然后停下等用户指示；未经允许不要上传。
- 上传前先说明待上传内容（版本号、DLL 哈希、manifest 权限、VDF 是否只含 appid/publishedfileid/contentfolder），等用户确认后再执行。
- 本机 Mods 目录部署与上传是两回事：部署也要等用户说“部署一下”再做。
- 反面案例：2026-10-31 用户只选择了“移除诊断写盘和 files 权限”，未授权发布，却自行上传了 1.2.1 到工坊。此类推断一律不允许。

## 本地更新（硬性）

- 本地 Mods 目录更新时，除 DLL 外还要同步更新 manifest.xml、preview.png 等随包文件，保持与项目/发布内容一致；不要只替换 DLL。
- 更新前备份被覆盖的文件；更新后核对：DLL 哈希、manifest 的 `<ModVersion>` 与 `<Capability>`、preview.png 哈希。
- 本地目录结构参照其他 mod：`DLL + manifest.xml + preview.png` 三件套，不额外放置无关文件（备份文件除外）。
- 备份文件不要留在 Mods 模组目录内：游戏模组扫描会把目录里的 `.dll.bak` 等可执行内容判为未声明 payload 并弹出“未通过模组扫描检查”。备份统一放到 Mods 之外的目录，例如 `D:\code\mod\Probably Stolen\SmartInventoryArrange-Final\deploy-backups\`。
