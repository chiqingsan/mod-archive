# 开发配方与验证

## 配方：安全查找场景对象

```csharp
var button = UnityEngine.Object.FindObjectOfType<StoreBell>();
if (button == null)
    return;

if (!button.gameObject.activeInHierarchy)
    return;
```

对象可能在场景切换后销毁。不要长期缓存场景对象，除非同时处理场景变化或每次使用前重新检查。

## 配方：按钮状态检查

```csharp
if (button == null || !button.gameObject.activeInHierarchy)
    return;

var unityButton = button.GetComponent<UnityEngine.UI.Button>();
if (unityButton != null && !unityButton.interactable)
    return;

button.OnClick();
```

如果目标类型本身已经暴露 `Button` 属性（例如 `NegociationUIManager.acceptOfferButtonButton`），直接检查该属性即可。

## 配方：一次性按键

```csharp
if (Input.GetKeyDown(KeyCode.H))
    StoreUIManager.Instance?.storeCalendar?.OnClick();
```

配置热键时使用解析后的 `KeyCode`，并给每个动作单独保存状态，避免多个动作共享“已触发”标记。

## 配方：可取消长按

```csharp
private float _elapsed;
private bool _triggered;

private bool UpdateHold(bool held, bool requiresHold, float holdDuration, Action action)
{
    if (!held)
    {
        _elapsed = 0f;
        _triggered = false;
        CustomUIManager.Instance?.HideHoldRing();
        return false;
    }

    if (_triggered)
        return false;

    if (!requiresHold)
    {
        _triggered = true;
        action();
        return true;
    }

    _elapsed += Time.unscaledDeltaTime;
    var duration = Math.Max(holdDuration, 0.01f);
    CustomUIManager.Instance?.ShowHoldRing(Math.Min(_elapsed / duration, 1f));
    if (_elapsed < duration)
        return false;

    CustomUIManager.Instance?.HideHoldRing();
    _triggered = true;
    action();
    return true;
}
```

## 配方：交易确认

按当前 UI 优先级处理：确认面板 → 普通报价 → 批发 → 批量 → 平板请求。每一步都检查对象可见和按钮可交互，避免快捷键绕过原版 UI。

## 配方：程序集检查

```powershell
$cecil = 'D:\app\steam\steamapps\common\Probably Stolen Demo\MelonLoader\net6\Mono.Cecil.dll'
$game = 'D:\app\steam\steamapps\common\Probably Stolen Demo\MelonLoader\Il2CppAssemblies\Assembly-CSharp.dll'
Add-Type -Path $cecil
$assembly = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($game)
$type = $assembly.MainModule.Types | Where-Object { $_.Name -eq 'NegociationUIManager' }
$type.Methods | ForEach-Object ToString
$type.Properties | ForEach-Object ToString
$assembly.Dispose()
```

过滤类型名：

```powershell
$assembly.MainModule.Types |
  Where-Object { $_.Namespace -eq 'Il2Cpp' -and $_.Name -match 'Store|Item|Inventory|UIManager|Client' } |
  Select-Object -ExpandProperty Name |
  Sort-Object -Unique
```

## 配方：用 ilspycmd 反编译查看签名

仓库自带反编译工具：`tools/ilspycmd/ilspycmd.exe`。

查看单个类型（类型名要带 `Il2Cpp.` 命名空间）：

```powershell
tools\ilspycmd\ilspycmd.exe -t Il2Cpp.StoreUIManager `
  "D:\app\steam\steamapps\common\Probably Stolen Demo\MelonLoader\Il2CppAssemblies\Assembly-CSharp.dll"
```

列出全部类型名（用于确认类型存在与命名空间）：

```powershell
tools\ilspycmd\ilspycmd.exe -l c <Assembly-CSharp.dll 路径>
```

只提取某个类型的成员清单，适合快速定位方法名：

```powershell
tools\ilspycmd\ilspycmd.exe -t Il2Cpp.WaterHelper <dll> |
  Select-String 'NativeMethodInfoPtr_' |
  ForEach-Object { ($_ -split 'NativeMethodInfoPtr_')[1] } |
  Sort-Object -Unique
```

### 关键限制：方法体是原生的

IL2CPP 包装器里的方法体只是 `il2cpp_runtime_invoke` 调用桩，**看不到任何业务逻辑**。反编译能拿到的是：

- 方法名、参数类型、返回类型
- **参数名和默认值**（完整签名，比 Cecil 的 `ToString()` 更详细）
- 字段名、属性名
- `[CallerCount(n)]` 和 `[CachedScanResults]` 属性，可用于推测调用热度

拿不到的是：条件判断、副作用、调用顺序、失败时的行为。**因此行为结论必须在游戏内验证，不能从反编译结果推断。**

### 教训：默认值能揭穿误读的签名

签名里的默认值是最容易被忽略、也最容易查错的地方。实例：

```csharp
void Notify(string message, string color = "default");
```

`Notify` 曾被文档和代码误当成 `Notify(title, body)` 使用，结果界面只显示第一个参数。默认值 `"default"` 暴露了第二个参数是颜色标签而非正文——如果只看参数类型（都是 `string`）或只看 Cecil 的名称列表，都发现不了这个错误。

**每次调用游戏 API 前，先用本配方打出完整签名并检查默认值。**

## 判断接口可靠程度

- **已确认签名**：Cecil 能从当前程序集读取类型、属性或方法。
- **已实机验证**：当前 Quick Counter 已实际调用并由用户测试，例如按钮 `OnClick()`、交易确认、垃圾箱入口和长按圈。
- **待验证行为**：只知道签名，参数语义或副作用未知。文档中会明确提示。
- **高风险内部接口**：存档、事件、底层库存集合和直接状态字段；测试前必须备份存档。

## 发布前检查清单

- [ ] 使用当前游戏 `Assembly-CSharp.dll` 重新确认签名。
- [ ] 所有单例、按钮和集合访问都有空值、激活状态检查。
- [ ] 长按动作有原版时长、进度圈、松开重置和一次触发保护。
- [ ] 不直接改底层集合，不在每帧保存。
- [ ] Release 构建为 0 错误、0 警告。
- [ ] 部署 DLL 与 Release DLL 的 SHA-256 一致。
- [ ] 文档标注游戏版本、程序集日期和已知限制。
- [ ] 使用独立存档完成至少一次真实游戏测试。
- [ ] 发布版关闭诊断日志和逐项扫描输出。
- [ ] 发布包包含 DLL、安装说明、功能说明、变更记录和版本号。
- [ ] 发布包不包含封面图。此前试过 `cover.svg` / `*-cover.png`，已确认不要放入发布包，不要再加回来。

## Quick Counter 性能审查记录（1.2.0）

本版本已针对 `OnUpdate()` 高频路径完成优化：

- Q/X 对应的 `StoreBell`、`StoreDismissButton` 只在快捷键实际按住时查找，不再空闲帧全场景搜索。
- 热键字符串只在 MelonPreferences 值发生变化时解析，不再每帧执行 `Split` 和 `Enum.TryParse`。
- Ctrl、Shift、Alt 和鼠标左键状态每帧各读取一次，再供所有热键复用。
- 删除未使用的 `PlayerStore.Instance` 读取和 `HotkeyState.Started` 状态。
- E、F 的按钮搜索以及 C/G 的交易、垃圾箱逻辑仍只在对应快捷键触发时执行，不属于持续高频路径。

后续新增快捷键时，优先遵循“缓存解析结果、按需查找场景对象、只在触发时执行 UI 扫描”的模式。
