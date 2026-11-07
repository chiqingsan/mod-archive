using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace PurifierTower;

/// <summary>
/// 净水箱 mod 入口（v0.5.0）。
/// 目标（按 DESIGN v0.8 与 m03076/v0.4.0 需求）：
///   1. 通过 Harmony Postfix FoodItemDirectory.InitDirectory 注册 purifier_tower、200L 自备水容器与五种容量水瓶；
///   2. 读取存档后（OnGameLoadedLate）在启动参数 -pt-debug 时发放一台净水箱到主库存（invElement）；
///   3. 塔的 onCycleEndLateSlotItemFunc 每夜末段打日志，验证夜结回调生效；
///   4. 塔窗口为三页签：运行（待处理 100L 固定水箱/已处理 600L 固定水箱 + 自备容器槽、目标档、接水开关、
///      输出区、满瓶打印）、净水器区 21×18（放入限制同原版机器区）、电池区 4×4 + UV 槽；
///   5. 把物品名注入游戏 Unity Localization「Item」表（item_&lt;id&gt;_name），
///      物品管理器据此按中文名搜索/显示，游戏内也走同一来源。
/// 不实现净化、接水逻辑（M1+）。发布版没有 -pt-debug 时只注册物品，不发测试道具。
/// </summary>
public sealed class PurifierTowerMod : IMod
{
    public static PurifierTowerMod? Instance { get; private set; }

    private static bool grantLogged;

    public void Init(ModManifest manifest)
    {
        Instance = this;
        Debug.Log($"[PurifierTower] Init id={manifest.ID} version={manifest.ModVersion}");
    }

    public void OnEnable()
    {
        DebugTools.Init();
        // 物品名注入游戏「Item」本地化表：物品管理器与游戏都按 item_<id>_name 解析名称。
        LocalizationInject.TryInject();
        ModHook.OnGameLoadedEarly += OnGameLoadedEarly;
        ModHook.OnGameLoadedNormal += OnGameLoadedNormal;
        ModHook.OnGameLoadedLate += OnGameLoadedLate;
        TryRegisterIntoExistingDirectory();
        Debug.Log("[PurifierTower] OnEnable: subscribed OnGameLoadedEarly/Normal/Late");
    }

    private void OnGameLoadedEarly()
    {
        LocalizationInject.TryInject();
    }

    private void OnGameLoadedNormal()
    {
        LocalizationInject.TryInject();
    }

    /// <summary>
    /// 兜底注册：如果 mod 在 FoodItemDirectory.Awake 之后才加载（补丁来不及生效），
    /// 这里直接找到已初始化的目录实例并注册物品。
    /// 逐个检查 6 个 id，避免「塔已注册但瓶子缺失」时提前 return、漏补瓶子。
    /// </summary>
    private static void TryRegisterIntoExistingDirectory()
    {
        try
        {
            if (AllItemsRegistered())
            {
                Debug.Log("[PurifierTower] items already registered via InitDirectory patch");
                return;
            }

            FoodItemDirectory? directory = UnityEngine.Object.FindObjectOfType<FoodItemDirectory>();
            if (directory == null)
            {
                Debug.Log("[PurifierTower] FoodItemDirectory not created yet; rely on InitDirectory patch");
                return;
            }

            TowerFactory.RegisterInto(directory);
            Debug.Log("[PurifierTower] items registered into existing FoodItemDirectory");
        }
        catch (Exception error)
        {
            Debug.LogException(error);
        }
    }

    private static bool AllItemsRegistered()
    {
        return DirectoryMaster.Has<GameItem>(TowerFactory.TowerIdentifier)
               && DirectoryMaster.Has<GameItem>(TowerFactory.ContainerIdentifier)
               && DirectoryMaster.Has<GameItem>(TowerFactory.Bottle1LIdentifier)
               && DirectoryMaster.Has<GameItem>(TowerFactory.Bottle3LIdentifier)
               && DirectoryMaster.Has<GameItem>(TowerFactory.Bottle6LIdentifier)
               && DirectoryMaster.Has<GameItem>(TowerFactory.Bottle20LIdentifier)
               && DirectoryMaster.Has<GameItem>(TowerFactory.Bottle60LIdentifier);
    }

