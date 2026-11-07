using System;

namespace QuickSell;

internal static class SelectionRules
{
    // -1 excludes an item, 0 is normal, 1 is low priority.
    internal static int RiskRank(bool isStolen, bool isContraband, bool security, string stolenRule, string contrabandRule)
    {
        if ((isStolen && (security || stolenRule == "不自动上货")) || (isContraband && contrabandRule == "不自动上货")) return -1;
        return (isStolen && stolenRule == "低优先级") || (isContraband && contrabandRule == "低优先级") ? 1 : 0;
    }

    internal static long Available(long total, bool keepOne) => Math.Max(0, total - (keepOne ? 1 : 0));

    internal static bool HasSource(bool mainInventory, int containerCount) => mainInventory || containerCount > 0;
    // 商品包装容器（试剂盒、净水片盒、营养片盒、啤酒箱等带 PRE_CONTAINER_TAG 的物品）
    // 即使带内容窗口也允许作为货源：先算内部物品，内部为空时容器本身也可参与上架。
    internal static bool IsSourceCandidate(bool fromMainInventory, bool hasContentWindow, bool packagingContainer = false)
        => !fromMainInventory || !hasContentWindow || packagingContainer;

    internal static bool RetryAfterEmptyContainer(int moved, bool newlyEmpty, int pass, int maxPasses)
        => moved > 0 && newlyEmpty && pass + 1 < maxPasses;
}
