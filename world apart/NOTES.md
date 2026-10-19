# 《不问凡尘》(World Apart) mod 开发笔记

> **状态: ✅ 首个 mod 已完成并实测生效**（2026-10-02 定稿）。
> 游戏: Steam AppID 4209920, `D:\app\steam\steamapps\common\A1`, 主程序 `WorldApart.exe`,
> Unity 2022.3.43f1 (IL2CPP), 发行商 Nuverse, 实测版本 v0.34.7343955。

---

## 0. 已交付: `local_infinite_needle`（疏经导脉·无限行针）

### 文件
```
C:\Users\chiqingsan\AppData\LocalLow\Nuverse\WorldApart\Mods\local_infinite_needle\
├── mod.json                              # schemaVersion 5, modId local_infinite_needle, units []
├── preview.png                           # 16:9, 工坊 UI 卡片用
└── tables\
    ├── tbminigamemedical.patch.json      # 行针: 36 关 rotation_limit 60 → 9999
    ├── tbnpcinteractgameentry.patch.json # 精力: 1335 行 costStamina 10 → 0 (小游戏)
    ├── tbsceneharvest.patch.json         # 精力: 242 行 staminaCost 5 → 0 (采集)
    ├── tbconstants.patch.json            # 精力: 双修20/说服10/线索12/炼丹5 → 0 (实验性 keyFields)
    ├── tbbattletype.patch.json           # 战败复活: 全类型 revivedHp/MpRatio 0.3/0.1 → 0.5 (切磋 -1 保留)
    └── tbpillrecipe.patch.json           # 续航: 金砂散/回春散 main_effects 三档 → 80%
```
开发副本: `D:\code\mod\world apart\infinite_needle\`（与部署副本内容一致）。

### 精力免耗（2026-10-03 新增）
用户要求精力等效 99999。精力**当前值在存档里**（table_patch 不可达），采用"全消耗归零"实现等效无限：
- 消耗点位全清单: 小游戏 `costStamina`=10（tbnpcinteractgameentry, 主键 npcId+subId 已验证唯一）、采集 `staminaCost`=5（tbsceneharvest, 主键 id）、双修 `DUAL_CULTIVATE_STAMINA_COST`=20 / 说服偷窥 `PERSUADE_PEEK_STAMINA_COST`=10 / 线索拼酒 `CLUES_COST_STAMINA`=12 / 炼丹 `REFINING_PILLS_STAMINA_COST`=5（tbconstants 单行常量表）
- **[实验性]** tbconstants 无 id 字段，keyFields 借用 `movie_name_length_limit`；若被拒只作废此 patch（小游戏/采集仍生效），届时换策略
- 属性 200 = 精力（正式名）；客栈全恢复列表 `HOTEL_STAY_FULLY_ATTR_ID_LIST: "1,25,200"`（1=生命, 25=灵力, 200=精力）；精力上限/自然恢复无表配置（代码逻辑）
- 属性速查（tbattribute）: 1=生命, 4=生命上限, 22=灵力上限, 25=灵力, 200=精力, 5=幸运, 6=魅力

### 天赋效果路线（2026-10-03, 精力/战后续航的正解）
- **tbexploreleveloutgameeffect = 天赋树节点效果表**（sourceType 4; sourceId 分系: 1xx 神识/2xx 炼器/3xx 商贾/4xx 善恶/5xx 宗门/6xx 体魄/7xx 情缘/8xx 魂幡/9xx 魔道/1001+ 神识；效果数值在**解锁瞬间固化进存档**——StaminaMaxOutGameEffectData 是 MessagePack 序列化类, 改表只影响未来解锁）。
- **精力上限 99999**: 点亮神识系天赋 4 节点（神意初生 4001001/精元充盈 4001006/神魂壮大 4001009/元神凝练 4001011, unlockValue 已降至 1, 数值已改 99999）。
- **战后恢复 50% 生命**: 体魄系天赋「战罢回元」4000604, 表达式 `战斗["胜利恢复生命"](100→500)`（千分比, 100=10%）。**无"胜利恢复灵力"动词**（字面量验证）——灵力续航靠回春散 80%。
- 表达式格式: `effectExprs[0].func[1] = {target: [variable {name: 命名空间}], index: [string {value: 动词名}], arguments: [["int", {value: N}]]}`; 一个节点可多效果（401 先例: effectExprs 数组多项）。
- 灵力上限天赋: 灵府盈溢/灵渊深蓄（神识 10/20 级, +100）未改。
- 实测状态矩阵（2026-10-03）: 行针 9999 ✅ 生效; 采集免耗 ❌（还扣 5）; 丹药 80% ❌; 天赋效果 ⏳ 待点亮验证。**生效规律**: Medical 小游戏生成器读表 ✓; 采集/丹药运行时数值不读 Luban 表（XML 或代码内嵌）。教训: 改表前先确认消费系统真的读这张表。
- HotTables 仅 3 张: tbnpcbasecfg / tbnpcaipersona / tbstrconfig; 其余 Reenter 类, 但行针（Reenter）生效 → Hot/Reenter 不是决定因素, 消费端才是。
- MergeSettings: `MergeArrayHandling = Replace(2)`, DefaultKeyFields = ["id"]（ModTablePatcher..cctor 反汇编）。

### 战斗续航（2026-10-03 终局: 丹药 80%）
- **无效路线（两次实测）**: tbskilllevel 的 `affects`/`buffs` 字段不是战斗运行时数据——技能真实效果（含治疗量）存在 `xmlLink` 指向的 **XML 能力定义**（`Assets/XMLAbilities/...`, 打包在 bundle 资源里, 非 Luban 表）。表里 affects 只有部分伤害, 治疗量（如魔修男 9.5%）全在 XML。改表 buffs（挂原版 Buff 4, 500 点）与新增 buff 行均无效果。**技能层效果修改超出 table_patch 能力**。
- **有效路线（待实测）**: `tbpillrecipe` 的 `main_effects: [{type:2, values:[百分比, 目标槽]}]` 是**百分比恢复**且表驱动（用户确认游戏内为百分比回复）。金砂散(values[1]=1)/回春散(values[1]=2) 分别对应生命/灵力条, 三档品质(main/enhanced/shen)全部改为 80%。
- 丹药知识: 回春散 item 180002 / 金砂散 item 180001（useEffectId 同号, 动词"战斗丹药"参数=物品编号）; tbpilleffect（荣生5点/引灵10点）是**探索玩法**小回复丹, 非战斗丹药, 别混淆。
- 备选路径（未采用）: tbscenerecover 打坐点改比例（大世界续航）。

### 小游戏跳过实验（2026-10-03 失败, 已回滚）
- 方案: tbminigamemedical 36 关极小化（1×2 板/起点终点相邻/干扰概率清零）→ **小游戏界面不出现**（生成器对异常参数静默失败, 中断启动流程, 连 UI 都不起）。
- 教训: 小游戏启动链对盘面参数有未文档化的隐式约束（表校验只查 >=1/坐标在板内, 实际生成器更严）。极小盘不可行; 无"跳过"开关; 改 interactGameType 类型风险高。**小游戏跳过在 table_patch 内无解**, 行针 9999 已保证不会失败（可玩但必胜）。
- 回滚: 棋盘参数恢复原版, 仅保留 rotation_limit 9999。

### 生效三要素（缺一不可，都是实测踩出来的）
1. **补丁文件名必须是 `<表名>.patch.json`** —— 普通 `.json` 被 `EnumerateIn` 静默忽略（`ModTablePatch.FileSuffix = ".patch.json"`）。
2. **mod.json 合法**（schemaVersion ≤ 5；modId 字符集 = 字母数字 `-._`，不许冒号）。
3. **注册表启用条目** —— table_patch 单元和化身一样需要"启用"，但 UI 没有入口，只能写注册表（见 §3）。

### 数值调整 / 回滚
- 改行针上限: 编辑补丁文件里 36 行的 `rotation_limit`（合法范围: `-1`=原生无上限, 或 `>=1`）→ 重启游戏。
- 彻底回滚: 删除 `Mods\local_infinite_needle\` 整个目录 + 从注册表值
  `HKCU\Software\Nuverse\WorldApart` 的 `MOD_ENABLED_ENTRIES_V2_h2913630511`（REG_BINARY）里删掉
  `local_infinite_needle:tables` 那一行。**改注册表必须在游戏关闭时**（Unity 退出时 flush 会覆盖）。

---

## 1. 资源解密与配置表提取（字节级事实，可自查）

### YooAsset bundle 自定义头
`StreamingAssets\yoo\DefaultPackage\*.bundle` 前 24 字节是自定义头，之后是**未加密的标准 UnityFS**:
```
偏移 0  : "A1BNDLHP"    (8 字节 magic)
偏移 8  : 02 00 00 00   (版本 2, LE)
偏移 12 : 18 00 00 00   (头长度 24, LE)
偏移 16 : int64 LE      (UnityFS 部分大小 = 文件大小 - 24)
偏移 24 : "UnityFS\0" … (标准 UnityFS, 明文 "2022.3.43f1" 可见)
```
**解析: 砍掉前 24 字节交给 UnityPy 即可**（实测 1387 个 bundle 全部成功、零错误）。

### 配置表总集
- bundle `b584dd66ff433fa742dc6829f2368641.bundle` = 配置表总集: 506 个 TextAsset = **259 张表 × (明文 JSON + 二进制打包) 两份**。
- 表 JSON 顶层是**数组**；表资源名全小写（如 `tbminigamemedical`），表内字段 snake_case（部分表 camelCase 如 `costStamina`，与打表工具有关）。
- 本工作区已落盘: `tables_dump/`（259 张明文表）、`scan_textassets.jsonl`（全部 1451 条 TextAsset 索引）。

---

## 2. 官方 mod 框架（Game.Mod）—— 反汇编级确认

### 类结构（Il2CppDumper dump.cs）
- `ModManifest`: 字段即 mod.json 键 → `schemaVersion`(CURRENT=5) / `modId` / `title` / `author` / `version` / `priority` / `publishedFileId` / `units[]`
- `ModUnitDeclaration` = `{unitId, type, root}`；`ModUnitType` 枚举: NpcAvatar=1 / TablePatch=2 / AssetOverride=3，类型名 `npc_avatar` / `table_patch` / `asset_override`
- `ModPackage.Load` 分支: mod.json 读得出来 → `LoadFromModJson`（按 units 声明）；读不出 → `LoadConvention`（纯目录约定）。**两者空 units 都会调 `SynthesizeConventionUnits`**
- `SynthesizeConventionUnits` 合成规则: `spaces/` → 路径型 unitId（`spaces.{场景}.npc_{npcId}`）；**`assets/` → unitId=`"assets"`；`tables/` → unitId=`"tables"`**
- `LocalModSource.ROOT_FOLDER_NAME = "Mods"` → 扫描 `LocalLow\Nuverse\WorldApart\Mods\`；另有 `SubscribedModSource`（工坊）与 `AddTransientPackage`（临时/GM）
- `ModIdAllocator.SegmentStart = 900000000`（mod 新增表行的 ID 分配段）

### ⚠ 启用机制（最隐蔽的一环）
- `ModRegistry.ActiveUnits(type)` 只返回 `AnyEntryEnabled(modId, unit)` 通过的包/单元。
- `AnyEntryEnabled`: NpcAvatar → `IsEntryEnabled(modId, unitId, unit.TargetNpcId)`；**其他类型（含 TablePatch）→ `IsEntryEnabled(modId, unitId, 0)`**。
- `EntryKey` 拼接: npcId>0 → `modId:unitId#npcId`；**npcId≤0 → `modId:unitId`（无 # 后缀）**。
- 存储: `PlayerPrefs.SetString("MOD_ENABLED_ENTRIES_V2", String.Join("\n", entries))`
  → Windows 注册表 `HKCU\Software\Nuverse\WorldApart`，值名 `MOD_ENABLED_ENTRIES_V2_h2913630511`（REG_BINARY = UTF8 字节 + `\0` 结尾）。
  排序同理存 `MOD_ORDER_V1_h2735767082`。