    public void OnDisable()
    {
        ModHook.OnGameLoadedEarly -= OnGameLoadedEarly;
        ModHook.OnGameLoadedNormal -= OnGameLoadedNormal;
        ModHook.OnGameLoadedLate -= OnGameLoadedLate;
        ProbePatches.Uninstall();
        Debug.Log("[PurifierTower] OnDisable");
    }

    private void OnGameLoadedLate()
    {
        try
        {
            // 部分 Unity Localization 表可能到读档末尾才同步可用，这里再补一次注入（幂等）。
            LocalizationInject.TryInject();
            // 若 OnEnable 时目录还没创建、补丁也没赶上，这里再补一次注册。
            TryRegisterIntoExistingDirectory();
            Debug.Log($"[PurifierTower] load-late: towerRegistered={DirectoryMaster.Has<GameItem>(TowerFactory.TowerIdentifier)}");

            if (!DebugTools.Enabled)
            {
                return;
            }

            ProbePatches.Install();
            GrantTowerOnce();
        }
        catch (Exception error)
        {
            Debug.LogException(error);
        }
    }

    private static void GrantTowerOnce()
    {
        if (PlayerStore.Instance == null)
        {
            Debug.LogWarning("[PurifierTower] PlayerStore.Instance is null, skip grant");
            return;
        }

        if (PlayerStore.Instance.FindAllItem().Any(item => item.identifier == TowerFactory.TowerIdentifier))
        {
            Debug.Log("[PurifierTower] tower already exists, skip grant");
            return;
        }

        GameItem tower = DirectoryMaster.Item(TowerFactory.TowerIdentifier, isOwned: true);
        if (tower == null)
        {
            Debug.LogError("[PurifierTower] DirectoryMaster.Item returned null (registration failed?)");
            return;
        }

        // GraphUtils.TryAcceptAll 的返回值语义不直观（实测为「未放进去的剩余量」），
        // 这里以 item.parentInventory 是否非空作为「已放置」的可靠判据。
        bool placed = false;
        if (EmporiumEntry.Instance != null && EmporiumEntry.Instance.invElement != null)
        {
            GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, tower);
            placed = tower.parentInventory != null;
        }

        if (!placed)
        {
            Debug.LogWarning("[PurifierTower] main inventory has no room, fallback to selling table");
            PlayerStore.Instance.AddDirectSellingItemToTable(tower, isOwend: true);
            placed = tower.parentInventory != null;
        }

        if (!placed)
        {
            Debug.LogError("[PurifierTower] failed to place tower, destroying probe item");
            tower.Destroy();
            return;
        }

        Debug.Log($"[PurifierTower] tower placed at inventory '{tower.parentInventory?.GetType().Name ?? "null"}'"
                  + $" id={tower.parentInventory?.identifier ?? "?"}");

        if (!grantLogged)
        {
            grantLogged = true;
            ProbeLog.LogTowerState(tower);
        }
        Debug.Log("[PurifierTower] tower granted to player");
        SelfTestFactories();
        // -pt-debug：对刚发放的塔跑一次水箱/所有权/灌装冒烟自检。
        TowerFactory.RunDebugSmokeTest(tower);
    }

    /// <summary>
    /// 读档后再创建一次所有物品（并把 1L 瓶放进背包），
    /// 既给玩家提供测试道具，也复现「UI 就绪后工厂调用」的场景以便抓异常栈。
    /// </summary>
    private static void SelfTestFactories()
    {
        if (!DebugTools.Enabled)
        {
            return;
        }

        foreach (string identifier in new[]
                 {
                     TowerFactory.TowerIdentifier,
                     TowerFactory.ContainerIdentifier,
                     TowerFactory.Bottle1LIdentifier,
                     TowerFactory.Bottle3LIdentifier,
                     TowerFactory.Bottle6LIdentifier,
                     TowerFactory.Bottle20LIdentifier,
                     TowerFactory.Bottle60LIdentifier
                 })
        {
            GameItem? item = null;
            try
            {
                item = DirectoryMaster.Item(identifier, isOwned: true);
                if (item == null)
                {
                    Debug.LogError($"[PurifierTower][selftest] {identifier} -> null");
                    continue;
                }

                Debug.Log($"[PurifierTower][selftest] {identifier} ok shape={item.shape.width}x{item.shape.height}");

                // 瓶子留给玩家试贴图/灌装：优先主库存，其次后柜台；塔仍只保留发放的那一台。
                if (identifier != TowerFactory.TowerIdentifier && EmporiumEntry.Instance != null)
                {
                    if (EmporiumEntry.Instance.invElement != null)
                    {
                        GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, item);
                    }

                    if (item.parentInventory == null && EmporiumEntry.Instance.backInvinvElement != null)
                    {
                        GraphUtils.TryAcceptAll(EmporiumEntry.Instance.backInvinvElement, item);
                    }

                    if (item.parentInventory != null)
                    {
                        Debug.Log($"[PurifierTower][selftest] {identifier} placed for testing");
                        item = null;
                    }
                }
            }
            catch (Exception error)
            {
                Debug.LogError($"[PurifierTower][selftest] {identifier} threw:\n{error}");
            }
            finally
            {
                if (item != null)
                {
                    item.Destroy();
                }
            }
        }
    }
}

