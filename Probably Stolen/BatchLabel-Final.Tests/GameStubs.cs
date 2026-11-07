// 测试替身：只镜像本 mod 用到的游戏类型/成员，签名与 0.46D 反编译一致
// （ItemFeature.CS 的 SetExposed、LabelerHelper.IsEditableLabelFeature 等）。
// 本工程不加载游戏 DLL，只验证 LabelBatch 的纯逻辑；真实游戏行为仍需实机验证。
public enum FeatureType
{
    Normale = 0,
    Event,
    Special,
    Temporary,
    TemporaryBuying,
    TemporarySelling,
    ClientTemporary
}

public sealed class ItemCondition
{
    public string display = "";
    public string identifier = "";
    public string? onActivateActionId;
    public Action<GameItem>? onActivate;

    public bool HasOnActivateAction() => !string.IsNullOrEmpty(onActivateActionId);

    public void ExecuteOnActivateAction(GameItem item)
    {
        onActivate?.Invoke(item);
    }
}

public static class ItemConditionList
{
    public static readonly Dictionary<string, List<ItemCondition>> conditionLookup = new();
}

public sealed class ItemFeature
{
    public FeatureType featureType = FeatureType.Normale;
    public string? category;
    public string? identifier;
    public ItemCondition? fakeCondition;
    public ItemCondition? realCondition;
    public bool useCondition = true;
    public int customIntValue1;
    public GameItem? parentItem;

    private bool isFeatureExposed;

    public int ExposeCount { get; private set; }

    public bool IsExposed() => isFeatureExposed;

    // 镜像 ItemFeature.cs:371：未揭穿返回假条件，已揭穿返回真实条件。
    public ItemCondition? GetCurrentItemCondition() => isFeatureExposed ? realCondition : fakeCondition;

    public void SetExposed(bool value)
    {
        ExposeCount++;
        isFeatureExposed = value;
        if (!useCondition)
        {
            return;
        }

        ItemCondition? condition = isFeatureExposed ? realCondition : fakeCondition;
        if (condition != null && condition.HasOnActivateAction() && parentItem != null)
        {
            condition.ExecuteOnActivateAction(parentItem);
        }
    }
}

public abstract class GameItem
{
    public string identifier = "";
    public List<ItemFeature> itemFeatures = new();

    private readonly HashSet<string> tags = new();

    public bool IsTag(string tagName) => tags.Contains(tagName);

    public void EnableTag(string tagName) => tags.Add(tagName);
}

public static class GeneralHelper
{
    // 镜像 GeneralHelper.cs:188：物品归属由 IS_OWNED_TAG 标记表示。
    public static bool IsItemOwned(GameItem item) => item.IsTag("IS_OWNED_TAG");
}

public class GameItemElement : GameItem
{
}

public static class LabelerHelper
{
    public const string LABELER_TAG = "LABELER_TAG";

    public static bool CanEditLabel(ItemFeature itemFeature)
    {
        if (itemFeature.category == "CATEGORY_INGOT_PURITY")
        {
            return false;
        }
        if (itemFeature.category == "CATEGORY_EQUIPMENT_CONDITION")
        {
            return false;
        }
        if (itemFeature.category == "CATEGORY_WINE_QUALITY")
        {
            return false;
        }
        return true;
    }

    public static bool IsEditableLabelFeature(ItemFeature itemFeature)
    {
        if (itemFeature.featureType != FeatureType.Normale)
        {
            return false;
        }
        if (itemFeature.category == null)
        {
            return false;
        }
        if (!ItemConditionList.conditionLookup.ContainsKey(itemFeature.category))
        {
            return false;
        }
        return CanEditLabel(itemFeature);
    }

    public static int GetEditableLabelCount(GameItem item)
    {
        int count = 0;
        foreach (ItemFeature itemFeature in item.itemFeatures)
        {
            if (IsEditableLabelFeature(itemFeature))
            {
                count++;
            }
        }
        return count;
    }
}