- **table_patch 没有 UI 启用入口**（工坊 UI 类型只有 avatar/scene/audio，本地 mod 连列表都不进）→ 只能直接写注册表。

### 其他
- 正常加载**完全静默**——Player.log 没有 `[Mod]` 行不代表没加载；只有失败才打日志（`[Mod] 配置补丁未生效/被拒`、`[Mod] 存档 {0} 缺少 mod：{1}` 等）。
- 工坊 UI（创意工坊面板）只列 Steam 订阅 + 我的制作；"套用"按钮按 NPC 粒度写启用条目；本地手动放的 mod 无任何 UI 入口。
- `ModSaveCheck` 存档校验 mod 版本；`ModStamp`（MessagePack, key: modId/version/title）随存档走。

---

## 3. tables/ 补丁格式规范（权威, 反汇编自 `ModTablePatch.Read`）

文件: `mod根目录\tables\<表名>.patch.json`，表名正则 `^[a-z0-9_]+$`。

```jsonc
{
    "table": "tbminigamemedical",       // 可选? 官方校验存在; 空则回退为文件名去掉 .patch.json
    "keyFields": ["level_id"],          // 主键字段; 缺省时有 DefaultKeyFields（未逆向细节）
    "upsert": [ { "level_id": 1001, "rotation_limit": 9999, … } ],  // 行对象数组, 每项必须含主键
    "remove": [ "<主键值>" ],           // 可选; 删行
    "refFields": { "字段": "表名" }     // 可选; 引用重写
}
```
- 解析用 Newtonsoft `JObject.Parse` 手动取**小写键** `table` / `keyFields` / `refFields` / `upsert` / `remove`。
- `upsert` 行是**按主键合并**语义（`JsonMergeSettings`）——写完整行兼容一切。
- 官方校验消息（拼错误用）: `upsert 里有一项缺主键字段(...)`、`upsert 必须是数组`、`remove 里有一项不是主键值`、`refFields 必须是 字段名->表名 的对象`、`table 名非法: "…"(只允许小写字母、数字、下划线)`、`keyFields 里有空项`。
- 枚举: `Directory.GetFiles(<unitRoot>/tables, "*.patch.json")` → 文件名排序 → 逐个 Read。

