using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PurifierTower;

/// <summary>
/// 净水箱与配套水瓶/水容器的物品工厂。
/// v0.4.0：
/// - 净水器区 21×18，放入限制照抄原版「机器区」machine_bay（ShipSystemDirectory.cs:132-146）。
/// - 待处理水区内置 100L 固定水箱、已处理水区内置 600L 固定水箱（真实液体容器，槽位锁定不可取出）；
///   把瓶子拖到水箱上 = 倒水进去；把空瓶拖到已处理水箱上 = 取水灌瓶。
/// - 待处理/已处理区各有一个只收「200L 水容器」的槽位；任一区放了自备容器时打印停用。
/// - 打印：按选定容量从已处理水箱取水，打出「装满已处理水的水瓶」放进输出区；
///   耗电 round(3 + 0.004 × ml)（1L 7 / 3L 15 / 6L 27 / 20L 83 / 60L 243），从电池区合并电量扣。
/// - 贴图全部复用游戏原版素材（塔/水箱 = items_tool2/water_tank，瓶 = water_container3/4）。
/// 净化/自动接水逻辑仍未实现（M1）。
/// </summary>
internal static class TowerFactory
{
    public const string TowerIdentifier = "purifier_tower";
    public const string ContainerIdentifier = "purifier_tower_container_200l";
    public const string Bottle1LIdentifier = "purifier_tower_bottle_1l";
    public const string Bottle3LIdentifier = "purifier_tower_bottle_3l";
    public const string Bottle6LIdentifier = "purifier_tower_bottle_6l";
    public const string Bottle20LIdentifier = "purifier_tower_bottle_20l";
    public const string Bottle60LIdentifier = "purifier_tower_bottle_60l";

    // 固定水箱容量（ml）。
    public const int PendingCapacityMl = 100000;
    public const int ProcessedCapacityMl = 600000;
    public const int PlayerContainerCapacityMl = 200000;

    private const string TankAtlas = "Items/items_tool2";
    private const string TankSprite = "water_tank";
    private const string TankSlotBackgroundAtlas = "Items/water_container4";
    private const string TankSlotBackgroundSprite = "xl";

    // v0.6.0：储水罐外观改用原版桶装水帧 Items/water_container4/xl。xl 与 water_tank 同为
    // 64×64 像素 / 4×4 格，换图不改 shape，槽位与存档都不受影响；液体层走原版透明容器机制
    // （帧名 xl_{blue|red|dark}_{10..100} + xl_hl_{档位}），因此能直接看到桶内水位。
    private const string TankBodyAtlas = "Items/water_container4";
    private const string TankBodySprite = "xl";

    // 五档打印容量（与瓶工厂一一对应）。
    private static readonly (int CapacityMl, string Label)[] PrintSizes =
    {
        (1000, "1L"),
        (3000, "3L"),
        (6000, "6L"),
        (20000, "20L"),
        (60000, "60L"),
    };

    private static readonly Color ColWater = new Color32(0x5F, 0xC4, 0xDD, 0xFF);
    private static readonly Color ColDirty = new Color32(0x9C, 0x8A, 0x5C, 0xFF);
    private static readonly Color ColPower = new Color32(0xF0, 0xCB, 0x5E, 0xFF);

    public static void RegisterInto(ItemDirectory directory)
    {
        Register(directory, TowerIdentifier, CreateTower);
        Register(directory, ContainerIdentifier, CreatePlayerContainer);
        Register(directory, Bottle1LIdentifier, CreateBottle1L);
        Register(directory, Bottle3LIdentifier, CreateBottle3L);
        Register(directory, Bottle6LIdentifier, CreateBottle6L);
        Register(directory, Bottle20LIdentifier, CreateBottle20L);
        Register(directory, Bottle60LIdentifier, CreateBottle60L);
        Debug.Log("[PurifierTower] registered item factories");
    }

    /// <summary>
    /// 幂等注册：目录里已有同 id 时跳过，避免 Directory.Add 内部先 Remove 再 TryAdd 的摘除/警告，
    /// 也避免 InitDirectory 被重复调用时把已注册物品顶掉。
    /// </summary>
    private static void Register(ItemDirectory directory, string identifier, Func<GameItem> builder)
    {
        if (directory.Has(identifier))
        {
            return;
        }

        directory.Add(identifier, () => SafeCreate(identifier, builder));
    }

    /// <summary>
    /// 工厂绝不能把异常抛给游戏/其他 mod（第三方物品工具会因此报「物品工厂失败」）。
    /// 先走正常构造；失败时记录完整栈，退化为「原版贴图 + 由贴图推导占格」的占位物品。
    /// </summary>
    private static GameItem SafeCreate(string identifier, Func<GameItem> builder)
    {
        try
        {
            return builder();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PurifierTower] factory '{identifier}' threw, using vanilla fallback:\n{ex}");
        }

