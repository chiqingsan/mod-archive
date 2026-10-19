# 爆头回弹 / HeadshotBulletRefund

《逃离鸭科夫》(Escape from Duckov) 的 mod：**爆头击杀敌人后，把「击杀时使用的那颗子弹」直接补回枪械弹匣**（不进入备弹）。返还数量、是否限定爆头、是否允许超出弹匣容量都**可在游戏内设置菜单里改**。

- 已安装到：`d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data\Mods\HeadshotBulletRefund\`
- 修改计划与设计依据：[PLAN.md](PLAN.md)

---

## 1. 启用步骤

游戏存档里这两个开关**都已经是开着的**（`%USERPROFILE%\AppData\LocalLow\TeamSoda\Duckov\Saves\Global.json` 里 `AllowLoadingMod = true`、`ModActive_HeadshotBulletRefund = true`、`ModActive_ModConfig = true`），所以只需要：

1. **重启游戏**（dll 是启动时加载的，本次已换成带设置菜单的新版本，必须重启才会生效）。
2. 进游戏后到 **设置 → Mod Settings** 标签页，会看到 `HeadshotBulletRefund` 的四个选项。
3. 日志里应出现 `Mod Loaded: HeadshotBulletRefund`。

若哪天开关变成关的：协议确认对应 `AllowLoadingMod`，勾选 mod 对应 `ModActive_HeadshotBulletRefund`。

## 2. 行为定义

| 情况 | 结果 |
| --- | --- |
| 主角爆头击杀敌人，弹匣还有同类弹 | 弹匣 +1 发（并入已有弹堆），HUD 弹数立刻 +1 |
| 主角爆头击杀敌人，弹匣正好被那一枪打空 | 现场生成 1 发同弹种子弹放进弹匣（走游戏自带的入匣逻辑） |
| 弹匣已满（默认设置） | 不返还，不会超容量 |
| 击杀时正在换弹 | 挂起，换弹结束后自动补（避免把弹匣装过头；超 10 秒丢弃） |
| 非爆头击杀 / 击杀者是队友宠物 / 手雷爆炸 / 近战 / 靶场 / 换枪 | 不返还（可关掉“仅爆头”） |

**“爆头”的判定沿用的是游戏自己的定义**：玩家开枪时 `critRate = 准星是否对着敌人头部 ? 1 : 0`，`Health.Hurt` 据此写入 `DamageInfo.crit`，本 mod 只看 `crit > 0`。这与游戏的爆头音效、护甲取值（取头盔而非防弹衣）、任务里的 `requireHeadShot` 完全一致。
注意这继承了原版的一个特性：评判依据是**开火瞬间的准星位置**，而不是子弹实际命中的部位。

**弹种**取击杀瞬间弹匣里那一堆子弹的 `TypeID`（弹匣打空时退回枪械记录的 `TargetBulletID`），所以**换弹种、混装弹匣、栓动枪打空弹匣都能对应上正确弹种**。

## 3. 数值配置（两种方式）

### 3.1 游戏内设置菜单（推荐）

需要社区框架 **ModConfig**（创意工坊 `3590674339`，你已订阅并启用）。它会在主菜单的**设置**里加一个 `Mod Settings` 标签页；本 mod 检测到它就自动注册下面 4 项，**改完立即生效，不需要重启**（ModConfig 每次落盘都会回调本 mod 重读数值）。

| 选项（界面显示） | 键 | 默认 | 范围 | 说明 |
| --- | --- | --- | --- | --- |
| 总开关 / Master switch | `Enabled` | 开 | 开/关 | 关掉后完全不返还（事件回调第一行就返回） |
| 仅爆头击杀时返还 | `HeadshotOnly` | 开 | 开/关 | 关掉则**任意击杀**都返还 |
| 每次击杀返还的子弹数 | `RefundCount` | 1 | 1–10 | 一次击杀补几颗（滑块） |
| 绝不超过弹匣容量 | `RespectMagazineCapacity` | 开 | 开/关 | 关掉后可以补到容量之上（配合上一项玩“越打越多”）|

> 实现方式：本 mod **不依赖 ModConfig**，只用反射去调它的公开 API（`ModConfig.ModBehaviour.AddBoolDropdownList / AddInputWithSlider / AddOnOptionsChangedDelegate`，要求其 `VERSION == 1`）。没装 ModConfig 时这些代码完全不执行，mod 照常工作。
> 因为 mod 是按文件夹名顺序激活的，本 mod（H…）可能比 ModConfig（M…）先加载，所以注册失败时会每 2 秒重试一次、最多 15 次，之后彻底放弃（不再有任何每帧开销）。

### 3.2 直接改文件 `config.json`

与 dll 同目录（`Duckov_Data\Mods\HeadshotBulletRefund\config.json`）。**不需要任何其它 mod**，改完重启游戏生效（或进一次游戏内设置菜单也会重读）：

```json
{
  "enabled": true,
  "headshotOnly": true,
  "refundCount": 1,
  "respectMagazineCapacity": true
}
```

文件缺失或格式错误时会静默退回内置默认值，不影响 mod 工作。取值优先级：内置默认值 < `config.json` < 游戏内设置菜单。

## 4. 性能

- 只订阅 `Health.OnDead` 静态事件，**不挂 Harmony 补丁、不轮询、无协程**；没有死亡事件时开销为 0。
- 回调前两行就是「总开关 + `crit` 短路」，绝大多数死亡（以及玩家自己死）在这里直接返回，不产生任何分配。
- 只有真的返还时才写物品，走一次已缓存的反射；`Update()` 在「设置已注册 + 无挂起返还」后只有两次 bool 判断。
- 设置项只在启动阶段注册一次；连不上 ModConfig 时最多重试 15 次（每 2 秒）后彻底放弃。
- 不打补丁 = 不存在与其它 mod 的 Harmony 版本冲突。

## 5. 交付物与重建

```
HeadshotBulletRefund/
├─ PLAN.md                      修改计划（钩子选型、分支、性能账、边界、配置方案）
├─ README.md                    本文件
├─ src/HeadshotBulletRefund.csproj
├─ src/ModBehaviour.cs          入口与全部玩法逻辑（HeadshotBulletRefund.ModBehaviour）
├─ src/ModSettings.cs           四个可配置数值 + config.json 读取（JsonUtility）
├─ src/ModConfigBridge.cs       对 ModConfig 的可选反射桥（没装也不报错）
├─ dist/                        安装成品：dll + info.ini + preview.png + config.json
└─ tools/
   ├─ install.ps1               编译产物同步进 dist 并安装到游戏（复现游戏 info.ini 校验）
   ├─ verify-contract.ps1       反射校验加载契约：类型名/继承/无缺失引用
   └─ make-preview.py           重新生成 256x256 preview.png
