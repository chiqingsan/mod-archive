# MelonLoader 生命周期与配置

## Mod 入口

```csharp
using MelonLoader;

[assembly: MelonInfo(typeof(MyMod.Main), "My Mod", "1.0.0", "Author")]
[assembly: MelonGame("Questing Goose Studio", "Probably Stolen")]

namespace MyMod;

public sealed class Main : MelonMod
{
    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg("My Mod loaded");
    }

    public override void OnUpdate()
    {
        // 适合轻量输入轮询；不要在这里做重型扫描或频繁写存档。
    }
}
```

常用生命周期：

- `OnInitializeMelon()`：注册配置、初始化静态状态、输出加载日志。
- `OnUpdate()`：每帧轮询输入和轻量状态。
- Harmony 补丁：只有在确实需要拦截或扩展原版方法时才使用。

## MelonPreferences

```csharp
var category = MelonPreferences.CreateCategory("MyMod", "我的 Mod 设置");
var enabled = category.CreateEntry("Enabled", true, "启用功能");
var hotkey = category.CreateEntry("OpenHotkey", "F", "打开快捷键");
```

兼容原则：

- 内部分类标识（例如 `MyMod`）和配置 Identifier 一旦发布就不要随意改名。
- 显示名称可以变更，但不应替换内部标识。
- 热键字符串可使用 `G`、`Ctrl+Shift+M`、`BackQuote` 等形式。
- XIAOWO Mod 管理器会识别 `UnityEngine.KeyCode` 类型，或 Identifier 含有 `hotkey`、`shortcut`、`keybind` 的字符串项。
- `` ` `` 和 `·` 可规范化为 `BackQuote`，以便管理器识别。

## 组合键与一次触发

```csharp
var modifiersDown = ctrl == (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                 && shift == (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                 && alt == (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
var held = modifiersDown && Input.GetKey(key);
```

长按或重复触发的动作要保存状态：按键松开时重置；动作完成后在本次按住期间不再重复执行。

## 日志

```csharp
LoggerInstance.Msg("信息");
LoggerInstance.Warning("可恢复问题");
LoggerInstance.Error("错误");
```

不要在 `OnUpdate()` 每帧输出日志；需要调试时加入节流或只在状态变化时输出。
