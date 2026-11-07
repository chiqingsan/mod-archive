using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace XGDragFixer;

/// <summary>
/// 两项容器相关修复/优化：
/// 1) 销毁容器物品后释放原版只 Hide() 的 contentWindow GameObject；
/// 2) 搅拌机 BlendInto 的 10 批液体写入合并为每种液体 1 批。
/// 每项都可单独关闭；目标缺失只记 [兼容] 日志，异常时回退原版路径。
/// </summary>
internal static class ContainerFixes
{
    internal static bool WindowLeakEnabled => Core.WindowLeakFix?.Value == true;

    internal static bool BlendBatchEnabled => Core.BlendBatch?.Value == true;

    internal static void Install(Harmony harmony)
    {
        var ok = 0;

        void Try(string label, MethodBase? target, string? prefix = null, string? postfix = null)
        {
            if (target == null)
            {
                Core.Log?.Log($"[兼容] 未找到 {label}，跳过该项");
                return;
            }

            try
            {
                harmony.Patch(target,
                    prefix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(ContainerFixes), prefix)),
                    postfix == null ? null : new HarmonyMethod(AccessTools.Method(typeof(ContainerFixes), postfix)));
                ok++;
            }
            catch (Exception ex)
            {
                Core.Log?.Warning($"{label} 补丁失败: {ex.Message}");
            }
        }

        // 1) 原版 GameItemElement.Destroy() 只对 contentWindow 调 Hide()，窗口 GameObject 及其
        //    子树不断累积；用户复测无 Mod 新开局：销毁 60 个空啤酒箱后对象数 6177 → 7317。
        Try("容器窗口泄漏清理",
            AccessTools.Method(typeof(GameItemElement), nameof(GameItemElement.Destroy), Type.EmptyTypes),
            postfix: nameof(ReleaseWindowPostfix));

        // 2) 原版 BlendInto 对每种液体分 10 批调用 AddLiquid（每批都做温度混合、水特征与
        //    sprite 刷新）；内容物不变，只是把同一份量合并为一次写入。
        Try("搅拌机液体合并",
            AccessTools.Method(typeof(ChemicalHelper), nameof(ChemicalHelper.BlendInto),
                new[] { typeof(GameItem), typeof(GameItem), typeof(int) }),
            prefix: nameof(BlendIntoPrefix));

        Core.Log?.Log($"[优化] 容器修复优化已挂接 {ok} 项");
    }

    private static void ReleaseWindowPostfix(GameItemElement __instance)
    {
        if (!WindowLeakEnabled || __instance == null)
        {
            return;
        }

        try
        {
            var window = __instance.contentWindow;
            if (window == null)
            {
                return;
            }

            // PixelWindow 不是 MonoBehaviour：窗口 UI 树挂在 handler（TreeNodeRender）下。
            var handler = window.handler;
            if (handler == null)
            {
                return;
            }

            var gameObject = handler.gameObject;
            if (gameObject == null)
            {
                return;
            }

            // 窗口里仍有内容物时保持原版（只隐藏），避免连带销毁子物品的渲染对象。
            var children = GraphUtils.FindChildrenType<GameItem>(window);
            if (children != null && children.Count > 0)
            {
                return;
            }

            UnityEngine.Object.Destroy(gameObject);
            Diagnostics.Count("窗口泄漏: 已释放");
        }
        catch (Exception ex)
        {
            Core.Log?.Warning($"容器窗口释放失败: {ex.Message}");
        }
    }

    private static bool BlendIntoPrefix(GameItem chemicalItem, GameItem liquidContainer, int loss)
    {
        if (!BlendBatchEnabled || chemicalItem == null || liquidContainer == null)
        {
            return true;
        }

        if (!TryBuildPlan(chemicalItem, liquidContainer, loss, out var releases))
        {
            // 容量不足或没有可写入液体：保持原版分批截断/空转行为。
            return true;
        }

        var started = Diagnostics.Begin();
        Exception? error = null;
        try
        {
            foreach (var release in releases)
            {
                WaterHelper.AddLiquid(liquidContainer, release.Key.identifier, release.Value);
            }

            WaterHelper.RemoveExcessLiquid(liquidContainer);
            chemicalItem.Destroy();
        }
        catch (Exception ex)
        {
            // 已写入的液体不再重复添加，本次也不回退原版。
            error = ex;
            Core.Log?.Warning($"搅拌机合并写入失败，本次跳过原版: {ex.Message}");
        }
        finally
        {
            Diagnostics.End(Diagnostics.Probe.Blend, started, error);
        }

        return false;
    }

    /// <summary>
    /// 按原版取整规则计算每种液体的保留量；总量超过容器剩余容量时返回 false，
    /// 由原版按批截断（避免合并写入改变容量临界时的液体组成）。
    /// </summary>
    private static bool TryBuildPlan(GameItem chemicalItem, GameItem liquidContainer, int loss,
        out List<KeyValuePair<Liquid, int>> releases)
    {
        releases = new List<KeyValuePair<Liquid, int>>();
        var factor = 1.0 - Math.Max(0, Math.Min(100, loss)) / 100.0;
        var total = 0;
        foreach (var liquid in Liquid.Liquids)
        {
            var amount = (int)Math.Round(chemicalItem.GetTagReadonly(liquid.currentPart).GetInt() * factor,
                MidpointRounding.AwayFromZero);
            if (amount <= 0)
            {
                continue;
            }

            releases.Add(new KeyValuePair<Liquid, int>(liquid, amount));
            total += amount;
        }

        if (releases.Count == 0)
        {
            return true;
        }

        return total <= WaterHelper.GetFreeCapacity(liquidContainer);
    }
}
