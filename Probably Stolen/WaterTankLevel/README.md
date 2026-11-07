# 储水罐贴图优化

替换原版的储水罐贴图，更加直观地显示储水罐中的水位情况，支持六档水位及液体颜色显示。
悬停提示仍显示精确容量；不修改容量、液体或存档中的原版贴图路径。

## 当前版本

1.1.6，适配游戏 0.46D，仅需 reflection-external 权限。尚未部署或上传。
本版源码已对齐实际部署的 1.1.5 自制贴图实现；使用原有做旧灰绿素材，未调整美术。
此前根目录保留的 1.1.3 原版图集方案已在本轮修改前备份至
`D:/code/mod/tmp/WaterTankLevel-source-before-1.1.6-20261105-042330/`。

## 修复

原版 WaterTankHelper.Create 先设置贴图并刷新，之后才写入 water_tank 标识。
首次刷新不满足模组匹配条件，而后续刷新不再重载桶身，导致新建物品显示原版贴图。
本版在该工厂方法返回后，对符合原版储水罐条件的物品调用一次 ValidatePartialSprite，
经既有贴图解析补丁刷新正常桶身及拖拽镜像，不重建物品形状、不修改存档字段。
已有的临时液体图层委托和液体变化刷新补丁保持原有行为；其他模组自定义外观仍排除。

## 构建及完整包

- 构建：`dotnet build -c Release WaterTankLevel.csproj`。
- 逻辑回归：`dotnet run -c Release --project ../WaterTankLevel.Tests/WaterTankLevel.Tests.csproj`。
- 完整待部署包：`release/1.1.6/`，包含 DLL、manifest.xml、preview.png 及 Textures/Items 下 16 张 64×64 PNG。
- 素材沿用 `design-candidates/worn-sage-v1.1.5/ModContent-1.1.5/Textures/`；部署必须同步完整包。
- 补丁安装/卸载由游戏 ModLoader 负责，模组不重复 PatchAll。

测试通过：初始化容量保护、异常时临时委托清理、新建标识赋值顺序、桶身与拖拽镜像刷新、
空罐及五档非空水位、液体变化刷新、自定义外观排除、禁用和缺失素材回退。
这些测试使用替代游戏对象和手动调用补丁方法，不能代表 Unity/Mono 内实际 Harmony 挂接通过。
仍需完整重启游戏后验证新生成的储水罐、读档已有储水罐及灌水/倒水。
