# Steam 创意工坊介绍更新流程

适用于本目录下所有模组的介绍、更新说明和发布配置维护。

1. 检查 Git 状态及已有改动，确认目标模组和 Workshop ID，不覆盖用户修改。
2. 每次更新前，从 Steam 获取目标模组当前已发布的完整介绍。优先使用公开接口 `ISteamRemoteStorage/GetPublishedFileDetails/v1/`，POST 参数为 `itemcount=1`、`publishedfileids[0]=目标 ID`，确认结果成功并读取 `description`。页面查看可作补充。读取失败时如实说明，不把本地旧介绍当作线上最新版本。
3. 以线上原文为基础保留现有内容及历史更新记录，加入本次版本的更新说明。仅对本次功能变化涉及的正文做必要修正，不擅自重写整篇介绍。
4. 更新说明须对应实际完成的代码改动。区分已验证修复、兼容性风险缓解和待验证反馈；未复现的问题不能写成已修复，不承诺兼容所有模组。
5. 将完整介绍保存为对应的 `.bbcode` 文件，保持 Steam BBCode 格式；同步对应 `.vdf` 的 `description` 和本次 `changenote`，正确转义 VDF 字符串。保留已有 Workshop ID、内容目录、封面等无关字段。
6. 检查差异及 BBCode、VDF 介绍是否一致，确认没有遗漏线上原文或重复添加同一版本说明。
7. 在聊天中用 BBCode 代码块发送完整介绍供用户审阅，并提供本地文件链接，说明线上来源与实际改动。
8. 只有用户明确要求“上传”或“更新 Steam 创意工坊”时才执行发布。整理文案、更新本地文件、部署到本机游戏目录和请求审阅均不代表授权上传。

## BBCode 换行约定

- 标题使用 `[h1]`、`[h2]` 或 `[h3]` 时，结束标签后只保留一个实际换行，下一行直接写正文或列表，不额外插入空白行。标题本身已有块级间距，额外空行会让 Steam 展示间距过大。
- 正确示例：

  ```bbcode
  [h1]点击推进对话[/h1]
  在对话时点击鼠标左键，即可显示完整的当前文字或推进对话。
  ```

- 不要将上面的内容改成标题后两个换行（`[/h1]\n\n正文`）。此处 `\n` 仅用于说明换行数量；实际 `.bbcode` 和 VDF 描述应存储真实换行，不写入字面量反斜杠加 n。
- BBCode 不按 Markdown 的排版习惯给每个标题额外添加空行。普通段落之间按需保留空行，避免对全部换行进行无差别压缩。
- 从 Steam API 读取介绍时正常解码 JSON 一次；写入 VDF 时按 VDF 字符串规则转义，避免重复转义造成字面量 `\n`。同步后核对 `.bbcode` 与解码后的 VDF `description` 一致。
- 用户提供审阅修改时以其版本为准；保留用户对尚未定位故障的说明，不擅自改成“已修复”。

## AquaSense

- Workshop ID：`3828878417`
- 线上页面：https://steamcommunity.com/sharedfiles/filedetails/?id=3828878417
- 本地介绍：`AquaSense.bbcode`
- 本地发布配置：`AquaSense.vdf`
- 更新记录：
  - 2026-11-04 17:50，版本 1.9.0：新增“韧棉织物耗尽后自动更换”（默认开启，可单独关闭）；配置 `AquaSense-3828878417-1.9.0.vdf`（仅 appid/publishedfileid/contentfolder，不含 title/description/previewfile/changenote → 封面、简介、更新公告均未改）；内容目录 `Mods/AquaSense`（`AquaSense.dll` 36,352 B `a089a188…`、manifest 1.9.0 857 B `da9f036e…`、preview `9b5874a9…`）；线上 `GetPublishedFileDetails`：result=1、`file_size` 2,598,586、`time_updated` 2026-11-04 17:50:59（本地时区）、`preview_url` 未变、`title`/`description` 未变。
  - 2026-11-07 00:10，版本 1.10.2：新增只读成分数据接口（`aquasense.composition.v1` 数据槽 + `aquasense.composition.version.v1` 版本槽，供《物品详情美化》等模组读取成分行数据），并修复 tooltip 文本被清空时的空引用异常、进度条隐藏改为延迟到下一帧；更新说明见 `AquaSense-1.10.2-update.bbcode`；配置 `AquaSense-3828878417-1.10.2.vdf`（仅 appid/publishedfileid/contentfolder，不含 title/description/previewfile/changenote → 封面、简介、更新公告均未改）；内容目录 `AquaSense-1.10.2-Content/`（`AquaSense.dll` 51,712 B `7ac2269c…`、manifest 1.10.2 1,496 B、preview 2,561,377 B `9b5874a9…`）；线上 `GetPublishedFileDetails`：result=1、`file_size` 2,614,585、`time_updated` 2026-11-07 00:10:02（本地时区）、`title`/`description`/`preview_url` 未变。

## 按键拖拽多选框（BatchSelect）

