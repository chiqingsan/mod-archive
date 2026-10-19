# Quick Counter

Probably Stolen 的可配置柜台快捷键 Mod。

## 使用

1. 用 Rider 打开 `QuickCounter.sln` 或 `QuickCounter.csproj`。
2. 构建项目。
3. 构建出的 `QuickCounter.dll` 会自动复制到游戏的 `Mods` 目录。
4. 启动游戏，在 `MelonLoader/Latest.log` 中查找 `Quick Counter 已成功加载！`。

默认游戏目录：

`D:\app\steam\steamapps\common\Probably Stolen Demo`

如果游戏移动了位置，只需修改 `QuickCounter.csproj` 中的 `GameDirectory`。

游戏 API 与后续 Mod 开发资料位于 `D:\code\mod\docs`。
