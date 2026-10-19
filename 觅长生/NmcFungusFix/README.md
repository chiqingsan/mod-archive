# NmcFungusFix（NextMoreCommands 闪退修复）

修复 NextMoreCommands（工坊 2921046709，v1.0.4）在进入灵界「砺剑峰」（F10281）/「月影寒渊」（F10283）等三场景时的**原生崩溃闪退**（Unity 访问违例，进程直接死，托管层拦不住）。

## 根因

NextMoreCommands 的 `OnEnterThreeScene.FungusPatch()` 在每次进入三场景时对全场景 Flowchart 做两轮**全子树遍历**（`GetComponentsInChildren`，其中一轮列表跨根对象累积、接近 O(n²)），且未对 `FindObjectsOfTypeAll` 可能返回的已销毁对象判空。砺剑峰这类对象多、层级深的迷宫场景下，遍历在 Unity 原生层触发访问违例 → 闪退。

崩溃栈（游戏崩溃转储证实，10-04 起共 7 份同栈）：

```
ThreeSceneMag.init → NextMoreCommands.OnEnterThreeScene_Postfix
  → FungusPatch() → GetComponentsInChildren → Unity 原生层 AV
```

## 修复方式

Harmony Prefix 接管 `FungusPatch`，用等价但安全的实现替换：

- 遍历 `Resources.FindObjectsOfTypeAll<Flowchart>()`（本就是内存中全部 Flowchart，逐根子树遍历纯属冗余），**判空后**逐个调 Next 的 `FPatch.PatchFlowchart`；
- `PatchFlowchart` 内部有 `FPatchRecord` 幂等守卫（已打过或无补丁数据直接返回），重复调用安全；
- 不再调用任何 `GetComponentsInChildren`，崩溃路径整体消除。

不修改工坊原件；未安装 NextMoreCommands 时自动跳过；补丁异常时只记日志（绝不放行到原实现）。

## 安装 / 卸载

- 安装：`本地Mod测试\NmcFungusFix\plugins\NmcFungusFix.dll`（或任意 BepInEx plugins 目录）
- 卸载：删除该 dll，游戏行为即恢复原样

## 验证

进游戏（需重启游戏客户端让 dll 完成加载）后看 `BepInEx\LogOutput.log`：

- 加载成功：`已接管 NextMoreCommands 的 OnEnterThreeScene.FungusPatch...`
- 首次触发替代实现（进任意三场景后）：`FungusPatch 安全替代已首次执行：本次扫描 N 个 Flowchart`
- 进砺剑峰不再闪退（这是最终验收，需人操作）

## 构建

```
dotnet build -c Release
```

依赖路径在 `NmcFungusFix.csproj` 中配置（游戏目录 / BepInEx core）。
