# 爆头回弹 (HeadshotBulletRefund) — 修改计划

> 目标：主角**爆头击杀敌人**后，给主角**回复 1 颗子弹**；弹种 = **击杀时使用的那颗子弹**；位置 = **枪械弹匣**（不是备弹）；**性能开销最小**。
>
> 追加需求（来自后续提问）：这些数值要能**通过 mod 菜单之类的框架在游戏内配置**（见 §7）。
>
> 目标游戏：逃离鸭科夫 / Escape from Duckov（Unity, Mono）。安装目录 `d:\app\steam\steamapps\common\Escape from Duckov`。
> 全部结论来自对 `Duckov_Data\Managed\TeamSoda.Duckov.Core.dll`、`ItemStatsSystem.dll` 的反编译（反编译工作副本在 `D:\code\mod\Escape from Duckov\decompiled\`，类型清单在 `type-lists\`）。

---

## 1. 数据模型（先搞清楚「弹匣」是什么）

本作**没有独立的弹匣物品**，弹匣就是「枪这件 Item 自己的内部 Inventory」：

| 概念 | 代码事实 |
| --- | --- |
| 弹匣内容 | `ItemStatsSystem.Item.Inventory`（`Item.cs:360`）里的若干堆叠子弹 Item |
| 弹匣余弹（权威） | `ItemStatsSystem.ItemSetting_Gun.GetBulletCount()`（`ItemSetting_Gun.cs:410`）= 弹匣内所有 Item 的 `StackCount` 之和 |
| 射击消耗 | `ItemSetting_Gun.UseABullet()`（`ItemSetting_Gun.cs:179`）：遍历 `Item.Inventory`，有堆叠的 `item.StackCount--`；弹堆减到 0 时 `Item.StackCount` setter 内部 `DestroyTree()`（`Item.cs:482`） |
| 显示用缓存 | 私有属性 `ItemSetting_Gun.bulletCount`（`ItemSetting_Gun.cs:101`）→ 字段 `_bulletCountCache`，setter 顺便写 `Item.Variables["BulletCount"]`；初始 `-1`，`BulletCount` getter（`:85`）在 `< 0` 时用 `GetBulletCount()` 重算。**这是唯一需要额外同步的东西** |
| 弹种 | `ItemSetting_Gun.TargetBulletID`（`:71`，字段 `targetBulletID` 未序列化，读档后可能为 `-1`） |
| 容量 | `ItemSetting_Gun.Capacity`（`:114`）= 枪的 `Capacity` 属性值 |
| HUD | `BulletCountHUD.Update()` 每帧读 `ItemAgent_Gun.BulletCount` → 只要修正了缓存/物品，HUD 下一帧自动正确 |

结论：**「往弹匣里补一发」= 往 `gunItem.Inventory` 里加 1 颗对应弹种 Item（或让已有同类弹堆 +1），再同步上面那个缓存。**

## 2. 钩子选型：只用事件，不打补丁

| 方案 | 结论 |
| --- | --- |
| ✅ **订阅 `Health.OnDead` 静态事件**（`Health.cs:205`, `Action<Health, DamageInfo>`） | 游戏自己的任务系统就是这么做的（`QuestTask_KillCount.cs:263-295` 的 `Health.OnDead += / -=`）。**零每帧开销、零 Harmony 依赖、零 Harmony 版本冲突**。死亡事件每次死亡才触发一次，handler 里第一行就短路 |
| ❌ Harmony 前缀补丁 `Health.Hurt` / `Projectile` | 多一个第三方依赖（游戏本体不含 Harmony）、每次受伤都要过一遍补丁，没有任何收益 |
| ❌ 订阅 `DamageReceiver.OnDead` 等实例事件 | 需要在敌人生成时逐个挂/摘，成本更高，且容易漏 |
| ❌ 在射击瞬间决定「不消耗这颗子弹」 | 射击时还不知道是否能爆头击杀（子弹有飞行时间），会被迫在每次开火都插钩子 |

## 3. 「爆头」在本作里的判定（关键前提）

- `Health.Hurt()` 里：`bool flag2 = UnityEngine.Random.Range(0f,1f) < damageInfo.critRate; damageInfo.crit = flag2 ? 1 : 0;`（`Health.cs:361-362`）
- 玩家开枪时：`ItemAgent_Gun` 设置 `context.critRate = CritRate * (...)`，**但如果是玩家操控的角色：`context.critRate = LevelManager.Instance.InputManager.AimingEnemyHead ? 1f : 0f;`**（`ItemAgent_Gun.cs:1167-1171`）
  → 玩家开枪时 `critRate` 只有 0/1 两种：**准星对着敌人头部 = 1（必爆），否则 = 0**（无随机性）。
- 游戏自己也把 `crit` 当作爆头：爆头命中音效 `hitmarker_head`/`killmarker_head`（`AudioManager.cs:147/151`）、暴击时用 `HeadArmor` + 头盔耐久而非防弹衣（`Health.cs:374-377`）、任务 `requireHeadShot` 就是 `info.crit > 0`（`QuestTask_KillCount.cs:295`）。
- 近战武器 `damageInfo.crit = -1`（`ItemAgent_MeleeWeapon.cs:192`），所以近战击杀天然不会误触发。

**所以判定条件 = `damageInfo.crit > 0`，与游戏自带的「爆头」定义完全一致。**

## 4. 返还逻辑（分支与优先级）

```
Health.OnDead → OnAnyDead(health, info)：
  0. 总开关关（设置项 Enabled）        → 退出
  1. 仅爆头 且 info.crit <= 0          → 不是爆头，退出（绝大多数死亡走这里）
  2. health.team == Teams.player       → 死的是我方，退出
  3. info.fromCharacter 不是主角        → 不是主角击杀，退出
  4. LevelManager.IsBaseLevel          → 基地靶场（开枪不耗弹），退出
  5. 主角手上拿的不是枪                 → 退出
  6. info.fromWeaponItemID > 0 且 ≠ 手里枪的 TypeID → 换枪/手雷/爆炸，退出
  7. 弹种 typeID：弹匣里还有弹 → 用当前那堆的 TypeID；弹匣空 → 用 TargetBulletID
     （若为 -1（读档后空弹匣）→ AutoSetTypeInInventory 按口径从背包认一次，认得就补，认不出就放弃）
  8. 弹匣满（默认「绝不超过弹匣容量」）：用真实统计值 GetBulletCount() 算剩余空位 room
     room <= 0 → 放弃；需要返还 want = RefundCount 颗，want > room 时按 room 截断（装得下几颗算几颗）
  9. 换弹中（LoadingBullets）→ 挂起，等换弹结束再补（见 §5）
 10. 写入（循环 want 次）：弹匣里有同类弹堆 → StackCount++；弹匣空 → InstantiateSync 生成 1 颗再放进弹匣
 11. 整批装完后，同步私有缓存 bulletCount 一次（不是每颗同步一次）