---

## 4. 医术小游戏（疏经导脉）数据链

- 关卡表 `tbminigamemedical`: 36 行, 主键 `level_id`(1001-4009)。字段:
  `level_id / level_seed / board_rows / board_cols / start_pos / end_pos / path_count /
   time_limit(-1=无限时) / rotation_limit / start_point_name / end_point_name(多语言) /
   blocker_replace_pct / stuck_replace_pct / bonus_type1_pct / bonus_type1_value(+5) /
   bonus_type2_pct / bonus_type2_value(+10) / mask_pct / connected_shuffle_pct / disconnected_shuffle_pct`
- **`rotation_limit` 全 36 关 = 60**，即行针上限（官方文案: "可消耗行针旋转，尝试接通相邻端口"、"与源穴连通后获得行针加成 +{0}"）。
- 校验消息: `TbMiniGameMedical level_id={0} field {1} must be -1 (unlimited) or >= 1` → **-1 是游戏原生 unlimited 语义**。
- 精力: `tbnpcinteractgameentry.json` 的 `costStamina`（1335 行全 = 10）。`interactGameType: 3` = 疏经导脉（1=留影石, 2=说服, 4=玄镜鉴妖），`interactGameParam` = 医术关卡 `level_id`。
- 代码侧: `GetNeedleLimitTotal`（基础+加成）/ `HasNeedleLimit` / `GetNeedleRemaining` / 技能加成走 `MedGameAddNeedleEffectData`；同族小游戏表 `tbminigameshooting / resonance / dualcultivate / learngongfa / spotfind / spotmirror`。

