# 商店与 UI API

以下接口均已在当前 `Assembly-CSharp.dll` 中确认过公开签名。方法是否适合直接调用，还要结合当前场景状态验证。

## 单例与主界面

```csharp
var ui = StoreUIManager.Instance;
var store = PlayerStore.Instance;
var customUi = CustomUIManager.Instance;
```

`StoreUIManager` 常用入口：

```csharp
bool IsTyping();
void TryTrashHoveredItem();
void CloseAllUI();
void QueueClientLeaveAnimation(bool right);
void OnClientStartLeaving(bool right);
void DisplayClient();
void HideClient();
void OnNextClientArrived();
void Notify(string message, string color = "default");
```

`Notify` 的第二个参数是颜色标签而非正文，传两个字符串会导致界面只显示第一个参数。它渲染为对话气泡样式（`TextBubble` / `dialogBubbleList`），游戏未提供独立的 Toast 控件。

常用属性包括 `storeCalendar`、`clientCounter`、`pcScreenObject`、`inventoryCanvas`、`hoveringStoreUI`。

`TryTrashHoveredItem()` 是游戏 Delete 键对应的高层入口。复用它比自行读取鼠标位置和销毁 `GameItem` 更安全。

## 营业与顾客按钮

### StoreShutterButton

```csharp
var shutter = StoreShutterButton.Instance;
bool isClosed;
void OnClick();
void OpenShutter();
void CloseShutter();
void ToggleShutterDoor();
```

模拟玩家点击时优先 `OnClick()`，不要直接调用 `OpenShutter()` 绕过原版检查。

### StoreBell

```csharp
var bell = UnityEngine.Object.FindObjectOfType<StoreBell>();
bool ShouldRequireHold();
float HoldDuration;
void OnClick();
void RingBell();
```

### StoreDismissButton

```csharp
var dismiss = UnityEngine.Object.FindObjectOfType<StoreDismissButton>();
bool ShouldRequireHold();
float HoldDuration;
void OnClick();
```

### StoreSecButton

```csharp
var security = StoreSecButton.Instance;
bool ShouldRequireHold();
float HoldDuration;
bool IsClientArrestable();
void OnClick();
void ResetLight();
```

### StoreTransferButton

```csharp
var transfer = UnityEngine.Object.FindObjectOfType<StoreTransferButton>();
void OnClick();
```

## PlayerStore 顾客与营业流程

```csharp
bool CanStartDay();
void OpenShutter();
void BeginDay();
bool CanNextClient();
void TryCallNextClient();
bool IsClientPresent();
bool IsClientDimissable(); // 游戏程序集中的原始拼写
void DismissCurrentClient(bool callNextWhenPossible, bool right, bool interruptDialog);
void EndDay();
void EndNight();
```

`DismissCurrentClient(...)` 是业务层方法，直接调用可能绕过按钮长按、动画和 UI 反馈。顾客操作一般应调用实体按钮 `OnClick()`。

## 原版长按与进度圈

```csharp
var duration = Math.Max(button.HoldDuration, 0.01f);
elapsed += Time.unscaledDeltaTime;
CustomUIManager.Instance?.ShowHoldRing(Math.Min(elapsed / duration, 1f));
// elapsed >= duration 后调用原版 OnClick()
CustomUIManager.Instance?.HideHoldRing();
```

规则：

1. 先读取 `ShouldRequireHold()`，不需要长按时只触发一次。
2. 需要长按时用 `Time.unscaledDeltaTime` 累计，不受游戏暂停/时间缩放影响。
3. 松开、目标消失或动作完成时都要 `HideHoldRing()`。
4. 触发后保持 `Triggered` 状态，直到按键松开，防止一次长按连续执行。

## 日历、平板与电脑

```csharp
StoreUIManager.Instance?.storeCalendar?.OnClick();

var tablet = UnityEngine.Object.FindObjectOfType<StoreTablet>();
tablet?.OnClick();
```

`StoreCalendar` 还提供 `GetTooltip()`、`IsTooltipEnabled()`、`GetDayOfWeek()`。

`StoreTablet` 提供 `GetTooltip()`、`IsTooltipEnabled()`、`OnClick()`。

## 物品旋转/翻转输入冲突

游戏原版在鼠标左键按下拖动物品时使用以下组合：

- 鼠标左键 + Q：旋转物品
- 鼠标左键 + E：旋转物品
- 鼠标左键 + F：翻转物品

Quick Counter 会在鼠标左键按下期间暂停对应的 Q、E、F 快捷键，并重置自己的触发/长按状态，让组合键交给游戏原版处理。该判断只对未加 Ctrl、Shift、Alt 的 Q/E/F 生效；如果玩家在 XIAOWO 中把快捷键改成其他按键或组合键，不会被这条冲突规则拦截。

## PCUIManager

```csharp
var pc = PCUIManager.Instance;
void OpenUI();
void HideUI();
void ToggleUI();
GameObject panel;
Il2CppSystem.Collections.Generic.List<StoreRequestElement> storeRequestElements;
```