```

重建与重装：

```powershell
dotnet build .\src\HeadshotBulletRefund.csproj -c Release
.\tools\install.ps1
```

`install.ps1` 会先把 `src\bin\Release` 的新 dll 同步进 `dist/`，再把整个 `dist/` 复制到游戏，同名目录自动备份为 `<name>.bak-yyyyMMdd-HHmmss`。换安装路径时用 `-GameRoot`，换编译目标用 `-p:GameManagedDir="...\Duckov_Data\Managed\"`。

## 6. 游戏内验证清单

1. 带一把自动步枪（弹匣至少 2 发），对敌人**瞄准头部**击杀 → 弹匣数应在击杀瞬间 +1（先打空 3 发再爆头，观察是否从 27 变回 28）。
2. **非爆头**击杀（打身体）→ 弹匣数不变。
3. 把弹匣打到只剩 1 发，爆头击杀最后一发 → 弹匣应变成 1 发（弹匣打空的重建路径）。
4. 弹匣打满（换弹后）再爆头击杀 → 不增长、不超容量。
5. 靶场里爆头 → 不返还（原版靶场也不耗弹）。
6. 让宠物/队友击杀、或用手雷/近战击杀 → 不返还。
7. 换弹过程中爆头击杀 → 换弹结束后弹匣比平时多 1 发。
8. 设置菜单：把 `RefundCount` 改成 3 → 爆头一次应 +3；把 `仅爆头` 关掉 → 打身体击杀也会 +3；把 `弹匣容量` 关掉 → 可以补到容量之上。

## 7. 已知限制

- 若其它 mod 让**打身体也能触发暴击**，本 mod 会把这种击杀也算作爆头（因为沿用了同一个 `crit` 字段）。
- 每次返还数的上限是 10（常量 `ModSettings.MaxRefundCount`，`config.json` 与滑块都受它约束）；想更大就改这个常量并重新编译。
- 游戏内设置菜单依赖社区框架 ModConfig；没装它时只能用 `config.json`。
- 若游戏更新改了 `ItemSetting_Gun` 的私有缓存成员名，返还本身仍然生效，只在第一次返还时打一条警告，之后由游戏自身重算弹药显示；数值型改动（容量/弹种 API）才需要重新编译。
- 只对「主角手持枪械」生效；载具武器、炮塔等非 `ItemAgent_Gun` 的武器不在范围内。
