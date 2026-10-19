# 顾客、电话、事件与存档

## 顾客管理

`PlayerStore` 常用状态：

```csharp
StoreClientInstance currentClientInstance;
StoreClientManager storeClientManager;
GameItem currentNegociatedItem;
bool isClientArrived;
bool isClientBeingArrested;
```

`StoreClientManager` 已确认入口：

```csharp
void GenerateClient();
StoreClient PickClient();
StoreClient PickRandomClient(string clientId);
StoreClient PickRandomValidClient(string clientId);
StoreClient PickShopperClient();
StoreClient PickSupplierClient();
bool IsClientValid(StoreClient client);
bool IsClientOnCooldown(StoreClient client);
bool IsClientOnCooldown(string clientId);
void AddClient(StoreClient client);
void TryAddClient(StoreClient client, bool value);
void AddNextClient(StoreClient client);
void HandleNormalClient();
void HandleSupplierClient();
void HandleInspectionClient();
void HandleServiceClient();
void DismissClientStack();
int GetNormalCustomerCount();
```

顾客选择受日期、解锁、冷却、故事线和当前库存影响；不要在 Mod 中假设 `PickClient()` 每次都会返回普通顾客。未知布尔参数的语义需要通过调用点或实机测试确认。

## 电话与治安部

```csharp
var phone = PhoneUIManager.Instance;
phone.OpenUI();
phone.AutoCall(longNumber);
phone.StopCall();
bool willAnswer = phone.WillAnswerCall(longNumber);
```

`PhoneClientList` 提供预定义联系人的构造入口，例如：

```csharp
StorePhoneClient CreateSecPhoneClient();
StorePhoneClient CreateCommissaryPhone();
StorePhoneClient CreateWildClient();
```

如果场景中有 `StoreSecButton`，优先调用它的 `OnClick()`，因为它包含当前可逮捕判断、长按时长和按钮反馈。`PhoneUIManager.AutoCall` 适合无实体按钮时的备用方案。

`PlayerStore.PhoneClientDict` 是联系人字典；其中 `locID`、`dialogFuncId`、`displayName` 可用于当前版本的识别，但不应视为跨版本稳定 ID。

## StoreOperationManager

当前确认的状态和方法：

```csharp
int lowerLevelUnrest;
int upperLevelFriendliness;
int revPower;
int secPower;
int blackMarketPower;
Il2CppSystem.Collections.Generic.List<string> availableOperations;

void OnDayStart();
void HandleBlackMarket();
void HandleUpperLevel();
void HandleLowerLevel();
void HandleRevolution();
void HandleSecurity();
void QueueOperation();
bool CanFactionQueueOperation(string operationId);
bool CanQueueThisOperation(string operationId);
bool IsOperationActive(string operationId);
bool IsOperationInQueue(string operationId);
bool IsOperationOnCooldown(string operationId);
```

操作 ID 来自当前版本数据，不建议硬编码未从运行时或数据文件验证过的字符串。

## 事件与通知

`StoreEventManager`、`EventTriggerManager`、`StoreEvent` 和各类 `*Event` 类型负责游戏事件。事件签名和触发条件更新风险较高，推荐：

- 先通过 Cecil 确认当前类型和公开入口。
- 优先调用已有按钮或 UI 入口，而不是直接触发事件对象。
- 事件测试使用独立存档，并记录触发前后的 `PlayerStore` 状态。

商店 UI 的简单反馈可使用：

```csharp
StoreUIManager.Instance?.Notify("消息内容");
```

注意 `Notify` 的真实签名是 `Notify(string message, string color = "default")`：**第二个参数是颜色标签，不是正文**。传两个字符串会把正文当成颜色，界面上只显示第一个参数。需要指定颜色时传颜色标签，例如 `Notify(message, "default")`。

## 金钱

当前确认的 `PlayerStore` 接口：

```csharp
int GetCash();
bool CanPay(int value);
void PayMoney(int value, bool value2);
void ModCash(int value, bool value2);
int playerCash;
```

推荐优先使用方法，而不是直接改 `playerCash`。两个布尔参数的业务含义无法仅由包装程序集可靠确定，使用前应查找调用点或实机验证。

## 存档边界

`PlayerStore` 当前确认存在：

```csharp
void InitialSave();
void SaveGame();
void WriteSlotFile();
void LoadGame();
PlayerStore PreLoadStore();
Il2CppSystem.Collections.Generic.Dictionary<string, string> modData;
```

可以考虑把 Mod 自己的少量状态放在 `PlayerStore.modData`，但这条路径尚未经过独立 Mod 的完整读写周期验证。使用带 Mod 前缀的键名，例如 `QuickCounter.SomeFlag`，避免冲突；写入前备份存档，不要每帧 `SaveGame()`。

`SaveState`、`SaveFiles`、`SaveManager` 是更底层的存档实现，适合研究和兼容性补丁，不建议普通功能 Mod 直接依赖其内部字段。
