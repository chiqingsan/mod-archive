# CustomerCreditBoost 正式版适配

CustomerCreditBoost 将旧版 CustomerCreditBoost 的 MelonLoader/IL2CPP 实现适配为 Probably Stolen 正式版内置 Mod API。

当顾客由展示柜中的物品或物品类型吸引时，模组会汇总展示柜内对应物品的基础价值。默认每累计 100 基础价值提高该顾客 8% 的预算；不匹配的顾客不会获得这项加成。默认档位和百分比都可以在游戏的模组设置中修改。

构建：`dotnet build -c Release`。安装时将 `manifest.xml` 和生成的 `CustomerCreditBoost.dll` 放入游戏 `Mods/CustomerCreditBoost` 文件夹，然后在游戏模组界面启用。
