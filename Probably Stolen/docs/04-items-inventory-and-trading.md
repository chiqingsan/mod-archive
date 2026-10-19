# 物品、库存与交易

## GameItem 核心属性

`GameItem` 是游戏物品的核心类型，当前确认的常用属性包括：

```csharp
string identifier;
string identifierName;
int uniqueId;
string shortDescription;
string longDescription;
string flavorText;
int unitCount;
long unitValue;
long unitBaseValue;
GameInventory parentInventory;
Il2CppSystem.Collections.Generic.List<ItemFeature> itemFeatures;
Il2CppSystem.Collections.Generic.List<string> itemTypes;
```

常用方法：

```csharp
static GameItem Create(string identifier, string identifierName, PixelWindow contentWindow);
string GetDisplayName(bool includeQuantity);
long GetBaseValue();
long GetCurrentValue(bool a, bool b, bool c, bool d, long value);
long GetNegociatedValue();
GameItem SetValue(long value);
GameItem SetUnitCount(int count);
GameItem SetAmount(int amount);
bool IsTag(string tag);
bool IsGameItemType(string type);
void SetType(string type);
GameItem SetName(string name);
GameItem CloneLinked();
void Destroy();
```

不要只修改 `unitValue` 就假设 UI、特性和交易价格都会同步；优先使用游戏提供的 Setter 或上层交易入口。`GetCurrentValue` 的布尔参数含义仅凭包装程序集无法可靠确定，使用前应检查调用点并在游戏内验证。

## 物品标识符

### 标识符不在程序集里

`Assembly-CSharp.dll` **不包含物品标识符字符串**，它们存放在游戏数据文件里：

```text
Probably Stolen_Data\resources.assets
```

程序集里只有类型和方法签名，所以不要指望用反编译程序集的办法找到物品 ID。

### 查找方法

物品的本地化键遵循固定命名，可用它反查标识符：

```text
item_<identifier>_name      物品名
item_<identifier>_desc      描述
item_<identifier>_flavor    风味文本
```

例如在 `resources.assets` 里可以读到：

```text
item_uv_filter_name      -> 紫外线灯
item_uv_filter_desc      -> 可用于水容器
item_water_filter_name   -> 滤水器
item_water_filter_adv_name -> 高级滤水器
item_hydroreed_filter_name -> 韧棉织物
```

反查步骤：

1. 在 `resources.assets` 里按 UTF-8 搜索物品的中文名或描述，拿到它附近的定位。
2. 在该文件里搜索 `item_*_name` 形式的键，列出全部物品条目。
3. 键名去掉 `item_` 前缀和 `_name` 后缀，剩下的通常是运行时 `identifier`。

运行时再用 `GameItem.identifier` 校验一次，不要只靠数据文件推断。

### 不要用"名字包含某个词"判断物品类别

同一类功能的物品，identifier 可能包含误导性单词。已确认的例子：

| identifier | 实际物品 | 陷阱 |
| --- | --- | --- |
| `uv_filter` | 紫外线灯 | 含 `filter`，但不是滤芯 |
| `hydroreed_filter` | 韧棉织物 | 含 `filter`，游戏描述写明无法用于净水 |
| `black_lamp` | 黑光灯 | 名称含 `lamp`，与紫外线灯是不同物品 |

因此**判断物品类别时必须先排除已知的非目标物品**，不能只做关键字正向匹配；否则会把无关物品当成目标处理。分类判断还应优先使用 identifier（稳定、不随语言变化），而不是显示名（会本地化）。

## GameInventory / GameGridInventory

基础库存接口：

```csharp
GameItem GetParentItem();
bool MayHaveValidInventorySlot(GameItem item);
SlotMarker TryFindOneValidInventorySlot(GameItem item);
bool UncheckedAccept(GameItem item);
bool Expel(GameItem item);
GameInventory UncheckedAcceptAll(Il2CppSystem.Collections.Generic.List<GameItem> items);
GameInventory ExpelAll();
bool IsInsertLocked();
bool IsRemoveLocked();
void LockInv();
void UnlockInv();
void Validate();
```

