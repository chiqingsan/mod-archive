# QuickCounter 正式版适配

针对本机 Probably Stolen 正式版 Mono 程序集构建，使用游戏内置 ModLoader 和配置界面。构建：`dotnet build -c Release`；将 `manifest.xml` 和生成的 `QuickCounter.dll` 放进游戏 `Mods/QuickCounter` 文件夹，然后在游戏的模组界面启用。

功能包括开始营业、呼叫顾客、转移物品、遣散顾客、呼叫治安部、打开日历和平板电脑，以及确认平板交易和处理鼠标下物品。每个动作都可以在游戏模组设置中绑定单键或 Ctrl、Shift、Alt 组合键。

试玩版的 MelonLoader 设置不会自动迁移；请在游戏模组设置中重新设置快捷键。