        try
        {
            return CreateVanillaFallback(identifier);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PurifierTower] fallback for '{identifier}' also threw:\n{ex}");
            return ItemDirectory.CreateEmptyItem();
        }
    }

    private static GameItem CreateVanillaFallback(string identifier)
    {
        bool isTankLike = identifier == TowerIdentifier || identifier == ContainerIdentifier;
        string spriteAtlas = isTankLike ? TankAtlas : "Items/water_container3";
        string spriteName = isTankLike ? TankSprite : "normal";
        GameItem item = ItemDirectory.CreateEmptyItem(identifier)
            .SetSpriteAndShape(spriteAtlas, spriteName);
        item.name = isTankLike ? "海德拉净水箱（工厂异常）" : "水瓶（工厂异常）";
        item.unitValue = 0L;
        return item;
    }

    // ================= 物品 =================

    private static GameItem CreateTower()
    {
        try
        {
            return CreateTowerCore();
        }
        catch (Exception ex)
        {
            Debug.LogError("[PurifierTower] CreateTower threw:\n" + ex);
            throw;
        }
    }

    private static GameItem CreateTowerCore()
    {
        // ---- 内部区域 ----
        // 注意：本塔不使用 MachineHelper 的「电池槽 = bodyStorage[0]」约定（电池在页签里，不在顶层，
        // MachineHelper.GetBodyStorage 会把页签内所有库存展平后按面板顺序返回，语义与本塔不一致）。
        // 因此塔上不要调用 MachineHelper.GetBatterySlot/GetSlot/GetModules 这些按索引取槽的 API。
        var batteryInv = new GameGridInventory(4, 4);                 // 电池区 4×4
        var purifierInv = new GameGridInventory(21, 18);              // 净水器区 21×18（对齐原版机器区配置）
        var outputInv = new GameGridInventory(7, 4);                  // 输出区 7 列 × 4 行 = 112×64
        var uvSlot = new GameSlotInventory();
        var pendingTankSlot = new GameSlotInventory();                // 内置 100L 待处理水箱（锁定）
        var processedTankSlot = new GameSlotInventory();              // 内置 600L 已处理水箱（锁定）
        var pendingContainerSlot = new GameSlotInventory();           // 玩家自备 200L 容器（待处理区）
        var processedContainerSlot = new GameSlotInventory();         // 玩家自备 200L 容器（已处理区）

        batteryInv.mayInventoryAddItemFunc = (GameItem item, GameInventory _) =>
            GeneralHelper.IsItemOwned(item)
            && (item.IsTag("power_source_item") || item.identifier == "powerblock");

        // 放入限制照抄原版机器区：MachineBay() 里的 itemInventory.mayInventoryAddItemFunc（ShipSystemDirectory.cs:132-146）。
        purifierInv.mayInventoryAddItemFunc = MayMachineBayAccept;

        outputInv.mayInventoryAddItemFunc = (GameItem item, GameInventory _) =>
            GeneralHelper.IsItemOwned(item);

        uvSlot.mayInventoryAddItemFunc = (GameItem item, GameInventory _) =>
            GeneralHelper.IsItemOwned(item) && item.identifier == "uv_filter";

        // 固定水箱槽不可替换；玩家自备容器槽只收专用道具。
        pendingTankSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory __) => false;
        processedTankSlot.mayInventoryAddItemFunc = (GameItem _, GameInventory __) => false;
        pendingContainerSlot.mayInventoryAddItemFunc = IsPlayerContainer;
        processedContainerSlot.mayInventoryAddItemFunc = IsPlayerContainer;

        pendingTankSlot.SetBackgroundFadeSprite(TankSlotBackgroundAtlas, TankSlotBackgroundSprite);
        processedTankSlot.SetBackgroundFadeSprite(TankSlotBackgroundAtlas, TankSlotBackgroundSprite);
        pendingContainerSlot.SetBackgroundFadeSprite(TankSlotBackgroundAtlas, TankSlotBackgroundSprite);
        processedContainerSlot.SetBackgroundFadeSprite(TankSlotBackgroundAtlas, TankSlotBackgroundSprite);

        // 固定水箱放进去后不允许拖出（倒水/取水交互不受影响）。
        pendingTankSlot.LockInv();
        processedTankSlot.LockInv();

        if (DebugTools.Enabled)
        {
            purifierInv.onSlotAddItemEndFunc = (GameItem item, GameInventory _, SlotMarker __) =>
                Debug.Log($"[PurifierTower][probe] purifier bay inserted: {item.identifier}");
            uvSlot.onSlotAddItemEndFunc = (GameItem item, GameInventory _, SlotMarker __) =>
                Debug.Log($"[PurifierTower][probe] uv inserted: {item.identifier}");
        }

        // ---- 页面 ----
        var ui = new RunPageUi
        {
            BatteryInv = batteryInv,
            OutputInv = outputInv,
            PendingTankSlot = pendingTankSlot,
            ProcessedTankSlot = processedTankSlot,
            PendingContainerSlot = pendingContainerSlot,
            ProcessedContainerSlot = processedContainerSlot,
            SelectedCapacityMl = 6000,
        };

        // 自备容器放进/取出时刷新运行页读数（打印按钮状态依赖它）。
        pendingContainerSlot.onSlotAddItemEndFunc =
            (GameItem _, GameInventory __, SlotMarker ___) => RefreshRunPage(ui);
        pendingContainerSlot.onSlotRemoveItemEndFunc =
            (GameItem _, GameInventory __, SlotMarker ___) => RefreshRunPage(ui);
        processedContainerSlot.onSlotAddItemEndFunc =
            (GameItem _, GameInventory __, SlotMarker ___) => RefreshRunPage(ui);
        processedContainerSlot.onSlotRemoveItemEndFunc =
            (GameItem _, GameInventory __, SlotMarker ___) => RefreshRunPage(ui);

        var tabs = new MachineryTabs();
        tabs.AddTab("运行", CreateRunPage(ui));
        tabs.AddTab("净水器", CreatePurifierPage(purifierInv));
        tabs.AddTab("电池", CreateBatteryPage(batteryInv, uvSlot, ui));
        LogStage("tabs+pages built");

        var tabsIntake = new MachineryIntake(tabs, vents: false);
        var settingsPanel = new MachineryPanel(content: tabsIntake, role: "settings");

        // 显示名走 Unity Localization「Item」表（LocalizationInject 注入），缺表时用硬编码兜底。
        string towerName = LocalizationInject.GetItemName(TowerIdentifier, "海德拉净水箱");
        var window = new PixelWindow(canMove: true, title: towerName, style: WindowStyle.Machinery);
        window.Attach(new MachineryPanelRow(settingsPanel));
        // 打开窗口时补水箱（老存档里塔没有内置水箱）并刷新读数；开窗耗时打日志便于定位卡顿。
        window.openCallback = () =>
        {
            float start = Time.realtimeSinceStartup;
            EnsureFixedTanks(pendingTankSlot, processedTankSlot, ui);
            RefreshRunPage(ui);
            if (DebugTools.Enabled)
            {
                float ms = (Time.realtimeSinceStartup - start) * 1000f;
                Debug.Log($"[PurifierTower][trace] openCallback {ms:0} ms");
            }
        };
        LogStage("window built");

        // ---- 物品本体 ----
        GameItem tower = ItemDirectory.CreateEmptyItem(TowerIdentifier)
            .SetContentWindow(window)
            .SetSpriteAndShape(TankAtlas, TankSprite);
        LogStage("tower item + sprite");

        tower.name = towerName;
        tower.shortDescription = "自动接水、净化、达标灌装的净水箱（海德拉）。";
        tower.flavorText = "把净水器和电池放进去，它会在夜里替你干活。";
        tower.unitValue = 1250L;
        tower.SetGameItemType("MACHINE");
        LogStage("item type set");
        tower.EnableTag("CONTAINER_TAG");
        tower.EnableTag("manufacture");
        tower.EnableTag("STANDARD_MACHINE_TAG");
        LogStage("tags set");

        MachineryHelper.InitMachinery(tower, 0);
        LogStage("machinery init");

        // 少数加载路径（PlayerStore.DecodeSaveItem）会回调 onLoaded；那时子物品已解码完，补水箱/读数。
        tower.onLoaded = delegate(GameItem _)
        {
            EnsureFixedTanks(pendingTankSlot, processedTankSlot, ui);
            RefreshRunPage(ui);
        };

        // 注意：不能在建塔工厂里创建内置水箱——DirectoryMaster.Item 会把工厂产物的子物品全部
        // Destroy（DirectoryMaster.cs:246 的警告）。这里只登记运行时状态，等塔真正创建或窗口打开时再补建。
        Runtimes.Add(tower, new TowerRuntime(ui, pendingTankSlot, processedTankSlot));

        tower.onCycleEndLateSlotItemFunc = (GameItem self, GameInventory _, SlotMarker __) =>
        {
            if (!DebugTools.Enabled)
            {
                return;
            }

            Debug.Log($"[PurifierTower][probe] night-end-late tick: purifiers={purifierInv.childItems.Count}, "
                      + $"batteries={batteryInv.childItems.Count}, output={outputInv.childItems.Count}, "
                      + $"pending={TankMilliliters(pendingTankSlot)}, processed={TankMilliliters(processedTankSlot)}, "
                      + $"towerOwned={GeneralHelper.IsItemOwned(self)}");
        };

        RefreshRunPage(ui);
        return tower;
    }

    /// <summary>
    /// 原版机器区（machine_bay）的放入限制，逐字照抄 ShipSystemDirectory.cs:132-146。
    /// 效果：任何「玩家拥有」的非背包物品都能放（储物类是否嵌套由 ContainerHelper.IsNestingAllowed 决定）。
    /// </summary>
    private static bool MayMachineBayAccept(GameItem addedItem, GameInventory inv)
    {
        if (!GeneralHelper.IsItemOwned(addedItem))
        {
            return false;
        }

        GameItem parentItem = inv.GetParentItem();
        if (parentItem != null && !GeneralHelper.IsItemOwned(parentItem))
        {
            return false;
        }

        if (addedItem.IsTag("BACKPACK_TAG"))
        {
            return false;
        }

        return !addedItem.IsTag("STORAGE") || ContainerHelper.IsNestingAllowed(addedItem, parentItem);
    }

    private static bool IsPlayerContainer(GameItem item, GameInventory _)
    {
        return GeneralHelper.IsItemOwned(item) && item.identifier == ContainerIdentifier;
    }

    // ---- 固定水箱 / 自备容器 ----

    private static void EnsureFixedTanks(GameSlotInventory pendingTankSlot, GameSlotInventory processedTankSlot,
        RunPageUi? ui)
    {
        EnsureOneTank(pendingTankSlot, "待处理水箱", PendingCapacityMl, "待处理水 100L", takeWater: false, ui);
        EnsureOneTank(processedTankSlot, "已处理水箱", ProcessedCapacityMl, "已处理水 600L", takeWater: true, ui);
    }

    /// <summary>
    /// 单个固定水箱的补建与回调补齐。运行时补建（工厂阶段会被 DirectoryMaster.Item 清掉子物品），
    /// 补建后必须设成玩家所有，否则游戏的所有权门槛会拦掉倒水/取水交互。
    /// </summary>
    private static void EnsureOneTank(GameSlotInventory tankSlot, string name, int capacityMl, string description,
        bool takeWater, RunPageUi? ui)
    {
        try
        {
            if (tankSlot.childItem == null)
            {
                GameItem tank = CreateFixedTank(name, capacityMl, description);
                GeneralHelper.SetItemOwned(tank, true);
                if (tank.uniqueId == 0 && PlayerStore.Instance != null)
                {
                    tank.uniqueId = PlayerStore.Instance.GetUniqueID();
                }

                if (!tankSlot.UncheckedAccept(tank))
                {
                    Debug.LogError($"[PurifierTower] {name} 无法放进固定槽位，已销毁。");
                    tank.Destroy();
                    return;
                }
            }

            // 存档恢复出来的水箱走原版 water_tank 工厂，没有「拖空瓶取水」回调，这里补齐。
            ConfigureTankCallbacks(tankSlot.childItem!, takeWater, ui);
            // v0.6.0：读档会把存档里的旧外观覆盖回来，统一在这里重套桶装水外观与液体层（幂等）。
            ApplyTankLook(tankSlot.childItem);
        }
        catch (Exception error)
        {
            Debug.LogError($"[PurifierTower] EnsureFixedTanks({name}) threw:\n" + error);
        }
    }

    private static GameItem CreateFixedTank(string name, int capacityMl, string description)
    {
        // 用原版 identifier：存档恢复时游戏按 identifier 重建（water_tank 工厂已注册），
        // 再由存档的 state 覆盖容量与水位，所以这里给的容量只是新建时的初始值。
        GameItem tank = ItemDirectory.CreateEmptyItem("water_tank")
            .SetSpriteAndShape(TankAtlas, TankSprite); // 先用原版水罐帧定 4×4 shape
        tank.name = name;
        tank.shortDescription = description + "（海德拉净水箱内置，固定不可取出）。";
        tank.unitValue = 0L;
        // v0.6.0：打开透明容器水位层（原版水瓶同一套机制），随后换成桶装水外观。
        WaterHelper.InitLiquidContainerItem(tank, capacityMl, 0,
            isFilterContainer: false, isGraduated: true, isDropper: false, isTransparentContainer: true,
            sheetPage: 4);
        ApplyTankLook(tank);
        AudioHelper.InitSolidMetal(tank);
        return tank;
    }

    /// <summary>
    /// v0.6.0：给储水罐套「桶装水可见水位」外观。
    /// - 主图：原版 water_container4/xl（64×64、4×4 格，与 water_tank 同尺寸）→ 用 SetSprite 只换图，
    ///   不重算 shape（BuildShapeFromSprite 的取样坐标有官方 bug，SetSpriteAndShape 会让 shape 不可预期）。
    /// - 液体层：自定义 getItemSpriteModifierForItemFunc，复用原版透明容器帧名，
    ///   档位前缀写死 "xl"（旧存档的 spritePath 可能还是 water_tank，不能拿它拼帧名）。
    /// 幂等：读档后存档会把这件物品的旧 spritePath 恢复回来，进塔/开窗时重套一次即可。
    /// </summary>
    private static bool ApplyTankLook(GameItem? tank)
    {
        if (tank == null)
        {
            return false;
        }

        try
        {
            bool changed = tank.spriteAtlasPath != TankBodyAtlas
                           || tank.spritePath != TankBodySprite
                           || tank.getItemSpriteModifierForItemFunc == null;
            if (tank.spriteAtlasPath != TankBodyAtlas || tank.spritePath != TankBodySprite)
            {
                tank.SetSprite(TankBodyAtlas, TankBodySprite);
            }

            tank.getItemSpriteModifierForItemFunc = BuildTankLiquidOverlay;
            return changed;
        }
        catch (Exception error)
        {
            Debug.LogError("[PurifierTower] ApplyTankLook threw:\n" + error);
            return false;
        }
    }

    /// <summary>是否是本 mod 接管了外观的储水罐（固定水箱沿用原版 identifier water_tank）。</summary>
    internal static bool IsManagedWaterTank(GameItem? item)
    {
        return item != null
               && item.getItemSpriteModifierForItemFunc != null
               && (item.identifier == "water_tank" || item.identifier == ContainerIdentifier);
    }

    /// <summary>液体层：与 WaterHelper.InitLiquidContainerItem 的透明容器分支等价，但帧前缀固定为 xl。</summary>
    private static List<ItemSpriteModifier> BuildTankLiquidOverlay(GameItem gameItem)
    {
        int percent = WaterHelper.GetCurrentCapacity(gameItem);
        if (percent <= 0)
        {
            return null!; // 与原版透明容器一致：空桶不画液体层（委托签名是非空 List）。
        }

        string level = SpriteHelper.MapCapacity(percent);
        string liquidColor = WaterHelper.GetLiquidContainerColor(gameItem);
        string colorBase = Liquid.GetColorBase(liquidColor);
        var modifiers = new List<ItemSpriteModifier>(2);
        Sprite? liquid = RenderHandler.TryLoadFromAtlas(TankBodyAtlas, TankBodySprite + "_" + colorBase + "_" + level);
        if (liquid != null)
        {
            modifiers.Add(new ItemSpriteModifier(liquid, Vector2.zero, 0, isFlipped: false, 0, haveMask: false,
                MetaHelper.HexToColor(liquidColor)));
        }

        Sprite? highlight = RenderHandler.TryLoadFromAtlas(TankBodyAtlas, TankBodySprite + "_hl_" + level);
        if (highlight != null)
        {
            modifiers.Add(new ItemSpriteModifier(highlight, Vector2.zero));
        }

        return modifiers;
    }

    /// <summary>按档位刷新液体层：容量百分比没跨档时不重建（Validate 可能被多次触发）。</summary>
    private static void RefreshTankOverlayIfNeeded(GameItem tank, RunPageUi ui, bool processed)
    {
        int bucket = int.Parse(SpriteHelper.MapCapacity(WaterHelper.GetCurrentCapacity(tank)));
        if (processed)
        {
            if (ui.ProcessedShownLevel == bucket)
            {
                return;
            }

            ui.ProcessedShownLevel = bucket;
        }
        else
        {
            if (ui.PendingShownLevel == bucket)
            {
                return;
            }

            ui.PendingShownLevel = bucket;
        }

        if (tank is GameItemElement element)
        {
            element.HandleInnateModSprite();
        }
    }

    /// <summary>最近创建的塔运行页：水箱回调只用来刷新显示。</summary>
    private static RunPageUi? LastUi;

    private static void ConfigureTankCallbacks(GameItem tank, bool takeWater, RunPageUi? ui)
    {
        if (ui != null)
        {
            LastUi = ui;
        }

        if (tank == null || tank.onItemTargetThisFunc != null)
        {
            return;
        }

        // 拖拽释放时：currentItem.Target(悬挂物)。targetItem.onItemTargetThisFunc(source, target)。
        // pending：瓶子自带 onThisTargetItemFunc（源→目标）已经能倒水进去，这里只在倾倒后刷新读数。
        // processed：空瓶拖上来时从水箱灌进瓶子（目标→源）；非空瓶则否决这次交互。
        if (takeWater)
        {
            tank.mayItemTargetThisFunc = (source, target) =>
                source != null && source.IsTag("LIQUID_CONTAINER_TAG") && WaterHelper.IsEmpty(source);
        }

        tank.onItemTargetThisFunc = delegate(GameItem source, GameItem target)
        {
            try
            {
                if (!takeWater || source == null || target == null)
                {
                    return;
                }

                if (!source.IsTag("LIQUID_CONTAINER_TAG") || !WaterHelper.IsEmpty(source))
                {
                    return;
                }

                WaterHelper.TransferLiquid(target, source);
            }
            catch (Exception error)
            {
                Debug.LogError("[PurifierTower] tank take-water threw:\n" + error);
            }
            finally
            {
                RunPageUi? targetUi = ui ?? LastUi;
                if (targetUi != null)
                {
                    RefreshRunPage(targetUi);
                }
            }
        };
    }

    private static GameItem CreatePlayerContainer()
    {
        // 显示名走 Unity Localization「Item」表（LocalizationInject 注入），缺表时用硬编码兜底。
        string displayName = LocalizationInject.GetItemName(ContainerIdentifier, "200L 水容器");
        GameItem container = ItemDirectory.CreateEmptyItem(ContainerIdentifier)
            .SetName(displayName)
            .SetSpriteAndShape(TankAtlas, TankSprite);
        container.shortDescription = displayName + "（可放入海德拉净水箱的待处理/已处理水区域）。";
        container.unitValue = 500L;
        WaterHelper.InitLiquidContainerItem(container, PlayerContainerCapacityMl, 500,
            isFilterContainer: false, isGraduated: true, isDropper: false, isTransparentContainer: true,
            sheetPage: 4);
        ApplyTankLook(container);
        AudioHelper.InitSolidMetal(container);
        return container;
    }

    // 水瓶：water_container3 的 mini/small/normal/large + water_container4 的 xl。
    // 注意 sheetPage：透明容器的液体覆盖层从 "Items/water_container" + sheetPage 取图，
    // water_container3 的瓶用 3、water_container4 的 xl（原版 water_jug）用 4。
    private static GameItem CreateBottle1L() => CreateBottle(Bottle1LIdentifier, "1L 水瓶", 1000, 4, "Items/water_container3", "mini");
    private static GameItem CreateBottle3L() => CreateBottle(Bottle3LIdentifier, "3L 水瓶", 3000, 8, "Items/water_container3", "small");
    private static GameItem CreateBottle6L() => CreateBottle(Bottle6LIdentifier, "6L 水瓶", 6000, 15, "Items/water_container3", "normal");
    private static GameItem CreateBottle20L() => CreateBottle(Bottle20LIdentifier, "20L 水桶", 20000, 45, "Items/water_container3", "large");
    private static GameItem CreateBottle60L() => CreateBottle(Bottle60LIdentifier, "60L 水桶", 60000, 120, "Items/water_container4", "xl", sheetPage: 4);

    private static GameItem CreateBottle(string identifier, string itemName, int capacityMl, int basePrice,
        string spriteAtlas, string spriteName, int sheetPage = 3)
    {
        try
        {
            // 显示名走 Unity Localization「Item」表（LocalizationInject 注入），缺表时用硬编码兜底。
            string displayName = LocalizationInject.GetItemName(identifier, itemName);
            GameItem bottle = ItemDirectory.CreateEmptyItem(identifier)
                .SetName(displayName)
                .SetSpriteAndShape(spriteAtlas, spriteName);
            LogStage($"{identifier} sprite");
            bottle.shortDescription = $"{displayName}（海德拉净水箱灌装用）。";
            bottle.unitValue = basePrice;
            WaterHelper.InitLiquidContainerItem(bottle, capacityMl, basePrice,
                isFilterContainer: false, isGraduated: false, isDropper: false, isTransparentContainer: true,
                sheetPage: sheetPage);
            AudioHelper.InitPlasticBottle(bottle);
            LogStage($"{identifier} liquid container init");
            return bottle;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PurifierTower] CreateBottle({identifier}) threw:\n{ex}");
            throw;
        }
    }

    private static GameItem? CreateBottleByCapacity(int capacityMl)
    {
        switch (capacityMl)
        {
            case 1000: return CreateBottle1L();
            case 3000: return CreateBottle3L();
            case 6000: return CreateBottle6L();
            case 20000: return CreateBottle20L();
            case 60000: return CreateBottle60L();
            default: return null;
        }
    }

    // ================= 打印 / 电量 / 读数 =================

    /// <summary>打印电耗：round(3 + 0.004 × ml)。</summary>
    public static int PrintEnergyCost(int capacityMl)
    {
        return 3 + capacityMl * 4 / 1000;
    }

    private static bool DrawEnergy(GameGridInventory batteryInv, int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        int remaining = amount;
        foreach (GameItem battery in batteryInv.childItems)
        {
            if (battery == null || !battery.IsTag("power_source_item"))
            {
                continue;
            }

            int available = PowerHelper.GetAvailableEnergyFromItem(battery);
            int take = Math.Min(available, remaining);
            if (take > 0 && PowerHelper.DrawPowerSource(battery, take))
            {
                remaining -= take;
            }

            if (remaining <= 0)
            {
                break;
            }
        }

        return remaining <= 0;
    }

    private static int TotalEnergy(GameGridInventory batteryInv)
    {
        int total = 0;
        foreach (GameItem battery in batteryInv.childItems)
        {
            if (battery != null && battery.IsTag("power_source_item"))
            {
                total += PowerHelper.GetAvailableEnergyFromItem(battery);
            }
        }

        return total;
    }

    private static int TankMilliliters(GameSlotInventory tankSlot)
    {
        GameItem? tank = tankSlot?.childItem;
        return tank == null ? 0 : WaterHelper.GetCurrentCapacityML(tank);
    }

    private static void PrintBottles(RunPageUi ui, int maxCount)
    {
        // 逐瓶打印用协程分摊到多帧：一次插入多瓶会触发整树 ValidateShapeState，容易卡主线程。
        if (GameMaster.current != null)
        {
            try
            {
                GameMaster.current.StartCoroutine(PrintBottlesRoutine(ui, maxCount));
                return;
            }
            catch (Exception error)
            {
                Debug.LogError("[PurifierTower] StartCoroutine failed, fallback to synchronous print:\n" + error);
            }
        }

        int printed = 0;
        try
        {
            while (printed < maxCount && PrintOne(ui, out _))
            {
                printed++;
            }
        }
        catch (Exception error)
        {
            Debug.LogError("[PurifierTower] PrintBottles threw:\n" + error);
        }

        RefreshRunPage(ui);
    }

    private static IEnumerator PrintBottlesRoutine(RunPageUi ui, int maxCount)
    {
        int printed = 0;
        bool stopped = false;
        while (printed < maxCount)
        {
            string hint;
            bool ok;
            try
            {
                ok = PrintOne(ui, out hint);
            }
            catch (Exception error)
            {
                Debug.LogError("[PurifierTower] PrintBottlesRoutine threw:\n" + error);
                break;
            }

            if (!ok)
            {
                SetPrintHint(ui, printed == 0 ? hint : $"已打印 {printed} 瓶 · {hint}");
                stopped = true;
                break;
            }

            printed++;
            if (printed < maxCount)
            {
                SetPrintHint(ui, $"正在打印 {printed}/{maxCount} …");
            }

            yield return null;
        }

        if (!stopped && printed > 0)
        {
            SetPrintHint(ui, $"已打印 {printed} 瓶 · {ui.SelectedCapacityMl / 1000}L/瓶");
        }

        RefreshRunPage(ui);
    }

    /// <summary>打印一瓶：选瓶 → 放进输出区 → 扣电 → 从已处理水箱灌水。失败时提示原因。</summary>
    private static bool PrintOne(RunPageUi ui, out string hint)
    {
        hint = string.Empty;
        if (ui.HasPlayerContainer)
        {
            hint = "已放置自备容器 · 打印停用";
            return false;
        }

        GameItem? tank = ui.ProcessedTankSlot?.childItem;
        if (tank == null)
        {
            hint = "已处理水箱缺失";
            return false;
        }

        int capacityMl = ui.SelectedCapacityMl;
        if (capacityMl <= 0)
        {
            hint = "未选容量";
            return false;
        }

        if (WaterHelper.GetCurrentCapacityML(tank) < capacityMl)
        {
            hint = $"已处理水不足 {capacityMl / 1000}L";
            return false;
        }

        GameItem? bottle = CreateBottleByCapacity(capacityMl);
        if (bottle == null)
        {
            hint = "瓶工厂异常";
            return false;
        }

        // 工厂产出的瓶子没有 IS_OWNED_TAG，不设所有权的话输出区/后续售卖交互都会被拦。
        GeneralHelper.SetItemOwned(bottle, true);
        if (bottle.uniqueId == 0 && PlayerStore.Instance != null)
        {
            bottle.uniqueId = PlayerStore.Instance.GetUniqueID();
        }

        if (!ui.OutputInv.UncheckedAccept(bottle))
        {
            bottle.Destroy();
            hint = "输出区已满";
            return false;
        }

        int cost = PrintEnergyCost(capacityMl);
        if (!DrawEnergy(ui.BatteryInv, cost))
        {
            ui.OutputInv.Expel(bottle);
            bottle.Destroy();
            hint = $"电量不足（需 {cost}）";
            return false;
        }

        WaterHelper.TransferLiquid(tank, bottle);
        return true;
    }

    private static void SetPrintHint(RunPageUi ui, string text)
    {
        SetTextIfChanged(ui.PrintHint, text);
    }

    /// <summary>只在文字变化时 SetText：刷新开窗/打印读数时避免反复触发文本布局。</summary>
    private static void SetTextIfChanged(MachineryText? text, string value)
    {
        if (text != null && text.GetText() != value)
        {
            text.SetText(value);
        }
    }

    private static void RefreshRunPage(RunPageUi ui)
    {
        if (ui == null || ui.PendingBar == null)
        {
            return;
        }

        UpdateTankPanel(ui, ui.PendingTankSlot, ui.PendingContainerSlot, PendingCapacityMl, ui.PendingBar,
            ui.PendingVolume, ui.PendingStatus, "空 · 拖瓶子倒水进来", "已接水", processed: false);
        UpdateTankPanel(ui, ui.ProcessedTankSlot, ui.ProcessedContainerSlot, ProcessedCapacityMl, ui.ProcessedBar,
            ui.ProcessedVolume, ui.ProcessedStatus, "空 · 拖空瓶取水", "已处理", processed: true);

        int energy = TotalEnergy(ui.BatteryInv);
        ui.PoolBar?.SetValue(Mathf.Clamp01(energy / 150f));
        SetTextIfChanged(ui.PoolText, $"合并电量 {energy}（仅打印用）");

        if (ui.HasPlayerContainer && ui.PrintHint != null && string.IsNullOrEmpty(ui.PrintHint.GetText()))
        {
            SetPrintHint(ui, "已放置自备容器 · 打印停用");
        }
    }

    private static void UpdateTankPanel(RunPageUi ui, GameSlotInventory tankSlot, GameSlotInventory containerSlot,
        int capacityMl, MachineryProgressBar bar, MachineryText volumeText, MachineryText statusText,
        string emptyHint, string readyHint, bool processed)
    {
        GameItem? tank = tankSlot?.childItem;
        int ml = tank == null ? 0 : WaterHelper.GetCurrentCapacityML(tank);
        bar?.SetValue(capacityMl <= 0 ? 0f : Mathf.Clamp01((float)ml / capacityMl));
        SetTextIfChanged(volumeText, $"{ml / 1000f:0.0} / {capacityMl / 1000} L · {ml * 100f / Mathf.Max(1, capacityMl):0.0}%");

        if (tank != null)
        {
            // v0.6.0：塔内倒水/取水/打印走 TransferLiquid，不经过原版 WaterFeatureHelper 的刷新链，
            // 所以这里按档位变化主动刷一次液体层（同档重复刷新会被跳过）。
            if (ApplyTankLook(tank))
            {
                // 读档把存档里的旧外观/旧委托覆盖回来了，清掉档位缓存强制重建液体层。
                if (processed)
                {
                    ui.ProcessedShownLevel = -1;
                }
                else
                {
                    ui.PendingShownLevel = -1;
                }
            }

            RefreshTankOverlayIfNeeded(tank, ui, processed);
        }

        GameItem? container = containerSlot?.childItem;
        if (container != null)
        {
            bool reattached = ApplyTankLook(container);
            if (reattached && container is GameItemElement containerElement)
            {
                containerElement.HandleInnateModSprite();
            }
        }

        if (statusText != null)
        {
            SetTextIfChanged(statusText, container != null
                ? $"自备容器：{container.name} · 打印停用"
                : (tank == null ? "水箱缺失" : (ml <= 0 ? emptyHint : readyHint)));
        }
    }

    // ================= 页面 =================

    private static PixelElement CreateRunPage(RunPageUi ui)
    {
        // 行1：待处理水（内置 100L 水箱 + 自备容器槽）
        ui.PendingBar = new MachineryProgressBar(0f, 96).SetColor(ColDirty);
        ui.PendingVolume = Txt("0.0 / 100 L · 0.0%");
        ui.PendingStatus = Txt("空 · 拖瓶子倒水进来");
        var pendingInfo = Box(MachineryStack.Vertical(ui.PendingVolume, ui.PendingBar, ui.PendingStatus), 128, 64);
        var pendingContent = Box(MachineryStack.Horizontal(
            Box(ui.PendingTankSlot, 64, 64),
            pendingInfo,
            Box(ui.PendingContainerSlot, 64, 64)), 328, 64);
        var pendingPanel = Panel("待处理水 · 固定 100L", pendingContent, "pending");

        // 行2：已处理水（内置 600L 水箱 + 自备容器槽）
        ui.ProcessedBar = new MachineryProgressBar(0f, 96).SetColor(ColWater);
        ui.ProcessedVolume = Txt("0.0 / 600 L · 0.0%");
        ui.ProcessedStatus = Txt("空 · 拖空瓶取水");
        var processedInfo = Box(MachineryStack.Vertical(ui.ProcessedVolume, ui.ProcessedBar, ui.ProcessedStatus), 128, 64);
        var processedContent = Box(MachineryStack.Horizontal(
            Box(ui.ProcessedTankSlot, 64, 64),
            processedInfo,
            Box(ui.ProcessedContainerSlot, 64, 64)), 328, 64);
        var processedPanel = Panel("已处理水 · 固定 600L", processedContent, "processed");

        // 行3：净水目标 / 自动水龙头（一个全宽面板，避免一行多面板宽度不一致）
        var targetGroup = new MachineryRadioGroup();
        targetGroup.Add(new MachineryRadioButton("普通 9800"));
        targetGroup.Add(new MachineryRadioButton("优质 9900", on: true));
        targetGroup.Add(new MachineryRadioButton("纯水 9990"));
        targetGroup.onChanged = index =>
        {
            if (DebugTools.Enabled)
            {
                Debug.Log($"[PurifierTower][probe] target grade = {index}");
            }
        };

        var feedGroup = new MachineryRadioGroup();
        feedGroup.Add(new MachineryRadioButton("关", on: true));
        feedGroup.Add(new MachineryRadioButton("开"));
        feedGroup.onChanged = index =>
        {
            if (DebugTools.Enabled)
            {
                Debug.Log($"[PurifierTower][probe] faucet switch = {(index == 1 ? "on" : "off")}");
            }
        };

        var switchesContent = Box(MachineryStack.Vertical(
            MachineryStack.Horizontal(Txt("目标："), MachineryStack.Horizontal(targetGroup.buttons.ToArray())),
            MachineryStack.Horizontal(Txt("水龙头："), MachineryStack.Horizontal(feedGroup.buttons.ToArray()))), 328, 52);
        var switchesPanel = Panel("净水目标 / 自动水龙头", switchesContent, "switches");

        // 行4：输出区 / 打印
        var sizeGroup = new MachineryRadioGroup();
        foreach ((int capacityMl, string label) in PrintSizes)
        {
            int captured = capacityMl;
            MachineryRadioButton button = new MachineryRadioButton(label, on: capacityMl == ui.SelectedCapacityMl,
                onChanged: on =>
                {
                    if (on)
                    {
                        ui.SelectedCapacityMl = captured;
                    }
                });
            sizeGroup.Add(button);
        }

        var printOne = new MachineryButton("打印 1 瓶", () => PrintBottles(ui, 1));
        var printAll = new MachineryButton("打满输出", () => PrintBottles(ui, int.MaxValue));
        ui.PrintHint = Txt("瓶价 + 水价 · 水来自已处理区");

        var printColumn = Box(MachineryStack.Vertical(
            Txt("打印满瓶（可直接售卖）"),
            MachineryStack.Horizontal(sizeGroup.buttons.ToArray()),
            MachineryStack.Horizontal(printOne, printAll),
            ui.PrintHint), 200, 64);

        var outputContent = Box(MachineryStack.Horizontal(
            Box(ui.OutputInv, 112, 64),
            printColumn), 328, 64);
        var outputPanel = Panel("输出区 7×4 · 打印", outputContent, "output");

        return Page(pendingPanel, processedPanel, switchesPanel, outputPanel);
    }

    private static PixelElement CreatePurifierPage(GameGridInventory purifierInv)
    {
        PixelElement grid = Box(purifierInv, 336, 288);
        var panel = Panel("净水器区 21×18",
            Box(Intake(grid), 328, 296), "purifier");

        return Page(panel, Txt("放入限制同原版机器区 · 双击净水器打开它自己的窗口"));
    }

    private static PixelElement CreateBatteryPage(GameGridInventory batteryInv, GameSlotInventory uvSlot, RunPageUi ui)
    {
        ui.PoolBar = new MachineryProgressBar(0f, 128).SetColor(ColPower);
        ui.PoolText = Txt("合并电量 0（仅打印用）");
        var batteryPanel = Panel("电池区 4×4",
            Box(MachineryStack.Horizontal(
                Box(batteryInv, 64, 64),
                Box(MachineryStack.Vertical(Txt("电池与能量列阵"), ui.PoolBar, ui.PoolText), 240, 64)), 328, 64),
            "battery");

        var uvPanel = Panel("紫外线灯",
            Box(MachineryStack.Horizontal(
                Box(uvSlot, 54, 54),
                Box(MachineryStack.Vertical(
                    Txt("放入后每次注水自动使用"),
                    Txt("也可把水瓶拖到它上面单独杀菌")), 240, 54)), 328, 54), "uv");

        var chargePanel = Panel("打印耗电 / 能量列阵",
            Box(MachineryStack.Vertical(
                Txt("耗电：1L 7 · 3L 15 · 6L 27 · 20L 83 · 60L 243"),
                Txt("能量列阵每夜 +2（商业电力 +4，需相邻、停电不充、不参与取电）")), 328, 40), "charge");

        return Page(batteryPanel, uvPanel, chargePanel);
    }

    // ================= 布局工具 =================

    private static void LogStage(string stage) =>
        Debug.Log($"[PurifierTower][factory] {stage}");

    private static MachineryText Txt(string text, int size = 6) =>
        new MachineryText(text, size, HorizontalAlignmentOptions.Left);

    private static MachineryIntake Intake(PixelElement content) => new MachineryIntake(content, vents: false);

    private static MachineryPanel Panel(string label, PixelElement content, string role) =>
        new MachineryPanel(label, content, role);

    private static MachineryPanel Panel(Sprite icon, string label, PixelElement content, string role) =>
        new MachineryPanel(label, content, role).SetLabelIcon(icon);

    /// <summary>固定尺寸的包裹盒：让面板内容拿到确定宽高，从而面板尺寸可预测。</summary>
    private static PixelElement Box(PixelElement content, int width, int height)
    {
        MachineryStack stack = MachineryStack.Vertical(content);
        stack.ResizePixels(width, height);
        return stack;
    }

    private static MachineryStack Row(int height, params MachineryPanel[] panels)
    {
        MachineryStack stack = MachineryStack.Horizontal(panels);
        stack.ResizePixels(352, height);
        return stack;
    }

    /// <summary>三页签页面统一 352×348，切页时窗口尺寸不变。</summary>
    private static PixelElement Page(params PixelElement[] elements)
    {
        MachineryStack page = MachineryStack.Vertical(elements);
        page.ResizePixels(352, 348);
        return page;
    }

    // ================= 运行时补建固定水箱 =================

    /// <summary>
    /// 塔的运行时状态：固定水箱不能在建塔工厂里创建（会被 DirectoryMaster.Item 清掉子物品），
    /// 因此在塔真正创建后或窗口打开时补建。键为塔物品的弱引用，避免常驻内存。
    /// </summary>
    private sealed class TowerRuntime
    {
        public readonly RunPageUi Ui;
        public readonly GameSlotInventory PendingTankSlot;
        public readonly GameSlotInventory ProcessedTankSlot;

        public TowerRuntime(RunPageUi ui, GameSlotInventory pendingTankSlot, GameSlotInventory processedTankSlot)
        {
            Ui = ui;
            PendingTankSlot = pendingTankSlot;
            ProcessedTankSlot = processedTankSlot;
        }

        public void EnsureInitialized()
        {
            if (SaveManager.IsDecoding)
            {
                return; // 解码时由存档里的水箱自己插回来，重复补建会丢物品。
            }

            float start = Time.realtimeSinceStartup;
            EnsureFixedTanks(PendingTankSlot, ProcessedTankSlot, Ui);
            RefreshRunPage(Ui);
            if (DebugTools.Enabled)
            {
                float ms = (Time.realtimeSinceStartup - start) * 1000f;
                Debug.Log($"[PurifierTower][trace] EnsureInitialized {ms:0} ms");
            }
        }
    }

    private static readonly ConditionalWeakTable<GameItem, TowerRuntime> Runtimes =
        new ConditionalWeakTable<GameItem, TowerRuntime>();

    /// <summary>塔物品创建完成（DirectoryMaster.Item 返回）后补建水箱并设好所有权。</summary>
    internal static void OnTowerSpawned(GameItem? item)
    {
        if (item == null || item.identifier != TowerIdentifier)
        {
            return;
        }

        try
        {
            if (Runtimes.TryGetValue(item, out TowerRuntime? runtime))
            {
                runtime.EnsureInitialized();
            }
        }
        catch (Exception error)
        {
            Debug.LogError("[PurifierTower] OnTowerSpawned threw:\n" + error);
        }
    }

    internal static bool HasRuntime(GameItem? item) =>
        item != null && Runtimes.TryGetValue(item, out _);

    /// <summary>仅 -pt-debug：对一台塔做水箱/所有权/灌装的冒烟自检，日志前缀 [smoke]。</summary>
    internal static void RunDebugSmokeTest(GameItem tower)
    {
        if (!DebugTools.Enabled || tower == null || !Runtimes.TryGetValue(tower, out TowerRuntime? runtime))
        {
            return;
        }

        try
        {
            runtime.EnsureInitialized();

            static string Report(bool ok, string what) => (ok ? "[smoke][ok] " : "[smoke][FAIL] ") + what;

            GameItem? pendingTank = runtime.PendingTankSlot.childItem;
            GameItem? processedTank = runtime.ProcessedTankSlot.childItem;
            Debug.Log(Report(pendingTank != null, "pending tank exists"));
            Debug.Log(Report(processedTank != null, "processed tank exists"));
            Debug.Log(Report(pendingTank != null && GeneralHelper.IsItemOwned(pendingTank), "pending tank owned"));
            Debug.Log(Report(processedTank != null && GeneralHelper.IsItemOwned(processedTank), "processed tank owned"));

            if (pendingTank == null || processedTank == null)
            {
                return;
            }

            Debug.Log(Report(WaterHelper.GetTotalCapacityML(pendingTank) == PendingCapacityMl,
                $"pending capacity {WaterHelper.GetTotalCapacityML(pendingTank)}"));
            Debug.Log(Report(WaterHelper.GetTotalCapacityML(processedTank) == ProcessedCapacityMl,
                $"processed capacity {WaterHelper.GetTotalCapacityML(processedTank)}"));

            int before = WaterHelper.GetCurrentCapacityML(processedTank);
            WaterHelper.AddWater(processedTank, 1, 6000);
            int after = WaterHelper.GetCurrentCapacityML(processedTank);
            Debug.Log(Report(after - before == 6000, $"add water +{after - before} ml"));

            GameItem? bottle = CreateBottleByCapacity(6000);
            if (bottle == null)
            {
                Debug.Log(Report(false, "create 6L bottle"));
                return;
            }

            try
            {
                GeneralHelper.SetItemOwned(bottle, true);
                WaterHelper.TransferLiquid(processedTank, bottle);
                Debug.Log(Report(WaterHelper.GetCurrentCapacityML(bottle) == 6000,
                    $"fill bottle {WaterHelper.GetCurrentCapacityML(bottle)} ml"));

                bool accepted = runtime.Ui.OutputInv.UncheckedAccept(bottle);
                Debug.Log(Report(accepted, "bottle accepted by output grid"));
                if (accepted)
                {
                    runtime.Ui.OutputInv.Expel(bottle);
                }

                WaterHelper.TransferLiquid(bottle, processedTank); // 水倒回去，不污染存档
            }
            finally
            {
                bottle.Destroy();
            }

            RefreshRunPage(runtime.Ui);
        }
        catch (Exception error)
        {
            Debug.LogError("[PurifierTower] RunDebugSmokeTest threw:\n" + error);
        }
    }

    /// <summary>运行页的控件引用：打印/刷新都通过它拿槽位与读数控件。</summary>
    private sealed class RunPageUi
    {
        public GameGridInventory BatteryInv = null!;
        public GameGridInventory OutputInv = null!;
        public GameSlotInventory PendingTankSlot = null!;
        public GameSlotInventory ProcessedTankSlot = null!;
        public GameSlotInventory PendingContainerSlot = null!;
        public GameSlotInventory ProcessedContainerSlot = null!;

        public MachineryProgressBar PendingBar = null!;
        public MachineryProgressBar ProcessedBar = null!;
        public MachineryProgressBar PoolBar = null!;
        public MachineryText PendingVolume = null!;
        public MachineryText ProcessedVolume = null!;
        public MachineryText PendingStatus = null!;
        public MachineryText ProcessedStatus = null!;
        public MachineryText PoolText = null!;
        public MachineryText PrintHint = null!;

        public int SelectedCapacityMl;

        // v0.6.0：上一次液体层用过的档位（MapCapacity 的 0/10/25/50/75/100），用于去重刷新。
        public int PendingShownLevel = -1;
        public int ProcessedShownLevel = -1;

        public bool HasPlayerContainer =>
            PendingContainerSlot?.childItem != null || ProcessedContainerSlot?.childItem != null;
    }
}