- Workshop ID：`3830936204`
- 线上页面：https://steamcommunity.com/sharedfiles/filedetails/?id=3830936204
- 本地介绍：`BatchSelect.bbcode`
- 本地发布配置：`BatchSelect.vdf`
- 首次发布：2026-10-31，版本 1.0.0，适配游戏 0.46D。

## 标签机批量打标签（BatchLabel）

- Workshop ID：`3831660460`
- 线上页面：https://steamcommunity.com/sharedfiles/filedetails/?id=3831660460
- 本地介绍：`BatchLabel.bbcode`
- 本地发布配置：`BatchLabel.vdf`；首发版本化配置：`BatchLabel-3831660460-1.0.2.vdf`
- 内容目录：`BatchLabel-1.0.2-Content/`（`BatchLabel.dll` 16,896 B `ffdacffb…`、manifest 1.0.2 930 B `e0dc971c…`、preview `995582 B` `6f5fb087…`；线上 `file_size` 1,013,408 = 三件套之和）
- 封面：`BatchLabel-cover.png`（1080×635，995,582 B，1MB 以内）
- 工坊标题：「标签机批量打标签 | Batch Labeling」
- 首次发布：2026-11-01，版本 1.0.2，适配游戏 0.46D，`visibility 0`（公开）。
- 线上核对：`GetPublishedFileDetails` result=1、标题一致、`preview_url` 正常、简介与本地 `.bbcode` 仅换行符差异（Steam 存为 CRLF）。
- 更新记录：
  - 2026-11-07 20:06，版本 1.0.3：新增「标签记忆」——给物品打标签时按物品类型记住刚设定的标签（多个标签分类各记各的），之后装备标签机直接右键同类物品即按记忆重贴并保持装备，记忆存进模组配置、重启不丢；未新增文件权限（仍只有 reflection-external）。简介以线上原文为底稿，追加开头一句、新增「标签记忆（右键快捷重贴）」小节、性能一句补充右键套用。配置 `BatchLabel-3831660460-1.0.3.vdf`（含 `description` + `changenote`，不含 title/previewfile → 标题与封面未改）；内容目录 `BatchLabel-1.0.3-Content/`（`BatchLabel.dll` 23,552 B `E8F8A71F…`、manifest 1.0.3 1,134 B `323D40E0…`、preview 995,582 B `6F5FB087…`）；SteamCMD 返回 Success；线上 `GetPublishedFileDetails`：result=1、`file_size` 1,020,268 = 三件套之和、`time_updated` 2026-11-07 20:06:59（本地时区）、`description` 与本地 `.bbcode` 一致、`title`/`preview_url` 未变、`visibility` 0。更新说明见 `BatchLabel-1.0.3-update.bbcode`。

## 深空当铺 · 更好的性能优化（BetterPerformance）

- Workshop ID：`3831227725`
- 线上页面：https://steamcommunity.com/sharedfiles/filedetails/?id=3831227725
- 本地介绍：`BetterPerformance.bbcode`
- 首发配置（含标题/介绍/可见性）：`BetterPerformance.vdf`
- 更新配置（后续更新只用这个）：`BetterPerformance-3831227725-1.0.0.vdf`
- 内容目录：`BetterPerformance-1.0.0-Content/`（`BetterPerformance.dll` 86,016 B `f6c01a98…`、manifest 1.0.0 869 B `d147490f…`、preview `d586868b…`）
- 封面：`BetterPerformance-cover.jpg`（1280×720）
- 首次发布：2026-11-01，版本 1.0.0，适配游戏 0.46D。
- 更新记录：
  - 2026-11-02 04:55，版本 1.0.8：配置 `BetterPerformance-3831227725-1.0.8.vdf`（仅 appid/publishedfileid/contentfolder/changenote，不含 title/description/previewfile → 封面与简介未改）；内容目录 `BetterPerformance-1.0.8-Content/`（DLL 102,400 B `a5ea5f0d…`、manifest 1.0.8 872 B `4aafdb23…`、preview `d586868b…`）；线上 `GetPublishedFileDetails`：result=1、`file_size` 611,368、`time_updated` 2026-11-01 20:55:38 UTC、`preview_url` 未变。

## 游戏快捷键增强/更好的操作体验（QuickCounter）

- Workshop ID：`3828875719`
- 线上页面：https://steamcommunity.com/sharedfiles/filedetails/?id=3828875719
- 本地介绍：`QuickCounter.bbcode`
- 本地发布配置：`QuickCounter.vdf`（旧版）；最新更新配置：`QuickCounter-3828875719-1.2.0.vdf`
- 内容目录：`QuickCounter-1.2.0-Content/`（`QuickCounter.dll` 16,384 B `0b2ef1e2…`、manifest 1.2.0 659 B、preview 903,465 B）
- 封面：内容包内 preview.png（1024×576，压缩到 1MB 内）
- 首次发布：2026-10-29，适配游戏 0.46D。
- 更新记录：
  - 2026-11-02 11:59，版本 1.2.0：配置 `QuickCounter-3828875719-1.2.0.vdf`（含 title/description，简介追加「更新记录 / 1.2.0」段；changenote 说明 C 键批发价修复）；内容目录 `QuickCounter-1.2.0-Content/`；线上 `GetPublishedFileDetails`：result=1、`file_size` 920,508、`time_updated` 2026-11-02 11:59:39（本地时区）、简介与本地 `.bbcode` 一致。

