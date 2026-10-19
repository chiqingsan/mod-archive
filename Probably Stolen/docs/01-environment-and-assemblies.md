# 环境与程序集

## 游戏与 MelonLoader 路径

默认本机路径：

```text
D:\app\steam\steamapps\common\Probably Stolen Demo
```

关键程序集：

```text
MelonLoader\net6\MelonLoader.dll
MelonLoader\net6\0Harmony.dll
MelonLoader\net6\Il2CppInterop.Runtime.dll
MelonLoader\net6\Mono.Cecil.dll
MelonLoader\Il2CppAssemblies\Assembly-CSharp.dll
MelonLoader\Il2CppAssemblies\Il2Cppmscorlib.dll
MelonLoader\Il2CppAssemblies\UnityEngine.CoreModule.dll
MelonLoader\Il2CppAssemblies\UnityEngine.InputLegacyModule.dll
MelonLoader\Il2CppAssemblies\UnityEngine.UI.dll
```

当前可工作的 Mod 项目设置：

```xml
<TargetFramework>net6.0</TargetFramework>
<PlatformTarget>x64</PlatformTarget>
<RootNamespace>QuickCounter</RootNamespace>
```

游戏类型主要位于 `Il2Cpp` 命名空间。Unity 类型仍来自 `UnityEngine`、`UnityEngine.UI` 等程序集。

## 最小引用

```xml
<Reference Include="MelonLoader">
  <HintPath>$(GameDirectory)\MelonLoader\net6\MelonLoader.dll</HintPath>
  <Private>false</Private>
</Reference>
<Reference Include="0Harmony">
  <HintPath>$(GameDirectory)\MelonLoader\net6\0Harmony.dll</HintPath>
  <Private>false</Private>
</Reference>
<Reference Include="Il2CppInterop.Runtime">
  <HintPath>$(GameDirectory)\MelonLoader\net6\Il2CppInterop.Runtime.dll</HintPath>
  <Private>false</Private>
</Reference>
<Reference Include="Assembly-CSharp">
  <HintPath>$(GameDirectory)\MelonLoader\Il2CppAssemblies\Assembly-CSharp.dll</HintPath>
  <Private>false</Private>
</Reference>
```

按需再添加 `Il2Cppmscorlib`、`UnityEngine.CoreModule`、`UnityEngine.InputLegacyModule`、`UnityEngine.UI`。

## IL2CPP 使用注意

- 不要假设普通 Unity Mono Mod 的反射行为完全相同；优先直接引用生成的 `Il2Cpp.*` 类型。
- `Il2CppSystem.Collections.Generic.List<T>`、`Il2CppSystem.Collections.Generic.Dictionary<TKey,TValue>` 与标准 .NET 集合不是同一个运行时类型。
- 通过 `UnityEngine.Object.FindObjectOfType<T>()` 找到的对象可能为 `null`，并且场景切换后引用可能失效。
- 访问 UI 前检查 `GameObject != null`、`activeInHierarchy`，访问按钮还要检查 `interactable`。
- 不要在 `OnUpdate()` 中反复创建大量对象或执行存档写入。

## 构建与部署

```powershell
dotnet build .\YourMod.csproj -c Release --no-restore -v minimal
```

可在项目中加入自动部署目标：

```xml
<Target Name="DeployMod" AfterTargets="Build" Condition="Exists('$(GameDirectory)\Mods')">
  <Copy SourceFiles="$(TargetPath)" DestinationFolder="$(GameDirectory)\Mods" />
</Target>
```

发布前必须确认构建为 0 错误、0 警告，并比较构建 DLL 与 `Mods` 目录 DLL 的 SHA-256。