---

## 5. 工具与方法论（可复用）

### Il2CppDumper
- Perfare/Il2CppDumper v6.7.46 win 版, 命令: `Il2CppDumper.exe GameAssembly.dll global-metadata.dat 输出目录`（输出落当前目录；结尾按键异常无害）。
- 产物: `dump.cs`（全部类/字段/方法签名 + RVA）、`script.json`、`stringliteral.json`、`DummyDll/`。
- metadata: `WorldApart_Data\il2cpp_data\Metadata\global-metadata.dat`（v31, sanity 0xFAB11BAF）。

### capstone 反汇编 IL2CPP 方法体
- **script.json 里的 Address 是 RVA，要 + ImageBase(0x180000000)** 才是 VA——这是本工作区踩过的坑。
- 方法体直接按 dump.cs 的 Offset/RVA 反汇编；call 目标与字符串字面量可用 script.json 回注成可读形式。
- metadata 初始化噪声（`lea rcx,[rip+..]; call 0x1805de1c0` 序列）可过滤。
- 适用于还原字符串键名、分支逻辑、调用链——本次靠它拿到 EntryKey 格式和 AnyEntryEnabled 的类型分派。

### metadata 字符串字面量精确解析（不依赖 Il2CppDumper 的轻量法）
- 解析 global-metadata.dat 头（偏移 8 起: slOff, slSize, sldOff, sldSize 各 int32）→ string literal 表每条 (len, off) 各 int32 → 全部 5.4 万条代码字符串。
- 字符串堆（类型/方法/字段名）无分隔符，字节窗口法只能看簇；literal 表才是权威。

### UnityPy 扫 bundle
- 批量砍头 + `UnityPy.load(io.BytesIO(raw[24:]))` → 遍历 `obj.type.name == 'TextAsset'` → `ta.m_Name` / `ta.m_Script`。1387 个 80 秒扫完。
- 结果落盘 JSONL 再交叉核对（防工具输出污染: 复解析比对、统计自然性）。

---

## 6. 排错时间线（教训浓缩）