## 地下交易材料单（UndergroundExchangeAssistant）

- Workshop ID：`3832460122`
- 线上页面：https://steamcommunity.com/sharedfiles/filedetails/?id=3832460122
- 本地介绍：`UndergroundExchangeAssistant.bbcode`
- 基础发布配置：`UndergroundExchangeAssistant.vdf`（首次发布用，无 publishedfileid）
- 更新配置：`UndergroundExchangeAssistant-3832460122-1.1.14.vdf`（封面更新，仅 appid/publishedfileid/contentfolder/previewfile/changenote）
- 内容目录：`UndergroundExchangeAssistant-1.1.14-Content/`（`UndergroundExchangeAssistant.dll` 1,082,880 B `78590d1b…`、manifest 1.1.14 839 B `770c9732…`、preview `A4F43866…`；线上 `file_size` 3,299,416 = 三件套之和）
- 封面：`UndergroundExchangeAssistant-cover-user.png`（613×540，181,572 B，使用游戏内垃圾桶材料单实机截图）
- 工坊标题：「地下交易材料单 | Underground Exchange Material Sheet」
- 首次发布：2026-11-02，版本 1.1.14，适配游戏 0.46D，`visibility 0`（公开）。
- 线上核对：`GetPublishedFileDetails` result=1、标题一致、`visibility=0`、`description` 与本地 `.bbcode` 一致（含 BBCode 标记）、`preview_url` 已更新为实机截图封面。
- 更新记录：
  - 2026-11-03 00:47，版本 1.1.15：配置 `UndergroundExchangeAssistant-3832460122-1.1.15.vdf`（含 description + previewfile + changenote：「1.1.15：展示柜陈列品不再参与取材；水壶等容器里已安装的过滤器不再被拆出。」）；内容目录 `UndergroundExchangeAssistant-1.1.15-Content/`（`UndergroundExchangeAssistant.dll` 1,083,392 B `d6df9c69…`、manifest 1.1.15 826 B `a41513a8…`、preview `a4f43866…`）；线上 `GetPublishedFileDetails`：result=1、`file_size` 3,299,915、`time_updated` 2026-11-03 00:47:19（本地时区）、`description` 与本地 `.bbcode` 一致、`preview_url` 未变。
  - 2026-11-03 00:47，本地部署：Mods 目录更新为 1.1.15 三件套（DLL/manifest/preview 哈希与 package 一致、ModVersion 1.1.15、Capabilities=reflection-external），旧 1.1.14 备份于 `deploy-backups/20261103-004755-1.1.14/`。

## 储水罐贴图优化（WaterTankLevel）

- Workshop ID：`3833754830`
- 线上页面：https://steamcommunity.com/sharedfiles/filedetails/?id=3833754830
- 首次公开发布：2026-11-05，版本 1.1.5，适配游戏 0.46D。
- 首发配置：`WaterTankLevel.vdf`（含标题、简介、封面、可见性）。
- 后续仅内容更新配置：`WaterTankLevel-3833754830-1.1.5.vdf`（仅 appid/publishedfileid/contentfolder）。
- 内容目录：`WaterTankLevel-1.1.5-Content/`；简介：`WaterTankLevel.bbcode`；封面：`WaterTankLevel-cover.png`（960×540，840320 B）。
- DLL SHA-256：`868F7301D58FC2A49A686B99D887ED8881FB0F7173FA5FB9CFA4DB903D6220C7`，权限 reflection-external。
- SteamCMD 返回 Success；GetPublishedFileDetails result=1，visibility=0，标题及简介一致，file_size=862036 与本地包一致，preview_url 已生成。未验证订阅下载及游戏运行。
- 更新记录：
  - 2026-11-05 18:34，版本 1.1.6：配置 `WaterTankLevel-3833754830-1.1.6.vdf`（仅 appid/publishedfileid/contentfolder/changenote，不含 title/description/previewfile → 封面与简介未改）；内容目录 `WaterTankLevel-1.1.6-Content/`（`WaterTankLevel.dll` 8,192 B `6a8a6f7c…`、manifest 1.1.6 684 B、preview `20ddaac4…`，共 19 文件 862,550 B）；changenote「1.1.6：新建的储水罐创建完成后补刷一次桶身与拖拽镜像，修复首次显示原版贴图的问题。」；线上 `GetPublishedFileDetails`：result=1、`file_size` 862,550、`time_updated` 2026-11-05 18:34:31（本地时区）、`title`/`description` 未变、`visibility` 0。

AquaSense 1.12.0（2026-11-07 上传）：阴沟水及以下（含未知水）文字色改 #C73E3A；新增「显示水质详情」开关（默认开启）。VDF 仅含 appid/publishedfileid/contentfolder，未改封面/简介/公告；ManifestID 3681480890279487067，file_size 2618076。