```

## 5. 两个「不直接写」的边界及处理

1. **击杀那一枪正好打空弹匣**（栓动/霰弹常见）：最后一颗已经被 `DestroyTree()` 销毁，无处可并 → 现场生成 1 颗同弹种子弹（`ItemAssetsCollection.InstantiateSync(typeID)`，游戏自己的换弹代码也用这个 API，见 `ItemSetting_Gun.cs:349`）。成本：**每次这种击杀 1 个小 GameObject**，正常弹匣余弹充足时不会走。
2. **击杀时正在换弹**：`LoadBulletsFromInventory` 在开头一次性算好 `needCount = capacity - bulletCount`（`ItemSetting_Gun.cs:305-306`）再异步装填，此刻插一发会让弹匣**溢出一发** → 所以换弹中不写，改为**挂起**：记 `(枪 Item, 弹种)`，在 `Update()` 里每帧尝试一次（`Update` 第一行 `if (!_hasPending) return;`，无挂起时只有一次 bool 判断），换弹结束就立即补；换满则放弃；10 秒超时兜底。挂起是计数累积的，同一段换弹里多次爆头击杀会补回多发。

## 6. 性能账（要求「占用最小性能」）

| 场景 | 开销 |
| --- | --- |
| 平时（没有死亡事件） | **0**：不挂 Harmony、不轮询、无协程；`Update` 在「设置已注册 + 无挂起返还」之后只有两次 bool 判断（纳秒级） |
| 启动阶段 | 读一次 `config.json`（失败静默）+ 注册 4 个设置项；连不上 ModConfig 时最多重试 15 次 × 每 2 秒一次，之后彻底放弃 |
| 任意死亡（含非爆头、玩家自己死） | 一次委托调用 + 1~3 次字段比较后 return；无分配、无 GC |
| 爆头击杀（返还成功） | 几次属性读取 + `StackCount++` ×N + 1 次反射写缓存（只在返还时用，且 `PropertyInfo` 已缓存）；无 LINQ、无闭包、无字符串拼接 |
| 弹匣打空的那种爆头击杀 | 额外 1 次 `InstantiateSync`（游戏内建 API，正常情况不触发） |
| 玩家在设置菜单里改数值 | ModConfig 回调 → 重读 4 个键，不在游戏循环里 |

反射仅在**真的返还那一瞬间**使用一次，用于同步 `ItemSetting_Gun.bulletCount`；若游戏更新改了私有名（`bulletCount`/`_bulletCountCache`），只会在第一次返还时打一条警告日志，之后靠游戏自身的 `BulletCount < 0 → 重算` 机制自愈，功能不受影响。

## 6.5 数值配置方案（后续追加需求）

**前提事实**：游戏本体没有 per-mod 设置菜单，只有两个全局开关（`AllowLoadingMod`、`ModActive_<name>`，`ModManager.cs:29-43/220-223`）。
社区方案是创意工坊的 **ModConfig**（`3590674339`）：它在主菜单「设置」里加一个 `Mod Settings` 标签页，给每个 mod 开一栏；键值存在 `Saves/ModConfig.ES3`（EasySave3）。它公开的接口是 `ModConfig.ModBehaviour` 的静态方法：
`AddBoolDropdownList / AddDropdownList / AddInputWithSlider(modName, key, description, ...) / AddOnOptionsChangedDelegate(Action<string>)`，且要求 `ModConfig.ModBehaviour.VERSION == 1`；**键名是拼好前缀的全名**（`<modName>_<key>`，回调参数也是它）。

**选型**：不复制它的 369 行模板 `ModConfigAPI`，而是自己写一个约 100 行的反射桥 `src/ModConfigBridge.cs`：
- 反射按名字找 `ModConfig.ModBehaviour`（从已加载程序集 + 磁盘上的 `ModConfig.dll` 两条路找），取 `VERSION` 与实际需要的那几个静态方法；任何一个缺失就整体放弃。
- 没装 ModConfig 时**一次都不调用**，本 mod 不产生任何硬依赖（`verify-contract.ps1` 的输出里 references 共 5 个，全部存在于游戏 `Managed`，其中**不含** ModConfig/EasySave3）。
- 激活顺序是文件夹名排序，本 mod（H…）可能早于 ModConfig（M…），所以 `OnAfterSetup` 注册失败后由 `Update` 每 2 秒重试一次、最多 15 次，之后置 `_modConfigGaveUp` 彻底停手（此后 `Update` 只剩两次 bool 判断）。

**取值优先级**：内置默认值 < `config.json`（`JsonUtility`，游戏自带 `UnityEngine.JSONSerializeModule`，无第三方依赖） < ModConfig 存档值。四个键：`Enabled`、`HeadshotOnly`、`RefundCount`(1–10 滑块)、`RespectMagazineCapacity`。

**原来的「不加配置 UI」被这条需求取代**：仍然不加自己的 IMGUI/按键 UI，但不再拒绝「游戏内可配置」。

## 7. 明确不改的东西

- 不改射击/换弹/伤害流程，不碰 `UseABullet`，不改容器容量、不改背包备弹。
- 不引第三方依赖：不需要 Harmony，也不硬依赖 ModConfig（只用反射按需对接）。
- 不改存档结构（弹匣内容本来就是 `Item.Variables` 序列化的一部分，补一发属于正常状态）。

## 8. 交付物

```
D:\code\mod\Escape from Duckov\HeadshotBulletRefund\
├─ PLAN.md                     本文件
├─ README.md                   安装/启用/配置/验证说明
├─ src/HeadshotBulletRefund.csproj
├─ src/ModBehaviour.cs          入口与全部玩法逻辑（入口类型名必须是 HeadshotBulletRefund.ModBehaviour）
├─ src/ModSettings.cs           4 个可配置数值 + config.json 读取（JsonUtility）
├─ src/ModConfigBridge.cs       对 ModConfig 的可选反射桥（未安装时不报错、不调用）
├─ tools/install.ps1            同步编译产物到 dist/ 并安装到游戏
├─ tools/verify-contract.ps1    反射校验加载契约（类型名/继承/无缺失引用）
├─ tools/make-preview.py        生成 256x256 preview.png
└─ dist/                       安装到游戏的成品
   ├─ HeadshotBulletRefund.dll
   ├─ info.ini
   ├─ config.json              （可选，四个数值的默认值）
   └─ preview.png
```

安装位置：`d:\app\steam\steamapps\common\Escape from Duckov\Duckov_Data\Mods\HeadshotBulletRefund\`
（`ModManager.DefaultModFolderPath = Application.dataPath + "/Mods"`，`ModManager.cs:61`；DLL 必须叫 `<info.ini 的 name>.dll`，`ModInfo.cs:30`；入口类型必须是 `<name>.ModBehaviour` 且继承 `Duckov.Modding.ModBehaviour`，`ModManager.cs:445-448`。）