| 轮次 | 尝试 | 结果 | 教训 |
|---|---|---|---|
| 1 | 补丁命名 `tbminigamemedical.json` 部署 | 无效, 日志干净 | 后缀错, **被静默忽略**——IL2CPP 加载失败大多无日志 |
| 2 | 以为要在工坊 UI"应用" | UI 无本地 mod 入口 | 先看清 UI 的数据来源（订阅驱动）再找入口 |
| 3 | 反汇编确认 `.patch.json` 并改名 | 仍无效 | **table_patch 也需要启用条目**（AnyEntryEnabled 类型分派） |
| 4 | 按汇编格式写注册表启用条目 | ✅ 生效 | 全链闭合后一次成功 |

通用教训:
- 字符串字面量只能证明"键存在"，**方法体反汇编才能确定分支逻辑**（如 EntryKey 无 `#` 后缀这种细节）。
- 验证格式用"官方校验错误消息"反推非常高效（游戏自己的报错文案 = 格式规范）。
- 改注册表/存档前先确认目标进程已退出。

---

## 7. 注入路线（已放弃，2026-10-03 终局）

结论: **gpShell 壳对 doorstop 系 IL2CPP 注入有系统性干扰**，两个 loader 双路线全军覆没，用户决定放弃注入。

实验记录:
- **BepInEx 6.0.0-pre.2**（winhttp.dll 代理 + doorstop 4.3）: 代理被加载（用户 `tasklist /m winhttp.dll` 显示进程内两个 WINHTTP.dll 副本），配置/文件全部完好，但 BepInEx 托管层**零痕迹**（无 LogOutput.log、无 Interop 生成）——执行链被静默掐断。
- **MelonLoader 0.7.3**（version.dll 代理，不同代理名+独立初始化路径，区分实验）: doorstop 阶段启动（创建了 UserData\ 目录）随即**原生崩溃**——WER 报告（Windows 自写，事件查看器可复核）: 故障模块 VERSION.dll 0.7.3.0、异常 0xC0000005 @ 0x5ee8c0、崩溃时 Unity 引擎模块尚未加载。
- gpShell.dll: Detours 壳（.detourc/.detourd 节），进程模块序列第 4 位极早期加载；字符串分析**无**按名过滤名单（coreclr/hostfxr/BepInEx 均不出现）→ 具体干扰机制未逆向（行为级确认足够，不再深挖）。
- 清理: 游戏 `A1\` 目录 loader 残留全部清除，恢复原状（仅剩游戏本体 + gpShell/gpHackerProc + debug.log）。

教训: 对抗壳的注入 = 深水区，收益（省一个注册表步骤）远低于成本。**静态分析完全不受壳影响**（Il2CppDumper/字符串/PE 解析一路畅通）——官方 mod 框架的知识全部由此获得。发布场景用"文件夹分发 + 一键注册表脚本"或等官方工坊 UI。

## 8. 风险与注意事项

- 保护壳: gpShell.dll（Detours 用户态壳）+ gpHackerProc.dll（外挂进程检测）+ GSDK。**表补丁路线完全不碰进程内存，无注入风险**；BepInEx 桥接（调官方 API）次之；真正的 IL hook（改 .text）风险最高，无需求不做。
- gpHackerProc 检测的是外挂进程名，BepInEx 无独立进程；GSDK 卡死监控对 doorstop 启动延迟的容忍度未验证。
- UGC 发布走服务器审核，成人内容只能本地用；table_patch 无 UI 入口，发布到工坊后订阅者同样需要注册表条目或桥接插件才能生效（或官方后续版本补 UI）。
- 游戏 EA 早期 schema 迭代快（已到 v5），格式可能随版本变动——`mod.json schemaVersion={0} 高于本版本支持的 {0}，请更新游戏`。

---

## 8. 工作区文件清单（`D:\code\mod\world apart\`）

| 文件/目录 | 内容 |
|---|---|
| `infinite_needle/` | mod 开发副本（与部署一致） |
| `tables_dump/` | 259 张原版配置表明文 JSON |
| `tbminigamemedical.json` | 医术关卡表原版副本（25166 字节） |
| `scan_textassets.jsonl` / `scan_errors.txt` | 1387 bundle 的 TextAsset 全量索引 / 错误（空） |
| `string_literals.txt` | metadata 5.4 万条代码字符串字面量 |
| `il2cppdumper/` | Il2CppDumper 产物: dump.cs / script.json / stringliteral.json / DummyDll |
| `disasm_report.txt` | 关键方法反汇编报告（LoadFromModJson/Synthesize/Read/IsEntryEnabled 等） |
| `Il2CppDumper.zip` | 工具压缩包 |
| `NOTES.md` | 本文档 |
