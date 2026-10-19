# Quick Counter Changelog / 更新日志

## 1.2.0 — 2026-09-22

- Performance and cleanup release for **Quick Counter**. / **Quick Counter** 性能优化与清理版。
- Added configurable hotkeys for opening the store, customer flow, transfer all, security actions, calendar, tablet, transaction confirmation, and trashing the hovered item. / 增加营业、顾客流程、转移全部、治安部、日历、平板、交易确认和鼠标悬停物品丢弃快捷键。
- Q, X, and R respect the game's native hold requirement and show the original circular progress UI. / Q、X、R 遵循游戏原版长按条件并显示原版圆形进度 UI。
- C supports normal transactions, wholesale, multi-item trades, and tablet requests. / C 支持普通买卖、批发、批量交易和平板请求。
- G reuses the game's original Delete-key trash logic. / G 复用游戏原版 Delete 键丢弃逻辑。
- Existing MelonPreferences category and entry identifiers are retained for configuration compatibility. / 保留已有 MelonPreferences 分类及配置键标识，以兼容开发版配置。
- Fixed shortcut conflicts with the game's mouse-left + Q/E item rotation and mouse-left + F item flip controls. / 修复与游戏原版鼠标左键 + Q/E 物品旋转、鼠标左键 + F 物品翻转的快捷键冲突。
- Removed idle per-frame StoreBell/StoreDismissButton scene searches. / 移除空闲帧中对 StoreBell/StoreDismissButton 的场景搜索。
- Cached parsed hotkeys and sampled modifier/mouse states once per frame. / 缓存已解析热键，并将修饰键/鼠标状态改为每帧只读取一次。
- Removed unused source code and replaced the old project API memo with the shared `D:\code\mod\docs` documentation set. / 清理未使用代码，并以共享的 `D:\code\mod\docs` 文档库替代项目内旧 API 备忘。
- Refreshes cached customer buttons safely after a scene change. / 场景切换后安全刷新缓存的顾客按钮对象。
- Renamed the internal namespace, class, and MelonPreferences category to QuickCounter; old ModTest configuration is intentionally not migrated. / 内部命名空间、类名和 MelonPreferences 分类改为 QuickCounter；旧 ModTest 配置不再迁移。