`GameGridInventory` 额外提供：

```csharp
GameGridInventory SetShape(int width, int height);
GameGridInventory SetShape(string shape, int orientation);
bool CheckShapePlacement(GameItem item, GridShape shape,
    Il2CppSystem.Collections.Generic.List<GameItem> excludedItems);
SlotMarker TryFindOneValidInventorySlot(GameItem item, bool topMiddle);
```

`GameSlotInventory` 适合单槽位物品，并提供 `currentItem`、`childItem`、`Hide()`、`Show()` 和背景精灵设置方法。

## 物品移动建议

1. 先检查源库存和目标库存是否存在、是否锁定。
2. 用 `MayHaveValidInventorySlot` 或 `TryFindOneValidInventorySlot` 预检。
3. 通过 `UncheckedAccept/Expel` 或游戏已有 UI 操作移动，不要直接改集合。
4. 移动后调用 `Validate()`，必要时触发对应的游戏刷新入口。
5. 物品移动涉及 UI 时，等待下一帧检查显示状态，不要立刻假设布局已重建。

`UncheckedAccept` 名称表明它可能跳过部分用户交互检查；使用前必须自行验证容量、锁定状态和物品归属。

### 嵌套容器遍历

需要检索玩家全部物品时，`PlayerStore.FindAllItem(true)` 只能作为根入口；容器中的库存还需要继续沿 `GameItem.child`、`GameItem.children`、`GraphNodeStorage.children` 和各类 `GameInventory` 的物品集合展开。使用 IL2CPP 对象指针去重，避免同一对象从多个图节点重复处理。

不要为了监听普通物品移动而默认给 `GameInventory.UncheckedAccept`、`Expel` 或 `GraphUtils.SwapItemsInPlace` 加 Harmony Postfix。当前游戏中 `UncheckedAccept` 的补丁曾导致启动阶段原生崩溃；需要物品移动时优先使用游戏已有操作，并进行实机验证。

## 买卖与协商

### PlayerStore 交易入口

```csharp
bool CanBuyItemFromTable();
bool CanSellThisItem(GameItem item, bool fromTable);
bool CanSellItemFromTable(bool value);
void BuyItemFromTable();
void BuyItem(GameItem item);
void SellItem(GameItem item);
void SellItemFromTable();
int GetWholesalePrice();
void AcceptBarter();
```

这些是业务层入口，直接调用可能跳过 UI 状态。快捷键或 UI 扩展应优先调用 `NegociationUIManager` 的按钮逻辑。

### NegociationUIManager

```csharp
GameObject uiObject;
GameObject confirmPanel;
GameObject acceptOfferButton;
Button acceptOfferButtonButton;
Button wholesaleButton;
Button acceptMultipleButton;
GameItem currentNegociatedItem;
```

推荐确认顺序：

```text
confirmPanel 可见
  -> OnConfirmPanelYes()
普通报价按钮可见且 interactable
  -> OnOfferAcceptClick()
普通报价不可用但 wholesaleButton 可用
  -> OnWholesaleClick()
acceptMultipleButton 可用
  -> OnAcceptMultipleButtonClick()
最后检查 PCUIManager 的 StoreRequestElement
```

其他已确认入口：

```csharp
void OpenUI();
void CloseUI();
void ToggleUI();
void OnConfirmPanelYes();
void OnConfirmPanelNo();
void OpenConfirmPanel();
void TrySellItem();
void SellItem();
void BuyItem();
void RefreshUI();
void UpdateUI(GameItem item);
int GetFinalOfferValue(GameItem item);
```

`BuyItem()`、`SellItem()`、`PlayerStore.BuyItem()` 等底层入口不适合作为默认快捷键目标，因为可能绕过确认面板、按钮禁用、对话和预算检查。