[HarmonyPatch(typeof(FoodItemDirectory), "InitDirectory")]
internal static class RegisterTowerItemsPatch
{
    private static void Postfix(FoodItemDirectory __instance)
    {
        try
        {
            TowerFactory.RegisterInto(__instance);
            Debug.Log("[PurifierTower] item registration postfix finished");
        }
        catch (Exception error)
        {
            Debug.LogException(error);
        }
    }
}

/// <summary>
/// 塔被真正创建出来（含物品管理器/控制台/存档解码）后的补建入口。
/// 固定水箱不能在工厂里创建——DirectoryMaster.Item 会 Destroy 工厂产物的所有子物品
/// （见 DirectoryMaster.cs:246 的警告），所以这里在创建完成后补建并设所有权。
/// </summary>
[HarmonyPatch(typeof(DirectoryMaster), "Item", new[] { typeof(string), typeof(bool) })]
internal static class TowerSpawnedPatch
{
    private static void Postfix(GameItem __result)
    {
        try
        {
            TowerFactory.OnTowerSpawned(__result);
        }
        catch (Exception error)
        {
            Debug.LogException(error);
        }
    }
}

/// <summary>
/// v0.6.0：储水罐液体层刷新。原版 SpriteHelper.UpdateWaterContainerSprite 只认白名单里的水瓶，
/// 对 water_tank 是 no-op；塔内倒水/取水/打印走的是 WaterHelper.TransferLiquid，不经过
/// WaterFeatureHelper 的刷新链，所以在原版方法后补一刀：本 mod 接管的水罐主动重建液体层。
/// 未接管（原版水罐/普通水瓶）时直接跳过。
/// </summary>
[HarmonyPatch(typeof(SpriteHelper), "UpdateWaterContainerSprite")]
internal static class TankWaterLevelRefreshPatch
{
    private static void Postfix(GameItem gameItem)
    {
        try
        {
            if (TowerFactory.IsManagedWaterTank(gameItem) && gameItem is GameItemElement element)
            {
                element.HandleInnateModSprite();
            }
        }
        catch (Exception error)
        {
            Debug.LogException(error);
        }
    }
}

internal static class ProbeLog
{
    public static void LogTowerState(GameItem tower)
    {
        Debug.Log($"[PurifierTower][probe] id={tower.identifier} shape={tower.shape.width}x{tower.shape.height} window={(tower.contentWindow != null)}");
        // 说明：GetBodyStorage 返回的是「页签内所有库存展平后」的列表，与面板顺序不完全对应，
        // 这里只用来确认窗口结构存在，不把它当电池槽/模块槽索引使用。
        for (int i = 0; i < 16; i++)
        {
            GraphNodeStorage? storage = MachineHelper.GetBodyStorage(tower, i);
            if (storage == null)
            {
                Debug.Log($"[PurifierTower][probe] storage[{i}] = null");
                continue;
            }

            GameInventory? inventory = storage as GameInventory;
            string children = inventory == null ? "?" : inventory.childItems.Count.ToString();
            Debug.Log($"[PurifierTower][probe] storage[{i}] = {storage.GetType().Name} children={children}");
        }
    }
}