处理请求时检查 `panel.activeInHierarchy`、`acceptButton.gameObject.activeInHierarchy` 和 `acceptButton.interactable`，再调用 `StoreRequestElement.OnClick()`。

## 自定义 UI

`CustomUIManager` 当前确认的入口：

```csharp
CustomUIBuilder Window(string id, string title, string layer);
CustomUIBuilder CreateWindow(string id, string title, string layer);
CustomUIWindow GetWindow(string id);
bool IsOpen(string id);
void CloseWindow(string id);
void CloseAllWindows();
CustomUIElement GetElement(string windowId, string elementId);
RectTransform OverlayRoot();
Sprite GetSprite(string id);
Sprite GetModSprite(string modId, string spriteId);
void ShowTooltip(string text, CustomUITooltipTrigger owner);
void HideTooltip(CustomUITooltipTrigger owner);
void ShowHoldRing(float progress);
void HideHoldRing();
```

建议先用 `Window/CreateWindow` 创建窗口，再通过返回的 Builder 添加控件；不要直接改 `windowsById` 或 `windowOrder`。

## 消息提示的可用控件

游戏**没有独立的 Toast / Notification 控件类型**。按类型名搜索 `toast`、`hint`、`notification` 在程序集里没有命中。想要"给玩家一条短提示"时，现有入口及其真实用途：

| 入口 | 实际用途 | 是否适合当提示 |
| --- | --- | --- |
| `StoreUIManager.Notify(message, color)` | 对话气泡（`TextBubble` / `dialogBubbleList`） | 可用；样式是气泡，不是滑入式提示条 |
| `ReputationUIManager.Notify(message, color)` | 声望提示条，字段全是声望专perk 图与文案 | 仅适合声望系统 |
| `SplashTextManager.DisplayFloatText(text, TextMode)` | 战斗飘字，`TextMode` 是 Damage / Heal / Stun 等 | 不适合 |
| `TooltipManager.SetAndShowTooltip(text)` | 跟随鼠标的物件 tooltip | 不适合 |
| `LogUIManager` / `RecorderUIManager` | 可滚动的记录面板，需手动开关 | 不适合 |

结论：需要通用短提示时用 `StoreUIManager.Notify`，并接受它的对话气泡样式；不存在"顶部滑入并自动消失"的原版控件。

## RichTextBuilder（Tooltip 富文本）

Tooltip 由 `RichTextBuilder` 构建。常用入口：

```csharp
RichTextBuilder AddLine(string text = null, bool ignoreEmpty = true,
    RenderHandler.ColorPalette color = RenderHandler.ColorPalette.None,
    bool bold = false, bool italic = false, bool underline = false, bool strikethrough = false,
    RenderHandler.ColorPalette backgroundColor = RenderHandler.ColorPalette.None,
    RenderHandler.ColorPalette underlineColor = RenderHandler.ColorPalette.None,
    RenderHandler.ColorPalette strikethroughColor = RenderHandler.ColorPalette.None);

RichTextBuilder Add(string text,
    RenderHandler.ColorPalette color = RenderHandler.ColorPalette.None, string end = "", ...);

RichTextBuilder AddProgressBar(float ratio, int length,
    RenderHandler.ColorPalette color = ..., RenderHandler.ColorPalette backgroundColor = ..., ...);

RichTextBuilder AddEmptyLine(int count);
static string Color(string text, RenderHandler.ColorPalette color);
static string Style(string text, bool bold, bool italic = false);
```

### 陷阱：AddLine 有四个颜色参数

`AddLine` 的参数顺序是 `text, ignoreEmpty, color, bold, italic, underline, strikethrough, backgroundColor, underlineColor, strikethroughColor`。**把同一个颜色同时传给 `color` 和 `backgroundColor` 会在该行背后画出整行宽度的色带**（实机出现过金色横条）。只想改文字色时，其余三个颜色参数必须传 `None`。

### 颜色值可以任意十六进制

`RenderHandler.ColorPalette` 是 int 底层枚举，**枚举值就是字面的 `0xRRGGBB` 颜色**（例如 `White = 12895408 = 0xC4C4B0`）。因此不必局限于枚举里预定义的颜色，任意十六进制都可以强制转换后传入：

```csharp
private const int NoColor = -1;                 // 等于 ColorPalette.None
private const int CustomColor = 0xCCB987;

builder.AddLine(text, true,
    (RenderHandler.ColorPalette)CustomColor,    // 文字色
    false, false, false, false,
    (RenderHandler.ColorPalette)NoColor,        // backgroundColor
    (RenderHandler.ColorPalette)NoColor,        // underlineColor
    (RenderHandler.ColorPalette)NoColor);       // strikethroughColor
```

### 结论口径

- `AddLine` / `Add` / `AddProgressBar` 签名：已确认签名。
- 自定义十六进制颜色、`backgroundColor` 色带：已实机验证（AquaSense 滤芯耐久行）。
- `ColorPalette` 枚举值等于 `0xRRGGBB`：由 `White`、`VeryLightGray` 等多个枚举值与其十六进制表示逐一对齐确认。
